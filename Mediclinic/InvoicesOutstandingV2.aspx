<%@ Page Title="Outstanding Invoices" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="InvoicesOutstandingV2.aspx.cs" Inherits="InvoicesOutstandingV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Outstanding Invoices</span> &nbsp;&nbsp;&nbsp;<h5 style="display: inline;color:blue;">Automatically Emailed 1st Monday Of Every Month</h5></div>
        <div class="main_content">
            <div class="user_login_form hiddencol">

                <asp:CheckBox ID="chkUsePaging" runat="server" Text="use paging" Font-Size="X-Small" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" Checked="True" />

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>

                <table>
                    <tr style="vertical-align:top">
                        <td>

                            <center>
                                <table>
                                    <tr>
                                        <td><h4><asp:Label ID="lblPtHeading" runat="server">Outstanding By Patient&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label></h4></td>
                                        <td><asp:Button ID="btnPrintAllPatients" runat="server" Text="Print All" OnCommand="btnPrintAllPatients_Command" /></td>
                                        <td><asp:Button ID="btnEmailAllPatients" runat="server" Text="Email All" OnCommand="btnEmailAllPatients_Command" /></td>
                                        <td><asp:Button ID="btnExportAllPatients" runat="server" Text="Export All" OnCommand="btnExportAllPatients_Command" /></td>
                                    </tr>
                                </table>
                            </center>

                            <div style="height:15px;"></div>

                            <asp:GridView ID="GrdPtInvoicesOutstanding" runat="server" 
                                 AutoGenerateColumns="False" DataKeyNames="patient_id" 
                                 OnRowCancelingEdit="GrdPtInvoicesOutstanding_RowCancelingEdit" 
                                 OnRowDataBound="GrdPtInvoicesOutstanding_RowDataBound" 
                                 OnRowEditing="GrdPtInvoicesOutstanding_RowEditing" 
                                 OnRowUpdating="GrdPtInvoicesOutstanding_RowUpdating" ShowFooter="True" 
                                 OnRowCommand="GrdPtInvoicesOutstanding_RowCommand" 
                                 OnRowCreated="GrdPtInvoicesOutstanding_RowCreated"
                                 AllowSorting="True" 
                                 OnSorting="GrdPtInvoicesOutstanding_Sorting"
                                 AllowPaging="True"
                                 OnPageIndexChanging="GrdPtInvoicesOutstanding_PageIndexChanging"
                                 PageSize="16"
                                 ClientIDMode="Predictable"
                                 CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                 <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                                <Columns> 

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_id"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Eval("patient_id") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Firstname"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_firstname" ItemStyle-CssClass="text_left"> 
                                        <ItemTemplate> 
                                            <a href='PatientDetailV2.aspx?type=view&id=<%# Eval("patient_id") %>'><%# Eval("patient_firstname") %></a>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Surname"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_surname" ItemStyle-CssClass="text_left"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblSurname" runat="server" Text='<%# Eval("patient_surname") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Total Due (Count)"  HeaderStyle-HorizontalAlign="Left" SortExpression="total_due"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblTotalDue" runat="server" Text='<%# "<b>" + string.Format("{0:C}", Convert.ToDecimal(Eval("total_due") == DBNull.Value ? 0 : Eval("total_due"))) + "</b>"  + " (" + Eval("total_inv_count") + ")" %>'></asp:Label> 
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lblTotalSumDue" runat="server" Font-Bold="true"></asp:Label> 
                                        </FooterTemplate> 
                                    </asp:TemplateField> 


                                    <asp:TemplateField HeaderText="Invoice #"  HeaderStyle-HorizontalAlign="Left" SortExpression="invoice_id_first"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblInvoiceIDs" runat="server" Text='<%# Eval("invoice_ids") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Treated"  HeaderStyle-HorizontalAlign="Left" SortExpression="bk_treatement_date_first"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblTreatmentDates" runat="server" Text='<%# Eval("bk_treatement_dates") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Organisation"  HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" SortExpression="bk_org_first"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("bk_orgs") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Total"  HeaderStyle-HorizontalAlign="Left" SortExpression="bk_total_first"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("bk_totals") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Owing"  HeaderStyle-HorizontalAlign="Left" SortExpression="bk_owing_first"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblOwing" runat="server" Text='<%# Eval("bk_owings") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 


                                    <asp:TemplateField HeaderText="View Invoices"  HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="nowrap"> 
                                        <ItemTemplate> 
                                            &nbsp;&nbsp;<a href='InvoiceListV2.aspx?patient=<%# Eval("patient_id") %>&start_date=&end_date=&inc_medicare=0&inc_dva=0&inc_private=1&inc_paid=0&inc_unpaid=1'>Unpaid</a> &nbsp;&nbsp; <a href='InvoiceListV2.aspx?patient=<%# Eval("patient_id") %>&start_date=&end_date='>All</a>&nbsp;&nbsp;
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Print"  HeaderStyle-HorizontalAlign="Center"> 
                                        <ItemTemplate> 
                                            <asp:LinkButton ID="btnPrint" runat="server" CommandArgument='<%# Eval("patient_id") %>' CommandName="Print">Print</asp:LinkButton>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Email"  HeaderStyle-HorizontalAlign="Center"> 
                                        <ItemTemplate> 
                                            <asp:LinkButton ID="btnEmail" runat="server" CommandArgument='<%# Eval("patient_id") %>' CommandName="Email" >Email</asp:LinkButton>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Set"  HeaderStyle-HorizontalAlign="Center"> 
                                        <ItemTemplate> 
                                            <table style="width:100%">
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnSetAllPaid" runat="server" Text="Paid" CommandName="SetAllPaid" CommandArgument='<%# Eval("patient_id") %>'  />
                                                    </td>
                                                    <td style="text-align:right;">
                                                        <asp:Button ID="btnSetAllWiped" runat="server" Text="Wipe" CommandName="SetAllWiped" CommandArgument='<%# Eval("patient_id") %>'  />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                </Columns> 

                            </asp:GridView>

                        </td>
                        <td style="width:38px;">&nbsp;</td>
                        <td>

                            <center>
                                <table>
                                    <tr>
                                        <td><h4><asp:Label ID="lblFacHeading" runat="server" style="white-space:nowrap;">Outstanding By Facility&nbsp;&nbsp;&nbsp;&nbsp;</asp:Label></h4></td>
                                        <td><asp:Button ID="btnPrintAllFacs" runat="server" Text="Print All" OnCommand="btnPrintAllFacs_Command" /></td>
                                        <td><asp:Button ID="btnEmailAllFacs" runat="server" Text="Email All" OnCommand="btnEmailAllFacs_Command" /></td>
                                        <td><asp:Button ID="btnExportAllFacs" runat="server" Text="Export All" OnCommand="btnExportAllFacs_Command" /></td>
                                    </tr>
                                </table>
                            </center>

                            <div style="height:15px;"></div>

                            <asp:GridView ID="GrdOrgInvoicesOutstanding" runat="server" 
                                 AutoGenerateColumns="False" DataKeyNames="organisation_id" 
                                 OnRowCancelingEdit="GrdOrgInvoicesOutstanding_RowCancelingEdit" 
                                 OnRowDataBound="GrdOrgInvoicesOutstanding_RowDataBound" 
                                 OnRowEditing="GrdOrgInvoicesOutstanding_RowEditing" 
                                 OnRowUpdating="GrdOrgInvoicesOutstanding_RowUpdating" ShowFooter="True" 
                                 OnRowCommand="GrdOrgInvoicesOutstanding_RowCommand" 
                                 OnRowCreated="GrdOrgInvoicesOutstanding_RowCreated"
                                 AllowSorting="True" 
                                 OnSorting="GrdOrgInvoicesOutstanding_Sorting"
                                 AllowPaging="True"
                                 OnPageIndexChanging="GrdOrgInvoicesOutstanding_PageIndexChanging"
                                 PageSize="16"
                                 ClientIDMode="Predictable"
                                 CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                 <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                                <Columns> 

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_id"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Eval("organisation_id") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Name"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_firstname"> 
                                        <ItemTemplate> 
                                            <a href='OrganisationDetailV2.aspx?type=view&id=<%# Eval("organisation_id") %>'><%# Eval("name") %></a>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Total Due (Count)"  HeaderStyle-HorizontalAlign="Left" SortExpression="total_due"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblTotalDue" runat="server" Text='<%# "<b>" + string.Format("{0:C}", Convert.ToDecimal(Eval("total_due") == DBNull.Value ? 0 : Eval("total_due"))) + "</b>"  + " (" + Eval("total_inv_count") + ")" %>'></asp:Label> 
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="lblTotalSumDue" runat="server" Font-Bold="true"></asp:Label> 
                                        </FooterTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="View Invoices"  HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="nowrap"> 
                                        <ItemTemplate> 
                                            &nbsp;&nbsp;<a href='InvoiceListV2.aspx?orgs=<%# Eval("organisation_id") %>&start_date=&end_date=&inc_medicare=0&inc_dva=0&inc_private=1&inc_paid=0&inc_unpaid=1'>Unpaid</a> &nbsp;&nbsp; <a href='InvoiceListV2.aspx?orgs=<%# Eval("organisation_id") %>&start_date=&end_date='>All</a>&nbsp;&nbsp;
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Print"  HeaderStyle-HorizontalAlign="Center"> 
                                        <ItemTemplate> 
                                            <asp:LinkButton ID="btnPrint" runat="server" CommandArgument='<%# Eval("organisation_id") %>' CommandName="Print" >Print</asp:LinkButton>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Email"  HeaderStyle-HorizontalAlign="Center"> 
                                        <ItemTemplate> 
                                            <asp:LinkButton ID="btnEmail" runat="server" CommandArgument='<%# Eval("organisation_id") %>' CommandName="Email" >Email</asp:LinkButton>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Set"  HeaderStyle-HorizontalAlign="Center"> 
                                        <ItemTemplate> 
                                            <table style="width:100%">
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnSetAllPaid" runat="server" Text="Paid" CommandName="SetAllPaid" CommandArgument='<%# Eval("organisation_id") %>'  />
                                                    </td>
                                                    <td style="text-align:right;">
                                                        <asp:Button ID="btnSetAllWiped" runat="server" Text="Wipe" CommandName="SetAllWiped" CommandArgument='<%# Eval("organisation_id") %>'  />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                </Columns> 

                            </asp:GridView>

                        </td>
                    </tr>
                </table>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>


        </div>
    </div>


</asp:Content>



