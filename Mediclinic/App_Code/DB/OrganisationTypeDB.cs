using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class OrganisationTypeDB
{

    public static void Delete(int organisation_type_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM OrganisationType WHERE organisation_type_id = " + organisation_type_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string descr, int organisation_type_group_id)
    {
        descr = descr.Replace("'", "''");
        string sql = "INSERT INTO OrganisationType (descr, organisation_type_group_id) VALUES (" + "'" + descr + "'," + organisation_type_group_id + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int organisation_type_id, string descr)
    {
        descr = descr.Replace("'", "''");
        string sql = "UPDATE OrganisationType SET descr = '" + descr + "' WHERE organisation_type_id = " + organisation_type_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT organisation_type_id,descr,organisation_type_group_id FROM OrganisationType ORDER BY display_order";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }



    public static DataTable GetDataTable_Clinics()
    {
        return GetDataTable_ByGroupType(5);
    }
    public static DataTable GetDataTable_AgedCareFacs()
    {
        return GetDataTable_ByGroupType(6);
    }
    public static DataTable GetDataTable_InsuranceOrgs()
    {
        return GetDataTable_ByGroupType(-1, "150");
    }
    public static DataTable GetDataTable_External(string org_type_ids = "")
    {
        return GetDataTable_ByGroupType(4, org_type_ids);
    }
    public static DataTable GetDataTable_ByType(params int[] types)
    {
        return GetDataTable_ByGroupType(-1, string.Join(",", types) );
    }

    public static DataTable GetDataTable_ByGroupType(int organisation_type_group_id = -1, string org_type_ids = "")
    {
        string sql = @"SELECT 
                                type.organisation_type_id,type.descr,type.organisation_type_group_id 
                       FROM 
                                OrganisationType type 
                                INNER JOIN OrganisationTypeGroup typegroup ON type.organisation_type_group_id = typegroup.organisation_type_group_id
                       WHERE    1=1 " +
                                (organisation_type_group_id > 0 ? @" AND typegroup.organisation_type_group_id = " + organisation_type_group_id : "") +
                                (org_type_ids != null && org_type_ids.Length > 0 ? @" AND type.organisation_type_id IN (" + org_type_ids + ") " : "") + @"
                       ORDER BY 
                                display_order";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return tbl;
    }


    public static OrganisationType GetByID(int organisation_type_id)
    {
        string sql = "SELECT organisation_type_id,descr,organisation_type_group_id FROM OrganisationType WHERE organisation_type_id = " + organisation_type_id.ToString() + " ORDER BY display_order";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static OrganisationType Load(DataRow row, string prefix = "")
    {
        return new OrganisationType(
            Convert.ToInt32(row[prefix+"organisation_type_id"]),
            Convert.ToString(row[prefix+"descr"]),
            Convert.ToInt32(row[prefix+"organisation_type_group_id"])
        );
    }



    protected static System.Collections.Hashtable _isDebtorHash = null;
    protected static System.Collections.Hashtable _isCrecitorHash = null;
    protected static void SetIsDebtorIsCreditorHashes()
    {
        if (_isDebtorHash == null || _isCrecitorHash == null)
        {
            _isDebtorHash = new System.Collections.Hashtable();
            _isCrecitorHash = new System.Collections.Hashtable();

            _isDebtorHash[218] = false;
            _isCrecitorHash[218] = false;

            _isDebtorHash[372] = false;
            _isCrecitorHash[372] = false;
            _isDebtorHash[367] = false;
            _isCrecitorHash[367] = false;
            _isDebtorHash[139] = true;
            _isCrecitorHash[139] = false;

            _isDebtorHash[141] = false;
            _isCrecitorHash[141] = false;

            // everything else : isDebtor=false isCreditor=true
        }
    }
    public static bool IsDebtor(int organisation_type_id)
    {
        SetIsDebtorIsCreditorHashes();
        return (_isDebtorHash[organisation_type_id] != null) ? Convert.ToBoolean(_isDebtorHash[organisation_type_id]) : false;
    }
    public static bool IsCreditor(int organisation_type_id)
    {
        SetIsDebtorIsCreditorHashes();
        return (_isCrecitorHash[organisation_type_id] != null) ? Convert.ToBoolean(_isCrecitorHash[organisation_type_id]) : true;
    }


}