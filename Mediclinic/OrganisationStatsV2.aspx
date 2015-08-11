<%@ Page Title="Organisation Stats" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="OrganisationStatsV2.aspx.cs" Inherits="OrganisationStatsV2" %>

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

            var newHeight = document.documentElement.clientHeight - 375;
            document.getElementById('pnlOrgStats').style.maxHeight = newHeight + "px";
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
        <div class="page_title"><span id="lblHeading" runat="server">Organisation Statistics</span></div>
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

                            <td><asp:CheckBox ID="chkIncDeleted" runat="server" Text="" Font-Bold="false" OnCheckedChanged="btnSearch_Click" />&nbsp;<label for="chkIncDeleted" style="font-weight:normal;">Include Deleted Clinics/Facs</label></td>

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

                        <asp:Panel ID="pnlOrgStats" runat="server" ScrollBars="Auto" style="max-height:600px;">
                            <asp:Repeater id="lstOrgStats" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                        <tr>
                                            <th></th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Total Bookings</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Avg Consult Time</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">New Bookings Added By</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">New Patients</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <tr>
                                            <td class="nowrap text_left" >&nbsp;<asp:Label ID="lblOrg" Text='<%# Eval("name") %>' runat="server"></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblTotalBookings" runat="server" Text='<%# String.Format("{0:n0}", Eval("total_bookings")) %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblAvgConsultTime" runat="server" Text='<%# String.Format("{0:n0}", Eval("avg_minutes")) %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:Label ID="lblBookings" runat="server" Text='<%# String.Format("{0:n0}", Eval("n_bookings")) %>'></asp:Label>&nbsp;</td>
                                            <td class="nowrap" align="center">&nbsp;<asp:LinkButton ID="lnkPatients" runat="server" OnCommand="lnkPatients_Command" CommandArgument='<%# Eval("organisation_id") %>' Text='<%# String.Format("{0:n0}", Eval("n_patients"))  %>' />&nbsp;</td>
                                        </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                        <tr>
                                            <td align="center"><b>Total</b></td>
                                            <td align="center"><asp:Label ID="lblSum_TotalBookings" runat="server" Font-Bold="True"></asp:Label></td>
                                            <td align="center"><asp:Label ID="lblSum_AvgConsultTime" runat="server" Font-Bold="True"></asp:Label></td>
                                            <td align="center"><asp:Label ID="lblSum_Bookings" runat="server" Font-Bold="True"></asp:Label></td>
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
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Added By</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Patient</th>
                                            <th style="padding-left:5px;padding-right:5px;vertical-align:top;">Organisation</t>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <tr>
                                            <td class="nowrap text_left">
                                                &nbsp;
                                                <asp:Label ID="lblReferrer" runat="server" Text='<%# Eval("referrer_info_firstname") + " " + Eval("referrer_info_surname") %>'></asp:Label>
                                                <asp:Label ID="lblReferrerCount" runat="server" Text='<%# " (" + Eval("referrer_count") + ")" %>' Visible='<%# ((int)Eval("referrer_count")) > 0 ? true : false %>'></asp:Label>
                                                &nbsp;</td>

                                            <td class="nowrap text_left">
                                                &nbsp;
                                                <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("added_by") %>'></asp:Label>
                                                <asp:Label ID="lblAddedByCount" runat="server" Text='<%# " (" + Eval("added_by_count") + ")" %>' Visible='<%# ((int)Eval("added_by_count")) > 0 ? true : false %>'></asp:Label>
                                                &nbsp;</td>
                                            <td class="nowrap text_left">&nbsp;<a href='<%# "PatientDetailV2.aspx?type=view&id=" + Eval("patient_patient_id")  %>' onclick='<%# "open_new_tab(\"PatientDetailV2.aspx?type=view&id=" + Eval("patient_patient_id") + "\");return false;" %>' ><%# Eval("patient_person_firstname") + " " + Eval("patient_person_surname") %></a>&nbsp;</td>
                                            <td class="nowrap text_left">&nbsp;<%# Eval("organisation_name") %>&nbsp;</td>
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



