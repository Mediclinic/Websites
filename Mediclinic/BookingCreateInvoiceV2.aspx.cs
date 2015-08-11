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
using System.Xml;
using TransmitSMSAPI;

public partial class BookingCreateInvoiceV2 : System.Web.UI.Page
{

    //    =>  if medicare inv - cant add any other items
    //
    //    => if dva - can add more
    //          and can only add non-dva and non-medicare
    //          => only show items that have dva code



    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);
        Utilities.UpdatePageHeaderV2(Page.Master, true);
        HideErrorMessage();

        if (!IsPostBack)
        {
            Session.Remove("data_selected");
            Session.Remove("sortExpression_Selected");
            Session.Remove("sortExpression_Offering");

            Session.Remove("downloadFile_Contents");
            Session.Remove("downloadFile_DocName");
        }


        try
        {
            Booking booking = GetFormBooking();
            if (booking == null)
                throw new CustomMessageException("Invalid booking");
            if (booking.BookingStatus.ID != 0)
                throw new CustomMessageException("Booking already set as : " + BookingDB.GetStatusByID(booking.BookingStatus.ID).Descr);

            Booking.InvoiceType invType = (GetFormIsCancelation() || GetFormIsPrivateInv()) ? Booking.InvoiceType.None : GetInvoiceType();
            if (invType == Booking.InvoiceType.Medicare)
                lblHeading.Text = "Medicare Invoice";
            else if (invType == Booking.InvoiceType.DVA)
                lblHeading.Text = "DVA Invoice";
            else if (invType == Booking.InvoiceType.Insurance)
                lblHeading.Text = "Insurance Invoice";
            else if (invType == Booking.InvoiceType.NoneFromCombinedYearlyThreshholdReached)
                lblHeading.Text = "PT Payable Invoice" + " - <font color=\"red\">Year Limit reached</font>";
            else if (invType == Booking.InvoiceType.NoneFromOfferingYearlyThreshholdReached)
                lblHeading.Text = "PT Payable Invoice" + " - <font color=\"red\">Year Limit reached for this service</font>";
            else if (invType == Booking.InvoiceType.NoneFromExpired)
                lblHeading.Text = "PT Payable Invoice" + " - <font color=\"red\">" + (booking.Provider.Field.ID == 1 ? "Card Expired" : "Referral Expired")  + "</font>";
            else if (invType == Booking.InvoiceType.None)
                lblHeading.Text = "PT Payable Invoice";
            else
                throw new CustomMessageException("Unknown Invoice Type");


            DataTable dt_selected_list = Session["data_selected"] as DataTable;
            if (dt_selected_list != null)
            {
                int r = dt_selected_list.Rows.Count;
            }

            invoiceItemsControl.InvoiceType = invType;
            invoiceItemsControl.SubmitButtonText = "Complete";
            invoiceItemsControl.Booking = booking;
            invoiceItemsControl.FillOfferingGrid();
            invoiceItemsControl.FillSelectedGrid();

            if (invType != Booking.InvoiceType.Medicare && invType != Booking.InvoiceType.DVA && invType != Booking.InvoiceType.Insurance)
                invoiceItemsControl.LabelSetPrivateInvoiceVisible = false;


            if (invType != Booking.InvoiceType.Medicare && invType != Booking.InvoiceType.DVA)
            {
                chkGenerateSystemLetters.Visible = false;
            }
            else
            {
                PatientReferrer[] patientReferrers = PatientReferrerDB.GetActiveEPCPatientReferrersOf(booking.Patient.PatientID);

                if (patientReferrers.Length > 0)
                {
                    string[] emails = ContactDB.GetEmailsByEntityID(patientReferrers[patientReferrers.Length - 1].RegisterReferrer.Organisation.EntityID);
                    chkGenerateSystemLetters.Visible = !patientReferrers[patientReferrers.Length - 1].RegisterReferrer.BatchSendAllPatientsTreatmentNotes && emails.Length == 0;
                }
                else // old crap from BEST can have an EPC with no fucking referring doctor
                {
                    chkGenerateSystemLetters.Visible = false;
                }
            }


            SetupGUI(booking, invType);
        }
        catch (CustomMessageException cmEx)
        {
            HideTableAndSetErrorMessage(cmEx.Message);
            return;
        }
        catch (Exception ex)
        {
            HideTableAndSetErrorMessage("", ex.ToString());
            return;
        }


        invoiceItemsControl.UserControlSubmitClicked             += new EventHandler(InvoiceItemsControl_SubmitButtonClicked);
        invoiceItemsControl.UserControlMakePrivateInvoiceClicked += new EventHandler(InvoiceItemsControl_MakePrivateInvoiceLinkClicked);

    }

    #endregion

    #region GetUrlParamType()

    private bool IsValidFormBooking()
    {
        string id = Request.QueryString["booking"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private Booking GetFormBooking()
    {
        if (!IsValidFormBooking())
            throw new Exception("Invalid url booking");
        return BookingDB.GetByID(Convert.ToInt32(Request.QueryString["booking"]));
    }

    private bool GetFormIsPrivateInv()
    {
        string id = Request.QueryString["is_private"];
        return id != null && id == "1";
    }
    private bool GetFormIsCancelation()
    {
        string type = Request.QueryString["type"];
        return type != null && type == "cancel";
    }

    #endregion

    #region SetupGUI()

    protected void SetupGUI(Booking booking, Booking.InvoiceType invType)
    {
        string screen_id = "15";
        string allFeatures = "dialogWidth:980px;dialogHeight:530px;center:yes;resizable:no; scroll:no";
        string js = "javascript:window.showModalDialog('" + "NoteListV2.aspx?id=" + booking.EntityID.ToString() + "&screen=" + screen_id + "', '', '" + allFeatures + "');document.getElementById('btnUpdateNotesIcon').click();return false;";
        this.lnkNotes.Attributes.Add("onclick", js);
        lnkNotes.ImageUrl = booking.NoteCount > 0 ? "~/images/notes-32.png" : "~/images/notes-bw-32.jpg";

        lblNotesMessage.Visible = false;
        notesMessageSpace.Visible = false;
        PatientReferrer[] patientReferrers = PatientReferrerDB.GetActiveEPCPatientReferrersOf(booking.Patient.PatientID);
        if (patientReferrers.Length > 0 && patientReferrers[patientReferrers.Length-1].RegisterReferrer.ReportEveryVisitToReferrer)  // [if only for epc-invoiced bookings, add:]   && (invType == Booking.InvoiceType.Medicare || invType == Booking.InvoiceType.DVA)
        {
            notesMessageSpace.Visible = true;
            lblNotesMessage.Visible = true;
            lblNotesMessage.Text = "** DETAILED NOTES MANDATORY FOR GP **";
            lblNotesMessage.Font.Bold = true;
            lblNotesMessage.ForeColor = System.Drawing.Color.Red;
            lblNotesMessage.Font.Size = FontUnit.Large;
        }
    }

    protected void btnUpdateNotesIcon_Click(object sender, EventArgs e)
    {
        Booking booking = GetFormBooking();
        lnkNotes.ImageUrl = NoteDB.HasNotes(booking.EntityID) ? "~/images/notes-48.png" : "~/images/notes-bw-48.jpg";
    }

    #endregion

    #region InvoiceItemsControl_SubmitButtonClicked

    private void InvoiceItemsControl_SubmitButtonClicked(object sender, EventArgs e)
    {
        bool showDownloadPopup = false;
        bool refresh_on_close = (Request.QueryString["refresh_on_close"] != null && Request.QueryString["refresh_on_close"] == "1");

        try
        {
            ///////////////
            // validation
            ///////////////

            Booking booking = GetFormBooking();
            if (booking == null)
                throw new CustomMessageException("Invalid booking");
            if (booking.BookingStatus.ID != 0)
                throw new CustomMessageException("Booking already set as : " + BookingDB.GetStatusByID(booking.BookingStatus.ID).Descr);
            if (InvoiceDB.GetCountByBookingID(booking.BookingID) > 0) // shouldnt get here since should have been set as completed and thrown in error above
                throw new CustomMessageException("Booking already has an invoice");


            ///////////////////
            // create invoice
            ///////////////////


            // keep id's to delete if exception and need to roll back
            int     hcInvID       = -2;
            int     nonHcInvID    = -2;
            decimal nonHcInvTotal =  0;
            ArrayList  invLineIDs = new ArrayList();
            ArrayList  offeringOrderIDs = new ArrayList();
            HealthCard hc = HealthCardDB.GetActiveByPatientID(booking.Patient.PatientID);
            HealthCardEPCRemaining[] epcsRemaining = hc == null ? new HealthCardEPCRemaining[] { } : HealthCardEPCRemainingDB.GetByHealthCardID(hc.HealthCardID, booking.Offering.Field.ID);
            HealthCardEPCRemaining[] epcsRemainingOriginal = HealthCardEPCRemaining.CloneList(epcsRemaining);

            // used to check update stock and check warning level emails sent
            ArrayList invoiceLines = new ArrayList();


            try
            {
                DataTable dt_selected_list = invoiceItemsControl.GetSelectedList();

                bool isCancelation = GetFormIsCancelation();
                Booking.InvoiceType invType = isCancelation || GetFormIsPrivateInv() ? Booking.InvoiceType.None : GetInvoiceType();
                int orgID = Booking.GetInvoiceTypeOrgID(invType);  // "org id"  -- clinic inv:  medicare, dva, or null for non medicare/dva (ie patient) booking ... 




                // [107='Clinic Invoice', 363='Aged Care Invoice', 108='Standard Invoice']
                int docType = booking.Organisation.OrganisationType.OrganisationTypeID == 218 ? 107 : 363;

                bool invoiceGapPayments = Convert.ToInt32(SystemVariableDB.GetByDescr("InvoiceGapPayments").Value) == 1;

                decimal ptTotal     = 0;
                decimal hcTotal     = 0;
                decimal ptGST       = 0;
                decimal hcGST       = 0;
                int     ptItemCount = 0;
                int     hcItemCount = 0;
                for (int i = 0; i < dt_selected_list.Rows.Count; i++)
                {
                    decimal total_hc_price = Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_price"]);
                    decimal total_pt_price = Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]);
                    decimal total_hc_gst   = Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_gst"]);
                    decimal total_pt_gst   = Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]);
                    bool    totalZero = total_hc_price + total_pt_price == 0;
                    bool    hcPaid    = Convert.ToBoolean(dt_selected_list.Rows[i]["hc_paid"]);

                    if (total_hc_price > 0 || (totalZero && hcPaid))
                    {
                        hcTotal += total_hc_price;
                        hcGST   += total_hc_gst;
                        hcItemCount++;
                    }
                    if (total_pt_price > 0 || (totalZero && !hcPaid))
                    {
                        ptTotal += total_pt_price;
                        ptGST   += total_pt_gst;
                        ptItemCount++;
                    }
                }


                // add healthcare invoice 
                if (hcItemCount > 0)
                {
                    hcInvID = InvoiceDB.Insert(docType, booking.BookingID, (invType != Booking.InvoiceType.Insurance ? orgID : hc.Organisation.OrganisationID), -1, 0, (invType != Booking.InvoiceType.Insurance ? "" : hc.CardNbr), "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), hcTotal + hcGST, hcGST, false, false, false, DateTime.MinValue);
                    
                    //if (Convert.ToInt32(SystemVariableDB.GetByDescr("AutoMedicareClaiming").Value) == 1)
                    //    MedicareClaimNbrDB.InsertIntoInvoice(hcInvID, DateTime.Now.Date);

                    for (int i = 0; i < dt_selected_list.Rows.Count; i++)
                    {
                        decimal total_hc_price   = Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_price"]);
                        decimal total_pt_price   = Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]);
                        bool    totalZero        = total_hc_price + total_pt_price == 0;
                        bool    hcPaid           = Convert.ToBoolean(dt_selected_list.Rows[i]["hc_paid"]);
                        string  areaTreated      = dt_selected_list.Rows[i]["area_treated"].ToString().Trim();
                        string  serviceReference = dt_selected_list.Rows[i]["service_reference"].ToString().Trim();

                        if (total_hc_price > 0 || (totalZero && hcPaid))
                        {

                            int offeringOrderID = -1;
                            if (Convert.ToBoolean(dt_selected_list.Rows[i]["on_order"]))
                            {
                                offeringOrderID = OfferingOrderDB.Insert(
                                    Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]),
                                    booking.Organisation.OrganisationID,
                                    booking.Provider.StaffID,
                                    booking.Patient.PatientID,
                                    Convert.ToInt32(dt_selected_list.Rows[i]["quantity"]),
                                    DateTime.Today,
                                    DateTime.MinValue,
                                    DateTime.MinValue,
                                    string.Empty
                                    );
                                offeringOrderIDs.Add(offeringOrderID);
                            }


                            int invoiceLineID = InvoiceLineDB.Insert(hcInvID, booking.Patient.PatientID, Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]), Convert.ToInt32(dt_selected_list.Rows[i]["quantity"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_price"]) + Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_gst"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_gst"]), areaTreated, serviceReference, offeringOrderID);
                            invLineIDs.Add(invoiceLineID);
                            invoiceLines.Add(new InvoiceLine(invoiceLineID, hcInvID, booking.Patient.PatientID, Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]), Convert.ToInt32(dt_selected_list.Rows[i]["quantity"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_price"]) + Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_gst"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_gst"]), areaTreated, serviceReference, offeringOrderID));

                            // update their epcs remaining
                            if (invType == Booking.InvoiceType.Medicare)
                            {
                                Offering offering = OfferingDB.GetByID(Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]));
                                for (int j = 0; j < epcsRemaining.Length; j++)
                                {
                                    if (epcsRemaining[j].Field.ID == offering.Field.ID)
                                    {
                                        epcsRemaining[j].NumServicesRemaining -= 1;
                                        HealthCardEPCRemainingDB.UpdateNumServicesRemaining(epcsRemaining[j].HealthCardEpcRemainingID, epcsRemaining[j].NumServicesRemaining);
                                    }
                                }
                            }
                        }
                    }
                }


                // add non-healthcare invoice
                if (ptItemCount > 0)
                {
                    nonHcInvID = InvoiceDB.Insert(docType, booking.BookingID, 0, booking.Patient.PatientID, 0, "", "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), ptTotal + ptGST, ptGST, false, false, false, DateTime.MinValue);
                    for (int i = 0; i < dt_selected_list.Rows.Count; i++)
                    {
                        decimal total_hc_price   = Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_price"]);
                        decimal total_pt_price   = Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]);
                        bool    totalZero        = total_hc_price + total_pt_price == 0;
                        bool    hcPaid           = Convert.ToBoolean(dt_selected_list.Rows[i]["hc_paid"]);
                        string  areaTreated      = dt_selected_list.Rows[i]["area_treated"].ToString().Trim();
                        string  serviceReference = dt_selected_list.Rows[i]["service_reference"].ToString().Trim();

                        if (total_pt_price > 0 || (totalZero && !hcPaid))
                        {

                            int offeringOrderID = -1;
                            if (Convert.ToBoolean(dt_selected_list.Rows[i]["on_order"]))
                            {
                                offeringOrderID = OfferingOrderDB.Insert(
                                    Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]),
                                    booking.Organisation.OrganisationID,
                                    booking.Provider.StaffID,
                                    booking.Patient.PatientID,
                                    Convert.ToInt32(dt_selected_list.Rows[i]["quantity"]),
                                    DateTime.Today,
                                    DateTime.MinValue,
                                    DateTime.MinValue,
                                    string.Empty
                                    );
                                offeringOrderIDs.Add(offeringOrderID);
                            }

                            int invoiceLineID = InvoiceLineDB.Insert(nonHcInvID, booking.Patient.PatientID, Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]), Convert.ToInt32(dt_selected_list.Rows[i]["quantity"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]) + Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]), areaTreated, serviceReference, offeringOrderID);
                            invLineIDs.Add(invoiceLineID);
                            invoiceLines.Add(new InvoiceLine(invoiceLineID, nonHcInvID, booking.Patient.PatientID, Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]), Convert.ToInt32(dt_selected_list.Rows[i]["quantity"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]) + Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]), areaTreated, serviceReference, offeringOrderID));

                            nonHcInvTotal += Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]);
                        }
                   }

                    if (nonHcInvTotal == 0) // for free services
                        InvoiceDB.UpdateIsPaid(null, nonHcInvID, true);
               }



                // set booking as completed (or cancelled)
                booking.BookingStatus.ID = GetFormIsCancelation() ? 188 : 187;
                BookingDB.UpdateSetBookingStatusID(booking.BookingID, booking.BookingStatus.ID);

                if (GetFormIsCancelation())
                    BookingDB.UpdateSetCancelledByPatient(booking.BookingID, Convert.ToInt32(Session["StaffID"]));




                if (!isCancelation)
                {
                    // send referrer letters
                    //
                    // NB: FIRST/LAST letters ONLY FOR MEDICARE - DVA doesn't need letters
                    // Treatment letters for anyone with epc though -- even for private invoices
                    PatientReferrer[] patientReferrers = PatientReferrerDB.GetActiveEPCPatientReferrersOf(booking.Patient.PatientID);
                    if (patientReferrers.Length == 0 && (invType == Booking.InvoiceType.Medicare || invType == Booking.InvoiceType.DVA))
                    {
                        // Marcus: let it create the invoice for medicare/dva and they will pick it up in the HINX sending rejection
                        ; //throw new CustomMessageException("Medicare/DVA invoice requires a referrering doctor - none found for this patient.");
                    }
                    else if (patientReferrers.Length > 0 && (invType == Booking.InvoiceType.Medicare || invType == Booking.InvoiceType.DVA))
                    {
                        bool needToGenerateFirstLetter = false;
                        bool needToGenerateLastLetter  = false;
                        bool needToGenerateTreatmentLetter = patientReferrers[patientReferrers.Length-1].RegisterReferrer.ReportEveryVisitToReferrer; // send treatment letter whether privately paid or not

                        if (invType == Booking.InvoiceType.Medicare)  // create first/last letters only if medicare
                        {
                            int nPodTreatmentsThisEPC = (int)InvoiceDB.GetMedicareCountByPatientAndDateRange(booking.Patient.PatientID, hc.DateReferralSigned.Date, DateTime.Now, -1, booking.Offering.Field.ID);
                            needToGenerateFirstLetter = (nPodTreatmentsThisEPC == 1);
                            needToGenerateLastLetter = (epcsRemaining[0].NumServicesRemaining == 0);
                        }

                        // if already generating first or last letter, don't generate treatement letter also
                        if (needToGenerateFirstLetter || needToGenerateLastLetter)
                            needToGenerateTreatmentLetter = false;


                        bool AutoSendFaxesAsEmailsIfNoEmailExistsToGPs = Convert.ToInt32(SystemVariableDB.GetByDescr("AutoSendFaxesAsEmailsIfNoEmailExistsToGPs").Value) == 1;

                        string[] emails = ContactDB.GetEmailsByEntityID(patientReferrers[patientReferrers.Length - 1].RegisterReferrer.Organisation.EntityID);
                        string[] faxes  = ContactDB.GetFaxesByEntityID (patientReferrers[patientReferrers.Length - 1].RegisterReferrer.Organisation.EntityID);

                        bool generateSystemLetters = !patientReferrers[patientReferrers.Length-1].RegisterReferrer.BatchSendAllPatientsTreatmentNotes && (emails.Length > 0 || chkGenerateSystemLetters.Checked);
                        int letterPrintHistorySendMethodID = emails.Length == 0 ? 1 : 2;

                        ArrayList fileContentsListToPopup = new ArrayList();

                        if (generateSystemLetters)
                        {
                            Letter.FileContents[] fileContentsList = booking.GetSystemLettersList(emails.Length > 0 ? Letter.FileFormat.PDF : Letter.FileFormat.Word, booking.Patient, hc, booking.Offering.Field.ID, patientReferrers[patientReferrers.Length - 1].RegisterReferrer.Referrer, true, needToGenerateFirstLetter, needToGenerateLastLetter, needToGenerateTreatmentLetter, false, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), letterPrintHistorySendMethodID);
                            if (fileContentsList != null && fileContentsList.Length > 0)
                            {

                                bool sendViaEmail = AutoSendFaxesAsEmailsIfNoEmailExistsToGPs ? (emails.Length > 0 || faxes.Length > 0) : emails.Length > 0;
                                if (sendViaEmail)
                                {
                                    string toEmail = AutoSendFaxesAsEmailsIfNoEmailExistsToGPs ?
                                                                (emails.Length > 0 ? string.Join(",", emails) : faxes[0] + "@fax.houseofit.com.au")
                                                                :
                                                                string.Join(",", emails);

                                    if (!Utilities.IsDev())
                                        Letter.EmailSystemLetter((string)Session["SiteName"], toEmail, fileContentsList);
                                }
                                else
                                {

                                    // generate pt letter
                                    if (needToGenerateLastLetter)
                                    {
                                        Letter.FileContents[] ptLastLetterFileContents = booking.GetSystemLettersList(Letter.FileFormat.Word, booking.Patient, hc, booking.Offering.Field.ID, patientReferrers[patientReferrers.Length - 1].RegisterReferrer.Referrer, true, false, false, false, true, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), letterPrintHistorySendMethodID);
                                        Letter.FileContents[] newFileContentsList = new Letter.FileContents[fileContentsList.Length + 1];
                                        fileContentsList.CopyTo(newFileContentsList, 0);
                                        newFileContentsList[newFileContentsList.Length - 1] = ptLastLetterFileContents[0];
                                        fileContentsList = newFileContentsList;
                                    }


                                    //Letter.FileContents fileContents = Letter.FileContents.Merge(fileContentsList, "Treatment Letters.pdf"); // change here to create as pdf
                                    //Session["downloadFile_Contents"] = fileContents.Contents;
                                    //Session["downloadFile_DocName"] = fileContents.DocName;
                                    //showDownloadPopup = true;

                                    fileContentsListToPopup.AddRange(fileContentsList);
                                }
                            }
                        }


                        // generate pt letter
                        if (needToGenerateLastLetter && !showDownloadPopup)  
                        {
                                Letter.FileContents[] ptLastLetterFileContents = booking.GetSystemLettersList(Letter.FileFormat.Word, booking.Patient, hc, booking.Offering.Field.ID, patientReferrers[patientReferrers.Length - 1].RegisterReferrer.Referrer, true, false, false, false, true, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), letterPrintHistorySendMethodID);
                                //Letter.FileContents fileContents = Letter.FileContents.Merge(ptLastLetterFileContents, "Treatment Letters.pdf"); // change here to create as pdf
                                //Session["downloadFile_Contents"] = fileContents.Contents;
                                //Session["downloadFile_DocName"] = fileContents.DocName;
                                //showDownloadPopup = true;
                                fileContentsListToPopup.AddRange(ptLastLetterFileContents);
                        }

                        
                        if (fileContentsListToPopup.Count == 1) // download the file
                        {
                            Letter.FileContents fc = (Letter.FileContents)fileContentsListToPopup[0];
                            fc = Letter.FileContents.Merge((Letter.FileContents[])fileContentsListToPopup.ToArray(typeof(Letter.FileContents)), Path.ChangeExtension(fc.DocName, ".pdf")); // change here to create as pdf
                            Session["downloadFile_Contents"] = fc.Contents;
                            Session["downloadFile_DocName"]  = fc.DocName;
                            showDownloadPopup = true;
                        }
                        else if (fileContentsListToPopup.Count > 1)  // Create Zip as we can not merge word docs from different templates
                        {
                            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
                            if (!Directory.Exists(tmpLettersDirectory))
                                throw new CustomMessageException("Temp letters directory doesn't exist");


                            // put into temp dir
                            string baseTmpDir = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
                            string tmpDir = baseTmpDir + "Treatment Letters" + @"\";
                            Directory.CreateDirectory(tmpDir);

                            string[] tmpFiles = new string[fileContentsListToPopup.Count];
                            for (int i = 0; i < fileContentsListToPopup.Count; i++)
                            {
                                Letter.FileContents fc = (Letter.FileContents)fileContentsListToPopup[i];
                                string tmpFileName = tmpDir + fc.DocName;
                                System.IO.File.WriteAllBytes(tmpFileName, fc.Contents);
                                tmpFiles[i] = tmpFileName;
                            }

                            // zip em
                            string zipFileName = "Treatment Letters.zip";
                            string zipFilePath = baseTmpDir + zipFileName;
                            ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                            zip.CreateEmptyDirectories = true;
                            zip.CreateZip(zipFilePath, tmpDir, true, "");

                            // get filecontents of zip here
                            Letter.FileContents zipFileContents = new Letter.FileContents(zipFilePath, zipFileName);
                            Session["downloadFile_Contents"] = zipFileContents.Contents;
                            Session["downloadFile_DocName"]  = zipFileContents.DocName;

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

                            showDownloadPopup = true;
                        }



                        if (needToGenerateLastLetter && !Utilities.IsDev())  // send SMS/Email to patient
                        {
                            bool EnableLastEPCReminderSMS    = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableLastEPCReminderSMS").Value) == 1;
                            bool EnableLastEPCReminderEmails = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableLastEPCReminderEmails").Value) == 1;


                            string[] ptMobiles;
                            if (Utilities.GetAddressType().ToString() == "Contact")
                                ptMobiles = ContactDB.GetByEntityID(-1, booking.Patient.Person.EntityID, 30).Select(r => r.AddrLine1).ToArray();
                            else if (Utilities.GetAddressType().ToString() == "ContactAus")
                                ptMobiles = ContactAusDB.GetByEntityID(-1, booking.Patient.Person.EntityID, 30).Select(r => r.AddrLine1).ToArray();
                            else
                                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());


                            string[] ptEmails = ContactDB.GetEmailsByEntityID(booking.Patient.Person.EntityID);


                            decimal balance = SMSCreditDataDB.GetTotal() - SMSHistoryDataDB.GetTotal();
                            decimal cost    = Convert.ToDecimal(SystemVariableDB.GetByDescr("SMSPrice").Value);

                            string  callerId    = System.Configuration.ConfigurationManager.AppSettings["SMSTech_callerId"];  // not here used as the callerId will be the org name
                            callerId = booking.Organisation.Name;
                            string  countryCode = System.Configuration.ConfigurationManager.AppSettings["SMSTech_CountryCode"];

                            string smsMessage   = @"Hi " + booking.Patient.Person.Firstname + @",

