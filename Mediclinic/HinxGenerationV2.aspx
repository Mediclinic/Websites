<%@ Page Title="Hinx Generation" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="HinxGenerationV2.aspx.cs" Inherits="HinxGenerationV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function expand_collapse(e_id) {
            var e = document.getElementById(e_id);
            if (e.style.display == "none")
                e.style.display = "block"
            else
                e.style.display = "none"
        }

        function expand_collapse_all(show) {

            var expand = (document.getElementById('div_invoice_list_to_be_claimed').style.display == "none" &&
                          document.getElementById('div_invoice_list_partially_paid_claims').style.display == "none" &&
                          document.getElementById('div_invoice_list_claim_manually').style.display == "none")

            expand = show || expand;

            document.getElementById('div_invoice_list_to_be_claimed').style.display = expand ? "block" : "none";
            document.getElementById('div_invoice_list_partially_paid_claims').style.display = expand ? "block" : "none";
            document.getElementById('div_invoice_list_claim_manually').style.display = expand ? "block" : "none";
        }

        function print_div(div_id) {
            var divToPrint = document.getElementById(div_id);
            newWin = window.open("");
            newWin.document.write(divToPrint.outerHTML);
            newWin.print();
            newWin.close();
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">HINX Generation</span></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">

                    <asp:HiddenField ID="lblSelectedOrgID" runat="server" Value="0" />

                    <table class="block_center" style="margin:6px auto;">
                        <tr>

                            <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">Start Date: </asp:Label></td>
                            <td class="nowrap"><asp:TextBox ID="txtStartDate" runat="server" Columns="10"/></td>
                            <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                            <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>

                            <td style="width:40px;"></td>

                            <td style="text-align:left;"><b>Medicare</b></td>
                            <td style="width:5px;"></td>
                            <td><asp:Button ID="btnGenerateMedicareReport" runat="server" OnClick="btnGenerateMedicareReport_Click" Text="Generate Report" /></td>
                            <td runat="server" visible="false" style="width:5px;"></td>
                            <td runat="server" visible="false"><asp:CheckBox ID="chkGenerateMedicareWordDoc" runat="server" Text="Print" /></td>
                            <td style="width:5px;"></td>
                            <td><asp:Button ID="btnGenerateMedicareHinxFiles" runat="server" OnClick="btnGenerateMedicareHinxFiles_Click" Text="Generate HINX Files" /></td>

                        </tr>
                        <tr>

                            <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                            <td class="nowrap"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                            <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                            <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>

                            <td></td>

                            <td style="text-align:left;"><b>DVA</b></td>
                            <td></td>
                            <td><asp:Button ID="btnGenerateDVAReport" runat="server" OnClick="btnGenerateDVAReport_Click" Text="Generate Report" /></td>
                            <td runat="server" visible="false"></td>
                            <td runat="server" visible="false"><asp:CheckBox ID="chkGenerateDVAWordDoc" runat="server" Text="Print" /></td>
                            <td></td>
                            <td><asp:Button ID="btnGenerateDVAHinxFiles" runat="server" OnClick="btnGenerateDVAHinxFiles_Click" Text="Generate HINX Files" /></td>

                        </tr>
                        <tr style="height:8px;">
                            <td colspan="12"></td>
                        </tr>
                        <tr>

                            <td colspan="12">
                                <span id="eClaimMinorID_row" runat="server">
                                    <asp:Label ID="lblEclaimDescr" runat="server" Text="eClaim Minor ID" />
                                    &nbsp;&nbsp;
                                    <asp:TextBox ID="txtEclaimNbr" runat="server" Columns="12" />

                                    <asp:Button ID="btnEclaimSetEditMode" runat="server" Text="Edit" OnClick="btnEclaimSetEditMode_Click" />
                                    <asp:Button ID="btnEclaimUpdate" runat="server" Text="Update" OnClick="btnEclaimUpdate_Click" CausesValidation="True" ValidationGroup="EditEclaimValidationSummary" />
                                    <asp:Button ID="btnEclaimCancelEditMode" runat="server" Text="Cancel Edit" OnClick="btnEclaimCancelEditMode_Click" />
                                </span>
                            </td>

                        </tr>
                    </table>


                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryEditEclaim" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEditEclaim"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <asp:RequiredFieldValidator ID="txtValidateEclaimNbrRequired" runat="server" CssClass="failureNotification"  
                Display="None"
                ControlToValidate="txtEclaimNbr" 
                ErrorMessage="eClaim Number is required."
                ValidationGroup="ValidationSummaryEditEclaim" />
            <asp:RegularExpressionValidator ID="txtValidateEclaimNbrRegex" runat="server" CssClass="failureNotification" 
                Display="None"
                ControlToValidate="txtEclaimNbr"
                ValidationExpression="^[a-zA-Z0-9]+$"
                ErrorMessage="eClaim Number can only be letters and digits."
                ValidationGroup="ValidationSummaryEditEclaim" />
            

            <div class="user_login_form">
                <div id="span_dev_stuff" runat="server" class="border_top_bottom">
                    <table class="block_center padded-table-2px text-left">
                        <tr>
                            <td>eClaim Nbr:&nbsp;</td>
                            <td><asp:TextBox ID="txtInvID" runat="server"></asp:TextBox></td>
                            <td><asp:Button ID="btnTestGenerate" runat="server" Text="Generate One" OnClick="btnTestGenerate_Click" Width="100%" /></td>
                        </tr>
                        <tr>
                            <td>Dir To Convert: &nbsp;</td>
                            <td><asp:TextBox ID="txtDirToConvert" Width="350" runat="server" Text="C:\Users\Eli\Documents\Mediclinic\Hinx\201208"></asp:TextBox></td>
                            <td><asp:Button ID="btnTestGenerateDir" runat="server" Text="Generate All" OnClick="btnTestGenerateDir_Click" Width="100%" /></td>
                        </tr>
                    </table>
                </div>
            </div>
            

            <table class="block_center">
                <tr>
                    <td>

                        <div id="div_full_report" runat="server" class="text-left">

                            <table>
                                <tr>
                                    <td style="min-width:225px">
                                        <b>To Be Claimed</b> (<asp:Label ID="lblToBeClaimedCount" runat="server" Font-Bold="True" Font-Size="Larger"/>)
                                    </td>
                                    <td style="min-width:300px">
                                        <a href="javascript:void(0)" onclick="expand_collapse('div_invoice_list_to_be_claimed');return false;">expand/collapse</a>
                                        &nbsp;&nbsp;&nbsp;
                                        <a href="javascript:void(0)" onclick="print_div('div_invoice_list_to_be_claimed');return false;">print</a>
                                    </td>
                                    <td>
                                        <a href="javascript:void(0)" onclick="expand_collapse_all();return false;">expand/collapse all</a>
                                    </td>
                                </tr>
                            </table>
                            <div style="line-height:7px;">&nbsp;</div>
                            <div id="div_invoice_list_to_be_claimed">
                                <asp:GridView ID="GrdInvoice_ToBeClaimed" runat="server" 
                                        AutoGenerateColumns="False" DataKeyNames="inv_invoice_id" 
                                        OnRowCancelingEdit="GrdInvoice_ToBeClaimed_RowCancelingEdit" 
                                        OnRowDataBound="GrdInvoice_ToBeClaimed_RowDataBound" 
                                        OnRowEditing="GrdInvoice_ToBeClaimed_RowEditing" 
                                        OnRowUpdating="GrdInvoice_ToBeClaimed_RowUpdating" ShowFooter="True" 
                                        OnRowCommand="GrdInvoice_ToBeClaimed_RowCommand" 
                                        OnRowDeleting="GrdInvoice_ToBeClaimed_RowDeleting" 
                                        OnRowCreated="GrdInvoice_ToBeClaimed_RowCreated"
                                        AllowSorting="True" 
                                        OnSorting="GrdInvoice_ToBeClaimed_Sorting"
                                        RowStyle-VerticalAlign="top"
                                        ClientIDMode="Predictable"
                                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">


                                    <Columns> 

                                        <asp:TemplateField HeaderText=""  HeaderStyle-HorizontalAlign="Left" SortExpression="display_group_id" HeaderStyle-CssClass="nowrap"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblGroup" runat="server" Text='<%# Bind("display_group_descr") %>' Font-Bold="True"></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Invoice #"  HeaderStyle-HorizontalAlign="Left" SortExpression="inv_invoice_id" HeaderStyle-CssClass="nowrap"> 
                                            <EditItemTemplate> 
                                                <asp:LinkButton ID="lnkId" runat="server" Text='<%# Bind("inv_invoice_id") %>' ></asp:LinkButton>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:LinkButton ID="lnkId" runat="server" Text='<%# Bind("inv_invoice_id") %>' ></asp:LinkButton>
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Inv Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="inv_invoice_date_added"  HeaderStyle-CssClass="nowrap" ItemStyle-CssClass="nowrap"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblDate" runat="server" Text='<%# Bind("inv_invoice_date_added", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblDate" runat="server" Text='<%# Bind("inv_invoice_date_added", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Booking"  HeaderStyle-HorizontalAlign="Left" SortExpression="inv_booking_id"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblBookinglId" runat="server" Text='<%# Bind("inv_booking_id") %>'></asp:Label>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblBookingId" runat="server" Text='<%# Bind("inv_booking_id") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Treat Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_date_start" HeaderStyle-CssClass="nowrap" ItemStyle-CssClass="nowrap"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblBookingStartDate" runat="server" Text='<%# Bind("booking_date_start", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblBookingStartDate" runat="server" Text='<%# Bind("booking_date_start", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Debtor"  HeaderStyle-HorizontalAlign="Left"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblPayer" runat="server"></asp:Label>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblPayer" runat="server"></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Claim Nbr" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_healthcare_claim_number" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="80px" ID="txtOrganisationClaimNumber" runat="server" Text='<%# Bind("inv_healthcare_claim_number") %>'></asp:TextBox> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblOrganisationClaimNumber" runat="server" Text='<%# Bind("inv_healthcare_claim_number") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Inv Total" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_total" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="60px" ID="txtTotal" runat="server" Text='<%# Bind("inv_total") %>'></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidateTotalRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtTotal" 
                                                    ErrorMessage="Amount is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidateTotalRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtTotal"
                                                    ValidationExpression="^\d+(\.\d{1,2})?$"
                                                    ErrorMessage="Amount can only be numbers and option decimal place with 1 or 2 digits following."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RegularExpressionValidator>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblTotal" runat="server" Text='<%# Bind("inv_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Total" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="GST" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_gst" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="60px" ID="txtGST" runat="server" Text='<%# Bind("inv_gst") %>'></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidateGSTRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtGST" 
                                                    ErrorMessage="GST is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidateGSTRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtGST"
                                                    ValidationExpression="^\d+(\.\d{1,2})?$"
                                                    ErrorMessage="GST can only be numbers and option decimal place with 1 or 2 digits following."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RegularExpressionValidator>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblGST" runat="server" Text='<%# Bind("inv_gst") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_GST" runat="server"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Receipts" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_receipts_total" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblReceipts" runat="server" Text='<%# Eval("inv_receipts_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Receipts" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Vouchers" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_vouchers_total" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblVouchers" runat="server" Text='<%# Eval("inv_vouchers_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Vouchers" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Adj" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_credit_notes_total" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblCreditNotes" runat="server" Text='<%# Eval("inv_credit_notes_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_CreditNotes" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Due" HeaderStyle-HorizontalAlign="Left" SortExpression="total_due" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblDue" runat="server" Text='<%# Eval("total_due") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Due" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Paid" SortExpression="inv_is_paid" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsPaid" runat="server" Text='<%# Eval("inv_is_paid").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Refund" SortExpression="inv_is_refund" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsRefund" runat="server" Text='<%# Eval("inv_is_refund").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Batched" SortExpression="inv_is_batched" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsBatched" runat="server" Text='<%# Eval("inv_is_batched").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditInvoiceValidationSummary"></asp:LinkButton> 
                                                <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                            </ItemTemplate> 
                                        </asp:TemplateField> 


                                    </Columns> 
                                </asp:GridView>
                            </div>

                            <br />

                            <table style="margin-left:0; margin-right:0;">
                                <tr>
                                    <td style="min-width:225px">
                                        <b>Partially Paid Claims</b> (<asp:Label ID="lblPartiallyPaidClaimsCount" runat="server" Font-Bold="True" Font-Size="Larger"/>)
                                    </td>
                                    <td>
                                        <a href="javascript:void(0)" onclick="expand_collapse('div_invoice_list_partially_paid_claims');return false;">expand/collapse</a>
                                        &nbsp;&nbsp;&nbsp;
                                        <a href="javascript:void(0)" onclick="print_div('div_invoice_list_partially_paid_claims');return false;">print</a>
                                    </td>
                                </tr>
                            </table>
                            <div style="line-height:7px;">&nbsp;</div>
                            <div id="div_invoice_list_partially_paid_claims">
                                <asp:GridView ID="GrdInvoice_PartiallyPaidClaims" runat="server" 
                                        AutoGenerateColumns="False" DataKeyNames="inv_invoice_id" 
                                        OnRowCancelingEdit="GrdInvoice_PartiallyPaidClaims_RowCancelingEdit" 
                                        OnRowDataBound="GrdInvoice_PartiallyPaidClaims_RowDataBound" 
                                        OnRowEditing="GrdInvoice_PartiallyPaidClaims_RowEditing" 
                                        OnRowUpdating="GrdInvoice_PartiallyPaidClaims_RowUpdating" ShowFooter="True" 
                                        OnRowCommand="GrdInvoice_PartiallyPaidClaims_RowCommand" 
                                        OnRowDeleting="GrdInvoice_PartiallyPaidClaims_RowDeleting" 
                                        OnRowCreated="GrdInvoice_PartiallyPaidClaims_RowCreated"
                                        AllowSorting="True" 
                                        OnSorting="GrdInvoice_PartiallyPaidClaims_Sorting"
                                        RowStyle-VerticalAlign="top"
                                        ClientIDMode="Predictable"
                                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">


                                    <Columns> 

                                        <asp:TemplateField HeaderText=""  HeaderStyle-HorizontalAlign="Left" SortExpression="display_group_id" HeaderStyle-CssClass="nowrap"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblGroup" runat="server" Text='<%# Bind("display_group_descr") %>' Font-Bold="True"></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Invoice #"  HeaderStyle-HorizontalAlign="Left" SortExpression="inv_invoice_id" HeaderStyle-CssClass="nowrap"> 
                                            <EditItemTemplate> 
                                                <asp:LinkButton ID="lnkId" runat="server" Text='<%# Bind("inv_invoice_id") %>' ></asp:LinkButton>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:LinkButton ID="lnkId" runat="server" Text='<%# Bind("inv_invoice_id") %>' ></asp:LinkButton>
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Inv Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="inv_invoice_date_added"  HeaderStyle-CssClass="nowrap" ItemStyle-CssClass="nowrap"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblDate" runat="server" Text='<%# Bind("inv_invoice_date_added", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblDate" runat="server" Text='<%# Bind("inv_invoice_date_added", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Booking"  HeaderStyle-HorizontalAlign="Left" SortExpression="inv_booking_id"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblBookinglId" runat="server" Text='<%# Bind("inv_booking_id") %>'></asp:Label>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblBookingId" runat="server" Text='<%# Bind("inv_booking_id") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Treat Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_date_start" HeaderStyle-CssClass="nowrap" ItemStyle-CssClass="nowrap"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblBookingStartDate" runat="server" Text='<%# Bind("booking_date_start", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblBookingStartDate" runat="server" Text='<%# Bind("booking_date_start", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Debtor"  HeaderStyle-HorizontalAlign="Left"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblPayer" runat="server"></asp:Label>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblPayer" runat="server"></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Claim Nbr" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_healthcare_claim_number" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="80px" ID="txtOrganisationClaimNumber" runat="server" Text='<%# Bind("inv_healthcare_claim_number") %>'></asp:TextBox> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblOrganisationClaimNumber" runat="server" Text='<%# Bind("inv_healthcare_claim_number") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Inv Total" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_total" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="60px" ID="txtTotal" runat="server" Text='<%# Bind("inv_total") %>'></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidateTotalRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtTotal" 
                                                    ErrorMessage="Amount is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidateTotalRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtTotal"
                                                    ValidationExpression="^\d+(\.\d{1,2})?$"
                                                    ErrorMessage="Amount can only be numbers and option decimal place with 1 or 2 digits following."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RegularExpressionValidator>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblTotal" runat="server" Text='<%# Bind("inv_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Total" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="GST" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_gst" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="60px" ID="txtGST" runat="server" Text='<%# Bind("inv_gst") %>'></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidateGSTRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtGST" 
                                                    ErrorMessage="GST is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidateGSTRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtGST"
                                                    ValidationExpression="^\d+(\.\d{1,2})?$"
                                                    ErrorMessage="GST can only be numbers and option decimal place with 1 or 2 digits following."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RegularExpressionValidator>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblGST" runat="server" Text='<%# Bind("inv_gst") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_GST" runat="server"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Receipts" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_receipts_total" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblReceipts" runat="server" Text='<%# Eval("inv_receipts_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Receipts" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Vouchers" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_vouchers_total" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblVouchers" runat="server" Text='<%# Eval("inv_vouchers_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Vouchers" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Adj" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_credit_notes_total" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblCreditNotes" runat="server" Text='<%# Eval("inv_credit_notes_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_CreditNotes" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Due" HeaderStyle-HorizontalAlign="Left" SortExpression="total_due" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblDue" runat="server" Text='<%# Eval("total_due") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Due" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Paid" SortExpression="inv_is_paid" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsPaid" runat="server" Text='<%# Eval("inv_is_paid").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Refund" SortExpression="inv_is_refund" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsRefund" runat="server" Text='<%# Eval("inv_is_refund").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Batched" SortExpression="inv_is_batched" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsBatched" runat="server" Text='<%# Eval("inv_is_batched").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditInvoiceValidationSummary"></asp:LinkButton> 
                                                <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                            </ItemTemplate> 
                                        </asp:TemplateField> 


                                    </Columns> 
                                </asp:GridView>
                            </div>

                            <br />

                            <table>
                                <tr>
                                    <td style="min-width:225px">
                                        <b>Claim Manually</b> (<asp:Label ID="lblClaimManuallyCount" runat="server" Font-Bold="True" Font-Size="Larger"/>)
                                    </td>
                                    <td>
                                        <a href="javascript:void(0)" onclick="expand_collapse('div_invoice_list_claim_manually');return false;">expand/collapse</a>
                                        &nbsp;&nbsp;&nbsp;
                                        <a href="javascript:void(0)" onclick="print_div('div_invoice_list_claim_manually');return false;">print</a>
                                    </td>
                                </tr>
                            </table>
                            <div style="line-height:7px;">&nbsp;</div>
                            <div id="div_invoice_list_claim_manually">
                                <asp:GridView ID="GrdInvoice_ClaimManually" runat="server" 
                                        AutoGenerateColumns="False" DataKeyNames="inv_invoice_id" 
                                        OnRowCancelingEdit="GrdInvoice_ClaimManually_RowCancelingEdit" 
                                        OnRowDataBound="GrdInvoice_ClaimManually_RowDataBound" 
                                        OnRowEditing="GrdInvoice_ClaimManually_RowEditing" 
                                        OnRowUpdating="GrdInvoice_ClaimManually_RowUpdating" ShowFooter="True" 
                                        OnRowCommand="GrdInvoice_ClaimManually_RowCommand" 
                                        OnRowDeleting="GrdInvoice_ClaimManually_RowDeleting" 
                                        OnRowCreated="GrdInvoice_ClaimManually_RowCreated"
                                        AllowSorting="True" 
                                        OnSorting="GrdInvoice_ClaimManually_Sorting"
                                        RowStyle-VerticalAlign="top"
                                        ClientIDMode="Predictable"
                                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">


                                    <Columns> 

                                        <asp:TemplateField HeaderText=""  HeaderStyle-HorizontalAlign="Left" SortExpression="display_group_id" HeaderStyle-CssClass="nowrap"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblGroup" runat="server" Text='<%# Bind("display_group_descr") %>' Font-Bold="True"></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Invoice #"  HeaderStyle-HorizontalAlign="Left" SortExpression="inv_invoice_id" HeaderStyle-CssClass="nowrap"> 
                                            <EditItemTemplate> 
                                                <asp:LinkButton ID="lnkId" runat="server" Text='<%# Bind("inv_invoice_id") %>' ></asp:LinkButton>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:LinkButton ID="lnkId" runat="server" Text='<%# Bind("inv_invoice_id") %>' ></asp:LinkButton>
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Inv Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="inv_invoice_date_added"  HeaderStyle-CssClass="nowrap" ItemStyle-CssClass="nowrap"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblDate" runat="server" Text='<%# Bind("inv_invoice_date_added", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblDate" runat="server" Text='<%# Bind("inv_invoice_date_added", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Booking"  HeaderStyle-HorizontalAlign="Left" SortExpression="inv_booking_id"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblBookinglId" runat="server" Text='<%# Bind("inv_booking_id") %>'></asp:Label>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblBookingId" runat="server" Text='<%# Bind("inv_booking_id") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Treat Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_date_start" HeaderStyle-CssClass="nowrap" ItemStyle-CssClass="nowrap"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblBookingStartDate" runat="server" Text='<%# Bind("booking_date_start", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblBookingStartDate" runat="server" Text='<%# Bind("booking_date_start", "{0:dd MMM yyyy}") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Debtor"  HeaderStyle-HorizontalAlign="Left"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblPayer" runat="server"></asp:Label>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblPayer" runat="server"></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Claim Nbr" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_healthcare_claim_number" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="80px" ID="txtOrganisationClaimNumber" runat="server" Text='<%# Bind("inv_healthcare_claim_number") %>'></asp:TextBox> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblOrganisationClaimNumber" runat="server" Text='<%# Bind("inv_healthcare_claim_number") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Inv Total" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_total" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="60px" ID="txtTotal" runat="server" Text='<%# Bind("inv_total") %>'></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidateTotalRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtTotal" 
                                                    ErrorMessage="Amount is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidateTotalRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtTotal"
                                                    ValidationExpression="^\d+(\.\d{1,2})?$"
                                                    ErrorMessage="Amount can only be numbers and option decimal place with 1 or 2 digits following."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RegularExpressionValidator>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblTotal" runat="server" Text='<%# Bind("inv_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Total" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="GST" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_gst" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="60px" ID="txtGST" runat="server" Text='<%# Bind("inv_gst") %>'></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidateGSTRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtGST" 
                                                    ErrorMessage="GST is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidateGSTRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtGST"
                                                    ValidationExpression="^\d+(\.\d{1,2})?$"
                                                    ErrorMessage="GST can only be numbers and option decimal place with 1 or 2 digits following."
                                                    Display="Dynamic"
                                                    ValidationGroup="EditInvoiceValidationSummary">*</asp:RegularExpressionValidator>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblGST" runat="server" Text='<%# Bind("inv_gst") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_GST" runat="server"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Receipts" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_receipts_total" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblReceipts" runat="server" Text='<%# Eval("inv_receipts_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Receipts" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Vouchers" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_vouchers_total" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblVouchers" runat="server" Text='<%# Eval("inv_vouchers_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Vouchers" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Adj" HeaderStyle-HorizontalAlign="Left" SortExpression="inv_credit_notes_total" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblCreditNotes" runat="server" Text='<%# Eval("inv_credit_notes_total") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_CreditNotes" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Due" HeaderStyle-HorizontalAlign="Left" SortExpression="total_due" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblDue" runat="server" Text='<%# Eval("total_due") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:Label ID="lblSum_Due" runat="server" Font-Bold="True"></asp:Label> 
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Paid" SortExpression="inv_is_paid" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsPaid" runat="server" Text='<%# Eval("inv_is_paid").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Refund" SortExpression="inv_is_refund" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsRefund" runat="server" Text='<%# Eval("inv_is_refund").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Batched" SortExpression="inv_is_batched" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsBatched" runat="server" Text='<%# Eval("inv_is_batched").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditInvoiceValidationSummary"></asp:LinkButton> 
                                                <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                            </ItemTemplate> 
                                        </asp:TemplateField> 


                                    </Columns> 
                                </asp:GridView>
                            </div>

                        </div>


                    </td>
                </tr>
            </table>


            <br />

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>


        </div>
    </div>

</asp:Content>



