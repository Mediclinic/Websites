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

public partial class ReceiptsListV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, true, false);
                Session.Remove("sortExpression_receiptsReport");
                Session.Remove("data_receiptsReport");
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
        chkIncMedicare.Checked   = IsValidFormIncMedicare()   ? GetFormIncMedicare(false)   : false;
        chkIncDVA.Checked        = IsValidFormIncDVA()        ? GetFormIncDVA(false)        : false;
        chkIncPrivate.Checked    = IsValidFormIncPrivate()    ? GetFormIncPrivate(false)    : true;
        chkIncReconciled.Checked = IsValidFormIncReconciled() ? GetFormIncReconciled(false) : true;


        UserView userView = UserView.GetInstance();

        ddlOrgs.Style["width"] = "300px";
        ddlOrgs.Items.Clear();
        ddlOrgs.Items.Add(new ListItem("All " + (userView.IsAgedCareView ? "Facilities" : "Clinics"), (-1).ToString()));
        foreach (Organisation curOrg in OrganisationDB.GetAll(false, true, !userView.IsClinicView && !userView.IsGPView, !userView.IsAgedCareView, true, true))
            ddlOrgs.Items.Add(new ListItem(curOrg.Name, curOrg.OrganisationID.ToString()));

        ddlProviders.Style["width"] = "300px";
        ddlProviders.Items.Clear();
        ddlProviders.Items.Add(new ListItem("All Staff", (-1).ToString()));
        foreach (Staff curProv in StaffDB.GetAll())
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

        ddlReceiptPaymentType.Style["width"] = "300px";
        DataTable paymentTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "ReceiptPaymentType", "", "descr", "receipt_payment_type_id,descr");
        ddlReceiptPaymentType.Items.Add(new ListItem("All Payment Types", "-1"));
        for (int i = 0; i < paymentTypes.Rows.Count; i++)
            ddlReceiptPaymentType.Items.Add(new ListItem(paymentTypes.Rows[i]["descr"].ToString(), paymentTypes.Rows[i]["receipt_payment_type_id"].ToString()));

        if (IsValidFormPaymentType())
            ddlReceiptPaymentType.SelectedValue = GetFormPaymentType(false).ToString();


        txtStartDate.Text = IsValidFormStartDate() ? (GetFormStartDate(false) == DateTime.MinValue ? "" : GetFormStartDate(false).ToString("dd-MM-yyyy")) : DateTime.Today.ToString("dd-MM-yyyy");
        txtEndDate.Text   = IsValidFormEndDate()   ? (GetFormEndDate(false)   == DateTime.MinValue ? "" : GetFormEndDate(false).ToString("dd-MM-yyyy"))   : DateTime.Today.ToString("dd-MM-yyyy");

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate',   this, 'dmy', '-'); return false;";
    }

    #region GrdSummaryReport

    protected void FillGrid()
    {
        UserView userView = UserView.GetInstance();

        DateTime fromDate = IsValidDate(txtStartDate.Text) ? GetDate(txtStartDate.Text) : DateTime.MinValue;
        DateTime toDate   = IsValidDate(txtEndDate.Text)   ? GetDate(txtEndDate.Text).Add(new TimeSpan(23, 59, 59))  : DateTime.MinValue;

        int organisation_type_group_id = -1;
        if (userView.IsClinicView)
            organisation_type_group_id = 5;
        if (userView.IsAgedCareView)
            organisation_type_group_id = 6;


        DataTable dt = BookingDB.GetReport_Receipts(fromDate, toDate, Convert.ToInt32(ddlOrgs.SelectedValue), Convert.ToInt32(ddlProviders.SelectedValue), chkIncMedicare.Checked, chkIncDVA.Checked, chkIncPrivate.Checked, chkIncReconciled.Checked, Convert.ToInt32(ddlReceiptPaymentType.SelectedValue), organisation_type_group_id);  // organisation_type_group_id : [clinic=5, aged care = 6]
        Session["data_receiptsReport"] = dt;

        if (!IsPostBack)
            chkUsePaging.Checked = dt.Rows.Count > 50;

        this.GrdSummaryReport.AllowPaging = chkUsePaging.Checked;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sortExpression_receiptsReport"] != null && Session["sortExpression_receiptsReport"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sortExpression_receiptsReport"].ToString();
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
        DataTable dt = Session["data_receiptsReport"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("receipt_id =" + lblId.Text);
            DataRow thisRow = foundRows[0];

            Booking booking = thisRow["booking_booking_id"] != DBNull.Value ? BookingDB.Load(thisRow, "booking_", false, false)  : null;
            int invoiceID = Convert.ToInt32(thisRow["invoice_id"]);

            HyperLink lnkBookingSheetForPatient = (HyperLink)e.Row.FindControl("lnkBookingSheetForPatient");
            if (lnkBookingSheetForPatient != null)
            {
                lnkBookingSheetForPatient.NavigateUrl = booking != null ? booking.GetBookingSheetLinkV2() : "";
                lnkBookingSheetForPatient.Visible = booking != null;
            }

            LinkButton lnkInvoiceID = (LinkButton)e.Row.FindControl("lnkInvoiceID");
            if (lnkInvoiceID != null)
                lnkInvoiceID.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?invoice_id={0}', '', 'dialogWidth:775px;dialogHeight:" + (thisRow["booking_booking_id"] != DBNull.Value ? "900" : "650") + "px;center:yes;resizable:no; scroll:no');return false;", thisRow["invoice_id"]);




            Label lblPayer = (Label)e.Row.FindControl("lblPayer");
            if (lblPayer != null)
            {
                if (thisRow["inv_payer_organisation_id"] != DBNull.Value)
                    lblPayer.Text = thisRow["inv_payer_organisation_name"].ToString();
                else if (thisRow["inv_payer_patient_id"] != DBNull.Value)
                    lblPayer.Text = thisRow["inv_payer_patient_person_firstname"].ToString() + " " + thisRow["inv_payer_patient_person_surname"].ToString();
                else
                {
                    if (booking != null)
                    {
                        // can add this query each row because in the whole system there is only 32 invoices that get to here
                        // since the rest keep the patient as the payer_patient
                        // and doing this for only 32 rows avoids pulling all the extra data for all invoices so its faster doing this

                        lblPayer.Text = BookingDB.GetByID(booking.BookingID).Patient.Person.FullnameWithoutMiddlename;
                    }
                }
            }

            Label lblPatient = (Label)e.Row.FindControl("lblPatient");
            if (lblPatient != null)
            {
                if (booking != null)
                    lblPatient.Text = thisRow["booking_patient_firstname"] + " " + thisRow["booking_patient_surname"];
                else // for private invoices
                    lblPatient.Text = thisRow["non_booking_patient_firstname"] + " " + thisRow["non_booking_patient_surname"];
            }



            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSum_Total = (Label)e.Row.FindControl("lblSum_Total");
            lblSum_Total.Text = String.Format("{0:C}", dt.Compute("Sum(total)", ""));
            if (lblSum_Total.Text == "") lblSum_Total.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

            Label lblSum_AmountReconciled = (Label)e.Row.FindControl("lblSum_AmountReconciled");
            lblSum_AmountReconciled.Text = String.Format("{0:C}", dt.Compute("Sum(amount_reconciled)", ""));
            if (lblSum_AmountReconciled.Text == "") lblSum_AmountReconciled.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";
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
        if (e.CommandName == "SetReconciled")
        {
            int receiptID = Convert.ToInt32(e.CommandArgument);
            ReceiptDB.SetReconciled(receiptID);
            FillGrid();

            Receipt receipt = ReceiptDB.GetByID(receiptID);
            SetErrorMessage("Receipt " + receiptID + " for Invoice " + receipt.Invoice.InvoiceID + " set as reconciled");
        }

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
        DataTable dataTable = Session["data_receiptsReport"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression_receiptsReport"] == null)
                Session["sortExpression_receiptsReport"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression_receiptsReport"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["sortExpression_receiptsReport"] = sortExpression + " " + newSortExpr;

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

    #region IsValidFormStartDate(),  GetFormStartDate()....

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

    protected bool IsValidFormIncMedicare()
    {
        string inc_medicare = Request.QueryString["inc_medicare"];
        return inc_medicare != null && (inc_medicare == "0" || inc_medicare == "1");
    }
    protected bool GetFormIncMedicare(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncMedicare())
            throw new Exception("Invalid url 'inc_medicare'");
        return Request.QueryString["inc_medicare"] == "1";
    }
    protected bool IsValidFormIncDVA()
    {
        string inc_dva = Request.QueryString["inc_dva"];
        return inc_dva != null && (inc_dva == "0" || inc_dva == "1");
    }
    protected bool GetFormIncDVA(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncDVA())
            throw new Exception("Invalid url 'inc_dva'");
        return Request.QueryString["inc_dva"] == "1";
    }
    protected bool IsValidFormIncPrivate()
    {
        string inc_private = Request.QueryString["inc_private"];
        return inc_private != null && (inc_private == "0" || inc_private == "1");
    }
    protected bool GetFormIncPrivate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncPrivate())
            throw new Exception("Invalid url 'inc_private'");
        return Request.QueryString["inc_private"] == "1";
    }
    protected bool IsValidFormIncReconciled()
    {
        string inc_reconciled = Request.QueryString["inc_reconciled"];
        return inc_reconciled != null && (inc_reconciled == "0" || inc_reconciled == "1");
    }
    protected bool GetFormIncReconciled(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncReconciled())
            throw new Exception("Invalid url 'inc_reconciled'");
        return Request.QueryString["inc_reconciled"] == "1";
    }
    protected bool IsValidFormPaymentType()
    {
        string payment_type = Request.QueryString["payment_type"];
        return payment_type != null && (Regex.IsMatch(payment_type, @"^\d+$"));
    }
    protected int GetFormPaymentType(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormPaymentType())
            throw new Exception("Invalid url 'payment_type'");
        return Convert.ToInt32(Request.QueryString["payment_type"]);
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

    protected void ddlReceiptPaymentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        int newddlReceiptPaymentTypeID = Convert.ToInt32(ddlReceiptPaymentType.SelectedValue);

        string url = Request.RawUrl;
        url = UrlParamModifier.Update(newddlReceiptPaymentTypeID != -1, url, "payment_type", newddlReceiptPaymentTypeID == -1 ? "" : newddlReceiptPaymentTypeID.ToString());
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
        url = UrlParamModifier.AddEdit(url, "inc_medicare",   chkIncMedicare.Checked   ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_dva",        chkIncDVA.Checked        ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_private",    chkIncPrivate.Checked    ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_reconciled", chkIncReconciled.Checked ? "1" : "0");

        Response.Redirect(url);
    }

    protected string ClearSearchesFromUrl(string url)
    {
        url = UrlParamModifier.Remove(url, "start_date");
        url = UrlParamModifier.Remove(url, "end_date");
        url = UrlParamModifier.Remove(url, "inc_medicare");
        url = UrlParamModifier.Remove(url, "inc_dva");
        url = UrlParamModifier.Remove(url, "inc_private");
        url = UrlParamModifier.Remove(url, "inc_reconciled");

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

        sb.Append("\"" + "Date Added  "    + "\"").Append(",");
        sb.Append("\"" + "Type"            + "\"").Append(",");
        sb.Append("\"" + "Total"           + "\"").Append(",");
        sb.Append("\"" + "Reconciled"      + "\"").Append(",");
        sb.Append("\"" + "Added By"        + "\"").Append(",");
        sb.Append("\"" + "Organisation"    + "\"").Append(",");
        sb.Append("\"" + "Reversed"        + "\"").Append(",");
        sb.Append("\"" + "Treatment Date"  + "\"").Append(",");
        sb.Append("\"" + "Invoice #"       + "\"").Append(",");
        sb.Append("\"" + "Inv Debtor"      + "\"").Append(",");
        sb.Append("\"" + "Patient"         + "\"").Append(",");
        sb.AppendLine();


        DataTable dt = Session["data_receiptsReport"] as DataTable;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["receipt_date_added"]).ToString("dd MMM yyyy") + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["receipt_payment_type_descr"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["total"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["amount_reconciled"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["receipt_staff_person_firstname"].ToString() + (dt.Rows[i]["receipt_staff_person_firstname"] == DBNull.Value ? "" : " ") + dt.Rows[i]["receipt_staff_person_surname"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["organisation_name"].ToString() + "\"").Append(",");

                if (dt.Rows[i]["reversed_date"] == DBNull.Value)
                    sb.Append("\"" + "" + "\"").Append(",");
                else
                    sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["reversed_date"]).ToString("dd MMM yyyy") + Environment.NewLine + "By " + dt.Rows[i]["receipt_reversed_by_person_firstname"].ToString() + (dt.Rows[i]["receipt_reversed_by_person_firstname"] == DBNull.Value ? "" : " ") + dt.Rows[i]["receipt_reversed_by_person_surname"].ToString() + (dt.Rows[i]["receipt_reversed_by_person_firstname"].ToString().Length + dt.Rows[i]["receipt_reversed_by_person_surname"].ToString().Length == 0 ? "" : Environment.NewLine) + "Previously " + dt.Rows[i]["pre_reversed_amount"].ToString() + "\"").Append(",");

                if (dt.Rows[i]["booking_date_start"] == DBNull.Value)
                    sb.Append("\"" + "" + "\"").Append(",");
                else
                    sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["booking_date_start"]).ToString("dd MMM yyyy HH:mm") + "\"").Append(",");

                sb.Append("\"" + dt.Rows[i]["invoice_id"].ToString() + "\"").Append(",");





                Booking booking = dt.Rows[i]["booking_booking_id"] != DBNull.Value ? BookingDB.Load(dt.Rows[i], "booking_", false, false) : null;

                string payer = string.Empty;
                if (dt.Rows[i]["inv_payer_organisation_id"] != DBNull.Value)
                    payer = dt.Rows[i]["inv_payer_organisation_name"].ToString();
                else if (dt.Rows[i]["inv_payer_patient_id"] != DBNull.Value)
                    payer = dt.Rows[i]["inv_payer_patient_person_firstname"].ToString() + " " + dt.Rows[i]["inv_payer_patient_person_surname"].ToString();
                else
                {
                    if (booking != null)
                    {
                        // can add this query each row because in the whole system there is only 32 invoices that get to here
                        // since the rest keep the patient as the payer_patient
                        // and doing this for only 32 rows avoids pulling all the extra data for all invoices so its faster doing this

                        payer = BookingDB.GetByID(booking.BookingID).Patient.Person.FullnameWithoutMiddlename;
                    }
                }

                string patient = string.Empty;
                if (booking != null)
                    patient = dt.Rows[i]["booking_patient_firstname"] + " " + dt.Rows[i]["booking_patient_surname"];
                else // for private invoices
                    patient  = dt.Rows[i]["non_booking_patient_firstname"] + " " + dt.Rows[i]["non_booking_patient_surname"];


                sb.Append("\"" + payer   + "\"").Append(",");
                sb.Append("\"" + patient + "\"").Append(",");

                sb.AppendLine();
            }
        }

        string Sum_Total = String.Format("{0:C}", dt.Compute("Sum(total)", ""));
        if (Sum_Total == "") Sum_Total = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";

        string Sum_AmountReconciled = String.Format("{0:C}", dt.Compute("Sum(amount_reconciled)", ""));
        if (Sum_AmountReconciled == "") Sum_AmountReconciled = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";


        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");

        sb.Append("\"" + Sum_Total            + "\"").Append(",");
        sb.Append("\"" + Sum_AmountReconciled + "\"").Append(",");

        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");
        sb.Append("\"" + "" + "\"").Append(",");

        sb.AppendLine();

        ExportCSV(Response, sb.ToString(), "ReceiptsReport.csv");
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