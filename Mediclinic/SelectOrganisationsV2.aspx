<%@ Page Title="Select Organisations" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="SelectOrganisationsV2.aspx.cs" Inherits="SelectOrganisationsV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/booking_modal_box.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery-1.4.1.js" ></script>
    <script type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript" src="Scripts/FUNZIONANTE.js"></script>
    <script type="text/javascript" src="Scripts/bookingsV2.js"></script>
    <script type="text/javascript" src="Scripts/get_details_of_person.js"></script>


    <script type="text/javascript">
        function select_organisation(val) {
            window.returnValue = val;
            self.close();
        }

        function check_selected_atleast_one() {

            var count = 0;
            var c = document.getElementsByTagName('input');
            for (var i = 0; i < c.length; i++)
                if (c[i].type == 'checkbox' && c[i].id.indexOf('chkSelect') !== -1 && c[i].checked)
                    count++;

            //alert(count);

            if (count == 0)
                alert("Please use checkboxes and select at least one organisation \r\nOr click on a link for a single organisation");

            return count > 0;
        }

        function highlight_row(chkBox) {  // doesnt pass in a checkbox -- read first comment below
            // asp:CheckBox control doesn't have a onchange event, and onchange event will be rendered in a <span> tag and not the <input> tag. 
            // so get parent, then get the control

            var gvDrv = document.getElementById("<%= GrdOrganisation.ClientID %>");

            for (i = 1; i < gvDrv.rows.length; i++) {

                // dont do all the processing if its not the row
                if (chkBox != null && gvDrv.rows[i] != chkBox.parentNode.parentNode)
                    continue;

                    // if it is the row, process than return out of the function
                else {

                    var cells = gvDrv.rows[i].cells;
                    for (j = 0; j < cells.length; j++) {
                        var HTML = cells[j].innerHTML;

                        if (chkBox != null && cells[j] != chkBox.parentNode)
                            continue; // alert("found");

                        if (HTML.indexOf("chkSelect") != -1) {
                            var lblID = cells[0].getElementsByTagName("*")[0];
                            var chkSelect = cells[j].getElementsByTagName("*")[1];  // first item is the onchange event rendered as a div, so get 2nd item

                            var cells2 = gvDrv.rows[i].cells;
                            for (j2 = 0; j2 < cells2.length; j2++)
                                cells2[j2].style.backgroundColor = chkSelect.checked ? '#FAFAD2' : '';  // LightGoldenrodYellow 
                        }
                    }

                    if (chkBox != null)
                        return;
                }
            }
        }


        // ---------------------------------------------------------------------------------------------

        function live_search(str) {

            if (str.length == 0) {
                document.getElementById("div_livesearch").innerHTML = "";
                document.getElementById("div_livesearch").style.border = "0px";
                document.getElementById("div_livesearch").style.display = "none";
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
                    document.getElementById("div_livesearch").style.display = "";

                    if (document.getElementById("txtSearchFullName").value.length == 0) {
                        document.getElementById("div_livesearch").innerHTML = "";
                        document.getElementById("div_livesearch").style.border = "0px";
                        document.getElementById("div_livesearch").style.display = "none";
                        return;
                    }
                }
            }

            // need to remove patient id from url, then use below to put in current url with patient id as below
            // and add in scroll pos, etc

            var new_url = window.location.href;
            new_url = set_url_field(new_url, "patient", "[patient_id]");

            var ddlDaysToDisplay = document.getElementById("ddlDaysToDisplay");
            new_url = set_url_field(new_url, "ndays", ddlDaysToDisplay.options[ddlDaysToDisplay.selectedIndex].value);

            var dt = get_date(document.getElementById("txtStartDate").value);
            new_url = (dt == null) ?
                remove_url_field(new_url, 'date ') :
                new_url = set_url_field(new_url, "date", get_date_urlstring(dt));

            xmlhttp.open("GET", "/AJAX/AjaxLivePatientSurnameSearch.aspx?q=" + str + "&max_results=80&link_href=" + encodeURIComponent(new_url) + "&link_onclick=", true);
            xmlhttp.send();
        }

        function clear_live_search() {
            document.getElementById("div_livesearch").innerHTML = "";
            document.getElementById("div_livesearch").style.border = "0px";
            document.getElementById("div_livesearch").style.display = "none";
            document.getElementById("txtSearchFullName").value = "";
            document.getElementById("txtSearchFullName").style.backgroundImage = "url('/images/textbox_watermark_surname_first.png')";
        }

        function clear_selected_patient() {
            var new_url = window.location.href;
            new_url = remove_url_field(new_url, "patient");

            var ddlDaysToDisplay = document.getElementById("ddlDaysToDisplay");
            new_url = set_url_field(new_url, "ndays", ddlDaysToDisplay.options[ddlDaysToDisplay.selectedIndex].value);

            var dt = get_date(document.getElementById("txtStartDate").value);
            new_url = (dt == null) ?
                remove_url_field(new_url, 'date ') :
                new_url = set_url_field(new_url, "date", get_date_urlstring(dt));

            window.location.href = new_url;
        }

        function set_watermark(txtbox, val) {
            txtbox.style.backgroundImage = (txtbox.value.length == 0 && val) ? "url('/images/textbox_watermark_surname_first.png')" : "";
        }

        function reload_page_ndays() {
            var new_url = window.location.href;
            var ddlDaysToDisplay = document.getElementById("ddlDaysToDisplay");
            new_url = set_url_field(new_url, "ndays", ddlDaysToDisplay.options[ddlDaysToDisplay.selectedIndex].value);
            window.location.href = new_url;
        }

        function reload_page_date() {

            var start_date_text = document.getElementById('txtStartDate').value;
            var valid_date_regex = /^\d{2}\-\d{2}\-\d{4}$/;
            var valid_start_date = valid_date_regex.test(start_date_text);

            var new_start_date = "";
            if (valid_start_date) {
                var mySplitResult = String(start_date_text).split("-");
                var yr = String(mySplitResult[2]);
                var mo = String(mySplitResult[1]);
                var day = String(mySplitResult[0]);
                new_start_date = yr + '_' + mo + '_' + day;
            }

            var new_url = window.location.href;
            if (new_start_date.length == 0)
                new_url = remove_url_field(new_url, 'date ');
            else
                new_url = set_url_field(new_url, "date", new_start_date);

            window.location.href = new_url;
        }

        // ---------------------------------------------------------------------------------------------

        /*
        document.onkeydown = keyEvent;
        document.onkeyup   = keyEvent;

        var ctrlPressed  = 0;
        var altPressed   = 0;
        var shiftPressed = 0;

        function keyEvent(e) {

            if (parseInt(navigator.appVersion) > 3) {

                var evt = e ? e : window.event;

                if (document.layers && navigator.appName == "Netscape"
                    && parseInt(navigator.appVersion) == 4) {
                    // NETSCAPE 4 CODE
                    var mString  = (e.modifiers + 32).toString(2).substring(3, 6);
                    shiftPressed = (mString.charAt(0) == "1");
                    ctrlPressed  = (mString.charAt(1) == "1");
                    altPressed   = (mString.charAt(2) == "1");
                    self.status  = "modifiers=" + e.modifiers + " (" + mString + ")"
                }
                else {
                    // NEWER BROWSERS [CROSS-PLATFORM]
                    shiftPressed = evt.shiftKey;
                    altPressed   = evt.altKey;
                    ctrlPressed  = evt.ctrlKey;
                    self.status  = ""
                     + "shiftKey=" + shiftPressed
                     + ", altKey=" + altPressed
                     + ", ctrlKey=" + ctrlPressed
                }


                document.getElementById("hiddenCtrlDown").value  = ctrlPressed  ? "1" : "0";
                document.getElementById("hiddenAltDown").value   = altPressed   ? "1" : "0";
                document.getElementById("hiddenShiftDown").value = shiftPressed ? "1" : "0";

                //if (shiftPressed || altPressed || ctrlPressed)
                //    alert("Mouse clicked with the following keys:\n"
                //     + (shiftPressed ? "Shift " : "")
                //     + (altPressed ? "Alt " : "")
                //     + (ctrlPressed ? "Ctrl " : "")
                //    )

                //document.getElementById("txtText").value =
                //    "ctrlPressed:  " + ctrlPressed  + "\n" +
                //    "altPressed:   " + altPressed   + "\n" +
                //    "shiftPressed: " + shiftPressed + "\n";
            }

            if (ctrlPressed || altPressed || shiftPressed)
                return false;

            return true;
        }
        */

        // ---------------------------------------------------------------------------------------------

        function set_date_today(txtbox) {
            set_date(new Date());
        }

        function add_to_date(direction) {

            var dt = get_date(document.getElementById("txtStartDate").value);
            if (dt == null) {
                alert("Invalid date - must be of the type dd-mm-yyyy and be a real date.");
                return;
            }


            var ddlMoveDateNum = document.getElementById("ddlMoveDateNum");
            var num = parseInt(ddlMoveDateNum.options[ddlMoveDateNum.selectedIndex].value, 10);

            var ddlMoveDateType = document.getElementById("ddlMoveDateType");
            var type = ddlMoveDateType.options[ddlMoveDateType.selectedIndex].value;

            if (direction == "Backwards")
                num *= -1;

            if (type == "Days")
                dt.setDate(dt.getDate() + num);
            else if (type == "Weeks")
                dt.setDate(dt.getDate() + 7 * num);
            else if (type == "Months")
                dt.setMonth(dt.getMonth() + num);
            else if (type == "Years")
                dt.setFullYear(dt.getFullYear() + num);

            set_date(dt);
        }

        function set_date(dt) {
            document.getElementById("txtStartDate").value = get_date_string(dt);
        }

        function get_date_string(dt) {
            var dd = dt.getDate();
            var mm = dt.getMonth() + 1; //January is 0!
            var yyyy = dt.getFullYear();

            if (dd < 10) { dd = '0' + dd }
            if (mm < 10) { mm = '0' + mm }
            return dd + '-' + mm + '-' + yyyy;
        }
        function get_date_urlstring(dt) {
            var dd = dt.getDate();
            var mm = dt.getMonth() + 1; //January is 0!
            var yyyy = dt.getFullYear();

            if (dd < 10) { dd = '0' + dd }
            if (mm < 10) { mm = '0' + mm }
            return yyyy + '_' + mm + '_' + dd;
        }

        function get_date(text) {  // return null if not valid date

            // first test fits regex
            var re = /^\d\d\-\d\d\-\d\d\d\d$/;
            if (!text.match(re)) {
                return null;
            }

            // now check is real date, not invalid such as 31 feb
            var comp = text.split('-');
            var d = parseInt(comp[0], 10);
            var m = parseInt(comp[1], 10);
            var y = parseInt(comp[2], 10);
            var date = new Date(y, m - 1, d); //January is 0!

            if (date.getFullYear() == y && date.getMonth() + 1 == m && date.getDate() == d)
                return date;
            else
                return null;
        }

        // ---------------------------------------------------------------------------------------------

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Select Organisations To Show On The Booking Sheet</span></div>
        <div class="main_content_with_header">
            <div class="user_login_form">


                <div class="border_top_bottom">
                    <center>

                        <!-- unused becuase firefox still detects shift/alt for linkbuttons and opens an empty page -->
                        <asp:HiddenField ID="hiddenCtrlDown"  runat="server" Value="0" />
                        <asp:HiddenField ID="hiddenAltDown"   runat="server" Value="0" />
                        <asp:HiddenField ID="hiddenShiftDown" runat="server" Value="0" />


                        <table class="block_center" style="vertical-align:top;">
                            <tr>
                                <td class="nowrap"><asp:Label ID="lblGetPatientLabelText" runat="server">Get Patient</asp:Label></td>
                                <td class="nowrap" style="width:10px"></td>
                                <td class="nowrap">
                                    <asp:TextBox ID="txtSearchFullName" runat="server" placeholder="Enter Surname" onkeyup="live_search(this.value)" autocomplete="off" onkeydown="return (event.keyCode!=13);" ></asp:TextBox>
                                    <div id="div_livesearch" style="display:none;position:absolute;background:#FFFFFF;"></div>
                                </td>
                                <td class="nowrap" style="width:5px"></td>
                                <td class="nowrap"><button type="button" name="btnClearFullNameSearch" id="btnClearFullNameSearch" runat="server" onclick="clear_live_search(); return false;">Clear</button></td>
                                <td class="nowrap" id="pt_table_seperator" runat="server" style="min-width:60px"></td>

                                <td>
                                    <table class="block_center">
                                        <tr>
                                            <td>Start Date:</td>
                                            <td><asp:TextBox ID="txtStartDate" runat="server" Columns="10"></asp:TextBox></td>
                                            <td><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                        </tr>
                                    </table>
                                </td>


                            </tr>
                            <tr>
                                <td class="nowrap"><asp:Label ID="lblSelectedPatientLabelText" runat="server">Selected Patient</asp:Label></td>
                                <td class="nowrap"></td>
                                <td class="nowrap"><asp:Label ID="lblSelectedPatientName" runat="server"></asp:Label></td>
                                <td class="nowrap"></td>
                                <td class="nowrap"><asp:Button ID="btnClearSelectedPatient" runat="server" Text="Clear" OnClientClick="javascript:clear_selected_patient();return false;" /></td>
                                <td class="nowrap"></td>

                                <td>

                                    <table class="block_center">
                                        <tr>
                                            <td><asp:ImageButton ID="btnMoveDateToday" runat="server" CommandArgument="Today" OnClientClick="set_date_today();return false;"   OnCommand="btnMoveDate_Command" ImageUrl="~/images/today_blue.png" AlternateText="Go" /></td>
                                            <td style="min-width:35px;"></td>
                                            <td><asp:ImageButton ID="btnMoveDateBackwards" runat="server" OnClientClick="add_to_date('Backwards');return false;" CommandArgument="Backwards" OnCommand="btnMoveDate_Command" ImageUrl="~/images/arrow_left_16.png" AlternateText="Go" /></td>
                                            <td style="width:8px;"></td>
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
                                            <td style="width:8px;"></td>
                                            <td><asp:ImageButton ID="btnMoveDateForwards" runat="server" OnClientClick="add_to_date('Forwards');return false;" CommandArgument="Forwards" OnCommand="btnMoveDate_Command" ImageUrl="~/images/arrow_right_16.png" AlternateText="Go" /></td>
                                        </tr>

                                    </table>

                                </td>

                            </tr>
                            <tr>
                                <td colspan="6"></td>

                                <td style="text-align:center;">

                                    Days To Display: <asp:DropDownList ID="ddlDaysToDisplay" runat="server"></asp:DropDownList>
                                    <br />

                                    <span style="line-height:5px;">&nbsp;</span>

                                    <span style="color:red;text-align:center;">
                                        * Fewer days = faster booking sheet loading.
                                    </span>


                                </td>
                            </tr>
                        </table>


                    </center>
                </div>

            </div>

            <div class="block_center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdOrganisation" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="organisation_id" 
                        OnRowCancelingEdit="GrdOrganisation_RowCancelingEdit" 
                        OnRowDataBound="GrdOrganisation_RowDataBound" 
                        OnRowEditing="GrdOrganisation_RowEditing" 
                        OnRowUpdating="GrdOrganisation_RowUpdating" ShowFooter="False" 
                        OnRowCommand="GrdOrganisation_RowCommand" 
                        OnRowDeleting="GrdOrganisation_RowDeleting" 
                        OnRowCreated="GrdOrganisation_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GrdOrganisation_Sorting"
                        RowStyle-VerticalAlign="top"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center select_organisations_table">


                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("organisation_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="name" Visible="false"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkName" runat="server" Text='<%# Bind("name") %>'></asp:HyperLink> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="name" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:LinkButton ID="lnkName2" runat="server" Text='<%# Bind("name") %>' OnCommand="lnkName_Command" CommandArgument='<%# Bind("organisation_id") %>' ></asp:LinkButton> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_type_id" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblType" runat="server" Text='<%# Eval("type_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Select" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Center"> 
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" Text="" onchange="highlight_row(this);" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 
                    </asp:GridView>

                </div>
            </center>

            <center>
                <asp:Button ID="btnBookingSheet" runat="server" Text="Booking Sheet" OnClick="btnBookingSheet_Click" OnClientClick="javascript: if (!check_selected_atleast_one()) return false;" />
            </center>

        </div>
    </div>


</asp:Content>



