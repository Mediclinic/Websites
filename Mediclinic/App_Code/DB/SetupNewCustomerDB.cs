using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class SetupNewCustomerDB
{

    public static void Delete(int setup_new_customer_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM SetupNewCustomer WHERE setup_new_customer_id = " + setup_new_customer_id.ToString(), "Mediclinic_Main");
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string company_name, string firstname, string surname, string company_email, string address_line1, string address_line2, string city, string state_province_region, string postcode, string country, string phone_nbr, string max_nbr_providers, string field1, string field2, string field3, string field4, bool setasprov_field1, bool setasprov_field2, bool setasprov_field3, bool setasprov_field4, string random_string)
    {
        company_name = company_name.Replace("'", "''");
        firstname = firstname.Replace("'", "''");
        surname = surname.Replace("'", "''");
        company_email = company_email.Replace("'", "''");
        address_line1 = address_line1.Replace("'", "''");
        address_line2 = address_line2.Replace("'", "''");
        city = city.Replace("'", "''");
        state_province_region = state_province_region.Replace("'", "''");
        postcode = postcode.Replace("'", "''");
        country = country.Replace("'", "''");
        phone_nbr = phone_nbr.Replace("'", "''");
        max_nbr_providers = max_nbr_providers.Replace("'", "''");
        field1 = field1.Replace("'", "''");
        field2 = field2.Replace("'", "''");
        field3 = field3.Replace("'", "''");
        field4 = field4.Replace("'", "''");
        random_string = random_string.Replace("'", "''");
        string sql = "INSERT INTO SetupNewCustomer (company_name,firstname,surname,company_email,address_line1,address_line2,city,state_province_region,postcode,country,phone_nbr,max_nbr_providers,field1,field2,field3,field4,setasprov_field1,setasprov_field2,setasprov_field3,setasprov_field4,random_string,date_added_info,date_added_db,db_name) VALUES (" + "'" + company_name + "'," + "'" + firstname + "'," + "'" + surname + "'," + "'" + company_email + "'," + "'" + address_line1 + "'," + "'" + address_line2 + "'," + "'" + city + "'," + "'" + state_province_region + "'," + "'" + postcode + "'," + "'" + country + "'," + "'" + phone_nbr + "'," + "'" + max_nbr_providers + "'," + "'" + field1 + "'," + "'" + field2 + "'," + "'" + field3 + "'," + "'" + field4 + "'," + (setasprov_field1 ? "1," : "0,") + (setasprov_field2 ? "1," : "0,") + (setasprov_field3 ? "1," : "0,") + (setasprov_field4 ? "1," : "0,") + "'" + random_string + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "NULL" + "," + "''" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql, "Mediclinic_Main"));
    }

    public static void UpdateCompleted(int setup_new_customer_id, string db_name)
    {
        db_name = db_name.Replace("'", "''");
        string sql = "UPDATE SetupNewCustomer SET date_added_db = " + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",db_name = '" + db_name + "' WHERE setup_new_customer_id = " + setup_new_customer_id.ToString();
        DBBase.ExecuteNonResult(sql, "Mediclinic_Main");
    }

    public static void Update(int setup_new_customer_id, string company_name, string firstname, string surname, string company_email, string address_line1, string address_line2, string city, string state_province_region, string postcode, string country, string phone_nbr, string max_nbr_providers, string field1, string field2, string field3, string field4, bool setasprov_field1, bool setasprov_field2, bool setasprov_field3, bool setasprov_field4, string random_string, DateTime date_added_info, DateTime date_added_db, string db_name)
    {
        company_name = company_name.Replace("'", "''");
        firstname = firstname.Replace("'", "''");
        surname = surname.Replace("'", "''");
        company_email = company_email.Replace("'", "''");
        address_line1 = address_line1.Replace("'", "''");
        address_line2 = address_line2.Replace("'", "''");
        city = city.Replace("'", "''");
        state_province_region = state_province_region.Replace("'", "''");
        postcode = postcode.Replace("'", "''");
        country = country.Replace("'", "''");
        phone_nbr = phone_nbr.Replace("'", "''");
        max_nbr_providers = max_nbr_providers.Replace("'", "''");
        field1 = field1.Replace("'", "''");
        field2 = field2.Replace("'", "''");
        field3 = field3.Replace("'", "''");
        field4 = field4.Replace("'", "''");
        random_string = random_string.Replace("'", "''");
        db_name = db_name.Replace("'", "''");
        string sql = "UPDATE SetupNewCustomer SET company_name = '" + company_name + "',firstname = '" + firstname + "',surname = '" + surname + "',company_email = '" + company_email + "',address_line1 = '" + address_line1 + "',address_line2 = '" + address_line2 + "',city = '" + city + "',state_province_region = '" + state_province_region + "',postcode = '" + postcode + "',country = '" + country + "',phone_nbr = '" + phone_nbr + "',max_nbr_providers = '" + max_nbr_providers + "',field1 = '" + field1 + "',field2 = '" + field2 + "',field3 = '" + field3 + "',field4 = '" + field4 + "'" + ",setasprov_field1 = " + (setasprov_field1 ? "1" : "0") + ",setasprov_field2 = " + (setasprov_field2 ? "1" : "0") + ",setasprov_field3 = " + (setasprov_field3 ? "1" : "0") + ",setasprov_field4 = " + (setasprov_field4 ? "1" : "0") + ",random_string = '" + random_string + "',date_added_info = '" + date_added_info.ToString("yyyy-MM-dd HH:mm:ss") + "',date_added_db = " + (date_added_db == DateTime.MinValue ? "NULL" : "'" + date_added_db.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",db_name = '" + db_name + "' WHERE setup_new_customer_id = " + setup_new_customer_id.ToString();
        DBBase.ExecuteNonResult(sql, "Mediclinic_Main");
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT setup_new_customer_id,company_name,firstname,surname,company_email,address_line1,address_line2,city,state_province_region,postcode,country,phone_nbr,max_nbr_providers,field1,field2,field3,field4,setasprov_field1,setasprov_field2,setasprov_field3,setasprov_field4,random_string,date_added_info,date_added_db,db_name FROM SetupNewCustomer";
        return DBBase.ExecuteQuery(sql, "Mediclinic_Main").Tables[0];
    }

    public static SetupNewCustomer GetByID(int setup_new_customer_id)
    {
        string sql = "SELECT setup_new_customer_id,company_name,firstname,surname,company_email,address_line1,address_line2,city,state_province_region,postcode,country,phone_nbr,max_nbr_providers,field1,field2,field3,field4,setasprov_field1,setasprov_field2,setasprov_field3,setasprov_field4,random_string,date_added_info,date_added_db,db_name FROM SetupNewCustomer WHERE setup_new_customer_id = " + setup_new_customer_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql, "Mediclinic_Main").Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static SetupNewCustomer GetByRandomString(string random_string)
    {
        random_string = random_string.Replace("'", "''");
        string sql = "SELECT setup_new_customer_id,company_name,firstname,surname,company_email,address_line1,address_line2,city,state_province_region,postcode,country,phone_nbr,max_nbr_providers,field1,field2,field3,field4,setasprov_field1,setasprov_field2,setasprov_field3,setasprov_field4,random_string,date_added_info,date_added_db,db_name FROM SetupNewCustomer WHERE random_string = '" + random_string + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql, "Mediclinic_Main").Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static bool SiteAlreadyCreatedForThisEmail(string email)
    {
        email = email.Replace("'", "''");
        string sql = "SELECT COUNT(*) FROM SetupNewCustomer WHERE company_email = '" + email + "' AND date_added_db IS NOT NULL;";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql, "Mediclinic_Main")) > 0;
    }

    public static bool RandomStringExists(string random_string)
    {
        random_string = random_string.Replace("'", "''");
        string sql = "SELECT COUNT(setup_new_customer_id) FROM SetupNewCustomer WHERE random_string = '" + random_string + "'";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql, "Mediclinic_Main")) > 0;
    }


    public static SetupNewCustomer Load(DataRow row)
    {
        return new SetupNewCustomer(
            Convert.ToInt32(row["setup_new_customer_id"]),
            Convert.ToString(row["company_name"]),
            Convert.ToString(row["firstname"]),
            Convert.ToString(row["surname"]),
            Convert.ToString(row["company_email"]),
            Convert.ToString(row["address_line1"]),
            Convert.ToString(row["address_line2"]),
            Convert.ToString(row["city"]),
            Convert.ToString(row["state_province_region"]),
            Convert.ToString(row["postcode"]),
            Convert.ToString(row["country"]),
            Convert.ToString(row["phone_nbr"]),
            Convert.ToString(row["max_nbr_providers"]),
            Convert.ToString(row["field1"]),
            Convert.ToString(row["field2"]),
            Convert.ToString(row["field3"]),
            Convert.ToString(row["field4"]),
            Convert.ToBoolean(row["setasprov_field1"]),
            Convert.ToBoolean(row["setasprov_field2"]),
            Convert.ToBoolean(row["setasprov_field3"]),
            Convert.ToBoolean(row["setasprov_field4"]),
            Convert.ToString(row["random_string"]),
            row["date_added_info"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["date_added_info"]),
            row["date_added_db"]   == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["date_added_db"]),
            Convert.ToString(row["db_name"])
        );
    }

}