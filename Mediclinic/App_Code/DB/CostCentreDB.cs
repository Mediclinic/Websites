using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class CostCentreDB
{

    public static void Delete(int costcentre_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM CostCentre WHERE costcentre_id = " + costcentre_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static int Insert(string descr, int parent_id)
    {
        descr = descr.Replace("'", "''");
        string sql = "INSERT INTO CostCentre (descr,parent_id) VALUES (" + "'" + descr + "'," + "" + (parent_id < 0 ? "NULL" : parent_id.ToString()) + "" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int costcentre_id, string descr, int parent_id)
    {
        descr = descr.Replace("'", "''");
        string sql = "UPDATE CostCentre SET descr = '" + descr + "',parent_id = " + (parent_id < 0 ? "NULL" : parent_id.ToString()) + " WHERE costcentre_id = " + costcentre_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static DataTable GetDataTable()
    {
        string sql = "SELECT costcentre_id,descr,parent_id FROM CostCentre";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return tbl;
    }

    public static CostCentre GetByID(int costcentre_id)
    {
        string sql = "SELECT costcentre_id,descr,parent_id FROM CostCentre WHERE costcentre_id = " + costcentre_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static CostCentre Load(DataRow row, string prefix="")
    {
        return new CostCentre(
            Convert.ToInt32(row[prefix+"costcentre_id"]),
            Convert.ToString(row[prefix+"descr"]),
            row[prefix+"parent_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix+"parent_id"])
        );
    }



    public static CostCentre[] GetTree()
    {
        DataTable dt = GetDataTable();
        return GetChildren(dt, "parent_id is NULL");
    }

    public static CostCentre[] GetChildren(DataTable dt, string query)
    {
        DataRow[] foundRows = dt.Select(query);
        CostCentre[] children = new CostCentre[foundRows.Length];
        for (int i = 0; i < foundRows.Length; i++)
        {
            CostCentre cc = CostCentreDB.Load(foundRows[i]);
            cc.Children = GetChildren(dt, "parent_id = " + cc.CostCentreID.ToString());
            children[i] = cc;
        }

        return children;
    }

}