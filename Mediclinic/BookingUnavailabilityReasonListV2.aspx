<%@ Page Title="Booking Unavailability Reasons" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="BookingUnavailabilityReasonListV2.aspx.cs" Inherits="BookingUnavailabilityReasonListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Booking Unavailability Reasons</span></div>
        <div class="main_content" style="padding:20px 5px;">
            <div class="user_login_form">

            </div>

            <div class="block_center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdBookingUnavailabilityReason" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="booking_unavailability_reason_id" 
                         OnRowCancelingEdit="GrdBookingUnavailabilityReason_RowCancelingEdit" 
                         OnRowDataBound="GrdBookingUnavailabilityReason_RowDataBound" 
                         OnRowEditing="GrdBookingUnavailabilityReason_RowEditing" 
                         OnRowUpdating="GrdBookingUnavailabilityReason_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdBookingUnavailabilityReason_RowCommand" 
                         OnRowCreated="GrdBookingUnavailabilityReason_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_unavailability_reason_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("booking_unavailability_reason_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("booking_unavailability_reason_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" ItemStyle-Width="250"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="75%" ID="txtDescr" runat="server" Text='<%# Bind("descr") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateDescrRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtDescr" 
                                        ErrorMessage="Descr is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateDescrRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtDescr"
                                        ValidationExpression="^[a-zA-Z0-9\s\-_\(\)\[\[\*]+$"
                                        ErrorMessage="Descr can only be letters, numbers, underscore, hpyhen, brackets or spaces."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="75%" ID="txtNewDescr" runat="server" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewDescrRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDescr" 
                                        ErrorMessage="Descr is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDescrRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDescr"
                                        ValidationExpression="^[a-zA-Z0-9\s\-_\(\)\[\[\*]+$"
                                        ErrorMessage="Descr can only be letters, numbers, underscore, hpyhen, brackets or spaces."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDescr" runat="server" Text='<%# Bind("descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="booking_unavailability_reason_type_id" ItemStyle-Width="250"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlType" runat="server">
                                        <asp:ListItem Text="Provider" Value="341"/>
                                        <asp:ListItem Text="Clinic/Facility" Value="340"/>
                                    </asp:DropDownList>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlNewType" runat="server">
                                        <asp:ListItem Text="Provider" Value="341"/>
                                        <asp:ListItem Text="Clinic/Facility" Value="340"/>
                                    </asp:DropDownList>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDisplayOrder" runat="server" Text='<%# Eval("booking_unavailability_reason_type_id") != null && (int)Eval("booking_unavailability_reason_type_id") == 341 ? "Provider" : "Clinic/Facility" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-20.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>


</asp:Content>



