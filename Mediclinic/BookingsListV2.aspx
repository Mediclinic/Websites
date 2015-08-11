<%@ Page Title="Bookings List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="BookingsListV2.aspx.cs" Inherits="BookingsListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function doClick(buttonName, e) {
            //the purpose of this function is to allow the enter key to 
            //point to the correct button to click.
            var key;

            if (window.event)
                key = window.event.keyCode;     //IE
            else
                key = e.which;     //firefox

            if (key == 13) {
                //Get the button the user wants to have clicked
                var btn = document.getElementById(buttonName);
                if (btn != null) { //If we find the button click it
                    btn.click();
                    event.keyCode = 0
                }
            }

            return (key != 13);
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Bookings List</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:800px;">

                <div id="div_search_section" runat="server" class="border_top_bottom user_login_form_no_width_div">
                    <center>

                        <asp:Panel ID="pnlDefaultButton" runat="server" DefaultButton="btnDefaultButton_NoSubmit" style="margin:6px auto;">
                            <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />
                            <table>
                                <tr style="vertical-align:middle;">
                                    <td>
                                        <table>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">Start Date: </asp:Label></td>
                                                <td class="nowrap"><asp:TextBox ID="txtStartDate" runat="server" Columns="10"/></td>
                                                <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                                                <td class="nowrap"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                                                <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                                <td class="nowrap"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:40px"></td>

                                    <td  class="nowrap">
                                        <table style="line-height:10px;">
                                            <tr style="vertical-align:top;">
                                                <td class="nowrap">

                                                    <asp:CheckBox ID="chkIncCompleted" runat="server" Text="&nbsp;Inc Completed"/> 
                                                    <br />
                                                    <asp:CheckBox ID="chkIncIncomplete" runat="server" Text="&nbsp;Inc Incomplete"/> 
                                                    <br />
                                                    <asp:CheckBox ID="chkIncCancelled" runat="server" Text="&nbsp;Inc Cancelled"/> 
                                                    <br />
                                                    <asp:CheckBox ID="chkIncDeleted" runat="server" Text="&nbsp;Inc Deleted"/> 
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:40px"></td>

                                    <td style="vertical-align:middle">
                                        <table>
                                            <tr>
                                                <td class="nowrap" style="text-align:center">
                                                    Booking Nbr Search
                                                    <div style="line-height:7px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap" style="text-align:center">
                                                    <asp:TextBox ID="txtBookingNbrSearch" runat="server" Width="80" onkeydown="return doClick('btnSearch',event);" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:30px"></td>

                                    <td>
                                        <table>
                                            <tr>
                                                <td class="nowrap" style="text-align:center">
                                                    <asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" Width="100%" style="min-width:65px;" />
                                                    <div style="line-height:7px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap" style="text-align:center">
                                                    <asp:Button ID="btnPrint" runat="server" Text="Print" OnClick="btnPrint_Click" Width="100%" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:30px"></td>

                                </tr>
                            </table>
                        </asp:Panel>
                        
                    <center>
                </div>

                <img src="images/printer_green-24.png" alt="print letter icon" style="margin:0 5px 5px 0;" />Print Letter
                <span style="padding-left:50px;"><img src="images/Calendar-icon-24px.png" alt="booking sheet icon" style="margin:0 5px 5px 0;" />BK Sheet</span>
            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <br />

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdBooking" runat="server" 
                            AutoGenerateColumns="False" DataKeyNames="booking_booking_id" 
                            OnRowCancelingEdit="GrdBooking_RowCancelingEdit" 
                            OnRowDataBound="GrdBooking_RowDataBound" 
                            OnRowEditing="GrdBooking_RowEditing" 
                            OnRowUpdating="GrdBooking_RowUpdating" ShowFooter="False" 
                            OnRowCommand="GrdBooking_RowCommand" 
                            OnRowDeleting="GrdBooking_RowDeleting" 
                            OnRowCreated="GrdBooking_RowCreated"
                            AllowSorting="True" 
                            OnSorting="GridView_Sorting"
                            RowStyle-VerticalAlign="top" 
                            CellPadding="1"
                            ClientIDMode="Predictable"
                            CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_booking_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("booking_booking_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Appointment Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_date_start"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAppointmentDate"      runat="server" Text='<%# Eval("booking_date_start", "{0:dd-MM-yyyy}") %>'></asp:Label>&nbsp;&nbsp;&nbsp;
                                    <asp:Label ID="lblAppointmentDateStart" runat="server" Text='<%# Eval("booking_date_start", "{0:HH:mm}") %>'></asp:Label>-<asp:Label ID="lblAppointmentDateEnd"   runat="server" Text='<%# Eval("booking_date_end", "{0:HH:mm}") %>'></asp:Label>&nbsp;
                                </ItemTemplate> 
                            </asp:TemplateField> 
 
                            <asp:TemplateField HeaderText="Patient"  HeaderStyle-HorizontalAlign="Left" SortExpression="person_patient_firstname"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPatient" runat="server" Text='<%# Eval("person_patient_firstname") + " " + Eval("person_patient_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Organisation"  HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_name"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("organisation_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Provider"  HeaderStyle-HorizontalAlign="Left" SortExpression="person_provider_firstname"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblProvider" runat="server" Text='<%# Eval("person_provider_firstname") + " " + Eval("person_provider_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Offering"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_date_start"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOffering" runat="server" Text='<%# Eval("offering_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Status"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_status_descr"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("booking_status_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Confirmed By"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_booking_confirmed_by_type_id desc, confirmed_by_text"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblConfirmedBy" runat="server" Text='<%# Eval("confirmed_by_text") %>'></asp:Label> 
                                    <asp:Label ID="lblConfirmedDate" runat="server" Text='<%# Eval("booking_date_confirmed") == DBNull.Value ? "" : ("["+ Eval("booking_date_confirmed", "{0:dd-MM-yyyy}") + "]") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Deleted By"  HeaderStyle-HorizontalAlign="Left" SortExpression="booking_deleted_by"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDeletedBy" runat="server" Text='<%# Eval("person_deleted_by_firstname") + " " + Eval("person_deleted_by_surname") %>'></asp:Label> 
                                    <asp:Label ID="lblDeletedDate" runat="server" Text='<%# Eval("booking_date_deleted") == DBNull.Value ? "" : ("["+ Eval("booking_date_deleted", "{0:dd-MM-yyyy}") + "]") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Sys Letters <br /> Generated" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" SortExpression="booking_has_generated_system_letters" HeaderStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblGeneratedSystemLetters" runat="server" Text='<%# Eval("booking_has_generated_system_letters").ToString()=="True"?"Yes":"No" %>' />
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblNotes" runat="server" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkPrintLetter" runat="server" NavigateUrl='<%#  String.Format("~/Letters_PrintV2.aspx?booking={0}",Eval("booking_booking_id")) %>' ImageUrl="~/images/printer_green-24.png" AlternateText="Letters" ToolTip="Letters" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblViewInvoice" runat="server" ToolTip= "View Invoice"></asp:Label>
                                    <asp:LinkButton ID="lnkReverseInvoice" runat="server" CommandName="Reverse" CommandArgument='<%# Bind("booking_booking_id") %>' Text="Reverse" AlternateText="Reverses booking status to uncompleted and deletes all invoices associated with it" ToolTip= "Reverses booking status to uncompleted and deletes all invoices associated with it" OnClientClick="javascript:if (!confirm('Are you sure you want to reverse this booking status to uncommpleted and delete all invoices associated with it?')) return false;"></asp:LinkButton>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkBookingSheetForPatient" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Booking Sheet" ToolTip="Booking Sheet" />
                                </ItemTemplate> 
                            </asp:TemplateField> 
 

                            <%-- 
                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditBookingValidationSummary"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="AddBookingValidationGroup"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" /> 
                            --%>

                        </Columns> 
                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



