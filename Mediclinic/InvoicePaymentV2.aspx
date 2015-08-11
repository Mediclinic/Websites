<%@ Page Title="Add Receipts & Adjustment Notes" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="InvoicePaymentV2.aspx.cs" Inherits="InvoicePaymentV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Invoice Payment</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <table>
                    <tr>
                        <td>

                            <div id="header_table" runat="server">
                            <center>

                                <div style="height:25px;"></div>

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

                            <div style="height:20px;"></div>

                            <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                            <center>
                                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                            </center>

                            <div id="maintable" runat="server">

                                <div style="height:20px;"></div>

                                <asp:Repeater id="lstPayments" runat="server" ClientIDMode="Predictable" EnableViewState="True">
                                    <HeaderTemplate>
                                        <table class="block_center">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr style="vertical-align:top;">
                                            <td class="nowrap"><asp:Label ID="lblDesc" runat="server"  Text='<%# Eval("descr") %>' /><asp:Label ID="lblTypeID" runat="server"  Text='<%# Eval("receipt_payment_type_id") %>' CssClass="hiddencol" /></td>
                                            <td class="nowrap" style="min-width:8px;"></td>
                                            <td class="nowrap">
                                                <asp:TextBox ID="txtAmount" runat="server" Visible='<%# ((string)Eval("Text")).Length == 0 ? true : false %>' Width="85" /><asp:Label ID="lblText" runat="server" Text='<%# Eval("Text") %>' Visible='<%# ((string)Eval("Text")).Length == 0 ? false : true %>' />
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

                                <div style="height:40px;"></div>


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



