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

public partial class CreditDetailV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, true, false);

                SetupGUI();

                UrlParamType urlParamType = GetUrlParamType();
                if ((urlParamType == UrlParamType.Edit || urlParamType == UrlParamType.View) && IsValidFormID())
                    FillEditViewForm(urlParamType == UrlParamType.Edit);
                else if (GetUrlParamType() == UrlParamType.Add && IsValidFormID() && GetUrlParamCreditType() != UrlParamCreditType.None)
                    FillEmptyAddForm();
                else
                    HideTableAndSetErrorMessage("", "Invalid URL Parameters");
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

    #region GetFormID, GetUrlParamType, GetUrlParamCreditType

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new Exception("Invalid url id");

        string id = Request.QueryString["id"];
        return Convert.ToInt32(id);
    }

    private enum UrlParamType { Add, Edit, View, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "add")
            return UrlParamType.Add;
        else if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "view")
            return UrlParamType.View;
        else
            return UrlParamType.None;
    }

    private enum UrlParamCreditType { Add = 1, Use = 2, CashoutTyroToMC = 3, CashoutMCtoPT, None };
    private UrlParamCreditType GetUrlParamCreditType()
    {
        string type = Request.QueryString["ct"];
        if (type != null && type.ToLower() == "1")
            return UrlParamCreditType.Add;
        if (type != null && type.ToLower() == "2")
            return UrlParamCreditType.Use;
        if (type != null && type.ToLower() == "3")
            return UrlParamCreditType.CashoutTyroToMC;
        if (type != null && type.ToLower() == "4")
            return UrlParamCreditType.CashoutMCtoPT;
        else
            return UrlParamCreditType.None;
    }

    #endregion

    #region SetupGUI()

    public void SetupGUI()
    {
        if (Utilities.IsMobileDevice(Request))
            hiddenIsMobileDevice.Value = "1";

        ddlExpiry_Day.Items.Add(new ListItem("--", "-1"));
        ddlExpiry_Month.Items.Add(new ListItem("--", "-1"));
        ddlExpiry_Year.Items.Add(new ListItem("----", "-1"));

        for (int i = 1; i <= 31; i++)
            ddlExpiry_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1; i <= 12; i++)
            ddlExpiry_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 2015; i <= DateTime.Today.Year + 5; i++)
            ddlExpiry_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));


        bool editable = GetUrlParamType() == UrlParamType.Add || GetUrlParamType() == UrlParamType.Edit;
        Utilities.SetEditControlBackColour(txtAmount,       editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtDescr,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlExpiry_Day,   editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlExpiry_Month, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlExpiry_Year,  editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }

    #endregion

    #region FillEditViewForm, FillEmptyAddForm, btnCancel_Click, btnSubmit_Click

    private void FillEditViewForm(bool isEditMode)
    {
        Credit credit = CreditDB.GetByID(GetFormID());
        if (credit == null)
        {
            HideTableAndSetErrorMessage("Invalid ID");
            return;
        }
       
        FillCreditGrid(credit);

        string heading = "";
        if (credit.CreditType.ID == 1)
            heading = "Voucher";
        if (credit.CreditType.ID == 2)
            heading = "Use Of Voucher";
        if (credit.CreditType.ID == 3)
            heading = "Cashout - Tyro To Mediclinic";
        if (credit.CreditType.ID == 4)
            heading = "Cashout - Mediclinic To Patient";
        lblHeading.Text = isEditMode ? "Edit " + heading : "View " + heading;

        if (isEditMode && credit.CreditType.ID != 1)
            throw new CustomMessageException("Can no edit a '" + heading + "'");

        lblId.Text      = credit.CreditID.ToString();
        lblType.Text    = credit.CreditType.Descr;
        idRow.Visible   = Utilities.IsDev();
        typeRow.Visible = Utilities.IsDev();

        bool isMobileDevice = Utilities.IsMobileDevice(Request);

        lblInvoice.Text = GetInvoiceLink(isMobileDevice, credit);
        lblVoucher.Text = GetVoucherLink(isMobileDevice, credit);

        lblAddedBy.Text          = credit.AddedBy.Person.FullnameWithoutMiddlename;
        lblDateAdded.Text        = credit.DateAdded.ToString("d MMM, yyyy");
        lblModifiedBy.Text       = credit.ModifiedBy == null ? "" : credit.ModifiedBy.Person.FullnameWithoutMiddlename;
        lblDateModified.Text     = credit.DateModified == DateTime.MinValue  ? "" : credit.DateModified.ToString("d MMM, yyyy");
        lblDeletedBy.Text        = credit.DeletedBy == null ? string.Empty : credit.DeletedBy.Person.FullnameWithoutMiddlename;
        lblDateDeleted.Text      = credit.DateDeleted  == DateTime.MinValue  ? "" : credit.DateDeleted.ToString("d MMM, yyyy");
        lblPreDeletedAmount.Text = ((credit.CreditType.ID == 1 || credit.CreditType.ID == 3 ? 1 : -1) * credit.PreDeletedAmount).ToString();

        bool isDeleted = credit.DeletedBy != null || credit.DateDeleted != DateTime.MinValue;
        if (!isDeleted)
        {
            deletedSpaceRow.Visible     = false;
            deletedbyRow.Visible        = false;
            dateDeletedRow.Visible      = false;
            preDeletedAmountRow.Visible = false;
        }

        bool isModified = credit.ModifiedBy != null || credit.DateModified != DateTime.MinValue;
        if (!isModified)
        {
            modifiedbyRow.Visible       = false;
            dateModifiedRow.Visible     = false;
        }

        invoiceListSpaceRow.Visible = false;
        invoiceListRow.Visible      = false;
        amountUsedRow.Visible       = false;
        amountRemainingRow.Visible  = false;

        if (credit.CreditType.ID == 1)
        {
            Credit[] payments = CreditDB.GetByVoucherCreditID(credit.CreditID);
            if (payments.Length > 0)
            {
                string invoicesText = string.Empty;
                foreach (Credit payment in payments)
                {
                    string invoiceLink = GetInvoiceLink(isMobileDevice, payment);
                    if (invoiceLink != null && invoiceLink.Length > 0)
                        invoicesText += (invoicesText.Length == 0 ? "" : "<br />") + invoiceLink + " $" + (-1 * payment.Amount);
                }

                if (invoicesText.Length > 0)
                {
                    invoiceListSpaceRow.Visible = true;
                    invoiceListRow.Visible      = true;
                    lblInvoicesUsingThisVoucher.Text = invoicesText;
                }
            }


            decimal totalUsed = CreditDB.GetTotalUsed(credit.CreditID);
            amountUsedRow.Visible      = true;
            amountRemainingRow.Visible = true;
            lblAmountUsed.Text         = (-1 * totalUsed).ToString();
            lblRemainingUsed.Text      = (credit.Amount + totalUsed).ToString();
        }



        if (isEditMode)
        {
            txtAmount.Text           = credit.Amount.ToString();
            txtDescr.Text            = credit.VoucherDescr;

            if (credit.ExpiryDate != DateTime.MinValue)
            {
                ddlExpiry_Day.SelectedValue = credit.ExpiryDate.Day.ToString();
                ddlExpiry_Month.SelectedValue = credit.ExpiryDate.Month.ToString();
                ddlExpiry_Year.SelectedValue = credit.ExpiryDate.Year.ToString();
            }

            lblAmount.Visible        = false;
            lblDescr.Visible         = false;
            lblExpiry.Visible        = false;
        }
        else
        {
            lblAmount.Text           = ((credit.CreditType.ID == 1 || credit.CreditType.ID == 3 ? 1 : -1) * credit.Amount).ToString();
            lblDescr.Text            = credit.VoucherDescr;
            lblExpiry.Text           = credit.ExpiryDate == DateTime.MinValue ? "No Date Set" : credit.ExpiryDate.ToString("d MMM, yyyy");

            txtAmount.Visible        = false;
            txtDescr.Visible         = false;
            ddlExpiry_Day.Visible    = false;
            ddlExpiry_Month.Visible  = false;
            ddlExpiry_Year.Visible   = false;
        }


        if (credit.CreditType.ID != 1)
        {
            descrRow.Visible         = false;
            expiryRow.Visible        = false;
        }
        if (credit.CreditType.ID != 2)
        {
            voucherUsedRow.Visible   = false;
            invoiceRow.Visible       = false;
        }

        btnSubmit.Text = isEditMode ? "Update Details" : "Edit Details";
        btnCancel.Text = isEditMode ? "Cancel" : "Close";

        btnSubmit.Visible = !isDeleted && credit.CreditType.ID == 1;
    }
    protected string GetInvoiceLink(bool isMobileDevice, Credit credit)
    {
        string invoiceLink    = "Invoice_ViewV2.aspx?invoice_id=" + credit.InvoiceID;
        string invoiceOnclick = isMobileDevice ? 
            "open_new_tab('"+invoiceLink+"');return false;" :
            "window.showModalDialog('" + invoiceLink + "', 'Show Popup Window', 'dialogWidth:775px;dialogHeight:725px;center:yes;resizable:no; scroll:no;');return false;";
        return credit.InvoiceID == -1 ? "" : "<a href=\"" + invoiceLink + "\" onclick=\"" + invoiceOnclick + "\">" + credit.InvoiceID + "</a>";
    }
    protected string GetVoucherLink(bool isMobileDevice, Credit credit)
    {
        string voucherLink = "CreditDetailV2.aspx?ct=1&type=view&id=" + credit.VoucherCredit.CreditID;
        string voucherOnclick = isMobileDevice ?
            "open_new_tab('" + voucherLink + "');return false;" :
            "window.showModalDialog('" + voucherLink + "', 'Show Popup Window', 'dialogWidth:500px;dialogHeight:600px;center:yes;resizable:no; scroll:no;');return false;";

        string descr              = credit.VoucherCredit == null || credit.VoucherCredit.VoucherDescr == null ? "" : credit.VoucherCredit.VoucherDescr;
        string descrShortened     = descr.Length > 42 ? descr.Substring(0, 40) + ".." : descr;
        return credit.VoucherCredit == null ? "" : "<a href=\"" + voucherOnclick + "\" onclick=\"" + voucherOnclick + "\">" + descrShortened + "</a> ($" + credit.VoucherCredit.Amount + ")";

    }

    private void FillEmptyAddForm()
    {
        UrlParamCreditType urlParamCreditType = GetUrlParamCreditType();
        if (urlParamCreditType != UrlParamCreditType.Add)
            throw new CustomMessageException("Can no add a '" + GetUrlParamCreditType().ToString() + "'");


        string heading = "";
        if (urlParamCreditType == UrlParamCreditType.Add)
            heading = "Voucher";
        if (urlParamCreditType == UrlParamCreditType.Use)
            heading = "Use Of Voucher";
        if (urlParamCreditType == UrlParamCreditType.CashoutTyroToMC)
            heading = "Cashout - Tyro To Mediclinic";
        if (urlParamCreditType == UrlParamCreditType.CashoutMCtoPT)
            heading = "Cashout - Mediclinic To Patient";
        lblHeading.Text = "Add " + heading;

        lblAmount.Visible           = false;
        lblDescr.Visible            = false;
        lblExpiry.Visible           = false;

        idRow.Visible               = false;
        typeRow.Visible             = false;
        amountUsedRow.Visible       = false;
        amountRemainingRow.Visible  = false;
        addedbyRow.Visible          = false;
        dateAddedRow.Visible        = false;
        modifiedbyRow.Visible       = false;
        dateModifiedRow.Visible     = false;
        deletedSpaceRow.Visible     = false;
        deletedbyRow.Visible        = false;
        dateDeletedRow.Visible      = false;
        preDeletedAmountRow.Visible = false;

        invoiceListSpaceRow.Visible = false;
        invoiceListRow.Visible      = false;


        if (urlParamCreditType != UrlParamCreditType.Add)
        {
            descrRow.Visible = false;
            expiryRow.Visible = false;
        }
        if (urlParamCreditType != UrlParamCreditType.Use)
        {
            voucherUsedRow.Visible = false;
            invoiceRow.Visible = false;
        }

        btnSubmit.Text = "Add " + urlParamCreditType;
        btnCancel.Visible = true;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (GetUrlParamType() == UrlParamType.Edit)
        {
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
            return;
        }

        // close this window
        Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {

        if (GetUrlParamType() == UrlParamType.View)
        {
            maintable.Visible = false; // hide this so that we don't send all the page data (all suburbs, etc) to display before it redirects
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "edit"));
        }
        else if (GetUrlParamType() == UrlParamType.Edit)
        {
            try
            {
                UrlParamCreditType urlParamCreditType = GetUrlParamCreditType();

                if (urlParamCreditType != UrlParamCreditType.Add)
                    throw new CustomMessageException("Can no edit a '" + GetUrlParamCreditType().ToString() + "'");

                if (!ddlExpiryValidateAllOrNoneSet.IsValid)
                    return;

                Credit credit = CreditDB.GetByID(GetFormID());

                txtAmount.Text = txtAmount.Text.Trim();
                if (txtAmount.Text.StartsWith("$")) txtAmount.Text = txtAmount.Text.Substring(1);
                decimal amount;
                if (!decimal.TryParse(txtAmount.Text, out amount))
                    throw new CustomMessageException("Amount must be a valid amount.");

                if (urlParamCreditType == UrlParamCreditType.Add)
                {
                    CreditDB.Update(credit.CreditID, credit.CreditType.ID, credit.EntityID, amount, txtDescr.Text.Trim(), GetExpiryFromForm(), credit.VoucherCredit == null ? -1 : credit.VoucherCredit.CreditID, credit.InvoiceID, credit.TyroPaymentPendingID, Convert.ToInt32(Session["StaffID"]));
                }


                Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.Message);
            }
        }
        else if (GetUrlParamType() == UrlParamType.Add)
        {
            try
            {
                UrlParamCreditType urlParamCreditType = GetUrlParamCreditType();

                if (urlParamCreditType != UrlParamCreditType.Add)
                    throw new CustomMessageException("Can no add a '" + GetUrlParamCreditType().ToString() + "'");

                if (!ddlExpiryValidateAllOrNoneSet.IsValid)
                    return;

                int entityID = GetFormID();

                txtAmount.Text = txtAmount.Text.Trim();
                if (txtAmount.Text.StartsWith("$")) txtAmount.Text = txtAmount.Text.Substring(1);
                decimal amount;
                if (!decimal.TryParse(txtAmount.Text, out amount))
                    throw new CustomMessageException("Amount must be a valid amount.");

                int credit_type_id = -1;
                if (urlParamCreditType == UrlParamCreditType.Add)
                    credit_type_id = 1;
                else if (urlParamCreditType == UrlParamCreditType.Use)
                    credit_type_id = 2;
                else if (urlParamCreditType == UrlParamCreditType.CashoutTyroToMC)
                    credit_type_id = 3;
                else if (urlParamCreditType == UrlParamCreditType.CashoutMCtoPT)
                    credit_type_id = 4;
                else
                    throw new CustomMessageException("Invalid URL Field ct");
                

                if (urlParamCreditType == UrlParamCreditType.Add)
                {
                    CreditDB.Insert_AddVoucher(entityID, amount, txtDescr.Text.Trim(), GetExpiryFromForm(), Convert.ToInt32(Session["StaffID"]));
                }



                // close this window

                maintable.Visible = false; 

                bool refresh_on_close = Request.QueryString["refresh_on_close"] != null && Request.QueryString["refresh_on_close"] == "1";

                if (refresh_on_close)
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.opener.location.href=window.opener.location.href;self.close();</script>");
                else
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.Message);
            }
        }
        else
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
        }
    }

    #endregion
    
    #region ExpiryAllOrNoneCheck, GetDOBFromForm, GetDate, IsValidDate

    protected void ExpiryAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlExpiry_Day.SelectedValue, ddlExpiry_Month.SelectedValue, ddlExpiry_Year.SelectedValue);
    }
    public DateTime GetExpiryFromForm()
    {
        return GetDate(ddlExpiry_Day.SelectedValue, ddlExpiry_Month.SelectedValue, ddlExpiry_Year.SelectedValue, "Expiry Date");
    }
    public DateTime GetDate(string day, string month, string year, string fieldNme)
    {
        if (day == "-1" && month == "-1" && year == "-1")
            return DateTime.MinValue;

        else if (day != "-1" && month != "-1" && year != "-1")
            return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));

        else
            throw new Exception(fieldNme + " format is some selected and some not selected.");
    }
    public bool IsValidDate(string day, string month, string year)
    {
        bool invalid = ((day == "-1" || month == "-1" || year == "-1") && (day != "-1" || month != "-1" || year != "-1"));
        return !invalid;
    }

    #endregion

    #region GrdCredit

    protected void FillCreditGrid(Credit credit)
    {
        DataTable tbl = CreditHistoryDB.GetDataTable_ByCreditID(credit.CreditID);

        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            if (Convert.ToInt32(tbl.Rows[i]["credit_history_credit_type_id"]) == 2 || Convert.ToInt32(tbl.Rows[i]["credit_history_credit_type_id"]) == 4)
            {
                tbl.Rows[i]["credit_history_amount"] = (-1 * Convert.ToInt32(tbl.Rows[i]["credit_history_amount"])).ToString();
                tbl.Rows[i]["credit_history_pre_deleted_amount"] = (-1 * Convert.ToInt32(tbl.Rows[i]["credit_history_pre_deleted_amount"])).ToString();
            }
        }

        if (credit.CreditType.ID == 1 && tbl.Rows.Count > 0)
        {
            show_hide_history_link.Visible = true;

            GrdCredit.DataSource = tbl;
            GrdCredit.DataBind();
        }
    }

    protected void GrdCredit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
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