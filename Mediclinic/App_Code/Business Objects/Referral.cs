using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Referral
{

    public Referral(int referral_id, int health_card_id, int medical_service_type_id, int register_referrer_id, 
                DateTime date_referral_signed, DateTime date_referral_received_in_office, 
                int added_or_last_modified_by, DateTime added_or_last_modified_date, 
                int deleted_by, DateTime date_deleted)
    {
        this.referral_id                      = referral_id;
        this.health_card                      = new HealthCard(health_card_id);
        this.medical_service_type             = new IDandDescr(medical_service_type_id);
        this.registerReferrer                 = register_referrer_id == -1 ? null : new RegisterReferrer(register_referrer_id);
        this.date_referral_signed             = date_referral_signed;
        this.date_referral_received_in_office = date_referral_received_in_office;
        this.added_or_last_modified_by        = new Staff(added_or_last_modified_by);
        this.added_or_last_modified_date      = added_or_last_modified_date;
        this.deleted_by                       = deleted_by == -1 ? null : new Staff(deleted_by);
        this.date_deleted                     = date_deleted;
    }
    public Referral(int referral_id)
    {
        this.referral_id = referral_id;
    }

    private int referral_id;
    public int ReferralID
    {
        get { return this.referral_id; }
        set { this.referral_id = value; }
    }
    private HealthCard health_card;
    public HealthCard HealthCard
    {
        get { return this.health_card; }
        set { this.health_card = value; }
    }
    private IDandDescr medical_service_type;
    public IDandDescr MedicalServiceType
    {
        get { return this.medical_service_type; }
        set { this.medical_service_type = value; }
    }
    private RegisterReferrer registerReferrer;
    public RegisterReferrer RegisterReferrer
    {
        get { return this.registerReferrer; }
        set { this.registerReferrer = value; }
    }
    private DateTime date_referral_signed;
    public DateTime DateReferralSigned
    {
        get { return this.date_referral_signed; }
        set { this.date_referral_signed = value; }
    }
    private DateTime date_referral_received_in_office;
    public DateTime DateReferralReceivedInOffice
    {
        get { return this.date_referral_received_in_office; }
        set { this.date_referral_received_in_office = value; }
    }
    private Staff added_or_last_modified_by;
    public Staff AddedOrLastModifiedBy
    {
        get { return this.added_or_last_modified_by; }
        set { this.added_or_last_modified_by = value; }
    }
    private DateTime added_or_last_modified_date;
    public DateTime AddedOrLastModifiedDate
    {
        get { return this.added_or_last_modified_date; }
        set { this.added_or_last_modified_date = value; }
    }
    private Staff deleted_by;
    public  Staff DeletedBy
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
    public override string ToString()
    {
        return referral_id.ToString() + " " + health_card.ToString() + " " + medical_service_type.ID.ToString() + " " + date_referral_signed.ToString() + " " + date_referral_received_in_office.ToString() + " " + 
                added_or_last_modified_by.ToString() + " " + added_or_last_modified_date.ToString() + " " + deleted_by.ToString() + " " + date_deleted.ToString();
    }

}