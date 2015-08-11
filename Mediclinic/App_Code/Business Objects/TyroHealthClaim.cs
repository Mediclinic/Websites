using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class TyroHealthClaim
{

    public TyroHealthClaim(
        int      tyro_health_claim_id, 
        int      invoice_id, 
        string   tyro_transaction_id, 
        decimal  amount, 
        DateTime date_added, 
        DateTime out_date_processed, 
        string   out_result, 
        string   out_healthpointRefTag, 
        decimal  out_healthpointTotalBenefitAmount, 
        DateTime out_healthpointSettlementDateTime, 
        DateTime out_healthpointTerminalDateTime, 
        string   out_healthpointMemberNumber, 
        string   out_healthpointProviderId, 
        string   out_healthpointServiceType, 
        decimal  out_healthpointGapAmount, 
        string   out_healthpointPhfResponseCode, 
        string   out_healthpointPhfResponseCodeDescription,
        DateTime date_cancelled)
    {
        this.tyro_health_claim_id                       = tyro_health_claim_id;
        this.invoice_id                                 = invoice_id;
        this.tyro_transaction_id                        = tyro_transaction_id;
        this.amount                                     = amount;
        this.date_added                                 = date_added;
        this.out_date_processed                         = out_date_processed;
        this.out_result                                 = out_result;
        this.out_healthpointRefTag                      = out_healthpointRefTag;
        this.out_healthpointTotalBenefitAmount          = out_healthpointTotalBenefitAmount;
        this.out_healthpointSettlementDateTime          = out_healthpointSettlementDateTime;
        this.out_healthpointTerminalDateTime            = out_healthpointTerminalDateTime;
        this.out_healthpointMemberNumber                = out_healthpointMemberNumber;
        this.out_healthpointProviderId                  = out_healthpointProviderId;
        this.out_healthpointServiceType                 = out_healthpointServiceType;
        this.out_healthpointGapAmount                   = out_healthpointGapAmount;
        this.out_healthpointPhfResponseCode             = out_healthpointPhfResponseCode;
        this.out_healthpointPhfResponseCodeDescription  = out_healthpointPhfResponseCodeDescription;
        this.date_cancelled                             = date_cancelled;
    }
    public TyroHealthClaim(int tyro_health_claim_id)
    {
        this.tyro_health_claim_id = tyro_health_claim_id;
    }

    private int tyro_health_claim_id;
    public int TyroHealthClaimID
    {
        get { return this.tyro_health_claim_id; }
        set { this.tyro_health_claim_id = value; }
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
    private decimal amount;
    public decimal Amount
    {
        get { return this.amount; }
        set { this.amount = value; }
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
    private string out_healthpointRefTag;
    public string OutHealthpointRefTag
    {
        get { return this.out_healthpointRefTag; }
        set { this.out_healthpointRefTag = value; }
    }
    private decimal out_healthpointTotalBenefitAmount;
    public decimal OutHealthpointTotalBenefitAmount
    {
        get { return this.out_healthpointTotalBenefitAmount; }
        set { this.out_healthpointTotalBenefitAmount = value; }
    }
    private DateTime out_healthpointSettlementDateTime;
    public DateTime OutHealthpointSettlementDateTime
    {
        get { return this.out_healthpointSettlementDateTime; }
        set { this.out_healthpointSettlementDateTime = value; }
    }
    private DateTime out_healthpointTerminalDateTime;
    public DateTime OutHealthpointTerminalDateTime
    {
        get { return this.out_healthpointTerminalDateTime; }
        set { this.out_healthpointTerminalDateTime = value; }
    }
    private string out_healthpointMemberNumber;
    public string OutHealthpointMemberNumber
    {
        get { return this.out_healthpointMemberNumber; }
        set { this.out_healthpointMemberNumber = value; }
    }
    private string out_healthpointProviderId;
    public string OutHealthpointProviderID
    {
        get { return this.out_healthpointProviderId; }
        set { this.out_healthpointProviderId = value; }
    }
    private string out_healthpointServiceType;
    public string OutHealthpointServiceType
    {
        get { return this.out_healthpointServiceType; }
        set { this.out_healthpointServiceType = value; }
    }
    private decimal out_healthpointGapAmount;
    public decimal OutHealthpointGapAmount
    {
        get { return this.out_healthpointGapAmount; }
        set { this.out_healthpointGapAmount = value; }
    }
    private string out_healthpointPhfResponseCode;
    public string OutHealthpointPhfResponseCode
    {
        get { return this.out_healthpointPhfResponseCode; }
        set { this.out_healthpointPhfResponseCode = value; }
    }
    private string out_healthpointPhfResponseCodeDescription;
    public string OutHealthpointPhfResponseCodeDescription
    {
        get { return this.out_healthpointPhfResponseCodeDescription; }
        set { this.out_healthpointPhfResponseCodeDescription = value; }
    }
    private DateTime date_cancelled;
    public DateTime DateCancelled
    {
        get { return this.date_cancelled; }
        set { this.date_cancelled = value; }
    }
        

    public override string ToString()
    {
        return tyro_health_claim_id.ToString() + " " + invoice_id.ToString() + " " + tyro_transaction_id.ToString() + " " + amount.ToString() + " " + date_added.ToString() + " " + 
                out_date_processed.ToString() + " " + out_result.ToString() + " " + out_healthpointRefTag.ToString() + " " + out_healthpointTotalBenefitAmount.ToString() + " " + out_healthpointSettlementDateTime.ToString() + " " + 
                out_healthpointTerminalDateTime.ToString() + " " + out_healthpointMemberNumber.ToString() + " " + out_healthpointProviderId.ToString() + " " + out_healthpointServiceType.ToString() + " " + out_healthpointGapAmount.ToString() + " " + 
                out_healthpointPhfResponseCode.ToString() + " " + out_healthpointPhfResponseCodeDescription.ToString();
    }

}