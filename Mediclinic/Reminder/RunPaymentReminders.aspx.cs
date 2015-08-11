using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections;
using System.Configuration;


public partial class RunPaymentReminders : System.Web.UI.Page
{

    // http://portal.mediclinic.com.au:803/Reminder/RunPaymentReminders.aspx?pwd=mah_sms_reminder
    // http://portal.mediclinic.com.au:803/Reminder/RunPaymentReminders.aspx?pwd=mah_sms_reminder&inc_sending=false


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {

            string pwd = Request.Form["pwd"];

            if (pwd != null)  // sent by http post
            {
                if (pwd == null || pwd != System.Configuration.ConfigurationManager.AppSettings["SMSRunRemindersPwd"])
                    throw new CustomMessageException("Incorrect password");

                string exceptionOutput = string.Empty; 

                if (Session != null && Session["DB"] != null)
                {
                    throw new CustomMessageException("Can not run this while logged in.");
                }
                else
                {

                    System.Data.DataTable tbl = DBBase.ExecuteQuery("EXEC sp_databases;", "master").Tables[0];
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        string databaseName = tbl.Rows[i][0].ToString();

                        if (!Regex.IsMatch(databaseName, @"Mediclinic_\d{4}"))
                            continue;

                        try
                        {
                            Session["DB"] = databaseName;
                            Session["SystemVariables"] = SystemVariableDB.GetAll();

                            Run(false, true);
                        }
                        catch (Exception ex)
                        {
                            exceptionOutput += Environment.NewLine + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " " + "DB: " + databaseName;
                            exceptionOutput += Environment.NewLine + ex.ToString();
                        }
                        finally
                        {
                            Session.Remove("DB");
                            Session.Remove("SystemVariables");
                        }
                    }

                }

                if (exceptionOutput.Length > 0)
                {
                    Response.Write("Run Completed But With Errors!");
                    Response.Write(Environment.NewLine + exceptionOutput);
                }
                else
                {
                    Response.Write("Run Completed!");
                }
            }

        }
        catch (CustomMessageException ex)
        {
            Response.Write(ex.Message);
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (ex.ToString()));
        }
    }

    protected void Run(bool incDisplay, bool incSending)
    {

        // 1. get all fields from systemvariables

        int DaysBeforePaymentDueToSendReminder = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DaysBeforePaymentDueToSendReminder"]);
        int PaymentDueDayOfMonth               = Convert.ToInt32(SystemVariableDB.GetByDescr("PaymentDueDayOfMonth").Value);

        string CustomerEmail                   = SystemVariableDB.GetByDescr("AdminAlertEmail_To").Value;
        string DB                              = System.Web.HttpContext.Current.Session["DB"].ToString();

        /*
        // log for debugging
        Hashtable logFile = new Hashtable();
        logFile["SMSSender_Scheduled_LogFile_Eli-PC"]     = @"C:\Users\Eli\Documents\Mediclinic\SMSSender_Scheduled_LogFile2.txt";
        logFile["SMSSender_Scheduled_LogFile_ELI-LAPTOP"] = @"D:\Dropbox\Mediclinic\SMSSender_Scheduled_LogFile2.txt";
		logFile["SMSSender_Scheduled_LogFile_box001"]     = @"C:\inetpub\sites\MediclinicDocs\SMSSender_Scheduled_LogFile2.txt";
        Logger.WriteToFile(
            ((DateTime.Today.Date != GetDueDateThisMonth(PaymentDueDayOfMonth, DaysBeforePaymentDueToSendReminder, DateTime.Today)) ? "Not Sent -- " : "Sent -- ") +
            @"DB = " + DB + " Today = " + DateTime.Today.ToString("dd-MM") + " SendDate = " + GetDueDateThisMonth(PaymentDueDayOfMonth, DaysBeforePaymentDueToSendReminder, DateTime.Today).ToString("dd-MM") + Environment.NewLine, 
            logFile["SMSSender_Scheduled_LogFile_" + Environment.MachineName].ToString());
        */


        // 2. validate

        if (DateTime.Today.Date != GetDueDateThisMonth(PaymentDueDayOfMonth, DaysBeforePaymentDueToSendReminder, DateTime.Today))
            return;


        // 3. run it -- send reminder email to marcus

        string emailBody = @"
Mediclinic payment due date is coming up for :
<br />
<br />
<table>
    <tr><td>Customer Email</td><td>" + CustomerEmail + @"</td></tr>
    <tr><td>Due Day Of Month</td><td>" + Utilities.GetDateOrdinal(PaymentDueDayOfMonth) + @"</td></tr>
    <tr><td>Database</td><td>" + DB + @"</td></tr>
</table>";

        Emailer.AsyncSimpleEmail(
            System.Configuration.ConfigurationManager.AppSettings["ErrorEmail_FromEmail"],
            System.Configuration.ConfigurationManager.AppSettings["ErrorEmail_FromName"],
            System.Configuration.ConfigurationManager.AppSettings["TopupEmail_To"],
            "Mediclinic Customer Payment Due",
            emailBody,
            true,
            null);
    }

    protected DateTime GetDueDateThisMonth(int dayOfMonth, int daysBefore, DateTime curDate)
    {
        int weekBeforeDay = dayOfMonth - daysBefore;

        DateTime weekBeforeDate;
        if (weekBeforeDay <= 0)
        {
            DateTime dateAddMonth = curDate.AddMonths(1);
            DateTime dateFirstNextMonth = new DateTime(dateAddMonth.Year, dateAddMonth.Month, 1);
            weekBeforeDate = dateFirstNextMonth.AddDays(weekBeforeDay - 1);
        }
        else
        {
            int daysThisMonth = DateTime.DaysInMonth(curDate.Year, curDate.Month);

            // check this date exists - eg 31 feb -> change to last day of feb
            if (weekBeforeDay > daysThisMonth)
                weekBeforeDay = daysThisMonth;

            weekBeforeDate = new DateTime(curDate.Year, curDate.Month, weekBeforeDay);
        }

        return weekBeforeDate;
    }

}
