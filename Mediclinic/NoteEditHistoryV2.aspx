<%@ Page Title="Note Edit History" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="NoteEditHistoryV2.aspx.cs" Inherits="NoteEditHistoryV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Note Information Editing History</span></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdNote" runat="server" 
                            AutoGenerateColumns="False" DataKeyNames="note_id" 
                            OnRowCancelingEdit="GrdNote_RowCancelingEdit" 
                            OnRowDataBound="GrdNote_RowDataBound" 
                            OnRowEditing="GrdNote_RowEditing" 
                            OnRowUpdating="GrdNote_RowUpdating" ShowFooter="False" 
                            OnRowCommand="GrdNote_RowCommand" 
                            OnRowDeleting="GrdNote_RowDeleting" 
                            OnRowCreated="GrdNote_RowCreated"
                            AllowSorting="True" 
                            OnSorting="GridView_Sorting"
                            RowStyle-VerticalAlign="top"
                            AllowPaging="False"
                            ClientIDMode="Predictable"
                            CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="note_history_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("note_history_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="note_type_descr" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("note_type_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Body Part" HeaderStyle-HorizontalAlign="Left" SortExpression="body_part_descr" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFirstname" runat="server" Text='<%# Eval("body_part_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Date"  HeaderStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAdded" runat="server" Text='<%# Eval("date_added", "{0:dd-MM-yy}")  %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Text" HeaderStyle-HorizontalAlign="Left" SortExpression="text" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFirstname" runat="server" Text='<%# Eval("text").ToString().Replace(Environment.NewLine, "<br />") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Added By"  HeaderStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("added_by_person_firstname") + " " + Eval("added_by_person_surname")  %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Mofidified"  HeaderStyle-HorizontalAlign="Left" SortExpression="staff_person_surname"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMofidified" runat="server" Text='<%# (Eval("date_modified") == DBNull.Value || Eval("modified_by") == DBNull.Value) ? string.Empty : Eval("date_modified", "{0:dd-MM-yy}") + "<br />" + Eval("modified_by_person_firstname") + " " + Eval("modified_by_person_surname")  %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Deleted"  HeaderStyle-HorizontalAlign="Left" SortExpression="staff_person_surname"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDeleted" runat="server" Text='<%# (Eval("date_deleted") == DBNull.Value || Eval("deleted_by") == DBNull.Value) ? string.Empty : Eval("date_deleted", "{0:dd-MM-yy}") + "<br />" + Eval("deleted_by_person_firstname") + " " + Eval("deleted_by_person_surname")  %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 
                    </asp:GridView>

                </div>

                <br />
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="window.returnValue=false;self.close();" />

            </center>

        </div>
    </div>


</asp:Content>



