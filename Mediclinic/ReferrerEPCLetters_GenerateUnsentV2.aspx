<%@ Page Title="Referral Letters - Generate Unsent" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReferrerEPCLetters_GenerateUnsentV2.aspx.cs" Inherits="ReferrerEPCLetters_GenerateUnsentV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function get_register_referrer() {

            var retVal = window.showModalDialog("ReferrerListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:1150px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined")
                return;

            var index = retVal.indexOf(":");
            document.getElementById('registerReferrerID').value = retVal.substring(0, index); // set value so can get from code behind
            document.getElementById('btnRegisterReferrerSelectionUpdate').click();  // call button press to let the code behind use this id and update accordingly
        }
        function clear_register_referrer() {
            document.getElementById('registerReferrerID').value = '-1';
            document.getElementById('btnRegisterReferrerSelectionUpdate').click();  // call button press to let the code behind use this id and update accordingly
        }

        function show_page_load_message() {

            if (!Page_ClientValidate("EPCValidationSummary"))
                return;

            // delay so it is only shown if it is taking a long time (ie if a last treatment letter is being generated)
            setTimeout(function () {
                show_hide('loadingDiv', true);
            }, 750);
        }
        function show_hide(id, show) {
            obj = document.getElementById(id);
            obj.style.display = show ? "" : "none";
        }

        function notification_info_edited() {

            //elem.style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkEnableEmails").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("txtEmailAddress").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 

            document.getElementById("chkIncClinicsAuto").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkIncAgedCareAuto").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 

            document.getElementById("chkIncUnsentAuto").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkIncBatchedAuto").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 

            document.getElementById("chkSendMondays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendTuesdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendWednesdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendThursdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendFridays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendSaturdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendSundays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 

            document.getElementById("btnUpdateNotificationInfo").className = ""; // make it visible
            document.getElementById("btnRevertNotificationInfo").className = ""; // make it visible
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Referral Letters - Generate Unsent</span></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:1200px;">

                <div class="border_top_bottom user_login_form_no_width_div">
                    <center>

                        <div style="height:8px;"></div>

                        <table class="block_center" style="margin:0 auto;">
                            <tr style="vertical-align:top;line-height:6px;">
                                <td>


                                    <asp:HiddenField ID="registerReferrerID" runat="server" Value="-1" />
                                    <asp:Button ID="btnRegisterReferrerSelectionUpdate" runat="server" CssClass="hiddencol" Text=""  OnClick="btnRegisterReferrerSelectionUpdate_Click" />

                                    <table id="maintable" runat="server" class="block_center">
                                        <tr style="vertical-align:middle;">
                                            <td class="nowrap">Generate Unsent Letters for:</td>
                                            <td style="width:10px;"></td>
                                            <td style="min-height:28px;vertical-align:middle;">
                                                <span style="line-height:15px !important;">
                                                <asp:Label style="vertical-align:middle;" ID="lblReferrerText" runat="server" Text="<b>All Referreres</b>" />
                                                </span>
                                            </td>
                                            <td style="width:15px;"></td>
                                            <td class="nowrap" style="line-height:normal;">
                                                <asp:Button ID="btnRegisterReferrerListPopup" runat="server" Text="Get Referrer" OnClientClick="javascript:get_register_referrer(); return false;"/>
                                                <asp:Button ID="btnClearRegisterReferrer" runat="server" Text="All Referrers" OnClientClick="javascript:clear_register_referrer(); return false;"/>
                                            </td>
                                        </tr>
                                    </table>




                                    <table id="submittable" runat="server" style="min-width:505px;">
                                        <tr id="select_sites_row" runat="server">
                                            <td>
                                                <asp:CheckBox ID="chkIncClinics" runat="server" Checked="True" Text="" Font-Bold="false" />&nbsp;<label for="chkIncClinics" style="font-weight:normal;">Include Clinics</label>
                                                <br />
                                                <asp:CheckBox ID="chkIncAgedCare" runat="server" Checked="True" Text="" />&nbsp;<label for="chkIncAgedCare" style="font-weight:normal;">Include Aged Care</label>
                                            </td>
                                        </tr>
                                        <tr id="select_sites_row_trailingspace" runat="server" style="height:10px">
                                            <td colspan="5"></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chkIncUnsent" runat="server" Checked="True" Text="" />&nbsp;<label for="chkIncUnsent" style="font-weight:normal;">Generate Unsent (not including referrer-requested batch sending)</label>
                                                <br />
                                                <asp:CheckBox ID="chkIncBatching" runat="server" Checked="True" Text="" />&nbsp;<label for="chkIncBatching" style="font-weight:normal;">Generate Referrer-Requested Batch Sending</label>
                                            </td>
                                        </tr>
                                        <tr style="height:10px">
                                            <td colspan="5"></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:RadioButtonList id="rdioSendType" runat="server" style="all: none;">
                                                    <asp:ListItem Value="Print">&nbsp;Batch & Print All</asp:ListItem>
                                                    <asp:ListItem Value="Email" Selected="True">&nbsp;Email Direct to Referrers (batch & print where no ref. email set)</asp:ListItem>
                                                </asp:RadioButtonList>
                                                <asp:RequiredFieldValidator   
                                                    ID="rdioSendTypeReqiredFieldValidator"  
                                                    runat="server"  
                                                    ControlToValidate="rdioSendType"  
                                                    ErrorMessage="Please select a method of sending."
                                                    ValidationGroup="EPCValidationSummary" Display="None"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr style="height:10px">
                                            <td colspan="5"></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chkIncWithEmailOrFaxOnly" runat="server" Text="" />&nbsp;<label for="chkIncWithEmailOrFaxOnly" style="font-weight:normal;">Include Only Where Ref Email Set</label>
                                            </td>
                                        </tr>
                                        <tr style="height:10px">
                                            <td colspan="5"></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chkShowFullList" runat="server" Text="" />&nbsp;<label for="chkShowFullList" style="font-weight:normal;">Display All (Not Only The First <%= ReferrerEPCLettersSending.MaxSending %> Which Will Be Processed At A Time)</label>
                                            </td>
                                        </tr>
                                        <tr style="height:6px">
                                            <td colspan="5"></td>
                                        </tr>
                                        <tr>
                                            <td style="line-height:normal;text-align:center;">
                                                <asp:Button ID="btnViewList" runat="server" Text="Refresh List" OnClick="btnViewList_Click"  />
                                                &nbsp;&nbsp;&nbsp;
                                                <asp:Button ID="btnSubmit" runat="server" Text="Generate" OnClick="btnSubmit_Click" CausesValidation="True" ValidationGroup="EPCValidationSummary" OnClientClick="show_page_load_message();" />
                                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="window.returnValue=false;self.close();return false;" Visible="False" />
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <table>
                                                    <tr valign="middle">
                                                        <td>
                                                            <div style="min-height:8px;"></div>
                                                            * Please note that this could take some time to complete. 
                                                        </td>
                                                        <td>
                                                            <span id="loadingDiv" runat="server" style="display:none">
                                                                <img src="images/loading_circle_small.gif" alt="Loading..."/>
                                                            </span>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>


                                </td>

                                <td>
                                    <div style="width: 1px !important; height:310px; background: #999; margin:0 45px;"></div>
                                </td>

                                <td>

                                    <table style="min-width:600px;" >
                                        <tr style="line-height:25px;">
                                            <th colspan="3" style="text-align:center;">Automated Settings</th>
                                        </tr>
                                        <tr style="height:4px">
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <th colspan="3">
                                                <asp:ValidationSummary ID="EPCValidationSummaryAutoSending" runat="server" ForeColor="Red" Font-Bold="true" ValidationGroup="EPCValidationSummary"/>
                                                <asp:Label ID="lblErrorMessageAutoSending" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td>Enable</td>
                                            <td style="width:15px"></td>
                                            <td><input type="checkbox" id="chkEnableEmails" runat="server" value="Accept Form" onclick="notification_info_edited()" /></td>
                                        </tr>
                                        <tr style="height:8px">
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td>Email</td>
                                            <td></td>
                                            <td><asp:TextBox ID="txtEmailAddress" runat="server" Columns="50" onkeyup="notification_info_edited();"></asp:TextBox></td>
                                        </tr>
                                        <tr style="height:8px">
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr id="select_sites_row_auto" runat="server">
                                            <td>Sites</td>
                                            <td></td>
                                            <td>
                                                <input type="checkbox" id="chkIncClinicsAuto" checked="checked" runat="server" value="Accept Form" onclick="notification_info_edited()" />
                                                <label for="chkIncClinicsAuto">Include Clinics</label>  
                                                <br />
                                                <input type="checkbox" id="chkIncAgedCareAuto" checked="checked" runat="server" value="Accept Form" onclick="notification_info_edited()" />
                                                <label for="chkIncAgedCareAuto">Include Aged Care</label>
                                            </td>
                                        </tr>
                                        <tr id="select_sites_row_trailingspace_auto" runat="server" style="height:8px">
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td class="nowrap" style="vertical-align:top;"><br />What to Generate</td>
                                            <td></td>
                                            <td>
                                                <input type="checkbox" id="chkIncUnsentAuto" checked="checked" runat="server" value="Accept Form" onclick="notification_info_edited()" />
                                                <label for="chkIncUnsentAuto">Generate Unsent (not inc. referrer-requested batch sending)</label>  
                                                <br />
                                                <input type="checkbox" id="chkIncBatchedAuto" runat="server" value="Accept Form" onclick="notification_info_edited()" />
                                                <label for="chkIncBatchedAuto">Generate Referrer-Requested Batch Sending</label>
                                            </td>
                                        </tr>
                                        <tr style="height:8px">
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td class="nowrap" style="vertical-align:top;"><br />Send Method</td>
                                            <td></td>
                                            <td>

                                                <asp:RadioButtonList id="rdioSendTypeAuto" runat="server">
                                                    <asp:ListItem Value="Print">&nbsp;Batch & Email All To Above Address</asp:ListItem>
                                                    <asp:ListItem Value="Email" Selected="True">&nbsp;Email Direct to Referrers (batch & email where no ref. email set)</asp:ListItem>
                                                </asp:RadioButtonList>
                                                <asp:RequiredFieldValidator   
                                                    ID="rdioSendTypeAutoReqiredFieldValidator"  
                                                    runat="server"  
                                                    ControlToValidate="rdioSendTypeAuto"  
                                                    ErrorMessage="Please select a method of sending."
                                                    ValidationGroup="EPCValidationSummary" Display="None"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr style="height:8px">
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td class="nowrap" style="vertical-align:top;"><br />Days To Send</td>
                                            <td></td>
                                            <td>

                                                <table>
                                                    <tr>
                                                        <td class="nowrap"><input type="checkbox" id="chkSendMondays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;<label for="chkSendMondays">Mondays</label></td>
                                                        <td style="min-width:20px;"></td>
                                                        <td class="nowrap"><input type="checkbox" id="chkSendFridays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;<label for="chkSendFridays">Fridays</label></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="nowrap"><input type="checkbox" id="chkSendTuesdays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;<label for="chkSendTuesdays">Tuesdays</label></td>
                                                        <td></td>
                                                        <td class="nowrap"><input type="checkbox" id="chkSendSaturdays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;<label for="chkSendSaturdays">Saturdays</label></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="nowrap"><input type="checkbox" id="chkSendWednesdays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;<label for="chkSendWednesdays">Wednesdays</label></td>
                                                        <td></td>
                                                        <td class="nowrap"><input type="checkbox" id="chkSendSundays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;<label for="chkSendSundays">Sundays</label></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="nowrap"><input type="checkbox" id="chkSendThursdays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;<label for="chkSendThursdays">Thursdays</label></td>
                                                        <td></td>
                                                        <td></td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                        <tr style="height:10px">
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="text-align:center;">
                                                <div class="block_center" style="margin:0 0 0;line-height:normal;">
                                                    <asp:Button ID="btnUpdateNotificationInfo" runat="server" Text="Update" OnClick="btnUpdateNotificationInfo_Click" />
                                                    &nbsp;&nbsp;
                                                    <asp:Button ID="btnRevertNotificationInfo" runat="server" Text="Revert" OnClick="btnRevertNotificationInfo_Click" />
                                                </div>
                                            </td>
                                        </tr>

                                    </table>


                                </td>
                            </tr>
                        </table>

                    </center>
                </div>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>

            </div>

            <center>
                <asp:Label ID="lblInfo" runat="server" ForeColor="Blue"></asp:Label>

                <div style="height:8px;"></div>

                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:Label ID="lblList" runat="server" ForeColor="Blue"></asp:Label>

                </div>
            </center>
            

        </div>
    </div>

</asp:Content>



