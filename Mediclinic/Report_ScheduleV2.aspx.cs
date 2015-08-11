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


public partial class Report_ScheduleV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, true, false);

                // don't delete, in case sends back to this page with org/prov id set, but date invalid, can still show grid of prevoius table from Session data
                //Session.Remove("sortExpression_summaryReport");
                //Session.Remove("data_summaryReport");

                SetupGUI();
                FillGrid();
            }
            else
            {
                RefillGrid();
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

    #region SetupGUI

    protected void SetupGUI()
    {
        UserView userView = UserView.GetInstance();

        chkIncAllSites.Text      = userView.IsAgedCareView ? "&nbsp;Inc Clinics" : "&nbsp;Inc Aged Care Facilities";
        chkIncAllSites.Checked   = IsValidFormIncAllSites()   ? GetFormIncAllSites(false)   : true;
        chkIncBookings.Checked   = IsValidFormIncBookings()   ? GetFormIncBookings(false)   : true;
        chkDateAcrossTop.Checked = IsValidFormDateAcrossTop() ? GetFormDateAcrossTop(false) : true;


        ddlOrgs.Style["width"] = "300px";
        ddlOrgs.Items.Clear();
        ddlOrgs.Items.Add(new ListItem("All " + (userView.IsAgedCareView ? "Facilities" : "Clinics"), (-1).ToString()));
        foreach (Organisation curOrg in OrganisationDB.GetAll(false, true, !userView.IsClinicView && !userView.IsGPView, !userView.IsAgedCareView, true, true))
            ddlOrgs.Items.Add(new ListItem(curOrg.Name, curOrg.OrganisationID.ToString()));

        ddlProviders.Style["width"] = "300px";
        ddlProviders.Items.Clear();
        ddlProviders.Items.Add(new ListItem("All Providers", (-1).ToString()));
        foreach (Staff curProv in StaffDB.GetAll())
            if (curProv.IsProvider)
                ddlProviders.Items.Add(new ListItem(curProv.Person.FullnameWithoutMiddlename, curProv.StaffID.ToString()));


        if (IsValidFormOrgID())
        {
            Organisation org = OrganisationDB.GetByID(GetFormOrgID());
            if (org != null)
                ddlOrgs.SelectedValue = org.OrganisationID.ToString();
        }

        if (!UserView.GetInstance().IsAdminView)
        {
            providerRow.Visible = false;

            Staff provider = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));
            if (provider != null)
                ddlProviders.SelectedValue = provider.StaffID.ToString();
        }
        else
        {
            if (IsValidFormProviderID())
            {
                Staff provider = StaffDB.GetByID(GetFormProviderID());
                if (provider != null)
                    ddlProviders.SelectedValue = provider.StaffID.ToString();
            }
        }

        txtStartDate.Text = IsValidFormStartDate() ? (GetFormStartDate(false) == DateTime.MinValue ? "" : GetFormStartDate(false).ToString("dd-MM-yyyy")) : DateTime.Today.ToString("dd-MM-yyyy");
        txtEndDate.Text   = IsValidFormEndDate()   ? (GetFormEndDate(false)   == DateTime.MinValue ? "" : GetFormEndDate(false).ToString("dd-MM-yyyy"))   : DateTime.Today.AddMonths(1).ToString("dd-MM-yyyy");

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";
    }

    #endregion

    #region FillGrid

    protected void FillGrid()
    {
        UserView userView = UserView.GetInstance();

        DateTime fromDate = IsValidDate(txtStartDate.Text) ? GetDate(txtStartDate.Text)                              : DateTime.MinValue;
        DateTime toDate   = IsValidDate(txtEndDate.Text)   ? GetDate(txtEndDate.Text).Add(new TimeSpan(23, 59, 59))  : DateTime.MinValue;


        if (!IsValidDate(txtStartDate.Text))
        {
            SetErrorMessage("Please Enter A Valid Start Date");
            SetGrid();
            return;
        }
        if (!IsValidDate(txtEndDate.Text))
        {
            SetErrorMessage("Please Enter A Valid End Date");
            SetGrid();
            return;
        }


        DataTable dt = BookingDB.GetReport_StaffTimetable(fromDate, toDate, chkIncBookings.Checked, userView.IsAgedCareView || chkIncAllSites.Checked, userView.IsClinicView || chkIncAllSites.Checked, Convert.ToInt32(ddlOrgs.SelectedValue), Convert.ToInt32(ddlProviders.SelectedValue));
        Session["data_summaryReport"] = dt;

        SetGrid();
    }

    protected void RefillGrid()
    {
        SetGrid();
    }

    protected void SetGrid()
    {
        if (Session["data_summaryReport"] == null)
            return;

        DataTable dt = Session["data_summaryReport"] as DataTable;


        for (int i = dt.Rows.Count - 1; i >=  0; i--)
        {
            if (dt.Rows[i][0] == DBNull.Value)
            {
                //dt.Rows[i][0] = "&nbsp;";
                dt.Rows.RemoveAt(i);
            }
        }


        if (chkDateAcrossTop.Checked)
            dt = GenerateTransposedTable(dt);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("<table class=\"table table-bordered table-striped table-grid auto_width block_center vertical_align_top\" border=\"1\"  style=\"border-collapse:collapse;\">");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.AppendLine("<tr " + (dt.Rows[i][0] == DBNull.Value || (i > 0 && dt.Rows[i][0].ToString() == "&nbsp;") ? " bgcolor=\"gray\" " : "") + ">");
            for (int j = 0; j < dt.Columns.Count; j++)
                sb.AppendLine((i == 0 ? "<th>" : "<td style=\"text-align: left !important;\">") + dt.Rows[i][j].ToString().Replace(" / ","<br />") + (i == 0 ? "</th>" : "</td>"));
            sb.AppendLine("</tr>");
        }
        sb.AppendLine("</table>");

        lblScheduleTable.Text = sb.ToString();
    }

    private DataTable GenerateTransposedTable(DataTable inputTable)
    {
        DataTable outputTable = new DataTable();

        foreach (DataRow inRow in inputTable.Rows)
        {
            string newColName = string.Empty;
            outputTable.Columns.Add(newColName);
        }

        // Add rows by looping columns        
        for (int rCount = 0; rCount < inputTable.Columns.Count; rCount++)
        {
            DataRow newRow = outputTable.NewRow();
            for (int cCount = 0; cCount < inputTable.Rows.Count; cCount++)
            {
                newRow[cCount] = inputTable.Rows[cCount][rCount].ToString();
            }
            outputTable.Rows.Add(newRow);
        }

        return outputTable;
    }

    #endregion

    #region IsValidFormStartDate(),  GetFormStartDate()....

    private bool IsValidFormPatientID()
    {
        string id = Request.QueryString["patient"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormPatientID()
    {
        if (!IsValidFormPatientID())
            throw new Exception("Invalid url patient");

        string id = Request.QueryString["patient"];
        return Convert.ToInt32(id);
    }
    private bool IsValidFormProviderID()
    {
        string id = Request.QueryString["provider"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormProviderID()
    {
        if (!IsValidFormProviderID())
            throw new Exception("Invalid url provider");

        string id = Request.QueryString["provider"];
        return Convert.ToInt32(id);
    }
    private bool IsValidFormOrgID()
    {
        string id = Request.QueryString["org"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormOrgID()
    {
        if (!IsValidFormOrgID())
            throw new Exception("Invalid url org");

        string id = Request.QueryString["org"];
        return Convert.ToInt32(id);
    }

    protected bool IsValidFormIncAllSites()
    {
        string inc_all_sites = Request.QueryString["inc_all_sites"];
        return inc_all_sites != null && (inc_all_sites == "0" || inc_all_sites == "1");
    }
    protected bool GetFormIncAllSites(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncAllSites())
            throw new Exception("Invalid url 'inc_all_sites'");
        return Request.QueryString["inc_all_sites"] == "1";
    }

    protected bool IsValidFormIncBookings()
    {
        string inc_bookings = Request.QueryString["inc_bookings"];
        return inc_bookings != null && (inc_bookings == "0" || inc_bookings == "1");
    }
    protected bool GetFormIncBookings(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncBookings())
            throw new Exception("Invalid url 'inc_all_sites'");
        return Request.QueryString["inc_bookings"] == "1";
    }

    protected bool IsValidFormDateAcrossTop()
    {
        string date_across_top = Request.QueryString["date_across_top"];
        return date_across_top != null && (date_across_top == "0" || date_across_top == "1");
    }
    protected bool GetFormDateAcrossTop(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormDateAcrossTop())
            throw new Exception("Invalid url 'date_top'");
        return Request.QueryString["date_across_top"] == "1";
    }


    protected bool IsValidDate(string strDate)
    {
        try
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(strDate, @"^\d{2}\-\d{2}\-\d{4}$"))
                return false;

            string[] parts = strDate.Split('-');
            DateTime d = new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public DateTime GetDate(string inDate)
    {
        inDate = inDate.Trim();

        if (inDate.Length == 0)
        {
            return DateTime.MinValue;
        }
        else
        {
            string[] dobParts = inDate.Split(new char[] { '-' });
            return new DateTime(Convert.ToInt32(dobParts[2]), Convert.ToInt32(dobParts[1]), Convert.ToInt32(dobParts[0]));
        }
    }


    protected bool IsValidFormStartDate()
    {
        string start_date = Request.QueryString["start_date"];
        return start_date != null && (start_date.Length == 0 || Regex.IsMatch(start_date, @"^\d{4}_\d{2}_\d{2}$"));
    }
    protected DateTime GetFormStartDate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormStartDate())
            throw new Exception("Invalid url 'start date'");
        return Request.QueryString["start_date"].Length == 0 ? DateTime.MinValue : GetDateFromString(Request.QueryString["start_date"], "yyyy_mm_dd");
    }
    protected bool IsValidFormEndDate()
    {
        string end_date = Request.QueryString["end_date"];
        return end_date != null && (end_date.Length == 0 || Regex.IsMatch(end_date, @"^\d{4}_\d{2}_\d{2}$"));
    }
    protected DateTime GetFormEndDate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormEndDate())
            throw new Exception("Invalid url 'end date'");
        return Request.QueryString["end_date"].Length == 0 ? DateTime.MinValue : GetDateFromString(Request.QueryString["end_date"], "yyyy_mm_dd");
    }
    protected DateTime GetDateFromString(string sDate, string format)
    {
        if (format == "yyyy_mm_dd")
        {
            string[] dateparts = sDate.Split('_');
            return new DateTime(Convert.ToInt32(dateparts[0]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[2]));
        }
        else if (format == "dd_mm_yyyy")
        {
            string[] dateparts = sDate.Split('_');
            return new DateTime(Convert.ToInt32(dateparts[2]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[0]));
        }
        if (format == "yyyy-mm-dd")
        {
            string[] dateparts = sDate.Split('-');
            return new DateTime(Convert.ToInt32(dateparts[0]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[2]));
        }
        else if (format == "dd-mm-yyyy")
        {
            string[] dateparts = sDate.Split('-');
            return new DateTime(Convert.ToInt32(dateparts[2]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[0]));
        }
        else
            throw new ArgumentOutOfRangeException("Unknown date format");
    }


    #endregion

    #region ddlOrgs_SelectedIndexChanged, ddlProviders_SelectedIndexChanged

    protected void ddlOrgs_SelectedIndexChanged(object sender, EventArgs e)
    {
        int newOrgID = Convert.ToInt32(ddlOrgs.SelectedValue);

        string url = Request.RawUrl;
        url = UrlParamModifier.Update(newOrgID != -1, url, "org", newOrgID == -1 ? "" : newOrgID.ToString());
        Response.Redirect(url);
    }
    protected void ddlProviders_SelectedIndexChanged(object sender, EventArgs e)
    {
        int newProvID = Convert.ToInt32(ddlProviders.SelectedValue);

        string url = Request.RawUrl;
        url = UrlParamModifier.Update(newProvID != -1, url, "provider", newProvID == -1 ? "" : newProvID.ToString());
        Response.Redirect(url);
    }

    #endregion

    #region btnSearch_Click, chkUsePaging_CheckedChanged

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        HideErrorMessage();

        if (txtStartDate.Text.Length > 0 && (!Regex.IsMatch(txtStartDate.Text, @"^\d{2}\-\d{2}\-\d{4}$") || !IsValidDate(txtStartDate.Text)))
        {
            SetErrorMessage("Start date must be empty or valid and of the format dd-mm-yyyy");
            return;
        }
        if (txtEndDate.Text.Length > 0 && (!Regex.IsMatch(txtEndDate.Text, @"^\d{2}\-\d{2}\-\d{4}$") || !IsValidDate(txtEndDate.Text)))
        {
            SetErrorMessage("End date must be empty or valid and of the format dd-mm-yyyy");
            return;
        }


        DateTime startDate = txtStartDate.Text.Length == 0 ? DateTime.MinValue : GetDateFromString(txtStartDate.Text, "dd-mm-yyyy");
        DateTime endDate = txtEndDate.Text.Length == 0 ? DateTime.MinValue : GetDateFromString(txtEndDate.Text, "dd-mm-yyyy");

        string url = ClearSearchesFromUrl(Request.RawUrl);
        url = UrlParamModifier.AddEdit(url, "start_date"      , startDate == DateTime.MinValue ? "" : startDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "end_date"        , endDate   == DateTime.MinValue ? "" : endDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "inc_all_sites"   , chkIncAllSites.Checked   ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_bookings"    , chkIncBookings.Checked   ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "date_across_top" , chkDateAcrossTop.Checked ? "1" : "0");

        Response.Redirect(url);
    }

    protected string ClearSearchesFromUrl(string url)
    {
        url = UrlParamModifier.Remove(url, "start_date");
        url = UrlParamModifier.Remove(url, "end_date");
        url = UrlParamModifier.Remove(url, "inc_all_sites");
        url = UrlParamModifier.Remove(url, "inc_bookings");
        url = UrlParamModifier.Remove(url, "date_across_top");

        return url;
    }

    #endregion

    #region btnExport_Click

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (Session["data_summaryReport"] == null)
            return;

        DataTable dt = Session["data_summaryReport"] as DataTable;

        for (int i = dt.Rows.Count - 1; i >= 0; i--)
            if (dt.Rows[i][0] == DBNull.Value)
                dt.Rows.RemoveAt(i);

        if (chkDateAcrossTop.Checked)
            dt = GenerateTransposedTable(dt);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            for (int j = 0; j < dt.Columns.Count; j++)
                sb.Append(
                    
                    "\"" + 
                    
                    (dt.Rows[i][j] == DBNull.Value ? 
                        string.Empty :
                        ((string)dt.Rows[i][j]).Replace("<font color=\"green\">", "")
                                               .Replace("<font color=\"blue\">", "")
                                               .Replace("</font>", "")
                                               .Replace("<b>", "")
                                               .Replace("</b>", "")
                                               .Replace("<br />", "\r\n")
                                               .Replace(" / ", "\r\n")
                    )

                    + "\"").Append(j < dt.Columns.Count - 1 ? "," : "");

            sb.AppendLine();
        }

        ExportCSV(Response, sb.ToString(), "Schedule.csv");
    }
    protected static void ExportCSV(HttpResponse response, string fileText, string fileName)
    {
        byte[] buffer = GetBytes(fileText);

        try
        {
            response.Clear();
            response.ContentType = "text/plain";
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            response.End();
        }
        catch (System.Web.HttpException ex) 
        {
            // ignore exception where user closed the download box
            if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                throw;
        }
    }
    protected static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
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