using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class InvoiceLine
{

    public InvoiceLine(int invoice_line_id, int invoice_id, int patient_id, int offering_id, decimal quantity, decimal price, decimal tax, string area_treated, string service_reference, int offering_order_id)
    {
        this.invoice_line_id   = invoice_line_id;
        this.invoice_id        = invoice_id;
        this.patient           = new Patient(patient_id);
        this.offering          = offering_id == -1 ? null : new Offering(offering_id);
        this.quantity          = quantity;
        this.price             = price;
        this.tax               = tax;
        this.area_treated      = area_treated;
        this.service_reference = service_reference;
        this.offering_order    = offering_order_id == -1 ? null : new OfferingOrder(offering_order_id);
    }

    private int invoice_line_id;
    public int InvoiceLineID
    {
        get { return this.invoice_line_id; }
        set { this.invoice_line_id = value; }
    }
    private int invoice_id;
    public int InvoiceID
    {
        get { return this.invoice_id; }
        set { this.invoice_id = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private Offering offering;
    public Offering Offering
    {
        get { return this.offering; }
        set { this.offering = value; }
    }
    private decimal quantity;
    public decimal Quantity
    {
        get { return this.quantity; }
        set { this.quantity = value; }
    }
    private decimal price;
    public decimal Price
    {
        get { return this.price; }
        set { this.price = value; }
    }
    private decimal tax;
    public decimal Tax
    {
        get { return this.tax; }
        set { this.tax = value; }
    }
    private string area_treated;
    public string AreaTreated
    {
        get { return this.area_treated; }
        set { this.area_treated = value; }
    }
    private string service_reference;
    public string ServiceReference
    {
        get { return this.service_reference; }
        set { this.service_reference = value; }
    }
    private OfferingOrder offering_order;
    public OfferingOrder OfferingOrder
    {
        get { return this.offering_order; }
        set { this.offering_order = value;  }
    }

    public override string ToString()
    {
        return invoice_line_id.ToString() + " " + invoice_id.ToString() + " " + patient.PatientID.ToString() + " " + offering.OfferingID.ToString() + " " + quantity.ToString() + " " +
                price.ToString() + " " + tax.ToString();
    }

}