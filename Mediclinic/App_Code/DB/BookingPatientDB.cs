using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class BookingPatientDB
{

    public static void Delete(int booking_patient_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM BookingPatient WHERE booking_patient_id = " + booking_patient_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int booking_id, int patient_id, int offering_id, string area_treated, int added_by)
    {
        int entityID = EntityDB.Insert();

        area_treated = area_treated.Replace("'","''");
        string sql = "INSERT INTO BookingPatient (booking_id,patient_id,entity_id,offering_id,area_treated,added_by,added_date,is_deleted,deleted_by,deleted_date,need_to_generate_first_letter,need_to_generate_last_letter,has_generated_system_letters) VALUES (" + "" + booking_id + "," + patient_id + "," + entityID + "," + (offering_id == -1 ? "NULL" : offering_id.ToString()) + "," + "'" + area_treated + "'" + "," + added_by + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "0," + "NULL" + "," + "NULL, 0, 0, 0" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int booking_patient_id, int booking_id, int patient_id, int offering_id, string area_treated, int added_by, DateTime added_date, bool is_deleted, int deleted_by, DateTime deleted_date)
    {
        area_treated = area_treated.Replace("'", "''");
        string sql = "UPDATE BookingPatient SET booking_id = " + booking_id + ",patient_id = " + patient_id + ",offering_id = " + (offering_id == -1 ? "NULL" : offering_id.ToString()) + ",area_treated = '" + area_treated + "',added_by = " + added_by + ",added_date = '" + added_date.ToString("yyyy-MM-dd HH:mm:ss") + "',is_deleted = " + (is_deleted ? "1," : "0,") + "deleted_by = " + (deleted_by == -1 ? "NULL" : deleted_by.ToString()) + ",deleted_date = " + (deleted_date == DateTime.MinValue ? "NULL" : "'" + deleted_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE booking_patient_id = " + booking_patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateOffering(int booking_patient_id, int offering_id)
    {
        string sql = "UPDATE BookingPatient SET offering_id = " + (offering_id == -1 ? "NULL" : offering_id.ToString()) + " WHERE booking_patient_id = " + booking_patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateAreaTreated(int booking_patient_id, string area_treated)
    {
        area_treated = area_treated.Replace("'", "''");
        string sql = "UPDATE BookingPatient SET area_treated = '" + area_treated + "' WHERE booking_patient_id = " + booking_patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

        

    public static void UpdateInactive(int booking_patient_id, int staff_id)
    {
        string sql = "UPDATE BookingPatient SET is_deleted = 1, deleted_by = " + staff_id + ", deleted_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE booking_patient_id = " + booking_patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int booking_patient_id, int staff_id)
    {
        string sql = "UPDATE BookingPatient SET is_deleted = 0, deleted_by = " + staff_id + ", deleted_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE booking_patient_id = " + booking_patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateSetGeneratedSystemLetters(int booking_patient_id, bool need_to_generate_first_letter, bool need_to_generate_last_letter, bool has_generated_system_letters)
    {
        string sql = "UPDATE BookingPatient SET need_to_generate_first_letter = " + (need_to_generate_first_letter ? "1," : "0,") + @"
                                                need_to_generate_last_letter  = " + (need_to_generate_last_letter  ? "1," : "0,") + @"
                                                has_generated_system_letters  = " + (has_generated_system_letters  ? "1"  : "0" ) + @"
            
             WHERE booking_patient_id = " + booking_patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetGeneratedSystemLetters(int booking_id, int patient_id, bool need_to_generate_first_letter, bool need_to_generate_last_letter, bool has_generated_system_letters)
    {
        string sql = "UPDATE BookingPatient SET need_to_generate_first_letter = " + (need_to_generate_first_letter ? "1," : "0,") + @"
                                                need_to_generate_last_letter  = " + (need_to_generate_last_letter  ? "1," : "0,") + @"
                                                has_generated_system_letters  = " + (has_generated_system_letters  ? "1"  : "0" ) + @"
            
             WHERE booking_id = " + booking_id + " AND patient_id = " + patient_id;
        DBBase.ExecuteNonResult(sql);
    }


    public static bool Exists(int booking_id, int patient_id)
    {
        string sql = "SELECT COUNT(*) FROM BookingPatient WHERE booking_id = " + booking_id + " AND patient_id = " + patient_id + " AND is_deleted = 0 ";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static DataTable GetDataTable()
    {
        string sql = JoinedSql() + " WHERE bp.is_deleted = 0 ORDER BY patient_person.surname, patient_person.firstname, patient_person.middlename";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static BookingPatient GetByID(int booking_patient_id)
    {
        //string sql = "SELECT booking_patient_id,booking_id,patient_id,added_by,added_date,is_deleted,deleted_by,deleted_date FROM BookingPatient WHERE booking_patient_id = " + booking_patient_id.ToString();
        string sql = JoinedSql() + " WHERE booking_patient_id = " + booking_patient_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static DataTable GetDataTable_ByBookingID(int booking_id)
    {
        string sql = JoinedSql(true) + " WHERE bp.booking_id = " + booking_id.ToString() + " AND bp.is_deleted = 0 ORDER BY patient_person.surname, patient_person.firstname, patient_person.middlename";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static BookingPatient[] GetByBookingID(int booking_id)
    {
        DataTable tbl = GetDataTable_ByBookingID(booking_id);
        BookingPatient[] bps = new BookingPatient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bps[i] = LoadAll(tbl.Rows[i], true);

        return bps;
    }

    public static DataTable GetDataTable_ByPatientID(int patient_id)
    {
        string sql = JoinedSql(true) + " WHERE bp.patient_id = " + patient_id.ToString() + " AND bp.is_deleted = 0 ORDER BY patient_person.surname, patient_person.firstname, patient_person.middlename";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static BookingPatient[] GetByPatientID(int patient_id)
    {
        DataTable tbl = GetDataTable_ByPatientID(patient_id);
        BookingPatient[] bps = new BookingPatient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bps[i] = LoadAll(tbl.Rows[i], true);

        return bps;
    }

    public static DataTable GetDataTable_ByBookingAndPatientID(int booking_id, int patient_id)
    {
        string sql = JoinedSql(true) + " WHERE bp.booking_id = " + booking_id.ToString() + " AND bp.patient_id = " + patient_id.ToString() + " AND bp.is_deleted = 0 ORDER BY patient_person.surname, patient_person.firstname, patient_person.middlename";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static BookingPatient GetByBookingAndPatientID(int booking_id, int patient_id)
    {
        DataTable tbl = GetDataTable_ByBookingAndPatientID(booking_id, patient_id);
        return tbl.Rows.Count > 0 ? LoadAll(tbl.Rows[0], true) : null;
    }

    public static DataTable GetBookingsPatientOfferingsWithEPCLetters(DateTime bookingStartDate, DateTime bookingEndDate, int register_referrer_id = -1, int patient_id = -1, bool inc_sent = false, bool inc_unsent = false, bool inc_batching = false, bool inc_non_batching = false)
    {

        string sentOrUnsent = string.Empty;
        if (!inc_sent && !inc_unsent)
            sentOrUnsent = " 1<>1 AND ";
        else if (!inc_sent && inc_unsent)
            sentOrUnsent = " bp.has_generated_system_letters = 0 AND ";
        else if (inc_sent && !inc_unsent)
            sentOrUnsent = " bp.has_generated_system_letters = 1 AND ";

        string referrerBatching = string.Empty;
        if (inc_unsent)
        {
            if (!inc_batching && !inc_non_batching)
                referrerBatching = " 1<>1 AND ";
            else if (!inc_batching && inc_non_batching)
                referrerBatching = " regref.batch_send_all_patients_treatment_notes = 0 AND ";
            else if (inc_batching && !inc_non_batching)
                referrerBatching = " regref.batch_send_all_patients_treatment_notes = 1 AND ";
        }


        string sql = @"
        SELECT DISTINCT

            -- offering info

            offering.offering_id as offering_offering_id, offering.offering_type_id as offering_offering_type_id, 
            --offering.field_id as offering_field_id,
            bk_provider.field_id as offering_field_id,
            offering.aged_care_patient_type_id as offering_aged_care_patient_type_id, 
            offering.num_clinic_visits_allowed_per_year as offering_num_clinic_visits_allowed_per_year, 
            offering.offering_invoice_type_id as offering_offering_invoice_type_id, 
            offering.name as offering_name, offering.short_name as offering_short_name, offering.descr as offering_descr, offering.is_gst_exempt as offering_is_gst_exempt, 
            offering.default_price as offering_default_price, offering.service_time_minutes as offering_service_time_minutes, 
            offering.max_nbr_claimable as offering_max_nbr_claimable, offering.max_nbr_claimable_months as offering_max_nbr_claimable_months,
            offering.medicare_company_code as offering_medicare_company_code, offering.dva_company_code as offering_dva_company_code, offering.tac_company_code as offering_tac_company_code, 
            offering.medicare_charge as offering_medicare_charge, offering.dva_charge as offering_dva_charge, offering.tac_charge as offering_tac_charge,
            offering.popup_message as offering_popup_message, offering.reminder_letter_months_later_to_send as offering_reminder_letter_months_later_to_send, offering.reminder_letter_id as offering_reminder_letter_id, offering.use_custom_color as offering_use_custom_color, offering.custom_color as offering_custom_color,


            -- bookingpatient info

            bp.booking_patient_id as bp_booking_patient_id, 
            bp.booking_id         as bp_booking_id,
            bp.patient_id         as bp_patient_id, 
            bp.entity_id          as bp_entity_id, 
            bp.added_by           as bp_added_by, 
            bp.added_date         as bp_added_date,
            bp.is_deleted         as bp_is_deleted,
            bp.deleted_by         as bp_deleted_by, 
            bp.deleted_date       as bp_deleted_date,
            bp.offering_id        as bp_offering_id,
            bp.area_treated       as bp_area_treated,
            bp.need_to_generate_first_letter as bp_need_to_generate_first_letter,
            bp.need_to_generate_last_letter  as bp_need_to_generate_last_letter,
            bp.has_generated_system_letters  as bp_has_generated_system_letters,

            (SELECT COUNT(*) FROM Note note WHERE note.entity_id = bp.entity_id) AS bp_note_count,


            -- booking info

            booking.booking_id as booking_booking_id,booking.entity_id as booking_entity_id,
            booking.date_start as booking_date_start,booking.date_end as booking_date_end,booking.organisation_id as booking_organisation_id,
            booking.provider as booking_provider,booking.patient_id as booking_patient_id,booking.offering_id as booking_offering_id,booking.booking_type_id as booking_booking_type_id,
            booking.booking_status_id as booking_booking_status_id,booking.booking_unavailability_reason_id as booking_booking_unavailability_reason_id,booking.added_by as booking_added_by,booking.date_created as booking_date_created,
            booking.booking_confirmed_by_type_id as booking_booking_confirmed_by_type_id,booking.confirmed_by as booking_confirmed_by,booking.date_confirmed as booking_date_confirmed,
            booking.deleted_by as booking_deleted_by, booking.date_deleted as booking_date_deleted,
            booking.cancelled_by as booking_cancelled_by, booking.date_cancelled as booking_date_cancelled,
            booking.is_patient_missed_appt as booking_is_patient_missed_appt,booking.is_provider_missed_appt as booking_is_provider_missed_appt,
            booking.is_emergency as booking_is_emergency,
            booking.need_to_generate_first_letter as booking_need_to_generate_first_letter,booking.need_to_generate_last_letter as booking_need_to_generate_last_letter,booking.has_generated_system_letters as booking_has_generated_system_letters,
            booking.arrival_time              as booking_arrival_time,
            booking.sterilisation_code        as booking_sterilisation_code,
            booking.informed_consent_added_by as booking_informed_consent_added_by, 
            booking.informed_consent_date     as booking_informed_consent_date,
            booking.is_recurring as booking_is_recurring,booking.recurring_weekday_id as booking_recurring_weekday_id,
            booking.recurring_start_time as booking_recurring_start_time,booking.recurring_end_time as booking_recurring_end_time,


            -- patientreferrer info

            pr.patient_referrer_id as pr_patient_referrer_id, pr.patient_id as pr_patient_id, pr.register_referrer_id as pr_register_referrer_id, pr.organisation_id as pr_organisation_id, pr.patient_referrer_date_added as pr_patient_referrer_date_added, pr.is_debtor as pr_is_debtor, pr.is_active as pr_is_active, 
            pat.patient_id as patient_patient_id,pat.person_id as patient_person_id, pat.patient_date_added as patient_patient_date_added, pat.is_clinic_patient as patient_is_clinic_patient, pat.is_gp_patient as patient_is_gp_patient, pat.is_deleted as patient_is_deleted, pat.is_deceased as patient_is_deceased, 
            pat.flashing_text as patient_flashing_text, pat.flashing_text_added_by as patient_flashing_text_added_by, pat.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
            pat.private_health_fund as patient_private_health_fund, pat.concession_card_number as patient_concession_card_number, pat.concession_card_expiry_date as patient_concession_card_expiry_date, pat.is_diabetic as patient_is_diabetic, pat.is_member_diabetes_australia as patient_is_member_diabetes_australia, pat.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, pat.ac_inv_offering_id as patient_ac_inv_offering_id, pat.ac_pat_offering_id as patient_ac_pat_offering_id, pat.login as patient_login, pat.pwd as patient_pwd, pat.is_company as patient_is_company, pat.abn as patient_abn, 
            pat.next_of_kin_name as patient_next_of_kin_name, pat.next_of_kin_relation as patient_next_of_kin_relation, pat.next_of_kin_contact_info as patient_next_of_kin_contact_info,
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
            " + PersonDB.GetFields("patient_person_", "patient_person") + @",
            patient_person_title.title_id as patient_person_title_title_id, patient_person_title.descr as patient_person_title_descr,
            " + PersonDB.GetFields("referrer_person_", "referrer_person") + @",
            referrer_person_title.title_id as referrer_person_title_title_id, referrer_person_title.descr as referrer_person_title_descr,

            -- hc info

            hc.health_card_id as hc_health_card_id, hc.patient_id as hc_patient_id, hc.organisation_id as hc_organisation_id, hc.card_name as hc_card_name, hc.card_nbr as hc_card_nbr, hc.card_family_member_nbr as hc_card_family_member_nbr, hc.expiry_date as hc_expiry_date,
            hc.date_referral_signed as hc_date_referral_signed, hc.date_referral_received_in_office as hc_date_referral_received_in_office, hc.is_active as hc_is_active, hc.added_or_last_modified_by as hc_added_or_last_modified_by, hc.added_or_last_modified_date as hc_added_or_last_modified_date, hc.area_treated as hc_area_treated, 

            -- if regref has email
             CASE WHEN EXISTS (SELECT * FROM " + Utilities.GetAddressType() + @" WHERE contact_date_deleted IS NULL AND entity_id = org.entity_id AND contact_type_id = 27 AND addr_line1 LIKE '%_@_%_.__%') THEN '1' ELSE '0' END AS ref_has_email,
            (SELECT TOP 1 addr_line1 FROM " + Utilities.GetAddressType() + @" WHERE contact_date_deleted IS NULL AND entity_id = org.entity_id AND contact_type_id = 27) as ref_email,
             CASE WHEN EXISTS (SELECT * FROM " + Utilities.GetAddressType() + @" WHERE contact_date_deleted IS NULL AND entity_id = org.entity_id AND contact_type_id = 29 AND PATINDEX('%[0-9][0-9][0-9]%', addr_line1) > 0) THEN '1' ELSE '0' END AS ref_has_fax,
            (SELECT TOP 1 addr_line1 FROM " + Utilities.GetAddressType() + @" WHERE contact_date_deleted IS NULL AND entity_id = org.entity_id AND contact_type_id = 29) as ref_fax

        FROM

            BookingPatient                   AS bp
            INNER JOIN Booking               AS booking     ON bp.booking_id                  = booking.booking_id
            INNER JOIN Staff                 AS bk_provider ON Booking.provider               = bk_provider.staff_id    -- just to update the field_id for the offering

            INNER JOIN PatientReferrer       AS pr          ON pr.patient_id                  = bp.patient_id
            INNER JOIN Patient               AS pat         ON pat.patient_id                 = pr.patient_id 
            LEFT OUTER JOIN RegisterReferrer AS regref      ON regref.register_referrer_id    = pr.register_referrer_id 
            LEFT OUTER JOIN Referrer         AS ref         ON ref.referrer_id                = regref.referrer_id 
            LEFT OUTER JOIN Organisation     AS org         ON org.organisation_id            = regref.organisation_id
            INNER JOIN Person AS patient_person             ON patient_person.person_id       = pat.person_id
            INNER JOIN Title  AS patient_person_title       ON patient_person_title.title_id  = patient_person.title_id
            INNER JOIN Person AS referrer_person            ON referrer_person.person_id      = ref.person_id
            INNER JOIN Title  AS referrer_person_title      ON referrer_person_title.title_id = referrer_person.title_id

            INNER JOIN Offering              AS offering ON pat.ac_inv_offering_id         = offering.offering_id

            INNER JOIN Invoice AS invoice ON invoice.booking_id = booking.booking_id AND invoice.payer_organisation_id = -1

            INNER JOIN HealthCard AS hc ON bp.patient_id = hc.patient_id AND hc.is_active = 1

        WHERE
            bp.is_deleted = 0 AND
            booking.deleted_by IS NULL AND booking.date_deleted IS NULL AND
            booking.booking_type_id = 34 AND
            booking.booking_status_id = 187 AND
            " + sentOrUnsent + @"
            " + referrerBatching + @"
            pr.is_active = 1 
            " + (register_referrer_id == -1 ? "" : "AND regref.register_referrer_id = " + register_referrer_id) + @"
            " + (patient_id == -1 ? "" : "AND booking.patient_id = " + patient_id) + @"
            " + (bookingStartDate == DateTime.MinValue ? "" : " AND booking.date_start >= '" + bookingStartDate.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
            " + (bookingEndDate == DateTime.MinValue ? "" : " AND booking.date_end   <= '" + bookingEndDate.Date.AddHours(23).ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
            AND (bp.need_to_generate_first_letter = 1 OR bp.need_to_generate_last_letter = 1)
        ORDER BY 
            regref.batch_send_all_patients_treatment_notes, ref_has_email DESC, referrer_person.surname,referrer_person.firstname, booking.patient_id";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }


    #region JoinedSQL

    protected static string JoinedSql(bool inc_deep_booking_info = true)
    {
        string sql = @"
                SELECT 

                        bp.booking_patient_id as bp_booking_patient_id, 
                        bp.booking_id         as bp_booking_id,
                        bp.patient_id         as bp_patient_id, 
                        bp.entity_id          as bp_entity_id, 
                        bp.offering_id        as bp_offering_id, 
                        bp.area_treated       as bp_area_treated,
                        bp.added_by           as bp_added_by, 
                        bp.added_date         as bp_added_date,
                        bp.is_deleted         as bp_is_deleted,
                        bp.deleted_by         as bp_deleted_by, 
                        bp.deleted_date       as bp_deleted_date,
                        bp.need_to_generate_first_letter as bp_need_to_generate_first_letter,
                        bp.need_to_generate_last_letter  as bp_need_to_generate_last_letter,
                        bp.has_generated_system_letters  as bp_has_generated_system_letters,

                        (SELECT COUNT(*) FROM Note note WHERE note.entity_id = bp.entity_id) AS bp_note_count,

                        booking.booking_id as booking_booking_id,booking.entity_id as booking_entity_id,booking.date_start as booking_date_start,booking.date_end as booking_date_end,booking.organisation_id as booking_organisation_id,
                        booking.provider as booking_provider,booking.patient_id as booking_patient_id,booking.offering_id as booking_offering_id,booking.booking_type_id as booking_booking_type_id,
                        booking.booking_status_id as booking_booking_status_id,booking.booking_unavailability_reason_id as booking_booking_unavailability_reason_id,
                        booking.added_by as booking_added_by,booking.date_created as booking_date_created,
                        booking.booking_confirmed_by_type_id as booking_booking_confirmed_by_type_id,booking.confirmed_by as booking_confirmed_by,booking.date_confirmed as booking_date_confirmed,
                        booking.deleted_by as booking_deleted_by, booking.date_deleted as booking_date_deleted,
                        booking.cancelled_by as booking_cancelled_by, booking.date_cancelled as booking_date_cancelled,
                        booking.is_patient_missed_appt as booking_is_patient_missed_appt,booking.is_provider_missed_appt as booking_is_provider_missed_appt,
                        booking.is_emergency as booking_is_emergency,
                        booking.need_to_generate_first_letter as booking_need_to_generate_first_letter,booking.need_to_generate_last_letter as booking_need_to_generate_last_letter,booking.has_generated_system_letters as booking_has_generated_system_letters,
                        booking.arrival_time              as booking_arrival_time,
                        booking.sterilisation_code        as booking_sterilisation_code,
                        booking.informed_consent_added_by as booking_informed_consent_added_by, 
                        booking.informed_consent_date     as booking_informed_consent_date,
                        booking.is_recurring as booking_is_recurring,booking.recurring_weekday_id as booking_recurring_weekday_id,
                        booking.recurring_start_time as booking_recurring_start_time,booking.recurring_end_time as booking_recurring_end_time,

                        ";
        if (inc_deep_booking_info)
            sql += @"
                        booking_status.booking_status_id as booking_status_booking_status_id,
                        booking_status.descr as booking_status_descr,

                        booking_confirmed_by_type.booking_confirmed_by_type_id as booking_confirmed_by_type_booking_confirmed_by_type_id,
                        booking_confirmed_by_type.descr as booking_confirmed_by_type_descr,

                        booking_unavailability_reason.booking_unavailability_reason_id as booking_unavailability_reason_booking_unavailability_reason_id,
                        booking_unavailability_reason.descr as booking_unavailability_reason_descr,

                        (SELECT COUNT(*) FROM Note note WHERE note.entity_id = booking.entity_id) AS booking_note_count,
                        (SELECT COUNT(*) FROM Invoice inv WHERE inv.booking_id = booking.booking_id) AS booking_inv_count,

                        booking_offering.offering_id as booking_offering_offering_id, booking_offering.offering_type_id as booking_offering_offering_type_id, booking_offering.field_id as booking_offering_field_id, booking_offering.aged_care_patient_type_id as booking_offering_aged_care_patient_type_id, 
                        booking_offering.num_clinic_visits_allowed_per_year as booking_offering_num_clinic_visits_allowed_per_year, booking_offering.offering_invoice_type_id as booking_offering_offering_invoice_type_id, 
                        booking_offering.name as booking_offering_name, booking_offering.short_name as booking_offering_short_name, booking_offering.descr as booking_offering_descr, booking_offering.is_gst_exempt as booking_offering_is_gst_exempt, booking_offering.default_price as booking_offering_default_price, booking_offering.service_time_minutes as booking_offering_service_time_minutes, 
                        booking_offering.max_nbr_claimable as booking_offering_max_nbr_claimable, booking_offering.max_nbr_claimable_months as booking_offering_max_nbr_claimable_months, booking_offering.medicare_company_code as booking_offering_medicare_company_code, booking_offering.dva_company_code as booking_offering_dva_company_code, booking_offering.tac_company_code as booking_offering_tac_company_code, 
                        booking_offering.medicare_charge as booking_offering_medicare_charge, booking_offering.dva_charge as booking_offering_dva_charge, booking_offering.tac_charge as booking_offering_tac_charge, 
                        booking_offering.popup_message as booking_offering_popup_message, booking_offering.reminder_letter_months_later_to_send as booking_offering_reminder_letter_months_later_to_send, booking_offering.reminder_letter_id as booking_offering_reminder_letter_id, booking_offering.use_custom_color as booking_offering_use_custom_color, booking_offering.custom_color as booking_offering_custom_color,

                        booking_org.organisation_id as booking_organisation_organisation_id, booking_org.entity_id as booking_organisation_entity_id, booking_org.parent_organisation_id as booking_organisation_parent_organisation_id, booking_org.use_parent_offernig_prices as booking_organisation_use_parent_offernig_prices, 
                        booking_org.organisation_type_id as booking_organisation_organisation_type_id, 
                        booking_org.organisation_customer_type_id as booking_organisation_organisation_customer_type_id,booking_org.name as booking_organisation_name, booking_org.acn as booking_organisation_acn, booking_org.abn as booking_organisation_abn, 
                        booking_org.organisation_date_added as booking_organisation_organisation_date_added, booking_org.organisation_date_modified as booking_organisation_organisation_date_modified, booking_org.is_debtor as booking_organisation_is_debtor, booking_org.is_creditor as booking_organisation_is_creditor, 
                        booking_org.bpay_account as booking_organisation_bpay_account, booking_org.weeks_per_service_cycle as booking_organisation_weeks_per_service_cycle, booking_org.start_date as booking_organisation_start_date, 
                        booking_org.end_date as booking_organisation_end_date, booking_org.comment as booking_organisation_comment, booking_org.free_services as booking_organisation_free_services, booking_org.excl_sun as booking_organisation_excl_sun, 
                        booking_org.excl_mon as booking_organisation_excl_mon, booking_org.excl_tue as booking_organisation_excl_tue, booking_org.excl_wed as booking_organisation_excl_wed, booking_org.excl_thu as booking_organisation_excl_thu, 
                        booking_org.excl_fri as booking_organisation_excl_fri, booking_org.excl_sat as booking_organisation_excl_sat, 
                        booking_org.sun_start_time as booking_organisation_sun_start_time, booking_org.sun_end_time as booking_organisation_sun_end_time, 
                        booking_org.mon_start_time as booking_organisation_mon_start_time, booking_org.mon_end_time as booking_organisation_mon_end_time, booking_org.tue_start_time as booking_organisation_tue_start_time, 
                        booking_org.tue_end_time as booking_organisation_tue_end_time, booking_org.wed_start_time as booking_organisation_wed_start_time, booking_org.wed_end_time as booking_organisation_wed_end_time, 
                        booking_org.thu_start_time as booking_organisation_thu_start_time, booking_org.thu_end_time as booking_organisation_thu_end_time, booking_org.fri_start_time as booking_organisation_fri_start_time, 
                        booking_org.fri_end_time as booking_organisation_fri_end_time, booking_org.sat_start_time as booking_organisation_sat_start_time, booking_org.sat_end_time as booking_organisation_sat_end_time, 
                        booking_org.sun_lunch_start_time as booking_organisation_sun_lunch_start_time, booking_org.sun_lunch_end_time as booking_organisation_sun_lunch_end_time, 
                        booking_org.mon_lunch_start_time as booking_organisation_mon_lunch_start_time, booking_org.mon_lunch_end_time as booking_organisation_mon_lunch_end_time, booking_org.tue_lunch_start_time as booking_organisation_tue_lunch_start_time, 
                        booking_org.tue_lunch_end_time as booking_organisation_tue_lunch_end_time, booking_org.wed_lunch_start_time as booking_organisation_wed_lunch_start_time, booking_org.wed_lunch_end_time as booking_organisation_wed_lunch_end_time, 
                        booking_org.thu_lunch_start_time as booking_organisation_thu_lunch_start_time, booking_org.thu_lunch_end_time as booking_organisation_thu_lunch_end_time, booking_org.fri_lunch_start_time as booking_organisation_fri_lunch_start_time, 
                        booking_org.fri_lunch_end_time as booking_organisation_fri_lunch_end_time, booking_org.sat_lunch_start_time as booking_organisation_sat_lunch_start_time, booking_org.sat_lunch_end_time as booking_organisation_sat_lunch_end_time, 
                        booking_org.last_batch_run as booking_organisation_last_batch_run, booking_org.is_deleted as booking_organisation_is_deleted,

                        booking_patient.patient_id as booking_patient_patient_id, booking_patient.person_id as booking_patient_person_id, booking_patient.patient_date_added as booking_patient_patient_date_added, booking_patient.is_clinic_patient as booking_patient_is_clinic_patient, booking_patient.is_gp_patient as booking_patient_is_gp_patient, 
                        booking_patient.is_deleted as booking_patient_is_deleted, booking_patient.is_deceased as booking_patient_is_deceased, 
                        booking_patient.flashing_text as booking_patient_flashing_text, booking_patient.flashing_text_added_by as booking_patient_flashing_text_added_by, booking_patient.flashing_text_last_modified_date as booking_patient_flashing_text_last_modified_date, 
                        booking_patient.private_health_fund as booking_patient_private_health_fund, booking_patient.concession_card_number as booking_patient_concession_card_number, booking_patient.concession_card_expiry_date as booking_patient_concession_card_expiry_date, booking_patient.is_diabetic as booking_patient_is_diabetic, booking_patient.is_member_diabetes_australia as booking_patient_is_member_diabetes_australia, booking_patient.diabetic_assessment_review_date as booking_patient_diabetic_assessment_review_date, booking_patient.ac_inv_offering_id as booking_patient_ac_inv_offering_id, booking_patient.ac_pat_offering_id as booking_patient_ac_pat_offering_id, booking_patient.ac_pat_offering_id as booking_patient_ac_pat_offering_id, booking_patient.login as booking_patient_login, booking_patient.pwd as booking_patient_pwd, booking_patient.is_company as booking_patient_is_company, booking_patient.abn as booking_patient_abn, 
                        booking_patient.next_of_kin_name as booking_patient_next_of_kin_name, booking_patient.next_of_kin_relation as booking_patient_next_of_kin_relation, booking_patient.next_of_kin_contact_info as booking_patient_next_of_kin_contact_info, 

                        " + PersonDB.GetFields("booking_patient_person_", "booking_patient_person") + @",
                        title_booking_patient.title_id as title_booking_patient_title_id, title_booking_patient.descr as title_booking_patient_descr,

                        booking_staff.staff_id as booking_staff_staff_id,booking_staff.person_id as booking_staff_person_id,booking_staff.login as booking_staff_login,booking_staff.pwd as booking_staff_pwd,booking_staff.staff_position_id as booking_staff_staff_position_id,
                        booking_staff.field_id as booking_staff_field_id,booking_staff.is_contractor as booking_staff_is_contractor,booking_staff.tfn as booking_staff_tfn,
                        booking_staff.is_fired as booking_staff_is_fired,booking_staff.costcentre_id as booking_staff_costcentre_id,
                        booking_staff.is_admin as booking_staff_is_admin,booking_staff.provider_number as booking_staff_provider_number,booking_staff.is_commission as booking_staff_is_commission,booking_staff.commission_percent as booking_staff_commission_percent,
                        booking_staff.is_stakeholder as booking_staff_is_stakeholder, booking_staff.is_master_admin as booking_staff_is_master_admin, booking_staff.is_admin as booking_staff_is_admin, booking_staff.is_principal as booking_staff_is_principal, booking_staff.is_provider as booking_staff_is_provider, booking_staff.is_external as booking_staff_is_external,
                        booking_staff.staff_date_added as booking_staff_staff_date_added,booking_staff.start_date as booking_staff_start_date,booking_staff.end_date as booking_staff_end_date,booking_staff.comment as booking_staff_comment, 
                        booking_staff.num_days_to_display_on_booking_screen as booking_staff_num_days_to_display_on_booking_screen,  booking_staff.show_header_on_booking_screen as booking_staff_show_header_on_booking_screen,
                        booking_staff.bk_screen_field_id as booking_staff_bk_screen_field_id, booking_staff.bk_screen_show_key as booking_staff_bk_screen_show_key, booking_staff.enable_daily_reminder_sms as booking_staff_enable_daily_reminder_sms, booking_staff.enable_daily_reminder_email as booking_staff_enable_daily_reminder_email,

                        " + PersonDB.GetFields("booking_staff_person_", "booking_staff_person") + @",
                        title_booking_staff.title_id as title_booking_staff_title_id, title_booking_staff.descr as title_booking_staff_descr,

                        ";
        sql += @"


                        patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, patient.is_clinic_patient as patient_is_clinic_patient, patient.is_gp_patient as patient_is_gp_patient, 
                        patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                        patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                        patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn, 
                        patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info,

                        " + PersonDB.GetFields("patient_person_", "patient_person") + @",
                        title_patient.title_id as title_patient_title_id, title_patient.descr as title_patient_descr,

                        bp_offering.offering_id as bp_offering_offering_id, bp_offering.offering_type_id as bp_offering_offering_type_id, bp_offering.field_id as bp_offering_field_id,
                        bp_offering.aged_care_patient_type_id as bp_offering_aged_care_patient_type_id, 
                        bp_offering.num_clinic_visits_allowed_per_year as bp_offering_num_clinic_visits_allowed_per_year, 
                        bp_offering.offering_invoice_type_id as bp_offering_offering_invoice_type_id, 
                        bp_offering.name as bp_offering_name, bp_offering.short_name as bp_offering_short_name, bp_offering.descr as bp_offering_descr, bp_offering.is_gst_exempt as bp_offering_is_gst_exempt, 
                        bp_offering.default_price as bp_offering_default_price, bp_offering.service_time_minutes as bp_offering_service_time_minutes, 
                        bp_offering.max_nbr_claimable as bp_offering_max_nbr_claimable, bp_offering.max_nbr_claimable_months as bp_offering_max_nbr_claimable_months,
                        bp_offering.medicare_company_code as bp_offering_medicare_company_code, bp_offering.dva_company_code as bp_offering_dva_company_code, bp_offering.tac_company_code as bp_offering_tac_company_code, 
                        bp_offering.medicare_charge as bp_offering_medicare_charge, bp_offering.dva_charge as bp_offering_dva_charge, bp_offering.tac_charge as bp_offering_tac_charge,
                        bp_offering.popup_message as bp_offering_popup_message, bp_offering.reminder_letter_months_later_to_send as bp_offering_reminder_letter_months_later_to_send, bp_offering.reminder_letter_id as bp_offering_reminder_letter_id, bp_offering.use_custom_color as bp_offering_use_custom_color, bp_offering.custom_color as bp_offering_custom_color,

                        bp_offeringfield.field_id as bp_offeringfield_field_id, bp_offeringfield.descr as bp_offeringfield_descr,

                        added_by.staff_id as added_by_staff_id,added_by.person_id as added_by_person_id,added_by.login as added_by_login,added_by.pwd as added_by_pwd,added_by.staff_position_id as added_by_staff_position_id,
                        added_by.field_id as added_by_field_id,added_by.is_contractor as added_by_is_contractor,added_by.tfn as added_by_tfn,
                        added_by.is_fired as added_by_is_fired,added_by.costcentre_id as added_by_costcentre_id,
                        added_by.is_admin as added_by_is_admin,added_by.provider_number as added_by_provider_number,added_by.is_commission as added_by_is_commission,added_by.commission_percent as added_by_commission_percent,
                        added_by.is_stakeholder as added_by_is_stakeholder, added_by.is_master_admin as added_by_is_master_admin, added_by.is_admin as added_by_is_admin, added_by.is_principal as added_by_is_principal, added_by.is_provider as added_by_is_provider, added_by.is_external as added_by_is_external,
                        added_by.staff_date_added as added_by_staff_date_added,added_by.start_date as added_by_start_date,added_by.end_date as added_by_end_date,added_by.comment as added_by_comment, 
                        added_by.num_days_to_display_on_booking_screen as added_by_num_days_to_display_on_booking_screen,  added_by.show_header_on_booking_screen as added_by_show_header_on_booking_screen,
                        added_by.bk_screen_field_id as added_by_bk_screen_field_id, added_by.bk_screen_show_key as added_by_bk_screen_show_key, added_by.enable_daily_reminder_sms as added_by_enable_daily_reminder_sms, added_by.enable_daily_reminder_email as added_by_enable_daily_reminder_email,

                        " + PersonDB.GetFields("added_by_person_", "added_by_person") + @",
                        title_added_by.title_id as title_added_by_title_id, title_added_by.descr as title_added_by_descr,


                        deleted_by.staff_id as deleted_by_staff_id,deleted_by.person_id as deleted_by_person_id,deleted_by.login as deleted_by_login,deleted_by.pwd as deleted_by_pwd,deleted_by.staff_position_id as deleted_by_staff_position_id,
                        deleted_by.field_id as deleted_by_field_id,deleted_by.is_contractor as deleted_by_is_contractor,deleted_by.tfn as deleted_by_tfn,
                        deleted_by.is_fired as deleted_by_is_fired,deleted_by.costcentre_id as deleted_by_costcentre_id,
                        deleted_by.is_admin as deleted_by_is_admin,deleted_by.provider_number as deleted_by_provider_number,deleted_by.is_commission as deleted_by_is_commission,deleted_by.commission_percent as deleted_by_commission_percent,
                        deleted_by.is_stakeholder as deleted_by_is_stakeholder, deleted_by.is_master_admin as deleted_by_is_master_admin, deleted_by.is_admin as deleted_by_is_admin, deleted_by.is_principal as deleted_by_is_principal, deleted_by.is_provider as deleted_by_is_provider, deleted_by.is_external as deleted_by_is_external,
                        deleted_by.staff_date_added as deleted_by_staff_date_added,deleted_by.start_date as deleted_by_start_date,deleted_by.end_date as deleted_by_end_date,deleted_by.comment as deleted_by_comment, 
                        deleted_by.num_days_to_display_on_booking_screen as deleted_by_num_days_to_display_on_booking_screen, deleted_by.show_header_on_booking_screen as deleted_by_show_header_on_booking_screen, 
                        deleted_by.bk_screen_field_id as deleted_by_bk_screen_field_id, deleted_by.bk_screen_show_key as deleted_by_bk_screen_show_key, deleted_by.enable_daily_reminder_sms as deleted_by_enable_daily_reminder_sms, deleted_by.enable_daily_reminder_email as deleted_by_enable_daily_reminder_email,

                        " + PersonDB.GetFields("deleted_by_person_", "deleted_by_person") + @",
                        title_deleted_by.title_id as title_deleted_by_title_id, title_deleted_by.descr as title_deleted_by_descr


                FROM 
                        BookingPatient bp

                        LEFT OUTER JOIN Booking                booking                ON bp.booking_id                    = booking.booking_id
                        ";

        if (inc_deep_booking_info)
            sql += @"
                        LEFT OUTER JOIN BookingStatus          booking_status         ON booking.booking_status_id        = booking_status.booking_status_id
                        LEFT OUTER JOIN BookingConfirmedByType      booking_confirmed_by_type     ON booking.booking_confirmed_by_type_id     = booking_confirmed_by_type.booking_confirmed_by_type_id
                        LEFT OUTER JOIN BookingUnavailabilityReason booking_unavailability_reason ON booking.booking_unavailability_reason_id = booking_unavailability_reason.booking_unavailability_reason_id
                        LEFT OUTER JOIN Offering               booking_offering       ON booking.offering_id              = booking_offering.offering_id
                        LEFT OUTER JOIN Organisation           booking_org            ON booking.organisation_id          = booking_org.organisation_id
                        LEFT OUTER JOIN Patient                booking_patient        ON booking.patient_id               = booking_patient.patient_id
                        LEFT OUTER JOIN Person                 booking_patient_person ON booking_patient_person.person_id = booking_patient.person_id
                        LEFT OUTER JOIN Title                  title_booking_patient  ON title_booking_patient.title_id   = booking_patient_person.title_id
                        LEFT OUTER JOIN Staff                  booking_staff          ON booking.provider                 = booking_staff.staff_id
                        LEFT OUTER JOIN Person                 booking_staff_person   ON booking_staff_person.person_id   = booking_staff.person_id
                        LEFT OUTER JOIN Title                  title_booking_staff    ON title_booking_staff.title_id     = booking_staff_person.title_id

                        ";
        sql += @"
                        LEFT OUTER JOIN Patient                patient                ON bp.patient_id               = patient.patient_id
                        LEFT OUTER JOIN Person                 patient_person         ON patient_person.person_id    = patient.person_id
                        LEFT OUTER JOIN Title                  title_patient          ON title_patient.title_id      = patient_person.title_id

                        LEFT OUTER JOIN Offering               bp_offering            ON bp.offering_id              = bp_offering.offering_id
                        LEFT OUTER JOIN Field                  bp_offeringfield       ON bp_offering.field_id        = bp_offeringfield.field_id

                        LEFT OUTER JOIN Staff                  added_by               ON bp.added_by                 = added_by.staff_id
                        LEFT OUTER JOIN Person                 added_by_person        ON added_by_person.person_id   = added_by.person_id
                        LEFT OUTER JOIN Title                  title_added_by         ON title_added_by.title_id     = added_by_person.title_id

                        LEFT OUTER JOIN Staff                  deleted_by             ON bp.deleted_by               = deleted_by.staff_id
                        LEFT OUTER JOIN Person                 deleted_by_person      ON deleted_by_person.person_id = deleted_by.person_id
                        LEFT OUTER JOIN Title                  title_deleted_by       ON title_deleted_by.title_id   = deleted_by_person.title_id

                    ";

        return sql;
    }

    #endregion

    public static BookingPatient LoadAll(DataRow row, bool inc_deep_booking_info = true)
    {
        BookingPatient bp = Load(row, "bp_");


        if (row["booking_booking_id"] != DBNull.Value)
        {
            bp.Booking = BookingDB.Load(row, "booking_", inc_deep_booking_info, inc_deep_booking_info);

            if (inc_deep_booking_info)
            {
                bp.Booking.BookingStatus = IDandDescrDB.Load(row, "booking_status_booking_status_id", "booking_status_descr");

                if (row["booking_offering_offering_id"] != DBNull.Value)
                    bp.Booking.Offering = OfferingDB.Load(row, "booking_offering_");
                if (row["booking_organisation_organisation_id"] != DBNull.Value)
                    bp.Booking.Organisation = OrganisationDB.Load(row, "booking_organisation_");
                if (row["booking_patient_patient_id"] != DBNull.Value)
                {
                    bp.Booking.Patient = PatientDB.Load(row, "booking_patient_");
                    bp.Booking.Patient.Person = PersonDB.Load(row, "booking_patient_person_");
                    bp.Booking.Patient.Person.Title = IDandDescrDB.Load(row, "title_booking_patient_title_id", "title_booking_patient_descr");
                }
                if (row["booking_staff_staff_id"] != DBNull.Value)
                {
                    bp.Booking.Provider = StaffDB.Load(row, "booking_staff_");
                    bp.Booking.Provider.Person = PersonDB.Load(row, "booking_staff_person_");
                    bp.Booking.Provider.Person.Title = IDandDescrDB.Load(row, "title_booking_staff_title_id", "title_booking_staff_descr");
                }
            }

        }


        bp.Patient = PatientDB.Load(row, "patient_");
        bp.Patient.Person = PersonDB.Load(row, "patient_person_");
        bp.Patient.Person.Title = IDandDescrDB.Load(row, "title_patient_title_id", "title_patient_descr");

        if (row["bp_offering_offering_id"] != DBNull.Value)
        {
            bp.Offering = OfferingDB.Load(row, "bp_offering_");
            bp.Offering.Field = IDandDescrDB.Load(row, "bp_offeringfield_field_id", "bp_offeringfield_descr");
        }

        bp.AddedBy = StaffDB.Load(row, "added_by_");
        bp.AddedBy.Person = PersonDB.Load(row, "added_by_person_");
        bp.AddedBy.Person.Title = IDandDescrDB.Load(row, "title_added_by_title_id", "title_added_by_descr");

        if (row["deleted_by_person_person_id"] != DBNull.Value) 
        {
            bp.DeletedBy = StaffDB.Load(row, "deleted_by_");
            bp.DeletedBy.Person = PersonDB.Load(row, "deleted_by_person_");
            bp.DeletedBy.Person.Title = IDandDescrDB.Load(row, "title_deleted_by_title_id", "title_deleted_by_descr");
        }

        return bp;
    }

    public static BookingPatient Load(DataRow row, string prefix = "")
    {
        return new BookingPatient(
            Convert.ToInt32(row[prefix + "booking_patient_id"]),
            Convert.ToInt32(row[prefix + "booking_id"]),
            Convert.ToInt32(row[prefix + "patient_id"]),
            Convert.ToInt32(row[prefix + "entity_id"]),
            row[prefix + "offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "offering_id"]),
            Convert.ToString(row[prefix + "area_treated"]),
            Convert.ToInt32(row[prefix + "added_by"]),
            Convert.ToDateTime(row[prefix + "added_date"]),
            Convert.ToBoolean(row[prefix + "is_deleted"]),
            row[prefix + "deleted_by"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "deleted_by"]),
            row[prefix + "deleted_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "deleted_date"]),
            Convert.ToBoolean(row[prefix + "need_to_generate_first_letter"]),
            Convert.ToBoolean(row[prefix + "need_to_generate_last_letter"]),
            Convert.ToBoolean(row[prefix + "has_generated_system_letters"]),
            
            Convert.ToInt32(row[prefix + "note_count"])
        );
    }

}
