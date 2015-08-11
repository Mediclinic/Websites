<%@ Page Title="Mass Marketing To Referrers (Bulk Letters, Emails, SMS)" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Letters_PrintBatchReferrersV2.aspx.cs" Inherits="Letters_PrintBatchReferrersV2" ValidateRequest="false" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function select_all(chkBox, selectBox) {

            // have we been passed an ID
            if (typeof selectBox == "string") {
                selectBox = document.getElementById(selectBox);
            }

            // is the select box a multiple select box?
            if (selectBox.type == "select-multiple") {
                for (var i = 0; i < selectBox.options.length; i++) {
                    selectBox.options[i].selected = chkBox.checked;
                }
            }
        }

        function clear_error_message() {
            document.getElementById('lblErrorMessage').value = '';
        }



        function get_referrer() {
            var retVal = window.showModalDialog("ReferrerListMultiplePopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:1275px;resizable:yes;center:yes;");

            if (typeof retVal === "undefined")
                return;

            if (retVal.length == 0)
                return;


            var ids = retVal.split("_");
            for (var i = 0; i < ids.length; i++) {
                var id = ids[i];

                // dont add if alread in there
                var alreadyAdded = false;
                var alreadyAddedList = document.getElementById('hiddenReferrerIDsList').value.split(',');
                for (var j = 0; j < alreadyAddedList.length; j++) {
                    if (alreadyAddedList[j] == id) {
                        alreadyAdded = true;
                        break;
                    }
                }

                if (!alreadyAdded) {
                    var itemNew = document.createElement('option');
                    itemNew.value = id;
                    itemNew.text = "";
                    try {
                        lstReferrers.add(itemNew, null); // standards compliant; doesn't work in IE
                    }
                    catch (ex) {
                        lstReferrers.add(itemNew); // IE only
                    }
                }
            }

            updateHiddenReferrerIDs();
            document.getElementById('btnUpdateReferrers').click();
        }


        function get_organisation() {
            var retVal = window.showModalDialog("OrganisationListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:1275px;resizable:yes;center:yes;");

            if (typeof retVal === "undefined")
                return;
            if (retVal.length == 0)
                return;

            var seperatorPos = retVal.indexOf(":");
            if (seperatorPos == -1)
                return;

            var org_id = retVal.substring(0, seperatorPos);
            var org_name = retVal.substring(seperatorPos + 1);

            // alert(retVal + "\r\n--" + org_id + "--\r\n--"+ org_name + "--")

            document.getElementById('hiddenUpdateReferrersFromClinic_OrgID').value = org_id;
            document.getElementById('btnUpdateReferrersFromClinic').click();
        }


        function remove_selected_referrer() {
            var lstReferrers = document.getElementById('lstReferrers');
            for (var i = lstReferrers.length - 1; i >= 0; i--) {
                if (lstReferrers.options[i].selected)
                    lstReferrers.remove(i);
            }
            updateHiddenReferrerIDs();
            document.getElementById('btnUpdateReferrers').click();
        }
        function updateHiddenReferrerIDs() {
            var items = "";
            var lstReferrers = document.getElementById('lstReferrers');
            for (var i = 0; i < lstReferrers.length; i++) {
                items = items + (items.length > 0 ? "," : "") + lstReferrers.options[i].value;
            }
            document.getElementById('hiddenReferrerIDsList').value = items;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Mass Marketing To Referrers (Bulk Letters, Emails, SMS)</asp:Label> &nbsp; <asp:HyperLink ID="lnkToEntity" runat="server"/></div>
        <div class="main_content" style="padding:20px 5px;">

            <center>

                <div class="block_center">
                    <table>
                        <tr>
                            <td style="text-align:left;">
                                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td style="text-align:left;">
                                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>


                <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="btnDefaultButton_NoSubmit" style="margin:6px auto;">
                    <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />

                    <table>

                        <tr style="vertical-align:top;">
                            <td>

                                <center>
                                    <table>

                                        <tr  style="vertical-align:top;">
                                            <td>

                                                <center>
                                                    <table>

                                                        <tr>
                                                            <th colspan="3">1. Choose Method Of Correspondence</th>
                                                        </tr>
                                                        <tr style="height:10px;">
                                                            <td colspan="3"></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Has Both Email & Mobile</td>
                                                            <td style="min-width:6px;"></td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlBothMobileAndEmail" runat="server">
                                                                    <asp:ListItem Text="Email" Value="2"></asp:ListItem>
                                                                    <asp:ListItem Text="Print" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="SMS" Value="3"></asp:ListItem>
                                                                    <asp:ListItem Text="Nothing" Value="-1"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Has Email, No Mobile</td>
                                                            <td></td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlEmailNoMobile" runat="server">
                                                                    <asp:ListItem Text="Email" Value="2"></asp:ListItem>
                                                                    <asp:ListItem Text="Print" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="Nothing" Value="-1"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Has Mobile, No Email</td>
                                                            <td></td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlMobileNoEmail" runat="server">
                                                                    <asp:ListItem Text="Print" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="SMS" Value="3"></asp:ListItem>
                                                                    <asp:ListItem Text="Nothing" Value="-1"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Has Neither Mobile Or Email</td>
                                                            <td></td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlNeitherMobileOrEmail" runat="server">
                                                                    <asp:ListItem Text="Print" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="Nothing" Value="-1"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                 </center>

                                            </td>
                                            <td></td>
                                            <td rowspan="6">

                                                <center>
                                                    <table>
                                                        <tr>
                                                            <th colspan="3">Email & SMS Text Merge Fields</th>
                                                        </tr>
                                                        <tr style="height:10px;">
                                                            <td colspan="3"></td>
                                                        </tr>
                                                        <tr><td>{ref_name}</td><td style="min-width:20px;"></td><td>Referrer Full Name</td></tr>
                                                        <tr><td>{ref_title}</td><td></td><td>Referrer Title</td></tr>
                                                        <tr><td>{ref_firstname}</td><td></td><td>Referrer Firstname</td></tr>
                                                        <tr><td>{ref_surname}</td><td></td><td>Referrer Surname</td></tr>
                                                    </table>
                                                </center>

                                            </td>
                                        </tr>

                                        <tr style="height:10px;">
                                            <td></td>
                                            <td></td>
                                        </tr>

                                        <tr>
                                            <th>2. Printed Batch Letters - Email Address To Send Document To Print</th>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td><asp:TextBox ID="txtEmailForPrinting" runat="server" Width="525px"></asp:TextBox></td>
                                            <td></td>
                                        </tr>

                                        <tr style="height:30px;">
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th>3a. Email Correspondence - Email Subject</th>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td><asp:TextBox ID="txtEmailSubject" runat="server" Width="525px"></asp:TextBox></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr style="height:10px;">
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th>3b. Email Correspondence - Email Text</th>
                                            <td></td>
                                            <th>4. SMS Correspondence - SMS Text</th>
                                        </tr>
                                        <tr style="vertical-align:top;">
                                            <td style="width:48%;">

                                                <FTB:FreeTextBox id="FreeTextBox1" runat="Server" Text="" Width="525px" Height="200px" />

                                            </td>
                                            <td style="min-width:40px;"></td>
                                            <td style="width:48%;">

                                                <asp:TextBox ID="txtSMSText" runat="server" Width="525px" Height="288px" TextMode="MultiLine"></asp:TextBox>

                                            </td>
                                        </tr>
                                    </table>
                                </center>

                            </td>
                        </tr>

                        <tr style="height:40px;">
                            <td></td>
                        </tr>

                        <tr style="vertical-align:top;">
                            <td>

                                <center>
                                    <table id="tbl_select_ref_and_patient" runat="server">
                                        <tr>
                                            <th style="text-align:left;">5. Select Referrer</th>
                                            <th></th>
                                            <td style="text-align:left">
                                                <table style="width:100%;">
                                                    <tr>
                                                        <th style="text-align:left">6. Select Letter</th>
                                                        <td style="text-align:right">
                                                            <table>
                                                                <tr>
                                                                    <td><asp:CheckBox ID="chkKeepInHistory" runat="server" Text="&nbsp;Keep In History" Checked="false" /></td>
                                                                    <td style="width:10px"></td>
                                                                    <td><asp:Button ID="btnPrint" runat="server" Text="&nbsp;&nbsp;&nbsp;Run&nbsp;&nbsp;&nbsp;" OnClick="btnPrint_Click" OnClientClick="javascript:clear_error_message();" /></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr id="tr_regs_search_row_space_below" runat="server" height="15">
                                            <td colspan="5"></td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <table style="line-height:normal;width:100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblReferrersWithMobileAndEmail" runat="server"><label for="chkReferrersWithMobileAndEmail">Total  w/ Both Email & Mobile</label></asp:Label>
                                                        </td>
                                                        <td style="min-width:4px;"></td>
                                                        <td>
                                                            <asp:Label ID="lblReferrersWithMobileAndEmailTotal" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td style="min-width:10px;"></td>
                                                        <td rowspan="3"  style="text-align:right;">
                                                            <asp:Label ID="lblTotalText" runat="server" Text="&nbsp;Total: "></asp:Label>
                                                            <asp:Label ID="lblReferrerCount" runat="server" Text="0"></asp:Label>
                                                        </td>

                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblReferrersWithEmailNoMobile" runat="server"><label for="chkReferrersWithEmailNoMobile">Total w/ Email, No Mobile</label></asp:Label>
                                                        </td>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblReferrersWithEmailNoMobileTotal" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblReferrersWithMobileNoEmail" runat="server"><label for="chkReferrersWithMobileNoEmail">Total w/ Mobile, No Email</label></asp:Label>
                                                        </td>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblReferrersWithMobileNoEmailTotal" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblReferrersWithNeitherMobileOrEmail" runat="server"><label for="chkReferrersWithNeitherMobileOrEmail">Total w/ Neither Mobile or Email</label></asp:Label>
                                                        </td>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lblReferrersWithNeitherMobileOrEmailTotal" runat="server" Text="0"></asp:Label>
                                                        </td>
                                                        <td></td>
                                                        <td style="text-align:right;">All/None <input id="chkSelectAllReferrers" onclick="select_all(this, 'lstReferrers')" type="checkbox" value="Accept Form" name="chkSelectAllReferrers" runat="server" /></td>
                                                    </tr>
                                                </table>

                                                <div style="min-height:6px;"></div>

                                                <asp:Button ID="btnAddReferrer" runat="server" Text="Add" OnClientClick="javascript:get_referrer(); return false;"/>
                                                <asp:Button ID="btnAddReferrerFromClinic" runat="server" Text="Add From Clinic" OnClientClick="javascript:get_organisation(); return false;"/>
                                                <asp:Button ID="btnAddAllReferrers" runat="server" Text="Add All" OnClick="btnAddAllReferrers_Click" />
                                                <asp:Button ID="btnDeleteSelected" runat="server" Text="Delete Selected" OnClientClick="javascript:remove_selected_referrer(); return false;" />
                                                <asp:Button ID="btnUpdateReferrers" runat="server" CssClass="hiddencol" OnClick="btnUpdateReferrers_Click" />
                                                <asp:HiddenField ID="hiddenReferrerIDsList" runat="server" />

                                                <asp:Button ID="btnUpdateReferrersFromClinic" runat="server" CssClass="hiddencol" OnClick="btnUpdateReferrersFromClinic_Click" />
                                                <asp:HiddenField ID="hiddenUpdateReferrersFromClinic_OrgID" runat="server" />

                                            </td>
                                            <td style="width:150px"></td>
                                            <td style="text-align:right;">

                                            </td>
                                        </tr>

                                        <tr>
                                            <td><asp:ListBox ID="lstReferrers" runat="server" rows="32" SelectionMode="Multiple" Width="100%" style="min-width:350px;"></asp:ListBox></td>
                                            <td></td>
                                            <td><asp:ListBox ID="lstLetters" runat="server" rows="32" SelectionMode="Single" style="min-width:350px;"></asp:ListBox></td>
                                        </tr>

                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>
                                </center>

                            </td>
                        </tr>
                    </table>

                </asp:Panel>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>


        </div>
    </div>


</asp:Content>



