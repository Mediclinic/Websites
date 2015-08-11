using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

public class HealthCardActionDB
{

    public static void Delete(int health_card_action_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM HealthCardAction WHERE health_card_action_id = " + health_card_action_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int health_card_id, int health_card_action_type_id, DateTime action_date)
    {
        string sql = "INSERT INTO HealthCardAction (health_card_id,health_card_action_type_id,action_date) VALUES (" + "" + health_card_id + "," + "" + health_card_action_type_id + "," + "'" + action_date.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int health_card_action_id, int health_card_id, int health_card_action_type_id, DateTime action_date)
    {
        string sql = "UPDATE HealthCardAction SET health_card_id = " + health_card_id + ",health_card_action_type_id = " + health_card_action_type_id + ",action_date = '" + action_date.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE health_card_action_id = " + health_card_action_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = GetJoinedSql();
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable_ByHealthCard(int health_card_id)
    {
        string sql = GetJoinedSql("health_card_id = " + health_card_id.ToString());
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static HealthCardAction GetByID(int health_card_action_id)
    {
        string sql = GetJoinedSql("health_card_action_id = " + health_card_action_id.ToString());
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }


    public static DateTime GetEPCDateSigned(int patient_id, DateTime booking_date)
    {
        HealthCardAction[] healthCardActions = HealthCardActionDB.GetReceivedActionsByPatientID(patient_id);
        return HealthCardActionDB.GetEPCDateSigned(healthCardActions, booking_date);
    }
    // use this if using a bulk data retrival of HealthCardActions to pass in for less db calls
    public static DateTime GetEPCDateSigned(HealthCardAction[] healthCardActions, DateTime booking_date)
    {
        /*
         * go through history (HealthCardAction) list for a patient
         * get all that is type Received
         * order by date DESC
         * find "first" where  booking_date >= date of that history item
        */
        for (int i = 0; i < healthCardActions.Length; i++)
        {
            if (booking_date >= healthCardActions[i].ActionDate.Date)
                return healthCardActions[i].ActionDate.Date;
        }

        return DateTime.MinValue; // not found
    }
    public static HealthCardAction[] GetReceivedActionsByPatientID(int patient_id)
    {
        string sql = @"
            SELECT
                     hca.health_card_action_id, hca.health_card_id, hca.health_card_action_type_id, hca.action_date, type.descr
            FROM
                     HealthCardAction AS hca 
                     INNER JOIN HealthCardActionType AS type ON type.health_card_action_type_id = hca.health_card_action_type_id
                     INNER JOIN HealthCard AS hc ON hc.health_card_id = hca.health_card_id

            WHERE
                     hc.patient_id = " + patient_id + @" AND hca.health_card_action_type_id = 0
            ORDER BY
                     hca.action_date DESC, hca.health_card_action_id DESC";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        HealthCardAction[] list = new HealthCardAction[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = Load(tbl.Rows[i]);

        return list;
    }
    public static Hashtable GetReceivedActionsByPatientIDs(int[] patient_ids)
    {
        string sql = @"
            SELECT
                     hca.health_card_action_id, hca.health_card_id, hca.health_card_action_type_id, hca.action_date, type.descr,   hc.patient_id
            FROM
                     HealthCardAction AS hca 
                     INNER JOIN HealthCardActionType AS type ON type.health_card_action_type_id = hca.health_card_action_type_id
                     INNER JOIN HealthCard AS hc ON hc.health_card_id = hca.health_card_id

            WHERE
                     "+ (patient_ids != null && patient_ids.Length > 0 ? " hc.patient_id IN (" + string.Join(",",patient_ids) + @")" : "1 <> 1" ) + @" AND hca.health_card_action_type_id = 0
            ORDER BY
                     hca.action_date DESC, hca.health_card_action_id DESC";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            HealthCardAction hca = Load(tbl.Rows[i]);
            int patient_id = Convert.ToInt32(tbl.Rows[i]["patient_id"]);
            if (hash[patient_id] == null)
                hash[patient_id] = new System.Collections.ArrayList();
            ((System.Collections.ArrayList)hash[patient_id]).Add(hca);
        }


        // convert from arraylists to arrays 
        ArrayList keys = new ArrayList();
        foreach (System.Collections.DictionaryEntry de in hash)
            keys.Add(de.Key);
        foreach (int key in keys)
            hash[key] = (HealthCardAction[])((ArrayList)hash[key]).ToArray(typeof(HealthCardAction)); ;

        return hash;
    }


    private static string GetJoinedSql(string whereClause = "")
    {
        string sql = @"
            SELECT
                     hca.health_card_action_id, hca.health_card_id, hca.health_card_action_type_id, hca.action_date, type.descr
            FROM
                     HealthCardAction AS hca 
                     INNER JOIN HealthCardActionType AS type ON type.health_card_action_type_id = hca.health_card_action_type_id";

        if (whereClause.Length > 0)
               sql += @"
            WHERE " + whereClause;

               sql += @"
            ORDER BY
                     hca.action_date DESC, hca.health_card_action_id DESC";

        return sql;
        }

    public static HealthCardAction Load(DataRow row, string prefix="")
    {
        return new HealthCardAction(
            Convert.ToInt32(row[prefix+"health_card_action_id"]),
            Convert.ToInt32(row[prefix + "health_card_id"]),
            Convert.ToInt32(row[prefix + "health_card_action_type_id"]),
            Convert.ToDateTime(row[prefix + "action_date"])
        );
    }

}