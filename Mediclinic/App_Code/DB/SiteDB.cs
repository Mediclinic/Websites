using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class SiteDB
{

    public static void Delete(int site_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Site WHERE site_id = " + site_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string name, int site_type_id, string abn, string acn, string tfn, string asic, bool is_provider, string bank_bpay, string bank_bsb, string bank_account, string bank_direct_debit_userid, string bank_username, decimal oustanding_balance_warning, bool print_epc, bool excl_sun, bool excl_mon, bool excl_tue, bool excl_wed, bool excl_thu, bool excl_fri, bool excl_sat, TimeSpan day_start_time, TimeSpan lunch_start_time, TimeSpan lunch_end_time, TimeSpan day_end_time, DateTime fiscal_yr_end, int num_booking_months_to_get)
    {
        int entityID = EntityDB.Insert();

        name = name.Replace("'", "''");
        abn = abn.Replace("'", "''");
        acn = acn.Replace("'", "''");
        tfn = tfn.Replace("'", "''");
        asic = asic.Replace("'", "''");
        bank_bpay = bank_bpay.Replace("'", "''");
        bank_bsb = bank_bsb.Replace("'", "''");
        bank_account = bank_account.Replace("'", "''");
        bank_direct_debit_userid = bank_direct_debit_userid.Replace("'", "''");
        bank_username = bank_username.Replace("'", "''");
        string sql = "INSERT INTO Site (entity_id,name,site_type_id,abn,acn,tfn,asic,is_provider,bank_bpay,bank_bsb,bank_account,bank_direct_debit_userid,bank_username,oustanding_balance_warning,print_epc,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat,day_start_time,lunch_start_time,lunch_end_time,day_end_time,fiscal_yr_end,num_booking_months_to_get) VALUES (" + entityID + "" + "'" + name + "'," + site_type_id + ",'" + abn + "'," + "'" + acn + "'," + "'" + tfn + "'," + "'" + asic + "'," + (is_provider ? "1," : "0,") + "'" + bank_bpay + "'," + "'" + bank_bsb + "'," + "'" + bank_account + "'," + "'" + bank_direct_debit_userid + "'," + "'" + bank_username + "'," + "" + oustanding_balance_warning + "," + (print_epc ? "1," : "0,") + (excl_sun ? "1," : "0,") + (excl_mon ? "1," : "0,") + (excl_tue ? "1," : "0,") + (excl_wed ? "1," : "0,") + (excl_thu ? "1," : "0,") + (excl_fri ? "1," : "0,") + (excl_sat ? "1," : "0,") + "'" + day_start_time.ToString() + "'," + "'" + lunch_start_time.ToString() + "'," + "'" + lunch_end_time.ToString() + "'," + "'" + day_end_time.ToString() + "'," + "'" + fiscal_yr_end.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "" + num_booking_months_to_get + "" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static void Update(int site_id, string name, int site_type_id, string abn, string acn, string tfn, string asic, bool is_provider, string bank_bpay, string bank_bsb, string bank_account, string bank_direct_debit_userid, string bank_username, decimal oustanding_balance_warning, bool print_epc, bool excl_sun, bool excl_mon, bool excl_tue, bool excl_wed, bool excl_thu, bool excl_fri, bool excl_sat, TimeSpan day_start_time, TimeSpan lunch_start_time, TimeSpan lunch_end_time, TimeSpan day_end_time, DateTime fiscal_yr_end, int num_booking_months_to_get)
    {
        name = name.Replace("'", "''");
        abn = abn.Replace("'", "''");
        acn = acn.Replace("'", "''");
        tfn = tfn.Replace("'", "''");
        asic = asic.Replace("'", "''");
        bank_bpay = bank_bpay.Replace("'", "''");
        bank_bsb = bank_bsb.Replace("'", "''");
        bank_account = bank_account.Replace("'", "''");
        bank_direct_debit_userid = bank_direct_debit_userid.Replace("'", "''");
        bank_username = bank_username.Replace("'", "''");
        string sql = "UPDATE Site SET name = '" + name + "',site_type_id = " + site_type_id + ",abn = '" + abn + "',acn = '" + acn + "',tfn = '" + tfn + "',asic = '" + asic + "',is_provider = " + (is_provider ? "1," : "0,") + "bank_bpay = '" + bank_bpay + "',bank_bsb = '" + bank_bsb + "',bank_account = '" + bank_account + "',bank_direct_debit_userid = '" + bank_direct_debit_userid + "',bank_username = '" + bank_username + "',oustanding_balance_warning = " + oustanding_balance_warning + ",print_epc = " + (print_epc ? "1," : "0,") + "excl_sun = " + (excl_sun ? "1," : "0,") + "excl_mon = " + (excl_mon ? "1," : "0,") + "excl_tue = " + (excl_tue ? "1," : "0,") + "excl_wed = " + (excl_wed ? "1," : "0,") + "excl_thu = " + (excl_thu ? "1," : "0,") + "excl_fri = " + (excl_fri ? "1," : "0,") + "excl_sat = " + (excl_sat ? "1," : "0,") + "day_start_time = '" + day_start_time.ToString() + "',lunch_start_time = '" + lunch_start_time.ToString() + "',lunch_end_time = '" + lunch_end_time.ToString() + "',day_end_time = '" + day_end_time.ToString() + "',fiscal_yr_end = '" + fiscal_yr_end.ToString("yyyy-MM-dd HH:mm:ss") + "',num_booking_months_to_get = " + num_booking_months_to_get + " WHERE site_id = " + site_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static int GetCountByEntityID(int entity_id)
    {
        string sql = "SELECT COUNT(*) FROM Site WHERE entity_id = " + entity_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static DataTable GetDataTable(string DB = null)
    {
        string sql = @"SELECT site_id,entity_id,name,Site.site_type_id,abn,acn,tfn,asic,is_provider,bank_bpay,bank_bsb,bank_account,bank_direct_debit_userid,bank_username,oustanding_balance_warning,print_epc,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat,day_start_time,lunch_start_time,lunch_end_time,day_end_time,fiscal_yr_end,num_booking_months_to_get, 
                              SiteType.descr as site_type_descr 
                       FROM   Site 
                              LEFT JOIN SiteType ON Site.site_type_id = SiteType.site_type_id";

        return DBBase.ExecuteQuery(sql, DB).Tables[0];
    }

    public static Site GetByID(int site_id, string DB = null)
    {
        string sql = @"SELECT site_id,entity_id,name,Site.site_type_id,abn,acn,tfn,asic,is_provider,bank_bpay,bank_bsb,bank_account,bank_direct_debit_userid,bank_username,oustanding_balance_warning,print_epc,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat,day_start_time,lunch_start_time,lunch_end_time,day_end_time,fiscal_yr_end,num_booking_months_to_get, 
                              SiteType.descr as site_type_descr 
                       FROM   Site 
                              LEFT JOIN SiteType ON Site.site_type_id = SiteType.site_type_id
                       WHERE  site_id = " + site_id.ToString();

        DataTable tbl = DBBase.ExecuteQuery( sql, DB ).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0], "", "site_type_descr");
    }

    public enum SiteType { Clinic = 1, AgedCare = 2, GP = 3 };
    public static Site GetSiteByType(SiteType siteType, Site[] allSites = null, string DB = null)
    {
        Site[] sites = allSites != null ? allSites : GetAll(DB);
        for (int i = 0; i < sites.Length; i++)
            if ((int)siteType == sites[i].SiteType.ID)
                return sites[i];

        return null; // should not get here
    }

    public static Site[] GetAll(string DB = null)
    {
        DataTable tbl = GetDataTable(DB);
        Site[] sites = new Site[tbl.Rows.Count];
        for (int i = 0; i < sites.Length; i++)
            sites[i] = Load(tbl.Rows[i], "", "site_type_descr");

        return sites;
    }

    public static System.Collections.Hashtable GetAllInHashtable()
    {
        System.Collections.Hashtable hash = new System.Collections.Hashtable();
        Site[] allSites = GetAll();
        for (int i = 0; i < allSites.Length; i++)
            hash[allSites[i].SiteID] = allSites[i];

        return hash;
    }

    /*
    public static Site LoadSiteType(Site site)
    {
        site.SiteType.Descr = ((SiteType)site.SiteType.ID).ToString();
        return site;
    }
    */

    public static Site Load(DataRow row, string prefix = "", string site_type_descr = null)
    {
        Site site = new Site(
            Convert.ToInt32(row[prefix+"site_id"]),
            Convert.ToInt32(row[prefix + "entity_id"]),
            Convert.ToString(row[prefix + "name"]),
            Convert.ToInt32(row[prefix + "site_type_id"]),
            Convert.ToString(row[prefix + "abn"]),
            Convert.ToString(row[prefix + "acn"]),
            Convert.ToString(row[prefix + "tfn"]),
            Convert.ToString(row[prefix + "asic"]),
            Convert.ToBoolean(row[prefix + "is_provider"]),
            Convert.ToString(row[prefix + "bank_bpay"]),
            Convert.ToString(row[prefix + "bank_bsb"]),
            Convert.ToString(row[prefix + "bank_account"]),
            Convert.ToString(row[prefix + "bank_direct_debit_userid"]),
            Convert.ToString(row[prefix + "bank_username"]),
            Convert.ToDecimal(row[prefix + "oustanding_balance_warning"]),
            Convert.ToBoolean(row[prefix + "print_epc"]),
            Convert.ToBoolean(row[prefix + "excl_sun"]),
            Convert.ToBoolean(row[prefix + "excl_mon"]),
            Convert.ToBoolean(row[prefix + "excl_tue"]),
            Convert.ToBoolean(row[prefix + "excl_wed"]),
            Convert.ToBoolean(row[prefix + "excl_thu"]),
            Convert.ToBoolean(row[prefix + "excl_fri"]),
            Convert.ToBoolean(row[prefix + "excl_sat"]),
            (TimeSpan)row[prefix + "day_start_time"],
            (TimeSpan)row[prefix + "lunch_start_time"],
            (TimeSpan)row[prefix + "lunch_end_time"],
            (TimeSpan)row[prefix + "day_end_time"],
            Convert.ToDateTime(row[prefix + "fiscal_yr_end"]),
            Convert.ToInt32(row[prefix + "num_booking_months_to_get"])
        );

        if (site_type_descr != null)
            site.SiteType.Descr = Convert.ToString(row[site_type_descr]);

        return site;
    }

}