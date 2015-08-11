using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;

public partial class Report_BookingsV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, true, false);
                Session.Remove("sortExpression_summaryReport");
                Session.Remove("data_summaryReport");
                SetupGUI();
                FillGrid();
            }

            this.GrdSummaryReport.EnableViewState = true;

        }
        catch (CustomMessageException ex)
        {
            if (IsPostBack) SetErrorMessage(ex.Message);
            else HideTableAndSetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            if (IsPostBack) SetErrorMessage("", ex.ToString());
            else HideTableAndSetErrorMessage("", ex.ToString());
        }
    }

    protected void SetupGUI()
    {
        UserView userView = UserView.GetInstance();

        if (!IsValidFormUseTreatmentDate() || GetFormUseTreatmentDate(false) == true)
            dateTypeToUse.SelectedValue = "booking_date_start";
        else
            dateTypeToUse.SelectedValue = "booking_date_created";

        chkIncCompleted.Checked       = IsValidFormIncCompleted()       ? GetFormIncCompleted(false)          : true;
        chkIncCancelled.Checked       = IsValidFormIncCancelled()       ? GetFormIncCancelled(false)          : true;
        chkIncIncomplete.Checked      = IsValidFormIncIncomplete()      ? GetFormIncIncomplete(false)         : true;
        chkIncDeleted.Checked         = IsValidFormIncDeleted()         ? GetFormIncDeleted(false)            : true;
        chkIncEPC.Checked             = IsValidFormIncEPC()             ? GetFormIncEPC(false)                : true;
        chkIncNonEPC.Checked          = IsValidFormIncNonEPC()          ? GetFormIncNonEPC(false)             : true;

        chkIncEPC.Text    = !userView.IsAgedCareView ? "&nbsp;Inc Incomplete EPC"     : "&nbsp;Inc Incomplete Referrals";
        chkIncNonEPC.Text = !userView.IsAgedCareView ? "&nbsp;Inc Incomplete Non-EPC" : "&nbsp;Inc Incomplete Non-Referrals";

        chkOnlyPtSelfBookings.Checked = IsValidFormOnlyPtSelfBookings() ? GetFormIncOnlyPtSelfBookings(false) : false;


        ddlOrgs.Style["width"] = "300px";
        ddlOrgs.Items.Clear();
        ddlOrgs.Items.Add(new ListItem("All " + (userView.IsAgedCareView ? "Facilities" : "Clinics"), (-1).ToString()));
        foreach (Organisation curOrg in OrganisationDB.GetAll(false, true, !userView.IsClinicView && !userView.IsGPView, !userView.IsAgedCareView, true, true))
            ddlOrgs.Items.Add(new ListItem(curOrg.Name, curOrg.OrganisationID.ToString()));

        ddlProviders.Style["width"] = "300px";
        ddlProviders.Items.Clear();
        ddlProviders.Items.Add(new ListItem("All Providers", (-1).ToString()));
        foreach (Staff curProv in StaffDB.GetAll())
            if (curProv.IsProvider)
                ddlProviders.Items.Add(new ListItem(curProv.Person.FullnameWithoutMiddlename, curProv.StaffID.ToString()));


        if (IsValidFormOrgID())
        {
            Organisation org = OrganisationDB.GetByID(GetFormOrgID());
            if (org != null)
                ddlOrgs.SelectedValue = org.OrganisationID.ToString();
        }

        if (!UserView.GetInstance().IsAdminView)
        {
            providerRow.Visible = false;

            Staff provider = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));
            if (provider != null)
                ddlProviders.SelectedValue = provider.StaffID.ToString();
        }
        else
        {
            if (IsValidFormProviderID())
            {
                Staff provider = StaffDB.GetByID(GetFormProviderID());
                if (provider != null)
                    ddlProviders.SelectedValue = provider.StaffID.ToString();
            }
        }

        txtStartDate.Text = IsValidFormStartDate() ? (GetFormStartDate(false) == DateTime.MinValue ? "" : GetFormStartDate(false).ToString("dd-MM-yyyy")) : DateTime.Today.ToString("dd-MM-yyyy");
        txtEndDate.Text = IsValidFormEndDate() ? (GetFormEndDate(false) == DateTime.MinValue ? "" : GetFormEndDate(false).ToString("dd-MM-yyyy")) : DateTime.Today.ToString("dd-MM-yyyy");

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";
    }


    #region GrdSummaryReport


    protected Hashtable GetPatientHealthCardCache(int[] patientIDs)
    {
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

    protected Hashtable GetPatientsPhoneNumbersCache(int[] entityIDs)
    {
        return PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, Convert.ToInt32(Session["SiteID"]));
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


    protected Hashtable GetPatientsMedicareCountThisYearCache(int[] patientIDs, int year)
    {
        return PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, year);
    }
    protected Hashtable GetPatientsEPCRemainingCacheDB(int[] patientIDs, DateTime startDate)
    {
        return PatientsEPCRemainingCacheDB.GetBullk(patientIDs, startDate);
    }



    protected void FillGrid()
    {
        UserView userView = UserView.GetInstance();

        DateTime fromDate = IsValidDate(txtStartDate.Text) ? GetDate(txtStartDate.Text) : DateTime.MinValue;
        DateTime toDate   = IsValidDate(txtEndDate.Text) ? GetDate(txtEndDate.Text).Add(new TimeSpan(23, 59, 59)) : DateTime.MinValue;

        ArrayList list = new ArrayList();
        if (chkIncCompleted.Checked) list.Add("187");
        if (chkIncCancelled.Checked) list.Add("188");
        if (chkIncIncomplete.Checked) list.Add("0");
        if (chkIncDeleted.Checked) list.Add("-1");
        string statusIDsToInclude = string.Join(",", (string[])list.ToArray(Type.GetType("System.String")));


        int organisation_type_group_id = -1;
        if (userView.IsClinicView)
            organisation_type_group_id = 5;
        if (userView.IsAgedCareView)
            organisation_type_group_id = 6;


        //int startTime = Environment.TickCount;
        //DataTable dt = BookingDB.GetReport_Bookings(fromDate, toDate, Convert.ToInt32(organisationID.Value), Convert.ToInt32(providerID.Value), organisation_type_group_id, statusIDsToInclude, dateTypeToUse.SelectedValue == "booking_date_start");
        //double time = ((double)(Environment.TickCount - startTime) / 1000.0);
        //Logger.LogQuery("seconds: " + time.ToString(), false, false, true);


        DataTable dt = null;
        if (dateTypeToUse.SelectedValue == "booking_date_start")
        {
            dt = BookingDB.GetReport_Bookings(fromDate, toDate, Convert.ToInt32(ddlOrgs.SelectedValue), Convert.ToInt32(ddlProviders.SelectedValue), organisation_type_group_id, statusIDsToInclude, dateTypeToUse.SelectedValue == "booking_date_start");
        }
        else
        {
            // for some reason it takes like a minute to load when searching by added date vs start date
            // so just get all (8 seconds) and delete the rest here
            dt = BookingDB.GetReport_Bookings(DateTime.MinValue, DateTime.MinValue, Convert.ToInt32(ddlOrgs.SelectedValue), Convert.ToInt32(ddlProviders.SelectedValue), organisation_type_group_id, statusIDsToInclude, dateTypeToUse.SelectedValue == "booking_date_start");
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DateTime addedDate = dt.Rows[i]["booking_date_created"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[i]["booking_date_created"]);
                if (addedDate != DateTime.MinValue && (fromDate == DateTime.MinValue || addedDate < fromDate))
                        dt.Rows.RemoveAt(i);
                else if (addedDate != DateTime.MinValue && (toDate == DateTime.MinValue || addedDate > toDate))
                    dt.Rows.RemoveAt(i);
            }

        }


        // ------------------------------------------------------------------------------------------------------------------------------


        //
        // Get if a booking is medicare/dva/pt-pays
        //


        // 1. caches for less db lookups since booking screen will be used ALOT

        int[] patientIDs = new int[dt.Rows.Count];
        int[] patientEntityIDs = new int[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["booking_patient_id"] != DBNull.Value)
                patientIDs[i] = Convert.ToInt32(dt.Rows[i]["booking_patient_id"]);
            if (dt.Rows[i]["patient_entity_id"] != DBNull.Value)
                patientEntityIDs[i] = Convert.ToInt32(dt.Rows[i]["patient_entity_id"]);
        }

        int MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);
        Hashtable patientHealthCardCache = GetPatientHealthCardCache(patientIDs);
        Hashtable epcRemainingCache = GetEPCRemainingCache(patientHealthCardCache);
        Hashtable patientsPhoneNumbersCache = GetPatientsPhoneNumbersCache(patientEntityIDs);
        int bookingSheetStartYear = fromDate.Year;
        int bookingSheetEndYear = toDate.Year;
        Hashtable patientsMedicareCountThisYearCache = GetPatientsMedicareCountThisYearCache(patientIDs, bookingSheetStartYear);
        Hashtable patientsMedicareCountNextYearCache = (bookingSheetStartYear == bookingSheetEndYear) ? patientsMedicareCountThisYearCache : GetPatientsMedicareCountThisYearCache(patientIDs, bookingSheetEndYear);
        Hashtable patientsEPCRemainingCache = GetPatientsEPCRemainingCacheDB(patientIDs, fromDate == DateTime.MinValue ? fromDate : fromDate.AddYears(-1));

        //Hashtable offeringCache = OfferingDB.GetHashtable();
        Hashtable offeringCache = OfferingDB.GetHashtable(false, -1, null, true);

        // 2. add fields into the data table
        // 3. also remove rows if set to not show EPC or to not show NonEPC

        Hashtable staffHash = StaffDB.GetAllInHashtable(true, true, false, false);

        dt.Columns.Add("booking_is_epc_text", typeof(string));
        dt.Columns.Add("added_by_deleted_by_row", typeof(string));
        for (int i = dt.Rows.Count - 1; i >= 0; i--)
        {
            Booking curBooking = BookingDB.Load(dt.Rows[i], "booking_", false, false);

            if (chkOnlyPtSelfBookings.Checked && curBooking.AddedBy.StaffID != -6)
            {
                dt.Rows.RemoveAt(i);
                continue;
            }

            string addedBy     = curBooking.AddedBy     == null || staffHash[curBooking.AddedBy.StaffID]     == null ? "" : ((Staff)staffHash[curBooking.AddedBy.StaffID]).Person.FullnameWithoutMiddlename;
            string addedDate   = curBooking.DateCreated == DateTime.MinValue                                         ? "" : curBooking.DateCreated.ToString("MMM d, yyyy");
            string deletedBy   = curBooking.DeletedBy   == null || staffHash[curBooking.DeletedBy.StaffID]   == null ? "" : ((Staff)staffHash[curBooking.DeletedBy.StaffID]).Person.FullnameWithoutMiddlename;
            string deletedDate = curBooking.DateDeleted == DateTime.MinValue                                         ? "" : curBooking.DateDeleted.ToString("MMM d, yyyy");
            string cancelledBy = curBooking.CancelledBy == null || staffHash[curBooking.CancelledBy.StaffID] == null ? "" : ((Staff)staffHash[curBooking.CancelledBy.StaffID]).Person.FullnameWithoutMiddlename;
            string cancelledDate = curBooking.DateCancelled == DateTime.MinValue                                     ? "" : curBooking.DateCancelled.ToString("MMM d, yyyy");
            string added_by_deleted_by_row = string.Empty;
            added_by_deleted_by_row += "Added By: " + addedBy + " (" + addedDate + ")";
            if (deletedBy.Length > 0 || deletedDate.Length > 0)
                added_by_deleted_by_row += "\r\nDeleted By: " + deletedBy + " (" + deletedDate + ")";
            if (cancelledBy.Length > 0 || cancelledDate.Length > 0)
                added_by_deleted_by_row += "\r\nCancelled By: " + cancelledBy + " (" + cancelledDate + ")";
            dt.Rows[i]["added_by_deleted_by_row"] = added_by_deleted_by_row;



            if (curBooking.Offering == null)
            {
                dt.Rows[i]["booking_is_epc_text"] = string.Empty;
                continue;
            }

            curBooking.Offering = (Offering)offeringCache[curBooking.Offering.OfferingID];
            curBooking.Provider = (Staff)staffHash[curBooking.Provider.StaffID];

            string bookingTypeText = string.Empty;
            bool isEPCBooking = false;
            string booking_is_epc_text = string.Empty;
            if (curBooking.Patient != null && curBooking.BookingStatus.ID == 0)
            {
                HealthCard hc = GetHealthCardFromCache(patientHealthCardCache, curBooking.Patient.PatientID);
                Hashtable patientsMedicareCountCache = (curBooking.DateStart.Year == bookingSheetStartYear) ? patientsMedicareCountThisYearCache : patientsMedicareCountNextYearCache;
                Booking.InvoiceType invType = curBooking.GetInvoiceType(hc, curBooking.Offering, curBooking.Patient, patientsMedicareCountCache, epcRemainingCache, MedicareMaxNbrServicesPerYear);
                isEPCBooking = invType != Booking.InvoiceType.None && invType != Booking.InvoiceType.NoneFromCombinedYearlyThreshholdReached && invType != Booking.InvoiceType.NoneFromOfferingYearlyThreshholdReached && invType != Booking.InvoiceType.NoneFromExpired;
                if (invType == Booking.InvoiceType.Medicare || invType == Booking.InvoiceType.DVA)
                {
                    string invTypeText = invType == Booking.InvoiceType.Medicare ? "Medicare" : "DVA";
                    int totalServicesAllowedLeft = (MedicareMaxNbrServicesPerYear - (int)patientsMedicareCountCache[curBooking.Patient.PatientID]);
                    Pair totalEPCRemaining = patientsEPCRemainingCache[curBooking.Patient.PatientID] as Pair;

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
                    bookingTypeText = invTypeText + (invType == Booking.InvoiceType.Medicare ? " - " + nServicesLeft + " Left" : "");
                    booking_is_epc_text = invType == Booking.InvoiceType.Medicare ? "Medicare" : "DVA";
                }
                else if (invType == Booking.InvoiceType.Insurance)
                    bookingTypeText = "Insurance";
                else if (invType == Booking.InvoiceType.None)
                    bookingTypeText = "PT Pays";
                else if (invType == Booking.InvoiceType.NoneFromCombinedYearlyThreshholdReached)
                    bookingTypeText = "PT Pays - Year Limit reached";
                else if (invType == Booking.InvoiceType.NoneFromOfferingYearlyThreshholdReached)
                    bookingTypeText = "PT Pays - Year Limit reached for service";
                else if (invType == Booking.InvoiceType.NoneFromExpired)
                    bookingTypeText = !userView.IsAgedCareView ? "PT Pays - EPC Expired" : "PT Pays - Ref. Expired";


                if ((!chkIncEPC.Checked && isEPCBooking) || (!chkIncNonEPC.Checked && !isEPCBooking))
                {
                    dt.Rows.RemoveAt(i);
                    continue;
                }

            }

            dt.Rows[i]["booking_is_epc_text"] = booking_is_epc_text;
        }


        // ------------------------------------------------------------------------------------------------------------------------------


        dt.Columns.Add("total_due_non_medicare_non_dva", typeof(Decimal));
        for (int i = 0; i < dt.Rows.Count; i++)
            dt.Rows[i]["total_due_non_medicare_non_dva"] = (Convert.ToInt32(dt.Rows[i]["invoices_count_non_medicare_non_dva"]) == 0) ?
                (object)DBNull.Value :
                Convert.ToDecimal(dt.Rows[i]["invoices_total_non_medicare_non_dva"]) - Convert.ToDecimal(dt.Rows[i]["total_credit_notes_non_medicare_non_dva"]) - Convert.ToDecimal(dt.Rows[i]["total_receipts_non_medicare_non_dva"]) - Convert.ToDecimal(dt.Rows[i]["total_vouchers"]);


        Hashtable staffOfferingHash = StaffOfferingsDB.Get2DHash(true, Convert.ToInt32(ddlProviders.SelectedValue));
        //dt.Columns.Add("commission_percent_text", typeof(string));
        //dt.Columns.Add("fixed_rate_text", typeof(string));
        //dt.Columns.Add("commission_percent_amount", typeof(decimal));
        //dt.Columns.Add("fixed_rate_amount", typeof(decimal));
        //for (int i = 0; i < dt.Rows.Count; i++)
        //{
        //    int booking_status_id = Convert.ToInt32(dt.Rows[i]["booking_status_id"]); // only show comission info for incomplete bookings [status=0] - as completed bookings have info in the inv lines info (see below)

        //    StaffOfferings staffOffering = (StaffOfferings)staffOfferingHash[new Hashtable2D.Key(Convert.ToInt32(dt.Rows[i]["provider_staff_id"]), dt.Rows[i]["offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(dt.Rows[i]["offering_id"]) )];
        //    dt.Rows[i]["commission_percent_text"]    = booking_status_id != 0 || staffOffering == null || !staffOffering.IsCommission ? "" : Math.Round(staffOffering.CommissionPercent * Convert.ToDecimal(dt.Rows[i]["offering_default_price"]) / 100, 2).ToString() + " (" + staffOffering.CommissionPercent + "%)";
        //    dt.Rows[i]["fixed_rate_text"]            = booking_status_id != 0 || staffOffering == null || !staffOffering.IsFixedRate  ? "" : staffOffering.FixedRate.ToString();
        //    dt.Rows[i]["commission_percent_amount"]  = booking_status_id != 0 || staffOffering == null || !staffOffering.IsCommission ? Convert.ToDecimal(0.00) : Math.Round(staffOffering.CommissionPercent * Convert.ToDecimal(dt.Rows[i]["offering_default_price"]) / 100, 2);
        //    dt.Rows[i]["fixed_rate_amount"]          = booking_status_id != 0 || staffOffering == null || !staffOffering.IsFixedRate  ? Convert.ToDecimal(0.00) : staffOffering.FixedRate;
        //}


        int[] bookingIDs = new int[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
            bookingIDs[i] = Convert.ToInt32(dt.Rows[i]["booking_booking_id"]);
        DataTable tblInvoiceLines = BookingDB.GetReport_Bookings_SubSection_InvoiceLines(bookingIDs);
        Hashtable invLinesHash = new Hashtable();
        for (int i = 0; i < tblInvoiceLines.Rows.Count; i++)
        {
            int bookingID = Convert.ToInt32(tblInvoiceLines.Rows[i]["booking_id"]);
            if (invLinesHash[bookingID] == null)
                invLinesHash[bookingID] = new ArrayList();
            ((ArrayList)invLinesHash[bookingID]).Add(tblInvoiceLines.Rows[i]);
        }



        dt.Columns.Add("invoice_lines_html", typeof(string));
        dt.Columns.Add("invoice_lines_text", typeof(string));
        dt.Columns.Add("invoice_lines_total", typeof(decimal));
        dt.Columns.Add("invoice_lines_count", typeof(int));
        dt.Columns.Add("commission_total", typeof(decimal));
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            decimal invLinesTotal = 0;
            int     invLinesCount = 0;
            decimal invLinesCommissionTotal = 0;

            ArrayList items = new ArrayList();

            /* Do this by getting invoice lines into a hashtable for only the bookings retrieved - using XML within the DB takes a massive amount of time
            string xml = "<parents>" + dt.Rows[i]["invoice_line_list"].ToString() + "</parents>";  // need to add root tags for XmlReader
            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(new System.IO.StringReader(xml)))
            {
                string  invoice_line_id          = null;
                string  quantity                 = null;
                string  offering_id              = null;
                string  offering_name            = null;
                string  price                    = null;
                string  commission_percent_text  = null;
                string  fixed_rate_text          = null;
                decimal commission_percent_amt   = 0;
                decimal fixed_rate_amt           = 0;


                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.Name)
                        {
                            case "invoice_line_id":  // start invoice_line_id element
                                if (reader.Read())
                                    invoice_line_id = reader.Value.Trim();
                                else
                                    invoice_line_id = string.Empty;
                                break;
                            case "quantity":
                                if (reader.Read())
                                {
                                    decimal q = Convert.ToDecimal(reader.Value.Trim());
                                    quantity = (q == Convert.ToDecimal((int)q)) ? ((int)q).ToString() : reader.Value.Trim();
                                }
                                else
                                    quantity = string.Empty;
                                break;
                            case "offering_name":
                                if (reader.Read())
                                    offering_name = reader.Value.Trim();
                                else
                                    offering_name = string.Empty;
                                break;
                            case "price":
                                if (reader.Read())
                                    price = reader.Value.Trim();
                                else
                                    price = string.Empty;
                                break;
                            case "offering_id":  // NOTE  -  This has to be gotten from db AFTER price as it uses price in the calculations
                                if (reader.Read())
                                {
                                    offering_id = reader.Value.Trim();

                                    StaffOfferings staffOffering = (StaffOfferings)staffOfferingHash[new Hashtable2D.Key(Convert.ToInt32(dt.Rows[i]["provider_staff_id"]), Convert.ToInt32(offering_id))];
                                    commission_percent_text = staffOffering == null || !staffOffering.IsCommission ? "" : "(comm " + staffOffering.CommissionPercent + "% = " + Math.Round(staffOffering.CommissionPercent * Convert.ToDecimal(price) / 100, 2).ToString() + ")";
                                    fixed_rate_text         = staffOffering == null || !staffOffering.IsFixedRate  ? "" : "(comm fixed = " + staffOffering.FixedRate.ToString() + ")";

                                    commission_percent_amt  = staffOffering == null || !staffOffering.IsCommission ? 0 : Math.Round(staffOffering.CommissionPercent * Convert.ToDecimal(price) / 100, 2);
                                    fixed_rate_amt          = staffOffering == null || !staffOffering.IsFixedRate  ? 0 : staffOffering.FixedRate;
                                }
                                else
                                    offering_id = string.Empty;

                                Tuple<string, string, string, string, string, string> t = new Tuple<string, string, string, string, string, string>(invoice_line_id, quantity, offering_name, price, commission_percent_text, fixed_rate_text);
                                items.Add(t);
                                invLinesTotal           += Convert.ToDecimal(price);
                                invLinesCommissionTotal += commission_percent_amt + fixed_rate_amt;

                                break;
                        }
                    }
                }
            }
            */


            int bookingID = Convert.ToInt32(dt.Rows[i]["booking_booking_id"]);
            if (invLinesHash[bookingID] != null)
            {
                ArrayList invLineDataRows = (ArrayList)invLinesHash[bookingID];
                for (int j = 0; j < invLineDataRows.Count; j++)
                {
                    DataRow curRow = (DataRow)invLineDataRows[j];
                    string invoice_line_id = Convert.ToInt32(curRow["invoice_line_id"]).ToString();
                    string offering_name = Convert.ToString(curRow["offering_name"]);
                    string quantity = Convert.ToDecimal(curRow["quantity"]).ToString();
                    string price = Convert.ToDecimal(curRow["price"]).ToString();


                    if (curRow["offering_id"] == DBNull.Value)
                    {
                        ;
                        continue;
                    }

                    string offering_id = Convert.ToInt32(curRow["offering_id"]).ToString();


                    StaffOfferings staffOffering = (StaffOfferings)staffOfferingHash[new Hashtable2D.Key(Convert.ToInt32(dt.Rows[i]["provider_staff_id"]), Convert.ToInt32(offering_id))];
                    string commission_percent_text = staffOffering == null || !staffOffering.IsCommission ? "" : "(comm " + staffOffering.CommissionPercent + "% = " + Math.Round(staffOffering.CommissionPercent * Convert.ToDecimal(price) / 100, 2).ToString() + ")";
                    string fixed_rate_text = staffOffering == null || !staffOffering.IsFixedRate ? "" : "(comm fixed = " + staffOffering.FixedRate.ToString() + ")";

                    decimal commission_percent_amt = staffOffering == null || !staffOffering.IsCommission ? 0 : Math.Round(staffOffering.CommissionPercent * Convert.ToDecimal(price) / 100, 2);
                    decimal fixed_rate_amt = staffOffering == null || !staffOffering.IsFixedRate ? 0 : staffOffering.FixedRate;


                    Tuple<string, string, string, string, string, string> t = new Tuple<string, string, string, string, string, string>(invoice_line_id, quantity, offering_name, price, commission_percent_text, fixed_rate_text);
                    items.Add(t);
                    invLinesTotal += Convert.ToDecimal(price);
                    invLinesCount++;
                    invLinesCommissionTotal += commission_percent_amt + fixed_rate_amt;
                }
            }




            string html = string.Empty;
            string text = string.Empty;
            foreach (Tuple<string, string, string, string, string, string> t in items)
            {
                html += "<tr><td class=\"nowrap\" align=\"left\">" + t.Item2 + " x " + t.Item3 + "</td><td>&nbsp;</td><td class=\"nowrap\" align=\"right\">" + " <b>" + t.Item5 + " " + t.Item6 + "</b> = " + t.Item4 + "</td></tr>";
                text += (text.Length > 0 ? Environment.NewLine : "") + t.Item2 + " x " + t.Item3 + " = " + t.Item4 + (t.Item5.Length > 0 ? " " + t.Item5 : "") + (t.Item6.Length > 0 ? " " + t.Item6 : "");
            }
            dt.Rows[i]["invoice_lines_html"] = "<table width=\"100%\">" + html + "</table>";
            dt.Rows[i]["invoice_lines_text"] = text;


            dt.Rows[i]["invoice_lines_count"] = invLinesCount;
            dt.Rows[i]["invoice_lines_total"] = invLinesTotal;
            dt.Rows[i]["commission_total"] = invLinesCommissionTotal;
        }


        Session["data_summaryReport"] = dt;

        if (!IsPostBack)
            chkUsePaging.Checked = dt.Rows.Count > 50;

        this.GrdSummaryReport.AllowPaging = chkUsePaging.Checked;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sortExpression_summaryReport"] != null && Session["sortExpression_summaryReport"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sortExpression_summaryReport"].ToString();
                GrdSummaryReport.DataSource = dataView;
            }
            else
            {
                GrdSummaryReport.DataSource = dt;
            }


            try
            {
                GrdSummaryReport.DataBind();
                GrdSummaryReport.PagerSettings.FirstPageText = "1";
                GrdSummaryReport.PagerSettings.LastPageText = GrdSummaryReport.PageCount.ToString();
                GrdSummaryReport.DataBind();

            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdSummaryReport.DataSource = dt;
            GrdSummaryReport.DataBind();

            int TotalColumns = GrdSummaryReport.Rows[0].Cells.Count;
            GrdSummaryReport.Rows[0].Cells.Clear();
            GrdSummaryReport.Rows[0].Cells.Add(new TableCell());
            GrdSummaryReport.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdSummaryReport.Rows[0].Cells[0].Text = "No Record Found";
        }

    }
    protected void GrdSummaryReport_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";

        if (e.Row.RowType == DataControlRowType.Header && UserView.GetInstance().IsAgedCareView)
            for (int i = 0; i < GrdSummaryReport.Columns.Count; i++)
                GrdSummaryReport.Columns[i].HeaderText = GrdSummaryReport.Columns[i].HeaderText.Replace("EPC", "Ref.");
    }
    protected void GrdSummaryReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["data_summaryReport"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("booking_booking_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];
            Booking booking = BookingDB.Load(thisRow, "booking_", false, false);


            HyperLink lnkBookingSheetForPatient = (HyperLink)e.Row.FindControl("lnkBookingSheetForPatient");
            if (lnkBookingSheetForPatient != null)
                lnkBookingSheetForPatient.NavigateUrl = booking.GetBookingSheetLinkV2();


            Label lnkPatient = (Label)e.Row.FindControl("lnkPatient");
            if (lnkPatient != null)
            {

                if (booking == null || booking.Patient == null)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (BookingPatient bookingPatient in BookingPatientDB.GetByBookingID(booking.BookingID))
                    {
                        string URL = "PatientDetailV2.aspx?type=view&id=" + bookingPatient.Patient.PatientID;
                        if (URL.StartsWith("~")) URL = URL.Substring(1);
                        sb.AppendLine(sb.Length == 0 ? "" : "<br />");
                        sb.AppendLine("<a href=\"" + URL + "\" onclick=\"open_new_tab('" + URL + "');return false;\">" + bookingPatient.Patient.Person.FullnameWithoutMiddlename + "</a>");
                    }
                    lnkPatient.Text = sb.ToString();
                }
                else
                {
                    string URL = "PatientDetailV2.aspx?type=view&id=" + booking.Patient.PatientID;
                    if (URL.StartsWith("~")) URL = URL.Substring(1);
                    lnkPatient.Text = "<a href=\"" + URL + "\" onclick=\"open_new_tab('" + URL + "');return false;\">" + thisRow["patient_firstname"] + " " + thisRow["patient_surname"] + "</a>";
                }
            }


            Label lnkBookingSheet = (Label)e.Row.FindControl("lnkBookingSheet");
            if (lnkBookingSheet != null)
            {
                string URL = booking.GetBookingSheetLinkV2();
                if (URL.StartsWith("~")) URL = URL.Substring(1);
                //lnkBookingSheet.Text = "<a href=\"#\" onclick=\"open_new_window('" + URL + "');return false;\" ><img src=\"images/Calendar-icon-24px.png\"></a>";
                lnkBookingSheet.Text = "<a href=\"" + URL + "\" onclick=\"var win=window.open('" + URL + "', '_blank'); win.focus();return false;\" ><img src=\"images/Calendar-icon-24px.png\"></a>";
            }


            Label lblComplete = (Label)e.Row.FindControl("lblComplete");
            lblComplete.Visible = booking.BookingStatus.ID == 0 && booking.DateEnd > booking.DateStart && booking.DateStart.Hour >= 5 && booking.DateStart.Hour < 22;
            lblComplete.Text = "<a href=\"#\" onclick=\"complete_booking(" + booking.BookingID + ", " + (!UserView.GetInstance().IsAgedCareView ? "true" : "false") + "); return false;\">Complete</a>";


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSum_Cash = (Label)e.Row.FindControl("lblSum_Cash");
            lblSum_Cash.Text = String.Format("{0:C}", dt.Compute("Sum(total_cash_receipts)", ""));
            if (lblSum_Cash.Text == "") lblSum_Cash.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_EFT = (Label)e.Row.FindControl("lblSum_EFT");
            lblSum_EFT.Text = String.Format("{0:C}", dt.Compute("Sum(total_eft_receipts)", ""));
            if (lblSum_EFT.Text == "") lblSum_EFT.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_CreditCard = (Label)e.Row.FindControl("lblSum_CreditCard");
            lblSum_CreditCard.Text = String.Format("{0:C}", dt.Compute("Sum(total_credit_card_receipts)", ""));
            if (lblSum_CreditCard.Text == "") lblSum_CreditCard.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_Cheque = (Label)e.Row.FindControl("lblSum_Cheque");
            lblSum_Cheque.Text = String.Format("{0:C}", dt.Compute("Sum(total_cheque_receipts)", ""));
            if (lblSum_Cheque.Text == "") lblSum_Cheque.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_MoneyOrder = (Label)e.Row.FindControl("lblSum_MoneyOrder");
            lblSum_MoneyOrder.Text = String.Format("{0:C}", dt.Compute("Sum(total_money_order_receipts)", ""));
            if (lblSum_MoneyOrder.Text == "") lblSum_MoneyOrder.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_DirectDebit = (Label)e.Row.FindControl("lblSum_DirectDebit");
            lblSum_DirectDebit.Text = String.Format("{0:C}", dt.Compute("Sum(total_direct_credit_receipts)", ""));
            if (lblSum_DirectDebit.Text == "") lblSum_DirectDebit.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_Medicare = (Label)e.Row.FindControl("lblSum_Medicare");
            lblSum_Medicare.Text = String.Format("{0:C}", dt.Compute("Sum(medicare_invoices_total)", ""));
            if (lblSum_Medicare.Text == "") lblSum_Medicare.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_DVA = (Label)e.Row.FindControl("lblSum_DVA");
            lblSum_DVA.Text = String.Format("{0:C}", dt.Compute("Sum(dva_invoices_total)", ""));
            if (lblSum_DVA.Text == "") lblSum_DVA.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_TotalVouchers = (Label)e.Row.FindControl("lblSum_TotalVouchers");
            lblSum_TotalVouchers.Text = String.Format("{0:C}", dt.Compute("Sum(total_vouchers)", ""));
            if (lblSum_TotalVouchers.Text == "") lblSum_TotalVouchers.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_TotalCreditNotes = (Label)e.Row.FindControl("lblSum_TotalCreditNotes");
            lblSum_TotalCreditNotes.Text = String.Format("{0:C}", dt.Compute("Sum(total_credit_notes)", ""));
            if (lblSum_TotalCreditNotes.Text == "") lblSum_TotalCreditNotes.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_TotalDue = (Label)e.Row.FindControl("lblSum_TotalDue");
            lblSum_TotalDue.Text = String.Format("{0:C}", dt.Compute("Sum(total_due_non_medicare_non_dva)", ""));
            if (lblSum_TotalDue.Text == "") lblSum_TotalDue.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_InvoiceTotal = (Label)e.Row.FindControl("lblSum_InvoiceTotal");
            lblSum_InvoiceTotal.Text = String.Format("{0:C}", dt.Compute("Sum(invoices_total)", ""));
            if (lblSum_InvoiceTotal.Text == "") lblSum_InvoiceTotal.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_InvoiceGSTTotal = (Label)e.Row.FindControl("lblSum_InvoiceGSTTotal");
            lblSum_InvoiceGSTTotal.Text = String.Format("{0:C}", dt.Compute("Sum(invoices_gst_total)", ""));
            if (lblSum_InvoiceGSTTotal.Text == "") lblSum_InvoiceGSTTotal.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            object oInvTotal = dt.Compute("Sum(invoices_total)", "");
            object  oTotal_credit_notes_non_medicare_non_dva = dt.Compute("Sum(total_credit_notes)", "");
            decimal invTotal                                 = oInvTotal == DBNull.Value ? 0 : (decimal)oInvTotal;
            decimal total_credit_notes_non_medicare_non_dva  = oTotal_credit_notes_non_medicare_non_dva == DBNull.Value ? 0 : (decimal)oTotal_credit_notes_non_medicare_non_dva;
            Label lblSum_InvoiceTotalLessAdJNotes = (Label)e.Row.FindControl("lblSum_InvoiceTotalLessAdJNotes");
            lblSum_InvoiceTotalLessAdJNotes.Text = String.Format("{0:C}", invTotal - total_credit_notes_non_medicare_non_dva);
            if (lblSum_InvoiceTotalLessAdJNotes.Text == "") lblSum_InvoiceTotalLessAdJNotes.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            object objInvLinesTotal = dt.Compute("Sum(invoice_lines_total)", "");
            decimal invLinesTotal = Convert.ToDecimal(objInvLinesTotal == DBNull.Value ? 0 : objInvLinesTotal);
            object objCommTotal = dt.Compute("Sum(commission_total)", "");
            decimal commTotal = Convert.ToDecimal(objCommTotal == DBNull.Value ? 0 : objCommTotal);
            Label lblSum_InvoiceLines = (Label)e.Row.FindControl("lblSum_InvoiceLines");
            lblSum_InvoiceLines.Text = "Comm: " + String.Format("{0:C}", commTotal) + "&nbsp;&nbsp;&nbsp;&nbsp; Inv: " + String.Format("{0:C}", invLinesTotal);
        }
    }
    protected void GrdSummaryReport_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "PrintInvoice")
        {
            try
            {
                int bookingID = Convert.ToInt32(e.CommandArgument);

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

        if (e.CommandName == "EmailInvoice")
        {
            try
            {
                int bookingID = Convert.ToInt32(e.CommandArgument);

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

    }
    protected static string GetEmail(Hashtable contactHash, int entityID)
    {
        return ContactDB.GetEmailsCommaSepByEntityID(contactHash, entityID, true, false);
    }
    protected void GrdSummaryReport_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdSummaryReport.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }
    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["data_summaryReport"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression_summaryReport"] == null)
                Session["sortExpression_summaryReport"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression_summaryReport"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["sortExpression_summaryReport"] = sortExpression + " " + newSortExpr;

            GrdSummaryReport.DataSource = dataView;
            GrdSummaryReport.DataBind();
        }
    }
    protected void GrdStaff_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdSummaryReport.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    #endregion

    #region IsValidFormStartDate(),  GetFormStartDate()....

    private bool IsValidFormPatientID()
    {
        string id = Request.QueryString["patient"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormPatientID()
    {
        if (!IsValidFormPatientID())
            throw new Exception("Invalid url patient");

        string id = Request.QueryString["patient"];
        return Convert.ToInt32(id);
    }
    private bool IsValidFormProviderID()
    {
        string id = Request.QueryString["provider"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormProviderID()
    {
        if (!IsValidFormProviderID())
            throw new Exception("Invalid url provider");

        string id = Request.QueryString["provider"];
        return Convert.ToInt32(id);
    }
    private bool IsValidFormOrgID()
    {
        string id = Request.QueryString["org"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormOrgID()
    {
        if (!IsValidFormOrgID())
            throw new Exception("Invalid url org");

        string id = Request.QueryString["org"];
        return Convert.ToInt32(id);
    }


    
    protected bool IsValidFormUseTreatmentDate()
    {
        string use_treatment_date = Request.QueryString["use_treatment_date"];
        return use_treatment_date != null && (use_treatment_date == "0" || use_treatment_date == "1");
    }
    protected bool GetFormUseTreatmentDate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormUseTreatmentDate())
            throw new Exception("Invalid url 'use_treatment_date'");
        return Request.QueryString["use_treatment_date"] == "1";
    }
    protected bool IsValidFormIncCompleted()
    {
        string inc_completed = Request.QueryString["inc_completed"];
        return inc_completed != null && (inc_completed == "0" || inc_completed == "1");
    }
    protected bool GetFormIncCompleted(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncCompleted())
            throw new Exception("Invalid url 'inc_completed'");
        return Request.QueryString["inc_completed"] == "1";
    }
    protected bool IsValidFormIncIncomplete()
    {
        string inc_incomplete = Request.QueryString["inc_incomplete"];
        return inc_incomplete != null && (inc_incomplete == "0" || inc_incomplete == "1");
    }
    protected bool GetFormIncIncomplete(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncIncomplete())
            throw new Exception("Invalid url 'inc_incomplete'");
        return Request.QueryString["inc_incomplete"] == "1";
    }
    protected bool IsValidFormIncCancelled()
    {
        string inc_cancelled = Request.QueryString["inc_cancelled"];
        return inc_cancelled != null && (inc_cancelled == "0" || inc_cancelled == "1");
    }
    protected bool GetFormIncCancelled(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncCancelled())
            throw new Exception("Invalid url 'inc_cancelled'");
        return Request.QueryString["inc_cancelled"] == "1";
    }
    protected bool IsValidFormIncDeleted()
    {
        string inc_deleted = Request.QueryString["inc_deleted"];
        return inc_deleted != null && (inc_deleted == "0" || inc_deleted == "1");
    }
    protected bool GetFormIncDeleted(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncDeleted())
            throw new Exception("Invalid url 'inc_deleted'");
        return Request.QueryString["inc_deleted"] == "1";
    }
    protected bool IsValidFormIncEPC()
    {
        string inc_epc = Request.QueryString["inc_epc"];
        return inc_epc != null && (inc_epc == "0" || inc_epc == "1");
    }
    protected bool GetFormIncEPC(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncEPC())
            throw new Exception("Invalid url 'inc_epc'");
        return Request.QueryString["inc_epc"] == "1";
    }
    protected bool IsValidFormIncNonEPC()
    {
        string inc_nonepc = Request.QueryString["inc_nonepc"];
        return inc_nonepc != null && (inc_nonepc == "0" || inc_nonepc == "1");
    }
    protected bool GetFormIncNonEPC(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncNonEPC())
            throw new Exception("Invalid url 'inc_nonepc'");
        return Request.QueryString["inc_nonepc"] == "1";
    }
    protected bool IsValidFormOnlyPtSelfBookings()
    {
        string only_pt_self_bks = Request.QueryString["only_pt_self_bks"];
        return only_pt_self_bks != null && (only_pt_self_bks == "0" || only_pt_self_bks == "1");
    }
    protected bool GetFormIncOnlyPtSelfBookings(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormOnlyPtSelfBookings())
            throw new Exception("Invalid url 'only_pt_self_bks'");
        return Request.QueryString["only_pt_self_bks"] != "0";
    }

    protected bool IsValidDate(string strDate)
    {
        try
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(strDate, @"^\d{2}\-\d{2}\-\d{4}$"))
                return false;

            string[] parts = strDate.Split('-');
            DateTime d = new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public DateTime GetDate(string inDate)
    {
        inDate = inDate.Trim();

        if (inDate.Length == 0)
        {
            return DateTime.MinValue;
        }
        else
        {
            string[] dobParts = inDate.Split(new char[] { '-' });
            return new DateTime(Convert.ToInt32(dobParts[2]), Convert.ToInt32(dobParts[1]), Convert.ToInt32(dobParts[0]));
        }
    }


    protected bool IsValidFormStartDate()
    {
        string start_date = Request.QueryString["start_date"];
        return start_date != null && (start_date.Length == 0 || Regex.IsMatch(start_date, @"^\d{4}_\d{2}_\d{2}$"));
    }
    protected DateTime GetFormStartDate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormStartDate())
            throw new Exception("Invalid url 'start date'");
        return Request.QueryString["start_date"].Length == 0 ? DateTime.MinValue : GetDateFromString(Request.QueryString["start_date"], "yyyy_mm_dd");
    }
    protected bool IsValidFormEndDate()
    {
        string end_date = Request.QueryString["end_date"];
        return end_date != null && (end_date.Length == 0 || Regex.IsMatch(end_date, @"^\d{4}_\d{2}_\d{2}$"));
    }
    protected DateTime GetFormEndDate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormEndDate())
            throw new Exception("Invalid url 'end date'");
        return Request.QueryString["end_date"].Length == 0 ? DateTime.MinValue : GetDateFromString(Request.QueryString["end_date"], "yyyy_mm_dd");
    }
    protected DateTime GetDateFromString(string sDate, string format)
    {
        if (format == "yyyy_mm_dd")
        {
            string[] dateparts = sDate.Split('_');
            return new DateTime(Convert.ToInt32(dateparts[0]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[2]));
        }
        else if (format == "dd_mm_yyyy")
        {
            string[] dateparts = sDate.Split('_');
            return new DateTime(Convert.ToInt32(dateparts[2]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[0]));
        }
        if (format == "yyyy-mm-dd")
        {
            string[] dateparts = sDate.Split('-');
            return new DateTime(Convert.ToInt32(dateparts[0]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[2]));
        }
        else if (format == "dd-mm-yyyy")
        {
            string[] dateparts = sDate.Split('-');
            return new DateTime(Convert.ToInt32(dateparts[2]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[0]));
        }
        else
            throw new ArgumentOutOfRangeException("Unknown date format");
    }


    #endregion

    #region ddlOrgs_SelectedIndexChanged, ddlProviders_SelectedIndexChanged

    protected void ddlOrgs_SelectedIndexChanged(object sender, EventArgs e)
    {
        int newOrgID = Convert.ToInt32(ddlOrgs.SelectedValue);

        string url = Request.RawUrl;
        url = UrlParamModifier.Update(newOrgID != -1, url, "org", newOrgID == -1 ? "" : newOrgID.ToString());
        Response.Redirect(url);
    }
    protected void ddlProviders_SelectedIndexChanged(object sender, EventArgs e)
    {
        int newProvID = Convert.ToInt32(ddlProviders.SelectedValue);

        string url = Request.RawUrl;
        url = UrlParamModifier.Update(newProvID != -1, url, "provider", newProvID == -1 ? "" : newProvID.ToString());
        Response.Redirect(url);
    }

    #endregion

    #region btnSearch_Click, chkUsePaging_CheckedChanged

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        HideErrorMessage();

        if (txtStartDate.Text.Length > 0 && (!Regex.IsMatch(txtStartDate.Text, @"^\d{2}\-\d{2}\-\d{4}$") || !IsValidDate(txtStartDate.Text)))
        {
            SetErrorMessage("Start date must be empty or valid and of the format dd-mm-yyyy");
            return;
        }
        if (txtEndDate.Text.Length > 0 && (!Regex.IsMatch(txtEndDate.Text, @"^\d{2}\-\d{2}\-\d{4}$") || !IsValidDate(txtEndDate.Text)))
        {
            SetErrorMessage("End date must be empty or valid and of the format dd-mm-yyyy");
            return;
        }


        DateTime startDate = txtStartDate.Text.Length == 0 ? DateTime.MinValue : GetDateFromString(txtStartDate.Text, "dd-mm-yyyy");
        DateTime endDate = txtEndDate.Text.Length == 0 ? DateTime.MinValue : GetDateFromString(txtEndDate.Text, "dd-mm-yyyy");

        string url = ClearSearchesFromUrl(Request.RawUrl);
        url = UrlParamModifier.AddEdit(url, "start_date", startDate == DateTime.MinValue ? "" : startDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "end_date", endDate == DateTime.MinValue ? "" : endDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "use_treatment_date", dateTypeToUse.SelectedValue == "booking_date_start" ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_completed", chkIncCompleted.Checked ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_incomplete", chkIncIncomplete.Checked ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_cancelled", chkIncCancelled.Checked ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_deleted", chkIncDeleted.Checked ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_epc", chkIncEPC.Checked ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "only_pt_self_bks", chkOnlyPtSelfBookings.Checked ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_nonepc", chkIncNonEPC.Checked ? "1" : "0");

        Response.Redirect(url);
    }

    protected string ClearSearchesFromUrl(string url)
    {
        url = UrlParamModifier.Remove(url, "start_date");
        url = UrlParamModifier.Remove(url, "end_date");
        url = UrlParamModifier.Remove(url, "use_treatment_date");
        url = UrlParamModifier.Remove(url, "inc_completed");
        url = UrlParamModifier.Remove(url, "inc_incomplete");
        url = UrlParamModifier.Remove(url, "inc_cancelled");
        url = UrlParamModifier.Remove(url, "inc_deleted");
        url = UrlParamModifier.Remove(url, "inc_epc");
        url = UrlParamModifier.Remove(url, "inc_nonepc");
        url = UrlParamModifier.Remove(url, "only_pt_self_bks");
        url = UrlParamModifier.Remove(url, "booking_nbr_search");

        return url;
    }

    protected void chkUsePaging_CheckedChanged(object sender, EventArgs e)
    {
        this.GrdSummaryReport.AllowPaging = chkUsePaging.Checked;
        FillGrid();
    }

    #endregion

    #region btnExport_Click

    protected void btnExport_Click(object sender, EventArgs e)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("\"" + "Organisation" + "\"").Append(",");
        sb.Append("\"" + "Treatment Date" + "\"").Append(",");
        sb.Append("\"" + "Provider" + "\"").Append(",");
        sb.Append("\"" + "Patient" + "\"").Append(",");
        sb.Append("\"" + "Status" + "\"").Append(",");

        sb.Append("\"" + "Cash" + "\"").Append(",");
        sb.Append("\"" + "Cheque" + "\"").Append(",");
        sb.Append("\"" + "Credit Card" + "\"").Append(",");
        sb.Append("\"" + "HICAPS" + "\"").Append(",");
        sb.Append("\"" + "Money Order" + "\"").Append(",");
        sb.Append("\"" + "Direct Debit" + "\"").Append(",");
        sb.Append("\"" + "DVA" + "\"").Append(",");
        sb.Append("\"" + "Medicare" + "\"").Append(",");
        sb.Append("\"" + "Adj Notes" + "\"").Append(",");
        sb.Append("\"" + "Owing" + "\"").Append(",");

        sb.Append("\"" + "Service" + "\"").Append(",");
        sb.Append("\"" + "Service Price" + "\"").Append(",");
        sb.Append("\"" + "Booking" + "\"").Append(",");
        sb.Append("\"" + "Inv Amt" + "\"").Append(",");
        sb.Append("\"" + "Inv Amt Less Adj Notes" + "\"").Append(",");
        sb.Append("\"" + "GST" + "\"").Append(",");

        sb.Append("\"" + "Inv Lines" + "\"");

        sb.AppendLine();


        DataTable dt = Session["data_summaryReport"] as DataTable;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("\"" + dt.Rows[i]["organisation_name"].ToString() + "\"").Append(",");
                sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["booking_date_start"]).ToString("dd MMM yyyy HH:mm") + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["provider_firstname"].ToString() + " " + dt.Rows[i]["provider_firstname"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["patient_firstname"].ToString() + " " + dt.Rows[i]["patient_surname"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["booking_status_descr"].ToString() + "\"").Append(",");

                sb.Append("\"" + (dt.Rows[i]["total_cash_receipts"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["total_cash_receipts"]) == 0 ? "" : Convert.ToDecimal(dt.Rows[i]["total_cash_receipts"]).ToString()) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["total_cheque_receipts"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["total_cheque_receipts"]) == 0 ? "" : Convert.ToDecimal(dt.Rows[i]["total_cheque_receipts"]).ToString()) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["total_credit_card_receipts"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["total_credit_card_receipts"]) == 0 ? "" : Convert.ToDecimal(dt.Rows[i]["total_credit_card_receipts"]).ToString()) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["total_eft_receipts"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["total_eft_receipts"]) == 0 ? "" : Convert.ToDecimal(dt.Rows[i]["total_eft_receipts"]).ToString()) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["total_money_order_receipts"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["total_money_order_receipts"]) == 0 ? "" : Convert.ToDecimal(dt.Rows[i]["total_money_order_receipts"]).ToString()) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["total_direct_credit_receipts"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["total_direct_credit_receipts"]) == 0 ? "" : Convert.ToDecimal(dt.Rows[i]["total_direct_credit_receipts"]).ToString()) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["dva_invoices_total"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["total_direct_credit_receipts"]) == 0 ? "" : Convert.ToDecimal(dt.Rows[i]["dva_invoices_total"]).ToString()) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["medicare_invoices_total"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["medicare_invoices_total"]) == 0 ? "" : Convert.ToDecimal(dt.Rows[i]["medicare_invoices_total"]).ToString()) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["total_credit_notes_non_medicare_non_dva"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["total_credit_notes_non_medicare_non_dva"]) == 0 ? "" : Convert.ToDecimal(dt.Rows[i]["total_credit_notes_non_medicare_non_dva"]).ToString()) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["total_due_non_medicare_non_dva"] == DBNull.Value ? "" : Convert.ToDecimal(dt.Rows[i]["total_due_non_medicare_non_dva"]).ToString()) + "\"").Append(",");

                sb.Append("\"" + dt.Rows[i]["offering_name"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["offering_default_price"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["booking_booking_id"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["invoices_total"].ToString() + "\"").Append(",");
                sb.Append("\"" + ((decimal)dt.Rows[i]["invoices_total"] - (decimal)dt.Rows[i]["total_credit_notes_non_medicare_non_dva"]).ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["invoices_gst_total"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["invoice_lines_text"].ToString() + "\"").Append(",");
                sb.AppendLine();
            }
        }

        string Sum_Cash = String.Format("{0:C}", dt.Compute("Sum(total_cash_receipts)", ""));
        if (Sum_Cash == "") Sum_Cash = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_EFT = String.Format("{0:C}", dt.Compute("Sum(total_eft_receipts)", ""));
        if (Sum_EFT == "") Sum_EFT = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_CreditCard = String.Format("{0:C}", dt.Compute("Sum(total_credit_card_receipts)", ""));
        if (Sum_CreditCard == "") Sum_CreditCard = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_Cheque = String.Format("{0:C}", dt.Compute("Sum(total_cheque_receipts)", ""));
        if (Sum_Cheque == "") Sum_Cheque = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_MoneyOrder = String.Format("{0:C}", dt.Compute("Sum(total_money_order_receipts)", ""));
        if (Sum_MoneyOrder == "") Sum_MoneyOrder = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_DirectDebit = String.Format("{0:C}", dt.Compute("Sum(total_direct_credit_receipts)", ""));
        if (Sum_DirectDebit == "") Sum_DirectDebit = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_Medicare = String.Format("{0:C}", dt.Compute("Sum(medicare_invoices_total)", ""));
        if (Sum_Medicare == "") Sum_Medicare = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_DVA = String.Format("{0:C}", dt.Compute("Sum(dva_invoices_total)", ""));
        if (Sum_DVA == "") Sum_DVA = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_TotalCreditNotes = String.Format("{0:C}", dt.Compute("Sum(total_credit_notes_non_medicare_non_dva)", ""));
        if (Sum_TotalCreditNotes == "") Sum_TotalCreditNotes = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_TotalDue = String.Format("{0:C}", dt.Compute("Sum(total_due_non_medicare_non_dva)", ""));
        if (Sum_TotalDue == "") Sum_TotalDue = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_InvoiceTotal = String.Format("{0:C}", dt.Compute("Sum(invoices_total)", ""));
        if (Sum_InvoiceTotal == "") Sum_InvoiceTotal = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_InvoiceGSTTotal = String.Format("{0:C}", dt.Compute("Sum(invoices_gst_total)", ""));
        if (Sum_InvoiceGSTTotal == "") Sum_InvoiceGSTTotal = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";


        object  oSumInvTotal        = dt.Compute("Sum(invoices_total)", "");
        object  oSumCreditNoteTotal = dt.Compute("Sum(total_credit_notes)", "");
        decimal sumInvtotal         = oSumInvTotal        == DBNull.Value ? 0 : (decimal)dt.Compute("Sum(invoices_total)", "");
        decimal sumCreditNoteTotal  = oSumCreditNoteTotal == DBNull.Value ? 0 : (decimal)dt.Compute("Sum(total_credit_notes)", "");
        string Sum_InvoiceTotalLessAdjNotes = String.Format("{0:C}", (sumInvtotal - sumCreditNoteTotal));
        if (Sum_InvoiceTotalLessAdjNotes == "") Sum_InvoiceTotalLessAdjNotes = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";


        object objInvLinesTotal = dt.Compute("Sum(invoice_lines_total)", "");
        decimal invLinesTotal = Convert.ToDecimal(objInvLinesTotal == DBNull.Value ? 0 : objInvLinesTotal);
        object objCommTotal = dt.Compute("Sum(commission_total)", "");
        decimal commTotal = Convert.ToDecimal(objCommTotal == DBNull.Value ? 0 : objCommTotal);
        //string Sum_InvoiceLines = "Comm: " + String.Format("{0:C}", commTotal) + "     Inv: " + String.Format("{0:C}", invLinesTotal);
        string Sum_InvoiceLines = String.Format("{0:C}", commTotal); // when exporting, only show the comm as single nbr



        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");

        sb.Append("\"" + Sum_Cash + "\"").Append(",");
        sb.Append("\"" + Sum_Cheque + "\"").Append(",");
        sb.Append("\"" + Sum_CreditCard + "\"").Append(",");
        sb.Append("\"" + Sum_EFT + "\"").Append(",");
        sb.Append("\"" + Sum_MoneyOrder + "\"").Append(",");
        sb.Append("\"" + Sum_DirectDebit + "\"").Append(",");
        sb.Append("\"" + Sum_DVA + "\"").Append(",");
        sb.Append("\"" + Sum_Medicare + "\"").Append(",");
        sb.Append("\"" + Sum_TotalCreditNotes + "\"").Append(",");
        sb.Append("\"" + Sum_TotalDue + "\"").Append(",");

        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");

        sb.Append("\"" + Sum_InvoiceTotal + "\"").Append(",");
        sb.Append("\"" + Sum_InvoiceTotalLessAdjNotes + "\"").Append(",");
        sb.Append("\"" + Sum_InvoiceGSTTotal + "\"").Append(",");

        sb.Append("\"" + Sum_InvoiceLines + "\"").Append(",");

        sb.AppendLine();


        ExportCSV(Response, sb.ToString(), "SummaryReport.csv");
    }
    protected static void ExportCSV(HttpResponse response, string fileText, string fileName)
    {
        byte[] buffer = GetBytes(fileText);

        try
        {
            response.Clear();
            response.ContentType = "text/plain";
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            response.End();
        }
        catch (System.Web.HttpException ex)
        {
            // ignore exception where user closed the download box
            if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                throw;
        }
    }
    protected static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    #endregion


    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
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