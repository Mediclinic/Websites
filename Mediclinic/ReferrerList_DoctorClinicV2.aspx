<%@ Page Title="Referrer List (Doctor-Clinic)" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ReferrerList_DoctorClinicV2.aspx.cs" Inherits="ReferrerList_DoctorClinicV2" %>
<%@ Register TagPrefix="UC" TagName="DuplicatePersonModalElementControl" Src="~/Controls/DuplicatePersonModalElementControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/duplicate_person_modal_boxV2.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/provider_nbr_check.js"></script>
    <script src="Scripts/check_duplicate_personsV2.js" type="text/javascript"></script>
    <script type="text/javascript">

        function duplicate_person_check() {
            var firstname = document.getElementById('MainContent_GrdReferrer_txtNewFirstname').value.trim();
            var surname = document.getElementById('MainContent_GrdReferrer_txtNewSurname').value.trim();

            var result = ajax_duplicate_persons("referrer", firstname, surname);

            if (result.length == 0) {
                alert("Error retreiving records for duplicate person check.");
            }
            else if (result == "NONE") {
                return;
            }
            else {
                var result_list = create_result_array(result);
                create_table(result_list, "ctable", "ReferrerListV2.aspx?surname_search=", true);
                reveal_modal('modalPopupDupicatePerson');
            }
        }
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }

        function title_changed_reset_gender(ddlTitle, ddlGender) {
            ddlTitle = document.getElementById(ddlTitle);
            var selValue = ddlTitle.options[ddlTitle.selectedIndex].value;
            if (selValue == 6 || selValue == 265 || selValue == 266)
                setSelectedValue(document.getElementById(ddlGender), "M");
            if (selValue == 7 || selValue == 26)
                setSelectedValue(document.getElementById(ddlGender), "F");
        }
        function setSelectedValue(selectObj, valueToSet) {
            for (var i = 0; i < selectObj.options.length; i++) {
                if (selectObj.options[i].value == valueToSet) {
                    selectObj.options[i].selected = true;
                    return;
                }
            }
        }


        var from_regref_id;
        var from_regref_text;

        function move_patients(_from_regref_id, _from_regref_text) {

            // set global variables for callback
            from_regref_id = _from_regref_id;
            from_regref_text = _from_regref_text;

            var isMobileDevice = document.getElementById('hiddenIsMobileDevice').value == "1";
            if (isMobileDevice)
                open_new_tab('ReferrerListPopupV2.aspx');
            else
                window.showModalDialog("ReferrerListPopupV2.aspx", 'Show Popup Window', "dialogHeight:800px;dialogWidth:1150px;resizable:yes;center:yes;");
        }
        function set_register_referrer(retVal) {
            var index = retVal.indexOf(":");
            var to_regref_id = retVal.substring(0, index);
            var to_regref_text = retVal.substring(index + 1)

            if (from_regref_id == to_regref_id)
                return;

            if (!(confirm('Are you sure you want to move all patients of\r\n    ' + from_regref_text + ' \r\nto\r\n    ' + to_regref_text + '?')))
                return

            document.getElementById('hiddenMovePatientFrom').value = from_regref_id;
            document.getElementById('hiddenMovePatientTo').value = to_regref_id;
            document.getElementById('btnMovePatients').click();
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <asp:HiddenField ID="hiddenMovePatientFrom" runat="server" />
    <asp:HiddenField ID="hiddenMovePatientTo" runat="server" />
    <asp:Button ID="btnMovePatients" runat="server" CssClass="hiddencol" OnClick="btnMovePatients_Click" />

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Referrer List (Doctor-Clinic)</span></div>
        <div class="main_content_with_header">
            <div class="user_login_form_no_width" style="width:1000px;">

                <asp:HiddenField ID="hiddenIsMobileDevice" runat="server" Value="0" />

                <div class="border_top_bottom user_login_form_no_width_div">
                    <center>

                        <table>
                            <tr>
                                <td class="nowrap"><asp:Label ID="lblSearchSurname" runat="server">Search By Surname: </asp:Label></td>
                                <td style="min-width:8px;"></td>
                                <td>
                                    <asp:Panel runat="server" DefaultButton="btnSearchSurname">
                                        <asp:TextBox ID="txtSearchSurname" runat="server" onkeypress="javascript:if (event.which == 13 || event.keyCode == 13) document.getElementById('btnSearchSurname').click();"></asp:TextBox>
                                    </asp:Panel>
                                </td>
                                <td class="nowrap">&nbsp;<asp:CheckBox ID="chkSurnameSearchOnlyStartWith" runat="server" Text="" />&nbsp;<label for="chkSurnameSearchOnlyStartWith" style="font-weight:normal;">starts with</label></td>
                                <td class="nowrap">&nbsp;<asp:Button ID="btnSearchSurname" runat="server" Text="Search" onclick="btnSearchSurname_Click" /></td>
                                <td><asp:Button ID="btnClearSurnameSearch" runat="server" Text="Clear" onclick="btnClearSurnameSearch_Click" /></td>
                                <td style="width:50px"></td>
                                <td rowspan="3" class="nowrap">&nbsp;<asp:CheckBox ID="chkUsePaging" runat="server" Text="" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" Checked="True" />&nbsp;<label for="chkUsePaging" style="font-weight:normal;">use paging</label></td>
                                <td rowspan="3" style="width:25px"></td>
                                <td rowspan="3" class="nowrap">&nbsp;<asp:CheckBox ID="chkShowDeleted" runat="server" Text="" AutoPostBack="True" OnCheckedChanged="chkShowDeleted_CheckedChanged" Checked="False" />&nbsp;<label for="chkShowDeleted" style="font-weight:normal;">show deleted</label></td>

                                <%-- 
                                <td id="td_generate_treatment_notes_space_before" runat="server" style="width:75px"></td>
                                <td id="td_generate_treatment_notes_date_label" runat="server" class="nowrap"><asp:Label ID="lblLastBatchSendTreatmentNotes_AllReferrers" runat="server"></asp:Label></td>
                                <td id="td_generate_treatment_notes_date_space_before" runat="server" style="width:2px"></td>
                                <td id="td_generate_treatment_notes_button" runat="server">
                                    <asp:Button ID="btnRunBatchSendTreatmentNotes_AllReferrers_Last6Months" runat="server" Text="Last 6 Mo" Width="75" onclick="btnRunBatchSendTreatmentNotes_AllReferrers_Last6Months_Click" />
                                    <br />
                                    <asp:Button ID="btnRunBatchSendTreatmentNotes_AllReferrers_SinceLast" runat="server" Text="Since Last" Width="75" onclick="btnRunBatchSendTreatmentNotes_AllReferrers_SinceLast_Click" />
                                </td>
                                --%>

                                <td style="width:75px"></td>
                            </tr>
                            <tr>
                                <td class="nowrap"><asp:Label ID="lblSearchProviderNbr" runat="server">Search By Provider Nbr: </asp:Label></td>
                                <td></td>
                                <td>
                                    <asp:Panel runat="server" DefaultButton="btnSearchProviderNbr">
                                        <asp:TextBox ID="txtSearchProviderNbr" runat="server" onkeypress="javascript:if (event.which == 13 || event.keyCode == 13) document.getElementById('btnSearchProviderNbr').click();"></asp:TextBox>
                                    </asp:Panel>
                                </td>
                                <td class="nowrap">&nbsp;<asp:CheckBox ID="chkProviderNbrSearchOnlyStartWith" runat="server" Text="" />&nbsp;<label for="chkProviderNbrSearchOnlyStartWith" style="font-weight:normal;">starts with</label></td>
                                <td class="nowrap">&nbsp;<asp:Button ID="btnSearchProviderNbr" runat="server" Text="Search" onclick="btnSearchProviderNbr_Click" /></td>
                                <td><asp:Button ID="btnClearProviderNbrSearch" runat="server" Text="Clear" onclick="btnClearProviderNbrSearch_Click" /></td>
                            </tr>
                            <tr>
                                <td class="nowrap"><asp:Label ID="lblSearchPhoneNumber" runat="server">Search By Fax/Phone Nbr: </asp:Label></td>
                                <td></td>
                                <td>
                                    <asp:Panel runat="server" DefaultButton="btnSearchPhoneNbr">
                                        <asp:TextBox ID="txtSearchPhoneNbr" runat="server" onkeypress="javascript:if (event.which == 13 || event.keyCode == 13) document.getElementById('btnSearchPhoneNbr').click();"></asp:TextBox>
                                    </asp:Panel>
                                </td>
                                <td class="nowrap">&nbsp;<asp:CheckBox ID="chkPhoneNbrSearchOnlyStartWith" runat="server" Text="" />&nbsp;<label for="chkPhoneNbrSearchOnlyStartWith" style="font-weight:normal;">starts with</label></td>
                                <td class="nowrap">&nbsp;<asp:Button ID="btnSearchPhoneNbr" runat="server" Text="Search" onclick="btnSearchPhoneNbr_Click" /></td>
                                <td><asp:Button ID="btnClearPhoneNbrSearch" runat="server" Text="Clear" onclick="btnClearPhoneNbrSearch_Click" /></td>
                            </tr>
                        </table>

                    </center>
                </div>

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit<span style="padding-left:50px;"><img src="imagesV2/x.png" alt="edit icon" style="margin:0 5px 5px 0;" />Delete</span>

                <br />
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>

            </div>


            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdReferrer" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="referrer_id" 
                         OnRowCancelingEdit="GrdReferrer_RowCancelingEdit" 
                         OnRowDataBound="GrdReferrer_RowDataBound" 
                         OnRowEditing="GrdReferrer_RowEditing" 
                         OnRowUpdating="GrdReferrer_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdReferrer_RowCommand" 
                         OnRowDeleting="GrdReferrer_RowDeleting" 
                         OnRowCreated="GrdReferrer_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         RowStyle-VerticalAlign="top"
                         AllowPaging="True"
                         OnPageIndexChanging="GrdReferrer_PageIndexChanging"
                         PageSize="15"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                         <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <%-- RegReferrer --%>

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_referrer_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_referrer_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_referrer_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <%-- Referrer --%>

                            <asp:TemplateField HeaderText="Title" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlTitle" runat="server" DataTextField="descr" DataValueField="title_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("title_id") == DBNull.Value || (int)Eval("title_id") == 0 ? "" :  Eval("descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewTitle" runat="server" DataTextField="descr" DataValueField="title_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Firstname" HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left" ItemStyle-Wrap="false"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtFirstname" runat="server" Text='<%# Bind("firstname") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateFirstnameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtFirstname"
                                        ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewFirstname" runat="server" ></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewFirstnameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewFirstname"
                                        ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Firstname can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFirstname" runat="server" Text='<%# Bind("firstname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="M.name" HeaderStyle-HorizontalAlign="Left" SortExpression="middlename" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left" ItemStyle-Wrap="false"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtMiddlename"
                                        ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewMiddlename" runat="server" ></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateNewMiddlenameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewMiddlename"
                                        ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Middlename can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Surname" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left" ItemStyle-Wrap="false"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtSurname" runat="server" Text='<%# Bind("surname") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtSurname" 
                                        ErrorMessage="Surname is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateSurnameNameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtSurname"
                                        ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                 </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewSurname" runat="server" ></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateSurnameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewSurname" 
                                        ErrorMessage="Surname is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewSurnameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewSurname"
                                        ValidationExpression="^[a-zA-Z\-\.\s']+$"
                                        ErrorMessage="Surname can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSurname" runat="server" Text='<%# Bind("surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Gender" HeaderStyle-HorizontalAlign="Left" SortExpression="gender" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlGender" runat="server" SelectedValue='<%# Eval("gender") %>'> 
                                        <asp:ListItem Text="M" Value="M"></asp:ListItem>
                                        <asp:ListItem Text="F" Value="F"></asp:ListItem>
                                        <asp:ListItem Text="-" Value=""></asp:ListItem>
                                    </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblGender" runat="server" Text='<%# ( Eval("gender").ToString() == "M")?"Male" : (( Eval("gender").ToString() == "F")?"Female" : "-") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewGender" runat="server" >
                                        <asp:ListItem Text="M" Value="M" Selected="True"></asp:ListItem> 
                                        <asp:ListItem Text="F" Value="F"></asp:ListItem>
                                    </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 


                            <%-- Org --%>

                            <asp:TemplateField HeaderText="Org Name" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="name" ItemStyle-CssClass="text_left" ItemStyle-Wrap="false"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="85%" ID="txtName" runat="server" Text='<%# Bind("name") %>'></asp:TextBox> 
                                    <asp:RequiredFieldValidator ID="txtValidateNameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtName" 
                                        ErrorMessage="Org Name is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtName"
                                        ValidationExpression="^[a-zA-Z\-\s\.\',]+$"
                                        ErrorMessage="Org Name can only be letters, and dashes."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="85%" ID="txtNewName" runat="server" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="txtValidateNewNameRequired" runat="server" CssClass="failureNotification"  
                                        ControlToValidate="txtNewName" 
                                        ErrorMessage="Org Name is required."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="txtValidateNewNameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewName"
                                        ValidationExpression="^[a-zA-Z\-\s\.\',]+$"
                                        ErrorMessage="Org Name can only be letters, and dashes."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lnkTitle" runat="server" Text='<%# Eval("name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Org ABN" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="abn" ItemStyle-Wrap="false"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="75%" ID="txtABN" runat="server" Text='<%# Bind("abn") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateABNRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtABN"
                                        ValidationExpression="^[0-9\-]+$"
                                        ErrorMessage="Org ABN can only be numbers and dashes."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="75%" ID="txtNewABN" runat="server" ></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewABNRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewABN"
                                        ValidationExpression="^[0-9\-]+$"
                                        ErrorMessage="Org ABN can only be numbers and dashes."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblABN" runat="server" Text='<%# Bind("abn") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Org ACN" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" SortExpression="acn" ItemStyle-Wrap="false"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="75%" ID="txtACN" runat="server" Text='<%# Bind("acn") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateACNRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtACN"
                                        ValidationExpression="^[0-9\-]+$"
                                        ErrorMessage="Org ACN can only be numbers and dashes."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="75%" ID="txtNewACN" runat="server" ></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewACNRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewACN"
                                        ValidationExpression="^[0-9\-]+$"
                                        ErrorMessage="Org ACN can only be numbers and dashes."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblACN" runat="server" Text='<%# Bind("acn") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <%-- RegReferrer --%>

                            <asp:TemplateField HeaderText="Provider Number" HeaderStyle-HorizontalAlign="Left" SortExpression="provider_number" FooterStyle-VerticalAlign="Top" ItemStyle-Wrap="false"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtProviderNumber" runat="server" Text='<%# Bind("provider_number") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtProviderNumber"
                                        ValidationExpression="^[a-zA-Z\-\s]+$"
                                        ErrorMessage="Provider Number can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="EditRegistrationValidationGroup">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewProviderNumber" runat="server" ></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewProviderNumber"
                                        ValidationExpression="^[a-zA-Z\-\s]+$"
                                        ErrorMessage="Provider Number can only be letters, hyphens, or fullstops."
                                        Display="Dynamic"
                                        ValidationGroup="AddRegistrationValidationGroup">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblProviderNumber" runat="server" Text='<%# Bind("provider_number") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Report Every Visit" SortExpression="report_every_visit_to_referrer" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsReportEveryVisit" runat="server" Checked='<%# Eval("report_every_visit_to_referrer").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsReportEveryVisit" runat="server" Text='<%# Eval("report_every_visit_to_referrer").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsReportEveryVisit" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Batch Send Treatment Notes" SortExpression="batch_send_all_patients_treatment_notes" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsBatchSendAllPatientsTreatmentNotes" runat="server" Checked='<%# Eval("batch_send_all_patients_treatment_notes").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsBatchSendAllPatientsTreatmentNotes" runat="server" Text='<%# Eval("batch_send_all_patients_treatment_notes").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsBatchSendAllPatientsTreatmentNotes" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Last Batch Send Letters"  HeaderStyle-HorizontalAlign="Left" SortExpression="date_last_batch_send_all_patients_treatment_notes"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblDateLastBatchSendAllPatientsTreatmentNotes" runat="server" Text='<%# Eval("date_last_batch_send_all_patients_treatment_notes", "{0:dd-MM-yyyy}")  %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblNewDateLastBatchSendAllPatientsTreatmentNotes" runat="server" Text='<%# Eval("date_last_batch_send_all_patients_treatment_notes", "{0:dd-MM-yyyy}")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <%-- 
                            <asp:TemplateField HeaderText="Treatment Notes<br />All Patients"  HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Center"> 
                                <ItemTemplate> 
                                    <asp:Button ID="btnRunBatchSendTreatmentNotes" runat="server" Text="Generate 6 mo" OnClick="btnRunBatchSendTreatmentNotes_Click" CommandArgument='<%# Eval("register_referrer_id") %>' />
                                </ItemTemplate> 
                            </asp:TemplateField> 
                            --%>


                            <asp:TemplateField HeaderText="Date Added"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_referrer_date_added"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_referrer_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_referrer_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Contact"  HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="nowrap"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkContactInfo" runat="server" Text="Clinic Contact" ></asp:HyperLink>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Suburb"  HeaderStyle-HorizontalAlign="Left" SortExpression="suburb_name" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblSuburb" runat="server" Text='<%# Eval("suburb_name")  %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSuburb" runat="server" Text='<%# Eval("suburb_name")  %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Patients"  HeaderStyle-HorizontalAlign="Left" SortExpression="count" ItemStyle-Wrap="False"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkPatients" runat="server" Text='<%# "Patients (" + Eval("count") + ")"  %>' NavigateUrl='<%# "~/PatientListV2.aspx?extended_search_open=1&referrer_search=" + Eval("register_referrer_id") %>' ></asp:HyperLink>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Move Patients"  HeaderStyle-HorizontalAlign="Left"> 
                                <ItemTemplate> 
                                    <asp:HyperLink ID="lnkMovePatients" runat="server" Text="Move" onclick='<%# "javascript:move_patients(" + Eval("register_referrer_id") + ",\"" + Eval("firstname") + " " + Eval("surname") + " [" + Eval("name") + "]\"); return false;"  %>' NavigateUrl='javascript:void(0)' ></asp:HyperLink>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Deleted" SortExpression="is_deleted" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
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

                            <asp:TemplateField HeaderText="" ShowHeader="True" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" runat="server"  CommandName="_Delete" CommandArgument='<%# Bind("register_referrer_id") %>' ImageUrl="~/images/Delete-icon-24.png" AlternateText="Delete" ToolTip="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns> 
                    </asp:GridView>


                </div>
            </center>
            

        </div>
    </div>

</asp:Content>



