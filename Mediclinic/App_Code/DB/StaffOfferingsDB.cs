using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class StaffOfferingsDB
{

    public static void Delete(int staff_offering_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM StaffOfferings WHERE staff_offering_id = " + staff_offering_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static int Insert(int staff_id, int offering_id, bool is_commission, decimal commission_percent, bool is_fixed_rate, decimal fixed_rate, DateTime date_active)
    {
        string sql = "INSERT INTO StaffOfferings (staff_id,offering_id,is_commission,commission_percent,is_fixed_rate,fixed_rate,date_active) VALUES (" + "" + staff_id + "," + "" + offering_id + "," + (is_commission ? "1," : "0,") + "" + commission_percent + "," + (is_fixed_rate ? "1," : "0,") + "" + fixed_rate + "," + (date_active == DateTime.MinValue ? "null" : "'" + date_active.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static void InsertBulkByStaffID(int staff_id, bool is_commission, decimal commission_percent, bool is_fixed_rate, decimal fixed_rate, DateTime date_active)
    {
        string sql = @"INSERT INTO StaffOfferings (staff_id,offering_id,is_commission,commission_percent,is_fixed_rate,fixed_rate,date_active)
                       SELECT " + staff_id + ", Offering.offering_id, " + (is_commission ? "1" : "0") + "," + commission_percent + "," + (is_fixed_rate ? "1" : "0") + "," + fixed_rate + "," + (date_active == DateTime.MinValue ? "NULL" : "'" + date_active.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                       FROM   Offering";

        DBBase.ExecuteNonResult(sql);
    }
    public static void InsertBulkByOfferingID(int offering_id, bool is_commission, decimal commission_percent, bool is_fixed_rate, decimal fixed_rate, DateTime date_active)
    {
        string sql = @"INSERT INTO StaffOfferings (staff_id,offering_id,is_commission,commission_percent,is_fixed_rate,fixed_rate,date_active)
                       SELECT Staff.staff_id, " + offering_id + "," + (is_commission ? "1" : "0") + "," + commission_percent + "," + (is_fixed_rate ? "1" : "0") + "," + fixed_rate + "," + (date_active == DateTime.MinValue ? "NULL" : "'" + date_active.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                       FROM   Staff";

        DBBase.ExecuteNonResult(sql);
    }

    public static void Update(int staff_offering_id, int staff_id, int offering_id, bool is_commission, decimal commission_percent, bool is_fixed_rate, decimal fixed_rate, DateTime date_active)
    {
        string sql = "UPDATE StaffOfferings SET staff_id = " + staff_id + ",offering_id = " + offering_id + ",is_commission = " + (is_commission ? "1," : "0,") + "commission_percent = " + commission_percent + ",is_fixed_rate = " + (is_fixed_rate ? "1," : "0,") + "fixed_rate = " + fixed_rate + ",date_active = " + (date_active == DateTime.MinValue ? "null" : "'" + date_active.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE staff_offering_id = " + staff_offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateBulk(int staff_id, int offering_id, bool is_commission, decimal commission_percent, bool is_fixed_rate, decimal fixed_rate, DateTime date_active)
    {
        string sql = "UPDATE StaffOfferings SET is_commission = " + (is_commission ? "1," : "0,") + "commission_percent = " + commission_percent + ",is_fixed_rate = " + (is_fixed_rate ? "1," : "0,") + "fixed_rate = " + fixed_rate + ",date_active = " + (date_active == DateTime.MinValue ? "null" : "'" + date_active.ToString("yyyy-MM-dd HH:mm:ss") + "'");
        
        string whereClause = string.Empty;
        if (staff_id != -1)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " staff_id = " + staff_id;
        if (offering_id != -1)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " offering_id = " + offering_id;

        DBBase.ExecuteNonResult(sql + whereClause);
    }

    public static DataTable GetDataTable(bool only_active, int staff_id = -1, int offering_id = -1)
    {
        string sql = JoinedSql + (only_active ? "  AND so.date_active <= GETDATE() " : "") + (staff_id != -1 ? " AND so.staff_id = " + staff_id : "") + (offering_id != -1 ? " AND so.offering_id = " + offering_id : "");
        sql += " AND s.is_fired = 0 AND s.is_provider = 1";
        sql += " AND o.is_deleted = 0";
        sql += " ORDER BY p.surname, p.firstname, p.middlename, o.name ";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static StaffOfferings[] GetAll(bool only_active)
    {
        DataTable tbl = GetDataTable(only_active);
        StaffOfferings[] staffOfferings = new StaffOfferings[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
             staffOfferings[i] = LoadAll(tbl.Rows[i]);
        return staffOfferings;
    }

    // returns 2d hashtable
    // get by:  hash[new Hashtable2D.Key(staffID, offeringID)]
    public static System.Collections.Hashtable Get2DHash(bool only_active, int staff_id = -1)
    {
        DataTable tbl = staff_id == -1 ? GetDataTable(only_active) : GetDataTableByStaffID(staff_id, only_active);

        System.Collections.Hashtable hash = new System.Collections.Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            StaffOfferings staffOffering = LoadAll(tbl.Rows[i]);
            hash[new Hashtable2D.Key(staffOffering.Staff.StaffID, staffOffering.Offering.OfferingID)] = staffOffering;
        }

        return hash;
    }



    public static DataTable GetDataTableByStaffID(int staff_id, bool only_active)
    {
        string sql = JoinedSql + " AND so.staff_id = " + staff_id + (only_active ? "  AND so.date_active <= GETDATE() " : "");
        sql += " ORDER BY p.surname, p.firstname, p.middlename, o.name ";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static StaffOfferings GetByID(int staff_offering_id, bool only_active)
    {
        string sql = JoinedSql + " AND staff_offering_id = " + staff_offering_id.ToString() + (only_active ? "  AND so.date_active <= GETDATE() " : "");
        sql += " ORDER BY p.surname, p.firstname, p.middlename, o.name ";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }



    private static string JoinedSql = @"
            SELECT
                -- staff offerings
                so.staff_offering_id as so_staff_offering_id,so.staff_id as so_staff_id,so.offering_id as so_offering_id,
                so.is_commission as so_is_commission,so.commission_percent as so_commission_percent,so.is_fixed_rate as so_is_fixed_rate,so.fixed_rate as so_fixed_rate,
                so.date_active as so_date_active,

                -- staff
                s.staff_id as staff_staff_id,s.person_id as staff_person_id,s.login as staff_login,s.pwd as staff_pwd,s.staff_position_id as staff_staff_position_id,
                s.field_id as staff_field_id,s.is_contractor as staff_is_contractor,s.tfn as staff_tfn,
                s.is_fired as staff_is_fired,s.costcentre_id as staff_costcentre_id,
                s.is_admin as staff_is_admin,s.provider_number as staff_provider_number,s.is_commission as staff_is_commission,s.commission_percent as staff_commission_percent,
                s.is_stakeholder as staff_is_stakeholder,s.is_master_admin as staff_is_master_admin,s.is_admin as staff_is_admin,s.is_principal as staff_is_principal,s.is_provider as staff_is_provider, s.is_external as staff_is_external,
                s.staff_date_added as staff_staff_date_added, s.start_date as staff_start_date,s.end_date as staff_end_date,s.comment as staff_comment,
                s.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen, 
                s.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                s.bk_screen_field_id as staff_bk_screen_field_id, 
                s.bk_screen_show_key as staff_bk_screen_show_key, 
                s.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                s.enable_daily_reminder_email as staff_enable_daily_reminder_email,

                " + PersonDB.GetFields("person_", "p") + @",
                t.title_id as title_title_id, t.descr as title_descr,

                c.costcentre_id as cost_centre_costcentre_id,c.descr as cost_centre_descr,c.parent_id as cost_centre_parent_id,
                sr.field_id as field_field_id,sr.descr as field_descr,
                sp.staff_position_id as staff_position_staff_position_id,sp.descr as staff_position_descr,


                -- offering
                o.offering_id as o_offering_id, o.offering_type_id as o_offering_type_id, o.field_id as o_field_id, 
                o.aged_care_patient_type_id as o_aged_care_patient_type_id, 
                o.num_clinic_visits_allowed_per_year as o_num_clinic_visits_allowed_per_year, 
                o.offering_invoice_type_id as o_offering_invoice_type_id, o.name as o_name, o.short_name as o_short_name, o.descr as o_descr, 
                o.is_gst_exempt as o_is_gst_exempt, o.default_price as o_default_price, 
                o.service_time_minutes as o_service_time_minutes, 
                o.max_nbr_claimable as o_max_nbr_claimable, o.max_nbr_claimable_months as o_max_nbr_claimable_months,
                o.medicare_company_code as o_medicare_company_code, o.dva_company_code as o_dva_company_code, o.tac_company_code as o_tac_company_code, 
                o.medicare_charge as o_medicare_charge, o.dva_charge as o_dva_charge, o.tac_charge as o_tac_charge, o.popup_message as o_popup_message, o.reminder_letter_months_later_to_send as o_reminder_letter_months_later_to_send, o.reminder_letter_id as o_reminder_letter_id, o.use_custom_color as o_use_custom_color, o.custom_color as o_custom_color,

                type.offering_type_id AS type_offering_type_id, type.descr AS type_descr,
                fld.field_id AS fld_field_id, fld.descr AS fld_descr, 
                acpatientcat.aged_care_patient_type_id AS acpatientcat_aged_care_patient_type_id, acpatientcat.descr AS acpatientcat_descr,
                invtype.offering_invoice_type_id AS invtype_offering_invoice_type_id, invtype.descr AS invtype_descr
            FROM
                StaffOfferings                                so 

                INNER JOIN Staff                              s            ON s.staff_id                                  = so.staff_id
                INNER JOIN Person                             p            ON s.person_id                                 = p.person_id
                LEFT OUTER JOIN Person                        p2           ON p2.person_id                                = p.added_by
                INNER JOIN Title                              t            ON t.title_id                                  = p.title_id
                INNER JOIN Field                              sr           ON s.field_id                             = sr.field_id
                INNER JOIN StaffPosition                      sp           ON sp.staff_position_id                        = s.staff_position_id
                INNER JOIN CostCentre                         c            ON c.costcentre_id                             = s.costcentre_id

                INNER JOIN Offering                           o            ON so.offering_id                              = o.offering_id
                INNER JOIN OfferingInvoiceType                invtype      ON o.offering_invoice_type_id                  = invtype.offering_invoice_type_id 
                INNER JOIN AgedCarePatientType acpatientcat ON o.aged_care_patient_type_id = acpatientcat.aged_care_patient_type_id 
                INNER JOIN Field                              fld     ON o.field_id                                  = fld.field_id 
                INNER JOIN OfferingType                       type         ON o.offering_type_id                          = type.offering_type_id
            WHERE 
                s.is_fired = 0 ";


    public static StaffOfferings LoadAll(DataRow row)
    {
        StaffOfferings so = Load(row, "so_");

        so.Offering = OfferingDB.Load(row, "o_");
        so.Offering.OfferingType = IDandDescrDB.Load(row, "type_offering_type_id", "type_descr");
        so.Offering.Field = IDandDescrDB.Load(row, "fld_field_id", "fld_descr");
        so.Offering.AgedCarePatientType = IDandDescrDB.Load(row, "acpatientcat_aged_care_patient_type_id", "acpatientcat_descr");
        so.Offering.OfferingInvoiceType = IDandDescrDB.Load(row, "invtype_offering_invoice_type_id", "invtype_descr");

        so.Staff = StaffDB.Load(row, "staff_");
        so.Staff.Person = PersonDB.Load(row, "person_");
        so.Staff.Person.Title = IDandDescrDB.Load(row, "title_title_id", "title_descr");
        so.Staff.Field = IDandDescrDB.Load(row, "field_field_id", "field_descr");
        so.Staff.StaffPosition = StaffPositionDB.Load(row, "staff_position_");
        so.Staff.CostCentre = CostCentreDB.Load(row, "cost_centre_");

        return so;
    }

    public static StaffOfferings Load(DataRow row, string prefix="")
    {
        return new StaffOfferings(
            Convert.ToInt32(row[prefix + "staff_offering_id"]),
            Convert.ToInt32(row[prefix + "staff_id"]),
            Convert.ToInt32(row[prefix + "offering_id"]),
            Convert.ToBoolean(row[prefix + "is_commission"]),
            Convert.ToDecimal(row[prefix + "commission_percent"]),
            Convert.ToBoolean(row[prefix + "is_fixed_rate"]),
            Convert.ToDecimal(row[prefix + "fixed_rate"]),
            row[prefix + "date_active"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_active"])
        );
    }

}