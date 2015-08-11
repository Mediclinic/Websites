using System;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using TransmitSMSAPI;

public class SmsAndEmailBirthdayMessages
{

    #region Run

    public static string Run(bool incDisplay, bool incSending, DateTime date)
    {
        if (incSending)
            RunBirthdaysWithoutSMSorEmail();

        date = date.Date;


        bool EnableBirthdaySMS    = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableBirthdaySMS").Value) == 1;
        bool EnableBirthdayEmails = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableBirthdayEmails").Value) == 1;


        Site[] sites = SiteDB.GetAll();

        Patient[] patients = PatientDB.GetBirthdays(date);
        Hashtable patientContactPhoneNbrHash = GetPatientPhoneNbrCache(patients);
        Hashtable patientContactEmailHash    = GetPatientEmailCache(patients);
        Hashtable patientRegOrgHash          = GetPatientRegOrgCache(patients);   // get a hash of patient reg to org


        decimal balance = SMSCreditDataDB.GetTotal() - SMSHistoryDataDB.GetTotal();
        decimal cost    = Convert.ToDecimal(SystemVariableDB.GetByDescr("SMSPrice").Value);


        string    callerId                 = System.Configuration.ConfigurationManager.AppSettings["SMSTech_callerId"];  // not here used as the callerId will be the org name
        string    countryCode              = System.Configuration.ConfigurationManager.AppSettings["SMSTech_CountryCode"];
        ArrayList messagesToSMS            = new ArrayList();
        ArrayList messagesToEmail          = new ArrayList();


        string output = "<table class=\"table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center\" border=\"1\" style=\"border-collapse:collapse;\">";
        int countWithPatient = 0;
        foreach (Patient patient in patients)
        {
            // get all info to send via sms or email

            Site site = null;
            foreach (Site curSite in sites)
                if ((patient.IsClinicPatient && curSite.SiteType.ID == 1) || (!patient.IsClinicPatient && curSite.SiteType.ID == 2))
                    site = curSite;

            ArrayList orgs = patientRegOrgHash[patient.PatientID] as ArrayList;
            string orgText = (orgs == null || orgs.Count == 0 || orgs.Count > 1) ? site.Name : ((Organisation)orgs[0]).Name;


            string phoneNumPatient = GetPhoneNbr(patientContactPhoneNbrHash, patient.Person.EntityID, true);
            if (phoneNumPatient != null)
                phoneNumPatient = phoneNumPatient.StartsWith("0") ? countryCode + phoneNumPatient.Substring(1) : phoneNumPatient;

            string emailPatient     = GetEmail(patientContactEmailHash, patient.Person.EntityID);

            string smsText          = GetSMSText(patient,   site, patientRegOrgHash[patient.PatientID] as ArrayList);
            string emailText        = GetEmailText(patient, site, patientRegOrgHash[patient.PatientID] as ArrayList);
            string emailSubjectText = GetEmailSubjectText(patient, site, patientRegOrgHash[patient.PatientID] as ArrayList);



            // display the info

            string tdTagStart          = phoneNumPatient == null && emailPatient == null ? "<td class=\"nowrap\" style=\"color:grey;\">" : (phoneNumPatient == null ? "<td>"  : "<td><b>");
            string tdTagStartLeftAlign = phoneNumPatient == null && emailPatient == null ? "<td class=\"nowrap text_left\" style=\"color:grey;\">" : (phoneNumPatient == null ? "<td class=\"text_left\">" : "<td class=\"text_left\"><b>");
            string tdTagEnd            = phoneNumPatient == null && emailPatient == null ? "</td>" : (phoneNumPatient == null ? "</td>" : "</b></td>");

            output += "<tr>";
            output += tdTagStart          + patient.PatientID + tdTagEnd;
            output += tdTagStart          + patient.Person.Dob.ToString("dd-MM-yyyy") + tdTagEnd;
            output += tdTagStart          + patient.Person.FullnameWithoutMiddlename + "<br />" +
                                            (phoneNumPatient == null ? "-- No Mobile --" : "<u>" + phoneNumPatient + "</u>") + "<br />" +
                                            (emailPatient == null ? "-- No Email --" : "<u>" + emailPatient + "</u>") + tdTagEnd;
            output += tdTagStartLeftAlign + (phoneNumPatient == null && emailPatient == null ? "" : "<u>" + emailSubjectText + "</u><br /><br />" + emailText) + tdTagEnd;
            output += "</tr>";

            countWithPatient++;



            // add to lists to sms or email (or both)

            if (EnableBirthdaySMS && phoneNumPatient != null && balance >= cost)
            {
                messagesToSMS.Add(new Tuple<int, decimal, string, string, string>(patient.PatientID, cost, phoneNumPatient, smsText, orgText));
                if (incSending)
                    balance -= cost;
            }
            if (EnableBirthdayEmails && emailPatient != null)
            {
                messagesToEmail.Add(new Tuple<int, string, string, string, string>(patient.PatientID, orgText, emailPatient, emailText, emailSubjectText));
            }

        }
        output += "</table>";


        // run the sending and send off reminders -- but only if there was any bookings

        if (incSending && patients.Length > 0)
        {

            /*
             * run the sendings
             */

            SendSMSes((Tuple<int, decimal, string, string, string>[])messagesToSMS.ToArray(typeof(Tuple<int, decimal, string, string, string>)));
            SendEmails((Tuple<int, string, string, string, string>[])messagesToEmail.ToArray(typeof(Tuple<int, string, string, string, string>)));


            /*
             * send balance warning
             */

            SystemVariables systemVariables = SystemVariableDB.GetAll();
            string  warningEmail               = systemVariables["SMSCreditNotificationEmailAddress"].Value;
            decimal warningThreshold           = Convert.ToDecimal(systemVariables["SMSCreditLowBalance_Threshold"].Value);
            bool    checkSMSCreditOutOfBalance = Convert.ToInt32(systemVariables["SMSCreditOutOfBalance_SendEmail"].Value) == 1;
            bool    checkMSCreditLowBalance    = Convert.ToInt32(systemVariables["SMSCreditLowBalance_SendEmail"].Value) == 1;


            if (warningEmail.Length > 0 && checkSMSCreditOutOfBalance && balance < cost)
            {
                SendEmail(
                    warningEmail,
                    "SMS Credit Used Up",
                    "Please note that your SMS credit at mediclinic has been used up. To continue sending, please top up.<br /><br />Best regards,<br />Mediclinic");
            }
            else if (warningEmail.Length > 0 && checkMSCreditLowBalance && balance <= warningThreshold)  // dont send warning low balance if already sending out of credit email
            {
                SendEmail(
                    warningEmail,
                    "SMS Credit Warning - Don't Forget To Top-Up Before It Runs Out",
                    "Hi! Just a friendly reminder that the SMS reminder threshold you set has been reached.<br /> To avoid missing SMS'es being sent, don't forget to top-up before the remainder runs out!<br /><br />Best regards,<br />Mediclinic");
            }
        }

        if (incDisplay)
            return "Count: <b>" + countWithPatient + "</b> &nbsp;&nbsp; [with mobile: <b>" + messagesToSMS.Count + "</b>] &nbsp;&nbsp; [with email: <b>" + messagesToEmail.Count + "</b>] " + "<br /><br />" + output;
        else
            return string.Empty;
    }

