<%@ Page Title="Referrer List (Doctor-Clinic)" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReferrerList_DoctorInfoOfClinicV2.aspx.cs" Inherits="ReferrerList_DoctorInfoOfClinicV2" %>
<%@ Register TagPrefix="UC" TagName="DuplicatePersonModalElementControl" Src="~/Controls/DuplicatePersonModalElementControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function open_new_window(URL) {
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,height=" + screen.height + ',width=' + screen.width);
            NewWindow.location = URL;
        }

    </script>
    <style type="text/css">
        .GridViewCSS td
        {
            padding: 0px 15px 0px 0px;
        }
        .GridViewCSS th
        {
            padding: 0px 15px 0px 0px;
            vertical-align:bottom;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Referrer List</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">
                    <center>

                        <table border="0" cellpadding="0" cellspacing="8"  id="tr_extendedSearch" runat="server" class="padded-table-2px">
                            <tr  id="tr_basicSearch" runat="server">
                                <td><asp:Label ID="lblSearch" runat="server">Search By Name: </asp:Label></td>
                                <td><asp:TextBox ID="txtSearchName" runat="server"></asp:TextBox></td>
                                <td class="nowrap">&nbsp;<asp:CheckBox ID="chkSearchOnlyStartWith" runat="server" Text="" />&nbsp;<label for="chkSearchOnlyStartWith" style="font-weight:normal;">starts with</label></td>
                                <td class="nowrap">&nbsp;&nbsp;<asp:Button ID="btnSearchName" runat="server" Text="Search" onclick="btnSearchName_Click" /></td>
                                <td><asp:Button ID="btnClearNameSearch" runat="server" Text="Clear" onclick="btnClearNameSearch_Click" /></td>
                                <td style="width:60px"></td>
                                <td class="nowrap">&nbsp;<asp:CheckBox ID="chkUsePaging" runat="server" Text="" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" Checked="True" />&nbsp;<label for="chkUsePaging" style="font-weight:normal;">use paging</label></td>
                                <td style="width:25px"></td>
                                <td class="nowrap">&nbsp;<asp:CheckBox ID="chkShowDeleted" runat="server" Text="" AutoPostBack="True" OnCheckedChanged="chkShowDeleted_CheckedChanged" Checked="False" />&nbsp;<label for="chkShowDeleted" style="font-weight:normal;">show deleted</label></td>
                            </tr>
                        </table>

                    </center>
                </div>

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit<span style="padding-left:50px;"><img src="imagesV2/x.png" alt="edit icon" style="margin:0 5px 5px 0;" />Delete</span>

                <br />
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>

            </div>

            <center>

                <table>
                    <tr style="vertical-align:top">
                        <td>

                            <asp:GridView ID="GrdReferrer" runat="server" 
                                 AutoGenerateColumns="False" DataKeyNames="referrer_id" 
                                 OnRowCancelingEdit="GrdReferrer_RowCancelingEdit" 
                                 OnRowDataBound="GrdReferrer_RowDataBound" 
                                 OnRowEditing="GrdReferrer_RowEditing" 
                                 OnRowUpdating="GrdReferrer_RowUpdating" ShowFooter="True" 
                                 OnRowCommand="GrdReferrer_RowCommand" 
                                 OnRowDeleting="GrdReferrer_RowDeleting" 
                                 OnRowCreated="GrdReferrer_RowCreated"
                                 AllowSorting="True" 
                                 OnSorting="GridView_Sorting"
                                 RowStyle-VerticalAlign="top" 
                                 AllowPaging="True"
                                 OnPageIndexChanging="GrdReferrer_PageIndexChanging"
                                 PageSize="16"
                                 ClientIDMode="Predictable"
                                 CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                 <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />


                                <Columns> 

                                    <%-- Referrer --%>

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="referrer_id" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("referrer_id") %>'></asp:Label>
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("referrer_id") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <%-- Referrer Person --%>

                                    <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="name" ItemStyle-CssClass="text_left"> 
                                        <EditItemTemplate> 
                                            <asp:DropDownList ID="ddlRefs" runat="server" />
                                        </EditItemTemplate> 
                                        <FooterTemplate>
                                            <asp:DropDownList ID="ddlNewRefs" runat="server"  />
                                        </FooterTemplate> 
                                        <ItemTemplate> 
                                            <span runat="server" style="color:red;" visible ='<%# Eval("referrer_is_deleted") != DBNull.Value && (bool)Eval("referrer_is_deleted")  %>' >
                                                [REFERRER DELETED]<br />
                                            </span>
                                            <asp:HyperLink ID="lnkName" runat="server" Text='<%#  Eval("firstname") + " " + Eval("surname") %>' NavigateUrl='<%# "~/ReferrerList_DoctorsV2.aspx?referrer=" + Eval("original_referrer_id")%>'></asp:HyperLink>
                                            (<asp:HyperLink ID="lnkClinics" runat="server" Text='Clinics' NavigateUrl='<%# "~/ReferrerList_ClinicInfoOfDoctorV2.aspx?referrer=" + Eval("original_referrer_id")%>'></asp:HyperLink>)
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <%-- Referrer --%>

                                    <asp:TemplateField HeaderText="Provider Number" HeaderStyle-HorizontalAlign="Left" SortExpression="provider_number" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:TextBox Width="90%" ID="txtProviderNumber" runat="server" Text='<%# Eval("provider_number") %>'></asp:TextBox> 
                                            <asp:RegularExpressionValidator ID="txtValidateProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtProviderNumber"
                                                ValidationExpression="^[a-zA-Z\-\s]+$"
                                                ErrorMessage="Provider Number can only be letters, hyphens, or fullstops."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                        </EditItemTemplate> 
                                        <FooterTemplate>
                                            <asp:TextBox Width="90%" ID="txtNewProviderNumber" runat="server" ></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="txtValidateNewProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtNewProviderNumber"
                                                ValidationExpression="^[a-zA-Z\-\s]+$"
                                                ErrorMessage="Provider Number can only be letters, hyphens, or fullstops."
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

                                    <asp:TemplateField HeaderText="Date Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="referrer_date_added"> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("referrer_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("referrer_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Deleted" SortExpression="is_deleted" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                        </ItemTemplate> 
                                        <FooterTemplate> 
                                        </FooterTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="View Patients"  HeaderStyle-HorizontalAlign="Left" SortExpression="referrer_date_added"> 
                                        <EditItemTemplate> 
                                            <asp:LinkButton ID="lnkViewPatients" runat="server" Text="View Patients" CommandName="ViewPatients" CommandArgument='<%# Bind("referrer_id") %>'></asp:LinkButton>
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:LinkButton ID="lnkViewPatients" runat="server" Text="View Patients" CommandName="ViewPatients" CommandArgument='<%# Bind("referrer_id") %>'></asp:LinkButton>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 


                                    <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditReferrerValidationSummary" />
                                            <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
                                        </EditItemTemplate> 
                                        <FooterTemplate> 
                                            <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="AddReferrerValidationGroup" />
                                        </FooterTemplate> 
                                        <ItemTemplate> 
                                            <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="" ShowHeader="True" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server"  CommandName="_Delete" CommandArgument='<%# Bind("referrer_id") %>' ImageUrl="~/images/Delete-icon-24.png" AlternateText="Delete" ToolTip="Delete" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns> 

                            </asp:GridView>

                        </td>
                        <td style="width:35px;"></td>
                        <td>

                            <asp:Label ID="lblPatientsHeading" runat="server" Visible="false">Heading..</asp:Label>
                            <br />
                            <br />

                            <asp:GridView ID="GrdPatients" runat="server" 
                                 AutoGenerateColumns="False" DataKeyNames="patient_id" 
                                 OnRowDataBound="GrdPatients_RowDataBound" 
                                 OnRowCommand="GrdPatients_RowCommand" 
                                 OnRowCreated="GrdPatients_RowCreated"
                                 ShowFooter="False" 
                                 AllowSorting="False" 
                                 OnSorting="GrdPatients_Sorting"
                                 RowStyle-VerticalAlign="top" 
                                 ClientIDMode="Predictable"
                                 GridLines="None"
                                 Visible="false"
                                 CssClass="GridViewCSS">

                                <Columns> 

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_id" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Eval("patient_id") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Patient"  HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblPatient" runat="server" Text='<%# Eval("firstname") + " " + Eval("surname") %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Ref. Signed"  HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblEPCSigned" runat="server" Text='<%# Eval("epc_signed_date", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Ref. Expires"  HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblEPCExpires" runat="server" Text='<%# Eval("epc_expiry_date", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Ref. Remaining"  HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblEPCRemaining" runat="server" Text='<%# Eval("epc_n_services_left") %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                </Columns> 

                            </asp:GridView>

                        </td>
                    </tr>
                </table>

            </center>
            
        </div>
    </div>

</asp:Content>



