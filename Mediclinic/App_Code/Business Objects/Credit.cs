using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Credit
{

    public Credit(int credit_id, int credit_type_id, int entity_id, decimal amount, decimal amount_used, string voucher_descr, DateTime expiry_date, int voucher_credit_id, int invoice_id,
                int tyro_payment_pending_id, int added_by, DateTime date_added, int deleted_by, DateTime date_deleted, decimal pre_deleted_amount, int modified_by, DateTime date_modified)
    {
        this.credit_id               = credit_id;
        this.credit_type             = new IDandDescr(credit_type_id);
        this.entity_id               = entity_id;
        this.amount                  = amount;
        this.amount_used             = amount_used;
        this.voucher_descr           = voucher_descr;
        this.expiry_date             = expiry_date;
        this.voucher_credit          = voucher_credit_id == -1 ? null : new Credit(voucher_credit_id);
        this.invoice_id              = invoice_id;
        this.tyro_payment_pending_id = tyro_payment_pending_id;
        this.added_by                = new Staff(added_by);
        this.date_added              = date_added;
        this.deleted_by              = deleted_by == -1 ? null : new Staff(deleted_by);
        this.date_deleted            = date_deleted;
        this.pre_deleted_amount      = pre_deleted_amount;
        this.date_modified           = date_modified;
        this.modified_by             = modified_by == -1 ? null : new Staff(modified_by);
    }
    public Credit(int credit_id)
    {
        this.credit_id = credit_id;
    }

    private int credit_id;
    public int CreditID
    {
        get { return this.credit_id; }
        set { this.credit_id = value; }
    }
    private IDandDescr credit_type;
    public IDandDescr CreditType
    {
        get { return this.credit_type; }
        set { this.credit_type = value; }
    }
    private int entity_id;
    public int EntityID
    {
        get { return this.entity_id; }
        set { this.entity_id = value; }
    }
    private decimal amount;
    public decimal Amount
    {
        get { return this.amount; }
        set { this.amount = value; }
    }
    private decimal amount_used;
    public decimal AmountUsed
    {
        get { return this.amount_used; }
        set { this.amount_used = value; }
    }
    private string voucher_descr;
    public string VoucherDescr
    {
        get { return this.voucher_descr; }
        set { this.voucher_descr = value; }
    }
    private DateTime expiry_date;
    public DateTime ExpiryDate
    {
        get { return this.expiry_date; }
        set { this.expiry_date = value; }
    }
    private Credit voucher_credit;
    public Credit VoucherCredit
    {
        get { return this.voucher_credit; }
        set { this.voucher_credit = value; }
    }
    private int invoice_id;
    public int InvoiceID
    {
        get { return this.invoice_id; }
        set { this.invoice_id = value; }
    }
    private int tyro_payment_pending_id;
    public int TyroPaymentPendingID
    {
        get { return this.tyro_payment_pending_id; }
        set { this.tyro_payment_pending_id = value; }
    }
    private Staff added_by;
    public Staff AddedBy
    {
        get { return this.added_by; }
        set { this.added_by = value; }
    }
    private DateTime date_added;
    public DateTime DateAdded
    {
        get { return this.date_added; }
        set { this.date_added = value; }
    }
    public bool IsDeleted
    {
        get { return this.deleted_by != null ||  this.date_deleted != DateTime.MinValue; }
    }
    private Staff deleted_by;
    public Staff DeletedBy
    {
        get { return this.deleted_by; }
        set { this.deleted_by = value; }
    }
    private DateTime date_deleted;
    public DateTime DateDeleted
    {
        get { return this.date_deleted; }
        set { this.date_deleted = value; }
    }
    private decimal pre_deleted_amount;
    public decimal PreDeletedAmount
    {
        get { return this.pre_deleted_amount; }
        set { this.pre_deleted_amount = value; }
    }
    private Staff modified_by;
    public Staff ModifiedBy
    {
        get { return this.modified_by; }
        set { this.modified_by = value; }
    }
    private DateTime date_modified;
    public DateTime DateModified
    {
        get { return this.date_modified; }
        set { this.date_modified = value; }
    }
    public override string ToString()
    {
        return credit_id.ToString() + " " + credit_type.ID.ToString() + " " + entity_id.ToString() + " " + amount.ToString() + " " + voucher_descr.ToString() + " " +
                invoice_id.ToString() + " " + tyro_payment_pending_id.ToString() + " " + added_by.ToString() + " " + date_added.ToString() + " " + deleted_by.ToString() + " " +
                date_deleted.ToString() + " " + pre_deleted_amount.ToString();
    }


    public string GetViewVoucherUsePopupLinkV2(string js_to_call = null, bool only_js = false)
    {
        System.Drawing.Size size = GetPopupWindowViewSize();

        string descr              = this.VoucherCredit == null || this.VoucherCredit.VoucherDescr == null ? "" : this.VoucherCredit.VoucherDescr;
        string descrShortened     = descr.Length > 42 ? descr.Substring(0, 40) + ".." : descr;

        return GetPopupLink(descrShortened, "CreditDetailV2.aspx?type=view&id=" + this.CreditID, size.Width, size.Height, js_to_call, only_js);
    }
    public static string GetAddVoucherUsePopupLinkV2(int invoice_id, string js_to_call = null, bool only_js = false)
    {
        System.Drawing.Size size = GetPopupWindowAddSize();
        return GetPopupLink("Add Voucher", "CreditDetailV2.aspx?id=" + invoice_id, size.Width, size.Height, js_to_call, only_js);
    }
    private static string GetPopupLink(string text, string url, int width, int height, string js_to_call = null, bool only_js = false, string confirm_message = null)
    {
        string js = "javascript:" + (confirm_message != null ? " if (!confirm('" + confirm_message + "')) return false; " : "") + "window.showModalDialog('" + url + @"', '', 'dialogWidth:" + width + @"px;dialogHeight:" + height + @"px;center:yes;resizable:no; scroll:no');" + (js_to_call != null ? js_to_call : "") + @"return false;";
        string onclick = "onclick=\"" + js + "\"";

        return only_js ? js : "<a " + onclick + " href=\"\">" + text + "</a>";
    }


    public static System.Drawing.Size GetPopupWindowViewSize()
    {
        return new System.Drawing.Size(425, 565);
    }
    public static System.Drawing.Size GetPopupWindowReconcileSize()
    {
        return new System.Drawing.Size(425, 550);
    }
    public static System.Drawing.Size GetPopupWindowAddSize()
    {
        return new System.Drawing.Size(540, 705);
    }

}