<%@ Page Title="Referrer List (Clinics)" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReferrerList_ClinicsV2.aspx.cs" Inherits="ReferrerList_ClinicsV2" %>

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
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Referrer List (Clinics)</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">
                    <center>

                        <table cellspacing="8" id="tr_extendedSearch" runat="server" class="padded-table-2px">
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
            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <center>


                <table>
                    <tr style="vertical-align:top">
                        <td>

                            <asp:GridView ID="GrdReferrer" runat="server" 
                                 AutoGenerateColumns="False" DataKeyNames="organisation_id" 
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
                                 PageSize="15"
                                 ClientIDMode="Predictable"
                                 CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                 <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />


                                <Columns> 

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_id" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("organisation_id") %>'></asp:Label>
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("organisation_id") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Total Refs"  HeaderStyle-HorizontalAlign="Left" SortExpression="count" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblCount" runat="server" Text='<%# Eval("count") + ((int)Eval("count_deleted") == 0 ? "" : " (" + Eval("count_deleted") + " del)")  %>'></asp:Label>
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblCount" runat="server" Text='<%# Eval("count") + (Eval("count_deleted") == DBNull.Value || (int)Eval("count_deleted") == 0 ? "" : " (" + Eval("count_deleted") + " del)") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="name" ItemStyle-CssClass="text_left"> 
                                        <EditItemTemplate> 
                                            <asp:TextBox Width="85%" ID="txtName" runat="server" Text='<%# Bind("name") %>'></asp:TextBox> 
                                            <asp:RequiredFieldValidator ID="txtValidateNameRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtName" 
                                                ErrorMessage="Name is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="txtValidateNameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtName"
                                                ValidationExpression="^[a-zA-Z\-\.\s',\(\)\[\]]+$"
                                                ErrorMessage="Name can only be letters, hyphens, comas, brackets, or fullstops."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                        </EditItemTemplate> 
                                        <FooterTemplate>
                                            <asp:TextBox Width="85%" ID="txtNewName" runat="server" ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="txtValidateNewNameRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtNewName" 
                                                ErrorMessage="Name is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="txtValidateNewNameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtNewName"
                                                ValidationExpression="^[a-zA-Z\-\.\s',\(\)\[\]]+$"
                                                ErrorMessage="Name can only be letters, hyphens, comas, or fullstops."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                        </FooterTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblName" runat="server" Text='<%# Bind("name") %>'></asp:Label> 
                                            <asp:HyperLink ID="lnkName" runat="server" Text='<%# Eval("name") %>' NavigateUrl='<%# "~/ReferrerList_DoctorInfoOfClinicV2.aspx?org=" + Eval("organisation_id")%>'></asp:HyperLink>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="ABN" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="abn"> 
                                        <EditItemTemplate> 
                                            <asp:TextBox Width="75%" ID="txtABN" runat="server" Text='<%# Bind("abn") %>' Columns="12"></asp:TextBox> 
                                            <asp:RegularExpressionValidator ID="txtValidateABNRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtABN"
                                                ValidationExpression="^[a-zA-Z0-9\-]+$"
                                                ErrorMessage="ABN can only be numbers, letters and dashes."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                        </EditItemTemplate> 
                                        <FooterTemplate>
                                            <asp:TextBox Width="75%" ID="txtNewABN" runat="server" Columns="12" ></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="txtValidateNewABNRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtNewABN"
                                                ValidationExpression="^[a-zA-Z0-9\-]+$"
                                                ErrorMessage="ABN can only be numbers, letters and dashes."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                        </FooterTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblABN" runat="server" Text='<%# Bind("abn") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="ACN" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="acn"> 
                                        <EditItemTemplate> 
                                            <asp:TextBox Width="75%" ID="txtACN" runat="server" Text='<%# Bind("acn") %>' Columns="12"></asp:TextBox> 
                                            <asp:RegularExpressionValidator ID="txtValidateACNRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtACN"
                                                ValidationExpression="^[a-zA-Z0-9\-]+$"
                                                ErrorMessage="ACN can only be numbers, letters and dashes."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                        </EditItemTemplate> 
                                        <FooterTemplate>
                                            <asp:TextBox Width="75%" ID="txtNewACN" runat="server" Columns="12" ></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="txtValidateNewACNRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtNewACN"
                                                ValidationExpression="^[a-zA-Z0-9\-]+$"
                                                ErrorMessage="ACN can only be numbers, letters and dashes."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                        </FooterTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblACN" runat="server" Text='<%# Bind("acn") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Date Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="referrer_date_added"> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("organisation_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("organisation_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label> 
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

                                    <asp:TemplateField HeaderText="Contact"  HeaderStyle-HorizontalAlign="Left"> 
                                        <ItemTemplate> 
                                            <asp:HyperLink ID="lnkContactInfo" runat="server" Text="Clinic Contact" ></asp:HyperLink>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                   <asp:TemplateField HeaderText="Suburb"  HeaderStyle-HorizontalAlign="Left" SortExpression="suburb_name" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:Label ID="lblSuburb" runat="server" Text='<%# Eval("suburb_name")  %>'></asp:Label>
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblSuburb" runat="server" Text='<%# Eval("suburb_name")  %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="View Patients"  HeaderStyle-HorizontalAlign="Left" SortExpression="referrer_date_added"> 
                                        <EditItemTemplate> 
                                            <asp:LinkButton ID="lnkViewPatients" runat="server" Text="View Patients" CommandName="ViewPatients" CommandArgument='<%# Bind("organisation_id") %>'></asp:LinkButton>
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:LinkButton ID="lnkViewPatients" runat="server" Text="View Patients" CommandName="ViewPatients" CommandArgument='<%# Bind("organisation_id") %>'></asp:LinkButton>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 



                                    <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit" />
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
                                            <asp:ImageButton ID="btnDelete" runat="server"  CommandName="_Delete" CommandArgument='<%# Bind("organisation_id") %>' ImageUrl="~/images/Delete-icon-24.png" AlternateText="Delete" ToolTip="Delete" />
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

                                    <asp:TemplateField HeaderText="EPC Signed"  HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblEPCSigned" runat="server" Text='<%# Eval("epc_signed_date", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="EPC Expires"  HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblEPCExpires" runat="server" Text='<%# Eval("epc_expiry_date", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="EPC Remaining"  HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
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



