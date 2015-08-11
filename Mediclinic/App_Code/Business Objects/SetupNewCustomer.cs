using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SetupNewCustomer
{

    public SetupNewCustomer(int setup_new_customer_id, string company_name, string firstname, string surname, string company_email, string address_line1, 
                string address_line2, string city, string state_province_region, string postcode, string country, string phone_nbr, 
                string max_nbr_providers, 
                string field1, string field2, string field3, string field4,
                bool setasprov_field1, bool setasprov_field2, bool setasprov_field3, bool setasprov_field4, 
                string random_string, 
                DateTime date_added_info, DateTime date_added_db, string db_name)
    {
        this.setup_new_customer_id = setup_new_customer_id;
        this.company_name = company_name;
        this.firstname = firstname;
        this.surname = surname;
        this.company_email = company_email;
        this.address_line1 = address_line1;
        this.address_line2 = address_line2;
        this.city = city;
        this.state_province_region = state_province_region;
        this.postcode = postcode;
        this.country = country;
        this.phone_nbr = phone_nbr;
        this.max_nbr_providers = max_nbr_providers;
        this.field1 = field1;
        this.field2 = field2;
        this.field3 = field3;
        this.field4 = field4;
        this.setasprov_field1 = setasprov_field1;
        this.setasprov_field2 = setasprov_field2;
        this.setasprov_field3 = setasprov_field3;
        this.setasprov_field4 = setasprov_field4;
        this.random_string = random_string;
        this.date_added_info = date_added_info;
        this.date_added_db = date_added_db;
        this.db_name = db_name;
    }

    private int setup_new_customer_id;
    public int SetupNewCustomerID
    {
        get { return this.setup_new_customer_id; }
        set { this.setup_new_customer_id = value; }
    }
    private string company_name;
    public string CompanyName
    {
        get { return this.company_name; }
        set { this.company_name = value; }
    }
    private string firstname;
    public string Firstname
    {
        get { return this.firstname; }
        set { this.firstname = value; }
    }
    private string surname;
    public string Surname
    {
        get { return this.surname; }
        set { this.surname = value; }
    }
    private string company_email;
    public string CompanyEmail
    {
        get { return this.company_email; }
        set { this.company_email = value; }
    }
    private string address_line1;
    public string AddressLine1
    {
        get { return this.address_line1; }
        set { this.address_line1 = value; }
    }
    private string address_line2;
    public string AddressLine2
    {
        get { return this.address_line2; }
        set { this.address_line2 = value; }
    }
    private string city;
    public string City
    {
        get { return this.city; }
        set { this.city = value; }
    }
    private string state_province_region;
    public string StateProvinceRegion
    {
        get { return this.state_province_region; }
        set { this.state_province_region = value; }
    }
    private string postcode;
    public string Postcode
    {
        get { return this.postcode; }
        set { this.postcode = value; }
    }
    private string country;
    public string Country
    {
        get { return this.country; }
        set { this.country = value; }
    }
    private string phone_nbr;
    public string PhoneNbr
    {
        get { return this.phone_nbr; }
        set { this.phone_nbr = value; }
    }
    private string max_nbr_providers;
    public string MaxNbrProviders
    {
        get { return this.max_nbr_providers; }
        set { this.max_nbr_providers = value; }
    }
    private string field1;
    public string Field1
    {
        get { return this.field1; }
        set { this.field1 = value; }
    }
    private string field2;
    public string Field2
    {
        get { return this.field2; }
        set { this.field2 = value; }
    }
    private string field3;
    public string Field3
    {
        get { return this.field3; }
        set { this.field3 = value; }
    }
    private string field4;
    public string Field4
    {
        get { return this.field4; }
        set { this.field4 = value; }
    }

    private bool setasprov_field1;
    public bool SetAsProvField1
    {
        get { return this.setasprov_field1; }
        set { this.setasprov_field1 = value; }
    }
    private bool setasprov_field2;
    public bool SetAsProvField2
    {
        get { return this.setasprov_field2; }
        set { this.setasprov_field2 = value; }
    }
    private bool setasprov_field3;
    public bool SetAsProvField3
    {
        get { return this.setasprov_field3; }
        set { this.setasprov_field3 = value; }
    }
    private bool setasprov_field4;
    public bool SetAsProvField4
    {
        get { return this.setasprov_field4; }
        set { this.setasprov_field4 = value; }
    }

    private string random_string;
    public string RandomString
    {
        get { return this.random_string; }
        set { this.random_string = value; }
    }
    private DateTime date_added_info;
    public DateTime DateAddedInfo
    {
        get { return this.date_added_info; }
        set { this.date_added_info = value; }
    }
    private DateTime date_added_db;
    public DateTime DateAddedDb
    {
        get { return this.date_added_db; }
        set { this.date_added_db = value; }
    }
    private string db_name;
    public string DbName
    {
        get { return this.db_name; }
        set { this.db_name = value; }
    }
    public override string ToString()
    {
        return setup_new_customer_id.ToString() + " " + company_name.ToString() + " " + firstname.ToString() + " " + surname.ToString() + " " + company_email.ToString() + " " + 
                address_line1.ToString() + " " + address_line2.ToString() + " " + city.ToString() + " " + state_province_region.ToString() + " " + postcode.ToString() + " " + 
                country.ToString() + " " + phone_nbr.ToString() + " " + max_nbr_providers.ToString() + " " + field1.ToString() + " " + field2.ToString() + " " + 
                field3.ToString() + " " + field4.ToString() + " " + date_added_info.ToString() + " " + date_added_db.ToString() + " " + db_name.ToString();
    }

}