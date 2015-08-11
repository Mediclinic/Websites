using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;

using X509Certificates = System.Security.Cryptography.X509Certificates;

public class Emailer
{

    public static void SimpleErrorEmail(string message, bool isHTML = false)
    {
        SimpleAlertEmail(message, "Mediclinic Error [" + Environment.MachineName + "]", isHTML);
    }
    public static void SimpleAlertEmail(string message, string subject, bool isHTML = false)
    {
        AsyncSimpleEmail(
            System.Configuration.ConfigurationManager.AppSettings["ErrorEmail_FromEmail"],
            System.Configuration.ConfigurationManager.AppSettings["ErrorEmail_FromName"],
            System.Configuration.ConfigurationManager.AppSettings["ErrorEmail_To"],
            subject,
            message,
            isHTML,
            null);
    }


    public static void SimpleEmail(string to, string subject, string body, bool isHTML, string[] attachments, System.Net.Mail.AlternateView[] alternativeViews)
    {
        SimpleEmail(
            ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
            to,
            subject,
            body,
            isHTML,
            attachments,
            alternativeViews);
    }
    public static void SimpleEmail(string from_name, string to, string subject, string body, bool isHTML, string[] attachments, System.Net.Mail.AlternateView[] alternativeViews)
    {
        SimpleEmail(
            ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
            from_name,
            to,
            subject,
            body,
            isHTML,
            attachments,
            alternativeViews);
    }
    public static void SimpleEmail(string from_email, string from_name, string to, string subject, string body, bool isHTML, string[] attachments, System.Net.Mail.AlternateView[] alternativeViews)
    {
        SimpleEmail(
            from_email,
            from_name,
            to,
            subject,
            body,
            isHTML,
            System.Configuration.ConfigurationManager.AppSettings["Email_SMTPHost"],
            Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Email_SMTPPort"]),
            System.Configuration.ConfigurationManager.AppSettings["Email_AccountUsername"],
            System.Configuration.ConfigurationManager.AppSettings["Email_AccountPassword"],
            attachments,
            alternativeViews);
    }


    // async email sending
    // can not send attachments because the calling method will delete the attached files before the new thread has sent the emails...
    // for emailing with attachments, need to create their own async methods and in there create the emailing files, send, and clean up files

    protected delegate void AsyncSimpleEmailDelegate2(string from_email, string from_name, string to, string subject, string body, bool isHTML, System.Net.Mail.AlternateView[] alternativeViews);
    public static void AsyncSimpleEmail(string from_email, string from_name, string to, string subject, string body, bool isHTML, System.Net.Mail.AlternateView[] alternativeViews)
    {
        //create delegate and invoke it asynchrnously, control passes past this line while this is done in another thread
        AsyncSimpleEmailDelegate2 myAction = new AsyncSimpleEmailDelegate2(SyncEmail2);
        myAction.BeginInvoke(from_email, from_name, to, subject, body, isHTML, alternativeViews, null, null);
    }
    protected static void SyncEmail2(string from_email, string from_name, string to, string subject, string body, bool isHTML, System.Net.Mail.AlternateView[] alternativeViews)
    {
        SimpleEmail(
            from_email,
            from_name,
            to,
            subject,
            body,
            isHTML,
            null,
            alternativeViews);
    }


    protected static void SimpleEmail(string from_address, string from_name, string to, string subject, string body, bool isHTML,
                                   string smtp_host, int smtp_port, string accountUsername, string accountPassword, string[] attachments, System.Net.Mail.AlternateView[] alternativeViews)
    {
        if (isHTML && to.Contains(ConfigurationManager.AppSettings["EmailStringMatchToConvertToPlainText"]))
        {
            body = HtmlConverter.HTMLToText(body);
            isHTML = false;
        }


        System.Net.Mail.Attachment[] list = null;

        try
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress(from_address, from_name);
            foreach (string _to in to.Split(','))
                message.To.Add(_to.Trim());
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHTML;

            if (alternativeViews != null)
                for (int i = 0; i < alternativeViews.Length; i++)
                    message.AlternateViews.Add(alternativeViews[i]);


            if (attachments != null && attachments.Length > 0)
            {
                list = new System.Net.Mail.Attachment[attachments.Length];
                for (int i = 0; i < attachments.Length; i++)
                {
                    list[i] = new System.Net.Mail.Attachment(attachments[i]);
                    message.Attachments.Add(list[i]);
                }
            }

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtp_host, smtp_port);
            if (accountUsername.Length > 0 || accountPassword.Length > 0)
                smtp.Credentials = new System.Net.NetworkCredential(accountUsername, accountPassword);
            else
            {
                smtp.UseDefaultCredentials = false;

                // turn off validating certificate because it is throwing an error when using anonymous sending
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificates.X509Certificate certificate, X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
            }

