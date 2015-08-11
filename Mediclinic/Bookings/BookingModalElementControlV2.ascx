<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BookingModalElementControlV2.ascx.cs" Inherits="Bookings_BookingModalElementControlV2" %>

  <div style="display: none;"  class="contextMenu" id="ContextMenu_Past_Completed_Has_Invoice_Clinic">
    <ul style="min-width:150px;">
      <li id="pchi_viewinvoice_cl">View Invoice</li>
      <li id="pchi_printinvoice_cl">Print Invoice</li>
      <li id="pchi_emailinvoice_cl">Email Invoice</li>
      <li id="pchi_reversebooking_cl">Reverse</li>
      <li id="pchi_printletters_cl">Letters</li>
      <li id="pchi_futurebooking_cl">Make Future Booking</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="ContextMenu_Past_Completed_Has_Invoice_AC">
    <ul style="min-width:150px;">
      <li id="pchi_viewinvoice_ac">View Invoice</li>
      <li id="pchi_treatmentlist_ac">Treatment List</li>
      <li id="pchi_printinvoice_ac">Print Invoice</li>
      <li id="pchi_emailinvoice_ac">Email Invoice</li>
      <li id="pchi_reversebooking_ac">Reverse</li>
      <li id="pchi_printletters_ac">Letters</li>
      <li id="pchi_futurebooking_ac">Make Future Booking</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="ContextMenu_Past_Completed_No_Invoice">
    <ul style="min-width:150px;">
      <li id="pcni_reversebooking">Reverse</li>
      <li id="pcni_printletters">Letters</li>
      <li id="pcni_futurebooking">Make Future Booking</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="ContextMenu_Past_Uncompleted_Clinic">
    <ul style="white-space:nowrap;min-width:125px;">
      <li id="pu_complete_cl">Complete</li>
      <li id="pu_edit_cl">Edit / Move</li>
      <li id="pu_cancel_no_fee_cl">Cancel (no fee)</li>
      <li id="pu_cancel_w_fee_cl">Cancel (charge)</li>
      <li id="pu_delete_cl">Delete</li>
      <li id="pu_deceased_cl">Deceased</li>
      <li id="pu_printletters_cl">Letters</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="ContextMenu_Past_Uncompleted_AC">
    <ul style="white-space:nowrap;min-width:125px;">
      <li id="pu_complete_ac">Complete</li>
      <li id="pu_add_patients_ac">Add Patients</li>
      <li id="pu_edit_ac">Edit / Move</li>
      <li id="pu_cancel_no_fee_ac">Cancel (no fee)</li>
      <li id="pu_cancel_w_fee_ac">Cancel (charge)</li>
      <li id="pu_delete_ac">Delete</li>
      <li id="pu_deceased_ac">Deceased</li>
      <li id="pu_printletters_ac">Letters</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="ContextMenu_Future_Clinic">
    <ul style="white-space:nowrap;">
      <li id="f_edit_cl">Edit / Move</li>
      <li id="f_delete_cl">Delete</li>
      <li id="f_printletters_cl">Letters</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="ContextMenu_Future_AC">
    <ul style="white-space:nowrap;">
      <li id="f_add_patients_ac">Add Patients</li>
      <li id="f_edit_ac">Edit / Move</li>
      <li id="f_delete_ac">Delete</li>
      <li id="f_printletters_ac">Letters</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="ContextMenu_Future_PatientLoggedIn">
    <ul style="white-space:nowrap;">
      <li id="fpt_edit">Edit / Move</li>
      <li id="fpt_delete">Delete</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="addableMenu">
    <ul style="min-width:150px;">
      <li id="add">Book</li>
      <li id="add_multiple_days">Book Multiple Days</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="addableMenu_PatientLoggedIn">
    <ul style="min-width:150px;">
      <li id="add_pt_loggedin">Book</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="takenButAddableMenu">
    <ul style="min-width:150px;">
      <li id="taken_add">Book</li>
      <li id="taken_add_multiple_days">Book Multiple Days</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="takenButUpdatableMenu">
    <ul style="min-width:150px;">
      <li id="taken_update_newtime">Change To This Time</li>
      <li id="taken_update_cancel_edit">Cancel Edit / Move</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="updateableMenu">
    <ul style="min-width:150px;">
      <li id="update_newtime">Change To This Time</li>
      <li id="update_cancel_edit">Cancel Edit / Move</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="takenMenu">
    <ul>
      <li>Slot Taken</li>
    </ul>
  </div>


  <div style="display: none;"  class="contextMenu" id="fullDaysAddableMenu">
    <ul style="min-width:230px;">
      <!--<li id="full_days_add">Set Days Unavailable</li>-->
      <li id="full_days_blockout_single_day">Blockout Today</li>
      <li id="full_days_blockout_multiple_days">Blockout Multiple Days</li>
      <li id="full_days_blockout_multiple_days_series">Blockout Multiple Days As Series</li>
      <li id="full_days_paid_blockout_single_day">Paid Blockout Today</li>
      <li id="full_days_paid_blockout_multiple_days">Paid Blockout Multiple Days</li>
      <li id="full_days_move">Move Incomplete To Another Prov</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="fullDaysUpdatableMenu">
    <ul style="min-width:165px;">
      <li id="update_fullday">Move To This Provider</li>
    </ul>
  </div>


  <div style="display: none;"  class="contextMenu" id="fullDayEditable">
    <ul>
      <li id="full_days_make_booking">Make Booking</li>
      <li id="full_days_delete">Remove Unavailability</li>
    </ul>
  </div>

  <div style="display: none;"  class="contextMenu" id="emptyContextMenu">
  </div>

  <div style="display: none;"  class="contextMenu" id="patientAndServiceNotSetMenu">
    <ul style="min-width:210px;">
      <li>Select A Patient & Service First</li>
    </ul>
  </div>
  <div style="display: none;"  class="contextMenu" id="patientNotSetMenu">
    <ul style="min-width:145px;">
      <li>Select A Patient First</li>
    </ul>
  </div>
  <div style="display: none;"  class="contextMenu" id="serviceNotSetMenu">
    <ul style="min-width:155px;">
      <li>Select A Service First</li>
    </ul>
  </div>


