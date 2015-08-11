<%@ Page Title="Referrer List (Doctors)" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReferrerList_DoctorsV2.aspx.cs" Inherits="ReferrerList_DoctorsV2" %>
<%@ Register TagPrefix="UC" TagName="DuplicatePersonModalElementControl" Src="~/Controls/DuplicatePersonModalElementControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/duplicate_person_modal_box.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/check_duplicate_persons.js" type="text/javascript"></script>
    <script type="text/javascript">

        function open_new_window(URL) {
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,height=" + screen.height + ',width=' + screen.width);
            NewWindow.location = URL;
        }

        function duplicate_person_check(obj) {

            var firstname = document.getElementById('MainContent_GrdReferrer_txtNewFirstname').value.trim();
            var surname = document.getElementById('MainContent_GrdReferrer_txtNewSurname').value.trim();

            var result = ajax_duplicate_persons("referrer", firstname, surname);

            if (result.length == 0) {
                alert("Error retreiving records for duplicate person check.");
            }
            else if (result == "NONE") {
                return;
            }
            else {
                var result_list = create_result_array(result);
                create_table(result_list, "ctable", "ReferrerList_ClinicInfoOfDoctorV2.aspx?referrer=");

                reveal_modal('modalPopupDupicatePerson');
            }
        }
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }

        function set_existing_person(referrer_id) {
            document.getElementById("jsSetId").value = String(referrer_id);
            document.getElementById('btnSetExistingReferrer').click();
        }

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.charAt(0).toUpperCase() + txtbox.value.slice(1);
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
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Referrer List (Doctors)</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">
                    <center>

                        <table id="tr_extendedSearch" runat="server" class="padded-table-2px">
                            <tr  id="tr_basicSearch" runat="server">
                                <td><asp:Label ID="lblSearch" runat="server">Search By Name: </asp:Label></td>
                                <td><asp:TextBox ID="txtSearchName" runat="server"></asp:TextBox></td>
                                <td class="nowrap">&nbsp;<asp:CheckBox ID="chkSearchOnlyStartWith" runat="server" Text="" />&nbsp;<label for="chkSearchOnlyStartWith" style="font-weight:normal;">starts with</label></td>
                                <td class="nowrap">&nbsp;&nbsp;<asp:Button ID="btnSearchName" runat="server" Text="Search" onclick="btnSearchName_Click" /></td>
                                <td><asp:Button ID="btnClearNameSearch" runat="server" Text="Clear" onclick="btnClearNameSearch_Click" /></td>
                                <td style="width:60px"></td>
                                <td class="nowrap">&nbsp;<asp:CheckBox ID="chkUsePaging" runat="server" Text="" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" Checked="True" />&nbsp;<label for="chkUsePaging" style="font-weight:normal;">use paging</label></td>
                                <td rowspan="2" style="width:25px"></td>
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
                        <tr style="vertical-align:top;">
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
                                     PageSize="15"
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

                                        <asp:TemplateField HeaderText="Total Clinics"  HeaderStyle-HorizontalAlign="Left" SortExpression="count" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:Label ID="lblCount" runat="server" Text='<%# Eval("count") + (Eval("count_deleted") == DBNull.Value || (int)Eval("count_deleted") == 0 ? "" : " (" + Eval("count_deleted") + " del)")  %>'></asp:Label>
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblCount" runat="server" Text='<%# Eval("count") + (Eval("count_deleted") == DBNull.Value || (int)Eval("count_deleted") == 0 ? "" : " (" + Eval("count_deleted") + " del)")  %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <%-- Referrer Person --%>

                                        <asp:TemplateField HeaderText="Title" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:DropDownList ID="ddlTitle" runat="server" DataTextField="descr" DataValueField="title_id"> </asp:DropDownList> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("title_id") == DBNull.Value || (int)Eval("title_id") == 0 ? "" :  Eval("descr") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:DropDownList ID="ddlNewTitle" runat="server" DataTextField="descr" DataValueField="title_id"> </asp:DropDownList>
                                            </FooterTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Firstname" HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left" ItemStyle-Wrap="false"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="90%" ID="txtFirstname" runat="server" Text='<%# Bind("firstname") %>'></asp:TextBox> 
                                                <asp:RegularExpressionValidator ID="txtValidateFirstnameRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtFirstname"
                                                    ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                    ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                            </EditItemTemplate> 
                                            <FooterTemplate>
                                                <asp:TextBox Width="90%" ID="txtNewFirstname" runat="server" onkeyup="capitalize_first(this);" ></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="txtValidateNewFirstnameRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtNewFirstname"
                                                    ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                    ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                            </FooterTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblFirstname" runat="server" Text='<%# Bind("firstname") %>'></asp:Label> 
                                                <asp:HyperLink ID="lnkFirstname" runat="server" Text='<%# Eval("firstname") %>' NavigateUrl='<%# "~/ReferrerList_ClinicInfoOfDoctorV2.aspx?referrer=" + Eval("referrer_id")%>'></asp:HyperLink>
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="M.name" HeaderStyle-HorizontalAlign="Left" SortExpression="middlename" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left" ItemStyle-Wrap="false"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="90%" ID="txtMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:TextBox> 
                                                <asp:RegularExpressionValidator ID="txtValidateMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtMiddlename"
                                                    ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                    ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                            </EditItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:TextBox Width="90%" ID="txtNewMiddlename" runat="server" onkeyup="capitalize_first(this);" ></asp:TextBox> 
                                                <asp:RegularExpressionValidator ID="txtValidateNewMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtNewMiddlename"
                                                    ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                    ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                            </FooterTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Surname" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left" ItemStyle-Wrap="false"> 
                                            <EditItemTemplate> 
                                                <asp:TextBox Width="90%" ID="txtSurname" runat="server" Text='<%# Bind("surname") %>'></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtSurname" 
                                                    ErrorMessage="Surname is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidateSurnameNameRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtSurname"
                                                    ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                    ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                             </EditItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:TextBox Width="90%" ID="txtNewSurname" runat="server" onkeyup="capitalize_first(this);" onblur="duplicate_person_check(this);" ></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtNewSurname" 
                                                    ErrorMessage="Surname is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidateNewSurnameRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtNewSurname"
                                                    ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                    ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                            </FooterTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblSurname" runat="server" Text='<%# Bind("surname") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Gender" HeaderStyle-HorizontalAlign="Left" SortExpression="gender" FooterStyle-VerticalAlign="Top"> 
                                            <EditItemTemplate> 
                                                <asp:DropDownList ID="ddlGender" runat="server" SelectedValue='<%# Eval("gender") %>'> 
                                                    <asp:ListItem Text="M" Value="M"></asp:ListItem>
                                                    <asp:ListItem Text="F" Value="F"></asp:ListItem>
                                                    <asp:ListItem Text="-" Value=""></asp:ListItem>
                                                </asp:DropDownList> 
                                            </EditItemTemplate> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblGender" runat="server" Text='<%# ( Eval("gender").ToString() == "M")?"Male" : (( Eval("gender").ToString() == "F")?"Female" : "-") %>'></asp:Label> 
                                            </ItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:DropDownList ID="ddlNewGender" runat="server" >
                                                    <asp:ListItem Text="M" Value="M" Selected="True"></asp:ListItem> 
                                                    <asp:ListItem Text="F" Value="F"></asp:ListItem>
                                                </asp:DropDownList>
                                            </FooterTemplate> 
                                        </asp:TemplateField> 


                                        <%-- Referrer --%>

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
                                                <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit" />
                                                <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
                                            </EditItemTemplate> 
                                            <FooterTemplate> 
                                                <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd" />
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



