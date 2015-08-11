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

public partial class EPCDetailV2 : System.Web.UI.Page
{

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


            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);

            HealthCard health_card = HealthCardDB.GetByID(GetFormID());
            if (health_card == null)
            {
                HideTableAndSetErrorMessage("Invalid healthcard id");
                return;
            }

            if (!IsPostBack)
            {
                SetupGUI(health_card);
                ResetEPCRemainingInfo(health_card);
                if (GetUrlParamType() == UrlParamType.Edit && IsValidFormID())
                    FillEditViewForm(health_card);
                else if (GetUrlParamType() == UrlParamType.Add && IsValidFormID())
                    FillEmptyAddForm(health_card);
                else
                    HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            }
            this.GrdHealthCardEPCRemaining.EnableViewState = true;

            GrdHealthCardEPCRemaining.Visible = false;
            spn_epcremaining_row.Visible = false;

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


    #region GetUrlParamCard(), GetUrlParamType(), IsValidFormID(), GetFormID()

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

    private enum UrlParamType { Add, Edit, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "add")
            return UrlParamType.Add;
        else if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else
            return UrlParamType.None;
    }

    #endregion

    #region SetupGUI()

    public void SetupGUI(HealthCard health_card)
    {
        lblHeading.Text = "Referral Info";
        lblGenerateLettersMsg.Text = "Please wait while we generate any previous referral Letters.";


        ddlDateReferralSigned_Day.Items.Add(new ListItem("--", "-1"));
        ddlDateReferralSigned_Month.Items.Add(new ListItem("--", "-1"));
        ddlDateReferralSigned_Year.Items.Add(new ListItem("--", "-1"));

        ddlDateReferralReceived_Day.Items.Add(new ListItem("--", "-1"));
        ddlDateReferralReceived_Month.Items.Add(new ListItem("--", "-1"));
        ddlDateReferralReceived_Year.Items.Add(new ListItem("--", "-1"));

        for (int i = 1; i <= 31; i++)
        {
            ddlDateReferralSigned_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlDateReferralReceived_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        for (int i = 1; i <= 12; i++)
        {
            ddlDateReferralSigned_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlDateReferralReceived_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
        {
            ddlDateReferralSigned_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlDateReferralReceived_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }


        bool editable = GetUrlParamType() == UrlParamType.Add || GetUrlParamType() == UrlParamType.Edit;
        Utilities.SetEditControlBackColour(ddlDateReferralReceived_Day, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDateReferralReceived_Month, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDateReferralReceived_Year, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDateReferralSigned_Day, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDateReferralSigned_Month, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDateReferralSigned_Year, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }

    #endregion


    private void FillEditViewForm(HealthCard health_card)
    {
        health_card.Patient = PatientDB.GetByID(health_card.Patient.PatientID);

        if (!Utilities.IsDev())
            idRow.Attributes["class"] = "hiddencol";


        lblId.Text = health_card.HealthCardID.ToString();


        if (health_card.DateReferralSigned != DateTime.MinValue)
        {
            ddlDateReferralSigned_Day.SelectedValue = health_card.DateReferralSigned.Day.ToString();
            ddlDateReferralSigned_Month.SelectedValue = health_card.DateReferralSigned.Month.ToString();
            ddlDateReferralSigned_Year.SelectedValue = health_card.DateReferralSigned.Year.ToString();
        }
        if (health_card.DateReferralReceivedInOffice != DateTime.MinValue)
        {
            ddlDateReferralReceived_Day.SelectedValue = health_card.DateReferralReceivedInOffice.Day.ToString();
            ddlDateReferralReceived_Month.SelectedValue = health_card.DateReferralReceivedInOffice.Month.ToString();
            ddlDateReferralReceived_Year.SelectedValue = health_card.DateReferralReceivedInOffice.Year.ToString();
        }


        FillHealthCardEPCRemainingGrid();

        btnSubmit.Text = "Update Details";
        btnCancel.Visible = true;
    }

    private void FillEmptyAddForm(HealthCard health_card)
    {
        idRow.Visible = false;

        spn_epcremaining_row.Visible = false;

        ddlDateReferralReceived_Day.SelectedValue = DateTime.Now.Day.ToString();
        ddlDateReferralReceived_Month.SelectedValue = DateTime.Now.Month.ToString();
        ddlDateReferralReceived_Year.SelectedValue = DateTime.Now.Year.ToString();

        ddlDateReferralSigned_Year.SelectedValue = DateTime.Now.Year.ToString();

        btnSubmit.Text = "Add Referral";
        btnCancel.Visible = true;
    }


    protected void DateReferralSignedAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDateReferralSigned();
    }
    protected void DateReferralReceivedAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDateReferralReceived();
    }

    protected void DateReferralSignedAllCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDateReferralSigned() && GetDateReferralSignedFromForm() != DateTime.MinValue;
    }
    protected void DateReferralReceivedAllCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDateReferralReceived() && GetDateReferralReceivedFromForm() != DateTime.MinValue;
    }

    protected void DateReferralSignedDateNotAfterTodayCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDateReferralSigned() && GetDateReferralSignedFromForm() <= DateTime.Today;
    }
    protected void DateReferralReceivedDateNotAfterTodayCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDateReferralReceived() && GetDateReferralReceivedFromForm() <= DateTime.Today;
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // close this window
        Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (!ddlDateReferralSignedAllOrNoneSet.IsValid || !ddlDateReferralReceivedAllOrNoneSet.IsValid ||
                !ddlDateReferralSignedAllSet.IsValid || !ddlDateReferralReceivedAllSet.IsValid ||
                !ddlDateReferralReceivedDateNotAfterToday.IsValid || !ddlDateReferralSignedDateNotAfterToday.IsValid)
                return;


            HealthCard hc = HealthCardDB.GetByID(GetFormID());
            hc.Patient = PatientDB.GetByID(hc.Patient.PatientID);

            if (GetUrlParamType() == UrlParamType.Edit)
            {
                if (!IsValidFormID())
                    throw new Exception("Invalid URL Parameters");

                // charles said disallow this, but marcus had an instance saying he needs it, so its commented out
                //
                //// disallow them to put in new referral date that is earlier than the previous referral date 
                //if (hc.DateReferralSigned != DateTime.MinValue && GetDateReferralSignedFromForm() != DateTime.MinValue &&
                //    GetDateReferralSignedFromForm().Date < hc.DateReferralSigned.Date)
                //{
                //    ResetEPCInfo(hc);
                //    SetErrorMessage("New referral signed date (" + GetDateReferralSignedFromForm().ToString("d MMM yyyy") + ") can not be earlier than original signed date (" + hc.DateReferralSigned.ToString("d MMM yyyy") + ").<br />Previous EPC information has been reset.");
                //    return;
                //}


                // if new epc signed date changed (and have epc remainnig > 0) generate final letter/s
                // because they may hit "edit" EPC when adding new one due to BEST working like that
                // so generate when signed date changed - just to make sure it is compliant with medicare laws (better too many letters than not enough)
                bool showDownloadPopup = false;
                if (hc.DateReferralSigned != DateTime.MinValue && GetDateReferralSignedFromForm() != DateTime.MinValue &&
                    GetDateReferralSignedFromForm().Date != hc.DateReferralSigned.Date)
                {
                    showDownloadPopup = SendLastLetters(hc);  // this mus be done before HealthCardDB.Update
                }


                // udpate card
                HealthCardDB.Update(hc.HealthCardID, hc.Patient.PatientID, hc.Organisation.OrganisationID, hc.CardName, hc.CardNbr, hc.CardFamilyMemberNbr, hc.ExpiryDate,
                    GetDateReferralSignedFromForm(), GetDateReferralReceivedFromForm(), hc.IsActive, Convert.ToInt32(Session["StaffID"]), hc.AreaTreated);


                // update items
                DataTable epcRemaining = HealthCardEPCRemainingDB.GetDataTable_ByHealthCardID(hc.HealthCardID);
                foreach (RepeaterItem item in lstEPCRemaining.Items)
                {
                    Label lblFieldDescr = (Label)item.FindControl("lblFieldDescr");
                    DropDownList ddlNumServicesRamining = (DropDownList)item.FindControl("ddlNumServicesRamining");
                    HiddenField lblFieldID = (HiddenField)item.FindControl("lblFieldID");


                    int id = -1;
                    int prevNbr = -1;
                    foreach (DataRow r in epcRemaining.Rows)
                    {
                        if (r["field_field_id"].ToString() == lblFieldID.Value.ToString())
                        {
                            id = Convert.ToInt32(r["epcremaining_health_card_epc_remaining_id"]);
                            prevNbr = Convert.ToInt32(r["epcremaining_num_services_remaining"]);
                        }
                    }

                    if (id != -1)  // update
                    {

                        HealthCardEPCRemainingDB.UpdateNumServicesRemaining(id, Convert.ToInt32(ddlNumServicesRamining.SelectedValue));
                        HealthCardEPCRemainingChangeHistoryDB.Insert(id, Convert.ToInt32(Session["StaffID"]), DateTime.Now, prevNbr, Convert.ToInt32(ddlNumServicesRamining.SelectedValue));
                    }
                    else  // add
                    {
                        if (Convert.ToInt32(ddlNumServicesRamining.SelectedValue) > 0)
                        {
                            id = HealthCardEPCRemainingDB.Insert(GetFormID(), Convert.ToInt32(lblFieldID.Value), Convert.ToInt32(ddlNumServicesRamining.SelectedValue));
                            HealthCardEPCRemainingChangeHistoryDB.Insert(id, Convert.ToInt32(Session["StaffID"]), DateTime.Now, -1, Convert.ToInt32(ddlNumServicesRamining.SelectedValue));
                        }
                    }
                }




                // set change history item, and remove all other epc info (epc remaning items) set to deleted/inactive
                if (GetDateReferralSignedFromForm() != hc.DateReferralSigned ||
                    GetDateReferralReceivedFromForm() != hc.DateReferralReceivedInOffice)
                {
                    HealthCardEPCChangeHistoryDB.Insert(hc.HealthCardID, Convert.ToInt32(Session["StaffID"]), false,
                        hc.DateReferralSigned, hc.DateReferralReceivedInOffice,
                        GetDateReferralSignedFromForm(), GetDateReferralReceivedFromForm());
                }


                // close this window
                if (Utilities.IsMobileDevice(Request))
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>opener.location.href=opener.location.href;self.close();</script>");
                else
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=" + (showDownloadPopup ? "true" : "false") + ";self.close();</script>");

            }
            else if (GetUrlParamType() == UrlParamType.Add)
            {
                if (!IsValidFormID())
                    throw new Exception("Invalid URL Parameters");

                // charles said disallow this, but marcus had an instance saying he needs it, so its commented out
                //
                //// disallow them to put in new referral date that is earlier than the previous referral date 
                //if (hc.DateReferralSigned != DateTime.MinValue && GetDateReferralSignedFromForm() != DateTime.MinValue &&
                //    GetDateReferralSignedFromForm().Date < hc.DateReferralSigned.Date)
                //{
                //    ResetEPCInfo(hc);
                //    SetErrorMessage("New referral signed date (" + GetDateReferralSignedFromForm().ToString("d MMM yyyy") + ") can not be earlier than original signed date (" + hc.DateReferralSigned.ToString("d MMM yyyy") + ").<br />Previous EPC information has been reset.");
                //    return;
                //}


                // 	if new epc added and have epc remainnig > 0 generate final letter/s 
                bool showDownloadPopup = SendLastLetters(hc);  // this mus be done before HealthCardDB.Update

                // add to change history (and say its a new card)
                HealthCardEPCChangeHistoryDB.Insert(hc.HealthCardID, Convert.ToInt32(Session["StaffID"]), true,
                    hc.DateReferralSigned, hc.DateReferralReceivedInOffice,
                    GetDateReferralSignedFromForm(), GetDateReferralReceivedFromForm());

                // set change history item, and remove all other epc info (epc remaning items) set to deleted/inactive
                HealthCardEPCRemainingDB.UpdateAllAsDeleted(hc.HealthCardID, Convert.ToInt32(Session["StaffID"]));


                // update info
                HealthCardDB.Update(hc.HealthCardID, hc.Patient.PatientID, hc.Organisation.OrganisationID, hc.CardName, hc.CardNbr, hc.CardFamilyMemberNbr, hc.ExpiryDate,
                    GetDateReferralSignedFromForm(), GetDateReferralReceivedFromForm(), hc.IsActive, Convert.ToInt32(Session["StaffID"]), hc.AreaTreated);

                // add action letter history saying received
                HealthCardActionDB.Insert(hc.HealthCardID, 0, GetDateReferralSignedFromForm());

                // add items
                foreach (RepeaterItem item in lstEPCRemaining.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        Label lblFieldDescr = (Label)item.FindControl("lblFieldDescr");
                        DropDownList ddlNumServicesRamining = (DropDownList)item.FindControl("ddlNumServicesRamining");
                        HiddenField lblFieldID = (HiddenField)item.FindControl("lblFieldID");

                        if (Convert.ToInt32(ddlNumServicesRamining.SelectedValue) > 0)
                        {
                            int id = HealthCardEPCRemainingDB.Insert(GetFormID(), Convert.ToInt32(lblFieldID.Value), Convert.ToInt32(ddlNumServicesRamining.SelectedValue));
                            HealthCardEPCRemainingChangeHistoryDB.Insert(id, Convert.ToInt32(Session["StaffID"]), DateTime.Now, -1, Convert.ToInt32(ddlNumServicesRamining.SelectedValue));
                        }
                    }
                }

                // if they are aged care, update their ac patient type
                if (hc.Patient.ACInvOffering != null) // they have aged care stuff
                {

                    System.Collections.Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
                    if (hc.Patient.ACInvOffering != null)
                        hc.Patient.ACInvOffering = (Offering)offeringsHash[hc.Patient.ACInvOffering.OfferingID];
                    if (hc.Patient.ACPatOffering != null)
                        hc.Patient.ACPatOffering = (Offering)offeringsHash[hc.Patient.ACPatOffering.OfferingID];



                    // Current      Add Medicare Ref    Add DVA Ref                                            Add TAC Ref
                    // -------      ----------------    -----------                                            -----------
                    //
                    // LC           MC (LC)             DVA (LC)                                              .
                    // LCF          MC (LCF)            DVA (LCF)                                             .
                    // HC           MC (HC)             <no change>                                           .
                    //
                    // LCE          MC (prev)           DVA (prev)                                            .
                    // HCE          MC (prev)           <no change>                                           .
                    //
                    // Medicare     <no change>         if prev LC/LCF then change to DVA, else <no change>   .
                    // DVA          MC (prev)           <no change>                                           .
                    //
                    // TAC          MC (prev)           ..                                                    <no change>
                    //
                    // **If DVA and has MC (or vice versa) just change to new card if can ... make it easy    .


                    // offering_id	name
                    //
                    // 1	LC
                    // 7	LCF
                    // 2	HC
                    // 8	HCU
                    //
                    // 3	DVA
                    // 4	Medicare
                    // 9	TAC
                    //
                    // 5	HCE
                    // 6	LCE


                    if (hc.Organisation.OrganisationID == -1) // Medicare
                    {
                        if ((new List<int> { 2, 3, 4, 5 }).Contains(hc.Patient.ACInvOffering.AgedCarePatientType.ID))
                        {
                            PatientHistoryDB.Insert(hc.Patient.PatientID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeleted, hc.Patient.IsDeceased,
                                                    hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                                    hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, hc.Patient.ACInvOffering == null ? -1 : hc.Patient.ACInvOffering.OfferingID, hc.Patient.ACPatOffering == null ? -1 : hc.Patient.ACPatOffering.OfferingID, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo, 
                                                    hc.Patient.Person.Title.ID, hc.Patient.Person.Firstname, hc.Patient.Person.Middlename, hc.Patient.Person.Surname, hc.Patient.Person.Nickname, hc.Patient.Person.Gender, hc.Patient.Person.Dob, Convert.ToInt32(Session["StaffID"]));

                            int acInvOfferingID_New = 4;  // change to medicare
                            int acPatOfferingID_New = hc.Patient.ACInvOffering.OfferingID;  // change it

                            PatientDB.Update(hc.Patient.PatientID, hc.Patient.Person.PersonID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeceased, 
                                             hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                             hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, acInvOfferingID_New, acPatOfferingID_New, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo);
                        }
                        else if ((new List<int> { 6, 7, 9, 10 }).Contains(hc.Patient.ACInvOffering.AgedCarePatientType.ID))
                        {
                            PatientHistoryDB.Insert(hc.Patient.PatientID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeleted, hc.Patient.IsDeceased,
                                                    hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                                    hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, hc.Patient.ACInvOffering == null ? -1 : hc.Patient.ACInvOffering.OfferingID, hc.Patient.ACPatOffering == null ? -1 : hc.Patient.ACPatOffering.OfferingID, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo, 
                                                    hc.Patient.Person.Title.ID, hc.Patient.Person.Firstname, hc.Patient.Person.Middlename, hc.Patient.Person.Surname, hc.Patient.Person.Nickname, hc.Patient.Person.Gender, hc.Patient.Person.Dob, Convert.ToInt32(Session["StaffID"]));

                            int acInvOfferingID_New = 4;  // change to medicare
                            int acPatOfferingID_New = hc.Patient.ACPatOffering.OfferingID;  // leave it the same

                            PatientDB.Update(hc.Patient.PatientID, hc.Patient.Person.PersonID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeceased, 
                                             hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                             hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, acInvOfferingID_New, acPatOfferingID_New, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo);
                        }
                        else if ((new List<int> { 8 }).Contains(hc.Patient.ACInvOffering.AgedCarePatientType.ID))
                        {
                            ; // do nothing if already medicare
                        }
                        else // (?)
                            ; //
                    }
                    if (hc.Organisation.OrganisationID == -2) // DVA
                    {
                        if ((new List<int> { 2, 4 }).Contains(hc.Patient.ACInvOffering.AgedCarePatientType.ID))  // LC/LCF change it
                        {
                            PatientHistoryDB.Insert(hc.Patient.PatientID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeleted, hc.Patient.IsDeceased,
                                                    hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                                    hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, hc.Patient.ACInvOffering == null ? -1 : hc.Patient.ACInvOffering.OfferingID, hc.Patient.ACPatOffering == null ? -1 : hc.Patient.ACPatOffering.OfferingID, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo, 
                                                    hc.Patient.Person.Title.ID, hc.Patient.Person.Firstname, hc.Patient.Person.Middlename, hc.Patient.Person.Surname, hc.Patient.Person.Nickname, hc.Patient.Person.Gender, hc.Patient.Person.Dob, Convert.ToInt32(Session["StaffID"]));

                            int acInvOfferingID_New = 3;  // change to dva
                            int acPatOfferingID_New = hc.Patient.ACInvOffering.OfferingID;  // change it

                            PatientDB.Update(hc.Patient.PatientID, hc.Patient.Person.PersonID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeceased,
                                             hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                             hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, acInvOfferingID_New, acPatOfferingID_New, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo);
                        }
                        else if ((new List<int> { 3, 5, 7 }).Contains(hc.Patient.ACInvOffering.AgedCarePatientType.ID))  // HC/HCU dont change it
                        {
                            ; // do nothing if HC since DVA is only for LC
                        }
                        else if ((new List<int> { 6 }).Contains(hc.Patient.ACInvOffering.AgedCarePatientType.ID))  // LCE
                        {
                            PatientHistoryDB.Insert(hc.Patient.PatientID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeleted, hc.Patient.IsDeceased,
                                                    hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                                    hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, hc.Patient.ACInvOffering == null ? -1 : hc.Patient.ACInvOffering.OfferingID, hc.Patient.ACPatOffering == null ? -1 : hc.Patient.ACPatOffering.OfferingID, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo, 
                                                    hc.Patient.Person.Title.ID, hc.Patient.Person.Firstname, hc.Patient.Person.Middlename, hc.Patient.Person.Surname, hc.Patient.Person.Nickname, hc.Patient.Person.Gender, hc.Patient.Person.Dob, Convert.ToInt32(Session["StaffID"]));

                            int acInvOfferingID_New = 3;  // change to dva
                            int acPatOfferingID_New = hc.Patient.ACPatOffering.OfferingID;  // leave it the same

                            PatientDB.Update(hc.Patient.PatientID, hc.Patient.Person.PersonID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeceased,
                                             hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                             hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, acInvOfferingID_New, acPatOfferingID_New, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo);
                        }
                        else if ((new List<int> { 8, 10 }).Contains(hc.Patient.ACInvOffering.AgedCarePatientType.ID) && (new List<int> { 2, 4 }).Contains(hc.Patient.ACPatOffering.AgedCarePatientType.ID))  // medicare/tac and prev was LC/LCF
                        {
                            PatientHistoryDB.Insert(hc.Patient.PatientID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeleted, hc.Patient.IsDeceased,
                                                    hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                                    hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, hc.Patient.ACInvOffering == null ? -1 : hc.Patient.ACInvOffering.OfferingID, hc.Patient.ACPatOffering == null ? -1 : hc.Patient.ACPatOffering.OfferingID, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo, 
                                                    hc.Patient.Person.Title.ID, hc.Patient.Person.Firstname, hc.Patient.Person.Middlename, hc.Patient.Person.Surname, hc.Patient.Person.Nickname, hc.Patient.Person.Gender, hc.Patient.Person.Dob, Convert.ToInt32(Session["StaffID"]));

                            int acInvOfferingID_New = 3;  // change to dva
                            int acPatOfferingID_New = hc.Patient.ACPatOffering.OfferingID;  // leave it the same

                            PatientDB.Update(hc.Patient.PatientID, hc.Patient.Person.PersonID, hc.Patient.IsClinicPatient, hc.Patient.IsGPPatient, hc.Patient.IsDeceased,
                                             hc.Patient.FlashingText, hc.Patient.FlashingTextAddedBy == null ? -1 : hc.Patient.FlashingTextAddedBy.StaffID, hc.Patient.FlashingTextLastModifiedDate,
                                             hc.Patient.PrivateHealthFund, hc.Patient.ConcessionCardNumber, hc.Patient.ConcessionCardExpiryDate, hc.Patient.IsDiabetic, hc.Patient.IsMemberDiabetesAustralia, hc.Patient.DiabeticAAassessmentReviewDate, acInvOfferingID_New, acPatOfferingID_New, hc.Patient.Login, hc.Patient.Pwd, hc.Patient.IsCompany, hc.Patient.ABN, hc.Patient.NextOfKinName, hc.Patient.NextOfKinRelation, hc.Patient.NextOfKinContactInfo);
                        }
                        else if ((new List<int> { 8, 10 }).Contains(hc.Patient.ACInvOffering.AgedCarePatientType.ID))  // medicare/tac and prev was NOT LC/LCF
                        {
                            ; // do nothing if medicare/tac and prev was not LC/LCF since DVA is only for LC
                        }
                        else if ((new List<int> { 9 }).Contains(hc.Patient.ACInvOffering.AgedCarePatientType.ID))
                        {
                            ; // do nothing if already dva
                        }
                        else // (?)
                            ; //
                    }

                }


                // close this window
                if (Utilities.IsMobileDevice(Request))
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>opener.location.href=opener.location.href;self.close();</script>");
                else
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=" + (showDownloadPopup ? "true" : "false") + ";self.close();</script>");
            }
            else
            {
                throw new Exception("Invalid URL Parameters");
            }
        }
        catch (CustomMessageException ex)
        {
            SetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            SetErrorMessage("", ex.ToString());
        }
    }


    // 	if "new" epc added and have epc remainnig > 0 generate final letter
    protected bool SendLastLetters(HealthCard hc)
    {
        System.Collections.ArrayList letters = new System.Collections.ArrayList();

        if (hc.DateReferralSigned == DateTime.MinValue) // can not get invoices since signed date if no signed date set
            return false;



        // often the user will add a new referrer and THEN add a new EPC card
        // so we will try to get the referrer that was in the system yesterday. else if non, then use most recent. 
        PatientReferrer[] allPatientsReferrers = PatientReferrerDB.GetEPCPatientReferrersOf(hc.Patient.PatientID);
        PatientReferrer patientReferrerYesterday = PatientReferrerDB.GetMostRecentlyAddedBeforeToday(hc.Patient.PatientID, allPatientsReferrers);
        PatientReferrer patientReferrer = patientReferrerYesterday != null ? patientReferrerYesterday : (allPatientsReferrers.Length == 0 ? null : allPatientsReferrers[allPatientsReferrers.Length - 1]);
        if (patientReferrerYesterday == null)
            return false;

        //PatientReferrer[] patientReferrers = PatientReferrerDB.GetActiveEPCPatientReferrersOf(hc.Patient.PatientID);
        //if (patientReferrers.Length != 1)
        //    return false;


        bool AutoSendFaxesAsEmailsIfNoEmailExistsToGPs = Convert.ToInt32(SystemVariableDB.GetByDescr("AutoSendFaxesAsEmailsIfNoEmailExistsToGPs").Value) == 1;
        string[] emails = ContactDB.GetEmailsByEntityID(patientReferrer.RegisterReferrer.Organisation.EntityID);
        string[] faxes  = ContactDB.GetFaxesByEntityID(patientReferrer.RegisterReferrer.Organisation.EntityID);

        string toEmail = null;
        if (emails.Length > 0)
            toEmail = string.Join(",", emails);
        else if (AutoSendFaxesAsEmailsIfNoEmailExistsToGPs && faxes.Length > 0)
            toEmail = faxes[0] + "@fax.houseofit.com.au";

        if (Utilities.IsDev())
            if (toEmail != null) toEmail = "eli.pollak@mediclinic.com.au";


        int letterPrintHistorySendMethodID = toEmail == null ? 1 : 2;

        Site[] sites = SiteDB.GetAll();

        Booking lastBooking = null;
        HealthCardEPCRemaining[] epcRemainingList = HealthCardEPCRemainingDB.GetByHealthCardID(hc.HealthCardID);
        foreach (HealthCardEPCRemaining epcRemaining in epcRemainingList)
        {
            if (epcRemaining.NumServicesRemaining == 0)
                continue;

            Invoice[] medicareInvoices = InvoiceDB.GetMedicareByPatientAndDateRangeStartDateAsc(hc.Patient.PatientID, hc.DateReferralSigned, hc.DateReferralSigned.AddYears(1).AddDays(-1), epcRemaining.Field.ID);
            if (medicareInvoices.Length == 0)
                continue;

            lastBooking = medicareInvoices[medicareInvoices.Length - 1].Booking;


            // send last letter
            Letter.FileContents fileContentsEPCTreatment = Letter.SendTreatmentLetter(toEmail != null ? Letter.FileFormat.PDF : Letter.FileFormat.Word, lastBooking, lastBooking.Patient, hc, Letter.TreatmentLetterType.LastWhenReplacingEPC, Booking.InvoiceType.Medicare, lastBooking.Offering.Field.ID, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), patientReferrer.RegisterReferrer.Referrer, true, letterPrintHistorySendMethodID);
            if (fileContentsEPCTreatment != null)
                letters.Add(fileContentsEPCTreatment);
        }




        if (letters.Count == 0)
        {
            return false;
        }
        else if (toEmail != null)
        {
            Site site = SiteDB.GetSiteByType(lastBooking.Organisation.IsAgedCare ? SiteDB.SiteType.AgedCare : SiteDB.SiteType.Clinic, sites);
            if (!Utilities.IsDev())
                Letter.EmailSystemLetter(site.Name, toEmail, (Letter.FileContents[])letters.ToArray(typeof(Letter.FileContents)));
            return false;
        }
        else if (letters.Count == 1)
        {
            letters[0] = Letter.ConvertContentsToPDF((Letter.FileContents)letters[0]);

            Session["downloadFile_Contents"] = ((Letter.FileContents)letters[0]).Contents;
            Session["downloadFile_DocName"] = ((Letter.FileContents)letters[0]).DocName;
            return true;
        }
        else
        {
            //
            // merge
            //

            string docName = "Referral Letters.pdf"; // +System.IO.Path.GetExtension(((Letter.FileContents)letters[0]).DocName);


            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            if (!System.IO.Directory.Exists(tmpLettersDirectory))
                throw new CustomMessageException("Temp letters directory doesn't exist");

            string[] docs = new string[letters.Count];
            for (int i = 0; i < letters.Count; i++)
            {
                string tmpFileName_EPCTreatment = FileHelper.GetTempFileName(tmpLettersDirectory + ((Letter.FileContents)letters[i]).DocName);
                System.IO.File.WriteAllBytes(tmpFileName_EPCTreatment, ((Letter.FileContents)letters[i]).Contents);
                docs[i] = tmpFileName_EPCTreatment;
            }

            string tmpFinalFileName = Letter.MergeMultipleDocuments(docs, tmpLettersDirectory + docName);
            byte[] fileContents = System.IO.File.ReadAllBytes(tmpFinalFileName);

            foreach (string doc in docs)
                System.IO.File.Delete(doc);
            System.IO.File.Delete(tmpFinalFileName);

            // set in session for AddEditHealthCard page to show a popup to get the letter
            Session["downloadFile_Contents"] = fileContents;
            Session["downloadFile_DocName"] = docName;

            return true;
        }
    }


    public void ResetEPCInfo(HealthCard hc)
    {
        ddlDateReferralSigned_Day.SelectedValue = hc.DateReferralSigned.Day.ToString();
        ddlDateReferralSigned_Month.SelectedValue = hc.DateReferralSigned.Month.ToString();
        ddlDateReferralSigned_Year.SelectedValue = hc.DateReferralSigned.Year.ToString();

        if (hc.DateReferralReceivedInOffice != DateTime.MinValue)
        {
            ddlDateReferralReceived_Day.SelectedValue = hc.DateReferralReceivedInOffice.Day.ToString();
            ddlDateReferralReceived_Month.SelectedValue = hc.DateReferralReceivedInOffice.Month.ToString();
            ddlDateReferralReceived_Year.SelectedValue = hc.DateReferralReceivedInOffice.Year.ToString();
        }
        else
        {
            ddlDateReferralReceived_Day.SelectedValue = "-1";
            ddlDateReferralReceived_Month.SelectedValue = "-1";
            ddlDateReferralReceived_Year.SelectedValue = "-1";
        }

    }
    public void ResetEPCRemainingInfo(HealthCard hc)
    {
        DataTable epcRemaining = HealthCardEPCRemainingDB.GetDataTable_ByHealthCardID(hc.HealthCardID);
        string typesIDsAlreadyUsed = "0";
        foreach (DataRow row in epcRemaining.Rows)
            typesIDsAlreadyUsed += (typesIDsAlreadyUsed.Length == 0 ? "" : ",") + row["epcremaining_field_id"].ToString();
        DataTable fields = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "has_offerings = 1 AND field_id != 0", "descr", "field_id", "descr");

        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[] { new DataColumn("field_desc"), new DataColumn("field_id"), new DataColumn("num_remaining") });


        foreach (DataRow row in fields.Rows)
        {
            int num_remaining = 0;

            if (GetUrlParamType() == UrlParamType.Edit)
            {
                foreach (DataRow r in epcRemaining.Rows)
                {
                    //if (Convert.ToInt32(r["field_field_id"]) == Convert.ToInt32(row["field_id"]))
                    if (r["field_field_id"].ToString() == row["field_id"].ToString())
                        num_remaining = Convert.ToInt32(r["epcremaining_num_services_remaining"]);
                }
            }
            dt.Rows.Add(row["descr"], row["field_id"], num_remaining);
        }
        DataView dv = dt.DefaultView;
        dv.Sort = "num_remaining DESC,field_desc";
        dt = dv.ToTable();

        lstEPCRemaining.DataSource = dt;
        lstEPCRemaining.DataBind();
    }

    public DateTime GetDateReferralReceivedFromForm()
    {
        if (ddlDateReferralReceived_Day.SelectedValue == "-1" && ddlDateReferralReceived_Month.SelectedValue == "-1" && ddlDateReferralReceived_Year.SelectedValue == "-1")
            return DateTime.MinValue;

        else if (ddlDateReferralReceived_Day.SelectedValue != "-1" && ddlDateReferralReceived_Month.SelectedValue != "-1" && ddlDateReferralReceived_Year.SelectedValue != "-1")
            return new DateTime(Convert.ToInt32(ddlDateReferralReceived_Year.SelectedValue), Convert.ToInt32(ddlDateReferralReceived_Month.SelectedValue), Convert.ToInt32(ddlDateReferralReceived_Day.SelectedValue));

        else
            throw new Exception("Date format is some selected and some not selected.");
    }
    public bool IsValidDateReferralReceived()
    {
        bool isvalid = ((ddlDateReferralReceived_Day.SelectedValue == "-1" && ddlDateReferralReceived_Month.SelectedValue == "-1" && ddlDateReferralReceived_Year.SelectedValue == "-1") ||
                        (ddlDateReferralReceived_Day.SelectedValue != "-1" && ddlDateReferralReceived_Month.SelectedValue != "-1" && ddlDateReferralReceived_Year.SelectedValue != "-1" && IsValidDate(Convert.ToInt32(ddlDateReferralReceived_Day.SelectedValue), Convert.ToInt32(ddlDateReferralReceived_Month.SelectedValue), Convert.ToInt32(ddlDateReferralReceived_Year.SelectedValue))));


        return isvalid;
    }
    public DateTime GetDateReferralSignedFromForm()
    {
        if (ddlDateReferralSigned_Day.SelectedValue == "-1" && ddlDateReferralSigned_Month.SelectedValue == "-1" && ddlDateReferralSigned_Year.SelectedValue == "-1")
            return DateTime.MinValue;

        else if (ddlDateReferralSigned_Day.SelectedValue != "-1" && ddlDateReferralSigned_Month.SelectedValue != "-1" && ddlDateReferralSigned_Year.SelectedValue != "-1")
            return new DateTime(Convert.ToInt32(ddlDateReferralSigned_Year.SelectedValue), Convert.ToInt32(ddlDateReferralSigned_Month.SelectedValue), Convert.ToInt32(ddlDateReferralSigned_Day.SelectedValue));

        else
            throw new Exception("Date format is some selected and some not selected.");
    }
    public bool IsValidDateReferralSigned()
    {
        bool isvalid = ((ddlDateReferralSigned_Day.SelectedValue == "-1" && ddlDateReferralSigned_Month.SelectedValue == "-1" && ddlDateReferralSigned_Year.SelectedValue == "-1") ||
                        (ddlDateReferralSigned_Day.SelectedValue != "-1" && ddlDateReferralSigned_Month.SelectedValue != "-1" && ddlDateReferralSigned_Year.SelectedValue != "-1" && IsValidDate(Convert.ToInt32(ddlDateReferralSigned_Day.SelectedValue), Convert.ToInt32(ddlDateReferralSigned_Month.SelectedValue), Convert.ToInt32(ddlDateReferralSigned_Year.SelectedValue))));
        return isvalid;
    }

    public bool IsValidDate(int day, int month, int year)
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


    #region GrdHealthCardEPCRemaining

    protected void FillHealthCardEPCRemainingGrid()
    {

        if (GetUrlParamType() != UrlParamType.Edit || !IsValidFormID())
        {
            GrdHealthCardEPCRemaining.Visible = false;
            return;
        }

        HealthCard health_card = HealthCardDB.GetByID(GetFormID());
        if (health_card == null)
        {
            GrdHealthCardEPCRemaining.Visible = false;
            return;
        }

        DataTable dt = HealthCardEPCRemainingDB.GetDataTable_ByHealthCardID(health_card.HealthCardID);
        Session["epcremaining_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["epcremaining_sortexpression"] != null && Session["epcremaining_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["epcremaining_sortexpression"].ToString();
                GrdHealthCardEPCRemaining.DataSource = dataView;
            }
            else
            {
                GrdHealthCardEPCRemaining.DataSource = dt;
            }


            try
            {
                GrdHealthCardEPCRemaining.DataBind();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Visible = true;
                lblErrorMessage.Text = ex.ToString();
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdHealthCardEPCRemaining.DataSource = dt;
            GrdHealthCardEPCRemaining.DataBind();

            int TotalColumns = GrdHealthCardEPCRemaining.Rows[0].Cells.Count;
            GrdHealthCardEPCRemaining.Rows[0].Cells.Clear();
            GrdHealthCardEPCRemaining.Rows[0].Cells.Add(new TableCell());
            GrdHealthCardEPCRemaining.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdHealthCardEPCRemaining.Rows[0].Cells[0].Text = "No service types added.";
        }
    }
    protected void GrdHealthCardEPCRemaining_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";

        // hide delete column when editing
        e.Row.Cells[7].Visible = (GrdHealthCardEPCRemaining.EditIndex == -1);
    }
    protected void GrdHealthCardEPCRemaining_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["epcremaining_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);

        string typesIDsAlreadyUsed = "0";
        if (!tblEmpty)
        {
            foreach (DataRow row in dt.Rows)
                typesIDsAlreadyUsed += (typesIDsAlreadyUsed.Length == 0 ? "" : ",") + row["epcremaining_field_id"].ToString();
        }
        DataTable fields = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "has_offerings = 1 AND field_id NOT IN (" + typesIDsAlreadyUsed + ")", "descr", "field_id", "descr");


        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("epcremaining_health_card_epc_remaining_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlField = (DropDownList)e.Row.FindControl("ddlField");
            if (ddlField != null)
            {
                ddlField.DataSource = fields;
                ddlField.DataTextField = "descr";
                ddlField.DataValueField = "field_id";
                ddlField.DataBind();
                ddlField.SelectedValue = thisRow["epcremaining_field_id"].ToString();
            }

            GrdHealthCardEPCRemaining.ShowFooter = fields.Rows.Count > 0;

            DropDownList ddlNumServicesRemaining = (DropDownList)e.Row.FindControl("ddlNumServicesRemaining");
            if (ddlNumServicesRemaining != null)
            {
                for (int i = 0; i < 10; i++)
                    ddlNumServicesRemaining.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlNumServicesRemaining.SelectedValue = thisRow["epcremaining_num_services_remaining"].ToString();
            }


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlField = (DropDownList)e.Row.FindControl("ddlNewField");
            if (ddlField != null)
            {
                ddlField.DataSource = fields;
                ddlField.DataTextField = "descr";
                ddlField.DataValueField = "field_id";
                ddlField.DataBind();
            }

            DropDownList ddlNumServicesRemaining = (DropDownList)e.Row.FindControl("ddlNewNumServicesRemaining");
            if (ddlNumServicesRemaining != null)
            {
                for (int i = 0; i < 10; i++)
                    ddlNumServicesRemaining.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

    }
    protected void GrdHealthCardEPCRemaining_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdHealthCardEPCRemaining.EditIndex = -1;
        btnSubmit.Visible = true;
        FillHealthCardEPCRemainingGrid();
    }
    protected void GrdHealthCardEPCRemaining_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdHealthCardEPCRemaining.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlField = (DropDownList)GrdHealthCardEPCRemaining.Rows[e.RowIndex].FindControl("ddlField");
        DropDownList ddlNumServicesRemaining = (DropDownList)GrdHealthCardEPCRemaining.Rows[e.RowIndex].FindControl("ddlNumServicesRemaining");

        DataTable dt = Session["epcremaining_data"] as DataTable;
        DataRow[] foundRows = dt.Select("epcremaining_health_card_epc_remaining_id=" + lblId.Text);
        DataRow thisRow = foundRows[0];
        HealthCardEPCRemaining cur = HealthCardEPCRemainingDB.LoadAll(thisRow);

        if (cur.NumServicesRemaining != Convert.ToInt32(ddlNumServicesRemaining.SelectedValue))
        {
            HealthCardEPCRemainingDB.Update(Convert.ToInt32(lblId.Text), cur.HealthCardID, cur.Field.ID, Convert.ToInt32(ddlNumServicesRemaining.SelectedValue));
            HealthCardEPCRemainingChangeHistoryDB.Insert(cur.HealthCardEpcRemainingID, Convert.ToInt32(Session["StaffID"]), DateTime.Now, cur.NumServicesRemaining, Convert.ToInt32(ddlNumServicesRemaining.SelectedValue));
        }

        GrdHealthCardEPCRemaining.EditIndex = -1;
        btnSubmit.Visible = true;
        FillHealthCardEPCRemainingGrid();
    }
    protected void GrdHealthCardEPCRemaining_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdHealthCardEPCRemaining.Rows[e.RowIndex].FindControl("lblId");

        DataTable dt = Session["epcremaining_data"] as DataTable;
        DataRow[] foundRows = dt.Select("epcremaining_health_card_epc_remaining_id=" + lblId.Text);
        DataRow thisRow = foundRows[0];
        HealthCardEPCRemaining cur = HealthCardEPCRemainingDB.LoadAll(thisRow);

        HealthCardEPCRemainingDB.UpdateAsDeleted(Convert.ToInt32(lblId.Text), Convert.ToInt32(Session["StaffID"]));
        HealthCardEPCRemainingChangeHistoryDB.Insert(cur.HealthCardEpcRemainingID, Convert.ToInt32(Session["StaffID"]), DateTime.Now, cur.NumServicesRemaining, -1);

        FillHealthCardEPCRemainingGrid();
    }
    protected void GrdHealthCardEPCRemaining_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlField = (DropDownList)GrdHealthCardEPCRemaining.FooterRow.FindControl("ddlNewField");
            DropDownList ddlNumServicesRemaining = (DropDownList)GrdHealthCardEPCRemaining.FooterRow.FindControl("ddlNewNumServicesRemaining");

            int id = HealthCardEPCRemainingDB.Insert(GetFormID(), Convert.ToInt32(ddlField.SelectedValue), Convert.ToInt32(ddlNumServicesRemaining.SelectedValue));
            HealthCardEPCRemainingChangeHistoryDB.Insert(id, Convert.ToInt32(Session["StaffID"]), DateTime.Now, -1, Convert.ToInt32(ddlNumServicesRemaining.SelectedValue));

            FillHealthCardEPCRemainingGrid();
        }
    }
    protected void GrdHealthCardEPCRemaining_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdHealthCardEPCRemaining.EditIndex = e.NewEditIndex;
        btnSubmit.Visible = false;
        FillHealthCardEPCRemainingGrid();
    }
    protected void GrdHealthCardEPCRemaining_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdHealthCardEPCRemaining.EditIndex >= 0)
            return;

        GrdHealthCardEPCRemaining_Sort(e.SortExpression);
    }

    protected void GrdHealthCardEPCRemaining_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["epcremaining_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["epcremaining_sortexpression"] == null)
                Session["epcremaining_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["epcremaining_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["epcremaining_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdHealthCardEPCRemaining.DataSource = dataView;
            GrdHealthCardEPCRemaining.DataBind();
        }
    }

    #endregion


    #region SetErrorMessage, HideErrorMessage

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