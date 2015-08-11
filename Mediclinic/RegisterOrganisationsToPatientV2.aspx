<%@ Page Title="Organisation List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="RegisterOrganisationsToPatientV2.aspx.cs" Inherits="RegisterOrganisationsToPatientV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Manage Registrations For </asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">

                    <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit
                    <span style="padding-left:20px;"><img src="imagesV2/x.png" alt="delete icon" style="margin:0 5px 5px 0;" />Delete</span>

                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <asp:Label ID="lblProvNbrs" runat="server" CssClass="hiddencol"></asp:Label>
            <div class="text-center">
                <asp:HyperLink ID="lnkThisPatient" runat="server" NavigateUrl="~/PatientListV2.aspx?id=">Edit</asp:HyperLink> 
                <div style="height:8px;"></div>
            </div>


            <span id="spn_booking_screen_link" runat="server">
            <br />
            <asp:HyperLink ID="lnkBookingScreen" runat="server"  Text="Make A Booking" Font-Bold="True"></asp:HyperLink> 
            <asp:Label ID="lblSelectOrgBeforeBooking" runat="server" Text="Please Register An Organistion To Make Bookings At" Font-Bold="True" ForeColor="#cc3300"></asp:Label>
            </span>


            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdRegistration" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="register_patient_id" 
                         OnRowCancelingEdit="GrdRegistration_RowCancelingEdit" 
                         OnRowDataBound="GrdRegistration_RowDataBound" 
                         OnRowEditing="GrdRegistration_RowEditing" 
                         OnRowUpdating="GrdRegistration_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdRegistration_RowCommand" 
                         OnRowDeleting="GrdRegistration_RowDeleting" 
                         OnRowCreated="GrdRegistration_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_patient_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_patient_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_patient_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Organisation" HeaderStyle-HorizontalAlign="Left" SortExpression="name" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("name") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Date Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_patient_date_added"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_patient_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_patient_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Booking" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" SortExpression="num_registered_orgs"> 
                                <EditItemTemplate> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkBookings" runat="server" NavigateUrl='<%#  String.Format("~/BookingScreenGetPatientOrgsV2.aspx?patient_id={0}",Eval("patient_id")) %>' ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Bookings" ToolTip="Bookings" />
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="Edit" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditRegistrationValidationSummary"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="AddRegistrationValidationGroup"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True" /> 
                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



