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

public partial class Notifications_SMSCreditV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);
                SetupGUI();
            }

            this.GrdSMSCredit.EnableViewState = true;

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
        txtStartDate.Text = IsValidFormStartDate() ? (GetFormStartDate(false) == DateTime.MinValue ? "" : GetFormStartDate(false).ToString("dd-MM-yyyy")) : "";
        txtEndDate.Text   = IsValidFormEndDate()   ? (GetFormEndDate(false)   == DateTime.MinValue ? "" : GetFormEndDate(false).ToString("dd-MM-yyyy"))   : "";

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";


        if (!Convert.ToBoolean(Session["IsStakeholder"]))
        {
            hideFotter = true;
            GrdSMSCredit.Columns[GrdSMSCredit.Columns.Count - 1].Visible = false;
            GrdSMSCredit.Columns[GrdSMSCredit.Columns.Count - 2].Visible = false;
        }

        SetSMSPriceTextBox(false);

        // disallow updating of sms price if not a stakeholder, but disguise it as as text instead of a textbox
        btnSMSPriceSetEditMode.Visible    = Convert.ToBoolean(Session["IsStakeholder"]);
        if (!Convert.ToBoolean(Session["IsStakeholder"]))
        {
            txtSMSPrice.BackColor = System.Drawing.Color.Transparent;
            txtSMSPrice.ForeColor = System.Drawing.Color.Black;
            txtSMSPrice.BorderStyle = BorderStyle.None;
        }

        SetNotificationInfo();

        Session.Remove("sortExpression_sms_credit");
        Session.Remove("data_sms_credit");
        FillGrid();    
    }

    #region IsValidFormStartDate(),  GetFormStartDate()....

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

    #region btnSearch_Click, chkUsePaging_CheckedChanged

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        HideErrorMessage();

        if (txtStartDate.Text.Length > 0 && (!Regex.IsMatch(txtStartDate.Text, @"^\d{2}\-\d{2}\-\d{4}$") || !IsValidDate(txtStartDate.Text)))
        {
            SetErrorMessage("Start date must be empty or valid and of the format dd-mm-yyyy<br />");
            return;
        }
        if (txtEndDate.Text.Length > 0 && (!Regex.IsMatch(txtEndDate.Text, @"^\d{2}\-\d{2}\-\d{4}$") || !IsValidDate(txtEndDate.Text)))
        {
            SetErrorMessage("End date must be empty or valid and of the format dd-mm-yyyy<br />");
            return;
        }


        DateTime startDate = txtStartDate.Text.Length == 0 ? DateTime.MinValue : GetDateFromString(txtStartDate.Text, "dd-mm-yyyy");
        DateTime endDate = txtEndDate.Text.Length == 0 ? DateTime.MinValue : GetDateFromString(txtEndDate.Text, "dd-mm-yyyy");

        string url = ClearSearchesFromUrl(Request.RawUrl);
        url = UrlParamModifier.AddEdit(url, "start_date", startDate == DateTime.MinValue ? "" : startDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "end_date", endDate == DateTime.MinValue ? "" : endDate.ToString("yyyy_MM_dd"));

        Response.Redirect(url);
    }

    protected string ClearSearchesFromUrl(string url)
    {
        url = UrlParamModifier.Remove(url, "start_date");
        url = UrlParamModifier.Remove(url, "end_date");
        return url;
    }

    #endregion


    #region GrdSMSCredit

    private bool hideFotter = false;

    protected void FillGrid()
    {
        DataTable dt = SMSCreditDataDB.GetDataTable();
        Session["data_sms_credit"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sortExpression_sms_credit"] != null && Session["sortExpression_sms_credit"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sortExpression_sms_credit"].ToString();
                GrdSMSCredit.DataSource = dataView;
            }
            else
            {
                GrdSMSCredit.DataSource = dt;
            }


            try
            {
                GrdSMSCredit.DataBind();
                GrdSMSCredit.PagerSettings.FirstPageText = "1";
                GrdSMSCredit.PagerSettings.LastPageText = GrdSMSCredit.PageCount.ToString();
                GrdSMSCredit.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdSMSCredit.DataSource = dt;
            GrdSMSCredit.DataBind();

            int TotalColumns = GrdSMSCredit.Rows[0].Cells.Count;
            GrdSMSCredit.Rows[0].Cells.Clear();
            GrdSMSCredit.Rows[0].Cells.Add(new TableCell());
            GrdSMSCredit.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdSMSCredit.Rows[0].Cells[0].Text = "No Record Found";
        }

        if (hideFotter)
            GrdSMSCredit.FooterRow.Visible = false;


        decimal credit = SMSCreditDataDB.GetTotal();
        decimal used = SMSHistoryDataDB.GetTotal();
        lblTotalCredit.Text = credit.ToString("0.00");
        lblTotalUsed.Text = used.ToString("0.00");
        lblTotalRemaining.Text = (credit - used).ToString("0.00");



        DateTime fromDate = IsValidDate(txtStartDate.Text) ? GetDate(txtStartDate.Text) : DateTime.MinValue;
        DateTime toDate   = IsValidDate(txtEndDate.Text)   ? GetDate(txtEndDate.Text).Add(new TimeSpan(23, 59, 59)) : DateTime.MinValue;

        decimal ptRemidners    = SMSHistoryDataDB.GetPTReminders(fromDate, toDate);
        decimal ptBirthdays    = SMSHistoryDataDB.GetPTBirthdays(fromDate, toDate);
        decimal staffRemidners = SMSHistoryDataDB.GetStaffReminders(fromDate, toDate);

        lblPTReminders.Text = ptRemidners.ToString("0.00");
        lblPTBirthdays.Text = ptBirthdays.ToString("0.00");
        lblStaffReminders.Text = staffRemidners.ToString("0.00");


    }
    protected void GrdSMSCredit_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdSMSCredit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
    }
    protected void GrdSMSCredit_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdSMSCredit.EditIndex = -1;
        FillGrid();
    }
    protected void GrdSMSCredit_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdSMSCredit.Rows[e.RowIndex].FindControl("lblId");
        TextBox txtAmount = (TextBox)GrdSMSCredit.Rows[e.RowIndex].FindControl("txtAmount");

        DataTable dt = Session["data_sms_credit"] as DataTable;
        DataRow[] foundRows = dt.Select("sms_credit_id=" + lblId.Text);
        DataRow row = foundRows[0];
        SMSCreditData smsCredit = SMSCreditDataDB.Load(row);

        if (Convert.ToDecimal(txtAmount.Text) >= 1000000.00M)
        {
            SetErrorMessage("Amount must be less than 1000000.00");
            return;
        }

        SMSCreditDataDB.Update(smsCredit.SMSCreditID, Convert.ToDecimal(txtAmount.Text), smsCredit.DatetimeAdded);

        GrdSMSCredit.EditIndex = -1;
        FillGrid();
    }
    protected void GrdSMSCredit_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdSMSCredit.Rows[e.RowIndex].FindControl("lblId");
        DataTable dt = Session["data_sms_credit"] as DataTable;
        DataRow[] foundRows = dt.Select("sms_credit_id=" + lblId.Text);
        DataRow row = foundRows[0];
        SMSCreditData smsCredit = SMSCreditDataDB.Load(row);


        try
        {
            SMSCreditDataDB.Delete(smsCredit.SMSCreditID);
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrid();
    }
    protected void GrdSMSCredit_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            TextBox txtAmount = (TextBox)GrdSMSCredit.FooterRow.FindControl("txtNewAmount");

            if (Convert.ToDecimal(txtAmount.Text) >= 1000000.00M)
            {
                SetErrorMessage("Amount must be less than 1000000.00");
                return;
            }

            SMSCreditDataDB.Insert(Convert.ToDecimal(txtAmount.Text), DateTime.Now);
            FillGrid();
        }
    }
    protected void GrdSMSCredit_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdSMSCredit.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdSMSCredit.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["data_sms_credit"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression_sms_credit"] == null)
                Session["sortExpression_sms_credit"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression_sms_credit"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["sortExpression_sms_credit"] = sortExpression + " " + newSortExpr;

            GrdSMSCredit.DataSource = dataView;
            GrdSMSCredit.DataBind();
        }
    }

    protected void GrdSMSCredit_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdSMSCredit.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    #endregion

    #region SetSMSPriceTextBox(editable), btnEditEclaim

    protected void btnSMSPriceSetEditMode_Click(object sender, EventArgs e)
    {
        SetSMSPriceTextBox(true);
    }
    protected void btnSMSPriceCancelEditMode_Click(object sender, EventArgs e)
    {
        SetSMSPriceTextBox(false);
    }
    protected void btnSMSPriceUpdate_Click(object sender, EventArgs e)
    {
        if (!txtValidateSMSPriceRequired.IsValid || !txtValidateSMSPriceRegex.IsValid)
            return;

        SystemVariableDB.Update("SMSPrice", txtSMSPrice.Text);
        SetSMSPriceTextBox(false);
    }

    protected void SetSMSPriceTextBox(bool editable)
    {
        decimal smsPrice = Convert.ToDecimal(SystemVariableDB.GetByDescr("SMSPrice").Value);

        sms_credit_div.Style["width"] = editable ? "575px" : "350px";

        txtSMSPrice.Text      = smsPrice.ToString("0.00");
        txtSMSPrice.ReadOnly  = !editable;
        txtSMSPrice.Enabled   =  editable;
        txtSMSPrice.ForeColor = System.Drawing.Color.Black;

        btnSMSPriceSetEditMode.Visible    = !editable;
        btnSMSPriceCancelEditMode.Visible =  editable;
        btnSMSPriceUpdate.Visible         =  editable;
        Utilities.SetEditControlBackColour(txtSMSPrice, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Transparent, editable);

        if (editable)
        {
            // set cursor at "end" of the text
            string jsSetCursorEnd = @"var b98 = document.getElementById('" + txtSMSPrice.ID.ToString() + @"'); b98.focus(); var val = b98.value; b98.value = ''; b98.value = val;";
            ClientScript.RegisterStartupScript(Page.ClientScript.GetType(), Page.ClientID, jsSetCursorEnd, true);
        }
    }

    #endregion

    #region btnUpdateNotificationInfo_Click

    protected void btnUpdateNotificationInfo_Click(object sender, EventArgs e)
    {
        string amountREGEX = @"^\d+(\.\d{1,2})?$";

        txtSMSCreditNotificationEmailAddress.Text = txtSMSCreditNotificationEmailAddress.Text.Trim();

        if ((chkSMSCreditOutOfBalance_SendEmail.Checked || chkSMSCreditLowBalance_SendEmail.Checked) && !Utilities.IsValidEmailAddress(txtSMSCreditNotificationEmailAddress.Text))
        {
            SetErrorMessage("Invalid email address");
            return;
        }

        txtSMSCreditLowBalance_Threshold.Text = txtSMSCreditLowBalance_Threshold.Text.Trim();
        if (txtSMSCreditLowBalance_Threshold.Text.Length == 0) txtSMSCreditLowBalance_Threshold.Text = "0";
        if (!Regex.IsMatch(txtSMSCreditLowBalance_Threshold.Text, amountREGEX))
        {
            SetErrorMessage("Low Balance Warning Threshold can only be numbers and optional decimal place with 1 or 2 digits following.");
            return;
        }

        SystemVariableDB.Update("SMSCreditNotificationEmailAddress", txtSMSCreditNotificationEmailAddress.Text);
        SystemVariableDB.Update("SMSCreditLowBalance_Threshold",     Convert.ToDouble(txtSMSCreditLowBalance_Threshold.Text).ToString("0.00"));
        SystemVariableDB.Update("SMSCreditOutOfBalance_SendEmail",   chkSMSCreditOutOfBalance_SendEmail.Checked ? "1" : "0");
        SystemVariableDB.Update("SMSCreditLowBalance_SendEmail",     chkSMSCreditLowBalance_SendEmail.Checked ? "1" : "0");
    }

    protected void btnRevertNotificationInfo_Click(object sender, EventArgs e)
    {
        SetNotificationInfo();
    }


    protected void SetNotificationInfo()
    {
        txtSMSCreditNotificationEmailAddress.Text  = SystemVariableDB.GetByDescr("SMSCreditNotificationEmailAddress").Value;
        txtSMSCreditLowBalance_Threshold.Text      = SystemVariableDB.GetByDescr("SMSCreditLowBalance_Threshold").Value;
        chkSMSCreditOutOfBalance_SendEmail.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("SMSCreditOutOfBalance_SendEmail").Value) == 1;
        chkSMSCreditLowBalance_SendEmail.Checked   = Convert.ToInt32(SystemVariableDB.GetByDescr("SMSCreditLowBalance_SendEmail").Value)   == 1;
        btnUpdateNotificationInfo.Style["visibility"] = "hidden";
        btnRevertNotificationInfo.Style["visibility"] = "hidden";
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