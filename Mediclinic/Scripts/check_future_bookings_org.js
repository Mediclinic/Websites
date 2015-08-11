function validate_uncheck(c) {

    if (getUrlVars()["type"] == "add")
        return;

    if (c.checked)
        return;

    var weekday = String(c.id).replace("chkInc", "").toLowerCase();
    var id = getUrlVars()["id"];
    var booked_days = ajax_future_bookings("org", id, weekday, get_start_time(weekday), get_end_time(weekday), "0");
    if (booked_days == "NONE")
        return;

    var space = "          ";
    var booked_days_list = space + booked_days.replace(/,/gi, "\r\n" + space);

    alert("Can not de-select day until the bookings on the following days have been moved or deleted:\r\n\r\n" + booked_days_list);
    c.checked = true;
}

function validate_change_time(obj) {

    var type_to_check = obj.id.indexOf("Lunch") == -1 ? "day" : "lunch";

    var weekday = type_to_check == "day" ?
        to_long_name(String(obj.id).replace("ddl", "").replace("Start_Hour", "").replace("Start_Minute", "").replace("End_Hour", "").replace("End_Minute", "").toLowerCase())
         :
        to_long_name(String(obj.id).replace("ddl", "").replace("LunchStart_Hour", "").replace("LunchStart_Minute", "").replace("LunchEnd_Hour", "").replace("LunchEnd_Minute", "").toLowerCase());

    var is_start  = String(obj.id).indexOf("Start") != -1;
    var is_end    = String(obj.id).indexOf("End") != -1;
    var is_hour   = String(obj.id).indexOf("Hour") != -1;
    var is_minute = String(obj.id).indexOf("Minute") != -1;

    if (!is_checked(weekday)) {

        // set time in invis label
        set_time_original();
        return;
    }

    // check ajax here. if no future bookings, return
    var id = getUrlVars()["id"]; type_to_check
    var booked_days = ajax_future_bookings("org", id, weekday, get_start_time(weekday, type_to_check), get_end_time(weekday, type_to_check), type_to_check == "day" ? "1" : "0");
    if (booked_days == "NONE") {
        set_time_original();  // set invis label
        return;
    }

    // show msg
    var space = "          ";
    var booked_days_list = space + booked_days.replace(/,/gi, "\r\n" + space);
    //alert("Can not change time until the bookings on the following days have been moved or deleted:\r\n\r\n" + booked_days_list);


    result = confirm("These future bookings will no longer be in opening hours.\r\nClick Ok to continue or Cancel to reset time:\r\n\r\n" + booked_days_list);
    if (result)
        return;


    // reset time
    var to_reset = "";
    if (is_start)
        var to_reset = get_start_time_original(weekday, type_to_check);
    else if (is_end)
        var to_reset = get_end_time_original(weekday, type_to_check);
    else {
        alert("Error: not start or end time");
        return;
    }

    var reset = "";
    if (is_hour)
        reset = to_reset.substring(0, 2);
    else if (is_minute)
        reset = to_reset.substring(2, 4);
    else {
        alert("Error: not hour or minute ddl");
        return;
    }

    var selectEl = obj;
    var opts = selectEl.getElementsByTagName('option');
    for (var i = 0; i < opts.length; i++) {
        opts[i].selected = (String(opts[i].text) == String(reset) ? "selected" : "");
    }
}



function is_checked(weekday) {
    if (weekday == "sunday" || weekday == "sun")
        return document.getElementById("chkIncSunday").checked;
    if (weekday == "monday" || weekday == "mon")
        return document.getElementById("chkIncMonday").checked;
    if (weekday == "tuesday" || weekday == "tue")
        return document.getElementById("chkIncTuesday").checked;
    if (weekday == "wednesday" || weekday == "wed")
        return document.getElementById("chkIncWednesday").checked;
    if (weekday == "thursday" || weekday == "thu")
        return document.getElementById("chkIncThursday").checked;
    if (weekday == "friday" || weekday == "fri")
        return document.getElementById("chkIncFriday").checked;
    if (weekday == "saturday" || weekday == "sat")
        return document.getElementById("chkIncSaturday").checked;
}

