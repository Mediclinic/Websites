<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="TyroReconciliationV2.aspx.cs" Inherits="TyroReconciliationV2" validateRequest="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

    <style type="text/css">
        .form-control-tyro {
            display: block;
            height: 34px;
            padding: 6px 12px;
            font-size: 14px;
            line-height: 1.428571429;
            color: #555;
            vertical-align: middle;
            background-color: #fff;
            background-image: none;
            border: 1px solid #ccc;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,0.075);
            box-shadow: inset 0 1px 1px rgba(0,0,0,0.075);
            -webkit-transition: border-color ease-in-out .15s,box-shadow ease-in-out .15s;
            transition: border-color ease-in-out .15s,box-shadow ease-in-out .15s;
        }
    </style>

    <script type="text/javascript" src="https://iclientsimulator.test.tyro.com/iclient-with-ui-v1.js"></script>
    <script type="text/javascript">


        // the tyro js include page screws up our menu's, but the fix ruins the menu's on other pages
        // so this tells the master page to apply the fix only for this page
        var isTyro = true;

        var is_debugging = getUrlVars()['debug'] == "1";


        // Before running this fiddle, you will need to pair your virtual terminal with the browser using this page:
        // https://iclientsimulator.test.tyro.com/configuration.html

        var apiKey = "Test API Key"; // API Key not validated test environments
        var posProductInfo = {
            posProductVendor: "Acme Co",
            posProductName: "Acme Cloud POS",
            posProductVersion: "1.0.0"
        };
        var iclient = new TYRO.IClientWithUI(apiKey, posProductInfo);

        // The Tyro simulator uses 'magic' amounts. Refer to this page for more details:
        // https://integrationsimulator.test.tyro.com/docs/



        function reconciliationReport() {
            clearLastResponses();

            // textbox seems to lose the value on post back
            document.getElementById('hiddenDate').value = $("#txtSearchDate").val();

            // run detailed xml report to extract information
            iclient.reconciliationReport({
                type: 'detail',
                format: 'xml',
                terminalBusinessDay: getDateFromTextBox()
            }, processResponseXML);

        }
        function processResponseXML(response) {
            document.getElementById('hiddenResultXML').value = response.result;
            document.getElementById('hiddenErrorXML').value = response.error;
            document.getElementById('hiddenTypeXML').value = response.type;
            document.getElementById('hiddenFormatXML').value = response.format;
            document.getElementById('hiddenDataXML').value = response.data ? response.data : "";

            // run txt report to extract information
            iclient.reconciliationReport({
                type: $("#type").val(),
                format: 'txt',
                terminalBusinessDay: getDateFromTextBox()
            }, processResponseText);
        }
        function processResponseText(response) {
            document.getElementById('hiddenResultText').value = response.result;
            document.getElementById('hiddenErrorText').value = response.error;
            document.getElementById('hiddenTypeText').value = response.type;
            document.getElementById('hiddenFormatText').value = response.format;
            document.getElementById('hiddenDataText').value = response.data ? response.data : "";

            document.getElementById('btnProcessResults').click();
        }
        function clearLastResponses() {

            document.getElementById('hiddenResultXML').value = '';
            document.getElementById('hiddenErrorXML').value = '';
            document.getElementById('hiddenTypeXML').value = '';
            document.getElementById('hiddenFormatXML').value = '';
            document.getElementById('hiddenDataXML').value = '';

            document.getElementById('hiddenResultText').value = '';
            document.getElementById('hiddenErrorText').value = '';
            document.getElementById('hiddenTypeText').value = '';
            document.getElementById('hiddenFormatText').value = '';
            document.getElementById('hiddenDataText').value = '';
        }
        function getDateFromTextBox() {
            var rawDate = $("#txtSearchDate").val();
            var dateParts = rawDate.split("-");
            return String(dateParts[2]) + String(dateParts[1]) + String(dateParts[0]);
        }
        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Tyro Reconcilliation</span></div>
        <div class="main_content" id="main_content" runat="server">

            <div id="pairing_link" runat="server" class="text-center">
                <a href="https://iclientsimulator.test.tyro.com/configuration.html" onclick="open_new_tab('https://iclientsimulator.test.tyro.com/configuration.html');return false;">Pair virtual terminal with browser</a>
            </div>

            <table id="main_table" runat="server" class="block_center">
                <tr>
                    <td>
                        <center>

                            <br />

                            <table>
                                <tr>
                                    <td>

                                        <table>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">Date: </asp:Label></td>
                                                <td class="nowrap"><asp:TextBox ID="txtSearchDate" runat="server" Columns="10" ReadOnly="true"/></td>
                                                <td class="nowrap"><asp:ImageButton ID="txtSearchDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width:10px;"></td>
                                    <td>
                                        <select class="form-control-tyro" id="type">
                                            <option value="detail">Detail</option>
                                            <option value="summary">Summary</option>
                                        </select>
                                    </td>
                                    <td style="width:10px;"></td>
                                    <td>
                                        <button id="btnRunReport" type="button" class="mt10 btn btn-primary" onclick="reconciliationReport(); return false;">Run Report</button>
                                        <asp:Button ID="btnProcessResults" runat="server" class="hiddencol" Text="Process" OnClick="btnProcessResults_Click" />
                                    </td>
                                </tr>
                            </table>

                            <br />
                            <br />

                            <asp:HiddenField ID="hiddenDate" runat="server" />

                            <asp:HiddenField ID="hiddenResultXML" runat="server" />
                            <asp:HiddenField ID="hiddenErrorXML" runat="server" />
                            <asp:HiddenField ID="hiddenTypeXML" runat="server" />
                            <asp:HiddenField ID="hiddenFormatXML" runat="server" />
                            <asp:HiddenField ID="hiddenDataXML" runat="server" />

                            <asp:HiddenField ID="hiddenResultText" runat="server" />
                            <asp:HiddenField ID="hiddenErrorText" runat="server" />
                            <asp:HiddenField ID="hiddenTypeText" runat="server" />
                            <asp:HiddenField ID="hiddenFormatText" runat="server" />
                            <asp:HiddenField ID="hiddenDataText" runat="server" />
                          
                        </center>

                    </td>
                </tr>
            </table>

            <center>
                <div>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>

                <div>
                    <asp:Label ID="lblXMLOutput" runat="server" ForeColor="Blue" CssClass="failureNotification"></asp:Label>
                </div>

                <div>
                    <asp:Label ID="lblTextOutput" runat="server" ForeColor="Blue" CssClass="failureNotification"></asp:Label>
                </div>
            </center>


            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>



