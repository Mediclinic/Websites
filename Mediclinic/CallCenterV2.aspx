<%@ Page Title="Call Centre" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="CallCenterV2.aspx.cs" Inherits="CallCenterV2" ValidateRequest="false" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="Scripts/post_to_url.js"></script>
    <script type="text/javascript">

        function http_post(db, org, site, patient) {
            var h = new Object(); // or just {}
            h['db']  = db;
            h['org'] = org;
            h['site'] = site;
            h['patient'] = patient;
            post_to_url(window.location.pathname, h, "post");
        }




        function hide_show(id) {
            var e = document.getElementById(id);
            if (e != null)
               e.style.display = e.style.display == "none" ? "" : "none";
        }
        function hide(id) {
            var e = document.getElementById(id);
            if (e != null)
                e.style.display = "none";
        }
        function show(id) {
            var e = document.getElementById(id);
            if (e != null)
                e.style.display = "";
        }


        function hide_show_all(idPrefix) {
            var list = document.querySelectorAll('*[id^="' + idPrefix + '"]');

            if (list.length == 0)
                return;

            var first = document.getElementById(list[0].id);
            if (first == null) return;
            var showing = first.style.display != "none";

            // loop through list of matching elements
            for (var i = 0, len = list.length; i < len; i++) {
                if (showing)
                    hide(list[i].id);
                else
                    show(list[i].id);
            }
        }
        function hide_all(idPrefix) {
            var list = document.querySelectorAll('*[id^="' + idPrefix + '"]');

            if (list.length == 0)
                return;

            // loop through list of matching elements
            for (var i = 0, len = list.length; i < len; i++) {
                hide(list[i].id);
            }
        }
        function show_all(idPrefix) {
            var list = document.querySelectorAll('*[id^="' + idPrefix + '"]');

            if (list.length == 0)
                return;

            // loop through list of matching elements
            for (var i = 0, len = list.length; i < len; i++) {
                show(list[i].id);
            }
        }

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1); });
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Call Centre</span></div>
        <div class="main_content_with_header">

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>


                <br />

                <asp:Panel  ID="pnlbuttons" runat="server" DefaultButton="btnSearch">
                <table>
                    <tr>
                        <td>Phone Number</td>
                        <td style="width:5px;"></td>
                        <td><asp:TextBox ID="txtPhoneNbrSearch" runat="server" style="width:100%;"></asp:TextBox></td>
                        <td style="width:3px;"></td>
                        <td><asp:Button ID="btnPhoneNbrClear" runat="server" Text="Clear" OnClientClick="document.getElementById('txtPhoneNbrSearch').value='';return false;" style="width:100%;" CssClass="thin_button" /></td>
                        <td rowspan="3" style="width:15px;"></td>                            
                        <td rowspan="3"><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" style="width:100%;" CssClass="thin_button" /></td>
                    </tr>
                    <tr>
                        <td>Surname</td>
                        <td style="width:5px;"></td>
                        <td><asp:TextBox ID="txtSurnameSearch" runat="server" style="width:100%;"></asp:TextBox></td>
                        <td style="width:3px;"></td>
                        <td><asp:Button ID="btnSurnameClear" runat="server" Text="Clear" OnClientClick="document.getElementById('txtSurnameSearch').value='';return false;" style="width:100%;" CssClass="thin_button" /></td>
                    </tr>
                    <tr>
                        <td>DOB</td>
                        <td style="width:5px;"></td>
                        <td>
                            <asp:DropDownList ID="ddlDOB_Day" runat="server"></asp:DropDownList>
                            <asp:DropDownList ID="ddlDOB_Month" runat="server"></asp:DropDownList>
                            <asp:DropDownList ID="ddlDOB_Year" runat="server"></asp:DropDownList>
                        </td>
                        <td style="width:3px;"></td>                            
                        <td><asp:Button ID="btnDOBClear" runat="server" Text="Clear" OnClientClick="document.getElementById('ddlDOB_Day').value='-1';document.getElementById('ddlDOB_Month').value='-1';document.getElementById('ddlDOB_Year').value='-1';return false;" style="width:100%;" CssClass="thin_button" /></td>
                    <tr>
                </table>
                </asp:Panel>

                <asp:Label ID="lblSearchResults" runat="server"></asp:Label>



                <hr width="70%" />
                <div style="height:15px;"></div>


                <a href="javascript:void(0)" onclick="show_all('heading_');return false;">Show All Clinics/Facs</a> 
                &nbsp;&nbsp;&nbsp;
                <a href="javascript:void(0)" onclick="hide_all('heading_');return false;">Hide All Clinics/Facs</a>

                <br />
                <br />
                <asp:Label ID="lblInfo1" runat="server"></asp:Label>


                <br />

                <hr width="70%" />
                <div style="height:3px;"></div>

                <h4>Export To Excel</h4>
                <asp:DropDownList ID="ddlDBs" runat="server" />
                &nbsp;
                <asp:Button ID="btnExportStaff" runat="server" OnClick="btnExportStaff_Click" Text="Export Staff" CssClass="thin_button" />


                <div style="height:12px;"></div>

                <hr width="70%" />
                
                <div style="height:3px;"></div>

                <a name="emailing_tag"></a>
                                
                <asp:Label ID="lblEmailErrorMessage" runat="server" ForeColor="Red"></asp:Label>


                <h4>Send Email To Clients</h4>
                <div style="height:6px;"></div>

                <b>Subject</b>&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtSubject" runat="server" style="width:650px;" onkeyup="capitalize_first(this);"></asp:TextBox>
                <div style="height:15px;"></div>

                <FTB:FreeTextBox id="FreeTextBox1" runat="Server" Text="" Width="725px" Height="305px" />
                <br />


                <table>
                    <tr>
                        <td>
                            <asp:DropDownList ID="ddlDBs2" runat="server" style="max-width:400px;" />
                        </td>
                        <td style="min-width:18px;"></td>
                        <td>
                            <asp:CheckBox ID="chkMasterAdminOnly" runat="server" Text="&nbsp;Master Admins Only" />
                            <br />
                            <asp:CheckBox ID="chkIgnore0001" runat="server" Text="&nbsp;Ignore 0001" Checked="true" />
                        </td>
                        <td style="min-width:18px;"></td>
                        <td>
                            <asp:Button ID="btnPreviewEmail" runat="server" OnClick="btnPreviewEmail_Click" Text="Preview" CssClass="thin_button" style="width:100%" />
                            <br />
                            <asp:Button ID="btnSendEmail" runat="server" OnClick="btnSendEmail_Click" Text="Send Emails" CssClass="thin_button" style="width:100%" OnClientClick="javascript:if (!confirm('This can not be undone. Are you sure?')) return false;" />

                        </td>
                    </tr>
                </table>

                <br />
                <br />

                <asp:Label ID="lblEmailOutput" runat="server"></asp:Label>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>
    
</asp:Content>



