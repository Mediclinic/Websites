using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.IO;
using System.Collections;


public class Letter
{

    public enum FileFormat { Word, PDF };

    public class FileContents
    {
        public byte[] Contents;
        public string DocName;

        public FileContents(byte[] Contents, string DocName)
        {
            this.Contents = Contents;
            this.DocName = DocName;
        }

        public FileContents(string fullpath, string DocName)
        {
            this.Contents = File.ReadAllBytes(fullpath);
            this.DocName = DocName;
        }

        public static FileContents Merge(FileContents[] multipleFileContents, string mergedDocName)
        {
            if (multipleFileContents.Length == 1)
            {
                if (Path.GetExtension(mergedDocName).ToUpper() == ".PDF")
                {
                    string tmpLettersDirectory = Letter.GetTempLettersDirectory();
                    if (!System.IO.Directory.Exists(tmpLettersDirectory))
                        throw new CustomMessageException("Temp letters directory doesn't exist");

                    string tmpSourceFileName = FileHelper.GetTempFileName(tmpLettersDirectory + multipleFileContents[0].DocName);
                    string tmpDestFileName   = FileHelper.GetTempFileName(tmpLettersDirectory + mergedDocName);


                    File.WriteAllBytes(tmpSourceFileName, multipleFileContents[0].Contents);

                    string errorString = string.Empty;
                    OfficeInterop.FormatConverter.WordToPDF(tmpSourceFileName, tmpDestFileName, out errorString);

                    byte[] pdf = File.ReadAllBytes(tmpDestFileName);

                    File.Delete(tmpSourceFileName);
                    File.Delete(tmpDestFileName);

                    return new FileContents(pdf, mergedDocName);
                }
                else
                {
                    return multipleFileContents[0];
                }
            }
            else if (multipleFileContents.Length > 1)
            {
                string tmpLettersDirectory = Letter.GetTempLettersDirectory();
                if (!System.IO.Directory.Exists(tmpLettersDirectory))
                    throw new CustomMessageException("Temp letters directory doesn't exist");

                string[] tmpFiles = new string[multipleFileContents.Length];
                for (int i = 0; i < multipleFileContents.Length; i++)
                {
                    string tmpFileName = FileHelper.GetTempFileName(tmpLettersDirectory + multipleFileContents[i].DocName);
                    System.IO.File.WriteAllBytes(tmpFileName, multipleFileContents[i].Contents);
                    tmpFiles[i] = tmpFileName;
                }

                string tmpFinalFileName = Letter.MergeMultipleDocuments(tmpFiles, tmpLettersDirectory + System.IO.Path.GetFileName(mergedDocName));
                byte[] fileContents = System.IO.File.ReadAllBytes(tmpFinalFileName);

                foreach (string file in tmpFiles)
                    System.IO.File.Delete(file);
                System.IO.File.Delete(tmpFinalFileName);

                return new Letter.FileContents(fileContents, mergedDocName);
            }
            else // if (fileContentsList.Length == 0)
            {
                return null;
            }
        }

    }

    public static FileContents ConvertContentsToPDF(FileContents fileContents)
    {
        string convertedDocName = System.IO.Path.ChangeExtension(fileContents.DocName, ".pdf");

        string tmpLettersDirectory = Letter.GetTempLettersDirectory();
        if (!System.IO.Directory.Exists(tmpLettersDirectory))
            throw new CustomMessageException("Temp letters directory doesn't exist");

        string tmpSourceFileName = FileHelper.GetTempFileName(tmpLettersDirectory + fileContents.DocName);
        string tmpDestFileName = FileHelper.GetTempFileName(tmpLettersDirectory + convertedDocName);


        File.WriteAllBytes(tmpSourceFileName, fileContents.Contents);

        string errorString = string.Empty;
        OfficeInterop.FormatConverter.WordToPDF(tmpSourceFileName, tmpDestFileName, out errorString);

        byte[] pdf = File.ReadAllBytes(tmpDestFileName);

        File.Delete(tmpSourceFileName);
        File.Delete(tmpDestFileName);

        return new Letter.FileContents(pdf, convertedDocName);
    }


    public Letter(int letter_id, int organisation_id, int letter_type_id, int site_id, string code, string reject_message, string docname, bool is_send_to_medico, bool is_allowed_reclaim,
                bool is_manual_override, int num_copies_to_print, bool is_deleted)
    {
        this.letter_id = letter_id;
        this.organisation = organisation_id == 0 ? null : new Organisation(organisation_id);
        this.letter_type = new IDandDescr(letter_type_id);
        this.site = new Site(site_id);
        this.code = code;
        this.reject_message = reject_message;
        this.docname = docname;
        this.is_send_to_medico = is_send_to_medico;
        this.is_allowed_reclaim = is_allowed_reclaim;
        this.is_manual_override = is_manual_override;
        this.num_copies_to_print = num_copies_to_print;
        this.is_deleted = is_deleted;
    }
    public Letter(int letter_id)
    {
        this.letter_id = letter_id;
    }

    private int letter_id;
    public int LetterID
    {
        get { return this.letter_id; }
        set { this.letter_id = value; }
    }
    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private IDandDescr letter_type;
    public IDandDescr LetterType
    {
        get { return this.letter_type; }
        set { this.letter_type = value; }
    }
    private Site site;
    public Site Site
    {
        get { return this.site; }
        set { this.site = value; }
    }
    private string code;
    public string Code
    {
        get { return this.code; }
        set { this.code = value; }
    }
    private string reject_message;
    public string RejectMessage
    {
        get { return this.reject_message; }
        set { this.reject_message = value; }
    }
    private string docname;
    public string Docname
    {
        get { return this.docname; }
        set { this.docname = value; }
    }
    private bool is_send_to_medico;
    public bool IsSendToMedico
    {
        get { return this.is_send_to_medico; }
        set { this.is_send_to_medico = value; }
    }
    private bool is_allowed_reclaim;
    public bool IsAllowedReclaim
    {
        get { return this.is_allowed_reclaim; }
        set { this.is_allowed_reclaim = value; }
    }
    private bool is_manual_override;
    public bool IsManualOverride
    {
        get { return this.is_manual_override; }
        set { this.is_manual_override = value; }
    }
    private int num_copies_to_print;
    public int NumCopiesToPrint
    {
        get { return this.num_copies_to_print; }
        set { this.num_copies_to_print = value; }
    }
    private bool is_deleted;
    public bool IsDeleted
    {
        get { return this.is_deleted; }
        set { this.is_deleted = value; }
    }
    public override string ToString()
    {
        return letter_id.ToString() + " " + organisation.OrganisationID.ToString() + " " + letter_type.ID.ToString() + " " + site.SiteID + " " + code.ToString() + " " + docname.ToString() + " " + is_send_to_medico.ToString() + " " +
                is_allowed_reclaim.ToString() + " " + is_manual_override.ToString() + " " + num_copies_to_print.ToString() + " " + is_deleted.ToString();
    }


    public bool FileExists(int siteID)
    {
        return File.Exists(this.GetFullPath(siteID));
    }

