using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class StockUpdateHistoryDB
{

    public static void Delete(int stock_update_history_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM StockUpdateHistory WHERE stock_update_history_id = " + stock_update_history_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int organisation_id, int offering_id, int qty_added, bool is_created, bool is_deleted, int added_by)
    {
        string sql = "INSERT INTO StockUpdateHistory (organisation_id,offering_id,qty_added,is_created,is_deleted,added_by,date_added) VALUES (" + "" + organisation_id + "," + "" + offering_id + "," + "" + qty_added + "," + "" + (is_created ? "1" : "0") + "," + "" + (is_deleted ? "1" : "0") + "," + "" + added_by + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static DataTable GetDataTable()
    {
        string sql = JoinedSql + " ORDER BY sa.date_added DESC";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable_ByOrg(int organisation_id)
    {
        string sql = JoinedSql + " AND sa.organisation_id = " + organisation_id.ToString() + " ORDER BY sa.date_added DESC";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static StockUpdateHistory GetByID(int stock_update_history_id)
    {
        string sql = JoinedSql + " AND stock_update_history_id = " + stock_update_history_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }


    private static string JoinedSql = @"
                        SELECT

                            sa.stock_update_history_id as sa_stock_update_history_id, sa.organisation_id as sa_organisation_id, sa.offering_id as sa_offering_id, sa.qty_added as sa_qty_added, sa.is_created as sa_is_created, sa.is_deleted as sa_is_deleted, sa.added_by as sa_added_by, sa.date_added as sa_date_added,

                            org.organisation_id as organisation_organisation_id, org.entity_id as organisation_entity_id, org.parent_organisation_id as organisation_parent_organisation_id, org.use_parent_offernig_prices as organisation_use_parent_offernig_prices, org.organisation_type_id as organisation_organisation_type_id, org.organisation_customer_type_id as organisation_organisation_customer_type_id,org.name as organisation_name, org.acn as organisation_acn, org.abn as organisation_abn, org.organisation_date_added as organisation_organisation_date_added, 
                            org.organisation_date_modified as organisation_organisation_date_modified, org.is_debtor as organisation_is_debtor, org.is_creditor as organisation_is_creditor, org.bpay_account as organisation_bpay_account, org.weeks_per_service_cycle as organisation_weeks_per_service_cycle, org.start_date as organisation_start_date, 
                            org.end_date as organisation_end_date, org.comment as organisation_comment, org.free_services as organisation_free_services, org.excl_sun as organisation_excl_sun, org.excl_mon as organisation_excl_mon, org.excl_tue as organisation_excl_tue, org.excl_wed as organisation_excl_wed, org.excl_thu as organisation_excl_thu, org.excl_fri as organisation_excl_fri, org.excl_sat as organisation_excl_sat, 
                            org.sun_start_time as organisation_sun_start_time, org.sun_end_time as organisation_sun_end_time, org.mon_start_time as organisation_mon_start_time, org.mon_end_time as organisation_mon_end_time, org.tue_start_time as organisation_tue_start_time, org.tue_end_time as organisation_tue_end_time, org.wed_start_time as organisation_wed_start_time, org.wed_end_time as organisation_wed_end_time, 
                            org.thu_start_time as organisation_thu_start_time, org.thu_end_time as organisation_thu_end_time, org.fri_start_time as organisation_fri_start_time, org.fri_end_time as organisation_fri_end_time, org.sat_start_time as organisation_sat_start_time, org.sat_end_time as organisation_sat_end_time, 
                            org.sun_lunch_start_time as organisation_sun_lunch_start_time, org.sun_lunch_end_time as organisation_sun_lunch_end_time, org.mon_lunch_start_time as organisation_mon_lunch_start_time, org.mon_lunch_end_time as organisation_mon_lunch_end_time, org.tue_lunch_start_time as organisation_tue_lunch_start_time, org.tue_lunch_end_time as organisation_tue_lunch_end_time, org.wed_lunch_start_time as organisation_wed_lunch_start_time, org.wed_lunch_end_time as organisation_wed_lunch_end_time, 
                            org.thu_lunch_start_time as organisation_thu_lunch_start_time, org.thu_lunch_end_time as organisation_thu_lunch_end_time, org.fri_lunch_start_time as organisation_fri_lunch_start_time, org.fri_lunch_end_time as organisation_fri_lunch_end_time, org.sat_lunch_start_time as organisation_sat_lunch_start_time, org.sat_lunch_end_time as organisation_sat_lunch_end_time, 
                            org.last_batch_run as organisation_last_batch_run,

                            o.offering_id as o_offering_id, o.offering_type_id as o_offering_type_id, o.field_id as o_field_id, 
                            o.aged_care_patient_type_id as o_aged_care_patient_type_id, 
                            o.num_clinic_visits_allowed_per_year as o_num_clinic_visits_allowed_per_year, 
                            o.offering_invoice_type_id as o_offering_invoice_type_id, o.name as o_name, o.short_name as o_short_name, o.descr as o_descr, 
                            o.is_gst_exempt as o_is_gst_exempt, 
                            o.default_price as o_default_price, 
                            o.service_time_minutes as o_service_time_minutes, 
                            o.max_nbr_claimable as o_max_nbr_claimable, o.max_nbr_claimable_months as o_max_nbr_claimable_months,
                            o.medicare_company_code as o_medicare_company_code, o.dva_company_code as o_dva_company_code, o.tac_company_code as o_tac_company_code, 
                            o.medicare_charge as o_medicare_charge, o.dva_charge as o_dva_charge, o.tac_charge as o_tac_charge, o.popup_message as o_popup_message, o.reminder_letter_months_later_to_send as o_reminder_letter_months_later_to_send, o.reminder_letter_id as o_reminder_letter_id, o.use_custom_color as o_use_custom_color, o.custom_color as o_custom_color,

                            type.offering_type_id AS type_offering_type_id, type.descr AS type_descr,
                            fld.field_id AS fld_field_id, fld.descr AS fld_descr, 
                            acpatientcat.aged_care_patient_type_id AS acpatientcat_aged_care_patient_type_id, acpatientcat.descr AS acpatientcat_descr,
                            invtype.offering_invoice_type_id AS invtype_offering_invoice_type_id, invtype.descr AS invtype_descr,


                            added_by.staff_id as added_by_staff_id, added_by.person_id as added_by_person_id, added_by.login as added_by_login, added_by.pwd as added_by_pwd, 
                            added_by.staff_position_id as added_by_staff_position_id, added_by.field_id as added_by_field_id, added_by.costcentre_id as added_by_costcentre_id, 
                            added_by.is_contractor as added_by_is_contractor, added_by.tfn as added_by_tfn, added_by.provider_number as added_by_provider_number, 
                            added_by.is_fired as added_by_is_fired, added_by.is_commission as added_by_is_commission, added_by.commission_percent as added_by_commission_percent, 
                            added_by.is_stakeholder as added_by_is_stakeholder, added_by.is_master_admin as added_by_is_master_admin, added_by.is_admin as added_by_is_admin, added_by.is_principal as added_by_is_principal, added_by.is_provider as added_by_is_provider, added_by.is_external as added_by_is_external,
                            added_by.staff_date_added as added_by_staff_date_added, added_by.start_date as added_by_start_date, added_by.end_date as added_by_end_date, added_by.comment as added_by_comment, 
                            added_by.num_days_to_display_on_booking_screen as added_by_num_days_to_display_on_booking_screen,
                            added_by.show_header_on_booking_screen as added_by_show_header_on_booking_screen, 
                            added_by.bk_screen_field_id as added_by_bk_screen_field_id,
                            added_by.bk_screen_show_key as added_by_bk_screen_show_key,
                            added_by.enable_daily_reminder_sms as added_by_enable_daily_reminder_sms, 
                            added_by.enable_daily_reminder_email as added_by_enable_daily_reminder_email,

                            " + PersonDB.GetFields("person_added_by_", "person_added_by") + @",
                            title_added_by.title_id as title_added_by_title_id, title_added_by.descr as title_added_by_descr

                        FROM
                            StockUpdateHistory                            sa
                            INNER JOIN Organisation                       org             ON sa.organisation_id                          = org.organisation_id
                            INNER JOIN Offering                           o               ON sa.offering_id                              = o.offering_id
                            INNER JOIN OfferingInvoiceType                invtype         ON o.offering_invoice_type_id                  = invtype.offering_invoice_type_id 
                            INNER JOIN AgedCarePatientType acpatientcat    ON o.aged_care_patient_type_id = acpatientcat.aged_care_patient_type_id 
                            INNER JOIN Field                              fld             ON o.field_id                                  = fld.field_id 
                            INNER JOIN OfferingType                       type            ON o.offering_type_id                          = type.offering_type_id

                            INNER JOIN Staff                              added_by        ON added_by.staff_id                           = sa.added_by
                            INNER JOIN Person                             person_added_by ON person_added_by.person_id                   = added_by.person_id
                            INNER JOIN Title                              title_added_by  ON title_added_by.title_id                     = person_added_by.title_id

                        WHERE 
                            o.is_deleted = 0 ";


    public static StockUpdateHistory Load(DataRow row, string prefix = "")
    {
        return new StockUpdateHistory(
            Convert.ToInt32(row[prefix + "stock_update_history_id"]),
            Convert.ToInt32(row[prefix + "organisation_id"]),
            Convert.ToInt32(row[prefix + "offering_id"]),
            Convert.ToInt32(row[prefix + "qty_added"]),
            Convert.ToBoolean(row[prefix + "is_created"]),
            Convert.ToBoolean(row[prefix + "is_deleted"]),
            Convert.ToInt32(prefix + row["added_by"]),
            row[prefix + "date_added"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_added"])
        );
    }

}