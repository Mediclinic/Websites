<%@ Page Title="Batch Printing Status" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Letters_PrintBatch_StatusV2.aspx.cs" Inherits="Letters_PrintBatch_StatusV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Batch Printing Status</span></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">

                    <center>
                        <table style="text-align:left;" >
                            <tr>
                                <td>Total Sent </td>
                                <td style="min-width:6px;"></td>
                                <td><asp:Label ID="lblTotalSent" runat="server"></asp:Label></td>
                                <td style="min-width:35px;"></td>
                                <td>Total Un-Sent </td>
                                <td style="min-width:6px;"></td>
                                <td><asp:Label ID="lblTotalUnSent" runat="server"></asp:Label></td>
                            </tr>
                            <tr style="height:10px;">
                                <td colspan="7"></td>
                            </tr>
                            <tr>
                                <td>SMS Sent </td>
                                <td style="min-width:6px;"></td>
                                <td><asp:Label ID="lblSMSSent" runat="server"></asp:Label></td>
                                <td style="min-width:35px;"></td>
                                <td>SMS Un-Sent </td>
                                <td style="min-width:6px;"></td>
                                <td><asp:Label ID="lblSMSUnSent" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>Email Sent </td>
                                <td></td>
                                <td><asp:Label ID="lblEmailSent" runat="server"></asp:Label></td>
                                <td></td>
                                <td>Email Un-Sent </td>
                                <td></td>
                                <td><asp:Label ID="lblEmailUnSent" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>Print Sent </td>
                                <td></td>
                                <td><asp:Label ID="lblPrintSent" runat="server"></asp:Label></td>
                                <td></td>
                                <td>Print Un-Sent </td>
                                <td></td>
                                <td><asp:Label ID="lblPrintUnSent" runat="server"></asp:Label></td>
                            </tr>
                        </table>
                    </center>

                    <span style="padding-left:20px;" class="hiddencol"><asp:CheckBox ID="chkUsePaging" runat="server" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" Checked="False"/> &nbsp;<label for="chkUsePaging" style="font-weight:normal;">use paging</label> </span>
                    <span style="padding-left:20px;" class="hiddencol"><asp:CheckBox ID="chkShowDeleted" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowDeleted_CheckedChanged" Checked="False" /> &nbsp;<label for="chkShowDeleted" style="font-weight:normal;">show deleted</label> </span>
                </div>
            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdBulkLetterSendingQueue" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="bulk_letter_sending_queue_id" 
                         OnRowCancelingEdit="GrdBulkLetterSendingQueue_RowCancelingEdit" 
                         OnRowDataBound="GrdBulkLetterSendingQueue_RowDataBound" 
                         OnRowEditing="GrdBulkLetterSendingQueue_RowEditing" 
                         OnRowUpdating="GrdBulkLetterSendingQueue_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdBulkLetterSendingQueue_RowCommand" 
                         OnRowDeleting="GrdBulkLetterSendingQueue_RowDeleting" 
                         OnRowCreated="GrdBulkLetterSendingQueue_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         RowStyle-VerticalAlign="top"
                         AutoGenerateDeleteButton="False"
                         AllowPaging="True"
                         OnPageIndexChanging="GrdBulkLetterSendingQueue_PageIndexChanging"
                         PageSize="15"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                         <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="bulk_letter_sending_queue_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("bulk_letter_sending_queue_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Batch Nbr" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="name"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblBatchNumber" runat="server" Text='<%# Eval("bulk_letter_sending_queue_batch_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Send Method" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="letter_print_history_send_method_descr"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSendMethod" runat="server" Text='<%# Eval("letter_print_history_send_method_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Added" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="datetime_added"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAdded" runat="server" Text='<%# Eval("added_by_name") + "<br />" + Eval("datetime_added", "{0:d MMM, yyyy}")  %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Sending"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_date_added" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <table style="text-align:left;">
                                        <tr>
                                            <td>Start</td>
                                            <td style="min-width:5px;"></td>
                                            <td><asp:Label ID="lblSendingStart" runat="server" Text='<%# (Eval("datetime_sending_start") == DBNull.Value ? "Not Started" : Eval("datetime_sending_start", "{0:dd-MM-yy HH:mm:ss}")) %>'></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td>Finish</td>
                                            <td></td>
                                            <td><asp:Label ID="lblSendingFinish" runat="server" Text='<%# (Eval("datetime_sent") == DBNull.Value ? "Not Sent" : Eval("datetime_sent", "{0:dd-MM-yy HH:mm:ss}")) %>'></asp:Label></td>
                                        </tr>
                                    </table>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patient" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="patient_name"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPatient" runat="server" Text='<%# Eval("patient_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Referrer" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="referrer_name"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblReferrer" runat="server" Text='<%# Eval("referrer_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Mobile" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="phone_number"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("phone_number") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Email" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="email_to_address" ItemStyle-CssClass="nowrap text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblEmail" runat="server" Text='<%# (Eval("email_to_address").ToString().Length == 0 ? "" : "To: ") + Eval("email_to_address") + "<br /><br />" + (Eval("email_from_address").ToString().Length == 0 ? "" : "From: ") + Eval("email_from_name") + "<br />" + Eval("email_from_address") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Text" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblText" runat="server" Text='<%# Eval("text").ToString().Replace(Environment.NewLine, "<br />") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Letter To Attach" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLetterDocName" runat="server" Text='<%# Eval("letter_doc_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



