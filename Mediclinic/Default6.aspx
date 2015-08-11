<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Default6.aspx.cs" Inherits="_Default6" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">&nbsp;</span></div>
        <div class="main_content" id="main_content" runat="server">


            <table class="block_center">
                <tr>
                    <td>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary1"/>
                        <asp:ValidationSummary ID="ValidationSummary2" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary2"/>
                        <asp:ValidationSummary ID="ValidationSummary3" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary3"/>
                        <asp:ValidationSummary ID="ValidationSummary4" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary4"/>
                        <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                    </td>
                </tr>
            </table>


            <table class="block_center">
                <tr>
                    <td>
                        <asp:TextBox ID="txtAddAmount" runat="server" Columns="8"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="validateAddAmountRequired" runat="server" CssClass="failureNotification"  
                            ControlToValidate="txtAddAmount" ErrorMessage="Amount is required." Display="Dynamic" ValidationGroup="ValidationSummary1">*</asp:RequiredFieldValidator>
                        <asp:RangeValidator ID="validateAddAmount" runat="server" CssClass="failureNotification" 
                            ControlToValidate="txtAddAmount" Type="Currency" MinimumValue="0.00" MaximumValue="999.99" ErrorMessage="Amount must be a valid amount and not more than 999.99."
                            Display="Dynamic" ValidationGroup="ValidationSummary1">*</asp:RangeValidator>
                        <asp:TextBox ID="txtAddDescr" runat="server" Columns="35" Text="Some Voucher..."></asp:TextBox>
                        <asp:Button ID="btn1" Text="Add Voucher"  runat="server" OnCommand="btn_Command" CommandArgument="Add Voucher" ValidationGroup="ValidationSummary1" />
                        <br />

                        <asp:TextBox ID="txtCashAmount" runat="server" Columns="8"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="validateCashAmountRequired" runat="server" CssClass="failureNotification"  
                            ControlToValidate="txtCashAmount" ErrorMessage="Amount is required." Display="Dynamic" ValidationGroup="ValidationSummary3">*</asp:RequiredFieldValidator>
                        <asp:RangeValidator ID="validateUCashAmount" runat="server" CssClass="failureNotification" 
                            ControlToValidate="txtCashAmount" Type="Currency" MinimumValue="0.00" MaximumValue="999.99" ErrorMessage="Amount must be a valid amount and not more than 999.99."
                            Display="Dynamic" ValidationGroup="ValidationSummary3">*</asp:RangeValidator>
                        <asp:Button ID="btn3" Text="Tyro Cashout" runat="server" OnCommand="btn_Command" CommandArgument="Tyro Cashout" ValidationGroup="ValidationSummary3" />
                    </td>
                </tr>
            </table>


            <br />


            <asp:Repeater id="lstPayments" runat="server" ClientIDMode="Predictable" EnableViewState="True">
                <HeaderTemplate>
                    <table class="block_center padded-table-width-4px">
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="vertical-align:top;">


                        <td runat="server" visible="false">[<%# Eval("credit_credit_id") %>]</td>
                        <td>
                            <%#
                                "<a href=\"#\" onclick=\"$(this).tooltip().mouseover();return false;\" title=\"" + Eval("credit_voucher_descr") + "\" style=\"color:inherit;\">" + (  Eval("credit_voucher_descr").ToString().Length > 22 ? Eval("credit_voucher_descr").ToString().Substring(0, 20) + ".." : Eval("credit_voucher_descr") ) + "</a>" + "<br />"
                            %>
                        </td>

                        <td>(<%# "$<b>" + ((decimal)Eval("credit_amount") - (decimal)Eval("credit_amount_used")) + "</b>"  %>)</td>
                        <td>(<%# "Exp.: <b>" + (Eval("credit_expiry_date") == DBNull.Value ? "No Exp Date" : Eval("credit_expiry_date", "{0:d MMM, yyyy}")) + "</b>" %>)</td>

                        <td class="nowrap">
                            <asp:HiddenField ID="hiddenCreditID" runat="server" Value='<%# Eval("credit_credit_id") %>' />
                            <asp:TextBox ID="txtAmount" runat="server" Width="85" /><asp:Label ID="lblText" runat="server" />
                            <asp:RangeValidator ID="txtValidateTotalRange" runat="server" CssClass="failureNotification" SetFocusOnError="true"
                                ControlToValidate="txtAmount"
                                ErrorMessage="Receipt Amount must be a valid amount and must be more than zero" Type="Double" MinimumValue="0.01" MaximumValue="10000.00" 
                                Display="Dynamic"
                                ValidationGroup="ValidationSummary4">*</asp:RangeValidator>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <center>
                <asp:Button ID="btnPay" Text="Pay" runat="server" OnClick="btnPay_Click" ValidationGroup="ValidationSummary4" />
            </center>
            
            <br />

            <asp:GridView ID="GrdCredit" runat="server" 
                AutoGenerateColumns="False" 
                GridLines="None" 
                onrowcommand="GrdCredit_RowCommand" 
                onrowdatabound="GrdCredit_RowDataBound"
                CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center vertical_align_top">
                <Columns>

                    <asp:BoundField DataField="credit_credit_id" SortExpression="credit_credit_id" />

                    <asp:BoundField DataField="credittype_descr" HeaderText="Type" SortExpression="credittype_credit_type_id" />

                    <asp:TemplateField HeaderText="Added" >
                        <ItemTemplate>
                            <%# Eval("credit_date_added", "{0:dd-MM-yyyy HH:mm}") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Added By" >
                        <ItemTemplate>
                            <%# Eval("person_added_by_firstname") + " " + Eval("person_added_by_surname") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Expiry Date">
                        <ItemTemplate>
                                <%# Eval("credit_expiry_date") == DBNull.Value ? "" : Eval("credit_expiry_date", "{0:dd-MM-yyyy}") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="credit_amount" HeaderText="Amount" SortExpression="credit_amount" />

                    <asp:TemplateField HeaderText="Used" >
                        <ItemTemplate>
                            <%# (int)Eval("credit_credit_type_id") != 1 ? "<font color=\"#9D9D9D\">N/A</font>" : Eval("credit_amount_used") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Remaining" >
                        <ItemTemplate>
                            <%# (int)Eval("credit_credit_type_id") != 1 ? "<font color=\"#9D9D9D\">N/A</font>" : ((decimal)Eval("credit_amount") - (decimal)Eval("credit_amount_used")).ToString() %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Voucher Descr">
                        <ItemTemplate>
                            <div style="max-width: 200px;">
                                <%# (int)Eval("credit_credit_type_id") != 1 ? "<font color=\"#9D9D9D\">N/A</font>" : Eval("credit_voucher_descr") %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Invoice" >
                        <ItemTemplate>
                            <%# (int)Eval("credit_credit_type_id") != 2 ? "<font color=\"#9D9D9D\">N/A</font>" : (Eval("credit_invoice_id") == DBNull.Value ? "" : "<a href=\"Invoice_ViewV2.aspx?invoice_id=" + Eval("credit_invoice_id") + "\" onclick=\"open_new_tab('Invoice_ViewV2.aspx?invoice_id=" + Eval("credit_invoice_id") + "');return false;\">Invoice</a>") %>
                        </ItemTemplate>
                    </asp:TemplateField>


                    <asp:TemplateField HeaderText="From Voucher" >
                        <ItemTemplate>
                            <%# 
                                (int)Eval("credit_credit_type_id") != 2  ? "<font color=\"#9D9D9D\">N/A</font>" :
                                (
                                    "ID: "    + Eval("credit_voucher_credit_id") + "<br />" +
                                    "Exp: "   + (Eval("vouchercredit_expiry_date") == DBNull.Value ? "[No Exp Date]" : Eval("vouchercredit_expiry_date", "{0:dd-MM-yyyy}")) + "<br />" +
                                    "<a href=\"#\" onclick=\"$(this).tooltip().mouseover();return false;\" title=\"" + Eval("vouchercredit_voucher_descr") + "\" style=\"color:inherit;\">(<u>Descr</u>)</a>" + "<br />" +
                                    "Amount: "    + Eval("vouchercredit_amount") + "<br />" +
                                    "Used: "    + Eval("vouchercredit_amount_used")
                                )
                            %>
                        </ItemTemplate>
                    </asp:TemplateField>




                    <asp:TemplateField HeaderText="Tyro Purchase ID" >
                        <ItemTemplate>
                            <%# ((int)Eval("credit_credit_type_id") != 3 && (int)Eval("credit_credit_type_id") != 4) ? "<font color=\"#9D9D9D\">N/A</font>" : Eval("credit_tyro_payment_pending_id") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Deleted" >
                        <ItemTemplate>
                            <%# Eval("credit_date_deleted") == DBNull.Value ? "" : Eval("credit_date_deleted", "{0:dd-MM-yyyy}") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Deleted By" >
                        <ItemTemplate>
                            <%# Eval("credit_deleted_by") == DBNull.Value ? "" : Eval("person_deleted_by_firstname") + " " + Eval("person_deleted_by_surname") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Pre-Delete Amt" >
                        <ItemTemplate>
                            <%# Eval("credit_deleted_by") == DBNull.Value ? "" : Eval("credit_pre_deleted_amount") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                        <ItemTemplate> 
                            <asp:ImageButton ID="lnkDelete" runat="server" ImageUrl="~/images/Delete-icon-16.png" CommandArgument='<%# Eval("credit_credit_id") %>' CommandName="_Delete" Visible='<%# (bool)Eval("can_delete") %>' OnClientClick="javascript:if (!confirm('Are you sure you want to delete this record?')) return false;" />
                        </ItemTemplate> 
                    </asp:TemplateField> 


                </Columns>
            </asp:GridView>




            <br />

            <p>
                <asp:Label ID="lblCurrentPath" runat="server"></asp:Label>
            </p>


            <asp:GridView ID="GrdScannedDoc" runat="server" 
                AutoGenerateColumns="False" 
                GridLines="None" 
                onpageindexchanging="GrdScannedDoc_PageIndexChanging" 
                onrowcommand="GrdScannedDoc_RowCommand" 
                onrowdatabound="GrdScannedDoc_RowDataBound"
                CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center vertical_align_top">
                <Columns>
                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lbFolderItem" CommandName="OpenFolder" CommandArgument='<%# Eval("Name") %>'></asp:LinkButton>
                            <asp:Literal runat="server" ID="ltlFileItem"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="FileSystemType" HeaderText="Type" SortExpression="FileSystemType" />
                    <asp:BoundField DataField="LastWriteTime" HeaderText="Date Modified" SortExpression="LastWriteTime" />
                    <asp:TemplateField HeaderText="Size" SortExpression="Size" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <%# DisplaySize((long?) Eval("Size")) %>
                        </ItemTemplate>

                        <ItemStyle HorizontalAlign="Right"></ItemStyle>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>



            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>


