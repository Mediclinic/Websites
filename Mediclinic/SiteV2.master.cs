using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

public partial class SiteMasterV2 : System.Web.UI.MasterPage
{

    private int startTime;

    protected override void OnInit(EventArgs e)
    {

        if (HttpContext.Current.Request.RawUrl.Contains("/Account/LoginV2.aspx") && Request.RawUrl.IndexOf("ReturnUrl=") != -1)
        {
            string ReturnUrl = Server.UrlDecode(Request.RawUrl.Substring(Request.RawUrl.IndexOf("ReturnUrl=") + 10));
            if (ReturnUrl.StartsWith("/Account/CreateNewPatientV2.aspx") ||
                ReturnUrl.StartsWith("/InvoicePaymentV2.aspx")           ||
                ReturnUrl.StartsWith("/PatientUnsubscribeV2.aspx")       ||
                ReturnUrl.StartsWith("/Invoice_WebPayV2.aspx")           ||
                ReturnUrl.StartsWith("/BookingNextAvailableV2.aspx")     ||
                ReturnUrl.StartsWith("/BookingNextAvailableV3.aspx")     ||
                ReturnUrl.StartsWith("/CreateNewCustomerSiteV2.aspx")    ||
                ReturnUrl.EndsWith("/TermsAndConditionsV2.aspx"))
            {

                // need this since if no auth set, always goes to config file field 'loginUrl'
                // which means LostPassword page (or any other page) will redirect back to what is set as the 'loginUrl'
                System.Web.Security.FormsAuthentication.SetAuthCookie("--", true);

                Response.Redirect(ReturnUrl);
            }
        }

        startTime = Environment.TickCount;

        bool hideHeader =
            //HttpContext.Current.Request.Url.LocalPath.Contains("/StreetAndSuburbInfo.aspx")                  ||
            //HttpContext.Current.Request.Url.LocalPath.Contains("/AddEditContact.aspx")                       ||

            HttpContext.Current.Request.Url.LocalPath.EndsWith("/Account/LogoutV2.aspx")                     ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/BookingSheetBlockoutV2.aspx")               ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/BookingCreateInvoiceV2.aspx")               ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/BookingCreateInvoiceAgedCareV2.aspx")       ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Contact_SuburbListPopupV2.aspx")            ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/ContactAusDetailV2.aspx")                   ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/ContactTypeListV2.aspx")                    ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/EPCDetailV2.aspx")                          ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Invoice_CreditNoteDetailV2.aspx")           ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Invoice_ReceiptDetailV2.aspx")              ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Invoice_RefundDetailV2.aspx")               ||
           (HttpContext.Current.Request.Url.LocalPath.Contains("/Invoice_ViewV2.aspx") && !HttpContext.Current.Request.Url.AbsoluteUri.Contains("is_popup=0")) ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/NoteListV2.aspx")                           ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Notifications_AddCreditEmailPopupV2.aspx")  ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/OfferingPopupMessageDetailV2.aspx")         ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/OrganisationListPopupV2.aspx")              ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/PatientFlashingMessageDetailV2.aspx")       ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/PatientScannedFileUploadsV2.aspx")          ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/PatientListPopupV2.aspx")                   ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/PatientReferrerHistoryPopupV2.aspx")        ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/ReferrerListPopupV2.aspx")                  ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/ReferrerDoctorListPopupV2.aspx")            ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/ReferrerClinicListPopupV2.aspx")            ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/StaffOfferingsBulkUpdateV2.aspx")           ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/CreditDetailV2.aspx");

        UpdateLogout(hideHeader);

        base.OnInit(e);
    }
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["ShowPageExecutionTime"]))
        {
            double executionTime = (double)(Environment.TickCount - startTime) / 1000.0;

            //if (Session["StaffID"] != null && Convert.ToInt32(Session["StaffID"]) < 0)
                lblPageLoadTime.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Load Time: " + executionTime + " seconds";
        }
    }



    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            SetupGUI();
            div_menu.Style["display"] = Request.Browser.Type.Contains("Firefox") ? "none" : "block";
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, true);
            throw;
        }
    }


    protected void lnkBackToCallCenter_Click(object sender, EventArgs e)
    {
        if (Session == null || Session["PreviousDB"] == null)
        {
            Utilities.LogoutV2(Session, Response, Request);
            return;
        }

        Session["DB"] = (string)Session["PreviousDB"];
        Session["SystemVariables"] = SystemVariableDB.GetAll();

        // Set system staff variables of external staff member 'Call Center'
        Staff staff = StaffDB.GetByID(Convert.ToInt32(Session["PreviousStaffID"]));
        Session["IsLoggedIn"]                       = true;
        Session["IsStakeholder"]                    = staff.IsStakeholder;
        Session["IsMasterAdmin"]                    = staff.IsMasterAdmin;
        Session["IsAdmin"]                          = staff.IsAdmin;
        Session["IsPrincipal"]                      = staff.IsPrincipal;
        Session["IsProvider"]                       = staff.IsProvider;
        Session["IsExternal"]                       = staff.IsExternal;
        Session["StaffID"]                          = staff.StaffID;
        Session["StaffFullnameWithoutMiddlename"]   = staff.Person.FullnameWithoutMiddlename;
        Session["StaffFirstname"]                   = staff.Person.Firstname;

        Site site = SiteDB.GetByID(Convert.ToInt32(Session["PreviousSiteID"]));
        Session["SiteID"]         = site.SiteID;
        Session["SiteName"]       = site.Name;
        Session["SiteIsClinic"]   = site.SiteType.ID == 1;
        Session["SiteIsAgedCare"] = site.SiteType.ID == 2;
        Session["SiteIsGP"]       = site.SiteType.ID == 3;
        Session["SiteTypeID"]     = site.SiteType.ID;
        Session["SiteTypeDescr"]  = site.SiteType.Descr;

        Session["IsMultipleSites"] = SiteDB.GetAll().Length > 1;



        Session.Remove("PreviousDB");
        Session.Remove("PreviousStaffID");
        Session.Remove("PreviousSiteID");


        // Set OrgID in session as external user has OrgID set
        Session.Remove("OrgID");
        Session.Remove("OrgName");

        // Remove patient list session data for pt searches
        Session.Remove("patientinfo_data");
        Session.Remove("patientlist_data");
        Session.Remove("patientlist_sortexpression");
        Session.Remove("patientinfo_sortexpression");

        // Go to call center page
        Response.Redirect("~/CallCenterV2.aspx", false);
        return;
    }

    protected void SetupGUI()
    {
        mi_patient_add_with_id.Visible = Session != null && Session["DB"] != null && Session["DB"].ToString() == "Mediclinic_0030";


        UserView userView = UserView.GetInstance();
        bool isLoggedInAsCallCenter = Session != null && Session["StaffID"] != null && (new List<int> { -5, -7, -8 }).Contains((int)Session["StaffID"]);

        if (isLoggedInAsCallCenter)
            nav.Style["background"] = nav_ul.Style["background"] = footer.Style["background"] = "#A3BEF5";

        lblSiteIsClinic.Value   = userView .IsClinicView? "1" : "0";
        lblSiteIsAgedCare.Value = userView.IsAgedCareView ? "1" : "0";

        banner.Style["background"] = (Session["SystemVariables"] == null || ((SystemVariables)Session["SystemVariables"])["MainLogo"] == null) ? "url(../imagesV2/comp_logo.png) no-repeat center center" : "url(../imagesV2/" + ((SystemVariables)Session["SystemVariables"])["MainLogo"].Value + ") no-repeat center center";

        lblSiteName.Text    = Session["SystemVariables"] == null ? "Mediclinic" : ((SystemVariables)Session["SystemVariables"])["Site"].Value;
        lblSiteName.Visible = lblDataStoredAusServersMsg.Visible = lnkLiveSupport.Visible = Session["SystemVariables"] != null;

        // when call centre logged in, put call-centre prefix showing at the end of the sitename always visible
        if (isLoggedInAsCallCenter)
            lblSiteName.Text += Session["SystemVariables"] == null ? "" : " <span style=\"color:#82cde9\">[" + ((SystemVariables)Session["SystemVariables"])["CallCenterPrefix"].Value + "]</span>";


        spn_login_display.Visible = userView.IsLoggedIn;

        if (userView.IsLoggedIn)
        {
            string siteOrgNameRaw = (userView.IsAdminView) ?
                (Session["SiteName"] == null ? "[No Site Selected]" : Utilities.TrimName((string)Session["SiteName"], 35, 2)) :
                (Session["SiteName"] == null ? "[No Site Selected]" : Utilities.TrimName((string)Session["SiteName"], 18, 2)) + (Session["SiteName"] == null ? "&nbsp;[" : "&nbsp;(") + (Session["OrgName"] == null ? "No Clinic Selected" : Utilities.TrimName((string)Session["OrgName"], 25, 2)) + (Session["SiteName"] == null ? "]" : ")");
            lblSiteOrOrg.Text = siteOrgNameRaw;

            lblUsername.Text = (siteOrgNameRaw.Length > 35) ?
                Utilities.TrimName((string)Session["StaffFullnameWithoutMiddlename"], 25, 2) :
                Session["StaffFullnameWithoutMiddlename"].ToString();


            if (Session["SiteID"] == null || Session["IsMultipleSites"] == null || (bool)Session["IsMultipleSites"] == false || Session["PatientID"] != null)
                lnkSelectSite.Visible = lnkSelectSite_OpenBracket.Visible = lnkSelectSite_CloseBracket.Visible = false;

            if (Session["OrgID"] == null || Session["IsMultipleOrgs"] == null || (bool)Session["IsMultipleOrgs"] == false)
                lnkSelectOrg.Visible = lnkSelectOrg_OpenBracket.Visible = lnkSelectOrg_CloseBracket.Visible = false;

            if (Session["SiteID"] == null || (!userView.IsAdminView && Session["OrgID"] == null))
                lnkChangePwd.Visible = lnkChangePwd_OpenBracket.Visible = lnkChangePwd_CloseBracket.Visible = false;
        }


        /*
         * set and hide/show menu items depending on user privileges
         */


        lblMenuOrganisations.Text                                   = !userView.IsAgedCareView ? "Clinics / Ins." : "Facilities / Ins.";
        lnkMenuOrganisationList.Text                                = !userView.IsAgedCareView ? "Clinics List"   : "Facilities List";
        lnkMenuOrganisationList.NavigateUrl                         = !userView.IsAgedCareView ? "~/OrganisationListV2.aspx?type=clinic" : "~/OrganisationListV2.aspx?type=ac";
        lnkMenuAddOrganisation.Text                                 = !userView.IsAgedCareView ? "Add Clinic"     : "Add Facility";
        lnkMenuAddOrganisation.NavigateUrl                          = !userView.IsAgedCareView ? "~/OrganisationDetailV2.aspx?type=add&orgtype=clinic" : "~/OrganisationDetailV2.aspx?type=add&orgtype=ac";
        lblMenuPatients.Text                                        = !userView.IsAgedCareView ? "Patients"       : "Residents";
        lnkMenuPatientList.Text                                     = !userView.IsAgedCareView ? "Patient List"   : "Resident List";
        lnkMenuPatientAdd.Text                                      = !userView.IsAgedCareView ? "Add Patient"    : "Add Resident";
        lnkMenuPatientAddWithID.Text                                = !userView.IsAgedCareView ? "Add Patient <b>WITH ID</b>"    : "Add Resident <b>WITH ID</b>";


        bool hasClinics = false;
        bool hasAC      = false;
        if (Session != null && Session["DB"] != null)
        {
            foreach (Site site in SiteDB.GetAll())
            {
                if (site.SiteType.ID == 1 || site.SiteType.ID == 3)
                    hasClinics = true;
                if (site.SiteType.ID == 2)
                    hasAC = true;
            }
        }

        if (hasClinics && hasAC)
            lnkMenuOrganisationCustomerTypes.Text = "Clinic/Facility Customer Types";
        else if (hasClinics)
            lnkMenuOrganisationCustomerTypes.Text = "Clinic Customer Types";
        else if (hasAC)
            lnkMenuOrganisationCustomerTypes.Text = "Facility Customer Types";


        mh_staff.Visible          = mh_staff_spacer.Visible         = userView.IsStakeholder || userView.IsMasterAdmin;
        mh_patients_space.Visible = mh_patients.Visible             = userView.IsAdminView   || userView.IsProviderView;
        mi_patient_cond_list.Visible                                = userView.IsAdminView;
        mh_organisation.Visible   = mh_organisation_spacer.Visible  = userView.IsAdminView;
        mh_referrers.Visible      = mh_referrers_space.Visible      = userView.IsAdminView   || userView.IsProviderView;
        mh_bookings_space.Visible = mh_bookings.Visible             = userView.IsAdminView   || userView.IsProviderView;
        lblMenuBookings.Text                                        = userView.IsAdminView ? "Bookings & Sales" : "Bookings";
        mi_bookings_offerings_seperator.Visible                     = userView.IsAdminView;
        mi_bookings_offerings_products_and_services_list.Visible    = userView.IsAdminView;
        mi_bookings_offerings_set_specific_prices.Visible           = userView.IsAdminView   || userView.IsProviderView;
        mi_bookings_offerings_stock.Visible                         = userView.IsAdminView   || userView.IsProviderView;
        mi_bookings_offerings_set_specific_prices.Visible           = userView.IsAdminView;
        mi_bookings_offerings_invoice_cash_add.Visible              = userView.IsAdminView;

        mh_sales.Visible          = mh_sales_space.Visible          = userView.IsAdminView   || userView.IsProviderView;
        mh_financials.Visible     = mh_financials_spacer.Visible    = userView.IsAdminView;
        mh_letters.Visible        = mh_letters_space.Visible        = userView.IsAdminView   || userView.IsProviderView;
        mh_sms_and_email.Visible  = mh_sms_and_email_spacer.Visible = userView.IsAdminView;
        mh_site_and_settings.Visible                                = userView.IsAdminView;

        mh_ext_patient_list.Visible                                 = mh_ext_patient_list_space.Visible = userView.IsExternalView;
        mh_ext_patient_add.Visible                                  = mh_ext_patient_add_space.Visible  = userView.IsExternalView;
        mh_ext_next_booking_space.Visible                           = userView.IsExternalView;
        mh_ext_next_booking.Visible                                 = mh_ext_bookings_space.Visible     = userView.IsExternalView;
        mh_ext_bookings_space.Visible                               = false;
        mh_ext_bookings.Visible                                     = userView.IsExternalView;
        //mh_ext_bookings.Visible                                   = mh_ext_bookings_space.Visible     = isExternal;
        mh_ext_return_to_callcenter.Visible = mh_ext_return_to_callcenter_space_post.Visible = isLoggedInAsCallCenter;

        mh_ext_return_to_callcenter_space_pre.Visible = false;

        if (isLoggedInAsCallCenter)
        {
            mh_ext_return_to_callcenter_space_pre.Visible  = true;
            mh_ext_return_to_callcenter_space_post.Visible = false;
        }



        if (userView.IsExternalView)
        {
            lnkMenuPatientListExt.NavigateUrl = "~/PatientListV2.aspx";
            lnkMenuPatientAddExt.NavigateUrl  = "~/PatientAddV2.aspx";
            lnkMenuBookingsExt.NavigateUrl    = "~/BookingsV2.aspx?orgs=" + Session["OrgID"] + ( Session != null && Session["StaffID"] != null && (int)Session["StaffID"] != -5 ? "&ndays=3" : "&ndays=4");
        }


        mi_patient_ac_types.Visible                          = userView.IsAgedCareView && userView.IsAdminView;
        mi_patient_ac_types_prices_per_fac.Visible           = userView.IsAgedCareView && userView.IsAdminView;

        mi_referrer_epc_letters_generate_unsent_list.Visible = userView.IsAdminView;
        mi_referrer_epc_letters_reprint_list.Visible         = userView.IsAdminView;

        mi_link_bookings.InnerText                           = userView.IsAdminView ? "Make Booking" : "Bookings";
        mi_bookings_call_center.Visible                      = userView.IsAdminView && !isLoggedInAsCallCenter && Session != null && Session["SystemVariables"] != null && ((SystemVariables)Session["SystemVariables"])["IsMediclinicCallCenter"].Value == "1";


        if (userView.IsAdminView)
        {
            mi_bookings_list.Visible = false;
        }
        if (!userView.IsAdminView)
        {
            mi_link_bookings.HRef      = "/BookingsV2.aspx?orgs=" + Session["OrgID"] + "&ndays=1";
            mi_link_bookings_list.HRef = "/BookingsListV2.aspx?staff=" + Session["StaffID"] + "&start_date=" + DateTime.Today.ToString("yyyy_MM_dd") + "&end_date=" + DateTime.Today.ToString("yyyy_MM_dd");

            mi_bookings_list.Visible                         = userView.IsAdminView || userView.IsProviderView;
            mi_bookings_report.Visible                       = userView.IsAdminView;
            mi_bookings_schedule_report.Visible              = userView.IsAdminView || userView.IsProviderView;
            mi_bookings_hours_worked_report.Visible          = userView.IsAdminView || userView.IsProviderView;
            mi_bookings_change_edit_reason.Visible           = userView.IsAdminView;
            mi_bookings_change_unavailability_reason.Visible = userView.IsAdminView;
        }


        mi_link_offerings_set_specific_prices.InnerText      = !userView.IsAgedCareView ? "Set Specific Prices Per Clinic" : "Set Specific Prices Per Facility/Wing/Unit";
        mi_link_offerings_invoice_cash_add.HRef              = userView.IsAdminView ? "/InvoiceCashAddV2.aspx" : "/InvoiceCashAddV2.aspx?org=" + (Session["OrgID"] == null ? "" : Session["OrgID"].ToString()); ;


        mh_sales.Visible = mh_sales_space.Visible = userView.IsProviderView;
        mi_link_sales_invoice_cash_add.HRef       = userView.IsAdminView ? 
                                                        "/InvoiceCashAddV2.aspx" : 
                                                        "/InvoiceCashAddV2.aspx?org=" + (Session["OrgID"] == null ? "" : Session["OrgID"].ToString());

        mi_financials_ezidebit_info.Visible        = userView.IsStakeholder || userView.IsMasterAdmin;
        mi_financials_claim_nbr_allocation.Visible = userView.IsStakeholder;
        mi_financials_claim_nbrs_allocated.Visible = userView.IsStakeholder;
        mi_financials_hinx_generation.Visible      = userView.IsStakeholder;


        if (!userView.IsAdminView)
        {
            mi_letters_maintain.Visible                      = false;
            mi_letters_maintain_treatment_letters.Visible    = false;
            mi_letters_print_batch.Visible                   = false;
            mi_letters_print_batch_referrers.Visible         = false;
            mi_letters_recall.Visible                        = false;
            mi_letters_service_specific_bk_reminders.Visible = false;

            mi_link_letters_print.HRef        += (Session != null && Session["OrgID"] != null ? "?org=" + Session["OrgID"].ToString() : "");
            mi_link_letters_sent_history.HRef += (Session != null && Session["OrgID"] != null ? "?org=" + Session["OrgID"].ToString() : "");
        }
        else
        {
            mi_letters_recall.Visible = !userView.IsAgedCareView;
        }


        mi_website_settings.Visible   = userView.IsStakeholder || userView.IsMasterAdmin;
        mi_add_aged_care_site.Visible = userView.IsStakeholder && SiteDB.GetSiteByType(SiteDB.SiteType.AgedCare) == null;
        mi_add_new_field.Visible      = userView.IsStakeholder;
        mi_create_new_site.Visible    = userView.IsStakeholder;


        if (HttpContext.Current.Request.Url.LocalPath.Contains("/Account/LoginV2.aspx")            ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Account/SelectOrgV2.aspx")        ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Account/SelectSiteV2.aspx")       ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Account/CreateNewLoginV2.aspx")   ||
            HttpContext.Current.Request.Url.LocalPath.Contains("/Account/LostPasswordV2.aspx")     ||
            HttpContext.Current.Request.Url.LocalPath.EndsWith("/Account/CreateNewPatientV2.aspx") ||
            HttpContext.Current.Request.Url.LocalPath.EndsWith("/InvoicePaymentV2.aspx")           ||
            HttpContext.Current.Request.Url.LocalPath.EndsWith("/PatientUnsubscribeV2.aspx")       ||
            HttpContext.Current.Request.Url.LocalPath.EndsWith("/Invoice_WebPayV2.aspx")           ||
            HttpContext.Current.Request.Url.LocalPath.EndsWith("/CreateNewCustomerSiteV2.aspx")    ||
            HttpContext.Current.Request.Url.LocalPath.EndsWith("/TermsAndConditionsV2.aspx")
            )
            div_menu2.Visible = false;


        if (Session["SystemVariables"] != null && !Page.Title.StartsWith(((SystemVariables)Session["SystemVariables"])["Site"].Value + " - "))
            Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + Page.Title;


        int s = Session["StaffID"] == null ? -1 : (int)Session["StaffID"];
        string db = Session["DB"] == null ? "" : (string)Session["DB"];

        if ((Session["DB"] != null && (string)Session["DB"] == "Mediclinic_0034") && (Session["StaffID"] != null && (new List<int> { -5, -7, -8 }).Contains((int)Session["StaffID"])))
        {
            lnkLiveSupport.Attributes.Add("onclick", "open_new_tab('http://www.homevisitphysio.com.au/phoneadmin');return false");
            lnkLiveSupport.NavigateUrl = "http://www.homevisitphysio.com.au/phoneadmin";
            lnkLiveSupport.Text = "&nbsp;PHONE SCRIPT&nbsp;";
            lnkLiveSupport.Style["background-color"] = "#BA9EB0";
            lnkLiveSupport.Style["color"] = "white";
        }
    }


    protected void UpdateLogout(bool hideHeader)
    {
        UserView userView = UserView.GetInstance();

        if (!userView.IsLoggedIn)
        {
            Logout(hideHeader);
            return;
        }

        // if another session logged in - logout here
        if (Session["StaffID"] == null || !(new List<int> { -5, -7, -8 }).Contains((int)Session["StaffID"]))
        {
            UserLogin userlogin = null;
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["DB"] != null)
            {
                userlogin = !userView.IsPatient ?
                    UserLoginDB.GetByUserID(Convert.ToInt32(Session["StaffID"]), -1) :
                    UserLoginDB.GetByUserID(-1, Convert.ToInt32(Session["PatientID"]));
            }

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


            if (!userView.IsAdminView && Session["OrgID"] == null &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/LoginV2.aspx") &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/LogoutV2.aspx") &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/SelectOrgV2.aspx") &&
                !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/SelectSiteV2.aspx"))
                Response.Redirect("~/Account/SelectOrgV2.aspx?from_url=" + Request.RawUrl);

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
        if (!HttpContext.Current.Request.Url.LocalPath.Contains("/Account/LoginV2.aspx")            &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/Account/LogoutV2.aspx")           &&
            !HttpContext.Current.Request.Url.LocalPath.Contains("/Account/CreateNewLoginV2.aspx")   &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/Account/LostPasswordV2.aspx")     &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/Account/CreateNewPatientV2.aspx") &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/InvoicePaymentV2.aspx")           &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/PatientUnsubscribeV2.aspx")       &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/Invoice_WebPayV2.aspx")           &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/BookingNextAvailableV2.aspx")     &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/BookingNextAvailableV3.aspx") &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/CreateNewCustomerSiteV2.aspx") &&
            !HttpContext.Current.Request.Url.LocalPath.EndsWith("/TermsAndConditionsV2.aspx"))
            Response.Redirect("~/Account/LoginV2.aspx" + (Request.RawUrl == "" || Request.RawUrl == "/" || Request.RawUrl.StartsWith("/Default.aspx") ? "" : "?show_header=" + (hideHeader ? "0" : "1") + "&from_url=" + Request.RawUrl));

    }

}
