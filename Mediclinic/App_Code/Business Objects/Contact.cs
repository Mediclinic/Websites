using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Contact
{

    public Contact(int contact_id, int entity_id, int contact_type_id, 
                string free_text, string addr_line1, string addr_line2, int address_channel_id, int suburb_id, int country_id,
                int site_id, bool is_billing, bool is_non_billing,
                DateTime contact_date_added, DateTime contact_date_modified)
    {
        this.contact_id         = contact_id;
        this.entity_id          = entity_id;
        this.contact_type       = new ContactType(contact_type_id); 
        this.free_text          = free_text;
        this.addr_line1         = addr_line1;
        this.addr_line2         = addr_line2;
        this.address_channel    = address_channel_id == -1 ? null : new AddressChannel(address_channel_id);
        this.suburb             = suburb_id          == -1 ? null : new Suburb(suburb_id);
        this.country            = country_id         == -1 ? null : new IDandDescr(country_id);
        this.site               = site_id            == -1 ? null : new Site(site_id);
        this.is_billing         = is_billing;
        this.is_non_billing     = is_non_billing;
        this.contact_date_added = contact_date_added;
        this.contact_date_modified = contact_date_modified;
    }

    public string AddressDisplayName
    { get {
        return this.addr_line1 + " " + this.address_channel.DisplayName;
    }}

    public string PhoneNumberWithDashes
    {
        get
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.addr_line1, @"^\d+$"))
                return this.addr_line1;
            else 
                return Utilities.FormatPhoneNumber(this.addr_line1);
        }
    }

    private int contact_id;
    public int ContactID
    {
        get { return this.contact_id; }
        set { this.contact_id = value; }
    }
    private int entity_id;
    public int EntityID
    {
        get { return this.entity_id; }
        set { this.entity_id = value; }
    }
    private ContactType contact_type;
    public ContactType ContactType
    {
        get { return this.contact_type; }
        set { this.contact_type = value; }
    }
    private string free_text;
    public string FreeText
    {
        get { return this.free_text; }
        set { this.free_text = value; }
    }
    private string addr_line1;
    public string AddrLine1
    {
        get { return this.addr_line1; }
        set { this.addr_line1 = value; }
    }
    private string addr_line2;
    public string AddrLine2
    {
        get { return this.addr_line2; }
        set { this.addr_line2 = value; }
    }
    private AddressChannel address_channel;
    public AddressChannel AddressChannel
    {
        get { return this.address_channel; }
        set { this.address_channel = value; }
    }
    private Suburb suburb;
    public Suburb Suburb
    {
        get { return this.suburb; }
        set { this.suburb = value; }
    }
    private IDandDescr country;
    public IDandDescr Country
    {
        get { return this.country; }
        set { this.country = value; }
    }
    private Site site;
    public Site Site
    {
        get { return this.site; }
        set { this.site = value; }
    }
    private bool is_billing;
    public bool IsBilling
    {
        get { return this.is_billing; }
        set { this.is_billing = value; }
    }
    private bool is_non_billing;
    public bool IsNonBilling
    {
        get { return this.is_non_billing; }
        set { this.is_non_billing = value; }
    }
    private DateTime contact_date_added;
    public DateTime ContactDateAdded
    {
        get { return this.contact_date_added; }
        set { this.contact_date_added = value; }
    }
    private DateTime contact_date_modified;
    public DateTime ContactDateModified
    {
        get { return this.contact_date_modified; }
        set { this.contact_date_modified = value; }
    }
    public override string ToString()
    {
        return contact_id.ToString() + " " + entity_id.ToString() + " " + addr_line1.ToString() + " " + addr_line2.ToString() + " " + contact_type.ContactTypeID.ToString() + " " +
                address_channel.AddressChannelID.ToString() + " " + suburb.SuburbID.ToString() + " " + country.ID.ToString() + " " + site.SiteID.ToString() + " " + is_billing.ToString() + " " +
                is_non_billing.ToString() + " " + contact_date_added.ToString() + " " + contact_date_modified.ToString();
    }

    public string GetFormattedAddress(string defaultEmptyString = "", int nTabs = 0)
    {

        string indent = new String('\t', nTabs);
        string formattedAddress = string.Empty;

        if (this.AddrLine2.Length == 0)
        {
            formattedAddress += this.AddrLine1 + (this.AddrLine1.Length == 0 ? "" : " ");
            formattedAddress += this.AddressChannel == null || this.AddressChannel.AddressChannelID == 1 ? "" : this.AddressChannel.DisplayName;
            if (formattedAddress.Length > 0)
                formattedAddress = indent + formattedAddress + Environment.NewLine;
        }
        else
        {
            string line1 = (this.AddrLine1.Length == 0 ? string.Empty : this.AddrLine1) + (this.AddrLine1.Length == 0 || this.AddrLine2.Length == 0 ? string.Empty : " ") + (this.AddrLine2.Length == 0 ? string.Empty : this.AddrLine2);
            string line2 = this.AddressChannel == null || this.AddressChannel.AddressChannelID == 1 ? "" : this.AddressChannel.DisplayName;

            if (line1.Length > 0)
                formattedAddress = indent + line1 + Environment.NewLine;
            if (line2.Length > 0)
                formattedAddress = indent + line2 + Environment.NewLine;


            //formattedAddress += "\t" + (this.AddrLine1.Length == 0 ? string.Empty : this.AddrLine1) + (this.AddrLine1.Length == 0 || this.AddrLine2.Length == 0 ? string.Empty : " ") + (this.AddrLine2.Length == 0 ? string.Empty : this.AddrLine2) + Environment.NewLine;
            //formattedAddress += "\t" + this.AddressChannel == null || this.AddressChannel.AddressChannelID == 1 ? "" : this.AddressChannel.DisplayName + Environment.NewLine;
        }

        string suburbName     = this.Suburb == null || this.suburb.Name.Length     == 0 ? string.Empty : this.suburb.Name     + ", ";
        string suburbState    = this.Suburb == null || this.suburb.State.Length    == 0 ? string.Empty : this.suburb.State    + ", ";
        string suburbPostcode = this.Suburb == null || this.suburb.Postcode.Length == 0 ? string.Empty : this.suburb.Postcode + ", ";
        string suburbText = suburbName + suburbState + suburbPostcode;
        if (suburbText.EndsWith(", "))
            suburbText = suburbText.Substring(0, suburbText.Length-2);
        formattedAddress += (suburbText.Length > 0 ? indent : "") + suburbText;
        //formattedAddress += Environment.NewLine;

        //formattedAddress += this.Country == null ? "Australia" + Environment.NewLine : this.Country.Descr;
        //formattedAddress += Environment.NewLine;

        return formattedAddress == string.Empty ? indent + defaultEmptyString : formattedAddress;
    }

    public string GetFormattedPhoneNumber(string defaultEmptyString = "")
    {
        string formattedAddress = Utilities.FormatPhoneNumber(this.AddrLine1);
        return formattedAddress == string.Empty ? defaultEmptyString : formattedAddress;
    }

    public static Contact[] RemoveInvalidEmailAddresses(Contact[] inList)
    {
        System.Collections.ArrayList list = new System.Collections.ArrayList();
        for (int i = 0; i < inList.Length; i++)
        {
            inList[i].AddrLine1 = inList[i].AddrLine1.Trim();
            if (Utilities.IsValidEmailAddress(inList[i].AddrLine1))
                list.Add(inList[i]);
        }
        return (Contact[])list.ToArray(typeof(Contact));
    }
    public static Contact[] RemoveInvalidPhoneNumbers(Contact[] inList)
    {
        System.Collections.ArrayList list = new System.Collections.ArrayList();
        for (int i = 0; i < inList.Length; i++)
            if (Utilities.IsValidPhoneNumber(inList[i].AddrLine1))
                list.Add(inList[i]);
        return (Contact[])list.ToArray(typeof(ContactAus));
    }


}