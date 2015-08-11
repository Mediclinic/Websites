<%@ Page Title="Complete Booking" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="BookingCreateInvoiceGroupV2.aspx.cs" Inherits="BookingCreateInvoiceGroupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="Scripts/post_to_url.js"></script>
    <script type="text/javascript">

        function live_search(str) {

            if (str.length == 0) {
                document.getElementById("div_livesearch").innerHTML = "";
                document.getElementById("div_livesearch").style.border = "0px";
                document.getElementById("div_livesearch").style.display = "none";
                return;
            }
            if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
                xmlhttp = new XMLHttpRequest();
            }
            else {// code for IE6, IE5
                xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
                    var response = String(xmlhttp.responseText);
                    if (response == "SessionTimedOutException")
                        window.location.href = window.location.href;  // reload page
                    document.getElementById("div_livesearch").innerHTML = response;
                    document.getElementById("div_livesearch").style.border = "1px solid #A5ACB2";
                    document.getElementById("div_livesearch").style.display = "";

                    if (document.getElementById("txtSearchFullName").value.length == 0) {
                        document.getElementById("div_livesearch").innerHTML = "";
                        document.getElementById("div_livesearch").style.border = "0px";
                        document.getElementById("div_livesearch").style.display = "none";
                        return;
                    }
                }
            }


            //xmlhttp.open("GET", "/AJAX/AjaxLivePatientSurnameSearch.aspx?q=" + str + "&max_results=80&link_href=" + encodeURIComponent("PatientDetailV2.aspx?type=view&id=[patient_id]") + "&link_onclick=", true);
            xmlhttp.open("GET", "/AJAX/AjaxLivePatientSurnameSearch.aspx?q=" + str + "&max_results=80&link_href=" + encodeURIComponent("PatientDetailV2.aspx?type=view&id=[patient_id]") + "&link_onclick=" + encodeURIComponent("add_patient([patient_id]); return false;"), true);
            xmlhttp.send();
        }

        function clear_live_search() {
            document.getElementById("div_livesearch").innerHTML = "";
            document.getElementById("div_livesearch").style.border = "0px";
            document.getElementById("div_livesearch").style.display = "none";
            document.getElementById("txtSearchFullName").value = "";
        }


        function quick_add() {
            //var h = new Object(); // or just {}
            //h['surname'] = document.getElementById('txtSearchFullName').value.trim();
            //post_to_url("PatientAddV2.aspx?type=add", h, "post");

            // popup & refresh page 
            var retVal = window.showModalDialog("PatientAddV2.aspx?type=add&surname=" + document.getElementById('txtSearchFullName').value.trim(), '', "dialogHeight:900px;dialogWidth:1550px;resizable:yes;center:yes;");
            window.location = window.location;
            return false;
        }


        function add_patient(patient_id) {
            clear_live_search();
            document.getElementById("lblPatientIDToAdd").value = patient_id;
            document.getElementById("btnAddPatient").click();
        }


        function update_total() {

            var hourly = document.getElementById("txtHourlyPrice").value;
            var hours = document.getElementById("txtTotalHours").value;

            if (hourly.length > 0 && !isNaN(hourly) && hours.length > 0 && !isNaN(hours)) {
                var total = parseFloat(hourly) * parseFloat(hours);
                document.getElementById('txtTotal').value = total.toFixed(2);
            }

        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server" ForeColor="#4280B7">Complete Booking</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom" id="header_div" runat="server" style="margin-top:10px !important;">

                    <asp:HiddenField ID="lblPatientIDToAdd" runat="server" />
                    <asp:Button ID="btnAddPatient" runat="server" OnClick="btnAddPatient_Click" CssClass="hiddencol" />

                    <asp:CheckBox ID="chkGenerateSystemLetters" runat="server" Text="&nbsp;Generate System Letters Now" />
                    <span id="spaceGenerateSystemLetters" style="display:block;height:5px;" runat="server"></span>
                    <table class="block_center">
                        <tr style="vertical-align:bottom;">
                            <td class="nowrap" style="width:30px"></td>
                            <td class="nowrap"><asp:Label ID="lblFullNameSearch" runat="server">Add Patient (Search Surname)</asp:Label></td>
                            <td class="nowrap" style="width:10px"></td>
                            <td class="nowrap">
                                <asp:TextBox ID="txtSearchFullName" runat="server" placeholder="Enter Surname" onkeyup="live_search(this.value)" autocomplete="off" onkeydown="return (event.keyCode!=13);" ></asp:TextBox>
                                <div id="div_livesearch" style="display:none;position:absolute;background:#FFFFFF;"></div>
                            </td>
                            <td class="nowrap" style="width:5px"></td>
                            <td class="nowrap"><button type="button" name="btnClearFullNameSearch" onclick="clear_live_search(); return false;">Clear</button></td>
                            <td class="nowrap" style="width:5px"></td>
                            <td class="nowrap"><button type="button" name="btnQuickAdd" onclick="quick_add(); return false;">New Patient</button></td>
                        </tr>
                    </table>

                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:ValidationSummary ID="validationSummaryHidden" runat="server" CssClass="failureNotification" ValidationGroup="validationSummaryHidden" Visible="false"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>

                <table id="main_table" runat="server" border="0" cellpadding="0" cellspacing="0">
                    <tr style="vertical-align:top;">
                        <td style="text-align:center;">

                            <asp:HiddenField ID="hiddenViewDdlBookingPatientIDs" Value="" runat="server" />

                            <h4>Booked</h4>
                            <div style="line-height:8px;">&nbsp;</div>

                            <asp:GridView ID="GrdBookingPatients" runat="server" 
                                    AutoGenerateColumns="False" DataKeyNames="booking_patient_id" 
                                    OnRowCancelingEdit="GrdBookingPatients_RowCancelingEdit" 
                                    OnRowDataBound="GrdBookingPatients_RowDataBound" 
                                    OnRowEditing="GrdBookingPatients_RowEditing" 
                                    OnRowUpdating="GrdBookingPatients_RowUpdating" ShowFooter="False" 
                                    OnRowCommand="GrdBookingPatients_RowCommand" 
                                    OnRowCreated="GrdBookingPatients_RowCreated"
                                    AllowSorting="True" 
                                    OnSorting="GrdBookingPatients_Sorting"
                                    ClientIDMode="Predictable"
                                    CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                                <Columns> 

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="bp_booking_patient_id" ItemStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("bp_booking_patient_id") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Patient ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_patient_id" ItemStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblPatientId" runat="server" Text='<%# Bind("patient_patient_id") %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblRowNbr" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label> 
                                        </ItemTemplate> 
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left" SortExpression="remove" FooterStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Button ID="btnRemove" runat="server" Text="Remove" CommandName="Remove" CommandArgument='<%# Eval("bp_booking_patient_id") %>' OnClientClick="javascript:if (!confirm('Are you sure you want to remove this?')) return false;" />
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" SortExpression="patient_person_firstname" FooterStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <table style="width:100%;">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblName" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align:left;">
                                                        <asp:Label ID="lblEPCInfo" runat="server" Text='<%# Eval("epc_text") == DBNull.Value ? "" : Eval("epc_text") %>' CssClass="nowrap" ></asp:Label> 
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Service" HeaderStyle-HorizontalAlign="Left" SortExpression="bp_offering_name" FooterStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:DropDownList ID="ddlBPOffering" runat="server" DataTextField="o_name" DataValueField="o_offering_id" ></asp:DropDownList> 
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <table style="width:100%;">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblBPOffering" runat="server" Text='<%# Eval("bp_offering_name")  %>' ></asp:Label> 
                                                    </td>
                                                </tr>
                                                <tr runat="server" visible='<%# Eval("show_area_treated") != DBNull.Value && (bool)Eval("show_area_treated") %>'>
                                                    <td>
                                                        <div style="min-height:12px;"></div>
                                                        <span style="white-space:nowrap">Area Treated:</span> <asp:TextBox ID="txtAreaTreated" runat="server" Columns="15" Text='<%# Eval("bp_area_treated") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left" SortExpression="remove" FooterStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblNote" runat="server" Text='<%# Eval("notes_text") %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="" ShowHeader="false" SortExpression="hide_edit_col" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top"> 
                                        <EditItemTemplate> 
                                            <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="validationSummary"></asp:LinkButton> 
                                            <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                        </EditItemTemplate> 
                                        <ItemTemplate> 
                                            <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-20.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Extras" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" ItemStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 

                                            <asp:Repeater id="rptBkPtOfferings" runat="server">
                                                <HeaderTemplate>
                                                    <table style="width:100%" class="padded-table-1px">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr style="vertical-align:top;">
                                                        <td class="nowrap"><asp:ImageButton ID="btnRemoveBookingPatientOffering" runat="server" OnCommand="btnRemoveBookingPatientOffering_Command" CommandName="RemoveBookingPatientOffering" CommandArgument='<%# Eval("bpo_booking_patient_offering_id") %>' ImageUrl="~/images/Delete-icon-16.png" /></td>
                                                        <td style="width:2px;"></td>

                                                        <td class="nowrap" style="text-align:left;">
                                                            <asp:Label ID="lblOfferingName" Text='<%# Eval("offering_name") %>' runat="server"/>
                                                        </td>

                                                        <td style="width:8px;"></td>
                                                        <td class="nowrap"><asp:Label ID="lblQuantity" runat="server" Text='<%# "<b>" + Eval("bpo_quantity") + "</b>" %>' /></td>
                                                        <td style="width:1px;"></td>
                                                        <td class="nowrap"> x </td>
                                                        <td style="width:1px;"></td>
                                                        <td class="nowrap"><asp:Label ID="lblItemCost" runat="server" Text='<%# string.Format("{0:C}", (
                                                                           ((Eval("is_dva") != DBNull.Value && (bool)Eval("is_dva") && Eval("offering_dva_company_code").ToString().Length > 0) ? (decimal)Eval("offering_dva_charge") : 
                                                                             (
                                                                               (Eval("is_tac") != DBNull.Value && (bool)Eval("is_tac") && Eval("offering_tac_company_code").ToString().Length > 0) ? (decimal)Eval("offering_tac_charge") : 
                                                                               (decimal)Eval("offering_default_price")
                                                                             )
                                                                           )
                                                                           * 
                                                                           (Convert.ToBoolean(Eval("offering_is_gst_exempt")) ? 1 : (  (100M + Convert.ToDecimal(((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["GST_Percent"].Value))/100M   )))   ) %>' /></td>
                                                        <td style="width:1px;"></td>
                                                        <td class="nowrap"> = </td>
                                                        <td style="width:1px;"></td>
                                                        <td class="nowrap"><asp:Label ID="lblTotalcost" runat="server" Text='<%# string.Format("{0:C}", (
                                                                           ((Eval("is_dva") != DBNull.Value && (bool)Eval("is_dva") && Eval("offering_dva_company_code").ToString().Length > 0) ? (decimal)Eval("offering_dva_charge") : 
                                                                             (
                                                                               (Eval("is_tac") != DBNull.Value && (bool)Eval("is_tac") && Eval("offering_tac_company_code").ToString().Length > 0) ? (decimal)Eval("offering_tac_charge") : 
                                                                               (decimal)Eval("offering_default_price")
                                                                             )
                                                                           )
                                                                           * 
                                                                           Convert.ToDecimal(Eval("bpo_quantity")) * (Convert.ToBoolean(Eval("offering_is_gst_exempt")) ? 1 : (  (100M + Convert.ToDecimal(((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["GST_Percent"].Value))/100M   )))   )  %>' /></td>
                                                        <td style="width:8px;"></td>
                                                        <td class="nowrap" align="right">
                                                            <asp:ImageButton ID="btnAddQty" runat="server" OnCommand="btnRemoveBookingPatientOffering_Command" CommandName="AddQty" CommandArgument='<%# Eval("bpo_booking_patient_offering_id") %>' ImageUrl="~/images/add-icon-16.png" />
                                                            <asp:ImageButton ID="btnSubtractQty" runat="server" OnCommand="btnRemoveBookingPatientOffering_Command" CommandName="SubtractQty" CommandArgument='<%# Eval("bpo_booking_patient_offering_id") %>' ImageUrl="~/images/subtract-icon-16.png" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2"></td>
                                                        <td colspan="13" style="text-align:left;">
                                                             <div id="div" runat="server" visible='<%# Eval("show_area_treated") != DBNull.Value && (bool)Eval("show_area_treated") %>'>
                                                                <asp:HiddenField ID="hiddenBookingPatientOfferingID" runat="server" Value='<%# Eval("bpo_booking_patient_offering_id") %>' />
                                                                 <table style="width:100%;">
                                                                     <tr>
                                                                         <td style="white-space:nowrap; width:10px;">
                                                                             Area Treated:&nbsp;
                                                                         </td>
                                                                         <td style="width:100%">
                                                                             <asp:TextBox ID="txtAreaTreated" runat="server" Text='<%# Eval("bpo_area_treated") %>' style="width:100%;min-width:100px;" />
                                                                         </td>
                                                                     </tr>
                                                                 </table>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>

                                            <center>
                                                <div id="space_before_offerings_ddl" runat="server" style="height:9px;"></div>
                                                <asp:DropDownList ID="ddlOfferings" DataValueField="o_offering_id" DataTextField="o_name" runat="server"/>
                                                <div id="space_before_addcancelbuttons" runat="server" style="height:5px;"></div>
                                                <asp:Button ID="btnBkPtOfferingsAdd" runat="server" Text="Add"  />
                                                <asp:Button ID="btnBkPtOfferingsCancelAdd" runat="server" Text="Cancel"  />
                                            </center>

                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                </Columns> 

                            </asp:GridView>

                            <br />
                            <br />

                            <asp:Button ID="btnSubmit" runat="server" Text = "Complete" OnClick="btnSubmit_Click"  OnClientClick="show_page_load_message();" CausesValidation="True" ValidationGroup="validationSummaryHidden" />
                            <span id="buttonsSpace" runat="server">&nbsp;&nbsp;</span>                           
                            <asp:Button ID="btnCancel" runat="server" Text = "Cancel" OnClientClick="javascript:window.returnValue=false;window.close();return false;" />


                        </td>
                    </tr>
                </table>


                <br id="br_close_button_space" runat="server" visible="false" />
                <asp:Button ID="btnClose" runat="server" Text = "Close" Visible="False" OnClientClick="javascript:window.returnValue=false;window.close();return false;" />

            </center>


            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>


        </div>
    </div>

</asp:Content>



