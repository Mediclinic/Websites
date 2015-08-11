using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxBookingGetClashes : System.Web.UI.Page
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
                GetClashOneTimeBooking();
            else if (bookingRecurrenceType == BookingOcurrenceType.Recurring)
                GetClashRecurringBooking();
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

    #region GetClashOneTimeBooking, GetClashRecurringBooking

    protected static string colSep = "|";
    protected static string rowSep = "<>";


    protected void GetClashOneTimeBooking()
    {
        string org_id          = Request.QueryString["org"];
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

        Booking[] bookings = BookingDB.GetToCheckOverlap_OneTime(startDateTime, endDateTime, staff, org, booking_type_id == "342", true, false);
        Booking[] overlappingBookings = Booking.GetOverlappingBookings(bookings, startDateTime, endDateTime, booking);
        Response.Write(GetLinks(overlappingBookings));
    }

    protected void GetClashRecurringBooking()
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

        string every_n_weeks   = Request.QueryString["every_n_weeks"];

        string inc_unavailable_bkgs = Request.QueryString["inc_unavailable_bkgs"];
        string inc_customer_bkgs = Request.QueryString["inc_customer_bkgs"];


        if (org_id               == null || !Regex.IsMatch(org_id,         @"^\-?\d+$") ||
            staff_id             == null || !Regex.IsMatch(staff_id,       @"^\-?\d+$") ||
            booking_id           == null || !Regex.IsMatch(booking_id,     @"^\-?\d+$") ||
            start_datetime       == null || !Regex.IsMatch(start_datetime, @"^\d{4}_\d{2}_\d{2}_\d{4}$") ||
            end_datetime         == null || (end_datetime != "NULL" && !Regex.IsMatch(end_datetime,   @"^\d{4}_\d{2}_\d{2}_\d{4}$")) ||
            days                 == null || !Regex.IsMatch(days,           @"^\d{7}$") ||
            start_time           == null || !Regex.IsMatch(start_time,     @"^\d{4}$") ||
            end_time             == null || !Regex.IsMatch(end_time,       @"^\d{4}$") ||
            every_n_weeks        == null || !Regex.IsMatch(every_n_weeks,  @"^\d+$")   ||
            inc_unavailable_bkgs == null || (inc_unavailable_bkgs != "0" && inc_unavailable_bkgs != "1") ||
            inc_customer_bkgs    == null || (inc_customer_bkgs    != "0" && inc_customer_bkgs    != "1"))
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

        int  everyNWeeks        = Convert.ToInt32(every_n_weeks);
        bool incUnavailableBkgs = inc_unavailable_bkgs == "1";
        bool incCustomerBkgs    = inc_customer_bkgs    == "1";


        bool onlyThisOrg = booking_type_id == "342" || (org != null && booking_type_id == "341") || (incUnavailableBkgs && !incCustomerBkgs);

        Booking[] bookings = BookingDB.GetToCheckOverlap_Recurring(startDateTime, endDateTime, startTime, endTime, days, staff, org, onlyThisOrg, true, incUnavailableBkgs, incCustomerBkgs);
        Booking[] overlappingBookings = Booking.GetOverlappingBookings(bookings, startDateTime, endDateTime, days, startTime, endTime, everyNWeeks, booking);
        Response.Write(GetLinks(overlappingBookings));
    }

    protected string GetLinks(Booking[] overlappingBookings)
    {
        string bookingLinks = string.Empty;
        for (int i = 0; i < overlappingBookings.Length; i++)
            bookingLinks += (bookingLinks.Length == 0 ? "" : rowSep) + GetLink(overlappingBookings[i]);
        return bookingLinks;
    }
    protected string GetLink(Booking booking)
    {
        string linkText = string.Empty;


        if (booking.BookingTypeID == 34)
            linkText = booking.DateStart.ToString(@"dd MMM yyyy H:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + " : " + booking.Patient.Person.FullnameWithoutMiddlename;
        else if (!booking.IsRecurring)
        {
            bool isFullDay = booking.DateStart.Hour == 0 && booking.DateEnd.Hour == 23;
            linkText = isFullDay ?
                booking.DateStart.ToString(@"dd MMM yyyy") + " : " + " Blockout Full Day" :
                booking.DateStart.ToString(@"dd MMM yyyy H:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + " : " + " Blockout";
        }
        else
        {
            DateTime dateStart = booking.DateStart;
            while (dateStart.DayOfWeek != booking.RecurringDayOfWeek)
                dateStart = dateStart.AddDays(1);
            DateTime dateEnd = booking.DateEnd;
            while (dateEnd != DateTime.MinValue && dateEnd.DayOfWeek != booking.RecurringDayOfWeek)
                dateEnd = dateEnd.AddDays(-1);


            string dateText = dateStart.ToString(@"dd'/'MM'/'yy") + " " + (dateEnd == DateTime.MinValue ? "Onwards" : "- " + dateEnd.ToString(@"dd'/'MM'/'yy"));

            bool isFullDay = booking.RecurringStartTime.Hours == 0 && booking.RecurringEndTime.Hours == 23;
            linkText = isFullDay ?
                dateText + " : " + " Series Blockout" :
                dateText + " " + booking.RecurringStartTime.Hours + ":" + booking.RecurringStartTime.Minutes.ToString().PadLeft(2, '0') + (booking.RecurringStartTime.TotalHours < 12 ? "am" : "pm") + " : " + " Series Blockout";
        }




        string href = booking.GetBookingSheetLinkV2(true);
        if (href.StartsWith("~/")) href = href.Substring(2);
        string allFeatures = "dialogWidth:1500px;dialogHeight:1000px;center:yes;resizable:no; scroll:no";
        string js = "javascript:open_new_tab('" + href + "');return false;";
        string link = "<a href=\"#\" onclick=\"" + js + "\">" + linkText + "</a>";
        return link;
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

    protected enum BookingOcurrenceType { OneTime, Recurring, None };
    protected BookingOcurrenceType GetBookingOcurrenceType()
    {
        string is_recurring = Request.QueryString["ocurrence"];
        if (is_recurring != null && is_recurring == "recurring")
            return BookingOcurrenceType.Recurring;
        if (is_recurring != null && is_recurring == "onetime")
            return BookingOcurrenceType.OneTime;
        else
            return BookingOcurrenceType.None;
    }

    #endregion

}