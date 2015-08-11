using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class BulkLetterSendingQueueBatchDB
{

    public static void Delete(int bulk_letter_sending_queue_batch_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM BulkLetterSendingQueueBatch WHERE bulk_letter_sending_queue_batch_id = " + bulk_letter_sending_queue_batch_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(string email_address_to_send_printed_letters_to, bool ready_to_process)
    {
        email_address_to_send_printed_letters_to = email_address_to_send_printed_letters_to.Replace("'", "''");
        string sql = "INSERT INTO BulkLetterSendingQueueBatch (email_address_to_send_printed_letters_to,ready_to_process) VALUES (" + "'" + email_address_to_send_printed_letters_to + "'," + (ready_to_process ? "1" : "0") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int bulk_letter_sending_queue_batch_id, string email_address_to_send_printed_letters_to, bool ready_to_process)
    {
        email_address_to_send_printed_letters_to = email_address_to_send_printed_letters_to.Replace("'", "''");
        string sql = "UPDATE BulkLetterSendingQueueBatch SET email_address_to_send_printed_letters_to = '" + email_address_to_send_printed_letters_to + "',ready_to_process = " + (ready_to_process ? "1" : "0") + " WHERE bulk_letter_sending_queue_batch_id = " + bulk_letter_sending_queue_batch_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateReadyToProcess(int bulk_letter_sending_queue_batch_id, bool ready_to_process)
    {
        string sql = "UPDATE BulkLetterSendingQueueBatch SET ready_to_process = " + (ready_to_process ? "1" : "0") + " WHERE bulk_letter_sending_queue_batch_id = " + bulk_letter_sending_queue_batch_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT bulk_letter_sending_queue_batch_id,email_address_to_send_printed_letters_to,ready_to_process FROM BulkLetterSendingQueueBatch";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static BulkLetterSendingQueueBatch GetByID(int bulk_letter_sending_queue_batch_id)
    {
        string sql = "SELECT bulk_letter_sending_queue_batch_id,email_address_to_send_printed_letters_to,ready_to_process FROM BulkLetterSendingQueueBatch WHERE bulk_letter_sending_queue_batch_id = " + bulk_letter_sending_queue_batch_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static BulkLetterSendingQueueBatch Load(DataRow row)
    {
        return new BulkLetterSendingQueueBatch(
            Convert.ToInt32(row["bulk_letter_sending_queue_batch_id"]),
            Convert.ToString(row["email_address_to_send_printed_letters_to"]),
            Convert.ToBoolean(row["ready_to_process"])
        );
    }

}