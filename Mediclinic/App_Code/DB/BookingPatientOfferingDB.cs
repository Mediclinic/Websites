using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class BookingPatientOfferingDB
{

    public static void Delete(int booking_patient_offering_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM BookingPatientOffering WHERE booking_patient_offering_id = " + booking_patient_offering_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void DeleteByBookingPatientID(int booking_patient_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM BookingPatientOffering WHERE booking_patient_id = " + booking_patient_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int booking_patient_id, int offering_id, int quantity, int added_by, string area_treated)
    {
        area_treated = area_treated.Replace("'", "''");

        string sql = "INSERT INTO BookingPatientOffering (booking_patient_id,offering_id,quantity,added_by,added_date,is_deleted,deleted_by,deleted_date, area_treated) VALUES (" + "" + booking_patient_id + "," + "" + offering_id + "," + "" + quantity + "," + "" + added_by + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "0," + "NULL" + "," + "NULL" + ",'" + area_treated + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int booking_patient_offering_id, int booking_patient_id, int offering_id, int quantity, int added_by, DateTime added_date, bool is_deleted, int deleted_by, DateTime deleted_date, string area_treated)
    {
        area_treated = area_treated.Replace("'", "''");

        string sql = "UPDATE BookingPatientOffering SET booking_patient_id = " + booking_patient_id + ",offering_id = " + offering_id + ",quantity = " + quantity + ",added_by = " + added_by + ",added_date = '" + added_date.ToString("yyyy-MM-dd HH:mm:ss") + "',is_deleted = " + (is_deleted ? "1," : "0,") + "deleted_by = " + deleted_by + ",deleted_date = '" + deleted_date.ToString("yyyy-MM-dd HH:mm:ss") + "',area_treated = '" + area_treated + "' WHERE booking_patient_offering_id = " + booking_patient_offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateAreaTreated(int booking_patient_offering_id, string area_treated)
    {
        area_treated = area_treated.Replace("'", "''");

        string sql = "UPDATE BookingPatientOffering SET area_treated = '" + area_treated + "' WHERE booking_patient_offering_id = " + booking_patient_offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static void UpdateInactive(int booking_patient_offering_id, int staff_id)
    {
        string sql = "UPDATE BookingPatientOffering SET is_deleted = 1, deleted_by = " + staff_id + ", deleted_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE booking_patient_offering_id = " + booking_patient_offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int booking_patient_offering_id, int staff_id)
    {
        string sql = "UPDATE BookingPatientOffering SET is_deleted = 0, deleted_by = " + staff_id + ", deleted_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE booking_patient_offering_id = " + booking_patient_offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateQuantity(int booking_patient_offering_id, int quantity)
    {
        string sql = "UPDATE BookingPatientOffering SET quantity = " + quantity + " WHERE booking_patient_offering_id = " + booking_patient_offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static DataTable GetDataTable(int[] booking_patient_ids = null)
    {
        // string sql = "SELECT booking_patient_offering_id,booking_patient_id,offering_id,quantity,added_by,added_date,is_deleted,deleted_by,deleted_date FROM BookingPatientOffering";
        string sql = JoinedSql() + " WHERE bpo.is_deleted = 0 " + (booking_patient_ids == null || booking_patient_ids.Length == 0 ? "" : " AND bpo.booking_patient_id IN (" + string.Join(", ", booking_patient_ids) + ")");
        return DBBase.ExecuteQuery( sql ).Tables[0];
    }
    public static BookingPatientOffering[] GetAll(int[] booking_patient_ids = null)
    {
        DataTable tbl = GetDataTable(booking_patient_ids);
        BookingPatientOffering[] bpos = new BookingPatientOffering[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bpos[i] = LoadAll(tbl.Rows[i]);

        return bpos;
    }
    public static Hashtable GetHashtable(int[] booking_patient_ids = null)
    {
        Hashtable hash = new Hashtable();
        DataTable tbl = GetDataTable(booking_patient_ids);
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            BookingPatientOffering bpo = LoadAll(tbl.Rows[i]);
            if (hash[bpo.BookingPatient.BookingPatientID] == null)
                hash[bpo.BookingPatient.BookingPatientID] = new ArrayList();
            ((ArrayList)hash[bpo.BookingPatient.BookingPatientID]).Add(bpo);
        }

        return hash;
    }

    public static BookingPatientOffering GetByID(int booking_patient_offering_id)
    {
        //string sql = "SELECT booking_patient_offering_id,booking_patient_id,offering_id,quantity,added_by,added_date,is_deleted,deleted_by,deleted_date FROM BookingPatientOffering WHERE booking_patient_offering_id = " + booking_patient_offering_id.ToString();
        string sql = JoinedSql() + " WHERE booking_patient_offering_id = " + booking_patient_offering_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery( sql ).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static DataTable GetDataTable_ByBookingPatientID(int booking_patient_id)
    {
        string sql = JoinedSql() + " WHERE bpo.booking_patient_id = " + booking_patient_id.ToString() + " AND bpo.is_deleted = 0 ORDER BY offering.name";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static BookingPatientOffering[] GetByBookingPatientID(int booking_patient_id)
    {
        DataTable tbl = GetDataTable_ByBookingPatientID(booking_patient_id);
        BookingPatientOffering[] bpos = new BookingPatientOffering[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bpos[i] = LoadAll(tbl.Rows[i]);

        return bpos;
    }


    protected static string JoinedSql()
    {
        string sql = @"
                SELECT 

                        bpo.booking_patient_offering_id as bpo_booking_patient_offering_id, bpo.booking_patient_id as bpo_booking_patient_id, 
                        bpo.offering_id as bpo_offering_id, bpo.quantity as bpo_quantity, bpo.added_by as bpo_added_by,
                        bpo.added_date as bpo_added_date, bpo.is_deleted as bpo_is_deleted, bpo.deleted_by as bpo_deleted_by, bpo.deleted_date as bpo_deleted_date, bpo.area_treated as bpo_area_treated,

                        offering.offering_id as offering_offering_id, offering.offering_type_id as offering_offering_type_id, offering.field_id as offering_field_id,
                        offering.aged_care_patient_type_id as offering_aged_care_patient_type_id, 
                        offering.num_clinic_visits_allowed_per_year as offering_num_clinic_visits_allowed_per_year, 
                        offering.offering_invoice_type_id as offering_offering_invoice_type_id, 
                        offering.name as offering_name, offering.short_name as offering_short_name, offering.descr as offering_descr, offering.is_gst_exempt as offering_is_gst_exempt, 
                        offering.default_price as offering_default_price, offering.service_time_minutes as offering_service_time_minutes, 
                        offering.max_nbr_claimable as offering_max_nbr_claimable, offering.max_nbr_claimable_months as offering_max_nbr_claimable_months,
                        offering.medicare_company_code as offering_medicare_company_code, offering.dva_company_code as offering_dva_company_code, offering.tac_company_code as offering_tac_company_code, 
                        offering.medicare_charge as offering_medicare_charge, offering.dva_charge as offering_dva_charge, offering.tac_charge as offering_tac_charge,
                        offering.popup_message as offering_popup_message, offering.reminder_letter_months_later_to_send as offering_reminder_letter_months_later_to_send, offering.reminder_letter_id as offering_reminder_letter_id, offering.use_custom_color as offering_use_custom_color, offering.custom_color as offering_custom_color,

                        offeringfield.field_id as offeringfield_field_id, offeringfield.descr as offeringfield_descr,


                        added_by.staff_id as added_by_staff_id,added_by.person_id as added_by_person_id,added_by.login as added_by_login,added_by.pwd as added_by_pwd,added_by.staff_position_id as added_by_staff_position_id,
                        added_by.field_id as added_by_field_id,added_by.is_contractor as added_by_is_contractor,added_by.tfn as added_by_tfn,
                        added_by.is_fired as added_by_is_fired,added_by.costcentre_id as added_by_costcentre_id,
                        added_by.is_admin as added_by_is_admin,added_by.provider_number as added_by_provider_number,added_by.is_commission as added_by_is_commission,added_by.commission_percent as added_by_commission_percent,
                        added_by.is_stakeholder as added_by_is_stakeholder, added_by.is_master_admin as added_by_is_master_admin, added_by.is_admin as added_by_is_admin, added_by.is_principal as added_by_is_principal, added_by.is_provider as added_by_is_provider, added_by.is_external as added_by_is_external,
                        added_by.staff_date_added as added_by_staff_date_added,added_by.start_date as added_by_start_date,added_by.end_date as added_by_end_date,added_by.comment as added_by_comment, 
                        added_by.num_days_to_display_on_booking_screen as added_by_num_days_to_display_on_booking_screen, added_by.show_header_on_booking_screen as added_by_show_header_on_booking_screen,
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
                        BookingPatientOffering bpo

                        LEFT OUTER JOIN BookingPatient         bookingpatient         ON bpo.booking_patient_id      = bookingpatient.booking_patient_id

                        LEFT OUTER JOIN Offering               offering               ON bpo.offering_id             = offering.offering_id
                        LEFT OUTER JOIN Field                  offeringfield          ON offeringfield.field_id      = offering.field_id


                        LEFT OUTER JOIN Staff                  added_by               ON bpo.added_by                = added_by.staff_id
                        LEFT OUTER JOIN Person                 added_by_person        ON added_by_person.person_id   = added_by.person_id
                        LEFT OUTER JOIN Title                  title_added_by         ON title_added_by.title_id     = added_by_person.title_id

                        LEFT OUTER JOIN Staff                  deleted_by             ON bpo.deleted_by              = deleted_by.staff_id
                        LEFT OUTER JOIN Person                 deleted_by_person      ON deleted_by_person.person_id = deleted_by.person_id
                        LEFT OUTER JOIN Title                  title_deleted_by       ON title_deleted_by.title_id   = deleted_by_person.title_id
                    ";

        return sql;
    }

    public static BookingPatientOffering LoadAll(DataRow row)
    {
        BookingPatientOffering bpo = Load(row, "bpo_");

        bpo.Offering = OfferingDB.Load(row, "offering_");
        bpo.Offering.Field = IDandDescrDB.Load(row, "offeringfield_field_id", "offeringfield_descr");

        bpo.AddedBy = StaffDB.Load(row, "added_by_");
        bpo.AddedBy.Person = PersonDB.Load(row, "added_by_person_");
        bpo.AddedBy.Person.Title = IDandDescrDB.Load(row, "title_added_by_title_id", "title_added_by_descr");

        if (row["deleted_by_person_person_id"] != DBNull.Value)
        {
            bpo.DeletedBy = StaffDB.Load(row, "deleted_by_");
            bpo.DeletedBy.Person = PersonDB.Load(row, "deleted_by_person_");
            bpo.DeletedBy.Person.Title = IDandDescrDB.Load(row, "title_deleted_by_title_id", "title_deleted_by_descr");
        }

        return bpo;
    }

    public static BookingPatientOffering Load(DataRow row, string prefix = "")
    {
        return new BookingPatientOffering(
            Convert.ToInt32(row[prefix + "booking_patient_offering_id"]),
            Convert.ToInt32(row[prefix + "booking_patient_id"]),
            Convert.ToInt32(row[prefix + "offering_id"]),
            Convert.ToInt32(row[prefix + "quantity"]),
            Convert.ToInt32(row[prefix + "added_by"]),
            Convert.ToDateTime(row[prefix + "added_date"]),
            Convert.ToBoolean(row[prefix + "is_deleted"]),
            row[prefix + "deleted_by"]   == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "deleted_by"]),
            row[prefix + "deleted_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "deleted_date"]),
            Convert.ToString(row[prefix + "area_treated"])
        );
    }

}