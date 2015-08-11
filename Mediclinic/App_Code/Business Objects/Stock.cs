using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Stock
{

    public Stock(int stock_id, int organisation_id, int offering_id, int qty, int warning_amt)
    {
        this.stock_id     = stock_id;
        this.organisation = new Organisation(organisation_id);
        this.offering     = new Offering(offering_id);
        this.qty          = qty;
        this.warning_amt  = warning_amt;
    }

    private int stock_id;
    public int StockID
    {
        get { return this.stock_id; }
        set { this.stock_id = value; }
    }
    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private Offering offering;
    public Offering Offering
    {
        get { return this.offering; }
        set { this.offering = value; }
    }
    private int qty;
    public int Quantity
    {
        get { return this.qty; }
        set { this.qty = value; }
    }
    private int warning_amt;
    public int WarningAmount
    {
        get { return this.warning_amt; }
        set { this.warning_amt = value; }
    }
    public override string ToString()
    {
        return stock_id.ToString() + " " + organisation.OrganisationID.ToString() + " " + offering.OfferingID.ToString() + " " + qty.ToString() + " " + warning_amt.ToString();
    }

}