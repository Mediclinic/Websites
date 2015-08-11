<%@ Page Title="Bookings Report" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Report_BookingsV2.aspx.cs" Inherits="Report_BookingsV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/duplicate_person_modal_box.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function complete_booking(booking_id, is_clinic) {

            // show modal popup
            var type = "complete"; // can use cancel here 
            var result = is_clinic ?
                window.showModalDialog('BookingCreateInvoiceV2.aspx?booking=' + booking_id + '&type=' + type, '', 'dialogHide:yes;dialogWidth:1250px;dialogHeight:650px;center:yes;resizable:no; scroll:no') :
                window.showModalDialog('BookingCreateInvoiceAgedCareV2.aspx?booking=' + booking_id + '&type=' + type + '&completion_type=standard', '', 'dialogHide:yes;dialogWidth:1200px;dialogHeight:800px;center:yes;resizable:no; scroll:no');

            // popup download file window in case letter to print
            if (result == true)
                window.showModalDialog('DownloadFile.aspx', '', 'dialogWidth:10px;dialogHeight:10px;resizable:no; scroll:no');

            // reload page
            window.location.href = window.location.href;
        }

        function open_new_window(URL) {
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,height=" + screen.height + ',width=' + screen.width);
            NewWindow.location = URL;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Bookings Report</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width">

                <div class="border_top_bottom user_login_form_no_width_div">
                    <center>

                        <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="btnDefaultButton_NoSubmit" style="margin:10px auto;">
                            <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />

                            <table style="line-height:12px;">
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
                                        </table>
                                    </td>


                                    <td id="td_search_space2" runat="server" style="width:22px"></td>

                                    <td id="td_search_dates" runat="server">
                                        <table>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">Start Date: </asp:Label></td>
                                                <td class="nowrap">
                                                    <asp:Panel runat="server" DefaultButton="btnSearch">
                                                        <asp:TextBox ID="txtStartDate" runat="server" Columns="10"/>
                                                    </asp:Panel>
                                                </td>
                                                <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                <td class="nowrap"  style="line-height:normal;"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                                                <td class="nowrap">
                                                    <asp:Panel runat="server" DefaultButton="btnSearch">
                                                        <asp:TextBox ID="txtEndDate" runat="server" Columns="10"/>
                                                    </asp:Panel>
                                                </td>
                                                <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                <td class="nowrap"  style="line-height:normal;"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>
                                            </tr>
                                        </table>
                                        <table>
                                            <tr>
                                                <td style="width:15px;"></td>
                                                <td>
                                                    <asp:RadioButtonList id="dateTypeToUse" runat="server" AutoPostBack="False">
                                                        <asp:ListItem Value="booking_date_start" Selected="True">&nbsp;Treatment Date</asp:ListItem>
                                                        <asp:ListItem Value="booking_date_created">&nbsp;Date Booking Added</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:22px"></td>


                                    <td class="nowrap">
                                        <asp:CheckBox ID="chkIncCompleted" runat="server" Text="&nbsp;Inc Completed"/> 
                                        <br />
                                        <asp:CheckBox ID="chkIncIncomplete" runat="server" Text="&nbsp;Inc Incomplete"/> 
                                        <br />
                                        <asp:CheckBox ID="chkIncCancelled" runat="server" Text="&nbsp;Inc Cancelled"/> 
                                        <br />
                                        <asp:CheckBox ID="chkIncDeleted" runat="server" Text="&nbsp;Inc Deleted"/> 
                                    </td>

                                    <td style="width:22px"></td>

                                    <td class="nowrap">
                                        <asp:CheckBox ID="chkIncEPC" runat="server" Text="&nbsp;Inc Incomplete EPC"/> 
                                        <br />
                                        <asp:CheckBox ID="chkIncNonEPC" runat="server" Text="&nbsp;Inc Incomplete Non-EPC"/>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:CheckBox ID="chkOnlyPtSelfBookings" runat="server" Text="&nbsp;Only PT Self Added BK's"/>
                                    </td>

                                    <td style="width:22px"></td>

                                    <td>
                                      <table>
                                            <tr>
                                                <td class="nowrap" style="text-align:center;line-height:normal;">
                                                    <asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" Width="100%" style="min-width:75px;" />
                                                    <span style="line-height:7px;">&nbsp;</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap" style="text-align:center;line-height:normal;">
                                                    <asp:Button ID="btnExport" runat="server" Text="Export List" OnClick="btnExport_Click" Width="100%" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:35px"></td>

                                    <td><asp:CheckBox ID="chkUsePaging" runat="server" Text="&nbsp;use paging" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" CssClass="nowrap" /></td>

                                </tr>
                            </table>

                        </asp:Panel>

                    </center>
                </div>

            </div>

            <div style="height:12px;"></div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>



            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdSummaryReport" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="booking_booking_id" 
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
                        PageSize="15"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_booking_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("booking_booking_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkBookingSheetForPatient" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Booking Sheet" ToolTip="Booking Sheet" Visible="false" />
                                    <asp:Label ID="lnkBookingSheet" runat="server" AlternateText="Booking Sheet" ToolTip="Booking Sheet" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:LinkButton ID="btnPrintInvoice" runat="server" CommandName="PrintInvoice" CommandArgument='<%# Eval("booking_booking_id") %>' Visible='<%# (Eval("invoice_lines_count") == DBNull.Value || (int)Eval("invoice_lines_count") == 0) ? false : true  %>'>Print Inv</asp:LinkButton>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:LinkButton ID="btnEmailInvoice" runat="server" CommandName="EmailInvoice" CommandArgument='<%# Eval("booking_booking_id") %>' Visible='<%# (Eval("invoice_lines_count") == DBNull.Value || (int)Eval("invoice_lines_count") == 0) ? false : true  %>'>Email Inv</asp:LinkButton>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Organisation"  SortExpression="organisation_name" ItemStyle-CssClass="nowrap text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Bind("organisation_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Treatment Date" SortExpression="booking_date_start" ItemStyle-CssClass="nowrap" ItemStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAppointmentDate"      runat="server" Text='<%# Eval("booking_date_start", "{0:dd MMM yyyy}") %>'></asp:Label>&nbsp;
                                    <asp:Label ID="lblAppointmentDateStart" runat="server" Text='<%# Eval("booking_date_start", "{0:HH:mm}") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Date Added" SortExpression="booking_date_created" ItemStyle-CssClass="nowrap" ItemStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("booking_date_created") == DBNull.Value ? "" : Eval("booking_date_created", "{0:dd MMM yyyy}") %>'></asp:Label>
                                    <a runat="server" href="javascript:void(0)"  onclick="javascript:return false;" title='<%#  Eval("added_by_deleted_by_row") %>' style="text-decoration: none"><b>&nbsp;*&nbsp;</b></a>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Provider"  SortExpression="provider_surname, provider_firstname" ItemStyle-CssClass="nowrap text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblProvider" runat="server" Text='<%# Eval("provider_firstname") + " " + Eval("provider_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patient"  SortExpression="patient_surname, patient_firstname" ItemStyle-CssClass="nowrap text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lnkPatient" runat="server" Text='<%# Eval("patient_firstname") + " " + Eval("patient_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Status"  SortExpression="booking_status_descr" ItemStyle-Wrap="false"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("booking_status_descr")  %>' Font-Bold='<%# Eval("booking_status_id") != DBNull.Value && (int)Eval("booking_status_id") == 0  %>' ></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="EPC"  SortExpression="booking_is_epc_text" ItemStyle-CssClass="nowrap" HeaderStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblInvType" runat="server" Text='<%# Eval("booking_is_epc_text") %>' ></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Complete" ItemStyle-CssClass="nowrap" HeaderStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblComplete" runat="server" ></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="Cash"  SortExpression="total_cash_receipts"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCash" runat="server" Text='<%# Eval("total_cash_receipts") == DBNull.Value || (decimal)Eval("total_cash_receipts") == 0 ? "" : Eval("total_cash_receipts")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_Cash" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Cheque"  SortExpression="total_cheque_receipts"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCheque" runat="server" Text='<%# Eval("total_cheque_receipts") == DBNull.Value || (decimal)Eval("total_cheque_receipts") == 0 ? "" : Eval("total_cheque_receipts")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_Cheque" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Credit Card"  SortExpression="total_credit_card_receipts"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCreditCard" runat="server" Text='<%# Eval("total_credit_card_receipts") == DBNull.Value || (decimal)Eval("total_credit_card_receipts") == 0 ? "" : Eval("total_credit_card_receipts")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_CreditCard" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="HICAPS"  SortExpression="total_eft_receipts"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblEFT" runat="server" Text='<%# Eval("total_eft_receipts") == DBNull.Value || (decimal)Eval("total_eft_receipts") == 0 ? "" : Eval("total_eft_receipts")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_EFT" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Money Order"  SortExpression="total_money_order_receipts"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMoneyOrder" runat="server" Text='<%# Eval("total_money_order_receipts") == DBNull.Value || (decimal)Eval("total_money_order_receipts") == 0 ? "" : Eval("total_money_order_receipts")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_MoneyOrder" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Direct Debit"  SortExpression="total_direct_credit_receipts"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDirectDebit" runat="server" Text='<%# Eval("total_direct_credit_receipts") == DBNull.Value || (decimal)Eval("total_direct_credit_receipts") == 0 ? "" : Eval("total_direct_credit_receipts")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_DirectDebit" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="DVA"  SortExpression="dva_invoices_total"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDVA" runat="server" Text='<%# Eval("dva_invoices_total") == DBNull.Value || (decimal)Eval("dva_invoices_total") == 0 ? "" : Eval("dva_invoices_total")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_DVA" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Medicare"  SortExpression="medicare_invoices_total"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMedicare" runat="server" Text='<%# Eval("medicare_invoices_total") == DBNull.Value || (decimal)Eval("medicare_invoices_total") == 0 ? "" : Eval("medicare_invoices_total")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_Medicare" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Vouchers"  SortExpression="total_vouchers"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTotalVouchers" runat="server" Text='<%# Eval("total_vouchers") == DBNull.Value || (decimal)Eval("total_vouchers") == 0 ? "" : Eval("total_vouchers")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_TotalVouchers" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Adj Notes"  SortExpression="total_credit_notes"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTotalCreditNotes" runat="server" Text='<%# Eval("total_credit_notes") == DBNull.Value || (decimal)Eval("total_credit_notes") == 0 ? "" : Eval("total_credit_notes")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_TotalCreditNotes" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Owing"  SortExpression="total_due_non_medicare_non_dva"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTotalDue" runat="server" Text='<%# Eval("total_due_non_medicare_non_dva") == DBNull.Value ? "" : Eval("total_due_non_medicare_non_dva")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_TotalDue" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 



                            <asp:TemplateField HeaderText="Service"  SortExpression="offering_name" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOfferingName" runat="server" Text='<%# Eval("offering_name")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Service Price"  SortExpression="offering_default_price"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOfferingDefaultPrice" runat="server" Text='<%# Eval("offering_default_price")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Booking"  SortExpression="booking_booking_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblBooking" runat="server" Text='<%# Eval("booking_booking_id")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Inv Amt<br />(inc. GST)"  SortExpression="invoices_total"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblInvoiceTotal" runat="server" Text='<%# Eval("invoices_total")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_InvoiceTotal" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Inv Amt Less Adj Notes<br />(inc. GST)"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblInvoiceTotalLessAdJNotes" runat="server" Text='<%# (Eval("invoices_total") == DBNull.Value ? 0 : (decimal)Eval("invoices_total")) - (Eval("total_credit_notes") == DBNull.Value ? 0 : (decimal)Eval("total_credit_notes"))  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_InvoiceTotalLessAdJNotes" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="GST"  SortExpression="invoices_gst_total"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblInvoiceGSTTotal" runat="server" Text='<%# Eval("invoices_gst_total")  %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_InvoiceGSTTotal" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Inv Lines" FooterStyle-HorizontalAlign="Right" FooterStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblInvLines" runat="server" Text='<%# Eval("invoice_lines_html") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblSum_InvoiceLines" runat="server" Font-Bold="True"></asp:Label>&nbsp;
                                </FooterTemplate> 
                            </asp:TemplateField> 


                        </Columns> 

                    </asp:GridView>

                </div>
            </center>
            

        </div>
    </div>

</asp:Content>



