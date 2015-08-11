<%@ Page Title="Cost Centres" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ConditionListV2.aspx.cs" Inherits="ConditionListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Patient Conditions</asp:Label></div>
        <center class="main_content" style="padding:20px 5px;">
            <div class="user_login_form">

                <asp:CheckBox ID="chkShowDeleted" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowDeleted_Submit" Text="" Font-Bold="false" />&nbsp;<label for="chkShowDeleted" style="font-weight:normal;">show deleted</label>

                <table class="block_center padded-table-2px">
                    <tr style="height:7px;"><td></td></tr>
                    <tr style="vertical-align:top">
                        <td>
                            <asp:Button ID="lbAddOneDisplayOrderToAll" runat="server" OnClick="lbAddOneDisplayOrderToAll_Click" Text="+1 All " />
                            <asp:Button ID="lbSubtractOneDisplayOrderToAll" runat="server" OnClick="lbSubtractOneDisplayOrderToAll_Click" Text="-1 All" />
                        </td>
                    </tr>
                    <tr style="height:13px;"><td></td></tr>
                </table>

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit
                <span style="padding-left:20px;"><img src="imagesV2/x.png" alt="delete icon" style="margin:0 5px 5px 0;" />Delete</span>

            </div>

            <center>
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </center>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdCondition" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="condition_condition_id" 
                         OnRowCancelingEdit="GrdCondition_RowCancelingEdit" 
                         OnRowDataBound="GrdCondition_RowDataBound" 
                         OnRowEditing="GrdCondition_RowEditing" 
                         OnRowUpdating="GrdCondition_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdCondition_RowCommand" 
                         OnRowCreated="GrdCondition_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="condition_condition_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("condition_condition_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("condition_condition_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_descr" ItemStyle-Width="250"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="75%" ID="txtDescr" runat="server" Text='<%# Eval("condition_descr") %>' MaxLength="250"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateDescrRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtDescr" 
                                        ErrorMessage="Descr is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="75%" ID="txtNewDescr" runat="server" MaxLength="250"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewDescrRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDescr" 
                                        ErrorMessage="Descr is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDescr" runat="server" Text='<%# Eval("condition_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Show Date" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_show_date" ItemStyle-Width="250"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkShowDate" Checked='<%# (bool)Eval("condition_show_date") %>' runat="server" />
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:CheckBox ID="chkNewShowDate" runat="server" />
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblShowDate" runat="server" Text='<%# Eval("condition_show_date") != DBNull.Value && (bool)Eval("condition_show_date") ? "Yes" : "No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Show Nbr Weeks Due" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_show_nweeksdue" ItemStyle-Width="250"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkShowNWeeksDue" Checked='<%# (bool)Eval("condition_show_nweeksdue") %>' runat="server" />
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:CheckBox ID="chkNewShowNWeeksDue" runat="server" />
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblShowNWeeksDue" runat="server" Text='<%# Eval("condition_show_nweeksdue") != DBNull.Value && (bool)Eval("condition_show_nweeksdue") ? "Yes" : "No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Show Text" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_show_text" ItemStyle-Width="250"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkShowText" Checked='<%# (bool)Eval("condition_show_text") %>' runat="server" />
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:CheckBox ID="chkNewShowText" runat="server" />
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblShowText" runat="server" Text='<%# Eval("condition_show_text") != DBNull.Value && (bool)Eval("condition_show_text") ? "Yes" : "No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Display Order" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_display_order" ItemStyle-Width="250"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlDisplayOrder" runat="server"/>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlNewDisplayOrder" runat="server"/>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDisplayOrder" runat="server" Text='<%# Bind("condition_display_order") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Deleted" SortExpression="condition_is_deleted" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("condition_is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("condition_is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-20.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="True" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" runat="server"  CommandName="_Delete" CommandArgument='<%# Bind("condition_condition_id") %>' ImageUrl="~/images/Delete-icon-24.png" AlternateText="Delete" ToolTip="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>


</asp:Content>



