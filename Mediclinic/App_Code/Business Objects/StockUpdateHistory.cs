using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class StockUpdateHistory
{

    public StockUpdateHistory(int stock_update_history_id, int organisation_id, int offering_id, int qty_added, bool is_created, bool is_deleted, int added_by, DateTime date_added)
    {
        this.stock_update_history_id = stock_update_history_id;
        this.organisation = new Organisation(organisation_id);
        this.offering     = new Offering(offering_id);
        this.qty_added    = qty_added;

        this.is_created = is_created;
        this.is_deleted = is_deleted;

        this.added_by     = added_by == -1 ? null : new Staff(added_by);
        this.date_added   = date_added;
    }

    private int stock_update_history_id;
    public int StockUpdateHistoryID
    {
        get { return this.stock_update_history_id; }
        set { this.stock_update_history_id = value; }
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
    private int qty_added;
    public int QuantityAdded
    {
        get { return this.qty_added; }
        set { this.qty_added = value; }
    }
    private bool is_created;
    public bool IsCreated
    {
        get { return this.is_created; }
        set { this.is_created = value; }
    }
    private bool is_deleted;
    public bool IsDeleted
    {
        get { return this.is_deleted; }
        set { this.is_deleted = value; }
    }
    private Staff added_by;
    public Staff AddedBy
    {
        get { return this.added_by; }
        set { this.added_by = value; }
    }
    private DateTime date_added;
    public DateTime DateAdded
    {
        get { return this.date_added; }
        set { this.date_added = value; }
    }
    public override string ToString()
    {
        return stock_update_history_id.ToString() + " " + organisation.OrganisationID.ToString() + " " + offering.OfferingID.ToString() + " " + qty_added.ToString() + " " + added_by.StaffID.ToString();
    }

}