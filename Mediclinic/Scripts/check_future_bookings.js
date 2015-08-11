function ajax_future_bookings(type, staff_id, weekday, start_time, end_time, outside) {

    var url_params = new Array();
    url_params[0] = create_url_param("type", type);
    url_params[1] = create_url_param("staff", staff_id);
    url_params[2] = create_url_param("weekday", weekday);
    url_params[3] = create_url_param("start_time", start_time);
    url_params[4] = create_url_param("end_time", end_time);
    url_params[5] = create_url_param("outside", outside);

    var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
    xmlhttp.open("GET", "/AJAX/AjaxBookingCheckFutureBookings.aspx?" + create_url_params(url_params), false);
    xmlhttp.send();

    var response = String(xmlhttp.responseText);
    if (response == "SessionTimedOutException")
        window.location.href = window.location.href;  // reload page

    return response;
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
function get_ddl_value(ddlName) {
    var e = document.getElementById(ddlName);
    var val = String(e.options[e.selectedIndex].value);
    if (val.length < 2)
        val = '0' + val;

    return val;
}