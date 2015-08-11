function validate_uncheck(c) {

    if (!c.checked) {

        var weekday = String(c.id).replace("chkInc", "").toLowerCase();
        var id = getUrlVars()["id"];
        var booked_days = ajax_future_bookings("provider", id, weekday, "0000", "2359", "0");
        if (booked_days == "NONE")
            return;

        var space = "          ";
        var booked_days_list = space + booked_days.replace(/,/gi, "\r\n" + space);

        alert("Can not de-select day until the bookings on the following days have been moved or deleted:\r\n\r\n" + booked_days_list);
        c.checked = true;
    }
}