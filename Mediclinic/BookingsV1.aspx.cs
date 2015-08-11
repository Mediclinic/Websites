using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;


public partial class BookingsV1 : System.Web.UI.Page
{

    #region classes: BookingSlotMinsCache, DayData, OrgDayData

    protected class BookingSlotMinsCache
    {
        public BookingSlotMinsCache(int orgID)
        {
            this.orgID = orgID;
        }


        protected int orgID = -1;
        protected int slotMins = -1;  // cache for less db access
        public int GetSlotMins()
        {
            // cached
            if (this.slotMins != -1)
                return this.slotMins;


            SystemVariables sysVariables = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"]);
            int BookingSheetTimeSlotMins_Clinic   = Convert.ToInt32(sysVariables["BookingSheetTimeSlotMins_Clinic"].Value);
            int BookingSheetTimeSlotMins_AgedCare = Convert.ToInt32(sysVariables["BookingSheetTimeSlotMins_AgedCare"].Value);
            int BookingSheetTimeSlotMins_GP       = Convert.ToInt32(sysVariables["BookingSheetTimeSlotMins_GP"].Value);


            Organisation org = OrganisationDB.GetByID(this.orgID);
            if (org == null)
            {
                if (Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteTypeID"]) == 1)
                    this.slotMins = BookingSheetTimeSlotMins_Clinic;
                else if (Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteTypeID"]) == 2)
                    this.slotMins = BookingSheetTimeSlotMins_AgedCare;
                else if (Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteTypeID"]) == 3)
                    this.slotMins = BookingSheetTimeSlotMins_GP;
                else
                    throw new CustomMessageException("Site Is Not Clinic, AC or GP");

                return this.slotMins;
            }


            if (org.OrganisationType.OrganisationTypeID == 218)  // clinic or gp
            {
                if (Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteTypeID"]) == 1)
                    this.slotMins = BookingSheetTimeSlotMins_Clinic;
                else if (Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteTypeID"]) == 3)
                    this.slotMins = BookingSheetTimeSlotMins_GP;
                else
                {
                    Site[] allSites = SiteDB.GetAll();
                    Site clinicSite = SiteDB.GetSiteByType(SiteDB.SiteType.Clinic, allSites);
                    Site gpSite     = SiteDB.GetSiteByType(SiteDB.SiteType.Clinic, allSites);

                    bool useACSlotMins = (clinicSite == null || BookingSheetTimeSlotMins_Clinic == BookingSheetTimeSlotMins_AgedCare) &&
                                         (gpSite     == null || BookingSheetTimeSlotMins_GP     == BookingSheetTimeSlotMins_AgedCare);
                    if (useACSlotMins)
                        this.slotMins = BookingSheetTimeSlotMins_AgedCare;
                    else
                        throw new CustomMessageException("You Are Logged Into AC But Vewing Bookings For A Clinic. <br />Please Switch To Clinics Site.", true);
                }
            }
            else
                this.slotMins = BookingSheetTimeSlotMins_AgedCare;

            return this.slotMins;
        }
        public void ClearSlotMinsCache()
        {
            slotMins = -1;
        }

    }
    protected class DayData
    {
        public DateTime Date;
        public OrgDayData[] OrgDayData;

        public DayData(DateTime Date, OrgDayData[] OrgDayData)
        {
            this.Date = Date;
            this.OrgDayData = OrgDayData;
        }

        public int NCols
        {
            get
            {
                int ncols = 0;
                foreach (OrgDayData o in OrgDayData)
                    foreach (Staff s in o.StaffList)
                        ncols++;
                return ncols;
            }
        }
        public int NOrgsWithCols
        {
            get
            {
                int ncols = 0;
                foreach (OrgDayData o in OrgDayData)
                    if (o.NCols > 0)
                        ncols++;
                return ncols;
            }
        }
    }
    protected class OrgDayData
    {
        public Organisation Org;
        public Staff[] StaffList;

        public OrgDayData(Organisation Org, Staff[] StaffList)
        {
            this.Org = Org;
            this.StaffList = StaffList;
        }

        public int NCols
        {
            get
            {
                int ncols = 0;
                foreach (Staff s in this.StaffList)
                    ncols++;
                return ncols;
            }
        }
    }

    #endregion

    #region Globals

    protected BookingSlotMinsCache bookingSlotMinsCache = null;
    protected int columnWidth = 163;  //160

    protected Organisation[] orgs = null; // keep for cache
    protected int[] orgIDs
    {
        get
        {
            int[] ret = new int[this.orgs.Length];
            for (int i = 0; i < this.orgs.Length; i++)
                ret[i] = this.orgs[i].OrganisationID;
            return ret;
        }
    }
    protected Staff[] staffList
    {
        get
        {
            Hashtable staffHash = new Hashtable();
            foreach (DayData dd in this.daysData)
                foreach (OrgDayData odd in dd.OrgDayData)
                    foreach (Staff s in odd.StaffList)
                        if (s != null && !staffHash.Contains(s.StaffID))
                            staffHash[s.StaffID] = s;
            Staff[] staffList = new Staff[staffHash.Count];
            staffHash.Values.CopyTo(staffList, 0);

            return staffList;
        }
    }

    protected DayData[] daysData = null;

    protected int NumColumns
    {
        get
        {
            int nCols = 0;
            for (int i = 0; i < this.daysData.Length; i++)
                for (int j = 0; j < this.daysData[i].OrgDayData.Length; j++)
                    for (int k = 0; k < this.daysData[i].OrgDayData[j].StaffList.Length; k++)
                        nCols++;

            return nCols;
        }
    }
    protected int DaySeperatorColumnWidth = 1;

    protected Booking[] bookings = null;

    public void SetGlobalVariables()
    {
        // get list of orgs
        int[] orgIDs = GetFormOrgs();
        //this.orgs = new Organisation[orgIDs.Length];

        // EDIT: changed to show all offerings to all clinics (only change the price if they set a price on invoiceing in the org-offering table)
        //
        // only show orgs that offer this offering -- or if editing booking and offering not offered by that org, still show the org
        ArrayList orgsList = new ArrayList();
        int offeringID = IsValidFormOffering() ? GetFormOffering() : -1;
        Booking editBooking = IsEditBookingMode() && IsValidFormBooking() ? BookingDB.GetByID(GetFormBooking()) : null;
        for (int i = 0; i < orgIDs.Length; i++)
        {
            //if (offeringID == -1 || OrganisationOfferingsDB.IsOfferingOfferedyByOrg(offeringID, orgIDs[i]) || (editBooking != null && editBooking.Organisation.OrganisationID == orgIDs[i]))
            orgsList.Add(OrganisationDB.GetByID(orgIDs[i]));
        }
        this.orgs = (Organisation[])orgsList.ToArray(typeof(Organisation));

        // foreach day:
        //   foreach org:
        //       add  date:[orgs:[staff]]

        if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]))
            Logger.LogQuery("START TIME - GET STAFF WORKING AT ALL BOOKING PAGE ORGS");

        DataTable tblAllRegisteredStaff = RegisterStaffDB.GetDataTable_All();
        DataTable tblUnavailableProvBookingsFullDays = BookingDB.GetUnavailableProvBookingsBetween(orgIDs, GetFormDate(), GetFormDate().AddDays(GetFormNumDays()));



        //
        // need to get the staff list before getting the bookings because when we get bookings with 'staff' put in, 
        // we get where staff are booked other organisations, which needs to be shown
        // 

        Hashtable allStaff = new Hashtable();
        DateTime day = GetFormDate();
        for (int i = 0; i < GetFormNumDays(); day = day.AddDays(1), i++)
        {
            for (int j = 0; j < this.orgs.Length; j++)
            {
                StartEndTime orgStartEndTime = this.orgs[j].GetStartEndTime(day.DayOfWeek);
                bool orgOpen = ((this.orgs[j].StartDate == DateTime.MinValue || this.orgs[j].StartDate <= day) &&
                                (this.orgs[j].EndDate   == DateTime.MinValue || this.orgs[j].EndDate   >= day) &&
                                (this.orgs[j].IsOpen(day.DayOfWeek)) &&
                                (orgStartEndTime.StartTime < orgStartEndTime.EndTime));

                Staff[] workingStaff = GetWorkingStaffFor(orgs[j].OrganisationID, true, day, null, tblAllRegisteredStaff, tblUnavailableProvBookingsFullDays, GetFormShowUnavailableStaff());

                for (int k = 0; k < workingStaff.Length; k++)
                    allStaff[workingStaff[k].StaffID] = workingStaff[k];
            }
        }
        Staff[] staff = new Staff[allStaff.Values.Count];
        allStaff.Values.CopyTo(staff, 0);


        // get all bookings for these orgs between the dates -- irregardless of provider
        // so we can add providers who have bookings before but are no longer registered to this clinic or are no longer set as a provider
        Booking[] bookingsTheseOrgs = BookingDB.GetBetween(debugPageLoadTime, GetFormDate(), GetFormDate().AddDays(GetFormNumDays()), staff, this.orgs, null, null);
        this.bookings = bookingsTheseOrgs; // save to use results again when displaying page...

        this.daysData = new DayData[GetFormNumDays()];
        day = GetFormDate();
        for (int i = 0; i < GetFormNumDays(); day = day.AddDays(1), i++)
        {
            OrgDayData[] orgDayData = new OrgDayData[this.orgs.Length];
            for (int j = 0; j < this.orgs.Length; j++)
            {
                StartEndTime orgStartEndTime = this.orgs[j].GetStartEndTime(day.DayOfWeek);
                bool orgOpen = ((this.orgs[j].StartDate == DateTime.MinValue || this.orgs[j].StartDate <= day) &&
                                (this.orgs[j].EndDate   == DateTime.MinValue || this.orgs[j].EndDate   >= day) &&
                                (this.orgs[j].IsOpen(day.DayOfWeek)) &&
                                (orgStartEndTime.StartTime < orgStartEndTime.EndTime));

                //orgDayData[j] = new OrgDayData(this.orgs[j], RegisterStaffDB.GetWorkingStaffOf(orgs[j].OrganisationID, day));   // THIS IS VERY SLOW, SO CREATED BELOW LINE
                orgDayData[j] = new OrgDayData(this.orgs[j], GetWorkingStaffFor(orgs[j].OrganisationID, orgOpen, day, bookingsTheseOrgs, tblAllRegisteredStaff, tblUnavailableProvBookingsFullDays, GetFormShowUnavailableStaff()));

                if (orgDayData[j].NCols == 0)  // need to put in blank column if no staff allocated
                    orgDayData[j].StaffList = new Staff[] { null };
            }
            this.daysData[i] = new DayData(day, orgDayData);
        }



        // if there is a booking for a staff member who is now set as not working on this day at this org:
        // then this staff memeber will be showing (as it shows staff who have 'existing' bookings but not set as working that day), but 
        // the last code will not get bookings for other orgs for this staff memmber
        // so this checks for those staff, and if so, has to re-get the list of bookings including for that staff memeber
        //
        // most of the time this should not be called, so the db lookups will stay minimal
        if (staff.Length < this.staffList.Length)
        {
            bookingsTheseOrgs = BookingDB.GetBetween(debugPageLoadTime, GetFormDate(), GetFormDate().AddDays(GetFormNumDays()), this.staffList, this.orgs, null, null);
            this.bookings = bookingsTheseOrgs; // save to use results again when displaying page...

            this.daysData = new DayData[GetFormNumDays()];
            day = GetFormDate();
            for (int i = 0; i < GetFormNumDays(); day = day.AddDays(1), i++)
            {
                OrgDayData[] orgDayData = new OrgDayData[this.orgs.Length];
                for (int j = 0; j < this.orgs.Length; j++)
                {
                    StartEndTime orgStartEndTime = this.orgs[j].GetStartEndTime(day.DayOfWeek);
                    bool orgOpen = ((this.orgs[j].StartDate == DateTime.MinValue || this.orgs[j].StartDate <= day) &&
                                    (this.orgs[j].EndDate   == DateTime.MinValue || this.orgs[j].EndDate   >= day) &&
                                    (this.orgs[j].IsOpen(day.DayOfWeek)) &&
                                    (orgStartEndTime.StartTime < orgStartEndTime.EndTime));

                    //orgDayData[j] = new OrgDayData(this.orgs[j], RegisterStaffDB.GetWorkingStaffOf(orgs[j].OrganisationID, day));   // THIS IS VERY SLOW, SO CREATED BELOW LINE
                    orgDayData[j] = new OrgDayData(this.orgs[j], GetWorkingStaffFor(orgs[j].OrganisationID, orgOpen, day, bookingsTheseOrgs, tblAllRegisteredStaff, tblUnavailableProvBookingsFullDays, GetFormShowUnavailableStaff()));

                    if (orgDayData[j].NCols == 0)  // need to put in blank column if no staff allocated
                        orgDayData[j].StaffList = new Staff[] { null };
                }
                this.daysData[i] = new DayData(day, orgDayData);
            }
        }


        if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]))
            Logger.LogQuery("END TIME - GET STAFF WORKING AT ALL BOOKING PAGE ORGS");