    public string GetFullPath(int siteID)
    {
        string lettersDir = GetLettersDirectory();

        bool useDefaultDocs = this.Organisation == null ? true : !LetterDB.OrgHasdocs(this.Organisation.OrganisationID);
        string sourceTemplatePath = lettersDir + (useDefaultDocs ? @"Default\" + siteID + @"\" : this.Organisation.OrganisationID + @"\") + this.Docname;
        return sourceTemplatePath;
    }

    public static string GetLettersDirectory()
    {
        string lettersDir = System.Configuration.ConfigurationManager.AppSettings["LettersDirectory"];
        lettersDir = lettersDir.Replace("[DBNAME]", System.Web.HttpContext.Current.Session["DB"].ToString().Replace("_TestDB", ""));
        if (!lettersDir.EndsWith(@"\"))
            lettersDir += @"\";

        if (!Directory.Exists(lettersDir))
            Directory.CreateDirectory(lettersDir);
        if (!Directory.Exists(lettersDir))
            throw new CustomMessageException("Couldn't create Letters Directory");

        return lettersDir;
    }
    public static string GetLettersHistoryDirectory(int orgID)
    {
        string historyDir = System.Configuration.ConfigurationManager.AppSettings["LettersHistoryDirectory"];
        historyDir = historyDir.Replace("[DBNAME]", System.Web.HttpContext.Current.Session["DB"].ToString());
        if (!historyDir.EndsWith(@"\"))
            historyDir += @"\";
        historyDir += orgID + @"\";
        return historyDir;
    }

    public static string GetTempLettersDirectory()
    {
        string tmpLettersDirectory = System.Configuration.ConfigurationManager.AppSettings["TmpLettersDirectory"];
        if (!tmpLettersDirectory.EndsWith(@"\"))
            tmpLettersDirectory = tmpLettersDirectory + @"\";

        return tmpLettersDirectory;
    }


    #region Email

    public static void EmailSystemLetter(string siteName, string emailTo, Letter.FileContents[] fileContentsList, string subject = null, string htmlBody = null)
    {
        if (subject == null)
            subject = "Referral/Treatment Note Letters From Mediclinic";
        if (htmlBody == null)
            htmlBody = "Please find attached referral/treatment note letters for your referrered patient<br /><br />Best regards,<br />" + siteName;

        Email(siteName,
            emailTo,
            subject,
            htmlBody,
            true,
            fileContentsList
            );
    }

    public static void EmailAsyncSystemLetter(string siteName, string emailTo, Letter.FileContents[] fileContentsList, string subject = null, string htmlBody = null)
    {
        if (subject == null)
            subject = "Referral/Treatment Note Letters From Mediclinic";
        if (htmlBody == null)
            htmlBody = "Please find attached referral/treatment note letters for your referrered patient<br /><br />Best regards,<br />" + siteName;

        EmailAsync(siteName,
            emailTo,
            subject,
            htmlBody,
            true,
            fileContentsList
            );
    }

    protected static bool logEmailGeneratingSendingTime = false;
    public static void Email(string fromName, string emailTo, string emailSubject, string emailMessage, bool emailIsHtml, Letter.FileContents[] fileContentsList)
    {
        string fromEmail = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value;

        SyncEmail(fromEmail, fromName, emailTo, emailSubject, emailMessage, emailIsHtml, fileContentsList);
        //AsyncEmail(fromName, emailTo, emailSubject, emailMessage, emailIsHtml, fileContentsList);

        if (logEmailGeneratingSendingTime)
            Logger.LogQuery("Done: " + DateTime.Now.ToLongTimeString());
    }
    public static void EmailAsync(string fromName, string emailTo, string emailSubject, string emailMessage, bool emailIsHtml, Letter.FileContents[] fileContentsList)
    {
        string fromEmail = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value;

        //SyncEmail(fromName, emailTo, emailSubject, emailMessage, emailIsHtml, fileContentsList);
        AsyncEmail(fromEmail, fromName, emailTo, emailSubject, emailMessage, emailIsHtml, fileContentsList);

        if (logEmailGeneratingSendingTime)
            Logger.LogQuery("Done: " + DateTime.Now.ToLongTimeString());
    }

    //
    // cant use async to send attachments... which is all the letter sending files in here...
    //

    protected delegate void AsyncEmailDelegate(string fromEmail, string fromName, string emailTo, string emailSubject, string emailMessage, bool emailIsHtml, Letter.FileContents[] fileContentsList);
    protected static void AsyncEmail(string fromEmail, string fromName, string emailTo, string emailSubject, string emailMessage, bool emailIsHtml, Letter.FileContents[] fileContentsList)
    {
        //create delegate and invoke it asynchrnously, control passes past this line while this is done in another thread
        AsyncEmailDelegate myAction = new AsyncEmailDelegate(SyncEmail);
        myAction.BeginInvoke(fromEmail, fromName, emailTo, emailSubject, emailMessage, emailIsHtml, fileContentsList, null, null);
    }

    protected static void SyncEmail(string fromEmail, string fromName, string emailTo, string emailSubject, string emailMessage, bool emailIsHtml, Letter.FileContents[] fileContentsList)
    {

        if (logEmailGeneratingSendingTime)
            Logger.LogQuery("Time A: " + DateTime.Now.ToLongTimeString());

        string   tmpDir      = null;
        string[] attachments = new string[fileContentsList.Length];  // file paths

        try
        {
            string to      = emailTo;
            string subject = emailSubject;
            string message = emailMessage;
            bool   isHtml  = emailIsHtml;

            if (logEmailGeneratingSendingTime)
                Logger.LogQuery("Time B: " + DateTime.Now.ToLongTimeString());

            // get attachments into files on the server
            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            if (!System.IO.Directory.Exists(tmpLettersDirectory))
                throw new CustomMessageException("Temp letters directory doesn't exist");
            tmpDir = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
            Directory.CreateDirectory(tmpDir);
            for (int i = 0; i < fileContentsList.Length; i++)
            {
                string tmpFileName = tmpDir + fileContentsList[i].DocName;
                System.IO.File.WriteAllBytes(tmpFileName, ((Letter.FileContents)fileContentsList[i]).Contents);
                attachments[i] = tmpFileName;
            }

            if (logEmailGeneratingSendingTime)
                Logger.LogQuery("Time C: " + DateTime.Now.ToLongTimeString());

            EmailerNew.SimpleEmail(
                fromEmail,
                fromName,
                to,
                subject,
                message,
                true,
                attachments,
                false,
                null
                );

            if (logEmailGeneratingSendingTime)
                Logger.LogQuery("Time D: " + DateTime.Now.ToLongTimeString());

        }
        finally
        {
            if (logEmailGeneratingSendingTime)
                Logger.LogQuery("Time E: " + DateTime.Now.ToLongTimeString());


            // delete temp files
            if (attachments != null)
            {
                foreach (string file in attachments)
                {
                    try { if (System.IO.File.Exists(file)) System.IO.File.Delete(file); }
                    catch (Exception){}
                }
            }

            // delete temp dir
            if (tmpDir != null)
            {

                try { Directory.Delete(tmpDir, true); }
                catch (Exception) { }
            }
        }

    }

    #endregion

    #region GenerateInvoices

    public static void GenerateInvoicesToPrint(int[] invoiceIDs, HttpResponse response, bool isClinicInvoice, bool isBookingInvoice = true)
    {
        string originalFile = null;
        if (isClinicInvoice)
            originalFile = Letter.GetLettersDirectory() + (isBookingInvoice ? @"InvoiceTemplate.docx" : @"PrivateInvoiceTemplate.docx");
        else
            originalFile = Letter.GetLettersDirectory() + (isBookingInvoice ? @"InvoiceTemplateAC.docx" : @"PrivateInvoiceTemplateAC.docx"); 
        
        string tmpLettersDirectory = Letter.GetTempLettersDirectory();

        string[] generatedFiles = new string[invoiceIDs.Length];
        for (int i = 0; i < invoiceIDs.Length; i++)
        {
            string tmpOutputFile = FileHelper.GetTempFileName(tmpLettersDirectory + "Invoice_" + invoiceIDs[i] + "." + System.IO.Path.GetExtension(originalFile));
            if (isBookingInvoice)
                GenerateInvoice(invoiceIDs[i], isClinicInvoice, originalFile, tmpOutputFile);
            else
                GeneratePrivateInvoice(invoiceIDs[i], isClinicInvoice, originalFile, tmpOutputFile);
            generatedFiles[i] = tmpOutputFile;
        }

        // merge all tmp files
        string tmpOutputFileFinal = FileHelper.GetTempFileName(tmpLettersDirectory + "Invoices." + "pdf");
        string tmpFinalFileName = Letter.MergeMultipleDocuments(generatedFiles, tmpOutputFileFinal);

        // delete all single tmp files
        foreach (string file in generatedFiles)
            File.Delete(file);

        // download the document
        byte[] fileContents = File.ReadAllBytes(tmpFinalFileName);
        System.IO.File.Delete(tmpFinalFileName);

        // Nothing gets past the "DownloadDocument" method because it outputs the file 
        // which is writing a response to the client browser and calls Response.End()
        // So make sure any other code that functions goes before this
        string filename = invoiceIDs.Length == 1 ? "Invoice_" + invoiceIDs[0] + "." + System.IO.Path.GetExtension(originalFile) : "Invoices." + System.IO.Path.GetExtension(originalFile);
        Letter.DownloadDocument(response, fileContents, Path.ChangeExtension(filename, ".pdf"));
    }

    public static Tuple<string, string[]> GenerateInvoiceToEmail(int invoiceID, bool isClinicInvoice)
    {
        Invoice invoice = InvoiceDB.GetByID(invoiceID);
        if (invoice == null)
            throw new CustomMessageException("No such invoice exists.");

        if (invoice.PayerPatient != null)
        {
            string[] emails = ContactDB.GetEmailsByEntityID(invoice.PayerPatient.Person.EntityID);
            
            if (emails.Length == 0)
                throw new CustomMessageException("No email address set for " + invoice.PayerPatient.Person.FullnameWithoutMiddlename);

            Letter.GenerateInvoicesToEmail(new int[] { invoiceID }, string.Join(",", emails), isClinicInvoice, invoice.Booking != null);

            return new Tuple<string, string[]>(invoice.PayerPatient.Person.FullnameWithoutMiddlename, emails);
        }
        else if (invoice.PayerOrganisation != null)
        {
            string[] emails = ContactDB.GetEmailsByEntityID(invoice.PayerOrganisation.EntityID);

            if (invoice.PayerOrganisation.OrganisationID == -1 || invoice.PayerOrganisation.OrganisationID == -2)
                throw new CustomMessageException("Can not email invoice to Medicare");

            if (emails.Length == 0)
                throw new CustomMessageException("No email address set for " + invoice.PayerOrganisation.Name);

            Letter.GenerateInvoicesToEmail(new int[] { invoiceID }, string.Join(",", emails), isClinicInvoice);

            return new Tuple<string, string[]>(invoice.PayerOrganisation.Name, emails);
        }
        else if (invoice.Booking != null) // clinic booking, so use patient from booking
        {
            string[] emails = ContactDB.GetEmailsByEntityID(invoice.Booking.Patient.Person.EntityID);

            if (emails.Length == 0)
                throw new CustomMessageException("No email address set for " + invoice.Booking.Patient.Person.FullnameWithoutMiddlename);

            Letter.GenerateInvoicesToEmail(new int[] { invoiceID }, string.Join(",", emails), isClinicInvoice);

            return new Tuple<string, string[]>(invoice.Booking.Patient.Person.FullnameWithoutMiddlename, emails);
        }
        else // private invoice, no booking
        {
            throw new CustomMessageException("Can not email private invoices with no customer set");
        }
    }


    public static void GenerateInvoicesToEmail(int[] invoiceIDs, string emailTo, bool isClinicInvoice, bool isBookingInvoice = true)
    {
        //AsyncGenerateInvoicesToEmail(invoiceIDs, emailTo, isClinicInvoice, isBookingInvoice);  // problem with this is that the context's "session" becomes null while executing and you cant re-set the context's session object
        SyncGenerateInvoicesToEmail(invoiceIDs, emailTo, isClinicInvoice, isBookingInvoice);
    }
    protected delegate void AsyncGenerateInvoicesToEmailDelegate(int[] invoiceIDs, string emailTo, bool isClinicInvoice, bool isBookingInvoice = true);
    protected static void AsyncGenerateInvoicesToEmail(int[] invoiceIDs, string emailTo, bool isClinicInvoice, bool isBookingInvoice = true)
    {
        //create delegate and invoke it asynchrnously, control passes past this line while this is done in another thread
        AsyncGenerateInvoicesToEmailDelegate myAction = new AsyncGenerateInvoicesToEmailDelegate(SyncGenerateInvoicesToEmail);
        myAction.BeginInvoke(invoiceIDs, emailTo, isClinicInvoice, isBookingInvoice, null, null);
    }
    protected static void SyncGenerateInvoicesToEmail(int[] invoiceIDs, string emailTo, bool isClinicInvoice, bool isBookingInvoice = true)
    {
        string originalFile = null;
        if (isClinicInvoice)
            originalFile = Letter.GetLettersDirectory() + (isBookingInvoice ? @"InvoiceTemplate.docx" : @"PrivateInvoiceTemplate.docx");
        else
            originalFile = Letter.GetLettersDirectory() + (isBookingInvoice ? @"InvoiceTemplateAC.docx" : @"PrivateInvoiceTemplateAC.docx");

        string tmpLettersDirectory = Letter.GetTempLettersDirectory();

        Letter.FileContents[] fileContentsList = new Letter.FileContents[invoiceIDs.Length];
        for (int i = 0; i < invoiceIDs.Length; i++)
        {
            string tmpOutputFile = FileHelper.GetTempFileName(tmpLettersDirectory + "Invoice_" + invoiceIDs[i] + "." + "pdf");
            if (isBookingInvoice)
                GenerateInvoice(invoiceIDs[i], isClinicInvoice, originalFile, tmpOutputFile);
            else
                GeneratePrivateInvoice(invoiceIDs[i], isClinicInvoice, originalFile, tmpOutputFile);
            fileContentsList[i] = new FileContents(System.IO.File.ReadAllBytes(tmpOutputFile), "Invoice_" + invoiceIDs[i] + "." + "pdf"); 
            File.Delete(tmpOutputFile);
        }

        Site site = SiteDB.GetSiteByType(!isClinicInvoice ? SiteDB.SiteType.AgedCare : SiteDB.SiteType.Clinic);
        Email(
            site.Name,
            emailTo,
            "Invoice" + (fileContentsList.Length == 0 ? "" : "s") + " From " + site.Name,
            "Please find invoice" + (fileContentsList.Length == 0 ? "" : "s") + " attached.<br /><br />Best regards,<br />" + site.Name,
            true,
            fileContentsList
            );
    }

    /*
    SELECT

        TOP (100)
        (SELECT        COUNT(*) AS Expr1
         FROM            InvoiceLine
         WHERE        (invoice_id = Invoice.invoice_id)) AS invoice_lines_count, 
        invoice_id, entity_id, invoice_type_id, booking_id, payer_organisation_id, payer_patient_id, non_booking_invoice_organisation_id, healthcare_claim_number, reject_letter_id, staff_id, site_id, invoice_date_added, total, gst, is_paid, is_refund, is_batched, reversed_by, reversed_date

    FROM     Invoice
    ORDER BY invoice_lines_count DESC

    */

    protected static void GenerateInvoice(int invoiceID, bool isClinicInvoice, string originalFile, string outputFile)
    {
        Invoice       invoice     = InvoiceDB.GetByID(invoiceID);
        InvoiceLine[] lines       = InvoiceLineDB.GetByInvoiceID(invoice.InvoiceID);
        Receipt[]     receipts    = ReceiptDB.GetByInvoice(invoice.InvoiceID);
        CreditNote[]  creditNotes = CreditNoteDB.GetByInvoice(invoice.InvoiceID);

        bool isMedicareOrDVAInvoice = invoice.PayerOrganisation != null && (invoice.PayerOrganisation.OrganisationID == -1 || invoice.PayerOrganisation.OrganisationID == -2);
        bool isTACInvoice           = invoice.PayerOrganisation != null && invoice.PayerOrganisation.OrganisationType.OrganisationTypeID == 150;
        bool invoiceGapPayments     = Convert.ToInt32(SystemVariableDB.GetByDescr("InvoiceGapPayments").Value) == 1;

        Site site = SiteDB.GetByID(Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteID"]));

        // was rejected when
        // - pt pays invoice, and
        // - booking has a medicare/dva invoice that has been rejected
        //      ie  payer org = -1/-2 and single adj/cr note equal to total, and single inv line with same offering as this one single inv line)
        //
        bool wasRejectedMedicareInvoice = false;
        if (!isMedicareOrDVAInvoice && lines.Length == 1)
        {
            foreach (Invoice curInvoice in InvoiceDB.GetByBookingID(invoice.Booking.BookingID, false))
            {
                // make sure is medicare/dva booking
                if (curInvoice.PayerOrganisation == null || (curInvoice.PayerOrganisation.OrganisationID != -1 && curInvoice.PayerOrganisation.OrganisationID != -2))
                    continue;

                // make sure is wiped (paid, nothing due, no receipts, credit notes == total of bill)
                if (!curInvoice.IsPaID || curInvoice.TotalDue != 0 || curInvoice.ReceiptsTotal != 0 || curInvoice.CreditNotesTotal != curInvoice.Total)
                    continue;

                // if only one invoice line on both this and the non-mediare/dva invoice and same offering
                InvoiceLine[] curInvLines = InvoiceLineDB.GetByInvoiceID(curInvoice.InvoiceID);
                if (curInvLines.Length == 1                                            && 
                    lines.Length       == 1                                            && 
                    curInvLines[0].Offering.OfferingID == lines[0].Offering.OfferingID &&
                    curInvLines[0].Price               == lines[0].Price               &&
                    curInvLines[0].Quantity            == lines[0].Quantity            &&
                    curInvLines[0].Tax                 == lines[0].Tax)
                        wasRejectedMedicareInvoice = true;
            }
        }

        string message = invoice.Message.Length > 0 ? invoice.Message : (wasRejectedMedicareInvoice ? "*Claim rejected by Medicare" : "");


        Hashtable acPtBedroomHash = null;
        if (!isClinicInvoice)
            acPtBedroomHash = PatientsContactCacheDB.GetBullkBedrooms(lines.Select(x => x.Patient.Person.EntityID).ToArray(), -1);




        /*
         * First merge the invoice fields
         */


        // create dataset of merge fields from invoice

        System.Data.DataSet sourceDataSet = new System.Data.DataSet();
        sourceDataSet.Tables.Add("MergeIt");

        sourceDataSet.Tables[0].Columns.Add("curr_date");
        sourceDataSet.Tables[0].Columns.Add("inv_nbr");
        sourceDataSet.Tables[0].Columns.Add("inv_date");
        sourceDataSet.Tables[0].Columns.Add("inv_debtor_name");
        sourceDataSet.Tables[0].Columns.Add("inv_debtor_addr");
        sourceDataSet.Tables[0].Columns.Add("inv_debtor_addr_tabbedx1");
        sourceDataSet.Tables[0].Columns.Add("bk_pt_fullname");
        sourceDataSet.Tables[0].Columns.Add("bk_pt_addr");
        sourceDataSet.Tables[0].Columns.Add("bk_pt_addr_tabbedx1");
        sourceDataSet.Tables[0].Columns.Add("bk_prov_fullname");
        sourceDataSet.Tables[0].Columns.Add("bk_prov_number");
        sourceDataSet.Tables[0].Columns.Add("bk_date");
        sourceDataSet.Tables[0].Columns.Add("bk_next_info");
        sourceDataSet.Tables[0].Columns.Add("bk_purchase_order_nbr");

        sourceDataSet.Tables[0].Columns.Add("pt_name");           // depricated as same as bk_pt_fullname
        sourceDataSet.Tables[0].Columns.Add("pt_addr");           // depricated as same as bk_addr
        sourceDataSet.Tables[0].Columns.Add("pt_addr_tabbedx1");  // depricated as same as bk_addr_tabbedx1
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_line1");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_line2");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_street");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_suburb");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_postcode");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_country");


        sourceDataSet.Tables[0].Columns.Add("pt_hc_card_nbr");
        sourceDataSet.Tables[0].Columns.Add("pt_hc_card_name");
        sourceDataSet.Tables[0].Columns.Add("pt_hc_card_refsigneddate");
        sourceDataSet.Tables[0].Columns.Add("pt_epc_expire_date");
        sourceDataSet.Tables[0].Columns.Add("pt_epc_count_remaining");

        sourceDataSet.Tables[0].Columns.Add("ref_name");
        sourceDataSet.Tables[0].Columns.Add("ref_title");
        sourceDataSet.Tables[0].Columns.Add("ref_firstname");
        sourceDataSet.Tables[0].Columns.Add("ref_middlename");
        sourceDataSet.Tables[0].Columns.Add("ref_surname");


        sourceDataSet.Tables[0].Columns.Add("bk_org_name");
        sourceDataSet.Tables[0].Columns.Add("bk_org_abn");
        sourceDataSet.Tables[0].Columns.Add("bk_org_acn");
        sourceDataSet.Tables[0].Columns.Add("bk_org_bpay_account");
        sourceDataSet.Tables[0].Columns.Add("bk_org_addr");
        sourceDataSet.Tables[0].Columns.Add("bk_org_addr_tabbedx1");
        sourceDataSet.Tables[0].Columns.Add("bk_org_phone");
        sourceDataSet.Tables[0].Columns.Add("bk_org_office_fax");
        sourceDataSet.Tables[0].Columns.Add("bk_org_web");
        sourceDataSet.Tables[0].Columns.Add("bk_org_email");


        Booking nextBooking = invoice.Booking.Patient == null ? null : BookingDB.GetNextAfterToday(invoice.Booking.Patient.PatientID, false);
        string nextBookingText = nextBooking == null ? " " : "Next appointment: " + Environment.NewLine + nextBooking.DateStart.ToString("h:mm") + (nextBooking.DateStart.Hour < 12 ? "am" : "pm") + " " + nextBooking.DateStart.ToString("d MMMM, yyyy") + " at " + nextBooking.Organisation.Name;


        RegisterStaff registerStaff = RegisterStaffDB.GetByStaffIDAndOrganisationID(invoice.Booking.Provider.StaffID, invoice.Booking.Organisation.OrganisationID);
        string providerNbrThisOrg   = registerStaff == null ? string.Empty : registerStaff.ProviderNumber;
        string providerNbrThisStaff = invoice.Booking.Provider.ProviderNumber;


        int    debtorEntityID = -1;
        string debtorName = "No debtor name found";
        bool?  debtorIsPatient = null;
        if (invoice.PayerOrganisation != null)
        {
            debtorEntityID = invoice.PayerOrganisation.EntityID;
            debtorName = invoice.PayerOrganisation.Name;
            debtorIsPatient = false;
        }
        else if (invoice.PayerPatient != null)
        {
            debtorEntityID = invoice.PayerPatient.Person.EntityID;
            debtorName = invoice.PayerPatient.Person.FullnameWithoutMiddlename;
            debtorIsPatient = true;
        }
        else
        {
            if (invoice.Booking != null && invoice.Booking.Patient != null)
            {
                debtorEntityID = invoice.Booking.Patient.Person.EntityID;
                debtorName = invoice.Booking.Patient.Person.FullnameWithoutMiddlename;
                debtorIsPatient = true;
            }
            else // no debtor for some cash invoices
                ;
        }

        string patientAddressText, patientAddressTabbedText;
        string orgAddressText    , orgAddressTabbedText      , orgPhoneText , orgFaxText, orgWebText, orgEmailText;
        string debtorAddressText , debtorAddressTabbedText;
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact patientAddress   = invoice.Booking.Patient == null ? null : ContactDB.GetFirstByEntityID(1, invoice.Booking.Patient.Person.EntityID);
            patientAddressText       = patientAddress != null ? patientAddress.GetFormattedAddress("No address found")    : "No address found";
            patientAddressTabbedText = patientAddress != null ? patientAddress.GetFormattedAddress("No address found", 1) : "No address found";

            Contact orgAddress = ContactDB.GetFirstByEntityID(1,     invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID);
            Contact orgPhone   = ContactDB.GetFirstByEntityID(null,  invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID, "30,33,34");
            Contact orgFax     = ContactDB.GetFirstByEntityID(-1,    invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID, 29);
            Contact orgWeb     = ContactDB.GetFirstByEntityID(-1,    invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID, 28);
            Contact orgEmail   = ContactDB.GetFirstByEntityID(-1,    invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID, 27);

            orgAddressText            = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found");
            orgAddressTabbedText      = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found", 1);
            orgPhoneText              = orgPhone       == null    ? "No phone number found" : orgPhone.GetFormattedPhoneNumber("No phone number found");
            orgFaxText                = orgFax         == null    ? "No fax number found"   : orgFax.GetFormattedPhoneNumber("No fax number found");
            orgWebText                = orgWeb         == null    ? "No website found"      : orgWeb.AddrLine1;
            orgEmailText              = orgEmail       == null    ? "No email found"        : orgEmail.AddrLine1;

            Contact debtorAddress     = debtorEntityID == -1 ? null : ContactDB.GetFirstByEntityID(1, debtorEntityID);
            debtorAddressText         = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found")    : "No address found";
            debtorAddressTabbedText   = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found", 1) : "No address found";

            // if AC && debtor is PT && no address found for PT .. then use fac address
            if (!isClinicInvoice && debtorIsPatient != null && debtorIsPatient.Value == true && debtorAddress == null)
            {
                debtorAddressText = orgAddressText;
                debtorAddressTabbedText = orgAddressTabbedText;
            }
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus patientAddress = invoice.Booking.Patient == null ? null : ContactAusDB.GetFirstByEntityID(1, invoice.Booking.Patient.Person.EntityID);
            patientAddressText        = patientAddress != null ? patientAddress.GetFormattedAddress("No address found")    : "No address found";
            patientAddressTabbedText  = patientAddress != null ? patientAddress.GetFormattedAddress("No address found", 1) : "No address found";

            ContactAus orgAddressAus  = ContactAusDB.GetFirstByEntityID(1,     invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID);
            ContactAus orgPhoneAus    = ContactAusDB.GetFirstByEntityID(null,  invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID, "30,33,34");
            ContactAus orgFaxAus      = ContactAusDB.GetFirstByEntityID(-1,    invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID, 29);
            ContactAus orgWebAus      = ContactAusDB.GetFirstByEntityID(-1,    invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID, 28);
            ContactAus orgEmailAus    = ContactAusDB.GetFirstByEntityID(-1,    invoice.Booking.Organisation != null ? invoice.Booking.Organisation.EntityID : site.EntityID, 27);

            orgAddressText            = orgAddressAus      == null ? "No address found"      : orgAddressAus.GetFormattedAddress("No address found");
            orgAddressTabbedText      = orgAddressAus      == null ? "No address found"      : orgAddressAus.GetFormattedAddress("No address found", 1);
            orgPhoneText              = orgPhoneAus        == null ? "No phone number found" : orgPhoneAus.GetFormattedPhoneNumber("No phone number found");
            orgFaxText                = orgFaxAus          == null ? "No fax number found"   : orgFaxAus.GetFormattedPhoneNumber("No fax number found");
            orgWebText                = orgWebAus          == null ? "No website found"      : orgWebAus.AddrLine1;
            orgEmailText              = orgEmailAus        == null ? "No email found"        : orgEmailAus.AddrLine1;

            ContactAus debtorAddress  = debtorEntityID == -1 ? null : ContactAusDB.GetFirstByEntityID(1, debtorEntityID);
            debtorAddressText         = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found")    : "No address found";
            debtorAddressTabbedText   = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found", 1) : "No address found";

            // if AC && debtor is PT && no address found for PT .. then use fac address
            if (!isClinicInvoice && debtorIsPatient != null && debtorIsPatient.Value == true && debtorAddress == null)
            {
                debtorAddressText = orgAddressText;
                debtorAddressTabbedText = orgAddressTabbedText;
            }
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());




        // referrer info

        Patient patient = null;
        if (invoice.Booking != null && invoice.Booking.Patient != null)
            patient = invoice.Booking.Patient;
        else if (invoice.PayerPatient != null)
            patient = invoice.PayerPatient;

        RegisterReferrer[] orgRefs     = PatientReferrerDB.GetActiveEPCReferrersOf(patient == null ? -1 : patient.PatientID);
        RegisterReferrer   regReferrer = orgRefs.Length == 0 ? null : orgRefs[0];


        // get hc & epc info

        HealthCard[] medicareCards = patient == null ? new HealthCard[0] : HealthCardDB.GetAllByPatientID(patient.PatientID, true, -1);
        HealthCard[] dvaCards      = patient == null ? new HealthCard[0] : HealthCardDB.GetAllByPatientID(patient.PatientID, true, -2);
        HealthCard   medicareCard  = medicareCards.Length == 0 ? null : medicareCards[0];
        HealthCard   dvaCard       = dvaCards.Length      == 0 ? null : dvaCards[0];

        HealthCard               activeHcCard                  = (medicareCard != null && medicareCard.IsActive) ? medicareCard : ((dvaCard != null && dvaCard.IsActive) ? dvaCard : null);
        bool                     hasEPC                        = activeHcCard  != null && activeHcCard.DateReferralSigned != DateTime.MinValue;
        HealthCardEPCRemaining[] epcsRemaining                 = !hasEPC ? new HealthCardEPCRemaining[] { } : HealthCardEPCRemainingDB.GetByHealthCardID(activeHcCard.HealthCardID);
        int                      MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);
        int                      totalServicesAllowedLeft      = !hasEPC ? 0 : MedicareMaxNbrServicesPerYear - (int)InvoiceDB.GetMedicareCountByPatientAndDateRange(patient == null ? -1 : patient.PatientID, new DateTime(DateTime.Now.Year , 1, 1), new DateTime(DateTime.Now.Year , 12, 31));

        int totalEpcsRemaining = 0;
        for (int j = 0; j < epcsRemaining.Length; j++)
            totalEpcsRemaining += epcsRemaining[j].NumServicesRemaining;

        DateTime referralSignedDate = !hasEPC ? DateTime.MinValue : activeHcCard.DateReferralSigned.Date;
        DateTime hcExpiredDate      = !hasEPC ? DateTime.MinValue : referralSignedDate.AddYears(1);
        bool     isExpired          = !hasEPC ? true              : hcExpiredDate <= DateTime.Today;

        int nServicesLeft = 0;
        if (activeHcCard != null && DateTime.Today >= referralSignedDate.Date && DateTime.Today < hcExpiredDate.Date)
            nServicesLeft = totalEpcsRemaining;
        if (activeHcCard != null && totalServicesAllowedLeft < nServicesLeft)
            nServicesLeft = totalServicesAllowedLeft;

        bool has_valid_epc = hasEPC && !isExpired && (activeHcCard.Organisation.OrganisationID == -2 || (activeHcCard.Organisation.OrganisationID == -1 && nServicesLeft > 0));

        int      epc_count_remaining = hasEPC && activeHcCard.Organisation.OrganisationID == -1 ? nServicesLeft : 0;
        DateTime epc_expire_date     = hasEPC                                                   ? hcExpiredDate : DateTime.MinValue;



        sourceDataSet.Tables[0].Rows.Add(
            DateTime.Now.ToString("d MMMM, yyyy"),
            invoice.InvoiceID.ToString() + (invoice.HealthcareClaimNumber.Trim().Length > 0 ? "  (Claim No. " + invoice.HealthcareClaimNumber.Trim() + ")" : ""),
            invoice.InvoiceDateAdded.ToString("d MMMM, yyyy"),
            debtorName,
            debtorAddressText,
            debtorAddressTabbedText,
            invoice.Booking.Patient == null  ? string.Empty : invoice.Booking.Patient.Person.FullnameWithoutMiddlename,
            patientAddressText,
            patientAddressTabbedText,
            invoice.Booking.Provider.Person.FullnameWithoutMiddlename,
            isClinicInvoice ? providerNbrThisOrg : providerNbrThisStaff,
            invoice.Booking.DateStart.ToString("d MMMM, yyyy"),
            nextBookingText,
            invoice.Booking.Patient != null && invoice.Booking.Patient.IsCompany ? "Purchase Order Nbr. " + invoice.Booking.SterilisationCode : " ",

            invoice.Booking.Patient == null ? "No patient found" : invoice.Booking.Patient.Person.FullnameWithTitleWithoutMiddlename,
            patientAddressText,
            patientAddressTabbedText,
            //patientAddress == null || patientAddress.AddrLine1.Length == 0  ? "No address found" : patientAddress.AddrLine1,
            //patientAddress == null || patientAddress.AddrLine2.Length == 0  ? " " : patientAddress.AddrLine2,
            //patientAddress == null || patientAddress.AddressChannel == null ? " " : (patientAddress.AddressChannel.AddressChannelID == 1 ? " " : patientAddress.AddressChannel.DisplayName),
            //patientAddress == null || patientAddress.Suburb  == null        ? " " : patientAddress.Suburb.Name,
            //patientAddress == null || patientAddress.Suburb  == null        ? " " : patientAddress.Suburb.Postcode,
            //patientAddress == null || patientAddress.Country == null        ? " " : patientAddress.Country.Descr


            activeHcCard == null ? "No hc card found" : activeHcCard.CardNbr + " - " + activeHcCard.CardFamilyMemberNbr,
            activeHcCard == null ? " " : (activeHcCard.CardName.Length > 0 ? activeHcCard.CardName : (patient != null ? patient.Person.FullnameWithoutMiddlename : " ")),
            activeHcCard == null ? (object)" " : activeHcCard.DateReferralSigned.ToString("d MMMM, yyyy"),
            epc_expire_date == DateTime.MinValue ? (object)" " : epc_expire_date.ToString("d MMMM, yyyy"),
            epc_expire_date == DateTime.MinValue ? (object)" " : epc_count_remaining,

            regReferrer == null ? "No referrer found" : regReferrer.Referrer.Person.FullnameWithTitleWithoutMiddlename,
            regReferrer == null ? "No referrer found" : (regReferrer.Referrer.Person.Title.ID == 0 ? "" : regReferrer.Referrer.Person.Title.Descr),
            regReferrer == null ? " " : regReferrer.Referrer.Person.Firstname,
            regReferrer == null ? " " : regReferrer.Referrer.Person.Middlename,
            regReferrer == null ? " " : regReferrer.Referrer.Person.Surname,


            invoice.Booking.Organisation.Name,
            invoice.Booking.Organisation.Abn,
            invoice.Booking.Organisation.Acn,
            invoice.Booking.Organisation.BpayAccount,
            orgAddressText,
            orgAddressTabbedText,
            orgPhoneText,
            orgFaxText,
            orgWebText,
            orgEmailText
            );


        // create table data to populate from invoice lines

        string[,] tblInfo;

        if (isMedicareOrDVAInvoice)
        {
            tblInfo = new string[lines.Length + 2, 4];

            int row = 0;
            for (int i = 0; i < lines.Length; i++, row++)
            {
                string mcCode  = lines[i].Offering.MedicareCompanyCode.Length > 0 ? (Environment.NewLine + "Medicare Code: " + lines[i].Offering.MedicareCompanyCode) : string.Empty;
                string dvaCode = lines[i].Offering.DvaCompanyCode.Length      > 0 ? (Environment.NewLine + "DVA Code: "      + lines[i].Offering.DvaCompanyCode)      : string.Empty;
                string code    = invoice.PayerOrganisation.OrganisationID == -1 ? mcCode : dvaCode;

                tblInfo[row, 0] = lines[i].Offering.Name + code + Environment.NewLine + (invoiceGapPayments ? "Bulk billed to Medicare" : "Normally $" + lines[i].Offering.DefaultPrice.ToString() + " but bulk billed to Medicare");
                tblInfo[row, 1] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString();
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = "0.00";
            }

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = string.Empty;
            row++;

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = "<align=right><b>Balance Due: </b></align>";
            tblInfo[row, 3] = "<b>0.00</b>";
            row++;
        }
        else if (isTACInvoice)
        {
            tblInfo = new string[lines.Length + 2, 4];

            int row = 0;
            for (int i = 0; i < lines.Length; i++, row++)
            {
                string insCode = lines[i].Offering.TacCompanyCode.Length      > 0 ? (Environment.NewLine + "Ins. Code: "     + lines[i].Offering.TacCompanyCode)      : string.Empty;

                tblInfo[row, 0] = lines[i].Offering.Name + insCode;
                tblInfo[row, 1] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString();
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = lines[i].Price.ToString();
            }

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = string.Empty;
            row++;

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = "<align=right><b>Balance Due: </b></align>";
            tblInfo[row, 3] = "<b>" + invoice.TotalDue.ToString() + "</b>";
            row++;
        }
        else if (isClinicInvoice)
        {
            tblInfo = new string[lines.Length + receipts.Length + creditNotes.Length + 4, 4];

            int row = 0;
            for (int i = 0; i < lines.Length; i++, row++)
            {
                string mcCode  = lines[i].Offering.MedicareCompanyCode.Length > 0 ? (Environment.NewLine + "Medicare Code: " + lines[i].Offering.MedicareCompanyCode) : string.Empty;
                string dvaCode = lines[i].Offering.DvaCompanyCode.Length      > 0 ? (Environment.NewLine + "DVA Code: "      + lines[i].Offering.DvaCompanyCode)      : string.Empty;
                string insCode = lines[i].Offering.TacCompanyCode.Length      > 0 ? (Environment.NewLine + "Ins. Code: "     + lines[i].Offering.TacCompanyCode)      : string.Empty;

                tblInfo[row, 0] = lines[i].Offering.Name + mcCode + insCode + (i == lines.Length - 1 && message.Length > 0 ? Environment.NewLine + message : "");
                tblInfo[row, 1] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString();
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = lines[i].Price.ToString();
            }
            for (int i = 0; i < receipts.Length; i++, row++)
            {
                tblInfo[row, 0] = "Receipt " + receipts[i].ReceiptID + " - " + receipts[i].ReceiptPaymentType.Descr;
                tblInfo[row, 1] = "1";
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = receipts[i].Total.ToString();
            }
            for (int i = 0; i < creditNotes.Length; i++, row++)
            {
                tblInfo[row, 0] = "Adjustment Note  " + creditNotes[i].CreditNoteID;
                tblInfo[row, 1] = "1";
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = creditNotes[i].Total.ToString();
            }

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = string.Empty;
            row++;

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = "<align=right><b>GST: </b></align>";
            tblInfo[row, 3] = invoice.Gst.ToString();
            row++;

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = "<align=right><b>Payments & Adjustments: </b></align>";
            tblInfo[row, 3] = (invoice.ReceiptsTotal + invoice.CreditNotesTotal).ToString();
            row++;

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = "<align=right><b>Balance Due: </b></align>";
            tblInfo[row, 3] = "<b>" + invoice.TotalDue.ToString() + "</b>";
            row++;
        }
        else // isAgedCareInvoice (private)
        {
            tblInfo = new string[lines.Length + receipts.Length + creditNotes.Length + 4, 6];

            int row = 0;
            for (int i = 0; i < lines.Length; i++, row++)
            {
                /*
                    0 = i (row nbr)
                    1 = room (addr1 of patient)
                    2 = patient name
                    3 = offering
                    4 = qty
                    5 = space
                    6 = price
                */

                string ptBedroom = PatientsContactCacheDB.GetBedroom(acPtBedroomHash, lines[i].Patient.Person.EntityID);


                tblInfo[row, 0] = (row + 1).ToString();
                tblInfo[row, 1] = ptBedroom == null ? string.Empty : ptBedroom;
                tblInfo[row, 2] = lines[i].Patient.Person.FullnameWithoutMiddlename;
                tblInfo[row, 3] = (lines[i].Offering == null ? "" : lines[i].Offering.Name) + (i == lines.Length - 1 && message.Length > 0 ? Environment.NewLine + Environment.NewLine + message : "");
                tblInfo[row, 4] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString(); ;
                tblInfo[row, 5] = lines[i].Price.ToString();
            }
            for (int i = 0; i < receipts.Length; i++, row++)
            {
                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = "Receipt " + receipts[i].ReceiptID + " - " + receipts[i].ReceiptPaymentType.Descr;
                tblInfo[row, 4] = string.Empty;
                tblInfo[row, 5] = receipts[i].Total.ToString();
            }
            for (int i = 0; i < creditNotes.Length; i++, row++)
            {
                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = "Adjustment Note  " + creditNotes[i].CreditNoteID;
                tblInfo[row, 4] = string.Empty;
                tblInfo[row, 5] = creditNotes[i].Total.ToString();
            }

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = string.Empty;
            tblInfo[row, 4] = string.Empty;
            tblInfo[row, 5] = string.Empty;
            row++;

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = "<align=right><b>GST: </b></align>";
            tblInfo[row, 4] = string.Empty;
            tblInfo[row, 5] = invoice.Gst.ToString();
            row++;

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = "<align=right><b>Payments & Adjustments: </b></align>";
            tblInfo[row, 4] = string.Empty;
            tblInfo[row, 5] = (invoice.ReceiptsTotal + invoice.CreditNotesTotal).ToString();
            row++;

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = "<align=right><b>Balance Due: </b></align>";
            tblInfo[row, 4] = string.Empty;
            tblInfo[row, 5] = "<b>" + invoice.TotalDue.ToString() + "</b>";
            row++;
        }


        // merge

        


        string errorString = null;
        WordMailMerger.Merge(

            originalFile,
            outputFile,
            sourceDataSet,

            tblInfo,
            1,
            true,

            true,
            null,
            true,
            ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["EziDebit_Enabled"].Value != "1" ? null : @"https://portal.mediclinic.com.au/InvoicePaymentV2.aspx?id=" + Invoice.EncodeInvoiceHash(invoice.InvoiceID, System.Web.HttpContext.Current.Session["DB"].ToString()),
            out errorString);

        if (errorString != string.Empty)
            throw new Exception(errorString);
    }

    public static void GenerateAllInvoicesAndTypes(int[] invoiceIDs, HttpResponse response)
    {
        Site site = SiteDB.GetByID(Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteID"]));

        bool invoiceGapPayments = Convert.ToInt32(SystemVariableDB.GetByDescr("InvoiceGapPayments").Value) == 1;

        Invoice[] invoices        = InvoiceDB.GetByIDs(invoiceIDs);
        Hashtable invoiceLineHash = InvoiceLineDB.GetBulkInvoiceLinesByInvoiceID(invoices);
        Hashtable receiptHash     = ReceiptDB.GetBulkReceiptsByInvoiceID(invoices);
        Hashtable creditNoteHash  = CreditNoteDB.GetBulkCreditNotesByInvoiceID(invoices);

        ArrayList entityIDsList = new ArrayList();
        foreach (Invoice invoice in invoices)
        {
            if (invoice.Booking != null && invoice.Booking.Patient != null)
                entityIDsList.Add(invoice.Booking.Patient.Person.EntityID);
            if (invoice.Booking != null && invoice.Booking.Organisation != null)
                entityIDsList.Add(invoice.Booking.Organisation.EntityID);
            if (invoice.PayerPatient != null)
                entityIDsList.Add(invoice.PayerPatient.Person.EntityID);
            if (invoice.PayerOrganisation != null)
                entityIDsList.Add(invoice.PayerOrganisation.EntityID);
        }
        entityIDsList.Add(site.EntityID);
        int[] entityIDs = (int[])entityIDsList.ToArray(typeof(int));

        Hashtable addressHash     = PatientsContactCacheDB.GetBullk("1",  null,  entityIDs, -1);
        Hashtable phoneHash       = PatientsContactCacheDB.GetBullk(null, "30,33,34",  entityIDs, -1);
        Hashtable faxHash         = PatientsContactCacheDB.GetBullk(null, "29",  entityIDs, -1);
        Hashtable webHash         = PatientsContactCacheDB.GetBullk(null, "28",  entityIDs, -1);
        Hashtable emailHash       = PatientsContactCacheDB.GetBullk(null, "27",  entityIDs, -1);
        Hashtable acPtBedroomHash = PatientsContactCacheDB.GetBullk(null, "166", entityIDs, -1);

        object siteAddressList = addressHash[site.EntityID] == null ? null : addressHash[site.EntityID];
        object sitePhoneList   = phoneHash[site.EntityID]   == null ? null : phoneHash[site.EntityID];
        object siteFaxList     = faxHash[site.EntityID]     == null ? null : faxHash[site.EntityID];
        object siteWebList     = webHash[site.EntityID]     == null ? null : webHash[site.EntityID];
        object siteEmailList   = emailHash[site.EntityID]   == null ? null : emailHash[site.EntityID];

        object siteAddress = siteAddressList == null ? null : (Utilities.GetAddressType().ToString() == "ContactAus" ? (object)((ContactAus[])siteAddressList)[0] : (object)((Contact[])siteAddressList)[0]);
        object sitePhone   = sitePhoneList   == null ? null : (Utilities.GetAddressType().ToString() == "ContactAus" ? (object)((ContactAus[])sitePhoneList)[0]   : (object)((Contact[])sitePhoneList)[0]);
        object siteFax     = siteFaxList     == null ? null : (Utilities.GetAddressType().ToString() == "ContactAus" ? (object)((ContactAus[])siteFaxList)[0]     : (object)((Contact[])siteFaxList)[0]);
        object siteWeb     = siteWebList     == null ? null : (Utilities.GetAddressType().ToString() == "ContactAus" ? (object)((ContactAus[])siteWebList)[0]     : (object)((Contact[])siteWebList)[0]);
        object siteEmail   = siteEmailList   == null ? null : (Utilities.GetAddressType().ToString() == "ContactAus" ? (object)((ContactAus[])siteEmailList)[0]   : (object)((Contact[])siteEmailList)[0]);



        string tmpLettersDirectory                 = FileHelper.GetTempDirectoryName(Letter.GetTempLettersDirectory());
        Directory.CreateDirectory(tmpLettersDirectory);
        string tmpLettersDirectory2                = FileHelper.GetTempDirectoryName(Letter.GetTempLettersDirectory());
        Directory.CreateDirectory(tmpLettersDirectory2);
        string originalFile_BookingClinicInvoices  = Letter.GetLettersDirectory() + @"InvoiceTemplate.docx";
        string originalFile_BookingACInvoices      = Letter.GetLettersDirectory() + @"InvoiceTemplateAC.docx";
        string originalFile_PrivateInvoices        = Letter.GetLettersDirectory() + @"PrivateInvoiceTemplate.docx";


        ArrayList filePaths_BookingClinicInvoices = new ArrayList();
        ArrayList filePaths_BookingACInvoices     = new ArrayList();
        ArrayList filePaths_PrivateInvoices       = new ArrayList();

        foreach (Invoice invoice in invoices)
        {
            InvoiceLine[] lines       = invoiceLineHash[invoice.InvoiceID] == null ? new InvoiceLine[]{} : (InvoiceLine[])invoiceLineHash[invoice.InvoiceID];
            Receipt[]     receipts    = receiptHash[invoice.InvoiceID]     == null ? new Receipt[]{}     : (Receipt[])    receiptHash[invoice.InvoiceID];
            CreditNote[]  creditNotes = creditNoteHash[invoice.InvoiceID]  == null ? new CreditNote[]{}  : (CreditNote[]) creditNoteHash[invoice.InvoiceID];

            bool isClinicInvoice  = invoice.Site.SiteType.ID == 1;
            bool isBookingInvoice = invoice.Booking != null;

            string originalFile = !isBookingInvoice ? originalFile_PrivateInvoices : (isClinicInvoice ? originalFile_BookingClinicInvoices : originalFile_BookingACInvoices);
            string outputFile   = FileHelper.GetTempFileName(tmpLettersDirectory + "Invoice_" + invoice.InvoiceID + "." + System.IO.Path.GetExtension(originalFile));




            // 1. change address/phone section below to use hashes
            // 2. just use one table to see if it works
            // 3. if works .. send in multiple tables as an array .. prob just add to arraylist of arrays






            bool isMedicareInvoice  = invoice.PayerOrganisation != null && (invoice.PayerOrganisation.OrganisationID == -1 || invoice.PayerOrganisation.OrganisationID == -2);
            bool isTACInvoice       = invoice.PayerOrganisation != null && invoice.PayerOrganisation.OrganisationType.OrganisationTypeID == 150;


            // was rejected when
            // - pt pays invoice, and
            // - booking has a medicare/dva invoice that has been rejected
            //      ie  payer org = -1/-2 and single adj/cr note equal to total, and single inv line with same offering as this one single inv line)
            //
            bool wasRejectedMedicareInvoice = false;
            if (!isMedicareInvoice && lines.Length == 1 && invoice.Booking != null)
            {
                foreach (Invoice curInvoice in InvoiceDB.GetByBookingID(invoice.Booking.BookingID, false))
                {
                    // make sure is medicare/dva booking
                    if (curInvoice.PayerOrganisation == null || (curInvoice.PayerOrganisation.OrganisationID != -1 && curInvoice.PayerOrganisation.OrganisationID != -2))
                        continue;

                    // make sure is wiped (paid, nothing due, no receipts, credit notes == total of bill)
                    if (!curInvoice.IsPaID || curInvoice.TotalDue != 0 || curInvoice.ReceiptsTotal != 0 || curInvoice.CreditNotesTotal != curInvoice.Total)
                        continue;

                    // if only one invoice line on both this and the non-mediare/dva invoice and same offering
                    InvoiceLine[] curInvLines = InvoiceLineDB.GetByInvoiceID(curInvoice.InvoiceID);
                    if (curInvLines.Length == 1                                            && 
                        lines.Length       == 1                                            && 
                        curInvLines[0].Offering.OfferingID == lines[0].Offering.OfferingID &&
                        curInvLines[0].Price               == lines[0].Price               &&
                        curInvLines[0].Quantity            == lines[0].Quantity            &&
                        curInvLines[0].Tax                 == lines[0].Tax)
                            wasRejectedMedicareInvoice = true;
                }
            }

            string message = invoice.Message.Length > 0 ? invoice.Message : (wasRejectedMedicareInvoice ? "*Claim rejected by Medicare" : "");




            //
            // merge the invoice fields
            //

            // create dataset of merge fields from invoice
            System.Data.DataSet sourceDataSet = new System.Data.DataSet();
            sourceDataSet.Tables.Add("MergeIt");


            if (invoice.Booking != null)
            {
                sourceDataSet.Tables[0].Columns.Add("curr_date");
                sourceDataSet.Tables[0].Columns.Add("inv_nbr");
                sourceDataSet.Tables[0].Columns.Add("inv_date");
                sourceDataSet.Tables[0].Columns.Add("inv_debtor_name");
                sourceDataSet.Tables[0].Columns.Add("inv_debtor_addr");
                sourceDataSet.Tables[0].Columns.Add("inv_debtor_addr_tabbedx1");
                sourceDataSet.Tables[0].Columns.Add("bk_pt_fullname");
                sourceDataSet.Tables[0].Columns.Add("bk_pt_addr");
                sourceDataSet.Tables[0].Columns.Add("bk_pt_addr_tabbedx1");
                sourceDataSet.Tables[0].Columns.Add("bk_prov_fullname");
                sourceDataSet.Tables[0].Columns.Add("bk_prov_number");
                sourceDataSet.Tables[0].Columns.Add("bk_date");
                sourceDataSet.Tables[0].Columns.Add("bk_next_info");
                sourceDataSet.Tables[0].Columns.Add("bk_purchase_order_nbr");

                sourceDataSet.Tables[0].Columns.Add("pt_name");           // depricated as same as bk_pt_fullname
                sourceDataSet.Tables[0].Columns.Add("pt_addr");           // depricated as same as bk_addr
                sourceDataSet.Tables[0].Columns.Add("pt_addr_tabbedx1");  // depricated as same as bk_addr_tabbedx1
                //sourceDataSet.Tables[0].Columns.Add("pt_addr_line1");
                //sourceDataSet.Tables[0].Columns.Add("pt_addr_line2");
                //sourceDataSet.Tables[0].Columns.Add("pt_addr_street");
                //sourceDataSet.Tables[0].Columns.Add("pt_addr_suburb");
                //sourceDataSet.Tables[0].Columns.Add("pt_addr_postcode");
                //sourceDataSet.Tables[0].Columns.Add("pt_addr_country");

                sourceDataSet.Tables[0].Columns.Add("bk_org_name");
                sourceDataSet.Tables[0].Columns.Add("bk_org_abn");
                sourceDataSet.Tables[0].Columns.Add("bk_org_acn");
                sourceDataSet.Tables[0].Columns.Add("bk_org_bpay_account");
                sourceDataSet.Tables[0].Columns.Add("bk_org_addr");
                sourceDataSet.Tables[0].Columns.Add("bk_org_addr_tabbedx1");
                sourceDataSet.Tables[0].Columns.Add("bk_org_phone");
                sourceDataSet.Tables[0].Columns.Add("bk_org_office_fax");
                sourceDataSet.Tables[0].Columns.Add("bk_org_web");
                sourceDataSet.Tables[0].Columns.Add("bk_org_email");
            }
            else // private non-booking invoice
            {
                sourceDataSet.Tables[0].Columns.Add("curr_date");
                sourceDataSet.Tables[0].Columns.Add("inv_nbr");
                sourceDataSet.Tables[0].Columns.Add("inv_date");
                sourceDataSet.Tables[0].Columns.Add("pt_fullname");
                sourceDataSet.Tables[0].Columns.Add("org_name");

                sourceDataSet.Tables[0].Columns.Add("org_abn");
                sourceDataSet.Tables[0].Columns.Add("org_acn");
                sourceDataSet.Tables[0].Columns.Add("org_bpay_account");
                sourceDataSet.Tables[0].Columns.Add("org_addr");
                sourceDataSet.Tables[0].Columns.Add("org_addr_tabbedx1");
                sourceDataSet.Tables[0].Columns.Add("org_phone");
                sourceDataSet.Tables[0].Columns.Add("org_office_fax");
                sourceDataSet.Tables[0].Columns.Add("org_web");
                sourceDataSet.Tables[0].Columns.Add("org_email");
            }


            Booking nextBooking = invoice.Booking == null || invoice.Booking.Patient == null ? null : BookingDB.GetNextAfterToday(invoice.Booking.Patient.PatientID, false);
            string nextBookingText = nextBooking == null ? " " : "Next appointment: " + Environment.NewLine + nextBooking.DateStart.ToString("h:mm") + (nextBooking.DateStart.Hour < 12 ? "am" : "pm") + " " + nextBooking.DateStart.ToString("d MMMM, yyyy") + " at " + nextBooking.Organisation.Name;


            RegisterStaff registerStaff = invoice.Booking == null ? null         : RegisterStaffDB.GetByStaffIDAndOrganisationID(invoice.Booking.Provider.StaffID, invoice.Booking.Organisation.OrganisationID);
            string providerNbrThisOrg   = registerStaff   == null ? string.Empty : registerStaff.ProviderNumber;
            string providerNbrThisStaff = invoice.Booking == null ? string.Empty : invoice.Booking.Provider.ProviderNumber;


            int    debtorEntityID = -1;
            string debtorName = "No debtor name found";
            bool?  debtorIsPatient = null;
            if (invoice.PayerOrganisation != null)
            {
                debtorEntityID = invoice.PayerOrganisation.EntityID;
                debtorName = invoice.PayerOrganisation.Name;
                debtorIsPatient = false;
            }
            else if (invoice.PayerPatient != null)
            {
                debtorEntityID = invoice.PayerPatient.Person.EntityID;
                debtorName = invoice.PayerPatient.Person.FullnameWithoutMiddlename;
                debtorIsPatient = true;
            }
            else
            {
                if (invoice.Booking != null && invoice.Booking.Patient != null)
                {
                    debtorEntityID = invoice.Booking.Patient.Person.EntityID;
                    debtorName = invoice.Booking.Patient.Person.FullnameWithoutMiddlename;
                    debtorIsPatient = true;
                }
                else // no debtor for some cash invoices
                    ;
            }

            string patientAddressText, patientAddressTabbedText;
            string orgAddressText    , orgAddressTabbedText      , orgPhoneText , orgFaxText, orgWebText, orgEmailText;
            string debtorAddressText , debtorAddressTabbedText;
            if (Utilities.GetAddressType().ToString() == "Contact")
            {
                Contact patientAddress = invoice.Booking == null || invoice.Booking.Patient == null || addressHash[invoice.Booking.Organisation.EntityID] == null ? null : ((Contact[])addressHash[invoice.Booking.Organisation.EntityID])[0];
                patientAddressText       = patientAddress != null ? patientAddress.GetFormattedAddress("No address found")    : "No address found";
                patientAddressTabbedText = patientAddress != null ? patientAddress.GetFormattedAddress("No address found", 1) : "No address found";

                Contact orgAddress = invoice.Booking == null || invoice.Booking.Organisation == null || addressHash[invoice.Booking.Organisation.EntityID] == null ? (Contact)siteAddress : ((Contact[])addressHash[invoice.Booking.Organisation.EntityID])[0];
                Contact orgPhone   = invoice.Booking == null || invoice.Booking.Organisation == null || phoneHash  [invoice.Booking.Organisation.EntityID] == null ? (Contact)sitePhone   : ((Contact[])phoneHash  [invoice.Booking.Organisation.EntityID])[0];
                Contact orgFax     = invoice.Booking == null || invoice.Booking.Organisation == null || faxHash    [invoice.Booking.Organisation.EntityID] == null ? (Contact)siteFax     : ((Contact[])faxHash    [invoice.Booking.Organisation.EntityID])[0];
                Contact orgWeb     = invoice.Booking == null || invoice.Booking.Organisation == null || webHash    [invoice.Booking.Organisation.EntityID] == null ? (Contact)siteWeb     : ((Contact[])webHash    [invoice.Booking.Organisation.EntityID])[0];
                Contact orgEmail   = invoice.Booking == null || invoice.Booking.Organisation == null || emailHash  [invoice.Booking.Organisation.EntityID] == null ? (Contact)siteEmail   : ((Contact[])emailHash  [invoice.Booking.Organisation.EntityID])[0];

                orgAddressText            = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found");
                orgAddressTabbedText      = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found", 1);
                orgPhoneText              = orgPhone       == null    ? "No phone number found" : orgPhone.GetFormattedPhoneNumber("No phone number found");
                orgFaxText                = orgFax         == null    ? "No fax number found"   : orgFax.GetFormattedPhoneNumber("No fax number found");
                orgWebText                = orgWeb         == null    ? "No website found"      : orgWeb.AddrLine1;
                orgEmailText              = orgEmail       == null    ? "No email found"        : orgEmail.AddrLine1;

                Contact debtorAddress     = debtorEntityID == -1 || addressHash[debtorEntityID] == null ? null : ((Contact[])addressHash[debtorEntityID])[0];
                debtorAddressText         = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found")    : "No address found";
                debtorAddressTabbedText   = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found", 1) : "No address found";

                // if AC && debtor is PT && no address found for PT .. then use fac address
                if (!isClinicInvoice && debtorIsPatient != null && debtorIsPatient.Value == true && debtorAddress == null)
                {
                    debtorAddressText       = orgAddressText;
                    debtorAddressTabbedText = orgAddressTabbedText;
                }
            }
            else if (Utilities.GetAddressType().ToString() == "ContactAus")
            {
                ContactAus patientAddress = invoice.Booking == null || invoice.Booking.Patient == null || addressHash[invoice.Booking.Organisation.EntityID] == null ? null : ((ContactAus[])addressHash[invoice.Booking.Organisation.EntityID])[0];
                patientAddressText       = patientAddress != null ? patientAddress.GetFormattedAddress("No address found")    : "No address found";
                patientAddressTabbedText = patientAddress != null ? patientAddress.GetFormattedAddress("No address found", 1) : "No address found";

                ContactAus orgAddress = invoice.Booking == null || invoice.Booking.Organisation == null || addressHash[invoice.Booking.Organisation.EntityID] == null ? (ContactAus)siteAddress : ((ContactAus[])addressHash[invoice.Booking.Organisation.EntityID])[0];
                ContactAus orgPhone   = invoice.Booking == null || invoice.Booking.Organisation == null || phoneHash  [invoice.Booking.Organisation.EntityID] == null ? (ContactAus)sitePhone   : ((ContactAus[])phoneHash  [invoice.Booking.Organisation.EntityID])[0];
                ContactAus orgFax     = invoice.Booking == null || invoice.Booking.Organisation == null || faxHash    [invoice.Booking.Organisation.EntityID] == null ? (ContactAus)siteFax     : ((ContactAus[])faxHash    [invoice.Booking.Organisation.EntityID])[0];
                ContactAus orgWeb     = invoice.Booking == null || invoice.Booking.Organisation == null || webHash    [invoice.Booking.Organisation.EntityID] == null ? (ContactAus)siteWeb     : ((ContactAus[])webHash    [invoice.Booking.Organisation.EntityID])[0];
                ContactAus orgEmail   = invoice.Booking == null || invoice.Booking.Organisation == null || emailHash  [invoice.Booking.Organisation.EntityID] == null ? (ContactAus)siteEmail   : ((ContactAus[])emailHash  [invoice.Booking.Organisation.EntityID])[0];

                orgAddressText            = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found");
                orgAddressTabbedText      = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found", 1);
                orgPhoneText              = orgPhone       == null    ? "No phone number found" : orgPhone.GetFormattedPhoneNumber("No phone number found");
                orgFaxText                = orgFax         == null    ? "No fax number found"   : orgFax.GetFormattedPhoneNumber("No fax number found");
                orgWebText                = orgWeb         == null    ? "No website found"      : orgWeb.AddrLine1;
                orgEmailText              = orgEmail       == null    ? "No email found"        : orgEmail.AddrLine1;

                ContactAus debtorAddress     = debtorEntityID == -1 || addressHash[debtorEntityID] == null ? null : ((ContactAus[])addressHash[debtorEntityID])[0];
                debtorAddressText         = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found")    : "No address found";
                debtorAddressTabbedText   = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found", 1) : "No address found";

                // if AC && debtor is PT && no address found for PT .. then use fac address
                if (!isClinicInvoice && debtorIsPatient != null && debtorIsPatient.Value == true && debtorAddress == null)
                {
                    debtorAddressText       = orgAddressText;
                    debtorAddressTabbedText = orgAddressTabbedText;
                }
            }
            else
                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());



            if (invoice.Booking != null)
            {
                string ptName = string.Empty;
                if (invoice.Booking != null && invoice.Booking.Patient != null)
                    ptName = invoice.Booking.Patient.Person.FullnameWithoutMiddlename;
                if (invoice.Booking == null && invoice.PayerPatient != null)
                    ptName = invoice.PayerPatient.Person.FullnameWithoutMiddlename;

                sourceDataSet.Tables[0].Rows.Add(
                    DateTime.Now.ToString("d MMMM, yyyy"),
                    invoice.InvoiceID.ToString() + (invoice.HealthcareClaimNumber.Trim().Length > 0 ? "  (Claim No. " + invoice.HealthcareClaimNumber.Trim() + ")" : ""),
                    invoice.InvoiceDateAdded.ToString("d MMMM, yyyy"),
                    debtorName,
                    debtorAddressText,
                    debtorAddressTabbedText,
                    ptName,
                    patientAddressText,
                    patientAddressTabbedText,
                    invoice.Booking == null ? string.Empty : invoice.Booking.Provider.Person.FullnameWithoutMiddlename,
                    isClinicInvoice ? providerNbrThisOrg : providerNbrThisStaff,
                    invoice.Booking == null ? string.Empty : invoice.Booking.DateStart.ToString("d MMMM, yyyy"),
                    nextBookingText,
                    invoice.Booking != null && invoice.Booking.Patient != null && invoice.Booking.Patient.IsCompany ? "Purchase Order Nbr. " + invoice.Booking.SterilisationCode : " ",

                    ptName,
                    patientAddressText,
                    patientAddressTabbedText,
                    //patientAddress == null || patientAddress.AddrLine1.Length == 0  ? "No address found" : patientAddress.AddrLine1,
                    //patientAddress == null || patientAddress.AddrLine2.Length == 0  ? " " : patientAddress.AddrLine2,
                    //patientAddress == null || patientAddress.AddressChannel == null ? " " : (patientAddress.AddressChannel.AddressChannelID == 1 ? " " : patientAddress.AddressChannel.DisplayName),
                    //patientAddress == null || patientAddress.Suburb  == null        ? " " : patientAddress.Suburb.Name,
                    //patientAddress == null || patientAddress.Suburb  == null        ? " " : patientAddress.Suburb.Postcode,
                    //patientAddress == null || patientAddress.Country == null        ? " " : patientAddress.Country.Descr

                    invoice.Booking == null ? invoice.NonBookinginvoiceOrganisation.Name : invoice.Booking.Organisation.Name,
                    invoice.Booking == null ? invoice.NonBookinginvoiceOrganisation.Abn : invoice.Booking.Organisation.Abn,
                    invoice.Booking == null ? invoice.NonBookinginvoiceOrganisation.Acn : invoice.Booking.Organisation.Acn,
                    invoice.Booking == null ? invoice.NonBookinginvoiceOrganisation.BpayAccount : invoice.Booking.Organisation.BpayAccount,
                    orgAddressText,
                    orgAddressTabbedText,
                    orgPhoneText,
                    orgFaxText,
                    orgWebText,
                    orgEmailText
                    );
            }
            else
            {
                sourceDataSet.Tables[0].Rows.Add(
                    DateTime.Now.ToString("d MMMM, yyyy"),
                    invoice.InvoiceID.ToString(),
                    invoice.InvoiceDateAdded.ToString("d MMMM, yyyy"),
                    invoice.PayerPatient == null ? "--" : invoice.PayerPatient.Person.FullnameWithTitleWithoutMiddlename,

                    invoice.NonBookinginvoiceOrganisation.Name,
                    invoice.NonBookinginvoiceOrganisation.Abn,
                    invoice.NonBookinginvoiceOrganisation.Acn,
                    invoice.NonBookinginvoiceOrganisation.BpayAccount,
                    orgAddressText,
                    orgAddressTabbedText,
                    orgPhoneText,
                    orgFaxText,
                    orgWebText,
                    orgEmailText
                    );
            }



            // create table data to populate from invoice lines

            string[,] tblInfo;

            if (isMedicareInvoice)
            {
                tblInfo = new string[lines.Length + 2, 4];

                int row = 0;
                for (int i = 0; i < lines.Length; i++, row++)
                {
                    tblInfo[row, 0] = lines[i].Offering.Name + (invoiceGapPayments ? " - bulk billed to Medicare" : " - normally $" + lines[i].Offering.DefaultPrice.ToString() + " but bulk billed to Medicare");
                    tblInfo[row, 1] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString();
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = "0.00";
                }

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = string.Empty;
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = "<align=right><b>Balance Due: </b></align>";
                tblInfo[row, 3] = "<b>0.00</b>";
                row++;
            }
            else if (isTACInvoice)
            {
                tblInfo = new string[lines.Length + 2, 4];

                int row = 0;
                for (int i = 0; i < lines.Length; i++, row++)
                {
                    tblInfo[row, 0] = lines[i].Offering.Name;
                    tblInfo[row, 1] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString();
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = lines[i].Price.ToString();
                }

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = string.Empty;
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = "<align=right><b>Balance Due: </b></align>";
                tblInfo[row, 3] = "<b>" + invoice.TotalDue.ToString() + "</b>";
                row++;
            }
            else if (invoice.Booking == null)  // private invoice
            {
                tblInfo = new string[lines.Length + receipts.Length + creditNotes.Length + 4, 4];

                int row = 0;
                for (int i = 0; i < lines.Length; i++, row++)
                {
                    tblInfo[row, 0] = lines[i].Offering.Name;
                    tblInfo[row, 1] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString();
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = lines[i].Price.ToString();
                }
                for (int i = 0; i < receipts.Length; i++, row++)
                {
                    tblInfo[row, 0] = "Receipt " + receipts[i].ReceiptID + " - " + receipts[i].ReceiptPaymentType.Descr;
                    tblInfo[row, 1] = "1";
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = receipts[i].Total.ToString();
                }
                for (int i = 0; i < creditNotes.Length; i++, row++)
                {
                    tblInfo[row, 0] = "Adjustment Note  " + creditNotes[i].CreditNoteID;
                    tblInfo[row, 1] = "1";
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = creditNotes[i].Total.ToString();
                }

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = string.Empty;
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = "<align=right><b>GST: </b></align>";
                tblInfo[row, 3] = invoice.Gst.ToString();
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = "<align=right><b>Payments & Adjustments: </b></align>";
                tblInfo[row, 3] = (invoice.ReceiptsTotal + invoice.CreditNotesTotal).ToString();
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = "<align=right><b>Balance Due: </b></align>";
                tblInfo[row, 3] = "<b>" + invoice.TotalDue.ToString() + "</b>";
                row++;
            }
            else if (isClinicInvoice)
            {
                tblInfo = new string[lines.Length + receipts.Length + creditNotes.Length + 4, 4];

                int row = 0;
                for (int i = 0; i < lines.Length; i++, row++)
                {
                    tblInfo[row, 0] = lines[i].Offering.Name + (i == lines.Length - 1 && message.Length > 0 ? Environment.NewLine + message : "");
                    tblInfo[row, 1] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString();
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = lines[i].Price.ToString();
                }
                for (int i = 0; i < receipts.Length; i++, row++)
                {
                    tblInfo[row, 0] = "Receipt " + receipts[i].ReceiptID + " - " + receipts[i].ReceiptPaymentType.Descr;
                    tblInfo[row, 1] = "1";
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = receipts[i].Total.ToString();
                }
                for (int i = 0; i < creditNotes.Length; i++, row++)
                {
                    tblInfo[row, 0] = "Adjustment Note  " + creditNotes[i].CreditNoteID;
                    tblInfo[row, 1] = "1";
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = creditNotes[i].Total.ToString();
                }

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = string.Empty;
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = "<align=right><b>GST: </b></align>";
                tblInfo[row, 3] = invoice.Gst.ToString();
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = "<align=right><b>Payments & Adjustments: </b></align>";
                tblInfo[row, 3] = (invoice.ReceiptsTotal + invoice.CreditNotesTotal).ToString();
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = "<align=right><b>Balance Due: </b></align>";
                tblInfo[row, 3] = "<b>" + invoice.TotalDue.ToString() + "</b>";
                row++;
            }
            else // isAgedCareInvoice (private)
            {
                tblInfo = new string[lines.Length + receipts.Length + creditNotes.Length + 4, 6];

                int row = 0;
                for (int i = 0; i < lines.Length; i++, row++)
                {

                    // 0 = i (row nbr)
                    // 1 = room (addr1 of patient)
                    // 2 = patient name
                    // 3 = offering
                    // 4 = qty
                    // 5 = space
                    // 6 = price


                    string ptBedroom = PatientsContactCacheDB.GetBedroom(isClinicInvoice ? null : acPtBedroomHash, lines[i].Patient.Person.EntityID);


                    tblInfo[row, 0] = (row + 1).ToString();
                    tblInfo[row, 1] = ptBedroom == null ? string.Empty : ptBedroom;
                    tblInfo[row, 2] = lines[i].Patient.Person.FullnameWithoutMiddlename;
                    tblInfo[row, 3] = (lines[i].Offering == null ? "" : lines[i].Offering.Name) + (i == lines.Length - 1 && message.Length > 0 ? Environment.NewLine + Environment.NewLine + message : "");
                    tblInfo[row, 4] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString(); ;
                    tblInfo[row, 5] = lines[i].Price.ToString();
                }
                for (int i = 0; i < receipts.Length; i++, row++)
                {
                    tblInfo[row, 0] = string.Empty;
                    tblInfo[row, 1] = string.Empty;
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = "Receipt " + receipts[i].ReceiptID + " - " + receipts[i].ReceiptPaymentType.Descr;
                    tblInfo[row, 4] = string.Empty;
                    tblInfo[row, 5] = receipts[i].Total.ToString();
                }
                for (int i = 0; i < creditNotes.Length; i++, row++)
                {
                    tblInfo[row, 0] = string.Empty;
                    tblInfo[row, 1] = string.Empty;
                    tblInfo[row, 2] = string.Empty;
                    tblInfo[row, 3] = "Adjustment Note  " + creditNotes[i].CreditNoteID;
                    tblInfo[row, 4] = string.Empty;
                    tblInfo[row, 5] = creditNotes[i].Total.ToString();
                }

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = string.Empty;
                tblInfo[row, 4] = string.Empty;
                tblInfo[row, 5] = string.Empty;
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = "<align=right><b>GST: </b></align>";
                tblInfo[row, 4] = string.Empty;
                tblInfo[row, 5] = invoice.Gst.ToString();
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = "<align=right><b>Payments & Adjustments: </b></align>";
                tblInfo[row, 4] = string.Empty;
                tblInfo[row, 5] = (invoice.ReceiptsTotal + invoice.CreditNotesTotal).ToString();
                row++;

                tblInfo[row, 0] = string.Empty;
                tblInfo[row, 1] = string.Empty;
                tblInfo[row, 2] = string.Empty;
                tblInfo[row, 3] = "<align=right><b>Balance Due: </b></align>";
                tblInfo[row, 4] = string.Empty;
                tblInfo[row, 5] = "<b>" + invoice.TotalDue.ToString() + "</b>";
                row++;
            }


            // merge

            string errorString = null;
            WordMailMerger.Merge(

                originalFile,
                outputFile,
                sourceDataSet,

                tblInfo,
                1,
                true,

                true,
                null,
                true,
                ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["EziDebit_Enabled"].Value != "1" ? null : @"https://portal.mediclinic.com.au/InvoicePaymentV2.aspx?id=" + Invoice.EncodeInvoiceHash(invoice.InvoiceID, System.Web.HttpContext.Current.Session["DB"].ToString()),
                out errorString);

            if (errorString != string.Empty)
                throw new Exception(errorString);

            if (!isBookingInvoice)
                filePaths_PrivateInvoices.Add(outputFile);
            else if (isClinicInvoice)
                filePaths_BookingClinicInvoices.Add(outputFile);
            else
                filePaths_BookingACInvoices.Add(outputFile);
        }



        // then merge them all into pdfs of each template type
        if (filePaths_BookingClinicInvoices.Count > 0) 
            MergeInvoices((string[])filePaths_BookingClinicInvoices.ToArray(typeof(string)), tmpLettersDirectory + @"InvoiceTemplate.pdf",        true);
        if (filePaths_BookingACInvoices.Count > 0) 
            MergeInvoices((string[])filePaths_BookingACInvoices.ToArray(typeof(string)),     tmpLettersDirectory + @"InvoiceTemplateAC.pdf",      true);
        if (filePaths_PrivateInvoices.Count > 0) 
            MergeInvoices((string[])filePaths_PrivateInvoices.ToArray(typeof(string)),       tmpLettersDirectory + @"PrivateInvoiceTemplate.pdf", true);


        // zip em
        string zipFileName = "Invoices.zip";
        string zipFilePath = tmpLettersDirectory2 + zipFileName;
        ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
        zip.CreateEmptyDirectories = true;
        zip.CreateZip(zipFilePath, tmpLettersDirectory, true, "");

        // get filecontents of zip here
        Letter.FileContents zipFileContents = new Letter.FileContents(zipFilePath, zipFileName);

        // delete files
        if (filePaths_BookingClinicInvoices.Count > 0)
            File.Delete(tmpLettersDirectory + @"InvoiceTemplate.pdf");
        if (filePaths_BookingACInvoices.Count > 0)
            File.Delete(tmpLettersDirectory + @"InvoiceTemplateAC.pdf");
        if (filePaths_PrivateInvoices.Count > 0)
            File.Delete(tmpLettersDirectory + @"PrivateInvoiceTemplate.pdf");

        System.IO.File.SetAttributes(zipFilePath, FileAttributes.Normal);
        System.IO.File.Delete(zipFilePath);
        System.IO.File.SetAttributes(tmpLettersDirectory, FileAttributes.Normal);
        System.IO.Directory.Delete(tmpLettersDirectory, true);
        System.IO.File.SetAttributes(tmpLettersDirectory2, FileAttributes.Normal);
        System.IO.Directory.Delete(tmpLettersDirectory2, true);


        // Nothing gets past the "DownloadDocument" method because it outputs the file 
        // which is writing a response to the client browser and calls Response.End()
        // So make sure any other code that functions goes before this
        Letter.DownloadDocument(response, zipFileContents.Contents, zipFileContents.DocName);
    }
    private static void MergeInvoices(string[] filePaths, string outputFile, bool deleteAfterMerge)
    {
        string tmpFinalFileName = Letter.MergeMultipleDocuments(filePaths, outputFile);
        File.Move(tmpFinalFileName, outputFile);

        if (deleteAfterMerge)
            foreach (string file in filePaths)
                System.IO.File.Delete(file);
    }

    // Cash sales invoice, so not linked to a booking
    protected static void GeneratePrivateInvoice(int invoiceID, bool isClinicInvoice, string originalFile, string outputFile)
    {
        Invoice       invoice     = InvoiceDB.GetByID(invoiceID);
        InvoiceLine[] lines       = InvoiceLineDB.GetByInvoiceID(invoice.InvoiceID);
        Receipt[]     receipts    = ReceiptDB.GetByInvoice(invoice.InvoiceID);
        CreditNote[]  creditNotes = CreditNoteDB.GetByInvoice(invoice.InvoiceID);

        Site site = SiteDB.GetByID(Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteID"]));


        /*
         * First merge the invoice fields
         */


        // create dataset of merge fields from invoice

        System.Data.DataSet sourceDataSet = new System.Data.DataSet();
        sourceDataSet.Tables.Add("MergeIt");

        sourceDataSet.Tables[0].Columns.Add("curr_date");
        sourceDataSet.Tables[0].Columns.Add("inv_nbr");
        sourceDataSet.Tables[0].Columns.Add("inv_date");
        sourceDataSet.Tables[0].Columns.Add("pt_fullname");
        sourceDataSet.Tables[0].Columns.Add("org_name");

        sourceDataSet.Tables[0].Columns.Add("org_abn");
        sourceDataSet.Tables[0].Columns.Add("org_acn");
        sourceDataSet.Tables[0].Columns.Add("org_bpay_account");
        sourceDataSet.Tables[0].Columns.Add("org_addr");
        sourceDataSet.Tables[0].Columns.Add("org_addr_tabbedx1");
        sourceDataSet.Tables[0].Columns.Add("org_phone");
        sourceDataSet.Tables[0].Columns.Add("org_office_fax");
        sourceDataSet.Tables[0].Columns.Add("org_web");
        sourceDataSet.Tables[0].Columns.Add("org_email");



        string orgAddressText    , orgAddressTabbedText      , orgPhoneText , orgFaxText, orgWebText, orgEmailText;
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact orgAddress = ContactDB.GetFirstByEntityID(1,    invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID);
            Contact orgPhone   = ContactDB.GetFirstByEntityID(null, invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID, "30,33,34");
            Contact orgFax     = ContactDB.GetFirstByEntityID(-1,   invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID, 29);
            Contact orgWeb     = ContactDB.GetFirstByEntityID(-1,   invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID, 28);
            Contact orgEmail   = ContactDB.GetFirstByEntityID(-1,   invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID, 27);

            orgAddressText            = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found");
            orgAddressTabbedText      = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found", 1);
            orgPhoneText              = orgPhone       == null    ? "No phone number found" : orgPhone.GetFormattedPhoneNumber("No phone number found");
            orgFaxText                = orgFax         == null    ? "No fax number found"   : orgFax.GetFormattedPhoneNumber("No fax number found");
            orgWebText                = orgWeb         == null    ? "No website found"      : orgWeb.AddrLine1;
            orgEmailText              = orgEmail       == null    ? "No email found"        : orgEmail.AddrLine1;
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus orgAddressAus = ContactAusDB.GetFirstByEntityID(1,    invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID);
            ContactAus orgPhoneAus   = ContactAusDB.GetFirstByEntityID(null, invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID, "30,33,34");
            ContactAus orgFaxAus     = ContactAusDB.GetFirstByEntityID(-1,   invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID, 29);
            ContactAus orgWebAus     = ContactAusDB.GetFirstByEntityID(-1,   invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID, 28);
            ContactAus orgEmailAus   = ContactAusDB.GetFirstByEntityID(-1,   invoice.NonBookinginvoiceOrganisation != null ? invoice.NonBookinginvoiceOrganisation.EntityID : site.EntityID, 27);

            orgAddressText            = orgAddressAus      == null ? "No address found"      : orgAddressAus.GetFormattedAddress("No address found");
            orgAddressTabbedText      = orgAddressAus      == null ? "No address found"      : orgAddressAus.GetFormattedAddress("No address found", 1);
            orgPhoneText              = orgPhoneAus        == null ? "No phone number found" : orgPhoneAus.GetFormattedPhoneNumber("No phone number found");
            orgFaxText                = orgFaxAus          == null ? "No fax number found"   : orgFaxAus.GetFormattedPhoneNumber("No fax number found");
            orgWebText                = orgWebAus          == null ? "No website found"      : orgWebAus.AddrLine1;
            orgEmailText              = orgEmailAus        == null ? "No email found"        : orgEmailAus.AddrLine1;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());


        sourceDataSet.Tables[0].Rows.Add(
            DateTime.Now.ToString("d MMMM, yyyy"),
            invoice.InvoiceID.ToString(),
            invoice.InvoiceDateAdded.ToString("d MMMM, yyyy"),
            invoice.PayerPatient == null ? "--" : invoice.PayerPatient.Person.FullnameWithTitleWithoutMiddlename,

            invoice.NonBookinginvoiceOrganisation.Name,
            invoice.NonBookinginvoiceOrganisation.Abn,
            invoice.NonBookinginvoiceOrganisation.Acn,
            invoice.NonBookinginvoiceOrganisation.BpayAccount,
            orgAddressText,
            orgAddressTabbedText,
            orgPhoneText,
            orgFaxText,
            orgWebText,
            orgEmailText

            );


        // create table data to populate from invoice lines

        string[,] tblInfo = new string[lines.Length + receipts.Length + creditNotes.Length + 4, 4];

        int row = 0;
        for (int i = 0; i < lines.Length; i++, row++)
        {
            tblInfo[row, 0] = lines[i].Offering.Name;
            tblInfo[row, 1] = ((lines[i].Quantity % 1) == 0) ? Convert.ToInt32(lines[i].Quantity).ToString() : lines[i].Quantity.ToString();
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = lines[i].Price.ToString();
        }
        for (int i = 0; i < receipts.Length; i++, row++)
        {
            tblInfo[row, 0] = "Receipt " + receipts[i].ReceiptID + " - " + receipts[i].ReceiptPaymentType.Descr;
            tblInfo[row, 1] = "1";
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = receipts[i].Total.ToString();
        }
        for (int i = 0; i < creditNotes.Length; i++, row++)
        {
            tblInfo[row, 0] = "Adjustment Note  " + creditNotes[i].CreditNoteID;
            tblInfo[row, 1] = "1";
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = creditNotes[i].Total.ToString();
        }

        tblInfo[row, 0] = string.Empty;
        tblInfo[row, 1] = string.Empty;
        tblInfo[row, 2] = string.Empty;
        tblInfo[row, 3] = string.Empty;
        row++;

        tblInfo[row, 0] = string.Empty;
        tblInfo[row, 1] = string.Empty;
        tblInfo[row, 2] = "<align=right><b>GST: </b></align>";
        tblInfo[row, 3] = invoice.Gst.ToString();
        row++;

        tblInfo[row, 0] = string.Empty;
        tblInfo[row, 1] = string.Empty;
        tblInfo[row, 2] = "<align=right><b>Payments & Adjustments: </b></align>";
        tblInfo[row, 3] = (invoice.ReceiptsTotal + invoice.CreditNotesTotal).ToString();
        row++;

        tblInfo[row, 0] = string.Empty;
        tblInfo[row, 1] = string.Empty;
        tblInfo[row, 2] = "<align=right><b>Balance Due: </b></align>";
        tblInfo[row, 3] = "<b>" + invoice.TotalDue.ToString() + "</b>";
        row++;


        // merge

        string errorString = null;
        WordMailMerger.Merge(

            originalFile,
            outputFile,
            sourceDataSet,

            tblInfo,
            1,
            true,

            true,
            null,
            true,
            ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["EziDebit_Enabled"].Value != "1" ? null : @"https://portal.mediclinic.com.au/InvoicePaymentV2.aspx?id=" + Invoice.EncodeInvoiceHash(invoice.InvoiceID, System.Web.HttpContext.Current.Session["DB"].ToString()),
            out errorString);

        if (errorString != string.Empty)
            throw new CustomMessageException(errorString);
    }


    public static DataTable GenerateInvoiceLines_ByBookingID(int bookingID, bool isAgedCare)
    {
        DataTable dt = null;
        foreach (Invoice invoice in InvoiceDB.GetByBookingID(bookingID))
        {
            if (dt == null) dt = GenerateInvoiceLines_ByInvoiceID(invoice, isAgedCare);
            else            dt.Merge(GenerateInvoiceLines_ByInvoiceID(invoice, isAgedCare));
        }

        return dt;
    }
    protected static DataTable GenerateInvoiceLines_ByInvoiceID(Invoice invoice, bool isAgedCare)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("InvoiceID",     typeof(Int32));
        dt.Columns.Add("InvoiceLineID", typeof(Int32));
        dt.Columns.Add("Room",          typeof(String));
        dt.Columns.Add("PatientName",   typeof(String));
        dt.Columns.Add("ItemDescr",     typeof(String));
        dt.Columns.Add("Qty",           typeof(Decimal));
        dt.Columns.Add("Price",         typeof(Decimal));
        dt.Columns.Add("Debtor",        typeof(String));


        InvoiceLine[] lines       = InvoiceLineDB.GetByInvoiceID(invoice.InvoiceID);
        Receipt[]     receipts    = ReceiptDB.GetByInvoice(invoice.InvoiceID);
        CreditNote[]  creditNotes = CreditNoteDB.GetByInvoice(invoice.InvoiceID);

        bool isMedicareInvoice  = invoice.PayerOrganisation != null && (invoice.PayerOrganisation.OrganisationID == -1 || invoice.PayerOrganisation.OrganisationID == -2);
        bool invoiceGapPayments = Convert.ToInt32(SystemVariableDB.GetByDescr("InvoiceGapPayments").Value) == 1;

        Site site = SiteDB.GetByID(Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteID"]));

        Hashtable acPtBedroomHash = PatientsContactCacheDB.GetBullkBedrooms(lines.Select(x => x.Patient.Person.EntityID).ToArray(), -1);

        for (int i = 0; i < lines.Length; i++)
        {

            if ( isAgedCare && lines[i].Offering.AgedCarePatientType.ID == 1) // if aged care, ignore extras
                continue;
            if (!isAgedCare && lines[i].Offering.OfferingType.ID != 63) // if clinics, ignore non services
                continue;

            DataRow newRow = dt.NewRow();
            newRow["InvoiceID"]     = lines[i].InvoiceID;
            newRow["InvoiceLineID"] = lines[i].InvoiceLineID;
            newRow["Room"]          = PatientsContactCacheDB.GetBedroom(acPtBedroomHash, lines[i].Patient.Person.EntityID);
            newRow["PatientName"]   = lines[i].Patient.Person.FullnameWithoutMiddlename;
            newRow["ItemDescr"]     = (lines[i].Offering == null ? "" : lines[i].Offering.Name);
            newRow["Qty"]           = lines[i].Quantity;
            newRow["Price"]         = lines[i].Price;

            if (invoice.PayerOrganisation != null)
                newRow["Debtor"] = invoice.PayerOrganisation.Name;
            else if (invoice.PayerPatient != null)
                newRow["Debtor"] = isAgedCare ? "Resident" : "Patient";
            else if (invoice.Booking != null && invoice.Booking.Patient != null)
                newRow["Debtor"] = isAgedCare ? "Resident" : "Patient";

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    #endregion

    #region GenerateOutstandingInvoices

    public static void GenerateOutstandingInvoicesToPrint(HttpResponse response, int[] invoiceIDs, int debtorPatientID, int debtorOrgID, bool isClinics)
    {
        string tmpLettersDirectory = Letter.GetTempLettersDirectory();
        string originalFile = Letter.GetLettersDirectory() + (isClinics ? @"OverdueInvoiceTemplate.docx" : @"OverdueInvoiceTemplateAC.docx");
        string outputFile   = FileHelper.GetTempFileName(tmpLettersDirectory + "OverdueInvoices." + "pdf"); // System.IO.Path.GetExtension(originalFile));


        GenerateOutstandingInvoices(originalFile, outputFile, invoiceIDs, debtorPatientID, debtorOrgID);



        // download the document
        byte[] fileContents = File.ReadAllBytes(outputFile);
        System.IO.File.Delete(outputFile);

        // Nothing gets past the "DownloadDocument" method because it outputs the file 
        // which is writing a response to the client browser and calls Response.End()
        // So make sure any other code that functions goes before this
        string filename = "OverdueInvoices." + System.IO.Path.GetExtension(originalFile);
        Letter.DownloadDocument(response, fileContents, Path.ChangeExtension(filename, ".pdf"));
    }

    public static void GenerateOutstandingInvoicesToPrint_Multiple(HttpResponse response, Tuple<int[], int, int>[] list, bool isClinics)
    {
        string tmpLettersDirectory = Letter.GetTempLettersDirectory();
        string originalFile        = Letter.GetLettersDirectory() + (isClinics ? @"OverdueInvoiceTemplate.docx" : @"OverdueInvoiceTemplateAC.docx");
        
        string[] filesToPrint = new string[list.Length];
        for (int i = 0; i < list.Length; i++)
        {
            int[] invoiceIDs    = list[i].Item1;
            int debtorPatientID = list[i].Item2;
            int debtorOrgID     = list[i].Item3;

            filesToPrint[i] = FileHelper.GetTempFileName(tmpLettersDirectory + "OverdueInvoices." + "docx"); // System.IO.Path.GetExtension(originalFile));
            GenerateOutstandingInvoices(originalFile, filesToPrint[i], invoiceIDs, debtorPatientID, debtorOrgID);
        }


        // merge
        string filename = "OverdueInvoices." + "pdf"; // System.IO.Path.GetExtension(originalFile);
        string tmpFinalFileName = Letter.MergeMultipleDocuments(filesToPrint, tmpLettersDirectory + System.IO.Path.GetFileName(filename));

        // put into bytes
        byte[] fileContents = System.IO.File.ReadAllBytes(tmpFinalFileName);

        // delete temp files and final merged temp file
        foreach (string file in filesToPrint)
            System.IO.File.Delete(file);
        System.IO.File.Delete(tmpFinalFileName);

        // Nothing gets past the "DownloadDocument" method because it outputs the file 
        // which is writing a response to the client browser and calls Response.End()
        // So make sure any other code that functions goes before this
        Letter.DownloadDocument(response, fileContents, filename);
    }

    public static Tuple<int, int> GenerateOutstandingInvoicesToEmail_Multiple(Tuple<int[], int, int>[] list, bool isClinics)
    {
        string tmpLettersDirectory = FileHelper.GetTempDirectoryName(Letter.GetTempLettersDirectory());
        Directory.CreateDirectory(tmpLettersDirectory);

        string originalFile = Letter.GetLettersDirectory() + (isClinics ? @"OverdueInvoiceTemplate.docx" : @"OverdueInvoiceTemplateAC.docx");

        int countSent = 0;
        for (int i = 0; i < list.Length; i++)
        {
            int[] invoiceIDs = list[i].Item1;
            int debtorPatientID = list[i].Item2;
            int debtorOrgID = list[i].Item3;

            string fileToPrint = tmpLettersDirectory + "OverdueInvoices.pdf"; // + System.IO.Path.GetExtension(originalFile);
            GenerateOutstandingInvoices(originalFile, fileToPrint, invoiceIDs, debtorPatientID, debtorOrgID);

            int entityID = -1;
            if (debtorPatientID != -1)
                entityID = PatientDB.GetEntityIDByPatientID(debtorPatientID);
            else if (debtorOrgID != -1)
                entityID = OrganisationDB.GetEntityIDByOrganisationID(debtorOrgID);


            string[] emails = ContactDB.GetEmailsByEntityID(entityID);

            if (emails.Length > 0)
            {
                EmailerNew.SimpleEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    (Utilities.IsDev() ? "eli.pollak@mediclinic.com.au" : string.Join(",", emails)),
                    "Overdue Invoices",
                    "Pease find your Invoices attached. <br/>Please call us if you do not agree with the Invoice amount stated.<br /><br />Thank you.",
                    true,
                    new string[] { fileToPrint },
                    false,
                    null
                    );
            }

            System.IO.File.Delete(fileToPrint);

            countSent++;
        }

        return new Tuple<int, int>(countSent, list.Length);
    }

    public static void GenerateOutstandingInvoices(string originalFile, string outputFile, int[] invoiceIDs, int debtorPatientID, int debtorOrgID)
    {
        Invoice[]    invoices      = InvoiceDB.GetByIDs(invoiceIDs);

        Array.Sort(invoices, delegate(Invoice x, Invoice y)
        {
            DateTime xDate = x.Booking != null ? x.Booking.DateStart : x.InvoiceDateAdded;
            DateTime yDate = y.Booking != null ? y.Booking.DateStart : y.InvoiceDateAdded;
            return xDate.CompareTo(yDate);
        });

        Patient      debtorPatient = debtorPatientID == -1 ? null : PatientDB.GetByID(debtorPatientID);
        Organisation debtorOrg     = debtorOrgID     == -1 ? null : OrganisationDB.GetByID(debtorOrgID);



        // create dataset of merge fields from invoice

        System.Data.DataSet sourceDataSet = new System.Data.DataSet();
        sourceDataSet.Tables.Add("MergeIt");

        sourceDataSet.Tables[0].Columns.Add("curr_date");
        sourceDataSet.Tables[0].Columns.Add("inv_debtor_name");
        sourceDataSet.Tables[0].Columns.Add("inv_debtor_addr");
        sourceDataSet.Tables[0].Columns.Add("inv_debtor_addr_tabbedx1");


        string debtorNameText = debtorPatient != null ? debtorPatient.Person.FullnameWithoutMiddlename : (debtorOrg != null ? debtorOrg.Name     : string.Empty);
        int    debtorEntityID = debtorPatient != null ? debtorPatient.Person.EntityID                  : (debtorOrg != null ? debtorOrg.EntityID : -1);

        string debtorAddressText, debtorAddressTabbedText;
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact debtorAddress = ContactDB.GetFirstByEntityID(1, debtorEntityID);
            debtorAddressText       = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found") : "No address found";
            debtorAddressTabbedText = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found", 1) : "No address found";
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus debtorAddress = ContactAusDB.GetFirstByEntityID(1, debtorEntityID);
            debtorAddressText        = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found") : "No address found";
            debtorAddressTabbedText  = debtorAddress != null ? debtorAddress.GetFormattedAddress("No address found", 1) : "No address found";
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        sourceDataSet.Tables[0].Rows.Add(
            DateTime.Now.ToString("d MMMM, yyyy"),
            debtorNameText,
            debtorAddressText,
            debtorAddressTabbedText
            );


        // create table data to populate from invoices

        string[,] tblInfo;

        if (true)
        {
            tblInfo = new string[invoices.Length + 2, 7];

            decimal totalDue = 0;
            int row = 0;
            for (int i = 0; i < invoices.Length; i++, row++)
            {

                string patientName = null;
                if (invoices[i].PayerPatient != null)
                    patientName = invoices[i].PayerPatient.Person.FullnameWithoutMiddlename;
                else if (invoices[i].PayerOrganisation != null && (new List<int> { 139, 367, 362, -7, -8 }).Contains(invoices[i].PayerOrganisation.OrganisationType.OrganisationTypeID))
                {
                    InvoiceLine[] invLines = InvoiceLineDB.GetByInvoiceID(invoices[i].InvoiceID);
                    if (invLines.Length == 1 && invLines[0].Patient != null)
                        patientName = invLines[0].Patient.Person.FullnameWithoutMiddlename;
                }

                tblInfo[row, 0] = invoices[i].InvoiceID.ToString() + (invoices[i].HealthcareClaimNumber.Trim().Length == 0 ? "" : Environment.NewLine + "Claim No." + Environment.NewLine + invoices[i].HealthcareClaimNumber.Trim()) + (patientName == null ? "" : Environment.NewLine + patientName);
                tblInfo[row, 1] = invoices[i].Booking != null ? invoices[i].Booking.DateStart.ToString("d MMM, yyyy") : invoices[i].InvoiceDateAdded.ToString("d MMM, yyyy");
                tblInfo[row, 2] = string.Format("{0:C}", invoices[i].Total);
                tblInfo[row, 3] = string.Format("{0:C}", invoices[i].ReceiptsTotal + invoices[i].CreditNotesTotal);
                tblInfo[row, 4] = string.Format("{0:C}", invoices[i].TotalDue);
                tblInfo[row, 5] = invoices[i].Booking != null ? ((int)DateTime.Today.Subtract(invoices[i].Booking.DateStart).TotalDays).ToString() : ((int)DateTime.Today.Subtract(invoices[i].InvoiceDateAdded).TotalDays).ToString();
                tblInfo[row, 6] = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["EziDebit_Enabled"].Value != "1" ? "" : "<invpaymentlink>" + @"https://portal.mediclinic.com.au/InvoicePaymentV2.aspx?id=" + Invoice.EncodeInvoiceHash(invoices[i].InvoiceID, System.Web.HttpContext.Current.Session["DB"].ToString()) + "</invpaymentlink>";

                totalDue += invoices[i].TotalDue;
            }

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = string.Empty;
            tblInfo[row, 4] = string.Empty;
            tblInfo[row, 5] = string.Empty;
            tblInfo[row, 6] = string.Empty;
            row++;

            tblInfo[row, 0] = string.Empty;
            tblInfo[row, 1] = string.Empty;
            tblInfo[row, 2] = string.Empty;
            tblInfo[row, 3] = "<b>Due: </b>"; 
            tblInfo[row, 4] = "<b>" + string.Format("{0:C}", totalDue) + "</b>";
            tblInfo[row, 5] = string.Empty;
            tblInfo[row, 6] = string.Empty;
            row++;
        }




        // merge

        string errorString = null;
        WordMailMerger.Merge(

            originalFile,
            outputFile,
            sourceDataSet,

            tblInfo,
            1,
            true,

            true,
            null,
            true,
            null,
            out errorString);

        if (errorString != string.Empty)
            throw new CustomMessageException(errorString);
    }

    #endregion
    
    #region CreateLetter + DownloadDocument, SendLetter

    public static FileContents CreateLetter(FileFormat fileFormat, Site site, int letter_id, int org_id, int patient_id, int staff_id, int booking_id, int healthCardActionID, int nCopies, string[] notes, bool keepInHistory, int letterPrintHistorySendMethodID)
    {
        string tmpFinalFileName = CreateLetterAndReturnTempFile(fileFormat, site, letter_id, org_id, patient_id, staff_id, booking_id, healthCardActionID, nCopies, notes, keepInHistory, letterPrintHistorySendMethodID);

        // download the document
        byte[] fileContents = File.ReadAllBytes(tmpFinalFileName);
        System.IO.File.Delete(tmpFinalFileName);

        Letter letter = LetterDB.GetByID(letter_id);
        return new FileContents(fileContents, fileFormat == FileFormat.PDF ? Path.ChangeExtension(letter.Docname, ".pdf") : letter.Docname.Replace(".dot", ".doc"));
    }

    public static string CreateLetterAndReturnTempFile(FileFormat fileFormat, Site site, int letter_id, int org_id, int patient_id, int staff_id, int booking_id, int healthCardActionID, int nCopies, string[] notes, bool keepInHistory, int letterPrintHistorySendMethodID)
    {
        string lettersDir = GetLettersDirectory();
        if (!Directory.Exists(lettersDir))
            throw new CustomMessageException("Letters directory doesn't exist");

        Letter letter = LetterDB.GetByID(letter_id);
        bool useDefaultDocs = letter.Organisation == null ? true : !LetterDB.OrgHasdocs(letter.Organisation.OrganisationID);
        string sourceTemplatePath = lettersDir + (useDefaultDocs ? @"Default\" + letter.Site.SiteID + @"\" : letter.Organisation.OrganisationID + @"\") + letter.Docname;
        if (!File.Exists(sourceTemplatePath))
            throw new CustomMessageException("File doesn't exist: " + Path.GetFileName(sourceTemplatePath));


        // get temp directory
        string tmpLettersDirectory = GetTempLettersDirectory();
        if (!Directory.Exists(tmpLettersDirectory))
            throw new CustomMessageException("Temp letters directory doesn't exist");

        // delete old tmp files
        FileHelper.DeleteOldFiles(tmpLettersDirectory, new TimeSpan(1, 0, 0));


        // create doc

        string newFileName = letter.Docname.Replace(".dot", ".doc");

        string tmpSingleFileName = Letter.CreateMergedDocument(
            letter.LetterID,
            keepInHistory && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["StoreLettersHistoryInDB"]),
            keepInHistory && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["StoreLettersHistoryInFlatFile"]),
            letterPrintHistorySendMethodID,
            Letter.GetLettersHistoryDirectory(org_id),
            newFileName,
            site,
            org_id,
            booking_id,
            patient_id,
            -1, // register_referrer_id_to_use_instead_of_patients_reg_ref
            staff_id,
            healthCardActionID,
            sourceTemplatePath,
            tmpLettersDirectory + newFileName,
            true,
            notes);


        // create multiple copies

        newFileName = (fileFormat == FileFormat.PDF) ? Path.ChangeExtension(newFileName, ".pdf") : newFileName;
        string tmpFinalFileName = FileHelper.GetTempFileName(tmpLettersDirectory + newFileName);
        Letter.CreateMultipleCopies(nCopies, tmpSingleFileName, tmpFinalFileName);
        File.Delete(tmpSingleFileName);

        return tmpFinalFileName;
    }

    public static void SendLetter(HttpResponse response, FileFormat fileFormat, Site site, int letter_id, int org_id, int patient_id, int staff_id, int booking_id, int healthCardActionID, int nCopies, string[] notes, bool keepInHistory, int letterPrintHistorySendMethodID)
    {
        Letter.FileContents fileContents = CreateLetter(fileFormat, site, letter_id, org_id, patient_id, staff_id, booking_id, healthCardActionID, nCopies, notes, keepInHistory, letterPrintHistorySendMethodID);

        // Nothing gets past the "DownloadDocument" method because it outputs the file 
        // which is writing a response to the client browser and calls Response.End()
        // So make sure any other code that functions goes before this
        Letter.DownloadDocument(response, fileContents.Contents, fileContents.DocName);
    }

    public static string CreateMergedDocument(int letterID, bool keepHistoryInDB, bool keepHistoryInFile, int letterPrintHistorySendMethodID, string historyDir, string historyFileName, Site site, int organisation_id, int booking_id, int patient_id, int register_referrer_id_to_use_instead_of_patients_reg_ref, int staff_id, int health_card_action_id, string sourceTemplatePath, string outputDocPath, bool isDoubleSidedPrinting = false, string[] notes = null)
    {
        // get patient (can be null - eg for bulk referrer letters sendout)

        Patient patient = patient_id == -1 ? null : PatientDB.GetByID(patient_id);
        if (patient_id != -1 && patient == null)
            throw new CustomMessageException("Invalid patient selected.");

        // get patient address
        Contact    patientAddress    = null, patientPhone    = null;
        ContactAus patientAddressAus = null, patientPhoneAus = null;
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            patientAddress = patient == null ? null : ContactDB.GetFirstByEntityID(1, patient.Person.EntityID);
            patientPhone   = patient == null ? null : ContactDB.GetFirstByEntityID(2, patient.Person.EntityID);
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            patientAddressAus = patient == null ? null : ContactAusDB.GetFirstByEntityID(1, patient.Person.EntityID);
            patientPhoneAus   = patient == null ? null : ContactAusDB.GetFirstByEntityID(2, patient.Person.EntityID);
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        // get healthcare cards
        HealthCard[] medicareCards = patient == null ? new HealthCard[0] : HealthCardDB.GetAllByPatientID(patient.PatientID, true, -1);
        HealthCard[] dvaCards      = patient == null ? new HealthCard[0] : HealthCardDB.GetAllByPatientID(patient.PatientID, true, -2);
        HealthCard   medicareCard  = medicareCards.Length == 0 ? null : medicareCards[0];
        HealthCard   dvaCard       = dvaCards.Length      == 0 ? null : dvaCards[0];

        // get organisation
        Organisation org = organisation_id == 0 ? null : OrganisationDB.GetByID(organisation_id);
        if (organisation_id != 0 && org == null)
            throw new CustomMessageException("Invalid organisation selected.");

        // get org address
        Contact    orgAddress    = null, orgPhone    = null, orgFax    = null, orgWeb    = null, orgEmail    = null;
        ContactAus orgAddressAus = null, orgPhoneAus = null, orgFaxAus = null, orgWebAus = null, orgEmailAus = null;
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            orgAddress = ContactDB.GetFirstByEntityID(1,    org != null ? org.EntityID : site.EntityID);
            orgPhone   = ContactDB.GetFirstByEntityID(null, org != null ? org.EntityID : site.EntityID, "30,33,34");
            orgFax     = ContactDB.GetFirstByEntityID(-1,   org != null ? org.EntityID : site.EntityID, 29);
            orgWeb     = ContactDB.GetFirstByEntityID(-1,   org != null ? org.EntityID : site.EntityID, 28);
            orgEmail   = ContactDB.GetFirstByEntityID(-1,   org != null ? org.EntityID : site.EntityID, 27);

        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            orgAddressAus = ContactAusDB.GetFirstByEntityID(1,    org != null ? org.EntityID : site.EntityID);
            orgPhoneAus   = ContactAusDB.GetFirstByEntityID(null, org != null ? org.EntityID : site.EntityID, "30,33,34");
            orgFaxAus     = ContactAusDB.GetFirstByEntityID(-1,   org != null ? org.EntityID : site.EntityID, 29);
            orgWebAus     = ContactAusDB.GetFirstByEntityID(-1,   org != null ? org.EntityID : site.EntityID, 28);
            orgEmailAus   = ContactAusDB.GetFirstByEntityID(-1,   org != null ? org.EntityID : site.EntityID, 27);
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        // get referrer
        RegisterReferrer regReferrer = null;
        if (register_referrer_id_to_use_instead_of_patients_reg_ref != -1)
        {
            regReferrer = RegisterReferrerDB.GetByID(register_referrer_id_to_use_instead_of_patients_reg_ref);
        }
        else
        {
            RegisterReferrer[] orgRefs = PatientReferrerDB.GetActiveEPCReferrersOf(patient.PatientID);
            regReferrer = orgRefs.Length == 0 ? null : orgRefs[0];
        }

        // get referrer address
        Contact    referrerAddress    = null, referrerPhone    = null, referrerFax    = null;
        ContactAus referrerAddressAus = null, referrerPhoneAus = null, referrerFaxAus = null;
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            referrerAddress = regReferrer == null || regReferrer.Organisation == null ? null : ContactDB.GetFirstByEntityID(1, regReferrer.Organisation.EntityID);
            referrerPhone   = regReferrer == null || regReferrer.Organisation == null ? null : ContactDB.GetFirstByEntityID("2", regReferrer.Organisation.EntityID, "30,33,34" );
            referrerFax     = regReferrer == null || regReferrer.Organisation == null ? null : ContactDB.GetFirstByEntityID(-1, regReferrer.Organisation.EntityID, 29);
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            referrerAddressAus = regReferrer == null || regReferrer.Organisation == null ? null : ContactAusDB.GetFirstByEntityID(1, regReferrer.Organisation.EntityID);
            referrerPhoneAus = regReferrer == null || regReferrer.Organisation == null ? null   : ContactAusDB.GetFirstByEntityID("2", regReferrer.Organisation.EntityID, "30,33,34");
            referrerFaxAus     = regReferrer == null || regReferrer.Organisation == null ? null : ContactAusDB.GetFirstByEntityID(-1, regReferrer.Organisation.EntityID, 29);
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());



        // get staff (logged in)
        Staff staff = StaffDB.GetByID(staff_id);

        // get booking and healthcardaction
        Booking          booking          = booking_id            == -1 ? null : BookingDB.GetByID(booking_id);
        HealthCardAction healthCardAction = health_card_action_id == -1 ? null : HealthCardActionDB.GetByID(health_card_action_id);


        // get epc info

        HealthCard               activeHcCard                  = (medicareCard != null && medicareCard.IsActive) ? medicareCard : ((dvaCard != null && dvaCard.IsActive) ? dvaCard : null);
        bool                     hasEPC                        = activeHcCard  != null && activeHcCard.DateReferralSigned != DateTime.MinValue;
        HealthCardEPCRemaining[] epcsRemaining                 = !hasEPC ? new HealthCardEPCRemaining[] { } : HealthCardEPCRemainingDB.GetByHealthCardID(activeHcCard.HealthCardID);
        int                      MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);
        int                      totalServicesAllowedLeft      = !hasEPC ? 0 : MedicareMaxNbrServicesPerYear - (int)InvoiceDB.GetMedicareCountByPatientAndDateRange(patient_id, new DateTime(DateTime.Now.Year , 1, 1), new DateTime(DateTime.Now.Year , 12, 31));

        int totalEpcsRemaining = 0;
        for (int j = 0; j < epcsRemaining.Length; j++)
            totalEpcsRemaining += epcsRemaining[j].NumServicesRemaining;

        DateTime referralSignedDate = !hasEPC ? DateTime.MinValue : activeHcCard.DateReferralSigned.Date;
        DateTime hcExpiredDate      = !hasEPC ? DateTime.MinValue : referralSignedDate.AddYears(1);
        bool     isExpired          = !hasEPC ? true              : hcExpiredDate <= DateTime.Today;

        int nServicesLeft = 0;
        if (activeHcCard != null && DateTime.Today >= referralSignedDate.Date && DateTime.Today < hcExpiredDate.Date)
            nServicesLeft = totalEpcsRemaining;
        if (activeHcCard != null && totalServicesAllowedLeft < nServicesLeft)
            nServicesLeft = totalServicesAllowedLeft;

        bool has_valid_epc = hasEPC && !isExpired && (activeHcCard.Organisation.OrganisationID == -2 || (activeHcCard.Organisation.OrganisationID == -1 && nServicesLeft > 0));

        int      epc_count_remaining = hasEPC && activeHcCard.Organisation.OrganisationID == -1 ? nServicesLeft : 0;
        DateTime epc_expire_date     = hasEPC                                         ? hcExpiredDate : DateTime.MinValue;

        DateTime pt_last_bk_date = BookingDB.GetLastBookingDate(patient_id);


        // merge
        string tmpSingleFileName = FileHelper.GetTempFileName(outputDocPath);
        MergeDocument(sourceTemplatePath, tmpSingleFileName, 
                      booking, patient, patientAddress, patientAddressAus, patientPhone, patientPhoneAus, medicareCard, dvaCard, site, 
                      org, orgAddress, orgAddressAus, orgPhone, orgPhoneAus, orgFax, orgFaxAus, orgWeb, orgWebAus, orgEmail, orgEmailAus,
                      staff, regReferrer, referrerAddress, referrerAddressAus, referrerPhone, referrerPhoneAus, referrerFax, referrerFaxAus, healthCardAction, 
                      pt_last_bk_date, epc_expire_date, epc_count_remaining, 
                      isDoubleSidedPrinting, notes, keepHistoryInDB, keepHistoryInFile, letterPrintHistorySendMethodID, historyDir, historyFileName, letterID);

        return tmpSingleFileName;
    }

    protected static void MergeDocument(string sourceTemplatePath, string outputDocPath,
                    Booking booking, Patient patient, Contact patientAddress, ContactAus patientAddressAus, Contact patientPhone, ContactAus patientPhoneAus, HealthCard medicareCard, HealthCard dvaCard, Site site,
                    Organisation org, Contact orgAddress, ContactAus orgAddressAus, Contact orgPhone, ContactAus orgPhoneAus, Contact orgFax, ContactAus orgFaxAus, Contact orgWeb, ContactAus orgWebAus, Contact orgEmail, ContactAus orgEmailAus,
                    Staff staff, RegisterReferrer regReferrer, Contact referrerAddress, ContactAus referrerAddressAus, Contact referrerPhone, ContactAus referrerPhoneAus, Contact referrerFax, ContactAus referrerFaxAus, HealthCardAction healthCardAction,

                    DateTime pt_last_bk_date, DateTime epc_expire_date, int      epc_count_remaining, 

                    bool isDoubleSidedPrinting, string[] extraPages,
                    bool keepHistoryInDB, bool keepHistoryInFile, int letterPrintHistorySendMethodID, string historyDir, string historyFileName, int letterID)
    {

        HealthCard activeHcCard = (medicareCard != null && medicareCard.IsActive) ? medicareCard : ((dvaCard != null && dvaCard.IsActive) ? dvaCard : null);


        DataSet sourceDataSet = new DataSet();
        sourceDataSet.Tables.Add("MergeIt");

        sourceDataSet.Tables[0].Columns.Add("curr_date");

        sourceDataSet.Tables[0].Columns.Add("pt_name");
        sourceDataSet.Tables[0].Columns.Add("pt_title");
        sourceDataSet.Tables[0].Columns.Add("pt_firstname");
        sourceDataSet.Tables[0].Columns.Add("pt_middlename");
        sourceDataSet.Tables[0].Columns.Add("pt_surname");
        sourceDataSet.Tables[0].Columns.Add("pt_gender");
        sourceDataSet.Tables[0].Columns.Add("pt_dob");
        sourceDataSet.Tables[0].Columns.Add("pt_dob_day_month_only");

        sourceDataSet.Tables[0].Columns.Add("pt_conditions");

        sourceDataSet.Tables[0].Columns.Add("pt_addr");
        sourceDataSet.Tables[0].Columns.Add("pt_addr_tabbedx1");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_line1");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_line2");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_street");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_suburb");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_postcode");
        //sourceDataSet.Tables[0].Columns.Add("pt_addr_country");
        sourceDataSet.Tables[0].Columns.Add("pt_phone");

