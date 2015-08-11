using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class LetterPrintHistory
{

    public LetterPrintHistory(int letter_print_history_id, int letter_id, int letter_print_history_send_method_id,
                              int booking_id, int patient_id, int organisation_id, int register_referrer_id,
                              int staff_id, int health_card_action_id, DateTime date, string doc_name, bool has_doc)
    {
        this.letter_print_history_id = letter_print_history_id;
        this.letter                  = letter_id       == -1 ? null : new Letter(letter_id);
        this.send_method             = new IDandDescr(letter_print_history_send_method_id);
        this.booking                 = new Booking(booking_id);
        this.patient                 = patient_id           == -1 ? null : new Patient(patient_id);
        this.organisation            = organisation_id      ==  0 ? null : new Organisation(organisation_id);
        this.register_referrer       = register_referrer_id == -1 ? null : new RegisterReferrer(register_referrer_id);
        this.staff                   = staff_id             == -1 ? null : new Staff(staff_id);
        this.health_card_action      = new HealthCardAction(health_card_action_id);
        this.date                    = date;
        this.doc_name                = doc_name;
        this.has_doc                 = has_doc;
    }

    private int letter_print_history_id;
    public int LetterPrintHistoryID
    {
        get { return this.letter_print_history_id; }
        set { this.letter_print_history_id = value; }
    }
    private Letter letter;
    public Letter Letter
    {
        get { return this.letter; }
        set { this.letter = value; }
    }
    private IDandDescr send_method;
    public IDandDescr SendMethod
    {
        get { return this.send_method; }
        set { this.send_method = value; }
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
    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private RegisterReferrer register_referrer;
    public RegisterReferrer RegisterReferrer
    {
        get { return this.register_referrer; }
        set { this.register_referrer = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    private HealthCardAction health_card_action;
    public HealthCardAction HealthCardAction
    {
        get { return this.health_card_action; }
        set { this.health_card_action = value; }
    }
    private DateTime date;
    public DateTime Date
    {
        get { return this.date; }
        set { this.date = value; }
    }
    private string doc_name;
    public string DocName
    {
        get { return this.doc_name; }
        set { this.doc_name = value; }
    }
    private bool has_doc;
    public bool HasDoc
    {
        get { return this.has_doc; }
        set { this.has_doc = value; }
    }

    public override string ToString()
    {
        return letter_print_history_id.ToString() + " " + letter.LetterID.ToString() + " " + patient.PatientID.ToString() + " " + organisation.OrganisationID.ToString() + " " +
                register_referrer.RegisterReferrerID.ToString() + " " + staff.StaffID.ToString() + " " + date.ToString();
    }

}