
/* hover methods - called directly from the html page */

function hide_patient_detail(d) {
    if (d.length < 1)
        return;

    document.getElementById(d).style.display = "none";
}
function show_patient_detail(d, patient_id) {
    if (d.length < 1)
        return;

    var dd = document.getElementById(d);
    var person_hash = ajax_get_details_of_person("patient", patient_id);
    dd.innerHTML = create_person_table_to_display(person_hash);
    dd.setAttribute("style", "display: block; font-weight: bold; font-size: 13px; font-family: Arial, sans-serif; background-color: #ffffe8; text-align: center; padding: 2px; border: solid 1px; position: absolute;");
}
function hide_register_referrer_detail(d) {
    if (d.length < 1)
        return;

    document.getElementById(d).style.display = "none";
}
function show_register_referrer_detail(d, register_referrer_id) {
    if (d.length < 1)
        return;

    var dd = document.getElementById(d);
    var person_hash = ajax_get_details_of_person("register_referrer", register_referrer_id);
    dd.innerHTML = create_person_table_to_display(person_hash);
    dd.setAttribute("style", "display: block; font-weight: bold; font-size: 13px; font-family: Arial, sans-serif; background-color: #ffffe8; text-align: center; padding: 2px; border: solid 1px; position: absolute;");
}

/* private methods */


function ajax_get_details_of_person(type, id) {
    var url_params = new Array();
    url_params[0] = create_url_param("type", type);
    url_params[1] = create_url_param("id", id);

    var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
    xmlhttp.open("GET", "/AJAX/AjaxGetDetailsOfPerson.aspx?" + create_url_params(url_params), false);
    xmlhttp.send();

    var result = String(xmlhttp.responseText);
    if (result == "SessionTimedOutException")
        window.location.href = window.location.href;  // reload page
    var items = create_person_details_hashtable(result);
    return items;
}
function create_person_details_hashtable(result) {

    var items_hash = {}; // New object

    tmp_list = result.split("<>");
    for (var i = 0; i < tmp_list.length; i++) {
        item = tmp_list[i].split("|");
        item_name = item[0];
        item_value = item[1];
        items_hash[item_name] = item_value;
    }
    return items_hash;
}

function create_person_table_to_display(person_hash) {
    var text = "<table>";
    for (var k in person_hash) {
        // use hasOwnProperty to filter out keys from the Object.prototype
        if (person_hash.hasOwnProperty(k))
            if (person_hash[k].length > 0)
                text += "<tr><td class=\"nowrap\" align=\"left\" valign=\"top\">" + k + "</td width=\"8\"><td></td><td class=\"nowrap\" align=\"left\" valign=\"top\">" + person_hash[k].replace("\n", "<br />"); + "</td></tr>";
    }
    text += "</table>"
    return text;
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
