using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PaymentPending
{

    public PaymentPending(int payment_pending_id,  int invoice_id, decimal payment_amount, string customer_name, DateTime date_added, 
        DateTime out_date_processed,    string out_payment_result,  string out_payment_result_code, 
        string out_payment_result_text, string out_bank_receipt_id, string out_paytecht_payment_id)
    {
        this.payment_pending_id      = payment_pending_id;
        this.invoice_id              = invoice_id;
        this.payment_amount          = payment_amount;
        this.customer_name           = customer_name;
        this.date_added              = date_added;
        this.out_date_processed      = out_date_processed;
        this.out_payment_result      = out_payment_result;
        this.out_payment_result_code = out_payment_result_code;
        this.out_payment_result_text = out_payment_result_text;
        this.out_bank_receipt_id     = out_bank_receipt_id;
        this.out_paytecht_payment_id = out_paytecht_payment_id;
    }

    private int payment_pending_id;
    public int PaymentPendingID
    {
        get { return this.payment_pending_id; }
        set { this.payment_pending_id = value; }
    }
    private int invoice_id;
    public int InvoiceID
    {
        get { return this.invoice_id; }
        set { this.invoice_id = value; }
    }
    private decimal payment_amount;
    public decimal PaymentAmount
    {
        get { return this.payment_amount; }
        set { this.payment_amount = value; }
    }
    private string customer_name;
    public string CustomerName
    {
        get { return this.customer_name; }
        set { this.customer_name = value; }
    }
    private DateTime date_added;
    public DateTime DateAdded
    {
        get { return this.date_added; }
        set { this.date_added = value; }
    }
    private DateTime out_date_processed;
    public DateTime OutDateProcessed
    {
        get { return this.out_date_processed; }
        set { this.out_date_processed = value; }
    }
    private string out_payment_result;
    public string OutPaymentResult
    {
        get { return this.out_payment_result; }
        set { this.out_payment_result = value; }
    }
    private string out_payment_result_code;
    public string OutPaymentResultCode
    {
        get { return this.out_payment_result_code; }
        set { this.out_payment_result_code = value; }
    }
    private string out_payment_result_text;
    public string OutPaymentResultText
    {
        get { return this.out_payment_result_text; }
        set { this.out_payment_result_text = value; }
    }
    private string out_bank_receipt_id;
    public string OutBankReceiptID
    {
        get { return this.out_bank_receipt_id; }
        set { this.out_bank_receipt_id = value; }
    }
    private string out_paytecht_payment_id;
    public string OutPaytechtPaymentID
    {
        get { return this.out_paytecht_payment_id; }
        set { this.out_paytecht_payment_id = value; }
    }
    public override string ToString()
    {
        return payment_pending_id.ToString()  + " " + invoice_id.ToString()         + " " + payment_amount.ToString()          + " " + customer_name.ToString()           + " " + date_added.ToString()          + " " + 
                out_date_processed.ToString() + " " + out_payment_result.ToString() + " " + out_payment_result_code.ToString() + " " + out_payment_result_text.ToString() + " " + out_bank_receipt_id.ToString() + " " + 
                out_paytecht_payment_id.ToString();
    }

}