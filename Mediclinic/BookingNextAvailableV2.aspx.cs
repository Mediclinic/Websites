using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections;

public partial class BookingNextAvailableV2 : System.Web.UI.Page
{

    /* http://localhost:88/BookingNextAvailableV2.aspx?id=0001
     * 
     * BookingsV2.aspx?orgs=1751&date=2015_01_23&field=68&offering=28
     * 
     * 
     * from next booking screen:
     * 
     * put booking in return url .. and go to sign up screen http://localhost:88/Account/CreateNewPatientV2.aspx?id=0001
     * 
     * at sign up screen at top ... have link "Already have a login/pwd? Sign In Here" .. and add return url to that
     * when logging in ... at login page .. if is pt ... and if has "BookingsV2.aspx?" and "orgs=\d+" (with one org only) in return url ... then auto set that org .. and go to return_url
     */

    #region IsLoggedIn(), GetDB()

    private bool IsLoggedIn()
    {
        return Session != null && UserView.GetInstance().IsLoggedIn;
    }
    private string GetDB()
    {
        if (IsLoggedIn())
            return (string)Session["DB"];
        else
            return Utilities.GetDB(Request.QueryString["id"]);
    }

    #endregion


    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        bool isLoggedIn = IsLoggedIn();
        try
        {
            string DB = GetDB();
            if (DB == null)
                throw new CustomMessageException("Invalid ID in URL");

            if (!isLoggedIn)
                Session["DB"] = DB;


            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                //PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, false, false, false, false);

                SetupGUI(isLoggedIn, DB);

                if (IsValidFormProviderID() && IsValidFormOrgID() && IsValidFormStartDate() && IsValidFormEndDate())
                {
                    FillGrid();

                    // if patient, set they need the org to be set in the session
                    if (UserView.GetInstance().IsPatient)
                    {
                        Organisation org = OrganisationDB.GetByID(GetFormOrgID());
                        if (org != null) // if "All Clinics" selected
                        {
                            Session["OrgID"] = org.OrganisationID;
                            Session["OrgName"] = org.Name;
                        }
                    }

                }
                else if (GetFormProviderID(false) == -1 && IsValidFormOrgID() && IsValidFormStartDate() && IsValidFormEndDate())
                {
                    SetErrorMessage("No Providers Linked To Selectable Clinics" + "<br />");
                    return;
                }
            }
        }
        catch (CustomMessageException ex)
        {
            if (IsPostBack) SetErrorMessage(ex.Message);
            else HideTableAndSetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            if (IsPostBack) SetErrorMessage("", ex.ToString());
            else HideTableAndSetErrorMessage("", ex.ToString());
        }
        finally
        {
            if (!isLoggedIn)
                Session.Remove("DB");
        }

        if (!isLoggedIn)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl div = Master.FindControl("div_menu2") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (div != null)
                div.Visible = true;
        }


    }

    #endregion

    #region SetupGUI, IsValidXXX/GetXXX

    protected void SetupGUI(bool isLoggedIn, string DB)
    {
        UserView userView = UserView.GetInstance();


        ddlServices.Style["width"] = "350px";
        ddlServices.Items.Clear();
        foreach (Offering offering in OfferingDB.GetAll(false, "1,3", "63", false))
            ddlServices.Items.Add(new ListItem(offering.Name, offering.OfferingID.ToString()));


        Organisation[] orgs = null;
        if (isLoggedIn && userView.IsPatient)
            orgs = RegisterPatientDB.GetOrganisationsOf(Convert.ToInt32(Session["PatientID"]), true, false, true, true, true);
        else if (isLoggedIn && userView.IsExternal)
            orgs = new Organisation[] { OrganisationDB.GetByID(Convert.ToInt32(Session["OrgID"])) };
        else if (isLoggedIn && !userView.IsAdminView)
            orgs = RegisterStaffDB.GetOrganisationsOf(Convert.ToInt32(Session["StaffID"]));
        else
            orgs = OrganisationDB.GetAll(false, true, false, true, true, true);


        ddlClinics.Style["width"] = "350px";
        ddlClinics.Items.Clear();
        ddlClinics.Items.Add(new ListItem("All Clinics", "0"));
        foreach (Organisation org in orgs)
            ddlClinics.Items.Add(new ListItem(org.Name, org.OrganisationID.ToString()));


        // only show providers working at that clinic

        int[]   orgIDs    = orgs.Select(r => r.OrganisationID).ToArray();
        Staff[] providers = RegisterStaffDB.GetStaffOf(orgIDs, true);

        ddlProviders.Style["width"] = "350px";
        ddlProviders.Items.Clear();
        if (providers.Length > 0)
            ddlProviders.Items.Add(new ListItem("All Providers", "0"));
        foreach (Staff staff in providers)
            ddlProviders.Items.Add(new ListItem(staff.Person.FullnameWithoutMiddlename + " (" + staff.Field.Descr + ")", staff.StaffID.ToString()));

        if (providers.Length == 0)
            ddlProviders.Items.Add(new ListItem("[No Providers Linked To Selectable Clinics]", (-1).ToString()));


        if (IsValidFormOfferingID())
        {
            Offering offering = OfferingDB.GetByID(GetFormOfferingID());
            if (offering != null)
                ddlServices.SelectedValue = offering.OfferingID.ToString();
        }
        if (!IsPostBack && !IsValidFormOfferingID())
        {
            int defaultServiceID = Convert.ToInt32(SystemVariableDB.GetByDescr("BookingScreenDefaultServiceID").Value);
            if (defaultServiceID != -1 && ddlServices.Items.FindByValue(defaultServiceID.ToString()) != null)
                ddlServices.SelectedValue = defaultServiceID.ToString();
        }


        if (IsValidFormOrgID())
        {
            Organisation org = OrganisationDB.GetByID(GetFormOrgID());
            if (org != null)
                ddlClinics.SelectedValue = org.OrganisationID.ToString();
        }
        else if (userView.IsPatient && Session["OrgID"] != null)
        {
            ddlClinics.SelectedValue = Session["OrgID"].ToString();
        }

        if (IsValidFormProviderID())
        {
            Staff provider = StaffDB.GetByID(GetFormProviderID());
            if (provider != null)
                ddlProviders.SelectedValue = provider.StaffID.ToString();
        }

        txtStartDate.Text = IsValidFormStartDate() ? GetFormStartDate(false).ToString("dd-MM-yyyy") : DateTime.Now.ToString("dd-MM-yyyy");
        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";

        int defaultNbrDays = Convert.ToInt32(SystemVariableDB.GetByDescr("NextAvailableDefaultNbrDaysShown").Value);
        txtEndDate.Text = IsValidFormEndDate() ? GetFormEndDate(false).ToString("dd-MM-yyyy") : DateTime.Now.AddDays(defaultNbrDays).ToString("dd-MM-yyyy");
        txtEndDate_Picker.OnClientClick = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";

    }

    private bool IsValidFormProviderID()
    {
        string id = Request.QueryString["provider"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormProviderID(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormProviderID())
            throw new Exception("Invalid url provider");

        string id = Request.QueryString["provider"];
        return Convert.ToInt32(id);
    }
    private bool IsValidFormOrgID()
    {
        string id = Request.QueryString["org"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormOrgID()
    {
        if (!IsValidFormOrgID())
            throw new Exception("Invalid url org");

        string id = Request.QueryString["org"];
        return Convert.ToInt32(id);
    }
    private bool IsValidFormOfferingID()
    {
        string id = Request.QueryString["offering"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormOfferingID()
    {
        if (!IsValidFormOfferingID())
            throw new Exception("Invalid url org");

        string id = Request.QueryString["offering"];
        return Convert.ToInt32(id);
    }

    protected bool IsValidFormStartDate()
    {
        string start_date = Request.QueryString["start_date"];
        return start_date != null && Regex.IsMatch(start_date, @"^\d{4}_\d{2}_\d{2}$");
    }
    protected DateTime GetFormStartDate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormStartDate())
            throw new Exception("Invalid url 'start date'");
        return GetDateFromString(Request.QueryString["start_date"], "yyyy_mm_dd");
    }

    protected bool IsValidFormEndDate()
    {
        string end_date = Request.QueryString["end_date"];
        return end_date != null && Regex.IsMatch(end_date, @"^\d{4}_\d{2}_\d{2}$");
    }
    protected DateTime GetFormEndDate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormEndDate())
            throw new Exception("Invalid url 'end date'");
        return GetDateFromString(Request.QueryString["end_date"], "yyyy_mm_dd");
    }

    protected DateTime GetDateFromString(string sDate, string format)
    {
        if (format == "yyyy_mm_dd")
        {
            string[] dateparts = sDate.Split('_');
            return new DateTime(Convert.ToInt32(dateparts[0]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[2]));
        }
        else if (format == "dd_mm_yyyy")
        {
            string[] dateparts = sDate.Split('_');
            return new DateTime(Convert.ToInt32(dateparts[2]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[0]));
        }
        if (format == "yyyy-mm-dd")
        {
            string[] dateparts = sDate.Split('-');
            return new DateTime(Convert.ToInt32(dateparts[0]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[2]));
        }
        else if (format == "dd-mm-yyyy")
        {
            string[] dateparts = sDate.Split('-');
            return new DateTime(Convert.ToInt32(dateparts[2]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[0]));
        }
        else
            throw new ArgumentOutOfRangeException("Unknown date format");
    }
    protected bool IsValidDate(string strDate)
    {
        try
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(strDate, @"^\d{2}\-\d{2}\-\d{4}$"))
                return false;

            string[] parts = strDate.Split('-');
            DateTime d = new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    protected DateTime GetDate(string strDate)
    {
        string[] parts = strDate.Split('-');
        return new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]));
    }

    #endregion

    #region btnSearch_Click

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (!Regex.IsMatch(txtStartDate.Text, @"^\d{2}\-\d{2}\-\d{4}$"))
        {
            SetErrorMessage("Start date must be of the format dd-mm-yyyy");
            return;
        }
        else if (!Regex.IsMatch(txtEndDate.Text, @"^\d{2}\-\d{2}\-\d{4}$"))
        {
            SetErrorMessage("End date must be of the format dd-mm-yyyy");
            return;
        }
        else
            HideErrorMessage();


        DateTime startDate = GetDateFromString(txtStartDate.Text, "dd-mm-yyyy");
        DateTime endDate = GetDateFromString(txtEndDate.Text, "dd-mm-yyyy");


        string url = ClearSearchesFromUrl(Request.RawUrl);
        url = UrlParamModifier.AddEdit(url, "start_date", startDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "end_date", endDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "offering", ddlServices.SelectedValue.ToString());
        url = UrlParamModifier.AddEdit(url, "provider", ddlProviders.SelectedValue.ToString());
        url = UrlParamModifier.AddEdit(url, "org", ddlClinics.SelectedValue.ToString());
        Response.Redirect(url);
    }

    protected string ClearSearchesFromUrl(string url)
    {
        url = UrlParamModifier.Remove(url, "start_date");
        url = UrlParamModifier.Remove(url, "end_date");
        url = UrlParamModifier.Remove(url, "offering");
        url = UrlParamModifier.Remove(url, "provider");
        url = UrlParamModifier.Remove(url, "org");

        return url;
    }

    #endregion

    #region FillGrid()

    protected void FillGrid()
    {
        DateTime fromDate = IsValidDate(txtStartDate.Text) ? GetDate(txtStartDate.Text)          : DateTime.Now.Date;
        DateTime endDate  = IsValidDate(txtEndDate.Text)   ? GetDate(txtEndDate.Text).AddDays(1) : fromDate.AddDays(42);


        Site      site             = SiteDB.GetSiteByType(SiteDB.SiteType.Clinic);
        Offering  selectedOffering = OfferingDB.GetByID(Convert.ToInt32(ddlServices.SelectedValue));


        // if all clinics selected
        if (Convert.ToInt32(ddlClinics.SelectedValue) == 0)
        {
            int totalDays = (int)endDate.Subtract(fromDate).TotalDays;


            Organisation[] allClinics   = OrganisationDB.GetAll(false, true, false, true, true, true, "", false, "218");
            Staff[]        allProviders = StaffDB.GetProviders();

            Hashtable orgHash  = new Hashtable();
            Hashtable provHash = new Hashtable();
            foreach (Organisation org  in allClinics)   orgHash[org.OrganisationID] = org;
            foreach (Staff        prov in allProviders) provHash[prov.StaffID]      = prov;
            Hashtable  registerStaffHash = RegisterStaffDB.Get2DHashByStaffIDOrgID();


            Hashtable orgsInDropdownHash  = new Hashtable();
            Hashtable provsInDropdownHash = new Hashtable();
            for (int i = 0; i < ddlClinics.Items.Count; i++)
                if (ddlClinics.Items[i].Value != "0")
                    orgsInDropdownHash[Convert.ToInt32(ddlClinics.Items[i].Value)] = (Organisation)orgHash[Convert.ToInt32(ddlClinics.Items[i].Value)];
            for (int i = 0; i < ddlProviders.Items.Count; i++)
                if (ddlProviders.Items[i].Value != "0")
                    provsInDropdownHash[Convert.ToInt32(ddlProviders.Items[i].Value)] = (Staff)provHash[Convert.ToInt32(ddlProviders.Items[i].Value)];


            ArrayList orgsToShow  = new ArrayList();
            ArrayList provsToShow = new ArrayList();

            if (Convert.ToInt32(ddlClinics.SelectedValue) == 0) // all clinics selected
            {
                if (Convert.ToInt32(ddlProviders.SelectedValue) != 0) // provider selected so get registered orgs of this provider
                {
                    foreach(Organisation org in RegisterStaffDB.GetOrganisationsOf(Convert.ToInt32(ddlProviders.SelectedValue)))
                        if (org.OrganisationType.OrganisationTypeID == 218 && orgsInDropdownHash[org.OrganisationID] != null)
                            orgsToShow.Add(org);

                    if (orgsToShow.Count == 0)
                    {
                        Staff staff = (Staff)provHash[Convert.ToInt32(ddlProviders.SelectedValue)];
                        SetErrorMessage(staff.Person.FullnameWithoutMiddlename + " is not registered at any clinics.<br />");
                        return;
                    }
                }
                else
                {
                    for (int i = 0; i < ddlClinics.Items.Count; i++)  // get all orgs
                        if (ddlClinics.Items[i].Value != "0")
                            orgsToShow.Add((Organisation)orgHash[Convert.ToInt32(ddlClinics.Items[i].Value)]);
                }
            }
            else
            {
                orgsToShow.Add((Organisation)orgHash[Convert.ToInt32(ddlClinics.SelectedValue)]);
            }

            if (Convert.ToInt32(ddlProviders.SelectedValue) == 0)
            {
                if (Convert.ToInt32(ddlClinics.SelectedValue) != 0) // clinic selected so get registered providers of this clinic
                {
                    foreach (Staff staff in RegisterStaffDB.GetStaffOf(Convert.ToInt32(ddlClinics.SelectedValue), true))
                        if (provsInDropdownHash[staff.StaffID] != null)
                            provsToShow.Add(staff);

                    if (provsToShow.Count == 0)
                    {
                        Organisation org = (Organisation)orgHash[Convert.ToInt32(ddlClinics.SelectedValue)];
                        SetErrorMessage(org.Name + " does not have any providers registered to work there.<br />");
                        return;
                    }
                }
                else
                {
                    for (int i = 0; i < ddlProviders.Items.Count; i++) // get all providers
                        if (ddlProviders.Items[i].Value != "0")
                            provsToShow.Add((Staff)provHash[Convert.ToInt32(ddlProviders.Items[i].Value)]);
                }
            }
            else
            {
                provsToShow.Add((Staff)provHash[Convert.ToInt32(ddlProviders.SelectedValue)]);
            }



            Booking[] bookings = BookingDB.GetBetween(fromDate, endDate, null, null, null, null);

            ArrayList dataList = new ArrayList();
            for (int i = 0; i < orgsToShow.Count; i++)
            {
                for (int j = 0; j < provsToShow.Count; j++)
                {
                    Organisation org   = (Organisation)orgsToShow[i];
                    Staff        staff = (Staff)provsToShow[j];

                    RegisterStaff registerStaff = registerStaffHash[new Hashtable2D.Key(staff.StaffID, org.OrganisationID)] as RegisterStaff;
                    if (registerStaff == null)
                        continue;

                    string[] provData = GetProviderData(site, selectedOffering, fromDate, endDate, staff, org, registerStaff, GetBookingListThisOrgProv(bookings, staff, org), true);
                    dataList.Add(provData);
                }
            }


            string[,] data = new string[totalDays + 3, dataList.Count];
            for (int i = 0; i < totalDays + 3; i++)
                for (int j = 0; j < dataList.Count; j++)
                    data[i, j] = ((string[])dataList[j])[i];


            int d = 0;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"table table-bordered table-grid table-grid-top-bottum-padding-normal table-grid-left-right-padding-normal auto_width block_center\">");

            sb.AppendLine("<tr>" + "<th colspan=\"4\"></th>");

            string previousClinic = null;
            for (int i = 0; i < data.GetLength(1); i++)
            {
                string thisClinic = data[d, i];
                if (previousClinic == null || thisClinic != previousClinic)
                {
                    int colspan = 1;
                    for (int j = i + 1; j < data.GetLength(1) && thisClinic == data[d, j]; j++)
                            colspan++;

                    sb.AppendLine("<th colspan=\"" + (4 * colspan) + "\"  style=\"max-width:215px;\">" + data[d, i] + "</th>");

                    previousClinic = thisClinic;
                }
            }

            sb.AppendLine("</tr>");

            d++;

            sb.AppendLine("<tr>" + "<th colspan=\"4\"></th>");
            for (int i = 0; i < data.GetLength(1); i++)
                sb.AppendLine("<th colspan=\"4\">" + data[d, i] + "</th>");
            sb.AppendLine("</tr>");

            d++;

            sb.AppendLine("<tr>" + "<th colspan=\"4\"></th>");
            for (int i = 0; i < data.GetLength(1); i++)
                sb.AppendLine("<th colspan=\"4\"><b>" + data[d, i] + "</b></th>");
            sb.AppendLine("</tr>");

            d++;

            for (DateTime curDate = fromDate; curDate < endDate; curDate = curDate.AddDays(1), d++)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td style=\"white-space:nowrap;                            border-right:none !important;text-align:left !important;\">&nbsp;&nbsp;" + curDate.ToString("ddd") + "</td>");
                sb.AppendLine("<td style=\"white-space:nowrap;border-left:none !important;border-right:none !important;text-align:left !important;\">" + curDate.ToString("d ") + "</td>");
                sb.AppendLine("<td style=\"white-space:nowrap;border-left:none !important;border-right:none !important;text-align:left !important;\">" + curDate.ToString("MMM") + "</td>");
                sb.AppendLine("<td style=\"white-space:nowrap;border-left:none !important;                             text-align:left !important;\">" + curDate.ToString("yyyy") + "&nbsp;&nbsp;</td>");

                for (int i = 0; i < data.GetLength(1); i++)
                    sb.AppendLine(data[d, i]);

                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            lblOutput.Text = sb.ToString();


            if (dataList.Count == 1)
            {
                autodivheight.Style["width"] = "auto";
                autodivheight.Style["padding-right"] = "17px";
            }
        }



        // all providers one clinic selected
        else if (Convert.ToInt32(ddlProviders.SelectedValue) == 0)
        {
            int totalDays = (int)endDate.Subtract(fromDate).TotalDays;

            Organisation org = OrganisationDB.GetByID(Convert.ToInt32(ddlClinics.SelectedValue));

            //string[,] data = new string[totalDays, ddlProviders.Items.Count];
            ArrayList dataList = new ArrayList();

            for (int i = 0; i < ddlProviders.Items.Count; i++)
            {
                if (ddlProviders.Items[i].Value == "0") // ignore the "All Staff"
                    continue;

                int           staffID       = Convert.ToInt32(ddlProviders.Items[i].Value);
                Staff         staff         = StaffDB.GetByID(Convert.ToInt32(ddlProviders.Items[i].Value));
                RegisterStaff registerStaff = RegisterStaffDB.GetByStaffIDAndOrganisationID(staff.StaffID, org.OrganisationID);

                if (registerStaff == null)
                    continue;

                string[] provData = GetProviderData(site, selectedOffering, fromDate, endDate, staff, org, registerStaff);
                //for(int j = 0; j < totalDays; j++)
                //    data[j, i] = provData[j];
                dataList.Add(provData);
            }

            if (dataList.Count == 0)
            {
                SetErrorMessage(org.Name + " does not have any providers registered to work there.<br />");
                return;
            }

            string[,] data = new string[totalDays + 2, dataList.Count];
            for(int i=0; i<totalDays + 2; i++)
                for (int j = 0; j < dataList.Count; j++)
                    data[i, j] = ((string[])dataList[j])[i];


            int d = 0;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"table table-bordered table-grid table-grid-top-bottum-padding-normal table-grid-left-right-padding-normal auto_width block_center\">");

            sb.AppendLine("<tr>" + "<th colspan=\"4\"></th>");
            for (int i = 0; i < data.GetLength(1); i++)
                sb.AppendLine("<th colspan=\"4\">" + data[d, i] + "</th>");
            sb.AppendLine("</tr>");

            d++;

            sb.AppendLine("<tr>" + "<th colspan=\"4\"></th>");
            for (int i = 0; i < data.GetLength(1); i++)
                sb.AppendLine("<th colspan=\"4\"><b>" + data[d, i] + "</b></th>");
            sb.AppendLine("</tr>");

            d++;

            for (DateTime curDate = fromDate; curDate < endDate; curDate = curDate.AddDays(1), d++)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td style=\"white-space:nowrap;                            border-right:none !important;text-align:left !important;\">&nbsp;&nbsp;" + curDate.ToString("ddd") + "</td>");
                sb.AppendLine("<td style=\"white-space:nowrap;border-left:none !important;border-right:none !important;text-align:left !important;\">" + curDate.ToString("d ") + "</td>");
                sb.AppendLine("<td style=\"white-space:nowrap;border-left:none !important;border-right:none !important;text-align:left !important;\">" + curDate.ToString("MMM") + "</td>");
                sb.AppendLine("<td style=\"white-space:nowrap;border-left:none !important;                             text-align:left !important;\">" + curDate.ToString("yyyy") + "&nbsp;&nbsp;</td>");

                for (int i = 0; i < data.GetLength(1); i++)
                    sb.AppendLine(data[d, i]);

                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            
            lblOutput.Text = sb.ToString();


            if (dataList.Count == 1)
            {
                autodivheight.Style["width"] = "auto";
                autodivheight.Style["padding-right"] = "17px";
            }

        }


        // one provider one clinic
        else
        {
            autodivheight.Style["width"] = "auto";
            autodivheight.Style["padding-right"] = "17px";

            Staff[]        staff         = new Staff[]        { StaffDB.GetByID(Convert.ToInt32(ddlProviders.SelectedValue))      };
            Organisation[] orgs          = new Organisation[] { OrganisationDB.GetByID(Convert.ToInt32(ddlClinics.SelectedValue)) };
            RegisterStaff  registerStaff = RegisterStaffDB.GetByStaffIDAndOrganisationID(staff[0].StaffID, orgs[0].OrganisationID);

            if (registerStaff == null)
            {
                SetErrorMessage(staff[0].Person.FullnameWithoutMiddlename + " does not work at " + orgs[0].Name + "<br />");
                return;
            }


            Booking[] bookings         = BookingDB.GetBetween(fromDate, endDate, staff, orgs, null, null);

        
            // seperate into 
            // - individual bookings that can be retrieved by date
            // - recurring  bookings that can be retrieved by day of week
            Hashtable bkRecurringHash = new Hashtable();
            Hashtable bkDateHash      = new Hashtable();
            foreach (Booking curBooking in bookings)
            {
                if (curBooking.Provider != null && curBooking.Provider.StaffID != staff[0].StaffID)
                    continue;
                //if (curBooking.Organisation != null && curBooking.Organisation.OrganisationID != orgs[0].OrganisationID)
                //    continue;

                if (!curBooking.IsRecurring)
                {
                    if (bkDateHash[curBooking.DateStart.Date] == null)
                        bkDateHash[curBooking.DateStart.Date] = new ArrayList();
                    if (IsWorkingToday(curBooking.DateStart.DayOfWeek, registerStaff))
                        ((ArrayList)bkDateHash[curBooking.DateStart.Date]).Add(curBooking);
                }
                else // curBooking.IsRecurring
                {
                    if (bkRecurringHash[curBooking.RecurringDayOfWeek] == null)
                        bkRecurringHash[curBooking.RecurringDayOfWeek] = new ArrayList();
                    if (IsWorkingToday(curBooking.RecurringDayOfWeek, registerStaff))
                        ((ArrayList)bkRecurringHash[curBooking.RecurringDayOfWeek]).Add(curBooking);
                }
            }


            string output1 = string.Empty;
            //output1 += "Service Time = " + selectedOffering.ServiceTimeMinutes + " Mins<br />";
            output1 += "<table class=\"table table-bordered table-grid table-grid-top-bottum-padding-normal table-grid-left-right-padding-normal auto_width block_center\">";

            output1 += "<tr>"; 
            output1 += "<th colspan=\"8\" style=\"white-space:nowrap;\">" + staff[0].Person.FullnameWithoutMiddlename + "</th>";
            output1 += "</tr>";

            for (DateTime curDate = fromDate; curDate < endDate; curDate = curDate.AddDays(1))
            {
                Tuple<DateTime, DateTime> startEndTime = GetStartEndTime(curDate, orgs[0], site);
                Tuple<DateTime, DateTime> orgLunchTime = GetOrgLunchStartTime(curDate, orgs[0]);


                Booking[] todayBookings  = bkDateHash[curDate]                == null ? new Booking[] { } : (Booking[])((ArrayList)bkDateHash[curDate]).ToArray(typeof(Booking));
                Booking[] todayRecurring = bkRecurringHash[curDate.DayOfWeek] == null ? new Booking[] { } : (Booking[])((ArrayList)bkRecurringHash[curDate.DayOfWeek]).ToArray(typeof(Booking));


                string bookingSheetLink = IsLoggedIn() ?
                    "<a href= \"" + Booking.GetLink(curDate, new int[] { orgs[0].OrganisationID }) + "\"><img src=\"/images/Calendar-icon-24px.png\" alt=\"Go To Booking Sheet\" title=\"Go To Booking Sheet\"></a>" :
                    "<a href= \"/Account/CreateNewPatientV2.aspx?id=" + Request.QueryString["id"] + "&from_url=" + Server.UrlEncode(Booking.GetLink(curDate, new int[] { orgs[0].OrganisationID })) + "\"><img src=\"/images/Calendar-icon-24px.png\" alt=\"Go To Booking Sheet\" title=\"Go To Booking Sheet\"></a>";


                string dateCols = 
                    "<td style=\"white-space:nowrap;                            border-right:none !important;text-align:left !important;\">&nbsp;&nbsp;" + curDate.ToString("ddd")  + "</td>" +
                    "<td style=\"white-space:nowrap;border-left:none !important;border-right:none !important;text-align:left !important;\">" + curDate.ToString("d ")    + "</td>" +
                    "<td style=\"white-space:nowrap;border-left:none !important;border-right:none !important;text-align:left !important;\">" + curDate.ToString("MMM")  + "</td>" +
                    "<td style=\"white-space:nowrap;border-left:none !important;border-right:none !important;text-align:left !important;\">" + curDate.ToString("yyyy") + "&nbsp;&nbsp;</td>";


                bool a = IsWorkingToday(curDate.DayOfWeek, registerStaff);
                bool b = IsClinicOpenToday(curDate, orgs[0]);

                if (IsWorkingToday(curDate.DayOfWeek, registerStaff) && IsClinicOpenToday(curDate, orgs[0]))
                {
                    //foreach (Booking curBooking in todayBookings)
                    //    output1 += "<tr><td></td><td>" + curBooking.BookingID + "</td><td>" + curBooking.BookingTypeID + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Organisation == null ? "" : curBooking.Organisation.Name) + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Provider == null ? "" : curBooking.Provider.Person.FullnameWithoutMiddlename) + "</td><td style=\"white-space:nowrap;\">" + curBooking.DateStart.ToString("dd MMM yyyy") + "</td><td style=\"white-space:nowrap;\">" + "[" + curBooking.DateStart.ToString("HH:mm") + " - " + curBooking.DateEnd.ToString("HH:mm") + "]" + "</td></tr>";
                    //
                    //foreach (Booking curBooking in todayRecurring)
                    //    if (curDate >= curBooking.DateStart.Date && curDate <= curBooking.DateEnd.Date)
                    //        output1 += "<tr><td> <b>R</b> </td><td>" + curBooking.BookingID + "</td><td>" + curBooking.BookingTypeID + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Organisation == null ? "" : curBooking.Organisation.Name) + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Provider == null ? "" : curBooking.Provider.Person.FullnameWithoutMiddlename) + "</td><td style=\"white-space:nowrap;\">" + curBooking.DateStart.ToString("dd MMM yyyy") + "</td><td style=\"white-space:nowrap;\">" + "[" + new DateTime(curBooking.RecurringStartTime.Ticks).ToString("HH:mm") + " - " + new DateTime(curBooking.RecurringEndTime.Ticks).ToString("HH:mm") + "]" + "</td></tr>";


                    // list of all TAKEN times
                    List<Tuple<DateTime, DateTime>> dateRows = new List<Tuple<DateTime, DateTime>>();

                    // add lunch breaks
                    if (site.LunchStartTime < site.LunchEndTime)
                        AddToDateRows(ref dateRows, startEndTime, curDate.Add(site.LunchStartTime), curDate.Add(site.LunchEndTime));
                    if (orgLunchTime.Item1 < orgLunchTime.Item2)
                        AddToDateRows(ref dateRows, startEndTime, orgLunchTime.Item1, orgLunchTime.Item2);

                    // add individual bookings/unavailabilities
                    foreach (Booking curBooking in todayBookings)
                        AddToDateRows(ref dateRows, startEndTime, curBooking.DateStart, curBooking.DateEnd);

                    // add recurring bookings/unavailabilities
                    foreach (Booking curBooking in todayRecurring)
                    {
                        if (curDate < curBooking.DateStart.Date || curDate > curBooking.DateEnd.Date)
                            continue;

                        DateTime bkStart = curDate.Add(curBooking.RecurringStartTime);
                        DateTime bkEnd = curDate.Add(curBooking.RecurringEndTime);
                        AddToDateRows(ref dateRows, startEndTime, bkStart, bkEnd);
                    }


                    dateRows = SortAndRemoveOverlaps(dateRows);


                    // list of all AVAILABLE times
                    List<Tuple<DateTime, DateTime>> availableTimeRows = GetAvailableTimes(startEndTime, dateRows);
                    //output1 += "<tr><td colspan=\"7\"><u>Available Times:</u><br />" + PrintList(availableTimeRows) + "</td></tr>";

                    // remove times when the space available is less than the default minutes set for the offering selected
                    for (int i = availableTimeRows.Count - 1; i >= 0; i--)
                        if (availableTimeRows[i].Item2.Subtract(availableTimeRows[i].Item1).TotalMinutes < selectedOffering.ServiceTimeMinutes)
                            availableTimeRows.RemoveAt(i);
                    //output1 += "<tr><td colspan=\"7\"><u>Available Times:</u><br />" + PrintList(availableTimeRows) + "</td></tr>";

                    if (availableTimeRows.Count > 0)
                        output1 += "<tr style=\"background:#ffffff !important;\">" + dateCols + "<td style=\"border-left:none !important;border-right:none !important;min-width:45px;\"></td><td style=\"white-space:nowrap;border-left:none !important;border-right:none !important;\"><u>Available Times</u><br />" + PrintList(availableTimeRows, orgs[0].OrganisationID, staff[0].StaffID) + "</td><td style=\"border-left:none !important;border-right:none !important;min-width:18px;\"></td><td style=\"border-left:none !important;\">" + bookingSheetLink + "</td></tr>";
                    else
                        output1 += "<tr>" + dateCols + "<td style=\"border-left:none !important;\" colspan=\"4\">Unavailable</td></tr>";
                }
                else
                {
                    output1 += "<tr>" + dateCols + "<td style=\"border-left:none !important;\" colspan=\"4\">Unavailable</td></tr>";
                }

            }
            output1 = output1 + "</table>"; ;


            /*
            string output = "<table class=\"table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center\">";
            foreach (Booking curBooking in bookings)
                output += "<tr><td>" + (curBooking.IsRecurring ? "R" : "") + "</td><td>" + curBooking.BookingID + "</td><td>" + curBooking.BookingTypeID + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Organisation == null ? "" : curBooking.Organisation.Name) + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Provider == null ? "" : curBooking.Provider.Person.FullnameWithoutMiddlename) + "</td><td style=\"white-space:nowrap;\">" + curBooking.DateStart.ToString("dd MMM yyyy") + "</td><td style=\"white-space:nowrap;\">" + "[" + curBooking.DateStart.ToString("HH:mm") + " - " + curBooking.DateEnd.ToString("HH:mm") + "]" + "</td></tr>";
            output = output + "</table>";
            */
        

            lblOutput.Text = output1;
        }
    }

    protected string[] GetProviderData(Site site, Offering selectedOffering, DateTime fromDate, DateTime endDate, Staff staff, Organisation org, RegisterStaff registerStaff, Booking[] bookings = null, bool incClinic = false)
    {
        int d = 0;

        string[] data = new string[(int)endDate.Subtract(fromDate).TotalDays + 2 + (incClinic ? 1 : 0)];
        if (incClinic) 
        { 
            data[d] = org.Name; 
            d++; 
        }
        data[d] = staff.Person.FullnameWithoutMiddlename;
        d++;

        data[d] = staff.Field.Descr;
        d++; 

        Organisation[] orgs      = new Organisation[] { org   };
        Staff[]        providers = new Staff[]        { staff };


        if (bookings == null) 
            bookings = BookingDB.GetBetween(fromDate, endDate, providers, orgs, null, null);


        // seperate into 
        // - individual bookings that can be retrieved by date
        // - recurring  bookings that can be retrieved by day of week
        Hashtable bkRecurringHash = new Hashtable();
        Hashtable bkDateHash      = new Hashtable();
        foreach (Booking curBooking in bookings)
        {
            if (curBooking.Provider != null && curBooking.Provider.StaffID != staff.StaffID)
                continue;
            //if (curBooking.Organisation != null && curBooking.Organisation.OrganisationID != orgs[0].OrganisationID)
            //    continue;

            if (!curBooking.IsRecurring)
            {
                if (bkDateHash[curBooking.DateStart.Date] == null)
                    bkDateHash[curBooking.DateStart.Date] = new ArrayList();
                if (IsWorkingToday(curBooking.DateStart.DayOfWeek, registerStaff))
                    ((ArrayList)bkDateHash[curBooking.DateStart.Date]).Add(curBooking);
            }
            else // curBooking.IsRecurring
            {
                if (bkRecurringHash[curBooking.RecurringDayOfWeek] == null)
                    bkRecurringHash[curBooking.RecurringDayOfWeek] = new ArrayList();
                if (IsWorkingToday(curBooking.RecurringDayOfWeek, registerStaff))
                    ((ArrayList)bkRecurringHash[curBooking.RecurringDayOfWeek]).Add(curBooking);
            }
        }


        for (DateTime curDate = fromDate; curDate < endDate; curDate = curDate.AddDays(1), d++)
        {
            Tuple<DateTime, DateTime> startEndTime = GetStartEndTime(curDate, orgs[0], site);
            Tuple<DateTime, DateTime> orgLunchTime = GetOrgLunchStartTime(curDate, orgs[0]);


            Booking[] todayBookings  = bkDateHash[curDate]                == null ? new Booking[] { } : (Booking[])((ArrayList)bkDateHash[curDate]).ToArray(typeof(Booking));
            Booking[] todayRecurring = bkRecurringHash[curDate.DayOfWeek] == null ? new Booking[] { } : (Booking[])((ArrayList)bkRecurringHash[curDate.DayOfWeek]).ToArray(typeof(Booking));


            if (IsWorkingToday(curDate.DayOfWeek, registerStaff) && IsClinicOpenToday(curDate, orgs[0]))
            {
                //foreach (Booking curBooking in todayBookings)
                //    output1 += "<tr><td></td><td>" + curBooking.BookingID + "</td><td>" + curBooking.BookingTypeID + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Organisation == null ? "" : curBooking.Organisation.Name) + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Provider == null ? "" : curBooking.Provider.Person.FullnameWithoutMiddlename) + "</td><td style=\"white-space:nowrap;\">" + curBooking.DateStart.ToString("dd MMM yyyy") + "</td><td style=\"white-space:nowrap;\">" + "[" + curBooking.DateStart.ToString("HH:mm") + " - " + curBooking.DateEnd.ToString("HH:mm") + "]" + "</td></tr>";
                //
                //foreach (Booking curBooking in todayRecurring)
                //    if (curDate >= curBooking.DateStart.Date && curDate <= curBooking.DateEnd.Date)
                //        output1 += "<tr><td> <b>R</b> </td><td>" + curBooking.BookingID + "</td><td>" + curBooking.BookingTypeID + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Organisation == null ? "" : curBooking.Organisation.Name) + "</td><td style=\"white-space:nowrap;\">" + (curBooking.Provider == null ? "" : curBooking.Provider.Person.FullnameWithoutMiddlename) + "</td><td style=\"white-space:nowrap;\">" + curBooking.DateStart.ToString("dd MMM yyyy") + "</td><td style=\"white-space:nowrap;\">" + "[" + new DateTime(curBooking.RecurringStartTime.Ticks).ToString("HH:mm") + " - " + new DateTime(curBooking.RecurringEndTime.Ticks).ToString("HH:mm") + "]" + "</td></tr>";


                // list of all TAKEN times
                List<Tuple<DateTime, DateTime>> dateRows = new List<Tuple<DateTime, DateTime>>();

                // add lunch breaks
                if (site.LunchStartTime < site.LunchEndTime)
                    AddToDateRows(ref dateRows, startEndTime, curDate.Add(site.LunchStartTime), curDate.Add(site.LunchEndTime));
                if (orgLunchTime.Item1 < orgLunchTime.Item2)
                    AddToDateRows(ref dateRows, startEndTime, orgLunchTime.Item1, orgLunchTime.Item2);

                // add individual bookings/unavailabilities
                foreach (Booking curBooking in todayBookings)
                    AddToDateRows(ref dateRows, startEndTime, curBooking.DateStart, curBooking.DateEnd);

                // add recurring bookings/unavailabilities
                foreach (Booking curBooking in todayRecurring)
                {
                    if ((curBooking.DateStart != DateTime.MinValue && curDate < curBooking.DateStart.Date) || (curBooking.DateEnd != DateTime.MinValue && curDate > curBooking.DateEnd.Date))
                        continue;

                    DateTime bkStart = curDate.Add(curBooking.RecurringStartTime);
                    DateTime bkEnd = curDate.Add(curBooking.RecurringEndTime);
                    AddToDateRows(ref dateRows, startEndTime, bkStart, bkEnd);
                }


                dateRows = SortAndRemoveOverlaps(dateRows);


                // list of all AVAILABLE times
                List<Tuple<DateTime, DateTime>> availableTimeRows = GetAvailableTimes(startEndTime, dateRows);
                //output1 += "<tr><td colspan=\"7\"><u>Available Times:</u><br />" + PrintList(availableTimeRows) + "</td></tr>";

                // remove times when the space available is less than the default minutes set for the offering selected
                for (int i = availableTimeRows.Count - 1; i >= 0; i--)
                    if (availableTimeRows[i].Item2.Subtract(availableTimeRows[i].Item1).TotalMinutes < selectedOffering.ServiceTimeMinutes)
                        availableTimeRows.RemoveAt(i);
                //output1 += "<tr><td colspan=\"7\"><u>Available Times:</u><br />" + PrintList(availableTimeRows) + "</td></tr>";

                if (availableTimeRows.Count > 0)
                {
                    string bookingSheetLink = IsLoggedIn() ?
                        "<a href= \"" + Booking.GetLink(curDate, new int[] { orgs[0].OrganisationID }) + "\"><img src=\"/images/Calendar-icon-24px.png\" alt=\"Go To Booking Sheet\" title=\"Go To Booking Sheet\"></a>" :
                        "<a href= \"/Account/CreateNewPatientV2.aspx?id=" + Request.QueryString["id"] + "&from_url=" + Server.UrlEncode(Booking.GetLink(curDate, new int[] { orgs[0].OrganisationID })) + "\"><img src=\"/images/Calendar-icon-24px.png\" alt=\"Go To Booking Sheet\" title=\"Go To Booking Sheet\"></a>";

                    data[d] = "<td style=\"background:#ffffff !important; border-left:none !important;border-right:none !important;min-width:45px;\"></td><td style=\"background:#ffffff !important; white-space:nowrap;border-left:none !important;border-right:none !important;\"><u>Available Times</u><br />" + PrintList(availableTimeRows, org.OrganisationID, staff.StaffID) + "</td><td style=\"background:#ffffff !important; border-left:none !important;border-right:none !important;min-width:18px;\"></td><td style=\"background:#ffffff !important; border-left:none !important;\">" + bookingSheetLink + "</td>";
                }
                else
                    data[d] = "<td style=\"border-left:none !important;\" colspan=\"4\">Unavailable</td>";
            }
            else
            {
                data[d] = "<td style=\"border-left:none !important;\" colspan=\"4\">Unavailable</td>";
            }

        }


        return data;
    }

    protected Booking[] GetBookingListThisOrgProv(Booking[] bookings, Staff staff, Organisation org)
    {
        ArrayList bookingListThisOrgProv = new ArrayList();
        for (int b = 0; b < bookings.Length; b++)
        {
            // dont include unavailable days for other orgs, so ... get where - normal booking (34) or (this org or blank org)
            bool realBookingOrValidUnavailability = bookings[b].BookingTypeID == 34 || bookings[b].Organisation == null || bookings[b].Organisation.OrganisationID == org.OrganisationID;
            bool thisProvider = bookings[b].Provider == null || bookings[b].Provider.StaffID == staff.StaffID;
            bool thisOrg = bookings[b].Organisation == null || bookings[b].Organisation.OrganisationID == org.OrganisationID;

            if (realBookingOrValidUnavailability && (thisProvider || thisOrg))
                bookingListThisOrgProv.Add(bookings[b]);
        }

        return (Booking[])bookingListThisOrgProv.ToArray(typeof(Booking));
    }

    protected bool IsWorkingToday(DayOfWeek dayOfWeek, RegisterStaff registerStaff)
    {
        return ((dayOfWeek == DayOfWeek.Sunday    && registerStaff != null && !registerStaff.ExclSun) ||
                (dayOfWeek == DayOfWeek.Monday    && registerStaff != null && !registerStaff.ExclMon) ||
                (dayOfWeek == DayOfWeek.Tuesday   && registerStaff != null && !registerStaff.ExclTue) ||
                (dayOfWeek == DayOfWeek.Wednesday && registerStaff != null && !registerStaff.ExclWed) ||
                (dayOfWeek == DayOfWeek.Thursday  && registerStaff != null && !registerStaff.ExclThu) ||
                (dayOfWeek == DayOfWeek.Friday    && registerStaff != null && !registerStaff.ExclFri) ||
                (dayOfWeek == DayOfWeek.Saturday  && registerStaff != null && !registerStaff.ExclSat));
    }
    protected bool IsClinicOpenToday(DateTime date, Organisation org)
    {
        return (org.StartDate == DateTime.MinValue || org.StartDate <= date) &&
               (org.EndDate   == DateTime.MinValue || org.EndDate   >= date) &&
               org.IsOpen(date.DayOfWeek);
    }

    protected void AddToDateRows(ref List<Tuple<DateTime, DateTime>> dateRows, Tuple<DateTime, DateTime> startEndTime, DateTime bkStart, DateTime bkEnd)
    {
        if (bkEnd <= startEndTime.Item1)
            return;
        if (bkStart >= startEndTime.Item2)
            return;

        DateTime start = bkStart >= startEndTime.Item1 ? bkStart : startEndTime.Item1;
        DateTime end   = bkEnd   <= startEndTime.Item2 ? bkEnd   : startEndTime.Item2;
        dateRows.Add(new Tuple<DateTime, DateTime>(start, end));
    }
    protected Tuple<DateTime, DateTime> GetStartEndTime(DateTime date, Organisation org, Site site)
    {
        Tuple<DateTime, DateTime> orgTimes = GetOrgStartEndTime(date, org);

        if (orgTimes.Item1 == orgTimes.Item2)
            return new Tuple<DateTime, DateTime>(
                date.Add(new TimeSpan(0, 0, 0)),
                date.Add(new TimeSpan(0, 0, 0))
            );

        DateTime siteStartTime = date.Add(site.DayStartTime);
        DateTime siteEndTime   = date.Add(site.DayEndTime);

        Tuple<DateTime, DateTime> ret = new Tuple<DateTime, DateTime>(
            orgTimes.Item1 > siteStartTime ? orgTimes.Item1 : siteStartTime,
            orgTimes.Item2 < siteEndTime   ? orgTimes.Item2 : siteEndTime
            );


        if (ret.Item2.Hour == 0 && ret.Item2.Minute == 0)
            return new Tuple<DateTime, DateTime>(
                ret.Item1,
                date.Add(new TimeSpan(23, 59, 59))
            );

        return ret;
    }
    protected Tuple<DateTime, DateTime> GetOrgStartEndTime(DateTime date, Organisation org)
    {
        if (date.DayOfWeek == DayOfWeek.Sunday)    return new Tuple<DateTime, DateTime>(date.Add(org.SunStartTime), date.Add(org.SunEndTime));
        if (date.DayOfWeek == DayOfWeek.Monday)    return new Tuple<DateTime, DateTime>(date.Add(org.MonStartTime), date.Add(org.MonEndTime));
        if (date.DayOfWeek == DayOfWeek.Tuesday)   return new Tuple<DateTime, DateTime>(date.Add(org.TueStartTime), date.Add(org.TueEndTime));
        if (date.DayOfWeek == DayOfWeek.Wednesday) return new Tuple<DateTime, DateTime>(date.Add(org.WedStartTime), date.Add(org.WedEndTime));
        if (date.DayOfWeek == DayOfWeek.Thursday)  return new Tuple<DateTime, DateTime>(date.Add(org.ThuStartTime), date.Add(org.ThuEndTime));
        if (date.DayOfWeek == DayOfWeek.Friday)    return new Tuple<DateTime, DateTime>(date.Add(org.FriStartTime), date.Add(org.FriEndTime));
        if (date.DayOfWeek == DayOfWeek.Saturday)  return new Tuple<DateTime, DateTime>(date.Add(org.SatStartTime), date.Add(org.SatEndTime));

        return new Tuple<DateTime, DateTime>(DateTime.MinValue, DateTime.MinValue);
    }
    protected Tuple<DateTime, DateTime> GetOrgLunchStartTime(DateTime date, Organisation org)
    {
        if (date.DayOfWeek == DayOfWeek.Sunday)    return new Tuple<DateTime, DateTime>(date.Add(org.SunLunchStartTime), date.Add(org.SunLunchEndTime));
        if (date.DayOfWeek == DayOfWeek.Monday)    return new Tuple<DateTime, DateTime>(date.Add(org.MonLunchStartTime), date.Add(org.MonLunchEndTime));
        if (date.DayOfWeek == DayOfWeek.Tuesday)   return new Tuple<DateTime, DateTime>(date.Add(org.TueLunchStartTime), date.Add(org.TueLunchEndTime));
        if (date.DayOfWeek == DayOfWeek.Wednesday) return new Tuple<DateTime, DateTime>(date.Add(org.WedLunchStartTime), date.Add(org.WedLunchEndTime));
        if (date.DayOfWeek == DayOfWeek.Thursday)  return new Tuple<DateTime, DateTime>(date.Add(org.ThuLunchStartTime), date.Add(org.ThuLunchEndTime));
        if (date.DayOfWeek == DayOfWeek.Friday)    return new Tuple<DateTime, DateTime>(date.Add(org.FriLunchStartTime), date.Add(org.FriLunchEndTime));
        if (date.DayOfWeek == DayOfWeek.Saturday)  return new Tuple<DateTime, DateTime>(date.Add(org.SatLunchStartTime), date.Add(org.SatLunchEndTime));

        return new Tuple<DateTime, DateTime>(DateTime.MinValue, DateTime.MinValue);
    }

    protected List<Tuple<DateTime, DateTime>> SortAndRemoveOverlaps(List<Tuple<DateTime, DateTime>> dateRows)
    {
        //sorting by start time; you can do this in SQL pretty easily
        //necessary to make sure the row most likely to overlap a given row is the next one
        dateRows.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        for (int i = 0; i < dateRows.Count - 1; i++)
        {
            for (int j = i + 1; j < dateRows.Count; j++)
            {
                if (dateRows[i].Item1 <= dateRows[j].Item2 && dateRows[i].Item2 >= dateRows[j].Item1) //overlap
                {
                    //keep date row i, with the values of the complete time range of i and j
                    dateRows[i] = new Tuple<DateTime, DateTime>(
                        dateRows[i].Item1 < dateRows[j].Item1 ? dateRows[i].Item1 : dateRows[j].Item1,
                        dateRows[i].Item2 > dateRows[j].Item2 ? dateRows[i].Item2 : dateRows[j].Item2
                        );

                    //remove row j and ensure we don't skip the row after it
                    dateRows.RemoveAt(j);
                    j--;
                }
            }
        }

        return dateRows;
    }
    protected string TestRemoveOverlaps(int orgID, int staffID)
    {
        /*
         10:00-----------10:30
         10:10---10:20
                 10:20----------10:40
                                       10:50---11:10
                                                           4--5
                                                      3:30-----6:00
         10:00 - 10:30
         10:10 - 10:20
         10:20 - 10:40
         10:50 - 11:10
         16:00 - 17:00
         15:30 - 18:00

         10:00 - 10:40
         10:50 - 11:10
         15:30 - 18:00
        */
        List<Tuple<DateTime, DateTime>> dateRows = new List<Tuple<DateTime, DateTime>>();
        dateRows.Add(new Tuple<DateTime, DateTime>(new DateTime(2000, 1, 1, 10, 0, 0), new DateTime(2000, 1, 1, 10, 30, 0)));
        dateRows.Add(new Tuple<DateTime, DateTime>(new DateTime(2000, 1, 1, 10, 10, 0), new DateTime(2000, 1, 1, 10, 20, 0)));
        dateRows.Add(new Tuple<DateTime, DateTime>(new DateTime(2000, 1, 1, 10, 20, 0), new DateTime(2000, 1, 1, 10, 40, 0)));
        dateRows.Add(new Tuple<DateTime, DateTime>(new DateTime(2000, 1, 1, 10, 50, 0), new DateTime(2000, 1, 1, 11, 10, 0)));
        dateRows.Add(new Tuple<DateTime, DateTime>(new DateTime(2000, 1, 1, 16, 0, 0), new DateTime(2000, 1, 1, 17, 0, 0)));
        dateRows.Add(new Tuple<DateTime, DateTime>(new DateTime(2000, 1, 1, 15, 30, 0), new DateTime(2000, 1, 1, 18, 00, 0)));

        string output = string.Empty;
        output += PrintList(dateRows, orgID, staffID);
        output += "<br /><br/>";
        dateRows = SortAndRemoveOverlaps(dateRows);
        output += PrintList(dateRows, orgID, staffID);

        return output;
    }

    protected List<Tuple<DateTime, DateTime>> GetAvailableTimes(Tuple<DateTime, DateTime> startEndTime, List<Tuple<DateTime, DateTime>> dateRows)
    {
        List<Tuple<DateTime, DateTime>> availableTimeRows = new List<Tuple<DateTime, DateTime>>();

        if (startEndTime.Item1 >= startEndTime.Item2)
            return availableTimeRows;

        if (dateRows.Count == 0)
        {
            availableTimeRows.Add(new Tuple<DateTime, DateTime>(startEndTime.Item1, startEndTime.Item2));
            return availableTimeRows;
        }

        if (dateRows[0].Item1 != startEndTime.Item1)
            availableTimeRows.Add(new Tuple<DateTime, DateTime>(startEndTime.Item1, dateRows[0].Item1));

        for (int i = 0; i < dateRows.Count-1; i++)
            availableTimeRows.Add(new Tuple<DateTime, DateTime>(dateRows[i].Item2, dateRows[i+1].Item1));

        if (dateRows[dateRows.Count-1].Item2 != startEndTime.Item2)
            availableTimeRows.Add(new Tuple<DateTime, DateTime>(dateRows[dateRows.Count - 1].Item2, startEndTime.Item2));

        return availableTimeRows;
    }

    protected string PrintList(List<Tuple<DateTime, DateTime>> dateRows, int orgID, int staffID)
    {
        string output = string.Empty;
        for (int i = 0; i < dateRows.Count; i++)
        {
            string text = dateRows[i].Item1.ToString("h:mm") + (dateRows[i].Item1.Hour < 12 ? "am" : "pm") + " - " + dateRows[i].Item2.ToString("h:mm") + (dateRows[i].Item2.Hour < 12 ? "am" : "pm");

            Tuple<int, int, DateTime> scrollTocell = new Tuple<int, int, DateTime>(orgID, staffID, dateRows[i].Item1);
            string bookingSheetLink = IsLoggedIn() ?
                "<a href= \"" + Booking.GetLink(dateRows[i].Item1.Date, new int[] { orgID }, -1, -1, scrollTocell) + "\">" + text + "</a>" :
                "<a href= \"/Account/CreateNewPatientV2.aspx?id=" + Request.QueryString["id"] + "&from_url=" + Server.UrlEncode(Booking.GetLink(dateRows[i].Item1.Date, new int[] { orgID }, -1, -1, scrollTocell)) + "\">" + text + "</a>";

            output += (i == 0 ? "" : "<br />") + bookingSheetLink;
        }
        if (dateRows.Count == 0)
            output = "None";
        return output;
    }

    #endregion
    
    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        lblOutput.Text = string.Empty;
        user_login_form.Visible = false;
        SetErrorMessage(errMsg, details);
    }
    private void SetErrorMessage(string errMsg = "", string details = "")
    {
        if (errMsg.Contains(Environment.NewLine))
            errMsg = errMsg.Replace(Environment.NewLine, "<br />");

        // double escape so shows up literally on webpage for 'alert' message
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"#\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

        lblErrorMessage.Visible = true;
        if (errMsg != null && errMsg.Length > 0)
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}