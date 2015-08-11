using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PatientHistory
{

    public PatientHistory(int patient_history_id, int patient_id, bool is_clinic_patient, bool is_gp_patient, bool is_deleted, bool is_deceased,
                string flashing_text, int flashing_text_added_by, DateTime flashing_text_last_modified_date,
                string private_health_fund, string concession_card_number, DateTime concession_card_expiry_date, bool is_diabetic, bool is_member_diabetes_australia, DateTime diabetic_assessment_review_date, int ac_inv_offering_id, int ac_pat_offering_id,
                string login, string pwd,
                bool is_company, string abn,
                string next_of_kin_name, string next_of_kin_relation, string next_of_kin_contact_info,
                int title_id, string firstname, string middlename, string surname, string nickname, string gender, DateTime dob, int modified_from_this_by, DateTime date_added)
    {
        this.patient_history_id               = patient_history_id;
        this.patient                          = new Patient(patient_id);
        this.is_clinic_patient                = is_clinic_patient;
        this.is_gp_patient                    = is_gp_patient;
        this.is_deleted                       = is_deleted;
        this.is_deceased                      = is_deceased;
        this.flashing_text                    = flashing_text;
        this.flashing_text_added_by           = new Staff(flashing_text_added_by);
        this.flashing_text_last_modified_date = flashing_text_last_modified_date;
        this.private_health_fund              = private_health_fund;
        this.concession_card_number           = concession_card_number;
        this.concession_card_expiry_date      = concession_card_expiry_date;
        this.is_diabetic                      = is_diabetic;
        this.is_member_diabetes_australia     = is_member_diabetes_australia;
        this.diabetic_assessment_review_date  = diabetic_assessment_review_date;
        this.ac_inv_offering                  = ac_inv_offering_id == -1 ? null : new Offering(ac_inv_offering_id);
        this.ac_pat_offering                  = ac_pat_offering_id == -1 ? null : new Offering(ac_pat_offering_id);
        this.login                            = login;
        this.pwd                              = pwd;
        this.is_company                       = is_company;
        this.abn                              = abn;
        this.next_of_kin_name                 = next_of_kin_name; 
        this.next_of_kin_relation             = next_of_kin_relation;
        this.next_of_kin_contact_info         = next_of_kin_contact_info;

        this.title                            = new IDandDescr(title_id);
        this.firstname                        = firstname;
        this.middlename                       = middlename;
        this.surname                          = surname;
        this.nickname                         = nickname;
        this.gender                           = gender;
        this.dob                              = dob;
        this.modified_from_this_by            = new Staff(modified_from_this_by);
        this.date_added                       = date_added;
    }

    private int patient_history_id;
    public int PatientHistoryID
    {
        get { return this.patient_history_id; }
        set { this.patient_history_id = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private bool is_clinic_patient;
    public bool IsClinicPatient
    {
        get { return this.is_clinic_patient; }
        set { this.is_clinic_patient = value; }
    }
    private bool is_gp_patient;
    public bool IsGPPatient
    {
        get { return this.is_gp_patient; }
        set { this.is_gp_patient = value; }
    }
    private bool is_deleted;
    public bool IsDeleted
    {
        get { return this.is_deleted; }
        set { this.is_deleted = value; }
    }
    private bool is_deceased;
    public bool IsDeceased
    {
        get { return this.is_deceased; }
        set { this.is_deceased = value; }
    }
    private string flashing_text;
    public string FlashingText
    {
        get { return this.flashing_text; }
        set { this.flashing_text = value; }
    }
    private Staff flashing_text_added_by;
    public Staff FlashingTextAddedBy
    {
        get { return this.flashing_text_added_by; }
        set { this.flashing_text_added_by = value; }
    }
    private DateTime flashing_text_last_modified_date;
    public DateTime FlashingTextLastModifiedDate
    {
        get { return this.flashing_text_last_modified_date; }
        set { this.flashing_text_last_modified_date = value; }
    }
    private string private_health_fund;
    public string PrivateHealthFund
    {
        get { return this.private_health_fund; }
        set { this.private_health_fund = value; }
    }
    private string concession_card_number;
    public string ConcessionCardNumber
    {
        get { return this.concession_card_number; }
        set { this.concession_card_number = value; }
    }
    private DateTime concession_card_expiry_date;
    public DateTime ConcessionCardExpiryDate
    {
        get { return this.concession_card_expiry_date; }
        set { this.concession_card_expiry_date = value; }
    }
    private bool is_diabetic;
    public bool IsDiabetic
    {
        get { return this.is_diabetic; }
        set { this.is_diabetic = value; }
    }
    private bool is_member_diabetes_australia;
    public bool IsMemberDiabetesAustralia
    {
        get { return this.is_member_diabetes_australia; }
        set { this.is_member_diabetes_australia = value; }
    }
    private DateTime diabetic_assessment_review_date;
    public DateTime DiabeticAAassessmentReviewDate
    {
        get { return this.diabetic_assessment_review_date; }
        set { this.diabetic_assessment_review_date = value; }
    }
    private Offering ac_inv_offering;
    public Offering ACInvOffering
    {
        get { return this.ac_inv_offering; }
        set { this.ac_inv_offering = value; }
    }
    private Offering ac_pat_offering;
    public Offering ACPatOffering
    {
        get { return this.ac_pat_offering; }
        set { this.ac_pat_offering = value; }
    }
    private string login;
    public string Login
    {
        get { return this.login; }
        set { this.login = value; }
    }
    private string pwd;
    public string Pwd
    {
        get { return this.pwd; }
        set { this.pwd = value; }
    }
    private bool is_company;
    public bool IsCompany
    {
        get { return this.is_company; }
        set { this.is_company = value; }
    }
    private string abn;
    public string ABN
    {
        get { return this.abn; }
        set { this.abn = value; }
    }
    private string next_of_kin_name;
    public string NextOfKinName
    {
        get { return this.next_of_kin_name; }
        set { this.next_of_kin_name = value; }
    }
    private string next_of_kin_relation;
    public string NextOfKinRelation
    {
        get { return this.next_of_kin_relation; }
        set { this.next_of_kin_relation = value; }
    }
    private string next_of_kin_contact_info;
    public string NextOfKinContactInfo
    {
        get { return this.next_of_kin_contact_info; }
        set { this.next_of_kin_contact_info = value; }
    }
    private IDandDescr title;
    public IDandDescr Title
    {
        get { return this.title; }
        set { this.title = value; }
    }
    private string firstname;
    public string Firstname
    {
        get { return this.firstname; }
        set { this.firstname = value; }
    }
    private string middlename;
    public string Middlename
    {
        get { return this.middlename; }
        set { this.middlename = value; }
    }
    private string surname;
    public string Surname
    {
        get { return this.surname; }
        set { this.surname = value; }
    }
    private string nickname;
    public string Nickname
    {
        get { return this.nickname; }
        set { this.nickname = value; }
    }
    private string gender;
    public string Gender
    {
        get { return this.gender; }
        set { this.gender = value; }
    }
    private DateTime dob;
    public DateTime Dob
    {
        get { return this.dob; }
        set { this.dob = value; }
    }
    private Staff modified_from_this_by;
    public Staff ModifiedFromThisBy
    {
        get { return this.modified_from_this_by; }
        set { this.modified_from_this_by = value; }
    }
    private DateTime date_added;
    public DateTime DateAdded
    {
        get { return this.date_added; }
        set { this.date_added = value; }
    }
    public override string ToString()
    {
        return patient_history_id.ToString() + " " + patient.PatientID.ToString() + " " + is_clinic_patient.ToString() + " " + is_deleted.ToString() + " " + is_deceased.ToString() + " " +
                title.ID.ToString() + " " + firstname.ToString() + " " + middlename.ToString() + " " +
                surname.ToString() + " " + nickname.ToString() + " " + gender.ToString() + " " + dob.ToString();
    }

}