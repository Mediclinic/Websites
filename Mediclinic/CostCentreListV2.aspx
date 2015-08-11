<%@ Page Title="Cost Centres" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="CostCentreListV2.aspx.cs" Inherits="CostCentreListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Cost Centres</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">
            <div class="user_login_form">

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit

            </div>

            <div class="block_center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdCostCentre" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="costcentre_id" 
                         OnRowCancelingEdit="GrdCostCentre_RowCancelingEdit" 
                         OnRowDataBound="GrdCostCentre_RowDataBound" 
                         OnRowEditing="GrdCostCentre_RowEditing" 
                         OnRowUpdating="GrdCostCentre_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdCostCentre_RowCommand" 
                         OnRowCreated="GrdCostCentre_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="costcentre_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("costcentre_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("costcentre_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="Parent" HeaderStyle-HorizontalAlign="Left" SortExpression="parent_descr"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlParent" runat="server" DataTextField="descr" DataValueField="costcentre_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblParent" runat="server" Text='<%# Eval("parent_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewParent" runat="server" DataTextField="descr" DataValueField="costcentre_id"> </asp:DropDownList> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" ItemStyle-Width="250"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="75%" ID="txtDescr" runat="server" Text='<%# Bind("descr") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateDescrRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtDescr" 
                                        ErrorMessage="Descr is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateDescrRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtDescr"
                                        ValidationExpression="^[a-zA-Z0-9]+$"
                                        ErrorMessage="Descr can only be letters, numbers, and underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="75%" ID="txtNewDescr" runat="server" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewDescrRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDescr" 
                                        ErrorMessage="Descr is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDescrRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDescr"
                                        ValidationExpression="^[a-zA-Z0-9]+$"
                                        ErrorMessage="Descr can only be letters, numbers, and underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummary">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDescr" runat="server" Text='<%# Bind("descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummary"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummary"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-20.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 

                    </asp:GridView>


                </div>
            </center>

        </div>
    </div>


</asp:Content>



