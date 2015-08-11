<%@ Page Title="Call Centre" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="EmailEmployeesV2.aspx.cs" Inherits="EmailEmployeesV2" ValidateRequest="false" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1); });
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Send Email To All Employees</span></div>
        <div class="main_content_with_header">

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>


                <br />

                <b>Subject</b>&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtSubject" runat="server" style="width:650px;" onkeyup="capitalize_first(this);"></asp:TextBox>
                <div style="height:15px;"></div>

                <FTB:FreeTextBox id="FreeTextBox1" runat="Server" Text="" Width="725px" Height="305px" />

                <a href="javascript:void(0)" title="If you have an online link to your company logo, click 'HTML' at the bottum left of the text area and change 
                                       
                    https://portal.mediclinic.com.au/imagesV2/comp_logo.png

to a link of your own company logo, then click back to 'Design' view.
You may need to ask your web developer for a this image logo link." >Tip (Hover)</a>

                <br /><br />

                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnPreviewEmail" runat="server" OnClick="btnPreviewEmail_Click" Text="Preview" CssClass="thin_button" style="width:100%" />
                            <br />
                            <asp:Button ID="btnSendEmail" runat="server" OnClick="btnSendEmail_Click" Text="Send Emails" CssClass="thin_button" style="width:100%" OnClientClick="javascript:if (!confirm('This can not be undone. Are you sure?')) return false;" />

                        </td>
                    </tr>
                </table>

                <a name="emailing_tag"></a>

                <br />

                <asp:Label ID="lblEmailOutput" runat="server"></asp:Label>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>
    
</asp:Content>



