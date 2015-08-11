<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddressAusControlV2.ascx.cs" Inherits="Controls_AddressAusControlV2" %>

<asp:HiddenField ID="addressControlEntityID" runat="server" />
<asp:HiddenField ID="addressControlEnabled" runat="server" />
<asp:HiddenField ID="addressEntityType" runat="server" />
<asp:HiddenField ID="addressIncBedrooms" runat="server" />

<table id="tblBedrooms" runat="server" border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td class="nowrap">
            <asp:Label ID="lblBedrooms" runat="server" Font-Bold="True" Text="Bedrooms"></asp:Label>
            &nbsp;
            <asp:HyperLink ID="lnkAddBedrooms" runat="server" Text="Add" ImageUrl="~/images/add-icon-10.png" CssClass="nowrap" ></asp:HyperLink>

        </td>
    </tr>
</table>
<asp:PlaceHolder ID="phBedrooms" runat="server"></asp:PlaceHolder>

<table>
    <tr>
        <td class="nowrap">
            <asp:Label ID="lblAddresses" runat="server" Font-Bold="True" Text="Addresses"></asp:Label><asp:Button ID="btnUpdateAddresses" runat="server" CssClass="hiddencol" onclick="btnUpdateAddresses_Click" CausesValidation="false" />
            &nbsp;
            <asp:HyperLink ID="lnkAddAddress" runat="server" Text="Add" ImageUrl="~/images/add-icon-10.png" CssClass="nowrap" ></asp:HyperLink>

        </td>
    </tr>
</table>
<asp:PlaceHolder ID="phAddresses" runat="server"></asp:PlaceHolder>

<div style="line-height:4px;">&nbsp;</div>

<table>
    <tr>
        <td class="nowrap">
            <asp:Label ID="lblphoneNums" runat="server" Font-Bold="True" Text="Phone Numbers"></asp:Label>
            &nbsp;
            <asp:HyperLink ID="lnkAddPhoneNums" runat="server" Text="Add" ImageUrl="~/images/add-icon-10.png" CssClass="nowrap" ></asp:HyperLink>
        </td>
    </tr>
</table>
<asp:PlaceHolder ID="phPhoneNums" runat="server"></asp:PlaceHolder>

<div style="line-height:4px;">&nbsp;</div>

<table>
    <tr>
        <td class="nowrap">
            <asp:Label ID="lblEmails" runat="server" Font-Bold="True" Text="Email/Website"></asp:Label>
            &nbsp;
            <asp:HyperLink ID="lnkAddEmails" runat="server" Text="Add" ImageUrl="~/images/add-icon-10.png" CssClass="nowrap" ></asp:HyperLink>
        </td>
    </tr>
</table>
<asp:PlaceHolder ID="phEmails" runat="server"></asp:PlaceHolder>
