<%@ Page Title="Create Cash Invoice" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="InvoiceCashAddV2.aspx.cs" Inherits="InvoiceCashAddV2" EnableEventValidation="false" %>
<%@ Register TagPrefix="UC" TagName="InvoiceItemsControl" Src="~/Controls/InvoiceItemsControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function get_patient() {
            var retVal = window.showModalDialog("PatientListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:700px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined")
                return;

            var index = retVal.indexOf(":");
            document.getElementById('txtUpdatePatientID').value = retVal.substring(0, index);
            document.getElementById('txtUpdatePatientName').value = retVal.substring(index + 1);
            document.getElementById('btnPatientListPopup').blur();
        }

        function clear_patient() {
            document.getElementById('txtUpdatePatientID').value = '';
            document.getElementById('txtUpdatePatientName').value = '';
            document.getElementById('txtClearPatient').blur();
        }

        function get_organisation() {
            var retVal = window.showModalDialog("OrganisationListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:750px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined")
                return;

            var index = retVal.indexOf(":");
            document.getElementById('txtUpdateOrganisationID').value = retVal.substring(0, index);
            document.getElementById('txtUpdateOrganisationName').value = retVal.substring(index + 1);
            document.getElementById('btnOrganisationListPopup').blur();
        }

        function clear_organisation() {
            document.getElementById('txtUpdateOrganisationID').value = '';
            document.getElementById('txtUpdateOrganisationName').value = '';
            document.getElementById('txtClearOrganisation').blur();
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
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Create Cash Invoice</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">
                    <center>

                        <asp:Button ID="btnHidden" runat="server" OnClientClick="return false;" style="display:none;"/>


                        <table class="padded-table-2px">
                            <tr id="patient_row" runat="server">
                                <td>Patient</td>
                                <td style="width:10px"></td>
                                <td>
                                    <asp:TextBox ID="txtUpdatePatientID" runat="server" CssClass="hiddencol" />
                                    <asp:TextBox ID="txtUpdatePatientName" runat="server" Width="230" Enabled="False" ForeColor="Black" style="border:0px solid;background-color:white;" />
                                </td>
                                <td>
                                    <asp:Button ID="btnPatientListPopup" runat="server" Text="Get Patient" OnClientClick="javascript:get_patient(); return false;" style="width:100%;" CssClass="thin_button"/>
                                </td>
                                <td>
                                    <asp:Button ID="txtClearPatient"     runat="server" Text="Clear"       OnClientClick="javascript:clear_patient();return false;" CssClass="thin_button" />

                                    <asp:Button ID="btnUpdatePatient"    runat="server" CssClass="hiddencol" Text="Get ID in Code Behind" onclick="btnUpdatePatient_Click" />
                                    <asp:Label  ID="lblPatientName"      runat="server" CssClass="hiddencol" Text="--" />
                                </td>
                            </tr>
                            <tr id="org_row" runat="server">
                                <td>Organisation</td>
                                <td style="width:10px"></td>
                                <td>
                                    <asp:TextBox ID="txtUpdateOrganisationID" runat="server" CssClass="hiddencol" />
                                    <asp:TextBox ID="txtUpdateOrganisationName" runat="server" Width="230" Enabled="False" ForeColor="Black" style="border:0px solid;background-color:white;" />
                                </td>
                                <td>
                                    <asp:Button ID="btnOrganisationListPopup" runat="server" Text="Get Organisation" OnClientClick="javascript:get_organisation(); return false;" style="width:100%;" CssClass="thin_button" />
                                </td>
                                <td>
                                    <asp:Button ID="txtClearOrganisation"     runat="server" Text="Clear"            OnClientClick="javascript:clear_organisation();return false;" CssClass="thin_button" />

                                    <asp:Button ID="btnUpdateOrganisation"    runat="server" CssClass="hiddencol" Text="Get ID in Code Behind" onclick="btnUpdateOrganisation_Click" />
                                    <asp:Label  ID="lblOrganisationName"      runat="server" CssClass="hiddencol" Text="--" />
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

                <UC:InvoiceItemsControl ID="invoiceItemsControl" runat="server" />
            
            </center>
            

        </div>
    </div>

</asp:Content>



