<%@ Page Title="Staff Email Reminders History" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Notifications_StaffReminderEmailHistoryV2.aspx.cs" Inherits="Notifications_StaffReminderEmailHistoryV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Staff Email Reminders History</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">

            <div class="user_login_form">

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdEmailHistory" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="email_history_id" 
                         OnRowCancelingEdit="GrdEmailHistory_RowCancelingEdit" 
                         OnRowDataBound="GrdEmailHistory_RowDataBound" 
                         OnRowEditing="GrdEmailHistory_RowEditing" 
                         OnRowUpdating="GrdEmailHistory_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdEmailHistory_RowCommand" 
                         OnRowDeleting="GrdEmailHistory_RowDeleting" 
                         OnRowCreated="GrdEmailHistory_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         AllowPaging="True"
                         OnPageIndexChanging="GrdPatient_PageIndexChanging"
                         PageSize="16"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">
                         <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="email_history_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("email_history_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Sent"  HeaderStyle-HorizontalAlign="Left" SortExpression="datetime_sent" ItemStyle-Wrap="False"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSent" runat="server" Text='<%# Eval("datetime_sent", "{0:dd MMM  HH:mm}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Email"  HeaderStyle-HorizontalAlign="Left" SortExpression="email"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("email") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Message"  HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMessage" runat="server" Text='<%# Eval("message") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 

                    </asp:GridView>


                </div>
            </center>

        </div>
    </div>


</asp:Content>


