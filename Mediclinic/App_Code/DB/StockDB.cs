using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class StockDB
{

    public static void Delete(int stock_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Stock WHERE stock_id = " + stock_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int organisation_id, int offering_id, int qty, int warning_amt)
    {
        string sql = "INSERT INTO Stock (organisation_id,offering_id,qty,warning_amt) VALUES (" + "" + organisation_id + "," + "" + offering_id + "," + "" + qty + "," + warning_amt + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int stock_id, int organisation_id, int offering_id, int qty, int warning_amt)
    {
        string sql = "UPDATE Stock SET organisation_id = " + organisation_id + ",offering_id = " + offering_id + ",qty = " + qty + ",warning_amt = " + warning_amt + " WHERE stock_id = " + stock_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateQuantity(int stock_id, int qty)
    {
        string sql = "UPDATE Stock SET qty = " + qty + " WHERE stock_id = " + stock_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateAndCheckWarning(int organisation_id, int offering_id, int qtyUsed)
    {
        Organisation org      = OrganisationDB.GetByID(organisation_id);
        Offering     offering = OfferingDB.GetByID(offering_id);

        Stock[] stockList = StockDB.GetByOrg(org.OrganisationID);
        string warningEmail = SystemVariableDB.GetByDescr("StockWarningNotificationEmailAddress").Value;

        for (int i = 0; i < stockList.Length; i++)
        {
            if (offering.OfferingID == stockList[i].Offering.OfferingID && stockList[i].Quantity >= 0)
            {
                int prevQty = stockList[i].Quantity;
                int postQty = stockList[i].Quantity - qtyUsed;
                if (postQty < 0) postQty = 0;

                if (warningEmail.Length > 0 && stockList[i].WarningAmount >= 0 && qtyUsed > 0 && stockList[i].WarningAmount < prevQty && stockList[i].WarningAmount >= postQty)
                {
                    try
                    {
                        Emailer.SimpleEmail(
                            warningEmail,
                            "Stock Warning Level Reached For " + stockList[i].Offering.Name + " at " + org.Name,
                            "This is an automated email to notify you that the stock warning level of <b>" + stockList[i].WarningAmount + "</b> items that was set for <b>" + stockList[i].Offering.Name + "</b> at <b>" + org.Name + "</b> has been reached and you may need to re-stock.<br /><br />Best regards,<br />Mediclinic",
                            true,
                            null,
                            null
                            );
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex, true);
                    }
                }

                StockDB.UpdateQuantity(stockList[i].StockID, postQty);
            }
        }
    }

    public static bool IsOfferingStockedByOrg(int offering_id, int organisation_id)
    {
        string sql = "SELECT COUNT(*) FROM Stock WHERE offering_id = " + offering_id.ToString() + " AND organisation_id = " + organisation_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static DataTable GetDataTable(int organisation_type_group_id)
    {
        string sql = JoinedSql + (organisation_type_group_id == -1 ? "" : " AND orgtypegroup.organisation_type_group_id = " + organisation_type_group_id) + " ORDER BY org.name, o.name";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable_OfferingsByOrg(int organisation_id)
    {
        string sql = JoinedSql_Offerings + "  AND Stock.organisation_id = " + organisation_id;
        sql += " ORDER BY o.name";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static DataTable GetDataTable_OfferingsByOrg(string organisation_ids)
    {
        string sql = JoinedSql_Offerings + "  AND Stock.organisation_id IN (" + organisation_ids + @")";
        sql += " ORDER BY o.name";

        sql = sql.Replace("SELECT", "SELECT DISTINCT");

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Offering[] GetOfferingsByOrg(int organisation_id)
    {
        DataTable tbl = GetDataTable_OfferingsByOrg(organisation_id);
        Offering[] list = new Offering[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = OfferingDB.LoadAll(tbl.Rows[i]);
        return list;
    }
    public static Offering[] GetOfferingsByOrg(string organisation_ids)
    {
        DataTable tbl = GetDataTable_OfferingsByOrg(organisation_ids);
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
                            Stock

                            INNER JOIN Organisation                       org          ON Stock.organisation_id                       = org.organisation_id

                            INNER JOIN Offering                           o            ON Stock.offering_id                           = o.offering_id
                            INNER JOIN OfferingInvoiceType                invtype      ON o.offering_invoice_type_id                  = invtype.offering_invoice_type_id 
                            INNER JOIN AgedCarePatientType                acpatientcat ON o.aged_care_patient_type_id                 = acpatientcat.aged_care_patient_type_id 
                            INNER JOIN Field                              fld          ON o.field_id                                  = fld.field_id 
                            INNER JOIN OfferingType                       type         ON o.offering_type_id                          = type.offering_type_id
                        WHERE 
                            o.is_deleted = 0 and org.is_deleted = 0 ";


    private static string JoinedSql = @"
                        SELECT

                            s.stock_id as s_stock_id, s.organisation_id as s_organisation_id,
                            s.offering_id as s_offering_id, s.qty as s_qty, s.warning_amt as s_warning_amt,

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
                            invtype.offering_invoice_type_id AS invtype_offering_invoice_type_id, invtype.descr AS invtype_descr
                        FROM
                            Stock                                         s
                            INNER JOIN Organisation                       org          ON s.organisation_id                           = org.organisation_id
                            INNER JOIN OrganisationType                   orgtype      ON org.organisation_type_id                    = orgtype.organisation_type_id 
                            INNER JOIN OrganisationTypeGroup              orgtypegroup ON orgtype.organisation_type_group_id          = orgtypegroup.organisation_type_group_id 
                            INNER JOIN Offering                           o            ON s.offering_id                               = o.offering_id
                            INNER JOIN OfferingInvoiceType                invtype      ON o.offering_invoice_type_id                  = invtype.offering_invoice_type_id 
                            INNER JOIN AgedCarePatientType                acpatientcat ON o.aged_care_patient_type_id                 = acpatientcat.aged_care_patient_type_id 
                            INNER JOIN Field                              fld          ON o.field_id                                  = fld.field_id 
                            INNER JOIN OfferingType                       type         ON o.offering_type_id                          = type.offering_type_id
                        WHERE 
                            o.is_deleted = 0 and org.is_deleted = 0 ";


    public static DataTable GetDataTable_ByOrg(int organisation_id)
    {
        string sql = JoinedSql + " AND s.organisation_id = " + organisation_id.ToString() + " ORDER BY o.name";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Stock[] GetByOrg(int organisation_id)
    {
        string sql = JoinedSql + " AND s.organisation_id = " + organisation_id.ToString() + " ORDER BY o.name";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Stock[] stockList = new Stock[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            stockList[i] = StockDB.Load(tbl.Rows[i], "s_");
            stockList[i].Offering = OfferingDB.Load(tbl.Rows[i], "o_");
        }

        return stockList;
    }



    public static Stock GetByID(int stock_id)
    {
        string sql = "SELECT stock_id,organisation_id,offering_id,qty,warning_amt FROM Stock WHERE stock_id = " + stock_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }


    public static Stock GetOfferingByOrgAndOffering(int organisation_id, int offering_id)
    {
        string sql = JoinedSql + "  AND s.organisation_id = " + organisation_id.ToString() + " AND s.offering_id = " + offering_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0], "s_");
    }


    public static System.Collections.Hashtable GetHashByOrg(int org_id)
    {
        System.Collections.Hashtable orgOfferings = new System.Collections.Hashtable();

        DataTable dt_orgOfferings = StockDB.GetDataTable_ByOrg(org_id);
        for (int i = 0; i< dt_orgOfferings.Rows.Count; i++)
        {
            Stock curStock = StockDB.Load(dt_orgOfferings.Rows[i], "s_");
            curStock.Offering = OfferingDB.Load(dt_orgOfferings.Rows[i], "o_");
            orgOfferings[curStock.Offering.OfferingID] = curStock;
        }

        return orgOfferings;
    }


    public static Stock Load(DataRow row, string prefix = "")
    {
        return new Stock(
            Convert.ToInt32(row[prefix + "stock_id"]),
            Convert.ToInt32(row[prefix + "organisation_id"]),
            Convert.ToInt32(row[prefix + "offering_id"]),
            Convert.ToInt32(row[prefix + "qty"]),
            Convert.ToInt32(row[prefix + "warning_amt"])
        );
    }

}