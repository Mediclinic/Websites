using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class TyroHealthClaimItem
{

    public TyroHealthClaimItem(
        int tyro_health_claim_item_id, 
        int tyro_health_claim_id, 
        decimal out_claimAmount, 
        decimal out_rebateAmount, 
        string out_serviceCode, 
        string out_description, 
        string out_serviceReference, 
        string out_patientId, 
        DateTime out_serviceDate, 
        string out_responseCodeString)
    {
        this.tyro_health_claim_item_id  = tyro_health_claim_item_id;
        this.tyro_health_claim          = tyro_health_claim_id == -1 ? null : new TyroHealthClaim(tyro_health_claim_id);
        this.out_claimAmount            = out_claimAmount;
        this.out_rebateAmount           = out_rebateAmount;
        this.out_serviceCode            = out_serviceCode;
        this.out_description            = out_description;
        this.out_serviceReference       = out_serviceReference;
        this.out_patientId              = out_patientId;
        this.out_serviceDate            = out_serviceDate;
        this.out_responseCodeString     = out_responseCodeString;    }
    public TyroHealthClaimItem(int tyro_health_claim_item_id)
    {
        this.tyro_health_claim_item_id  = tyro_health_claim_item_id;
    }

    private int tyro_health_claim_item_id;
    public int TyroHealthClaimItemID
    {
        get { return this.tyro_health_claim_item_id; }
        set { this.tyro_health_claim_item_id = value; }
    }
    private TyroHealthClaim tyro_health_claim;
    public TyroHealthClaim TyroHealthClaim
    {
        get { return this.tyro_health_claim; }
        set { this.tyro_health_claim = value; }
    }
    private decimal out_claimAmount;
    public decimal OutClaimAmount
    {
        get { return this.out_claimAmount; }
        set { this.out_claimAmount = value; }
    }
    private decimal out_rebateAmount;
    public decimal OutRebateAmount
    {
        get { return this.out_rebateAmount; }
        set { this.out_rebateAmount = value; }
    }
    private string out_serviceCode;
    public string OutServiceCode
    {
        get { return this.out_serviceCode; }
        set { this.out_serviceCode = value; }
    }
    private string out_description;
    public string OutDescription
    {
        get { return this.out_description; }
        set { this.out_description = value; }
    }
    private string out_serviceReference;
    public string OutServiceReference
    {
        get { return this.out_serviceReference; }
        set { this.out_serviceReference = value; }
    }
    private string out_patientId;
    public string OutPatientID
    {
        get { return this.out_patientId; }
        set { this.out_patientId = value; }
    }
    private DateTime out_serviceDate;
    public DateTime OutServiceDate
    {
        get { return this.out_serviceDate; }
        set { this.out_serviceDate = value; }
    }
    private string out_responseCodeString;
    public string OutResponseCodeString
    {
        get { return this.out_responseCodeString; }
        set { this.out_responseCodeString = value; }
    }
    public override string ToString()
    {
        return tyro_health_claim_item_id.ToString() + " " + tyro_health_claim.TyroHealthClaimID.ToString() + " " + out_claimAmount.ToString() + " " + out_rebateAmount.ToString() + " " + out_serviceCode.ToString() + " " + 
                out_description.ToString() + " " + out_serviceReference.ToString() + " " + out_patientId.ToString() + " " + out_serviceDate.ToString() + " " + out_responseCodeString.ToString();
    }

}