<%@ Page Title="Unsubscribe" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="PatientUnsubscribeV2.aspx.cs" Inherits="PatientUnsubscribeV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Unsubscribe</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <table>
                    <tr>
                        <td>

                            <div style="height:15px;"></div>

                            <center>
                                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>

                                <br />
                                <h3><asp:Label ID="lblResult" runat="server" ></asp:Label></h3>

                            </center>

                        </td>
                    </tr>
                </table>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:100px;">
            </div>

        </div>
    </div>


</asp:Content>



