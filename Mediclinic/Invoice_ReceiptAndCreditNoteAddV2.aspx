﻿<%@ Page Title="Add Receipts & Adjustment Notes" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Invoice_ReceiptAndCreditNoteAddV2.aspx.cs" Inherits="Invoice_ReceiptAndCreditNoteAddV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function update_total() {

            // get all textboxes in the maintable div

            var total = 0.00;
            var str = "";

            var inputs = document.getElementsByTagName("input");
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].id.indexOf('MainContent_lstPayments_txtAmount_') == 0 && inputs[i].value.length > 0 && !isNaN(inputs[i].value))
                    total += parseFloat(inputs[i].value);
            }
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].id.indexOf('MainContent_lstVouchers_txtAmount_') == 0 && inputs[i].value.length > 0 && !isNaN(inputs[i].value))
                    total += parseFloat(inputs[i].value);
            }

            var credit_note_total = document.getElementById('txtCreditNoteTotal').value;
            if (credit_note_total.length > 0 && !isNaN(credit_note_total))
                total += parseFloat(credit_note_total);

            document.getElementById('txtTotal').value = total.toFixed(2);
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Add Receipts & Adjustment Notes</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <table>
                    <tr>
                        <td>

                            <div id="header_table" runat="server">
                            <center>

                                <table>
                                    <tr>
                                        <td>
                                            <b>
                                            Invoice #<br />
                                            Amount Owing
                                            </b>
                                        </td>
                                        <td style="width:18px"></td>
                                        <td>
                                            <b>
                                                <asp:Label ID="lblInvoiceNbr" runat="server"></asp:Label><br />
                                                <asp:Label ID="lblAmountOwing" runat="server"></asp:Label>
                                            </b>

                                        </td>
                                    </tr>
                                </table>

                            </center>
                            </div>

                            <div style="height:8px;"></div>
                            <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                            <center>
                                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                            </center>

                            <div id="maintable" runat="server">

                                <b>Add Receipt(s)</b>
                                <div style="height:8px;"></div>

                                <asp:Repeater id="lstPayments" runat="server" ClientIDMode="Predictable" EnableViewState="True">
                                    <HeaderTemplate>
                                        <table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr style="vertical-align:top;">
                                            <td class="nowrap"><asp:Label ID="lblDesc" runat="server"  Text='<%# Eval("descr") %>' /><asp:Label ID="lblTypeID" runat="server"  Text='<%# Eval("receipt_payment_type_id") %>' CssClass="hiddencol" /></td>
                                            <td class="nowrap" style="min-width:8px;"></td>
                                            <td class="nowrap">
                                                <asp:TextBox ID="txtAmount" runat="server" Visible='<%# ((string)Eval("Text")).Length == 0 ? true : false %>' Width="85" onKeyUp="update_total();" /><asp:Label ID="lblText" runat="server" Text='<%# Eval("Text") %>' Visible='<%# ((string)Eval("Text")).Length == 0 ? false : true %>' />
                                                <asp:RangeValidator ID="txtValidateTotalRange" runat="server" CssClass="failureNotification" SetFocusOnError="true"
                                                    ControlToValidate="txtAmount"
                                                    ErrorMessage="Receipt Amount must be a number and must be more than zero" Type="Double" MinimumValue="0.01" MaximumValue="10000.00" 
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;<asp:Button ID="btnWebPay" runat="server" style="padding: 0px 6px !important;height:22px;" Text="Pay Now" OnCommand="btnWebPay_Command" CommandArgument='<%# Eval("receipt_payment_type_id") %>'  BackColor="White" CssClass="white_button" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>

                                <div id="divVouchers" runat="server">

                                    <div style="height:15px;"></div>

                                    <b>Use Voucher(s)</b>
                                    <div style="height:8px;"></div>

                                    <asp:Repeater id="lstVouchers" runat="server" ClientIDMode="Predictable" EnableViewState="True">
                                        <HeaderTemplate>
                                            <table>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr style="vertical-align:top;">
                                                <td><%# "<a href=\"#\" title=\"" + Eval("credit_voucher_descr") + (Eval("credit_expiry_date") == DBNull.Value ? Environment.NewLine + "Exp.: No Exp. Date" : (Environment.NewLine + "Exp.: " + Eval("credit_expiry_date", "{0:d MMM, yyyy}"))) + (Environment.NewLine + "Total: " + Eval("credit_amount")) + (Environment.NewLine + "Used: " + Eval("credit_amount_used")) + (Environment.NewLine + "Remaining: " + ((decimal)Eval("credit_amount") - (decimal)Eval("credit_amount_used")).ToString()) + "\" style=\"color:inherit;\">" + (  Eval("credit_voucher_descr").ToString().Length > 32 ? Eval("credit_voucher_descr").ToString().Substring(0, 30) + ".." : Eval("credit_voucher_descr") ) + "</a>" %>&nbsp;&nbsp;</td>
                                                <td>[<%# "$<b>" + ((decimal)Eval("credit_amount") - (decimal)Eval("credit_amount_used")) + "</b> Left"  %>]&nbsp;&nbsp;</td>
                                                <td class="nowrap">
                                                    <asp:HiddenField ID="hiddenCreditID" runat="server" Value='<%# Eval("credit_credit_id") %>' />
                                                    <asp:HiddenField ID="hiddenEntityID" runat="server" Value='<%# Eval("credit_entity_id") %>' />
                                                    <asp:TextBox ID="txtAmount" runat="server" Width="85" onKeyUp="update_total();" /><asp:Label ID="lblText" runat="server" />
                                                    <asp:RangeValidator ID="txtValidateTotalRange" runat="server" CssClass="failureNotification" SetFocusOnError="true"
                                                        ControlToValidate="txtAmount"
                                                        ErrorMessage="Voucher Amount must be a valid amount and must be more than zero" Type="Double" MinimumValue="0.01" MaximumValue="10000.00" 
                                                        Display="Dynamic"
                                                        ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>

                                </div>

                                <div style="height:15px;"></div>

                                <b>Add Adjustment Note</b>
                                <div style="height:8px;"></div>

                                <table>
                                    <tr style="vertical-align:top;">
                                        <td class="nowrap">Amount</td>
                                        <td class="nowrap" style="min-width:8px;"></td>
                                        <td class="nowrap">
                                            <asp:TextBox ID="txtCreditNoteTotal" runat="server" Width="85" Text="0.00" onKeyUp="update_total();" />
                                            <asp:RangeValidator ID="txtValidateTotalRange" runat="server" CssClass="failureNotification" SetFocusOnError="true"
                                                ControlToValidate="txtCreditNoteTotal"
                                                ErrorMessage="Credit Note Amount must be a number and not less than zero" Type="Double" MinimumValue="0.00" MaximumValue="10000.00" 
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                                        </td>
                                        <td class="nowrap" style="min-width:14px;"></td>
                                        <td class="nowrap">Reason</td>
                                        <td class="nowrap"style="min-width:8px;"></td>
                                        <td class="nowrap"><asp:TextBox ID="txtCreditCardReason" runat="server" Width="200" MaxLength="250"></asp:TextBox></td>
                                    </tr>
                                </table>

                                <div style="height:40px;"></div>

                                <center>
                                    <table>
                                        <tr>
                                            <td><b>Total Receipts & Credit Notes</b></td>
                                            <td style="width:18px"></td>
                                            <td><asp:TextBox ID="txtTotal" runat="server" Width="85" Text="0.00" Enabled="false" Font-Bold="true" /></td>
                                        </tr>
                                        <tr style="height:10px">
                                            <td colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="text-align:center">
                                                <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                                                <asp:Button ID="btnCancel" runat="server" Text="No Payment" onclick="btnCancel_Click" OnClientClick="self.close();" />
                                            </td>


                                        </tr>

                                    </table>
                                </center>

                            </div>

                        </td>
                    </tr>
                </table>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:100px;">
            </div>

        </div>
    </div>


</asp:Content>



