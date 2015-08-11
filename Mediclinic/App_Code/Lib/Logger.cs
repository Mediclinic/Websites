using System;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class Logger
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

            string IP       = string.Empty;
            string Country  = string.Empty;
            string SiteType = string.Empty;
            try
            {
                IP      = context == null || context.Request == null || context.Request.UserHostAddress == null ? "" : context.Request.UserHostAddress.ToString();
                Country = context == null || context.Request == null || context.Request.UserHostAddress == null ? "" : GetIPCountry(context.Request.UserHostAddress.ToString(), Database == "" ? "Mediclinic_0001" : null);
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["SystemVariables"] != null)
                {
                    int siteTypeID = Convert.ToInt32(System.Web.HttpContext.Current.Session["SiteTypeID"]);
                    if (siteTypeID == 1)
                        SiteType = "Clinic";
                    if (siteTypeID == 2)
                        SiteType = "Aged Care";
                    if (siteTypeID == 3)
                        SiteType = "GP";
                }            
            }
            catch(Exception) { }

            string site     = System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session["SystemVariables"] == null ? 
                "" : 
                ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Site"].Value;


            // create text error output

            txtOutput += "------------------------" + Environment.NewLine;
            txtOutput += "• Machine : " + Environment.MachineName                                        + " : " + Environment.NewLine;
            txtOutput += "• URL     : " + (context != null ? context.Request.Url.AbsoluteUri : "NO URL") + " : " + Environment.NewLine;

            try
            {
                string nonCCUserID = (context == null || context.Session["PreviousStaffID"] == null ? string.Empty : " [CC User ID:] " + context.Session["PreviousStaffID"]);
                txtOutput += "• User: [ID:] " + (context == null || context.Session["StaffID"] == null ? string.Empty : context.Session["StaffID"]) + " [Name:] " + (context == null || context.Session["StaffFullnameWithoutMiddlename"] == null ? string.Empty : context.Session["StaffFullnameWithoutMiddlename"]) + nonCCUserID + Environment.NewLine;
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

            txtOutput += "• Database   : " + Database + "(" + site + ")" + Environment.NewLine;
            txtOutput += "• SiteType   : " + SiteType + Environment.NewLine;
            //txtOutput += "• ConnStr  : " + ConnStr + Environment.NewLine;
            txtOutput += "• IP         : " + IP + "(" + Country + ")" + " : " + Environment.NewLine;
            txtOutput += "• User Agent : " + (context != null ? context.Request.UserAgent : "") + Environment.NewLine;
            txtOutput += "• " + DateTime.Now.ToString("dd MMM HH:mm:ss.fff") + Environment.NewLine + Environment.NewLine;
            txtOutput += info + Environment.NewLine;
            txtOutput += "------------------------" + Environment.NewLine;


            // create html error output

            htmlOutput += "<hr>" + Environment.NewLine;

            htmlOutput += "<table>" + Environment.NewLine;
            htmlOutput += "<tr><td>DateTime</td><td><b>" + DateTime.Now.ToString("ddd dd MMM   HH:mm:ss.fff")            + "</b></td></tr>" + Environment.NewLine;
            htmlOutput += "<tr><td>Machine</td><td><b>"  + Environment.MachineName                                       + "</b></td></tr>" + Environment.NewLine;
            htmlOutput += "<tr><td>URL</td><td>"         + (context != null ? context.Request.Url.AbsoluteUri : "NO URL")    + "</td></tr>" + Environment.NewLine;

            try
            {
                string nonCCUserID = (context == null || context.Session["PreviousStaffID"] == null ? string.Empty : " [CC User ID: " + context.Session["PreviousStaffID"] + "]");
                htmlOutput += "<tr><td>User</td><td><b>" + (context.Session["StaffFullnameWithoutMiddlename"] == null ? string.Empty : context.Session["StaffFullnameWithoutMiddlename"]) + " [ID: " + (context.Session["StaffID"] == null ? string.Empty : context.Session["StaffID"]) + "]" + nonCCUserID + "</b></td></tr>" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                htmlOutput += "<tr><td>User</td><td><font color=\"red\">" + 
                    "FAILURE TO GET USER ID AND USERNAME FROM SESSION - THREW AN ERROR:<br>" + ex.ToString().Replace(Environment.NewLine, "<br />" + Environment.NewLine)  +
                    "</font></td></tr>" + Environment.NewLine;
            }


            htmlOutput += "<tr><td>Database</td><td><b>" + Database + "</b> (" + site + ")" + "</td></tr>" + Environment.NewLine;
            htmlOutput += "<tr><td style=\"white-space:nowrap;\">Site Type</td><td><b>" + SiteType + "</b></td></tr>" + Environment.NewLine;
            //htmlOutput += "<tr><td>ConnStr</td><td><b>" + ConnStr + "</b></td></tr>" + Environment.NewLine;
            htmlOutput += "<tr><td>IP</td><td><b>" + IP + "(" + Country + ")" + "</b></td></tr>" + Environment.NewLine;
            htmlOutput += "<tr><td style=\"white-space:nowrap;\">User Agent</td><td><b>" + (context != null ? context.Request.UserAgent : "") + "</b></td></tr>" + Environment.NewLine;
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



    protected static string GetIPCountry(string ip, string DB = null)
    {
        Hashtable ipHash = GetIpHash( new string[] { ip }, DB );
        return ipHash[ip] == null ? "" : ipHash[ip].ToString();
    }
    protected static Hashtable GetIpHash(string[] ips, string DB = null)
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
            System.Data.DataSet ds = DBBase.ExecuteQuery(sql, DB);
            for (int i = 0; i < ips.Length; i++)
                ipHash[ips[i]] = ds.Tables[i].Rows[0][0].ToString();
        }
        return ipHash;
    }
}