        sourceDataSet.Tables[0].Columns.Add("pt_last_bk_date");

        sourceDataSet.Tables[0].Columns.Add("pt_hc_card_nbr");
        sourceDataSet.Tables[0].Columns.Add("pt_hc_card_name");
        sourceDataSet.Tables[0].Columns.Add("pt_hc_card_refsigneddate");
        sourceDataSet.Tables[0].Columns.Add("pt_epc_expire_date");
        sourceDataSet.Tables[0].Columns.Add("pt_epc_count_remaining");

        sourceDataSet.Tables[0].Columns.Add("pt_mc_card_nbr");
        sourceDataSet.Tables[0].Columns.Add("pt_mc_card_name");
        sourceDataSet.Tables[0].Columns.Add("pt_mc_card_refsigneddate");

        sourceDataSet.Tables[0].Columns.Add("pt_dvacard_nbr");
        sourceDataSet.Tables[0].Columns.Add("pt_dvacard_name");
        sourceDataSet.Tables[0].Columns.Add("pt_dvacard_refsigneddate");

        sourceDataSet.Tables[0].Columns.Add("org_name");
        sourceDataSet.Tables[0].Columns.Add("org_abn");
        sourceDataSet.Tables[0].Columns.Add("org_acn");
        sourceDataSet.Tables[0].Columns.Add("org_bpay_account");

