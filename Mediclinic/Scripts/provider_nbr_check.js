function provider_check(obj) {

    var prov_nbr = obj.value;
    if (String(prov_nbr).length == 0)
        return;

    var nbr_exists = ajax_provider_nbr_exists(String(prov_nbr));
    if (nbr_exists) {
        var result = confirm("Provider number already in use. Would you like to edit it?");
        if (result)
            setTimeout(function () { obj.focus(); obj.value = obj.value; }, 300);
        else {
            setTimeout(function () {
                var target = obj.explicitOriginalTarget || document.activeElement;
                target.focus();
                var pos = doGetCaretPosition(target);
                if (pos > 0)
                    setCaretPosition(target, pos);
            }, 1);

        }
    }
}
function ajax_provider_nbr_exists(provider_nrb) {

    var url_params = new Array();
    url_params[0] = create_url_param("provider_nrb", provider_nrb);

    var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
    xmlhttp.open("GET", "/AJAX/AjaxCheckProviderNbrExists.aspx?" + create_url_params(url_params), false);
    xmlhttp.send();

    var result = String(xmlhttp.responseText);
    if (result == "SessionTimedOutException")
        window.location.href = window.location.href;  // reload page
    if (result == "1")
        return true;
    else if (result == "0")
        return false;
    else {
        alert(result);
        return true;
    }
}

function doGetCaretPosition(ctrl) {
    var CaretPos = 0;   // IE Support
    if (document.selection) {
        ctrl.focus();
        var Sel = document.selection.createRange();
        Sel.moveStart('character', -ctrl.value.length);
        CaretPos = Sel.text.length;
    }
    // Firefox support
    else if (ctrl.selectionStart || ctrl.selectionStart == '0')
        CaretPos = ctrl.selectionStart;
    return (CaretPos);
}
function setCaretPosition(ctrl, pos) {
    if (ctrl.setSelectionRange) {
        ctrl.focus();
        ctrl.setSelectionRange(pos, pos);
    }
    else if (ctrl.createTextRange) {
        var range = ctrl.createTextRange();
        range.collapse(true);
        range.moveEnd('character', pos);
        range.moveStart('character', pos);
        range.select();
    }
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