        this.bookingSlotMinsCache = new BookingSlotMinsCache(this.orgs.Length == 0 ? 0 : this.orgs[0].OrganisationID);
    }


    protected Staff[] GetWorkingStaffFor(int orgID, bool orgOpen, DateTime date, Booking[] bookings, DataTable tblAllRegisteredStaff, DataTable tblUnavailableProvBookingsFullDays, bool showUnavailableStaff)
    {
        UserView userView              = UserView.GetInstance();
        int      loggedInStaffID       = userView.IsStaff ? Convert.ToInt32(Session["StaffID"]) : -1;
        bool     ProvsCanSeeOtherProvs = ((SystemVariables)Session["SystemVariables"])["Bookings_ProvsCanSeeOtherProvs"].Value == "1";
        bool     ProvidersSelectToSeeOtherProviders = (bool)Session["ShowOtherProvidersOnBookingScreen"];

        string regStaffWhereClause = @" organisation_id = " + orgID + (showUnavailableStaff ? "" : " AND excl_" + date.DayOfWeek.ToString().Substring(0, 3).ToLower() + @" = 0") + " AND staff_is_provider = 1";
        DataRow[] foundRows = orgOpen ?
            tblAllRegisteredStaff.Select(regStaffWhereClause) :
            tblAllRegisteredStaff.Select(" 1 = 0 ");


        Hashtable staffHash = new Hashtable();
        DataTable staffNames = new DataTable();
        staffNames.Columns.Add("firstname");
        staffNames.Columns.Add("middlename");
        staffNames.Columns.Add("surname");
        staffNames.Columns.Add("staff_id");
        foreach (DataRow row in foundRows)
        {
            int staff_id = Convert.ToInt32(row["staff_staff_id"]);

            if (userView.IsProviderView && (!ProvsCanSeeOtherProvs || !ProvidersSelectToSeeOtherProviders) && staff_id != loggedInStaffID)
                continue;

            if (staffHash[staff_id] == null)
            {
                Staff s = StaffDB.Load(row, "staff_");
                s.Person = PersonDB.Load(row, "", "person_entity_id");
                s.Person.Title = IDandDescrDB.Load(row, "title_id", "descr");
                staffHash[staff_id] = s;


                DataRow newRow = staffNames.NewRow();
                newRow["firstname"] = row["firstname"];
                newRow["middlename"] = row["middlename"];
                newRow["surname"] = row["surname"];
                newRow["staff_id"] = row["staff_staff_id"];
                staffNames.Rows.Add(newRow);
            }
        }


        //
        // whether or not to add staff to be displayed who are not set as providers but have existing bookings
        //
        // since the fee to customers is based on the number of providers, they can just set one prov as a non-prov and re-set another one as one ... to make a booking
        // setting this as true will show bookings of those set as non-providers still
        // settting this as false will hide it, so it is more difficult for them to cheat the system as their bookings are not shown.
        //
        bool showExistingBookingsOfStaffWhoAreNowNonProvidersForTodayOnwards = false;


        //
        // add where there are bookings already for some provider at this org but they are not on the list of registered providers
        //
        Hashtable staffHashHasExistingBookings = new Hashtable();

        if (bookings != null)
        {
            foreach (Booking booking in bookings)
            {
                if (booking.Organisation == null                 ||
                    booking.Organisation.OrganisationID != orgID ||
                    booking.BookingTypeID != 34                  ||
                    booking.DateStart.Date != date.Date)
                    continue;

                // if is today-onwards, and is non-provider, and is set to hide those of non-prov for toay-onwards, don't add them in.
                if (!showExistingBookingsOfStaffWhoAreNowNonProvidersForTodayOnwards && date >= DateTime.Today.Date && !booking.Provider.IsProvider)
                    continue;


                staffHashHasExistingBookings[booking.Provider.StaffID] = booking.Provider;

                if (userView.IsProviderView && (!ProvsCanSeeOtherProvs || !ProvidersSelectToSeeOtherProviders) && booking.Provider.StaffID != loggedInStaffID)
                    continue;

                if (staffHash[booking.Provider.StaffID] != null)
                    continue;

                staffHash[booking.Provider.StaffID] = booking.Provider;
                DataRow newRow = staffNames.NewRow();
                newRow["firstname"]  = booking.Provider.Person.Firstname;
                newRow["middlename"] = booking.Provider.Person.Middlename;
                newRow["surname"]    = booking.Provider.Person.Surname;
                newRow["staff_id"]   = booking.Provider.StaffID;
                staffNames.Rows.Add(newRow);
            }
        }



        if (staffNames.Rows.Count > 0)
        {
            DataView dv = staffNames.DefaultView;
            dv.Sort = "surname,firstname,middlename";
            staffNames = dv.ToTable();
        }

        ArrayList returnArrayList = new ArrayList();
        TimeSpan dateEndMin = new TimeSpan(23, 59, 0);
        for (int i = 0; i < staffNames.Rows.Count; i++)
        {
            int staff_id = Convert.ToInt32(staffNames.Rows[i]["staff_id"]);

            bool unavailable = false;

            bool alreadyHasBookings = staffHashHasExistingBookings[staff_id] != null;
            if (!alreadyHasBookings && !showUnavailableStaff)
            {

                foreach (DataRow row in tblUnavailableProvBookingsFullDays.Rows)
                {
                    // lop off this loops processing before doing ANYTHING if not for this provider or if deleted
                    if (row["provider"] == DBNull.Value || Convert.ToInt32(row["provider"]) != staff_id || row["date_deleted"] != DBNull.Value)
                        continue;

                    Booking b = BookingDB.Load(row);

                    bool unavail =

                            (b.Provider != null && b.Provider.StaffID == staff_id)

                            &&

                            b.DateDeleted == DateTime.MinValue

                            &&

                            (
                                (b.BookingTypeID == 341 && (b.Organisation == null || b.Organisation.OrganisationID == orgID) && ((!b.IsRecurring && b.DateStart.TimeOfDay == TimeSpan.Zero && b.DateEnd.TimeOfDay >= dateEndMin) || (b.IsRecurring && b.RecurringStartTime == TimeSpan.Zero && b.RecurringEndTime >= dateEndMin))) ||
                                (b.BookingTypeID == 342 && (b.Organisation == null || b.Organisation.OrganisationID == orgID) && ((!b.IsRecurring && b.DateStart.TimeOfDay == TimeSpan.Zero && b.DateEnd.TimeOfDay >= dateEndMin) || (b.IsRecurring && b.RecurringStartTime == TimeSpan.Zero && b.RecurringEndTime >= dateEndMin)))
                            )

                            &&

                            (
                                (!b.IsRecurring && b.DateStart.Date == date.Date) ||
                                (b.IsRecurring && b.DateStart.Date <= date.Date
                                                && (b.DateEnd == DateTime.MinValue || b.DateEnd.Date >= date.Date)
                                                && b.RecurringDayOfWeek == date.DayOfWeek)
                            );



                    if (unavail)
                    {
                        unavailable = true;
                        break;
                    }
                }

            }

            if (!unavailable && staffHash[staff_id] != null)
                returnArrayList.Add((Staff)staffHash[staff_id]);
        }


        Staff[] returnList = (Staff[])returnArrayList.ToArray(typeof(Staff));
        return returnList;
    }

    #endregion


    #region Page_Load

    bool debugPageLoadTime = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["UpdateFromWebPay"] != null)
            {
                PaymentPendingDB.UpdateAllPaymentsPending(null, DateTime.Now.AddDays(-15), DateTime.Now.AddDays(1), Convert.ToInt32(Session["StaffID"]));
                Session.Remove("UpdateFromWebPay");
            }

            if (!IsPostBack)
                Utilities.SetNoCache(Response);  // can comment this out when not changes been made for a while

            if (debugPageLoadTime)
                Logger.LogQuery("Page_Load - Start", false, false, true);


            if (!IsPostBack && Request.QueryString["scroll_pos"] != null)
                scrollValues.Value = Request.QueryString["scroll_pos"];

            if (debugPageLoadTime)
                Logger.LogQuery("Page_Load - Start Set Globals from DB", false, false, true);
            SetGlobalVariables();
            if (debugPageLoadTime)
                Logger.LogQuery("Page_Load - Got Globals from DB", false, false, true);

            if (this.orgs.Length == 0)
                throw new CustomMessageException("No valid organisations to display", true);

            if (this.NumColumns == 0)
            {
                if (this.daysData.Length == 0)
                    throw new CustomMessageException("No days set to display", true);
                else
                    throw new CustomMessageException("No workers working on allocated days", true);
            }

            SetPageHeading();
            SetHiddenVariablesForJavascriptToAccess();


            if (!IsPostBack)
            {
                SetupGUI();

                lblMinBookingDurationMins.Text = "10";
                if (Convert.ToBoolean(Session["SiteIsClinic"]))
                    lblMinBookingDurationMins.Text = ((SystemVariables)Session["SystemVariables"])["BookingSheetTimeSlotMins_Clinic"].Value;
                else if (Convert.ToBoolean(Session["SiteIsAgedCare"]))
                    lblMinBookingDurationMins.Text = ((SystemVariables)Session["SystemVariables"])["BookingSheetTimeSlotMins_AgedCare"].Value;
                else if (Convert.ToBoolean(Session["SiteIsGP"]))
                    lblMinBookingDurationMins.Text = ((SystemVariables)Session["SystemVariables"])["BookingSheetTimeSlotMins_GP"].Value;
                else
                    throw new Exception("Unknown type - not Clinics/AgedCare/GP");

                // SetupSlideOutWaitingList();


                // most mobile broswers don't call right click on long click
                // so we created modal menu's for those
                // but on chrome on android, a long click also registers a right click, so both types of menu's showed up
                // this is to hide right click menu on mobile devices
                if (Utilities.IsMobileDevice(Request, true, false))
                    div_right_click_menu.Visible = false;


                if (IsEditBookingMode())
                {
                    bookingModalElementControl.ShowAddBtn(false);
                    btnStopEdit.Visible = true;
                    div_stop_edit_btn.Style["width"] = "250px";

                    div_stop_edit_btn.Visible = true;
                    tbl_patient_flashing_message.Visible = false;

                    Booking bookingToEdit = BookingDB.GetByID(GetFormBooking());
                    bool isConfirmed = bookingToEdit.DateConfirmed != DateTime.MinValue;
                    TimeSpan duration = bookingToEdit.DateEnd.Subtract(bookingToEdit.DateStart);
                    lblEditBooking_Duration.Text = duration.TotalMinutes.ToString();
                    lblEditBooking_IsConfirmed.Text = isConfirmed ? "1" : "0";
                    lblEditBooking_IsAgedCare.Text = bookingToEdit.Organisation.OrganisationType.OrganisationTypeID == 367 ? "1" : "0";
                    lblEditBooking_Org.Text = bookingToEdit.Organisation.OrganisationID.ToString();


                    bool keepPatientUnset = Request.QueryString["patient"] != null && Request.QueryString["patient"] == "-1";
                    if (bookingToEdit.Patient != null && !keepPatientUnset)
                    {
                        txtPatientID.Text = bookingToEdit.Patient.PatientID.ToString();
                        txtUpdatePatientName.Text = bookingToEdit.Patient.Person.FullnameWithoutMiddlename;
                        UpdateTextbox(txtUpdatePatientName, txtPatientID.Text.Length == 0);
                    }
                    btnClearPatient.Visible = false;
                    btnPatientListPopup.Visible = false;
                }
                else if (IsEditDayMode())
                {
                    btnStopEdit.Visible = true;
                    div_stop_edit_btn.Style["width"] = "250px";

                    div_stop_edit_btn.Visible = true;
                    tbl_patient_flashing_message.Visible = false;

                    div_services_trailing_seperator.Visible = false;

                    tblPatientInfo.Visible = false;
                }
                else
                {
                    bookingModalElementControl.ShowUpdateBtn(false);
                    btnStopEdit.Visible = false;
                    div_stop_edit_btn.Style["width"] = "100px";
                    div_stop_edit_btn.Visible = false;
                    tbl_patient_flashing_message.Visible = false; // = true;  // chuck it .. since now both showing in the booking slots
                }
            }
            else
            {
                ResetPatientName();
            }

            bool redirected = SetSchedule();
            if (redirected)
                return;

            string showHideHeaderSectionScript = "show_hide_booking_sheet('header_section'," + ((Boolean)Session["ShowHeaderOnBookingScreen"] ? "true" : "false") + ");";
            if (!IsPostBack)
            {
                string showHideBookingDetailsScript = GetFormIsCollapsed() ? "show_hide_booking_details(false);" : "";
                string getFormBookingSheetFocusedScript = ""; // GetFormBookingSheetFocused() ? "window.onload = function() { alert('about to do iiiiit');" : "";
                string returnToCallCenterScript = (Session["StaffID"] != null && (new List<int> { -5, -7, -8 }).Contains((int)Session["StaffID"])) ? "setInterval(function(){ document.getElementById(\"lnkBackToCallCenter\").click(); },2400000);;" : "";

                if (Request.QueryString["scroll_pos"] != null)
                    ClientScript.RegisterStartupScript(Page.ClientScript.GetType(), Page.ClientID, "resetScrollPosition(" + Request.QueryString["scroll_pos"].Replace('_', ',') + ");" + showHideBookingDetailsScript + getFormBookingSheetFocusedScript + showHideHeaderSectionScript + returnToCallCenterScript, true);
                else if (Request.QueryString["scroll_to_cell"] != null)
                    ClientScript.RegisterStartupScript(Page.ClientScript.GetType(), Page.ClientID, "go_to_anchor('" + Request.QueryString["scroll_to_cell"] + "');" + showHideBookingDetailsScript + getFormBookingSheetFocusedScript + showHideHeaderSectionScript + returnToCallCenterScript, true);
                else
                    ClientScript.RegisterStartupScript(Page.ClientScript.GetType(), Page.ClientID, showHideBookingDetailsScript + getFormBookingSheetFocusedScript + showHideHeaderSectionScript + returnToCallCenterScript, true);
            }
            else
            {
                if (scrollValues.Value != null && Regex.IsMatch(scrollValues.Value, @"\d+_\d+"))
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), Page.ClientID, "resetScrollPosition(" + scrollValues.Value.Replace('_', ',') + ");" + showHideHeaderSectionScript, true);
                else
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), Page.ClientID, showHideHeaderSectionScript, true);
            }

            txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', document.getElementById('txtStartDate'), 'dmy', '-', 'reload_booking_page_start_date'); return false;";
        }
        catch (CustomMessageException ex)
        {
            if (ex.HidePage)
                HideTableAndSetErrorMessage(ex.Message);
            else
                SetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            HideTableAndSetErrorMessage("", ex.ToString());
        }
        finally
        {
            if (debugPageLoadTime)
                Logger.LogQuery("Page_Load - End", false, false, true);


            if (Session["downloadFile_Contents"] != null && Session["downloadFile_DocName"] != null)
            {

                // when downloading the file, it stops the response (ie the page 'diplay' isn't reloaded)
                // so if it hasn't reloaded, then set it as reloaded for the next load, and set a javascript page reload so that 
                // this page is updated/loaded and THEN the reload will not update the display and that time we can run the download

                if (Session["downloadFile_PageReloaded"] == null)
                {
                    Session["downloadFile_PageReloaded"] = true;
                    hiddenFieldReloadPage.Value = "1";
                    //ClientScript.RegisterStartupScript(this.GetType(), "reload_it", "<script language=javascript>window.location.href = window.location.href;</script>");
                }
                else
                {
                    byte[] downloadFile_Contents = (byte[])Session["downloadFile_Contents"];
                    string downloadFile_DocName = (string)Session["downloadFile_DocName"];

                    Session.Remove("downloadFile_Contents");
                    Session.Remove("downloadFile_DocName");
                    Session.Remove("downloadFile_PageReloaded");


                    try
                    {
                        string contentType = "application/octet-stream";
                        try { contentType = Utilities.GetMimeType(System.IO.Path.GetExtension(downloadFile_DocName)); }
                        catch (Exception) { }

                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ContentType = contentType;
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFile_DocName + "\"");
                        Response.OutputStream.Write(downloadFile_Contents, 0, downloadFile_Contents.Length);
                        Response.End();
                    }
                    catch (System.Web.HttpException ex)
                    {
                        // ignore exception where user closed the download box
                        if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                            throw;
                    }
                }

            }
        }

    }

    protected void SetPageHeading()
    {

        if (IsEditDayMode())
        {
            lblHeading.Text = "Move All Bookings for " + StaffDB.GetByID(GetFormEditDay_Provider()).Person.FullnameWithoutMiddlename;
            lblOrgAddress.Text = string.Empty;
            return;
        }


        if (this.GetFormOrgs().Length == 1)
        {
            Tuple<string, string, string, string, string> orgContactInfo = GetOrgContactInfo(this.orgs[0].EntityID);
            string orgAddressText = (string)orgContactInfo.Item1;
            //string orgPhoneText   = (string)orgContactInfo.Item2;  // if needed, uncomment out from the below method
            //string orgFaxText     = (string)orgContactInfo.Item3;  // if needed, uncomment out from the below method
            //string orgWebText     = (string)orgContactInfo.Item4;  // if needed, uncomment out from the below method
            //string orgEmailText   = (string)orgContactInfo.Item5;  // if needed, uncomment out from the below method
            lblHeading.Text    = this.orgs[0].Name;
            lblOrgAddress.Text = orgAddressText;
        }
        else
        {
            lblHeading.Text = IsEditBookingMode() ? "Edit Booking" : "Bookings";
            lblOrgAddress.Text = string.Empty;
        }
    }
    protected Tuple<string, string, string, string, string> GetOrgContactInfo(int entitytID)
    {
        string orgAddressText = null, orgPhoneText = null, orgFaxText = null, orgWebText = null, orgEmailText = null;
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact orgAddress = ContactDB.GetFirstByEntityID(1, entitytID);
            //Contact orgPhone   = ContactDB.GetFirstByEntityID(2, entitytID);
            //Contact orgFax     = ContactDB.GetFirstByEntityID(-1, entitytID, 31);
            //Contact orgWeb     = ContactDB.GetFirstByEntityID(-1, entitytID, 28);
            //Contact orgEmail   = ContactDB.GetFirstByEntityID(-1, entitytID, 27);

            orgAddressText = orgAddress == null ? "" : orgAddress.GetFormattedAddress("");
            //orgPhoneText       = orgPhone   == null ? "" : orgPhone.GetFormattedPhoneNumber("");
            //orgFaxText         = orgFax     == null ? "" : orgFax.GetFormattedPhoneNumber("");
            //orgWebText         = orgWeb     == null ? "" : orgWeb.AddrLine1;
            //orgEmailText       = orgEmail   == null ? "" : orgEmail.AddrLine1;
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus orgAddressAus = ContactAusDB.GetFirstByEntityID(1, entitytID);
            //ContactAus orgPhoneAus   = ContactAusDB.GetFirstByEntityID(2, entitytID);
            //ContactAus orgFaxAus     = ContactAusDB.GetFirstByEntityID(-1, entitytID, 31);
            //ContactAus orgWebAus     = ContactAusDB.GetFirstByEntityID(-1, entitytID, 28);
            //ContactAus orgEmailAus   = ContactAusDB.GetFirstByEntityID(-1, entitytID, 27);

            orgAddressText = orgAddressAus == null ? "" : orgAddressAus.GetFormattedAddress("");
            //orgPhoneText             = orgPhoneAus   == null ? "" : orgPhoneAus.GetFormattedPhoneNumber("");
            //orgFaxText               = orgFaxAus     == null ? "" : orgFaxAus.GetFormattedPhoneNumber("");
            //orgWebText               = orgWebAus     == null ? "" : orgWebAus.AddrLine1;
            //orgEmailText             = orgEmailAus   == null ? "" : orgEmailAus.AddrLine1;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        return new Tuple<string, string, string, string, string>(orgAddressText, orgPhoneText, orgFaxText, orgWebText, orgEmailText);
    }

    protected void SetHiddenVariablesForJavascriptToAccess()
    {
        lblOrgIDs.Text = string.Empty;
        lblOrgTypeIDs.Text = string.Empty;
        lblOrgNames.Text = string.Empty;
        for (int i = 0; i < this.orgs.Length; i++)
        {
            if (i > 0)
            {
                lblOrgIDs.Text += ",";
                lblOrgTypeIDs.Text += ",";
                lblOrgNames.Text += ",";
            }
            lblOrgIDs.Text += this.orgs[i].OrganisationID;
            lblOrgTypeIDs.Text += this.orgs[i].OrganisationType.OrganisationTypeID;
            lblOrgNames.Text += this.orgs[i].Name;
        }

        lblProviderIDs.Text = string.Empty;
        lblProviderNames.Text = string.Empty;
        for (int i = 0; i < this.staffList.Length; i++)
        {
            if (i > 0)
            {
                lblProviderIDs.Text += ",";
                lblProviderNames.Text += ",";
            }
            lblProviderIDs.Text += this.staffList[i].StaffID;
            lblProviderNames.Text += this.staffList[i].Person.FullnameWithoutMiddlename;
        }

        bool PTLogin_BookingTimeEditable = ((SystemVariables)Session["SystemVariables"])["PTLogin_BookingTimeEditable"].Value == "1";
        lblBookingTimeEditable.Text = UserView.GetInstance().IsPatient && !PTLogin_BookingTimeEditable ? "0" : "1";
    }


    #endregion

    #region SetSchedule,UpdateSchedule, ddl_SelectedIndexChanged, btnUpdateDaysToDisplay_Click, chkShowDetails_Click, chkShowUnavailableStaff_Click

    protected void btnStopEdit_Click(object sender, EventArgs e)
    {
        if (IsEditBookingMode())
        {
            string newURL = Request.RawUrl;
            newURL = UrlParamModifier.Remove(newURL, "edit_booking_id");
            newURL = UrlParamModifier.Remove(newURL, "offering"); // also remove offering to make them have to reset it
            newURL = UrlParamModifier.Remove(newURL, "patient");
            newURL = UrlParamModifier.Update(!chkShowDetails.Checked, newURL, "is_collapsed", "1");
            if (scrollValues.Value != "0")
                newURL = UrlParamModifier.AddEdit(newURL, "scroll_pos", scrollValues.Value);

            Response.Redirect(newURL);
            return;
        }
        if (IsEditDayMode())
        {
            string newURL = Request.RawUrl;
            newURL = UrlParamModifier.Remove(newURL, "edit_date");
            newURL = UrlParamModifier.Remove(newURL, "edit_org");
            newURL = UrlParamModifier.Remove(newURL, "edit_provider");
            newURL = UrlParamModifier.Update(!chkShowDetails.Checked, newURL, "is_collapsed", "1");
            if (scrollValues.Value != "0")
                newURL = UrlParamModifier.AddEdit(newURL, "scroll_pos", scrollValues.Value);

            Response.Redirect(newURL);
            return;
        }
    }

    protected void ddlFields_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Session["StaffID"] != null)
            StaffDB.UpdateBookingScreenField(Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(ddlFields.SelectedValue)); // save to db
