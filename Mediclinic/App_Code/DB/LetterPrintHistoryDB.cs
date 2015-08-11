using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class LetterFile
{
    public string Name;
    public byte[] Contents;

    public LetterFile(string Name, byte[] Contents)
    {
        this.Contents = Contents;
        this.Name     = Name;
    }
}


public class LetterPrintHistoryDB
{

    public static void Delete(int letter_print_history_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM LetterPrintHistory WHERE letter_print_history_id = " + letter_print_history_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int letter_id, int letter_print_history_send_method_id, int booking_id, int patient_id, int organisation_id, int register_referrer_id, int staff_id, int health_card_action_id, string doc_name, byte[] doc_contents)
    {
        //string sql = "INSERT INTO LetterPrintHistory (letter_id,patient_id,organisation_id,referrer_id,staff_id) VALUES (" + "" + letter_id + "," + "" + (patient_id == -1 ? "NULL" : patient_id.ToString()) + "," + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + "," + (referrer_id == -1 ? "NULL" : referrer_id.ToString()) + "," + (staff_id == -1 ? "NULL" : staff_id.ToString()) + ");SELECT SCOPE_IDENTITY();";
        //return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));

        string sql = "INSERT INTO LetterPrintHistory (letter_id,letter_print_history_send_method_id,booking_id,patient_id,organisation_id,register_referrer_id,staff_id,health_card_action_id,doc_name,doc_contents) VALUES (@letter_id, @letter_print_history_send_method_id, @booking_id, @patient_id, @organisation_id, @register_referrer_id, @staff_id, @health_card_action_id, @doc_name, @doc_contents);SELECT SCOPE_IDENTITY();";

        object result = null;

        using (System.Data.SqlClient.SqlConnection _con = new System.Data.SqlClient.SqlConnection(DBBase.GetConnectionString()))
        {
            using (System.Data.SqlClient.SqlCommand _cmd = new System.Data.SqlClient.SqlCommand(sql, _con))
            {
                System.Data.SqlClient.SqlParameter _letter_id              = _cmd.Parameters.Add("@letter_id",             SqlDbType.Int);
                System.Data.SqlClient.SqlParameter _letter_print_history_send_method_id = _cmd.Parameters.Add("@letter_print_history_send_method_id", SqlDbType.Int);
                System.Data.SqlClient.SqlParameter _booking_id             = _cmd.Parameters.Add("@booking_id",            SqlDbType.Int);
                System.Data.SqlClient.SqlParameter _patient_id             = _cmd.Parameters.Add("@patient_id",            SqlDbType.Int);
                System.Data.SqlClient.SqlParameter _organisation_id        = _cmd.Parameters.Add("@organisation_id",       SqlDbType.Int);
                System.Data.SqlClient.SqlParameter _register_referrer_id   = _cmd.Parameters.Add("@register_referrer_id",  SqlDbType.Int);
                System.Data.SqlClient.SqlParameter _staff_id               = _cmd.Parameters.Add("@staff_id",              SqlDbType.Int);
                System.Data.SqlClient.SqlParameter _health_card_action_id  = _cmd.Parameters.Add("@health_card_action_id", SqlDbType.Int);
                System.Data.SqlClient.SqlParameter _doc_name               = _cmd.Parameters.Add("@doc_name",              SqlDbType.VarChar);
                System.Data.SqlClient.SqlParameter _doc_contents           = _cmd.Parameters.Add("@doc_contents",          SqlDbType.VarBinary);

                _letter_id.Value                           = letter_id;
                _letter_print_history_send_method_id.Value = letter_print_history_send_method_id;
                _booking_id.Value                          = booking_id            == -1   ? (object)DBNull.Value : booking_id;
                _patient_id.Value                          = patient_id            == -1   ? (object)DBNull.Value : patient_id;
                _organisation_id.Value                     = organisation_id       ==  0   ? (object)DBNull.Value : organisation_id;
                _register_referrer_id.Value                = register_referrer_id  == -1   ? (object)DBNull.Value : register_referrer_id;
                _staff_id.Value                            = staff_id              == -1   ? (object)DBNull.Value : staff_id;
                _health_card_action_id.Value               = health_card_action_id == -1   ? (object)DBNull.Value : health_card_action_id;
                _doc_name.Value                            = doc_name;
                _doc_contents.Value                        = doc_contents          == null ? (object)DBNull.Value : doc_contents;

                _con.Open();
                result = _cmd.ExecuteScalar();
                _con.Close();
            }
        }

        return Convert.ToInt32(result);
    }

    public static void Update(int letter_print_history_id, int letter_id, int letter_print_history_send_method_id, int patient_id, int organisation_id, int register_referrer_id, int staff_id)
    {
        string sql = "UPDATE LetterPrintHistory SET letter_id = " + letter_id + ",letter_print_history_send_method_id = " + letter_print_history_send_method_id + ",patient_id = " + (patient_id == -1 ? "NULL" : patient_id.ToString()) + ",organisation_id = " + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + ",referrer_id = " + (register_referrer_id == -1 ? "NULL" : register_referrer_id.ToString()) + ",staff_id = " + (staff_id == -1 ? "NULL" : staff_id.ToString()) + " WHERE letter_print_history_id = " + letter_print_history_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static LetterFile GetLetterFile(int letter_print_history_id)
    {
        string sql = @"SELECT doc_name, doc_contents FROM LetterPrintHistory WHERE letter_print_history_id = " + letter_print_history_id;
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : new LetterFile((string)tbl.Rows[0]["doc_name"], (tbl.Rows[0]["doc_contents"] == DBNull.Value) ? null : (byte[])tbl.Rows[0]["doc_contents"]);
    }

    public static Hashtable GetMostRecentRecallHashByPatients(int[] patient_ids)
    {
        if (patient_ids == null || patient_ids.Length == 0)
            return new Hashtable();

        string sql = JoinedSql + @" 

                        --SELECT * from LetterPrintHistory lph
                        WHERE lph.patient_id in (" + string.Join(",", patient_ids) + @")
                          and lph.letter_print_history_id = (
	                        SELECT TOP 1 letter_print_history_id 
	                        FROM   LetterPrintHistory lph2 
		                           INNER JOIN Letter l ON lph2.letter_id = l.letter_id
	                        WHERE  lph2.patient_id = lph.patient_id
	                           AND l.letter_type_id = 390
	                        ORDER BY lph2.date DESC
                          )";

        Hashtable hash = new Hashtable();

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            LetterPrintHistory lph = LoadAll(tbl.Rows[i]);
            hash[lph.Patient.PatientID] = lph;
        }

        return hash;
    }


    public static string JoinedSql = @"
                        SELECT
                                lph.letter_print_history_id as lph_letter_print_history_id,lph.letter_id as lph_letter_id,lph.letter_print_history_send_method_id as lph_letter_print_history_send_method_id,lph.booking_id as lph_booking_id,
                                lph.patient_id as lph_patient_id,lph.organisation_id as lph_organisation_id,lph.register_referrer_id as lph_register_referrer_id,lph.staff_id as lph_staff_id,lph.health_card_action_id as lph_health_card_action_id,lph.date as lph_date,lph.doc_name as lph_doc_name
                                ,
                                CASE WHEN doc_contents IS NULL THEN 0 ELSE 1 END AS lph_has_doc
                                ,
                                letter.letter_id as letter_letter_id,letter.organisation_id as letter_organisation_id,letter.letter_type_id as letter_letter_type_id,letter.site_id as letter_site_id,
                                letter.code as letter_code,letter.reject_message as letter_reject_message,letter.docname as letter_docname,letter.is_send_to_medico as letter_is_send_to_medico,letter.is_allowed_reclaim as letter_is_allowed_reclaim,letter.is_manual_override as letter_is_manual_override,letter.num_copies_to_print as letter_num_copies_to_print, letter.is_deleted as letter_is_deleted
                                ,
                                lettertype.descr as lettertype_descr, lettertype.letter_type_id as lettertype_letter_type_id
                                ,

                                letterorg.organisation_id as letterorg_organisation_id,letterorg.entity_id as letterorg_entity_id, letterorg.parent_organisation_id as letterorg_parent_organisation_id, letterorg.use_parent_offernig_prices as letterorg_use_parent_offernig_prices, 
                                letterorg.organisation_type_id as letterorg_organisation_type_id, letterorg.organisation_customer_type_id as letterorg_organisation_customer_type_id, letterorg.name as letterorg_name, letterorg.acn as letterorg_acn, letterorg.abn as letterorg_abn, letterorg.organisation_date_added as letterorg_organisation_date_added, letterorg.organisation_date_modified as letterorg_organisation_date_modified, letterorg.is_debtor as letterorg_is_debtor, letterorg.is_creditor as letterorg_is_creditor, letterorg.bpay_account as letterorg_bpay_account, 
                                letterorg.weeks_per_service_cycle as letterorg_weeks_per_service_cycle, letterorg.start_date as letterorg_start_date, letterorg.end_date as letterorg_end_date, letterorg.comment as letterorg_comment, letterorg.free_services as letterorg_free_services, letterorg.excl_sun as letterorg_excl_sun, letterorg.excl_mon as letterorg_excl_mon, letterorg.excl_tue as letterorg_excl_tue, letterorg.excl_wed as letterorg_excl_wed, letterorg.excl_thu as letterorg_excl_thu, letterorg.excl_fri as letterorg_excl_fri, 
                                letterorg.excl_sat as letterorg_excl_sat, 
                                letterorg.sun_start_time as letterorg_sun_start_time, letterorg.sun_end_time as letterorg_sun_end_time, letterorg.mon_start_time as letterorg_mon_start_time, letterorg.mon_end_time as letterorg_mon_end_time, letterorg.tue_start_time as letterorg_tue_start_time, letterorg.tue_end_time as letterorg_tue_end_time, letterorg.wed_start_time as letterorg_wed_start_time, letterorg.wed_end_time as letterorg_wed_end_time, 
                                letterorg.thu_start_time as letterorg_thu_start_time, letterorg.thu_end_time as letterorg_thu_end_time, letterorg.fri_start_time as letterorg_fri_start_time, letterorg.fri_end_time as letterorg_fri_end_time, letterorg.sat_start_time as letterorg_sat_start_time, letterorg.sat_end_time as letterorg_sat_end_time, 
                                letterorg.sun_lunch_start_time as letterorg_sun_lunch_start_time, letterorg.sun_lunch_end_time as letterorg_sun_lunch_end_time, letterorg.mon_lunch_start_time as letterorg_mon_lunch_start_time, letterorg.mon_lunch_end_time as letterorg_mon_lunch_end_time, letterorg.tue_lunch_start_time as letterorg_tue_lunch_start_time, letterorg.tue_lunch_end_time as letterorg_tue_lunch_end_time, letterorg.wed_lunch_start_time as letterorg_wed_lunch_start_time, letterorg.wed_lunch_end_time as letterorg_wed_lunch_end_time, 
                                letterorg.thu_lunch_start_time as letterorg_thu_lunch_start_time, letterorg.thu_lunch_end_time as letterorg_thu_lunch_end_time, letterorg.fri_lunch_start_time as letterorg_fri_lunch_start_time, letterorg.fri_lunch_end_time as letterorg_fri_lunch_end_time, letterorg.sat_lunch_start_time as letterorg_sat_lunch_start_time, letterorg.sat_lunch_end_time as letterorg_sat_lunch_end_time, 
                                letterorg.last_batch_run as letterorg_last_batch_run, letterorg.is_deleted as letterorg_is_deleted
                                ,
                                site.site_id as site_site_id,site.entity_id as site_entity_id,site.name as site_name,site.site_type_id as site_site_type_id,site.abn as site_abn,site.acn as site_acn,site.tfn as site_tfn,
                                site.asic as site_asic,site.is_provider as site_is_provider,site.bank_bpay as site_bank_bpay,site.bank_bsb as site_bank_bsb,site.bank_account as site_bank_account,
                                site.bank_direct_debit_userid as site_bank_direct_debit_userid,site.bank_username as site_bank_username,site.oustanding_balance_warning as site_oustanding_balance_warning,
                                site.print_epc as site_print_epc,site.excl_sun as site_excl_sun,site.excl_mon as site_excl_mon,site.excl_tue as site_excl_tue,site.excl_wed as site_excl_wed,site.excl_thu as site_excl_thu,
                                site.excl_fri as site_excl_fri,site.excl_sat as site_excl_sat,site.day_start_time as site_day_start_time,site.lunch_start_time as site_lunch_start_time,
                                site.lunch_end_time as site_lunch_end_time,site.day_end_time as site_day_end_time,site.fiscal_yr_end as site_fiscal_yr_end,site.num_booking_months_to_get as site_num_booking_months_to_get
                                ,
                                sitetype.descr as site_type_descr 
                                ,


                                lph_send_method.letter_print_history_send_method_id as lph_send_method_letter_print_history_send_method_id, lph_send_method.descr as lph_send_method_descr
                                ,


                                patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, patient.is_clinic_patient as patient_is_clinic_patient, patient.is_gp_patient as patient_is_gp_patient,
                                patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                                patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                                patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn,
                                patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info
                                ,
                                " + PersonDB.GetFields("person_patient_", "person_patient") + @"
                                ,
                                title_patient.title_id as title_patient_title_id, title_patient.descr as title_patient_descr
                                ,


                                org.organisation_id as organisation_organisation_id, org.entity_id as organisation_entity_id, org.parent_organisation_id as organisation_parent_organisation_id, org.use_parent_offernig_prices as organisation_use_parent_offernig_prices, 
                                org.organisation_type_id as organisation_organisation_type_id, org.organisation_customer_type_id as organisation_organisation_customer_type_id, org.name as organisation_name, org.acn as organisation_acn, org.abn as organisation_abn, 
                                org.organisation_date_added as organisation_organisation_date_added, org.organisation_date_modified as organisation_organisation_date_modified, org.is_debtor as organisation_is_debtor, org.is_creditor as organisation_is_creditor, 
                                org.bpay_account as organisation_bpay_account, org.weeks_per_service_cycle as organisation_weeks_per_service_cycle, org.start_date as organisation_start_date, 
                                org.end_date as organisation_end_date, org.comment as organisation_comment, org.free_services as organisation_free_services, org.excl_sun as organisation_excl_sun, 
                                org.excl_mon as organisation_excl_mon, org.excl_tue as organisation_excl_tue, org.excl_wed as organisation_excl_wed, org.excl_thu as organisation_excl_thu, 
                                org.excl_fri as organisation_excl_fri, org.excl_sat as organisation_excl_sat, 
                                org.sun_start_time as organisation_sun_start_time, org.sun_end_time as organisation_sun_end_time, 
                                org.mon_start_time as organisation_mon_start_time, org.mon_end_time as organisation_mon_end_time, org.tue_start_time as organisation_tue_start_time, 
                                org.tue_end_time as organisation_tue_end_time, org.wed_start_time as organisation_wed_start_time, org.wed_end_time as organisation_wed_end_time, 
                                org.thu_start_time as organisation_thu_start_time, org.thu_end_time as organisation_thu_end_time, org.fri_start_time as organisation_fri_start_time, 
                                org.fri_end_time as organisation_fri_end_time, org.sat_start_time as organisation_sat_start_time, org.sat_end_time as organisation_sat_end_time, 
                                org.sun_lunch_start_time as organisation_sun_lunch_start_time, org.sun_lunch_end_time as organisation_sun_lunch_end_time, 
                                org.mon_lunch_start_time as organisation_mon_lunch_start_time, org.mon_lunch_end_time as organisation_mon_lunch_end_time, org.tue_lunch_start_time as organisation_tue_lunch_start_time, 
                                org.tue_lunch_end_time as organisation_tue_lunch_end_time, org.wed_lunch_start_time as organisation_wed_lunch_start_time, org.wed_lunch_end_time as organisation_wed_lunch_end_time, 
                                org.thu_lunch_start_time as organisation_thu_lunch_start_time, org.thu_lunch_end_time as organisation_thu_lunch_end_time, org.fri_lunch_start_time as organisation_fri_lunch_start_time, 
                                org.fri_lunch_end_time as organisation_fri_lunch_end_time, org.sat_lunch_start_time as organisation_sat_lunch_start_time, org.sat_lunch_end_time as organisation_sat_lunch_end_time, 
                                org.last_batch_run as organisation_last_batch_run, org.is_deleted as organisation_is_deleted
                                ,



                                staff.staff_id as staff_staff_id, staff.person_id as staff_person_id, staff.login as staff_login, staff.pwd as staff_pwd, 
                                staff.staff_position_id as staff_staff_position_id, staff.field_id as staff_field_id, staff.costcentre_id as staff_costcentre_id, 
                                staff.is_contractor as staff_is_contractor, staff.tfn as staff_tfn, staff.provider_number as staff_provider_number, 
                                staff.is_fired as staff_is_fired, staff.is_commission as staff_is_commission, staff.commission_percent as staff_commission_percent, 
                                staff.is_stakeholder as staff_is_stakeholder,staff.is_master_admin as staff_is_master_admin,staff.is_admin as staff_is_admin,staff.is_principal as staff_is_principal,staff.is_provider as staff_is_provider, staff.is_external as staff_is_external,
                                staff.staff_date_added as staff_staff_date_added, staff.start_date as staff_start_date, staff.end_date as staff_end_date, staff.comment as staff_comment, 
                                staff.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen, 
                                staff.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                                staff.bk_screen_field_id as staff_bk_screen_field_id,
                                staff.bk_screen_show_key as staff_bk_screen_show_key,
                                staff.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                                staff.enable_daily_reminder_email as staff_enable_daily_reminder_email
                                ,
                                " + PersonDB.GetFields("person_staff_", "person_staff") + @"
                                ,
                                title_staff.title_id as title_staff_title_id, title_staff.descr as title_staff_descr
                                ,


                                regref.register_referrer_id as regref_register_referrer_id,regref.organisation_id as regref_organisation_id, regref.referrer_id as regref_referrer_id, regref.provider_number as regref_provider_number, 
                                regref.report_every_visit_to_referrer as regref_report_every_visit_to_referrer,
                                regref.batch_send_all_patients_treatment_notes as regref_batch_send_all_patients_treatment_notes, regref.date_last_batch_send_all_patients_treatment_notes as regref_date_last_batch_send_all_patients_treatment_notes,
                                regref.register_referrer_date_added as regref_register_referrer_date_added
                                ,  
                                regreforg.entity_id as regreforg_entity_id, regreforg.parent_organisation_id as regreforg_parent_organisation_id, regreforg.use_parent_offernig_prices as regreforg_use_parent_offernig_prices, regreforg.organisation_type_id as regreforg_organisation_type_id, regreforg.organisation_customer_type_id as regreforg_organisation_customer_type_id,org.name as regreforg_name, regreforg.acn as regreforg_acn, regreforg.abn as regreforg_abn, regreforg.organisation_date_added as regreforg_organisation_date_added, 
                                regreforg.organisation_id as regreforg_organisation_id, regreforg.organisation_date_modified as regreforg_organisation_date_modified, regreforg.is_debtor as regreforg_is_debtor, regreforg.is_creditor as regreforg_is_creditor, regreforg.bpay_account as regreforg_bpay_account, regreforg.weeks_per_service_cycle as regreforg_weeks_per_service_cycle, regreforg.start_date as regreforg_start_date, 
                                regreforg.end_date as regreforg_end_date, regreforg.comment as regreforg_comment, regreforg.free_services as regreforg_free_services, regreforg.excl_sun as regreforg_excl_sun, regreforg.excl_mon as regreforg_excl_mon, regreforg.excl_tue as regreforg_excl_tue, regreforg.excl_wed as regreforg_excl_wed, regreforg.excl_thu as regreforg_excl_thu, regreforg.excl_fri as regreforg_excl_fri, regreforg.excl_sat as regreforg_excl_sat, 
                                regreforg.sun_start_time as regreforg_sun_start_time, regreforg.sun_end_time as regreforg_sun_end_time, regreforg.mon_start_time as regreforg_mon_start_time, regreforg.mon_end_time as regreforg_mon_end_time, regreforg.tue_start_time as regreforg_tue_start_time, regreforg.tue_end_time as regreforg_tue_end_time, regreforg.wed_start_time as regreforg_wed_start_time, regreforg.wed_end_time as regreforg_wed_end_time, 
                                regreforg.thu_start_time as regreforg_thu_start_time, regreforg.thu_end_time as regreforg_thu_end_time, regreforg.fri_start_time as regreforg_fri_start_time, regreforg.fri_end_time as regreforg_fri_end_time, regreforg.sat_start_time as regreforg_sat_start_time, regreforg.sat_end_time as regreforg_sat_end_time, 
                                regreforg.sun_lunch_start_time as regreforg_sun_lunch_start_time, regreforg.sun_lunch_end_time as regreforg_sun_lunch_end_time, regreforg.mon_lunch_start_time as regreforg_mon_lunch_start_time, regreforg.mon_lunch_end_time as regreforg_mon_lunch_end_time, regreforg.tue_lunch_start_time as regreforg_tue_lunch_start_time, regreforg.tue_lunch_end_time as regreforg_tue_lunch_end_time, regreforg.wed_lunch_start_time as regreforg_wed_lunch_start_time, regreforg.wed_lunch_end_time as regreforg_wed_lunch_end_time, 
                                regreforg.thu_lunch_start_time as regreforg_thu_lunch_start_time, regreforg.thu_lunch_end_time as regreforg_thu_lunch_end_time, regreforg.fri_lunch_start_time as regreforg_fri_lunch_start_time, regreforg.fri_lunch_end_time as regreforg_fri_lunch_end_time, regreforg.sat_lunch_start_time as regreforg_sat_lunch_start_time, regreforg.sat_lunch_end_time as regreforg_sat_lunch_end_time, 
                                regreforg.last_batch_run as regreforg_last_batch_run, regreforg.is_deleted as regreforg_is_deleted
                                ,
                                ref.referrer_id as referrer_referrer_id,ref.person_id as referrer_person_id, ref.referrer_date_added as referrer_referrer_date_added
                                ,
                                " + PersonDB.GetFields("person_referrer_", "person_referrer") + @"
                                ,
                                title_referrer.title_id as title_referrer_title_id, title_referrer.descr as title_referrer_descr
                                ,


                                hca.health_card_action_id as hca_health_card_action_id, hca.health_card_id as hca_health_card_id, hca.health_card_action_type_id as hca_health_card_action_type_id, hca.action_date as hca_action_date, 
                                hcat.health_card_action_type_id as hcat_health_card_action_type_id,hcat.descr as hcat_descr,

                                hc.health_card_id as hc_health_card_id, hc.patient_id as hc_patient_id, hc.organisation_id as hc_organisation_id, hc.card_name as hc_card_name, hc.card_nbr as hc_card_nbr, hc.card_family_member_nbr as hc_card_family_member_nbr, 
                                hc.expiry_date as hc_expiry_date, hc.date_referral_signed as hc_date_referral_signed,
                                hc.date_referral_received_in_office as hc_date_referral_received_in_office, hc.is_active as hc_is_active,
                                hc.added_or_last_modified_by as hc_added_or_last_modified_by,hc.added_or_last_modified_date as hc_added_or_last_modified_date,
                                hc.area_treated as hc_area_treated
                                ,
                                booking.booking_id as booking_booking_id,booking.entity_id as booking_entity_id,
                                booking.date_start as booking_date_start,booking.date_end as booking_date_end,booking.organisation_id as booking_organisation_id,
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
                                booking.recurring_start_time as booking_recurring_start_time,booking.recurring_end_time as booking_recurring_end_time
                                ,
                                (SELECT COUNT(*) FROM Note note WHERE note.entity_id = booking.entity_id) AS booking_note_count
                                ,
                                (SELECT COUNT(*) FROM Invoice inv WHERE inv.booking_id = booking.booking_id) AS booking_inv_count


                        FROM 
                                LetterPrintHistory lph

                                INNER JOIN Letter                    letter          ON lph.letter_id                    = letter.letter_id
                                INNER JOIN LetterType                lettertype      ON letter.letter_type_id            = lettertype.letter_type_id 
                                INNER JOIN Site                      site            ON site.site_id                     = letter.site_id
                                INNER JOIN SiteType                  sitetype        ON site.site_type_id                = sitetype.site_type_id
                                LEFT OUTER JOIN Organisation         letterorg       ON letterorg.organisation_id        = letter.organisation_id 
                                
                                INNER JOIN LetterPrintHistorySendMethod lph_send_method ON lph_send_method.letter_print_history_send_method_id = lph.letter_print_history_send_method_id

                                LEFT OUTER JOIN Patient              patient         ON patient.patient_id               = lph.patient_id 
                                LEFT OUTER JOIN Person               person_patient  ON person_patient.person_id         = patient.person_id
                                LEFT OUTER JOIN Title                title_patient   ON title_patient.title_id           = person_patient.title_id

                                LEFT OUTER JOIN Organisation         org             ON org.organisation_id              = lph.organisation_id

                                LEFT OUTER JOIN Staff                staff           ON staff.staff_id                   = lph.staff_id
                                LEFT OUTER JOIN Person               person_staff    ON person_staff.person_id           = staff.person_id
                                LEFT OUTER JOIN Title                title_staff     ON title_staff.title_id             = person_staff.title_id


                                LEFT OUTER JOIN RegisterReferrer     regref           ON regref.register_referrer_id     = lph.register_referrer_id 
                                LEFT OUTER JOIN Organisation         regreforg        ON regreforg.organisation_id       = regref.organisation_id 
                                LEFT OUTER JOIN Referrer             ref              ON ref.referrer_id                 = regref.referrer_id 
                                LEFT OUTER JOIN Person               person_referrer  ON person_referrer.person_id       = ref.person_id 
                                LEFT OUTER JOIN Title                title_referrer   ON title_referrer.title_id         = person_referrer.title_id

                                LEFT OUTER JOIN HealthCardAction     hca              ON hca.health_card_action_id       = lph.health_card_action_id
                                LEFT OUTER JOIN HealthCardActionType hcat             ON hcat.health_card_action_type_id = hca.health_card_action_type_iD
                                LEFT OUTER JOIN HealthCard           hc               ON hc.health_card_id               = hca.health_card_id 

                                LEFT OUTER JOIN Booking              booking          ON booking.booking_id              = lph.booking_id ";



    public static DataTable GetDataTable(DateTime bookingStartDate, DateTime bookingEndDate, int organisation_id = 0, int patient_id = -1, int register_referrer_id = -1, int site_id = -1)
    {
        string sql = JoinedSql;

        if (organisation_id != 0 || patient_id != -1 || register_referrer_id != -1 || site_id != -1 || bookingStartDate != DateTime.MinValue || bookingEndDate != DateTime.MinValue)
        {
            string where = string.Empty;

            if (bookingStartDate != DateTime.MinValue)
                where += " AND booking.date_start >= '" + bookingStartDate.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (bookingEndDate != DateTime.MinValue)
                where += " AND booking.date_start <= '" + bookingEndDate.Date.AddHours(23).ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (organisation_id != 0)
                where += " AND lph.organisation_id = " + organisation_id;
            if (patient_id != -1)
                where += " AND lph.patient_id = " + patient_id;
            if (register_referrer_id != -1)
                where += " AND lph.register_referrer_id = " + register_referrer_id;
            if (site_id != -1)
                where += " AND letter.site_id = " + site_id;

            sql += " WHERE " + where.Substring(5);
        }

        sql += " ORDER BY date DESC ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable(DateTime bookingStartDate, DateTime bookingEndDate, string organisation_ids = null, int patient_id = -1, int register_referrer_id = -1, int site_id = -1)
    {
        string sql = JoinedSql;

        if (organisation_ids == null)
            organisation_ids = string.Empty;

        if (organisation_ids != string.Empty || patient_id != -1 || register_referrer_id != -1 || site_id != -1 || bookingStartDate != DateTime.MinValue || bookingEndDate != DateTime.MinValue)
        {
            string where = string.Empty;

            if (bookingStartDate != DateTime.MinValue)
                where += " AND booking.date_start >= '" + bookingStartDate.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (bookingEndDate != DateTime.MinValue)
                where += " AND booking.date_start <= '" + bookingEndDate.Date.AddHours(23).ToString("yyyy-MM-dd HH:mm:ss") + "'";
            if (organisation_ids != string.Empty)
                where += " AND lph.organisation_id IN (" + organisation_ids + ")";
            if (patient_id != -1)
                where += " AND lph.patient_id = " + patient_id;
            if (register_referrer_id != -1)
                where += " AND lph.register_referrer_id = " + register_referrer_id;
            if (site_id != -1)
                where += " AND letter.site_id = " + site_id;

            sql += " WHERE " + where.Substring(5);
        }

        sql += " ORDER BY date DESC ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static LetterPrintHistory GetByID(int letter_print_history_id)
    {
        string sql = JoinedSql + " WHERE lph.letter_print_history_id = " + letter_print_history_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static LetterPrintHistory LoadAll(DataRow row)
    {
        LetterPrintHistory lph = Load(row, "lph_");

        lph.Letter = LetterDB.Load(row, "letter_");

        lph.SendMethod = new IDandDescr(Convert.ToInt32(row["lph_send_method_letter_print_history_send_method_id"]), Convert.ToString(row["lph_send_method_descr"]));

        lph.Letter.LetterType = IDandDescrDB.Load(row, "lettertype_letter_type_id", "lettertype_descr");
        if (row["letterorg_organisation_id"] != DBNull.Value)
            lph.Letter.Organisation = OrganisationDB.Load(row, "letterorg_");

        if (row["organisation_organisation_id"] != DBNull.Value)
            lph.Organisation = OrganisationDB.Load(row, "organisation_");

        if (row["patient_patient_id"] != DBNull.Value)
            lph.Patient = PatientDB.Load(row, "patient_");
        if (row["patient_patient_id"] != DBNull.Value)
        {
            lph.Patient.Person = PersonDB.Load(row, "person_patient_");
            lph.Patient.Person.Title = IDandDescrDB.Load(row, "title_patient_title_id", "title_patient_descr");
        }

        if (row["staff_staff_id"] != DBNull.Value)
            lph.Staff = StaffDB.Load(row, "staff_");
        if (row["staff_staff_id"] != DBNull.Value)
        {
            lph.Staff.Person = PersonDB.Load(row, "person_staff_");
            lph.Staff.Person.Title = IDandDescrDB.Load(row, "title_staff_title_id", "title_staff_descr");
        }

        if (row["regref_register_referrer_id"] != DBNull.Value)
            lph.RegisterReferrer = RegisterReferrerDB.Load(row, "regref_");
        if (row["regreforg_organisation_id"] != DBNull.Value)
            lph.RegisterReferrer.Organisation = OrganisationDB.Load(row, "regreforg_");
        if (row["referrer_referrer_id"] != DBNull.Value)
            lph.RegisterReferrer.Referrer = ReferrerDB.Load(row, "referrer_");
        if (row["referrer_referrer_id"] != DBNull.Value)
        {
            lph.RegisterReferrer.Referrer.Person = PersonDB.Load(row, "person_referrer_");
            lph.RegisterReferrer.Referrer.Person.Title = IDandDescrDB.Load(row, "title_referrer_title_id", "title_referrer_descr");
        }

        if (row["lph_health_card_action_id"] != DBNull.Value)
        {
            lph.HealthCardAction = HealthCardActionDB.Load(row, "hca_");
            lph.HealthCardAction.healthCardActionType = IDandDescrDB.Load(row, "hcat_health_card_action_type_id", "hcat_descr");
            lph.HealthCardAction.HealthCard = HealthCardDB.Load(row, "hc_");
        }

        if (row["lph_booking_id"] != DBNull.Value)
            lph.Booking = BookingDB.Load(row, "booking_");

        return lph;
    }


    public static LetterPrintHistory Load(DataRow row, string prefix = "")
    {
        return new LetterPrintHistory(
            Convert.ToInt32(row[prefix + "letter_print_history_id"]),
            Convert.ToInt32(row[prefix + "letter_id"]),
            Convert.ToInt32(row[prefix + "letter_print_history_send_method_id"]),
            row[prefix + "booking_id"]            == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "booking_id"]),
            row[prefix + "patient_id"]            == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "patient_id"]),
            row[prefix + "organisation_id"]       == DBNull.Value ?  0 : Convert.ToInt32(row[prefix + "organisation_id"]),
            row[prefix + "register_referrer_id"]  == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "register_referrer_id"]),
            row[prefix + "staff_id"]              == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "staff_id"]),
            row[prefix + "health_card_action_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "health_card_action_id"]),
            Convert.ToDateTime(row[prefix+"date"]),
            row[prefix + "doc_name"].ToString(),
            Convert.ToBoolean(row[prefix + "has_doc"])
        );
    }

}