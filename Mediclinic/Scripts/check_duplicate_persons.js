function ajax_duplicate_persons(type, firstname, surname) {

    var url_params = new Array();
    url_params[0] = create_url_param("type", type);
    url_params[1] = create_url_param("firstname", firstname);
    url_params[2] = create_url_param("surname", surname);

    var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
    xmlhttp.open("GET", "/AJAX/AjaxDuplicatePersonList.aspx?" + create_url_params(url_params), false);
    xmlhttp.send();

    var response = String(xmlhttp.responseText);
    if (response == "SessionTimedOutException")
        window.location.href = window.location.href;  // reload page

    return response;
}

function create_table(result_list, span_name, button_url, use_surname) {

    use_surname = typeof use_surname !== 'undefined' ? use_surname : false;


    // first remove the table so dont add more and more!
    var persons_table = document.getElementById("persons_table");
    if (persons_table != null)
        persons_table.parentNode.removeChild(persons_table);


    // now create the table
    var tb = document.createElement('table');
    tb.id = "persons_table";
    tb.width = "100%";
    tb.border = 1;
    tb.setAttribute("cellpadding", "2");
    tb.setAttribute("cellspacing", "0");

    var tbody = document.createElement('tbody');

    for (var row = 0; row < result_list.length; row++) {

        var tr = document.createElement( 'tr');

        if (row == 0)
            tr.setAttribute('style', 'font-weight:bold');

        for (var col = 0; col < result_list[row].length; col++) {

            var td = document.createElement('td');
            td.align = "center";

            var text = result_list[row][col];
            var lines = text.split("\r\n")  // split by newlines, and add 'br' element in its place
            for (var i = 0; i < lines.length; i++) {
                if (i > 0)
                    td.appendChild(document.createElement("BR"));
                td.appendChild(document.createTextNode(lines[i]));
            }

            tr.appendChild(td);
        }


        // create the last cell and add the button to it
        td = document.createElement('td');
        td.align = "center";
        if (row == 0)
            td.appendChild(document.createTextNode(" "));
        else {
            var el = document.createElement('input');
            el.type = 'button';
            el.name = 'text';
            el.value = "Select";
            el.onclick = (function (p) { // create closure to pass by val not ref
                return function () {
                    if (button_url.length > 0)
                        location.href = button_url + (use_surname ? p[3] : p[0]);
                    else
                        set_existing_person(p[0]);
                }
            } (result_list[row]));

            td.appendChild(el);
        }


        tr.appendChild(td);


        // add the row to the body of the table
        tbody.appendChild(tr);
    }

    tb.appendChild(tbody);
    document.getElementById(span_name).appendChild(tb);
}


function create_result_array(result) {
    result_list = new Array();
    tmp_list = result.split("<>");
    for (var i = 0; i < tmp_list.length; i++)
        result_list[i] = tmp_list[i].split("|");
    return result_list;
}

function reveal_modal(divID) {
    window.onscroll = function () { document.getElementById(divID).style.top = document.body.scrollTop; };
    document.getElementById(divID).style.display = "block";
    document.getElementById(divID).style.top = document.body.scrollTop;
}
function hide_modal(divID) {
    document.getElementById(divID).style.display = "none";
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