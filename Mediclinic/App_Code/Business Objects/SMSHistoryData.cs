using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SMSHistoryData
{

    public SMSHistoryData(int sms_history_id, int sms_and_email_type_id, int patient_id, int booking_id, string phone_number, string message, decimal cost, DateTime datetime_sent, string smstech_message_id,
                string smstech_status, DateTime smstech_datetime)
    {
        this.sms_history_id     = sms_history_id;
        this.sms_and_email_type = new IDandDescr(sms_and_email_type_id);
        this.patient            = patient_id == -1 ? null : new Patient(patient_id);
        this.booking            = booking_id == -1 ? null : new Booking(booking_id);
        this.phone_number       = phone_number;
        this.cost               = cost;
        this.message            = message;
        this.datetime_sent      = datetime_sent;
        this.smstech_message_id = smstech_message_id;
        this.smstech_status     = smstech_status;
        this.smstech_datetime   = smstech_datetime;
    }

    private int sms_history_id;
    public int SmsHistoryID
    {
        get { return this.sms_history_id; }
        set { this.sms_history_id = value; }
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
    public Booking Booking
    {
        get { return this.booking; }
        set { this.booking = value; }
    }
    private string phone_number;
    public string PhoneNumber
    {
        get { return this.phone_number; }
        set { this.phone_number = value; }
    }
    private string message;
    public string Message
    {
        get { return this.message; }
        set { this.message = value; }
    }
    private decimal cost;
    public decimal Cost
    {
        get { return this.cost; }
        set { this.cost = value; }
    }
    private DateTime datetime_sent;
    public DateTime DatetimeSent
    {
        get { return this.datetime_sent; }
        set { this.datetime_sent = value; }
    }
    private string smstech_message_id;
    public string SmstechMessageID
    {
        get { return this.smstech_message_id; }
        set { this.smstech_message_id = value; }
    }
    private string smstech_status;
    public string SmstechStatus
    {
        get { return this.smstech_status; }
        set { this.smstech_status = value; }
    }
    private DateTime smstech_datetime;
    public DateTime SmstechDatetime
    {
        get { return this.smstech_datetime; }
        set { this.smstech_datetime = value; }
    }
    public override string ToString()
    {
        return sms_history_id.ToString() + " " + booking.BookingID.ToString() + " " + phone_number.ToString() + " " + message.ToString() + " " + datetime_sent.ToString() + " " +
                smstech_message_id.ToString() + " " + smstech_status.ToString() + " " + smstech_datetime.ToString();
    }

}