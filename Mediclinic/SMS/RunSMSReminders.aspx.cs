using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections;
using System.Configuration;

using System.Xml;
using TransmitSMSAPI;

public partial class RunSMSReminders : System.Web.UI.Page
{

    // http://localhost:2524/SMS/RunSMSReminders.aspx?pwd=mah_sms_reminder
    // http://localhost:2524/SMS/RunSMSReminders.aspx?pwd=mah_sms_reminder&inc_sending=false

    // http://portal.mediclinic.com.au/SMS/RunSMSReminders.aspx?pwd=mah_sms_reminder
    // http://portal.mediclinic.com.au/SMS/RunSMSReminders.aspx?pwd=mah_sms_reminder&inc_sending=false

    // can upload and let marcus add booking, view, mobile nbr, view, then try send to his nbr in aus...


    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {

        Page.Server.ScriptTimeout = 7500;

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
                    SmsAndEmailBookingReminders.Run(false, true, DateTime.Now.Date.AddDays(1));
                }
                else
                {

                    if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseConfigDB"]))
                    {
                        Session["DB"] = System.Configuration.ConfigurationManager.AppSettings["Database"];
                        Session["SystemVariables"] = SystemVariableDB.GetAll();
                        SmsAndEmailBookingReminders.Run(false, true, DateTime.Now.Date.AddDays(1));
                        Session.Remove("DB");
                        Session.Remove("SystemVariables");
                    }
                    else // Get all DB's and run it on all of them
                    {
                        string logFile = ConfigurationManager.AppSettings["SMSSender_Scheduled_LogFile" + "_" + System.Environment.MachineName];
                        string fileNameOnly = System.IO.Path.GetFileNameWithoutExtension(logFile);
                        string extension    = System.IO.Path.GetExtension(logFile);
                        string path         = System.IO.Path.GetDirectoryName(logFile);
                        logFile = System.IO.Path.Combine(path, string.Format("{0}({1})", fileNameOnly, "_2") + extension);


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

                                Logger.WriteToFile("[" + DateTime.Now.ToString("dd MMM HH:mm:ss.fff") + "] " + "Starting RunSMSReminders.aspx DB: " + databaseName + Environment.NewLine, logFile);
                                SmsAndEmailBookingReminders.Run(false, true, DateTime.Now.Date.AddDays(1));
                                Logger.WriteToFile("[" + DateTime.Now.ToString("dd MMM HH:mm:ss.fff") + "] " + "Finished RunSMSReminders.aspx DB: " + databaseName + Environment.NewLine, logFile);
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
            else // send in url by http get
            {
                pwd = Request.QueryString["pwd"];
                if (pwd == null || pwd != System.Configuration.ConfigurationManager.AppSettings["SMSRunRemindersPwd"])
                    throw new CustomMessageException("Incorrect password");


                string inc_sending = Request.QueryString["inc_sending"];
                if (inc_sending != null && inc_sending != "true" && inc_sending != "false")
                    throw new CustomMessageException("Incorrect inc_sending value");
                bool incSending = inc_sending != null && inc_sending == "true";



                if (Session != null && Session["DB"] != null)
                {
                    string displayData = SmsAndEmailBookingReminders.Run(true, incSending, DateTime.Now.Date.AddDays(1));
                    Response.Write(displayData);
                }
                else
                {
                    if (!Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseConfigDB"]))
                        throw new CustomMessageException("Can not run for logged out user where UseConfigDB = false");
                    else
                    {
                        Session["DB"] = System.Configuration.ConfigurationManager.AppSettings["Database"];
                        Session["SystemVariables"] = SystemVariableDB.GetAll();

                        string displayData = SmsAndEmailBookingReminders.Run(true, incSending, DateTime.Now.Date.AddDays(1));
                        Response.Write(displayData);

                        Session.Remove("DB");
                        Session.Remove("SystemVariables");
                    }
                }
            }

        }
        catch (CustomMessageException ex)
        {
            try {
                Logger.LogQuery(" [From A] " + Environment.NewLine + ex.ToString(), true, true, false);
            } catch (Exception) { }

            Response.Write(ex.Message);
        }
        catch (Exception ex)
        {
            try
            {
                Logger.LogQuery(" [From B] " + Environment.NewLine + ex.ToString(), true, true, false);
            }
            catch (Exception) { }

            Response.Write("Exception: " + (ex.ToString()));
        }
    }

    #endregion



}
