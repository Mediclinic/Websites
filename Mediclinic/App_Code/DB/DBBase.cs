using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;

public class DBBase
{

    public static string GetConnectionString(string DB = null)
    {
        if (DB == null)
            DB = System.Web.HttpContext.Current.Session["DB"].ToString();

        if (DB == "master")
            return @"Server=(local);Database=master;Integrated Security=SSPI;";
        else
            return @"Data Source=(local);Initial Catalog=" + DB + @";Integrated Security=True;MultipleActiveResultSets=True";
    }

    public static void ExecuteNonResult(string sql, string DB = null)
    {
        int startTime = Environment.TickCount;
        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(sql, Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
            else if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]))
                Logger.LogCallingMethod(true);

            /*
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(GetConnectionString(DB));

            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            cmd.Connection = con;

            con.Open();
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
            con.Dispose();
            */

            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(GetConnectionString(DB)))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Connection  = con;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

        }
        catch (Exception ex)
        {
            Logger.LogQuery("SQL Exception:" + Environment.NewLine + Environment.NewLine + sql + Environment.NewLine + Environment.NewLine + ex.ToString(), true, true);
            throw;
        }
        finally
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(((double)(Environment.TickCount - startTime) / 1000.0).ToString() + " seconds", Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
        }
    }

    public static DataSet ExecuteQuery(string sql, string DB = null, int timeoutSeconds = -1)
    {
        int startTime = Environment.TickCount;
        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(sql, Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
            else if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]))
                Logger.LogCallingMethod(true);

            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = GetConnectionString(DB);
            con.Open();

            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sql, con);
            if (timeoutSeconds != -1)
                da.SelectCommand.CommandTimeout = timeoutSeconds;
            DataSet ds1 = new DataSet();
            da.Fill(ds1);

            da.Dispose();
            con.Close();
            con.Dispose();

            return ds1;
        }
        catch (Exception ex)
        {
            Logger.LogQuery("SQL Exception:" + Environment.NewLine + Environment.NewLine + sql + Environment.NewLine + Environment.NewLine + ex.ToString(), true, true);
            throw;
        }
        finally
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(((double)(Environment.TickCount - startTime) / 1000.0).ToString() + " seconds", Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
        }
    }

    public static object ExecuteSingleResult(string sql, string DB = null)
    {
        int startTime = Environment.TickCount;
        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(sql, Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
            else if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]))
                Logger.LogCallingMethod(true);

            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = GetConnectionString(DB);
            con.Open();

            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sql, con);
            DataSet ds1 = new DataSet();
            da.Fill(ds1);

            da.Dispose();
            con.Close();
            con.Dispose();

            return ds1.Tables[0].Rows[0][0];
        }
        catch (Exception ex)
        {
            Logger.LogQuery("SQL Exception:" + Environment.NewLine + Environment.NewLine + sql + Environment.NewLine + Environment.NewLine + ex.ToString(), true, true);
            throw;
        }
        finally
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(((double)(Environment.TickCount - startTime) / 1000.0).ToString() + " seconds", Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
        }
    }



    public static object ExecuteSingleResult_SP(string spName, System.Collections.Hashtable spParams, string DB = null)
    {
        int startTime = Environment.TickCount;
        try
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
            {
                string _params = "";
                foreach(string param in spParams)
                    _params += param + ":" + spParams[param] + Environment.NewLine;
                Logger.LogQuery("Stored Proc: " + spName + Environment.NewLine + "Params: " + Environment.NewLine + _params, false, false, true);
            }

            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = GetConnectionString(DB);
            con.Open();

            System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();
            command.Connection = con;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;

            foreach(string key in spParams.Keys)
                command.Parameters.AddWithValue(key, spParams[key]);

            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(command);
            DataSet ds1 = new DataSet();
            da.Fill(ds1);

            da.Dispose();
            con.Close();
            con.Dispose();

            return ds1.Tables[0].Rows[0][0];
        }
        finally
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(((double)(Environment.TickCount - startTime) / 1000.0).ToString() + " seconds", Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
        }

    }


    public static DataTable GetGenericDataTable(string DB, string tableName, params string[] fields)
    {
        int startTime = Environment.TickCount;
        try
        {
            string sql = "SELECT " + String.Join(",", fields) + " FROM " + tableName;

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(sql, Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
            else if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]))
                Logger.LogCallingMethod(true);


            DataTable tbl = DBBase.ExecuteQuery(sql, DB).Tables[0];
            return tbl.Copy();

        }
        finally
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(((double)(Environment.TickCount - startTime) / 1000.0).ToString() + " seconds", Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
        }
    }

    public static DataTable GetGenericDataTable_WithWhereOrderClause(string DB, string tableName, string whereClause, string orderClause, params string[] fields)
    {
        int startTime = Environment.TickCount;
        try
        {
            string sql = "SELECT " + String.Join(",", fields) + " FROM " + tableName;
            if (whereClause != null && whereClause.Length > 0)
                sql += " WHERE " + whereClause;
            if (orderClause != null && orderClause.Length > 0)
                sql += " ORDER BY " + orderClause;

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(sql, Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
            else if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]))
                Logger.LogCallingMethod(true);


            DataTable tbl = DBBase.ExecuteQuery(sql, DB).Tables[0];
            return tbl;
        }
        finally
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(((double)(Environment.TickCount - startTime) / 1000.0).ToString() + " seconds", Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
        }
    }


    public static int GetGenericCount(string tableName, string whereClause = null)
    {
        int startTime = Environment.TickCount;
        try
        {
            string sql = "SELECT COUNT(*) FROM " + tableName + " ";
            if (whereClause != null && whereClause.Length > 0)
                sql += " WHERE " + whereClause;

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(sql, Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
            else if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]))
                Logger.LogCallingMethod(true);

            int result = Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
            return result;
        }
        finally
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSql"]))
                Logger.LogQuery(((double)(Environment.TickCount - startTime) / 1000.0).ToString() + " seconds", Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllSqlStackTraces"]), false, true);
        }

    }


    public static string GetFields(string[] fieldsList, string fieldsPrefix = "", string tableAlias = "")
    {
        for (int i = 0; i < fieldsList.Length; i++)
            fieldsList[i] = (tableAlias.Length > 0 ? tableAlias + "." : "") + fieldsList[i] + (fieldsPrefix.Length > 0 ? " AS " + fieldsPrefix + fieldsList[i] : "");

        return String.Join(", ", fieldsList);
    }

}