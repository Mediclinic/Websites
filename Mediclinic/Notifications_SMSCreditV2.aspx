<%@ Page Title="SMS Credit" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Notifications_SMSCreditV2.aspx.cs" Inherits="Notifications_SMSCreditV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function notification_info_edited(elem) {

            //elem.style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("txtSMSCreditNotificationEmailAddress").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("txtSMSCreditLowBalance_Threshold").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 

            document.getElementById("btnUpdateNotificationInfo").style.visibility = ""; // make it visible
            document.getElementById("btnRevertNotificationInfo").style.visibility = ""; // make it visible
        }


        function buy_credit_popup() {
            var URL = 'Notifications_AddCreditEmailPopupV2.aspx';
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,Width=600,Height=640");
            NewWindow.location = URL;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">SMS Credit</asp:Label></div>
        <div class="main_content_with_header">

            <div id="sms_credit_div" runat="server" class="user_login_form_no_width" style="width:350px;margin:0px auto 20px;">

                <div class="border_top_bottom user_login_form_no_width_div">

                    <table class="block_center">
                        <tr>
                            <td>

                                <asp:RequiredFieldValidator ID="txtValidateSMSPriceRequired" runat="server" CssClass="failureNotification"  
                                    Display="None"
                                    ControlToValidate="txtSMSPrice" 
                                    ErrorMessage="SMS Price is required."
                                    ValidationGroup="ValidationSummaryEditSMSPrice">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="txtValidateSMSPriceRegex" runat="server" CssClass="failureNotification" 
                                    Display="None"
                                    ControlToValidate="txtSMSPrice"
                                    ValidationExpression="^\d+(\.\d{1,2})?$"
                                    ErrorMessage="SMS Price can only be numbers and option decimal place with 1 or 2 digits following."
                                    ValidationGroup="ValidationSummaryEditSMSPrice">*</asp:RegularExpressionValidator>

                                <asp:Label ID="lblSMSPriceDescr" runat="server" Text="SMS Price" />
                                &nbsp;&nbsp;
                                <asp:TextBox ID="txtSMSPrice" runat="server" Columns="6" />

                                <asp:Button ID="btnSMSPriceSetEditMode" runat="server" Text="Edit" OnClick="btnSMSPriceSetEditMode_Click" />
                                <asp:Button ID="btnSMSPriceUpdate" runat="server" Text="Update" OnClick="btnSMSPriceUpdate_Click" CausesValidation="True" ValidationGroup="ValidationSummaryEditSMSPrice" />
                                <asp:Button ID="btnSMSPriceCancelEditMode" runat="server" Text="Cancel Edit" OnClick="btnSMSPriceCancelEditMode_Click" />
                            </td>
                        </tr>
                    </table>

                </div>

                <asp:ValidationSummary ID="ValidationSummaryEditSMSPrice" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEditSMSPrice"/>

            </div>
            <div class="user_login_form_no_width" style="width:1050px;margin:0px auto 20px;">

                <div class="border_top_bottom user_login_form_no_width_div">

                    <asp:Button ID="btnBuyCreditEmail" runat="server" Text="Buy Credit" OnClientClick="buy_credit_popup(); return false" />
                    
                    <br />
                    <br />

                    <table class="block_center text-left">
                        <tr>
                            <td style="width:100px;"></td>
                            <td>SMS Balance Notification Email:&nbsp;&nbsp;</td>
                            <td></td>
                            <td><asp:TextBox ID="txtSMSCreditNotificationEmailAddress" runat="server" Columns="35" onkeyup="notification_info_edited();"></asp:TextBox></td>
                            <td style="width:20px"></td>
                            <td>Out Of Ballance Email Warning Active:&nbsp;&nbsp;</td>
                            <td><asp:CheckBox ID="chkSMSCreditOutOfBalance_SendEmail" runat="server" onclick="notification_info_edited()" /></td>
                            <td style="width:20px"></td>
                            <td rowspan="2"><asp:Button ID="btnUpdateNotificationInfo" runat="server" Text="Update" OnClick="btnUpdateNotificationInfo_Click" />&nbsp;&nbsp;</td>
                            <td rowspan="2"><asp:Button ID="btnRevertNotificationInfo" runat="server" Text="Revert" OnClick="btnRevertNotificationInfo_Click" /></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>Low Balance Warning Threshold:&nbsp;&nbsp;</td>
                            <td>$&nbsp;</td>
                            <td><asp:TextBox ID="txtSMSCreditLowBalance_Threshold" runat="server" Columns="10" onkeyup="notification_info_edited();"></asp:TextBox></td>
                            <td></td>
                            <td>Low Ballance Email Warning Active:&nbsp;&nbsp;</td>
                            <td><asp:CheckBox ID="chkSMSCreditLowBalance_SendEmail" runat="server" onclick="notification_info_edited()" /></td>
                            <td></td>
                        </tr>
                    </table>

                </div>

            </div>

            <div style="height:15px;"></div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>




            <table class="block_center">
                <tr style="vertical-align:top;">
                    <td>
                        <center>
                            <h4>Topup History</h4>
                            <div style="height:2px;"></div>
                        </center>

                        <div id="autodivheight" class="divautoheight" style="height:500px;min-height:250px;">

                            <asp:GridView ID="GrdSMSCredit" runat="server" 
                                 AutoGenerateColumns="False" DataKeyNames="sms_credit_id" 
                                 OnRowCancelingEdit="GrdSMSCredit_RowCancelingEdit" 
                                 OnRowDataBound="GrdSMSCredit_RowDataBound" 
                                 OnRowEditing="GrdSMSCredit_RowEditing" 
                                 OnRowUpdating="GrdSMSCredit_RowUpdating" ShowFooter="True" 
                                 OnRowCommand="GrdSMSCredit_RowCommand" 
                                 OnRowDeleting="GrdSMSCredit_RowDeleting" 
                                 OnRowCreated="GrdSMSCredit_RowCreated"
                                 AllowSorting="True" 
                                 OnSorting="GridView_Sorting"
                                 AllowPaging="True"
                                 OnPageIndexChanging="GrdSMSCredit_PageIndexChanging"
                                 PageSize="10"
                                 ClientIDMode="Predictable"
                                 CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                 <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                                <Columns> 

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="sms_credit_id"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("sms_credit_id") %>'></asp:Label> 
                                        </ItemTemplate> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("sms_credit_id") %>'></asp:Label>
                                        </EditItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Amount"  HeaderStyle-HorizontalAlign="Left" SortExpression="surname"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("amount")  %>'></asp:Label> 
                                        </ItemTemplate> 
                                        <EditItemTemplate> 
                                            <asp:TextBox ID="txtAmount" runat="server" Text='<%# Bind("amount") %>' Columns="10"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="txtValidateAmountRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtAmount" 
                                                ErrorMessage="Amount is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="txtValidateAmountRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtAmount"
                                                ValidationExpression="^\d+(\.\d{1,2})?$"
                                                ErrorMessage="Amount can only be numbers and optional decimal place with 1 or 2 digits following."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                        </EditItemTemplate> 
                                        <FooterTemplate>
                                            <asp:TextBox ID="txtNewAmount" runat="server" Columns="10"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="txtValidateNewAmountRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtNewAmount" 
                                                ErrorMessage="Amount is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="txtValidateNewAmountRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtNewAmount"
                                                ValidationExpression="^\d+(\.\d{1,2})?$"
                                                ErrorMessage="Amount can only be numbers and optional decimal place with 1 or 2 digits following."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                        </FooterTemplate>
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="phone_number"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblAdded" runat="server" Text='<%# Eval("datetime_added", "{0:dd-MM-yyyy HH:mm:ss}") %>'></asp:Label> 
                                        </ItemTemplate> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblAdded" runat="server" Text='<%# Eval("datetime_added", "{0:dd-MM-yyyy HH:mm:ss}") %>'></asp:Label> 
                                        </EditItemTemplate> 
                                    </asp:TemplateField> 


                                    <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit"></asp:LinkButton> 
                                            <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                        </EditItemTemplate> 
                                        <FooterTemplate> 
                                            <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd"></asp:LinkButton> 
                                        </FooterTemplate> 
                                        <ItemTemplate> 
                                            <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" />


                                </Columns> 

                            </asp:GridView>

                        </div>

                    </td>
                    <td style="width:75px;"></td>
                    <td>

                        <table>
                            <tr>
                                <td colspan="4" style="text-align:center">
                                    <h4>Totals</h4>
                                    <div style="height:2px;"></div>
                                </td>
                            </tr>
                            <tr>
                                <td>Credit</td><td style="width:20px"></td><td style="width:12px">$</td><td><asp:Label ID="lblTotalCredit" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>Used</td><td></td><td>$</td><td><asp:Label ID="lblTotalUsed" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>Remaining</td><td></td><td>$</td><td><asp:Label ID="lblTotalRemaining" runat="server" Font-Bold="True"></asp:Label></td>
                            </tr>
                        </table>

                        <div style="min-height:30px;"></div>

                        <table>
                            <tr>
                                <td colspan="4" style="text-align:center">
                                    <h4>Breakdown By Date</h4>
                                    <div style="height:2px;"></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">Start Date: </asp:Label></td>
                                <td class="nowrap"><asp:TextBox ID="txtStartDate" runat="server" Columns="10"/></td>
                                <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                <td class="nowrap"  style="line-height:normal;"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>

                                <td rowspan="2" style="width:10px;"></td>
                                <td rowspan="2" style="vertical-align:middle"><asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" /></td>
                            </tr>
                            <tr>
                                <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                                <td class="nowrap"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                                <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                <td class="nowrap"  style="line-height:normal;"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>
                            </tr>
                        </table>

                        <div style="height:20px;"></div>

                        <table>
                            <tr>
                                <td>PT Booking Reminders</td><td style="width:20px"></td><td style="width:12px">$</td><td><asp:Label ID="lblPTReminders" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>PT B'days, EPC Used Up, Mass Marketing</td><td></td><td>$</td><td><asp:Label ID="lblPTBirthdays" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>Staff Booking Reminders</td><td></td><td>$</td><td><asp:Label ID="lblStaffReminders" runat="server"></asp:Label></td>
                            </tr>
                        </table>

                    </td>
                </tr>
            </table>


        </div>
    </div>

</asp:Content>



