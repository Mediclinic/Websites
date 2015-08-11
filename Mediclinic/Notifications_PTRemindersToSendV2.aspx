<%@ Page Title="Patient Reminders To Send" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Notifications_PTRemindersToSendV2.aspx.cs" Inherits="Notifications_PTRemindersToSendV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Patient Reminders To Send</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">

            <div class="user_login_form">

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:Literal ID="htmlOutput" runat="server" />

                </div>
            </center>

        </div>
    </div>


</asp:Content>