<div style="display: none; z-index: 2000"  id="modalPage">
    <div class="modalBackground"></div>
    <div class="modalContainer">
        <div id="spnModalPage" class="modalPopup">
            <div class="modalBody">


                <table width="100%" height="100%">
                    <tr>
                        <td align="center" valign="middle">


                            <span id="modalDate" class="hiddencol"></span>
                            <table style="min-width:85%;">
                                <tr>
                                    <td colspan="3">
                                            <h3><span id="modalTitle">New Booking</span></h3>
                                    </td>
                                </tr>

                                <tr id="tr_clash_bookings_bk" class="hiddencol">
                                    <td colspan="3">
                                        <br />
                                        <font color="red">Please move/delete existing bookings first:</font>
                                        <br />
                                        <asp:Panel ID="pnl_clash_bookings_bk" runat="server" ScrollBars="Auto" Width="100%" style="max-height:50px;">
                                            <span id="spn_clash_bookings_bk" class="nowrap"></span>
                                        </asp:Panel>
                                    </td>
                                </tr>

                                <tr height="10">
                                    <td colspan="3"/>
                                </tr>
                                <tr id="serviceRow">
                                    <td>
                                        Service:
                                    </td>
                                    <td style="width:8px"></td>
                                    <td id="modalServiceDescr"></td>
                                </tr>
                                <tr id="serviceTrailingSpaceRow" height="5">
                                    <td colspan="3"/>
                                </tr>
                                <tr style="vertical-align:top;">
                                    <td id="orgRow">
                                        Org: 
                                    </td>
                                    <td></td>
                                    <td id="modalOrgDescr"></td>
                                </tr>
                                <tr>
                                    <td>
                                        Provider: 
                                    </td>
                                    <td></td>
                                    <td id="modalProviderDescr"></td>
                                </tr>
                                <tr>
                                    <td>
                                        Date: 
                                    </td>
                                    <td></td>
                                    <td id="modalServiceDate"></td>
                                </tr>
                                <tr>
                                    <td>
                                        Start Time:
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlModalStartHour" runat="server"></asp:DropDownList> :
                                        <asp:DropDownList ID="ddlModalStartMinute" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        End Time:
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlModalEndHour" runat="server"></asp:DropDownList> :
                                        <asp:DropDownList ID="ddlModalEndMinute" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>

                                <tr  id="tr_ModalPopupRecurring_weeks_space" style="height:8px">
                                    <td colspan="3"/>
                                </tr>

                                <tr id="tr_ModalPopupRecurring_every_n_weeks">
                                    <td>
                                        Every:
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlEveryNWeeks" runat="server"/> Week(s) &nbsp; For <asp:DropDownList ID="ddlOcurrences" runat="server"/> occurrence(s)
                                    </td>
                                </tr>

                                <tr style="height:8px">
                                    <td colspan="3"/>
                                </tr>

                                <tr>
                                    <td>
                                        Confirmed: 
                                    </td>
                                    <td></td>
                                    <td>
                                        <input type="checkbox" id="chkConfirmed" />
                                    </td>
                                </tr>
                                <tr id="editReasonRow">
                                    <td class="nowrap">
                                        Edit reason: 
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlBookingMovementReason" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr height="15">
                                    <td colspan="3"/>
                                </tr>
                                <tr>
                                    <td colspan="3" align="center" class="tfoot">
                                        <asp:Button ID="btnModalSubmitAdd"    Text="Book"   runat="server" OnClientClick="add_booking();return false;" />
                                        <asp:Button ID="btnModalSubmitUpdate" Text="Update" runat="server" OnClientClick="update_booking();return false;" />
                                        &nbsp;&nbsp;&nbsp;
                                        <asp:Button ID="btnModalCancel" Text="Cancel" runat="server" OnClientClick="hide_modal('modalPage'); return false;" />

                                    </td>
                                </tr>
                            </table>


                        </td>
                    </tr>
                </table>


            </div>
        </div>
    </div>
