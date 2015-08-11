using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxBookingPhoneNbrs : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string booking_id = Request.QueryString["booking_id"];

            if (booking_id   == null || !Regex.IsMatch(booking_id,   @"^\d+$"))
                throw new CustomMessageException("Booking id does not exist or is not a number");

            Booking booking = BookingDB.GetByID(Convert.ToInt32(booking_id));
            if (booking == null)
                throw new CustomMessageException("Booking is null");



            Patient patient = booking.Patient;
            if (booking.Patient == null)
                Response.Write("NONE");
            else
            {
                bool is_confirmed = booking.DateConfirmed != DateTime.MinValue;

                string ret = string.Empty;
                ret += booking.Patient.Person.FullnameWithoutMiddlename + "::";
                ret += (is_confirmed ? "1" : "0") + "::";


                if (Utilities.GetAddressType().ToString() == "Contact")
                {
                    Contact[] phNums = ContactDB.GetByEntityID(2, booking.Patient.Person.EntityID);
                    for (int i = 0; i < phNums.Length; i++)
                    {
                        if (i > 0)
                            ret += ",";
                        ret += phNums[i].AddrLine1;
                    }
                }
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                {
                    ContactAus[] phNums = ContactAusDB.GetByEntityID(2, booking.Patient.Person.EntityID);
                    for (int i = 0; i < phNums.Length; i++)
                    {
                        if (i > 0)
                            ret += ",";
                        ret += phNums[i].AddrLine1;
                    }
                }
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

                Response.Write(ret);
            }
            
        }
        catch (SessionTimedOutException)
        {
            Utilities.UnsetSessionVariables();
            Response.Write("SessionTimedOutException");
        }
        catch (Exception ex)
        {
            Response.Write((Utilities.IsDev() ? "Exception: " + ex.ToString() : "Error - please contact system administrator."));
        }
    }
}