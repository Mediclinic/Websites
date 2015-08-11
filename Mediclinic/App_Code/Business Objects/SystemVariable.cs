using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SystemVariable
{

    public SystemVariable(string descr, string value, bool editable_in_gui, bool viewable_in_gui)
    {
        this.descr = descr;
        this.value = value;
        this.editable_in_gui = editable_in_gui;
        this.viewable_in_gui = viewable_in_gui;
    }

    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    private string value;
    public string Value
    {
        get { return this.value; }
        set { this.value = value; }
    }
    private bool editable_in_gui;
    public bool EditableInGui
    {
        get { return this.editable_in_gui; }
        set { this.editable_in_gui = value; }
    }
    private bool viewable_in_gui;
    public bool ViewableInGui
    {
        get { return this.viewable_in_gui; }
        set { this.viewable_in_gui = value; }
    }
    public override string ToString()
    {
        return descr.ToString() + " " + value.ToString() + " " + editable_in_gui.ToString() + " " + viewable_in_gui.ToString();
    }

}