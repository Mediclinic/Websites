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

public partial class StaffDetailExternalV2 : System.Web.UI.Page
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
                    }
                    else
                        HideTableAndSetErrorMessage();
                }
                else if (GetUrlParamType() == UrlParamType.Add)
                    FillEmptyAddForm();
                else
                    HideTableAndSetErrorMessage();

                txtFirstname.Focus();
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
        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate',   this, 'dmy', '-'); return false;";


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
        }
        for (int i = 1; i <= 12; i++)
        {
            ddlStartDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlEndDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }

        for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
        {
            ddlStartDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlEndDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }






        bool editable = GetUrlParamType() == UrlParamType.Add || GetUrlParamType() == UrlParamType.Edit;
        Utilities.SetEditControlBackColour(ddlTitle,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtFirstname,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtMiddlename,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtSurname,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlGender,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtLogin,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPwd,               editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
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



        this.lnkThisStaff.Visible = true;
        this.lnkThisStaff.NavigateUrl = "~/RegisterOrganisationsToStaffV2.aspx?id=" + Request.QueryString["id"].ToString();
        this.lnkThisStaff.Text = "Edit";


        lblId.Text = staff.StaffID.ToString();
        lblAddedBy.Text = addedBy == null ? "" : addedBy.Firstname + " " + addedBy.Surname;
        lblStaffDateAdded.Text = staff.StaffDateAdded.ToString("dd-MM-yyyy");

        txtComments.Text = staff.Comment;


        if (isEditMode)
        {
            DataTable titles = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Title", staff.Person.Title.ID == 0 ? "" : " title_id <> 0 ", " descr ", "title_id", "descr");
            ddlTitle.DataSource = titles;
            ddlTitle.DataBind();

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

            txtLogin.Text                  = staff.Login;
            txtPwd.Text                    = staff.Pwd;
            txtPwd.Attributes["value"]     = staff.Pwd;
            ddlStatus.SelectedValue        = staff.IsFired ? "Inactive" : "Active";

            lblTitle.Visible              = false;
            lblFirstname.Visible          = false;
            lblMiddlename.Visible         = false;
            lblSurname.Visible            = false;
            lblGender.Visible             = false;
            lblLogin.Visible              = false;
            lblPwd.Visible                = false;
            lblIsFired.Visible            = false;
        }
        else
        {
            lblTitle.Text             = staff.Person.Title.ID          == 0 ? "" : staff.Person.Title.Descr;
            lblFirstname.Text         = staff.Person.Firstname.Length  == 0 ? "" : staff.Person.Firstname;
            lblMiddlename.Text        = staff.Person.Middlename.Length == 0 ? "" : staff.Person.Middlename;
            lblSurname.Text           = staff.Person.Surname.Length    == 0 ? "" : staff.Person.Surname;
            lblGender.Text            = GetGenderText(staff.Person.Gender);
            lblLogin.Text             = staff.Login.Length             == 0 ? "" : staff.Login;
            lblIsFired.Text           = staff.IsFired                       ? "<b><font color=\"red\">Inactive</font></b>" : "Active";

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

            ddlStartDate_Day.Visible     = false;
            ddlStartDate_Month.Visible   = false;
            ddlStartDate_Year.Visible    = false;
            ddlEndDate_Day.Visible       = false;
            ddlEndDate_Month.Visible     = false;
            ddlEndDate_Year.Visible      = false;

            txtLogin.Visible             = false;
            txtPwd.Visible               = false;
            ddlStatus.Visible            = false;
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
        SetBookingsList(staff);

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

    private void FillEmptyAddForm()
    {
        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + "Add External Staff";
        lblHeading.Text = "Add External Staff";

        this.lnkThisStaff.Visible = false;

        ddlTitle.DataSource = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Title", " title_id <> 0 ", " descr ", "title_id", "descr");
        ddlTitle.DataBind();
        ddlTitle.SelectedIndex = Utilities.IndexOf(ddlTitle, "mr", "mr.");

        idRow.Visible                 = false;
        lblAddedByText.Visible        = false;
        lblAddedBy.Visible            = false;
        lblStaffDateAddedText.Visible = false;
        lblStaffDateAdded.Visible     = false;
        lblPwd.Visible                = false;

        existingStaffInfoSpace.Visible = false;
        existingStaffInfo.Visible      = false;


        btnSubmit.Text = "Add External Staff Member";
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
            !ddlStartDateValidateAllOrNoneSet.IsValid)
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


            PersonDB.Update(staff.Person.PersonID, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), staff.Person.Nickname, ddlGender.SelectedValue, staff.Person.Dob, DateTime.Now);
            StaffDB.Update(staff.StaffID, staff.Person.PersonID, txtLogin.Text, txtPwd.Text, staff.StaffPosition.StaffPositionID, staff.Field.ID, staff.CostCentre.CostCentreID,
                           staff.IsContractor, staff.Tfn, staff.ProviderNumber,
                           ddlStatus.SelectedValue == "Inactive", staff.IsCommission, staff.CommissionPercent,
                           staff.IsStakeholder, staff.IsMasterAdmin, staff.IsAdmin, staff.IsPrincipal, staff.IsProvider, staff.IsExternal,
                           GetStartDateFromForm(), GetEndDateFromForm(), txtComments.Text, staff.EnableDailyReminderSMS, staff.EnableDailyReminderEmail);

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && staff.Login != txtLogin.Text)
            {
                UserDatabaseMapper curDBMapper = UserDatabaseMapperDB.GetByLogin(staff.Login, Session["DB"].ToString());
                UserDatabaseMapperDB.Update(curDBMapper.ID, txtLogin.Text, Session["DB"].ToString());
            }

            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
        }
        else if (GetUrlParamType() == UrlParamType.Add)
        {

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && UserDatabaseMapperDB.UsernameExists(txtLogin.Text))
            {
                SetErrorMessage("Login name already in use by another user");
                return;
            }
            if (StaffDB.LoginExists(txtLogin.Text))
            {
                SetErrorMessage("Login name already in use by another user");
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

                Staff loggedInStaff = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));
                person_id = PersonDB.Insert(loggedInStaff.Person.PersonID, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), "", ddlGender.SelectedValue, DateTime.MinValue);
                staff_id = StaffDB.Insert(person_id, txtLogin.Text, txtPwd.Text, StaffPositionDB.GetByDescr("Unknown").StaffPositionID, 0, 59,
                               false, "", "",
                               ddlStatus.SelectedValue == "Inactive", false, 0,
                               false, false, false, false, false, true,
                               GetStartDateFromForm(), GetEndDateFromForm(), txtComments.Text, false, false);
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



    protected void btnSearchBookingList_Click(object sender, EventArgs e)
    {
        SetBookingsList();
    }
    protected void btnUpdateBookingList_Click(object sender, EventArgs e)
    {
        SetBookingsList();
    }
    protected DataTable SetBookingsList(Staff staff = null)
    {
        if (txtStartDate.Text.Length > 0 && !Utilities.IsValidDate(txtStartDate.Text, "dd-mm-yyyy"))
        {
            SetErrorMessage("Start date must be empty or valid and of the format dd-mm-yyyy");
            return null;
        }
        if (txtEndDate.Text.Length > 0 && !Utilities.IsValidDate(txtEndDate.Text, "dd-mm-yyyy"))
        {
            SetErrorMessage("End date must be empty or valid and of the format dd-mm-yyyy");
            return null;
        }
        DateTime startDate = txtStartDate.Text.Length == 0 ? DateTime.MinValue : Utilities.GetDate(txtStartDate.Text, "dd-mm-yyyy");
        DateTime endDate   = txtEndDate.Text.Length   == 0 ? DateTime.MinValue : Utilities.GetDate(txtEndDate.Text,   "dd-mm-yyyy");

        UserView userView        = UserView.GetInstance();
        int      loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);

        if (staff == null)
            staff = StaffDB.GetByID(GetFormID());



        DataTable tblBookingList = BookingDB.GetDataTable_Between(startDate, endDate, null, null, null, staff, true);

        int[] booking_ids = new int[tblBookingList.Rows.Count];
        for(int i=0; i<tblBookingList.Rows.Count; i++)
            booking_ids[i] = Convert.ToInt32(tblBookingList.Rows[i]["booking_booking_id"]);
        Hashtable changeHistoryHash = BookingDB.GetChangeHistoryCountHash(booking_ids);

        lblBookingListCount.Text = "("+tblBookingList.Rows.Count+")";
        if (tblBookingList.Rows.Count == 0)
        {
            lblBookingsList_NoRowsMessage.Visible = true;
            pnlBookingsList.Visible = false;
        }
        else
        {
            lblBookingsList_NoRowsMessage.Visible = false;
            pnlBookingsList.Visible = true;


            System.Collections.Hashtable staffHash = StaffDB.GetAllInHashtable(true, true, true, false);
            System.Collections.ArrayList bookingsWithInvoices = new System.Collections.ArrayList();


            tblBookingList.Columns.Add("notes_text",                  typeof(string));
            tblBookingList.Columns.Add("invoice_text",                typeof(string));
            tblBookingList.Columns.Add("booking_url",                 typeof(string));
            tblBookingList.Columns.Add("hide_booking_link",           typeof(Boolean));
            tblBookingList.Columns.Add("show_invoice_row",            typeof(int));
            tblBookingList.Columns.Add("show_notes_row",              typeof(int));
            tblBookingList.Columns.Add("show_printletter_row",        typeof(int));
            tblBookingList.Columns.Add("show_bookingsheet_row",       typeof(int));
            tblBookingList.Columns.Add("inv_type_text",               typeof(string));
            tblBookingList.Columns.Add("inv_outstanding_text",        typeof(string));
            tblBookingList.Columns.Add("added_by_deleted_by_row",     typeof(string));
            tblBookingList.Columns.Add("booking_change_history_link", typeof(string));
            tblBookingList.Columns.Add("hide_change_history_link",    typeof(Boolean));
            tblBookingList.Columns.Add("show_change_history_row",     typeof(string));
            bool hasInvoiceRows      = false;
            bool hasNotesRows        = false;
            bool hasPrintLetterRows  = false;
            bool hasBookingSheetRows = false;
            for (int i = 0; i < tblBookingList.Rows.Count; i++)
            {
                Booking curBooking = BookingDB.LoadFull(tblBookingList.Rows[i]);

                tblBookingList.Rows[i]["notes_text"] = Note.GetPopupLinkTextV2(15, curBooking.EntityID, curBooking.NoteCount > 0, true, 1050, 530, "images/notes-bw-24.jpg", "images/notes-24.png", "btnUpdateBookingList.click()");

                bool canSeeInvoiceInfo = userView.IsAdminView || userView.IsPrincipal || (curBooking.Provider != null && curBooking.Provider.StaffID == loggedInStaffID && curBooking.DateStart > DateTime.Today.AddMonths(-2));
                if (canSeeInvoiceInfo && Convert.ToInt32(tblBookingList.Rows[i]["booking_inv_count"]) > 0)
                {
                    string onclick = @"onclick=""javascript:window.showModalDialog('Invoice_ViewV2.aspx?booking_id=" + curBooking.BookingID + @"', '', 'dialogWidth:820px;dialogHeight:860px;center:yes;resizable:no; scroll:no');return false;""";
                    tblBookingList.Rows[i]["invoice_text"] = "<a " + onclick + " href=\"\">View Inv.</a>";

                    if (curBooking.DateDeleted == DateTime.MinValue && curBooking.DeletedBy == null)
                        hasInvoiceRows = true;

                    bookingsWithInvoices.Add(curBooking.BookingID);
                }
                else
                {
                    tblBookingList.Rows[i]["invoice_text"] = "";
                }

                tblBookingList.Rows[i]["hide_booking_link"] = !((userView.IsClinicView   && curBooking.Organisation.OrganisationType.OrganisationTypeID == 218) ||
                                                                (userView.IsAgedCareView && (new List<int> { 139, 367, 372 }).Contains(curBooking.Organisation.OrganisationType.OrganisationTypeID)));

                if (curBooking.DateDeleted == DateTime.MinValue && curBooking.DeletedBy == null)
                {
                    hasNotesRows        = true;
                    hasPrintLetterRows  = true;
                    if (!Convert.ToBoolean(tblBookingList.Rows[i]["hide_booking_link"]))
                        hasBookingSheetRows = true;
                }

                string urlParams = string.Empty;
                if (curBooking.Organisation != null)
                    urlParams += (urlParams.Length == 0 ? "?" : "&") + "orgs=" + curBooking.Organisation.OrganisationID;
                if (curBooking.Patient != null)
                    urlParams += (urlParams.Length == 0 ? "?" : "&") + "patient=" + curBooking.Patient.PatientID;
                urlParams += (urlParams.Length == 0 ? "?" : "&") + "scroll_to_cell=" + "td_" + (curBooking.Organisation != null ? "" : curBooking.Organisation.OrganisationID.ToString()) + "_" + curBooking.Provider.StaffID + "_" + curBooking.DateStart.ToString("yyyy_MM_dd_HHmm");
                urlParams += (urlParams.Length == 0 ? "?" : "&") + "date=" + curBooking.DateStart.ToString("yyyy_MM_dd");
                tblBookingList.Rows[i]["booking_url"] = curBooking.GetBookingSheetLinkV2();



                string addedBy     = curBooking.AddedBy     == null || staffHash[curBooking.AddedBy.StaffID]   == null ? "" : (((Staff)staffHash[curBooking.AddedBy.StaffID]).IsExternal ? "[External Staff] " : "") + ((Staff)staffHash[curBooking.AddedBy.StaffID]).Person.FullnameWithoutMiddlename;
                string addedDate   = curBooking.DateCreated == DateTime.MinValue                                       ? "" : curBooking.DateCreated.ToString("MMM d, yyyy");
                string deletedBy   = curBooking.DeletedBy   == null || staffHash[curBooking.DeletedBy.StaffID] == null ? "" : ((Staff)staffHash[curBooking.DeletedBy.StaffID]).Person.FullnameWithoutMiddlename;
                string deletedDate = curBooking.DateDeleted == DateTime.MinValue                                       ? "" : curBooking.DateDeleted.ToString("MMM d, yyyy");
                string added_by_deleted_by_row = string.Empty;
                added_by_deleted_by_row += "Added By: " + addedBy + " (" + addedDate + ")";
                if (deletedBy.Length > 0 || deletedDate.Length > 0)
                    added_by_deleted_by_row += "\r\nDeleted By: " + deletedBy + " (" + deletedDate + ")";
                tblBookingList.Rows[i]["added_by_deleted_by_row"] = added_by_deleted_by_row;

                tblBookingList.Rows[i]["booking_change_history_link"] = curBooking.GetBookingChangeHistoryPopupLinkImage();
                tblBookingList.Rows[i]["hide_change_history_link"]    = changeHistoryHash[curBooking.BookingID] == null;
            }

            System.Collections.Hashtable hashHasMedicareOrDVAInvoices = BookingDB.GetHashHasMedicareDVA((int[])bookingsWithInvoices.ToArray(typeof(int)));

            for (int i = 0; i < tblBookingList.Rows.Count; i++)
            {
                tblBookingList.Rows[i]["show_invoice_row"]        = hasInvoiceRows              ? 1 : 0;
                tblBookingList.Rows[i]["show_notes_row"]          = hasNotesRows                ? 1 : 0;
                tblBookingList.Rows[i]["show_printletter_row"]    = hasPrintLetterRows          ? 1 : 0;
                tblBookingList.Rows[i]["show_bookingsheet_row"]   = hasBookingSheetRows         ? 1 : 0;
                tblBookingList.Rows[i]["show_change_history_row"] = changeHistoryHash.Count > 0 ? 1 : 0;

                int  booking_id   = Convert.ToInt32(tblBookingList.Rows[i]["booking_booking_id"]);
                bool has_medicare = hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -1)] != null && Convert.ToBoolean(hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -1)]);
                bool has_dva      = hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -2)] != null && Convert.ToBoolean(hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -2)]);
                if (has_medicare) tblBookingList.Rows[i]["inv_type_text"] = "Medicare";
                else if (has_dva) tblBookingList.Rows[i]["inv_type_text"] = "DVA";
                else              tblBookingList.Rows[i]["inv_type_text"] = string.Empty;
            }
            tblBookingList.DefaultView.Sort = "booking_date_start DESC";
            tblBookingList = tblBookingList.DefaultView.ToTable();
            lstBookingList.DataSource = tblBookingList;
            lstBookingList.DataBind();
        }

        return tblBookingList;
    }
    protected void btnPrintBookingList_Click(object sender, EventArgs e)
    {
        DataTable tblBookingList = SetBookingsList();
        if (tblBookingList == null)
            return;

        try
        {
            string originalFile        = Letter.GetLettersDirectory() + @"BookingListForPatient.docx";
            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            string tmpOutputFile       = FileHelper.GetTempFileName(tmpLettersDirectory + "BookingList." + System.IO.Path.GetExtension(originalFile));


            // create table data to populate

            DataTable dt = tblBookingList;
            string[,] tblInfo = null;
            bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
            if (tblEmpty)
            {
                tblInfo = new string[1, 4];
                tblInfo[0, 0] = "No Bookings Found";
                tblInfo[0, 1] = "";
                tblInfo[0, 2] = "";
                tblInfo[0, 3] = "";
            }
            else
            {
                tblInfo = new string[dt.Rows.Count, 4];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string inv_type_text = tblBookingList.Rows[i]["inv_type_text"].ToString();
                    if (inv_type_text.Length > 0) inv_type_text = " (" + inv_type_text + ")";

                    Booking booking = BookingDB.LoadFull(dt.Rows[i]);
                    tblInfo[i, 0] = booking.DateStart.ToString("d MMM yyyy") + Environment.NewLine + booking.DateStart.ToString("h:mm") + " - " + booking.DateEnd.ToString("h:mm") + (booking.DateEnd.Hour < 12 ? "am" : "pm");
                    tblInfo[i, 1] = booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename;
                    tblInfo[i, 2] = (booking.Offering == null ? "" : booking.Offering.Name + Environment.NewLine) + booking.Provider.Person.FullnameWithoutMiddlename + " @ " + booking.Organisation.Name;
                    tblInfo[i, 3] = booking.BookingStatus.Descr + Environment.NewLine + inv_type_text;
                }
            }


            // create empty dataset

            System.Data.DataSet sourceDataSet = new System.Data.DataSet();
            sourceDataSet.Tables.Add("MergeIt");


            // merge

            string errorString = null;
            WordMailMerger.Merge(

                originalFile,
                tmpOutputFile,
                sourceDataSet,

                tblInfo,
                1,
                true,

                false,
                null,
                true,
                null,
                out errorString);

            if (errorString != string.Empty)
                throw new CustomMessageException(errorString);

            Letter.FileContents fileContents = new Letter.FileContents(System.IO.File.ReadAllBytes(tmpOutputFile), "BookingList." + System.IO.Path.GetExtension(originalFile));
            System.IO.File.Delete(tmpOutputFile);


            // Nothing gets past the "DownloadDocument" method because it outputs the file 
            // which is writing a response to the client browser and calls Response.End()
            // So make sure any other code that functions goes before this
            Letter.DownloadDocument(Response, fileContents.Contents, fileContents.DocName);
        }
        catch(CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch(Exception ex)
        {
            SetErrorMessage(ex.ToString());
        }

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