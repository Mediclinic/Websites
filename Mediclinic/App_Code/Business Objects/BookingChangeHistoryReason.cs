using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BookingChangeHistoryReason
{

    public BookingChangeHistoryReason(int booking_change_history_reason_id, string descr, int display_order)
    {
        this.booking_change_history_reason_id = booking_change_history_reason_id;
        this.descr = descr;
        this.display_order = display_order;
    }

    private int booking_change_history_reason_id;
    public int BookingChangeHistoryReasonID
    {
        get { return this.booking_change_history_reason_id; }
        set { this.booking_change_history_reason_id = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    private int display_order;
    public int DisplayOrder
    {
        get { return this.display_order; }
        set { this.display_order = value; }
    }
    public override string ToString()
    {
        return booking_change_history_reason_id.ToString() + " " + descr.ToString() + " " + display_order.ToString();
    }

}