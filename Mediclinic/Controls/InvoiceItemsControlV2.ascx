<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InvoiceItemsControlV2.ascx.cs" Inherits="InvoiceItemsControlV2" %>

    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>

    <asp:HiddenField ID="hiddenField_InvoiceType" runat="server" />
    <asp:HiddenField ID="hiddenItemList" runat="server" />

    <table>

        <tr>
            <td style="text-align:center">

                Search: <asp:TextBox ID="txtSearchOffering" runat="server" placeholder="Enter Offering Name"  onkeyup="live_search(this.value)" autocomplete="off" onkeydown="return (event.keyCode!=13);" ></asp:TextBox>
                <div id="div_livesearch" style="position:absolute;background:#FFFFFF;"></div>
                <button type="button" name="btnClearOfferingSearch" onclick="clear_live_search(); return false;">Clear</button>

                <div style="height:15px;"></div>
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr style="vertical-align:top;">
            <td>

                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdOffering" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="o_offering_id" 
                         OnRowCancelingEdit="GrdOffering_RowCancelingEdit" 
                         OnRowDataBound="GrdOffering_RowDataBound" 
                         OnRowEditing="GrdOffering_RowEditing" 
                         OnRowUpdating="GrdOffering_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdOffering_RowCommand" 
                         OnRowDeleting="GrdOffering_RowDeleting" 
                         OnRowCreated="GrdOffering_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridOffering_Sorting"
                         RowStyle-VerticalAlign="top" 
                         CellPadding="3"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="o_offering_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("o_offering_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="o_name" ItemStyle-CssClass="text_left"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblShortName" runat="server" Text='<%# Bind("o_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Price" HeaderStyle-HorizontalAlign="Left" SortExpression="o_default_price" FooterStyle-VerticalAlign="Top" Visible="false"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDefaultPrice" runat="server" Text='<%# Bind("o_default_price") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Inv" HeaderStyle-HorizontalAlign="Left" SortExpression="hc_paid" Visible="false" > 
                                <ItemTemplate> 
                                    <asp:Label ID="lblHCPaid" runat="server" Font-Bold='<%#  Eval("hc_paid").ToString()=="True"? false : true %>' Text='<%# Eval("hc_paid").ToString()=="True" ? "MC" : "Priv"  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>



                            <asp:TemplateField HeaderText="PT Price" HeaderStyle-HorizontalAlign="Left" SortExpression="pt_price" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPTPrice" runat="server" Text='<%# Eval("pt_price") == DBNull.Value || Convert.ToDecimal(Eval("pt_price")) == 0 ? string.Empty : string.Format("{0:C}", Convert.ToDecimal(Eval("pt_price"))) %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="PT GST" HeaderStyle-HorizontalAlign="Left" SortExpression="pt_gst" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPTGst" runat="server" Text='<%# Eval("pt_price") == DBNull.Value || Eval("pt_gst") == DBNull.Value || Convert.ToDecimal(Eval("pt_price")) == 0 || Convert.ToDecimal(Eval("pt_gst")) == 0 ? string.Empty : string.Format("{0:C}", Convert.ToDecimal(Eval("pt_gst"))) %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="MC/DVA Price" HeaderStyle-HorizontalAlign="Left" SortExpression="hc_price" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMCPrice" runat="server" Text='<%# Eval("hc_price") == DBNull.Value || Convert.ToDecimal(Eval("hc_price")) == 0 ? string.Empty : string.Format("{0:C}", Convert.ToDecimal(Eval("hc_price")))  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="MC/DVA GST" HeaderStyle-HorizontalAlign="Left" SortExpression="hc_gst" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMCGst" runat="server" Text='<%# Eval("hc_price") == DBNull.Value || Eval("hc_gst") == DBNull.Value || Convert.ToDecimal(Eval("hc_price")) == 0 || Convert.ToDecimal(Eval("hc_gst")) == 0 ? string.Empty : string.Format("{0:C}", Convert.ToDecimal(Eval("hc_gst"))) %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 



                            <asp:TemplateField HeaderText=""> 
                                <ItemTemplate> 
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" OnCommand="btnAdd_Command" CommandName="Add" CommandArgument='<%# Bind("o_offering_id") %>'  />
                                </ItemTemplate> 
                            </asp:TemplateField>

                        </Columns> 
                    </asp:GridView>

                </div>
            </td>
            <td id="td_gridview_space" runat="server" valign="top" style="width:40px"></td>
            <td>

                <div id="autodivheight_otherconrol">

                    <asp:GridView ID="GrdSelectedList" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="offering_id" 
                         OnRowCancelingEdit="GrdSelectedList_RowCancelingEdit" 
                         OnRowDataBound="GrdSelectedList_RowDataBound" 
                         OnRowEditing="GrdSelectedList_RowEditing" 
                         OnRowUpdating="GrdSelectedList_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdSelectedList_RowCommand" 
                         OnRowDeleting="GrdSelectedList_RowDeleting" 
                         OnRowCreated="GrdSelectedList_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridSelectedList_Sorting"
                         RowStyle-VerticalAlign="top" 
                         CellPadding="3"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="offering_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("offering_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Inv" HeaderStyle-HorizontalAlign="Left" SortExpression="hc_paid" Visible="false" > 
                                <ItemTemplate> 
                                    <asp:Label ID="lblHCPaid" runat="server" Font-Bold='<%#  Eval("hc_paid").ToString()=="True"? false : true %>' ForeColor='<%#  Eval("hc_paid").ToString()=="True"? System.Drawing.Color.Black : System.Drawing.Color.Red %>' Text='<%# Eval("hc_paid").ToString()=="True" ? "MC" : "Priv"  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="name"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblShortName" runat="server" Text='<%# Eval("name") %>'></asp:Label> 

                                    <div runat="server" visible='<%# Eval("show_area_treated") != DBNull.Value && (bool)Eval("show_area_treated") %>'>
                                        <div style="height:8px;"></div>
                                        Area Treated:<br />
                                        <asp:TextBox ID="txtAreaTreated" runat="server" Text='<%# Eval("area_treated") %>' Columns="13" />
                                    </div>
                                    <div runat="server" visible='<%# Eval("show_service_reference") != DBNull.Value && (bool)Eval("show_service_reference") %>' style="white-space:nowrap;">
                                        <div style="height:8px;"></div>
                                        Service Reference: <asp:TextBox ID="txtServiceReference" runat="server" Text='<%# Eval("service_reference") %>' MaxLength="2" Columns="2" />
                                    </div>

                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Unit Price" HeaderStyle-HorizontalAlign="Left" SortExpression="default_price" FooterStyle-VerticalAlign="Top" Visible="false" > 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDefaultPrice" runat="server" Text='<%# Eval("default_price") == DBNull.Value ? string.Empty : string.Format("{0:C}", Convert.ToDecimal(Eval("default_price")))  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 



                            <asp:TemplateField HeaderText="Unit Price (PT)" HeaderStyle-HorizontalAlign="Left" SortExpression="pt_price" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPTPrice" runat="server" Text='<%# Eval("pt_price") == DBNull.Value || Convert.ToDecimal(Eval("pt_price")) == 0 ? string.Empty : (string.Format("{0:C}", Convert.ToDecimal(Eval("pt_price"))) + (Eval("pt_gst") == DBNull.Value || Convert.ToDecimal(Eval("pt_gst")) == 0 ? string.Empty : " <br />(+GST: "+ string.Format("{0:C}", Convert.ToDecimal(Eval("pt_gst"))) +")")   )  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Unit Price (MC/DVA)" HeaderStyle-HorizontalAlign="Left" SortExpression="hc_price" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMCPrice" runat="server" Text='<%# Eval("hc_price") == DBNull.Value || Convert.ToDecimal(Eval("hc_price")) == 0 ? string.Empty : (string.Format("{0:C}", Convert.ToDecimal(Eval("hc_price"))) + (Eval("hc_gst") == DBNull.Value || Convert.ToDecimal(Eval("hc_gst")) == 0 ? string.Empty : " <br />(+GST: "+ string.Format("{0:C}", Convert.ToDecimal(Eval("hc_gst"))) +")")  ) %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>



                            <asp:TemplateField HeaderText="Qty" HeaderStyle-HorizontalAlign="Left" SortExpression="quantity" FooterStyle-VerticalAlign="Top">
                                <ItemTemplate> 
                                    <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("quantity") %>'></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="40" ID="txtQuantity" runat="server" Text='<%# Eval("quantity") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateMedicareChargeRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtQuantity" 
                                        ErrorMessage="Quantity is required."
                                        Display="Dynamic"
                                        ValidationGroup="EditOfferingValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtQuantityRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtQuantity"
                                        ValidationExpression="^\d+$"
                                        ErrorMessage="Quantity must be only digits."
                                        Display="Dynamic"
                                        ValidationGroup="EditOfferingValidationSummary">*</asp:RegularExpressionValidator>
                                    <asp:CompareValidator ID="txtQuantityNonZero" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtQuantity"
                                        ValueToCompare="0"
                                        Operator="NotEqual"
                                        ErrorMessage="Quantity must not be zero."
                                        Display="Dynamic"
                                        ValidationGroup="EditOfferingValidationSummary">*</asp:CompareValidator>
                                </EditItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Total" HeaderStyle-HorizontalAlign="Left" SortExpression="total_line_price" FooterStyle-VerticalAlign="Top" Visible="false" > 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTotalLinePrice" runat="server" Text='<%# Eval("total_line_price") == DBNull.Value ? string.Empty : string.Format("{0:C}", Convert.ToDecimal(Eval("total_line_price")))  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 



                            <asp:TemplateField HeaderText="Total (PT)" HeaderStyle-HorizontalAlign="Left" SortExpression="total_pt_price" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPTPriceTotal" runat="server" Text='<%# Eval("total_pt_price") == DBNull.Value || Convert.ToDecimal(Eval("total_pt_price")) == 0 ? string.Empty : string.Format("{0:C}", Convert.ToDecimal(Eval("total_pt_price")) + Convert.ToDecimal(Eval("total_pt_gst"))) %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Total (MC/DVA)" HeaderStyle-HorizontalAlign="Left" SortExpression="total_hc_price" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblHCPriceTotal" runat="server" Text='<%# Eval("total_hc_price") == DBNull.Value || Convert.ToDecimal(Eval("total_hc_price")) == 0 ? string.Empty : string.Format("{0:C}", Convert.ToDecimal(Eval("total_hc_price")) + Convert.ToDecimal(Eval("total_hc_gst"))) %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="On Order" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:CheckBox ID="chkOnOrder" runat="server" checked='<%# Eval("on_order") == DBNull.Value ? false : Eval("on_order") %>' OnCheckedChanged="chkOnOrder_CheckedChanged" AutoPostBack="true" />
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="" Visible="True" HeaderStyle-BorderStyle="None"  ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditOfferingValidationSummary"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit Qty" style="white-space:nowrap;"></asp:LinkButton> 
                                    <div style="white-space:nowrap;">
                                        <asp:Button ID="btnAddOne" runat="server" style="padding:1px 2px;width:25px;" CommandName="AddOne" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>'   Text="+1" /><asp:Button ID="btnSubtractOne" runat="server" style="padding:1px 2px;width:25px;" CommandName="SubtractOne" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' Text="-1" />
                                    </div>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:CommandField HeaderText="" ShowDeleteButton="True" HeaderStyle-BorderStyle="None" ShowHeader="False" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" />

                        </Columns> 

                    </asp:GridView>

                    <br />
                
                    <span id="spnTotal" runat="server">

                        <br />
                        <table>
                            <tr id="tr_hc_row" runat="server">
                                <td  style="width:40px"></td>
                                <td><asp:Label ID="lblHcTotalText" runat="server" Font-Bold="True" Text='Medicare Total: ' /></td>
                                <td style="width:15px"></td>
                                <td><asp:Label ID="lblHcTotalPrice" runat="server" Font-Bold="True" Text='0.00' /></td>                            
                            </tr>
                            <tr id="tr_nonhc_row" runat="server">
                                <td  style="width:40px"></td>
                                <td><asp:Label ID="lblNonHcTotalText" runat="server" Font-Bold="True" Text='PT Payable Total: ' /></td>
                                <td style="width:15px"></td>
                                <td><asp:Label ID="lblNonHcTotalPrice" runat="server" Font-Bold="True" Text='0.00' /></td>
                            </tr>
                            <tr id="tr_hc_space_row" runat="server" height="10">
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td  style="width:40px"></td>
                                <td><asp:Label ID="lblTotalText" runat="server" Font-Bold="True" Text='Total: ' /></td>
                                <td></td>
                                <td><asp:Label ID="lblTotalPrice" runat="server" Font-Bold="True" Text='0.00' /></td>                            
                            </tr>
                        </table>

                    </span>

                    <br />

                    <span id="spnCreatePrivateInvoice" runat="server">
                        <br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="lnkCreatePrivateInvoice" runat="server" 
                        onclick="lnkCreatePrivateInvoice_Click">Set as private invoice</asp:LinkButton>
                        <br />
                        <br />
                    </span>
                    <span id="spnButtons" runat="server">
                        <br />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnSubmit" runat="server" Text = "Complete" OnClick="btnSubmit_Click"  OnClientClick="show_page_load_message();" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text = "Cancel" OnClientClick="javascript:window.returnValue=false;window.close()" />
                    </span>
                    <span id="spnErrorClosePageButtons" runat="server">
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnClose" runat="server" Text = "Close Window" OnClientClick="javascript:window.returnValue=false;window.close()" />
                    </span>

                </div>

            </td>
        </tr>
    </table>