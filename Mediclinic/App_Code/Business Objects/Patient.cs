using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Collections;

public class Patient
{

    public Patient(int patient_id, int person_id, DateTime patient_date_added, bool is_clinic_patient, bool is_gp_patient,
        bool is_deleted, bool is_deceased,
        string flashing_text, int flashing_text_added_by, DateTime flashing_text_last_modified_date,
        string private_health_fund, string concession_card_number, DateTime concession_card_expiry_date,
        bool is_diabetic, bool is_member_diabetes_australia, DateTime diabetic_assessment_review_date, int ac_inv_offering_id, int ac_pat_offering_id,
        string login, string pwd,
        bool is_company, string abn,
        string next_of_kin_name, string next_of_kin_relation, string next_of_kin_contact_info)
    {
        this.patient_id                       = patient_id;
        this.person                           = new Person(person_id);
        this.patient_date_added               = patient_date_added;
        this.is_clinic_patient                = is_clinic_patient;
        this.is_gp_patient                    = is_gp_patient;
        this.is_deleted                       = is_deleted;
        this.is_deceased                      = is_deceased;
        this.flashing_text                    = flashing_text;
        this.flashing_text_added_by           = flashing_text_added_by == -1 ? null : new Staff(flashing_text_added_by);
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
    }
    public Patient(int patient_id)
    {
        this.patient_id = patient_id;
    }

    private int patient_id;
    public int PatientID
    {
        get { return this.patient_id; }
        set { this.patient_id = value; }
    }
    private Person person;
    public Person Person
    {
        get { return this.person; }
        set { this.person = value; }
    }
    private DateTime patient_date_added;
    public DateTime PatientDateAdded
    {
        get { return this.patient_date_added; }
        set { this.patient_date_added = value; }
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
    public override string ToString()
    {
        return patient_id.ToString() + " " + person.PersonID.ToString() + " " + patient_date_added.ToString() + " " + is_deleted.ToString() + " " + is_deceased.ToString();
    }


    public static Patient[] RemoveByID(Patient[] patients, int patient_id_to_remove)
    {
        Patient[] newList = new Patient[patients.Length - 1];

        bool found = false;
        for (int i = 0; i < patients.Length; i++)
        {
            if (patients[i].PatientID != patient_id_to_remove)
                newList[i - (found ? 1 : 0)] = patients[i];
            else
                found = true;
        }

        return newList;
    }

    public string GetScannedDocsDirectory()
    {
        string dbName = System.Web.HttpContext.Current.Session["DB"].ToString();
        bool useLocalFolder = Convert.ToBoolean(
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] != null ?
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] :
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal"]
            );

        string configName = useLocalFolder ? "PatientScannedDocsLocalFolder" : "PatientScannedDocsNetworkFolder";
        string dir = 
            System.Configuration.ConfigurationManager.AppSettings[configName + "_" + dbName] != null ?
            System.Configuration.ConfigurationManager.AppSettings[configName + "_" + dbName] :
            System.Configuration.ConfigurationManager.AppSettings[configName].Replace("[DBNAME]", dbName);

        dir = dir.EndsWith(@"\") ? dir : dir + @"\";
        return dir + this.PatientID + @"\";
    }

