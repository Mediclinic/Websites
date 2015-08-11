<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="TestPrint.aspx.cs" Inherits="_Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <asp:Button ID="btnTest3" runat="server" Text="Test 1 - all printers" OnCommand="btnTest3_Command" CommandArgument="" Width="165" />
    <br />
    <br />
    <asp:Button ID="btnTest1" runat="server" Text="Test - OKI C9800 PCL" OnCommand="btnTest_Command" CommandArgument="OKI C9800 PCL" Width="165" />
    <br />
    <asp:Button ID="btnTest2" runat="server" Text="Test - OKI C9600 PCL 5" OnCommand="btnTest_Command" CommandArgument="OKI C9600 PCL 5" Width="165" />
    <br />
    
    


    <br />
    <asp:Label ID="lblMeg" runat="server" />

</asp:Content>
