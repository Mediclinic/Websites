using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CreditNote
{

    public CreditNote(int creditnote_id, int invoice_id, decimal total, string reason, DateTime credit_note_date_added, int staff_id, 
                int reversed_by, DateTime reversed_date, decimal pre_reversed_amount)
    {
        this.creditnote_id          = creditnote_id;
        this.invoice                = new Invoice(invoice_id);
        this.total                  = total;
        this.reason                 = reason;
        this.credit_note_date_added = credit_note_date_added;
        this.staff                  = new Staff(staff_id);
        this.reversed_by            = reversed_by == -1 ? null : new Staff(reversed_by);
        this.reversed_date          = reversed_date;
        this.pre_reversed_amount    = pre_reversed_amount;
    }

    private int creditnote_id;
    public int CreditNoteID
    {
        get { return this.creditnote_id; }
        set { this.creditnote_id = value; }
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
    private string reason;
    public string Reason
    {
        get { return this.reason; }
        set { this.reason = value; }
    }
    private DateTime credit_note_date_added;
    public DateTime CreditNoteDateAdded
    {
        get { return this.credit_note_date_added; }
        set { this.credit_note_date_added = value; }
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
        return creditnote_id.ToString() + " " + invoice.InvoiceID.ToString() + " " + total.ToString() + " " + reason.ToString() + " " + credit_note_date_added.ToString() + " " +
                staff.StaffID.ToString() + " " + reversed_by.StaffID.ToString() + " " + reversed_date.ToString() + " " + pre_reversed_amount.ToString();
    }


    public string GetViewPopupLink(string js_to_call = null, bool only_js = false)
    {
        return GetPopupLink(this.CreditNoteID.ToString(), "AddEditCreditNote.aspx?type=view&id=" + this.CreditNoteID, js_to_call, only_js);
    }
    public static string GetAddCreditNotePopupLink(int invoice_id, string js_to_call = null, bool only_js = false)
    {
        return GetPopupLink("Add Adjustment Note", "AddEditCreditNote.aspx?type=add&id=" + invoice_id, js_to_call, only_js);
    }
    public string GetViewPopupLinkV2(string js_to_call = null, bool only_js = false)
    {
        return GetPopupLink(this.CreditNoteID.ToString(), "Invoice_CreditNoteDetailV2.aspx?type=view&id=" + this.CreditNoteID, js_to_call, only_js);
    }
    public static string GetAddCreditNotePopupLinkV2(int invoice_id, string js_to_call = null, bool only_js = false)
    {
        return GetPopupLink("Add Adjustment Note", "Invoice_CreditNoteDetailV2.aspx?type=add&id=" + invoice_id, js_to_call, only_js);
    }
    private static string GetPopupLink(string text, string url, string js_to_call = null, bool only_js = false)
    {
        string js = "javascript:window.showModalDialog('" + url + @"', '', 'dialogWidth:375px;dialogHeight:375px;center:yes;resizable:no;scroll:no');" + (js_to_call != null ? js_to_call : "") + @"return false;";
        string onclick = "onclick=\"" + js + "\"";

        return only_js ? js : "<a " + onclick + " href=\"\">" + text + "</a>";
    }

}