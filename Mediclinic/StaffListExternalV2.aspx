<%@ Page Title="External Staff" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StaffListExternalV2.aspx.cs" Inherits="StaffListExternalV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
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
        <div class="page_title"><span id="lblHeading">External Staff</span></div>
        <div class="main_content">
            <div class="user_login_form">

                <div class="border_top_bottom">
                    <center>
                        Search By Surname: <asp:TextBox ID="txtSearchSurname" runat="server"/>
                        <asp:CheckBox ID="chkSurnameSearchOnlyStartWith" runat="server" /> &nbsp;<label for="chkSurnameSearchOnlyStartWith" style="font-weight:normal;">starts with</label>
                        <span style="padding-left:20px;">
                            <asp:Button ID="btnSearchSurname" runat="server" Text="Search" onclick="btnSearchSurname_Click" />
                            <asp:Button ID="btnClearSurnameSearch" runat="server" Text="Clear" onclick="btnClearSurnameSearch_Click" />
                        </span>
                        <span style="padding-left:20px;" class="hiddencol"><asp:CheckBox ID="chkUsePaging" runat="server" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" Checked="False"/> &nbsp;<label for="chkUsePaging" style="font-weight:normal;">use paging</label> </span>
                        <span style="padding-left:20px;"><asp:CheckBox ID="chkShowFired" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowFired_CheckedChanged" Checked="False" /> &nbsp;<label for="chkShowFired" style="font-weight:normal;">show inactive</label> </span>
                    </center>
                </div>

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit
            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;width:auto;padding-right: 17px;">

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
                                    <asp:HyperLink ID="lnkFirstname" runat="server" Text='<%# Eval("firstname") %>' NavigateUrl='<%#  String.Format("~/StaffDetailExternalV2.aspx?type=view&id={0}",Eval("staff_id"))%>' ToolTip="Full Edit"  />
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

                            <asp:TemplateField HeaderText="Surname" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top"> 
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

                            <asp:TemplateField HeaderText="Username" HeaderStyle-HorizontalAlign="Left" SortExpression="login"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtLogin" runat="server" Text='<%# Bind("login") %>'></asp:TextBox> 
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

                            <asp:TemplateField HeaderText="Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="staff_date_added"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("added_by_firstname") + (Eval("added_by_firstname") == DBNull.Value ? "" : "<br>") + Eval("staff_date_added", "{0:dd-MM-yy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("added_by_firstname") + (Eval("added_by_firstname") == DBNull.Value ? "" : "<br>") + Eval("staff_date_added", "{0:dd-MM-yy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

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



