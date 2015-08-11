using System;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using TransmitSMSAPI;
using System.Data;
using System.IO;

public class ReferrerEPCLettersSendingV2
{

    public static bool   LogDebugEmailInfo    = true;   //******
    public static bool   UseBulkLetterSender  = true;   //******
    public static bool   UseDebugEmail        = false;  //******
    
    public static string DebugEmail           = "eli@elipollak.com";
    public static int    MaxSending           = 175;

    public static string RndPageID            = (new Random()).Next().ToString();


    public enum SendMethod { Batch, Email_To_Referrer };

    public static Letter.FileContents Run(SendMethod sendMethod, int siteID, int staffID, int registerReferrerID, bool incBatching, bool incUnsent, bool incWithEmailOrFaxOnly, bool viewListOnly, bool viewFullList, out string outputInfo, out string outputList, string btnViewListClientID)
    {
        int bulkLetterSendingQueueBatchID = !viewListOnly && UseBulkLetterSender ? BulkLetterSendingQueueBatchDB.Insert(DebugEmail, false) : -1;

        RndPageID = (new Random()).Next().ToString();

        bool debugMode = true;

        bool AutoSendFaxesAsEmailsIfNoEmailExistsToGPs = Convert.ToInt32(SystemVariableDB.GetByDescr("AutoSendFaxesAsEmailsIfNoEmailExistsToGPs").Value) == 1;

        string tmpLettersDirectory = Letter.GetTempLettersDirectory();
        if (!Directory.Exists(tmpLettersDirectory))
            throw new CustomMessageException("Temp letters directory doesn't exist");


        int    startTime = 0;
        double queryExecutionTimeClinic                  = 0;
        double generateFilesToPrintExecutionTimeClinic   = 0;
        double queryExecutionTimeAgedCare                = 0;
        double generateFilesToPrintExecutionTimeAgedCare = 0;

        outputInfo = string.Empty;
        outputList = string.Empty;


        //
        //  We can not send email all their patients in one email - will be too big with attachments and rejected by their mail provider
        //  So if via email - need to send one at a time
        //  Then if cuts out or times out, it has processed some so don't need to re-process those when it's run again
        //
        //  remember to process the emails first ... so if any interruptions/errors ... at least some will have been processed
        //


        Site[] allSites    = SiteDB.GetAll();
        bool   runAllSites = siteID == -1;
        
        Site   agedCareSite = null;
        Site   clinicSite   = null;
        Site[] sitesToRun   = runAllSites ? allSites : new Site[] { SiteDB.GetByID(siteID) };
        foreach (Site s in sitesToRun)
        {
            if (s.SiteType.ID == 1)
                clinicSite   = s;
            else if (s.SiteType.ID == 2)
                agedCareSite = s;
        }



        Hashtable orgHash = OrganisationDB.GetAllHashtable(true, true, false, false, true, true);



        ArrayList filesToPrintClinic   = new ArrayList();
        ArrayList filesToPrintAgedCare = new ArrayList();
        string debugOutput = string.Empty;
        int numGenerated = 0;

        DataTable bookingsWithUnsetnLettersClinic   = null;
        DataTable bookingsWithUnsetnLettersAgedCare = null;

        if (clinicSite != null)
        {

            startTime = Environment.TickCount;

            bookingsWithUnsetnLettersClinic = BookingDB.GetBookingsWithEPCLetters(DateTime.MinValue, DateTime.MinValue, registerReferrerID, -1, false, true, incBatching, incUnsent);

            queryExecutionTimeClinic = (double)(Environment.TickCount - startTime) / 1000.0;
            startTime = Environment.TickCount;

            
            int currentRegReferrerID = -1;
            ArrayList bookingsForCurrentReferrer = new ArrayList();
            Hashtable rowIDsToRemove = new Hashtable();
            foreach (DataRow row in bookingsWithUnsetnLettersClinic.Rows)
            {
                numGenerated++;

                //if (numGenerated % 15 != 1) continue;
                if ((!viewListOnly || !viewFullList) && (numGenerated > MaxSending))
                    continue;

                Tuple<Booking, PatientReferrer, bool, bool, string, string, HealthCard> rowData = LoadClinicRow(row);

                // realod org to get full org data (in particular, org type to get site-type
                if (orgHash[rowData.Item1.Organisation.OrganisationID] != null)
                    rowData.Item1.Organisation = (Organisation)orgHash[rowData.Item1.Organisation.OrganisationID];

                Booking         booking     = rowData.Item1;
                PatientReferrer pr          = rowData.Item2;
                bool            refHasEmail = rowData.Item3;
                bool            refHasFax   = rowData.Item4;
                string          refEmail    = rowData.Item5;
                string          refFax      = rowData.Item6;
                HealthCard      hc          = rowData.Item7;

                bool refHasEmailOrFax = AutoSendFaxesAsEmailsIfNoEmailExistsToGPs ? (refHasEmail || refHasFax) : refHasEmail;
                if (incWithEmailOrFaxOnly && !refHasEmailOrFax)
                {
                    numGenerated--;
                    rowIDsToRemove[booking.BookingID] = 1;
                    continue;
                }

                //if (booking.Patient == null || (booking.Patient.PatientID != 31522 && booking.Patient.PatientID != 27654))
                //{
                //    numGenerated--;
                //    continue;
                //}



                if (pr.RegisterReferrer.RegisterReferrerID != currentRegReferrerID)
                {
                    filesToPrintClinic.AddRange(ProcessReferrersClinicLetters(sendMethod, viewListOnly, allSites, staffID, bookingsForCurrentReferrer, AutoSendFaxesAsEmailsIfNoEmailExistsToGPs, ref debugOutput, btnViewListClientID, bulkLetterSendingQueueBatchID));
                    currentRegReferrerID = pr.RegisterReferrer.RegisterReferrerID;
                    bookingsForCurrentReferrer = new ArrayList();
                }

                bookingsForCurrentReferrer.Add(rowData);
            }

            if (bookingsWithUnsetnLettersClinic.Rows.Count > 0)
                for (int i = bookingsWithUnsetnLettersClinic.Rows.Count - 1; i >= 0; i--)
                    if (rowIDsToRemove[Convert.ToInt32(bookingsWithUnsetnLettersClinic.Rows[i]["booking_booking_id"])] != null)
                        bookingsWithUnsetnLettersClinic.Rows.RemoveAt(i);

            // process last group
            filesToPrintClinic.AddRange(ProcessReferrersClinicLetters(sendMethod, viewListOnly, allSites, staffID, bookingsForCurrentReferrer, AutoSendFaxesAsEmailsIfNoEmailExistsToGPs, ref debugOutput, btnViewListClientID, bulkLetterSendingQueueBatchID));

            generateFilesToPrintExecutionTimeClinic = (double)(Environment.TickCount - startTime) / 1000.0;

        }
        if (agedCareSite != null)
        {

            startTime = Environment.TickCount;

            bookingsWithUnsetnLettersAgedCare = BookingPatientDB.GetBookingsPatientOfferingsWithEPCLetters(DateTime.MinValue, DateTime.MinValue, registerReferrerID, -1, false, true, incBatching, incUnsent);

            queryExecutionTimeAgedCare = (double)(Environment.TickCount - startTime) / 1000.0;
            startTime = Environment.TickCount;

            
            int currentRegReferrerID = -1;
            ArrayList bookingsForCurrentReferrer = new ArrayList();
            Hashtable rowIDsToRemove = new Hashtable();
            foreach (DataRow row in bookingsWithUnsetnLettersAgedCare.Rows)
            {
                numGenerated++;
                //if (numGenerated % 15 != 1) continue;
                if ((!viewListOnly || !viewFullList) && (numGenerated > MaxSending))
                    continue;
                Tuple<BookingPatient, Offering, PatientReferrer, bool, bool, string, string, Tuple<HealthCard>> rowData = LoadAgedCareRow(row);

                // realod org to get full org data (in particular, org type to get site-type
                if (orgHash[rowData.Item1.Booking.Organisation.OrganisationID] != null)
                    rowData.Item1.Booking.Organisation = (Organisation)orgHash[rowData.Item1.Booking.Organisation.OrganisationID];

                BookingPatient  bp          = rowData.Item1;
                Offering        offering    = rowData.Item2;
                PatientReferrer pr          = rowData.Item3;
                bool            refHasEmail = rowData.Item4;
                bool            refHasFax   = rowData.Item5;
                string          refEmail    = rowData.Item6;
                string          refFax      = rowData.Item7;
                HealthCard      hc          = rowData.Rest.Item1;

                bool refHasEmailOrFax = AutoSendFaxesAsEmailsIfNoEmailExistsToGPs ? (refHasEmail || refHasFax) : refHasEmail;
                if (incWithEmailOrFaxOnly && !refHasEmailOrFax)
                {
                    numGenerated--;
                    rowIDsToRemove[bp.BookingPatientID] = 1;
                    continue;
                }

                //if (bp.Booking.Patient == null || (bp.Booking.Patient.PatientID != 31522 && bp.Booking.Patient.PatientID != 27654))
                //{
                //    numGenerated--;
                //    continue;
                //}

                if (pr.RegisterReferrer.RegisterReferrerID != currentRegReferrerID)
                {
                    filesToPrintAgedCare.AddRange(ProcessReferrersAgedCareLetters(sendMethod, viewListOnly, allSites, staffID, bookingsForCurrentReferrer, AutoSendFaxesAsEmailsIfNoEmailExistsToGPs, ref debugOutput, btnViewListClientID, bulkLetterSendingQueueBatchID));
                    currentRegReferrerID = pr.RegisterReferrer.RegisterReferrerID;
                    bookingsForCurrentReferrer = new ArrayList();
                }

                bookingsForCurrentReferrer.Add(rowData);
            }

            if (bookingsWithUnsetnLettersAgedCare.Rows.Count > 0)
                for (int i = bookingsWithUnsetnLettersAgedCare.Rows.Count - 1; i >= 0; i--)
                    if (rowIDsToRemove[Convert.ToInt32(bookingsWithUnsetnLettersAgedCare.Rows[i]["bp_booking_patient_id"])] != null)
                        bookingsWithUnsetnLettersAgedCare.Rows.RemoveAt(i);

            // process last group
            filesToPrintAgedCare.AddRange(ProcessReferrersAgedCareLetters(sendMethod, viewListOnly, allSites, staffID, bookingsForCurrentReferrer, AutoSendFaxesAsEmailsIfNoEmailExistsToGPs, ref debugOutput, btnViewListClientID, bulkLetterSendingQueueBatchID));

            generateFilesToPrintExecutionTimeAgedCare = (double)(Environment.TickCount - startTime) / 1000.0;

        }

        startTime = Environment.TickCount;


        bool zipSeperately = true;
        Letter.FileContents zipFileContents = null;

        if (zipSeperately && (filesToPrintClinic.Count + filesToPrintAgedCare.Count) > 0)
        {

            // if 2 sites exist in the system - change doc names to have "[AgedCare]" or "[Clinics]" before docname
            if (allSites.Length > 1)
            {
                for (int i = 0; i < filesToPrintClinic.Count; i++)
                    ((Letter.FileContents)filesToPrintClinic[i]).DocName = "[Clinics] " + ((Letter.FileContents)filesToPrintClinic[i]).DocName;
                for (int i = 0; i < filesToPrintAgedCare.Count; i++)
                    ((Letter.FileContents)filesToPrintAgedCare[i]).DocName = "[AgedCare] " + ((Letter.FileContents)filesToPrintAgedCare[i]).DocName;
            }

            ArrayList filesToPrint = new ArrayList();
            filesToPrint.AddRange(filesToPrintClinic);
            filesToPrint.AddRange(filesToPrintAgedCare);



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
            string tmpDir = baseTmpDir + "Referral Letters" + @"\";
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
            string zipFileName = "Referral Letters.zip";
            string zipFilePath = baseTmpDir + zipFileName;
            ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
            zip.CreateEmptyDirectories = true;
            zip.CreateZip(zipFilePath, tmpDir, true, "");

            // get filecontents of zip here
            zipFileContents = new Letter.FileContents(zipFilePath, zipFileName);
            //Letter.FileContents zipFileContents = new Letter.FileContents(zipFilePath, zipFileName);
            //System.Web.HttpContext.Current.Session["downloadFile_Contents"] = zipFileContents.Contents;
            //System.Web.HttpContext.Current.Session["downloadFile_DocName"]  = zipFileContents.DocName;

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
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "download", "<script language=javascript>window.open('DownloadFile.aspx','_blank','status=1,toolbar=0,menubar=0,location=1,scrollbars=1,resizable=1,width=30,height=30');</script>");
        }