    public FileInfo[] GetScannedDocs()
    {
        string dbName = System.Web.HttpContext.Current.Session["DB"].ToString().Replace("_TestDB", "");
        bool useLocalFolder = Convert.ToBoolean(
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] != null ?
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] :
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal"]
            );

        if (useLocalFolder)
        {

            string s = GetScannedDocsDirectory();

            if (!Directory.Exists(GetScannedDocsDirectory()))
                return new FileInfo[] { };

            DirectoryInfo dir = new DirectoryInfo(GetScannedDocsDirectory());
            return (FileInfo[])GetFiles(dir).ToArray(typeof(FileInfo));  // dir.GetFiles();
        }
        else  // use network folder
        {
            string username    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderUsername"];
            string password    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderPassword"];

            string networkName =
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] != null ?
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] :
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder"];

            if (networkName.EndsWith(@"\"))
                networkName = networkName.Substring(0, networkName.Length - 1);

            using (new NetworkConnection(networkName, new System.Net.NetworkCredential(username, password)))
            {
                if (!System.IO.Directory.Exists(GetScannedDocsDirectory()))
                    return new FileInfo[] { };
                DirectoryInfo dir = new DirectoryInfo(GetScannedDocsDirectory() + @"\");
                return (FileInfo[])GetFiles(dir).ToArray(typeof(FileInfo));  // dir.GetFiles();
            }
        }

        /*
        if (!Directory.Exists(GetScannedDocsDirectory()))
            return new FileInfo[]{};

        DirectoryInfo dir = new DirectoryInfo(GetScannedDocsDirectory() + @"\");
        return dir.GetFiles();
        */
    }
    protected ArrayList GetFiles(DirectoryInfo dir)
    {
        ArrayList list = new ArrayList();
        foreach (DirectoryInfo subDir in dir.GetDirectories())
            list.AddRange(GetFiles(subDir));

        list.AddRange(dir.GetFiles());

        return list;
    }

    public static string GetFlashingTextLink(int patientID, bool hasFlashingText, string flashingText, bool updateAfterPopupClosed, int width, int height, string noFlashingTextImage, string hasFlashingTextImage, string functionsToCallAfter = null, bool usePopup = true)
    {
        string allFeatures = "dialogWidth:" + width + "px;dialogHeight:" + height + "px;center:yes;resizable:no; scroll:no";

        string js =
            usePopup ?
            "javascript:window.showModalDialog('" + "PatientFlashingMessageDetailV2.aspx?type=" + (hasFlashingText ? "view" : "edit") + "&id=" + patientID.ToString() + "', '', '" + allFeatures + "');" + (functionsToCallAfter != null ? functionsToCallAfter + ";" : "") + (updateAfterPopupClosed ? "" : "return false;") :
            "javascript: var win=window.open('" + "PatientFlashingMessageDetailV2.aspx?type=" + (hasFlashingText ? "view" : "edit") + "&id=" + patientID.ToString() + "', '_blank'); win.focus();return false;";

        string img = hasFlashingText ? hasFlashingTextImage : noFlashingTextImage;
        string notesText = "<input type=\"image\" " + (hasFlashingText ? " class=\"flashing_text_icon\" " : "") + " title=\"" + (hasFlashingText ? "PT Alert : " + flashingText : "PT Alert") + "\" src=\"" + img + "\" alt=\"PT Alert Note\" onclick=\"" + js + "\" />";
        return notesText;
    }


    public static string EncodeUnsubscribeHash(int patientID, string DB)
    {
        string inString = DB + "__" + patientID.ToString().PadLeft(6, '0');
        return SimpleAES.Encrypt(SimpleAES.KeyType.EmailUnsubscribe, inString);
    }
    public static bool IsValidUnsubscribeHash(string inString)
    {
        if (inString == null)
            return false;

        inString = inString.Replace(" ", "+");

        string result = SimpleAES.Decrypt(SimpleAES.KeyType.EmailUnsubscribe, inString);
        if (result == null || !System.Text.RegularExpressions.Regex.IsMatch(result, @"^Mediclinic_\d{4}__\d+$"))
            return false;

        string[] resultSplit = result.Split(new string[] { "__" }, StringSplitOptions.None);
        string DB = resultSplit[0];
        string invNbr = resultSplit[1];

        if (!Utilities.IsValidDB(DB.Substring(DB.Length - 4)))
            return false;

        if (PatientDB.GetByID(Convert.ToInt32(invNbr), DB) == null)
            return false;

        return true;
    }
    public static Tuple<string, int> DecodeUnsubscribeHash(string inString)
    {
        if (inString == null)
            return null;

        inString = inString.Replace(" ", "+");

        string result = SimpleAES.Decrypt(SimpleAES.KeyType.EmailUnsubscribe, inString);
        if (result == null || !System.Text.RegularExpressions.Regex.IsMatch(result, @"^Mediclinic_\d{4}__\d+$"))
            return null;

        string[] resultSplit = result.Split(new string[] { "__" }, StringSplitOptions.None);
        string DB = resultSplit[0];
        string invNbr = resultSplit[1];

        return new Tuple<string, int>(DB, Convert.ToInt32(invNbr));
    }
    public static string GetUnsubscribeLink(int patientID, string DB)
    {
        return @"http://localhost:2524/PatientUnsubscribeV2.aspx?id=" + EncodeUnsubscribeHash(patientID, DB);
        //return @"https://portal.mediclinic.com.au/PatientUnsubscribeV2.aspx?id=" + EncodeUnsubscribeHash(patientID, DB);
    }
    public static void UnsubscribeAll(int patientID, string DB)
    {
        SystemVariables sysVariables = SystemVariableDB.GetAll(DB);
        if (sysVariables["AddressType"].Value == "Contact")
        {
            Contact[] emails = ContactDB.GetByEntityID(-1, patientID, 27, false, DB);
            foreach (Contact email in emails)
                ContactDB.UpdateIsBillingIsNonbilling(email.ContactID, email.IsBilling, false, DB);
        }
        else if (sysVariables["AddressType"].Value == "ContactAus")
        {
            ContactAus[] emails = ContactAusDB.GetByEntityID(-1, patientID, 27, false, DB);
            foreach (ContactAus email in emails)
                ContactAusDB.UpdateIsBillingIsNonbilling(email.ContactID, email.IsBilling, false, DB);
        }
        else
            throw new Exception("Unknown AddressType in config: " + sysVariables["AddressType"].Value);
    }

}