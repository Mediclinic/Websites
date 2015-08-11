<%@ Page Title="Manage Stock" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StockListV2.aspx.cs" Inherits="StockListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function expand_collapse(e_id) {
            var e = document.getElementById(e_id);
            if (e.style.display == "none") {
                e.style.display = "block";
                document.getElementById('hiddenUpdateHistoryShowing').value = "True";
            }
            else {
                e.style.display = "none";
                document.getElementById('hiddenUpdateHistoryShowing').value = "False";
            }
        }


        function notification_info_edited(elem) {

            //elem.style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("txtStockWarningLevelNotificationEmailAddress").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("btnUpdateNotificationInfo").className = ""; // make it visible
            document.getElementById("btnRevertNotificationInfo").className = ""; // make it visible

            update_to_max_height();
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Manage Stock</span></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:950px;">

                <div class="border_top_bottom user_login_form_no_width_div">

                    <table class="padded-table-1px text_left" style="margin:6px auto;">
                        <tr>
                            <td class="nowrap">
                                Stock Warning Level Notification Email:
                            </td>
                            <td style="width:5px;"></td>
                            <td>
                                <asp:TextBox ID="txtStockWarningLevelNotificationEmailAddress" runat="server" width="98%" onkeyup="notification_info_edited();" style="min-width:100%;" />
                            </td>
                            <td style="width:5px;"></td>
                            <td><asp:Button ID="btnUpdateNotificationInfo" runat="server" Text="Update" OnClick="btnUpdateNotificationInfo_Click" />&nbsp;&nbsp;</td>
                            <td><asp:Button ID="btnRevertNotificationInfo" runat="server" Text="Revert" OnClick="btnRevertNotificationInfo_Click" /></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblOrgType" runat="server"></asp:Label>
                            </td>
                            <td></td>
                            <td>
                                <asp:DropDownList ID="ddlOrgs" runat="server" Width="100%" AutoPostBack="True" OnSelectedIndexChanged="ddlOrgs_SelectedIndexChanged" />
                            </td>
                            <td></td>
                            <td colspan="2" class="nowrap"><asp:Label ID="lblHowToAddItems" runat="server" Font-Bold="True" ForeColor="Blue" Text="** To Add Items, First Select A Clinic" /></td>
                        </tr>
                    </table>

                </div>

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit<span style="padding-left:50px;"><img src="imagesV2/x.png" alt="edit icon" style="margin:0 5px 5px 0;" />Delete</span>
            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;display: inline-table !important;" >

                    <asp:GridView ID="GrdRegistration" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="s_stock_id" 
                         OnRowCancelingEdit="GrdRegistration_RowCancelingEdit" 
                         OnRowDataBound="GrdRegistration_RowDataBound" 
                         OnRowEditing="GrdRegistration_RowEditing" 
                         OnRowUpdating="GrdRegistration_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdRegistration_RowCommand" 
                         OnRowDeleting="GrdRegistration_RowDeleting" 
                         OnRowCreated="GrdRegistration_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         AllowPaging="True"
                         OnPageIndexChanging="GrdRegistration_PageIndexChanging"
                         PageSize="14"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                         <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="s_stock_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("s_stock_id") %>'  />
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("s_stock_id") %>' />
                                </EditItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Organisation" HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_name" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("organisation_name") %>' />
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("organisation_name") %>' />
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblNewOrganisation" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Offering" HeaderStyle-HorizontalAlign="Left" SortExpression="o_name" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOffering" runat="server" Text='<%# Eval("o_name") %>' />
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblOffering" runat="server" Text='<%# Eval("o_name") %>' />
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOffering" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Quantity" HeaderStyle-HorizontalAlign="Left" SortExpression="s_qty" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("s_qty") %>' />
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlQuantity_Hundreds" runat="server"></asp:DropDownList>
                                    <asp:DropDownList ID="ddlQuantity_Tens" runat="server"></asp:DropDownList>
                                    <asp:DropDownList ID="ddlQuantity_Zeros" runat="server"></asp:DropDownList>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewQuantity_Hundreds" runat="server"></asp:DropDownList>
                                    <asp:DropDownList ID="ddlNewQuantity_Tens" runat="server"></asp:DropDownList>
                                    <asp:DropDownList ID="ddlNewQuantity_Zeros" runat="server"></asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Warning Amount" HeaderStyle-HorizontalAlign="Left" SortExpression="s_warning_amt" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblWarningAmount" runat="server" Text='<%# Eval("s_warning_amt") %>' />
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlWarningAmount" runat="server"/>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewWarningAmount" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 



                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="AddRegistrationValidationGroup"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="AddRegistrationValidationGroup"></asp:LinkButton> 
                                    <%-- OnClientClick= "javascript:if (!provider_check_submit()) return false;" --%>
                                </FooterTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Delete-icon-24.png" CommandName="Delete" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <%--<asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True" DeleteImageUrl="~/images/Delete-icon-24.png"  />--%>

                        </Columns> 

                    </asp:GridView>

                    <br />

                    <div id="div_history_section" runat="server">
                        <asp:HiddenField ID="hiddenUpdateHistoryShowing" runat="server" />
                        <div style="line-height:2px;">&nbsp;</div>
                        <h4>Stock Update History &nbsp;&nbsp;&nbsp; <a href="javascript:void(0)"  onclick="expand_collapse('div_show_hide_stock_update_history');return false;">expand/collapse</a></h4>
                        <div style="line-height:7px;">&nbsp;</div>
                        <div id="div_show_hide_stock_update_history" runat="server">

                            <asp:GridView ID="GrdUpdateHistory" runat="server" 
                                 AutoGenerateColumns="False" DataKeyNames="sa_stock_update_history_id" 
                                 OnRowCancelingEdit="GrdUpdateHistory_RowCancelingEdit" 
                                 OnRowDataBound="GrdUpdateHistory_RowDataBound" 
                                 OnRowEditing="GrdUpdateHistory_RowEditing" 
                                 OnRowUpdating="GrdUpdateHistory_RowUpdating" ShowFooter="False" 
                                 OnRowCommand="GrdUpdateHistory_RowCommand" 
                                 OnRowDeleting="GrdUpdateHistory_RowDeleting" 
                                 OnRowCreated="GrdUpdateHistory_RowCreated"
                                 AllowSorting="True" 
                                 OnSorting="GridView_Sorting_UpdateHistory"
                                 AllowPaging="True"
                                 OnPageIndexChanging="GrdUpdateHistory_PageIndexChanging"
                                 PageSize="10"
                                 ClientIDMode="Predictable"
                                 CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                                 <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />


                                <Columns> 

                                    <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="sa_stock_update_history_id"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("sa_stock_update_history_id") %>'  />
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Organisation" HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_name" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("organisation_name") %>' />
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Offering" HeaderStyle-HorizontalAlign="Left" SortExpression="o_name" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblOffering" runat="server" Text='<%# Eval("o_name") %>' />
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Quantity Added" HeaderStyle-HorizontalAlign="Left" SortExpression="sa_qty_added" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblQuantityAdded" runat="server" Text='<%# ( Eval("sa_qty_added") != DBNull.Value && ((int)Eval("sa_qty_added")) >= 0 ? "&nbsp;" : "") + Eval("sa_qty_added") %>' />
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Is Created" HeaderStyle-HorizontalAlign="Left" SortExpression="sa_is_created" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblIsCreated" runat="server" Text='<%# Eval("sa_qty_added") == DBNull.Value ? "" : ( ((bool)Eval("sa_is_created")) ? "Yes" : "No" ) %>' />
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Is Deleted" HeaderStyle-HorizontalAlign="Left" SortExpression="sa_is_deleted" FooterStyle-VerticalAlign="Top"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("sa_qty_added") == DBNull.Value ? "" : ( ((bool)Eval("sa_is_deleted")) ? "Yes" : "No" ) %>' />
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Date Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="sa_date_added"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("sa_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                    <asp:TemplateField HeaderText="Added By"  HeaderStyle-HorizontalAlign="Left" SortExpression="person_added_by_firstname"> 
                                        <ItemTemplate> 
                                            <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("person_added_by_firstname") + " " + Eval("person_added_by_surname") %>'></asp:Label>
                                        </ItemTemplate> 
                                    </asp:TemplateField> 

                                </Columns> 

                            </asp:GridView>

                        </div>
                    </div>

                </div>

            </center>

        </div>
    </div>

</asp:Content>



