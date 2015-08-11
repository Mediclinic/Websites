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

public partial class EzidebitInfoV2 : System.Web.UI.Page
{


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, false, false, false, false);
            SetupGUI();
        }

        lblErrorMessage.Text = string.Empty;
        Run();
    }

    protected void SetupGUI()
    {
        DateTime start = DateTime.Now.AddDays(-21);
        DateTime end   = DateTime.Now.AddDays(1);

        txtStartDate.Text = start.ToString("dd-MM-yyyy");
        txtEndDate.Text   = end.ToString("dd-MM-yyyy");

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Run();
    }

    protected void Run()
    {
        if (!CheckIsValidStartEndDates())
            return;

        try
        {
            string output = PaymentPendingDB.UpdateAllPaymentsPending(null, GetFromDate(), GetToDate(), Convert.ToInt32(Session["StaffID"]), true);
            lblOutput.Text = output;
        }
        catch (System.ServiceModel.CommunicationException ex)
        {
            if (ex.Message == "The maximum message size quota for incoming messages (65536) has been exceeded. To increase the quota, use the MaxReceivedMessageSize property on the appropriate binding element.")
            {
                SetErrorMessage("Ezidebit can not return this much data in a single request. Please narrow the date range and try again.");
                return;
            }
            throw;
        }
    }


    #region CheckIsValidStartEndDates, GetFromDate, GetToDate

    protected bool CheckIsValidStartEndDates()
    {
        try
        {
            if (!Utilities.IsValidDate(txtStartDate.Text, "dd-mm-yyyy"))
                throw new CustomMessageException("Invalid start date");
            if (!Utilities.IsValidDate(txtEndDate.Text, "dd-mm-yyyy"))
                throw new CustomMessageException("Invalid end date");

            return true;
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return false;
        }
    }
    protected DateTime GetFromDate()
    {
        return txtStartDate.Text.Length > 0 ? Utilities.GetDate(txtStartDate.Text, "dd-mm-yyyy") : DateTime.MinValue;
    }
    protected DateTime GetToDate()
    {
        return txtEndDate.Text.Length > 0 ? Utilities.GetDate(txtEndDate.Text, "dd-mm-yyyy").Add(new TimeSpan(23, 59, 59)) : DateTime.MinValue;
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