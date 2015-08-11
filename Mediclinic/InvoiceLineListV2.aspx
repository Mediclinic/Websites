<%@ Page Title="Invoice Items" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="InvoiceLineListV2.aspx.cs" Inherits="InvoiceLineListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function open_new_window(URL) {
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,height=" + screen.height + ',width=' + screen.width);
            NewWindow.location = URL;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Invoice Items Report</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:1250px;">

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
                                            <tr id="offeringRow" runat="server">
                                                <td>
                                                    <asp:DropDownList ID="ddlOfferings" runat="server" OnSelectedIndexChanged="ddlOfferings_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList>
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
                                        <asp:RadioButtonList ID="rblDateType" runat="server">
                                            <asp:ListItem Text="Inv Date"   Value="Invoices" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Treatment Date" Value="Bookings" ></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>

                                    <td style="width:30px"></td>

                                    <td>
                                        <table>
                                            <tr>
                                                <td class="nowrap" style="text-align:center">
                                                    <asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" Width="100%" style="min-width:75px;" />
                                                    <div style="line-height:7px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap" style="text-align:center">
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
                        AutoGenerateColumns="False" DataKeyNames="invoice_line_id" 
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
                        PageSize="16"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_booking_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("invoice_line_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Provider"  SortExpression="provider_surname, provider_firstname" ItemStyle-CssClass="nowrap text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblProvider" runat="server" Text='<%# Eval("provider_firstname") + " " + Eval("provider_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Recorded" SortExpression="booking_date_created" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                        <asp:Label ID="lblAppointmentAddedDate" runat="server" Text='<%# Eval("booking_date_last_moved") != DBNull.Value ? Eval("booking_date_last_moved", "{0:dd MMM yyyy}") : Eval("booking_date_created", "{0:dd MMM yyyy}") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Booked for" SortExpression="date_start" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                        <asp:Label ID="lblAppointmentDate"      runat="server" Text='<%# Eval("booking_date_start", "{0:dd MMM yyyy}") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Start" SortExpression="date_start" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                        <asp:Label ID="lblAppointmentStart" runat="server" Text='<%# Eval("booking_date_start", "{0:H:mm}") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="End" SortExpression="date_end" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                        <asp:Label ID="lblAppointmentEnd" runat="server" Text='<%# Eval("booking_date_end", "{0:H:mm}") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Durtaion" SortExpression="booking_duration_total_minutes" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                        <asp:Label ID="lblAppointmentDuration" runat="server" Text='<%# Eval("booking_duration_total_minutes") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 
            
                            <asp:TemplateField HeaderText="Inv date" SortExpression="invoice_date_added" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                        <asp:Label ID="lblInvoiceDateAdded" runat="server" Text='<%# Eval("invoice_date_added", "{0:dd MMM yyyy}") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Clinic"  SortExpression="organisation_name" ItemStyle-CssClass="nowrap text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lbOrganisationName" runat="server" Text='<%# Bind("organisation_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Booking"  SortExpression="booking_id" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="BookingId" runat="server" Text='<%# Bind("booking_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>
             
                            <asp:TemplateField HeaderText="Patient"  SortExpression="patient_surname, patient_firstname" ItemStyle-CssClass="nowrap text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lnkPatient" runat="server" Text='<%# Eval("patient_firstname") + " " + Eval("patient_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Offering"  SortExpression="offering_name" ItemStyle-CssClass="nowrap text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOfferingName" runat="server" Text='<%# Eval("offering_name")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Amount<br />(inc. GST)"  SortExpression="invoice_line_price"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblInvoiceLinePrice" runat="server" Text='<%# Eval("invoice_line_price")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_Amount" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="GST"  SortExpression="invoice_line_tax"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblInvoiceLineGST" runat="server" Text='<%# Eval("invoice_line_tax")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_GST" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Qty"  SortExpression="invoice_line_quantity"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblInvoiceLineQuantity" runat="server" Text='<%# Eval("invoice_line_quantity") == DBNull.Value ? "" : (Convert.ToDecimal(Eval("invoice_line_quantity")) == Convert.ToDecimal((int)Convert.ToDecimal(Eval("invoice_line_quantity"))) ? Convert.ToDecimal((int)Convert.ToDecimal(Eval("invoice_line_quantity"))) : Eval("invoice_line_quantity"))  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Rebates <a onclick='return false;' href='?' title='This is the credit notes total for the whole invoice, which can be more than the individual invoice LINE item amount' >*</a>"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTotalCreditNotes" runat="server" Text='<%# Eval("total_credit_notes") == DBNull.Value || (decimal)Eval("total_credit_notes") == 0 ? "" : Eval("total_credit_notes")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_Rebates" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Comm. %"  SortExpression="commission_percent_amount"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCommissionPercent" runat="server" Text='<%# Eval("commission_percent_text")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_CommissionPercent" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Comm. Fixed"  SortExpression="fixed_rate_amount"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCommissionFixed" runat="server" Text='<%# Eval("fixed_rate_text")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_CommissionFixed" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 


                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