        sourceDataSet.Tables[0].Columns.Add("org_addr");
        sourceDataSet.Tables[0].Columns.Add("org_addr_tabbedx1");
        //sourceDataSet.Tables[0].Columns.Add("org_addr_line1");
        //sourceDataSet.Tables[0].Columns.Add("org_addr_line2");
        //sourceDataSet.Tables[0].Columns.Add("org_addr_street");
        //sourceDataSet.Tables[0].Columns.Add("org_addr_suburb");
        //sourceDataSet.Tables[0].Columns.Add("org_addr_postcode");
        //sourceDataSet.Tables[0].Columns.Add("org_addr_country");
        sourceDataSet.Tables[0].Columns.Add("org_phone");
        sourceDataSet.Tables[0].Columns.Add("org_office_fax");
        sourceDataSet.Tables[0].Columns.Add("org_web");
        sourceDataSet.Tables[0].Columns.Add("org_email");

        sourceDataSet.Tables[0].Columns.Add("ref_name");
        sourceDataSet.Tables[0].Columns.Add("ref_title");
        sourceDataSet.Tables[0].Columns.Add("ref_firstname");
        sourceDataSet.Tables[0].Columns.Add("ref_middlename");
        sourceDataSet.Tables[0].Columns.Add("ref_surname");
        //sourceDataSet.Tables[0].Columns.Add("ref_gender");
        //sourceDataSet.Tables[0].Columns.Add("ref_dob");

