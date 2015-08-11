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
using System.IO;

public partial class SettingsV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {

        try
        {
            HideErrorMessage();
            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, false, false, false, false);
                SetupGUI();
                ResetValues();
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

    protected void SetupGUI()
    {
        UserView userView = UserView.GetInstance();

        if (!userView.IsStakeholder)
        {
            tr_MedicareMaxNbrServicesPerYear.Attributes["class"]             = "hiddencol";
            tr_AutoMedicareClaiming.Attributes["class"]                      = "hiddencol";
            tr_MedicareEclaimsLicenseNbr.Attributes["class"]                 = "hiddencol";
            tr_MaxNbrProviders.Attributes["class"]                           = "hiddencol";
            tr_SMSPrice.Attributes["class"]                                  = "hiddencol";
            tr_StockWarningNotificationEmailAddress.Attributes["class"]      = "hiddencol";
            tr_AllowAddSiteClinic.Attributes["class"]                        = "hiddencol";
            tr_AllowAddSiteAgedCare.Attributes["class"]                      = "hiddencol";
            tr_AllowAddSiteGP.Attributes["class"]                            = "hiddencol";
            tr_DayOfMonthPaymentDue.Attributes["class"]                      = "hiddencol";
            tr_UseCallCenter.Attributes["class"]                             = "hiddencol";
            tr_CallCenterPrefix.Attributes["class"]                          = "hiddencol";
            tr_AutoSendFaxesAsEmailsIfNoEmailExistsToGPs.Attributes["class"] = "hiddencol";

            tr_CC_Nbr.Attributes["class"]                                    = "hiddencol";
            tr_CC_Exp.Attributes["class"]                                    = "hiddencol";
            tr_CC_CCV.Attributes["class"]                                    = "hiddencol";
            tr_RateIncomingCall.Attributes["class"]                          = "hiddencol";
            tr_RateOutgoingCall.Attributes["class"]                          = "hiddencol";
            tr_RateCreditCardAmt.Attributes["class"]                         = "hiddencol";
            tr_RateCreditCardPct.Attributes["class"]                         = "hiddencol";
            tr_RateDebitCardAmt.Attributes["class"]                          = "hiddencol";
            tr_RateDebitCardPct.Attributes["class"]                          = "hiddencol";
            tr_EziDebit_Enabled.Attributes["class"]                          = "hiddencol";
            tr_EziDebit_DigitalKey.Attributes["class"]                       = "hiddencol";
            tr_EziDebit_FormDate.Attributes["class"]                         = "hiddencol";

            tr_SpaceTrailingStakeholderFields.Attributes["class"]            = "hiddencol";
            tr_SpaceBetweenStakeholderFields1.Attributes["class"]            = "hiddencol";
            tr_SpaceBetweenStakeholderFields2.Attributes["class"]            = "hiddencol";
            tr_SpaceBetweenStakeholderFields3.Attributes["class"]            = "hiddencol";
            tr_SpaceBetweenStakeholderFields4.Attributes["class"]            = "hiddencol";
        }

        for (int i = 1; i <= 31; i++)
            ddlDayOfMonthPaymentDue.Items.Add(new ListItem(i.ToString(), i.ToString()));

        for (int i = 1; i <= 30; i++)
            ddlNbrDaysAheadToSendDailyBookingReminderSMS.Items.Add(new ListItem(i.ToString(), i.ToString()));

        ddlEziDebit_FormDate_Day.Items.Add(new ListItem("--", "-1"));
        ddlEziDebit_FormDate_Month.Items.Add(new ListItem("--", "-1"));
        ddlEziDebit_FormDate_Year.Items.Add(new ListItem("--", "-1"));

        for (int i = 1; i <= 31; i++)
            ddlEziDebit_FormDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1; i <= 12; i++)
            ddlEziDebit_FormDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 2014; i <= 2020; i++)
            ddlEziDebit_FormDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

        /*
        if (!Utilities.IsDev())
        {
            bk0.Attributes["class"] = "hiddencol";
            bk1.Attributes["class"] = "hiddencol";
            bk2.Attributes["class"] = "hiddencol";
            bk3.Attributes["class"] = "hiddencol";
            bk4.Attributes["class"] = "hiddencol";
            bk5.Attributes["class"] = "hiddencol";
            bk6.Attributes["class"] = "hiddencol";
            bk7.Attributes["class"] = "hiddencol";
            bk8.Attributes["class"] = "hiddencol";
            bk9.Attributes["class"] = "hiddencol";
            bk10.Attributes["class"] = "hiddencol";
            bk11.Attributes["class"] = "hiddencol";
            bk12.Attributes["class"] = "hiddencol";
            bk13.Attributes["class"] = "hiddencol";
            bk14.Attributes["class"] = "hiddencol";
            bk15.Attributes["class"] = "hiddencol";
            bk16.Attributes["class"] = "hiddencol";
            bk17.Attributes["class"] = "hiddencol";
            bk18.Attributes["class"] = "hiddencol";
            bk19.Attributes["class"] = "hiddencol";
            bk20.Attributes["class"] = "hiddencol";
            bk21.Attributes["class"] = "hiddencol";
            bk22.Attributes["class"] = "hiddencol";
            bk23.Attributes["class"] = "hiddencol";
            bk24.Attributes["class"] = "hiddencol";
            bk25.Attributes["class"] = "hiddencol";
            bk26.Attributes["class"] = "hiddencol";
            bk27.Attributes["class"] = "hiddencol";
            bk28.Attributes["class"] = "hiddencol";
            bk29.Attributes["class"] = "hiddencol";
        }
        */

    }

    protected void ResetValues()
    {
        SystemVariables sysVariables = SystemVariableDB.GetAll();

        ddlDefaultState.Items.Clear();
        DataTable states = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Suburb", "", "state", "distinct state");
        foreach (DataRow row in states.Rows)
            ddlDefaultState.Items.Add(new ListItem(row["state"].ToString(), row["state"].ToString()));


        txtMedicareMaxNbrServicesPerYear.Text                               = sysVariables["MedicareMaxNbrServicesPerYear"].Value;
        txtMedicareEclaimsLicenseNbr.Text                                   = sysVariables["MedicareEclaimsLicenseNbr"].Value;
        chkAutoMedicareClaiming.Checked                                     = Convert.ToInt32(sysVariables["AutoMedicareClaiming"].Value) == 1;
        txtMaxNbrProviders.Text                                             = Convert.ToInt32(sysVariables["MaxNbrProviders"].Value).ToString();
        txtSMSPrice.Text                                                    = Convert.ToDouble(sysVariables["SMSPrice"].Value).ToString("0.00");
        txtStockWarningNotificationEmailAddress.Text                        = sysVariables["StockWarningNotificationEmailAddress"].Value;
        chkAllowAddSiteClinic.Checked                                       = Convert.ToInt32(sysVariables["AllowAddSiteClinic"].Value) == 1;
        chkAllowAddSiteAgedCare.Checked                                     = Convert.ToInt32(sysVariables["AllowAddSiteAgedCare"].Value) == 1;
        chkAllowAddSiteGP.Checked                                           = Convert.ToInt32(sysVariables["AllowAddSiteGP"].Value) == 1;
        ddlDayOfMonthPaymentDue.SelectedValue                               = sysVariables["PaymentDueDayOfMonth"].Value;
        chkUseCallCenter.Checked                                            = Convert.ToInt32(sysVariables["UseMediclinicCallCenter"].Value) == 1;
        txtCallCenterPrefix.Text                                            = sysVariables["CallCenterPrefix"].Value;
        chkAutoSendFaxesAsEmailsIfNoEmailExistsToGPs.Checked                = Convert.ToInt32(sysVariables["AutoSendFaxesAsEmailsIfNoEmailExistsToGPs"].Value) == 1;

        txtCC_Nbr.Text                                                      = sysVariables["CC_Nbr"].Value;
        txtCC_Exp_Mo.Text                                                   = sysVariables["CC_Exp_Mo"].Value;
        txtCC_Exp_Yr.Text                                                   = sysVariables["CC_Exp_Yr"].Value;
        txtCC_CCV.Text                                                      = sysVariables["CC_CCV"].Value;
        txtRateIncomingCall.Text                                            = Convert.ToDouble(sysVariables["Rate_IncomingCall"].Value).ToString("0.00");
        txtRateOutgoingCall.Text                                            = Convert.ToDouble(sysVariables["Rate_OutgoingCall"].Value).ToString("0.00");
        txtRateCreditCardAmt.Text                                           = Convert.ToDouble(sysVariables["Rate_CreditCardAmt"].Value).ToString("0.00");
        txtRateCreditCardPct.Text                                           = Convert.ToDouble(sysVariables["Rate_CreditCardPct"].Value).ToString("0.00");
        txtRateDebitCardAmt.Text                                            = Convert.ToDouble(sysVariables["Rate_DebitCardAmt"].Value).ToString("0.00");
        txtRateDebitCardPct.Text                                            = Convert.ToDouble(sysVariables["Rate_DebitCardPct"].Value).ToString("0.00");
        chkEziDebit_Enabled.Checked                                         = Convert.ToInt32(sysVariables["EziDebit_Enabled"].Value) == 1;
        txtEziDebit_DigitalKey.Text                                         = sysVariables["EziDebit_DigitalKey"].Value;
        DateTime EziDebit_FormDate = Utilities.GetDate(sysVariables["EziDebit_FormDate"].Value, "yyyy-mm-dd");
        ddlEziDebit_FormDate_Day.SelectedValue                              = EziDebit_FormDate == DateTime.MinValue ? "-1" : EziDebit_FormDate.Day.ToString();
        ddlEziDebit_FormDate_Month.SelectedValue                            = EziDebit_FormDate == DateTime.MinValue ? "-1" : EziDebit_FormDate.Month.ToString();
        ddlEziDebit_FormDate_Year.SelectedValue                             = EziDebit_FormDate == DateTime.MinValue ? "-1" : EziDebit_FormDate.Year.ToString();


        txtSite.Text                                                        = sysVariables["Site"].Value;
        txtBannerMessage.Text                                               = sysVariables["BannerMessage"].Value;
        chkShowBannerMessage.Checked                                        = Convert.ToBoolean(sysVariables["ShowBannerMessage"].Value);
        txtEmail_FromName.Text                                              = sysVariables["Email_FromName"].Value;
        txtEmail_FromEmail.Text                                             = sysVariables["Email_FromEmail"].Value;
        txtAdminAlertEmail_To.Text                                          = sysVariables["AdminAlertEmail_To"].Value;
        chkEnablePatientLogins.Checked                                      = Convert.ToInt32(sysVariables["AllowPatientLogins"].Value)                     == 1;
        chkBookings_ProvsCanSeeOtherProvs.Checked                           = Convert.ToInt32(sysVariables["Bookings_ProvsCanSeeOtherProvs"].Value)         == 1;
        chkBookings_ProvsCanSeePatientsOfAllOrgs.Checked                    = Convert.ToInt32(sysVariables["Bookings_ProvsCanSeePatientsOfAllOrgs"].Value)  == 1;
        chkProvsCanSeePricesWhenCompletingBks_AC.Checked                    = Convert.ToInt32(sysVariables["ProvsCanSeePricesWhenCompletingBks_AC"].Value)  == 1;
        chkEnableExistingPatientsCreateOwnLogins.Checked                    = Convert.ToInt32(sysVariables["AllowPatientsToCreateOwnLogin"].Value)          == 1;
        chkEnableNewPatientsCreateOwnLogins.Checked                         = Convert.ToInt32(sysVariables["AllowPatientsToCreateOwnRecords"].Value)        == 1;
        lnkPtDirectLinkToCreateOwnLogin.NavigateUrl                         = "~/BookingNextAvailableV2.aspx?id=" + Session["DB"].ToString().Substring(Session["DB"].ToString().Length-4);
        lnkPtDirectLinkToCreateOwnLogin.Text                                = Request.Url.Authority + "/BookingNextAvailableV2.aspx?id=" + Session["DB"].ToString().Substring(Session["DB"].ToString().Length-4);
        chkPTLogin_BookingTimeEditable.Checked                              = Convert.ToInt32(sysVariables["PTLogin_BookingTimeEditable"].Value)            == 1;
        chkPTAddSelfBookingAlert.Checked                                    = Convert.ToInt32(sysVariables["EnableAlert_BookingAddedByPT"].Value)           == 1;
        chkPTEditSelfBookingAlert.Checked                                   = Convert.ToInt32(sysVariables["EnableAlert_BookingEditedByPT"].Value)          == 1;
        chkPTDeleteSelfBookingAlert.Checked                                 = Convert.ToInt32(sysVariables["EnableAlert_BookingDeletedByPT"].Value)         == 1;
        chkEnableDeletedBookingsAlerts.Checked                              = Convert.ToInt32(sysVariables["EnableDeletedBookingsAlerts"].Value)            == 1;
        txtServiceSpecificBookingReminderLettersToBatch_EmailAddress.Text   = sysVariables["ServiceSpecificBookingReminderLettersToBatch_EmailAddress"].Value;
        ddlDefaultState.SelectedValue                                       = sysVariables["DefaultState"].Value;
        txtSMSCreditNotificationEmailAddress.Text                           = sysVariables["SMSCreditNotificationEmailAddress"].Value;
        chkSMSCreditOutOfBalance_SendEmail.Checked                          = Convert.ToInt32(sysVariables["SMSCreditOutOfBalance_SendEmail"].Value)        == 1;
        chkSMSCreditLowBalance_SendEmail.Checked                            = Convert.ToInt32(sysVariables["SMSCreditLowBalance_SendEmail"].Value)          == 1;
        txtSMSCreditLowBalance_Threshold.Text                               = Convert.ToDouble(sysVariables["SMSCreditLowBalance_Threshold"].Value).ToString("0.00");

        chkEnableLastEPCReminderSMS.Checked                                 = Convert.ToInt32(sysVariables["EnableLastEPCReminderSMS"].Value)               == 1;
        chkEnableLastEPCReminderEmails.Checked                              = Convert.ToInt32(sysVariables["EnableLastEPCReminderEmails"].Value)            == 1;
        txtNextAvailableDefaultNbrDaysShown.Text                            = Convert.ToInt32(sysVariables["NextAvailableDefaultNbrDaysShown"].Value).ToString();
        chkEnableAutoMontlyOverdueReminders.Checked                         = Convert.ToInt32(sysVariables["EnableAutoMontlyOverdueReminders"].Value)       == 1;
        chkEnableDailyBookingReminderSMS.Checked                            = Convert.ToInt32(sysVariables["EnableDailyBookingReminderSMS"].Value)          == 1;
        chkEnableDailyBookingReminderEmails.Checked                         = Convert.ToInt32(sysVariables["EnableDailyBookingReminderEmails"].Value)       == 1;
        ddlPT_Reminders_HasBothSMSandEmail.SelectedValue                    = sysVariables["PT_Reminders_HasBothSMSandEmail"].Value;
        ddlNbrDaysAheadToSendDailyBookingReminderSMS.SelectedValue          = sysVariables["NbrDaysAheadToSendDailyBookingReminderSMS"].Value;
        txtSendDailyBookingReminderText_SMS.Text                            = sysVariables["SendDailyBookingReminderText_SMS"].Value;
        FreeTextBox2.Text                                                   = sysVariables["SendDailyBookingReminderText_Email"].Value;
        txtSendDailyBookingReminderText_EmailSubect.Text                    = sysVariables["SendDailyBookingReminderText_EmailSubject"].Value;
        chkEnableDailyStaffBookingReminderSMS.Checked                       = Convert.ToInt32(sysVariables["EnableDailyStaffBookingsReminderSMS"].Value)    == 1;
        chkEnableDailyStaffBookingReminderEmails.Checked                    = Convert.ToInt32(sysVariables["EnableDailyStaffBookingsReminderEmails"].Value) == 1;
        ddlStaff_Reminders_HasBothSMSandEmail.SelectedValue                 = sysVariables["Staff_Reminders_HasBothSMSandEmail"].Value;
        chkEnableBirthdaySMS.Checked                                        = Convert.ToInt32(sysVariables["EnableBirthdaySMS"].Value)                      == 1;
        chkEnableBirthdayEmails.Checked                                     = Convert.ToInt32(sysVariables["EnableBirthdayEmails"].Value)                   == 1;
        chkInvoiceGapPayments.Checked                                       = Convert.ToInt32(sysVariables["InvoiceGapPayments"].Value) == 1;

        txtLettersEmailDeafultSubjectLine.Text                              = sysVariables["LettersEmailDefaultSubject"].Value;
        FreeTextBox1.Text                                                   = sysVariables["LettersEmailSignature"].Value;

        ColorPicker_Unavailable.Value                                       = sysVariables["BookingColour_Unavailable"].Value.Substring(1);
        ColorPicker_Available.Value                                         = sysVariables["BookingColour_Available"].Value.Substring(1);
        ColorPicker_UnavailableButAddable.Value                             = sysVariables["BookingColour_UnavailableButAddable"].Value.Substring(1);
        ColorPicker_UnavailableButUpdatable.Value                           = sysVariables["BookingColour_UnavailableButUpdatable"].Value.Substring(1);
        ColorPicker_Updatable.Value                                         = sysVariables["BookingColour_Updatable"].Value.Substring(1);
        ColorPicker_FullDayTaken.Value                                      = sysVariables["BookingColour_FullDayTaken"].Value.Substring(1);
        ColorPicker_CL_EPC_Past_Completed_Has_Invoice.Value                 = sysVariables["BookingColour_CL_EPC_Past_Completed_Has_Invoice"].Value.Substring(1);
        ColorPicker_CL_EPC_Past_Completed_No_Invoice.Value                  = sysVariables["BookingColour_CL_EPC_Past_Completed_No_Invoice"].Value.Substring(1);
        ColorPicker_CL_EPC_Future_Unconfirmed.Value                         = sysVariables["BookingColour_CL_EPC_Future_Unconfirmed"].Value.Substring(1);
        ColorPicker_CL_EPC_Future_Confirmed.Value                           = sysVariables["BookingColour_CL_EPC_Future_Confirmed"].Value.Substring(1);
        ColorPicker_AC_EPC_Past_Completed_Has_Invoice.Value                 = sysVariables["BookingColour_AC_EPC_Past_Completed_Has_Invoice"].Value.Substring(1);
        ColorPicker_AC_EPC_Past_Completed_No_Invoice.Value                  = sysVariables["BookingColour_AC_EPC_Past_Completed_No_Invoice"].Value.Substring(1);
        ColorPicker_AC_EPC_Future_Unconfirmed.Value                         = sysVariables["BookingColour_AC_EPC_Future_Unconfirmed"].Value.Substring(1);
        ColorPicker_AC_EPC_Future_Confirmed.Value                           = sysVariables["BookingColour_AC_EPC_Future_Confirmed"].Value.Substring(1);
        ColorPicker_CL_NonEPC_Past_Completed_Has_Invoice.Value              = sysVariables["BookingColour_CL_NonEPC_Past_Completed_Has_Invoice"].Value.Substring(1);
        ColorPicker_CL_NonEPC_Past_Completed_No_Invoice.Value               = sysVariables["BookingColour_CL_NonEPC_Past_Completed_No_Invoice"].Value.Substring(1);
        ColorPicker_CL_NonEPC_Future_Unconfirmed.Value                      = sysVariables["BookingColour_CL_NonEPC_Future_Unconfirmed"].Value.Substring(1);
        ColorPicker_CL_NonEPC_Future_Confirmed.Value                        = sysVariables["BookingColour_CL_NonEPC_Future_Confirmed"].Value.Substring(1);
        ColorPicker_AC_NonEPC_Past_Completed_Has_Invoice.Value              = sysVariables["BookingColour_AC_NonEPC_Past_Completed_Has_Invoice"].Value.Substring(1);
        ColorPicker_AC_NonEPC_Past_Completed_No_Invoice.Value               = sysVariables["BookingColour_AC_NonEPC_Past_Completed_No_Invoice"].Value.Substring(1);
        ColorPicker_AC_NonEPC_Future_Unconfirmed.Value                      = sysVariables["BookingColour_AC_NonEPC_Future_Unconfirmed"].Value.Substring(1);
        ColorPicker_AC_NonEPC_Future_Confirmed.Value                        = sysVariables["BookingColour_AC_NonEPC_Future_Confirmed"].Value.Substring(1);
        ColorPicker_Future_PatientLoggedIn.Value                            = sysVariables["BookingColour_Future_PatientLoggedIn"].Value.Substring(1);
        ColorPicker_Past_PatientLoggedIn.Value                              = sysVariables["BookingColour_Past_PatientLoggedIn"].Value.Substring(1);

        lblSysColor_Unavailable.Text                                        = ConfigurationManager.AppSettings["BookingColour_Unavailable"].Substring(1);
        lblSysColor_Available.Text                                          = ConfigurationManager.AppSettings["BookingColour_Available"].Substring(1);
        lblSysColor_UnavailableButAddable.Text                              = ConfigurationManager.AppSettings["BookingColour_UnavailableButAddable"].Substring(1);
        lblSysColor_UnavailableButUpdatable.Text                            = ConfigurationManager.AppSettings["BookingColour_UnavailableButUpdatable"].Substring(1);
        lblSysColor_Updatable.Text                                          = ConfigurationManager.AppSettings["BookingColour_Updatable"].Substring(1);
        lblSysColor_FullDayTaken.Text                                       = ConfigurationManager.AppSettings["BookingColour_FullDayTaken"].Substring(1);
        lblSysColor_CL_EPC_Past_Completed_Has_Invoice.Text                  = ConfigurationManager.AppSettings["BookingColour_CL_EPC_Past_Completed_Has_Invoice"].Substring(1);
        lblSysColor_CL_EPC_Past_Completed_No_Invoice.Text                   = ConfigurationManager.AppSettings["BookingColour_CL_EPC_Past_Completed_No_Invoice"].Substring(1);
        lblSysColor_CL_EPC_Future_Unconfirmed.Text                          = ConfigurationManager.AppSettings["BookingColour_CL_EPC_Future_Unconfirmed"].Substring(1);
        lblSysColor_CL_EPC_Future_Confirmed.Text                            = ConfigurationManager.AppSettings["BookingColour_CL_EPC_Future_Confirmed"].Substring(1);
        lblSysColor_AC_EPC_Past_Completed_Has_Invoice.Text                  = ConfigurationManager.AppSettings["BookingColour_AC_EPC_Past_Completed_Has_Invoice"].Substring(1);
        lblSysColor_AC_EPC_Past_Completed_No_Invoice.Text                   = ConfigurationManager.AppSettings["BookingColour_AC_EPC_Past_Completed_No_Invoice"].Substring(1);
        lblSysColor_AC_EPC_Future_Unconfirmed.Text                          = ConfigurationManager.AppSettings["BookingColour_AC_EPC_Future_Unconfirmed"].Substring(1);
        lblSysColor_AC_EPC_Future_Confirmed.Text                            = ConfigurationManager.AppSettings["BookingColour_AC_EPC_Future_Confirmed"].Substring(1);
        lblSysColor_CL_NonEPC_Past_Completed_Has_Invoice.Text               = ConfigurationManager.AppSettings["BookingColour_CL_NonEPC_Past_Completed_Has_Invoice"].Substring(1);
        lblSysColor_CL_NonEPC_Past_Completed_No_Invoice.Text                = ConfigurationManager.AppSettings["BookingColour_CL_NonEPC_Past_Completed_No_Invoice"].Substring(1);
        lblSysColor_CL_NonEPC_Future_Unconfirmed.Text                       = ConfigurationManager.AppSettings["BookingColour_CL_NonEPC_Future_Unconfirmed"].Substring(1);
        lblSysColor_CL_NonEPC_Future_Confirmed.Text                         = ConfigurationManager.AppSettings["BookingColour_CL_NonEPC_Future_Confirmed"].Substring(1);
        lblSysColor_AC_NonEPC_Past_Completed_Has_Invoice.Text               = ConfigurationManager.AppSettings["BookingColour_AC_NonEPC_Past_Completed_Has_Invoice"].Substring(1);
        lblSysColor_AC_NonEPC_Past_Completed_No_Invoice.Text                = ConfigurationManager.AppSettings["BookingColour_AC_NonEPC_Past_Completed_No_Invoice"].Substring(1);
        lblSysColor_AC_NonEPC_Future_Unconfirmed.Text                       = ConfigurationManager.AppSettings["BookingColour_AC_NonEPC_Future_Unconfirmed"].Substring(1);
        lblSysColor_AC_NonEPC_Future_Confirmed.Text                         = ConfigurationManager.AppSettings["BookingColour_AC_NonEPC_Future_Confirmed"].Substring(1);
        lblSysColor_Future_PatientLoggedIn.Text                             = ConfigurationManager.AppSettings["BookingColour_Future_PatientLoggedIn"].Substring(1);
        lblSysColor_Past_PatientLoggedIn.Text                               = ConfigurationManager.AppSettings["BookingColour_Past_PatientLoggedIn"].Substring(1);

        lblSysColor_Unavailable.BackColor                                   = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_Unavailable"].ToString());
        lblSysColor_Available.BackColor                                     = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_Available"].ToString());
        lblSysColor_UnavailableButAddable.BackColor                         = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_UnavailableButAddable"].ToString());
        lblSysColor_UnavailableButUpdatable.BackColor                       = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_UnavailableButUpdatable"].ToString());
        lblSysColor_Updatable.BackColor                                     = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_Updatable"].ToString());
        lblSysColor_FullDayTaken.BackColor                                  = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_FullDayTaken"].ToString());
        lblSysColor_CL_EPC_Past_Completed_Has_Invoice.BackColor             = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_CL_EPC_Past_Completed_Has_Invoice"].ToString());
        lblSysColor_CL_EPC_Past_Completed_No_Invoice.BackColor              = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_CL_EPC_Past_Completed_No_Invoice"].ToString());
        lblSysColor_CL_EPC_Future_Unconfirmed.BackColor                     = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_CL_EPC_Future_Unconfirmed"].ToString());
        lblSysColor_CL_EPC_Future_Confirmed.BackColor                       = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_CL_EPC_Future_Confirmed"].ToString());
        lblSysColor_AC_EPC_Past_Completed_Has_Invoice.BackColor             = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_AC_EPC_Past_Completed_Has_Invoice"].ToString());
        lblSysColor_AC_EPC_Past_Completed_No_Invoice.BackColor              = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_AC_EPC_Past_Completed_No_Invoice"].ToString());
        lblSysColor_AC_EPC_Future_Unconfirmed.BackColor                     = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_AC_EPC_Future_Unconfirmed"].ToString());
        lblSysColor_AC_EPC_Future_Confirmed.BackColor                       = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_AC_EPC_Future_Confirmed"].ToString());
        lblSysColor_CL_NonEPC_Past_Completed_Has_Invoice.BackColor          = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_CL_NonEPC_Past_Completed_Has_Invoice"].ToString());
        lblSysColor_CL_NonEPC_Past_Completed_No_Invoice.BackColor           = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_CL_NonEPC_Past_Completed_No_Invoice"].ToString());
        lblSysColor_CL_NonEPC_Future_Unconfirmed.BackColor                  = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_CL_NonEPC_Future_Unconfirmed"].ToString());
        lblSysColor_CL_NonEPC_Future_Confirmed.BackColor                    = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_CL_NonEPC_Future_Confirmed"].ToString());
        lblSysColor_AC_NonEPC_Past_Completed_Has_Invoice.BackColor          = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_AC_NonEPC_Past_Completed_Has_Invoice"].ToString());
        lblSysColor_AC_NonEPC_Past_Completed_No_Invoice.BackColor           = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_AC_NonEPC_Past_Completed_No_Invoice"].ToString());
        lblSysColor_AC_NonEPC_Future_Unconfirmed.BackColor                  = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_AC_NonEPC_Future_Unconfirmed"].ToString());
        lblSysColor_AC_NonEPC_Future_Confirmed.BackColor                    = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_AC_NonEPC_Future_Confirmed"].ToString());
        lblSysColor_Future_PatientLoggedIn.BackColor                        = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_Future_PatientLoggedIn"].ToString());
        lblSysColor_Past_PatientLoggedIn.BackColor                          = System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BookingColour_Past_PatientLoggedIn"].ToString());

        lblSysColor_Unavailable.Style["padding"]                            = "3px";
        lblSysColor_Available.Style["padding"]                              = "3px";
        lblSysColor_UnavailableButAddable.Style["padding"]                  = "3px";
        lblSysColor_UnavailableButUpdatable.Style["padding"]                = "3px";
        lblSysColor_Updatable.Style["padding"]                              = "3px";
        lblSysColor_FullDayTaken.Style["padding"]                           = "3px";
        lblSysColor_CL_EPC_Past_Completed_Has_Invoice.Style["padding"]      = "3px";
        lblSysColor_CL_EPC_Past_Completed_No_Invoice.Style["padding"]       = "3px";
        lblSysColor_CL_EPC_Future_Unconfirmed.Style["padding"]              = "3px";
        lblSysColor_CL_EPC_Future_Confirmed.Style["padding"]                = "3px";
        lblSysColor_AC_EPC_Past_Completed_Has_Invoice.Style["padding"]      = "3px";
        lblSysColor_AC_EPC_Past_Completed_No_Invoice.Style["padding"]       = "3px";
        lblSysColor_AC_EPC_Future_Unconfirmed.Style["padding"]              = "3px";
        lblSysColor_AC_EPC_Future_Confirmed.Style["padding"]                = "3px";
        lblSysColor_CL_NonEPC_Past_Completed_Has_Invoice.Style["padding"]   = "3px";
        lblSysColor_CL_NonEPC_Past_Completed_No_Invoice.Style["padding"]    = "3px";
        lblSysColor_CL_NonEPC_Future_Unconfirmed.Style["padding"]           = "3px";
        lblSysColor_CL_NonEPC_Future_Confirmed.Style["padding"]             = "3px";
        lblSysColor_AC_NonEPC_Past_Completed_Has_Invoice.Style["padding"]   = "3px";
        lblSysColor_AC_NonEPC_Past_Completed_No_Invoice.Style["padding"]    = "3px";
        lblSysColor_AC_NonEPC_Future_Unconfirmed.Style["padding"]           = "3px";
        lblSysColor_AC_NonEPC_Future_Confirmed.Style["padding"]             = "3px";
        lblSysColor_Future_PatientLoggedIn.Style["padding"]                 = "3px";
        lblSysColor_Past_PatientLoggedIn.Style["padding"]                   = "3px";






        if (!chkEnableNewPatientsCreateOwnLogins.Checked)
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>hide_show('pt_direct_link');</script>");
    
    }

    protected void UpdateValues()
    {

        try
        {
            lblErrorMessage.Text = string.Empty;

            txtMedicareMaxNbrServicesPerYear.Text        = txtMedicareMaxNbrServicesPerYear.Text.Trim();
            txtMedicareEclaimsLicenseNbr.Text            = txtMedicareEclaimsLicenseNbr.Text.Trim();
            txtMaxNbrProviders.Text                      = txtMaxNbrProviders.Text.Trim();
            txtSMSPrice.Text                             = txtSMSPrice.Text.Trim();
            txtStockWarningNotificationEmailAddress.Text = txtStockWarningNotificationEmailAddress.Text.Trim();
            txtSite.Text                                 = txtSite.Text.Trim();
            txtEmail_FromName.Text                       = txtEmail_FromName.Text.Trim();
            txtEmail_FromEmail.Text                      = txtEmail_FromEmail.Text.Trim();
            txtAdminAlertEmail_To.Text                   = txtAdminAlertEmail_To.Text.Trim();
            txtServiceSpecificBookingReminderLettersToBatch_EmailAddress.Text = txtServiceSpecificBookingReminderLettersToBatch_EmailAddress.Text.Trim();
            txtSMSCreditNotificationEmailAddress.Text    = txtSMSCreditNotificationEmailAddress.Text.Trim();
            txtSMSCreditLowBalance_Threshold.Text        = txtSMSCreditLowBalance_Threshold.Text.Trim();


            int n;
            decimal d;

            if (!int.TryParse(txtMedicareMaxNbrServicesPerYear.Text, out n))
                throw new CustomMessageException("Medicare Max Nbr Services Per Year must be an integer");
            if (txtMedicareEclaimsLicenseNbr.Text.Length == 0)
                throw new CustomMessageException("Medicare Eclaims License Nbr is a required field");
            if (!int.TryParse(txtMaxNbrProviders.Text, out n))
                throw new CustomMessageException("Max Nbr Providers must be an integer");
            if (!decimal.TryParse(txtSMSPrice.Text, out d))
                throw new CustomMessageException("SMS Price must be a decimal");
            else if (d < 0)
                throw new CustomMessageException("SMS Price must be at least 0.00");

            if (!decimal.TryParse(txtRateIncomingCall.Text, out d))
                throw new CustomMessageException("Rate of Incoming Call must be a decimal");
            else if (d < 0)
                throw new CustomMessageException("Rate of Incoming Call must be at least 0.00");
            if (!decimal.TryParse(txtRateOutgoingCall.Text, out d))
                throw new CustomMessageException("Rate of Outgoing Call must be a decimal");
            else if (d < 0)
                throw new CustomMessageException("Rate of Outgoing Call must be at least 0.00");
            if (!decimal.TryParse(txtRateCreditCardAmt.Text, out d))
                throw new CustomMessageException("Rate of Credit Card (Amt) must be a decimal");
            else if (d < 0)
                throw new CustomMessageException("Rate of Credit Card (Amt) must be at least 0.00");
            if (!decimal.TryParse(txtRateCreditCardPct.Text, out d))
                throw new CustomMessageException("Rate of Credit Card (%) must be a decimal");
            else if (d < 0)
                throw new CustomMessageException("Rate of Credit Card (%) must be at least 0.00");
            if (!decimal.TryParse(txtRateDebitCardAmt.Text, out d))
                throw new CustomMessageException("Rate of Debit Card (Amt) must be a decimal");
            else if (d < 0)
                throw new CustomMessageException("Rate of Debit Card (Amt) must be at least 0.00");
            if (!decimal.TryParse(txtRateDebitCardPct.Text, out d))
                throw new CustomMessageException("Rate of Debit Card (%) must be a decimal");
            else if (d < 0)
                throw new CustomMessageException("Rate of Debit Card (%) must be at least 0.00");


            DateTime EziDebit_FormDate = DateTime.MinValue;

            if (ddlEziDebit_FormDate_Day.SelectedValue == "-1" &&
                ddlEziDebit_FormDate_Month.SelectedValue == "-1" &&
                ddlEziDebit_FormDate_Year.SelectedValue == "-1")
            {
                EziDebit_FormDate = DateTime.MinValue;
            }
            else if (ddlEziDebit_FormDate_Day.SelectedValue != "-1" &&
                ddlEziDebit_FormDate_Month.SelectedValue != "-1" &&
                ddlEziDebit_FormDate_Year.SelectedValue != "-1")
            {
                try
                {
                    EziDebit_FormDate = new DateTime(
                        Convert.ToInt32(ddlEziDebit_FormDate_Year.SelectedValue),
                        Convert.ToInt32(ddlEziDebit_FormDate_Month.SelectedValue),
                        Convert.ToInt32(ddlEziDebit_FormDate_Day.SelectedValue)
                        );
                }
                catch (Exception)
                {
                    throw new CustomMessageException("EziDebit Form Date must be a valid date");
                }
            }
            else
            {
                throw new CustomMessageException("EziDebit Form Date must be ALL blank or ALL set as a valid date");
            }



            if (txtStockWarningNotificationEmailAddress.Text.Length > 0 && !Utilities.IsValidEmailAddress(txtStockWarningNotificationEmailAddress.Text))
                throw new CustomMessageException("Stock Warning Notification Email Address must either be blank or a valid email address.");
            if (txtSite.Text.Length == 0)
                throw new CustomMessageException("Site Name is a required field");
            if (txtEmail_FromName.Text.Length == 0)
                throw new CustomMessageException("Email From Name is a required field");
            if (!Utilities.IsValidEmailAddress(txtEmail_FromEmail.Text))
                throw new CustomMessageException("Email From Email must be a valid email address.");
            if (!Utilities.IsValidEmailAddress(txtAdminAlertEmail_To.Text))
                throw new CustomMessageException("Admin Alert Email must be a valid email address.");
            if (txtServiceSpecificBookingReminderLettersToBatch_EmailAddress.Text.Length > 0 && !Utilities.IsValidEmailAddress(txtServiceSpecificBookingReminderLettersToBatch_EmailAddress.Text))
                throw new CustomMessageException("Service Specific Booking Reminder Letters To Batch - Email Address must either be blank or a valid email address.");
            if ((chkSMSCreditOutOfBalance_SendEmail.Checked || chkSMSCreditLowBalance_SendEmail.Checked) && !Utilities.IsValidEmailAddress(txtSMSCreditNotificationEmailAddress.Text))
                throw new CustomMessageException("SMS Credit Notification Email Address must be a valid email address if you check either 'SMS Credit Out Of Balance' or 'SMS Credit Low Balance'.");
            if (txtSMSCreditNotificationEmailAddress.Text.Length > 0 && !Utilities.IsValidEmailAddress(txtSMSCreditNotificationEmailAddress.Text))
                throw new CustomMessageException("SMS Credit Notification Email Address must either be blank or a valid email address.");
            if (!decimal.TryParse(txtSMSCreditLowBalance_Threshold.Text, out d))
                throw new CustomMessageException("SMS Credit Low Balance Threshold must be a decimal");
            if (!int.TryParse(txtNextAvailableDefaultNbrDaysShown.Text, out n))
                throw new CustomMessageException("'<i>Next Available - Default Number Of Days To Show</i>' &nbsp;&nbsp; must be a number");



            UserView userView = UserView.GetInstance();

            if (userView.IsStakeholder)
            {
                SystemVariableDB.Update("MedicareMaxNbrServicesPerYear"             , txtMedicareMaxNbrServicesPerYear.Text);
                SystemVariableDB.Update("MedicareEclaimsLicenseNbr"                 , txtMedicareEclaimsLicenseNbr.Text);
                SystemVariableDB.Update("AutoMedicareClaiming"                      , chkAutoMedicareClaiming.Checked ? "1" : "0");
                SystemVariableDB.Update("MaxNbrProviders"                           , Convert.ToInt32(txtMaxNbrProviders.Text).ToString());
                SystemVariableDB.Update("SMSPrice"                                  , Convert.ToDouble(txtSMSPrice.Text).ToString("0.00"));
                SystemVariableDB.Update("StockWarningNotificationEmailAddress"      , txtStockWarningNotificationEmailAddress.Text);
                SystemVariableDB.Update("AllowAddSiteClinic"                        , chkAllowAddSiteClinic.Checked   ? "1" : "0");
                SystemVariableDB.Update("AllowAddSiteAgedCare"                      , chkAllowAddSiteAgedCare.Checked ? "1" : "0");
                SystemVariableDB.Update("AllowAddSiteGP"                            , chkAllowAddSiteGP.Checked       ? "1" : "0");
                SystemVariableDB.Update("PaymentDueDayOfMonth"                      , Convert.ToInt32(ddlDayOfMonthPaymentDue.SelectedValue).ToString());
                SystemVariableDB.Update("UseMediclinicCallCenter"                   , chkUseCallCenter.Checked ? "1" : "0");
                SystemVariableDB.Update("CallCenterPrefix"                          , txtCallCenterPrefix.Text);
                SystemVariableDB.Update("AutoSendFaxesAsEmailsIfNoEmailExistsToGPs" , chkAutoSendFaxesAsEmailsIfNoEmailExistsToGPs.Checked ? "1" : "0");

                SystemVariableDB.Update("CC_Nbr"                                 , txtCC_Nbr.Text.Trim());
                SystemVariableDB.Update("CC_Exp_Mo"                              , txtCC_Exp_Mo.Text.Trim());
                SystemVariableDB.Update("CC_Exp_Yr"                              , txtCC_Exp_Yr.Text.Trim());
                SystemVariableDB.Update("CC_CCV"                                 , txtCC_CCV.Text.Trim());
                SystemVariableDB.Update("Rate_IncomingCall"                      , Convert.ToDouble(txtRateIncomingCall.Text).ToString("0.00"));
                SystemVariableDB.Update("Rate_OutgoingCall"                      , Convert.ToDouble(txtRateOutgoingCall.Text).ToString("0.00"));
                SystemVariableDB.Update("Rate_CreditCardAmt"                     , Convert.ToDouble(txtRateCreditCardAmt.Text).ToString("0.00"));
                SystemVariableDB.Update("Rate_CreditCardPct"                     , Convert.ToDouble(txtRateCreditCardPct.Text).ToString("0.00"));
                SystemVariableDB.Update("Rate_DebitCardAmt"                      , Convert.ToDouble(txtRateDebitCardAmt.Text).ToString("0.00"));
                SystemVariableDB.Update("Rate_DebitCardPct"                      , Convert.ToDouble(txtRateDebitCardPct.Text).ToString("0.00"));
                SystemVariableDB.Update("EziDebit_Enabled"                       , chkEziDebit_Enabled.Checked ? "1" : "0");
                SystemVariableDB.Update("EziDebit_DigitalKey"                    , txtEziDebit_DigitalKey.Text);
                SystemVariableDB.Update("EziDebit_FormDate"                      , EziDebit_FormDate == DateTime.MinValue ? "" : EziDebit_FormDate.ToString("yyyy-MM-dd"));
            }

            SystemVariableDB.Update("Site"                                       , txtSite.Text);
            SystemVariableDB.Update("BannerMessage"                              , txtBannerMessage.Text);
            SystemVariableDB.Update("ShowBannerMessage"                          , chkShowBannerMessage.Checked ? "True" : "False");
            SystemVariableDB.Update("Email_FromName"                             , txtEmail_FromName.Text);
            SystemVariableDB.Update("Email_FromEmail"                            , txtEmail_FromEmail.Text);
            SystemVariableDB.Update("AdminAlertEmail_To"                         , txtAdminAlertEmail_To.Text);
            SystemVariableDB.Update("PTLogin_BookingTimeEditable"                , chkPTLogin_BookingTimeEditable.Checked                ? "1" : "0");
            SystemVariableDB.Update("EnableAlert_BookingAddedByPT"               , chkPTAddSelfBookingAlert.Checked                      ? "1" : "0");
            SystemVariableDB.Update("EnableAlert_BookingEditedByPT"              , chkPTEditSelfBookingAlert.Checked                     ? "1" : "0");
            SystemVariableDB.Update("EnableAlert_BookingDeletedByPT"             , chkPTDeleteSelfBookingAlert.Checked                   ? "1" : "0");
            SystemVariableDB.Update("EnableDeletedBookingsAlerts"                , chkEnableDeletedBookingsAlerts.Checked                ? "1" : "0");
            SystemVariableDB.Update("AllowPatientLogins"                         , chkEnablePatientLogins.Checked                        ? "1" : "0");
            SystemVariableDB.Update("Bookings_ProvsCanSeeOtherProvs"             , chkBookings_ProvsCanSeeOtherProvs.Checked             ? "1" : "0");
            SystemVariableDB.Update("Bookings_ProvsCanSeePatientsOfAllOrgs"      , chkBookings_ProvsCanSeePatientsOfAllOrgs.Checked      ? "1" : "0");
            SystemVariableDB.Update("ProvsCanSeePricesWhenCompletingBks_AC"      , chkProvsCanSeePricesWhenCompletingBks_AC.Checked      ? "1" : "0");
            SystemVariableDB.Update("AllowPatientsToCreateOwnLogin"              , chkEnableExistingPatientsCreateOwnLogins.Checked      ? "1" : "0");
            SystemVariableDB.Update("AllowPatientsToCreateOwnRecords"            , chkEnableNewPatientsCreateOwnLogins.Checked           ? "1" : "0");
            SystemVariableDB.Update("ServiceSpecificBookingReminderLettersToBatch_EmailAddress", txtServiceSpecificBookingReminderLettersToBatch_EmailAddress.Text);
            SystemVariableDB.Update("DefaultState"                               , ddlDefaultState.SelectedValue);
            SystemVariableDB.Update("SMSCreditNotificationEmailAddress"          , txtSMSCreditNotificationEmailAddress.Text);
            SystemVariableDB.Update("SMSCreditOutOfBalance_SendEmail"            , chkSMSCreditOutOfBalance_SendEmail.Checked            ? "1" : "0");
            SystemVariableDB.Update("SMSCreditLowBalance_SendEmail"              , chkSMSCreditLowBalance_SendEmail.Checked              ? "1" : "0");
            SystemVariableDB.Update("SMSCreditLowBalance_Threshold"              , Convert.ToDouble(txtSMSCreditLowBalance_Threshold.Text).ToString("0.00"));
            SystemVariableDB.Update("EnableLastEPCReminderSMS"                   , chkEnableLastEPCReminderSMS.Checked                   ? "1" : "0");
            SystemVariableDB.Update("EnableAutoMontlyOverdueReminders"           , chkEnableAutoMontlyOverdueReminders.Checked           ? "1" : "0");
            SystemVariableDB.Update("EnableLastEPCReminderEmails"                , chkEnableLastEPCReminderEmails.Checked                ? "1" : "0");
            SystemVariableDB.Update("NextAvailableDefaultNbrDaysShown"           , txtNextAvailableDefaultNbrDaysShown.Text);
            SystemVariableDB.Update("EnableDailyBookingReminderSMS"              , chkEnableDailyBookingReminderSMS.Checked              ? "1" : "0");
            SystemVariableDB.Update("EnableDailyBookingReminderEmails"           , chkEnableDailyBookingReminderEmails.Checked           ? "1" : "0");
            SystemVariableDB.Update("PT_Reminders_HasBothSMSandEmail"            , ddlPT_Reminders_HasBothSMSandEmail.SelectedValue);
            SystemVariableDB.Update("NbrDaysAheadToSendDailyBookingReminderSMS"  , ddlNbrDaysAheadToSendDailyBookingReminderSMS.SelectedValue);
            SystemVariableDB.Update("SendDailyBookingReminderText_SMS"           , txtSendDailyBookingReminderText_SMS.Text);
            SystemVariableDB.Update("SendDailyBookingReminderText_Email"         , FreeTextBox2.Text);
            SystemVariableDB.Update("SendDailyBookingReminderText_EmailSubject"  , txtSendDailyBookingReminderText_EmailSubect.Text);

            SystemVariableDB.Update("EnableDailyStaffBookingsReminderSMS"        , chkEnableDailyStaffBookingReminderSMS.Checked         ? "1" : "0");
            SystemVariableDB.Update("EnableDailyStaffBookingsReminderEmails"     , chkEnableDailyStaffBookingReminderEmails.Checked      ? "1" : "0");
            SystemVariableDB.Update("Staff_Reminders_HasBothSMSandEmail"         , ddlStaff_Reminders_HasBothSMSandEmail.SelectedValue);
            SystemVariableDB.Update("EnableBirthdaySMS"                          , chkEnableBirthdaySMS.Checked                          ? "1" : "0");
            SystemVariableDB.Update("EnableBirthdayEmails"                       , chkEnableBirthdayEmails.Checked                       ? "1" : "0");
            SystemVariableDB.Update("InvoiceGapPayments"                         , chkInvoiceGapPayments.Checked                         ? "1" : "0");
            SystemVariableDB.Update("LettersEmailDefaultSubject"                 , txtLettersEmailDeafultSubjectLine.Text);
            SystemVariableDB.Update("LettersEmailSignature"                      , FreeTextBox1.Text);

            SystemVariableDB.Update("BookingColour_Unavailable"                             , "#" + ColorPicker_Unavailable.Value);
            SystemVariableDB.Update("BookingColour_Available"                               , "#" + ColorPicker_Available.Value);
            SystemVariableDB.Update("BookingColour_UnavailableButAddable"                   , "#" + ColorPicker_UnavailableButAddable.Value);
            SystemVariableDB.Update("BookingColour_UnavailableButUpdatable"                 , "#" + ColorPicker_UnavailableButUpdatable.Value);
            SystemVariableDB.Update("BookingColour_Updatable"                               , "#" + ColorPicker_Updatable.Value);
            SystemVariableDB.Update("BookingColour_FullDayTaken"                            , "#" + ColorPicker_FullDayTaken.Value);
            SystemVariableDB.Update("BookingColour_CL_EPC_Past_Completed_Has_Invoice"       , "#" + ColorPicker_CL_EPC_Past_Completed_Has_Invoice.Value);
            SystemVariableDB.Update("BookingColour_CL_EPC_Past_Completed_No_Invoice"        , "#" + ColorPicker_CL_EPC_Past_Completed_No_Invoice.Value);
            SystemVariableDB.Update("BookingColour_CL_EPC_Future_Unconfirmed"               , "#" + ColorPicker_CL_EPC_Future_Unconfirmed.Value);
            SystemVariableDB.Update("BookingColour_CL_EPC_Future_Confirmed"                 , "#" + ColorPicker_CL_EPC_Future_Confirmed.Value);
            SystemVariableDB.Update("BookingColour_AC_EPC_Past_Completed_Has_Invoice"       , "#" + ColorPicker_AC_EPC_Past_Completed_Has_Invoice.Value);
            SystemVariableDB.Update("BookingColour_AC_EPC_Past_Completed_No_Invoice"        , "#" + ColorPicker_AC_EPC_Past_Completed_No_Invoice.Value);
            SystemVariableDB.Update("BookingColour_AC_EPC_Future_Unconfirmed"               , "#" + ColorPicker_AC_EPC_Future_Unconfirmed.Value);
            SystemVariableDB.Update("BookingColour_AC_EPC_Future_Confirmed"                 , "#" + ColorPicker_AC_EPC_Future_Confirmed.Value);
            SystemVariableDB.Update("BookingColour_CL_NonEPC_Past_Completed_Has_Invoice"    , "#" + ColorPicker_CL_NonEPC_Past_Completed_Has_Invoice.Value);
            SystemVariableDB.Update("BookingColour_CL_NonEPC_Past_Completed_No_Invoice"     , "#" + ColorPicker_CL_NonEPC_Past_Completed_No_Invoice.Value);
            SystemVariableDB.Update("BookingColour_CL_NonEPC_Future_Unconfirmed"            , "#" + ColorPicker_CL_NonEPC_Future_Unconfirmed.Value);
            SystemVariableDB.Update("BookingColour_CL_NonEPC_Future_Confirmed"              , "#" + ColorPicker_CL_NonEPC_Future_Confirmed.Value);
            SystemVariableDB.Update("BookingColour_AC_NonEPC_Past_Completed_Has_Invoice"    , "#" + ColorPicker_AC_NonEPC_Past_Completed_Has_Invoice.Value);
            SystemVariableDB.Update("BookingColour_AC_NonEPC_Past_Completed_No_Invoice"     , "#" + ColorPicker_AC_NonEPC_Past_Completed_No_Invoice.Value);
            SystemVariableDB.Update("BookingColour_AC_NonEPC_Future_Unconfirmed"            , "#" + ColorPicker_AC_NonEPC_Future_Unconfirmed.Value);
            SystemVariableDB.Update("BookingColour_AC_NonEPC_Future_Confirmed"              , "#" + ColorPicker_AC_NonEPC_Future_Confirmed.Value);
            SystemVariableDB.Update("BookingColour_Future_PatientLoggedIn"                  , "#" + ColorPicker_Future_PatientLoggedIn.Value);
            SystemVariableDB.Update("BookingColour_Past_PatientLoggedIn"                    , "#" + ColorPicker_Past_PatientLoggedIn.Value);


            bool siteNameChanged =  ((SystemVariables)Session["SystemVariables"])["Site"].Value != txtSite.Text;
            bool bannerChanged   = (((SystemVariables)Session["SystemVariables"])["ShowBannerMessage"].Value != (chkShowBannerMessage.Checked ? "True" : "False")) ||
                                   chkShowBannerMessage.Checked && (((SystemVariables)Session["SystemVariables"])["BannerMessage"].Value != txtBannerMessage.Text);

            Session["SystemVariables"] = SystemVariableDB.GetAll();

            if (siteNameChanged || bannerChanged)
            {
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                ResetValues();
                SetErrorMessage("Updated.");
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

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        UpdateValues();
    }
    

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

}