</div>

<div style="display: none; z-index: 2000"  id="modalPopupBookFullDays">
    <div class="modalBackground"></div>
    <div class="modalContainer">
        <div class="modalFullDays">
            <div class="modalBody">

                <span id="modalPopupBookFullDaysDate" class="hiddencol"></span>
                <table width="100%" height="100%">
                    <tr>
                        <td colspan="3">
                                <h3><span>Set Unavailable Days</span></h3>
                        </td>
                    </tr>
                    <tr height="10">
                        <td colspan="2"/>
                    </tr>

                    <tr>
                        <td>
                            Provider: 
                        </td>
                        <td id="modalPopupBookFullDays_ProviderDescr"></td>
                    </tr>

                    <tr>
                        <td>
                            Start Date: 
                        </td>
                        <td>
                            <asp:TextBox ID="txtModalPopupBookFullDays_StartDate" runat="server" Width="80"></asp:TextBox>
                            <asp:ImageButton ID="txtModalPopupBookFullDays_StartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            End Time:
                        </td>
                        <td>
                            <asp:TextBox ID="txtModalPopupBookFullDays_EndDate" runat="server" Width="80"></asp:TextBox>
                            <asp:ImageButton ID="txtModalPopupBookFullDays_EndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" />
                        </td>
                    </tr>
                    <tr height="15">
                        <td colspan="2"/>
                    </tr>
                    <tr>
                        <td colspan="2" align="center" class="tfoot">
                            <asp:Button ID="btnModalPopupBookFullDays_SubmitAdd"    Text="Set Unavailable"   runat="server" OnClientClick="add_fulldays_booking(); return false;" />
                            <asp:Button ID="btnModalPopupBookFullDays_SubmitUpdate" Text="Update"            runat="server" OnClientClick="update_fulldays_booking(); return false;" />
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnModalPopupBookFullDays_Cancel"       Text="Cancel"            runat="server" OnClientClick="hide_modal('modalPopupBookFullDays'); return false;" />

                        </td>
                    </tr>
                </table>

            </div>
        </div>
    </div>
