<%@ Page Title="Staff Detail" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StaffDetailV2.aspx.cs" Inherits="StaffDetailV2" %>
<%@ Register TagPrefix="UC" TagName="DuplicatePersonModalElementControl" Src="~/Controls/DuplicatePersonModalElementControlV2.ascx" %>
<%@ Register TagPrefix="UC" TagName="AddressControl" Src="~/Controls/AddressControl.ascx" %>
<%@ Register TagPrefix="UC" TagName="AddressAusControl" Src="~/Controls/AddressAusControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="Scripts/check_future_bookings_provider.js" type="text/javascript"></script>
    <script src="Scripts/check_future_bookings.js" type="text/javascript"></script>

    <link href="Styles/duplicate_person_modal_boxV2.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/provider_nbr_check.js"></script>
    <script type="text/javascript" src="Scripts/check_duplicate_persons.js"></script>
    <script type="text/javascript">

        function create_username() {

            if (document.getElementById('txtLogin').value.length > 0 ||
                document.getElementById('txtPwd').value.length > 0)
                return; // dont update if already set

            var firstname = document.getElementById('txtFirstname').value.trim();
            var surname = document.getElementById('txtSurname').value.trim();
            document.getElementById('txtLogin').value = firstname.toLowerCase() + surname.toLowerCase();
            document.getElementById('txtPwd').value = firstname.toLowerCase() + surname.toLowerCase();
        }
        function duplicate_person_check(obj) {

            var firstname = document.getElementById('txtFirstname').value.trim();
            var surname = document.getElementById('txtSurname').value.trim();

            var result = ajax_duplicate_persons("staff", firstname, surname);

            if (result.length == 0) {
                alert("Error retreiving records for duplicate person check.");
            }
            else if (result == "NONE") {
                return;
            }
            else {
                var result_list = create_result_array(result);
                create_table(result_list, "ctable", "StaffDetailV2.aspx?type=view&id=");
                reveal_modal('modalPopupDupicatePerson');
            }
        }
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.charAt(0).toUpperCase() + txtbox.value.slice(1);
        }

        function title_changed_reset_gender() {
            var selValue = ddlTitle.options[ddlTitle.selectedIndex].value
            if (selValue == 6 || selValue == 265 || selValue == 266)
                setSelectedValue(document.getElementById("ddlGender"), "M");
            if (selValue == 7 || selValue == 26)
                setSelectedValue(document.getElementById("ddlGender"), "F");
        }
        function setSelectedValue(selectObj, valueToSet) {
            for (var i = 0; i < selectObj.options.length; i++) {
                if (selectObj.options[i].value == valueToSet) {
                    selectObj.options[i].selected = true;
                    return;
                }
            }
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

        <div class="clearfix">
            <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server" Text="Staff Information" /></h3></div>
            <div class="main_content">

                <UC:DuplicatePersonModalElementControl ID="duplicatePersonModalElementControl" runat="server" />

                <div class="text-center">
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>

                <table id="maintable" runat="server" style="width:80%; margin: 0 auto;">
                    <tr style="vertical-align:top;">

                        <td style="width:500px;">

                            <table class="detailtable_staff">
                                <tr id="idRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">ID</td>
                                    <td></td>
                                    <td><asp:Label ID="lblId" runat="server"></asp:Label></td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap"></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Title</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlTitle" runat="server" DataTextField="descr" DataValueField="title_id" onchange='title_changed_reset_gender();' TabIndex="1" ></asp:DropDownList><asp:Label ID="lblTitle" runat="server" Font-Bold="True"/>
                                    </td>
                                    <td></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateTFNRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtTFN"
                                                ValidationExpression="^[0-9\-]+$"
                                                ErrorMessage="TFN can only be numbers and hyphens."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">TFN</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtTFN" runat="server" TabIndex="19"></asp:TextBox><asp:Label ID="lblTFN" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidateFirstnameRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtFirstname" 
                                            ErrorMessage="Firstname is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateFirstnameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtFirstname"
                                            ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                            ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">First Name</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtFirstname" runat="server" onkeyup="capitalize_first(this);" TabIndex="2"></asp:TextBox><asp:Label ID="lblFirstname" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtProviderNumber"
                                            ValidationExpression="^[a-zA-Z0-9]+$"
                                            ErrorMessage="ProviderNumber can only be letters, numbers, and underscore."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Aged Care Provider No.</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtProviderNumber" runat="server" onblur="provider_check(this);" TabIndex="21"></asp:TextBox><asp:Label ID="lblProviderNumber" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:RegularExpressionValidator ID="txtValidateMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtMiddlename"
                                            ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                            ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Middle Name</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtMiddlename" runat="server" onkeyup="capitalize_first(this);" TabIndex="3"></asp:TextBox><asp:Label ID="lblMiddlename" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap">Commission Based</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkIsCommission" runat="server" TabIndex="22" /><asp:Label ID="lblIsCommission" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtSurname" 
                                            ErrorMessage="Surname is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateSurnameNameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtSurname"
                                            ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                            ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Surname</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtSurname" runat="server" onblur="create_username();duplicate_person_check(this);" onkeyup="capitalize_first(this);"  TabIndex="4"/><asp:Label ID="lblSurname" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                    <td><asp:RequiredFieldValidator ID="txtValidateCommissionPercentRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtCommissionPercent" 
                                            ErrorMessage="Commission percent is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateCommissionPercentRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtCommissionPercent"
                                            ValidationExpression="^\d+(\.\d{1,2})?$"
                                            ErrorMessage="Commission percent can only be numbers and option decimal place with 1 or 2 digits following."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Commission Percent</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtCommissionPercent" runat="server" TabIndex="23"></asp:TextBox><asp:Label ID="lblCommissionPercent" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Gender</td>
                                    <td></td>
                                    <td><asp:DropDownList ID="ddlGender" runat="server"  TabIndex="5"> 
                                            <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                            <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblGender" runat="server" Font-Bold="True"/>
                                    </td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap"></td>
                                    <td></td>
                                    <td></td>                                </tr>
                                <tr>
                                    <td>
                                        <asp:CustomValidator ID="ddlDOBValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDOB_Day"
                                            OnServerValidate="DOBAllOrNoneCheck"
                                            ErrorMessage="DOB must have each of day/month/year selected, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                    <td class="nowrap">Date of Birth</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlDOB_Day" runat="server"  TabIndex="6"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDOB_Month" runat="server"  TabIndex="7"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDOB_Year" runat="server"  TabIndex="8"></asp:DropDownList>
                                        <asp:Label ID="lblDOB" runat="server" Font-Bold="True"/>
                                    </td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap">SMS Bookings</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkSMSBKs" runat="server" Checked="true" TabIndex="25" /><asp:Label ID="lblSMSBKs" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidateLoginRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtLogin" 
                                            ErrorMessage="Login is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateLoginRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtLogin"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Login can only be letters and numbers."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Username</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtLogin" runat="server" TabIndex="9" autocomplete="off"></asp:TextBox><asp:Label ID="lblLogin" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap">Email Bookings</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkEmailBKs" runat="server" Checked="true" TabIndex="26" /><asp:Label ID="lblEmailBKs" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidatePwdRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtPwd" 
                                            ErrorMessage="Password is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidatePwdRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtPwd"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Password can only be letters, numbers, and underscore."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Password</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtPwd" runat="server" TextMode="Password" TabIndex="10" autocomplete="off"></asp:TextBox><asp:Label ID="lblPwd" runat="server" Font-Bold="True" Text="●●●●●●●●●"/></td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap"></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Role</td>
                                    <td></td>
                                    <td><asp:DropDownList ID="ddlField" runat="server" DataTextField="descr" DataValueField="field_id"  TabIndex="11"></asp:DropDownList><asp:Label ID="lblField" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap">Provider</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkIsProvider" runat="server" TabIndex="27" /><asp:Label ID="lblIsProvider" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Contractor</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkContractor" runat="server" TabIndex="12" /><asp:Label ID="lblContractor" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap">Principal</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkIsPrincipal" runat="server" TabIndex="28" /><asp:Label ID="lblIsPrincipal" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap"></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap">Admin</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkIsAdmin" runat="server" TabIndex="29" /><asp:Label ID="lblIsAdmin" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:CustomValidator ID="ddlStartDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlStartDate_Day"
                                            OnServerValidate="StartDateAllOrNoneCheck"
                                            ErrorMessage="Start Date must have each of day/month/year selected, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                    <td class="nowrap">Start Date</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlStartDate_Day" runat="server" TabIndex="13"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlStartDate_Month" runat="server" TabIndex="14"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlStartDate_Year" runat="server" TabIndex="15"></asp:DropDownList>
                                        <asp:Label ID="lblStartDate" runat="server" Font-Bold="True"/>
                                    </td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblMasterAdminText" runat="server">Master Admin</asp:Label></td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkIsMasterAdmin" runat="server" TabIndex="30" /><asp:Label ID="lblIsMasterAdmin" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:CustomValidator ID="ddlEndDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlEndDate_Day"
                                            OnServerValidate="EndDateAllOrNoneCheck"
                                            ErrorMessage="End Date must have each of day/month/year selected, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                    <td class="nowrap">End Date</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlEndDate_Day" runat="server" TabIndex="16"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlEndDate_Month" runat="server" TabIndex="17"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlEndDate_Year" runat="server" TabIndex="18"></asp:DropDownList>
                                        <asp:Label ID="lblEndDate" runat="server" Font-Bold="True"/>
                                    </td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblStakeholderText" runat="server">Stakeholder</asp:Label></td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkIsStakeholder" runat="server" TabIndex="31" /><asp:Label ID="lblIsStakeholder" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblAddedByText" runat="server">Added By</asp:Label></td>
                                    <td></td>
                                    <td><asp:Label ID="lblAddedBy" runat="server"></asp:Label></td>
                                    <td ></td>
                                    <td></td>
                                    <td class="nowrap"></td>
                                    <td></td>
                                    <td></td>
                                </tr> 
                                <tr>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblStaffDateAddedText" runat="server">Date Added</asp:Label></td>
                                    <td></td>
                                    <td><asp:Label ID="lblStaffDateAdded" runat="server"></asp:Label></td>
                                    <td></td>
                                    <td></td>
                                    <td class="nowrap">Status</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlStatus" runat="server">
                                            <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                                            <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblIsFired" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="9">
                                        <div style="height:25px;"></div>
                                        <center>
                                        Comments<br />
                                        <asp:TextBox ID="txtComments" runat="server" TextMode="multiline" rows="2" Columns="50" TabIndex="32"  />
                                        </center>

                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="9">
                                        <div style="height:15px;"></div>
                                        <center>
                                            <asp:Button ID="btnSubmit" runat="server" Text="Button" TabIndex="33" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" TabIndex="34" onclick="btnCancel_Click" Visible="False" />
                                        </center>
                                    </td>
                                </tr>

                            </table>

                        </td>
                        <td id="existingStaffInfoSpace" runat="server" style="width:5%;"><div class="staff_detail_vertical_divider"></div></td>
                        <td id="existingStaffInfo" runat="server" style="width:300px;">

                            <table class="block_center">
                                <tr style="vertical-align:top;">
                                    <td class="nowrap"><b>Clinics Registered To:</b>  &nbsp;&nbsp;&nbsp;&nbsp;(<asp:HyperLink ID="lnkThisStaff" runat="server" NavigateUrl="~/StaffListV2.aspx?id=">Edit</asp:HyperLink>)</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Panel ID="pnlOrgsList" runat="server" ScrollBars="Auto" style="max-height:140px;" >
                                            <asp:BulletedList ID="lstClinics" runat="server"></asp:BulletedList>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">
                                        <div style="height:35px;"></div>
                                        <b>Sites Registered To:</b></td>
                                </tr>
                                <tr style="height:3px">
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>

                                        <asp:Repeater id="lstSites" runat="server">
                                            <HeaderTemplate>
                                                <table>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="nowrap"><asp:Label ID="lblSiteName" runat="server" Text='<%# Eval("name") %>'></asp:Label></td>
                                                    <td style="min-width:12px"></td>
                                                    <td><asp:ImageButton ID="imgStatus" runat="server" ImageUrl='<%# Convert.ToBoolean(Eval("has_access")) ? "~/images/tick-12.png" : "~/images/Delete-icon-12.png" %>'></asp:ImageButton></td>
                                                    <td style="min-width:12px"></td>
                                                    <td><asp:LinkButton ID="btnToggleSiteRestriction" runat="server" OnCommand="btnToggleSiteRestriction_Click" CommandName='<%#  Convert.ToBoolean(Eval("has_access")) ? "TurnOff" : "TurnOn" %>' CommandArgument='<%# Eval("site_id") %>' Text="Toggle"></asp:LinkButton></td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>

                                        <asp:Label ID="lblNoSitesExist" runat="server" Text="No sites exist yet. Go to the Sites table to create a site."></asp:Label>
                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="height:25px;"></div>
                                        <UC:AddressControl ID="addressControl" runat="server" Visible="False" />
                                        <UC:AddressAusControl ID="addressAusControl" runat="server" Visible="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="height:25px;"></div>
                                        <asp:HyperLink ID="lnkBookingList" runat="server" NavigateUrl="~/BookingsListV2.aspx?id=">Booking List</asp:HyperLink>
                                        <br />
                                        <asp:HyperLink ID="lnkUnavailabilities" runat="server">Maintain Unavailabilities</asp:HyperLink>
                                        <br />
                                        <asp:HyperLink ID="lnkStaffOfferings" runat="server" NavigateUrl="~/StaffOfferingsListV2.aspx?id=">Set Comissions/Fixed Rates</asp:HyperLink>
                                        <br />
                                        <span class="nowrap">Bookings yet to generate system letters: <asp:Label ID="lblBookingsYetToGenerateSystemLetters" runat="server" /><asp:Button ID="btnGenerateSystemLetters" runat="server" OnClick="btnGenerateSystemLetters_Click" Text="Generate" /></span>
                                    </td>
                                </tr>

                            </table>

                        </td>

                    </tr>
                </table>

                <div id="autodivheight" class="divautoheight" style="height:500px;">
                </div>
  
            </div>
        </div>

</asp:Content>



