using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Text;


public partial class TyroReconcilliation : System.Web.UI.Page
{

    // http://portal.mediclinic.com.au:803/Reconcilliations/TyroReconcilliation.aspx?pwd=mah_sms_reminder


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

                        Session["DB"] = databaseName;
                        Session["SystemVariables"] = SystemVariableDB.GetAll();
                        Run();
                        Session.Remove("DB");
                        Session.Remove("SystemVariables");
                    }
                }

                Response.Write("Run Completed!");
            }
            else // send in url by http get
            {
                pwd = Request.QueryString["pwd"];
                if (pwd == null || pwd != System.Configuration.ConfigurationManager.AppSettings["SMSRunRemindersPwd"])
                    throw new CustomMessageException("Incorrect password");


                if (Session != null && Session["DB"] != null)
                {
                    Run();
                }
                else
                {
                    if (!Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseConfigDB"]))
                        throw new CustomMessageException("Can not run for logged out user where UseConfigDB = false");
                    else
                    {
                        Session["DB"] = System.Configuration.ConfigurationManager.AppSettings["Database"];
                        Session["SystemVariables"] = SystemVariableDB.GetAll();
                        Run();
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

    protected void Run()
    {
        StringBuilder debugOutput = new StringBuilder();


        DataTable terminals = null; // get from table                 // id, MID/TID/Key, Descr (eg which clinic location, etc)
        for (int i = 0; i < terminals.Rows.Count; i++)
        {
            // get from DB
            string mid = string.Empty;
            string tid = string.Empty;
            string key = string.Empty;  // like a terminal password
            DateTime lastDateSuccessfullyRetrieved = DateTime.Today.AddDays(-7);   // if null, set to 28 days ago

            bool successful = true;
            for (DateTime date = lastDateSuccessfullyRetrieved.AddDays(1); date <= DateTime.Today; date = date.AddDays(1))
            {
                string format     = "xml";
                string reportType = "detail";
                string url        = "https://integrationsimulator.test.tyro.com/terminals/" + mid + "/" + tid + "/reconciliationReports/" + DateTime.Today.ToString("yyyyMMdd") + "." + format + "?key=" + key + "&reportType=" + reportType;

                debugOutput.AppendLine("url: " + url);

                try
                {
                    System.Net.HttpStatusCode httpResponseStatusCode;
                    string output = Utilities.HttpGet(url, out httpResponseStatusCode, "Mediclinic/1.0");

                    if ((int)httpResponseStatusCode != 200)
                    {
                        debugOutput.AppendLine("failed: " + httpResponseStatusCode + " (" + (int)httpResponseStatusCode + ")");
                        successful = false; // failed, so don't set lastDateSuccessfullyRetrieved for this day or beyond

                        // email alert
                    }
                    else
                    {
                        debugOutput.AppendLine("output: " + Environment.NewLine + output);

                        // process result
                    }
                }
                catch (Exception ex)
                {
                    // email alert with 'debugOutput' and 'ex.ToString()'
                }

                if (successful)
                {
                    ; // update lastDateSuccessfullyRetrieved for this TID
                }

            }
        }

    
    }
}
