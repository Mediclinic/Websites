<%@ Page Title="Flashing Message" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="PatientFlashingMessageDetailV2.aspx.cs" Inherits="PatientFlashingMessageDetailV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title" id="page_title" runat="server"><asp:Label ID="lblHeading" runat="server">Flashing Message</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <table id="maintable" runat="server">
                    <tr id="edit_row" runat="server">
                        <td>Message</td>
                        <td style="width:8px;"></td>
                        <td><asp:TextBox ID="txtFlashingText" runat="server" Columns="50" /></td>
                        <td style="width:8px;"></td>
                        <td><asp:Button ID="btnClear" runat="server" Text="Clear" OnClientClick="document.getElementById('txtFlashingText').value='';return false;" /></td>
                    </tr>
                    <tr id="view_row" runat="server" visible="false">
                        <td colspan="3" style="text-align:center;"><asp:Label ID="lblFlashingText" runat="server" ForeColor="Red" Font-Bold="true" CssClass="failureNotification" style="font-size:150%;"></asp:Label></td>
                    </tr>
                    <tr>
                        <td colspan="3" style="text-align:center;">
                            <br />  
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="EditValidationSummary" />
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" onclick="btnEdit_Click" Visible="false" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click"  OnClientClick="window.returnValue=false;self.close();" />
                            <br />              
                        </td>
                    </tr>

                </table>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>

        </div>
    </div>


</asp:Content>



