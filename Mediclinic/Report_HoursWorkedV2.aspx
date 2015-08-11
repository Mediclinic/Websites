<%@ Page Title="Schedule Report" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Report_HoursWorkedV2.aspx.cs" Inherits="Report_HoursWorkedV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Treatment Hours Report</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width"  style="width:1150px;">

                <div class="border_top_bottom user_login_form_no_width_div">
                    <center>

                        <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="btnDefaultButton_NoSubmit" style="margin:10px auto;">
                            <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />
                            <table>
                                <tr>

                                    <td  id="td_orgs_patients" runat="server" valign="middle">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:DropDownList ID="ddlOrgs" runat="server" OnSelectedIndexChanged="ddlOrgs_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr id="providerRow" runat="server">
                                                <td>
                                                    <asp:DropDownList ID="ddlProviders" runat="server" OnSelectedIndexChanged="ddlProviders_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td id="td_search_space2" runat="server" style="width:30px"></td>

                                    <td id="td_search_dates" runat="server">
                                        <table>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">Start Date: </asp:Label></td>
                                                <td class="nowrap"><asp:TextBox ID="txtStartDate" runat="server" Columns="10"/></td>
                                                <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                                                <td class="nowrap"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                                                <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:30px"></td>


                                    <td class="nowrap" style="vertical-align:middle;">
                                        <asp:CheckBox ID="chkIncAllSites" runat="server" Text="&nbsp;Inc All Sites"  CssClass="chkbox_spaceleft" />
                                        <br />
                                        <asp:CheckBox ID="chkIncIncompleteBookings" runat="server" Text="&nbsp;Inc Incomplete Bookings" CssClass="chkbox_spaceleft" />
                                        <br />
                                        <asp:CheckBox ID="chkIncPaidUnavailabilities" runat="server" Text="&nbsp;Inc Paid Unavailabilities" CssClass="chkbox_spaceleft" />
                                        <br />
                                        <asp:CheckBox ID="chkDateAcrossTop" runat="server" Text="&nbsp;Date Across Top" CssClass="chkbox_spaceleft" />
                                    </td>

                                    <td style="width:30px"></td>

                                    <td>
                                        <table border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td class="nowrap" align="center">
                                                    <asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" Width="100%" style="min-width:75px;" />
                                                    <div style="line-height:7px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap" align="center">
                                                    <asp:Button ID="btnExport" runat="server" Text="Export List" OnClick="btnExport_Click" Width="100%" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                </tr>
                            </table>
                        </asp:Panel>

                    </center>
                </div>

            </div>

            <div style="height:12px;"></div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:Label ID="lblScheduleTable" runat="server" ></asp:Label>

                </div>
            </center>
            

        </div>
    </div>

</asp:Content>



