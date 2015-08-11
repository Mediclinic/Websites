<%@ Page Title="Booking Sheet Blockout" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="BookingSheetBlockoutV2.aspx.cs" Inherits="BookingSheetBlockoutV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        addLoadEvent(function () {
            resize_panels();
        });

        function close_window() {
            window.returnValue = false; self.close();
        }

        function updateHourMinRows(chkBox) {
            if (chkBox.checked) {
                document.getElementById('tr_start_time_row').className += " hiddencol";
                document.getElementById('tr_end_time_row').className += " hiddencol";
            }
            else {
                document.getElementById("tr_start_time_row").className = document.getElementById("tr_start_time_row").className.replace(/(?:^|\s)hiddencol(?!\S)/, '');
                document.getElementById("tr_end_time_row").className = document.getElementById("tr_end_time_row").className.replace(/(?:^|\s)hiddencol(?!\S)/, '');
            }
        }

        function resize_panels() {
            resize_panel('pnlIndividualBookings');
            resize_panel('pnlRecurringBookings');
        }
        function resize_panel(id) {
            var panel = document.getElementById(id);
            if (panel != null) {
                var width_difference = parseInt(panel.offsetWidth) - parseInt(panel.clientWidth);
                panel.style.width += String(parseInt(panel.offsetWidth) + width_difference) + "px";
            }
        }

        function open_new_window(URL) {
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,Width=1300,Height=960");
            NewWindow.location = URL;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

        <div class="clearfix">
            <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server" Text="Booking Sheet Blockout" /></h3></div>
            <div class="main_content">


            <center>
                <div>
                    <table id="tblInfoOnSettingSpecificUnavailabilities" runat="server" class="block_center" >
                        <tr>
                            <td style="text-align:left;">
                                <asp:Label ID="lblMessage_SpecificOrgSpecificProvider_DoFromBookingScreen" runat="server" ForeColor="Blue" Text="* To blockout a specific provider at a specific organsiation, do so directly from the booking sheet"/>
                                <br />
                                <asp:Label ID="lblMessage_SpecificProvider_DoFromProviderInfoScreen" runat="server" ForeColor="Blue" Text="* To blockout a specific provider, do so directly from the provider info screen"/>
                                <br />
                                <asp:Label ID="lblMessage_SpecificOrg_DoFromOrgInfoScreen" runat="server" ForeColor="Blue" Text="* To blockout a specific organsiation, do so directly from the organisation info screen"/>
                            </td>
                        </tr>
                    </table>

                    <br />

                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>
            </center>


            <table class="block_center">
                <tr style="vertical-align:top;">
                    <td>

                        <b>Create New Blockouts</b>
                        <br />
                        <br />

                        <table id="maintable" runat="server">
                            <tr valign="top" id="tr_provider_row" runat="server">
                                <td class="nowrap">Provider: </td>
                                <td></td>
                                <td class="nowrap"><asp:Label ID="lblProvider" runat="server"/></td>
                            </tr>
                            <tr valign="top"  id="tr_provider_only_row" runat="server" class="hiddencol">
                                <td class="nowrap">Only This Provider: </td>
                                <td></td>
                                <td class="nowrap"><asp:CheckBox ID="chkOnlyThisProvider" runat="server" Checked="True" AutoPostBack="True" /></td>
                            </tr>

                            <tr height="15" id="tr_provider_space_row" runat="server"></tr>

                            <tr valign="top" id="tr_org_row" runat="server">
                                <td class="nowrap">Organistion: </td>
                                <td></td>
                                <td class="nowrap"><asp:Label ID="lblOrganistion" runat="server"/></td>
                            </tr>
                            <tr valign="top"  id="tr_organisation_only_row" runat="server" class="hiddencol">
                                <td class="nowrap">Only This Organistion: </td>
                                <td></td>
                                <td class="nowrap"><asp:CheckBox ID="chkOnlyThisOrganistion" runat="server" Checked="True" AutoPostBack="True" /></td>
                            </tr>

                            <tr height="15" id="tr_organisation_space_row" runat="server"></tr>

                            <tr>
                                <td valign="top">
                                    Days
                                </td>
                                <td></td>
                                <td>
                        
                                    <table border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td><label for="chkSunday">Sundays</label></td>
                                            <td style="width:5px"></td>
                                            <td><asp:CheckBox ID="chkSunday" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td><label for="chkMonday">Mondays</label></td>
                                            <td></td>
                                            <td><asp:CheckBox ID="chkMonday" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td><label for="chkTuesday">Tuesdays</label></td>
                                            <td></td>
                                            <td><asp:CheckBox ID="chkTuesday" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td><label for="chkWednesday">Wednesdays</label></td>
                                            <td></td>
                                            <td><asp:CheckBox ID="chkWednesday" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td><label for="chkThursday">Thursdays</label></td>
                                            <td></td>
                                            <td><asp:CheckBox ID="chkThursday" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td><label for="chkFriday">Fridays</label></td>
                                            <td></td>
                                            <td><asp:CheckBox ID="chkFriday" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td><label for="chkSaturday">Saturdays</label></td>
                                            <td></td>
                                            <td><asp:CheckBox ID="chkSaturday" runat="server" /></td>
                                        </tr>
                                    </table>
                        
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    Full Day: 
                                </td>
                                <td></td>
                                <td>
                                    <asp:CheckBox ID="chkAllDay" runat="server" onclick="updateHourMinRows(this);" />
                                </td>
                            </tr>

                            <tr id="tr_start_time_row">
                                <td>
                                    Start Time:
                                </td>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlStartHour" runat="server"></asp:DropDownList> :
                                    <asp:DropDownList ID="ddlStartMinute" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr id="tr_end_time_row">
                                <td>
                                    End Time:
                                </td>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlEndHour" runat="server"></asp:DropDownList> :
                                    <asp:DropDownList ID="ddlEndMinute" runat="server"></asp:DropDownList>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <b>Start Date: </b>
                                </td>
                                <td></td>
                                <td>
                                    <asp:TextBox ID="txtStartDate" runat="server" Columns="10"></asp:TextBox>
                                    <asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>End Date:</b>
                                </td>
                                <td></td>
                                <td>
                                    <asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox>
                                    <asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" />
                                    &nbsp;&nbsp;<asp:LinkButton ID="btnEndTimeHoverToolTip" runat="server" Text="?" ToolTip="Leave Blank To Have No End Date" OnClientClick="javascript:return false;" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Every:</b>
                                </td>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlEveryNWeeks" runat="server"/> Week(s)
                                </td>
                            </tr>

                            <tr style="height:8px">
                                <td colspan="3"/>
                            </tr>

                            <tr>
                                <td colspan="3">
                                    <table>
                                        <tr>
                                            <td class="nowrap"><label for="radBookingSequenceTypeSeperate">Create seperate unavailabilities</label></td>
                                            <td style="width:10px"></td>
                                            <td><asp:RadioButton ID="radBookingSequenceTypeSeperate" runat="server" GroupName="booking_sequence_type" /></td>
                                            <td style="width:15px"></td>
                                            <td><asp:LinkButton ID="btnCreateSeperateUnavailabilityToolTip" runat="server" Text="?" ToolTip="Deleting this will delete only that day's unavailability from the series this creates" OnClientClick="javascript:return false;" /></td>
                                        </tr>
                                        <tr>
                                            <td class="nowrap"><label for="radBookingSequenceTypeSeries">Create single series</label></td>
                                            <td style="width:10px"></td>
                                            <td><asp:RadioButton ID="radBookingSequenceTypeSeries" runat="server" GroupName="booking_sequence_type" /></td>
                                            <td style="width:15px"></td>
                                            <td><asp:LinkButton ID="btnCreateSeriesUnavailabilityToolTip" runat="server" Text="?" ToolTip="Deleting this will delete all unavailabilities this creates" OnClientClick="javascript:return false;" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr style="height:8px">
                                <td colspan="3"/>
                            </tr>

                            <tr>
                                <td>
                                    <b>Reason:</b>
                                </td>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="ddlProvUnavailabilityReason" runat="server"></asp:DropDownList>
                                    <asp:DropDownList ID="ddlOrgUnavailabilityReason" runat="server"></asp:DropDownList>
                                </td>
                            </tr>


                            <tr style="height:15px">
                                <td colspan="3"/>
                            </tr>
                            <tr>
                                <td colspan="3" style="text-align:center">
                                    <asp:Button ID="btnSubmit" Text="Book"   runat="server" OnClick="btnSubmit_Click" />
                                    &nbsp;&nbsp;
                                    <asp:Button ID="btnCancel" Text="Cancel" runat="server" OnClientClick="window.returnValue = false;self.close();" />

                                </td>
                            </tr>
                        </table>

                    </td>

                    <td style="width:90px"></td>

                    <td>
                        <center>

                        <b>Individual Blockouts</b>
                        <br />
                        <br />
                        <asp:Panel ID="pnlIndividualBookings" runat="server" ScrollBars="Auto" style="display:inline-block; max-height:230px; height:auto;">
                        <table class="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center" border="1" style="border-collapse:collapse;padding:3px;">
                            <tr>
                                <th>Date</th>
                                <th>Clnic/Facility</th>
                                <th>Provider</th>
                                <th></th>
                            </tr>
                            <asp:Repeater id="lstIndividualBookings" runat="server"  onitemdatabound="lstIndividualBookings_ItemDataBound">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr >
                                        <td class="nowrap">
                                            <asp:Label ID="lblAppointmentDateStart" runat="server" Text='<%# Eval("booking_date_start", "{0:dd-MM-yyyy}") %>'></asp:Label>&nbsp;&nbsp;&nbsp;
                                            <asp:Label ID="lblAppointmentTimeStart" runat="server" Text='<%# Eval("booking_date_start", "{0:HH:mm}") %>'></asp:Label>
                                            -
                                            <%--<asp:Label ID="lblAppointmentDateEnd"   runat="server" Text='<%# Eval("booking_date_end", "{0:dd-MM-yyyy}") %>'></asp:Label>&nbsp;&nbsp;&nbsp;--%>
                                            <asp:Label ID="lblAppointmentTimeEnd"   runat="server" Text='<%# Eval("booking_date_end", "{0:HH:mm}") %>'></asp:Label>
                                        </td>
                                        <td class="nowrap">
                                            <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("booking_organisation_id") == DBNull.Value ? "All" : Eval("organisation_name") %>'></asp:Label> 
                                        </td>
                                        <td class="nowrap">
                                            <asp:Label ID="lblProvider" runat="server" Text='<%#  Eval("booking_provider") == DBNull.Value ? "All" : (Eval("person_provider_firstname") + " " + Eval("person_provider_surname")) %>'></asp:Label> 
                                        </td>
                                        <td class="nowrap">
                                            <asp:LinkButton ID="btnDelete" runat="server" OnCommand="btnDelete_Command" CommandArgument='<%# Eval("booking_booking_id") %>' Text="Remove" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <tr id="footerTableRow" runat="server"><td colspan="4"><asp:Label ID="lblEmptyData" Text="No Record Found" runat="server"></asp:Label></td></tr>
                                </FooterTemplate>
                            </asp:Repeater>
                        </table>
                        </asp:Panel>

                        <br />
                        <br />

                        <b>Recurring (Series) Blockouts</b>
                        <br />
                        <br />
                        <asp:Panel ID="pnlRecurringBookings" runat="server" ScrollBars="Auto" style="display:inline-block; max-height:230px; height:auto;">
                        <table class="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center" border="1" style="border-collapse:collapse;padding:3px;">
                            <tr>
                                <th>Recurring Date Start</th>
                                <th>Recurring Date End</th>
                                <th>Time</th>
                                <th>Weekday</th>
                                <th>Clinic/Facility</th>
                                <th>Provider</th>
                                <th></th>
                            </tr>
                            <asp:Repeater id="lstRecurringBookings" runat="server" onitemdatabound="lstRecurringBookings_ItemDataBound">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr >
                                        <td class="nowrap">
                                            <asp:Label ID="lblAppointmentDateStart" runat="server" Text='<%# Eval("booking_date_start", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                        </td>
                                        <td class="nowrap">
                                            <asp:Label ID="lblAppointmentDateEnd" runat="server" Text='<%# Eval("booking_date_end", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                        </td>
                                        <td class="nowrap">
                                            <asp:Label ID="lblAppointmentTimeStart" runat="server" Text='<%# ((TimeSpan)Eval("booking_recurring_start_time")).Hours + ":" + PadLeft( ((TimeSpan)Eval("booking_recurring_start_time")).Minutes.ToString(), 2, "0" ) %>'></asp:Label>
                                            -
                                            <asp:Label ID="lblAppointmentTimeEnd" runat="server" Text='<%# ((TimeSpan)Eval("booking_recurring_end_time")).Hours     + ":" + PadLeft( ((TimeSpan)Eval("booking_recurring_end_time")).Minutes.ToString(),   2, "0" ) %>'></asp:Label>
                                        </td>
                                        <td class="nowrap">
                                            <asp:Label ID="lblWeekday" runat="server" Text='<%# WeekDayDB.GetDayOfWeek((int)Eval("booking_recurring_weekday_id")) + "s"  %>'></asp:Label> 
                                        </td>
                                        <td class="nowrap">
                                            <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("booking_organisation_id") == DBNull.Value ? "All" : Eval("organisation_name") %>'></asp:Label> 
                                        </td>
                                        <td class="nowrap">
                                            <asp:Label ID="lblProvider" runat="server" Text='<%#  Eval("booking_provider") == DBNull.Value ? "All" : (Eval("person_provider_firstname") + " " + Eval("person_provider_surname")) %>'></asp:Label> 
                                        </td>
                                        <td class="nowrap">
                                            <asp:LinkButton ID="btnDelete" runat="server" OnCommand="btnDelete_Command" CommandArgument='<%# Eval("booking_booking_id") %>' Text="Remove" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <tr id="footerTableRow" runat="server"><td colspan="6"><asp:Label ID="lblEmptyData" Text="No Record Found" runat="server"></asp:Label></td></tr>
                                </FooterTemplate>
                            </asp:Repeater>
                        </table>
                        </asp:Panel>


                        </center>
                    </td>
                </tr>

            </table>


            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>

  
            </div>
        </div>

</asp:Content>



