<%@ Page Title="Website Settings" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="SettingsV2.aspx.cs" Inherits="SettingsV2" ValidateRequest="false"  %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="ScriptsV2/jscolor.js"></script>
    <script type="text/javascript">

        function hide_show(id) {
            var e = document.getElementById(id);
            if (e != null)
                e.style.display = e.style.display == "none" ? "" : "none";
        }


        function ensure_ishex(txtbox) {
            txtbox.value = txtbox.value.toUpperCase();

            /*
            var new_value = "";
            for (var i = 0, len = txtbox.value.length; i < len; i++)
                if (is_hex(txtbox.value.charAt(i))) new_value += txtbox.value.charAt(i);

            if (txtbox.value != new_value)
                alert("Please ensure character is a valid hexadecimal (0-9 or A-F)");

            txtbox.value = new_value;
            */
        }
        function is_hex(c) {
            validChar = '012345678ABCDEF';   // legal chars
            return validChar.indexOf(c) >= 0;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Website Settings</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">

            <div class="user_login_form">

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="btnUpdate">

                    <div id="autodivheight" class="divautoheight" style="height:500px;border:1px solid #606060;width:auto;padding-right:17px;">
                        <table style="padding:12px;border-collapse:separate;">
                            <tr id="tr_MedicareMaxNbrServicesPerYear" runat="server">
                                <td class="nowrap">
                                        <asp:RequiredFieldValidator ID="txtMedicareMaxNbrServicesPerYearRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtMedicareMaxNbrServicesPerYear" 
                                            ErrorMessage="Medicare Max Nbr Services Per Year is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="txtMedicareMaxNbrServicesPerYearIsInteger" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtMedicareMaxNbrServicesPerYear" 
                                            Operator="DataTypeCheck" Type="Integer"
                                            ErrorMessage="Medicare Max Nbr Services Per Year must be an integer."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CompareValidator>
                                </td>
                                <td>Medicare Max Nbr Services Per Year</td>
                                <td style="min-width:10px;"></td>
                                <td><asp:TextBox ID="txtMedicareMaxNbrServicesPerYear" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_AutoMedicareClaiming" runat="server">
                                <td></td>
                                <td>Auto Medicare Claiming</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkAutoMedicareClaiming" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr id="tr_MedicareEclaimsLicenseNbr" runat="server">
                                <td class="nowrap">
                                        <asp:RequiredFieldValidator ID="txtMedicareEclaimsLicenseNbrRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtMedicareEclaimsLicenseNbr" 
                                            ErrorMessage="Medicare Eclaims License Nbr is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtMedicareEclaimsLicenseNbrRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtMedicareEclaimsLicenseNbr"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Medicare Eclaims License Nbr can only be alphanumeric, hyphens, or underscore."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </td>
                                <td>Medicare Eclaims License Nbr</td>
                                <td></td>
                                <td><asp:TextBox ID="txtMedicareEclaimsLicenseNbr" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_MaxNbrProviders" runat="server">
                                <td class="nowrap">
                                        <asp:RequiredFieldValidator ID="txtMaxNbrProvidersRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtMaxNbrProviders" 
                                            ErrorMessage="Max Nbr Providers is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="txtMaxNbrProvidersIsInteger" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtMaxNbrProviders" 
                                            Operator="DataTypeCheck" Type="Integer"
                                            ErrorMessage="Max Nbr Providers must be an integer."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CompareValidator>
                                </td>
                                <td>Max Nbr Providers</td>
                                <td></td>
                                <td><asp:TextBox ID="txtMaxNbrProviders" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_SMSPrice" runat="server">
                                <td class="nowrap">
                                        <asp:RequiredFieldValidator ID="txtSMSPriceRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtSMSPrice" 
                                            ErrorMessage="SMS Price is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RangeValidator ID="txtSMSPriceRange" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtSMSPrice"
                                            Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                            ErrorMessage="SMS Price must be between 0.00 and 999.99" 
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                                </td>
                                <td>SMS Price</td>
                                <td></td>
                                <td><asp:TextBox ID="txtSMSPrice" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_StockWarningNotificationEmailAddress" runat="server" class="hiddencol">
                                <td></td>
                                <td>Stock Warning Notification Email Address</td>
                                <td></td>
                                <td><asp:TextBox ID="txtStockWarningNotificationEmailAddress" runat="server" Columns="40"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_AllowAddSiteClinic" runat="server" class="hiddencol">
                                <td></td>
                                <td>Allow Adding Site Clinic</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkAllowAddSiteClinic" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr id="tr_AllowAddSiteAgedCare" runat="server" class="hiddencol">
                                <td></td>
                                <td>Allow Adding Site AgedCare</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkAllowAddSiteAgedCare" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr id="tr_AllowAddSiteGP" runat="server" class="hiddencol">
                                <td></td>
                                <td>Allow Adding Site GP</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkAllowAddSiteGP" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr id="tr_DayOfMonthPaymentDue" runat="server">
                                <td></td>
                                <td>Day Of Month Payemnt Due</td>
                                <td></td>
                                <td><asp:DropDownList ID="ddlDayOfMonthPaymentDue" runat="server"/></td>
                            </tr>

                            <tr id="tr_SpaceBetweenStakeholderFields1" runat="server">
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr id="tr_UseCallCenter" runat="server">
                                <td></td>
                                <td>Use Mediclinic Call Center</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkUseCallCenter" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr id="tr_CallCenterPrefix" runat="server">
                                <td></td>
                                <td>Call Center Prefix</td>
                                <td></td>
                                <td><asp:TextBox ID="txtCallCenterPrefix" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_AutoSendFaxesAsEmailsIfNoEmailExistsToGPs" runat="server">
                                <td></td>
                                <td>Auto Send Faxes As House.Of.IT Email If No Email Set</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkAutoSendFaxesAsEmailsIfNoEmailExistsToGPs" runat="server"></asp:CheckBox></td>
                            </tr>

                            <tr id="tr_SpaceBetweenStakeholderFields2" runat="server">
                                <td style="height:10px;" colspan="4"></td>
                            </tr>

                            <tr id="tr_CC_Nbr" runat="server">
                                <td></td>
                                <td>Credit Card Nbr</td>
                                <td></td>
                                <td><asp:TextBox ID="txtCC_Nbr" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_CC_Exp" runat="server">
                                <td></td>
                                <td>Credit Card Exp.</td>
                                <td></td>
                                <td><asp:TextBox ID="txtCC_Exp_Mo" runat="server" Columns="2" /> / <asp:TextBox ID="txtCC_Exp_Yr" runat="server" Columns="4" /></td>
                            </tr>
                            <tr id="tr_CC_CCV" runat="server">
                                <td></td>
                                <td>Credit Card CCV</td>
                                <td></td>
                                <td><asp:TextBox ID="txtCC_CCV" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>

                            <tr id="tr_SpaceBetweenStakeholderFields3" runat="server">
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr id="tr_RateIncomingCall" runat="server">
                                <td></td>
                                <td>Rate - Incoming Call</td>
                                <td></td>
                                <td><asp:TextBox ID="txtRateIncomingCall" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_RateOutgoingCall" runat="server">
                                <td></td>
                                <td>Rate - Outgoing Call</td>
                                <td></td>
                                <td><asp:TextBox ID="txtRateOutgoingCall" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_RateCreditCardAmt" runat="server">
                                <td></td>
                                <td>Rate - Credit Card (Amount)</td>
                                <td></td>
                                <td><asp:TextBox ID="txtRateCreditCardAmt" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_RateCreditCardPct" runat="server">
                                <td></td>
                                <td>Rate - Credit Card (%)</td>
                                <td></td>
                                <td><asp:TextBox ID="txtRateCreditCardPct" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_RateDebitCardAmt" runat="server">
                                <td></td>
                                <td>Rate - Debit Card (Amount)</td>
                                <td></td>
                                <td><asp:TextBox ID="txtRateDebitCardAmt" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_RateDebitCardPct" runat="server">
                                <td></td>
                                <td>Rate - Debit Card (%)</td>
                                <td></td>
                                <td><asp:TextBox ID="txtRateDebitCardPct" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>

                            <tr id="tr_SpaceBetweenStakeholderFields4" runat="server">
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr id="tr_EziDebit_Enabled" runat="server">
                                <td></td>
                                <td>EziDebit - Enabled</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEziDebit_Enabled" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr id="tr_EziDebit_DigitalKey" runat="server">
                                <td></td>
                                <td>EziDebit - DigitalKey</td>
                                <td></td>
                                <td><asp:TextBox ID="txtEziDebit_DigitalKey" runat="server" Columns="65"></asp:TextBox></td>
                            </tr>
                            <tr id="tr_EziDebit_FormDate" runat="server">
                                <td></td>
                                <td>EziDebit - Form Date</td>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlEziDebit_FormDate_Day" runat="server"/>
                                    <asp:DropDownList ID="ddlEziDebit_FormDate_Month" runat="server"/>
                                    <asp:DropDownList ID="ddlEziDebit_FormDate_Year" runat="server"/>
                                </td>
                            </tr>




                            <tr id="tr_SpaceTrailingStakeholderFields" runat="server">
                                <td colspan="4">
                                    <div style="min-height:85px;vertical-align:middle;text-align:center;">
                                        <br />
                                        <br />
                                        <i>End Of Stakeholder Settings</i>
                                        <hr style="width:65%;border-color:#191919;" />
                                        <br />
                                        <br />
                                    </div>
                                 </td>
                            </tr>




                            <tr>
                                <td class="nowrap">
                                        <asp:RequiredFieldValidator ID="txtSiteRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtSite" 
                                            ErrorMessage="Site Name is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                </td>
                                <td>Site Name</td>
                                <td style="min-width:10px;"></td>
                                <td><asp:TextBox ID="txtSite" runat="server" Columns="40"></asp:TextBox></td>
                            </tr>
                            <tr runat="server" visible="false">
                                <td></td>
                                <td>Banner Message</td>
                                <td></td>
                                <td><asp:TextBox ID="txtBannerMessage" runat="server" Columns="40"></asp:TextBox></td>
                            </tr>
                            <tr runat="server" visible="false">
                                <td></td>
                                <td>Show Banner Message</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkShowBannerMessage" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="nowrap">
                                        <asp:RequiredFieldValidator ID="txtEmail_FromNameRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtEmail_FromName" 
                                            ErrorMessage="Email - From Name is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                </td>
                                <td>Email - From Name</td>
                                <td></td>
                                <td><asp:TextBox ID="txtEmail_FromName" runat="server" Columns="40"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="nowrap">
                                        <asp:RequiredFieldValidator ID="txtEmail_FromEmailRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtEmail_FromEmail" 
                                            ErrorMessage="Email - From Email is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                </td>
                                <td>Email - From Email (pt/ref emails sent from this address)</td>
                                <td></td>
                                <td><asp:TextBox ID="txtEmail_FromEmail" runat="server" Columns="40"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="nowrap">
                                        <asp:RequiredFieldValidator ID="txtAdminAlertEmail_ToRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtAdminAlertEmail_To" 
                                            ErrorMessage="Admin Alert Email is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                </td>
                                <td>Admin Alert Email (admin alerts eg deleted bookings sent here)</td>
                                <td></td>
                                <td><asp:TextBox ID="txtAdminAlertEmail_To" runat="server" Columns="40"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable Deleted/Reversed Booking Alerts</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableDeletedBookingsAlerts" runat="server"></asp:CheckBox></td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>Providers Can See Other Providers On The Booking Sheet</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkBookings_ProvsCanSeeOtherProvs" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Providers Can See Patients Of All Clinics</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkBookings_ProvsCanSeePatientsOfAllOrgs" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Providers Can See Prices When Completing Bks [Aged Care]</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkProvsCanSeePricesWhenCompletingBks_AC" runat="server"></asp:CheckBox></td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>Enable Patient Logins</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnablePatientLogins" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr runat="server" visible="false">
                                <td></td>
                                <td>Enable Existing Patients To Create Their Own Logins</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableExistingPatientsCreateOwnLogins" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable New Patients To Create Their Own Logins</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableNewPatientsCreateOwnLogins" runat="server" onclick="hide_show('pt_direct_link');"></asp:CheckBox></td>
                            </tr>
                            <tr id="pt_direct_link">
                                <td></td>
                                <td>Link For Patients To Create Their Own Logins</td>
                                <td></td>
                                <td><asp:HyperLink ID="lnkPtDirectLinkToCreateOwnLogin" runat="server"></asp:HyperLink></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>PT Login - Can Edit Booking Time</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkPTLogin_BookingTimeEditable" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable PT Added Self Booking Alerts</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkPTAddSelfBookingAlert" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable PT Edited Self Booking Alerts</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkPTEditSelfBookingAlert" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable PT Deleted Self Booking Alerts</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkPTDeleteSelfBookingAlert" runat="server"></asp:CheckBox></td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td class="nowrap">Service Specific Booking Reminder Letters To Batch - Email Address</td>
                                <td></td>
                                <td><asp:TextBox ID="txtServiceSpecificBookingReminderLettersToBatch_EmailAddress" runat="server" Columns="40"></asp:TextBox></td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>Default State When Adding/Searching Suburbs</td>
                                <td></td>
                                <td><asp:DropDownList ID="ddlDefaultState" runat="server" /></td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>SMS Credit Notification Email Address</td>
                                <td></td>
                                <td><asp:TextBox ID="txtSMSCreditNotificationEmailAddress" runat="server" Columns="40"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>SMS Credit Out Of Balance - SendEmail</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkSMSCreditOutOfBalance_SendEmail" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>SMS Credit Low Balance - Send Email</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkSMSCreditLowBalance_SendEmail" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="nowrap">
                                        <asp:RequiredFieldValidator ID="txtSMSCreditLowBalance_ThresholdRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtSMSCreditLowBalance_Threshold" 
                                            ErrorMessage="SMS Credit Low Balance Threshold is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RangeValidator ID="txtSMSCreditLowBalance_ThresholdRange" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtSMSCreditLowBalance_Threshold"
                                            Type="Currency" MinimumValue="0.00" MaximumValue="9999.99" 
                                            ErrorMessage="SMS Credit Low Balance Threshold must be between 0.00 and 9999.99" 
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                                </td>
                                <td>SMS Credit Low Balance Threshold</td>
                                <td></td>
                                <td><asp:TextBox ID="txtSMSCreditLowBalance_Threshold" runat="server" Columns="12"></asp:TextBox></td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>Enable 'Renew After Last EPC Used' Reminder SMS's For Patients</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableLastEPCReminderSMS" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable 'Renew After Last EPC Used' Reminder Email's For Patients</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableLastEPCReminderEmails" runat="server"></asp:CheckBox></td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>Enable Automatic Montly Overdue Invoice Emailing</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableAutoMontlyOverdueReminders" runat="server"></asp:CheckBox></td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>Next Available Booking - Default Number Of Days To Show</td>
                                <td></td>
                                <td><asp:TextBox ID="txtNextAvailableDefaultNbrDaysShown" runat="server" Columns="3"/></td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>Enable PT Booking Reminder SMS's</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableDailyBookingReminderSMS" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable PT Booking Reminder Emails</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableDailyBookingReminderEmails" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>What To Send If PT Has Both SMS & Email Set</td>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlPT_Reminders_HasBothSMSandEmail" runat="server">
                                        <asp:ListItem Text="Send Both SMS & Email" Value="Both"></asp:ListItem>
                                        <asp:ListItem Text="Send Only SMS" Value="SMS"></asp:ListItem>
                                        <asp:ListItem Text="Send Only Email" Value="Email"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>No. Days Ahead To Send PT Booking Reminders</td>
                                <td></td>
                                <td><asp:DropDownList ID="ddlNbrDaysAheadToSendDailyBookingReminderSMS" runat="server" /></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="vertical-align:top;">
                                    PT Booking Reminders - SMS Text
                                    
                                    <div style="height:9px;"></div>
                                    <table>
                                        <tr>
                                            <td></td>
                                            <td colspan="3"><u>Replaceable Fields</u><div style="height:3px;"></div></td>
                                        </tr>
                                        <tr>
                                            <td style="min-width:36px;"></td>
                                            <td>pt_firstname</td>
                                            <td style="min-width:18px;"></td>
                                            <td>Patient's First Name</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>pt_fullname</td>
                                            <td></td>
                                            <td>Patient's First + Last Name</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>provider_fullname</td>
                                            <td></td>
                                            <td>Provider's First + Last Name</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>serivce_field</td>
                                            <td></td>
                                            <td>Eg. Podiatry, Physiotherapy, etc</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>org_name</td>
                                            <td></td>
                                            <td>Clinic/Facility Name</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>org_phone</td>
                                            <td></td>
                                            <td>Clinic/Facility Phone Number</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>org_addr</td>
                                            <td></td>
                                            <td>Clinic/Facility Address</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>bk_date</td>
                                            <td></td>
                                            <td>Booking Date</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>bk_time</td>
                                            <td></td>
                                            <td>Booking Time</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>bk_length</td>
                                            <td></td>
                                            <td>Booking Time Length</td>
                                        </tr>
                                    </table>
                                    <div style="min-height:8px;"></div>
                                </td>
                                <td></td>
                                <td><asp:TextBox ID="txtSendDailyBookingReminderText_SMS" runat="server" TextMode="MultiLine" Width="525px" Height="200px" /><div style="min-height:6px;"></div></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    PT Booking Reminders - Email Subject
                                    <div style="min-height:4px;"></div>
                                </td>
                                <td></td>
                                <td><asp:TextBox ID="txtSendDailyBookingReminderText_EmailSubect" runat="server" Columns="65" /></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="vertical-align:top;">
                                    PT Booking Reminders - Email Text

                                    <div style="height:9px;"></div>
                                    <table>
                                        <tr>
                                            <td></td>
                                            <td colspan="3"><u>Replaceable Fields</u><div style="height:3px;"></div></td>
                                        </tr>
                                        <tr>
                                            <td style="min-width:36px;"></td>
                                            <td>pt_firstname</td>
                                            <td style="min-width:18px;"></td>
                                            <td>Patient's First Name</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>pt_fullname</td>
                                            <td></td>
                                            <td>Patient's First + Last Name</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>provider_fullname</td>
                                            <td></td>
                                            <td>Provider's First + Last Name</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>serivce_field</td>
                                            <td></td>
                                            <td>Eg. Podiatry, Physiotherapy, etc</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>org_name</td>
                                            <td></td>
                                            <td>Clinic/Facility Name</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>org_phone</td>
                                            <td></td>
                                            <td>Clinic/Facility Phone Number</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>org_addr</td>
                                            <td></td>
                                            <td>Clinic/Facility Address</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>bk_date</td>
                                            <td></td>
                                            <td>Booking Date</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>bk_time</td>
                                            <td></td>
                                            <td>Booking Time</td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>bk_length</td>
                                            <td></td>
                                            <td>Booking Time Length</td>
                                        </tr>
                                    </table>
                                    <div style="min-height:6px;"></div>
                                </td>
                                <td></td>
                                <td><FTB:FreeTextBox id="FreeTextBox2" runat="Server" Text="" Width="525px" Height="200px" /><div style="min-height:6px;"></div></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable PT Birthday SMS's</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableBirthdaySMS" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable PT Birthday Emails</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableBirthdayEmails" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable Staff Booking Reminder SMS's</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableDailyStaffBookingReminderSMS" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>Enable Staff Booking Reminder Emails</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkEnableDailyStaffBookingReminderEmails" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>What To Send If Staff Has Both SMS & Email Set</td>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlStaff_Reminders_HasBothSMSandEmail" runat="server" >
                                        <asp:ListItem Text="Send Both SMS & Email" Value="Both"></asp:ListItem>
                                        <asp:ListItem Text="Send Only SMS" Value="SMS"></asp:ListItem>
                                        <asp:ListItem Text="Send Only Email" Value="Email"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>

                            <tr>
                                <td style="height:13px" colspan="4"></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>Invoice Gap Payments</td>
                                <td></td>
                                <td><asp:CheckBox ID="chkInvoiceGapPayments" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td style="height:30px" colspan="4"></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="vertical-align:top;">Letters Email Deafult Subject Line</td>
                                <td></td>
                                <td>
                                    <asp:TextBox ID="txtLettersEmailDeafultSubjectLine" runat="server" Columns="65"/>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="vertical-align:top;">Letters Email Signature</td>
                                <td></td>
                                <td>
                                    <FTB:FreeTextBox id="FreeTextBox1" runat="Server" Text="" Width="525px" Height="200px" />
                                </td>
                            </tr>




                            <tr id="bk0" runat="server">
                                <td style="height:45px" colspan="4"></td>
                            </tr>

                            <tr id="bk1" runat="server">
                                <td colspan="3"><center><b>Booking Slot Colours</b></center</td>
                                <td></td>
                            </tr>
                            <tr id="bk2" runat="server">
                                <td style="height:20px" colspan="4"></td>
                            </tr>
                            <tr id="bk3" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Available</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_Available" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_Available" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk4" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Unavailable</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_Unavailable" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_Unavailable" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk5" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Unavailable But Addable</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_UnavailableButAddable" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_UnavailableButAddable" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk6" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Unavailable But Updatable</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_UnavailableButUpdatable" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_UnavailableButUpdatable" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk7" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Full Day Taken</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_FullDayTaken" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_FullDayTaken" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk8" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Updatable</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_Updatable" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_Updatable" runat="server"></asp:Label>
                                </td>
                            </tr>

                            <tr id="bk9" runat="server">
                                <td style="height:15px" colspan="4"></td>
                            </tr>

                            <tr id="bk10" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">CLlinic - EPC Future BK - Unconfirmed</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_CL_EPC_Future_Unconfirmed" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_CL_EPC_Future_Unconfirmed" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk11" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">CLlinic - EPC Future BK - Confirmed</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_CL_EPC_Future_Confirmed" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_CL_EPC_Future_Confirmed" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk12" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">CLlinic - Non-EPC Future BK - Unconfirmed</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_CL_NonEPC_Future_Unconfirmed" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_CL_NonEPC_Future_Unconfirmed" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk13" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">CLlinic - Non-EPC Future BK - Confirmed</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_CL_NonEPC_Future_Confirmed" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_CL_NonEPC_Future_Confirmed" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk14" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">CLlinic - EPC Past BK Completed - Has Invoice</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_CL_EPC_Past_Completed_Has_Invoice" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_CL_EPC_Past_Completed_Has_Invoice" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk15" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">CLlinic - EPC Past BK Completed - No Invoice</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_CL_EPC_Past_Completed_No_Invoice" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_CL_EPC_Past_Completed_No_Invoice" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk16" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">CLlinic - Non-EPC Past BK Completed - Has Invoice</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_CL_NonEPC_Past_Completed_Has_Invoice" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_CL_NonEPC_Past_Completed_Has_Invoice" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk17" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">CLlinic - Non-EPC Past BK Completed - No Invoice</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_CL_NonEPC_Past_Completed_No_Invoice" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_CL_NonEPC_Past_Completed_No_Invoice" runat="server"></asp:Label>
                                </td>
                            </tr>

                            <tr id="bk18" runat="server">
                                <td style="height:15px" colspan="4"></td>
                            </tr>

                            <tr id="bk19" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Aged Care - EPC - Future BK - Unconfirmed</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_AC_EPC_Future_Unconfirmed" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_AC_EPC_Future_Unconfirmed" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk20" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Aged Care - EPC Future BK - Confirmed</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_AC_EPC_Future_Confirmed" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_AC_EPC_Future_Confirmed" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk21" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Aged Care - Non-EPC Future - Unconfirmed</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_AC_NonEPC_Future_Unconfirmed" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_AC_NonEPC_Future_Unconfirmed" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk22" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Aged Care - NonEPC Future - Confirmed</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_AC_NonEPC_Future_Confirmed" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_AC_NonEPC_Future_Confirmed" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk23" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Aged Care - EPC Past BK Completed - Has Invoice</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_AC_EPC_Past_Completed_Has_Invoice" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_AC_EPC_Past_Completed_Has_Invoice" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk24" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Aged Care - EPC Past BK Completed - No Invoice</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_AC_EPC_Past_Completed_No_Invoice" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_AC_EPC_Past_Completed_No_Invoice" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk25" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Aged Care - Non-EPC Past BK Completed - Has Invoice</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_AC_NonEPC_Past_Completed_Has_Invoice" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_AC_NonEPC_Past_Completed_Has_Invoice" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk26" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">Aged Care - Non-EPC Past BK Completed - No Invoice</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_AC_NonEPC_Past_Completed_No_Invoice" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_AC_NonEPC_Past_Completed_No_Invoice" runat="server"></asp:Label>
                                </td>
                            </tr>

                            <tr id="bk27" runat="server">
                                <td style="height:15px" colspan="4"></td>
                            </tr>

                            <tr id="bk28" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">PT Logged In - Future BK</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_Future_PatientLoggedIn" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_Future_PatientLoggedIn" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="bk29" runat="server">
                                <td></td>
                                <td style="vertical-align:top;">PT Logged In - Past BK</td>
                                <td></td>
                                <td>
                                    <input class="color" id="ColorPicker_Past_PatientLoggedIn" runat="server" maxlength="6" onchange="ensure_ishex(this);" onkeyup="ensure_ishex(this);" size="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;Original Sytem Colour: <asp:Label ID="lblSysColor_Past_PatientLoggedIn" runat="server"></asp:Label>
                                </td>
                            </tr>


                            <tr>
                                <td style="height:15px" colspan="4"></td>
                            </tr>

                        </table>
                    </div>

                <div style="height:15px;"></div>

                <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClientClick="document.getElementById('lblErrorMessage').innerHTML = '';" OnClick="btnUpdate_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />

                </asp:Panel>
            </center>

        </div>
    </div>


</asp:Content>


