using System;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using TransmitSMSAPI;
using System.Data;

public class ServiceSpecificBookingReminderLettersSending
{

    #region Run

    protected enum SendMethod { Batch, Email_To_Patient, None };
    public static string Run(bool incDisplay, bool incSending, bool incPtSending, DateTime date)
    {
        date = date.Date;

        string batchEmail = SystemVariableDB.GetByDescr("ServiceSpecificBookingReminderLettersToBatch_EmailAddress").Value;

        // don't actually run it if email empty (ie deactivated)
        incSending = incSending && batchEmail.Length > 0;

        Site[] sites = SiteDB.GetAll();


        string output = string.Empty;

        Hashtable lettersHash = LetterDB.GetHashTable();
        Offering[] offerings  = OfferingDB.GetAll(false, -1);
        for (int j = 0; j < offerings.Length; j++)
        {
            if (offerings[j].ReminderLetterMonthsLaterToSend == 0 || offerings[j].ReminderLetterID == -1)
                continue;

            Booking[] bookings = BookingDB.GetWhenLastServiceFromXMonthsAgoToGenerageReminderLetter(offerings[j].OfferingID, date, offerings[j].ReminderLetterMonthsLaterToSend);


            Hashtable distinctPatients = new Hashtable();
            for (int i = 0; i < bookings.Length; i++)
                if (bookings[i].Patient != null && distinctPatients[bookings[i].Patient.PatientID] == null)
                    distinctPatients[bookings[i].Patient.PatientID] = bookings[i].Patient;

            Patient[] patients                   = (Patient[])(new ArrayList(distinctPatients.Values)).ToArray(typeof(Patient));
            Hashtable patientContactEmailHash    = GetPatientEmailCache(patients);



            // Generate Letters

            ArrayList filesToPrint = new ArrayList();
            for (int i = 0; i < bookings.Length; i++)
            {
                Booking curBooking = bookings[i];
                if (curBooking.Patient == null)
                    continue;

                Patient curPatient         = curBooking.Patient;
                string  curPatientEmail    = GetEmail(patientContactEmailHash, curPatient.Person.EntityID);
                bool    curPatientHasEmail = curPatientEmail != null;


                SendMethod sendMethod = incPtSending && curPatientHasEmail ? SendMethod.Email_To_Patient : SendMethod.Batch;

                if (incSending)
                {
                    if (sendMethod == SendMethod.Email_To_Patient)
                    {
                        // generate and send email
                        Letter.FileContents fileContents = GenerteLetter(curBooking, Letter.FileFormat.PDF, lettersHash, sites);
                        fileContents.DocName = "Reminder" + System.IO.Path.GetExtension(fileContents.DocName);
                        if (fileContents != null)
                        {
                            Site site = SiteDB.GetSiteByType(curBooking.Organisation.IsAgedCare ? SiteDB.SiteType.AgedCare : SiteDB.SiteType.Clinic);
                            SendEmail(site.Name, curPatientEmail, "Important Reminder", "Hi " + curBooking.Patient.Person.Firstname + ",<br /><br />Please find attached a review reminder letter for a previous appointment.<br /><br/>Best regards,<br />" + site.Name, true, new Letter.FileContents[] { fileContents });
                        }
                    }
                    else
                    {
                        // generate and add to batch list (if batch email set)
                        if (batchEmail.Length > 0)
                        {
                            Letter.FileContents fileContents = GenerteLetter(curBooking, Letter.FileFormat.Word, lettersHash, sites);
                            if (fileContents != null)
                                filesToPrint.Add(fileContents);
                        }
                    }
                }

                string addEditContactListPage;
                if (Utilities.GetAddressType().ToString() == "Contact")
                    addEditContactListPage = "AddEditContactList.aspx";
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                    addEditContactListPage = "ContactAusListV2.aspx";
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

                string allFeatures     = "dialogWidth:555px;dialogHeight:350px;center:yes;resizable:no; scroll:no";
                string onclick         = "onclick=\"javascript:window.showModalDialog('"+addEditContactListPage+"?entity_type=referrer&id=" + curBooking.Patient.Person.EntityID.ToString() + "', '', '" + allFeatures + "');document.getElementById('btnUpdateList').click();return false;\"";
                string hrefUpdateEmail = "<u><a style=\"text-decoration: none\" title=\"Edit\" AlternateText=\"Edit\" " + onclick + " href=\"\">Update PT Email</a></u>";

                output += @"<tr>
                                <td class=""nowrap"">" + curBooking.BookingID + " &nbsp;&nbsp;&nbsp;[" + curBooking.DateStart.ToString("dd-MM-yyyy") + "&nbsp;&nbsp;&nbsp;" + curBooking.DateStart.ToString("HH:mm") + "-" + curBooking.DateEnd.ToString("HH:mm") + "]" + @"</td>
                                <td class=""text_left"">" + curBooking.Organisation.Name + @"</td>
                                <td class=""text_left"">" + curBooking.Offering.Name + @"</td>
                                <td class=""text_left"">" + ((Letter)lettersHash[curBooking.Offering.ReminderLetterID]).Docname + @"</td>
                                <td class=""text_left"">" + curPatient.Person.FullnameWithoutMiddlename + @"</td>
                                <td class=""nowrap"">" + (curPatientHasEmail ? curPatientEmail : "Has No Email") + " (" + hrefUpdateEmail + ")" + @"</td>
                                <td>" + sendMethod.ToString().Replace("_", " ") + @"</td>
                            </tr>";

            }


            // combine and email where the patient had no email
            if (incSending && filesToPrint.Count > 0)
            {
                Letter.FileContents filesContents = Letter.FileContents.Merge((Letter.FileContents[])filesToPrint.ToArray(typeof(Letter.FileContents)), "Reminders.pdf"); // .pdf
                SendEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    batchEmail, 
                    "Batch Reminder Letters", 
                    string.Empty, 
                    true, 
                    new Letter.FileContents[] { filesContents });
            }
        }


