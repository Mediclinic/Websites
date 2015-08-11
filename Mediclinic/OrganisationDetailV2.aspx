<%@ Page Title="Organisation Details" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="OrganisationDetailV2.aspx.cs" Inherits="StaffDetailV2" %>
<%@ Register TagPrefix="UC" TagName="AddressControl" Src="~/Controls/AddressControl.ascx" %>
<%@ Register TagPrefix="UC" TagName="AddressAusControl" Src="~/Controls/AddressAusControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="Scripts/check_future_bookings_org.js" type="text/javascript"></script>
    <script src="Scripts/check_future_bookings.js" type="text/javascript"></script>
        <script type="text/javascript">

            function capitalize_first(txtbox) {
                txtbox.value = txtbox.value.charAt(0).toUpperCase() + txtbox.value.slice(1);
            }

            function org_type_changed_reset_use_parent_prices() {
                var selValue = ddlType.options[ddlType.selectedIndex].value
                if (selValue == 367 || selValue == 372)
                    setSelectedValue(document.getElementById("ddlUseParentOffernigPrices"), "False");
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
            <div class="page_title"><span id="lblHeading" runat="server">Edit Organisation</span></div>
            <div class="main_content">

                <div class="text-center">
                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                </div>

                <table style="margin: 0 auto;" id="maintable" runat="server">
                    <tr style="vertical-align:top;">
                        <td>

                            <table id="tbl_detailtable_organisation" runat="server" class="detailtable_organisation">
                                <tr id="idRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">ID</td>
                                    <td></td>
                                    <td><asp:Label ID="lblId" runat="server"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td><asp:RequiredFieldValidator ID="txtValidateNameRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtName" 
                                            ErrorMessage="Name is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="txtValidateNameRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtName"
                                            ValidationExpression="^[0-9a-zA-Z_\-\.\s',\(\)\[\]]+$"
                                            ErrorMessage="Name can only be alphanumeric, hyphens, underscore, comas, or fullstops."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">Name</td>
                                    <td></td>
                                    <td class="nowrap" style="width:320px"><asp:TextBox ID="txtName" runat="server" onkeyup="capitalize_first(this);" Width="75%" MaxLength="100"></asp:TextBox><asp:Label ID="lblName" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr id="parentRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">Parent</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlParent" runat="server" DataTextField="name" DataValueField="organisation_id"></asp:DropDownList><asp:Label ID="lblParent" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr id="useParentOfferingPricesRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">Use Parent Offernig Prices</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlUseParentOffernigPrices" runat="server"> 
                                            <asp:ListItem Value="True" Text="Yes"></asp:ListItem>
                                            <asp:ListItem Value="False" Text="No"></asp:ListItem>
                                        </asp:DropDownList> 
                                        <asp:Label ID="lblUseParentOffernigPrices" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>
                                <%--
                                <tr id="debtorRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">Debtor</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlIsDebtor" runat="server"> 
                                            <asp:ListItem Value="True" Text="Yes"></asp:ListItem>
                                            <asp:ListItem Value="False" Text="No"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblIsDebtor" runat="server" Font-Bold="True"/>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr id="creditorRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">Creditor</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlIsCreditor" runat="server"> 
                                            <asp:ListItem Value="True" Text="Yes"></asp:ListItem>
                                            <asp:ListItem Value="False" Text="No"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblIsCreditor" runat="server" Font-Bold="True"/>
                                    </td>
                                    <td></td>
                                </tr>
                                --%>
                                <tr id="typeRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">Type</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlType" runat="server" DataTextField="descr" DataValueField="organisation_type_id" onchange='org_type_changed_reset_use_parent_prices();'></asp:DropDownList><asp:Label ID="lblType" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr id="customerTypeRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">Customer Type</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlCustType" runat="server" DataTextField="descr" DataValueField="organisation_customer_type_id"></asp:DropDownList><asp:Label ID="lblCustType" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr id="abnRow" runat="server">
                                    <td><asp:RegularExpressionValidator ID="txtValidateABNRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtABN"
                                            ValidationExpression="^[0-9\-]+$"
                                            ErrorMessage="ABN can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">ABN</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox Width="75%" ID="txtABN" runat="server" Text='<%# Bind("abn") %>'></asp:TextBox><asp:Label ID="lblABN" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr id="acnRow" runat="server">
                                    <td><asp:RegularExpressionValidator ID="txtValidateACNRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtACN"
                                            ValidationExpression="^[0-9\-]+$"
                                            ErrorMessage="ACN can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">ACN</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox Width="75%" ID="txtACN" runat="server" ></asp:TextBox><asp:Label ID="lblACN" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr id="bpayRow" runat="server">
                                    <td><asp:RegularExpressionValidator ID="txtValidateBPayRegex" runat="server" CssClass="failureNotification" 
                                            ControlToValidate="txtBPayAccount"
                                            ValidationExpression="^[0-9\-]+$"
                                            ErrorMessage="BPay can only be numbers and dashes."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td class="nowrap">BPay</td>
                                    <td></td>
                                    <td class="nowrap"><asp:TextBox Width="75%" ID="txtBPayAccount" runat="server" Text='<%# Bind("bank_bpay") %>'></asp:TextBox><asp:Label ID="lblBPayAccount" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr id="serviceCycleRow" runat="server" visible="false">
                                    <td></td>
                                    <td class="nowrap">Service Cycle (Weeks)</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlServiceCycle" runat="server"></asp:DropDownList><asp:Label ID="lblServiceCycle" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr id="numFreeServicesRow" runat="server" visible="false">
                                    <td></td>
                                    <td class="nowrap">Number of Free Services</td>
                                    <td></td>
                                    <td class="nowrap"><asp:DropDownList ID="ddlFreeServices" runat="server"></asp:DropDownList><asp:Label ID="lblFreeServices" runat="server" Font-Bold="True"/></td>
                                </tr>
                                <tr id="dateAddedRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">Date Added</td>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblDateAdded" runat="server"></asp:Label></td>
                                </tr>
                                <tr id="startDateRow" runat="server">
                                    <td><asp:CustomValidator ID="ddlStartDateValidateAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlEndDate_Day"
                                            OnServerValidate="StartDateAllOrNoneCheck"
                                            ErrorMessage="Start Date must have each of day/month/year selected, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummary">*</asp:CustomValidator>
                                    </td>
                                    <td class="nowrap">Start Date</td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlStartDate_Day" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlStartDate_Month" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlStartDate_Year" runat="server"></asp:DropDownList>
                                        <asp:Label ID="lblStartDate" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>
                                <tr id="endDateRow" runat="server">
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
                                        <asp:DropDownList ID="ddlEndDate_Day" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlEndDate_Month" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlEndDate_Year" runat="server"></asp:DropDownList>
                                        <asp:Label ID="lblEndDate" runat="server" Font-Bold="True"/>
                                    </td>
                                </tr>
                                <tr id="lastBatchRunRow" runat="server">
                                    <td></td>
                                    <td class="nowrap">Last Batch Run</td>
                                    <td></td>
                                    <td class="nowrap"><asp:Label ID="lblLastBatchRun" runat="server"/></td>
                                </tr>


                                <tr id="commentsRow" runat="server">
                                    <td colspan="4">
                                        <div style="height:12px;"></div>
                                        <center>
                                        Comments<br />
                                        <asp:TextBox ID="txtComments" runat="server" TextMode="multiline" rows="2" Columns="50" TabIndex="32"  />
                                        </center>

                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="9">
                                        <div style="height:15px;"></div>
                                        <center>
                                            <asp:Button ID="btnSubmit" runat="server" Text="Button" TabIndex="33" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="ValidationSummary" />
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" TabIndex="34" onclick="btnCancel_Click" Visible="False" />
                                        </center>
                                    </td>
                                </tr>

                            </table>

                        </td>
                        <td id="existingOrgInfoSpace" runat="server"><div class="org_detail_vertical_divider"></div></td>
                        <td id="existingOrgInfoSpaceShort" runat="server"><div class="org_detail_vertical_divider_short"></div></td>
                        <td>

                            <table id="workingDaysRow" runat="server">
                                <tr>
                                    <td colspan="2"><b>Days Open</b><br /><br /></td>
                                    <td style="min-width:20px"></td>
                                    <td ><b>Opening Hours</b><br /><br /></td>
                                    <td style="min-width:20px"></td>
                                    <td><b>Lunch Hours</b><br /><br /></td>
                                </tr>
                                <tr>
                                    <td>Sunday&nbsp;</td>
                                    <td><input id="chkIncSunday" onclick="validate_uncheck(this);" type="checkbox" value="Accept Form" name="chkIncSunday" runat="server" /><asp:CheckBox ID="chkIncSun" runat="server" /></td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlSunStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlSunStart_Minute" runat="server"  onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlSunEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlSunEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblSunStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSunStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSunEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSunEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                    <td></td>
                                    <td class="nowrap">
                                        <asp:DropDownList ID="ddlSunLunchStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlSunLunchStart_Minute" runat="server"  onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlSunLunchEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlSunLunchEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblSunLunchStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSunLunchStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSunLunchEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSunLunchEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Monday&nbsp;</td>
                                    <td><input id="chkIncMonday" onclick="validate_uncheck(this);" type="checkbox" value="Accept Form" name="chkIncMonday" runat="server" /><asp:CheckBox ID="chkIncMon" runat="server" /></td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlMonStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlMonStart_Minute" runat="server" onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlMonEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlMonEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblMonStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblMonStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblMonEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblMonEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlMonLunchStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlMonLunchStart_Minute" runat="server"  onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlMonLunchEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlMonLunchEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblMonLunchStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblMonLunchStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblMonLunchEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblMonLunchEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Tuesday&nbsp;</td>
                                    <td><input id="chkIncTuesday" onclick="validate_uncheck(this);" type="checkbox" value="Accept Form" name="chkIncTuesday" runat="server" /><asp:CheckBox ID="chkIncTue" runat="server" /></td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlTueStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlTueStart_Minute" runat="server" onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlTueEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlTueEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblTueStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblTueStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblTueEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblTueEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlTueLunchStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlTueLunchStart_Minute" runat="server"  onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlTueLunchEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlTueLunchEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblTueLunchStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblTueLunchStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblTueLunchEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblTueLunchEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Wednesday&nbsp;</td>
                                    <td><input id="chkIncWednesday" onclick="validate_uncheck(this);" type="checkbox" value="Accept Form" name="chkIncWednesday" runat="server" /><asp:CheckBox ID="chkIncWed" runat="server" /></td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlWedStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlWedStart_Minute" runat="server" onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlWedEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlWedEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblWedStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblWedStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblWedEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblWedEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>                                
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlWedLunchStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlWedLunchStart_Minute" runat="server"  onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlWedLunchEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlWedLunchEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblWedLunchStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblWedLunchStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblWedLunchEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblWedLunchEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Thursday&nbsp;</td>
                                    <td><input id="chkIncThursday" onclick="validate_uncheck(this);" type="checkbox" value="Accept Form" name="chkIncThursday" runat="server" /><asp:CheckBox ID="chkIncThu" runat="server" /></td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlThuStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlThuStart_Minute" runat="server" onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlThuEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlThuEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblThuStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblThuStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblThuEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblThuEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlThuLunchStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlThuLunchStart_Minute" runat="server"  onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlThuLunchEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlThuLunchEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblThuLunchStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblThuLunchStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblThuLunchEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblThuLunchEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Friday&nbsp;</td>
                                    <td><input id="chkIncFriday" onclick="validate_uncheck(this);" type="checkbox" value="Accept Form" name="chkIncFriday" runat="server" /><asp:CheckBox ID="chkIncFri" runat="server" /></td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlFriStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlFriStart_Minute" runat="server" onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlFriEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlFriEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblFriStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblFriStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblFriEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblFriEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlFriLunchStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlFriLunchStart_Minute" runat="server"  onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlFriLunchEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlFriLunchEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblFriLunchStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblFriLunchStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblFriLunchEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblFriLunchEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Saturday&nbsp;</td>
                                    <td><input id="chkIncSaturday" onclick="validate_uncheck(this);" type="checkbox" value="Accept Form" name="chkIncSaturday" runat="server" /><asp:CheckBox ID="chkIncSat" runat="server" /></td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlSatStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlSatStart_Minute" runat="server" onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlSatEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlSatEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblSatStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSatStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSatEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSatEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlSatLunchStart_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlSatLunchStart_Minute" runat="server"  onChange="validate_change_time(this)"/>
                                        -
                                        <asp:DropDownList ID="ddlSatLunchEnd_Hour" runat="server" onChange="validate_change_time(this)"/> <b>:</b>
                                        <asp:DropDownList ID="ddlSatLunchEnd_Minute" runat="server" onChange="validate_change_time(this)"/> 
                                        <asp:Label ID="lblSatLunchStart_Hour"   runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSatLunchStart_Minute" runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSatLunchEnd_Hour"     runat="server" CssClass="hiddencol"></asp:Label>
                                        <asp:Label ID="lblSatLunchEnd_Minute"   runat="server" CssClass="hiddencol"></asp:Label>
                                    </td>
                                </tr>

                            </table>

                            <div id="existingOrgInfo" runat="server" style="width:100%">

                                <div id="beforeContactSpac1" runat="server" style="height:15px;"></div>
                                <div id="beforeContactSpac2" runat="server" style="margin:5px 0; background: #999; height:1px; width:97%;"></div>
                                <div id="beforeContactSpac3" runat="server" style="height:15px;"></div>


                                <table id="addressAndRegisteredEntitiessRow" runat="server" style="width:100%;">
                                    <tr valign="top">
                                        <td valign="top">

                                            <UC:AddressControl ID="addressControl" runat="server" Visible="False" />
                                            <UC:AddressAusControl ID="addressAusControl" runat="server" Visible="False" />
                        
                                        </td>
                                        <td style="width:20px"></td>
                                        <td valign="top">

                                            <table id="tblRegisteredEntitiesList" runat="server" valign="top">

                                                <tr>
                                                    <td colspan="5" class="nowrap">
                                                        <b>Patients:</b>&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="lnkThisOrgsPatients" runat="server" NavigateUrl="~/RegisterPatientsToOrganisationV2.aspx?id=">Edit All</asp:HyperLink>&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="lnkThisOrgsExistingPatients" runat="server" NavigateUrl="~/RegisterPatientsToOrganisationV2.aspx?id=">Edit Existing Patients</asp:HyperLink>
                                                        <div style="min-height:10px;"></div>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td id="td_staff_heading_list"><b>Staff:</b>&nbsp;&nbsp;<asp:HyperLink ID="lnkThisOrgsStaff" runat="server" NavigateUrl="~/OrganisationListV2.aspx?id=">Edit</asp:HyperLink><br /></td>
                                                    <td id="td_staff_heading_list_space" style="width:25px"></td>
                                                    <td runat="server" visible="false" id="td_referrers_heading_list"><b>Referrers:</b>&nbsp;&nbsp;<asp:HyperLink ID="lnkThisOrgsReferrers" runat="server" NavigateUrl="~/OrganisationListV2.aspx?id=">Edit</asp:HyperLink> <br /></td>
                                                    <td runat="server" visible="false" id="td_referrers_heading_list_space" style="width:25px"></td>
                                                    <td runat="server" visible="false" class="nowrap" id="td_patients_heading_list"><b>Patients:</b>&nbsp;&nbsp;<asp:HyperLink ID="lnkThisOrgsPatients2" runat="server" NavigateUrl="~/OrganisationListV2.aspx?id=">Edit All</asp:HyperLink>&nbsp;&nbsp;<asp:HyperLink ID="lnkThisOrgsExistingPatients2" runat="server" NavigateUrl="~/OrganisationInfo.aspx?id=">Edit Existing Patients</asp:HyperLink><br /></td>
                                                </tr>
                                                <tr>
                                                    <td id="td_staff_list" valign="top"><asp:Panel ID="pnlStaffList" runat="server" ScrollBars="Auto" Wrap="False" CssClass="max_height_170px"><asp:BulletedList ID="lstStaff" runat="server"></asp:BulletedList></asp:Panel></td>
                                                    <td id="td_staff_list_space"></td>
                                                    <td runat="server" visible="false" id="td_referrers_list" valign="top"><asp:Panel ID="pnlReferrersList" runat="server" ScrollBars="Auto" CssClass="max_height_170px"><asp:BulletedList ID="lstReferrers" runat="server"></asp:BulletedList></asp:Panel></td>
                                                    <td runat="server" visible="false" id="td_referrers_list_space"></td>
                                                    <td runat="server" visible="false" id="td_patients_list" valign="top"><asp:Panel ID="pnlPatientsList" runat="server" ScrollBars="Auto" CssClass="max_height_170px"><asp:BulletedList ID="lstPatients" runat="server"></asp:BulletedList></asp:Panel></td>
                                                </tr>
                                            </table>

                                        </td>
                                    </tr>
                                </table>


                                <div id="afterContactSpac1" runat="server" style="height:15px;"></div>
                                <div id="afterContactSpac2" runat="server" style="margin:5px 0; background: #999; height:1px; width:97%;"></div>
                                <div id="afterContactSpac3" runat="server" style="height:15px;"></div>

                                <span id="spnBookingsAndLettersLinks" runat="server">

                                <asp:HyperLink ID="lnkBooking" runat="server" NavigateUrl="~/Bookings.aspx?id=">Bookings</asp:HyperLink><br />
                                <asp:HyperLink ID="lnkBookingList" runat="server" NavigateUrl="~/BookingsListV2.aspx?id=">Booking List</asp:HyperLink><br />
                                <asp:HyperLink ID="lnkUnavailabilities" runat="server">Maintain Unavailabilities</asp:HyperLink><br />
                                <asp:HyperLink ID="lnkInvoices" runat="server">Invoices</asp:HyperLink><br />

                                Letters: <asp:HyperLink ID="lnkPrintLetter" runat="server" NavigateUrl="~/Letters_PrintV2.aspx?org=">Print Single</asp:HyperLink> | 
                                <asp:HyperLink ID="lnkPrintBatchLetters" runat="server" NavigateUrl="~/Letters_PrintBatchV2.aspx?org=">Print Batch</asp:HyperLink> | 
                                <asp:HyperLink ID="lnkLetterPrintHistory" runat="server" NavigateUrl="~/MaintainLetters.aspx?org=">View History</asp:HyperLink> | 
                                <asp:HyperLink ID="lnkLetters" runat="server" NavigateUrl="~/MaintainLetters.aspx?org=">Maintenance</asp:HyperLink>

                                </span>

                            </div>

                        </td>
                    </tr>
                </table>

                <div id="autodivheight" class="divautoheight" style="height:500px;">
                </div>
  
            </div>
        </div>

</asp:Content>



