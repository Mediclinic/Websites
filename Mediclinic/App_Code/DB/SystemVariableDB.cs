using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class SystemVariableDB
{

    public static void Delete(string descr)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM SystemVariable WHERE descr = " + descr.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void Insert(string descr, string value, bool editable_in_gui, bool viewable_in_gui)
    {
        descr = descr.Replace("'", "''");
        value = value.Replace("'", "''");
        string sql = "INSERT INTO SystemVariable (descr,value,editable_in_gui,viewable_in_gui) VALUES (" + "'" + descr + "','" + value + "'," + (editable_in_gui ? "1," : "0,") + (viewable_in_gui ? "1" : "0") + ");SELECT SCOPE_IDENTITY();";
        DBBase.ExecuteNonResult(sql);
    }

    public static void Update(string descr, string value, bool editable_in_gui, bool viewable_in_gui, string DB = null)
    {
        descr = descr.Replace("'", "''");
        value = value.Replace("'", "''");
        string sql = "UPDATE SystemVariable SET value = '" + value + "',editable_in_gui = " + (editable_in_gui ? "1," : "0,") + "viewable_in_gui = " + (viewable_in_gui ? "1" : "0") + " WHERE descr = '" + descr.ToString() + "'";
        DBBase.ExecuteNonResult(sql);
    }

    public static void Update(string descr, string value, string DB = null)
    {
        descr = descr.Replace("'", "''");
        value = value.Replace("'", "''");
        string sql = "UPDATE SystemVariable SET value = '" + value + "'" + " WHERE descr = '" + descr.ToString() + "'";
        DBBase.ExecuteNonResult(sql, DB);
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT descr,value,editable_in_gui,viewable_in_gui FROM SystemVariable";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static SystemVariable GetByDescr(string descr)
    {
        descr = descr.Replace("'", "''");
        string sql = "SELECT descr,value,editable_in_gui,viewable_in_gui FROM SystemVariable WHERE descr = '" + descr.ToString() + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static SystemVariables GetAll(string DB = null)
    {
        string sql = "SELECT descr,value,editable_in_gui,viewable_in_gui FROM SystemVariable";
        DataTable tbl = DBBase.ExecuteQuery(sql, DB).Tables[0];

        SystemVariables systemVariables = new SystemVariables();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            SystemVariable var = Load(tbl.Rows[i]);
            systemVariables[var.Descr] = var;
        }
        return systemVariables;
    }

    public static SystemVariable Load(DataRow row)
    {
        return new SystemVariable(
            Convert.ToString(row["descr"]),
            Convert.ToString(row["value"]),
            Convert.ToBoolean(row["editable_in_gui"]),
            Convert.ToBoolean(row["viewable_in_gui"])
        );
    }

}

