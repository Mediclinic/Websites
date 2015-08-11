using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Receipt
{

    public Receipt(int receipt_id, int receipt_payment_type_id, int invoice_id, decimal total, decimal amount_reconciled,
                bool is_failed_to_clear, bool is_overpaid, DateTime receipt_date_added, DateTime reconciliation_date, int staff_id, 
                int reversed_by, DateTime reversed_date, decimal pre_reversed_amount)
    {
        this.receipt_id           = receipt_id;
        this.receipt_payment_type = new IDandDescr(receipt_payment_type_id);
        this.invoice              = new Invoice(invoice_id);
        this.total                = total;
        this.amount_reconciled    = amount_reconciled;
        this.is_failed_to_clear   = is_failed_to_clear;
        this.is_overpaid          = is_overpaid;
        this.receipt_date_added   = receipt_date_added;
        this.reconciliation_date  = reconciliation_date;
        this.staff                = new Staff(staff_id);
        this.reversed_by          = reversed_by == -1 ? null : new Staff(reversed_by);
        this.reversed_date        = reversed_date;
        this.pre_reversed_amount  = pre_reversed_amount;
    }
    public Receipt(int receipt_id)
    {
        this.receipt_id = receipt_id;
    }

    private int receipt_id;
    public int ReceiptID
    {
        get { return this.receipt_id; }
        set { this.receipt_id = value; }
    }
    private IDandDescr receipt_payment_type;
    public IDandDescr ReceiptPaymentType
    {
        get { return this.receipt_payment_type; }
        set { this.receipt_payment_type = value; }
    }
    private Invoice invoice;
    public Invoice Invoice
    {
        get { return this.invoice; }
        set { this.invoice = value; }
    }
    private decimal total;
    public decimal Total
    {
        get { return this.total; }
        set { this.total = value; }
    }
    private DateTime receipt_date_added;
    public DateTime ReceiptDateAdded
    {
        get { return this.receipt_date_added; }
        set { this.receipt_date_added = value; }
    }
    public bool IsReconciled
    {
        // can not go by reconciled date since old data has this set when inv created
        // and can only reconcile once, so doesn't need to be = invoice total, only bet more than zero

        get { return AmountReconciled > 0; }
    }
    private decimal amount_reconciled;
    public decimal AmountReconciled
    {
        get { return this.amount_reconciled; }
        set { this.amount_reconciled = value; }
    }
    private DateTime reconciliation_date;
    public DateTime ReconciliationDate
    {
        get { return this.reconciliation_date; }
        set { this.reconciliation_date = value; }
    }
    private bool is_failed_to_clear;
    public bool IsFailedToClear
    {
        get { return this.is_failed_to_clear; }
        set { this.is_failed_to_clear = value; }
    }
    private bool is_overpaid;
    public bool IsOverpaid
    {
        get { return this.is_overpaid; }
        set { this.is_overpaid = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    public bool IsReversed
    {
        get { return this.total == 0; }
    }
    private Staff reversed_by;
    public Staff ReversedBy
    {
        get { return this.reversed_by; }
        set { this.reversed_by = value; }
    }
    private DateTime reversed_date;
    public DateTime ReversedDate
    {
        get { return this.reversed_date; }
        set { this.reversed_date = value; }
    }
    private decimal pre_reversed_amount;
    public decimal PreReversedAmount
    {
        get { return this.pre_reversed_amount; }
        set { this.pre_reversed_amount = value; }
    }
    public override string ToString()
    {
        return receipt_id.ToString() + " " + receipt_payment_type.ID.ToString() + " " + invoice.InvoiceID.ToString() + " " + total.ToString() + " " +
                amount_reconciled.ToString() + " " + is_failed_to_clear.ToString() + " " + is_overpaid.ToString() + " " + receipt_date_added.ToString() + " " + reconciliation_date.ToString() + " " +
                staff.StaffID.ToString() + " " + reversed_by.StaffID.ToString() + " " + reversed_date.ToString() + " " + pre_reversed_amount.ToString();
    }



    public string GetViewPopupLink(string js_to_call = null, bool only_js = false)
    {
        System.Drawing.Size size = GetPopupWindowViewSize();
        return GetPopupLink(this.ReceiptID.ToString(), "AddEditReceipt.aspx?type=view_only&id=" + this.ReceiptID, size.Width, size.Height, js_to_call, only_js);
    }
    public string GetReconcilePopupLink(string js_to_call = null, bool only_js = false)
    {
        System.Drawing.Size size = GetPopupWindowReconcileSize();
        return GetPopupLink("Reconcile", "AddEditReceipt.aspx?type=reconcile&id=" + this.ReceiptID, size.Width, size.Height, js_to_call, only_js, "Reconciling will mean you can no longer reverse the receipt and therefore the invoice. \\r\\nAre you sure that you want to reconcile this receipt?");
    }
    public static string GetAddReceiptPopupLink(int invoice_id, string js_to_call = null, bool only_js = false)
    {
        System.Drawing.Size size = GetPopupWindowAddSize();
        return GetPopupLink("Add Payment", "AddReceiptsAndCreditNotes.aspx?id=" + invoice_id, size.Width, size.Height, js_to_call, only_js);
    }
    public string GetViewPopupLinkV2(string js_to_call = null, bool only_js = false)
    {
        System.Drawing.Size size = GetPopupWindowViewSize();
        return GetPopupLink(this.ReceiptID.ToString(), "Invoice_ReceiptDetailV2.aspx?type=view_only&id=" + this.ReceiptID, size.Width, size.Height, js_to_call, only_js);
    }
    public string GetReconcilePopupLinkV2(string js_to_call = null, bool only_js = false)
    {
        System.Drawing.Size size = GetPopupWindowReconcileSize();
        return GetPopupLink("Reconcile", "Invoice_ReceiptDetailV2.aspx?type=reconcile&id=" + this.ReceiptID, size.Width, size.Height, js_to_call, only_js, "Reconciling will mean you can no longer reverse the receipt and therefore the invoice. \\r\\nAre you sure that you want to reconcile this receipt?");
    }
    public static string GetAddReceiptPopupLinkV2(int invoice_id, string text = "Add Payment", string js_to_call = null, bool only_js = false)
    {
        System.Drawing.Size size = GetPopupWindowAddSize();
        return GetPopupLink(text, "Invoice_ReceiptAndCreditNoteAddV2.aspx?id=" + invoice_id, size.Width, size.Height, js_to_call, only_js);
    }
    private static string GetPopupLink(string text, string url, int width, int height, string js_to_call = null, bool only_js = false, string confirm_message = null)
    {
        string js = "javascript:" + (confirm_message != null ? " if (!confirm('" + confirm_message + "')) return false; " : "") + "window.showModalDialog('" + url + @"', '', 'dialogWidth:" + width + @"px;dialogHeight:" + height + @"px;center:yes;resizable:no; scroll:no');" + (js_to_call != null ? js_to_call : "") + @"return false;";
        string onclick = "onclick=\"" + js + "\"";

        return only_js ? js : "<a " + onclick + " href=\"\">" + text + "</a>";
    }


    public static System.Drawing.Size GetPopupWindowViewSize()
    {
        return new System.Drawing.Size(425, 515);
    }
    public static System.Drawing.Size GetPopupWindowReconcileSize()
    {
        return new System.Drawing.Size(425, 500);
    }
    public static System.Drawing.Size GetPopupWindowAddSize()
    {
        return new System.Drawing.Size(540, 800);
    }



}