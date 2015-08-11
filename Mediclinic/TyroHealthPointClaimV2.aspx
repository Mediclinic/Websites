<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="TyroHealthPointClaimV2.aspx.cs" Inherits="TyroHealthPointClaimV2" validateRequest="false" %>

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

    <style>
        table.left-right-padded-3px th, td { padding-left:3px; padding-right:3px; }
    </style>

    <script type="text/javascript" src="https://iclientsimulator.test.tyro.com/iclient-with-ui-v1.js"></script>
    <script type="text/javascript">


        // the tyro js include page screws up our menu's, but the fix ruins the menu's on other pages
        // so this tells the master page to apply the fix only for this page
        var isTyro = true;

        // before a transaction, pending transaction inserted into db and this is set to be used for 
        // the callback post transaction to update the db
        var tyro_transaction_id = null;

        var is_debugging = getUrlVars()['debug'] == "1";
        var send = getUrlVars()['send'] == null || getUrlVars()['send'] != "0";


        if (is_debugging) {
            addLoadEvent(function () { set_ddl_value('ddlPtId_Digit1', '4'); set_ddl_value('ddlPtId_Digit2', '7'); });
        }

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
        function initiateHealthPointClaim() {
            document.getElementById('lblErrorMessage').innerHTML = '';

            if (ajax_check_session_timed_out()) {
                alert("Session timed out. You will be redirected to the login page to login again to complete this transaction.");
                window.location.href = window.location.href;  // reload page so they have to login again
                return;
            }
            
            var ptId = get_ddl_value('ddlPtId_Digit1') + get_ddl_value('ddlPtId_Digit2');
            if (ptId[0] == '-' || ptId[1] == '-')
                { alert('Please Make Sure Both Patient Number Digits Are Set'); return; }
            for (var i = 0; i < _allClaimItems.length; i++)
                _allClaimItems[i].patientId = ptId;

            if (is_debugging) {

                var debugClaimItems = "<table class=\"block_center\" style=\"text-align:left;\">";
                for (var i = 0; i < _allClaimItems.length; i++)
                    debugClaimItems += "</tr>" +
                        "<td><font color=\"blue\">claimAmount: </font>" + _allClaimItems[i].claimAmount + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;serviceCode: </font>" + _allClaimItems[i].serviceCode + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;description: </font>" + _allClaimItems[i].description + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;serviceReference: </font>" + _allClaimItems[i].serviceReference + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;patientId: </font>" + _allClaimItems[i].patientId + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;serviceDate: </font>" + _allClaimItems[i].serviceDate + "</td>" +
                        "</tr>";
                debugClaimItems += "</table>";

                var debugItems =
                    "providerId: " + _providerId + "\r\n" +
                    "serviceType: " + _serviceType + "\r\n" +
                    "claimItemsCount: " + _claimItemsCount + "\r\n" +
                    "totalClaimAmount: " + _totalClaimAmount + "\r\n" +
                    "claimItems: \r\n" + debugClaimItems + "\r\n";  // JSON.stringify(_allClaimItems)

                debugItems = debugItems.replace(/(?:\r\n|\r|\n)/g, '<br />');

                document.getElementById('lblErrorMessage').innerHTML = debugItems;

                if (!send) // put "send=0" into url (with debug=1 to just see what will be sent without sending it
                    return;
            }



            // Note: (24/4/2015)
            // The tyro system does not currently have a trasactionId that is sent in for automatic reconcilliation the way
            // they do for purchases
            // When I asked, they said "oh yea, we will have to put that in"
            // So right now, tyro_transaction_id is not used - but I have put it in here so that when it is setup, reconcillation
            // will be ready to use (after puttin in a Reconcilliation() method into TyroHealthClaimDB the way it is in TyroPaymentDB


            // set page global variable for callback to have access to and save completed transaction info
            tyro_transaction_id = ajax_tyro_healthpoint_insert(getUrlVars()['invoice'], String(_totalClaimAmount));

            if (tyro_transaction_id == null) {
                alert("Error: tyro_transaction_id is null");
                return
            }

            if (tyro_transaction_id.startsWith('ISPAID')) {
                var overPaymentAmount = tyro_transaction_id.substr(12);
                document.getElementById('lblErrorMessage').innerHTML = 'Invoice already set as paid';
                return
            }
            if (tyro_transaction_id.startsWith('HASPAYMENT')) {
                var overPaymentAmount = tyro_transaction_id.substr(12);
                document.getElementById('lblErrorMessage').innerHTML = 'Invoice already has a payment on it';
                return
            }

            reset();
            doInitiateHealthPointClaim(_providerId, _serviceType, _claimItemsCount, _totalClaimAmount, _allClaimItems);  // get args set from code behind
        }
        function cancelHealthPointClaim() {
            document.getElementById('lblErrorMessage').innerHTML = '';

            if (ajax_check_session_timed_out()) {
                alert("Session timed out. You will be redirected to the login page to login again to complete this transaction.");
                window.location.href = window.location.href;  // reload page so they have to login again
                return;
            }

            var ptId = get_ddl_value('ddlPtId_Digit1') + get_ddl_value('ddlPtId_Digit2');
            if (ptId[0] == '-' || ptId[1] == '-')
            { alert('Please Make Sure Both Patient Number Digits Are Set'); return; }
            for (var i = 0; i < _allClaimItems.length; i++)
                _allClaimItems[i].patientId = ptId;

            if (is_debugging) {

                var debugClaimItems = "<table class=\"block_center\" style=\"text-align:left;\">";
                for (var i = 0; i < _allClaimItems.length; i++)
                    debugClaimItems += "</tr>" +
                        "<td><font color=\"blue\">claimAmount: </font>" + _allClaimItems[i].claimAmount + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;serviceCode: </font>" + _allClaimItems[i].serviceCode + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;description: </font>" + _allClaimItems[i].description + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;serviceReference: </font>" + _allClaimItems[i].serviceReference + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;patientId: </font>" + _allClaimItems[i].patientId + "</td>" +
                        "<td><font color=\"blue\">&nbsp;&nbsp;serviceDate: </font>" + _allClaimItems[i].serviceDate + "</td>" +
                        "</tr>";
                debugClaimItems += "</table>";

                var debugItems =
                    "providerId: " + _providerId + "\r\n" +
                    "serviceType: " + _serviceType + "\r\n" +
                    "claimItemsCount: " + _claimItemsCount + "\r\n" +
                    "totalClaimAmount: " + _totalClaimAmount + "\r\n" +
                    "refTag: " + _refTag + "\r\n" +
                    "claimItems: \r\n" + debugClaimItems + "\r\n";  // JSON.stringify(_allClaimItems)

                debugItems = debugItems.replace(/(?:\r\n|\r|\n)/g, '<br />');

                document.getElementById('lblErrorMessage').innerHTML = debugItems;

                if (!send) // put "send=0" into url (with debug=1 to just see what will be sent without sending it
                    return;
            }



            // make sure they can cancel
            tyro_can_cancel = ajax_tyro_healthpoint_can_cancel(getUrlVars()['reftag']);

            if (tyro_can_cancel == null) {
                alert("Error: unable to get result");
                return
            }
            if (tyro_can_cancel != "OK") {
                alert(tyro_can_cancel);
                return
            }


            reset();
            doCancelHealthPointClaim(_providerId, _serviceType, _claimItemsCount, _totalClaimAmount, _allClaimItems, getUrlVars()['reftag']);  // get args set from code behind
        }


        function reset() {
            document.getElementById('lblResult').value = '';
            document.getElementById('hiddenResponse').value = '';
            document.getElementById('lblXML').innerHTML = '';
        }
        var transactionCompleteCallbackImpl_Claim = function (response) {
            processResult('claim',response);
        };
        var transactionCompleteCallbackImpl_Cancel = function (response) {
            processResult('cancel', response);
        };
        function processResult(type, response) {

            console.log(JSON.stringify(response));

            document.getElementById('lblResult').value = formatResult(response);

            // process result in page behind
            document.getElementById('hiddenResponse').value = saveResult(response);
            tyro_transaction_id = null;  // unset it

            /*  
            // don't return here ... let it go to page behind so we can email alert on the error
            if (response.result != "APPROVED") {

                var errMsg = "<br />Result: <b>" + response.result + "</b>"

                if (response.healthpointErrorCode && response.healthpointErrorDescription)
                    errMsg += "<br />" + response.healthpointErrorDescription + " (Error Code " + response.healthpointErrorCode + ")";
                else if (response.healthpointErrorCode)
                    errMsg += "<br />Error Code: " + response.healthpointErrorCode;
                else if (response.healthpointErrorDescription)
                    errMsg += "<br />Error: " + response.healthpointErrorDescription;

                document.getElementById('lblErrorMessage').innerHTML += errMsg;

                return; 
            }
            */

            if (type == 'claim')
                document.getElementById('btnSaveResult').click();
            if (type == 'cancel')
                document.getElementById('btnSaveCancellation').click();
        };
        function doInitiateHealthPointClaim(providerId, serviceType, claimItemsCount, totalClaimAmount, claimItems) {

            iclient.initiateHealthPointClaim({
                providerId : providerId,
                serviceType : serviceType,
                claimItemsCount : claimItemsCount,
                totalClaimAmount : totalClaimAmount,
                claimItems : claimItems  // JSON array
            }, {
                transactionCompleteCallback: transactionCompleteCallbackImpl_Claim
            });
        }
        function doCancelHealthPointClaim(providerId, serviceType, claimItemsCount, totalClaimAmount, claimItems, refTag) {

            iclient.cancelHealthPointClaim({
                providerId : providerId,
                serviceType : serviceType,
                claimItemsCount : claimItemsCount,
                totalClaimAmount : totalClaimAmount,
                claimItems : claimItems,  // JSON array
                refTag : refTag
            }, {
                transactionCompleteCallback: transactionCompleteCallbackImpl_Cancel
            });
        }


        function formatResult(response) {

            var result = "<table style=\"text-align:left;\" class=\"block_center left-right-padded-3px\">";

            result += "<tr><td>Result</td><td>" + response.result + "</td></tr>";

            if (response.healthpointRefTag)
                result += "<tr><td>healthpointRefTag</td><td>" + response.healthpointRefTag + "</td></tr>";
            if (response.healthpointErrorCode)
                result += "<tr><td>healthpointErrorCode</td><td>" + response.healthpointErrorCode + "</td></tr>";
            if (response.healthpointErrorDescription)
                result += "<tr><td>healthpointErrorDescription</td><td>" + response.healthpointErrorDescription + "</td></tr>";
            if (response.healthpointTotalBenefitAmount)
                result += "<tr><td>healthpointTotalBenefitAmount</td><td>" + response.healthpointTotalBenefitAmount + "</td></tr>";
            if (response.healthpointSettlementDateTime)
                result += "<tr><td>healthpointSettlementDateTime</td><td>" + response.healthpointSettlementDateTime + "</td></tr>";
            if (response.healthpointTerminalDateTime)
                result += "<tr><td>healthpointTerminalDateTime</td><td>" + response.healthpointTerminalDateTime + "</td></tr>";
            if (response.healthpointMemberNumber)
                result += "<tr><td>healthpointMemberNumber</td><td>" + response.healthpointMemberNumber + "</td></tr>";
            if (response.healthpointProviderId)
                result += "<tr><td>healthpointProviderId</td><td>" + response.healthpointProviderId + "</td></tr>";
            if (response.healthpointServiceType)
                result += "<tr><td>healthpointServiceType</td><td>" + response.healthpointServiceType + "</td></tr>";
            if (response.healthpointGapAmount)
                result += "<tr><td>healthpointGapAmount</td><td>" + response.healthpointGapAmount + "</td></tr>";
            if (response.healthpointPhfResponseCode)
                result += "<tr><td>healthpointPhfResponseCode</td><td>" + response.healthpointPhfResponseCode + "</td></tr>";
            if (response.healthpointPhfResponseCodeDescription)
                result += "<tr><td>healthpointPhfResponseCodeDescription</td><td>" + response.healthpointPhfResponseCodeDescription + "</td></tr>";

            result += "</table>"

            if (response.healthpointClaimItems) {
                var returnItems = "<br/><table style=\"text-align:left;\" class=\"block_center left-right-padded-3px\">";

                returnItems += "<tr>";
                returnItems += "<th>claimAmount</th>";
                returnItems += "<th>rebateAmount</th>";
                returnItems += "<th>serviceCode</th>";
                returnItems += "<th>description</th>";
                returnItems += "<th>serviceReference</th>";
                returnItems += "<th>patientId</th>";
                returnItems += "<th>serviceDate</th>";
                returnItems += "<th>responseCodeString</th>";
                returnItems += "</tr>";

                for (var i = 0; i < response.healthpointClaimItems.length; i++) {
                    returnItems += "<tr>";
                    returnItems += "<td>" + response.healthpointClaimItems[i].claimAmount + "</td>";
                    returnItems += "<td>" + response.healthpointClaimItems[i].rebateAmount + "</td>";
                    returnItems += "<td>" + response.healthpointClaimItems[i].serviceCode + "</td>";
                    returnItems += "<td>" + response.healthpointClaimItems[i].description + "</td>";
                    returnItems += "<td>" + response.healthpointClaimItems[i].serviceReference + "</td>";
                    returnItems += "<td>" + response.healthpointClaimItems[i].patientId + "</td>";
                    returnItems += "<td>" + response.healthpointClaimItems[i].serviceDate + "</td>";
                    returnItems += "<td>" + response.healthpointClaimItems[i].responseCode + "</td>";
                    returnItems += "</tr>";
                }
                returnItems += "</table>";
                result += returnItems;
            }

            return "<center><div>" + result + "</div><center>";
        }
        function saveResult(response) {

            var result = '<?xml version="1.0" encoding="UTF-8"?>\r\n';

            var detailFields = '';
            detailFields += ' tyro_transaction_id="' + tyro_transaction_id + '"';
            detailFields += ' result="' + response.result + '"';
            detailFields += ' healthpointErrorCode="' + response.healthpointErrorCode + '"';
            detailFields += ' healthpointErrorDescription="' + response.healthpointErrorDescription + '"';
            detailFields += ' healthpointRefTag="' + response.healthpointRefTag + '"';
            detailFields += ' healthpointTotalBenefitAmount="' + response.healthpointTotalBenefitAmount + '"';
            detailFields += ' healthpointSettlementDateTime="' + response.healthpointSettlementDateTime + '"';
            detailFields += ' healthpointTerminalDateTime="' + response.healthpointTerminalDateTime + '"';
            detailFields += ' healthpointMemberNumber="' + response.healthpointMemberNumber + '"';
            detailFields += ' healthpointProviderId="' + response.healthpointProviderId + '"';
            detailFields += ' healthpointServiceType="' + response.healthpointServiceType + '"';
            detailFields += ' healthpointGapAmount="' + response.healthpointGapAmount + '"';
            detailFields += ' healthpointPhfResponseCode="' + response.healthpointPhfResponseCode + '"';
            detailFields += ' healthpointPhfResponseCodeDescription="' + response.healthpointPhfResponseCodeDescription + '"';

            result += '<detail' + detailFields + ' >\r\n';

            if (response.healthpointClaimItems) {
                for (var i = 0; i < response.healthpointClaimItems.length; i++) {

                    var claimItemFields = '';
                    claimItemFields += ' claimAmount="' + response.healthpointClaimItems[i].claimAmount + '"';
                    claimItemFields += ' rebateAmount="' + response.healthpointClaimItems[i].rebateAmount + '"';
                    claimItemFields += ' serviceCode="' + response.healthpointClaimItems[i].serviceCode + '"';
                    claimItemFields += ' description="' + response.healthpointClaimItems[i].description + '"';
                    claimItemFields += ' serviceReference="' + response.healthpointClaimItems[i].serviceReference + '"';
                    claimItemFields += ' patientId="' + response.healthpointClaimItems[i].patientId + '"';
                    claimItemFields += ' serviceDate="' + response.healthpointClaimItems[i].serviceDate + '"';
                    claimItemFields += ' responseCode="' + response.healthpointClaimItems[i].responseCode + '"';
                    result += '<claimItem' + claimItemFields + ' />\r\n';
                }
            }

            result += '</detail>\r\n';

            return result;
        }
        function formatReceipt(text) {
            return "<pre>" + text + "</pre>";
        }

        /* ---------------- START - Save To DB ---------------- */

        function ajax_tyro_healthpoint_insert(invoice, amount) {

            var url_params = new Array(); // make sure index is +1 for each url_parameter
            url_params[0] = create_url_param("type", "insert_healthpoint");
            url_params[1] = create_url_param("invoice_id", invoice);
            url_params[2] = create_url_param("amount", amount);

            var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
            xmlhttp.open("GET", "/AJAX/AjaxTyro.aspx?" + create_url_params(url_params), false);
            xmlhttp.send();

            var response = String(xmlhttp.responseText);

            if (is_debugging)
                document.getElementById('lblErrorMessage').innerHTML += (document.getElementById('lblErrorMessage').innerHTML.length == 0 ? "" : "<br />") + "Insert: " + response;

            var successfully_inserted = !response.startsWith("SessionTimedOutException") && !response.startsWith("Exception");
            if (!successfully_inserted) {
                document.getElementById('lblErrorMessage').innerHTML = "Insert Failed: <br />" + response;
                return null;
            }

            return response;
        }

        function ajax_tyro_healthpoint_can_cancel(reftag) {

            var url_params = new Array(); // make sure index is +1 for each url_parameter
            url_params[0] = create_url_param("type", "can_cancel_healthpoint");
            url_params[1] = create_url_param("reftag", reftag);

            var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
            xmlhttp.open("GET", "/AJAX/AjaxTyro.aspx?" + create_url_params(url_params), false);
            xmlhttp.send();

            var response = String(xmlhttp.responseText);

            if (is_debugging)
                document.getElementById('lblErrorMessage').innerHTML += (document.getElementById('lblErrorMessage').innerHTML.length == 0 ? "" : "<br />") + "Response: " + response;

            return response;
        }

        /* ----------------- END - Save To DB ----------------- */

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
        function create_url_params(list) {
            var url = "";
            for (var i = 0; i < list.length; i++) {
                url = url + "&" + list[i].n + "=" + list[i].v;
            }
            return (url.length > 0) ? url.substr(1, url.length - 1) : url;
        }
        function create_url_param(n, v) {
            var item = new Array();
            item.n = n;
            item.v = v;
            return item;
        }

        function is_valid_amount(n) {
            return !isNaN(parseFloat(n)) && isFinite(n);
        }

        if (typeof String.prototype.startsWith != 'function') {
            String.prototype.startsWith = function (str) {
                return this.indexOf(str) === 0;
            };
        }

        function get_ddl_value(ddlID) {
            var e = document.getElementById(ddlID);
            return e.options[e.selectedIndex].value
        }
        function set_ddl_value(ddlID, val) {
            var e = document.getElementById(ddlID);
            for (var i = 0; i < e.options.length; i++)
                if (e.options[i].value == val)
                    e.selectedIndex = i;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Tyro HealthPoint Payment</span></div>
        <div class="main_content" id="main_content" runat="server">

            <div id="header_info_table" runat="server" class="user_login_form_no_width" style="width:750px;">

                <div class="border_top_bottom" style="margin-top:0;">
                    <div style="height:4px;"></div>
                    <table class="block_center" style="text-align:left;">
                        <tr>
                            <td class="nowrap">Invoice</td>
                            <td style="width:10px;"></td>
                            <td class="nowrap"><asp:Label ID="lblInvoiceID" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="nowrap">Total</td>
                            <td style="width:10px;"></td>
                            <td class="nowrap"><asp:Label ID="lblInvoiceTotal" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="nowrap">Receipted</td>
                            <td style="width:10px;"></td>
                            <td class="nowrap"><asp:Label ID="lblReceiptedTotal" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="nowrap">Owing</td>
                            <td style="width:10px;"></td>
                            <td class="nowrap"><asp:Label ID="lblInvoiceOwing" runat="server" Font-Bold="true" /></td>
                        </tr>
                        <tr>
                            <td class="nowrap">Debtor</td>
                            <td></td>
                            <td class="nowrap"><asp:Label ID="lblDebtor" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="nowrap">Booking Date</td>
                            <td></td>
                            <td class="nowrap"><asp:Label ID="lblBkDate" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="nowrap"><asp:Label ID="lblBkOrgText" runat="server">Booking At</asp:Label></td>
                            <td></td>
                            <td class="nowrap"><asp:Label ID="lblBkOrg" runat="server" /></td>
                        </tr>
                    </table>
                    <div style="height:4px;"></div>
                </div>

            </div>

            <div id="pairing_link" runat="server" class="text-center">
                <a href="https://iclientsimulator.test.tyro.com/configuration.html" onclick="open_new_tab('https://iclientsimulator.test.tyro.com/configuration.html');return false;">Pair virtual terminal with browser</a>
            </div>

            <div style="height:8px;"></div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <asp:HiddenField ID="hiddenResponse" runat="server" />
            <asp:Button ID="btnSaveResult" runat="server" OnClick="btnSaveResult_Click" Text="Update" CssClass="hiddencol"  />
            <asp:Button ID="btnSaveCancellation" runat="server" OnClick="btnSaveCancellation_Click" Text="Update" CssClass="hiddencol"  />


            <div style="height:8px;"></div>
            <table id="main_table" runat="server" class="block_center">
                <tr>
                    <td>
                        <center>

                            <table class="block_center">
                                <tr style="height:25px;">
                                    <td>
                                        <div id="result" style="color:blue;text-align:center;" runat="server">
                                            <asp:label ID="lblResult" runat="server" />
                                            <div style="height:8px;"></div>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblXML" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">

                                        Patient ID
                                        <asp:DropDownList ID="ddlPtId_Digit1" runat="server">
                                            <asp:ListItem Text="-" Value="-"></asp:ListItem>
                                            <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                            <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                            <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                            <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                            <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                        </asp:DropDownList><asp:DropDownList ID="ddlPtId_Digit2" runat="server">
                                            <asp:ListItem Text="-" Value="-"></asp:ListItem>
                                            <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                            <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                            <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                            <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                            <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                        </asp:DropDownList>
 
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="height:4px;"></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <button id="btnInitiateHealthPointClaim" style="width:100px;" type="button" class="mt10 btn btn-primary" onclick="initiateHealthPointClaim()" runat="server">Claim</button>
                                        <button id="btnCancelHealthPointClaim"   style="width:100px;" type="button" class="mt10 btn btn-primary" onclick="cancelHealthPointClaim()"   runat="server" visible="false">Cancel Claim</button>
                                    </td>
                                </tr>
                            </table>

                        </center>

                    </td>
                </tr>
            </table>


            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>