            smtp.Timeout = 300000; // 300 sec = 5 mins

            smtp.EnableSsl = true;
            smtp.Send(message);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, false); // cant email it again if emailing is failing..
        }
        finally
        {
            // unlock the files they reference
            if (list != null)
                for (int i = 0; i < list.Length; i++)
                    list[i].Dispose();
        }
    }


    // async email sending
    // can not send attachments because the calling method will delete the attached files before the new thread has sent the emails...
    // for emailing with attachments, need to create their own async methods and in there create the emailing files, send, and clean up files

    protected delegate void AsyncSimpleEmailDelegate3(string from_address, string from_name, string to, string subject, string body, bool isHTML,
                               string smtp_host, int smtp_port, string accountUsername, string accountPassword, System.Net.Mail.AlternateView[] alternativeViews);
    public static void AsyncSimpleEmail(string from_address, string from_name, string to, string subject, string body, bool isHTML,
                                   string smtp_host, int smtp_port, string accountUsername, string accountPassword, System.Net.Mail.AlternateView[] alternativeViews)
    {
        //create delegate and invoke it asynchrnously, control passes past this line while this is done in another thread
        AsyncSimpleEmailDelegate3 myAction = new AsyncSimpleEmailDelegate3(SyncEmail3);
        myAction.BeginInvoke(from_address, from_name, to, subject, body, isHTML,
                                   smtp_host, smtp_port, accountUsername, accountPassword, alternativeViews, null, null);
    }
    protected static void SyncEmail3(string from_address, string from_name, string to, string subject, string body, bool isHTML,
                                   string smtp_host, int smtp_port, string accountUsername, string accountPassword, System.Net.Mail.AlternateView[] alternativeViews)
    {
        SimpleEmail(
            from_address,
            from_name,
            to,
            subject,
            body,
            isHTML,
            smtp_host,
            smtp_port,
            accountUsername,
            accountPassword,
            null,
            alternativeViews);
    }



    // add calendar attachment
    public static System.Net.Mail.AlternateView MakeCalendarView(DateTime start, DateTime end, string subject, string description, string location, string organiserName, string organiserEmail)
    {
        string ics = MakeICSFile(start, end, subject, description, location, organiserName, organiserEmail);

        System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
        contype.Parameters.Add("method", "REQUEST");
        contype.Parameters.Add("name", "Meeting.ics");
        System.Net.Mail.AlternateView avCal = System.Net.Mail.AlternateView.CreateAlternateViewFromString(ics, contype);
        return avCal;
    }
    protected static string MakeICSFile(DateTime start, DateTime end, string subject, string description, string location, string organiserName, string organiserEmail)
    {
        // http://www.c-sharpcorner.com/uploadfile/4cf18c/construct-and-send-an-ics-file-as-an-attachment-in-the-email/
        // http://www.codeproject.com/Articles/27451/Send-Calendar-Appointment-As-Email-Attachment

        // THERE IS NO TITLE IN THE CALENDAR EVENT!!!! 

        // Now Contruct the ICS file using string builder
        System.Text.StringBuilder str = new System.Text.StringBuilder();
        str.AppendLine("BEGIN:VCALENDAR");
        str.AppendLine("PRODID:-//Schedule a Meeting");
        str.AppendLine("VERSION:2.0");
        str.AppendLine("METHOD:REQUEST");
        str.AppendLine("BEGIN:VEVENT");
        str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmss}", start));
        str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmss}", end));
        str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmss}", DateTime.Now));
        str.AppendLine(string.Format("LOCATION:{0}", location));
        str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
        str.AppendLine(string.Format("DESCRIPTION:{0}", description));
        str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", description));
        str.AppendLine(string.Format("SUMMARY:{0}", subject));
        //str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", organiserEmail));
        str.AppendLine(string.Format("ORGANIZER;CN={0}:MAILTO:{1}", organiserName, organiserEmail));

        //str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", toName, toEmail));

        str.AppendLine("BEGIN:VALARM");
        str.AppendLine("TRIGGER:-PT15M");
        str.AppendLine("ACTION:DISPLAY");
        str.AppendLine("DESCRIPTION:Reminder");
        str.AppendLine("END:VALARM");
        str.AppendLine("END:VEVENT");
        str.AppendLine("END:VCALENDAR");

        return str.ToString();
    }

    // when adding an alternative view, the body of the email will no longer be shown as html
    // unless we move the body into its own alterntive view as html
    public static System.Net.Mail.AlternateView MakeBodyView(string body)
    {
        return System.Net.Mail.AlternateView.CreateAlternateViewFromString(body, System.Text.Encoding.UTF8, System.Net.Mime.MediaTypeNames.Text.Html);
    }


}