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
using System.Web.UI.DataVisualization.Charting;

public partial class _DefaultV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
                main_content.Style["background"] = (Session["SystemVariables"] == null || ((SystemVariables)Session["SystemVariables"])["MainLogoBackground"] == null) ? "url(../imagesV2/login_bg.png) center top no-repeat #EDEDED" : "url(../imagesV2/" + ((SystemVariables)Session["SystemVariables"])["MainLogoBackground"].Value + ") center top no-repeat #EDEDED";

            if (Session["StaffFirstname"] != null)
                lblStaffName.Text = Session["StaffFirstname"].ToString();

            if (Session["SiteName"] != null)
                lblSiteName.Text = Session["SiteName"].ToString();


            //lblTest.Text = PersonDB.GetFields("patient_person_", "patient_person");

            string s;
            string output = "<table>";
            s = "123456";
            output += "<tr><td>" + s.Replace(" ", "&nbsp;") + "</td><td>" + Regex.Replace(s, "[^0-9]", "").Replace(" ", "&nbsp;") + "</td><tr>" + Environment.NewLine;
            s = "s123456";
            output += "<tr><td>" + s.Replace(" ", "&nbsp;") + "</td><td>" + Regex.Replace(s, "[^0-9]", "").Replace(" ", "&nbsp;") + "</td><tr>" + Environment.NewLine;
            s = " s 123456";
            output += "<tr><td>" + s.Replace(" ", "&nbsp;") + "</td><td>" + Regex.Replace(s, "[^0-9]", "").Replace(" ", "&nbsp;") + "</td><tr>" + Environment.NewLine;
            s = "12    345 6";
            output += "<tr><td>" + s.Replace(" ", "&nbsp;") + "</td><td>" + Regex.Replace(s, "[^0-9]", "").Replace(" ", "&nbsp;") + "</td><tr>" + Environment.NewLine;
            s = "1 2 3 4 5 6";
            output += "<tr><td>" + s.Replace(" ", "&nbsp;") + "</td><td>" + Regex.Replace(s, "[^0-9]", "").Replace(" ", "&nbsp;") + "</td><tr>" + Environment.NewLine;
            s = "1m2m3m4m5m6";
            output += "<tr><td>" + s.Replace(" ", "&nbsp;") + "</td><td>" + Regex.Replace(s, "[^0-9]", "").Replace(" ", "&nbsp;") + "</td><tr>" + Environment.NewLine;
            s = "1m2 m3 m4m 5m6";
            output += "<tr><td>" + s.Replace(" ", "&nbsp;") + "</td><td>" + Regex.Replace(s, "[^0-9]", "").Replace(" ", "&nbsp;") + "</td><tr>" + Environment.NewLine;
            s = "...'123456";
            output += "<tr><td>" + s.Replace(" ", "&nbsp;") + "</td><td>" + Regex.Replace(s, "[^0-9]", "").Replace(" ", "&nbsp;") + "</td><tr>" + Environment.NewLine;
            output += "</table>";

            //int[] invoiceIDs = new int[] {  };

            //Letter.GenerateInvoicesToPrint(

            //lblTest.Text = output;

            //Letter.GenerateAllInvoicesAndTypes(new int[] { 332453, 332455, 332456, 332457, 332041 }, Response);
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