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

public partial class PatientAddV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, true);
                SetupGUI();
                FillEmptyAddForm();
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

    #region SetupGUI

    private void SetupGUI()
    {
        txtFirstname.Focus();

        ddlDOB_Day.Items.Add(new ListItem("--", "-1"));
        ddlDOB_Month.Items.Add(new ListItem("--", "-1"));
        ddlDOB_Year.Items.Add(new ListItem("----", "-1"));

        for (int i = 1; i <= 31; i++)
            ddlDOB_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1; i <= 12; i++)
            ddlDOB_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1900; i <= DateTime.Today.Year; i++)
            ddlDOB_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

        ddlDARev_Day.Items.Add(new ListItem("--", "-1"));
        ddlDARev_Month.Items.Add(new ListItem("--", "-1"));
        ddlDARev_Year.Items.Add(new ListItem("----", "-1"));

        for (int i = 1; i <= 31; i++)
            ddlDARev_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1; i <= 12; i++)
            ddlDARev_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 2000; i <= DateTime.Today.Year; i++)
            ddlDARev_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

        ddlConcessionCardExpiry_Month.Items.Add(new ListItem("--", "-1"));
        ddlConcessionCardExpiry_Year.Items.Add(new ListItem("--", "-1"));

        for (int i = 1; i <= 12; i++)
            ddlConcessionCardExpiry_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = DateTime.Today.Year - 10; i <= DateTime.Today.Year + 10; i++)
            ddlConcessionCardExpiry_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

        acTypeRow.Visible = UserView.GetInstance().IsAgedCareView;

        hiddenIsMobileDevice.Value = Utilities.IsMobileDevice(Request) ? "1" : "0";


        bool editable = true;
        Utilities.SetEditControlBackColour(ddlTitle ,                     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtFirstname ,                 editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtMiddlename ,                editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtSurname ,                   editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtNickname ,                  editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlGender ,                    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Day ,                   editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Month ,                 editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Year ,                  editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDARev_Day ,                 editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDARev_Month ,               editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDARev_Year ,                editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPrivateHealthFund,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtConcessionCardNbr,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlConcessionCardExpiry_Month, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlConcessionCardExpiry_Year,  editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlACInvOffering,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtLogin,                      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPwd,                        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtABN,                        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtNextOfKinName,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtNextOfKinRelation,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtNextOfKinContactInfo,       editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }

    #endregion

    #region SetupAddressGUI(), btnUpdate Address/Ph/Email methods

    protected void SetupAddressGUI()
    {
        string allFeaturesType = "dialogWidth:500px;dialogHeight:750px;center:yes;resizable:no; scroll:no";
        string jsType = "javascript:window.showModalDialog('" + "ContactTypeListV2.aspx', '', '" + allFeaturesType + "');document.getElementById('btnUpdateAddressType').click();return false;";

        string allFeatures = "dialogWidth:1100px;dialogHeight:600px;center:yes;resizable:no; scroll:no";
        string js = "javascript:window.showModalDialog('" + "StreetAndSuburbInfo.aspx', '', '" + allFeatures + "');document.getElementById('btnUpdateAddressStreetAndSuburb').click();return false;";

        lnkAddressUpdateType.NavigateUrl = "  ";
        lnkAddressUpdateType.Text = "Add/Edit";
        lnkAddressUpdateType.Attributes.Add("onclick", jsType);

        lnkAddressUpdateChannel.NavigateUrl = "  ";
        lnkAddressUpdateChannel.Text = "Add/Edit";
        lnkAddressUpdateChannel.Attributes.Add("onclick", js);

        string allFeaturesType2 = "dialogWidth:500px;dialogHeight:750px;center:yes;resizable:no; scroll:no";
        string jsType2 = "javascript:window.showModalDialog('" + "ContactTypeListV2.aspx', '', '" + allFeaturesType2 + "');document.getElementById('btnUpdatePhoneType').click();return false;";

        string allFeaturesType3 = "dialogWidth:500px;dialogHeight:750px;center:yes;resizable:no; scroll:no";
        string jsType3 = "javascript:window.showModalDialog('" + "ContactTypeListV2.aspx', '', '" + allFeaturesType3 + "');document.getElementById('btnUpdateEmailType').click();return false;";

        lnkEmailUpdateType.NavigateUrl = "  ";
        lnkEmailUpdateType.Text = "Add/Edit";
        lnkEmailUpdateType.Attributes.Add("onclick", jsType3);



        DataTable orgs = OrganisationDB.GetDataTable(0, false, true, true, false, true, true);
        orgs.DefaultView.Sort = "name ASC";
        ddlOrganisationAC.Items.Clear();
        ddlOrganisationAC.Items.Add(new ListItem(string.Empty, "-1"));
        foreach (DataRowView row in orgs.DefaultView)
            if (row["name"].ToString().Trim().Length > 0)
                ddlOrganisationAC.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));


        if (!UserView.GetInstance().IsAgedCareView)
        {
            tr_ac_org_trailingspace.Visible = false;
            tr_ac_org.Visible = false;

            tr_ac_room.Visible = false;
            tr_ac_room_note.Visible = false;
            tr_ac_room_trailingspace.Visible = false;
        }
        


        ddlAddressContactType.DataSource = ContactTypeDB.GetDataTable(1);
        ddlAddressContactType.DataBind();

        DataTable dt_PhoneNumbers = ContactTypeDB.GetDataTable(2);
        ddlPhoneNumber1.DataSource = dt_PhoneNumbers.Copy();
        ddlPhoneNumber2.DataSource = dt_PhoneNumbers.Copy();
        ddlPhoneNumber3.DataSource = dt_PhoneNumbers.Copy();
        ddlPhoneNumber1.DataBind();
        ddlPhoneNumber2.DataBind();
        ddlPhoneNumber3.DataBind();
        ddlPhoneNumber1.SelectedValue = "30"; // mobile
        ddlPhoneNumber2.SelectedValue = "33"; // home
        ddlPhoneNumber3.SelectedValue = "34"; // office
        lnkPhone1UpdateType.NavigateUrl = "  ";
        lnkPhone1UpdateType.Text = "Add/Edit";
        lnkPhone1UpdateType.Attributes.Add("onclick", jsType2);
        lnkPhone2UpdateType.NavigateUrl = "  ";
        lnkPhone2UpdateType.Text = "Add/Edit";
        lnkPhone2UpdateType.Attributes.Add("onclick", jsType2);
        lnkPhone3UpdateType.NavigateUrl = "  ";
        lnkPhone3UpdateType.Text = "Add/Edit";
        lnkPhone3UpdateType.Attributes.Add("onclick", jsType2);


        ddlEmailContactType.DataSource = ContactTypeDB.GetDataTable(4);
        ddlEmailContactType.DataBind();

        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            streetRow_Contact.Visible = true;
            DataTable addrChannels = AddressChannelDB.GetDataTable();
            ddlAddressAddressChannel.Items.Add(new ListItem("--", "-1"));
            foreach (DataRow row in addrChannels.Rows)
                ddlAddressAddressChannel.Items.Add(new ListItem(row["ac_descr"] + " " + row["act_descr"], row["ac_address_channel_id"].ToString()));
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            streetRow_ContactAus.Visible = true;
            DataTable addrChannelTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "AddressChannelType", "", "descr", "address_channel_type_id", "descr");
            ddlAddressAddressChannelType.Items.Add(new ListItem("--", "-1"));
            foreach (DataRow row in addrChannelTypes.Rows)
                ddlAddressAddressChannelType.Items.Add(new ListItem(row["descr"].ToString(), row["address_channel_type_id"].ToString()));
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        DataTable countries = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Country", "", "descr", "country_id", "descr");
        ddlAddressCountry.Items.Add(new ListItem("--", "-1"));
        foreach (DataRow row in countries.Rows)
            ddlAddressCountry.Items.Add(new ListItem(row["descr"].ToString(), row["country_id"].ToString()));
        ddlAddressCountry.SelectedIndex = Utilities.IndexOf(ddlAddressCountry, "australia");


        bool editable = true;

        Utilities.SetEditControlBackColour(ddlOrganisationAC,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtACRoom,                    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtACRoomNote,                editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        Utilities.SetEditControlBackColour(ddlAddressContactType,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtAddressAddrLine1,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtAddressAddrLine2,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlAddressAddressChannel,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtStreet,                    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlAddressAddressChannelType, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlAddressCountry,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtAddressFreeText,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        Utilities.SetEditControlBackColour(ddlPhoneNumber1,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlPhoneNumber2,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlPhoneNumber3,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPhoneNumber1,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPhoneNumber2,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPhoneNumber3,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPhoneNumber1FreeText,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPhoneNumber2FreeText,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPhoneNumber3FreeText,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        Utilities.SetEditControlBackColour(ddlEmailContactType,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtEmailAddrLine1,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }

    protected void btnUpdateAddressStreetAndSuburb_Click(object sender, EventArgs e)
    {
        DataTable addrChannels = AddressChannelDB.GetDataTable();
        ddlAddressAddressChannel.Items.Clear();
        ddlAddressAddressChannel.Items.Add(new ListItem("--", "-1"));
        foreach (DataRow row in addrChannels.Rows)
            ddlAddressAddressChannel.Items.Add(new ListItem(row["ac_descr"] + " " + row["act_descr"], row["ac_address_channel_id"].ToString()));
        //ddlAddressChannel.SelectedValue = thisRow["ad_address_channel_id"] == DBNull.Value ? "-1" : thisRow["ad_address_channel_id"].ToString();
    }

    protected void btnUpdateAddressType_Click(object sender, EventArgs e)
    {
        ddlAddressContactType.DataSource = ContactTypeDB.GetDataTable(1);
        ddlAddressContactType.DataBind();
        // ddlAddressContactType.SelectedValue = thisRow["ad_contact_type_id"].ToString();
    }

    protected void btnUpdatePhoneType_Click(object sender, EventArgs e)
    {
        DataTable dt_PhoneNumbers = ContactTypeDB.GetDataTable(2);
        ddlPhoneNumber1.DataSource = dt_PhoneNumbers.Copy();
        ddlPhoneNumber2.DataSource = dt_PhoneNumbers.Copy();
        ddlPhoneNumber3.DataSource = dt_PhoneNumbers.Copy();
        ddlPhoneNumber1.DataBind();
        ddlPhoneNumber2.DataBind();
        ddlPhoneNumber3.DataBind();
        ddlPhoneNumber1.SelectedValue = "30"; // mobile
        ddlPhoneNumber2.SelectedValue = "33"; // home
        ddlPhoneNumber3.SelectedValue = "34"; // office
    }

    protected void btnUpdateEmailType_Click(object sender, EventArgs e)
    {
        ddlEmailContactType.DataSource = ContactTypeDB.GetDataTable(4);
        ddlEmailContactType.DataBind();
        // ddlEmailContactType.SelectedValue = thisRow["ad_contact_type_id"].ToString();
    }

    #endregion

    #region SetupHealthCardGUI()

    protected void SetupHealthCardGUI()
    {
        DataTable orgs = OrganisationDB.GetDataTable_GroupOrganisations();
        for (int i = orgs.Rows.Count - 1; i >= 0; i--)
        {
            Organisation org = OrganisationDB.Load(orgs.Rows[i]);
            if (org.OrganisationID != -1 && org.OrganisationID != -2)
                orgs.Rows.RemoveAt(i);
        }
        ddlHealthCardOrganisation.DataSource     = orgs;
        ddlHealthCardOrganisation.DataTextField  = "name";
        ddlHealthCardOrganisation.DataValueField = "organisation_id";
        ddlHealthCardOrganisation.DataBind();
        ddlHealthCardOrganisation.SelectedValue = "-1";

        txtHealthCardCardNbr.Style["display"] = "none";

        for (int i = 1; i < 10; i++)
            ddlHealthCardCardFamilyMemberNbr.Items.Add(new ListItem(i.ToString(), i.ToString()));

        ddlHealthCardCardExpiry_Month.Items.Add(new ListItem("--", "-1"));
        ddlHealthCardCardExpiry_Year.Items.Add(new ListItem("--", "-1"));

        for (int i = 1; i <= 12; i++)
            ddlHealthCardCardExpiry_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = DateTime.Today.Year - 10; i <= DateTime.Today.Year + 10; i++)
            ddlHealthCardCardExpiry_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));


        bool editable = true;
        Utilities.SetEditControlBackColour(ddlHealthCardOrganisation,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardName,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlHealthCardCardFamilyMemberNbr, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlHealthCardCardExpiry_Month,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlHealthCardCardExpiry_Year,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_1,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_2,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_3,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_4,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_5,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_6,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_7,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_8,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_9,     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtHealthCardCardNbr_Digit_10,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

    }

    #endregion

    #region FillEmptyAddForm

    private void FillEmptyAddForm()
    {
        UserView userView = UserView.GetInstance();

        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + (!userView.IsAgedCareView ? "Add Patient" : "Add Resident");
        lblHeading.Text = !userView.IsAgedCareView ? "Add Patient" : "Add Resident";


        string surnamePostParam = Request.Form["surname"];
        if (surnamePostParam != null)  // sent by http post
            txtSurname.Text = Utilities.FormatName(surnamePostParam);

        string surnameGetParam = Request.QueryString["surname"];
        if (surnameGetParam != null)  // sent by http get
            txtSurname.Text = Utilities.FormatName(surnameGetParam);


        DataTable dt_titles = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Title", "", " descr ", "title_id", "descr");
        for (int i = dt_titles.Rows.Count - 1; i >= 0; i--)
        {
            if (Convert.ToInt32(dt_titles.Rows[i]["title_id"]) == 0)
            {
                DataRow newRow = dt_titles.NewRow();
                newRow.ItemArray = dt_titles.Rows[i].ItemArray;
                dt_titles.Rows.RemoveAt(i);
                dt_titles.Rows.InsertAt(newRow, 0);
                break;
            }
        }
        ddlTitle.DataSource = dt_titles;
        ddlTitle.DataBind();
        ddlTitle.SelectedIndex = Utilities.IndexOf(ddlTitle, "mr", "mr.");


        DataTable dt_offerings = OfferingDB.GetDataTable(true, -1, null, true);
        for (int i = dt_offerings.Rows.Count - 1; i >= 0; i--)
        {
            if (Convert.ToInt32(dt_offerings.Rows[i]["o_aged_care_patient_type_id"]) == 1 || Convert.ToBoolean(dt_offerings.Rows[i]["o_is_deleted"]))
                dt_offerings.Rows.RemoveAt(i);

            // only allow LC/HC/LCF/HCF initially ... not medicare/dva/emergency/etc
            else if (!(new List<int> { 2, 3, 4, 5 }).Contains(Convert.ToInt32(dt_offerings.Rows[i]["o_aged_care_patient_type_id"])))
                dt_offerings.Rows.RemoveAt(i);
        }


        DataView dv = dt_offerings.DefaultView;
        dv.Sort = "acpatientcat_aged_care_patient_type_id, o_name";
        dt_offerings = dv.ToTable();

        ddlACInvOffering.DataSource = dt_offerings;
        ddlACInvOffering.DataBind();

        FillGrdCondition();

        btnSubmitAddAndGoToViewScreen.Visible       = !userView.IsExternalView;
        btnSubmitAddAndGoToBookingScreen.Visible    = true;
        btnSubmitAddAndGoToHealthCardScreen.Visible = !userView.IsExternalView;
        btnSubmitAddAndAddAonther.Visible           = !userView.IsExternalView;

        SetupAddressGUI();
        SetupHealthCardGUI();


        if (Convert.ToBoolean(Session["SiteIsGP"]))
        {
            spnSpaceBeforeSubmitAddAndGoToHealthCardScreen.Visible = false;
            btnSubmitAddAndGoToHealthCardScreen.Visible = false;
        }

    }

    #endregion

    #region DOBAllOrNoneCheck

    protected void DOBAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue);
    }

    #endregion

    #region DARevAllOrNoneCheck

    protected void DARevAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlDARev_Day.SelectedValue, ddlDARev_Month.SelectedValue, ddlDARev_Year.SelectedValue);
    }

    #endregion

    #region btnSubmitAdd_Click

    protected void btnSubmitAdd_Click(object sender, CommandEventArgs e)
    {
        btnAdd(e.CommandName);
    }
    protected void btnAdd(string nextScreen)
    {
        if (!ddlDOBValidateAllOrNoneSet.IsValid)
            return;
        if (!ddlDARevValidateAllOrNoneSet.IsValid)
            return;

        UserView userView = UserView.GetInstance();

        int  person_id           = -1;
        int  patient_id          = -1;
        int  register_patient_id = -1;
        bool patient_added       =  false;
        int mainDbUserID         = -1;

        int  ac_register_patient_id  = -1;

        int  bedroom_id       = -1;
        int  address_id       = -1;
        int  phone_id1        = -1;
        int  phone_id2        = -1;
        int  phone_id3        = -1;
        int  email_id         = -1;
        bool contacts_added   =  false;
        int  healthcard_id    = -1;
        bool healthcard_added =  false;

        try
        {
            txtPwd.Attributes["value"] = txtPwd.Text;  // pwd fields is unset on send back to server, so re-set it

            if (txtLogin.Text.Length > 0)
            {
                if (txtPwd.Text.Length == 0)
                    throw new CustomMessageException("Password is required when username entered");
                if (txtPwd.Text.Length < 6)
                    throw new CustomMessageException("Password must be at least 6 characters");
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && UserDatabaseMapperDB.UsernameExists(txtLogin.Text))
                    throw new CustomMessageException("Login name already in use by another user");
                if (PatientDB.LoginExists(txtLogin.Text))
                    throw new CustomMessageException("Login name already in use by another user");
            }

            if (nextScreen != "AddAndGoToHealthCardScreen" && nextScreen != "AddAndGoToBookingScreen" && nextScreen != "AddAndGoToViewScreen" && nextScreen != "AddAndAddAonther")
                throw new CustomMessageException("unknown nextScreen value : " + nextScreen);

            if (nextScreen == "AddAndGoToHealthCardScreen" && (txtHealthCardCardName.Text.Trim().Length == 0 && txtHealthCardCardNbr.Text.Trim().Length == 0))
                throw new CustomMessageException("Can not go to add " + (userView.IsClinicView ? "EPC" : "Referral") + " info unless you have set healthcard information");

            if ((ddlConcessionCardExpiry_Year.SelectedValue  != "-1" && ddlConcessionCardExpiry_Month.SelectedValue == "-1") ||
                (ddlConcessionCardExpiry_Month.SelectedValue != "-1" && ddlConcessionCardExpiry_Year.SelectedValue  == "-1"))
                throw new CustomMessageException("Concession Card Expiry Must Be Both Set or Both Unset.");

            DateTime concessionCardExpiryDate = ddlConcessionCardExpiry_Year.SelectedValue == "-1" || ddlConcessionCardExpiry_Month.SelectedValue == "-1" ?
                                                DateTime.MinValue :
                                                new DateTime(Convert.ToInt32(ddlConcessionCardExpiry_Year.SelectedValue), Convert.ToInt32(ddlConcessionCardExpiry_Month.SelectedValue), 1);

            int acOfferingID = userView.IsAgedCareView ? Convert.ToInt32(ddlACInvOffering.SelectedValue) : -1;

            Staff loggedInStaff = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));
            person_id  = PersonDB.Insert(loggedInStaff.Person.PersonID, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text.Trim()), Utilities.FormatName(txtMiddlename.Text.Trim()), Utilities.FormatName(txtSurname.Text.Trim()), txtNickname.Text.Trim(), ddlGender.SelectedValue, GetDOBFromForm());
            patient_id = PatientDB.Insert(person_id, userView.IsClinicView, userView.IsGPView, chkIsDeceased.Checked, "", -1, DateTime.MinValue, txtPrivateHealthFund.Text, txtConcessionCardNbr.Text, concessionCardExpiryDate, chkIsDiabetic.Checked, chkIsMemberDiabetesAustralia.Checked, GetDARevFromForm(), acOfferingID, acOfferingID, txtLogin.Text, txtPwd.Text, chkIsCompany.Checked, txtABN.Text.Trim(), txtNextOfKinName.Text.Trim(), txtNextOfKinRelation.Text.Trim(), txtNextOfKinContactInfo.Text.Trim());
            if (!userView.IsAdminView)
                register_patient_id = RegisterPatientDB.Insert(Convert.ToInt32(Session["OrgID"]), patient_id);
            patient_added = true;   // added this because was throwing a thread aborted exception after patient added before Response.Redirect



            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                if (txtLogin.Text.Length > 0)
                    mainDbUserID = UserDatabaseMapperDB.Insert(txtLogin.Text, Session["DB"].ToString());


            Patient patient = PatientDB.GetByID(patient_id);

            UpdateConditions(patient);


            // add contact info

            if (UserView.GetInstance().IsAgedCareView && ddlOrganisationAC.SelectedValue != "-1")
                ac_register_patient_id = RegisterPatientDB.Insert(Convert.ToInt32(ddlOrganisationAC.SelectedValue), patient.PatientID);

            if (Utilities.GetAddressType().ToString() == "Contact")
            {
                if (userView.IsAgedCareView && txtACRoom.Text.Trim().Length > 0)
                    bedroom_id = ContactDB.Insert(patient.Person.EntityID,
                        166,
                        txtACRoomNote.Text.Trim(),
                        txtACRoom.Text.Trim(),
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);

                if (txtAddressAddrLine1.Text.Trim().Length > 0 || txtAddressAddrLine2.Text.Trim().Length > 0)
                    address_id = ContactDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlAddressContactType.SelectedValue),
                        txtAddressFreeText.Text,
                        txtAddressAddrLine1.Text,
                        txtAddressAddrLine2.Text,
                        Convert.ToInt32(ddlAddressAddressChannel.SelectedValue),
                        //Convert.ToInt32(ddlAddressSuburb.SelectedValue),
                        Convert.ToInt32(suburbID.Value),
                        Convert.ToInt32(ddlAddressCountry.SelectedValue),
                        Convert.ToInt32(Session["SiteID"]),
                        chkIsBilling.Checked,
                        chkIsNonBilling.Checked);

                if (txtPhoneNumber1.Text.Trim().Length > 0)
                    phone_id1 = ContactDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlPhoneNumber1.SelectedValue),
                        txtPhoneNumber1FreeText.Text,
                        System.Text.RegularExpressions.Regex.Replace(txtPhoneNumber1.Text, "[^0-9]", ""),
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);

                if (txtPhoneNumber2.Text.Trim().Length > 0)
                    phone_id2 = ContactDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlPhoneNumber2.SelectedValue),
                        txtPhoneNumber2FreeText.Text,
                        System.Text.RegularExpressions.Regex.Replace(txtPhoneNumber2.Text, "[^0-9]", ""),
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);

                if (txtPhoneNumber3.Text.Trim().Length > 0)
                    phone_id3 = ContactDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlPhoneNumber3.SelectedValue),
                        txtPhoneNumber3FreeText.Text,
                        System.Text.RegularExpressions.Regex.Replace(txtPhoneNumber3.Text, "[^0-9]", ""),
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);

                if (txtEmailAddrLine1.Text.Trim().Length > 0)
                    email_id = ContactDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlEmailContactType.SelectedValue),
                        "",
                        txtEmailAddrLine1.Text,
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);
            }
            else if (Utilities.GetAddressType().ToString() == "ContactAus")
            {
                if (UserView.GetInstance().IsAgedCareView && txtACRoom.Text.Trim().Length > 0)
                    bedroom_id = ContactAusDB.Insert(patient.Person.EntityID,
                        166,
                        txtACRoomNote.Text.Trim(),
                        txtACRoom.Text.Trim(),
                        string.Empty,
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);

                if (txtAddressAddrLine1.Text.Trim().Length > 0 || txtAddressAddrLine2.Text.Trim().Length > 0)
                    address_id = ContactAusDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlAddressContactType.SelectedValue),
                        txtAddressFreeText.Text,
                        txtAddressAddrLine1.Text,
                        txtAddressAddrLine2.Text,
                        txtStreet.Text,
                        Convert.ToInt32(ddlAddressAddressChannelType.SelectedValue),
                        //Convert.ToInt32(ddlAddressSuburb.SelectedValue),
                        Convert.ToInt32(suburbID.Value),
                        Convert.ToInt32(ddlAddressCountry.SelectedValue),
                        Convert.ToInt32(Session["SiteID"]),
                        chkIsBilling.Checked,
                        chkIsNonBilling.Checked);

                if (txtPhoneNumber1.Text.Trim().Length > 0)
                    phone_id1 = ContactAusDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlPhoneNumber1.SelectedValue),
                        txtPhoneNumber1FreeText.Text,
                        System.Text.RegularExpressions.Regex.Replace(txtPhoneNumber1.Text, "[^0-9]", ""),
                        string.Empty,
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);

                if (txtPhoneNumber2.Text.Trim().Length > 0)
                    phone_id2 = ContactAusDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlPhoneNumber2.SelectedValue),
                        txtPhoneNumber2FreeText.Text,
                        System.Text.RegularExpressions.Regex.Replace(txtPhoneNumber2.Text, "[^0-9]", ""),
                        string.Empty,
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);

                if (txtPhoneNumber3.Text.Trim().Length > 0)
                    phone_id3 = ContactAusDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlPhoneNumber3.SelectedValue),
                        txtPhoneNumber3FreeText.Text,
                        System.Text.RegularExpressions.Regex.Replace(txtPhoneNumber3.Text, "[^0-9]", ""),
                        string.Empty,
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);

                if (txtEmailAddrLine1.Text.Trim().Length > 0)
                    email_id = ContactAusDB.Insert(patient.Person.EntityID,
                        Convert.ToInt32(ddlEmailContactType.SelectedValue),
                        "",
                        txtEmailAddrLine1.Text,
                        string.Empty,
                        string.Empty,
                        -1,
                        -1,
                        -1,
                        Convert.ToInt32(Session["SiteID"]),
                        true,
                        true);
            }
            else
            {
                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
            }

            contacts_added = true;

            // add healthcard

            if (txtHealthCardCardName.Text.Trim().Length > 0 || txtHealthCardCardNbr.Text.Trim().Length > 0 ||
                txtHealthCardCardNbr_Digit_1.Text.Trim().Length > 0  ||
                txtHealthCardCardNbr_Digit_2.Text.Trim().Length > 0  ||
                txtHealthCardCardNbr_Digit_3.Text.Trim().Length > 0  ||
                txtHealthCardCardNbr_Digit_4.Text.Trim().Length > 0  ||
                txtHealthCardCardNbr_Digit_5.Text.Trim().Length > 0  ||
                txtHealthCardCardNbr_Digit_6.Text.Trim().Length > 0  ||
                txtHealthCardCardNbr_Digit_7.Text.Trim().Length > 0  ||
                txtHealthCardCardNbr_Digit_8.Text.Trim().Length > 0  ||
                txtHealthCardCardNbr_Digit_9.Text.Trim().Length > 0  ||
                txtHealthCardCardNbr_Digit_10.Text.Trim().Length > 0)
            {
                int organisation_id = Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue);

                string cardNbr;
                if (organisation_id == -1)
                {
                    cardNbr =
                        txtHealthCardCardNbr_Digit_1.Text.PadLeft(1, ' ') + txtHealthCardCardNbr_Digit_2.Text.PadLeft(1, ' ') + txtHealthCardCardNbr_Digit_3.Text.PadLeft(1, ' ') + txtHealthCardCardNbr_Digit_4.Text.PadLeft(1, ' ') + txtHealthCardCardNbr_Digit_5.Text.PadLeft(1, ' ') +
                        txtHealthCardCardNbr_Digit_6.Text.PadLeft(1, ' ') + txtHealthCardCardNbr_Digit_7.Text.PadLeft(1, ' ') + txtHealthCardCardNbr_Digit_8.Text.PadLeft(1, ' ') + txtHealthCardCardNbr_Digit_9.Text.PadLeft(1, ' ') + txtHealthCardCardNbr_Digit_10.Text.PadLeft(1, ' ');
                }
                else if (organisation_id == -2)
                    cardNbr = Regex.Replace(txtHealthCardCardNbr.Text, @"[^a-zA-Z0-9]", "");
                else
                    throw new Exception("Unknown organisation id for healthcard card : " + organisation_id);


                if ((ddlHealthCardCardExpiry_Year.SelectedValue  != "-1" && ddlHealthCardCardExpiry_Month.SelectedValue == "-1") ||
                    (ddlHealthCardCardExpiry_Month.SelectedValue != "-1" && ddlHealthCardCardExpiry_Year.SelectedValue  == "-1"))
                    throw new Exception("Health card expiry date Must Be Both Set or Both Unset");

                DateTime expiryDate = ddlHealthCardCardExpiry_Year.SelectedValue == "-1" || ddlHealthCardCardExpiry_Month.SelectedValue == "-1" ?
                                      DateTime.MinValue :
                                      new DateTime(Convert.ToInt32(ddlHealthCardCardExpiry_Year.SelectedValue), Convert.ToInt32(ddlHealthCardCardExpiry_Month.SelectedValue), 1);

                string cardFamilyMemberNbr = (organisation_id == -1) ? ddlHealthCardCardFamilyMemberNbr.SelectedValue : string.Empty;
                healthcard_id = HealthCardDB.Insert(patient.PatientID, organisation_id, txtHealthCardCardName.Text.Trim(), cardNbr, cardFamilyMemberNbr, expiryDate, DateTime.MinValue, DateTime.MinValue, true, Convert.ToInt32(Session["StaffID"]), "");
            }

            healthcard_added = true;


            // remove session datatable of patient in search lists so it is regenereated
            Session.Remove("patientinfo_data"); 


            if (nextScreen == "AddAndGoToBookingScreen")
            {
                if (userView.IsProviderView || userView.IsExternalView)
                {
                    Response.Redirect("~/BookingsV2.aspx?orgs=" + Session["OrgID"] + "&ndays=1&patient=" + patient_id, false);
                    return;
                }
                else
                {
                    Response.Redirect("~/SelectOrganisationsV2.aspx?patient=" + patient_id, false);
                    return;
                }
            }
            else if (nextScreen == "AddAndAddAonther")
            {
                Response.Redirect("~/PatientAddV2.aspx", false);
                return;
            }
            else if (nextScreen == "AddAndGoToHealthCardScreen" && healthcard_id > 0)
            {
                Response.Redirect("~/HealthCardDetailV2.aspx?type=view&id=" + healthcard_id + "&card=" + (Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue) == -1 ? "medicare" : "dva"), false);
                return;
            }
            else // if (nextScreen == "AddAndGoToViewScreen")
            {
                string url = "~/PatientDetailV2.aspx?type=view&id=" + patient_id.ToString();
                Response.Redirect(url, false);
                return;
            }

        }
        catch (Exception ex)
        {
            if (patient_added && contacts_added && healthcard_added)
            {

                if (nextScreen == "AddAndGoToBookingScreen")
                {
                    Response.Redirect("~/SelectOrganisationsV2.aspx?patient=" + patient_id);
                    return;
                }
                else if (nextScreen == "AddAndGoToHealthCardScreen" && healthcard_id > 0)
                {
                    Response.Redirect("~/HealthCardDetailV2.aspx?type=view&id=" + healthcard_id + "&card=" + (Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue) == -1 ? "medicare" : "dva"));
                    return;
                }
                else // if (nextScreen == "AddAndGoToViewScreen")
                {
                    string url = Request.RawUrl;
                    url = UrlParamModifier.AddEdit(url, "type", "view");
                    url = UrlParamModifier.AddEdit(url, "id", patient_id.ToString());
                    Response.Redirect(url);
                    return;
                }

            }

            // roll back - backwards of creation order

            HealthCardDB.Delete(healthcard_id);

            if (Utilities.GetAddressType().ToString() == "Contact")
            {
                ContactDB.Delete(address_id);
                ContactDB.Delete(phone_id1);
                ContactDB.Delete(phone_id2);
                ContactDB.Delete(phone_id3);
                ContactDB.Delete(email_id);
                ContactDB.Delete(bedroom_id);
            }
            else if (Utilities.GetAddressType().ToString() == "ContactAus")
            {
                ContactAusDB.Delete(address_id);
                ContactAusDB.Delete(phone_id1);
                ContactAusDB.Delete(phone_id2);
                ContactAusDB.Delete(phone_id3);
                ContactAusDB.Delete(email_id);
                ContactAusDB.Delete(bedroom_id);
            }
            else
                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

            RegisterPatientDB.Delete(ac_register_patient_id);

            PatientConditionDB.DeleteByPatient(patient_id);

            RegisterPatientDB.Delete(register_patient_id);
            PatientConditionDB.DeleteByPatient(patient_id);
            PatientDB.Delete(patient_id);
            PersonDB.Delete(person_id);

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                UserDatabaseMapperDB.Delete(mainDbUserID);

            if (ex is CustomMessageException)
                SetErrorMessage(((CustomMessageException)ex).Message);
            else
                HideTableAndSetErrorMessage("", ex.ToString());
        }


    }

    #endregion


    #region GrdCondition

    protected void FillGrdCondition()
    {

        DataTable dt = ConditionDB.GetDataTable(false);
        System.Collections.Hashtable conditionHash = ConditionDB.GetHashtable();

        if (dt.Rows.Count > 0)
        {
            GrdCondition.DataSource = dt;
            try
            {
                GrdCondition.DataBind();

                foreach (GridViewRow row in GrdCondition.Rows)
                {
                    Label lblId = row.FindControl("lblId") as Label;
                    CheckBox chkSelect = row.FindControl("chkSelect") as CheckBox;
                    DropDownList ddlDate_Day       = (DropDownList)row.FindControl("ddlDate_Day");
                    DropDownList ddlDate_Month     = (DropDownList)row.FindControl("ddlDate_Month");
                    DropDownList ddlDate_Year      = (DropDownList)row.FindControl("ddlDate_Year");
                    DropDownList ddlNbrWeeksDue    = (DropDownList)row.FindControl("ddlNbrWeeksDue");

                    Label        lblNextDue        = row.FindControl("lblNextDue") as Label;
                    Label        lblWeeksLater     = row.FindControl("lblWeeksLater") as Label;
                    Label        lblAdditionalInfo = row.FindControl("lblAdditionalInfo") as Label;
                    

                    System.Web.UI.HtmlControls.HtmlControl br_date      = (System.Web.UI.HtmlControls.HtmlControl)row.FindControl("br_date");
                    System.Web.UI.HtmlControls.HtmlControl br_nweeksdue = (System.Web.UI.HtmlControls.HtmlControl)row.FindControl("br_nweeksdue");
                    System.Web.UI.HtmlControls.HtmlControl br_text      = (System.Web.UI.HtmlControls.HtmlControl)row.FindControl("br_text");

                    TextBox txtText = (TextBox)row.FindControl("txtText");

                    if (lblId == null || chkSelect == null)
                        continue;

                    Condition condition = (Condition)conditionHash[Convert.ToInt32(lblId.Text)];

                    br_date.Visible             = condition.ShowDate;
                    ddlDate_Day.Visible         = condition.ShowDate;
                    ddlDate_Month.Visible       = condition.ShowDate;
                    ddlDate_Year.Visible        = condition.ShowDate;
                    br_nweeksdue.Visible        = condition.ShowNWeeksDue;
                    ddlNbrWeeksDue.Visible      = condition.ShowNWeeksDue;
                    lblNextDue.Visible          = condition.ShowNWeeksDue;
                    lblWeeksLater.Visible       = condition.ShowNWeeksDue;
                    br_text.Visible             = condition.ShowText;
                    txtText.Visible             = condition.ShowText;
                    lblAdditionalInfo.Visible   = condition.ShowText;
                }

            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }

            //Sort("parent_descr", "ASC");
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdCondition.DataSource = dt;
            GrdCondition.DataBind();

            int TotalColumns = GrdCondition.Rows[0].Cells.Count;
            GrdCondition.Rows[0].Cells.Clear();
            GrdCondition.Rows[0].Cells.Add(new TableCell());
            GrdCondition.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdCondition.Rows[0].Cells[0].Text = "No Conditions";
        }
    }
    protected void GrdCondition_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdCondition_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");

            DropDownList ddlDate_Day    = (DropDownList)e.Row.FindControl("ddlDate_Day");
            DropDownList ddlDate_Month  = (DropDownList)e.Row.FindControl("ddlDate_Month");
            DropDownList ddlDate_Year   = (DropDownList)e.Row.FindControl("ddlDate_Year");
            DropDownList ddlNbrWeeksDue = (DropDownList)e.Row.FindControl("ddlNbrWeeksDue");

            ddlDate_Day.Items.Add(new ListItem("--", "-1"));
            ddlDate_Month.Items.Add(new ListItem("--", "-1"));
            ddlDate_Year.Items.Add(new ListItem("--", "-1"));
            for (int i = 1; i <= 31; i++)
                ddlDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 1; i <= 12; i++)
                ddlDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                ddlDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

            for (int i = 0; i <= 52; i++)
                ddlNbrWeeksDue.Items.Add(new ListItem(i.ToString(), i.ToString()));
            


            Utilities.AddConfirmationBox(e);
            Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow, false);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdCondition_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }
    protected void GrdCondition_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdCondition_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdCondition_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdCondition_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GrdCondition_Sorting(object sender, GridViewSortEventArgs e)
    {
    }

    #endregion

    #region UpdateConditions

    protected void UpdateConditions(Patient patient)
    {
        bool tblEmpty = GrdCondition.Rows.Count == 1 && GrdCondition.Rows[0].Cells[0].Text == "No Conditions";
        if (tblEmpty)
            return;

        foreach (GridViewRow row in GrdCondition.Rows)
        {
            Label        lblId          = row.FindControl("lblId") as Label;
            CheckBox     chkSelect      = row.FindControl("chkSelect") as CheckBox;
            DropDownList ddlDate_Day    = (DropDownList)row.FindControl("ddlDate_Day");
            DropDownList ddlDate_Month  = (DropDownList)row.FindControl("ddlDate_Month");
            DropDownList ddlDate_Year   = (DropDownList)row.FindControl("ddlDate_Year");
            DropDownList ddlNbrWeeksDue = (DropDownList)row.FindControl("ddlNbrWeeksDue");
            TextBox      txtText        = (TextBox)row.FindControl("txtText");

            Label        lblDescr       = (Label)row.FindControl("lblDescr");

            if (lblId == null || chkSelect == null)
                continue;


            DateTime date = DateTime.MinValue;
            if (ddlDate_Day.Visible && ddlDate_Month.Visible && ddlDate_Year.Visible)
            {
                if (!IsValidDate(Convert.ToInt32(ddlDate_Day.Text), Convert.ToInt32(ddlDate_Month.Text), Convert.ToInt32(ddlDate_Year.Text)))
                    throw new CustomMessageException("For condition '" + lblDescr.Text + "' please enter a valid date or unset all");

                date = GetDateFromForm(Convert.ToInt32(ddlDate_Day.Text), Convert.ToInt32(ddlDate_Month.Text), Convert.ToInt32(ddlDate_Year.Text));
            }

            int nweeksdue = ddlNbrWeeksDue.Visible ? Convert.ToInt32(ddlNbrWeeksDue.Text) : 0;
            string text = txtText.Visible ? txtText.Text.Trim() : string.Empty;

            if (chkSelect.Checked)
                PatientConditionDB.Insert(patient.PatientID, Convert.ToInt32(lblId.Text), date, nweeksdue, text, false);
        }

    }

    public DateTime GetDateFromForm(int day, int month, int year)
    {
        if (day == -1 && month == -1 && year == -1)
            return DateTime.MinValue;

        else if (day != -1 && month != -1 && year != -1)
            return new DateTime(year, month, day);

        else
            throw new  CustomMessageException("Date format is some selected and some not selected.");
    }
    public bool IsValidDate(int day, int month, int year)
    {
        bool isvalid = ((day == -1 && month == -1 && year == -1) ||
                        (day != -1 && month != -1 && year != -1 && _IsValidDate(day, month, year)));
        return isvalid;
    }
    private bool _IsValidDate(int day, int month, int year)
    {
        try
        {
            DateTime dt = new DateTime(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion



    #region GetDOBFromForm, GetDARevFromForm, GetDate, IsValidDate

    public DateTime GetDOBFromForm()
    {
        return GetDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue, "DOB");
    }
    public DateTime GetDARevFromForm()
    {
        return GetDate(ddlDARev_Day.SelectedValue, ddlDARev_Month.SelectedValue, ddlDARev_Year.SelectedValue, "DA Rev");
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

        try
        {
            if (day != "-1" || month != "-1" || year != "-1")
            {
                DateTime dt = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
            }
        }
        catch(Exception)
        {
            invalid = true;
        }

        return !invalid;
    }

    #endregion

    #region btnSuburbSelectionUpdate_Click

    protected void btnSuburbSelectionUpdate_Click(object sender, EventArgs e)
    {
        UpdateSuburbInfo(true);
    }

    protected void UpdateSuburbInfo(bool redirect)
    {
        return;

        int newSuburbID = Convert.ToInt32(suburbID.Value);

        if (newSuburbID == -1)
        {
            lblSuburbText.Text = "--";
        }
        else
        {
            Suburb suburb = SuburbDB.GetByID(newSuburbID);
            lblSuburbText.Text = suburb.Name + ", " + suburb.State + "(" + suburb.Postcode + ")";
        }

        if (redirect)
        {
            string url = Request.RawUrl;
            url = UrlParamModifier.Update(newSuburbID != -1, url, "suburb", newSuburbID == -1 ? "" : newSuburbID.ToString());
            Response.Redirect(url);
        }
    }

    #endregion


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