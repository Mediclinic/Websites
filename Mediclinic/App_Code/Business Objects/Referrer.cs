using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Referrer
{

    public Referrer(int referrer_id, int person_id, DateTime referrer_date_added)
    {
        this.referrer_id = referrer_id;
        this.person = new Person(person_id);
        this.referrer_date_added = referrer_date_added;
    }
    public Referrer(int referrer_id)
    {
        this.referrer_id = referrer_id;
    }

    private int referrer_id;
    public int ReferrerID
    {
        get { return this.referrer_id; }
        set { this.referrer_id = value; }
    }
    private Person person;
    public Person Person
    {
        get { return this.person; }
        set { this.person = value; }
    }
    private DateTime referrer_date_added;
    public DateTime ReferrerDateAdded
    {
        get { return this.referrer_date_added; }
        set { this.referrer_date_added = value; }
    }
    public override string ToString()
    {
        return referrer_id.ToString() + " " + person.PersonID.ToString() + " " + referrer_date_added.ToString();
    }


    public static Referrer[] RemoveByID(Referrer[] referrers, int referrer_id_to_remove)
    {
        Referrer[] newList = new Referrer[referrers.Length - 1];

        bool found = false;
        for (int i = 0; i < referrers.Length; i++)
        {
            if (referrers[i].ReferrerID != referrer_id_to_remove)
                newList[i - (found ? 1 : 0)] = referrers[i];
            else
                found = true;
        }

        return newList;
    }

}