Please be advised that you have used your last visit under your current EPC referral. 
Please consult your doctor about a new referral to cover future consultations.

Regards, 
" + booking.Organisation.Name;

                            string emailSubject = @"EPC Referral Used Up.";
                            string emailMessage = @"Hi " + booking.Patient.Person.Firstname + @",<br /><br />Please be advised that you have used your last visit under the current EPC referral.<br />Plase consult your doctor about a new referral to cover future consultations.<br /><br />Regards, <br />" + booking.Organisation.Name;


                            if (ptMobiles.Length > 0)
                                ptMobiles[0] = ptMobiles[0].StartsWith("0") ? countryCode + ptMobiles[0].Substring(1) : ptMobiles[0];


                            bool sendingAlready = false;
                            if (EnableLastEPCReminderSMS && ptMobiles.Length > 0 && balance >= cost)
                            {
                                try
                                {
                                    TransmitSMSAPIWrapper sms = new TransmitSMSAPIWrapper(
                                        System.Configuration.ConfigurationManager.AppSettings["SMSTech_Key"],
                                        System.Configuration.ConfigurationManager.AppSettings["SMSTech_Secret"],
                                        System.Configuration.ConfigurationManager.AppSettings["SMSTech_RequestURL"]);

                                    string xmlResponse = sms.MessageSingle(ptMobiles[0], smsMessage, callerId);

                                    if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogSMSSending"]))
                                        Logger.LogSMSSend(
                                            System.Web.HttpContext.Current.Session["DB"].ToString() + Environment.NewLine + Environment.NewLine +
                                            ptMobiles[0] + Environment.NewLine + Environment.NewLine +
                                            smsMessage   + Environment.NewLine + Environment.NewLine +
                                            callerId     + Environment.NewLine + Environment.NewLine +
                                            xmlResponse,
                                            true);

                                    string SMSTechMessageID = GetSMSTechMessageID(xmlResponse);
                                    SMSHistoryDataDB.Insert(1, booking.Patient.PatientID, -1, ptMobiles[0], smsMessage, cost, SMSTechMessageID);

                                    sendingAlready = true;
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogException(ex, true);
                                }
                            }
                            if (EnableLastEPCReminderEmails && ptEmails.Length > 0)
                            {
                                if (!sendingAlready)  // if not already added for sms sending
                                {
                                    try
                                    {
                                        string fromEmail = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value;
                                        EmailerNew.SimpleEmail(
                                            fromEmail,
                                            booking.Organisation.Name,
                                            string.Join(",", ptEmails),
                                            emailSubject,
                                            emailMessage,
                                            true,
                                            null,
                                            false,
                                            null
                                            );

                                        EmailHistoryDataDB.Insert(1, booking.Patient.PatientID, -1, string.Join(",", ptEmails), emailMessage);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogException(ex, true);
                                    }
                                }
                            }

                        }


                        BookingDB.UpdateSetGeneratedSystemLetters(booking.BookingID, needToGenerateFirstLetter, needToGenerateLastLetter, generateSystemLetters);


                        /*
                        if (showEPCTreatmentDownloadPopup && !showStandardTreatmentDownloadPopup)
                        {
                            Session["downloadFile_Contents"] = fileContentsEPCTreatment.Contents;
                            Session["downloadFile_DocName"]  = fileContentsEPCTreatment.DocName;
                            showDownloadPopup = true;
                        }
                        else if (!showEPCTreatmentDownloadPopup && showStandardTreatmentDownloadPopup)
                        {
                            Session["downloadFile_Contents"] = fileContentsStandardTreatment.Contents;
                            Session["downloadFile_DocName"]  = fileContentsStandardTreatment.DocName;
                            showDownloadPopup = true;
                        }
                        else if (showEPCTreatmentDownloadPopup && showStandardTreatmentDownloadPopup)
                        {
                            // merge

                            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
                            if (!Directory.Exists(tmpLettersDirectory))
                                throw new CustomMessageException("Temp letters directory doesn't exist");

                            string tmpFileName_EPCTreatment = FileHelper.GetTempFileName(tmpLettersDirectory + fileContentsEPCTreatment.DocName);
                            File.WriteAllBytes(tmpFileName_EPCTreatment, fileContentsEPCTreatment.Contents);
                            string tmpFileName_StandardTreatment = FileHelper.GetTempFileName(tmpLettersDirectory + fileContentsStandardTreatment.DocName);
                            File.WriteAllBytes(tmpFileName_StandardTreatment, fileContentsStandardTreatment.Contents);

                            string tmpFinalFileName = Letter.MergeMultipleDocuments(new string[] { tmpFileName_EPCTreatment, tmpFileName_StandardTreatment }, tmpLettersDirectory + Path.GetFileName(fileContentsEPCTreatment.DocName));
                            byte[] fileContents = System.IO.File.ReadAllBytes(tmpFinalFileName);

                            File.Delete(tmpFileName_EPCTreatment);
                            File.Delete(tmpFileName_StandardTreatment);
                            File.Delete(tmpFinalFileName);

                            Session["downloadFile_Contents"] = fileContents;
                            Session["downloadFile_DocName"] = fileContentsEPCTreatment.DocName;
                            showDownloadPopup = true;
                        }
                        */

                    }
                }

                // clear in memory (ie session) list
                Session.Remove("data_selected");



                // successfully completed, so update and check warning level for stocks
                foreach (InvoiceLine invoiceLine in invoiceLines)
                    if (invoiceLine.OfferingOrder == null) // stkip counting down if item is on order
                        StockDB.UpdateAndCheckWarning(booking.Organisation.OrganisationID, invoiceLine.Offering.OfferingID, (int)invoiceLine.Quantity);



                if (nonHcInvID > 0 && nonHcInvTotal > 0)  // go to pay screen - but pass in "showDownloadPopup" somehow .. and close window with same as below script...
                {
                    System.Drawing.Size size = Receipt.GetPopupWindowAddSize();
                    size = new System.Drawing.Size(size.Width + 15, size.Height + 60);
                    Response.Redirect("~/Invoice_ReceiptAndCreditNoteAddV2.aspx?id=" + nonHcInvID + "&returnValue=" + (showDownloadPopup ? "true" : "false") + "&window_size=" + size.Width + "_" + size.Height + (refresh_on_close ? "&refresh_on_close=1" : ""), false);
                    return;
                }

            }
            catch (Exception ex)
            {
                if (ex is CustomMessageException == false)
                    Logger.LogException(ex);

                // roll back...
                BookingDB.UpdateSetBookingStatusID(booking.BookingID, 0);
                BookingDB.UpdateSetGeneratedSystemLetters(booking.BookingID, booking.NeedToGenerateFirstLetter, booking.NeedToGenerateLastLetter, booking.HasGeneratedSystemLetters);
                foreach (int invLineID in invLineIDs)
                    InvoiceLineDB.Delete(invLineID);
                foreach (int offeringOrderID in offeringOrderIDs)
                    OfferingOrderDB.Delete(offeringOrderID);
                InvoiceDB.Delete(hcInvID);
                InvoiceDB.Delete(nonHcInvID);
                for (int j = 0; j < epcsRemainingOriginal.Length; j++)
                    HealthCardEPCRemainingDB.UpdateNumServicesRemaining(epcsRemainingOriginal[j].HealthCardEpcRemainingID, epcsRemainingOriginal[j].NumServicesRemaining);

                throw;
            }


            // close this window
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=" + (showDownloadPopup ? "true" : "false") + ";" + (refresh_on_close ? "window.opener.location.href = window.opener.location.href;" : "") + "self.close();</script>");

        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Message.StartsWith("No claim numbers left") || sqlEx.Message.StartsWith("Error: Claim number already in use"))
                SetErrorMessage(sqlEx.Message);
            else
                SetErrorMessage(Utilities.IsDev() ? sqlEx.ToString() : "");
            return;
        }
        catch (Exception ex)
        {
            SetErrorMessage("", ex.ToString());
            return;
        }
    }

    #endregion

    #region SendSMSes SendEmails

    protected static void SendSMSes(Tuple<int, decimal, string, string, string>[] messagesInfo)
    {
        for (int i = 0; i < messagesInfo.Length; i++)
        {
            Tuple<int, decimal, string, string, string> messageInfo = (Tuple<int, decimal, string, string, string>)messagesInfo[i];
            int bookingID = messageInfo.Item1;
            decimal cost = messageInfo.Item2;
            string mobile = messageInfo.Item3;
            string message = messageInfo.Item4;
            string callerId = messageInfo.Item5;

            // until they have an interface to send all in one call, it's better to put each 
            // in its own try/catch block so if some causes an error, it doesn't ruin all of them.
            try
            {
                TransmitSMSAPIWrapper sms = new TransmitSMSAPIWrapper(
                    System.Configuration.ConfigurationManager.AppSettings["SMSTech_Key"],
                    System.Configuration.ConfigurationManager.AppSettings["SMSTech_Secret"],
                    System.Configuration.ConfigurationManager.AppSettings["SMSTech_RequestURL"]);

                string xmlResponse = sms.MessageSingle(mobile, message, callerId);

                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogSMSSending"]))
                    Logger.LogSMSSend(
                        System.Web.HttpContext.Current.Session["DB"].ToString() + Environment.NewLine + Environment.NewLine +
                        mobile + Environment.NewLine + Environment.NewLine +
                        message + Environment.NewLine + Environment.NewLine +
                        callerId + Environment.NewLine + Environment.NewLine +
                        xmlResponse,
                        true);

                string SMSTechMessageID = GetSMSTechMessageID(xmlResponse);
                SMSHistoryDataDB.Insert(1, -1, bookingID, mobile, message, cost, SMSTechMessageID);
            }
            catch (Exception ex)
            {
                Logger.LogSMSSend(
                    mobile + Environment.NewLine + Environment.NewLine +
                    message + Environment.NewLine + Environment.NewLine +
                    cost + Environment.NewLine + Environment.NewLine +
                    callerId + Environment.NewLine + Environment.NewLine +
                    ex.ToString(),
                    true);

                Logger.LogException(ex, true);
            }

        }
    }
    protected static string GetSMSTechMessageID(string xmlResponse)
    {
        using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlResponse)))
        {
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.Name == "message_id")
                {
                    reader.Read();
                    return reader.Value;
                }
            }
        }

        return string.Empty;
    }


    protected static void SendEmails(Tuple<int, string, string, string, string>[] messagesInfo)
    {
        for (int i = 0; i < messagesInfo.Length; i++)
        {
            Tuple<int, string, string, string, string> messageInfo = (Tuple<int, string, string, string, string>)messagesInfo[i];
            int bookingID = messageInfo.Item1;
            string orgName = messageInfo.Item2;
            string email = messageInfo.Item3;
            string message = messageInfo.Item4;
            string subject = messageInfo.Item5;

            try
            {
                Emailer.SimpleEmail(
                    orgName,
                    email,
                    subject,
                    message,
                    true,
                    null,
                    null
                    );

                EmailHistoryDataDB.Insert(1, -1, bookingID, email, message);
            }
            catch (Exception ex)
            {
                Logger.LogSMSSend(
                    email + Environment.NewLine + Environment.NewLine +
                    message + Environment.NewLine + Environment.NewLine +
                    subject + Environment.NewLine + Environment.NewLine +
                    ex.ToString(),
                    true);

                Logger.LogException(ex, true);
            }

        }
    }

    #endregion



    #region InvoiceItemsControl_MakePrivateInvoiceLinkClicked

    private void InvoiceItemsControl_MakePrivateInvoiceLinkClicked(object sender, EventArgs e)
    {
        string newURL = UrlParamModifier.AddEdit(Request.RawUrl, "is_private", "1");
        Response.Redirect(newURL);
        return;
    }

    #endregion

    #region GetInvoiceType()

    private Booking.InvoiceType GetInvoiceType()
    {
        Booking booking = GetFormBooking();
        if (booking == null)
            throw new CustomMessageException("Invalid booking");

        return booking.GetInvoiceType();
    }

    #endregion


    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        invoiceItemsControl.HideElementsForError();
        chkGenerateSystemLetters.Visible = false;
        lnkNotes.Visible = false;
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