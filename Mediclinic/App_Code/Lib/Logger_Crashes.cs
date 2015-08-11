using System;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class Logger_Crashes // This file causes the crash!
{

    public static void LogException(Exception ex, bool emailAlso = true)
    {

        string[] logFiles = new string[]{
                ConfigurationManager.AppSettings["ExceptionLogFile"]
            };
        LogIt(emailAlso, ex.ToString(), logFiles);
    }


    public static void LogSMSSend(string info, bool emailAlso = false, bool logSingleLine = false)
    {
        string[] logFiles = new string[]{
                System.Configuration.ConfigurationManager.AppSettings["SMSSendLogFile"]
            };
        LogIt(emailAlso, info, logFiles, logSingleLine);
    }
    public static void LogSMSDelivery(string info, bool emailAlso = false, bool logSingleLine = false)
    {
        string[] logFiles = new string[]{
                System.Configuration.ConfigurationManager.AppSettings["SMSDeliveryLogFile"]
            };
        LogIt(emailAlso, info, logFiles, logSingleLine);
    }

    public static void LogRequest(System.Web.HttpRequest request, bool emailAlso = false, bool logSingleLine = false)
    {
        bool logIt = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogAllRequests"]);
        if (logIt)
        {
            string LogAllRequestsFile = System.Configuration.ConfigurationManager.AppSettings["LogAllRequestsFile"];
            if (LogAllRequestsFile.EndsWith(".txt"))
                LogAllRequestsFile = LogAllRequestsFile.Substring(0, LogAllRequestsFile.Length - 4) + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";

            string[] logFiles = new string[]{
                LogAllRequestsFile
            };

            string Database = System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session["DB"] == null ? "               " : System.Web.HttpContext.Current.Session["DB"].ToString();
            System.Web.HttpContext context = System.Web.HttpContext.Current;

            string UserID = string.Empty;
            try
            {
                UserID = context == null || context.Session["StaffID"] == null ? string.Empty : context.Session["StaffID"].ToString();
            }
            catch (Exception ex)
            {}
            UserID = UserID.PadLeft(4, ' ');

            string info = "[" + Database + "] [" + UserID + "] " + request.RawUrl;

            LogIt(emailAlso, info, logFiles, logSingleLine);
        }
    }


    public static void LogQuery(string sql, bool includeStackTrace = false, bool emailAlso = false, bool logSingleLine = false)
    {
        bool logIt = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AllowSqlLogging"]);
        if (logIt)
        {
            if (includeStackTrace)
            {
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();

                string s = string.Empty;
                s += Environment.NewLine;
                s += Environment.NewLine;
                s += "Calling Medhod : " + stackTrace.GetFrame(2).GetMethod().DeclaringType.Name + "." +  stackTrace.GetFrame(2).GetMethod().Name + "()" + Environment.NewLine;
                s += Environment.NewLine;

                sql = s + sql;
            }

            string[] logFiles = new string[]{
                System.Configuration.ConfigurationManager.AppSettings["SqlLogFile"]
            };
            LogIt(emailAlso, sql, logFiles, logSingleLine);
        }
    }

    public static void LogCallingMethod(bool logSingleLine = false)
    {
        bool logIt = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AllowSqlLogging"]);
        if (logIt)
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            string s = "Calling Medhod : " + stackTrace.GetFrame(2).GetMethod().DeclaringType.Name + "." +  stackTrace.GetFrame(2).GetMethod().Name + "()" + (logSingleLine ? "" : Environment.NewLine);

            string[] logFiles = new string[]{
                System.Configuration.ConfigurationManager.AppSettings["SqlLogFile"]
            };
            LogIt(false, s, logFiles, logSingleLine);
        }
    }



    public static void LogMailMerge(bool emailAlso, string info)
    {
        bool logIt = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["LogMSWordMailMerge"]);
        if (logIt)
        {
            string[] logFiles = new string[]{
                System.Configuration.ConfigurationManager.AppSettings["MSWordMailMergeLogFile"]
            };
            LogIt(emailAlso, info, logFiles, true);
        }
    }


    private static void LogIt(bool emailAlso, string info, string[] logFiles, bool logSingleLine = false)
    {
        String txtOutput  = string.Empty;
        String htmlOutput = string.Empty;

        if (logSingleLine)
        {
            txtOutput  += "[" + DateTime.Now.ToString("dd MMM HH:mm:ss.fff") + "] " + info + Environment.NewLine;
            htmlOutput += "[" + DateTime.Now.ToString("dd MMM HH:mm:ss.fff") + "] " + info + Environment.NewLine;
        }
        else
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;

            string Database = System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session["DB"] == null ?  "" : System.Web.HttpContext.Current.Session["DB"].ToString();
            string ConnStr  = System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session["DB"] == null ? "" : DBBase.GetConnectionString();


            string site     = System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session["SystemVariables"] == null ? 
                "" : 
                ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Site"].Value;


            string siteType = string.Empty;
            try
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["SystemVariables"] != null)
                {
                    int siteTypeID = Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteTypeID"]);
                    if (siteTypeID == 1)
                        siteType = "Clinic";
                    if (siteTypeID == 2)
                        siteType = "Aged Care";
                    if (siteTypeID == 3)
                        siteType = "GP";
                }
            }
            catch (Exception) { } // make sure this doesn't crash the logging




            // create text error output

            txtOutput += "------------------------" + Environment.NewLine;
            txtOutput += "• DateTime : " + DateTime.Now.ToString("ddd dd MMM   HH:mm:ss.fff") + ": " + Environment.NewLine;
            txtOutput += "• Machine : " + Environment.MachineName + ": " + Environment.NewLine;
            txtOutput += "• URL : " + (context != null ? context.Request.Url.AbsoluteUri : "NO URL") + ": " + Environment.NewLine;
            txtOutput += "• IP : " + (context != null ? context.Request.UserHostAddress + " (" + GetIPCountry(context.Request.UserHostAddress.ToString()) + ")" : "NO IP") + ": " + Environment.NewLine;

            try
            {
                txtOutput += "• User: [ID:] " + (context == null || context.Session["StaffID"] == null ? string.Empty : context.Session["StaffID"]) + " [Name:] " + (context == null || context.Session["StaffFullnameWithoutMiddlename"] == null ? string.Empty : context.Session["StaffFullnameWithoutMiddlename"]) + Environment.NewLine;
            }
            catch (Exception ex)
            {
                txtOutput += Environment.NewLine;
                txtOutput += Environment.NewLine;
                txtOutput += Environment.NewLine;

                txtOutput += "FAILURE TO GET USER ID AND USERNAME FROM SESSION - THREW AN ERROR:" + Environment.NewLine;
                txtOutput += ex.ToString() + Environment.NewLine;

                txtOutput += Environment.NewLine;
                txtOutput += Environment.NewLine;
                txtOutput += Environment.NewLine;
            }

            txtOutput += "• Database : " + Database + " (" + site + ")" + Environment.NewLine;
            txtOutput += "• Site     : " + siteType + Environment.NewLine;
            //txtOutput += "• ConnStr  : " + ConnStr + Environment.NewLine;
            txtOutput += "• " + DateTime.Now.ToString("dd MMM HH:mm:ss.fff") + Environment.NewLine + Environment.NewLine;
            txtOutput += info + Environment.NewLine;
            txtOutput += "------------------------" + Environment.NewLine;


            // create html error output

            htmlOutput += "<hr>" + Environment.NewLine;

            htmlOutput += "<table>" + Environment.NewLine;
            htmlOutput += "<tr><td>DateTime</td><td><b>" + DateTime.Now.ToString("ddd dd MMM   HH:mm:ss.fff") + "</b></td></tr>" + Environment.NewLine;
            htmlOutput += "<tr><td>Machine</td><td><b>" + Environment.MachineName + "</b></td></tr>" + Environment.NewLine;
            htmlOutput += "<tr><td>URL</td><td>" + (context != null ? context.Request.Url.AbsoluteUri : "NO URL") + "</td></tr>" + Environment.NewLine;
            htmlOutput += "<tr><td>IP</td><td>" + (context != null ? context.Request.UserHostAddress + " (" + GetIPCountry(context.Request.UserHostAddress.ToString()) + ")" : "NO IP") + "</td></tr>" + Environment.NewLine;

            try
            {
                htmlOutput += "<tr><td>User</td><td><b>" + (context.Session["StaffFullnameWithoutMiddlename"] == null ? string.Empty : context.Session["StaffFullnameWithoutMiddlename"]) + " [ID: " + (context.Session["StaffID"] == null ? string.Empty : context.Session["StaffID"]) + "]" + "</b></td></tr>" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                htmlOutput += "<tr><td>User</td><td><font color=\"red\">" + 
                    "FAILURE TO GET USER ID AND USERNAME FROM SESSION - THREW AN ERROR:<br>" + ex.ToString().Replace(Environment.NewLine, "<br />" + Environment.NewLine)  +
                    "</font></td></tr>" + Environment.NewLine;
            }


            htmlOutput += "<tr><td>Database</td><td><b>" + Database + " (" + site + ")</b></td></tr>" + Environment.NewLine;
            htmlOutput += "<tr><td>Site</td><td><b>" + siteType + "</b></td></tr>" + Environment.NewLine;
            //htmlOutput += "<tr><td>ConnStr</td><td><b>" + ConnStr + "</b></td></tr>" + Environment.NewLine;
            htmlOutput += "</table>" + Environment.NewLine;
            htmlOutput += "<br /><br />" + Environment.NewLine;

            htmlOutput += info.Replace(Environment.NewLine, "<br>" + Environment.NewLine) + "<br />" + Environment.NewLine;
            htmlOutput += "<hr>" + Environment.NewLine;
        }

        foreach (string logFile in logFiles)
            WriteToFile(txtOutput, logFile);

        if (emailAlso)
            Emailer.SimpleErrorEmail(htmlOutput, true);
    }

    /*

     Change To Lock The File For Writing:

    http://stackoverflow.com/questions/4127234/writing-log-to-a-lock-for-edit-file-which-throws-exception-how-to-fix-this

    public static class FileAppendHelper
    {
        private static readonly object _syncRoot = new object();

        public static void AppendToFile(string appendString)
        {
            lock (_syncRoot)
            {
                File.AppendAllText("log.txt", appendString);
            }
        }
    }


    */

    public static void WriteToFile(string output, string logFile)
    {
        if (!File.Exists(logFile))
        {
            using (StreamWriter SW = File.CreateText(logFile))
            {
                ;
            }
        }

        if (!File.Exists(logFile))
        {
            using (StreamWriter SW = File.CreateText(logFile))
            {
                SW.Write(output);
            }
        }
        else
        {
            using (StreamWriter SW = File.AppendText(logFile))
            {
                SW.Write(output);
            }
        }
    }



    protected static string GetIPCountry(string ip)
    {
        Hashtable ipHash = GetIpHash( new string[] { ip } );
        return ipHash[ip] == null ? "" : ipHash[ip].ToString();
    }
    protected static Hashtable GetIpHash(string[] ips)
    {
        string sql = string.Empty;
        ArrayList uniqueIPs = new ArrayList();
        Hashtable seenIPs = new Hashtable();
        foreach (string ip in ips)
        {
            if (seenIPs[ip] != null) continue;
            if (!Regex.IsMatch(ip, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$")) continue;

            uint ipNum = ip.Split('.').Select(uint.Parse).Aggregate((a, b) => a * 256 + b);
            sql += @" SELECT TOP 1 c.country FROM  ip2nationCountries c LEFT JOIN ip2nation i ON c.code = i.country  WHERE  i.ip < " + ipNum + @" ORDER BY  i.ip DESC " + Environment.NewLine;

            uniqueIPs.Add(ip);
            seenIPs[ip] = 1;
        }
        ips = (string[])uniqueIPs.ToArray(typeof(string));

        Hashtable ipHash = new Hashtable();
        if (sql.Length > 0)
        {
            System.Data.DataSet ds = DBBase.ExecuteQuery(sql);
            for (int i = 0; i < ips.Length; i++)
                ipHash[ips[i]] = ds.Tables[i].Rows[0][0].ToString();
        }
        return ipHash;
    }

}

