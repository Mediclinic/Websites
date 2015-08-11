<%@ Page Title="Products & Services" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="OfferingListV2.aspx.cs" Inherits="OfferingListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="ScriptsV2/autodivheight.js" ></script>
    <script type="text/javascript" src="ScriptsV2/jscolor.js"></script>
    <script type="text/javascript">

        function set_if_empty_price(txtMedicareCharge, txtDvaCharge, txtTacCharge) {

            if (txtMedicareCharge.value.trim() == '')
                txtMedicareCharge.value = "0.00";
            if (txtDvaCharge.value.trim() == '')
                txtDvaCharge.value = "0.00";
            if (txtTacCharge.value.trim() == '')
                txtTacCharge.value = "0.00";
        }

        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }

        function validate_and_confirm(message, validation_group) {
            var validated = Page_ClientValidate(validation_group);
            if (validated) {
                return confirm(message);
            }
        }

        function ensure_ishex(txtbox)
        {
            txtbox.value = txtbox.value.toUpperCase();

            /*
            var new_value = "";
            for (var i = 0, len = txtbox.value.length; i < len; i++)
                if (is_hex(txtbox.value.charAt(i))) new_value += txtbox.value.charAt(i);

            if (txtbox.value != new_value)
                alert("Please ensure character is a valid hexadecimal (0-9 or A-F)");

            txtbox.value = new_value;
            */
        }
        function is_hex(c){
            validChar='012345678ABCDEF';   // legal chars
            return validChar.indexOf(c) >= 0;
        }

        function show_hide(id) {

            obj = document.getElementById(id);
            obj.style.display = (obj.style.display == "none") ? "" : "none";

            update_to_max_height(); // udpate autodivheight
        }


    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Products & Services</asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:1000px;"> 


                <a href="javascript:void(0)"  onclick="show_hide('search_div');return false;">Show / Hide Search & Settings</a>

                <div id="search_div" class="border_top_bottom user_login_form_no_width_div" style="display:none;margin-bottom:0px !important;margin-top:10px !important;">
                    <center>

                        <table class="padded-table-2px" style="margin:3px auto;">
                            <tr>
                                <td rowspan="3"><asp:Label ID="lblSearch" runat="server">Search By Name: </asp:Label></td>
                                <td rowspan="3"><asp:TextBox ID="txtSearchName" runat="server"></asp:TextBox></td>
                                <td rowspan="3"><asp:CheckBox ID="chkSearchOnlyStartWith" runat="server" Text="&nbsp;starts with" /></td>
                                <td rowspan="3"><asp:Button ID="btnSearchName" runat="server" Text="Search" onclick="btnSearchName_Click" /></td>
                                <td rowspan="3"><asp:Button ID="btnClearNameSearch" runat="server" Text="Clear" onclick="btnClearNameSearch_Click" /></td>
                                <td rowspan="3" style="width:45px"></td>
                                <td rowspan="3">
                                    <asp:CheckBox ID="chkShowDeleted" runat="server" Text="&nbsp;show deleted" AutoPostBack="True" OnCheckedChanged="chkShowDeleted_CheckedChanged" Checked="False" />
                                    <br />
                                    <asp:CheckBox ID="chkUsePaging" runat="server" Text="&nbsp;use paging" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" Checked="True" />
                                </td>
                                <td rowspan="3" style="width:45px"></td>
                                <td>
                                    <asp:TextBox ID="txtNewPrice" runat="server" Width="50px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtNewPriceRequired" runat="server" CssClass="failureNotification"  
                                            ControlToValidate="txtNewPrice" 
                                            ErrorMessage="Price is required."
                                            Display="Dynamic"
                                            ValidationGroup="ValidationSummaryEditMcPrice">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtNewPriceIsCurrency" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewPrice"
                                        Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                        ErrorMessage="Price must be a number and not more than 999.99."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEditMcPrice">*</asp:RangeValidator>
                                </td>
                                <td>
                                    <asp:Button ID="btnUpdateMedicarePrice" runat="server" style="width:100%;" CssClass="thin_button" CausesValidation="True" ValidationGroup="ValidationSummaryEditMcPrice" OnCommand="btnUpdatePrice_Command" CommandName="UpdateMedicarePriceAll" Text="Update All MC Prices" OnClientClick="return validate_and_confirm('This will update the Medicare price for  ALL  products and services to $' + parseFloat(document.getElementById('txtNewPrice').value, 10).toFixed(2) + '\r\n\r\nThis can not be undone.   Are you sure?', 'ValidationSummaryEditMcPrice');" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <asp:Button ID="btnSetAllGstExempt" runat="server" style="width:100%;" CssClass="thin_button" OnCommand="btnUpdateGstExempt_Command" CommandName="SetAllGstExempt" Text="Set All GST Exempt" OnClientClick="return confirm('Are you sure you want to set all items as GST exempt?');" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <asp:Button ID="btnSetAllGstNotExempt" runat="server" style="width:100%;" CssClass="thin_button" OnCommand="btnUpdateGstExempt_Command" CommandName="SetAllGstNotExempt" Text="Set All GST 'NOT' Exempt" OnClientClick="return confirm('Are you sure you want to set all items as GST \'NOT\' exempt?');"  />
                                </td>
                            </tr>
                        </table>

                    </center>
                </div>

                <div style="height:12px;"></div>

                
                <img src="images/popup_icon_24.gif" alt="popup icon" style="margin:0 5px 5px 0;" />Popup
                <span style="padding-left:50px;"><img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit</span>
                <span style="padding-left:50px;"><img src="imagesV2/x.png" alt="delete icon" style="margin:0 5px 5px 0;" />Delete</span>
            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:ValidationSummary ID="ValidationSummaryEditMcPrice" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEditMcPrice"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdOffering" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="o_offering_id" 
                         OnRowCancelingEdit="GrdOffering_RowCancelingEdit" 
                         OnRowDataBound="GrdOffering_RowDataBound" 
                         OnRowEditing="GrdOffering_RowEditing" 
                         OnRowUpdating="GrdOffering_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdOffering_RowCommand" 
                         OnRowDeleting="GrdOffering_RowDeleting" 
                         OnRowCreated="GrdOffering_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         RowStyle-VerticalAlign="top" 
                         AllowPaging="True"
                         OnPageIndexChanging="GrdOffering_PageIndexChanging"
                         PageSize="11"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                         <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />


                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="o_offering_id" FooterStyle-VerticalAlign="Top" > 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("o_offering_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("o_offering_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" SortExpression="o_name" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Columns="30" ID="txtName" runat="server" Text='<%# Bind("o_name") %>' MaxLength="100"></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateNameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtName" 
                                        ErrorMessage="Name is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Columns="30" ID="txtNewName" runat="server" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewNameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewName" 
                                        ErrorMessage="Name is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblName" runat="server" Text='<%# Eval("o_name").ToString().Replace(Environment.NewLine, "<br />") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="ShortName" HeaderStyle-HorizontalAlign="Left" SortExpression="o_short_name" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="75%" ID="txtShortName" runat="server" Text='<%# Bind("o_short_name") %>'></asp:TextBox> 
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="75%" ID="txtNewShortName" runat="server" ></asp:TextBox>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblShortName" runat="server" Text='<%# Bind("o_short_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="o_descr" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="75%" ID="txtDescr" runat="server" Text='<%# Bind("o_descr") %>'></asp:TextBox> 
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="75%" ID="txtNewDescr" runat="server" ></asp:TextBox>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDescr" runat="server" Text='<%# Bind("o_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 



                            <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="type_descr" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlOfferingType" runat="server" DataTextField="type_descr" DataValueField="type_offering_type_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOfferingType" runat="server" Text='<%# Eval("type_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOfferingType" runat="server" DataTextField="descr" DataValueField="offering_type_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Field" HeaderStyle-HorizontalAlign="Left" SortExpression="fld_descr" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlField" runat="server" DataTextField="fld_descr" DataValueField="fld_field_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblField" runat="server" Text='<%# Eval("fld_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewField" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patient Subcategory (Aged Care)" HeaderStyle-HorizontalAlign="Left" SortExpression="acpatientcat_descr" FooterStyle-VerticalAlign="Top" > 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlOfferingPatientSubcategory" runat="server" DataTextField="acpatientcat_descr" DataValueField="acpatientcat_aged_care_patient_type_id"></asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOfferingPatientSubcategory" runat="server" Text='<%# Eval("acpatientcat_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOfferingPatientSubcategory" runat="server" DataTextField="descr" DataValueField="aged_care_patient_type_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Yrly Visits Allowed" HeaderStyle-HorizontalAlign="Left" SortExpression="o_num_clinic_visits_allowed_per_year" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlNumClinicVisitsAllowedPerYear" runat="server" DataTextField="o_num_clinic_visits_allowed_per_year" DataValueField="o_num_clinic_visits_allowed_per_year"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblNumClinicVisitsAllowedPerYear" runat="server" Text='<%# Eval("o_num_clinic_visits_allowed_per_year") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewNumClinicVisitsAllowedPerYear" runat="server" DataTextField="o_num_clinic_visits_allowed_per_year" DataValueField="o_num_clinic_visits_allowed_per_year"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Inv Type" HeaderStyle-HorizontalAlign="Left" SortExpression="invtype_descr" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlOfferingInvoiceType" runat="server" DataTextField="invtype_descr" DataValueField="invtype_offering_invoice_type_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOfferingInvoiceType" runat="server" Text='<%# Eval("invtype_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOfferingInvoiceType" runat="server" DataTextField="descr" DataValueField="offering_invoice_type_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="GST Exempt" SortExpression="o_is_gst_exempt" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsGstExempt" runat="server" Checked='<%# Eval("o_is_gst_exempt").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsGstExempt" runat="server" Text='<%# Eval("o_is_gst_exempt").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsGstExempt" runat="server" Checked="true" />
                                </FooterTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Service Time Mins" HeaderStyle-HorizontalAlign="Left" SortExpression="o_service_time_minutes" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlServiceTimeMinutes" runat="server" DataTextField="service_time_minutes" DataValueField="service_time_minutes"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblServiceTimeMinutes" runat="server" Text='<%# Eval("o_service_time_minutes") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewServiceTimeMinutes" runat="server" DataTextField="service_time_minutes" DataValueField="service_time_minutes"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Default Price" HeaderStyle-HorizontalAlign="Left" SortExpression="o_default_price" FooterStyle-VerticalAlign="Top" ItemStyle-Wrap="False"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtDefaultPrice" runat="server" Text='<%# Bind("o_default_price") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateDefaultPriceRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtDefaultPrice" 
                                        ErrorMessage="DefaultPrice is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtDefaultPriceIsCurrency" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtDefaultPrice"
                                        Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                        ErrorMessage="Default Price must be a number and not more than 999.99."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RangeValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewDefaultPrice" runat="server" Text='0.00'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewDefaultPriceRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDefaultPrice" 
                                        ErrorMessage="DefaultPrice is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtNewDefaultPriceIsCurrency" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDefaultPrice"
                                        Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                        ErrorMessage="Default Price must be a number and not more than 999.99."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RangeValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDefaultPrice" runat="server" Text='<%# Bind("o_default_price") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Medicare Charge" HeaderStyle-HorizontalAlign="Left" SortExpression="o_medicare_charge" FooterStyle-VerticalAlign="Top" ItemStyle-Wrap="False"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtMedicareCharge" runat="server" Text='<%# Bind("o_medicare_charge") %>'></asp:TextBox> 
                                    <!-- disabled because marcus wants to allow empty fields, so then we have javascript out in 0.00 -->
                                    <asp:RequiredFieldValidator ID="txtValidateMedicareChargeRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtMedicareCharge" 
                                        ErrorMessage="MedicareCharge is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit" Enabled="False">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtMedicareChargeIsCurrency" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtMedicareCharge"
                                        Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                        ErrorMessage="Medicare Charge must be a number and not more than 999.99."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RangeValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewMedicareCharge" runat="server" Text='0.00'></asp:TextBox>
                                    <!-- disabled because marcus wants to allow empty fields, so then we have javascript out in 0.00 -->
                                    <asp:RequiredFieldValidator ID="txtValidateNewMedicareChargeRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewMedicareCharge" 
                                        ErrorMessage="Medicare Charge is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd" Enabled="False">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtNewMedicareChargeIsCurrency" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewMedicareCharge"
                                        Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                        ErrorMessage="Medicare Charge must be a number and not more than 999.99."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RangeValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMedicareCharge" runat="server" Text='<%# Eval("o_medicare_charge") %>'></asp:Label>&nbsp;&nbsp;
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="DVA Charge" HeaderStyle-HorizontalAlign="Left" SortExpression="o_dva_charge" FooterStyle-VerticalAlign="Top" ItemStyle-Wrap="False"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtDvaCharge" runat="server" Text='<%# Bind("o_dva_charge") %>'></asp:TextBox> 
                                    <!-- disabled because marcus wants to allow empty fields, so then we have javascript out in 0.00 -->
                                    <asp:RequiredFieldValidator ID="txtValidateDvaChargeRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtDvaCharge" 
                                        ErrorMessage="Dva Charge is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit" Enabled="False">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtDvaChargeIsCurrency" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtDvaCharge"
                                        Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                        ErrorMessage="DVA Charge must be a number and not more than 999.99."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RangeValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewDvaCharge" runat="server" Text='0.00'></asp:TextBox>
                                    <!-- disabled because marcus wants to allow empty fields, so then we have javascript out in 0.00 -->
                                    <asp:RequiredFieldValidator ID="txtValidateNewDvaChargeRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewDvaCharge" 
                                        ErrorMessage="DVA Charge is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd" Enabled="False">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtNewDvaChargeIsCurrency" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDvaCharge"
                                        Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                        ErrorMessage="DVA Charge must be a number and not more than 999.99."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RangeValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDvaCharge" runat="server" Text='<%# Eval("o_dva_charge") %>'></asp:Label> &nbsp;&nbsp;
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Ins. Charge" HeaderStyle-HorizontalAlign="Left" SortExpression="o_tac_charge" FooterStyle-VerticalAlign="Top" ItemStyle-Wrap="False"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtTacCharge" runat="server" Text='<%# Bind("o_tac_charge") %>'></asp:TextBox> 
                                    <!-- disabled because marcus wants to allow empty fields, so then we have javascript out in 0.00 -->
                                    <asp:RequiredFieldValidator ID="txtValidateTacChargeRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtTacCharge" 
                                        ErrorMessage="Ins. Charge is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit" Enabled="False">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtTacChargeIsCurrency" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtTacCharge"
                                        Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                        ErrorMessage="Ins. Charge must be a number and not more than 999.99."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RangeValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewTacCharge" runat="server" Text='0.00'></asp:TextBox>
                                    <!-- disabled because marcus wants to allow empty fields, so then we have javascript out in 0.00 -->
                                    <asp:RequiredFieldValidator ID="txtValidateNewTacChargeRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewTacCharge" 
                                        ErrorMessage="Ins. Charge is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd" Enabled="False">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="txtNewTacChargeIsCurrency" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewTacCharge"
                                        Type="Currency" MinimumValue="0.00" MaximumValue="999.99" 
                                        ErrorMessage="Ins. Charge must be a number and not more than 999.99."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RangeValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTacCharge" runat="server" Text='<%# Bind("o_tac_charge") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Medicare Code" HeaderStyle-HorizontalAlign="Left" SortExpression="o_medicare_company_code" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtMedicareCompanyCode" runat="server" Text='<%# Bind("o_medicare_company_code") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateMedicareCompanyCodeRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtMedicareCompanyCode"
                                        ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                        ErrorMessage="MedicareCompanyCode can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewMedicareCompanyCode" runat="server" ></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewMedicareCompanyCodeRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewMedicareCompanyCode"
                                        ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                        ErrorMessage="MedicareCompanyCode can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMedicareCompanyCode" runat="server" Text='<%# Bind("o_medicare_company_code") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="DVA Code" HeaderStyle-HorizontalAlign="Left" SortExpression="o_dva_company_code" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtDvaCompanyCode" runat="server" Text='<%# Bind("o_dva_company_code") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateDvaCompanyCodeRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtDvaCompanyCode"
                                        ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                        ErrorMessage="DvaCompanyCode can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewDvaCompanyCode" runat="server" ></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDvaCompanyCodeRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDvaCompanyCode"
                                        ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                        ErrorMessage="DvaCompanyCode can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDvaCompanyCode" runat="server" Text='<%# Bind("o_dva_company_code") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Ins. Code" HeaderStyle-HorizontalAlign="Left" SortExpression="o_tac_company_code" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtTacCompanyCode" runat="server" Text='<%# Bind("o_tac_company_code") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateTacCompanyCodeRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtTacCompanyCode"
                                        ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                        ErrorMessage="TacCompanyCode can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewTacCompanyCode" runat="server" ></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewTacCompanyCodeRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewTacCompanyCode"
                                        ValidationExpression="^[a-zA-Z0-9\-\s]+$"
                                        ErrorMessage="TacCompanyCode can only be letters or hyphens."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTacCompanyCode" runat="server" Text='<%# Bind("o_tac_company_code") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Max Claimable (Nbr)" HeaderStyle-HorizontalAlign="Left" SortExpression="o_max_nbr_claimable" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlMaxNbrClaimable" runat="server" > </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMaxNbrClaimable" runat="server" Text='<%# Eval("o_max_nbr_claimable") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewMaxNbrClaimable" runat="server"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Max Claimable (Months)" HeaderStyle-HorizontalAlign="Left" SortExpression="o_max_nbr_claimable_months" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlMaxNbrClaimableMonths" runat="server" > </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMaxNbrClaimableMonths" runat="server" Text='<%# Eval("o_max_nbr_claimable_months") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewMaxNbrClaimableMonths" runat="server"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Reminder Letter<br />Months Later To Send" HeaderStyle-HorizontalAlign="Left" SortExpression="o_reminder_letter_months_later_to_send" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlReminderLetterMonthsLaterToSend" runat="server" > </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblReminderLetterMonthsLaterToSend" runat="server" Text='<%# Eval("o_reminder_letter_months_later_to_send") == DBNull.Value || (int)Eval("o_reminder_letter_months_later_to_send") == 0 ? "Disabled" : Eval("o_reminder_letter_months_later_to_send").ToString() %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewReminderLetterMonthsLaterToSend" runat="server" CssClass="hiddencol"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Reminder Letter" HeaderStyle-HorizontalAlign="Left" SortExpression="o_reminder_letter_id" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlReminderLetter" runat="server" > </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblReminderLetter" runat="server"></asp:Label>
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewReminderLetter" runat="server" CssClass="hiddencol"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Use Custom Colour" SortExpression="o_use_custom_color" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkUseCustomColour" runat="server" Checked='<%# Eval("o_use_custom_color").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblUseCustomColour" runat="server" Text='<%# Eval("o_use_custom_color").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewUseCustomColour" runat="server" Checked="false" />
                                </FooterTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Custom Colour" SortExpression="o_custom_color" FooterStyle-VerticalAlign="Top" > 
                                <EditItemTemplate> 
                                    <input class="color" id="ColorPicker" runat="server" maxlength="6" onchange="ensure_ishex(this);" value='<%# Eval("o_custom_color") %>' onkeyup="ensure_ishex(this);" size="6" />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCustomColour" runat="server" Text='<%# Eval("o_custom_color") %>' BackColor='<%# Eval("o_custom_color") == DBNull.Value ? System.Drawing.Color.Transparent : System.Drawing.ColorTranslator.FromHtml("#" + Eval("o_custom_color")) %>' ></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <input class="color" id="NewColorPicker" runat="server" maxlength="6" onkeyup="ensure_ishex(this);" size="6" />
                                </FooterTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="BK Sheet Default Service" ItemStyle-Wrap="false" > 
                                <ItemTemplate> 
                                    <asp:LinkButton ID="btnSetAsBookingScreenDefaultService" runat="server" CommandArgument='<%# Eval("o_offering_id") %>' CommandName="SetAsBookingScreenDefaultService" AlternateText="Set As BK Sheet Default Service Selected" ToolTip="Set As BK Sheet Default Service Selected">Set</asp:LinkButton>
                                    <asp:image ID="imgBookingScreenDefaultService" runat="server" ImageUrl="~/images/tick-24.png" AlternateText="BK Sheet Default Service Selected" ToolTip="BK Sheet Default Service Selected" />
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:ImageButton ID="lnkPopupMessage" runat="server"  ImageUrl="~/images/popup_icon_24.gif" AlternateText="Create A Message/Reminder That Pops-Up When This Service Selected In Booking Sheet" ToolTip="Create A Message/Reminder That Pops-Up When This Service Selected In Booking Sheet" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit" />
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd" />
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <%--<asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" />--%>

                            <asp:TemplateField HeaderText="" ShowHeader="True" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" runat="server"  CommandName="_Delete" CommandArgument='<%# Bind("o_offering_id") %>' ImageUrl="~/images/Delete-icon-24.png" AlternateText="Delete" ToolTip="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns> 

                    </asp:GridView>



                </div>
            </center>
            

        </div>
    </div>

</asp:Content>



