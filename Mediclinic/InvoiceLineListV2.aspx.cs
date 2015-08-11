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

public partial class InvoiceLineListV2 : System.Web.UI.Page
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
                Session.Remove("sortExpression_summaryReport");
                Session.Remove("data_summaryReport");
                SetupGUI();
                FillGrid();
            }

            this.GrdSummaryReport.EnableViewState = true;

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
        if (Request.QueryString["date_type"] != null)
            rblDateType.SelectedValue = Request.QueryString["date_type"];


        UserView userView = UserView.GetInstance();

        ddlOrgs.Style["width"] = "300px";
        ddlOrgs.Items.Clear();
        ddlOrgs.Items.Add(new ListItem("All " + (userView.IsAgedCareView ? "Facilities" : "Clinics"), (-1).ToString()));
        foreach (Organisation curOrg in OrganisationDB.GetAll(false, true, !userView.IsClinicView && !userView.IsGPView, !userView.IsAgedCareView, true, true))
        {
            ListItem li = new ListItem(curOrg.Name, curOrg.OrganisationID.ToString());
            li.Attributes.Add("title", curOrg.Name);
            ddlOrgs.Items.Add(li);
        }

        ddlProviders.Style["width"] = "300px";
        ddlProviders.Items.Clear();
        ddlProviders.Items.Add(new ListItem("All Providers", (-1).ToString()));
        foreach (Staff curProv in StaffDB.GetAll())
            if (curProv.IsProvider)
                ddlProviders.Items.Add(new ListItem(curProv.Person.FullnameWithoutMiddlename, curProv.StaffID.ToString()));

        ddlOfferings.Style["width"] = "300px";
        ddlOfferings.Items.Clear();
        ddlOfferings.Items.Add(new ListItem("All Products & Services", (-1).ToString()));
        foreach (Offering offering in OfferingDB.GetAll(false, userView.IsAgedCareView ? "3,4" : "1,3", "63,89", false))
            ddlOfferings.Items.Add(new ListItem(offering.Name, offering.OfferingID.ToString()));

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

        if (IsValidFormOfferingID())
        {
            Offering offering = OfferingDB.GetByID(GetFormOfferingID());
            if (offering != null)
                ddlOfferings.SelectedValue = offering.OfferingID.ToString();
        }



        txtStartDate.Text = IsValidFormStartDate() ? (GetFormStartDate(false) == DateTime.MinValue ? "" : GetFormStartDate(false).ToString("dd-MM-yyyy")) : DateTime.Today.ToString("dd-MM-yyyy");
        txtEndDate.Text   = IsValidFormEndDate()   ? (GetFormEndDate(false)   == DateTime.MinValue ? "" : GetFormEndDate(false).ToString("dd-MM-yyyy"))   : DateTime.Today.ToString("dd-MM-yyyy");

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";
    }


    #region GrdSummaryReport

    protected void FillGrid()
    {
        UserView userView = UserView.GetInstance();

        DateTime fromDate = IsValidDate(txtStartDate.Text) ? GetDate(txtStartDate.Text)                              : DateTime.MinValue;
        DateTime toDate   = IsValidDate(txtEndDate.Text)   ? GetDate(txtEndDate.Text).Add(new TimeSpan(23, 59, 59))  : DateTime.MinValue;


        int organisation_type_group_id = -1;
        if (userView.IsClinicView)
            organisation_type_group_id = 5;
        if (userView.IsAgedCareView)
            organisation_type_group_id = 6;

        DataTable dt = null;
        if (rblDateType.SelectedValue == "Bookings")
            dt = BookingDB.GetReport_InvoiceLines(fromDate, toDate, DateTime.MinValue, DateTime.MinValue, Convert.ToInt32(ddlOrgs.SelectedValue), Convert.ToInt32(ddlProviders.SelectedValue), Convert.ToInt32(ddlOfferings.SelectedValue), organisation_type_group_id);
        else if (rblDateType.SelectedValue == "Invoices")
            dt = BookingDB.GetReport_InvoiceLines(DateTime.MinValue, DateTime.MinValue, fromDate, toDate, Convert.ToInt32(ddlOrgs.SelectedValue), Convert.ToInt32(ddlProviders.SelectedValue), Convert.ToInt32(ddlOfferings.SelectedValue), organisation_type_group_id);
        else
        {
            SetErrorMessage("Please select date range to be treatment date or invoice date.");
            return;
        }



        dt.Columns.Add("booking_duration_total_minutes", typeof(string));
        dt.Columns.Add("organisation_name", typeof(string));
        dt.Columns.Add("patient_id",        typeof(int));
        dt.Columns.Add("patient_firstname", typeof(string));
        dt.Columns.Add("patient_surname",   typeof(string));
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["booking_id"] == DBNull.Value)
            {
                dt.Rows[i]["booking_duration_total_minutes"] = "";
                dt.Rows[i]["organisation_name"] = dt.Rows[i]["non_booking_organisation_name"];
                dt.Rows[i]["patient_id"]        = dt.Rows[i]["non_booking_patient_id"]; 
                dt.Rows[i]["patient_firstname"] = dt.Rows[i]["non_booking_patient_firstname"]; 
                dt.Rows[i]["patient_surname"]   = dt.Rows[i]["non_booking_patient_surname"];
            }
            else
            {
                DateTime bookingStart = Convert.ToDateTime(dt.Rows[i]["booking_date_start"]);
                DateTime bookingEnd   = Convert.ToDateTime(dt.Rows[i]["booking_date_end"]);
                dt.Rows[i]["booking_duration_total_minutes"] = bookingEnd.Subtract(bookingStart).TotalMinutes.ToString();
                dt.Rows[i]["organisation_name"] = dt.Rows[i]["booking_organisation_name"];
                dt.Rows[i]["patient_id"]        = dt.Rows[i]["booking_patient_id"]; 
                dt.Rows[i]["patient_firstname"] = dt.Rows[i]["booking_patient_firstname"];
                dt.Rows[i]["patient_surname"]   = dt.Rows[i]["booking_patient_surname"];
            }
        }


        Hashtable staffOfferingHash = StaffOfferingsDB.Get2DHash(true, Convert.ToInt32(ddlProviders.SelectedValue));
        dt.Columns.Add("commission_percent_text", typeof(string));
        dt.Columns.Add("fixed_rate_text", typeof(string));
        dt.Columns.Add("commission_percent_amount", typeof(decimal));
        dt.Columns.Add("fixed_rate_amount", typeof(decimal));
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["booking_id"] == DBNull.Value)
            {
                dt.Rows[i]["commission_percent_text"]   = "";
                dt.Rows[i]["fixed_rate_text"]           = "";
                dt.Rows[i]["commission_percent_amount"] = 0;
                dt.Rows[i]["fixed_rate_amount"]         = 0;
            }
            else
            {
                StaffOfferings staffOffering = (StaffOfferings)staffOfferingHash[new Hashtable2D.Key(Convert.ToInt32(dt.Rows[i]["provider_staff_id"]), dt.Rows[i]["offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(dt.Rows[i]["offering_id"]))];
                //dt.Rows[i]["commission_percent_text"]   = staffOffering == null || !staffOffering.IsCommission ? "" : Math.Round(staffOffering.CommissionPercent * Convert.ToDecimal(dt.Rows[i]["invoice_line_price"]) / 100, 2).ToString() + " (" + staffOffering.CommissionPercent + "%)";
                dt.Rows[i]["commission_percent_text"]   = staffOffering == null || !staffOffering.IsCommission ? "" : Math.Round(staffOffering.CommissionPercent * Convert.ToDecimal(dt.Rows[i]["invoice_line_price"]) / 100, 2).ToString();
                dt.Rows[i]["fixed_rate_text"]           = staffOffering == null || !staffOffering.IsFixedRate  ? "" : staffOffering.FixedRate.ToString();
                dt.Rows[i]["commission_percent_amount"] = staffOffering == null || !staffOffering.IsCommission ? Convert.ToDecimal(0.00) : Math.Round(staffOffering.CommissionPercent * Convert.ToDecimal(dt.Rows[i]["invoice_line_price"]) / 100, 2);
                dt.Rows[i]["fixed_rate_amount"]         = staffOffering == null || !staffOffering.IsFixedRate  ? Convert.ToDecimal(0.00) : staffOffering.FixedRate;
            }
        }



        Session["data_summaryReport"] = dt;

        if (!IsPostBack)
            chkUsePaging.Checked = dt.Rows.Count > 50;

        this.GrdSummaryReport.AllowPaging = chkUsePaging.Checked;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sortExpression_summaryReport"] != null && Session["sortExpression_summaryReport"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sortExpression_summaryReport"].ToString();
                GrdSummaryReport.DataSource = dataView;
            }
            else
            {
                GrdSummaryReport.DataSource = dt;
            }


            try
            {
                GrdSummaryReport.DataBind();
                GrdSummaryReport.PagerSettings.FirstPageText = "1";
                GrdSummaryReport.PagerSettings.LastPageText = GrdSummaryReport.PageCount.ToString();
                GrdSummaryReport.DataBind();

            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdSummaryReport.DataSource = dt;
            GrdSummaryReport.DataBind();

            int TotalColumns = GrdSummaryReport.Rows[0].Cells.Count;
            GrdSummaryReport.Rows[0].Cells.Clear();
            GrdSummaryReport.Rows[0].Cells.Add(new TableCell());
            GrdSummaryReport.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdSummaryReport.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdSummaryReport_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdSummaryReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["data_summaryReport"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("invoice_line_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            Label lnkPatient = (Label)e.Row.FindControl("lnkPatient");
            if (lnkPatient != null)
            {
                if (thisRow["patient_id"] == null)
                    lnkPatient.Visible = false;
                else
                {
                    string URL = "PatientDetailV2.aspx?type=view&id=" + thisRow["patient_id"];
                    if (URL.StartsWith("~")) URL = URL.Substring(1);
                    lnkPatient.Text = "<a href=\"#\" onclick=\"open_new_window('" + URL + "');return false;\" >" + thisRow["patient_firstname"] + " " + thisRow["patient_surname"] + "</a>";
                }
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSum_Amount = (Label)e.Row.FindControl("lblSum_Amount");
            lblSum_Amount.Text = String.Format("{0:C}", dt.Compute("Sum(invoice_line_price)", ""));
            if (lblSum_Amount.Text == "") lblSum_Amount.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_GST = (Label)e.Row.FindControl("lblSum_GST");
            lblSum_GST.Text = String.Format("{0:C}", dt.Compute("Sum(invoice_line_tax)", ""));
            if (lblSum_GST.Text == "") lblSum_GST.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";


            //decimal totalCreditNotes = (decimal)dt.Compute("Sum(total_credit_notes)", "");
            decimal totalCreditNotes = GetCreditNotesTotal(dt);

            Label lblSum_Rebates = (Label)e.Row.FindControl("lblSum_Rebates");
            lblSum_Rebates.Text = String.Format("{0:C}", totalCreditNotes);
            if (lblSum_Rebates.Text == "") lblSum_Rebates.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_CommissionPercent = (Label)e.Row.FindControl("lblSum_CommissionPercent");
            lblSum_CommissionPercent.Text = String.Format("{0:C}", dt.Compute("Sum(commission_percent_amount)", ""));
            if (lblSum_CommissionPercent.Text == "") lblSum_CommissionPercent.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_CommissionFixed = (Label)e.Row.FindControl("lblSum_CommissionFixed");
            lblSum_CommissionFixed.Text = String.Format("{0:C}", dt.Compute("Sum(fixed_rate_amount)", ""));
            if (lblSum_CommissionFixed.Text == "") lblSum_CommissionFixed.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";
        }
    }
    protected void GrdSummaryReport_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdSummaryReport.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }
    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["data_summaryReport"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression_summaryReport"] == null)
                Session["sortExpression_summaryReport"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression_summaryReport"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["sortExpression_summaryReport"] = sortExpression + " " + newSortExpr;

            GrdSummaryReport.DataSource = dataView;
            GrdSummaryReport.DataBind();
        }
    }
    protected void GrdStaff_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdSummaryReport.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    #endregion

    protected decimal GetCreditNotesTotal(DataTable dt)
    {
        Hashtable seenInvoices = new Hashtable();
        decimal total = 0;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int invoice_id = Convert.ToInt32(dt.Rows[i]["invoice_id"]);

                if (seenInvoices[invoice_id] != null)
                    continue;

                seenInvoices[invoice_id] = 1;
                total += Convert.ToDecimal(dt.Rows[i]["total_credit_notes"]);
            }
        }

        return total;
    }

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
    private bool IsValidFormOfferingID()
    {
        string id = Request.QueryString["offering"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormOfferingID()
    {
        if (!IsValidFormOfferingID())
            throw new Exception("Invalid url offering");

        string id = Request.QueryString["offering"];
        return Convert.ToInt32(id);
    }

    protected bool IsValidFormIncCompleted()
    {
        string inc_completed = Request.QueryString["inc_completed"];
        return inc_completed != null && (inc_completed == "0" || inc_completed == "1");
    }
    protected bool GetFormIncCompleted(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncCompleted())
            throw new Exception("Invalid url 'inc_completed'");
        return Request.QueryString["inc_completed"] == "1";
    }
    protected bool IsValidFormIncIncomplete()
    {
        string inc_incomplete = Request.QueryString["inc_incomplete"];
        return inc_incomplete != null && (inc_incomplete == "0" || inc_incomplete == "1");
    }
    protected bool GetFormIncIncomplete(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncIncomplete())
            throw new Exception("Invalid url 'inc_incomplete'");
        return Request.QueryString["inc_incomplete"] == "1";
    }
    protected bool IsValidFormIncCancelled()
    {
        string inc_cancelled = Request.QueryString["inc_cancelled"];
        return inc_cancelled != null && (inc_cancelled == "0" || inc_cancelled == "1");
    }
    protected bool GetFormIncCancelled(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncCancelled())
            throw new Exception("Invalid url 'inc_cancelled'");
        return Request.QueryString["inc_cancelled"] == "1";
    }
    protected bool IsValidFormIncDeleted()
    {
        string inc_deleted = Request.QueryString["inc_deleted"];
        return inc_deleted != null && (inc_deleted == "0" || inc_deleted == "1");
    }
    protected bool GetFormIncDeleted(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncDeleted())
            throw new Exception("Invalid url 'inc_deleted'");
        return Request.QueryString["inc_deleted"] == "1";
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

    #region ddlOrgs_SelectedIndexChanged, ddlProviders_SelectedIndexChanged, ddlOfferings_SelectedIndexChanged

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

    protected void ddlOfferings_SelectedIndexChanged(object sender, EventArgs e)
    {
        int newOfferingID = Convert.ToInt32(ddlOfferings.SelectedValue);

        string url = Request.RawUrl;
        url = UrlParamModifier.Update(newOfferingID != -1, url, "offering", newOfferingID == -1 ? "" : newOfferingID.ToString());
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
        url = UrlParamModifier.AddEdit(url, "start_date"    , startDate == DateTime.MinValue ? "" : startDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "end_date"      , endDate   == DateTime.MinValue ? "" : endDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "date_type"     , rblDateType.SelectedValue);

        Response.Redirect(url);
    }

    protected string ClearSearchesFromUrl(string url)
    {
        url = UrlParamModifier.Remove(url, "start_date");
        url = UrlParamModifier.Remove(url, "end_date");
        url = UrlParamModifier.Remove(url, "date_type");

        return url;
    }

    protected void chkUsePaging_CheckedChanged(object sender, EventArgs e)
    {
        this.GrdSummaryReport.AllowPaging = chkUsePaging.Checked;
        FillGrid();
    }

    #endregion

    #region btnExport_Click

    protected void btnExport_Click(object sender, EventArgs e)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("\"" + "Provider"    + "\"").Append(",");
        sb.Append("\"" + "Recorded"    + "\"").Append(",");
        sb.Append("\"" + "Booked for"  + "\"").Append(",");
        sb.Append("\"" + "Start"       + "\"").Append(",");
        sb.Append("\"" + "End"         + "\"").Append(",");
        sb.Append("\"" + "Durtaion"    + "\"").Append(",");
        sb.Append("\"" + "Inv date"    + "\"").Append(",");
        sb.Append("\"" + "Clinic"      + "\"").Append(",");
        sb.Append("\"" + "Booking"     + "\"").Append(",");
        sb.Append("\"" + "Patient"     + "\"").Append(",");
        sb.Append("\"" + "Offering"    + "\"").Append(",");
        sb.Append("\"" + "Amount"      + "\"").Append(",");
        sb.Append("\"" + "GST"         + "\"").Append(",");
        sb.Append("\"" + "Qty"         + "\"").Append(",");
        sb.Append("\"" + "Rebates"     + "\"").Append(",");
        sb.Append("\"" + "Comm. %"     + "\"").Append(",");
        sb.Append("\"" + "Comm. Fixed" + "\"");

        sb.AppendLine();


        DataTable dt = Session["data_summaryReport"] as DataTable;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["booking_id"] == DBNull.Value)
                {
                    sb.Append("\"" + "\"").Append(",");
                    sb.Append("\"" + "\"").Append(",");
                    sb.Append("\"" + "\"").Append(",");
                    sb.Append("\"" + "\"").Append(",");
                    sb.Append("\"" + "\"").Append(",");
                    sb.Append("\"" + "\"").Append(",");
                }
                else
                {
                    sb.Append("\"" + dt.Rows[i]["provider_firstname"].ToString() + " " + dt.Rows[i]["provider_surname"].ToString()                                                                                  + "\"").Append(",");
                    sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["booking_date_last_moved"] != DBNull.Value ? dt.Rows[i]["booking_date_last_moved"] : dt.Rows[i]["booking_date_created"]).ToString("dd MMM yyyy") + "\"").Append(",");
                    sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["booking_date_start"]).ToString("dd MMM yyyy")                                                                                                   + "\"").Append(",");
                    sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["booking_date_start"]).ToString("H:mm")                                                                                                          + "\"").Append(",");
                    sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["booking_date_end"]).ToString("H:mm")                                                                                                            + "\"").Append(",");
                    sb.Append("\"" + Convert.ToInt32(dt.Rows[i]["booking_duration_total_minutes"])                                                                                                                  + "\"").Append(",");
                }
                sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["invoice_date_added"]).ToString("dd MMM yyyy")                                                                                                   + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["organisation_name"].ToString()                                                                                                                                     + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["booking_id"].ToString()                                                                                                                                            + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["patient_firstname"].ToString()  + " " + dt.Rows[i]["patient_surname"].ToString()                                                                                   + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["offering_name"].ToString()                                                                                                                                         + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["invoice_line_price"].ToString()                                                                                                                                    + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["invoice_line_tax"].ToString()                                                                                                                                    + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["invoice_line_quantity"] == DBNull.Value ? "" : (Convert.ToDecimal(dt.Rows[i]["invoice_line_quantity"]) == Convert.ToDecimal((int)Convert.ToDecimal(dt.Rows[i]["invoice_line_quantity"])) ? Convert.ToDecimal((int)Convert.ToDecimal(dt.Rows[i]["invoice_line_quantity"])).ToString() : dt.Rows[i]["organisation_name"].ToString())) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["total_credit_notes"] == DBNull.Value || Convert.ToDecimal(dt.Rows[i]["total_credit_notes"]) == 0 ? "" : dt.Rows[i]["total_credit_notes"].ToString())              + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["commission_percent_text"].ToString()                                                                                                                               + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["fixed_rate_text"].ToString()                                                                                                                                       + "\"");
                sb.AppendLine();
            }
        }

        string Sum_Amount = String.Format("{0:C}", dt.Compute("Sum(invoice_line_price)", ""));
        if (Sum_Amount == "") Sum_Amount = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_GST = String.Format("{0:C}", dt.Compute("Sum(invoice_line_tax)", ""));
        if (Sum_GST == "") Sum_GST = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_Rebates = String.Format("{0:C}", GetCreditNotesTotal(dt));
        if (Sum_Rebates == "") Sum_Rebates = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_CommissionPercent = String.Format("{0:C}", dt.Compute("Sum(commission_percent_amount)", ""));
        if (Sum_CommissionPercent == "") Sum_CommissionPercent = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_CommissionFixed = String.Format("{0:C}", dt.Compute("Sum(fixed_rate_amount)", ""));
        if (Sum_CommissionFixed == "") Sum_CommissionFixed = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + Sum_Amount + "\"").Append(",");
        sb.Append("\"" + Sum_GST + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + Sum_Rebates + "\"").Append(",");
        sb.Append("\"" + Sum_CommissionPercent + "\"").Append(",");
        sb.Append("\"" + Sum_CommissionFixed + "\"");

        sb.AppendLine();


        ExportCSV(Response, sb.ToString(), "ProviderInvoicedBooking.csv");
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
        GrdSummaryReport.Visible = false;
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