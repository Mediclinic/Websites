using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class ConditionDB
{

    public static void Delete(int condition_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Condition WHERE condition_id = " + condition_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string descr, bool show_date, bool show_nweeksdue, bool show_text, int display_order, bool is_deleted)
    {
        descr = descr.Replace("'", "''");
        string sql = "INSERT INTO Condition (descr,show_date,show_nweeksdue,show_text,display_order,is_deleted) VALUES (" + "'" + descr + "'," + (show_date ? "1" : "0") + "," + (show_nweeksdue ? "1" : "0") + "," + (show_text ? "1" : "0") + "," + display_order + "," + (is_deleted ? "1" : "0") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int condition_id, string descr, bool show_date, bool show_nweeksdue, bool show_text, int display_order, bool is_deleted)
    {
        descr = descr.Replace("'", "''");
        string sql = "UPDATE Condition SET descr = '" + descr + "',show_date = " + (show_date ? "1" : "0") + ",show_nweeksdue = " + (show_nweeksdue ? "1" : "0") + ",show_text = " + (show_text ? "1" : "0") + ",display_order = " + display_order + ",is_deleted = " + (is_deleted ? "1" : "0") + " WHERE condition_id = " + condition_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int condition_id)
    {
        string sql = "UPDATE Condition SET is_deleted = 1 WHERE condition_id = " + condition_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int condition_id)
    {
        string sql = "UPDATE Condition SET is_deleted = 0 WHERE condition_id = " + condition_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateDisplayOrderAll(int amount)
    {
        string sql = "UPDATE Condition SET display_order = display_order + (" + amount + "); UPDATE Condition SET display_order = 0 WHERE display_order < 0;";
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable(bool inc_deleted = false)
    {
        string sql = JoinedSql + (inc_deleted ? "" : " WHERE condition.is_deleted = 0") + " ORDER BY condition.display_order, condition.descr";
        return DBBase.ExecuteQuery( sql ).Tables[0];
    }
    public static Condition[] GetAll(bool inc_deleted = false)
    {
        DataTable tbl = GetDataTable(inc_deleted);
        Condition[] list = new Condition[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = ConditionDB.Load(tbl.Rows[i], "condition_");

        return list;
    }

    public static Hashtable GetHashtable(bool inc_deleted = false)
    {
        Hashtable hash = new Hashtable();
        foreach (Condition condition in ConditionDB.GetAll(inc_deleted))
            hash[condition.ConditionID] = condition;
        return hash;
    }


    public static Condition GetByID(int condition_id)
    {
        string sql = JoinedSql + " WHERE condition.condition_id = " + condition_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery( sql ).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0], "condition_");
    }


    private static string JoinedSql = @"
            SELECT 
                condition.condition_id   as condition_condition_id,
                condition.descr          as condition_descr,
                condition.show_date      as condition_show_date, 
                condition.show_nweeksdue as condition_show_nweeksdue, 
                condition.show_text      as condition_show_text,
                condition.display_order  as condition_display_order,
                condition.is_deleted     as condition_is_deleted
            FROM 
                Condition condition ";

    public static Condition Load(DataRow row, string prefix = "")
    {
        return new Condition(
            Convert.ToInt32(row[prefix + "condition_id"]),
            Convert.ToString(row[prefix + "descr"]),
            Convert.ToBoolean(row[prefix + "show_date"]),
            Convert.ToBoolean(row[prefix + "show_nweeksdue"]),
            Convert.ToBoolean(row[prefix + "show_text"]),
            Convert.ToInt32(row[prefix + "display_order"]),
            Convert.ToBoolean(row[prefix + "is_deleted"])
        );
    }

}