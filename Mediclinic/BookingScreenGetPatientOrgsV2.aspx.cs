using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections;

public partial class BookingScreenGetPatientOrgsV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            UserView userView = UserView.GetInstance();

            string patient_id = Request.QueryString["patient_id"];
            if (patient_id == null || !Regex.IsMatch(patient_id, @"^\d+$"))
                throw new CustomMessageException();

            Patient patient = PatientDB.GetByID(Convert.ToInt32(patient_id));
            if (patient == null)
                throw new CustomMessageException();

            Organisation[] orgs = RegisterPatientDB.GetOrganisationsOf(patient.PatientID);

            ArrayList list = new ArrayList();
            for(int i=0; i<orgs.Length; i++)
            {
                if (userView.IsClinicView && orgs[i].OrganisationType.OrganisationTypeID == 218)
                    list.Add(orgs[i]);
                if (userView.IsAgedCareView && (new List<int> { 139, 367, 372 }).Contains(orgs[i].OrganisationType.OrganisationTypeID))
                    list.Add(orgs[i]);
            }
            orgs = (Organisation[])list.ToArray(typeof(Organisation));


            if (orgs.Length == 0)
            {
                Response.Redirect("~/RegisterOrganisationsToPatientV2.aspx?id=" + patient.PatientID + "&type=select_to_go_to_bookings");
                return;
            }
            else
            {
                string strOrgs = string.Empty;
                foreach (Organisation o in orgs)
                    strOrgs = strOrgs + (strOrgs.Length == 0 ? "" : "_") + o.OrganisationID;

                Response.Redirect("~/BookingsV2.aspx?orgs=" + strOrgs + (userView.IsAgedCareView ? "" : "&patient=" + patient.PatientID));
                return;
            }
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "Error - please contact system administrator."));
        }
    }

    #endregion


}
