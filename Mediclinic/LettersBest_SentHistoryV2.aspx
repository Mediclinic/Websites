<%@ Page Title="Letters Sent History" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="LettersBest_SentHistoryV2.aspx.cs" Inherits="LettersBest_SentHistoryV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Letter Print History - B.E.S.T.</asp:Label> &nbsp; <asp:HyperLink ID="lnkToEntity" runat="server"></asp:HyperLink></div>
        <div class="main_content" style="padding:10px 5px;">

            <div class="user_login_form">

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdLetterPrintHistory" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="lph_letter_print_history_id" 
                         OnRowCancelingEdit="GrdLetterPrintHistory_RowCancelingEdit" 
                         OnRowDataBound="GrdLetterPrintHistory_RowDataBound" 
                         OnRowEditing="GrdLetterPrintHistory_RowEditing" 
                         OnRowUpdating="GrdLetterPrintHistory_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdLetterPrintHistory_RowCommand" 
                         OnRowDeleting="GrdLetterPrintHistory_RowDeleting" 
                         OnRowCreated="GrdLetterPrintHistory_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         RowStyle-VerticalAlign="top" 
                         CellPadding="2"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="lph_letter_print_history_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("lph_letter_print_history_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Print Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="lph_date"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPrintDate" runat="server" Text='<%# Bind("lph_date", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Document"  HeaderStyle-HorizontalAlign="Left" SortExpression="letter_docname"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDocument" runat="server" Text='<%# Eval("letter_docname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patient"  HeaderStyle-HorizontalAlign="Left" SortExpression="person_patient_firstname"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPatient" runat="server" Text='<%# Eval("person_patient_firstname") + " " + Eval("person_patient_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 
                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>


</asp:Content>


