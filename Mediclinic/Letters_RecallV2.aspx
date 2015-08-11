<%@ Page Title="Invoices" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Letters_RecallV2.aspx.cs" Inherits="Letters_RecallV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="Scripts/post_to_url.js"></script>
    <script type="text/javascript">

        function go_to_print_batch() {
            var h = new Object(); // or just {}
            h['selected_patient_ids'] = document.getElementById('hiddenPatientIDs').value.trim();
            post_to_url("Letters_PrintBatchV2.aspx", h, "post");
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Recall Letters</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:900px;">

                <div id="div_search_section" runat="server" class="border_top_bottom user_login_form_no_width_div">
                    <center>

                        <table style="margin:6px auto;">
                            <tr>
                                <td>

                                    <table style="line-height:6px;">
                                        <tr>
                                            <td rowspan="3" style="vertical-align:middle;">Last Visit</td>
                                            <td rowspan="3" style="width:10px;"></td>
                                            <td rowspan="3">Before</td>
                                            <td rowspan="3" style="width:5px;"></td>
                                            <td rowspan="3"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"/></td>
                                            <td rowspan="3"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                            <td rowspan="3" style="width:25px;"></td>
                                            <td style="vertical-align:bottom;"><label for="chkShowWithEPC">Show Those With EPC's Remaining</label></td>
                                            <td style="width:6px;"></td>
                                            <td style="vertical-align:top;"><asp:CheckBox ID="chkShowWithEPC" runat="server" Text="" Checked="true" /></td>
                                        </tr>
                                        <tr style="height:4px;">
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td style="vertical-align:bottom;"><label for="chkShowWithNoEPC">Show Those with No EPC or EPC Expired</label></td>
                                            <td></td>
                                            <td style="vertical-align:top;"><asp:CheckBox ID="chkShowWithNoEPC" runat="server" Text="" Checked="true" /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="100%" style="height:8px;"></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Clinic
                                            </td>
                                            <td></td>
                                            <td colspan="9">
                                                <asp:DropDownList ID="ddlClinics" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="100%" style="height:4px;"></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                No Recall Letters After 
                                            </td>
                                            <td></td>
                                            <td colspan="9">
                                                <table>
                                                    <tr>
                                                        <td><asp:TextBox ID="txtNoRecallLettersAfterDate" runat="server" Columns="10"/></td>
                                                        <td><asp:ImageButton ID="txtNoRecallLettersAfterDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                    </tr>
                                                </table>
                                                
                                            </td>
                                        </tr>

                                    </table>
                                </td>
                                <td style="width:25px;"></td>
                                <td>
                                    <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" Text="Update" />
                                </td>

                            </tr>

                        </table>
                        
                    <center>
                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <asp:HiddenField ID="hiddenPatientIDs" runat="server" />

            <center>

                <table>
                    <tr valign="top">
                        <td>

                            <table id="tbl_select_org_and_patient" runat="server">
                                <tr>
                                    <th style="text-align:left;">
                                        <center>
                                            <asp:Button ID="btnPrintBatch" runat="server" Text="Print Letters" OnClientClick="go_to_print_batch(); return false;" />
                                            &nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnExport" runat="server" OnClick="btnExport_Click" Text="Export List" />
                                        </center>
                                    </th>
                                    <th  class="hiddencol"></th>
                                    <td  class="hiddencol" style="text-align:left;">
                                        <table style="width:100%;">
                                            <tr>
                                                <th style="text-align:left;">Select Letter</th>
                                                <td style="text-align:right;"><asp:Button ID="btnPrint" runat="server" Text="&nbsp;&nbsp;&nbsp;Print&nbsp;&nbsp;&nbsp;" OnClick="btnPrint_Click" OnClientClick="javascript:clear_error_msg();" /> </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>

                                <tr id="tr_orgs_search_row_space_below" runat="server" height="6">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td></td>
                                    <td class="hiddencol" style="width:40px"></td>
                                    <td class="hiddencol"></td>
                                </tr>

                                <tr>
                                    <td style="vertical-align:top;">

                                        <div id="autodivheight" class="divautoheight" style="height:500px;">

                                            <asp:GridView ID="GrdPatient" runat="server" 
                                                 AutoGenerateColumns="False" DataKeyNames="patient_patient_id" 
                                                 OnRowCancelingEdit="GrdPatient_RowCancelingEdit" 
                                                 OnRowDataBound="GrdPatient_RowDataBound" 
                                                 OnRowEditing="GrdPatient_RowEditing" 
                                                 OnRowUpdating="GrdPatient_RowUpdating" ShowFooter="False" 
                                                 OnRowCommand="GrdPatient_RowCommand" 
                                                 OnRowDeleting="GrdPatient_RowDeleting" 
                                                 OnRowCreated="GrdPatient_RowCreated"
                                                 AllowSorting="True" 
                                                 OnSorting="GridView_Sorting"
                                                 RowStyle-VerticalAlign="top"
                                                 AllowPaging="True"
                                                 OnPageIndexChanging="GrdPatient_PageIndexChanging"
                                                 PageSize="17"
                                                 ClientIDMode="Predictable"
                                                 CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">
                                                 <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />
             

                                                <Columns> 

                                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_patient_id"> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblId" runat="server" Text='<%# Eval("patient_patient_id") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="Patient" HeaderStyle-HorizontalAlign="Left" SortExpression="person_firstname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left nowrap"> 
                                                        <ItemTemplate> 
                                                            <a href=javascript:void(0)'  onclick='<%# "open_new_tab(\"PatientDetailV2.aspx?type=view&id=" + Eval("patient_patient_id") + "\");return false;" %>' ><%# Eval("person_firstname") + " " + Eval("person_surname") %></a>
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="Last Booking" HeaderStyle-HorizontalAlign="Left" SortExpression="booking_date_start" FooterStyle-VerticalAlign="Top"> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblLastBkDate" runat="server" Text='<%# Eval("booking_date_start", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="Clinic" HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_name" FooterStyle-VerticalAlign="Top"  ItemStyle-CssClass="text_left nowrap"> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblClinic" runat="server" Text='<%# Eval("organisation_name") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="EPC Expires" HeaderStyle-HorizontalAlign="Left" SortExpression="epc_expire_date" FooterStyle-VerticalAlign="Top"> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblEPCExpiry" runat="server" Text='<%# Eval("epc_expire_date", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="EPCs Remaining" HeaderStyle-HorizontalAlign="Left" SortExpression="epc_count_remaining" FooterStyle-VerticalAlign="Top"> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblEPCsRemaining" runat="server" Text='<%# Eval("epc_count_remaining") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="Last Recall Letter Sent" HeaderStyle-HorizontalAlign="Left" SortExpression="most_recent_recall_sent" FooterStyle-VerticalAlign="Top"> 
                                                        <ItemTemplate> 
                                                            <asp:HyperLink ID="lnkLastRecallLetterSent" runat="server" Text='<%# Eval("most_recent_recall_sent", "{0:dd-MM-yyyy}") %>' NavigateUrl='<%# String.Format("~/Letters_SentHistoryV2.aspx?patient={0}", Eval("patient_patient_id")) %>' />
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                </Columns> 
                                            </asp:GridView>

                                        </div>

                                    </td>
                                    <td class="hiddencol"></td>
                                    <td class="hiddencol" style="vertical-align:top"><asp:ListBox ID="lstLetters" runat="server" rows="30" SelectionMode="Single" style="min-width:350px;"></asp:ListBox></td>
                                </tr>

                            </table>

                        </td>
                    </tr>
                </table>

            </center>

        </div>
    </div>

</asp:Content>



