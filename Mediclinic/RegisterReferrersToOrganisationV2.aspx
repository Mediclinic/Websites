<%@ Page Title="Organisation List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="RegisterReferrersToOrganisationV2.aspx.cs" Inherits="RegisterReferrersToOrganisationV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Manage Staff For </asp:Label></div>
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


            <div class="text-center">
                <asp:HyperLink ID="lnkThisOrg" runat="server" NavigateUrl="~/OrganisationListV2.aspx?id=">Edit</asp:HyperLink> 
                <div style="height:8px;"></div>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdRegistration" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="register_referrer_id" 
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

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_referrer_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_referrer_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_referrer_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Referrer" HeaderStyle-HorizontalAlign="Left" SortExpression="surname,firstname,middlename" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlReferrer" runat="server"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblReferrer" runat="server" Text='<%# Eval("surname") + ", " + Eval("firstname") + " " + Eval("middlename") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewReferrer" runat="server"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Provider Number" HeaderStyle-HorizontalAlign="Left" SortExpression="provider_number" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtProviderNumber" runat="server" Text='<%# Bind("provider_number") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateProviderNumberRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtProviderNumber" 
                                        ErrorMessage="ProviderNumber is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtProviderNumber"
                                        ValidationExpression="^[a-zA-Z\-\s]+$"
                                        ErrorMessage="Provider Number can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewProviderNumber" runat="server" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewProviderNumberRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewProviderNumber" 
                                        ErrorMessage="ProviderNumber is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewProviderNumber"
                                        ValidationExpression="^[a-zA-Z\-\s]+$"
                                        ErrorMessage="Provider Number can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblProviderNumber" runat="server" Text='<%# Bind("provider_number") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Report Every Visit" SortExpression="report_every_visit_to_referrer" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsReportEveryVisit" runat="server" Checked='<%# Eval("report_every_visit_to_referrer").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsReportEveryVisit" runat="server" Text='<%# Eval("report_every_visit_to_referrer").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsReportEveryVisit" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Batch Send Treatment Notes" SortExpression="batch_send_all_patients_treatment_notes" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsBatchSendAllPatientsTreatmentNotes" runat="server" Checked='<%# Eval("batch_send_all_patients_treatment_notes").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsBatchSendAllPatientsTreatmentNotes" runat="server" Text='<%# Eval("batch_send_all_patients_treatment_notes").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsBatchSendAllPatientsTreatmentNotes" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Last Batch Send Treatment Notes"  HeaderStyle-HorizontalAlign="Left" SortExpression="date_last_batch_send_all_patients_treatment_notes"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblDateLastBatchSendAllPatientsTreatmentNotes" runat="server" Text='<%# Bind("date_last_batch_send_all_patients_treatment_notes") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblNewDateLastBatchSendAllPatientsTreatmentNotes" runat="server" Text='<%# Bind("date_last_batch_send_all_patients_treatment_notes") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Date Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_referrer_date_added"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_referrer_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_referrer_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Edit" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd"></asp:LinkButton> 
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



