using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class OrganisationOfferingsDB
{

    public static void Delete(int organisation_offering_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM OrganisationOfferings WHERE organisation_offering_id = " + organisation_offering_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int organisation_id, int offering_id, decimal price, DateTime date_active)
    {
        string sql = "INSERT INTO OrganisationOfferings (organisation_id,offering_id,price,date_active) VALUES (" + "" + organisation_id + "," + "" + offering_id + "," + "" + price + "," + (date_active == DateTime.MinValue ? "null" : "'" + date_active.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int organisation_offering_id, int organisation_id, int offering_id, decimal price, DateTime date_active)
    {
        string sql = "UPDATE OrganisationOfferings SET organisation_id = " + organisation_id + ",offering_id = " + offering_id + ",price = " + price + ",date_active = " + (date_active == DateTime.MinValue ? "null" : "'" + date_active.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE organisation_offering_id = " + organisation_offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static bool IsOfferingOfferedyByOrg(int offering_id, int organisation_id)
    {
        string sql = "SELECT COUNT(*) FROM OrganisationOfferings WHERE offering_id = " + offering_id.ToString() + " AND organisation_id = " + organisation_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static DataTable GetDataTable(bool only_active, int organisation_type_group_id)
    {
        string sql = JoinedSql + (organisation_type_group_id == -1 ? "" : " AND orgtypegroup.organisation_type_group_id = " + organisation_type_group_id) + (only_active ? "  AND OrganisationOfferings.date_active <= GETDATE() " : "") + " ORDER BY org.name, o.name, oo.date_active DESC";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable_OfferingsByOrg(bool only_active, int organisation_id, string offering_invoice_type_ids = null, string offering_type_ids = null)
    {
        string sql = JoinedSql_Offerings + "  AND OrganisationOfferings.organisation_id = " + organisation_id +
            (only_active ? "  AND OrganisationOfferings.date_active <= GETDATE() " : "") +
            (offering_invoice_type_ids == null || offering_invoice_type_ids .Length == 0 ? "" : " AND o.offering_invoice_type_id IN (" + offering_invoice_type_ids + ")") +
            (offering_type_ids        == null ? "" : " AND o.offering_type_id IN ("+offering_type_ids+")") ;
        sql += " ORDER BY o.name";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static DataTable GetDataTable_OfferingsByOrg(bool only_active, string organisation_ids, string offering_invoice_type_ids = null, string offering_type_ids = null)
    {
        string sql = JoinedSql_Offerings + "  AND OrganisationOfferings.organisation_id IN (" + organisation_ids + @")" +
            (only_active ? "  AND OrganisationOfferings.date_active <= GETDATE() " : "") +
            (offering_invoice_type_ids == null || offering_invoice_type_ids.Length == 0 ? "" : " AND o.offering_invoice_type_id IN (" + offering_invoice_type_ids + ")") +
            (offering_type_ids == null ? "" : " AND o.offering_type_id IN (" + offering_type_ids + ")");
        sql += " ORDER BY o.name";

        sql = sql.Replace("SELECT", "SELECT DISTINCT");

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Offering[] GetOfferingsByOrg(bool only_active, int organisation_id, string offering_invoice_type_ids = null, string offering_type_ids = null)
    {
        DataTable tbl = GetDataTable_OfferingsByOrg(only_active, organisation_id, offering_invoice_type_ids, offering_type_ids);
        Offering[] list = new Offering[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = OfferingDB.LoadAll(tbl.Rows[i]);
        return list;
    }
    public static Offering[] GetOfferingsByOrg(bool only_active, string organisation_ids, string offering_invoice_type_ids = null, string offering_type_ids = null)
    {
        DataTable tbl = GetDataTable_OfferingsByOrg(only_active, organisation_ids, offering_invoice_type_ids, offering_type_ids);
        Offering[] list = new Offering[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = OfferingDB.LoadAll(tbl.Rows[i]);
        return list;
    }


    private static string JoinedSql_Offerings = @"
                        SELECT
                            o.offering_id as o_offering_id, o.offering_type_id as o_offering_type_id, o.field_id as o_field_id, 
                            o.aged_care_patient_type_id as o_aged_care_patient_type_id, 
                            o.num_clinic_visits_allowed_per_year as o_num_clinic_visits_allowed_per_year, 
                            o.offering_invoice_type_id as o_offering_invoice_type_id, o.name as o_name, o.short_name as o_short_name, o.descr as o_descr, 
                            o.is_gst_exempt as o_is_gst_exempt, 

                            OrganisationOfferings.price as o_default_price,
                            -- o.default_price as o_default_price, 

                            o.service_time_minutes as o_service_time_minutes, 
                            o.max_nbr_claimable as o_max_nbr_claimable, o.max_nbr_claimable_months as o_max_nbr_claimable_months,
                            o.medicare_company_code as o_medicare_company_code, o.dva_company_code as o_dva_company_code, o.tac_company_code as o_tac_company_code, 
                            o.medicare_charge as o_medicare_charge, o.dva_charge as o_dva_charge, o.tac_charge as o_tac_charge, o.popup_message as o_popup_message,o.reminder_letter_months_later_to_send as o_reminder_letter_months_later_to_send, o.reminder_letter_id as o_reminder_letter_id, o.use_custom_color as o_use_custom_color, o.custom_color as o_custom_color,

                            type.offering_type_id AS type_offering_type_id, type.descr AS type_descr,
                            fld.field_id AS fld_field_id, fld.descr AS fld_descr, 
                            acpatientcat.aged_care_patient_type_id AS acpatientcat_aged_care_patient_type_id, acpatientcat.descr AS acpatientcat_descr,
                            invtype.offering_invoice_type_id AS invtype_offering_invoice_type_id, invtype.descr AS invtype_descr
                        FROM
                            OrganisationOfferings
                            INNER JOIN Organisation                       org          ON OrganisationOfferings.organisation_id       = org.organisation_id
                            INNER JOIN Offering                           o            ON OrganisationOfferings.offering_id           = o.offering_id
                            INNER JOIN OfferingInvoiceType                invtype      ON o.offering_invoice_type_id                  = invtype.offering_invoice_type_id 
                            INNER JOIN AgedCarePatientType acpatientcat ON o.aged_care_patient_type_id = acpatientcat.aged_care_patient_type_id 
                            INNER JOIN Field                              fld     ON o.field_id                                  = fld.field_id 
                            INNER JOIN OfferingType                       type         ON o.offering_type_id                          = type.offering_type_id
                        WHERE 
                            o.is_deleted = 0 and org.is_deleted = 0 ";


    private static string JoinedSql = @"
                        SELECT

                            oo.organisation_offering_id as oo_organisation_offering_id, oo.organisation_id as oo_organisation_id,
                            oo.offering_id as oo_offering_id, oo.price as oo_price, oo.date_active as oo_date_active,

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

                            -- OrganisationOfferings.price as o_default_price,
                            o.default_price as o_default_price, 

                            o.service_time_minutes as o_service_time_minutes, 
                            o.max_nbr_claimable as o_max_nbr_claimable, o.max_nbr_claimable_months as o_max_nbr_claimable_months,
                            o.medicare_company_code as o_medicare_company_code, o.dva_company_code as o_dva_company_code, o.tac_company_code as o_tac_company_code, 
                            o.medicare_charge as o_medicare_charge, o.dva_charge as o_dva_charge, o.tac_charge as o_tac_charge, o.popup_message as o_popup_message, o.reminder_letter_months_later_to_send as o_reminder_letter_months_later_to_send, o.reminder_letter_id as o_reminder_letter_id, o.use_custom_color as o_use_custom_color, o.custom_color as o_custom_color,

                            type.offering_type_id AS type_offering_type_id, type.descr AS type_descr,
                            fld.field_id AS fld_field_id, fld.descr AS fld_descr, 
                            acpatientcat.aged_care_patient_type_id AS acpatientcat_aged_care_patient_type_id, acpatientcat.descr AS acpatientcat_descr,
                            invtype.offering_invoice_type_id AS invtype_offering_invoice_type_id, invtype.descr AS invtype_descr
                        FROM
                            OrganisationOfferings                         oo
                            INNER JOIN Organisation                       org          ON oo.organisation_id                          = org.organisation_id
                            INNER JOIN OrganisationType                   orgtype      ON org.organisation_type_id                    = orgtype.organisation_type_id 
                            INNER JOIN OrganisationTypeGroup              orgtypegroup ON orgtype.organisation_type_group_id          = orgtypegroup.organisation_type_group_id 
                            INNER JOIN Offering                           o            ON oo.offering_id                              = o.offering_id
                            INNER JOIN OfferingInvoiceType                invtype      ON o.offering_invoice_type_id                  = invtype.offering_invoice_type_id 
                            INNER JOIN AgedCarePatientType acpatientcat ON o.aged_care_patient_type_id = acpatientcat.aged_care_patient_type_id 
                            INNER JOIN Field                              fld     ON o.field_id                                  = fld.field_id 
                            INNER JOIN OfferingType                       type         ON o.offering_type_id                          = type.offering_type_id
                        WHERE 
                            o.is_deleted = 0 and org.is_deleted = 0 ";


    public static DataTable GetDataTable_ByOrg(int organisation_id)
    {
        string sql = JoinedSql + " AND oo.organisation_id = " + organisation_id.ToString() + " ORDER BY o.name, oo.date_active DESC";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static OrganisationOfferings GetByID(int organisation_offering_id)
    {
        string sql = "SELECT organisation_offering_id,organisation_id,offering_id,price,date_active FROM OrganisationOfferings WHERE organisation_offering_id = " + organisation_offering_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }


    public static Offering GetOfferingByOrgAndOffering(int organisation_id, int offering_id)
    {
        string sql = JoinedSql_Offerings + "  AND OrganisationOfferings.date_active <= GETDATE() AND OrganisationOfferings.organisation_id = " + organisation_id.ToString() + " AND OrganisationOfferings.offering_id = " + offering_id.ToString() + " ORDER BY OrganisationOfferings.date_active DESC";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : OfferingDB.LoadAll(tbl.Rows[0]);
    }


    public static System.Collections.Hashtable GetHashActiveByOrg(int org_id)
    {
        System.Collections.Hashtable activeOrgOfferings = new System.Collections.Hashtable();

        DataTable dt_activeOrgOfferings = OrganisationOfferingsDB.GetDataTable_ByOrg(org_id);
        dt_activeOrgOfferings = OrganisationOfferingsDB.AddIsActiveFieldToRows(dt_activeOrgOfferings);
        for (int i = dt_activeOrgOfferings.Rows.Count - 1; i >= 0; i--)
        {
            if (Convert.ToBoolean(dt_activeOrgOfferings.Rows[i]["is_active"]))
            {
                OrganisationOfferings curOrgOffering = OrganisationOfferingsDB.Load(dt_activeOrgOfferings.Rows[i], "oo_");
                curOrgOffering.Offering = OfferingDB.Load(dt_activeOrgOfferings.Rows[i], "o_");
                activeOrgOfferings[curOrgOffering.Offering.OfferingID] = curOrgOffering;
            }
        }

        return activeOrgOfferings;
    }

    public static DataTable AddIsActiveFieldToRows(DataTable dt_original)
    {
        DataTable dt = dt_original.Copy();

        dt.Columns.Add("is_active", typeof(bool));
        System.Collections.Hashtable offerings = new System.Collections.Hashtable();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            OrganisationOfferings curOrgOffering = OrganisationOfferingsDB.Load(dt.Rows[i], "oo_");
            curOrgOffering.Offering = OfferingDB.Load(dt.Rows[i], "o_");

            if (curOrgOffering.DateActive.Date > DateTime.Today)  // if date after today - ignore (its inactive)
                continue;

            if (offerings[new Hashtable2D.Key(curOrgOffering.Offering.OfferingID, curOrgOffering.Organisation.OrganisationID)] == null)
                offerings[new Hashtable2D.Key(curOrgOffering.Offering.OfferingID, curOrgOffering.Organisation.OrganisationID)] = curOrgOffering;
            else if (((OrganisationOfferings)offerings[new Hashtable2D.Key(curOrgOffering.Offering.OfferingID, curOrgOffering.Organisation.OrganisationID)]).DateActive < curOrgOffering.DateActive)
                offerings[new Hashtable2D.Key(curOrgOffering.Offering.OfferingID, curOrgOffering.Organisation.OrganisationID)] = curOrgOffering;
            else if (((OrganisationOfferings)offerings[new Hashtable2D.Key(curOrgOffering.Offering.OfferingID, curOrgOffering.Organisation.OrganisationID)]).DateActive == curOrgOffering.DateActive && ((OrganisationOfferings)offerings[new Hashtable2D.Key(curOrgOffering.Offering.OfferingID, curOrgOffering.Organisation.OrganisationID)]).OrganisationOfferingID > curOrgOffering.OrganisationOfferingID)
                offerings[new Hashtable2D.Key(curOrgOffering.Offering.OfferingID, curOrgOffering.Organisation.OrganisationID)] = curOrgOffering;
        }
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            OrganisationOfferings curOrgOffering = OrganisationOfferingsDB.Load(dt.Rows[i], "oo_");
            curOrgOffering.Offering = OfferingDB.Load(dt.Rows[i], "o_");

            OrganisationOfferings activeOrgOffering = (OrganisationOfferings)(offerings[new Hashtable2D.Key(curOrgOffering.Offering.OfferingID, curOrgOffering.Organisation.OrganisationID)]);

            dt.Rows[i]["is_active"] = curOrgOffering.OrganisationOfferingID == activeOrgOffering.OrganisationOfferingID;

            if (dt.Rows[i]["oo_date_active"] == DBNull.Value)
                dt.Rows[i]["is_active"] = false;
        }

        return dt;
    }

    public static OrganisationOfferings Load(DataRow row, string prefix = "")
    {
        return new OrganisationOfferings(
            Convert.ToInt32(row[prefix + "organisation_offering_id"]),
            Convert.ToInt32(row[prefix + "organisation_id"]),
            Convert.ToInt32(row[prefix + "offering_id"]),
            Convert.ToDecimal(row[prefix + "price"]),
            row[prefix + "date_active"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_active"])
        );
    }

}