using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxBookingCheckClash : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            BookingOcurrenceType bookingRecurrenceType = GetBookingOcurrenceType();
            if (bookingRecurrenceType == BookingOcurrenceType.OneTime)
                CheckClashOneTimeBooking();
            else if (bookingRecurrenceType == BookingOcurrenceType.Recurring)
                CheckClashRecurringBooking();
            else if (bookingRecurrenceType == BookingOcurrenceType.FullDay)
                CheckClashFullDayBookings();
            else
                throw new CustomMessageException("Unknown occurrence type");
        }
        catch (SessionTimedOutException)
        {
            Utilities.UnsetSessionVariables();
            Response.Write("SessionTimedOutException");
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "Error - please contact system administrator."));
        }
    }

    #endregion

    #region CheckClashOneTimeBooking, CheckClashRecurringBooking

    protected void CheckClashOneTimeBooking()
    {
        UserView userView = UserView.GetInstance();

        string org_id          = Request.QueryString["org"];
        string all_orgs        = Request.QueryString["all_orgs"];
        string staff_id        = Request.QueryString["staff"];
        string booking_id      = Request.QueryString["edit_booking_id"];
        string booking_type_id = Request.QueryString["booking_type_id"];

        string start_datetime  = Request.QueryString["start_datetime"];
        string end_datetime    = Request.QueryString["end_datetime"];


        if (start_datetime == null || !Regex.IsMatch(start_datetime, @"^\d{4}_\d{2}_\d{2}_\d{4}$") ||
            end_datetime   == null || !Regex.IsMatch(end_datetime,   @"^\d{4}_\d{2}_\d{2}_\d{4}$") ||
            org_id         == null || !Regex.IsMatch(org_id,         @"^\-?\d+$") ||
            staff_id       == null || !Regex.IsMatch(staff_id,       @"^\-?\d+$") ||
            booking_id     == null || !Regex.IsMatch(booking_id,     @"^\-?\d+$"))
            throw new CustomMessageException();

        Organisation org = OrganisationDB.GetByID(Convert.ToInt32(org_id));
        Staff staff = StaffDB.GetByID(Convert.ToInt32(staff_id));
        Booking booking = booking_id == "-1" ? null : BookingDB.GetByID(Convert.ToInt32(booking_id));

        if (booking != null && booking_type_id == "-1")
            booking_type_id = booking.BookingTypeID.ToString();

        if ((org_id != "0"      && org     == null) || 
            (staff_id != "-1"   && staff   == null) || 
            (booking_id != "-1" && booking == null) ||
            (booking_type_id == null || (booking_type_id != "34" && booking_type_id != "340" && booking_type_id != "341" && booking_type_id != "342")) )
            throw new CustomMessageException();

        DateTime startDateTime = ConvertStringToDateTime(start_datetime);
        DateTime endDateTime   = ConvertStringToDateTime(end_datetime);

        // if pt logged in, disallow booking over unavailabilities, otherwise allow it and ignore the check
        bool checkUnavailableDays = userView.IsPatient;

        Booking[] bookings = (all_orgs != null && all_orgs == "1") ?
            BookingDB.GetToCheckOverlap_OneTime(startDateTime, endDateTime, staff, null, booking_type_id == "342", true, checkUnavailableDays) :
            BookingDB.GetToCheckOverlap_OneTime(startDateTime, endDateTime, staff, org , booking_type_id == "342", true, checkUnavailableDays);

        if (bookings.Length > 0)
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            for (int i = 0; i < bookings.Length; i++)
            {
                if ((bookings[i].Organisation == null || (org != null && bookings[i].Organisation.OrganisationID == org.OrganisationID)) &&
                    (bookings[i].Provider     == null || bookings[i].Provider.StaffID == staff.StaffID))
                        list.Add(bookings[i]);
            }
            bookings = (Booking[])list.ToArray(typeof(Booking));
        }

        // for patient, disallow booking over start/end times when clinic unavailable
        if (userView.IsPatient)
        {
            if (org == null) org = booking.Organisation;
            StartEndTime startEndTime      = org.GetStartEndTime(startDateTime.DayOfWeek);
            StartEndTime startEndLunchTime = org.GetStartEndLunchTime(startDateTime.DayOfWeek);
            if ((startEndTime.StartTime      < startEndTime.EndTime      && startDateTime.TimeOfDay < endDateTime.TimeOfDay && !(startDateTime.TimeOfDay >= startEndTime.StartTime && endDateTime.TimeOfDay <= startEndTime.EndTime)) ||
                (startEndLunchTime.StartTime < startEndLunchTime.EndTime && startDateTime.TimeOfDay < endDateTime.TimeOfDay &&  Booking.TimeIntersects(startEndLunchTime.StartTime, startEndLunchTime.EndTime, startDateTime.TimeOfDay, endDateTime.TimeOfDay)))
            {
                Response.Write("1");
                return;
            }
        }


        Response.Write(Booking.HasOverlap(bookings, startDateTime, endDateTime, booking) ? "1" : "0");
    }

    protected void CheckClashRecurringBooking()
    {
        string org_id          = Request.QueryString["org"];
        string staff_id        = Request.QueryString["staff"];
        string booking_id      = Request.QueryString["edit_booking_id"];
        string booking_type_id = Request.QueryString["booking_type_id"];

        string start_datetime  = Request.QueryString["start_datetime"];
        string end_datetime    = Request.QueryString["end_datetime"];

        string days            = Request.QueryString["days"];
        string start_time      = Request.QueryString["start_time"];
        string end_time        = Request.QueryString["end_time"];


        if (org_id         == null || !Regex.IsMatch(org_id,         @"^\-?\d+$") ||
            staff_id       == null || !Regex.IsMatch(staff_id,       @"^\-?\d+$") ||
            booking_id     == null || !Regex.IsMatch(booking_id,     @"^\-?\d+$") ||
            start_datetime == null || !Regex.IsMatch(start_datetime, @"^\d{4}_\d{2}_\d{2}_\d{4}$") ||
            end_datetime   == null || (end_datetime != "NULL" && !Regex.IsMatch(end_datetime,   @"^\d{4}_\d{2}_\d{2}_\d{4}$")) ||
            days           == null || !Regex.IsMatch(days,           @"^\d{7}$") ||
            start_time     == null || !Regex.IsMatch(start_time,     @"^\d{4}$") ||
            end_time       == null || !Regex.IsMatch(end_time,       @"^\d{4}$"))
            throw new CustomMessageException();

        Organisation org     = OrganisationDB.GetByID(Convert.ToInt32(org_id));
        Staff        staff   = StaffDB.GetByID(Convert.ToInt32(staff_id));
        Booking      booking = booking_id == "-1" ? null : BookingDB.GetByID(Convert.ToInt32(booking_id));

        if (booking != null && booking_type_id == "-1")
            booking_type_id = booking.BookingTypeID.ToString();

        if ((org_id != "0"      && org     == null) || 
            (staff_id != "-1"   && staff   == null) || 
            (booking_id != "-1" && booking == null) ||
            (booking_type_id == null || (booking_type_id != "34" && booking_type_id != "340" && booking_type_id != "341" && booking_type_id != "342")) )
            throw new CustomMessageException();

        DateTime startDateTime = ConvertStringToDateTime(start_datetime);
        DateTime endDateTime   = ConvertStringToDateTime(end_datetime);

        TimeSpan startTime     = ConvertStringToTimeSpan(start_time);
        TimeSpan endTime       = ConvertStringToTimeSpan(end_time);
        if (endTime == new TimeSpan(23, 59, 0))
            endTime = new TimeSpan(0, 23, 59, 59, 999);

        //Response.Write("0"); return;
        Booking[] bookings = BookingDB.GetToCheckOverlap_Recurring(startDateTime, endDateTime, startTime, endTime, days, staff, org, booking_type_id == "342", true, false, true);
        Booking clash = Booking.HasOverlap(bookings, startDateTime, endDateTime, days, startTime, endTime, 1, booking);
        Response.Write(clash == null ? "0" : "1");
    }

    protected void CheckClashFullDayBookings()
    {
        string old_edit_date        = Request.QueryString["editfullday_old_date"];
        string old_edit_org_id      = Request.QueryString["editfullday_old_org"];
        string old_edit_provider_id = Request.QueryString["editfullday_old_provider"];
        string new_edit_date        = Request.QueryString["editfullday_new_date"];
        string new_edit_org_id      = Request.QueryString["editfullday_new_org"];
        string new_edit_provider_id = Request.QueryString["editfullday_new_provider"];

        if (old_edit_date        == null || !Regex.IsMatch(old_edit_date,        @"^\d{4}_\d{2}_\d{2}$") ||
            new_edit_date        == null || !Regex.IsMatch(new_edit_date,        @"^\d{4}_\d{2}_\d{2}$") ||
            old_edit_org_id      == null || !Regex.IsMatch(old_edit_org_id,      @"^\d+$") ||
            new_edit_org_id      == null || !Regex.IsMatch(new_edit_org_id,      @"^\d+$") ||
            old_edit_provider_id == null || !Regex.IsMatch(old_edit_provider_id, @"^\d+$") ||
            new_edit_provider_id == null || !Regex.IsMatch(new_edit_provider_id, @"^\d+$"))
            throw new CustomMessageException();


        DateTime oldEditDateTime = ConvertStringToDateTime(old_edit_date + "_0000");
        DateTime newEditDateTime = ConvertStringToDateTime(new_edit_date + "_0000");

        Organisation oldOrg   = OrganisationDB.GetByID(Convert.ToInt32(old_edit_org_id));
        Organisation newOrg   = OrganisationDB.GetByID(Convert.ToInt32(new_edit_org_id));
        Staff        oldStaff = StaffDB.GetByID(Convert.ToInt32(old_edit_provider_id));
        Staff        newStaff = StaffDB.GetByID(Convert.ToInt32(new_edit_provider_id));

        if (oldOrg   == null ||
            newOrg   == null ||
            oldStaff == null ||
            newStaff == null)
            throw new CustomMessageException();



        // get all bookings from that day/staff/org
        // check each for clash

        bool hasClash = false;
        Booking[] daysBookingList = BookingDB.GetBetween(oldEditDateTime, oldEditDateTime.AddDays(1), new Staff[] { oldStaff }, new Organisation[] { oldOrg }, null, null, false, "0", false);

        // remove any 34 types not for this org
        System.Collections.ArrayList tmp = new System.Collections.ArrayList();
        foreach (Booking booking in daysBookingList)
            if (booking.BookingTypeID != 34 || (booking.Organisation.OrganisationID == oldOrg.OrganisationID && booking.Provider.StaffID == oldStaff.StaffID))
                tmp.Add(booking);
        daysBookingList = (Booking[])tmp.ToArray(typeof(Booking));

        foreach (Booking booking in daysBookingList)
        {
            DateTime newDateStart = newEditDateTime.AddHours(booking.DateStart.Hour).AddMinutes(booking.DateStart.Minute).AddSeconds(booking.DateStart.Second);
            DateTime newDateEnd   = newEditDateTime.AddHours(booking.DateEnd.Hour).AddMinutes(booking.DateEnd.Minute).AddSeconds(booking.DateEnd.Second);

            Booking[] bookings = BookingDB.GetToCheckOverlap_OneTime(newDateStart, newDateEnd, newStaff, null, true, true, true, new Booking[]{ booking} );

            // remove any non 34 types (unavailabilities) for other orgs
            tmp = new System.Collections.ArrayList();
            foreach (Booking bk in bookings)
                if (bk.Organisation.OrganisationID == oldOrg.OrganisationID || bk.BookingTypeID == 34)
                    tmp.Add(bk);
            bookings = (Booking[])tmp.ToArray(typeof(Booking));

            if (Booking.HasOverlap(bookings, newDateStart, newDateEnd, booking))
            {
                hasClash = true;
                break;
            }
        }

        Response.Write(hasClash ? "1" : "0");
    }


    #endregion

    #region ConvertStringToDateTime, ConvertStringToTimeSpan

    protected DateTime ConvertStringToDateTime(string strDate)
    {
        if (strDate == "NULL")
            return DateTime.MinValue;

        return new DateTime(Convert.ToInt32(strDate.Substring(0,  4)), 
                            Convert.ToInt32(strDate.Substring(5,  2)), 
                            Convert.ToInt32(strDate.Substring(8,  2)), 
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

    #region GetUrlTypes

    protected enum BookingOcurrenceType { OneTime, Recurring, FullDay, None };
    protected BookingOcurrenceType GetBookingOcurrenceType()
    {
        string is_recurring = Request.QueryString["ocurrence"];
        if (is_recurring != null && is_recurring == "recurring")
            return BookingOcurrenceType.Recurring;
        if (is_recurring != null && is_recurring == "onetime")
            return BookingOcurrenceType.OneTime;
        if (is_recurring != null && is_recurring == "fullday")
            return BookingOcurrenceType.FullDay;
        else
            return BookingOcurrenceType.None;
    }

    #endregion

}