using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;

public partial class StaffDetailV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, false, false, false, false);
                SetUpGUI();

                if ((GetUrlParamType() == UrlParamType.Edit || GetUrlParamType() == UrlParamType.View) && IsValidFormID())
                {
                    Staff staff = StaffDB.GetByID(GetFormID());
                    if (staff != null)
                    {
                        // hide higher privleiged users from lower priveliged users
                        UserView userView = UserView.GetInstance();
                        if ((!userView.IsStakeholder && staff.IsStakeholder) || (!userView.IsStakeholder && !userView.IsMasterAdmin && staff.IsMasterAdmin))
                            Response.Redirect(PagePermissions.UnauthorisedAccessPageForward());

                        FillEditViewForm(staff, GetUrlParamType() == UrlParamType.Edit);

                        if (Utilities.GetAddressType().ToString() == "Contact")
                        {
                            addressControl.Visible = true;
                            addressControl.Set(staff.Person.EntityID, true, EntityType.GetByType(EntityType.EntityTypeEnum.Staff));
                        }
                        else if (Utilities.GetAddressType().ToString() == "ContactAus")
                        {
                            addressAusControl.Visible = true;
                            addressAusControl.Set(staff.Person.EntityID, true, EntityType.GetByType(EntityType.EntityTypeEnum.Staff));
                        }
                        else
                            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
                    }
                    else
                        HideTableAndSetErrorMessage();
                }
                else if (GetUrlParamType() == UrlParamType.Add)
                    FillEmptyAddForm();
                else
                    HideTableAndSetErrorMessage();
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

    #region GetUrlParams

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new Exception("Invalid url id");

        string id = Request.QueryString["id"];
        return Convert.ToInt32(id);
    }

    private enum UrlParamType { Add, Edit, View, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "add")
            return UrlParamType.Add;
        else if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "view")
            return UrlParamType.View;
        else
            return UrlParamType.None;
    }

    #endregion

    protected void SetUpGUI()
    {
        ddlDOB_Day.Items.Add(new ListItem("--", "-1"));
        ddlDOB_Month.Items.Add(new ListItem("--", "-1"));
        ddlDOB_Year.Items.Add(new ListItem("----", "-1"));

        ddlStartDate_Day.Items.Add(new ListItem("--", "-1"));
        ddlStartDate_Month.Items.Add(new ListItem("--", "-1"));
        ddlStartDate_Year.Items.Add(new ListItem("----", "-1"));

        ddlEndDate_Day.Items.Add(new ListItem("--", "-1"));
        ddlEndDate_Month.Items.Add(new ListItem("--", "-1"));
        ddlEndDate_Year.Items.Add(new ListItem("----", "-1"));

        for (int i = 1; i <= 31; i++)
        {
            ddlStartDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlEndDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlDOB_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        for (int i = 1; i <= 12; i++)
        {
            ddlStartDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlEndDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlDOB_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }

        for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
        {
            ddlStartDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlEndDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        for (int i = 1915; i <= DateTime.Today.Year; i++)
        {
            ddlDOB_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }

        UserView userView = UserView.GetInstance();
        lblMasterAdminText.Visible = userView.IsStakeholder || userView.IsMasterAdmin;
        chkIsMasterAdmin.Visible   = userView.IsStakeholder || userView.IsMasterAdmin;
        lblStakeholderText.Visible = userView.IsStakeholder;
        chkIsStakeholder.Visible   = userView.IsStakeholder;





        bool editable = GetUrlParamType() == UrlParamType.Add || GetUrlParamType() == UrlParamType.Edit;
        Utilities.SetEditControlBackColour(ddlTitle,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtFirstname,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtMiddlename,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtSurname,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlGender,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Day,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Month,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Year,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtLogin,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPwd,               editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlField,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtTFN,               editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtProviderNumber,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtCommissionPercent, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlStartDate_Day,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlStartDate_Month,   editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlStartDate_Year,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlEndDate_Day,       editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlEndDate_Month,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlEndDate_Year,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtComments,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlStatus,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

    }

    private void FillEditViewForm(Staff staff, bool isEditMode)
    {
        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + staff.Person.FullnameWithoutMiddlename;

        staff.Person = PersonDB.GetByID(staff.Person.PersonID);
        Person addedBy = staff.Person.PersonID < 0 ? null : PersonDB.GetByID(staff.Person.AddedBy);

        if (!Utilities.IsDev())
            idRow.Attributes["class"] = "hiddencol";

        this.lnkBookingList.Visible = true;
        this.lnkBookingList.NavigateUrl = "~/BookingsListV2.aspx?staff=" + Request.QueryString["id"].ToString();
        this.lnkBookingList.Text = "Bookings List";

        string allFeatures = "dialogWidth:1250px;dialogHeight:835px;center:yes;resizable:no; scroll:no";
        string js = "javascript:window.showModalDialog('" + "BookingSheetBlockoutV2.aspx?staff=" + staff.StaffID.ToString() + "', '', '" + allFeatures + "');return false;";
        this.lnkUnavailabilities.Attributes.Add("onclick", js);
        lnkUnavailabilities.NavigateUrl = "javascript:void(0)";

        UpdateGenerateSystemLetters();

        this.lnkThisStaff.Visible = true;
        this.lnkThisStaff.NavigateUrl = "~/RegisterOrganisationsToStaffV2.aspx?id=" + Request.QueryString["id"].ToString();
        this.lnkThisStaff.Text = "Edit";

        this.lnkStaffOfferings.Visible = true;
        this.lnkStaffOfferings.NavigateUrl = "~/StaffOfferingsListV2.aspx?staff=" + Request.QueryString["id"].ToString();
        this.lnkStaffOfferings.Text = "Set Comissions/Fixed Rates";


        lblId.Text = staff.StaffID.ToString();
        lblAddedBy.Text = addedBy == null ? "" : addedBy.Firstname + " " + addedBy.Surname;
        lblStaffDateAdded.Text = staff.StaffDateAdded.ToString("dd-MM-yyyy");

        txtComments.Text = staff.Comment;


        if (isEditMode)
        {
            DataTable titles = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Title", staff.Person.Title.ID == 0 ? "" : " title_id <> 0 ", " descr ", "title_id", "descr");
            ddlTitle.DataSource = titles;
            ddlTitle.DataBind();
            DataTable fields = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "field_id <> 0", "descr", "field_id", "descr");
            ddlField.DataSource = fields;
            ddlField.DataBind();
            //DataTable costcentres = CostCentreDB.GetDataTable();
            //ddlCostCentre.DataSource = costcentres;
            //ddlCostCentre.DataBind();

            if (staff.StartDate != DateTime.MinValue)
            {
                ddlStartDate_Day.SelectedValue = staff.StartDate.Day.ToString();
                ddlStartDate_Month.SelectedValue = staff.StartDate.Month.ToString();
                ddlStartDate_Year.SelectedValue = staff.StartDate.Year.ToString();
            }
            if (staff.EndDate != DateTime.MinValue)
            {
                ddlEndDate_Day.SelectedValue = staff.EndDate.Day.ToString();
                ddlEndDate_Month.SelectedValue = staff.EndDate.Month.ToString();
                ddlEndDate_Year.SelectedValue = staff.EndDate.Year.ToString();
            }
            ddlTitle.SelectedValue = staff.Person.Title.ID.ToString();
            txtFirstname.Text = staff.Person.Firstname;
            txtMiddlename.Text = staff.Person.Middlename;
            txtSurname.Text = staff.Person.Surname;
            if (ddlGender.Items.FindByValue(staff.Person.Gender) == null)
                ddlGender.Items.Add(new ListItem(staff.Person.Gender == "" ? "" : staff.Person.Gender, staff.Person.Gender));
            ddlGender.SelectedValue = staff.Person.Gender;
            if (staff.Person.Dob != DateTime.MinValue)
            {
                ddlDOB_Day.SelectedValue = staff.Person.Dob.Day.ToString();
                ddlDOB_Month.SelectedValue = staff.Person.Dob.Month.ToString();
                ddlDOB_Year.SelectedValue = staff.Person.Dob.Year.ToString();
            }
            txtLogin.Text                  = staff.Login;
            txtPwd.Text                    = staff.Pwd;
            txtPwd.Attributes["value"]     = staff.Pwd;

            if (ddlField.Items.FindByValue(staff.Field.ID.ToString()) == null)
                ddlField.Items.Add(new ListItem(staff.Field.Descr, staff.Field.ID.ToString()));

            ddlField.SelectedValue         = staff.Field.ID.ToString();
            chkContractor.Checked          = staff.IsContractor;
            txtTFN.Text                    = staff.Tfn;
            ddlStatus.SelectedValue        = staff.IsFired ? "Inactive" : "Active";
            chkSMSBKs.Checked              = staff.EnableDailyReminderSMS;
            chkEmailBKs.Checked            = staff.EnableDailyReminderEmail;
            //ddlCostCentre.SelectedValue    = staff.CostCentre.CostCentreID.ToString();
            txtProviderNumber.Text         = staff.ProviderNumber;
            chkIsCommission.Checked        = staff.IsCommission;
            txtCommissionPercent.Text      = staff.CommissionPercent.ToString();
            chkIsProvider.Checked          = staff.IsProvider;
            chkIsPrincipal.Checked         = staff.IsPrincipal;
            chkIsAdmin.Checked             = staff.IsAdmin;
            chkIsMasterAdmin.Checked       = staff.IsMasterAdmin;
            chkIsStakeholder.Checked       = staff.IsStakeholder;

            lblTitle.Visible              = false;
            lblFirstname.Visible          = false;
            lblMiddlename.Visible         = false;
            lblSurname.Visible            = false;
            lblGender.Visible             = false;
            lblDOB.Visible                = false;
            lblLogin.Visible              = false;
            lblPwd.Visible                = false;
            lblField.Visible              = false;
            lblContractor.Visible         = false;
            lblTFN.Visible                = false;
            //lblCostCentre.Visible         = false;
            lblProviderNumber.Visible     = false;
            lblIsCommission.Visible       = false;
            lblCommissionPercent.Visible  = false;
            lblIsFired.Visible            = false;
            lblSMSBKs.Visible             = false;
            lblEmailBKs.Visible           = false;
            lblIsProvider.Visible         = false;
            lblIsPrincipal.Visible        = false;
            lblIsAdmin.Visible            = false;
            lblIsMasterAdmin.Visible      = false;
            lblIsStakeholder.Visible      = false;
        }
        else
        {
            lblTitle.Text             = staff.Person.Title.ID          == 0 ? "" : staff.Person.Title.Descr;
            lblFirstname.Text         = staff.Person.Firstname.Length  == 0 ? "" : staff.Person.Firstname;
            lblMiddlename.Text        = staff.Person.Middlename.Length == 0 ? "" : staff.Person.Middlename;
            lblSurname.Text           = staff.Person.Surname.Length    == 0 ? "" : staff.Person.Surname;
            lblGender.Text            = GetGenderText(staff.Person.Gender);
            lblDOB.Text               = staff.Person.Dob == DateTime.MinValue ? "" : staff.Person.Dob.ToString("dd-MM-yyyy");
            lblLogin.Text             = staff.Login.Length  == 0 ? "" : staff.Login;
            lblField.Text             = staff.Field.Descr;
            lblContractor.Text        = staff.IsContractor  ? "Yes" : "No";
            lblTFN.Text               = staff.Tfn.Length  == 0 ? "" : staff.Tfn;
            //lblCostCentre.Text        = staff.CostCentre.Descr.Length  == 0 ? "" : staff.CostCentre.Descr;
            lblProviderNumber.Text    = staff.ProviderNumber.Length  == 0 ? "" : staff.ProviderNumber;
            lblIsCommission.Text      = staff.IsCommission  ? "Yes" : "No";
            lblCommissionPercent.Text = staff.CommissionPercent.ToString();
            lblIsFired.Text           = staff.IsFired       ? "<b><font color=\"red\">Inactive</font></b>" : "Active";
            lblSMSBKs.Text            = staff.EnableDailyReminderSMS   ? "Yes" : "No";
            lblEmailBKs.Text          = staff.EnableDailyReminderEmail ? "Yes" : "No";

            lblIsProvider.Text        = staff.IsProvider    ? "Yes" : "No";
            lblIsPrincipal.Text       = staff.IsPrincipal   ? "Yes" : "No";
            lblIsAdmin.Text           = staff.IsAdmin       ? "Yes" : "No";
            lblIsMasterAdmin.Text     = staff.IsMasterAdmin ? "Yes" : "No";
            lblIsStakeholder.Text     = staff.IsStakeholder ? "Yes" : "No";

            lblStartDate.Text = staff.StartDate == DateTime.MinValue ? "" : staff.StartDate.ToString("dd-MM-yyyy");
            lblEndDate.Text   = staff.EndDate   == DateTime.MinValue ? "" : staff.EndDate.ToString("dd-MM-yyyy");

            lblAddedBy.Font.Bold = true;
            lblStaffDateAdded.Font.Bold = true;

            txtComments.Enabled = false;
            txtComments.ForeColor = System.Drawing.Color.Black;




            ddlTitle.Visible             = false;
            txtFirstname.Visible         = false;
            txtMiddlename.Visible        = false;
            txtSurname.Visible           = false;
            ddlGender.Visible            = false;
            ddlDOB_Day.Visible           = false;
            ddlDOB_Month.Visible         = false;
            ddlDOB_Year.Visible          = false;

            ddlStartDate_Day.Visible     = false;
            ddlStartDate_Month.Visible   = false;
            ddlStartDate_Year.Visible    = false;
            ddlEndDate_Day.Visible       = false;
            ddlEndDate_Month.Visible     = false;
            ddlEndDate_Year.Visible      = false;

            txtLogin.Visible             = false;
            txtPwd.Visible               = false;
            ddlField.Visible             = false;
            chkContractor.Visible        = false;
            txtTFN.Visible               = false;
            ddlStatus.Visible            = false;
            chkSMSBKs.Visible            = false;
            chkEmailBKs.Visible          = false;
            //ddlCostCentre.Visible        = false;
            txtProviderNumber.Visible    = false;
            chkIsCommission.Visible      = false;
            txtCommissionPercent.Visible = false;
            chkIsProvider.Visible        = false;
            chkIsPrincipal.Visible       = false;
            chkIsAdmin.Visible           = false;
            chkIsMasterAdmin.Visible     = false;
            chkIsStakeholder.Visible     = false;
        }


        DataTable incList = RegisterStaffDB.GetDataTable_OrganisationsOf(staff.StaffID);
        incList.DefaultView.Sort = "name ASC";
        if (incList.Rows.Count == 0)
        {
            lstClinics.Items.Add(new ListItem("No Clinics Allocated Yet"));
        }
        else
        {
            foreach (DataRowView row in incList.DefaultView)
                lstClinics.Items.Add(new ListItem(row["name"].ToString()));
        }

        UpdateSiteRestrictions();


        btnSubmit.Text = isEditMode ? "Update Details" : "Edit Details";
        btnCancel.Visible = isEditMode;
    }
    protected string GetGenderText(string originalText)
    {
        if (originalText.ToUpper() == "M")
            return "Male";
        else if (originalText.ToUpper() == "F")
            return "Female";
        else
            return "";
    }
    protected void UpdateGenerateSystemLetters()
    {
        int bookingCountToGenerateSystemLetters = BookingDB.GetBookingCountToGenerateSystemLetters(DateTime.MinValue, DateTime.MinValue, GetFormID());
        lblBookingsYetToGenerateSystemLetters.Text = bookingCountToGenerateSystemLetters.ToString();
        btnGenerateSystemLetters.Visible = bookingCountToGenerateSystemLetters > 0;
    }

    private void FillEmptyAddForm()
    {
        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + "Add Staff";
        lblHeading.Text = "Add Staff";

        txtFirstname.Focus();

        this.lnkThisStaff.Visible = false;


        txtPwd.TextMode = TextBoxMode.SingleLine;

        ddlTitle.DataSource = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Title", " title_id <> 0 ", " descr ", "title_id", "descr");
        ddlTitle.DataBind();
        ddlTitle.SelectedIndex = Utilities.IndexOf(ddlTitle, "mr", "mr.");

        DataTable fields    = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "field_id <> 0", "descr", "field_id", "descr");
        ddlField.DataSource = fields;
        ddlField.DataBind();
        //DataTable costcentres = CostCentreDB.GetDataTable();
        //ddlCostCentre.DataSource = costcentres;
        //ddlCostCentre.DataBind();

        idRow.Visible                 = false;
        lblAddedByText.Visible        = false;
        lblAddedBy.Visible            = false;
        lblStaffDateAddedText.Visible = false;
        lblStaffDateAdded.Visible     = false;
        lblPwd.Visible                = false;

        existingStaffInfoSpace.Visible = false;
        existingStaffInfo.Visible      = false;


        chkIsCommission.Checked = true;
        txtCommissionPercent.Text = "0.00";

        btnSubmit.Text = "Add Staff Member";
    }


    protected void DOBAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue);
    }
    protected void StartDateAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlStartDate_Day.SelectedValue, ddlStartDate_Month.SelectedValue, ddlStartDate_Year.SelectedValue);
    }
    protected void EndDateAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlEndDate_Day.SelectedValue, ddlEndDate_Month.SelectedValue, ddlEndDate_Year.SelectedValue);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (!ddlEndDateValidateAllOrNoneSet.IsValid ||
            !ddlStartDateValidateAllOrNoneSet.IsValid ||
            !ddlDOBValidateAllOrNoneSet.IsValid)
            return;

        txtPwd.Attributes["value"] = txtPwd.Text;  // pwd fields is unset on send back to server, so re-set it

        if (GetUrlParamType() == UrlParamType.View)
        {
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "edit"));
        }
        else if (GetUrlParamType() == UrlParamType.Edit)
        {
            Staff staff = StaffDB.GetByID(Convert.ToInt32(this.lblId.Text));

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && staff.Login != txtLogin.Text && UserDatabaseMapperDB.UsernameExists(txtLogin.Text))
            {
                SetErrorMessage("Login name already in use by another user");
                return;
            }
            if (StaffDB.LoginExists(txtLogin.Text, staff.StaffID))
            {
                SetErrorMessage("Login name already in use by another user");
                return;
            }
            if (staff.Pwd != txtPwd.Text && txtPwd.Text.Length < 6)
            {
                SetErrorMessage(staff.Pwd.Length >= 6 ? "Password must be at least 6 characters" : "New passwords must be at least 6 characters");
                return;
            }

            bool loggedInUserIsStakeholder = Session["IsStakeholder"] != null && Convert.ToBoolean(Session["IsStakeholder"]);
            bool loggedInUserIsMasterAdmin = Session["IsMasterAdmin"] != null && Convert.ToBoolean(Session["IsMasterAdmin"]);
            bool setIsStakeholder = loggedInUserIsStakeholder ? chkIsStakeholder.Checked : staff.IsStakeholder;
            bool setIsMasterAdmin = loggedInUserIsStakeholder || loggedInUserIsMasterAdmin ? chkIsMasterAdmin.Checked : staff.IsMasterAdmin;

            if (!staff.IsProvider && chkIsProvider.Checked && (StaffDB.GetCountOfProviders() >= Convert.ToInt32(SystemVariableDB.GetByDescr("MaxNbrProviders").Value)))
            {
                SetErrorMessage("You have reached your maximum allowable providers. Please uncheck their status as a provider to update them or hit cancel. Contact Mediclinic if you would like to upgrade your account.");
                return;
            }


            if (chkIsProvider.Checked)
            {
                System.Data.DataTable tbl = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "has_offerings=1 AND field_id <> 0", "", "field_id", "descr");

                bool roleSetAsProvider = false;
                IDandDescr[] fields = new IDandDescr[tbl.Rows.Count];
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    fields[i] = new IDandDescr(Convert.ToInt32(tbl.Rows[i]["field_id"]), tbl.Rows[i]["descr"].ToString());
                    if (Convert.ToInt32(ddlField.SelectedValue) == Convert.ToInt32(tbl.Rows[i]["field_id"]))
                        roleSetAsProvider = true;
                }

                if (!roleSetAsProvider)
                {
                    if (fields.Length == 1)
                    {
                        SetErrorMessage("When setting a staff member as a provider, you need to set their Role as '" + fields[0].Descr + "'.");
                        return;
                    }
                    else if (fields.Length == 2)
                    {
                        SetErrorMessage("When setting a staff member as a provider, you need to set their Role as '" + fields[0].Descr + "' or '" + fields[1].Descr + "'.");
                        return;
                    }
                    else
                    {
                        string providerFields = string.Empty;
                        for (int i = 0; i < fields.Length; i++)
                            providerFields += (providerFields.Length == 0 ? "" : ", ") + (fields.Length >= 2 && i == (fields.Length - 2) ? "or " : "") + fields[i].Descr;

                        SetErrorMessage("When setting a staff member as a provider, you need to set their Role as one of the following: " + providerFields);
                        return;
                    }
                }
            }



            if (chkIsMasterAdmin.Checked)
                chkIsAdmin.Checked = true;

            PersonDB.Update(staff.Person.PersonID, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), staff.Person.Nickname, ddlGender.SelectedValue, GetDOBFromForm(), DateTime.Now);
            StaffDB.Update(staff.StaffID, staff.Person.PersonID, txtLogin.Text, txtPwd.Text, staff.StaffPosition.StaffPositionID, Convert.ToInt32(ddlField.SelectedValue), staff.CostCentre.CostCentreID,
                           chkContractor.Checked, txtTFN.Text, txtProviderNumber.Text.ToUpper(),
                           ddlStatus.SelectedValue == "Inactive", chkIsCommission.Checked, Convert.ToDecimal(txtCommissionPercent.Text),
                           setIsStakeholder, setIsMasterAdmin, chkIsAdmin.Checked, chkIsPrincipal.Checked, chkIsProvider.Checked, staff.IsExternal,
                           GetStartDateFromForm(), GetEndDateFromForm(), txtComments.Text, chkSMSBKs.Checked, chkEmailBKs.Checked);

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && staff.Login != txtLogin.Text)
            {
                UserDatabaseMapper curDBMapper = UserDatabaseMapperDB.GetByLogin(staff.Login, Session["DB"].ToString());
                if (curDBMapper == null)
                    UserDatabaseMapperDB.Insert(txtLogin.Text, Session["DB"].ToString());
                else
                    UserDatabaseMapperDB.Update(curDBMapper.ID, txtLogin.Text, Session["DB"].ToString());
            }

            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
        }
        else if (GetUrlParamType() == UrlParamType.Add)
        {
            if (chkIsProvider.Checked && (StaffDB.GetCountOfProviders() >= Convert.ToInt32(SystemVariableDB.GetByDescr("MaxNbrProviders").Value)))
            {
                SetErrorMessage("You have reached your maximum allowable providers. Please uncheck their status as a provider to add them. Contact Mediclinic if you would like to upgrade your account.");
                return;
            }

            if (chkIsProvider.Checked)
            {
                System.Data.DataTable tbl = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "has_offerings=1 AND field_id <> 0", "", "field_id", "descr");

                bool roleSetAsProvider = false;
                IDandDescr[] fields = new IDandDescr[tbl.Rows.Count];
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    fields[i] = new IDandDescr(Convert.ToInt32(tbl.Rows[i]["field_id"]), tbl.Rows[i]["descr"].ToString());
                    if (Convert.ToInt32(ddlField.SelectedValue) == Convert.ToInt32(tbl.Rows[i]["field_id"]))
                        roleSetAsProvider = true;
                }

                if (!roleSetAsProvider)
                {
                    if (fields.Length == 1)
                    {
                        SetErrorMessage("When setting a staff member as a provider, you need to set their Role as '" + fields[0].Descr + "'.");
                        return;
                    }
                    else if (fields.Length == 2)
                    {
                        SetErrorMessage("When setting a staff member as a provider, you need to set their Role as '" + fields[0].Descr + "' or '" + fields[1].Descr + "'.");
                        return;
                    }
                    else
                    {
                        string providerFields = string.Empty;
                        for (int i = 0; i < fields.Length; i++)
                            providerFields += (providerFields.Length == 0 ? "" : ", ") + (fields.Length >= 2 && i == (fields.Length - 2) ? "or " : "") + fields[i].Descr;

                        SetErrorMessage("When setting a staff member as a provider, you need to set their Role as one of the following: " + providerFields);
                        return;
                    }
                }
            }

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && UserDatabaseMapperDB.UsernameExists(txtLogin.Text))
            {
                lblErrorMessage.Text = "Login name already in use by another user";
                lblErrorMessage.Visible = true;
                return;
            }
            if (StaffDB.LoginExists(txtLogin.Text))
            {
                lblErrorMessage.Text = "Login name already in use by another user";
                lblErrorMessage.Visible = true;
                return;
            }
            if (txtPwd.Text.Length < 6)
            {
                SetErrorMessage("Password must be at least 6 characters");
                return;
            }


            int  person_id    = -1;
            int  staff_id     = -1;
            bool staff_added  = false;
            int  mainDbUserID = -1;

            try
            {
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                    mainDbUserID = UserDatabaseMapperDB.Insert(txtLogin.Text, Session["DB"].ToString());

                bool loggedInUserIsStakeholder = Session["IsStakeholder"] != null && Convert.ToBoolean(Session["IsStakeholder"]);
                bool loggedInUserIsMasterAdmin = Session["IsMasterAdmin"] != null && Convert.ToBoolean(Session["IsMasterAdmin"]);
                bool setIsStakeholder = loggedInUserIsStakeholder ? chkIsStakeholder.Checked : false;
                bool setIsMasterAdmin = loggedInUserIsStakeholder || loggedInUserIsMasterAdmin ? chkIsMasterAdmin.Checked : false;

                if (chkIsMasterAdmin.Checked)
                    chkIsAdmin.Checked = true;

                Staff loggedInStaff = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));
                person_id = PersonDB.Insert(loggedInStaff.Person.PersonID, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), "", ddlGender.SelectedValue, GetDOBFromForm());
                staff_id = StaffDB.Insert(person_id, txtLogin.Text, txtPwd.Text, StaffPositionDB.GetByDescr("Unknown").StaffPositionID, Convert.ToInt32(ddlField.SelectedValue), 59,
                               chkContractor.Checked, txtTFN.Text, txtProviderNumber.Text.ToUpper(),
                               ddlStatus.SelectedValue == "Inactive", chkIsCommission.Checked, Convert.ToDecimal(txtCommissionPercent.Text),
                               setIsStakeholder, setIsMasterAdmin, chkIsAdmin.Checked, chkIsPrincipal.Checked, chkIsProvider.Checked, false,
                               GetStartDateFromForm(), GetEndDateFromForm(), txtComments.Text, chkSMSBKs.Checked, chkEmailBKs.Checked);
                staff_added = true;

                string url = Request.RawUrl;
                url = UrlParamModifier.AddEdit(url, "type", "view");
                url = UrlParamModifier.AddEdit(url, "id", staff_id.ToString());
                Response.Redirect(url);
            }
            catch (Exception)
            {
                if (staff_added)
                {
                    string url = Request.RawUrl;
                    url = UrlParamModifier.AddEdit(url, "type", "view");
                    url = UrlParamModifier.AddEdit(url, "id", staff_id.ToString());
                    Response.Redirect(url);
                    return;
                }

                // roll back - backwards of creation order
                PersonDB.Delete(person_id);
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                    UserDatabaseMapperDB.Delete(mainDbUserID);

                throw;
            }
        }
        else
        {
            HideTableAndSetErrorMessage();
        }
    }


    public bool HasFutureBookings(Staff staff, System.Web.UI.HtmlControls.HtmlInputCheckBox chkBox, DayOfWeek dayOfWeek)
    {
        if (chkBox.Checked)
            return false;

        Booking[] bookings = BookingDB.GetFutureBookings(staff, null, dayOfWeek.ToString(), new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59));
        if (bookings.Length == 0)
            return false;

        string space = "          ";
        string bookingDates = string.Empty;
        for (int i = 0; i < bookings.Length; i++)
            bookingDates += "<br />" + space + bookings[i].DateStart.ToString(@"ddd MMM d, yyy HH:mm");

        lblErrorMessage.Text = "Can not select " + dayOfWeek.ToString() + "  as not working until the bookings on the following days have been moved or deleted:" + "<br />" + bookingDates + "<br />";
        lblErrorMessage.Visible = true;
        return true;
    }


    public DateTime GetDOBFromForm()
    {
        return GetDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue, "DOB");
    }
    public DateTime GetStartDateFromForm()
    {
        return GetDate(ddlStartDate_Day.SelectedValue, ddlStartDate_Month.SelectedValue, ddlStartDate_Year.SelectedValue, "Start Date");
    }
    public DateTime GetEndDateFromForm()
    {
        return GetDate(ddlEndDate_Day.SelectedValue, ddlEndDate_Month.SelectedValue, ddlEndDate_Year.SelectedValue, "End Date");
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
        bool invalid = ((day == "-1" || month == "-1" || year == "-1") && (day != "-1" || month != "-1" || year != "-1"));
        return !invalid;
    }




    protected void btnGenerateSystemLetters_Click(object sender, EventArgs e)
    {
        System.Collections.ArrayList fileContentsList = new System.Collections.ArrayList();

        Booking[] bookings = BookingDB.GetBookingsToGenerateSystemLetters(DateTime.MinValue, DateTime.MinValue, GetFormID());
        for (int i = 0; i < bookings.Length; i++)
        {

            // send referrer letters

            PatientReferrer[] patientReferrers = PatientReferrerDB.GetActiveEPCPatientReferrersOf(bookings[i].Patient.PatientID);
            if (patientReferrers.Length > 0)
            {
                HealthCard hc = HealthCardDB.GetActiveByPatientID(bookings[i].Patient.PatientID);
                bool needToGenerateTreatmentLetter = patientReferrers[patientReferrers.Length - 1].RegisterReferrer.ReportEveryVisitToReferrer;

                Letter.FileContents[] bookingFileContentsList = bookings[i].GetSystemLettersList(Letter.FileFormat.Word, bookings[i].Patient, hc, bookings[i].Offering.Field.ID, patientReferrers[patientReferrers.Length - 1].RegisterReferrer.Referrer, true, bookings[i].NeedToGenerateFirstLetter, bookings[i].NeedToGenerateLastLetter, needToGenerateTreatmentLetter, false, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), 1);
                fileContentsList.AddRange(bookingFileContentsList);
                BookingDB.UpdateSetGeneratedSystemLetters(bookings[i].BookingID, bookings[i].NeedToGenerateFirstLetter, bookings[i].NeedToGenerateLastLetter, true);
            }
        }

        Letter.FileContents mergedFileContents = Letter.FileContents.Merge((Letter.FileContents[])fileContentsList.ToArray(typeof(Letter.FileContents)), "Treatment Letters.doc"); // .pdf
        if (mergedFileContents != null)
        {
            Session["downloadFile_Contents"] = mergedFileContents.Contents;
            Session["downloadFile_DocName"] = mergedFileContents.DocName;
            Page.ClientScript.RegisterStartupScript(this.GetType(), "download", "<script language=javascript>window.open('DownloadFile.aspx','_blank','status=1,toolbar=0,menubar=0,location=1,scrollbars=1,resizable=1,width=30,height=30');</script>");
        }

        UpdateGenerateSystemLetters();
    }


    protected void btnToggleSiteRestriction_Click(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "TurnOff" || e.CommandName == "TurnOn")
        {
            int siteID = Convert.ToInt32(e.CommandArgument);
            int staffID = GetFormID();
            bool setRestriction = e.CommandName == "TurnOff";

            StaffSiteRestrictionDB.Update(staffID, siteID, setRestriction);

            // log them out so to make this have an immeidate effect
            if (setRestriction)
                UserLoginDB.UpdateSetAllSessionsLoggedOut(staffID, -1);

            UpdateSiteRestrictions();
        }
    }

    protected void UpdateSiteRestrictions()
    {
        DataTable sitesWithRestrictions = StaffSiteRestrictionDB.GetDataTable_AllSitesWithRestriction(GetFormID());
        lstSites.DataSource = sitesWithRestrictions;
        lstSites.DataBind();

        lblNoSitesExist.Visible = sitesWithRestrictions.Rows.Count == 0;
    }



    #region HideTableAndSetErrorMessage, SetErrorMessage, HideErrorMessag

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
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
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br /><br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br /><br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion


}