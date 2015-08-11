using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class BookingChangeHistoryReasonDB
{

    public static void Delete(int booking_change_history_reason_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM BookingChangeHistoryReason WHERE booking_change_history_reason_id = " + booking_change_history_reason_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string descr, int display_order)
    {
        descr = descr.Replace("'", "''");
        string sql = "INSERT INTO BookingChangeHistoryReason (descr,display_order) VALUES (" + "'" + descr + "'," + "" + display_order + "" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int booking_change_history_reason_id, string descr, int display_order)
    {
        descr = descr.Replace("'", "''");
        string sql = "UPDATE BookingChangeHistoryReason SET descr = '" + descr + "',display_order = " + display_order + " WHERE booking_change_history_reason_id = " + booking_change_history_reason_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateDisplayOrderAll(int amount)
    {
        string sql = "UPDATE BookingChangeHistoryReason SET display_order = display_order + (" + amount + "); UPDATE BookingChangeHistoryReason SET display_order = 0 WHERE display_order < 0;";
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT booking_change_history_reason_id,descr,display_order FROM BookingChangeHistoryReason ORDER BY display_order, descr";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static BookingChangeHistoryReason GetByID(int booking_change_history_reason_id)
    {
        string sql = "SELECT booking_change_history_reason_id,descr,display_order FROM BookingChangeHistoryReason WHERE booking_change_history_reason_id = " + booking_change_history_reason_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static BookingChangeHistoryReason Load(DataRow row)
    {
        return new BookingChangeHistoryReason(
            Convert.ToInt32(row["booking_change_history_reason_id"]),
            Convert.ToString(row["descr"]),
            Convert.ToInt32(row["display_order"])
        );
    }

}