        if (!zipSeperately && (filesToPrintClinic.Count + filesToPrintAgedCare.Count) > 0)
        {
            ArrayList filesToPrint = new ArrayList();
            filesToPrint.AddRange(filesToPrintClinic);
            filesToPrint.AddRange(filesToPrintAgedCare);

            zipFileContents = Letter.FileContents.Merge((Letter.FileContents[])filesToPrint.ToArray(typeof(Letter.FileContents)), "Referral Letters.doc"); // .pdf
            //Letter.FileContents fileContents = Letter.FileContents.Merge((Letter.FileContents[])filesToPrint.ToArray(typeof(Letter.FileContents)), "Referral Letters.doc"); // .pdf
            //System.Web.HttpContext.Current.Session["downloadFile_Contents"] = fileContents.Contents;
            //System.Web.HttpContext.Current.Session["downloadFile_DocName"]  = fileContents.DocName;

            // put in session variables so when it reloads to this page, we can popup the download window 
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "download", "<script language=javascript>window.open('DownloadFile.aspx','_blank','status=1,toolbar=0,menubar=0,location=1,scrollbars=1,resizable=1,width=30,height=30');</script>");
        }


        if (!viewListOnly && registerReferrerID == -1 && incBatching)
            SetLastDateBatchSendTreatmentNotesAllReferrers(DateTime.Now);


        double restExecutionTime = (double)(Environment.TickCount - startTime) / 1000.0;

        if (debugMode)
        {
            int total = (bookingsWithUnsetnLettersClinic == null ? 0 : bookingsWithUnsetnLettersClinic.Rows.Count) + (bookingsWithUnsetnLettersAgedCare == null ? 0 : bookingsWithUnsetnLettersAgedCare.Rows.Count);
            string countGenrated = total > MaxSending ? MaxSending + " of " + total + " generated" : total.ToString() + " generated";
            string countShowing = total > MaxSending ? MaxSending + " of " + total + " showing to generate. <br />* If there are more than " + MaxSending + ", the next " + MaxSending + " will have to be generated seperately after this." : total.ToString();
            if (total > MaxSending && viewFullList)
                countShowing = total + " showing to generate. <br />* If there are more than " + MaxSending + ", only the first " + MaxSending + " will be generated and batches of " + MaxSending + " will have to be generated seperately after.";

            string queryExecutionTimeText = string.Empty;
            if (agedCareSite == null && clinicSite == null)
                queryExecutionTimeText = "0";
            if (agedCareSite == null && clinicSite != null)
                queryExecutionTimeText = queryExecutionTimeClinic.ToString();
            if (agedCareSite != null && clinicSite == null)
                queryExecutionTimeText = queryExecutionTimeAgedCare.ToString();
            if (agedCareSite != null && clinicSite != null)
                queryExecutionTimeText = "[Clinics: " + queryExecutionTimeClinic + "] [AgedCare: " + queryExecutionTimeAgedCare + "]";

            string restExecutionTimeText = string.Empty;
            if (agedCareSite == null && clinicSite == null)
                restExecutionTimeText = "0";
            if (agedCareSite == null && clinicSite != null)
                restExecutionTimeText = (generateFilesToPrintExecutionTimeClinic + restExecutionTime).ToString();
            if (agedCareSite != null && clinicSite == null)
                restExecutionTimeText = (generateFilesToPrintExecutionTimeAgedCare + restExecutionTime).ToString();
            if (agedCareSite != null && clinicSite != null)
                restExecutionTimeText = "[Clinics: " + generateFilesToPrintExecutionTimeClinic + "] [AgedCare: " + generateFilesToPrintExecutionTimeAgedCare + "] [Merging" + restExecutionTime + "]";

            if (!viewListOnly)
                outputInfo = @"<table cellpadding=""0"">
                                <tr><td><b>Send Method</b></td><td style=""width:10px;""></td><td>" + sendMethod.ToString() + @"</td><td style=""width:25px;""></td><td><b>Query Time</b></td><td style=""width:10px;""></td><td>" + queryExecutionTimeText + @" seconds</td></tr>
                                <tr><td><b>Count</b></td><td style=""width:10px;""></td><td>" + countGenrated + @"</td><td style=""width:25px;""></td><td><b>Runing Time</b></td><td style=""width:10px;""></td><td>" + restExecutionTimeText + @" seconds</td></tr>
                                </table>";

            if (viewListOnly)
                outputInfo = @"<table cellpadding=""0"">
                                <tr><td valign=""top""><b>Count</b></td><td style=""width:10px;""></td><td>" + countShowing + @"</td></tr>
                                </table>";

            if (viewListOnly)
                outputList = @"<table class=""table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center"" border=""1"">
                                    <tr>
                                        <th>Site</th>
                                        <th>Send By</th>
                                        <th>Booking</th>
                                        <th>Generate</th>
                                        <th>Referrer</th>
                                        <th>Email</th>
                                        <th>Fax</th>
                                        <th>Update Email/Fax</th>
                                        <th>Patient</th>
                                    </tr>" +
                                    (debugOutput.Length == 0 ? "<tr><td colspan=\"9\">No Rows</td></tr>" : debugOutput) +
                                "</table>";
        }

        if (UseBulkLetterSender)
            BulkLetterSendingQueueBatchDB.UpdateReadyToProcess(bulkLetterSendingQueueBatchID, true);

        return zipFileContents;

    }

    protected static int countFound = 0;
    protected static Letter.FileContents[] ProcessReferrersClinicLetters(SendMethod selectedSendMethod, bool viewListOnly, Site[] allSites, int staffID, ArrayList bookingsForCurrentReferrer, bool autoSendFaxesAsEmailsIfNoEmailExistsToGPs, ref string debugOutput, string btnViewListClientID, int bulkLetterSendingQueueBatchID)
    {
        if (bookingsForCurrentReferrer.Count == 0)
            return new Letter.FileContents[0];


        // to return - only files to print, as emailing will have been completed
        ArrayList filesToPrint = new ArrayList();


        // an email belongs to the regRef.Org ... so one referrer can have multiple.
        // keep in hash to avoid continued lookups.
        Hashtable refEmailHash = new Hashtable();

        ArrayList regRefIDsToUpdateDateTimeOfLastBatchSend = new ArrayList();

        for (int i = 0; i < bookingsForCurrentReferrer.Count; i++)
        {
            Tuple<Booking, PatientReferrer, bool, bool, string, string, HealthCard> curTuple = (Tuple<Booking, PatientReferrer, bool, bool, string, string, HealthCard>)bookingsForCurrentReferrer[i];
            Booking curBooking    = curTuple.Item1;
            PatientReferrer curPR = curTuple.Item2;
            bool curRefHasEmail   = curTuple.Item3;
            bool curRefHasFax     = curTuple.Item4;
            string curRefEmail    = curTuple.Item5;
            string curRefFax      = curTuple.Item6;
            HealthCard curHC      = curTuple.Item7;

            SiteDB.SiteType siteType = curBooking.Organisation.GetSiteType();
            Site site = SiteDB.GetSiteByType(siteType, allSites);


            bool needToGenerateLetters = curBooking.NeedToGenerateFirstLetter || curBooking.NeedToGenerateLastLetter ||
                                        (curPR.RegisterReferrer.ReportEveryVisitToReferrer && curBooking.NoteCount > 0);
            if (needToGenerateLetters)
            {
                SendMethod sendMethod = (curRefHasEmail && selectedSendMethod == SendMethod.Email_To_Referrer ? SendMethod.Email_To_Referrer : SendMethod.Batch);

                if (!viewListOnly)
                {
                    bool sendViaEmail = autoSendFaxesAsEmailsIfNoEmailExistsToGPs ? (curRefHasEmail || curRefHasFax) : curRefHasEmail;
                    if (sendViaEmail && selectedSendMethod == SendMethod.Email_To_Referrer)
                    {
                        BulkLetterSendingQueueAdditionalLetter[] filesList = GetFilesInfo(curBooking, Letter.FileFormat.PDF, curBooking.Patient, curHC, curBooking.Offering.Field.ID, curPR.RegisterReferrer.Referrer, true, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, curPR.RegisterReferrer.ReportEveryVisitToReferrer, site.SiteID, staffID, sendMethod == SendMethod.Email_To_Referrer ? 2 : 1);

                        string toEmail = autoSendFaxesAsEmailsIfNoEmailExistsToGPs ?
                                                    (curRefHasEmail ? curRefEmail : Regex.Replace(curRefFax, "[^0-9]", "") + "@fax.houseofit.com.au")
                                                    :
                                                    curRefEmail;

                        if (filesList != null && filesList.Length > 0)
                        {
                            if (ReferrerEPCLettersSending.LogDebugEmailInfo)
                                Logger.LogQuery("[" + RndPageID + "]" + "A ReferrerEPCLetters_GenerateUnsent -- Email Send Item Starting [" + toEmail + "][" + curBooking.BookingID + "][" + curBooking.Patient.PatientID + "][" + curBooking.Patient.Person.FullnameWithoutMiddlename + "]", false, false, true);
                            
                            if (UseBulkLetterSender)
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
                            else
                            {
                                Letter.FileContents[] fileContentsList = curBooking.GetSystemLettersList(Letter.FileFormat.PDF, curBooking.Patient, curHC, curBooking.Offering.Field.ID, curPR.RegisterReferrer.Referrer, true, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, curPR.RegisterReferrer.ReportEveryVisitToReferrer, false, site.SiteID, staffID, sendMethod == SendMethod.Email_To_Referrer ? 2 : 1);

                                Letter.EmailSystemLetter(site.Name, toEmail, fileContentsList,
                                                         "Referral/Treatment Note Letters From Mediclinic" + (curPR.Patient == null ? string.Empty : " For " + curPR.Patient.Person.FullnameWithoutMiddlename),
                                                         "Dr. " + curPR.RegisterReferrer.Referrer.Person.FullnameWithoutMiddlename + "<br /><br />Please find attached referral/treatment note letters for your referrered patient" + (curPR.Patient == null ? string.Empty : " <b>" + curPR.Patient.Person.FullnameWithoutMiddlename + "</b>") + "<br /><br />Best regards,<br />" + site.Name);
                            }
                            

                            if (ReferrerEPCLettersSending.LogDebugEmailInfo)
                                Logger.LogQuery( "["+RndPageID+"]" + "A ReferrerEPCLetters_GenerateUnsent -- Email Send Item Done!", false, false, true);
                        }
                    }
                    else
                    {
                        Letter.FileContents[] fileContentsList = curBooking.GetSystemLettersList(Letter.FileFormat.Word, curBooking.Patient, curHC, curBooking.Offering.Field.ID, curPR.RegisterReferrer.Referrer, true, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, curPR.RegisterReferrer.ReportEveryVisitToReferrer, false, site.SiteID, staffID, sendMethod == SendMethod.Email_To_Referrer ? 2 : 1);
                        if (fileContentsList != null && fileContentsList.Length > 0)
                        {
                            filesToPrint.AddRange(fileContentsList);
                        }
                    }

                    BookingDB.UpdateSetGeneratedSystemLetters(curBooking.BookingID, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, true);
                }

                ArrayList toGenerateList = new ArrayList();
                if (curBooking.NeedToGenerateFirstLetter) toGenerateList.Add("First");
                if (curBooking.NeedToGenerateLastLetter) toGenerateList.Add("Last");
                if (curPR.RegisterReferrer.ReportEveryVisitToReferrer && curBooking.NoteCount > 0) toGenerateList.Add("Notes");
                string toGenerate = string.Join(",", (string[])toGenerateList.ToArray(typeof(string)));

                string addEditContactListPage;
                if (Utilities.GetAddressType().ToString() == "Contact")
                    addEditContactListPage = "ContactAusListV2.aspx";
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                    addEditContactListPage = "ContactAusListV2.aspx";
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

                string allFeatures = "dialogWidth:555px;dialogHeight:350px;center:yes;resizable:no; scroll:no";
                string onclick = "onclick=\"javascript:window.showModalDialog('" + addEditContactListPage + "?entity_type=referrer&id=" + curPR.RegisterReferrer.Organisation.EntityID.ToString() + "', '', '" + allFeatures + "');document.getElementById('" + btnViewListClientID + "').click();return false;\"";
                string hrefUpdateEmail = "<u><a style=\"text-decoration: none\" title=\"Edit\" AlternateText=\"Edit\" " + onclick + " href=\"\">Update Clinic Email / Fax</a></u>";


                debugOutput += @"<tr>
                                    <td style=""white-space:nowrap;"">" + curBooking.Organisation.GetSiteType().ToString() + @"</td>
                                    <td>" + sendMethod + @"</td>
                                    <td style=""white-space:nowrap;"">" + curBooking.BookingID + " &nbsp;&nbsp;&nbsp;[" + curBooking.DateStart.ToString("dd-MM-yyyy") + "&nbsp;&nbsp;&nbsp;" + curBooking.DateStart.ToString("HH:mm") + "-" + curBooking.DateEnd.ToString("HH:mm") + "]" + @"</td>
                                    <td>" + toGenerate + @"</td>
                                    <td>" + curPR.RegisterReferrer.Referrer.Person.FullnameWithoutMiddlename + @"</td>
                                    <td>" + (curRefHasEmail    ? curRefEmail : "") + @"</td>
                                    <td style=""white-space:nowrap;"">" + (curRefFax != null ? curRefFax : "") + @"</td>
                                    <td style=""white-space:nowrap;"">" + hrefUpdateEmail + @"</td>
                                    <td>" + "<a onclick=\"open_new_tab('PatientDetailV2.aspx?type=view&id=" + curPR.Patient.PatientID + "');return false;\" href=\"PatientDetailV2.aspx?type=view&id=" + curPR.Patient.PatientID + "\">" + curPR.Patient.Person.FullnameWithoutMiddlename + @"</a></td>
                                </tr>";
            }

            if (curPR.RegisterReferrer.BatchSendAllPatientsTreatmentNotes)
                regRefIDsToUpdateDateTimeOfLastBatchSend.Add(curPR.RegisterReferrer.RegisterReferrerID);
        }

        RegisterReferrerDB.UpdateLastBatchSendAllPatientsTreatmentNotes((int[])regRefIDsToUpdateDateTimeOfLastBatchSend.ToArray(typeof(int)), DateTime.Now);

        return (Letter.FileContents[])filesToPrint.ToArray(typeof(Letter.FileContents));
    }

    protected static Letter.FileContents[] ProcessReferrersAgedCareLetters(SendMethod selectedSendMethod, bool viewListOnly, Site[] allSites, int staffID, ArrayList bookingsForCurrentReferrer, bool autoSendFaxesAsEmailsIfNoEmailExistsToGPs, ref string debugOutput, string btnViewListClientID, int bulkLetterSendingQueueBatchID)
    {
        if (bookingsForCurrentReferrer.Count == 0)
            return new Letter.FileContents[0];


        // to return - only files to print, as emailing will have been completed
        ArrayList filesToPrint = new ArrayList();


        // an email belongs to the regRef.Org ... so one referrer can have multiple.
        // keep in hash to avoid continued lookups.
        Hashtable refEmailHash = new Hashtable();

        ArrayList regRefIDsToUpdateDateTimeOfLastBatchSend = new ArrayList();

        for (int i = 0; i < bookingsForCurrentReferrer.Count; i++)
        {
            Tuple<BookingPatient, Offering, PatientReferrer, bool, bool, string, string, Tuple<HealthCard>> curTuple = (Tuple<BookingPatient, Offering, PatientReferrer, bool, bool, string, string, Tuple<HealthCard>>)bookingsForCurrentReferrer[i];
            BookingPatient          curBP          = curTuple.Item1;
            Offering                curOffering    = curTuple.Item2;
            PatientReferrer         curPR          = curTuple.Item3;
            bool                    curRefHasEmail = curTuple.Item4;
            bool                    curRefHasFax   = curTuple.Item5;
            string                  curRefEmail    = curTuple.Item6;
            string                  curRefFax      = curTuple.Item7;
            HealthCard              curHC          = curTuple.Rest.Item1;

            SiteDB.SiteType siteType = curBP.Booking.Organisation.GetSiteType();
            Site site = SiteDB.GetSiteByType(siteType, allSites);

            bool needToGenerateLetters = curBP.NeedToGenerateFirstLetter || curBP.NeedToGenerateLastLetter;
            if (needToGenerateLetters)
            {
                SendMethod sendMethod = (curRefHasEmail && selectedSendMethod == SendMethod.Email_To_Referrer ? SendMethod.Email_To_Referrer : SendMethod.Batch);

                if (!viewListOnly)
                {
                    bool sendViaEmail = autoSendFaxesAsEmailsIfNoEmailExistsToGPs ? (curRefHasEmail || curRefHasFax) : curRefHasEmail;
                    if (sendViaEmail && selectedSendMethod == SendMethod.Email_To_Referrer)
                    {
                        BulkLetterSendingQueueAdditionalLetter[] filesList = GetFilesInfo(curBP.Booking, Letter.FileFormat.PDF, curBP.Patient, curHC, curOffering.Field.ID, curPR.RegisterReferrer.Referrer, true, curBP.NeedToGenerateFirstLetter, curBP.NeedToGenerateLastLetter, false, site.SiteID, staffID, sendMethod == SendMethod.Email_To_Referrer ? 2 : 1);

                        string toEmail = autoSendFaxesAsEmailsIfNoEmailExistsToGPs ?
                                                    (curRefHasEmail ? curRefEmail : Regex.Replace(curRefFax, "[^0-9]", "") + "@fax.houseofit.com.au")
                                                    :
                                                    curRefEmail;

                        if (filesList != null && filesList.Length > 0)
                        {
                            if(ReferrerEPCLettersSending.LogDebugEmailInfo)
                                Logger.LogQuery("B ReferrerEPCLetters_GenerateUnsent -- Email Send Item Starting [" + toEmail + "][" + curBP.Booking.BookingID + "][" + curBP.Booking.Patient.PatientID + "][" + curBP.Booking.Patient.Person.FullnameWithoutMiddlename + "]", false, false, true);

                            if (UseBulkLetterSender)
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
                                    curBP.Booking.BookingID,                    // booking_id
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
                                    false, //filesList[0].EmailLetterIsDoubleSidedPrinting,
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
                                        false, //filesList[f].EmailLetterIsDoubleSidedPrinting,
                                        filesList[f].EmailLetterExtraPages,
                                        filesList[f].EmailLetterItemSeperator);
                                }
                            }
                            else
                            {
                                Letter.FileContents[] fileContentsList = curBP.Booking.GetSystemLettersList(Letter.FileFormat.PDF, curBP.Patient, curHC, curOffering.Field.ID, curPR.RegisterReferrer.Referrer, true, curBP.NeedToGenerateFirstLetter, curBP.NeedToGenerateLastLetter, false, false, site.SiteID, staffID, sendMethod == SendMethod.Email_To_Referrer ? 2 : 1);

                                Letter.EmailSystemLetter(site.Name, toEmail, fileContentsList,
                                                         "Referral/Treatment Note Letters From Mediclinic" + (curPR.Patient == null ? string.Empty : " For " + curPR.Patient.Person.FullnameWithoutMiddlename),
                                                         "Dr. " + curPR.RegisterReferrer.Referrer.Person.FullnameWithoutMiddlename + "<br /><br />Please find attached referral/treatment note letters for your referrered patient" + (curPR.Patient == null ? string.Empty : " <b>" + curPR.Patient.Person.FullnameWithoutMiddlename + "</b>") + "<br /><br />Best regards,<br />" + site.Name);
                            }

                            if (ReferrerEPCLettersSending.LogDebugEmailInfo)
                                Logger.LogQuery("B ReferrerEPCLetters_GenerateUnsent -- Email Send Item Done!", false, false, true);
                        }
                    }
                    else
                    {
                        Letter.FileContents[] fileContentsList = curBP.Booking.GetSystemLettersList(Letter.FileFormat.Word, curBP.Patient, curHC, curOffering.Field.ID, curPR.RegisterReferrer.Referrer, true, curBP.NeedToGenerateFirstLetter, curBP.NeedToGenerateLastLetter, false, false, site.SiteID, staffID, sendMethod == SendMethod.Email_To_Referrer ? 2 : 1);
                        if (fileContentsList != null && fileContentsList.Length > 0)
                        {
                            filesToPrint.AddRange(fileContentsList);
                        }
                    }

                    BookingPatientDB.UpdateSetGeneratedSystemLetters(curBP.BookingPatientID, curBP.NeedToGenerateFirstLetter, curBP.NeedToGenerateLastLetter, true);
                }

                ArrayList toGenerateList = new ArrayList();
                if (curBP.NeedToGenerateFirstLetter) toGenerateList.Add("First");
                if (curBP.NeedToGenerateLastLetter)  toGenerateList.Add("Last");
                string toGenerate = string.Join(",", (string[])toGenerateList.ToArray(typeof(string)));

                string addEditContactListPage;
                if (Utilities.GetAddressType().ToString() == "Contact")
                    addEditContactListPage = "AddEditContactList.aspx";
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                    addEditContactListPage = "AddEditContactAusList.aspx";
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

                string allFeatures = "dialogWidth:555px;dialogHeight:350px;center:yes;resizable:no; scroll:no";
                string onclick = "onclick=\"javascript:window.showModalDialog('" + addEditContactListPage + "?entity_type=referrer&id=" + curPR.RegisterReferrer.Organisation.EntityID.ToString() + "', '', '" + allFeatures + "');document.getElementById('" + btnViewListClientID + "').click();return false;\"";
                string hrefUpdateEmail = "<u><a style=\"text-decoration: none\" title=\"Edit\" AlternateText=\"Edit\" " + onclick + " href=\"\">Update Clinic Email / Fax</a></u>";

                debugOutput += @"<tr>
                                    <td style=""white-space:nowrap;"">" + curBP.Booking.Organisation.GetSiteType().ToString() + "<br />(Group BK)" + @"</td>
                                    <td>" + sendMethod  + @"</td>
                                    <td style=""white-space:nowrap;"">" + curBP.Booking.BookingID + " &nbsp;&nbsp;&nbsp;[" + curBP.Booking.DateStart.ToString("dd-MM-yyyy") + "&nbsp;&nbsp;&nbsp;" + curBP.Booking.DateStart.ToString("HH:mm") + "-" + curBP.Booking.DateEnd.ToString("HH:mm") + "]" + @"</td>
                                    <td>" + toGenerate  + @"</td>
                                    <td>" + curPR.RegisterReferrer.Referrer.Person.FullnameWithoutMiddlename + @"</td>
                                    <td>" + (curRefHasEmail    ? curRefEmail : "") + @"</td>
                                    <td style=""white-space:nowrap;"">" + (curRefFax != null ? curRefFax : "") + @"</td>
                                    <td style=""white-space:nowrap;"">" + hrefUpdateEmail + @"</td>
                                    <td>" + "<a onclick=\"open_new_tab('PatientDetailV2.aspx?type=view&id=" + curPR.Patient.PatientID + "');return false;\" href=\"PatientDetailV2.aspx?type=view&id=" + curPR.Patient.PatientID + "\">" + curPR.Patient.Person.FullnameWithoutMiddlename + @"</a></td>
                                </tr>";
            }


            if (curPR.RegisterReferrer.BatchSendAllPatientsTreatmentNotes)
                regRefIDsToUpdateDateTimeOfLastBatchSend.Add(curPR.RegisterReferrer.RegisterReferrerID);
        }

        RegisterReferrerDB.UpdateLastBatchSendAllPatientsTreatmentNotes((int[])regRefIDsToUpdateDateTimeOfLastBatchSend.ToArray(typeof(int)), DateTime.Now);

        return (Letter.FileContents[])filesToPrint.ToArray(typeof(Letter.FileContents));
    }

    protected static BulkLetterSendingQueueAdditionalLetter[] GetFilesInfo(Booking booking, Letter.FileFormat fileFormat, Patient patient, HealthCard hc, int fieldID, Referrer referrer, bool keepInHistory, bool needToGenerateFirstLetter, bool needToGenerateLastLetter, bool needToGenerateTreatmentLetter, int siteID, int staffID, int letterPrintHistorySendMethodID)
    {
        System.Collections.ArrayList lettersList = new System.Collections.ArrayList();

        if (needToGenerateFirstLetter)
            lettersList.Add(GetFileInfo(fileFormat, booking, patient, hc, Letter.TreatmentLetterType.First, Booking.InvoiceType.Medicare, fieldID, siteID, staffID, null, keepInHistory, letterPrintHistorySendMethodID));
        if (needToGenerateLastLetter)
            lettersList.Add(GetFileInfo(fileFormat, booking, patient, hc, Letter.TreatmentLetterType.Last, Booking.InvoiceType.Medicare, fieldID, siteID, staffID, null, keepInHistory, letterPrintHistorySendMethodID));
        if (!needToGenerateFirstLetter && !needToGenerateLastLetter && needToGenerateTreatmentLetter)
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


    protected static Tuple<Booking, PatientReferrer, bool, bool, string, string, HealthCard> LoadClinicRow(DataRow row)
    {
        Booking booking        = BookingDB.Load(row,  "booking_", true, false);
        booking.Offering       = OfferingDB.Load(row, "offering_");
        booking.Patient        = PatientDB.Load(row,  "patient_");
        booking.Patient.Person = PersonDB.Load(row,   "patient_person_");


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

        bool   refHasEmail = Convert.ToInt32(row["ref_has_email"]) == 1;
        string refEmail    = row["ref_email"] == DBNull.Value ? null : Convert.ToString(row["ref_email"]);  
        if (UseDebugEmail) refEmail = DebugEmail;
        bool   refHasFax   = Convert.ToInt32(row["ref_has_fax"]) == 1;
        string refFax      = row["ref_fax"] == DBNull.Value ? null : Convert.ToString(row["ref_fax"]);

        HealthCard hc = HealthCardDB.Load(row, "hc_");

        return new Tuple<Booking, PatientReferrer, bool, bool, string, string, HealthCard>(booking, pr, refHasEmail, refHasFax, refEmail, refFax, hc);
    }
    protected static Tuple<BookingPatient, Offering, PatientReferrer, bool, bool, string, string, Tuple<HealthCard>> LoadAgedCareRow(DataRow row)
    {
        BookingPatient bp         = BookingPatientDB.Load(row, "bp_");
        bp.Booking                = BookingDB.Load(row, "booking_", false, false);
        bp.Booking.Patient        = PatientDB.Load(row, "patient_");
        bp.Booking.Patient.Person = PersonDB.Load(row, "patient_person_");


        Offering offering = OfferingDB.Load(row, "offering_");

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

        bool   refHasEmail = Convert.ToInt32(row["ref_has_email"]) == 1;
        string refEmail    = row["ref_email"] == DBNull.Value ? null : Convert.ToString(row["ref_email"]);
        if (UseDebugEmail) refEmail = DebugEmail;
        bool   refHasFax   = Convert.ToInt32(row["ref_has_fax"]) == 1;
        string refFax      = row["ref_fax"]   == DBNull.Value ? null : Convert.ToString(row["ref_fax"]);

        HealthCard hc = HealthCardDB.Load(row, "hc_");

        return new Tuple<BookingPatient, Offering, PatientReferrer, bool, bool, string, string, Tuple<HealthCard>>(bp, offering, pr, refHasEmail, refHasFax, refEmail, refFax, new Tuple<HealthCard>(hc));
    }


    protected static DateTime GetLastDateBatchSendTreatmentNotesAllReferrers()
    {
        SystemVariables systemVariables = SystemVariableDB.GetAll();
        string strLastDate = systemVariables["LastDateBatchSendTreatmentNotesAllReferrers"].Value;
        if (strLastDate.Length == 0)
            return DateTime.MinValue;

        // "12:46:48 05-12-2012" 
        string[] parts = strLastDate.Split(' ');
        string[] timeParts = parts[0].Split(':');
        string[] dateParts = parts[1].Split('-');

        DateTime dateTime = new DateTime(
            Convert.ToInt32(dateParts[2]),
            Convert.ToInt32(dateParts[1]),
            Convert.ToInt32(dateParts[0]),
            Convert.ToInt32(timeParts[0]),
            Convert.ToInt32(timeParts[1]),
            Convert.ToInt32(timeParts[2])
            );

        return dateTime;
    }
    protected static void SetLastDateBatchSendTreatmentNotesAllReferrers(DateTime dateTime)
    {
        string val = dateTime == DateTime.MinValue ? "" : dateTime.ToString("HH:mm:ss dd-MM-yyyy");
        SystemVariableDB.Update("LastDateBatchSendTreatmentNotesAllReferrers", val);
    }

}