</div>



<div style="display: none; z-index: 2000"  id="modalPopupUnavailableRecurring">
    <div class="modalBackground"></div>
    <div class="modalContainer">
        <div id="spnModalPopupUnavailableRecurring" class="modalPopupUnavailableRecurring">
            <div class="modalBody">

                <asp:HiddenField ID="modalPopupUnavailableRecurring_IsPaid" runat="server" />

                <table style="width:100%;">
                    <tr>
                        <td colspan="3">
                            <h3><span id="modalPopupUnavailableRecurring_Title">Add Recurring Unavailability</span></h3>
                        </td>
                    </tr>

                    <tr id="tr_clash_bookings" class="hiddencol">
                        <td colspan="3">
                            <br />
                            <font color="red">Please move/delete existing bookings first:</font>
                            <br />
                            <asp:Panel ID="pnl_clash_bookings" runat="server" ScrollBars="Auto" Width="100%" style="max-height:50px;">
                                <span id="spn_clash_bookings" class="nowrap"></span>
                            </asp:Panel>

                        </td>
                    </tr>
                </table>

                <table style="width:100%;">
                    <tr style="height:10px">
                        <td colspan="3"/>
                    </tr>
                    <tr valign="top">
                        <td>
                            Provider: 
                        </td>
                        <td></td>
                        <td id="modalPopupUnavailableRecurring_ProviderDescr"></td>
                    </tr>
                    <tr valign="top">
                        <td>
                            Organistion: 
                        </td>
                        <td></td>
                        <td id="modalPopupUnavailableRecurring_OrgDescr"></td>
                    </tr>

                    <tr id="tr_ModalPopupUnavailableRecurring_onlyThisOrg" valign="top">
                        <td style="white-space:nowrap;">
                            Only This Org:&nbsp;
                        </td>
                        <td></td>
                        <td>
                            <input type="checkbox" id="chkModalPopupUnavailableRecurringOnlyThisOrg" checked="checked" />
                        </td>
                    </tr>
                    <tr id="tr_ModalPopupUnavailableRecurring_days">
                        <td valign="top">
                            Days
                        </td>
                        <td></td>
                        <td>
                        
                            <table>
                                <tr>
                                    <td>Sundays</td>
                                    <td style="width:5px"></td>
                                    <td><input type="checkbox" id="chkModalPopupUnavailableRecurring_Sunday" /></td>
                                </tr>
                                <tr>
                                    <td>Mondays</td>
                                    <td></td>
                                    <td><input type="checkbox" id="chkModalPopupUnavailableRecurring_Monday" /></td>
                                </tr>
                                <tr>
                                    <td>Tuesdays</td>
                                    <td></td>
                                    <td><input type="checkbox" id="chkModalPopupUnavailableRecurring_Tuesday" /></td>
                                </tr>
                                <tr>
                                    <td>Wednesdays</td>
                                    <td></td>
                                    <td><input type="checkbox" id="chkModalPopupUnavailableRecurring_Wednesday" /></td>
                                </tr>
                                <tr>
                                    <td>Thursdays</td>
                                    <td></td>
                                    <td><input type="checkbox" id="chkModalPopupUnavailableRecurring_Thursday" /></td>
                                </tr>
                                <tr>
                                    <td>Fridays</td>
                                    <td></td>
                                    <td><input type="checkbox" id="chkModalPopupUnavailableRecurring_Friday" /></td>
                                </tr>
                                <tr>
                                    <td>Saturdays</td>
                                    <td></td>
                                    <td><input type="checkbox" id="chkModalPopupUnavailableRecurring_Saturday" /></td>
                                </tr>
                            </table>
                        
                        </td>
                    </tr>

                    <tr style="height:6px;">
                        <td colspan="3"></td>
                    </tr>

                    <tr id="tr_ModalPopupUnavailableRecurringModal_fullDay">
                        <td>
                            Full Day: 
                        </td>
                        <td></td>
                        <td>
                            <input type="checkbox" id="chkModalPopupUnavailableRecurring_AllDay" onclick="updateModalPopupUnavailableRecurring_HourMinRows(this);" />
                        </td>
                    </tr>

                    <tr id="td_ModalPopupUnavailableRecurringModalStartHour_Row">
                        <td>
                            Start Time:
                        </td>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="ddlModalPopupUnavailableRecurringModalStartHour" runat="server"></asp:DropDownList> :
                            <asp:DropDownList ID="ddlModalPopupUnavailableRecurringModalStartMinute" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr id="td_ModalPopupUnavailableRecurringModalEndHour_Row">
                        <td>
                            End Time:
                        </td>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="ddlModalPopupUnavailableRecurringModalEndHour" runat="server"></asp:DropDownList> :
                            <asp:DropDownList ID="ddlModalPopupUnavailableRecurringModalEndMinute" runat="server"></asp:DropDownList>
                        </td>
                    </tr>

                    <tr id="tr_ModalPopupUnavailableRecurring_start_date">
                        <td>
                            <b>Start Date: </b>
                        </td>
                        <td></td>
                        <td>
                            <asp:TextBox ID="txtModalPopupUnavailableRecurring_StartDate" runat="server" Width="80"></asp:TextBox>
                            <asp:ImageButton ID="txtModalPopupUnavailableRecurring_StartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" />
                        </td>
                    </tr>
                    <tr id="tr_ModalPopupUnavailableRecurring_end_date">
                        <td>
                            <b>End Date:</b>
                        </td>
                        <td></td>
                        <td>
                            <asp:TextBox ID="txtModalPopupUnavailableRecurring_EndDate" runat="server" Width="80"></asp:TextBox>
                            <asp:ImageButton ID="txtModalPopupUnavailableRecurring_EndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" />
                            &nbsp;&nbsp;<asp:LinkButton ID="btnModalPopupUnavailableRecurring_EndTimeHoverToolTip" runat="server" Text="?" ToolTip="Leave Blank To Have No End Date" OnClientClick="javascript:return false;" />
                        </td>
                    </tr>
                    <tr id="tr_ModalPopupUnavailableRecurring_every_n_weeks">
                        <td>
                            <b>Every:</b>
                        </td>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="ddlUnavailableEveryNWeeks" runat="server"/> Week(s)
                        </td>
                    </tr>

                    <tr style="height:8px">
                        <td colspan="3"/>
                    </tr>

                    <tr id="tr_ModalPopupUnavailableRecurring_series_or_seperate">
                        <td colspan="3">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td><label for="booking_sequence_type_seperate">Create seperate unavailabilities</label></td>
                                    <td style="width:10px"></td>
                                    <td><input type="radio" name="booking_sequence_type" id="booking_sequence_type_seperate" value="seperate" /></td>
                                    <td style="width:15px"></td>
                                    <td><asp:LinkButton ID="btnModalPopupUnavailableRecurring_CreateSeperateUnavailabilityToolTip" runat="server" Text="?" ToolTip="Deleting this will delete only that day's unavailability from the series this creates" OnClientClick="javascript:return false;" /></td>
                                </tr>
                                <tr>
                                    <td><label for="booking_sequence_type_series">Create single series</label></td>
                                    <td style="width:10px"></td>
                                    <td><input type="radio" name="booking_sequence_type" id="booking_sequence_type_series" value="series" /></td>
                                    <td style="width:15px"></td>
                                    <td><asp:LinkButton ID="btnModalPopupUnavailableRecurring_CreateSeriesUnavailabilityToolTip" runat="server" Text="?" ToolTip="Deleting this will delete all unavailabilities this creates" OnClientClick="javascript:return false;" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr height="8">
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


                    <tr height="15">
                        <td colspan="3"/>
                    </tr>
                    <tr>
                        <td colspan="3" align="center" class="tfoot">
                            <asp:Button ID="btnModalPopupUnavailableRecurring_SubmitAdd"    Text="Book"   runat="server" OnClientClick="add_recurring_unavailability_booking();return false;" />
                            <asp:Button ID="btnModalPopupUnavailableRecurring_SubmitUpdate" Text="Update" runat="server" OnClientClick="update_recurring_booking();return false;" />
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnModalPopupUnavailableRecurring_Cancel" Text="Cancel" runat="server" OnClientClick="hide_modal('modalPopupUnavailableRecurring'); return false;" />

                        </td>
                    </tr>
                </table>

            </div>
        </div>
    </div>
