using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class AddressChannelDB
{

    public static void Delete(int address_channel_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM AddressChannel WHERE address_channel_id = " + address_channel_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string descr, int address_channel_type_id)
    {
        descr = descr.Replace("'", "''");
        string sql = "INSERT INTO AddressChannel (descr,address_channel_type_id,address_channel_date_modified) VALUES (" + "'" + descr + "'," + "" + address_channel_type_id + "," +  "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int address_channel_id, string descr, int address_channel_type_id)
    {
        descr = descr.Replace("'", "''");
        string sql = "UPDATE AddressChannel SET descr = '" + descr + "',address_channel_type_id = " + address_channel_type_id + ",address_channel_date_modified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE address_channel_id = " + address_channel_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    static string JoinedSql = @"
                SELECT  ac.address_channel_id as ac_address_channel_id,ac.descr as ac_descr,ac.address_channel_type_id as ac_address_channel_type_id,ac.address_channel_date_added as ac_address_channel_date_added,ac.address_channel_date_modified as ac_address_channel_date_modified, 
                        act.address_channel_type_id as act_address_channel_type_id,act.descr as act_descr
                FROM    AddressChannel ac
                        INNER JOIN AddressChannelType act ON  ac.address_channel_type_id = act.address_channel_type_id ";

    public static DataTable GetDataTable()
    {
        string sql = JoinedSql + " ORDER BY ac.descr, act.descr";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static DataTable GetDataTable_StartingWith(string search)
    {
        string sql = JoinedSql + " WHERE ac.descr LIKE '" + search + "%' ";
        sql += " ORDER BY ac.descr, act.descr";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }


    public static AddressChannel GetByID(int address_channel_id)
    {
        string sql = JoinedSql + " WHERE address_channel_id = " + address_channel_id.ToString() + " ORDER BY ac_descr, act_descr";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        if (tbl.Rows.Count == 0)
            return null;
        else
            return LoadAll(tbl.Rows[0]);
    }


    public static AddressChannel LoadAll(DataRow row)
    {
        AddressChannel ac = Load(row, "ac_");
        ac.AddressChannelType = IDandDescrDB.Load(row, "act_address_channel_type_id", "act_descr");
        return ac;
    }

    public static AddressChannel Load(DataRow row, string prefix = "")
    {
        return new AddressChannel(
            Convert.ToInt32(row[prefix+"address_channel_id"]),
            Convert.ToString(row[prefix + "descr"]),
            Convert.ToInt32(row[prefix + "address_channel_type_id"]),
            row[prefix + "address_channel_date_added"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "address_channel_date_added"]),
            row[prefix + "address_channel_date_modified"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "address_channel_date_modified"])
        );
    }

}