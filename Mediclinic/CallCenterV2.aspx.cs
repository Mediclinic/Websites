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
using System.Text;

public partial class CallCenterV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = string.Empty;
            lblEmailErrorMessage.Text = string.Empty;

            // need to be admin+ to see this page
            PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);

            // needs to be call center db to see this page
            if (((SystemVariables)Session["SystemVariables"])["IsMediclinicCallCenter"].Value != "1")
                Response.Redirect(PagePermissions.UnauthorisedAccessPageForward());


            if (!IsPostBack)
            {
                lblInfo1.Text = string.Empty;

                // Post
                if (Request.Form["db"] != null && Request.Form["org"] != null && Request.Form["site"] != null && Request.Form["patient"] != null)
                {
                    GoTo(Request.Form["db"], Convert.ToInt32(Request.Form["org"]), Convert.ToInt32(Request.Form["site"]), Convert.ToInt32(Request.Form["patient"]));
                    return;
                }

                // Get (don't use this...
                //if (Request.QueryString["db"] != null && Request.QueryString["org"] != null)
                //    GoTo(Request.QueryString["db"], Convert.ToInt32(Request.QueryString["org"]));

                DisplayInfo();

                FreeTextBox1.Text = @"<center>
<font color='#2E2E2E'>
<h1>Newsletter</h1>
<img src='https://portal.mediclinic.com.au/imagesV2/comp_logo.png'><br>
<br>
<div style=""display: inline-block;min-width:450px;text-align:left;"">

        <br>
        <br>
        <br>
        Best regards,<br>
        Mediclinic<br>
        <a href='http://www.mediclinic.com.au'>http://www.mediclinic.com.au</a><br>
        <br>

</div>
</font>
</center>

";

                ddlDOB_Day.Items.Add(new ListItem("--", "-1"));
                ddlDOB_Month.Items.Add(new ListItem("--", "-1"));
                ddlDOB_Year.Items.Add(new ListItem("----", "-1"));

                for (int i = 1; i <= 31; i++)
                    ddlDOB_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 1; i <= 12; i++)
                    ddlDOB_Month.Items.Add(new ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).Substring(0, 3), i.ToString()));
                for (int i = 1915; i <= DateTime.Today.Year; i++)
                    ddlDOB_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                txtPhoneNbrSearch.Focus();
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
    }

    #endregion

    #region GoTo(db, orgID, siteID, patientID)

    protected void GoTo(string db, int orgID, int siteID, int patientID)
    {
        //lblInfo1.Text += "GET<br/>DB: " + db + "<br />Org: " + orgID;

        bool isExternalStaff = orgID != 0 && patientID == -1;
        bool isStakeHolder   = !isExternalStaff && Convert.ToBoolean(Session["IsStakeholder"]);

        if (db != Session["DB"].ToString())
        {
            // Set previous StaffID, pervious DB
            Session["PreviousStaffID"] = Session["StaffID"];
            Session["PreviousDB"]      = Session["DB"];
            Session["PreviousSiteID"]  = Session["SiteID"];

            // Change to new DB & SystemVariables
            Session["DB"] = db;
            Session["SystemVariables"] = SystemVariableDB.GetAll();

            // Set system staff variables
            int callCentreStaffID = isExternalStaff ? -5 : (!isStakeHolder ? -7 : -8);
            Staff staff = StaffDB.GetByID(callCentreStaffID);
            Session["IsLoggedIn"]                     = true;
            Session["IsStakeholder"]                  = staff.IsStakeholder;
            Session["IsMasterAdmin"]                  = staff.IsMasterAdmin;
            Session["IsAdmin"]                        = staff.IsAdmin;
            Session["IsPrincipal"]                    = staff.IsPrincipal;
            Session["IsProvider"]                     = staff.IsProvider;
            Session["IsExternal"]                     = staff.IsExternal;
            Session["StaffID"]                        = staff.StaffID;
            Session["StaffFullnameWithoutMiddlename"] = staff.Person.FullnameWithoutMiddlename;
            Session["StaffFirstname"]                 = staff.Person.Firstname;

            // Set OrgID in session as external user has OrgID set
            if (isExternalStaff)
            {
                Organisation org = OrganisationDB.GetByID(orgID);
                Session["OrgID"]   = orgID;
                Session["OrgName"] = org.Name;
            }


            Site site = null;
            if (siteID != 0)
                site = SiteDB.GetByID(siteID);
            else
            {
                // log in to same site type if possible, else just log in to site 1
                site = SiteDB.GetSiteByType((SiteDB.SiteType)Convert.ToInt32(Session["SiteTypeID"]), null, db);
                if (site == null)
                    site = SiteDB.GetByID(1);
            }

            Session["SiteID"]         = site.SiteID;
            Session["SiteName"]       = site.Name;
            Session["SiteIsClinic"]   = site.SiteType.ID == 1;
            Session["SiteIsAgedCare"] = site.SiteType.ID == 2;
            Session["SiteIsGP"]       = site.SiteType.ID == 3;
            Session["SiteTypeID"]     = site.SiteType.ID;
            Session["SiteTypeDescr"]  = site.SiteType.Descr;

            Session["IsMultipleSites"] = SiteDB.GetAll().Length > 1;



            // Remove patient list session data for pt searches
            Session.Remove("patientinfo_data");
            Session.Remove("patientlist_data");
            Session.Remove("patientlist_sortexpression");
            Session.Remove("patientinfo_sortexpression");
        }


        // Go to booking page with this org

        if (isExternalStaff)
            Response.Redirect("~/BookingsV2.aspx?orgs=" + Session["OrgID"] + "&ndays=4", false);

        else if (patientID != -1 && orgID != 0)
            Response.Redirect("~/BookingsV2.aspx?orgs=" + orgID + "&patient=" + patientID + "&ndays=4", false);
        else if (patientID != -1 && orgID == 0)
            Response.Redirect("~/PatientDetailV2.aspx?type=view&id=" + patientID, false);
        
        else
            Response.Redirect("~/Default.aspx", false);
        return;
    }

    #endregion

    #region DisplayInfo

    protected void DisplayInfo()
    {
        string curDbName = Session["DB"].ToString();

        ddlDBs.Items.Clear();
        ddlDBs.Items.Add(new ListItem("All Clients", "0"));

        ddlDBs2.Items.Clear();
        ddlDBs2.Items.Add(new ListItem("All Clients", "0"));

        ArrayList dbNames = new ArrayList();
        Hashtable dbHash = new Hashtable();

        try
        {
            bool isSupportStaff3 = Session != null && Session["StaffID"] != null && Convert.ToInt32(Session["StaffID"]) == -4;

            List<Tuple<string, string>> list = new List<Tuple<string, string>>();


            System.Data.DataTable tbl = DBBase.ExecuteQuery("EXEC sp_databases;", "master").Tables[0];
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                string databaseName = tbl.Rows[i][0].ToString();

                if (!Regex.IsMatch(databaseName, @"Mediclinic_\d{4}"))
                    continue;
                //if (databaseName == "Mediclinic_0001")
                //    continue;


                SystemVariables sysVariables = SystemVariableDB.GetAll(databaseName);

                dbNames.Add(sysVariables["Site"].Value);
                dbHash[sysVariables["Site"].Value] = databaseName;


                System.Text.StringBuilder output = new System.Text.StringBuilder();

                Session["DB"] = databaseName;
                Session["SystemVariables"] = SystemVariableDB.GetAll();

                string callCenterPrefix = ((SystemVariables)Session["SystemVariables"])["CallCenterPrefix"].Value;

                int loginsPastWeek_Total = UserLoginDB.GetCount(7);
                int loginsPastWeek_Staff = UserLoginDB.GetStaffCount(7);


                if (((SystemVariables)Session["SystemVariables"])["UseMediclinicCallCenter"].Value == "1")
                {
                    Organisation[] orgs = OrganisationDB.GetAll(false, true, false, false, true, true);


                    Site[] sites = SiteDB.GetAll();
                    int clinicSiteID = -1;
                    int agedCareSiteID = -1;
                    for (int j = 0; j < sites.Length; j++)
                    {
                        if (sites[j].SiteType.ID == 1)
                            clinicSiteID = sites[j].SiteID;
                        if (sites[j].SiteType.ID == 2)
                            agedCareSiteID = sites[j].SiteID;
                    }


                    string showHideID1   = "heading_" + databaseName;
                    string showHideLink1 = @"<a href=""javascript:void(0)"" onclick=""hide_show_all('" + showHideID1 + @"');return false;"">Show/Hide Clinics/Facs</a>";

                    string siteTitleLink        = @"<span style=""display:inline-block;vertical-align:middle;max-width:300px;min-width:300px;overflow:hidden;text-overflow:ellipsis;""><a style=""white-space:nowrap;"" class=""call_centre_header"" title=""" + ((SystemVariables)Session["SystemVariables"])["Site"].Value + @""" href=""" + HttpContext.Current.Request.Url.AbsolutePath + "?db=" + databaseName + @""" onclick=""http_post('" + databaseName + @"','0','0',-1);return false;"">" + ((SystemVariables)Session["SystemVariables"])["Site"].Value + @"</a></span>";
                    //string siteTitleLink      = @"<span style=""display:inline-block;min-width:250px;""><a class=""call_centre_header"" title=""" + ((SystemVariables)Session["SystemVariables"])["Site"].Value + @""" href=""" + HttpContext.Current.Request.Url.AbsolutePath + "?db=" + databaseName + @""" onclick=""http_post('" + databaseName + @"','0','0',-1);return false;"">" + ((SystemVariables)Session["SystemVariables"])["Site"].Value + @"</a></span>";
                    string callCentrePrefixText = @"<span style=""display:inline-block;min-width:160px;"">[Call Center Prefix: " + callCenterPrefix + "]</span>";

                    output.AppendLine("<tr>");
                    //output.AppendLine("    <td><u>" + ((SystemVariables)Session["SystemVariables"])["Site"].Value + "</u> &nbsp;&nbsp; [Call Center Prefix: " + callCenterPrefix + "] &nbsp;&nbsp; [" + Session["DB"] + "]</td>");
                    output.AppendLine("    <td>" + showHideLink1 + " &nbsp;&nbsp; " + (isSupportStaff3 ? @"<span style=""display:inline-block;min-width:55px;"">" + (loginsPastWeek_Staff == 0 && loginsPastWeek_Total == 0 ? "" : "[" + loginsPastWeek_Staff + "," + loginsPastWeek_Total + "]") + "</span>" + " &nbsp;&nbsp; " : "") + siteTitleLink + " &nbsp;&nbsp; " + callCentrePrefixText + " &nbsp;&nbsp; [" + Session["DB"] + "]</td>");
                    output.AppendLine("</tr>");
                
                    if (orgs.Length > 0)
                    {
                        for (int j = 0; j < orgs.Length; j++)
                        {
                            string showHideID = databaseName + "_" + orgs[j].OrganisationID;
                            string contactInfo = GetContactInfo(orgs[j].EntityID, 60, showHideID);

                            string titleLink   = @"<a href=""" + HttpContext.Current.Request.Url.AbsolutePath + "?db=" + databaseName + @"&org=" + orgs[j].OrganisationID + @""" onclick=""http_post('" + databaseName + "','" + orgs[j].OrganisationID + @"','" + (orgs[j].IsClinic ? clinicSiteID : agedCareSiteID) + @"',-1);return false;"">" + orgs[j].Name + " " + (orgs[j].IsClinic ? @"(Clinic)" : "(Aged Care)") + @"</a>";
                            string showHideLink = @"<a href=""javascript:void(0)"" onclick=""hide_show('" + showHideID + @"');return false;"">Show/Hide Details</a>";


                            output.AppendLine(@"<tr id=""" + showHideID1 + j.ToString() + @""" style=""margin:0 0;display:none;"">");
                            output.AppendLine(@"    <td style=""margin:0 0;"">");
                            output.AppendLine(@"            <table><tr style=""vertical-align:top;""><td style=""width:500px;""><ul style=""margin:0 0;""><li>" + titleLink + @"</li></ul></td><td style=""min-width:10px;""></td><td>" + (contactInfo.Length > 0 ? showHideLink : "") + "</td></tr></table>");

                            if (contactInfo.Length > 0)
                                output.AppendLine(contactInfo);

                            output.AppendLine(@"    </td>");
                            output.AppendLine(@"</tr>");
                        }
                    }
                }

                list.Add(new Tuple<string, string>(((SystemVariables)Session["SystemVariables"])["Site"].Value, output.ToString()));

                Session.Remove("DB");
                Session.Remove("SystemVariables");
            }


            dbNames.Sort();
            foreach (string siteName in dbNames)
            {
                ddlDBs.Items.Add(new ListItem("[" + dbHash[siteName] + "] " + siteName, dbHash[siteName].ToString()));
                ddlDBs2.Items.Add(new ListItem("[" + dbHash[siteName] + "] " + siteName, dbHash[siteName].ToString()));
            }


            list.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            System.Text.StringBuilder finalOutput = new System.Text.StringBuilder();
            foreach (Tuple<string, string> item in list)
                finalOutput.Append(item.Item2);

            lblInfo1.Text = "<table>" + finalOutput.ToString() + "</table>";
        }
        finally
        {
            Session["DB"] = curDbName;
            Session["SystemVariables"] = SystemVariableDB.GetAll();
        }

    }

    protected string GetContactInfo(int EntityID, int indentPX, string showHideID)
    {
        string output = string.Empty;

        DataTable dt = ContactAusDB.GetDataTable_ByEntityID(-1, EntityID);

        output += GetContactDetail(dt.Select("atg_contact_type_group_id=1"));
        output += GetContactDetail(dt.Select("atg_contact_type_group_id=2"));
        output += GetContactDetail(dt.Select("atg_contact_type_group_id=4"));

        return output == string.Empty ? "" : @"<table id='" + showHideID + @"' style=""margin-left:" + indentPX + @"px; display:none;"">" + output + "</table>";
    }
    protected string GetContactDetail(DataRow[] rows)
    {
        string output = string.Empty;

        for (int i = 0; i < rows.Length; i++)
        {
            ContactAus contact = ContactAusDB.LoadAll(rows[i]);

            bool isAddress = contact.ContactType.ContactTypeGroup.ID == 1;
            bool isPhone   = contact.ContactType.ContactTypeGroup.ID == 2;
            bool isBedroom = contact.ContactType.ContactTypeGroup.ID == 3;

            bool isEmail   = contact.ContactType.ContactTypeID == 27;
            bool isWebsite = contact.ContactType.ContactTypeID == 28;


            if (isAddress)
            {
                output += @"<tr><td style=""min-width:140px"">" + contact.ContactType.Descr + @"</td><td style=""min-width:10px;""></td><td>" + contact.GetFormattedAddress() + "<br>" + "</td></tr>";
            }
            else if (isPhone)
            {
                output += @"<tr><td style=""min-width:140px"">" + contact.ContactType.Descr + @"</td><td style=""min-width:10px;""></td><td>" + contact.GetFormattedPhoneNumber() + "</td></tr>";
            }
            else if (isEmail)
            {
                output += @"<tr><td style=""min-width:140px"">" + contact.ContactType.Descr + @"</td><td style=""min-width:10px;""></td><td>" + contact.AddrLine1 + "</td></tr>";
            }
        }

        return output;
    }

    #endregion

    #region btnExport_Click

    protected void btnExportStaff_Click(object sender, EventArgs e)
    {
        ExportAllUsers( ddlDBs.SelectedValue == "0" ? null : ddlDBs.SelectedValue );
    }
    protected void ExportAllUsers(string DB = null)
    {
        string curDbName = Session["DB"].ToString();

        try
        {
            DataTable tblAllStaff = null;

            System.Data.DataTable tbl = DBBase.ExecuteQuery("EXEC sp_databases;", "master").Tables[0];
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                string databaseName = tbl.Rows[i][0].ToString();

                if (!Regex.IsMatch(databaseName, @"Mediclinic_\d{4}"))
                    continue;

                if (DB != null && databaseName != DB)
                    continue;

                Session["DB"] = databaseName;
                Session["SystemVariables"] = SystemVariableDB.GetAll();


                DataTable tblStaff = StaffDB.GetDataTable();


                // get entity ID's so we can get all emails into a hashtable in one db query
                int[] entityIDs = new int[tblStaff.Rows.Count];
                for (int j = 0; j < tblStaff.Rows.Count; j++)
                    entityIDs[j] = Convert.ToInt32(tblStaff.Rows[j]["person_entity_id"]);
                Hashtable emailHash = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);

                // add the emails to the datatable - as comma-seperated string
                tblStaff.Columns.Add("database_name", typeof(String));
                tblStaff.Columns.Add("emails", typeof(String));
                tblStaff.Columns.Add("site", typeof(String));
                for (int j = 0; j < tblStaff.Rows.Count; j++)
                {
                    Staff s = StaffDB.LoadAll(tblStaff.Rows[j]);

                    string emails = string.Empty;
                    if (emailHash[s.Person.EntityID] != null)
                    {
                        if (Utilities.GetAddressType().ToString() == "Contact")
                        {
                            if (emailHash[s.Person.EntityID] != null)
                                foreach (Contact c in (Contact[])emailHash[s.Person.EntityID])
                                    if (c.AddrLine1.Trim().Length > 0 && Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                                        emails += (emails.Length == 0 ? "" : ",") + c.AddrLine1.Trim();
                        }
                        else if (Utilities.GetAddressType().ToString() == "ContactAus")
                        {
                            if (emailHash[s.Person.EntityID] != null)
                                foreach (ContactAus c in (ContactAus[])emailHash[s.Person.EntityID])
                                    if (c.AddrLine1.Trim().Length > 0 && Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                                        emails += (emails.Length == 0 ? "" : ",") + c.AddrLine1.Trim();
                        }
                        else
                            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
                    }

                    tblStaff.Rows[j]["database_name"] = databaseName;
                    tblStaff.Rows[j]["emails"]        = emails;
                    tblStaff.Rows[j]["site"]          = ((SystemVariables)Session["SystemVariables"])["Site"].Value;
                }


                // if first table, set alldb's to this, else add this to alldb's list
                if (tblAllStaff == null)
                    tblAllStaff = tblStaff;
                else
                    tblAllStaff.Merge(tblStaff);

                Session.Remove("DB");
                Session.Remove("SystemVariables");
            }


            // create output 

            System.Text.StringBuilder htmlOoutput   = new System.Text.StringBuilder();
            System.Text.StringBuilder exportOoutput = new System.Text.StringBuilder();

            htmlOoutput.Append("<table border=\"1\">");

            htmlOoutput.Append("<tr>");
            htmlOoutput.Append("<th>Database</td>");
            htmlOoutput.Append("<th>Site Name</td>");
            htmlOoutput.Append("<th>Firstname</td>");
            htmlOoutput.Append("<th>Surname</td>");
            htmlOoutput.Append("<th>Fullname</td>");
            htmlOoutput.Append("<th>Stakeholder</td>");
            htmlOoutput.Append("<th>Master Admin</td>");
            htmlOoutput.Append("<th>Admin</td>");
            htmlOoutput.Append("<th>Principal</td>");
            htmlOoutput.Append("<th>Provider</td>");
            htmlOoutput.Append("<th>Email(s)</td>");
            htmlOoutput.Append("</tr>");

            exportOoutput.Append("Database").Append(",");
            exportOoutput.Append("Site Name").Append(",");
            exportOoutput.Append("Firstname").Append(",");
            exportOoutput.Append("Surname").Append(",");
            exportOoutput.Append("Fullname").Append(",");
            exportOoutput.Append("Stakeholder").Append(",");
            exportOoutput.Append("Master Admin").Append(",");
            exportOoutput.Append("Admin").Append(",");
            exportOoutput.Append("Principal").Append(",");
            exportOoutput.Append("Provider").Append(",");
            exportOoutput.Append("Email(s)").Append(",");
            exportOoutput.AppendLine();

            if (tblAllStaff != null)
            {
                for (int i = 0; i < tblAllStaff.Rows.Count; i++)
                {
                    Staff s = StaffDB.LoadAll(tblAllStaff.Rows[i]);

                    htmlOoutput.Append("<tr>");
                    htmlOoutput.Append("<td>" + tblAllStaff.Rows[i]["database_name"] + "</td>");
                    htmlOoutput.Append("<td>" + tblAllStaff.Rows[i]["site"] + "</td>");
                    htmlOoutput.Append("<td>" + s.Person.Firstname + "</td>");
                    htmlOoutput.Append("<td>" + s.Person.Surname + "</td>");
                    htmlOoutput.Append("<td>" + s.Person.Fullname + "</td>");
                    htmlOoutput.Append("<td>" + (s.IsStakeholder ? "Yes" : "No") + "</td>");
                    htmlOoutput.Append("<td>" + (s.IsMasterAdmin ? "Yes" : "No") + "</td>");
                    htmlOoutput.Append("<td>" + (s.IsAdmin       ? "Yes" : "No") + "</td>");
                    htmlOoutput.Append("<td>" + (s.IsPrincipal   ? "Yes" : "No") + "</td>");
                    htmlOoutput.Append("<td>" + (s.IsProvider    ? "Yes" : "No") + "</td>");
                    htmlOoutput.Append("<td>" + tblAllStaff.Rows[i]["emails"] + "</td>");
                    htmlOoutput.Append("</tr>");

                    exportOoutput.Append(tblAllStaff.Rows[i]["database_name"]).Append(",");
                    exportOoutput.Append(tblAllStaff.Rows[i]["site"]).Append(",");
                    exportOoutput.Append(s.Person.Firstname).Append(",");
                    exportOoutput.Append(s.Person.Surname).Append(",");
                    exportOoutput.Append(s.Person.Fullname).Append(",");
                    exportOoutput.Append(s.IsStakeholder ? "Yes" : "No").Append(",");
                    exportOoutput.Append(s.IsMasterAdmin ? "Yes" : "No").Append(",");
                    exportOoutput.Append(s.IsAdmin       ? "Yes" : "No").Append(",");
                    exportOoutput.Append(s.IsPrincipal   ? "Yes" : "No").Append(",");
                    exportOoutput.Append(s.IsProvider    ? "Yes" : "No").Append(",");
                    exportOoutput.Append(tblAllStaff.Rows[i]["emails"]).Append(",");
                    exportOoutput.AppendLine();
                }
            }

            htmlOoutput.Append("</table>");

            // send the output

            //lblResultMessage2.Text = htmlOoutput.ToString();
            ExportCSV(Response, exportOoutput.ToString(), "All Users All Sites.csv");

        }
        finally
        {
            Session["DB"] = curDbName;
            Session["SystemVariables"] = SystemVariableDB.GetAll();
        }

    }
    protected static void ExportCSV(HttpResponse response, string fileText, string fileName)
    {
        byte[] buffer = GetBytes(fileText);

        try
        {
            response.Clear();
            response.ContentType = "text/plain";
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            response.End();
        }
        catch (System.Web.HttpException ex)
        {
            // ignore exception where user closed the download box
            if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                throw;
        }
    }
    protected static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
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


    protected void btnSendEmail_Click(object sender, EventArgs e)
    {
        EmailAllUsers(false, ddlDBs2.SelectedValue == "0" ? null : ddlDBs2.SelectedValue);
    }
    protected void btnPreviewEmail_Click(object sender, EventArgs e)
    {
        EmailAllUsers(true, ddlDBs2.SelectedValue == "0" ? null : ddlDBs2.SelectedValue);
    }
    protected void EmailAllUsers(bool previewOnly, string DB = null)
    {
        bool IsDebug = Utilities.IsDev();


        string curDbName = Session["DB"].ToString();

        try
        {
            txtSubject.Text = txtSubject.Text.Trim();
            if (txtSubject.Text.Length == 0)
            {
                lblEmailErrorMessage.Text = "<br />No Subject Entered<br /><br />";
                return;
            }
            if (FreeTextBox1.Text.Trim().Length == 0)
            {
                lblEmailErrorMessage.Text = "<br />No Email Text Entered<br /><br />";
                return;
            }


            string fromEmail = IsDebug ? "eli@elipollak.com" : ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value;
            string fromName  = ((SystemVariables)Session["SystemVariables"])["Email_FromName"].Value;


            DataTable tblAllStaff = null;

            ArrayList emailInfoList = new ArrayList(); // list of Tuple<site, person, email>

            System.Data.DataTable tbl = DBBase.ExecuteQuery("EXEC sp_databases;", "master").Tables[0];
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                string databaseName = tbl.Rows[i][0].ToString();

                if (!Regex.IsMatch(databaseName, @"Mediclinic_\d{4}"))
                    continue;

                if (chkIgnore0001.Checked && databaseName == @"Mediclinic_0001")
                    continue;

                if (DB != null && databaseName != DB)
                    continue;

                Session["DB"] = databaseName;
                Session["SystemVariables"] = SystemVariableDB.GetAll();

                string clientSiteName = ((SystemVariables)Session["SystemVariables"])["Site"].Value;

                DataTable tblStaff = StaffDB.GetDataTable();


                // get entity ID's so we can get all emails into a hashtable in one db query
                int[] entityIDs = new int[tblStaff.Rows.Count];
                for (int j = 0; j < tblStaff.Rows.Count; j++)
                    entityIDs[j] = Convert.ToInt32(tblStaff.Rows[j]["person_entity_id"]);
                Hashtable emailHash = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);

                // add the emails to the datatable - as comma-seperated string
                tblStaff.Columns.Add("database_name", typeof(String));
                tblStaff.Columns.Add("emails", typeof(String));
                tblStaff.Columns.Add("site", typeof(String));
                for (int j = 0; j < tblStaff.Rows.Count; j++)
                {
                    Staff s = StaffDB.LoadAll(tblStaff.Rows[j]);

                    if (chkMasterAdminOnly.Checked && !s.IsMasterAdmin)
                        continue;

                    if (emailHash[s.Person.EntityID] != null)
                    {
                        if (Utilities.GetAddressType().ToString() == "Contact")
                        {
                            if (emailHash[s.Person.EntityID] != null)
                                foreach (Contact c in (Contact[])emailHash[s.Person.EntityID])
                                    if (Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                                        emailInfoList.Add(new Tuple<string, string, string>(clientSiteName, s.Person.FullnameWithoutMiddlename, c.AddrLine1.Trim()));
                        }
                        else if (Utilities.GetAddressType().ToString() == "ContactAus")
                        {
                            if (emailHash[s.Person.EntityID] != null)
                                foreach (ContactAus c in (ContactAus[])emailHash[s.Person.EntityID])
                                    if (Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                                        emailInfoList.Add(new Tuple<string, string, string>(clientSiteName, s.Person.FullnameWithoutMiddlename, c.AddrLine1.Trim()));
                        }
                        else
                            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
                    }
                }


                // if first table, set alldb's to this, else add this to alldb's list
                if (tblAllStaff == null)
                    tblAllStaff = tblStaff;
                else
                    tblAllStaff.Merge(tblStaff);

                Session.Remove("DB");
                Session.Remove("SystemVariables");
            }


            // send the email

            string output = "<h4>" + (previewOnly ? "Message Preview" : "Message Sent") + "</h4>" + Environment.NewLine +

                "<table style=\"min-width:400px;border:1px solid black;text-align:center;background-color:#FFFFFF;border-spacing:2px;border-collapse:separate;\"><tr><td><b>" + txtSubject.Text + "</b></td></tr></table><div style=\"height:10px;\"></div>" + Environment.NewLine +

                
                
                "<table style=\"min-width:500px;border:1px solid black;text-align:left;background-color:#FFFFFF;border-spacing:10px;border-collapse:separate;\"><tr style=\"min-height:200px;\"><td>" + (FreeTextBox1.Text.Length == 0 ? "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" : FreeTextBox1.Text) + "</td></tr></table><br />";

            output += "<h4>" + (previewOnly ? "Will Be Sent To" : "Sent To") + "</h4><table border=\"1\" class=\"table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center\" >";
            string emails = string.Empty;

            foreach (Tuple<string, string, string> emailInfo in emailInfoList)
            {
                output += "<tr><td style=\"padding-left:4px;padding-right:4px;text-align:left !important;\">" + emailInfo.Item1 + "</td><td style=\"padding-left:4px;padding-right:4px;text-align:left !important;\">" + emailInfo.Item2 + "</td><td style=\"padding-left:4px;padding-right:4px;text-align:left !important;\">" + emailInfo.Item3 + "</td></tr>";
                emails = (emails.Length == 0 ? "" : ",") + emailInfo.Item3; 
            }
            if (emailInfoList.Count == 0)
                output += "<tr><td style=\"padding-left:4px;padding-right:4px;text-align:left !important;\">&nbsp;No Staff Have Emails In The Selected Site(s)&nbsp;</td></tr>";
            output += "</table>";



            if (!previewOnly && emails.Length > 0)
            {
                // email: put to addresss as from address
                // email: put all emails in BCC
                EmailerNew.SimpleEmail(
                    fromEmail,
                    fromName,
                    fromEmail,
                    txtSubject.Text.Trim(),
                    FreeTextBox1.Text,
                    true,
                    null,
                    false,
                    null,
                    IsDebug ? "eli.pollak@mediclnic.com.au" : emails
                    );
            }

            lblEmailOutput.Text = output;


        }
        finally
        {
            Session["DB"] = curDbName;
            Session["SystemVariables"] = SystemVariableDB.GetAll();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "download", @"<script language=javascript>addLoadEvent(function () { window.location.hash = ""emailing_tag""; });</script>");
        }

    }

    public bool IsValidDate(string day, string month, string year)
    {
        bool invalid = ((day == "-1" || month == "-1" || year == "-1") && (day != "-1" || month != "-1" || year != "-1"));

        if ((day == "-1" && month == "-1" && year == "-1"))
            return true;
        else if ((day == "-1" || month == "-1" || year == "-1"))
            return false;

        try
        {
            DateTime d = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    protected DateTime GetDate(string day, string month, string year)
    {
        if ((day == "-1" && month == "-1" && year == "-1"))
            return DateTime.MinValue;

        return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Search(txtPhoneNbrSearch.Text, txtSurnameSearch.Text, ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue);
    }
    protected void Search(string phoneNumberIn = null, string surnameIn = null, string dob_day = null, string dob_month = null, string dob_year = null)
    {
        string phoneNumberSearch = phoneNumberIn == null ? "" : Regex.Replace(phoneNumberIn, "[^0-9]", "");
        string surnameSearch     = surnameIn     == null ? "" : surnameIn.Trim();

        if (phoneNumberSearch == "" && surnameSearch == "" && dob_day == "-1" && dob_month == "-1" && dob_year == "-1")
        {
            lblSearchResults.Text = "<font color=\"red\"><br />Please enter a phone number or surname or DOB to search</font>";
            return;
        }


        string curDbName = Session["DB"].ToString();


        ArrayList dbNames = new ArrayList();
        Hashtable dbHash = new Hashtable();

        string searchResults = string.Empty;

        try
        {

            List<Tuple<string, string>> list = new List<Tuple<string, string>>();


            System.Data.DataTable tbl = DBBase.ExecuteQuery("EXEC sp_databases;", "master").Tables[0];
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                string databaseName = tbl.Rows[i][0].ToString();

                if (!Regex.IsMatch(databaseName, @"Mediclinic_\d{4}"))
                    continue;


                SystemVariables sysVariables = SystemVariableDB.GetAll(databaseName);

                dbNames.Add(sysVariables["Site"].Value);
                dbHash[sysVariables["Site"].Value] = databaseName;


                StringBuilder output = new StringBuilder();

                Session["DB"] = databaseName;
                Session["SystemVariables"] = SystemVariableDB.GetAll();

                string callCenterPrefix = ((SystemVariables)Session["SystemVariables"])["CallCenterPrefix"].Value;


                string siteName = ((SystemVariables)Session["SystemVariables"])["Site"].Value;

                list.Add(new Tuple<string, string>(((SystemVariables)Session["SystemVariables"])["Site"].Value, output.ToString()));


                Site[] sites = SiteDB.GetAll();
                int clinicSiteID = -1;
                int agedCareSiteID = -1;
                for (int j = 0; j < sites.Length; j++)
                {
                    if (sites[j].SiteType.ID == 1)
                        clinicSiteID = sites[j].SiteID;
                    if (sites[j].SiteType.ID == 2)
                        agedCareSiteID = sites[j].SiteID;
                }


                DataTable dt = PatientDB.GetDataTable(false, false, false, false, surnameSearch, true, "", false, "", "", phoneNumberSearch, "", "", false, Convert.ToInt32(dob_day), Convert.ToInt32(dob_month), Convert.ToInt32(dob_year));
                if (dt.Rows.Count > 0)
                {

                    int[] entityIDs = new int[dt.Rows.Count];
                    int[] patientIDs = new int[dt.Rows.Count];
                    for (int p = 0; p < dt.Rows.Count; p++)
                    {
                        entityIDs[p] = Convert.ToInt32(dt.Rows[p]["entity_id"]);
                        patientIDs[p] = Convert.ToInt32(dt.Rows[p]["patient_id"]);
                    }
                    Hashtable bullkPhoneNumbers = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1);

                    Hashtable ptOrgsHash = RegisterPatientDB.GetMostRecentOrganisationOf(patientIDs);

                    for (int p = 0; p < dt.Rows.Count; p++)
                    {
                        string   ptName   = dt.Rows[p]["firstname"].ToString() + " " + dt.Rows[p]["surname"].ToString();
                        DateTime dob      = dt.Rows[p]["dob"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dt.Rows[p]["dob"]);
                        int      ptID     = Convert.ToInt32(dt.Rows[p]["patient_id"]);
                        int      entityID = Convert.ToInt32(dt.Rows[p]["entity_id"]);

                        Organisation org = ptOrgsHash[ptID] as Organisation;

                        string phoneNbrs = string.Empty;
                        if (bullkPhoneNumbers[entityID] != null)
                        {
                            if (Utilities.GetAddressType().ToString() == "Contact")
                            {
                                foreach (Contact c in ((Contact[])bullkPhoneNumbers[entityID]))
                                {
                                    string phoneNumber = Regex.Replace(c.AddrLine1, "[^0-9]", "");
                                    phoneNbrs += (phoneNbrs.Length == 0 ? string.Empty : "<br />") + Utilities.FormatPhoneNumber(phoneNumber).Replace(" ", "-");

                                }
                            }
                            else if (Utilities.GetAddressType().ToString() == "ContactAus")
                            {
                                foreach (ContactAus c in ((ContactAus[])bullkPhoneNumbers[entityID]))
                                {
                                    string phoneNumber = Regex.Replace(c.AddrLine1, "[^0-9]", "");
                                    if (phoneNumber.StartsWith(phoneNumberSearch))
                                        phoneNbrs += (phoneNbrs.Length == 0 ? string.Empty : "<br />") + Utilities.FormatPhoneNumber(phoneNumber).Replace(" ", "-");
                                }
                            }
                            else
                                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
                        }

                        string orgLink = org == null ? "" : @"<a href=""" + HttpContext.Current.Request.Url.AbsolutePath + "?db=" + databaseName + @"&org=" + org.OrganisationID + @"&patient=" + ptID + @""" onclick=""http_post('" + databaseName + "','" + org.OrganisationID + @"','" + (org.IsClinic ? clinicSiteID : agedCareSiteID) + @"'," + ptID + @");return false;"">" + org.Name + @"</a>";
                        string ptLink  =                    @"<a href=""" + HttpContext.Current.Request.Url.AbsolutePath + "?db=" + databaseName + @"&org=0"                     + @"&patient=" + ptID + @""" onclick=""http_post('" + databaseName + "','" + "0"                + @"','" + clinicSiteID                                   + @"'," + ptID + @");return false;"">" + ptName   + @"</a>";

                        output.AppendLine("<tr><td>" + siteName + "</td><td>" + ptLink + "</td><td>" + orgLink + "</td><td style=\"white-space:nowrap\">" + (dob == DateTime.MinValue ? "" : dob.ToString("d MMM, yyyy")) + "</td>" + (phoneNumberSearch == null ? "" : "<td>" + phoneNbrs + "</td>") + "</tr>");
                    }
                }


                list.Add(new Tuple<string, string>(((SystemVariables)Session["SystemVariables"])["Site"].Value, output.ToString()));

                Session.Remove("DB");
                Session.Remove("SystemVariables");
            }



            list.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            System.Text.StringBuilder finalOutput = new System.Text.StringBuilder();
            foreach (Tuple<string, string> item in list)
                finalOutput.Append(item.Item2);


            if (finalOutput.Length == 0)
                lblSearchResults.Text = "<font color=\"red\"><br />No patient found with the search parameters entered</font>";
            else
                lblSearchResults.Text = @"<br />
<table class=""table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center"">
  <tr>
    <th>Site</th>
    <th>Patient</th>
    <th>Book</th>
    <th>D.O.B</th>
    " + (phoneNumberSearch == null ? "" : "<th>Phone Nbr</th>") + @"  
  </tr>
" + finalOutput.ToString() + "</table>";
        }
        finally
        {
            Session["DB"] = curDbName;
            Session["SystemVariables"] = SystemVariableDB.GetAll();
        }

    }


}