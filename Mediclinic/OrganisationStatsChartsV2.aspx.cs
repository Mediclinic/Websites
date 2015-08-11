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
using System.Drawing;

public partial class OrganisationStatsChartsV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, true, false, false, true);
                SetupGUI();
                FillForm();
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

    protected void SetupGUI()
    {
        ddlType.Items.Clear();
        ddlType.Items.Add("Total Bookings");
        ddlType.Items.Add("New Bookings Added By");
        ddlType.Items.Add("New Patients");

        DateTime startOfThisMonth = DateTime.Now.AddDays((-1*DateTime.Now.Day) + 1);
        DateTime endOfThisMonth   = startOfThisMonth.AddMonths(1).AddDays(-1);

        txtStartDate.Text = startOfThisMonth.ToString("dd-MM-yyyy");
        txtEndDate.Text   = endOfThisMonth.ToString("dd-MM-yyyy");

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";

        lblHeading.InnerText = UserView.GetInstance().IsAgedCareView ? "Facilities Statistics Charts" : "Clinics Statistics Charts";
    }

    #endregion
    
    #region FillForm

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        FillForm();
    }

    protected void FillForm()
    {

        try
        {
            if (!CheckIsValidStartEndDates())
                return;

            SeriesChartType chartType = SeriesChartType.StackedBar;
            if (ddlChartType.SelectedValue == "Pie")
                chartType = SeriesChartType.Pie;
            if (ddlChartType.SelectedValue == "Doughnut")
                chartType = SeriesChartType.Doughnut;
            if (ddlChartType.SelectedValue == "StackedBar")
                chartType = SeriesChartType.StackedBar;
            if (ddlChartType.SelectedValue == "Area")
                chartType = SeriesChartType.Area;
            if (ddlChartType.SelectedValue == "BoxPlot")
                chartType = SeriesChartType.BoxPlot;
            if (ddlChartType.SelectedValue == "Column")
                chartType = SeriesChartType.Column;
            if (ddlChartType.SelectedValue == "Line")
                chartType = SeriesChartType.Line;
            if (ddlChartType.SelectedValue == "Point")
                chartType = SeriesChartType.Point;
            if (ddlChartType.SelectedValue == "Range")
                chartType = SeriesChartType.Range;
            if (ddlChartType.SelectedValue == "RangeBar")
                chartType = SeriesChartType.RangeBar;
            if (ddlChartType.SelectedValue == "RangeColumn")
                chartType = SeriesChartType.RangeColumn;

            chartOrgStats.Series["Series1"].ChartType = chartType;



            DataTable tblStats = OrganisationDB.GetStats(GetFromDate(), GetToDate(), UserView.GetInstance().IsClinicView ? 5 : 6, chkIncDeleted.Checked);

            int maxToShow = 350;
            if (tblStats.Rows.Count > maxToShow)
                for (int i = tblStats.Rows.Count - 1; i >= maxToShow; i--)
                    tblStats.Rows.RemoveAt(i);

            DataView dv = tblStats.DefaultView;

            string field = string.Empty;
            if (ddlType.SelectedValue == "New Bookings Added By")
                field = "n_bookings";
            if (ddlType.SelectedValue == "New Patients")
                field = "n_patients";
            if (ddlType.SelectedValue == "Total Bookings")
                field = "total_bookings";

            dv.Sort = field;

            chartOrgStats.Titles[0].Text = ddlType.SelectedValue;
            chartOrgStats.Series["Series1"].Points.DataBindXY(dv, "name", dv, field);

            lblChartHeading.Text = ddlType.SelectedValue;

            if (chartType == SeriesChartType.StackedBar)
                chartOrgStats.Height = new Unit((tblStats.Rows.Count * 30), UnitType.Pixel);

            chartOrgStats.ChartAreas["ChartArea1"].AxisX.MinorTickMark.Enabled = false;
            chartOrgStats.ChartAreas["ChartArea1"].AxisX.Interval = 1;
            chartOrgStats.ChartAreas["ChartArea1"].AxisX.IsLabelAutoFit = true;
            //chartOrgStats.ChartAreas["ChartArea1").AxisX.LabelStyle.IsStaggered = True
            //chartOrgStats.ChartAreas["ChartArea1"].AxisX.LabelAutoFitStyle = System.Web.UI.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont;

            chartOrgStats.ChartAreas["ChartArea1"].AxisY.Enabled = System.Web.UI.DataVisualization.Charting.AxisEnabled.False;

            if (chartType == SeriesChartType.Pie || chartType == SeriesChartType.Doughnut)
            {
                chartOrgStats.Legends.Add("Legend1");
                chartOrgStats.Legends["Legend1"].Enabled = true;
                chartOrgStats.Legends["Legend1"].Docking = System.Web.UI.DataVisualization.Charting.Docking.Right;
                chartOrgStats.Legends["Legend1"].LegendItemOrder = System.Web.UI.DataVisualization.Charting.LegendItemOrder.SameAsSeriesOrder;
                chartOrgStats.Legends["Legend1"].Font = new Font("Trebuchet MS", 10);
                
                // Add Color column
                LegendCellColumn firstColumn = new LegendCellColumn();
                firstColumn.ColumnType = LegendCellColumnType.SeriesSymbol;
                chartOrgStats.Legends["Legend1"].CellColumns.Add(firstColumn);

                // Add name cell column
                LegendCellColumn textColumn = new LegendCellColumn();
                textColumn.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
                textColumn.Text = "#LEGENDTEXT";
                textColumn.Name = "nameColumn";
                chartOrgStats.Legends["Legend1"].CellColumns.Add(textColumn);

                // Add value cell column
                LegendCellColumn valColumn = new LegendCellColumn();
                valColumn.Text = "#VAL";
                valColumn.Name = "totalColumn";
                chartOrgStats.Legends["Legend1"].CellColumns.Add(valColumn);

                // Add percent cell column
                LegendCellColumn percentColumn = new LegendCellColumn();
                percentColumn.Text = "#PERCENT";
                percentColumn.Name = "percentColumn";
                chartOrgStats.Legends["Legend1"].CellColumns.Add(percentColumn);
            }
        }
        catch (CustomMessageException cmEx)
        {
            HideTableAndSetErrorMessage(cmEx.Message);
        }
        catch (Exception ex)
        {
            HideTableAndSetErrorMessage(ex.ToString());
        }

    }

    #endregion

    #region IsValidFormID(), GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$") && StaffDB.Exists(Convert.ToInt32(id));
    }
    private int GetFormID(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormID())
            throw new Exception("Invalid url id");
        return Convert.ToInt32(Request.QueryString["id"]);
    }

    #endregion

    #region CheckIsValidStartEndDates, GetFromDate, GetToDate

    protected bool CheckIsValidStartEndDates()
    {
        try
        {
            if (txtStartDate.Text.Length > 0 && !Utilities.IsValidDate(txtStartDate.Text, "dd-mm-yyyy"))
                throw new CustomMessageException("Invalid start date");
            if (txtEndDate.Text.Length > 0 && !Utilities.IsValidDate(txtEndDate.Text, "dd-mm-yyyy"))
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