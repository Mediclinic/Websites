using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BookingPatient
{

    public BookingPatient(int booking_patient_id, int booking_id, int patient_id, int entity_id, int offering_id, string area_treated, int added_by, DateTime added_date, bool is_deleted, int deleted_by, DateTime deleted_date,
                          bool need_to_generate_first_letter, bool need_to_generate_last_letter, bool has_generated_system_letters, int note_count)
    {
        this.booking_patient_id = booking_patient_id;
        this.booking            = booking_id  == -1 ? null : new Booking(booking_id);
        this.patient            = patient_id  == -1 ? null : new Patient(patient_id);
        this.entity_id          = entity_id;
        this.offering           = offering_id == -1 ? null : new Offering(offering_id);
        this.area_treated       = area_treated;
        this.added_by           = added_by    == -1 ? null : new Staff(added_by);
        this.added_date         = added_date;
        this.is_deleted         = is_deleted;
        this.deleted_by         = deleted_by  == -1 ? null : new Staff(deleted_by);
        this.deleted_date       = deleted_date;

        this.need_to_generate_first_letter = need_to_generate_first_letter;
        this.need_to_generate_last_letter  = need_to_generate_last_letter;
        this.has_generated_system_letters  = has_generated_system_letters;

        this.note_count = note_count;
    }
    public BookingPatient(int booking_patient_id)
    {
        this.booking_patient_id = booking_patient_id;
    }

    private int booking_patient_id;
    public int BookingPatientID
    {
        get { return this.booking_patient_id; }
        set { this.booking_patient_id = value; }
    }
    private Booking booking;
    public Booking Booking
    {
        get { return this.booking; }
        set { this.booking = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private int entity_id;
    public int EntityID
    {
        get { return this.entity_id; }
        set { this.entity_id = value; }
    }
    private Offering offering;
    public Offering Offering
    {
        get { return this.offering; }
        set { this.offering = value; }
    }
    private string area_treated;
    public string AreaTreated
    {
        get { return this.area_treated; }
        set { this.area_treated = value; }
    }
    private Staff added_by;
    public Staff AddedBy
    {
        get { return this.added_by; }
        set { this.added_by = value; }
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
    private DateTime added_date;
    public DateTime AddedDate
    {
        get { return this.added_date; }
        set { this.added_date = value; }
    }

    private bool need_to_generate_first_letter;
    public bool NeedToGenerateFirstLetter
    {
        get { return this.need_to_generate_first_letter; }
        set { this.need_to_generate_first_letter = value; }
    }
    private bool need_to_generate_last_letter;
    public bool NeedToGenerateLastLetter
    {
        get { return this.need_to_generate_last_letter; }
        set { this.need_to_generate_last_letter = value; }
    }
    private bool has_generated_system_letters;
    public bool HasGeneratedSystemLetters
    {
        get { return this.has_generated_system_letters; }
        set { this.has_generated_system_letters = value; }
    }

    private int note_count;
    public int NoteCount
    {
        get { return this.note_count; }
        set { this.note_count = value; }
    }

    public override string ToString()
    {
        return booking_patient_id.ToString() + " " + booking.BookingID.ToString() + " " + patient.PatientID.ToString() + " " + added_by.StaffID.ToString() + " " + added_date.ToString() + " " +
                is_deleted.ToString() + " " + deleted_by.StaffID.ToString() + " " + deleted_date.ToString();
    }

}