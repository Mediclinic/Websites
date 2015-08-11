<%@ Page Title="Referrer Clinic List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReferrerClinicListPopupV2.aspx.cs" Inherits="ReferrerClinicListPopupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="Scripts/get_details_of_person.js" type="text/javascript"></script>
    <script type="text/javascript">
        function select_referrer(val) {

            window.returnValue = val;
            self.close();
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
            xmlhttp.open("GET", "/AJAX/AjaxLiveReferrerOrgNameSearch.aspx?q=" + str + "&max_results=150&link_href=" + encodeURIComponent("javascript:void(0);") + "&link_onclick=" + encodeURIComponent("select_referrer('[organisation_id]:[name]');"), true);
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
        <div class="page_title"><span id="lblHeading">Referrer Clinic List</span></div>
        <div class="main_content" style="padding:10px 5px;">
            <div class="user_login_form" style="width: 600px;">

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
                            <tr>
                                <td><asp:Label ID="lblFullNameSearch" runat="server">Search Surname </asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtSearchFullName" runat="server" placeholder="Enter Surname" onkeyup="live_search(this.value)" autocomplete="off" onkeydown="return (event.keyCode!=13);" ></asp:TextBox>
                                    <div id="div_livesearch" style="display:none;position:absolute;background:#FFFFFF;"></div>
                                </td>
                                <td></td>
                                <td><asp:Button ID="btnClearLiveSearch" runat="server" Text="Clear" OnClientClick="clear_live_search();return false;" /></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblSearchSurname" runat="server">Narrow Below List By Surname: </asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtSearchSurname" runat="server"></asp:TextBox>
                                    <asp:CheckBox ID="chkSurnameSearchOnlyStartWith" runat="server" Text="&nbsp;starts with" Checked="true" />
                                </td>
                                <td><asp:Button ID="btnSearchSurname" runat="server" Text="Search" onclick="btnSearchSurname_Click" /></td>
                                <td><asp:Button ID="btnClearSurname" runat="server" Text="Clear" onclick="btnClearSurnameSearch_Click" /></td>
                            </tr>
                            <tr>
                                <td colspan="4" style="text-align:center">
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

                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdReferrer" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="organisation_id" 
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
                        AllowPaging="True"
                        OnPageIndexChanging="GrdReferrer_PageIndexChanging"
                        PageSize="13"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <%-- Org --%>

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("organisation_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Org Name" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="name"> 
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


                            <%-- Select Button --%>

                            <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Button ID="btnSelect" runat="server" Text="Select" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 
                    </asp:GridView>

                </div>

                <br />
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="window.returnValue=false;self.close();" />

            </center>

        </div>
    </div>


</asp:Content>



