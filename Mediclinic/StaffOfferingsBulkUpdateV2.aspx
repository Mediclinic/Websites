<%@ Page Title="Staff Commission Bulk Update" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StaffOfferingsBulkUpdateV2.aspx.cs" Inherits="StaffOfferingsBulkUpdateV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function date_active_changed(obj) {

            if (obj.value.length == 2)
                obj.value += "-";

            if (obj.value.length == 5)
                obj.value += "-"; //  + String((new Date()).getFullYear()).substr(0, 2);
        }


        function validate_and_confirm() {

            if (Page_ClientValidate("validationSummary")) {

                var txtNewActiveDate = document.getElementById('txtNewActiveDate')
                if (document.getElementById('chkShowDateWarning').checked && txtNewActiveDate.value.length == 0)
                    if (!confirm('Blank active date means that the updated values will be inactive in the system. Continue anyway?'))
                        return false;

                var ddlStaff = document.getElementById("ddlNewStaff");
                var staffID = ddlStaff.options[ddlStaff.selectedIndex].value;
                var staffName = ddlStaff.options[ddlStaff.selectedIndex].text;

                var ddlOffering = document.getElementById("ddlNewOffering");
                var OfferingID = ddlOffering.options[ddlOffering.selectedIndex].value;
                var OfferingName = ddlOffering.options[ddlOffering.selectedIndex].text;

                var message = "Are you sure that you want to update to these details:\n\r    For " + staffName + "\n\r    For " + OfferingName;

                return confirm(message);
            }
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Staff Commission Bulk Update</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">
            <div class="user_login_form" style="width: 600px;">

                <div class="border_top_bottom">
                    <center>

                        <input id="chkShowDateWarning" type="checkbox" value="Accept Form" name="chkShowDateWarning" runat="server" checked="checked" /> &nbsp;<label for="chkShowDateWarning" style="font-weight:normal;">Show "<i>no date entered</i>" warning</label>

                    </center>
                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>

                <div id="_autodivheight" class="divautoheight">

                <table class="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center" border="1" style="border-collapse:collapse;">

                    <tr>
                        <th>Staff</th>
                        <th>Offering</th>
                        <th>Commission</th>
                        <th>Comm %</th>
                        <th>Fixed Rate</th>
                        <th>Rate</th>
                        <th>Active Date</th>
                        <th></th>
                        <th></th>
                    </tr>

                    <tr>

                        <td><asp:DropDownList ID="ddlNewStaff" runat="server"> </asp:DropDownList></td>
                        <td><asp:DropDownList ID="ddlNewOffering" runat="server"> </asp:DropDownList></td>
                        <td><asp:CheckBox ID="chkNewIsCommission" runat="server" /></td>
                        <td class="nowrap">
                            <asp:TextBox Width="90%" ID="txtNewCommissionPercent" runat="server" Text='0.00'></asp:TextBox> 
                            <asp:RequiredFieldValidator ID="txtValidateNewCommissionPercentRequired" runat="server" CssClass="failureNotification"  
                                ControlToValidate="txtNewCommissionPercent" 
                                ErrorMessage="Commission percent is required."
                                Display="Dynamic"
                                ValidationGroup="validationSummary">*</asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="txtValidateNewCommissionPercentRange" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtNewCommissionPercent"
                                ErrorMessage="Commission percent must be a number and must be between 0 and 100" Type="Double" MinimumValue="0.00" MaximumValue="100.00" 
                                Display="Dynamic"
                                ValidationGroup="validationSummary">*</asp:RangeValidator>
                        </td>
                        <td><asp:CheckBox ID="chkNewIsFixedRate" runat="server" /></td>
                        <td class="nowrap">
                            <asp:TextBox Width="90%" ID="txtNewFixedRate" runat="server" Text='0.00'></asp:TextBox> 
                            <asp:RequiredFieldValidator ID="txtValidateNewFixedRateRequired" runat="server" CssClass="failureNotification"  
                                ControlToValidate="txtNewFixedRate" 
                                ErrorMessage="Fixed rate is required."
                                Display="Dynamic"
                                ValidationGroup="validationSummary">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="txtValidateNewFixedRateRegex" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtNewFixedRate"
                                ValidationExpression="^\d+(\.\d{1,2})?$"
                                ErrorMessage="Fixed rate can only be numbers and option decimal place with 1 or 2 digits following."
                                Display="Dynamic"
                                ValidationGroup="validationSummary">*</asp:RegularExpressionValidator>
                        </td>
                        <td class="nowrap">
                            <asp:TextBox Width="90%" ID="txtNewActiveDate" runat="server" onkeyup="javascript:date_active_changed(this);"></asp:TextBox> 
                            <asp:RegularExpressionValidator ID="txtValidateNewActiveDateRegex" runat="server" CssClass="failureNotification" 
                                ControlToValidate="txtNewActiveDate"
                                ValidationExpression="^\d{2}\-\d{2}\-\d{4}$"
                                ErrorMessage="Active Date must either be empty (indicating it is inactive) or the format must be dd-mm-yyyy"
                                Display="Dynamic"
                                ValidationGroup="validationSummary">*</asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="txtValidateNewActiveDate" runat="server"  CssClass="failureNotification"  
                                ControlToValidate="txtNewActiveDate"
                                OnServerValidate="ValidDateCheck"
                                ErrorMessage="Invalid Active Date"
                                Display="Dynamic"
                                ValidationGroup="validationSummary">*</asp:CustomValidator>
                        </td>
                        <td></td>
                        <td><asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" CausesValidation="True" OnClientClick="return validate_and_confirm();" Text="Update" ValidationGroup="validationSummary"></asp:Button> </td>
                    </tr>
                </table>


                </div>

                <br />
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="window.returnValue=false;self.close();" />

            </center>

        </div>
    </div>


</asp:Content>



