<%@ Page Title="Allocated Claim Numbers" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ClaimNumbersAllocation_AllDBsV2.aspx.cs" Inherits="ClaimNumbersAllocation_AllDBsV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="Scripts/post_to_url.js"></script>
    <script type="text/javascript">

        function http_post(db, org) {
            var h = new Object(); // or just {}
            h['db']  = db;
            h['org'] = org;
            post_to_url(window.location.pathname, h, "post");
        }

        function hide_show(id) {
            var e = document.getElementById(id);
            if (e != null)
               e.style.display = e.style.display == "none" ? "" : "none";
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Allocated Claim Numbers</span></div>
        <div class="main_content_with_header">

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:200px;width:auto;padding-right:17px;">
                    <asp:Label ID="lblOutput" runat="server"></asp:Label>
                </div>
            </center>

        </div>
    </div>
    
</asp:Content>



