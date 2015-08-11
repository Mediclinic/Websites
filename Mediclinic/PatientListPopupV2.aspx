<%@ Page Title="Patients" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="PatientListPopupV2.aspx.cs" Inherits="PatientListPopupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="Scripts/get_details_of_person.js" type="text/javascript"></script>
    <script type="text/javascript">
        function select_patient(val) {
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

            //xmlhttp.open("GET", "/AJAX/AjaxLivePatientSurnameSearch.aspx?q=" + str + "&max_results=150&link_href=" + encodeURIComponent("PatientDetailV2.aspx?type=view&id=[patient_id]"), true);
            xmlhttp.open("GET", "/AJAX/AjaxLivePatientSurnameSearch.aspx?q=" + str + "&max_results=150&link_href=" + encodeURIComponent("javascript:void(0);") + "&link_onclick=" + encodeURIComponent("select_patient('[patient_id]:[firstname] [surname]');"), true);
            xmlhttp.send();
        }
        function clear_live_search() {
            document.getElementById("div_livesearch").innerHTML = "";
            document.getElementById("div_livesearch").style.border = "0px";
            document.getElementById("div_livesearch").style.display = "none";
            document.getElementById("txtSearchFullName").value = "";
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Patients</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">
            <div class="user_login_form" style="width: 650px;">

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
                                    <asp:CheckBox ID="chkSurnameSearchOnlyStartWith" runat="server" Text="&nbsp;starts with" Checked="true" />&nbsp;
                                </td>
                                <td><asp:Button ID="btnSearchSurname" runat="server" Text="Search" onclick="btnSearchSurname_Click" /></td>
                                <td><asp:Button ID="btnClearSurname" runat="server" Text="Clear" onclick="btnClearSurnameSearch_Click" /></td>
                            </tr>
                        </table>

                    </center>
                </div>

            </div>


            <center>

                <div id="autodivheight" class="divautoheight" style="height:500px; width: auto; padding-right: 17px;"">

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
                        OnSorting="GrdPatient_Sorting"
                        RowStyle-VerticalAlign="top"
                        AllowPaging="True"
                        OnPageIndexChanging="GrdPatient_PageIndexChanging"
                        PageSize="14"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("patient_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>
                
                            <asp:TemplateField HeaderText="Title" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Firstname" HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFirstname" runat="server" Text='<%# Bind("firstname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="M.name" HeaderStyle-HorizontalAlign="Left" SortExpression="middlename" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Surname" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSurname" runat="server" Text='<%# Bind("surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Gender" HeaderStyle-HorizontalAlign="Left" SortExpression="gender" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblGender" runat="server" Text='<%# ( Eval("gender").ToString() == "M")?"Male" : (( Eval("gender").ToString() == "F")?"Female" : "-") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="D.O.B." HeaderStyle-HorizontalAlign="Left" SortExpression="dob" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDOB" runat="server" Text='<%# Bind("dob", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

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



