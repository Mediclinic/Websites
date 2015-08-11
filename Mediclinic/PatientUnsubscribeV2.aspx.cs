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

public partial class PatientUnsubscribeV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            HideErrorMessage();

            if (!IsPostBack)
            {
                if (IsValidFormParams())
                {
                    Tuple<string, int> formParams = GetFormParams(false);

                    Patient patient = PatientDB.GetByID(formParams.Item2, formParams.Item1);
                    if (patient == null)
                        throw new CustomMessageException("Invalid patient ID");

                    Patient.UnsubscribeAll(patient.PatientID, formParams.Item1);
                    lblResult.Text = "Apologies for the inconvenience.<br /><br />You are now unsubscribed.";
                }
                else
                    throw new CustomMessageException("Invalid Link");
            }
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

    #endregion

    #region IsValidFormID(), GetFormID()

    protected bool IsValidFormParams()
    {
        return Patient.IsValidUnsubscribeHash(Request.QueryString["id"]);
    }
    protected Tuple<string, int> GetFormParams(bool checkIsValid = true)
    {
        if (checkIsValid && !Patient.IsValidUnsubscribeHash(Request.QueryString["id"]))
            throw new Exception("Invalid Link");

        return Patient.DecodeUnsubscribeHash(Request.QueryString["id"]);
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