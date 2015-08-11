<%@ Page Title="Refund" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Invoice_RefundDetailV2.aspx.cs" Inherits="Invoice_RefundDetailV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Refund</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <table id="maintable" runat="server">
                    <tr id="idRow" runat="server">
                        <td>Refund #</td>
                        <td></td>
                        <td><asp:Label ID="lblId" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>Invoice #</td>
                        <td style="min-width:8px;"></td>
                        <td><asp:Label ID="lblInvoiceId" runat="server"></asp:Label></td>
                    </tr>
                    <tr id="refundDateRow" runat="server">
                        <td class="nowrap">Refund Date</td>
                        <td></td>
                        <td><asp:Label ID="lblRefundDate" runat="server"/></td>
                    </tr>
                    <tr id="amountReceiptedRow" runat="server">
                        <td>Amount Receipted</td>
                        <td></td>
                        <td><asp:Label ID="lblAmountReceipted" runat="server"/></td>
                    </tr>
                    <tr>
                        <td>Total</td>
                        <td></td>
                        <td>
                            <asp:TextBox ID="txtTotal" runat="server" style="width:100%;"></asp:TextBox><asp:Label ID="lblTotal" runat="server" Font-Bold="True"/>
                            <asp:RequiredFieldValidator ID="txtValidateTotalRequired" runat="server" CssClass="failureNotification"  
                                ControlToValidate="txtTotal" 
                                ErrorMessage="Total is required."
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="txtValidateTotalRange" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtTotal"
                                ErrorMessage="Total must be a number and must be more than zero" Type="Double" MinimumValue="0.01" MaximumValue="10000.00" 
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                        </td>
                    </tr>

                    <tr>
                        <td>Reason</td>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="ddlRefundReason" runat="server" style="width:100%;" DataValueField="refund_reason_id" DataTextField="descr"></asp:DropDownList>
                            <asp:Label ID="lblReason" runat="server"/>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">Comment</td>
                        <td></td>
                        <td>
                            <asp:TextBox ID="txtComment" runat="server" style="width:100%;"></asp:TextBox>
                            <asp:Label ID="lblComment" runat="server"/>
                        </td>
                    </tr>
                    <tr id="addedByRow" runat="server">
                        <td>Added By</td>
                        <td></td>
                        <td><asp:Label ID="lblAddedBy" runat="server" /></td>
                    </tr>

                    <tr height="10">
                        <td colspan="3"></td>
                    </tr>

                    <tr>
                        <td colspan="3" align="center">
                            <br />  
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click"  OnClientClick="window.returnValue=false;self.close();" />
                            <br />              
                        </td>
                    </tr>

                </table>


            </center>

            <div id="autodivheight" class="divautoheight" style="height:100px;">
            </div>

        </div>
    </div>


</asp:Content>



