<%@ Page Title="Offering Popup Message" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="OfferingPopupMessageDetailV2.aspx.cs" Inherits="OfferingPopupMessageDetailV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Popup Message When Creating Bookings</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <table id="maintable" runat="server">
                    <tr id="idRow" runat="server">
                        <td>ID</td>
                        <td></td>
                        <td><asp:Label ID="lblId" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>Offering</td>
                        <td style="min-width:10px;"></td>
                        <td>
                            <asp:Label ID="lblOffering" runat="server"></asp:Label>
                        </td>
                    </tr>

                    <tr style="vertical-align:top">
                        <td>Message</td>
                        <td></td>
                        <td>
                            <asp:TextBox ID="txtPopupMessage" runat="server" TextMode="MultiLine" Rows="4" Columns="46"></asp:TextBox>
                        </td>
                    </tr>

                    <tr style="height:5px;">
                        <td colspan="3"></td>
                    </tr>

                    <tr>
                        <td colspan="3" style="text-align:center;">
                            <br />  
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click"  OnClientClick="window.returnValue=false;self.close();" />
                            <br />              
                        </td>
                    </tr>

                </table>




            </center>

        </div>
    </div>


</asp:Content>



