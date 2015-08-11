<%@ Page Title="Contact List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ContactAusDetailV2.aspx.cs" Inherits="ContactAusDetailV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        var previous_contact_type_id_selected = ""; // global
        function ddl_health_card_type_changed() {

            // only if url: contact_type_group=1 // addresses
            if (getUrlVars()['contact_type_group'] != 1)
                return;

            if (document.getElementById("ddlContactType").options[ddlContactType.selectedIndex].value == "37")  // PO Box
            {
                document.getElementById("txtAddrLine1").value = "PO Box";
            }
            else if (document.getElementById("ddlContactType").options[ddlContactType.selectedIndex].value == "262")  // GPO Box
            {
                document.getElementById("txtAddrLine1").value = "GPO Box";
            }
            else {
                if (previous_contact_type_id_selected == "37" || previous_contact_type_id_selected == "262") {
                    document.getElementById("txtAddrLine1").value = "";
                }
            }

            previous_contact_type_id_selected = document.getElementById("ddlContactType").options[ddlContactType.selectedIndex].value;
        }

        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }

        function get_suburb() {
            isMobileDevice = document.getElementById('hiddenIsMobileDevice').value == "1";
            if (isMobileDevice)
                open_new_tab('Contact_SuburbListPopupV2.aspx');
            else
                window.showModalDialog("Contact_SuburbListPopupV2.aspx", 'Show Popup Window', "dialogHeight:810px;dialogWidth:700px;resizable:yes;center:yes;");
        }
        function clear_suburb() {
            document.getElementById('suburbID').value = '-1';
            document.getElementById('lblSuburbText').innerHTML = '--';
            //document.getElementById('btnSuburbSelectionUpdate').click();  // call button press to let the code behind use this id and update accordingly
        }


        function set_suburb(retVal) {
            var index = retVal.indexOf(":");
            document.getElementById('suburbID').value = retVal.substring(0, index);
            document.getElementById('lblSuburbText').innerHTML = retVal.substring(index + 1);
            //document.getElementById('btnSuburbSelectionUpdate').click();
        }


    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><h3><asp:Label ID="lblHeading" runat="server">Contact</asp:Label></h3></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:HiddenField ID="hiddenIsMobileDevice" runat="server" Value="0" />

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>

                <asp:Button ID="btnUpdateStreetAndSuburb" runat="server" CssClass="hiddencol" onclick="btnUpdateStreetAndSuburb_Click" />
                <asp:Button ID="btnUpdateType" runat="server" CssClass="hiddencol" onclick="btnUpdateType_Click" />


                <table id="maintable" runat="server">
                    <tr id="idRow" runat="server">
                        <td>ID</td>
                        <td></td>
                        <td><asp:Label ID="lblId" runat="server"></asp:Label></td>
                    </tr>
                    <tr>
                        <td class="nowrap">Type</td>
                        <td style="min-width:8px;"></td>
                        <td class="nowrap">
                            <asp:DropDownList ID="ddlContactType" runat="server" DataTextField="at_descr" DataValueField="at_contact_type_id" onchange="javascript:ddl_health_card_type_changed();" />
                            <small><asp:HyperLink ID="lnkUpdateType" runat="server" onfocus="set_focus_color(this, true);" onblur="set_focus_color(this, false, 'transparent');" TabIndex="-1"></asp:HyperLink></small>
                            <asp:Label ID="lblContactType" runat="server" Font-Bold="True"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="nowrap"><asp:Label ID="lblLine1Descr" runat="server">Line 1</asp:Label></td>
                        <td style="min-width:8px;"></td>
                        <td class="nowrap">
                            <asp:TextBox ID="txtAddrLine1" runat="server" Columns="30"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="txtValidateAddrLine1Required" runat="server" CssClass="failureNotification"  
                                ControlToValidate="txtAddrLine1" 
                                ErrorMessage="AddrLine1 is required."
                                Display="Dynamic"
                                ValidationGroup="EditContactValidationSummary">*Required</asp:RequiredFieldValidator>
                            <asp:Label ID="lblAddrLine1" runat="server" Font-Bold="True"/>
                        </td>
                    </tr>
                    <tr id="line2Row" runat="server">
                        <td class="nowrap">Line 2</td>
                        <td></td>
                        <td class="nowrap">
                            <asp:TextBox ID="txtAddrLine2" runat="server"  Columns="30"></asp:TextBox>
                            <asp:Label ID="lblAddrLine2" runat="server" Font-Bold="True"/>
                        </td>
                    </tr>
                    <tr id="streetRow" runat="server">
                        <td class="nowrap">Street</td>
                        <td></td>
                        <td class="nowrap">
                            <asp:TextBox ID="txtStreet" runat="server"  Columns="30"></asp:TextBox>
                            <asp:Label ID="lblStreet" runat="server" Font-Bold="True"/>
                            <asp:DropDownList ID="ddlAddressChannelType" runat="server"/>
                            <asp:Label ID="lblAddressChannelType" runat="server" Font-Bold="True"/>
                        </td>
                    </tr>
                    <tr id="suburbRow" runat="server">
                        <td class="nowrap">Suburb</td>
                        <td></td>
                        <td class="nowrap">
                            <table>
                                <tr>
                                    <td style="min-width:175px"><asp:Label  ID="lblSuburbText" runat="server" Text="--" CssClass="nowrap" /></td>
                                    <td style="min-width:15px"></td>
                                    <td>
                                        <a id="lnkGetSuburb" runat="server" href="javascript:void(0)" onclick="javascript:get_suburb(); return false;">Get Suburb</a>&nbsp;&nbsp;
                                        <a id="lnkClearSuburb" runat="server" href="javascript:void(0)" onclick="javascript:clear_suburb(); return false;">Clear Suburb</a>
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="suburbID" runat="server" Value="-1" />
                            <asp:Button ID="btnSuburbSelectionUpdate" runat="server" CssClass="hiddencol" Text=""  OnClick="btnSuburbSelectionUpdate_Click" />
                        </td>
                    </tr>
                    <tr id="countryRow" runat="server">
                        <td class="nowrap">Country</td>
                        <td></td>
                        <td class="nowrap">
                            <asp:DropDownList ID="ddlCountry" runat="server" DataTextField="descr" DataValueField="country_id"/>
                            <asp:Label ID="lblCountry" runat="server" Font-Bold="True"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="nowrap">Note</td>
                        <td></td>
                        <td class="nowrap">
                            <asp:TextBox ID="txtFreeText" runat="server"  Columns="30"></asp:TextBox>
                            <asp:Label ID="lblFreeText" runat="server" Font-Bold="True"/>
                        </td>
                    </tr>
                    <tr id="billingRow" runat="server">
                        <td class="nowrap">For Billing</td>
                        <td></td>
                        <td class="nowrap">
                            <asp:CheckBox ID="chkIsBilling" runat="server" Checked="true" />
                            <asp:Label ID="lblIsBilling" runat="server" Font-Bold="True"/>
                        </td>
                    </tr>
                    <tr id="nonbillingRow" runat="server">
                        <td class="nowrap">For Non Billing</td>
                        <td></td>
                        <td class="nowrap">
                            <asp:CheckBox ID="chkIsNonBilling" runat="server" Checked="true" />
                            <asp:Label ID="lblIsNonBilling" runat="server" Font-Bold="True"/>
                        </td>
                    </tr>


                    <tr style="height:10px">
                        <td colspan="3"></td>
                    </tr>

                    <tr>
                        <td colspan="3" style="text-align:center">
                            <br />  
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" CausesValidation="True" ValidationGroup="EditContactValidationSummary" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click"  OnClientClick="window.returnValue=false;self.close();" />
                            <br />              
                        </td>
                    </tr>

                </table>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>


</asp:Content>



