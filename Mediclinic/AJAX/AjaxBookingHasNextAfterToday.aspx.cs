using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxBookingHasNextAfterToday : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string type = Request.QueryString["type"];
            if (type == null)
                throw new CustomMessageException("No type in url");
            if (type != "booking" && type != "patient")
                throw new CustomMessageException("Parameter 'type' unknown: " + type);

            string inc_completed = Request.QueryString["inc_completed"];
            if (inc_completed == null)
                throw new CustomMessageException("No inc_completed in url");
            if (inc_completed != "1" && inc_completed != "0")
                throw new CustomMessageException("Parameter 'inc_completed' unknown: " + inc_completed);


            Patient patient = null;

            if (type == "booking")
            {
                string booking_id = Request.QueryString["booking_id"];
                if (booking_id == null)
                    throw new CustomMessageException("No booking_id in url");
                if (!Regex.IsMatch(booking_id, @"^\d+$"))
                    throw new CustomMessageException("Booking id is not a number");
                Booking booking = BookingDB.GetByID(Convert.ToInt32(booking_id));
                if (booking == null)
                    throw new CustomMessageException("Booking id does not exist");

                if (booking.Organisation.OrganisationType.OrganisationTypeID != 218) // aged care - doesn't need to check this
                {
                    Response.Write("1");
                    return;
                }

                patient = booking.Patient;
                if (patient == null)
                    throw new CustomMessageException("No patient set for booking ");
            }
            else if (type == "patient")
            {
                string patient_id = Request.QueryString["patient_id"];
                if (patient_id == null)
                    throw new CustomMessageException("No patient_id in url");
                if (!Regex.IsMatch(patient_id, @"^\d+$"))
                    throw new CustomMessageException("Patient id is not a number");
                patient = PatientDB.GetByID(Convert.ToInt32(patient_id));
                if (patient == null)
                    throw new CustomMessageException("Patient ID does not exist");
            }


            Booking nextBooking = BookingDB.GetNextAfterToday(patient.PatientID, inc_completed == "1");

            Response.Write(nextBooking == null ? "0" : "1");

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
}