<%@ Page Title="Add Credit" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Notifications_AddCreditEmailPopupV2.aspx.cs" Inherits="Notifications_AddCreditEmailPopupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function radio_clicked(radioButton) {

            //radioButton.checked = true;

            if (radioButton.value == "call") {
                document.getElementById("nameRow").className = document.getElementById("nameRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("phoneNumberRow").className = document.getElementById("phoneNumberRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("mobileNumberRow").className = document.getElementById("mobileNumberRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("space1Row").className = document.getElementById("space1Row").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById('creditCardNumberRow').className = "hiddencol";
                document.getElementById('creditCardExpiryRow').className = "hiddencol";
                document.getElementById('ccvRow').className = "hiddencol";
                document.getElementById('space2Row').className = "hiddencol";
                document.getElementById("amountRow").className = document.getElementById("amountRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("space3Row").className = document.getElementById("space3Row").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("extraInfoRow").className = document.getElementById("extraInfoRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("space4Row").className = document.getElementById("space4Row").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("buttonsRow").className = document.getElementById("buttonsRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
            }
            else if (radioButton.value == "charge_credit_card") {
                document.getElementById("nameRow").className = document.getElementById("nameRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("phoneNumberRow").className = document.getElementById("phoneNumberRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("mobileNumberRow").className = document.getElementById("mobileNumberRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("space1Row").className = document.getElementById("space1Row").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("creditCardNumberRow").className = document.getElementById("creditCardNumberRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("creditCardExpiryRow").className = document.getElementById("creditCardExpiryRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("ccvRow").className = document.getElementById("ccvRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("space2Row").className = document.getElementById("space2Row").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("amountRow").className = document.getElementById("amountRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("space3Row").className = document.getElementById("space3Row").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("extraInfoRow").className = document.getElementById("extraInfoRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("space4Row").className = document.getElementById("space4Row").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
                document.getElementById("buttonsRow").className = document.getElementById("buttonsRow").className.replace(/(?:^|\s)hiddencol(?!\S)/g, '');
            }
            else {
                document.getElementById('nameRow').className = "hiddencol";
                document.getElementById('phoneNumberRow').className = "hiddencol";
                document.getElementById('mobileNumberRow').className = "hiddencol";
                document.getElementById('space1Row').className = "hiddencol";
                document.getElementById('creditCardNumberRow').className = "hiddencol";
                document.getElementById('creditCardExpiryRow').className = "hiddencol";
                document.getElementById('ccvRow').className = "hiddencol";
                document.getElementById('space2Row').className = "hiddencol";
                document.getElementById('amountRow').className = "hiddencol";
                document.getElementById('space3Row').className = "hiddencol";
                document.getElementById('extraInfoRow').className = "hiddencol";
                document.getElementById('space4Row').className = "hiddencol";
                document.getElementById('buttonsRow').className = "hiddencol";
            }
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Add Credit</asp:Label></div>
        <div class="main_content" style="padding:5px 5px;">

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <br />

            <table class="block_center">
                <tr>
                    <td>
                        <asp:RadioButtonList id="radioType" runat="server" AutoPostBack="False">
                            <asp:ListItem Value="call" onclick="radio_clicked(this);">Call Me</asp:ListItem>
                            <asp:ListItem Value="charge_credit_card" onclick="radio_clicked(this);">Charge Credit Card</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>



            <br />

            <table id="maintable" runat="server" class="block_center">

                <tr id="nameRow" runat="server" class="hiddencol">
                    <td>Name</td>
                    <td style="width:18px"></td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server"/>
                        <asp:RequiredFieldValidator ID="txtValidateNameRequired" runat="server" CssClass="failureNotification"  
                            ControlToValidate="txtName" 
                            ErrorMessage="Name is empty."
                            Display="Dynamic"
                            ValidationGroup="ValidationSummary">&nbsp;*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr id="phoneNumberRow" runat="server" class="hiddencol">
                    <td>Phone Number</td>
                    <td></td>
                    <td><asp:TextBox ID="txtPhoneNumber" runat="server"/></td>
                </tr>
                <tr id="mobileNumberRow" runat="server" class="hiddencol">
                    <td>Mobile Number</td>
                    <td></td>
                    <td><asp:TextBox ID="txtMobileNumber" runat="server"/></td>
                </tr>

                <tr id="space1Row" runat="server" class="hiddencol">
                    <td style="height:8px" colspan="6"></td>
                </tr>

                <tr id="creditCardNumberRow" runat="server" class="hiddencol">
                    <td>Credit Card Number</td>
                    <td></td>
                    <td><asp:TextBox ID="txtCreditCardNumber" runat="server"/></td>
                </tr>
                <tr id="creditCardExpiryRow" runat="server" class="hiddencol">
                    <td>Credit Card Expiry Date</td>
                    <td></td>
                    <td><asp:TextBox ID="txtCreditCarddExpiryDate" runat="server"/></td>
                </tr>
                <tr id="ccvRow" runat="server" class="hiddencol">
                    <td>CCV</td>
                    <td></td>
                    <td><asp:TextBox ID="txtCCV" runat="server"/></td>
                </tr>

                <tr id="space2Row" runat="server" class="hiddencol">
                    <td style="height:8px" colspan="3"></td>
                </tr>

                <tr id="amountRow" runat="server" class="hiddencol">
                    <td>Amount To Top-Up</td>
                    <td align="right">$</td>
                    <td>
                        <asp:TextBox ID="txtAmountToTopUp" runat="server"/>
                        <asp:RequiredFieldValidator ID="txtValidateAmountRequired" runat="server" CssClass="failureNotification"  
                            ControlToValidate="txtAmountToTopUp" 
                            ErrorMessage="Amount is empty."
                            Display="Dynamic"
                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                        <asp:RangeValidator ID="txtValidateAmountRange" runat="server" CssClass="failureNotification" 
                            ControlToValidate="txtAmountToTopUp"
                            ErrorMessage="Amount must be a number and must be more than zero" Type="Double" MinimumValue="0.01" MaximumValue="10000.00" 
                            Display="Dynamic"
                            ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                    </td>
                </tr>

                <tr id="space3Row" runat="server" class="hiddencol">
                    <td style="height:8px" colspan="3"></td>
                </tr>

                <tr id="extraInfoRow" runat="server" class="hiddencol">
                    <td valign="top">Extra Information</td>
                    <td></td>
                    <td valign="top">
                        <asp:TextBox ID="txtEmailMessage" runat="server" TextMode="MultiLine" Columns="40" Rows="5"/>
                    </td>
                </tr>

                <tr id="space4Row" runat="server" class="hiddencol">
                    <td style="height:25px;" colspan="6"></td>
                </tr>

                <tr id="buttonsRow" runat="server" class="hiddencol">
                    <td colspan="3" align="center">
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click"  OnClientClick="window.returnValue=false;self.close();" />
                        <br />
                        <br />
                    </td>
                </tr>

            </table>

        </div>
    </div>


</asp:Content>



