<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddressControl.ascx.cs" Inherits="Controls_AddressControl" %>

<asp:HiddenField ID="addressControlEntityID" runat="server" />
<asp:HiddenField ID="addressControlEnabled" runat="server" />
<asp:HiddenField ID="addressEntityType" runat="server" />
<asp:HiddenField ID="addressIncBedrooms" runat="server" />

<asp:Label ID="lblBedrooms" runat="server" Font-Bold="True" Text="Bedrooms"></asp:Label>
&nbsp;
<asp:HyperLink ID="lnkAddBedrooms" runat="server" Text="Add" ImageUrl="~/images/add-icon-10.png" CssClass="nowrap" ></asp:HyperLink>
<br />
<asp:PlaceHolder ID="phBedrooms" runat="server"></asp:PlaceHolder>

<asp:Label ID="lblAddresses" runat="server" Font-Bold="True" Text="Addresses"></asp:Label><asp:Button ID="btnUpdateAddresses" runat="server" CssClass="hiddencol" onclick="btnUpdateAddresses_Click" CausesValidation="false" />
&nbsp;
<asp:HyperLink ID="lnkAddAddress" runat="server" Text="Add" ImageUrl="~/images/add-icon-10.png" ></asp:HyperLink>
<br />
<asp:PlaceHolder ID="phAddresses" runat="server"></asp:PlaceHolder>

<div style="line-height:4px;">&nbsp;</div>

<asp:Label ID="lblphoneNums" runat="server" Font-Bold="True" Text="Phone Numbers"></asp:Label>
&nbsp;
<asp:HyperLink ID="lnkAddPhoneNums" runat="server" Text="Add" ImageUrl="~/images/add-icon-10.png" ></asp:HyperLink>
<br />
<asp:PlaceHolder ID="phPhoneNums" runat="server"></asp:PlaceHolder>

<div style="line-height:4px;">&nbsp;</div>

<asp:Label ID="lblEmails" runat="server" Font-Bold="True" Text="Email/Website"></asp:Label>
&nbsp;
<asp:HyperLink ID="lnkAddEmails" runat="server" Text="Add" ImageUrl="~/images/add-icon-10.png" ></asp:HyperLink>
<br />
<asp:PlaceHolder ID="phEmails" runat="server"></asp:PlaceHolder>
