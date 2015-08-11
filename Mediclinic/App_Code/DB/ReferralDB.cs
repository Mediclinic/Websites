using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class ReferralDB
{

    public static void Delete(int referral_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Referral WHERE referral_id = " + referral_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int health_card_id, int medical_service_type_id, int register_referrer_id, DateTime date_referral_signed, DateTime date_referral_received_in_office, int added_or_last_modified_by, DateTime added_or_last_modified_date, int deleted_by, DateTime date_deleted)
    {
        string sql = "INSERT INTO Referral (health_card_id,medical_service_type_id,register_referrer_id,date_referral_signed,date_referral_received_in_office,added_or_last_modified_by,added_or_last_modified_date,deleted_by,date_deleted) VALUES (" + "" + health_card_id + "," + "" + medical_service_type_id + "," + (register_referrer_id == -1 ? "NULL" : register_referrer_id.ToString()) + "," + (date_referral_signed == DateTime.MinValue ? "null" : "'" + date_referral_signed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (date_referral_received_in_office == DateTime.MinValue ? "null" : "'" + date_referral_received_in_office.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + added_or_last_modified_by + "," + "'" + added_or_last_modified_date.ToString("yyyy-MM-dd HH:mm:ss") + "'," + (deleted_by == -1 ? "NULL" : deleted_by.ToString()) + "," + (date_deleted == DateTime.MinValue ? "null" : "'" + date_deleted.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int referral_id, int health_card_id, int medical_service_type_id, int register_referrer_id, DateTime date_referral_signed, DateTime date_referral_received_in_office, int added_or_last_modified_by, DateTime added_or_last_modified_date, int deleted_by, DateTime date_deleted)
    {
        string sql = "UPDATE Referral SET health_card_id = " + health_card_id + ",medical_service_type_id = " + medical_service_type_id + ",register_referrer_id = " + (register_referrer_id == -1 ? "NULL" : register_referrer_id.ToString()) + ",date_referral_signed = " + (date_referral_signed == DateTime.MinValue ? "null" : "'" + date_referral_signed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",date_referral_received_in_office = " + (date_referral_received_in_office == DateTime.MinValue ? "null" : "'" + date_referral_received_in_office.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",added_or_last_modified_by = " + added_or_last_modified_by + ",added_or_last_modified_date = '" + added_or_last_modified_date.ToString("yyyy-MM-dd HH:mm:ss") + "',deleted_by = " + (deleted_by == -1 ? "NULL" : deleted_by.ToString()) + ",date_deleted = " + (date_deleted == DateTime.MinValue ? "null" : "'" + date_deleted.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE referral_id = " + referral_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateRegRef(int referral_id, int register_referrer_id)
    {
        string sql = "UPDATE Referral SET register_referrer_id = " + register_referrer_id + " WHERE referral_id = " + referral_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateAsDeleted(int referral_id, int staff_id)
    {
        string sql = "UPDATE Referral SET deleted_by = " + staff_id + ",date_deleted = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE referral_id = " + referral_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static DataTable GetDataTable(bool inc_deleted = false, int patient_id = -1, int mst = -1)
    {
        string sql = JoinedSql;

        string where = string.Empty;
        if (patient_id != -1)
            where += (where.Length == 0 ? " WHERE " : " AND ") + " hc.patient_id = " + patient_id;
        if (mst != -1)
            where += (where.Length == 0 ? " WHERE " : " AND ") + " referral.medical_service_type_id = " + mst;
        if (!inc_deleted)
            where += (where.Length == 0 ? " WHERE " : " AND ") + " referral.deleted_by IS NULL AND referral.date_deleted IS NULL";

        sql += where;

        return DBBase.ExecuteQuery( sql ).Tables[0];
    }

    public static Referral GetByID(int referral_id)
    {
        string sql = JoinedSql + "WHERE referral_id = " + referral_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery( sql ).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static Referral[] GetAll(bool inc_deleted = false, int patient_id = -1)
    {
        DataTable tbl = GetDataTable(inc_deleted, patient_id);
        Referral[] list = new Referral[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);

        return list;
    }


    private static string JoinedSql = @"
                SELECT

                        referral.referral_id,referral.health_card_id,referral.medical_service_type_id,referral.register_referrer_id,referral.date_referral_signed,referral.date_referral_received_in_office,referral.added_or_last_modified_by,referral.added_or_last_modified_date,referral.deleted_by,referral.date_deleted,
                        mct.medical_service_type_id as mct_medical_service_type_id, mct.descr as mct_descr,


                        regref.register_referrer_id as regref_register_referrer_id,regref.organisation_id as regref_organisation_id, regref.referrer_id as regref_referrer_id, regref.provider_number as regref_provider_number, 
                        regref.report_every_visit_to_referrer as regref_report_every_visit_to_referrer,
                        regref.batch_send_all_patients_treatment_notes as regref_batch_send_all_patients_treatment_notes, regref.date_last_batch_send_all_patients_treatment_notes as regref_date_last_batch_send_all_patients_treatment_notes,
                        regref.register_referrer_date_added as regref_register_referrer_date_added,  
                        ref.referrer_id as referrer_referrer_id,ref.person_id as referrer_person_id, ref.referrer_date_added as referrer_referrer_date_added, 
                        org.entity_id as organisation_entity_id, org.parent_organisation_id as organisation_parent_organisation_id, org.use_parent_offernig_prices as organisation_use_parent_offernig_prices, org.organisation_type_id as organisation_organisation_type_id, org.organisation_customer_type_id as organisation_organisation_customer_type_id,org.name as organisation_name, org.acn as organisation_acn, org.abn as organisation_abn, org.organisation_date_added as organisation_organisation_date_added, 
                        org.organisation_id as organisation_organisation_id, org.organisation_date_modified as organisation_organisation_date_modified, org.is_debtor as organisation_is_debtor, org.is_creditor as organisation_is_creditor, org.bpay_account as organisation_bpay_account, org.weeks_per_service_cycle as organisation_weeks_per_service_cycle, org.start_date as organisation_start_date, 
                        org.end_date as organisation_end_date, org.comment as organisation_comment, org.free_services as organisation_free_services, org.excl_sun as organisation_excl_sun, org.excl_mon as organisation_excl_mon, org.excl_tue as organisation_excl_tue, org.excl_wed as organisation_excl_wed, org.excl_thu as organisation_excl_thu, org.excl_fri as organisation_excl_fri, org.excl_sat as organisation_excl_sat, 
                        org.sun_start_time as organisation_sun_start_time, org.sun_end_time as organisation_sun_end_time, org.mon_start_time as organisation_mon_start_time, org.mon_end_time as organisation_mon_end_time, org.tue_start_time as organisation_tue_start_time, org.tue_end_time as organisation_tue_end_time, org.wed_start_time as organisation_wed_start_time, org.wed_end_time as organisation_wed_end_time, 
                        org.thu_start_time as organisation_thu_start_time, org.thu_end_time as organisation_thu_end_time, org.fri_start_time as organisation_fri_start_time, org.fri_end_time as organisation_fri_end_time, org.sat_start_time as organisation_sat_start_time, org.sat_end_time as organisation_sat_end_time, 
                        org.sun_lunch_start_time as organisation_sun_lunch_start_time, org.sun_lunch_end_time as organisation_sun_lunch_end_time, org.mon_lunch_start_time as organisation_mon_lunch_start_time, org.mon_lunch_end_time as organisation_mon_lunch_end_time, org.tue_lunch_start_time as organisation_tue_lunch_start_time, org.tue_lunch_end_time as organisation_tue_lunch_end_time, org.wed_lunch_start_time as organisation_wed_lunch_start_time, org.wed_lunch_end_time as organisation_wed_lunch_end_time, 
                        org.thu_lunch_start_time as organisation_thu_lunch_start_time, org.thu_lunch_end_time as organisation_thu_lunch_end_time, org.fri_lunch_start_time as organisation_fri_lunch_start_time, org.fri_lunch_end_time as organisation_fri_lunch_end_time, org.sat_lunch_start_time as organisation_sat_lunch_start_time, org.sat_lunch_end_time as organisation_sat_lunch_end_time, 
                        org.last_batch_run as organisation_last_batch_run, org.is_deleted as organisation_is_deleted,

                        " + PersonDB.GetFields("referrer_person_", "referrer_person") + @",
                        referrer_person_title.title_id as referrer_person_title_title_id, referrer_person_title.descr as referrer_person_title_descr,


                        hc.health_card_id, hc.patient_id, hc.organisation_id, hcorg.name AS hc_organisation_name, hc.card_name, hc.card_nbr, hc.card_family_member_nbr, hc.expiry_date,
                        hc.date_referral_signed, hc.date_referral_received_in_office, is_active, hc.added_or_last_modified_by, hc.added_or_last_modified_date, hc.area_treated,


                        added_or_last_modified_by.staff_id as added_or_last_modified_by_staff_id, added_or_last_modified_by.person_id as added_or_last_modified_by_person_id, added_or_last_modified_by.login as added_or_last_modified_by_login, added_or_last_modified_by.pwd as added_or_last_modified_by_pwd, 
                        added_or_last_modified_by.staff_position_id as added_or_last_modified_by_staff_position_id, added_or_last_modified_by.field_id as added_or_last_modified_by_field_id, added_or_last_modified_by.costcentre_id as added_or_last_modified_by_costcentre_id, 
                        added_or_last_modified_by.is_contractor as added_or_last_modified_by_is_contractor, added_or_last_modified_by.tfn as added_or_last_modified_by_tfn, added_or_last_modified_by.provider_number as added_or_last_modified_by_provider_number, 
                        added_or_last_modified_by.is_fired as added_or_last_modified_by_is_fired, added_or_last_modified_by.is_commission as added_or_last_modified_by_is_commission, added_or_last_modified_by.commission_percent as added_or_last_modified_by_commission_percent, 
                        added_or_last_modified_by.is_stakeholder as added_or_last_modified_by_is_stakeholder, added_or_last_modified_by.is_master_admin as added_or_last_modified_by_is_master_admin, added_or_last_modified_by.is_admin as added_or_last_modified_by_is_admin, added_or_last_modified_by.is_principal as added_or_last_modified_by_is_principal, added_or_last_modified_by.is_provider as added_or_last_modified_by_is_provider, added_or_last_modified_by.is_external as added_or_last_modified_by_is_external,
                        added_or_last_modified_by.staff_date_added as added_or_last_modified_by_staff_date_added, added_or_last_modified_by.start_date as added_or_last_modified_by_start_date, added_or_last_modified_by.end_date as added_or_last_modified_by_end_date, added_or_last_modified_by.comment as added_or_last_modified_by_comment, 
                        added_or_last_modified_by.num_days_to_display_on_booking_screen as added_or_last_modified_by_num_days_to_display_on_booking_screen, 
                        added_or_last_modified_by.show_header_on_booking_screen as added_or_last_modified_by_show_header_on_booking_screen,
                        added_or_last_modified_by.bk_screen_field_id as added_or_last_modified_by_bk_screen_field_id, 
                        added_or_last_modified_by.bk_screen_show_key as added_or_last_modified_by_bk_screen_show_key, 
                        added_or_last_modified_by.enable_daily_reminder_sms   as added_or_last_modified_by_enable_daily_reminder_sms, 
                        added_or_last_modified_by.enable_daily_reminder_email as added_or_last_modified_by_enable_daily_reminder_email, 

                        " + PersonDB.GetFields("person_added_or_last_modified_by_", "person_added_or_last_modified_by") + @",
                        title_added_or_last_modified_by.title_id as title_added_or_last_modified_by_title_id, title_added_or_last_modified_by.descr as title_added_or_last_modified_by_descr,


                        deleted_by.staff_id as deleted_by_staff_id, deleted_by.person_id as deleted_by_person_id, deleted_by.login as deleted_by_login, deleted_by.pwd as deleted_by_pwd, 
                        deleted_by.staff_position_id as deleted_by_staff_position_id, deleted_by.field_id as deleted_by_field_id, deleted_by.costcentre_id as deleted_by_costcentre_id, 
                        deleted_by.is_contractor as deleted_by_is_contractor, deleted_by.tfn as deleted_by_tfn, deleted_by.provider_number as deleted_by_provider_number, 
                        deleted_by.is_fired as deleted_by_is_fired, deleted_by.is_commission as deleted_by_is_commission, deleted_by.commission_percent as deleted_by_commission_percent, 
                        deleted_by.is_stakeholder as deleted_by_is_stakeholder, deleted_by.is_master_admin as deleted_by_is_master_admin, deleted_by.is_admin as deleted_by_is_admin, deleted_by.is_principal as deleted_by_is_principal, deleted_by.is_provider as deleted_by_is_provider, deleted_by.is_external as deleted_by_is_external,
                        deleted_by.staff_date_added as deleted_by_staff_date_added, deleted_by.start_date as deleted_by_start_date, deleted_by.end_date as deleted_by_end_date, deleted_by.comment as deleted_by_comment, 
                        deleted_by.num_days_to_display_on_booking_screen as deleted_by_num_days_to_display_on_booking_screen, 
                        deleted_by.show_header_on_booking_screen as deleted_by_show_header_on_booking_screen,
                        deleted_by.bk_screen_field_id as deleted_by_bk_screen_field_id, 
                        deleted_by.bk_screen_show_key as deleted_by_bk_screen_show_key, 
                        deleted_by.enable_daily_reminder_sms   as deleted_by_enable_daily_reminder_sms, 
                        deleted_by.enable_daily_reminder_email as deleted_by_enable_daily_reminder_email, 

                        " + PersonDB.GetFields("person_deleted_by_", "person_deleted_by") + @",
                        title_deleted_by.title_id as title_deleted_by_title_id, title_deleted_by.descr as title_deleted_by_descr



                FROM

                        Referral referral
                        INNER JOIN MedicalServiceType AS mct ON mct.medical_service_type_id = referral.medical_service_type_id


                        LEFT OUTER JOIN RegisterReferrer AS regref                ON regref.register_referrer_id    = referral.register_referrer_id 
                        LEFT OUTER JOIN Referrer         AS ref                   ON ref.referrer_id                = regref.referrer_id 
                        LEFT OUTER JOIN Organisation     AS org                   ON org.organisation_id            = regref.organisation_id
                        LEFT OUTER JOIN Person                AS referrer_person       ON referrer_person.person_id      = ref.person_id
                        LEFT OUTER JOIN Title                 AS referrer_person_title ON referrer_person_title.title_id = referrer_person.title_id


                        LEFT OUTER JOIN Staff  added_or_last_modified_by        ON added_or_last_modified_by.staff_id         = referral.added_or_last_modified_by
                        LEFT OUTER JOIN Person person_added_or_last_modified_by ON person_added_or_last_modified_by.person_id = added_or_last_modified_by.person_id
                        LEFT OUTER JOIN Title  title_added_or_last_modified_by  ON title_added_or_last_modified_by.title_id   = person_added_or_last_modified_by.title_id

                        LEFT OUTER JOIN Staff  deleted_by                       ON deleted_by.staff_id                        = referral.deleted_by
                        LEFT OUTER JOIN Person person_deleted_by                ON person_deleted_by.person_id                = deleted_by.person_id
                        LEFT OUTER JOIN Title  title_deleted_by                 ON title_deleted_by.title_id                  = person_deleted_by.title_id

                        INNER JOIN HealthCard AS hc ON hc.health_card_id = referral.health_card_id
                        INNER JOIN Organisation AS hcorg ON hcorg.organisation_id = hc.organisation_id
                    ";


    public static Referral LoadAll(DataRow row)
    {
        Referral referral = Load(row);
        referral.HealthCard = HealthCardDB.Load(row);
        referral.MedicalServiceType = IDandDescrDB.Load(row, "mct_medical_service_type_id", "mct_descr");


        referral.RegisterReferrer = RegisterReferrerDB.Load(row, "regref_");
        referral.RegisterReferrer.Referrer = ReferrerDB.Load(row, "referrer_");
        referral.RegisterReferrer.Referrer.Person = PersonDB.Load(row, "referrer_person_");
        referral.RegisterReferrer.Referrer.Person.Title = IDandDescrDB.Load(row, "referrer_person_title_title_id", "referrer_person_title_descr");
        if (row["organisation_organisation_id"] != DBNull.Value)
            referral.RegisterReferrer.Organisation = OrganisationDB.Load(row, "organisation_");


        if (row["added_or_last_modified_by_staff_id"] != DBNull.Value)
            referral.AddedOrLastModifiedBy = StaffDB.Load(row, "added_or_last_modified_by_");
        if (row["person_added_or_last_modified_by_person_id"] != DBNull.Value)
        {
            referral.AddedOrLastModifiedBy.Person = PersonDB.Load(row, "person_added_or_last_modified_by_");
            referral.AddedOrLastModifiedBy.Person.Title = IDandDescrDB.Load(row, "title_added_or_last_modified_by_title_id", "title_added_or_last_modified_by_descr");
        }

        if (row["deleted_by_staff_id"] != DBNull.Value)
            referral.AddedOrLastModifiedBy = StaffDB.Load(row, "deleted_by_");
        if (row["person_deleted_by_person_id"] != DBNull.Value)
        {
            referral.AddedOrLastModifiedBy.Person = PersonDB.Load(row, "person_deleted_by_");
            referral.AddedOrLastModifiedBy.Person.Title = IDandDescrDB.Load(row, "title_deleted_by_title_id", "title_deleted_by_descr");
        }


        return referral;
    }

    public static Referral Load(DataRow row, string prefix="")
    {
        return new Referral(
            Convert.ToInt32(row[prefix  + "referral_id"]),
            Convert.ToInt32(row[prefix  + "health_card_id"]),
            Convert.ToInt32(row[prefix  + "medical_service_type_id"]),
            row[prefix + "register_referrer_id"]             == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "register_referrer_id"]),
            row[prefix + "date_referral_signed"]             == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_referral_signed"]),
            row[prefix + "date_referral_received_in_office"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_referral_received_in_office"]),
            row[prefix + "added_or_last_modified_by"]        == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "added_or_last_modified_by"]),
            row[prefix + "added_or_last_modified_date"]      == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "added_or_last_modified_date"]),
            row[prefix + "deleted_by"]                       == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "deleted_by"]),
            row[prefix + "date_deleted"]                     == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_deleted"])
        );
    }

}