function get_start_time(weekday, type_to_check) {

    var type = type_to_check == "day" ? "" : "Lunch";

    if (weekday == "sunday" || weekday == "sun")
        return get_ddl_value("ddlSun" + type + "Start_Hour") + get_ddl_value("ddlSun" + type + "Start_Minute");
    if (weekday == "monday" || weekday == "mon")
        return get_ddl_value("ddlMon" + type + "Start_Hour") + get_ddl_value("ddlMon" + type + "Start_Minute");
    if (weekday == "tuesday" || weekday == "tue")
        return get_ddl_value("ddlTue" + type + "Start_Hour") + get_ddl_value("ddlTue" + type + "Start_Minute");
    if (weekday == "wednesday" || weekday == "wed")
        return get_ddl_value("ddlWed" + type + "Start_Hour") + get_ddl_value("ddlWed" + type + "Start_Minute");
    if (weekday == "thursday" || weekday == "thu")
        return get_ddl_value("ddlThu" + type + "Start_Hour") + get_ddl_value("ddlThu" + type + "Start_Minute");
    if (weekday == "friday" || weekday == "fri")
        return get_ddl_value("ddlFri" + type + "Start_Hour") + get_ddl_value("ddlFri" + type + "Start_Minute");
    if (weekday == "saturday" || weekday == "sat")
        return get_ddl_value("ddlSat" + type + "Start_Hour") + get_ddl_value("ddlSat" + type + "Start_Minute");
}
function get_end_time(weekday, type_to_check) {

    var type = type_to_check == "day" ? "" : "Lunch";

    if (weekday == "sunday" || weekday == "sun")
        return get_ddl_value("ddlSun" + type + "End_Hour") + get_ddl_value("ddlSun" + type + "End_Minute");
    if (weekday == "monday" || weekday == "mon")
        return get_ddl_value("ddlMon" + type + "End_Hour") + get_ddl_value("ddlMon" + type + "End_Minute");
    if (weekday == "tuesday" || weekday == "tue")
        return get_ddl_value("ddlTue" + type + "End_Hour") + get_ddl_value("ddlTue" + type + "End_Minute");
    if (weekday == "wednesday" || weekday == "wed")
        return get_ddl_value("ddlWed" + type + "End_Hour") + get_ddl_value("ddlWed" + type + "End_Minute");
    if (weekday == "thursday" || weekday == "thu")
        return get_ddl_value("ddlThu" + type + "End_Hour") + get_ddl_value("ddlThu" + type + "End_Minute");
    if (weekday == "friday" || weekday == "fri")
        return get_ddl_value("ddlFri" + type + "End_Hour") + get_ddl_value("ddlFri" + type + "End_Minute");
    if (weekday == "saturday" || weekday == "sat")
        return get_ddl_value("ddlSat" + type + "End_Hour") + get_ddl_value("ddlSat" + type + "End_Minute");
}

