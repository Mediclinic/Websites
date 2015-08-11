<%@ Page Title="Login" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="LoginV2.aspx.cs" Inherits="LoginV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Log In</span></div>
        <div class="main_content" id="main_content" runat="server" style="background: url(../imagesV2/login_bg.png) center top no-repeat #EDEDED;">

            <br />

            <center>

                <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="LoginButton" CssClass="login_form">
                <table>

                    <tr>
                        <td style="white-space:nowrap;">
                            ALL DATA STORED IN AUSTRALIA ON AUSTRALIAN SERVERS
                            <div style="height:10px;"></div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div id="beforeDevPanelSpace" runat="server" style="height:15px;"></div>
                            <asp:Panel ID="DevPanel" runat="server" CssClass="nowrap" style="text-align:center;">
                                <asp:Button ID="btnDevLoginSupport"  runat="server" onclick="btnDevLogin_Click" CommandArgument="Support1" Text="Support Login" CssClass="btn btn-primary" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>

                            <table class="block_center">
                                <tr>
                                    <td class="nowrap">
                                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Font-Bold="false">Username:</asp:Label>
                                    </td>
                                    <td class="nowrap">&nbsp;
                                        <asp:TextBox ID="UserName" runat="server" autocomplete="off"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                            ControlToValidate="UserName" ErrorMessage="Username is required." 
                                            Display="Dynamic"
                                            ToolTip="User Name is required." ValidationGroup="LoginUserValidationGroup" CssClass="failureNotification" >*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">
                                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Font-Bold="false">Password:</asp:Label>
                                    </td>
                                    <td class="nowrap">&nbsp;
                                        <asp:TextBox ID="Password" runat="server" TextMode="Password" autocomplete="off"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                            ControlToValidate="Password" ErrorMessage="Password is required." 
                                            Display="Dynamic"
                                            ToolTip="Password is required." ValidationGroup="LoginUserValidationGroup" CssClass="failureNotification" >*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr id="beforeButtonSpace" runat="server" style="height:15px;">
                                    <td colspan="2"></td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="font-size:85%;white-space:nowrap;">
                                        By Logging in, you agree to the <asp:HyperLink ID="lnkTermsAndConditinos" runat="server" Text="Terms & Conditions Of Use" NavigateUrl="~/Account/TermsAndConditionsV2.aspx" Target="_blank"></asp:HyperLink>
                                    </td>
                                </tr>
                                <tr style="height:6px;">
                                    <td colspan="2"></td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="text-align:center;">
                                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" onclick="LoginButton_Click" ValidationGroup="LoginUserValidationGroup" CssClass="btn btn-primary" />
                                    </td>
                                </tr>
                                <tr id="afterButtonSpace" runat="server" style="height:20px;">
                                    <td colspan="2"></td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="color:Red;">
                                        <table class="block_center">
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                        <table class="block_center">
                                            <tr>
                                                <td>
                                                    <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" style="color:Red;" ValidationGroup="LoginUserValidationGroup"/>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr id="recommendMozilla" runat="server" >
                                    <td colspan="2">
                                        <div style="height:15px;"></div>
                                        <asp:Label ID="RecommendMozillaText" runat="server" style="color:black;font-weight:bold;">For Best results we recommend using MOZILLA FIREFOX.  Our security is the highest possible and some browsers do not like the high security levels and can obstruct your data flow. <a href="http://www.downloadfirefoxbrowser.com/" onclick="open_new_tab('http://www.downloadfirefoxbrowser.com/');return false;" style="color:inherit;font-weight:normal;" onmouseover="this.style.color='white'" onmouseout="this.style.color='inherit'" >Download Firefox</a>.</asp:Label>
                                    </td>                                   
                                </tr>

                                <tr style="height:15px;">
                                    <td colspan="2"></td>
                                </tr>
                                <tr>
                                    <td colspan="2"  class="nowrap" style="text-align:right;">
                                        <asp:LinkButton ID="lnkLostPassword" runat="server" OnClick="lnkLostPassword_Click">Forgot your password?</asp:LinkButton>
                                    </td>
                                </tr>
                                <tr runat="server" visible="false">
                                    <td colspan="2"  class="nowrap" style="text-align:right;">
                                        <asp:LinkButton ID="lnkPatientCreateLogin" runat="server" OnClick="lnkPatientCreateLogin_Click">Existing patient without a web login?</asp:LinkButton>
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



