<%@ Page Title="Next Availalbe Booking" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="BookingNextAvailableV2.aspx.cs" Inherits="BookingNextAvailableV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Next Available Booking</span></div>
        <div class="main_content_with_header" style="padding-top:1px;">
            <div id="user_login_form" runat="server" class="user_login_form">

                <div>
                    <center>
                        <table>
                            <tr>
                                <td>Date</td>
                                <td style="min-width:10px;"></td>
                                <td>
                                    <table>
                                        <tr>
                                            <td><asp:TextBox ID="txtStartDate" runat="server" Columns="10"></asp:TextBox></td>
                                            <td style="width:3px;"></td>
                                            <td><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                            <td style="width:25px;text-align:center;"> - </td>
                                            <td><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                                            <td style="width:3px;"></td>
                                            <td><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>Clinic</td>
                                <td></td>
                                <td><asp:DropDownList ID="ddlClinics" runat="server"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td>Provider</td>
                                <td></td>
                                <td><asp:DropDownList ID="ddlProviders" runat="server"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td>Service</td>
                                <td></td>
                                <td><asp:DropDownList ID="ddlServices" runat="server"></asp:DropDownList></td>
                            </tr>
                            <tr style="height:8px;">
                                <td colspan="3"></td>
                            </tr>
                            <tr>
                                <td colspan="3" style="text-align:center;">
                                    <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Submit" CssClass="thin_button" />
                                </td>
                            </tr>
                        </table>
                    </center>
                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>



            <center>
                <div id="autodivheight" runat="server" class="divautoheight" style="height:500px;min-height:160px;">

                    <asp:Label ID="lblOutput" runat="server" ForeColor="Blue"></asp:Label>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



