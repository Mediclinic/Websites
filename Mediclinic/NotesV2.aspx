<%@ Page Title="Cost Centres" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="NotesV2.aspx.cs" Inherits="NotesV2" ValidateRequest="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function show_tab(tabName) {
            show_hide('tab1', false);
            show_hide('tab2', false);
            show_hide('tab3', false);
            show_hide(tabName, true);

            /*
            document.getElementById('link_tab1').style.fontWeight = 'normal';
            document.getElementById('link_tab2').style.fontWeight = 'normal';
            document.getElementById('link_tab3').style.fontWeight = 'normal';
            document.getElementById('link_' + tabName).style.fontWeight = 'bold';
            */

            document.getElementById('link_tab1').style.textDecoration = "";
            document.getElementById('link_tab2').style.textDecoration = "";
            document.getElementById('link_tab3').style.textDecoration = "";
            document.getElementById('link_' + tabName).style.textDecoration = "underline";


            var heading = document.getElementById('lblHeading_' + tabName).innerHTML;
            change_heading_text(heading);

            document.getElementById("hiddenFieldSelectedTab").value = tabName;
        }

        function change_heading_text(text) {
            document.getElementById('lblHeading').innerHTML = text;
        }





        function check_selected_atleast_one() {

            var count = 0;
            var c = document.getElementsByTagName('input');
            for (var i = 0; i < c.length; i++)
                if (c[i].type == 'checkbox' && c[i].id.indexOf('chkPrint') !== -1 && c[i].checked)
                    count++;

            //alert(count);

            if (count == 0)
                alert("Please use checkboxes and select at least one note to print.");

            return count > 0;
        }

        function highlight_row(chkBox, grdViewNbr) {  // doesnt pass in a checkbox -- read first comment below
            // asp:CheckBox control doesn't have a onchange event, and onchange event will be rendered in a <span> tag and not the <input> tag. 
            // so get parent, then get the control


            var gvDrv = '';
            if (grdViewNbr == 1)
                gvDrv = document.getElementById("<%= GrdNote1.ClientID %>");
            else if (grdViewNbr == 2)
                gvDrv = document.getElementById("<%= GrdNote2.ClientID %>");
            else if (grdViewNbr == 3)
                gvDrv = document.getElementById("<%= GrdNote3.ClientID %>");


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

                        if (HTML.indexOf("chkPrint") != -1) {
                            var lblID = cells[0].getElementsByTagName("*")[0];
                            var chkSelect = cells[j].getElementsByTagName("*")[1];  // first item is the onchange event rendered as a div, so get 2nd item

                            var cells2 = gvDrv.rows[i].cells;
                            for (j2 = 0; j2 < cells2.length; j2++)
                                cells2[j2].style.backgroundColor = chkSelect.checked ? "#FFCC00" : '';  // '#FAFAD2' LightGoldenrodYellow 
                        }
                    }

                    if (chkBox != null)
                        return;
                }
            }
        }

        function getCookie(c_name) {
            var c_value = document.cookie;
            var c_start = c_value.indexOf(" " + c_name + "=");
            if (c_start == -1) {
                c_start = c_value.indexOf(c_name + "=");
            }
            if (c_start == -1) {
                c_value = null;
            }
            else {
                c_start = c_value.indexOf("=", c_start) + 1;
                var c_end = c_value.indexOf(";", c_start);
                if (c_end == -1) {
                    c_end = c_value.length;
                }
                c_value = unescape(c_value.substring(c_start, c_end));
            }
            return c_value;
        }
        function setCookie(c_name, value, exmins) {
            var exdate = new Date();
            exdate.setMinutes(exdate.getMinutes() + exmins);
            var c_value = escape(value) + ((exmins == null) ? "" : "; expires=" + exdate.toUTCString());
            document.cookie = c_name + "=" + c_value;
        }
        function deleteCookie(c_name) {
            date = new Date();
            date.setDate(date.getDate() - 1);
            document.cookie = escape(c_name) + '=;expires=' + date.toUTCString();
        }


        function set_note(txtBox, user_id, entity_id) {
            setCookie(txtBox.id + "_" + user_id + "_" + entity_id + "_" + "new_note", txtBox.value, 180);
        }
        function load_note(txtBox, user_id, entity_id) {
            var txt = getCookie(txtBox.id + "_" + user_id + "_" + entity_id + "_" + "new_note");
            if (txt != null)
                txtBox.value = getCookie(txtBox.id + "_" + user_id + "_" + entity_id + "_" + "new_note");
        }
        function clear_note(txtBox, user_id, entity_id) {
            setCookie(txtBox.id + "_" + user_id + "_" + entity_id + "_" + "new_note", "", -1440);
        }
        function alert_note(txtBox, user_id, entity_id) {
            alert(getCookie(txtBox.id + "_" + user_id + "_" + entity_id + "_" + "new_note"));
        }



        function send_email() {

            var openWindow = window.open('SendEmail.aspx', 'Email', 'width=675px,height=675px,left=100,top=100,center=yes,resizable=no,scrollbars=no');
            if (openWindow) {
                openWindow.onload = function () {
                    openWindow.document.getElementById("FreeTextBox1").value = document.getElementById('emailText').value;
                }
            }

            //var param = { 'body_text': document.getElementById('emailText').value };
            //OpenWindowWithPost("SendEmail.aspx",
            //  "width=675px,height=675px,left=100,top=100,center=yes,resizable=no,scrollbars=no",
            //  "Email", param);
        }
        function OpenWindowWithPost(url, windowoption, name, params) {
            var form = document.createElement("form");
            form.setAttribute("method", "post");
            form.setAttribute("action", url);
            form.setAttribute("target", name);

            for (var i in params) {
                if (params.hasOwnProperty(i)) {
                    var input = document.createElement('input');
                    input.type = 'hidden';
                    input.name = i;
                    input.value = params[i];
                    form.appendChild(input);
                }
            }

            document.body.appendChild(form);

            //note I am using a post.htm page since I did not want to make double request to the page 
            //it might have some Page_Load call which might screw things up.
            window.open("post.htm", name, windowoption);

            form.submit();

            document.body.removeChild(form);
        }

        function select_body_part(v) {
            var editModeDDL = null;
            for (var i = 0; i < 100; i++)
                if (document.getElementById('MainContent_GrdNote2_ddlBodyPart_' + i) != null)
                    editModeDDL = document.getElementById('MainContent_GrdNote2_ddlBodyPart_' + i)

            if (editModeDDL == null)
                selectItemByValue(document.getElementById('MainContent_GrdNote2_ddlNewBodyPart'), String(v))
            else
                selectItemByValue(editModeDDL, String(v))
        }
        function selectItemByValue(elmnt, v) {
            for (var i = 0; i < elmnt.options.length; i++) {
                if (elmnt.options[i].value === v) {
                    elmnt.selectedIndex = i;
                    break;
                }
            }
        }


        function open_history(url) {
            var isMobileDevice = document.getElementById('hiddenIsMobileDevice').value == "1";
            if (!isMobileDevice)
                window.showModalDialog(url, '', 'dialogWidth:900px;dialogHeight:520px;center:yes;resizable:no; scroll:no');
            else
                open_new_tab(url);
        }

        function show_hide_deleted_notes() {
            elements = document.getElementsByClassName('deleted_note');
            if (elements.length == 0)
                return;
            var show = elements[0].style.display == "none";
            for (var i = 0; i < elements.length; i++) {
                elements[i].style.display = show ? "" : "none";
            }
        }


    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <asp:HiddenField ID="hiddenFieldSelectedTab" runat="server" />

    <div class="clearfix">
        <div class="page_title">

            <center>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblHeading" runat="server">Notes</asp:Label>
                        </td>
                        <td style="width:25px;"></td>
                        <td style="text-align:center;">
                            <asp:Label ID="lnkToEntity" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </center>
            

        </div>
        <div class="main_content" style="padding:10px 5px;">

            <asp:HiddenField ID="hiddenIsMobileDevice" runat="server" Value="0" />

            <center>

                <asp:ValidationSummary ID="ValidationSummaryAdd1" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd1"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit1" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit2"/>
                <asp:ValidationSummary ID="ValidationSummaryAdd2" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd2"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit2" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit2"/>
                <asp:ValidationSummary ID="ValidationSummaryAdd3" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd3"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit3" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit3"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>

                <asp:HiddenField ID="userID" runat="server" />
                <asp:HiddenField ID="entityID" runat="server" />
                <asp:HiddenField ID="emailText" runat="server" />

                 <table id="maintable" runat="server">
                     <tr>
                         <td>

                           <div id="autodivheight" class="divautoheight" style="min-height:235px; height:500px; width: auto; padding-right: 17px;">

                               
                            <table style="margin:auto;">
                                <tr>
                                    <td style="width:80px;"></td>
                                    <td style="vertical-align:bottom !important;">
                                        <table class="pt_page_nav">
                                            <tr class="pt_page_nav">

                                                <td style="text-align:center;vertical-align:middle;margin-left:8px;" class="pt_page_nav">
                                                    <a id="link_tab1" href="javascript:void(0)"  onclick="show_tab('tab1');return false;" class="pt_page_nav" style="outline:none;"><asp:Label ID="lblHeading_tab1" runat="server">Referrer Notes</asp:Label></a>
                                                </td>

                                                <td class="pt_page_nav" style="vertical-align:middle;margin-left:8px;margin-right:8px;">
                                                    <div style=" width: 1px !important; height:20px; background-color:#608080; margin:0 3px;"></div>
                                                </td>

                                                <td style="text-align:center;vertical-align:middle;" class="pt_page_nav">
                                                    <a id="link_tab3" href="javascript:void(0)"  onclick="show_tab('tab3');return false;" class="pt_page_nav" style="outline:none;"><asp:Label ID="lblHeading_tab3" runat="server">Session Notes</asp:Label></a>
                                                </td>

                                                <td class="pt_page_nav" style="vertical-align:middle;margin-left:8px;margin-right:8px;">
                                                    <div style=" width: 1px !important; height:20px; background-color:#608080; margin:0 3px;"></div>
                                                </td>

                                                <td style="text-align:center;vertical-align:middle;margin-right:8px;" class="pt_page_nav">
                                                    <a id="link_tab2" href="javascript:void(0)"  onclick="show_tab('tab2');return false;" class="pt_page_nav" style="outline:none;"><asp:Label ID="lblHeading_tab2" runat="server">Body Chart</asp:Label></a>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="min-width:20px;"></td>
                                    <td>
                                        <table style="text-align:center;line-height: 18px !important;">
                                            <tr>
                                                <td colspan="2"><asp:Label ID="lblSterilisationCode" runat="server">Sterilisation Code</asp:Label> </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtSterilisationCode" runat="server"  Columns="10"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnUpdateSterilisationCode" runat="server" Text="Save" OnClick="btnUpdateSterilisationCode_Click" CssClass="thin_button" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="min-width:20px;"></td>
                                    <td>
                                        <asp:CheckBox ID="chkInformedConcent" runat="server" Text="PT Consent&nbsp;" TextAlign="Left" AutoPostBack="true" OnCheckedChanged="chkInformedConcent_CheckedChanged" />
                                    </td>
                                </tr>
                                <tr style="line-height:normal;">
                                    <td></td>
                                    <td style="text-align:center;">
                                        <a href="javascript:void(0)"  onclick="show_hide_deleted_notes(); return false;">Show/Hide Deleted Notes</a>
                                    </td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                            </table>

                            <div style="height:4px;"></div>

                            <table style="width:100%;vertical-align:top;">
                                <tr id="tab1" style="">
                                    <td>
                                        <center>

                                            <asp:GridView ID="GrdNote1" runat="server" 
                                                 AutoGenerateColumns="False" DataKeyNames="note_id" 
                                                 OnRowCancelingEdit="GrdNote1_RowCancelingEdit" 
                                                 OnRowDataBound="GrdNote1_RowDataBound" 
                                                 OnRowEditing="GrdNote1_RowEditing" 
                                                 OnRowUpdating="GrdNote1_RowUpdating" ShowFooter="True" 
                                                 OnRowCommand="GrdNote1_RowCommand" 
                                                 OnRowDeleting="GrdNote1_RowDeleting" 
                                                 OnRowCreated="GrdNote1_RowCreated"
                                                 AllowSorting="True" 
                                                 OnSorting="GrdNote1_Sorting"
                                                 RowStyle-VerticalAlign="top"
                                                 ClientIDMode="Predictable"
                                                 CssClass="table table-bordered table-white table-grid table-grid-top-bottum-padding-normal auto_width block_center vertical_align_top">
                                                <Columns> 

                                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="note_id"> 
                                                        <EditItemTemplate> 
                                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("note_id") %>'></asp:Label>
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("note_id") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                    </asp:TemplateField>
                
                                                    <asp:TemplateField HeaderText="Entity"  HeaderStyle-HorizontalAlign="Left" SortExpression="entity_id">
                                                        <EditItemTemplate> 
                                                            <asp:Label ID="lblEntityID" runat="server" Text='<%# Bind("entity_id") %>'></asp:Label>
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblEntityID" runat="server" Text='<%# Bind("entity_id") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="date_added" FooterStyle-VerticalAlign="Top" FooterStyle-Wrap="False" ItemStyle-Wrap="false">
                                                        <EditItemTemplate> 
                                                            <asp:DropDownList ID="ddlDate_Day" runat="server"/> <asp:DropDownList ID="ddlDate_Month" runat="server"/> <asp:DropDownList ID="ddlDate_Year" runat="server"/>
                                                            <asp:CustomValidator ID="ddlDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                                                ControlToValidate="ddlDate_Day"
                                                                OnServerValidate="DateAllOrNoneCheck"
                                                                ErrorMessage="Date must be valid with each of day/month/year selected"
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryEdit1">*</asp:CustomValidator>
                                                            <asp:Label ID="lblAddedBy" runat="server" Text='<%#  Eval("added_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Added By\">A</a>: " + Eval("added_by_person_firstname") + " " + Eval("added_by_person_surname") %>' ></asp:Label> 
                                                            <asp:Label ID="lblModifiedBy" runat="server" Visible='<%# Eval("last_modified_note_info_visible") %>' Text='<%#  Eval("modified_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Last Modified By\">M</a>: " + Eval("modified_by_person_firstname") + " " + Eval("modified_by_person_surname") %>' ></asp:Label> 
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("date_added") == DBNull.Value ? "" : Convert.ToDateTime(Eval("date_added")).ToString("dd / MM / yyyy") %>' ></asp:Label> 
                                                            <asp:Label ID="lblAddedBy" runat="server" Text='<%#  Eval("added_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Added By\">A</a>: " + Eval("added_by_person_firstname") + " " + Eval("added_by_person_surname") %>' ></asp:Label> 
                                                            <asp:Label ID="lblModifiedBy" runat="server" Visible='<%# Eval("last_modified_note_info_visible") != DBNull.Value && Convert.ToBoolean(Eval("last_modified_note_info_visible")) %>' Text='<%#  Eval("modified_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Last Modified By\">M</a>: " + Eval("modified_by_person_firstname") + " " + Eval("modified_by_person_surname") %>' ></asp:Label> 
                                                        </ItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:DropDownList ID="ddlNewDate_Day" runat="server"/> <asp:DropDownList ID="ddlNewDate_Month" runat="server"/> <asp:DropDownList ID="ddlNewDate_Year" runat="server"/>
                                                            <asp:CustomValidator ID="ddlNewDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                                                ControlToValidate="ddlNewDate_Day"
                                                                OnServerValidate="DateAllOrNoneCheck"
                                                                ErrorMessage="Date must be valid with each of day/month/year selected"
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryAdd1">*</asp:CustomValidator>
                                                        </FooterTemplate> 
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="note_type_descr" FooterStyle-VerticalAlign="Top"> 
                                                        <EditItemTemplate> 
                                                            <asp:DropDownList ID="ddlNoteType" runat="server" DataTextField="descr" DataValueField="note_type_id"> </asp:DropDownList> 
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <div style="text-align:left;">
                                                                <asp:Label ID="lblNoteType" runat="server" Text='<%# Eval("note_type_descr") %>'></asp:Label> 
                                                            </div>
                                                        </ItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:DropDownList ID="ddlNewNoteType" runat="server" DataTextField="descr" DataValueField="note_type_id"> </asp:DropDownList>
                                                        </FooterTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="Body Part" HeaderStyle-HorizontalAlign="Left" SortExpression="body_part_descr" FooterStyle-VerticalAlign="Top"> 
                                                        <EditItemTemplate> 
                                                            <asp:DropDownList ID="ddlBodyPart" runat="server" DataTextField="descr" DataValueField="body_part_id"></asp:DropDownList>
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblBodyPart" runat="server" Text='<%# Eval("body_part_descr") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:DropDownList ID="ddlNewBodyPart" runat="server" DataTextField="descr" DataValueField="body_part_id"></asp:DropDownList>
                                                        </FooterTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="Note" HeaderStyle-HorizontalAlign="Left" SortExpression="text" FooterStyle-VerticalAlign="Top"> 
                                                        <EditItemTemplate> 
                                                            <asp:TextBox ID="txtText" TextMode="multiline" rows="6" style="width:375px;" runat="server" Text='<%# Bind("text") %>'></asp:TextBox> 
                                                            <asp:RequiredFieldValidator ID="txtValidateTextRequired" runat="server" CssClass="failureNotification"  
                                                                ControlToValidate="txtText" 
                                                                ErrorMessage="Text is required."
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryEdit1">*</asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="txtValidatetxtTextRegex" runat="server" CssClass="failureNotification"
                                                                ControlToValidate="txtText"
                                                                ValidationExpression="^[^<>]+$"
                                                                ErrorMessage="The following letters are not permitted: '<', '>'"
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryEdit1" Enabled="false">*</asp:RegularExpressionValidator>
                                                        </EditItemTemplate> 
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="txtNewText" TextMode="multiline" rows="6" style="width:375px" runat="server" ></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="txtValidateNewTextRequired" runat="server" CssClass="failureNotification"  
                                                                ControlToValidate="txtNewText" 
                                                                ErrorMessage="Note Text is required."
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryAdd1">*</asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="txtValidatetxtNewTextRegex" runat="server" CssClass="failureNotification"
                                                                ControlToValidate="txtNewText"
                                                                ValidationExpression="^[^<>]+$"
                                                                ErrorMessage="The following letters are not permitted: '<', '>'"
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryAdd1" Enabled="false">*</asp:RegularExpressionValidator>
                                                        </FooterTemplate> 
                                                        <ItemTemplate>
                                                            <div style="width:375px;max-width:375px;text-align:left;" class="wrapword">
                                                                <asp:Label ID="lblText" runat="server" Text='<%# (Eval("deleted_by") != DBNull.Value || Eval("date_deleted") != DBNull.Value ? "[DELETED]<br />" : "") +  (Eval("text") == DBNull.Value ? "" : Server.HtmlEncode((string)Eval("text")).Replace("\n", "<br/>")) %>'></asp:Label> 
                                                            </div>
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                        <ItemTemplate> 
                                                            <asp:ImageButton ID="lnkViewHistory" runat="server" ImageUrl="~/images/Letter-H-gold-24.png" OnClientClick='<%# "open_history(\"NoteEditHistoryV2.aspx?id=" + Eval("note_id") + "\"); return false;" %>' />
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <%--
                                                    <asp:TemplateField HeaderText="Site" HeaderStyle-HorizontalAlign="Left" SortExpression="site_name" FooterStyle-VerticalAlign="Top"> 
                                                        <EditItemTemplate> 
                                                            <asp:DropDownList ID="ddlSite" runat="server" DataTextField="name" DataValueField="site_id"> </asp:DropDownList> 
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblSite" runat="server" Text='<%# Eval("site_name") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:DropDownList ID="ddlNewSite" runat="server" DataTextField="name" DataValueField="site_id"> </asp:DropDownList>
                                                        </FooterTemplate> 
                                                    </asp:TemplateField> 
                                                    --%>

                                                    <asp:TemplateField HeaderText="." ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" HeaderStyle-Font-Bold="false" > 
                                                        <EditItemTemplate> 
                                                            <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit1"></asp:LinkButton> 
                                                            <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                                        </EditItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd1"></asp:LinkButton> 
                                                        </FooterTemplate> 
                                                        <ItemTemplate> 
                                                           <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png" AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                        <ItemTemplate> 
                                                            <asp:ImageButton ID="lnkDelete" runat="server" ImageUrl="~/images/Delete-icon-24.png" CommandArgument='<%# Eval("note_id") %>' CommandName="_Delete" />
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText=""  HeaderStyle-HorizontalAlign="Center" HeaderStyle-Wrap="false">
                                                        <HeaderTemplate>
                                                            <asp:ImageButton ID="btnPrint" runat="server" OnClick="btnPrint_Click1" ImageUrl="~/images/printer_green-24.png" AlternateText="Print" ToolTip="Print" OnClientClick="javascript: if (!check_selected_atleast_one()) return false;" />
                                                            <asp:ImageButton ID="btnEmail" runat="server" OnClick="btnEmail_Click1" ImageUrl="~/images/email-24.png" AlternateText="Email To Referrer" ToolTip="Email To Referrer" OnClientClick="javascript: if (!check_selected_atleast_one()) return false;" />
                                                            <asp:HiddenField ID="hiddenRefEmail" runat="server" />
                                                            <asp:HiddenField ID="hiddenRefName" runat="server" />
                                                            <asp:HiddenField ID="hiddenBookingOrg" runat="server" />
                                                            <asp:HiddenField ID="HiddenBookingPatientName" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate> 
                                                            <asp:CheckBox ID="chkPrint" runat="server" onchange="highlight_row(this, 1)" />
                                                        </ItemTemplate> 
                                                    </asp:TemplateField>

                                                    <%--
                                                    <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" /> 
                                                    --%>

                                                </Columns> 
                                            </asp:GridView>

                                        </center>
                                    </td>
                                </tr>
                                <tr id="tab2" style="display:none;">
                                    <td>
                                        <center>

                                            <table>
                                                <tr>
                                                    <td>

		                                                <img src="images/body_chart.png" alt="" usemap="#logos" />
		                                                <map name="logos">
			                                                <area shape="circle" coords="73,39,6"    onclick="select_body_part(1);return false;"  href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="76,65,2"    onclick="select_body_part(2);return false;"  href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="75,73,3"    onclick="select_body_part(3);return false;"  href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="95,104,6"   onclick="select_body_part(4);return false;"  href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="57,104,6"   onclick="select_body_part(5);return false;"  href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="103,155,6"  onclick="select_body_part(6);return false;"  href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="50,155,6"   onclick="select_body_part(7);return false;"  href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="76,141,6"   onclick="select_body_part(8);return false;"  href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="76,175,6"   onclick="select_body_part(9);return false;"  href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="76,195,6"   onclick="select_body_part(10);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="80,218,10"  onclick="select_body_part(11);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="57,249,6"   onclick="select_body_part(13);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="95,249,6"   onclick="select_body_part(12);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="63,331,6"   onclick="select_body_part(15);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="95,331,6"   onclick="select_body_part(14);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="28,133,6"   onclick="select_body_part(17);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="126,133,6"  onclick="select_body_part(16);return false;" href="javascript:void(0)" alt="" />

			                                                <area shape="circle" coords="232,21,6"   onclick="select_body_part(18);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="232,56,6"   onclick="select_body_part(19);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="232,106,6"  onclick="select_body_part(20);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="182,135,6"  onclick="select_body_part(16);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="281,135,6"  onclick="select_body_part(17);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="232,158,6"  onclick="select_body_part(21);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="232,184,6"  onclick="select_body_part(22);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="214,249,6"  onclick="select_body_part(23);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="250,249,6"  onclick="select_body_part(24);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="217,326,6"  onclick="select_body_part(25);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="244,326,6"  onclick="select_body_part(26);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="73,393,6"   onclick="select_body_part(28);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="91,393,6"   onclick="select_body_part(27);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="218,395,6"  onclick="select_body_part(27);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="244,395,6"  onclick="select_body_part(28);return false;" href="javascript:void(0)" alt="" />

			                                                <area shape="circle" coords="114,87,10"  onclick="select_body_part(29);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="37,87,10"   onclick="select_body_part(30);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="142,209,8"  onclick="select_body_part(31);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="16,209,8"   onclick="select_body_part(32);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="149,228,8"  onclick="select_body_part(33);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="10,227,8"   onclick="select_body_part(34);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="142,251,8"  onclick="select_body_part(35);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="16,259,8"   onclick="select_body_part(36);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="91,378,6"   onclick="select_body_part(37);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="71,379,6"   onclick="select_body_part(38);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="178,162,6"   onclick="select_body_part(39);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="284,162,6"   onclick="select_body_part(40);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="97,299,6"   onclick="select_body_part(41);return false;" href="javascript:void(0)" alt="" />
			                                                <area shape="circle" coords="66,299,6"   onclick="select_body_part(42);return false;" href="javascript:void(0)" alt="" />

			                                                <area shape="default" nohref="nohref" title="Default" alt="Default"/>
		                                                </map>

                                                    </td>
                                                    <td style="min-width:2px;width:15px;"></td>
                                                    <td style="vertical-align:top;">

                                                        <asp:GridView ID="GrdNote2" runat="server" 
                                                             AutoGenerateColumns="False" DataKeyNames="note_id" 
                                                             OnRowCancelingEdit="GrdNote2_RowCancelingEdit" 
                                                             OnRowDataBound="GrdNote2_RowDataBound" 
                                                             OnRowEditing="GrdNote2_RowEditing" 
                                                             OnRowUpdating="GrdNote2_RowUpdating" ShowFooter="True" 
                                                             OnRowCommand="GrdNote2_RowCommand" 
                                                             OnRowDeleting="GrdNote2_RowDeleting" 
                                                             OnRowCreated="GrdNote2_RowCreated"
                                                             AllowSorting="True" 
                                                             OnSorting="GrdNote2_Sorting"
                                                             RowStyle-VerticalAlign="top"
                                                             ClientIDMode="Predictable"
                                                             CssClass="table table-bordered table-white table-grid table-grid-top-bottum-padding-normal auto_width block_center vertical_align_top">
                                                            <Columns> 

                                                                <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="note_id"> 
                                                                    <EditItemTemplate> 
                                                                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("note_id") %>'></asp:Label>
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("note_id") %>'></asp:Label> 
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField>
                
                                                                <asp:TemplateField HeaderText="Entity"  HeaderStyle-HorizontalAlign="Left" SortExpression="entity_id">
                                                                    <EditItemTemplate> 
                                                                        <asp:Label ID="lblEntityID" runat="server" Text='<%# Bind("entity_id") %>'></asp:Label>
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblEntityID" runat="server" Text='<%# Bind("entity_id") %>'></asp:Label> 
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="date_added" FooterStyle-VerticalAlign="Top" FooterStyle-Wrap="False" ItemStyle-Wrap="false">
                                                                    <EditItemTemplate> 
                                                                        <asp:DropDownList ID="ddlDate_Day" runat="server"/> <asp:DropDownList ID="ddlDate_Month" runat="server"/> <asp:DropDownList ID="ddlDate_Year" runat="server"/>
                                                                        <asp:CustomValidator ID="ddlDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                                                            ControlToValidate="ddlDate_Day"
                                                                            OnServerValidate="DateAllOrNoneCheck"
                                                                            ErrorMessage="Date must be valid with each of day/month/year selected"
                                                                            Display="Dynamic"
                                                                            ValidationGroup="ValidationSummaryEdit2">*</asp:CustomValidator>
                                                                        <asp:Label ID="lblAddedBy" runat="server" Text='<%#  Eval("added_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Added By\">A</a>: " + Eval("added_by_person_firstname") + " " + Eval("added_by_person_surname") %>' ></asp:Label> 
                                                                        <asp:Label ID="lblModifiedBy" runat="server" Visible='<%# Eval("last_modified_note_info_visible") %>' Text='<%#  Eval("modified_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Last Modified By\">M</a>: " + Eval("modified_by_person_firstname") + " " + Eval("modified_by_person_surname") %>' ></asp:Label> 
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("date_added") == DBNull.Value ? "" : Convert.ToDateTime(Eval("date_added")).ToString("dd / MM / yyyy") %>' ></asp:Label> 
                                                                        <asp:Label ID="lblAddedBy" runat="server" Text='<%#  Eval("added_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Added By\">A</a>: " + Eval("added_by_person_firstname") + " " + Eval("added_by_person_surname") %>' ></asp:Label> 
                                                                        <asp:Label ID="lblModifiedBy" runat="server" Visible='<%# Eval("last_modified_note_info_visible") != DBNull.Value && Convert.ToBoolean(Eval("last_modified_note_info_visible")) %>' Text='<%#  Eval("modified_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Last Modified By\">M</a>: " + Eval("modified_by_person_firstname") + " " + Eval("modified_by_person_surname") %>' ></asp:Label> 
                                                                    </ItemTemplate> 
                                                                    <FooterTemplate> 
                                                                        <asp:DropDownList ID="ddlNewDate_Day" runat="server"/> <asp:DropDownList ID="ddlNewDate_Month" runat="server"/> <asp:DropDownList ID="ddlNewDate_Year" runat="server"/>
                                                                        <asp:CustomValidator ID="ddlNewDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                                                            ControlToValidate="ddlNewDate_Day"
                                                                            OnServerValidate="DateAllOrNoneCheck"
                                                                            ErrorMessage="Date must be valid with each of day/month/year selected"
                                                                            Display="Dynamic"
                                                                            ValidationGroup="ValidationSummaryAdd2">*</asp:CustomValidator>
                                                                    </FooterTemplate> 
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="note_type_descr" FooterStyle-VerticalAlign="Top"> 
                                                                    <EditItemTemplate> 
                                                                        <asp:DropDownList ID="ddlNoteType" runat="server" DataTextField="descr" DataValueField="note_type_id"> </asp:DropDownList> 
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        <div style="text-align:left;">
                                                                            <asp:Label ID="lblNoteType" runat="server" Text='<%# Eval("note_type_descr") %>'></asp:Label> 
                                                                        </div>
                                                                    </ItemTemplate> 
                                                                    <FooterTemplate> 
                                                                        <asp:DropDownList ID="ddlNewNoteType" runat="server" DataTextField="descr" DataValueField="note_type_id"> </asp:DropDownList>
                                                                    </FooterTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="Body Part" HeaderStyle-HorizontalAlign="Left" SortExpression="body_part_descr" FooterStyle-VerticalAlign="Top"> 
                                                                    <EditItemTemplate> 
                                                                        <asp:DropDownList ID="ddlBodyPart" runat="server" DataTextField="descr" DataValueField="body_part_id"></asp:DropDownList>
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblBodyPart" runat="server" Text='<%# Eval("body_part_descr") %>'></asp:Label> 
                                                                    </ItemTemplate> 
                                                                    <FooterTemplate> 
                                                                        <asp:DropDownList ID="ddlNewBodyPart" runat="server" DataTextField="descr" DataValueField="body_part_id"></asp:DropDownList>
                                                                    </FooterTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="Note" HeaderStyle-HorizontalAlign="Left" SortExpression="text" FooterStyle-VerticalAlign="Top"> 
                                                                    <EditItemTemplate> 
                                                                        <asp:TextBox ID="txtText" TextMode="multiline" rows="6" style="width:375px;" runat="server" Text='<%# Bind("text") %>'></asp:TextBox> 
                                                                        <asp:RequiredFieldValidator ID="txtValidateTextRequired" runat="server" CssClass="failureNotification"  
                                                                            ControlToValidate="txtText" 
                                                                            ErrorMessage="Text is required."
                                                                            Display="Dynamic"
                                                                            ValidationGroup="ValidationSummaryEdit2">*</asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator ID="txtValidatetxtTextRegex" runat="server" CssClass="failureNotification"
                                                                            ControlToValidate="txtText"
                                                                            ValidationExpression="^[^<>]+$"
                                                                            ErrorMessage="The following letters are not permitted: '<', '>'"
                                                                            Display="Dynamic"
                                                                            ValidationGroup="ValidationSummaryEdit2" Enabled="false">*</asp:RegularExpressionValidator>
                                                                    </EditItemTemplate> 
                                                                    <FooterTemplate>
                                                                        <asp:TextBox ID="txtNewText" TextMode="multiline" rows="6" style="width:375px" runat="server" ></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="txtValidateNewTextRequired" runat="server" CssClass="failureNotification"  
                                                                            ControlToValidate="txtNewText" 
                                                                            ErrorMessage="Note Text is required."
                                                                            Display="Dynamic"
                                                                            ValidationGroup="ValidationSummaryAdd2">*</asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator ID="txtValidatetxtNewTextRegex" runat="server" CssClass="failureNotification"
                                                                            ControlToValidate="txtNewText"
                                                                            ValidationExpression="^[^<>]+$"
                                                                            ErrorMessage="The following letters are not permitted: '<', '>'"
                                                                            Display="Dynamic"
                                                                            ValidationGroup="ValidationSummaryAdd2" Enabled="false">*</asp:RegularExpressionValidator>
                                                                    </FooterTemplate> 
                                                                    <ItemTemplate>
                                                                        <div style="width:375px;max-width:375px;text-align:left;" class="wrapword">
                                                                            <asp:Label ID="lblText" runat="server" Text='<%# (Eval("deleted_by") != DBNull.Value || Eval("date_deleted") != DBNull.Value ? "[DELETED]<br />" : "") +  (Eval("text") == DBNull.Value ? "" : Server.HtmlEncode((string)Eval("text")).Replace("\n", "<br/>")) %>'></asp:Label> 
                                                                        </div>
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                                    <ItemTemplate> 
                                                                        <asp:ImageButton ID="lnkViewHistory" runat="server" ImageUrl="~/images/Letter-H-gold-24.png" OnClientClick='<%# "open_history(\"NoteEditHistoryV2.aspx?id=" + Eval("note_id") + "\"); return false;" %>' />
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField> 

                                                                <%--
                                                                <asp:TemplateField HeaderText="Site" HeaderStyle-HorizontalAlign="Left" SortExpression="site_name" FooterStyle-VerticalAlign="Top"> 
                                                                    <EditItemTemplate> 
                                                                        <asp:DropDownList ID="ddlSite" runat="server" DataTextField="name" DataValueField="site_id"> </asp:DropDownList> 
                                                                    </EditItemTemplate> 
                                                                    <ItemTemplate> 
                                                                        <asp:Label ID="lblSite" runat="server" Text='<%# Eval("site_name") %>'></asp:Label> 
                                                                    </ItemTemplate> 
                                                                    <FooterTemplate> 
                                                                        <asp:DropDownList ID="ddlNewSite" runat="server" DataTextField="name" DataValueField="site_id"> </asp:DropDownList>
                                                                    </FooterTemplate> 
                                                                </asp:TemplateField> 
                                                                --%>

                                                                <asp:TemplateField HeaderText="." ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" HeaderStyle-Font-Bold="false" > 
                                                                    <EditItemTemplate> 
                                                                        <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit2"></asp:LinkButton> 
                                                                        <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                                                    </EditItemTemplate> 
                                                                    <FooterTemplate> 
                                                                        <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd2"></asp:LinkButton> 
                                                                    </FooterTemplate> 
                                                                    <ItemTemplate> 
                                                                       <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png" AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                                    <ItemTemplate> 
                                                                        <asp:ImageButton ID="lnkDelete" runat="server" ImageUrl="~/images/Delete-icon-24.png" CommandArgument='<%# Eval("note_id") %>' CommandName="_Delete" />
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField> 

                                                                <asp:TemplateField HeaderText=""  HeaderStyle-HorizontalAlign="Center" HeaderStyle-Wrap="false">
                                                                    <HeaderTemplate>
                                                                        <asp:ImageButton ID="btnPrint" runat="server" OnClick="btnPrint_Click2" ImageUrl="~/images/printer_green-24.png" AlternateText="Print" ToolTip="Print" OnClientClick="javascript: if (!check_selected_atleast_one()) return false;" />
                                                                        <asp:ImageButton ID="btnEmail" runat="server" OnClick="btnEmail_Click2" ImageUrl="~/images/email-24.png" AlternateText="Email To Referrer" ToolTip="Email To Referrer" OnClientClick="javascript: if (!check_selected_atleast_one()) return false;" />
                                                                        <asp:HiddenField ID="hiddenRefEmail" runat="server" />
                                                                        <asp:HiddenField ID="hiddenRefName" runat="server" />
                                                                        <asp:HiddenField ID="hiddenBookingOrg" runat="server" />
                                                                        <asp:HiddenField ID="HiddenBookingPatientName" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate> 
                                                                        <asp:CheckBox ID="chkPrint" runat="server" onchange="highlight_row(this, 2)" />
                                                                    </ItemTemplate> 
                                                                </asp:TemplateField>

                                                                <%--
                                                                <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" /> 
                                                                --%>

                                                            </Columns> 
                                                        </asp:GridView>
                                                    
                                                    </td>
                                                </tr>
                                            </table>

                                        </center>
                                    </td>
                                </tr>
                                <tr id="tab3" style="display:none;">
                                    <td>
                                        <center>

                                            <asp:GridView ID="GrdNote3" runat="server" 
                                                 AutoGenerateColumns="False" DataKeyNames="note_id" 
                                                 OnRowCancelingEdit="GrdNote3_RowCancelingEdit" 
                                                 OnRowDataBound="GrdNote3_RowDataBound" 
                                                 OnRowEditing="GrdNote3_RowEditing" 
                                                 OnRowUpdating="GrdNote3_RowUpdating" ShowFooter="True" 
                                                 OnRowCommand="GrdNote3_RowCommand" 
                                                 OnRowDeleting="GrdNote3_RowDeleting" 
                                                 OnRowCreated="GrdNote3_RowCreated"
                                                 AllowSorting="True" 
                                                 OnSorting="GrdNote3_Sorting"
                                                 RowStyle-VerticalAlign="top"
                                                 ClientIDMode="Predictable"
                                                 CssClass="table table-bordered table-white table-grid table-grid-top-bottum-padding-normal auto_width block_center vertical_align_top">
                                                <Columns> 

                                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="note_id"> 
                                                        <EditItemTemplate> 
                                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("note_id") %>'></asp:Label>
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("note_id") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                    </asp:TemplateField>
                
                                                    <asp:TemplateField HeaderText="Entity"  HeaderStyle-HorizontalAlign="Left" SortExpression="entity_id">
                                                        <EditItemTemplate> 
                                                            <asp:Label ID="lblEntityID" runat="server" Text='<%# Bind("entity_id") %>'></asp:Label>
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblEntityID" runat="server" Text='<%# Bind("entity_id") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="date_added" FooterStyle-VerticalAlign="Top" FooterStyle-Wrap="False" ItemStyle-Wrap="false">
                                                        <EditItemTemplate> 
                                                            <asp:DropDownList ID="ddlDate_Day" runat="server"/> <asp:DropDownList ID="ddlDate_Month" runat="server"/> <asp:DropDownList ID="ddlDate_Year" runat="server"/>
                                                            <asp:CustomValidator ID="ddlDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                                                ControlToValidate="ddlDate_Day"
                                                                OnServerValidate="DateAllOrNoneCheck"
                                                                ErrorMessage="Date must be valid with each of day/month/year selected"
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryEdit3">*</asp:CustomValidator>
                                                            <asp:Label ID="lblAddedBy" runat="server" Text='<%#  Eval("added_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Added By\">A</a>: " + Eval("added_by_person_firstname") + " " + Eval("added_by_person_surname") %>' ></asp:Label> 
                                                            <asp:Label ID="lblModifiedBy" runat="server" Visible='<%# Eval("last_modified_note_info_visible") %>' Text='<%#  Eval("modified_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Last Modified By\">M</a>: " + Eval("modified_by_person_firstname") + " " + Eval("modified_by_person_surname") %>' ></asp:Label> 
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("date_added") == DBNull.Value ? "" : Convert.ToDateTime(Eval("date_added")).ToString("dd / MM / yyyy") %>' ></asp:Label> 
                                                            <asp:Label ID="lblAddedBy" runat="server" Text='<%#  Eval("added_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Added By\">A</a>: " + Eval("added_by_person_firstname") + " " + Eval("added_by_person_surname") %>' ></asp:Label> 
                                                            <asp:Label ID="lblModifiedBy" runat="server" Visible='<%# Eval("last_modified_note_info_visible") != DBNull.Value && Convert.ToBoolean(Eval("last_modified_note_info_visible")) %>' Text='<%#  Eval("modified_by_staff_id") == DBNull.Value ? "" : "<br /><a href=\"#\" onclick=\"return false;\" title=\"Last Modified By\">M</a>: " + Eval("modified_by_person_firstname") + " " + Eval("modified_by_person_surname") %>' ></asp:Label> 
                                                        </ItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:DropDownList ID="ddlNewDate_Day" runat="server"/> <asp:DropDownList ID="ddlNewDate_Month" runat="server"/> <asp:DropDownList ID="ddlNewDate_Year" runat="server"/>
                                                            <asp:CustomValidator ID="ddlNewDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                                                ControlToValidate="ddlNewDate_Day"
                                                                OnServerValidate="DateAllOrNoneCheck"
                                                                ErrorMessage="Date must be valid with each of day/month/year selected"
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryAdd3">*</asp:CustomValidator>
                                                        </FooterTemplate> 
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="note_type_descr" FooterStyle-VerticalAlign="Top"> 
                                                        <EditItemTemplate> 
                                                            <asp:DropDownList ID="ddlNoteType" runat="server" DataTextField="descr" DataValueField="note_type_id"> </asp:DropDownList> 
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <div style="text-align:left;">
                                                                <asp:Label ID="lblNoteType" runat="server" Text='<%# Eval("note_type_descr") %>'></asp:Label> 
                                                            </div>
                                                        </ItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:DropDownList ID="ddlNewNoteType" runat="server" DataTextField="descr" DataValueField="note_type_id"> </asp:DropDownList>
                                                        </FooterTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="Body Part" HeaderStyle-HorizontalAlign="Left" SortExpression="body_part_descr" FooterStyle-VerticalAlign="Top"> 
                                                        <EditItemTemplate> 
                                                            <asp:DropDownList ID="ddlBodyPart" runat="server" DataTextField="descr" DataValueField="body_part_id"></asp:DropDownList>
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblBodyPart" runat="server" Text='<%# Eval("body_part_descr") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:DropDownList ID="ddlNewBodyPart" runat="server" DataTextField="descr" DataValueField="body_part_id"></asp:DropDownList>
                                                        </FooterTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="Note" HeaderStyle-HorizontalAlign="Left" SortExpression="text" FooterStyle-VerticalAlign="Top"> 
                                                        <EditItemTemplate> 
                                                            <asp:TextBox ID="txtText" TextMode="multiline" rows="6" style="width:375px;" runat="server" Text='<%# Bind("text") %>'></asp:TextBox> 
                                                            <asp:RequiredFieldValidator ID="txtValidateTextRequired" runat="server" CssClass="failureNotification"  
                                                                ControlToValidate="txtText" 
                                                                ErrorMessage="Text is required."
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryEdit3">*</asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="txtValidatetxtTextRegex" runat="server" CssClass="failureNotification"
                                                                ControlToValidate="txtText"
                                                                ValidationExpression="^[^<>]+$"
                                                                ErrorMessage="The following letters are not permitted: '<', '>'"
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryEdit3" Enabled="false">*</asp:RegularExpressionValidator>
                                                        </EditItemTemplate> 
                                                        <FooterTemplate>
                                                            <asp:TextBox ID="txtNewText" TextMode="multiline" rows="6" style="width:375px" runat="server" ></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="txtValidateNewTextRequired" runat="server" CssClass="failureNotification"  
                                                                ControlToValidate="txtNewText" 
                                                                ErrorMessage="Note Text is required."
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryAdd3">*</asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="txtValidatetxtNewTextRegex" runat="server" CssClass="failureNotification"
                                                                ControlToValidate="txtNewText"
                                                                ValidationExpression="^[^<>]+$"
                                                                ErrorMessage="The following letters are not permitted: '<', '>'"
                                                                Display="Dynamic"
                                                                ValidationGroup="ValidationSummaryAd3d" Enabled="false">*</asp:RegularExpressionValidator>
                                                        </FooterTemplate> 
                                                        <ItemTemplate>
                                                            <div style="width:375px;max-width:375px;text-align:left;" class="wrapword">
                                                                <asp:Label ID="lblText" runat="server" Text='<%# (Eval("deleted_by") != DBNull.Value || Eval("date_deleted") != DBNull.Value ? "[DELETED]<br />" : "") +  (Eval("text") == DBNull.Value ? "" : Server.HtmlEncode((string)Eval("text")).Replace("\n", "<br/>")) %>'></asp:Label> 
                                                            </div>
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                        <ItemTemplate> 
                                                            <asp:ImageButton ID="lnkViewHistory" runat="server" ImageUrl="~/images/Letter-H-gold-24.png" OnClientClick='<%# "open_history(\"NoteEditHistoryV2.aspx?id=" + Eval("note_id") + "\"); return false;" %>' />
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <%--
                                                    <asp:TemplateField HeaderText="Site" HeaderStyle-HorizontalAlign="Left" SortExpression="site_name" FooterStyle-VerticalAlign="Top"> 
                                                        <EditItemTemplate> 
                                                            <asp:DropDownList ID="ddlSite" runat="server" DataTextField="name" DataValueField="site_id"> </asp:DropDownList> 
                                                        </EditItemTemplate> 
                                                        <ItemTemplate> 
                                                            <asp:Label ID="lblSite" runat="server" Text='<%# Eval("site_name") %>'></asp:Label> 
                                                        </ItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:DropDownList ID="ddlNewSite" runat="server" DataTextField="name" DataValueField="site_id"> </asp:DropDownList>
                                                        </FooterTemplate> 
                                                    </asp:TemplateField> 
                                                    --%>

                                                    <asp:TemplateField HeaderText="." ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" HeaderStyle-Font-Bold="false" > 
                                                        <EditItemTemplate> 
                                                            <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit3"></asp:LinkButton> 
                                                            <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                                        </EditItemTemplate> 
                                                        <FooterTemplate> 
                                                            <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd3"></asp:LinkButton> 
                                                        </FooterTemplate> 
                                                        <ItemTemplate> 
                                                           <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png" AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                        <ItemTemplate> 
                                                            <asp:ImageButton ID="lnkDelete" runat="server" ImageUrl="~/images/Delete-icon-24.png" CommandArgument='<%# Eval("note_id") %>' CommandName="_Delete" />
                                                        </ItemTemplate> 
                                                    </asp:TemplateField> 

                                                    <asp:TemplateField HeaderText=""  HeaderStyle-HorizontalAlign="Center" HeaderStyle-Wrap="false">
                                                        <HeaderTemplate>
                                                            <asp:ImageButton ID="btnPrint" runat="server" OnClick="btnPrint_Click3" ImageUrl="~/images/printer_green-24.png" AlternateText="Print" ToolTip="Print" OnClientClick="javascript: if (!check_selected_atleast_one()) return false;" />
                                                            <asp:ImageButton ID="btnEmail" runat="server" OnClick="btnEmail_Click3" ImageUrl="~/images/email-24.png" AlternateText="Email To Referrer" ToolTip="Email To Referrer" OnClientClick="javascript: if (!check_selected_atleast_one()) return false;" />
                                                            <asp:HiddenField ID="hiddenRefEmail" runat="server" />
                                                            <asp:HiddenField ID="hiddenRefName" runat="server" />
                                                            <asp:HiddenField ID="hiddenBookingOrg" runat="server" />
                                                            <asp:HiddenField ID="HiddenBookingPatientName" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate> 
                                                            <asp:CheckBox ID="chkPrint" runat="server" onchange="highlight_row(this, 3)" />
                                                        </ItemTemplate> 
                                                    </asp:TemplateField>

                                                    <%--
                                                    <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" /> 
                                                    --%>

                                                </Columns> 
                                            </asp:GridView>

                                        </center>
                                    </td>
                                </tr>
                            </table>

                           </div>
                
                            <br />
                            <center>
                                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="self.close();" />
                            </center>


                         </td>
                     </tr>

                 </table>


            </center>


        </div>
    </div>


</asp:Content>



