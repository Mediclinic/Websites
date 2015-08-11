using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Overpayment
{

    public Overpayment(int overpayment_id, int receipt_id, decimal total, DateTime overpayment_date_added, int staff_id)
    {
        this.overpayment_id = overpayment_id;
        this.receipt = new Receipt(receipt_id);
        this.total = total;
        this.overpayment_date_added = overpayment_date_added;
        this.staff = new Staff(staff_id);
    }

    private int overpayment_id;
    public int OverpaymentID
    {
        get { return this.overpayment_id; }
        set { this.overpayment_id = value; }
    }
    private Receipt receipt;
    public Receipt Receipt
    {
        get { return this.receipt; }
        set { this.receipt = value; }
    }
    private decimal total;
    public decimal Total
    {
        get { return this.total; }
        set { this.total = value; }
    }
    private DateTime overpayment_date_added;
    public DateTime OverpaymentDateAdded
    {
        get { return this.overpayment_date_added; }
        set { this.overpayment_date_added = value; }
    }
    private Staff staff;
    public Staff StaffID
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    public override string ToString()
    {
        return overpayment_id.ToString() + " " + receipt.ReceiptID.ToString() + " " + total.ToString() + " " + overpayment_date_added.ToString() + " " + staff.StaffID.ToString();
    }

}