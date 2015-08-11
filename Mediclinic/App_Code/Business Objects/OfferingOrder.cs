using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class OfferingOrder
{

    public OfferingOrder(int offering_order_id, int offering_id, int organisation_id, int staff_id, int patient_id, int quantity,
                DateTime date_ordered, DateTime date_filled, DateTime date_cancelled, string descr)
    {
        this.offering_order_id = offering_order_id;
        this.offering          = new Offering(offering_id);
        this.organisation      = new Organisation(organisation_id);
        this.staff             = new Staff(staff_id);
        this.patient           = new Patient(patient_id);
        this.quantity          = quantity;
        this.date_ordered      = date_ordered;
        this.date_filled       = date_filled;
        this.date_cancelled    = date_cancelled;
        this.descr             = descr;
    }
    public OfferingOrder(int offering_order_id)
    {
        this.offering_order_id = offering_order_id;
    }

    private int offering_order_id;
    public int OfferingOrderID
    {
        get { return this.offering_order_id; }
        set { this.offering_order_id = value; }
    }
    private Offering offering;
    public Offering Offering
    {
        get { return this.offering; }
        set { this.offering = value; }
    }
    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private int quantity;
    public int Quantity
    {
        get { return this.quantity; }
        set { this.quantity = value; }
    }
    private DateTime date_ordered;
    public DateTime DateOrdered
    {
        get { return this.date_ordered; }
        set { this.date_ordered = value; }
    }
    private DateTime date_filled;
    public DateTime DateFilled
    {
        get { return this.date_filled; }
        set { this.date_filled = value; }
    }
    private DateTime date_cancelled;
    public DateTime DateCancelled
    {
        get { return this.date_cancelled; }
        set { this.date_cancelled = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    public override string ToString()
    {
        return offering_order_id.ToString() + " " + offering.OfferingID.ToString() + " " + organisation.OrganisationID.ToString() + " " + staff.StaffID.ToString() + " " + patient.PatientID.ToString() + " " + quantity.ToString() + " " +
                date_ordered.ToString() + " " + date_filled.ToString() + " " + date_cancelled.ToString() + " " + descr.ToString();
    }

}