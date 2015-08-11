
function show_modal_updade_epc(health_card_id) {

    if (!ajax_has_active_referrer()) {
        alert('Please make sure you have set a reffering doctor before you add ' + (is_aged_care() ? 'an EPC' : 'a referral'));
        return;
    }

    // show modal popup
    var result = window.showModalDialog('AddEditEPC.aspx?type=add&id=' + health_card_id, '', 'dialogWidth:550px;dialogHeight:550px;center:yes;resizable:no; scroll:no');

    // popup download file window in case letter to print
    if (result == true)
        window.showModalDialog('DownloadFile.aspx', '', 'dialogWidth:10px;dialogHeight:10px;resizable:no; scroll:no');

    return false;
}


function ajax_has_active_referrer() {

    var url_id = getUrlVars()["id"];
    if (url_id == undefined) {
        alert("No url field 'id'");
        return false;
    }
    var url_type = getUrlVars()["type"];
    if (url_type == undefined) {
        alert("No url field 'type'");
        return false;
    }

    var url_id_type = "patient";

    var url_params = new Array();
    url_params[0] = create_url_param("id", url_id);
    url_params[1] = create_url_param("id_type", url_id_type);

    var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
    xmlhttp.open("GET", "/AJAX/AjaxHasActiveReferrer.aspx?" + create_url_params(url_params), false);
    xmlhttp.send();

    var response = String(xmlhttp.responseText);
    if (response == "SessionTimedOutException")
        window.location.href = window.location.href;  // reload page

    if (response != "0" && response != "1")
        alert("Error: \n\r" + response);
    return response != "0"; // if neither '1' or '0' will say already have other card and popup causing alert to be error
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

