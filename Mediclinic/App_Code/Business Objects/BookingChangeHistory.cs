using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BookingChangeHistory
{

    public BookingChangeHistory(int booking_change_history_id, int booking_id, int moved_by, DateTime date_moved, int booking_change_history_reason_id, DateTime previous_datetime)
    {
        this.booking_change_history_id = booking_change_history_id;
        this.booking = new Booking(booking_id);
        this.moved_by = moved_by;
        this.date_moved = date_moved;
        this.booking_change_history_reason = new IDandDescr(booking_change_history_reason_id);
        this.previous_datetime = previous_datetime;
    }

    private int booking_change_history_id;
    public int BookingChangeHistoryID
    {
        get { return this.booking_change_history_id; }
        set { this.booking_change_history_id = value; }
    }
    private Booking booking;
    public Booking Booking
    {
        get { return this.booking; }
        set { this.booking = value; }
    }
    private int moved_by;
    public int MovedBy
    {
        get { return this.moved_by; }
        set { this.moved_by = value; }
    }
    private DateTime date_moved;
    public DateTime DateMoved
    {
        get { return this.date_moved; }
        set { this.date_moved = value; }
    }
    private IDandDescr booking_change_history_reason;
    public IDandDescr BookingChangeHistoryReason
    {
        get { return this.booking_change_history_reason; }
        set { this.booking_change_history_reason = value; }
    }
    private DateTime previous_datetime;
    public DateTime PreviousDatetime
    {
        get { return this.previous_datetime; }
        set { this.previous_datetime = value; }
    }
    public override string ToString()
    {
        return booking_change_history_id.ToString() + " " + booking.BookingID.ToString() + " " + moved_by.ToString() + " " + date_moved.ToString() + " " + booking_change_history_reason.ID.ToString() + " " +
                previous_datetime.ToString();
    }

}