using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxHasActiveReferrer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string id = Request.QueryString["id"];
            if (id == null)
                throw new CustomMessageException("No id in url");
            if (!Regex.IsMatch(id,   @"^\d+$"))
                throw new CustomMessageException("Booking id does not exist or is not a number");

            string id_type = Request.QueryString["id_type"];
            if (id_type == null)
                throw new CustomMessageException("No id_type in url");
            if (id_type != "patient" && id_type != "healthcard")
                throw new CustomMessageException("Unknown id_type in url not in ('patient','healthcard') : '"+id_type+"'");


            int patientID = -1;
            if (id_type == "patient")
            {
                patientID = Convert.ToInt32(id);
            }
            else
            {
                HealthCard hc = HealthCardDB.GetByID(Convert.ToInt32(id));
                if (hc != null)
                    patientID = hc.Patient.PatientID;
            }

            if (!PatientDB.Exists(patientID))
                throw new CustomMessageException("Unknown patient id : " + patientID);

            bool hasReferrer = PatientReferrerDB.GetActiveEPCPatientReferrersOf(patientID).Length > 0;

            Response.Write(hasReferrer ? "1" : "0");

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