using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class TyroPaymentPending
{

    public TyroPaymentPending(int tyro_payment_pending_id, int invoice_id, string tyro_transaction_id, 
        int tyro_payment_type_id, decimal amount, decimal cashout, DateTime date_added, 
        DateTime out_date_processed,       string out_result,             string out_cardType, 
        string   out_transactionReference, string out_authorisationCode,  string out_issuerActionCode)
    {
        this.tyro_payment_pending_id    = tyro_payment_pending_id;
        this.invoice_id                 = invoice_id;
        this.tyro_transaction_id        = tyro_transaction_id;
        this.tyro_payment_type_id       = tyro_payment_type_id;
        this.amount                     = amount;
        this.cashout                    = cashout;
        this.date_added                 = date_added;
        this.out_date_processed         = out_date_processed;
        this.out_result                 = out_result;
        this.out_cardType               = out_cardType;
        this.out_transactionReference   = out_transactionReference;
        this.out_authorisationCode      = out_authorisationCode;
        this.out_issuerActionCode       = out_issuerActionCode;
    }

    private int tyro_payment_pending_id;
    public int TyroPaymentPendingID
    {
        get { return this.tyro_payment_pending_id; }
        set { this.tyro_payment_pending_id = value; }
    }
    private int invoice_id;
    public int InvoiceID
    {
        get { return this.invoice_id; }
        set { this.invoice_id = value; }
    }
    private string tyro_transaction_id;
    public string TyroTransactionID
    {
        get { return this.tyro_transaction_id; }
        set { this.tyro_transaction_id = value; }
    }
    private int tyro_payment_type_id;
    public int TyroPaymentTypeID
    {
        get { return this.tyro_payment_type_id; }
        set { this.tyro_payment_type_id = value; }
    }
    private decimal amount;
    public decimal Amount
    {
        get { return this.amount; }
        set { this.amount = value; }
    }
    private decimal cashout;
    public decimal Cashout
    {
        get { return this.cashout; }
        set { this.cashout = value; }
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
    private string out_result;
    public string OutResult
    {
        get { return this.out_result; }
        set { this.out_result = value; }
    }
    private string out_cardType;
    public string OutCardType
    {
        get { return this.out_cardType; }
        set { this.out_cardType = value; }
    }
    private string out_transactionReference;
    public string OutTransactionReference
    {
        get { return this.out_transactionReference; }
        set { this.out_transactionReference = value; }
    }
    private string out_authorisationCode;
    public string OutAuthorisationCode
    {
        get { return this.out_authorisationCode; }
        set { this.out_authorisationCode = value; }
    }
    private string out_issuerActionCode;
    public string OutIssuerActionCode
    {
        get { return this.out_issuerActionCode; }
        set { this.out_issuerActionCode = value; }
    }
    public override string ToString()
    {
        return tyro_payment_pending_id.ToString() + " " + invoice_id.ToString() + " " + tyro_transaction_id.ToString() + " " + tyro_payment_type_id.ToString() + " " + amount.ToString() + " " + 
                cashout.ToString() + " " + date_added.ToString() + " " + out_date_processed.ToString() + " " + out_result.ToString() + " " + out_cardType.ToString() + " " + 
                out_transactionReference.ToString() + " " + out_authorisationCode.ToString() + " " + out_issuerActionCode.ToString();
    }

}