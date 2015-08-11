<%@ Page Title="Receipts" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReceiptsListV2.aspx.cs" Inherits="ReceiptsListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Receipts</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:1150px;">

                <div id="div_search_section" runat="server" class="border_top_bottom user_login_form_no_width_div">
                    <center>

                        <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="btnDefaultButton_NoSubmit" style="margin:6px auto;">
                            <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />
                            <table>
                                <tr>

                                    <td  id="td_orgs_patients" runat="server" valign="middle">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:DropDownList ID="ddlOrgs" runat="server" OnSelectedIndexChanged="ddlOrgs_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr id="providerRow" runat="server">
                                                <td>
                                                    <asp:DropDownList ID="ddlProviders" runat="server" OnSelectedIndexChanged="ddlProviders_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr id="receiptPaymentTyperow" runat="server">
                                                <td>
                                                    <asp:DropDownList ID="ddlReceiptPaymentType" runat="server" OnSelectedIndexChanged="ddlReceiptPaymentType_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td id="td_search_space2" runat="server" style="width:30px"></td>

                                    <td id="td_search_dates" runat="server">
                                        <table>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">Start Date: </asp:Label></td>
                                                <td class="nowrap"><asp:TextBox ID="txtStartDate" runat="server" Columns="10"/></td>
                                                <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                                                <td class="nowrap"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                                                <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:30px"></td>

                                    <td class="nowrap">
                                        <table style="line-height:10px;">
                                            <tr style="vertical-align:top;">
                                                <td class="nowrap">
                                                    <asp:CheckBox ID="chkIncMedicare"   runat="server" Text="&nbsp;Inc Medicare"/> 
                                                    <br />
                                                    <asp:CheckBox ID="chkIncDVA"        runat="server" Text="&nbsp;Inc DVA" />
                                                    <br />
                                                    <asp:CheckBox ID="chkIncPrivate"    runat="server" Text="&nbsp;Inc PT Payable" />
                                                    <br />
                                                    <br />
                                                    <asp:CheckBox ID="chkIncReconciled" runat="server" Text="&nbsp;Inc Reconciled" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:30px"></td>

                                    <td>
                                        <table>
                                            <tr>
                                                <td class="nowrap">
                                                    <asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" Width="100%" style="min-width:75px;" />
                                                    <div style="line-height:7px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap">
                                                    <asp:Button ID="btnExport" runat="server" Text="Export List" OnClick="btnExport_Click" Width="100%" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:55px"></td>

                                    <td><asp:CheckBox ID="chkUsePaging" runat="server" Text="&nbsp;use paging" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" CssClass="nowrap" /></td>

                                </tr>
                            </table>
                        </asp:Panel>
                        
                    <center>
                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <br />

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdSummaryReport" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="receipt_id" 
                        OnRowCancelingEdit="GrdSummaryReport_RowCancelingEdit" 
                        OnRowDataBound="GrdSummaryReport_RowDataBound" 
                        OnRowEditing="GrdSummaryReport_RowEditing" 
                        OnRowUpdating="GrdSummaryReport_RowUpdating" ShowFooter="True" 
                        OnRowCommand="GrdSummaryReport_RowCommand" 
                        OnRowDeleting="GrdSummaryReport_RowDeleting" 
                        OnRowCreated="GrdSummaryReport_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GridView_Sorting"
                        AllowPaging="True"
                        OnPageIndexChanging="GrdStaff_PageIndexChanging"
                        PageSize="14"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="receipt_id" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("receipt_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Date Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="receipt_date_added" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDate" runat="server" Text='<%# Eval("receipt_date_added", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Type"  HeaderStyle-HorizontalAlign="Left" SortExpression="receipt_payment_type_descr" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblType" runat="server" Text='<%# Eval("receipt_payment_type_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Total"  HeaderStyle-HorizontalAlign="Left" SortExpression="total" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("total") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_Total" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Reconciled"  HeaderStyle-HorizontalAlign="Left" SortExpression="amount_reconciled" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAmountReconciled" runat="server" Text='<%# Eval("amount_reconciled") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_AmountReconciled" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Added By"  HeaderStyle-HorizontalAlign="Left" SortExpression="receipt_staff_person_firstname" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("receipt_staff_person_firstname") + (Eval("receipt_staff_person_firstname") == DBNull.Value ? "" : " ") + Eval("receipt_staff_person_surname")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Organisation"  HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_name" FooterStyle-BorderStyle="None" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisationName" runat="server" Text='<%# Eval("organisation_name")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Reversed"  HeaderStyle-HorizontalAlign="Left" SortExpression="reversed_date" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblReversed" runat="server" Text='<%#   Eval("reversed_date") == DBNull.Value ? "" : (Eval("reversed_date", "{0:dd MMM yyyy}") + "<br />" + "By " + Eval("receipt_reversed_by_person_firstname") + (Eval("receipt_reversed_by_person_firstname").ToString().Length == 0 ? "" : " ") + Eval("receipt_reversed_by_person_surname") + (Eval("receipt_reversed_by_person_firstname").ToString().Length + Eval("receipt_reversed_by_person_surname").ToString().Length == 0 ? "" : "<br />") + "Previously " + Eval("pre_reversed_amount") )  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Treatment Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_date_start" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAppointmentDate"      runat="server" Text='<%# Eval("booking_date_start", "{0:dd MMM yyyy}") %>'></asp:Label>&nbsp;
                                    <asp:Label ID="lblAppointmentDateStart" runat="server" Text='<%# Eval("booking_date_start", "{0:HH:mm}") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Invoice #"  HeaderStyle-HorizontalAlign="Left" SortExpression="invoice_id" HeaderStyle-CssClass="nowrap" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:LinkButton ID="lnkInvoiceID" runat="server" Text='<%# Bind("invoice_id") %>' ></asp:LinkButton>
                                    &nbsp;
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Inv Debtor"  HeaderStyle-HorizontalAlign="Left" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPayer" runat="server"></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patient" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPatient" runat="server" Text="Patient Info"></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkBookingSheetForPatient" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Booking Sheet" ToolTip="Booking Sheet" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-BorderStyle="None"> 
                                <ItemTemplate> 
                                    <asp:Button ID="btnSetReconciled" runat="server" Text="Set Reconciled" CommandArgument='<%# Eval("receipt_id") %>' CommandName="SetReconciled" OnClientClick="if (!confirm('Are you sure you want to set this as reconciled?')) return false;" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



