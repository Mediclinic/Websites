<%@ Page Title="Error" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Error" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server"></asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">

            <div class="user_login_form">

            </div>

            <div style="height:8px;"></div>

            <center>
                <table style="text-align:left;width:670px;">
                    <tr>
                        <td>

                            <h2>
                                <asp:Label ID="lblErrorMessageHeading" runat="server"/>
                            </h2>

                            <br />

                            <asp:Label ID="lblErrorMessage" runat="server"/>

                        </td>
                    </tr>
                </table>
            </center>

            <br /><br />

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>

        </div>
    </div>


</asp:Content>


