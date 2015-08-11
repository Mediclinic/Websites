using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class AddressChannel
{

    public AddressChannel(int address_channel_id, string descr, int address_channel_type_id, DateTime address_channel_date_added, DateTime address_channel_date_modified)
    {
        this.address_channel_id = address_channel_id;
        this.descr = descr;
        this.address_channel_type = new IDandDescr(address_channel_type_id);
        this.address_channel_date_added = address_channel_date_added;
        this.address_channel_date_modified = address_channel_date_modified;
    }
    public AddressChannel(int address_channel_id)
    {
        this.address_channel_id = address_channel_id;
    }


    public string DisplayName
    { get {
        return this.descr + " " + this.address_channel_type.Descr;
    }}


    private int address_channel_id;
    public int AddressChannelID
    {
        get { return this.address_channel_id; }
        set { this.address_channel_id = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    private IDandDescr address_channel_type;
    public IDandDescr AddressChannelType
    {
        get { return this.address_channel_type; }
        set { this.address_channel_type = value; }
    }
    private DateTime address_channel_date_added;
    public DateTime AddressChannelDateAdded
    {
        get { return this.address_channel_date_added; }
        set { this.address_channel_date_added = value; }
    }
    private DateTime address_channel_date_modified;
    public DateTime AddressChannelDateModified
    {
        get { return this.address_channel_date_modified; }
        set { this.address_channel_date_modified = value; }
    }
    public override string ToString()
    {
        return address_channel_id.ToString() + " " + descr.ToString() + " " + address_channel_type.ID.ToString() + " " + address_channel_date_added.ToString() + " " + address_channel_date_modified.ToString();
    }

}