using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

public class HealthCardEPCRemainingDB
{

    public static void Delete(int health_card_epc_remaining_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM HealthCardEPCRemaining WHERE health_card_epc_remaining_id = " + health_card_epc_remaining_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int health_card_id, int field_id, int num_services_remaining)
    {
        // disallow insert if has (health_card_id, field_id) where not deleted
        string sqlExists = "SELECT COUNT(*) FROM HealthCardEPCRemaining WHERE health_card_id = " + health_card_id.ToString() + " AND field_id = " + field_id.ToString() + " AND deleted_by IS NULL AND date_deleted IS NULL";
        bool exists = Convert.ToInt32(DBBase.ExecuteSingleResult(sqlExists)) > 0;
        if (exists)
            throw new CustomMessageException("This card already has this offering");

        string sql = "INSERT INTO HealthCardEPCRemaining (health_card_id,field_id,num_services_remaining) VALUES (" + "" + health_card_id + "," + "" + field_id + "," + "" + num_services_remaining + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int health_card_epc_remaining_id, int health_card_id, int field_id, int num_services_remaining)
    {
        string sql = "UPDATE HealthCardEPCRemaining SET health_card_id = " + health_card_id + ",field_id = " + field_id + ",num_services_remaining = " + num_services_remaining + " WHERE health_card_epc_remaining_id = " + health_card_epc_remaining_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateNumServicesRemaining(int health_card_epc_remaining_id, int num_services_remaining)
    {
        string sql = "UPDATE HealthCardEPCRemaining SET num_services_remaining = " + num_services_remaining + " WHERE health_card_epc_remaining_id = " + health_card_epc_remaining_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateAsDeleted(int health_card_epc_remaining_id, int staff_id)
    {
        string sql = "UPDATE HealthCardEPCRemaining SET deleted_by = " + staff_id + ",date_deleted = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE health_card_epc_remaining_id = " + health_card_epc_remaining_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateAllAsDeleted(int health_card_id, int staff_id)
    {
        string sql = "UPDATE HealthCardEPCRemaining SET deleted_by = " + staff_id + ",date_deleted = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE health_card_id = " + health_card_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }



    public static int GetCountByHealthCardID(int health_card_id)
    {
        string sql = "SELECT COUNT(*) FROM HealthCardEPCRemaining WHERE deleted_by IS NULL AND date_deleted IS NULL AND health_card_id = " + health_card_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static Hashtable GetTotalServicesRemainingByPatients(int[] patient_ids, DateTime start_date)
    {
        if (start_date == DateTime.MinValue)
            start_date = new DateTime(1900, 1, 1);  // dates in sql server must be after year 17xx

        string sql = @"SELECT hc.patient_id, hc.date_referral_signed,
                              ISNULL((SELECT SUM(num_services_remaining)
                                      FROM   HealthCardEPCRemaining
                                      WHERE  (deleted_by IS NULL AND date_deleted IS NULL AND health_card_id = hc.health_card_id)), 0) AS count

                       FROM   HealthCard hc
                       WHERE  hc.patient_id IN (" + string.Join(",", patient_ids) + @") AND hc.is_active = 1 AND hc.date_referral_signed > '"+start_date.ToString("yyyy-MM-dd")+@"'";

        Hashtable result = new Hashtable();

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int      patient_id  = Convert.ToInt32(tbl.Rows[i]["patient_id"]);
            int      count       = Convert.ToInt32(tbl.Rows[i]["count"]);
            DateTime date_signed = Convert.ToDateTime(tbl.Rows[i]["date_referral_signed"]);
            result[patient_id] = new System.Web.UI.Pair(count, date_signed);
        }

        return result;
    }

    /// <summary>Returns Hashtable with key "HealthCardID" and value is an array "HealthCardEPCRemaining[]".</summary>
    public static Hashtable GetHashtableByHealthCardIDs(int[] health_card_ids)
    {
        string sql = JoinedSql + " AND health_card_id IN (" + (health_card_ids.Length == 0 ? "0" : string.Join(", ", health_card_ids)) + ")";
        

        Hashtable hash = new Hashtable();

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            HealthCardEPCRemaining epcRemaining = LoadAll(tbl.Rows[i]);
            if (hash[epcRemaining.HealthCardID] == null)
                hash[epcRemaining.HealthCardID] = new ArrayList();
            ((ArrayList)hash[epcRemaining.HealthCardID]).Add(epcRemaining);
        }

        ArrayList keys = new ArrayList();
        foreach (int key in hash.Keys)
            keys.Add(key);
        for (int i = 0; i < keys.Count; i++)
        {
            ArrayList item = (ArrayList)hash[(int)keys[i]];
            hash[(int)keys[i]] = (HealthCardEPCRemaining[])item.ToArray(typeof(HealthCardEPCRemaining));
        }

        return hash;
    }



    public static DataTable GetDataTable()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static HealthCardEPCRemaining GetByID(int health_card_epc_remaining_id)
    {
        string sql = JoinedSql + " AND epcremaining.health_card_epc_remaining_id = " + health_card_epc_remaining_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }


    public static DataTable GetDataTable_ByHealthCardID(int health_card_id, int field_id = -1)
    {
        string sql = JoinedSql + " AND epcremaining.health_card_id = " + health_card_id.ToString() + (field_id == -1 ? "" : " AND epcremaining.field_id = " + field_id.ToString());
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return tbl;
    }
    public static HealthCardEPCRemaining[] GetByHealthCardID(int health_card_id, int field_id = -1)
    {
        DataTable tbl = GetDataTable_ByHealthCardID(health_card_id, field_id);
        HealthCardEPCRemaining[] result = new HealthCardEPCRemaining[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            result[i] = LoadAll(tbl.Rows[i]);

        return result;
    }

    protected static string JoinedSql = @"
            SELECT 
                    epcremaining.health_card_epc_remaining_id as epcremaining_health_card_epc_remaining_id,epcremaining.health_card_id as epcremaining_health_card_id,epcremaining.field_id as epcremaining_field_id,epcremaining.num_services_remaining as epcremaining_num_services_remaining,epcremaining.deleted_by as epcremaining_deleted_by,epcremaining.date_deleted as epcremaining_date_deleted,
                    field.field_id as field_field_id,field.descr as field_descr
            FROM 
                    HealthCardEPCRemaining  epcremaining
                    INNER JOIN Field field ON epcremaining.field_id = field.field_id 
            WHERE 
                    deleted_by IS NULL AND date_deleted IS NULL";


    public static HealthCardEPCRemaining LoadAll(DataRow row)
    {
        HealthCardEPCRemaining epcremaining = Load(row, "epcremaining_");
        epcremaining.Field = IDandDescrDB.Load(row, "field_field_id", "field_descr");
        return epcremaining;
    }

    public static HealthCardEPCRemaining Load(DataRow row, string prefix = "")
    {
        return new HealthCardEPCRemaining(
            Convert.ToInt32(row[prefix+"health_card_epc_remaining_id"]),
            Convert.ToInt32(row[prefix+"health_card_id"]),
            Convert.ToInt32(row[prefix+"field_id"]),
            Convert.ToInt32(row[prefix+"num_services_remaining"]),
            row[prefix+"deleted_by"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix+"deleted_by"]),
            row[prefix+"date_deleted"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix+"date_deleted"])
        );
    }

}