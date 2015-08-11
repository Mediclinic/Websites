<%@ Page Title="Print A Letter" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Letters_PrintV2.aspx.cs" Inherits="Letters_PrintV2" ValidateRequest="false" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function get_patient() {
            var org_id = document.getElementById('txtUpdateOrganisationID').value;
            var retVal = (org_id == '') ?
                    window.showModalDialog("PatientListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:700px;resizable:yes;center:yes;") :
                    window.showModalDialog("PatientListPopupV2.aspx?org=" + org_id, 'Show Popup Window', "dialogHeight:700px;dialogWidth:700px;resizable:yes;center:yes;");

            if (typeof retVal === "undefined")
                return;

            var index = retVal.indexOf(":");
            document.getElementById('txtUpdatePatientID').value = retVal.substring(0, index);
            document.getElementById('txtUpdatePatientName').value = retVal.substring(index + 1);

            set_textbox_style('txtUpdatePatientName', false);
            document.getElementById('btnPatientUpdated').click();
        }

        function clear_patient() {
            document.getElementById('txtUpdatePatientID').value = '';
            document.getElementById('txtUpdatePatientName').value = '';

            set_textbox_style('txtUpdatePatientName', true);
            document.getElementById('btnPatientUpdated').click();
        }

        function get_organisation() {
            var patient_id = document.getElementById('txtUpdatePatientID').value;
            var retVal = (patient_id == '') ?
                    window.showModalDialog("OrganisationListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:750px;resizable:yes;center:yes;") :
                    window.showModalDialog("OrganisationListPopupV2.aspx?patient=" + patient_id, 'Show Popup Window', "dialogHeight:800px;dialogWidth:750px;resizable:yes;center:yes;");

            if (typeof retVal === "undefined")
                return;

            var index = retVal.indexOf(":");
            document.getElementById('txtUpdateOrganisationID').value = retVal.substring(0, index);
            document.getElementById('txtUpdateOrganisationName').value = retVal.substring(index + 1);

            set_textbox_style('txtUpdateOrganisationName', false);
            document.getElementById('btnOrganisationUpdated').click();
        }

        function clear_organisation() {
            document.getElementById('txtUpdateOrganisationID').value = '';
            document.getElementById('txtUpdateOrganisationName').value = '';

            set_textbox_style('txtUpdateOrganisationName', true);
            document.getElementById('btnOrganisationUpdated').click();
        }


        function set_textbox_style(textboxID, is_empty) {
            if (!is_empty)
                document.getElementById(textboxID).style.cssText = "border:none;background-color:transparent;color:black;";
            else
                document.getElementById(textboxID).style.cssText = "width:175px;";
        }

        function clear_error_message() {
            document.getElementById('lblErrorMessage').value = '';
        }



        function check_org_set() {
            if (document.getElementById('txtUpdateOrganisationID').value == '') {
                var _continue = confirm('Organisation has not been set. \r\n\r\nAre you sure you would like to continue?\r\n\r\nPress \'Ok\' to go ahead and use the head offices name and address in place of an organisation\'s. \r\nPress \'Cancel\' to go back and select an organisation.');
                return _continue;
            }
        }

        function open_new_window(URL) {
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,height=" + screen.height + ',width=' + screen.width);
            NewWindow.location = URL;
        }

        function clear_file_upload(id) {

            // get the file upload element
            fileField = document.getElementById(id);

            // get the file upload parent element
            parentNod = fileField.parentNode;

            // create new element
            tmpForm = document.createElement("form");
            parentNod.replaceChild(tmpForm, fileField);
            tmpForm.appendChild(fileField);
            tmpForm.reset();
            parentNod.replaceChild(fileField, tmpForm);
        }


        // -------------------------------

        // expire the cookie so does not exist on page load
        expireCookie("fileDownloaded");

        // every X seconds, check if file downloaded has put a cookie saying file download complete - if so de-select the listbox
        // so if they print a letter then edit, then attach to email, it doesn't regenerate the letter for the email along with the edited one they attached
        window.setInterval(function () {
            var token = getCookie("fileDownloaded");
            if ((token == "true")) {
                document.getElementById("lstLetters").selectedIndex = -1;  // deselect file from listbox
                expireCookie("fileDownloaded");
            }
        }, 1500);

        function getCookie(name) {
            var parts = document.cookie.split(name + "=");
            if (parts.length == 2) return parts.pop().split(";").shift();
        }

        function expireCookie(cName) {
            document.cookie =
              encodeURIComponent(cName) +
              "=deleted; expires=" +
              new Date(0).toUTCString();
        }

        // -------------------------------



    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Print A Letter</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>

                <table id="main_table" runat="server">
                    <tr style="vertical-align:top;">

                        <td id="td_booking" runat="server">
                
                            <table id ="tbl_booking_details" runat="server">
                                <tr id="booking_title" runat="server" valign="top">
                                    <td colspan="2" style="vertical-align:top; text-align:left;"><b>Booking Details</b></td>
                                    <td colspan="3" style="vertical-align:top; text-align:left;">
                        
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:LinkButton ID="lnkBookingSheetForPatient" runat="server"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkBookingListForPatient" runat="server"></asp:LinkButton>
                                                </td>
                                            </tr>
                                        </table>

                                    </td>
                                </tr>
                                <tr style="height:10px;">
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr id="booking_apptmt_time" runat="server">
                                    <td>Appointment Time</td>
                                    <td style="width:15px"></td>
                                    <td><asp:Label ID="lblBooking_Time" runat="server"></asp:Label></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr id="booking_provider" runat="server">
                                    <td>Provider</td>
                                    <td></td>
                                    <td><asp:Label ID="lblBooking_Provider" runat="server"></asp:Label></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr id="booking_offering" runat="server">
                                    <td>Service</td>
                                    <td></td>
                                    <td><asp:Label ID="lblBooking_Offering" runat="server"></asp:Label></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr id="booking_status" runat="server">
                                    <td>Booking Status</td>
                                    <td></td>
                                    <td><asp:Label ID="lblBooking_BookingStatus" runat="server"></asp:Label></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr id="booking_notes" runat="server">
                                    <td>Booking Notes (Add/Edit)</td>
                                    <td></td>
                                    <td><asp:Label ID="lblBooking_Notes" runat="server"></asp:Label></td>
                                    <td></td>
                                    <td></td>
                                </tr>

                            </table>

                            <br /><br />

                            <b>Check booking notes to include for printing</b>

                            <br />
                            <br />

                            <asp:Repeater id="lstNotes" runat="server" onitemdatabound="lstNotes_ItemDataBound">
                                <HeaderTemplate>
                                    <table border="1" style="width:100%; border-style: solid;border-collapse: collapse;" >
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><asp:CheckBox ID="chkUseNote" runat="server" /><asp:Label ID="lblNoteID" runat="server" CssClass="hiddencol" Text='<%# Bind("note_id") %>'></asp:Label><asp:Label ID="lblOriginalText" runat="server" CssClass="hiddencol" Text='<%# Bind("text") %>'></asp:Label></td>
                                        <td style="width:500px"><asp:Label ID="lblNoteText" Text='<%# ((string)Eval("text")).Replace("\n", "<br/>") %>' runat="server"></asp:Label></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <tr id="myNoDataRow" runat="server">
                                        <td>Booking has no notes</td>
                                    </tr>
                                    </table>           
                                </FooterTemplate>
                            </asp:Repeater>
            
                        </td>

                        <td style="min-width:25px" id="td_booking_space" runat="server"></td>

                        <td id="td_letter" runat="server">

                            <table>

                                <tr>
                                    <td><asp:Label ID="lblOrganisationType" runat="server">Organisation</asp:Label></td>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtUpdateOrganisationID" runat="server" CssClass="hiddencol" />
                                        <asp:TextBox ID="txtUpdateOrganisationName" runat="server" Enabled="False" />
                                        <asp:Label ID="lblUpdateOrganisationName" runat="server" Visible="false" ></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnOrganisationListPopup" runat="server" Text="Get Organisation" OnClientClick="javascript:get_organisation(); return false;" Width="100%" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnClearOrganisation"     runat="server" Text="Clear"            OnClientClick="javascript:clear_organisation();return false;" Width="100%" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnOrganisationUpdated"   runat="server" CssClass="hiddencol" Text="" onclick="btnOrganisationUpdated_Click" />

                                        <asp:Button ID="btnUpdateOrganisation"    runat="server" CssClass="hiddencol" Text="Get ID in Code Behind" onclick="btnUpdateOrganisation_Click" />
                                        <asp:Label  ID="lblOrganisationName"      runat="server" CssClass="hiddencol" Text="--" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Patient</td>
                                    <td style="width:6px"></td>
                                    <td>
                                        <asp:TextBox ID="txtUpdatePatientID" runat="server" CssClass="hiddencol" />
                                        <asp:TextBox ID="txtUpdatePatientName" runat="server" Enabled="False" />
                                        <asp:Label ID="lblUpdatePatientName" runat="server" Visible="false" ></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnPatientListPopup" runat="server" Text="Get Patient" OnClientClick="javascript:get_patient(); return false;" Width="100%" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnClearPatient"     runat="server" Text="Clear"       OnClientClick="javascript:clear_patient();return false;" Width="100%" />
                                    </td>
                                    <td style="white-space:nowrap;">
                                        <asp:Button ID="btnPatientUpdated"   runat="server" CssClass="hiddencol" Text="" onclick="btnPatientUpdated_Click" />

                                        <asp:Button ID="btnUpdatePatient"    runat="server" CssClass="hiddencol" Text="Get ID in Code Behind" onclick="btnUpdatePatient_Click" />
                                        <asp:Label  ID="lblPatientName"      runat="server" CssClass="hiddencol" Text="--" />
                                    </td>
                                </tr>
                                <tr style="height:15px">
                                    <td colspan="6"></td>
                                </tr>
                                <tr>
                                    <td colspan="2"></td>
                                    <td colspan="4">
                                        <asp:Label ID="lblDefaultOrOrgSpecificDocs" runat="server"></asp:Label>
                                        <asp:Label ID="lblSpaceBeforeUseDefaultDocsCheckbox" runat="server">&nbsp;&nbsp;&nbsp;</asp:Label>
                                        <asp:CheckBox ID="chkUseDefaultDocs" runat="server" Text="Use Default Docs Instead Of Clinic Specific Docs" AutoPostBack="true" OnCheckedChanged="chkUseDefaultDocs_CheckedChanged" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Letter</td>
                                    <td></td>
                                    <td colspan="3"><asp:ListBox ID="lstLetters" runat="server" rows="34" SelectionMode="Single" style="min-width:325px;"></asp:ListBox></td>
                                    <td></td>
                                </tr>

                            </table>
            
                        </td>

                        <td style="min-width:25px"></td>

                        <td>

                            <table>

                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center">

                                        <asp:Button ID="btnPrint" runat="server" Text="&nbsp;&nbsp;&nbsp;Print&nbsp;&nbsp;&nbsp;" OnClientClick="javascript:clear_error_msg();return check_org_set();" OnClick="btnPrint_Click" /> 
                                        &nbsp;&nbsp;
                                        <asp:Button ID="SendEmail" runat="server" Text="Send Email" OnClick="SendEmail_Click" />
                                    </td>
                                </tr>
                                <tr style="height:15px">
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>



                                <tr>
                                    <td>

                                        <table class="padded-table-1px">
                                            <tr id="toRow" runat="server">
                                                <td>
                                                    To:
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="txtEmailTo" Columns="53" runat="server" BackColor="LightGoldenrodYellow"></asp:TextBox>

                                                    <asp:Button ID="btnPTEmail" runat="server" OnClick="btnPTEmail_Click" Text="PT" />
                                                    <asp:Button ID="btnGPEmail" runat="server" OnClick="btnGPEmail_Click" Text="GP" />

                                                </td>
                                            </tr>
                                            <tr id="subjectRow" runat="server">
                                                <td>
                                                    Subject:
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="txtSubject" Columns="67" runat="server" BackColor="LightGoldenrodYellow"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3">
                                                    <div style="line-height:6px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr id="messageRow" runat="server">
                                                <td colspan="3" style="text-align:center">
                                                    <FTB:FreeTextBox id="FreeTextBox1" runat="Server" Text="" Width="525px" Height="275px" />

		                                            <div>
			                                            <asp:Literal id="Output" runat="server" />
		                                            </div>

                                                    <div style="line-height:10px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr id="attachmentRow1" runat="server">
				                                <td style="text-align:right; vertical-align:middle;white-space:nowrap;">Attachment :</td>
				                                <td style="white-space:nowrap;">
                                                    <input id="inpAttachment1" type="file" size="53" name="filMyFile" runat="server" />
                                                </td>
                                                <td style="text-align:right;white-space:nowrap;">
                                                    <input type="button" value="Remove" onclick="javascript: clear_file_upload('inpAttachment1'); return false;" />
				                                </td>
			                                </tr>
			                                <tr id="attachmentRow2" runat="server">
				                                <td style="text-align:right; vertical-align:middle;white-space:nowrap;">Attachment :</td>
				                                <td style="white-space:nowrap;">
                                                    <input id="inpAttachment2" type="file" size="53" name="filMyFile" runat="server" />
                                                </td>
                                                <td style="text-align:right;white-space:nowrap;">
                                                    <input type="button" value="Remove" onclick="javasript: clear_file_upload('inpAttachment2'); return false;" />
				                                </td>
			                                </tr>
			                                <tr id="attachmentRow3" runat="server">
				                                <td style="text-align:right; vertical-align:middle;white-space:nowrap;">Attachment :</td>
				                                <td style="white-space:nowrap;">
                                                    <input id="inpAttachment3" type="file" size="53" name="filMyFile" runat="server" />
                                                </td>
                                                <td style="text-align:right;white-space:nowrap;">
                                                    <input type="button" value="Remove" onclick="javascript: clear_file_upload('inpAttachment3'); return false;" />
				                                </td>
			                                </tr>
                                        </table>





                                    </td>
                                </tr>
                            </table>

                        </td>

                    </tr>
                </table>

                <div id="select_booking_patient_table" runat="server" visible="false">

                    <h5>
                        This is a group booking.<br />
                        Please select a patient from the group booking.<br />
                    </h5>

                    <div style="height:6px;"></div>

                    <asp:Repeater id="lstBookingPatients" runat="server" onitemdatabound="lstBookingPatients_ItemDataBound">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <a href='Letters_PrintV2.aspx?bookingpatient=<%# Eval("bp_booking_patient_id") %>'><%# Eval("patient_person_firstname") + " " + Eval("patient_person_surname") %></a>
                            <br />
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Label ID="lblEmptyData" Text="No patients have been added to this group booking." ForeColor="Red" runat="server" Visible="false"></asp:Label>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>


        </div>
    </div>


</asp:Content>



