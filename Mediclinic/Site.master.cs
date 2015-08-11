using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

public partial class SiteMaster : System.Web.UI.MasterPage
{

    private int startTime; 

    protected override void OnInit(EventArgs e)
    {
        startTime = Environment.TickCount;

        bool hideHeader =
            HttpContext.Current.Request.Url.LocalPath.EndsWith("/Account/Logout.aspx")               ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/ContactTypeInfo.aspx")              ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/StreetAndSuburbInfo.aspx")          ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/SuburbListPopup.aspx")              ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/NoteInfo.aspx")                     ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/BookingCreateInvoice.aspx")         ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/BookingCreateInvoiceAgedCare.aspx") ||
            (HttpContext.Current.Request.Url.LocalPath.Contains("/ViewInvoice.aspx") && !HttpContext.Current.Request.Url.AbsoluteUri.Contains("is_popup=0")) ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/PatientListPopup.aspx")             ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/OrganisationListPopup.aspx")        ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/AddEditEPC.aspx")                   ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/AddEditReceipt.aspx")               ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/AddEditRefund.aspx")                ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/AddEditCreditNote.aspx")            ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/AddEditContact.aspx")               ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/AddEditContactAus.aspx")            ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/BookingSheetBlockout.aspx")         ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/AddCreditEmailPopup.aspx")          ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/ReferrerListPopup.aspx")            ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/ReferrerPersonListPopup.aspx")      ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/ReferrerOrgListPopup.aspx")         ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/PatientReferrerHistoryPopup.aspx")  ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/StaffOfferingsBulkUpdate.aspx")  ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/AddEditOfferingPopupMessage.aspx");

        UpdateLogout(hideHeader);

        if (Session["SystemVariables"] != null && !Page.Title.StartsWith(((SystemVariables)Session["SystemVariables"])["Site"].Value + " - "))
            Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + Page.Title;

        base.OnInit(e);
    }
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["ShowPageExecutionTime"]))
        {
            double executionTime = (double)(Environment.TickCount - startTime) / 1000.0;
            lblPageLoadTime.Text = "Page Load Time: " + executionTime + " seconds";
        }
    }



    protected void Page_Load(object sender, EventArgs e)
    {
        // staffinfo page has to show alot of data
        //PageDiv.Attributes["class"] = HttpContext.Current.Request.Url.LocalPath.EndsWith("/StaffInfo.aspx") ? "pagewide" : "page";

        SetupGUI();
        div_menu.Style["display"] = Request.Browser.Type.Contains("Firefox") ? "none" : "block";
    }


    protected void SetupGUI()
    {
        UserView userView = UserView.GetInstance();

        lblSiteIsClinic.Value   = userView.IsClinicView || userView.IsGPView ? "1" : "0";
        lblSiteIsAgedCare.Value = userView.IsAgedCareView                    ? "1" : "0";


        if (Session["SystemVariables"] != null)
            lblBannerMessage.Text    = ((SystemVariables)Session["SystemVariables"])["BannerMessage"].Value;
        lblBannerMessage.Visible = Session["SystemVariables"] != null && Convert.ToBoolean(((SystemVariables)Session["SystemVariables"])["ShowBannerMessage"].Value);

        lblSupportContactMessage.Text = "<b>" + System.Configuration.ConfigurationManager.AppSettings["SupportContactMessage"] + "</b>";

        // set the style sheet
        System.Web.UI.HtmlControls.HtmlLink newStyleSheet = new System.Web.UI.HtmlControls.HtmlLink();
        newStyleSheet.Attributes.Add("type", "text/css");
        newStyleSheet.Attributes.Add("rel", "stylesheet");
        newStyleSheet.Href = Session["SystemVariables"] == null ? System.Configuration.ConfigurationManager.AppSettings["CssPage"] : ((SystemVariables)Session["SystemVariables"])["CssPage"].Value;
        Page.Header.Controls.Add(newStyleSheet);

        SiteName.Text = Session["SystemVariables"] == null ? "Mediclinic" : ((SystemVariables)Session["SystemVariables"])["Site"].Value;


        spn_login_display_not_loged_in.Visible = !userView.IsLoggedIn;
        spn_login_display_loged_in.Visible     =  userView.IsLoggedIn;

        if (userView.IsLoggedIn)
        {
            lblUsername.Text = Session["StaffFullnameWithoutMiddlename"].ToString();

            if (userView.IsAdminView)
                lblSiteOrOrg.Text = Session["SiteName"] == null ? "[No Site Selected]" : (string)Session["SiteName"];
            else
                lblSiteOrOrg.Text = Session["OrgName"] == null ? "[No Clinic Selected]" : (string)Session["OrgName"];

            lnkSelectSiteOrOrg.Text        = userView.IsAdminView ? "Change Site"               : "Change Clinic";
            lnkSelectSiteOrOrg.NavigateUrl = userView.IsAdminView ? "~/Account/SelectSite.aspx" : "~/Account/SelectOrg.aspx";


            if (Session["SiteID"] == null || (!userView.IsAdminView && Session["OrgID"] == null))
            {
                lnkChangePwd.Visible              = false;
                lnkChangePwd_OpenBracket.Visible  = false;
                lnkChangePwd_CloseBracket.Visible = false;
            }


            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["ShowLastBuildTime"]))
            {
                lblLastBuildTime.Text = Utilities.IsDev() ?
                    " Last Build: " + Utilities.GetBuildDate().ToString("dd MMM HH:mm:ss") + " " :
                    " Last Build: " + Utilities.GetBuildDate().ToString("dd MMM HH:mm") + " ";
            }
        }



        for (int i = NavigationMenu.Items.Count - 1; i >= 0; i--)
        {

            if (userView.IsAgedCareView && NavigationMenu.Items[i].Text == "Patients")
            {
                NavigationMenu.Items[i].Text = "Residents";
                for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                    NavigationMenu.Items[i].ChildItems[j].Text = NavigationMenu.Items[i].ChildItems[j].Text.Replace("Patient", "Resident");
            }

            if (NavigationMenu.Items[i].Text == "Bookings")
            {
                if (userView.IsAdminView) // admin has no specific Session["OrgID"] set so show page for them to select multiple orgs to show
                {
                    NavigationMenu.Items[i].NavigateUrl = "~/SelectOrganisations.aspx";
                    for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                    {
                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Bookings Report")
                            ;
                        else if (NavigationMenu.Items[i].ChildItems[j].Text == "Schedule Report")
                            ;
                        else if (NavigationMenu.Items[i].ChildItems[j].Text == "Bookings")
                            NavigationMenu.Items[i].ChildItems[j].NavigateUrl = "~/SelectOrganisations.aspx";
                        else if (NavigationMenu.Items[i].ChildItems[j].Text == "Booking Edit Reason List" || NavigationMenu.Items[i].ChildItems[j].Text == "Booking Unavailability Reason List")
                            ;
                        else
                            NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                    }
                }
                else
                {
                    NavigationMenu.Items[i].NavigateUrl = "~/BookingsForClinic.aspx?orgs=" + Session["OrgID"] + "&ndays=1";
                    for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                    {
                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Bookings")
                            NavigationMenu.Items[i].ChildItems[j].NavigateUrl = "~/BookingsForClinic.aspx?orgs=" + Session["OrgID"] + "&ndays=1";

                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Bookings List")
                            NavigationMenu.Items[i].ChildItems[j].NavigateUrl = "~/BookingsList.aspx?staff=" + Session["StaffID"] + "&start_date=" + DateTime.Today.ToString("yyyy_MM_dd") + "&end_date=" + DateTime.Today.ToString("yyyy_MM_dd");

                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Booking Edit Reason List" || NavigationMenu.Items[i].ChildItems[j].Text == "Booking Unavailability Reason List")
                            NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                    }

                }
            }

            if (NavigationMenu.Items[i].Text == "Organisations")
            {
                if (!userView.IsAdminView)
                {
                    NavigationMenu.Items.RemoveAt(i);
                    continue;
                }

                for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                {
                    if (userView.IsAgedCareView &&
                        (NavigationMenu.Items[i].ChildItems[j].Text == "Clinics List" ||
                         NavigationMenu.Items[i].ChildItems[j].Text == "Add Clinic"))
                    {
                        NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                        continue;
                    }

                    if (!userView.IsAgedCareView &&
                        (NavigationMenu.Items[i].ChildItems[j].Text == "Facilities List" ||
                         NavigationMenu.Items[i].ChildItems[j].Text == "Add Facility"))
                    {
                        NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                        continue;
                    }

                    if (NavigationMenu.Items[i].ChildItems[j].Text == "External Organisations - All" ||
                        NavigationMenu.Items[i].ChildItems[j].Text == "External Organisations - Medical Practices")
                    {
                        NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                        continue;
                    }
                }

                if (!userView.IsAdminView)
                {
                    for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                    {
                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Stats")
                        {
                            NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                            continue;
                        }
                    }
                }

                if (userView.IsAgedCareView)
                    NavigationMenu.Items[i].Text = "Facilities";
                else
                    NavigationMenu.Items[i].Text = "Clinics";
            }
            else if (NavigationMenu.Items[i].Text == "Sales")
            {
                for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                {
                    if (NavigationMenu.Items[i].ChildItems[j].Text == "Cash Sale")
                    {
                        if (userView.IsAdminView)
                            NavigationMenu.Items[i].ChildItems[j].NavigateUrl = "~/AddStandardInvoice.aspx";
                        else
                            NavigationMenu.Items[i].ChildItems[j].NavigateUrl = "~/AddStandardInvoice.aspx?org=" + (Session["OrgID"] == null ? "" : Session["OrgID"].ToString());
                    }
                }
            }
            else if (NavigationMenu.Items[i].Text == "Create New Website")
            {
                if (!userView.IsStakeholder)
                {
                    NavigationMenu.Items.RemoveAt(i);
                    continue;
                }
            }
            else if (NavigationMenu.Items[i].Text == "Site & Settings")
            {
                for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                {
                    if (!userView.IsStakeholder && !userView.IsMasterAdmin && NavigationMenu.Items[i].ChildItems[j].Text == "Website Settings")
                    {
                        NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                        continue;
                    }
                }
            }
            else if (NavigationMenu.Items[i].Text == "Letters")
            {
                if (userView.IsAgedCareView)
                    for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Recall Letters")
                            NavigationMenu.Items[i].ChildItems.RemoveAt(j);
            }




            if (!userView.IsAdminView)
            {
                if (NavigationMenu.Items[i].Text == "Staff"               ||
                    NavigationMenu.Items[i].Text == "Organisations"       ||
                    NavigationMenu.Items[i].Text == "Products & Services" ||
                    NavigationMenu.Items[i].Text == "Financials"          ||
                    NavigationMenu.Items[i].Text == "SMS & Email"         ||
                    NavigationMenu.Items[i].Text == "Site & Settings")
                {
                    NavigationMenu.Items.RemoveAt(i);
                    continue;
                }
                else if (NavigationMenu.Items[i].Text == "Organisations")
                {
                    for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                    {
                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Stats")
                            NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                    }
                }
                else if (NavigationMenu.Items[i].Text == "Letters")
                {
                    for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                    {
                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Print Letter")
                            NavigationMenu.Items[i].ChildItems[j].NavigateUrl += (Session != null && Session["OrgID"] != null ? "?org=" + Session["OrgID"].ToString() : "");
                        else if (NavigationMenu.Items[i].ChildItems[j].Text == "Letters Sent History")
                            NavigationMenu.Items[i].ChildItems[j].NavigateUrl += (Session != null && Session["OrgID"] != null ? "?org=" + Session["OrgID"].ToString() : "");
                        else
                            NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                    }
                }
            }
            else  // is admin view
            {
                if ((!userView.IsStakeholder && !userView.IsMasterAdmin) && NavigationMenu.Items[i].Text == "Staff")
                {
                    NavigationMenu.Items.RemoveAt(i);
                }

                if (NavigationMenu.Items[i].Text == "Sales")
                {
                    NavigationMenu.Items.RemoveAt(i);
                }

                if (NavigationMenu.Items[i].Text == "Products & Services")
                {
                    for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                    {
                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Set Specific Prices Per Clinic" && userView.IsAgedCareView)
                            NavigationMenu.Items[i].ChildItems[j].Text = "Set Specific Prices Per Facility/Wing/Unit";

                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Products & Services List")
                            NavigationMenu.Items[i].ChildItems[j].NavigateUrl = "~/OfferingInfo.aspx";  // ?inv_type=" + (isClinics ? "1,2,3" : "4");

                        if (NavigationMenu.Items[i].ChildItems[j].Text == "Cash Sale")
                        {
                            if (userView.IsAdminView)
                                NavigationMenu.Items[i].ChildItems[j].NavigateUrl = "~/AddStandardInvoice.aspx";
                            else
                                NavigationMenu.Items[i].ChildItems[j].NavigateUrl = "~/AddStandardInvoice.aspx?org=" + (Session["OrgID"] == null ? "" : Session["OrgID"].ToString());
                        }

                    }
                }

                if (NavigationMenu.Items[i].Text == "Financials")
                {
                    for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                    {
                        if (!userView.IsStakeholder && NavigationMenu.Items[i].ChildItems[j].Text == "Claim Numbers Allocation")
                        {
                            NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                            continue;
                        }
                        if (!userView.IsStakeholder && NavigationMenu.Items[i].ChildItems[j].Text == "HINX Files")
                        {
                            NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                            continue;
                        }
                        if (!userView.IsAdminView && (NavigationMenu.Items[i].ChildItems[j].Text == "Invoices Report" || NavigationMenu.Items[i].ChildItems[j].Text == "Receipts Report"))
                        {
                            NavigationMenu.Items[i].ChildItems.RemoveAt(j);
                            continue;
                        }
                    }
                }

            }
        }


        if (HttpContext.Current.Request.Url.LocalPath.Contains("/Account/Login.aspx")      ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Account/SelectOrg.aspx")  ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Account/SelectSite.aspx") ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Account/LostPassword.aspx"))
            RemoveMenu();
    }
    protected void RemoveMenu()
    {
        for (int i = NavigationMenu.Items.Count - 1; i >= 0; i--)
        {
            for (int j = NavigationMenu.Items[i].ChildItems.Count - 1; j >= 0; j--)
                NavigationMenu.Items[i].ChildItems.RemoveAt(j);
            NavigationMenu.Items.RemoveAt(i);
        }
    }


    protected void UpdateLogout(bool hideHeader)
    {
        bool isLoggedIn    = Session["IsLoggedIn"]     != null && Convert.ToBoolean(Session["IsLoggedIn"]);
        bool isStakeholder = Session["IsStakeholder"]  != null && Convert.ToBoolean(Session["IsStakeholder"]);
        bool isMasterAdmin = Session["IsMasterAdmin"]  != null && Convert.ToBoolean(Session["IsMasterAdmin"]);
        bool isAdmin       = Session["IsAdmin"]        != null && Convert.ToBoolean(Session["IsAdmin"]);
        bool isPrincipal   = Session["IsPrincipal"]    != null && Convert.ToBoolean(Session["IsPrincipal"]);

        bool isAdminView   = isStakeholder || isMasterAdmin || isAdmin;


        if (!isLoggedIn)
        {
            Logout(hideHeader);
            return;
        }

        // if another session logged in - logout here
        if (!(new List<int> { -5, -7, -8 }).Contains((int)Session["StaffID"]))
        {
            UserLogin userlogin = (Session["PatientID"] == null) ?
                UserLoginDB.GetByUserID(Convert.ToInt32(Session["StaffID"]), -1) :
                UserLoginDB.GetByUserID(-1, Convert.ToInt32(Session["StaffID"]));

            if (userlogin == null || userlogin.SessionID != HttpContext.Current.Session.SessionID.ToString())
            {
                Logout(hideHeader);
                return;
            }

            if (Session["SiteID"] == null &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/LoginV2.aspx") &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/LogoutV2.aspx") &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/SelectSiteV2.aspx"))
                Response.Redirect("~/Account/SelectSiteV2.aspx?from_url=" + Request.RawUrl);


            if (!isAdminView && Session["OrgID"] == null &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/Login.aspx") &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/Logout.aspx") &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/SelectOrg.aspx") &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/SelectSite.aspx"))
                Response.Redirect("~/Account/SelectOrgV2.aspx?from_url=" + Request.RawUrl);

            if (!(new List<int> { -5, -7, -8 }).Contains((int)Session["StaffID"]))
                UserLoginDB.UpdateLastAccessTime(userlogin.UserloginID, DateTime.Now, Request.RawUrl.Contains("/Account/Logout.aspx") ? "" : Request.RawUrl);
        }
    }

    protected void Logout(bool hideHeader)
    {
        Session.Remove("IsLoggedIn");
        Session.Remove("IsStakeholder");
        Session.Remove("IsPrincipal");
        Session.Remove("IsAdmin");
        Session.Remove("StaffID");
        Session.Remove("StaffFullnameWithoutMiddlename");
        Session.Remove("StaffFirstname");
        Session.Remove("NumDaysToDisplayOnBookingScreen");
        Session.Clear();
        //System.Web.Security.FormsAuthentication.SignOut();
        if (!HttpContext.Current.Request.Url.LocalPath.Contains("/Account/Login.aspx") &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/Account/Logout.aspx") &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/Account/LostPassword.aspx"))
            Response.Redirect("~/Account/Login.aspx"+ (Request.RawUrl == "" || Request.RawUrl == "/" || Request.RawUrl.StartsWith("/Default.aspx") ? "" : "?show_header=" + (hideHeader ? "0" : "1") + "&from_url=" + Request.RawUrl));

    }
}
