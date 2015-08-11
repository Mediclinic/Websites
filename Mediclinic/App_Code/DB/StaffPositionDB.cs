using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class StaffPositionDB
{

    public static void Delete(int staff_position_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM StaffPosition WHERE staff_position_id = " + staff_position_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string descr)
    {
        descr = descr.Replace("'", "''");
        string sql = "INSERT INTO StaffPosition (descr) VALUES (" + "'" + descr + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int staff_position_id, string descr)
    {
        descr = descr.Replace("'", "''");
        string sql = "UPDATE StaffPosition SET descr = '" + descr + "' WHERE staff_position_id = " + staff_position_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT staff_position_id,descr FROM StaffPosition ORDER BY descr";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return tbl;
    }

    public static StaffPosition GetByID(int staff_position_id)
    {
        string sql = "SELECT staff_position_id,descr FROM StaffPosition WHERE staff_position_id = " + staff_position_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }
    public static StaffPosition GetByDescr(string descr)
    {
        descr = descr.Replace("'", "''");
        string sql = "SELECT staff_position_id,descr FROM StaffPosition WHERE descr = '" + descr.ToString() + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static StaffPosition Load(DataRow row, string prefix="")
    {
        return new StaffPosition(
            Convert.ToInt32(row[prefix+"staff_position_id"]),
            Convert.ToString(row[prefix+"descr"])
        );
    }

}