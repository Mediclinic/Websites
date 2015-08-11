<%@ Page Title="Organisation List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="RegisterOrganisationsToStaffV2.aspx.cs" Inherits="RegisterOrganisationsToStaffV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="Scripts/provider_nbr_check.js"></script>
    <script type="text/javascript">
        function provider_check_submit() {
            var isValid = Page_ClientValidate('ValidationSummaryAdd');
            if (!isValid)
                return false;

            var enteredProvNbr = document.getElementById('txtNewProviderNumber').value;
            if (String(enteredProvNbr).length == 0)
                return true;
            var provListString = document.getElementById('lblProvNbrs').firstChild.data;
            var proList = String(provListString).split(",");

            var contains = false;
            for (var i = 0; i < proList.length; i++) {
                if (enteredProvNbr == String(proList[i]))
                    contains = true;
            }
            if (contains)
                return confirm("Provider number already in use. Are you sure you want to use it again?");
            else
                return true;
            //window.showModalDialog('ProvNbrCheckPopup.aspx?prov_id=' + document.getElementById('txtNewProviderNumber').value, "", "dialogWidth:370px; dialogHeight:150px; center:yes; resizable:no; scroll:no;"); 
        }
        function update_new_main_provider_popup(obj) {
            if (obj.checked) {
                var conf = confirm('Setting this person as the primary provider will set any previous primary providers for that clinic as not the primary provider.\n\rDo you still want to set this person as this organisations primary provider for this clinic?');
                if (!conf)
                    obj.checked = false;
            }
        }

        function show_hide(id) {
            obj = document.getElementById(id);
            obj.style.display = (obj.style.display == "none") ? "" : "none";
            update_to_max_height(); // update autodivheight
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Manage Registrations For </asp:Label></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">

                    <table class="block_center">
                        <tr>
                            <td style="white-space:nowrap;vertical-align:top;">
                                <asp:CheckBox ID="chkShowDeleted" runat="server" Text="&nbsp;show deleted" AutoPostBack="True" OnCheckedChanged="chkShowDeleted_CheckedChanged" Checked="False" />
                                <br />

                                <span>
                                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit
                                <span style="padding-left:15px;"><img src="imagesV2/x.png" alt="delete icon" style="margin:0 5px 5px 0;" />Delete</span>
                                </span>

                            </td>
                            <td style="min-width:35px;"></td>
                            <td style="text-align:left;color:blue;width:525px;">
                                <center>
                                    <a href="javascript:void(0)"  onclick="show_hide('help_link'); return false;">Not Showing On Booking Screen? Click Here.</a>
                                </center>
                                <span id="help_link" style="display:none;">
                                    * Note that to be showing in the booking sheet, the following must be done:
                                    <ol>
                                    <li>Clinic below set to work <u>on that weekday</u></li>
                                    <li>Staff detail page - not set to fired</li>
                                    <li>Staff detail page - <u>start date</u> - unset or set to before date required</li>
                                    <li>Staff detail page - <u>end date</u> - unset or set to after date required</li>
                                    <li>Clinic detail page - <u>start date</u> - unset or set to before date required</li>
                                    <li>Clinic detail page - <u>end date</u> - unset or set to after date required</li>
                                    <li>Clinic detail page - <u>weekday</u> - checked to set as open on that weekday</li>
                                    <li>Clinic detail page - weekday <u>hours</u> - start time before end time</li>
                                    </ol>
                                </span>
                            </td>
                        </tr>
                    </table>

                </div>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <asp:Label ID="lblProvNbrs" runat="server" CssClass="hiddencol"></asp:Label>
            <div class="text-center">
                <asp:HyperLink ID="lnkThisStaff" runat="server" CssClass="text_center" NavigateUrl="~/StaffListV2.aspx?id=">Edit</asp:HyperLink> 
                <div style="height:8px;"></div>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdRegistration" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="register_staff_id" 
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

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_staff_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_staff_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("register_staff_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Added to this org'n"  HeaderStyle-HorizontalAlign="Left" SortExpression="register_staff_date_added"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_staff_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Eval("register_staff_date_added", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Organisation" HeaderStyle-HorizontalAlign="Left" SortExpression="name" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("name") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewOrganisation" runat="server" DataTextField="name" DataValueField="organisation_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Provider Number" HeaderStyle-HorizontalAlign="Left" SortExpression="registration_provider_number" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtProviderNumber" runat="server" Text='<%# Bind("registration_provider_number") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtProviderNumber"
                                        ValidationExpression="^[a-zA-Z0-9]+$"
                                        ErrorMessage="ProviderNumber can only be letters, numbers, and underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:TextBox Width="90%" ID="txtNewProviderNumber" runat="server" onblur="provider_check(this);"></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateNewProviderNumberRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewProviderNumber"
                                        ValidationExpression="^[a-zA-Z0-9]+$"
                                        ErrorMessage="ProviderNumber can only be letters, numbers, and underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblProviderNumber" runat="server" Text='<%# Bind("registration_provider_number") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Main Provider" SortExpression="main_provider_for_clinic" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkMainProvider" runat="server" Checked='<%# Eval("main_provider_for_clinic").ToString()=="True"?true:false %>' onclick="update_new_main_provider_popup(this);" />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMainProvider" runat="server" Text='<%# Eval("main_provider_for_clinic").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewMainProvider" runat="server" onclick="update_new_main_provider_popup(this);" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Works<br />Sundays" SortExpression="excl_sun" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate>
                                    <asp:CheckBox ID="chkIncSundays" runat="server" Checked='<%# Eval("excl_sun").ToString()=="True"?false:true%>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIncSundays" runat="server" Text='<%# Eval("excl_sun").ToString()=="True"?"No":"Yes" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIncSundays" runat="server" Checked="True" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Works<br />Mondays" SortExpression="excl_mon" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIncMondays" runat="server" Checked='<%# Eval("excl_mon").ToString()=="True"?false:true%>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIncMondays" runat="server" Text='<%# Eval("excl_mon").ToString()=="True"?"No":"Yes" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIncMondays" runat="server" Checked="True" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Works<br />Tuesdays" SortExpression="excl_tue" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIncTuesdays" runat="server" Checked='<%# Eval("excl_tue").ToString()=="True"?false:true%>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIncTuesdays" runat="server" Text='<%# Eval("excl_tue").ToString()=="True"?"No":"Yes" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIncTuesdays" runat="server" Checked="True" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Works<br />Wednesdays" SortExpression="excl_wed" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIncWednesdays" runat="server" Checked='<%# Eval("excl_wed").ToString()=="True"?false:true%>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIncWednesdays" runat="server" Text='<%# Eval("excl_wed").ToString()=="True"?"No":"Yes" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIncWednesdays" runat="server" Checked="True" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Works<br />Thursdays" SortExpression="excl_thu" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIncThursdays" runat="server" Checked='<%# Eval("excl_thu").ToString()=="True"?false:true%>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIncThursdays" runat="server" Text='<%# Eval("excl_thu").ToString()=="True"?"No":"Yes" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIncThursdays" runat="server" Checked="True" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Works<br />Fridays" SortExpression="excl_fri" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIncFridays" runat="server" Checked='<%# Eval("excl_fri").ToString()=="True"?false:true%>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIncFridays" runat="server" Text='<%# Eval("excl_fri").ToString()=="True"?"No":"Yes" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIncFridays" runat="server" Checked="True" />
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Works<br />Saturdays" SortExpression="excl_sat" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIncSaturdays" runat="server" Checked='<%# Eval("excl_sat").ToString()=="True"?false:true%>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIncSaturdays" runat="server" Text='<%# Eval("excl_sat").ToString()=="True"?"No":"Yes" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIncSaturdays" runat="server" Checked="True" />
                                </FooterTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="Deleted" SortExpression="registration_is_deleted" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("registration_is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("registration_is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
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
                                    <%-- OnClientClick= "javascript:if (!provider_check_submit()) return false;" --%>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <%-- <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True" /> --%>

                            <asp:TemplateField HeaderText="" ShowHeader="True" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" runat="server"  CommandName="_Delete" CommandArgument='<%# Bind("register_staff_id") %>' ImageUrl="~/images/Delete-icon-24.png" AlternateText="Delete" ToolTip="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



