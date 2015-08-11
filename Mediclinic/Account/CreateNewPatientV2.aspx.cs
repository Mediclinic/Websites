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
using System.Collections.Specialized;

public partial class CreateNewPatientV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            lblErrorMessage.Text = string.Empty;

            if (!IsPostBack)
            {
                Utilities.SetNoCache(Response);
                main_content.Style["background"] = (Session["SystemVariables"] == null) ? "url(../imagesV2/login_bg.png) center top no-repeat #EDEDED" : "url(../imagesV2/" + ((SystemVariables)Session["SystemVariables"])["MainLogoBackground"].Value + ") center top no-repeat #EDEDED";

                if (Request.QueryString["id"] == null)
                    throw new CustomMessageException("No ID in URL");

                // if url is   ?id=0001&id=0001 then  Request.QueryString["id"]  =  "0001,0001" ...   so split it and keep only the first
                string id = Request.QueryString["id"].Contains(",") ? Request.QueryString["id"].Split(',')[0] : Request.QueryString["id"];

                if (!Regex.IsMatch(id, @"^\d{4}$"))
                    throw new CustomMessageException("No valid ID in URL");

                string dbName = "Mediclinic_" + id;
                string sql      = "SELECT Count(name) FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = '" + dbName + "' OR name = '" + dbName + "')";
                bool   dbExists = Convert.ToBoolean(DBBase.ExecuteSingleResult(sql, "master"));

                if (!dbExists)
                    throw new CustomMessageException("Invalid ID in URL");

                SetupGUI(id, Request.QueryString["org"]);
            }

        }
        catch (CustomMessageException ex)
        {
            this.lblErrorMessage.Text = ex.Message;
            main_table.Visible = false;
            lblExistingUserMessage.Visible = false;
        }
        catch (Exception ex)
        {
            this.lblErrorMessage.Text = ex.ToString();
            main_table.Visible = false;
            lblExistingUserMessage.Visible = false;
        }
    }

    #endregion

    #region SetupGUI

    private void SetupGUI(string dbID, string orgID)
    {
        ddlClinic.Focus();

        ddlDOB_Day.Items.Add(new ListItem("--", "-1"));
        ddlDOB_Month.Items.Add(new ListItem("--", "-1"));
        ddlDOB_Year.Items.Add(new ListItem("----", "-1"));

        for (int i = 1; i <= 31; i++)
            ddlDOB_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1; i <= 12; i++)
            ddlDOB_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1900; i <= DateTime.Today.Year; i++)
            ddlDOB_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));



        string curDbName = Session == null || Session["DB"] == null ? null : Session["DB"].ToString();
        try
        {
            Session["DB"]              = "Mediclinic_" + dbID;
            Session["SystemVariables"] = SystemVariableDB.GetAll();

            bool allowPatientsToCreateOwnRecords = ((SystemVariables)Session["SystemVariables"])["AllowPatientsToCreateOwnRecords"].Value == "1";
            if (!allowPatientsToCreateOwnRecords)
                throw new CustomMessageException("Invalid ID in URL");

            ddlTitle.DataSource = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Title", " title_id <> 0 ", " descr ", "title_id", "descr");
            ddlTitle.DataBind();
            ddlTitle.SelectedIndex = Utilities.IndexOf(ddlTitle, "mr", "mr.");

            DataTable phoneNumberTypes = ContactTypeDB.GetDataTable(2);
            for (int i = phoneNumberTypes.Rows.Count - 1; i >= 0; i--)
                if (Convert.ToInt32(phoneNumberTypes.Rows[i]["at_contact_type_id"]) != 30 && Convert.ToInt32(phoneNumberTypes.Rows[i]["at_contact_type_id"]) != 33)
                    phoneNumberTypes.Rows.RemoveAt(i);
            ddlPhoneNumberType.DataSource = phoneNumberTypes;
            ddlPhoneNumberType.DataBind();
            ddlPhoneNumberType.SelectedValue = "30"; // mobile

            lblSiteName.Text = ((SystemVariables)Session["SystemVariables"])["Site"].Value;

            Site[] sites = SiteDB.GetAll();
            Site clinicSite = null;
            Site agedCareSite = null;
            for (int i = 0; i < sites.Length; i++)
            {
                if (sites[i].SiteType.ID == 1)
                    clinicSite = sites[i];
                if (sites[i].SiteType.ID == 2)
                    agedCareSite = sites[i];
            }

            List<Tuple<string, Organisation>> clinics = GetClinicList();
            foreach (Tuple<string, Organisation> item in clinics)
                ddlClinic.Items.Add(new ListItem(item.Item2.Name, "Mediclinic_" + dbID + "__" + clinicSite.SiteID + "__" + item.Item2.OrganisationID));

            if (orgID != null && Regex.IsMatch(orgID, @"^\d+$"))
            {
                Organisation org = OrganisationDB.GetByID(Convert.ToInt32(orgID));
                if (org != null && ddlClinic.Items.FindByValue("Mediclinic_" + dbID + "__" + clinicSite.SiteID + "__" + org.OrganisationID) != null)
                    ddlClinic.SelectedValue = "Mediclinic_" + dbID + "__" + clinicSite.SiteID + "__" + org.OrganisationID;
            }

        }
        finally
        {
            Session.Remove("DB");
            Session.Remove("SystemVariables");

            if (curDbName != null)
            {
                Session["DB"] = curDbName;
                Session["SystemVariables"] = SystemVariableDB.GetAll();
            }
        }



        bool editable = true;
        Utilities.SetEditControlBackColour(ddlClinic,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlTitle ,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtFirstname ,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtSurname ,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlGender ,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Day ,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Month ,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Year ,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPhoneNumber,       editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlPhoneNumberType,   editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtEmailAddr,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtLogin,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPwd,               editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }

    protected List<Tuple<string, Organisation>> GetClinicList()
    {
        try
        {

            List<Tuple<string, Organisation>> list = new List<Tuple<string, Organisation>>();

            bool allowPatientsToCreateOwnRecords = ((SystemVariables)Session["SystemVariables"])["AllowPatientsToCreateOwnRecords"].Value == "1";
            if (allowPatientsToCreateOwnRecords)
            {
                Organisation[] orgs = OrganisationDB.GetAll(false, true, false, false, true, true);
                if (orgs.Length > 0)
                    for (int j = 0; j < orgs.Length; j++)
                        if (orgs[j].IsClinic)
                            list.Add(new Tuple<string, Organisation>(((SystemVariables)Session["SystemVariables"])["Site"].Value, orgs[j]));
            }

            return list;
        }
        catch (CustomMessageException cmEx)
        {
            this.lblErrorMessage.Text = cmEx.Message;
            return null;
        }
    }

    #endregion

    #region DOBSetCheck

    protected void DOBSetCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue);
    }

    public DateTime GetDOBFromForm()
    {
        return GetDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue, "DOB");
    }
    public DateTime GetDate(string day, string month, string year, string fieldNme)
    {
        if (day == "-1" && month == "-1" && year == "-1")
            return DateTime.MinValue;

        else if (day != "-1" && month != "-1" && year != "-1")
            return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));

        else
            throw new Exception(fieldNme + " format is some selected and some not selected.");
    }
    public bool IsValidDate(string day, string month, string year)
    {
        return (day != "-1" && month != "-1" && year != "-1");
    }

    #endregion

    #region CreatePatient

    protected void CreatePatientButton_Click(object sender, EventArgs e)
    {
        if (!ddlDOBValidateAllSet.IsValid)
            return;

        int  person_id           = -1;
        int  patient_id          = -1;
        int  register_patient_id = -1;
        bool patient_added       =  false;
        int  mainDbUserID        = -1;

        int  phone_id         = -1;
        int  email_id         = -1;
        bool contacts_added   =  false;

        try
        {
            string[] clinicInfo = ddlClinic.SelectedValue.Split(new string[] { "__" }, StringSplitOptions.None);
            string   dbID   = clinicInfo[0];
            int      siteID = Convert.ToInt32(clinicInfo[1]);
            int      orgID  = Convert.ToInt32(clinicInfo[2]);

            Session["DB"] = dbID;
            Session["SystemVariables"] = SystemVariableDB.GetAll();

            txtEmailAddr.Text   = txtEmailAddr.Text.Trim();
            txtPhoneNumber.Text = txtPhoneNumber.Text.Trim();
            if (!Utilities.IsValidEmailAddress(txtEmailAddr.Text))
                throw new CustomMessageException("Email must be in valid email format.");

            txtLogin.Text = txtLogin.Text.Trim();
            txtPwd.Text   = txtPwd.Text.Trim();

            txtFirstname.Text = txtFirstname.Text.Trim();
            txtSurname.Text   = txtSurname.Text.Trim();



            // check if patient exists in the system, if so use existing patietn

            bool patientAlreadyExists = false;

            // check if email exists in the system
            if (!patientAlreadyExists)
            {
                if (ExistsAndCreatedLogin_FromEmail(orgID, txtPhoneNumber.Text, txtEmailAddr.Text, siteID, ref register_patient_id, ref phone_id, ref email_id))
                {
                    patientAlreadyExists = true;
                    patient_added  = true;
                    contacts_added = true;
                    this.lblErrorMessage.Text = "Your email alrady exist in this sytem.<br/>An email has been sent with new login details.<br/>When you receieve it, use the login link below.";
                }
            }

            // check if firstname / surname / DOB exists in the system
            if (!patientAlreadyExists)
            {
                if (ExistsAndCreatedLogin_FromNameAndDOB(orgID, txtPhoneNumber.Text, txtEmailAddr.Text, txtFirstname.Text, txtSurname.Text, GetDOBFromForm(), siteID, ref register_patient_id, ref phone_id, ref email_id))
                {
                    patientAlreadyExists = true;
                    patient_added  = true;
                    contacts_added = true;
                    this.lblErrorMessage.Text = "You alrady exist in this sytem.<br/>An email has been sent with new login details.<br/>When you receieve it, use the login link below.";
                }
            }




            if (!patientAlreadyExists)
            {
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && UserDatabaseMapperDB.UsernameExists(txtLogin.Text))
                    throw new CustomMessageException("Login name already in use. Please choose another");
                if (PatientDB.LoginExists(txtLogin.Text))
                    throw new CustomMessageException("Login name already in use. Please choose another");


                // 1. Create Patient

                Staff loggedInStaff = StaffDB.GetByID(-6);
                person_id           = PersonDB.Insert(loggedInStaff.Person.PersonID, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text), "", Utilities.FormatName(txtSurname.Text), "", ddlGender.SelectedValue, GetDOBFromForm());
                patient_id          = PatientDB.Insert(person_id, true, false, false, "", -1, DateTime.MinValue, "", "", DateTime.MinValue, false, false, DateTime.MinValue, -1, -1, txtLogin.Text, txtPwd.Text, false, "", "", "", "");
                register_patient_id = RegisterPatientDB.Insert(orgID, patient_id);
                patient_added       = true;   // added this because was throwing a thread aborted exception after patient added before Response.Redirect


                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                    if (txtLogin.Text.Length > 0)
                        mainDbUserID = UserDatabaseMapperDB.Insert(txtLogin.Text, Session["DB"].ToString());


                // 2. Add Contact Info

                Patient patient = PatientDB.GetByID(patient_id);

                phone_id            = AddPhoneNbrIfNotExists(patient, siteID, txtPhoneNumber.Text);
                email_id            = AddEmailIfNotExists(patient,    siteID, txtEmailAddr.Text);
                register_patient_id = AddOrgIfNotExists(patient,      siteID, orgID);
                contacts_added      = true;


                SendInfoEmail(txtEmailAddr.Text, txtLogin.Text, txtPwd.Text);

                this.lblErrorMessage.Text = "An email has been sent with new login details.<br />When you receieve it, use the login link below.";

            }

        }
        catch(Exception ex)
        {
            if (!patient_added || !contacts_added)
            {
                // roll back - backwards of creation order

                if (Utilities.GetAddressType().ToString() == "Contact")
                {
                    ContactDB.Delete(phone_id);
                    ContactDB.Delete(email_id);
                }
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                {
                    ContactAusDB.Delete(phone_id);
                    ContactAusDB.Delete(email_id);
                }
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

                RegisterPatientDB.Delete(register_patient_id);
                PatientDB.Delete(patient_id);
                PersonDB.Delete(person_id);

                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                    UserDatabaseMapperDB.Delete(mainDbUserID);

                if (ex is CustomMessageException)
                    this.lblErrorMessage.Text = ex.Message;
                else
                    lblErrorMessage.Text = ex.ToString();
            }
        }
        finally
        {
            //Session["DB"] = curDbName;
            //Session["SystemVariables"] = SystemVariableDB.GetAll();
            Session.Remove("DB");
            Session.Remove("SystemVariables");
        }

    }

    protected bool ExistsAndCreatedLogin_FromNameAndDOB(int orgID, string phoneNumber, string email, string firstname, string surname, DateTime DOB, int siteID, ref int register_patient_id, ref int phone_id, ref int email_id)
    {
        bool patientAlreadyExists = false;

        Patient[] matchingPatients = PatientDB.GetByFirstnameSurnameDOB(firstname, surname, DOB);
        foreach (Patient patient in matchingPatients)
        {
            if (patient == null || patient.IsDeceased || patient.IsDeleted)
                continue;

            if (patient.Person.Firstname != firstname         ||
                patient.Person.Surname   != surname           || 
                patient.Person.Dob       == DateTime.MinValue || 
                patient.Person.Dob       != DOB)
                continue;

            // if no login set, create it

            bool hasLoginDetails = patient.Login.Length > 0;
            if (!hasLoginDetails)
            {
                string login = txtLogin.Text;
                string loginTry = login;

                Random rnd = new Random();
                int nbr = rnd.Next(11, 999);

                do
                {
                    bool loginUsed = (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && UserDatabaseMapperDB.UsernameExists(loginTry)) ||
                                        (PatientDB.LoginExists(loginTry));

                    if (loginUsed)
                        throw new CustomMessageException("Login name in use. Please choose another");

                    if (!loginUsed)
                    {
                        patient.Login = loginTry;
                        patient.Pwd   = txtPwd.Text;

                        PatientDB.UpdateLoginPwd(patient.PatientID, patient.Login, patient.Pwd);
                        if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                            UserDatabaseMapperDB.Insert(loginTry, Session["DB"].ToString());

                        break;
                    }

                    nbr++;
                    loginTry = login + nbr;

                } while (true);

            }



            // add phone number if different from existing
            phone_id = AddPhoneNbrIfNotExists(patient, siteID, phoneNumber);

            // add email if different from existing
            email_id = AddEmailIfNotExists(patient, siteID, email);

            // add clinic if different from existing
            register_patient_id = AddOrgIfNotExists(patient, siteID, orgID);


            SendInfoEmail(email, patient.Login, patient.Pwd);
            patientAlreadyExists = true;
        }

        return patientAlreadyExists;
    }

    protected bool ExistsAndCreatedLogin_FromEmail(int orgID, string phoneNumber, string email, int siteID, ref int register_patient_id, ref int phone_id, ref int email_id)
    {
        bool patientAlreadyExists = false;


        int[] entityIDs;
        if (Utilities.GetAddressType().ToString() == "Contact")
            entityIDs = ContactDB.GetByAddrLine1(null, email, 27).Select(r => r.EntityID).ToArray();
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
            entityIDs = ContactAusDB.GetByAddrLine1(null, email, 27).Select(r => r.EntityID).ToArray();
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        foreach (int entityID in entityIDs)
        {
            Patient patient = PatientDB.GetByEntityID(entityID);
            if (patient == null || patient.IsDeceased || patient.IsDeleted)
                continue;

            // if no login set, create it

            bool hasLoginDetails = patient.Login.Length > 0;
            if (!hasLoginDetails)
            {
                string login = txtLogin.Text;
                string loginTry = login;

                Random rnd = new Random();
                int nbr = rnd.Next(11, 999);

                do
                {
                    bool loginUsed = (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && UserDatabaseMapperDB.UsernameExists(loginTry)) ||
                                        (PatientDB.LoginExists(loginTry));


                    if (loginUsed)
                        throw new CustomMessageException("Login name in use. Please choose another");

                    if (!loginUsed)
                    {
                        patient.Login = loginTry;
                        patient.Pwd   = txtPwd.Text;

                        PatientDB.UpdateLoginPwd(patient.PatientID, patient.Login, patient.Pwd);
                        if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                            UserDatabaseMapperDB.Insert(loginTry, Session["DB"].ToString());

                        break;
                    }

                    nbr++;
                    loginTry = login + nbr;

                } while (true);

            }


            // add phone number if different from existing
            phone_id = AddPhoneNbrIfNotExists(patient, siteID, phoneNumber);

            // add clinic if different from existing
            register_patient_id = AddOrgIfNotExists(patient, siteID, orgID);


            SendInfoEmail(email, patient.Login, patient.Pwd);
            patientAlreadyExists = true;
        }

        return patientAlreadyExists;
    }

    protected int AddPhoneNbrIfNotExists(Patient patient, int siteID, string phoneNumber)
    {
        // add phone number if different from existing

        int phone_id = -1;

        string[] phoneNumbers;
        if (Utilities.GetAddressType().ToString() == "Contact")
            phoneNumbers = ContactDB.GetByEntityID(2, patient.Person.EntityID, -1).Select(r => r.AddrLine1).ToArray();
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
            phoneNumbers = ContactAusDB.GetByEntityID(2, patient.Person.EntityID, -1).Select(r => r.AddrLine1).ToArray();
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        bool phoneNumberAlreadyExists = false;
        foreach (string p in phoneNumbers)
            if (Regex.Replace(p, "[^0-9]", "") == Regex.Replace(phoneNumber, "[^0-9]", ""))
                phoneNumberAlreadyExists = true;

        if (!phoneNumberAlreadyExists)
        {
            if (Utilities.GetAddressType().ToString() == "Contact")
            {
                if (phoneNumber.Length > 0)
                    phone_id = ContactDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlPhoneNumberType.SelectedValue),
                        "",
                        System.Text.RegularExpressions.Regex.Replace(phoneNumber, "[^0-9]", ""),
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        siteID,
                        true,
                        true);
            }
            else if (Utilities.GetAddressType().ToString() == "ContactAus")
            {
                if (phoneNumber.Length > 0)
                    phone_id = ContactAusDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlPhoneNumberType.SelectedValue),
                        "",
                        System.Text.RegularExpressions.Regex.Replace(phoneNumber, "[^0-9]", ""),
                        string.Empty,
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        siteID,
                        true,
                        true);
            }
            else
            {
                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
            }
        }

        return phone_id;
    }

    protected int AddEmailIfNotExists(Patient patient, int siteID, string email)
    {
        // add email if different from existing

        int email_id = -1;

        string[] emails = ContactDB.GetEmailsByEntityID(patient.Person.EntityID);

        bool emailAlreadyExists = false;
        foreach (string e in emails)
            if (e == email)
                emailAlreadyExists = true;

        if (!emailAlreadyExists)
        {
            if (Utilities.GetAddressType().ToString() == "Contact")
            {
                if (email.Length > 0)
                    email_id = ContactDB.Insert(patient.Person.EntityID,
                        27,
                        "",
                        email,
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(siteID),
                        true,
                        true);
            }
            else if (Utilities.GetAddressType().ToString() == "ContactAus")
            {
                if (email.Length > 0)
                    email_id = ContactAusDB.Insert(patient.Person.EntityID,
                        27,
                        "",
                        email,
                        string.Empty,
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(siteID),
                        true,
                        true);
            }
            else
            {
                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
            }
        }

        return email_id;
    }

    protected int AddOrgIfNotExists(Patient patient, int siteID, int orgID)
    {
        // add clinic if different from existing

        int register_patient_id = -1;

        bool orgAlreadyExists = false;
        Organisation[] orgs = RegisterPatientDB.GetOrganisationsOf(patient.PatientID);
        foreach(Organisation org in orgs)
            if (org.OrganisationID == orgID)
                orgAlreadyExists = true;

        if (!orgAlreadyExists)
            register_patient_id = RegisterPatientDB.Insert(orgID, patient.PatientID);

        return register_patient_id;
    }

    protected void SendInfoEmail(string email, string login, string pwd)
    {
        string body = @"Below is your login/password details as requested from the Create Web Login webpage.<br>
<br>
<table>
  <tr>
    <td>Username:</td>
    <td><b>" + login + @"</b></td>
  </tr>
  <tr>
    <td>Password:</td>
    <td><b>" + pwd + @"</b></td>
  </tr>
</table>

<br>
<br>    Regards,
<br>    Mediclinic";


        Emailer.SimpleEmail(email,
            "Password Generation Request",
            body,
            true,
            null,
            null);
    }

    #endregion 

    #region lnkLogin_Click

    protected void lnkLogin_Click(object sender, EventArgs e)
    {
        // un-set auth cookie that was set to allow this page to not be directed to always goes to config file field 'loginUrl'
        System.Web.Security.FormsAuthentication.SetAuthCookie("--", true);

        if (Request.QueryString["from_url"] != null)
            Response.Redirect("~/Account/LoginV2.aspx?from_url=" + Request.QueryString["from_url"], false);
        else
            Response.Redirect("~/Account/LoginV2.aspx", false);
    }

    #endregion

}
