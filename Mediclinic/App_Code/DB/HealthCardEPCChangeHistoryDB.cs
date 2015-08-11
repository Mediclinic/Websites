using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class HealthCardEPCChangeHistoryDB
{

    /*
    public static void Delete(int health_card_epc_change_history_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM HealthCardEPCChangeHistory WHERE health_card_epc_change_history_id = " + health_card_epc_change_history_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    */
    public static int Insert(int health_card_id, int staff_id, bool is_new_epc_card_set, DateTime pre_date_referral_signed, DateTime pre_date_referral_received_in_office, DateTime post_date_referral_signed, DateTime post_date_referral_received_in_office)
    {
        string sql = "INSERT INTO HealthCardEPCChangeHistory (health_card_id,staff_id,is_new_epc_card_set,pre_date_referral_signed,pre_date_referral_received_in_office,post_date_referral_signed,post_date_referral_received_in_office) VALUES (" + "" + health_card_id + "," + "" + staff_id + "," + (is_new_epc_card_set ? "1" : "0") + "," + (pre_date_referral_signed == DateTime.MinValue ? "null" : "'" + pre_date_referral_signed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (pre_date_referral_received_in_office == DateTime.MinValue ? "null" : "'" + pre_date_referral_received_in_office.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (post_date_referral_signed == DateTime.MinValue ? "null" : "'" + post_date_referral_signed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (post_date_referral_received_in_office == DateTime.MinValue ? "null" : "'" + post_date_referral_received_in_office.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    /*
    public static void Update(int health_card_epc_change_history_id, int health_card_id, int staff_id, int pre_medicare_num_services_remaining, int pre_medicare_num_services_used, int pre_physio_num_services_remaining, DateTime pre_date_referral_signed, DateTime pre_date_referral_received_in_office, int post_medicare_num_services_remaining, int post_medicare_num_services_used, int post_physio_num_services_remaining, DateTime post_date_referral_signed, DateTime post_date_referral_received_in_office)
    {
        string sql = "UPDATE HealthCardEPCChangeHistory SET health_card_id = " + health_card_id + ",staff_id = " + staff_id + ",pre_medicare_num_services_remaining = " + pre_medicare_num_services_remaining + ",pre_medicare_num_services_used = " + pre_medicare_num_services_used + ",pre_physio_num_services_remaining = " + pre_physio_num_services_remaining + ",pre_date_referral_signed = " + (pre_date_referral_signed == DateTime.MinValue ? "null" : "'" + pre_date_referral_signed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",pre_date_referral_received_in_office = " + (pre_date_referral_received_in_office == DateTime.MinValue ? "null" : "'" + pre_date_referral_received_in_office.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",post_medicare_num_services_remaining = " + post_medicare_num_services_remaining + ",post_medicare_num_services_used = " + post_medicare_num_services_used + ",post_physio_num_services_remaining = " + post_physio_num_services_remaining + ",post_date_referral_signed = " + (post_date_referral_signed == DateTime.MinValue ? "null" : "'" + post_date_referral_signed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",post_date_referral_received_in_office = " + (post_date_referral_received_in_office == DateTime.MinValue ? "null" : "'" + post_date_referral_received_in_office.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE health_card_epc_change_history_id = " + health_card_epc_change_history_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
     */


    public static int GetCountByHealthCard(int health_card_id)
    {
        string sql = "SELECT COUNT(*) FROM HealthCardEPCChangeHistory WHERE health_card_id = " + health_card_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static HealthCardEPCChangeHistory[] GetByHealthCardID(int health_card_id)
    {
        string sql = @"
            SELECT  
                    health_card_epc_change_history_id,health_card_id,HealthCardEPCChangeHistory.staff_id,date,is_new_epc_card_set,
                    pre_date_referral_signed,pre_date_referral_received_in_office,
                    post_date_referral_signed,post_date_referral_received_in_office,

                    staff.staff_id as staff_staff_id, staff.person_id as staff_person_id, staff.login as staff_login, staff.pwd as staff_pwd, 
                    staff.staff_position_id as staff_staff_position_id, staff.field_id as staff_field_id, staff.costcentre_id as staff_costcentre_id, 
                    staff.is_contractor as staff_is_contractor, staff.tfn as staff_tfn, staff.provider_number as staff_provider_number, 
                    staff.is_fired as staff_is_fired, staff.is_commission as staff_is_commission, staff.commission_percent as staff_commission_percent, 
                    staff.is_stakeholder as staff_is_stakeholder,staff.is_master_admin as staff_is_master_admin,staff.is_admin as staff_is_admin,staff.is_principal as staff_is_principal,staff.is_provider as staff_is_provider, staff.is_external as staff_is_external,
                    staff.staff_date_added as staff_staff_date_added, staff.start_date as staff_start_date, staff.end_date as staff_end_date, 
                    staff.comment as staff_comment, 
                    staff.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen,
                    staff.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                    staff.bk_screen_field_id as staff_bk_screen_field_id, staff.bk_screen_show_key as staff_bk_screen_show_key,
                    staff.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                    staff.enable_daily_reminder_email as staff_enable_daily_reminder_email,

                    " + PersonDB.GetFields("person_", "person") + @",
                    title.title_id as title_title_id, title.descr as title_descr

            FROM    
                    HealthCardEPCChangeHistory 
                    LEFT OUTER JOIN Staff  staff   ON staff.staff_id   = HealthCardEPCChangeHistory.staff_id
                    LEFT OUTER JOIN Person person  ON person.person_id = staff.person_id
                    LEFT OUTER JOIN Title  title   ON title.title_id   = person.title_id

            WHERE   
                    health_card_id = " + health_card_id;

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        HealthCardEPCChangeHistory[] histories = new HealthCardEPCChangeHistory[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            histories[i] = Load(tbl.Rows[i]);
            histories[i].Staff = StaffDB.Load(tbl.Rows[i], "staff_");
            histories[i].Staff.Person = PersonDB.Load(tbl.Rows[i], "person_");
            histories[i].Staff.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_title_id", "title_descr");
        }
        return histories;
    }

    /*
    public static DataTable GetDataTable()
    {
        string sql = "SELECT health_card_epc_change_history_id,health_card_id,staff_id,date,pre_medicare_num_services_remaining,pre_medicare_num_services_used,pre_physio_num_services_remaining,pre_date_referral_signed,pre_date_referral_received_in_office,post_medicare_num_services_remaining,post_medicare_num_services_used,post_physio_num_services_remaining,post_date_referral_signed,post_date_referral_received_in_office FROM HealthCardEPCChangeHistory";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static HealthCardEPCChangeHistory GetByID(int health_card_epc_change_history_id)
    {
        string sql = "SELECT health_card_epc_change_history_id,health_card_id,staff_id,date,pre_medicare_num_services_remaining,pre_medicare_num_services_used,pre_physio_num_services_remaining,pre_date_referral_signed,pre_date_referral_received_in_office,post_medicare_num_services_remaining,post_medicare_num_services_used,post_physio_num_services_remaining,post_date_referral_signed,post_date_referral_received_in_office FROM HealthCardEPCChangeHistory WHERE health_card_epc_change_history_id = " + health_card_epc_change_history_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }


    */

    public static HealthCardEPCChangeHistory Load(DataRow row)
    {
        return new HealthCardEPCChangeHistory(
            Convert.ToInt32(row["health_card_epc_change_history_id"]),
            Convert.ToInt32(row["health_card_id"]),
            Convert.ToInt32(row["staff_id"]),
            Convert.ToDateTime(row["date"]),
            Convert.ToBoolean(row["is_new_epc_card_set"]),
            row["pre_date_referral_signed"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["pre_date_referral_signed"]),
            row["pre_date_referral_received_in_office"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["pre_date_referral_received_in_office"]),
            row["post_date_referral_signed"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["post_date_referral_signed"]),
            row["post_date_referral_received_in_office"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["post_date_referral_received_in_office"])
        );
    }
}