</div>




<div style="display: none; z-index: 2000"  id="modalPopupViewPhNbrs">
    <div class="modalBackground"></div>
    <div class="modalContainer">
        <div class="modalFullDays">
            <div class="modalBody">

                <span id="modalPopupViewPhNbrs_booking_tid" class="hiddencol"></span>
                <table  width="100%" height="100%">
                    <tr>
                        <td colspan="2">
                            <strong>
                                <big><span id="modalPopupViewPhNbrs_Name"/></big>
                            </strong>
                        </td>
                    </tr>
                    <tr height="15">
                        <td colspan="2"/>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <span id="modalPopupViewPhNbrs_PhNums"/>
                        </td>
                    </tr>
                    <tr height="15">
                        <td colspan="2"/>
                    </tr>
                    <tr id="conf_buttons">
                        <td align="center">
                            <asp:Button ID="btnModalPopupViewPhNbrs_Confirm" Text="Set Confirmed" runat="server" OnClientClick="modal_ph_num__set_confirmed(); return false;" />
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnModalPopupViewPhNbrs_Cancel" Text="Cancel" runat="server" OnClientClick="hide_modal('modalPopupViewPhNbrs'); return false;" />
                        </td>
                    </tr>
                    <tr id="non_conf_buttons" class="tfoot">
                        <td colspan="2" align="left">
                            <asp:Button ID="Button3" Text="Close" runat="server" OnClientClick="hide_modal('modalPopupViewPhNbrs'); return false;" />
                        </td>
                    </tr>

                </table>

            </div>
        </div>
    </div>
