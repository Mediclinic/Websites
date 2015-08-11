<%@ Page Title="Patient Details" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="PatientDetailV3.aspx.cs" Inherits="PatientDetailV3" %>
<%@ Register TagPrefix="UC" TagName="DuplicatePersonModalElementControl" Src="~/Controls/DuplicatePersonModalElementControlV2.ascx" %>
<%@ Register TagPrefix="UC" TagName="AddressControl" Src="~/Controls/AddressControl.ascx" %>
<%@ Register TagPrefix="UC" TagName="AddressAusControl" Src="~/Controls/AddressAusControlV2.ascx" %>
<%@ Register TagPrefix="UC" TagName="PatientReferrerControl" Src="~/Controls/PatientReferrerControlV2.ascx" %>
<%@ Register TagPrefix="UC" TagName="HealthCardInfoControl" Src="~/Controls/HealthCardInfoControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/duplicate_person_modal_boxV2.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/update_epcV2.js"></script>
    <script type="text/javascript" src="Scripts/provider_nbr_check.js"></script>
    <script type="text/javascript" src="Scripts/check_duplicate_persons.js"></script>

    <!--// plugin-specific resources //-->
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="Scripts/jquery.form.js" type="text/javascript"></script>
    <script src="Scripts/jquery.MetaData.js" type="text/javascript"></script>
    <script src="Scripts/jquery.MultiFile.js" type="text/javascript"></script>
    <script src="Scripts/jquery.blockUI.js" type="text/javascript"></script>

    <script type="text/javascript">


        // to make the flashing text.  every 1000ms : turn on for 700ms - then turn off 
        window.addEventListener("load", function () {
            setInterval(function () {

                document.getElementById('lblFlashingText').style.visibility = 'visible';

                setTimeout(function () {
                    document.getElementById('lblFlashingText').style.visibility = 'hidden';
                }, 700);

            }, 1000);
        }, false);

        addLoadEvent(function () {
            resize_booking_list_panel_width();
        });
        
        
        // ---------------------------------------------------------------------------


        function resize_booking_list_panel_width() {
            var panel = document.getElementById('pnlBookingsList');
            if (panel != null) {
                var width_difference = parseInt(panel.offsetWidth) - parseInt(panel.clientWidth);
                panel.style.width += String(parseInt(panel.offsetWidth) + width_difference) + "px";
            }
        }


        // ---------------------------------------------------------------------------

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

        function SetCursorToTextEnd(textControlID) {
            var text = document.getElementById(textControlID);
            if (text != null) {
                text.focus(); //sets focus to element
                if (text.value.length > 0) {
                    var val = text.value; //store the value of the element
                    text.value = ''; //clear the value of the element
                    text.value = val; //set that value back.  
                }
            }
        }


        function show_hide(id, show) {
            obj = document.getElementById(id);
            obj.style.display = show ? "" : "none";
        }
        function show_tab(tabName) {

            if (document.getElementById('tab1') != null) show_hide('tab1', false);
            if (document.getElementById('tab2') != null) show_hide('tab2', false);
            if (document.getElementById('tab3') != null) show_hide('tab3', false);
            if (document.getElementById('tab4') != null) show_hide('tab4', false);
            if (document.getElementById('tab5') != null) show_hide('tab5', false);
            if (document.getElementById('tab6') != null) show_hide('tab6', false);

            if (document.getElementById(tabName) != null) show_hide(tabName, true);

            /*
            document.getElementById('link_tab1').style.fontWeight = 'normal';
            document.getElementById('link_tab2').style.fontWeight = 'normal';
            document.getElementById('link_tab3').style.fontWeight = 'normal';
            document.getElementById('link_tab4').style.fontWeight = 'normal';
            document.getElementById('link_tab5').style.fontWeight = 'normal';
            document.getElementById('link_tab6').style.fontWeight = 'normal';
            document.getElementById('link_' + tabName).style.fontWeight = 'bold';
            */

            if (document.getElementById('link_tab1') != null) document.getElementById('link_tab1').style.textDecoration = "";
            if (document.getElementById('link_tab2') != null) document.getElementById('link_tab2').style.textDecoration = "";
            if (document.getElementById('link_tab3') != null) document.getElementById('link_tab3').style.textDecoration = "";
            if (document.getElementById('link_tab4') != null) document.getElementById('link_tab4').style.textDecoration = "";
            if (document.getElementById('link_tab5') != null) document.getElementById('link_tab5').style.textDecoration = "";
            if (document.getElementById('link_tab6') != null) document.getElementById('link_tab6').style.textDecoration = "";
            if (document.getElementById('link_' + tabName) != null) document.getElementById('link_' + tabName).style.textDecoration = "underline";


            document.getElementById("hiddenFieldSelectedTab").value = tabName;
        }


        // ---------------------------------------------------------------------------


        function get_register_referrer_forlist(referral_id) {
            document.getElementById('hiddenField_ReferralID_ToUpdateInList').value = referral_id;              // set value so can get from code behind

            var isMobileDevice = document.getElementById('hiddenIsMobileDevice').value == "1";
            if (isMobileDevice)
                open_new_tab('ReferrerListPopupV2.aspx');
            else
                window.showModalDialog("ReferrerListPopupV2.aspx", 'Show Popup Window', "dialogHeight:850px;dialogWidth:1150px;resizable:yes;center:yes;");
        }
        function set_register_referrer(retVal) {
            var index = retVal.indexOf(":");
            document.getElementById('hiddenField_RegRefID_ToUpdateInList').value = retVal.substring(0, index); // set value so can get from code behind
            document.getElementById('btnRegisterReferrer_ToUpdateInList').click();  // call button press to let the code behind use this id and update accordingly
        }


        // ---------------------------------------------------------------------------


        function ddl_health_card_type_changed(org_id, seperator_id, family_member_id) {

            var ddlHealthCardOrganisation = document.getElementById(org_id);
            var ddlHealthCardOrganisationSelected = ddlHealthCardOrganisation.options[ddlHealthCardOrganisation.selectedIndex].value;

            var lblHealthCardCardNbrFamilyNbrSeperator = document.getElementById(seperator_id);

            var ddlHealthCardCardFamilyMemberNbr = document.getElementById(family_member_id);
            var ddlHealthCardCardFamilyMemberNbrSelected = ddlHealthCardCardFamilyMemberNbr.options[ddlHealthCardCardFamilyMemberNbr.selectedIndex].value;

            if (ddlHealthCardOrganisationSelected == "-1")  // medicare
            {
                lblHealthCardCardNbrFamilyNbrSeperator.style.display = "";
                ddlHealthCardCardFamilyMemberNbr.style.display = "";
            }
            else if (ddlHealthCardOrganisationSelected == "-2") // dva
            {

                lblHealthCardCardNbrFamilyNbrSeperator.style.display = "none";
                ddlHealthCardCardFamilyMemberNbr.style.display = "none";
            }
        }

        // ---------------------------------------------------------------------------

        function show_hide_by_class(className, show) {
            elements = document.getElementsByClassName(className);
            for (var i = 0; i < elements.length; i++)
                elements[i].style.display = show ? "" : "none";
        }

        // ---------------------------------------------------------------------------

     </script>

