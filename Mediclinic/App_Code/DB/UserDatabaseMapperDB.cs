using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class UserDatabaseMapperDB
{

    public static void Delete(int id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM UserDatabaseMapper WHERE id = " + id.ToString(), "Mediclinic_Main");
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void Delete(string dbname)
    {
        DBBase.ExecuteNonResult("DELETE FROM UserDatabaseMapper WHERE dbname = '" + dbname + "'", "Mediclinic_Main");
    }
    public static int Insert(string username, string dbname)
    {
        username = username.Replace("'", "''");
        dbname = dbname.Replace("'", "''");
        string sql = "INSERT INTO UserDatabaseMapper (username,dbname) VALUES (" + "'" + username + "'," + "'" + dbname + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql, "Mediclinic_Main"));
    }

    public static void Update(int id, string username, string dbname)
    {
        username = username.Replace("'", "''");
        dbname = dbname.Replace("'", "''");
        string sql = "UPDATE UserDatabaseMapper SET username = '" + username + "',dbname = '" + dbname + "' WHERE id = " + id.ToString();
        DBBase.ExecuteNonResult(sql, "Mediclinic_Main");
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT id,username,dbname FROM UserDatabaseMapper";
        return DBBase.ExecuteQuery(sql, "Mediclinic_Main").Tables[0];
    }
    public static UserDatabaseMapper[] GetAll()
    {
        DataTable tbl = GetDataTable();
        
        UserDatabaseMapper[] all = new UserDatabaseMapper[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            all[i] = Load(tbl.Rows[i]);

        return all;
    }

    public static UserDatabaseMapper GetByID(int id)
    {
        string sql = "SELECT id,username,dbname FROM UserDatabaseMapper WHERE id = " + id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql, "Mediclinic_Main").Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static UserDatabaseMapper GetByLogin(string username, string dbname = null)
    {
        username = username.Replace("'", "''");

        string sql = "SELECT id,username,dbname FROM UserDatabaseMapper WHERE username COLLATE Latin1_General_CS_AS = '" + username + "'" + (dbname == null ? "" : " AND dbname = '" + dbname + "'");
        DataTable tbl = DBBase.ExecuteQuery(sql, "Mediclinic_Main").Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }
    public static bool UsernameExists(string username)
    {
        username = username.Replace("'", "''");
        string sql = "SELECT COUNT(*) FROM UserDatabaseMapper WHERE username COLLATE Latin1_General_CS_AS = '" + username + "'";
        return (Convert.ToInt32(DBBase.ExecuteSingleResult(sql, "Mediclinic_Main")) > 0);
    }
    public static bool UsernameExists(UserDatabaseMapper[] list, string username)
    {
        foreach (UserDatabaseMapper user in list)
            if (user.Username == username)
                return true;

        return false;
    }

    public static UserDatabaseMapper Load(DataRow row)
    {
        return new UserDatabaseMapper(
            Convert.ToInt32(row["id"]),
            Convert.ToString(row["username"]),
            Convert.ToString(row["dbname"])
        );
    }

}