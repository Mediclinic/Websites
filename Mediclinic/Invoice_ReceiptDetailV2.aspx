<%@ Page Title="Receipt" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Invoice_ReceiptDetailV2.aspx.cs" Inherits="Invoice_ReceiptDetailV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Receipt</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <table id="maintable" runat="server">
                    <tr id="idRow" runat="server">
                        <td>Receipt #</td>
                        <td></td>
                        <td><asp:Label ID="lblId" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>Invoice #</td>
                        <td></td>
                        <td><asp:Label ID="lblInvoiceId" runat="server"></asp:Label></td>
                    </tr>
                    <tr id="receiptDateRow" runat="server">
                        <td class="nowrap">Receipt Date</td>
                        <td></td>
                        <td><asp:Label ID="lblReceiptDate" runat="server"/></td>
                    </tr>
                    <tr>
                        <td class="nowrap">Payment Type</td>
                        <td style="width:12px"></td>
                        <td><asp:DropDownList ID="ddlPaymentType" runat="server" DataValueField="receipt_payment_type_id" DataTextField="descr" /><asp:Label ID="lblPaymentType" runat="server" Font-Bold="True"/></td>
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
                    <tr id="isOverPaidRow" runat="server">
                        <td>Overpaid</td>
                        <td></td>
                        <td><asp:Label ID="lblIsOverpaid" runat="server" Font-Bold="True"/></td>
                    </tr>
                    <tr id="addedByRow" runat="server">
                        <td>Added By</td>
                        <td></td>
                        <td><asp:Label ID="lblAddedBy" runat="server" Font-Bold="True"/></td>
                    </tr>


                    <tr style="height:20px">
                        <td colspan="3"></td>
                    </tr>

                    <tr id="isReconciledRow" runat="server">
                        <td>Is Reconciled</td>
                        <td></td>
                        <td><asp:Label ID="lblIsReconciled" runat="server" Font-Bold="True"/></td>
                    </tr>
                    <tr id="reconciliationDateRow" runat="server">
                        <td>Reconciliation Date</td>
                        <td></td>
                        <td><asp:Label ID="lblReconciliationDate" runat="server" Font-Bold="True"/></td>
                    </tr>
                    <tr id="amountReconciledRow" runat="server">
                        <td>Reconcile Amount</td>
                        <td></td>
                        <td>
                            <asp:TextBox ID="txtAmountReconciled" runat="server"></asp:TextBox><asp:Label ID="lblAmountReconciled" runat="server" Font-Bold="True"/>
                            <asp:RequiredFieldValidator ID="txtValidateAmountReconciledRequired" runat="server" CssClass="failureNotification"  
                                ControlToValidate="txtAmountReconciled" 
                                ErrorMessage="Amount Reconciled is required."
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="txtValidateAmountReconciledRange" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtAmountReconciled"
                                ErrorMessage="Amount Reconciled must be a number and must be more than zero" Type="Double" MinimumValue="0.01" MaximumValue="10000.00" 
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                        </td>
                    </tr>
                    <tr id="failedToClearRow" runat="server">
                        <td>Failed To Clear</td>
                        <td></td>
                        <td><asp:CheckBox ID="chkFailedToClear" runat="server" /><asp:Label ID="lblFailedToClear" runat="server" Font-Bold="True"/></td>
                    </tr>


                    <tr height="10">
                        <td colspan="3"></td>
                    </tr>

                    <tr>
                        <td colspan="3" align="center">
                            <br />  
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" OnClientClick="window.returnValue=false;self.close();" />
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



