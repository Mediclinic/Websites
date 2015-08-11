using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class LetterTreatmentTemplateDB
{

    public static void Delete(int letter_treatment_template_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM LetterTreatmentTemplate WHERE letter_treatment_template_id = " + letter_treatment_template_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int field_id, int first_letter_id, int last_letter_id, int last_letter_pt_id, int last_letter_when_replacing_epc_id, int treatment_notes_letter_id, int site_id)
    {
        string sql = "INSERT INTO LetterTreatmentTemplate (field_id,first_letter_id,last_letter_id,last_letter_pt_id,last_letter_when_replacing_epc_id,treatment_notes_letter_id,site_id) VALUES (" + "" + field_id + "," + "" + first_letter_id + "," + last_letter_id + "," + last_letter_pt_id + "," + last_letter_when_replacing_epc_id + "," + treatment_notes_letter_id + "," + site_id + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int letter_treatment_template_id, int field_id, int first_letter_id, int last_letter_id, int last_letter_pt_id, int last_letter_when_replacing_epc_id, int treatment_notes_letter_id)
    {
        string sql = "UPDATE LetterTreatmentTemplate SET field_id = " + field_id + ",first_letter_id = " + first_letter_id + ",last_letter_id = " + last_letter_id + ",last_letter_pt_id = " + last_letter_pt_id + ",last_letter_when_replacing_epc_id = " + last_letter_when_replacing_epc_id + ",treatment_notes_letter_id = " + treatment_notes_letter_id + " WHERE letter_treatment_template_id = " + letter_treatment_template_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable(int site_id = -1)
    {
        string sql = JoinedSql + (site_id == -1 ? "" : " WHERE lettertreatmenttemplate.site_id = " + site_id);
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static LetterTreatmentTemplate[] GetAll(int site_id = -1)
    {
        DataTable tbl = GetDataTable(site_id);

        LetterTreatmentTemplate[] list = new LetterTreatmentTemplate[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);

        return list;
    }

    public static LetterTreatmentTemplate GetByID(int letter_treatment_template_id)
    {
        string sql = JoinedSql + " WHERE lettertreatmenttemplate.letter_treatment_template_id = " + letter_treatment_template_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static LetterTreatmentTemplate GetByFieldID(int field_id, int site_id = -1)
    {
        string sql = JoinedSql + " WHERE lettertreatmenttemplate.field_id = " + field_id.ToString() + (site_id == -1 ? "" : " AND lettertreatmenttemplate.site_id = " + site_id);
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }



    public static string JoinedSql = @"
            SELECT

                    lettertreatmenttemplate.letter_treatment_template_id      as lettertreatmenttemplate_letter_treatment_template_id,
                    lettertreatmenttemplate.field_id                          as lettertreatmenttemplate_field_id,
                    lettertreatmenttemplate.first_letter_id                   as lettertreatmenttemplate_first_letter_id,
                    lettertreatmenttemplate.last_letter_id                    as lettertreatmenttemplate_last_letter_id,
                    lettertreatmenttemplate.last_letter_pt_id                 as lettertreatmenttemplate_last_letter_pt_id,
                    lettertreatmenttemplate.last_letter_when_replacing_epc_id as lettertreatmenttemplate_last_letter_when_replacing_epc_id,
                    lettertreatmenttemplate.treatment_notes_letter_id         as lettertreatmenttemplate_treatment_notes_letter_id,
                    lettertreatmenttemplate.site_id                           as lettertreatmenttemplate_site_id,




                    field.field_id as field_field_id,
                    field.descr as field_descr,



                    firstletter.letter_id as firstletter_letter_id,firstletter.organisation_id as firstletter_organisation_id,firstletter.letter_type_id as firstletter_letter_type_id,firstletter.site_id as firstletter_site_id,firstletter.code as firstletter_code,firstletter.reject_message as firstletter_reject_message,firstletter.docname as firstletter_docname,firstletter.is_send_to_medico as firstletter_is_send_to_medico,firstletter.is_allowed_reclaim as firstletter_is_allowed_reclaim,firstletter.is_manual_override as firstletter_is_manual_override,firstletter.num_copies_to_print as firstletter_num_copies_to_print, firstletter.is_deleted as firstletter_is_deleted,

                    firstlettertype.descr as firstlettertype_descr, firstlettertype.letter_type_id as firstlettertype_letter_type_id,

                    firstletterorg.organisation_id as firstletterorg_organisation_id,firstletterorg.entity_id as firstletterorg_entity_id, firstletterorg.parent_organisation_id as firstletterorg_parent_organisation_id, firstletterorg.use_parent_offernig_prices as firstletterorg_use_parent_offernig_prices, 
                    firstletterorg.organisation_type_id as firstletterorg_organisation_type_id, firstletterorg.organisation_customer_type_id as firstletterorg_organisation_customer_type_id, firstletterorg.name as firstletterorg_name, firstletterorg.acn as firstletterorg_acn, firstletterorg.abn as firstletterorg_abn, firstletterorg.organisation_date_added as firstletterorg_organisation_date_added, firstletterorg.organisation_date_modified as firstletterorg_organisation_date_modified, firstletterorg.is_debtor as firstletterorg_is_debtor, firstletterorg.is_creditor as firstletterorg_is_creditor, firstletterorg.bpay_account as firstletterorg_bpay_account, 
                    firstletterorg.weeks_per_service_cycle as firstletterorg_weeks_per_service_cycle, firstletterorg.start_date as firstletterorg_start_date, firstletterorg.end_date as firstletterorg_end_date, firstletterorg.comment as firstletterorg_comment, firstletterorg.free_services as firstletterorg_free_services, firstletterorg.excl_sun as firstletterorg_excl_sun, firstletterorg.excl_mon as firstletterorg_excl_mon, firstletterorg.excl_tue as firstletterorg_excl_tue, firstletterorg.excl_wed as firstletterorg_excl_wed, firstletterorg.excl_thu as firstletterorg_excl_thu, firstletterorg.excl_fri as firstletterorg_excl_fri, firstletterorg.excl_sat as firstletterorg_excl_sat, 
                    firstletterorg.sun_start_time as firstletterorg_sun_start_time, firstletterorg.sun_end_time as firstletterorg_sun_end_time, firstletterorg.mon_start_time as firstletterorg_mon_start_time, firstletterorg.mon_end_time as firstletterorg_mon_end_time, firstletterorg.tue_start_time as firstletterorg_tue_start_time, firstletterorg.tue_end_time as firstletterorg_tue_end_time, firstletterorg.wed_start_time as firstletterorg_wed_start_time, firstletterorg.wed_end_time as firstletterorg_wed_end_time, 
                    firstletterorg.thu_start_time as firstletterorg_thu_start_time, firstletterorg.thu_end_time as firstletterorg_thu_end_time, firstletterorg.fri_start_time as firstletterorg_fri_start_time, firstletterorg.fri_end_time as firstletterorg_fri_end_time, firstletterorg.sat_start_time as firstletterorg_sat_start_time, firstletterorg.sat_end_time as firstletterorg_sat_end_time, 
                    firstletterorg.sun_lunch_start_time as firstletterorg_sun_lunch_start_time, firstletterorg.sun_lunch_end_time as firstletterorg_sun_lunch_end_time, firstletterorg.mon_lunch_start_time as firstletterorg_mon_lunch_start_time, firstletterorg.mon_lunch_end_time as firstletterorg_mon_lunch_end_time, firstletterorg.tue_lunch_start_time as firstletterorg_tue_lunch_start_time, firstletterorg.tue_lunch_end_time as firstletterorg_tue_lunch_end_time, firstletterorg.wed_lunch_start_time as firstletterorg_wed_lunch_start_time, firstletterorg.wed_lunch_end_time as firstletterorg_wed_lunch_end_time, 
                    firstletterorg.thu_lunch_start_time as firstletterorg_thu_lunch_start_time, firstletterorg.thu_lunch_end_time as firstletterorg_thu_lunch_end_time, firstletterorg.fri_lunch_start_time as firstletterorg_fri_lunch_start_time, firstletterorg.fri_lunch_end_time as firstletterorg_fri_lunch_end_time, firstletterorg.sat_lunch_start_time as firstletterorg_sat_lunch_start_time, firstletterorg.sat_lunch_end_time as firstletterorg_sat_lunch_end_time, 
                    firstletterorg.last_batch_run as firstletterorg_last_batch_run, firstletterorg.is_deleted as firstletterorg_is_deleted,

                    firstsite.site_id as firstsite_site_id,firstsite.entity_id as firstsite_entity_id,firstsite.name as firstsite_name,firstsite.site_type_id as firstsite_site_type_id,firstsite.abn as firstsite_abn,firstsite.acn as firstsite_acn,firstsite.tfn as firstsite_tfn,
                    firstsite.asic as firstsite_asic,firstsite.is_provider as firstsite_is_provider,firstsite.bank_bpay as firstsite_bank_bpay,firstsite.bank_bsb as firstsite_bank_bsb,firstsite.bank_account as firstsite_bank_account,
                    firstsite.bank_direct_debit_userid as firstsite_bank_direct_debit_userid,firstsite.bank_username as firstsite_bank_username,firstsite.oustanding_balance_warning as firstsite_oustanding_balance_warning,
                    firstsite.print_epc as firstsite_print_epc,firstsite.excl_sun as firstsite_excl_sun,firstsite.excl_mon as firstsite_excl_mon,firstsite.excl_tue as firstsite_excl_tue,firstsite.excl_wed as firstsite_excl_wed,firstsite.excl_thu as firstsite_excl_thu,
                    firstsite.excl_fri as firstsite_excl_fri,firstsite.excl_sat as firstsite_excl_sat,firstsite.day_start_time as firstsite_day_start_time,firstsite.lunch_start_time as firstsite_lunch_start_time,
                    firstsite.lunch_end_time as firstsite_lunch_end_time,firstsite.day_end_time as firstsite_day_end_time,firstsite.fiscal_yr_end as firstsite_fiscal_yr_end,firstsite.num_booking_months_to_get as firstsite_num_booking_months_to_get,



                    treatmentnotesletter.letter_id as treatmentnotesletter_letter_id,treatmentnotesletter.organisation_id as treatmentnotesletter_organisation_id,treatmentnotesletter.letter_type_id as treatmentnotesletter_letter_type_id,treatmentnotesletter.site_id as treatmentnotesletter_site_id,treatmentnotesletter.code as treatmentnotesletter_code,treatmentnotesletter.reject_message as treatmentnotesletter_reject_message,treatmentnotesletter.docname as treatmentnotesletter_docname,treatmentnotesletter.is_send_to_medico as treatmentnotesletter_is_send_to_medico,treatmentnotesletter.is_allowed_reclaim as treatmentnotesletter_is_allowed_reclaim,treatmentnotesletter.is_manual_override as treatmentnotesletter_is_manual_override,treatmentnotesletter.num_copies_to_print as treatmentnotesletter_num_copies_to_print, treatmentnotesletter.is_deleted as treatmentnotesletter_is_deleted,

                    treatmentnoteslettertype.descr as treatmentnoteslettertype_descr, treatmentnoteslettertype.letter_type_id as treatmentnoteslettertype_letter_type_id,

                    treatmentnotesletterorg.organisation_id as treatmentnotesletterorg_organisation_id,treatmentnotesletterorg.entity_id as treatmentnotesletterorg_entity_id, treatmentnotesletterorg.parent_organisation_id as treatmentnotesletterorg_parent_organisation_id, treatmentnotesletterorg.use_parent_offernig_prices as treatmentnotesletterorg_use_parent_offernig_prices, 
                    treatmentnotesletterorg.organisation_type_id as treatmentnotesletterorg_organisation_type_id, treatmentnotesletterorg.organisation_customer_type_id as treatmentnotesletterorg_organisation_customer_type_id, treatmentnotesletterorg.name as treatmentnotesletterorg_name, treatmentnotesletterorg.acn as treatmentnotesletterorg_acn, treatmentnotesletterorg.abn as treatmentnotesletterorg_abn, treatmentnotesletterorg.organisation_date_added as treatmentnotesletterorg_organisation_date_added, treatmentnotesletterorg.organisation_date_modified as treatmentnotesletterorg_organisation_date_modified, treatmentnotesletterorg.is_debtor as treatmentnotesletterorg_is_debtor, treatmentnotesletterorg.is_creditor as treatmentnotesletterorg_is_creditor, treatmentnotesletterorg.bpay_account as treatmentnotesletterorg_bpay_account, 
                    treatmentnotesletterorg.weeks_per_service_cycle as treatmentnotesletterorg_weeks_per_service_cycle, treatmentnotesletterorg.start_date as treatmentnotesletterorg_start_date, treatmentnotesletterorg.end_date as treatmentnotesletterorg_end_date, treatmentnotesletterorg.comment as treatmentnotesletterorg_comment, treatmentnotesletterorg.free_services as treatmentnotesletterorg_free_services, treatmentnotesletterorg.excl_sun as treatmentnotesletterorg_excl_sun, treatmentnotesletterorg.excl_mon as treatmentnotesletterorg_excl_mon, treatmentnotesletterorg.excl_tue as treatmentnotesletterorg_excl_tue, treatmentnotesletterorg.excl_wed as treatmentnotesletterorg_excl_wed, treatmentnotesletterorg.excl_thu as treatmentnotesletterorg_excl_thu, treatmentnotesletterorg.excl_fri as treatmentnotesletterorg_excl_fri, treatmentnotesletterorg.excl_sat as treatmentnotesletterorg_excl_sat, 
                    treatmentnotesletterorg.sun_start_time as treatmentnotesletterorg_sun_start_time, treatmentnotesletterorg.sun_end_time as treatmentnotesletterorg_sun_end_time, treatmentnotesletterorg.mon_start_time as treatmentnotesletterorg_mon_start_time, treatmentnotesletterorg.mon_end_time as treatmentnotesletterorg_mon_end_time, treatmentnotesletterorg.tue_start_time as treatmentnotesletterorg_tue_start_time, treatmentnotesletterorg.tue_end_time as treatmentnotesletterorg_tue_end_time, treatmentnotesletterorg.wed_start_time as treatmentnotesletterorg_wed_start_time, treatmentnotesletterorg.wed_end_time as treatmentnotesletterorg_wed_end_time, 
                    treatmentnotesletterorg.thu_start_time as treatmentnotesletterorg_thu_start_time, treatmentnotesletterorg.thu_end_time as treatmentnotesletterorg_thu_end_time, treatmentnotesletterorg.fri_start_time as treatmentnotesletterorg_fri_start_time, treatmentnotesletterorg.fri_end_time as treatmentnotesletterorg_fri_end_time, treatmentnotesletterorg.sat_start_time as treatmentnotesletterorg_sat_start_time, treatmentnotesletterorg.sat_end_time as treatmentnotesletterorg_sat_end_time, 
                    treatmentnotesletterorg.sun_lunch_start_time as treatmentnotesletterorg_sun_lunch_start_time, treatmentnotesletterorg.sun_lunch_end_time as treatmentnotesletterorg_sun_lunch_end_time, treatmentnotesletterorg.mon_lunch_start_time as treatmentnotesletterorg_mon_lunch_start_time, treatmentnotesletterorg.mon_lunch_end_time as treatmentnotesletterorg_mon_lunch_end_time, treatmentnotesletterorg.tue_lunch_start_time as treatmentnotesletterorg_tue_lunch_start_time, treatmentnotesletterorg.tue_lunch_end_time as treatmentnotesletterorg_tue_lunch_end_time, treatmentnotesletterorg.wed_lunch_start_time as treatmentnotesletterorg_wed_lunch_start_time, treatmentnotesletterorg.wed_lunch_end_time as treatmentnotesletterorg_wed_lunch_end_time, 
                    treatmentnotesletterorg.thu_lunch_start_time as treatmentnotesletterorg_thu_lunch_start_time, treatmentnotesletterorg.thu_lunch_end_time as treatmentnotesletterorg_thu_lunch_end_time, treatmentnotesletterorg.fri_lunch_start_time as treatmentnotesletterorg_fri_lunch_start_time, treatmentnotesletterorg.fri_lunch_end_time as treatmentnotesletterorg_fri_lunch_end_time, treatmentnotesletterorg.sat_lunch_start_time as treatmentnotesletterorg_sat_lunch_start_time, treatmentnotesletterorg.sat_lunch_end_time as treatmentnotesletterorg_sat_lunch_end_time, 
                    treatmentnotesletterorg.last_batch_run as treatmentnotesletterorg_last_batch_run, treatmentnotesletterorg.is_deleted as treatmentnotesletterorg_is_deleted,

                    treatmentnotessite.site_id as treatmentnotessite_site_id,treatmentnotessite.entity_id as treatmentnotessite_entity_id,treatmentnotessite.name as treatmentnotessite_name,treatmentnotessite.site_type_id as treatmentnotessite_site_type_id,treatmentnotessite.abn as treatmentnotessite_abn,treatmentnotessite.acn as treatmentnotessite_acn,treatmentnotessite.tfn as treatmentnotessite_tfn,
                    treatmentnotessite.asic as treatmentnotessite_asic,treatmentnotessite.is_provider as treatmentnotessite_is_provider,treatmentnotessite.bank_bpay as treatmentnotessite_bank_bpay,treatmentnotessite.bank_bsb as treatmentnotessite_bank_bsb,treatmentnotessite.bank_account as treatmentnotessite_bank_account,
                    treatmentnotessite.bank_direct_debit_userid as treatmentnotessite_bank_direct_debit_userid,treatmentnotessite.bank_username as treatmentnotessite_bank_username,treatmentnotessite.oustanding_balance_warning as treatmentnotessite_oustanding_balance_warning,
                    treatmentnotessite.print_epc as treatmentnotessite_print_epc,treatmentnotessite.excl_sun as treatmentnotessite_excl_sun,treatmentnotessite.excl_mon as treatmentnotessite_excl_mon,treatmentnotessite.excl_tue as treatmentnotessite_excl_tue,treatmentnotessite.excl_wed as treatmentnotessite_excl_wed,treatmentnotessite.excl_thu as treatmentnotessite_excl_thu,
                    treatmentnotessite.excl_fri as treatmentnotessite_excl_fri,treatmentnotessite.excl_sat as treatmentnotessite_excl_sat,treatmentnotessite.day_start_time as treatmentnotessite_day_start_time,treatmentnotessite.lunch_start_time as treatmentnotessite_lunch_start_time,
                    treatmentnotessite.lunch_end_time as treatmentnotessite_lunch_end_time,treatmentnotessite.day_end_time as treatmentnotessite_day_end_time,treatmentnotessite.fiscal_yr_end as treatmentnotessite_fiscal_yr_end,treatmentnotessite.num_booking_months_to_get as treatmentnotessite_num_booking_months_to_get,



                    lastletter.letter_id as lastletter_letter_id,lastletter.organisation_id as lastletter_organisation_id,lastletter.letter_type_id as lastletter_letter_type_id,lastletter.site_id as lastletter_site_id,lastletter.code as lastletter_code,lastletter.reject_message as lastletter_reject_message,lastletter.docname as lastletter_docname,lastletter.is_send_to_medico as lastletter_is_send_to_medico,lastletter.is_allowed_reclaim as lastletter_is_allowed_reclaim,lastletter.is_manual_override as lastletter_is_manual_override,lastletter.num_copies_to_print as lastletter_num_copies_to_print, lastletter.is_deleted as lastletter_is_deleted,

                    lastlettertype.descr as lastlettertype_descr, lastlettertype.letter_type_id as lastlettertype_letter_type_id,

                    lastletterorg.organisation_id as lastletterorg_organisation_id,lastletterorg.entity_id as lastletterorg_entity_id, lastletterorg.parent_organisation_id as lastletterorg_parent_organisation_id, lastletterorg.use_parent_offernig_prices as lastletterorg_use_parent_offernig_prices, 
                    lastletterorg.organisation_type_id as lastletterorg_organisation_type_id, lastletterorg.organisation_customer_type_id as lastletterorg_organisation_customer_type_id, lastletterorg.name as lastletterorg_name, lastletterorg.acn as lastletterorg_acn, lastletterorg.abn as lastletterorg_abn, lastletterorg.organisation_date_added as lastletterorg_organisation_date_added, lastletterorg.organisation_date_modified as lastletterorg_organisation_date_modified, lastletterorg.is_debtor as lastletterorg_is_debtor, lastletterorg.is_creditor as lastletterorg_is_creditor, lastletterorg.bpay_account as lastletterorg_bpay_account, 
                    lastletterorg.weeks_per_service_cycle as lastletterorg_weeks_per_service_cycle, lastletterorg.start_date as lastletterorg_start_date, lastletterorg.end_date as lastletterorg_end_date, lastletterorg.comment as lastletterorg_comment, lastletterorg.free_services as lastletterorg_free_services, lastletterorg.excl_sun as lastletterorg_excl_sun, lastletterorg.excl_mon as lastletterorg_excl_mon, lastletterorg.excl_tue as lastletterorg_excl_tue, lastletterorg.excl_wed as lastletterorg_excl_wed, lastletterorg.excl_thu as lastletterorg_excl_thu, lastletterorg.excl_fri as lastletterorg_excl_fri, 
                    lastletterorg.excl_sat as lastletterorg_excl_sat, 
                    lastletterorg.sun_start_time as lastletterorg_sun_start_time, lastletterorg.sun_end_time as lastletterorg_sun_end_time, lastletterorg.mon_start_time as lastletterorg_mon_start_time, lastletterorg.mon_end_time as lastletterorg_mon_end_time, lastletterorg.tue_start_time as lastletterorg_tue_start_time, lastletterorg.tue_end_time as lastletterorg_tue_end_time, lastletterorg.wed_start_time as lastletterorg_wed_start_time, lastletterorg.wed_end_time as lastletterorg_wed_end_time, 
                    lastletterorg.thu_start_time as lastletterorg_thu_start_time, lastletterorg.thu_end_time as lastletterorg_thu_end_time, lastletterorg.fri_start_time as lastletterorg_fri_start_time, lastletterorg.fri_end_time as lastletterorg_fri_end_time, lastletterorg.sat_start_time as lastletterorg_sat_start_time, lastletterorg.sat_end_time as lastletterorg_sat_end_time, 
                    lastletterorg.sun_lunch_start_time as lastletterorg_sun_lunch_start_time, lastletterorg.sun_lunch_end_time as lastletterorg_sun_lunch_end_time, lastletterorg.mon_lunch_start_time as lastletterorg_mon_lunch_start_time, lastletterorg.mon_lunch_end_time as lastletterorg_mon_lunch_end_time, lastletterorg.tue_lunch_start_time as lastletterorg_tue_lunch_start_time, lastletterorg.tue_lunch_end_time as lastletterorg_tue_lunch_end_time, lastletterorg.wed_lunch_start_time as lastletterorg_wed_lunch_start_time, lastletterorg.wed_lunch_end_time as lastletterorg_wed_lunch_end_time, 
                    lastletterorg.thu_lunch_start_time as lastletterorg_thu_lunch_start_time, lastletterorg.thu_lunch_end_time as lastletterorg_thu_lunch_end_time, lastletterorg.fri_lunch_start_time as lastletterorg_fri_lunch_start_time, lastletterorg.fri_lunch_end_time as lastletterorg_fri_lunch_end_time, lastletterorg.sat_lunch_start_time as lastletterorg_sat_lunch_start_time, lastletterorg.sat_lunch_end_time as lastletterorg_sat_lunch_end_time, 
                    lastletterorg.last_batch_run as lastletterorg_last_batch_run, lastletterorg.is_deleted as lastletterorg_is_deleted,

                    lastsite.site_id as lastsite_site_id,lastsite.entity_id as lastsite_entity_id,lastsite.name as lastsite_name,lastsite.site_type_id as lastsite_site_type_id,lastsite.abn as lastsite_abn,lastsite.acn as lastsite_acn,lastsite.tfn as lastsite_tfn,
                    lastsite.asic as lastsite_asic,lastsite.is_provider as lastsite_is_provider,lastsite.bank_bpay as lastsite_bank_bpay,lastsite.bank_bsb as lastsite_bank_bsb,lastsite.bank_account as lastsite_bank_account,
                    lastsite.bank_direct_debit_userid as lastsite_bank_direct_debit_userid,lastsite.bank_username as lastsite_bank_username,lastsite.oustanding_balance_warning as lastsite_oustanding_balance_warning,
                    lastsite.print_epc as lastsite_print_epc,lastsite.excl_sun as lastsite_excl_sun,lastsite.excl_mon as lastsite_excl_mon,lastsite.excl_tue as lastsite_excl_tue,lastsite.excl_wed as lastsite_excl_wed,lastsite.excl_thu as lastsite_excl_thu,
                    lastsite.excl_fri as lastsite_excl_fri,lastsite.excl_sat as lastsite_excl_sat,lastsite.day_start_time as lastsite_day_start_time,lastsite.lunch_start_time as lastsite_lunch_start_time,
                    lastsite.lunch_end_time as lastsite_lunch_end_time,lastsite.day_end_time as lastsite_day_end_time,lastsite.fiscal_yr_end as lastsite_fiscal_yr_end,lastsite.num_booking_months_to_get as lastsite_num_booking_months_to_get,



                    lastletterpt.letter_id as lastletterpt_letter_id,lastletterpt.organisation_id as lastletterpt_organisation_id,lastletterpt.letter_type_id as lastletterpt_letter_type_id,lastletterpt.site_id as lastletterpt_site_id,lastletterpt.code as lastletterpt_code,lastletterpt.reject_message as lastletterpt_reject_message,lastletterpt.docname as lastletterpt_docname,lastletterpt.is_send_to_medico as lastletterpt_is_send_to_medico,lastletterpt.is_allowed_reclaim as lastletterpt_is_allowed_reclaim,lastletterpt.is_manual_override as lastletterpt_is_manual_override,lastletterpt.num_copies_to_print as lastletterpt_num_copies_to_print, lastletterpt.is_deleted as lastletterpt_is_deleted,

                    lastlettertypept.descr as lastlettertypept_descr, lastlettertypept.letter_type_id as lastlettertypept_letter_type_id,

                    lastletterorgpt.organisation_id as lastletterorgpt_organisation_id,lastletterorgpt.entity_id as lastletterorgpt_entity_id, lastletterorgpt.parent_organisation_id as lastletterorgpt_parent_organisation_id, lastletterorgpt.use_parent_offernig_prices as lastletterorgpt_use_parent_offernig_prices, 
                    lastletterorgpt.organisation_type_id as lastletterorgpt_organisation_type_id, lastletterorgpt.organisation_customer_type_id as lastletterorgpt_organisation_customer_type_id, lastletterorgpt.name as lastletterorgpt_name, lastletterorgpt.acn as lastletterorgpt_acn, lastletterorgpt.abn as lastletterorgpt_abn, lastletterorgpt.organisation_date_added as lastletterorgpt_organisation_date_added, lastletterorgpt.organisation_date_modified as lastletterorgpt_organisation_date_modified, lastletterorgpt.is_debtor as lastletterorgpt_is_debtor, lastletterorgpt.is_creditor as lastletterorgpt_is_creditor, lastletterorgpt.bpay_account as lastletterorgpt_bpay_account, 
                    lastletterorgpt.weeks_per_service_cycle as lastletterorgpt_weeks_per_service_cycle, lastletterorgpt.start_date as lastletterorgpt_start_date, lastletterorgpt.end_date as lastletterorgpt_end_date, lastletterorgpt.comment as lastletterorgpt_comment, lastletterorgpt.free_services as lastletterorgpt_free_services, lastletterorgpt.excl_sun as lastletterorgpt_excl_sun, lastletterorgpt.excl_mon as lastletterorgpt_excl_mon, lastletterorgpt.excl_tue as lastletterorgpt_excl_tue, lastletterorgpt.excl_wed as lastletterorgpt_excl_wed, lastletterorgpt.excl_thu as lastletterorgpt_excl_thu, lastletterorgpt.excl_fri as lastletterorgpt_excl_fri, 
                    lastletterorgpt.excl_sat as lastletterorgpt_excl_sat, 
                    lastletterorgpt.sun_start_time as lastletterorgpt_sun_start_time, lastletterorgpt.sun_end_time as lastletterorgpt_sun_end_time, lastletterorgpt.mon_start_time as lastletterorgpt_mon_start_time, lastletterorgpt.mon_end_time as lastletterorgpt_mon_end_time, lastletterorgpt.tue_start_time as lastletterorgpt_tue_start_time, lastletterorgpt.tue_end_time as lastletterorgpt_tue_end_time, lastletterorgpt.wed_start_time as lastletterorgpt_wed_start_time, lastletterorgpt.wed_end_time as lastletterorgpt_wed_end_time, 
                    lastletterorgpt.thu_start_time as lastletterorgpt_thu_start_time, lastletterorgpt.thu_end_time as lastletterorgpt_thu_end_time, lastletterorgpt.fri_start_time as lastletterorgpt_fri_start_time, lastletterorgpt.fri_end_time as lastletterorgpt_fri_end_time, lastletterorgpt.sat_start_time as lastletterorgpt_sat_start_time, lastletterorgpt.sat_end_time as lastletterorgpt_sat_end_time, 
                    lastletterorgpt.sun_lunch_start_time as lastletterorgpt_sun_lunch_start_time, lastletterorgpt.sun_lunch_end_time as lastletterorgpt_sun_lunch_end_time, lastletterorgpt.mon_lunch_start_time as lastletterorgpt_mon_lunch_start_time, lastletterorgpt.mon_lunch_end_time as lastletterorgpt_mon_lunch_end_time, lastletterorgpt.tue_lunch_start_time as lastletterorgpt_tue_lunch_start_time, lastletterorgpt.tue_lunch_end_time as lastletterorgpt_tue_lunch_end_time, lastletterorgpt.wed_lunch_start_time as lastletterorgpt_wed_lunch_start_time, lastletterorgpt.wed_lunch_end_time as lastletterorgpt_wed_lunch_end_time, 
                    lastletterorgpt.thu_lunch_start_time as lastletterorgpt_thu_lunch_start_time, lastletterorgpt.thu_lunch_end_time as lastletterorgpt_thu_lunch_end_time, lastletterorgpt.fri_lunch_start_time as lastletterorgpt_fri_lunch_start_time, lastletterorgpt.fri_lunch_end_time as lastletterorgpt_fri_lunch_end_time, lastletterorgpt.sat_lunch_start_time as lastletterorgpt_sat_lunch_start_time, lastletterorgpt.sat_lunch_end_time as lastletterorgpt_sat_lunch_end_time, 
                    lastletterorgpt.last_batch_run as lastletterorgpt_last_batch_run, lastletterorgpt.is_deleted as lastletterorgpt_is_deleted,

                    lastsitept.site_id as lastsitept_site_id,lastsitept.entity_id as lastsitept_entity_id,lastsitept.name as lastsitept_name,lastsitept.site_type_id as lastsitept_site_type_id,lastsitept.abn as lastsitept_abn,lastsitept.acn as lastsitept_acn,lastsitept.tfn as lastsitept_tfn,
                    lastsitept.asic as lastsitept_asic,lastsitept.is_provider as lastsitept_is_provider,lastsitept.bank_bpay as lastsitept_bank_bpay,lastsitept.bank_bsb as lastsitept_bank_bsb,lastsitept.bank_account as lastsitept_bank_account,
                    lastsitept.bank_direct_debit_userid as lastsitept_bank_direct_debit_userid,lastsitept.bank_username as lastsitept_bank_username,lastsitept.oustanding_balance_warning as lastsitept_oustanding_balance_warning,
                    lastsitept.print_epc as lastsitept_print_epc,lastsitept.excl_sun as lastsitept_excl_sun,lastsitept.excl_mon as lastsitept_excl_mon,lastsitept.excl_tue as lastsitept_excl_tue,lastsitept.excl_wed as lastsitept_excl_wed,lastsitept.excl_thu as lastsitept_excl_thu,
                    lastsitept.excl_fri as lastsitept_excl_fri,lastsitept.excl_sat as lastsitept_excl_sat,lastsitept.day_start_time as lastsitept_day_start_time,lastsitept.lunch_start_time as lastsitept_lunch_start_time,
                    lastsitept.lunch_end_time as lastsitept_lunch_end_time,lastsitept.day_end_time as lastsitept_day_end_time,lastsitept.fiscal_yr_end as lastsitept_fiscal_yr_end,lastsitept.num_booking_months_to_get as lastsitept_num_booking_months_to_get,



                    lastletterwhenreplacingepc.letter_id as lastletterwhenreplacingepc_letter_id,lastletterwhenreplacingepc.organisation_id as lastletterwhenreplacingepc_organisation_id,lastletterwhenreplacingepc.letter_type_id as lastletterwhenreplacingepc_letter_type_id,lastletterwhenreplacingepc.site_id as lastletterwhenreplacingepc_site_id,lastletterwhenreplacingepc.code as lastletterwhenreplacingepc_code,lastletterwhenreplacingepc.reject_message as lastletterwhenreplacingepc_reject_message,lastletterwhenreplacingepc.docname as lastletterwhenreplacingepc_docname,lastletterwhenreplacingepc.is_send_to_medico as lastletterwhenreplacingepc_is_send_to_medico,lastletterwhenreplacingepc.is_allowed_reclaim as lastletterwhenreplacingepc_is_allowed_reclaim,lastletterwhenreplacingepc.is_manual_override as lastletterwhenreplacingepc_is_manual_override,lastletterwhenreplacingepc.num_copies_to_print as lastletterwhenreplacingepc_num_copies_to_print, lastletterwhenreplacingepc.is_deleted as lastletterwhenreplacingepc_is_deleted,

                    lastletterwhenreplacingepctype.descr as lastletterwhenreplacingepctype_descr, lastletterwhenreplacingepctype.letter_type_id as lastletterwhenreplacingepctype_letter_type_id,

                    lastletterwhenreplacingepcorg.organisation_id as lastletterwhenreplacingepcorg_organisation_id,lastletterwhenreplacingepcorg.entity_id as lastletterwhenreplacingepcorg_entity_id, lastletterwhenreplacingepcorg.parent_organisation_id as lastletterwhenreplacingepcorg_parent_organisation_id, lastletterwhenreplacingepcorg.use_parent_offernig_prices as lastletterwhenreplacingepcorg_use_parent_offernig_prices, 
                    lastletterwhenreplacingepcorg.organisation_type_id as lastletterwhenreplacingepcorg_organisation_type_id, lastletterwhenreplacingepcorg.organisation_customer_type_id as lastletterwhenreplacingepcorg_organisation_customer_type_id, lastletterwhenreplacingepcorg.name as lastletterwhenreplacingepcorg_name, lastletterwhenreplacingepcorg.acn as lastletterwhenreplacingepcorg_acn, lastletterwhenreplacingepcorg.abn as lastletterwhenreplacingepcorg_abn, lastletterwhenreplacingepcorg.organisation_date_added as lastletterwhenreplacingepcorg_organisation_date_added, lastletterwhenreplacingepcorg.organisation_date_modified as lastletterwhenreplacingepcorg_organisation_date_modified, lastletterwhenreplacingepcorg.is_debtor as lastletterwhenreplacingepcorg_is_debtor, lastletterwhenreplacingepcorg.is_creditor as lastletterwhenreplacingepcorg_is_creditor, lastletterwhenreplacingepcorg.bpay_account as lastletterwhenreplacingepcorg_bpay_account, 
                    lastletterwhenreplacingepcorg.weeks_per_service_cycle as lastletterwhenreplacingepcorg_weeks_per_service_cycle, lastletterwhenreplacingepcorg.start_date as lastletterwhenreplacingepcorg_start_date, lastletterwhenreplacingepcorg.end_date as lastletterwhenreplacingepcorg_end_date, lastletterwhenreplacingepcorg.comment as lastletterwhenreplacingepcorg_comment, lastletterwhenreplacingepcorg.free_services as lastletterwhenreplacingepcorg_free_services, lastletterwhenreplacingepcorg.excl_sun as lastletterwhenreplacingepcorg_excl_sun, lastletterwhenreplacingepcorg.excl_mon as lastletterwhenreplacingepcorg_excl_mon, lastletterwhenreplacingepcorg.excl_tue as lastletterwhenreplacingepcorg_excl_tue, lastletterwhenreplacingepcorg.excl_wed as lastletterwhenreplacingepcorg_excl_wed, lastletterwhenreplacingepcorg.excl_thu as lastletterwhenreplacingepcorg_excl_thu, lastletterwhenreplacingepcorg.excl_fri as lastletterwhenreplacingepcorg_excl_fri, 
                    lastletterwhenreplacingepcorg.excl_sat as lastletterwhenreplacingepcorg_excl_sat, 
                    lastletterwhenreplacingepcorg.sun_start_time as lastletterwhenreplacingepcorg_sun_start_time, lastletterwhenreplacingepcorg.sun_end_time as lastletterwhenreplacingepcorg_sun_end_time, lastletterwhenreplacingepcorg.mon_start_time as lastletterwhenreplacingepcorg_mon_start_time, lastletterwhenreplacingepcorg.mon_end_time as lastletterwhenreplacingepcorg_mon_end_time, lastletterwhenreplacingepcorg.tue_start_time as lastletterwhenreplacingepcorg_tue_start_time, lastletterwhenreplacingepcorg.tue_end_time as lastletterwhenreplacingepcorg_tue_end_time, lastletterwhenreplacingepcorg.wed_start_time as lastletterwhenreplacingepcorg_wed_start_time, lastletterwhenreplacingepcorg.wed_end_time as lastletterwhenreplacingepcorg_wed_end_time, 
                    lastletterwhenreplacingepcorg.thu_start_time as lastletterwhenreplacingepcorg_thu_start_time, lastletterwhenreplacingepcorg.thu_end_time as lastletterwhenreplacingepcorg_thu_end_time, lastletterwhenreplacingepcorg.fri_start_time as lastletterwhenreplacingepcorg_fri_start_time, lastletterwhenreplacingepcorg.fri_end_time as lastletterwhenreplacingepcorg_fri_end_time, lastletterwhenreplacingepcorg.sat_start_time as lastletterwhenreplacingepcorg_sat_start_time, lastletterwhenreplacingepcorg.sat_end_time as lastletterwhenreplacingepcorg_sat_end_time, 
                    lastletterwhenreplacingepcorg.sun_lunch_start_time as lastletterwhenreplacingepcorg_sun_lunch_start_time, lastletterwhenreplacingepcorg.sun_lunch_end_time as lastletterwhenreplacingepcorg_sun_lunch_end_time, lastletterwhenreplacingepcorg.mon_lunch_start_time as lastletterwhenreplacingepcorg_mon_lunch_start_time, lastletterwhenreplacingepcorg.mon_lunch_end_time as lastletterwhenreplacingepcorg_mon_lunch_end_time, lastletterwhenreplacingepcorg.tue_lunch_start_time as lastletterwhenreplacingepcorg_tue_lunch_start_time, lastletterwhenreplacingepcorg.tue_lunch_end_time as lastletterwhenreplacingepcorg_tue_lunch_end_time, lastletterwhenreplacingepcorg.wed_lunch_start_time as lastletterwhenreplacingepcorg_wed_lunch_start_time, lastletterwhenreplacingepcorg.wed_lunch_end_time as lastletterwhenreplacingepcorg_wed_lunch_end_time, 
                    lastletterwhenreplacingepcorg.thu_lunch_start_time as lastletterwhenreplacingepcorg_thu_lunch_start_time, lastletterwhenreplacingepcorg.thu_lunch_end_time as lastletterwhenreplacingepcorg_thu_lunch_end_time, lastletterwhenreplacingepcorg.fri_lunch_start_time as lastletterwhenreplacingepcorg_fri_lunch_start_time, lastletterwhenreplacingepcorg.fri_lunch_end_time as lastletterwhenreplacingepcorg_fri_lunch_end_time, lastletterwhenreplacingepcorg.sat_lunch_start_time as lastletterwhenreplacingepcorg_sat_lunch_start_time, lastletterwhenreplacingepcorg.sat_lunch_end_time as lastletterwhenreplacingepcorg_sat_lunch_end_time, 
                    lastletterwhenreplacingepcorg.last_batch_run as lastletterwhenreplacingepcorg_last_batch_run, lastletterwhenreplacingepcorg.is_deleted as lastletterwhenreplacingepcorg_is_deleted,

                    lastwhenreplacingepcsite.site_id as lastwhenreplacingepcsite_site_id,lastwhenreplacingepcsite.entity_id as lastwhenreplacingepcsite_entity_id,lastwhenreplacingepcsite.name as lastwhenreplacingepcsite_name,lastwhenreplacingepcsite.site_type_id as lastwhenreplacingepcsite_site_type_id,lastwhenreplacingepcsite.abn as lastwhenreplacingepcsite_abn,lastwhenreplacingepcsite.acn as lastwhenreplacingepcsite_acn,lastwhenreplacingepcsite.tfn as lastwhenreplacingepcsite_tfn,
                    lastwhenreplacingepcsite.asic as lastwhenreplacingepcsite_asic,lastwhenreplacingepcsite.is_provider as lastwhenreplacingepcsite_is_provider,lastwhenreplacingepcsite.bank_bpay as lastwhenreplacingepcsite_bank_bpay,lastwhenreplacingepcsite.bank_bsb as lastwhenreplacingepcsite_bank_bsb,lastwhenreplacingepcsite.bank_account as lastwhenreplacingepcsite_bank_account,
                    lastwhenreplacingepcsite.bank_direct_debit_userid as lastwhenreplacingepcsite_bank_direct_debit_userid,lastwhenreplacingepcsite.bank_username as lastwhenreplacingepcsite_bank_username,lastwhenreplacingepcsite.oustanding_balance_warning as lastwhenreplacingepcsite_oustanding_balance_warning,
                    lastwhenreplacingepcsite.print_epc as lastwhenreplacingepcsite_print_epc,lastwhenreplacingepcsite.excl_sun as lastwhenreplacingepcsite_excl_sun,lastwhenreplacingepcsite.excl_mon as lastwhenreplacingepcsite_excl_mon,lastwhenreplacingepcsite.excl_tue as lastwhenreplacingepcsite_excl_tue,lastwhenreplacingepcsite.excl_wed as lastwhenreplacingepcsite_excl_wed,lastwhenreplacingepcsite.excl_thu as lastwhenreplacingepcsite_excl_thu,
                    lastwhenreplacingepcsite.excl_fri as lastwhenreplacingepcsite_excl_fri,lastwhenreplacingepcsite.excl_sat as lastwhenreplacingepcsite_excl_sat,lastwhenreplacingepcsite.day_start_time as lastwhenreplacingepcsite_day_start_time,lastwhenreplacingepcsite.lunch_start_time as lastwhenreplacingepcsite_lunch_start_time,
                    lastwhenreplacingepcsite.lunch_end_time as lastwhenreplacingepcsite_lunch_end_time,lastwhenreplacingepcsite.day_end_time as lastwhenreplacingepcsite_day_end_time,lastwhenreplacingepcsite.fiscal_yr_end as lastwhenreplacingepcsite_fiscal_yr_end,lastwhenreplacingepcsite.num_booking_months_to_get as lastwhenreplacingepcsite_num_booking_months_to_get,


                    site.site_id as site_site_id,site.entity_id as site_entity_id,site.name as site_name,site.site_type_id as site_site_type_id,site.abn as site_abn,site.acn as site_acn,site.tfn as site_tfn,
                    site.asic as site_asic,site.is_provider as site_is_provider,site.bank_bpay as site_bank_bpay,site.bank_bsb as site_bank_bsb,site.bank_account as site_bank_account,
                    site.bank_direct_debit_userid as site_bank_direct_debit_userid,site.bank_username as site_bank_username,site.oustanding_balance_warning as site_oustanding_balance_warning,
                    site.print_epc as site_print_epc,site.excl_sun as site_excl_sun,site.excl_mon as site_excl_mon,site.excl_tue as site_excl_tue,site.excl_wed as site_excl_wed,site.excl_thu as site_excl_thu,
                    site.excl_fri as site_excl_fri,site.excl_sat as site_excl_sat,site.day_start_time as site_day_start_time,site.lunch_start_time as site_lunch_start_time,
                    site.lunch_end_time as site_lunch_end_time,site.day_end_time as site_day_end_time,site.fiscal_yr_end as site_fiscal_yr_end,site.num_booking_months_to_get as site_num_booking_months_to_get


            FROM

                    LetterTreatmentTemplate       lettertreatmenttemplate
                    INNER JOIN Field              field                          ON field.field_id                                              = lettertreatmenttemplate.field_id

                    INNER JOIN Letter             firstletter                    ON lettertreatmenttemplate.first_letter_id                     = firstletter.letter_id
                    INNER JOIN LetterType         firstlettertype                ON firstletter.letter_type_id                                  = firstlettertype.letter_type_id 
                    INNER JOIN Site               firstsite                      ON firstsite.site_id                                           = firstletter.site_id
                    LEFT OUTER JOIN Organisation  firstletterorg                 ON firstletterorg.organisation_id                              = firstletter.organisation_id

                    INNER JOIN Letter             treatmentnotesletter           ON lettertreatmenttemplate.treatment_notes_letter_id           = treatmentnotesletter.letter_id
                    INNER JOIN LetterType         treatmentnoteslettertype       ON treatmentnotesletter.letter_type_id                         = treatmentnoteslettertype.letter_type_id 
                    INNER JOIN Site               treatmentnotessite             ON treatmentnotessite.site_id                                  = treatmentnotesletter.site_id
                    LEFT OUTER JOIN Organisation  treatmentnotesletterorg        ON treatmentnotesletterorg.organisation_id                     = treatmentnotesletter.organisation_id 

                    INNER JOIN Letter             lastletter                     ON lettertreatmenttemplate.last_letter_id                      = lastletter.letter_id
                    INNER JOIN LetterType         lastlettertype                 ON lastletter.letter_type_id                                   = lastlettertype.letter_type_id 
                    INNER JOIN Site               lastsite                       ON lastsite.site_id                                            = lastletter.site_id
                    LEFT OUTER JOIN Organisation  lastletterorg                  ON lastletterorg.organisation_id                               = lastletter.organisation_id

                    INNER JOIN Letter             lastletterpt                   ON lettertreatmenttemplate.last_letter_pt_id                   = lastletterpt.letter_id
                    INNER JOIN LetterType         lastlettertypept               ON lastletterpt.letter_type_id                                 = lastlettertypept.letter_type_id 
                    INNER JOIN Site               lastsitept                     ON lastsitept.site_id                                          = lastletterpt.site_id
                    LEFT OUTER JOIN Organisation  lastletterorgpt                ON lastletterorgpt.organisation_id                             = lastletterpt.organisation_id

                    INNER JOIN Letter             lastletterwhenreplacingepc     ON lettertreatmenttemplate.last_letter_when_replacing_epc_id   = lastletterwhenreplacingepc.letter_id
                    INNER JOIN LetterType         lastletterwhenreplacingepctype ON lastletterwhenreplacingepc.letter_type_id                   = lastletterwhenreplacingepctype.letter_type_id 
                    INNER JOIN Site               lastwhenreplacingepcsite       ON lastwhenreplacingepcsite.site_id                            = lastletterwhenreplacingepc.site_id
                    LEFT OUTER JOIN Organisation  lastletterwhenreplacingepcorg  ON lastletterwhenreplacingepcorg.organisation_id               = lastletterwhenreplacingepc.organisation_id 

                    INNER JOIN Site               site                           ON site.site_id                                                = lettertreatmenttemplate.site_id ";



    public static LetterTreatmentTemplate LoadAll(DataRow row)
    {
        LetterTreatmentTemplate letters = Load(row, "lettertreatmenttemplate_");

        letters.Field = IDandDescrDB.Load(row, "field_field_id", "field_descr");

        letters.FirstLetter = LetterDB.Load(row, "firstletter_");
        letters.FirstLetter.LetterType = IDandDescrDB.Load(row, "firstlettertype_letter_type_id", "firstlettertype_descr");
        letters.FirstLetter.Site = SiteDB.Load(row, "firstsite_");
        if (row["firstletterorg_organisation_id"] != DBNull.Value)
            letters.FirstLetter.Organisation = OrganisationDB.Load(row, "firstletterorg_");

        letters.TreatmentNotesLetter = LetterDB.Load(row, "treatmentnotesletter_");
        letters.TreatmentNotesLetter.LetterType = IDandDescrDB.Load(row, "treatmentnoteslettertype_letter_type_id", "treatmentnoteslettertype_descr");
        letters.TreatmentNotesLetter.Site = SiteDB.Load(row, "treatmentnotessite_");
        if (row["treatmentnotesletterorg_organisation_id"] != DBNull.Value)
            letters.TreatmentNotesLetter.Organisation = OrganisationDB.Load(row, "treatmentnotesletterorg_");

        letters.LastLetter = LetterDB.Load(row, "lastletter_");
        letters.LastLetter.LetterType = IDandDescrDB.Load(row, "lastlettertype_letter_type_id", "lastlettertype_descr");
        letters.LastLetter.Site = SiteDB.Load(row, "lastsite_");
        if (row["lastletterorg_organisation_id"] != DBNull.Value)
            letters.LastLetter.Organisation = OrganisationDB.Load(row, "lastletterorg_");

        letters.LastLetterPT = LetterDB.Load(row, "lastletterpt_");
        letters.LastLetterPT.LetterType = IDandDescrDB.Load(row, "lastlettertypept_letter_type_id", "lastlettertypept_descr");
        letters.LastLetterPT.Site = SiteDB.Load(row, "lastsitept_");
        if (row["lastletterorgpt_organisation_id"] != DBNull.Value)
            letters.LastLetterPT.Organisation = OrganisationDB.Load(row, "lastletterorgpt_");

        letters.LastLetterWhenReplacingEPC = LetterDB.Load(row, "lastletterwhenreplacingepc_");
        letters.LastLetterWhenReplacingEPC.LetterType = IDandDescrDB.Load(row, "lastletterwhenreplacingepctype_letter_type_id", "lastletterwhenreplacingepctype_descr");
        letters.LastLetterWhenReplacingEPC.Site = SiteDB.Load(row, "lastwhenreplacingepcsite_");
        if (row["lastletterorg_organisation_id"] != DBNull.Value)
            letters.LastLetterWhenReplacingEPC.Organisation = OrganisationDB.Load(row, "lastletterwhenreplacingepcorg_");

        letters.Site = SiteDB.Load(row, "site_");

        return letters;
    }


    public static LetterTreatmentTemplate Load(DataRow row, string prefix="")
    {
        return new LetterTreatmentTemplate(
            Convert.ToInt32(row[prefix + "letter_treatment_template_id"]),
            Convert.ToInt32(row[prefix + "field_id"]),
            Convert.ToInt32(row[prefix + "first_letter_id"]),
            Convert.ToInt32(row[prefix + "last_letter_pt_id"]),
            Convert.ToInt32(row[prefix + "last_letter_id"]),
            Convert.ToInt32(row[prefix + "last_letter_when_replacing_epc_id"]),
            Convert.ToInt32(row[prefix + "treatment_notes_letter_id"]),
            Convert.ToInt32(row[prefix + "site_id"])
        );
    }

}