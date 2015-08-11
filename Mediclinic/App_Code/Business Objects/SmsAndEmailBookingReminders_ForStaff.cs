using System;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using TransmitSMSAPI;

public class SmsAndEmailBookingReminders_ForStaff
{

    #region Run

    public static string Run(bool incDisplay, bool incSending, DateTime date)
    {
        date = date.Date;

        bool   EnableDailyStaffBookingsReminderSMS    = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableDailyStaffBookingsReminderSMS").Value)    == 1;
        bool   EnableDailyStaffBookingsReminderEmails = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableDailyStaffBookingsReminderEmails").Value) == 1;
        string Staff_Reminders_HasBothSMSandEmail     = SystemVariableDB.GetByDescr("Staff_Reminders_HasBothSMSandEmail").Value;

        decimal balance = SMSCreditDataDB.GetTotal() - SMSHistoryDataDB.GetTotal();
        decimal cost    = Convert.ToDecimal(SystemVariableDB.GetByDescr("SMSPrice").Value);

        //string  callerId    = System.Configuration.ConfigurationManager.AppSettings["SMSTech_callerId"];  // not here used as the callerId will be the org name
        string  callerId    = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value;
        string  countryCode = System.Configuration.ConfigurationManager.AppSettings["SMSTech_CountryCode"];


        // get bookings and organise the data to send/display
        Booking[] bookings = GetBookingsSortedByProviderTimeOrganisation(date);
        ArrayList list     = OrganiseData(bookings);

        // get phone nbr and email hash in one db call
        int[] entityIDs = new int[list.Count];
        for (int i = 0; i < list.Count; i++)
            entityIDs[i] = ((Tuple<Staff, ArrayList>)list[i]).Item1.Person.EntityID;
        Hashtable staffContactPhoneNbrHash = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1);
        Hashtable staffContactEmailHash   = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1); ;



        // lists to send
        ArrayList messagesToSMS = new ArrayList();
        ArrayList messagesToEmail = new ArrayList();


        // display all bookings for debugging
        string bkOutput = "<a href=\"#\" onclick=\"   var e = document.getElementById('div_bk'); e.style.display = (e.style.display == '') ? 'none' : ''; return false; \">Show/Hide Bookings</a><br /><br />";
        bkOutput += "<div id=\"div_bk\" style=\"display:none;\" ><table border=\"1\" cellpadding=\"4\" style=\"border-collapse:collapse;\">";
        for (int i = 0; i < bookings.Length; i++)
            bkOutput += "<tr><td>" + bookings[i].BookingID + "</td><td>" + bookings[i].Provider.Person.FullnameWithoutMiddlename + " [" + bookings[i].Provider.StaffID + "]</td><td>" + bookings[i].Organisation.OrganisationID + "</td><td>" + bookings[i].DateStart.ToString("HH:mm") + " - " + bookings[i].DateEnd.ToString("HH:mm") + "</td></tr>";
        bkOutput += "</table></div>";


        // go through all items to add to send lists and to display for debugging
        string output = "<table border=\"1\" cellpadding=\"4\" style=\"border-collapse:collapse;\">";
        for (int i = 0; i < list.Count; i++)
        {
            Tuple<Staff, ArrayList> t = (Tuple<Staff, ArrayList>)list[i];

            string phoneNum = GetPhoneNbr(staffContactPhoneNbrHash, t.Item1.Person.EntityID, true);
            if (phoneNum != null)
                phoneNum = phoneNum.StartsWith("0") ? countryCode + phoneNum.Substring(1) : phoneNum;

            string email    = GetEmail(staffContactEmailHash, t.Item1.Person.EntityID);


            // ignore if setting is to not sending sms's or emails
            if (phoneNum != null && (!EnableDailyStaffBookingsReminderSMS   || !t.Item1.EnableDailyReminderSMS))
                phoneNum  = null;
            if (email != null    && (!EnableDailyStaffBookingsReminderEmails || !t.Item1.EnableDailyReminderEmail))
                email  = null;

            // if balance too low, can not send by SMS
            if (phoneNum != null && balance < cost)
                phoneNum = null;

            // if has both, then send based on setting
            if (phoneNum != null && email != null)
            {
                if (Staff_Reminders_HasBothSMSandEmail == "Email") // setting is - when both, send only via email
                    phoneNum = null;
                if (Staff_Reminders_HasBothSMSandEmail == "SMS")   // setting is - when both, send only via sms
                    email = null;
            }


            output += "<tr>";
            output += "<td>" + "<b><u>" + t.Item1.Person.FullnameWithoutMiddlename + " [ID:" + t.Item1.StaffID + "]</u></b>";
            output += "    <table cellpadding=\"0\" cellspacing=\"0\">";
            output += "        <tr><td>" + "Mobile:</td><td>" + (phoneNum == null ? "NONE" : "<b>" + phoneNum + "</b>") + "</td></tr>";
            output += "        <tr><td>" + "Email:</td><td>"  + (email    == null ? "NONE" : "<b>" + email    + "</b>") + "</td></tr>";
            output += "    </table>";
            output += "</td>";

            string smsText   = "Hi " + t.Item1.Person.Firstname + Environment.NewLine + "Tomorrow (" + date.ToString("dddd") + " " + Utilities.GetDateOrdinal(date.Day) + ") you have appointments at:" + Environment.NewLine;
            string emailText = "Hi " + t.Item1.Person.Firstname + "," + "<br />"      + "Tomorrow (" + date.ToString("dddd") + " " + Utilities.GetDateOrdinal(date.Day) + ") you have appointments at:" + "<br /><br />";

            emailText += "<table>";
            string times = string.Empty;
            for (int j = 0; j < t.Item2.Count; j++)
            {
                Tuple<Organisation, DateTime, DateTime> t2 = (Tuple<Organisation, DateTime, DateTime>)t.Item2[j];

                smsText   += Environment.NewLine + "[" + ConvertTime(t2.Item2) + " - " + ConvertTime(t2.Item3) + "] " + t2.Item1.Name;
                emailText += "<tr><td>" + "[" + ConvertTime(t2.Item2) + " - " + ConvertTime(t2.Item3) + "]" + "</td><td><b>" + t2.Item1.Name + "</b></td></tr>";

                times += (times.Length == 0 ? "" : "<br />") + t2.Item1.Name + " [ID:" + t2.Item1.OrganisationID + "] " + ConvertTime(t2.Item2) + " - " + ConvertTime(t2.Item3);
            }
            emailText += "</table><br />Regards,<br />" + callerId;

            output += "<td>" + times + "</td>";
            output += "<td>" + "<b><font color=\"blue\">" + smsText.Replace(Environment.NewLine, "<br />") + "</font></b>" + "</td>";




            // add to lists to sms or email (or both)
            if (phoneNum != null)
            {
                messagesToSMS.Add(new Tuple<Staff, decimal, string, string, string>(t.Item1, cost, phoneNum, smsText, callerId));
                if (incSending)
                    balance -= cost;
            }
            if (email != null)
            {
                messagesToEmail.Add(new Tuple<Staff, string, string, string, string>(t.Item1, callerId, email, emailText, "Work locations for " + date.ToString("d MMM, yyyy") ));
            }


        }

        output += "</table>";


        // run the sending and send off reminders

        if (incSending)
        {

            /*
             * run the sendings
             */

            SendSMSes((Tuple<Staff, decimal, string, string, string>[])messagesToSMS.ToArray(typeof(Tuple<Staff, decimal, string, string, string>)));
            SendEmails((Tuple<Staff, string, string, string, string>[])messagesToEmail.ToArray(typeof(Tuple<Staff, string, string, string, string>)));


            /*
             * send balance warning
             */

            SystemVariables systemVariables = SystemVariableDB.GetAll();
            string warningEmail = systemVariables["SMSCreditNotificationEmailAddress"].Value;
            decimal warningThreshold = Convert.ToDecimal(systemVariables["SMSCreditLowBalance_Threshold"].Value);
            bool checkSMSCreditOutOfBalance = Convert.ToInt32(systemVariables["SMSCreditOutOfBalance_SendEmail"].Value) == 1;
            bool checkMSCreditLowBalance = Convert.ToInt32(systemVariables["SMSCreditLowBalance_SendEmail"].Value) == 1;


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
            return output + "<br />" + bkOutput;
        else
            return string.Empty;
    }

    protected static string ConvertTime(DateTime dt)
    {
        return dt.ToString("h:mm") + (dt.Hour < 12 ? "am" : "pm");
    }


    protected static Booking[] GetBookingsSortedByProviderTimeOrganisation(DateTime date)
    {
        // get bookings
        date = date.Date;
        Booking[] bookings = BookingDB.GetBetween(date, date.AddHours(23).AddMinutes(59), null, null, null, null);

        // remove unavailabilities
        ArrayList list = new ArrayList();
        for (int i = 0; i < bookings.Length; i++)
            if (bookings[i].BookingTypeID == 34)
                list.Add(bookings[i]);
        bookings = (Booking[])list.ToArray(typeof(Booking));

        // sort by prov, bk time, org
        Array.Sort(bookings, BkCompare);

        return bookings;
    }

    protected static int BkCompare(Booking x, Booking y)
    {
        if (x.Provider.StaffID > y.Provider.StaffID)
            return 1;
        if (x.Provider.StaffID < y.Provider.StaffID)
            return -1;

        if (x.DateStart > y.DateStart)
            return 1;
        if (x.DateStart < y.DateStart)
            return -1;

        if (x.Organisation.OrganisationID > y.Organisation.OrganisationID)
            return 1;
        if (x.Organisation.OrganisationID < y.Organisation.OrganisationID)
            return -1;

        return 0;
    }


    // list of   Tuple<Staff, ArrayList>
    // each provicer has list of  Tuple<Organisation, DateTime, DateTime>  -- start/end times
    protected static ArrayList OrganiseData(Booking[] bookings)
    {
        ArrayList list = new ArrayList();

        Staff curProvider = null;
        Organisation curOrg = null;
        DateTime curStartTime = DateTime.MinValue;
        DateTime curEndTime = DateTime.MinValue;


        for (int i = 0; i < bookings.Length; i++)
        {
            if (curProvider == null || curOrg == null)
            {
                curProvider = bookings[i].Provider;
                curOrg = bookings[i].Organisation;
                curStartTime = bookings[i].DateStart;
                curEndTime = bookings[i].DateEnd;
            }
            else
            {
                if (curProvider.StaffID == bookings[i].Provider.StaffID && curOrg.OrganisationID == bookings[i].Organisation.OrganisationID)
                {
                    curEndTime = bookings[i].DateEnd;
                }
                else
                {
                    Add(ref list, curProvider, curOrg, curStartTime, curEndTime);

                    curProvider = bookings[i].Provider;
                    curOrg = bookings[i].Organisation;
                    curStartTime = bookings[i].DateStart;
                    curEndTime = bookings[i].DateEnd;
                }
            }
        }

        // add the last one
        if (curProvider != null || curOrg != null)
            Add(ref list, curProvider, curOrg, curStartTime, curEndTime);

        return list;
    }

    protected static void Add(ref ArrayList list, Staff staff, Organisation org, DateTime start, DateTime end)
    {
        Tuple<Organisation, DateTime, DateTime> t = new Tuple<Organisation, DateTime, DateTime>(org, start, end);

        bool providerFound = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (((Tuple<Staff, ArrayList>)list[i]).Item1.StaffID == staff.StaffID)
            {
                ((ArrayList)((Tuple<Staff, ArrayList>)list[i]).Item2).Add(t);
                providerFound = true;
            }
        }

        if (!providerFound)
        {
            Tuple<Staff, ArrayList> t2 = new Tuple<Staff, ArrayList>(staff, new ArrayList());
            ((ArrayList)((Tuple<Staff, ArrayList>)t2).Item2).Add(t);
            list.Add(t2);
        }
    }

    #endregion

    #region SendSMSes SendEmails

    protected static void SendSMSes(Tuple<Staff, decimal, string, string, string>[] messagesInfo)
    {
        for (int i = 0; i < messagesInfo.Length; i++)
        {
            Staff   staff     = messagesInfo[i].Item1;
            decimal cost      = messagesInfo[i].Item2;
            string  mobile    = messagesInfo[i].Item3;
            string  message   = messagesInfo[i].Item4;
            string  callerId  = messagesInfo[i].Item5;

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
                        mobile   + Environment.NewLine + Environment.NewLine + 
                        message  + Environment.NewLine + Environment.NewLine + 
                        callerId + Environment.NewLine + Environment.NewLine +
                        xmlResponse, 
                        true);

                string SMSTechMessageID = GetSMSTechMessageID(xmlResponse);
                SMSHistoryDataDB.Insert(3, -1, -1, mobile, message, cost, SMSTechMessageID);
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


    protected static void SendEmails(Tuple<Staff, string, string, string, string>[] messagesInfo)
    {
        for (int i = 0; i < messagesInfo.Length; i++)
        {
            Staff  staff    = messagesInfo[i].Item1;
            string fromName = messagesInfo[i].Item2;
            string email    = messagesInfo[i].Item3;
            string message  = messagesInfo[i].Item4;
            string subject  = messagesInfo[i].Item5;

            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogSMSSending"]))
                    Logger.LogSMSSend(
                        System.Web.HttpContext.Current.Session["DB"].ToString() + Environment.NewLine + Environment.NewLine + 
                        email   + Environment.NewLine + Environment.NewLine + 
                        message + Environment.NewLine + Environment.NewLine + 
                        subject + Environment.NewLine + Environment.NewLine, 
                        true);

                SendEmail(fromName, email, subject, message);
                EmailHistoryDataDB.Insert(3, -1, -1, email, message);
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

    protected static string GetEmail(Hashtable contactHash, int entityID)
    {
        return ContactDB.GetEmailsCommaSepByEntityID(contactHash, entityID, false, true);
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

}