        sourceDataSet.Tables[0].Columns.Add("ref_addr");
        sourceDataSet.Tables[0].Columns.Add("ref_addr_tabbedx1");
        //sourceDataSet.Tables[0].Columns.Add("ref_addr_line1");
        //sourceDataSet.Tables[0].Columns.Add("ref_addr_line2");
        //sourceDataSet.Tables[0].Columns.Add("ref_addr_street");
        //sourceDataSet.Tables[0].Columns.Add("ref_addr_suburb");
        //sourceDataSet.Tables[0].Columns.Add("ref_addr_postcode");
        //sourceDataSet.Tables[0].Columns.Add("ref_addr_country");
        sourceDataSet.Tables[0].Columns.Add("ref_phone");
        sourceDataSet.Tables[0].Columns.Add("ref_fax");

        sourceDataSet.Tables[0].Columns.Add("bk_date");
        sourceDataSet.Tables[0].Columns.Add("bk_time");
        sourceDataSet.Tables[0].Columns.Add("bk_length");

        sourceDataSet.Tables[0].Columns.Add("bk_prov_name");
        sourceDataSet.Tables[0].Columns.Add("bk_prov_title");
        sourceDataSet.Tables[0].Columns.Add("bk_prov_firstname");
        sourceDataSet.Tables[0].Columns.Add("bk_prov_middlename");
        sourceDataSet.Tables[0].Columns.Add("bk_prov_surname");
        sourceDataSet.Tables[0].Columns.Add("bk_prov_number");
        //sourceDataSet.Tables[0].Columns.Add("bk_prov_gender");
        //sourceDataSet.Tables[0].Columns.Add("bk_prov_dob");

