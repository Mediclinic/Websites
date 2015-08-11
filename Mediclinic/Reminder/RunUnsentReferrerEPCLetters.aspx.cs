using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections;
using System.Configuration;


public partial class RunUnsentReferrerEPCLetters : System.Web.UI.Page
{

    // http://portal.mediclinic.com.au:803/Reminder/RunUnsentReferrerEPCLetters.aspx?pwd=mah_sms_reminder
    // http://portal.mediclinic.com.au:803/Reminder/RunUnsentReferrerEPCLetters.aspx?pwd=mah_sms_reminder&inc_sending=false


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
                    Run(false, true);
                }
                else
                {

                    if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseConfigDB"]))
                    {
                        Session["DB"] = System.Configuration.ConfigurationManager.AppSettings["Database"];
                        Session["SystemVariables"] = SystemVariableDB.GetAll();
                        Run(false, true);
                        Session.Remove("DB");
                        Session.Remove("SystemVariables");
                    }
                    else // Get all DB's and run it on all of them
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
                    Run(true, incSending);
                }
                else
                {
                    if (!Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseConfigDB"]))
                        throw new CustomMessageException("Can not run for logged out user where UseConfigDB = false");
                    else
                    {
                        Session["DB"] = System.Configuration.ConfigurationManager.AppSettings["Database"];
                        Session["SystemVariables"] = SystemVariableDB.GetAll();

                        Run(true, incSending);

                        Session.Remove("DB");
                        Session.Remove("SystemVariables");
                    }
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

        bool   EnableEmails   = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendEmail").Value)  == 1;
        string EmailAddress   = SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_EmailAddress").Value;

        bool IncClinicsAuto   = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_IncClinic").Value)     == 1;
        bool IncAgedCareAuto  = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_IncAgedCare").Value)   == 1;

        bool IncUnsentAuto    = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_IncUnsent").Value)  == 1;
        bool IncBatchedAuto   = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_IncBatched").Value) == 1;

        ReferrerEPCLettersSending.SendMethod sendMethod = SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendMethod").Value == "Email" ?
            ReferrerEPCLettersSending.SendMethod.Email_To_Referrer :
            ReferrerEPCLettersSending.SendMethod.Batch;

        bool SendMondays    = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendMondays").Value)    == 1;
        bool SendTuesdays   = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendTuesdays").Value)   == 1;
        bool SendWednesdays = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendWednesdays").Value) == 1;
        bool SendThursdays  = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendThursdays").Value)  == 1;
        bool SendFridays    = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendFridays").Value)    == 1;
        bool SendSaturdays  = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendSaturdays").Value)  == 1;
        bool SendSundays    = Convert.ToInt32(SystemVariableDB.GetByDescr("ReferrerEPCAutoGenerateLettersEmail_SendSundays").Value)    == 1;



        // 2. validate

        if (!EnableEmails) return;
        if (!Utilities.IsValidEmailAddresses(EmailAddress, false)) return;
        if (!IncClinicsAuto && !IncAgedCareAuto) return;

        if (DateTime.Today.DayOfWeek == DayOfWeek.Monday   && !SendMondays)    return;
        if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday  && !SendTuesdays)   return;
        if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday&& !SendWednesdays) return;
        if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday && !SendThursdays)  return;
        if (DateTime.Today.DayOfWeek == DayOfWeek.Friday   && !SendFridays)    return;
        if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday && !SendSaturdays)  return;
        if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday   && !SendSundays)    return;



        // 3. run it

        int siteID = -1;

        if (IncClinicsAuto && IncAgedCareAuto)
            siteID = -1;
        else if (IncClinicsAuto && !IncAgedCareAuto)
        {
            foreach (Site s in SiteDB.GetAll())
                if (s.SiteType.ID == 1) siteID = s.SiteID;
        }
        else if (!IncClinicsAuto && IncAgedCareAuto)
        {
            foreach (Site s in SiteDB.GetAll())
                if (s.SiteType.ID == 2) siteID = s.SiteID;
        }


        string outputInfo;
        string outputList;

        Letter.FileContents fileContents = ReferrerEPCLettersSending.Run(
            sendMethod,
            siteID,
            -1,
            -1,
            IncBatchedAuto,
            IncUnsentAuto,
            !incSending,
            false,
            out outputInfo,
            out outputList,
            string.Empty
            );

        if (incDisplay)
            Response.Write(outputInfo + "<br /><br />" + outputList);



        // 4. Put in file and email it

        if (fileContents != null)
        {

            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            if (!System.IO.Directory.Exists(tmpLettersDirectory))
                throw new CustomMessageException("Temp letters directory doesn't exist");

            string tmpDir = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
            System.IO.Directory.CreateDirectory(tmpDir);
            string tmpFileName = tmpDir + fileContents.DocName;
            System.IO.File.WriteAllBytes(tmpFileName, fileContents.Contents);

            Emailer.SimpleEmail(EmailAddress, "Automated Referral Letters [" + ((SystemVariables)Session["SystemVariables"])["Site"].Value + "]", "Please find attached referral letters to send to referrers.<br /><br />Regards,<br />Mediclinic", true, new string[] { tmpFileName }, null);

            System.IO.File.Delete(tmpFileName);
            System.IO.Directory.Delete(tmpDir);
        }
    }

}
