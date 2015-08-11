<%@ Page Title="Referrer List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReferrerListMultiplePopupV2.aspx.cs" Inherits="ReferrerListMultiplePopupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="Scripts/get_details_of_person.js" type="text/javascript"></script>
    <script type="text/javascript">

        function select_referrer(val) {
            window.returnValue = val;
            self.close();
        }

        function select_referrer_multiple() {

            // loop through and return underscore seperated list

            var selectedIDs = "";
            var gvDrv = document.getElementById("<%= GrdReferrer.ClientID %>");
            for (i = 1; i < gvDrv.rows.length; i++) {
                var cells = gvDrv.rows[i].cells;
                for (j = 0; j < cells.length; j++) {
                    var HTML = cells[j].innerHTML;
                    if (HTML.indexOf("chkSelect") != -1) {
                        var lblID = cells[0].getElementsByTagName("*")[0];
                        var chkSelect = cells[j].getElementsByTagName("*")[1];  // first item is the onchange event rendered as a div, so get 2nd item
                        if (chkSelect.checked)
                            selectedIDs += (selectedIDs.length == 0 ? "" : "_") + lblID.innerHTML;
                    }
                }
            }

            if (selectedIDs.length == 0) {
                alert("No boxes checked.");
                return;
            }
            else {
                window.returnValue = selectedIDs;
                self.close();
            }
        }

        function highlight_row(chkBox) {  // doesnt pass in a checkbox -- read first comment below
            // asp:CheckBox control doesn't have a onchange event, and onchange event will be rendered in a <span> tag and not the <input> tag. 
            // so get parent, then get the control

            var gvDrv = document.getElementById("<%= GrdReferrer.ClientID %>");
            for (i = 1; i < gvDrv.rows.length; i++) {

                // dont do all the processing if its not the row
                if (gvDrv.rows[i] != chkBox.parentNode.parentNode)
                    continue;

                    // if it is the row, process than return out of the function
                else {

                    var cells = gvDrv.rows[i].cells;
                    for (j = 0; j < cells.length; j++) {
                        var HTML = cells[j].innerHTML;

                        if (cells[j] != chkBox.parentNode)
                            continue; // alert("found");

                        if (HTML.indexOf("chkSelect") != -1) {
                            var lblID = cells[0].getElementsByTagName("*")[0];
                            var chkSelect = cells[j].getElementsByTagName("*")[1];  // first item is the onchange event rendered as a div, so get 2nd item

                            var cells2 = gvDrv.rows[i].cells;
                            for (j2 = 0; j2 < cells2.length; j2++)
                                cells2[j2].style.backgroundColor = chkSelect.checked ? '#FAFAD2' : '';  // LightGoldenrodYellow 
                        }
                    }

                    return;
                }
            }
        }

        function highlight_all_rows(chkBox) { // this uses a plain html checkbox, so it is an actual checkbox passed in
            var newColor = chkBox.checked ? '#FAFAD2' : '';   // LightGoldenrodYellow 
            var gvDrv = document.getElementById("<%= GrdReferrer.ClientID %>");
            for (i = 1; i < gvDrv.rows.length; i++) {
                var cells = gvDrv.rows[i].cells;
                for (j = 0; j < cells.length; j++) {
                    var HTML = cells[j].innerHTML;
                    if (HTML.indexOf("chkSelect") != -1) {
                        var chkSelect = cells[j].getElementsByTagName("*")[1];  // first item is the onchange event rendered as a div, so get 2nd item
                        chkSelect.checked = chkBox.checked;
                        gvDrv.rows[i].style.backgroundColor = newColor;
                    }
                }
            }
        }



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

            //xmlhttp.open("GET", "/AJAX/AjaxLiveReferrerSurnameSearch.aspx?q=" + str + "&max_results=150&link_href=" + encodeURIComponent("AddEditReferrer.aspx?type=view&id=[register_referrer_id]"), true);
            xmlhttp.open("GET", "/AJAX/AjaxLiveReferrerSurnameSearch.aspx?q=" + str + "&max_results=150&link_href=" + encodeURIComponent("javascript:void(0);") + "&link_onclick=" + encodeURIComponent("select_referrer('[register_referrer_id]:[firstname] [surname]');"), true);
            xmlhttp.send();
        }
        function clear_live_search() {
            document.getElementById("div_livesearch").innerHTML = "";
            document.getElementById("div_livesearch").style.border = "0px";
            document.getElementById("div_livesearch").style.display = "none";
            document.getElementById("txtSearchFullName").value = "";
        }


        function add_referrer() {
            window.showModalDialog('ReferrerAddV2.aspx?popup=1', '', 'dialogWidth:1250px;dialogHeight:800px;center:yes;resizable:no; scroll:no');
            document.getElementById('btnUpdateReferrersList').click();
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Referrer List</span></div>
        <div class="main_content" style="padding:10px 5px;">
            <div class="user_login_form" style="width: 950px;">

                <div class="border_top_bottom">
                    <center>

                        <table>
                            <tr>
                                <td>
                                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                                </td>
                            </tr>
                        </table>

                        

                        <table class="padded-table-2px">
                            <tr id="Tr1" runat="server" visible="false">
                                <td><asp:Label ID="lblFullNameSearch" runat="server">Search Surname </asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtSearchFullName" runat="server" placeholder="Enter Surname" onkeyup="live_search(this.value)" autocomplete="off" onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                                    <button name="btnClearFullNameSearch" onclick="clear_live_search(); return false;">Clear</button>
                                    <div id="div_livesearch" style="display:none;position:absolute;background:#FFFFFF;"></div>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblSearchSurname" runat="server">Narrow Below List By Surname: </asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtSearchSurname" runat="server"></asp:TextBox>
                                    <asp:CheckBox ID="chkSurnameSearchOnlyStartWith" runat="server" Text="starts with" Font-Size="X-Small" Checked="true" />
                                </td>
                                <td><asp:Button ID="btnSearchSurname" runat="server" Text="Search" onclick="btnSearchSurname_Click" /></td>
                                <td><asp:Button ID="btnClearSurname" runat="server" Text="Clear" onclick="btnClearSurnameSearch_Click" /></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblSearchSuburb" runat="server">Narrow Below List By Suburb: </asp:Label></td>
                                <td><asp:TextBox ID="txtSearchSuburb" runat="server"></asp:TextBox></td>
                                <td><asp:Button ID="btnSearchSuburb" runat="server" Text="Search" onclick="btnSearchSuburb_Click" /></td>
                                <td><asp:Button ID="btnClearSuburb" runat="server" Text="Clear" onclick="btnClearSuburbSearch_Click" /></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblSearchPostcode" runat="server">Narrow Below List By Postcode: </asp:Label></td>
                                <td><asp:TextBox ID="txtSearchPostcode" runat="server"></asp:TextBox></td>
                                <td><asp:Button ID="btnSearchPostcode" runat="server" Text="Search" onclick="btnSearchPostcode_Click" /></td>
                                <td><asp:Button ID="btnClearPostcode" runat="server" Text="Clear" onclick="btnClearPostcodeSearch_Click" /></td>
                            </tr>




                            <tr>
                                <td colspan="4" align="center">
                                    Referrer Not Found? 
                                    &nbsp;
                                    <asp:LinkButton ID="btnRegisterReferrerListPopup" runat="server" Text="Add New Referrer" OnClientClick="javascript:add_referrer(); return false;"/>
                                    <asp:Button ID="btnUpdateReferrersList" runat="server" Text="Search" OnClick="btnUpdateReferrersList_Click" CssClass="hiddencol" />
                                </td>
                            </tr>
                        </table>


                    </center>
                </div>

            </div>




            <center>

                <div id="autodivheight" class="divautoheight" style="height:400px;">

                    <asp:GridView ID="GrdReferrer" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="register_referrer_id" 
                        OnRowCancelingEdit="GrdReferrer_RowCancelingEdit" 
                        OnRowDataBound="GrdReferrer_RowDataBound" 
                        OnRowEditing="GrdReferrer_RowEditing" 
                        OnRowUpdating="GrdReferrer_RowUpdating" ShowFooter="False" 
                        OnRowCommand="GrdReferrer_RowCommand" 
                        OnRowDeleting="GrdReferrer_RowDeleting" 
                        OnRowCreated="GrdReferrer_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GrdReferrer_Sorting"
                        RowStyle-VerticalAlign="top"
                        AllowPaging="False"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <%-- RegReferrer --%>

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_referrer_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_referrer_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>
                

                            <%-- Referrer --%>

                            <asp:TemplateField HeaderText="Title" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Firstname" HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFirstname" runat="server" Text='<%# Bind("firstname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="M.name" HeaderStyle-HorizontalAlign="Left" SortExpression="middlename" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Surname" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSurname" runat="server" Text='<%# Bind("surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Gender" HeaderStyle-HorizontalAlign="Left" SortExpression="gender" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblGender" runat="server" Text='<%# ( Eval("gender").ToString() == "M")?"Male" : (( Eval("gender").ToString() == "F")?"Female" : "-") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <%-- Org --%>

                            <asp:TemplateField HeaderText="Org Name" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="name" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lnkTitle" runat="server" Text='<%# Eval("name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Org ABN" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="abn"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblABN" runat="server" Text='<%# Bind("abn") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Org ACN" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="acn"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblACN" runat="server" Text='<%# Bind("acn") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <%-- RegReferrer --%>

                            <asp:TemplateField HeaderText="Provider Number" HeaderStyle-HorizontalAlign="Left" SortExpression="provider_number" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblProviderNumber" runat="server" Text='<%# Bind("provider_number") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <%--
                            <asp:TemplateField HeaderText="Deleted" SortExpression="is_deleted" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 
                            --%>


                            <%-- Select Button --%>

                            <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" Visible="False"> 
                                <ItemTemplate> 
                                    <asp:Button ID="btnSelect" runat="server" Text="Select" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Center" FooterStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="50px"> 
                                <HeaderTemplate>
                                    <input type="checkbox" id="chkCheckAllNone" name="chkCheckAllNone" onchange="highlight_all_rows(this);return false;" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" onchange="highlight_row(this);"  />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 
                    </asp:GridView>

                </div>

                <br />

                <asp:Button ID="btnAddAllSelected" runat="server" Text="Add All Selected" OnClientClick="javascript:select_referrer_multiple();return false;" />
                &nbsp;&nbsp;
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="window.returnValue=false;self.close();" />

            </center>

        </div>
    </div>


</asp:Content>