        sourceDataSet.Tables[0].Columns.Add("bk_treatment_notes");

        sourceDataSet.Tables[0].Columns.Add("bk_offering_name");
        sourceDataSet.Tables[0].Columns.Add("bk_offering_short_name");
        sourceDataSet.Tables[0].Columns.Add("bk_offering_descr");


        string bookingNotes = string.Empty;
        if (booking != null)
        {
            if (booking.Patient != null)
            {
                foreach (Note note in booking.GetTreatmentNotes())
                    bookingNotes += "Treatment Note:" + Environment.NewLine + Environment.NewLine + note.Text + Environment.NewLine + Environment.NewLine;
            }
            else if (patient != null) // group bookings - but need patientID
            {
                BookingPatient bp = BookingPatientDB.GetByBookingAndPatientID(booking.BookingID, patient.PatientID);
                if (bp != null)
                    foreach (Note note in NoteDB.GetByEntityID(bp.EntityID, "252"))
                        bookingNotes += "Treatment Note:" + Environment.NewLine + Environment.NewLine + note.Text + Environment.NewLine + Environment.NewLine;
            }

        }

        /*  -- for testing
            string s1 = patient == null ? "No patient found" : patient.Person.FullnameWithTitleWithoutMiddlename;
            string s2 = patient == null ? "No patient found" : (patient.Person.Title.ID == 0 ? "" : patient.Person.Title.Descr);
            string s3 = patient == null ? "" : patient.Person.Firstname;
            string s4 = patient == null ? "" : patient.Person.Middlename;
            string s5 = patient == null ? "" : patient.Person.Surname;
            string s6 = patient == null ? "" : patient.Person.Gender;
            string s7 = patient == null ? "" : patient.Person.Dob == DateTime.MinValue ? "" : patient.Person.Dob.ToString("d MMMM, yyyy");

            string s8 = patientAddress == null ? "No address found" : patientAddress.AddrLine1;
            string s9 = patientAddress == null ? "" : patientAddress.AddrLine2;
            string s10 = patientAddress == null || patientAddress.AddressChannel == null ? "" : (patientAddress.AddressChannel.AddressChannelID == 1 ? "" : patientAddress.AddressChannel.DisplayName);
            string s11 = patientAddress == null ? "" : patientAddress.Suburb.Name;
            string s12 = patientAddress == null ? "" : patientAddress.Suburb.Postcode;
            string s13 = patientAddress == null || patientAddress.Country == null ? "" : patientAddress.Country.Descr;

            string s14 = activeHcCard == null ? "No hc card found" : activeHcCard.CardNbr + " - " + activeHcCard.CardFamilyMemberNbr;
            string s15 = activeHcCard == null ? "" : activeHcCard.CardName;
            string s16 = activeHcCard == null ? "" : activeHcCard.DateReferralSigned.ToString("d MMMM, yyyy");

            string s17 = medicareCard == null ? "No medicare card found" : medicareCard.CardNbr + " - " + medicareCard.CardFamilyMemberNbr;
            string s18 = medicareCard == null ? "" : medicareCard.CardName;
            string s19 = medicareCard == null ? "" : medicareCard.DateReferralSigned.ToString("d MMMM, yyyy");

            string s20 = dvaCard == null ? "No dva card found" : dvaCard.CardNbr;
            string s21 = dvaCard == null ? "" : dvaCard.CardName;
            string s22 = dvaCard == null ? "" : dvaCard.DateReferralSigned.ToString("d MMMM, yyyy");

            string s23 = org != null ? org.Name        : site.Name;
            string s24 = org != null ? org.Abn         : site.Abn;
            string s25 = org != null ? org.Acn         : site.Acn;
            string s26 = org != null ? org.BpayAccount : site.BankBpay;

            string s27 = orgAddress == null ? "No address found" : orgAddress.AddrLine1;
            string s28 = orgAddress == null ? "" : orgAddress.AddrLine2;
            string s29 = orgAddress == null ? "" : (orgAddress.AddressChannel.AddressChannelID == 1 ? "" : orgAddress.AddressChannel.DisplayName);
            string s30 = orgAddress == null ? "" : orgAddress.Suburb.Name;
            string s31 = orgAddress == null ? "" : orgAddress.Suburb.Postcode;
            string s32 = orgAddress == null || orgAddress.Country == null ? "" : orgAddress.Country.Descr;

            string s33 = regReferrer == null ? "No referrer found" : regReferrer.Referrer.Person.FullnameWithTitleWithoutMiddlename;
            string s34 = regReferrer == null ? "No referrer found" : (regReferrer.Referrer.Person.Title.ID == 0 ? "" : regReferrer.Referrer.Person.Title.Descr);
            string s35 = regReferrer == null ? "" : regReferrer.Referrer.Person.Firstname;
            string s36 = regReferrer == null ? "" : regReferrer.Referrer.Person.Middlename;
            string s37 = regReferrer == null ? "" : regReferrer.Referrer.Person.Surname;
            string s38 = regReferrer == null ? "" : regReferrer.Referrer.Person.Gender;
            string s39 = regReferrer == null ? "" : regReferrer.Referrer.Person.Dob.ToString("d MMMM, yyyy");

            string s40 = referrerAddress == null ? "No address found" : referrerAddress.AddrLine1;
            string s41 = referrerAddress == null ? "" : referrerAddress.AddrLine2;
            string s42 = referrerAddress == null ? "" : (referrerAddress.AddressChannel.AddressChannelID == 1 ? "" : referrerAddress.AddressChannel.DisplayName);
            string s43 = referrerAddress == null ? "" : referrerAddress.Suburb.Name;
            string s44 = referrerAddress == null ? "" : referrerAddress.Suburb.Postcode;
            string s45 = referrerAddress == null || referrerAddress.Country == null ? "" : referrerAddress.Country.Descr;

            string s46 = booking == null ? "" : booking.DateStart.ToString("d MMMM, yyyy");
            string s47 = booking == null ? "" : booking.DateStart.ToString("HH:mm");

            string s48 = booking == null || booking.Provider == null ? "" : booking.Provider.Person.FullnameWithTitleWithoutMiddlename;
            string s49 = booking == null || booking.Provider == null ? "" : (booking.Provider.Person.Title.ID == 0 ? "" : booking.Provider.Person.Title.Descr);
            string s50 = booking == null || booking.Provider == null ? "" : booking.Provider.Person.Firstname;
            string s51 = booking == null || booking.Provider == null ? "" : booking.Provider.Person.Middlename;
            string s52 = booking == null || booking.Provider == null ? "" : booking.Provider.Person.Surname;
            string s53 = booking == null || booking.Provider == null ? "" : booking.Provider.Person.Gender;
            string s54 = booking == null || booking.Provider == null ? "" : booking.Provider.Person.Dob.ToString("d MMMM, yyyy");

            string s55 = booking == null ? "" : bookingNotes;

            string s56 = booking == null || booking.Offering == null ? "" : booking.Offering.Name;
            string s57 = booking == null || booking.Offering == null ? "" : booking.Offering.ShortName;
            string s58 = booking == null || booking.Offering == null ? "" : booking.Offering.Descr;
        */



