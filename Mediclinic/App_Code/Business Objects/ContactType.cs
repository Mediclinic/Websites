using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ContactType
{

    public ContactType(int contact_type_id, int contact_type_group_id, string descr, int display_order)
    {
        this.contact_type_id = contact_type_id;
        this.contact_type_group = new IDandDescr(contact_type_group_id);
        this.descr = descr;
        this.display_order = display_order;
    }
    public ContactType(int contact_type_id)
    {
        this.contact_type_id = contact_type_id;
    }

    private int contact_type_id;
    public int ContactTypeID
    {
        get { return this.contact_type_id; }
        set { this.contact_type_id = value; }
    }
    private IDandDescr contact_type_group;
    public IDandDescr ContactTypeGroup
    {
        get { return this.contact_type_group; }
        set { this.contact_type_group = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    private int display_order;
    public int DisplayOrder
    {
        get { return this.display_order; }
        set { this.display_order = value; }
    }
    public override string ToString()
    {
        return contact_type_id.ToString() + " " + contact_type_group.ID.ToString() + " " + descr.ToString();
    }

}