using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class RefundDB
{

    public static void Delete(int refund_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Refund WHERE refund_id = " + refund_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int invoice_id, decimal total, int refund_reason_id, string comment, int staff_id, string DB = null)
    {
        comment = comment.Replace("'", "''");
        string sql = "INSERT INTO Refund (invoice_id,total,refund_reason_id,comment,refund_date_added,staff_id) VALUES (" + "" + invoice_id + "," + "" + total + "," + "" + refund_reason_id + "," + "'" + comment + "'," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "" + staff_id + "" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql, DB));
    }
    public static void Update(int refund_id, decimal total, int refund_reason_id, string comment)
    {
        comment = comment.Replace("'", "''");
        string sql = "UPDATE Refund SET total = " + total + ",refund_reason_id = " + refund_reason_id + ",comment = '" + comment + "' WHERE refund_id = " + refund_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTableByInvoice(int invoice_id)
    {
        string sql = JoinedSql + " WHERE invoice_id = " + invoice_id;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Refund GetByID(int refund_id)
    {
        string sql = JoinedSql + " WHERE refund_id = " + refund_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static string JoinedSql = @"
                       SELECT   
                                refund_id,invoice_id,total,Refund.refund_reason_id,Refund.comment,refund_date_added,Refund.staff_id,
                                RefundReason.refund_reason_id,RefundReason.descr,

                                s.staff_id as staff_staff_id,s.person_id as staff_person_id,s.login as staff_login,s.pwd as staff_pwd,s.staff_position_id as staff_staff_position_id,
                                s.field_id as staff_field_id,s.is_contractor as staff_is_contractor,s.tfn as staff_tfn,
                                s.is_fired as staff_is_fired,s.costcentre_id as staff_costcentre_id,
                                s.is_admin as staff_is_admin,s.provider_number as staff_provider_number,s.is_commission as staff_is_commission,s.commission_percent as staff_commission_percent,
                                s.is_stakeholder as staff_is_stakeholder,s.is_master_admin as staff_is_master_admin,s.is_admin as staff_is_admin,s.is_principal as staff_is_principal,s.is_provider as staff_is_provider, s.is_external as staff_is_external,
                                s.staff_date_added as staff_staff_date_added,
                                s.start_date as staff_start_date,s.end_date as staff_end_date,s.comment as staff_comment,
                                s.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen, 
                                s.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                                s.bk_screen_field_id as staff_bk_screen_field_id,
                                s.bk_screen_show_key as staff_bk_screen_show_key,
                                s.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                                s.enable_daily_reminder_email as staff_enable_daily_reminder_email,

                                " + PersonDB.GetFields("staff_person_", "p") + @",
                                t.title_id as title_title_id, t.descr as title_descr
                       FROM     
                                Refund
                                INNER JOIN RefundReason            ON Refund.refund_reason_id = RefundReason.refund_reason_id 
                                LEFT OUTER JOIN Staff  s           ON Refund.staff_id  = s.staff_id
                                LEFT OUTER JOIN Person p           ON s.person_id      = p.person_id
                                LEFT OUTER JOIN Title  t           ON p.title_id       = t.title_id ";


    public static Refund LoadAll(DataRow row)
    {
        Refund refund = Load(row);
        refund.RefundReason = IDandDescrDB.Load(row, "refund_reason_id", "descr");

        refund.Staff = StaffDB.Load(row, "staff_");
        refund.Staff.Person = PersonDB.Load(row, "staff_person_");
        refund.Staff.Person.Title = IDandDescrDB.Load(row, "title_title_id", "title_descr");

        return refund;
    }

    public static Refund Load(DataRow row)
    {
        return new Refund(
            Convert.ToInt32(row["refund_id"]),
            Convert.ToInt32(row["invoice_id"]),
            Convert.ToDecimal(row["total"]),
            Convert.ToInt32(row["refund_reason_id"]),
            Convert.ToString(row["comment"]),
            Convert.ToDateTime(row["refund_date_added"]),
            Convert.ToInt32(row["staff_id"])
        );
    }

}