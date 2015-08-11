<%@ Page Title="Orders" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="OfferingOrderListV2.aspx.cs" Inherits="OfferingOrderListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function date_active_changed(obj) {

            if (obj.value.length == 2)
                obj.value += "-";

            if (obj.value.length == 5)
                obj.value += "-";
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Orders</asp:Label> &nbsp; <asp:HyperLink ID="lnkToEntity" runat="server"></asp:HyperLink></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:1000px;margin:0px auto 20px;">

                <div class="border_top_bottom user_login_form_no_width_div">

                    <table class="block_center" style="margin:6px auto;">
                        <tr>

                            <td class="nowrap">
                                <asp:DropDownList ID="ddlOrganisations" runat="server" OnSelectedIndexChanged="ddlOrganisations_SelectedIndexChanged" AutoPostBack="True" style="width:200px;" />
                                <br />
                                <asp:DropDownList ID="ddlStaff" runat="server" OnSelectedIndexChanged="ddlStaff_SelectedIndexChanged" AutoPostBack="True" style="width:200px;" />
                                <br />
                                <asp:DropDownList ID="ddlOfferings" runat="server" OnSelectedIndexChanged="ddlOfferings_SelectedIndexChanged" AutoPostBack="True" style="width:200px;" />
                            </td>
                            <td style="width:22px;"></td>
                            <td>
                                <table>
                                    <tr>
                                        <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">Ordered Start Date: </asp:Label></td>
                                        <td class="nowrap"><asp:TextBox ID="txtStartDate" runat="server" Columns="10"/></td>
                                        <td class="nowrap"><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                        <td class="nowrap"  style="line-height:normal;"><button type="button" onclick="javascript:document.getElementById('txtStartDate').value = '';return false;">Clear</button></td>
                                    </tr>
                                    <tr>
                                        <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">Ordered End Date: </asp:Label></td>
                                        <td class="nowrap"><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                                        <td class="nowrap"><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>
                                        <td class="nowrap"  style="line-height:normal;"><button type="button" onclick="javascript:document.getElementById('txtEndDate').value = '';return false;">Clear</button></td>
                                    </tr>
                                </table>
                            </td>

                            <td style="width:22px"></td>

                            <td class="nowrap" style="text-align:left;">
                                <asp:CheckBox ID="chkOnlyUnfilled" runat="server" Text="&nbsp;Only Unfilled Orders" OnCheckedChanged="chkOnlyUnfilled_CheckedChanged" AutoPostBack="true" /> 
                                <br />
                                <asp:CheckBox ID="chkOnlyFilled" runat="server" Text="&nbsp;Only Filled Orders" OnCheckedChanged="chkOnlyFilled_CheckedChanged" AutoPostBack="true" />
                            </td>

                            <td style="width:22px"></td>

                            <td>
                                <table>
                                    <tr>
                                        <td class="nowrap" style="text-align:center;line-height:normal;">
                                            <asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" Width="100%" style="min-width:75px;" />
                                            <span style="line-height:7px;">&nbsp;</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="nowrap" style="text-align:center;line-height:normal;">
                                            <asp:Button ID="btnExport" runat="server" Text="Export List" OnClick="btnExport_Click" Width="100%" />
                                        </td>
                                    </tr>
                                </table>
                            </td>

                            <td style="width:35px"></td>

                            <td><asp:CheckBox ID="chkUsePaging" runat="server" Text="&nbsp;use paging" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" CssClass="nowrap" /></td>



                        </tr>
                    </table>

                </div>

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit<span style="padding-left:50px;"><img src="imagesV2/x.png" alt="edit icon" style="margin:0 5px 5px 0;" />Delete</span>
            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdOfferingOrder" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="offeringorder_offering_order_id" 
                        OnRowCancelingEdit="GrdOfferingOrder_RowCancelingEdit" 
                        OnRowDataBound="GrdOfferingOrder_RowDataBound" 
                        OnRowEditing="GrdOfferingOrder_RowEditing" 
                        OnRowUpdating="GrdOfferingOrder_RowUpdating" ShowFooter="True" 
                        OnRowCommand="GrdOfferingOrder_RowCommand" 
                        OnRowDeleting="GrdOfferingOrder_RowDeleting" 
                        OnRowCreated="GrdOfferingOrder_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GridView_Sorting"
                        RowStyle-VerticalAlign="top"
                        AllowPaging="True"
                        OnPageIndexChanging="GrdOfferingOrder_PageIndexChanging"
                        PageSize="12"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="offeringorder_offering_order_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("offeringorder_offering_order_id") %>' ></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("offeringorder_offering_order_id") %>' ></asp:Label>
                                </EditItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patient" HeaderStyle-HorizontalAlign="Left" SortExpression="person_patient_surname" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPatient" runat="server" Text='<%# Eval("person_patient_firstname") + " " + Eval("person_patient_surname") %>' ></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblPatient" runat="server" Text='<%# Eval("person_patient_firstname") + " " + Eval("person_patient_surname") %>' ></asp:Label> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Clinic / Facility" HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_name" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("organisation_name") %>' ></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlOrganisation" runat="server" style="max-width:150px;"> </asp:DropDownList>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOrganisation" runat="server" style="max-width:150px;"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Staff" HeaderStyle-HorizontalAlign="Left" SortExpression="person_staff_surname" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblStaff" runat="server" Text='<%# Eval("person_staff_firstname") + " " + Eval("person_staff_surname") %>' ></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlStaff" runat="server" style="max-width:150px;"> </asp:DropDownList>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewStaff" runat="server" style="max-width:150px;"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Item" HeaderStyle-HorizontalAlign="Left" SortExpression="offering_name" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOffering" runat="server" Text='<%# Eval("offering_name") %>' ></asp:Label> &nbsp;&nbsp;
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlOffering" runat="server" style="max-width:150px;"> </asp:DropDownList>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOffering" runat="server" style="max-width:150px;"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Qty" HeaderStyle-HorizontalAlign="Left" SortExpression="offeringorder_quantity" FooterStyle-VerticalAlign="Top" > 
                                <ItemTemplate> 
                                    <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("offeringorder_quantity") %>' ></asp:Label> &nbsp;&nbsp;
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlQuantity" runat="server"> </asp:DropDownList>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewQuantity" runat="server"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="offeringorder_descr" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDescr" runat="server" Text='<%# Eval("offeringorder_descr").ToString().Replace("\r\n", "<br />") %>' ></asp:Label> &nbsp;&nbsp;
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:TextBox ID="txtDescr" runat="server" TextMode="MultiLine" Rows="2" Text='<%# Eval("offeringorder_descr") %>'></asp:TextBox>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox ID="txtNewDescr" runat="server" TextMode="MultiLine" Rows="2" ></asp:TextBox>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Ordered Date" HeaderStyle-HorizontalAlign="Left" SortExpression="offeringorder_date_ordered" FooterStyle-VerticalAlign="Top" FooterStyle-Wrap="false"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrderedDate" runat="server" Text='<%# Bind("offeringorder_date_ordered", "{0:dd-MM-yyyy}") %>' ></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <span style="white-space: nowrap">
                                        <asp:DropDownList ID="ddlOrderedDate_Day" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlOrderedDate_Month" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlOrderedDate_Year" runat="server"></asp:DropDownList>
                                    </span> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <span style="white-space: nowrap">
                                        <asp:DropDownList ID="ddlNewOrderedDate_Day" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlNewOrderedDate_Month" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlNewOrderedDate_Year" runat="server"></asp:DropDownList>
                                    </span> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Filled Date" HeaderStyle-HorizontalAlign="Left" SortExpression="offeringorder_date_filled" FooterStyle-VerticalAlign="Top" FooterStyle-Wrap="false"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFilledDate" runat="server" Text='<%# Bind("offeringorder_date_filled", "{0:dd-MM-yyyy}") %>' ></asp:Label> 
                                    <asp:Button ID="btnSetFilled" runat="server" CommandName="SetFilled" CommandArgument='<%# Eval("offeringorder_offering_order_id") %>' Text="Set Filled" />
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <span style="white-space: nowrap">
                                        <asp:DropDownList ID="ddlFilledDate_Day" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlFilledDate_Month" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlFilledDate_Year" runat="server"></asp:DropDownList>
                                    </span> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <span style="white-space: nowrap">
                                        <asp:DropDownList ID="ddlNewFilledDate_Day" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlNewFilledDate_Month" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlNewFilledDate_Year" runat="server"></asp:DropDownList>
                                    </span> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Cancelled Date" HeaderStyle-HorizontalAlign="Left" SortExpression="offeringorder_date_cancelled" FooterStyle-VerticalAlign="Top" FooterStyle-Wrap="false"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCancelledDate" runat="server" Text='<%# Bind("offeringorder_date_cancelled", "{0:dd-MM-yyyy}") %>' ></asp:Label> 
                                    <asp:Button ID="btnSetCancelled" runat="server" CommandName="SetCancelled" CommandArgument='<%# Eval("offeringorder_offering_order_id") %>' Text="Set Cancelled" />
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <span style="white-space: nowrap">
                                        <asp:DropDownList ID="ddlCancelledDate_Day" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlCancelledDate_Month" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlCancelledDate_Year" runat="server"></asp:DropDownList>
                                    </span> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <span style="white-space: nowrap">
                                        <asp:DropDownList ID="ddlNewCancelledDate_Day" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlNewCancelledDate_Month" runat="server"></asp:DropDownList><asp:DropDownList ID="ddlNewCancelledDate_Year" runat="server"></asp:DropDownList>
                                    </span> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd" OnClientClick="confirm_blank_date();"></asp:LinkButton> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" /> 

                        </Columns> 
                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



