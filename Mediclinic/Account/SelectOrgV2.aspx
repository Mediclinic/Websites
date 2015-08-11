<%@ Page Title="Select Clinic" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="SelectOrgV2.aspx.cs" Inherits="SelectOrgV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Select A Clinic</span></div>
        <div class="main_content" id="main_content" runat="server" style="background: url(../imagesV2/login_bg.png) center top no-repeat #EDEDED;">

            <br />

            <center>
                <div class="login_form">

                    <asp:Repeater id="lstOrgs" runat="server" onitemdatabound="lstOrgs_ItemDataBound">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkSelect" runat="server" OnCommand="lnkSelect_Click" CommandArgument='<%# Eval("organisation_id") %>' Text='<%# Eval("name") %>'></asp:LinkButton>
                            <br />
                        </ItemTemplate>
                        <FooterTemplate>
                                <asp:Label ID="lblEmptyData" Text="You have not been allocated to any clinics. Please contact an administrator." runat="server" Visible="false"></asp:Label>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>
            </center>

            <br />

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>



