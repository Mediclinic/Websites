using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class LetterBestDB
{

    public static void Delete(int letter_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM LetterBest WHERE letter_id = " + letter_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int organisation_id, int letter_type_id, int site_id, string code, string docname, bool is_send_to_medico, bool is_allowed_reclaim, bool is_manual_override, int num_copies_to_print)
    {
        code = code.Replace("'", "''");
        docname = docname.Replace("'", "''");
        string sql = "INSERT INTO LetterBest (organisation_id,letter_type_id,site_id,code,docname,is_send_to_medico,is_allowed_reclaim,is_manual_override,num_copies_to_print) VALUES (" + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + "," + letter_type_id + "," + site_id + "," + "'" + code + "'," + "'" + docname + "'," + (is_send_to_medico ? "1," : "0,") + (is_allowed_reclaim ? "1," : "0,") + (is_manual_override ? "1," : "0,") + "" + num_copies_to_print + "" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int letter_id, int organisation_id, int letter_type_id, int site_id, string code, string docname, bool is_send_to_medico, bool is_allowed_reclaim, bool is_manual_override, int num_copies_to_print)
    {
        code = code.Replace("'", "''");
        docname = docname.Replace("'", "''");
        string sql = "UPDATE LetterBest SET organisation_id = " + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + ",letter_type_id = " + letter_type_id + ",site_id = " + site_id + ",code = '" + code + "',docname = '" + docname + "',is_send_to_medico = " + (is_send_to_medico ? "1," : "0,") + "is_allowed_reclaim = " + (is_allowed_reclaim ? "1," : "0,") + "is_manual_override = " + (is_manual_override ? "1," : "0,") + "num_copies_to_print = " + num_copies_to_print + " WHERE letter_id = " + letter_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static DataTable GetDataTable()
    {
        string sql = JoinedSql + " ORDER BY lettertype.descr";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable_ByLetterType(int letter_type_id)
    {
        string sql = JoinedSql + " WHERE letter.letter_type_id = " + letter_type_id.ToString();
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static LetterBest GetByID(int letter_id)
    {
        string sql = JoinedSql + " WHERE letter.letter_id = " + letter_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static string JoinedSql = @"
            SELECT
                    letter.letter_id as letter_letter_id,letter.organisation_id as letter_organisation_id,letter.letter_type_id as letter_letter_type_id,letter.site_id as letter_site_id,letter.code as letter_code,letter.docname as letter_docname,letter.is_send_to_medico as letter_is_send_to_medico,letter.is_allowed_reclaim as letter_is_allowed_reclaim,letter.is_manual_override as letter_is_manual_override,letter.num_copies_to_print as letter_num_copies_to_print,
                    lettertype.descr as lettertype_descr, lettertype.letter_type_id as lettertype_letter_type_id
            FROM
                    Letter letterBest
                    INNER JOIN LetterType lettertype ON letter.letter_type_id = lettertype.letter_type_id ";


    public static LetterBest LoadAll(DataRow row)
    {
        LetterBest letter = Load(row, "letter_");
        letter.LetterType = IDandDescrDB.Load(row, "lettertype_letter_type_id", "lettertype_descr");
        return letter;
    }

    public static LetterBest Load(DataRow row, string prefix = "")
    {
        return new LetterBest(
            Convert.ToInt32(row[prefix + "letter_id"]),
            Convert.ToInt32(row[prefix + "letter_type_id"]),
            Convert.ToString(row[prefix + "code"]),
            Convert.ToString(row[prefix + "docname"]),
            Convert.ToBoolean(row[prefix + "is_send_to_medico"]),
            Convert.ToBoolean(row[prefix + "is_allowed_reclaim"]),
            Convert.ToBoolean(row[prefix + "is_manual_override"]),
            Convert.ToInt32(row[prefix + "num_copies_to_print"])
        );
    }

}