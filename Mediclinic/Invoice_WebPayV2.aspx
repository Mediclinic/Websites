<%@ Page Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Invoice_WebPayV2.aspx.cs" Inherits="Invoice_WebPayV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading"></span></div>

            <asp:Button ID="btnSubmit" runat="server" CssClass="hiddencol" OnClick="btnSubmit_Click" />

            <div class="text_center">

                <br />
                <br />
                <h4>Please wait while we redirect you to the payment processor</h4>

                <br />
                <br />
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

    </div>

</asp:Content>