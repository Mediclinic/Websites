<%@ Page Title="Add Patient" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="PatientAddV2.aspx.cs" Inherits="PatientAddV2" %>
<%@ Register TagPrefix="UC" TagName="DuplicatePersonModalElementControl" Src="~/Controls/DuplicatePersonModalElementControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/duplicate_person_modal_boxV2.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/provider_nbr_check.js"></script>
    <script type="text/javascript" src="Scripts/check_duplicate_persons.js"></script>
    <script type="text/javascript">

        function duplicate_person_check(obj) {

            var firstname = document.getElementById('txtFirstname').value.trim();
            var surname = document.getElementById('txtSurname').value.trim();

            var result = ajax_duplicate_persons("patient", firstname, surname);

            if (result.length == 0) {
                alert("Error retreiving records for duplicate person check.");
            }
            else if (result == "NONE") {
                return;
            }
            else {
                var result_list = create_result_array(result);
                create_table(result_list, "ctable", "PatientDetailV2.aspx?add_to_this_org=1&type=view&id=");
                reveal_modal('modalPopupDupicatePerson');
            }
        }
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }


        function health_card_nbr_changed(txtBox) {
            if (txtBox.value.length == 10)
                document.getElementById('card_nbr_tick').style.display = "block";
            else
                document.getElementById('card_nbr_tick').style.display = "none";
        }
        function health_card_nbr_unfocused(txtBox) {

            var nbr = txtBox.value;
            if (nbr.length == 0)
                return;

            var alphanumericCount = 0;
            for (var i = 0; i < nbr.length; i++) {

                var c = nbr.charAt(i);
                if (((c >= "0") && (c <= "9")) || ((c >= "a") && (c <= "z")) || ((c >= "A") && (c <= "Z")))
                    alphanumericCount += 1;
            }

            var ddlHealthCardOrganisation = document.getElementById("ddlHealthCardOrganisation");
            var ddlHealthCardOrganisationSelected = ddlHealthCardOrganisation.options[ddlHealthCardOrganisation.selectedIndex].value;
            if (ddlHealthCardOrganisationSelected == "-1" && alphanumericCount != 10)  // medicare
                alert("Medicare card number should be 10 characters (or left blank)");
        }

        function ddl_health_card_type_changed() {

            var ddlHealthCardOrganisation = document.getElementById("ddlHealthCardOrganisation");
            var ddlHealthCardOrganisationSelected = ddlHealthCardOrganisation.options[ddlHealthCardOrganisation.selectedIndex].value;
            if (ddlHealthCardOrganisationSelected == "-1")  // medicare
            {
                document.getElementById("ddlHealthCardCardFamilyMemberNbr").style.display = "";
                document.getElementById("lblHealthCardCardNbrFamilyNbrSeperator").style.display = "";



                document.getElementById("txtHealthCardCardNbr").style.display = "none";

                document.getElementById("txtHealthCardCardNbr_Digit_1").style.display = "";
                document.getElementById("txtHealthCardCardNbr_Digit_2").style.display = "";
                document.getElementById("txtHealthCardCardNbr_Digit_3").style.display = "";
                document.getElementById("txtHealthCardCardNbr_Digit_4").style.display = "";
                document.getElementById("txtHealthCardCardNbr_Digit_5").style.display = "";
                document.getElementById("txtHealthCardCardNbr_Digit_6").style.display = "";
                document.getElementById("txtHealthCardCardNbr_Digit_7").style.display = "";
                document.getElementById("txtHealthCardCardNbr_Digit_8").style.display = "";
                document.getElementById("txtHealthCardCardNbr_Digit_9").style.display = "";
                document.getElementById("txtHealthCardCardNbr_Digit_10").style.display = "";
            }
            else if (ddlHealthCardOrganisationSelected == "-2") // dva
            {
                document.getElementById("ddlHealthCardCardFamilyMemberNbr").style.display = "none";
                document.getElementById("lblHealthCardCardNbrFamilyNbrSeperator").style.display = "none";


                document.getElementById("txtHealthCardCardNbr").style.display = "";

                document.getElementById("txtHealthCardCardNbr_Digit_1").style.display = "none";
                document.getElementById("txtHealthCardCardNbr_Digit_2").style.display = "none";
                document.getElementById("txtHealthCardCardNbr_Digit_3").style.display = "none";
                document.getElementById("txtHealthCardCardNbr_Digit_4").style.display = "none";
                document.getElementById("txtHealthCardCardNbr_Digit_5").style.display = "none";
                document.getElementById("txtHealthCardCardNbr_Digit_6").style.display = "none";
                document.getElementById("txtHealthCardCardNbr_Digit_7").style.display = "none";
                document.getElementById("txtHealthCardCardNbr_Digit_8").style.display = "none";
                document.getElementById("txtHealthCardCardNbr_Digit_9").style.display = "none";
                document.getElementById("txtHealthCardCardNbr_Digit_10").style.display = "none";
            }
        }

        function refocus_medicare_digits(obj, newID) {
            document.getElementById('card_nbr_tick').style.display = "none";
            if (obj.value.length > 0) {

                if (obj.value.length != 1 || '1234567890'.indexOf(obj.value) == -1) {
                    obj.value = '';
                    return;
                }

                var newbox = document.getElementById(newID);
                var istextbox = newbox.tagName && newbox.tagName.toLowerCase() == "input" && newbox.type.toLowerCase() == "text";
                if (!istextbox || newbox.value.length == 0)
                    newbox.focus();

                var all_digits_entered = document.getElementById('txtHealthCardCardNbr_Digit_1').value.length == 1 &&
                                         document.getElementById('txtHealthCardCardNbr_Digit_2').value.length == 1 &&
                                         document.getElementById('txtHealthCardCardNbr_Digit_3').value.length == 1 &&
                                         document.getElementById('txtHealthCardCardNbr_Digit_4').value.length == 1 &&
                                         document.getElementById('txtHealthCardCardNbr_Digit_5').value.length == 1 &&
                                         document.getElementById('txtHealthCardCardNbr_Digit_6').value.length == 1 &&
                                         document.getElementById('txtHealthCardCardNbr_Digit_7').value.length == 1 &&
                                         document.getElementById('txtHealthCardCardNbr_Digit_8').value.length == 1 &&
                                         document.getElementById('txtHealthCardCardNbr_Digit_9').value.length == 1 &&
                                         document.getElementById('txtHealthCardCardNbr_Digit_10').value.length == 1;
                document.getElementById('card_nbr_tick').style.display = all_digits_entered ? "block" : "none";
            }
        }

        function show_hide(id) {
            obj = document.getElementById(id);
            obj.style.display = (obj.style.display == "none") ? "" : "none";
        }

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1); });
            //txtbox.value = txtbox.value.charAt(0).toUpperCase() + txtbox.value.slice(1);
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

        function get_suburb() {
            isMobileDevice = document.getElementById('hiddenIsMobileDevice').value == "1";
            if (isMobileDevice)
                open_new_tab('Contact_SuburbListPopupV2.aspx');
            else
                window.showModalDialog("Contact_SuburbListPopupV2.aspx", 'Show Popup Window', "dialogHeight:810px;dialogWidth:700px;resizable:yes;center:yes;");
        }
        function clear_suburb() {
            document.getElementById('suburbID').value = '-1';
            document.getElementById('lblSuburbText').innerHTML = '--';
            //document.getElementById('btnSuburbSelectionUpdate').click();  // call button press to let the code behind use this id and update accordingly
        }
        function set_suburb(retVal) {
            var index = retVal.indexOf(":");
            document.getElementById('suburbID').value = retVal.substring(0, index);
            document.getElementById('lblSuburbText').innerHTML = retVal.substring(index + 1);
            //document.getElementById('btnSuburbSelectionUpdate').click();
        }



     </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="btnDefaultButton_NoSubmit">

        <div class="clearfix">
            <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server" Text="Add Patient" /></h3></div>
            <div class="main_content">

                <UC:DuplicatePersonModalElementControl ID="duplicatePersonModalElementControl" runat="server" />

                <asp:HiddenField ID="hiddenIsMobileDevice" runat="server" Value="0" />

                <div class="text-center">
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>

                <table id="maintable" runat="server" style="margin: 0 auto;">
                    <tr style="vertical-align:top;">
                        <td style="width:330px;">

                            <table>
                                <tr>
                                    <td></td>
                                    <td colspan="3">
                                        <b>Personal Information</b>
                                        <div style="line-height:7px;">&nbsp;</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Title</td>
                                    <td style="min-width:12px"></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlTitle" runat="server" DataTextField="descr" DataValueField="title_id" onchange='title_changed_reset_gender();' ></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td class="nowrap"><asp:RequiredFieldValidator ID="txtValidateFirstnameRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtFirstname" 
                                                ErrorMessage="Firstname is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="txtValidateFirstnameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtFirstname"
                                                ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">First Name</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtFirstname" runat="server" onkeyup="capitalize_first(this);" /></td>
                                </tr>
                                <tr>
                                    <td><asp:RegularExpressionValidator ID="txtValidateMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtMiddlename"
                                                ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Middle Name</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtMiddlename" runat="server" onkeyup="capitalize_first(this);" /></td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtSurname" 
                                                ErrorMessage="Surname is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="txtValidateSurnameNameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtSurname"
                                                ValidationExpression="^[a-zA-Z\-\.\s'\(\)]+$"
                                                ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Surname</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtSurname" runat="server" onblur="duplicate_person_check(this);" onkeyup="capitalize_first(this);"/></td>
                                </tr>
                                <tr>
                                    <td><asp:RegularExpressionValidator ID="txtValidateNicknameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtNickname"
                                                ValidationExpression="^[a-zA-Z\-\s']+$"
                                                ErrorMessage="Nickname can only be letters or hyphens."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Nickname</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtNickname" runat="server"  onkeyup="capitalize_first(this);" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Gender</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlGender" runat="server"> 
                                            <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                            <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:CustomValidator ID="ddlDOBValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDOB_Day"
                                            OnServerValidate="DOBAllOrNoneCheck"
                                            ErrorMessage="DOB must have each of day/month/year selected and be a valid date, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                    <td class="nowrap">D.O.B.</td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlDOB_Day" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDOB_Month" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDOB_Year" runat="server"></asp:DropDownList>
                                    </td>
                                 </tr>
                                <tr id="is_clinic_patient_row" runat="server" visible="False">
                                    <td></td>
                                    <td class="nowrap">Clinic Patient</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsClinicPatient" runat="server" /></td>
                                </tr>
                                <tr id="is_gp_patient_row" runat="server" visible="False">
                                    <td></td>
                                    <td class="nowrap">GP Patient</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsGPPatient" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Deceased</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsDeceased" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>Priv Health Fund</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtPrivateHealthFund" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Concession Card Nbr</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtConcessionCardNbr" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Concession Card Exp</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlConcessionCardExpiry_Month" runat="server" />
                                        <asp:DropDownList ID="ddlConcessionCardExpiry_Year" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Diabetic</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsDiabetic" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Member Diabetes Aus.</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsMemberDiabetesAustralia" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td><asp:CustomValidator ID="ddlDARevValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDARev_Day"
                                            OnServerValidate="DARevAllOrNoneCheck"
                                            ErrorMessage="DA Review Date must have each of day/month/year selected and be a valid date, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                    <td class="nowrap">DA Review Date</td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlDARev_Day" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDARev_Month" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDARev_Year" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr id="acTypeRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">PT Type</td>
                                    <td style="width:12px"></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlACInvOffering" runat="server" DataTextField="o_name" DataValueField="o_offering_id"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td><asp:RegularExpressionValidator ID="txtValidateLoginRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtLogin"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Login can only be letters and numbers."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Web Login Username</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtLogin" runat="server" TabIndex="9" autocomplete="off"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtPwd"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Password can only be letters, numbers, and underscore."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Web Login Password</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtPwd" runat="server" TabIndex="10" autocomplete="off"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Is A Company</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsCompany" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>ABN</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtABN" runat="server"></asp:TextBox></td>
                                </tr>

                                <tr>
                                    <td colspan="4"><div style="height:14px;"></div></td>
                                </tr>
                                <tr>
                                    <td colspan="4">


                                    </td>
                                </tr>

                            </table>

                        </td>
                        <td style="width:55px; min-width:25px"></td>
                        <td style="width:330px;">

                            <asp:Button ID="btnUpdateAddressStreetAndSuburb" runat="server" CssClass="hiddencol" onclick="btnUpdateAddressStreetAndSuburb_Click" />
                            <asp:Button ID="btnUpdateAddressType" runat="server" CssClass="hiddencol" onclick="btnUpdateAddressType_Click" />
                            <asp:Button ID="btnUpdatePhoneType" runat="server" CssClass="hiddencol" onclick="btnUpdatePhoneType_Click" />
                            <asp:Button ID="btnUpdateEmailType" runat="server" CssClass="hiddencol" onclick="btnUpdateEmailType_Click" />


                            <table>
                                <tr>
                                    <td colspan="3">
                                        <b>Address</b>
                                        <div style="line-height:7px;">&nbsp;</div>
                                    </td>
                                </tr>
                                <tr id="tr_ac_org" runat="server">
                                    <td>Fac/Ward/Unit</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlOrganisationAC" runat="server" DataTextField="name" DataValueField="organisation_id"> </asp:DropDownList> 
                                        <asp:RequiredFieldValidator ID="ddlOrganisationACReqiired" runat="server"
                                                InitialValue="-1"  
                                                ControlToValidate="ddlOrganisationAC" 
                                                ErrorMessage="Fac/Ward/Unit is required."
                                                Display="Dynamic" 
                                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr id="tr_ac_org_trailingspace" runat="server">
                                    <td colspan="7">&nbsp;</td>
                                </tr>

                                <tr id="tr_ac_room" runat="server">
                                    <td>AC Room</td>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtACRoom" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr id="tr_ac_room_note" runat="server">
                                    <td>Note</td>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtACRoomNote" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr id="tr_ac_room_trailingspace" runat="server">
                                    <td colspan="7">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>Type</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlAddressContactType" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id"/>
                                        <small><asp:HyperLink ID="lnkAddressUpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                                        <asp:Label ID="lblAddressContactType" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblAddressLine1Descr" runat="server">Line 1</asp:Label></td>
                                    <td style="width:12px"></td>
                                    <td>
                                        <asp:TextBox ID="txtAddressAddrLine1" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Line 2</td>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtAddressAddrLine2" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr id="streetRow_Contact" runat="server" visible="False">
                                    <td>Street</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlAddressAddressChannel" runat="server" DataTextField="ac_descr" DataValueField="ac_address_channel_id" />
                                        <small><asp:HyperLink ID="lnkAddressUpdateChannel" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                                    </td>
                                </tr>
                                <tr id="streetRow_ContactAus" runat="server" visible="False">
                                    <td>Street</td>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtStreet" runat="server"  Columns="30"></asp:TextBox>
                                        <asp:DropDownList ID="ddlAddressAddressChannelType" runat="server"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Suburb</td>
                                    <td></td>
                                    <td>
                                        <table>
                                            <tr>
                                                <td style="min-width:170px"><asp:Label  ID="lblSuburbText" runat="server" Text="--" CssClass="nowrap" /></td>
                                                <td style="min-width:15px"></td>
                                                <td class="nowrap">
                                                    <a id="lnkGetSuburb" runat="server" href="javascript:void(0)"  onclick="javascript:get_suburb(); return false;">Get Suburb</a>&nbsp;&nbsp;
                                                    <a id="lnkClearSuburb" runat="server" href="javascript:void(0)"  onclick="javascript:clear_suburb(); return false;">Clear Suburb</a>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="suburbID" runat="server" Value="-1" />
                                        <asp:Button ID="btnSuburbSelectionUpdate" runat="server" CssClass="hiddencol" Text=""  OnClick="btnSuburbSelectionUpdate_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Country</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlAddressCountry" runat="server" DataTextField="descr" DataValueField="country_id"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblAddressFreeText" runat="server">Note</asp:Label></td>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtAddressFreeText" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblIsBilling" runat="server">For Billing</asp:Label></td>
                                    <td></td>
                                    <td>
                                        <asp:CheckBox ID="chkIsBilling" runat="server" Checked="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblIsNonBilling" runat="server">For Non Billing</asp:Label></td>
                                    <td></td>
                                    <td>
                                        <asp:CheckBox ID="chkIsNonBilling" runat="server" Checked="true" />
                                    </td>
                                </tr>

                                <tr>
                                    <td><div style="height:35px;"></div></td>
                                </tr>

                                <tr>
                                    <td colspan="3">
                                        <b>Next Of Kin</b>
                                        <div style="line-height:7px;">&nbsp;</div>
                                    </td>
                                </tr>
                                <tr style="vertical-align:bottom;">
                                    <td style="vertical-align:top;" class="nowrap">Name</td>
                                    <td style="min-width:12px"></td>
                                    <td style="vertical-align:top;" class="nowrap">
                                        <asp:TextBox ID="txtNextOfKinName" runat="server" MaxLength="100" onkeyup="capitalize_first(this);"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr style="vertical-align:bottom;">
                                    <td style="vertical-align:top;" class="nowrap">Relation</td>
                                    <td style="min-width:12px"></td>
                                    <td style="vertical-align:top;" class="nowrap">
                                        <asp:TextBox ID="txtNextOfKinRelation" runat="server" MaxLength="100"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr style="vertical-align:bottom;">
                                    <td style="vertical-align:top;" class="nowrap">Ph/Addr</td>
                                    <td style="min-width:12px"></td>
                                    <td id="td_nextofkincontactinfo" runat="server" style="vertical-align:top;">
                                        <asp:TextBox ID="txtNextOfKinContactInfo" runat="server" TextMode="MultiLine" Rows="2" MaxLength="2000" ></asp:TextBox>
                                    </td>
                                </tr>
                            </table>

                        </td>
                        <td style="width:45px; min-width:15px"></td>
                        <td style="width:330px;min-width:330px">

                            <b>Conditions</b>
                            <div style="line-height:1px;">&nbsp;</div>
                                                    
                            <asp:GridView ID="GrdCondition" runat="server" 
                                    AutoGenerateColumns="False" DataKeyNames="condition_condition_id" 
                                    OnRowCancelingEdit="GrdCondition_RowCancelingEdit" 
                                    OnRowDataBound="GrdCondition_RowDataBound" 
                                    OnRowEditing="GrdCondition_RowEditing" 
                                    OnRowUpdating="GrdCondition_RowUpdating" ShowHeader="False" ShowFooter="False" 
                                    OnRowCommand="GrdCondition_RowCommand" 
                                    OnRowCreated="GrdCondition_RowCreated"
                                    AllowSorting="True" 
                                    OnSorting="GrdCondition_Sorting"
                                    ClientIDMode="Predictable"
                                    GridLines="None"
                                    CssClass="table table-grid-top-bottum-padding-normal auto_width block_center">

                                <Columns> 

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="condition_condition_id" Visible="false"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("condition_condition_id") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Checkbox" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_descr" ItemStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:CheckBox ID="chkSelect" runat="server" />
                                            &nbsp;
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_descr" ItemStyle-CssClass="text_left"> 
                                        <ItemTemplate> 

                                            <asp:Label ID="lblDescr" runat="server" Text='<%# Bind("condition_descr") %>'></asp:Label> 

                                            <div id="br_date" runat="server" style="height:5px;"></div>
                                            <asp:DropDownList ID="ddlDate_Day" runat="server"></asp:DropDownList>
                                            <asp:DropDownList ID="ddlDate_Month" runat="server"></asp:DropDownList>
                                            <asp:DropDownList ID="ddlDate_Year" runat="server"></asp:DropDownList>

                                            <div id="br_nweeksdue" runat="server" style="height:5px;"></div>
                                            <asp:Label ID="lblNextDue" runat="server" Text="Next Due: "></asp:Label>
                                            <asp:DropDownList ID="ddlNbrWeeksDue" runat="server"></asp:DropDownList> 
                                            <asp:Label ID="lblWeeksLater" runat="server" Text="Weeks Later "></asp:Label>

                                            <div id="br_text" runat="server" style="height:5px;"></div>
                                            <asp:Label ID="lblAdditionalInfo" runat="server" Text="<u>Additional Info</u>:<br />"></asp:Label>
                                            <asp:TextBox ID="txtText" runat="server" TextMode="MultiLine" Rows="2" Columns="30"></asp:TextBox>

                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                </Columns> 

                            </asp:GridView>


                        </td>

                    </tr>
                    <tr style="vertical-align:top; height:20px;">
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr style="vertical-align:top;">

                        <td colspan="5">
                            <table class="block_center">
                                <tr>

                                    <td>

                                        <table>
                                            <tr>
                                                <td colspan="7">
                                                    <b>Health Card</b>
                                                    <div style="line-height:7px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Organisation</td>
                                                <td style="width:12px"></td>
                                                <td colspan="4">
                                                    <asp:DropDownList ID="ddlHealthCardOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id" onchange="javascript:ddl_health_card_type_changed();"></asp:DropDownList>
                                                    <asp:Label ID="lblHealthCardOrganisation" runat="server"></asp:Label>
                                                </td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>Card Name</td>
                                                <td></td>
                                                <td colspan="4">
                                                    <asp:TextBox ID="txtHealthCardCardName" runat="server"></asp:TextBox><asp:Label ID="lblHealthCardCardName" runat="server" Font-Bold="True"/>
                                                    <asp:RegularExpressionValidator ID="txtValidateCardNameRegex" runat="server" CssClass="failureNotification" 
                                                        ControlToValidate="txtHealthCardCardName"
                                                        ValidationExpression="^[a-zA-Z\-\s]+$"
                                                        ErrorMessage="Health Card Name can only be letters or hyphens."
                                                        Display="Dynamic"
                                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>Card Nbr</td>
                                                <td></td>
                                                <td colspan="4">

                                                    <asp:TextBox ID="txtHealthCardCardNbr_Digit_1"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtHealthCardCardNbr_Digit_2');"></asp:TextBox><asp:TextBox ID="txtHealthCardCardNbr_Digit_2"  runat="server" MaxLength="1" Width="14" onkeyup="javascript:refocus_medicare_digits(this,'txtHealthCardCardNbr_Digit_3');"></asp:TextBox><asp:TextBox ID="txtHealthCardCardNbr_Digit_3"  runat="server" MaxLength="1" Width="14" onkeyup="javascript:refocus_medicare_digits(this,'txtHealthCardCardNbr_Digit_4');"></asp:TextBox><asp:TextBox ID="txtHealthCardCardNbr_Digit_4"  runat="server" MaxLength="1" Width="14" onkeyup="javascript:refocus_medicare_digits(this,'txtHealthCardCardNbr_Digit_5');"></asp:TextBox><asp:TextBox ID="txtHealthCardCardNbr_Digit_5"  runat="server" MaxLength="1" Width="14" onkeyup="javascript:refocus_medicare_digits(this,'txtHealthCardCardNbr_Digit_6');"></asp:TextBox><asp:TextBox ID="txtHealthCardCardNbr_Digit_6"  runat="server" MaxLength="1" Width="14" onkeyup="javascript:refocus_medicare_digits(this,'txtHealthCardCardNbr_Digit_7');"></asp:TextBox><asp:TextBox ID="txtHealthCardCardNbr_Digit_7"  runat="server" MaxLength="1" Width="14" onkeyup="javascript:refocus_medicare_digits(this,'txtHealthCardCardNbr_Digit_8');"></asp:TextBox><asp:TextBox ID="txtHealthCardCardNbr_Digit_8"  runat="server" MaxLength="1" Width="14" onkeyup="javascript:refocus_medicare_digits(this,'txtHealthCardCardNbr_Digit_9');"></asp:TextBox><asp:TextBox ID="txtHealthCardCardNbr_Digit_9"  runat="server" MaxLength="1" Width="14" onkeyup="javascript:refocus_medicare_digits(this,'txtHealthCardCardNbr_Digit_10');"></asp:TextBox><asp:TextBox ID="txtHealthCardCardNbr_Digit_10" runat="server" MaxLength="1" Width="14" onkeyup="javascript:refocus_medicare_digits(this,'ddlHealthCardCardFamilyMemberNbr');"></asp:TextBox>

                                                    <asp:TextBox ID="txtHealthCardCardNbr" runat="server" onkeyup="health_card_nbr_changed(this)"  onblur="health_card_nbr_unfocused(this);" /><asp:Label ID="lblHealthCardCardNbrFamilyNbrSeperator" runat="server"> - </asp:Label>
                                                    <asp:DropDownList ID="ddlHealthCardCardFamilyMemberNbr" runat="server"></asp:DropDownList>
                                                    <asp:Label ID="lblHealthCardCardNbr" runat="server" Font-Bold="True"/>

                                                    <asp:RegularExpressionValidator ID="txtValidateHealthCardCardNbrRegex" runat="server" CssClass="failureNotification" 
                                                        ControlToValidate="txtHealthCardCardNbr"
                                                        ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                                        ErrorMessage="Health Card Number can only be letters, numbers and hyphens"
                                                        Display="Dynamic"
                                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td style="width:15px"><img id="card_nbr_tick" src="images/tick-10.png" style="display: none;"  alt=""/></td>
                                            </tr>
                                            <tr>
                                                <td>Exp. Date</td>
                                                <td></td>
                                                <td colspan="4">
                                                    <asp:DropDownList ID="ddlHealthCardCardExpiry_Month" runat="server" />
                                                    <asp:DropDownList ID="ddlHealthCardCardExpiry_Year" runat="server" />
                                                </td>
                                                <td></td>
                                            </tr>
                                        </table>

                                    </td>
                                    <td style="width:55px; min-width:25px"></td>
                                    <td>

                                        <table>
                                            <tr>
                                                <td colspan="8">
                                                    <b>Phone/Other Contact Information</b>
                                                    <div style="line-height:7px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap">
                                                    <asp:DropDownList ID="ddlPhoneNumber1" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id" TabIndex="-1"/>
                                                    <small><asp:HyperLink ID="lnkPhone1UpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                                                </td>
                                                <td style="width:12px;"></td>
                                                <td><asp:TextBox ID="txtPhoneNumber1" runat="server" Columns="25"/></td>
                                                <td style="width:5px;"></td>
                                                <td><asp:Label ID="lblPhoneNumber1FreeText" runat="server">Note</asp:Label></td>
                                                <td style="width:2px;"></td>
                                                <td><asp:TextBox ID="txtPhoneNumber1FreeText" runat="server" Columns="25"/></td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap">
                                                    <asp:DropDownList ID="ddlPhoneNumber2" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id" TabIndex="-1"/>
                                                    <small><asp:HyperLink ID="lnkPhone2UpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                                                </td>
                                                <td></td>
                                                <td><asp:TextBox ID="txtPhoneNumber2" runat="server" Columns="25"/></td>
                                                <td></td>
                                                <td><asp:Label ID="lblPhoneNumber2FreeText" runat="server">Note</asp:Label></td>
                                                <td style="width:2px;"></td>
                                                <td><asp:TextBox ID="txtPhoneNumber2FreeText" runat="server" Columns="25"/></td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap">
                                                    <asp:DropDownList ID="ddlPhoneNumber3" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id" TabIndex="-1"/>
                                                    <small><asp:HyperLink ID="lnkPhone3UpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                                                </td>
                                                <td></td>
                                                <td><asp:TextBox ID="txtPhoneNumber3" runat="server" Columns="25"/></td>
                                                <td></td>
                                                <td><asp:Label ID="lblPhoneNumber3FreeText" runat="server">Note</asp:Label></td>
                                                <td style="width:2px;"></td>
                                                <td><asp:TextBox ID="txtPhoneNumber3FreeText" runat="server" Columns="25"/></td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap">
                                                    <asp:DropDownList ID="ddlEmailContactType" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id" TabIndex="-1"/>
                                                    <small><asp:HyperLink ID="lnkEmailUpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                                                </td>
                                                <td></td>
                                                <td><asp:TextBox ID="txtEmailAddrLine1" runat="server" Columns="25"></asp:TextBox></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                        </table>

                                    </td>

                                </tr>
                            </table>

                        </td>
                    </tr>
                </table>


                <div style="width:800px; height:1px; margin:35px auto; background:#999;"></div>


                <div class="text-center">

                    <asp:Button ID="btnSubmitAddAndGoToViewScreen" runat="server" Text="Add" OnCommand="btnSubmitAdd_Click" CommandName="AddAndGoToViewScreen" CausesValidation="True" ValidationGroup="ValidationSummary" />
                    <span style="min-width:5px;display:inline-block;"></span>
                    <asp:Button ID="btnSubmitAddAndGoToBookingScreen" runat="server" Text="Add & Go Make Booking" OnCommand="btnSubmitAdd_Click" CommandName="AddAndGoToBookingScreen" CausesValidation="True" ValidationGroup="ValidationSummary" />
                    <span id="spnSpaceBeforeSubmitAddAndGoToHealthCardScreen" runat="server" style="min-width:5px;display:inline-block;"></span>
                    <asp:Button ID="btnSubmitAddAndGoToHealthCardScreen" runat="server" Text="Add & Go Add Referral" OnCommand="btnSubmitAdd_Click" CommandName="AddAndGoToHealthCardScreen" CausesValidation="True" ValidationGroup="ValidationSummary" />
                    <span style="min-width:5px;display:inline-block;"></span>
                    <asp:Button ID="btnSubmitAddAndAddAonther" runat="server" Text="Add & Add Aonther" OnCommand="btnSubmitAdd_Click" CommandName="AddAndAddAonther" CausesValidation="True" ValidationGroup="ValidationSummary" />

                    <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />

                </div>


                <div style="height:15px;"></div>

                <div id="autodivheight" class="divautoheight" style="height:500px;">
                </div>
  
            </div>
        </div>

    </asp:Panel>

</asp:Content>