//        SetSchedule();
    }

    protected void btnMoveDate_Command(object sender, CommandEventArgs e)
    {
        DateTime startDate = GetFormDate();

        if (e.CommandArgument == "Today")
            startDate = DateTime.Today;
        else
        {
            int totalToMove = Convert.ToInt32(ddlMoveDateNum.SelectedValue);
            if (e.CommandArgument == "Backwards")
                totalToMove *= -1;

            if (ddlMoveDateType.SelectedValue == "Days")
                startDate = startDate.AddDays(totalToMove);
            if (ddlMoveDateType.SelectedValue == "Weeks")
                startDate = startDate.AddDays(totalToMove * 7);
            if (ddlMoveDateType.SelectedValue == "Months")
                startDate = startDate.AddMonths(totalToMove);
            if (ddlMoveDateType.SelectedValue == "Years")
                startDate = startDate.AddYears(totalToMove);

        }

        string newURL = Request.RawUrl;
        if (scrollValues.Value != "0")
            newURL = UrlParamModifier.AddEdit(newURL, "scroll_pos", scrollValues.Value);
        newURL = UrlParamModifier.Update(!chkShowUnavailableStaff.Checked, newURL, "show_unavailable_staff", "0");
        newURL = UrlParamModifier.Update(ddlFields.SelectedValue != "-1", newURL, "field", ddlFields.SelectedValue);
        newURL = UrlParamModifier.Update(ddlServices.SelectedValue != "-1", newURL, "offering", ddlServices.SelectedValue);
        newURL = UrlParamModifier.AddEdit(newURL, "date", startDate.ToString("yyyy_MM_dd"));
        Response.Redirect(newURL);
    }

    protected void chkShowOtherProviders_CheckedChanged(object sender, EventArgs e)
    {
        Session["ShowOtherProvidersOnBookingScreen"] = chkShowOtherProviders.Checked;
        Response.Redirect(Request.RawUrl);
    }

    protected void btnUpdateDisplayOptions_Click(Object sender, EventArgs e)
    {
        UpdateDisplayOptions();
    }

    protected void UpdateDisplayOptions()
    {
        string newURL = Request.RawUrl;

        if (scrollValues.Value != "0")
            newURL = UrlParamModifier.AddEdit(newURL, "scroll_pos", scrollValues.Value);


        DateTime startDate;
        try
        {
            string[] parts = txtStartDate.Text.Split('-');
            startDate = new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]));
        }
        catch (Exception)
        {
            SetErrorMessage("Start date is incorrect format");
            return;
        }

        if (startDate <= new DateTime(1900, 1, 1))
        {
            SetErrorMessage("Start date must be within the last century");
            return;
        }



        newURL = UrlParamModifier.AddEdit(newURL, "date", startDate.ToString("yyyy_MM_dd"));
        newURL = UrlParamModifier.AddEdit(newURL, "ndays", ddlDaysToDisplay.SelectedValue);
        newURL = UrlParamModifier.Update(!chkShowDetails.Checked, newURL, "is_collapsed", "1");
        newURL = UrlParamModifier.Update(!chkShowUnavailableStaff.Checked, newURL, "show_unavailable_staff", "0");
        newURL = UrlParamModifier.Update(ddlFields.SelectedValue != "-1", newURL, "field", ddlFields.SelectedValue);
        newURL = UrlParamModifier.Update(ddlServices.SelectedValue != "-1", newURL, "offering", ddlServices.SelectedValue);


        if (Convert.ToInt32(ddlDaysToDisplay.SelectedValue) != Convert.ToInt32(Session["NumDaysToDisplayOnBookingScreen"]))
        {
            if (Session["StaffID"] != null)
                StaffDB.UpdateNumDaysToDisplayOnBookingScreen(Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(ddlDaysToDisplay.SelectedValue));
            Session["NumDaysToDisplayOnBookingScreen"] = Convert.ToInt32(ddlDaysToDisplay.SelectedValue);
        }

        //if (UrlParamModifier.Remove(newURL, "scroll_pos") != UrlParamModifier.Remove(Request.RawUrl, "scroll_pos"))
        //{

            bool resetXscroll = GetFormIsCollapsed() == chkShowDetails.Checked;
            bool resetYscroll = GetFormShowUnavailableStaff() != chkShowUnavailableStaff.Checked;

            string scroll_pos = scrollValues.Value;
            string[] split = scroll_pos.Split('_');

            if (resetXscroll && resetYscroll)
                newURL = UrlParamModifier.AddEdit(newURL, "scroll_pos", "0_0");
            else if (split.Length == 2 &&  resetXscroll && !resetYscroll)
                newURL = UrlParamModifier.AddEdit(newURL, "scroll_pos", "0_" + split[1]);
            else if (split.Length == 2 && !resetXscroll &&  resetYscroll)
                newURL = UrlParamModifier.AddEdit(newURL, "scroll_pos", split[0] + "_0");
            else
                newURL = UrlParamModifier.AddEdit(newURL, "scroll_pos", scroll_pos);

            Response.Redirect(newURL);
        //}
    }




    protected bool SetSchedule() // redirect throws an exception, so return if redirected and end in function that calls it
    {
        RemovePreviousElements();

        if (!IsEditDayMode())
        {
            string newURL = Request.RawUrl;

            if (scrollValues.Value != "0")
                newURL = UrlParamModifier.AddEdit(newURL, "scroll_pos", scrollValues.Value);

            // if url contains these .. carry on ... else post back here with these to have them in the url
            int fieldID = Convert.ToInt32(ddlFields.SelectedValue);
            int offeringID = Convert.ToInt32(ddlServices.SelectedValue);
            int patientID = txtPatientID.Text.Length == 0 ? -1 : Convert.ToInt32(txtPatientID.Text);
            newURL = UrlParamModifier.Update((fieldID != -1), newURL, "field", fieldID.ToString());
            newURL = UrlParamModifier.Update((offeringID != -1), newURL, "offering", offeringID.ToString());
            newURL = UrlParamModifier.Update((patientID != -1), newURL, "patient", patientID.ToString());


            if (Request.RawUrl.Contains("patient=-1") && patientID == -1)
            {
                newURL = UrlParamModifier.AddEdit(newURL, "patient", "-1");
                if (UrlParamModifier.Remove(newURL, "scroll_pos") != UrlParamModifier.Remove(Request.RawUrl, "scroll_pos"))
                {
                    Response.Redirect(newURL, false);
                    return true;
                }
            }
            else
            {
                if (UrlParamModifier.Remove(newURL, "scroll_pos") != UrlParamModifier.Remove(Request.RawUrl, "scroll_pos"))
                {
                    Response.Redirect(newURL, false);
                    return true;
                }
            }
        }

        UpdateSchedule();

        return false;
    }

    protected void RemovePreviousElements()
    {
        bool showContextMenu = ddlServices.SelectedValue != "-1" && txtPatientID.Text.Length > 0;

        bool isEditMode = IsEditBookingMode() || IsEditDayMode();
        for (int r = 0; r < main_table.Rows.Count; r++)
        {
            for (int c = 1; c < main_table.Rows[r].Cells.Count + 1; c++)
            {
                HtmlTableCell cell = main_table.Rows[r].Cells[c];
                if (!showContextMenu)
                {
                    cell.Attributes.Remove("class");
                    cell.Attributes.Add("class", BookingSlot.GetContextMenuClass(BookingSlot.Type.None));
                }
                for (int i = cell.Controls.Count - 1; i >= 0; i--)
                {
                    if (!(cell.Controls[i] is Label))
                        continue;

                    if (((Label)cell.Controls[i]).ID.Contains("lblBookingID_") ||
                        ((Label)cell.Controls[i]).ID.Contains("lblPatientName_") ||
                        ((Label)cell.Controls[i]).ID.Contains("lblOfferingID_") ||
                        ((Label)cell.Controls[i]).ID.Contains("lblProviderID_") ||
                        ((Label)cell.Controls[i]).ID.Contains("lblOrgID_"))
                        cell.Controls.Remove(cell.Controls[i]);
                    else
                    {
                        ((Label)cell.Controls[i]).BackColor = isEditMode ? HSLColor.ChangeBrightness(BookingSlot.GetColor(BookingSlot.Type.Available), 10) : BookingSlot.GetColor(BookingSlot.Type.Available);
                        ((Label)cell.Controls[i]).Text = (Utilities.IsDev()) ? cell.ID.Substring(3).Replace("_", " ") : "";
                        cell.BgColor = System.Drawing.ColorTranslator.ToHtml(((Label)cell.Controls[i]).BackColor);
                    }

                }
            }

        }
    }

    // adding bookings done here:
    protected void UpdateSchedule()
    {
        if (this.NumColumns == 0)
            return;


        bool isEditBookingMode = IsEditBookingMode();
        bool isEditDayMode     = IsEditDayMode();
        if (isEditBookingMode && !IsValidFormBooking())
            throw new CustomMessageException();


        int offeringID = Convert.ToInt32(ddlServices.SelectedValue);
        Offering offering = offeringID == -1 ? null : OfferingDB.GetByID(offeringID);


        // edit single booking - variables
        Booking bookingToEdit = isEditBookingMode ? BookingDB.GetByID(GetFormBooking()) : null;
        if (isEditBookingMode && bookingToEdit == null)
            throw new CustomMessageException();

        // edit (move) days bookings - variables
        DateTime editDay = isEditDayMode ? GetFormEditDay_Date() : DateTime.MinValue;
        int editOrg  = isEditDayMode ? GetFormEditDay_Org() : -1;
        int editProv = isEditDayMode ? GetFormEditDay_Provider() : -1;

        SetTable();

        StartEndTime startEndTime = Organisation.GetStartEndTime(daysData[0].Date, daysData[daysData.Length - 1].Date, this.orgs);

        string firstRow = GetFirstTimeOfDay();
        TimeSpan firstCellTimeOfDay = new TimeSpan(Convert.ToInt32(firstRow.Substring(0, 2)), Convert.ToInt32(firstRow.Substring(2, 2)), 0);

        bool isAgedCareBooking = this.orgs.Length > 0 && this.orgs[0].OrganisationType.OrganisationTypeID != 218;

        Booking[] bookings = this.bookings != null ? this.bookings : BookingDB.GetBetween(debugPageLoadTime, this.daysData[0].Date, this.daysData[this.daysData.Length - 1].Date.AddDays(1), this.staffList, this.orgs, null, null);

        if (debugPageLoadTime) 
            Logger.LogQuery("UpdateSchedule - Get Caches", false, false, true);

        // caches for less db lookups since booking screen will be used ALOT
        Hashtable changeHistoryHash = BookingDB.GetChangeHistoryCountHash(bookings);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 02", false, false, true);
        Hashtable patientHealthCardCache       = GetPatientHealthCardCache(bookings);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 03", false, false, true);
        Hashtable patientsHasNotesCache = GetPatientsHasNotesCache(bookings);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 04", false, false, true);
        Hashtable patientsHasMedNotesCache = GetPatientsHasMedNotesCache(bookings);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 05", false, false, true);
        Hashtable patientsHasMedCondNotesCache = GetPatientsHasMedCondNotesCache(bookings);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 06", false, false, true);
        Hashtable epcRemainingCache = GetEPCRemainingCache(patientHealthCardCache);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 07", false, false, true);
        Hashtable patientsPhoneNumbersCache = GetPatientsPhoneNumbersCache(bookings);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 08", false, false, true);
        int       bookingSheetStartYear        = this.daysData[0].Date.Year;
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 09", false, false, true);
        int       bookingSheetEndYear          = this.daysData[this.daysData.Length - 1].Date.Year;
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 10", false, false, true);
        Hashtable patientsMedicareCountThisYearCache = GetPatientsMedicareCountThisYearCache(bookings, bookingSheetStartYear);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 11", false, false, true);
        Hashtable patientsMedicareCountNextYearCache = (bookingSheetStartYear == bookingSheetEndYear) ? patientsMedicareCountThisYearCache : GetPatientsMedicareCountThisYearCache(bookings, bookingSheetEndYear);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 12", false, false, true);
        Hashtable patientsEPCRemainingCache = GetPatientsEPCRemainingCacheDB(bookings, this.daysData[0].Date.AddYears(-1));
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 13", false, false, true);
        Hashtable patientsFlashingNotesCache = PatientDB.GetFlashingNotesCache(bookings);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 14", false, false, true);
        Hashtable patientsOwingCache = PatientDB.GetOwingCache(bookings);
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 15", false, false, true);

        // has the children and parent organisations set
        Hashtable orgTreeHash = OrganisationTree.GetTreeHashtable(null, true, 0, false, "139,367,372,218");
        //if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Get Caches - 16", false, false, true);


        if (debugPageLoadTime) Logger.LogQuery("UpdateSchedule - Put Bookings Into Display", false, false, true);


        UserView userView        =  UserView.GetInstance();
        int      loggedInStaffID = userView.IsPatient ? -1 : Convert.ToInt32(Session["StaffID"]);

        int    patientLoginID                = userView.IsPatient ? Convert.ToInt32(Session["PatientID"]) : -1;
        bool   isCallCenterStaffLoggedIn     = (new List<int> { -5, -7, -8 }).Contains(loggedInStaffID);
        string callCenterPrefix              = isCallCenterStaffLoggedIn ? SystemVariableDB.GetByDescr("CallCenterPrefix").Value : string.Empty;
        int    MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);
        foreach (Booking curBooking in bookings)
        {
            bool isShowableToPatient = !userView.IsPatient || (curBooking.Patient != null && curBooking.Patient.PatientID == patientLoginID);

            bool isConfirmed = curBooking.DateConfirmed != DateTime.MinValue;
            bool isCompleted = curBooking.BookingStatus != null && curBooking.BookingStatus.ID != 0;

            int nSlots = curBooking.IsRecurring ?
                (int)Math.Ceiling(curBooking.RecurringEndTime.Subtract(curBooking.RecurringStartTime).TotalMinutes / (double)bookingSlotMinsCache.GetSlotMins()) :
                (int)Math.Ceiling(curBooking.DateEnd.Subtract(curBooking.DateStart).TotalMinutes / (double)bookingSlotMinsCache.GetSlotMins());

            bool isUnavailabilityAllOrgs = curBooking.BookingTypeID != 34 && curBooking.Organisation == null;

            bool useFirstTimeOfDayAsFirstText = curBooking.DateStart.TimeOfDay < firstCellTimeOfDay;

            // re-set org object that has children and parent organisations set
            if (curBooking.Organisation != null)
                curBooking.Organisation = (Organisation)orgTreeHash[curBooking.Organisation.OrganisationID];


            // foreach org
            for (int j = 0; j < this.orgs.Length; j++)
            {
                Organisation curOrgToUpdate = this.orgs[j];

                bool isForThisOrg = isUnavailabilityAllOrgs || this.orgs[j].OrganisationID == curBooking.Organisation.OrganisationID;
                bool isShowable = isForThisOrg && ((!isEditBookingMode && !isEditDayMode) || (isEditBookingMode && bookingToEdit.BookingID == curBooking.BookingID) || (isEditDayMode && curBooking.DateStart.Date == editDay && curBooking.Organisation.OrganisationID == editOrg && curBooking.Provider.StaffID == editProv));

                string bookingTypeText = string.Empty;
                bool  isEPCBooking     = false;
                bool  agedCareOrg      = curBooking.Organisation != null && curBooking.Organisation.OrganisationType.OrganisationTypeGroup.ID == 6;
                if (!userView.IsPatient && !agedCareOrg && curBooking.Patient != null && curBooking.BookingStatus.ID == 0)
                {
                    HealthCard hc = GetHealthCardFromCache(patientHealthCardCache, curBooking.Patient.PatientID);
                    Hashtable patientsMedicareCountCache = (curBooking.DateStart.Year == bookingSheetStartYear) ? patientsMedicareCountThisYearCache : patientsMedicareCountNextYearCache;


                    Booking.InvoiceType invType = curBooking.GetInvoiceType(hc, curBooking.Offering, curBooking.Patient, patientsMedicareCountCache, epcRemainingCache, MedicareMaxNbrServicesPerYear);
                    //Booking.InvoiceType invType = curBooking.GetInvoiceType(hc, null);

                    isEPCBooking = invType != Booking.InvoiceType.None && invType != Booking.InvoiceType.NoneFromCombinedYearlyThreshholdReached && invType != Booking.InvoiceType.NoneFromOfferingYearlyThreshholdReached && invType != Booking.InvoiceType.NoneFromExpired;

                    if (invType == Booking.InvoiceType.Medicare || invType == Booking.InvoiceType.DVA)
                    {
                        if (curBooking.Provider.Field.ID == 1) // provider is a GP
                        {
                            bookingTypeText = (invType == Booking.InvoiceType.Medicare ? "[Medicare]" : "[DVA]");
                        }
                        else
                        {
                            string invTypeText              = invType == Booking.InvoiceType.Medicare ? "Medicare" : "DVA";
                            int    totalServicesAllowedLeft = (MedicareMaxNbrServicesPerYear - (int)patientsMedicareCountCache[curBooking.Patient.PatientID]);
                            Pair   totalEPCRemaining        = patientsEPCRemainingCache[curBooking.Patient.PatientID] as Pair;

                            int nServicesLeft = 0;
                            if (totalEPCRemaining != null)
                            {
                                DateTime referralSignedDate = (DateTime)totalEPCRemaining.Second;
                                DateTime hcExpiredDate = referralSignedDate.AddYears(1);
                                if (curBooking.DateStart.Date >= referralSignedDate.Date && curBooking.DateStart.Date < hcExpiredDate.Date)
                                    nServicesLeft = (int)totalEPCRemaining.First;
                                if (totalServicesAllowedLeft < nServicesLeft)
                                    nServicesLeft = totalServicesAllowedLeft;
                            }
                            bookingTypeText = "[" + invTypeText + (invType == Booking.InvoiceType.Medicare ? " - " + (isShowable && !userView.IsPatient ? "<b>" : "") + nServicesLeft + (isShowable && !userView.IsPatient ? "</b>" : "") + " Left" : "") + "]";
                        }
                    }
                    else if (invType == Booking.InvoiceType.Insurance)
                        bookingTypeText = "[Insurance]";
                    else if (invType == Booking.InvoiceType.None)
                        bookingTypeText = "[PT Pays]";
                    else if (invType == Booking.InvoiceType.NoneFromCombinedYearlyThreshholdReached)
                        bookingTypeText = (isShowable && !userView.IsPatient ? "<font color=\"red\">" : "") + "[PT Pays - " + (isShowable && !userView.IsPatient ? "<b>" : "") + "Year Limit reached" + (isShowable && !userView.IsPatient ? "</b>" : "") + "]" + (isShowable && !userView.IsPatient ? "</font>" : "");
                    else if (invType == Booking.InvoiceType.NoneFromOfferingYearlyThreshholdReached)
                        bookingTypeText = (isShowable && !userView.IsPatient ? "<font color=\"red\">" : "") + "[PT Pays - " + (isShowable && !userView.IsPatient ? "<b>" : "") + "Year Limit reached for service" + (isShowable && !userView.IsPatient ? "</b>" : "") + "]" + (isShowable && !userView.IsPatient ? "</font>" : "");
                    else if (invType == Booking.InvoiceType.NoneFromExpired)
                        bookingTypeText = (isShowable && !userView.IsPatient ? "<font color=\"red\">" : "") + "[PT Pays - " + (isShowable && !userView.IsPatient ? "<b>" : "") + (curBooking.Provider.Field.ID == 1 ? "Card Expired" : (userView.IsClinicView ? "EPC Expired" : "Ref Expired")) + (isShowable && !userView.IsPatient ? "</b>" : "") + "]" + (isShowable && !userView.IsPatient ? "</font>" : "");
                }


                bool bkHasChangeHistory = changeHistoryHash[curBooking.BookingID] != null;

                bool menuVisible = userView.IsAdminView || userView.IsPrincipal || (userView.IsExternalView && !isCompleted) || (curBooking.Provider != null && curBooking.Provider.StaffID == loggedInStaffID) || (userView.IsPatient && isShowableToPatient);

                BookingSlot.Type bookingSlotType = BookingSlot.Type.None;
                if (isShowable && isShowableToPatient)
                {
                    if (curBooking.BookingTypeID != 34) // staff or org: full day slot set unavailable
                    {
                        bookingSlotType = isEditBookingMode || isEditDayMode ? BookingSlot.Type.FullDayAvailable : BookingSlot.Type.FullDayTaken;
                        if (!menuVisible) bookingSlotType = BookingSlot.Type.None;
                    }
                    else
                    {
                        if (isEditBookingMode)
                            bookingSlotType = BookingSlot.Type.UpdatableConfirmable;
                        else if (isEditDayMode)
                            bookingSlotType = BookingSlot.Type.UpdatableConfirmable;
                        else
                        {
                            if (curBooking.DateStart > DateTime.Now.AddHours(1))
                            {
                                bookingSlotType = BookingSlot.GetFuture(userView.IsPatient && isShowableToPatient, isAgedCareBooking, isEPCBooking, isConfirmed);
                                if (!menuVisible) bookingSlotType = BookingSlot.Type.None;
                            }
                            else if (isCompleted)
                            {
                                bool canSeeInvoiceInfo = userView.IsAdminView || userView.IsPrincipal || (curBooking.Provider != null && curBooking.Provider.StaffID == loggedInStaffID && curBooking.DateStart > DateTime.Today.AddMonths(-2));
                                bookingSlotType = BookingSlot.GetPastCompleted(userView.IsPatient && isShowableToPatient, isAgedCareBooking, isEPCBooking, canSeeInvoiceInfo && curBooking.InvoiceCount > 0);
                                if (!menuVisible) bookingSlotType = BookingSlot.Type.None;
                            }
                            else
                            {
                                bookingSlotType = BookingSlot.GetPastUncompleted(userView.IsPatient && isShowableToPatient, isAgedCareBooking, isEPCBooking, isConfirmed);
                                if (!menuVisible) bookingSlotType = BookingSlot.Type.None;
                            }
                        }
                    }
                }
                else
                {
                    // ignore if unavailable at specific org for prov (=341) and not for this org
                    if ((curBooking.BookingTypeID != 341 && curBooking.BookingTypeID != 342 && curBooking.BookingTypeID != 36) || curBooking.Organisation == null || curBooking.Organisation.OrganisationID == this.orgs[j].OrganisationID)
                    {
                        bookingSlotType = curBooking.BookingTypeID == 34 ? BookingSlot.Type.Unavailable : BookingSlot.Type.UnavailableButAddable;
                        if (!menuVisible) bookingSlotType = BookingSlot.Type.None;
                    }
                }



                string lblTextFirstCell        = GetBookingText(patientsPhoneNumbersCache, callCenterPrefix, curBooking, patientsHasNotesCache, patientsHasMedNotesCache, patientsHasMedCondNotesCache, patientsFlashingNotesCache, patientsOwingCache, isShowable && isShowableToPatient && (!userView.IsExternal || !isCompleted), userView.IsPatient || (userView.IsExternal && isCompleted), bookingTypeText, bkHasChangeHistory, true,  false, bookingSlotType);
                string lblTextFirstCellWithOrg = GetBookingText(patientsPhoneNumbersCache, callCenterPrefix, curBooking, patientsHasNotesCache, patientsHasMedNotesCache, patientsHasMedCondNotesCache, patientsFlashingNotesCache, patientsOwingCache, isShowable && isShowableToPatient && (!userView.IsExternal || !isCompleted), userView.IsPatient || (userView.IsExternal && isCompleted), bookingTypeText, bkHasChangeHistory, true, true, bookingSlotType);
                string lblTextRestCells        = GetBookingText(patientsPhoneNumbersCache, callCenterPrefix, curBooking, patientsHasNotesCache, patientsHasMedNotesCache, patientsHasMedCondNotesCache, patientsFlashingNotesCache, patientsOwingCache, isShowable && isShowableToPatient && (!userView.IsExternal || !isCompleted), userView.IsPatient || (userView.IsExternal && isCompleted), bookingTypeText, bkHasChangeHistory, false, false, bookingSlotType);

                if (isShowable && isShowableToPatient)
                {
                    if (curBooking.BookingTypeID != 34) // staff or org: full day slot set unavailable
                        UpdateCells(nSlots, curBooking, lblTextFirstCell, lblTextFirstCellWithOrg, lblTextRestCells, useFirstTimeOfDayAsFirstText, curOrgToUpdate, curBooking, isEditBookingMode || isEditDayMode ? BookingSlot.Type.FullDayAvailable : BookingSlot.Type.FullDayTaken, menuVisible);
                    else
                    {
                        if (isEditBookingMode)
                            UpdateCells(nSlots, curBooking, lblTextFirstCell, lblTextFirstCellWithOrg, lblTextRestCells, useFirstTimeOfDayAsFirstText, curOrgToUpdate, curBooking, BookingSlot.Type.UpdatableConfirmable, true);
                        else if (isEditDayMode)
                            UpdateCells(nSlots, curBooking, lblTextFirstCell, lblTextFirstCellWithOrg, lblTextRestCells, useFirstTimeOfDayAsFirstText, curOrgToUpdate, curBooking, BookingSlot.Type.UpdatableConfirmable, false);
                        else
                        {
                            if (curBooking.DateStart > DateTime.Now.AddHours(1))
                                UpdateCells(nSlots, curBooking, lblTextFirstCell, lblTextFirstCellWithOrg, lblTextRestCells, useFirstTimeOfDayAsFirstText, curOrgToUpdate, curBooking, BookingSlot.GetFuture(userView.IsPatient && isShowableToPatient, isAgedCareBooking, isEPCBooking, isConfirmed), menuVisible);
                            else if (isCompleted)
                            {
                                bool canSeeInvoiceInfo = userView.IsAdminView || userView.IsPrincipal || (curBooking.Provider != null && curBooking.Provider.StaffID == loggedInStaffID && curBooking.DateStart > DateTime.Today.AddMonths(-2));
                                UpdateCells(nSlots, curBooking, lblTextFirstCell, lblTextFirstCellWithOrg, lblTextRestCells, useFirstTimeOfDayAsFirstText, curOrgToUpdate, curBooking, BookingSlot.GetPastCompleted(userView.IsPatient && isShowableToPatient, isAgedCareBooking, isEPCBooking, canSeeInvoiceInfo && curBooking.InvoiceCount > 0), menuVisible);
                            }
                            else
                                UpdateCells(nSlots, curBooking, lblTextFirstCell, lblTextFirstCellWithOrg, lblTextRestCells, useFirstTimeOfDayAsFirstText, curOrgToUpdate, curBooking, BookingSlot.GetPastUncompleted(userView.IsPatient && isShowableToPatient, isAgedCareBooking, isEPCBooking, isConfirmed), menuVisible);
                        }
                    }
                }
                else
                {
                    // ignore if unavailable at specific org for prov (=341) and not for this org
                    if ((curBooking.BookingTypeID != 341 && curBooking.BookingTypeID != 342 && curBooking.BookingTypeID != 36) || curBooking.Organisation == null || curBooking.Organisation.OrganisationID == this.orgs[j].OrganisationID)
                        UpdateCells(nSlots, curBooking, lblTextFirstCell, lblTextFirstCellWithOrg, lblTextRestCells, useFirstTimeOfDayAsFirstText, curOrgToUpdate, curBooking, curBooking.BookingTypeID == 34 ? BookingSlot.Type.Unavailable : BookingSlot.Type.UnavailableButAddable, !isEditDayMode && menuVisible);
                }
            }

        }

        if (debugPageLoadTime)
            Logger.LogQuery("UpdateSchedule - End Put Bookings Into Display", false, false, true);
    }

    protected string GetBookingText(Hashtable patientsPhoneNumbersCache, string callCenterPrefix, Booking curBooking, Hashtable patientsHasNotesCache, Hashtable patientsHasMedNotesCache, Hashtable patientsHasMedCondNotesCache, Hashtable patientsFlashingNotesCache, Hashtable patientsOwingCache, bool isShowable, bool isPatientLoggedIn, string bookingTypeText, bool bkHasChangeHistory, bool isfirstSlotOfBooking, bool incOrg, BookingSlot.Type bookingSlotType)
    {
        if (isPatientLoggedIn && !isShowable)
            return Utilities.IsDev() ? "&nbsp;&nbsp;" : string.Empty;

        
        bool     IsMobileDevice = Utilities.IsMobileDevice(Request);
        UserView userView       = UserView.GetInstance();
        bool     isProviderViewAndOtherProviderBk = userView.IsProviderView && curBooking.Provider != null && Convert.ToInt32(Session["StaffID"]) != curBooking.Provider.StaffID;
        if (isProviderViewAndOtherProviderBk)
            isShowable = false;


        bool isCollapsed = false; //GetFormIsCollapsed();

        string blankText          = Utilities.IsDev() ? "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br/>" : "<br/><br/>";
        string blankTextCollapsed = Utilities.IsDev() ? "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" : "";

        string unavailableProvText = string.Empty;
        if (curBooking.BookingTypeID == 341 || curBooking.BookingTypeID == 342 || curBooking.BookingTypeID == 36)
        {
            unavailableProvText = (curBooking.IsRecurring) ?
                unavailableProvText = curBooking.RecurringDayOfWeek.ToString() + "'s this time <br /><b>" + curBooking.DateStart.Date.ToString("dd'/'MM'/'yy") + (curBooking.DateEnd == DateTime.MinValue ? " onwards" : " - " + curBooking.DateEnd.AddDays(-1).Date.ToString("dd'/'MM'/'yy")) + "</b>" + (curBooking.Organisation == null ? " <br />*<b>all orgs</b>*" : "") :
                unavailableProvText = "this time" + (curBooking.Organisation == null ? " *<b>all orgs</b>*" : "");
        }

        string unavailabilityReason = curBooking.BookingUnavailabilityReason == null ? "" : "<br /><i>[" + curBooking.BookingUnavailabilityReason.Descr + "]</i>";
        string addedByText = curBooking.AddedBy == null ? "" : "<a href=\"#\" onclick=\"$(this).tooltip().mouseover();return false;\" title=\"Added By " + curBooking.AddedBy.Person.FullnameWithoutMiddlename + " on " + curBooking.DateCreated.ToString("ddd d MMM, yyyy") + "\" style=\"text-decoration:none;\">*</a>";

        if (curBooking.BookingTypeID == 342)
        {
            if (curBooking.Provider != null)
                return isfirstSlotOfBooking && !isCollapsed ? curBooking.Provider.Person.FullnameWithoutMiddlename + " is unavailable " + unavailableProvText + unavailabilityReason + " " + addedByText : (isCollapsed ? blankTextCollapsed : blankText);
            else if (curBooking.Organisation != null)
                return isfirstSlotOfBooking && !isCollapsed ? curBooking.Organisation.Name + " is unavailable " + unavailableProvText + unavailabilityReason + " " + addedByText : (isCollapsed ? blankTextCollapsed : blankText);
            else
                return isfirstSlotOfBooking && !isCollapsed ? "All orgs/providers unavailable " + unavailableProvText + unavailabilityReason + " " + addedByText : (isCollapsed ? blankTextCollapsed : blankText);
        }
        else if (curBooking.BookingTypeID == 341)
            return isfirstSlotOfBooking && !isCollapsed ? curBooking.Provider.Person.FullnameWithoutMiddlename + " is unavailable " + unavailableProvText + unavailabilityReason + " " + addedByText : (isCollapsed ? blankTextCollapsed : blankText);
        else if (curBooking.BookingTypeID == 340)
            return isfirstSlotOfBooking && !isCollapsed ? curBooking.Organisation.Name + " is unavailable this day" : (isCollapsed ? blankTextCollapsed + unavailabilityReason + " " + addedByText : blankText);
        else if (curBooking.BookingTypeID == 36)
            return isfirstSlotOfBooking && !isCollapsed ? curBooking.Provider.Person.FullnameWithoutMiddlename + " is unavailable this time <a href=\"#\" onclick=\"$(this).tooltip().mouseover();return false;\" title=\"Paid Break - Counted In The Hours Worked Report\" style=\"color:inherit;\">(<u>P</u>)</a>" + unavailabilityReason + " " + addedByText : (isCollapsed ? blankTextCollapsed : blankText);
        else if (!isfirstSlotOfBooking)
            return (isCollapsed ? blankTextCollapsed : blankText);
        else
        {
            if (isCollapsed)
                return blankTextCollapsed;


            bool SiteIsClinic   = Convert.ToBoolean(Session["SiteIsClinic"]);
            bool SiteIsAgedCare = Convert.ToBoolean(Session["SiteIsAgedCare"]);
            bool SiteIsGP       = Convert.ToBoolean(Session["SiteIsGP"]);


            if (curBooking.Organisation.OrganisationType.OrganisationTypeGroup.ID == 6 && curBooking.Patient == null) // aged care
            {
                ArrayList orgTreeBranch = new ArrayList();

                Organisation org = curBooking.Organisation;
                orgTreeBranch.Add(org);
                while (org.ParentOrganisation != null)
                {
                    org = org.ParentOrganisation;
                    orgTreeBranch.Add(org);
                }

                string text = string.Empty;
                for (int i = orgTreeBranch.Count - 1; i >= 0; i--)
                    text += (text.Length == 0 ? "" : "<br />") + "&nbsp;・ " + ((Organisation)orgTreeBranch[i]).Name;

                return text;
            }
            else if (curBooking.Organisation.OrganisationType.OrganisationTypeGroup.ID == 6 && curBooking.Patient != null)
            {

                bool includeOrg = incOrg;
                bool incProvider = false;


                string notesText = string.Empty; 
                if (curBooking.Patient != null)
                    notesText             = Note.GetBookingPopupLinkTextV2(curBooking.EntityID, curBooking.NoteCount > 0, true, 1425, 700, "images/notes-bw-24-Thin.png", "images/notes-24-Thin.png", null, !IsMobileDevice);
                //string notesText        = Note.GetPopupLinkTextV2(15, curBooking.EntityID, curBooking.NoteCount > 0, true, 980, 530, "images/notes-bw-24-Thin.png", "images/notes-24-Thin.png", null, !IsMobileDevice);
                bool   ptHasNotes         = patientsHasNotesCache[curBooking.Patient.Person.EntityID] != null;
                string ptNotesText        = Note.GetPopupLinkTextV2(16, curBooking.Patient.Person.EntityID, ptHasNotes, true, 1425, 640, "images/BC-24-Thin-bw.png", "images/BC-24-Thin.png", null, !IsMobileDevice);
                bool   ptHasMedsNotes     = patientsHasMedNotesCache[curBooking.Patient.Person.EntityID] != null;
                string ptMedsNotesText    = Note.GetPopupLinkTextV2(17, curBooking.Patient.Person.EntityID, ptHasMedsNotes, true, 1050, 530, "images/M-24-Thin-bw.png", "images/M-24-Thin.png", null, !IsMobileDevice);
                bool   ptHasMedCondNotes  = patientsHasMedCondNotesCache[curBooking.Patient.Person.EntityID] != null;
                string ptMedCondNotesText = Note.GetPopupLinkTextV2(18, curBooking.Patient.Person.EntityID, ptHasMedCondNotes, true, 1050, 530, "images/MC-24-Thin-bw.png", "images/MC-24-Thin.png", null, !IsMobileDevice);


                bool hasArrived = curBooking.ArrivalTime != DateTime.MinValue;
                string jsArrivedText = hasArrived ?
                                            "javascript:ajax_booking_confirm_delete_cancel('unarrived', '" + curBooking.BookingID.ToString() + "');" :
                                            "javascript:ajax_booking_confirm_delete_cancel('arrived', '" + curBooking.BookingID.ToString() + "');";
                string imgArrivedText = "images/person2colour-24.png";
                string setArrivedText = "<input type=\"image\" title=\"" + (hasArrived ? "Unset Arrived" : "Set Arrived") + "\" src=\"" + imgArrivedText + "\" alt=\"" + (hasArrived ? "Unset Arrived" : "Set Arrived") + "\" onclick=\"" + jsArrivedText + "\" />";
                if (curBooking.ArrivalTime != DateTime.MinValue)
                    setArrivedText += "&nbsp;<b>" + curBooking.ArrivalTime.ToString("HH:mm") + "</b>";



                bool isConfirmed = curBooking.DateConfirmed != DateTime.MinValue;
                string jsConfirmedText = isConfirmed ?
                                            "javascript:ajax_booking_confirm_delete_cancel('unconfirm', '" + curBooking.BookingID.ToString() + "');" :
                                            "javascript:ajax_booking_confirm_delete_cancel('confirm', '" + curBooking.BookingID.ToString() + "');";
                string imgConfirmedText = isConfirmed ? "images/tick2color-24.png" : "images/tick2bw-24.png";
                string setConfirmedText = "<input type=\"image\" title=\"" + (isConfirmed ? "Set Unconfirmed" : "Set Confirmed") + "\" src=\"" + imgConfirmedText + "\" alt=\"" + (isConfirmed ? "Set Unconfirmed" : "Set Confirmed") + "\" onclick=\"" + jsConfirmedText + "\" />";


                bool   hasFlashingNote = patientsFlashingNotesCache[curBooking.Patient.PatientID] != null;
                string flashingNote    = hasFlashingNote ? (string)patientsFlashingNotesCache[curBooking.Patient.PatientID] : string.Empty;
                string flasingTextIcon = Patient.GetFlashingTextLink(curBooking.Patient.PatientID, hasFlashingNote, flashingNote, true, 525, 275, "images/asterisk_bw_16.png", "images/asterisk_red_16.gif", null, !IsMobileDevice);

                bool   hasOwing  = patientsOwingCache[curBooking.Patient.PatientID] != null;
                string owingText = hasOwing ? "<b><font color=\"red\">Owes $" + Convert.ToDecimal(patientsOwingCache[curBooking.Patient.PatientID]).ToString("0.00") + "</font></b>" : string.Empty;


                //string printLetterJS = "javascript:window.location.href='Letters_PrintV2.aspx?booking="+curBooking.BookingID+"';return false;";
                //string printLetterImg = "images/printer_green-24.png";
                //string printLetterText = "<input type=\"image\" title=\"Print Letter\" src=\"" + printLetterImg + "\" alt=\"Print Letter\" onclick=\"" + printLetterJS + "\" />";

                string preferredNbr = string.Empty;
                if (curBooking.Patient != null)
                {
                    if (Utilities.GetAddressType().ToString() == "Contact")
                    {
                        Contact[] phoneNbrs = GetPatientsPhoneNumbersFromCache(patientsPhoneNumbersCache, curBooking.Patient.Person.EntityID);
                        if (phoneNbrs != null && phoneNbrs.Length > 0)
                        {
                            preferredNbr = phoneNbrs[0].PhoneNumberWithDashes;

                            bool curIsMobile = phoneNbrs[0].ContactType.ContactTypeID == 30;

                            for (int i = 1; i < phoneNbrs.Length; i++)
                            {
                                if (phoneNbrs[i].ContactType.ContactTypeID == 30 && !curIsMobile)
                                {
                                    curIsMobile = phoneNbrs[i].ContactType.ContactTypeID == 30;
                                    preferredNbr = phoneNbrs[i].PhoneNumberWithDashes;
                                }
                            }
                        }
                    }
                    else if (Utilities.GetAddressType().ToString() == "ContactAus")
                    {
                        ContactAus[] phoneNbrs = GetPatientsPhoneNumbersFromAusCache(patientsPhoneNumbersCache, curBooking.Patient.Person.EntityID);
                        if (phoneNbrs != null && phoneNbrs.Length > 0)
                        {
                            preferredNbr = phoneNbrs[0].PhoneNumberWithDashes;

                            bool curIsMobile = phoneNbrs[0].ContactType.ContactTypeID == 30;

                            for (int i = 1; i < phoneNbrs.Length; i++)
                            {
                                if (phoneNbrs[i].ContactType.ContactTypeID == 30 && !curIsMobile)
                                {
                                    curIsMobile = phoneNbrs[i].ContactType.ContactTypeID == 30;
                                    preferredNbr = phoneNbrs[i].PhoneNumberWithDashes;
                                }
                            }
                        }
                    }
                    else
                        throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
                }

                if (preferredNbr.Length > 0 && callCenterPrefix.Length > 0)
                    preferredNbr = callCenterPrefix + " " + preferredNbr;

                int      loggedInStaffID   = Session["StaffID"]       == null ? -1 : Convert.ToInt32(Session["StaffID"]);
                bool     canSeePatientLink = userView.IsAdminView || userView.IsPrincipal || (curBooking.Patient != null && curBooking.Provider != null && curBooking.Provider.StaffID == loggedInStaffID);

                string ptURL = "PatientDetailV2.aspx?type=view&id=" + curBooking.Patient.PatientID;
                //string patientLink = curBooking.Patient == null ? "" : (canSeePatientLink ? flasingTextIcon + " " + "<a href=\"#\" onclick=\"open_new_window('PatientDetailV2.aspx?type=view&id=" + curBooking.Patient.PatientID + "', 1750, 1000);return false;\">" + curBooking.Patient.Person.FullnameWithoutMiddlename + (curBooking.Patient.Person.Nickname.Trim().Length == 0 ? "" : " (" + curBooking.Patient.Person.Nickname.Trim() + ")") + "</a><br />" : "<b>" + curBooking.Patient.Person.FullnameWithoutMiddlename + "</b><br />");
                string patientLink = curBooking.Patient == null ? "" : (canSeePatientLink ? flasingTextIcon + " " + "<a href=\"" + ptURL + "\" onclick=\"var win=window.open('" + ptURL + "', '_blank'); win.focus();return false;\">" + curBooking.Patient.Person.FullnameWithoutMiddlename + (curBooking.Patient.Person.Nickname.Trim().Length == 0 ? "" : " (" + curBooking.Patient.Person.Nickname.Trim() + ")") + "</a><span style=\"color:transparent;\">-</span>" + (curBooking.Patient.Person.Dob == DateTime.MinValue ? "" : "(" + Utilities.GetAge(curBooking.Patient.Person.Dob) + ")") + "<br />" : "<b>" + curBooking.Patient.Person.FullnameWithoutMiddlename + "</b><br />");


                bool isCancelled = curBooking.BookingStatus.ID == 188;
                bool notYetCompleted = curBooking.BookingStatus.ID == 0;


                return
                    //(!isShowable || isPatientLoggedIn || userView.IsExternalView ? "" : (isShowable && !isPatientLoggedIn ? notesText + "&nbsp;" + ptNotesText + "&nbsp;" +  (SiteIsGP ? ptMedsNotesText + "&nbsp;" : "") : "") : "") + (notYetCompleted ? setConfirmedText + "&nbsp;" + setArrivedText : "") + "<br/>") +
                    //(bookingTypeText.Length > 0 ? bookingTypeText + "<br/>" : "") +

                    (isCancelled                                     ? "<b>[Cancelled" + (curBooking.InvoiceCount > 0 ? "" : " - No Inv") + "]</b><br/>" : "") +
                    (!includeOrg  || curBooking.Organisation == null ? "" : "<i>[" + curBooking.Organisation.Name + "]</i><br/>") +
                    (!incProvider || curBooking.Provider == null     ? "" : curBooking.Provider.Person.FullnameWithoutMiddlename + "<br/>") +
                    (curBooking.Patient == null || !curBooking.Patient.IsDiabetic || isPatientLoggedIn ? "" : "<font color=\"blue\"><b><i>PT is Diabetic</i></b></font><br/>") +
                    patientLink +
                    (preferredNbr        == string.Empty             ? "" : preferredNbr + "<br/>") +
                    (curBooking.Offering == null                     ? "" : curBooking.Offering.Name) +
                    (!isShowable || (!userView.IsProvider && !userView.IsAdminView) || !hasOwing  || curBooking.Offering == null ? "" : "<br />" + owingText);
            }
            else // clinics
            {
                bool includeOrg = incOrg;
                bool incProvider = false;

                string notesText          = String.Empty;
                string ptNotesText        = String.Empty;
                string ptMedsNotesText    = String.Empty;
                string ptMedCondNotesText = String.Empty;
                string ptFilesText        = String.Empty;

                notesText = Note.GetBookingPopupLinkTextV2(curBooking.EntityID, curBooking.NoteCount > 0, true, 1425, 700, "images/notes-bw-24-Thin.png", "images/notes-24-Thin.png", null, !IsMobileDevice);

                if (curBooking.Patient != null)
                {
                    //string notesText       = Note.GetPopupLinkTextV2(15, curBooking.EntityID, curBooking.NoteCount > 0, true, 980, 530, "images/notes-bw-24-Thin.png", "images/notes-24-Thin.png", null, !IsMobileDevice);
                    bool   ptHasNotes        = patientsHasNotesCache[curBooking.Patient.Person.EntityID] != null;
                    ptNotesText              = Note.GetPopupLinkTextV2(16, curBooking.Patient.Person.EntityID, ptHasNotes,        true, 1270, 640, "images/BC-24-Thin-bw.png", "images/BC-24-Thin.png", null, !IsMobileDevice);
                    bool   ptHasMedsNotes    = patientsHasMedNotesCache[curBooking.Patient.Person.EntityID] != null;
                    ptMedsNotesText          = Note.GetPopupLinkTextV2(17, curBooking.Patient.Person.EntityID, ptHasMedsNotes,    true, 1050,  530, "images/M-24-Thin-bw.png",  "images/M-24-Thin.png",  null, !IsMobileDevice);
                    bool   ptHasMedCondNotes = patientsHasMedCondNotesCache[curBooking.Patient.Person.EntityID] != null;
                    ptMedCondNotesText       = Note.GetPopupLinkTextV2(18, curBooking.Patient.Person.EntityID, ptHasMedCondNotes, true, 1050,  530, "images/MC-24-Thin-bw.png", "images/MC-24-Thin.png", null, !IsMobileDevice);
                    ptFilesText              = curBooking.GetScannedDocsImageLink();
                }

                bool hasArrived = curBooking.ArrivalTime != DateTime.MinValue;
                string jsArrivedText = hasArrived ?
                                            "javascript:ajax_booking_confirm_delete_cancel('unarrived', '" + curBooking.BookingID.ToString() + "');" :
                                            "javascript:ajax_booking_confirm_delete_cancel('arrived', '" + curBooking.BookingID.ToString() + "');";
                string imgArrivedText = "images/person2colour-24.png";
                string setArrivedText = "<input type=\"image\" title=\"" + (hasArrived ? "Unset Arrived" : "Set Arrived") + "\" src=\"" + imgArrivedText + "\" alt=\"" + (hasArrived ? "Unset Arrived" : "Set Arrived") + "\" onclick=\"" + jsArrivedText + "\" />";
                if (curBooking.ArrivalTime != DateTime.MinValue)
                    setArrivedText += "&nbsp;<b>" + curBooking.ArrivalTime.ToString("HH:mm") + "</b>";



                bool isConfirmed = curBooking.DateConfirmed != DateTime.MinValue;
                string jsConfirmedText = isConfirmed ?
                                            "javascript:ajax_booking_confirm_delete_cancel('unconfirm', '" + curBooking.BookingID.ToString() + "');" :
                                            "javascript:ajax_booking_confirm_delete_cancel('confirm', '" + curBooking.BookingID.ToString() + "');";
                string imgConfirmedText = isConfirmed ? "images/tick2color-24.png" : "images/tick2bw-24.png";
                string setConfirmedText = "<input type=\"image\" title=\"" + (isConfirmed ? "Set Unconfirmed" : "Set Confirmed") + "\" src=\"" + imgConfirmedText + "\" alt=\"" + (isConfirmed ? "Set Unconfirmed" : "Set Confirmed") + "\" onclick=\"" + jsConfirmedText + "\" />";


                string flashingNote = String.Empty;
                string flasingTextIcon = String.Empty;
                if (curBooking.Patient != null)
                {
                    bool hasFlashingNote = patientsFlashingNotesCache[curBooking.Patient.PatientID] != null;
                    flashingNote         = hasFlashingNote ? (string)patientsFlashingNotesCache[curBooking.Patient.PatientID] : "";
                    flasingTextIcon      = Patient.GetFlashingTextLink(curBooking.Patient.PatientID, hasFlashingNote, flashingNote, true, 525, 275, "images/asterisk_bw_16.png", "images/asterisk_red_16.gif", null, !IsMobileDevice);
                }

                bool   hasOwing  = false;
                string owingText = string.Empty;
                if (curBooking.Patient != null)
                {
                    hasOwing  = patientsOwingCache[curBooking.Patient.PatientID] != null;
                    owingText = hasOwing ? "<b><font color=\"red\">Owes $" + Convert.ToDecimal(patientsOwingCache[curBooking.Patient.PatientID]).ToString ("0.00") + "</font></b>" : string.Empty;
                }


                //string printLetterJS = "javascript:window.location.href='Letters_PrintV2.aspx?booking="+curBooking.BookingID+"';return false;";
                //string printLetterImg = "images/printer_green-24.png";
                //string printLetterText = "<input type=\"image\" title=\"Print Letter\" src=\"" + printLetterImg + "\" alt=\"Print Letter\" onclick=\"" + printLetterJS + "\" />";

                string preferredNbr = string.Empty;
                if (curBooking.Patient != null)
                {
                    if (Utilities.GetAddressType().ToString() == "Contact")
                    {
                        Contact[] phoneNbrs = GetPatientsPhoneNumbersFromCache(patientsPhoneNumbersCache, curBooking.Patient.Person.EntityID);
                        if (phoneNbrs != null && phoneNbrs.Length > 0)
                        {
                            preferredNbr = phoneNbrs[0].PhoneNumberWithDashes;

                            bool curIsMobile = phoneNbrs[0].ContactType.ContactTypeID == 30;

                            for (int i = 1; i < phoneNbrs.Length; i++)
                            {
                                if (phoneNbrs[i].ContactType.ContactTypeID == 30 && !curIsMobile)
                                {
                                    curIsMobile = phoneNbrs[i].ContactType.ContactTypeID == 30;
                                    preferredNbr = phoneNbrs[i].PhoneNumberWithDashes;
                                }
                            }
                        }
                    }
                    else if (Utilities.GetAddressType().ToString() == "ContactAus")
                    {
                        ContactAus[] phoneNbrs = GetPatientsPhoneNumbersFromAusCache(patientsPhoneNumbersCache, curBooking.Patient.Person.EntityID);
                        if (phoneNbrs != null && phoneNbrs.Length > 0)
                        {
                            preferredNbr = phoneNbrs[0].PhoneNumberWithDashes;

                            bool curIsMobile = phoneNbrs[0].ContactType.ContactTypeID == 30;

                            for (int i = 1; i < phoneNbrs.Length; i++)
                            {
                                if (phoneNbrs[i].ContactType.ContactTypeID == 30 && !curIsMobile)
                                {
                                    curIsMobile = phoneNbrs[i].ContactType.ContactTypeID == 30;
                                    preferredNbr = phoneNbrs[i].PhoneNumberWithDashes;
                                }
                            }
                        }
                    }
                    else
                        throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
                }

                if (preferredNbr.Length > 0 && callCenterPrefix.Length > 0)
                    preferredNbr = callCenterPrefix + " " + preferredNbr;


                int  loggedInStaffID   = Session["StaffID"]       == null ? -1 : Convert.ToInt32(Session["StaffID"]);
                bool canSeePatientLink = userView.IsAdminView || userView.IsPrincipal || userView.IsExternal|| (curBooking.Patient != null && curBooking.Provider != null && curBooking.Provider.StaffID == loggedInStaffID);

                string patientLink = string.Empty;
                if (curBooking.Patient != null)
                {
                    string ptURL  = "PatientDetailV2.aspx?type=view&id=" + curBooking.Patient.PatientID;
                    //patientLink = curBooking.Patient == null ? "" : (canSeePatientLink ? flasingTextIcon + " " + "<a href=\"#\" onclick=\"open_new_window('PatientDetailV2.aspx?type=view&id=" + curBooking.Patient.PatientID + "', 1750, 1000);return false;\">" + curBooking.Patient.Person.FullnameWithoutMiddlename + (curBooking.Patient.Person.Nickname.Trim().Length == 0 ? "" : " (" + curBooking.Patient.Person.Nickname.Trim() + ")") + "</a><br />" : "<b>" + curBooking.Patient.Person.FullnameWithoutMiddlename + "</b><br />");
                    patientLink   = curBooking.Patient == null ? "" : (canSeePatientLink ? flasingTextIcon + " " + "<a href=\"" + ptURL + "\" onclick=\"var win=window.open('" + ptURL + "', '_blank'); win.focus();return false;\">" + curBooking.Patient.Person.FullnameWithoutMiddlename + (curBooking.Patient.Person.Nickname.Trim().Length == 0 ? "" : " (" + curBooking.Patient.Person.Nickname.Trim() + ")") + "</a><span style=\"color:transparent;\">-</span>" + (curBooking.Patient.Person.Dob == DateTime.MinValue ? "" : "(" + Utilities.GetAge(curBooking.Patient.Person.Dob) + ")") + "<br />" : "<b>" + curBooking.Patient.Person.FullnameWithoutMiddlename + "</b><br />");
                }

                bool isCancelled = curBooking.BookingStatus.ID == 188;
                bool notYetCompleted = curBooking.BookingStatus.ID == 0;

                // "td_1751_76_2015_02_16_0930"// 
                string id = "td_" + curBooking.Organisation.OrganisationID + "_" + curBooking.Provider.StaffID + "_" + curBooking.DateStart.ToString("yyyy_MM_dd_HHmm");


                string firstRow = string.Empty;
                if (isShowable && !isPatientLoggedIn && !userView.IsExternalView && curBooking.Patient != null)
                {
                    firstRow = (bkHasChangeHistory ? curBooking.GetBookingChangeHistoryPopupLinkImage(null, false, !IsMobileDevice) + "&nbsp;" : "") + notesText + "&nbsp;" + (SiteIsGP ? ptMedsNotesText + "&nbsp;" + ptMedCondNotesText + "&nbsp;" : "") + (ptFilesText + "&nbsp;") + (notYetCompleted ? setConfirmedText + "&nbsp;" + setArrivedText : "");
                }
                else // first row still blank
                {
                    if (notYetCompleted && (userView.IsExternalView || isProviderViewAndOtherProviderBk))
                        firstRow += setArrivedText;
                }
                if (firstRow.Length > 0) firstRow += "<br />";


                if (SiteIsAgedCare || SiteIsClinic)
                {
                    return

                        // don't put notes in because the note is not linked to which PT in the group booking, so at end of PT page, it can't show the note only for that PT
                        //(curBooking.Patient == null ? ((userView.IsAdminView || (userView.IsProviderView && !isProviderViewAndOtherProviderBk)) ? notesText + "&nbsp;" : "") + "<font color=\"blue\"><b><i>Group Booking</i></b></font><br/>" : "") +
                        (curBooking.Patient == null ?  "<font color=\"blue\"><b><i>Group Booking</i></b></font><br/>" : "") +
                        firstRow +
                        (bookingTypeText.Length <= 0                                 ? "" : bookingTypeText + "<br/>") +
                        (!isCancelled                                                ? "" : "<b>[Cancelled" + (curBooking.InvoiceCount > 0 ? "" : " - No Inv") + "]</b><br/>") +
                        (!includeOrg  || curBooking.Organisation == null             ? "" : "<i>[" + curBooking.Organisation.Name + "]</i><br/>") +
                        (!incProvider || curBooking.Provider == null                 ? "" : curBooking.Provider.Person.FullnameWithoutMiddlename + "<br/>") +
                        (curBooking.Patient == null || !curBooking.Patient.IsDiabetic || isPatientLoggedIn ? "" : "<font color=\"blue\"><b><i>PT is Diabetic</i></b></font><br/>") +
                        patientLink +
                        (preferredNbr == string.Empty                                ? "" : preferredNbr + "<br/>") +
                        (curBooking.Offering == null                                 ? "" : curBooking.Offering.Name) +
                        (!isShowable || (!userView.IsProvider && !userView.IsAdminView) || !hasOwing || curBooking.Offering == null ? "" : "<br />" + owingText) +
                        (isShowable && bookingSlotType != BookingSlot.Type.None ? "<a href='javascript:void(0)' title=\"Long Click To Show Menu\r\nFor Mouse Without A Right Click Button\" onclick=\"ShowMenu_" + GetContextMenuClass(bookingSlotType, curBooking) + "('" + id + "');return false;\" style=\"text-decoration:none;\">&nbsp;★&nbsp;</a>&nbsp;" : "");
                }
                else
                {
                    return

                        // don't put notes in because the note is not linked to which PT in the group booking, so at end of PT page, it can't show the note only for that PT
                        //(curBooking.Patient == null ? ((userView.IsAdminView || (userView.IsProviderView && !isProviderViewAndOtherProviderBk)) ? notesText + "&nbsp;" : "") + "<font color=\"blue\"><b><i>Group Booking</i></b></font><br/>" : "") +
                        (curBooking.Patient == null ? "<font color=\"blue\"><b><i>Group Booking</i></b></font><br/>" : "") +
                        firstRow +
                        (!isCancelled                                                ? "" : "<b>[Cancelled" + (curBooking.InvoiceCount > 0 ? "" : " - No Inv") + "]</b><br/>") +
                        (!includeOrg  || curBooking.Organisation == null             ? "" : "<i>[" + curBooking.Organisation.Name + "]</i><br/>") +
                        (!incProvider || curBooking.Provider     == null             ? "" : curBooking.Provider.Person.FullnameWithoutMiddlename + "<br/>") +
                        (curBooking.Patient == null || !curBooking.Patient.IsDiabetic || isPatientLoggedIn ? "" : "<font color=\"blue\"><b><i>PT is Diabetic</i></b></font><br/>") +
                        patientLink +
                        (preferredNbr == string.Empty                                ? "" : preferredNbr + "<br/>") +
                        (curBooking.Offering == null                                 ? "" : curBooking.Offering.Name) +
                        (!isShowable || (!userView.IsProvider && !userView.IsAdminView) || !hasOwing  || curBooking.Offering == null ? "" : "<br />" + owingText) +
                        (isShowable && bookingSlotType != BookingSlot.Type.None ? "<a href='javascript:void(0)' onclick=\"ShowMenu_Menu_" + GetContextMenuClass(bookingSlotType, curBooking) + "('" + id + "');return false;\" style=\"text-decoration:none;\">&nbsp;★&nbsp;</a>&nbsp;" : "");
                }

            }
        }
    }

    protected void UpdateCells(int nSlots, Booking curBooking, string lblTextFirstCell, string lblTextFirstCellWithOrg, string lblTextRestCells, bool useFirstTimeOfDayAsFirstText, Organisation curOrgToUpdate, Booking booking, BookingSlot.Type slotType, bool canBook = true)
    {
        string firstRow = GetFirstTimeOfDay();

        TimeSpan timeToAddPerSlot = new TimeSpan(0, 0, 0);
        for (int i = 0; i < nSlots; i++)
        {
            bool isFirstSlot = curBooking.DateStart.Add(timeToAddPerSlot).ToString("HHmm") == firstRow || i == 0;
            string text = isFirstSlot ? (slotType == BookingSlot.Type.Unavailable ? lblTextFirstCellWithOrg : lblTextFirstCell) : lblTextRestCells;
            string nameText = isFirstSlot && booking.Patient != null && slotType != BookingSlot.Type.Unavailable ? booking.Patient.Person.FullnameWithoutMiddlename : "";

            if (curBooking.Provider != null)
                UpdateCell(curOrgToUpdate, curBooking, timeToAddPerSlot, text, nameText, slotType, canBook);
            else // all providers
            {
                Staff original = curBooking.Provider;
                foreach (Staff s in this.staffList)
                {
                    curBooking.Provider = s;
                    UpdateCell(curOrgToUpdate, curBooking, timeToAddPerSlot, text, nameText, slotType, canBook);
                }
                curBooking.Provider = original;
            }

            timeToAddPerSlot = timeToAddPerSlot.Add(new TimeSpan(0, bookingSlotMinsCache.GetSlotMins(), 0));
        }
    }

    protected void UpdateCell(Organisation curOrgToUpdate, Booking booking, TimeSpan timeToAdd, string text, string nameText, BookingSlot.Type slotType, bool canBook = true)
    {
        Hashtable OfferingColors = (Hashtable)Session["OfferingColors"];

        ArrayList bookingStartDateTimes = new ArrayList();
        if (booking.IsRecurring)
        {
            for (int i = 0; i < this.daysData.Length; i++)
                if (booking.RecurringDayOfWeek == this.daysData[i].Date.DayOfWeek)
                    bookingStartDateTimes.Add(this.daysData[i].Date.Add(booking.RecurringStartTime));
        }
        else
        {
            bookingStartDateTimes.Add(booking.DateStart);
        }

        foreach (DateTime bookingStartDateTime in bookingStartDateTimes)
        {
            // change to beginning of hour for aged care since its hourly bookings
            DateTime date = bookingSlotMinsCache.GetSlotMins() == 60 ? bookingStartDateTime.Subtract(new TimeSpan(0, bookingStartDateTime.Minute, 0)).Add(timeToAdd) : bookingStartDateTime.Add(timeToAdd);
            //string cellID = booking.Organisation.OrganisationID + "_" + booking.Provider.StaffID + "_" + date.Date.ToString("yyyy_MM_dd") + "_" + date.Hour.ToString().PadLeft(2, '0') + date.Minute.ToString().PadLeft(2, '0');
            string cellID = curOrgToUpdate.OrganisationID + "_" + booking.Provider.StaffID + "_" + date.Date.ToString("yyyy_MM_dd") + "_" + date.Hour.ToString().PadLeft(2, '0') + date.Minute.ToString().PadLeft(2, '0');

            string lblID = "lbl_" + cellID;
            Control lbl = GetControlByID2(lblID, main_table);
            if (lbl != null)
            {
                System.Drawing.Color color = booking.Offering != null && OfferingColors[booking.Offering.OfferingID] != null && !IsEditBookingMode() && !IsEditDayMode() ?
                    (System.Drawing.Color)System.Drawing.ColorTranslator.FromHtml("#" + OfferingColors[booking.Offering.OfferingID]) :
                    BookingSlot.GetColor(slotType);

                //((Label)lbl).BackColor = IsEditBookingMode() || IsEditDayMode() ? HSLColor.ChangeBrightness(BookingSlot.GetColor(slotType), -10) : BookingSlot.GetColor(slotType);
                ((Label)lbl).BackColor = IsEditBookingMode() || IsEditDayMode() ? HSLColor.ChangeBrightness(color, -10) : color;
                ((Label)lbl).Text = text;
            }


            string tdID = "td_" + cellID;
            Control cell = GetControlByID2(tdID, main_table);
            if (cell != null)
            {
                ((HtmlTableCell)cell).Attributes.Remove("ondblclick");
                ((HtmlTableCell)cell).Attributes.Remove("onmousedown");
                ((HtmlTableCell)cell).Attributes.Remove("onmouseup");

                if (!Utilities.IsDev())
                    ((HtmlTableCell)cell).BgColor = System.Drawing.ColorTranslator.ToHtml(((Label)lbl).BackColor);
                else
                    ((HtmlTableCell)cell).BgColor = System.Drawing.ColorTranslator.ToHtml(HSLColor.ChangeBrightness(((Label)lbl).BackColor, 35));

                if (canBook)
                {
                    ((HtmlTableCell)cell).Attributes["class"] = GetContextMenuClass(slotType, booking);
                }
                else
                {
                    ((HtmlTableCell)cell).Attributes.Remove("class");
                    ((HtmlTableCell)cell).Attributes["class"] = BookingSlot.GetContextMenuClass(BookingSlot.Type.None);
                }

                // Add hiddencol labels
                NewOrUpdateLabel(cell, "lblBookingID_" + cellID, booking.BookingID.ToString());
                if (booking.Patient != null)
                    NewOrUpdateLabel(cell, "lblPatientName_" + cellID, nameText, false, true);
                if (booking.Offering != null)
                    NewOrUpdateLabel(cell, "lblOfferingID_" + cellID, booking.Offering.OfferingID.ToString());
                if (booking.Provider != null)
                    NewOrUpdateLabel(cell, "lblProviderID_" + cellID, booking.Provider.StaffID.ToString());
                if (booking.Organisation != null)
                    NewOrUpdateLabel(cell, "lblOrgID_" + cellID, booking.Organisation.OrganisationID.ToString());
            }


        }
    }

    protected string GetContextMenuClass(BookingSlot.Type bookingSlotType, Booking curBooking)
    {
        if (bookingSlotType == BookingSlot.Type.Booking_CL_EPC_Past_Completed_Has_Invoice ||
            bookingSlotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Completed_Has_Invoice ||
            bookingSlotType == BookingSlot.Type.Booking_AC_EPC_Past_Completed_Has_Invoice ||
            bookingSlotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_Has_Invoice)
        {
            if (curBooking.Patient == null)  // set as ac slot (ie with treatment list menu item) - use any AC from the 'if' statement
                return BookingSlot.GetContextMenuClass(BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_Has_Invoice);
            else                          // set as clinic slot (ie without treatment list menu item) - use either Clinic from the 'if' statement
                return BookingSlot.GetContextMenuClass(BookingSlot.Type.Booking_CL_NonEPC_Past_Completed_Has_Invoice);
        }
        else if (bookingSlotType == BookingSlot.Type.Booking_CL_EPC_Future_Unconfirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_CL_EPC_Future_Confirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_AC_EPC_Future_Unconfirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_AC_EPC_Future_Confirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_CL_NonEPC_Future_Unconfirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_CL_NonEPC_Future_Confirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_AC_NonEPC_Future_Unconfirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_AC_NonEPC_Future_Confirmed)
        {
            if (curBooking.Patient == null)  // set as ac slot (ie with complete  menu item) - use any AC from the 'if' statement
                return BookingSlot.GetContextMenuClass(BookingSlot.Type.Booking_AC_NonEPC_Future_Unconfirmed);
            else                          // set as clinic slot (ie without complete menu item) - use any Clinic from the 'if' statement
                return BookingSlot.GetContextMenuClass(BookingSlot.Type.Booking_CL_NonEPC_Future_Unconfirmed);
        }
        else if (bookingSlotType == BookingSlot.Type.Booking_CL_EPC_Past_Uncompleted_Unconfirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_CL_EPC_Past_Uncompleted_Confirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Uncompleted_Unconfirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Uncompleted_Confirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_AC_EPC_Past_Uncompleted_Unconfirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_AC_EPC_Past_Uncompleted_Confirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Uncompleted_Unconfirmed ||
                 bookingSlotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Uncompleted_Confirmed)
        {
            if (curBooking.Patient == null)  // set as ac slot (ie with complete  menu item) - use any AC from the 'if' statement
                return BookingSlot.GetContextMenuClass(BookingSlot.Type.Booking_AC_NonEPC_Past_Uncompleted_Unconfirmed);
            else                          // set as clinic slot (ie without complete menu item) - use any Clinic from the 'if' statement
                return BookingSlot.GetContextMenuClass(BookingSlot.Type.Booking_CL_NonEPC_Past_Uncompleted_Unconfirmed);
        }
        else
        {
            return BookingSlot.GetContextMenuClass(bookingSlotType);
        }
    }

    protected void NewOrUpdateLabel(Control cell, string lblID, string text, bool useHiddencol = true, bool useHiddenBlock = false)
    {
        Control lbl = GetControlByID(lblID, cell);
        if (lbl == null)
            cell.Controls.Add(NewHiddenLabel(lblID, text, useHiddencol, useHiddenBlock));
        else
            ((Label)lbl).Text = text;
    }

    protected Label NewHiddenLabel(string cellID, string text, bool useHiddencol = true, bool useHiddenBlock = false)
    {
        Label newLbl = new Label();
        newLbl.ID = cellID;
        newLbl.Text = text;
        if (useHiddencol)
            newLbl.CssClass = "hiddencol";
        if (useHiddenBlock)
            newLbl.Style["display"] = "none";
        return newLbl;
    }

    protected Control GetControlByID(string id, Control parent)
    {
        foreach (Control c in parent.Controls)
        {
            if (c.ID != null && c.ID == id)
                return c;

            Control found = GetControlByID(id, c);
            if (found != null)
                return found;
        }
        return null;
    }

    Hashtable mainTableControlHash = null;
    protected Control GetControlByID2(string id, Control parent)
    {
        if (this.mainTableControlHash == null)
        {
            this.mainTableControlHash = new Hashtable();
            SetMainTableControlHash(main_table);
        }

        return this.mainTableControlHash[id] == null ? null : (Control)this.mainTableControlHash[id];
    }
    protected void SetMainTableControlHash(Control parent)
    {
        foreach (Control c in parent.Controls)
        {
            if (c.ID != null)
                this.mainTableControlHash[c.ID] = c;
            SetMainTableControlHash(c);
        }
    }


    protected string _FirstTimeOfDay = null;
    protected string GetFirstTimeOfDay(bool startHour = false)
    {
        if (this._FirstTimeOfDay != null)
            return this._FirstTimeOfDay;

        //string[] parts = main_table.Rows[0].Cells[0].InnerText.Split(new char[] { ':', '-' });
        //
        //if (!startHour || parts[1].PadLeft(2, '0') == "00")
        //    return (parts[0].PadLeft(2, '0') + parts[1].PadLeft(2, '0'));
        //else
        //    return (Convert.ToInt32(parts[0].PadLeft(2, '0')) + 1).ToString().PadLeft(2, '0') + "00";

        TimeSpan startTime = GetStartEndTimes().StartTime;
        if (!startHour || startTime.Minutes == 0)
            this._FirstTimeOfDay = startTime.Hours.ToString().PadLeft(2, '0') + startTime.Minutes.ToString().PadLeft(2, '0');
        else
            this._FirstTimeOfDay = (startTime.Hours + 1).ToString().PadLeft(2, '0') + "00";

        return this._FirstTimeOfDay;
    }


    protected Hashtable GetPatientHealthCardCache(Booking[] bookings)
    {
        ArrayList patientIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Patient != null)
                patientIDArrayList.Add(curBooking.Patient.PatientID);

        int[] patientIDs = (int[])patientIDArrayList.ToArray(typeof(int));
        return PatientsHealthCardsCacheDB.GetBullkActive(patientIDs);
    }
    protected HealthCard GetHealthCardFromCache(Hashtable patientHealthCardCache, int patientID)
    {
        HealthCard hc = null;
        if (patientHealthCardCache[patientID] != null)
        {
            PatientActiveHealthCards hcs = (PatientActiveHealthCards)patientHealthCardCache[patientID];
            if (hcs.MedicareCard != null)
                hc = hcs.MedicareCard;
            if (hcs.DVACard != null)
                hc = hcs.DVACard;
            if (hcs.TACCard != null)
                hc = hcs.TACCard;
        }

        return hc;
    }


    protected Hashtable GetEPCRemainingCache(Hashtable patientHealthCardCache)
    {
        ArrayList healthCardIDs = new ArrayList();
        foreach (PatientActiveHealthCards ptHCs in patientHealthCardCache.Values)
        {
            if (ptHCs.MedicareCard != null)
                healthCardIDs.Add(ptHCs.MedicareCard.HealthCardID);
            if (ptHCs.DVACard != null)
                healthCardIDs.Add(ptHCs.DVACard.HealthCardID);
        }

        return HealthCardEPCRemainingDB.GetHashtableByHealthCardIDs((int[])healthCardIDs.ToArray(typeof(int)));
    }


    protected Hashtable GetPatientsPhoneNumbersCache(Booking[] bookings)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Patient != null)
                entityIDArrayList.Add(curBooking.Patient.Person.EntityID);

        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));



        return PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1);
    }
    protected Contact[] GetPatientsPhoneNumbersFromCache(Hashtable patientsPhoneNumbersCache, int entityID)
    {
        if (patientsPhoneNumbersCache[entityID] != null)
            return (Contact[])patientsPhoneNumbersCache[entityID];
        else
            return null;
    }
    protected ContactAus[] GetPatientsPhoneNumbersFromAusCache(Hashtable patientsPhoneNumbersCache, int entityID)
    {
        if (patientsPhoneNumbersCache[entityID] != null)
            return (ContactAus[])patientsPhoneNumbersCache[entityID];
        else
            return null;
    }


    protected Hashtable GetPatientsMedicareCountThisYearCache(Booking[] bookings, int year)
    {
        ArrayList patientIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Patient != null)
                patientIDArrayList.Add(curBooking.Patient.PatientID);

        int[] patientIDs = (int[])patientIDArrayList.ToArray(typeof(int));
        return PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, year);
    }
    protected Hashtable GetPatientsEPCRemainingCacheDB(Booking[] bookings, DateTime startDate)
    {
        ArrayList patientIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Patient != null)
                patientIDArrayList.Add(curBooking.Patient.PatientID);

        int[] patientIDs = (int[])patientIDArrayList.ToArray(typeof(int));
        return PatientsEPCRemainingCacheDB.GetBullk(patientIDs, startDate);
    }


    protected Hashtable GetPatientsHasNotesCache(Booking[] bookings)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Patient != null)
                entityIDArrayList.Add(curBooking.Patient.Person.EntityID);

        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));
        return NoteDB.GetHasNotesHash(entityIDs, null, new int[]{ 1,2 });
    }
    protected Hashtable GetPatientsHasMedNotesCache(Booking[] bookings)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Patient != null)
                entityIDArrayList.Add(curBooking.Patient.Person.EntityID);

        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));
        return NoteDB.GetHasNotesHash(entityIDs, new int[]{ 1 });
    }
    protected Hashtable GetPatientsHasMedCondNotesCache(Booking[] bookings)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Patient != null)
                entityIDArrayList.Add(curBooking.Patient.Person.EntityID);

        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));
        return NoteDB.GetHasNotesHash(entityIDs, new int[] { 2 });
    }


    #endregion


    #region SetupGUI, SetPatientEPCInfo, ShowPatientEPCInfo

    protected void SetupGUI()
    {
        UserView userView = UserView.GetInstance();

        if (IsEditBookingMode())
        {
            /*
            colorkey_available.Style["background"]            = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Updatable));
            colorkey_unavailable.Style["background"]          = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.UnavailableButAddable));
            colorkey_completed.Style["background"]            = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.UpdatableConfirmable));
            colorkey_completed_text.InnerText                 = "Booking To Edit";
            */

            colorkey_available2.Style["background"]            = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Updatable));
            colorkey_unavailable2.Style["background"]          = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.UnavailableButAddable));
            colorkey_completed2.Style["background"]            = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.UpdatableConfirmable));
            colorkey_completed_text2.InnerText                 = "Booking To Edit";

        }
        else
        {
            /*
            colorkey_available.Style["background"]            = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Available));
            colorkey_unavailable.Style["background"]          = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.UnavailableButAddable));
            colorkey_completed.Style["background"]            = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_Has_Invoice));
            colorkey_completed_text.InnerText                 = "Completed";
            */

            colorkey_available2.Style["background"]            = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Available));
            colorkey_unavailable2.Style["background"]          = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.UnavailableButAddable));
            colorkey_completed2.Style["background"]            = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_Has_Invoice));
            colorkey_completed_text2.InnerText                 = "Completed";

        }

        /*
        colorkey_ac_uncomf_nonepc.Style["background"]     = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_NonEPC_Future_Unconfirmed));
        colorkey_ac_comf_nonepc.Style["background"]       = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_NonEPC_Future_Confirmed));
        colorkey_ac_uncomf_epc.Style["background"]        = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_EPC_Future_Unconfirmed));
        colorkey_ac_comf_epc.Style["background"]          = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_EPC_Future_Confirmed));

        colorkey_clinic_uncomf_nonepc.Style["background"] = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_NonEPC_Future_Unconfirmed));
        colorkey_clinic_comf_nonepc.Style["background"]   = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_NonEPC_Future_Confirmed));
        colorkey_clinic_uncomf_epc.Style["background"]    = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_EPC_Future_Unconfirmed));
        colorkey_clinic_comf_epc.Style["background"]      = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_EPC_Future_Confirmed));
        */

        colorkey_ac_uncomf_nonepc2.Style["background"]     = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_NonEPC_Future_Unconfirmed));
        colorkey_ac_comf_nonepc2.Style["background"]       = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_NonEPC_Future_Confirmed));
        colorkey_ac_uncomf_epc2.Style["background"]        = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_EPC_Future_Unconfirmed));
        colorkey_ac_comf_epc2.Style["background"]          = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_EPC_Future_Confirmed));

        colorkey_clinic_uncomf_nonepc2.Style["background"] = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_NonEPC_Future_Unconfirmed));
        colorkey_clinic_comf_nonepc2.Style["background"]   = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_NonEPC_Future_Confirmed));
        colorkey_clinic_uncomf_epc2.Style["background"]    = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_EPC_Future_Unconfirmed));
        colorkey_clinic_comf_epc2.Style["background"]      = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_EPC_Future_Confirmed));



        txtStartDate.Text = GetFormDate().ToString("dd-MM-yyyy");

        for (int i = 1; i <= 8; i++)
            ddlDaysToDisplay.Items.Add(new ListItem(i.ToString(), i.ToString()));

        int ndays = GetFormNumDays();
        if (ndays < 1) ndays = 1;
        if (ndays > 8) ndays = 8;
        ddlDaysToDisplay.SelectedValue = ndays.ToString();

        if (Convert.ToInt32(ddlDaysToDisplay.SelectedValue) != Convert.ToInt32(Session["NumDaysToDisplayOnBookingScreen"]))
        {
            if (Session["StaffID"] != null)
                StaffDB.UpdateNumDaysToDisplayOnBookingScreen(Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(ddlDaysToDisplay.SelectedValue));
            Session["NumDaysToDisplayOnBookingScreen"] = Convert.ToInt32(ddlDaysToDisplay.SelectedValue);
        }



        chkShowDetails.Checked = !GetFormIsCollapsed();
        chkShowUnavailableStaff.Checked = GetFormShowUnavailableStaff();
        chkShowOtherProviders.Checked = (bool)Session["ShowOtherProvidersOnBookingScreen"];
        tr_show_other_providers.Visible = userView.IsProviderView;


        bool isValidFormPatient    = IsValidFormPatient();
        bool isValidSessionPatient = IsValidSessionPatient();
        if (isValidSessionPatient || isValidFormPatient)
        {
            Patient patient = PatientDB.GetByID(isValidSessionPatient ? GetSessionPatient() : GetFormPatient()); // choose session first
            if (patient.FlashingTextAddedBy != null)
                patient.FlashingTextAddedBy = StaffDB.GetByID(patient.FlashingTextAddedBy.StaffID);

            txtPatientID.Text = patient.PatientID.ToString();
            txtUpdatePatientName.Text = patient.Person.FullnameWithoutMiddlename;
            UpdateTextbox(txtUpdatePatientName, txtPatientID.Text.Length == 0);
            string addedByHoverLink = patient.FlashingTextAddedBy == null ? "" : @"<a href=""javascript:void(0)"" onclick=""javascript:return false;"" title='Added By: " + patient.FlashingTextAddedBy.Person.FullnameWithoutMiddlename + @"'>*</a>";
            string date = (patient.FlashingText.Length == 0 || patient.FlashingTextLastModifiedDate == DateTime.MinValue ? "" : @"[" + patient.FlashingTextLastModifiedDate.ToString("d'/'M'/'yy") + " " + addedByHoverLink + "]&nbsp;&nbsp;");
            string fashingText = patient.FlashingText;
            SetFlashingText(patient.PatientID, date, fashingText);
            SetPatientEPCInfo(patient);

            tr_selected_patient_row.Visible = true;
            string href = "PatientDetailV2.aspx?type=view&id=" + patient.PatientID;
            lblSelectedPatientName.Text = "<a href=\"" + href + "\" target=\"_blank\">" + patient.Person.FullnameWithoutMiddlename + "</a>";

            // update this in the other place also
            Invoice[] outstandingInvoices = InvoiceDB.GetOutstanding(patient.PatientID, Convert.ToInt32(Session["SiteID"]), false);
            lblInvoicesOwingMessage.Text = (outstandingInvoices.Length == 0) ? "" : "<font color=\"red\"><b>Outstanding Invoices:  $" + outstandingInvoices.Sum(item => item.TotalDue) + "</b></font>&nbsp;&nbsp;";
            br_invoices_owing_message_trailing_space.Visible = outstandingInvoices.Length > 0;

            if (isValidSessionPatient)
            {
                WebControlExtensions.AddCssClass(div_pt_search,       "hiddencol");
                WebControlExtensions.AddCssClass(div_pt_search_space, "hiddencol");
            }

        }
        else
        {
            ShowPatientEPCInfo(false);
            SetFlashingText(-1, string.Empty, string.Empty);

            tr_selected_patient_row.Visible = false;
        }




        bool isValidFormField = IsValidFormField();

        int selectedFieldID = !isValidFormField ? -1 : GetFormField();
        int selectedOfferingID = !IsValidFormOffering() ? -1 : GetFormOffering();

        if (!isValidFormField && Session["StaffID"] != null)  // get from db
        {
            IDandDescr field = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"])).BookingScreenField;
            if (field != null) selectedFieldID = field.ID;
        }


        // check if all providers are the same field ... if so use that as selected field
        int  curStaffFieldID     = -1;
        bool allStaffSameFieldID = false;
        for(int i=0; i<this.staffList.Length; i++)
        {
            if (curStaffFieldID == -1)
            {
                curStaffFieldID = this.staffList[i].Field.ID;
                allStaffSameFieldID = true;
            }
            else if (curStaffFieldID != this.staffList[i].Field.ID)
            {
                allStaffSameFieldID = false;
            }
        }
        if (allStaffSameFieldID)
            selectedFieldID = curStaffFieldID;


        ddlFields.Items.Add(new ListItem("All", "-1"));
        foreach (DataRow row in DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "has_offerings = 1 AND field_id <> 0", "descr", "field_id", "descr").Rows)
        {
            IDandDescr field = IDandDescrDB.Load(row, "field_id", "descr");
            ListItem li = new ListItem(field.Descr, field.ID.ToString());
            ddlFields.Items.Add(li);
        }

        if (selectedFieldID != -1)
            ddlFields.SelectedValue = selectedFieldID.ToString();

        ddlFields.Style["width"] = "290px";
        ddlServices.Style["width"] = "290px";
        if ((!IsEditBookingMode() && !IsEditDayMode()) || userView.IsAgedCareView)
            ddlServices.Items.Add(new ListItem("--", "-1"));
        if (IsValidFormOrgs())
        {

            bool containsClinic = false;
            bool containsAC     = false;
            for (int i = 0; i < this.orgs.Length; i++)
            {
                if (this.orgs[i].OrganisationType.OrganisationTypeID == 218)
                    containsClinic = true;
                else
                    containsAC = true;
            }

            string offering_invoice_type_ids = string.Empty;
            if (containsClinic && !containsAC)
                offering_invoice_type_ids = "1,3";
            else if (!containsClinic && containsAC)
                offering_invoice_type_ids = "3,4";
            else // has both or neither
            {
                if (userView.IsAgedCareView)            // logged into AC
                    offering_invoice_type_ids = "3,4";
                else                                    // logged into Clinic or GP
                    offering_invoice_type_ids = "1,3";
            }

            //bool isClinic = this.orgs.Length == 0 || this.orgs[0].OrganisationType.OrganisationTypeID == 218;
            //string offering_invoice_type_ids = isClinic ? "1,2,3" : "1,2,3,4";  // 1 = clinic, 4 = fac

            foreach (DataRow row in OfferingDB.GetDataTable(false, offering_invoice_type_ids, "63", true).Rows)
            {
                Offering offering = OfferingDB.LoadAll(row);

                bool offeringIsDeleted = Convert.ToBoolean(row["o_is_deleted"]);
                if (offeringIsDeleted && offering.OfferingID != selectedOfferingID)  // get rid of deleted offerings unless this is the offering previous selected such as for an edit booking)
                    continue;

                // if selected a field and this offering is different field -> and (not [edit mode && same offering]) -> dont add it in
                if (selectedFieldID != -1 && offering.Field.ID != selectedFieldID && ((!IsEditBookingMode() && !IsEditDayMode()) || offering.OfferingID != selectedOfferingID))
                    continue;

                ddlServices.Items.Add(new ListItem(offering.Name, offering.OfferingID.ToString()));
            }

            if (selectedOfferingID != -1)
                ddlServices.SelectedValue = selectedOfferingID.ToString();
        }

        // set items if in url!
        if (selectedOfferingID != -1)
            ddlServices.SelectedValue = selectedOfferingID.ToString();


        if (ddlServices.SelectedValue == "-1" && !IsEditBookingMode() && !IsEditDayMode())
        {
            int defaultServiceID = Convert.ToInt32(SystemVariableDB.GetByDescr("BookingScreenDefaultServiceID").Value);
            if (defaultServiceID != -1 && ddlServices.Items.FindByValue(defaultServiceID.ToString()) != null)
                ddlServices.SelectedValue = defaultServiceID.ToString();
        }



        if (userView.IsAgedCareView)
        {
            div_services.Visible                    = false;
            div_services_trailing_seperator.Visible = false;
        }


        if (userView.IsGPView)
            ShowPatientEPCInfo(false);

    }

    protected void SetPatientEPCInfo(Patient patient)
    {
        healthCardInfoControl.SetInfo(patient.PatientID, true, false, false, true, true, false, false);
    }

    protected void ShowPatientEPCInfo(bool show)
    {
        healthcardInfoRow.Visible = show;
    }

    #endregion

    #region SetTable

    protected bool mutlipleTimesShowing = true;

    protected void SetTable()
    {
        header_table.Controls.Clear();
        main_table.Controls.Clear();

        int totalDaysShowing = 0;
        for (int i = 0; i < this.daysData.Length; i++)
            if (this.daysData[i].NCols > 0)
                totalDaysShowing++;

        int timeCol = mutlipleTimesShowing ? totalDaysShowing * (108 + this.DaySeperatorColumnWidth) : 100;
        int totalWidthNotIncScrollbars = timeCol + this.NumColumns * this.columnWidth + this.daysData.Length * this.DaySeperatorColumnWidth;
        int totalWidthIncScrollbars = totalWidthNotIncScrollbars + 17;

        //header_table.Width = totalWidthNotIncScrollbars.ToString();
        //main_table.Width   = totalWidthNotIncScrollbars.ToString();
        header_table.Style["width"]     = totalWidthNotIncScrollbars + "px";
        main_table.Style["width"]       = totalWidthNotIncScrollbars + "px";
        header_table.Style["max-width"] = totalWidthNotIncScrollbars + "px";
        main_table.Style["max-width"]   = totalWidthNotIncScrollbars + "px";

        header_panel.Width = new Unit(totalWidthNotIncScrollbars);
        main_panel.Width   = new Unit(totalWidthIncScrollbars);
        int maxWidth = 1700;
        if (totalWidthNotIncScrollbars > maxWidth)
        {
            header_panel.Width = new Unit(maxWidth);
            main_panel.Width   = new Unit(maxWidth + 17);
        }



        AddHeader();
        AddBody();
    }
    protected void AddHeader()
    {
        UserView userView = UserView.GetInstance();

        int   loggedInStaffID           = !userView.IsStaff ? -1 : Convert.ToInt32(Session["StaffID"]);
        Staff loggedInStaff             = StaffDB.GetByID(loggedInStaffID);

        bool  isExternalStaffLoggedIn   = loggedInStaff        != null && loggedInStaff.IsExternal;
        bool  isCallCenterStaffLoggedIn = userView.IsStaff && (new List<int> { -5, -7, -8 }).Contains((int)Session["StaffID"]);
        bool  isPatientLoggedIn         = userView.IsPatient;



        Hashtable regStaffHash = RegisterStaffDB.Get2DHashByStaffIDDayID();


        HtmlTableRow htr_dates = new HtmlTableRow();
        HtmlTableRow htr_orgs  = new HtmlTableRow();
        HtmlTableRow htr       = new HtmlTableRow();

        int totalDaysShowing = 0;
        for (int i = 0; i < this.daysData.Length; i++)
            if (this.daysData[i].NCols > 0)
                totalDaysShowing++;

        int timeCol = totalDaysShowing == 1 ? 112 : 108;
        int width = this.columnWidth;


        bool addedTimeCol = false;
        for (int i = 0; i < this.daysData.Length; i++)
        {

            // dont display anything for days with no cols
            if (this.daysData[i].NCols == 0)
                continue;

            if (!addedTimeCol || mutlipleTimesShowing)
            {
                if (addedTimeCol)
                {
                    // add day seperator
                    //htr_dates.Controls.Add(NewBlankCell(true, this.DaySeperatorColumnWidth, true, System.Drawing.Color.Empty, (this.GetFormOrgs().Length > 1) ? 3 : 2));
                }

                htr_dates.Controls.Add(NewBlankCell(true, timeCol, true, System.Drawing.Color.Empty, (this.GetFormOrgs().Length > 1) ? 3 : 2));
                addedTimeCol = true;
            }


            // add day seperator
            //htr_dates.Controls.Add(NewBlankCell(true, this.DaySeperatorColumnWidth, true, System.Drawing.Color.Empty, (this.GetFormOrgs().Length > 1) ? 3 : 2));

            string time_cellID = "td_" + this.daysData[i].Date.ToString("yyyy_MM_dd");
            HtmlTableCell time_cell = NewCell(time_cellID, true, this.daysData[i].Date.ToString("ddd d MMM, yyyy"), System.Drawing.Color.Empty, -1, -1, null);
            //time_cell.Style["text-align"] = "center";
            time_cell.Attributes["class"] = "days";
            time_cell.ColSpan = this.daysData[i].NCols;
            htr_dates.Controls.Add(time_cell);

            bool seenFirst = false;
            for (int j = 0; j < this.daysData[i].OrgDayData.Length; j++)
            {

                // dont display anything for orgs with no cols
                if (this.daysData[i].OrgDayData[j].NCols == 0)
                    continue;


                // add org seperator
                if (j != 0 && seenFirst)
                {
                    //htr_orgs.Controls.Add(NewBlankCell(true, this.DaySeperatorColumnWidth, true, System.Drawing.Color.Empty));
                    //htr.Controls.Add(NewBlankCell(true, this.DaySeperatorColumnWidth, true, System.Drawing.Color.Empty));
                }

                seenFirst = true;

                string org_cellID = "td_" + this.daysData[i].OrgDayData[j].Org.OrganisationID + "_" + this.daysData[i].Date.ToString("yyyy_MM_dd");
                HtmlTableCell org_cell = NewCell(org_cellID, true, this.daysData[i].OrgDayData[j].Org.Name, System.Drawing.Color.Empty, -1, -1, null);
                org_cell.ColSpan = this.daysData[i].OrgDayData[j].StaffList.Length;
                htr_orgs.Controls.Add(org_cell);

                for (int k = 0; k < this.daysData[i].OrgDayData[j].StaffList.Length; k++)
                {
                    string hoverLink = "<a href=\"#\" onclick=\"$(this).tooltip().mouseover();return false;\" title=\"Hover over provider name to view locations set to be working at today.\">*</a>";
                    int    staffID   = this.daysData[i].OrgDayData[j].StaffList[k] != null ? this.daysData[i].OrgDayData[j].StaffList[k].StaffID : 0;
                    string staffName = this.daysData[i].OrgDayData[j].StaffList[k] != null ? this.daysData[i].OrgDayData[j].StaffList[k].Person.FullnameWithoutMiddlename + " " + hoverLink : "NO STAFF ALLOCATED";

                    string        cellID = "td_" + this.daysData[i].OrgDayData[j].Org.OrganisationID + "_" + staffID + "_" + this.daysData[i].Date.ToString("yyyy_MM_dd");
                    HtmlTableCell cell   = NewCell(cellID, true, staffName, System.Drawing.Color.Empty, width - 3, -1, null);

                    bool providerLoggedInButOtherProviderColumn = userView.IsProviderView && (this.daysData[i].OrgDayData[j].StaffList[k] == null || this.daysData[i].OrgDayData[j].StaffList[k].StaffID != loggedInStaffID);

                    if (IsEditBookingMode() || this.daysData[i].OrgDayData[j].StaffList[k] == null || providerLoggedInButOtherProviderColumn || isExternalStaffLoggedIn || isPatientLoggedIn)
                        cell.Attributes.Add("class", BookingSlot.GetContextMenuClass(BookingSlot.Type.None));
                    else if (IsEditDayMode())
                        cell.Attributes.Add("class", BookingSlot.GetContextMenuClass(BookingSlot.Type.FullDayUpdatable));
                    else
                        cell.Attributes.Add("class", BookingSlot.GetContextMenuClass(BookingSlot.Type.FullDayAvailable));


                    ArrayList list = (ArrayList)regStaffHash[new Hashtable2D.Key(staffID, (int)this.daysData[i].Date.DayOfWeek)];
                    ArrayList ClinicNamesList = new ArrayList();
                    ArrayList ACNamesList = new ArrayList();
                    if (list != null)
                        foreach (RegisterStaff regStaff in list)
                        {
                            if (regStaff.Organisation.IsDeleted)
                                continue;

                            if (regStaff.Organisation.IsClinic)
                                ClinicNamesList.Add(regStaff.Organisation.Name);
                            if (regStaff.Organisation.IsAgedCare)
                                ACNamesList.Add(regStaff.Organisation.Name);
                        }

                    ClinicNamesList.Sort();
                    ACNamesList.Sort();

                    string otherClinicsWorkingAt = string.Empty;

                    if (ClinicNamesList.Count > 0)
                    {
                        otherClinicsWorkingAt += (otherClinicsWorkingAt.Length == 0 ? "" : "\r\n\r\n\r\n") + "Working Today At These Clinics:";
                        foreach (string orgName in ClinicNamesList)
                            otherClinicsWorkingAt += "\n\r" + " • " + orgName;
                    }

                    if (ACNamesList.Count > 0)
                    {
                        otherClinicsWorkingAt += (otherClinicsWorkingAt.Length == 0 ? "" : "\r\n\r\n\r\n") + "Working Today At These Facilities/Wings:";
                        foreach (string orgName in ACNamesList)
                            otherClinicsWorkingAt += "\n\r" + " • " + orgName;
                    }

                    if (otherClinicsWorkingAt.Length > 0)
                        cell.Attributes["title"] = otherClinicsWorkingAt;

                    htr.Controls.Add(cell);
                }
            }
        }

        header_table.Controls.Add(htr_dates);
        if (this.GetFormOrgs().Length > 1)
            header_table.Controls.Add(htr_orgs);
        header_table.Controls.Add(htr);

        // if aged care, no scroll bars, so extend the header line 
        if (this.orgs.Length > 0 && this.orgs[0].OrganisationType.OrganisationTypeID != 218)
            header_panel.Width = main_panel.Width;
    }
    protected void AddBody(string bgColor = null)
    {
        if (this.daysData.Length == 0)
        {
            HideTableAndSetErrorMessage("No days selected to display");
            return;
        }
        if (this.NumColumns == 0)
        {
            HideTableAndSetErrorMessage("No providers allocated for selected days and organisation/s");
            return;
        }


        UserView userView = UserView.GetInstance();
        int loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);


        StartEndTime startEndTime = GetStartEndTimes();
        bookingModalElementControl.SetModalDropDownList(startEndTime);

        int minsPerRow = bookingSlotMinsCache.GetSlotMins();
        bool serviceAndPatientSet = ddlServices.SelectedValue != "-1" && txtPatientID.Text.Length > 0;

        int slotMins = bookingSlotMinsCache.GetSlotMins();
        int nRows = (int)startEndTime.EndTime.Subtract(startEndTime.StartTime).TotalMinutes / slotMins;
        for (int r = 0; r < nRows; r++)
        {
            HtmlTableRow htr = new HtmlTableRow();
            main_table.Controls.Add(htr);
            if (bgColor != null)
                htr.BgColor = bgColor;

            int totalDaysShowing = 0;
            for (int i = 0; i < this.daysData.Length; i++)
                if (this.daysData[i].NCols > 0)
                    totalDaysShowing++;

            int rowMinHeight = -1; // 43;
            int timeCol = 109;
            int scrollBarWidth = (int)main_panel.Width.Value - Convert.ToInt32(header_table.Style["width"].Substring(0, header_table.Style["width"].Length - 2));

            TimeSpan rowTime = startEndTime.StartTime.Add(new TimeSpan(0, r * slotMins, 0));

            bool addedTimeCol = false;
            for (int i = 0; i < this.daysData.Length; i++)
            {
                // dont display anything for days with no cols
                if (this.daysData[i].NCols == 0)
                    continue;

                htr.VAlign = "top";
                htr.Align = "left";


                //if (i == 0 || mutlipleTimesShowing)
                if (!addedTimeCol || mutlipleTimesShowing)
                {
                    // add day seperator
                    //if (i != 0)
                    //    main_table.Rows[r].Controls.Add(NewBlankCell(false, this.DaySeperatorColumnWidth, false, System.Drawing.Color.DarkGray));

                    HtmlTableCell timeCell = NewCell(null, false, GetRowTime(startEndTime.StartTime, r, slotMins), System.Drawing.Color.Empty, timeCol, rowMinHeight, "center");
                    timeCell.Attributes.Add("class", BookingSlot.GetContextMenuClass(BookingSlot.Type.None));
                    main_table.Rows[r].Controls.Add(timeCell);
                    addedTimeCol = true;
                }

                //main_table.Rows[r].Controls.Add(NewBlankCell(false, this.DaySeperatorColumnWidth, false, System.Drawing.Color.DarkGray));



                DayData curDayData = this.daysData[i];
                DateTime curDate = curDayData.Date;
                bool seenFirst = false;
                for (int j = 0; j < curDayData.OrgDayData.Length; j++)
                {

                    // dont display anything for orgs with no cols
                    if (this.daysData[i].OrgDayData[j].NCols == 0)
                        continue;

                    // add org seperator
                    //if (j != 0 && seenFirst)
                    //    main_table.Rows[r].Controls.Add(NewBlankCell(false, this.DaySeperatorColumnWidth, false, System.Drawing.Color.DarkGray));

                    seenFirst = true;

                    OrgDayData curOrgDayData = curDayData.OrgDayData[j];
                    Organisation curDayOrg = curOrgDayData.Org;
                    for (int k = 0; k < curOrgDayData.StaffList.Length; k++)
                    {
                        Staff curDayStaff = curOrgDayData.StaffList[k];

                        // if there are no staff working ... still show a column with rows they can't right click to open a menu
                        if (curDayStaff == null)
                        {
                            string blankCellID = curDayOrg.OrganisationID + "_" + "0" + "_" + curDate.Date.ToString("yyyy_MM_dd") + "_" + rowTime.Hours.ToString().PadLeft(2, '0') + rowTime.Minutes.ToString().PadLeft(2, '0');
                            HtmlTableCell blankCell = NewCell("td_" + blankCellID, false, "", System.Drawing.Color.Empty, r == 0 ? this.columnWidth : -1, -1);
                            blankCell.Attributes.Add("class", BookingSlot.GetContextMenuClass(BookingSlot.Type.None));
                            System.Drawing.Color c = (IsEditBookingMode() || IsEditDayMode()) ? HSLColor.ChangeBrightness(BookingSlot.GetColor(BookingSlot.Type.Unavailable), -10) : BookingSlot.GetColor(BookingSlot.Type.Unavailable);
                            blankCell.BgColor = System.Drawing.ColorTranslator.ToHtml(Utilities.IsDev() ? HSLColor.ChangeBrightness(c, 40) : c);
                            main_table.Rows[r].Controls.Add(blankCell);
                            continue;
                        }


                        bool isProviderViewButOtherProvider = userView.IsProviderView && curDayStaff.StaffID != loggedInStaffID;

                        BookingSlot.Type slotType = (startEndTime == StartEndTime.NullStartEndTime || rowTime < startEndTime.StartTime || rowTime >= startEndTime.EndTime) ? (userView.IsPatient ? BookingSlot.Type.Unavailable : BookingSlot.Type.UnavailableButAddable) : (userView.IsPatient ? BookingSlot.Type.Available_PatientLoggedIn : BookingSlot.Type.Available);

                        //if (Request.QueryString["blah"] != null)
                        //    Logger.LogQuery("A1:" + (startEndTime == StartEndTime.NullStartEndTime));
                        //if (Request.QueryString["blah"] != null)
                        //    Logger.LogQuery("A2:" + (rowTime < startEndTime.StartTime));
                        //if (Request.QueryString["blah"] != null)
                        //    Logger.LogQuery("A3:" + (rowTime >= startEndTime.EndTime));
                        //if (Request.QueryString["blah"] != null)
                        //    Logger.LogQuery("B:" + slotType.ToString());

                        StartEndTime thisOrgStartEndTime = curDayOrg.GetStartEndTime(curDate.DayOfWeek);
                        StartEndTime thisOrgLunchStartEndTime = curDayOrg.GetStartEndLunchTime(curDate.DayOfWeek);
                        StartEndTime slotStartEndTime = new StartEndTime(rowTime, rowTime.Add(new TimeSpan(0, this.bookingSlotMinsCache.GetSlotMins(), 0)));
                        if ((thisOrgStartEndTime.StartTime != TimeSpan.Zero || thisOrgStartEndTime.EndTime != TimeSpan.Zero) && (thisOrgStartEndTime.StartTime > slotStartEndTime.StartTime || thisOrgStartEndTime.EndTime < slotStartEndTime.EndTime))
                        {
                            slotType = userView.IsPatient ? BookingSlot.Type.Unavailable : BookingSlot.Type.UnavailableButAddable;
                            //if (Request.QueryString["blah"] != null)
                            //    Logger.LogQuery("C");
                        }
                        if (thisOrgLunchStartEndTime.EndTime > thisOrgLunchStartEndTime.StartTime && (slotStartEndTime.StartTime < thisOrgLunchStartEndTime.EndTime && slotStartEndTime.EndTime > thisOrgLunchStartEndTime.StartTime))
                        {
                            slotType = userView.IsPatient ? BookingSlot.Type.Unavailable : BookingSlot.Type.UnavailableButAddable;
                            //if (Request.QueryString["blah"] != null)
                            //    Logger.LogQuery("D");
                        }
                        //if (Request.QueryString["blah"] != null)
                        //    Logger.LogQuery("E:" + slotType.ToString());

                        if (IsEditBookingMode() && (slotType == BookingSlot.Type.Available || slotType == BookingSlot.Type.Available_PatientLoggedIn))
                            slotType = BookingSlot.Type.Updatable;
                        if (IsEditDayMode() && (slotType == BookingSlot.Type.Available || slotType == BookingSlot.Type.Available_PatientLoggedIn))
                            slotType = BookingSlot.Type.Updatable;  // use same color as editing bookings so screen colours are same when "moving" bookings, but remove context menu for full day move
                        if (IsEditBookingMode() && (slotType == BookingSlot.Type.UnavailableButAddable))
                            slotType = BookingSlot.Type.UnavailableButUpdatable;
                        if (IsEditDayMode() && slotType == BookingSlot.Type.UnavailableButAddable)
                            slotType = BookingSlot.Type.UnavailableButUpdatable;

                        //if (Request.QueryString["blah"] != null)
                        //    Logger.LogQuery("F:" + slotType.ToString());
                        string cellID = curDayOrg.OrganisationID + "_" + curDayStaff.StaffID + "_" + curDate.Date.ToString("yyyy_MM_dd") + "_" + rowTime.Hours.ToString().PadLeft(2, '0') + rowTime.Minutes.ToString().PadLeft(2, '0');
                        HtmlTableCell cell = NewCell("td_" + cellID, false, "", System.Drawing.Color.Empty, r == 0 ? this.columnWidth : -1, -1); // only set width for first row, so html has less text to send
                        Label newLbl = NewLbl(-1, 31, slotType, "lbl_" + cellID);

                        if (!IsEditDayMode() && !IsEditBookingMode())
                            cell.Attributes["ondblclick"]  = "dblclick(this.id); return false;";
                        //cell.Attributes["onmousedown"] = "bk_mouse_down(this.id); return false;";
                        //cell.Attributes["onmouseup"]   = "bk_mouse_up(this.id); return false;";
                        cell.BgColor = System.Drawing.ColorTranslator.ToHtml(Utilities.IsDev() ? HSLColor.ChangeBrightness(newLbl.BackColor, 40) : newLbl.BackColor);

                        if (rowTime == new TimeSpan(8, 30, 0))
                        {
                            string st = curDayStaff.Person.FullnameWithoutMiddlename;
                            ;
                        }

                        if (IsEditDayMode())
                            cell.Attributes.Add("class", BookingSlot.GetContextMenuClass(BookingSlot.Type.None));
                        //else if (!userView.IsStakeholder && !isMasterAdmin && !isAdmin && !isPrincipal && loggedInStaffID != curDayStaff.StaffID)  // if not admin, only show slot menu for logged in provider
                        //{
                        //    ((HtmlTableCell)cell).Attributes.Remove("class");
                        //    ((HtmlTableCell)cell).Attributes["class"] = BookingSlot.GetContextMenuClass(BookingSlot.Type.None);
                        //}
                        else // if (serviceAndPatientSet)
                        {
                            //if (isProviderViewButOtherProvider)
                            //    cell.Attributes.Add("class", BookingSlot.GetContextMenuClass(BookingSlot.Type.None));
                            //else
                                cell.Attributes.Add("class", BookingSlot.GetContextMenuClass((!userView.IsPatient || slotType != BookingSlot.Type.Unavailable) ? slotType : BookingSlot.Type.None));
                        }
                        /*
                        else
                        {
                            BookingSlot.Type type = BookingSlot.Type.PatientAndServiceNotSet;
                            if (ddlServices.SelectedValue == "-1" && txtPatientID.Text.Length > 0)
                                type = BookingSlot.Type.ServiceNotSet;
                            if (ddlServices.SelectedValue != "-1" && txtPatientID.Text.Length == 0)
                                type = BookingSlot.Type.PatientNotSet;

                            cell.Attributes.Remove("class");
                            cell.Attributes.Add("class", BookingSlot.GetContextMenuClass(type));
                        }
                        */

                        main_table.Rows[r].Controls.Add(cell);
                        cell.Controls.Add(newLbl);
                    }

                }

            }

        }
    }
    protected HtmlTableCell NewCell(string id, bool isHeader, string text, System.Drawing.Color bgColor, int width = -1, int height = -1, string align = null, string valign = null, int rowSpan = 1)
    {
        HtmlTableCell cell = isHeader ? new HtmlTableCell("th") : new HtmlTableCell();
        if (id != null)
            cell.ID = id;
        if (align != null)
            cell.Align = align;
        if (valign != null)
            cell.VAlign = valign;
        if (width > 0)
        {
            cell.Style["width"] = width.ToString() + "px";
            //cell.Width = width.ToString();
        }
        if (height > 0)
            cell.Height = height.ToString();
        if (bgColor != System.Drawing.Color.Empty)
            cell.BgColor = System.Drawing.ColorTranslator.ToHtml(bgColor);
        cell.InnerHtml = text;
        if (rowSpan != 1)
            cell.RowSpan = rowSpan;
        return cell;
    }
    protected HtmlTableCell NewBlankCell(bool isHeader, int width, bool hasBorder, System.Drawing.Color bgColor, int rowSpan = 1)
    {
        HtmlTableCell cell = isHeader ? new HtmlTableCell("th") : new HtmlTableCell();
        if (width > 0)
        {
            //cell.Width = width.ToString();
            cell.Style["width"] = width.ToString() + "px";
        }
        if (bgColor != System.Drawing.Color.Empty)
            cell.BgColor = System.Drawing.ColorTranslator.ToHtml(bgColor);
        if (!hasBorder)
            cell.Style["border-style"] = "hidden;";
        cell.InnerText = "";
        if (rowSpan != 1)
            cell.RowSpan = rowSpan;
        return cell;
    }
    protected string GetRowTime(TimeSpan startTime, int r, int slotMins)
    {
        TimeSpan start = startTime.Add(new TimeSpan(0, r * slotMins, 0));
        TimeSpan end = start.Add(new TimeSpan(0, slotMins, 0));

        string amORpmSuffix = start.Hours < 12 ? "am" : "pm";
        if (start.Hours > 12) start = start.Subtract(new TimeSpan(12, 0, 0));
        if (end.Hours > 12) end = end.Subtract(new TimeSpan(12, 0, 0));
        return start.Hours + ":" + start.Minutes.ToString().PadLeft(2, '0') + "-" + end.Hours + ":" + end.Minutes.ToString().PadLeft(2, '0') + " " + amORpmSuffix;
    }
    protected Label NewLbl(int width, int height, BookingSlot.Type slotType, string lblID = null, bool isNonAddable = false)
    {
        Label lbl = new Label();

        if (width > 0)
            lbl.Width = new Unit(width);
        lbl.Height = new Unit(100, UnitType.Percentage); ;// new Unit(height);

        if (!isNonAddable)
            lbl.BackColor = (IsEditBookingMode() || IsEditDayMode()) ? HSLColor.ChangeBrightness(BookingSlot.GetColor(slotType), -10) : BookingSlot.GetColor(slotType);
        else
            lbl.BackColor = HSLColor.ChangeBrightness(System.Drawing.Color.Tan, 20);

        if (lbl != null)
        {
            lbl.ID = lblID;

            if (Utilities.IsDev())
                lbl.Text = "<small>" + lblID.Replace("lbl_", "").Replace("_", " ") + "</small>";
        }

        lbl.Attributes.Add("style", "padding:1px;");
        return lbl;
    }

    protected StartEndTime GetStartEndTimes()
    {
        bool containsPodiatry = false;
        bool containsAgedCare = false;
        foreach (Organisation o in this.orgs)
        {
            if (o.OrganisationType.OrganisationTypeID == 218)
                containsPodiatry = true;
            else
                containsAgedCare = true;
        }

        Site[] sites = SiteDB.GetAll();
        Hashtable siteHash = new Hashtable();
        foreach (Site s in sites)
        {
            siteHash[(s.SiteType.ID == 1)] = s;
        }



        if (!containsPodiatry && containsAgedCare)
        {
            //return new StartEndTime(((Site)siteHash[1]).DayStartTime, ((Site)siteHash[1]).DayEndTime);
            return new StartEndTime(((Site)siteHash[false]).DayStartTime, ((Site)siteHash[false]).DayEndTime);
        }
        else if (containsPodiatry && !containsAgedCare)
        {
            //return new StartEndTime(((Site)siteHash[2]).DayStartTime, ((Site)siteHash[2]).DayEndTime);
            return new StartEndTime(((Site)siteHash[true]).DayStartTime, ((Site)siteHash[true]).DayEndTime);
        }
        else if (containsPodiatry && containsAgedCare)
        {
            TimeSpan startTime = new TimeSpan(12, 0, 0);
            TimeSpan endTime = new TimeSpan(12, 10, 0);

            for (int i = 0; i < sites.Length; i++)
            {
                StartEndTime startEndTime = new StartEndTime(sites[i].DayStartTime, sites[i].DayEndTime);
                if (startEndTime.StartTime < startTime)
                    startTime = startEndTime.StartTime;
                if (startEndTime.EndTime > endTime)
                    endTime = startEndTime.EndTime;
            }
            return new StartEndTime(startTime, endTime);
        }
        else
            return new StartEndTime(new TimeSpan(8, 0, 0), new TimeSpan(20, 0, 0));
    }

    #endregion


    #region ResetPatientName, btnPatientUpdated_Click, btnUpdateFlashingTextIcon_Click

    protected void ResetPatientName()
    {
        if (txtPatientID.Text.Length == 0)
        {
            txtPatientID.Text = string.Empty;
            SetFlashingText(-1, string.Empty, string.Empty);
            ShowPatientEPCInfo(false);

            tr_selected_patient_row.Visible = false;
        }
        else
        {
            Patient patient = PatientDB.GetByID(Convert.ToInt32(txtPatientID.Text));
            if (patient.FlashingTextAddedBy != null)
                patient.FlashingTextAddedBy = StaffDB.GetByID(patient.FlashingTextAddedBy.StaffID);

            txtUpdatePatientName.Text = patient.Person.FullnameWithoutMiddlename;
            string addedByHoverLink = patient.FlashingTextAddedBy == null ? "" : @"<a href=""javascript:void(0)"" onclick=""javascript:return false;"" title='Added By: " + patient.FlashingTextAddedBy.Person.FullnameWithoutMiddlename + @"'>*</a>";
            string date        = (patient.FlashingText.Length == 0 || patient.FlashingTextLastModifiedDate == DateTime.MinValue ? "" : @"[" + patient.FlashingTextLastModifiedDate.ToString("d'/'M'/'yy") + " " + addedByHoverLink + "]&nbsp;&nbsp;");
            string fashingText = patient.FlashingText;
            SetFlashingText(patient.PatientID, date, fashingText);
            SetPatientEPCInfo(patient);
            tr_selected_patient_row.Visible = true;
            string href = "PatientDetailV2.aspx?type=view&id=" + patient.PatientID;
            lblSelectedPatientName.Text = "<a href=\"" + href + "\" target=\"_blank\">" + patient.Person.FullnameWithoutMiddlename + "</a>";


            // update this in the other place also
            Invoice[] outstandingInvoices = InvoiceDB.GetOutstanding(patient.PatientID, Convert.ToInt32(Session["SiteID"]), false);
            lblInvoicesOwingMessage.Text = (outstandingInvoices.Length == 0) ? "" : "<font color=\"red\"><b>Outstanding Invoices:  $" + outstandingInvoices.Sum(item => item.TotalDue) + "</b></font>&nbsp;&nbsp;";
            br_invoices_owing_message_trailing_space.Visible = outstandingInvoices.Length > 0;
        }

        UpdateTextbox(txtUpdatePatientName, txtPatientID.Text.Length == 0);
    }

    protected void UpdateTextbox(TextBox textbox, bool isEmpty)
    {
        if (!isEmpty)
            textbox.Attributes["style"] = "font-weight:bold;border:none;background-color:transparent;";
        else
            textbox.Attributes["style"] = ""; // "width:230px;";
    }

    protected void btnUpdateFlashingTextIcon_Click(object sender, EventArgs e)
    {
        ResetPatientName();
    }

    protected void SetFlashingText(int patientID, string date, string flashingText)
    {
        flashingText = date + string.Join("<br />", SplitIntoLines(flashingText, 50));

        string allFeaturesFlashingText = "dialogWidth:525px;dialogHeight:255px;center:yes;resizable:no; scroll:no";
        string jsFlashingText = Utilities.IsMobileDevice(Request) ?
            "javascript: document.getElementById('lblFlashingText').style.display = ''; open_new_tab('" + "PatientFlashingMessageDetailV2.aspx?type=edit&id=" + patientID.ToString() + "');return false;"
            :
            "javascript: document.getElementById('lblFlashingText').style.display = ''; window.showModalDialog('" + "PatientFlashingMessageDetailV2.aspx?type=edit&id=" + patientID.ToString() + "', '', '" + allFeaturesFlashingText + "');return false;";
        this.lnkFlashingText.Attributes.Add("onclick", jsFlashingText);
        this.lnkFlashingText.Visible = patientID != -1;
        this.lnkFlashingText.ImageUrl = "~/images/asterisk_12.png";
        this.lblFlashingText.Text = flashingText;
        //this.lblFlashingText.Attributes.Add("onclick", jsFlashingText);
    }

    protected string[] SplitIntoLines(string text, int maxLength)
    {
        if (text.Length < maxLength)
            return new string[] { text };

        int offset = 0;

        if (text.Length > 0 && text[text.Length - 1] != ' ')
            text = text + " ";

        ArrayList lines = new ArrayList();
        while (offset < text.Length)
        {
            int index = text.LastIndexOf(" ", Math.Min(text.Length, offset + maxLength));
            string line = text.Substring(offset, (index - offset <= 0 ? text.Length : index) - offset);
            offset += line.Length + 1;
            lines.Add(line);
        }

        return (string[])lines.ToArray(typeof(string));
    }

    #endregion

    #region btnPrintInvoice_Click, btnEmailInvoice_Click

    protected void btnPrintInvoice_Click(object sender, EventArgs e)
    {
        try
        {
            int bookingID = Convert.ToInt32(printOrEmailInvoiceBookingID.Value);
            printOrEmailInvoiceBookingID.Value = "-1";

            Invoice[] invoices = InvoiceDB.GetByBookingID(Convert.ToInt32(bookingID));
            int[] invoiceIDs = new int[invoices.Length];
            for (int i = 0; i < invoices.Length; i++)
                invoiceIDs[i] = invoices[i].InvoiceID;

            if (invoices.Length == 0)
                throw new CustomMessageException("No Invoices For Booking: " + bookingID);

            Letter.GenerateInvoicesToPrint(invoiceIDs, Response, invoices[0].Site.SiteType.ID == 1);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch (Exception ex)
        {
            SetErrorMessage(ex.ToString());
        }
    }
    protected void btnEmailInvoice_Click(object sender, EventArgs e)
    {
        try
        {
            int bookingID = Convert.ToInt32(printOrEmailInvoiceBookingID.Value);
            printOrEmailInvoiceBookingID.Value = "-1";

            Invoice[] invoices = InvoiceDB.GetByBookingID(bookingID);
            if (invoices.Length == 0)
                throw new CustomMessageException("No Invoices For Booking: " + bookingID);


            //
            // group invoices by entity to send to 
            //
            Hashtable entityHash = new Hashtable();   // emailHash[entityID] = ArrayLst of invoiceIDs
            for (int i = 0; i < invoices.Length; i++)
            {
                int entityID = -1;
                if (invoices[i].PayerPatient != null)
                    entityID = invoices[i].PayerPatient.Person.EntityID;
                else if (invoices[i].PayerOrganisation != null)
                {
                    if (invoices[i].PayerOrganisation.OrganisationID != -1 && invoices[i].PayerOrganisation.OrganisationID != -2)
                        entityID = invoices[i].PayerOrganisation.EntityID;
                }
                else if (invoices[i].Booking != null && invoices[i].Booking.Patient != null) // clinic booking, so use patient from booking
                    entityID = invoices[i].Booking.Patient.Person.EntityID;

                if (entityID != -1)
                {
                    if (entityHash[entityID] == null)
                        entityHash[entityID] = new ArrayList();
                    ((ArrayList)entityHash[entityID]).Add(invoices[i].InvoiceID);
                }
            }


            //
            // get entity ID's so we can get all emails into a hashtable in one db query
            //
            int[] entityIDs = new int[entityHash.Keys.Count];
            entityHash.Keys.CopyTo(entityIDs, 0);
            Hashtable emailHash = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);


            //
            // send them off in batches by entity
            //
            int countSent = 0;
            foreach (int entityID in entityHash.Keys)
            {
                string email = GetEmail(emailHash, entityID);
                if (email == null || email.Length == 0)
                    continue;

                ArrayList invoiceIDsArrayList = (ArrayList)entityHash[entityID];
                int[] invoiceIDs = (int[])invoiceIDsArrayList.ToArray(typeof(int));
                Letter.GenerateInvoicesToEmail(invoiceIDs, email, invoices[0].Site.SiteType.ID == 1);
                countSent++;
            }


            for (int i = 0; i < invoices.Length; i++)
                InvoiceDB.UpdateLastDateEmailed(invoices[i].InvoiceID, DateTime.Now);

            if (countSent == 0)
                SetErrorMessage("No invoices sent. Non-Medicare/DVA debtor(s) have no email set.");
            else if (countSent == 1)
                SetErrorMessage("Invoice Emailed");
            else
                SetErrorMessage("Invoices Emailed");
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch (Exception ex)
        {
            SetErrorMessage(ex.ToString());
        }
    }

    protected static string GetEmail(Hashtable contactHash, int entityID)
    {
        return ContactDB.GetEmailsCommaSepByEntityID(contactHash, entityID, true, false);
    }

    #endregion

    #region GetUrlParams

    protected bool IsEditBookingMode()
    {
        return Request.QueryString["edit_booking_id"] != null;
    }
    protected bool IsEditDayMode()
    {
        return Request.QueryString["edit_date"] != null && Request.QueryString["edit_org"] != null && Request.QueryString["edit_provider"] != null;
    }

    protected bool IsValidFormBooking()
    {
        string id = Request.QueryString["edit_booking_id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    protected bool IsValidFormPatient()
    {
        string patient_id = Request.QueryString["patient"];
        return patient_id != null && Regex.IsMatch(patient_id, @"^\d+$");
    }
    protected bool IsValidSessionPatient()
    {
        string patient_id = Session == null || Session["PatientID"] == null ? null : Session["PatientID"].ToString();
        return patient_id != null && Regex.IsMatch(patient_id, @"^\d+$");
    }
    protected bool IsValidFormProvider()
    {
        string provider_id = Request.QueryString["provider"];
        return provider_id != null && Regex.IsMatch(provider_id, @"^\d+$");
    }
    protected bool IsValidFormClinic()
    {
        string clinic_id = Request.QueryString["clinic"];
        return clinic_id != null && Regex.IsMatch(clinic_id, @"^\d+$");
    }
    protected bool IsValidFormAgedCare()
    {
        string agedcare_id = Request.QueryString["agedcare"];
        return agedcare_id != null && Regex.IsMatch(agedcare_id, @"^\d+$");
    }
    protected bool IsValidFormOrgs()
    {
        string org_ids = Request.QueryString["orgs"];
        return org_ids != null && Regex.IsMatch(org_ids, @"^[\d_]+$") && OrganisationDB.Exists(org_ids.Replace('_', ','));
    }
    protected bool IsValidFormStaff()
    {
        string staff_id = Request.QueryString["staff"];
        return (staff_id != null && Regex.IsMatch(staff_id, @"^\d+$") && StaffDB.GetByID(Convert.ToInt32(staff_id)) != null);
    }
    protected bool IsValidFormField()
    {
        string field_id = Request.QueryString["field"];
        return field_id != null && Regex.IsMatch(field_id, @"^\d+$");
    }
    protected bool IsValidFormOffering()
    {
        string offering_id = Request.QueryString["offering"];
        return offering_id != null && Regex.IsMatch(offering_id, @"^\d+$") && OfferingDB.Exists(Convert.ToInt32(offering_id));
    }

    protected DateTime GetFormEditDay_Date()
    {
        string dateString = Request.QueryString["edit_date"];
        if (dateString == null)
            throw new InvalidExpressionException("No url parameter edit_date");

        string[] parts = dateString.Split('_');
        if (parts.Length != 3)
            throw new InvalidExpressionException("edit_date does not contain 3 parts seeperated by underscore :" + dateString);
        return new DateTime(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]));
    }
    protected int GetFormEditDay_Org()
    {
        return Convert.ToInt32(Request.QueryString["edit_org"]);
    }
    protected int GetFormEditDay_Provider()
    {
        return Convert.ToInt32(Request.QueryString["edit_provider"]);
    }


    protected int GetFormBooking()
    {
        if (!IsValidFormBooking())
            throw new Exception("Invalid url booking");
        return Convert.ToInt32(Request.QueryString["edit_booking_id"]);
    }
    protected int GetFormPatient()
    {
        if (!IsValidFormPatient())
            throw new Exception("Invalid url patient");
        return Convert.ToInt32(Request.QueryString["patient"]);
    }
    protected int GetSessionPatient()
    {
        if (!IsValidSessionPatient())
            throw new Exception("Invalid session patient");
        return Convert.ToInt32(Session["PatientID"]);
    }
    protected int GetFormProvider()
    {
        if (!IsValidFormProvider())
            throw new Exception("Invalid url provider");
        return Convert.ToInt32(Request.QueryString["provider"]);
    }
    protected int GetFormClinic()
    {
        if (!IsValidFormClinic())
            throw new Exception("Invalid url clinic");
        return Convert.ToInt32(Request.QueryString["clinic"]);
    }
    protected int GetFormAgedCare()
    {
        if (!IsValidFormAgedCare())
            throw new Exception("Invalid url agedcare");
        return Convert.ToInt32(Request.QueryString["agedcare"]);
    }
    protected int[] GetFormOrgs()
    {
        if (!IsValidFormOrgs())
            throw new Exception("Invalid url orgs");

        string[] strList = Request.QueryString["orgs"].Split('_');
        int[] ret = new int[strList.Length];
        for (int i = 0; i < strList.Length; i++)
            ret[i] = Convert.ToInt32(strList[i]);

        return ret;
    }
    protected int GetFormStaff()
    {
        if (!IsValidFormStaff())
            throw new Exception("Invalid url staff");
        return Convert.ToInt32(Request.QueryString["staff"]);
    }
    protected int GetFormField()
    {
        if (!IsValidFormField())
            throw new Exception("Invalid url field");
        return Convert.ToInt32(Request.QueryString["field"]);
    }
    protected int GetFormOffering()
    {
        if (!IsValidFormOffering())
            throw new Exception("Invalid url offering");
        return Convert.ToInt32(Request.QueryString["offering"]);
    }

    protected DateTime GetFormDate()
    {
        try
        {
            string dateString = Request.QueryString["date"];
            if (dateString == null)
                throw new InvalidExpressionException("No url parameter date");

            string[] parts = dateString.Split('_');
            if (parts.Length != 3)
                throw new InvalidExpressionException("Does not contain 3 parts seeperated by underscore :" + dateString);
            return new DateTime(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]));
        }
        catch (Exception)
        {
            return DateTime.Today;
        }
    }
    protected int GetFormNumDays()
    {
        try
        {
            string numdays = Request.QueryString["ndays"];
            if (numdays == null)
                return Convert.ToInt32(Session["NumDaysToDisplayOnBookingScreen"]);

            int ndays = Convert.ToInt32(numdays);
            if (ndays > 8)
                return 8;
            else if (ndays <= 0)
                return 1;
            else
                return ndays;
        }
        catch (Exception)
        {
            return Convert.ToInt32(Session["NumDaysToDisplayOnBookingScreen"]);
        }
    }
    protected bool GetFormIsCollapsed()
    {
        string is_collapsed = Request.QueryString["is_collapsed"];
        return is_collapsed != null && is_collapsed == "1";
    }
    protected bool GetFormShowUnavailableStaff()
    {
        string show_unavailable_staff = Request.QueryString["show_unavailable_staff"];
        return show_unavailable_staff != null && show_unavailable_staff == "1";
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        links_header_section.Visible = false;
        header_section.Visible = false;
        header_table.Controls.Clear();
        main_table.Controls.Clear();
        SetErrorMessage(errMsg, details);
    }
    private void SetErrorMessage(string errMsg = "", string details = "")
    {
        if (errMsg.Contains(Environment.NewLine))
            errMsg = errMsg.Replace(Environment.NewLine, "<br />");

        // double escape so shows up literally on webpage for 'alert' message
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"#\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

        lblErrorMessage.Visible = true;
        if (errMsg != null && errMsg.Length > 0)
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}