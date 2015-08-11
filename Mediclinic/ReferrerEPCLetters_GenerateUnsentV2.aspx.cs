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

public partial class ReferrerEPCLetters_GenerateUnsentV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();
            HideErrorMessage(lblErrorMessageAutoSending);


            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);
                SetNotificationInfo();
                FillForm();
                Run(true);
            }
            SetupGUI();
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

    protected void SetupGUI()
    {
        if (SiteDB.GetAll().Length <= 1)
        {
            select_sites_row.Attributes.Add("class", "hiddencol");
            select_sites_row_trailingspace.Attributes.Add("class", "hiddencol");

            select_sites_row_auto.Attributes.Add("class", "hiddencol");
            select_sites_row_trailingspace.Attributes.Add("class", "hiddencol");
        }

        for (int i = 0; i < rdioSendTypeAuto.Items.Count; i++)
        {
            rdioSendTypeAuto.Items[i].Attributes["onclick"] = "notification_info_edited();";
            rdioSendTypeAuto.Items[i].Attributes["class"] = "nowrap";
        }

        for (int i = 0; i < rdioSendType.Items.Count; i++)
        {
            rdioSendType.Items[i].Attributes["class"] = "nowrap";
        }
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
        Run(true); // update list
    }

    protected void Run(bool viewListOnly)
    {

        try
        {
            if (SelectedSendMethod == SendMethod.None)
                throw new CustomMessageException("Send method not selected");


            int staffID = Session != null && Session["StaffID"] != null ? Convert.ToInt32(Session["StaffID"]) : -1;

            int siteID = -1;

            if (!chkIncClinics.Checked && !chkIncAgedCare.Checked)
                throw new CustomMessageException("Plese check to generate for Clinics and/or Aged Care");
            else if (chkIncClinics.Checked && chkIncAgedCare.Checked)
                siteID = -1;
            else if (chkIncClinics.Checked)
            {
                foreach (Site s in SiteDB.GetAll())
                    if (s.SiteType.ID == 1) siteID = s.SiteID;
            }
            else if (chkIncAgedCare.Checked)
            {
                foreach (Site s in SiteDB.GetAll())
                    if (s.SiteType.ID == 2) siteID = s.SiteID;
            }




            /*
            // if called by automated settings there will be no session setting for SiteID or StaffID
            // but siteID is needed to know which letter template to use for generation
            if (siteID == null) 
            {
                Site[] sites = SiteDB.GetAll();
                siteID = (sites.Length == 1) ? sites[0].SiteID : sites[sites.Length - 1].SiteID; // if one site, use that -- else choose last one since clinics site developed first and exists
            }
            */


            string outputInfo;
            string outputList;

            Letter.FileContents fileContents = ReferrerEPCLettersSendingV2.Run(
                SelectedSendMethod == SendMethod.Email ? ReferrerEPCLettersSendingV2.SendMethod.Email_To_Referrer : ReferrerEPCLettersSendingV2.SendMethod.Batch,
                siteID,
                staffID,
                Convert.ToInt32(registerReferrerID.Value),
                chkIncBatching.Checked,
                chkIncUnsent.Checked,
                chkIncWithEmailOrFaxOnly.Checked,
                viewListOnly,
                chkShowFullList.Checked,
                out outputInfo,
                out outputList,
                btnViewList.ClientID
                );

            lblInfo.Text = outputInfo;
            lblList.Text = outputList;

            if (fileContents != null)
            {
                System.Web.HttpContext.Current.Session["downloadFile_Contents"] = fileContents.Contents;
                System.Web.HttpContext.Current.Session["downloadFile_DocName"] = fileContents.DocName;

                // put in session variables so when it reloads to this page, we can popup the download window 
                Page.ClientScript.RegisterStartupScript(this.GetType(), "download", "<script language=javascript>window.open('DownloadFile.aspx','_blank','status=1,toolbar=0,menubar=0,location=1,scrollbars=1,resizable=1,width=30,height=30');</script>");
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

        return;





        bool debugMode = true;


        //
        //  We can not send email all their patients in one email - will be too big with attachments and rejected by their mail provider
        //  So if via email - need to send one at a time
        //  Then if cuts out or times out, it has processed some so don't need to re-process those when it's run again
        //
        //  remember to process the emails first ... so if any interruptions/errors ... at least some will have been processed
        //



        try
        {
            string sendMethod = rdioSendType.SelectedValue;
            if (!viewListOnly && SelectedSendMethod == SendMethod.None)
                throw new CustomMessageException("Send method not selected");

            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            if (!Directory.Exists(tmpLettersDirectory))
                throw new CustomMessageException("Temp letters directory doesn't exist");


            string debugOutput = string.Empty;
            int startTime = Environment.TickCount;

            DataTable bookingsWithUnsetnLetters = BookingDB.GetBookingsWithEPCLetters(DateTime.MinValue, DateTime.MinValue, Convert.ToInt32(registerReferrerID.Value), -1, false, true, chkIncBatching.Checked, chkIncUnsent.Checked);

            double queryExecutionTime = (double)(Environment.TickCount - startTime) / 1000.0;
            startTime = Environment.TickCount;

            ArrayList filesToPrint = new ArrayList();

            int c = 0;
            int currentRegReferrerID = -1;
            ArrayList bookingsForCurrentReferrer = new ArrayList();
            foreach (DataRow row in bookingsWithUnsetnLetters.Rows)
            {
                //c++; if (c % 15 != 1) continue;
                if (c > ReferrerEPCLettersSendingV2.MaxSending)
                    continue;
                Tuple<Booking, PatientReferrer, bool, string, HealthCard> rowData = LoadRow(row);
                Booking booking = rowData.Item1;
                PatientReferrer pr = rowData.Item2;
                bool refHasEmail = rowData.Item3;
                string refEmail = rowData.Item4;
                HealthCard hc = rowData.Item5;


                if (pr.RegisterReferrer.RegisterReferrerID != currentRegReferrerID)
                {
                    filesToPrint.AddRange(ProcessReferrersLetters(viewListOnly, bookingsForCurrentReferrer, ref debugOutput));
                    currentRegReferrerID = pr.RegisterReferrer.RegisterReferrerID;
                    bookingsForCurrentReferrer = new ArrayList();
                }

                bookingsForCurrentReferrer.Add(rowData);
            }

            // process last group
            filesToPrint.AddRange(ProcessReferrersLetters(viewListOnly, bookingsForCurrentReferrer, ref debugOutput));



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
                Letter.FileContents fileContents = Letter.FileContents.Merge((Letter.FileContents[])filesToPrint.ToArray(typeof(Letter.FileContents)), "Referral Letters.doc"); // .pdf
                Session["downloadFile_Contents"] = fileContents.Contents;
                Session["downloadFile_DocName"] = fileContents.DocName;

                // put in session variables so when it reloads to this page, we can popup the download window 
                Page.ClientScript.RegisterStartupScript(this.GetType(), "download", "<script language=javascript>window.open('DownloadFile.aspx','_blank','status=1,toolbar=0,menubar=0,location=1,scrollbars=1,resizable=1,width=30,height=30');</script>");
            }


            if (!viewListOnly && Convert.ToInt32(registerReferrerID.Value) == -1 && chkIncBatching.Checked)
                SetLastDateBatchSendTreatmentNotesAllReferrers(DateTime.Now);


            double restExecutionTime = (double)(Environment.TickCount - startTime) / 1000.0;

            if (debugMode)
            {
                string countGenrated = bookingsWithUnsetnLetters.Rows.Count > ReferrerEPCLettersSendingV2.MaxSending ? ReferrerEPCLettersSendingV2.MaxSending + " of " + bookingsWithUnsetnLetters.Rows.Count + " generated" : bookingsWithUnsetnLetters.Rows.Count.ToString() + " generated";
                string countShowing = bookingsWithUnsetnLetters.Rows.Count > ReferrerEPCLettersSendingV2.MaxSending ? ReferrerEPCLettersSendingV2.MaxSending + " of " + bookingsWithUnsetnLetters.Rows.Count + " showing to generate. <br />* If there are more than " + ReferrerEPCLettersSendingV2.MaxSending + ", the next " + ReferrerEPCLettersSendingV2.MaxSending + " will have to be generated seperately after this." : bookingsWithUnsetnLetters.Rows.Count.ToString();

                if (!viewListOnly)
                    lblInfo.Text = @"<table cellpadding=""0"">
                                    <tr><td><b>Send Method</b></td><td style=""width:10px;""></td><td>" + SelectedSendMethod.ToString() + @"</td><td style=""width:25px;""></td><td><b>Query Time</b></td><td style=""width:10px;""></td><td>" + queryExecutionTime + @" seconds</td></tr>
                                    <tr><td><b>Count</b></td><td style=""width:10px;""></td><td>" + countGenrated + @"</td><td style=""width:25px;""></td><td><b>Runing Time</b></td><td style=""width:10px;""></td><td>" + restExecutionTime + @" seconds</td></tr>
                                 </table>" + "<br />";

                if (viewListOnly)
                    lblInfo.Text = @"<table cellpadding=""0"">
                                    <tr><td valign=""top""><b>Count</b></td><td style=""width:10px;""></td><td>" + countShowing + @"</td></tr>
                                 </table>" + "<br />";

                if (viewListOnly)
                    lblList.Text = @"<table class=""table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center"" border=""1"">
                                        <tr>
                                            <th>Send By</th>
                                            <th>Booking</th>
                                            <th>Generate</th>
                                            <th>Referrer</th>
                                            <th>Email</th>
                                            <th>Patient</th>
                                        </tr>" +
                                        (debugOutput.Length == 0 ? "<tr><td colspan=\"6\">No Rows</td></tr>" : debugOutput) +
                                    "</table>";
            }
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch (Exception ex)
        {
            SetErrorMessage(ex.ToString());
        }

    }

    protected Letter.FileContents[] ProcessReferrersLetters(bool viewListOnly, ArrayList bookingsForCurrentReferrer, ref string debugOutput)
    {
        if (bookingsForCurrentReferrer.Count == 0)
            return new Letter.FileContents[0];

        // to return - only files to print, as emailing will have been completed
        ArrayList filesToPrint = new ArrayList();


        // an email belongs to the regRef.Org ... so one referrer can have multiple.
        // keep in hash to avoid continued lookups.
        Hashtable refEmailHash = new Hashtable();

        ArrayList regRefIDsToUpdateDateTimeOfLastBatchSend = new ArrayList();

        Site[] sites = SiteDB.GetAll();
        for (int i = 0; i < bookingsForCurrentReferrer.Count; i++)
        {
            Tuple<Booking, PatientReferrer, bool, string, HealthCard> curTuple = (Tuple<Booking, PatientReferrer, bool, string, HealthCard>)bookingsForCurrentReferrer[i];
            Booking curBooking = curTuple.Item1;
            PatientReferrer curPR = curTuple.Item2;
            bool curRefHasEmail = curTuple.Item3;
            string curRefEmail = curTuple.Item4;
            HealthCard curHC = curTuple.Item5;

            bool needToGenerateLetters = curBooking.NeedToGenerateFirstLetter || curBooking.NeedToGenerateLastLetter ||
                                        (curPR.RegisterReferrer.ReportEveryVisitToReferrer && curBooking.NoteCount > 0);
            if (needToGenerateLetters)
            {
                SendMethod sendMethod = (curRefHasEmail && this.SelectedSendMethod == SendMethod.Email ? SendMethod.Email : SendMethod.Print);

                if (!viewListOnly)
                {
                    if (curRefHasEmail && this.SelectedSendMethod == SendMethod.Email)
                    {
                        Letter.FileContents[] fileContentsList = curBooking.GetSystemLettersList(Letter.FileFormat.PDF, curBooking.Patient, curHC, curBooking.Offering.Field.ID, curPR.RegisterReferrer.Referrer, true, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, curPR.RegisterReferrer.ReportEveryVisitToReferrer, false, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), sendMethod == SendMethod.Email ? 2 : 1);
                        if (fileContentsList != null && fileContentsList.Length > 0)
                        {
                            if (ReferrerEPCLettersSendingV2.LogDebugEmailInfo)
                                Logger.LogQuery("C ReferrerEPCLetters_GenerateUnsent -- Email Send Item Starting [" + curRefEmail + "]", false, false, true);
                            
                            Site site = SiteDB.GetSiteByType(curBooking.Organisation.IsAgedCare ? SiteDB.SiteType.AgedCare : SiteDB.SiteType.Clinic, sites);
                            Letter.EmailSystemLetter(site.Name, curRefEmail, fileContentsList,
                                                     "Referral/Treatment Note Letters From Mediclinic" + (curPR.Patient == null ? string.Empty : " For " + curPR.Patient.Person.FullnameWithoutMiddlename),
                                                     "Dr. " + curPR.RegisterReferrer.Referrer.Person.FullnameWithoutMiddlename + "<br /><br />Please find attached referral/treatment note letters for your referrered patient" + (curPR.Patient == null ? string.Empty : " <b>" + curPR.Patient.Person.FullnameWithoutMiddlename + "</b>") + "<br /><br />Best regards,<br />" + site.Name);

                            if (ReferrerEPCLettersSendingV2.LogDebugEmailInfo)
                                Logger.LogQuery("C ReferrerEPCLetters_GenerateUnsent -- Email Send Item Done!", false, false, true);
                        }
                    }
                    else
                    {
                        Letter.FileContents[] fileContentsList = curBooking.GetSystemLettersList(Letter.FileFormat.Word, curBooking.Patient, curHC, curBooking.Offering.Field.ID, curPR.RegisterReferrer.Referrer, true, curBooking.NeedToGenerateFirstLetter, curBooking.NeedToGenerateLastLetter, curPR.RegisterReferrer.ReportEveryVisitToReferrer, false, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), sendMethod == SendMethod.Email ? 2 : 1);
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
                    addEditContactListPage = "AddEditContactList.aspx";
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                    addEditContactListPage = "ContactAusListV2.aspx";
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

                string allFeatures = "dialogWidth:525px;dialogHeight:430px;center:yes;resizable:no; scroll:no";
                string onclick = "onclick=\"javascript:window.showModalDialog('" + addEditContactListPage + "?entity_type=referrer&id=" + curPR.RegisterReferrer.Organisation.EntityID.ToString() + "', '', '" + allFeatures + "');document.getElementById('" + btnViewList.ClientID + "').click();return false;\"";
                string hrefUpdateEmail = "<u><a style=\"text-decoration: none\" title=\"Edit\" AlternateText=\"Edit\" " + onclick + " href=\"\">Update Clinic Email</a></u>";

                debugOutput += @"<tr>
                                    <td>" + sendMethod + @"</td>
                                    <td style=""white-space:nowrap;"">" + curBooking.BookingID + " &nbsp;&nbsp;&nbsp;[" + curBooking.DateStart.ToString("dd-MM-yyyy") + "&nbsp;&nbsp;&nbsp;" + curBooking.DateStart.ToString("HH:mm") + "-" + curBooking.DateEnd.ToString("HH:mm") + "]" + @"</td>
                                    <td>" + toGenerate + @"</td>
                                    <td>" + curPR.RegisterReferrer.Referrer.Person.FullnameWithoutMiddlename + @"</td>
                                    <td style=""white-space:nowrap;"">" + (curRefHasEmail ? curRefEmail : "Has No Email") + " (" + hrefUpdateEmail + ")" + @"</td>
                                    <td>" + curPR.Patient.Person.FullnameWithoutMiddlename + @"</td>
                                </tr>";
            }

            if (curPR.RegisterReferrer.BatchSendAllPatientsTreatmentNotes)
                regRefIDsToUpdateDateTimeOfLastBatchSend.Add(curPR.RegisterReferrer.RegisterReferrerID);
        }

        RegisterReferrerDB.UpdateLastBatchSendAllPatientsTreatmentNotes((int[])regRefIDsToUpdateDateTimeOfLastBatchSend.ToArray(typeof(int)), DateTime.Now);

        return (Letter.FileContents[])filesToPrint.ToArray(typeof(Letter.FileContents));
    }

    protected Tuple<Booking, PatientReferrer, bool, string, HealthCard> LoadRow(DataRow row)
    {
        Booking booking = BookingDB.Load(row, "booking_", true, false);
        booking.Offering = OfferingDB.Load(row, "offering_");

        PatientReferrer pr = PatientReferrerDB.Load(row, "pr_");
        pr.RegisterReferrer = RegisterReferrerDB.Load(row, "regref_");
        pr.RegisterReferrer.Referrer = ReferrerDB.Load(row, "referrer_");
        pr.RegisterReferrer.Referrer.Person = PersonDB.Load(row, "referrer_person_");
        pr.RegisterReferrer.Referrer.Person.Title = IDandDescrDB.Load(row, "referrer_person_title_title_id", "referrer_person_title_descr");
        if (row["organisation_organisation_id"] != DBNull.Value)
            pr.RegisterReferrer.Organisation = OrganisationDB.Load(row, "organisation_");
        pr.Patient = PatientDB.Load(row, "patient_");
        pr.Patient.Person = PersonDB.Load(row, "patient_person_");
        pr.Patient.Person.Title = IDandDescrDB.Load(row, "patient_person_title_title_id", "patient_person_title_descr");

        bool refHasEmail = Convert.ToInt32(row["ref_has_email"]) == 1;
        string refEmail = row["ref_email"] == DBNull.Value ? null : Convert.ToString(row["ref_email"]);

        HealthCard hc = HealthCardDB.Load(row, "hc_");

        return new Tuple<Booking, PatientReferrer, bool, string, HealthCard>(booking, pr, refHasEmail, refEmail, hc);
    }

    #endregion

    #region GetLastDateBatchSendTreatmentNotesAllReferrers(), SetLastDateBatchSendTreatmentNotesAllReferrers(DateTime dateTime)

    protected DateTime GetLastDateBatchSendTreatmentNotesAllReferrers()
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
    protected void SetLastDateBatchSendTreatmentNotesAllReferrers(DateTime dateTime)
    {
        string val = dateTime == DateTime.MinValue ? "" : dateTime.ToString("HH:mm:ss dd-MM-yyyy");
        SystemVariableDB.Update("LastDateBatchSendTreatmentNotesAllReferrers", val);
    }

    #endregion

    #region btnRegisterReferrerSelectionUpdate_Click

    protected void btnRegisterReferrerSelectionUpdate_Click(object sender, EventArgs e)
    {
        // can update info ... if needed...
        int newRegisterReferrerID = Convert.ToInt32(registerReferrerID.Value);

        if (newRegisterReferrerID == -1)
        {
            lblReferrerText.Text = "<b>All Referreres</b>";
        }
        else
        {
            RegisterReferrer regRef = RegisterReferrerDB.GetByID(newRegisterReferrerID);
            //lblReferrerText.Text = regRef.Referrer.Person.FullnameWithoutMiddlename;

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

    #endregion


    #region btnUpdateNotificationInfo_Click

    protected void btnUpdateNotificationInfo_Click(object sender, EventArgs e)
    {
        txtEmailAddress.Text = txtEmailAddress.Text.Trim();

        try
        {
            if (chkEnableEmails.Checked && txtEmailAddress.Text.Length == 0)
                throw new CustomMessageException("To enable this, please set an email address");

            txtEmailAddress.Text = Utilities.CleanEmailAddresses(txtEmailAddress.Text);
            if (txtEmailAddress.Text.Length > 0 && !Utilities.IsValidEmailAddresses(txtEmailAddress.Text, false))
                throw new CustomMessageException("Invalid email address");

            if (select_sites_row_auto.Attributes["class"] != "hiddencol" && !chkIncAgedCareAuto.Checked && !chkIncClinicsAuto.Checked)
                throw new CustomMessageException("Plese check to generate for Clinics and/or Aged Care");

            if (rdioSendTypeAuto.SelectedValue != "Print" && rdioSendTypeAuto.SelectedValue != "Email")
                throw new ArgumentException("Please select a send method of Email or Print");

            if (chkEnableEmails.Checked &&
                   (!chkSendMondays.Checked &&
                    !chkSendTuesdays.Checked &&
                    !chkSendWednesdays.Checked &&
                    !chkSendThursdays.Checked &&
                    !chkSendFridays.Checked &&
                    !chkSendSaturdays.Checked &&
                    !chkSendSundays.Checked))
                throw new CustomMessageException("To enable this, please set at least one day to send");
        }
        catch (CustomMessageException ex)
        {
            SetErrorMessage(ex.Message, "", lblErrorMessageAutoSending);
            return;
        }


        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_SendEmail", chkEnableEmails.Checked ? "1" : "0");
        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_EmailAddress", txtEmailAddress.Text);

        if (select_sites_row_auto.Attributes["class"] != "hiddencol")
        {
            SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_IncClinic", chkIncClinicsAuto.Checked ? "1" : "0");
            SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_IncAgedCare", chkIncAgedCareAuto.Checked ? "1" : "0");
        }

        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_IncUnsent", chkIncUnsentAuto.Checked ? "1" : "0");
        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_IncBatched", chkIncBatchedAuto.Checked ? "1" : "0");

        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_SendMethod", rdioSendTypeAuto.SelectedValue);

        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_SendMondays", chkSendMondays.Checked ? "1" : "0");
        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_SendTuesdays", chkSendTuesdays.Checked ? "1" : "0");
        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_SendWednesdays", chkSendWednesdays.Checked ? "1" : "0");
        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_SendThursdays", chkSendThursdays.Checked ? "1" : "0");
        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_SendFridays", chkSendFridays.Checked ? "1" : "0");
        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_SendSaturdays", chkSendSaturdays.Checked ? "1" : "0");
        SystemVariableDB.Update("ReferrerEPCAutoGenerateLettersEmail_SendSundays", chkSendSundays.Checked ? "1" : "0");

        SetNotificationInfo(); // re-set to show it was update in the db
    }

    protected void btnRevertNotificationInfo_Click(object sender, EventArgs e)
    {
        SetNotificationInfo();
    }

    protected void SetNotificationInfo()
    {
        chkEnableEmails.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendEmail").Value) == 1;
        txtEmailAddress.Text = SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_EmailAddress").Value;

        chkIncClinicsAuto.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_IncClinic").Value) == 1;
        chkIncAgedCareAuto.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_IncAgedCare").Value) == 1;

        chkIncUnsentAuto.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_IncUnsent").Value) == 1;
        chkIncBatchedAuto.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_IncBatched").Value) == 1;

        rdioSendTypeAuto.SelectedValue = SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendMethod").Value;

        chkSendMondays.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendMondays").Value) == 1;
        chkSendTuesdays.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendTuesdays").Value) == 1;
        chkSendWednesdays.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendWednesdays").Value) == 1;
        chkSendThursdays.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendThursdays").Value) == 1;
        chkSendFridays.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendFridays").Value) == 1;
        chkSendSaturdays.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendSaturdays").Value) == 1;
        chkSendSundays.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendSundays").Value) == 1;


        btnUpdateNotificationInfo.CssClass = "hiddencol";
        btnRevertNotificationInfo.CssClass = "hiddencol";
    }

    #endregion


    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        SetErrorMessage(errMsg, details);
    }
    private void SetErrorMessage(string errMsg = "", string details = "", Label label = null)
    {
        if (errMsg.Contains(Environment.NewLine))
            errMsg = errMsg.Replace(Environment.NewLine, "<br />");

        // double escape so shows up literally on webpage for 'alert' message
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"#\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

        (label == null ? lblErrorMessage : label).Visible = true;
        if (errMsg != null && errMsg.Length > 0)
            (label == null ? lblErrorMessage : label).Text = errMsg + detailsToDisplay + "<br />";
        else
            (label == null ? lblErrorMessage : label).Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br />";
    }
    private void HideErrorMessage(Label label = null)
    {
        (label == null ? lblErrorMessage : label).Visible = false;
        (label == null ? lblErrorMessage : label).Text = "";
    }

    #endregion

}