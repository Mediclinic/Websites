<%@ Page Title="Treatment Report" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Invoice_ACTreatmentListV2.aspx.cs" Inherits="Invoice_ACTreatmentListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Treatment Report</asp:Label></div>
        <div class="main_content_with_header">

            <div class="user_login_form">

                <div>
                    <center>

                        <table>
                            <tr>
                                <td>

                                    <table>
                                        <tr>
                                            <td><asp:Label ID="lblOrgType" runat="server"></asp:Label></td>
                                            <td></td>
                                            <td><asp:Label ID="lblOrgName" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr id="tr_address" runat="server">
                                            <td style="vertical-align:top;">Address</td>
                                            <td></td>
                                            <td><asp:Label ID="lblOrgAddress" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td>Booking Date</td>
                                            <td style="min-width:15px;"></td>
                                            <td><asp:Label ID="lblTreatmentDate" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td>Provider</td>
                                            <td></td>
                                            <td><asp:Label ID="lblProviderName" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td>Provider Number</td>
                                            <td></td>
                                            <td><asp:Label ID="lblProviderNbr" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td><asp:Label ID="lblInvLink" runat="server"></asp:Label></td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                    </table>

                                </td>
                                <td style="min-width:50px;"></td>
                                <td style="vertical-align:middle;">

                                    <table>
                                        <tr>
                                            <td>
                                                <asp:CheckBox id="chkIncAddOns" runat="server" AutoPostBack="true" OnCheckedChanged="chkIncAddOns_CheckedChanged" Text="&nbsp;Inc PT Add Ons" Visible="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnExport" runat="server" OnClick="btnExport_Click" Text="Print" style="width:100%" />
                                                <br id="br_before_email_to_fac" runat="server" />
                                                <asp:Button ID="btnEmailToFac" runat="server" OnClick="btnEmailToFac_Click" Text="Email To Facility" style="width:100%" />
                                            </td>
                                        </tr>
                                    </table>

                                </td>
                            </tr>
                        </table>



                    </center>
                </div>

            </div>




            <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>

                <div style="height:10px;"></div>

                <div id="autodivheight" class="divautoheight" style="height:500px;width:auto;padding-right:3px;">

                    <asp:GridView ID="GrdBooking" runat="server" 
                            AutoGenerateColumns="False" DataKeyNames="InvoiceLineID" 
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

                            <asp:TemplateField> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblRowNbr" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Room"  HeaderStyle-HorizontalAlign="Left" SortExpression="PaddedRoom" ItemStyle-Wrap="false"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblRoom" runat="server" Text='<%# Eval("Room") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Resident"  HeaderStyle-HorizontalAlign="Left" SortExpression="PatientName" ItemStyle-Wrap="false" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblResident" runat="server" Text='<%# Eval("PatientName") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Resident Type"  HeaderStyle-HorizontalAlign="Left" SortExpression="ItemDescr" ItemStyle-Wrap="false"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblACType" runat="server" Text='<%# Eval("ItemDescr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Debtor"  HeaderStyle-HorizontalAlign="Left" SortExpression="Debtor" ItemStyle-Wrap="false"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDebtor" runat="server" Text='<%# Eval("Debtor") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 
                    </asp:GridView>

                </div>

            </center>

        </div>
    </div>

</asp:Content>



