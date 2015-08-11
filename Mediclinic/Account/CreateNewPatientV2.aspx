<%@ Page Title="Create New Patient" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="CreateNewPatientV2.aspx.cs" Inherits="CreateNewPatientV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1); });
            //txtbox.value = txtbox.value.charAt(0).toUpperCase() + txtbox.value.slice(1);
        }

        function title_changed_reset_gender() {
            var selValue = ddlTitle.options[ddlTitle.selectedIndex].value
            if (selValue == 6 || selValue == 265 || selValue == 266)
                setSelectedValue(document.getElementById("ddlGender"), "M");
            if (selValue == 7 || selValue == 26)
                setSelectedValue(document.getElementById("ddlGender"), "F");
        }
        function setSelectedValue(selectObj, valueToSet) {
            for (var i = 0; i < selectObj.options.length; i++) {
                if (selectObj.options[i].value == valueToSet) {
                    selectObj.options[i].selected = true;
                    return;
                }
            }
        }

        function create_username() {

            if (document.getElementById('txtLogin').value.length > 0 ||
                document.getElementById('txtPwd').value.length > 0)
                return; // dont update if already set

            var firstname = document.getElementById('txtFirstname').value.trim();
            var surname   = document.getElementById('txtSurname').value.trim();
            document.getElementById('txtLogin').value = firstname.toLowerCase() + surname.toLowerCase();
            document.getElementById('txtPwd').value = firstname.toLowerCase() + surname.toLowerCase();
        }

     </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label id="lblHeading" runat="server">New Patient - Create Web Login</asp:Label></div>
        <div class="main_content" id="main_content" runat="server" style="background: url(../imagesV2/login_bg.png) center top no-repeat #EDEDED;">

            <br />

            <center>

                <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="CreatePatientButton" CssClass="login_form">

                <center>
                    <table>
                        <tr>
                            <td style="text-align:left;">
                                <asp:ValidationSummary ID="ValidationSummary" runat="server" style="color:Red;" ValidationGroup="ValidationSummary"/>
                                <asp:Label ID="lblErrorMessage" runat="server" style="color:Red;"/>
                            </td>
                        </tr>
                    </table>
                </center>

                <table style="border-collapse:collapse;" class="block_center">
                    <tr>
                        <td>
                            <center>
                                <h4><font color="#5A5A5A"><asp:Label id="lblSiteName" runat="server" CssClass="nowrap"></asp:Label></font></h4>
                            </center>

                            <div style="height:1px;"></div>

                            <center>
                                <h5><font color="#3441F8"><asp:Label id="lblExistingUserMessage" runat="server" CssClass="nowrap">If you already have a username/pwd, please use the login link at the bottum</asp:Label></font></h5>
                            </center>

                            <div style="height:26px;"></div>
                            <table class="block_center" id="main_table" runat="server">

                                <tr>
                                    <td>
                                        <asp:Label ID="lblClinic" runat="server">Clinic</asp:Label>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlClinic" runat="server"></asp:DropDownList>
                                    </td>
                                    <td></td>
                                </tr>


                                <tr>
                                    <td colspan="4" style="height:12px;"></td>
                                </tr>


                                <tr>
                                    <td class="nowrap">Title</td>
                                    <td style="min-width:12px"></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlTitle" runat="server" DataTextField="descr" DataValueField="title_id" onchange='title_changed_reset_gender();' ></asp:DropDownList></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">First Name</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtFirstname" runat="server" onkeyup="capitalize_first(this);" /></td>
                                    <td class="nowrap"><asp:RequiredFieldValidator ID="txtValidateFirstnameRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtFirstname" 
                                                ErrorMessage="Firstname is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="txtValidateFirstnameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtFirstname"
                                                ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                                ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Surname</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtSurname" runat="server" onkeyup="capitalize_first(this);"  onblur="create_username();"/></td>
                                    <td><asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtSurname" 
                                                ErrorMessage="Surname is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="txtValidateSurnameNameRegex" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtSurname"
                                                ValidationExpression="^[a-zA-Z\-\.\s'\(\)]+$"
                                                ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Gender</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlGender" runat="server"> 
                                            <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                            <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                                        </asp:DropDownList>
                                    <td></td>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">D.O.B.</td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlDOB_Day" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDOB_Month" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDOB_Year" runat="server"></asp:DropDownList>
                                    </td>
                                    <td><asp:CustomValidator ID="ddlDOBValidateAllSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDOB_Day"
                                            OnServerValidate="DOBSetCheck"
                                            ErrorMessage="DOB must have each of day/month/year selected"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                </tr>


                                <tr>
                                    <td colspan="4" style="height:12px;"></td>
                                </tr>


                                <tr>
                                    <td class="nowrap">Email</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtEmailAddr" runat="server" /></td>
                                    <td class="nowrap"><asp:RequiredFieldValidator ID="txtValidateEmailRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtEmailAddr" 
                                                ErrorMessage="Email is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" CssClass="failureNotification" 
                                                ControlToValidate="txtEmailAddr"
                                                ValidationExpression="^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}$"
                                                ErrorMessage="Email must be in valid email format."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Phone Number</td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:TextBox ID="txtPhoneNumber" runat="server" />
                                        <asp:DropDownList ID="ddlPhoneNumberType" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id"/>
                                    </td>
                                    <td class="nowrap"><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtPhoneNumber" 
                                                ErrorMessage="Phone Number is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="4" style="height:12px;"></td>
                                </tr>

                                <tr>
                                    <td class="nowrap">Web Login Username</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtLogin" runat="server"></asp:TextBox></td>
                                    <td><asp:RequiredFieldValidator ID="txtValidateLoginRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtLogin" 
                                                ErrorMessage="Login is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateLoginRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtLogin"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Login can only be letters, numbers, and underscore."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Web Login Password</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox ID="txtPwd" runat="server"></asp:TextBox></td>
                                    <td><asp:RequiredFieldValidator ID="txtValidatePwdRequired" runat="server" CssClass="failureNotification"  
                                                ControlToValidate="txtPwd" 
                                                ErrorMessage="Password is required."
                                                Display="Dynamic"
                                                ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtPwd"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Password can only be letters, numbers, and underscore."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtPwd"
                                            ValidationExpression="^[a-zA-Z0-9\s]{6,}$"
                                            ErrorMessage="Password must be at least 6 characters."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>

                                <tr style="height:30px;">
                                    <td colspan="4"></td>
                                </tr>
                                <tr>
                                    <td colspan="4" align="center">
                                        <asp:Button ID="CreatePatientButton" runat="server" CommandName="Retrieve" Text="Create" onclick="CreatePatientButton_Click" ValidationGroup="ValidationSummary" CssClass="btn btn-primary" />
                                    </td>
                                </tr>
                                <tr id="afterButtonSpace" runat="server" style="height:25px;">
                                    <td colspan="4"></td>
                                </tr>
                                <tr>
                                    <td colspan="4" align="center">
                                        <asp:LinkButton ID="lnkLogin" runat="server" OnClick="lnkLogin_Click">To login page</asp:LinkButton>
                                    </td>
                                </tr>

                            </table>

                        </td>
                    </tr>
                </table>

            </asp:Panel>
            </center> 

            <br />

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>



