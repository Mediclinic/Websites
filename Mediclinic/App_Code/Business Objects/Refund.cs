using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Refund
{

    public Refund(int refund_id, int invoice_id, decimal total, int refund_reason_id, string comment, DateTime refund_date_added, int staff_id)
    {
        this.refund_id         = refund_id;
        this.invoice           = new Invoice(invoice_id);
        this.total             = total;
        this.refund_reason     = new IDandDescr(refund_reason_id);
        this.comment           = comment;
        this.refund_date_added = refund_date_added;
        this.staff             = new Staff(staff_id);
    }

    private int refund_id;
    public int RefundID
    {
        get { return this.refund_id; }
        set { this.refund_id = value; }
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
    private IDandDescr refund_reason;
    public IDandDescr RefundReason
    {
        get { return this.refund_reason; }
        set { this.refund_reason = value; }
    }
    private string comment;
    public string Comment
    {
        get { return this.comment; }
        set { this.comment = value; }
    }
    private DateTime refund_date_added;
    public DateTime RefundDateAdded
    {
        get { return this.refund_date_added; }
        set { this.refund_date_added = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    public override string ToString()
    {
        return refund_id.ToString() + " " + invoice.InvoiceID.ToString() + " " + total.ToString() + " " + refund_reason.ID.ToString() + " " + comment.ToString() + " " +
                refund_date_added.ToString() + " " + staff.StaffID.ToString();
    }



    public string GetViewPopupLink(string js_to_call = null)
    {
        return GetPopupLink(this.RefundID.ToString(), "AddEditRefund.aspx?type=view&id=" + this.RefundID, js_to_call);
    }
    public static string GetAddPopupLink(int refund_id, string js_to_call = null)
    {
        return GetPopupLink("Add Refund", "AddEditRefund.aspx?type=add&id=" + refund_id, js_to_call);
    }
    public string GetViewPopupLinkV2(string js_to_call = null)
    {
        return GetPopupLink(this.RefundID.ToString(), "Invoice_RefundDetailV2.aspx?type=view&id=" + this.RefundID, js_to_call);
    }
    public static string GetAddPopupLinkV2(int refund_id, string js_to_call = null)
    {
        return GetPopupLink("Add Refund", "Invoice_RefundDetailV2.aspx?type=add&id=" + refund_id, js_to_call);
    }
    private static string GetPopupLink(string text, string url, string js_to_call = null)
    {
        string onclick = @"onclick=""javascript:window.showModalDialog('" + url + @"', '', 'dialogWidth:425px;dialogHeight:400px;center:yes;resizable:no; scroll:no');" + (js_to_call != null ? js_to_call : "") + @"return false;""";
        return "<a " + onclick + " href=\"\">" + text + "</a>";
    }

}