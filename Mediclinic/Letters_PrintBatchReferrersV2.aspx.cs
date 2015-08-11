using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Web.UI.HtmlControls;

public partial class Letters_PrintBatchReferrersV2 : System.Web.UI.Page
{

    protected static bool UseBulkLetterSender = true;


    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, false);
                Session.Remove("data_selected");
                Session.Remove("sortExpression_Selected");
                Session.Remove("sortExpression_Offering");

                PopulateLettersList();

                SetUrlFields();
            }

        }
        catch (CustomMessageException ex)
        {
            if (IsPostBack) SetErrorMessage(ex.Message);
            else HideTableAndSetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            if (IsPostBack) SetErrorMessage("", ex.ToString());
            else HideTableAndSetErrorMessage("", ex.ToString());
        }
    }

    #endregion

    #region SetUrlFields

    protected void SetUrlFields()
    {
        try
        {
            string letter_id = Request.QueryString["letter"];
            if (letter_id != null && letter_id != "-1")
            {
                if (!Regex.IsMatch(letter_id, @"^\d+$"))
                    throw new CustomMessageException();

                Letter letter = LetterDB.GetByID(Convert.ToInt32(letter_id));
                if (letter == null)
                    throw new CustomMessageException();

                foreach (ListItem item in lstLetters.Items)
                    if (item.Value == letter.LetterID.ToString())
                        item.Selected = true;
            }
        }
        catch (CustomMessageException)
        {
            SetErrorMessage();
        }
    }

    #endregion

    #region btnUpdateReferrers_Click, btnUpdateReferrersFromClinic_Click, btnAddAllReferrers_Click, AddAllReferrers, PopulateLettersList, LetterExists

    protected void btnUpdateReferrers_Click(object sender, EventArgs e)
    {
        UpdateOrgsFromJavascriptChanges();
    }

    protected void btnAddAllReferrers_Click(object sender, EventArgs e)
    {
        AddAllReferrers();
    }
    protected void AddAllReferrers()
    {
        // clear and re-set all (in alphabetical order)

        ArrayList newList = new ArrayList();
        DataTable referrers = RegisterReferrerDB.GetDataTable();
        for (int i = 0; i < referrers.Rows.Count; i++)
            newList.Add(RegisterReferrerDB.LoadAll(referrers.Rows[i]));

        RegisterReferrer[] allReferrers = (RegisterReferrer[])newList.ToArray(typeof(RegisterReferrer));

        lstReferrers.Items.Clear();
        string items = string.Empty;
        for (int i = 0; i < allReferrers.Length; i++)
        {
            string text = allReferrers[i].Referrer.Person.FullnameWithoutMiddlename + " [" + allReferrers[i].Organisation.Name + "]";
            lstReferrers.Items.Add(new ListItem(text, allReferrers[i].RegisterReferrerID.ToString()));

            // add to hidden list
            items += (items.Length == 0 ? "" : ",") + allReferrers[i].RegisterReferrerID.ToString();
        }


        int hasBothMobileEmail    = 0;
        int hasMobileNoEmail      = 0;
        int hasEmailNoMobile      = 0;
        int hasNeitherMobileEmail = 0;
        RegisterReferrerDB.GetCountsByEmailMobile(
            allReferrers,
            ref hasBothMobileEmail,
            ref hasMobileNoEmail,
            ref hasEmailNoMobile,
            ref hasNeitherMobileEmail);

        lblReferrersWithMobileAndEmailTotal.Text       = hasBothMobileEmail.ToString();
        lblReferrersWithMobileNoEmailTotal.Text        = hasMobileNoEmail.ToString();
        lblReferrersWithEmailNoMobileTotal.Text        = hasEmailNoMobile.ToString();
        lblReferrersWithNeitherMobileOrEmailTotal.Text = hasNeitherMobileEmail.ToString();
        lblReferrerCount.Text                          = lstReferrers.Items.Count.ToString();

        hiddenReferrerIDsList.Value = items;
    }


    protected void UpdateOrgsFromJavascriptChanges()
    {
        string referrerIDs = hiddenReferrerIDsList.Value;  // comma seperated
        Hashtable referrerIDsHash = new Hashtable();
        if (referrerIDs.Length > 0)
            foreach (string referrerID in referrerIDs.Split(','))
                referrerIDsHash[Convert.ToInt32(referrerID)] = 1;


        DataTable referrers = RegisterReferrerDB.GetDataTable(0, -1, true);
        RegisterReferrer[] regRefs = new RegisterReferrer[referrers.Rows.Count];
        for (int i = 0; i < referrers.Rows.Count; i++)
            regRefs[i] = RegisterReferrerDB.LoadAll(referrers.Rows[i]);


        ArrayList referrersAdded = new ArrayList();
        lstReferrers.Items.Clear();
        for (int i = 0; i < regRefs.Length; i++)
        {
            if (referrerIDsHash[regRefs[i].RegisterReferrerID] != null)
            {
                string text = regRefs[i].Referrer.Person.FullnameWithoutMiddlename + " [" + regRefs[i].Organisation.Name + "]";
                lstReferrers.Items.Add(new ListItem(text, regRefs[i].RegisterReferrerID.ToString()));

                referrersAdded.Add(regRefs[i]);
            }
        }

        int hasBothMobileEmail    = 0;
        int hasMobileNoEmail      = 0;
        int hasEmailNoMobile      = 0;
        int hasNeitherMobileEmail = 0;
        RegisterReferrerDB.GetCountsByEmailMobile(
            (RegisterReferrer[])referrersAdded.ToArray(typeof(RegisterReferrer)),
            ref hasBothMobileEmail,
            ref hasMobileNoEmail,
            ref hasEmailNoMobile,
            ref hasNeitherMobileEmail);

        lblReferrersWithMobileAndEmailTotal.Text       = hasBothMobileEmail.ToString();
        lblReferrersWithMobileNoEmailTotal.Text        = hasMobileNoEmail.ToString();
        lblReferrersWithEmailNoMobileTotal.Text        = hasEmailNoMobile.ToString();
        lblReferrersWithNeitherMobileOrEmailTotal.Text = hasNeitherMobileEmail.ToString();
        lblReferrerCount.Text = lstReferrers.Items.Count.ToString();
    }


    protected void btnUpdateReferrersFromClinic_Click(object sender, EventArgs e)
    {
        int orgID = Convert.ToInt32(hiddenUpdateReferrersFromClinic_OrgID.Value);
        Organisation org = OrganisationDB.GetByID(orgID);

        if (org != null)
        {
            string referrerIDs = hiddenReferrerIDsList.Value;  // comma seperated
            Hashtable referrerIDsHash = new Hashtable();
            if (referrerIDs.Length > 0)
                foreach (string referrerID in referrerIDs.Split(','))
                    referrerIDsHash[Convert.ToInt32(referrerID)] = 1;


            // add all new ones (not including those already in there)
            RegisterReferrer[] refs = RegisterReferrerDB.GetAllActiveRegRefByPatientsOfInternalOrg(orgID);
            for (int i = 0; i < refs.Length; i++)
            {
                if (referrerIDsHash[refs[i].RegisterReferrerID] == null)
                {
                    hiddenReferrerIDsList.Value += (hiddenReferrerIDsList.Value.Length == 0 ? "" : ",") + refs[i].RegisterReferrerID; // add to hiddenfield list of id's
                    referrerIDsHash[refs[i].RegisterReferrerID] = 1;  // add to hash
                }
            }


            // clear and re-set all (in alphabetical order)

            ArrayList newList = new ArrayList();
            DataTable referrers = RegisterReferrerDB.GetDataTable();
            for (int i = 0; i < referrers.Rows.Count; i++)
                newList.Add(RegisterReferrerDB.LoadAll(referrers.Rows[i]));

            RegisterReferrer[] allReferrers = (RegisterReferrer[])newList.ToArray(typeof(RegisterReferrer));

            ArrayList referrersAdded = new ArrayList();
            lstReferrers.Items.Clear();
            for (int i = 0; i < allReferrers.Length; i++)
            {
                if (referrerIDsHash[allReferrers[i].RegisterReferrerID] != null)
                {
                    string text = allReferrers[i].Referrer.Person.FullnameWithoutMiddlename + " [" + allReferrers[i].Organisation.Name + "]";
                    lstReferrers.Items.Add(new ListItem(text, allReferrers[i].RegisterReferrerID.ToString()));

                    referrersAdded.Add(allReferrers[i]);
                }
            }


            int hasBothMobileEmail    = 0;
            int hasMobileNoEmail      = 0;
            int hasEmailNoMobile      = 0;
            int hasNeitherMobileEmail = 0;
            RegisterReferrerDB.GetCountsByEmailMobile(
                (RegisterReferrer[])referrersAdded.ToArray(typeof(RegisterReferrer)),
                ref hasBothMobileEmail,
                ref hasMobileNoEmail,
                ref hasEmailNoMobile,
                ref hasNeitherMobileEmail);

            lblReferrersWithMobileAndEmailTotal.Text       = hasBothMobileEmail.ToString();
            lblReferrersWithMobileNoEmailTotal.Text        = hasMobileNoEmail.ToString();
            lblReferrersWithEmailNoMobileTotal.Text        = hasEmailNoMobile.ToString();
            lblReferrersWithNeitherMobileOrEmailTotal.Text = hasNeitherMobileEmail.ToString();
            lblReferrerCount.Text = lstReferrers.Items.Count.ToString();

            // -----------------------------------------------

        }

        hiddenUpdateReferrersFromClinic_OrgID.Value = string.Empty;
    }

    
    protected void ddlLetterTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateLettersList();
    }

    protected void PopulateLettersList()
    {
        DataTable letters = LetterDB.GetDataTable_ByOrg(0, Convert.ToInt32(Session["SiteID"]));

        // remove ones that dont exists
        for (int i = letters.Rows.Count - 1; i >= 0; i--)
        {
            Letter letter = LetterDB.LoadAll(letters.Rows[i]);
            if (!letter.FileExists(Convert.ToInt32(Session["SiteID"])))
                letters.Rows.RemoveAt(i);
        }

        lstLetters.DataSource     = letters;
        lstLetters.DataTextField  = "letter_docname";
        lstLetters.DataValueField = "letter_letter_id";
        lstLetters.DataBind();
    }

    protected bool LetterExists(int letterID)
    {
        Letter letter = LetterDB.GetByID(letterID);
        bool useDefaultDocs = letter.Organisation == null ? true : !LetterDB.OrgHasdocs(letter.Organisation.OrganisationID);

        string dir = Letter.GetLettersDirectory();
        return (File.Exists(dir + (useDefaultDocs ? "" : letter.Organisation.OrganisationID + @"\") + letter.Docname));
    }


    #endregion



    #region GetCache & GetPhoneNbr/GetEmail from hashtable caches

    protected static Regex re = new Regex("[^0-9]"); // new Regex("[^0-9 -,]");

    protected static string GetPhoneNbr(Hashtable contactHash, int entityID, bool onlyMobile)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] contacts = (Contact[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            foreach (Contact c in contacts)
            {
                if (onlyMobile && c.ContactType.ContactTypeID != 30)  // ignore if not mobile nbr
                    continue;

                string phNum = re.Replace(c.AddrLine1, "").Trim();
                if (phNum.Length > 0)
                    return phNum;
            }

        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] contacts = (ContactAus[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            foreach (ContactAus c in contacts)
            {
                if (onlyMobile && c.ContactType.ContactTypeID != 30)  // ignore if not mobile nbr
                    continue;

                string phNum = re.Replace(c.AddrLine1, "").Trim();
                if (phNum.Length > 0)
                    return phNum;
            }

        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        return null;
    }

    protected static string GetPhoneNbrs(Hashtable contactHash, int entityID)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] contacts = (Contact[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            string nbrs = string.Empty;
            for (int i = 0; i < contacts.Length; i++)
            {
                if (i > 0 && i == contacts.Length - 1)
                    nbrs += " or " + re.Replace(contacts[i].AddrLine1, "").Trim();
                else if (i > 0)
                    nbrs += ", " + re.Replace(contacts[i].AddrLine1, "").Trim();
                else
                    nbrs += re.Replace(contacts[i].AddrLine1, "").Trim();
            }

            return nbrs;

        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] contacts = (ContactAus[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            string nbrs = string.Empty;
            for (int i = 0; i < contacts.Length; i++)
            {
                if (i > 0 && i == contacts.Length - 1)
                    nbrs += " or " + re.Replace(contacts[i].AddrLine1, "").Trim();
                else if (i > 0)
                    nbrs += ", " + re.Replace(contacts[i].AddrLine1, "").Trim();
                else
                    nbrs += re.Replace(contacts[i].AddrLine1, "").Trim();
            }

            return nbrs;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
    }

    protected static string GetEmail(Hashtable contactHash, int entityID)
    {
        return ContactDB.GetEmailsCommaSepByEntityID(contactHash, entityID, false, true);
    }

    #endregion

    #region SendMethod, GetSendMethod()

    protected enum SendMethod { Print = 1, Email = 2, SMS = 3, None = -1 };
    protected SendMethod GetSendMethod(bool hasEmail, bool hasMobile)
    {
        if (hasEmail && hasMobile)
            return (SendMethod)Convert.ToInt32(ddlBothMobileAndEmail.SelectedValue);
        else if (hasEmail && !hasMobile)
            return (SendMethod)Convert.ToInt32(ddlEmailNoMobile.SelectedValue);
        else if (!hasEmail && hasMobile)
            return (SendMethod)Convert.ToInt32(ddlMobileNoEmail.SelectedValue);
        else // if (!hasEmail && !hasMobile)
            return (SendMethod)Convert.ToInt32(ddlNeitherMobileOrEmail.SelectedValue);
    }

    #endregion

    #region PrintLetter

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
	        decimal smsBalance  = SMSCreditDataDB.GetTotal() - SMSHistoryDataDB.GetTotal();
	        decimal smsCost     = Convert.ToDecimal(SystemVariableDB.GetByDescr("SMSPrice").Value);

            int maxSMSCountCanAfford = smsCost == 0 ? 1000000 : (int)(smsBalance / smsCost);
            int smsCountSending = 0;


            //
            // Start Validation
            //

            txtEmailSubject.Text     = txtEmailSubject.Text.Trim();
            txtEmailForPrinting.Text = txtEmailForPrinting.Text.Trim();
            txtSMSText.Text          = txtSMSText.Text.Trim();


            bool printSelected = (ddlBothMobileAndEmail.SelectedValue   == "1" || ddlEmailNoMobile.SelectedValue        == "1" ||
                                  ddlMobileNoEmail.SelectedValue        == "1" || ddlNeitherMobileOrEmail.SelectedValue == "1");
            bool emailSelected = (ddlBothMobileAndEmail.SelectedValue   == "2" || ddlEmailNoMobile.SelectedValue        == "2" ||
                                  ddlMobileNoEmail.SelectedValue        == "2" || ddlNeitherMobileOrEmail.SelectedValue == "2");
            bool smsSelected   = (ddlBothMobileAndEmail.SelectedValue   == "3" || ddlEmailNoMobile.SelectedValue        == "3" ||
                                  ddlMobileNoEmail.SelectedValue        == "3" || ddlNeitherMobileOrEmail.SelectedValue == "3");


            string validationErrors = string.Empty;

            if (printSelected)
            {
                if (txtEmailForPrinting.Text.Length == 0)
                    validationErrors += "<li>Printed Batch Letters Email Address To Send To can not be empty.</li>";
                else if (!Utilities.IsValidEmailAddress(txtEmailForPrinting.Text))
                    validationErrors += "<li>Printed Batch Letters Email Address To Send To must look like a valid email address.</li>";
            }
            if (emailSelected)
            {
                if (txtEmailSubject.Text.Length == 0)
                    validationErrors += "<li>Email Subject can not be empty.</li>";
                if (FreeTextBox1.Text.Length == 0)
                    validationErrors += "<li>Email Text can not be empty.</li>";
            }
            if (smsSelected)
            {
                if (smsCost > 0 && smsBalance == 0)
                    validationErrors += "<li>Can not send SMS's - your SMS balance is empty. Please topup or unselect sending by SMS.</li>";
                else if (txtSMSText.Text.Length == 0)
                    validationErrors += "<li>SMS Text can not be empty.</li>";
            }

            if (validationErrors.Length > 0)
                throw new CustomMessageException("<ul>" + validationErrors + "</ul>");

            //
            // End Validation
            //



            //
            // get hashtables of those with mobiles and emails
            //

            ArrayList regRefIDsArr = new ArrayList();


            foreach (ListItem referrerItem in lstReferrers.Items)  // regrefid
                if (referrerItem.Selected)
                    regRefIDsArr.Add(Convert.ToInt32(referrerItem.Value));


            int[]     regRefIDs    = (int[])regRefIDsArr.ToArray(typeof(int));
            int[]     entityIDs    = RegisterReferrerDB.GetOrgEntityIDs(regRefIDs);
            Hashtable entityIDHash = RegisterReferrerDB.GetOrgEntityIDsHash(regRefIDs);
            Hashtable regRefIDHash = RegisterReferrerDB.GetByIDsInHashtable(regRefIDs);

            Hashtable emailHash    = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);
            Hashtable mobileHash   = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1, "30");

            string email_from_address = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value;
            string email_from_name    = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value;

            //bool StoreLettersHistoryInDB       = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["StoreLettersHistoryInDB"]);
            //bool StoreLettersHistoryInFlatFile = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["StoreLettersHistoryInFlatFile"]);
            bool StoreLettersHistoryInDB       = false; // don't store bulk marketing letters
            bool StoreLettersHistoryInFlatFile = false; // don't store bulk marketing letters



            //
            // ok start the sending process
            //


            int bulkLetterSendingQueueBatchID = UseBulkLetterSender ? BulkLetterSendingQueueBatchDB.Insert(txtEmailForPrinting.Text, false) : -1;


            // TODO: Send Letter By Email
            int letterPrintHistorySendMethodID = 1; // send by mail


            // make sure at least one referrer selected
            if (lstReferrers.GetSelectedIndices().Length == 0)
                throw new CustomMessageException("Please select at least one referrer.");

            // make sure at least one letter selected
            if (lstLetters.GetSelectedIndices().Length == 0)
                throw new CustomMessageException("Please select a letter.");


            // get letter and make sure it exists
            Letter letter = LetterDB.GetByID(Convert.ToInt32(lstLetters.SelectedValue));
            string sourchTemplatePath = letter.GetFullPath(Convert.ToInt32(Session["SiteID"]));
            if (!File.Exists(sourchTemplatePath))
                throw new CustomMessageException("File doesn't exist.");

            // get temp directory
            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            if (!Directory.Exists(tmpLettersDirectory))
                throw new CustomMessageException("Temp letters directory doesn't exist");

            // delete old tmp files
            FileHelper.DeleteOldFiles(tmpLettersDirectory, new TimeSpan(1, 0, 0));



            // create individual merged docs and put into list of docsToMerge - only if there is an org-patient relationship
            ArrayList docsToMerge = new ArrayList();

            Site site   = SiteDB.GetByID(Convert.ToInt32(Session["SiteID"]));
            int StaffID = Convert.ToInt32(Session["StaffID"]);
            foreach (ListItem referrerItem in lstReferrers.Items)
            {
                if (!referrerItem.Selected)
                    continue;

                if (UseBulkLetterSender)
                {
                    int        refEntityID    = (int)entityIDHash[Convert.ToInt32(referrerItem.Value)];
                    string     refPhoneNumber = GetPhoneNbr(mobileHash, refEntityID, true);
                    string     refEmail       = GetEmail(emailHash,     refEntityID);
                    SendMethod sendMethod     = GetSendMethod(refEmail != null, refPhoneNumber != null);

                    RegisterReferrer regRef = RegisterReferrerDB.GetByID(Convert.ToInt32(referrerItem.Value));

                    if (sendMethod != SendMethod.None)
                    {
                        string text = string.Empty;
                        if (sendMethod == SendMethod.SMS)
                            text = txtSMSText.Text;
                        if (sendMethod == SendMethod.Email)
                            text = FreeTextBox1.Text;

                        text = ReplaceMergeFields(text, regRefIDHash, Convert.ToInt32(referrerItem.Value));

                        bool generateLetter = false;
                        if (sendMethod == SendMethod.SMS)
                            generateLetter = false;
                        if (sendMethod == SendMethod.Email)
                            generateLetter = lstLetters.GetSelectedIndices().Length != 0;
                        if (sendMethod == SendMethod.Print)
                            generateLetter = true;


                        if (sendMethod == SendMethod.SMS)  // copy to other methods!!
                            smsCountSending++;


                        BulkLetterSendingQueueDB.Insert
                        (
                            bulkLetterSendingQueueBatchID,
                            (int)sendMethod,                     // bulk_letter_sending_queue_method_id
                            StaffID,                             // added_by
                            -1,                                  // patient_id
                            regRef.Referrer.ReferrerID,          // referrer_id
                            -1,                                  // booking_id
                            (sendMethod == SendMethod.SMS)   ? refPhoneNumber       : "",  // phone_number
                            (sendMethod == SendMethod.Email) ? refEmail             : "",  // email_to_address
                            "",                                                            // email_to_name
                            (sendMethod == SendMethod.Email) ? email_from_address   : "",  // email_from_address
                            (sendMethod == SendMethod.Email) ? email_from_name      : "",  // email_from_name
                            text,                                                          // text
                            (sendMethod == SendMethod.Email) ? txtEmailSubject.Text : "",  // email_subject
                            "",    // email_attachment_location
                            false, // email_attachment_delete_after_sending
                            false, // email_attachment_folder_delete_after_sending

                            !generateLetter ? -1    : letter.LetterID,
                            !generateLetter ? false : chkKeepInHistory.Checked && StoreLettersHistoryInDB,
                            !generateLetter ? false : chkKeepInHistory.Checked && StoreLettersHistoryInFlatFile,
                            !generateLetter ? -1    : letterPrintHistorySendMethodID,
                            !generateLetter ? ""    : Letter.GetLettersHistoryDirectory(0),
                            !generateLetter ? ""    : letter.Docname.Replace(".dot", ".doc"),
                            !generateLetter ? -1    : site.SiteID,
                             0,    // organisation_id
                            -1,    // booking id
                            -1,    // patient_id

                            !generateLetter ? -1    : Convert.ToInt32(referrerItem.Value),  // register_referrer_id_to_use_instead_of_patients_reg_ref
                            !generateLetter ? -1    : StaffID,
                            -1,    //healthcardactionid
                            !generateLetter ? ""    : sourchTemplatePath,
                            !generateLetter ? ""    : tmpLettersDirectory + letter.Docname.Replace(".dot", ".doc"),
                            !generateLetter ? false : true,

                            "",    // email_letter_extra_pages
                            "",    // email_letter_item_seperator
                            "",    // sql_to_run_on_completion
                            ""     // sql_to_run_on_failure
                        );

                    }

                }
                else
                {
                    // create doc
                    string tmpSingleFileName = Letter.CreateMergedDocument(
                        letter.LetterID,
                        chkKeepInHistory.Checked && StoreLettersHistoryInDB,
                        chkKeepInHistory.Checked && StoreLettersHistoryInFlatFile,
                        letterPrintHistorySendMethodID,
                        Letter.GetLettersHistoryDirectory(0),
                        letter.Docname.Replace(".dot", ".doc"),
                        site,
                         0, // org id
                        -1, // booking id
                        -1, // patient id
                        Convert.ToInt32(referrerItem.Value),
                        StaffID,
                        -1, //healthcardactionid
                        sourchTemplatePath,
                        tmpLettersDirectory + letter.Docname.Replace(".dot", ".doc"),
                        true);

                    // record name of merged doc
                    docsToMerge.Add(tmpSingleFileName);
                }

            }


            if (UseBulkLetterSender)
            {
                if ((smsCountSending * smsCost) > smsBalance)
                {
                    BulkLetterSendingQueueDB.DeleteByBatchID(bulkLetterSendingQueueBatchID);
                    BulkLetterSendingQueueBatchDB.Delete(bulkLetterSendingQueueBatchID);

                    SetErrorMessage("Not Enough Credit To Send SMS's. Please Top Up You SMS Credit or Choose Methods Other Than SMS.");
                    return;
                }

                BulkLetterSendingQueueBatchDB.UpdateReadyToProcess(bulkLetterSendingQueueBatchID, true);
                SetErrorMessage("Items Added To Sending Queue. View Details <a href='/Letters_PrintBatch_StatusV2.aspx?batch_id=" + bulkLetterSendingQueueBatchID + "'>Here</a>");
            }
            else
            {
                // merge all tmp files
                string tmpFinalFileName = Letter.MergeMultipleDocuments(
                    ((string[])docsToMerge.ToArray(typeof(string))),
                    tmpLettersDirectory + letter.Docname.Replace(".dot", ".doc"));

                // delete all single tmp files
                foreach(string file in docsToMerge)
                    File.Delete(file);

                // download the document
                byte[] fileContents = File.ReadAllBytes(tmpFinalFileName);
                System.IO.File.Delete(tmpFinalFileName);

                // Nothing gets past the "DownloadDocument" method because it outputs the file 
                // which is writing a response to the client browser and calls Response.End()
                // So make sure any other code that functions goes before this
                Letter.DownloadDocument(Response, fileContents, letter.Docname.Replace(".dot", ".doc"));
            }

        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
    }

    public string ReplaceMergeFields(string text, Hashtable regRefIDHash, int regRefID)
    {
        if (text.Contains("{ref_name}")       ||
            text.Contains("{ref_title}")      ||
            text.Contains("{ref_firstname}")  ||
            text.Contains("{ref_surname}"))
        {
            RegisterReferrer regRef = (RegisterReferrer)regRefIDHash[regRefID];

            string title = string.Empty;
            if (regRef.Referrer.Person.Title.ID != 0)
                title = regRef.Referrer.Person.Title.Descr;
            else if (regRef.Referrer.Person.Title.ID == 0 && regRef.Referrer.Person.Gender == "M")
                title = "Mr.";
            else if (regRef.Referrer.Person.Title.ID == 0 && regRef.Referrer.Person.Gender == "F")
                title = "Ms.";

            text = text
                .Replace("{ref_name}"     , regRef.Referrer.Person.FullnameWithoutMiddlename)
                .Replace("{ref_title}"    , title)
                .Replace("{ref_firstname}", regRef.Referrer.Person.Firstname)
                .Replace("{ref_surname}"  , regRef.Referrer.Person.Surname)
                ;
        }

        return text;
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        SetErrorMessage(errMsg, details);
    }
    private void SetErrorMessage(string errMsg = "", string details = "")
    {
        if (errMsg.Contains(Environment.NewLine))
            errMsg = errMsg.Replace(Environment.NewLine, "<br />");

        // double escape so shows up literally on webpage for 'alert' message
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"#\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

        lblErrorMessage.Visible = true;
        if (errMsg != null && errMsg.Length > 0)
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}