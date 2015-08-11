using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SMSCreditData
{

    public SMSCreditData(int sms_credit_id, decimal amount, DateTime datetime_added)
    {
        this.sms_credit_id = sms_credit_id;
        this.amount = amount;
        this.datetime_added = datetime_added;
    }

    private int sms_credit_id;
    public int SMSCreditID
    {
        get { return this.sms_credit_id; }
        set { this.sms_credit_id = value; }
    }
    private decimal amount;
    public decimal Amount
    {
        get { return this.amount; }
        set { this.amount = value; }
    }
    private DateTime datetime_added;
    public DateTime DatetimeAdded
    {
        get { return this.datetime_added; }
        set { this.datetime_added = value; }
    }
    public override string ToString()
    {
        return sms_credit_id.ToString() + " " + amount.ToString() + " " + datetime_added.ToString();
    }

}