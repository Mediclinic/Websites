using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BookingPatientOffering
{

    public BookingPatientOffering(int booking_patient_offering_id, int booking_patient_id, int offering_id, int quantity, int added_by, DateTime added_date,
                                  bool is_deleted, int deleted_by, DateTime deleted_date, string area_treated)
    {
        this.booking_patient_offering_id = booking_patient_offering_id;
        this.booking_patient             = booking_patient_id == -1 ? null : new BookingPatient(booking_patient_id);
        this.offering                    = offering_id        == -1 ? null : new Offering(offering_id);
        this.quantity                    = quantity;
        this.added_by                    = added_by           == -1 ? null : new Staff(added_by);
        this.added_date                  = added_date;
        this.is_deleted                  = is_deleted;
        this.deleted_by                  = deleted_by         == -1 ? null : new Staff(deleted_by);
        this.deleted_date                = deleted_date;
        this.area_treated                = area_treated;
    }

    private int booking_patient_offering_id;
    public int BookingPatientOfferingID
    {
        get { return this.booking_patient_offering_id; }
        set { this.booking_patient_offering_id = value; }
    }
    private BookingPatient booking_patient;
    public BookingPatient BookingPatient
    {
        get { return this.booking_patient; }
        set { this.booking_patient = value; }
    }
    private Offering offering;
    public Offering Offering
    {
        get { return this.offering; }
        set { this.offering = value; }
    }
    private int quantity;
    public int Quantity
    {
        get { return this.quantity; }
        set { this.quantity = value; }
    }
    private Staff added_by;
    public Staff AddedBy
    {
        get { return this.added_by; }
        set { this.added_by = value; }
    }
    private DateTime added_date;
    public DateTime AddedDate
    {
        get { return this.added_date; }
        set { this.added_date = value; }
    }
    private bool is_deleted;
    public bool IsDeleted
    {
        get { return this.is_deleted; }
        set { this.is_deleted = value; }
    }
    private Staff deleted_by;
    public Staff DeletedBy
    {
        get { return this.deleted_by; }
        set { this.deleted_by = value; }
    }
    private DateTime deleted_date;
    public DateTime DeletedDate
    {
        get { return this.deleted_date; }
        set { this.deleted_date = value; }
    }
    private string area_treated;
    public string AreaTreated
    {
        get { return this.area_treated; }
        set { this.area_treated = value; }
    }

    public override string ToString()
    {
        return booking_patient_offering_id.ToString() + " " + booking_patient.BookingPatientID.ToString() + " " + offering.OfferingID.ToString() + " " + quantity.ToString() + " " + added_by.StaffID.ToString() + " " + 
                added_date.ToString() + " " + is_deleted.ToString() + " " + deleted_by.StaffID.ToString() + " " + deleted_date.ToString();
    }

}