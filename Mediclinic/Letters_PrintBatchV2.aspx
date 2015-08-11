<%@ Page Title="Mass Marketing To Patients (Bulk Letters, Emails, SMS)" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Letters_PrintBatchV2.aspx.cs" Inherits="Letters_PrintBatchV2" ValidateRequest="false" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function select_all(chkBox, selectBox) {

            // have we been passed an ID
            if (typeof selectBox == "string") {
                selectBox = document.getElementById(selectBox);
            }

            // is the select box a multiple select box?
            if (selectBox.type == "select-multiple") {
                for (var i = 0; i < selectBox.options.length; i++) {
                    selectBox.options[i].selected = chkBox.checked;
                }
            }


            // if orgs list all deselected, then deselect all patients also
            if (!chkBox.checked && chkBox.id == '<%= chkSelectAllOrgs.ClientID  %>') {
                selectBox = document.getElementById('<%= lstPatients.ClientID  %>');
                for (var i = 0; i < selectBox.options.length; i++) {
                    selectBox.options[i].selected = chkBox.checked;
                }

                document.getElementById('<%= chkSelectAllPatients.ClientID  %>').checked = false;
            }

        }

        function clear_error_message() {
            var lblErrorMessage = document.getElementById('lblErrorMessage');
            if (lblErrorMessage)
                document.getElementById('lblErrorMessage').value = '';
        }



        function get_organisation() {
            var retVal = window.showModalDialog("OrganisationListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:750px;resizable:yes;center:yes;");

            if (typeof retVal === "undefined")
                return;

            var index = retVal.indexOf(":");
            var newOrgID = retVal.substring(0, index);
            var newOrgName = retVal.substring(index + 1);


            // dont add if alread in there
            var alreadyAddedList = document.getElementById('hiddenOrgIDsList').value.split(',');
            for (var i = 0; i < alreadyAddedList.length; i++) {
                if (alreadyAddedList[i] == newOrgID) {
                    alert("Organisation already added");
                    return;
                }
            }


            var itemNew = document.createElement('option');
            itemNew.value = newOrgID;
            itemNew.text = newOrgName;

            var added = false;
            var lstOrgs = document.getElementById('lstOrgs');
            for (var i = 0; i < lstOrgs.length; i++) {
                if (newOrgName.localeCompare(lstOrgs.options[i].text) > 0)
                    continue;

                var itemOld = lstOrgs.options[i];
                try {
                    lstOrgs.add(itemNew, itemOld); // standards compliant; doesn't work in IE
                }
                catch (ex) {
                    lstOrgs.add(itemNew, i); // IE only
                }
                added = true;
                break;
            }
            if (!added) {
                try {
                    lstOrgs.add(itemNew, null); // standards compliant; doesn't work in IE
                }
                catch (ex) {
                    lstOrgs.add(itemNew); // IE only
                }
                added = true;
            }

            updateHiddenOrgIDs();
            document.getElementById('btnUpdateOrgs').click();
        }
        function remove_selected_organisation() {
            var lstOrgs = document.getElementById('lstOrgs');
            for (var i = lstOrgs.length - 1; i >= 0; i--) {
                if (lstOrgs.options[i].selected)
                    lstOrgs.remove(i);
            }
            updateHiddenOrgIDs();
            document.getElementById('btnUpdateOrgs').click();
        }
        function updateHiddenOrgIDs() {
            var items = "";
            var lstOrgs = document.getElementById('lstOrgs');
            for (var i = 0; i < lstOrgs.length; i++)
                items = items + (items.length > 0 ? "," : "") + lstOrgs.options[i].value;
            document.getElementById('hiddenOrgIDsList').value = items;
        }


        function hideColumn(hide, tableId, colIndex) {
            var table = document.getElementById(tableId);
            if (table != null)
                for (i = 0; i < table.rows.length; i++)
                    table.rows[i].cells[colIndex].style.display = hide ? 'none' : '';
        }

        function hideOrgColumn() {
            hideColumn(true, 'tbl_select_org_and_patient', 0);
            hideColumn(true, 'tbl_select_org_and_patient', 1);

            document.getElementById('lblPatientsNotLinkedToAnyOrg').style.display = 'none';
            document.getElementById('chkIncPatientsWithNoOrg').style.display = 'none';
            document.getElementById('btnUpdatePatientList').style.display = 'none';
            document.getElementById('lblTotalText').style.display = 'none';
            document.getElementById('lblPatientCount').style.display = 'none';

            document.getElementById('lblOneLetterPerPatientText').style.display = 'none';
            document.getElementById('chkOneLetterPerPatient').style.display = 'none';
        }




        function confirm_letter_selected() {

            // if print (1) selected, and no letter selected, will catch it in code behind.
            // so .. check if print not selected (ie wont be caught in code behind) but email selected ... check if they forgot or not
            if (!send_method_selected_contains('1') && send_method_selected_contains('2') && !letter_selected())
                if (!confirm('Email is selected as a method of correspondence, but no letter is selected to be attached. \n\r\n\rSend anyway?'))
                    return false;

            return true;
        }
        function letter_selected()
        {
            var sel = document.getElementById('lstLetters');
            var listLength = sel.options.length;
            for (var i = 0; i < listLength; i++)
                if (sel.options[i].selected)
                    return true;

            return false;
        }
        function send_method_selected_contains(sendMethodID) {
            return (get_selected_value_from_ddl('ddlBothMobileAndEmail')   == sendMethodID ||
                    get_selected_value_from_ddl('ddlMobileNoEmail')        == sendMethodID ||
                    get_selected_value_from_ddl('ddlEmailNoMobile')        == sendMethodID ||
                    get_selected_value_from_ddl('ddlNeitherMobileOrEmail') == sendMethodID);
        }
        function get_selected_value_from_ddl(id) {
            var e = document.getElementById(id);
            return e.options[e.selectedIndex].value;
        }


        function show_hide(id) {
            obj = document.getElementById(id);
            obj.style.display = (obj.style.display == "none") ? "" : "none";
        }



        function check_special_fields() {

            if (document.getElementById('txtSMSText').value.length > 0) {
                var ok = check_special_fields_of_text(document.getElementById('txtSMSText').value);
                if (!ok)
                    return false;
            }
            if (document.getElementById('FreeTextBox1').value.length > 0) {
                var ok = check_special_fields_of_text(document.getElementById('FreeTextBox1').value);
                if (!ok)
                    return false;
            }

            return true;
        }
        function check_special_fields_of_text(text) {

            if (!check_special_fields_ok(text, 'pt_name') && !confirm('field  \'pt_name\'  is missing a bracket and will not be replaced. Continue anyway?'))
                return false;
            if (!check_special_fields_ok(text, 'pt_title') && !confirm('field  \'pt_title\'  is missing a bracket and will not be replaced. Continue anyway?'))
                return false;
            if (!check_special_fields_ok(text, 'pt_firstname') && !confirm('field  \'pt_firstname\'  is missing a bracket and will not be replaced. Continue anyway?'))
                return false;
            if (!check_special_fields_ok(text, 'pt_middlename') && !confirm('field  \'pt_middlename\'  is missing a bracket and will not be replaced. Continue anyway?'))
                return false;
            if (!check_special_fields_ok(text, 'pt_surname') && !confirm('field  \'pt_surname\'  is missing a bracket and will not be replaced. Continue anyway?'))
                return false;

            return true;
        }
        function check_special_fields_ok(text, field) {

            var pos = 0;
            var i   = -1;
            while (pos != -1) {

                pos = text.indexOf(field, i + 1);
                i = pos;
                if (pos != -1) {

                    if (pos == 0) {
                        return false;
                    }
                    if (text.charAt(pos - 1) != '{') {
                        return false;
                    }
                    if (text.length <= (pos + field.length)) {
                        return false;
                    }
                    if (text.charAt(pos + field.length) != '}') {
                        return false;
                    }
                }
            }

            return true;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Mass Marketing To Patients (Bulk Letters, Emails, SMS)</asp:Label> &nbsp; <asp:HyperLink ID="lnkToEntity" runat="server"/></div>
        <div class="main_content" style="padding:20px 5px;">

            <center>

                <div class="block_center">
                    <table>
                        <tr>
                            <td style="text-align:left;">
                                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td style="text-align:left;">
                                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>

           
                <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="btnDefaultButton_NoSubmit" style="margin:6px auto;">
                    <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />

                    <asp:HiddenField ID="hiddenSelectedIDsSentIn" runat="server" Value="false" />

                    <table>

                        <tr style="vertical-align:top;">
                            <td>

                                <center>
                                    <table>

                                        <tr  style="vertical-align:top;">
                                            <td>

                                                <center>
                                                    <table>

                                                        <tr>
                                                            <th colspan="3">1. Choose Method Of Correspondence</th>
                                                        </tr>
                                                        <tr style="height:10px;">
                                                            <td colspan="3"></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Has Both Mobile & Email</td>
                                                            <td style="min-width:6px;"></td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlBothMobileAndEmail" runat="server">
                                                                    <asp:ListItem Text="SMS" Value="3"></asp:ListItem>
                                                                    <asp:ListItem Text="Email" Value="2"></asp:ListItem>
                                                                    <asp:ListItem Text="Print" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="Nothing" Value="-1"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Has Mobile, No Email</td>
                                                            <td></td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlMobileNoEmail" runat="server">
                                                                    <asp:ListItem Text="SMS" Value="3"></asp:ListItem>
                                                                    <asp:ListItem Text="Print" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="Nothing" Value="-1"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Has Email, No Mobile</td>
                                                            <td></td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlEmailNoMobile" runat="server">
                                                                    <asp:ListItem Text="Email" Value="2"></asp:ListItem>
                                                                    <asp:ListItem Text="Print" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="Nothing" Value="-1"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Has Neither Mobile Or Email</td>
                                                            <td></td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlNeitherMobileOrEmail" runat="server">
                                                                    <asp:ListItem Text="Print" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="Nothing" Value="-1"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                 </center>

                                            </td>
                                            <td></td>
                                            <td rowspan="6">

                                                <center>
                                                    <table>
                                                        <tr>
                                                            <th colspan="3">Email & SMS Text Merge Fields</th>
                                                        </tr>
                                                        <tr style="height:10px;">
                                                            <td colspan="3"></td>
                                                        </tr>
                                                        <tr><td>{pt_name}</td><td style="min-width:20px;"></td><td>Patient Full Name</td></tr>
                                                        <tr><td>{pt_title}</td><td></td><td>Patient Title</td></tr>
                                                        <tr><td>{pt_firstname}</td><td></td><td>Patient Firstname</td></tr>
                                                        <tr><td>{pt_middlename}</td><td></td><td>Patient Middlename</td></tr>
                                                        <tr><td>{pt_surname}</td><td></td><td>Patient Surname</td></tr>
                                                        <tr><td colspan="3"></td></tr>
                                                        <tr><td>{org_name}</td><td></td><td>Clinic/Facility Name</td></tr>
                                                        <tr><td>{org_addr}</td><td></td><td>Clinic/Facility Address</td></tr>
                                                        <tr><td>{org_phone}</td><td></td><td>Clinic/Facility Phone Nbr</td></tr>
                                                        <tr><td>{org_office_fax}</td><td></td><td>Clinic/Facility Fax Nbr</td></tr>
                                                        <tr><td>{org_web}</td><td></td><td>Clinic/Facility Website</td></tr>
                                                        <tr><td>{org_email}</td><td></td><td>Clinic/Facility Email Address</td></tr>
                                                    </table>
                                                </center>

                                            </td>
                                        </tr>

                                        <tr style="height:10px;">
                                            <td></td>
                                            <td></td>
                                        </tr>

                                        <tr>
                                            <th>2. Printed Batch Letters - Email Address To Send Document To Print</th>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td><asp:TextBox ID="txtEmailForPrinting" runat="server" Width="525px"></asp:TextBox></td>
                                            <td></td>
                                        </tr>

                                        <tr style="height:30px;">
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th>3a. Email Correspondence - Email Subject</th>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td><asp:TextBox ID="txtEmailSubject" runat="server" Width="525px"></asp:TextBox></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr style="height:10px;">
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th>3b. Email Correspondence - Email Text</th>
                                            <td></td>
                                            <th>4. SMS Correspondence - SMS Text</th>
                                        </tr>
                                        <tr style="vertical-align:top;">
                                            <td style="width:48%;">

                                                <FTB:FreeTextBox id="FreeTextBox1" runat="Server" Text="" Width="525px" Height="200px" />

                                            </td>
                                            <td style="min-width:40px;"></td>
                                            <td style="width:48%;">

                                                <asp:TextBox ID="txtSMSText" runat="server" Width="525px" Height="288px" TextMode="MultiLine"></asp:TextBox>

                                            </td>
                                        </tr>
                                    </table>
                                </center>

                            </td>
                        </tr>

                        <tr style="height:40px;">
                            <td></td>
                        </tr>

                        <tr style="vertical-align:top;">
                            <td>

                                <table id="tbl_select_org_and_patient" runat="server">
                                    <tr>
                                        <th id="td_orgcol_row1" align="left">5. Select Organisation(s)</th>
                                        <th></th>
                                        <th style="text-align:left;">6. Select Patients</th>
                                        <th></th>
                                        <th style="text-align:left;">
                                            <table style="width:100%;">
                                                <tr>
                                                    <th style="text-align:left;">7. Select Letter<br />(Req'd for printing. Optional for email)</th>
                                                    <td style="text-align:right;"><asp:Button ID="btnPrint" runat="server" Text="&nbsp;&nbsp;&nbsp;Run&nbsp;&nbsp;&nbsp;" OnClick="btnPrint_Click" OnClientClick="clear_error_message(); return (confirm_letter_selected() && check_special_fields());" /> </td>
                                                </tr>
                                            </table>
                                        </th>
                                    </tr>

                                    <tr id="tr_orgs_search_row_space_below" runat="server" height="15">
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>

                                    <tr style="vertical-align:bottom;">
                                        <td>
                                            <asp:Button ID="btnAddOrg" runat="server" Text="Add" OnClientClick="javascript:get_organisation(); return false;"/>
                                            <asp:Button ID="btnAddAllOrgs" runat="server" Text="Add All" onclick="btnAddAllOrgs_Click" />
                                            <asp:Button ID="btnDeleteSelected" runat="server" Text="Delete Selected" OnClientClick="javascript:remove_selected_organisation(); return false;" />
                                            <asp:Button ID="btnUpdateOrgs" runat="server" CssClass="hiddencol" onclick="btnUpdateOrgs_Click" />
                                            <asp:HiddenField ID="hiddenOrgIDsList" runat="server" />
                                        </td>
                                        <td style="width:60px"></td>
                                        <td>

                                            <table style="line-height:normal;">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblPatientsNotLinkedToAnyOrg" runat="server"><label for="chkIncPatientsWithNoOrg">Include PTs not linked to any org</label></asp:Label>
                                                    </td>
                                                    <td style="min-width:5px;"></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkIncPatientsWithNoOrg"  runat="server" AutoPostBack="false" />
                                                    </td>
                                                    <td style="min-width:10px;"></td>
                                                    <td rowspan="4">
                                                        <asp:Button ID="btnUpdatePatientList" runat="server" Text="Update" OnClick="btnUpdatePatientList_Click" />
                                                        <asp:Label ID="lblTotalText" runat="server" Text="&nbsp;Total: "></asp:Label>
                                                        <asp:Label ID="lblPatientCount" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblPatientsWithMobileAndEmail" runat="server"><label for="chkPatientsWithMobileAndEmail">Inc. PTs w/ Both Mobile & Email</label></asp:Label>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkPatientsWithMobileAndEmail"  runat="server" AutoPostBack="false" Checked="true" />
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblPatientsWithMobileNoEmail" runat="server"><label for="chkPatientsWithMobileNoEmail">Inc. PTs w/ Mobile, No Email</label></asp:Label>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkPatientsWithMobileNoEmail"  runat="server" AutoPostBack="false" Checked="true" />
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblPatientsWithEmailNoMobile" runat="server"><label for="chkPatientsWithEmailNoMobile">Inc. PTs w/ Email, No Mobile</label></asp:Label>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkPatientsWithEmailNoMobile"  runat="server" AutoPostBack="false" Checked="true" />
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblPatientsWithNeitherMobileOrEmail" runat="server"><label for="chkPatientsWithNeitherMobileOrEmail">Inc. PTs w/ Neither Mobile or Email</label></asp:Label>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkPatientsWithNeitherMobileOrEmail"  runat="server" AutoPostBack="false" Checked="true" />
                                                    </td>
                                                    <td></td>
                                                    <td style="text-align:right;">All/None <input id="chkSelectAllPatients" onclick="select_all(this, 'lstPatients')" type="checkbox" value="Accept Form" name="chkSelectAllPatients" runat="server" /></td>
                                                </tr>
                                            </table>

                                        </td>
                                        <td style="width:60px"></td>
                                        <td class="block_right">
                                            <table align="right">
                                                <tr>
                                                    <td><asp:Label ID="lblOneLetterPerPatientText" runat="server">Max one correspondence per patient&nbsp;</asp:Label></td>
                                                    <td><asp:CheckBox ID="chkOneLetterPerPatient" runat="server" Checked="True" /></td>
                                                </tr>
                                                <tr>
                                                    <td><asp:Label ID="lblOneEmailPerEmailAddress" runat="server">Max one email per email address&nbsp;</asp:Label></td>
                                                    <td><asp:CheckBox ID="chkOneEmailPerEmailAddress" runat="server" Checked="False" /></td>
                                                </tr>
                                                <tr>
                                                    <td><asp:Label ID="lblOneSMSPerMobile" runat="server">Max one SMS per mobile number&nbsp;</asp:Label></td>
                                                    <td><asp:CheckBox ID="chkOneSMSPerMobile" runat="server" Checked="False" /></td>
                                                </tr>
                                            </table>
                                              
                                        </td>
                                    </tr>

                                    <tr>
                                        <td><asp:ListBox ID="lstOrgs" runat="server" rows="32" SelectionMode="Multiple" Width="100%" style="min-width:350px;"></asp:ListBox></td>
                                        <td></td>
                                        <td><asp:ListBox ID="lstPatients" runat="server" rows="32" SelectionMode="Multiple" Width="100%" style="min-width:350px;"></asp:ListBox></td>
                                        <td></td>
                                        <td><asp:ListBox ID="lstLetters" runat="server" rows="32" SelectionMode="Single" style="min-width:350px;"></asp:ListBox></td>
                                    </tr>

                                    <tr>
                                        <td style="text-align:right;display:none;">All/None <input id="chkSelectAllOrgs" onclick="select_all(this, 'lstOrgs')" type="checkbox" value="Accept Form" name="chkSelectAllOrgs" runat="server" /></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </table>

                            </td>
                        </tr>
                    </table>
                                            
                </asp:Panel>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>


        </div>
    </div>


</asp:Content>



