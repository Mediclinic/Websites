using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class SuburbDB
{

    public static void Delete(int suburb_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Suburb WHERE suburb_id = " + suburb_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string name, string postcode, string state)
    {
        name = name.Replace("'", "''");
        postcode = postcode.Replace("'", "''");
        state = state.Replace("'", "''");
        string sql = "INSERT INTO Suburb (name,postcode,state,previous) VALUES (" + "'" + name + "'," + "'" + postcode + "'," + "'" + state + "','');SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int suburb_id, string name, string postcode, string state, int amended_by, string previous)
    {
        name = name.Replace("'", "''");
        postcode = postcode.Replace("'", "''");
        state = state.Replace("'", "''");
        previous = previous.Replace("'", "''");
        string sql = "UPDATE Suburb SET name = '" + name + "',postcode = '" + postcode + "',state = '" + state + "',amended_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",amended_by = " + (amended_by == -1 ? "NULL" : amended_by.ToString()) + ",previous = '" + previous + "' WHERE suburb_id = " + suburb_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static bool PostCodeExists(string postcode, int id_to_ignore)
    {
        postcode = postcode.Replace("'", "''");

        string sql = "SELECT COUNT(*) FROM Suburb WHERE postcode = '" + postcode + "' ";
        if (id_to_ignore != -1)
            sql += " AND suburb_id <> " + id_to_ignore;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }
    public static bool Exists(string name, string postcode, string state, int id_to_ignore)
    {
        name = name.Replace("'", "''");
        postcode = postcode.Replace("'", "''");
        state = state.Replace("'", "''");

        string sql = "SELECT COUNT(*) FROM Suburb WHERE name = '" + name + "' AND postcode = '" + postcode + "' AND state = '" + state + "' ";
        if (id_to_ignore != -1)
            sql += " AND suburb_id <> " + id_to_ignore;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static DataTable GetDataTable(bool incEmptyName = false, string nameSearch = null, bool searchNameOnlyStartsWith = false, string poscodeSearch = null, string stateSearch = null)
    {
        if (nameSearch    != null) nameSearch    = nameSearch.Replace("'", "''");
        if (poscodeSearch != null) poscodeSearch = poscodeSearch.Replace("'", "''");
        if (stateSearch   != null) stateSearch   = stateSearch.Replace("'", "''");

        string sql = "SELECT suburb_id,name,postcode,state,amended_date,amended_by,previous FROM Suburb ";

        string whereClause = string.Empty;
        if (!incEmptyName)                                     whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " name <> '' ";
        if (nameSearch    != null && !searchNameOnlyStartsWith && nameSearch.Length > 0) whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " name LIKE '%" + nameSearch + "%' ";
        if (nameSearch    != null &&  searchNameOnlyStartsWith && nameSearch.Length > 0) whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " name LIKE '"  + nameSearch + "%' ";
        if (poscodeSearch != null && poscodeSearch.Length > 0) whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " postcode LIKE '%" + poscodeSearch + "%' ";
        if (stateSearch   != null)                             whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " state    =    '"  + stateSearch   + "' ";

        sql += whereClause + " ORDER BY name";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static DataTable GetDataTable_StartingWith(string search)
    {
        search = search.Replace("'", "''");

        string sql = "SELECT suburb_id,name,postcode,state,amended_date,amended_by,previous FROM Suburb WHERE name LIKE '"+search+"%' ";
        sql += " ORDER BY name";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    


    public static Suburb GetByID(int suburb_id, bool incEmpty = false)
    {
        string sql = "SELECT suburb_id,name,postcode,state,amended_date,amended_by,previous FROM Suburb WHERE suburb_id = " + suburb_id.ToString() + (!incEmpty ? " AND name <> ''" : "") + " ORDER BY name";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static Suburb Load(DataRow row, string prefix = "")
    {
        return new Suburb(
            Convert.ToInt32(row[prefix+"suburb_id"]),
            Convert.ToString(row[prefix + "name"]),
            Convert.ToString(row[prefix + "postcode"]),
            Convert.ToString(row[prefix + "state"]),
            row[prefix + "amended_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "amended_date"]),
            row[prefix + "amended_by"]   == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "amended_by"]),
            Convert.ToString(row[prefix + "previous"])
        );
    }

}