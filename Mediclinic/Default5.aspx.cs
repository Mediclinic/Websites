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

public partial class _Default5 : System.Web.UI.Page
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

            DataTable tblStats = StaffDB.GetStats2(
                DateTime.Today.AddDays(-30), 
                DateTime.Today.AddDays(0),
                false, false);

            DataView dv = tblStats.DefaultView;
            dv.Sort = "n_bookings ASC";


            cTestChart2.Series["Series1"].Points.DataBindXY(dv, "firstname", dv, "n_bookings");

            cTestChart2.BackColor = System.Drawing.Color.Transparent;
            cTestChart2.BorderlineColor = System.Drawing.Color.Transparent;

            cTestChart2.ChartAreas["ChartArea1"].AxisX.MinorTickMark.Enabled = false;
            cTestChart2.ChartAreas["ChartArea1"].AxisX.Interval = 1;
            cTestChart2.ChartAreas["ChartArea1"].AxisX.IsLabelAutoFit = true;
            //cht.ChartAreas["ChartArea1").AxisX.LabelStyle.IsStaggered = True
            //cTestChart2.ChartAreas["ChartArea1"].AxisX.LabelAutoFitStyle = System.Web.UI.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont;

            //cTestChart2.ChartAreas["ChartArea1"].AxisY.LabelStyle.Enabled = false;
            cTestChart2.ChartAreas["ChartArea1"].AxisY.Enabled = System.Web.UI.DataVisualization.Charting.AxisEnabled.False;

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