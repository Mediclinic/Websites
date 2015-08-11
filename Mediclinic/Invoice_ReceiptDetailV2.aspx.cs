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

public partial class Invoice_ReceiptDetailV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);

            if (!IsPostBack)
            {
                SetupGUI();

                UrlParamType urlParamType = GetUrlParamType();
                if ((urlParamType == UrlParamType.Edit || urlParamType == UrlParamType.Reconcile || urlParamType == UrlParamType.View || urlParamType == UrlParamType.ViewOnly) && IsValidFormID())
                    FillEditViewForm(urlParamType == UrlParamType.Edit, urlParamType == UrlParamType.Reconcile);
                else if (GetUrlParamType() == UrlParamType.Add && IsValidFormID())
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


    #region GetUrlParamCard(), GetUrlParamType(), IsValidFormID(), GetFormID()

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

    private enum UrlParamType { Add, Edit, View, ViewOnly, Reconcile, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "add")
            return UrlParamType.Add;

        /* 

        // receipts can not be "edited" (or viewed with button to "edit").
        // receipt can only be updated via reconciliation

        else if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "view")
            return UrlParamType.View;

        */

        else if (type != null && type.ToLower() == "view_only")
            return UrlParamType.ViewOnly;
        else if (type != null && type.ToLower() == "reconcile")
            return UrlParamType.Reconcile;
        else
            return UrlParamType.None;
    }

    #endregion

    #region SetupGUI()

    public void SetupGUI()
    {
        UrlParamType urlParamType = GetUrlParamType();

        DataTable paymentTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "ReceiptPaymentType", "", "descr", "receipt_payment_type_id,descr");
        ddlPaymentType.DataSource = paymentTypes;
        ddlPaymentType.DataBind();

        bool editable = GetUrlParamType() == UrlParamType.Add || GetUrlParamType() == UrlParamType.Edit || GetUrlParamType() == UrlParamType.Reconcile;
        Utilities.SetEditControlBackColour(ddlPaymentType,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtTotal,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtAmountReconciled, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }

    #endregion


    private void FillEditViewForm(bool isEditMode, bool isReconcileMode)
    {
        if (isEditMode)
        {
            lblHeading.Text = "Edit Receipt";
            Page.Title      = "Edit Receipt";
        }
        else if (isReconcileMode)
        {
            lblHeading.Text = "Reconcile Receipt";
            Page.Title      = "Reconcile Receipt";
        }
        else
        {
            lblHeading.Text = "View Receipt";
            Page.Title      = "View Receipt";
        }

        Receipt receipt = ReceiptDB.GetByID(GetFormID());
        if (receipt == null)
        {
            HideTableAndSetErrorMessage("Invalid receipt ID");
            return;
        }


        lblId.Text          = receipt.ReceiptID.ToString();
        lblInvoiceId.Text   = receipt.Invoice.InvoiceID.ToString();
        lblReceiptDate.Text = receipt.ReceiptDateAdded.ToString("d MMM, yyyy");
        lblIsOverpaid.Text  = receipt.IsOverpaid ? "Yes" : "No";
        lblAddedBy.Text     = receipt.Staff.Person.FullnameWithoutMiddlename;

        lblReceiptDate.Font.Bold = !isEditMode && !isReconcileMode;
        lblPaymentType.Font.Bold = !isEditMode && !isReconcileMode;
        lblTotal.Font.Bold       = !isEditMode && !isReconcileMode;
        lblIsOverpaid.Font.Bold  = !isEditMode && !isReconcileMode;


        if (isEditMode)
        {

            ddlPaymentType.SelectedValue = receipt.ReceiptPaymentType.ID.ToString();

            txtTotal.Text            = receipt.Total.ToString();
            txtAmountReconciled.Text = receipt.AmountReconciled.ToString();
            chkFailedToClear.Checked = receipt.IsFailedToClear;
            isReconciledRow.Visible  = false;
            lblReconciliationDate.Text = receipt.ReconciliationDate == DateTime.MinValue ? "--" : receipt.ReconciliationDate.ToString("d MMM, yyyy");

            lblPaymentType.Visible       = false;
            lblTotal.Visible             = false;
            lblAmountReconciled.Visible  = false;
            lblFailedToClear.Visible     = false;

        }
        else if (isReconcileMode)
        {

            if (receipt.IsReconciled)
            {
                HideTableAndSetErrorMessage("This receipt has already been reconciled.");
                return;
            }

            lblPaymentType.Text = receipt.ReceiptPaymentType.Descr;

            lblTotal.Text                 = receipt.Total.ToString();
            txtAmountReconciled.Text      = receipt.AmountReconciled == 0 ? receipt.Total.ToString() : receipt.AmountReconciled.ToString();
            chkFailedToClear.Checked      = receipt.IsFailedToClear;
            isReconciledRow.Visible       = false;
            reconciliationDateRow.Visible = false;

            ddlPaymentType.Visible        = false;
            txtTotal.Visible              = false;
            lblAmountReconciled.Visible   = false;
            lblFailedToClear.Visible      = false;
        
        }
        else
        {

            lblPaymentType.Text = receipt.ReceiptPaymentType.Descr;

            lblTotal.Text              = receipt.Total.ToString();
            lblAmountReconciled.Text   = receipt.AmountReconciled.ToString();
            lblFailedToClear.Text      = receipt.IsFailedToClear ? "Yes" : "No";
            lblReconciliationDate.Text = receipt.ReconciliationDate == DateTime.MinValue ? "--" : receipt.ReconciliationDate.ToString("d MMM, yyyy");
            lblIsReconciled.Text       = receipt.IsReconciled    ? "Yes" : "No";

            ddlPaymentType.Visible             = false;
            txtTotal.Visible                   = false;
            txtAmountReconciled.Visible        = false;
            chkFailedToClear.Visible           = false;

            if (receipt.IsReconciled)
            {
                isReconciledRow.Visible        = false;
            }
            else
            {
                reconciliationDateRow.Visible  = false;
                amountReconciledRow.Visible    = false;
                failedToClearRow.Visible       = false;
            }
        }



        if (isEditMode)
        {
            btnSubmit.Text = "Update Details";
        }
        else if (isReconcileMode)
        {
            btnSubmit.Text = "Reconcile";
        }
        else // is view mode
        {
            if (GetUrlParamType() == UrlParamType.ViewOnly)
                btnSubmit.Visible = false;
            else
                btnSubmit.Text = "Edit";

            btnCancel.Text = "Close";
        }
    }

    private void FillEmptyAddForm()
    {
        lblHeading.Text = "Add Receipt";
        Page.Title      = "Add Receipt";

        idRow.Visible = false;
        isReconciledRow.Visible = false;

        Invoice invoice = InvoiceDB.GetByID(GetFormID());
        if (invoice == null)
        {
            HideTableAndSetErrorMessage("Invalid invoice ID");
            return;
        }

        lblInvoiceId.Text = invoice.InvoiceID.ToString();

        if (invoice.TotalDue > 0)
            txtTotal.Text = invoice.TotalDue.ToString();

        amountReconciledRow.Visible   = false;
        failedToClearRow.Visible      = false;
        isOverPaidRow.Visible         = false;
        addedByRow.Visible            = false;
        receiptDateRow.Visible        = false;
        reconciliationDateRow.Visible = false;


        btnSubmit.Text = "Add Receipt";
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
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "edit"));
        }
        //else if (GetUrlParamType() == UrlParamType.Edit)
        //{
        //    if (!IsValidFormID())
        //    {
        //        HideTableAndSetErrorMessage();
        //        return;
        //    }
        //    Receipt receipt = ReceiptDB.GetByID(GetFormID());
        //    if (receipt == null)
        //    {
        //        HideTableAndSetErrorMessage("Invalid receipt ID");
        //        return;
        //    }

        //    ReceiptDB.Update(receipt.ReceiptID, Convert.ToInt32(ddlPaymentType.SelectedValue), Convert.ToDecimal(txtTotal.Text), Convert.ToDecimal(txtAmountReconciled.Text), chkFailedToClear.Checked, receipt.IsOverpaid, GetBankProcessedDateFromForm());

        //    Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view_only"));


        //    // close this window
        //    //Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
        //}

        else if (GetUrlParamType() == UrlParamType.Reconcile)
        {
            if (!IsValidFormID())
            {
                HideTableAndSetErrorMessage();
                return;
            }
            Receipt receipt = ReceiptDB.GetByID(GetFormID());
            if (receipt == null)
            {
                HideTableAndSetErrorMessage("Invalid receipt ID");
                return;
            }


            ReceiptDB.Update(receipt.ReceiptID, receipt.ReceiptPaymentType.ID, receipt.Total, Convert.ToDecimal(txtAmountReconciled.Text), chkFailedToClear.Checked, receipt.IsOverpaid, DateTime.Now, receipt.ReversedBy == null ? -1 : receipt.ReversedBy.StaffID, receipt.ReversedDate, receipt.PreReversedAmount);

            // close this window
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
        }

        else if (GetUrlParamType() == UrlParamType.Add)
        {
            if (!IsValidFormID())
            {
                HideTableAndSetErrorMessage();
                return;
            }
            Invoice invoice = InvoiceDB.GetByID(GetFormID());
            if (invoice == null)
            {
                HideTableAndSetErrorMessage("Invalid invoice ID");
                return;
            }

            decimal thisReceitptAmount = Convert.ToDecimal(txtTotal.Text);
            decimal totalOwed          = invoice.TotalDue - thisReceitptAmount;
            bool    isOverPaid         = totalOwed <  0;
            bool    isPaid             = totalOwed <= 0;
            int     receipt_id         = ReceiptDB.Insert(null, Convert.ToInt32(ddlPaymentType.SelectedValue), invoice.InvoiceID, thisReceitptAmount, Convert.ToDecimal(0.00), false, isOverPaid, DateTime.MinValue, Convert.ToInt32(Session["StaffID"]));

            if (isPaid)
                InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);
            if (isOverPaid)
                OverpaymentDB.Insert(receipt_id, -1 * totalOwed, Convert.ToInt32(Session["StaffID"]));


            //string url = Request.RawUrl;
            //url = UrlParamModifier.AddEdit(url, "type", "view_only");
            //url = UrlParamModifier.AddEdit(url, "id", receipt_id.ToString());
            //Response.Redirect(url);
            //return;

            // close this window
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
        }
        else
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
        }
    }


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