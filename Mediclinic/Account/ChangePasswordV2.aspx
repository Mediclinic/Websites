<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ChangePasswordV2.aspx.cs" Inherits="ChangePasswordV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Change Password</span></div>
        <div class="main_content">

            <br />

            <div class="login_form">
                <table style="border-collapse:collapse;" class="block_center">
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="CurrentPasswordLabel" runat="server" CssClass="nowrap">Old Password:</asp:Label>
                                    </td>
                                    <td class="nowrap">&nbsp;&nbsp;
                                        <asp:TextBox ID="CurrentPassword" runat="server" CssClass="passwordEntry" TextMode="Password" EnableViewState="false" autocomplete="off"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" 
                                                ControlToValidate="CurrentPassword" 
                                                CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Old Password is required." 
                                                Display="Dynamic"
                                                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="NewPasswordLabel" runat="server" CssClass="nowrap">New Password:</asp:Label>
                                    </td>
                                    <td class="nowrap">&nbsp;&nbsp;
                                        <asp:TextBox ID="NewPassword" runat="server" CssClass="passwordEntry" TextMode="Password" EnableViewState="false" autocomplete="off"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword" 
                                            CssClass="failureNotification" ErrorMessage="New Password is required." ToolTip="New Password is required." 
                                            Display="Dynamic"
                                            ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="ConfirmNewPasswordLabel" runat="server" CssClass="nowrap">Confirm New Password:</asp:Label>
                                    </td>
                                    <td class="nowrap">&nbsp;&nbsp;
                                        <asp:TextBox ID="ConfirmNewPassword" runat="server" CssClass="passwordEntry" TextMode="Password" autocomplete="off"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword" 
                                                CssClass="failureNotification" Display="Dynamic" ErrorMessage="Confirm New Password is required."
                                                ToolTip="Confirm New Password is required." ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword" 
                                                CssClass="failureNotification" Display="Dynamic" ErrorMessage="The Confirm New Password must match the New Password entry."
                                                ValidationGroup="ChangeUserPasswordValidationGroup">*</asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr style="height:20px">
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:Button ID="ChangePasswordButton" runat="server" CommandName="ChangePassword" Text="Change Password" onclick="ChangePasswordButton_Click" ValidationGroup="ChangeUserPasswordValidationGroup" CssClass="btn btn-primary" />
                                    </td>
                                </tr>
                                <tr style="height:25px">
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="color:Red;">
                                        <center>
                                            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                            <br />
                                            <asp:ValidationSummary ID="ChangeUserPasswordValidationSummary" runat="server" style="color:Red;" ValidationGroup="ChangeUserPasswordValidationGroup"/>
                                        </center>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

            </div>

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>



