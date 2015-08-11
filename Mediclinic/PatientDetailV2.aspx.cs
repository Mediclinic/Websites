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

public partial class PatientDetailV2 : System.Web.UI.Page
{
    #region Page_Load

    protected bool debugPageLoadTime = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // if EPC info updated to be new EPC, we need to make "last letters" for referrers
            // this is done in the 'edit' part of the btnSubmit_Click method, and put in session variables so when it reloads to this page, we can popup the download window 
            if (!IsPostBack && Session["downloadFile_Contents"] != null && Session["downloadFile_DocName"] != null)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "download", "<script language=javascript>window.open('DownloadFile.aspx','_blank','status=1,toolbar=0,menubar=0,location=1,scrollbars=1,resizable=1,width=30,height=30');</script>");
            else
            {
                Session.Remove("downloadFile_Contents");
                Session.Remove("downloadFile_DocName");
            }


            //if (!IsPostBack)
            //    Utilities.SetNoCache(Response);
            HideErrorMessage();


            if (!IsPostBack)
            {

                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, true);
                hiddenIsMobileDevice.Value = Utilities.IsMobileDevice(Request) ? "1" : "0";

                UserView userView = UserView.GetInstance();
                UrlParamType urlParamType = GetUrlParamType();

                SetupGUI();

                if (urlParamType == UrlParamType.None)
                    throw new CustomMessageException("No url parameter 'type'");
                if (!IsValidFormID())
                    throw new CustomMessageException("No valid id in url");


                Patient patient = PatientDB.GetByID(GetFormID());
                if (patient == null)
                    throw new Exception("Invalid id in url");

                if (patient.IsDeleted && urlParamType == UrlParamType.Edit && !userView.IsAdminView)
                    urlParamType = UrlParamType.View;

                if (patient.IsGPPatient)
                    Response.Redirect(Request.RawUrl.Replace("PatientDetailV2.aspx", "PatientDetailV3.aspx"));

                // add to logged in providers org, then redirect back without add_to_this_org in url
                if (Request.QueryString["add_to_this_org"] != null)
                {
                    if (!userView.IsAdminView && !RegisterPatientDB.IsPatientRegisteredToOrg(patient.PatientID, Convert.ToInt32(Session["OrgID"])))
                        RegisterPatientDB.Insert(Convert.ToInt32(Session["OrgID"]), patient.PatientID);
                    Response.Redirect(UrlParamModifier.Remove(Request.RawUrl, "add_to_this_org"));
                }


                if (userView.IsAgedCareView && !RegisterPatientDB.IsAgedCarePatient(patient.PatientID))
                    SetErrorMessage("<b>WARNING: Please attach a facility or wing in the &nbsp;'Registered To'&nbsp; section below or this resident will not be visible in Aged Care.</b>");


                Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
                PatientDB.AddACOfferings(ref offeringsHash, ref patient);

                if (debugPageLoadTime) Logger.LogQuery("A1", false, false, true);
                FillEditViewForm(patient, urlParamType == UrlParamType.Edit);

                if (debugPageLoadTime) Logger.LogQuery("A2", false, false, true);
                GrdCondition.Visible = false;
                GrdConditionView.Visible = false;
                if (urlParamType == UrlParamType.Edit)
                {
                    GrdCondition.Visible = true;
                    FillGrdCondition(patient);
                }
                else if (urlParamType == UrlParamType.View)
                {
                    GrdConditionView.Visible = true;
                    FillGrdConditionView(patient);
                }

                if (debugPageLoadTime) Logger.LogQuery("A3", false, false, true);
                if (urlParamType == UrlParamType.Edit)
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "setcursor", "<script language=javascript>SetCursorToTextEnd('txtFirstname');</script>");

                if (Utilities.GetAddressType().ToString() == "Contact")
                {
                    addressControl.Visible = true;
                    addressControl.Set(patient.Person.EntityID, true, EntityType.GetByType(EntityType.EntityTypeEnum.Patient), userView.IsAgedCareView);
                }
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                {
                    addressAusControl.Visible = true;
                    addressAusControl.Set(patient.Person.EntityID, true, EntityType.GetByType(EntityType.EntityTypeEnum.Patient), userView.IsAgedCareView);
                }
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
                if (debugPageLoadTime) Logger.LogQuery("A4", false, false, true);
                patientReferrer.SetInfo(patient.PatientID, urlParamType.ToString());
                if (debugPageLoadTime) Logger.LogQuery("A5", false, false, true);
                healthCardInfoControl.SetInfo(patient.PatientID, false, true, true, false, false, true, true);
                if (debugPageLoadTime) Logger.LogQuery("A6", false, false, true);

                //txtFirstname.Focus();
            }
            else
                GrdRegistration_Reset();

            this.GrdCredit.EnableViewState = true;
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

    #region GetUrlParamType(), IsValidFormID(), GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new CustomMessageException("Invalid url id");

        string id = Request.QueryString["id"];
        return Convert.ToInt32(id);
    }

    private enum UrlParamType { Edit, View, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "view")
            return UrlParamType.View;
        else
            return UrlParamType.None;
    }

    #endregion
    
    #region SetGUI

    private void SetupGUI()
    {
        UserView userView = UserView.GetInstance();

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

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate',   this, 'dmy', '-'); return false;";


        ddlConcessionCardExpiry_Month.Items.Add(new ListItem("--", "-1"));
        ddlConcessionCardExpiry_Year.Items.Add(new ListItem("--", "-1"));

        for (int i = 1; i <= 12; i++)
            ddlConcessionCardExpiry_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = DateTime.Today.Year - 10; i <= DateTime.Today.Year + 10; i++)
            ddlConcessionCardExpiry_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

        acTypeRow.Visible = userView.IsAgedCareView;

        if (userView.IsExternalView)
        {
            td_conditions_and_notes.Visible = td_conditions_and_notes_space_before.Visible = false;
            td_hc_cards_and_links.Visible   = td_hc_cards_and_links_space_before.Visible   = false;
        }


        lblLetterBestPrintHistorySeperator.Visible = Session["DB"].ToString() == "Mediclinic_0001";
        lnkLetterBestPrintHistory.Visible          = Session["DB"].ToString() == "Mediclinic_0001";


        bool editable = GetUrlParamType() == UrlParamType.Edit;
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

    #region FillEditViewForm

    private void FillEditViewForm(Patient patient, bool isEditMode)
    {
        UserView userView = UserView.GetInstance();


        Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
        PatientDB.AddACOfferings(ref offeringsHash, ref patient);


        patient.Person = PersonDB.GetByID(patient.Person.PersonID);
        Person addedBy = patient.Person.PersonID < 0 ? null : PersonDB.GetByID(patient.Person.AddedBy);

        lblHeading.Text = patient.Person.FullnameWithoutMiddlename + (patient.Person.Nickname.Length == 0 ? "" : " (" + patient.Person.Nickname + ")");
        lblPatientID.Text = "[ID:" + patient.PatientID + "]";
        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + patient.Person.FullnameWithoutMiddlename;

        string checkStillLoggedIn = "if (ajax_check_session_timed_out()) {window.location.href = window.location.href;return false;}";
        string confirmNotBkNotesForProviders = (userView.IsAdminView ? "" : "if (!confirm('These are NOT booking notes. Are you sure you want to update PATIENT notes?')) return false;");
        string js = "javascript:" + checkStillLoggedIn + confirmNotBkNotesForProviders + "addEditNotes(" + patient.Person.EntityID.ToString() + ");return false;";
        string jsBodyChart = "javascript:" + checkStillLoggedIn + confirmNotBkNotesForProviders + "addEditBCNotes(" + patient.Person.EntityID.ToString() + ");return false;";
        this.lnkNotes.Attributes["onclick"] = js;
        this.lnkNotesBodyChart.Attributes["onclick"] = jsBodyChart;
        string jsAddVoucher = "javascript:" + checkStillLoggedIn + "addVoucher(" + patient.Person.EntityID.ToString() + ");return false;";
        this.lnkAddVoucher.Attributes["onclick"] = jsAddVoucher;



        patient.FlashingText = patient.FlashingText.Replace(Environment.NewLine, "<br />");

        string allFeaturesFlashingText = "dialogWidth:525px;dialogHeight:255px;center:yes;resizable:no; scroll:no";
        string jsFlashingText = Utilities.IsMobileDevice(Request) ?
            "javascript: document.getElementById('lblFlashingText').style.display = ''; open_new_tab('" + "PatientFlashingMessageDetailV2.aspx?type=edit&id=" + patient.PatientID.ToString() + "');return false;"
            :
            "javascript: document.getElementById('lblFlashingText').style.display = ''; window.showModalDialog('" + "PatientFlashingMessageDetailV2.aspx?type=edit&id=" + patient.PatientID.ToString() + "', '', '" + allFeaturesFlashingText + "');return false;";
        this.lnkFlashingText.Attributes.Add("onclick", jsFlashingText);
        this.lnkFlashingText.ImageUrl = "~/images/asterisk_12.png";

        if (patient.FlashingTextAddedBy != null)
            patient.FlashingTextAddedBy = StaffDB.GetByID(patient.FlashingTextAddedBy.StaffID);
        string addedByHoverLink = patient.FlashingTextAddedBy == null ? "" : @"<a href=""javascript:void(0)"" onclick=""javascript:return false;"" title='Added By: " + patient.FlashingTextAddedBy.Person.FullnameWithoutMiddlename + @"' style=""text-decoration: none"">*</a>";
        this.lblFlashingText.Text = (patient.FlashingText.Length == 0 || patient.FlashingTextLastModifiedDate == DateTime.MinValue ? "" : @"<span style=""font-size:70%;font-weight:normal;"">" + "[" + patient.FlashingTextLastModifiedDate.ToString("d'/'M'/'yyyy") + addedByHoverLink + "]</span> ") + patient.FlashingText;
        //this.lblFlashingText.Attributes.Add("onclick", jsFlashingText);


        if (!Utilities.IsDev())
            idRow.Attributes["class"] = "hiddencol";

        this.lnkMakeBooking.Visible = true;
        this.lnkMakeBooking.Text = "Book Other Clinic";
        this.lnkMakeBooking.NavigateUrl = userView.IsAgedCareView ?
            "~/SelectOrganisationsV2.aspx" :
            "~/SelectOrganisationsV2.aspx?patient=" + GetFormID().ToString();




        this.lnkBookingList.Visible = true;
        this.lnkBookingList.Text = "Bookings List";
        this.lnkBookingList.NavigateUrl = "~/BookingsListV2.aspx?patient=" + GetFormID().ToString();

        this.lnkInvoices.Visible = true;
        this.lnkInvoices.Text = "View Invoices";
        this.lnkInvoices.NavigateUrl = "~/InvoiceListV2.aspx?patient=" + GetFormID().ToString();

        this.lnkOrders.Visible = true;
        this.lnkOrders.Text = "View Orders";
        this.lnkOrders.NavigateUrl = "~/OfferingOrderListV2.aspx?patient=" + GetFormID().ToString();


        if (debugPageLoadTime) Logger.LogQuery("-B1", false, false, true);
        UpdateScannedDocumentsCount();
        if (debugPageLoadTime) Logger.LogQuery("-B2", false, false, true);


        Booking mostRecentBooking = BookingDB.GetMostRecent(patient.PatientID);
        this.lnkLastBooking.Text = mostRecentBooking == null ? "None" : mostRecentBooking.DateStart.ToString("d MMM yyyy");
        this.lnkLastBooking.NavigateUrl = mostRecentBooking == null ? "javascript:return false;" : mostRecentBooking.GetBookingSheetLinkV2();

        Booking nextBooking = BookingDB.GetNextAfterNow(patient.PatientID);
        this.lnkNextBooking.Text = nextBooking == null ? "None" : nextBooking.DateStart.ToString("d MMM yyyy");
        this.lnkNextBooking.NavigateUrl = nextBooking == null ? "javascript:return false;" : nextBooking.GetBookingSheetLinkV2();

        if (debugPageLoadTime) Logger.LogQuery("-B3", false, false, true);
        this.lnkLastBooking.Enabled = mostRecentBooking != null;
        if (mostRecentBooking == null)
            this.lnkLastBooking.Style["text-decoration"] = "none";
        else
            this.lnkLastBooking.Style.Remove("text-decoration");

        this.lnkNextBooking.Enabled = nextBooking != null;
        if (nextBooking == null)
            this.lnkNextBooking.Style["text-decoration"] = "none";
        else
            this.lnkNextBooking.Style.Remove("text-decoration");



        this.lnkLetterPrintHistory.NavigateUrl = String.Format("~/Letters_SentHistoryV2.aspx?patient={0}", patient.PatientID);
        this.lnkLetterBestPrintHistory.NavigateUrl = String.Format("~/LettersBest_SentHistoryV2.aspx?patient={0}", patient.PatientID);
        this.lnkPrintLetter.NavigateUrl = String.Format("~/Letters_PrintV2.aspx?patient={0}", patient.PatientID);


        lblId.Text = patient.PatientID.ToString();
        lblAddedBy.Text = addedBy == null ? "" : addedBy.Firstname + " " + addedBy.Surname;
        lblPatientDateAdded.Text = patient.PatientDateAdded.ToString("dd-MM-yyyy");

        btnDeleteUndeletePatient.CommandName = patient.IsDeleted ? "UnDelete" : "Delete";
        btnDeleteUndeletePatient.Text = patient.IsDeleted ? "Un-Archive" : "Archive";
        lblPatientStatus.Text = patient.IsDeleted ? "<b><font color=\"red\">Archived</font></b>" : "Active";

        btnHistory.OnClientClick = @"javascript:open_new_tab('/PatientEditHistoryV2.aspx?id=" + patient.PatientID + @"');return false;";
        changeHistoryLinknRow.Visible = PatientHistoryDB.Exists(patient.PatientID);


        if (isEditMode)
        {
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

            ddlTitle.SelectedValue = patient.Person.Title.ID.ToString();
            txtFirstname.Text = patient.Person.Firstname;
            txtMiddlename.Text = patient.Person.Middlename;
            txtSurname.Text = patient.Person.Surname;
            txtNickname.Text = patient.Person.Nickname;
            if (ddlGender.Items.FindByValue(patient.Person.Gender) == null)
                ddlGender.Items.Add(new ListItem(patient.Person.Gender == "" ? "--" : patient.Person.Gender, patient.Person.Gender));
            ddlGender.SelectedValue = patient.Person.Gender;

            if (patient.Person.Dob != DateTime.MinValue)
            {
                ddlDOB_Day.SelectedValue = patient.Person.Dob.Day.ToString();
                ddlDOB_Month.SelectedValue = patient.Person.Dob.Month.ToString();

                int firstYearSelectable = Convert.ToInt32(ddlDOB_Year.Items[1].Value);
                int lastYearSelectable = Convert.ToInt32(ddlDOB_Year.Items[ddlDOB_Year.Items.Count - 1].Value);
                if (patient.Person.Dob.Year < firstYearSelectable)
                    ddlDOB_Year.Items.Insert(1, new ListItem(patient.Person.Dob.Year.ToString(), patient.Person.Dob.Year.ToString()));
                if (patient.Person.Dob.Year > lastYearSelectable)
                    ddlDOB_Year.Items.Add(new ListItem(patient.Person.Dob.Year.ToString(), patient.Person.Dob.Year.ToString()));

                ddlDOB_Year.SelectedValue = patient.Person.Dob.Year.ToString();
            }
            if (patient.DiabeticAAassessmentReviewDate != DateTime.MinValue)
            {
                ddlDARev_Day.SelectedValue = patient.DiabeticAAassessmentReviewDate.Day.ToString();
                ddlDARev_Month.SelectedValue = patient.DiabeticAAassessmentReviewDate.Month.ToString();

                int firstYearSelectable = Convert.ToInt32(ddlDARev_Year.Items[1].Value);
                int lastYearSelectable = Convert.ToInt32(ddlDARev_Year.Items[ddlDARev_Year.Items.Count - 1].Value);
                if (patient.DiabeticAAassessmentReviewDate.Year < firstYearSelectable)
                    ddlDARev_Year.Items.Insert(1, new ListItem(patient.DiabeticAAassessmentReviewDate.Year.ToString(), patient.DiabeticAAassessmentReviewDate.Year.ToString()));
                if (patient.DiabeticAAassessmentReviewDate.Year > lastYearSelectable)
                    ddlDARev_Year.Items.Add(new ListItem(patient.DiabeticAAassessmentReviewDate.Year.ToString(), patient.DiabeticAAassessmentReviewDate.Year.ToString()));

                ddlDARev_Year.SelectedValue = patient.DiabeticAAassessmentReviewDate.Year.ToString();
            }

            chkIsClinicPatient.Checked = patient.IsClinicPatient;
            chkIsDeceased.Checked = patient.IsDeceased;
            chkIsDiabetic.Checked = patient.IsDiabetic;
            chkIsMemberDiabetesAustralia.Checked = patient.IsMemberDiabetesAustralia;

            txtPrivateHealthFund.Text = patient.PrivateHealthFund;
            txtConcessionCardNbr.Text = patient.ConcessionCardNumber;
            if (patient.ConcessionCardExpiryDate != DateTime.MinValue)
            {
                Utilities.AddIfNotExists(ddlConcessionCardExpiry_Month, patient.ConcessionCardExpiryDate.Month);
                ddlConcessionCardExpiry_Month.SelectedValue = patient.ConcessionCardExpiryDate.Month.ToString();

                Utilities.AddIfNotExists(ddlConcessionCardExpiry_Year, patient.ConcessionCardExpiryDate.Year);
                ddlConcessionCardExpiry_Year.SelectedValue = patient.ConcessionCardExpiryDate.Year.ToString();
            }

            int ac_inv_offering_id = patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID;
            int ac_pat_offering_id = patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID;
            string ac_inv_offering_name = patient.ACInvOffering == null ? string.Empty : patient.ACInvOffering.Name;
            string ac_pat_offering_name = patient.ACPatOffering == null ? string.Empty : patient.ACPatOffering.Name;

            int ac_inv_aged_care_patient_type_id = patient.ACInvOffering == null ? -1 : patient.ACInvOffering.AgedCarePatientType.ID;
            string ac_inv_aged_care_patient_type_descr = patient.ACInvOffering == null ? string.Empty : patient.ACInvOffering.AgedCarePatientType.Descr;
            int ac_pat_aged_care_patient_type_id = patient.ACInvOffering == null ? -1 : patient.ACPatOffering.AgedCarePatientType.ID;
            string ac_pat_aged_care_patient_type_descr = patient.ACInvOffering == null ? string.Empty : patient.ACPatOffering.AgedCarePatientType.Descr;

            DataTable dt_offerings = OfferingDB.GetDataTable(true, -1, null, true);
            for (int i = dt_offerings.Rows.Count - 1; i >= 0; i--)
            {
                Offering o = OfferingDB.LoadAll(dt_offerings.Rows[i]);
                int o_id = o.OfferingID;
                int o_ac_pt_id = Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"]);

                if (Convert.ToInt32(dt_offerings.Rows[i]["o_offering_id"]) != ac_inv_offering_id &&
                    (Convert.ToInt32(dt_offerings.Rows[i]["o_aged_care_patient_type_id"]) == 1 || Convert.ToBoolean(dt_offerings.Rows[i]["o_is_deleted"])))
                    dt_offerings.Rows.RemoveAt(i);

                // if clinic patient and no ac pt type set, only allow HC/LC/HCU/LCF
                else if ((ac_inv_aged_care_patient_type_id == -1 || ac_pat_aged_care_patient_type_id == -1) &&
                    !(new List<int> { 2, 3, 4, 5 }).Contains(Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"])))
                    dt_offerings.Rows.RemoveAt(i);

                else if (!(new List<int> { 2, 4 }).Contains(ac_pat_aged_care_patient_type_id) && Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"]) == 9) // if not LC/LCF - remove option for DVA
                    dt_offerings.Rows.RemoveAt(i);
                else if (!(new List<int> { 2, 4 }).Contains(ac_pat_aged_care_patient_type_id) && Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"]) == 6) // if not LC/LCF - remove option for LCE
                    dt_offerings.Rows.RemoveAt(i);
                else if (!(new List<int> { 3, 5 }).Contains(ac_pat_aged_care_patient_type_id) && Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"]) == 7) // if not HC/HCU - remove option for HCE
                    dt_offerings.Rows.RemoveAt(i);

                else if (ac_inv_offering_id != -1 && (new List<int> { 2, 3, 4, 5 }).Contains(ac_inv_aged_care_patient_type_id) && (new List<int> { 6, 7, 8, 9, 10 }).Contains(Convert.ToInt32(dt_offerings.Rows[i]["o_aged_care_patient_type_id"])))
                    dt_offerings.Rows[i]["o_name"] = dt_offerings.Rows[i]["o_name"].ToString() + " (" + ac_pat_offering_name + ")";
            }

            DataView dv = dt_offerings.DefaultView;
            dv.Sort = "acpatientcat_aged_care_patient_type_id, o_name";
            dt_offerings = dv.ToTable();

            ddlACInvOffering.DataSource = dt_offerings;
            ddlACInvOffering.DataBind();

            if (patient.ACInvOffering != null)
                ddlACInvOffering.SelectedValue = patient.ACInvOffering.OfferingID.ToString();

            txtLogin.Text = patient.Login;
            txtPwd.Text = patient.Pwd;
            txtPwd.Attributes["value"] = patient.Pwd;

            chkIsCompany.Checked = patient.IsCompany;
            txtABN.Text = patient.ABN;

            txtNextOfKinName.Text = patient.NextOfKinName;
            txtNextOfKinRelation.Text = patient.NextOfKinRelation;
            txtNextOfKinContactInfo.Text = patient.NextOfKinContactInfo;

            lblTitle.Visible = false;
            lblFirstname.Visible = false;
            lblMiddlename.Visible = false;
            lblSurname.Visible = false;
            lblNickname.Visible = false;
            lblGender.Visible = false;
            lblDOB.Visible = false;
            lblDARev.Visible = false;
            lblIsClinicPatient.Visible = false;
            lblIsDeceased.Visible = false;
            lblIsDiabetic.Visible = false;
            lblIsMemberDiabetesAustralia.Visible = false;
            lblPrivateHealthFund.Visible = false;
            lblConcessionCardNbr.Visible = false;
            lblConcessionCardExpiry.Visible = false;
            lblACInvOffering.Visible = false;
            lblLogin.Visible = false;
            lblPwd.Visible = false;
            lblIsCompany.Visible = false;
            lblABN.Visible = false;
            lblNextOfKinName.Visible = false;
            lblNextOfKinRelation.Visible = false;
            lblNextOfKinContactInfo.Visible = false;
        }
        else
        {
            int age = -1;
            if (patient.Person.Dob != DateTime.MinValue)
            {
                DateTime now = DateTime.Today;
                age = now.Year - patient.Person.Dob.Year;
                if (now.Month < patient.Person.Dob.Month || (now.Month == patient.Person.Dob.Month && now.Day < patient.Person.Dob.Day))
                    age--;
            }

            lblTitle.Text = patient.Person.Title.ID == 0 ? "" : patient.Person.Title.Descr;
            lblFirstname.Text = patient.Person.Firstname.Length == 0 ? "" : patient.Person.Firstname;
            lblMiddlename.Text = patient.Person.Middlename.Length == 0 ? "" : patient.Person.Middlename;
            lblSurname.Text = patient.Person.Surname.Length == 0 ? "" : patient.Person.Surname;
            lblNickname.Text = patient.Person.Nickname.Length == 0 ? "" : patient.Person.Nickname;
            lblGender.Text = GetGenderText(patient.Person.Gender);
            lblDOB.Text = patient.Person.Dob == DateTime.MinValue ? "" : patient.Person.Dob.ToString("d MMM yyyy") + " (Age " + age + ")";
            lblDARev.Text = patient.DiabeticAAassessmentReviewDate == DateTime.MinValue ? "" : patient.DiabeticAAassessmentReviewDate.ToString("d MMM yyyy");
            lblIsClinicPatient.Text = patient.IsClinicPatient ? "Yes" : "No";
            lblIsDeceased.Text = patient.IsDeceased ? "Yes" : "No";
            lblIsDiabetic.Text = patient.IsDiabetic ? "YES" : "No";
            lblIsMemberDiabetesAustralia.Text = patient.IsMemberDiabetesAustralia ? "Yes" : "No";
            lblPrivateHealthFund.Text = patient.PrivateHealthFund.Length == 0 ? "" : patient.PrivateHealthFund;
            lblConcessionCardNbr.Text = patient.ConcessionCardNumber.Length == 0 ? "" : patient.ConcessionCardNumber;
            lblConcessionCardExpiry.Text = patient.ConcessionCardExpiryDate == DateTime.MinValue ? "" : patient.ConcessionCardExpiryDate.ToString("MM  '/'  yyyy");
            lblAddedBy.Font.Bold = true;
            lblPatientDateAdded.Font.Bold = true;

            if (patient.IsDiabetic)
            {
                lblIsDiabetic.Font.Bold = true;
                lblIsDiabeticText.Font.Bold = true;
                lblIsDiabetic.ForeColor = System.Drawing.Color.Red;
                lblIsDiabeticText.ForeColor = System.Drawing.Color.Red;
            }

            if (patient.ACInvOffering == null)
                lblACInvOffering.Text = string.Empty;
            else if ((new List<int> { 2, 3, 4, 5 }).Contains(patient.ACInvOffering.AgedCarePatientType.ID))
                lblACInvOffering.Text = patient.ACInvOffering.Name;
            else if ((new List<int> { 6, 7, 8, 9, 10 }).Contains(patient.ACInvOffering.AgedCarePatientType.ID))
                lblACInvOffering.Text = patient.ACInvOffering.Name + " (" + patient.ACPatOffering.Name + ")";
            else // (?)
                lblACInvOffering.Text = patient.ACInvOffering.Name;

            lblLogin.Text = patient.Login.Length == 0 ? "" : patient.Login;
            lblPwd.Text = patient.Pwd.Length == 0 ? "" : "●●●●●●●●●";

            lblIsCompany.Text = patient.IsCompany ? "Yes" : "No";
            lblABN.Text = patient.ABN;

            lblNextOfKinName.Text = patient.NextOfKinName.Length == 0 ? "--" : patient.NextOfKinName;
            lblNextOfKinRelation.Text = patient.NextOfKinRelation.Length == 0 ? "--" : patient.NextOfKinRelation;
            lblNextOfKinContactInfo.Text = patient.NextOfKinContactInfo.Length == 0 ? "--" : patient.NextOfKinContactInfo.Replace(Environment.NewLine, "<br />");

            if (patient.NextOfKinContactInfo.Length > 25)
                td_nextofkincontactinfo.Style["min-width"] = "220px";

            ddlTitle.Visible = false;
            txtFirstname.Visible = false;
            txtMiddlename.Visible = false;
            txtSurname.Visible = false;
            txtNickname.Visible = false;
            ddlGender.Visible = false;
            ddlDOB_Day.Visible = false;
            ddlDOB_Month.Visible = false;
            ddlDOB_Year.Visible = false;
            ddlDARev_Day.Visible = false;
            ddlDARev_Month.Visible = false;
            ddlDARev_Year.Visible = false;
            chkIsClinicPatient.Visible = false;
            chkIsDeceased.Visible = false;
            chkIsDiabetic.Visible = false;
            chkIsMemberDiabetesAustralia.Visible = false;
            txtPrivateHealthFund.Visible = false;
            txtConcessionCardNbr.Visible = false;
            ddlConcessionCardExpiry_Month.Visible = false;
            ddlConcessionCardExpiry_Year.Visible = false;
            ddlACInvOffering.Visible = false;
            txtLogin.Visible = false;
            txtPwd.Visible = false;
            chkIsCompany.Visible = false;
            txtABN.Visible = false;
            txtNextOfKinName.Visible = false;
            txtNextOfKinRelation.Visible = false;
            txtNextOfKinContactInfo.Visible = false;
        }
        if (debugPageLoadTime) Logger.LogQuery("-B4", false, false, true);


        //this.lnkClinics.Visible = true;
        //this.lnkClinics.NavigateUrl = "~/RegisterOrganisationsToPatientV2.aspx?id=" + GetFormID().ToString() + "&type=edit";
        //this.lnkClinics.Text = "Add / Remove";

        //DataTable incList = RegisterPatientDB.GetDataTable_OrganisationsOf(patient.PatientID);
        //incList.DefaultView.Sort = "name ASC";
        //lstOrgs.DataSource = incList;
        //lstOrgs.DataBind();

        FillOrganisationRegistrationGrid();
        if (debugPageLoadTime) Logger.LogQuery("-B5", false, false, true);
        SetBookingsList(patient);  // <====================================================
        if (debugPageLoadTime) Logger.LogQuery("-B6", false, false, true);
        SetNotesList(patient);
        if (debugPageLoadTime) Logger.LogQuery("-B7", false, false, true);
            FillCreditGrid(patient);
        if (debugPageLoadTime) Logger.LogQuery("-B8", false, false, true);


        btnSubmit.Text = isEditMode ? "Update Details" : "Edit Details";

        btnSubmit.Visible = true;
        btnCancel.Visible = isEditMode;


        btnDeleteUndeletePatient.Visible = userView.IsAdminView;
        if (patient.IsDeleted && !userView.IsAdminView)
        {
            btnSubmit.Visible = false;
            btnCancel.Visible = false;
        }
        if (debugPageLoadTime) Logger.LogQuery("-B8", false, false, true);

    }
    protected void lstLetterHistory_RowCommand(object sender, RepeaterCommandEventArgs e)
    {
        if (e.CommandName.Equals("RetrieveLetterDB"))
        {
            int letter_print_hisotory_id = Int32.Parse((string)e.CommandArgument);
            LetterFile letterFile = LetterPrintHistoryDB.GetLetterFile(letter_print_hisotory_id);
            if (letterFile == null)
                throw new CustomMessageException("No file with selected ID");

            Letter.DownloadDocument(Response, letterFile.Contents, letterFile.Name);
        }
        if (e.CommandName.Equals("RetrieveLetterFlatFile"))
        {
            int letter_print_hisotory_id = Int32.Parse((string)e.CommandArgument);
            LetterPrintHistory letterPrintHistory = LetterPrintHistoryDB.GetByID(letter_print_hisotory_id);

            string historyDir = Letter.GetLettersHistoryDirectory(letterPrintHistory.Organisation.OrganisationID);

            string filePath = historyDir + letterPrintHistory.LetterPrintHistoryID + System.IO.Path.GetExtension(letterPrintHistory.DocName);
            if (!System.IO.File.Exists(filePath))
                throw new CustomMessageException("No file with selected ID");

            byte[] fileContents = System.IO.File.ReadAllBytes(filePath);
            Letter.DownloadDocument(Response, fileContents, letterPrintHistory.DocName);
        }
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
    protected void SetNotEditable(System.Web.UI.WebControls.WebControl c)
    {
        c.Enabled = false;
        c.BorderStyle = BorderStyle.None;
        c.BackColor = System.Drawing.Color.Transparent;
        c.ForeColor = System.Drawing.Color.Black;
    }

    #endregion

    #region SetBookingsList, btnSearchBookingList, btnPrintBookingList

    protected void btnSearchBookingList_Click(object sender, EventArgs e)
    {
        SetBookingsList(null, true);
    }
    protected DataTable SetBookingsList(Patient patient = null, bool goToAnchor = false)
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
        DateTime endDate = txtEndDate.Text.Length == 0 ? DateTime.MinValue : Utilities.GetDate(txtEndDate.Text, "dd-mm-yyyy");


        bool isMobileDevice = Utilities.IsMobileDevice(Request);

        UserView userView = UserView.GetInstance();
        int loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);


        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());


        if (debugPageLoadTime) Logger.LogQuery("--C2", false, false, true);
        Invoice[] outstandingInvoices = InvoiceDB.GetOutstanding(patient.PatientID, -1, false);
        if (debugPageLoadTime) Logger.LogQuery("--C3", false, false, true);

        // show outstanding for those not attached to a booking on top of booking list
        // show others with the bookings list (below)
        ArrayList outstandingPrivateInvoices = new ArrayList();
        ArrayList outstandingBookingInvoices = new ArrayList();
        for (int i = 0; i < outstandingInvoices.Length; i++)
        {
            if (outstandingInvoices[i].Booking == null)
                outstandingPrivateInvoices.Add(outstandingInvoices[i]);
            else
                outstandingBookingInvoices.Add(outstandingInvoices[i]);
        }
        lblInvoiceOwingMessage.Text = string.Empty;
        if (outstandingPrivateInvoices.Count > 0)
        {
            lblInvoiceOwingMessage.Text += "<font color=\"red\">Has outstanding invoices on cash sales:<br>";

            lblInvoiceOwingMessage.Text += "<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"block_center\">";
            for (int i = 0; i < outstandingPrivateInvoices.Count; i++)
            {
                string onclick = @"onclick=""open_new_tab('Invoice_ViewV2.aspx?invoice_id=" + ((Invoice)outstandingPrivateInvoices[i]).InvoiceID + @"', '', 'dialogWidth:675px;dialogHeight:725px;center:yes;resizable:no; scroll:no');return false;""";
                string link = "<a " + onclick + " href=\"\">View Inv.</a>";

                lblInvoiceOwingMessage.Text += "<tr>";
                lblInvoiceOwingMessage.Text += "  <td style=\"width:30px\"></td>";
                lblInvoiceOwingMessage.Text += "  <td><b>Owes $" + ((Invoice)outstandingPrivateInvoices[i]).TotalDue + "</b>" + "</td>";
                lblInvoiceOwingMessage.Text += "  <td style=\"width:15px\"></td>";
                lblInvoiceOwingMessage.Text += "  <td>" + link + "</td>";
                lblInvoiceOwingMessage.Text += "</tr>";
            }

            lblInvoiceOwingMessage.Text += "</table>";
            lblInvoiceOwingMessage.Text += "</font><br />";
        }
        if (outstandingBookingInvoices.Count > 0)
        {
            lblInvoiceOwingMessage.Text += "<font color=\"red\">Has outstanding invoices from treatments:<br>";

            lblInvoiceOwingMessage.Text += "<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"block_center\">";
            for (int i = 0; i < outstandingBookingInvoices.Count; i++)
            {
                string onclick = @"onclick=""open_new_tab('Invoice_ViewV2.aspx?invoice_id=" + ((Invoice)outstandingBookingInvoices[i]).InvoiceID + @"', '', 'dialogWidth:675px;dialogHeight:725px;center:yes;resizable:no; scroll:no');return false;""";
                string link = "<a " + onclick + " href=\"\">View Inv.</a>";

                lblInvoiceOwingMessage.Text += "<tr>";
                lblInvoiceOwingMessage.Text += "  <td style=\"width:30px\"></td>";
                lblInvoiceOwingMessage.Text += "  <td><b>Owes $" + ((Invoice)outstandingBookingInvoices[i]).TotalDue + "</b>" + "</td>";
                lblInvoiceOwingMessage.Text += "  <td style=\"width:15px\"></td>";
                lblInvoiceOwingMessage.Text += "  <td>" + link + "</td>";
                lblInvoiceOwingMessage.Text += "</tr>";
            }

            lblInvoiceOwingMessage.Text += "</table>";
            lblInvoiceOwingMessage.Text += "</font><br />";
        }



        if (debugPageLoadTime) Logger.LogQuery("--C5", false, false, true);
        DataTable tblBookingList = BookingDB.GetDataTable_Between(debugPageLoadTime, startDate, endDate, null, null, patient, null, true, null);
        if (debugPageLoadTime) Logger.LogQuery("--C6", false, false, true);

        int[] booking_ids = new int[tblBookingList.Rows.Count];
        int[] entity_ids = new int[tblBookingList.Rows.Count];
        for (int i = 0; i < tblBookingList.Rows.Count; i++)
        {
            booking_ids[i] = Convert.ToInt32(tblBookingList.Rows[i]["booking_booking_id"]);
            entity_ids[i] = Convert.ToInt32(tblBookingList.Rows[i]["booking_entity_id"]);
        }


        BookingPatient[] bps = BookingPatientDB.GetByPatientID(patient.PatientID);

        Hashtable bpsHash = new Hashtable();
        foreach(BookingPatient bp in bps)
            bpsHash[new Hashtable2D.Key(bp.Booking.BookingID, bp.Patient.PatientID)] = bp;

        int[] bp_entity_ids = bps.Select(o => o.EntityID).ToArray<int>();
        entity_ids = entity_ids.Concat(bp_entity_ids).ToArray();


        Hashtable changeHistoryHash = BookingDB.GetChangeHistoryCountHash(booking_ids);

        lblBookingListCount.Text = "(" + tblBookingList.Rows.Count + ")";
        if (tblBookingList.Rows.Count == 0)
        {
            lblBookingsList_NoRowsMessage.Visible = true;
            pnlBookingsList.Visible = false;

            lblProvidersSeenHeading.Text = "<b>Providers Seen</b>";
            lblProvidersSeen.Text = "No Bookings Yet";
        }
        else
        {
            lblBookingsList_NoRowsMessage.Visible = false;
            pnlBookingsList.Visible = true;

            ArrayList providersSeenList = new ArrayList(); // to get the order
            Hashtable providersSeenDateHash = new Hashtable(); // for a quick lookup whether we added to the list or not (and contains date of last bk)

            Hashtable staffHash = StaffDB.GetAllInHashtable(true, true, false, false);
            ArrayList bookingsWithInvoices = new ArrayList();
            if (debugPageLoadTime) Logger.LogQuery("--C11", false, false, true);

            Hashtable noteHash = NoteDB.GetNoteHash(entity_ids, true);
            if (debugPageLoadTime) Logger.LogQuery("--C12", false, false, true);

            tblBookingList.Columns.Add("notes_text",                    typeof(string));
            tblBookingList.Columns.Add("booking_notes_text",            typeof(string));
            tblBookingList.Columns.Add("invoice_text",                  typeof(string));
            tblBookingList.Columns.Add("booking_url",                   typeof(string));
            tblBookingList.Columns.Add("printletter_link",              typeof(String));
            tblBookingList.Columns.Add("hide_booking_link",             typeof(Boolean));
            tblBookingList.Columns.Add("show_invoice_row",              typeof(int));
            tblBookingList.Columns.Add("show_notes_row",                typeof(int));
            tblBookingList.Columns.Add("show_printletter_row",          typeof(int));
            tblBookingList.Columns.Add("show_bookingsheet_row",         typeof(int));
            tblBookingList.Columns.Add("inv_type_text",                 typeof(string));
            tblBookingList.Columns.Add("inv_outstanding_text",          typeof(string));
            tblBookingList.Columns.Add("show_outstanding_row",          typeof(int));
            tblBookingList.Columns.Add("added_by_deleted_by_row",       typeof(string));
            tblBookingList.Columns.Add("booking_files_link",            typeof(string));
            tblBookingList.Columns.Add("booking_change_history_link",   typeof(string));
            tblBookingList.Columns.Add("hide_change_history_link",      typeof(Boolean));
            tblBookingList.Columns.Add("show_change_history_row",       typeof(string));
            tblBookingList.Columns.Add("is_deleted",                    typeof(Boolean));
            tblBookingList.Columns.Add("is_cancelled",                  typeof(Boolean));
            bool hasInvoiceRows      = false;
            bool hasNotesRows        = false;
            bool hasPrintLetterRows  = false;
            bool hasBookingSheetRows = false;
            bool hasOutstandingRows  = false;
            for (int i = 0; i < tblBookingList.Rows.Count; i++)
            {
                Booking curBooking = BookingDB.LoadFull(tblBookingList.Rows[i]);
                bool isDeleted     = curBooking.DateDeleted != DateTime.MinValue || curBooking.DeletedBy != null;


                if (!isDeleted)
                {
                    if (providersSeenDateHash[curBooking.Provider.StaffID] == null)
                    {
                        providersSeenList.Add(curBooking.Provider);
                        providersSeenDateHash[curBooking.Provider.StaffID] = curBooking.DateStart;
                    }
                    else if (curBooking.DateStart > (DateTime)providersSeenDateHash[curBooking.Provider.StaffID])
                    {
                        providersSeenDateHash[curBooking.Provider.StaffID] = curBooking.DateStart;
                    }
                }


                if (curBooking.Patient != null)
                    tblBookingList.Rows[i]["notes_text"] = Note.GetBookingPopupLinkTextV2(curBooking.EntityID, curBooking.NoteCount > 0, true, 1425, 700, "images/notes-bw-24.jpg", "images/notes-24.png", "btnUpdateBookingList.click()", !isMobileDevice);
                else // group booking
                {
                    BookingPatient bp = bpsHash[new Hashtable2D.Key(curBooking.BookingID, patient.PatientID)] as BookingPatient;
                    if (bp != null)
                        tblBookingList.Rows[i]["notes_text"] = Note.GetBookingPopupLinkTextV2(bp.EntityID, bp.NoteCount > 0, true, 1425, 700, "images/notes-bw-24.jpg", "images/notes-24.png", "btnUpdateBookingList.click()", !isMobileDevice);
                }


                string notesText = string.Empty;

                Note[] bkNotes = null;
                if (curBooking.Patient != null)
                {
                    bkNotes = (Note[])noteHash[curBooking.EntityID];
                    tblBookingList.Rows[i]["printletter_link"] = String.Format("~/Letters_PrintV2.aspx?booking={0}", curBooking.BookingID);

                }
                else
                {
                    BookingPatient bp = bpsHash[new Hashtable2D.Key(curBooking.BookingID, patient.PatientID)] as BookingPatient;
                    if (bp != null)
                    {
                        bkNotes = (Note[])noteHash[bp.EntityID];
                        tblBookingList.Rows[i]["printletter_link"] = String.Format("~/Letters_PrintV2.aspx?bookingpatient={0}", bp.BookingPatientID);
                    }
                }

                if (bkNotes != null)
                {
                    foreach (Note note in bkNotes)
                    {
                        if (note.IsDeleted)
                        {
                            // just dont show if deleted - they can open the notes to see it

                            /*
                            notesText += (notesText.Length == 0 ? "" : "<br /><br />") +
                                "<font color=\"gray\">" +
                                "<u>" + note.DateAdded.ToString("dd / MM / yyyy") + "</u>" + "<br />" +
                                "<u>" + note.NoteType.Descr + "</u>" + "<br />" +
                                "*** DELETED ***<br />" +
                                ((note.BodyPart.ID == -1 || note.BodyPart.Descr.Length == 0) ? "" : @"<u>" + Eval("body_part_descr") + "</u> : ") + note.Text.Replace("\n", "<br/>") +
                                "</font>";
                            */
                        }
                        else
                        {
                            notesText += (notesText.Length == 0 ? "" : "<br /><br />") +
                                "<b><u>" + note.DateAdded.ToString("dd / MM / yyyy") + "</u></b>" + "<br />" +
                                "<u>" + note.NoteType.Descr + "</u>" + "<br />" +
                                "<font color=\"#a52a2a\">" + ((note.BodyPart.ID == -1 || note.BodyPart.Descr.Length == 0) ? "" : "<i>" + note.BodyPart.Descr + "</i>:<br />") + note.Text.Replace("\n", "<br/>") + "</font>";
                        }
                    }
                }
                if (notesText.Length == 0)
                    notesText = "No Notes.";
                tblBookingList.Rows[i]["booking_notes_text"] = notesText;


                bool canSeeInvoiceInfo = userView.IsAdminView || userView.IsPrincipal || (curBooking.Provider != null && curBooking.Provider.StaffID == loggedInStaffID && curBooking.DateStart > DateTime.Today.AddMonths(-2));
                if (canSeeInvoiceInfo && Convert.ToInt32(tblBookingList.Rows[i]["booking_inv_count"]) > 0)
                {
                    string onclick = @"onclick=""javascript:open_new_tab('Invoice_ViewV2.aspx?booking_id=" + curBooking.BookingID + @"');return false;""";
                    tblBookingList.Rows[i]["invoice_text"] = "<a " + onclick + " href=\"\">View<br/>Inv.</a>";

                    if (!isDeleted)
                        hasInvoiceRows = true;

                    bookingsWithInvoices.Add(curBooking.BookingID);
                }
                else
                {
                    tblBookingList.Rows[i]["invoice_text"] = "";
                }

                tblBookingList.Rows[i]["hide_booking_link"] = !((userView.IsClinicView && curBooking.Organisation.OrganisationType.OrganisationTypeID == 218) ||
                                                                (userView.IsAgedCareView && (new List<int> { 139, 367, 372 }).Contains(curBooking.Organisation.OrganisationType.OrganisationTypeID)));

                if (!isDeleted)
                {
                    hasNotesRows = true;
                    hasPrintLetterRows = true;
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

                decimal totalOwing = 0;
                for (int j = 0; j < outstandingInvoices.Length; j++)
                    if (outstandingInvoices[j].Booking != null && outstandingInvoices[j].Booking.BookingID == curBooking.BookingID)
                        totalOwing += outstandingInvoices[j].TotalDue;
                tblBookingList.Rows[i]["inv_outstanding_text"] = totalOwing == 0 ? "" : "<font color=\"red\"><b>Owes<br />$" + totalOwing.ToString() + "</b></font>";
                if (totalOwing > 0)
                    hasOutstandingRows = true;


                string addedBy       = curBooking.AddedBy == null || staffHash[curBooking.AddedBy.StaffID] == null ? "" : ((Staff)staffHash[curBooking.AddedBy.StaffID]).Person.FullnameWithoutMiddlename;
                string addedDate     = curBooking.DateCreated == DateTime.MinValue ? "" : curBooking.DateCreated.ToString("MMM d, yyyy");
                string deletedBy     = curBooking.DeletedBy == null || staffHash[curBooking.DeletedBy.StaffID] == null ? "" : ((Staff)staffHash[curBooking.DeletedBy.StaffID]).Person.FullnameWithoutMiddlename;
                string deletedDate   = curBooking.DateDeleted == DateTime.MinValue ? "" : curBooking.DateDeleted.ToString("MMM d, yyyy");
                string cancelledBy   = curBooking.CancelledBy == null || staffHash[curBooking.CancelledBy.StaffID] == null ? "" : ((Staff)staffHash[curBooking.CancelledBy.StaffID]).Person.FullnameWithoutMiddlename;
                string cancelledDate = curBooking.DateCancelled == DateTime.MinValue ? "" : curBooking.DateCancelled.ToString("MMM d, yyyy");
                
                string added_by_deleted_by_row = string.Empty;
                added_by_deleted_by_row += "Added By: " + addedBy + " (" + addedDate + ")";
                if (deletedBy.Length > 0 || deletedDate.Length > 0)
                    added_by_deleted_by_row += "\r\nDeleted By: " + deletedBy + " (" + deletedDate + ")";
                if (cancelledBy.Length > 0 || cancelledDate.Length > 0)
                    added_by_deleted_by_row += "\r\nCancelled By: " + cancelledBy + " (" + cancelledDate + ")";

                tblBookingList.Rows[i]["added_by_deleted_by_row"]     = added_by_deleted_by_row;
                tblBookingList.Rows[i]["booking_files_link"]          = curBooking.GetScannedDocsImageLink(true);
                tblBookingList.Rows[i]["booking_change_history_link"] = curBooking.GetBookingChangeHistoryPopupLinkImage(null, false, !isMobileDevice);
                tblBookingList.Rows[i]["hide_change_history_link"]    = changeHistoryHash[curBooking.BookingID] == null;
                tblBookingList.Rows[i]["is_deleted"]                  = (curBooking.BookingStatus != null && curBooking.BookingStatus.ID == -1) || isDeleted;
                tblBookingList.Rows[i]["is_cancelled"]                = curBooking.BookingStatus != null && curBooking.BookingStatus.ID == 188;
            }

            if (debugPageLoadTime) Logger.LogQuery("--C13", false, false, true);
            Hashtable hashHasMedicareOrDVAInvoices = BookingDB.GetHashHasMedicareDVA((int[])bookingsWithInvoices.ToArray(typeof(int)), patient.PatientID);
            if (debugPageLoadTime) Logger.LogQuery("--C14", false, false, true);

            for (int i = 0; i < tblBookingList.Rows.Count; i++)
            {
                tblBookingList.Rows[i]["show_invoice_row"]        = !userView.IsExternalView && hasInvoiceRows              ? 1 : 0;
                tblBookingList.Rows[i]["show_notes_row"]          = !userView.IsExternalView && hasNotesRows                ? 1 : 0;
                tblBookingList.Rows[i]["show_printletter_row"]    = !userView.IsExternalView && hasPrintLetterRows          ? 1 : 0;
                tblBookingList.Rows[i]["show_bookingsheet_row"]   = hasBookingSheetRows                                     ? 1 : 0;
                tblBookingList.Rows[i]["show_outstanding_row"]    = !userView.IsExternalView && hasOutstandingRows          ? 1 : 0;
                tblBookingList.Rows[i]["show_change_history_row"] = !userView.IsExternalView && changeHistoryHash.Count > 0 ? 1 : 0;

                int  booking_id   = Convert.ToInt32(tblBookingList.Rows[i]["booking_booking_id"]);
                bool has_medicare = hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -1)] != null && Convert.ToBoolean(hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -1)]);
                bool has_dva      = hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -2)] != null && Convert.ToBoolean(hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -2)]);
                if (has_medicare) tblBookingList.Rows[i]["inv_type_text"] = "Medicare";
                else if (has_dva) tblBookingList.Rows[i]["inv_type_text"] = "DVA";
                else tblBookingList.Rows[i]["inv_type_text"] = string.Empty;
            }
            tblBookingList.DefaultView.Sort = "booking_date_start DESC";
            tblBookingList = tblBookingList.DefaultView.ToTable();
            lstBookingList.DataSource = tblBookingList;
            lstBookingList.DataBind();


            // put provider and last date into tuple, then sort, the output it for display
            List<Tuple<Staff, DateTime>> providersSeenTupleList = new List<Tuple<Staff, DateTime>>();
            for (int i = providersSeenList.Count - 1; i >= 0; i--)
                providersSeenTupleList.Add(new Tuple<Staff, DateTime>((Staff)providersSeenList[i], (DateTime)providersSeenDateHash[((Staff)providersSeenList[i]).StaffID]));
            providersSeenTupleList.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            string providersSeenText = string.Empty;
            foreach (Tuple<Staff, DateTime> item in providersSeenTupleList)
                providersSeenText += "<tr style=\"white-space:nowrap;\"><td>" + item.Item1.Person.FullnameWithoutMiddlename + "&nbsp;&nbsp;</td><td>" + item.Item2.Day + "&nbsp;&nbsp;</td><td>" + item.Item2.ToString("MMM") + ",&nbsp;</td><td>" + item.Item2.ToString("yyyy") + "&nbsp;</td></tr>";
            lblProvidersSeenHeading.Text = "<b>Providers Seen</b> <i>(Last BK Date)</i>:";
            lblProvidersSeen.Text = "<table>" + providersSeenText + "</table>";
        }



        if (goToAnchor)
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>location.href=\"#bookinglist\";</script>");

        return tblBookingList;
    }

    protected void btnPrintBookingList_Click(object sender, EventArgs e)
    {
        DataTable tblBookingList = SetBookingsList();
        if (tblBookingList == null)
            return;

        try
        {
            string originalFile = Letter.GetLettersDirectory() + @"BookingListForPatient.docx";
            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            string tmpOutputFile = FileHelper.GetTempFileName(tmpLettersDirectory + "BookingList." + System.IO.Path.GetExtension(originalFile));


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
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch (Exception ex)
        {
            SetErrorMessage(ex.ToString());
        }

    }

    #endregion

    #region SetNotesList, btnUpdateNoteList

    protected void btnUpdateNoteList_Click(object sender, EventArgs e)
    {
        SetNotesList(null, true);
    }
    protected void SetNotesList(Patient patient = null, bool goToAnchor = false)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        DataTable tblNoteList = NoteDB.GetDataTable_ByEntityID(patient.Person.EntityID, null, false);

        for (int i = tblNoteList.Rows.Count - 1; i >= 0; i--)
            if (tblNoteList.Rows[i]["date_deleted"] != DBNull.Value || tblNoteList.Rows[i]["deleted_by"] != DBNull.Value)
                tblNoteList.Rows.RemoveAt(i);


        lblNotesListCount.Text = "(" + tblNoteList.Rows.Count + ")";
        if (tblNoteList.Rows.Count == 0)
        {
            lblNotesList_NoRowsMessage.Visible = true;
            pnlNotesList.Visible = false;
        }
        else
        {
            lblNotesList_NoRowsMessage.Visible = false;
            pnlNotesList.Visible = true;

            //tblBookingList.DefaultView.Sort = "booking_date_start DESC";
            lstNoteList.DataSource = tblNoteList;
            lstNoteList.DataBind();
        }


        //
        // set booking list scroll panel max-height depending on size of notes list
        //

        /*
        int totalNoteLines = 0;
        for (int i = 0; i < tblNoteList.Rows.Count; i++)
            totalNoteLines += ((int)(NoteDB.Load(tblNoteList.Rows[i]).Text.Length / 70) + 1);

        if (totalNoteLines > 10)
            pnlBookingsList.Style["max-height"] = "250px";
        else if (totalNoteLines > 6)
            pnlBookingsList.Style["max-height"] = "320px";
        else if (totalNoteLines > 2)
            pnlBookingsList.Style["max-height"] = "390px";
        else
            pnlBookingsList.Style["max-height"] = "470px";
        */

        if (goToAnchor)
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>location.href=\"#notelist\";</script>");
    }

    #endregion

    #region btnUpdateNotesIcon, btnUpdateFlashingTextIcon, btnUpdateReferrersList, btnUpdateBookingList
    
    protected void btnUpdateNotesIcon_Click(object sender, EventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        SetNotesList(patient, true);
    }
    protected void btnUpdateFlashingTextIcon_Click(object sender, EventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());

        if (patient.FlashingTextAddedBy != null)
            patient.FlashingTextAddedBy = StaffDB.GetByID(patient.FlashingTextAddedBy.StaffID);

        string addedByHoverLink = patient.FlashingTextAddedBy == null ? "" : @"<a href=""javascript:void(0)"" onclick=""javascript:return false;"" title='Added By: " + patient.FlashingTextAddedBy.Person.FullnameWithoutMiddlename + @"' style=""text-decoration: none"">*</a>";
        lblFlashingText.Text = (patient.FlashingText.Length == 0 || patient.FlashingTextLastModifiedDate == DateTime.MinValue ? "" : @"<span style=""font-size:70%;font-weight:normal;"">" + "[" + patient.FlashingTextLastModifiedDate.ToString("d'/'M'/'yyyy") + addedByHoverLink + "]</span> ") + patient.FlashingText;
    }
    protected void btnUpdateReferrersList_Click(object sender, EventArgs e)
    {
        this.patientReferrer.UpdateReferrersList();
    }
    protected void btnUpdateBookingList_Click(object sender, EventArgs e)
    {
        SetBookingsList();
    }

    #endregion

    #region btnDeleteUndeletePatient, btnUpdateScannedDocumentsCount, UpdateScannedDocumentsCount

    protected void btnDeleteUndeletePatient_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "Delete" || e.CommandName == "UnDelete")
        {
            if (e.CommandName == "Delete")
            {
                PatientDB.UpdateInactive(GetFormID(), Convert.ToInt32(Session["StaffID"]));
            }
            if (e.CommandName == "UnDelete")
            {
                PatientDB.UpdateActive(GetFormID(), Convert.ToInt32(Session["StaffID"]));
            }

            Patient patient = PatientDB.GetByID(GetFormID());
            btnDeleteUndeletePatient.CommandName = patient.IsDeleted ? "UnDelete" : "Delete";
            btnDeleteUndeletePatient.Text = patient.IsDeleted ? "Un-Archive" : "Archive";
            lblPatientStatus.Text = patient.IsDeleted ? "<b><font color=\"red\">Archived</font></b>" : "Active";

            changeHistoryLinknRow.Visible = true;
        }
    }
    protected void btnUpdateScannedDocumentsCount_Click(object sender, EventArgs e)
    {
        UpdateScannedDocumentsCount();
    }

    protected void UpdateScannedDocumentsCount()
    {
        try
        {
            Patient patient = PatientDB.GetByID(GetFormID());
            if (patient == null)
                throw new Exception("Invalid patient id");

            // set it first without hover comments so there is 'some' link if the network is unavailable
            string scannedDocsPopup = "";
            string jsScannedDocuments = !Utilities.IsMobileDevice(Request) ?
                "javascript:window.showModalDialog('PatientScannedFileUploadsV2.aspx?patient=" + patient.PatientID + "', '', 'dialogWidth:500px;dialogHeight:610px;center:yes;resizable:no; scroll:no');btnUpdateScannedDocumentsCount.click();return false;" :
                "open_new_tab('PatientScannedFileUploadsV2.aspx?patient=" + patient.PatientID + "&refresh_on_close=1');return false;";
            this.lnkScannedDocuments.Text = "<a href=\"#\" onclick=\"" + jsScannedDocuments + "\" title=\"" + scannedDocsPopup + "\">Scanned Documents</a>"; ;
            this.lnkScannedDocumentsCount.ToolTip = scannedDocsPopup;
            this.lnkScannedDocumentsCount.Text = scannedDocsPopup;

            // now try re-set it all if network retrieved info
            System.IO.FileInfo[] scannedDocs = patient.GetScannedDocs();
            string scannedDocsDir = patient.GetScannedDocsDirectory();
            scannedDocsPopup = string.Empty;
            Hashtable medicalServiceTypeHash = MedicalServiceTypeDB.GetMedicareServiceTypeHash();
            foreach (System.IO.FileInfo fi in scannedDocs)
                scannedDocsPopup += (scannedDocsPopup.Length == 0 ? "" : "\r\n") + MedicalServiceType.ReplaceMedicareServiceTypeFolders(fi.FullName.Substring(scannedDocsDir.Length), medicalServiceTypeHash, false);
            this.lnkScannedDocuments.Text = "<a href=\"#\" onclick=\"" + jsScannedDocuments + "\" title=\"" + scannedDocsPopup + "\">Scanned Documents</a>"; ;
            this.lnkScannedDocumentsCount.ToolTip = scannedDocsPopup;
            this.lnkScannedDocumentsCount.Text = scannedDocs.Length.ToString();
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // don't need to set any message - it just wont show the current files
            //SetErrorMessage("Connection to network files currently unavailable.");
        }
        catch (Exception ex)
        {
            SetErrorMessage("", ex.ToString());
        }
    }

    #endregion



    #region btnSubmit_Click, btnCancel_Click

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (!ddlDOBValidateAllOrNoneSet.IsValid)
            return;
        if (!ddlDARevValidateAllOrNoneSet.IsValid)
            return;

        UserView userView = UserView.GetInstance();


        txtPwd.Attributes["value"] = txtPwd.Text;  // pwd fields is unset on send back to server, so re-set it

        if (GetUrlParamType() == UrlParamType.View)
        {
            Response.Redirect( UrlParamModifier.AddEdit(Request.RawUrl, "type", "edit") );
        }
        else if (GetUrlParamType() == UrlParamType.Edit)
        {
            if ((ddlConcessionCardExpiry_Year.SelectedValue  != "-1" && ddlConcessionCardExpiry_Month.SelectedValue == "-1") ||
                (ddlConcessionCardExpiry_Month.SelectedValue != "-1" && ddlConcessionCardExpiry_Year.SelectedValue  == "-1"))
            {
                SetErrorMessage("Concession Card Expiry Must Be Both Set or Both Unset.");
                return;
            }


            DateTime concessionCardExpiryDate = ddlConcessionCardExpiry_Year.SelectedValue == "-1" || ddlConcessionCardExpiry_Month.SelectedValue == "-1" ?
                                                DateTime.MinValue :
                                                new DateTime(Convert.ToInt32(ddlConcessionCardExpiry_Year.SelectedValue), Convert.ToInt32(ddlConcessionCardExpiry_Month.SelectedValue), 1);

            Patient patient = PatientDB.GetByID(Convert.ToInt32(this.lblId.Text));


            if (txtLogin.Text.Length > 0)
            {
                if (txtPwd.Text.Length == 0)
                {
                    SetErrorMessage("Password is required when username entered");
                    return;
                }
                if (patient.Pwd != txtPwd.Text && txtPwd.Text.Length < 6)
                {
                    SetErrorMessage("New password must be at least 6 characters");
                    return;
                }

                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && patient.Login != txtLogin.Text && UserDatabaseMapperDB.UsernameExists(txtLogin.Text))
                {
                    SetErrorMessage("Login name already in use by another user");
                    return;
                }
                if (PatientDB.LoginExists(txtLogin.Text, patient.PatientID))
                {
                    SetErrorMessage("Login name already in use by another user");
                    return;
                }
            }


            Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
            PatientDB.AddACOfferings(ref offeringsHash, ref patient);


            int acInvOfferingID_New = patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID;
            int acPatOfferingID_New = patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID;

            if (patient.ACInvOffering == null || patient.ACPatOffering == null)
            {
                acInvOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
                acPatOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
            }
            else if (userView.IsAgedCareView && patient.ACInvOffering.OfferingID != Convert.ToInt32(ddlACInvOffering.SelectedValue))
            {
                int acInvOfferingID_Old = patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID;
                int acPatOfferingID_Old = patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID;

                acInvOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
                acPatOfferingID_New = acPatOfferingID_Old;

                int acInvAcPtTypeID_New = ((Offering)offeringsHash[Convert.ToInt32(ddlACInvOffering.SelectedValue)]).AgedCarePatientType.ID;
                int acInvAcPtTypeID_Old = patient.ACInvOffering.AgedCarePatientType.ID;

	            //when updating:
	            //- if changing to LC/HC/LCF/HCUF    - change BOTH to that (to make sure second is always clearly the pt type)
	            //- if changing to MC/DVA/TAC/LCE/HCE
                //  - if prev_first is LC/HC/LCF/HCUF     - move prev_first to second, and set first as selected
                //  - if prev_first is MC/DVA/TAC/LCE/HCE - set first as selected (and leave second)

                if ((new List<int> { 2, 3, 4, 5 }).Contains(acInvAcPtTypeID_New))
                {
                    acPatOfferingID_New = acInvOfferingID_New;
                }
                else if ((new List<int> { 6, 7, 8, 9, 10 }).Contains(acInvAcPtTypeID_New))
                {
                    if ((new List<int> { 2, 3, 4, 5 }).Contains(acInvAcPtTypeID_Old))
                        acPatOfferingID_New = acInvOfferingID_Old;
                }
                else // (?)
                    ; //
            }

            PatientHistoryDB.Insert(patient.PatientID, patient.IsClinicPatient, patient.IsGPPatient, patient.IsDeleted, patient.IsDeceased,
                                    patient.FlashingText, patient.FlashingTextAddedBy == null ? -1 : patient.FlashingTextAddedBy.StaffID, patient.FlashingTextLastModifiedDate,
                                    patient.PrivateHealthFund, patient.ConcessionCardNumber, patient.ConcessionCardExpiryDate, patient.IsDiabetic, patient.IsMemberDiabetesAustralia, patient.DiabeticAAassessmentReviewDate, patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID, patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID, patient.Login, patient.Pwd, patient.IsCompany, patient.ABN, patient.NextOfKinName, patient.NextOfKinRelation, patient.NextOfKinContactInfo, 
                                    patient.Person.Title.ID, patient.Person.Firstname, patient.Person.Middlename, patient.Person.Surname, patient.Person.Nickname, patient.Person.Gender, patient.Person.Dob, Convert.ToInt32(Session["StaffID"]));

            PersonDB.Update(patient.Person.PersonID, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), txtNickname.Text, ddlGender.SelectedValue, GetDOBFromForm(), DateTime.Now);
            PatientDB.Update(patient.PatientID, patient.Person.PersonID, patient.IsClinicPatient, patient.IsGPPatient, chkIsDeceased.Checked,
                patient.FlashingText, patient.FlashingTextAddedBy == null ? -1 : patient.FlashingTextAddedBy.StaffID, patient.FlashingTextLastModifiedDate,
                txtPrivateHealthFund.Text, txtConcessionCardNbr.Text, concessionCardExpiryDate, chkIsDiabetic.Checked, chkIsMemberDiabetesAustralia.Checked, GetDARevFromForm(), acInvOfferingID_New, acPatOfferingID_New, txtLogin.Text, txtLogin.Text.Length == 0 ? "" : txtPwd.Text, chkIsCompany.Checked, txtABN.Text.Trim(), txtNextOfKinName.Text.Trim(), txtNextOfKinRelation.Text.Trim(), txtNextOfKinContactInfo.Text.Trim());

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && patient.Login != txtLogin.Text)
            {
                UserDatabaseMapper curDBMapper = UserDatabaseMapperDB.GetByLogin(patient.Login, Session["DB"].ToString());
                if (txtLogin.Text.Length == 0) // delete from db
                {
                    if (curDBMapper != null)
                        UserDatabaseMapperDB.Delete(curDBMapper.ID);
                }
                else
                {
                    if (curDBMapper != null)
                        UserDatabaseMapperDB.Update(curDBMapper.ID, txtLogin.Text, Session["DB"].ToString());
                    else
                        UserDatabaseMapperDB.Insert(txtLogin.Text, Session["DB"].ToString());
                }
            }

            UpdateConditions(patient);


            Response.Redirect( UrlParamModifier.AddEdit(Request.RawUrl, "type", "view") );
        }
        else
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
    }

    #endregion

    #region DOBAllOrNoneCheck, DARevAllOrNoneCheck

    protected void DOBAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue);
    }
    protected void DARevAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlDARev_Day.SelectedValue, ddlDARev_Month.SelectedValue, ddlDARev_Year.SelectedValue);
    }

    #endregion

    #region GetDOBFromForm, GetDARevFromForm, GetDate, IsValidDate

    public DateTime GetDOBFromForm()
    {
        return GetDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue, "DOB");
    }
    public DateTime GetDARevFromForm()
    {
        return GetDate(ddlDARev_Day.SelectedValue, ddlDARev_Month.SelectedValue, ddlDARev_Year.SelectedValue, "DARev");
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

    #endregion



    #region GrdCredit

    protected void FillCreditGrid(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        UserView userview = UserView.GetInstance();
        int staffID = Convert.ToInt32(Session["StaffID"]);

        DataTable tbl = CreditDB.GetDataTable_ByEntityID(patient.Person.EntityID, "1", true);

        ViewState["tbl_voucher"] = tbl;

        if (tbl.Rows.Count > 0)
        {
            tbl.Columns.Add("can_delete", typeof(Boolean));
            tbl.Columns.Add("is_deleted", typeof(Boolean));
            tbl.Columns.Add("hidden"    , typeof(Boolean));
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                bool isDeleted = tbl.Rows[i]["credit_date_deleted"] != DBNull.Value || tbl.Rows[i]["credit_deleted_by"] != DBNull.Value;
                bool allUsed = Convert.ToInt32(tbl.Rows[i]["credit_amount"]) - Convert.ToInt32(tbl.Rows[i]["credit_amount_used"]) <= 0;
                bool isExpired = tbl.Rows[i]["credit_expiry_date"] != DBNull.Value && Convert.ToDateTime(tbl.Rows[i]["credit_expiry_date"]).Date < DateTime.Today;

                tbl.Rows[i]["hidden"] = isDeleted || allUsed || isExpired;
                tbl.Rows[i]["is_deleted"] = isDeleted;

                tbl.Rows[i]["can_delete"] =
                    (Convert.ToInt32(tbl.Rows[i]["credit_credit_type_id"]) == 1 || Convert.ToInt32(tbl.Rows[i]["credit_credit_type_id"]) == 2) &&
                    !isDeleted &&
                    (!userview.IsProviderView || staffID == Convert.ToInt32(tbl.Rows[i]["credit_added_by"]));
            }

            ViewState["tbl_voucher"] = tbl;

            GrdCredit.DataSource = tbl;
            GrdCredit.DataBind();
        }

        lblVouchersList_NoRowsMessage.Visible = tbl.Rows.Count == 0;
        show_hide_hidden_vouchers_checkbox.Visible = tbl.Rows.Count > 0;

    }

    protected void GrdCredit_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "_Delete")
        {
            try
            {
                int credit_id = Convert.ToInt32(e.CommandArgument);
                Credit credit = CreditDB.GetByID(credit_id);

                UserView userview = UserView.GetInstance();

                if (userview.IsProviderView && Convert.ToInt32(Session["StaffID"]) != credit.AddedBy.StaffID)
                    throw new CustomMessageException("You Can Not Delete Vouchers Entered By Other Providers.");

                CreditDB.SetAsDeleted(credit_id, Convert.ToInt32(Session["StaffID"]));

                FillCreditGrid();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
            }
        }
    }

    protected void GrdCredit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "hidden")))
        {
            e.Row.ForeColor = System.Drawing.Color.Gray;
            e.Row.Attributes["class"] = "hidden_voucher";
            e.Row.Style["display"] = "none";
        }


        DataTable dt = (DataTable)ViewState["tbl_voucher"];
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("credit_credit_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];

            Label lblIsDeleted = (Label)e.Row.FindControl("lblIsDeleted");
            if (lblIsDeleted != null && Convert.ToBoolean(thisRow["is_deleted"]))
                 lblIsDeleted.Text = "Is Deleted";

            Label lblDescr = (Label)e.Row.FindControl("lblDescr");
            if (lblDescr != null)
            {
                string checkStillLoggedIn = "if (ajax_check_session_timed_out()) {window.location.href = window.location.href;return false;}";
                string jsViewVoucher      = "javascript:" + checkStillLoggedIn + "viewVoucher(" + thisRow["credit_credit_id"].ToString() + ");return false;";
                string descr              = thisRow["credit_voucher_descr"].ToString();
                string descrShortened     = descr.Length > 42 ? descr.Substring(0, 40) + ".." : descr;
                lblDescr.Text             = "<a href='javascript:void(0)' onclick='" + jsViewVoucher + "' title='" + descr + "'>" + descrShortened + "</a>";
            }
        }

    }

    #endregion
    
    #region GrdCondition

    protected void FillGrdCondition(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        DataTable dt = ConditionDB.GetDataTable(false);
        Hashtable conditionHash = ConditionDB.GetHashtable(false);

        Session["patientcondition_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdCondition.DataSource = dt;
            try
            {
                GrdCondition.DataBind();

                Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);
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


                    if (selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                    {
                        PatientCondition ptCondition = (PatientCondition)selectedConditions[Convert.ToInt32(lblId.Text)];

                        chkSelect.Checked = selectedConditions[Convert.ToInt32(lblId.Text)] != null;

                        if (condition.ShowDate)
                        {
                            if (ptCondition.Date != DateTime.MinValue)
                            {
                                ddlDate_Day.SelectedValue   = ptCondition.Date.Day.ToString();
                                ddlDate_Month.SelectedValue = ptCondition.Date.Month.ToString();
                                ddlDate_Year.SelectedValue  = ptCondition.Date.Year.ToString();
                            }
                        }
                        if (condition.ShowNWeeksDue)
                        {
                            ddlNbrWeeksDue.SelectedValue = ptCondition.NWeeksDue.ToString();
                        }
                        if (condition.ShowText)
                        {
                            txtText.Text = ptCondition.Text;
                        }

                    }

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
        DataTable dt = Session["patientcondition_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("condition_condition_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


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
            


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["patientcondition_is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName   = "_UnDelete";
                    btnDelete.ImageUrl      = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip       = "UnDelete";
                }
            }

            Utilities.AddConfirmationBox(e);
            Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow, false);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdCondition_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdCondition.EditIndex = -1;
        FillGrdCondition();
    }
    protected void GrdCondition_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdCondition.Rows[e.RowIndex].FindControl("lblId");

        //Condition condition = ConditionDB.GetByID(Convert.ToInt32(lblId.Text));
        //ConditionDB.Update(Convert.ToInt32(lblId.Text), txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), condition.IsDeleted);

        GrdCondition.EditIndex = -1;
        FillGrdCondition();
    }
    protected void GrdCondition_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdCondition.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //CostCentreDB.Delete(Convert.ToInt32(lblId.Text));
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrdCondition();
    }
    protected void GrdCondition_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {

            //ConditionDB.Insert(txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), false);

            FillGrdCondition();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    PatientConditionDB.UpdateInactive(id);
                else
                    PatientConditionDB.UpdateActive(id);
            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                if (Utilities.IsDev())
                    SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
                else
                    SetErrorMessage("Can not delete because other records depend on this");
            }

            FillGrdCondition();
        }

    }
    protected void GrdCondition_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdCondition.EditIndex = e.NewEditIndex;
        FillGrdCondition();
    }
    protected void GrdCondition_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdCondition.EditIndex >= 0)
            return;

        GrdCondition_Sort(e.SortExpression);
    }

    protected void GrdCondition_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["patientcondition_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["patientcondition_sortexpression"] == null)
                Session["patientcondition_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["patientcondition_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["patientcondition_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdCondition.DataSource = dataView;
            GrdCondition.DataBind();
        }
    }

    #endregion

    #region GrdConditionView

    protected void FillGrdConditionView(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        DataTable dt = ConditionDB.GetDataTable(false);
        Hashtable conditionHash = ConditionDB.GetHashtable(false);

        Session["patientconditionview_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdConditionView.DataSource = dt;
            try
            {
                GrdConditionView.DataBind();

                Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);

                int nRows = 0;
                for(int i = GrdConditionView.Rows.Count-1 ; i>= 0; i--)
                {
                    Label lblId             = GrdConditionView.Rows[i].FindControl("lblId") as Label;

                    Label lblDate           = GrdConditionView.Rows[i].FindControl("lblDate") as Label;

                    Label lblNextDue        = GrdConditionView.Rows[i].FindControl("lblNextDue") as Label;
                    Label lblDateDue        = GrdConditionView.Rows[i].FindControl("lblDateDue") as Label;

                    Label lblAdditionalInfo = GrdConditionView.Rows[i].FindControl("lblAdditionalInfo") as Label;
                    Label lblText           = GrdConditionView.Rows[i].FindControl("lblText") as Label;
                    
                    System.Web.UI.HtmlControls.HtmlControl br_date      = (System.Web.UI.HtmlControls.HtmlControl)GrdConditionView.Rows[i].FindControl("br_date");
                    System.Web.UI.HtmlControls.HtmlControl br_nweeksdue = (System.Web.UI.HtmlControls.HtmlControl)GrdConditionView.Rows[i].FindControl("br_nweeksdue");
                    System.Web.UI.HtmlControls.HtmlControl br_text      = (System.Web.UI.HtmlControls.HtmlControl)GrdConditionView.Rows[i].FindControl("br_text");


                    if (lblId == null)
                        continue;

                    Condition condition = (Condition)conditionHash[Convert.ToInt32(lblId.Text)];

                    br_date.Visible             = condition.ShowDate;
                    lblDate.Visible             = condition.ShowDate;
                    br_nweeksdue.Visible        = condition.ShowNWeeksDue;
                    lblNextDue.Visible          = condition.ShowNWeeksDue;
                    lblDateDue.Visible          = condition.ShowNWeeksDue;
                    br_text.Visible             = condition.ShowText;
                    lblText.Visible             = condition.ShowText;
                    lblAdditionalInfo.Visible   = condition.ShowText;


                    if (selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                    {
                        PatientCondition ptCondition = (PatientCondition)selectedConditions[Convert.ToInt32(lblId.Text)];

                        if (condition.ShowDate)
                        {
                            lblDate.Text = ptCondition.Date == DateTime.MinValue ? "[Date Not Set]" : ptCondition.Date.ToString("d MMM, yyyy");
                        }
                        if (condition.ShowNWeeksDue)
                        {
                            bool expired = ptCondition.Date != DateTime.MinValue && ptCondition.Date.AddDays(7 * ptCondition.NWeeksDue) < DateTime.Today;
                            lblDateDue.Text = ptCondition.Date == DateTime.MinValue ? "[Date Not Set]" : (expired ? "<font color=\"red\">" : "") + ptCondition.Date.AddDays(7 * ptCondition.NWeeksDue).ToString("d MMM, yyyy") + (expired ? "</font>" : "");
                        }
                        if (condition.ShowText)
                        {
                            lblText.Text = ptCondition.Text.Length == 0 ? "[Blank]" : ptCondition.Text;
                        }

                        nRows++;
                    }
                    else
                    {
                        GrdConditionView.Rows[i].Visible = false;
                    }

                }

                if (nRows == 0)
                {
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        dt.Rows.RemoveAt(i);

                    dt.Rows.Add(dt.NewRow());
                    GrdConditionView.DataSource = dt;
                    GrdConditionView.DataBind();

                    int TotalColumns = GrdConditionView.Rows[0].Cells.Count;
                    GrdConditionView.Rows[0].Cells.Clear();
                    GrdConditionView.Rows[0].Cells.Add(new TableCell());
                    GrdConditionView.Rows[0].Cells[0].ColumnSpan = TotalColumns;
                    GrdConditionView.Rows[0].Cells[0].Text = "PT has no conditions";

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
            GrdConditionView.DataSource = dt;
            GrdConditionView.DataBind();

            int TotalColumns = GrdConditionView.Rows[0].Cells.Count;
            GrdConditionView.Rows[0].Cells.Clear();
            GrdConditionView.Rows[0].Cells.Add(new TableCell());
            GrdConditionView.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdConditionView.Rows[0].Cells[0].Text = "PT has no conditions";
        }
    }
    protected void GrdConditionView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdConditionView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["patientconditionview_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("condition_condition_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["patientconditionview_is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName   = "_UnDelete";
                    btnDelete.ImageUrl      = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip       = "UnDelete";
                }
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdConditionView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdConditionView.EditIndex = -1;
        FillGrdConditionView();
    }
    protected void GrdConditionView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdConditionView.Rows[e.RowIndex].FindControl("lblId");

        //Condition condition = ConditionDB.GetByID(Convert.ToInt32(lblId.Text));
        //ConditionDB.Update(Convert.ToInt32(lblId.Text), txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), condition.IsDeleted);

        GrdConditionView.EditIndex = -1;
        FillGrdConditionView();
    }
    protected void GrdConditionView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdConditionView.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //CostCentreDB.Delete(Convert.ToInt32(lblId.Text));
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrdConditionView();
    }
    protected void GrdConditionView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {

            //ConditionDB.Insert(txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), false);

            FillGrdConditionView();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    PatientConditionDB.UpdateInactive(id);
                else
                    PatientConditionDB.UpdateActive(id);
            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                if (Utilities.IsDev())
                    SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
                else
                    SetErrorMessage("Can not delete because other records depend on this");
            }

            FillGrdConditionView();
        }

    }
    protected void GrdConditionView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdConditionView.EditIndex = e.NewEditIndex;
        FillGrdConditionView();
    }
    protected void GrdConditionView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdConditionView.EditIndex >= 0)
            return;

        GrdConditionView_Sort(e.SortExpression);
    }

    protected void GrdConditionView_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["patientconditionview_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["patientconditionview_sortexpression"] == null)
                Session["patientconditionview_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["patientconditionview_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["patientconditionview_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdConditionView.DataSource = dataView;
            GrdConditionView.DataBind();
        }
    }

    #endregion

    #region UpdateConditions

    protected void UpdateConditions(Patient patient)
    {
        bool tblEmpty = GrdCondition.Rows.Count == 1 && GrdCondition.Rows[0].Cells[0].Text == "No Conditions";
        if (tblEmpty)
            return;

        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);
        foreach (GridViewRow row in GrdCondition.Rows)
        {
            Label        lblId          = row.FindControl("lblId") as Label;
            CheckBox     chkSelect      = row.FindControl("chkSelect") as CheckBox;
            DropDownList ddlDate_Day    = (DropDownList)row.FindControl("ddlDate_Day");
            DropDownList ddlDate_Month  = (DropDownList)row.FindControl("ddlDate_Month");
            DropDownList ddlDate_Year   = (DropDownList)row.FindControl("ddlDate_Year");
            DropDownList ddlNbrWeeksDue = (DropDownList)row.FindControl("ddlNbrWeeksDue");
            TextBox      txtText        = (TextBox)row.FindControl("txtText");


            if (lblId == null || chkSelect == null)
                continue;


            DateTime date = DateTime.MinValue;
            if (ddlDate_Day.Visible && ddlDate_Month.Visible && ddlDate_Year.Visible)
            {
                if (!IsValidDate(Convert.ToInt32(ddlDate_Day.Text), Convert.ToInt32(ddlDate_Month.Text), Convert.ToInt32(ddlDate_Year.Text)))
                {
                    SetErrorMessage("Please enter a valid date or unset all");
                    return;
                }
                else
                    date = GetDateFromForm(Convert.ToInt32(ddlDate_Day.Text), Convert.ToInt32(ddlDate_Month.Text), Convert.ToInt32(ddlDate_Year.Text));
            }

            int nweeksdue = ddlNbrWeeksDue.Visible ? Convert.ToInt32(ddlNbrWeeksDue.Text) : 0;
            string text = txtText.Visible ? txtText.Text.Trim() : string.Empty;

            if (chkSelect.Checked && selectedConditions[Convert.ToInt32(lblId.Text)] == null)
                PatientConditionDB.Insert(patient.PatientID, Convert.ToInt32(lblId.Text), date, nweeksdue, text, false);

            if (!chkSelect.Checked && selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                PatientConditionDB.Delete(patient.PatientID, Convert.ToInt32(lblId.Text));

            if (chkSelect.Checked && selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                PatientConditionDB.Update(patient.PatientID, Convert.ToInt32(lblId.Text), date, nweeksdue, text, false);
        }

        FillGrdCondition(patient);
        FillGrdConditionView(patient);

        SetErrorMessage("Updated");
    }

    public DateTime GetDateFromForm(int day, int month, int year)
    {
        if (day == -1 && month == -1 && year == -1)
            return DateTime.MinValue;

        else if (day != -1 && month != -1 && year != -1)
            return new DateTime(year, month, day);

        else
            throw new Exception("Date format is some selected and some not selected.");
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

    #region GrdRegistration

    protected void GrdRegistration_Reset()
    {
        DataTable dt = Session["addeditpatient_regorg_data"] as DataTable;
        if (dt == null)
            return;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (tblEmpty)
        {
            int TotalColumns = GrdRegistration.Rows[0].Cells.Count;
            GrdRegistration.Rows[0].Cells.Clear();
            GrdRegistration.Rows[0].Cells.Add(new TableCell());
            GrdRegistration.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdRegistration.Rows[0].Cells[0].Text = "No Organisations Allocated Yet";
        }
    }

    protected void FillOrganisationRegistrationGrid()
    {
        try
        {
            if (!IsValidFormID())
                throw new Exception("Invalid URL Parameters");

            Patient patient = PatientDB.GetByID(GetFormID());
            if (patient == null)
                throw new Exception("Invalid PT ID");


            DataTable dt = RegisterPatientDB.GetDataTable_OrganisationsOf(patient.PatientID);
            dt.DefaultView.Sort = "register_patient_id DESC"; // sort on most recently added org
            dt = dt.DefaultView.ToTable();

            Session["addeditpatient_regorg_data"] = dt;


            if (dt.Rows.Count > 0)
            {
                if (IsPostBack && Session["sortExpression"] != null && Session["sortExpression"].ToString().Length > 0)
                {
                    DataView dataView = new DataView(dt);
                    dataView.Sort = Session["sortExpression"].ToString();
                    GrdRegistration.DataSource = dataView;
                }
                else
                {
                    GrdRegistration.DataSource = dt;
                }


                GrdRegistration.DataBind();

            }
            else
            {
                dt.Rows.Add(dt.NewRow());
                GrdRegistration.DataSource = dt;
                GrdRegistration.DataBind();

                int TotalColumns = GrdRegistration.Rows[0].Cells.Count;
                GrdRegistration.Rows[0].Cells.Clear();
                GrdRegistration.Rows[0].Cells.Add(new TableCell());
                GrdRegistration.Rows[0].Cells[0].ColumnSpan = TotalColumns;
                GrdRegistration.Rows[0].Cells[0].Text = "No Organisations Allocated Yet";
            }

            SetOrgDdlList();
        }
        catch (CustomMessageException ex)
        {
            SetErrorMessage(ex.ToString());
        }
        catch (Exception ex)
        {
            SetErrorMessage("", Utilities.IsDev() ? ex.ToString() : ex.Message);
        }

    }
    protected void GrdRegistration_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Pager)
        {
            //if (!Utilities.IsDev())
                e.Row.Cells[0].CssClass = "hiddencol";
        }
    }
    protected void GrdRegistration_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        UserView userView = UserView.GetInstance();

        DataTable dt = Session["addeditpatient_regorg_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("register_patient_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];

            HyperLink lnkBookings = (HyperLink)e.Row.FindControl("lnkBookings");
            if (lnkBookings != null)
            {
                lnkBookings.NavigateUrl = string.Format("~/BookingsV2.aspx?orgs={0}&patient={1}", Convert.ToInt32(thisRow["organisation_id"]), patient.PatientID);

                lnkBookings.Visible = (userView.IsClinicView   && Convert.ToInt32(thisRow["organisation_type_id"]) == 218) ||
                                      (userView.IsAgedCareView && (new List<int> { 139, 367, 372 }).Contains(Convert.ToInt32(thisRow["organisation_type_id"])));
            }


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdRegistration_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdRegistration.EditIndex = -1;
        FillOrganisationRegistrationGrid();
    }
    protected void GrdRegistration_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlOrganisation = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlOrganisation");

        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        RegisterPatientDB.Update(Convert.ToInt32(lblId.Text), Convert.ToInt32(ddlOrganisation.SelectedValue), patient.PatientID);

        GrdRegistration.EditIndex = -1;
        FillOrganisationRegistrationGrid();
    }
    protected void GrdRegistration_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");

        RegisterPatient registerPatient = RegisterPatientDB.GetByID(Convert.ToInt32(lblId.Text));
        if (BookingDB.GetCountByPatientAndOrg(registerPatient.Patient.PatientID, registerPatient.Organisation.OrganisationID) > 0)
        {
            SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' because there exists a booking for this patient there.");
            return;
        }

        if (registerPatient.Organisation.IsAgedCare)
        {
            int agedCareOrgRegistrations = RegisterPatientDB.GetCountByPatientAndOrgTypeGroup(registerPatient.Patient.PatientID, "6");
            if (agedCareOrgRegistrations < 2)
            {
                SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' until another Fac/Wing/Unit has been added.");
                return;
            }
        }


        try
        {
            RegisterPatientDB.UpdateInactive(Convert.ToInt32(lblId.Text), false);
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                HideTableAndSetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                HideTableAndSetErrorMessage("Can not delete because other records depend on this");
        }

        FillOrganisationRegistrationGrid();
    }
    protected void GrdRegistration_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlOrganisation = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewOrganisation");

            Patient patient = PatientDB.GetByID(GetFormID());
            if (patient == null)
            {
                HideTableAndSetErrorMessage("");
                return;
            }

            try
            {
                RegisterPatientDB.Insert(Convert.ToInt32(ddlOrganisation.SelectedValue), patient.PatientID);
            }
            catch (UniqueConstraintException)
            {
                // happens when 2 forms allow adding
                // do nothing and let form re-update
            }

            FillOrganisationRegistrationGrid();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int resiter_patient_id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                {
                    RegisterPatient registerPatient = RegisterPatientDB.GetByID(resiter_patient_id);
                    if (BookingDB.GetCountByPatientAndOrg(registerPatient.Patient.PatientID, registerPatient.Organisation.OrganisationID) > 0)
                    {
                        SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' because there exists a booking for this patient there.");
                        return;
                    }


                    if (registerPatient.Organisation.IsAgedCare)
                    {
                        int agedCareOrgRegistrations = RegisterPatientDB.GetCountByPatientAndOrgTypeGroup(registerPatient.Patient.PatientID, "6");
                        if (agedCareOrgRegistrations < 2)
                        {
                            SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' until another Fac/Wing/Unit has been added.");
                            return;
                        }
                    }

                    RegisterPatientDB.UpdateInactive(resiter_patient_id);
                }
                else
                    RegisterPatientDB.UpdateActive(resiter_patient_id);
            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                SetErrorMessage("Can not delete because other records depend on this" + (Utilities.IsDev() ? " : " + fkcEx.Message : "") );
            }

            FillOrganisationRegistrationGrid();
        }
    }
    protected void GrdRegistration_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdRegistration.EditIndex = e.NewEditIndex;
        FillOrganisationRegistrationGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdRegistration.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["addeditpatient_regorg_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression"] == null)
                Session["sortExpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["sortExpression"] = sortExpression + " " + newSortExpr;

            GrdRegistration.DataSource = dataView;
            GrdRegistration.DataBind();
        }
    }

    protected void lnkRegisterPatient_Click(object sender, EventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        try
        {

            if (ddlOrganisation.SelectedValue == string.Empty)
            {
                string organisationType = UserView.GetInstance().IsAgedCareView ? "Facilities" : "Clinics";
                SetErrorMessage("No " + organisationType + " have been entered into the system.<br />Please add at least one before trying to attach a patient to one.");
                return;
            }
            else
            {
                RegisterPatientDB.Insert(Convert.ToInt32(ddlOrganisation.SelectedValue), patient.PatientID);
            }
        }
        catch (UniqueConstraintException)
        {
            // happens when 2 forms allow adding
            // do nothing and let form re-update
        }

        FillOrganisationRegistrationGrid();

    }
    protected void SetOrgDdlList()
    {
        UserView userView = UserView.GetInstance();

        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        Organisation[] incList = RegisterPatientDB.GetOrganisationsOf(patient.PatientID);
        DataTable orgs = OrganisationDB.GetDataTable_AllNotInc(incList, true, !userView.IsClinicView, !userView.IsAgedCareView, true, true);
        orgs.DefaultView.Sort = "name ASC";

        ddlOrganisation.Items.Clear();


        lnkRegisterPatient.Visible = orgs.Rows.Count > 0;
        if (orgs.Rows.Count == 0)
        {
            string organisationTypeS = userView.IsAgedCareView ? "Facility"   : "Clinic";
            string organisationTypeP = userView.IsAgedCareView ? "Facilities" : "Clinics";

            ddlOrganisation.Items.Add(new ListItem("No " + organisationTypeP + " Entered In The System Yet", "-1"));
            ddlOrganisation.Enabled = false;
        }
        else
        {
            foreach (DataRowView row in orgs.DefaultView)
                ddlOrganisation.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
        }


    }

    #endregion



    #region HideTableAndSetErrorMessage, SetErrorMessage, HideErrorMessag

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
        span_header_flashing_text.Visible = false;
        span_header_flashing_text_image.Visible = false;
        span_header_ptid.Visible = false;
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