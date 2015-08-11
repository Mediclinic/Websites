using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Suburb
{

    public Suburb(int suburb_id, string name, string postcode, string state, DateTime amended_date, int amended_by,
                string previous)
    {
        this.suburb_id = suburb_id;
        this.name = name;
        this.postcode = postcode;
        this.state = state;
        this.amended_date = amended_date;
        this.amended_by = amended_by;
        this.previous = previous;
    }
    public Suburb(int suburb_id)
    {
        this.suburb_id = suburb_id;
    }

    private int suburb_id;
    public int SuburbID
    {
        get { return this.suburb_id; }
        set { this.suburb_id = value; }
    }
    private string name;
    public string Name
    {
        get { return this.name; }
        set { this.name = value; }
    }
    private string postcode;
    public string Postcode
    {
        get { return this.postcode; }
        set { this.postcode = value; }
    }
    private string state;
    public string State
    {
        get { return this.state; }
        set { this.state = value; }
    }
    private DateTime amended_date;
    public DateTime AmendedDate
    {
        get { return this.amended_date; }
        set { this.amended_date = value; }
    }
    private int amended_by;
    public int AmendedBy
    {
        get { return this.amended_by; }
        set { this.amended_by = value; }
    }
    private string previous;
    public string Previous
    {
        get { return this.previous; }
        set { this.previous = value; }
    }
    public override string ToString()
    {
        return suburb_id.ToString() + " " + name.ToString() + " " + postcode.ToString() + " " + state.ToString() + " " + amended_date.ToString() + " " +
                amended_by.ToString() + " " + previous.ToString();
    }

}