function get_start_time_original(weekday, type_to_check) {

    var type = type_to_check == "day" ? "" : "Lunch";

    if (weekday == "sunday" || weekday == "sun")
        return document.getElementById("lblSun" + type + "Start_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblSun" + type + "Start_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "monday" || weekday == "mon")
        return document.getElementById("lblMon" + type + "Start_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblMon" + type + "Start_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "tuesday" || weekday == "tue")
        return document.getElementById("lblTue" + type + "Start_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblTue" + type + "Start_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "wednesday" || weekday == "wed")
        return document.getElementById("lblWed" + type + "Start_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblWed" + type + "Start_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "thursday" || weekday == "thu")
        return document.getElementById("lblThu" + type + "Start_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblThu" + type + "Start_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "friday" || weekday == "fri")
        return document.getElementById("lblFri" + type + "Start_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblFri" + type + "Start_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "saturday" || weekday == "sat")
        return document.getElementById("lblSat" + type + "Start_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblSat" + type + "Start_Minute").innerHTML.pad_left(2, "0");
}
function get_end_time_original(weekday, type_to_check) {

    var type = type_to_check == "day" ? "" : "Lunch";

    if (weekday == "sunday" || weekday == "sun")
        return document.getElementById("lblSun" + type + "End_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblSun" + type + "End_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "monday" || weekday == "mon")
        return document.getElementById("lblMon" + type + "End_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblMon" + type + "End_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "tuesday" || weekday == "tue")
        return document.getElementById("lblTue" + type + "End_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblTue" + type + "End_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "wednesday" || weekday == "wed")
        return document.getElementById("lblWed" + type + "End_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblWed" + type + "End_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "thursday" || weekday == "thu")
        return document.getElementById("lblThu" + type + "End_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblThu" + type + "End_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "friday" || weekday == "fri")
        return document.getElementById("lblFri" + type + "End_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblFri" + type + "End_Minute").innerHTML.pad_left(2, "0");
    if (weekday == "saturday" || weekday == "sat")
        return document.getElementById("lblSat" + type + "End_Hour").innerHTML.pad_left(2, "0") + document.getElementById("lblSat" + type + "End_Minute").innerHTML.pad_left(2, "0");
}

function set_time_original() {

    document.getElementById("lblSunStart_Hour").innerHTML = get_ddl_value("ddlSunStart_Hour");
    document.getElementById("lblSunStart_Minute").innerHTML = get_ddl_value("ddlSunStart_Minute");
    document.getElementById("lblSunEnd_Hour").innerHTML = get_ddl_value("ddlSunEnd_Hour");
    document.getElementById("lblSunEnd_Minute").innerHTML = get_ddl_value("ddlSunEnd_Minute");

    document.getElementById("lblSunLunchStart_Hour").innerHTML = get_ddl_value("ddlSunLunchStart_Hour");
    document.getElementById("lblSunLunchStart_Minute").innerHTML = get_ddl_value("ddlSunLunchStart_Minute");
    document.getElementById("lblSunLunchEnd_Hour").innerHTML = get_ddl_value("ddlSunLunchEnd_Hour");
    document.getElementById("lblSunLunchEnd_Minute").innerHTML = get_ddl_value("ddlSunLunchEnd_Minute");


    document.getElementById("lblMonStart_Hour").innerHTML = get_ddl_value("ddlMonStart_Hour");
    document.getElementById("lblMonStart_Minute").innerHTML = get_ddl_value("ddlMonStart_Minute");
    document.getElementById("lblMonEnd_Hour").innerHTML = get_ddl_value("ddlMonEnd_Hour");
    document.getElementById("lblMonEnd_Minute").innerHTML = get_ddl_value("ddlMonEnd_Minute");

    document.getElementById("lblMonLunchStart_Hour").innerHTML = get_ddl_value("ddlMonLunchStart_Hour");
    document.getElementById("lblMonLunchStart_Minute").innerHTML = get_ddl_value("ddlMonLunchStart_Minute");
    document.getElementById("lblMonLunchEnd_Hour").innerHTML = get_ddl_value("ddlMonLunchEnd_Hour");
    document.getElementById("lblMonLunchEnd_Minute").innerHTML = get_ddl_value("ddlMonLunchEnd_Minute");


    document.getElementById("lblTueStart_Hour").innerHTML = get_ddl_value("ddlTueStart_Hour");
    document.getElementById("lblTueStart_Minute").innerHTML = get_ddl_value("ddlTueStart_Minute");
    document.getElementById("lblTueEnd_Hour").innerHTML = get_ddl_value("ddlTueEnd_Hour");
    document.getElementById("lblTueEnd_Minute").innerHTML = get_ddl_value("ddlTueEnd_Minute");

    document.getElementById("lblTueLunchStart_Hour").innerHTML = get_ddl_value("ddlTueLunchStart_Hour");
    document.getElementById("lblTueLunchStart_Minute").innerHTML = get_ddl_value("ddlTueLunchStart_Minute");
    document.getElementById("lblTueLunchEnd_Hour").innerHTML = get_ddl_value("ddlTueLunchEnd_Hour");
    document.getElementById("lblTueLunchEnd_Minute").innerHTML = get_ddl_value("ddlTueLunchEnd_Minute");


    document.getElementById("lblWedStart_Hour").innerHTML = get_ddl_value("ddlWedStart_Hour");
    document.getElementById("lblWedStart_Minute").innerHTML = get_ddl_value("ddlWedStart_Minute");
    document.getElementById("lblWedEnd_Hour").innerHTML = get_ddl_value("ddlWedEnd_Hour");
    document.getElementById("lblWedEnd_Minute").innerHTML = get_ddl_value("ddlWedEnd_Minute");

    document.getElementById("lblWedLunchStart_Hour").innerHTML = get_ddl_value("ddlWedLunchStart_Hour");
    document.getElementById("lblWedLunchStart_Minute").innerHTML = get_ddl_value("ddlWedLunchStart_Minute");
    document.getElementById("lblWedLunchEnd_Hour").innerHTML = get_ddl_value("ddlWedLunchEnd_Hour");
    document.getElementById("lblWedLunchEnd_Minute").innerHTML = get_ddl_value("ddlWedLunchEnd_Minute");


    document.getElementById("lblThuStart_Hour").innerHTML = get_ddl_value("ddlThuStart_Hour");
    document.getElementById("lblThuStart_Minute").innerHTML = get_ddl_value("ddlThuStart_Minute");
    document.getElementById("lblThuEnd_Hour").innerHTML = get_ddl_value("ddlThuEnd_Hour");
    document.getElementById("lblThuEnd_Minute").innerHTML = get_ddl_value("ddlThuEnd_Minute");

    document.getElementById("lblThuLunchStart_Hour").innerHTML = get_ddl_value("ddlThuLunchStart_Hour");
    document.getElementById("lblThuLunchStart_Minute").innerHTML = get_ddl_value("ddlThuLunchStart_Minute");
    document.getElementById("lblThuLunchEnd_Hour").innerHTML = get_ddl_value("ddlThuLunchEnd_Hour");
    document.getElementById("lblThuLunchEnd_Minute").innerHTML = get_ddl_value("ddlThuLunchEnd_Minute");


    document.getElementById("lblFriStart_Hour").innerHTML = get_ddl_value("ddlFriStart_Hour");
    document.getElementById("lblFriStart_Minute").innerHTML = get_ddl_value("ddlFriStart_Minute");
    document.getElementById("lblFriEnd_Hour").innerHTML = get_ddl_value("ddlFriEnd_Hour");
    document.getElementById("lblFriEnd_Minute").innerHTML = get_ddl_value("ddlFriEnd_Minute");

    document.getElementById("lblFriLunchStart_Hour").innerHTML = get_ddl_value("ddlFriLunchStart_Hour");
    document.getElementById("lblFriLunchStart_Minute").innerHTML = get_ddl_value("ddlFriLunchStart_Minute");
    document.getElementById("lblFriLunchEnd_Hour").innerHTML = get_ddl_value("ddlFriLunchEnd_Hour");
    document.getElementById("lblFriLunchEnd_Minute").innerHTML = get_ddl_value("ddlFriLunchEnd_Minute");


    document.getElementById("lblSatStart_Hour").innerHTML = get_ddl_value("ddlSatStart_Hour");
    document.getElementById("lblSatStart_Minute").innerHTML = get_ddl_value("ddlSatStart_Minute");
    document.getElementById("lblSatEnd_Hour").innerHTML = get_ddl_value("ddlSatEnd_Hour");
    document.getElementById("lblSatEnd_Minute").innerHTML = get_ddl_value("ddlSatEnd_Minute");

    document.getElementById("lblSatLunchStart_Hour").innerHTML = get_ddl_value("ddlSatLunchStart_Hour");
    document.getElementById("lblSatLunchStart_Minute").innerHTML = get_ddl_value("ddlSatLunchStart_Minute");
    document.getElementById("lblSatLunchEnd_Hour").innerHTML = get_ddl_value("ddlSatLunchEnd_Hour");
    document.getElementById("lblSatLunchEnd_Minute").innerHTML = get_ddl_value("ddlSatLunchEnd_Minute");
}


String.prototype.pad_left = function (length, padString) {
    var str = this;
    while (str.length < length)
        str = padString + str;
    return str;
}

function to_long_name(weekday) {
    if (weekday == "sun")
        return "sunday";
    if (weekday == "mon")
        return "monday";
    if (weekday == "tue")
        return "tuesday";
    if (weekday == "wed")
        return "wednesday";
    if (weekday == "thu")
        return "thursday";
    if (weekday == "fri")
        return "friday";
    if (weekday == "sat")
        return "saturday";
}
