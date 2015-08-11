<%@ Page Title="External Staff Detail" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StaffDetailExternalV2.aspx.cs" Inherits="StaffDetailExternalV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function create_username() {

            if (document.getElementById('txtLogin').value.length > 0 ||
                document.getElementById('txtPwd').value.length > 0)
                return; // dont update if already set

            var firstname = document.getElementById('txtFirstname').value.trim();
            var surname = document.getElementById('txtSurname').value.trim();
            document.getElementById('txtLogin').value = firstname.toLowerCase() + surname.toLowerCase();
            document.getElementById('txtPwd').value = firstname.toLowerCase() + surname.toLowerCase();
        }

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.charAt(0).toUpperCase() + txtbox.value.slice(1);
        }

        function title_changed_reset_gender() {
            var selValue = ddlTitle.options[ddlTitle.selectedIndex].value
            if (selValue == 6 || selValue == 265 || selValue == 266)
                setSelectedValue(document.getElementById("ddlGender"), "M");
            if (selValue == 7 || selValue == 26)
                setSelectedValue(document.getElementById("ddlGender"), "F");
        }
        function setSelectedValue(selectObj, valueToSet) {
            for (var i = 0; i < selectObj.options.length; i++) {
                if (selectObj.options[i].value == valueToSet) {
                    selectObj.options[i].selected = true;
                    return;
                }
            }
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

        <div class="clearfix">
            <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server" Text="External Staff Information" /></h3></div>
            <div class="main_content">

                <div class="text-center">
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>

                <table id="maintable" runat="server" style="margin: 0 auto;">
                    <tr style="vertical-align:top;">

                        <td style="width:325px;">

                            <table class="detailtable_staff_external">
                                <tr id="idRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">ID</td>
                                    <td></td>
                                    <td><asp:Label ID="lblId" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Title</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlTitle" runat="server" DataTextField="descr" DataValueField="title_id" onchange='title_changed_reset_gender();' TabIndex="1" ></asp:DropDownList><asp:Label ID="lblTitle" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidateFirstnameRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtFirstname" 
                                            ErrorMessage="Firstname is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateFirstnameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtFirstname"
                                            ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                            ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">First Name</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtFirstname" runat="server" onkeyup="capitalize_first(this);" TabIndex="2"></asp:TextBox><asp:Label ID="lblFirstname" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:RegularExpressionValidator ID="txtValidateMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtMiddlename"
                                            ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                            ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Middle Name</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtMiddlename" runat="server" onkeyup="capitalize_first(this);" TabIndex="3"></asp:TextBox><asp:Label ID="lblMiddlename" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtSurname" 
                                            ErrorMessage="Surname is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateSurnameNameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtSurname"
                                            ValidationExpression="^[0-9a-zA-Z\-\.\s']+$"
                                            ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Surname</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtSurname" runat="server" onblur="create_username();" onkeyup="capitalize_first(this);"  TabIndex="4"/><asp:Label ID="lblSurname" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Gender</td>
                                    <td></td>
                                    <td><asp:DropDownList ID="ddlGender" runat="server"  TabIndex="5"> 
                                            <asp:ListItem Value="M" Text="Male"></asp:ListItem>
                                            <asp:ListItem Value="F" Text="Female"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblGender" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidateLoginRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtLogin" 
                                            ErrorMessage="Login is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateLoginRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtLogin"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Login can only be letters and numbers."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Username</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtLogin" runat="server" TabIndex="9"></asp:TextBox><asp:Label ID="lblLogin" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidatePwdRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtPwd" 
                                            ErrorMessage="Password is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidatePwdRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtPwd"
                                            ValidationExpression="^[0-9a-zA-Z\-_]+$"
                                            ErrorMessage="Password can only be letters, numbers, and underscore."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Password</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtPwd" runat="server" TextMode="Password" TabIndex="10"></asp:TextBox><asp:Label ID="lblPwd" runat="server" Font-Bold="True" Text="●●●●●●●●●"/></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap"></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td><asp:CustomValidator ID="ddlStartDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlStartDate_Day"
                                            OnServerValidate="StartDateAllOrNoneCheck"
                                            ErrorMessage="Start Date must have each of day/month/year selected, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                    <td class="nowrap">Start Date</td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlStartDate_Day" runat="server" TabIndex="13"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlStartDate_Month" runat="server" TabIndex="14"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlStartDate_Year" runat="server" TabIndex="15"></asp:DropDownList>
                                        <asp:Label ID="lblStartDate" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:CustomValidator ID="ddlEndDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlEndDate_Day"
                                            OnServerValidate="EndDateAllOrNoneCheck"
                                            ErrorMessage="End Date must have each of day/month/year selected, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                    <td class="nowrap">End Date</td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlEndDate_Day" runat="server" TabIndex="16"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlEndDate_Month" runat="server" TabIndex="17"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlEndDate_Year" runat="server" TabIndex="18"></asp:DropDownList>
                                        <asp:Label ID="lblEndDate" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblAddedByText" runat="server">Added By</asp:Label></td>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblAddedBy" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblStaffDateAddedText" runat="server">Date Added</asp:Label></td>
                                    <td></td>
                                    <td><asp:Label ID="lblStaffDateAdded" runat="server"></asp:Label></td>
                                </tr> 
                                <tr>
                                    <td></td>
                                    <td class="nowrap">Status</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlStatus" runat="server">
                                            <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                                            <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblIsFired" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>

                                <tr>
                                    <td colspan="4">
                                        <div style="height:12px;"></div>
                                        <center>
                                        Comments<br />
                                        <asp:TextBox ID="txtComments" runat="server" TextMode="multiline" rows="2" Columns="38" TabIndex="32"  />
                                        </center>

                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <div style="height:15px;"></div>
                                        <center>
                                            <asp:Button ID="btnSubmit" runat="server" Text="Button" TabIndex="33" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" TabIndex="34" onclick="btnCancel_Click" Visible="False" />
                                        </center>
                                    </td>
                                </tr>

                            </table>

                        </td>
                        <td id="existingStaffInfoSpace" runat="server" style="width:5%;"><div class="staff_external_detail_vertical_divider"></div></td>
                        <td id="existingStaffInfo" runat="server" style="width:350px;">

                            <table class="block_center" style="margin-left:5%;">
                                <tr style="vertical-align:top;">
                                    <td class="nowrap"><b>Clinics Registered To:</b>  &nbsp;&nbsp;&nbsp;&nbsp;(<asp:HyperLink ID="lnkThisStaff" runat="server" NavigateUrl="~/StaffListV2.aspx?id=">Edit</asp:HyperLink>)</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Panel ID="pnlOrgsList" runat="server" ScrollBars="Auto" style="max-height:140px;" >
                                            <asp:BulletedList ID="lstClinics" runat="server"></asp:BulletedList>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="nowrap">
                                        <div style="height:28px;"></div>
                                        <b>Sites Registered To:</b>
                                    </td>
                                </tr>
                                <tr style="height:3px">
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>

                                        <asp:Repeater id="lstSites" runat="server">
                                            <HeaderTemplate>
                                                <table>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="nowrap"><asp:Label ID="lblSiteName" runat="server" Text='<%# Eval("name") %>'></asp:Label></td>
                                                    <td style="min-width:12px"></td>
                                                    <td><asp:ImageButton ID="imgStatus" runat="server" ImageUrl='<%# Convert.ToBoolean(Eval("has_access")) ? "~/images/tick-12.png" : "~/images/Delete-icon-12.png" %>'></asp:ImageButton></td>
                                                    <td style="min-width:12px"></td>
                                                    <td><asp:LinkButton ID="btnToggleSiteRestriction" runat="server" OnCommand="btnToggleSiteRestriction_Click" CommandName='<%#  Convert.ToBoolean(Eval("has_access")) ? "TurnOff" : "TurnOn" %>' CommandArgument='<%# Eval("site_id") %>' Text="Toggle"></asp:LinkButton></td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>

                                        <asp:Label ID="lblNoSitesExist" runat="server" Text="No sites exist yet. Go to the Sites table to create a site."></asp:Label>
                                        
                                    </td>
                                </tr>




                                <tr style="vertical-align:top;">
                                    <td class="block_center">

                                        <div style="height:28px;"></div>
                                        <asp:Button ID="btnUpdateBookingList" runat="server" CssClass="hiddencol" onclick="btnUpdateBookingList_Click" />
                                        <asp:Panel ID="pnlDefaultButton_SearchBookingList" runat="server" DefaultButton="btnSearchBookingList">
                                            <table>
                                                <tr>
                                                    <td><b>Bookings</b>&nbsp;<asp:Label ID="lblBookingListCount" runat="server"></asp:Label></td>

                                                    <td style="min-width:25px"></td>

                                                    <td class="nowrap">From: </td>
                                                    <td class="nowrap"><asp:TextBox ID="txtStartDate" runat="server" Columns="10"/></td>
                                                    <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                    <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>

                                                    <td style="min-width:10px"></td>

                                                    <td class="nowrap">To: </td>
                                                    <td class="nowrap"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                                                    <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                    <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>

                                                    <td style="min-width:25px"></td>
                        
                                                    <td><asp:Button ID="btnSearchBookingList" runat="server" Text="Refresh" OnClick="btnSearchBookingList_Click" /></td>
                                                    <td style="min-width:8px"></td>
                                                    <td><asp:Button ID="btnPrintBookingList" runat="server" Text="Print" OnClick="btnPrintBookingList_Click" /></td>

                                                </tr>
                                            </table>
                                        </asp:Panel>

                                        <span style="height:10px;display:block"></span>
                                        <asp:Label ID="lblBookingsList_NoRowsMessage" runat="server" Text="No bookings added by this user."></asp:Label>
                                        <asp:Panel ID="pnlBookingsList" runat="server" ScrollBars="Auto" style="max-height:470px; overflow: auto; display:inline-block;">
                                            <table id="tblBookingsList" border="1" style="border-collapse:collapse;" class=" text_center padded-table-2px" >
                                            <asp:Repeater id="lstBookingList" runat="server">
                                                <HeaderTemplate>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="nowrap" style="text-align:left">
                                                            <table>
                                                                <tr>
                                                                    <td>BK:</td>
                                                                    <td><asp:Label ID="lblBookingDate"      Text='<%# Eval("booking_date_start", "{0:dd-MM-yyyy}") + "&nbsp;&nbsp;" + Eval("booking_date_start", "{0:HH:mm}") %>' runat="server" ForeColor='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? System.Drawing.ColorTranslator.FromHtml("#777777") : System.Drawing.ColorTranslator.FromHtml("#333333") %>' /></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Added:</td>
                                                                    <td><asp:Label ID="lblBookingDateAdded" Text='<%# Eval("booking_date_created", "{0:dd-MM-yyyy}") + "&nbsp;&nbsp;" + Eval("booking_date_created", "{0:HH:mm}") %>' runat="server" ForeColor='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? System.Drawing.ColorTranslator.FromHtml("#777777") : System.Drawing.ColorTranslator.FromHtml("#333333") %>' /></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td class="nowrap">
                                                            <asp:Label ID="lblOfferingDescr"    Text='<%#  Eval("offering_name") %>' runat="server" ForeColor='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? System.Drawing.ColorTranslator.FromHtml("#777777") : System.Drawing.ColorTranslator.FromHtml("#333333") %>' />
                                                            <br />
                                                            <asp:Label ID="lblProviderAndOrg"   Text='<%#  Eval("person_provider_firstname") + " " + Eval("person_provider_surname") + " @ " + Eval("organisation_name") %>' runat="server" ForeColor='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? System.Drawing.ColorTranslator.FromHtml("#777777") : System.Drawing.ColorTranslator.FromHtml("#a52a2a") %>' />
                                                        </td>
                                                        <td class="nowrap">
                                                            <asp:Label ID="lblBookingStatus" Text='<%# Eval("booking_status_descr").ToString() + (Eval("inv_type_text").ToString().Length == 0 ? "" : "<br /><font color=\"#A52A2A\">" + Eval("inv_type_text") + "</font>" ) %>' ForeColor='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? System.Drawing.ColorTranslator.FromHtml("#777777") : System.Drawing.ColorTranslator.FromHtml("#333333") %>' runat="server"/><a id="A1" runat="server" href="javascript:void(0)"  onclick="javascript:return false;" title='<%#  Eval("added_by_deleted_by_row") %>' style="text-decoration: none"><b>&nbsp;*&nbsp;</b></a>
                                                        </td>
                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_change_history_row").ToString()=="1"?true:false %>'><asp:Label ID="lblBookingHistory" Text='<%# Eval("booking_change_history_link").ToString() %>' runat="server"  Visible='<%# (bool)Eval("hide_change_history_link") ? false : true %>' ></asp:Label></td>
                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_notes_row").ToString()=="1"?true:false %>'><asp:Label ID="lblNotes" runat="server" Text='<%#  Eval("notes_text") %>' Visible='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? false : true %>' /></td>
                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_printletter_row").ToString()=="1"?true:false %>'><asp:HyperLink ID="lnkPrintLetter" runat="server" NavigateUrl='<%#  String.Format("~/Letters_PrintV2.aspx?booking={0}",Eval("booking_booking_id")) %>' ImageUrl="~/images/printer_green-24.png" AlternateText="Letters" ToolTip="Letters" Visible='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? false : true %>' /></td>
                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_invoice_row").ToString()=="1"?true:false %>'><asp:Label ID="lblViewInvoice" runat="server" Text='<%#  Eval("invoice_text") %>' ToolTip= "View Invoice" Visible='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value) ? false : true %>'></asp:Label></td>
                                                        <td class="nowrap" runat="server" Visible='<%# Eval("show_bookingsheet_row").ToString()=="1"?true:false %>'><asp:HyperLink ID="lnkBookingSheetForPatient" runat="server" NavigateUrl='<%#  Eval("booking_url") %>' ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Booking Sheet" ToolTip="Booking Sheet" Visible='<%# (Eval("booking_deleted_by") != DBNull.Value || Eval("booking_date_deleted") != DBNull.Value || (bool)Eval("hide_booking_link")) ? false : true %>' /></td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                            </table>
                                        </asp:Panel>

                                    </td>
                                </tr>







                            </table>

                        </td>

                    </tr>
                </table>

                <div id="autodivheight" class="divautoheight" style="height:500px;">
                </div>
  
            </div>
        </div>

</asp:Content>



