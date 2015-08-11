<%@ Page Title="Contact List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ContactAusListV2.aspx.cs" Inherits="ContactAusListV2" %>
<%@ Register TagPrefix="UC" TagName="AddressAusControl" Src="~/Controls/AddressAusControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server">Contact</asp:Label></h3></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <table id="maintable" runat="server">
                    <tr>
                        <td>
                            <UC:AddressAusControl ID="addressControl" runat="server" />
                        </td>
                    </tr>
                    <tr style="height:35px;"><td></td></tr>
                    <tr>
                        <td style="text-align:center"><asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="javascript:window.close();" /></td>
                    </tr>
                </table>


            </center>

        </div>
    </div>


</asp:Content>



