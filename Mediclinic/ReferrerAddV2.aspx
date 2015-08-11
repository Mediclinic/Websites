<%@ Page Title="Add Patient" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReferrerAddV2.aspx.cs" Inherits="ReferrerAddV2" %>
<%@ Register TagPrefix="UC" TagName="DuplicatePersonModalElementControl" Src="~/Controls/DuplicatePersonModalElementControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/duplicate_person_modal_boxV2.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/check_duplicate_persons.js"></script>
    <script type="text/javascript">

        function duplicate_person_check(obj) {

            var firstname = document.getElementById('txtFirstname').value.trim();
            var surname = document.getElementById('txtSurname').value.trim();

            var result = ajax_duplicate_persons("referrer", firstname, surname);

            if (result.length == 0) {
                alert("Error retreiving records for duplicate person check.");
            }
            else if (result == "NONE") {
                return;
            }
            else {
                var result_list = create_result_array(result);
                create_table(result_list, "ctable", "ReferrerList_ClinicInfoOfDoctorV2.aspx?referrer=");

                reveal_modal('modalPopupDupicatePerson');
            }
        }
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }

        function set_existing_person(referrer_id) {
            document.getElementById("jsSetId").value = String(referrer_id);
            document.getElementById('btnSetExistingReferrer').click();
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
            <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server">Add Referrer</asp:Label></h3></div>
            <div class="main_content">

                <UC:DuplicatePersonModalElementControl ID="duplicatePersonModalElementControl" runat="server" />
                <asp:Button ID="btnSetExistingReferrer" runat="server" CssClass="hiddencol" onclick="btnSetExistingReferrer_Click" />
                <asp:HiddenField ID="hiddenIsMobileDevice" runat="server" Value="0" />

                <div class="text-center">
                    <table class="block_center">
                        <tr>
                            <td style="text-align:left">
                                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>


                <table id="maintable" runat="server" style="margin:0 auto;" class="padded-table-2px">
                    <tr id="idRow" runat="server">
                       <td></td>
                        <td>ID</td>
                        <td>
                            <asp:Label ID="lblId" runat="server" Text="-1"></asp:Label>
                            <asp:HiddenField ID="jsSetId" runat="server" Value="-1" />

                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="min-width:12px;"></td>
                        <td>Title</td>
                        <td>
                            <asp:DropDownList ID="ddlTitle" runat="server" DataTextField="descr" DataValueField="title_id" onchange='title_changed_reset_gender();'></asp:DropDownList> 
                        </td>
                        <td rowspan="13" valign="top" id="td_ReferrersOfSelectedOrg" runat="server">
                            
                            <table id="pnlReferrersOfSelectedOrg" runat="server" style="min-width:550px;">
                                <tr style="vertical-align:top;">
                                    <td style="width:32px"></td>
                                    <td>

                                        <b><big><asp:Label ID="lblProvidersOf" runat="server" Text="Providers Of" Visible="false" /> <asp:Label ID="lblSelectedOrg" runat="server" Text="" /></big></b>
                                        <div style="line-height:7px;">&nbsp;</div>
                                        <asp:Label ID="lblProvidersOfSelectedOrg" runat="server" Text="" />

                                    </td>
                                </tr>
                            </table>
                        </td>

                    </tr>
                    <tr>
                        <td><asp:RegularExpressionValidator ID="txtValidateFirstnameRegex" runat="server" CssClass="failureNotification" 
                                    ControlToValidate="txtFirstname"
                                    ValidationExpression="^[a-zA-Z\-\s']+$"
                                    ErrorMessage="Firstname can only be letters or hyphens."
                                    Display="Dynamic"
                                    ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                        </td>
                        <td class="nowrap">First Name</td>
                        <td><asp:TextBox ID="txtFirstname" runat="server" onkeyup="capitalize_first(this);" /></td>
                    </tr>
                    <tr>
                        <td><asp:RegularExpressionValidator ID="txtValidateMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                    ControlToValidate="txtMiddlename"
                                    ValidationExpression="^[a-zA-Z\-\s']+$"
                                    ErrorMessage="Middlename can only be letters or hyphens."
                                    Display="Dynamic"
                                    ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                        </td>
                        <td class="nowrap">Middle Name</td>
                        <td><asp:TextBox ID="txtMiddlename" runat="server" onkeyup="capitalize_first(this);" /></td>
                    </tr>
                    <tr>
                        <td><asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                    ControlToValidate="txtSurname" 
                                    ErrorMessage="Surname is required."
                                    Display="Dynamic"
                                    ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="txtValidateSurnameNameRegex" runat="server" CssClass="failureNotification" 
                                    ControlToValidate="txtSurname"
                                    ValidationExpression="^[a-zA-Z\-\s']+$"
                                    ErrorMessage="Surname can only be letters or hyphens."
                                    Display="Dynamic"
                                    ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                        </td>
                        <td class="nowrap">Surname</td>
                        <td> <asp:TextBox ID="txtSurname" runat="server" onkeyup="capitalize_first(this);" onblur="duplicate_person_check(this);" /></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>Gender</td>
                        <td><asp:DropDownList ID="ddlGender" runat="server"> 
                                <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>

                    <tr style="height:30px"> 
                        <td colspan="3"></td>
                    </tr>

                    <tr>
                        <td><asp:RegularExpressionValidator ID="txtValidateProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtProviderNumber"
                                ValidationExpression="^[0-9a-zA-Z\-\s]+$"
                                ErrorMessage="Provider Number can only be letters, numbers, or hyphens."
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                        </td>
                        <td class="nowrap">Provider Number</td>
                        <td> <asp:TextBox ID="txtProviderNumber" runat="server" onblur="duplicate_person_check(this);"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>Report Every Patient Visit</td>
                        <td>
                            <asp:CheckBox ID="chkIsReportEveryVisit" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="nowrap">Batch Send Treatment Notes</td>
                        <td>
                            <asp:CheckBox ID="chkIsBatchSendAllPatientsTreatmentNotes" runat="server" Checked="true" />
                        </td>
                    </tr>
                    <tr  id="newOrgSepeatorRow" runat="server" style="height:30px"> 
                        <td colspan="3"></td>
                    </tr>

                    <tr id="orgsListRow" runat="server">
                        <td></td>
                        <td class="nowrap">Organisation</td>
                        <td class="nowrap">
                            <asp:DropDownList ID="ddlOrgsList" runat="server" DataTextField="name" DataValueField="organisation_id" AutoPostBack="True" OnSelectedIndexChanged="ddlOrgsList_SelectedIndexChanged"> </asp:DropDownList> 
                            &nbsp;
                            <asp:LinkButton ID="lnkAddNewOrg" runat="server" OnClick="lnkAddNewOrg_Click">Not In List?</asp:LinkButton></td>
                    </tr>



                    <tr id="newOrgTitleRow" runat="server">
                        <td></td>
                        <td colspan="3">

                            <div style="width:75%; height:1px; margin:35px auto; background:#999;"></div>

                            <b>New Clinic Information</b>
                        </td>
                    </tr>
                    <tr id="newOrgNameRow" runat="server">
                        <td><asp:RequiredFieldValidator ID="txtValidateOrgNameRequired" runat="server" CssClass="failureNotification"  
                                ControlToValidate="txtOrgName" 
                                ErrorMessage="Org name is required."
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="txtValidateOrgNameRegex" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtOrgName"
                                ValidationExpression="^[a-zA-Z\-\s]+$"
                                ErrorMessage="Org name can only be letters, and dashes."
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                        </td>
                        <td class="nowrap">Org Name</td>
                        <td><asp:TextBox ID="txtOrgName" runat="server" onkeyup="capitalize_first(this);" Width="75%"></asp:TextBox> </td>
                        <td></td>

                    </tr>
                    <tr  id="newOrgABNRow" runat="server">
                        <td><asp:RegularExpressionValidator ID="txtValidateABNRegex" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtOrgABN"
                                ValidationExpression="^[0-9\-]+$"
                                ErrorMessage="ABN can only be numbers and dashes."
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                        </td>
                        <td class="nowrap">Org ABN</td>
                        <td><asp:TextBox Width="75%" ID="txtOrgABN" runat="server" Text='<%# Bind("abn") %>'></asp:TextBox></td>
                        <td></td>
                    </tr>
                    <tr id="newOrgACNRow" runat="server">
                        <td><asp:RegularExpressionValidator ID="txtValidateACNRegex" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtOrgACN"
                                ValidationExpression="^[0-9\-]+$"
                                ErrorMessage="ACN can only be numbers and dashes."
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                        </td>
                        <td class="nowrap">Org ACN</td>
                        <td><asp:TextBox Width="75%" ID="txtOrgACN" runat="server" ></asp:TextBox></td>
                        <td></td>
                    </tr>
                    <tr id="newOrgCommentsRow" runat="server">
                        <td></td>
                        <td class="nowrap">Org Comments</td>
                        <td colspan="2">
                            <asp:TextBox ID="txtOrgComments" TextMode="multiline" rows="3" style="width:75%" runat="server" />
                        </td>
                    </tr>
                    <tr id="newOrgSepeatorTrailingRow" runat="server" style="height:10px">
                        <td colspan="4">&nbsp;</td>
                    </tr>
                    <tr id="newOrgAddressRow1" runat="server">
                        <td colspan="4">
                            <b>Address</b>
                            <div style="height:5px;"></div>
                            <asp:Button ID="btnUpdateAddressStreetAndSuburb" runat="server" CssClass="hiddencol" onclick="btnUpdateAddressStreetAndSuburb_Click" />
                            <asp:Button ID="btnUpdateAddressType" runat="server" CssClass="hiddencol" onclick="btnUpdateAddressType_Click" />
                            <asp:Button ID="btnUpdatePhoneType" runat="server" CssClass="hiddencol" onclick="btnUpdatePhoneType_Click" />
                            <asp:Button ID="btnUpdateEmailType" runat="server" CssClass="hiddencol" onclick="btnUpdateEmailType_Click" />
                        </td>
                    </tr>
                    <tr id="newOrgAddressRow2" runat="server">
                        <td></td>
                        <td>Type</td>
                        <td colspan="2">
                            <asp:DropDownList ID="ddlAddressContactType" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id"/>
                            <small><asp:HyperLink ID="lnkAddressUpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                            <asp:Label ID="lblAddressContactType" runat="server" Font-Bold="True"/>
                        </td>
                    </tr>
                    <tr id="newOrgAddressRow3" runat="server">
                        <td></td>
                        <td><asp:Label ID="lblAddressLine1Descr" runat="server">Line 1</asp:Label></td>
                        <td colspan="2">
                            <asp:TextBox ID="txtAddressAddrLine1" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="newOrgAddressRow4" runat="server">
                        <td></td>
                        <td>Line 2</td>
                        <td colspan="2">
                            <asp:TextBox ID="txtAddressAddrLine2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="newOrgAddressRow5" runat="server" visible="False">
                        <td></td>
                        <td>Street</td>
                        <td colspan="2">
                            <asp:DropDownList ID="ddlAddressAddressChannel" runat="server" DataTextField="ac_descr" DataValueField="ac_address_channel_id"/>
                            <small><asp:HyperLink ID="lnkAddressUpdateChannel" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                        </td>
                    </tr>
                    <tr id="newOrgAddressRow6" runat="server" visible="False">
                        <td></td>
                        <td>Street</td>
                        <td colspan="2">
                            <asp:TextBox ID="txtStreet" runat="server"  Columns="30"></asp:TextBox>
                            <asp:DropDownList ID="ddlAddressAddressChannelType" runat="server"/>
                        </td>
                    </tr>
                    <tr id="newOrgAddressRow7" runat="server">
                        <td></td>
                        <td>Suburb</td>
                        <td colspan="2">
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
                    <tr id="newOrgAddressRow8" runat="server">
                        <td></td>
                        <td>Country</td>
                        <td colspan="2">
                            <asp:DropDownList ID="ddlAddressCountry" runat="server" DataTextField="descr" DataValueField="country_id"/>
                        </td>
                    </tr>
                    <tr id="newOrgAddressRow9" runat="server">
                        <td></td>
                        <td><asp:Label ID="lblAddressFreeText" runat="server">Note</asp:Label></td>
                        <td colspan="2">
                            <asp:TextBox ID="txtAddressFreeText" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="newOrgAddressRow10" runat="server" style="height:10px">
                        <td colspan="4">&nbsp;</td>
                    </tr>
                    <tr id="newOrgContactTitleRow" runat="server">
                        <td colspan="4"><b>Phone/Other Contact Information</b></td>
                    </tr>
                    <tr id="newOrgContactRow1" runat="server">
                        <td></td>
                        <td class="nowrap">
                            <asp:DropDownList ID="ddlPhoneNumber1" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id" TabIndex="-1"/>
                            <small><asp:HyperLink ID="lnkPhone1UpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtPhoneNumber1" runat="server" Columns="25"/>
                            &nbsp;&nbsp;
                            <asp:Label ID="lblPhoneNumber1FreeText" runat="server">Note</asp:Label>
                            &nbsp;
                            <asp:TextBox ID="txtPhoneNumber1FreeText" runat="server" Columns="25"/>
                        </td>
                    </tr>
                    <tr id="newOrgContactRow2" runat="server">
                        <td></td>
                        <td class="nowrap">
                            <asp:DropDownList ID="ddlPhoneNumber2" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id" TabIndex="-1"/>
                            <small><asp:HyperLink ID="lnkPhone2UpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtPhoneNumber2" runat="server" Columns="25"/>
                            &nbsp;&nbsp;
                            <asp:Label ID="lblPhoneNumber2FreeText" runat="server">Note</asp:Label>
                            &nbsp;
                            <asp:TextBox ID="txtPhoneNumber2FreeText" runat="server" Columns="25"/>
                        </td>
                    </tr>
                    <tr id="newOrgContactRow3" runat="server">
                        <td></td>
                        <td class="nowrap">
                            <asp:DropDownList ID="ddlEmailContactType" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id" TabIndex="-1"/>
                            <small><asp:HyperLink ID="lnkEmailUpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtEmailAddrLine1" runat="server" Columns="25"></asp:TextBox>
                        </td>
                    </tr>

                </table>


                <div style="width:800px; height:1px; margin:35px auto; background:#999;"></div>


                <div class="text-center">

                    <asp:Button ID="btnSubmit" runat="server" Text="Button" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="window.returnValue=false;self.close();" />

                    <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />

                </div>


                <div style="height:5px;"></div>

                <div id="autodivheight" class="divautoheight" style="height:500px;">
                </div>

            </div>
        </div>

    </asp:Panel>

</asp:Content>



