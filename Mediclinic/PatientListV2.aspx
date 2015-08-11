<%@ Page Title="Patient List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="PatientListV2.aspx.cs" Inherits="PatientListV2" %>
<%@ Register TagPrefix="UC" TagName="DuplicatePersonModalElementControl" Src="~/Controls/DuplicatePersonModalElementControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link   type="text/css" href="Styles/duplicate_person_modal_box.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/provider_nbr_check.js"></script>
    <script type="text/javascript" src="Scripts/check_duplicate_persons.js"></script>
    <script type="text/javascript" src="Scripts/get_details_of_person.js"></script>
    <script type="text/javascript" src="Scripts/post_to_url.js"></script>
    <script type="text/javascript">

        function duplicate_person_check() {
            var firstname = document.getElementById('MainContent_GrdPatient_txtNewFirstname').value.trim();
            var surname = document.getElementById('MainContent_GrdPatient_txtNewSurname').value.trim();

            var result = ajax_duplicate_persons("patient", firstname, surname);

            if (result.length == 0) {
                alert("Error retreiving records for duplicate person check.");
            }
            else if (result == "NONE") {
                return;
            }
            else {
                var result_list = create_result_array(result);
                create_table(result_list, "ctable", "PatientListV2.aspx?add_to_this_org=1&type=view&id=");  // add "add_to_this_org=1"
                reveal_modal('modalPopupDupicatePerson');                                                    // then (if in PatientListV2.aspx && prov portal => add to Session["OrgID"])
            }                                                                                                // then regardless ... redirect without that in url
        }
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }

        function dob_changed() {
            if (document.getElementById('txtNewDOB').value.length == 2 ||
                document.getElementById('txtNewDOB').value.length == 5)
                document.getElementById('txtNewDOB').value += "-";
        }

        function show_hide_more_search_options(c) {
            if (c.checked) {
                document.getElementById("td_extended_search").removeAttribute('class');
            }
            else {
                document.getElementById("td_extended_search").setAttribute("class", "hiddencol");
            }
            update_to_max_height();
        }

        var live_search_timer; // global variable that u put in a timer to run after 200ms (see in the methods for more details)
        function live_search(str){
            clearTimeout(live_search_timer);
            live_search_timer = setTimeout(function () { run_live_search(str); }, 200);  // wait 200ms so that if they are still typing, wait until they have stopped
        }
        function run_live_search(str) {
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


            xmlhttp.open("GET", "/AJAX/AjaxLivePatientSurnameSearch.aspx?q=" + str + "&max_results=80&link_href=" + encodeURIComponent("PatientDetailV2.aspx?type=view&id=[patient_id]") + "&link_onclick=", true);
            xmlhttp.send();
        }

        function clear_live_search() {
            document.getElementById("div_livesearch").innerHTML = "";
            document.getElementById("div_livesearch").style.border = "0px";
            document.getElementById("div_livesearch").style.display = "none";
            document.getElementById("txtSearchFullName").value = "";
        }

        function quick_add() {
            var h = new Object(); // or just {}
            h['surname'] = document.getElementById('txtSearchFullName').value.trim();
            post_to_url("PatientAddV2.aspx", h, "post");
        }

        function title_changed_reset_gender(ddlTitle, ddlGender) {
            ddlTitle = document.getElementById(ddlTitle);
            var selValue = ddlTitle.options[ddlTitle.selectedIndex].value;
            if (selValue == 6 || selValue == 265 || selValue == 266)
                setSelectedValue(document.getElementById(ddlGender), "M");
            if (selValue == 7 || selValue == 26)
                setSelectedValue(document.getElementById(ddlGender), "F");
        }
        function setSelectedValue(selectObj, valueToSet) {
            for (var i = 0; i < selectObj.options.length; i++) {
                if (selectObj.options[i].value == valueToSet) {
                    selectObj.options[i].selected = true;
                    return;
                }
            }
        }

        function get_suburb() {
            var retVal = window.showModalDialog("Contact_SuburbListPopupV2.aspx", 'Show Popup Window', "dialogHeight:810px;dialogWidth:700px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined" || retVal == false)
                return false;

            set_suburb(retVal);
        }
        function clear_suburb() {
            document.getElementById('suburbID').value = '-1';
            document.getElementById('lblSuburbText').innerHTML = '--';
        }
        function set_suburb(retVal) {
            var index = retVal.indexOf(":");
            document.getElementById('suburbID').value = retVal.substring(0, index);
            document.getElementById('lblSuburbText').innerHTML = retVal.substring(index + 1);
            document.getElementById('btnSuburbSelectionUpdate').click();
        }

        function get_internal_org() {
            var retVal = window.showModalDialog("OrganisationListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:1150px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined")
                return false;

            var index = retVal.indexOf(":");
            document.getElementById('internalOrgID').value = retVal.substring(0, index);

            return true;
        }
        function clear_internal_org() {
            document.getElementById('internalOrgID').value = '0';
        }

        function get_provider() {
            var retVal = window.showModalDialog("StaffListPopupV2.aspx?only_providers=1", 'Show Popup Window', "dialogHeight:675px;dialogWidth:650px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined")
                return false;

            var index = retVal.indexOf(":");
            document.getElementById('providerID').value = retVal.substring(0, index);

            return true;
        }
        function clear_internal_org() {
            document.getElementById('providerID').value = '-1';
        }

        function get_referrer() {
            var retVal = window.showModalDialog("ReferrerListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:1150px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined")
                return false;

            var index = retVal.indexOf(":");
            document.getElementById('referrerID').value = retVal.substring(0, index);

            return true;
        }
        function clear_referrer() {
            document.getElementById('referrerID').value = '-1';
        }

        function get_referrer_person() {
            var retVal = window.showModalDialog("ReferrerDoctorListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:650px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined")
                return false;

            var index = retVal.indexOf(":");
            document.getElementById('referrerPersonID').value = retVal.substring(0, index);

            return true;
        }
        function clear_referrer_person() {
            document.getElementById('referrerPersonID').value = '-1';
        }

        function get_referrer_org() {
            var retVal = window.showModalDialog("ReferrerClinicListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:650px;resizable:yes;center:yes;");
            if (typeof retVal === "undefined")
                return false;

            var index = retVal.indexOf(":");
            document.getElementById('referrerOrgID').value = retVal.substring(0, index);

            return true;
        }
        function clear_referrer_org() {
            document.getElementById('referrerOrgID').value = '0';
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <UC:DuplicatePersonModalElementControl ID="duplicatePersonModalElementControl" runat="server" />
    
    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Patient List</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form" style="width: 950px;">

                <div class="border_top_bottom">
                    <center>

                        <table>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtSearchFullName" placeholder="Enter Surname" runat="server" onkeyup="live_search(this.value)" autocomplete="off" onkeydown="return (event.keyCode!=13);"></asp:TextBox><div id="div_livesearch" style="display:none;position:absolute;background:#FFFFFF;"></div>
                                </td>

                                <td>
                                    &nbsp;<asp:Button ID="btnClearFullNameSearch" runat="server" OnClientClick="clear_live_search(); return false;" Text="Clear" />
                                    &nbsp;<asp:Button ID="btnQuickAdd" runat="server" OnClientClick="quick_add(); return false;" Text="Add" />

                                    <span style="padding-left:20px;"><input id="chkShowExtendedSearch" onclick="show_hide_more_search_options(this);" type="checkbox" value="Accept Form" name="chkShowExtendedSearch" runat="server" />&nbsp;<label for="chkShowExtendedSearch" style="font-weight:normal;">more search options</label></span>
                                    <span style="padding-left:20px;"><asp:CheckBox ID="chkShowDeceased" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowDeceased_Submit" Text="" Font-Bold="false" />&nbsp;<label for="chkShowDeceased" style="font-weight:normal;">show deceased</label></span>
                                    <span style="padding-left:20px;"><asp:CheckBox ID="chkShowDeleted" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowDeleted_Submit" Text="" Font-Bold="false" />&nbsp;<label for="chkShowDeleted" style="font-weight:normal;">show archived</label></span>
                                    <span style="padding-left:20px;" class="nowrap" id="td_ShowOnlyMyPatients" runat="server"><asp:CheckBox ID="chkShowOnlyMyPatients" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowOnlyMyPatients_Submit" />&nbsp;<label for="chkShowOnlyMyPatients" style="font-weight:normal;">show only my patients</label></span>
                                    <span style="padding-left:20px;" class="hiddencol"><asp:CheckBox ID="chkUsePaging" runat="server" Text="" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" Checked="True" style="font-weight:normal;" />&nbsp;<label for="chkUsePaging" style="font-weight:normal;">use paging</label></span>
                                </td>

                            </tr>
                        </table>


                    </center>
                </div>


                <img src="images/Calendar-icon-24px.png" alt="booking sheet icon" style="margin:0 5px 5px 0;" />BK Sheet
                <span style="padding-left:50px;"><img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit</span>
                <span style="padding-left:50px;"><img src="imagesV2/x.png" alt="archive icon" style="margin:0 5px 5px 0;" />Archive</span>
            </div>


            <div id="td_extended_search" runat="server">

                <asp:Panel ID="pnlExtendedSearch" runat="server" DefaultButton="btnSearchAll">
                    <table class="block_center" style="border-spacing:0;border-collapse:collapse;">

                        <tr>
                            <td colspan="15"><div style="height:6px;"></div></td>
                        </tr>

                        <tr>
                            <td class="nowrap"><asp:Label ID="lblSearch" runat="server">Surname&nbsp;&nbsp;</asp:Label></td>
                            <td><asp:TextBox ID="txtSearchSurname" runat="server" autocomplete="off"></asp:TextBox></td>

                            <td rowspan="6" style="width:10px"></td>
                            <td rowspan="6" style="width:1px;background-color:darkgray;"></td>
                            <td rowspan="6" style="width:15px"></td>

                            <td class="nowrap"><asp:Label ID="lblSearchDOB" runat="server">DOB&nbsp;&nbsp;</asp:Label></td>
                            <td style="width:165px">
                                <asp:DropDownList ID="ddlDOB_Day" runat="server"></asp:DropDownList>
                                <asp:DropDownList ID="ddlDOB_Month" runat="server"></asp:DropDownList>
                                <asp:DropDownList ID="ddlDOB_Year" runat="server"></asp:DropDownList>
                            </td>

                            <td rowspan="6" style="width:15px"></td>
                            <td rowspan="6" style="width:1px;background-color:darkgray;"></td>
                            <td rowspan="6" style="width:15px"></td>

                            <td class="nowrap"><asp:Label ID="lblSearchSuburb" runat="server">Suburb&nbsp;&nbsp;</asp:Label></td>
                            <td><asp:Label  ID="lblSuburbText" runat="server" Text="--" CssClass="nowrap" /></td>
                            <td style="min-width:5px;"></td>
                            <td>
                                <asp:Button ID="btnSearchSuburb" runat="server" Text="Select" OnClick="btnSearchSuburb_Click" OnClientClick="javascript:return get_suburb();" CssClass="thin_button" />
                                <asp:HiddenField ID="suburbID" runat="server" Value="-1" />
                                <asp:Button ID="btnSuburbSelectionUpdate" runat="server" CssClass="hiddencol thin_button" Text=""  OnClick="btnSuburbSelectionUpdate_Click" />
                            </td>
                            <td><asp:Button ID="btnClearSuburbSearch" runat="server" Text="Clear" OnClick="btnClearSuburbSearch_Click" OnClientClick="javascript:clear_suburb();" CssClass="thin_button"/></td>
                        </tr>

                        <tr>
                            <td class="nowrap"><asp:Label ID="lblSearchFirstName" runat="server">First Name&nbsp;&nbsp;</asp:Label></td>
                            <td><asp:TextBox ID="txtSearchFirstName" runat="server" autocomplete="off"></asp:TextBox></td>

                            <td class="nowrap"><asp:Label ID="lblSearchMedicareCardNo" runat="server">HC Card No.&nbsp;&nbsp;</asp:Label></td>
                            <td><asp:TextBox ID="txtSearchMedicareCardNo" runat="server" style="width:100%;"></asp:TextBox></td>

                            <td class="nowrap"><asp:Label ID="lblSearchInternalOrg" runat="server">Clinic&nbsp;&nbsp;</asp:Label></td>
                            <td style="min-width:100px;"><asp:Label ID="lblInternalOrgText" runat="server" Text="--" CssClass="nowrap" /></td>
                            <td style="min-width:5px;"></td>
                            <td>
                                <asp:Button ID="btnSearchInternalOrg" runat="server" Text="Select" OnClick="btnSearchInternalOrg_Click" OnClientClick="javascript:return get_internal_org();" CssClass="thin_button" />
                                <asp:HiddenField ID="internalOrgID" runat="server" Value="0" />
                                <asp:Button ID="btnInternalOrgSelectionUpdate" runat="server" CssClass="hiddencol thin_button" Text=""  OnClick="btnInternalOrgSelectionUpdate_Click" />
                            </td>
                            <td><asp:Button ID="btnClearInternalOrgSearch" runat="server" Text="Clear" OnClick="btnClearInternalOrgSearch_Click" OnClientClick="javascript:clear_internal_org();" CssClass="thin_button"/></td>
                        </tr>

                        <tr>
                            <td class="nowrap"><asp:Label ID="lblSearchEmail" runat="server">Email&nbsp;&nbsp;</asp:Label></td>
                            <td style="width:165px"><asp:TextBox ID="txtSearchEmail" runat="server"></asp:TextBox></td>

                            <td colspan="2" align="right"><label for="chkOnlyDiabetics" style="font-weight:normal;">Only Diabetics</label>&nbsp;<asp:CheckBox ID="chkOnlyDiabetics" runat="server" Text="" TextAlign="Left" /></td>

                            <td class="nowrap"><asp:Label ID="lblSearchProvider" runat="server">Provider&nbsp;&nbsp;</asp:Label></td>
                            <td><asp:Label ID="lblProviderText" runat="server" Text="--" CssClass="nowrap" /></td>
                            <td style="min-width:5px;"></td>
                            <td><asp:Button ID="btnSearchProviderSearch" runat="server" Text="Select" OnClick="btnSearchProvider_Click" OnClientClick="javascript:return get_provider();" CssClass="thin_button" /></td>
                            <td>
                                <asp:Button ID="btnClearProviderSearch" runat="server" Text="Clear" OnClick="btnClearProviderSearch_Click" OnClientClick="javascript:return clear_provider();" CssClass="thin_button" />
                                <asp:HiddenField ID="providerID" runat="server" Value="-1" />
                                <asp:Button ID="btnProviderSelectionUpdate" runat="server" CssClass="hiddencol thin_button" Text=""  OnClick="btnProviderSelectionUpdate_Click" />

                            </td>
                        </tr>

                        <tr>
                            <td class="nowrap"><asp:Label ID="lblSearchPhNbr" runat="server">Ph Nbr&nbsp;&nbsp;</asp:Label></td>
                            <td style="width:165px"><asp:TextBox ID="txtSearchPhNum" runat="server"></asp:TextBox></td>

                            <td colspan="2" align="right"><label id="lblChkOnlyMedicareEPC" runat="server" for="chkOnlyMedicareEPC" style="font-weight:normal;">Only With Valid Medicare EPC</label>&nbsp;<asp:CheckBox ID="chkOnlyMedicareEPC" runat="server" Text="" TextAlign="Left" /></td>

                            <td class="nowrap"><asp:Label ID="lblSearchReferrerPerson" runat="server">Referrer [Doctor]&nbsp;&nbsp;</asp:Label></td>
                            <td><asp:Label ID="lblReferrerPersonText" runat="server" Text="--" CssClass="nowrap" /></td>
                            <td style="min-width:5px;"></td>
                            <td><asp:Button ID="btnSearchReferrerPerson" runat="server" Text="Select" OnClick="btnSearchReferrerPerson_Click" OnClientClick="javascript:return get_referrer_person();" CssClass="thin_button" /></td>
                            <td>
                                <asp:Button ID="btnClearReferrerPersonSearch" runat="server" Text="Clear" OnClick="btnClearReferrerPersonSearch_Click" OnClientClick="javascript:clear_referrer_person();" CssClass="thin_button"/>
                                <asp:HiddenField ID="referrerPersonID" runat="server" Value="-1" />
                                <asp:Button ID="btnReferrerPersonSelectionUpdate" runat="server" CssClass="hiddencol thin_button" Text=""  OnClick="btnReferrerPersonSelectionUpdate_Click" />
                            </td>
                        </tr>

                        <tr>
                            <td class="nowrap"><asp:Label ID="lblSearchStreet" runat="server">Street&nbsp;&nbsp;</asp:Label></td>
                            <td id="tr_Contact"    runat="server" valign="top"><asp:ListBox ID="lstStreets" runat="server" SelectionMode="Multiple" Rows="8"/></td>
                            <td id="tr_ContactAus" runat="server" valign="top"><asp:TextBox ID="txtSearchStreet" runat="server" /></td>

                            <td colspan="2" align="right"><label for="chkOnlyDVAEPC" style="font-weight:normal;">Only With Valid DVA</label>&nbsp;<asp:CheckBox ID="chkOnlyDVAEPC" runat="server" Text="" TextAlign="Left" /></td>

                            <td class="nowrap"><asp:Label ID="lblSearchReferrerOrg" runat="server">Referrer [Clinic]&nbsp;&nbsp;</asp:Label></td>
                            <td><asp:Label  ID="lblReferrerOrgText" runat="server" Text="--" CssClass="nowrap" /></td>
                            <td style="min-width:5px;"></td>
                            <td><asp:Button ID="btnSearchReferrerOrg" runat="server" Text="Select" OnClick="btnSearchReferrerOrg_Click" OnClientClick="javascript:return get_referrer_org();" CssClass="thin_button" /></td>
                            <td>
                                <asp:Button ID="btnClearReferrerOrgSearch" runat="server" Text="Clear" OnClick="btnClearReferrerOrgSearch_Click" OnClientClick="javascript:clear_referrer_org();" CssClass="thin_button"/>
                                <asp:HiddenField ID="referrerOrgID" runat="server" Value="-1" />
                                <asp:Button ID="btnReferrerOrgSelectionUpdate" runat="server" CssClass="hiddencol thin_button" Text=""  OnClick="btnReferrerOrgSelectionUpdate_Click" />
                            </td>
                        </tr>

                        <tr>
                            <td class="nowrap"></td>
                            <td valign="top"></td>
                            <td valign="top"></td>

                            <td colspan="2" align="right"></td>

                            <td class="nowrap"><asp:Label ID="lblSearchReferrer" runat="server">Referrer [Doctor @ Clinic]&nbsp;&nbsp;</asp:Label></td>
                            <td><asp:Label  ID="lblReferrerText" runat="server" Text="--" CssClass="nowrap" /></td>
                            <td style="min-width:5px;"></td>
                            <td><asp:Button ID="btnSearchReferrer" runat="server" Text="Select" OnClick="btnSearchReferrer_Click" CssClass="thin_button" OnClientClick="javascript:return get_referrer();" /></td>
                            <td>
                                <asp:Button ID="btnClearReferrerSearch" runat="server" Text="Clear" OnClick="btnClearReferrerSearch_Click" CssClass="thin_button" OnClientClick="javascript:clear_referrer();"/>
                                <asp:HiddenField ID="referrerID" runat="server" Value="-1" />
                                <asp:Button ID="btnReferrerSelectionUpdate" runat="server" CssClass="hiddencol thin_button" Text="" OnClick="btnReferrerSelectionUpdate_Click" />
                            </td>
                        </tr>

                        <tr valign="top">
                            <td colspan="15" align="center">
                                <br />
                                <asp:Button ID="btnSearchAll" runat="server" Text="Refresh"     OnClick="btnSearchAll_Click" CssClass="thin_button" />
                                <asp:Button ID="btnClearAll" runat="server" Text="Clear All"     OnClick="btnClearAll_Click" CssClass="thin_button" />
                                <asp:Button ID="btnExport"    runat="server" Text="Export List" OnClick="btnExport_Click" CssClass="thin_button" />
                                <br />
                                <br />
                            </td>
                        </tr>

                    </table>
                </asp:Panel>
            </div>



            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdPatient" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="patient_id" 
                         OnRowCancelingEdit="GrdPatient_RowCancelingEdit" 
                         OnRowDataBound="GrdPatient_RowDataBound" 
                         OnRowEditing="GrdPatient_RowEditing" 
                         OnRowUpdating="GrdPatient_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdPatient_RowCommand" 
                         OnRowDeleting="GrdPatient_RowDeleting" 
                         OnRowCreated="GrdPatient_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         RowStyle-VerticalAlign="top"
                         AllowPaging="True"
                         OnPageIndexChanging="GrdPatient_PageIndexChanging"
                         PageSize="15"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                         <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />
             
                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("patient_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("patient_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Title" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlTitle" runat="server" DataTextField="descr" DataValueField="title_id" ></asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("title_id") == DBNull.Value || (int)Eval("title_id") == 0 ? "" : Eval("descr") %>' ></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewTitle" runat="server" DataTextField="descr" DataValueField="title_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Firstname" HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtFirstname" runat="server" Text='<%# Bind("firstname") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateFirstnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtFirstname" 
                                        ErrorMessage="Firstname is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateFirstnameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtFirstname"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s'\(\)]+$"
                                        ErrorMessage="Firstname can only be alpha-numeric, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewFirstname" runat="server" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewFirstnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewFirstname" 
                                        ErrorMessage="Firstname is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewFirstnameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewFirstname"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s'\(\)]+$"
                                        ErrorMessage="Firstname can only be alpha-numeric, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkFirstname" runat="server" Text='<%# Eval("firstname") %>' NavigateUrl='<%#  String.Format("~/PatientDetailV2.aspx?type=view&id={0}",Eval("patient_id"))%>' ToolTip="Full Edit"  />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="M.name" HeaderStyle-HorizontalAlign="Left" SortExpression="middlename" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left nowrap"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtMiddlename"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Middlename can only be alpha-numeric, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewMiddlename" runat="server" ></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateNewMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewMiddlename"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Middlename can only be alpha-numeric, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Surname" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left nowrap"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtSurname" runat="server" Text='<%# Bind("surname") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtSurname" 
                                        ErrorMessage="ySurname is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateSurnameNameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtSurname"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s'\(\)]+$"
                                        ErrorMessage="Surname can only be alpha-numeric, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                 </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewSurname" runat="server" onblur="duplicate_person_check(this);"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewSurname" 
                                        ErrorMessage="xSurname is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewSurnameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewSurname"
                                        ValidationExpression="^[0-9a-zA-Z\-\.\s'\(\)]+$"
                                        ErrorMessage="Surname can only be alpha-numeric, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSurname" runat="server" Text='<%# Bind("surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Nickname" HeaderStyle-HorizontalAlign="Left" SortExpression="nickname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNickname" runat="server" Text='<%# Bind("nickname") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateNicknameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNickname"
                                        ValidationExpression="^[0-9a-zA-Z\-\s']+$"
                                        ErrorMessage="Nickname can only be alpha-numeric or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewNickname" runat="server" ></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateNewNicknameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewNickname"
                                        ValidationExpression="^[0-9a-zA-Z\-\s']+$"
                                        ErrorMessage="Nickname can only be alpha-numeric or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblNickname" runat="server" Text='<%# Bind("nickname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Gender" HeaderStyle-HorizontalAlign="Left" SortExpression="gender" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlGender" runat="server" SelectedValue='<%# Eval("gender") %>'> 
                                        <asp:ListItem Text="M" Value="M"></asp:ListItem>
                                        <asp:ListItem Text="F" Value="F"></asp:ListItem>
                                        <asp:ListItem Text="-" Value=""></asp:ListItem>
                                    </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblGender" runat="server" Text='<%# ( Eval("gender").ToString() == "M")?"Male" : (( Eval("gender").ToString() == "F")?"Female" : "-") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewGender" runat="server" >
                                        <asp:ListItem Text="M" Value="M" Selected="True"></asp:ListItem> 
                                        <asp:ListItem Text="F" Value="F"></asp:ListItem>
                                    </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="D.O.B." HeaderStyle-HorizontalAlign="Left" SortExpression="dob" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlDOB_Day" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlDOB_Month" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlDOB_Year" runat="server"></asp:DropDownList>
                                    <asp:CustomValidator ID="ddlDOBValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                        ControlToValidate="ddlDOB_Day"
                                        OnServerValidate="DOBAllOrNoneCheck"
                                        ErrorMessage="DOB must be valid, and either have each of day/month/year selected, or all set to '--'"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewDOB" runat="server"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateNewDOBRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDOB" 
                                        ErrorMessage="DOB is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDOBRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDOB"
                                        ValidationExpression="^\d{2}\-\d{2}\-\d{4}$"
                                        ErrorMessage="DOB format must be dd-mm-yyyy"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="txtValidateNewDOB" runat="server"  CssClass="failureNotification"  
                                        ControlToValidate="txtNewDOB"
                                        OnServerValidate="ValidDOBCheck"
                                        ErrorMessage="Invalid DOB"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDOB" runat="server" Text='<%# Bind("dob", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Diabetic" SortExpression="is_diabetic" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsDiabetic" runat="server" Checked='<%# Eval("is_diabetic").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDiabetic" runat="server" Text='<%# Eval("is_diabetic").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsDiabetic" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Member Diabetes Aus." SortExpression="is_member_diabetes_australia" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsMemberDiabetesAustralia" runat="server" Checked='<%# Eval("is_member_diabetes_australia").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsMemberDiabetesAustralia" runat="server" Text='<%# Eval("is_member_diabetes_australia").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsMemberDiabetesAustralia" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="DA Review" SortExpression="diabetic_assessment_review_date" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDAReview" runat="server" Text='<%# Eval("diabetic_assessment_review_date", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlDARev_Day" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlDARev_Month" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlDARev_Year" runat="server"></asp:DropDownList>
                                    <asp:CustomValidator ID="ddlDARevValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                        ControlToValidate="ddlDARev_Day"
                                        OnServerValidate="DARevAllOrNoneCheck"
                                        ErrorMessage="DA Review must be valid, and either have each of day/month/year selected, or all set to '--'"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewDARev" runat="server"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateNewDARevRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDARev" 
                                        ErrorMessage="DA Review is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDARevRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDARev"
                                        ValidationExpression="^\d{2}\-\d{2}\-\d{4}$"
                                        ErrorMessage="DA Review format must be dd-mm-yyyy"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="txtValidateNewDARev" runat="server"  CssClass="failureNotification"  
                                        ControlToValidate="txtNewDARev"
                                        OnServerValidate="ValidDARevCheck"
                                        ErrorMessage="Invalid DARev"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                </FooterTemplate> 

                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Clinic Patient" SortExpression="is_clinic_patient" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsClinicPatient" runat="server" Checked='<%# Eval("is_clinic_patient").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsClinicPatient" runat="server" Text='<%# Eval("is_clinic_patient").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsClinicPatient" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="GP Patient" SortExpression="is_gp_patient" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsGPPatient" runat="server" Checked='<%# Eval("is_gp_patient").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsGPPatient" runat="server" Text='<%# Eval("is_gp_patient").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsGPPatient" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="PT Type" HeaderStyle-HorizontalAlign="Left" SortExpression="ac_offering" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlACInvOffering" runat="server" DataTextField="o_name" DataValueField="o_offering_id" ></asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblACInvOffering" runat="server" Text='<%# Eval("ac_offering")  %>' ></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewACInvOffering" runat="server" DataTextField="descr" DataValueField="offering_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="Deceased" SortExpression="is_deceased" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsDeceased" runat="server" Checked='<%# Eval("is_deceased").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDeceased" runat="server" Text='<%# Eval("is_deceased").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsDeceased" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_date_added" ItemStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("added_by_firstname") + (Eval("added_by_firstname") == DBNull.Value ? "" : "<br>") + Eval("patient_date_added", "{0:dd-MM-yy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("patient_date_added", "{0:dd-MM-yy}") + (Eval("added_by") == DBNull.Value || Eval("added_by_firstname").ToString().Trim().Length == 0 ? "" : " (" + Eval("added_by_firstname") + ")") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Booking" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" SortExpression="num_registered_orgs"> 
                                <EditItemTemplate> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkBookings" runat="server" NavigateUrl='<%#  String.Format("~/BookingScreenGetPatientOrgsV2.aspx?patient_id={0}",Eval("patient_id")) %>' ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Bookings" ToolTip="Bookings" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Archived" SortExpression="is_deleted" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <%--
                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                <EditItemTemplate> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:ImageButton ID="lnkBookings" runat="server" PostBackUrl='<%#  String.Format("~/Bookings.aspx?type=patient&patient={0}",Eval("patient_id")) %>' ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Bookings" ToolTip="Bookings" />
                                </ItemTemplate> 
                            </asp:TemplateField> 
                            --%>

                            <%--
                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                <EditItemTemplate> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkOrgRegistrations" runat="server" NavigateUrl='<%#  String.Format("~/RegisterOrganisationsToPatientV2.aspx?id={0}&type=edit",Eval("patient_id"))%>'>Orgs</asp:HyperLink> 
                                </ItemTemplate> 
                            </asp:TemplateField> 
                            --%>

                            <%--
                            <asp:TemplateField HeaderText="Add/ Remove Referrers" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                <EditItemTemplate> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkRefRegistrations" runat="server" NavigateUrl='<%#  String.Format("~/RegisterReferrersToPatient.aspx?id={0}",Eval("patient_id"))%>'>Referrers</asp:HyperLink> 
                                </ItemTemplate> 
                            </asp:TemplateField> 
                            --%>



                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummary"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummary"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <%--<asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" />--%>

                            <asp:TemplateField HeaderText="" ShowHeader="True" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" runat="server"  CommandName="_Delete" CommandArgument='<%# Eval("patient_id") %>' ImageUrl="~/images/Delete-icon-24.png" AlternateText="Archive" ToolTip="Archive" />
                                </ItemTemplate>
                            </asp:TemplateField>


                        </Columns> 
                    </asp:GridView>


                </div>
            </center>            

        </div>
    </div>

</asp:Content>



