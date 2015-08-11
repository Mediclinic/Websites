using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class ContactTypeDB
{

    public static void Delete(int contact_type_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM ContactType WHERE contact_type_id = " + contact_type_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int contact_type_group_id, string descr, int display_order)
    {
        descr = descr.Replace("'", "''");
        string sql = "INSERT INTO ContactType (contact_type_group_id,descr,display_order) VALUES (" + "" + contact_type_group_id + "," + "'" + descr + "'" + "," + display_order + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int contact_type_id, int contact_type_group_id, string descr, int display_order)
    {
        descr = descr.Replace("'", "''");
        string sql = "UPDATE ContactType SET contact_type_group_id = " + contact_type_group_id + ",descr = '" + descr + "',display_order = " + display_order + " WHERE contact_type_id = " + contact_type_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    static string JoinedSql = @"
                SELECT  at.contact_type_id as at_contact_type_id,at.contact_type_group_id as at_contact_type_group_id,at.descr as at_descr,at.display_order as at_display_order,
                        atg.contact_type_group_id as atg_contact_type_group_id,atg.descr as atg_descr
                FROM    ContactType at
                        INNER JOIN ContactTypeGroup atg ON at.contact_type_group_id = atg.contact_type_group_id ";


    public static DataTable GetDataTable(int contact_type_group_id = -1)
    {
        string sql = JoinedSql;
        if (contact_type_group_id != -1)
            sql += " WHERE at.contact_type_group_id = " + contact_type_group_id;
        sql += " ORDER BY at.contact_type_group_id,at.display_order";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static DataTable GetDataTable(string contact_type_group_ids = null)
    {
        string sql = JoinedSql;
        if (contact_type_group_ids != null && contact_type_group_ids.Length > 0)
            sql += " WHERE at.contact_type_group_id IN (" + contact_type_group_ids + ") ";
        sql += " ORDER BY at.contact_type_group_id, at.display_order";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static ContactType GetByID(int contact_type_id)
    {
        string sql = JoinedSql + " WHERE at.contact_type_id = " + contact_type_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0], "at_");
    }


    public static ContactType LoadAll(DataRow row)
    {
        ContactType at = Load(row, "at_");
        at.ContactTypeGroup = IDandDescrDB.Load(row, "atg_contact_type_group_id", "atg_descr");
        return at;
    }

    public static ContactType Load(DataRow row, string prefix = "")
    {
        return new ContactType(
            Convert.ToInt32(row[prefix+"contact_type_id"]),
            Convert.ToInt32(row[prefix + "contact_type_group_id"]),
            Convert.ToString(row[prefix + "descr"]),
            Convert.ToInt32(row[prefix + "display_order"])
        );
    }

}