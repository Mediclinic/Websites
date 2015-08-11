<%@ Page Title="Site Information" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="SiteDetailV2.aspx.cs" Inherits="SiteDetailV2" %>
<%@ Register TagPrefix="UC" TagName="AddressControl" Src="~/Controls/AddressControl.ascx" %>
<%@ Register TagPrefix="UC" TagName="AddressAusControl" Src="~/Controls/AddressAusControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.charAt(0).toUpperCase() + txtbox.value.slice(1);
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

        <div class="clearfix">
            <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server" Text="Site Information" /></h3></div>
            <div class="main_content">

                <div class="text-center">
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>




                <table class="block_center" style="margin: 0 auto;" id="maintable" runat="server">
                    <tr style="vertical-align:top;">

                        <td>
                            <table>
                                <tr>
                                    <td colspan="4"><b>Site Information</b><br /><br /></td>
                                </tr>
                                <tr id="idRow" runat="server">
                                    <td><asp:Label ID="lblIdText" runat="server" Text="ID"></asp:Label></td>
                                    <td></td>
                                    <td style="width:210px"><asp:Label ID="lblId" runat="server"></asp:Label></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Site Name</td>
                                    <td style="width:12px"></td>
                                    <td style="width:210px"><asp:TextBox Width="75%" ID="txtName" runat="server" Text='<%# Bind("name") %>' onkeyup="capitalize_first(this);" /><asp:Label ID="lblName" runat="server" Font-Bold="True" CssClass="nowrap"/></td>
                                    <td><asp:RequiredFieldValidator ID="txtValidateNameRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtName" 
                                            ErrorMessage="Name is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateNameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtName"
                                            ValidationExpression="^[a-zA-Z\-\s'\(\)\.]+$"
                                            ErrorMessage="Name can only be letters, and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Type</td>
                                    <td></td>
                                    <td><asp:DropDownList ID="ddlClinic" runat="server" SelectedValue='<%# Convert.ToInt32(Eval("site_type_id")) %>'></asp:DropDownList>
                                        <asp:Label ID="lblClinic" runat="server" Font-Bold="True"  CssClass="nowrap"/>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>ABN</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtABN" runat="server" Text='<%# Bind("abn") %>'></asp:TextBox><asp:Label ID="lblABN" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateABNRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtABN"
                                            ValidationExpression="^[0-9\-\s]+$"
                                            ErrorMessage="ABN can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>ACN</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtACN" runat="server" ></asp:TextBox><asp:Label ID="lblACN" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateACNRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtACN"
                                            ValidationExpression="^[0-9\-]+$"
                                            ErrorMessage="ACN can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="AddSiteValidationGroup">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>TFN</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtTFN" runat="server" Text='<%# Bind("tfn") %>'></asp:TextBox><asp:Label ID="lblTFN" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateTFNRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtTFN"
                                            ValidationExpression="^[0-9\-]+$"
                                            ErrorMessage="TFN can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>ASIC</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtASIC" runat="server" Text='<%# Bind("asic") %>'></asp:TextBox><asp:Label ID="lblASIC" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateASICRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtASIC"
                                            ValidationExpression="^[0-9\-]+$"
                                            ErrorMessage="ASIC can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                        </td>
                                </tr>
                                <tr>
                                    <td>Provider</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkIsProvider" runat="server" Checked='<%#Eval("is_provider").ToString()=="True"?true:false %>' /><asp:Label ID="lblIsProvider" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>BPay</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtBPay" runat="server" Text='<%# Bind("bank_bpay") %>'></asp:TextBox><asp:Label ID="lblBPay" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateBPayRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtBPay"
                                            ValidationExpression="^[0-9\-]+$"
                                            ErrorMessage="BPay can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>BSB</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtBSB" runat="server" Text='<%# Bind("bank_bsb") %>'></asp:TextBox><asp:Label ID="lblBSB" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateBSBRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtBSB"
                                            ValidationExpression="^[0-9\-]+$"
                                            ErrorMessage="BSB can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Bank Acct</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtBankAccount" runat="server" Text='<%# Bind("bank_account") %>'></asp:TextBox><asp:Label ID="lblBankAccount" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateBankAccountRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtBankAccount"
                                            ValidationExpression="^[0-9\-]+$"
                                            ErrorMessage="Bank Account can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Bank Dir. Debit UserID</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtBankDirectDebitUserID" runat="server" Text='<%# Bind("bank_direct_debit_userid") %>'></asp:TextBox><asp:Label ID="lblBankDirectDebitUserID" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateBankDirectDebitUserIDRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtBankDirectDebitUserID"
                                            ValidationExpression="^[a-zA-Z0-9\-]+$"
                                            ErrorMessage="Bank Dir. Debit UserID can only be letters, numbers, and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Bank Username</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtBankUsername" runat="server" Text='<%# Bind("bank_username") %>'></asp:TextBox><asp:Label ID="lblBankUsername" runat="server" Font-Bold="True"  CssClass="nowrap"/></td>
                                    <td><asp:RegularExpressionValidator ID="txtValidateBankUsernameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtBankUsername"
                                            ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                            ErrorMessage="Bank Username can only be letters, numbers, and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Oustanding Balance Warning</td>
                                    <td></td>
                                    <td><asp:TextBox Width="75%" ID="txtOustandingBalanceWarning" runat="server" Text='<%# Bind("oustanding_balance_warning") %>'></asp:TextBox><asp:Label ID="lblOustandingBalanceWarning" runat="server" Font-Bold="True"/></td>
                                    <td><asp:RequiredFieldValidator ID="txtValidateOustandingBalanceWarningRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtOustandingBalanceWarning" 
                                        ErrorMessage="Oustanding Balance Warning is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateOustandingBalanceWarningRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtOustandingBalanceWarning"
                                        ValidationExpression="^\d+(\.\d{1,2})?$"
                                        ErrorMessage="Oustanding Balance Warning can only be numbers and option decimal place with 1 or 2 digits following."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblPrintEPCText" runat="server">Print EPC</asp:Label></td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkIsPrintEPC" runat="server" Checked='<%# Eval("print_epc").ToString()=="True"?true:false %>' /><asp:Label ID="lblIsPrintEPC" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Retrieve Booking Months</td>
                                    <td></td>
                                    <td><asp:DropDownList ID="ddlNumBookingMonthsToGet" runat="server"/><asp:Label ID="lblNumBookingMonthsToGet" runat="server" Font-Bold="True"/></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Fiscal Yr End</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlFiscalYrEnd_Day" runat="server"/> 
                                        <asp:DropDownList ID="ddlFiscalYrEnd_Month" runat="server"> 
                                                <asp:ListItem Value="1" Text="January"></asp:ListItem>
                                                <asp:ListItem Value="2" Text="February"></asp:ListItem>
                                                <asp:ListItem Value="3" Text="March"></asp:ListItem>
                                                <asp:ListItem Value="4" Text="April"></asp:ListItem>
                                                <asp:ListItem Value="5" Text="May"></asp:ListItem>
                                                <asp:ListItem Value="6" Text="June"></asp:ListItem>
                                                <asp:ListItem Value="7" Text="July"></asp:ListItem>
                                                <asp:ListItem Value="8" Text="August"></asp:ListItem>
                                                <asp:ListItem Value="9" Text="September"></asp:ListItem>
                                                <asp:ListItem Value="10" Text="October"></asp:ListItem>
                                                <asp:ListItem Value="11" Text="November"></asp:ListItem>
                                                <asp:ListItem Value="12" Text="December"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="ddlFiscalYrEnd_Year" runat="server"/> 
                                        <asp:Label ID="lblFiscalYrEnd" runat="server" Font-Bold="True"/>
                                    </td>
                                    <td></td>
                                </tr>
                            </table>
                        </td>
                        <td style="width:65px"></td>
                        <td>
                            <table>
                                <tr>
                                    <td colspan="2"><b>Days Open</b><br /><br /></td>
                                    <td style="width:35px"></td>
                                    <td colspan="2"><b>Hours</b><br /><br /></td>
                                </tr>
                                <tr>
                                    <td>Sunday</td>
                                    <td><asp:CheckBox ID="chkIncSunday" runat="server" /></td>
                                    <td></td>
                                    <td>Day Start</td>
                                    <td>
                                        <asp:DropDownList ID="ddlDayStart_Hour" runat="server"/> <b>:</b>
                                        <asp:DropDownList ID="ddlDayStart_Minute" runat="server"/> 
                                    </td>
                                </tr>
                                <tr>
                                    <td>Monday</td>
                                    <td><asp:CheckBox ID="chkIncMonday" runat="server" /></td>
                                    <td></td>
                                    <td>Lunch Start</td>
                                    <td>
                                        <asp:DropDownList ID="ddlLunchStart_Hour" runat="server" /> <b>:</b>
                                        <asp:DropDownList ID="ddlLunchStart_Minute" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Tuesday</td>
                                    <td><asp:CheckBox ID="chkIncTuesday" runat="server" /></td>
                                    <td></td>
                                    <td>Lunch End</td>
                                    <td>
                                        <asp:DropDownList ID="ddlLunchEnd_Hour" runat="server" /> <b>:</b>
                                        <asp:DropDownList ID="ddlLunchEnd_Minute" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Wednesday</td>
                                    <td><asp:CheckBox ID="chkIncWednesday" runat="server" /></td>
                                    <td></td>
                                    <td>Day End</td>
                                    <td>
                                        <asp:DropDownList ID="ddlDayEnd_Hour" runat="server" /> <b>:</b>
                                        <asp:DropDownList ID="ddlDayEnd_Minute" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Thursday</td>
                                    <td><asp:CheckBox ID="chkIncThursday" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td>Friday</td>
                                    <td><asp:CheckBox ID="chkIncFriday" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td>Saturday</td>
                                    <td><asp:CheckBox ID="chkIncSaturday" runat="server" /></td>
                                </tr>
                            </table>

                            <br />

                            <UC:AddressControl ID="addressControl" runat="server" Visible="False" />
                            <UC:AddressAusControl ID="addressAusControl" runat="server" Visible="False" />

                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:center">
                            <br />  
                            <asp:Button ID="btnSubmit" runat="server" Text="Button" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" Visible="False" />
                            <br />              
                        </td>
                    </tr>

                </table>


                <div id="autodivheight" class="divautoheight" style="height:500px;">
                </div>

  
            </div>
        </div>

</asp:Content>



