using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ReferralRemaining
{

    public ReferralRemaining(int referral_remaining_id, int referral_id, int field_id, int num_services_remaining, int deleted_by, DateTime date_deleted)
    {
        this.referral_remaining_id = referral_remaining_id;
        this.referral = new Referral(referral_id);
        this.field = new IDandDescr(field_id);
        this.num_services_remaining = num_services_remaining;
        this.deleted_by = deleted_by;
        this.date_deleted = date_deleted;
    }
    public ReferralRemaining(int referral_remaining_id)
    {
        this.referral_remaining_id = referral_remaining_id;
    }

    private int referral_remaining_id;
    public int ReferralRemainingID
    {
        get { return this.referral_remaining_id; }
        set { this.referral_remaining_id = value; }
    }
    private Referral referral;
    public Referral Referral
    {
        get { return this.referral; }
        set { this.referral = value; }
    }
    private IDandDescr field;
    public IDandDescr Field
    {
        get { return this.field; }
        set { this.field = value; }
    }
    private int num_services_remaining;
    public int NumServicesRemaining
    {
        get { return this.num_services_remaining; }
        set { this.num_services_remaining = value; }
    }
    private int deleted_by;
    public int DeletedBy
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
        return referral_remaining_id.ToString() + " " + referral.ToString() + " " + field.ID.ToString() + " " + num_services_remaining.ToString() + " " + deleted_by.ToString() + " " +
                date_deleted.ToString();
    }



    public static ReferralRemaining GetByOfferinSubtype(ReferralRemaining[] list, int fieldID)
    {
        for (int i = 0; i < list.Length; i++)
            if (list[i].Field.ID == fieldID)
                return list[i];

        return null;
    }



    public static ReferralRemaining[] CloneList(ReferralRemaining[] list)
    {
        ReferralRemaining[] retList = new ReferralRemaining[list.Length];
        for (int i = 0; i < list.Length; i++)
            retList[i] = list[i].Clone();
        return retList;
    }
    public ReferralRemaining Clone()
    {
        ReferralRemaining o = new ReferralRemaining(
            this.ReferralRemainingID,
            this.Referral.ReferralID,
            this.Field.ID,
            this.NumServicesRemaining,
            this.DeletedBy,
            this.DateDeleted);

        o.field.Descr = this.Field.Descr;

        return o;
    }

}