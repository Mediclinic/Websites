<%@ Page Title="Bookings" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="BookingsV1.aspx.cs" Inherits="BookingsV1" %>
<%@ Register TagPrefix="UC" TagName="BookingModalElementControl" Src="~/Bookings/BookingModalElementControlV2.ascx" %>
<%@ Register TagPrefix="UC" TagName="HealthCardInfoControl" Src="~/Controls/HealthCardInfoControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

    <link href='http://fonts.googleapis.com/css?family=Open+Sans' rel='stylesheet' type='text/css'>
    <link href="Styles/booking_modal_boxV2.css" rel="stylesheet" type="text/css" />


    <!--script type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script-->
    <!-- right click menu works only if inc 1.4.1 .... longclick only works without it... -->

    <script src="ScriptsV2/jquery.mobile.taphold.min.js"></script>


    <script type="text/javascript" src="Scripts/FUNZIONANTE.js"></script>
    <script type="text/javascript" src="Scripts/bookingsV2.js"></script>
    <script type="text/javascript" src="Scripts/test_context_menus.js"></script>
    <script type="text/javascript" src="Scripts/get_details_of_person.js"></script>



    <script type="text/javascript">

        var page_type = "bookings_for_clinic";

        var min_height_to_allow_resize = 525;

        window.onresize = function(event) {
            resize_booking_sheet();
        }

        window.onload = function() { 

            // hide initially - can't do this in div setting display=none - interferes w construction of slide-out-div & then can't set to block to be visible
            //document.getElementById("slide_out_div").style.display = "none"; 
            //ajax_update_waiting_patient_list();

            resize_booking_sheet(true); 


            // for letters downloading to client, need to have it reload fully, and when done, the 
            // javascript here reloads to send download to user which prevents the new page display (re-)load
            if (document.getElementById("hiddenFieldReloadPage").value == "1")
                window.location.href = window.location.href;
        };

        var doit; 
        var doit2; 
        function resize_booking_sheet(onload)
        {
            clearTimeout(doit2);
            //doit2 = setTimeout(resize_width, 200);
            resize_width();


            if (onload)
            {
                fit_top_screen();
            }
            else
            {
                clearTimeout(doit);
                doit = setTimeout(fit_top_screen, 200);
            }

        }

        function dblclick(t_id)
        {
            show_modal_add_box(t_id, false);
        }

        var pressTimer;
        function bk_mouse_down(t_id)
        {
            pressTimer = window.setTimeout(function() {  show_modal_add_box(t_id, false);  },1000)
        }

        function bk_mouse_up(t_id)
        {
            clearTimeout(pressTimer)
        }


        function resize_width()
        {
            var controlID          = 'main_panel';
            var secondaryControlID = 'header_panel';

            var curText       = document.getElementById(controlID).style.width;
            var curVal        = parseInt(curText.endsWith("px") ? curText.substring(0, curText.length-2) : curText);
            var curTextEnding = curText.endsWith("px") ? curText.substring(curText.length-2) : "";

            var newWidth = document.documentElement.clientWidth - 40;

            document.getElementById(controlID).style.width = newWidth + curTextEnding;


            // if width of screen is wider than width of the booking sheet, put the scroll bar (ie the panel width) equal to the booking sheet
            // ratehr than the panel bar being way on the end of the screen beyond the booking sheet width
            if (newWidth > document.getElementById('main_table').clientWidth)
            {
                newWidth = document.getElementById('main_table').clientWidth;
                document.getElementById(controlID).style.width = newWidth + curTextEnding;

                scrollbarWidth = document.getElementById(controlID).offsetWidth - document.getElementById(controlID).clientWidth;
                if (scrollbarWidth > 0)
                {
                    document.getElementById(controlID).style.width = (newWidth + scrollbarWidth + 2) + curTextEnding;
                }
            }

            // upate the header div with date/org/staff names ... to match
            scrollbarWidth = document.getElementById(controlID).offsetWidth - document.getElementById(controlID).clientWidth;
            curText       = document.getElementById(controlID).style.width;
            curVal        = curText.endsWith("px") ? curText.substring(0, curText.length-2) : curText;
            document.getElementById(secondaryControlID).style.width = (parseInt(curVal) - scrollbarWidth) + curTextEnding;


        }

        // -------------------------------

        // key press events

        var isCtrl = false; 
        var isShift = false; 
        document.onkeyup=function(e){ 
            if(e.which == 17) isCtrl=false;
            if(e.which == 16) isShift=false;
        } 
        document.onkeydown=function(e){ 

            if(e.which == 17) isCtrl=true; 
            if(e.which == 16) isShift=true; 
            if (e.which == 38 && isCtrl == true) { // alert("ctrl-up");
                show_hide_booking_sheet('header_section', false)
                //set_booking_sheet_focused(false);
                // return false;  // stops any further action from keystroke
            }
            if (e.which == 40 && isCtrl == true) { // alert("ctrl-down");
                show_hide_booking_sheet('header_section', true)
                //set_booking_sheet_focused(true);
                // return false;  // stops any further action from keystroke
            }

            if (e.which == 112) { //alert("F1");
                var wasChecked = chkShowDetails.checked;
                chkShowDetails.checked = !wasChecked;
                show_hide_booking_details(!wasChecked);
                if (wasChecked)
                    removeExcessFromPanelHeight('main_panel');
                return false;  // stops any further action from keystroke
            }
            if (e.which == 113) { //alert("F2");
                var chkShowUnavailableStaff = document.getElementById("chkShowUnavailableStaff");
                if (chkShowUnavailableStaff != null)
                    chkShowUnavailableStaff.click();
                return false;  // stops any further action from keystroke
            }
            if (e.which == 114) { //alert("F3");
                var chkShowOtherProviders = document.getElementById("chkShowOtherProviders");
                if (chkShowOtherProviders != null)
                    chkShowOtherProviders.click();
                return false;  // stops any further action from keystroke
            }
        }

        // -------------------------------

        function show_hide_booking_details(checked) {
            var regEx = /lbl_\d+_\d+_\d{4}_\d{2}_\d{2}_\d{4}/i;    //  "lbl_2301_4_2012_06_27_0800"
            var regEx_lblPatientName = /lblPatientName_\d+_\d+_\d{4}_\d{2}_\d{2}_\d{4}/i;    //  "lblPatientName_2301_4_2012_06_27_0800"

            var tds = document.getElementById("main_table").getElementsByTagName('td');
            for (var i = 0; i < tds.length; i++) {
                for (var j = 0; j < tds[i].childNodes.length; j++) {
                    var elem = tds[i].childNodes[j];
                    if (regEx.test(elem.id))
                        elem.style.display = (checked) ? "block" : "none";
                    if (regEx_lblPatientName.test(elem.id))
                        elem.style.display = (checked) ? "none" : "block";
                }
            }
        }

        // -------------------------------

        function set_booking_sheet_focused(checked) {

            chkBookingSheetFocused.checked = checked;

            if (!checked)
                fit_top_screen();
            else
                fit_bottum_screen();
        }

        function fit_top_screen()
        {
            document.body.scrollTop = document.documentElement.scrollTop = 0; // scroll to top
            update_to_max_height_no_scrollbars(min_height_to_allow_resize);
            resize_width();
            //resize_booking_sheet();
        }



        function fit_bottum_screen()
        {
            var controlID          = 'main_panel';
            var secondaryControlID = 'header_panel';

            var curText       = document.getElementById(controlID).style.height;
            var curVal        = parseInt(curText.endsWith("px") ? curText.substring(0, curText.length-2) : curText);
            var curTextEnding = curText.endsWith("px") ? curText.substring(curText.length-2) : "";

            var newHeight = document.documentElement.clientHeight - 110;
            document.getElementById(controlID).style.height = newHeight + curTextEnding;

            document.getElementById('pnlBookingArea').scrollIntoView();
        }

        function update_to_max_height_no_scrollbars(min_window_height)
        {
            // update height of sheet so window just barely has no vertical scroll

            var controlID = 'main_panel';

            var curText       = document.getElementById(controlID).style.height;
            var curVal        = parseInt(curText.endsWith("px") ? curText.substring(0, curText.length-2) : curText);
            var curTextEnding = curText.endsWith("px") ? curText.substring(curText.length-2) : "";

            var newVal = curVal;

            newVal = resize_vertical(false, newVal, 256, curTextEnding, controlID, min_window_height);
            newVal = resize_vertical(true,  newVal, 64,  curTextEnding, controlID, min_window_height);
            newVal = resize_vertical(false, newVal, 16,  curTextEnding, controlID, min_window_height);
            newVal = resize_vertical(true,  newVal, 4,   curTextEnding, controlID, min_window_height);
        }
        function resize_vertical(whileHasScroll, newVal, stepSize, curTextEnding, controlID, min_window_height)
        {
            if (document.documentElement.clientHeight < min_window_height)
                return newVal;

            var hasVScroll = document.documentElement.scrollHeight > document.documentElement.clientHeight;

            if (whileHasScroll)
            {
                while (document.documentElement.clientHeight >= min_window_height && (newVal - stepSize) > 0 && hasVScroll)
                {
                    newVal -= stepSize;
                    document.getElementById(controlID).style.height = newVal + curTextEnding;

                    hasVScroll = document.documentElement.scrollHeight > document.documentElement.clientHeight;
                }
            }
            else
            {
                while (document.documentElement.clientHeight >= min_window_height && !hasVScroll)
                {
                    newVal += stepSize;
                    document.getElementById(controlID).style.height = newVal + curTextEnding;

                    hasVScroll = document.documentElement.scrollHeight > document.documentElement.clientHeight;
                }
            }

            return newVal;
        }

        function removeExcessFromPanelHeight(controlID)
        {
            if (document.getElementById(controlID).scrollHeight <= document.getElementById(controlID).clientHeight)  // if no scroll bars, remove excess at the end of the panel to fit contents
            {
                var curText       = document.getElementById(controlID).style.height;
                var curVal        = parseInt(curText.endsWith("px") ? curText.substring(0, curText.length-2) : curText);
                var curTextEnding = curText.endsWith("px") ? curText.substring(curText.length-2) : "";

                var newVal = curVal;

                newVal = resize_remove_vertical_excess(false, newVal, 1024, curTextEnding, controlID, min_window_height);
                newVal = resize_remove_vertical_excess(true,  newVal, 264,  curTextEnding, controlID, min_window_height);
                newVal = resize_remove_vertical_excess(false, newVal, 64,   curTextEnding, controlID, min_window_height);
                newVal = resize_remove_vertical_excess(true,  newVal, 16,   curTextEnding, controlID, min_window_height);
                newVal = resize_remove_vertical_excess(false, newVal, 4,    curTextEnding, controlID, min_window_height);
                newVal = resize_remove_vertical_excess(false, newVal, 1,    curTextEnding, controlID, min_window_height);
            }
        }
        function resize_remove_vertical_excess(whileHasScroll, newVal, stepSize, curTextEnding, controlID)
        {
            var hasVScroll = document.getElementById(controlID).scrollHeight > document.getElementById(controlID).clientHeight;

            if (!whileHasScroll)
            {
                while (!hasVScroll) // while no scroll, reduce by step until has scroll
                {
                    newVal -= stepSize;
                    document.getElementById(controlID).style.height = newVal + curTextEnding;

                    hasVScroll = document.getElementById(controlID).scrollHeight > document.getElementById(controlID).clientHeight;
                }
            }
            else
            {
                while (hasVScroll) // while has scroll, increase by step until no scroll
                {
                    newVal += stepSize;
                    document.getElementById(controlID).style.height = newVal + curTextEnding;

                    hasVScroll = document.getElementById(controlID).scrollHeight > document.getElementById(controlID).clientHeight;
                }
            }

            return newVal;
        }

        // ---------------------------------------------------------------------------------------------

        function live_search(str) {

            if (str.length == 0) {
                document.getElementById("div_livesearch").innerHTML = "";
                document.getElementById("div_livesearch").style.border = "0px";
                return;
            }
            if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
                xmlhttp = new XMLHttpRequest();
            }
            else {// code for IE6, IE5
                xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
                    var response = String(xmlhttp.responseText);
                    if (response == "SessionTimedOutException")
                        window.location.href = window.location.href;  // reload page
                    document.getElementById("div_livesearch").innerHTML = response;
                    document.getElementById("div_livesearch").style.border = "1px solid #A5ACB2";
                }
            }

            // need to remove patient id from url, then use below to put in current url with patient id as below
            // and add in scroll pos, etc

            var new_url = window.location.href;
            new_url = set_url_field(new_url, "scroll_pos", document.getElementById('scrollValues').value);
            new_url = set_url_field(new_url, "is_collapsed", document.getElementById("chkShowDetails").checked ? "0" : "1");
            new_url = set_url_field(new_url, "patient", "[patient_id]");
            new_url = update_url_field(element_exists('ddlFields') && get_ddl_value("ddlFields") != "-1", new_url, "field", element_exists('ddlFields') ? get_ddl_value("ddlFields") : "");
            new_url = update_url_field(element_exists('ddlServices') && get_ddl_value("ddlServices") != "-1", new_url, "offering", element_exists('ddlServices') ? get_ddl_value("ddlServices") : "");

            xmlhttp.open("GET", "/AJAX/AjaxLivePatientSurnameSearch.aspx?q=" + str + "&max_results=80&link_href=" + encodeURIComponent(new_url) + "&link_onclick=", true);
            xmlhttp.send();
        }

        function clear_live_search() {
            document.getElementById("div_livesearch").innerHTML = "";
            document.getElementById("div_livesearch").style.border = "0px";
            document.getElementById("txtSearchFullName").value = "";
            //document.getElementById("txtSearchFullName").style.backgroundImage = "url('/images/textbox_watermark_surname_first.png')";
        }

        function clear_selected_patient() {
            var new_url = window.location.href;
            new_url = set_url_field(new_url, "scroll_pos", document.getElementById('scrollValues').value);
            new_url = set_url_field(new_url, "is_collapsed", document.getElementById("chkShowDetails").checked ? "0" : "1");
            new_url = set_url_field(new_url, "patient", "-1");
            new_url = update_url_field(element_exists('ddlFields') && get_ddl_value("ddlFields") != "-1", new_url, "field", element_exists('ddlFields') ? get_ddl_value("ddlFields") : "");
            new_url = update_url_field(element_exists('ddlServices') && get_ddl_value("ddlServices") != "-1", new_url, "offering", element_exists('ddlServices') ? get_ddl_value("ddlServices") : "");
            window.location.href = new_url;
        }

        function set_watermark(txtbox, val) {
            //txtbox.style.backgroundImage = (txtbox.value.length == 0 && val) ? "url('/images/textbox_watermark_surname_first.png')" : "";
        }


        // ---------------------------------------------------------------------------------------------

        function get_patient() {

            var IsAdmin = <% = Session["IsAdmin"].ToString().ToLower() %>; 

            var org_ids = document.getElementById('lblOrgIDs').innerHTML;
            var retVal = IsAdmin ?
                window.showModalDialog("PatientListPopupV2.aspx", 'Show Popup Window', "dialogHeight:700px;dialogWidth:700px;resizable:yes;center:yes;")
                :
                window.showModalDialog("PatientListPopupV2.aspx?orgs=" + org_ids, 'Show Popup Window', "dialogHeight:700px;dialogWidth:700px;resizable:yes;center:yes;");

            if (typeof retVal === "undefined" || retVal == null)
                return;

            var index = retVal.indexOf(":");
            document.getElementById('txtPatientID').value = retVal.substring(0, index);
            document.getElementById('txtUpdatePatientName').value = retVal.substring(index + 1);


            var new_url = window.location.href;
            new_url = set_url_field(new_url, "patient", retVal.substring(0, index));
            new_url = set_url_field(new_url, "scroll_pos", document.getElementById('scrollValues').value);
            new_url = update_url_field(get_ddl_value("ddlFields") != "-1", new_url, "field", get_ddl_value("ddlFields"));
            new_url = update_url_field(get_ddl_value("ddlServices") != "-1", new_url, "offering", get_ddl_value("ddlServices"));
            window.location.href = new_url;

            set_textbox_style('txtUpdatePatientName', false);
        }

        function clear_patient() {
            document.getElementById('txtPatientID').value = '';
            document.getElementById('txtUpdatePatientName').value = '';

            // hide before it posts back
            document.getElementById('tdEPCsRemaining').style.display = "none";

            var new_url = window.location.href;
            new_url = remove_url_field(new_url, "patient");
            new_url = set_url_field(new_url, "scroll_pos", document.getElementById('scrollValues').value);
            new_url = update_url_field(get_ddl_value("ddlFields") != "-1", new_url, "field", get_ddl_value("ddlFields"));
            new_url = update_url_field(get_ddl_value("ddlServices") != "-1", new_url, "offering", get_ddl_value("ddlServices"));
            window.location.href = new_url;

            set_textbox_style('txtUpdatePatientName', true);
        }

        function quick_add() {
            var retVal = window.showModalDialog("PatientAddV2.aspx?surname="+document.getElementById('txtSearchFullName').value.trim(), '', "dialogHeight:900px;dialogWidth:1550px;resizable:yes;center:yes;");
            live_search(document.getElementById('txtSearchFullName').value);
        }

        // ---------------------------------------------------------------------------------------------

        // to make the flashing text.  every 1000ms : turn on for 700ms - then turn off 
        window.addEventListener("load", function () {
            setInterval(function () {

                document.getElementById('lblFlashingText').style.visibility='visible';

                setTimeout(function () {
                    document.getElementById('lblFlashingText').style.visibility='hidden';
                }, 700);

            }, 1000);
        }, false);

        // to make the flashing image.  every 1000ms : turn on for 700ms - then turn off 
        window.addEventListener("load", function () {
            setInterval(function () {


                elements = document.getElementsByClassName('flashing_text_icon');
                for (var i = 0; i < elements.length; i++)
                    elements[i].style.opacity = '0.0';

                //$('flashing_text_icon').css("opacity", "0.1");

                setTimeout(function () {

                    elements = document.getElementsByClassName('flashing_text_icon');
                    for (var i = 0; i < elements.length; i++)
                        elements[i].style.opacity = '1';
                    //$('flashing_text_icon').css("opacity", "1");
                }, 700);

            }, 1000);
        }, false);

        // ---------------------------------------------------------------------------------------------

        function show_hide_booking_sheet(id, show)
        {
            if (show === undefined)
                show = document.getElementById(id).style.display != "";
            document.getElementById(id).style.display = show ? "" : "none";
            if (id == 'header_section')
            {
                fit_top_screen(); //update autodivheight
                ajax_booking_update_show_header_section(show) // update ajax
            }
        }

        // ---------------------------------------------------------------------------------------------

        function is_mobile_device() {
            return ( navigator.userAgent.match(/Android/i) ||
                navigator.userAgent.match(/Windows Phone/i) ||
                navigator.userAgent.match(/iPhone/i));
        }

        // ---------------------------------------------------------------------------------------------


    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

        <asp:HiddenField ID="hiddenFieldReloadPage" runat="server" />

        <asp:Label ID="lblMinBookingDurationMins" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblOrgIDs" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblOrgTypeIDs" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblOrgNames" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblProviderNames" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblProviderIDs" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblSelectedOrgID" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblSelectedOrgTypeID" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblSelectedProviderID" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblEditBooking_Duration" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblEditBooking_Org" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblEditBooking_IsConfirmed" runat="server" CssClass="hiddencol"></asp:Label>
        <asp:Label ID="lblEditBooking_IsAgedCare" runat="server" CssClass="hiddencol">0</asp:Label>
        <asp:Label ID="lblBookingTimeEditable" runat="server" CssClass="hiddencol"></asp:Label>
        <input type="hidden" id="scrollValues" name="scrollValues" runat="server" value="0" />

        <asp:HiddenField ID="printOrEmailInvoiceBookingID" runat="server" Value="-1" />
        <asp:Button ID="btnPrintInvoice" runat="server" CssClass="hiddencol" OnClick="btnPrintInvoice_Click" />
        <asp:Button ID="btnEmailInvoice" runat="server" CssClass="hiddencol" OnClick="btnEmailInvoice_Click" />

        <UC:BookingModalElementControl ID="bookingModalElementControl" runat="server" />


    
        <div class="clearfix">
            <div class="page_title">

                <table class="block_center">
                    <tr>
                        <td>
                            <span><asp:Label ID="lblHeading" runat="server">Bookings for Fairfield Clinic</asp:Label></span>
                        </td>
                        <td style="min-width:20px;">&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <asp:label ID="lblOrgAddress" runat="server">82 Cotton Road MORPETH, NSW 2321</asp:label>
                        </td>
                    </tr>

                </table>

            </div>
            <div class="main_content" style="padding:10px 5px 20px;">


                <center>

                    <table  id="links_header_section" runat="server" style="width:75%">
                        <tr>
                            <td style="width:26%;min-width:250px;"></td>
                            <td style="width:11%;min-width:55px;"></td>
                            <td style="width:26%;min-width:250px;text-align:center;">
                                <a href="javascript:void(0)" onclick="show_hide_booking_sheet('header_section');return false;">Hide/Show Header (Ctrl-UP/Down)</a>
                            </td>
                            <td style="width:11%;min-width:55px;"></td>
                            <td style="width:26%;min-width:250px;">
                                <a href="javascript:void(0)" class="" onclick="show_hide_booking_sheet('div_fields_list'); return false">Show/Hide Booking Colour Key</a><br />
                                <div id="div_fields_list" style="display:none;position:absolute;background:#FFFFFF;border:1px solid grey;">

                                    <div class="text_center" style="margin-bottom:10px;"><strong>Availability Key</strong></div>
                                    <table style="width:300px;overflow: hidden;margin:5px;">
                                        <tbody>
                                            <tr>
                                                <td class="nowrap" style="width:130px;"><span id="colorkey_available2" runat="server" class="circle" style="background: #800080;"></span> Available</td>
                                                <td style="width:40px;"></td>
                                                <td class="nowrap" style="width:130px;"><span id="colorkey_completed2" runat="server" class="circle" style="background: #800080;"></span> <span id="colorkey_completed_text2" runat="server">Completed</span></td>
                                            </tr>
                                            <tr>
                                                <td><span id="colorkey_unavailable2" runat="server" class="circle" style="background: #800080;"></span> Unavailable</td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <div class="text_center" style="margin-bottom:10px;"><strong>Booking Key</strong></div>
                                    <table style="width:100%;overflow: hidden;margin:8px;">
                                        <tbody>
                                            <tr>
                                                <td class="nowrap" style="width:130px"><span id="colorkey_clinic_uncomf_nonepc2" runat="server" class="circle" style="background: #800080;"></span> [Clinic] Uncof.</td>
                                                <td style="width:40px;"></td>
                                                <td class="nowrap" style="width:130px"><span id="colorkey_ac_uncomf_nonepc2" runat="server" class="circle" style="background: #800080;"></span> [AC] Unconf.</td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><span id="colorkey_clinic_comf_nonepc2" runat="server" class="circle" style="background: #800080;"></span> [Clinic] Conf.</td>
                                                <td></td>
                                                <td class="nowrap"><span id="colorkey_ac_comf_nonepc2" runat="server" class="circle" style="background: #800080;"></span> [AC] Conf.</td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><span id="colorkey_clinic_uncomf_epc2" runat="server" class="circle" style="background: #800080;"></span> [Clinic] Unconf. EPC</td>
                                                <td></td>
                                                <td class="nowrap"><span id="colorkey_ac_uncomf_epc2" runat="server" class="circle" style="background: #800080;"></span> [AC] Unconf. EPC  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><span id="colorkey_clinic_comf_epc2" runat="server" class="circle" style="background: #800080;"></span> [Clinic] Conf. EPC</td>
                                                <td></td>
                                                <td class="nowrap"><span id="colorkey_ac_comf_epc2" runat="server" class="circle" style="background: #800080;"></span> [AC] Conf. EPC</td>
                                            </tr>
                                        </tbody>
                                    </table>

                                </div>
                            </td>
                        </tr>
                    </table>

                    <div style="height:10px;"></div>
                </center>


                <div id="header_section" runat="server" class="clearfix line_height_booking_settings" >

                    <table style="width:100%;text-align:center">
                        <tr>
                            <td>

                                <center>

                                    <table style="text-align:left; vertical-align:top;">
                                        <tr style="vertical-align:top;">
                                            <td id="div_pt_search" runat="server" style="width:350px;">

                                                <table id="tblPatientSearch" runat="server" border="0">

                                                    <tr>
                                                        <td style="text-align: left;">

                                                            <table border="0">

                                                                <tr>
                                                                    <td class="nowrap" style="text-align: left;">Get Patient</td>
                                                                    <td class="nowrap" style="text-align: left; min-width:10px;"></td>
                                                                    <td class="nowrap" style="text-align: left;">
                                                                        <asp:TextBox ID="txtSearchFullName" runat="server" placeholder="Enter Surname" onkeyup="live_search(this.value)" autocomplete="off" onkeydown="return (event.keyCode!=13);" ></asp:TextBox>
                                                                        <div id="div_livesearch" style="position: absolute; background: #FFFFFF;"></div>
                                                                    </td>
                                                                    <td class="nowrap" style="text-align: left; min-width:5px;"></td>
                                                                    <td class="nowrap" style="text-align: left;">
                                                                        <button type='button' name="btnClearFullNameSearch" onclick="clear_live_search(); return false;">Clear</button>
                                                                        <button type='button' name="btnClearFullNameSearch" onclick="quick_add(); return false;">Add</button>
                                                                    </td>
                                                                    <td class="nowrap" style="min-width: 5px"></td>
                                                                </tr>

                                                                <tr id="tr_selected_patient_row" runat="server">
                                                                    <td class="nowrap" style="text-align: left;">Selected Patient</td>
                                                                    <td class="nowrap" style="text-align: left; min-width:10px;"></td>
                                                                    <td class="nowrap" style="text-align: left;">
                                                                        <asp:Label ID="lblSelectedPatientName" runat="server"></asp:Label></td>
                                                                    <td class="nowrap" style="text-align: left; min-width:5px;"></td>
                                                                    <td class="nowrap" style="text-align: left;">
                                                                        <asp:Button ID="btnClearSelectedPatient" runat="server" Text="Clear" OnClientClick="javascript:clear_selected_patient();return false;" /></td>
                                                                    <td class="nowrap"></td>
                                                                </tr>

                                                            </table>
                                                        </td>
                                                    </tr>

                                                    <tr id="healthcardInfoRow" runat="server">
                                                        <td>
                                                            <div style="margin:5px 0; background: #999; height:1px; width:97%;"></div>
                                                               <UC:HealthCardInfoControl ID="healthCardInfoControl" runat="server" />
                                                            <div style="margin:5px 0; background: #999; height:1px; width:97%;"></div>
                                                        </td>
                                                    </tr>

                                                </table>

                                                <table id="tblPatientInfo" runat="server" border="0" class="hiddencol">
                                                    <tr>
                                                        <td>Patient:</td>
                                                        <td style="width: 6px"></td>
                                                        <td>
                                                            <asp:TextBox ID="txtPatientID" runat="server" CssClass="hiddencol" />
                                                            <asp:TextBox ID="txtUpdatePatientName" runat="server" Enabled="False" />
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btnPatientListPopup" runat="server" Text="Get Patient" OnClientClick="javascript:get_patient(); return false;" />
                                                            <asp:Button ID="btnClearPatient" runat="server" Text="Clear" OnClientClick="javascript:clear_patient();return false;" />
                                                        </td>
                                                    </tr>
                                                    <tr style="height: 5px;">
                                                        <td colspan="4"></td>
                                                    </tr>
                                                    <tr id="healthcardInfoRow_OLD" runat="server">
                                                        <td colspan="4"></td>
                                                    </tr>
                                                </table>
                        
                                                <table id="tbl_patient_flashing_message" runat="server" style="width:100%;">
                                                    <tr style="vertical-align:top;">
                                                        <td style="text-align:left;width:15px;">
                                                            <asp:ImageButton ID="lnkFlashingText" runat="server" AlternateText="Edit Flashing Text For This Patient" ToolTip="Edit Flashing Text For This Patient" />
                                                        </td>
                                                        <td style="text-align:center;">
                                                            <asp:Label ID="lblFlashingText" runat="server" ForeColor="Red" style="width:100%;" />
                                                            <br />
                                                            <asp:Label ID="lblInvoicesOwingMessage" runat="server"></asp:Label>
                                                            <br id="br_invoices_owing_message_trailing_space" runat="server" />
                                                        </td>
                                                        <td style="width:15px;"></td>
                                                    </tr>
                                                </table>

                                            </td>
                                            <td  id="div_pt_search_space" runat="server" style="width:1px;">
                                                <div class="bookings_header_vertical_divider"></div>
                                            </td>
                                            <td style="width:350px;">

                                                <div id="div_services" runat="server">
                                                    <table border="0" >
                                                        <tr>
                                                            <td>Field</td>
                                                            <td class="nowrap">&nbsp;<asp:DropDownList ID="ddlFields" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlFields_SelectedIndexChanged"/></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Service</td>
                                                            <td class="nowrap">&nbsp;<asp:DropDownList ID="ddlServices" runat="server"/></td>
                                                        </tr>
                                                    </table>
                                                </div>

                                                <div id="div_services_trailing_seperator" runat="server" style="margin:5px 0; background: #999; height:1px; width:97%;"></div>

                                                <table>
                                                    <tr>
                                                        <td>Start Date:</td>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td><asp:TextBox ID="txtStartDate" runat="server" Columns="10" onkeypress="javascript:if (event.which == 13 || event.keyCode == 13) document.getElementById('btnUpdateDisplayOptions').click();" /></td>
                                                                    <td><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                                    <td><asp:Button ID="btnUpdateDisplayOptions" runat="server" Text="Refresh" OnClick="btnUpdateDisplayOptions_Click"/></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Days:</td>
                                                        <td><asp:DropDownList ID="ddlDaysToDisplay" runat="server" onchange="reload_booking_page_ndays();"></asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Show Bookings Details:</td>
                                                        <td><asp:CheckBox ID="chkShowDetails" runat="server" Checked="True" onclick="show_hide_booking_details(this.checked);" />&nbsp;&nbsp;(F1)</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Show Unavailable Staff:</td>
                                                        <td><asp:CheckBox ID="chkShowUnavailableStaff" runat="server" onclick="reload_booking_page_show_unavailable_staff();" />&nbsp;&nbsp;(F2)</td>
                                                    </tr>
                                                    <tr id="tr_show_other_providers" runat="server">
                                                        <td>Show Other Providers:</td>
                                                        <td><asp:CheckBox ID="chkShowOtherProviders" runat="server" OnCheckedChanged="chkShowOtherProviders_CheckedChanged" AutoPostBack="true" />&nbsp;&nbsp;(F3) <font color="red">*New*</font></td>
                                                    </tr>
                                                </table>

                                                <div style="margin:5px 0; background: #999; height:1px; width:97%;"></div>

                                                <table style="width:100%; margin: 0 auto; ">
                                                    <tr>
                                                        <td style="width:25%;text-align:center;">
                                                            <asp:ImageButton ID="btnMoveDateToday" runat="server" CommandArgument="Today" OnCommand="btnMoveDate_Command" ImageUrl="~/images/today_blue.png" AlternateText="Go" />
                                                        </td>
                                                        <td style="width:75%;text-align:center">

                                                            <table>
                                                                <tr>
                                                                    <td><asp:ImageButton ID="btnMoveDateBackwards" runat="server" CommandArgument="Backwards" OnCommand="btnMoveDate_Command" ImageUrl="~/images/arrow_left_16.png" AlternateText="Go" /></td>
                                                                    <td style="width:5px;"></td>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlMoveDateNum" runat="server">
                                                                            <asp:ListItem Text="1" Value="1"/>
                                                                            <asp:ListItem Text="2" Value="2"/>
                                                                            <asp:ListItem Text="3" Value="3"/>
                                                                            <asp:ListItem Text="4" Value="4"/>
                                                                            <asp:ListItem Text="5" Value="5"/>
                                                                            <asp:ListItem Text="6" Value="6"/>
                                                                            <asp:ListItem Text="7" Value="7"/>
                                                                            <asp:ListItem Text="8" Value="8"/>
                                                                            <asp:ListItem Text="9" Value="9"/>
                                                                            <asp:ListItem Text="10" Value="10"/>
                                                                            <asp:ListItem Text="11" Value="11"/>
                                                                            <asp:ListItem Text="12" Value="12"/>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td style="width:1px;"></td>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlMoveDateType" runat="server">
                                                                            <asp:ListItem Text="Day(s)" Value="Days"/>
                                                                            <asp:ListItem Text="Week(s)" Value="Weeks" Selected="True"/>
                                                                            <asp:ListItem Text="Month(s)" Value="Months"/>
                                                                            <asp:ListItem Text="Year(s)" Value="Years"/>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td style="width:5px;"></td>
                                                                    <td><asp:ImageButton ID="btnMoveDateForwards" runat="server" CommandArgument="Forwards" OnCommand="btnMoveDate_Command" ImageUrl="~/images/arrow_right_16.png" AlternateText="Go" /></td>
                                                                </tr>
                                                            </table>
                                            
                                                        </td>
                                                    </tr>
                                                </table>

                                                <div class="text_center" id="div_stop_edit_btn" runat="server">
                                                    <asp:Button ID="btnStopEdit" runat="server" Height="25px" Font-Bold="True" Text="Cancel Edit / Move" onclick="btnStopEdit_Click"/>
                                                </div>

                                                <div style="height:26px;"></div>

                                            </td>

                                        </tr>
                                    </table>

                                </center>

                            </td>
                        </tr>
                    </table>

                </div>

                <div style="min-height:8px;"></div>

                <div class="text-center">
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>

                <table class="block_center" id="tbl_booking_sheet" oncontextmenu="return false;">
                    <tr>
                        <td>
                            <asp:Panel ID="header_panel" runat="server" ScrollBars="Auto" Width="1497"  onscroll="Javascript:elementOnScroll(this);elementOnScrollLeft(this);">
                                <table border="1" id="header_table" runat="server" class="booking-table-grid-header" style="width:1480px;max-width:1480px;" ></table>
                            </asp:Panel>
                            <asp:Panel ID="main_panel" runat="server" ScrollBars="Auto" Height="550" Width="1497" style="min-height:150px;" onscroll="JavaScript:elementOnScroll(this);elementOnScrollLeft(this);" >
                                <table border="1" id="main_table" runat="server" class="booking-table-grid-body" style="width:1480px;max-width:1480px;" ></table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>


            </div>
        </div>

        <script>


            function ShowMenu_Menu_Past_Completed_Has_Invoice (elementID) { 
                ShowContextMenu(null, 
                        [
                         "View Invoice",
                         "Email Invoice",
                         "Reverse",
                         "Letters",
                         "Make Future Booking"
                        ],
                        [
                         "view_invoice('" + elementID + "', false);",
                         "email_invoice('" + elementID + "');",
                         "reverse_booking('" + elementID + "');",
                         "print_letters('" + elementID + "', false);",
                         "future_booking('" + elementID + "');"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_Past_Completed_No_Invoice (elementID) { 
                ShowContextMenu(null, 
                        [
                         "Reverse",
                         "Letters",
                         "Make Future Booking"
                        ],
                        [
                         "reverse_booking('" + elementID + "');",
                         "print_letters('" + elementID + "', false);",
                         "future_booking('" + elementID + "');"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_Past_Uncompleted_Clinic(elementID) { 
                ShowContextMenu(null, 
                        [
                         "Complete",
                         "Edit / Move",
                         "Cancel (no fee)",
                         "Cancel (charge)",
                         "Delete",
                         "Deceased",
                         "Letters"
                        ],
                        [
                         "show_modal_complete_booking('" + elementID + "', 'complete', false);",
                         "set_edit_booking('" + elementID + "');",
                         "delete_confirm_cancel_booking('" + elementID + "','cancel');",
                         "show_modal_complete_booking('" + elementID + "','cancel', false);",
                         "delete_confirm_cancel_booking('" + elementID + "','delete');",
                         "delete_confirm_cancel_booking('" + elementID + "','deceased');",
                         "print_letters('" + elementID + "', false);"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_Past_Uncompleted_AC(elementID) { 
                ShowContextMenu(null, 
                        [
                         "Complete",
                         "Add Patients",
                         "Edit / Move",
                         "Cancel (no fee)",
                         "Cancel (charge)",
                         "Delete",
                         "Deceased",
                         "Letters"
                        ],
                        [
                         "show_modal_complete_booking('" + elementID + "', 'complete', false);",
                         "show_modal_complete_booking('" + elementID + "', 'add_patients', false);",
                         "set_edit_booking('" + elementID + "');",
                         "delete_confirm_cancel_booking('" + elementID + "','cancel');",
                         "show_modal_complete_booking('" + elementID + "','cancel', false);",
                         "delete_confirm_cancel_booking('" + elementID + "','delete');",
                         "delete_confirm_cancel_booking('" + elementID + "','deceased');",
                         "print_letters('" + elementID + "', false);"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_Future_Clinic(elementID) { 
                ShowContextMenu(null, 
                        [
                         "Edit / Move",
                         "Delete",
                         "Letters"
                        ],
                        [
                         "set_edit_booking('" + elementID + "');",
                         "delete_confirm_cancel_booking('" + elementID + "','delete');",
                         "print_letters('" + elementID + "', false);"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_Future_AC(elementID) { 
                ShowContextMenu(null, 
                        [
                         "Add Patients",
                         "Edit / Move",
                         "Delete",
                         "Letters"
                        ],
                        [
                         "show_modal_complete_booking('" + elementID + "', 'add_patients', true);",
                         "set_edit_booking('" + elementID + "');",
                         "delete_confirm_cancel_booking('" + elementID + "','delete');",
                         "print_letters('" + elementID + "', false);"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }


            function ShowMenu_Menu_Future_PatientLoggedIn(elementID) { 
                ShowContextMenu(null, 
                        [
                         "Edit / Move",
                         "Delete"
                        ],
                        [
                         "set_edit_booking('" + elementID + "');",
                         "delete_confirm_cancel_booking('" + elementID + "','delete');"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }


            function ShowMenu_Menu_showAddableContext(elementID) { 
                ShowContextMenu(null, 
                        [
                         "Book"
                        ],
                        [
                         "show_modal_add_box('" + elementID + "','false');"
                        ],
                        [
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_showAddableContext_PatientLoggedIn(elementID) { 
                ShowContextMenu(null, 
                        [
                         "Book"
                        ],
                        [
                         "show_modal_add_box('" + elementID + "','false');"
                        ],
                        [
                         "javascript:void(0)"
                        ],
                         220
                    );
            }


            function ShowMenu_Menu_showTakenButAddableContext(elementID) { 
                var line1 = "Warning!  This will add a booking to an unavailable time slot.";
                var line2 = "Are you sure you want to continue?";
                var text = line1 + "\\r\\n" + "\\r\\n" + Array((line1.length - line2.length) / 2).join(" ") + line2;

                ShowContextMenu(null, 
                        [
                          "Book"
                        ],
                        [
                         "if (confirm('" + text + "')) show_modal_add_box('" + elementID + "', false);"
                        ],
                        [
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_showTakenButUpdatableContext(elementID) { 
                var line1 = "Warning!  This will move the booking to an unavailable time slot.";
                var line2 = "Are you sure you want to continue?";
                var text = line1 + "\\r\\n" + "\\r\\n" + Array(Math.round((line1.length - line2.length) / 2)).join(" ") + line2;

                ShowContextMenu(null, 
                        [
                          "Change To This Time",
                          "Cancel Edit / Move"
                        ],
                        [
                         "if (confirm('" + text + "')) show_modal_add_box('" + elementID + "', false);",
                         "cancel_edit();"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_showUpdatableContext(elementID) { 
                ShowContextMenu(null, 
                        [
                          "Change To This Time",
                          "Cancel Edit / Move"
                        ],
                        [
                         "show_modal_add_box('" + elementID + "');",
                         "cancel_edit();"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_showFullDayAddableContext(elementID) { 
                ShowContextMenu(null, 
                        [
                         "Blockout Today",
                         "Blockout Multiple Days",
                         "Blockout Multiple Days As Series",
                         "Paid Blockout Today",
                         "Paid Blockout Multiple Days",
                         "Move Incomplete Bookings To Another Provider"
                        ],
                        [
                         "show_modal_add_full_days_box('" + elementID + "', 'single_day');",
                         "show_modal_add_full_days_box('" + elementID + "', 'multiple_days');",
                         "show_modal_add_full_days_box('" + elementID + "', 'multiple_days_series');",
                         "show_modal_add_full_days_box('" + elementID + "', 'paid_single_day');",
                         "show_modal_add_full_days_box('" + elementID + "', 'paid_multiple_days');",
                         "set_edit_day('" + elementID + "');"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         385
                    );
            }
            function ShowMenu_Menu_showFullDayUpdatableContext(elementID) { 
                ShowContextMenu(null, 
                        [
                         "Move To This Provider"
                        ],
                        [
                         "change_full_days_bookings('" + elementID + "');"
                        ],
                        [
                         "javascript:void(0)"
                        ],
                         220
                    );
            }
            function ShowMenu_Menu_showFullDayTakenContext(elementID) { 
                var line1 = "Warning!  This will add a booking to an unavailable time slot.";
                var line2 = "Are you sure you want to continue?";
                var text = line1 + "\\r\\n" + "\\r\\n" + Array((line1.length - line2.length) / 2).join(" ") + line2;

                ShowContextMenu(null, 
                        [
                         "Make Booking",
                         "Remove Unavailability"
                        ],
                        [
                         "if (confirm('" + text + "')) show_modal_add_box('" + elementID + "');",
                         "delete_confirm_cancel_booking('" + elementID + "','delete');"
                        ],
                        [
                         "javascript:void(0)",
                         "javascript:void(0)"
                        ],
                         220
                    );
            }

            function ShowMenu_Menu_Past_Completed_Has_Invoice_Clinic(elementID) { 
                ShowMenu_Menu_Past_Completed_Has_Invoice (elementID)
            }
            function ShowMenu_Menu_Past_Completed_No_Invoice (elementID)
            {
                ShowMenu_Menu_Past_Completed_No_Invoice (elementID);
            }



            /*
             *  Sometimes the element clicked didn't have an ID. This will happen for example 
             *  if we have HTML like this:
             *      <div id="example">This is some <b>example</b> text.</div>
             *  and someone clicks on the bolded word "example", so that the event target will 
             *  be the b element instead of the div.
             *  So we walk-up the DOM tree until you find the element that does have an ID.
             */
            function GetTarget(e) {
                var target = e.target || e.srcElement;
                while (target && !target.id) {
                    target = target.parentNode;
                }
                return target;
            }


                $("td.Menu_Past_Completed_Has_Invoice_Clinic").on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "View Invoice",
                             "Print Invoice",
                             "Email Invoice",
                             "Reverse",
                             "Letters",
                             "Make Future Booking"
                            ],
                            [
                             "view_invoice('" + GetTarget(e).id + "', false);",
                             "print_invoice('" + GetTarget(e).id + "');",
                             "email_invoice('" + GetTarget(e).id + "');",
                             "reverse_booking('" + GetTarget(e).id + "');",
                             "print_letters('" + GetTarget(e).id + "', false);",
                             "future_booking('" + GetTarget(e).id + "');"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $("td.Menu_Past_Completed_Has_Invoice_AC").on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "View Invoice",
                             "Print Invoice",
                             "Email Invoice",
                             "Reverse",
                             "Letters",
                             "Make Future Booking",
                             "Treatment List"
                            ],
                            [
                             "view_invoice('" + GetTarget(e).id + "', false);",
                             "print_invoice('" + GetTarget(e).id + "');",
                             "email_invoice('" + GetTarget(e).id + "');",
                             "reverse_booking('" + GetTarget(e).id + "');",
                             "print_letters('" + GetTarget(e).id + "', false);",
                             "future_booking('" + GetTarget(e).id + "');",
                             "treatment_list('" + GetTarget(e).id + "');"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.Menu_Past_Completed_No_Invoice').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Reverse",
                             "Letters",
                             "Make Future Booking"
                            ],
                            [
                             "reverse_booking('" + GetTarget(e).id + "');",
                             "print_letters('" + GetTarget(e).id + "', false);",
                             "future_booking('" + GetTarget(e).id + "');"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.Menu_Past_Uncompleted_Clinic').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Complete",
                             "Edit / Move",
                             "Cancel (no fee)",
                             "Cancel (charge)",
                             "Delete",
                             "Deceased",
                             "Letters"
                            ],
                            [
                             "show_modal_complete_booking('" + GetTarget(e).id + "', 'complete', false);",
                             "set_edit_booking('" + GetTarget(e).id + "');",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','cancel');",
                             "show_modal_complete_booking('" + GetTarget(e).id + "','cancel', false);",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','delete');",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','deceased');",
                             "print_letters('" + GetTarget(e).id + "', false);"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.Menu_Past_Uncompleted_AC').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Complete",
                             "Add Patients",
                             "Edit / Move",
                             "Cancel (no fee)",
                             "Cancel (charge)",
                             "Delete",
                             "Deceased",
                             "Letters"
                            ],
                            [
                             "show_modal_complete_booking('" + GetTarget(e).id + "', 'complete', false);",
                             "show_modal_complete_booking('" + GetTarget(e).id + "', 'add_patients', false);",
                             "set_edit_booking('" + GetTarget(e).id + "');",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','cancel');",
                             "show_modal_complete_booking('" + GetTarget(e).id + "','cancel', false);",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','delete');",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','deceased');",
                             "print_letters('" + GetTarget(e).id + "', false);"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.Menu_Future_Clinic').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Edit / Move",
                             "Delete",
                             "Letters"
                            ],
                            [
                             "set_edit_booking('" + GetTarget(e).id + "');",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','delete');",
                             "print_letters('" + GetTarget(e).id + "', false);"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.Menu_Future_AC').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Add Patients",
                             "Edit / Move",
                             "Delete",
                             "Letters"
                            ],
                            [
                             "show_modal_complete_booking('" + GetTarget(e).id + "', 'add_patients', false);",
                             "set_edit_booking('" + GetTarget(e).id + "');",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','delete');",
                             "print_letters('" + GetTarget(e).id + "', false);"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.Menu_Future_PatientLoggedIn').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Edit / Move",
                             "Delete"
                            ],
                            [
                             "set_edit_booking('" + GetTarget(e).id + "');",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','delete');"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });


                $('td.showAddableContext').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Book"
                            ],
                            [
                             "show_modal_add_box('" + GetTarget(e).id + "','false');"
                            ],
                            [
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.showAddableContext_PatientLoggedIn').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Book"
                            ],
                            [
                             "show_modal_add_box('" + GetTarget(e).id + "','false');"
                            ],
                            [
                             "javascript:void(0)"
                            ],
                             220
                        );
                });

                $('td.showTakenButAddableContext').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    var line1 = "Warning!  This will add a booking to an unavailable time slot.";
                    var line2 = "Are you sure you want to continue?";
                    var text = line1 + "\\r\\n" + "\\r\\n" + Array((line1.length - line2.length) / 2).join(" ") + line2;

                    ShowContextMenu(e, 
                            [
                              "Book"
                            ],
                            [
                             "if (confirm('" + text + "')) show_modal_add_box('" + GetTarget(e).id + "', false);"
                            ],
                            [
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.showTakenButUpdatableContext').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    var line1 = "Warning!  This will move the booking to an unavailable time slot.";
                    var line2 = "Are you sure you want to continue?";
                    var text = line1 + "\\r\\n" + "\\r\\n" + Array(Math.round((line1.length - line2.length) / 2)).join(" ") + line2;

                    ShowContextMenu(e, 
                            [
                              "Change To This Time",
                              "Cancel Edit / Move"
                            ],
                            [
                             "if (confirm('" + text + "')) show_modal_add_box('" + GetTarget(e).id + "', false);",
                             "cancel_edit();"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.showUpdatableContext').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                              "Change To This Time",
                              "Cancel Edit / Move"
                            ],
                            [
                             "show_modal_add_box('" + GetTarget(e).id + "');",
                             "cancel_edit();"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('th.showFullDayAddableContext').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Blockout Today",
                             "Blockout Multiple Days",
                             "Blockout Multiple Days As Series",
                             "Paid Blockout Today",
                             "Paid Blockout Multiple Days",
                             "Move Incomplete Bookings To Another Provider"
                            ],
                            [
                             "show_modal_add_full_days_box('" + GetTarget(e).id + "', 'single_day');",
                             "show_modal_add_full_days_box('" + GetTarget(e).id + "', 'multiple_days');",
                             "show_modal_add_full_days_box('" + GetTarget(e).id + "', 'multiple_days_series');",
                             "show_modal_add_full_days_box('" + GetTarget(e).id + "', 'paid_single_day');",
                             "show_modal_add_full_days_box('" + GetTarget(e).id + "', 'paid_multiple_days');",
                             "set_edit_day('" + GetTarget(e).id + "');"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             385
                        );
                });
                $('th.showFullDayUpdatableContext').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    ShowContextMenu(e, 
                            [
                             "Move To This Provider"
                            ],
                            [
                             "change_full_days_bookings('" + GetTarget(e).id + "');"
                            ],
                            [
                             "javascript:void(0)"
                            ],
                             220
                        );
                });
                $('td.showFullDayTakenContext').on("taphold", function (e) { 
                    e.preventDefault();
                    e.stopImmediatePropagation();

                    var line1 = "Warning!  This will add a booking to an unavailable time slot.";
                    var line2 = "Are you sure you want to continue?";
                    var text = line1 + "\\r\\n" + "\\r\\n" + Array((line1.length - line2.length) / 2).join(" ") + line2;

                    ShowContextMenu(e, 
                            [
                             "Make Booking",
                             "Remove Unavailability"
                            ],
                            [
                             "if (confirm('" + text + "')) show_modal_add_box('" + GetTarget(e).id + "');",
                             "delete_confirm_cancel_booking('" + GetTarget(e).id + "','delete');"
                            ],
                            [
                             "javascript:void(0)",
                             "javascript:void(0)"
                            ],
                             220
                        );
                });


                /*
                $('td.Menu_Past_Completed_Has_Invoice').mousedown(function(e) {
                    if (e.which == 3 )   // 3 = right click
    
                        return false;
                        //ShowContextMenu(e);
                });
                */

                var mobileContextMenuShowing = false;
                var space_in_mobile_ctxt_menu = 13;
                function ShowContextMenu(e, text_list, onclick_list, link_list, width) {

                    //left: -225px;   top:   -50px;
                    //width: 450px;   height: 100px;
                    var spacing_in_tbl_cell = (space_in_mobile_ctxt_menu + 1) * 2;
                    var height = 92 + (text_list.length * (23 + spacing_in_tbl_cell));
                    var top    = -1 * Math.round((height/2)); 
                    var left   = -1 * Math.round((width/2)); 

                    document.getElementById("div_modalPopupGeneric").innerHTML   = CreateMenuItems(text_list, onclick_list, link_list);
                    document.getElementById('spnModalPage_Generic').style.left   = left   + "px";
                    document.getElementById('spnModalPage_Generic').style.width  = width  + "px";
                    document.getElementById('spnModalPage_Generic').style.top    = top    + "px";
                    document.getElementById('spnModalPage_Generic').style.height = height + "px";
                    reveal_modal('modalPopup_Generic');

                    mobileContextMenuShowing = true;
                }
                function CreateMenuItems(text_list, onclick_list, link_list) {

                    var output = "<table style=\"border-collapse: separate !important; margin-left:auto !important; margin-right:auto !important; vertical-align:middle !important; text-align:center; width:100%;\" border=\"1\">";
                    for(var i=0; i<text_list.length; i++)
                        output += "<tr onmouseover=\"ChangeBgColor(this)\" onmouseout=\"RestoreBgColor(this)\"><td onclick=\"hide_modal('modalPopup_Generic'); mobileContextMenuShowing=false; " + onclick_list[i] + "\"><div style=\"height:" + space_in_mobile_ctxt_menu + "px;\"></div><b>" + text_list[i] + "</b><div style=\"height:" + space_in_mobile_ctxt_menu + "px;\"></div></td></tr>";
                    output += "</table>"

                    return output;
                }
                function ChangeBgColor(element) { element.style.backgroundColor = "#ADD8E6"; }
                function RestoreBgColor(element) { element.style.backgroundColor = "transparent"; }

                // hide the modal context menu on escape
                $( document ).on( 'keydown', function ( e ) {
                    if ( mobileContextMenuShowing && e.keyCode === 27 ) { // ESC
                        hide_modal('modalPopup_Generic');
                        mobileContextMenuShowing = false;
                    }
                });
                // hide the modal context menu on clicking outside it
                $( document ).on( 'vmousedown', function ( e ) {
                    if ( mobileContextMenuShowing && $( e.target ).closest( "#div_modalPopup_Generic" ).length === 0 ) {
                        hide_modal('modalPopup_Generic');
                        mobileContextMenuShowing = false;
                    }
                });

        </script>

        <script>

            /*
            function contextMenuClick(element, X, Y){
                var evt = element.ownerDocument.createEvent('MouseEvents');

                var RIGHT_CLICK_BUTTON_CODE = 2; // the same for FF and IE

                evt.initMouseEvent('contextmenu', true, true,
                     element.ownerDocument.defaultView, 1, 0, 0, X, Y, false,
                     false, false, false, RIGHT_CLICK_BUTTON_CODE, null);

                if (document.createEventObject){  // dispatch for IE
                    console.log("IE");
                    return element.fireEvent('onclick', evt);
                }
                else{                             // dispatch for firefox + others
                    console.log("FFox");
                    return !element.dispatchEvent(evt);
                }
            }

            var holdCords = { holdX : 0, holdY : 0 }
            $(document).on('vmousedown', function(event){
                holdCords.holdX = event.pageX;
                holdCords.holdY = event.pageY;
            });
            */

            /*
            var tapTime = 0;
            $("td.Menu_Past_Completed_Has_Invoice").on("vmousedown", function (e) {
                tapTime = new Date().getTime();
                //holdCords.holdX = e.pageX;
                //holdCords.holdY = e.pageY;
            });
            $("td.Menu_Past_Completed_Has_Invoice").on("vmouseup", function (e) {

                var duration = (new Date().getTime() - tapTime);
                if (duration > 750) {
                    //this is a tap-hold

                    var element = e.target; //document.getElementById(e.target.id);
                    console.log(element.id);
                    console.log(holdCords.holdX + ", " + holdCords.holdY);
                    //alert("e:" + element.id + ", X:" + holdCords.holdX + ", Y:" + holdCords.holdY);
                    contextMenuClick(element, holdCords.holdX, holdCords.holdY);
                } else {
                    //this is a tap
                    ;
                }
            });
            */

            /*
            $("td.Menu_Past_Completed_Has_Invoice").on("taphold", function (e) {
                
                //alert(e.target.id);


                e.stopPropagation();
                alert('b-b');
                $(e.target.id).simpledialog2({
                    mode: "blank",
                    headerText: "Image Options",
                    showModal: false,
                    headerClose: true,
                    blankContent: "<ul data-role='listview'><li><a href=''>Send to Facebook</a></li><li><a href=''>Send to Twitter</a></li><li><a href=''>Send to Cat</a></li></ul>"
                });
                alert('c-c');


                var element = e.target; //document.getElementById(e.target.id);
                alert("e:" + element.id + ", X:" + holdCords.holdX + ", Y:" + holdCords.holdY);
                console.log(element.id);
                console.log(holdCords.holdX + ", " + holdCords.holdY);
                contextMenuClick(element, holdCords.holdX, holdCords.holdY);


                if (document.createEvent) {
                    var ev = document.createEvent('HTMLEvents');
                    ev.initEvent('contextmenu', true, false);
                    element.dispatchEvent(ev);

                    e.preventDefault();
                } else { // Internet Explorer
                    element.fireEvent('oncontextmenu');
                }
            });
            */

        </script>

        <div id="div_right_click_menu" runat="server">

        <script>

            /* ------------- */
            /* --- MENUS --- */
            /* ------------- */


            $('td.Menu_Past_Completed_Has_Invoice_Clinic').contextMenu('ContextMenu_Past_Completed_Has_Invoice_Clinic', {

                bindings: {
                    'pchi_viewinvoice_cl': function (t) {
                        view_invoice(t.id, false);
                    },
                    'pchi_futurebooking_cl': function (t) {

                        // popup new booking screen..

                        var booking_id = get_booking_id_from_t_id(t.id);
                        var patient_id = ajax_get_patient_id_by_booking_id(booking_id);

                        var new_url = window.location.href;
                        new_url = set_url_field(new_url, "scroll_pos", document.getElementById('scrollValues').value);
                        new_url = set_url_field(new_url, "patient", patient_id);
                        new_url = remove_url_field(new_url, "offering");
                        new_url = remove_url_field(new_url, "date");  // remove date to make it show today, so that it's not in the past!
                        //open_new_window(new_url, -1, -1);
                        var win = window.open(new_url, '_blank');
                        win.focus();
                    },
                    'pchi_printinvoice_cl': function (t) {

                        print_invoice(t.id);
                    },
                    'pchi_emailinvoice_cl': function (t) {

                        email_invoice(t.id);
                    },
                    'pchi_reversebooking_cl': function (t) {
                        reverse_booking(t.id);
                    },
                    'pchi_printletters_cl': function (t) {
                        print_letters(t.id, false);
                    }
                }
            });

            $('td.Menu_Past_Completed_Has_Invoice_AC').contextMenu('ContextMenu_Past_Completed_Has_Invoice_AC', {

                bindings: {
                    'pchi_viewinvoice_ac': function (t) {
                        view_invoice(t.id, false);
                    },
                    'pchi_futurebooking_ac': function (t) {

                        // popup new booking screen..

                        var booking_id = get_booking_id_from_t_id(t.id);
                        var patient_id = ajax_get_patient_id_by_booking_id(booking_id);

                        var new_url = window.location.href;
                        new_url = set_url_field(new_url, "scroll_pos", document.getElementById('scrollValues').value);
                        new_url = set_url_field(new_url, "patient", patient_id);
                        new_url = remove_url_field(new_url, "offering");
                        new_url = remove_url_field(new_url, "date");  // remove date to make it show today, so that it's not in the past!
                        //open_new_window(new_url, -1, -1);
                        var win = window.open(new_url, '_blank');
                        win.focus();
                    },
                    'pchi_printinvoice_ac': function (t) {

                        print_invoice(t.id);
                    },
                    'pchi_emailinvoice_ac': function (t) {

                        email_invoice(t.id);
                    },
                    'pchi_reversebooking_ac': function (t) {
                        reverse_booking(t.id);
                    },
                    'pchi_printletters_ac': function (t) {
                        print_letters(t.id, false);
                    },
                    'pchi_treatmentlist_ac': function (t) {
                        treatment_list(t.id);
                    }

                }
            });

            $('td.Menu_Past_Completed_No_Invoice').contextMenu('ContextMenu_Past_Completed_No_Invoice', {

                bindings: {
                    'pcni_futurebooking': function (t) {

                        // popup new booking screen..

                        var booking_id = get_booking_id_from_t_id(t.id);
                        var patient_id = ajax_get_patient_id_by_booking_id(booking_id);

                        var new_url = window.location.href;
                        new_url = set_url_field(new_url, "scroll_pos", document.getElementById('scrollValues').value);
                        new_url = set_url_field(new_url, "patient", patient_id);
                        new_url = remove_url_field(new_url, "offering");
                        new_url = remove_url_field(new_url, "date");  // remove date to make it show today, so that it's not in the past!
                        //open_new_window(new_url, -1, -1);
                        var win = window.open(new_url, '_blank');
                        win.focus();
                    },
                    'pcni_printletters': function (t) {
                        print_letters(t.id, false);
                    },
                    'pcni_reversebooking': function (t) {
                        reverse_booking(t.id);
                    }
                }
            });

            $('td.Menu_Past_Uncompleted_Clinic').contextMenu('ContextMenu_Past_Uncompleted_Clinic', {

                bindings: {
                    'pu_complete_cl': function (t) {
                        show_modal_complete_booking(t.id, "complete", true);
                    },
                    'pu_viewphnum_cl': function (t) {
                        show_modal_view_ph_num(t.id);
                    },
                    'pu_printletters_cl': function (t) {
                        print_letters(t.id, false);
                    },
                    'pu_edit_cl': function (t) {
                        set_edit_booking(t.id);
                    },
                    'pu_cancel_no_fee_cl': function (t) {
                        delete_confirm_cancel_booking(t.id, "cancel");
                    },
                    'pu_cancel_w_fee_cl': function (t) {
                        show_modal_complete_booking(t.id, "cancel", true);
                    },
                    'pu_delete_cl': function (t) {
                        delete_confirm_cancel_booking(t.id, "delete");
                    },
                    'pu_deceased_cl': function (t) {
                        delete_confirm_cancel_booking(t.id, "deceased");
                    }
                }

            });

            $('td.Menu_Past_Uncompleted_AC').contextMenu('ContextMenu_Past_Uncompleted_AC', {

                bindings: {
                    'pu_complete_ac': function (t) {
                        show_modal_complete_booking(t.id, "complete", true);
                    },
                    'pu_add_patients_ac': function (t) {
                        show_modal_complete_booking(t.id, "add_patients", true);
                    },
                    'pu_viewphnum_ac': function (t) {
                        show_modal_view_ph_num(t.id);
                    },
                    'pu_printletters_ac': function (t) {
                        print_letters(t.id, false);
                    },
                    'pu_edit_ac': function (t) {
                        set_edit_booking(t.id);
                    },
                    'pu_cancel_no_fee_ac': function (t) {
                        delete_confirm_cancel_booking(t.id, "cancel");
                    },
                    'pu_cancel_w_fee_ac': function (t) {
                        show_modal_complete_booking(t.id, "cancel", true);
                    },
                    'pu_delete_ac': function (t) {
                        delete_confirm_cancel_booking(t.id, "delete");
                    },
                    'pu_deceased_ac': function (t) {
                        delete_confirm_cancel_booking(t.id, "deceased");
                    }
                }

            });

            $('td.Menu_Future_Clinic').contextMenu('ContextMenu_Future_Clinic', {

                bindings: {
                    'f_viewphnum_cl': function (t) {
                        show_modal_view_ph_num(t.id);
                    },
                    'f_printletters_cl': function (t) {
                        print_letters(t.id, false);
                    },
                    'f_edit_cl': function (t) {
                        set_edit_booking(t.id);
                    },
                    'f_delete_cl': function (t) {
                        delete_confirm_cancel_booking(t.id, "delete");
                    }
                }
            });

            $('td.Menu_Future_AC').contextMenu('ContextMenu_Future_AC', {

                bindings: {
                    'f_viewphnum_ac': function (t) {
                        show_modal_view_ph_num(t.id);
                    },
                    'f_printletters_ac': function (t) {
                        print_letters(t.id, false);
                    },
                    'f_edit_ac': function (t) {
                        set_edit_booking(t.id);
                    },
                    'f_delete_ac': function (t) {
                        delete_confirm_cancel_booking(t.id, "delete");
                    },
                    'f_add_patients_ac': function (t) {
                        show_modal_complete_booking(t.id, "add_patients", true);
                    }
                }
            });

            $('td.Menu_Future_PatientLoggedIn').contextMenu('ContextMenu_Future_PatientLoggedIn', {

                bindings: {
                    'fpt_edit': function (t) {
                        set_edit_booking(t.id);
                    },
                    'fpt_delete': function (t) {
                        delete_confirm_cancel_booking(t.id, "delete");
                    }
                }
            });




            $('td.showAddableContext').contextMenu('addableMenu', {

                bindings: {
                    'add': function (t) {
                        show_modal_add_box(t.id, false);
                    },
                    'add_multiple_days': function (t) {
                        show_modal_add_box(t.id, true);
                    }
                }
            });

            $('td.showAddableContext_PatientLoggedIn').contextMenu('addableMenu_PatientLoggedIn', {

                bindings: {
                    'add_pt_loggedin': function (t) {
                        show_modal_add_box(t.id, false);
                    }
                }
            });

            $('td.showTakenButAddableContext').contextMenu('takenButAddableMenu', {

                bindings: {
                    'taken_add': function (t) {

                        var line1 = "Warning!  This will add a booking to an unavailable time slot.";
                        var line2 = "Are you sure you want to continue?";
                        var text = line1 + "\r\n" + "\r\n" + Array((line1.length - line2.length) / 2).join(" ") + line2;
                        if (confirm(text))
                            show_modal_add_box(t.id, false);
                    },
                    'taken_add_multiple_days': function (t) {

                        var line1 = "Warning!  This will add a booking to an unavailable time slot.";
                        var line2 = "Are you sure you want to continue?";
                        var text = line1 + "\r\n" + "\r\n" + Array((line1.length - line2.length) / 2).join(" ") + line2;
                        if (confirm(text))
                            show_modal_add_box(t.id, true);
                    }
                }
            });

            $('td.showTakenButUpdatableContext').contextMenu('takenButUpdatableMenu', {

                bindings: {
                    'taken_update_newtime': function (t) {

                        var line1 = "Warning!  This will move the booking to an unavailable time slot.";
                        var line2 = "Are you sure you want to continue?";
                        var text = line1 + "\r\n" + "\r\n" + Array(Math.round((line1.length - line2.length) / 2)).join(" ") + line2;
                        if (confirm(text))
                            show_modal_add_box(t.id);
                    },
                    'taken_update_cancel_edit': function (t) {
                        cancel_edit();
                    }
                }
            });

            $('td.showTakenContext').contextMenu('takenMenu', {

                itemStyle: {
                    backgroundColor: 'gray',
                    color: 'white',
                    border: 'none',
                    padding: '1px'
                },
                itemHoverStyle: {
                    color: '#fff',
                    backgroundColor: 'gray',
                    border: 'none'
                }

            });

            $('td.showUpdatableContext').contextMenu('updateableMenu', {

                bindings: {
                    'update_newtime': function (t) {
                        show_modal_add_box(t.id);
                    },
                    'update_cancel_edit': function (t) {
                        cancel_edit();
                    }
                }
            });

            $('th.showFullDayAddableContext').contextMenu('fullDaysAddableMenu', {

                bindings: {
                    'full_days_add': function (t) {
                        show_modal_add_full_days_box(t.id);
                    },
                    'full_days_blockout_single_day': function (t) {
                        show_modal_add_full_days_box(t.id, "single_day");
                    },
                    'full_days_blockout_multiple_days': function (t) {
                        show_modal_add_full_days_box(t.id, "multiple_days");
                    },
                    'full_days_blockout_multiple_days_series': function (t) {
                        show_modal_add_full_days_box(t.id, "multiple_days_series");
                    },
                    'full_days_paid_blockout_single_day': function (t) {
                        show_modal_add_full_days_box(t.id, "paid_single_day");
                    },
                    'full_days_paid_blockout_multiple_days': function (t) {
                        show_modal_add_full_days_box(t.id, "paid_multiple_days");
                    },
                    'full_days_move': function (t) {
                        set_edit_day(t.id);
                    }
                }
            });

            $('th.showFullDayUpdatableContext').contextMenu('fullDaysUpdatableMenu', {

                bindings: {
                    'update_fullday': function (t) {
                        change_full_days_bookings(t.id);
                    }
                }
            });

            $('td.showFullDayTakenContext').contextMenu('fullDayEditable', {

                bindings: {
                    'full_days_make_booking': function (t) {

                        var line1 = "Warning!  This will add a booking to an unavailable time slot.";
                        var line2 = "Are you sure you want to continue?";
                        var text = line1 + "\r\n" + "\r\n" + Array((line1.length - line2.length) / 2).join(" ") + line2;
                        if (confirm(text))
                            show_modal_add_box(t.id);
                    },
                    'full_days_delete': function (t) {
                        delete_confirm_cancel_booking(t.id, "delete");
                    }
                }
            });


            $('td.emptyContext').contextMenu('emptyContextMenu', {

            });
            $('th.emptyContext').contextMenu('emptyContextMenu', {

            });


            $('td.showPatientAndServiceNotSetContext').contextMenu('patientAndServiceNotSetMenu', {

                itemStyle: {
                    backgroundColor: 'gray',
                    color: 'white',
                    border: 'none',
                    padding: '1px'
                },
                itemHoverStyle: {
                    color: '#fff',
                    backgroundColor: 'gray',
                    border: 'none'
                }

            });
            $('th.showPatientAndServiceNotSetContext').contextMenu('patientAndServiceNotSetMenu', {

                itemStyle: {
                    backgroundColor: 'gray',
                    color: 'white',
                    border: 'none',
                    padding: '1px'
                },
                itemHoverStyle: {
                    color: '#fff',
                    backgroundColor: 'gray',
                    border: 'none'
                }

            });

            $('td.showPatientNotSetContext').contextMenu('patientNotSetMenu', {

                itemStyle: {
                    backgroundColor: 'gray',
                    color: 'white',
                    border: 'none',
                    padding: '1px'
                },
                itemHoverStyle: {
                    color: '#fff',
                    backgroundColor: 'gray',
                    border: 'none'
                }

            });
            $('th.showPatientNotSetContext').contextMenu('patientNotSetMenu', {

                itemStyle: {
                    backgroundColor: 'gray',
                    color: 'white',
                    border: 'none',
                    padding: '1px'
                },
                itemHoverStyle: {
                    color: '#fff',
                    backgroundColor: 'gray',
                    border: 'none'
                }

            });

            $('td.showServiceNotSetContext').contextMenu('serviceNotSetMenu', {

                itemStyle: {
                    backgroundColor: 'gray',
                    color: 'white',
                    border: 'none',
                    padding: '1px'
                },
                itemHoverStyle: {
                    color: '#fff',
                    backgroundColor: 'gray',
                    border: 'none'
                }

            });
            $('th.showServiceNotSetContext').contextMenu('serviceNotSetMenu', {

                itemStyle: {
                    backgroundColor: 'gray',
                    color: 'white',
                    border: 'none',
                    padding: '1px'
                },
                itemHoverStyle: {
                    color: '#fff',
                    backgroundColor: 'gray',
                    border: 'none'
                }

            });

        </script>

        </div>


</asp:Content>



