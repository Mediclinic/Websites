using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class BookingChangeHistoryDB
{

    public static void Delete(int booking_change_history_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM BookingChangeHistory WHERE booking_change_history_id = " + booking_change_history_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int booking_id, int moved_by, int booking_change_history_reason_id, DateTime previous_datetime)
    {
        string sql = "INSERT INTO BookingChangeHistory (booking_id,moved_by,booking_change_history_reason_id,previous_datetime) VALUES (" + "" + booking_id + "," + "" + moved_by + "," + booking_change_history_reason_id + "," + "'" + previous_datetime.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int booking_change_history_id, int booking_id, int moved_by, int booking_change_history_reason_id, DateTime previous_datetime)
    {
        string sql = "UPDATE BookingChangeHistory SET booking_id = " + booking_id + ",moved_by = " + moved_by + ",booking_change_history_reason_id = " + booking_change_history_reason_id + ",previous_datetime = '" + previous_datetime.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE booking_change_history_id = " + booking_change_history_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT booking_change_history_id,booking_id,moved_by,date_moved,booking_change_history_reason_id,previous_datetime FROM BookingChangeHistory";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable_ByBookingID(int booking_id)
    {
        string sql = JoinedSql + "  WHERE booking_change_history.booking_id = " + booking_id.ToString();
        return DBBase.ExecuteQuery(sql).Tables[0];

    }

    public static BookingChangeHistory GetByID(int booking_change_history_id)
    {
        string sql = JoinedSql + "  WHERE booking_change_history.booking_change_history_id = " + booking_change_history_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }



    protected static string
        JoinedSql = @"
            SELECT 

                    booking_change_history.booking_change_history_id                as booking_change_history_booking_change_history_id,
                    booking_change_history.booking_id                               as booking_change_history_booking_id,
                    booking_change_history.moved_by                                 as booking_change_history_moved_by,
                    booking_change_history.date_moved                               as booking_change_history_date_moved,
                    booking_change_history.booking_change_history_reason_id         as booking_change_history_booking_change_history_reason_id,
                    booking_change_history.previous_datetime                        as booking_change_history_previous_datetime,

                    booking_change_history_reason.booking_change_history_reason_id  as booking_change_history_reason_booking_change_history_reason_id,
                    booking_change_history_reason.descr                             as booking_change_history_reason_descr,
                    booking_change_history_reason.display_order                     as booking_change_history_reason_display_order,


                    staff.staff_id as staff_staff_id,staff.person_id as staff_person_id,staff.login as staff_login,staff.pwd as staff_pwd,staff.staff_position_id as staff_staff_position_id,
                    staff.field_id as staff_field_id,staff.is_contractor as staff_is_contractor,staff.tfn as staff_tfn,
                    staff.is_fired as staff_is_fired,staff.costcentre_id as staff_costcentre_id,
                    staff.is_admin as staff_is_admin,staff.provider_number as staff_provider_number,staff.is_commission as staff_is_commission,staff.commission_percent as staff_commission_percent,
                    staff.is_stakeholder as staff_is_stakeholder,staff.is_master_admin as staff_is_master_admin,staff.is_admin as staff_is_admin,staff.is_principal as staff_is_principal,staff.is_provider as staff_is_provider, staff.is_external as staff_is_external,
                    staff.staff_date_added as staff_staff_date_added,staff.start_date as staff_start_date,staff.end_date as staff_end_date,staff.comment as staff_comment, 
                    staff.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen, staff.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                    staff.bk_screen_field_id as staff_bk_screen_field_id,
                    staff.bk_screen_show_key as staff_bk_screen_show_key,
                    staff.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                    staff.enable_daily_reminder_email as staff_enable_daily_reminder_email,

                    " + PersonDB.GetFields("staff_person_", "staff_person") + @", 

                    title_staff.title_id as title_staff_title_id, title_staff.descr as title_staff_descr



            FROM 
                    BookingChangeHistory booking_change_history
                    Left OUTER Join BookingChangeHistoryReason booking_change_history_reason ON booking_change_history.booking_change_history_reason_id = booking_change_history_reason.booking_change_history_reason_id
                    LEFT OUTER JOIN Staff    staff         ON booking_change_history.moved_by = staff.staff_id
                    LEFT OUTER JOIN Person   staff_person  ON staff_person.person_id          = staff.person_id
                    LEFT OUTER JOIN Title    title_staff   ON title_staff.title_id            = staff_person.title_id  ";



    public static BookingChangeHistory Load(DataRow row)
    {
        return new BookingChangeHistory(
            Convert.ToInt32(row["booking_change_history_id"]),
            Convert.ToInt32(row["booking_id"]),
            Convert.ToInt32(row["moved_by"]),
            Convert.ToDateTime(row["date_moved"]),
            Convert.ToInt32(row["booking_change_history_reason_id"]),
            Convert.ToDateTime(row["previous_datetime"])
        );
    }

}