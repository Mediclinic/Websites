using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using System.Text.RegularExpressions;


public class Utilities
{

    public static bool IsDev()
    {
        return Convert.ToBoolean(ConfigurationManager.AppSettings["Development"]);
    }

    public static bool IsValidDB(string db)
    {
        if (db == null)
            return false;

        // if url is   ?id=0001&id=0001 then  Request.QueryString["id"]  =  "0001,0001" ...   so split it and keep only the first
        string id = db.Contains(",") ? db.Split(',')[0] : db;

        if (!Regex.IsMatch(id, @"^\d{4}$"))
            return false;

        string dbName = "Mediclinic_" + id;
        string sql = "SELECT Count(name) FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = '" + dbName + "' OR name = '" + dbName + "')";
        bool dbExists = Convert.ToBoolean(DBBase.ExecuteSingleResult(sql, "master"));

        return dbExists;
    }
    public static string GetDB(string db)
    {
        if (db == null)
            return null;

        // if url is   ?id=0001&id=0001 then  Request.QueryString["id"]  =  "0001,0001" ...   so split it and keep only the first
        string id = db.Contains(",") ? db.Split(',')[0] : db;

        if (!Regex.IsMatch(id, @"^\d{4}$"))
            return null;

        string dbName = "Mediclinic_" + id;
        string sql = "SELECT Count(name) FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = '" + dbName + "' OR name = '" + dbName + "')";
        bool dbExists = Convert.ToBoolean(DBBase.ExecuteSingleResult(sql, "master"));

        return dbExists ? dbName : null;
    }

    public static string GetAddressType()
    {
        if (System.Web.HttpContext.Current.Session["AddressType"] == null)
            System.Web.HttpContext.Current.Session["AddressType"] = SystemVariableDB.GetByDescr("AddressType").Value;

        return System.Web.HttpContext.Current.Session["AddressType"].ToString();
    }

