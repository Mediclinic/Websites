using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxDoBooking : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            UserView userView = UserView.GetInstance();

            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();
            if ((!userView.IsStaff && !userView.IsPatient) || Session["SiteID"] == null)
                throw new SessionTimedOutException();

            if (GetUrlParamType() == UrlParamType.Add)
                AddBooking();
            else if (GetUrlParamType() == UrlParamType.AddRecurring)
                AddRecurringBooking();
            else if (GetUrlParamType() == UrlParamType.Edit)
                EditBooking();
            else if (GetUrlParamType() == UrlParamType.EditFullDay)
                EditFullDayBookings();
            else if (GetUrlParamType() == UrlParamType.Reverse)
                ReverseBooking(true);
            else if (GetUrlParamType() == UrlParamType.CanReverse)
                ReverseBooking(false);
            else if (GetUrlParamType() == UrlParamType.Delete)
                ConfirmOrDeleteOrCancelBooking(UrlParamType.Delete);
            else if (GetUrlParamType() == UrlParamType.Confirm)
                ConfirmOrDeleteOrCancelBooking(UrlParamType.Confirm);
            else if (GetUrlParamType() == UrlParamType.Unconfirm)
                ConfirmOrDeleteOrCancelBooking(UrlParamType.Unconfirm);
            else if (GetUrlParamType() == UrlParamType.Cancel)
                ConfirmOrDeleteOrCancelBooking(UrlParamType.Cancel);
            else if (GetUrlParamType() == UrlParamType.Arrived)
                ConfirmOrDeleteOrCancelBooking(UrlParamType.Arrived);
            else if (GetUrlParamType() == UrlParamType.Unarrived)
                ConfirmOrDeleteOrCancelBooking(UrlParamType.Unarrived);
            else if (GetUrlParamType() == UrlParamType.Edit)
                EditBooking();
            else
                throw new Exception("Unknown type");

            Response.Write("0");
        }
        catch (SessionTimedOutException)
        {
            Response.Write("1");
        }
        catch (CustomMessageException ex)
        {
            Response.Write("2:" + ex.Message);
        }
        catch (Exception ex)
        {
            Response.Write("3:" + (Utilities.IsDev() ? ex.ToString() : "Error - please contact system administrator."));
        }
    }

    #endregion


    protected void EditBooking()
    {
        //UrlReturnPage returnPage    = GetUrlReturnPage();
        bool?         checkClashAllOrgs = UrlCheckClashAllOrgs;

        Booking       booking       = UrlBooking;
        DateTime?     startDateTime = UrlStartDateTime;
        DateTime?     endDateTime   = UrlEndDateTime;
        Patient       patient       = UrlPatient;
        Organisation  org           = UrlOrg;
        Staff         staff         = UrlStaff;
        Offering      offering      = UrlOffering;
        bool?         confirmed     = UrlIsConfirmed;
        int?          editReason    = UrlEditReasonID;

        if (booking == null)
            throw new Exception("Invalid url field booking_id");
        if (startDateTime == null)
            throw new Exception("Invalid url field start_datetime");
        if (endDateTime == null)
            throw new Exception("Invalid url field end_datetime");
        if (org == null)
            throw new Exception("Invalid url field org_id");
        if (staff == null)
            throw new Exception("Invalid url field staff_id");
        if (confirmed == null)
            throw new Exception("Invalid url field is_confirmed");
        if (editReason == null)
            throw new Exception("Invalid url field edit_reason_id");

        if (booking.AddedBy == null)
            throw new CustomMessageException("Error - please contact system administrator.\r\n\r\nError Details:\r\nBooking 'Added By' is not set and must be set. BK ID: " + booking.BookingID);

        // check booking is valid ie no overlapping with current bookings
        Booking[] bookings = BookingDB.GetToCheckOverlap_OneTime(startDateTime.Value, endDateTime.Value, staff, checkClashAllOrgs.Value ? null : org, booking.BookingTypeID == 342, true, false);
        if (Booking.HasOverlap(bookings, startDateTime.Value, endDateTime.Value, booking))
        {
            string fromTime = startDateTime.Value.Hour.ToString().PadLeft(2, '0') + ":" + startDateTime.Value.Minute.ToString().PadLeft(2, '0');
            string toTime   = endDateTime.Value.Hour.ToString().PadLeft(2, '0')   + ":" + endDateTime.Value.Minute.ToString().PadLeft(2, '0');
            throw new CustomMessageException("Can not book " + startDateTime.Value.ToString(@"ddd MMM d") + " " + fromTime + "-" + toTime + " due to overlap with existing booking");
        }


        int booking_confirmed_by_type_id = !confirmed.Value ? -1 : 1;
        int      confirmedBy   = !confirmed.Value ? -1                : (booking.ConfirmedBy == null ? GetStaffID() : booking.ConfirmedBy.StaffID);
        DateTime dateConfirmed = !confirmed.Value ? DateTime.MinValue : (booking.ConfirmedBy == null ? DateTime.Now : booking.DateConfirmed);

        if (patient != null && !RegisterPatientDB.IsPatientRegisteredToOrg(patient.PatientID, org.OrganisationID))
            RegisterPatientDB.Insert(org.OrganisationID, patient.PatientID);

        BookingChangeHistoryDB.Insert(booking.BookingID, GetStaffID(), Convert.ToInt32(editReason.Value), booking.DateStart);
        BookingDB.Update(booking.BookingID, startDateTime.Value, endDateTime.Value, org.OrganisationID, staff.StaffID, patient == null ? -1 : patient.PatientID, offering == null ? -1 : offering.OfferingID,
                    booking.BookingTypeID, booking.BookingStatus.ID, -1, booking.AddedBy.StaffID, booking_confirmed_by_type_id, confirmedBy, dateConfirmed,
                    booking.DeletedBy == null ? -1 : booking.DeletedBy.StaffID, booking.DateDeleted, booking.CancelledBy == null ? -1 : booking.CancelledBy.StaffID, booking.DateCancelled, booking.IsPatientMissedAppt, booking.IsProviderMissedAppt, booking.IsEmergency, booking.IsRecurring, booking.RecurringDayOfWeek, booking.RecurringStartTime, booking.RecurringEndTime);

        if (booking.BookingTypeID == 34)
        {
            Booking newBooking = BookingDB.GetByID(booking.BookingID);
            newBooking.SendReminderEmail(booking);
        }

        if (booking.ArrivalTime != DateTime.MinValue && booking.DateStart != startDateTime)
            BookingDB.RemoveArrivalTime(booking.BookingID);
    }

    protected void EditFullDayBookings()
    {
        DateTime?    old_date = UrlEditFullDay_OldDate;
        DateTime?    new_date = UrlEditFullDay_NewDate;
        Organisation old_org  = UrlEditFullDay_OldOrg;
        Organisation new_org  = UrlEditFullDay_NewOrg;
        Staff        old_prov = UrlEditFullDay_OldProvider;
        Staff        new_prov = UrlEditFullDay_NewProvider;

        if (old_date == null)
            throw new Exception("Invalid url field editfullday_old_date");
        if (new_date == null)
            throw new Exception("Invalid url field editfullday_new_date");
        if (old_org == null)
            throw new Exception("Invalid url field editfullday_old_org");
        if (new_org == null)
            throw new Exception("Invalid url field editfullday_new_org");
        if (old_prov == null)
            throw new Exception("Invalid url field editfullday_old_provider");
        if (new_prov == null)
            throw new Exception("Invalid url field editfullday_new_provider");


        // get all bookings from that day/staff/org
        // check each for clash

        bool hasClash = false;
        System.Collections.ArrayList clashes = new System.Collections.ArrayList();
        Booking[] daysBookingList = BookingDB.GetBetween(old_date.Value, old_date.Value.AddDays(1), new Staff[] { old_prov }, new Organisation[] { old_org }, null, null, false, "0", false);

        // remove any 34 types not for this org
        System.Collections.ArrayList tmp = new System.Collections.ArrayList();
        foreach (Booking booking in daysBookingList)
            if (booking.BookingTypeID != 34 || (booking.Organisation.OrganisationID == old_org.OrganisationID && booking.Provider.StaffID == old_prov.StaffID))
                tmp.Add(booking);
        daysBookingList = (Booking[])tmp.ToArray(typeof(Booking));

        foreach (Booking booking in daysBookingList)
        {
            DateTime newDateStart = new_date.Value.AddHours(booking.DateStart.Hour).AddMinutes(booking.DateStart.Minute).AddSeconds(booking.DateStart.Second);
            DateTime newDateEnd   = new_date.Value.AddHours(booking.DateEnd.Hour).AddMinutes(booking.DateEnd.Minute).AddSeconds(booking.DateEnd.Second);

            Booking[] bookings = BookingDB.GetToCheckOverlap_OneTime(newDateStart, newDateEnd, new_prov, null, true, true, true, new Booking[] { booking });

            // remove any non 34 types (unavailabilities) for other orgs
            tmp = new System.Collections.ArrayList();
            foreach (Booking bk in bookings)
                if (bk.Organisation.OrganisationID == old_org.OrganisationID || bk.BookingTypeID == 34)
                    tmp.Add(bk);
            bookings = (Booking[])tmp.ToArray(typeof(Booking));

            if (Booking.HasOverlap(bookings, newDateStart, newDateEnd, booking))
            {
                hasClash = true;
                clashes.Add(booking.DateStart.ToString("yyyy-MM-dd HH:mm") + (booking.Patient == null ? "" : " " + booking.Patient.Person.FullnameWithoutMiddlename));
                //break;
            }
        }

        if (hasClash)
        {
            string clashesString = string.Empty;
            foreach (string s in clashes)
                clashesString += (clashesString.Length == 0 ? "" : "\r\n") + s;
            throw new CustomMessageException("There are clashes that need to be moved first:\r\n\r\nThese are unable to be moved:\r\n" + clashesString);
        }

        foreach (Booking booking in daysBookingList)
        {
            if (booking.BookingTypeID != 34)
                continue;

            DateTime newDateStart = new_date.Value.AddHours(booking.DateStart.Hour).AddMinutes(booking.DateStart.Minute).AddSeconds(booking.DateStart.Second);
            DateTime newDateEnd   = new_date.Value.AddHours(booking.DateEnd.Hour).AddMinutes(booking.DateEnd.Minute).AddSeconds(booking.DateEnd.Second);

            int bookingChangeHistoryReason = 276; // 276 = Admininstration reschedule needs
            BookingChangeHistoryDB.Insert(booking.BookingID, GetStaffID(), bookingChangeHistoryReason, booking.DateStart);
            BookingDB.Update(booking.BookingID, newDateStart, newDateEnd, new_org.OrganisationID, new_prov.StaffID, booking.Patient == null ? -1 : booking.Patient.PatientID, booking.Offering == null ? -1 : booking.Offering.OfferingID,
                             booking.BookingTypeID, booking.BookingStatus.ID, -1, booking.AddedBy.StaffID, booking.BookingConfirmedByType == null ? -1 : booking.BookingConfirmedByType.ID, booking.ConfirmedBy == null ? -1 : booking.ConfirmedBy.StaffID, booking.DateConfirmed,
                             booking.DeletedBy == null ? -1 : booking.DeletedBy.StaffID, booking.DateDeleted,
                             booking.CancelledBy == null ? -1 : booking.CancelledBy.StaffID, booking.DateCancelled, 
                             booking.IsPatientMissedAppt, booking.IsProviderMissedAppt, booking.IsEmergency, booking.IsRecurring, booking.RecurringDayOfWeek, booking.RecurringStartTime, booking.RecurringEndTime);
        }
    }


    protected void ConfirmOrDeleteOrCancelBooking(UrlParamType urlParamType)
    {
        Booking booking = UrlBooking;
        if (booking == null)
            throw new Exception("Invalid url field booking_id");

        if (urlParamType == UrlParamType.Delete)
            BookingDB.UpdateSetDeleted(booking.BookingID, GetStaffID());
        else if (urlParamType == UrlParamType.Confirm)
            BookingDB.UpdateSetConfirmed(booking.BookingID, 1, GetStaffID());
        else if (urlParamType == UrlParamType.Unconfirm)
            BookingDB.UpdateSetUnconfirmed(booking.BookingID);
        else if (urlParamType == UrlParamType.Arrived)
            BookingDB.UpdateSetArrivalTime(booking.BookingID);
        else if (urlParamType == UrlParamType.Unarrived)
            BookingDB.RemoveArrivalTime(booking.BookingID);
        else if (urlParamType == UrlParamType.Cancel)
            BookingDB.UpdateSetCancelledByPatient(booking.BookingID, GetStaffID());
        else if (urlParamType == UrlParamType.Deceased)
        {
            PatientDB.UpdateDeceased(booking.Patient.PatientID, GetStaffID());
            BookingDB.UpdateSetDeceasedByPatient(booking.BookingID);
        }
    }

    protected void AddBooking()
    {
        //UrlReturnPage returnPage    = GetUrlReturnPage();
        bool?         checkClashAllOrgs = UrlCheckClashAllOrgs;

        DateTime?     startDateTime     = UrlStartDateTime;
        DateTime?     endDateTime       = UrlEndDateTime;
        int?          bookingTypeID     = UrlBookingTypeID;
        Patient       patient           = UrlPatient;
        Organisation  org               = UrlOrg;
        Staff         staff             = UrlStaff;
        Offering      offering          = UrlOffering;
        bool?         confirmed         = UrlIsConfirmed;

        if (startDateTime == null)
            throw new Exception("Invalid url field start_datetime");
        if (endDateTime == null)
            throw new Exception("Invalid url field end_datetime");
        if (bookingTypeID == null)
            throw new Exception("Invalid url field booking_type_id");
        if (org == null)
            throw new Exception("Invalid url field org_id");
        if (staff == null)
            throw new Exception("Invalid url field staff_id");
        if (confirmed == null)
            throw new Exception("Invalid url field is_confirmed");


        int booking_confirmed_by_type_id = !confirmed.Value ? -1 : 1;
        int      confirmedBy   = !confirmed.Value ? -1                : GetStaffID();
        DateTime dateConfirmed = !confirmed.Value ? DateTime.MinValue : DateTime.Now;

        if (bookingTypeID.Value == 34)
        {
            // check booking is valid ie no overlapping with current bookings
            Booking[] bookings = BookingDB.GetToCheckOverlap_OneTime(startDateTime.Value, endDateTime.Value, staff, checkClashAllOrgs.Value ? null : org, bookingTypeID.Value == 342, true, false);
            if (Booking.HasOverlap(bookings, startDateTime.Value, endDateTime.Value))
            {
                string fromTime = startDateTime.Value.Hour.ToString().PadLeft(2, '0') + ":" + startDateTime.Value.Minute.ToString().PadLeft(2, '0');
                string toTime = endDateTime.Value.Hour.ToString().PadLeft(2, '0') + ":" + endDateTime.Value.Minute.ToString().PadLeft(2, '0');
                throw new CustomMessageException("Can not book " + startDateTime.Value.ToString(@"ddd MMM d") + " " + fromTime + "-" + toTime + " due to overlap with existing booking");
            }


            // set prev for this pt/org inactive and put new one so that the most recent orgs for reg-pt items is the org with the most recent booking
            //if (!RegisterPatientDB.IsPatientRegisteredToOrg(patient.PatientID, org.OrganisationID))
            //    RegisterPatientDB.Insert(org.OrganisationID, patient.PatientID);
            if (patient != null && org != null)
            {
                RegisterPatientDB.UpdateInactive(patient.PatientID, org.OrganisationID, false);
                RegisterPatientDB.Insert(org.OrganisationID, patient.PatientID);
            }

            int newBookingID = BookingDB.Insert(startDateTime.Value, endDateTime.Value, org == null ? 0 : org.OrganisationID, staff == null ? 0 : staff.StaffID, patient == null ? -1 : patient.PatientID, offering == null ? -1 : offering.OfferingID,
                                                bookingTypeID.Value, 0, -1, GetStaffID(), booking_confirmed_by_type_id, confirmedBy, dateConfirmed, -1, DateTime.MinValue, -1, DateTime.MinValue, false, false, false, false, startDateTime.Value.DayOfWeek, TimeSpan.Zero, TimeSpan.Zero);

            Booking newBooking = BookingDB.GetByID(newBookingID);
            newBooking.SendReminderEmail();
        }
        else
        {
            // make sepertae booking for each day so that they can delete individual days
            int nDays = (int)endDateTime.Value.Subtract(startDateTime.Value).TotalHours / 24;
            for (int i = 0; i < nDays; i++)
            {
                // check if have booking for this day already
                Booking[] bookings = BookingDB.GetUnavailableDaysByStartEndDate(startDateTime.Value.AddDays(i), startDateTime.Value.AddDays(i + 1), staff, org);
                if (bookings.Length == 0)
                    BookingDB.Insert(startDateTime.Value.AddDays(i), startDateTime.Value.AddDays(i).Date.AddHours(23).AddMinutes(59).AddSeconds(59), org == null ? 0 : org.OrganisationID, staff == null ? -1 : staff.StaffID, patient == null ? -1 : patient.PatientID, offering == null ? -1 : offering.OfferingID,
                             bookingTypeID.Value, 0, -1, GetStaffID(), booking_confirmed_by_type_id, confirmedBy, dateConfirmed, -1, DateTime.MinValue, -1, DateTime.MinValue, false, false, false, false, startDateTime.Value.DayOfWeek, TimeSpan.Zero, TimeSpan.Zero);
            }
        }

    }

    protected void AddRecurringBooking()
    {
        //UrlReturnPage returnPage    = GetUrlReturnPage();
        bool? checkClashAllOrgs = UrlCheckClashAllOrgs;

        DateTime?     startDateTime          = UrlStartDateTime;
        DateTime?     endDateTime            = UrlEndDateTime;
        TimeSpan?     startTime              = UrlStartTime;
        TimeSpan?     endTime                = UrlEndTime;
        int?          bookingTypeID          = UrlBookingTypeID;
        Patient       patient                = UrlPatient;
        Organisation  org                    = UrlOrg;
        Staff         staff                  = UrlStaff;
        Offering      offering               = UrlOffering;
        bool?         confirmed              = UrlIsConfirmed;
        int?          unavailabilityReasonID = UrlUnavailabilityReasonID;
        string        days                   = UrlBookingDays;
        bool?         createAsSeries         = UrlCreateAsSeries;
        int?          everyNWeeks            = UrlEveryNWeeks;



        if (startDateTime == null)
            throw new Exception("Invalid url field start_datetime");
        if (endDateTime == null)  // if no end time for recurring booking, url has "NULL" which returns this as DateTime.MinValue (not as null)
            throw new Exception("Invalid url field end_datetime");
        if (startTime == null)
            throw new Exception("Invalid url field start_time");
        if (endTime == null)
            throw new Exception("Invalid url field end_time");
        if (bookingTypeID == null)
            throw new Exception("Invalid url field booking_type_id");
        //if (org == null)
        //    throw new Exception("Invalid url field org_id");
        //if (staff == null)
        //    throw new Exception("Invalid url field staff_id");
        //if (offering == null)
        //    throw new Exception("Invalid url field offering_id");
        if (confirmed == null)
            throw new Exception("Invalid url field is_confirmed");
        if (unavailabilityReasonID == null)
            throw new Exception("Invalid url field unavailability_reason_id");
        if (days == null)
            throw new Exception("Invalid url field days");
        if (createAsSeries == null)
            throw new Exception("Invalid url field create_as_series");
        if (everyNWeeks == null)
            throw new Exception("Invalid url field every_n_weeks");



        if (endTime.Value == new TimeSpan(23, 59, 0))
            endTime = new TimeSpan(23, 59, 59);


        int booking_confirmed_by_type_id = !confirmed.Value ? -1 : 1;
        int      confirmedBy   = !confirmed.Value ? -1                : GetStaffID();
        DateTime dateConfirmed = !confirmed.Value ? DateTime.MinValue : DateTime.Now;

        if (bookingTypeID.Value == 34)
        {
            // This has not been tested!!!
            //throw new Exception();
            
            if (createAsSeries.Value)
                throw new Exception();

            // check booking is valid ie no overlapping with current bookings
            Booking[] bookings = BookingDB.GetToCheckOverlap_Recurring(startDateTime.Value, endDateTime.Value, startTime.Value, endTime.Value, days, staff, org, false, true, false, true);
            Booking[] overlappingBookings = Booking.GetOverlappingBookings(bookings, startDateTime.Value, endDateTime.Value, days, startTime.Value, endTime.Value, everyNWeeks.Value);
            if (overlappingBookings.Length > 0)
            {
                string fromTime = startDateTime.Value.Hour.ToString().PadLeft(2, '0') + ":" + startDateTime.Value.Minute.ToString().PadLeft(2, '0');
                string toTime   = endDateTime.Value.Hour.ToString().PadLeft(2, '0') + ":" + endDateTime.Value.Minute.ToString().PadLeft(2, '0');
                throw new CustomMessageException("Can not book due to overlap with existing booking");
            }


            // Make booking for each weekday
            for (int i = 0; i < 7; i++)
            {
                if (days[i] != '1')
                    continue;
                DayOfWeek dayOfWeek = WeekDayDB.GetDayOfWeek(i + 1);


                // get which dates will occur, and create individual bookings
                DateTime curStartDate = startDateTime.Value;
                while (curStartDate.DayOfWeek != dayOfWeek)
                    curStartDate = curStartDate.AddDays(1);

                DateTime curStartDateTime = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, startTime.Value.Hours, startTime.Value.Minutes, 0);
                DateTime curEndDateTime   = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, endTime.Value.Hours, endTime.Value.Minutes, 0);
                int weekNbr = 0;
                while (curStartDateTime.Date <= endDateTime.Value.Date)
                {
                    if (weekNbr % everyNWeeks.Value == 0)
                        BookingDB.Insert(curStartDateTime, curEndDateTime, org.OrganisationID, staff == null ? -1 : staff.StaffID, patient == null ? -1 : patient.PatientID, offering == null ? -1 : offering.OfferingID,
                            bookingTypeID.Value, 0, unavailabilityReasonID == null ? -1 : unavailabilityReasonID.Value, GetStaffID(), booking_confirmed_by_type_id, confirmedBy, dateConfirmed, -1, DateTime.MinValue, -1, DateTime.MinValue, false, false, false, false, curStartDateTime.DayOfWeek, TimeSpan.Zero, TimeSpan.Zero);

                    curStartDateTime = curStartDateTime.AddDays(7);
                    curEndDateTime   = curEndDateTime.AddDays(7);
                    weekNbr++;
                }

            }
        }
        else
        {
            // check booking is valid ie no overlapping with current bookings
            Booking[] bookings = BookingDB.GetToCheckOverlap_Recurring(startDateTime.Value, endDateTime.Value, startTime.Value, endTime.Value, days, staff, checkClashAllOrgs.Value && (bookingTypeID.Value != 342 && bookingTypeID.Value != 341) ? null : org, bookingTypeID.Value == 342 || bookingTypeID.Value == 341, bookingTypeID.Value != 341, false, true);
            Booking clash = Booking.HasOverlap(bookings, startDateTime.Value, endDateTime.Value, days, startTime.Value, endTime.Value, everyNWeeks.Value);
            if (clash != null)
            {
                string fromTime = startDateTime.Value.Hour.ToString().PadLeft(2, '0') + ":" + startDateTime.Value.Minute.ToString().PadLeft(2, '0');
                string toTime   = endDateTime.Value.Hour.ToString().PadLeft(2, '0')   + ":" + endDateTime.Value.Minute.ToString().PadLeft(2, '0');
                bool isFullDay  = startDateTime.Value.Hour == 0 && startDateTime.Value.Minute == 0 && endDateTime.Value.Hour == 0 && endDateTime.Value.Minute == 0;
                throw new CustomMessageException("Can not book " + startDateTime.Value.ToString(@"ddd MMM d") + (isFullDay ? "" : " " + fromTime + "-" + toTime) + " due to overlap with existing booking on " + clash.DateStart.ToString(@"ddd MMM d ") + clash.DateStart.ToString("h:mm") + (clash.DateStart.Hour < 12 ? "am" : "pm"));
            }

            // Make booking for each weekday
            for (int i = 0; i < 7; i++)
            {
                if (days[i] != '1')
                    continue;
                DayOfWeek dayOfWeek = WeekDayDB.GetDayOfWeek(i + 1);


                if (createAsSeries.Value)
                {
                    DateTime dateStart = startDateTime.Value;
                    while (dateStart.DayOfWeek != dayOfWeek)
                        dateStart = dateStart.AddDays(1);
                    DateTime dateEnd = endDateTime.Value;
                    while (dateEnd != DateTime.MinValue && dateEnd.DayOfWeek != dayOfWeek)
                        dateEnd = dateEnd.AddDays(-1);

                    BookingDB.Insert(dateStart, dateEnd, org == null || bookingTypeID.Value == 342 ? 0 : org.OrganisationID, staff == null ? -1 : staff.StaffID, patient == null ? -1 : patient.PatientID, offering == null ? -1 : offering.OfferingID,
                                bookingTypeID.Value, 0, unavailabilityReasonID.Value, GetStaffID(), booking_confirmed_by_type_id, confirmedBy, dateConfirmed, -1, DateTime.MinValue, -1, DateTime.MinValue, false, false, false, true, dayOfWeek, startTime.Value, endTime.Value);
                }
                else
                {
                    // get which dates will occur, and create individual bookings
                    DateTime curStartDate = startDateTime.Value;
                    while (curStartDate.DayOfWeek != dayOfWeek)
                        curStartDate = curStartDate.AddDays(1);

                    DateTime curStartDateTime = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, startTime.Value.Hours, startTime.Value.Minutes, 0);
                    DateTime curEndDateTime   = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, endTime.Value.Hours,   endTime.Value.Minutes,   0);
                    int weekNbr = 0;
                    while (curStartDateTime.Date <= endDateTime.Value.Date)
                    {
                        if (weekNbr % everyNWeeks.Value == 0)
                            BookingDB.Insert(curStartDateTime, curEndDateTime, org == null || bookingTypeID.Value == 342 ? 0 : org.OrganisationID, staff == null ? -1 : staff.StaffID, patient == null ? -1 : patient.PatientID, offering == null ? -1 : offering.OfferingID,
                                     bookingTypeID.Value, 0, unavailabilityReasonID.Value, GetStaffID(), booking_confirmed_by_type_id, confirmedBy, dateConfirmed, -1, DateTime.MinValue, -1, DateTime.MinValue, false, false, false, false, curStartDateTime.DayOfWeek, TimeSpan.Zero, TimeSpan.Zero);

                        curStartDateTime = curStartDateTime.AddDays(7);
                        curEndDateTime   = curEndDateTime.AddDays(7);
                        weekNbr++;
                    }
                }
            }


        }
    }

    protected void ReverseBooking(bool runIt = true)  // can use it to check if "able" to reverse a booking with argument false
    {
        UserView userView = UserView.GetInstance();

        Booking booking = UrlBooking;
        if (booking == null)
            throw new Exception("Invalid url field booking_id");

        string errorString = string.Empty;
        if (!booking.CanReverse(userView.IsAdminView, out errorString))
            throw new CustomMessageException(errorString);

        if (runIt)
            booking.Reverse(userView.IsAdminView, GetStaffID());
    }


    protected int GetStaffID()
    {
        return !UserView.GetInstance().IsPatient ? Convert.ToInt32(Session["StaffID"]) : -6;
    }


    #region Url Fields, GetUrlParamType(), GetUrlBookingScrType()

    protected Patient UrlPatient
    {
        get
        {
            try
            {
                string patient_id = Request.QueryString["patient_id"];
                if (patient_id == null || !Regex.IsMatch(patient_id, @"^\-?\d+$"))
                    return null;
                return PatientDB.GetByID(Convert.ToInt32(patient_id));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    protected Booking UrlBooking
    {
        get
        {
            string booking_id = Request.QueryString["booking_id"];
            if (booking_id == null || !Regex.IsMatch(booking_id, @"^\d+$"))
                return null;
            return BookingDB.GetByID(Convert.ToInt32(booking_id));
        }
    }
    protected int? UrlBookingTypeID
    {
        get
        {
            string booking_type_id = Request.QueryString["booking_type_id"];
            if (booking_type_id == null || (booking_type_id != "34" && booking_type_id != "36" && booking_type_id != "340" && booking_type_id != "341" && booking_type_id != "342"))
                return null;
            return Convert.ToInt32(booking_type_id);
        }
    }
    protected Organisation UrlOrg
    {
        get
        {
            string org_id = Request.QueryString["org_id"];
            if (org_id == null || !Regex.IsMatch(org_id, @"^\-?\d+$"))
                return null;
            return OrganisationDB.GetByID(Convert.ToInt32(org_id));
        }
    }
    protected Staff UrlStaff
    {
        get
        {
            string staff_id = Request.QueryString["staff_id"];
            if (staff_id == null || !Regex.IsMatch(staff_id, @"^\-?\d+$"))
                return null;
            return StaffDB.GetByID(Convert.ToInt32(staff_id));
        }
    }
    protected Offering UrlOffering
    {
        get
        {
            string offering_id = Request.QueryString["offering_id"];
            if (offering_id == null || !Regex.IsMatch(offering_id, @"^\-?\d+$"))
                return null;
            return OfferingDB.GetByID(Convert.ToInt32(offering_id));
        }
    }
    protected bool? UrlIsConfirmed
    {
        get
        {
            string is_confirmed = Request.QueryString["confirmed"];
            if (is_confirmed == null || (is_confirmed != "0" && is_confirmed != "1"))
                return null;
            return is_confirmed == "1";
        }
    }
    protected int? UrlEditReasonID
    {
        get
        {
            string edit_reason_id = Request.QueryString["edit_reason_id"];
            if (edit_reason_id == null || !Regex.IsMatch(edit_reason_id, @"^\-?\d+$"))
                return null;
            return Convert.ToInt32(edit_reason_id);
        }
    }
    protected int? UrlUnavailabilityReasonID
    {
        get
        {
            string unavailability_reason_id = Request.QueryString["unavailability_reason_id"];
            if (unavailability_reason_id == null || !Regex.IsMatch(unavailability_reason_id, @"^\-?\d+$"))
                return null;
            return Convert.ToInt32(unavailability_reason_id);
        }
    }
    protected DateTime? UrlStartDateTime
    {
        get
        {
            try
            {
                string start_datetime = Request.QueryString["start_datetime"];
                if (start_datetime == "NULL")
                    return DateTime.MinValue;
                if (start_datetime == null || !Regex.IsMatch(start_datetime, @"^\d{4}_\d{2}_\d{2}_\d{4}$"))
                    return null;
                return ConvertStringToDateTime(start_datetime);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    protected DateTime? UrlEndDateTime
    {
        get
        {
            try
            {
                string end_datetime = Request.QueryString["end_datetime"];
                if (end_datetime == "NULL")
                    return DateTime.MinValue;
                if (end_datetime == null || !Regex.IsMatch(end_datetime, @"^\d{4}_\d{2}_\d{2}_\d{4}$"))
                    return null;
                return ConvertStringToDateTime(end_datetime);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    protected TimeSpan? UrlStartTime
    {
        get
        {
            try
            {
                string start_time = Request.QueryString["start_time"];
                if (start_time == null || !Regex.IsMatch(start_time, @"^\d{4}$"))
                    return null;
                return ConvertStringToTimeSpan(start_time);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    protected TimeSpan? UrlEndTime
    {
        get
        {
            try
            {
                string end_time = Request.QueryString["end_time"];
                if (end_time == null || !Regex.IsMatch(end_time, @"^\d{4}$"))
                    return null;
                return ConvertStringToTimeSpan(end_time);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    protected string UrlBookingDays
    {
        get
        {
            string days = Request.QueryString["days"];
            if (days == null || !Regex.IsMatch(days, @"^\d{7}$"))
                return null;
            return days;
        }
    }
    protected bool? UrlCreateAsSeries
    {
        get
        {
            string create_as_series = Request.QueryString["create_as_series"];
            if (create_as_series == null || (create_as_series != "0" && create_as_series != "1"))
                return null;
            return create_as_series == "1";
        }
    }
    protected int? UrlEveryNWeeks
    {
        get
        {
            string every_n_weeks = Request.QueryString["every_n_weeks"];
            if (every_n_weeks == null || !Regex.IsMatch(every_n_weeks, @"^\d+$"))
                return null;
            return Convert.ToInt32(every_n_weeks);
        }
    }

    protected bool? UrlCheckClashAllOrgs
    {
        get
        {
            string check_clash_all_orgs = Request.QueryString["check_clash_all_orgs"];
            if (check_clash_all_orgs == null || (check_clash_all_orgs != "0" && check_clash_all_orgs != "1"))
                return null;
            return check_clash_all_orgs == "1";
        }
    }

    protected DateTime? UrlEditFullDay_OldDate
    {
        get
        {
            try
            {
                string datetime = Request.QueryString["editfullday_old_date"];
                if (datetime == null || !Regex.IsMatch(datetime, @"^\d{4}_\d{2}_\d{2}$"))
                    return null;
                return ConvertStringToDateTime(datetime + "_0000");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    protected DateTime? UrlEditFullDay_NewDate
    {
        get
        {
            try
            {
                string datetime = Request.QueryString["editfullday_new_date"];
                if (datetime == null || !Regex.IsMatch(datetime, @"^\d{4}_\d{2}_\d{2}$"))
                    return null;
                return ConvertStringToDateTime(datetime + "_0000");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    protected Organisation UrlEditFullDay_OldOrg
    {
        get
        {
            string org_id = Request.QueryString["editfullday_old_org"];
            if (org_id == null || !Regex.IsMatch(org_id, @"^\d+$"))
                return null;
            return OrganisationDB.GetByID(Convert.ToInt32(org_id));
        }
    }
    protected Organisation UrlEditFullDay_NewOrg
    {
        get
        {
            string org_id = Request.QueryString["editfullday_new_org"];
            if (org_id == null || !Regex.IsMatch(org_id, @"^\d+$"))
                return null;
            return OrganisationDB.GetByID(Convert.ToInt32(org_id));
        }
    }
    protected Staff UrlEditFullDay_OldProvider
    {
        get
        {
            string staff_id = Request.QueryString["editfullday_old_provider"];
            if (staff_id == null || !Regex.IsMatch(staff_id, @"^\d+$"))
                return null;
            return StaffDB.GetByID(Convert.ToInt32(staff_id));
        }
    }
    protected Staff UrlEditFullDay_NewProvider
    {
        get
        {
            string staff_id = Request.QueryString["editfullday_new_provider"];
            if (staff_id == null || !Regex.IsMatch(staff_id, @"^\d+$"))
                return null;
            return StaffDB.GetByID(Convert.ToInt32(staff_id));
        }
    }


    protected enum UrlParamType { Edit, EditFullDay, Add, AddRecurring, Delete, Cancel, Confirm, Unconfirm, Arrived, Unarrived, Deceased, Reverse, CanReverse, None };
    protected UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "editfullday")
            return UrlParamType.EditFullDay;
        else if (type != null && type.ToLower() == "add")
            return UrlParamType.Add;
        else if (type != null && type.ToLower() == "addrecurring")
            return UrlParamType.AddRecurring;
        else if (type != null && type.ToLower() == "delete")
            return UrlParamType.Delete;
        else if (type != null && type.ToLower() == "confirm")
            return UrlParamType.Confirm;
        else if (type != null && type.ToLower() == "unconfirm")
            return UrlParamType.Unconfirm;
        else if (type != null && type.ToLower() == "arrived")
            return UrlParamType.Arrived;
        else if (type != null && type.ToLower() == "unarrived")
            return UrlParamType.Unarrived;
        else if (type != null && type.ToLower() == "cancel")
            return UrlParamType.Cancel;
        else if (type != null && type.ToLower() == "deceased")
            return UrlParamType.Deceased;
        else if (type != null && type.ToLower() == "reverse")
            return UrlParamType.Reverse;
        else if (type != null && type.ToLower() == "canreverse")
            return UrlParamType.CanReverse;
        else
            return UrlParamType.None;
    }

    private enum UrlBookingScrType { Patient, Provider, Clinic, AgedCare, None };
    private UrlBookingScrType GetUrlBookingScrType()
    {
        string type = Request.QueryString["booking_scr_type"];
        if (type != null && type.ToLower() == "patient")
            return UrlBookingScrType.Patient;
        else if (type != null && type.ToLower() == "provider")
            return UrlBookingScrType.Provider;
        else if (type != null && type.ToLower() == "clinic")
            return UrlBookingScrType.Clinic;
        else if (type != null && type.ToLower() == "agedcare")
            return UrlBookingScrType.AgedCare;
        else
            return UrlBookingScrType.None;
    }

    private enum UrlReturnPage { Bookings, BookingsForClinic };
    private UrlReturnPage GetUrlReturnPage()
    {
        string type = Request.QueryString["return_page"];
        if (type != null && type.ToLower() == "bookingsforclinic")
            return UrlReturnPage.BookingsForClinic;
        else
            return UrlReturnPage.Bookings;
    }

    protected DateTime ConvertStringToDateTime(string strDate)
    {
        if (strDate == "NULL")
            return DateTime.MinValue;

        return new DateTime(Convert.ToInt32(strDate.Substring(0, 4)),
                            Convert.ToInt32(strDate.Substring(5, 2)),
                            Convert.ToInt32(strDate.Substring(8, 2)),
                            Convert.ToInt32(strDate.Substring(11, 2)),
                            Convert.ToInt32(strDate.Substring(13, 2)),
                            0);
    }
    protected TimeSpan ConvertStringToTimeSpan(string strTime)
    {
        return new TimeSpan(Convert.ToInt32(strTime.Substring(0, 2)),
                            Convert.ToInt32(strTime.Substring(2, 2)),
                            0);
    }

    #endregion

}