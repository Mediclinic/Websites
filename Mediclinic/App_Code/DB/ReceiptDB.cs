using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class ReceiptDB
{

    public static void Delete(int receipt_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Receipt WHERE receipt_id = " + receipt_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string DB, int receipt_payment_type_id, int invoice_id, decimal total, decimal amount_reconciled, bool is_failed_to_clear, bool is_overpaid, DateTime reconciliation_date, int staff_id)
    {
        string sql = "INSERT INTO Receipt (receipt_payment_type_id,invoice_id,total,amount_reconciled,is_failed_to_clear,is_overpaid,receipt_date_added,reconciliation_date,staff_id,reversed_by,reversed_date,pre_reversed_amount) VALUES (" + "" + receipt_payment_type_id + "," + "" + invoice_id + "," + "" + total + "," + "" + amount_reconciled + "," + (is_failed_to_clear ? "1" : "0") + "," + (is_overpaid ? "1" : "0") + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + (reconciliation_date == DateTime.MinValue ? "NULL" : "'" + reconciliation_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + "" + staff_id + "," + "NULL" + "," + "NULL" + "," + "0.00" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql, DB));
    }

    public static void Update(int receipt_id, int receipt_payment_type_id, decimal total, decimal amount_reconciled, bool is_failed_to_clear, bool is_overpaid, DateTime reconciliation_date, int reversed_by, DateTime reversed_date, decimal pre_reversed_amount)
    {
        string sql = "UPDATE Receipt SET receipt_payment_type_id = " + receipt_payment_type_id + ",total = " + total + ",amount_reconciled = " + amount_reconciled + ",is_failed_to_clear = " + (is_failed_to_clear ? "1" : "0") + ",is_overpaid = " + (is_overpaid ? "1" : "0") + ",reconciliation_date = " + (reconciliation_date == DateTime.MinValue ? "NULL" : "'" + reconciliation_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",reversed_by = " + (reversed_by == -1 ? "NULL" : reversed_by.ToString()) + ",reversed_date = " + (reversed_date == DateTime.MinValue ? "NULL" : "'" + reversed_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",pre_reversed_amount = " + pre_reversed_amount + " WHERE receipt_id = " + receipt_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void SetReconciled(int receipt_id)
    {
        string sql = "UPDATE Receipt SET amount_reconciled = total WHERE receipt_id = " + receipt_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void Reverse(int receipt_id, int reversed_by)
    {
        Receipt receipt = ReceiptDB.GetByID(receipt_id);
        if (receipt == null)
            throw new ArgumentException("Invalid receipt id :" + receipt_id);
        if (receipt.IsReversed)
            throw new ArgumentException("Receipt already reversed");
        if (receipt.IsReconciled)
            throw new ArgumentException("Can not reverse a receipt that has been reconciled");


        // remove any overpayment records for this receipt
        OverpaymentDB.DeleteByReceiptID(receipt_id);

        // set total=0, set not overpaid, set who and when it was reversed, and original amount
        string sql = "UPDATE Receipt SET total = 0, is_overpaid = 0 ,reversed_by = " + reversed_by + ",reversed_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',pre_reversed_amount = " + receipt.Total + " WHERE receipt_id = " + receipt_id.ToString();
        DBBase.ExecuteNonResult(sql);

        // set invoice as not paid
        InvoiceDB.UpdateIsPaid(null, receipt.Invoice.InvoiceID, false);

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
    public static Receipt[] GetByInvoice(int invoice_id, bool incReversed = true)
    {
        DataTable tbl  = GetDataTableByInvoice(invoice_id);
        ArrayList list = new ArrayList();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Receipt receipt = LoadAll(tbl.Rows[i]);
            if (incReversed || !receipt.IsReversed)
                list.Add(receipt);
        }
        return (Receipt[])list.ToArray(typeof(Receipt));;
    }



    public static Receipt GetByID(int receipt_id)
    {
        string sql = JoinedSql  + " WHERE receipt_id = " + receipt_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static string JoinedSql = @"
                       SELECT   
                                Receipt.receipt_id, Receipt.receipt_payment_type_id, Receipt.invoice_id,Receipt.total, Receipt.amount_reconciled, Receipt.is_failed_to_clear,
                                Receipt.is_overpaid, Receipt.receipt_date_added, Receipt.reconciliation_date, Receipt.staff_id, Receipt.reversed_by, Receipt.reversed_date, Receipt.pre_reversed_amount,
                                ReceiptPaymentType.receipt_payment_type_id, ReceiptPaymentType.descr,

                                s.staff_id as staff_staff_id,s.person_id as staff_person_id,s.login as staff_login,s.pwd as staff_pwd,s.staff_position_id as staff_staff_position_id,
                                s.field_id as staff_field_id,s.is_contractor as staff_is_contractor,s.tfn as staff_tfn,
                                s.is_fired as staff_is_fired,s.costcentre_id as staff_costcentre_id,
                                s.is_admin as staff_is_admin,s.provider_number as staff_provider_number,s.is_commission as staff_is_commission,s.commission_percent as staff_commission_percent,
                                s.is_stakeholder as staff_is_stakeholder,s.is_master_admin as staff_is_master_admin,s.is_admin as staff_is_admin,s.is_principal as staff_is_principal,s.is_provider as staff_is_provider, s.is_external as staff_is_external,
                                s.staff_date_added as staff_staff_date_added,s.start_date as staff_start_date,s.end_date as staff_end_date,s.comment as staff_comment,
                                s.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen, 
                                s.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                                s.bk_screen_field_id as staff_bk_screen_field_id,
                                s.bk_screen_show_key as staff_bk_screen_show_key,
                                s.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                                s.enable_daily_reminder_email as staff_enable_daily_reminder_email,

                                " + PersonDB.GetFields("person_", "p") + @",
                                t.title_id as title_title_id, t.descr as title_descr
                       FROM     
                                Receipt
                                LEFT OUTER JOIN Staff  s           ON Receipt.staff_id = s.staff_id
                                LEFT OUTER JOIN Person p           ON s.person_id      = p.person_id
                                LEFT OUTER JOIN Title  t           ON p.title_id       = t.title_id
                                INNER      JOIN ReceiptPaymentType ON Receipt.receipt_payment_type_id = ReceiptPaymentType.receipt_payment_type_id 

";


    public static Receipt[] GetByInvoiceIDs(int[] invoice_ids)
    {
        string sql = JoinedSql + (invoice_ids != null && invoice_ids.Length > 0 ? " WHERE invoice_id IN (" + string.Join(",", invoice_ids) + ")" : " WHERE 1<>1");
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Receipt[] ret = new Receipt[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            ret[i] = LoadAll(tbl.Rows[i]);

        return ret;
    }

    public static Hashtable GetBulkReceiptsByInvoiceID(Invoice[] invoices)
    {
        Hashtable hash = new Hashtable();

        int[] invoiceIDs = new int[invoices.Length];
        for (int i = 0; i < invoices.Length; i++)
            invoiceIDs[i] = invoices[i].InvoiceID;
        Receipt[] allcurReceipts = ReceiptDB.GetByInvoiceIDs(invoiceIDs);

        foreach (Invoice curInvoice in invoices)
        {
            ArrayList curReceipts = new ArrayList();
            for (int i = 0; i < allcurReceipts.Length; i++)
            {
                if (allcurReceipts[i].Invoice.InvoiceID == curInvoice.InvoiceID)
                    curReceipts.Add(allcurReceipts[i]);
            }

            hash[curInvoice.InvoiceID] = (Receipt[])curReceipts.ToArray(typeof(Receipt));
        }

        return hash;
    }

    public static Receipt LoadAll(DataRow row)
    {
        Receipt receipt = Load(row);

        receipt.Staff              = StaffDB.Load(row, "staff_");
        receipt.Staff.Person       = PersonDB.Load(row, "person_");
        receipt.Staff.Person.Title = IDandDescrDB.Load(row, "title_title_id", "title_descr");

        receipt.ReceiptPaymentType = IDandDescrDB.Load(row, "receipt_payment_type_id", "descr");
        
        return receipt;
    }

    public static Receipt Load(DataRow row)
    {
        return new Receipt(
            Convert.ToInt32(row["receipt_id"]),
            Convert.ToInt32(row["receipt_payment_type_id"]),
            Convert.ToInt32(row["invoice_id"]),
            Convert.ToDecimal(row["total"]),
            Convert.ToDecimal(row["amount_reconciled"]),
            Convert.ToBoolean(row["is_failed_to_clear"]),
            Convert.ToBoolean(row["is_overpaid"]),
            Convert.ToDateTime(row["receipt_date_added"]),
            row["reconciliation_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["reconciliation_date"]),
            Convert.ToInt32(row["staff_id"]),
            row["reversed_by"]   == DBNull.Value ? -1 : Convert.ToInt32(row["reversed_by"]),
            row["reversed_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["reversed_date"]),
            Convert.ToDecimal(row["pre_reversed_amount"])
        );
    }

}