using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class OrganisationOfferings
{

    public OrganisationOfferings(int organisation_offering_id, int organisation_id, int offering_id, decimal price, DateTime date_active)
    {
        this.organisation_offering_id = organisation_offering_id;
        this.organisation = new Organisation(organisation_id);
        this.offering = new Offering(offering_id);
        this.price = price;
        this.date_active = date_active;
    }

    private int organisation_offering_id;
    public int OrganisationOfferingID
    {
        get { return this.organisation_offering_id; }
        set { this.organisation_offering_id = value; }
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
    private decimal price;
    public decimal Price
    {
        get { return this.price; }
        set { this.price = value; }
    }
    private DateTime date_active;
    public DateTime DateActive
    {
        get { return this.date_active; }
        set { this.date_active = value; }
    }
    public override string ToString()
    {
        return organisation_offering_id.ToString() + " " + organisation.OrganisationID.ToString() + " " + offering.OfferingID.ToString() + " " + price.ToString() + " " + date_active.ToString();
    }

}