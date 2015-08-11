using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxCheckOfferingMaxNbrLimit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string patient_id = Request.QueryString["patient_id"];
            if (patient_id == null || !Regex.IsMatch(patient_id, @"^\d+$"))
                throw new CustomMessageException();
            Patient patient = PatientDB.GetByID(Convert.ToInt32(patient_id));
            if (patient == null)
                throw new CustomMessageException();

            string offering_id = Request.QueryString["offering_id"];
            if (offering_id == null || !Regex.IsMatch(offering_id, @"^\d+$"))
                throw new CustomMessageException();
            Offering offering = OfferingDB.GetByID(Convert.ToInt32(offering_id));
            if (offering == null)
                throw new CustomMessageException();

            string booking_datetime = Request.QueryString["booking_datetime"];
            if (booking_datetime == null || !Regex.IsMatch(booking_datetime, @"^\d{4}_\d{2}_\d{2}_\d{4}$"))
                throw new CustomMessageException();
            DateTime dateTime = ConvertStringToDateTime(booking_datetime);

            int nbrMedicareThisServiceSoFarThisPeriod = (int)InvoiceDB.GetMedicareCountByPatientAndDateRange(patient.PatientID, dateTime.Date.AddMonths(-1 * offering.MaxNbrClaimableMonths), dateTime.Date, offering.OfferingID);

            // return "[done]:[limit]"
            Response.Write(nbrMedicareThisServiceSoFarThisPeriod + ":" + offering.MaxNbrClaimable + ":" + offering.MaxNbrClaimableMonths);
        }
        catch (SessionTimedOutException)
        {
            Utilities.UnsetSessionVariables();
            Response.Write("SessionTimedOutException");
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "please contact system administrator."));
        }
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

}