using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class HealthCardEPCRemainingChangeHistory
{

    public HealthCardEPCRemainingChangeHistory(int health_card_epc_remaining_change_history_id, int health_card_epc_remaining_id, int staff_id, DateTime date, int pre_num_services_remaining, int post_num_services_remaining)
    {
        this.health_card_epc_remaining_change_history_id = health_card_epc_remaining_change_history_id;
        this.health_card_epc_remaining = new HealthCardEPCRemaining(health_card_epc_remaining_id);
        this.staff = new Staff(staff_id);
        this.date = date;
        this.pre_num_services_remaining = pre_num_services_remaining;
        this.post_num_services_remaining = post_num_services_remaining;
    }

    private int health_card_epc_remaining_change_history_id;
    public int HealthCardEpcRemainingChangeHistoryID
    {
        get { return this.health_card_epc_remaining_change_history_id; }
        set { this.health_card_epc_remaining_change_history_id = value; }
    }
    private HealthCardEPCRemaining health_card_epc_remaining;
    public HealthCardEPCRemaining HealthCardEpcRemaining
    {
        get { return this.health_card_epc_remaining; }
        set { this.health_card_epc_remaining = value; }
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
    private int pre_num_services_remaining;
    public int PreNumServicesRemaining
    {
        get { return this.pre_num_services_remaining; }
        set { this.pre_num_services_remaining = value; }
    }
    private int post_num_services_remaining;
    public int PostNumServicesRemaining
    {
        get { return this.post_num_services_remaining; }
        set { this.post_num_services_remaining = value; }
    }
    public override string ToString()
    {
        return health_card_epc_remaining_change_history_id.ToString() + " " + health_card_epc_remaining.HealthCardEpcRemainingID.ToString() + " " + staff.StaffID.ToString() + " " + date.ToString() + " " + pre_num_services_remaining.ToString() + " " +
                post_num_services_remaining.ToString();
    }

}