<%@ Page Title="Contact List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="CreditDetailV2.aspx.cs" Inherits="CreditDetailV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1); });
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server">Contact</asp:Label></h3></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:HiddenField ID="hiddenIsMobileDevice" runat="server" Value="0" />

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>

                <div style="height:12px;"></div>

                <table id="maintable" runat="server">
                    <tr id="idRow" runat="server">
                        <td style="min-width:20px;"></td>
                        <td>ID</td>
                        <td></td>
                        <td><asp:Label ID="lblId" runat="server"></asp:Label></td>
                        <td style="min-width:20px;"></td>
                    </tr>
                    <tr id="typeRow" runat="server">
                        <td></td>
                        <td>Type</td>
                        <td></td>
                        <td><asp:Label ID="lblType" runat="server"></asp:Label></td>
                        <td></td>
                    </tr>
                    <tr id="descrRow" style="vertical-align:top;" runat="server">
                        <td></td>
                        <td class="nowrap">Description</td>
                        <td style="min-width:8px;"></td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" Columns="40" onkeyup="capitalize_first(this);" ></asp:TextBox>
                            <asp:Label ID="lblDescr" runat="server" Font-Bold="True"/>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="min-width:20px;"></td>
                        <td class="nowrap">Amount</td>
                        <td style="min-width:8px;"></td>
                        <td class="nowrap">
                            <asp:TextBox ID="txtAmount" runat="server" Columns="6"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="validateAmountRequired" runat="server" CssClass="failureNotification"  
                                ControlToValidate="txtAmount" ErrorMessage="Amount is required." Display="Dynamic" ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="validateAmount" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtAmount" Type="Currency" MinimumValue="0.00" MaximumValue="999.99" ErrorMessage="Amount must be a valid amount and not more than 999.99."
                                Display="Dynamic" ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                            <asp:Label ID="lblAmount" runat="server" Font-Bold="True"/>
                        </td>
                        <td style="min-width:20px;"></td>
                    </tr>
                    <tr id="amountUsedRow" runat="server">
                        <td></td>
                        <td>Used</td>
                        <td></td>
                        <td><asp:Label ID="lblAmountUsed" runat="server" Font-Bold="True"></asp:Label></td>
                        <td></td>
                    </tr>
                    <tr id="amountRemainingRow" runat="server">
                        <td></td>
                        <td>Remaining</td>
                        <td></td>
                        <td><asp:Label ID="lblRemainingUsed" runat="server" Font-Bold="True"></asp:Label></td>
                        <td></td>
                    </tr>
                    <tr id="expiryRow" runat="server">
                        <td></td>
                        <td class="nowrap">Expiry Date</td>
                        <td></td>
                        <td class="nowrap">
                            <asp:DropDownList ID="ddlExpiry_Day" runat="server"></asp:DropDownList>
                            <asp:DropDownList ID="ddlExpiry_Month" runat="server"></asp:DropDownList>
                            <asp:DropDownList ID="ddlExpiry_Year" runat="server"></asp:DropDownList>
                            <asp:Label ID="lblExpiry" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                        <td><asp:CustomValidator ID="ddlExpiryValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                ControlToValidate="ddlExpiry_Day"
                                OnServerValidate="ExpiryAllOrNoneCheck"
                                ErrorMessage="Expiry Date must have each of day/month/year selected, or all set to '--'"
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                        </td>
                        <td></td>
                    </tr>
                    <tr id="voucherUsedRow" runat="server">
                        <td></td>
                        <td class="nowrap">Voucher Used</td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblVoucher" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>

                    <tr id="invoiceRow" runat="server">
                        <td></td>
                        <td class="nowrap">Invoice Used</td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblInvoice" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr id="addedbyRow" runat="server">
                        <td></td>
                        <td class="nowrap">Added By</td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblAddedBy" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr id="dateAddedRow" runat="server">
                        <td></td>
                        <td class="nowrap">Date Added</td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblDateAdded" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr id="modifiedbyRow" runat="server">
                        <td></td>
                        <td class="nowrap">Last Modified By</td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblModifiedBy" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr id="dateModifiedRow" runat="server">
                        <td></td>
                        <td class="nowrap">Last Modified Date</td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblDateModified" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr id="deletedSpaceRow" runat="server">
                        <td colspan="6"><div style="height:10px;"></div></td>
                    </tr>
                    <tr id="deletedbyRow" runat="server">
                        <td></td>
                        <td class="nowrap">Deleted By</td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblDeletedBy" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr id="dateDeletedRow" runat="server">
                        <td></td>
                        <td class="nowrap">Date Deleted</td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblDateDeleted" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr id="preDeletedAmountRow" runat="server">
                        <td></td>
                        <td class="nowrap">Pre Deleted Amount</td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblPreDeletedAmount" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>

                    <tr id="invoiceListSpaceRow" runat="server">
                        <td colspan="6"><div style="height:10px;"></div></td>
                    </tr>
                    <tr id="invoiceListRow" runat="server" style="vertical-align:top;">
                        <td></td>
                        <td class="nowrap"><i>Invoices Using<br />This Voucher</i></td>
                        <td style="min-width:12px"></td>
                        <td class="nowrap"><asp:Label ID="lblInvoicesUsingThisVoucher" runat="server"></asp:Label></td>
                        <td></td>
                        <td></td>
                    </tr>

                    <tr style="height:10px">
                        <td colspan="6"></td>
                    </tr>

                    <tr>
                        <td colspan="6" style="text-align:center">
                            <br />  
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="EditContactValidationSummary" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" />
                            <br />              
                        </td>
                    </tr>

                </table>

                 <br />
                 <a id="show_hide_history_link" runat="server" visible="false" href="javascript:void(0)" onclick="show_hide('div_history');update_to_max_height();">View Edit History</a>
                 <div id="div_history" runat="server" style="display:none;">

                    <asp:GridView ID="GrdCredit" runat="server" 
                        AutoGenerateColumns="False" 
                        GridLines="None" 
                        onrowdatabound="GrdCredit_RowDataBound"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal block_center auto_width vertical_align_top" style="max-width:90%;">
                        <Columns>

                            <asp:TemplateField HeaderText="Voucher<br />Descr">
                                <ItemTemplate>
                                    <div style="max-width: 200px;">
                                        <%# (int)Eval("credit_history_credit_type_id") != 1 ? "<font color=\"#9D9D9D\">N/A</font>" : Eval("credit_history_voucher_descr") %>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Amt">
                                <ItemTemplate>
                                    <asp:label ID="lblAmount" runat="server" Text='<%# Eval("credit_history_amount") == DBNull.Value ? "" : Eval("credit_history_amount") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Expiry Date">
                                <ItemTemplate>
                                        <%# Eval("credit_history_expiry_date") == DBNull.Value ? "" : Eval("credit_history_expiry_date", "{0:dd-MM-yyyy}") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Modified" >
                                <ItemTemplate>
                                    <%# 
                                    (Eval("credit_history_date_modified") == DBNull.Value ? "" : Eval("credit_history_date_modified", "{0:dd-MM-yyyy}")) +
                                    (Eval("credit_history_date_modified") != DBNull.Value && Eval("credit_history_modified_by") != DBNull.Value ? "<br />" : "") +
                                    (Eval("credit_history_modified_by")   == DBNull.Value ? "" : "By: " + Eval("person_modified_by_firstname") + " " + Eval("person_modified_by_surname"))
                                    %>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>

                 </div>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>


</asp:Content>



