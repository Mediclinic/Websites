<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="TyroPaymentV2.aspx.cs" Inherits="TyroPaymentV2" validateRequest="false" %>

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

        // before a transaction, pending transaction inserted into db and this is set to be used for 
        // the callback post transaction to update the db
        var tyro_transaction_id = null;

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
        function purchase() {
            document.getElementById('lblErrorMessage').innerHTML = '';
            if (ajax_check_session_timed_out()) {
                alert("Session timed out. You will be redirected to the login page to login again to complete this transaction.");
                window.location.href = window.location.href;  // reload page so they have to login again
                return;
            }
            if (!is_valid_amount($("#amount").val())) {
                document.getElementById('lblErrorMessage').innerHTML = "Payment amount must be a valid amount.";
                return;
            }
            if ($("#cashout").val().length > 0 && !is_valid_amount($("#cashout").val())) {
                document.getElementById('lblErrorMessage').innerHTML = "Cashout must be a valid amount.";
                return;
            }

            var totalOwingCents = parseInt(document.getElementById('hiddenInvoiceOwingTotalCents').value);
            var amount = $("#amount").val().length == 0 ? "0" : String(Math.round(100 * parseFloat(($("#amount").val()).replace(/[$,]/g, ''))));
            var cashout = $("#cashout").val().length == 0 ? "0" : String(Math.round(100 * parseFloat(($("#cashout").val()).replace(/[$,]/g, ''))));

            if (parseInt(amount) > totalOwingCents) {
                document.getElementById('lblErrorMessage').innerHTML = "Payment amount can not be more than the due amount of " + document.getElementById('lblInvoiceOwing').innerHTML + ".";
                return;
            }

            // set page global variable for callback to have access to and save completed transaction info
            tyro_transaction_id = ajax_tyro_insert(getUrlVars()['invoice'], "1", String(amount), String(cashout));

            if (tyro_transaction_id == null) {
                alert("Error: tyro_transaction_id is null");
                return
            }

            if (tyro_transaction_id.startsWith('OVERPAYMENT:')) {
                var overPaymentAmount = tyro_transaction_id.substr(12);
                document.getElementById('lblErrorMessage').innerHTML = 'Payment is over the due amount of ' + overPaymentAmount + '. <a href=\"#\" onclick=\"window.location.href=window.location.href;return false;\"><font color=\"blue\">Refresh</font></a> the page to update the due amount.';
                return
            }

            reset();
            doPurchase(amount, cashout, tyro_transaction_id);
        }

        function refund() {
            document.getElementById('lblErrorMessage').innerHTML = '';
            if (ajax_check_session_timed_out()) {
                alert("Session timed out. You will be redirected to the login page to login again to complete this transaction.");
                window.location.href = window.location.href;  // reload page so they have to login again
                return;
            }
            if (!is_valid_amount($("#amount").val())) {
                document.getElementById('lblErrorMessage').innerHTML = "Refund amount must be a valid amount.";
                return;
            }
            
            var totalReceiptedCents = parseInt(document.getElementById('hiddenReceiptedAmountTotalCents').value);
            var amount = String(Math.round(100 * parseFloat(($("#amount").val()).replace(/[$,]/g, ''))));

            if (parseInt(amount) > totalReceiptedCents) {
                document.getElementById('lblErrorMessage').innerHTML = "Refund amount can not be more than the receipted amount of " + document.getElementById('lblReceiptedTotal').innerHTML + ".";
                return;
            }

            // set page global variable for callback to have access to and save completed transaction info
            tyro_transaction_id = ajax_tyro_insert(getUrlVars()['invoice'], "2", String(amount), "0");
            if (tyro_transaction_id == null) {
                alert("Error: tyro_transaction_id is null");
                return
            }

            if (tyro_transaction_id.startsWith('OVERREFUND:')) {
                var overRefundAmount = tyro_transaction_id.substr(11);
                document.getElementById('lblErrorMessage').innerHTML = 'Refund Payment is over the total receipted amount of ' + overRefundAmount + '. <a href=\"#\" onclick=\"window.location.href=window.location.href;return false;\"><font color=\"blue\">Refresh</font></a> the page to update the receipt total.';
                return
            }

            reset();
            doRefund(amount, tyro_transaction_id);
        }

        function reset() {
            document.getElementById('hiddenMerchangeReceipt').value = ''
            document.getElementById('hiddenCustomerReceipt').value = '';
            document.getElementById('hiddenResponse').value = ''
            document.getElementById('btnPrintMerchantReceipt').style.display = 'none';
            document.getElementById('btnPrintCustomerReceipt').style.display = 'none';

            $("#merchantReceipt").html("Merchant Receipt");
            $("#customerReceipt").html("Customer Receipt");
            $("#result").html("Result");
        }
        var receiptCallbackImpl = function (receipt) {
            $("#merchantReceipt").html(formatReceipt(receipt.merchantReceipt));
            document.getElementById('hiddenMerchangeReceipt').value = receipt.merchantReceipt;
            update_to_max_height(); // update autodiv
        };
        var transactionCompleteCallbackImpl = function (response) {
            if (response.customerReceipt) {
                $("#customerReceipt").html(formatReceipt(response.customerReceipt));
                document.getElementById('hiddenCustomerReceipt').value = response.customerReceipt;
                update_to_max_height(); // update autodiv
            }
            $("#result").html(formatResult(response));
            document.getElementById('hiddenResponse').value = formatResult(response);

            // use page global variable in callback to save completed transaction info
            ajax_tyro_update(tyro_transaction_id, response.result, response.cardType, response.transactionReference, response.authorisationCode, response.issuerActionCode);
            tyro_transaction_id = null;  // unset it

            document.getElementById('btnUpdateInvoiceInfo').click();
        };

        function doPurchase(amount, cashout, transactionId) {
            iclient.initiatePurchase({
                amount: amount,
                cashout: cashout,
                integratedReceipt: true,
                transactionId: transactionId
            }, {
                receiptCallback: receiptCallbackImpl,
                transactionCompleteCallback: transactionCompleteCallbackImpl
            });
        }

        function doRefund(amount, transactionId) {
            iclient.initiateRefund({
                amount: amount,
                integratedReceipt: true,
                transactionId: transactionId
            }, {
                receiptCallback: receiptCallbackImpl,
                transactionCompleteCallback: transactionCompleteCallbackImpl
            });
        }

        function formatReceipt(text) {
            return "<pre>" + text + "</pre>";
        }

        function formatResult(response) {
            var result = "<div>Result: " + response.result + "</div>";
            /*
            if (response.cardType)
                result += "<div>Card type: " + response.cardType + "</div>";
            if (response.transactionReference)
                result += "<div>Trans ref: " + response.transactionReference + "</div>";
            if (response.authorisationCode)
                result += "<div>Auth code: " + response.authorisationCode + "</div>";
            if (response.issuerActionCode)
                result += "<div>Action code: " + response.issuerActionCode + "</div>";
            */
            return result;
        }



        /* ---------------- START - Save To DB ---------------- */

        function ajax_tyro_insert(invoice, tyro_payment_type, amount, cashout) {

            var url_params = new Array(); // make sure index is +1 for each url_parameter
            url_params[0] = create_url_param("type", "insert");
            url_params[1] = create_url_param("invoice_id", invoice);
            url_params[2] = create_url_param("tyro_payment_type_id", tyro_payment_type);
            url_params[3] = create_url_param("amount", amount);
            url_params[4] = create_url_param("cashout", cashout);

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
        function ajax_tyro_update(tyro_transaction_id, result, cardType, transactionReference, authorisationCode, issuerActionCode) {

            var url_params = new Array(); // make sure index is +1 for each url_parameter
            url_params[0] = create_url_param("type", "update");
            url_params[1] = create_url_param("tyro_transaction_id", tyro_transaction_id);
            url_params[2] = create_url_param("out_result", result);
            url_params[3] = create_url_param("out_cardType", cardType ? cardType : '');
            url_params[4] = create_url_param("out_transactionReference", transactionReference ? transactionReference : '');
            url_params[5] = create_url_param("out_authorisationCode", authorisationCode ? authorisationCode : '');
            url_params[6] = create_url_param("out_issuerActionCode", issuerActionCode ? issuerActionCode : '');

            var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
            xmlhttp.open("GET", "/AJAX/AjaxTyro.aspx?" + create_url_params(url_params), false);
            xmlhttp.send();

            var response = String(xmlhttp.responseText);

            if (is_debugging)
                document.getElementById('lblErrorMessage').innerHTML += "&nbsp;&nbsp;&nbsp;&nbsp;" + "Update: " + response;

            if (response != "1")
                document.getElementById('lblErrorMessage').innerHTML = "Update Failed: <br />" + response;
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

        function set_type() {
            var ddlType = document.getElementById('<%= ddlType.ClientID %>');
            var SelectedValue = ddlType.value;
            document.getElementById('row_cashout').style.visibility = SelectedValue == "1" ? "" : "hidden";
            document.getElementById('btnPurchase').style.display = SelectedValue == "1" ? "" : "none";
            document.getElementById('btnRefund').style.display = SelectedValue == "2" ? "" : "none";

            if (SelectedValue != "1")
                document.getElementById('cashout').value = '';

            update_to_max_height(); // update autodiv
        }




    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Tyro Payment</span></div>
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
                            <td class="nowrap">
                                <asp:Label ID="lblReceiptedTotal" runat="server" />
                                <asp:HiddenField ID="hiddenReceiptedAmountTotalCents" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="nowrap">Owing</td>
                            <td style="width:10px;"></td>
                            <td class="nowrap">
                                <asp:Label ID="lblInvoiceOwing" runat="server" Font-Bold="true" />
                                <asp:HiddenField ID="hiddenInvoiceOwingTotalCents" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="nowrap">Debtor</td>
                            <td></td>
                            <td class="nowrap"><asp:Label ID="lblDebtor" runat="server" /></td>
                        </tr>
                        <tr id="td_bk_date" runat="server">
                            <td class="nowrap">Booking Date</td>
                            <td></td>
                            <td class="nowrap"><asp:Label ID="lblBkDate" runat="server" /></td>
                        </tr>
                        <tr id="td_bk_org" runat="server">
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
                <br />
                <a href="javascript:void(0)"  onclick="show_hide('transaction_list');return false;">Show/Hide Transaction Test List</a>
            </div>


            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <asp:HiddenField ID="hiddenCustomerReceipt" runat="server" />
            <asp:HiddenField ID="hiddenMerchangeReceipt" runat="server" />
            <asp:HiddenField ID="hiddenResponse" runat="server" />
            <asp:Button ID="btnUpdateInvoiceInfo" runat="server" OnClick="btnUpdateInvoiceInfo_Click" Text="Update" CssClass="hiddencol"  />

            <table id="main_table" runat="server" class="block_center">
                <tr>
                    <td>
                        <center>

                            <div id="transaction_list" style="display:none;">

                                <select class="form-control-tyro" id="amount2">
                                    <option value="10000">$100.00 (Approved; swiped + cheque + PIN; EFTPOS card type)</option>
                                    <option value="10001">$100.01 (Declined; swiped + cheque + PIN; EFTPOS card type)</option>
                                    <option value="10100">$101.00 (Approved; swiped + savings + PIN; EFTPOS card type)</option>
                                    <option value="10101">$101.01 (Declined; swiped + savings + PIN; EFTPOS card type)</option>
                                    <option value="10200">$102.00 (Approved; swiped + credit + PIN; Visa card type)</option>
                                    <option value="10201">$102.01 (Declined; swiped + credit + PIN; Visa card type)</option>
                                    <option value="10208">$102.08 (Approved; swiped + credit + signature; Visa card type)</option>
                                    <option value="10300">$103.00 (Approved; swiped + PIN; Mastercard card type)</option>
                                    <option value="10301">$103.01 (Declined; swiped + PIN; Mastercard card type)</option>
                                    <option value="10308">$103.08 (Approved; swiped + signature; Mastercard card type)</option>
                                    <option value="10400">$104.00 (Approved; swiped + PIN; AMEX card type)</option>
                                    <option value="10401">$104.01 (Declined; swiped + PIN; AMEX card type)</option>
                                    <option value="10408">$104.08 (Approved; swiped + signature; AMEX card type)</option>
                                </select>

                            </div>

                            <br />

                            <table>
                                <tr style="height:25px;">
                                    <td>
                                        <div id="result" style="font-weight:bold;color:blue;text-align:center;" runat="server"></div>
                                    </td>
                                </tr>
                                <tr style="vertical-align:top;">
                                    <td style="padding:0 20px;">

                                        <table>
                                            <tr>
                                                <td>Transaction Type</td>
                                                <td style="min-width:10px;"></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlType" runat="server" onchange="set_type();" onblur="set_type()" style="width:100%;">
                                                        <asp:ListItem Text="Prchase" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="Refund" Value="2"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3"><div style="height:8px;"></div></td>
                                            </tr>
                                            <tr>
                                                <td><label for="amount" style="white-space:nowrap;">Amount:</label></td>
                                                <td></td>
                                                <td><input id="amount" type="text" placeholder="Amount" name="amount" value="100.00" onkeydown="if (event.keyCode == 13) {return false;}"></td>
                                            </tr>
                                            <tr>
                                                <td colspan="3"><div style="height:8px;"></div></td>
                                            </tr>
                                            <tr id="row_cashout">
                                                <td><label for="cashout" style="white-space:nowrap;">Cash out:</label></td>
                                                <td></td>
                                                <td><input id="cashout" type="text" placeholder="Cash out" name="cashout" onkeydown="if (event.keyCode == 13) {return false; }"></td>
                                            </tr>
                                            <tr>
                                                <td colspan="3"><div style="height:8px;"></div></td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="text-align:center;">
                                                    <button id="btnPurchase" style="width:100px;" type="button" class="mt10 btn btn-primary" onclick="purchase()">Purchase</button>
                                                    <button id="btnRefund"   style="display:none;width:100px;" type="button" class="mt10 btn btn-primary" onclick="refund()">Refund</button>
                                                </td>
                                            </tr>
                                        </table>

                                    </td>
                                    <td>

                                        <table class="block_center">
                                            <tr style="text-align:center;vertical-align:top;">
                                                <td style="width:50%;padding:0 10px;">
                                                    <div id="merchantReceipt" runat="server"></div>
                                                    <asp:Button ID="btnPrintMerchantReceipt" runat="server" OnClick="btnPrintMerchantReceipt_Click" Text="Print" style="display:none;" />
                                                </td>
                                                <td style="width:50%;padding:0 10px;">
                                                    <div id="customerReceipt" runat="server"></div>
                                                    <asp:Button ID="btnPrintCustomerReceipt" runat="server" OnClick="btnPrintCustomerReceipt_Click" Text="Print" style="display:none;" />
                                                </td>
                                            </tr>
                                        </table>

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



