<%@ Page Title="Staff Detail" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="HealthCardDetailV2.aspx.cs" Inherits="HealthCardDetailV2" %>
<%@ Register TagPrefix="UC" TagName="PatientReferrerControl" Src="~/Controls/PatientReferrerControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        function card_nbr_changed() {
            if (document.getElementById('txtCardNbr').value.length == 10)
                document.getElementById('card_nbr_tick').style.display = "block";
            else
                document.getElementById('card_nbr_tick').style.display = "none";
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
                if (!istextbox || newbox.value.length == 0) {
                    newbox.focus();
                }


                var all_digits_entered = document.getElementById('txtCardNbr_Digit_1').value.length == 1 &&
                                         document.getElementById('txtCardNbr_Digit_2').value.length == 1 &&
                                         document.getElementById('txtCardNbr_Digit_3').value.length == 1 &&
                                         document.getElementById('txtCardNbr_Digit_4').value.length == 1 &&
                                         document.getElementById('txtCardNbr_Digit_5').value.length == 1 &&
                                         document.getElementById('txtCardNbr_Digit_6').value.length == 1 &&
                                         document.getElementById('txtCardNbr_Digit_7').value.length == 1 &&
                                         document.getElementById('txtCardNbr_Digit_8').value.length == 1 &&
                                         document.getElementById('txtCardNbr_Digit_9').value.length == 1 &&
                                         document.getElementById('txtCardNbr_Digit_10').value.length == 1;
                document.getElementById('card_nbr_tick').style.display = all_digits_entered ? "block" : "none";
            }
        }
        function show_modal_updade_epc(health_card_id) {

            if (!ajax_has_active_referrer()) {
                alert('Please make sure you have set a reffering doctor before you add ' + (is_aged_care() ? 'an EPC' : 'a referral'));
                return;
            }

            var isMobileDevice = document.getElementById('hiddenIsMobileDevice').value == "1";
            if (!isMobileDevice) {
                // show modal popup
                var result = window.showModalDialog('EPCDetailV2.aspx?type=add&id=' + health_card_id, '', 'dialogWidth:550px;dialogHeight:520px;center:yes;resizable:no; scroll:no');

                // popup download file window in case letter to print
                if (result == true)
                    window.showModalDialog('DownloadFile.aspx', '', 'dialogWidth:10px;dialogHeight:10px;resizable:no; scroll:no');
            }
            else {
                open_new_tab('EPCDetailV2.aspx?type=add&id=' + health_card_id);
            }

            return false;
        }
        function ScrollEPCHistoryToBottom() {
            var divChat = document.getElementById('pnlChangeHistoryList');
            divChat.scrollTop = divChat.scrollHeight;
        }


        function ajax_has_active_referrer() {
            var url_id = getUrlVars()["id"];
            if (url_id == undefined) {
                alert("No url field 'id'");
                return;
            }
            var url_type = getUrlVars()["type"];
            if (url_type == undefined) {
                alert("No url field 'type'");
                return;
            }
            var url_id_type = (url_type == "add") ? "patient" : "healthcard";

            var url_params = new Array();
            url_params[0] = create_url_param("id", url_id);
            url_params[1] = create_url_param("id_type", url_id_type);

            var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
            xmlhttp.open("GET", "/AJAX/AjaxHasActiveReferrer.aspx?" + create_url_params(url_params), false);
            xmlhttp.send();

            var response = String(xmlhttp.responseText);
            if (response == "SessionTimedOutException")
                window.location.href = window.location.href;  // reload page

            if (response != "0" && response != "1")
                alert("Error: \n\r" + response);
            return response != "0"; // if neither '1' or '0' will say already have other card and popup causing alert to be error
        }


        /* -- referrer control -- */

        function update_referrers() {
            window.showModalDialog('ReferrerAddV2.aspx', '', 'dialogWidth:1250px;dialogHeight:800px;center:yes;resizable:no; scroll:no');
            document.getElementById('btnUpdateReferrersList').click();
        }
        function get_register_referrer() {
            var isMobileDevice = document.getElementById('hiddenIsMobileDevice').value == "1";
            if (isMobileDevice)
                open_new_tab('ReferrerListPopupV2.aspx');
            else
                window.showModalDialog("ReferrerListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:1150px;resizable:yes;center:yes;");
        }
        function clear_register_referrer() {
            document.getElementById('txtUpdateRegisterReferrerID').value = '';
        }
        function set_register_referrer(retVal) {
            var index = retVal.indexOf(":");
            document.getElementById('txtUpdateRegisterReferrerID').value = retVal.substring(0, index); // set value so can get from code behind
            document.getElementById('btnRegisterReferrerUpdate').click();  // call button press to let the code behind use this id and update accordingly
        }


        /* -- is_active checkbox -- */

        function on_check_is_active(chkBox) {
            if (!chkBox.checked)
                return;
            var already_has_other_active_healthcard = ajax_already_has_other_active_healthcard();
            if (already_has_other_active_healthcard) {
                if (!confirm('Default card already exists - are you sure you want change to this as the default card to use?'))
                    chkBox.checked = false;  // if false uncheck this box (else check will auto set other card inactive when submitting form)
            }
        }
        function ajax_already_has_other_active_healthcard() {
            var url_id = getUrlVars()["id"];
            if (url_id == undefined) {
                alert("No url field 'id'");
                return;
            }
            var url_type = getUrlVars()["type"];
            if (url_type == undefined) {
                alert("No url field 'type'");
                return;
            }
            var url_cardtype = getUrlVars()["card"];
            if (url_cardtype == undefined) {
                alert("No url field 'card'");
                return;
            }

            var url_params = new Array();
            url_params[0] = create_url_param("id", url_id);
            url_params[1] = create_url_param("type", url_type);
            url_params[2] = create_url_param("cardtype", url_cardtype);

            var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
            xmlhttp.open("GET", "/AJAX/AjaxAlreadyHasOtherActiveHealthcard.aspx?" + create_url_params(url_params), false);
            xmlhttp.send();

            var response = String(xmlhttp.responseText);
            if (response == "SessionTimedOutException")
                window.location.href = window.location.href;  // reload page
            if (response != "0" && response != "1")
                alert("Error: \n\r" + response);
            return response != "0"; // if neither '1' or '0' will say already have other card and popup causing alert to be error
        }
        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }
        function create_url_params(list) {
            var url = "";
            for (var i = 0; i < list.length; i++) {
                url = url + "&" + list[i].n + "=" + list[i].v;
            }
            return (url.length > 0) ? url.substr(1, url.length - 1) : url;
        }
        function create_url_param(n, v) {
            var item = new Array();
            item.n = n;
            item.v = v;
            return item;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

        <div class="clearfix">
            <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server" Text="Health Card Information For" />&nbsp;<asp:HyperLink ID="lnkToEntity" runat="server"></asp:HyperLink></h3></div>
            <div class="main_content">

                <asp:HiddenField ID="hiddenIsMobileDevice" runat="server" Value="0" />

                <div class="text-center">
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                    <asp:ValidationSummary ID="EditHealthCardActionValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="EditHealthCardActionValidationSummary"/>
                    <asp:ValidationSummary ID="EditHealthCardValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="EditHealthCardValidationSummary"/>
                    <asp:ValidationSummary ID="AddHealthCardActionValidationGroup" runat="server" CssClass="failureNotification" ValidationGroup="AddHealthCardActionValidationGroup"/>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>



                <table id="maintable" runat="server" class="block_center">
                    <tr>
                        <td colspan="3" style="text-align:center;">
                            <h3><asp:HyperLink ID="lnkGoToBookingScreen" runat="server" Text="Make Booking" /></h3>
                            <div style="min-height:20px;"></div>
                        </td>
                    </tr>
                    <tr style="vertical-align:top;">
                        <td>
                            <table>
                                <tr id="idRow" runat="server">
                                    <td>ID</td>
                                    <td></td>
                                    <td style="width:200px"><asp:Label ID="lblId" runat="server"></asp:Label></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblOrganisationText" runat="server" Text="Organisation"></asp:Label></td>
                                    <td style="min-width:12px"></td>
                                    <td style="width:200px">
                                        <asp:DropDownList ID="ddlOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id"></asp:DropDownList>
                                        <asp:Label ID="lblOrganisation" runat="server"></asp:Label>
                                    </td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Card Name</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtCardName" runat="server"></asp:TextBox><asp:Label ID="lblCardName" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateCardNameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtCardName"
                                            ValidationExpression="^[a-zA-Z\-\s]+$"
                                            ErrorMessage="CardName can only be letters or hyphens."
                                            Display="Dynamic"
                                            ValidationGroup="EditHealthCardValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblCardNbrText" runat="server" Text="Card Nbr"/></td>
                                    <td></td>
                                    <td>
                                        <span style="white-space:nowrap;">
                                            <asp:TextBox ID="txtCardNbr_Digit_1"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtCardNbr_Digit_2');"></asp:TextBox><asp:TextBox ID="txtCardNbr_Digit_2"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtCardNbr_Digit_3');"></asp:TextBox><asp:TextBox ID="txtCardNbr_Digit_3"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtCardNbr_Digit_4');"></asp:TextBox><asp:TextBox ID="txtCardNbr_Digit_4"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtCardNbr_Digit_5');"></asp:TextBox><asp:TextBox ID="txtCardNbr_Digit_5"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtCardNbr_Digit_6');"></asp:TextBox><asp:TextBox ID="txtCardNbr_Digit_6"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtCardNbr_Digit_7');"></asp:TextBox><asp:TextBox ID="txtCardNbr_Digit_7"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtCardNbr_Digit_8');"></asp:TextBox><asp:TextBox ID="txtCardNbr_Digit_8"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtCardNbr_Digit_9');"></asp:TextBox><asp:TextBox ID="txtCardNbr_Digit_9"  runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'txtCardNbr_Digit_10');"></asp:TextBox><asp:TextBox ID="txtCardNbr_Digit_10" runat="server" MaxLength="1" Width="14px" onkeyup="javascript:refocus_medicare_digits(this,'ddlCardFamilyMemberNbr');"></asp:TextBox>
                                            <asp:TextBox ID="txtCardNbr" runat="server" onkeyup="javascript: card_nbr_changed();"></asp:TextBox><asp:Label ID="lblCardNbrFamilyNbrSeperator" runat="server"> - </asp:Label>
                                            <asp:DropDownList ID="ddlCardFamilyMemberNbr" runat="server"></asp:DropDownList>
                                            <asp:Label ID="lblCardNbr" runat="server" Font-Bold="True"/>
                                        </span>
                                    </td>
                                    <td>
                                        <asp:CustomValidator ID="txtValidateCardNbrsRequired" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="txtCardNbr_Digit_1"
                                            OnServerValidate="CardNbrsRequiredCheck"
                                            ErrorMessage="All 10 medicare card digits must be entered"
                                            Display="Dynamic"
                                            ValidationGroup="EditHealthCardValidationSummary">*</asp:CustomValidator>
                                        <asp:RequiredFieldValidator ID="txtValidateCardNbrRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtCardNbr" 
                                            ErrorMessage="CardNbr is required."
                                            Display="Dynamic"
                                            ValidationGroup="EditHealthCardValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateCardNbrRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtCardNbr"
                                            ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                            ErrorMessage="Card Number can only be letters, numbers and hyphens"
                                            Display="Dynamic"
                                            ValidationGroup="EditHealthCardValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td style="width:15px"><img id="card_nbr_tick" src="images/tick-10.png" style="display: none;"  alt=""/></td>
                                </tr>
                                <tr>
                                    <td>Exp. Date</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlExpiry_Month" runat="server" />
                                        <asp:DropDownList ID="ddlExpiry_Year" runat="server" />
                                        <asp:Label ID="lblExpiry" runat="server" Font-Bold="True" />
                                    </td>
                                    <td></td>
                                </tr>
                                <tr id="area_treated" runat="server">
                                    <td>Area Treated</td>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtAreaTreated" runat="server"/>
                                        <asp:Label ID="lblAreaTreated" runat="server" Font-Bold="true"/>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Default Card</td>
                                    <td></td>
                                    <td>
                                        <asp:CheckBox ID="chkIsActive" runat="server" onclick="on_check_is_active(this);" />
                                        <asp:Label ID="lblIsActive" runat="server" Font-Bold="True"/>
                                    </td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr id="addOrModByRow" runat="server">
                                    <td class="nowrap">Last Modified By</td>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblLastModBy" runat="server"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr id="addOrModDateRow" runat="server">
                                    <td class="nowrap">Last Modified</td>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblLastModDate" runat="server"></asp:Label></td>
                                    <td></td>
                                </tr>

                                <tr>
                                    <td colspan="5" align="center">
                                        <br />  
                                        <asp:Button ID="btnSubmit" runat="server" Text="Button" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="EditHealthCardValidationSummary" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" Visible="False" />
                                        <br />              
                                    </td>
                                </tr>

                                <tr style="height:30px;">
                                    <td colspan="5"></td>
                                </tr>
                                <tr id="tr_epc_heading" runat="server" >
                                    <td colspan="5">
                                        <b><asp:Label ID="lblEPCInfoText" runat="server">EPC Info</asp:Label></b> &nbsp;&nbsp;&nbsp;
                                        <asp:HyperLink ID="lnkEditEPC" runat="server" Text="Edit" />
                                        &nbsp;&nbsp;
                                        <asp:HyperLink ID="lnkNewEPC" runat="server" Text="Add" />
                                        <asp:Button ID="btnUpdateEPCInfo" runat="server" CssClass="hiddencol" onclick="btnUpdateEPCInfo_Click" />
                                    </td>
                                </tr>
                                <tr id="tr_epc_space_row" runat="server" height="10">
                                    <td colspan="5"></td>
                                </tr>
                                <tr id="tr_referral_received" runat="server">
                                    <td>Referral Received</td>
                                    <td></td>
                                    <td><asp:Label ID="lblDateReferralReceived" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr id="tr_referral_signed" runat="server">
                                    <td>Referral Signed</td>
                                    <td></td>
                                    <td><asp:Label ID="lblDateReferralSigned" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr height="10">
                                    <td colspan="5"></td>
                                </tr>


                                <asp:Repeater id="lstEPCRemaining" runat="server">
                                    <HeaderTemplate></HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="nowrap"><asp:Label ID="lblDate" Text='<%# Eval("field_descr") + " services left"  %>' runat="server"></asp:Label></td>
                                            <td></td>
                                            <td class="nowrap"><asp:Label ID="lblStaff" Text='<%# Bind("epcremaining_num_services_remaining") %>' Font-Bold="True" runat="server"></asp:Label></td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></FooterTemplate>
                                </asp:Repeater>

                                <tr id="tr_epc_change_remaining_space_row" runat="server" height="20">
                                    <td colspan="5"></td>
                                </tr>

                                <tr id="tr_epc_change_history" runat="server" visible="False">
                                    <td colspan="5">

                                        <br />
                                        <asp:Repeater id="lstEPCChangeHistory" runat="server">
                                            <HeaderTemplate>

                                                <strong><asp:Label ID="lblEPCChangeHistoryText" runat="server">EPC Change History</asp:Label></strong>
                                                <br /><br />
                                                <div id="pnlChangeHistoryList" style="overflow:auto;width:104%;max-height:190px; padding-right:1px; ">
                                                <table cellpadding="2" border="1">
                                                <tr>
                                                    <th>Date</th>
                                                    <th>Staff</th>
                                                    <th>Descr</th>
                                                    <th>Before</th>
                                                    <th>After</th>
                                                </tr>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td colspan="5" bgcolor="#D3D3D3" <%# (Eval("is_new_epc_card_set").ToString() != "True") ? "style='display:none'" : ""  %>'><center><asp:Label ID="lblNewEpcAdded" Text='<%# Eval("new_referral_added_text") %>' runat="server" Font-Bold="True" BackColor="LightGray"/></center></td>
                                                    <td class="nowrap" <%# (Eval("is_new_epc_card_set").ToString() == "True") ? "style='display:none'" : ""  %>'><asp:Label ID="lblDate" Text='<%# Bind("date") %>' runat="server"/></td>
                                                    <td class="nowrap" <%# (Eval("is_new_epc_card_set").ToString() == "True") ? "style='display:none'" : ""  %>'><asp:Label ID="lblStaff" Text='<%# Bind("staff_name") %>' runat="server"/></td>
                                                    <td class="nowrap" <%# (Eval("is_new_epc_card_set").ToString() == "True") ? "style='display:none'" : ""  %>'><asp:Label ID="lblDesc" Text='<%# Bind("desc") %>' runat="server"/></td>
                                                    <td class="nowrap" <%# (Eval("is_new_epc_card_set").ToString() == "True") ? "style='display:none'" : ""  %>'><asp:Label ID="Label2" Text='<%# Bind("before") %>' runat="server"/></td>
                                                    <td class="nowrap" <%# (Eval("is_new_epc_card_set").ToString() == "True") ? "style='display:none'" : ""  %>'><asp:Label ID="Label3" Text='<%# Bind("after") %>' runat="server"/></td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </table>
                                                </div>
                                            </FooterTemplate>
                                        </asp:Repeater>
                        
                                    </td>
                                </tr>

                                <tr id="tr_epc_change_history_space_row" runat="server" height="40">
                                    <td colspan="5"></td>
                                </tr>

                            </table>

                        </td>
                        <td style="width:30px">
                        </td>
                        <td>


                            <UC:PatientReferrerControl ID="patientReferrer" runat="server" />

                            <br />

                            <asp:Label ID="lblActions" runat="server" Font-Bold="true">&nbsp;EPC Letters Sent<br /></asp:Label>

                            <div style="line-height:8px;">&nbsp;</div>

                            <asp:Panel ID="pnlGrdHealthCardAction" runat="server" ScrollBars="Auto" Height="300" style="padding-right:25px;"> <%-- if more stuff under here, change height to 270 or so --%>
                            <asp:GridView ID="GrdHealthCardAction" runat="server" 
                                    AutoGenerateColumns="False" DataKeyNames="health_card_action_id" 
                                    OnRowCancelingEdit="GrdHealthCardAction_RowCancelingEdit" 
                                    OnRowDataBound="GrdHealthCardAction_RowDataBound" 
                                    OnRowEditing="GrdHealthCardAction_RowEditing" 
                                    OnRowUpdating="GrdHealthCardAction_RowUpdating" ShowFooter="True" ShowHeader="False"
                                    OnRowCommand="GrdHealthCardAction_RowCommand" 
                                    OnRowDeleting="GrdHealthCardAction_RowDeleting" 
                                    OnRowCreated="GrdHealthCardAction_RowCreated"
                                    AllowSorting="True" 
                                    OnSorting="GrdHealthCardAction_Sorting" 
                                    GridLines="None"
                                    ClientIDMode="Predictable">

                                <Columns> 

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="health_card_action_id" ItemStyle-CssClass="gridviewcol_spaceright nowrap"> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("health_card_action_id") %>'></asp:Label> &nbsp;
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("health_card_action_id") %>'></asp:Label> &nbsp;
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="action_date" FooterStyle-CssClass="gridviewcol_spaceright nowrap"> 
                                        <EditItemTemplate> 
                                            <asp:DropDownList ID="ddlActionDate_Day" runat="server"></asp:DropDownList>
                                            <asp:DropDownList ID="ddlActionDate_Month" runat="server"></asp:DropDownList>
                                            <asp:DropDownList ID="ddlActionDate_Year" runat="server"></asp:DropDownList>
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblActionDate" runat="server" Text='<%# Bind("action_date", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                        </ItemTemplate> 
                                        <FooterTemplate>
                                            <asp:DropDownList ID="ddlNewActionDate_Day" runat="server"></asp:DropDownList>
                                            <asp:DropDownList ID="ddlNewActionDate_Month" runat="server"></asp:DropDownList>
                                            <asp:DropDownList ID="ddlNewActionDate_Year" runat="server"></asp:DropDownList>
                                        </FooterTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" FooterStyle-VerticalAlign="Top" FooterStyle-CssClass="gridviewcol_spaceright"> 
                                        <EditItemTemplate> 
                                            <asp:DropDownList ID="ddlHealthCardActionType" runat="server" DataTextField="descr" DataValueField="health_card_action_type_id"> </asp:DropDownList> 
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblHealthCardActionType" runat="server" Text='<%# Eval("descr") %>'></asp:Label> 
                                        </ItemTemplate> 
                                        <FooterTemplate> 
                                            <asp:DropDownList ID="ddlNewHealthCardActionType" runat="server" DataTextField="descr" DataValueField="health_card_action_type_id"> </asp:DropDownList>
                                        </FooterTemplate> 
                                    </asp:TemplateField> 


                                    <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditHealthCardActionValidationSummary"></asp:LinkButton> 
                                            <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                        </EditItemTemplate> 
                                        <FooterTemplate> 
                                            <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="AddHealthCardActionValidationGroup"></asp:LinkButton> 
                                        </FooterTemplate> 
                                        <ItemTemplate> 
                                            <%-- <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton> --%>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                </Columns> 

                            </asp:GridView>
                            </asp:Panel>

                        </td>
                    </tr>
                </table>


                <br />
                <br />
                <asp:HyperLink ID="lnkThisPatient" runat="server" NavigateUrl="~/PatientInfo.aspx?id=">Edit</asp:HyperLink> 

                <div id="autodivheight" class="divautoheight" style="height:500px;">
                </div>

  
            </div>
        </div>

</asp:Content>



