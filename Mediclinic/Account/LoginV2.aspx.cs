using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;

public partial class LoginV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["DB"] == null && Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
        {
            Session["DB"] = ConfigurationManager.AppSettings["Database"];
            Session["SystemVariables"] = SystemVariableDB.GetAll();
            Response.Redirect(Request.RawUrl, false);
            return;
        }

        // PC & Not FF => Message Suggesting To Use FF
        recommendMozilla.Visible = !Utilities.IsMobileDevice(Request, false, false) && !Request.Browser.Type.ToLower().Contains("firefox");

        bool showPageHeader = Request.QueryString["show_header"] == null || Request.QueryString["show_header"] == "1";
        if (!showPageHeader)
        {
            Utilities.UpdatePageHeaderV2(Page.Master, true);
            beforeDevPanelSpace.Visible = false;
            beforeButtonSpace.Visible   = false;
            afterButtonSpace.Visible    = false;
            recommendMozilla.Visible    = false;
        }

        if (!Utilities.IsDev() && !IsPostBack)
            this.DevPanel.Visible = false;

        Page.Form.DefaultFocus = UserName.ClientID;

    }


    protected void LoginButton_Click(object sender, EventArgs e)
    {
        LogIn(UserName.Text, Password.Text);
    }

    protected void btnDevLogin_Click(object sender, EventArgs e)
    {
        if (!Utilities.IsDev())
            return;

        string loginFor = ((Button)sender).CommandArgument;
        if (loginFor == "Mark")
            LogIn("kia", "770");
        if (loginFor == "Marcus")
            LogIn("marcus", "deaddog2011");
        else if (loginFor == "Steph")
            LogIn("Steph", "Step12525844565");
        else if (loginFor == "Support1")
            LogIn("sp", "99");
        else
        {
            return;
        }
    }


    private void LogIn(string login, string pwd)
    {
        try
        {

            Session.Remove("DB");
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
            {
                Session["DB"] = ConfigurationManager.AppSettings["Database"];
            }
            else // Get DB from Mediclinic_Main
            {
                UserDatabaseMapper user = UserDatabaseMapperDB.GetByLogin(login);
                if (user == null)
                {
                    this.FailureText.Text = "Login Failed.";
                    return;
                }

                Session["DB"] = user.DBName;
            }



            Staff   staff   = StaffDB.GetByLogin(login);
            Patient patient = PatientDB.GetByLogin(login);
            bool allowPatientLogins = Convert.ToInt32(SystemVariableDB.GetByDescr("AllowPatientLogins").Value) == 1;
            bool validStaff         = staff   != null && staff.Pwd   == pwd && !staff.IsFired;
            bool validPatient       = allowPatientLogins && patient != null && patient.Pwd == pwd && !patient.IsDeleted;

            if (validStaff)
            {
                UserLogin curLogin = UserLoginDB.GetCurLoggedIn(staff.StaffID, -1, HttpContext.Current.Session.SessionID, -1);
                if (curLogin != null)
                {
                    UserLoginDB.UpdateLastAccessTime(curLogin.UserloginID, DateTime.Now, Request.RawUrl);
                    UserLoginDB.UpdateSetOtherSessionsOfThisUserLoggedOut(curLogin.UserloginID, staff.StaffID, -1);
                }
                else
                {
                    UserLoginDB.UpdateSetAllSessionsLoggedOut(staff.StaffID, -1);
                    UserLoginDB.Insert((staff == null) ? -1 : staff.StaffID, -1, login, -1, validStaff, HttpContext.Current.Session.SessionID, Request.UserHostAddress);
                }


                this.FailureText.Text = "";

                Session["IsLoggedIn"]                        = true;
                Session["IsStakeholder"]                     = staff.IsStakeholder;
                Session["IsMasterAdmin"]                     = staff.IsMasterAdmin;
                Session["IsAdmin"]                           = staff.IsAdmin;
                Session["IsPrincipal"]                       = staff.IsPrincipal;
                Session["IsProvider"]                        = staff.IsProvider;
                Session["IsExternal"]                        = staff.IsExternal;
                Session["StaffID"]                           = staff.StaffID;
                Session["StaffFullnameWithoutMiddlename"]    = staff.Person.FullnameWithoutMiddlename;
                Session["StaffFirstname"]                    = staff.Person.Firstname;
                Session["NumDaysToDisplayOnBookingScreen"]   = staff.NumDaysToDisplayOnBookingScreen;
                Session["ShowOtherProvidersOnBookingScreen"] = false;
                Session["ShowHeaderOnBookingScreen"]         = staff.ShowHeaderOnBookingScreen;
                Session["SystemVariables"]                   = SystemVariableDB.GetAll();
                Session["OfferingColors"]                    = OfferingDB.GetColorCodes();
                System.Web.Security.FormsAuthentication.SetAuthCookie("--", true);  // needed to use forms authentication


                UserView userView = UserView.GetInstance();

                Site[] allowedSites = StaffSiteRestrictionDB.GetSitesNotRestricted(staff.StaffID, -1, false);


                //
                // until aged care is running, remove aged care from display
                //
                /*
                System.Collections.ArrayList list = new System.Collections.ArrayList();
                for (int i = 0; i < allowedSites.Length; i++)
                    if (allowedSites[i].SiteType.ID == 1 || Utilities.IsDev())
                        list.Add(allowedSites[i]);
                allowedSites = (Site[])list.ToArray(typeof(Site));
                */

                Site[] allSites = SiteDB.GetAll();
                if (allowedSites.Length == 0 && allSites.Length == 1)
                {
                    Session["SiteID"]          = allSites[0].SiteID;
                    Session["SiteName"]        = allSites[0].Name;
                    Session["IsMultipleSites"] = false;
                    Session["SiteIsClinic"]    = allSites[0].SiteType.ID == 1;
                    Session["SiteIsAgedCare"]  = allSites[0].SiteType.ID == 2;
                    Session["SiteIsGP"]        = allSites[0].SiteType.ID == 3;
                    Session["SiteTypeID"]      = allSites[0].SiteType.ID;
                    Session["SiteTypeDescr"]   = allSites[0].SiteType.Descr;

                    UserLoginDB.UpdateSite(staff.StaffID, -1, allSites[0].SiteID);

                    if (!userView.IsAdminView)  // need to choose org
                    {
                        if (Session["OrgID"] == null)  // providers need to select an org, need to choose one
                        {
                            Response.Redirect("~/Account/SelectOrgV2.aspx" + GetUrlCarryOverParams(), false);
                            return;
                        }
                    }
                }



                if (allowedSites.Length == 1)
                {
                    Session["SiteID"]          = allowedSites[0].SiteID;
                    Session["SiteName"]        = allowedSites[0].Name;
                    Session["IsMultipleSites"] = false;
                    Session["SiteIsClinic"]    = allowedSites[0].SiteType.ID == 1;
                    Session["SiteIsAgedCare"]  = allowedSites[0].SiteType.ID == 2;
                    Session["SiteIsGP"]        = allowedSites[0].SiteType.ID == 3;
                    Session["SiteTypeID"]      = allowedSites[0].SiteType.ID;
                    Session["SiteTypeDescr"]   = allowedSites[0].SiteType.Descr;

                    UserLoginDB.UpdateSite(staff.StaffID, -1, allowedSites[0].SiteID);

                    if (!userView.IsAdminView)  // need to choose org
                    {
                        if (Session["OrgID"] == null)  // providers need to select an org, need to choose one
                        {
                            Response.Redirect("~/Account/SelectOrgV2.aspx" + GetUrlCarryOverParams(), false);
                            return;
                        }
                    }
                }
                else // if more than one site, go to choose. if no sites this page will say to contact admin
                {
                    if (Session["SiteID"] == null)  // admins if yet to login to a site, need to choose one
                    {
                        Session["IsMultipleSites"] = true;
                        Response.Redirect("~/Account/SelectSiteV2.aspx" + GetUrlCarryOverParams(), false);
                        return;
                    }
                }



                /*

                if (!staff.IsAdmin)
                {
                    // provs only login to clinic site
                    Site site = SiteDB.GetByID(2);
                    Session["SiteID"]   = site.SiteID;
                    Session["SiteName"] = site.Name;

                    if (Session["OrgID"] == null)  // providers et to login to select an org, need to choose one
                    {
                        if (Request.QueryString["from_url"] != null)
                        {
                            Response.Redirect("~/Account/SelectOrgV2.aspx?" + Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url=")), false);
                            return;
                        }
                        else
                        {
                            Response.Redirect("~/Account/SelectOrgV2.aspx", false);
                            return;
                        }
                    }
                }
                else
                {
                    if (Session["SiteID"] == null)  // admins if yet to login to a site, need to choose one
                    {
                        if (Request.QueryString["from_url"] != null)
                        {
                            Response.Redirect("~/Account/SelectSiteV2.aspx?" + Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url=")), false);
                            return;
                        }
                        else
                        {
                            Response.Redirect("~/Account/SelectSiteV2.aspx", false);
                            return;
                        }
                    }
                }

                */

                if (Request.QueryString["from_url"] != null)
                {
                    Response.Redirect(Server.UrlDecode(Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url=") + 9)), false);
                    return;
                }
                else
                {
                    Response.Redirect(Convert.ToInt32(Session["StaffID"]) >= 0 ? "~/Default.aspx" : "~/StaffLoginsV2.aspx", false);
                    return;
                }
            }
            else if (validPatient)
            {
                UserLogin curLogin = UserLoginDB.GetCurLoggedIn(-1, patient.PatientID, HttpContext.Current.Session.SessionID, -1);
                if (curLogin != null)
                {
                    UserLoginDB.UpdateLastAccessTime(curLogin.UserloginID, DateTime.Now, Request.RawUrl);
                    UserLoginDB.UpdateSetOtherSessionsOfThisUserLoggedOut(curLogin.UserloginID, -1, patient.PatientID);
                }
                else
                {
                    UserLoginDB.UpdateSetAllSessionsLoggedOut(-1, patient.PatientID);
                    UserLoginDB.Insert(-1, (patient == null) ? -1 : patient.PatientID, login, -1, validPatient, HttpContext.Current.Session.SessionID, Request.UserHostAddress);
                }


                this.FailureText.Text = "";

                Session["IsLoggedIn"]    = true;
                Session["IsStakeholder"] = false;
                Session["IsMasterAdmin"] = false;
                Session["IsAdmin"]       = false;
                Session["IsPrincipal"]   = false;
                Session["IsProvider"]    = false;
                Session["IsExternal"]    = false;
                Session["PatientID"]     = patient.PatientID;
                Session["StaffFullnameWithoutMiddlename"]    = patient.Person.FullnameWithoutMiddlename;
                Session["StaffFirstname"]                    = patient.Person.Firstname;
                Session["NumDaysToDisplayOnBookingScreen"]   = 3;
                Session["ShowOtherProvidersOnBookingScreen"] = false;
                Session["ShowHeaderOnBookingScreen"]         = true;
                Session["SystemVariables"] = SystemVariableDB.GetAll();
                Session["OfferingColors"]  = OfferingDB.GetColorCodes();
                System.Web.Security.FormsAuthentication.SetAuthCookie("--", true);  // needed to use forms authentication


                Site[] allSites     = SiteDB.GetAll();
                Site[] allowedSites = SiteDB.GetAll();


                //
                // remove aged care from display
                //
                System.Collections.ArrayList list = new System.Collections.ArrayList();
                for (int i = 0; i < allSites.Length; i++)
                    if (allSites[i].SiteType.ID == 1)
                        list.Add(allSites[i]);
                allowedSites = (Site[])list.ToArray(typeof(Site));

                if (allowedSites.Length == 0 && allSites.Length == 1)
                {
                    Session["SiteID"]         = allSites[0].SiteID;
                    Session["SiteName"]       = allSites[0].Name;
                    Session["SiteIsClinic"]   = allSites[0].SiteType.ID == 1;
                    Session["SiteIsAgedCare"] = allSites[0].SiteType.ID == 2;
                    Session["SiteIsGP"]       = allSites[0].SiteType.ID == 3;
                    Session["SiteTypeID"]     = allSites[0].SiteType.ID;
                    Session["SiteTypeDescr"]  = allSites[0].SiteType.Descr;


                    UserLoginDB.UpdateSite(-1, patient.PatientID, allSites[0].SiteID);

                    if (Session["OrgID"] == null)  // providers, ext staff, patient logins need to select an org, need to choose one
                    {

                        if (Request.QueryString["from_url"] != null)
                        {
                            string from_url = Server.UrlDecode(Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url=") + 9));
                            if (from_url.Contains("BookingsV2.aspx?") && from_url.Contains("orgs="))
                            {
                                Uri theRealURL = new Uri(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + from_url);
                                string orgs = HttpUtility.ParseQueryString(theRealURL.Query).Get("orgs");
                                if (Regex.IsMatch(orgs, @"^\d+$"))
                                {
                                    Organisation org   = OrganisationDB.GetByID(Convert.ToInt32(orgs));
                                    if (org != null)
                                    {
                                        Session["OrgID"] = org.OrganisationID.ToString();
                                        Session["OrgName"] = org.Name;
                                        Response.Redirect(from_url, false);
                                        return;
                                    }
                                }
                            }
                        }


                        Response.Redirect("~/Account/SelectOrgV2.aspx" + GetUrlCarryOverParams(), false);
                        return;
                    }
                }

                if (allowedSites.Length == 1)
                {
                    Session["SiteID"]         = allowedSites[0].SiteID;
                    Session["SiteName"]       = allowedSites[0].Name;
                    Session["SiteIsClinic"]   = allowedSites[0].SiteType.ID == 1;
                    Session["SiteIsAgedCare"] = allowedSites[0].SiteType.ID == 2;
                    Session["SiteIsGP"]       = allowedSites[0].SiteType.ID == 3;
                    Session["SiteTypeID"]     = allowedSites[0].SiteType.ID;
                    Session["SiteTypeDescr"]  = allowedSites[0].SiteType.Descr;

                    UserLoginDB.UpdateSite(-1, patient.PatientID, allowedSites[0].SiteID);

                    if (Session["OrgID"] == null)  // providers need to select an org, need to choose one
                    {
                        if (Request.QueryString["from_url"] != null)
                        {
                            string from_url = Server.UrlDecode(Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url=") + 9));
                            if (from_url.Contains("BookingsV2.aspx?") && from_url.Contains("orgs="))
                            {
                                Uri theRealURL = new Uri(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + from_url);
                                string orgs = HttpUtility.ParseQueryString(theRealURL.Query).Get("orgs");
                                if (Regex.IsMatch(orgs, @"^\d+$"))
                                {
                                    Organisation org = OrganisationDB.GetByID(Convert.ToInt32(orgs));
                                    if (org != null)
                                    {
                                        Session["OrgID"] = org.OrganisationID.ToString();
                                        Session["OrgName"] = org.Name;
                                        Response.Redirect(from_url, false);
                                        return;
                                    }
                                }
                            }
                        }

                        Response.Redirect("~/Account/SelectOrgV2.aspx" + GetUrlCarryOverParams(), false);
                        return;
                    }
                }
                else // if more than one site, go to choose. if no sites this page will say to contact admin
                {
                    if (Session["SiteID"] == null)  // admins if yet to login to a site, need to choose one
                    {
                        Response.Redirect("~/Account/SelectSiteV2.aspx" + GetUrlCarryOverParams(), false);
                        return;
                    }
                }


                if (Request.QueryString["from_url"] != null)
                {
                    Response.Redirect(Server.UrlDecode(Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url=") + 9)), false);
                    return;
                }
                else
                {
                    Response.Redirect(Convert.ToInt32(Session["StaffID"]) >= 0 ? "~/Default.aspx" : "~/StaffLoginsV2.aspx", false);
                    return;
                }
            }

            else
            {
                //UserLoginDB.Insert((staff == null) ? -1 : staff.StaffID, login, -1, validStaff, HttpContext.Current.Session.SessionID, Request.UserHostAddress);
                this.FailureText.Text = "Login Failed.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            if (Utilities.IsDev())
                FailureText.Text = ex.ToString();
            else
                throw;
        }
    }

    private string GetUrlCarryOverParams()
    {
        string show_header = Request.QueryString["show_header"] == null ? "" : "show_header=" + Request.QueryString["show_header"];
        string from_url = Request.QueryString["from_url"] == null ? "" : Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url="));

        string urlParams = string.Empty;
        if (show_header.Length > 0 && from_url.Length > 0)
            urlParams = "?" + show_header + "&" + from_url;
        else if (show_header.Length > 0)
            urlParams = "?" + show_header;
        else if (from_url.Length > 0)
            urlParams = "?" + from_url;

        return urlParams;
    }

    protected void lnkLostPassword_Click(object sender, EventArgs e)
    {
        // need this since if no auth set, always goes to config file field 'loginUrl'
        // which means LostPassword page (or any other page) will redirect back to what is set as the 'loginUrl'
        System.Web.Security.FormsAuthentication.SetAuthCookie("--", true); 

        Response.Redirect("~/Account/LostPasswordV2.aspx", false);
    }

    protected void lnkPatientCreateLogin_Click(object sender, EventArgs e)
    {
        // need this since if no auth set, always goes to config file field 'loginUrl'
        // which means LostPassword page (or any other page) will redirect back to what is set as the 'loginUrl'
        System.Web.Security.FormsAuthentication.SetAuthCookie("--", true);

        Response.Redirect("~/Account/CreateNewLoginV2.aspx", false);
    }
}