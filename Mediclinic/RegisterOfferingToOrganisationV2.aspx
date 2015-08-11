<%@ Page Title="Specific Prices Per Clinic" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="RegisterOfferingToOrganisationV2.aspx.cs" Inherits="RegisterOfferingToOrganisationV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Specific Prices Per Clinic</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">
                    <center>

                        <table class="padded-table-2px">
                            <tr>
                                <td>
                                    <asp:Label ID="lblOrgType" runat="server"></asp:Label>
                                </td>
                                <td style="width:10px;"></td>
                                <td>
                                    <asp:DropDownList ID="ddlOrgs" runat="server" Width="100%" AutoPostBack="True" OnSelectedIndexChanged="ddlOrgs_SelectedIndexChanged" />
                                </td>
                                <td style="width:10px;"></td>
                                <td class="nowrap"><asp:Label ID="lblHowToAddItems" runat="server" Font-Bold="True" Text="** To Add Items, First Select A Clinic" /></td>
                            </tr>
                        </table>

                    </center>
                </div>

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit<span style="padding-left:50px;"><img src="imagesV2/x.png" alt="edit icon" style="margin:0 5px 5px 0;" />Delete</span>
            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;width:auto;padding-right:17px;">

                    <asp:GridView ID="GrdRegistration" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="oo_organisation_offering_id" 
                        OnRowCancelingEdit="GrdRegistration_RowCancelingEdit" 
                        OnRowDataBound="GrdRegistration_RowDataBound" 
                        OnRowEditing="GrdRegistration_RowEditing" 
                        OnRowUpdating="GrdRegistration_RowUpdating" ShowFooter="True" 
                        OnRowCommand="GrdRegistration_RowCommand" 
                        OnRowDeleting="GrdRegistration_RowDeleting" 
                        OnRowCreated="GrdRegistration_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GridView_Sorting"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="oo_organisation_offering_id" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("oo_organisation_offering_id") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>' ></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("oo_organisation_offering_id") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label>
                                </EditItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Organisation" HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_name" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("organisation_name") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>' />
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("organisation_name") %>'  ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'/>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOrganisation" runat="server"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Product/Service" HeaderStyle-HorizontalAlign="Left" SortExpression="o_name" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOffering" runat="server" Text='<%# Eval("o_name") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> &nbsp;&nbsp;
                                    <asp:Label ID="lblOffering_MessageIsOld" runat="server" Text="(Inactive)"  Visible='<%# Eval("is_active") != DBNull.Value && !((bool)Eval("is_active")) %>'></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblOffering" runat="server" Text='<%# Eval("o_name") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> &nbsp;&nbsp;
                                    <asp:Label ID="lblOffering_MessageIsOld" runat="server" Text="(Inactive)"  Visible='<%# Eval("is_active") != DBNull.Value && !((bool)Eval("is_active")) %>'></asp:Label> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOffering" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Price" HeaderStyle-HorizontalAlign="Left" SortExpression="oo_price" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPrice" runat="server" Text='<%# Bind("oo_price") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:TextBox Columns="10" ID="txtPrice" runat="server" Text='<%# Bind("oo_price") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidatePriceRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtPrice" 
                                        ErrorMessage="Price is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidatePriceRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtPrice"
                                        ValidationExpression="^\d+(\.\d{1,2})?$"
                                        ErrorMessage="Price can only be numbers and option decimal place with 1 or 2 digits following."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Columns="10" ID="txtNewPrice" runat="server" Text='0.00'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateNewPriceRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewPrice" 
                                        ErrorMessage="Price is required."
                                        Display="Dynamic"
                                        ValidationGroup="AddStaffOfferingsValidationGroup">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewPriceRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewPrice"
                                        ValidationExpression="^\d+(\.\d{1,2})?$"
                                        ErrorMessage="Price can only be numbers and option decimal place with 1 or 2 digits following."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Date Active" HeaderStyle-HorizontalAlign="Left" SortExpression="oo_date_active" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDateActive" runat="server" Text='<%# Bind("oo_date_active", "{0:dd-MM-yyyy}") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:TextBox Columns="10" ID="txtDateActive" runat="server" Text='<%# Bind("oo_date_active", "{0:dd-MM-yyyy}") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateRequiredDateActive" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtDateActive" 
                                        ErrorMessage="Date Active is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateDateActiveRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtDateActive"
                                        ValidationExpression="^\d{2}\-\d{2}\-\d{4}$"
                                        ErrorMessage="'Date Active' format must be dd-mm-yyyy"
                                        Display="Dynamic"
                                        ValidationGroup="EditStaffValidationSummary">*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="txtValidateDateActive" runat="server"  CssClass="failureNotification"  
                                        ControlToValidate="txtDateActive"
                                        OnServerValidate="ValidDateCheck"
                                        ErrorMessage="Invalid 'Date Active'"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:CustomValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Columns="10" ID="txtNewDateActive" runat="server" Text='<%# DateTime.Today.ToString("dd-MM-yyyy") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateRequiredDateActive" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDateActive" 
                                        ErrorMessage="Date Active is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDateActiveRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDateActive"
                                        ValidationExpression="^\d{2}\-\d{2}\-\d{4}$"
                                        ErrorMessage="'Date Active' format must be dd-mm-yyyy"
                                        Display="Dynamic"
                                        ValidationGroup="AddStaffValidationGroup">*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="txtValidateNewDateActive" runat="server"  CssClass="failureNotification"  
                                        ControlToValidate="txtNewDateActive"
                                        OnServerValidate="ValidDateCheck"
                                        ErrorMessage="Invalid 'Date Active'"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:CustomValidator>
                                </FooterTemplate> 
                            </asp:TemplateField> 



                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryAdd"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd"></asp:LinkButton> 
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


                </div>
            </center>
            

        </div>
    </div>

</asp:Content>



