using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxBookingCheckFutureBookings : System.Web.UI.Page
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

            string type     = Request.QueryString["type"];
            string id       = Request.QueryString["staff"];
            string weekday  = Request.QueryString["weekday"];
            string start_time = Request.QueryString["start_time"];
            string end_time = Request.QueryString["end_time"];
            string outside  = Request.QueryString["outside"];

            if (type       == null || (type != "provider" && type != "org")  ||
                id         == null || (id != "-1" && !Regex.IsMatch(id, @"^\d+$"))    ||
                weekday    == null || (weekday != "monday" && weekday != "tuesday" && weekday != "wednesday" && weekday != "thursday" && weekday != "friday" && weekday != "saturday" && weekday != "sunday") ||
                start_time == null || !Regex.IsMatch(start_time, @"^\d{4}$") ||
                end_time   == null || !Regex.IsMatch(end_time, @"^\d{4}$")   ||
                outside    == null || (outside != "0" && outside != "1"))
                throw new CustomMessageException();


            Organisation org     = type == "org" ? OrganisationDB.GetByID(Convert.ToInt32(id)) : null;
            Staff        staff   = type == "provider" ? StaffDB.GetByID(Convert.ToInt32(id)) : null;

            if ((type == "provider" && staff == null) ||
                (type == "org"     && org == null))
                throw new CustomMessageException();

            Booking[] bookings = BookingDB.GetFutureBookings(staff, org, weekday, ConvertToTimeSpan(start_time), ConvertToTimeSpan(end_time), Request.QueryString["outside"] == "1");
            string bookingDates = string.Empty;
            for (int i = 0; i < bookings.Length; i++)
                bookingDates += bookings[i].DateStart.ToString(@"ddd MMM d yyy HH:mm") + ",";

            if (bookingDates.Length > 0)
                Response.Write(bookingDates.Substring(0, bookingDates.Length - 1));
            else
                Response.Write("NONE");
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

    protected TimeSpan ConvertToTimeSpan(string time)
    {
        return new TimeSpan(Convert.ToInt32(time.Substring(0, 2)), Convert.ToInt32(time.Substring(2, 2)), 0);
    }

    #endregion

}