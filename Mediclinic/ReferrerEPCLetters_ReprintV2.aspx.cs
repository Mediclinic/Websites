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

public partial class ReferrerEPCLetters_ReprintV2 : System.Web.UI.Page
{

    public static bool   LogDebugEmailInfo    = true;   //******
    public static bool   UseBulkLetterSender  = true;   //******
    public static bool   UseDebugEmail        = false;  //******
    
    public static string DebugEmail           = "eli@elipollak.com";
    public static int    MaxSending           = 175;





    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);
                SetupGUI();
                FillForm();

                Run(true);
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

    #region GetUrlParamCard(), GetUrlParamType(), IsValidFormID(), GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new Exception("Invalid url id");

        string id = Request.QueryString["id"];
        return Convert.ToInt32(id);
    }

    private enum UrlParamType { Add, Edit, View, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "add")
            return UrlParamType.Add;
        //else if (type != null && type.ToLower() == "edit")
        //    return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "view")
            return UrlParamType.View;
        else
            return UrlParamType.None;
    }

    #endregion

    #region SetupGUI()

    public void SetupGUI()
    {
        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate',   this, 'dmy', '-'); return false;";

        txtStartDate.Text = DateTime.Today.AddDays(-7).ToString(@"dd-MM-yyyy");
    }

    #endregion

    #region FillForm()

    private void FillForm()
    {
        // Heading.InnerText = isEditMode ? "Edit Credit Note" : "View Credit Note";


    }

    #endregion


    #region Run

    protected enum SendMethod { Print, Email, None };
    protected SendMethod SelectedSendMethod
    {
        get
        {
            if (rdioSendType.SelectedIndex == -1)
                return SendMethod.None;
            else if (rdioSendType.SelectedValue == "Print")
                return SendMethod.Print;
            else if (rdioSendType.SelectedValue == "Email")
                return SendMethod.Email;
            else
                throw new ArgumentException("Unknown Send Method: " + rdioSendType.SelectedValue);
        }
    }

    protected void btnViewList_Click(object sender, EventArgs e)
    {
        Run(true);
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        Run(false);
    }

    protected void Run(bool viewListOnly)
    {
        bool debugMode = true;

        int bulkLetterSendingQueueBatchID = !viewListOnly && UseBulkLetterSender ? BulkLetterSendingQueueBatchDB.Insert(DebugEmail, false) : -1;

        //
        //  We can not send email all their patients in one email - will be too big with attachments and rejected by their mail provider
        //  So if via email - need to send one at a time
        //  Then if cuts out or times out, it has processed some so don't need to re-process those when it's run again
        //
        //  just remember to process the emails first ... so if any interruptions/errors ... at least some will have been processed
        //



        try
        {

            bool AutoSendFaxesAsEmailsIfNoEmailExistsToGPs = Convert.ToInt32(SystemVariableDB.GetByDescr("AutoSendFaxesAsEmailsIfNoEmailExistsToGPs").Value) == 1;

            string sendMethod = rdioSendType.SelectedValue;
            if (!viewListOnly && SelectedSendMethod == SendMethod.None)
                throw new CustomMessageException("Send method not selected");

            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            if (!Directory.Exists(tmpLettersDirectory))
                throw new CustomMessageException("Temp letters directory doesn't exist");


            string debugOutput = string.Empty;
            int startTime = Environment.TickCount;




            // NB. 
            // start/emd date time __must__ refer to the treatment date because
            // as a letter can be generated any time (days or weeks) after a treatment, there is no way 
            // to be sure which invoice it is attached to, only which booking
            if (txtStartDate.Text.Length > 0 && !Utilities.IsValidDate(txtStartDate.Text, "dd-mm-yyyy"))
                throw new CustomMessageException("Start date must be empty or valid and of the format dd-mm-yyyy");
            if (txtEndDate.Text.Length > 0 && !Utilities.IsValidDate(txtEndDate.Text, "dd-mm-yyyy"))
                throw new CustomMessageException("End date must be empty or valid and of the format dd-mm-yyyy");
            DateTime startDate = txtStartDate.Text.Length == 0 ? DateTime.MinValue : Utilities.GetDate(txtStartDate.Text, "dd-mm-yyyy");
            DateTime endDate   = txtEndDate.Text.Length   == 0 ? DateTime.MinValue : Utilities.GetDate(txtEndDate.Text,   "dd-mm-yyyy");


            DataTable bookingsWithSentLetters = BookingDB.GetBookingsWithEPCLetters(startDate, endDate, Convert.ToInt32(registerReferrerID.Value), Convert.ToInt32(patientID.Value), true, false);

            if (!viewListOnly && bookingsWithSentLetters.Rows.Count > MaxSending)
                throw new CustomMessageException("Can not generate more than " + MaxSending + " letters at a time. Please narrow your date range.");

            double queryExecutionTime = (double)(Environment.TickCount - startTime) / 1000.0;
            startTime = Environment.TickCount;


            ArrayList filesToPrint = new ArrayList();

            int currentRegReferrerID = -1;
            ArrayList bookingsForCurrentReferrer = new ArrayList();
            foreach (DataRow row in bookingsWithSentLetters.Rows)
            {
                Tuple<Booking, PatientReferrer, bool, bool, HealthCard> rowData = LoadRow(row);
                Booking         booking     = rowData.Item1;
                PatientReferrer pr          = rowData.Item2;
                bool            refHasEmail = rowData.Item3;
                bool            refHasFax   = rowData.Item4;
                HealthCard      hc          = rowData.Item5;


                if (pr.RegisterReferrer.RegisterReferrerID != currentRegReferrerID)
                {
                    filesToPrint.AddRange(ProcessReferrersLetters(viewListOnly, bookingsForCurrentReferrer, AutoSendFaxesAsEmailsIfNoEmailExistsToGPs, ref debugOutput, bulkLetterSendingQueueBatchID));
                    currentRegReferrerID = pr.RegisterReferrer.RegisterReferrerID;
                    bookingsForCurrentReferrer = new ArrayList();
                }

                bookingsForCurrentReferrer.Add(rowData);
            }

            // process last group
            filesToPrint.AddRange(ProcessReferrersLetters(viewListOnly, bookingsForCurrentReferrer, AutoSendFaxesAsEmailsIfNoEmailExistsToGPs, ref debugOutput, bulkLetterSendingQueueBatchID));



            bool zipSeperately = true;

            if (zipSeperately && filesToPrint.Count > 0)
            {

                // seperate into doc types because can only merge docs with docs of same template (ie docname)
                Hashtable filesToPrintHash = new Hashtable();
                for (int i = 0; i < filesToPrint.Count; i++)
                {
                    Letter.FileContents curFileContents = (Letter.FileContents)filesToPrint[i];
                    if (filesToPrintHash[curFileContents.DocName] == null)
                        filesToPrintHash[curFileContents.DocName] = new ArrayList();
                    ((ArrayList)filesToPrintHash[curFileContents.DocName]).Add(curFileContents);
                }

                // merge and put merged files into temp dir
                string baseTmpDir = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
                string tmpDir = baseTmpDir + "EPC Letters" + @"\";
                Directory.CreateDirectory(tmpDir);
                string[] tmpFiles = new string[filesToPrintHash.Keys.Count];
                IDictionaryEnumerator enumerator = filesToPrintHash.GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++)
                {
                    ArrayList files = (ArrayList)enumerator.Value;
                    string docName = (string)enumerator.Key;


                    // last file is screwing up, so just re-add the last file again for a temp fix
                    files.Add(files[files.Count - 1]);


                    Letter.FileContents fileContents = Letter.FileContents.Merge((Letter.FileContents[])files.ToArray(typeof(Letter.FileContents)), docName); // .pdf

                    string tmpFileName = tmpDir + fileContents.DocName;
                    System.IO.File.WriteAllBytes(tmpFileName, fileContents.Contents);
                    tmpFiles[i] = tmpFileName;
                }

                // zip em
                string zipFileName = "EPC Letters.zip";
                string zipFilePath = baseTmpDir + zipFileName;
                ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                zip.CreateEmptyDirectories = true;
                zip.CreateZip(zipFilePath, tmpDir, true, "");

                // get filecontents of zip here
                Letter.FileContents zipFileContents = new Letter.FileContents(zipFilePath, zipFileName);
                Session["downloadFile_Contents"] = zipFileContents.Contents;
                Session["downloadFile_DocName"] = zipFileContents.DocName;

                // delete files
                for (int i = 0; i < tmpFiles.Length; i++)
                {
                    System.IO.File.SetAttributes(tmpFiles[i], FileAttributes.Normal);
                    System.IO.File.Delete(tmpFiles[i]);
                }
                System.IO.File.SetAttributes(zipFilePath, FileAttributes.Normal);
                System.IO.File.Delete(zipFilePath);
                System.IO.Directory.Delete(tmpDir, false);
                System.IO.Directory.Delete(baseTmpDir, false);

                // put in session variables so when it reloads to this page, we can popup the download window 
                Page.ClientScript.RegisterStartupScript(this.GetType(), "download", "<script language=javascript>window.open('DownloadFile.aspx','_blank','status=1,toolbar=0,menubar=0,location=1,scrollbars=1,resizable=1,width=30,height=30');</script>");
            }

            if (!zipSeperately && filesToPrint.Count > 0)
            {
                Letter.FileContents fileContents = Letter.FileContents.Merge((Letter.FileContents[])filesToPrint.ToArray(typeof(Letter.FileContents)), "Referral Letters.doc");  // .pdf
                Session["downloadFile_Contents"] = fileContents.Contents;
                Session["downloadFile_DocName"] = fileContents.DocName;

                // put in session variables so when it reloads to this page, we can popup the download window 
                Page.ClientScript.RegisterStartupScript(this.GetType(), "download", "<script language=javascript>window.open('DownloadFile.aspx','_blank','status=1,toolbar=0,menubar=0,location=1,scrollbars=1,resizable=1,width=30,height=30');</script>");
            }


            double restExecutionTime = (double)(Environment.TickCount - startTime) / 1000.0;

            if (debugMode)
            {
                if (!viewListOnly)
                    lblInfo.Text = @"<table cellpadding=""0"">
                                    <tr><td><b>Send Method</b></td><td style=""width:10px;""></td><td>" + SelectedSendMethod.ToString() + @"</td><td style=""width:25px;""></td><td><b>Query Time</b></td><td style=""width:10px;""></td><td>" + queryExecutionTime + @" seconds</td></tr>
                                    <tr><td><b>Count</b></td><td style=""width:10px;""></td><td>" + bookingsWithSentLetters.Rows.Count + @"</td><td style=""width:25px;""></td><td><b>Runing Time</b></td><td style=""width:10px;""></td><td>" + restExecutionTime + @" seconds</td></tr>
                                 </table>";

                string countShowing = bookingsWithSentLetters.Rows.Count > MaxSending ? bookingsWithSentLetters.Rows.Count + " showing to re-generate. <br /><font color=red>* You can not generate more than 175 at a time. Please narrow your search before printing.</font>" : bookingsWithSentLetters.Rows.Count.ToString();
                if (viewListOnly)
                    lblInfo.Text = @"<table cellpadding=""0"">
                                    <tr><td valign=""top""><b>Count</b></td><td style=""width:10px;""></td><td>" + countShowing + @"</td></tr>
                                 </table>";


                if (viewListOnly)
                    lblList.Text = @"<table class=""table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center"" cellpadding=""4"" border=""1"">
                                        <tr>
                                            <th style=""white-space:nowrap;"">Send By</th>
                                            <th>Booking</th>
                                            <th>Generate</th>
                                            <th>Referrer</th>
                                            <th>Email</th>
                                            <th>Fax</th>
                                            <th>Patient</th>
                                        </tr>" +
                                        (debugOutput.Length == 0 ? "<tr><td colspan=\"7\">No Rows</td></tr>" : debugOutput) +
                                    "</table>";
            }
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch (Exception ex)
        {
            SetErrorMessage("", ex.ToString());
        }

    }

    protected Letter.FileContents[] ProcessReferrersLetters(bool viewListOnly, ArrayList bookingsForCurrentReferrer, bool autoSendFaxesAsEmailsIfNoEmailExistsToGPs, ref string debugOutput, int bulkLetterSendingQueueBatchID)
    {
        if (bookingsForCurrentReferrer.Count == 0)
            return new Letter.FileContents[0];


        // to return - only files to print, as emailing will have been completed
        ArrayList filesToPrint = new ArrayList();


        // single db lookup per referrer to get email
        Tuple<Booking, PatientReferrer, bool, bool, HealthCard> firstTuple = (Tuple<Booking, PatientReferrer, bool, bool, HealthCard>)bookingsForCurrentReferrer[0];
        PatientReferrer firstPR          = firstTuple.Item2;


        int s = firstPR.RegisterReferrer.RegisterReferrerID;



        string[] refEmails = ContactDB.GetEmailsByEntityID(firstPR.RegisterReferrer.Organisation.EntityID);
        string[] refFaxes  = ContactDB.GetFaxesByEntityID(firstPR.RegisterReferrer.Organisation.EntityID);

        bool     firstRefHasEmail = refEmails.Length > 0;
        string   refEmail         = refEmails.Length > 0 ? string.Join(",", refEmails) : null;

        bool     firstRefHasFax = refFaxes.Length > 0;
        string   refFax         = refFaxes.Length > 0 ? refFaxes[0] : null;


        int  siteID  = Convert.ToInt32(Session["SiteID"]);
        Site site    = SiteDB.GetByID(siteID);
        int  staffID = Convert.ToInt32(Session["StaffID"]);
        Site[] sites = SiteDB.GetAll();
        for (int i = 0; i < bookingsForCurrentReferrer.Count; i++)
        {
            Tuple<Booking, PatientReferrer, bool, bool, HealthCard> curTuple = (Tuple<Booking, PatientReferrer, bool, bool, HealthCard>)bookingsForCurrentReferrer[i];
            Booking         curBooking     = curTuple.Item1;
            PatientReferrer curPR          = curTuple.Item2;
            bool            curRefHasEmail = curTuple.Item3;
            bool            curRefHasFax   = curTuple.Item4;
            HealthCard      curHC          = curTuple.Item5;


            bool needToGenerateLetters = curBooking.NeedToGenerateFirstLetter || curBooking.NeedToGenerateLastLetter ||
                                        (curPR.RegisterReferrer.ReportEveryVisitToReferrer && curBooking.NoteCount > 0);
            if (needToGenerateLetters)
            {
                SendMethod sendMethod = (curRefHasEmail && this.SelectedSendMethod == SendMethod.Email ? SendMethod.Email : SendMethod.Print);

                if (!viewListOnly)
                {
                    bool sendViaEmail = autoSendFaxesAsEmailsIfNoEmailExistsToGPs ? (curRefHasEmail || curRefHasFax) : curRefHasEmail;
                    if (sendViaEmail && this.SelectedSendMethod == SendMethod.Email)
                    {

                        string toEmail = autoSendFaxesAsEmailsIfNoEmailExistsToGPs ? 
                            (curRefHasEmail ? refEmail : Regex.Replace(refFax, "[^0-9]", "") + "@fax.houseofit.com.au") 
                            : 
                            refEmail;

                        if (UseBulkLetterSender)
                        {
                            BulkLetterSendingQueueAdditionalLetter[] filesList = GetFilesInfo(curBooking, Letter.FileFormat.PDF, curBooking.Patient, curHC, curBooking.Offering.Field.ID, curPR.RegisterReferrer.Referrer, false, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, curPR.RegisterReferrer.ReportEveryVisitToReferrer, siteID, staffID, sendMethod == SendMethod.Email ? 2 : 1);

                            if (filesList != null && filesList.Length > 0)
                            {
                                string from_email = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value;
                                string subject    = "Referral/Treatment Note Letters From Mediclinic" + (curPR.Patient == null ? string.Empty : " For " + curPR.Patient.Person.FullnameWithoutMiddlename);
                                string text       = "Dr. " + curPR.RegisterReferrer.Referrer.Person.FullnameWithoutMiddlename + "<br /><br />Please find attached referral/treatment note letters for your referrered patient" + (curPR.Patient == null ? string.Empty : " <b>" + curPR.Patient.Person.FullnameWithoutMiddlename + "</b>") + "<br /><br />Best regards,<br />" + site.Name;

                                int bulk_letter_sending_queue_id = BulkLetterSendingQueueDB.Insert
                                (
                                    bulkLetterSendingQueueBatchID,
                                    2,                                   // bulk_letter_sending_queue_method_id (2 = email)
                                    staffID,                             // added_by
                                    curPR.Patient.PatientID,                    // patient_id
                                    curPR.RegisterReferrer.Referrer.ReferrerID, // referrer_id
                                    curBooking.BookingID,                       // booking_id
                                    "",          // phone_number
                                    toEmail,     // email_to_address
                                    "",          // email_to_name
                                    from_email,  // email_from_address
                                    site.Name,   // email_from_name
                                    text,        // text
                                    subject,     // email_subject
                                    "",    // email_attachment_location
                                    false, // email_attachment_delete_after_sending
                                    false, // email_attachment_folder_delete_after_sending

                                    filesList[0].EmailLetterLetterID,
                                    filesList[0].EmailLetterKeepHistoryInDb,
                                    filesList[0].EmailLetterKeepHistoryInFile,
                                    filesList[0].EmailLetterLetterPrintHistorySendMethodID,
                                    filesList[0].EmailLetterHistoryDir,
                                    filesList[0].EmailLetterHistoryFilename,
                                    filesList[0].EmailLetterSiteID,
                                    filesList[0].EmailLetterOrganisationID,
                                    filesList[0].EmailLetterBookingID,
                                    filesList[0].EmailLetterPatientID,
                                    filesList[0].EmailLetterRegisterReferrerIdToUseInsteadOfPatientsRegRef,
                                    filesList[0].EmailLetterStaffID,
                                    filesList[0].EmailLetterHealthCardActionID,
                                    filesList[0].EmailLetterSourceTemplatePath,
                                    filesList[0].EmailLetterOutputDocPath,
                                    false, // filesList[0].EmailLetterIsDoubleSidedPrinting,
                                    filesList[0].EmailLetterExtraPages,
                                    filesList[0].EmailLetterItemSeperator,

                                    "",    // sql_to_run_on_completion
                                    ""     // sql_to_run_on_failure
                                );

                                for (int f = 1; f < filesList.Length; f++)
                                {
                                    BulkLetterSendingQueueAdditionalLetterDB.Insert(
                                        bulk_letter_sending_queue_id,
                                        filesList[f].EmailLetterLetterID,
                                        filesList[f].EmailLetterKeepHistoryInDb,
                                        filesList[f].EmailLetterKeepHistoryInFile,
                                        filesList[f].EmailLetterLetterPrintHistorySendMethodID,
                                        filesList[f].EmailLetterHistoryDir,
                                        filesList[f].EmailLetterHistoryFilename,
                                        filesList[f].EmailLetterSiteID,
                                        filesList[f].EmailLetterOrganisationID,
                                        filesList[f].EmailLetterBookingID,
                                        filesList[f].EmailLetterPatientID,
                                        filesList[f].EmailLetterRegisterReferrerIdToUseInsteadOfPatientsRegRef,
                                        filesList[f].EmailLetterStaffID,
                                        filesList[f].EmailLetterHealthCardActionID,
                                        filesList[f].EmailLetterSourceTemplatePath,
                                        filesList[f].EmailLetterOutputDocPath,
                                        false, // filesList[f].EmailLetterIsDoubleSidedPrinting,
                                        filesList[f].EmailLetterExtraPages,
                                        filesList[f].EmailLetterItemSeperator);
                                }
                            }
                        }
                        else
                        {
                            Letter.FileContents[] fileContentsList = curBooking.GetSystemLettersList(Letter.FileFormat.PDF, curBooking.Patient, curHC, curBooking.Offering.Field.ID, curPR.RegisterReferrer.Referrer, false, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, curPR.RegisterReferrer.ReportEveryVisitToReferrer, false, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), sendMethod == SendMethod.Email ? 2 : 1);
                            if (fileContentsList != null && fileContentsList.Length > 0)
                            {
                                //Logger.LogQuery("ReferrerEPCLetters_Reprint -- Email Send Item Starting!");
                                Site bkSite = SiteDB.GetSiteByType(curBooking.Organisation.IsAgedCare ? SiteDB.SiteType.AgedCare : SiteDB.SiteType.Clinic, sites);
                                Letter.EmailSystemLetter(bkSite.Name, toEmail, fileContentsList);
                                //Logger.LogQuery("ReferrerEPCLetters_Reprint -- Email Send Item Done!");
                            }
                        }

                    }
                    else
                    {
                        Letter.FileContents[] fileContentsList = curBooking.GetSystemLettersList(Letter.FileFormat.Word, curBooking.Patient, curHC, curBooking.Offering.Field.ID, curPR.RegisterReferrer.Referrer, false, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, curPR.RegisterReferrer.ReportEveryVisitToReferrer, false, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), sendMethod == SendMethod.Email ? 2 : 1);
                        if (fileContentsList != null && fileContentsList.Length > 0)
                        {
                            filesToPrint.AddRange(fileContentsList);
                        }
                    }

                    BookingDB.UpdateSetGeneratedSystemLetters(curBooking.BookingID, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, true);
                }

                ArrayList toGenerateList = new ArrayList();
                if (curBooking.NeedToGenerateFirstLetter) toGenerateList.Add("First");
                if (curBooking.NeedToGenerateLastLetter)  toGenerateList.Add("Last");
                if (curPR.RegisterReferrer.ReportEveryVisitToReferrer && curBooking.NoteCount > 0) toGenerateList.Add("Notes");
                string toGenerate = string.Join(",", (string[])toGenerateList.ToArray(typeof(string)));

                debugOutput += @"<tr>
                                    <td>" + sendMethod + @"</td>
                                    <td style=""white-space:nowrap;"">" + curBooking.BookingID + " &nbsp;&nbsp;&nbsp;[" + curBooking.DateStart.ToString("dd-MM-yyyy") + "&nbsp;&nbsp;&nbsp;" + curBooking.DateStart.ToString("HH:mm") + "-" + curBooking.DateEnd.ToString("HH:mm") + "]" + @"</td>
                                    <td>" + toGenerate + @"</td>
                                    <td>" + curPR.RegisterReferrer.Referrer.Person.FullnameWithoutMiddlename + @"</td>
                                    <td style=""white-space:nowrap;"">" + (curRefHasEmail ? refEmail : "") + @"</td>
                                    <td style=""white-space:nowrap;"">" + (curRefHasFax ? refFax : "") + @"</td>
                                    <td>" + curPR.Patient.Person.FullnameWithoutMiddlename + @"</td>
                                </tr>";
            }
        }

        return (Letter.FileContents[])filesToPrint.ToArray(typeof(Letter.FileContents));
    }

    protected static BulkLetterSendingQueueAdditionalLetter[] GetFilesInfo(Booking booking, Letter.FileFormat fileFormat, Patient patient, HealthCard hc, int fieldID, Referrer referrer, bool keepInHistory, bool needToGenerateFirstLetter, bool needToGenerateLastLetter, bool needToGenerateTreatmentLetter, int siteID, int staffID, int letterPrintHistorySendMethodID)
    {
        System.Collections.ArrayList lettersList = new System.Collections.ArrayList();

        if (needToGenerateFirstLetter)
            lettersList.Add(GetFileInfo(fileFormat, booking, patient, hc, Letter.TreatmentLetterType.First, Booking.InvoiceType.Medicare, fieldID, siteID, staffID, null, keepInHistory, letterPrintHistorySendMethodID));
        if (needToGenerateLastLetter)
            lettersList.Add(GetFileInfo(fileFormat, booking, patient, hc, Letter.TreatmentLetterType.Last, Booking.InvoiceType.Medicare, fieldID, siteID, staffID, null, keepInHistory, letterPrintHistorySendMethodID));
        if (needToGenerateTreatmentLetter)
            lettersList.Add(GetFileInfo(fileFormat, booking, patient, hc, Letter.TreatmentLetterType.TreatmentNotes, Booking.InvoiceType.Medicare, fieldID, siteID, staffID, referrer, keepInHistory, letterPrintHistorySendMethodID));

        return (BulkLetterSendingQueueAdditionalLetter[])lettersList.ToArray(typeof(BulkLetterSendingQueueAdditionalLetter));

    }
    public static BulkLetterSendingQueueAdditionalLetter GetFileInfo(Letter.FileFormat fileFormat, Booking booking, Patient patient, HealthCard hc, Letter.TreatmentLetterType treatmentLetterType, Booking.InvoiceType invType, int fieldID, int siteID, int staffID, Referrer referrer, bool keepInHistory, int letterPrintHistorySendMethodID)
    {
        // 1. Add to healthcardaction
        int healthCardActionID = -1;
        if (treatmentLetterType == Letter.TreatmentLetterType.First || treatmentLetterType == Letter.TreatmentLetterType.Last || treatmentLetterType == Letter.TreatmentLetterType.LastWhenReplacingEPC)
            healthCardActionID = HealthCardActionDB.Insert(hc.HealthCardID, Letter.GetHealthCardActionTypeID(treatmentLetterType), DateTime.Now);

        // 2.create document and put it in history
        int letterID = Letter.GetLetterIDByTreatmentLetterTypeAndInvoiceType(treatmentLetterType, invType, fieldID, siteID);
        if (letterID == -1)
            return null;


        string lettersDir = Letter.GetLettersDirectory();
        if (!Directory.Exists(lettersDir))
            throw new CustomMessageException("Letters directory doesn't exist");

        Letter letter = LetterDB.GetByID(letterID);
        bool useDefaultDocs = letter.Organisation == null ? true : !LetterDB.OrgHasdocs(letter.Organisation.OrganisationID);
        string sourceTemplatePath = lettersDir + (useDefaultDocs ? @"Default\" + letter.Site.SiteID + @"\" : letter.Organisation.OrganisationID + @"\") + letter.Docname;
        if (!File.Exists(sourceTemplatePath))
            throw new CustomMessageException("File doesn't exist: " + Path.GetFileName(sourceTemplatePath));

        // get temp directory
        string tmpLettersDirectory = Letter.GetTempLettersDirectory();
        if (!Directory.Exists(tmpLettersDirectory))
            throw new CustomMessageException("Temp letters directory doesn't exist");

        return new BulkLetterSendingQueueAdditionalLetter(
            -1,
            -1,
            letter.LetterID,
            keepInHistory && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["StoreLettersHistoryInDB"]),
            keepInHistory && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["StoreLettersHistoryInFlatFile"]),
            letterPrintHistorySendMethodID,
            Letter.GetLettersHistoryDirectory(booking.Organisation.OrganisationID),
            letter.Docname.Replace(".dot", ".doc"),
            siteID,
            booking.Organisation.OrganisationID,
            booking.BookingID,
            patient.PatientID,
            -1, // register_referrer_id_to_use_instead_of_patients_reg_ref
            staffID,
            healthCardActionID,
            sourceTemplatePath,
            tmpLettersDirectory + letter.Docname.Replace(".dot", ".doc"),
            true,
            "",
            ""
            );
    }


    protected Tuple<Booking, PatientReferrer, bool, bool, HealthCard> LoadRow(DataRow row)
    {
        Booking booking = BookingDB.Load(row, "booking_", true, false);
        booking.Offering = OfferingDB.Load(row, "offering_");

        PatientReferrer pr                        = PatientReferrerDB.Load(row, "pr_");
        pr.RegisterReferrer                       = RegisterReferrerDB.Load(row, "regref_");
        pr.RegisterReferrer.Referrer              = ReferrerDB.Load(row, "referrer_");
        pr.RegisterReferrer.Referrer.Person       = PersonDB.Load(row, "referrer_person_");
        pr.RegisterReferrer.Referrer.Person.Title = IDandDescrDB.Load(row, "referrer_person_title_title_id", "referrer_person_title_descr");
        if (row["organisation_organisation_id"] != DBNull.Value)
            pr.RegisterReferrer.Organisation      = OrganisationDB.Load(row, "organisation_");
        pr.Patient                                = PatientDB.Load(row, "patient_");
        pr.Patient.Person                         = PersonDB.Load(row, "patient_person_");
        pr.Patient.Person.Title                   = IDandDescrDB.Load(row, "patient_person_title_title_id", "patient_person_title_descr");

        bool refHasEmail = Convert.ToInt32(row["ref_has_email"]) == 1;
        bool refHasFax = Convert.ToInt32(row["ref_has_fax"]) == 1;

        HealthCard hc = HealthCardDB.Load(row, "hc_");

        return new Tuple<Booking, PatientReferrer, bool, bool, HealthCard>(booking, pr, refHasEmail, refHasFax, hc);
    }

    #endregion


    #region btnRegisterReferrerSelectionUpdate_Click, btnPatientSelectionUpdate_Click

    protected void btnRegisterReferrerSelectionUpdate_Click(object sender, EventArgs e)
    {
        int newRegisterReferrerID = Convert.ToInt32(registerReferrerID.Value);

        if (newRegisterReferrerID == -1)
        {
            lblReferrerText.Text = "<b>All Referreres</b>";
        }
        else
        {
            RegisterReferrer regRef = RegisterReferrerDB.GetByID(newRegisterReferrerID);

            string phNumTxt = string.Empty;
            if (Utilities.GetAddressType().ToString() == "Contact")
            {
                Contact[] phNums = ContactDB.GetByEntityID(2, regRef.Organisation.EntityID);
                for (int i = 0; i < phNums.Length; i++)
                    phNumTxt += (i > 0 ? "<br />" : "") + Utilities.FormatPhoneNumber(phNums[i].AddrLine1) + " &nbsp;&nbsp; (" + phNums[i].ContactType.Descr + ")";
            }
            else if (Utilities.GetAddressType().ToString() == "ContactAus")
            {
                ContactAus[] phNums = ContactAusDB.GetByEntityID(2, regRef.Organisation.EntityID);
                for (int i = 0; i < phNums.Length; i++)
                    phNumTxt += (i > 0 ? "<br />" : "") + Utilities.FormatPhoneNumber(phNums[i].AddrLine1) + " &nbsp;&nbsp; (" + phNums[i].ContactType.Descr + ")";
            }
            else
                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());


            lblReferrerText.Text = "<b>" + regRef.Referrer.Person.FullnameWithoutMiddlename + "</b> [" + regRef.Organisation.Name + "]" + "<br />" +
                  (phNumTxt.Length == 0 ? "" : phNumTxt + "<br />"); // put in referrers fax and phone numbers
        }
    }

    protected void btnPatientSelectionUpdate_Click(object sender, EventArgs e)
    {
        int newPatientID = Convert.ToInt32(patientID.Value);

        if (newPatientID == -1)
        {
            lblPatientText.Text = "<b>All Patients</b>";
        }
        else
        {
            Patient patient = PatientDB.GetByID(newPatientID);
            lblPatientText.Text = "<b>" + patient.Person.FullnameWithoutMiddlename + "</b> " + (patient.Person.Dob == DateTime.MinValue ? "" : "[" + patient.Person.Dob.ToString("dd-MM-yyyy") + "]") + "<br />";
        }
    }

    #endregion

    #region HideTableAndSetErrorMessage, SetErrorMessage, HideErrorMessag

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
        submittable.Visible = false;
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