</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <asp:HiddenField ID="hiddenFieldSelectedTab" runat="server" />

    <asp:HiddenField ID="hiddenField_RegRefID_ToUpdateInList" runat="server" />
    <asp:HiddenField ID="hiddenField_ReferralID_ToUpdateInList" runat="server" />
    <asp:Button ID="btnRegisterReferrer_ToUpdateInList"   runat="server" CssClass="hiddencol" Text="" onclick="btnRegisterReferrer_ToUpdateInList_Click" />

    <asp:HiddenField ID="hiddenIsMobileDevice" runat="server" Value="0" />

    <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="btnDefaultButton_NoSubmit">

        <div class="clearfix">
            <div class="page_title">
                <h3>

                    <table style="max-width:92%" class="block_center">
                        <tr>
                            <td class="nowrap">
                                <asp:Label ID="lblHeading" runat="server" Text="Add Patient" />
                                <span style="font-size:70%;color:#aaaaaa;"> &nbsp;&nbsp;&nbsp;<asp:Label ID="lblPatientID" runat="server"></asp:Label></span> 
                            </td>
                            <td>
                                <span style="font-size:100%;"> &nbsp;&nbsp;&nbsp            
                                    <b><asp:Label ID="lblFlashingText" runat="server" ForeColor="Red" /></b>
                                </span>                            
                            </td>
                            <td style="vertical-align:middle;" class="nowrap">
                                &nbsp;
                                <asp:ImageButton ID="lnkFlashingText" runat="server" AlternateText="Edit Flashing Text" ToolTip="Edit Flashing Text" />
                                <asp:Button ID="btnUpdateFlashingTextIcon" runat="server" CssClass="hiddencol" onclick="btnUpdateFlashingTextIcon_Click" />
                            </td>
                        </tr>
                    </table>

                </h3>
            </div>
            <div class="main_content">

                <UC:DuplicatePersonModalElementControl ID="duplicatePersonModalElementControl" runat="server" />

                <div class="text-center">
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>


                <div class="text-center">
                    <asp:Label ID="lblInvoiceOwingMessage" runat="server"></asp:Label>
                </div>



                <table id="maintable" runat="server" style="margin: 0 auto; width:800px;" border="0">
                    <tr style="vertical-align:top;">
                        <td>

                            <table border="0">
                                <tr>
                                    <td colspan="4">
                                        <b>Personal Information</b>
                                        <div style="line-height:7px;">&nbsp;</div>
                                    </td>
                                </tr>
                                <tr id="idRow" runat="server">
                                    <td class="nowrap">ID</td>
                                    <td style="min-width:12px"></td>
                                    <td class="nowrap"><asp:Label ID="lblId" runat="server"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Title</td>
                                    <td style="min-width:12px"></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlTitle" runat="server" DataTextField="descr" DataValueField="title_id" onchange='title_changed_reset_gender();' ></asp:DropDownList><asp:Label ID="lblTitle" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">First Name</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtFirstname" runat="server" onkeyup="capitalize_first(this);" /><asp:Label ID="lblFirstname" runat="server" Font-Bold="True" /></td>
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
                                </tr>
                                <tr>
                                    <td class="nowrap">Middle Name</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtMiddlename" runat="server" onkeyup="capitalize_first(this);" /><asp:Label ID="lblMiddlename" runat="server" Font-Bold="True"></asp:Label></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtMiddlename"
                                                ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Surname</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtSurname" runat="server" onkeyup="capitalize_first(this);"/><asp:Label ID="lblSurname" runat="server" Font-Bold="True"></asp:Label></td>
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
                                </tr>
                                <tr>
                                    <td class="nowrap">Nickname</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtNickname" runat="server"  onkeyup="capitalize_first(this);" /><asp:Label ID="lblNickname" runat="server" Font-Bold="True"></asp:Label></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateNicknameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtNickname"
                                                ValidationExpression="^[a-zA-Z\-\s']+$"
                                                ErrorMessage="Nickname can only be letters or hyphens."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Gender</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlGender" runat="server"> 
                                            <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                            <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblGender" runat="server" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">D.O.B.</td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlDOB_Day" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDOB_Month" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDOB_Year" runat="server"></asp:DropDownList>
                                        <asp:Label ID="lblDOB" runat="server" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td><asp:CustomValidator ID="ddlDOBValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDOB_Day"
                                            OnServerValidate="DOBAllOrNoneCheck"
                                            ErrorMessage="DOB must have each of day/month/year selected, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                 </tr>
                                <tr id="is_clinic_patient_row" runat="server" visible="False">
                                    <td class="nowrap">Clinic Patient</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsClinicPatient" runat="server" /><asp:Label ID="lblIsClinicPatient" runat="server" Font-Bold="True"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr id="is_gp_patient_row" runat="server" visible="False">
                                    <td class="nowrap">GP Patient</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsGPPatient" runat="server" /><asp:Label ID="lblIsGPPatient" runat="server" Font-Bold="True"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Deceased</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsDeceased" runat="server" /><asp:Label ID="lblIsDeceased" runat="server" Font-Bold="True"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Priv Health Fund</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtPrivateHealthFund" runat="server"></asp:TextBox><asp:Label ID="lblPrivateHealthFund" runat="server" Font-Bold="True"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Concession Card Nbr</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtConcessionCardNbr" runat="server"></asp:TextBox><asp:Label ID="lblConcessionCardNbr" runat="server" Font-Bold="True"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Concession Card Exp</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlConcessionCardExpiry_Month" runat="server" />
                                        <asp:DropDownList ID="ddlConcessionCardExpiry_Year" runat="server" />
                                        <asp:Label ID="lblConcessionCardExpiry" runat="server" Font-Bold="True" />
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap"><asp:Label ID="lblIsDiabeticText" runat="server">Diabetic</asp:Label></td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsDiabetic" runat="server" /><asp:Label ID="lblIsDiabetic" runat="server" Font-Bold="True"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Member Diabetes Aus.</td>
                                    <td></td>
                                    <td class="nowrap"><asp:CheckBox ID="chkIsMemberDiabetesAustralia" runat="server" /><asp:Label ID="lblIsMemberDiabetesAustralia" runat="server" Font-Bold="True"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">DA Review Date</td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlDARev_Day" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDARev_Month" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDARev_Year" runat="server"></asp:DropDownList>
                                        <asp:Label ID="lblDARev" runat="server" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td><asp:CustomValidator ID="ddlDARevValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDARev_Day"
                                            OnServerValidate="DARevAllOrNoneCheck"
                                            ErrorMessage="DA Review Date must have each of day/month/year selected and be a valid date, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr id="acTypeRow" runat="server">
                                    <td class="nowrap">PT Type</td>
                                    <td style="width:12px"></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlACInvOffering" runat="server" DataTextField="o_name" DataValueField="o_offering_id"></asp:DropDownList><asp:Label ID="lblACInvOffering" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Web Login Username</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtLogin" runat="server" TabIndex="9"></asp:TextBox><asp:Label ID="lblLogin" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateLoginRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtLogin"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Login can only be letters and numbers."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Web Login Password</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtPwd" runat="server" TabIndex="10"></asp:TextBox><asp:Label ID="lblPwd" runat="server" Font-Bold="True"/></td>
                                    <td>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtPwd"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Password can only be letters, numbers, and underscore."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr id="addbyRow" runat="server">
                                    <td class="nowrap">Added By</td>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblAddedBy" runat="server"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr id="patientDateAddedRow" runat="server">
                                    <td class="nowrap">Date Added</td>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblPatientDateAdded" runat="server"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr id="changeHistoryLinknRow" runat="server">
                                    <td>View Edit History</td>
                                    <td></td>
                                    <td><asp:LinkButton ID="btnHistory" runat="server" Text="View" /></td>
                                    <td></td>
                                </tr>
                                <tr  id="deleteUndeletePatientRow" runat="server">
                                    <td><asp:Label ID="lblDeleteUndeletePatientText" runat="server" Text="Status"/></td>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblPatientStatus" runat="server" Text ="Active"></asp:Label>
                                        &nbsp;&nbsp;
                                        <asp:linkbutton ID="btnDeleteUndeletePatient" runat="server" Text="Archive Patient" OnCommand="btnDeleteUndeletePatient_Command" />
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="4"><div style="height:14px;"></div></td>
                                </tr>

                                <tr style="vertical-align:bottom;">
                                    <td style="vertical-align:top;" class="nowrap">Next Of Kin - Name</td>
                                    <td style="min-width:12px"></td>
                                    <td style="vertical-align:top;" class="nowrap">
                                        <asp:TextBox ID="txtNextOfKinName" runat="server" MaxLength="100" style="min-width:100%;"></asp:TextBox>
                                        <asp:Label ID="lblNextOfKinName" runat="server" Font-Bold="True" />
                                    </td>
                                    <td></td>
                                </tr>
                                <tr style="vertical-align:bottom;">
                                    <td style="vertical-align:top;" class="nowrap">Next Of Kin - Relation</td>
                                    <td style="min-width:12px"></td>
                                    <td style="vertical-align:top;" class="nowrap">
                                        <asp:TextBox ID="txtNextOfKinRelation" runat="server" MaxLength="100" style="min-width:100%;"></asp:TextBox>
                                        <asp:Label ID="lblNextOfKinRelation" runat="server" Font-Bold="True" />
                                    </td>
                                    <td></td>
                                </tr>
                                <tr style="vertical-align:bottom;">
                                    <td style="vertical-align:top;" class="nowrap">Next Of Kin - Ph/Addr</td>
                                    <td style="min-width:12px"></td>
                                    <td id="td_nextofkincontactinfo" runat="server" style="vertical-align:top;">
                                        <asp:TextBox ID="txtNextOfKinContactInfo" runat="server" TextMode="MultiLine" Rows="2" MaxLength="2000" style="min-width:100%;"></asp:TextBox>
                                        <asp:Label ID="lblNextOfKinContactInfo" runat="server" Font-Bold="True" />
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="4"><div style="height:14px;"></div></td>
                                </tr>
                                <tr id="addressListRow" runat="server">
                                    <td colspan="3">
                                        <UC:AddressControl ID="addressControl" runat="server" ViewStateMode="Enabled" Visible="false" />
                                        <UC:AddressAusControl ID="addressAusControl" runat="server" ViewStateMode="Enabled" Visible="false" />
                                    </td>
                                    <td style="font-family:'Microsoft Sans Serif';"></td>
                                </tr>
                                <tr style="height:20px">
                                    <td colspan="4"></td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="4">
                                        <asp:Button ID="btnSubmit" runat="server" Text="Button" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" Visible="False" />
                                        <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />
                                    </td>
                                </tr>

                            </table>

                        </td>
                        <td rowspan="5" style="min-width:25px;">
                            <div style=" width: 1px !important; height:600px; background: #999; margin:0 28px;"></div>
                        </td>
                        <td style="min-width:935px;">

                            <center>
                                <asp:DropDownList ID="ddlMedicalServiceType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMedicalServiceType_SelectedIndexChanged" ></asp:DropDownList>
                                <div style="height:6px;"></div>
                            </center>                           

                            <table class="pt_page_nav">
                                <tr class="pt_page_nav">


                                    <td style="text-align:center;vertical-align:middle;margin-left:12px;" class="pt_page_nav">
                                        <a id="link_tab1" href="javascript:void(0)"  onclick="show_tab('tab1');return false;" class="pt_page_nav" style="outline:none;">History & Meds & Conditions</a>
                                    </td>

                                    <td class="pt_page_nav" style="vertical-align:middle;margin-left:12px;margin-right:12px;">
                                        <div style=" width: 1px !important; height:20px; background-color:#608080; margin:0 3px;"></div>
                                    </td>

                                    <td style="text-align:center;vertical-align:middle;" class="pt_page_nav">
                                        <a id="link_tab2" href="javascript:void(0)"  onclick="show_tab('tab2');return false;" class="pt_page_nav" style="outline:none;">Current Consultations</a>
                                    </td>

                                    <td class="pt_page_nav" style="vertical-align:middle;margin-left:12px;margin-right:12px;">
                                        <div style=" width: 1px !important; height:20px; background-color:#608080; margin:0 3px;"></div>
                                    </td>

                                    <td style="text-align:center;vertical-align:middle;" class="pt_page_nav">
                                        <a id="link_tab3" href="javascript:void(0)"  onclick="show_tab('tab3');return false;" class="pt_page_nav" style="outline:none;">Make Booking / Clinics</a>
                                    </td>

                                    <td class="pt_page_nav" style="vertical-align:middle;margin-left:12px;margin-right:12px;">
                                        <div style=" width: 1px !important; height:20px; background-color:#608080; margin:0 3px;"></div>
                                    </td>

                                    <td style="text-align:center;vertical-align:middle;" class="pt_page_nav">
                                        <a id="link_tab4" href="javascript:void(0)"  onclick="show_tab('tab4');return false;" class="pt_page_nav" style="outline:none;">Referrals</a>
                                    </td>

                                    <td class="pt_page_nav" style="vertical-align:middle;margin-left:12px;margin-right:12px;">
                                        <div style=" width: 1px !important; height:20px; background-color:#608080; margin:0 3px;"></div>
                                    </td>

                                    <td style="text-align:center;vertical-align:middle;margin-right:12px;" class="pt_page_nav">
                                        <a id="link_tab6" href="javascript:void(0)"  onclick="show_tab('tab6');return false;" class="pt_page_nav" style="outline:none;">Attached files & Letters</a>
                                    </td>


                                </tr>
                            </table>

                            <table style="width:100%;vertical-align:top;">
                                <tr id="tab1" style="background-color:#E8E4BF;display:none;">
                                    <td>
                                        <center>
                                            <table style="margin:4px 8px;">
                                                <tr>
                                                    <td>
                                                        <div style="height:10px;"></div>

                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <b>Medical History</b>&nbsp;<asp:Label ID="lblNotesListCount" runat="server"></asp:Label>
                                                                </td>
                                                                <td style="min-width:20px;"></td>
                                                                <td>
                                                                    <asp:HyperLink ID="lnkNotes" runat="server" ToolTip="Notes" Text="Add/Edit History" NavigateUrl="javascript:void(0)" />
                                                                    &nbsp;&nbsp;
                                                                    <asp:HyperLink ID="lnkNotesBodyChart" runat="server" ToolTip="Notes" Text="Add/Edit Body Chart Notes" NavigateUrl="javascript:void(0)" />
                                                                    <asp:Button ID="btnUpdateNotesIcon" runat="server" CssClass="hiddencol" onclick="btnUpdateNotesIcon_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                
                                                        <div style="height:10px;"></div>

                                                        <asp:Label ID="lblNotesList_NoRowsMessage" runat="server" Text="No notes entered for this patient."></asp:Label>

                                                        <asp:Panel ID="pnlNotesList" runat="server" ScrollBars="Auto" style="max-height:250px;">
                                                            <table id="tblNotesList" border="1" style="border-collapse:collapse; width:100%;" class="table table-bordered table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                                                <tr>
                                                                    <th class="nowrap hiddencol">Note ID</th>
                                                                    <th class="nowrap hiddencol">Note Entity ID</th>
                                                                    <th class="nowrap">Date Added</th>
                                                                    <th class="nowrap">Type</th>
                                                                    <th style="width:99%;">Note</th>
                                                                </tr>
                                                            <asp:Repeater id="lstNoteList" runat="server">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <tr style="vertical-align:top;">
                                                                        <td class="nowrap hiddencol">
                                                                            <asp:Label ID="lblNoteID"   Text='<%#  Eval("note_id") %>' runat="server"/>
                                                                        </td>
                                                                        <td class="nowrap hiddencol">
                                                                            <asp:Label ID="lblNoteEntityID" Text='<%#  Eval("entity_id") %>' runat="server"/>
                                                                        </td>
                                                                        <td class="nowrap">
                                                                            <asp:Label ID="lblNoteDateAdded" runat="server" Text='<%# Bind("date_added", "{0:dd-MM-yyyy}") %>' ></asp:Label> 
                                                                        </td>
                                                                        <td class="nowrap">
                                                                            <asp:Label ID="lblNoteType" runat="server" Text='<%# Eval("note_type_descr") %>' ></asp:Label> 
                                                                        </td>
                                                                        <td style="text-align:left;width:99%;">
                                                                            <asp:Label ID="lblText" runat="server" Font-Bold="True" Text='<%#  (Eval("body_part_descr") == DBNull.Value || Eval("body_part_descr").ToString().Length == 0 ? "" : @"<u>" + Eval("body_part_descr") + "</u> : ")   +   (Eval("text") == DBNull.Value ? "" : ((string)Eval("text")).Replace("\n", "<br/>")) %>'></asp:Label> 
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <tr id="trEmptyData" runat="server" visible="false">
                                                                        <td id="tdEmptyData" runat="server">
                                                                            No notes entered for this patient.
                                                                        </td>
                                                                    </tr>
                                                                </FooterTemplate>
                                                            </asp:Repeater>
                                                            </table>
                                                        </asp:Panel>


                                                        <br />


                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <b>Medications</b>&nbsp;<asp:Label ID="lblMedNotesListCount" runat="server"></asp:Label>
                                                                </td>
                                                                <td style="min-width:20px;"></td>
                                                                <td>
                                                                    <asp:HyperLink ID="lnkMedNotes" runat="server" ToolTip="Medications" Text="Add/Edit Medications" NavigateUrl="javascript:void(0)" />
                                                                    &nbsp;&nbsp;
                                                                    <asp:Button ID="btnUpdateMedNotesIcon" runat="server" CssClass="hiddencol" onclick="btnUpdateMedNotesIcon_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                
                                                        <div style="height:10px;"></div>

                                                        <asp:Label ID="lblMedNotesList_NoRowsMessage" runat="server" Text="No medications entered for this patient."></asp:Label>

                                                        <asp:Panel ID="pnlMedNotesList" runat="server" ScrollBars="Auto" style="max-height:250px;">
                                                            <table id="tblMedNotesList" border="1" style="border-collapse:collapse; width:100%;" class="table table-bordered table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                                                <tr>
                                                                    <th class="nowrap hiddencol">Note ID</th>
                                                                    <th class="nowrap hiddencol">Note Entity ID</th>
                                                                    <th class="nowrap">Date Added</th>
                                                                    <th style="width:99%;">Medication</th>
                                                                </tr>
                                                            <asp:Repeater id="lstMedNoteList" runat="server">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <tr style="vertical-align:top;">
                                                                        <td class="nowrap hiddencol">
                                                                            <asp:Label ID="lblNoteID"   Text='<%#  Eval("note_id") %>' runat="server"/>
                                                                        </td>
                                                                        <td class="nowrap hiddencol">
                                                                            <asp:Label ID="lblNoteEntityID" Text='<%#  Eval("entity_id") %>' runat="server"/>
                                                                        </td>
                                                                        <td class="nowrap">
                                                                            <asp:Label ID="lblNoteDateAdded" runat="server" Text='<%# Bind("date_added", "{0:dd-MM-yyyy}") %>' ></asp:Label> 
                                                                        </td>
                                                                        <td style="text-align:left; width:99%;">
                                                                            <asp:Label ID="lblText" runat="server" Font-Bold="True" Text='<%#  (Eval("body_part_descr") == DBNull.Value || Eval("body_part_descr").ToString().Length == 0 ? "" : @"<u>" + Eval("body_part_descr") + "</u> : ")   +   (Eval("text") == DBNull.Value ? "" : ((string)Eval("text")).Replace("\n", "<br/>")) %>'></asp:Label> 
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <tr id="trEmptyData" runat="server" visible="false">
                                                                        <td id="tdEmptyData" runat="server">
                                                                            No medications entered for this patient.
                                                                        </td>
                                                                    </tr>
                                                                </FooterTemplate>
                                                            </asp:Repeater>
                                                            </table>
                                                        </asp:Panel>


                                                        <br />


                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <b>Medical Conditions</b>&nbsp;<asp:Label ID="lblMedCondNotesListCount" runat="server"></asp:Label>
                                                                </td>
                                                                <td style="min-width:20px;"></td>
                                                                <td>
                                                                    <asp:HyperLink ID="lnkMedCondNotes" runat="server" ToolTip="Medical Conditions" Text="Add/Edit Medical Conditions" NavigateUrl="javascript:void(0)" />
                                                                    &nbsp;&nbsp;
                                                                    <asp:Button ID="btnUpdateMedCondNotesIcon" runat="server" CssClass="hiddencol" onclick="btnUpdateMedCondNotesIcon_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                
                                                        <div style="height:10px;"></div>

                                                        <asp:Label ID="lblMedCondNotesList_NoRowsMessage" runat="server" Text="No medical conditions entered for this patient."></asp:Label>

                                                        <asp:Panel ID="pnlMedCondNotesList" runat="server" ScrollBars="Auto" style="max-height:250px;">
                                                            <table id="tblMedCondNotesList" border="1" style="border-collapse:collapse; width:100%;" class="table table-bordered table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                                                <tr>
                                                                    <th class="nowrap hiddencol">Note ID</th>
                                                                    <th class="nowrap hiddencol">Note Entity ID</th>
                                                                    <th class="nowrap">Date Added</th>
                                                                    <th style="width:99%;">Medical Condition</th>
                                                                </tr>
                                                            <asp:Repeater id="lstMedCondNoteList" runat="server">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <tr style="vertical-align:top;">
                                                                        <td class="nowrap hiddencol">
                                                                            <asp:Label ID="lblNoteID"   Text='<%#  Eval("note_id") %>' runat="server"/>
                                                                        </td>
                                                                        <td class="nowrap hiddencol">
                                                                            <asp:Label ID="lblNoteEntityID" Text='<%#  Eval("entity_id") %>' runat="server"/>
                                                                        </td>
                                                                        <td class="nowrap">
                                                                            <asp:Label ID="lblNoteDateAdded" runat="server" Text='<%# Bind("date_added", "{0:dd-MM-yyyy}") %>' ></asp:Label> 
                                                                        </td>
                                                                        <td style="text-align:left; width:99%;">
                                                                            <asp:Label ID="lblText" runat="server" Font-Bold="True" Text='<%#  (Eval("body_part_descr") == DBNull.Value || Eval("body_part_descr").ToString().Length == 0 ? "" : @"<u>" + Eval("body_part_descr") + "</u> : ")   +   (Eval("text") == DBNull.Value ? "" : ((string)Eval("text")).Replace("\n", "<br/>")) %>'></asp:Label> 
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <tr id="trEmptyData" runat="server" visible="false">
                                                                        <td id="tdEmptyData" runat="server">
                                                                            No medical conditions entered for this patient.
                                                                        </td>
                                                                    </tr>
                                                                </FooterTemplate>
                                                            </asp:Repeater>
                                                            </table>
                                                        </asp:Panel>

                                                        <br />

                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <b>Conditions</b>
                                                                </td>
                                                                <td style="min-width:20px;"></td>
                                                             </tr>
                                                            <tr>
                                                                <td>

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
                                                                                    <asp:TextBox ID="txtText" runat="server" TextMode="MultiLine" Rows="3" Columns="30"></asp:TextBox>

                                                                                </ItemTemplate> 
                                                                            </asp:TemplateField> 

                                                                        </Columns> 

                                                                    </asp:GridView>

                                                                    <asp:GridView ID="GrdConditionView" runat="server" 
                                                                            AutoGenerateColumns="False" DataKeyNames="condition_condition_id" 
                                                                            OnRowCancelingEdit="GrdConditionView_RowCancelingEdit" 
                                                                            OnRowDataBound="GrdConditionView_RowDataBound" 
                                                                            OnRowEditing="GrdConditionView_RowEditing" 
                                                                            OnRowUpdating="GrdConditionView_RowUpdating" ShowHeader="False" ShowFooter="False" 
                                                                            OnRowCommand="GrdConditionView_RowCommand" 
                                                                            OnRowCreated="GrdConditionView_RowCreated"
                                                                            AllowSorting="True" 
                                                                            OnSorting="GrdConditionView_Sorting"
                                                                            ClientIDMode="Predictable"
                                                                            GridLines="None"
                                                                            CssClass="table table-grid-top-bottum-padding-normal auto_width block_center">

                                                                        <Columns> 

                                                                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="condition_condition_id" Visible="false"> 
                                                                                <ItemTemplate> 
                                                                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("condition_condition_id") %>'></asp:Label> 
                                                                                </ItemTemplate> 
                                                                            </asp:TemplateField> 

                                                                            <asp:TemplateField HeaderText="Checkbox" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_descr" ItemStyle-VerticalAlign="Top"> 
                                                                                <ItemTemplate> 
                                                                                    •
                                                                                </ItemTemplate> 
                                                                            </asp:TemplateField> 

                                                                            <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_descr" ItemStyle-CssClass="text_left"> 
                                                                                <ItemTemplate> 

                                                                                    <asp:Label ID="lblDescr" runat="server" Text='<%# Eval("condition_descr") %>'></asp:Label> 

                                                                                    <div id="br_date" runat="server" style="height:1px;"></div>
                                                                                    <asp:Label ID="lblDate" runat="server"></asp:Label> 

                                                                                    <div id="br_nweeksdue" runat="server" style="height:1px;"></div>
                                                                                    <asp:Label ID="lblNextDue" runat="server" Text="Next Due: "></asp:Label>
                                                                                    <asp:Label ID="lblDateDue" runat="server"></asp:Label> 

                                                                                    <div id="br_text" runat="server" style="height:1px;"></div>
                                                                                    <asp:Label ID="lblAdditionalInfo" runat="server" Text="<u>Additional Info</u>:<br />"></asp:Label>
                                                                                    <asp:Label ID="lblText" runat="server"></asp:Label> 

                                                                                </ItemTemplate> 
                                                                            </asp:TemplateField> 

                                                                        </Columns> 

                                                                    </asp:GridView>

                                                                </td>
                                                            </tr>
                                                        </table>


                                                        <br />


                                                    </td>
                                                </tr>
                                            </table>
                                        </center>
                                    </td>
                                </tr>
                                <tr id="tab2" style="background-color:#C7E6E6;display:none;">
                                    <td>
                                        <center>
                                            <table style="margin:4px 8px;">
                                                <tr>
                                                    <td>
                                                        <div style="height:10px;"></div>


                                                        <center>

                                                            <table>
                                                                <tr>
                                                                    <td id="lastBookingTextRow" runat="server" class="nowrap"><asp:Label ID="lblLastBookingText" runat="server">Last Appointment:</asp:Label></td>
                                                                    <td style="width:30px"></td>
                                                                    <td class="nowrap"><asp:HyperLink ID="lnkLastBooking" runat="server"></asp:HyperLink><asp:Label ID="lblLastBooking" runat="server"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td id="nextBookingTextRow" runat="server" class="nowrap"><asp:Label ID="lblNextBookingText" runat="server">Next Appointment:</asp:Label></td>
                                                                    <td style="width:30px"></td>
                                                                    <td class="nowrap"><asp:HyperLink ID="lnkNextBooking" runat="server"></asp:HyperLink><asp:Label ID="lblNextBooking" runat="server"></asp:Label></td>
                                                                </tr>
                                                                <tr style="height:6px">
                                                                    <td></td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="3" id="invoicesRow" runat="server" class="nowrap" style="text-align:center;"><asp:HyperLink ID="lnkInvoices" runat="server" NavigateUrl="~/InvoiceListV2.aspx?id=">View Invoices</asp:HyperLink></td>
                                                                </tr>

                                                            </table>

                                                            <br />

                                                            <asp:Button ID="btnUpdateBookingList" runat="server" CssClass="hiddencol" onclick="btnUpdateBookingList_Click" />
                                                            <asp:Panel  ID="pnlDefaultButton_SearchBookingList" runat="server" DefaultButton="btnSearchBookingList">
                                                                <table>
                                                                    <tr>
                                                                        <td><b>Bookings</b>&nbsp;<asp:Label ID="lblBookingListCount" runat="server"></asp:Label></td>

                                                                        <td style="width:25px"></td>

                                                                        <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">From: </asp:Label></td>
                                                                        <td class="nowrap"><asp:TextBox ID="txtStartDate" runat="server" Columns="10"/></td>
                                                                        <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                                        <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>

                                                                        <td style="width:10px"></td>

                                                                        <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">To: </asp:Label></td>
                                                                        <td class="nowrap"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                                                                        <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                                        <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>

                                                                        <td style="width:25px"></td>

                                                                        <td class="nowrap">
                                                                            <table style="line-height:8px;">
                                                                                <tr>
                                                                                    <td><label><input type="checkbox" checked="checked" onclick="show_hide_by_class('deleted_bk', this.checked);"/>&nbsp;Show/Hide Deleted BKs</label></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td><label><input type="checkbox" checked="checked" onclick="show_hide_by_class('cancelled_bk', this.checked);"/>&nbsp;Show/Hide Cancelled BKs</label></td>
                                                                                </tr>
                                                                            </table>
                                                                            <div style="height:8px;"></div>
                                                                       </td>

                                                                        <td style="width:25px"></td>
                        
                                                                        <td><asp:Button ID="btnSearchBookingList" runat="server" Text="Refresh" OnClick="btnSearchBookingList_Click" /></td>
                                                                        <td style="width:8px"></td>
                                                                        <td><asp:Button ID="btnPrintBookingList" runat="server" Text="Print" OnClick="btnPrintBookingList_Click" /></td>

                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>

                                                            <span style="height:10px;"></span>
                                                            <asp:Label ID="lblBookingsList_NoRowsMessage" runat="server" Text="No bookings exist for this patient."></asp:Label>
                                                    
                                                            <table id="tblBookingsList" border="1" style="text-align:center;width:100%;" class="table table-bordered table-grid table-grid-top-bottum-padding-normal auto_width block_center" >
                                                                <tr>
                                                                    <th id="bk_list_show_outstanding_row" runat="server" Visible='<%# Eval("show_outstanding_row").ToString()=="1"?true:false %>'>Due</th>
                                                                    <th>Date / Provider</th>
                                                                    <th>Notes</th>
                                                                    <th id="bk_list_show_notes_row" runat="server" Visible='<%# Eval("show_notes_row").ToString()=="1"?true:false %>'></th>
                                                                    <th>Status</th>
                                                                    <th></th>
                                                                    <th id="bk_list_show_change_history_row" runat="server" Visible='<%# Eval("show_change_history_row").ToString()=="1"?true:false %>'></th>
                                                                    <th id="bk_list_show_printletter_row" runat="server" Visible='<%# Eval("show_printletter_row").ToString()=="1"?true:false %>'></th>
                                                                    <th id="bk_list_show_invoice_row" runat="server" Visible='<%# Eval("show_invoice_row").ToString()=="1"?true:false %>'></th>
                                                                    <th id="bk_list_show_bookingsheet_row" runat="server" Visible='<%# Eval("show_bookingsheet_row").ToString()=="1"?true:false %>'></th>
                                                                </tr>

                                                            <asp:Repeater id="lstBookingList" runat="server">
                                                                <HeaderTemplate>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <tr runat="server" class='<%# (Convert.ToBoolean(Eval("is_deleted")) ? "deleted_bk" : "") + (Convert.ToBoolean(Eval("is_cancelled")) ? "cancelled_bk" : "")  %>'>
                                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_outstanding_row").ToString()=="1"?true:false %>'><asp:Label ID="lblOutstanding" runat="server" Text='<%#  Eval("inv_outstanding_text") %>'></asp:Label></td>
                                                                        <td class="nowrap">
                                                                            <asp:Label ID="lblBookingDate"   Text='<%# Eval("booking_date_start", "{0:dd-MM-yyyy}") + "<br />" + Eval("booking_date_start", "{0:HH:mm}") %>' runat="server" ForeColor='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? System.Drawing.ColorTranslator.FromHtml("#777777") : System.Drawing.ColorTranslator.FromHtml("#333333") %>' />
                                                                            <br />
                                                                            <asp:Label ID="lblProviderAndOrg" style="max-width:200px;" Text='<%#  Eval("person_provider_firstname") + " " + Eval("person_provider_surname") %>' runat="server" ForeColor='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? System.Drawing.ColorTranslator.FromHtml("#777777") : System.Drawing.ColorTranslator.FromHtml("#a52a2a") %>' />
                                                                        </td>
                                                                        <td runat="server" Visible='<%# Eval("show_notes_row").ToString()=="1"?true:false %>' style="text-align:left !important;">
                                                                            <div style="width:375px;max-width:375px;text-align:left;" class="wrapword">
                                                                                <asp:Label ID="lblBookingNotes" runat="server" Text='<%#  Eval("booking_notes_text") %>' ></asp:Label>
                                                                            </div>
                                                                        </td>
                                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_notes_row").ToString()=="1"?true:false %>'><asp:Label ID="lblNotes" runat="server" Text='<%#  Eval("notes_text") %>' Visible='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? false : true %>' /></td>
                                                                        <td class="nowrap">
                                                                            <asp:Label ID="lblBookingStatus" Text='<%# Eval("booking_status_descr").ToString() + (Eval("inv_type_text").ToString().Length == 0 ? "" : "<br /><font color=\"#A52A2A\">" + Eval("inv_type_text") + "</font>" ) %>' ForeColor='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? System.Drawing.ColorTranslator.FromHtml("#777777") : System.Drawing.ColorTranslator.FromHtml("#333333") %>' runat="server"/><a id="A1" runat="server" href="javascript:void(0)"  onclick="javascript:return false;" title='<%#  Eval("added_by_deleted_by_row") %>' style="text-decoration: none"><b>&nbsp;*&nbsp;</b></a>
                                                                        </td>

                                                                        <td class="nowrap" runat="server"><asp:Label ID="lblBookingFiles" Text='<%# Eval("booking_files_link").ToString() %>' runat="server"></asp:Label></td>

                                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_change_history_row").ToString()=="1"?true:false %>'><asp:Label ID="lblBookingHistory" Text='<%# Eval("booking_change_history_link").ToString() %>' runat="server"  Visible='<%# (bool)Eval("hide_change_history_link") ? false : true %>' ></asp:Label></td>
                                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_printletter_row").ToString()=="1"?true:false %>'><asp:HyperLink ID="lnkPrintLetter" runat="server" NavigateUrl='<%#  String.Format("~/Letters_PrintV2.aspx?booking={0}",Eval("booking_booking_id")) %>' ImageUrl="~/images/printer_green-24.png" AlternateText="Letters" ToolTip="Letters" Visible='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? false : true %>' /></td>
                                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_invoice_row").ToString()=="1"?true:false %>'><asp:Label ID="lblViewInvoice" runat="server" Text='<%#  Eval("invoice_text") %>' ToolTip= "View Invoice" Visible='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? false : true %>'></asp:Label></td>
                                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_bookingsheet_row").ToString()=="1"?true:false %>'><asp:HyperLink ID="lnkBookingSheetForPatient" runat="server" NavigateUrl='<%#  Eval("booking_url") %>' ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Booking Sheet" ToolTip="Booking Sheet" Visible='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value || (bool)Eval("hide_booking_link")) ? false : true %>' /></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <tr id="trEmptyData" runat="server" visible="false">
                                                                        <td id="tdEmptyData" runat="server">
                                                                            No bookings exist for this patient.
                                                                        </td>
                                                                    </tr>
                                                                </FooterTemplate>
                                                            </asp:Repeater>
                                                            </table>

                                                            <br />

                                                        </center>



                                                    </td>
                                                </tr>
                                            </table>
                                        </center>
                                    </td>
                                </tr>
                                <tr id="tab3" style="background-color:#E8DCCF;">
                                    <td>
                                        <center>
                                            <table style="margin:4px 8px;">


                                                <tr style="vertical-align:top;">

                                                    <td>

                                                        <br />

                                                        <b>Registered To:</b> &nbsp;&nbsp;&nbsp; <asp:LinkButton ID="showHideOrgsList" runat="server" OnClientClick="javascript:show_hide('div_orgs_list'); return false;">Add</asp:LinkButton>
                                                        <div style="line-height:4px;">&nbsp;</div>

                                                        <asp:GridView ID="GrdRegistration" runat="server" 
                                                                AutoGenerateColumns="False" DataKeyNames="register_patient_id" 
                                                                OnRowCancelingEdit="GrdRegistration_RowCancelingEdit" 
                                                                OnRowDataBound="GrdRegistration_RowDataBound" 
                                                                OnRowEditing="GrdRegistration_RowEditing" 
                                                                OnRowUpdating="GrdRegistration_RowUpdating" ShowFooter="False" ShowHeader="False" GridLines="None"
                                                                OnRowCommand="GrdRegistration_RowCommand" 
                                                                OnRowDeleting="GrdRegistration_RowDeleting" 
                                                                OnRowCreated="GrdRegistration_RowCreated"
                                                                AllowSorting="True" 
                                                                OnSorting="GridView_Sorting"
                                                                ClientIDMode="Predictable">

                                                            <Columns> 

                                                                <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_patient_id"> 
                                                                    <EditItemTemplate> 
                                                                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_patient_id") %>'></asp:Label>
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_patient_id") %>'></asp:Label> 
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="Organisation" HeaderStyle-HorizontalAlign="Left" SortExpression="name" FooterStyle-VerticalAlign="Top" ItemStyle-Wrap="false"> 
                                                                    <EditItemTemplate> 
                                                                        <asp:DropDownList ID="ddlOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id"> </asp:DropDownList> 
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("name") %>'></asp:Label> 
                                                                    </ItemTemplate> 
                                                                    <FooterTemplate> 
                                                                        <asp:DropDownList ID="ddlNewOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id"> </asp:DropDownList>
                                                                    </FooterTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField>
                                                                    <EditItemTemplate> 
                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Booking" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" SortExpression="num_registered_orgs" ItemStyle-Wrap="false"> 
                                                                    <EditItemTemplate> 
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        <asp:HyperLink ID="lnkBookings" runat="server" NavigateUrl='<%#  String.Format("~/BookingScreenGetPatientOrgsV2.aspx?patient_id={0}",Eval("patient_id")) %>' Text="Make Booking" AlternateText="Bookings" ToolTip="Bookings" />
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField>
                                                                    <EditItemTemplate> 
                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                    </EditItemTemplate> 
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="" ShowHeader="True" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="btnDelete" runat="server"  CommandName="_Delete" CommandArgument='<%# Bind("register_patient_id") %>' Text="Del" AlternateText="Delete" ToolTip="Delete" />
                                                                    </ItemTemplate>
                                                                    <FooterTemplate> 
                                                                        <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="AddRegistrationValidationGroup"></asp:LinkButton> 
                                                                    </FooterTemplate> 
                                                                </asp:TemplateField>


                                                            </Columns> 

                                                        </asp:GridView>

                                                        <div id="div_orgs_list" runat="server" style="display:none;" class="nowrap">
                                                            <asp:DropDownList ID="ddlOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id" /><asp:LinkButton ID="lnkRegisterPatient" runat="server" CausesValidation="True" OnClick="lnkRegisterPatient_Click" Text="Insert" ></asp:LinkButton> 
                                                        </div>


                                                        <div style="height:20px;"></div>

                                                        <center>
                                                            <table>
                                                                <tr>
                                                                    <td id="makeBookingRow" runat="server" class="nowrap"><asp:HyperLink ID="lnkMakeBooking" runat="server" >Make Booking List</asp:HyperLink></td>
                                                                </tr>
                                                                <tr style="height:15px">
                                                                    <td></td>
                                                                </tr>
                                                            </table>
                                                        </center>
                                                        <br />
                                                        <br />

                                                    </td>
                                                </tr>

                                            </table>
                                        </center>
                                    </td>
                                </tr>

                                <tr id="tab4" style="background-color:#F2F2D3;display:none;">
                                    <td>
                                        <center>
                                            <table style="margin:4px 8px;">
                                                <tr>
                                                    <td>
                                                        <div style="height:10px;"></div>



                                                        <center>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblValidationErrorReferral" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                                                                        <asp:ValidationSummary ID="ValidationSummaryReferralEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryReferralEdit"/>
                                                                        <asp:ValidationSummary ID="ValidationSummaryReferralAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryReferralAdd"/>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </center>

                                                        <br />

                                                        <b>Referrals</b>
                                                        
                                                        <div style="height:12px;"></div>

                                                        <asp:HiddenField ID="hiddenViewDdlFieldIDs" Value="" runat="server" />

                                                        <asp:GridView ID="GrdReferrals" runat="server" 
                                                                AutoGenerateColumns="False" DataKeyNames="referral_id" 
                                                                OnRowCancelingEdit="GrdReferrals_RowCancelingEdit" 
                                                                OnRowDataBound="GrdReferrals_RowDataBound" 
                                                                OnRowEditing="GrdReferrals_RowEditing" 
                                                                OnRowUpdating="GrdReferrals_RowUpdating" ShowFooter="True" ShowHeader="True" GridLines="Both"
                                                                OnRowCommand="GrdReferrals_RowCommand" 
                                                                OnRowDeleting="GrdReferrals_RowDeleting" 
                                                                OnRowCreated="GrdReferrals_RowCreated"
                                                                AllowSorting="True" 
                                                                OnSorting="GrdReferrals_Sorting"
                                                                ClientIDMode="Predictable"
                                                                CssClass="table table-bordered table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                                                            <Columns> 

                                                                <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="referral_id"> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblId" runat="server" Text='<%# Eval("referral_id") %>'></asp:Label> 
                                                                    </ItemTemplate> 
                                                                    <EditItemTemplate> 
                                                                        <asp:Label ID="lblId" runat="server" Text='<%# Eval("referral_id") %>'></asp:Label>
                                                                    </EditItemTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="Medical Service Type"  HeaderStyle-HorizontalAlign="Left" SortExpression="mct_descr"> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblMedicalServiceType" runat="server" Text='<%# Eval("mct_descr") %>'></asp:Label> 
                                                                    </ItemTemplate> 
                                                                    <EditItemTemplate> 
                                                                        <asp:DropDownList ID="ddlMedicalServiceType" runat="server" DataTextField="mct_descr" DataValueField="mct_medical_service_type_id"> </asp:DropDownList> 
                                                                    </EditItemTemplate> 
                                                                    <FooterTemplate>
                                                                        <asp:DropDownList ID="ddlNewMedicalServiceType" runat="server" DataTextField="mct_descr" DataValueField="mct_medical_service_type_id"> </asp:DropDownList> 
                                                                    </FooterTemplate>
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="Referred To"  HeaderStyle-HorizontalAlign="Left" SortExpression="referral_id"> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblReferrer" runat="server" Text='<%# Eval("referrer_person_firstname") + " " + Eval("referrer_person_surname")  %>'></asp:Label> 
                                                                        <%# Eval("register_referrer_id") != DBNull.Value ? "<br />" : "" %>
                                                                        <a href="javascript:void(0)"  onclick="get_register_referrer_forlist('<%# Eval("referral_id") %>');return false;"><%# Eval("register_referrer_id") != DBNull.Value ? "Change" : "Add" %></a>
                                                                    </ItemTemplate> 
                                                                    <EditItemTemplate> 
                                                                        <asp:Label ID="lblReferrer" runat="server" Text='<%# Eval("referrer_person_firstname") + " " + Eval("referrer_person_surname")  %>'></asp:Label> 
                                                                    </EditItemTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="Referral Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="date_referral_signed" ItemStyle-Wrap="false" FooterStyle-Wrap="false" ItemStyle-CssClass="text_left" FooterStyle-CssClass="text_left"> 
                                                                    <ItemTemplate> 
                                                                        <table>
                                                                            <tr style="text-align:left;">
                                                                                <td>Signed: </td>
                                                                                <td style="min-width:4px;"></td>
                                                                                <td><asp:Label ID="lblDateReferralSigned" runat="server" Text='<%# Eval("date_referral_signed", "{0:dd-MM-yyyy}") %>' ></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Received: </td>
                                                                                <td></td>
                                                                                <td><asp:Label ID="lblDateReferralReceived" runat="server" Text='<%# Eval("date_referral_received_in_office", "{0:dd-MM-yyyy}") %>' ></asp:Label></td>
                                                                            </tr>
                                                                        </table>
                                                                    </ItemTemplate> 
                                                                    <EditItemTemplate> 
                                                                        Signed: 
                                                                        <br />
                                                                        <asp:DropDownList ID="ddlDateReferralSigned_Day" runat="server"></asp:DropDownList>
                                                                        <asp:DropDownList ID="ddlDateReferralSigned_Month" runat="server"></asp:DropDownList>
                                                                        <asp:DropDownList ID="ddlDateReferralSigned_Year" runat="server"></asp:DropDownList>
                                                                        <br />
                                                                        Received: 
                                                                        <br />
                                                                        <asp:DropDownList ID="ddlDateReferralReceived_Day" runat="server"></asp:DropDownList>
                                                                        <asp:DropDownList ID="ddlDateReferralReceived_Month" runat="server"></asp:DropDownList>
                                                                        <asp:DropDownList ID="ddlDateReferralReceived_Year" runat="server"></asp:DropDownList>
                                                                    </EditItemTemplate> 
                                                                    <FooterTemplate>
                                                                        Signed: 
                                                                        <br />
                                                                        <asp:DropDownList ID="ddlNewDateReferralSigned_Day" runat="server"></asp:DropDownList>
                                                                        <asp:DropDownList ID="ddlNewDateReferralSigned_Month" runat="server"></asp:DropDownList>
                                                                        <asp:DropDownList ID="ddlNewDateReferralSigned_Year" runat="server"></asp:DropDownList>
                                                                        <br />
                                                                        Received: 
                                                                        <br />
                                                                        <asp:DropDownList ID="ddlNewDateReferralReceived_Day" runat="server"></asp:DropDownList>
                                                                        <asp:DropDownList ID="ddlNewDateReferralReceived_Month" runat="server"></asp:DropDownList>
                                                                        <asp:DropDownList ID="ddlNewDateReferralReceived_Year" runat="server"></asp:DropDownList>
                                                                    </FooterTemplate>
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="Remaining"  HeaderStyle-HorizontalAlign="Left"> 
                                                                    <ItemTemplate> 

                                                                        <asp:Repeater id="rptReferralRemaining" runat="server">
                                                                            <HeaderTemplate>
                                                                                <table style="width:100%" class="padded-table-1px">
                                                                            </HeaderTemplate>
                                                                            <ItemTemplate>
                                                                                <tr style="vertical-align:top;">
                                                                                    <td class="nowrap" style="text-align:left;"><asp:Label ID="lblField" Text='<%# Eval("field_descr") %>' runat="server"/></td>
                                                                                    <td class="nowrap" style="text-align:right">
                                                                                        <span style="min-width:8px;">&nbsp;</span>
                                                                                        <asp:Label ID="lblRemaining" runat="server" Text='<%# "<b>" + Eval("epcremaining_num_services_remaining") + "</b>" %>' />
                                                                                        <span style="min-width:8px;">&nbsp;</span>
                                                                                        <asp:ImageButton ID="btnAddQty" runat="server" OnCommand="rptReferralRemaining_Command" CommandName="AddQty" CommandArgument='<%# Eval("epcremaining_referral_remaining_id") %>' ImageUrl="~/images/add-icon-16.png" />
                                                                                        <asp:ImageButton ID="btnSubtractQty" runat="server" OnCommand="rptReferralRemaining_Command" CommandName="SubtractQty" CommandArgument='<%# Eval("epcremaining_referral_remaining_id") %>' ImageUrl="~/images/subtract-icon-16.png" />
                                                                                        <span style="min-width:8px;">&nbsp;</span>
                                                                                        <asp:ImageButton ID="btnRemoveItem" runat="server" OnCommand="rptReferralRemaining_Command" CommandName="RemoveItem" CommandArgument='<%# Eval("epcremaining_referral_remaining_id") %>' ImageUrl="~/images/Delete-icon-16.png" OnClientClick="javascript:if (!confirm('Are you sure you want to delete this record?')) return false;" />
                                                                                    </td>
                                                                                </tr>
                                                                            </ItemTemplate>
                                                                            <FooterTemplate>
                                                                                </table>
                                                                            </FooterTemplate>
                                                                        </asp:Repeater>

                                                                        <center>
                                                                        <table>
                                                                            <tr>
                                                                                <td style="white-space:nowrap;">
                                                                                    <center>
                                                                                        <asp:DropDownList ID="ddlFields" DataValueField="o_offering_id" DataTextField="o_name" runat="server"/>
                                                                                        <asp:LinkButton ID="btnFieldsShowToAdd" runat="server" Text="Add"  />
                                                                                        <asp:Button ID="btnFieldAdd" runat="server" Text="Add"  />
                                                                                        <asp:Button ID="btnFieldsCancelAdd" runat="server" Text="Cancel"  />
                                                                                    </center>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        </center>


                                                                    </ItemTemplate> 
                                                                </asp:TemplateField>                                                                
                                                                
                                                                <asp:TemplateField HeaderText="Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="added_or_last_modified_date" ItemStyle-CssClass="nowrap"> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblAddedLastModified" runat="server" Text='<%# Eval("added_or_last_modified_date", "{0:dd-MM-yyyy}") + (Eval("added_or_last_modified_by") == DBNull.Value || Eval("person_added_or_last_modified_by_firstname").ToString().Trim().Length == 0 ? "" : "<br />(" + Eval("person_added_or_last_modified_by_firstname") + ")") %>'></asp:Label>
                                                                    </ItemTemplate> 
                                                                    <EditItemTemplate> 
                                                                        <asp:Label ID="lblAddedLastModified" runat="server" Text='<%# Eval("added_or_last_modified_date", "{0:dd-MM-yyyy}") + (Eval("added_or_last_modified_by") == DBNull.Value || Eval("person_added_or_last_modified_by_firstname").ToString().Trim().Length == 0 ? "" : "<br />(" + Eval("person_added_or_last_modified_by_firstname") + ")") %>'></asp:Label>
                                                                    </EditItemTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="Health Card"  HeaderStyle-HorizontalAlign="Left" SortExpression="hc_organisation_name" ItemStyle-CssClass="text-center" > 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblHealthCard" runat="server" Text='<%# Eval("hc_organisation_name") == DBNull.Value || Eval("card_nbr") == DBNull.Value ? "" : ( Eval("hc_organisation_name") + (((string)Eval("card_nbr")).Length == 0 ? "" : ( "<br />" + (string)Eval("card_nbr") )) )  %>'></asp:Label> 
                                                                    </ItemTemplate> 
                                                                    <EditItemTemplate> 
                                                                        <asp:Label ID="lblHealthCard" runat="server" Text='<%# Eval("hc_organisation_name") == DBNull.Value || Eval("card_nbr") == DBNull.Value ? "" : ( Eval("hc_organisation_name") + (((string)Eval("card_nbr")).Length == 0 ? "" : ( "<br />" + (string)Eval("card_nbr") )) )  %>'></asp:Label> 
                                                                    </EditItemTemplate> 
                                                                    <FooterTemplate>
                                                                        <asp:DropDownList ID="ddlNewHealthCard" runat="server"> </asp:DropDownList> 
                                                                    </FooterTemplate>
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                                                    <ItemTemplate> 
                                                                       <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                                                    </ItemTemplate> 
                                                                    <EditItemTemplate> 
                                                                        <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryReferralEdit"></asp:LinkButton> 
                                                                        <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                                                    </EditItemTemplate> 
                                                                    <FooterTemplate> 
                                                                        <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryReferralAdd"></asp:LinkButton> 
                                                                    </FooterTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="" ShowHeader="True" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="btnDelete" runat="server"  CommandName="_Delete" CommandArgument='<%# Eval("referral_id") %>' Text="Del" AlternateText="Delete" ToolTip="Delete" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                            </Columns> 

                                                        </asp:GridView>


                                                        <div style="height:8px;"></div>

                                                        <center>
                                                            <table>
                                                                <tr>
                                                                    <td>

                                                                        <center>
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblValidationErrorHealthCard" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                                                                                        <asp:ValidationSummary ID="ValidationSummaryHealthCardEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryHealthCardEdit"/>
                                                                                        <asp:ValidationSummary ID="ValidationSummaryHealthCardAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryHealthCardAdd"/>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </center>

                                                                        <br />

                                                                        <b>Health Cards</b>
                                                        
                                                                        <div style="height:12px;"></div>

                                                                        <asp:GridView ID="GrdHealthCards" runat="server" 
                                                                                AutoGenerateColumns="False" DataKeyNames="health_card_id" 
                                                                                OnRowCancelingEdit="GrdHealthCards_RowCancelingEdit" 
                                                                                OnRowDataBound="GrdHealthCards_RowDataBound" 
                                                                                OnRowEditing="GrdHealthCards_RowEditing" 
                                                                                OnRowUpdating="GrdHealthCards_RowUpdating" ShowFooter="True" ShowHeader="True" GridLines="Both" Visible="true"
                                                                                OnRowCommand="GrdHealthCards_RowCommand" 
                                                                                OnRowDeleting="GrdHealthCards_RowDeleting" 
                                                                                OnRowCreated="GrdHealthCards_RowCreated"
                                                                                AllowSorting="True" 
                                                                                OnSorting="GrdHealthCards_Sorting"
                                                                                ClientIDMode="Predictable"
                                                                                CssClass="table table-bordered table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                                                                            <Columns> 

                                                                                <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="health_card_id"> 
                                                                                    <ItemTemplate> 
                                                                                        <asp:Label ID="lblId" runat="server" Text='<%# Eval("health_card_id") %>'></asp:Label> 
                                                                                    </ItemTemplate> 
                                                                                    <EditItemTemplate> 
                                                                                        <asp:Label ID="lblId" runat="server" Text='<%# Eval("health_card_id") %>'></asp:Label>
                                                                                    </EditItemTemplate> 
                                                                                </asp:TemplateField> 

                                                                                <asp:TemplateField HeaderText="Type"  HeaderStyle-HorizontalAlign="Left" SortExpression="mct_descr"> 
                                                                                    <ItemTemplate> 
                                                                                        <asp:Label ID="lblHealthCardOrganisation" runat="server" Text='<%# Eval("organisation_name") %>'></asp:Label> 
                                                                                    </ItemTemplate> 
                                                                                    <EditItemTemplate> 
                                                                                        <asp:DropDownList ID="ddlHealthCardOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id"></asp:DropDownList>
                                                                                    </EditItemTemplate> 
                                                                                    <FooterTemplate>
                                                                                        <asp:DropDownList ID="ddlNewHealthCardOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id" onchange="javascript:ddl_health_card_type_changed('MainContent_GrdHealthCards_ddlNewHealthCardOrganisation', 'MainContent_GrdHealthCards_lblNewHealthCardCardNbrFamilyNbrSeperator', 'MainContent_GrdHealthCards_ddlNewHealthCardCardFamilyMemberNbr');"></asp:DropDownList>
                                                                                    </FooterTemplate>
                                                                                </asp:TemplateField> 

                                                                                <asp:TemplateField HeaderText="Card Name"  HeaderStyle-HorizontalAlign="Left" SortExpression="card_name"> 
                                                                                    <ItemTemplate> 
                                                                                        <asp:Label ID="lblHealthCardCardName" runat="server" Text='<%# Eval("card_name") %>'></asp:Label> 
                                                                                    </ItemTemplate> 
                                                                                    <EditItemTemplate> 
                                                                                        <asp:TextBox ID="txtHealthCardCardName" runat="server" Text='<%# Eval("card_name") %>'></asp:TextBox>
                                                                                        <asp:RegularExpressionValidator ID="txtValidateCardNameRegex" runat="server" CssClass="failureNotification" 
                                                                                            ControlToValidate="txtHealthCardCardName"
                                                                                            ValidationExpression="^[a-zA-Z\-\s]+$"
                                                                                            ErrorMessage="Health Card Name can only be letters or hyphens."
                                                                                            Display="Dynamic"
                                                                                            ValidationGroup="ValidationSummaryHealthCardEdit">*</asp:RegularExpressionValidator>
                                                                                    </EditItemTemplate> 
                                                                                    <FooterTemplate>
                                                                                        <asp:TextBox ID="txtNewHealthCardCardName" runat="server"></asp:TextBox><asp:Label ID="lblHealthCardCardName" runat="server" Font-Bold="True"/>
                                                                                        <asp:RegularExpressionValidator ID="txtValidateCardNameRegex" runat="server" CssClass="failureNotification" 
                                                                                            ControlToValidate="txtNewHealthCardCardName"
                                                                                            ValidationExpression="^[a-zA-Z\-\s]+$"
                                                                                            ErrorMessage="Health Card Name can only be letters or hyphens."
                                                                                            Display="Dynamic"
                                                                                            ValidationGroup="ValidationSummaryHealthCardAdd">*</asp:RegularExpressionValidator>
                                                                                    </FooterTemplate>
                                                                                </asp:TemplateField> 

                                                                                <asp:TemplateField HeaderText="Card Nbr"  HeaderStyle-HorizontalAlign="Left" SortExpression="card_nbr"> 
                                                                                    <ItemTemplate> 
                                                                                        <asp:Label ID="lblHealthCardCardNbr" runat="server" Text='<%# Eval("card_nbr") + (  Eval("organisation_id") != DBNull.Value && ((int)Eval("organisation_id")) != -1 ? "" : (" - " + Eval("card_family_member_nbr")) ) %>'></asp:Label> 
                                                                                    </ItemTemplate> 
                                                                                    <EditItemTemplate> 
                                                                                        <asp:TextBox ID="txtHealthCardCardNbr" runat="server" Text='<%# Eval("card_nbr")  %>' />
                                                                                        <asp:Label ID="lblHealthCardCardNbrFamilyNbrSeperator" runat="server"> - </asp:Label>
                                                                                        <asp:DropDownList ID="ddlHealthCardCardFamilyMemberNbr" runat="server"></asp:DropDownList>
                                                                                        <asp:RegularExpressionValidator ID="txtValidateHealthCardCardNbrRegex" runat="server" CssClass="failureNotification" 
                                                                                            ControlToValidate="txtHealthCardCardNbr"
                                                                                            ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                                                                            ErrorMessage="Health Card Number can only be letters, numbers and hyphens"
                                                                                            Display="Dynamic"
                                                                                            ValidationGroup="ValidationSummaryHealthCardEdit">*</asp:RegularExpressionValidator>
                                                                                    </EditItemTemplate> 
                                                                                    <FooterTemplate>
                                                                                        <asp:TextBox ID="txtNewHealthCardCardNbr" runat="server"  />
                                                                                        <asp:Label ID="lblNewHealthCardCardNbrFamilyNbrSeperator" runat="server"> - </asp:Label>
                                                                                        <asp:DropDownList ID="ddlNewHealthCardCardFamilyMemberNbr" runat="server"></asp:DropDownList>
                                                                                        <asp:RegularExpressionValidator ID="txtValidateNewHealthCardCardNbrRegex" runat="server" CssClass="failureNotification" 
                                                                                            ControlToValidate="txtNewHealthCardCardNbr"
                                                                                            ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                                                                            ErrorMessage="Health Card Number can only be letters, numbers and hyphens"
                                                                                            Display="Dynamic"
                                                                                            ValidationGroup="ValidationSummaryHealthCardAdd">*</asp:RegularExpressionValidator>
                                                                                    </FooterTemplate>
                                                                                </asp:TemplateField> 

                                                                                <asp:TemplateField HeaderText="Exp. Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="expiry_date" ItemStyle-Wrap="false" FooterStyle-Wrap="false" > 
                                                                                    <ItemTemplate> 
                                                                                        <asp:Label ID="lblDateHealthCardSigned" runat="server" Text='<%# Eval("expiry_date", "{0:MM / yyyy}") %>' ></asp:Label>
                                                                                    </ItemTemplate> 
                                                                                    <EditItemTemplate> 
                                                                                        <asp:DropDownList ID="ddlHealthCardCardExpiry_Month" runat="server" />
                                                                                        <asp:DropDownList ID="ddlHealthCardCardExpiry_Year" runat="server" />
                                                                                    </EditItemTemplate> 
                                                                                    <FooterTemplate>
                                                                                        <asp:DropDownList ID="ddlNewHealthCardCardExpiry_Month" runat="server" />
                                                                                        <asp:DropDownList ID="ddlNewHealthCardCardExpiry_Year" runat="server" />
                                                                                    </FooterTemplate>
                                                                                </asp:TemplateField> 

                                                                                <asp:TemplateField HeaderText="Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="added_or_last_modified_date" ItemStyle-CssClass="nowrap"> 
                                                                                    <ItemTemplate> 
                                                                                        <asp:Label ID="lblAddedLastModified" runat="server" Text='<%# Eval("added_or_last_modified_date", "{0:dd-MM-yyyy}") + (Eval("added_or_last_modified_by") == DBNull.Value || Eval("person_added_or_last_modified_by_firstname").ToString().Trim().Length == 0 ? "" : "<br />(" + Eval("person_added_or_last_modified_by_firstname") + ")") %>'></asp:Label>
                                                                                    </ItemTemplate> 
                                                                                    <EditItemTemplate> 
                                                                                        <asp:Label ID="lblAddedLastModified" runat="server" Text='<%# Eval("added_or_last_modified_date", "{0:dd-MM-yyyy}") + (Eval("added_or_last_modified_by") == DBNull.Value || Eval("person_added_or_last_modified_by_firstname").ToString().Trim().Length == 0 ? "" : "<br />(" + Eval("person_added_or_last_modified_by_firstname") + ")") %>'></asp:Label>
                                                                                    </EditItemTemplate> 
                                                                                </asp:TemplateField> 

                                                                                <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                                                                    <ItemTemplate> 
                                                                                       <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                                                                    </ItemTemplate> 
                                                                                    <EditItemTemplate> 
                                                                                        <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryHealthCardEdit"></asp:LinkButton> 
                                                                                        <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                                                                    </EditItemTemplate> 
                                                                                    <FooterTemplate> 
                                                                                        <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryHealthCardAdd"></asp:LinkButton> 
                                                                                    </FooterTemplate> 
                                                                                </asp:TemplateField> 

                                                                            </Columns> 

                                                                        </asp:GridView>

                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </center>

                                                    </td>
                                                </tr>
                                            </table>
                                        </center>
                                    </td>
                                </tr>
                                <tr id="tab5" style="background-color:#FDD7E4;display:none;">
                                    <td>
                                        <center>
                                            <table style="margin:4px 8px;">
                                                <tr>
                                                    <td>
                                                        <div style="height:10px;"></div>

                                                        <b>Referral Templates</b>
                                                        <br />
                                                        <br />
                                                        ....
                                                    </td>
                                                </tr>
                                            </table>
                                        </center>
                                    </td>
                                </tr>
                                <tr id="tab6" style="background-color:#AFDDAF;display:none;">
                                    <td>
                                        <center>
                                            <table style="margin:4px 8px;">
                                                <tr>
                                                    <td>
                                                        <div style="height:10px;"></div>

                                                        <table id="spn_manage_files" runat="server" border="0" cellpadding="0" cellspacing="0">
                                                            <tr style="vertical-align:top;">
                                                                <td>

                                                                    <center>
                                                                        <asp:Label ID="lblCurrentPath" runat="server"></asp:Label>
                                                                    </center>

                                                                    <asp:GridView ID="GrdScannedDoc" runat="server" 
                                                                            AutoGenerateColumns="False"
                                                                            OnRowCancelingEdit="GrdScannedDoc_RowCancelingEdit" 
                                                                            OnRowDataBound="GrdScannedDoc_RowDataBound" 
                                                                            OnRowEditing="GrdScannedDoc_RowEditing" 
                                                                            OnRowUpdating="GrdScannedDoc_RowUpdating" ShowFooter="FALSE" 
                                                                            OnRowCommand="GrdScannedDoc_RowCommand" 
                                                                            OnRowDeleting="GrdScannedDoc_RowDeleting" 
                                                                            OnRowCreated="GrdScannedDoc_RowCreated"
                                                                            AllowSorting="False" 
                                                                            GridLines="Both"
                                                                            OnSorting="GrdScannedDoc_Sorting"
                                                                            RowStyle-VerticalAlign="top"
                                                                            ClientIDMode="Predictable"
                                                                            CssClass="table table-bordered table-lightgray table-grid table-grid-top-bottum-padding-normal auto_width block_center vertical_align_top">
                                                                        <Columns>

                                                                            <asp:TemplateField HeaderText="Name" ItemStyle-CssClass="text_left">
                                                                                <ItemTemplate>

                                                                                    <asp:LinkButton runat="server" ID="lbFolderItem" CommandName="OpenFolder" CommandArgument='<%# Eval("FullName") %>'></asp:LinkButton>
                                                                                    <asp:Literal runat="server" ID="ltlFileItem"></asp:Literal>

                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="Date Created" SortExpression="CreationTime">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblDateCreated" runat="server" Text='<%# Eval("CreationTime", "{0:dd MMM, yyyy}") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="Size" ItemStyle-HorizontalAlign="Right">
                                                                                <ItemTemplate>
                                                                                    <%# DisplaySize((long?) Eval("Size")) %>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="Name" SortExpression="Name" ItemStyle-CssClass="text_left">
                                                                                <ItemTemplate>
                                                                                        <asp:LinkButton ID="lnkFileDownload" runat="server" Visible='<%# (string)Eval("Name") != ".." && (bool)Eval("IsFolder") == false %>' CommandArgument='<%# Eval("FullName") %>' OnClick="btnDownload_Click" Text='download' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>

                                                                            <asp:TemplateField HeaderText="">
                                                                                <ItemTemplate>
                                                                                    <asp:ImageButton ID="btnDelete" runat="server" Visible='<%# Eval("Name") != ".." %>' ImageUrl="~/images/Delete-icon-16.png" CommandArgument='<%# Eval("FullName") %>' OnClick="btnDeleteFie_Click" OnClientClick="javascript:if (!confirm('Are you sure you want to permanently delete this file?')) return false;" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>

                                                                        </Columns>
                                                                    </asp:GridView>

                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>


                                                                    <center>
                                                                        <div style="height:8px;"></div>
                                                                        <table class="block_center">
                                                                            <tr style="vertical-align:top;">
                                                                                <td>
                                                                                    <center>

                                                                                        <strong>Upload Files</strong>
                                                                                        <div style="line-height:10px;">&nbsp;</div>

                                                                                        <asp:FileUpload ID="FileUpload1" runat="server" class="multi" style="color:transparent;width:85px;" accept="bmp|dcs|doc|docm|docx|dot|dotm|dotx|gz|gif|htm|html|ipg|jpe|jpeg|jpg|mp3|mp4|wav|pdf|png|ppa|ppam|pps|ppsm|ppt|pptm|tar|tgz|tif|tiff|txt|xla|xlam|xlc|xld|xlk|xll|xlm|xls|xlsx|xlt|xltm|xltx|xlw|xml|z|zip|7z"  />
                                                                                        <div style="height:12px;"></div>
                                                                                        <asp:CheckBox ID="chkAllowOverwrite" runat="server" Font-Size="Small" ForeColor="GrayText" Text="&nbsp;Allow File Overwrite" />

                                                                                        <center>
                                                                                            <asp:Button ID="btnUpload" runat="server" Text="Upload All" OnClick="btnUpload_Click" />
                                                                                        </center>
                                                                                        <div style="height:15px;"></div>
                                                                                        <asp:Label ID="lblUploadMessage" runat="server"></asp:Label>

                                                                                    </center>
                                                                                </td>
                                                                                <td style="min-width:30px;"></td>
                                                                                <td>
                                                                                    <center>

                                                                                        <strong>Create Directory</strong>
                                                                                        <div style="line-height:10px;">&nbsp;</div>

                                                                                        <asp:Panel ID="pnlNewDirectory" runat="server" DefaultButton="btnAddDirectory">
                                                                                            <asp:Textbox ID="txtNewDirectory" runat="server" style="width:125px;min-width:100%;"></asp:Textbox>
                                                                                            <div style="line-height:2px;">&nbsp;</div>
                                                                                            <asp:Button ID="btnAddDirectory" runat="server" style="width:125px;min-width:100%;" Text="Create Directory" OnClick="btnAddDirectory_Click" />&nbsp;
                                                                                        </asp:Panel>

                                                                                    </center>
                                                                                </td>
                                                                            </tr>
                                                                        </table>

                                                                    </center>


                                                                </td>
                                                            </tr>

                                                        </table>

                                                        <br />

                                                        <table>
                                                            <tr>
                                                                <td colspan="5" class="nowrap">
                                                                    <b>Patient Letters</b>: <asp:HyperLink ID="lnkPrintLetter" runat="server" NavigateUrl="~/Letters_PrintV2.aspx?org=">Print</asp:HyperLink> |
                                                                    <asp:HyperLink ID="lnkLetterPrintHistory" runat="server" NavigateUrl="~/Letters_SentHistoryV2.aspx?org=">View History</asp:HyperLink>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                            
                                                        <br />

                                                    </td>
                                                </tr>
                                            </table>
                                        </center>
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

    </asp:Panel>

</asp:Content>



