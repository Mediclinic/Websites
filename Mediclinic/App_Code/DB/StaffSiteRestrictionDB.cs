using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class StaffSiteRestrictionDB
{

    public static void Delete(int staff_site_restriction_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM StaffSiteRestriction WHERE staff_site_restriction_id = " + staff_site_restriction_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void Delete(int staff_id, int site_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE StaffSiteRestriction WHERE staff_id = " + staff_id + " AND site_id = " + site_id);
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int staff_id, int site_id)
    {
        if (Exists(staff_id, site_id))
            throw new UniqueConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        string sql = "INSERT INTO StaffSiteRestriction (staff_id,site_id) VALUES (" + "" + staff_id + "," + "" + site_id + "" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int staff_id, int site_id, bool restrict)
    {
        if (!restrict)
        {
            Delete(staff_id, site_id);
        }
        else
        {
            if (!Exists(staff_id, site_id))
                DBBase.ExecuteNonResult("INSERT INTO StaffSiteRestriction (staff_id,site_id) VALUES (" + "" + staff_id + "," + "" + site_id + "" + ")");
        }
    }

    public static bool Exists(int staff_id, int site_id)
    {
        string sql = "SELECT COUNT(*) FROM StaffSiteRestriction WHERE staff_id = " + staff_id + " AND site_id = " + site_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static DataTable GetDataTable(int staff_id = -1, int site_id = -1)
    {
        string whereClause = string.Empty;
        if (staff_id != -1)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " staff_id = " + staff_id;
        if (site_id != -1)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " site_id = " + site_id;

        string sql = JoinedSql + whereClause;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static StaffSiteRestriction[] GetBySiteAndOrStaff(int staff_id = -1, int site_id = -1)
    {
        DataTable tbl = GetDataTable(staff_id, site_id);
        StaffSiteRestriction[] list = new StaffSiteRestriction[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = Load(tbl.Rows[i]);

        return list;
    }

    public static StaffSiteRestriction GetByID(int staff_site_restriction_id)
    {
        string sql = JoinedSql + " WHERE staff_site_restriction_id = " + staff_site_restriction_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }


    public static DataTable GetDataTable_SitesNotRestricted(int staff_id = -1, int site_id = -1, bool only_clinic = false)
    {
        string whereClause = string.Empty;

        if (staff_id != -1)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + "site_id NOT IN (SELECT site_id FROM StaffSiteRestriction WHERE (staff_id = " + staff_id + "))";
        if (site_id != -1)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + "site_id NOT IN (SELECT site_id FROM StaffSiteRestriction WHERE (site_id = " + site_id + "))";
        if (only_clinic)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + "Site.site_type_id = 1";

        string sql = @"
            SELECT
                    site_id,entity_id,name,Site.site_type_id,abn,acn,tfn,asic,is_provider,bank_bpay,bank_bsb,bank_account,bank_direct_debit_userid,bank_username,oustanding_balance_warning,print_epc,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat,day_start_time,lunch_start_time,lunch_end_time,day_end_time,fiscal_yr_end,num_booking_months_to_get,
                    SiteType.descr as site_type_descr
            FROM 
                    Site 
                    LEFT JOIN SiteType ON Site.site_type_id = SiteType.site_type_id " + whereClause;

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Site[] GetSitesNotRestricted(int staff_id = -1, int site_id = -1, bool only_clinic = false)
    {
        DataTable tbl = GetDataTable_SitesNotRestricted(staff_id, site_id, only_clinic);
        Site[] list = new Site[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = SiteDB.Load(tbl.Rows[i]);

        return list;
    }

    public static DataTable GetDataTable_AllSitesWithRestriction(int staff_id)
    {
        string sql = @"
            SELECT
                    site_id,entity_id,name,Site.site_type_id,abn,acn,tfn,asic,is_provider,bank_bpay,bank_bsb,bank_account,bank_direct_debit_userid,bank_username,oustanding_balance_warning,print_epc,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat,day_start_time,lunch_start_time,lunch_end_time,day_end_time,fiscal_yr_end,num_booking_months_to_get,
                    SiteType.descr as site_type_descr,
                    CASE WHEN EXISTS
                             (SELECT  *
                               FROM   StaffSiteRestriction
                               WHERE  StaffSiteRestriction.site_id = site.site_id AND StaffSiteRestriction.staff_id = " + staff_id + @") THEN 0 ELSE 1 END AS has_access
            FROM 
                    Site
                    INNER JOIN SiteType ON Site.site_type_id = SiteType.site_type_id";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }


    private static string JoinedSql = @"
        SELECT 
                staff_site_restriction_id,staff_id,site_id 
        FROM 
                StaffSiteRestriction";

    public static StaffSiteRestriction Load(DataRow row)
    {
        return new StaffSiteRestriction(
            Convert.ToInt32(row["staff_site_restriction_id"]),
            Convert.ToInt32(row["staff_id"]),
            Convert.ToInt32(row["site_id"])
        );
    }

}