        string patientAddressText  , patientAddressTabbedText  , patientPhoneText;
        string orgAddressText      , orgAddressTabbedText      , orgPhoneText       , orgFaxText, orgWebText, orgEmailText;
        string referrerAddressText , referrerAddressTabbedText , referrerPhoneText, referrerFaxText;
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            patientAddressText        = patientAddress == null    ? "No address found"      : patientAddress.GetFormattedAddress("No address found");
            patientAddressTabbedText  = patientAddress == null    ? "No address found"      : patientAddress.GetFormattedAddress("No address found", 1);
            orgAddressText            = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found");
            orgAddressTabbedText      = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found", 1);
            referrerAddressText       = orgAddress     == null    ? "No address found"      : referrerAddress.GetFormattedAddress("No address found");
            referrerAddressTabbedText = orgAddress     == null    ? "No address found"      : referrerAddress.GetFormattedAddress("No address found", 1);
            patientPhoneText          = patientPhone   == null    ? "No phone number found" : patientPhone.GetFormattedPhoneNumber("No phone number found");
            orgPhoneText              = orgPhone       == null    ? "No phone number found" : orgPhone.GetFormattedPhoneNumber("No phone number found");
            referrerPhoneText         = referrerPhone  == null    ? "No phone number found" : referrerPhone.GetFormattedPhoneNumber("No phone number found");
            referrerFaxText           = referrerFax    == null    ? "No fax number found"   : referrerFax.GetFormattedPhoneNumber("No fax number found");
            orgFaxText                = orgFax         == null    ? "No fax number found"   : orgFax.GetFormattedPhoneNumber("No fax number found");
            orgWebText                = orgWeb         == null    ? "No website found"      : orgWeb.AddrLine1;
            orgEmailText              = orgEmail       == null    ? "No email found"        : orgEmail.AddrLine1;
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            patientAddressText        = patientAddressAus  == null ? "No address found"      : patientAddressAus.GetFormattedAddress("No address found");
            patientAddressTabbedText  = patientAddressAus  == null ? "No address found"      : patientAddressAus.GetFormattedAddress("No address found", 1);
            orgAddressText            = orgAddressAus      == null ? "No address found"      : orgAddressAus.GetFormattedAddress("No address found");
            orgAddressTabbedText      = orgAddressAus      == null ? "No address found"      : orgAddressAus.GetFormattedAddress("No address found", 1);
            referrerAddressText       = referrerAddressAus == null ? "No address found"      : referrerAddressAus.GetFormattedAddress("No address found");
            referrerAddressTabbedText = referrerAddressAus == null ? "No address found"      : referrerAddressAus.GetFormattedAddress("No address found", 1);
            patientPhoneText          = patientPhoneAus    == null ? "No phone number found" : patientPhoneAus.GetFormattedPhoneNumber("No phone number found");
            orgPhoneText              = orgPhoneAus        == null ? "No phone number found" : orgPhoneAus.GetFormattedPhoneNumber("No phone number found");
            referrerPhoneText         = referrerPhoneAus   == null ? "No phone number found" : referrerPhoneAus.GetFormattedPhoneNumber("No phone number found");
            referrerFaxText           = referrerFaxAus     == null ? "No fax number found"   : referrerFaxAus.GetFormattedPhoneNumber("No fax number found");
            orgFaxText                = orgFaxAus          == null ? "No fax number found"   : orgFaxAus.GetFormattedPhoneNumber("No fax number found");
            orgWebText                = orgWebAus          == null ? "No website found"      : orgWebAus.AddrLine1;
            orgEmailText              = orgEmailAus        == null ? "No email found"        : orgEmailAus.AddrLine1;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());


        string    ptConditionsText   = string.Empty;
        Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);
        foreach (Condition condition in ConditionDB.GetAll())
            if (selectedConditions[condition.ConditionID] != null)
                ptConditionsText += (ptConditionsText.Length == 0 ? "" : Environment.NewLine) + " • " + ((PatientCondition)selectedConditions[condition.ConditionID]).Condition.Descr;
        if (ptConditionsText == string.Empty)
            ptConditionsText = " • None";

        string bk_prov_number = string.Empty;
        if (booking != null && booking.Provider != null && booking.Organisation != null)
        {
            RegisterStaff regStaff = RegisterStaffDB.GetByStaffIDAndOrganisationID(booking.Provider.StaffID, booking.Organisation.OrganisationID);
            bk_prov_number = (regStaff == null) ? string.Empty : regStaff.ProviderNumber;
        }

        TimeSpan bkTime = booking != null ? booking.DateEnd.Subtract(booking.DateStart) : TimeSpan.Zero;
        string bkTimeLength;
        if (bkTime.Minutes > 0 && bkTime.Hours > 0)
            bkTimeLength = bkTime.Hours + (bkTime.Hours > 1 ? "hrs " : "hr ") + bkTime.Minutes + " minutes";
        else if (bkTime.Hours > 0)
            bkTimeLength = bkTime.Hours + (bkTime.Hours > 1 ? "hours " : "hour ");
        else if (bkTime.Minutes > 0)
            bkTimeLength = bkTime.Minutes + bkTime.Minutes + " minutes";
        else
            bkTimeLength = bkTime.Minutes + " minutes";


        sourceDataSet.Tables[0].Rows.Add(

            DateTime.Now.ToString("d MMMM, yyyy"),

            patient == null ? "No patient found" : patient.Person.FullnameWithTitleWithoutMiddlename,
            patient == null ? "No patient found" : (patient.Person.Title.ID == 0 ? "" : patient.Person.Title.Descr),
            patient == null ? "" : patient.Person.Firstname,
            patient == null ? "" : patient.Person.Middlename,
            patient == null ? "" : patient.Person.Surname,
            patient == null ? "" : patient.Person.Gender,
            patient == null ? "" : patient.Person.Dob == DateTime.MinValue ? (object)DBNull.Value : patient.Person.Dob.ToString("d MMMM, yyyy"),
            patient == null ? "" : patient.Person.Dob == DateTime.MinValue ? (object)DBNull.Value : patient.Person.Dob.ToString("MMMM ") + Utilities.GetDateOrdinal(patient.Person.Dob.Day),

            ptConditionsText,
            
            patientAddressText,
            patientAddressTabbedText,
            //patientAddress == null                                          ? "No address found" : patientAddress.AddrLine1,
            //patientAddress == null                                          ? "" : patientAddress.AddrLine2,
            //patientAddress == null || patientAddress.AddressChannel == null ? "" : (patientAddress.AddressChannel.AddressChannelID == 1 ? "" : patientAddress.AddressChannel.DisplayName),
            //patientAddress == null || patientAddress.Suburb  == null        ? "" : patientAddress.Suburb.Name,
            //patientAddress == null || patientAddress.Suburb  == null        ? "" : patientAddress.Suburb.Postcode,
            //patientAddress == null || patientAddress.Country == null        ? "" : patientAddress.Country.Descr,
            patientPhoneText,

            pt_last_bk_date == DateTime.MinValue ? "No previous bookings found" : pt_last_bk_date.ToString("d MMMM, yyyy"),

            activeHcCard == null ? "No hc card found" : activeHcCard.CardNbr + " - " + activeHcCard.CardFamilyMemberNbr,
            activeHcCard == null ? "" : activeHcCard.CardName,
            activeHcCard == null ? (object)"" : activeHcCard.DateReferralSigned.ToString("d MMMM, yyyy"),
            epc_expire_date == DateTime.MinValue ? (object)"" : epc_expire_date.ToString("d MMMM, yyyy"),
            epc_expire_date == DateTime.MinValue ? (object)"" : epc_count_remaining,

            medicareCard == null ? "No medicare card found" : medicareCard.CardNbr + " - " + medicareCard.CardFamilyMemberNbr,
            medicareCard == null ? " " : medicareCard.CardName,
            medicareCard == null ? (object)" " : medicareCard.DateReferralSigned.ToString("d MMMM, yyyy"),

            dvaCard == null ? "No dva card found" : dvaCard.CardNbr,
            dvaCard == null ? "" : dvaCard.CardName,
            dvaCard == null ? (object)"" : dvaCard.DateReferralSigned.ToString("d MMMM, yyyy"),

            org != null ? org.Name        : site.Name,
            org != null ? org.Abn         : site.Abn,
            org != null ? org.Acn         : site.Acn,
            org != null ? org.BpayAccount : site.BankBpay,

            orgAddressText,
            orgAddressTabbedText,
            //orgAddress == null ? "No address found" : orgAddress.AddrLine1,
            //orgAddress == null ? "" : orgAddress.AddrLine2,
            //orgAddress == null ? "" : (orgAddress.AddressChannel.AddressChannelID == 1 ? "" : orgAddress.AddressChannel.DisplayName),
            //orgAddress == null ? "" : orgAddress.Suburb.Name,
            //orgAddress == null ? "" : orgAddress.Suburb.Postcode,
            //orgAddress == null || orgAddress.Country == null ? "" : orgAddress.Country.Descr,
            orgPhoneText,
            orgFaxText,
            orgWebText,
            orgEmailText,


            regReferrer == null ? "No referrer found" : regReferrer.Referrer.Person.FullnameWithTitleWithoutMiddlename,
            regReferrer == null ? "No referrer found" : (regReferrer.Referrer.Person.Title.ID == 0 ? "" : regReferrer.Referrer.Person.Title.Descr),
            regReferrer == null ? "" : regReferrer.Referrer.Person.Firstname,
            regReferrer == null ? "" : regReferrer.Referrer.Person.Middlename,
            regReferrer == null ? "" : regReferrer.Referrer.Person.Surname,
            //regReferrer == null ? "" : regReferrer.Referrer.Person.Gender,
            //regReferrer == null ? (object)"" : regReferrer.Referrer.Person.Dob.ToString("d MMMM, yyyy"),

            referrerAddressText,
            referrerAddressTabbedText,
            //referrerAddress == null ? "No address found" : referrerAddress.AddrLine1,
            //referrerAddress == null ? "" : referrerAddress.AddrLine2,
            //referrerAddress == null ? "" : (referrerAddress.AddressChannel.AddressChannelID == 1 ? "" : referrerAddress.AddressChannel.DisplayName),
            //referrerAddress == null ? "" : referrerAddress.Suburb.Name,
            //referrerAddress == null ? "" : referrerAddress.Suburb.Postcode,
            //referrerAddress == null || referrerAddress.Country == null ? "" : referrerAddress.Country.Descr,
            referrerPhoneText,
            referrerFaxText,

            booking == null ? "" : booking.DateStart.ToString("d MMMM, yyyy"),
            booking == null ? "" : booking.DateStart.ToString("HH:mm"),
            booking == null ? "" : bkTimeLength,
            

            booking == null || booking.Provider == null ? "" : booking.Provider.Person.FullnameWithTitleWithoutMiddlename,
            booking == null || booking.Provider == null ? "" : (booking.Provider.Person.Title.ID == 0 ? "" : booking.Provider.Person.Title.Descr),
            booking == null || booking.Provider == null ? "" : booking.Provider.Person.Firstname,
            booking == null || booking.Provider == null ? "" : booking.Provider.Person.Middlename,
            booking == null || booking.Provider == null ? "" : booking.Provider.Person.Surname,
            bk_prov_number,
            //booking == null || booking.Provider == null ? "" : booking.Provider.Person.Gender,
            //booking == null || booking.Provider == null ? "" : booking.Provider.Person.Dob.ToString("d MMMM, yyyy"),

            booking == null ? "" : bookingNotes,

            booking == null || booking.Offering == null ? "" : booking.Offering.Name,
            booking == null || booking.Offering == null ? "" : booking.Offering.ShortName,
            booking == null || booking.Offering == null ? "" : booking.Offering.Descr

            );


        //now merge
        string errorString = string.Empty;
        //if (!OfficeInterop.MailMerge.MergeDataWithWordTemplate(sourceTemplatePath, outputDocPath, sourceDataSet, isDoubleSidedPrinting, extraPages, out errorString))
        //    throw new CustomMessageException("Error:" + errorString);
        if (!WordMailMerger.Merge(sourceTemplatePath, outputDocPath, sourceDataSet, null, 0, false, isDoubleSidedPrinting, extraPages, true, null, out errorString))
            throw new CustomMessageException("Error:" + errorString);




        if (!historyDir.EndsWith(@"\"))
            historyDir += @"\";


        //
        // create pdf file to be stored because it's much smaller in size
        //
        string tmpLettersDirectory = Letter.GetTempLettersDirectory();
        if (!System.IO.Directory.Exists(tmpLettersDirectory))
            throw new CustomMessageException("Temp letters directory doesn't exist");
        string outputDocPathPDF = FileHelper.GetTempFileName(tmpLettersDirectory + Path.ChangeExtension(historyFileName, ".pdf"));
        string _errorString = string.Empty;
        if (keepHistoryInDB || keepHistoryInFile)
            OfficeInterop.FormatConverter.WordToPDF(outputDocPath, outputDocPathPDF, out _errorString);

        if (keepHistoryInDB)
        {
            byte[] doc_contents = System.IO.File.ReadAllBytes(outputDocPathPDF);
            int letterHistoryID = LetterPrintHistoryDB.Insert(letterID, letterPrintHistorySendMethodID, booking == null ? -1 : booking.BookingID, patient == null ? -1 : patient.PatientID, org != null ? org.OrganisationID : 0, regReferrer == null ? -1 : regReferrer.RegisterReferrerID, staff.StaffID, -1, historyFileName, doc_contents);

            if (keepHistoryInFile)
            {
                string filePath = historyDir + letterHistoryID + System.IO.Path.GetExtension(outputDocPathPDF);
                if (System.IO.File.Exists(filePath))
                    throw new CustomMessageException("File already exists: " + filePath);
                if (!Directory.Exists(historyDir))
                    Directory.CreateDirectory(historyDir);
                System.IO.File.Copy(outputDocPathPDF, filePath);
            }
        }
        else if (keepHistoryInFile)
        {
            int letterHistoryID = LetterPrintHistoryDB.Insert(letterID, letterPrintHistorySendMethodID, booking == null ? -1 : booking.BookingID, patient == null ? -1 : patient.PatientID, org != null ? org.OrganisationID : 0, regReferrer == null ? -1 : regReferrer.RegisterReferrerID, staff == null ? -1 : staff.StaffID, healthCardAction == null ? -1 : healthCardAction.HealthCardActionID, historyFileName, null);

            string filePath = historyDir + letterHistoryID + System.IO.Path.GetExtension(outputDocPathPDF);
            if (System.IO.File.Exists(filePath))
                throw new CustomMessageException("File already exists: " + filePath);
            if (!Directory.Exists(historyDir))
                Directory.CreateDirectory(historyDir);
            System.IO.File.Copy(outputDocPathPDF, filePath);
        }

        if (keepHistoryInDB || keepHistoryInFile)
            File.Delete(outputDocPathPDF);

    }

    public static void CreateMultipleCopies(int numCopies, string fileToCopy, string outputDocPath)
    {
        string[] filesToMerge = new string[numCopies];
        for (int i = 0; i < numCopies; i++)
            filesToMerge[i] = fileToCopy;

        string errorString = string.Empty;
        if (!OfficeInterop.MultiDocumentMerger.Merge(filesToMerge, outputDocPath, true, out errorString))
            throw new CustomMessageException("Error:" + errorString);

    }

    public static string MergeMultipleDocuments(string[] filesToMerge, string outputDocPath)
    {
        string errorString = string.Empty;
        string tmpFinalFileName = FileHelper.GetTempFileName(outputDocPath);
        if (!OfficeInterop.MultiDocumentMerger.Merge(filesToMerge, tmpFinalFileName, true, out errorString))
            throw new CustomMessageException("Error:" + errorString);

        return tmpFinalFileName;
    }

    public static void DownloadDocument(HttpResponse httpResponse, byte[] fileContents, string fileName)
    {
        try
        {

            string contentType = "application/octet-stream";
            try { contentType = Utilities.GetMimeType(System.IO.Path.GetExtension(fileName)); }
            catch (Exception) { }


            httpResponse.Clear();
            httpResponse.ClearHeaders();

            // add cooke so that javascript can detect when file downloaded is done and started if it want's to 
            // do something (such as letter print page to deselect leter to print)
            httpResponse.Cookies["fileDownloaded"].Value = "true";
            httpResponse.Cookies["fileDownloaded"].Expires = DateTime.Now.AddHours(3);

            httpResponse.ContentType = contentType;
            httpResponse.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            httpResponse.OutputStream.Write(fileContents, 0, fileContents.Length);
            httpResponse.Flush();
            httpResponse.End();
        }
        catch (System.Web.HttpException ex) 
        {
            // ignore exception where user closed the download box
            if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                throw;
        }
    }

    #endregion

    #region TreatmentLetterType, GetHealthCardActionTypeID

    public enum TreatmentLetterType { First, Last, LastWhenReplacingEPC, TreatmentNotes, LastPT };

    public static int GetHealthCardActionTypeID(TreatmentLetterType treatmentLetterType)
    {
        if (treatmentLetterType == TreatmentLetterType.First)
            return 2;
        else if (treatmentLetterType == TreatmentLetterType.Last || treatmentLetterType == TreatmentLetterType.LastWhenReplacingEPC)
            return 3;
        throw new CustomMessageException("Unknown TreatmentLetterType");
    }
    public static int GetLetterIDByTreatmentLetterTypeAndInvoiceType(TreatmentLetterType treatmentLetterType, Booking.InvoiceType invType, int fieldID, int siteID)
    {
        if (invType == Booking.InvoiceType.Medicare)
            return GetLetterIDByTreatmentLetterTypeAndTreatmentSubType(treatmentLetterType, fieldID, siteID);

        throw new CustomMessageException("Unknown TreatmentLetterType");
    }
    protected static int GetLetterIDByTreatmentLetterTypeAndTreatmentSubType(TreatmentLetterType treatmentLetterType, int fieldID, int siteID)
    {
        LetterTreatmentTemplate treatmentLetters = LetterTreatmentTemplateDB.GetByFieldID(fieldID, siteID);
        if (treatmentLetters == null)
            return -1; // indicates "No treatment letters set for \"" + fieldID + "\"" ... but dont need to throw error
            
        if (treatmentLetterType == TreatmentLetterType.First)
            return treatmentLetters.FirstLetter.LetterID;
        else if (treatmentLetterType == TreatmentLetterType.Last)
            return treatmentLetters.LastLetter.LetterID;
        else if (treatmentLetterType == TreatmentLetterType.LastWhenReplacingEPC)
            return treatmentLetters.LastLetterWhenReplacingEPC.LetterID;
        else if (treatmentLetterType == TreatmentLetterType.TreatmentNotes)
            return treatmentLetters.TreatmentNotesLetter.LetterID;
        else if (treatmentLetterType == TreatmentLetterType.LastPT)
            return treatmentLetters.LastLetterPT.LetterID;

        throw new CustomMessageException("Unknown TreatmentLetterType");
    }

    public static Letter.FileContents SendTreatmentLetter(FileFormat fileFormat, Booking booking, Patient patient, HealthCard hc, Letter.TreatmentLetterType treatmentLetterType, Booking.InvoiceType invType, int fieldID, int siteID, int staffID, Referrer referrer, bool keepInHistory, int letterPrintHistorySendMethodID)
    {
        // 1. Add to healthcardaction
        int healthCardActionID = -1;
        if (treatmentLetterType == Letter.TreatmentLetterType.First || treatmentLetterType == Letter.TreatmentLetterType.Last || treatmentLetterType == Letter.TreatmentLetterType.LastWhenReplacingEPC)
            healthCardActionID = HealthCardActionDB.Insert(hc.HealthCardID, Letter.GetHealthCardActionTypeID(treatmentLetterType), DateTime.Now);

        // 2.create document and put it in history
        int letterID = Letter.GetLetterIDByTreatmentLetterTypeAndInvoiceType(treatmentLetterType, invType, fieldID, siteID);
        if (letterID == -1)
            return null;
        //string[] notes = (treatmentLetterType == TreatmentLetterType.TreatmentNotes) ? booking.GetNoteTextForTreatmentLetter(referrer) : null;
        string[] notes = null;
        Letter.FileContents fileContents = Letter.CreateLetter(fileFormat, SiteDB.GetByID(siteID), letterID, booking.Organisation.OrganisationID, patient.PatientID, staffID, booking.BookingID, healthCardActionID, 1, notes, keepInHistory, letterPrintHistorySendMethodID);
        return fileContents;

        //Letter.SendLetter(Response, SiteDB.GetByID(Convert.ToInt32(Session["SiteID"])), GetLetterID(treatmentLetterType, invType), booking.Organisation.OrganisationID, booking.Patient.PatientID, Convert.ToInt32(Session["StaffID"]), booking.BookingID, healthCardActionID, 1, null);

    }

    #endregion

}