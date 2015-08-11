<%@ Page Title="Referral Letters - Re-Print" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReferrerEPCLetters_ReprintV2.aspx.cs" Inherits="ReferrerEPCLetters_ReprintV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function get_register_referrer() {

            var retVal = window.showModalDialog("ReferrerListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:1150px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined" || retVal == false)
                return;

            var index = retVal.indexOf(":");
            document.getElementById('registerReferrerID').value = retVal.substring(0, index); // set value so can get from code behind
            document.getElementById('btnRegisterReferrerSelectionUpdate').click();  // call button press to let the code behind use this id and update accordingly
        }
        function clear_register_referrer() {
            document.getElementById('registerReferrerID').value = '-1';
            document.getElementById('btnRegisterReferrerSelectionUpdate').click();  // call button press to let the code behind use this id and update accordingly
        }

        function get_patient() {

            var ref_id = document.getElementById('registerReferrerID').value;
            var retVal = (ref_id == '' || ref_id == '-1') ?
                    window.showModalDialog("PatientListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:750px;resizable:yes;center:yes;") :
                    window.showModalDialog("PatientListPopupV2.aspx?ref=" + ref_id, 'Show Popup Window', "dialogHeight:800px;dialogWidth:750px;resizable:yes;center:yes;");

            //var retVal = window.showModalDialog("PatientListPopupV2.aspx", 'Show Popup Window', "dialogHeight:700px;dialogWidth:1150px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined" || retVal == false)
                return;

            var index = retVal.indexOf(":");
            document.getElementById('patientID').value = retVal.substring(0, index); // set value so can get from code behind
            document.getElementById('btnPatientSelectionUpdate').click();  // call button press to let the code behind use this id and update accordingly
        }
        function clear_patient() {
            document.getElementById('patientID').value = '-1';
            document.getElementById('btnPatientSelectionUpdate').click();  // call button press to let the code behind use this id and update accordingly
        }



        function show_page_load_message() {

            if (!Page_ClientValidate("EditRecepitValidationSummary"))
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

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Referral Letters - Re-Print</span></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:600px;">

                <div class="border_top_bottom user_login_form_no_width_div">
                    <center>

                        <div style="height:8px;"></div>

                        <table class="block_center" style="margin:0 auto;">
                            <tr style="vertical-align:top;line-height:6px;">
                                <td>


                                    <asp:HiddenField ID="registerReferrerID" runat="server" Value="-1" />
                                    <asp:Button ID="btnRegisterReferrerSelectionUpdate" runat="server" CssClass="hiddencol" Text=""  OnClick="btnRegisterReferrerSelectionUpdate_Click" />
                                    <asp:HiddenField ID="patientID" runat="server" Value="-1" />
                                    <asp:Button ID="btnPatientSelectionUpdate" runat="server" CssClass="hiddencol" Text=""  OnClick="btnPatientSelectionUpdate_Click" />


                                    <table id="maintable" runat="server" class="block_center">
                                        <tr>
                                            <td style="min-height:28px;vertical-align:middle;">
                                                <span style="line-height:15px !important;">
                                                <asp:Label style="vertical-align:middle;" ID="lblReferrerText" runat="server" Text="<b>All Referreres</b>" />
                                                </span>
                                            </td>
                                            <td style="width:15px;"></td>
                                            <td style="line-height:normal;">
                                                <asp:Button ID="btnRegisterReferrerListPopup" runat="server" Width="100%" Text="Get Referrer" OnClientClick="javascript:get_register_referrer(); return false;"/>
                                            </td>
                                            <td style="line-height:normal;">
                                                <asp:Button ID="btnClearRegisterReferrer" runat="server" Width="100%" Text="All Referrers" OnClientClick="javascript:clear_register_referrer(); return false;"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="min-height:28px;vertical-align:middle;">
                                                <span style="line-height:15px !important;">
                                                <asp:Label style="vertical-align:middle;" ID="lblPatientText" runat="server" Text="<b>All Patients</b>" />
                                                </span>
                                            </td>
                                            <td style="width:15px;"></td>
                                            <td style="line-height:normal;">
                                                <asp:Button ID="btnPatientListPopup" runat="server" Width="100%" Text="Get Patient" OnClientClick="javascript:get_patient(); return false;"/>
                                            </td>
                                            <td style="line-height:normal;">
                                                <asp:Button ID="btnClearPatient" runat="server" Width="100%" Text="All Patients" OnClientClick="javascript:clear_patient(); return false;"/>
                                            </td>
                                        </tr>
                                    </table>


                                    <div style="line-height:12px;">&nbsp;</div>


                                    <table class="block_center">
                                        <tr>
                                            <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">From:</asp:Label>&nbsp;</td>
                                            <td class="nowrap"><asp:TextBox ID="txtStartDate" runat="server" Columns="10"/>&nbsp;</td>
                                            <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" />&nbsp;</td>
                                            <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>

                                            <td style="width:20px"></td>

                                            <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">To:</asp:Label>&nbsp;</td>
                                            <td class="nowrap"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox>&nbsp;</td>
                                            <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" />&nbsp;</td>
                                            <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>
                                        </tr>
                                    </table>


                                    <div style="line-height:12px;">&nbsp;</div>






                                    <table id="submittable" runat="server" class="block_center">
                                        <tr>
                                            <td>
                                                <asp:RadioButtonList id="rdioSendType" runat="server">
                                                    <asp:ListItem Value="Email">Email (print if no referrer email / fax set)</asp:ListItem>
                                                    <asp:ListItem Value="Print" Selected="True">Print (regardless of whether referrer has email set)</asp:ListItem>
                                                </asp:RadioButtonList>
                                                <asp:RequiredFieldValidator   
                                                    ID="rdioSendTypeReqiredFieldValidator"  
                                                    runat="server"  
                                                    ControlToValidate="rdioSendType"  
                                                    ErrorMessage="Please select a method of sending."
                                                    ValidationGroup="EditRecepitValidationSummary" Display="None"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr style="height:12px">
                                            <td colspan="5"></td>
                                        </tr>
                                        <tr>
                                            <td style="text-align:center;line-height:normal;">
                                                <asp:Button ID="btnViewList" runat="server" Text="Refresh List" onclick="btnViewList_Click"  />
                                                &nbsp;&nbsp;&nbsp;
                                                <asp:Button ID="btnSubmit" runat="server" Text="Re-Print" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="EditRecepitValidationSummary" OnClientClick="show_page_load_message();" />
                                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="window.returnValue=false;self.close();return false;" Visible="False" />
                                                <br />
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align:center;">
                                                <table>
                                                    <tr style="vertical-align:middle">
                                                        <td>
                                                            * Please note that this could take some time to complete. 
                                                            <div style="height:8px;"></div>
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
                            </tr>
                        </table>

                        <div style="height:8px;"></div>

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



