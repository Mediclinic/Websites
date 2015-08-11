<%@ Page Title="Contact Types" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ContactTypeListV2.aspx.cs" Inherits="ContactTypeListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Contact Types</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">
            <div class="user_login_form" style="width: 300px;">

                <div class="border_top_bottom">
                    <center>

                        <input id="chkShowDateWarning" type="checkbox" value="Accept Form" name="chkShowDateWarning" runat="server" checked="checked" /> &nbsp;<label for="chkShowDateWarning" style="font-weight:normal;">Show "<i>no date entered</i>" warning</label>

                    </center>
                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>

                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdContactType" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="at_contact_type_id" 
                         OnRowCancelingEdit="GrdContactType_RowCancelingEdit" 
                         OnRowDataBound="GrdContactType_RowDataBound" 
                         OnRowEditing="GrdContactType_RowEditing" 
                         OnRowUpdating="GrdContactType_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdContactType_RowCommand" 
                         OnRowDeleting="GrdContactType_RowDeleting" 
                         OnRowCreated="GrdContactType_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GrdContactType_Sorting"
                         RowStyle-VerticalAlign="top"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="at_contact_type_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblContactTypeId" runat="server" Text='<%# Bind("at_contact_type_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblContactTypeId" runat="server" Text='<%# Bind("at_contact_type_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>
                
                            <asp:TemplateField HeaderText="Desc" HeaderStyle-HorizontalAlign="Left" SortExpression="at_descr" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtDesc" runat="server" Text='<%# Bind("at_descr") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateDescRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtDesc" 
                                        ErrorMessage="Desc is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtDesc"
                                        ValidationExpression="^[a-zA-Z\-\s]+$"
                                        ErrorMessage="Desc can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewDesc" runat="server" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewDescRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDesc" 
                                        ErrorMessage="Desc is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDescRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDesc"
                                        ValidationExpression="^[a-zA-Z\-\s]+$"
                                        ErrorMessage="Desc can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblName" runat="server" Text='<%# Bind("at_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="at_contact_type_group_id" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlContactTypeGroup" runat="server" DataTextField="descr" DataValueField="contact_type_group_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblContactTypeGroup" runat="server" Text='<%# Eval("atg_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewContactTypeGroup" runat="server" DataTextField="descr" DataValueField="contact_type_group_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Display Order" HeaderStyle-HorizontalAlign="Left" SortExpression="at_display_order" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlDisplayOrder" runat="server"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDisplayOrder" runat="server" Text='<%# Eval("at_display_order") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewDisplayOrder" runat="server"> </asp:DropDownList>
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
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <%--
                            <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" /> 
                            --%>

                        </Columns> 
                    </asp:GridView>

                </div>

                <br />
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="window.returnValue=false;self.close();" />

            </center>

        </div>
    </div>


</asp:Content>



