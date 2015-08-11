<%@ Page Title="Service-Specific Booking Reminder Letters To Send Today" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Letters_ServiceSpecificBookingRemindersToSendV2.aspx.cs" Inherits="Letters_ServiceSpecificBookingRemindersToSendV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function notification_info_edited() {

            //elem.style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("txtEmailAddress").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 

            document.getElementById("update_button_row").className = ""; // make it visible

            document.getElementById("test_run_button_row").className = "hiddencol";
        }

        function open_new_window(URL) {
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,height=" + screen.height + ',width=' + screen.width);
            NewWindow.location = URL;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Service-Specific Booking Reminder Letters To Send Today</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">
            <div class="user_login_form">

                <div class="border_top_bottom">
                    <table>
                        <tr>
                            <td>
                                Email Batch To: <asp:TextBox ID="txtEmailAddress" runat="server" Columns="35" onkeyup="notification_info_edited();"></asp:TextBox>
                            </td>
                            <td  style="width:10px"></td>
                            <td colspan="3" align="center" id="update_button_row" class="hiddencol" runat="server">
                                <asp:Button ID="btnUpdateNotificationInfo" runat="server" Text="Update" OnClick="btnUpdateNotificationInfo_Click" />
                                <asp:Button ID="btnRevertNotificationInfo" runat="server" Text="Revert" OnClick="btnRevertNotificationInfo_Click" />
                            </td>
                            <td colspan="3" align="center" id="test_run_button_row" runat="server">
                                <asp:Button ID="btnTestWithoutPtEmailing" runat="server" Text="Run Test w/o PT Emailing" OnClick="btnTestWithoutPtEmailing_Click" Width="100%" />
                                <br />
                                <asp:Button ID="btnTestWithPtEmailing" runat="server" Text="Run Test with PT Emailing" OnClick="btnTestWithPtEmailing_Click" Width="100%" />
                            </td>
                        </tr>
                    </table>
                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <asp:Button ID="btnUpdateList" runat="server" Text="Refresh List" OnClick="btnUpdateList_Click" CssClass="hiddencol"  />

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:Literal ID="htmlOutput" runat="server" />

                </div>
            </center>

        </div>
    </div>


</asp:Content>


