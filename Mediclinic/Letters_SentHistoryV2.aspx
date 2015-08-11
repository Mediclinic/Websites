<%@ Page Title="Letters Sent History" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Letters_SentHistoryV2.aspx.cs" Inherits="Letters_SentHistoryV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Letters Sent History</asp:Label> &nbsp; <asp:HyperLink ID="lnkToEntity" runat="server"></asp:HyperLink></div>
        <div class="main_content" style="padding:10px 5px;">

            <div class="user_login_form">

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdLetterPrintHistory" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="lph_letter_print_history_id" 
                         OnRowCancelingEdit="GrdLetterPrintHistory_RowCancelingEdit" 
                         OnRowDataBound="GrdLetterPrintHistory_RowDataBound" 
                         OnRowEditing="GrdLetterPrintHistory_RowEditing" 
                         OnRowUpdating="GrdLetterPrintHistory_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdLetterPrintHistory_RowCommand" 
                         OnRowDeleting="GrdLetterPrintHistory_RowDeleting" 
                         OnRowCreated="GrdLetterPrintHistory_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         RowStyle-VerticalAlign="top"
                         CellPadding="2"
                         AllowPaging="True"
                         OnPageIndexChanging="GrdLetterPrintHistory_PageIndexChanging"
                         PageSize="12"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">
                         <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="lph_letter_print_history_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("lph_letter_print_history_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Date"  HeaderStyle-HorizontalAlign="Left" SortExpression="lph_date"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPrintDate" runat="server" Text='<%# Bind("lph_date", "{0:dd-MM-yyyy HH:mm}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Send Method"  HeaderStyle-HorizontalAlign="Left" SortExpression="lph_send_method_descr"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSendMethod" runat="server" Text='<%# Bind("lph_send_method_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Document"  HeaderStyle-HorizontalAlign="Left" SortExpression="letter_docname" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDocument" runat="server" Text='<%# (Eval("letterorg_organisation_id") == DBNull.Value ? "[Default] " : "["+Eval("letterorg_name")+"] ") +  Eval("letter_docname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Organisation"  HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_name" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("organisation_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patient"  HeaderStyle-HorizontalAlign="Left" SortExpression="person_patient_firstname" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPatient" runat="server" Text='<%# Eval("person_patient_firstname") + " " + Eval("person_patient_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Referrer"  HeaderStyle-HorizontalAlign="Left" SortExpression="person_referrer_firstname" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblReferrer" runat="server" Text='<%# Eval("person_referrer_firstname") + " " + Eval("person_referrer_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Staff"  HeaderStyle-HorizontalAlign="Left" SortExpression="person_staff_firstname" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblStaff" runat="server" Text='<%# Eval("person_staff_firstname") + " " + Eval("person_staff_surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 
                
                            <asp:TemplateField HeaderText="Booking"  HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="center"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkBookingSheetForPatient" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" AlternateText="Booking Sheet" ToolTip="Booking Sheet" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Health Card<br/> Action Letter"  HeaderStyle-HorizontalAlign="Left" SortExpression="hcat_descr"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkHealthCardActionLetter"  runat="server" Text='<%# Eval("hcat_descr") %>'  Visible='<%# Eval("hc_health_card_id") != DBNull.Value  %>'
                                                NavigateUrl='<%# String.Format("~/HealthCardDetailV2.aspx?type=view&id={0}&card={1}",Eval("hc_health_card_id"), Eval("hc_organisation_id").ToString() == "-1" ? "medicare" : "dva") %>'></asp:HyperLink>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Letter (DB)"  HeaderStyle-HorizontalAlign="Left" SortExpression="lph_has_doc" ItemStyle-HorizontalAlign="Center"> 
                                <ItemTemplate> 
                                    <asp:Button ID="btnRetrieveDB" runat="server" Text="Retrieve" CommandName="RetrieveLetterDB" CommandArgument='<%# Container.DataItemIndex %>' Visible='<%# Eval("lph_has_doc").ToString()=="1"?true:false %>' />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Letter (File)"  HeaderStyle-HorizontalAlign="Left" SortExpression="lph_has_doc" ItemStyle-HorizontalAlign="Center"> 
                                <ItemTemplate> 
                                    <asp:Button ID="btnRetrieveFlatFile" runat="server" Text="Retrieve" CommandName="RetrieveLetterFlatFile" CommandArgument='<%# Container.DataItemIndex %>' Visible='<%# Eval("lph_has_doc").ToString()=="1"?true:false %>' />
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <%-- 
                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditLetterPrintHistoryValidationSummary"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="AddLetterPrintHistoryValidationGroup"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 
                            --%>

                            <%-- 
                            <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" /> 
                            --%>

                        </Columns> 
                    </asp:GridView>


                </div>
            </center>

        </div>
    </div>


</asp:Content>


