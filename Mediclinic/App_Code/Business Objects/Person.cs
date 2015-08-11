using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Person
{

    public Person(int person_id, int entity_id, int added_by, int title_id, string firstname, string middlename, string surname, string nickname, string gender,
                DateTime dob, DateTime person_date_added, DateTime person_date_modified)
    {
        this.person_id = person_id;
        this.entity_id = entity_id;
        this.added_by = added_by;
        this.title = new IDandDescr(title_id);
        this.firstname = firstname;
        this.middlename = middlename;
        this.surname = surname;
        this.nickname = nickname;
        this.gender = gender;
        this.dob = dob;
        this.person_date_added = person_date_added;
        this.person_date_modified = person_date_modified;
    }
    public Person(int person_id)
    {
        this.person_id = person_id;
    }

    private int person_id;
    public int PersonID
    {
        get { return this.person_id; }
        set { this.person_id = value; }
    }
    private int entity_id;
    public int EntityID
    {
        get { return this.entity_id; }
        set { this.entity_id = value; }
    }
    private int added_by;
    public int AddedBy
    {
        get { return this.added_by; }
        set { this.added_by = value; }
    }
    private IDandDescr title;
    public IDandDescr Title
    {
        get { return this.title; }
        set { this.title = value; }
    }
    public string Fullname
    {
        get { return firstname + " " + middlename + (middlename.Length > 0 ? " " : "") + surname; }
    }
    public string FullnameWithoutMiddlename
    {
        get { return firstname + " " + surname; }
    }
    public string FullnameWithTitleWithoutMiddlename
    {
        get { return (title.ID == 0 ? "" : title.Descr + " ") + firstname + " " + surname; }
    }
    private string firstname;
    public string Firstname
    {
        get { return this.firstname; }
        set { this.firstname = value; }
    }
    private string middlename;
    public string Middlename
    {
        get { return this.middlename; }
        set { this.middlename = value; }
    }
    private string surname;
    public string Surname
    {
        get { return this.surname; }
        set { this.surname = value; }
    }
    private string nickname;
    public string Nickname
    {
        get { return this.nickname; }
        set { this.nickname = value; }
    }
    private string gender;
    public string Gender
    {
        get { return this.gender; }
        set { this.gender = value; }
    }
    private DateTime dob;
    public DateTime Dob
    {
        get { return this.dob; }
        set { this.dob = value; }
    }
    private DateTime person_date_added;
    public DateTime PersonDateAdded
    {
        get { return this.person_date_added; }
        set { this.person_date_added = value; }
    }
    private DateTime person_date_modified;
    public DateTime PersonDateModified
    {
        get { return this.person_date_modified; }
        set { this.person_date_modified = value; }
    }
    public override string ToString()
    {
        return person_id.ToString() + " " + entity_id.ToString() + " " + added_by.ToString() + " " + title.ID.ToString() + " " + firstname.ToString() + " " + middlename.ToString() + " " + surname.ToString() + " " + nickname.ToString() + " " +
                gender.ToString() + " " + dob.ToString() + " " + person_date_added.ToString() + " " + person_date_modified.ToString();
    }

}