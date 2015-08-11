using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class CostCentre
{

    public CostCentre(int costcentre_id, string descr, int parent_id)
    {
        this.costcentre_id = costcentre_id;
        this.descr = descr;
        this.parent_id = parent_id;
    }
    public CostCentre(int costcentre_id)
    {
        this.costcentre_id = costcentre_id;
    }

    private int costcentre_id;
    public int CostCentreID
    {
        get { return this.costcentre_id; }
        set { this.costcentre_id = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    private int parent_id;
    public int ParentID
    {
        get { return this.parent_id; }
        set { this.parent_id = value; }
    }


    private CostCentre[] children;
    public CostCentre[] Children
    {
        get { return this.children; }
        set { this.children = value; }
    }


    public override string ToString()
    {
        return costcentre_id.ToString() + " " + descr.ToString() + " " + parent_id.ToString();
    }

}