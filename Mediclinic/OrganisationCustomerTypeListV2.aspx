<%@ Page Title="Clinic/Facility Customer Types" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="OrganisationCustomerTypeListV2.aspx.cs" Inherits="OrganisationCustomerTypeListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1); });
            //txtbox.value = txtbox.value.charAt(0).toUpperCase() + txtbox.value.slice(1);
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label runat="server" ID="lblHeading">Clinic/Facility Customer Types</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

            <br />
            <br />

            <div class="block_center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdOrganisationCustomerType" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="organisation_customer_type_id" 
                         OnRowCancelingEdit="GrdOrganisationCustomerType_RowCancelingEdit" 
                         OnRowDataBound="GrdOrganisationCustomerType_RowDataBound" 
                         OnRowEditing="GrdOrganisationCustomerType_RowEditing" 
                         OnRowUpdating="GrdOrganisationCustomerType_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdOrganisationCustomerType_RowCommand" 
                         OnRowCreated="GrdOrganisationCustomerType_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_customer_type_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("organisation_customer_type_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("organisation_customer_type_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" ItemStyle-Width="250"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="75%" ID="txtDescr" runat="server" Text='<%# Bind("descr") %>' onkeyup="capitalize_first(this);"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateDescrRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtDescr" 
                                        ErrorMessage="Descr is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateDescrRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtDescr"
                                        ValidationExpression="^[a-zA-Z0-9\s\-_\(\)\[\[\*]+$"
                                        ErrorMessage="Descr can only be letters, numbers, underscore, hpyhen, brackets or spaces."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="75%" ID="txtNewDescr" runat="server" onkeyup="capitalize_first(this);" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewDescrRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDescr" 
                                        ErrorMessage="Descr is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDescrRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDescr"
                                        ValidationExpression="^[a-zA-Z0-9\s\-_\(\)\[\[\*]+$"
                                        ErrorMessage="Descr can only be letters, numbers, underscore, hpyhen, brackets or spaces."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDescr" runat="server" Text='<%# Bind("descr") %>'></asp:Label> 
                                </ItemTemplate> 
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

                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>


</asp:Content>



