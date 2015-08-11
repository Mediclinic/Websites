using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

public class HealthCard
{

    public HealthCard(int health_card_id, int patient_id, int organisation_id, string card_name, string card_nbr, string card_family_member_nbr, DateTime expiry_date,
                DateTime date_referral_signed, DateTime date_referral_received_in_office, bool is_active,
                int added_or_last_modified_by, DateTime added_or_last_modified_date,
                string area_treated)
    {
        this.health_card_id                     = health_card_id;
        this.patient                            = new Patient(patient_id);
        this.organisation                       = new Organisation(organisation_id);
        this.card_name                          = card_name;
        this.card_nbr                           = card_nbr;
        this.card_family_member_nbr             = card_family_member_nbr;
        this.expiry_date                        = expiry_date;
        this.date_referral_signed               = date_referral_signed;
        this.date_referral_received_in_office   = date_referral_received_in_office;
        this.is_active                          = is_active;
        this.added_or_last_modified_by          = added_or_last_modified_by == -1 ? null : new Staff(added_or_last_modified_by);
        this.added_or_last_modified_date        = added_or_last_modified_date;
        this.area_treated                       = area_treated;
    }
    public HealthCard(int health_card_id)
    {
        this.health_card_id = health_card_id;
    }

    private int health_card_id;
    public int HealthCardID
    {
        get { return this.health_card_id; }
        set { this.health_card_id = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private string card_name;
    public string CardName
    {
        get { return this.card_name; }
        set { this.card_name = value; }
    }
    private string card_nbr;
    public string CardNbr
    {
        get { return this.card_nbr; }
        set { this.card_nbr = value; }
    }
    private string card_family_member_nbr;
    public string CardFamilyMemberNbr
    {
        get { return this.card_family_member_nbr; }
        set { this.card_family_member_nbr = value; }
    }
    private DateTime expiry_date;
    public DateTime ExpiryDate
    {
        get { return this.expiry_date; }
        set { this.expiry_date = value; }
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
    private bool is_active;
    public bool IsActive
    {
        get { return this.is_active; }
        set { this.is_active = value; }
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
    private string area_treated;
    public string AreaTreated
    {
        get { return this.area_treated; }
        set { this.area_treated = value; }
    }
    public override string ToString()
    {
        return health_card_id.ToString() + " " + patient.PatientID.ToString() + " " + organisation.OrganisationID.ToString() + " " + card_name.ToString() + " " + card_nbr.ToString() + " " +
                card_family_member_nbr.ToString() + " " + date_referral_signed.ToString() + " " +
                date_referral_received_in_office.ToString();
    }


    public bool HasEPC(Hashtable epcRemainingCache = null)
    {
        bool hasEpcRemainingRows = (epcRemainingCache == null) ?
            HealthCardEPCRemainingDB.GetCountByHealthCardID(this.HealthCardID) > 0 :
            epcRemainingCache[this.HealthCardID] != null && ((HealthCardEPCRemaining[])epcRemainingCache[this.HealthCardID]).Length > 0;

        return (this.DateReferralReceivedInOffice != DateTime.MinValue || this.DateReferralSigned != DateTime.MinValue || hasEpcRemainingRows);
    }

    public static bool ContainsMedicareCard(HealthCard[] cards)
    {
        foreach (HealthCard card in cards)
            if (card.Organisation.OrganisationID == -1)
                return true;
        return false;
    }
    public static bool ContainsDVACard(HealthCard[] cards)
    {
        foreach (HealthCard card in cards)
            if (card.Organisation.OrganisationID == -2)
                return true;
        return false;
    }
    public static HealthCard GetMedicareCard(HealthCard[] cards)
    {
        foreach (HealthCard card in cards)
            if (card.Organisation.OrganisationID == -1)
                return card;

        return null;
    }
    public static HealthCard GetDVACard(HealthCard[] cards)
    {
        foreach (HealthCard card in cards)
            if (card.Organisation.OrganisationID == -2)
                return card;

        return null;
    }
    
    



    public static System.Collections.ArrayList GetFullEPCChangeHistory(int health_card_id)
    {
        HealthCardEPCChangeHistory[] epcChangeHistoryList = HealthCardEPCChangeHistoryDB.GetByHealthCardID(health_card_id);
        HealthCardEPCRemainingChangeHistory[] epcRemainingChangeHistoryList = HealthCardEPCRemainingChangeHistoryDB.GetByHealthCardID(health_card_id);

        // merge lists in date order
        int posA = 0;
        int posB = 0;
        System.Collections.ArrayList list = new System.Collections.ArrayList();
        while (posA < epcChangeHistoryList.Length || posB < epcRemainingChangeHistoryList.Length)
        {
            if (posA == epcChangeHistoryList.Length)
            {
                list.Add(epcRemainingChangeHistoryList[posB]);
                posB++;
                continue;
            }
            if (posB == epcRemainingChangeHistoryList.Length)
            {
                list.Add(epcChangeHistoryList[posA]);
                posA++;
                continue;
            }



            if (RemoveMicroseconds(epcChangeHistoryList[posA].Date) <= RemoveMicroseconds(epcRemainingChangeHistoryList[posB].Date))
            {
                list.Add(epcChangeHistoryList[posA]);
                posA++;
                continue;
            }
            else
            {
                list.Add(epcRemainingChangeHistoryList[posB]);
                posB++;
                continue;
            }
        }

        return list;
    }
    protected static DateTime RemoveMicroseconds(DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
    }

}