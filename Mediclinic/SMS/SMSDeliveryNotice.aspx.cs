using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections;

public partial class SMSDeliveryNotice : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            // [message_id] => 100000 [mobile] => 6140000000 [datetime] => 2012-04-27 10:38:00 [status] => ok
            string message_id  = Request.Form["message_id"];
            string mobile      = Request.Form["mobile"];
            string datetime    = Request.Form["datetime"];
            string status      = Request.Form["status"];

            string info =
                "message_id  : " + message_id + Environment.NewLine +
                "mobile      : " + mobile     + Environment.NewLine +
                "datetime    : " + datetime   + Environment.NewLine +
                "status      : " + status     + Environment.NewLine;

            if (message_id != null)
            {
                System.Data.DataTable tbl = DBBase.ExecuteQuery("EXEC sp_databases;", "master").Tables[0];
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    string databaseName = tbl.Rows[i][0].ToString();

                    if (!Regex.IsMatch(databaseName, @"Mediclinic_\d{4}"))
                        continue;

                    Session["DB"] = databaseName;
                    Session["SystemVariables"] = SystemVariableDB.GetAll();
                    SMSHistoryDataDB.Update(message_id, status);
                    Session.Remove("DB");
                    Session.Remove("SystemVariables");
                }
            }


            //
            // NO NEED TO RELAY TO OTHER SITES - WE ALREADY UPDATED ALL DATABASES
            //


            //
            // relay to other site(s)
            //


            /*
            string is_relay = Request.Form["is_relay"];
            if (is_relay == null)
            {

                string[] urls = System.Configuration.ConfigurationManager.AppSettings["SMSDeliveryOtherSites"].Length == 0 ? new string[] { } : System.Configuration.ConfigurationManager.AppSettings["SMSDeliveryOtherSites"].Split(';');

                for (int i = 0; i < urls.Length; i++)
                {
                    string url = urls[i];

                    try
                    {
                        //string response = HttpGet(((SystemVariables)Session["SystemVariables"])["Url1"].Value);
                        string response = Utilities.HttpPost(url, new System.Collections.Specialized.NameValueCollection() {
							    { "message_id", message_id }, 
							    { "mobile",     mobile     }, 
							    { "datetime",   datetime   }, 
							    { "status",     status     },
							    { "is_relay",   "1"        }
						    });
                    }
                    catch (Exception ex)
                    {
                        Logger.LogSMSDelivery("Timestamp: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " [" + url + "]" + Environment.NewLine + Environment.NewLine + ex.ToString(), true);
                    }

                }
            }
            */

        }
        catch (Exception ex)
        {
            Logger.LogSMSDelivery(ex.ToString(), true);
        }
    }



}