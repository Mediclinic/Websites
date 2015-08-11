using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class EmailHistoryData
{

    public EmailHistoryData(int email_history_id, int sms_and_email_type_id, int patient_id, int booking_id, string email, string message, DateTime datetime_sent)
    {
        this.email_history_id   = email_history_id;
        this.sms_and_email_type = new IDandDescr(sms_and_email_type_id);
        this.patient            = patient_id == -1 ? null : new Patient(patient_id);
        this.booking            = booking_id == -1 ? null : new Booking(booking_id);
        this.email              = email;
        this.message            = message;
        this.datetime_sent      = datetime_sent;
    }

    private int email_history_id;
    public int EmailHistoryID
    {
        get { return this.email_history_id; }
        set { this.email_history_id = value; }
    }
    private IDandDescr sms_and_email_type;
    public IDandDescr SmsAndEmailType
    {
        get { return this.sms_and_email_type; }
        set { this.sms_and_email_type = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return patient; }
        set { this.patient = value; }
    }

    private Booking booking;
    public Booking BookingID
    {
        get { return this.booking; }
        set { this.booking = value; }
    }
    private string email;
    public string Email
    {
        get { return this.email; }
        set { this.email = value; }
    }
    private string message;
    public string Message
    {
        get { return this.message; }
        set { this.message = value; }
    }
    private DateTime datetime_sent;
    public DateTime DatetimeSent
    {
        get { return this.datetime_sent; }
        set { this.datetime_sent = value; }
    }
    public override string ToString()
    {
        return email_history_id.ToString() + " " + booking.BookingID.ToString() + " " + email.ToString() + " " + message.ToString() + " " + datetime_sent.ToString();
    }

}