using System;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using TransmitSMSAPI;

public class SmsAndEmailBookingReminders
{

    #region Run

    public static string Run(bool incDisplay, bool incSending, DateTime date)
    {
        date = date.Date;


        bool   EnableDailyBookingReminderSMS             = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableDailyBookingReminderSMS").Value)    == 1;
        bool   EnableDailyBookingReminderEmails          = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableDailyBookingReminderEmails").Value) == 1;
        int    NbrDaysAheadToSendDailyBookingReminderSMS = Convert.ToInt32(SystemVariableDB.GetByDescr("NbrDaysAheadToSendDailyBookingReminderSMS").Value);
        string SendDailyBookingReminderText_SMS          = SystemVariableDB.GetByDescr("SendDailyBookingReminderText_SMS").Value;
        string SendDailyBookingReminderText_Email        = SystemVariableDB.GetByDescr("SendDailyBookingReminderText_Email").Value;
        string SendDailyBookingReminderText_EmailSubject = SystemVariableDB.GetByDescr("SendDailyBookingReminderText_EmailSubject").Value;
        string PT_Reminders_HasBothSMSandEmail           = SystemVariableDB.GetByDescr("PT_Reminders_HasBothSMSandEmail").Value;

        date = date.AddDays(NbrDaysAheadToSendDailyBookingReminderSMS - 1);


        Booking[] bookings = BookingDB.GetBetween(date, date.AddDays(1).AddMinutes(-1), null, null, null, null, false, "0", false, null);
        Hashtable patientContactPhoneNbrHash = GetPatientPhoneNbrCache(bookings);
        Hashtable patientContactEmailHash    = GetPatientEmailCache(bookings);
        Hashtable orgContactHash             = GetOrgPhoneNbrCache(bookings);
        Hashtable orgAddrContactHash         = GetOrgAddrCache(bookings);



        decimal balance = SMSCreditDataDB.GetTotal() - SMSHistoryDataDB.GetTotal();
        decimal cost = Convert.ToDecimal(SystemVariableDB.GetByDescr("SMSPrice").Value);


        string    callerId                 = System.Configuration.ConfigurationManager.AppSettings["SMSTech_callerId"];  // not used here as the callerId will be the org name
        string    countryCode              = System.Configuration.ConfigurationManager.AppSettings["SMSTech_CountryCode"];
        ArrayList messagesToSMS            = new ArrayList();
        ArrayList messagesToEmail          = new ArrayList();
        ArrayList bookingIDsConfirmedSMS   = new ArrayList();
        ArrayList bookingIDsConfirmedEmail = new ArrayList();


        string output = "<table class=\"table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center\" border=\"1\" style=\"border-collapse:collapse;\">";
        int countWithPatient = 0;
        foreach (Booking booking in bookings)
        {
            if (booking.BookingTypeID != 34)  // only bookings, not days marked off
                continue;

            if (booking.Patient == null || booking.Offering == null)      // prob aged care booking
                continue;

            // Marcus: send sms even if booking is confirmed
            //if (booking.ConfirmedBy != null)  // don't send reminders to those already confirmed
            //    continue;

            // get all info to send via sms or email

            string phoneNumPatient = GetPhoneNbr(patientContactPhoneNbrHash, booking.Patient.Person.EntityID, true);
            if (phoneNumPatient != null)
                phoneNumPatient = phoneNumPatient.StartsWith("0") ? countryCode + phoneNumPatient.Substring(1) : phoneNumPatient;

            string emailPatient = GetEmail(patientContactEmailHash, booking.Patient.Person.EntityID);
            string phoneNumOrg  = GetPhoneNbrs(orgContactHash, booking.Organisation.EntityID);
            string addrOrg      = GetAddr(orgAddrContactHash, booking.Organisation.EntityID);

            string smsText          = GetSMSText(         booking, phoneNumOrg, addrOrg, SendDailyBookingReminderText_SMS);
            string emailText        = GetEmailText(       booking, phoneNumOrg, addrOrg, SendDailyBookingReminderText_Email);
            string emailSubjectText = GetEmailSubjectText(booking, phoneNumOrg, addrOrg, SendDailyBookingReminderText_EmailSubject);


            // kept just to show their email/phone number exists even though we may not be sending to there due to settings or low balance.
            string phoneNumPatient_Original = phoneNumPatient;
            string emailPatient_Original    = emailPatient;

            
            // ignore if setting is to not sending sms's or emails
            if (phoneNumPatient != null && !EnableDailyBookingReminderSMS)
                phoneNumPatient  = null;
            if (emailPatient != null    && !EnableDailyBookingReminderEmails)
                emailPatient  = null;

            // if balance too low, can not send by SMS
            if (phoneNumPatient != null && balance < cost)
                phoneNumPatient = null;

            // if has both, then send based on setting
            if (phoneNumPatient != null && emailPatient != null)
            {
                if (PT_Reminders_HasBothSMSandEmail == "Email") // setting is - when both, send only via email
                    phoneNumPatient = null;
                if (PT_Reminders_HasBothSMSandEmail == "SMS")   // setting is - when both, send only via sms
                    emailPatient = null;
            }


            string textToDisplay = string.Empty;
            if (phoneNumPatient != null) textToDisplay += "<b>" + smsText.Replace(Environment.NewLine, "<br />") + "</b>";
            if (emailPatient    != null) textToDisplay += (textToDisplay.Length == 0 ? "" : "<br><hr>") + "<u>" + emailSubjectText + "</u><br /><br />" + emailText;



            // display the info

            string tdTagStart          = phoneNumPatient == null && emailPatient == null ? "<td class=\"nowrap\" style=\"color:grey;\">" : (phoneNumPatient == null ? "<td>"  : "<td>");
            string tdTagStartLeftAlign = phoneNumPatient == null && emailPatient == null ? "<td class=\"nowrap text_left\" style=\"color:grey;\">" : (phoneNumPatient == null ? "<td class=\"text_left\">" : "<td class=\"text_left\">");
            string tdTagEnd            = phoneNumPatient == null && emailPatient == null ? "</td>" : (phoneNumPatient == null ? "</td>" : "</td>");

            output += "<tr>";
            output += tdTagStart          + booking.BookingID + tdTagEnd;
            output += tdTagStart          + booking.DateStart.ToString("dd-MM-yy") + "<br />" + booking.DateStart.ToString("HH:mm") + " - " + booking.DateEnd.ToString("HH:mm") + tdTagEnd;
            output += tdTagStart          + booking.Organisation.Name + "<br />" + (phoneNumOrg == null ? "-- No Phone --" : phoneNumOrg.Replace(",", "<br />").Replace("or", "<br />")) + tdTagEnd;
            output += tdTagStart          + booking.Patient.Person.FullnameWithoutMiddlename + "<br />" +
                                            (phoneNumPatient_Original == null ? "-- No Mobile --" : "<u>" + phoneNumPatient_Original + "</u>") + "<br />" +
                                            (emailPatient_Original    == null ? "-- No Email --"  : "<u>" + emailPatient_Original    + "</u>") + tdTagEnd;
            output += tdTagStartLeftAlign + textToDisplay + tdTagEnd;
            output += "</tr>";

            countWithPatient++;



            /*
             *  add to lists to sms or email (or both)
             */

            if (phoneNumPatient != null)
            {
                messagesToSMS.Add(new Tuple<int, decimal, string, string, string>(booking.BookingID, cost, phoneNumPatient, smsText, booking.Organisation.Name));
                bookingIDsConfirmedSMS.Add(booking.BookingID);
                if (incSending)
                    balance -= cost;
            }
            if (emailPatient != null)
            {
                messagesToEmail.Add(new Tuple<int, string, string, string, string>(booking.BookingID, booking.Organisation.Name, emailPatient, emailText, emailSubjectText));
                bookingIDsConfirmedEmail.Add(booking.BookingID);
            }


            /*
            bool sendingAlready = false;
            if (EnableDailyBookingReminderSMS && phoneNumPatient != null && balance >= cost)
            {
                messagesToSMS.Add(new Tuple<int, decimal, string, string, string>(booking.BookingID, cost, phoneNumPatient, smsText, booking.Organisation.Name));
                bookingIDsConfirmedSMS.Add(booking.BookingID);
                sendingAlready = true;
                if (incSending)
                    balance -= cost;
            }
            if (EnableDailyBookingReminderEmails && emailPatient != null)
            {
                messagesToEmail.Add(new Tuple<int, string, string, string, string>(booking.BookingID, booking.Organisation.Name, emailPatient, emailText, emailSubjectText));
                if (!sendingAlready)  // if not already added for sms sending
                    bookingIDsConfirmedEmail.Add(booking.BookingID);
            }
            */

        }
        output += "</table>";


        // run the sending and send off reminders -- but only if there was any bookings

        if (incSending && bookings.Length > 0)
        {

            /*
             * run the sendings
             */

            SendSMSes((Tuple<int, decimal, string, string, string>[])messagesToSMS.ToArray(typeof(Tuple<int, decimal, string, string, string>)));
            SendEmails((Tuple<int, string, string, string, string>[])messagesToEmail.ToArray(typeof(Tuple<int, string, string, string, string>)));

            /*
             * if sms or email sent, set booking as confirmed
             */
            BookingDB.UpdateSetConfirmed((int[])bookingIDsConfirmedSMS.ToArray(typeof(int)), 2, -1);
            BookingDB.UpdateSetConfirmed((int[])bookingIDsConfirmedEmail.ToArray(typeof(int)), 3, -1);

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
            return "Count: <b>" + countWithPatient + "</b> &nbsp;&nbsp; [Sending Via SMS: <b>" + messagesToSMS.Count + "</b>] &nbsp;&nbsp; [Sending Via Email: <b>" + messagesToEmail.Count + "</b>] " + "<br /><br />" + output;
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
            int     bookingID = messageInfo.Item1;
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

                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogSMSSending"]))
                    Logger.LogSMSSend(
                        System.Web.HttpContext.Current.Session["DB"].ToString() + Environment.NewLine + Environment.NewLine + 
                        mobile   + Environment.NewLine + Environment.NewLine + 
                        message  + Environment.NewLine + Environment.NewLine + 
                        callerId + Environment.NewLine + Environment.NewLine +
                        xmlResponse, 
                        true);

                string SMSTechMessageID = GetSMSTechMessageID(xmlResponse);
                SMSHistoryDataDB.Insert(1, -1, bookingID, mobile, message, cost, SMSTechMessageID);
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
            int bookingID  = messageInfo.Item1;
            string orgName = messageInfo.Item2;
            string email   = messageInfo.Item3;
            string message = messageInfo.Item4;
            string subject = messageInfo.Item5;

            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogSMSSending"]))
                    Logger.LogSMSSend(
                        System.Web.HttpContext.Current.Session["DB"].ToString() + Environment.NewLine + Environment.NewLine + 
                        email   + Environment.NewLine + Environment.NewLine + 
                        message + Environment.NewLine + Environment.NewLine + 
                        subject + Environment.NewLine + Environment.NewLine, 
                        true);

                SendEmail(orgName, email, subject, message);
                EmailHistoryDataDB.Insert(1, -1, bookingID, email, message);
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

    protected static string GetSMSText(Booking booking, string phoneNumOrg, string addrOrg, string text)
    {
        if (phoneNumOrg == null)
            phoneNumOrg = string.Empty;

        TimeSpan bkTime       = booking.DateEnd.Subtract(booking.DateStart);
        string bkTimeLength;
        if (bkTime.Minutes > 0 && bkTime.Hours > 0)
            bkTimeLength = bkTime.Hours + (bkTime.Hours > 1 ? "hrs " : "hr ") + bkTime.Minutes + " minutes";
        else if (bkTime.Hours > 0)
            bkTimeLength = bkTime.Hours + (bkTime.Hours > 1 ? "hours " : "hour ");
        else if (bkTime.Minutes > 0)
            bkTimeLength = bkTime.Minutes + bkTime.Minutes + " minutes";
        else
            bkTimeLength = bkTime.Minutes + " minutes";

        text = text.Replace("pt_firstname"      , booking.Patient != null ? booking.Patient.Person.Firstname                 : string.Empty);
        text = text.Replace("pt_fullname"       , booking.Patient != null ? booking.Patient.Person.FullnameWithoutMiddlename : string.Empty);
        text = text.Replace("provider_fullname" , booking.Provider.Person.FullnameWithoutMiddlename);
        text = text.Replace("service_field"     , booking.Offering.Field.Descr.ToLower());
        text = text.Replace("org_name"          , booking.Organisation.Name);
        text = (phoneNumOrg.Length > 0) ?
            text.Replace("org_phone", phoneNumOrg) :
            text.Replace("on org_phone", "").Replace(" org_phone", "").Replace("org_phone", "");
        text = (addrOrg != null && addrOrg.Length > 0) ?
            text.Replace("org_addr", addrOrg) :
            text.Replace("on org_addr", "").Replace(" org_addr", "").Replace("org_addr", "");
        text = text.Replace("bk_date"           , booking.DateStart.ToString("dddd") + " " + Utilities.GetDateOrdinal(booking.DateStart.Day));
        text = text.Replace("bk_time"           , booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm"));
        text = text.Replace("bk_length"         , bkTimeLength);


        return text;
    }

    protected static string GetEmailText(Booking booking, string phoneNumOrg, string addrOrg, string text)
    {
        if (phoneNumOrg == null)
            phoneNumOrg = string.Empty;

        if (addrOrg != null)
            addrOrg = addrOrg.Replace(Environment.NewLine, "<br />");

        TimeSpan bkTime = booking.DateEnd.Subtract(booking.DateStart);
        string bkTimeLength;
        if (bkTime.Minutes > 0 && bkTime.Hours > 0)
            bkTimeLength = bkTime.Hours + (bkTime.Hours > 1 ? "hrs " : "hr ") + bkTime.Minutes + " minutes";
        else if (bkTime.Hours > 0)
            bkTimeLength = bkTime.Hours + (bkTime.Hours > 1 ? "hours " : "hour ");
        else if (bkTime.Minutes > 0)
            bkTimeLength = bkTime.Minutes + bkTime.Minutes + " minutes";
        else
            bkTimeLength = bkTime.Minutes + " minutes";

        text = text.Replace("pt_firstname"      , booking.Patient != null ? booking.Patient.Person.Firstname                 : string.Empty);
        text = text.Replace("pt_fullname"       , booking.Patient != null ? booking.Patient.Person.FullnameWithoutMiddlename : string.Empty);
        text = text.Replace("provider_fullname" , booking.Provider.Person.FullnameWithoutMiddlename);
        text = text.Replace("service_field"     , booking.Offering.Field.Descr.ToLower());
        text = text.Replace("org_name"          , booking.Organisation.Name);
        text = (phoneNumOrg != null && phoneNumOrg.Length > 0) ?
            text.Replace("org_phone", phoneNumOrg) :
            text.Replace("on org_phone", "").Replace(" org_phone", "").Replace("org_phone", "");
        text = (addrOrg != null && addrOrg.Length > 0) ?
            text.Replace("org_addr", addrOrg) :
            text.Replace("on org_addr", "").Replace(" org_addr", "").Replace("org_addr", "");
        text = text.Replace("bk_date"           , booking.DateStart.ToString("dddd") + " " + Utilities.GetDateOrdinal(booking.DateStart.Day));
        text = text.Replace("bk_time"           , booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm"));
        text = text.Replace("bk_length"         , bkTimeLength);

        return text;
    }
    protected static string GetEmailSubjectText(Booking booking, string phoneNumOrg, string addrOrg, string text)
    {
        //return @"Reminder - Appointment at " + booking.Organisation.Name + @" tomorrow at " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm");

        TimeSpan bkTime = booking.DateEnd.Subtract(booking.DateStart);
        string bkTimeLength;
        if (bkTime.Minutes > 0 && bkTime.Hours > 0)
            bkTimeLength = bkTime.Hours + (bkTime.Hours > 1 ? "hrs " : "hr ") + bkTime.Minutes + " minutes";
        else if (bkTime.Hours > 0)
            bkTimeLength = bkTime.Hours + (bkTime.Hours > 1 ? "hours " : "hour ");
        else if (bkTime.Minutes > 0)
            bkTimeLength = bkTime.Minutes + bkTime.Minutes + " minutes";
        else
            bkTimeLength = bkTime.Minutes + " minutes";

        text = text.Replace("pt_firstname"      , booking.Patient != null ? booking.Patient.Person.Firstname                 : string.Empty);
        text = text.Replace("pt_fullname"       , booking.Patient != null ? booking.Patient.Person.FullnameWithoutMiddlename : string.Empty);
        text = text.Replace("provider_fullname" , booking.Provider.Person.FullnameWithoutMiddlename);
        text = text.Replace("service_field"     , booking.Offering.Field.Descr.ToLower());
        text = text.Replace("org_name"          , booking.Organisation.Name);
        text = (phoneNumOrg != null && phoneNumOrg.Length > 0) ?
            text.Replace("org_phone", phoneNumOrg) :
            text.Replace("on org_phone", "").Replace(" org_phone", "").Replace("org_phone", "");
        text = (addrOrg != null && addrOrg.Length > 0) ?
            text.Replace("org_addr", addrOrg) :
            text.Replace("on org_addr", "").Replace(" org_addr", "").Replace("org_addr", "");
        text = text.Replace("bk_date"           , booking.DateStart.ToString("dddd") + " " + Utilities.GetDateOrdinal(booking.DateStart.Day));
        text = text.Replace("bk_time"           , booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm"));
        text = text.Replace("bk_length"         , bkTimeLength);

        return text;
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

    protected static string GetAddr(Hashtable contactHash, int entityID)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] contacts = (Contact[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            for (int i = 0; i < contacts.Length; i++)
            {
                return contacts[i].GetFormattedAddress();
            }

            return null;

        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] contacts = (ContactAus[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            for (int i = 0; i < contacts.Length; i++)
            {
                return contacts[i].GetFormattedAddress();
            }

            return null;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
    }
    
    protected static Hashtable GetPatientPhoneNbrCache(Booking[] bookings)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Patient != null)
                entityIDArrayList.Add(curBooking.Patient.Person.EntityID);
        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));

        Hashtable contactHash = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1);

        return contactHash;
    }

    protected static Hashtable GetOrgPhoneNbrCache(Booking[] bookings)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Organisation != null)
                entityIDArrayList.Add(curBooking.Organisation.EntityID);
        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));
        Hashtable contactHash = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1, "34");

        return contactHash;
    }

    protected static Hashtable GetOrgAddrCache(Booking[] bookings)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Organisation != null)
                entityIDArrayList.Add(curBooking.Organisation.EntityID);
        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));
        Hashtable contactHash = PatientsContactCacheDB.GetBullkAddress(entityIDs, -1);

        return contactHash;
    }
    
    protected static Hashtable GetPatientEmailCache(Booking[] bookings)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Booking curBooking in bookings)
            if (curBooking.Patient != null)
                entityIDArrayList.Add(curBooking.Patient.Person.EntityID);
        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));

        Hashtable contactHash = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);

        return contactHash;
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