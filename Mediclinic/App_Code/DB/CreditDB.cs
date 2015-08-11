using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class CreditDB
{

    public static void Delete(int credit_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Credit WHERE credit_id = " + credit_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    protected static int Insert(int credit_type_id, int entity_id, decimal amount, string voucher_descr, DateTime expiry_date, int voucher_credit_id, int invoice_id, int tyro_payment_pending_id, int added_by)
    {
        voucher_descr = voucher_descr.Replace("'", "''");
        string sql = "INSERT INTO Credit (credit_type_id,entity_id,amount,voucher_descr,expiry_date,voucher_credit_id,invoice_id,tyro_payment_pending_id,added_by,date_added,deleted_by,date_deleted,pre_deleted_amount,modified_by,date_modified) VALUES (" + "" + credit_type_id + "," + (entity_id == -1 ? "NULL" : entity_id.ToString()) + "," + "" + amount + "," + "'" + voucher_descr + "'," + (expiry_date == DateTime.MinValue ? "NULL" : "'" + expiry_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (voucher_credit_id == -1 ? "NULL" : voucher_credit_id.ToString()) + "," + (invoice_id == -1 ? "NULL" : invoice_id.ToString()) + "," + (tyro_payment_pending_id == -1 ? "NULL" : tyro_payment_pending_id.ToString()) + "," + "" + added_by + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', NULL, NULL, 0, NULL, NULL);SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static int Insert_AddVoucher(int entity_id, decimal amount, string voucher_descr, DateTime expiry_date, int added_by)
    {
        return Insert(1, entity_id, amount, voucher_descr, expiry_date, -1, -1, -1, added_by);
    }
    public static int Insert_UseVoucher(int entity_id, decimal amount, int voucher_credit_id, int invoice_id, int added_by)
    {
        Credit credit = GetByID(voucher_credit_id);
        decimal totalUsed = -1 * GetTotalUsed(voucher_credit_id);
        if ((totalUsed + amount) > credit.Amount)
            throw new CustomMessageException("Not Enough Available Credit For Voucher: " + credit.VoucherDescr + ".<br />Total: " + (credit.Amount) + ". Used: " + totalUsed + ". Remaining: " + (credit.Amount - totalUsed));

        return Insert(2, entity_id, -1 * amount, string.Empty, DateTime.MinValue, voucher_credit_id, invoice_id, -1, added_by);
    }
    public static Tuple<int, int> Insert_Cashout(decimal amount, int tyro_payment_pending_id, int added_by)
    {
        int CashoutTyroToMediclinic = Insert(3, -1,      amount, string.Empty, DateTime.MinValue, -1, -1, tyro_payment_pending_id, added_by);
        int CashoutMediclincToPT    = Insert(4, -1, -1 * amount, string.Empty, DateTime.MinValue, -1, -1, tyro_payment_pending_id, added_by);
        return new Tuple<int, int>(CashoutTyroToMediclinic, CashoutMediclincToPT);
    }

    public static void Update(int credit_id, int credit_type_id, int entity_id, decimal amount, string voucher_descr, DateTime expiry_date, int voucher_credit_id, int invoice_id, int tyro_payment_pending_id, int modified_by)
    {
        Credit credit = GetByID(credit_id);
        if (credit.CreditType.ID == 1)
        {
            decimal totalUsed = -1 * GetTotalUsed(credit_id);
            if (totalUsed > amount)
                throw new CustomMessageException("$" + totalUsed + " Has Been Used From This Voucher.<br />To Set It Less Than That, You Need To Reverse Payments From This Voucher In The Invoice(s).");
        }

        CreditHistoryDB.Insert(
            credit.CreditID,
            credit.CreditType.ID,
            credit.Amount,
            credit.VoucherDescr,
            credit.ExpiryDate,
            credit.VoucherCredit == null ? -1 : credit.VoucherCredit.CreditID,
            credit.InvoiceID,
            credit.TyroPaymentPendingID,
            credit.AddedBy.StaffID,
            credit.DateAdded,
            credit.DeletedBy == null ? -1 : credit.DeletedBy.StaffID,
            credit.DateDeleted,
            credit.PreDeletedAmount,
            credit.ModifiedBy == null ? -1 : credit.ModifiedBy.StaffID,
            credit.DateModified);

        voucher_descr = voucher_descr.Replace("'", "''");
        string sql = "UPDATE Credit SET credit_type_id = " + credit_type_id + ",entity_id = " + entity_id + ",amount = " + amount + ",voucher_descr = '" + voucher_descr + "', expiry_date = " + (expiry_date == DateTime.MinValue ? "NULL" : "'" + expiry_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",voucher_credit_id = " + (voucher_credit_id == -1 ? "NULL" : voucher_credit_id.ToString()) + ",invoice_id = " + (invoice_id == -1 ? "NULL" : invoice_id.ToString()) + ",tyro_payment_pending_id = " + (tyro_payment_pending_id == -1 ? "NULL" : tyro_payment_pending_id.ToString()) + ",modified_by = " + (modified_by == -1 ? "NULL" : modified_by.ToString()) + ",date_modified = " + ("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE credit_id = " + credit_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void SetAsDeleted(int credit_id, int deleted_by)
    {
        Credit credit = GetByID(credit_id);
        if (credit == null)
            throw new CustomMessageException("Voucher Use - does not exist");
        if (credit.IsDeleted)
            throw new CustomMessageException("Voucher Use already deleted");

        decimal totalUsed = -1 * GetTotalUsed(credit_id);
        if (totalUsed > 0)
            throw new CustomMessageException("You Must Reverse Credit That Has Used This Voucher Before You Can Delete It.");

        CreditHistoryDB.Insert(
            credit.CreditID,
            credit.CreditType.ID,
            credit.Amount,
            credit.VoucherDescr,
            credit.ExpiryDate,
            credit.VoucherCredit == null ? -1 : credit.VoucherCredit.CreditID,
            credit.InvoiceID,
            credit.TyroPaymentPendingID,
            credit.AddedBy.StaffID,
            credit.DateAdded,
            credit.DeletedBy == null ? -1 : credit.DeletedBy.StaffID,
            credit.DateDeleted,
            credit.PreDeletedAmount,
            credit.ModifiedBy == null ? -1 : credit.ModifiedBy.StaffID,
            credit.DateModified);

        string sql = "UPDATE Credit SET amount = 0, deleted_by = " + deleted_by.ToString() + ",date_deleted = " + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",pre_deleted_amount = " + credit.Amount + ",modified_by = " + deleted_by + ",date_modified = " + ("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE credit_id = " + credit_id.ToString();
        DBBase.ExecuteNonResult(sql);

        if (credit.InvoiceID != -1)
        {
            Invoice invoice = InvoiceDB.GetByID(credit.InvoiceID);
            InvoiceDB.UpdateIsPaid(null, credit.InvoiceID, invoice.TotalDue <= 0);
        }
    }
    public static void UnDelete(int credit_id)  // If exceptions thrown when deleting multiple invoices of a booking, need to un-delete it back to being in use
    {
        Credit credit = CreditDB.GetByID(credit_id);
        if (credit == null)
            throw new CustomMessageException("Invalid credit id :" + credit.CreditID);
        if (!credit.IsDeleted)
            throw new CustomMessageException("Credit can not be un-deleted because it is not deleted. ID :" + credit.CreditID);

        CreditHistoryDB.Insert(
            credit.CreditID,
            credit.CreditType.ID,
            credit.Amount,
            credit.VoucherDescr,
            credit.ExpiryDate,
            credit.VoucherCredit == null ? -1 : credit.VoucherCredit.CreditID,
            credit.InvoiceID,
            credit.TyroPaymentPendingID,
            credit.AddedBy.StaffID,
            credit.DateAdded,
            credit.DeletedBy == null ? -1 : credit.DeletedBy.StaffID,
            credit.DateDeleted,
            credit.PreDeletedAmount,
            credit.ModifiedBy == null ? -1 : credit.ModifiedBy.StaffID,
            credit.DateModified);

        // set total=0, set not overpaid, set who and when it was reversed, and original amount
        string sql = "UPDATE Credit SET amount = " + credit.PreDeletedAmount + ", pre_deleted_amount = 0,  deleted_by = NULL, date_deleted = NULL" + ",modified_by = " + credit.ModifiedBy.StaffID + ",date_modified = " + ("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE credit_id = " + credit_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    
    public static DataTable GetDataTable()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Credit GetByID(int credit_id)
    {
        string sql = JoinedSql + " WHERE credit.credit_id = " + credit_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static DataTable GetDataTable_ByEntityID(int entity_id, string credit_type_ids = null, bool inc_deleted = false)
    {
        string sql = JoinedSql + " WHERE credit.entity_id = " + entity_id.ToString() + (credit_type_ids != null && credit_type_ids.Length > 0 ? " AND credit.credit_type_id IN (" + credit_type_ids + ") " : string.Empty) + (inc_deleted ? "" : " AND credit.deleted_by IS NULL AND credit.date_deleted IS NULL ");
        return DBBase.ExecuteQuery(sql).Tables[0];
    }


    public static Credit[] GetByInvoiceID(int invoice_id)
    {
        DataTable tbl = GetDataTable_ByInvoiceID(invoice_id);

        Credit[] list = new Credit[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = CreditDB.LoadAll(tbl.Rows[i]);

        return list;
    }

    public static DataTable GetDataTable_ByInvoiceID(int invoice_id)
    {
        string sql = JoinedSql + " WHERE credit.credit_type_id = 2 AND credit.invoice_id = " + invoice_id;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Credit[] GetByVoucherCreditID(int voucher_entity_id, bool inc_deleted = false)
    {
        DataTable tbl = GetDataTable_ByVoucherCreditID(voucher_entity_id, inc_deleted);

        Credit[] list = new Credit[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = CreditDB.LoadAll(tbl.Rows[i]);

        return list;
    }

    public static DataTable GetDataTable_ByVoucherCreditID(int voucher_entity_id, bool inc_deleted = false)
    {
        string sql = JoinedSql + " WHERE credit.voucher_credit_id = " + voucher_entity_id + (inc_deleted ? "" : " AND credit.deleted_by IS NULL AND credit.date_deleted IS NULL ");
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Credit[] GetByEntityID(int entity_id, string credit_type_ids = null, bool inc_deleted = false)
    {
        DataTable tbl = GetDataTable_ByEntityID(entity_id, credit_type_ids, inc_deleted);

        Credit[] list = new Credit[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = CreditDB.LoadAll(tbl.Rows[i]);

        return list;
    }

    public static decimal GetTotal(int entity_id, string credit_type_ids = null)
    {
        string sql = "SELECT ISNULL(SUM(amount),0) FROM Credit " + (credit_type_ids != null && credit_type_ids.Length > 0 ? " WHERE credit_type_id IN (" + credit_type_ids + ")" : string.Empty);
        return Convert.ToDecimal(DBBase.ExecuteSingleResult(sql));
    }

    public static decimal GetTotalUsed(int credit_id)
    {
        string sql = "SELECT ISNULL(SUM(amount),0) FROM Credit WHERE credit_type_id = 2 AND voucher_credit_id = " + credit_id;
        return Convert.ToDecimal(DBBase.ExecuteSingleResult(sql));
    }

    public static DataTable GetUnusedVouchers(int entity_id)
    {
        string sql = JoinedSql + " WHERE credit.entity_id = " + entity_id.ToString() + 
            " AND credit.credit_type_id = 1" +
            " AND (credit.expiry_date IS NULL OR CONVERT(date, credit.expiry_date) >= CONVERT(date, getdate()))" +
            " AND credit.amount > ((SELECT -1 * ISNULL(SUM(amount),0) FROM Credit c2 WHERE c2.credit_type_id = 2 AND c2.voucher_credit_id = credit.credit_id))";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }


    #region JoinedSQL

    public static string JoinedSql = @"

                SELECT   
                        credit.credit_id                as credit_credit_id,
                        credit.credit_type_id           as credit_credit_type_id,
                        credit.entity_id                as credit_entity_id,
                        credit.amount                   as credit_amount,
                        credit.voucher_descr            as credit_voucher_descr,
                        credit.expiry_date              as credit_expiry_date,
                        credit.voucher_credit_id        as credit_voucher_credit_id,
                        credit.invoice_id               as credit_invoice_id,
                        credit.tyro_payment_pending_id  as credit_tyro_payment_pending_id,
                        credit.added_by                 as credit_added_by,
                        credit.date_added               as credit_date_added,
                        credit.deleted_by               as credit_deleted_by,
                        credit.date_deleted	            as credit_date_deleted,
                        credit.pre_deleted_amount       as credit_pre_deleted_amount,
                        credit.modified_by              as credit_modified_by,
                        credit.date_modified            as credit_date_modified
                        ,
                        credittype.credit_type_id as credittype_credit_type_id, credittype.descr as credittype_descr
                        ,

                        (SELECT -1 * ISNULL(SUM(amount),0) FROM Credit c2 WHERE c2.credit_type_id = 2 AND c2.voucher_credit_id = credit.credit_id) as credit_amount_used
                        ,

                        vouchercredit.credit_id               as vouchercredit_credit_id,
                        vouchercredit.credit_type_id          as vouchercredit_credit_type_id,
                        vouchercredit.entity_id               as vouchercredit_entity_id,
                        vouchercredit.amount                  as vouchercredit_amount,
                        vouchercredit.voucher_descr           as vouchercredit_voucher_descr,
                        vouchercredit.expiry_date             as vouchercredit_expiry_date,
                        vouchercredit.voucher_credit_id       as vouchercredit_voucher_credit_id,
                        vouchercredit.invoice_id              as vouchercredit_invoice_id,
                        vouchercredit.tyro_payment_pending_id as vouchercredit_tyro_payment_pending_id,
                        vouchercredit.added_by                as vouchercredit_added_by,
                        vouchercredit.date_added              as vouchercredit_date_added,
                        vouchercredit.deleted_by              as vouchercredit_deleted_by,
                        vouchercredit.date_deleted            as vouchercredit_date_deleted,
                        vouchercredit.pre_deleted_amount      as vouchercredit_pre_deleted_amount,
                        vouchercredit.modified_by             as vouchercredit_modified_by,
                        vouchercredit.date_modified           as vouchercredit_date_modified
                        ,
                        vouchercredittype.credit_type_id as vouchercredittype_credit_type_id, vouchercredittype.descr as vouchercredittype_descr
                        ,

                        (SELECT -1 * ISNULL(SUM(amount),0) FROM Credit c2 WHERE c2.credit_type_id = 2 AND c2.voucher_credit_id = credit.voucher_credit_id) as vouchercredit_amount_used
                        ,

                        added_by.staff_id as added_by_staff_id, added_by.person_id as added_by_person_id, added_by.login as added_by_login, added_by.pwd as added_by_pwd, 
                        added_by.staff_position_id as added_by_staff_position_id, added_by.field_id as added_by_field_id, added_by.costcentre_id as added_by_costcentre_id, 
                        added_by.is_contractor as added_by_is_contractor, added_by.tfn as added_by_tfn, added_by.provider_number as added_by_provider_number, 
                        added_by.is_fired as added_by_is_fired, added_by.is_commission as added_by_is_commission, added_by.commission_percent as added_by_commission_percent, 
                        added_by.is_stakeholder as added_by_is_stakeholder, added_by.is_master_admin as added_by_is_master_admin, added_by.is_admin as added_by_is_admin, added_by.is_principal as added_by_is_principal, added_by.is_provider as added_by_is_provider, added_by.is_external as added_by_is_external,
                        added_by.staff_date_added as added_by_staff_date_added, added_by.start_date as added_by_start_date, added_by.end_date as added_by_end_date, added_by.comment as added_by_comment, 
                        added_by.num_days_to_display_on_booking_screen as added_by_num_days_to_display_on_booking_screen,  added_by.show_header_on_booking_screen as added_by_show_header_on_booking_screen,
                        added_by.bk_screen_field_id as added_by_bk_screen_field_id, added_by.bk_screen_show_key as added_by_bk_screen_show_key, added_by.enable_daily_reminder_sms as added_by_enable_daily_reminder_sms, added_by.enable_daily_reminder_email as added_by_enable_daily_reminder_email
                        ,
                        " + PersonDB.GetFields("person_added_by_", "person_added_by") + @"
                        ,
                        title_added_by.title_id as title_added_by_title_id, title_added_by.descr as title_added_by_descr
                        ,
                        deleted_by.staff_id as deleted_by_staff_id, deleted_by.person_id as deleted_by_person_id, deleted_by.login as deleted_by_login, deleted_by.pwd as deleted_by_pwd, 
                        deleted_by.staff_position_id as deleted_by_staff_position_id, deleted_by.field_id as deleted_by_field_id, deleted_by.costcentre_id as deleted_by_costcentre_id, 
                        deleted_by.is_contractor as deleted_by_is_contractor, deleted_by.tfn as deleted_by_tfn, deleted_by.provider_number as deleted_by_provider_number, 
                        deleted_by.is_fired as deleted_by_is_fired, deleted_by.is_commission as deleted_by_is_commission, deleted_by.commission_percent as deleted_by_commission_percent, 
                        deleted_by.is_stakeholder as deleted_by_is_stakeholder, deleted_by.is_master_admin as deleted_by_is_master_admin, deleted_by.is_admin as deleted_by_is_admin, deleted_by.is_principal as deleted_by_is_principal, deleted_by.is_provider as deleted_by_is_provider, deleted_by.is_external as deleted_by_is_external,
                        deleted_by.staff_date_added as deleted_by_staff_date_added, deleted_by.start_date as deleted_by_start_date, deleted_by.end_date as deleted_by_end_date, deleted_by.comment as deleted_by_comment, 
                        deleted_by.num_days_to_display_on_booking_screen as deleted_by_num_days_to_display_on_booking_screen,  deleted_by.show_header_on_booking_screen as deleted_by_show_header_on_booking_screen,
                        deleted_by.bk_screen_field_id as deleted_by_bk_screen_field_id, deleted_by.bk_screen_show_key as deleted_by_bk_screen_show_key, deleted_by.enable_daily_reminder_sms as deleted_by_enable_daily_reminder_sms, deleted_by.enable_daily_reminder_email as deleted_by_enable_daily_reminder_email
                        ,
                        " + PersonDB.GetFields("person_deleted_by_", "person_deleted_by") + @"
                        ,
                        title_deleted_by.title_id as title_deleted_by_title_id, title_deleted_by.descr as title_deleted_by_descr
                        ,
                        modified_by.staff_id as modified_by_staff_id, modified_by.person_id as modified_by_person_id, modified_by.login as modified_by_login, modified_by.pwd as modified_by_pwd, 
                        modified_by.staff_position_id as modified_by_staff_position_id, modified_by.field_id as modified_by_field_id, modified_by.costcentre_id as modified_by_costcentre_id, 
                        modified_by.is_contractor as modified_by_is_contractor, modified_by.tfn as modified_by_tfn, modified_by.provider_number as modified_by_provider_number, 
                        modified_by.is_fired as modified_by_is_fired, modified_by.is_commission as modified_by_is_commission, modified_by.commission_percent as modified_by_commission_percent, 
                        modified_by.is_stakeholder as modified_by_is_stakeholder, modified_by.is_master_admin as modified_by_is_master_admin, modified_by.is_admin as modified_by_is_admin, modified_by.is_principal as modified_by_is_principal, modified_by.is_provider as modified_by_is_provider, modified_by.is_external as modified_by_is_external,
                        modified_by.staff_date_added as modified_by_staff_date_added, modified_by.start_date as modified_by_start_date, modified_by.end_date as modified_by_end_date, modified_by.comment as modified_by_comment, 
                        modified_by.num_days_to_display_on_booking_screen as modified_by_num_days_to_display_on_booking_screen,  modified_by.show_header_on_booking_screen as modified_by_show_header_on_booking_screen,
                        modified_by.bk_screen_field_id as modified_by_bk_screen_field_id, modified_by.bk_screen_show_key as modified_by_bk_screen_show_key, modified_by.enable_daily_reminder_sms as modified_by_enable_daily_reminder_sms, modified_by.enable_daily_reminder_email as modified_by_enable_daily_reminder_email
                        ,
                        " + PersonDB.GetFields("person_modified_by_", "person_modified_by") + @"
                        ,
                        title_modified_by.title_id as title_modified_by_title_id, title_modified_by.descr as title_modified_by_descr
        
        
                    FROM 
                            Credit credit
                            LEFT JOIN CreditType credittype               ON credittype.credit_type_id        = credit.credit_type_id

                            LEFT JOIN Credit     vouchercredit            ON vouchercredit.credit_id          = credit.voucher_credit_id
                            LEFT JOIN CreditType vouchercredittype        ON vouchercredittype.credit_type_id = vouchercredit.credit_type_id

                            LEFT JOIN Staff      added_by                 ON added_by.staff_id                = credit.added_by
                            LEFT JOIN Person     person_added_by          ON person_added_by.person_id        = added_by.person_id
                            LEFT JOIN Title      title_added_by           ON title_added_by.title_id          = person_added_by.title_id
                            LEFT JOIN Staff      deleted_by               ON deleted_by.staff_id              = credit.deleted_by
                            LEFT JOIN Person     person_deleted_by        ON person_deleted_by.person_id      = deleted_by.person_id
                            LEFT JOIN Title      title_deleted_by         ON title_deleted_by.title_id        = person_deleted_by.title_id 
                            LEFT JOIN Staff      modified_by              ON modified_by.staff_id             = credit.modified_by
                            LEFT JOIN Person     person_modified_by       ON person_modified_by.person_id     = modified_by.person_id
                            LEFT JOIN Title      title_modified_by        ON title_modified_by.title_id       = person_modified_by.title_id ";

    #endregion

    public static Credit LoadAll(DataRow row)
    {
        Credit credit = Load(row, "credit_");
        credit.CreditType = new IDandDescr(Convert.ToInt32(row["credittype_credit_type_id"]), Convert.ToString(row["credittype_descr"]));

        if (row["vouchercredit_credit_id"] != DBNull.Value)
        {
            credit.VoucherCredit = CreditDB.Load(row, "vouchercredit_");
            credit.VoucherCredit.CreditType = new IDandDescr(Convert.ToInt32(row["vouchercredittype_credit_type_id"]), Convert.ToString(row["vouchercredittype_descr"]));
        }

        if (row["added_by_staff_id"] != DBNull.Value)
            credit.AddedBy = StaffDB.Load(row, "added_by_");
        if (row["person_added_by_person_id"] != DBNull.Value)
        {
            credit.AddedBy.Person = PersonDB.Load(row, "person_added_by_");
            credit.AddedBy.Person.Title = IDandDescrDB.Load(row, "title_added_by_title_id", "title_added_by_descr");
        }

        if (row["deleted_by_staff_id"] != DBNull.Value)
            credit.DeletedBy = StaffDB.Load(row, "deleted_by_");
        if (row["person_deleted_by_person_id"] != DBNull.Value)
        {
            credit.DeletedBy.Person.Title = IDandDescrDB.Load(row, "title_deleted_by_title_id", "title_deleted_by_descr");
            credit.DeletedBy.Person = PersonDB.Load(row, "person_deleted_by_");
        }

        if (row["modified_by_staff_id"] != DBNull.Value)
            credit.ModifiedBy = StaffDB.Load(row, "modified_by_");
        if (row["person_modified_by_person_id"] != DBNull.Value)
        {
            credit.ModifiedBy.Person.Title = IDandDescrDB.Load(row, "title_modified_by_title_id", "title_modified_by_descr");
            credit.ModifiedBy.Person = PersonDB.Load(row, "person_modified_by_");
        }

        return credit;
    }

    public static Credit Load(DataRow row, string prefix = "")
    {
        return new Credit(
            Convert.ToInt32(row[prefix + "credit_id"]),
            Convert.ToInt32(row[prefix + "credit_type_id"]),
            Convert.ToInt32(row[prefix + "entity_id"]),
            Convert.ToDecimal(row[prefix + "amount"]),
            row.Table.Columns.Contains(prefix + "amount_used") ? Convert.ToDecimal(row[prefix + "amount_used"]) : 0,
            Convert.ToString(row[prefix + "voucher_descr"]),
            row[prefix + "expiry_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "expiry_date"]),
            row[prefix + "credit_id"]   == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "credit_id"]),
            row[prefix + "invoice_id"]              == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "invoice_id"]),
            row[prefix + "tyro_payment_pending_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "tyro_payment_pending_id"]),
            Convert.ToInt32(row[prefix + "added_by"]),
            Convert.ToDateTime(row[prefix + "date_added"]),
            row[prefix + "deleted_by"]              == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "deleted_by"]),
            row[prefix + "date_deleted"]            == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_deleted"]),
            Convert.ToDecimal(row[prefix + "pre_deleted_amount"]),
            row[prefix + "modified_by"]             == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "modified_by"]),
            row[prefix + "date_modified"]           == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_modified"])
        );
    }

}