<%@ Page Title="Select Site" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="SelectSiteV2.aspx.cs" Inherits="SelectSiteV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Select Site</span></div>
        <div class="main_content" id="main_content" runat="server" style="background: url(../imagesV2/login_bg.png) center top no-repeat #EDEDED;">

            <br />

            <center>
                <div class="login_form">

                    <asp:Repeater id="lstSites" runat="server">
                        <HeaderTemplate>
                            <table class="block_center">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class="nowrap">
                                <td><%# "[" + Eval("site_type_descr") + "]" %></td>
                                <td>&nbsp;&nbsp;<asp:LinkButton ID="lnkSelect" runat="server" OnCommand="lnkSelect_Click" CommandArgument='<%# Eval("site_id") %>' Text='<%# Eval("name") %>'></asp:LinkButton></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <asp:Label ID="lblNoSitesMessage" runat="server">You don't have access to any sties. Please contact a system administrator.</asp:Label>

                </div>
            </center>
            <br />

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>