    #endregion

    #region SendSMSes SendEmails

    protected static void SendSMSes(Tuple<int, decimal, string, string, string>[] messagesInfo)
    {
        for (int i = 0; i < messagesInfo.Length; i++)
        {
            Tuple<int, decimal, string, string, string> messageInfo = (Tuple<int, decimal, string, string, string>)messagesInfo[i];
            int     patientID = messageInfo.Item1;
            decimal cost      = messageInfo.Item2;
            string  mobile    = messageInfo.Item3;
            string  message   = messageInfo.Item4;
            string  callerId  = messageInfo.Item5;

            // until they have an interface to send all in one call, it's better to put each 
            // in its own try/catch block so if some causes an error, it doesn't ruin all of them.
            try
            {
                TransmitSMSAPIWrapper sms = new TransmitSMSAPIWrapper(
                    System.Configuration.ConfigurationManager.AppSettings["SMSTech_Key"],
                    System.Configuration.ConfigurationManager.AppSettings["SMSTech_Secret"],
                    System.Configuration.ConfigurationManager.AppSettings["SMSTech_RequestURL"]);

                string xmlResponse = sms.MessageSingle(mobile, message, callerId);

                //Logger.LogSMSSend(
                //    mobile   + Environment.NewLine + Environment.NewLine + 
                //    message  + Environment.NewLine + Environment.NewLine + 
                //    callerId + Environment.NewLine + Environment.NewLine +
                //    response, 
                //    true);

                string SMSTechMessageID = GetSMSTechMessageID(xmlResponse);
                SMSHistoryDataDB.Insert(2, patientID, -1, mobile, message, cost, SMSTechMessageID);
            }
            catch (Exception ex)
            {
                Logger.LogSMSSend(
                    mobile   + Environment.NewLine + Environment.NewLine +
                    message  + Environment.NewLine + Environment.NewLine +
                    cost     + Environment.NewLine + Environment.NewLine +
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
            int    patientID  = messageInfo.Item1;
            string orgName    = messageInfo.Item2;
            string email      = messageInfo.Item3;
            string message    = messageInfo.Item4;
            string subject    = messageInfo.Item5;

            try
            {
                SendEmail(orgName, email, subject, message);
                EmailHistoryDataDB.Insert(2, patientID, -1, email, message);
            }
            catch (Exception ex)
            {
                Logger.LogSMSSend(
                    email   + Environment.NewLine + Environment.NewLine +
                    message + Environment.NewLine + Environment.NewLine +
                    subject + Environment.NewLine + Environment.NewLine +
                    ex.ToString(),
                    true);

                Logger.LogException(ex, true);
            }

        }
    }

    #endregion

    #region GetSMSText, GetEmailText

    protected static string GetSMSText(Patient patient, Site site, ArrayList orgs)
    {
        string orgsText = string.Empty;
        if (orgs == null || orgs.Count == 0)
        {
            orgsText = site.Name;
        }
        else
        {
            for (int i = 0; i < orgs.Count; i++)
            {
                if (i > 0 && i == orgs.Count - 1)
                    orgsText += " and ";
                else if (i > 0)
                    orgsText += ", ";

                orgsText += ((Organisation)orgs[i]).Name;
            }
        }

        return @"Happy Birthday " + patient.Person.Firstname + @",

Wishing you the best on this special day from all of us at " + orgsText;
    }

    protected static string GetEmailText(Patient patient, Site site, ArrayList orgs)
    {
        string orgsText = string.Empty;
        if (orgs == null || orgs.Count == 0)
        {
            orgsText = site.Name;
        }
        else
        {
            for (int i = 0; i < orgs.Count; i++)
            {
                if (i > 0 && i == orgs.Count - 1)
                    orgsText += " and ";
                else if (i > 0)
                    orgsText += ", ";

                orgsText += ((Organisation)orgs[i]).Name;
            }
        }

        return @"Happy Birthday " + patient.Person.Firstname + @",
<br /><br />You remain valued to us and we hope you enjoy many more years of health, wealth, and happiness for years to come.
<br /><br />Best wishes from all of us here at " + orgsText;
    }
    protected static string GetEmailSubjectText(Patient patient, Site site, ArrayList orgs)
    {
        string orgText = (orgs == null || orgs.Count == 0 || orgs.Count > 1) ? site.Name : ((Organisation)orgs[0]).Name;

        return @"Happy Birthday From " + orgText;
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

    protected static void SendEmail(string from_name, string to, string subject, string message)
    {
        Emailer.SimpleEmail(
            from_name,
            to,
            subject,
            message,
            true,
            null,
            null
            );
    }
    protected static void SendEmail(string to, string subject, string message)
    {
        Emailer.SimpleEmail(
            to,
            subject,
            message,
            true,
            null,
            null
            );
    }

    #endregion


    #region RunBirthdaysWithoutSMSorEmail

    public static void RunBirthdaysWithoutSMSorEmail(bool includeValidation = true)
    {
        bool   enableEmails             = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendEmail").Value) == 1;
        string emailAddress             = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_EmailAddress").Value;

        bool   incPatientsWithMobile    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_IncPatientsWithMobile").Value) == 1;
        bool   incPatientsWithEmail     = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_IncPatientsWithEmail").Value)  == 1;

        bool   sendMondays              = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendMondays").Value)    == 1;
        bool   sendTuesdays             = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendTuesdays").Value)   == 1;
        bool   sendWednesdays           = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendWednesdays").Value) == 1;
        bool   sendThursdays            = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendThursdays").Value)  == 1;
        bool   sendFridays              = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFridays").Value)    == 1;
        bool   sendSaturdays            = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendSaturdays").Value)  == 1;
        bool   sendSundays              = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendSundays").Value)    == 1;

        int    fromDaysAheadMondays     = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Mondays").Value);
        int    untilDaysAheadMondays    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Mondays").Value);
        int    fromDaysAheadTuesdays    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Tuesdays").Value);
        int    untilDaysAheadTuesdays   = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Tuesdays").Value);
        int    fromDaysAheadWednesdays  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Wednesdays").Value);
        int    untilDaysAheadWednesdays = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Wednesdays").Value);
        int    fromDaysAheadThursdays   = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Thursdays").Value);
        int    untilDaysAheadThursdays  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Thursdays").Value);
        int    fromDaysAheadFridays     = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Fridays").Value);
        int    untilDaysAheadFridays    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Fridays").Value);
        int    fromDaysAheadSaturdays   = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Saturdays").Value);
        int    untilDaysAheadSaturdays  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Saturdays").Value);
        int    fromDaysAheadSundays     = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Sundays").Value);
        int    untilDaysAheadSundays    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Sundays").Value);



        if (includeValidation && !enableEmails) return;
        if (!Utilities.IsValidEmailAddresses(emailAddress, false)) return;

        if (includeValidation && DateTime.Today.DayOfWeek == DayOfWeek.Monday   && !sendMondays)    return;
        if (includeValidation && DateTime.Today.DayOfWeek == DayOfWeek.Tuesday  && !sendTuesdays)   return;
        if (includeValidation && DateTime.Today.DayOfWeek == DayOfWeek.Wednesday&& !sendWednesdays) return;
        if (includeValidation && DateTime.Today.DayOfWeek == DayOfWeek.Thursday && !sendThursdays)  return;
        if (includeValidation && DateTime.Today.DayOfWeek == DayOfWeek.Friday   && !sendFridays)    return;
        if (includeValidation && DateTime.Today.DayOfWeek == DayOfWeek.Saturday && !sendSaturdays)  return;
        if (includeValidation && DateTime.Today.DayOfWeek == DayOfWeek.Sunday   && !sendSundays)    return;

        if (fromDaysAheadMondays    > untilDaysAheadMondays)    return;
        if (fromDaysAheadTuesdays   > untilDaysAheadTuesdays)   return;
        if (fromDaysAheadWednesdays > untilDaysAheadWednesdays) return;
        if (fromDaysAheadThursdays  > untilDaysAheadThursdays)  return;
        if (fromDaysAheadFridays    > untilDaysAheadFridays)    return;
        if (fromDaysAheadSaturdays  > untilDaysAheadSaturdays)  return;
        if (fromDaysAheadSundays    > untilDaysAheadSundays)    return;



        int fromDaysAhead = 0, untilDaysAhead = 0;
        if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)    { fromDaysAhead = fromDaysAheadMondays;    untilDaysAhead = untilDaysAheadMondays;    }
        if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)   { fromDaysAhead = fromDaysAheadTuesdays;   untilDaysAhead = untilDaysAheadTuesdays;   }
        if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday) { fromDaysAhead = fromDaysAheadWednesdays; untilDaysAhead = untilDaysAheadWednesdays; }
        if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)  { fromDaysAhead = fromDaysAheadThursdays;  untilDaysAhead = untilDaysAheadThursdays;  }
        if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)    { fromDaysAhead = fromDaysAheadFridays;    untilDaysAhead = untilDaysAheadFridays;    }
        if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)  { fromDaysAhead = fromDaysAheadSaturdays;  untilDaysAhead = untilDaysAheadSaturdays;  }
        if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)    { fromDaysAhead = fromDaysAheadSundays;    untilDaysAhead = untilDaysAheadSundays;    }

        DateTime start = DateTime.Now.AddDays(fromDaysAhead);
        DateTime end   = DateTime.Now.AddDays(untilDaysAhead);

        System.Data.DataTable dt;
        try
        {
            dt = PatientDB.GetBirthdays_DataTable(start.Month, start.Day, end.Month, end.Day);
        }
        catch (CustomMessageException ex)
        {
            Logger.LogException(ex);
            return;
        }


        // get their mobile and emails to filter

        Patient[] patients = new Patient[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            patients[i] = PatientDB.Load(dt.Rows[i]);
            patients[i].Person = PersonDB.Load(dt.Rows[i]);
            patients[i].Person.Title = IDandDescrDB.Load(dt.Rows[i], "t_title_id", "t_descr");
        }

        Hashtable patientContactPhoneNbrHash = GetPatientPhoneNbrCache(patients);
        Hashtable patientContactEmailHash    = GetPatientEmailCache(patients);

        dt.Columns.Add("mobile", typeof(string));
        dt.Columns.Add("email", typeof(string));
        for (int i = dt.Rows.Count - 1; i >= 0; i--)
        {
            string phoneNumPatient = GetPhoneNbr(patientContactPhoneNbrHash, patients[i].Person.EntityID, true);
            string emailPatient    = GetEmail(patientContactEmailHash, patients[i].Person.EntityID);

            if ((!incPatientsWithMobile && (phoneNumPatient != null && phoneNumPatient.Length > 0)) ||
                (!incPatientsWithEmail  && (emailPatient    != null && emailPatient.Length    > 0)))
            {
                dt.Rows.RemoveAt(i);
                continue;
            }

            dt.Rows[i]["mobile"] = phoneNumPatient == null ? "" : phoneNumPatient;
            dt.Rows[i]["email"]  = emailPatient    == null ? "" : emailPatient;
        }




        // put in file to email

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("\"" + "D.O.B."           + "\"").Append(",");
        sb.Append("\"" + "Name"             + "\"").Append(",");
        sb.Append("\"" + "Clinic Patient"   + "\"").Append(",");
        sb.Append("\"" + "Mobile"           + "\"").Append(",");
        sb.Append("\"" + "Email"            + "\"").Append(",");

        sb.AppendLine();


        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["dob"]).ToString("d MMMMM, yyyy")                    + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["firstname"].ToString() + " " + dt.Rows[i]["surname"].ToString() + (dt.Rows[i]["t_title_id"] == DBNull.Value || Convert.ToInt32(dt.Rows[i]["t_title_id"]) == 0 ? "" :  " ("+dt.Rows[i]["t_descr"]+")") + "\"").Append(",");
                sb.Append("\"" + (Convert.ToBoolean(dt.Rows[i]["is_clinic_patient"]) ? "Yes" : "No")                + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["mobile"].ToString()                                                    + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["email"].ToString()                                                     + "\"").Append(",");
                sb.AppendLine();
            }
        }


        // put in file, then email it

        string tmpLettersDirectory = Letter.GetTempLettersDirectory();
        if (!System.IO.Directory.Exists(tmpLettersDirectory))
            throw new CustomMessageException("Temp letters directory doesn't exist");

        string tmpDir = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
        System.IO.Directory.CreateDirectory(tmpDir);
        string tmpFileName = tmpDir + "Birthdays.csv";
        System.IO.File.WriteAllText(tmpFileName, sb.ToString());

        Emailer.SimpleEmail(emailAddress, "Upcoming Birthdays", "Please find attached all birthdays from " + fromDaysAhead + " days ahead until " + untilDaysAhead + " days ahead.<br /><br />Regards,<br />Mediclinic", true, new string[]{ tmpFileName }, null);

        System.IO.File.Delete(tmpFileName);
        System.IO.Directory.Delete(tmpDir);
    }

    #endregion

}