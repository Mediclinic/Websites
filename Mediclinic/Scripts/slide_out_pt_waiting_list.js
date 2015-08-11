/*
--update Booking set date_start = '2013-08-17 18:00' , date_end = '2013-08-17 18:20' where booking_id = 89283
update Booking set arrival_time = GETDATE() where booking_id = 89283

select * from booking where date_start >= '2013-08-17 18:00' and date_start <= '2013-08-17 22:00'
*/


//
// from http://www.building58.com/examples/tabSlideOut.html
//
$(function () {
    $('.slide-out-div').tabSlideOut({
        tabHandle: '.handle',                                   //class of the element that will be your tab
        pathToTabImage: 'images/patients-waiting-sidebar.gif',  //path to the image for the tab (optionaly can be set using css)
        imageHeight: '138px',                                   //height of tab image
        imageWidth: '40px',                                     //width of tab image    
        tabLocation: 'right',                                   //side of screen where tab lives, top, right, bottom, or left
        speed: 300,                                             //speed of animation
        action: 'click',                                        //options: 'click' or 'hover', action to trigger animation
        topPos: '135px',                                        //position from the top
        fixedPosition: false                                    //options: true makes it stick(fixed position) on scroll
    });
});


function ajax_update_waiting_patient_list() {

    if (document.getElementById('hidden_is_provider_login').value != "1")
        return;

    var url_params = new Array();
    url_params[0] = create_url_param("staff", document.getElementById('hidden_staff_id').value);
    url_params[1] = create_url_param("org", document.getElementById('hidden_org_id').value);

    var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {

            var result = xmlhttp.responseText;
            var pos_seperator = result.indexOf(':');
            var count = parseInt(result.substring(0, pos_seperator));
            var text = result.substring(pos_seperator + 1);

            document.getElementById("div_patient_wait_list_table").innerHTML = text;
            document.getElementById("slide_out_div").style.display = count > 0 ? "block" : "none";
        }
    }

    xmlhttp.open("GET", "/AJAX/AjaxGetWaitingPatientList.aspx?" + create_url_params(url_params), true);
    xmlhttp.send();
}

function ajax_unset_arrival_time(booking_id) {

    var url_params = new Array();
    url_params[0] = create_url_param("type", "unarrived");
    url_params[1] = create_url_param("booking_id", booking_id);

    var xmlhttp = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            var result = xmlhttp.responseText;
            ajax_update_waiting_patient_list();
        }
    }

    xmlhttp.open("GET", "/AJAX/AjaxDoBooking.aspx?" + create_url_params(url_params), true);
    xmlhttp.send();
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