    public static string FormatName(string inName)
    {
        if (inName.Length == 0)
            return inName;

        char[] array = inName.ToCharArray();

        // Handle the first letter in the string.
        if (array.Length >= 1)
        {
            if (char.IsLower(array[0]))
                array[0] = char.ToUpper(array[0]);
        }

        // Scan through the letters, checking for spaces.
        // ... Uppercase then lowercase letters following spaces.
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i - 1] == ' ' || array[i - 1] == '-')
            {
                if (char.IsLower(array[i]))
                    array[i] = char.ToUpper(array[i]);
            }
            else
            {
                //if (char.IsUpper(array[i]))
                //    array[i] = char.ToLower(array[i]);
            }
        }

        return new string(array);
    }
    public static string FormatPhoneNumber(string inNbr)
    {
        inNbr = System.Text.RegularExpressions.Regex.Replace(inNbr, "[^0-9]", "");

        if (inNbr.Length >= 10)
            return inNbr.Substring(0, 2) + " " + inNbr.Substring(2, 4) + " " + inNbr.Substring(6);
        if (inNbr.Length >= 9)
            return inNbr.Substring(0, 3) + " " + inNbr.Substring(3, 3) + " " + inNbr.Substring(6);
        else if (inNbr.Length >= 6)
            return inNbr.Substring(0, inNbr.Length-4) + " " + inNbr.Substring(inNbr.Length - 4);
        else
            return inNbr;
    }
    public static string EncodePassword(string pwdIn)
    {
        //Declarations
        Byte[] originalBytes;
        Byte[] encodedBytes;
        MD5 md5;

        //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
        md5 = new MD5CryptoServiceProvider();
        originalBytes = ASCIIEncoding.Default.GetBytes(pwdIn);
        encodedBytes = md5.ComputeHash(originalBytes);

        //Convert encoded bytes back to a 'readable' string
        return BitConverter.ToString(encodedBytes).Substring(0, 30);
    }


    public static void AddConfirmationBox(GridViewRowEventArgs e, bool incConfirmMsg = true, string text = "Delete")
    {
        foreach (DataControlFieldCell cell in e.Row.Cells)
        {
            foreach (Control control in cell.Controls)
            {
                LinkButton button = control as LinkButton;
                if (button != null && (button.CommandName.ToLower() == "delete" || button.CommandName.ToLower() == "_delete"))
                    if (incConfirmMsg)
                        button.OnClientClick = "javascript:if (!confirm('Are you sure you want to " + text.ToLower() + " this record?')) return false;";

                ImageButton delButton = control as ImageButton;
                if (delButton != null && (delButton.CommandName.ToLower() == "delete" || delButton.CommandName.ToLower() == "_delete"))
                {
                    if (incConfirmMsg)
                        delButton.OnClientClick = "javascript:if (!confirm('Are you sure you want to " + text.ToLower() + " this record?')) return false;";

                    delButton.AlternateText = text;
                    delButton.ToolTip = text;
                }

            }
        }
    }
    public static void SetEditRowBackColour(GridViewRowEventArgs e, System.Drawing.Color color, bool changeRowBackColor = true)
    {
        if (changeRowBackColor)
            e.Row.BackColor = color;

        SetControlColors(e.Row, color);
    }
    protected static void SetControlColors(Control parent, System.Drawing.Color color)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is DropDownList || control is TextBox)
            {
                ((WebControl)control).BackColor = color;

                ((WebControl)control).Attributes["onfocus"] += "set_focus_color(this, true);";
                ((WebControl)control).Attributes["onblur"] += control is CheckBox ? "set_focus_color(this, false, 'transparent');" : "set_focus_color(this, false);";
            }

            SetControlColors(control, color);
        }
    }
    public static void SetEditControlBackColour(Control control, bool editable, System.Drawing.Color editColor, System.Drawing.Color nonEditColor, bool incFocusColorChange = true)
    {
        if (control is DropDownList || control is TextBox)
        {
            ((WebControl)control).BackColor = editable ? editColor : nonEditColor;


            // remove so that don't add it more than once
            if (((WebControl)control).Attributes["onfocus"] != null)
                ((WebControl)control).Attributes["onfocus"] = ((WebControl)control).Attributes["onfocus"].Replace("set_focus_color(this, true);", "");
            if (((WebControl)control).Attributes["onblur"] != null)
                ((WebControl)control).Attributes["onblur"] = control is CheckBox ? ((WebControl)control).Attributes["onblur"].Replace("set_focus_color(this, false, 'transparent');", "") : ((WebControl)control).Attributes["onblur"].Replace("set_focus_color(this, false);", "");

            if (incFocusColorChange)
            {
                ((WebControl)control).Attributes["onfocus"] += "set_focus_color(this, true);";
                ((WebControl)control).Attributes["onblur"]  += control is CheckBox ? "set_focus_color(this, false, 'transparent');" : "set_focus_color(this, false);";
            }
        }
    }
    private static String HexConverter(System.Drawing.Color c)
    {
        return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    }


    public static int IndexOf(DropDownList ddl, params string[] texts)
    {
        for(int i=0; i<ddl.Items.Count; i++)
        {
            foreach(string text in texts)
                if (string.Compare(text, ddl.Items[i].Text, true) == 0)
                    return i;
        }

        return -1;
    }
    public static void AddIfNotExists(DropDownList ddl, int val)
    {
        if (ddl.Items.FindByValue(val.ToString()) == null)
        {
            ListItem li = new ListItem(val.ToString(), val.ToString());

            if (ddl.Items.Count == 0)
                ddl.Items.Add(li);
            else
            {
                bool added = false;
                for (int i = 0; i < ddl.Items.Count - 1; i++)
                {
                    if (val < Convert.ToInt32(ddl.Items[i].Value))
                    {
                        ddl.Items.Insert(i, li);
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    ddl.Items.Add(li);
                }
            }
        }
    }

    public static int IndexOfNth(string str, char c, int n)
    {
        int s = -1;
        for (int i = 0; i < n; i++)
        {
            s = str.IndexOf(c, s + 1);
            if (s == -1) break;
        }
        return s;
    }

    public static bool IsValidDBDateTime(DateTime dt)
    {
        return IsValidSqlServerDateTime(dt);
    }
    protected static bool IsValidSqlServerDateTime(DateTime dt)
    {
        return dt.Year >= 1753 && dt.Year <= 9999;
    }
    public static bool IsValidDOB(DateTime dt)
    {
        return (dt < DateTime.Now) && (DateTime.Now.Year - dt.Year <= 125);
    }

    public static string CleanEmailAddresses(string emails)
    {
        ArrayList newList = new ArrayList();
        foreach (string email in emails.Split(','))
            if (email.Trim().Length > 0)
                newList.Add(email.Trim());

        return String.Join(",", (string[])newList.ToArray(typeof(string)));
    }
    public static bool IsValidEmailAddresses(string emails, bool allowEmpty)
    {
        bool isValid = allowEmpty;
        if (emails.Length > 0)
        {
            string[] emailList = emails.Split(',');
            for (int i = 0; i < emailList.Length; i++)
            {
                if (!Utilities.IsValidEmailAddress(emailList[i]))
                    return false;
                else
                    isValid = true;
            }
        }
        return isValid;
    }
    public static bool IsValidEmailAddress(string email)
    {
        string emailREGEX = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}$";
        return System.Text.RegularExpressions.Regex.IsMatch(email, emailREGEX);
    }
    public static bool IsValidWebURL(string url)
    {
        string websiteREGEX = @"^(https?:\/\/)?([\dA-Za-z\.-]+)\.([A-Za-z\.]{2,6})([\/\w \.-]*)*\/?$";
        return System.Text.RegularExpressions.Regex.IsMatch(url, websiteREGEX);
    }
    public static bool IsValidPhoneNumber(string IsValidPhoneNumber)
    {
        string emailREGEX = @"\d{3,}";
        return System.Text.RegularExpressions.Regex.IsMatch(IsValidPhoneNumber, emailREGEX);
    }

    


    public static void CheckSessionAuth(System.Web.SessionState.HttpSessionState session, System.Web.UI.Page page, HttpResponse response, HttpRequest request)
    {
        bool isLoggedIn = session["DB"] != null && session["IsLoggedIn"] != null && Convert.ToBoolean(session["IsLoggedIn"]);
        if (!isLoggedIn)
        {
            Logout(session, response, request);
            return;
        }

        // if another session logged in - logout here
        if (!(new List<int> { -5, -7, -8 }).Contains((int)session["StaffID"])) 
        {
            UserLogin userlogin = (session["PatientID"] == null) ?
                UserLoginDB.GetByUserID(Convert.ToInt32(session["StaffID"]), -1) :
                UserLoginDB.GetByUserID(-1, Convert.ToInt32(session["StaffID"]));

            if (userlogin == null || userlogin.SessionID != HttpContext.Current.Session.SessionID.ToString())
            {
                Logout(session, response, request);
                return;
            }

            UserLoginDB.UpdateLastAccessTime(userlogin.UserloginID, DateTime.Now, request.RawUrl);
        }
    }
    public static void Logout(System.Web.SessionState.HttpSessionState session, HttpResponse response, HttpRequest request, bool includeForwardUrl = true)
    {
        if (session["StaffID"] != null)
            UserLoginDB.UpdateSetAllSessionsLoggedOut(Convert.ToInt32(session["StaffID"]), -1);
        if (session["PatientID"] != null)
            UserLoginDB.UpdateSetAllSessionsLoggedOut(-1, Convert.ToInt32(session["PatientID"]));

        Utilities.UnsetSessionVariables();

        //System.Web.Security.FormsAuthentication.SignOut();
        if (!HttpContext.Current.Request.Url.LocalPath.Contains("/Account/Login.aspx"))
        {
            if (includeForwardUrl)
                response.Redirect("~/Account/Login.aspx" + "?from_url=" + request.RawUrl);
            else
                response.Redirect("~/Account/Login.aspx");
        }
    }
    public static void LogoutV2(System.Web.SessionState.HttpSessionState session, HttpResponse response, HttpRequest request, bool includeForwardUrl = true)
    {
        if (session["StaffID"] != null)
            UserLoginDB.UpdateSetAllSessionsLoggedOut(Convert.ToInt32(session["StaffID"]), -1);
        if (session["PatientID"] != null)
            UserLoginDB.UpdateSetAllSessionsLoggedOut(-1, Convert.ToInt32(session["PatientID"]));

        Utilities.UnsetSessionVariables();

        //System.Web.Security.FormsAuthentication.SignOut();
        if (!HttpContext.Current.Request.Url.LocalPath.Contains("/Account/LoginV2.aspx"))
        {
            if (includeForwardUrl)
                response.Redirect("~/Account/LoginV2.aspx" + "?from_url=" + request.RawUrl);
            else
                response.Redirect("~/Account/LoginV2.aspx");
        }
    }
    public static void UnsetSessionVariables()
    {
        HttpContext.Current.Session.Remove("DB");
        HttpContext.Current.Session.Remove("SystemVariables");

        HttpContext.Current.Session.Remove("IsLoggedIn");
        HttpContext.Current.Session.Remove("IsStakeholder");
        HttpContext.Current.Session.Remove("IsMasterAdmin");
        HttpContext.Current.Session.Remove("IsAdmin");
        HttpContext.Current.Session.Remove("IsPrincipal");
        HttpContext.Current.Session.Remove("IsProvider");
        HttpContext.Current.Session.Remove("IsExternal");
        HttpContext.Current.Session.Remove("StaffID");
        HttpContext.Current.Session.Remove("PatientID");
        HttpContext.Current.Session.Remove("StaffFullnameWithoutMiddlename");
        HttpContext.Current.Session.Remove("StaffFirstname");
        HttpContext.Current.Session.Remove("NumDaysToDisplayOnBookingScreen");

        HttpContext.Current.Session.Remove("ShowOtherProvidersOnBookingScreen");
        HttpContext.Current.Session.Remove("ShowHeaderOnBookingScreen");
        HttpContext.Current.Session.Remove("SystemVariables");
        HttpContext.Current.Session.Remove("OfferingColors");

        HttpContext.Current.Session.Remove("SiteID");
        HttpContext.Current.Session.Remove("SiteName");
        HttpContext.Current.Session.Remove("SiteIsClinic");
        HttpContext.Current.Session.Remove("SiteIsAgedCare");
        HttpContext.Current.Session.Remove("SiteIsGP");
        HttpContext.Current.Session.Remove("SiteTypeID");
        HttpContext.Current.Session.Remove("SiteTypeDescr");
        HttpContext.Current.Session.Remove("OrgID");
        HttpContext.Current.Session.Remove("OrgName");

        HttpContext.Current.Session.Clear();
    }


    public static void SetNoCache(HttpResponse response)
    {
        response.Buffer = true;
        response.CacheControl = "no-cache";
        response.AddHeader("Pragma", "no-cache");
        response.Expires = -1441;
    }

    public static void UpdatePageHeader(MasterPage Master, bool removeHeader, bool removePageMinHeight)
    {
        System.Web.UI.HtmlControls.HtmlGenericControl div;

        if (removeHeader)
        {
            div = Master.FindControl("headerContent") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (div != null)
                div.Visible = false;
        }

        if (removePageMinHeight)
        {
            div = Master.FindControl("main") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (div != null)
                div.Attributes["class"] = "main_no_height";
        }
    }
    public static void UpdatePageHeaderV2(MasterPage Master, bool removeHeader)
    {
        System.Web.UI.HtmlControls.HtmlGenericControl div;

        if (removeHeader)
        {
            div = Master.FindControl("banner") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (div != null)
                div.Visible = false;

            div = Master.FindControl("div_menu") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (div != null)
                div.Visible = false;

            div = Master.FindControl("mainsection") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (div != null)
                div.RemoveCssClass("mainsection");
        }
    }

    public static string TrimName(string name, int maxLength, int dotsAtEndToAdd = 0)
    {
        if (name.Length <= maxLength)
            return name;

        return name.Substring(0, maxLength - dotsAtEndToAdd) + new string('.', dotsAtEndToAdd);
    }

    // Don't use this
    // whenever config is updated, sessions are ended and people have to log back in
    // instead - anything that need to be updated should go in the database
    //
    //public static void UpdateConfigElement(string name, string value)
    //{
    //    Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
    //    config.AppSettings.Settings[name].Value = value;
    //    config.Save(ConfigurationSaveMode.Modified);
    //    ConfigurationManager.RefreshSection("appSettings");
    //}

    public static DateTime GetBuildDate()
    {
        UriBuilder uri = new UriBuilder(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        return System.IO.File.GetLastWriteTime(
            System.IO.Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path))
            );
    }

    public static bool IsValidDate(string strDate, string format)
    {
        if (format == "dd-mm-yyyy" || format == "d-m-yyyy")
        {
            try
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(strDate, @"^\d{1,2}\-\d{1,2}\-\d{4}$"))
                    return false;

                string[] dateParts = strDate.Split('-');
                DateTime date = new DateTime(Convert.ToInt32(dateParts[2]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[0]));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        else if (format == "yyyy-mm-dd" || format == "yyyy-m-d")
        {
            try
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(strDate, @"^\d{4}\-\d{1,2}\-\d{1,2}$"))
                    return false;

                string[] dateParts = strDate.Split('-');
                DateTime date = new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        else
            throw new Exception("Unknown date format: " + format);
                
    }
    public static DateTime GetDate(string strDate, string format)
    {
        strDate = strDate.Trim();

        if (strDate.Length == 0)
        {
            return DateTime.MinValue;
        }
        else
        {
            string[] dateParts = strDate.Split(new char[] { '-' });

            if (format == "dd-mm-yyyy" || format == "d-m-yyyy")
                return new DateTime(Convert.ToInt32(dateParts[2]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[0]));
            else if (format == "yyyy-mm-dd" || format == "yyyy-m-d")
                return new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]));
            else
                throw new Exception("Unknown date format: " + format);
        }
    }

    public static string GetDateOrdinal(int day)
    {
	    string suffix = String.Empty;

        int ones = day % 10;
        int tens = (int)Math.Floor(day / 10M) % 10;

	    if (tens == 1)
	    {
		    suffix = "th";
	    }
	    else
	    {
		    switch (ones)
		    {
			    case 1:
				    suffix = "st";
				    break;

			    case 2:
				    suffix = "nd";
				    break;

			    case 3:
				    suffix = "rd";
				    break;

			    default:
				    suffix = "th";
				    break;
		    }
	    }
        return String.Format("{0}{1}", day, suffix);
    }

    public static int GetAge(DateTime DOB)
    {
        int age = -1;
        if (DOB != DateTime.MinValue)
        {
            DateTime now = DateTime.Today;
            age = now.Year - DOB.Year;
            if (now.Month < DOB.Month || (now.Month == DOB.Month && now.Day < DOB.Day))
                age--;
        }

        return age;
    }

    public static string ConvertHtmlToText(string source)
    {
        string result;

        // Remove HTML Development formatting
        // Replace line breaks with space
        // because browsers inserts space
        result = source.Replace("\r", " ");
        // Replace line breaks with space
        // because browsers inserts space
        result = result.Replace("\n", " ");
        // Remove step-formatting
        result = result.Replace("\t", string.Empty);
        // Remove repeating spaces because browsers ignore them
        result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                @"( )+", " ");

        // Remove the header (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*head([^>])*>", "<head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*head( )*>)", "</head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, "(<head>).*(</head>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // remove all scripts (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*script([^>])*>", "<script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*script( )*>)", "</script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //result = System.Text.RegularExpressions.Regex.Replace(result, @"(<script>)([^(<script>\.</script>)])*(</script>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"(<script>).*(</script>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // remove all styles (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*style([^>])*>", "<style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*style( )*>)", "</style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, "(<style>).*(</style>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert tabs in spaces of <td> tags
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*td([^>])*>", "\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert line breaks in places of <BR> and <LI> tags
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*br( )*/>", "\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*li( )*>", "\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert line paragraphs (double line breaks) in place
        // if <P>, <DIV> and <TR> tags
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*div([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*tr([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*p([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Remove remaining tags like <a>, links, images,
        // comments etc - anything that's enclosed inside < >
        result = System.Text.RegularExpressions.Regex.Replace(result, @"<[^>]*>", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // replace special characters:
        result = System.Text.RegularExpressions.Regex.Replace(result, @" ", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        result = System.Text.RegularExpressions.Regex.Replace(result, @"&bull;", " * ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"&lsaquo;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"&rsaquo;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"&trade;", "(tm)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"&frasl;", "/", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"&lt;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"&gt;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"&copy;", "(c)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, @"&reg;", "(r)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove all others. More can be added, see
        // http://hotwired.lycos.com/webmonkey/reference/special_characters/
        result = System.Text.RegularExpressions.Regex.Replace(result, @"&(.{2,6});", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // for testing
        //System.Text.RegularExpressions.Regex.Replace(result, this.txtRegex.Text,string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // make line breaking consistent
        result = result.Replace("\n", "\r");

        // Remove extra line breaks and tabs:
        // replace over 2 breaks with 2 and over 4 tabs with 4.
        // Prepare first to remove any whitespaces in between
        // the escaped characters and remove redundant tabs in between line breaks
        result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\t)", "\t\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\r)", "\t\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\t)", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove redundant tabs
        result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove multiple tabs following a line break with just one tab
        result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Initial replacement target string for line breaks
        string breaks = "\r\r\r";
        // Initial replacement target string for tabs
        string tabs = "\t\t\t\t\t";
        for (int index = 0; index < result.Length; index++)
        {
            result = result.Replace(breaks, "\r\r");
            result = result.Replace(tabs, "\t\t\t\t");
            breaks = breaks + "\r";
            tabs = tabs + "\t";
        }

        // That's it.
        return result;
    }

    public static string GenerateRandomString(int length, bool incNumeric = true)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" + (incNumeric ? "0123456789" : "");
        char[] stringChars = new char[length];

        var random = new Random();
        for (int i = 0; i < length; i++)
            stringChars[i] = chars[random.Next(chars.Length)];

        return new String(stringChars);
    }

    /// <summary>
    /// string response = HttpGet(url);
    /// 
    /// System.Net.HttpStatusCode httpResponseStatusCode;
    /// string output = Utilities.HttpGet(url, out httpResponseStatusCode);
    ///
    /// if ((int)httpResponseStatusCode != 200)
    /// {
    ///    email and/or display alert
    /// }
    /// else
    /// {
    ///    process output/result
    /// }
    /// </summary>
    public static string HttpGet(string url, out System.Net.HttpStatusCode httpResponseStatusCode, string customRequestUserAgent = null)
    {
        // used to build entire input
        StringBuilder sb = new StringBuilder();

        // used on each read operation
        byte[] buf = new byte[8192];

        // prepare the web page we will be asking for
        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

        if (customRequestUserAgent != null)
            request.UserAgent = customRequestUserAgent;

        System.Net.HttpWebResponse response = null;
        try
        {
            // execute the request
            response = (System.Net.HttpWebResponse)request.GetResponse();
            httpResponseStatusCode = response.StatusCode;

            // we will read data via the response stream
            System.IO.Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            return sb.ToString();
        }
        catch (System.Net.WebException we)
        {
            httpResponseStatusCode = ((System.Net.HttpWebResponse)we.Response).StatusCode;
            return null;
        }
    }

    /// <summary>
    /// string response = HttpPost(url, new NameValueCollection() {
    /// { "name1", "val1" },
    /// { "name2", "val2" }
    /// });
    /// </summary>
    public static string HttpPost(string uri, System.Collections.Specialized.NameValueCollection pairs)
    {
        byte[] response = null;
        using (System.Net.WebClient client = new System.Net.WebClient())
        {
            response = client.UploadValues(uri, pairs);
        }

        return Encoding.ASCII.GetString(response);
    }



    public static bool IsMobileDevice(HttpRequest request, bool incMac = true, bool incChrome = true)
    {
        if (request == null || request.UserAgent == null)
            return false;

        bool isMac    = request.UserAgent.ToUpper().Contains("MAC OS X");
        bool isChrome = request.UserAgent.ToUpper().Contains("CHROME");

        var UserAgent = request.ServerVariables["HTTP_USER_AGENT"];
        return (incMac && isMac) || (incChrome && isChrome) || UserAgent.Contains("iPhone") || UserAgent.Contains("Windows Phone") || UserAgent.Contains("Android");
    }


    public static string GetMimeType(string extension)
    {
        if (extension == null)
        {
            throw new ArgumentNullException("extension");
        }

        if (!extension.StartsWith("."))
        {
            extension = "." + extension;
        }

        string mime;

        return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
    }

    private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        #region Big freaking list of mime types
        // combination of values from Windows 7 Registry and 
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion

    };



}

