<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Default7.aspx.cs" Inherits="_Default7" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">&nbsp;</span></div>
        <div class="main_content" id="main_content" runat="server">

            <table class="block_center">
                <tr>
                    <td>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary1"/>
                        <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                    </td>
                </tr>
            </table>


            <br />

            
            <br />






            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>