</div>



<div style="display: none; z-index: 2000"  id="modalPopupPageLoadingMessage">
    <div class="modalBackground"></div>
    <div class="modalContainer">
        <div class="modalPageLoadingMessage">
            <div style="margin-left:auto !important; margin-right:auto !important;">

                <br />
                <table style="margin-left:auto !important; margin-right:auto !important; vertical-align:middle !important;">
                    <tr>
                        <td>
                            <img src="/images/loading_circle.gif" alt="loading image...">
                        </td>
                        <td style="min-width:20px;"></td>
                        <td>
                            <span id="modalPopupPageLoadingMessage_Text">Processing. Please wait....</span>
                        </td>
                    </tr>
                </table>

            </div>
        </div>
    </div>
</div>



<div style="display: none; z-index: 2000"  id="modalPopup_Generic" oncontextmenu="return false;">
    <div class="modalBackground_None"></div>
    <div class="modalContainer">
        <div id="spnModalPage_Generic" class="modalPage_Generic">
            <div class="modalBody" id="div_modalPopup_Generic">

                <div style="height:8px;"></div>

                <span id="div_modalPopupGeneric"></span>

                <div style="height:16px;"></div>

                <table style="width:100%" border="1">
                    <tr>
                        <td class="tfoot" style="text-align:center;">
                            <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="hide_modal('modalPopup_Generic'); return false;" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>
    </div>
</div>