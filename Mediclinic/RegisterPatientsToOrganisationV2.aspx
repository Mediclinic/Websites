<%@ Page Title="Organisation List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="RegisterPatientsToOrganisationV2.aspx.cs" Inherits="RegisterPatientsToOrganisationV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function open_new_window(URL) {
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,height=" + screen.height + ',width=' + screen.width);
            NewWindow.location = URL;
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Manage Registrations For  </asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">
                    <center>
                        Search By Surname: <asp:TextBox ID="txtSearchSurname" runat="server"/>
                        <asp:CheckBox ID="chkSurnameSearchOnlyStartWith" runat="server" /> &nbsp;<label for="chkSurnameSearchOnlyStartWith" style="font-weight:normal;">starts with</label>
                        <span style="padding-left:20px;">
                            <asp:Button ID="btnSearchSurname" runat="server" Text="Search" onclick="btnSearchSurname_Click" />
                            <asp:Button ID="btnClearSurnameSearch" runat="server" Text="Clear" onclick="btnClearSurnameSearch_Click" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnExport" runat="server" Text="Eport" OnClick="btnExport_Click" />
                        </span>

                        <br />

                        <asp:LinkButton ID="btnSurnameA" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="A" Text="A" />
                        <asp:LinkButton ID="btnSurnameB" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="B" Text="B" />
                        <asp:LinkButton ID="btnSurnameC" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="C" Text="C" />
                        <asp:LinkButton ID="btnSurnameD" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="D" Text="D" />
                        <asp:LinkButton ID="btnSurnameE" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="E" Text="E" />
                        <asp:LinkButton ID="btnSurnameF" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="F" Text="F" />
                        <asp:LinkButton ID="btnSurnameG" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="G" Text="G" />
                        <asp:LinkButton ID="btnSurnameH" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="H" Text="H" />
                        <asp:LinkButton ID="btnSurnameI" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="I" Text="I" />
                        <asp:LinkButton ID="btnSurnameJ" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="J" Text="J" />
                        <asp:LinkButton ID="btnSurnameK" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="K" Text="K" />
                        <asp:LinkButton ID="btnSurnameL" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="L" Text="L" />
                        <asp:LinkButton ID="btnSurnameM" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="M" Text="M" />
                        <asp:LinkButton ID="btnSurnameN" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="N" Text="N" />
                        <asp:LinkButton ID="btnSurnameO" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="O" Text="O" />
                        <asp:LinkButton ID="btnSurnameP" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="P" Text="P" />
                        <asp:LinkButton ID="btnSurnameQ" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="Q" Text="Q" />
                        <asp:LinkButton ID="btnSurnameR" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="R" Text="R" />
                        <asp:LinkButton ID="btnSurnameS" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="S" Text="S" />
                        <asp:LinkButton ID="btnSurnameT" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="T" Text="T" />
                        <asp:LinkButton ID="btnSurnameU" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="U" Text="U" />
                        <asp:LinkButton ID="btnSurnameV" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="V" Text="V" />
                        <asp:LinkButton ID="btnSurnameW" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="W" Text="W" />
                        <asp:LinkButton ID="btnSurnameX" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="X" Text="X" />
                        <asp:LinkButton ID="btnSurnameY" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="Y" Text="Y" />
                        <asp:LinkButton ID="btnSurnameZ" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="Z" Text="Z" />
                        <asp:LinkButton ID="btnSurnameAll" runat="server" OnCommand="btnSurnameSearch_Command" CommandName="SurnameSearch" CommandArgument="All" Text="All" />

                    </center>
                </div>

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit
                <span style="padding-left:20px;"><img src="imagesV2/x.png" alt="delete icon" style="margin:0 5px 5px 0;" />Delete</span>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <div class="text-center">
                <asp:HyperLink ID="lnkThisOrg" runat="server" NavigateUrl="~/OrganisationListV2.aspx?id=">Edit</asp:HyperLink> 
                <div style="height:8px;"></div>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;width:auto;padding-right:10px;">

                    <asp:GridView ID="GrdRegistration" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="register_patient_id" 
                         OnRowCancelingEdit="GrdRegistration_RowCancelingEdit" 
                         OnRowDataBound="GrdRegistration_RowDataBound" 
                         OnRowEditing="GrdRegistration_RowEditing" 
                         OnRowUpdating="GrdRegistration_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdRegistration_RowCommand" 
                         OnRowDeleting="GrdRegistration_RowDeleting" 
                         OnRowCreated="GrdRegistration_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         AllowPaging="True"
                         OnPageIndexChanging="GrdRegistration_PageIndexChanging"
                         PageSize="17"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                         <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_patient_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_patient_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_patient_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patient" HeaderStyle-HorizontalAlign="Left" SortExpression="surname,firstname,middlename" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left nowrap" > 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlPatient" runat="server"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lnkPatient" runat="server" Text='<%# Eval("surname") + ", " + Eval("firstname") + " " + Eval("middlename") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewPatient" runat="server"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Date Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_patient_date_added"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_patient_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_patient_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Last Booking"  HeaderStyle-HorizontalAlign="Left" SortExpression="last_booking_date"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblLastBooking" runat="server" Text='<%# Eval("last_booking_date", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLastBooking" runat="server" Text='<%# Eval("last_booking_date", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Next Booking"  HeaderStyle-HorizontalAlign="Left" SortExpression="next_booking_date"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblNextBooking" runat="server" Text='<%# Eval("next_booking_date", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblNextBooking" runat="server" Text='<%# Eval("next_booking_date", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="EPC Expires" HeaderStyle-HorizontalAlign="Left" SortExpression="epc_expire_date" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblEPCExpiry" runat="server" Text='<%# Eval("epc_expire_date", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="EPCs Remaining" HeaderStyle-HorizontalAlign="Left" SortExpression="epc_count_remaining" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblEPCsRemaining" runat="server" Text='<%# Eval("epc_count_remaining") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Last Recall Letter Sent" HeaderStyle-HorizontalAlign="Left" SortExpression="most_recent_recall_sent" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkLastRecallLetterSent" runat="server" Text='<%# Eval("most_recent_recall_sent", "{0:dd-MM-yyyy}") %>' NavigateUrl='<%# String.Format("~/Letters_SentHistoryV2.aspx?patient={0}", Eval("patient_id")) %>' />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Booking" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center" SortExpression="num_registered_orgs"> 
                                <EditItemTemplate> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkBookings" runat="server" NavigateUrl='<%#  String.Format("~/BookingScreenGetPatientOrgsV2.aspx?patient_id={0}",Eval("patient_id")) %>' ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Bookings" ToolTip="Bookings" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Edit" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" Visible="false"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True" /> 
                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



