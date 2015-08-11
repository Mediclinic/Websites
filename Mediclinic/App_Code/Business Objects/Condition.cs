using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Condition
{

    public Condition(int condition_id,  string descr, 
        bool show_date, bool show_nweeksdue, bool show_text,
        int display_order, bool is_deleted)
    {
        this.condition_id   = condition_id;
        this.descr          = descr;
        this.show_date      = show_date;
        this.show_nweeksdue = show_nweeksdue;
        this.show_text      = show_text;
        this.display_order  = display_order;
        this.is_deleted     = is_deleted;
    }
    public Condition(int condition_id)
    {
        this.condition_id = condition_id;
    }

    private int condition_id;
    public int ConditionID
    {
        get { return this.condition_id; }
        set { this.condition_id = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    private bool show_date;
    public bool ShowDate
    {
        get { return this.show_date; }
        set { this.show_date = value; }
    }
    private bool show_nweeksdue;
    public bool ShowNWeeksDue
    {
        get { return this.show_nweeksdue; }
        set { this.show_nweeksdue = value; }
    }
    private bool show_text;
    public bool ShowText
    {
        get { return this.show_text; }
        set { this.show_text = value; }
    }
    private int display_order;
    public int DisplayOrder
    {
        get { return this.display_order; }
        set { this.display_order = value; }
    }
    private bool is_deleted;
    public bool IsDeleted
    {
        get { return this.is_deleted; }
        set { this.is_deleted = value; }
    }
    public override string ToString()
    {
        return condition_id.ToString() + " " + descr.ToString() + " " + display_order.ToString() + " " + is_deleted.ToString();
    }

}