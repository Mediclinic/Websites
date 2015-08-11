using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class OfferingOrderDB
{

    public static void Delete(int offering_order_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM OfferingOrder WHERE offering_order_id = " + offering_order_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static int Insert(int offering_id, int organisation_id, int staff_id, int patient_id, int quantity, DateTime date_ordered, DateTime date_filled, DateTime date_cancelled, string descr)
    {
        descr = descr.Replace("'", "''");
        string sql = "INSERT INTO OfferingOrder (offering_id,organisation_id,staff_id,patient_id,quantity,date_ordered,date_filled,date_cancelled,descr) VALUES (" + "" + (offering_id == -1 ? "NULL" : offering_id.ToString()) + "," + "" + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + "," + "" + (staff_id == -1 ? "NULL" : staff_id.ToString()) + "," + (patient_id == -1 ? "NULL" : patient_id.ToString()) + "," + quantity + "," + (date_ordered == DateTime.MinValue ? "NULL" : "'" + date_ordered.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (date_filled == DateTime.MinValue ? "NULL" : "'" + date_filled.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (date_cancelled == DateTime.MinValue ? "NULL" : "'" + date_cancelled.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + "'" + descr + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int offering_order_id, int offering_id, int organisation_id, int staff_id, int patient_id, int quantity, DateTime date_ordered, DateTime date_filled, DateTime date_cancelled, string descr)
    {
        descr = descr.Replace("'", "''");
        string sql = "UPDATE OfferingOrder SET offering_id = " + (offering_id == -1 ? "NULL" : offering_id.ToString()) + ",organisation_id = " + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + ",staff_id = " + (staff_id == -1 ? "NULL" : staff_id.ToString()) + ",patient_id = " + (patient_id == -1 ? "NULL" : patient_id.ToString()) + ",quantity = " + quantity + ",date_ordered = " + (date_ordered == DateTime.MinValue ? "NULL" : "'" + date_ordered.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",date_filled = " + (date_filled == DateTime.MinValue ? "NULL" : "'" + date_filled.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",date_cancelled = " + (date_cancelled == DateTime.MinValue ? "NULL" : "'" + date_cancelled.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",descr = '" + descr + "' WHERE offering_order_id = " + offering_order_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable(DateTime startOrderedDate, DateTime endOrderedDate, int offering_id = -1, int organisation_id = 0, int patient_id = -1, int staff_id = -1, bool is_filled_or_cancelled = false, bool is_not_filled_or_cancelled = false)
    {
        string whereClause = string.Empty;

        if (offering_id != -1)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " offeringorder.offering_id = " + offering_id;
        if (organisation_id != 0)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " offeringorder.organisation_id = " + organisation_id;
        if (patient_id != -1)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " offeringorder.patient_id = " + patient_id;
        if (staff_id != -1)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " offeringorder.staff_id = " + staff_id;
        if (is_filled_or_cancelled)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " (offeringorder.date_filled IS NOT NULL OR offeringorder.date_cancelled IS NOT NULL)";
        if (is_not_filled_or_cancelled)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " offeringorder.date_filled IS NULL AND offeringorder.date_cancelled IS NULL";

        if (startOrderedDate != DateTime.MinValue)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " offeringorder.date_ordered >= '" + startOrderedDate.AddDays(-1).ToString("yyyy-MM-dd" + " 23:59:59") + "'";
        if (endOrderedDate != DateTime.MinValue)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " offeringorder.date_ordered <= '" + endOrderedDate.ToString("yyyy-MM-dd") + " 23:59:59" + "'";

        string sql = JoinedSql + whereClause;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static OfferingOrder[] GetAll(DateTime startOrderedDate, DateTime endOrderedDate, int offering_id = -1, int organisation_id = 0, int patient_id = -1, int staff_id = -1, bool is_filled_or_cancelled = false, bool is_not_filled_or_cancelled = false)
    {
        DataTable tbl = GetDataTable(startOrderedDate, endOrderedDate, offering_id, organisation_id, patient_id, staff_id, is_filled_or_cancelled, is_not_filled_or_cancelled);

        OfferingOrder[] o = new OfferingOrder[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            o[i] = LoadAll(tbl.Rows[i]);

        return o;
    }

    public static OfferingOrder GetByID(int offering_order_id)
    {
        string sql = JoinedSql + " WHERE offering_order_id = " + offering_order_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    private static string JoinedSql = @"
            SELECT 

                offeringorder.offering_order_id as offeringorder_offering_order_id,
                offeringorder.offering_id       as offeringorder_offering_id,
                offeringorder.organisation_id   as offeringorder_organisation_id,
                offeringorder.staff_id          as offeringorder_staff_id,
                offeringorder.patient_id        as offeringorder_patient_id,
                offeringorder.patient_id        as offeringorder_patient_id,
                offeringorder.quantity          as offeringorder_quantity,
                offeringorder.date_ordered      as offeringorder_date_ordered,
                offeringorder.date_filled       as offeringorder_date_filled,
                offeringorder.date_cancelled    as offeringorder_date_cancelled,
                offeringorder.descr             as offeringorder_descr,

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
                org_type.organisation_type_id as org_type_organisation_type_id,org_type.descr as org_type_descr,org_type.organisation_type_group_id as org_type_organisation_type_group_id
                ,
                typegroup.organisation_type_group_id as typegroup_organisation_type_group_id, typegroup.descr as typegroup_descr
                ,
                staff.staff_id as staff_staff_id, staff.person_id as staff_person_id, staff.login as staff_login, staff.pwd as staff_pwd, 
                staff.staff_position_id as staff_staff_position_id, staff.field_id as staff_field_id, staff.costcentre_id as staff_costcentre_id, 
                staff.is_contractor as staff_is_contractor, staff.tfn as staff_tfn, staff.provider_number as staff_provider_number, 
                staff.is_fired as staff_is_fired, staff.is_commission as staff_is_commission, staff.commission_percent as staff_commission_percent, 
                staff.is_stakeholder as staff_is_stakeholder, staff.is_master_admin as staff_is_master_admin, staff.is_admin as staff_is_admin, staff.is_principal as staff_is_principal, staff.is_provider as staff_is_provider, staff.is_external as staff_is_external,
                staff.staff_date_added as staff_staff_date_added, staff.start_date as staff_start_date, staff.end_date as staff_end_date, staff.comment as staff_comment, 
                staff.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen,  staff.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                staff.bk_screen_field_id as staff_bk_screen_field_id, staff.bk_screen_show_key as staff_bk_screen_show_key, staff.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, staff.enable_daily_reminder_email as staff_enable_daily_reminder_email
                ,
                " + PersonDB.GetFields("person_staff_", "person_staff") + @"
                ,
                title_staff.title_id as title_staff_title_id, title_staff.descr as title_staff_descr
                ,
                patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, 
                patient.is_clinic_patient as patient_is_clinic_patient, patient.is_gp_patient as patient_is_gp_patient,
                patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn,
                patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info
                ,
                " + PersonDB.GetFields("person_patient_", "person_patient") + @"
                ,
                title_patient.title_id as title_patient_title_id, title_patient.descr as title_patient_descr
                ,
                offering.offering_id as offering_offering_id, offering.offering_type_id as offering_offering_type_id, offering.field_id as offering_field_id,
                offering.aged_care_patient_type_id as offering_aged_care_patient_type_id, 
                offering.num_clinic_visits_allowed_per_year as offering_num_clinic_visits_allowed_per_year, 
                offering.offering_invoice_type_id as offering_offering_invoice_type_id, 
                offering.name as offering_name, offering.short_name as offering_short_name, offering.descr as offering_descr, offering.is_gst_exempt as offering_is_gst_exempt, 
                offering.default_price as offering_default_price, offering.service_time_minutes as offering_service_time_minutes, 
                offering.max_nbr_claimable as offering_max_nbr_claimable, offering.max_nbr_claimable_months as offering_max_nbr_claimable_months,
                offering.medicare_company_code as offering_medicare_company_code, offering.dva_company_code as offering_dva_company_code, offering.tac_company_code as offering_tac_company_code, 
                offering.medicare_charge as offering_medicare_charge, offering.dva_charge as offering_dva_charge, offering.tac_charge as offering_tac_charge,
                offering.popup_message as offering_popup_message, offering.reminder_letter_months_later_to_send as offering_reminder_letter_months_later_to_send, offering.reminder_letter_id as offering_reminder_letter_id, offering.use_custom_color as offering_use_custom_color, offering.custom_color as offering_custom_color
                ,
                offeringfield.field_id as offeringfield_field_id, offeringfield.descr as offeringfield_descr

            FROM 
                OfferingOrder offeringorder

                LEFT OUTER JOIN Organisation org                ON org.organisation_id           = offeringorder.organisation_id
                LEFT OUTER JOIN OrganisationType org_type       ON org.organisation_type_id      = org_type.organisation_type_id
                LEFT OUTER JOIN OrganisationTypeGroup typegroup ON org_type.organisation_type_group_id = typegroup.organisation_type_group_id
                LEFT OUTER JOIN Staff staff                     ON staff.staff_id                = offeringorder.staff_id
                LEFT OUTER JOIN Person person_staff             ON person_staff.person_id        = staff.person_id
                LEFT OUTER JOIN Title  title_staff              ON title_staff.title_id          = person_staff.title_id
                LEFT OUTER JOIN Patient patient                 ON patient.patient_id            = offeringorder.patient_id
                LEFT OUTER JOIN Person person_patient           ON person_patient.person_id      = patient.person_id
                LEFT OUTER JOIN Title  title_patient            ON title_patient.title_id        = person_patient.title_id
                LEFT OUTER JOIN Offering offering               ON offering.offering_id          = offeringorder.offering_id
                LEFT OUTER JOIN Field offeringfield             ON offeringfield.field_id        = offering.field_id";



    public static OfferingOrder LoadAll(DataRow row)
    {
        OfferingOrder o = Load(row, "offeringorder_");

        if (row["organisation_organisation_id"] != DBNull.Value)
        {
            o.Organisation = OrganisationDB.Load(row, "organisation_");
            o.Organisation.OrganisationType = OrganisationTypeDB.Load(row, "org_type_");
            o.Organisation.OrganisationType.OrganisationTypeGroup = IDandDescrDB.Load(row, "typegroup_organisation_type_group_id", "typegroup_descr");
        }
        if (row["staff_staff_id"] != DBNull.Value)
            o.Staff = StaffDB.Load(row, "staff_");
        if (row["staff_staff_id"] != DBNull.Value)
        {
            o.Staff.Person = PersonDB.Load(row, "person_staff_");
            o.Staff.Person.Title = IDandDescrDB.Load(row, "title_staff_title_id", "title_staff_descr");
        }
        if (row["patient_patient_id"] != DBNull.Value)
            o.Patient = PatientDB.Load(row, "patient_");
        if (row["patient_patient_id"] != DBNull.Value)
        {
            o.Patient.Person = PersonDB.Load(row, "person_patient_");
            o.Patient.Person.Title = IDandDescrDB.Load(row, "title_patient_title_id", "title_patient_descr");
        }
        if (row["offering_offering_id"] != DBNull.Value)
            o.Offering = OfferingDB.Load(row, "offering_");
        if (row["offeringfield_field_id"] != DBNull.Value)
            o.Offering.Field = IDandDescrDB.Load(row, "offeringfield_field_id", "offeringfield_descr");
        

        return o;
    }

    public static OfferingOrder Load(DataRow row, string prefix = "")
    {
        return new OfferingOrder(
            Convert.ToInt32(row[prefix + "offering_order_id"]),
            row[prefix + "offering_id"]     == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "offering_id"]),
            row[prefix + "organisation_id"] == DBNull.Value ?  0                : Convert.ToInt32(row[prefix + "organisation_id"]),
            row[prefix + "staff_id"]        == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "staff_id"]),
            row[prefix + "patient_id"]      == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "patient_id"]),
            Convert.ToInt32(row[prefix + "quantity"]),
            row[prefix + "date_ordered"]    == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_ordered"]),
            row[prefix + "date_filled"]     == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_filled"]),
            row[prefix + "date_cancelled"]  == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_cancelled"]),
            Convert.ToString(row[prefix + "descr"])
        );
    }

}