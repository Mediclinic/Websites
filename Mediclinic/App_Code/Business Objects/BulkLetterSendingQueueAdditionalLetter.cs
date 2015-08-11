using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class BulkLetterSendingQueueAdditionalLetter
{

    public BulkLetterSendingQueueAdditionalLetter(int bulk_letter_sending_queue_letter_id, int bulk_letter_sending_queue_id, int email_letter_letter_id, bool email_letter_keep_history_in_db, bool email_letter_keep_history_in_file, int email_letter_letter_print_history_send_method_id,
                string email_letter_history_dir, string email_letter_history_filename, int email_letter_site_id, int email_letter_organisation_id, int email_letter_booking_id, int email_letter_patient_id,
                int email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref, int email_letter_staff_id, int email_letter_health_card_action_id, string email_letter_source_template_path, string email_letter_output_doc_path, bool email_letter_is_double_sided_printing,
                string email_letter_extra_pages, string email_letter_item_seperator)
    {
        this.bulk_letter_sending_queue_letter_id = bulk_letter_sending_queue_letter_id;
        this.bulk_letter_sending_queue_id = bulk_letter_sending_queue_id;
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
    }

    private int bulk_letter_sending_queue_letter_id;
    public int BulkLetterSendingQueueLetterID
    {
        get { return this.bulk_letter_sending_queue_letter_id; }
        set { this.bulk_letter_sending_queue_letter_id = value; }
    }
    private int bulk_letter_sending_queue_id;
    public int BulkLetterSendingQueueID
    {
        get { return this.bulk_letter_sending_queue_id; }
        set { this.bulk_letter_sending_queue_id = value; }
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
    public override string ToString()
    {
        return bulk_letter_sending_queue_letter_id.ToString() + " " + bulk_letter_sending_queue_id.ToString() + " " + email_letter_letter_id.ToString() + " " + email_letter_keep_history_in_db.ToString() + " " + email_letter_keep_history_in_file.ToString() + " " +
                email_letter_letter_print_history_send_method_id.ToString() + " " + email_letter_history_dir.ToString() + " " + email_letter_history_filename.ToString() + " " + email_letter_site_id.ToString() + " " + email_letter_organisation_id.ToString() + " " +
                email_letter_booking_id.ToString() + " " + email_letter_patient_id.ToString() + " " + email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref.ToString() + " " + email_letter_staff_id.ToString() + " " + email_letter_health_card_action_id.ToString() + " " +
                email_letter_source_template_path.ToString() + " " + email_letter_output_doc_path.ToString() + " " + email_letter_is_double_sided_printing.ToString() + " " + email_letter_extra_pages.ToString() + " " + email_letter_item_seperator.ToString();
    }

}