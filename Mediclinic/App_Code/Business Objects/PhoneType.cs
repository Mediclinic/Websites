using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PhoneType
{

    public PhoneType(int phone_type_id, string descr)
    {
        this.phone_type_id = phone_type_id;
        this.descr = descr;
    }
    public PhoneType(int phone_type_id)
    {
        this.phone_type_id = phone_type_id;
    }

    private int phone_type_id;
    public int PhoneTypeID
    {
        get { return this.phone_type_id; }
        set { this.phone_type_id = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    public override string ToString()
    {
        return phone_type_id.ToString() + " " + descr.ToString();
    }

}