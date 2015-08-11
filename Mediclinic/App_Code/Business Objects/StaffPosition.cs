using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class StaffPosition
{

    public StaffPosition(int staff_position_id, string descr)
    {
        this.staff_position_id = staff_position_id;
        this.descr = descr;
    }
    public StaffPosition(int staff_position_id)
    {
        this.staff_position_id = staff_position_id;
    }

    private int staff_position_id;
    public int StaffPositionID
    {
        get { return this.staff_position_id; }
        set { this.staff_position_id = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    public override string ToString()
    {
        return staff_position_id.ToString() + " " + descr.ToString();
    }

}