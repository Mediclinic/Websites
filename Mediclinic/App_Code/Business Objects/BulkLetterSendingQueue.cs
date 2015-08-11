using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BulkLetterSendingQueue
{

    public BulkLetterSendingQueue(int bulk_letter_sending_queue_id, int bulk_letter_sending_queue_batch_id, int letter_print_history_send_method_id, int added_by, int patient_id, int referrer_id, 
                int booking_id, string phone_number, string email_to_address, string email_to_name, string email_from_address, string email_from_name, 
                string text, string email_subject, string email_attachment_location, bool email_attachment_delete_after_sending, bool email_attachment_folder_delete_after_sending, int email_letter_letter_id, 
                bool email_letter_keep_history_in_db, bool email_letter_keep_history_in_file, int email_letter_letter_print_history_send_method_id, string email_letter_history_dir, string email_letter_history_filename, int email_letter_site_id, 
                int email_letter_organisation_id, int email_letter_booking_id, int email_letter_patient_id, int email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref, int email_letter_staff_id, int email_letter_health_card_action_id, 
                string email_letter_source_template_path, string email_letter_output_doc_path, bool email_letter_is_double_sided_printing, string email_letter_extra_pages, string email_letter_item_seperator, string sql_to_run_on_completion, 
                string sql_to_run_on_failure, DateTime datetime_added, DateTime datetime_sending_start, DateTime datetime_sent)
    {
        this.bulk_letter_sending_queue_id = bulk_letter_sending_queue_id;
        this.bulk_letter_sending_queue_batch_id = bulk_letter_sending_queue_batch_id;
        this.letter_print_history_send_method_id = letter_print_history_send_method_id;
        this.added_by = added_by;
        this.patient_id = patient_id;
        this.referrer_id = referrer_id;
        this.booking_id = booking_id;
        this.phone_number = phone_number;
        this.email_to_address = email_to_address;
        this.email_to_name = email_to_name;
        this.email_from_address = email_from_address;
        this.email_from_name = email_from_name;
        this.text = text;
        this.email_subject = email_subject;
        this.email_attachment_location = email_attachment_location;
        this.email_attachment_delete_after_sending = email_attachment_delete_after_sending;
        this.email_attachment_folder_delete_after_sending = email_attachment_folder_delete_after_sending;
        this.email_letter_letter_id = email_letter_letter_id;
        this.email_letter_keep_history_in_db = email_letter_keep_history_in_db;
        this.email_letter_keep_history_in_file = email_letter_keep_history_in_file;
        this.email_letter_letter_print_history_send_method_id = email_letter_letter_print_history_send_method_id;
        this.email_letter_history_dir = email_letter_history_dir;
        this.email_letter_history_filename = email_letter_history_filename;
        this.email_letter_site_id = email_letter_site_id;
        this.email_letter_organisation_id = email_letter_organisation_id;
        this.email_letter_booking_id = email_letter_booking_id;
        this.email_letter_patient_id = email_letter_patient_id;
        this.email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref = email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref;
        this.email_letter_staff_id = email_letter_staff_id;
        this.email_letter_health_card_action_id = email_letter_health_card_action_id;
        this.email_letter_source_template_path = email_letter_source_template_path;
        this.email_letter_output_doc_path = email_letter_output_doc_path;
        this.email_letter_is_double_sided_printing = email_letter_is_double_sided_printing;
        this.email_letter_extra_pages = email_letter_extra_pages;
        this.email_letter_item_seperator = email_letter_item_seperator;
        this.sql_to_run_on_completion = sql_to_run_on_completion;
        this.sql_to_run_on_failure = sql_to_run_on_failure;
        this.datetime_added = datetime_added;
        this.datetime_sending_start = datetime_sending_start;
        this.datetime_sent = datetime_sent;
    }

    private int bulk_letter_sending_queue_id;
    public int BulkLetterSendingQueueID
    {
        get { return this.bulk_letter_sending_queue_id; }
        set { this.bulk_letter_sending_queue_id = value; }
    }
    private int bulk_letter_sending_queue_batch_id;
    public int BulkLetterSendingQueueBatchID
    {
        get { return this.bulk_letter_sending_queue_batch_id; }
        set { this.bulk_letter_sending_queue_batch_id = value; }
    }
    private int letter_print_history_send_method_id;
    public int LetterPrintHistorySendMethodID
    {
        get { return this.letter_print_history_send_method_id; }
        set { this.letter_print_history_send_method_id = value; }
    }
    private int added_by;
    public int AddedBy
    {
        get { return this.added_by; }
        set { this.added_by = value; }
    }
    private int patient_id;
    public int PatientID
    {
        get { return this.patient_id; }
        set { this.patient_id = value; }
    }
    private int referrer_id;
    public int ReferrerID
    {
        get { return this.referrer_id; }
        set { this.referrer_id = value; }
    }
    private int booking_id;
    public int BookingID
    {
        get { return this.booking_id; }
        set { this.booking_id = value; }
    }
    private string phone_number;
    public string PhoneNumber
    {
        get { return this.phone_number; }
        set { this.phone_number = value; }
    }
    private string email_to_address;
    public string EmailToAddress
    {
        get { return this.email_to_address; }
        set { this.email_to_address = value; }
    }
    private string email_to_name;
    public string EmailToName
    {
        get { return this.email_to_name; }
        set { this.email_to_name = value; }
    }
    private string email_from_address;
    public string EmailFromAddress
    {
        get { return this.email_from_address; }
        set { this.email_from_address = value; }
    }
    private string email_from_name;
    public string EmailFromName
    {
        get { return this.email_from_name; }
        set { this.email_from_name = value; }
    }
    private string text;
    public string Text
    {
        get { return this.text; }
        set { this.text = value; }
    }
    private string email_subject;
    public string EmailSubject
    {
        get { return this.email_subject; }
        set { this.email_subject = value; }
    }
    private string email_attachment_location;
    public string EmailAttachmentLocation
    {
        get { return this.email_attachment_location; }
        set { this.email_attachment_location = value; }
    }
    private bool email_attachment_delete_after_sending;
    public bool EmailAttachmentDeleteAfterSending
    {
        get { return this.email_attachment_delete_after_sending; }
        set { this.email_attachment_delete_after_sending = value; }
    }
    private bool email_attachment_folder_delete_after_sending;
    public bool EmailAttachmentFolderDeleteAfterSending
    {
        get { return this.email_attachment_folder_delete_after_sending; }
        set { this.email_attachment_folder_delete_after_sending = value; }
    }
    private int email_letter_letter_id;
    public int EmailLetterLetterID
    {
        get { return this.email_letter_letter_id; }
        set { this.email_letter_letter_id = value; }
    }
    private bool email_letter_keep_history_in_db;
    public bool EmailLetterKeepHistoryInDb
    {
        get { return this.email_letter_keep_history_in_db; }
        set { this.email_letter_keep_history_in_db = value; }
    }
    private bool email_letter_keep_history_in_file;
    public bool EmailLetterKeepHistoryInFile
    {
        get { return this.email_letter_keep_history_in_file; }
        set { this.email_letter_keep_history_in_file = value; }
    }
    private int email_letter_letter_print_history_send_method_id;
    public int EmailLetterLetterPrintHistorySendMethodID
    {
        get { return this.email_letter_letter_print_history_send_method_id; }
        set { this.email_letter_letter_print_history_send_method_id = value; }
    }
    private string email_letter_history_dir;
    public string EmailLetterHistoryDir
    {
        get { return this.email_letter_history_dir; }
        set { this.email_letter_history_dir = value; }
    }
    private string email_letter_history_filename;
    public string EmailLetterHistoryFilename
    {
        get { return this.email_letter_history_filename; }
        set { this.email_letter_history_filename = value; }
    }
    private int email_letter_site_id;
    public int EmailLetterSiteID
    {
        get { return this.email_letter_site_id; }
        set { this.email_letter_site_id = value; }
    }
    private int email_letter_organisation_id;
    public int EmailLetterOrganisationID
    {
        get { return this.email_letter_organisation_id; }
        set { this.email_letter_organisation_id = value; }
    }
    private int email_letter_booking_id;
    public int EmailLetterBookingID
    {
        get { return this.email_letter_booking_id; }
        set { this.email_letter_booking_id = value; }
    }
    private int email_letter_patient_id;
    public int EmailLetterPatientID
    {
        get { return this.email_letter_patient_id; }
        set { this.email_letter_patient_id = value; }
    }
    private int email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref;
    public int EmailLetterRegisterReferrerIdToUseInsteadOfPatientsRegRef
    {
        get { return this.email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref; }
        set { this.email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref = value; }
    }
    private int email_letter_staff_id;
    public int EmailLetterStaffID
    {
        get { return this.email_letter_staff_id; }
        set { this.email_letter_staff_id = value; }
    }
    private int email_letter_health_card_action_id;
    public int EmailLetterHealthCardActionID
    {
        get { return this.email_letter_health_card_action_id; }
        set { this.email_letter_health_card_action_id = value; }
    }
    private string email_letter_source_template_path;
    public string EmailLetterSourceTemplatePath
    {
        get { return this.email_letter_source_template_path; }
        set { this.email_letter_source_template_path = value; }
    }
    private string email_letter_output_doc_path;
    public string EmailLetterOutputDocPath
    {
        get { return this.email_letter_output_doc_path; }
        set { this.email_letter_output_doc_path = value; }
    }
    private bool email_letter_is_double_sided_printing;
    public bool EmailLetterIsDoubleSidedPrinting
    {
        get { return this.email_letter_is_double_sided_printing; }
        set { this.email_letter_is_double_sided_printing = value; }
    }
    private string email_letter_extra_pages;
    public string EmailLetterExtraPages
    {
        get { return this.email_letter_extra_pages; }
        set { this.email_letter_extra_pages = value; }
    }
    private string email_letter_item_seperator;
    public string EmailLetterItemSeperator
    {
        get { return this.email_letter_item_seperator; }
        set { this.email_letter_item_seperator = value; }
    }
    private string sql_to_run_on_completion;
    public string SqlToRunOnCompletion
    {
        get { return this.sql_to_run_on_completion; }
        set { this.sql_to_run_on_completion = value; }
    }
    private string sql_to_run_on_failure;
    public string SqlToRunOnFailure
    {
        get { return this.sql_to_run_on_failure; }
        set { this.sql_to_run_on_failure = value; }
    }
    private DateTime datetime_added;
    public DateTime DatetimeAdded
    {
        get { return this.datetime_added; }
        set { this.datetime_added = value; }
    }
    private DateTime datetime_sending_start;
    public DateTime DatetimeSendingStart
    {
        get { return this.datetime_sending_start; }
        set { this.datetime_sending_start = value; }
    }
    private DateTime datetime_sent;
    public DateTime DatetimeSent
    {
        get { return this.datetime_sent; }
        set { this.datetime_sent = value; }
    }
    public override string ToString()
    {
        return bulk_letter_sending_queue_id.ToString() + " " + bulk_letter_sending_queue_batch_id.ToString() + " " + letter_print_history_send_method_id.ToString() + " " + added_by.ToString() + " " + patient_id.ToString() + " " + 
                referrer_id.ToString() + " " + booking_id.ToString() + " " + phone_number.ToString() + " " + email_to_address.ToString() + " " + email_to_name.ToString() + " " + 
                email_from_address.ToString() + " " + email_from_name.ToString() + " " + text.ToString() + " " + email_subject.ToString() + " " + email_attachment_location.ToString() + " " + 
                email_attachment_delete_after_sending.ToString() + " " + email_attachment_folder_delete_after_sending.ToString() + " " + email_letter_letter_id.ToString() + " " + email_letter_keep_history_in_db.ToString() + " " + email_letter_keep_history_in_file.ToString() + " " + 
                email_letter_letter_print_history_send_method_id.ToString() + " " + email_letter_history_dir.ToString() + " " + email_letter_history_filename.ToString() + " " + email_letter_site_id.ToString() + " " + email_letter_organisation_id.ToString() + " " + 
                email_letter_booking_id.ToString() + " " + email_letter_patient_id.ToString() + " " + email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref.ToString() + " " + email_letter_staff_id.ToString() + " " + email_letter_health_card_action_id.ToString() + " " + 
                email_letter_source_template_path.ToString() + " " + email_letter_output_doc_path.ToString() + " " + email_letter_is_double_sided_printing.ToString() + " " + email_letter_extra_pages.ToString() + " " + email_letter_item_seperator.ToString() + " " + 
                sql_to_run_on_completion.ToString() + " " + sql_to_run_on_failure.ToString() + " " + datetime_added.ToString() + " " + datetime_sending_start.ToString() + " " + datetime_sent.ToString();
    }

}