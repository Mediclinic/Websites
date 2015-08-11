<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="TyroHealthPointReconciliationV2.aspx.cs" Inherits="TyroHealthPointReconciliationV2" validateRequest="false" %>

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



        function healthpointReconciliationReport() {
            clearLastResponses();

            // textbox seems to lose the value on post back
            document.getElementById('hiddenDate').value = $("#txtSearchDate").val();

            startTimer();

            iclient.healthpointReconciliationReport({
                reportType: $("#type").val(),
                reconDate: getDateFromTextBox()
            }, processResponse);

            /*
            iclient.healthpointReconciliationReport({
                reportType: "payments",
                reconDate: "20150504"
            }, processResponse);
            */
        }
        function processResponse(response) {

            stopTimer();
            console.log(JSON.stringify(response));

            document.getElementById('hiddenResultXML').value = response.result;
            document.getElementById('hiddenErrorCodeXML').value = response.healthpointErrorCode ? response.healthpointErrorCode : "";
            document.getElementById('hiddenErrorDescrXML').value = response.healthpointErrorDescription ? response.healthpointErrorDescription : "";
            document.getElementById('hiddenDataXML').value = response.data ? response.data : "";

            document.getElementById('btnProcessResults').click();
        }
        function clearLastResponses() {
            document.getElementById('hiddenResultXML').value = '';
            document.getElementById('hiddenErrorCodeXML').value = '';
            document.getElementById('hiddenErrorDescrXML').value = '';
            document.getElementById('hiddenDataXML').value = '';

            document.getElementById('lblXMLOutput').innerHTML = '';
            document.getElementById('lblReadableOutput').innerHTML = '';
            document.getElementById('lblRunning').innerHTML = '';           
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


        var runningTimer = null;
        var count = 0;
        function startTimer() {
            count = 0;
            document.getElementById('lblRunning').innerHTML = "Running.. (" + getTime(count) + ")";
            document.getElementById('divRunning').style.display = "";
            

            runningTimer = setInterval(function () {
                //document.getElementById('lblRunning').innerHTML += ".";
                document.getElementById('lblRunning').innerHTML = "Running.. (" + getTime(count) + ")";
                count++;
            }, 1000);
        }
        function stopTimer() {
            clearInterval(runningTimer);
            document.getElementById('lblRunning').innerHTML = '';
            document.getElementById('divRunning').style.display = "none";
        }
        function getTime(totalSec) {
            var totalMinutes = parseInt(totalSec / 60);
            var seconds = totalSec % 60;
            return totalMinutes + "m " + (seconds < 10 ? "0" + seconds : seconds) + "s";
        }


    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Tyro HealthPoint Reconcilliation</span></div>
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
                                            <option value="claims">Claims</option>
                                            <option value="payments">Payments</option>
                                        </select>
                                    </td>
                                    <td style="width:10px;"></td>
                                    <td>
                                        <button id="btnRunReport" type="button" class="mt10 btn btn-primary" onclick="healthpointReconciliationReport(); return false;">Run Report</button>
                                        <asp:Button ID="btnProcessResults" runat="server" class="hiddencol" Text="Process" OnClick="btnProcessResults_Click" />
                                    </td>
                                </tr>
                            </table>

                            <div id="divRunning">
                                <br />
                                <asp:Label ID="lblRunning" runat="server" ForeColor="Blue"/>
                            </div>
                            <br />

                            <asp:HiddenField ID="hiddenDate" runat="server" />
                            <asp:HiddenField ID="hiddenResultXML" runat="server" />
                            <asp:HiddenField ID="hiddenErrorCodeXML" runat="server" />
                            <asp:HiddenField ID="hiddenErrorDescrXML" runat="server" />
                            <asp:HiddenField ID="hiddenDataXML" runat="server" />
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
                <div id="divTestButtons" runat="server">
                    <asp:Button ID="btnTest1" runat="server" CssClass="thin_button" Text="Test1" OnCommand="btnTest_Click" CommandArgument="T1" />
                    <asp:Button ID="btnTest2" runat="server" CssClass="thin_button" Text="Test2" OnCommand="btnTest_Click" CommandArgument="T2"/>
                    <asp:Button ID="btnTest3" runat="server" CssClass="thin_button" Text="Test3" OnCommand="btnTest_Click" CommandArgument="T3"/>
                </div>
                <div>
                    <asp:Label ID="lblReadableOutput" runat="server" ForeColor="#493D26"></asp:Label>
                </div>
            </center>


            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>



