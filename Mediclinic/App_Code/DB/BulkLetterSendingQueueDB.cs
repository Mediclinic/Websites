using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class BulkLetterSendingQueueDB
{

    public static void Delete(int bulk_letter_sending_queue_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM BulkLetterSendingQueue WHERE bulk_letter_sending_queue_id = " + bulk_letter_sending_queue_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static void DeleteByBatchID(int bulk_letter_sending_queue_batch_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM BulkLetterSendingQueue WHERE bulk_letter_sending_queue_batch_id = " + bulk_letter_sending_queue_batch_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static int Insert(int bulk_letter_sending_queue_batch_id, int letter_print_history_send_method_id, int added_by, int patient_id, int referrer_id, int booking_id, string phone_number, string email_to_address, string email_to_name, string email_from_address, string email_from_name, string text, string email_subject, string email_attachment_location, bool email_attachment_delete_after_sending, bool email_attachment_folder_delete_after_sending, int email_letter_letter_id, bool email_letter_keep_history_in_db, bool email_letter_keep_history_in_file, int email_letter_letter_print_history_send_method_id, string email_letter_history_dir, string email_letter_history_filename, int email_letter_site_id, int email_letter_organisation_id, int email_letter_booking_id, int email_letter_patient_id, int email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref, int email_letter_staff_id, int email_letter_health_card_action_id, string email_letter_source_template_path, string email_letter_output_doc_path, bool email_letter_is_double_sided_printing, string email_letter_extra_pages, string email_letter_item_seperator, string sql_to_run_on_completion, string sql_to_run_on_failure)
    {
        phone_number = phone_number.Replace("'", "''");
        email_to_address = email_to_address.Replace("'", "''");
        email_to_name = email_to_name.Replace("'", "''");
        email_from_address = email_from_address.Replace("'", "''");
        email_from_name = email_from_name.Replace("'", "''");
        text = text.Replace("'", "''");
        email_subject = email_subject.Replace("'", "''");
        email_attachment_location = email_attachment_location.Replace("'", "''");
        email_letter_history_dir = email_letter_history_dir.Replace("'", "''");
        email_letter_history_filename = email_letter_history_filename.Replace("'", "''");
        email_letter_source_template_path = email_letter_source_template_path.Replace("'", "''");
        email_letter_output_doc_path = email_letter_output_doc_path.Replace("'", "''");
        email_letter_extra_pages = email_letter_extra_pages.Replace("'", "''");
        string sql = "INSERT INTO BulkLetterSendingQueue (bulk_letter_sending_queue_batch_id,letter_print_history_send_method_id,added_by,patient_id,referrer_id,booking_id,phone_number,email_to_address,email_to_name,email_from_address,email_from_name,text,email_subject,email_attachment_location,email_attachment_delete_after_sending,email_attachment_folder_delete_after_sending,email_letter_letter_id,email_letter_keep_history_in_db,email_letter_keep_history_in_file,email_letter_letter_print_history_send_method_id,email_letter_history_dir,email_letter_history_filename,email_letter_site_id,email_letter_organisation_id,email_letter_booking_id,email_letter_patient_id,email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref,email_letter_staff_id,email_letter_health_card_action_id,email_letter_source_template_path,email_letter_output_doc_path,email_letter_is_double_sided_printing,email_letter_extra_pages,email_letter_item_seperator,sql_to_run_on_completion,sql_to_run_on_failure,datetime_added,datetime_sending_start,datetime_sent) VALUES (" + "" + bulk_letter_sending_queue_batch_id + "," + "" + letter_print_history_send_method_id + "," + "" + (added_by == -1 ? "NULL" : added_by.ToString()) + "," + "" + (patient_id == -1 ? "NULL" : patient_id.ToString()) + "," + "" + (referrer_id == -1 ? "NULL" : referrer_id.ToString()) + "," + "" + (booking_id == -1 ? "NULL" : booking_id.ToString()) + "," + "'" + phone_number + "'," + "'" + email_to_address + "'," + "'" + email_to_name + "'," + "'" + email_from_address + "'," + "'" + email_from_name + "'," + "'" + text + "'," + "'" + email_subject + "'," + "'" + email_attachment_location + "'," + (email_attachment_delete_after_sending ? "1," : "0,") + (email_attachment_folder_delete_after_sending ? "1," : "0,") + "" + (email_letter_letter_id == -1 ? "NULL" : email_letter_letter_id.ToString()) + "," + (email_letter_keep_history_in_db ? "1," : "0,") + (email_letter_keep_history_in_file ? "1," : "0,") + "" + (email_letter_letter_print_history_send_method_id == -1 ? "NULL" : email_letter_letter_print_history_send_method_id.ToString()) + "," + "'" + email_letter_history_dir + "'," + "'" + email_letter_history_filename + "'," + "" + (email_letter_site_id == -1 ? "NULL" : email_letter_site_id.ToString()) + "," + "" + (email_letter_organisation_id == 0 ? "NULL" : email_letter_organisation_id.ToString()) + "," + "" + (email_letter_booking_id == -1 ? "NULL" : email_letter_booking_id.ToString()) + "," + "" + (email_letter_patient_id == -1 ? "NULL" : email_letter_patient_id.ToString()) + "," + "" + (email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref == -1 ? "NULL" : email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref.ToString()) + "," + "" + (email_letter_staff_id == -1 ? "NULL" : email_letter_staff_id.ToString()) + "," + "" + (email_letter_health_card_action_id == -1 ? "NULL" : email_letter_health_card_action_id.ToString()) + "," + "'" + email_letter_source_template_path + "'," + "'" + email_letter_output_doc_path + "'," + (email_letter_is_double_sided_printing ? "1," : "0,") + "'" + email_letter_extra_pages + "'," + "'" + email_letter_item_seperator + "'," + "'" + sql_to_run_on_completion + "'," + "'" + sql_to_run_on_failure + "'," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "NULL" + "," + "NULL" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int bulk_letter_sending_queue_id, int bulk_letter_sending_queue_batch_id, int letter_print_history_send_method_id, int added_by, int patient_id, int referrer_id, int booking_id, string phone_number, string email_to_address, string email_to_name, string email_from_address, string email_from_name, string text, string email_subject, string email_attachment_location, bool email_attachment_delete_after_sending, bool email_attachment_folder_delete_after_sending, int email_letter_letter_id, bool email_letter_keep_history_in_db, bool email_letter_keep_history_in_file, int email_letter_letter_print_history_send_method_id, string email_letter_history_dir, string email_letter_history_filename, int email_letter_site_id, int email_letter_organisation_id, int email_letter_booking_id, int email_letter_patient_id, int email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref, int email_letter_staff_id, int email_letter_health_card_action_id, string email_letter_source_template_path, string email_letter_output_doc_path, bool email_letter_is_double_sided_printing, string email_letter_extra_pages, string email_letter_item_seperator, string sql_to_run_on_completion, string sql_to_run_on_failure, DateTime datetime_sending_start, DateTime datetime_sent)
    {
        phone_number = phone_number.Replace("'", "''");
        email_to_address = email_to_address.Replace("'", "''");
        email_to_name = email_to_name.Replace("'", "''");
        email_from_address = email_from_address.Replace("'", "''");
        email_from_name = email_from_name.Replace("'", "''");
        text = text.Replace("'", "''");
        email_subject = email_subject.Replace("'", "''");
        email_attachment_location = email_attachment_location.Replace("'", "''");
        email_letter_history_dir = email_letter_history_dir.Replace("'", "''");
        email_letter_history_filename = email_letter_history_filename.Replace("'", "''");
        email_letter_source_template_path = email_letter_source_template_path.Replace("'", "''");
        email_letter_output_doc_path = email_letter_output_doc_path.Replace("'", "''");
        email_letter_extra_pages = email_letter_extra_pages.Replace("'", "''");
        string sql = "UPDATE BulkLetterSendingQueue SET bulk_letter_sending_queue_batch_id = " + bulk_letter_sending_queue_batch_id + ",letter_print_history_send_method_id = " + letter_print_history_send_method_id + ",added_by = " + (added_by == -1 ? "NULL" : added_by.ToString()) + ",patient_id = " + (patient_id == -1 ? "NULL" : patient_id.ToString()) + ",referrer_id = " + (referrer_id == -1 ? "NULL" : referrer_id.ToString()) + ",booking_id = " + (booking_id == -1 ? "NULL" : booking_id.ToString()) + ",phone_number = '" + phone_number + "',email_to_address = '" + email_to_address + "',email_to_name = '" + email_to_name + "',email_from_address = '" + email_from_address + "',email_from_name = '" + email_from_name + "',text = '" + text + "',email_subject = '" + email_subject + "',email_attachment_location = '" + email_attachment_location + "',email_attachment_delete_after_sending = " + (email_attachment_delete_after_sending ? "1," : "0,") + "email_attachment_folder_delete_after_sending = " + (email_attachment_folder_delete_after_sending ? "1," : "0,") + "email_letter_letter_id = " + (email_letter_letter_id == -1 ? "NULL" : email_letter_letter_id.ToString()) + ",email_letter_keep_history_in_db = " + (email_letter_keep_history_in_db ? "1," : "0,") + "email_letter_keep_history_in_file = " + (email_letter_keep_history_in_file ? "1," : "0,") + "email_letter_letter_print_history_send_method_id = " + (email_letter_letter_print_history_send_method_id == -1 ? "NULL" : email_letter_letter_print_history_send_method_id.ToString()) + ",email_letter_history_dir = '" + email_letter_history_dir + "',email_letter_history_filename = '" + email_letter_history_filename + "',email_letter_site_id = " + (email_letter_site_id == -1 ? "NULL" : email_letter_site_id.ToString()) + ",email_letter_organisation_id = " + (email_letter_organisation_id == 0 ? "NULL" : email_letter_organisation_id.ToString()) + ",email_letter_booking_id = " + (email_letter_booking_id == -1 ? "NULL" : email_letter_booking_id.ToString()) + ",email_letter_patient_id = " + (email_letter_patient_id == -1 ? "NULL" : email_letter_patient_id.ToString()) + ",email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref = " + (email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref == -1 ? "NULL" : email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref.ToString()) + ",email_letter_staff_id = " + (email_letter_staff_id == -1 ? "NULL" : email_letter_staff_id.ToString()) + ",email_letter_health_card_action_id = " + (email_letter_health_card_action_id == -1 ? "NULL" : email_letter_health_card_action_id.ToString()) + ",email_letter_source_template_path = '" + email_letter_source_template_path + "',email_letter_output_doc_path = '" + email_letter_output_doc_path + "',email_letter_is_double_sided_printing = " + (email_letter_is_double_sided_printing ? "1," : "0,") + "email_letter_extra_pages = '" + email_letter_extra_pages + "',email_letter_item_seperator = '" + email_letter_item_seperator + "',sql_to_run_on_completion = '" + sql_to_run_on_completion + "',sql_to_run_on_failure = '" + sql_to_run_on_failure + "',datetime_sending_start = '" + datetime_sending_start.ToString("yyyy-MM-dd HH:mm:ss") + "',datetime_sent = '" + datetime_sent.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE bulk_letter_sending_queue_id = " + bulk_letter_sending_queue_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateSentTime(int bulk_letter_sending_queue_id, DateTime datetime_sent)
    {
        string sql = "UPDATE BulkLetterSendingQueue SET datetime_sent = " + (datetime_sent == DateTime.MinValue ? "NULL" : "'" + datetime_sent.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE bulk_letter_sending_queue_id = " + bulk_letter_sending_queue_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static DataTable GetDataTable(int bulk_letter_sending_queue_batch_id = -1)
    {
        string sql = JoinedSql() + (bulk_letter_sending_queue_batch_id != -1 ? " WHERE BulkLetterSendingQueue.bulk_letter_sending_queue_batch_id = " + bulk_letter_sending_queue_batch_id : "");
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static BulkLetterSendingQueue GetByID(int bulk_letter_sending_queue_id)
    {
        string sql = JoinedSql() + "WHERE bulk_letter_sending_queue_id = " + bulk_letter_sending_queue_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }


    public static int GetNextBatchIDToSend()
    {
        string sql = @"
					SELECT TOP 1 
						BulkLetterSendingQueue.bulk_letter_sending_queue_batch_id
					FROM 
						BulkLetterSendingQueue
						LEFT JOIN BulkLetterSendingQueueBatch ON BulkLetterSendingQueue.bulk_letter_sending_queue_batch_id = BulkLetterSendingQueueBatch.bulk_letter_sending_queue_batch_id 
					WHERE 
						datetime_sent IS NULL AND datetime_sending_start IS NULL AND BulkLetterSendingQueueBatch.ready_to_process = 1 
					ORDER BY 
						BulkLetterSendingQueue.bulk_letter_sending_queue_batch_id,  letter_print_history_send_method_id DESC ";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        if (tbl.Rows.Count == 0)
            return -1;
        else
            return Convert.ToInt32(tbl.Rows[0][0]);
    }

    public static BulkLetterSendingQueue[] GetNextBatch(int bulk_letter_sending_queue_batch_id, bool setToSending)
    {
        string whereClause = " WHERE datetime_sent IS NULL AND datetime_sending_start IS NULL AND BulkLetterSendingQueue.bulk_letter_sending_queue_batch_id = " + bulk_letter_sending_queue_batch_id;
        string updateSetSending = " UPDATE BulkLetterSendingQueue SET datetime_sending_start = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " + whereClause;
        string sql = JoinedSql() + whereClause + ";" + (setToSending ? updateSetSending + ";" : "");
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        BulkLetterSendingQueue[] list = new BulkLetterSendingQueue[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = Load(tbl.Rows[i]);

        return list;
    }



    public static string JoinedSql(int topN = -1)
    {
        return
@"               SELECT " + (topN == -1 ? "" : @" TOP " + topN + " ") + @"
						bulk_letter_sending_queue_id,
						BulkLetterSendingQueue.bulk_letter_sending_queue_batch_id,
						letter_print_history_send_method_id,
						added_by,
						patient_id,
						referrer_id,
						booking_id,
						phone_number,
                        email_to_address,
                        email_to_name,
                        email_from_address,
                        email_from_name,
                        text,
                        email_subject,
                        email_attachment_location,
                        email_attachment_delete_after_sending,
                        email_attachment_folder_delete_after_sending,
                        email_letter_letter_id,
                        email_letter_keep_history_in_db,
                        email_letter_keep_history_in_file,
                        email_letter_letter_print_history_send_method_id,
                        email_letter_history_dir,
                        email_letter_history_filename,
                        email_letter_site_id,
                        email_letter_organisation_id,
                        email_letter_booking_id,
                        email_letter_patient_id,
                        email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref,
                        email_letter_staff_id,
                        email_letter_health_card_action_id,
                        email_letter_source_template_path,
                        email_letter_output_doc_path,
                        email_letter_is_double_sided_printing,
                        email_letter_extra_pages,
                        email_letter_item_seperator,
                        sql_to_run_on_completion,
                        sql_to_run_on_failure,
                        datetime_added,
                        datetime_sending_start,
                        datetime_sent
                 FROM 
						BulkLetterSendingQueue 
						LEFT JOIN BulkLetterSendingQueueBatch ON BulkLetterSendingQueue.bulk_letter_sending_queue_batch_id = BulkLetterSendingQueueBatch.bulk_letter_sending_queue_batch_id " + Environment.NewLine;
    }

    public static BulkLetterSendingQueue Load(DataRow row)
    {
        return new BulkLetterSendingQueue(
            Convert.ToInt32(row["bulk_letter_sending_queue_id"]),
            Convert.ToInt32(row["bulk_letter_sending_queue_batch_id"]),
            Convert.ToInt32(row["letter_print_history_send_method_id"]),
            row["added_by"] == DBNull.Value ? -1 : Convert.ToInt32(row["added_by"]),
            row["patient_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["patient_id"]),
            row["referrer_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["referrer_id"]),
            row["booking_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["booking_id"]),
            Convert.ToString(row["phone_number"]),
            Convert.ToString(row["email_to_address"]),
            Convert.ToString(row["email_to_name"]),
            Convert.ToString(row["email_from_address"]),
            Convert.ToString(row["email_from_name"]),
            Convert.ToString(row["text"]),
            Convert.ToString(row["email_subject"]),
            Convert.ToString(row["email_attachment_location"]),
            Convert.ToBoolean(row["email_attachment_delete_after_sending"]),
            Convert.ToBoolean(row["email_attachment_folder_delete_after_sending"]),
            row["email_letter_letter_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_letter_id"]),
            Convert.ToBoolean(row["email_letter_keep_history_in_db"]),
            Convert.ToBoolean(row["email_letter_keep_history_in_file"]),
            row["email_letter_letter_print_history_send_method_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_letter_print_history_send_method_id"]),
            Convert.ToString(row["email_letter_history_dir"]),
            Convert.ToString(row["email_letter_history_filename"]),
            row["email_letter_site_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_site_id"]),
            row["email_letter_organisation_id"] == DBNull.Value ? 0 : Convert.ToInt32(row["email_letter_organisation_id"]),
            row["email_letter_booking_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_booking_id"]),
            row["email_letter_patient_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_patient_id"]),
            row["email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref"] == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref"]),
            row["email_letter_staff_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_staff_id"]),
            row["email_letter_health_card_action_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_health_card_action_id"]),
            Convert.ToString(row["email_letter_source_template_path"]),
            Convert.ToString(row["email_letter_output_doc_path"]),
            Convert.ToBoolean(row["email_letter_is_double_sided_printing"]),
            Convert.ToString(row["email_letter_extra_pages"]),
            Convert.ToString(row["email_letter_item_seperator"]),
            Convert.ToString(row["sql_to_run_on_completion"]),
            Convert.ToString(row["sql_to_run_on_failure"]),
            Convert.ToDateTime(row["datetime_added"]),
            row["datetime_sending_start"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["datetime_sending_start"]),
            row["datetime_sent"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["datetime_sent"])
        );
    }

}