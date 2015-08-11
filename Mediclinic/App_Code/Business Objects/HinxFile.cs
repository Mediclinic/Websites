using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Xml.Linq;
using System.Configuration;
using System.Data;
using System.IO;

public class HinxFile
{

    public class ReportItems
    {
        public Invoice[] ClaimManually;
        public Invoice[] PartiallyPaidClaims;
        public Invoice[] ToBeClaimed;
        public DataTable reportTable;

        public ReportItems(Invoice[] ClaimManually, Invoice[] PartiallyPaidClaims, Invoice[] ToBeClaimed, DataTable reportTable)
        {
            this.ClaimManually       = ClaimManually;
            this.PartiallyPaidClaims = PartiallyPaidClaims;
            this.ToBeClaimed         = ToBeClaimed;
            this.reportTable         = reportTable;
        }
    }


    protected ClaimType claimType;
    protected int siteID;
    private   bool inc_deep_booking_info = false;

    public enum ClaimType { Medicare, DVA };
    public HinxFile(ClaimType claimType, int siteID)
	{
        this.claimType = claimType;
        this.siteID    = siteID;

        if (!MedicareLicenceNbrExists())
            throw new CustomMessageException("eClaim licence number missing - plase contact the system administrator.");
	}


    /*
     * medicare eclaims has a licence to use the sytem. each licence has an ADV number
     * put in config and then put the hinx files in a folder of same name
     */
    protected string MedicareLicenceNbr
    {
        get { return SystemVariableDB.GetByDescr("MedicareEclaimsLicenseNbr").Value; }
    }
    protected bool MedicareLicenceNbrExists()
    {
        return MedicareLicenceNbr != null && MedicareLicenceNbr.Length > 0;
    }
    protected string HinxDirectory
    {
        get
        {
            bool useLocalFolder = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["HinxFolderUseLocal"]);
            string folder = ConfigurationManager.AppSettings[(useLocalFolder ? "HinxLocalFolder" : "HinxNetworkFolder")];
            folder = folder.Replace("[DBNAME]", System.Web.HttpContext.Current.Session["DB"].ToString());

            if (!folder.EndsWith(@"\"))
                folder = folder + @"\";
            folder += MedicareLicenceNbr + @"\";
            return folder;
        }
    }
    protected DateTime CutOffDate
    {
        get {  return DateTime.Today.AddYears(-2); }
    }

    public string[] GetExistingHinxFiles(DataTable invoices)
    {
        string hinxDirectory = HinxDirectory;
        ArrayList existingHinxFiles = new ArrayList();
        for (int i = 0; i < invoices.Rows.Count; i++)
        {
            Invoice invoice = InvoiceDB.LoadAll(invoices.Rows[i], this.inc_deep_booking_info);
            if (File.Exists(hinxDirectory + invoice.HealthcareClaimNumber + ".hinx"))
                existingHinxFiles.Add(invoice.HealthcareClaimNumber);
        }

        return (string[])existingHinxFiles.ToArray(typeof(string));
    }
    protected Invoice[] GetInvoicesTooOldToClaim(Invoice[] invoices)
    {
        DateTime CutOffDate = this.CutOffDate;
        ArrayList list = new ArrayList();
        foreach (Invoice invoice in invoices)
            if (invoice.InvoiceDateAdded <= CutOffDate)
                list.Add(invoice);
        return (Invoice[])list.ToArray(typeof(Invoice));
    }

    public ReportItems GenerateReportItems(DateTime fromDate, DateTime toDate, bool validate = true)  // only doing medicare or dva at a time
    {
        if (!MedicareLicenceNbrExists())
            throw new CustomMessageException("eClaim licence number missing - plase contact the system administrator.");


        DateTime  CutOffDate          = this.CutOffDate;

        ArrayList ClaimManually       = new ArrayList();
        ArrayList PartiallyPaidClaims = new ArrayList();
        ArrayList ToBeClaimed         = new ArrayList();


        //int startTimeGetFromDB = Environment.TickCount;
        DataTable invoices = InvoiceDB.GetUnclaimedMedicareInvoices(this.inc_deep_booking_info, this.claimType == ClaimType.Medicare ? -1 : -2, this.siteID, fromDate, toDate);
        //double executionTimeGetFromDB = (double)(Environment.TickCount - startTimeGetFromDB) / 1000.0;

        if (validate)
        {
            string[] existingHinxFiles = GetExistingHinxFiles(invoices);
            if (existingHinxFiles.Length > 0)
            {
                int itemsPerLine = 8;
                string existing = string.Empty;
                for (int i = 0; i < existingHinxFiles.Length; i++)
                    existing += (i != 0 && i % itemsPerLine == 0 ? "<br />" : "") + existingHinxFiles[i] + (i != existingHinxFiles.Length-1 ? ", " : "");
                throw new CustomMessageException("Can not generate while the following eclaim files already exist : " + "<br />" + string.Join("<br />", existing));
            }
        }



        invoices.Columns.Add("display_group_id", typeof(int)); 
        invoices.Columns.Add("display_group_descr", typeof(string));
        for (int i = 0; i < invoices.Rows.Count; i++)
        {
            Invoice invoice = InvoiceDB.LoadAll(invoices.Rows[i], this.inc_deep_booking_info);

            if (invoice.Booking.DateStart <= CutOffDate)
            {
                // too old to claim 
                ClaimManually.Add(invoice);
                invoices.Rows[i]["display_group_id"]    = 4;
                invoices.Rows[i]["display_group_descr"] = "Claim Manually";
            }
            else if (invoice.CreditNotesTotal > 0 || invoice.ReceiptsTotal > 0)
            {
                // already paritially paid
                PartiallyPaidClaims.Add(invoice);
                invoices.Rows[i]["display_group_id"]    = 3;
                invoices.Rows[i]["display_group_descr"] = "Partially Paid Claims";
            }
            else
            {
                ToBeClaimed.Add(invoice);
                invoices.Rows[i]["display_group_id"]    = 1;
                invoices.Rows[i]["display_group_descr"] = "To Be Claimed";
            }
        }

        DataView dv = invoices.DefaultView;
        dv.Sort = "display_group_id ASC, inv_invoice_id ASC";
        invoices = dv.ToTable();


        ReportItems reportItems = new ReportItems(
             (Invoice[])ClaimManually.ToArray(typeof(Invoice)),
             (Invoice[])PartiallyPaidClaims.ToArray(typeof(Invoice)),
             (Invoice[])ToBeClaimed.ToArray(typeof(Invoice)),
             invoices
        );

        return reportItems;
    }
    protected int[] GetAllPatientIDs(InvoiceLine[] invoiceLines)
    {
        Hashtable allPatientIDsHash = new Hashtable();
        foreach (InvoiceLine invoiceLine in invoiceLines)
            allPatientIDsHash[invoiceLine.Patient.PatientID] = 1;

        int[] allPatientIDsList = new int[allPatientIDsHash.Keys.Count];
        int i = 0;
        foreach (DictionaryEntry pair in allPatientIDsHash)
            allPatientIDsList[i++] = (int)pair.Key;

        return allPatientIDsList;
    }
    protected int[] GetAllProviderStaffIDs(Invoice[] invoices)
    {
        Hashtable allStaffIDsHash = new Hashtable();
        foreach (Invoice invoice in invoices)
            allStaffIDsHash[invoice.Booking.Provider.StaffID] = 1;

        int[] allStaffIDsList = new int[allStaffIDsHash.Keys.Count];
        int i = 0;
        foreach (DictionaryEntry pair in allStaffIDsHash)
            allStaffIDsList[i++] = (int)pair.Key;

        return allStaffIDsList;
    }

    public void CreateXML(Invoice[] invoices, bool validate = true)
    {
        if (validate)
        {
            Invoice[] tooOldList = GetInvoicesTooOldToClaim(invoices);
            if (tooOldList.Length > 0)
            {
                string invalids = string.Empty;
                foreach(Invoice i in tooOldList)
                    invalids += (invalids.Length == 0 ? "" : ",") + i.InvoiceID.ToString();
                throw new Exception("The following invoices are too old to claim: " + "<br />" + invalids);

            }
        }

        // get bulk invoice lines for less db calls in individual invoice create xml method  [invoiceID => list of invoicelines]
        Hashtable bulkInvoiceLineHash = InvoiceLineDB.GetBulkInvoiceLinesByInvoiceID(invoices);

        ArrayList allInvoiceLines = new ArrayList();
        foreach (DictionaryEntry pair in bulkInvoiceLineHash)
            allInvoiceLines.AddRange((InvoiceLine[])pair.Value);

        // get bluk health cards  [patientID=>healthcard]
        //
        // NB:
        // A DVA invoice can only use a DVA card
        // A Medicare invoice can only use a Medicare card
        // The system can only create a DVA invoice is if DVA is set as the active card (vice versa for Medicare)
        // So when a DVA invoice is created the DVA card was active, and then someone switches it to be the Medicare card thatis active.
        // So, it's correct to get only the DVA cards for DVA invoices (and Medicare cards for Medicare invoices), and also would be correct to ignore the active flag and just get the most recent.
        int[] allPatientIDs = GetAllPatientIDs((InvoiceLine[])allInvoiceLines.ToArray(typeof(InvoiceLine)));
        Hashtable bulkHealthCardHash = PatientsHealthCardsCacheDB.GetBullkMostRecent(allPatientIDs, claimType == ClaimType.Medicare ? -1 : -2);
        
        // get bluk staff provider numbers from registerstaff table
        int[] allProviderStaffIDs = GetAllProviderStaffIDs(invoices);
        Hashtable bulkRegisterStaffHash = RegisterStaffDB.Get2DHashByStaffIDOrgID(allProviderStaffIDs);
        Hashtable bulkStaffHash = StaffDB.GetAllInHashtable(false, true, false, false);

        // get bluk healthcard actions to get EPC signed dates
        Hashtable bulkHealthCardActionsHash = HealthCardActionDB.GetReceivedActionsByPatientIDs(allPatientIDs);

        // get bluk epcreferrers
        Hashtable bulkEPCReferrersHash = PatientReferrerDB.GetEPCReferrersOf(allPatientIDs, false);

        // get all sites in one call
        Hashtable bulkSites = SiteDB.GetAllInHashtable();


        Hashtable claimNumberInvoiceGroups = new Hashtable();
        for (int i = 0; i < invoices.Length; i++)
        {
            if (claimNumberInvoiceGroups[(invoices[i]).HealthcareClaimNumber] == null)
                claimNumberInvoiceGroups[(invoices[i]).HealthcareClaimNumber] = new ArrayList();

            ((ArrayList)claimNumberInvoiceGroups[(invoices[i]).HealthcareClaimNumber]).Add(invoices[i]);
        }


        string noPatientFailures    = string.Empty;
        string noHealthcardFailures = string.Empty;
        foreach (string claimNbr in claimNumberInvoiceGroups.Keys)
        {
            Invoice[] invoiceList = (Invoice[])((ArrayList)claimNumberInvoiceGroups[claimNbr]).ToArray(typeof(Invoice));

            try
            {
                CreateXML(invoiceList, bulkInvoiceLineHash, bulkHealthCardHash, bulkRegisterStaffHash, bulkStaffHash, bulkSites, bulkHealthCardActionsHash, bulkEPCReferrersHash);
            }
            catch (HINXNoPatientOnInvoiceLineException ex)
            {
                noPatientFailures += (noPatientFailures.Length == 0 ? "" : "<br />") + ex.Message;
            }
            catch (HINXNoHealthcardException ex)
            {
                noHealthcardFailures += (noHealthcardFailures.Length == 0 ? "" : "<br />") + ex.Message;
            }
        }




        string errors = string.Empty;
        if (noPatientFailures.Length > 0)
            errors += (errors.Length == 0 ? "" : "<br /><br />") + "The following invoices have invoices lines with no patient set (Fix this and re-generate): <br />" + noPatientFailures;
        if (noHealthcardFailures.Length > 0)
            errors += (errors.Length == 0 ? "" : "<br /><br />") + "The following invoices have patients with no " + (claimType == ClaimType.Medicare ? "Medicare" : "DVA") + " card set (Fix this and re-generate): <br />" + noHealthcardFailures;
        if (errors.Length > 0)
            throw new HINXUnsuccessfulItemsException(errors);
    }
    protected void CreateXML(Invoice[] invoices, Hashtable bulkInvoiceLineHash, Hashtable bulkHealthCardHash = null, Hashtable bulkRegisterStaffHash = null, Hashtable bulkStaffHash = null, Hashtable bulkSites = null, Hashtable bulkHealthCardActionsHash = null, Hashtable bulkEPCReferrersHash = null)
    {
        if (!MedicareLicenceNbrExists())
            throw new CustomMessageException("eClaim licence number missing - plase contact the system administrator.");

        XElement root = new XElement("RootData");


        ArrayList claims = new ArrayList();
        for (int i = 0; i < invoices.Length; i++)
        {
            InvoiceLine[] invoiceLines = (InvoiceLine[])bulkInvoiceLineHash[invoices[i].InvoiceID];
            ArrayList invClaims = CreateClaims(invoices[i], invoiceLines, bulkHealthCardHash, bulkRegisterStaffHash, bulkStaffHash, bulkSites, bulkHealthCardActionsHash, bulkEPCReferrersHash);
            claims.AddRange(invClaims);
        }
        for (int i = 0; i < claims.Count; i++)
        {
            root.Add((XElement)claims[i]);
        }


        bool useLocalFolder = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["HinxFolderUseLocal"]);
        if (useLocalFolder)
        {
            if (!System.IO.Directory.Exists(HinxDirectory))
                System.IO.Directory.CreateDirectory(HinxDirectory);
            string outputFile = HinxDirectory + invoices[0].HealthcareClaimNumber + ".hinx";
            using (StreamWriter SW = new StreamWriter(outputFile, false))
            {
                SW.WriteLine(root.ToString());
            }
        }
        else  // use network folder
        {
            string username = System.Configuration.ConfigurationManager.AppSettings["HinxNetworkFolderUsername"];
            string password = System.Configuration.ConfigurationManager.AppSettings["HinxNetworkFolderPassword"];
            string networkName = System.Configuration.ConfigurationManager.AppSettings[(useLocalFolder ? "HinxLocalFolder" : "HinxNetworkFolder")];
            networkName = networkName.Replace(@"[DBNAME]", ""); // remove the db name, use that for hinx folder, not for the network name
            while (networkName.EndsWith(@"\"))
                networkName = networkName.Substring(0, networkName.Length - 1);

            using (new NetworkConnection(networkName, new System.Net.NetworkCredential(username, password)))
            {
                if (!System.IO.Directory.Exists(HinxDirectory))
                    System.IO.Directory.CreateDirectory(HinxDirectory);
                string outputFile = HinxDirectory + invoices[0].HealthcareClaimNumber + ".hinx";
                using (StreamWriter SW = new StreamWriter(outputFile, false))
                {
                    SW.WriteLine(root.ToString());
                }
            }
        }

        //string path = System.Web.HttpContext.Current.Server.MapPath(@"\\2008ts\portal");

    }
    protected void CreateXML(Invoice invoice, InvoiceLine[] invoiceLines, Hashtable bulkHealthCardHash = null, Hashtable bulkRegisterStaffHash = null, Hashtable bulkStaffHash = null, Hashtable bulkSites = null, Hashtable bulkHealthCardActionsHash = null, Hashtable bulkEPCReferrersHash = null)
    {
        if (!MedicareLicenceNbrExists())
            throw new CustomMessageException("eClaim licence number missing - plase contact the system administrator.");

        XElement root = new XElement("RootData");


        ArrayList claims = CreateClaims(invoice, invoiceLines, bulkHealthCardHash, bulkRegisterStaffHash, bulkStaffHash, bulkSites, bulkHealthCardActionsHash, bulkEPCReferrersHash);
        for(int i=0; i<claims.Count; i++)
        {
            root.Add((XElement)claims[i]);
        }


        bool useLocalFolder = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["HinxFolderUseLocal"]);
        if (useLocalFolder)
        {
            if (!System.IO.Directory.Exists(HinxDirectory))
                System.IO.Directory.CreateDirectory(HinxDirectory);
            string outputFile = HinxDirectory + invoice.HealthcareClaimNumber + ".hinx";
            using (StreamWriter SW = new StreamWriter(outputFile, false))
            {
                SW.WriteLine(root.ToString());
            }
        }
        else  // use network folder
        {
            string username    = System.Configuration.ConfigurationManager.AppSettings["HinxNetworkFolderUsername"];
            string password    = System.Configuration.ConfigurationManager.AppSettings["HinxNetworkFolderPassword"];
            string networkName = System.Configuration.ConfigurationManager.AppSettings[(useLocalFolder ? "HinxLocalFolder" : "HinxNetworkFolder")];
            networkName = networkName.Replace(@"[DBNAME]", ""); // remove the db name, use that for hinx folder, not for the network name
            while (networkName.EndsWith(@"\"))
                networkName = networkName.Substring(0, networkName.Length - 1);

            using (new NetworkConnection(networkName, new System.Net.NetworkCredential(username, password)))
            {
                if (!System.IO.Directory.Exists(HinxDirectory))
                    System.IO.Directory.CreateDirectory(HinxDirectory);
                string outputFile = HinxDirectory + invoice.HealthcareClaimNumber + ".hinx";
                using (StreamWriter SW = new StreamWriter(outputFile, false))
                {
                    SW.WriteLine(root.ToString());
                }
            }
        }

        //string path = System.Web.HttpContext.Current.Server.MapPath(@"\\2008ts\portal");

    }

    protected ArrayList CreateClaims(Invoice invoice, InvoiceLine[] invoiceLines, Hashtable bulkHealthCardHash = null, Hashtable bulkRegisterStaffHash = null, Hashtable bulkStaffHash = null, Hashtable bulkSites = null, Hashtable bulkHealthCardActionsHash = null, Hashtable bulkEPCReferrersHash = null)
    {
        ArrayList claims = new ArrayList();

        invoice.Booking.Provider = StaffDB.GetByID(invoice.Booking.Provider.StaffID);

        bool[] isDuplicate = new bool[invoiceLines.Length];  // IF EITHER OF PREV OR NEXT IS SAME .. YES .. ELSE NO
        for (int i = 0; i < invoiceLines.Length; i++)
        {
            if (i == 0 && i == invoiceLines.Length - 1)
                isDuplicate[i] = false;
            else if (i == 0)
                isDuplicate[i] = invoiceLines[i].Patient.PatientID == invoiceLines[i + 1].Patient.PatientID;
            else if (i == invoiceLines.Length - 1)
                isDuplicate[i] = invoiceLines[i].Patient.PatientID == invoiceLines[i - 1].Patient.PatientID;
            else
                isDuplicate[i] = invoiceLines[i].Patient.PatientID == invoiceLines[i + 1].Patient.PatientID || invoiceLines[i].Patient.PatientID == invoiceLines[i - 1].Patient.PatientID;
        }



        for (int i = 0; i < invoiceLines.Length; i++)
        {
            InvoiceLine invoiceLine = invoiceLines[i];

            if (invoiceLine.Patient.PatientID == -1)
                throw new HINXNoPatientOnInvoiceLineException("Invoice: " + invoice.InvoiceID + " Claim Nbr: " + invoice.HealthcareClaimNumber);

            HealthCard hc = GetHealthCareCard(bulkHealthCardHash, invoiceLine.Patient.PatientID);  // usees bulk db result
            if (hc == null)
                throw new HINXNoHealthcardException("Invoice: " + invoice.InvoiceID + " Claim Nbr: " + invoice.HealthcareClaimNumber + " InvoiceLine: " + invoiceLine.InvoiceLineID + " PT: " + invoiceLine.Patient.PatientID + "(" + invoiceLine.Patient.Person.FullnameWithoutMiddlename + ")");
            System.Web.UI.Pair name = GetFirstLastNames(hc.CardName, invoiceLine.Patient);                   // no db used
            string staffProviderNumber = GetStaffProviderNumber(bulkRegisterStaffHash, bulkStaffHash, bulkSites, invoice);                // usees bulk db result
            string referrerProviderNbr = GetReferrerProviderNumber(bulkEPCReferrersHash, invoiceLine);          // usees bulk db result
            
            DateTime epcSignedDate = DateTime.MinValue; // not used for GP invoices
            if (invoice.Booking.Provider.Field.ID != 1)
                epcSignedDate = //claimType == ClaimType.DVA ? hc.DateReferralSigned :   // if aged care, use date signed as they never renew dva epc's. but in clinics, need to get most recent hc action date
                                         GetEPCDateSigned(bulkHealthCardActionsHash, invoiceLine.Patient.PatientID, invoice.Booking.DateStart.Date);  // usees bulk db result
            // apparently it is still renewed.... so run the function GetEPCDateSigned(...) for both


            // -- start creating xml -- //

            XElement claim = new XElement("Claim");
            claim.Add(new XElement("TypeOfService", this.claimType == ClaimType.Medicare ? "M" : "V"));
            if (this.claimType == ClaimType.DVA)
                claim.Add(new XElement("VaaServiceTypeCde", "J"));
            claim.Add(new XElement("ServiceTypeCde", invoice.Booking.Provider.Field.ID == 1 ? "G" : "S"));
            claim.Add(new XElement("ExtServicingDoctor", staffProviderNumber));


            XElement voucher = new XElement("Voucher");
            voucher.Add(new XElement("BenefitAssignmentAuthorised", "Y"));
            voucher.Add(new XElement("ExternalPatientId", invoiceLine.Patient.PatientID));
            voucher.Add(new XElement("ExternalInvoice", invoice.InvoiceID));
            voucher.Add(new XElement("PatientFamilyName", ((string)name.Second).ToUpper()));
            voucher.Add(new XElement("PatientFirstName", ((string)name.First).ToUpper()));
            voucher.Add(new XElement("PatientDateOfBirth", invoiceLine.Patient.Person.Dob == DateTime.MinValue ? "" : invoiceLine.Patient.Person.Dob.ToString("dd'/'MM'/'yyyy")));
            if (this.claimType == ClaimType.DVA && invoiceLine.Patient.Person.Gender.Length > 0)
                voucher.Add(new XElement("PatientGender", invoiceLine.Patient.Person.Gender));
            voucher.Add(new XElement(this.claimType == ClaimType.Medicare ? "PatientMedicareCardNum" : "VeteranFileNum", hc.CardNbr));
            if (this.claimType == ClaimType.Medicare)
                voucher.Add(new XElement("PatientReferenceNum", hc.CardFamilyMemberNbr));
            voucher.Add(new XElement("DateOfService", invoice.Booking.DateStart.ToString("dd'/'MM'/'yyyy")));
            
            if (invoice.Booking.Provider.Field.ID != 1)  // if provider is not GP
            {
                voucher.Add(new XElement("ReferringProviderNum", referrerProviderNbr));
                voucher.Add(new XElement("ReferralIssueDate", epcSignedDate.ToString("dd'/'MM'/'yyyy")));
                voucher.Add(new XElement("ReferralPeriodTypeCde", "S"));
            }

            voucher.Add(new XElement("BClmAmt", invoiceLine.Price));

            /*
             * Used breifly in BEST, but removed 3 days after going live with this software.

            if (this.claimType == ClaimType.DVA && invoiceLine.Offering.OfferingType.ID == 398)  // DVA home visit
            {
                //  home visits set to be so that 
                // - first invoice line says it was for a home visit (ie offering type id = 398)
                // - next invoice line says if has a travel component (ie offering type id = 399) saying how many km's travelled and the visitation code
                //
                //  ie IF an invoice line has offering type 398 AND IF next invoice line has offering type 399, that is info belonging to first invoice line
                //  - so need to fucking look at next invoice line
                //  - then when done this methods - skip that invoice line!!


                // in BEST he has to get the Offering.DvaVisitType code, and that points to a kpi id row and the code is in kpi descr
                // but as I explained in the definition of Create Table Offering, this is idiotic and we will store it directly in that field
                voucher.Add(new XElement("TreatmentLocationCde", invoiceLine.Offering.DvaVisitType));

                // if has travel component
                if (i + 1 < invoiceLines.Length && invoiceLines[i + 1].Offering.OfferingType.ID == 399)
                {
                    voucher.Add(new XElement("DistanceKms", invoiceLines[i + 1].Quantity));  // travel component is on next invoice line
                    i++;  // move past this in list of invoice lines
                }
            }
            */

            voucher.Add(new XElement("NumberItems", "1"));
            if (this.claimType == ClaimType.DVA)
                voucher.Add(new XElement("TimeOfService", invoice.Booking.DateStart.ToString("HHmm")));
            voucher.Add(AddServiceTag(invoiceLine, invoice, this.claimType, isDuplicate[i]));


            claim.Add(voucher);
            claims.Add(claim);
        }

        return claims;
    }
    protected XElement AddServiceTag(InvoiceLine invoiceLine, Invoice invoice, ClaimType claimType, bool isDuplicate)
    {
        if (invoice.Booking.Provider.Field.ID != 1)  // allied health, not GP
        {
            if (claimType == ClaimType.Medicare)
            {
                return new XElement("Service",
                            new XElement("ItemNum", this.claimType == ClaimType.Medicare ? invoiceLine.Offering.MedicareCompanyCode : invoiceLine.Offering.DvaCompanyCode),
                            new XElement("ChargeAmount", invoiceLine.Price)
                );
            }
            else // DVA
            {
                invoiceLine.AreaTreated = invoiceLine.AreaTreated.Trim();
                string areaTreated = invoiceLine.AreaTreated.Length > 0 ? invoiceLine.AreaTreated + ", " : "";

                return new XElement("Service",
                            new XElement("ItemNum", this.claimType == ClaimType.Medicare ? invoiceLine.Offering.MedicareCompanyCode : invoiceLine.Offering.DvaCompanyCode),
                            new XElement("ChargeAmount", invoiceLine.Price),
                            new XElement("DuplicateServiceOverrideInd", isDuplicate ? "Y" : "N"),
                            new XElement("ServiceText", areaTreated + (invoice.Booking.DateStart.Hour < 12 ? "AM" : "PN") + " visit " + invoice.Booking.DateStart.ToString("HH:mm") + (invoice.Booking.DateStart.Hour < 12 ? "am" : "pm"))
                );
            }
        }
        else
        {
            if (claimType == ClaimType.Medicare)  // GP, not allied health
            {
                return new XElement("Service",
                            new XElement("ItemNum", this.claimType == ClaimType.Medicare ? invoiceLine.Offering.MedicareCompanyCode : invoiceLine.Offering.DvaCompanyCode),
                            new XElement("NoOfPatientsSeen", "0"),
                            new XElement("AfterCareOverrideInd", "N"),
                            new XElement("DuplicateServiceOverrideInd", "N"),
                            new XElement("MultipleProcedureOverrideInd", "N"),
                            new XElement("HospitalInd", "N"),
                            new XElement("ChargeAmount", invoiceLine.Price)
                );
            }
            else // DVA
            {
                invoiceLine.AreaTreated = invoiceLine.AreaTreated.Trim();
                string areaTreated = invoiceLine.AreaTreated.Length > 0 ? invoiceLine.AreaTreated + ", " : "";

                return new XElement("Service",
                            new XElement("ItemNum", this.claimType == ClaimType.Medicare ? invoiceLine.Offering.MedicareCompanyCode : invoiceLine.Offering.DvaCompanyCode),
                            new XElement("NoOfPatientsSeen", "0"),
                            new XElement("AfterCareOverrideInd", "N"),
                            new XElement("DuplicateServiceOverrideInd", "N"),
                            new XElement("MultipleProcedureOverrideInd", "N"),
                            new XElement("HospitalInd", "N"),
                            new XElement("ChargeAmount", invoiceLine.Price),
                            new XElement("DuplicateServiceOverrideInd", isDuplicate ? "Y" : "N"),
                            new XElement("ServiceText", areaTreated + (invoice.Booking.DateStart.Hour < 12 ? "AM" : "PN") + " visit " + invoice.Booking.DateStart.ToString("HH:mm") + (invoice.Booking.DateStart.Hour < 12 ? "am" : "pm"))
                );
            }
        }

    }

    protected System.Web.UI.Pair GetFirstLastNames(string hcCardName, Patient patient)
    {
        if (hcCardName.Length == 0)
            return new System.Web.UI.Pair(patient.Person.Firstname, patient.Person.Surname);

        hcCardName = hcCardName.Trim();

        // if 2 spaces seperate by 2nd space  eg "mary p. smith", else seperate bt first space
        int posFirstSpace = hcCardName.IndexOf(' ');
        int posSecondSpace = posFirstSpace == -1 ? -1 : hcCardName.IndexOf(' ', posFirstSpace + 1);
        if (posSecondSpace != -1 || posFirstSpace != -1)
        {
            int posSpace = posSecondSpace != -1 ? posSecondSpace : posFirstSpace;
            string firstname = hcCardName.Substring(0, posSpace);
            string surname = hcCardName.Length > (posSpace + 1) ? hcCardName.Substring(posSpace + 1) : string.Empty;
            return new System.Web.UI.Pair(firstname, surname);
        }
        else
        {
            return new System.Web.UI.Pair(string.Empty, hcCardName);
        }
    }
    protected HealthCard GetHealthCareCard(Hashtable bulkHealthCardHash, int patientID)
    {
        if (bulkHealthCardHash != null) // get from bulk cache of healthcards of all patients
        {
            PatientActiveHealthCards healthCards = (PatientActiveHealthCards)bulkHealthCardHash[patientID];
            if (healthCards == null)
                return null;
            else
                return this.claimType == ClaimType.Medicare ? healthCards.MedicareCard : healthCards.DVACard;
        }
        else
        {
            HealthCard[] cards = HealthCardDB.GetAllByPatientID(patientID, false, this.claimType == ClaimType.Medicare ? -1 : -2);
            return cards.Length > 0 ? cards[cards.Length - 1] : null;
        }
    }
    protected DateTime GetEPCDateSigned(Hashtable bulkHealthCardActions, int patientID, DateTime bookingDate)
    {
        if (bulkHealthCardActions != null) // get from bulk cache of healthcardactions of all patients
        {
            HealthCardAction[] healthCardActions = (HealthCardAction[])bulkHealthCardActions[patientID];
            return HealthCardActionDB.GetEPCDateSigned(healthCardActions, bookingDate);
        }
        else
        {
            return HealthCardActionDB.GetEPCDateSigned(patientID, bookingDate);
        }
    }
    protected string GetStaffProviderNumber(Hashtable bulkRegisterStaffHash, Hashtable bulkStaffHash, Hashtable bulkSites, Invoice invoice)
    {
        bool isClinicSite = ((Site)bulkSites[invoice.Site.SiteID]).SiteType.ID == 1;

        if (!isClinicSite)       // aged care use prov number from staff table
        {
            // return invoice.Booking.Provider.ProviderNumber; // doesnt have all provider info loaded from the db

            return (bulkStaffHash != null) ?
                ((Staff)bulkStaffHash[invoice.Booking.Provider.StaffID]).ProviderNumber :
                StaffDB.GetByID(invoice.Booking.Provider.StaffID).ProviderNumber;
        }
        else  // clinic use prov number specific to that clinic
        {
            if (bulkRegisterStaffHash != null)  // use cached bulk preload to avoid excess db calls
            {
                if (bulkRegisterStaffHash[new Hashtable2D.Key(invoice.Booking.Provider.StaffID, invoice.Booking.Organisation.OrganisationID)] == null)
                {
                    // normally doesn't pull back this info, so retrieve it for error info
                    //invoice.Booking.Provider = StaffDB.GetByID(invoice.Booking.Provider.StaffID); 
                    //invoice.Booking.Organisation = OrganisationDB.GetByID(invoice.Booking.Organisation.OrganisationID);
                    //string msg = @"For invoice " + invoice.InvoiceID + @" - can not get provider number for <br />&nbsp;&nbsp;" + invoice.Booking.Provider.Person.FullnameWithoutMiddlename + @" (StaffID: " + invoice.Booking.Provider.StaffID + @")<br />at<br />&nbsp;&nbsp;" + invoice.Booking.Organisation.Name + @" (OrgID: " + invoice.Booking.Organisation.OrganisationID + @")<br />becuase they are not registered to this clinic, and the provider number for clinic invoices is stored there.";
                    //throw new CustomMessageException(msg);


                    // Marcus wants it generated with empty provider number, and when rejected, they will fix it
                    return string.Empty;
                }
                RegisterStaff regStaff = (RegisterStaff)bulkRegisterStaffHash[new Hashtable2D.Key(invoice.Booking.Provider.StaffID, invoice.Booking.Organisation.OrganisationID)];
                return regStaff.ProviderNumber;
            }
            else
            {
                RegisterStaff regStaff = RegisterStaffDB.GetByStaffIDAndOrganisationID(invoice.Booking.Provider.StaffID, invoice.Booking.Organisation.OrganisationID);
                if (regStaff == null)
                {
                    //string msg = @"For invoice " + invoice.InvoiceID + @" - can not get provider number for <br />&nbsp;&nbsp;" + invoice.Booking.Provider.Person.FullnameWithoutMiddlename + @" (StaffID: " + invoice.Booking.Provider.StaffID + @")<br />at<br />&nbsp;&nbsp;" + invoice.Booking.Organisation.Name + @" (OrgID: " + invoice.Booking.Organisation.OrganisationID + @")<br />becuase they are not registered to this clinic, and the provider number for clinic invoices is stored there.";
                    //throw new CustomMessageException(msg);


                    // Marcus wants it generated with empty provider number, and when rejected, they will fix it
                    return string.Empty;
                }
                return regStaff.ProviderNumber;
            }
        }
    }
    protected string GetReferrerProviderNumber(Hashtable bulkEPCReferrersHash, InvoiceLine invoiceLine)
    {
        if (bulkEPCReferrersHash != null)  // use cache bulk preload to avoid excess db calls
        {
            if (bulkEPCReferrersHash[invoiceLine.Patient.PatientID] == null)
                return string.Empty;

            RegisterReferrer[] registerReferrer = (RegisterReferrer[])bulkEPCReferrersHash[invoiceLine.Patient.PatientID];
            return (registerReferrer.Length > 0) ? registerReferrer[registerReferrer.Length-1].ProviderNumber : string.Empty;
        }
        else
        {
            PatientReferrer[] patientReferrer = PatientReferrerDB.GetEPCPatientReferrersOf(invoiceLine.Patient.PatientID);
            return (patientReferrer.Length > 0) ? patientReferrer[patientReferrer.Length-1].RegisterReferrer.ProviderNumber : string.Empty;
        }
    }

}