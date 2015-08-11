<%@ Page Title="Staff List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StaffListV2.aspx.cs" Inherits="StaffListV2" %>
<%@ Register TagPrefix="UC" TagName="DuplicatePersonModalElementControl" Src="~/Controls/DuplicatePersonModalElementControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/duplicate_person_modal_boxV2.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/provider_nbr_check.js"></script>
    <script type="text/javascript" src="Scripts/check_duplicate_personsV2.js"></script>
    <script type="text/javascript">

        function create_username() {

            if (document.getElementById('MainContent_GrdStaff_txtNewLogin').value.length > 0 ||
                document.getElementById('MainContent_GrdStaff_txtNewPwd').value.length > 0)
                return; // dont update if already set

            var firstname = document.getElementById('MainContent_GrdStaff_txtNewFirstname').value.trim();
            var surname = document.getElementById('MainContent_GrdStaff_txtNewSurname').value.trim();
            document.getElementById('MainContent_GrdStaff_txtNewLogin').value = firstname.toLowerCase() + surname.toLowerCase();
            document.getElementById('MainContent_GrdStaff_txtNewPwd').value = firstname.toLowerCase() + surname.toLowerCase();
        }
        function duplicate_person_check() {
            var firstname = document.getElementById('MainContent_GrdStaff_txtNewFirstname').value.trim();
            var surname = document.getElementById('MainContent_GrdStaff_txtNewSurname').value.trim();

            var result = ajax_duplicate_persons("staff", firstname, surname);

            if (result.length == 0) {
                alert("Error retreiving records for duplicate person check.");
            }
            else if (result == "NONE") {
                return;
            }
            else {
                var result_list = create_result_array(result);
                create_table(result_list, "ctable", "StaffDetailV2.aspx?type=view&id=");
                reveal_modal('modalPopupDupicatePerson');
            }
        }
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }

        function dob_changed() {
            if (document.getElementById('txtNewDOB').value.length == 2 ||
                document.getElementById('txtNewDOB').value.length == 5)
                document.getElementById('txtNewDOB').value += "-";
        }

        function title_changed_reset_gender(ddlTitle, ddlGender) {
            ddlTitle = document.getElementById(ddlTitle);
            var selValue = ddlTitle.options[ddlTitle.selectedIndex].value;
            if (selValue == 6 || selValue == 265 || selValue == 266)
                setSelectedValue(document.getElementById(ddlGender), "M");
            if (selValue == 7 || selValue == 26)
                setSelectedValue(document.getElementById(ddlGender), "F");
        }
        function setSelectedValue(selectObj, valueToSet) {
            for (var i = 0; i < selectObj.options.length; i++) {
                if (selectObj.options[i].value == valueToSet) {
                    selectObj.options[i].selected = true;
                    return;
                }
            }
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Staff List</span></div>
        <div class="main_content">
            <div class="user_login_form">
                Total Providers: <asp:Label ID="lblNbrProviders" runat="server" /> <span style="padding-left:50px; ">Max Allowed:</span> <asp:TextBox ID="txtMaxNbrProviders" runat="server" Columns="4" />
                <asp:Button ID="btnMaxNbrProvidersSetEditMode" runat="server" Text="Edit" OnClick="btnMaxNbrProvidersSetEditMode_Click" />
                <asp:Button ID="btnMaxNbrProvidersUpdate" runat="server" Text="Update" OnClick="btnMaxNbrProvidersUpdate_Click" CausesValidation="True" ValidationGroup="EditStaffValidationSummary" />
                <asp:Button ID="btnMaxNbrProvidersCancelEditMode" runat="server" Text="Cancel Edit" OnClick="btnMaxNbrProvidersCancelEditMode_Click" />

                <div class="border_top_bottom">
                    <center>
                        Search By Surname: <asp:TextBox ID="txtSearchSurname" runat="server"/>
                        <asp:CheckBox ID="chkSurnameSearchOnlyStartWith" runat="server" /> &nbsp;<label for="chkSurnameSearchOnlyStartWith" style="font-weight:normal;">starts with</label>
                        <span style="padding-left:20px;">
                            <asp:Button ID="btnSearchSurname" runat="server" Text="Search" onclick="btnSearchSurname_Click" />
                            <asp:Button ID="btnClearSurnameSearch" runat="server" Text="Clear" onclick="btnClearSurnameSearch_Click" />
                        </span>
                        <span style="padding-left:20px;" class="hiddencol"><asp:CheckBox ID="chkUsePaging" runat="server" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" Checked="False"/>&nbsp;<label for="chkUsePaging" style="font-weight:normal;">use paging</label> </span>
                        <span style="padding-left:20px;"><asp:CheckBox ID="chkShowFired" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowFired_CheckedChanged" Checked="False" />&nbsp;<label for="chkShowFired" style="font-weight:normal;">show inactive</label> </span>
                    </center>
                </div>

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit
            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <asp:RequiredFieldValidator ID="txtValidateMaxNbrProvidersRequired" runat="server" CssClass="failureNotification"  
                Display="None"
                ControlToValidate="txtMaxNbrProviders" 
                ErrorMessage="Max Nbr Providers is required."
                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="txtValidateMaxNbrProvidersRegex" runat="server" CssClass="failureNotification" 
                Display="None"
                ControlToValidate="txtMaxNbrProviders"
                ValidationExpression="^\d+$"
                ErrorMessage="Max Nbr Providers can only be numbers"
                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>


            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <!-- grid css style:  http://stackoverflow.com/questions/3921497/setting-gridview-header-color -->
                    <asp:GridView ID="GrdStaff" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="staff_id" 
                        OnRowCancelingEdit="GrdStaff_RowCancelingEdit" 
                        OnRowDataBound="GrdStaff_RowDataBound" 
                        OnRowEditing="GrdStaff_RowEditing" 
                        OnRowUpdating="GrdStaff_RowUpdating" ShowFooter="false" 
                        OnRowCommand="GrdStaff_RowCommand" 
                        OnRowCreated="GrdStaff_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GridView_Sorting"
                        RowStyle-VerticalAlign="top"
                        AllowPaging="True"
                        OnPageIndexChanging="GrdStaff_PageIndexChanging"
                        PageSize="9"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="staff_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("staff_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("staff_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummary"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummary"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

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

                            <asp:TemplateField HeaderText="Firstname" HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtFirstname" runat="server" Text='<%# Bind("firstname") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateFirstnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtFirstname" 
                                        ErrorMessage="Firstname is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateFirstnameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtFirstname"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewFirstname" runat="server" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewFirstnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewFirstname" 
                                        ErrorMessage="Firstname is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewFirstnameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewFirstname"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkFirstname" runat="server" Text='<%# Eval("firstname") %>' NavigateUrl='<%#  String.Format("~/StaffDetailV2.aspx?type=view&id={0}",Eval("staff_id"))%>' ToolTip="Full Edit"  />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="M.name" HeaderStyle-HorizontalAlign="Left" SortExpression="middlename" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtMiddlename"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewMiddlename" runat="server" ></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateNewMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewMiddlename"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Surname" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtSurname" runat="server" Text='<%# Bind("surname") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtSurname" 
                                        ErrorMessage="Surname is required."
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateSurnameNameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtSurname"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                 </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewSurname" runat="server" onblur="create_username();duplicate_person_check(this);" ></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewSurname" 
                                        ErrorMessage="Surname is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewSurnameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewSurname"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                 </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSurname" runat="server" Text='<%# Bind("surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Gender" HeaderStyle-HorizontalAlign="Left" SortExpression="gender" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
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

                            <asp:TemplateField HeaderText="D.O.B." HeaderStyle-HorizontalAlign="Left" SortExpression="dob" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <span style="white-space: nowrap">
                                        <asp:DropDownList ID="ddlDOB_Day" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlDOB_Month" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlDOB_Year" runat="server"></asp:DropDownList>
                                        <asp:CustomValidator ID="ddlDOBValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDOB_Day"
                                            OnServerValidate="DOBAllOrNoneCheck"
                                            ErrorMessage="DOB must have each of day/month/year selected, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </span> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewDOB" runat="server" onkeyup="javascript: dob_changed();"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateNewDOBRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDOB" 
                                        ErrorMessage="DOB is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDOBRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDOB"
                                        ValidationExpression="^\d{2}\-\d{2}\-\d{4}$"
                                        ErrorMessage="DOB format must be dd-mm-yyyy"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="txtValidateNewDOB" runat="server"  CssClass="failureNotification"  
                                        ControlToValidate="txtNewDOB"
                                        OnServerValidate="ValidDateCheck"
                                        ErrorMessage="Invalid DOB"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDOB" runat="server" Text='<%# Bind("dob", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Username" HeaderStyle-HorizontalAlign="Left" SortExpression="login"> 
                                <EditItemTemplate> 
                                    <table class="nowrap">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtLogin" runat="server" Text='<%# Bind("login") %>' style="width:100px;"></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidateLoginRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtLogin" 
                                                    ErrorMessage="Login is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidateLoginRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtLogin"
                                                    ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                                    ErrorMessage="Login can only be letters, numbers, dash, underscore."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewLogin" runat="server"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateLoginRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewLogin" 
                                        ErrorMessage="Login is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewLoginRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewLogin"
                                        ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                        ErrorMessage="Login can only be letters, numbers, dash, underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLogin" runat="server" Text='<%# Bind("login") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Password" HeaderStyle-HorizontalAlign="Left" SortExpression="pwd"> 
                                <EditItemTemplate> 
                                    <table class="nowrap">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtPwd" runat="server" Text='<%# Eval("pwd") %>' TextMode="Password" style="width:100px;" ></asp:TextBox> 
                                                <asp:RequiredFieldValidator ID="txtValidatePwdRequired" runat="server" CssClass="failureNotification"  
                                                    ControlToValidate="txtPwd" 
                                                    ErrorMessage="Pwd is required."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="txtValidatePwdRegex" runat="server" CssClass="failureNotification" 
                                                    ControlToValidate="txtPwd"
                                                    ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                                    ErrorMessage="Password can only be letters, numbers, dash, underscore."
                                                    Display="Dynamic"
                                                    ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewPwd" runat="server"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidatePwdRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewPwd" 
                                        ErrorMessage="Pwd is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewPwdRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewPwd"
                                        ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                        ErrorMessage="Password can only be letters, numbers, dash, underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPwd" runat="server" Text="●●●●●●●●●"></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Position" HeaderStyle-HorizontalAlign="Left" SortExpression="staff_position_descr" FooterStyle-VerticalAlign="Top" Visible="False"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlStaffPosition" runat="server" DataTextField="staff_position_descr" DataValueField="staff_position_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblStaffPosition" runat="server" Text='<%# Eval("staff_position_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewStaffPosition" runat="server" DataTextField="descr" DataValueField="staff_position_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Role" HeaderStyle-HorizontalAlign="Left" SortExpression="field_descr" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlField" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblField" runat="server" Text='<%# Eval("field_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewField" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Contractor" SortExpression="is_contractor" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkContractor" runat="server" Checked='<%# Eval("is_contractor").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblContractor" runat="server" Text='<%# Eval("is_contractor").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewContractor" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="TFN" HeaderStyle-HorizontalAlign="Left" SortExpression="tfn" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtTFN" runat="server" Text='<%# Bind("tfn") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateTFNRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtTFN"
                                        ValidationExpression="^[0-9\-]+$"
                                        ErrorMessage="TFN can only be numbers and hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewTFN" runat="server" ></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateNewTFNRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewTFN"
                                        ValidationExpression="^[0-9\-]+$"
                                        ErrorMessage="TFN can only be numbers and hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTFN" runat="server" Text='<%# Bind("tfn") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Cost Centre" HeaderStyle-HorizontalAlign="Left" SortExpression="costcentre_descr" FooterStyle-VerticalAlign="Top" Visible="false"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlCostCentre" runat="server" DataTextField="costcentre_descr" DataValueField="costcentre_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCostCentre" runat="server" Text='<%# Eval("costcentre_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewCostCentre" runat="server" DataTextField="descr" DataValueField="costcentre_id"> </asp:DropDownList> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="SMS BKs" SortExpression="enable_daily_reminder_sms" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkSMSBKs" runat="server" Checked='<%#Eval("enable_daily_reminder_sms").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSMSBKs" runat="server" Text='<%# Eval("enable_daily_reminder_sms").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewSMSBKs" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Email BKs" SortExpression="enable_daily_reminder_email" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkEmailBKs" runat="server" Checked='<%#Eval("enable_daily_reminder_email").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblEmailBKs" runat="server" Text='<%# Eval("enable_daily_reminder_email").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewEmailBKs" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Aged Care Prov Nbr" HeaderStyle-HorizontalAlign="Left" SortExpression="provider_number" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtProviderNumber" runat="server" Text='<%# Bind("provider_number") %>' onblur="provider_check(this);"></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtProviderNumber"
                                        ValidationExpression="^[a-zA-Z0-9]+$"
                                        ErrorMessage="ProviderNumber can only be letters, numbers, and underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewProviderNumber" runat="server" onblur="provider_check(this);" ></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateNewProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewProviderNumber"
                                        ValidationExpression="^[a-zA-Z0-9]+$"
                                        ErrorMessage="ProviderNumber can only be letters, numbers, and underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblProviderNumber" runat="server" Text='<%# Bind("provider_number") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Comm" SortExpression="is_commission" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsCommission" runat="server" Checked='<%#Eval("is_commission").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsCommission" runat="server" Text='<%# Eval("is_commission").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsCommission" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Comm %" HeaderStyle-HorizontalAlign="Left" SortExpression="commission_percent" FooterStyle-VerticalAlign="Top" HeaderStyle-Wrap="False"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtCommissionPercent" runat="server" Text='<%# Bind("commission_percent") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateCommissionPercentRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtCommissionPercent" 
                                        ErrorMessage="Commission percent is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtValidateCommissionPercentRange" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtCommissionPercent"
                                        ErrorMessage="Commission percent must be a number and between 0 and 100" Type="Double" MinimumValue="0.00" MaximumValue="100.00" 
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewCommissionPercent" runat="server" Text='0.00'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateNewCommissionPercentRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewCommissionPercent" 
                                        ErrorMessage="Commission percent is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtValidateNewCommissionPercentRange" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewCommissionPercent"
                                        ErrorMessage="Commission percent must be a number and must be between 0 and 100" Type="Double" MinimumValue="0.00" MaximumValue="100.00" 
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RangeValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSalaryAdvance" runat="server" Text='<%# Bind("commission_percent") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Provider" SortExpression="is_provider" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsProvider" runat="server" Checked='<%#Eval("is_provider").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsProvider" runat="server" Text='<%# Eval("is_provider").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsProvider" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Principal" SortExpression="is_principal" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsPrincipal" runat="server" Checked='<%#Eval("is_principal").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsPrincipal" runat="server" Text='<%# Eval("is_principal").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsPrincipal" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Admin" SortExpression="is_admin" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsAdmin" runat="server" Checked='<%#Eval("is_admin").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsAdmin" runat="server" Text='<%# Eval("is_admin").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsAdmin" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Master Admin" SortExpression="is_master_admin" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsMasterAdmin" runat="server" Checked='<%#Eval("is_master_admin").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsMasterAdmin" runat="server" Text='<%# Eval("is_master_admin").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsMasterAdmin" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Stake Holder" SortExpression="is_stakeholder" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsStakeholder" runat="server" Checked='<%#Eval("is_stakeholder").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsStakeholder" runat="server" Text='<%# Eval("is_stakeholder").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsStakeholder" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="staff_date_added"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("added_by_firstname") + (Eval("added_by_firstname") == DBNull.Value ? "" : "<br>") + Eval("staff_date_added", "{0:dd-MM-yy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("added_by_firstname") + (Eval("added_by_firstname") == DBNull.Value ? "" : "<br>") + Eval("staff_date_added", "{0:dd-MM-yy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <%-- 
                            <asp:TemplateField HeaderText="Working Days"  HeaderStyle-HorizontalAlign="Left"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblWorkingDays" runat="server" Text='<%# (Eval("excl_sun").ToString()=="False"?"Sun<br/>":"") + (Eval("excl_mon").ToString()=="False"?"Mon<br/>":"") + (Eval("excl_tue").ToString()=="False"?"Tue<br/>":"") + (Eval("excl_wed").ToString()=="False"?"Wed<br/>":"") + (Eval("excl_thu").ToString()=="False"?"Thurs<br/>":"") + (Eval("excl_fri").ToString()=="False"?"Fri<br/>":"") + (Eval("excl_sat").ToString()=="False"?"Sat<br/>":"") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblWorkingDays" runat="server" Text='<%# (Eval("excl_sun").ToString()=="False"?"Sun<br/>":"") + (Eval("excl_mon").ToString()=="False"?"Mon<br/>":"") + (Eval("excl_tue").ToString()=="False"?"Tue<br/>":"") + (Eval("excl_wed").ToString()=="False"?"Wed<br/>":"") + (Eval("excl_thu").ToString()=="False"?"Thurs<br/>":"") + (Eval("excl_fri").ToString()=="False"?"Fri<br/>":"") + (Eval("excl_sat").ToString()=="False"?"Sat<br/>":"") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 
                            --%>

                            <%--  // not used because points to old booking screen ... at the minimum if use this, prob need to disallow editing because now a booking is not linked to room (?)
                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                <EditItemTemplate> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:ImageButton ID="lnkBookings" runat="server" PostBackUrl='<%#  String.Format("~/Bookings.aspx?type=provider&provider={0}",Eval("staff_id"))%>' ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Bookings" ToolTip="Bookings" />
                                </ItemTemplate> 
                            </asp:TemplateField> 
                            --%>

                            <asp:TemplateField HeaderText="Status" SortExpression="is_fired" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlStatus" runat="server">
                                        <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                                        <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsFired" runat="server" Text='<%# Eval("is_fired").ToString()=="True" ? "Inactive" : "Active" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <%--
                            <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True" /> 
                            --%>

                        </Columns> 
                    </asp:GridView>

                </div>
            </center>
            

        </div>
    </div>

</asp:Content>



