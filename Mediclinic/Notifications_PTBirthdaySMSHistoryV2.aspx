<%@ Page Title="Patient SMS Birthday History" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Notifications_PTBirthdaySMSHistoryV2.aspx.cs" Inherits="Notifications_PTBirthdaySMSHistoryV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Patient SMS Birthday History</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">

            <div class="user_login_form">

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdSMSHistory" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="sms_history_id" 
                         OnRowCancelingEdit="GrdSMSHistory_RowCancelingEdit" 
                         OnRowDataBound="GrdSMSHistory_RowDataBound" 
                         OnRowEditing="GrdSMSHistory_RowEditing" 
                         OnRowUpdating="GrdSMSHistory_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdSMSHistory_RowCommand" 
                         OnRowDeleting="GrdSMSHistory_RowDeleting" 
                         OnRowCreated="GrdSMSHistory_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         AllowPaging="True"
                         OnPageIndexChanging="GrdPatient_PageIndexChanging"
                         PageSize="16"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">
                         <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="sms_history_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("sms_history_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Sent"  HeaderStyle-HorizontalAlign="Left" SortExpression="datetime_sent" ItemStyle-Wrap="False"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSent" runat="server" Text='<%# Eval("datetime_sent", "{0:dd MMM  HH:mm}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patient"  HeaderStyle-HorizontalAlign="Left" ItemStyle-Wrap="False" > 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPatient" runat="server" Text='<%# Eval("person_patient_firstname") + (Eval("person_patient_firstname").ToString().Length > 0 ? " " : "") + Eval("person_patient_surname")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Phone Nbr"  HeaderStyle-HorizontalAlign="Left" SortExpression="phone_number"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPhoneNbr" runat="server" Text='<%# Eval("phone_number") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Message"  HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMessage" runat="server" Text='<%# Eval("message").ToString().Replace(Environment.NewLine, "<br />") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Cost"  HeaderStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCost" runat="server" Text='<%# Eval("cost") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Ext Message ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="smstech_message_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSMSTechMessageID" runat="server" Text='<%# Eval("smstech_message_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Ext Status"  HeaderStyle-HorizontalAlign="Left" SortExpression="smstech_status"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSMSTechStatus" runat="server" Text='<%# Eval("smstech_status") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Ext Response"  HeaderStyle-HorizontalAlign="Left" SortExpression="smstech_datetime"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSMSTechResponseDateTime" runat="server" Text='<%# Eval("smstech_datetime", "{0:dd-MM-yy HH:mm:ss}")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>


</asp:Content>


