<%@ Page Title="Adjustment Note" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Invoice_CreditNoteDetailV2.aspx.cs" Inherits="Invoice_CreditNoteDetailV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Adjustment Note</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <table id="maintable" runat="server">
                    <tr id="idRow" runat="server">
                        <td>Adj Note #</td>
                        <td></td>
                        <td><asp:Label ID="lblId" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>Invoice #</td>
                        <td style="min-width:10px;"></td>
                        <td><asp:Label ID="lblInvoiceId" runat="server"></asp:Label></td>
                    </tr>
                    <tr id="creditnoteDateRow" runat="server">
                        <td class="nowrap">Credit Note Date</td>
                        <td></td>
                        <td><asp:Label ID="lblCreditNoteDate" runat="server"/></td>
                    </tr>
                    <tr id="amountOwedRow" runat="server">
                        <td class="nowrap">Amount Owing</td>
                        <td></td>
                        <td><asp:Label ID="lblAmountOwing" runat="server"/></td>
                    </tr>
                    <tr>
                        <td>Total</td>
                        <td></td>
                        <td>
                            <asp:TextBox ID="txtTotal" runat="server"></asp:TextBox><asp:Label ID="lblTotal" runat="server" Font-Bold="True"/>
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
                            <asp:TextBox ID="txtReason" runat="server"></asp:TextBox>
                            <asp:Label ID="lblReason" runat="server"/>
                        </td>
                    </tr>
                    <tr id="addedByRow" runat="server">
                        <td>Added By</td>
                        <td></td>
                        <td><asp:Label ID="lblAddedBy" runat="server"/></td>
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



