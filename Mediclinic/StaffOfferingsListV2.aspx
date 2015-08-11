<%@ Page Title="Staff Commission" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StaffOfferingsListV2.aspx.cs" Inherits="StaffOfferingsListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function date_active_changed(obj) {

            if (obj.value.length == 2)
                obj.value += "-";

            if (obj.value.length == 5)
                obj.value += "-"; //  + String((new Date()).getFullYear()).substr(0, 2);
        }

        function confirm_blank_date() {
            var obj = document.getElementById('MainContent_GrdStaffOfferings_txtNewActiveDate')
            if (document.getElementById('chkShowDateWarning').checked && obj.value.length == 0)
                alert('Blank active date means that this will not be active in the system.');
        }

        function bulk_update() {
            window.showModalDialog('StaffOfferingsBulkUpdateV2.aspx', '', 'dialogHide:yes;dialogWidth:1350px;dialogHeight:370px;center:yes;resizable:no; scroll:no');
            window.location.href = window.location.href;  // reload page
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Staff Commission</asp:Label> &nbsp; <asp:HyperLink ID="lnkToEntity" runat="server"></asp:HyperLink></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:1000px;margin:0px auto 20px;">

                <div class="border_top_bottom user_login_form_no_width_div">

                    <table class="block_center" style="margin:6px auto;">
                        <tr>

                            <td class="nowrap">
                                Providers <asp:DropDownList ID="ddlStaff" runat="server" OnSelectedIndexChanged="ddlStaff_SelectedIndexChanged" AutoPostBack="True" />
                                &nbsp;&nbsp;&nbsp;
                                Offerings <asp:DropDownList ID="ddlOfferings" runat="server" OnSelectedIndexChanged="ddlOfferings_SelectedIndexChanged" AutoPostBack="True" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:LinkButton ID="btnUpdateBulk" Text="Bulk Update" runat="server" OnClientClick="bulk_update(); return false;" />
                            </td>

                            <td style="width:50px" class="hiddencol"></td>
                            <td style="text-align:right" class="hiddencol">
                                <input id="chkShowDateWarning" type="checkbox" value="Accept Form" name="chkShowDateWarning" runat="server" checked="checked" />Show "<i>no date entered</i>" warning
                            </td>

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

                    <asp:GridView ID="GrdStaffOfferings" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="so_staff_offering_id" 
                        OnRowCancelingEdit="GrdStaffOfferings_RowCancelingEdit" 
                        OnRowDataBound="GrdStaffOfferings_RowDataBound" 
                        OnRowEditing="GrdStaffOfferings_RowEditing" 
                        OnRowUpdating="GrdStaffOfferings_RowUpdating" ShowFooter="False" 
                        OnRowCommand="GrdStaffOfferings_RowCommand" 
                        OnRowDeleting="GrdStaffOfferings_RowDeleting" 
                        OnRowCreated="GrdStaffOfferings_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GridView_Sorting"
                        RowStyle-VerticalAlign="top"
                        AllowPaging="True"
                        OnPageIndexChanging="GrdStaffOfferings_PageIndexChanging"
                        PageSize="17"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="so_staff_offering_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("so_staff_offering_id") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("so_staff_offering_id") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Staff" HeaderStyle-HorizontalAlign="Left" SortExpression="person_surname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblStaff" runat="server" Text='<%# Eval("person_firstname") + " " + Eval("person_surname") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblStaff" runat="server" Text='<%# Eval("person_firstname") + " " + Eval("person_surname") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewStaff" runat="server"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Offering" HeaderStyle-HorizontalAlign="Left" SortExpression="o_name" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblOffering" runat="server" Text='<%# Eval("o_name") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> &nbsp;&nbsp;
                                    <asp:Label ID="lblOffering_MessageIsOld" runat="server" Text="(Inactive)"  Visible='<%# Eval("is_active") != DBNull.Value && !((bool)Eval("is_active")) %>'></asp:Label> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOffering" runat="server" Text='<%# Eval("o_name") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> &nbsp;&nbsp;
                                    <asp:Label ID="lblOffering_MessageIsOld" runat="server" Text="(Inactive)"  Visible='<%# Eval("is_active") != DBNull.Value && !((bool)Eval("is_active")) %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOffering" runat="server"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Commission" SortExpression="so_is_commission" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsCommission" runat="server" Checked='<%#Eval("so_is_commission").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsCommission" runat="server" Text='<%# Eval("so_is_commission").ToString()=="True"?"Yes":"No" %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsCommission" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Comm_%" HeaderStyle-HorizontalAlign="Left" SortExpression="so_commission_percent" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtCommissionPercent" runat="server" Text='<%# Bind("so_commission_percent") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateCommissionPercentRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtCommissionPercent" 
                                        ErrorMessage="Commission percent is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtValidateCommissionPercentRange" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtCommissionPercent"
                                        ErrorMessage="Commission percent must be a number and must be between 0 and 100" Type="Double" MinimumValue="0.00" MaximumValue="100.00" 
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RangeValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewCommissionPercent" runat="server" Text='0.00'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateNewCommissionPercentRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewCommissionPercent" 
                                        ErrorMessage="Commission percent is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtValidateNewCommissionPercentRange" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewCommissionPercent"
                                        ErrorMessage="Commission percent must be a number and must be between 0 and 100" Type="Double" MinimumValue="0.00" MaximumValue="100.00" 
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RangeValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCommissionPercent" runat="server" Text='<%# Bind("so_commission_percent") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Fixed Rate" SortExpression="so_is_fixed_rate" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsFixedRate" runat="server" Checked='<%#Eval("so_is_fixed_rate").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsFixedRate" runat="server" Text='<%# Eval("so_is_fixed_rate").ToString()=="True"?"Yes":"No" %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsFixedRate" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Rate" HeaderStyle-HorizontalAlign="Left" SortExpression="so_fixed_rate" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtFixedRate" runat="server" Text='<%# Bind("so_fixed_rate") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateFixedRateRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtFixedRate" 
                                        ErrorMessage="Fixed rate is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateFixedRateRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtFixedRate"
                                        ValidationExpression="^\d+(\.\d{1,2})?$"
                                        ErrorMessage="Fixed rate can only be numbers and option decimal place with 1 or 2 digits following."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewFixedRate" runat="server" Text='0.00'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateNewFixedRateRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewFixedRate" 
                                        ErrorMessage="Fixed rate is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewFixedRateRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewFixedRate"
                                        ValidationExpression="^\d+(\.\d{1,2})?$"
                                        ErrorMessage="Fixed rate can only be numbers and option decimal place with 1 or 2 digits following."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFixedRate" runat="server" Text='<%# Bind("so_fixed_rate") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Active Date" HeaderStyle-HorizontalAlign="Left" SortExpression="so_date_active" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtActiveDate" runat="server" Text='<%# Bind("so_date_active", "{0:dd-MM-yyyy}") %>' onkeyup="javascript:date_active_changed(this);"></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateActiveDateRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtActiveDate"
                                        ValidationExpression="^\d{2}\-\d{2}\-\d{4}$"
                                        ErrorMessage="Active Date must either be empty (indicating it is inactive) or the format must be dd-mm-yyyy"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="txtValidateActiveDate" runat="server"  CssClass="failureNotification"  
                                        ControlToValidate="txtActiveDate"
                                        OnServerValidate="ValidDateCheck"
                                        ErrorMessage="Invalid Active Date"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:CustomValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewActiveDate" runat="server" onkeyup="javascript:date_active_changed(this);"></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateNewActiveDateRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewActiveDate"
                                        ValidationExpression="^\d{2}\-\d{2}\-\d{4}$"
                                        ErrorMessage="Active Date must either be empty (indicating it is inactive) or the format must be dd-mm-yyyy"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="txtValidateNewActiveDate" runat="server"  CssClass="failureNotification"  
                                        ControlToValidate="txtNewActiveDate"
                                        OnServerValidate="ValidDateCheck"
                                        ErrorMessage="Invalid Active Date"
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:CustomValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblActiveDate" runat="server" Text='<%# Bind("so_date_active", "{0:dd-MM-yyyy}") %>' ForeColor='<%# Eval("is_active") == DBNull.Value || (bool)Eval("is_active") ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 



                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd" OnClientClick="confirm_blank_date();"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" Visible="false" /> 

                        </Columns> 
                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



