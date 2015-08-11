using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class BulkLetterSendingQueueAdditionalLetterDB
{

    public static void Delete(int bulk_letter_sending_queue_letter_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM BulkLetterSendingQueueAdditionalLetter WHERE bulk_letter_sending_queue_letter_id = " + bulk_letter_sending_queue_letter_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int bulk_letter_sending_queue_id, int email_letter_letter_id, bool email_letter_keep_history_in_db, bool email_letter_keep_history_in_file, int email_letter_letter_print_history_send_method_id, string email_letter_history_dir, string email_letter_history_filename, int email_letter_site_id, int email_letter_organisation_id, int email_letter_booking_id, int email_letter_patient_id, int email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref, int email_letter_staff_id, int email_letter_health_card_action_id, string email_letter_source_template_path, string email_letter_output_doc_path, bool email_letter_is_double_sided_printing, string email_letter_extra_pages, string email_letter_item_seperator)
    {
        email_letter_history_dir = email_letter_history_dir.Replace("'", "''");
        email_letter_history_filename = email_letter_history_filename.Replace("'", "''");
        email_letter_source_template_path = email_letter_source_template_path.Replace("'", "''");
        email_letter_output_doc_path = email_letter_output_doc_path.Replace("'", "''");
        email_letter_extra_pages = email_letter_extra_pages.Replace("'", "''");
        email_letter_item_seperator = email_letter_item_seperator.Replace("'", "''");
        //string sql = "INSERT INTO BulkLetterSendingQueueAdditionalLetter (bulk_letter_sending_queue_id,email_letter_letter_id,email_letter_keep_history_in_db,email_letter_keep_history_in_file,email_letter_letter_print_history_send_method_id,email_letter_history_dir,email_letter_history_filename,email_letter_site_id,email_letter_organisation_id,email_letter_booking_id,email_letter_patient_id,email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref,email_letter_staff_id,email_letter_health_card_action_id,email_letter_source_template_path,email_letter_output_doc_path,email_letter_is_double_sided_printing,email_letter_extra_pages,email_letter_item_seperator) VALUES (" + "" + bulk_letter_sending_queue_id + "," + "" + email_letter_letter_id + "," + (email_letter_keep_history_in_db ? "1," : "0,") + (email_letter_keep_history_in_file ? "1," : "0,") + "" + email_letter_letter_print_history_send_method_id + "," + "'" + email_letter_history_dir + "'," + "'" + email_letter_history_filename + "'," + "" + email_letter_site_id + "," + "" + email_letter_organisation_id + "," + "" + email_letter_booking_id + "," + "" + email_letter_patient_id + "," + "" + email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref + "," + "" + email_letter_staff_id + "," + "" + email_letter_health_card_action_id + "," + "'" + email_letter_source_template_path + "'," + "'" + email_letter_output_doc_path + "'," + (email_letter_is_double_sided_printing ? "1," : "0,") + "'" + email_letter_extra_pages + "'," + "'" + email_letter_item_seperator + "'" + ");SELECT SCOPE_IDENTITY();";
        string sql = "INSERT INTO BulkLetterSendingQueueAdditionalLetter (bulk_letter_sending_queue_id,email_letter_letter_id,email_letter_keep_history_in_db,email_letter_keep_history_in_file,email_letter_letter_print_history_send_method_id,email_letter_history_dir,email_letter_history_filename,email_letter_site_id,email_letter_organisation_id,email_letter_booking_id,email_letter_patient_id,email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref,email_letter_staff_id,email_letter_health_card_action_id,email_letter_source_template_path,email_letter_output_doc_path,email_letter_is_double_sided_printing,email_letter_extra_pages,email_letter_item_seperator) VALUES (" + "" + bulk_letter_sending_queue_id + "," + "" + (email_letter_letter_id == -1 ? "NULL" : email_letter_letter_id.ToString()) + "," + (email_letter_keep_history_in_db ? "1," : "0,") + (email_letter_keep_history_in_file ? "1," : "0,") + "" + (email_letter_letter_print_history_send_method_id == -1 ? "NULL" : email_letter_letter_print_history_send_method_id.ToString()) + "," + "'" + email_letter_history_dir + "'," + "'" + email_letter_history_filename + "'," + "" + (email_letter_site_id == -1 ? "NULL" : email_letter_site_id.ToString()) + "," + "" + (email_letter_organisation_id == 0 ? "NULL" : email_letter_organisation_id.ToString()) + "," + "" + (email_letter_booking_id == -1 ? "NULL" : email_letter_booking_id.ToString()) + "," + "" + (email_letter_patient_id == -1 ? "NULL" : email_letter_patient_id.ToString()) + "," + "" + (email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref == -1 ? "NULL" : email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref.ToString()) + "," + "" + (email_letter_staff_id == -1 ? "NULL" : email_letter_staff_id.ToString()) + "," + "" + (email_letter_health_card_action_id == -1 ? "NULL" : email_letter_health_card_action_id.ToString()) + "," + "'" + email_letter_source_template_path + "'," + "'" + email_letter_output_doc_path + "'," + (email_letter_is_double_sided_printing ? "1," : "0,") + "'" + email_letter_extra_pages + "'," + "'" + email_letter_item_seperator + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int bulk_letter_sending_queue_letter_id, int bulk_letter_sending_queue_id, int email_letter_letter_id, bool email_letter_keep_history_in_db, bool email_letter_keep_history_in_file, int email_letter_letter_print_history_send_method_id, string email_letter_history_dir, string email_letter_history_filename, int email_letter_site_id, int email_letter_organisation_id, int email_letter_booking_id, int email_letter_patient_id, int email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref, int email_letter_staff_id, int email_letter_health_card_action_id, string email_letter_source_template_path, string email_letter_output_doc_path, bool email_letter_is_double_sided_printing, string email_letter_extra_pages, string email_letter_item_seperator)
    {
        email_letter_history_dir = email_letter_history_dir.Replace("'", "''");
        email_letter_history_filename = email_letter_history_filename.Replace("'", "''");
        email_letter_source_template_path = email_letter_source_template_path.Replace("'", "''");
        email_letter_output_doc_path = email_letter_output_doc_path.Replace("'", "''");
        email_letter_extra_pages = email_letter_extra_pages.Replace("'", "''");
        email_letter_item_seperator = email_letter_item_seperator.Replace("'", "''");
        //string sql = "UPDATE BulkLetterSendingQueueAdditionalLetter SET bulk_letter_sending_queue_id = " + bulk_letter_sending_queue_id + ",email_letter_letter_id = " + email_letter_letter_id + ",email_letter_keep_history_in_db = " + (email_letter_keep_history_in_db ? "1," : "0,") + "email_letter_keep_history_in_file = " + (email_letter_keep_history_in_file ? "1," : "0,") + "email_letter_letter_print_history_send_method_id = " + email_letter_letter_print_history_send_method_id + ",email_letter_history_dir = '" + email_letter_history_dir + "',email_letter_history_filename = '" + email_letter_history_filename + "',email_letter_site_id = " + email_letter_site_id + ",email_letter_organisation_id = " + email_letter_organisation_id + ",email_letter_booking_id = " + email_letter_booking_id + ",email_letter_patient_id = " + email_letter_patient_id + ",email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref = " + email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref + ",email_letter_staff_id = " + email_letter_staff_id + ",email_letter_health_card_action_id = " + email_letter_health_card_action_id + ",email_letter_source_template_path = '" + email_letter_source_template_path + "',email_letter_output_doc_path = '" + email_letter_output_doc_path + "',email_letter_is_double_sided_printing = " + (email_letter_is_double_sided_printing ? "1," : "0,") + "email_letter_extra_pages = '" + email_letter_extra_pages + "',email_letter_item_seperator = '" + email_letter_item_seperator + "' WHERE bulk_letter_sending_queue_letter_id = " + bulk_letter_sending_queue_letter_id.ToString();
        string sql = "UPDATE BulkLetterSendingQueueAdditionalLetter SET bulk_letter_sending_queue_id = " + bulk_letter_sending_queue_id + ",email_letter_letter_id = " + (email_letter_letter_id == -1 ? "NULL" : email_letter_letter_id.ToString()) + ",email_letter_keep_history_in_db = " + (email_letter_keep_history_in_db ? "1," : "0,") + "email_letter_keep_history_in_file = " + (email_letter_keep_history_in_file ? "1," : "0,") + "email_letter_letter_print_history_send_method_id = " + (email_letter_letter_print_history_send_method_id == -1 ? "NULL" : email_letter_letter_print_history_send_method_id.ToString()) + ",email_letter_history_dir = '" + email_letter_history_dir + "',email_letter_history_filename = '" + email_letter_history_filename + "',email_letter_site_id = " + (email_letter_site_id == -1 ? "NULL" : email_letter_site_id.ToString()) + ",email_letter_organisation_id = " + (email_letter_organisation_id == 0 ? "NULL" : email_letter_organisation_id.ToString()) + ",email_letter_booking_id = " + (email_letter_booking_id == -1 ? "NULL" : email_letter_booking_id.ToString()) + ",email_letter_patient_id = " + (email_letter_patient_id == -1 ? "NULL" : email_letter_patient_id.ToString()) + ",email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref = " + (email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref == -1 ? "NULL" : email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref.ToString()) + ",email_letter_staff_id = " + (email_letter_staff_id == -1 ? "NULL" : email_letter_staff_id.ToString()) + ",email_letter_health_card_action_id = " + (email_letter_health_card_action_id == -1 ? "NULL" : email_letter_health_card_action_id.ToString()) + ",email_letter_source_template_path = '" + email_letter_source_template_path + "',email_letter_output_doc_path = '" + email_letter_output_doc_path + "',email_letter_is_double_sided_printing = " + (email_letter_is_double_sided_printing ? "1," : "0,") + "email_letter_extra_pages = '" + email_letter_extra_pages + "',email_letter_item_seperator = '" + email_letter_item_seperator + "' WHERE bulk_letter_sending_queue_letter_id = " + bulk_letter_sending_queue_letter_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = JoinedSql();
        return DBBase.ExecuteQuery( sql ).Tables[0];
    }

    public static BulkLetterSendingQueueAdditionalLetter GetByID(int bulk_letter_sending_queue_letter_id)
    {
        string sql = JoinedSql() + " WHERE bulk_letter_sending_queue_letter_id = " + bulk_letter_sending_queue_letter_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery( sql ).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static Hashtable GetByBatchID(int bulk_letter_sending_queue_batch_id)
    {
        string sql = JoinedSql() +
            @" LEFT JOIN BulkLetterSendingQueue ON BulkLetterSendingQueueAdditionalLetter.bulk_letter_sending_queue_id = BulkLetterSendingQueue.bulk_letter_sending_queue_id
	        	   WHERE     BulkLetterSendingQueue.bulk_letter_sending_queue_batch_id = " + bulk_letter_sending_queue_batch_id.ToString();

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            BulkLetterSendingQueueAdditionalLetter letter = Load(tbl.Rows[i]);

            if (hash[letter.BulkLetterSendingQueueID] == null)
                hash[letter.BulkLetterSendingQueueID] = new ArrayList();

            ((ArrayList)hash[letter.BulkLetterSendingQueueID]).Add(letter);
        }

        return hash;
    }


    public static string JoinedSql()
    {
        return
@"               SELECT 
						BulkLetterSendingQueueAdditionalLetter.bulk_letter_sending_queue_letter_id,
						BulkLetterSendingQueueAdditionalLetter.bulk_letter_sending_queue_id,
	
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_letter_id,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_keep_history_in_db,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_keep_history_in_file,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_letter_print_history_send_method_id,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_history_dir,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_history_filename,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_site_id,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_organisation_id,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_booking_id,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_patient_id,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_staff_id,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_health_card_action_id,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_source_template_path,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_output_doc_path,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_is_double_sided_printing,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_extra_pages,
	                    BulkLetterSendingQueueAdditionalLetter.email_letter_item_seperator
                 FROM 
						BulkLetterSendingQueueAdditionalLetter " + Environment.NewLine;
    }


    public static BulkLetterSendingQueueAdditionalLetter Load(DataRow row)
    {
        return new BulkLetterSendingQueueAdditionalLetter(
	        Convert.ToInt32(row["bulk_letter_sending_queue_letter_id"]),
	        Convert.ToInt32(row["bulk_letter_sending_queue_id"]),
			row["email_letter_letter_id"]					                            == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_letter_id"]),
			Convert.ToBoolean(row["email_letter_keep_history_in_db"]),
			Convert.ToBoolean(row["email_letter_keep_history_in_file"]),
			row["email_letter_letter_print_history_send_method_id"]					    == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_letter_print_history_send_method_id"]),
			Convert.ToString(row["email_letter_history_dir"]),
			Convert.ToString(row["email_letter_history_filename"]),
			row["email_letter_site_id"]												    == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_site_id"]),
			row["email_letter_organisation_id"]										    == DBNull.Value ?  0 : Convert.ToInt32(row["email_letter_organisation_id"]),
			row["email_letter_booking_id"]											    == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_booking_id"]),
			row["email_letter_patient_id"]											    == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_patient_id"]),
			row["email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref"] == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref"]),
			row["email_letter_staff_id"]												== DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_staff_id"]),
			row["email_letter_health_card_action_id"]								    == DBNull.Value ? -1 : Convert.ToInt32(row["email_letter_health_card_action_id"]),
			Convert.ToString(row["email_letter_source_template_path"]),
			Convert.ToString(row["email_letter_output_doc_path"]),
			Convert.ToBoolean(row["email_letter_is_double_sided_printing"]),
			Convert.ToString(row["email_letter_extra_pages"]),
			Convert.ToString(row["email_letter_item_seperator"])

        );
    }

}