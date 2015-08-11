using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class IDandDescr
{
    public IDandDescr(int id, string descr)
	{
        this.id = id;
        this.descr = descr;
	}
    public IDandDescr(int id)
    {
        this.id = id;
    }

    private int id;
    public int ID
    {
        get { return this.id; }
        set { this.id = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }

}