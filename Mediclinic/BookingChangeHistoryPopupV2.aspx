<%@ Page Title="Booking Change History" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="BookingChangeHistoryPopupV2.aspx.cs" Inherits="BookingChangeHistoryPopupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Booking Change History</asp:Label></div>
        <div class="main_content" style="padding:5px 5px;">

            <div class="user_login_form_no_width" style="width:450px;">

                <div class="border_top_bottom user_login_form_no_width_div" style="margin-bottom:20px;">

                    <center>
                        <asp:Label ID="lblHeadingDetail" runat="server">Booking Change History</asp:Label>
                    </center>
                
                </div>

            </div>



            <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <div id="autodivheight" class="divautoheight" style="height:500px;width:auto;padding-right:9px;">

                    <asp:GridView ID="GrdPatient" runat="server" 
                            AutoGenerateColumns="False" DataKeyNames="booking_change_history_booking_change_history_id" 
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
                            AllowPaging="False"
                            ClientIDMode="Predictable"
                            CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_change_history_booking_change_history_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("booking_change_history_booking_change_history_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Date Moved" HeaderStyle-HorizontalAlign="Left" SortExpression="booking_change_history_date_moved" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDateMo0ved" runat="server" Text='<%# Eval("booking_change_history_date_moved", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Previous Date Time" HeaderStyle-HorizontalAlign="Left" SortExpression="booking_change_history_previous_datetime" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPreviousDatetime" runat="server" Text='<%# Eval("booking_change_history_previous_datetime", "{0:dd-MM-yyyy h:mm}") +   (Eval("booking_change_history_previous_datetime") == DBNull.Value ? "" : (((DateTime)Eval("booking_change_history_previous_datetime")).Hour < 12 ? "am" : "pm")) %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Reason" HeaderStyle-HorizontalAlign="Left" SortExpression="booking_change_history_reason_descr" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMiddlename" runat="server" Text='<%# Eval("booking_change_history_reason_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Moved By"  HeaderStyle-HorizontalAlign="Left" SortExpression="staff_person_surname" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMovedBy" runat="server" Text='<%# Eval("staff_person_firstname") + " " + Eval("staff_person_surname") %>'></asp:Label>
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



