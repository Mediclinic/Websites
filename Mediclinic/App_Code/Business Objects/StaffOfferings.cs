using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class StaffOfferings
{

    public StaffOfferings(int staff_offering_id, int staff_id, int offering_id, bool is_commission, decimal commission_percent, bool is_fixed_rate, decimal fixed_rate, DateTime date_active)
    {
        this.staff_offering_id = staff_offering_id;
        this.staff = new Staff(staff_id);
        this.offering = new Offering(offering_id);
        this.is_commission = is_commission;
        this.commission_percent = commission_percent;
        this.is_fixed_rate = is_fixed_rate;
        this.fixed_rate = fixed_rate;
        this.date_active = date_active;
    }

    private int staff_offering_id;
    public int StaffOfferingID
    {
        get { return this.staff_offering_id; }
        set { this.staff_offering_id = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    private Offering offering;
    public Offering Offering
    {
        get { return this.offering; }
        set { this.offering = value; }
    }
    private bool is_commission;
    public bool IsCommission
    {
        get { return this.is_commission; }
        set { this.is_commission = value; }
    }
    private decimal commission_percent;
    public decimal CommissionPercent
    {
        get { return this.commission_percent; }
        set { this.commission_percent = value; }
    }
    private bool is_fixed_rate;
    public bool IsFixedRate
    {
        get { return this.is_fixed_rate; }
        set { this.is_fixed_rate = value; }
    }
    private decimal fixed_rate;
    public decimal FixedRate
    {
        get { return this.fixed_rate; }
        set { this.fixed_rate = value; }
    }
    private DateTime date_active;
    public DateTime DateActive
    {
        get { return this.date_active; }
        set { this.date_active = value; }
    }
    public override string ToString()
    {
        return staff_offering_id.ToString() + " " + staff.StaffID.ToString() + " " + offering.OfferingID.ToString() + " " + is_commission.ToString() + " " + commission_percent.ToString() + " " + is_fixed_rate.ToString() + " " + fixed_rate.ToString() + " " + date_active.ToString();
    }

}