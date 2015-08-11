using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class CreditNoteDB
{

    public static void Delete(int creditnote_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM CreditNote WHERE creditnote_id = " + creditnote_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int invoice_id, decimal total, string reason, int staff_id)
    {
        reason = reason.Replace("'", "''");
        string sql = "INSERT INTO CreditNote (invoice_id,total,reason,credit_note_date_added,staff_id,reversed_by,reversed_date,pre_reversed_amount) VALUES (" + "" + invoice_id + "," + "" + total + "," + "'" + reason + "'," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "" + staff_id + "," + "NULL" + "," + "NULL" + "," + "0.00" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int creditnote_id, int invoice_id, decimal total, string reason, int reversed_by, DateTime reversed_date, decimal pre_reversed_amount)
    {
        reason = reason.Replace("'", "''");
        string sql = "UPDATE CreditNote SET invoice_id = " + invoice_id + ",total = " + total + ",reason = '" + reason + "'" + ",reversed_by = " + (reversed_by == -1 ? "NULL" : reversed_by.ToString()) + ",reversed_date = " + (reversed_date == DateTime.MinValue ? "NULL" : "'" + reversed_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",pre_reversed_amount = " + pre_reversed_amount + " WHERE creditnote_id = " + creditnote_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void Reverse(int creditnote_id, int reversed_by)
    {
        CreditNote creditNote = CreditNoteDB.GetByID(creditnote_id);
        if (creditNote == null)
            throw new CustomMessageException("Adjustment note - does not exist");
        if (creditNote.IsReversed)
            throw new CustomMessageException("Adjustment note already reversed");

        // set total=0, set who and when it was reversed, and original amount
        string sql = "UPDATE CreditNote SET total = 0, reversed_by = " + reversed_by + ",reversed_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',pre_reversed_amount = " + creditNote.Total + " WHERE creditnote_id = " + creditnote_id.ToString();
        DBBase.ExecuteNonResult(sql);

        // set invoice as not paid
        InvoiceDB.UpdateIsPaid(null, creditNote.Invoice.InvoiceID, false);

        // update the GL for the year this was done
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
    public static CreditNote[] GetByInvoice(int invoice_id)
    {
        DataTable tbl = GetDataTableByInvoice(invoice_id);
        CreditNote[] list = new CreditNote[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }



    public static CreditNote GetByID(int creditnote_id)
    {
        string sql = JoinedSql + " WHERE creditnote_id = " + creditnote_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static string JoinedSql = @"
                       SELECT   
                                creditnote_id,invoice_id,total,reason,credit_note_date_added,CreditNote.staff_id,reversed_by,reversed_date,pre_reversed_amount,

                                s.staff_id as staff_staff_id,s.person_id as staff_person_id,s.login as staff_login,s.pwd as staff_pwd,s.staff_position_id as staff_staff_position_id,
                                s.field_id as staff_field_id,s.is_contractor as staff_is_contractor,s.tfn as staff_tfn,
                                s.is_fired as staff_is_fired,s.costcentre_id as staff_costcentre_id,
                                s.is_admin as staff_is_admin,s.provider_number as staff_provider_number,s.is_commission as staff_is_commission,s.commission_percent as staff_commission_percent,
                                s.is_stakeholder as staff_is_stakeholder,s.is_master_admin as staff_is_master_admin,s.is_admin as staff_is_admin,s.is_principal as staff_is_principal,s.is_provider as staff_is_provider, s.is_external as staff_is_external,
                                s.staff_date_added as staff_staff_date_added,s.start_date as staff_start_date,s.end_date as staff_end_date,s.comment as staff_comment,
                                s.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen, s.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                                s.bk_screen_field_id as staff_bk_screen_field_id, s.bk_screen_show_key as staff_bk_screen_show_key, s.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, s.enable_daily_reminder_email as staff_enable_daily_reminder_email, 

                                " + PersonDB.GetFields("person_", "p") + @",
                                t.title_id as title_title_id, t.descr as title_descr
                       FROM     
                                CreditNote 
                                LEFT OUTER JOIN Staff  s           ON CreditNote.staff_id = s.staff_id
                                LEFT OUTER JOIN Person p           ON s.person_id         = p.person_id
                                LEFT OUTER JOIN Title  t           ON p.title_id          = t.title_id ";


    public static CreditNote[] GetByInvoiceIDs(int[] invoice_ids)
    {
        string sql = JoinedSql + (invoice_ids != null && invoice_ids.Length > 0 ? " WHERE invoice_id IN (" + string.Join(",", invoice_ids) + ")" : " WHERE 1<>1");
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        CreditNote[] ret = new CreditNote[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            ret[i] = LoadAll(tbl.Rows[i]);

        return ret;
    }

    public static Hashtable GetBulkCreditNotesByInvoiceID(Invoice[] invoices)
    {
        Hashtable hash = new Hashtable();

        int[] invoiceIDs = new int[invoices.Length];
        for (int i = 0; i < invoices.Length; i++)
            invoiceIDs[i] = invoices[i].InvoiceID;
        CreditNote[] allcurCreditNotes = CreditNoteDB.GetByInvoiceIDs(invoiceIDs);

        foreach (Invoice curInvoice in invoices)
        {
            ArrayList curCreditNotes = new ArrayList();
            for (int i = 0; i < allcurCreditNotes.Length; i++)
            {
                if (allcurCreditNotes[i].Invoice.InvoiceID == curInvoice.InvoiceID)
                    curCreditNotes.Add(allcurCreditNotes[i]);
            }

            hash[curInvoice.InvoiceID] = (CreditNote[])curCreditNotes.ToArray(typeof(CreditNote));
        }

        return hash;
    }

    public static CreditNote LoadAll(DataRow row)
    {
        CreditNote creditNote = Load(row);

        creditNote.Staff = StaffDB.Load(row, "staff_");
        creditNote.Staff.Person = PersonDB.Load(row, "person_");
        creditNote.Staff.Person.Title = IDandDescrDB.Load(row, "title_title_id", "title_descr");

        return creditNote;
    }

    public static CreditNote Load(DataRow row)
    {
        return new CreditNote(
            Convert.ToInt32(row["creditnote_id"]),
            Convert.ToInt32(row["invoice_id"]),
            Convert.ToDecimal(row["total"]),
            Convert.ToString(row["reason"]),
            Convert.ToDateTime(row["credit_note_date_added"]),
            Convert.ToInt32(row["staff_id"]),
            row["reversed_by"]   == DBNull.Value ? -1 : Convert.ToInt32(row["reversed_by"]),
            row["reversed_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["reversed_date"]),
            Convert.ToDecimal(row["pre_reversed_amount"])
        );
    }

}