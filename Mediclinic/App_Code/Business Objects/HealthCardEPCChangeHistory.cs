using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class HealthCardEPCChangeHistory
{

    public HealthCardEPCChangeHistory(int health_card_epc_change_history_id, int health_card_id, int staff_id, DateTime date, bool is_new_epc_card_set,
                                        DateTime pre_date_referral_signed,  DateTime pre_date_referral_received_in_office,
                                        DateTime post_date_referral_signed, DateTime post_date_referral_received_in_office)
    {
        this.health_card_epc_change_history_id = health_card_epc_change_history_id;
        this.healthCard = new HealthCard(health_card_id);
        this.staff = new Staff(staff_id);
        this.date = date;

        this.is_new_epc_card_set = is_new_epc_card_set;

        this.pre_date_referral_signed = pre_date_referral_signed;
        this.pre_date_referral_received_in_office = pre_date_referral_received_in_office;
        this.post_date_referral_signed = post_date_referral_signed;
        this.post_date_referral_received_in_office = post_date_referral_received_in_office;
    }

    private int health_card_epc_change_history_id;
    public int HealthCardEpcChangeHistoryID
    {
        get { return this.health_card_epc_change_history_id; }
        set { this.health_card_epc_change_history_id = value; }
    }
    private HealthCard healthCard;
    public HealthCard HealthCard
    {
        get { return this.healthCard; }
        set { this.healthCard = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    private DateTime date;
    public DateTime Date
    {
        get { return this.date; }
        set { this.date = value; }
    }
    private bool is_new_epc_card_set;
    public bool IsNewEpcCardSet
    {
        get { return this.is_new_epc_card_set; }
        set { this.is_new_epc_card_set = value; }
    }
    private DateTime pre_date_referral_signed;
    public DateTime PreDateReferralSigned
    {
        get { return this.pre_date_referral_signed; }
        set { this.pre_date_referral_signed = value; }
    }
    private DateTime pre_date_referral_received_in_office;
    public DateTime PreDateReferralReceivedInOffice
    {
        get { return this.pre_date_referral_received_in_office; }
        set { this.pre_date_referral_received_in_office = value; }
    }
    private DateTime post_date_referral_signed;
    public DateTime PostDateReferralSigned
    {
        get { return this.post_date_referral_signed; }
        set { this.post_date_referral_signed = value; }
    }
    private DateTime post_date_referral_received_in_office;
    public DateTime PostDateReferralReceivedInOffice
    {
        get { return this.post_date_referral_received_in_office; }
        set { this.post_date_referral_received_in_office = value; }
    }
    public override string ToString()
    {
        return health_card_epc_change_history_id.ToString() + " " + healthCard.HealthCardID.ToString() + " " + staff.StaffID.ToString() + " " + date.ToString() + " " +
                pre_date_referral_signed.ToString() + " " + pre_date_referral_received_in_office.ToString() + " " +
                post_date_referral_received_in_office.ToString();
    }
}