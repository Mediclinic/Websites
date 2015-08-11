<%@ Page Title="Staff Stats" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StaffStatsV2.aspx.cs" Inherits="StaffStatsV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">


        addLoadEvent(function () {
            resize(true);
            setTimeout(function () { resize_panel_width('pnlOrgStats'); resize_panel_width('pnlPatients'); }, 50);
        });

        window.onresize = function (event) { resize(false); };
        //window.onload = resize(true); // has to be after onresize for some reason


        function resize(onload) {

            if (onload) {
                // only seems to work with some timeout after page is loaded
                setTimeout(function () { resize(false); }, 25);
                return;
            }

            var newHeight = document.documentElement.clientHeight - 400;

            document.getElementById('pnlStaffStats').style.maxHeight = newHeight + "px";
            document.getElementById('pnlPatients').style.maxHeight = newHeight + "px";
        }


        function resize_panel_width(pnlName) {
            var panel = document.getElementById(pnlName);
            if (panel != null) {
                var width_difference = parseInt(panel.offsetWidth) - parseInt(panel.clientWidth);
                panel.style.width += String(parseInt(panel.offsetWidth) + width_difference) + "px";
            }
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Staff Statistics</span></div>
        <div class="main_content_with_header">
            <div class="user_login_form">


                <div class="border_top_bottom">
                    <table class="block_center">
                        <tr>
                            <td><asp:Label ID="lblSearchDate" runat="server">Start Date: </asp:Label></td>
                            <td><asp:TextBox ID="txtStartDate" runat="server" Columns="10"></asp:TextBox></td>
                            <td><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>

                            <td style="width:15px"></td>

                            <td><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                            <td><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                            <td><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>

                            <td style="width:15px"></td>

                            <td><asp:CheckBox ID="chkIncDeleted" runat="server" Text="" Font-Bold="false" OnCheckedChanged="btnSearch_Click" />&nbsp;<label for="chkIncDeleted" style="font-weight:normal;">Include Inactive</label></td>

                            <td style="width:15px"></td>

                            <td><asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" /></td>

                        </tr>
                    </table>
                </div>

            </div>


            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>
           
            <table class="block_center">
                <tr>
                    <td style="vertical-align:top">

                        <asp:Panel ID="pnlStaffStats" runat="server" ScrollBars="Auto" style="max-height:600px;">
                            <asp:Repeater id="lstStaffStats" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                        <tr>
                                            <th></th>
                                            <th></th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Total Bookings</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Avg Consult Time</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">New Bookings Added By</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Completions</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Receipts</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Owing</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Total</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">New Patients</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <tr>
                                            <td class="nowrap text_left" >&nbsp;<asp:Label ID="lblStaff" Text='<%# Eval("firstname") + " " + Eval("surname") %>' runat="server"></asp:Label>&nbsp;</td>
                                            <td class="nowrap text_left" >&nbsp;<asp:Label ID="lblType" Text='<%# (Eval("is_external") == DBNull.Value || Eval("staff_id") == DBNull.Value || !((bool)Eval("is_external")) || (int)Eval("staff_id") < 0 ? "" : " <font color=\"#DF7401\">External</font>") + 
                                                                                (Eval("staff_id")    == DBNull.Value || (int)Eval("staff_id") > 0 ? "" : " <font color=\"#0000A6\"></font>") +
                                                                                (Eval("staff_id")    == DBNull.Value || (int)Eval("staff_id") < 0  || (!(bool)Eval("is_stakeholder") && !(bool)Eval("is_master_admin") && !(bool)Eval("is_admin"))  ? "" : " <font color=\"#0000A6\">Admin</font>") +
                                                                                (Eval("staff_id")    == DBNull.Value || (int)Eval("staff_id") < 0  || !(bool)Eval("is_provider")  ? "" : " <font color=\"#008000\">Provider</font>") 
                                                                                %>' runat="server"></asp:Label>&nbsp;</td>


                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblTotalBookings"  runat="server" Text='<%# Eval("total_bookings") == DBNull.Value ? "" : String.Format("{0:n0}", Eval("total_bookings"))  %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblAvgConsultTime" runat="server" Text='<%# Eval("avg_minutes")    == DBNull.Value ? "" : String.Format("{0:n0}", Eval("avg_minutes"))     %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblBookings"       runat="server" Text='<%# Eval("n_bookings")     == DBNull.Value ? "" : String.Format("{0:n0}", Eval("n_bookings"))      %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblCompletions"    runat="server" Text='<%# Eval("n_completions")  == DBNull.Value ? "" : String.Format("{0:n0}", Eval("n_completions"))   %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblReceipts"       runat="server" Text='<%# Eval("sum_receipts")   == DBNull.Value ? "" : String.Format("{0:C}",  Eval("sum_receipts"))    %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblOwing"          runat="server" Text='<%# Eval("total_owing")    == DBNull.Value ? "" : String.Format("{0:C}",  Eval("total_owing"))     %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblRcptsPlusOwing" runat="server" Text='<%# Eval("total_in")       == DBNull.Value ? "" : String.Format("{0:C}",  Eval("total_in"))        %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:LinkButton ID="lnkPatients"  runat="server" OnCommand="lnkPatients_Command" CommandArgument='<%# Eval("staff_id") %>' Text='<%# Eval("n_patients") %>' />&nbsp;</td>
                                        </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                        <tr>
                                            <td align="center"><b>Total</b></td>
                                            <td align="center"></td>
                                            <td align="center"><asp:Label ID="lblSum_TotalBookings" runat="server" Font-Bold="True"></asp:Label></td>
                                            <td align="center"><asp:Label ID="lblSum_AvgConsultTime" runat="server" Font-Bold="True"></asp:Label></td>
                                            <td align="center"><asp:Label ID="lblSum_Bookings" runat="server" Font-Bold="True"></asp:Label></td>
                                            <td align="center"><asp:Label ID="lblSum_Completions" runat="server" Font-Bold="True"></asp:Label></td>
                                            <td align="center"><asp:Label ID="lblSum_Receipts" runat="server" Font-Bold="True"></asp:Label></td>
                                            <td align="center"><asp:Label ID="lblSum_Owing" runat="server" Font-Bold="True"></asp:Label></td>
                                            <td align="center"><asp:Label ID="lblSum_total" runat="server" Font-Bold="True"></asp:Label></td>
                                            <td align="center"><asp:Label ID="lblSum_Patients" runat="server" Font-Bold="True"></asp:Label></td>
                                        </tr>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </asp:Panel>

                        <br />
                        <div style="height:28px;"></div>

                    </td>

                    <td style="width:30px"></td>

                    <td valign="top">

                        <asp:Panel ID="pnlPatients" runat="server" ScrollBars="Auto" style="max-height:600px;">
                            <asp:Repeater id="lstPatients" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                        <tr>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Referrer</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Patient</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Added By</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <tr>
                                            <td class="nowrap text_left">
                                                &nbsp;
                                                <asp:Label ID="lblReferrer" runat="server" Text='<%# Eval("referrer_info_firstname") + " " + Eval("referrer_info_surname") %>'></asp:Label>
                                                <asp:Label ID="lblReferrerCount" runat="server" Text='<%# " (" + Eval("referrer_count") + ")" %>' Visible='<%# ((int)Eval("referrer_count")) > 0 ? true : false %>'></asp:Label>
                                                &nbsp;</td>
                                            <td class="nowrap text_left">&nbsp;<a href='<%# "PatientDetailV2.aspx?type=view&id=" + Eval("patient_id")  %>' onclick='<%# "open_new_tab(\"PatientDetailV2.aspx?type=view&id=" + Eval("patient_id") + "\");return false;" %>' ><%# Eval("firstname") + " " + Eval("surname") %></a>&nbsp;</td>
                                            <td class="nowrap text_left">&nbsp;<asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("added_by_name") %>'></asp:Label>&nbsp;</td>
                                        </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </asp:Panel>


                        <br />
                        <center>
                            <asp:Button ID="btnExport" runat="server" Text="Export" OnCommand="btnExport_Command" Visible="False" />
                        </center>

                    </td>

                </tr>
            </table>

        </div>
    </div>


</asp:Content>



