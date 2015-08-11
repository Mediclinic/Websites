using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


public partial class Controls_HealthCardInfoControlV2 : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblEPCText_Medicare.Text       = "Ref.";
            lblEPCText_NoMedicareCard.Text = "Ref.";
            lblEPCText_DVA.Text            = "Ref.";
            lblEPCText_NoDVACard.Text      = "Ref.";
            lblEPCText_Insurance.Text            = "Ref.";
            lblEPCText_NoInsuranceCard.Text      = "Ref.";
            lblEPCText_InfoTitle.Text      = "Ref.";


            // just hide while implementing INS .. then delete this section

            bool hideInsurance = false;
            if (hideInsurance)
            {
                insurance_info_row.Visible           = false;
                insurance_short_heading_row.Visible  = false;
                insurance_epc_info_row.Visible       = false;
                insurance_no_epc_message_row.Visible = false;
            }
        }
    }


    protected int PatientID
    {
        get { return healthCardPatientID.Value == "" ? -1 : Convert.ToInt32(healthCardPatientID.Value); }
        set { healthCardPatientID.Value = value.ToString(); }
    }
    protected bool ShowOnlyActiveCard
    {
        get { return healthCardShowOnlyActiveCard.Value == "" ? false : Convert.ToBoolean(healthCardShowOnlyActiveCard.Value); }
        set { healthCardShowOnlyActiveCard.Value = value.ToString(); lblInsuranceOrgName.Visible = value; }
    }
    public void SetInfo(int patientID, bool showOnlyActiveCard, bool showLeftColumn, bool showCardInfoRow, bool showCardShortHeadingRow, bool showNoEpcMessageRow, bool showAddEditEpcLinks, bool showSpaceBeforeInsuranceExpiry)
    {
        this.PatientID = patientID;
        this.ShowOnlyActiveCard = showOnlyActiveCard;
        ShowLeftColumn  = showLeftColumn;
        ShowCardInfoRow = showCardInfoRow;
        ShowCardShortHeadingRow = showCardShortHeadingRow;
        ShowNoEpcMessageRow = showNoEpcMessageRow;
        ShowAddEditEpcLinks = showAddEditEpcLinks;
        ShowSpaceBeforeInsuranceExpiry = showSpaceBeforeInsuranceExpiry;

        UpdateInfo();
    }
    public bool ShowLeftColumn
    {
        set
        {
            healthCardShowLeftColumn.Value = value.ToString();

            r1c1.Visible = value;
            r1c2.Visible = value;
            r2c1.Visible = value;
            r2c2.Visible = value;
            r3c1.Visible = value;
            r3c2.Visible = value;
            r4c1.Visible = value;
            r4c2.Visible = value;
            r5c1.Visible = value;
            r5c2.Visible = value;
            r6c1.Visible = value;
            r6c2.Visible = value;
            r7c1.Visible = value;
            r7c2.Visible = value;
            r8c1.Visible = value;
            r8c2.Visible = value;
            r9c1.Visible = value;
            r9c2.Visible = value;
            r10c1.Visible = value;
            r10c2.Visible = value;
            r12c1.Visible = value;
            r12c2.Visible = value;
            r13c1.Visible = value;
            r13c2.Visible = value;
            r14c1.Visible = value;
            r14c2.Visible = value;
            r15c1.Visible = value;
            r15c2.Visible = value;
            r16c1.Visible = value;
            r16c2.Visible = value;
            
            r11c1.Visible = value;
            r11c2.Visible = value;
        }
        get
        {
            return healthCardShowLeftColumn.Value == "" ? false : Convert.ToBoolean(healthCardShowLeftColumn.Value);
        }
    }
    public bool ShowCardInfoRow
    {
        set
        {
            healthCardShowCardInfoRow.Value = value.ToString();

            medicare_info_row.Visible   = value;
            dva_info_row.Visible        = value;
            insurance_info_row.Visible        = value;
        }
        get
        {
            return healthCardShowCardInfoRow.Value == "" ? false : Convert.ToBoolean(healthCardShowCardInfoRow.Value);
        }
    }
    public bool ShowCardShortHeadingRow
    {
        set
        {
            healthCardShowCardShortHeadingRow.Value = value.ToString();

            medicare_short_heading_row.Visible = value;
            dva_short_heading_row.Visible = value;
            insurance_short_heading_row.Visible = value;
        }
        get
        {
            return healthCardShowCardShortHeadingRow.Value == "" ? false : Convert.ToBoolean(healthCardShowCardShortHeadingRow.Value);
        }
    }
    public bool ShowNoEpcMessageRow
    {
        set
        {
            healthCardShowNoEpcMessageRow.Value = value.ToString();
        }
        get
        {
            return healthCardShowNoEpcMessageRow.Value == "" ? false : Convert.ToBoolean(healthCardShowNoEpcMessageRow.Value);
        }
    }
    public bool ShowAddEditEpcLinks
    {
        set
        {
            healthCardShowAddEditEpcLinks.Value = value.ToString();

            spnShowAddEditEpcLinksMedicare.Visible = value;
            spnShowAddEditEpcLinksDVA.Visible      = value;
            spnShowAddEditEpcLinksInsurance.Visible      = value;
            spnShowAddEpcLinksMedicare.Visible     = value;
            spnShowAddEpcLinksDVA.Visible          = value;
            spnShowAddEpcLinksInsurance.Visible          = value;
        }
        get
        {
            return healthCardShowAddEditEpcLinks.Value == "" ? false : Convert.ToBoolean(healthCardShowAddEditEpcLinks.Value);
        }
    }
    public bool ShowSpaceBeforeInsuranceExpiry
    {
        get
        {
            return true;
            return insurance_space1_before_exp.Visible && insurance_space2_before_exp.Visible;
        }
        set
        {
            insurance_space1_before_exp.Visible = value;
            insurance_space2_before_exp.Visible = value;
        }
    }

    protected void UpdateInfo()
    {
        if (this.PatientID == -1)
            return;

        HealthCard[] healthcards = HealthCardDB.GetAllByPatientID(this.PatientID, this.ShowOnlyActiveCard);
        HealthCard medicareCard = null;
        HealthCard dvaCard      = null;
        HealthCard insuranceCard      = null;
        for (int i = 0; i < healthcards.Length; i++)
        {
            if (healthcards[i].Organisation.OrganisationID == -1)
                medicareCard = healthcards[i];
            else if (healthcards[i].Organisation.OrganisationID == -2)
                dvaCard = healthcards[i];
            else
            {
                healthcards[i].Organisation = OrganisationDB.GetByID(healthcards[i].Organisation.OrganisationID);
                if (healthcards[i].Organisation.OrganisationType.OrganisationTypeGroup.ID == 7)
                    insuranceCard = healthcards[i];
            }
        }

        if (this.ShowOnlyActiveCard)
            HideInactiveCards(medicareCard == null || !medicareCard.IsActive, dvaCard == null || !dvaCard.IsActive, insuranceCard == null || !insuranceCard.IsActive);

        no_epc_message_row.Visible = this.ShowNoEpcMessageRow && medicareCard == null && dvaCard == null && insuranceCard == null;

        UpdateCard(medicareCard, CardType.Medicare);
        UpdateCard(dvaCard,      CardType.DVA);
        UpdateCard(insuranceCard,      CardType.Insurance);
    }
    protected void HideInactiveCards(bool hideMedicare, bool hideDVA, bool hideInsurance)
    {
        if (hideMedicare || hideDVA)
            space_row.Visible = false;

        if (hideDVA || hideInsurance)
            space_row2.Visible = false;

        if (hideMedicare)
        {
            medicare_info_row.Visible                   = false;
            medicare_short_heading_row.Visible          = false;
            medicare_epc_info_row.Visible               = false;
            medicare_epc_combined_remaining_row.Visible = false;
            medicare_no_epc_message_row.Visible         = false;
        }
        if (hideDVA)
        {
            dva_info_row.Visible           = false;
            dva_short_heading_row.Visible  = false;
            dva_epc_info_row.Visible       = false;
            dva_no_epc_message_row.Visible = false;
        }
        if (hideInsurance)
        {
            insurance_info_row.Visible           = false;
            insurance_short_heading_row.Visible  = false;
            insurance_epc_info_row.Visible       = false;
            insurance_no_epc_message_row.Visible = false;
        }
    }


    protected enum CardType { Medicare = -1, DVA = -2, Insurance = -3 };
    protected void UpdateCard(HealthCard card, CardType cardType)  // can be null if no card and will set info for no card of that org
    {
        bool isGPsite = Session != null && Convert.ToBoolean(Session["SiteIsGP"]);

        if (card != null && card.Organisation.OrganisationID != -1 && card.Organisation.OrganisationID != -2 && card.Organisation.OrganisationType.OrganisationTypeGroup.ID != 7)
            throw new Exception("Unknown health card type");

        Label lblInfo = lblMedicareCard;
        if (cardType == CardType.Medicare)  lblInfo = lblMedicareCard;
        if (cardType == CardType.DVA)       lblInfo = lblDVACard;
        if (cardType == CardType.Insurance) lblInfo = lblInsuranceCard;


        lblInfo.Text = card == null ? "No card" : (card.CardNbr + (card.CardFamilyMemberNbr.Length > 0 ? " - " + card.CardFamilyMemberNbr : ""));

        if (card != null && card.Organisation != null && card.Organisation.OrganisationType != null && card.Organisation.OrganisationType.OrganisationTypeGroup.ID == 7)
            lblInfo.Text = "[" + card.Organisation.Name + "] &nbsp;" + lblInfo.Text;


        bool cardHasEPC = card != null && card.HasEPC();

        (cardType == CardType.Medicare ? lblMedicareCardInfo         : (cardType == CardType.DVA ? lblDVACardInfo         : lblInsuranceCardInfo)).Visible         = card != null;
        (cardType == CardType.Medicare ? lnkMedicareCardEdit         : (cardType == CardType.DVA ? lnkDVACardEdit         : lnkInsuranceCardEdit)).Visible         = card != null;
        (cardType == CardType.Medicare ? lnkMedicareCardEdit         : (cardType == CardType.DVA ? lnkDVACardEdit         : lnkInsuranceCardEdit)).CommandArgument = card != null ? card.HealthCardID.ToString() : "-1";
        (cardType == CardType.Medicare ? lblMedicareAddBtnSeperator  : (cardType == CardType.DVA ? lblDVAAddBtnSeperator  : lblInsuranceAddBtnSeperator)).Visible  = card != null;
        (cardType == CardType.Medicare ? lnkMedicareCardAdd          : (cardType == CardType.DVA ? lnkDVACardAdd          : lnkInsuranceCardAdd)).Text             = card == null ? "Add Card" : "Replace Card";
        (cardType == CardType.Medicare ? lnkMedicareCardAdd          : (cardType == CardType.DVA ? lnkDVACardAdd          : lnkInsuranceCardAdd)).OnClientClick    = card == null ? null : "javascript:if (!confirm('Setting a new card will delete the current card and any associated referral'. Continue?')) return false;";

        (cardType == CardType.Medicare ? medicare_epc_info_row       : (cardType == CardType.DVA ? dva_epc_info_row       : insurance_epc_info_row)).Visible       = !isGPsite && card != null &&  cardHasEPC;
        (cardType == CardType.Medicare ? medicare_no_epc_message_row : (cardType == CardType.DVA ? dva_no_epc_message_row : insurance_no_epc_message_row)).Visible = !isGPsite && card != null && !cardHasEPC;
        if (cardType == CardType.Medicare)
            medicare_epc_combined_remaining_row.Visible = !isGPsite && card != null && cardHasEPC;


        if (card != null)
        {


            (cardType == CardType.Medicare ? chkMedicareIsActive  : (cardType == CardType.DVA ? chkDvaIsActive  : chkInsuranceIsActive )).Checked = card.IsActive;
            (cardType == CardType.Medicare ? chkMedicareIsActive2 : (cardType == CardType.DVA ? chkDvaIsActive2 : chkInsuranceIsActive2)).Checked = card.IsActive;
            //if (card.IsActive)
            //{
            //    (cardType == CardType.Medicare ? chkMedicareIsActive  : (cardType == CardType.DVA ? chkDvaIsActive  : chkInsuranceIsActive )).Attributes["onclick"]  = "return false;";
            //    (cardType == CardType.Medicare ? chkMedicareIsActive2 : (cardType == CardType.DVA ? chkDvaIsActive2 : chkInsuranceIsActive2)).Attributes["onclick"] = "return false;";
            //}
            //else
            //{
            //    (cardType == CardType.Medicare ? chkMedicareIsActive  : (cardType == CardType.DVA ? chkDvaIsActive  : chkInsuranceIsActive )).Attributes.Remove("onclick");
            //    (cardType == CardType.Medicare ? chkMedicareIsActive2 : (cardType == CardType.DVA ? chkDvaIsActive2 : chkInsuranceIsActive2)).Attributes.Remove("onclick");
            //}
            (cardType == CardType.Medicare ? chkMedicareIsActive  : (cardType == CardType.DVA ? chkDvaIsActive  : chkInsuranceIsActive )).Attributes["CommandArgument"] = string.Format("{0}", card.HealthCardID);
            (cardType == CardType.Medicare ? chkMedicareIsActive2 : (cardType == CardType.DVA ? chkDvaIsActive2 : chkInsuranceIsActive2)).Attributes["CommandArgument"] = string.Format("{0}", card.HealthCardID);
            
            // marcus said that can deactivate so that have all cards de-activated
            // insurance was not added to this yet
            /*
            if (card.IsActive)
            {
                (cardType == CardType.Medicare ? chkMedicareIsActive : chkDvaIsActive).Attributes.Add("onclick", "return false;");
                (cardType == CardType.Medicare ? chkMedicareIsActive2 : chkDvaIsActive2).Attributes.Add("onclick", "return false;");
            }
            else
            {
                (cardType == CardType.Medicare ? chkMedicareIsActive : chkDvaIsActive).Attributes.Remove("onclick");
                (cardType == CardType.Medicare ? chkMedicareIsActive2 : chkDvaIsActive2).Attributes.Remove("onclick");
            }
            */


            //
            // set add epc link
            //

            string allFeatures = "dialogWidth:550px;dialogHeight:550px;center:yes;resizable:no; scroll:no";
            string js = "show_modal_updade_epc(" + card.HealthCardID.ToString() + ");" + (!Utilities.IsMobileDevice(Request) ? "window.location=window.location;" : "") + "return false;";

            (cardType == CardType.Medicare ? lnkNewMedicareEPC2 : (cardType == CardType.DVA ? lnkNewDVAEPC2 : lnkNewInsuranceEPC2)).Visible = true;
            (cardType == CardType.Medicare ? lnkNewMedicareEPC2 : (cardType == CardType.DVA ? lnkNewDVAEPC2 : lnkNewInsuranceEPC2)).NavigateUrl = "  ";
            (cardType == CardType.Medicare ? lnkNewMedicareEPC2 : (cardType == CardType.DVA ? lnkNewDVAEPC2 : lnkNewInsuranceEPC2)).Text = "Add Referral";
            (cardType == CardType.Medicare ? lnkNewMedicareEPC2 : (cardType == CardType.DVA ? lnkNewDVAEPC2 : lnkNewInsuranceEPC2)).Attributes.Add("onclick", js);


            if (cardType == CardType.Insurance)
            {
                lblInsuranceOrgName.Text = card.Organisation.Name + "&nbsp;&nbsp;";

                lblInsuranceExpireDate.Text = card.ExpiryDate == DateTime.MinValue ? "Not Entered" : card.ExpiryDate.ToString("d MMM yyyy");

                if (card.ExpiryDate != DateTime.MinValue && DateTime.Now.Date >= card.ExpiryDate)
                {
                    lblInsuranceExpireDate.ForeColor = System.Drawing.Color.Red;
                    lblInsuranceExpireDateText.Text = lblEPCExpireDateText.Text.Replace("Expires:", "*** Expired ***");
                    lblInsuranceExpireDateText.Font.Bold = true;
                    lblInsuranceExpireDateText.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblInsuranceExpireDateText.Font.Bold = false;
                }
            }


            if (!isGPsite && cardHasEPC)
            {
                string refDate = card.DateReferralSigned == DateTime.MinValue ? "date not set" : card.DateReferralSigned.ToString("d MMM yyyy");
                string expDate = card.DateReferralSigned == DateTime.MinValue ? "date not set" : card.DateReferralSigned.AddYears(1).AddDays(-1).ToString("d MMM yyyy");
                (cardType == CardType.Medicare ? lblEPCSignedDate : (cardType == CardType.DVA ? lblDVAEPCSignedDate : lblInsuranceEPCSignedDate)).Text = refDate;
                (cardType == CardType.Medicare ? lblEPCExpireDate : (cardType == CardType.DVA ? lblDVAEPCExpireDate : lblInsuranceEPCExpireDate)).Text = expDate;



                //
                // set add/edit epc links
                //

                (cardType == CardType.Medicare ? lnkNewMedicareEPC : (cardType == CardType.DVA ? lnkNewDVAEPC : lnkNewInsuranceEPC)).Visible = true;
                (cardType == CardType.Medicare ? lnkNewMedicareEPC : (cardType == CardType.DVA ? lnkNewDVAEPC : lnkNewInsuranceEPC)).NavigateUrl = "  ";
                (cardType == CardType.Medicare ? lnkNewMedicareEPC : (cardType == CardType.DVA ? lnkNewDVAEPC : lnkNewInsuranceEPC)).Text = "Replace Referral";
                (cardType == CardType.Medicare ? lnkNewMedicareEPC : (cardType == CardType.DVA ? lnkNewDVAEPC : lnkNewInsuranceEPC)).Attributes.Add("onclick", js);


                if (!Utilities.IsMobileDevice(Request))
                {
                    allFeatures = "dialogWidth:550px;dialogHeight:520px;center:yes;resizable:no; scroll:no";
                    js = "javascript:window.showModalDialog('EPCDetailV2.aspx?type=edit&id=" + card.HealthCardID.ToString() + "', '', '" + allFeatures + "');window.location=window.location;return false;";
                }
                else
                {
                    js = "open_new_tab('EPCDetailV2.aspx?type=edit&id=" + card.HealthCardID.ToString() + "');return false;";
                }

                (cardType == CardType.Medicare ? lnkEditMedicareEPC : (cardType == CardType.DVA ? lnkEditDVAEPC : lnkEditInsuranceEPC)).Visible = true;
                (cardType == CardType.Medicare ? lnkEditMedicareEPC : (cardType == CardType.DVA ? lnkEditDVAEPC : lnkEditInsuranceEPC)).NavigateUrl = "  ";
                (cardType == CardType.Medicare ? lnkEditMedicareEPC : (cardType == CardType.DVA ? lnkEditDVAEPC : lnkEditInsuranceEPC)).Text = "Edit Referral";
                (cardType == CardType.Medicare ? lnkEditMedicareEPC : (cardType == CardType.DVA ? lnkEditDVAEPC : lnkEditInsuranceEPC)).Attributes.Add("onclick", js);



                if (card.DateReferralSigned != DateTime.MinValue && DateTime.Now.Date >= card.DateReferralSigned.AddYears(1).Date)
                {
                    (cardType == CardType.Medicare ? lblEPCExpireDate     : (cardType == CardType.DVA ? lblDVAEPCExpireDate     : lblInsuranceEPCExpireDate)).ForeColor = System.Drawing.Color.Red;
                    (cardType == CardType.Medicare ? lblEPCExpireDateText : (cardType == CardType.DVA ? lblDVAEPCExpireDateText : lblInsuranceEPCExpireDateText)).Text = lblEPCExpireDateText.Text.Replace("Expires:", "*** Expired ***");
                    (cardType == CardType.Medicare ? lblEPCExpireDateText : (cardType == CardType.DVA ? lblDVAEPCExpireDateText : lblInsuranceEPCExpireDateText)).Font.Bold = true;
                    (cardType == CardType.Medicare ? lblEPCExpireDateText : (cardType == CardType.DVA ? lblDVAEPCExpireDateText : lblInsuranceEPCExpireDateText)).ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    (cardType == CardType.Medicare ? lblEPCExpireDateText : (cardType == CardType.DVA ? lblDVAEPCExpireDateText : lblInsuranceEPCExpireDateText)).Font.Bold = false;
                }



                if (cardType == CardType.Medicare)
                {
                    string lblEPCsRemainingTable = @"
                    <table border=""0"" cellspacing=""0"" cellpadding=""0"">";

                    int totalRemainingAllFields = 0;
                    HealthCardEPCRemaining[] epcsRemaining = HealthCardEPCRemainingDB.GetByHealthCardID(card.HealthCardID, -1);
                    if (epcsRemaining.Length == 0)
                    {
                        lblEPCsRemainingTable += @"
                        <tr>
                            <td width=""18""></td>
                            <td>No service types added.</td>
                        </tr>";
                    }
                    else
                    {
                        for (int i = 0; i < epcsRemaining.Length; i++)
                        {
                            string colorStartTag = epcsRemaining[i].NumServicesRemaining > 0 ? "" : @"<font color=""red"">";
                            string colorEndTag = epcsRemaining[i].NumServicesRemaining > 0 ? "" : @"</font>";

                            totalRemainingAllFields += epcsRemaining[i].NumServicesRemaining;
                            lblEPCsRemainingTable += @"
                        <tr>
                            <td width=""18""></td>
                            <td>" + colorStartTag + epcsRemaining[i].Field.Descr + colorEndTag + @"</td>
                            <td width=""12""></td>
                            <td><b>" + colorStartTag + epcsRemaining[i].NumServicesRemaining + colorEndTag + @"</b></td>
                        </tr>";
                        }
                    }
                    lblEPCsRemainingTable += @"
                    </table>";

                    lblEPCsRemaining.Text = lblEPCsRemainingTable;

                    EPCInfo epcInfo = EPCInfoDB.GetEPCInfo(card.Patient.PatientID);
                    lblCombinedEPCRemainingThisYear.Text = (totalRemainingAllFields < epcInfo.RamainingThisYear) ? totalRemainingAllFields.ToString() : epcInfo.RamainingThisYear.ToString();
                    lblCombinedEPCUsedThisYear.Text = epcInfo.NbrMedicareServicesUsedSoFarThisYear.ToString();
                    lblCombinedEPCRemainingNextYear.Text = (totalRemainingAllFields < epcInfo.RamainingThisYear) ? "0" : epcInfo.RemainingNextYear.ToString();
                    if (Convert.ToInt32(lblCombinedEPCRemainingThisYear.Text) <= 0)
                    {
                        lblCombinedEPCRemainingThisYear.ForeColor = System.Drawing.Color.Red;
                        lblCombinedEPCRemainingThisYearText.Font.Bold = true;
                        lblCombinedEPCRemainingThisYearText.ForeColor = System.Drawing.Color.Red;
                    }
                    if (Convert.ToInt32(lblCombinedEPCRemainingNextYear.Text) <= 0)
                        spn_combined_epc_remaining_next_year.Visible = false;

                    bool expired = (card.DateReferralSigned != DateTime.MinValue && DateTime.Now.Date >= card.DateReferralSigned.AddYears(1).Date);
                    if (expired)
                    {
                        lblCombinedEPCRemainingThisYear.Text = "0";
                        lblCombinedEPCRemainingNextYear.Text = "0";
                    }
                }
            }


        }
    }

    #region Events

    protected void chkIsActive_CheckedChanged(object sender, EventArgs e)
    {
        int healthCardID = Convert.ToInt32(((CheckBox)sender).Attributes["CommandArgument"]);

        if (((CheckBox)sender).Checked)
        {
            HealthCardDB.UpdateAllCardsInactive(this.PatientID, healthCardID);
            HealthCardDB.UpdateIsActive(healthCardID, true);
        }
        else
        {
            HealthCardDB.UpdateIsActive(healthCardID, false);
        }

        UpdateInfo();
    }

    protected void btnAddEditMedicareCard_Click(object sender, EventArgs e)
    {
        btnAddEditCard(CardType.Medicare);
    }
    protected void btnAddEditDVACard_Click(object sender, EventArgs e)
    {
        btnAddEditCard(CardType.DVA);
    }
    protected void btnAddEditInsuranceCard_Click(object sender, EventArgs e)
    {
        btnAddEditCard(CardType.Insurance);
    }
    protected void btnAddEditCard(CardType cardType)
    {
        HealthCard[] hcCards = HealthCardDB.GetAllByPatientID(this.PatientID, false, cardType == CardType.Medicare ? -1 : - 2);
        string urlFieldsTypeAndID = (hcCards.Length == 0) ? "?type=add&id="+this.PatientID : "?type=view&id="+hcCards[0].HealthCardID;
        Response.Redirect("~/HealthCardDetailV2.aspx" + urlFieldsTypeAndID + "&card=" + (cardType == CardType.Medicare ? "medicare" : (cardType == CardType.DVA ? "dva" : "ins")));
    }


    protected void btnEditMedicareCard_Click(object sender, EventArgs e)
    {
        RedirectAddEditCard(true, CardType.Medicare, Convert.ToInt32(((LinkButton)sender).CommandArgument));
    }
    protected void btnAddMedicareCard_Click(object sender, EventArgs e)
    {
        RedirectAddEditCard(false, CardType.Medicare);
    }
    protected void btnEditDVACard_Click(object sender, EventArgs e)
    {
        RedirectAddEditCard(true, CardType.DVA, Convert.ToInt32(((LinkButton)sender).CommandArgument));
    }
    protected void btnAddDVACard_Click(object sender, EventArgs e)
    {
        RedirectAddEditCard(false, CardType.DVA);
    }
    protected void btnEditInsuranceCard_Click(object sender, EventArgs e)
    {
        RedirectAddEditCard(true, CardType.Insurance, Convert.ToInt32(((LinkButton)sender).CommandArgument));
    }
    protected void btnAddInsuranceCard_Click(object sender, EventArgs e)
    {
        RedirectAddEditCard(false, CardType.Insurance);
    }
    protected void RedirectAddEditCard(bool isEdit, CardType cardType, int healthcard_id = -1)
    {
        string urlFieldsTypeAndID = !isEdit ? "?type=add&id=" + this.PatientID : "?type=view&id=" + healthcard_id;
        Response.Redirect("~/HealthCardDetailV2.aspx" + urlFieldsTypeAndID + "&card=" + (cardType == CardType.Medicare ? "medicare" : (cardType == CardType.DVA ? "dva" : "ins")));
    }

    #endregion

}