        if (output.Length == 0)
            output += @"<tr>
                            <td colspan=""7"">No Reminders To Send Today</td>
                        </tr>";

        return @"
            <table class=""table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center"" style=""border-style:solid;border-width:1px;border-collapse:collapse;padding:4px;"">
            <tr>
              <th>Booking (ID, Date/Time)</th>
              <th>Organisation</th>
              <th>Service</th>
              <th>Letter</th>
              <th>Patient</th>
              <th>PT Email</th>
              <th>Send Method</th>
            </tr>
            " + output + @"
            </table>";
    }

    protected static Letter.FileContents GenerteLetter(Booking booking, Letter.FileFormat fileFormat, Hashtable lettersHash, Site[] sites)
    {

        try
        {
            bool isClinic   = booking.Organisation.OrganisationType.OrganisationTypeGroup.ID == 5;
            bool isAgedCare = booking.Organisation.OrganisationType.OrganisationTypeGroup.ID == 6;

            Site site = null;
            for (int i = 0; i < sites.Length; i++)
            {
                if ((isClinic && sites[i].SiteType.ID == 1) || (isAgedCare && sites[i].SiteType.ID == 2))
                    site = sites[i];
            }


            // get letter and make sure it exists
            Letter letter = (Letter)lettersHash[booking.Offering.ReminderLetterID];
            string sourchTemplatePath = letter.GetFullPath(site.SiteID);
            if (!System.IO.File.Exists(sourchTemplatePath))
                throw new CustomMessageException("File doesn't exist.");

            Letter.FileContents fileContents = Letter.CreateLetter(
                fileFormat,
                site,
                booking.Offering.ReminderLetterID,
                booking.Organisation.OrganisationID,
                booking.Patient.PatientID,
                booking.Provider.StaffID,
                booking.BookingID,
                -1,
                1,
                null,
                false,
                1);


            return fileContents;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            return null;
        }

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

    protected static Hashtable GetPatientPhoneNbrCache(Patient[] patients)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Patient patient in patients)
            entityIDArrayList.Add(patient.Person.EntityID);
        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));

        Hashtable contactHash = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1);

        return contactHash;
    }

    protected static Hashtable GetPatientEmailCache(Patient[] patients)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Patient patient in patients)
            entityIDArrayList.Add(patient.Person.EntityID);
        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));

        Hashtable contactHash = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);

        return contactHash;
    }

    protected static Hashtable GetPatientRegOrgCache(Patient[] patients)
    {
        ArrayList patientIDArrayList = new ArrayList();
        foreach (Patient patient in patients)
            patientIDArrayList.Add(patient.PatientID);
        int[] patientIDs = (int[])patientIDArrayList.ToArray(typeof(int));

        Hashtable regOrgHash = new Hashtable();
        System.Data.DataTable tbl = RegisterPatientDB.GetDataTable_OrganisationsOf(patientIDs, true, false, false, true, true);
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int patientID = Convert.ToInt32(tbl.Rows[i]["patient_id"]);
            Organisation org = OrganisationDB.Load(tbl.Rows[i], "", "organisation_entity_id", "organisation_is_deleted");

            if (regOrgHash[patientID] == null)
                regOrgHash[patientID] = new System.Collections.ArrayList();
            ((System.Collections.ArrayList)regOrgHash[patientID]).Add(org);
        }

        return regOrgHash;
    }

    #endregion

    #region SendEmail

    protected static void SendEmail(string fromName, string emailTo, string emailSubject, string emailMessage, bool emailIsHtml, Letter.FileContents[] fileContentsList)
    {

        string tmpDir = null;
        string[] attachments = new string[fileContentsList.Length];  // file paths

        try
        {
            string to = emailTo;
            string subject = emailSubject;
            string message = emailMessage;
            bool isHtml = emailIsHtml;

            // get attachments into files on the server
            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            if (!System.IO.Directory.Exists(tmpLettersDirectory))
                throw new CustomMessageException("Temp letters directory doesn't exist");
            tmpDir = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
            System.IO.Directory.CreateDirectory(tmpDir);
            for (int i = 0; i < fileContentsList.Length; i++)
            {
                string tmpFileName = tmpDir + fileContentsList[i].DocName;
                System.IO.File.WriteAllBytes(tmpFileName, ((Letter.FileContents)fileContentsList[i]).Contents);
                attachments[i] = tmpFileName;
            }

            Emailer.SimpleEmail(
                fromName,
                to,
                subject,
                message,
                true,
                attachments,
                null
                );

        }
        finally
        {
            // delete temp files
            if (attachments != null)
            {
                foreach (string file in attachments)
                {
                    try { System.IO.File.Delete(file); }
                    catch (Exception) { }
                }
            }

            // delete temp dir
            if (tmpDir != null)
            {

                try { System.IO.Directory.Delete(tmpDir, true); }
                catch (Exception) { }
            }
        }

    }


    #endregion

}