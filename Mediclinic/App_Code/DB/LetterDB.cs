using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class LetterDB
{

    public static void Delete(int letter_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Letter WHERE letter_id = " + letter_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int organisation_id, int letter_type_id, int site_id, string code, string reject_message, string docname, bool is_send_to_medico, bool is_allowed_reclaim, bool is_manual_override, int num_copies_to_print, bool is_deleted)
    {
        code = code.Replace("'", "''");
        reject_message = reject_message.Replace("'", "''");
        docname = docname.Replace("'", "''");
        string sql = "INSERT INTO Letter (organisation_id,letter_type_id,site_id,code,reject_message,docname,is_send_to_medico,is_allowed_reclaim,is_manual_override,num_copies_to_print, is_deleted) VALUES (" + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + "," + letter_type_id + "," + site_id + "," + "'" + code + "','" + reject_message + "','" + docname + "'," + (is_send_to_medico ? "1," : "0,") + (is_allowed_reclaim ? "1," : "0,") + (is_manual_override ? "1," : "0,") + "" + num_copies_to_print + "," + (is_deleted ? "1" : "0") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int letter_id, int organisation_id, int letter_type_id, int site_id, string code, string reject_message, string docname, bool is_send_to_medico, bool is_allowed_reclaim, bool is_manual_override, int num_copies_to_print, bool is_deleted)
    {
        code = code.Replace("'", "''");
        reject_message = reject_message.Replace("'", "''");
        docname = docname.Replace("'", "''");
        string sql = "UPDATE Letter SET organisation_id = " + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + ",letter_type_id = " + letter_type_id + ",site_id = " + site_id + ",code = '" + code + "',reject_message = '" + reject_message + "',docname = '" + docname + "',is_send_to_medico = " + (is_send_to_medico ? "1," : "0,") + "is_allowed_reclaim = " + (is_allowed_reclaim ? "1," : "0,") + "is_manual_override = " + (is_manual_override ? "1," : "0,") + "num_copies_to_print = " + num_copies_to_print + ",is_deleted = " + (is_deleted ? "1" : "0") + " WHERE letter_id = " + letter_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void SetAsDeleted(int letter_id, bool is_deleted)
    {
        string sql = "UPDATE Letter SET is_deleted = " + (is_deleted ? "1" : ")") + " WHERE letter_id = " + letter_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static bool OrgHasdocs(int organisation_id)
    {
        return GetCountByOrg(organisation_id) > 0;
    }
    public static int GetCountByOrg(int organisation_id)
    {
        string sql = "SELECT COUNT(*) FROM Letter WHERE organisation_id = " + organisation_id;
        return Convert.ToInt32( DBBase.ExecuteSingleResult(sql) );
    }

    public static DataTable GetDataTable(int site_id = -1)
    {
        string sql = JoinedSql() + (site_id == -1 ? "" : " AND letter.site_id = " + site_id) + " ORDER BY letter.organisation_id, lettertype.descr, letter.code, letter.docname";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Hashtable GetHashTable(int site_id = -1)
    {
        Hashtable hash = new Hashtable();

        DataTable tbl = GetDataTable(site_id);
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Letter letter = LetterDB.LoadAll(tbl.Rows[i]);
            hash[letter.LetterID] = letter;
        }

        return hash;
    }

    public static DataTable GetDataTable_ByOrg(int organisation_id, int site_id = -1)
    {
        string sql = JoinedSql() + (organisation_id ==  0 ? " AND letter.organisation_id IS NULL" : " AND letter.organisation_id = " + organisation_id) +
                                                 (site_id         == -1 ? "" : " AND letter.site_id = " + site_id);
        sql += " ORDER BY lettertype.descr, letter.code, letter.docname";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable_ByLetterType(int letter_type_id, int org_id = 0)
    {
        string sql = JoinedSql() + " AND letter.letter_type_id = " + letter_type_id.ToString() + (org_id == 0 ? " AND letter.organisation_id IS NULL" : " AND letter.organisation_id = " + org_id);
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Letter GetByID(int letter_id, string DB = null)
    {
        string sql = JoinedSql() + " AND letter.letter_id = " + letter_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql, DB).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static string JoinedSql(bool incDeleted = false)
    {
        return @"
            SELECT
                    letter.letter_id as letter_letter_id,letter.organisation_id as letter_organisation_id,letter.letter_type_id as letter_letter_type_id,letter.site_id as letter_site_id,letter.code as letter_code,letter.reject_message as letter_reject_message,letter.docname as letter_docname,letter.is_send_to_medico as letter_is_send_to_medico,letter.is_allowed_reclaim as letter_is_allowed_reclaim,letter.is_manual_override as letter_is_manual_override,letter.num_copies_to_print as letter_num_copies_to_print, letter.is_deleted as letter_is_deleted,

                    lettertype.descr as lettertype_descr, lettertype.letter_type_id as lettertype_letter_type_id,

                    letterorg.organisation_id as letterorg_organisation_id,letterorg.entity_id as letterorg_entity_id, letterorg.parent_organisation_id as letterorg_parent_organisation_id, letterorg.use_parent_offernig_prices as letterorg_use_parent_offernig_prices, 
                    letterorg.organisation_type_id as letterorg_organisation_type_id, letterorg.organisation_customer_type_id as letterorg_organisation_customer_type_id, letterorg.name as letterorg_name, letterorg.acn as letterorg_acn, letterorg.abn as letterorg_abn, letterorg.organisation_date_added as letterorg_organisation_date_added, letterorg.organisation_date_modified as letterorg_organisation_date_modified, letterorg.is_debtor as letterorg_is_debtor, letterorg.is_creditor as letterorg_is_creditor, letterorg.bpay_account as letterorg_bpay_account, 
                    letterorg.weeks_per_service_cycle as letterorg_weeks_per_service_cycle, letterorg.start_date as letterorg_start_date, letterorg.end_date as letterorg_end_date, letterorg.comment as letterorg_comment, letterorg.free_services as letterorg_free_services, letterorg.excl_sun as letterorg_excl_sun, letterorg.excl_mon as letterorg_excl_mon, letterorg.excl_tue as letterorg_excl_tue, letterorg.excl_wed as letterorg_excl_wed, letterorg.excl_thu as letterorg_excl_thu, letterorg.excl_fri as letterorg_excl_fri, 
                    letterorg.excl_sat as letterorg_excl_sat, 
                    letterorg.sun_start_time as letterorg_sun_start_time, letterorg.sun_end_time as letterorg_sun_end_time, letterorg.mon_start_time as letterorg_mon_start_time, letterorg.mon_end_time as letterorg_mon_end_time, letterorg.tue_start_time as letterorg_tue_start_time, letterorg.tue_end_time as letterorg_tue_end_time, letterorg.wed_start_time as letterorg_wed_start_time, letterorg.wed_end_time as letterorg_wed_end_time, 
                    letterorg.thu_start_time as letterorg_thu_start_time, letterorg.thu_end_time as letterorg_thu_end_time, letterorg.fri_start_time as letterorg_fri_start_time, letterorg.fri_end_time as letterorg_fri_end_time, letterorg.sat_start_time as letterorg_sat_start_time, letterorg.sat_end_time as letterorg_sat_end_time, 
                    letterorg.sun_lunch_start_time as letterorg_sun_lunch_start_time, letterorg.sun_lunch_end_time as letterorg_sun_lunch_end_time, letterorg.mon_lunch_start_time as letterorg_mon_lunch_start_time, letterorg.mon_lunch_end_time as letterorg_mon_lunch_end_time, letterorg.tue_lunch_start_time as letterorg_tue_lunch_start_time, letterorg.tue_lunch_end_time as letterorg_tue_lunch_end_time, letterorg.wed_lunch_start_time as letterorg_wed_lunch_start_time, letterorg.wed_lunch_end_time as letterorg_wed_lunch_end_time, 
                    letterorg.thu_lunch_start_time as letterorg_thu_lunch_start_time, letterorg.thu_lunch_end_time as letterorg_thu_lunch_end_time, letterorg.fri_lunch_start_time as letterorg_fri_lunch_start_time, letterorg.fri_lunch_end_time as letterorg_fri_lunch_end_time, letterorg.sat_lunch_start_time as letterorg_sat_lunch_start_time, letterorg.sat_lunch_end_time as letterorg_sat_lunch_end_time, 
                    letterorg.last_batch_run as letterorg_last_batch_run, letterorg.is_deleted as letterorg_is_deleted,

                    site.site_id as site_site_id,site.entity_id as site_entity_id,site.name as site_name,site.site_type_id as site_site_type_id,site.abn as site_abn,site.acn as site_acn,site.tfn as site_tfn,
                    site.asic as site_asic,site.is_provider as site_is_provider,site.bank_bpay as site_bank_bpay,site.bank_bsb as site_bank_bsb,site.bank_account as site_bank_account,
                    site.bank_direct_debit_userid as site_bank_direct_debit_userid,site.bank_username as site_bank_username,site.oustanding_balance_warning as site_oustanding_balance_warning,
                    site.print_epc as site_print_epc,site.excl_sun as site_excl_sun,site.excl_mon as site_excl_mon,site.excl_tue as site_excl_tue,site.excl_wed as site_excl_wed,site.excl_thu as site_excl_thu,
                    site.excl_fri as site_excl_fri,site.excl_sat as site_excl_sat,site.day_start_time as site_day_start_time,site.lunch_start_time as site_lunch_start_time,
                    site.lunch_end_time as site_lunch_end_time,site.day_end_time as site_day_end_time,site.fiscal_yr_end as site_fiscal_yr_end,site.num_booking_months_to_get as site_num_booking_months_to_get,

                    sitetype.descr as site_type_descr 

            FROM
                    Letter letter
                    INNER JOIN LetterType lettertype ON letter.letter_type_id = lettertype.letter_type_id 
                    INNER JOIN Site       site       ON site.site_id          = letter.site_id
                    INNER JOIN SiteType   sitetype   ON site.site_type_id     = sitetype.site_type_id
                    LEFT OUTER JOIN Organisation AS letterorg ON letterorg.organisation_id = letter.organisation_id 
            WHERE
                    1=1
                    " + (incDeleted ? "" : " AND letter.is_deleted = 0 ");
    }


    public static Letter LoadAll(DataRow row)
    {
        Letter letter = Load(row, "letter_");
        letter.LetterType = IDandDescrDB.Load(row, "lettertype_letter_type_id", "lettertype_descr");
        letter.Site = SiteDB.Load(row, "site_");
        if (row["letterorg_organisation_id"] != DBNull.Value)
            letter.Organisation = OrganisationDB.Load(row, "letterorg_");
        return letter;
    }

    public static Letter Load(DataRow row, string prefix = "")
    {
        return new Letter(
            Convert.ToInt32(row[prefix + "letter_id"]),
            row[prefix + "organisation_id"] == DBNull.Value ? 0 : Convert.ToInt32(row[prefix + "organisation_id"]),
            Convert.ToInt32(row[prefix + "letter_type_id"]),
            Convert.ToInt32(row[prefix + "site_id"]),
            Convert.ToString(row[prefix + "code"]),
            Convert.ToString(row[prefix + "reject_message"]),
            Convert.ToString(row[prefix + "docname"]),
            Convert.ToBoolean(row[prefix + "is_send_to_medico"]),
            Convert.ToBoolean(row[prefix + "is_allowed_reclaim"]),
            Convert.ToBoolean(row[prefix + "is_manual_override"]),
            Convert.ToInt32(row[prefix + "num_copies_to_print"]),
            Convert.ToBoolean(row[prefix + "is_deleted"])
        );
    }

}