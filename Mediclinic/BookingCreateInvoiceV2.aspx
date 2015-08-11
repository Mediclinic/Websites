<%@ Page Title="Complete Booking" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="BookingCreateInvoiceV2.aspx.cs" Inherits="BookingCreateInvoiceV2" EnableEventValidation="false" %>
<%@ Register TagPrefix="UC" TagName="InvoiceItemsControl" Src="~/Controls/InvoiceItemsControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function show_page_load_message() {

            // delay so it is only shown if it is taking a long time (ie if a last treatment letter is being generated)
            setTimeout(function () {
                show_hide('loadingDiv', true);
            }, 750);
        }
        function show_hide(id, show) {
            obj = document.getElementById(id);
            obj.style.display = show ? "" : "none";
        }



        function live_search(str) {

            if (str.length == 0) {
                clear_live_search();
                return;
            }

            var fieldsSep = "[[fieldsSep]]";
            var itemSep = "[[itemSep]]";

            var countFound = 0;
            var output = "<table>";
            var offerings = document.getElementById('hiddenItemList').value;
            var offeringLines = offerings.split(itemSep);
            for (var i = 0; i < offeringLines.length; i++) {
                var fields = offeringLines[i].split(fieldsSep);
                if (fields[0].toLowerCase().indexOf(str.toLowerCase()) != -1) {
                    countFound++;
                    //output += "<tr><td>" + fields[0] + "</td><td>" + fields[1] + "</td></tr>";  // <<< it one td, with javascript click link
                    output += "<tr><td><a href=javascript:void(0)'  onclick=\"clear_live_search();document.getElementById('" + fields[1] + "').click();return false;\">" + fields[0] + "</a></td></tr>";
                }
            }
            output += "</table>";

            document.getElementById("div_livesearch").innerHTML = countFound == 0 ? "<table><tr><td>No results matching that text</td></tr></table>" : output;
            document.getElementById("div_livesearch").style.border = "1px solid #A5ACB2";
        }

        function clear_live_search() {
            document.getElementById("div_livesearch").innerHTML = "";
            document.getElementById("div_livesearch").style.border = "0px";
            document.getElementById("txtSearchOffering").value = "";
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Complete Booking</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">
            <div class="user_login_form" style="width: 400px;">

                <div class="border_top_bottom">
                    <center>

                        <table>
                            <tr style="text-align:center;">
                                <td><asp:ImageButton ID="lnkNotes" runat="server" AlternateText="Notes" ToolTip="Notes" /><asp:Button ID="btnUpdateNotesIcon" runat="server" CssClass="hiddencol" onclick="btnUpdateNotesIcon_Click" /></td>
                                <td id="notesMessageSpace" runat="server" style="width:50px"></td>
                                <td><asp:Label ID="lblNotesMessage" runat="server"/></td>
                                <td></td>
                            </tr>
                            <tr style="text-align:center;">
                                <td colspan="4">
                                    <asp:CheckBox ID="chkGenerateSystemLetters" runat="server" Text="&nbsp;Generate System Letters Now" />
                                </td>
                            </tr>
                        </table>

                    </center>
                </div>

            </div>


            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <center>

                <table>
                    <tr>
                        <td style="width:25px"></td>
                        <td><UC:InvoiceItemsControl ID="invoiceItemsControl" runat="server" /></td>
                        <td style="width:25px"></td>
                    </tr>
                </table>
            
            </center>


        </div>
    </div>


</asp:Content>



