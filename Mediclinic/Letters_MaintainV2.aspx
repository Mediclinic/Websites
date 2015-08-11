<%@ Page Title="Maintain Letters" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Letters_MaintainV2.aspx.cs" Inherits="Letters_MaintainV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <!--script type="text/javascript" src="ScriptsV2/autodivheight.js" ></!--script-->
    <!--// plugin-specific resources //-->
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="Scripts/jquery.form.js" type="text/javascript"></script>
    <script src="Scripts/jquery.MetaData.js" type="text/javascript"></script>
    <script src="Scripts/jquery.MultiFile.js" type="text/javascript"></script>
    <script src="Scripts/jquery.blockUI.js" type="text/javascript"></script>
    <script type="text/javascript">


        var prev_font_color = 'transparent';
        var prev_bg_color = 'transparent';

        // http://www.webreference.com/programming/javascript/onloads/index.html
        function addLoadEvent(func) {
            var oldonload = window.onload;
            if (typeof window.onload != 'function') {
                window.onload = func;
            } else {
                window.onload = function () {
                    if (oldonload) {
                        oldonload();
                    }
                    func();
                }
            }
        }

        //window.onload = function () {
        //    show_hide('div_fields_list');
        //};

        addLoadEvent(function () {
            //show_hide('div_fields_list');
        });

        function show_hide(id) {
            obj = document.getElementById(id);

            obj = document.getElementById(id);
            obj.style.display = (obj.style.display == "none") ? "" : "none";

            /*
            var new_font_color = prev_font_color;
            prev_font_color = document.getElementById(id).style.color;
            document.getElementById(id).style.color = new_font_color;

            var new_bg_color = prev_bg_color;
            prev_bg_color = document.getElementById(id).style.background;
            document.getElementById(id).style.background = new_bg_color;
            */
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Maintain Letters</asp:Label> &nbsp; <asp:HyperLink ID="lnkToEntity" runat="server"></asp:HyperLink></div>
        <div class="main_content" style="padding:20px 5px;">

            <div class="user_login_form">

                <table style="min-width:250px;" class="block_center">
                    <tr>

                        <td>

                            <a href="javascript:void(0)" onclick="show_hide('div_fields_list'); return false">Show/Hide Fields</a><br />

                            <div id="div_fields_list" style="display:none;position: absolute; left: 50%;">
                                <div style="position:relative; left:-50%; background:#FFFFFF;" class="block_center">

                                    <table class="padded-table-2px text-left">

                                        <tr><td>curr_date</td><td style="width:10px;"></td><td>Current Date</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td>pt_name</td><td></td><td>Patient Full Name</td></tr>
                                        <tr><td>pt_title</td><td></td><td>Patient Title</td></tr>
                                        <tr><td>pt_firstname</td><td></td><td>Patient Firstname</td></tr>
                                        <tr><td>pt_middlename</td><td></td><td>Patient Middlename</td></tr>
                                        <tr><td>pt_surname</td><td></td><td>Patient Surname</td></tr>
                                        <tr><td>pt_gender</td><td></td><td>Patient Gender</td></tr>
                                        <tr><td>pt_dob</td><td></td><td>Patient DOB</td></tr>
                                        <tr><td>pt_dob_day_month_only</td><td></td><td>Patient DOB - Day & Month Only</td></tr>
                                        <tr><td>pt_addr</td><td></td><td>Patient Address</td></tr>
                                        <tr><td>pt_addr_tabbedx1</td><td></td><td>Patient Address Tabbed</td></tr>
                                        <tr><td>pt_phone</td><td></td><td>Patient Phone Nbr</td></tr>
                                        <tr><td>pt_last_bk_date</td><td></td><td>Patient Last Booking Date</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td>pt_conditions</td><td></td><td>Patient Conditions</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td>pt_hc_card_nbr</td><td></td><td>Active Health Card Nbr</td></tr>
                                        <tr><td>pt_hc_card_name</td><td></td><td>Active Health Card Name</td></tr>
                                        <tr><td>pt_hc_card_refsigneddate</td><td></td><td>Active Health Card EPC/Ref. Signed Date</td></tr>
                                        <tr><td>pt_epc_expire_date</td><td></td><td>Active Health Card EPC Expire Date</td></tr>
                                        <tr><td>pt_epc_count_remaining</td><td></td><td>Active Health Card EPC Count Remaining</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr style="display:none;"><td>pt_mc_card_nbr</td><td></td><td>Medicare Card Nbr</td></tr>
                                        <tr style="display:none;"><td>pt_mc_card_name</td><td></td><td>Medicare Card Name</td></tr>
                                        <tr style="display:none;"><td>pt_mc_card_refsigneddate</td><td></td><td>Medicare Card EPC/Ref. Signed Date</td></tr>
                                        <tr style="display:none;"><td colspan="3">&nbsp;</td></tr>

                                        <tr style="display:none;"><td>pt_dvacard_nbr</td><td></td><td>DVA Card Nbr</td></tr>
                                        <tr style="display:none;"><td>pt_dvacard_name</td><td></td><td>DVA Card Name</td></tr>
                                        <tr style="display:none;"><td>pt_dvacard_refsigneddate</td><td></td><td>DVA Card EPC/Ref. Signed Date</td></tr>
                                        <tr style="display:none;"><td colspan="3">&nbsp;</td></tr>

                                        <tr><td>org_name</td><td></td><td>Clinic/Facility Name</td></tr>
                                        <tr><td>org_abn</td><td></td><td>Clinic/Facility ABN</td></tr>
                                        <tr><td>org_acn</td><td></td><td>Clinic/Facility ACN</td></tr>
                                        <tr><td>org_bpay_account</td><td></td><td>Clinic/Facility BPay Acct</td></tr>
                                        <tr><td>org_addr</td><td></td><td>Clinic/Facility Address</td></tr>
                                        <tr><td>org_addr_tabbedx1</td><td></td><td>Clinic/Facility Address Tabbed</td></tr>
                                        <tr><td>org_phone</td><td></td><td>Clinic/Facility Phone Nbr</td></tr>
                                        <tr><td>org_office_fax</td><td></td><td>Clinic/Facility Fax Nbr</td></tr>
                                        <tr><td>org_web</td><td></td><td>Clinic/Facility Website</td></tr>
                                        <tr><td>org_email</td><td></td><td>Clinic/Facility Email Address</td></tr>

                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td>ref_name</td><td></td><td>Referrer Full Name</td></tr>
                                        <tr><td>ref_title</td><td></td><td>Referrer Title</td></tr>
                                        <tr><td>ref_firstname</td><td></td><td>Referrer First Name</td></tr>
                                        <tr><td>ref_middlename</td><td></td><td>Referrer Middlename</td></tr>
                                        <tr><td>ref_surname</td><td></td><td>Referrer Surname</td></tr>
                                        <tr><td>ref_addr</td><td></td><td>Referrer Address</td></tr>
                                        <tr><td>ref_addr_tabbedx1</td><td></td><td>Referrer Address Tabbed</td></tr>
                                        <tr><td>ref_phone</td><td></td><td>Referrer Phone Nbr</td></tr>
                                        <tr><td>ref_fax</td><td></td><td>Referrer Fax Nbr</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td>bk_date</td><td></td><td>Booking Treatment Date</td></tr>
                                        <tr><td>bk_time</td><td></td><td>Booking Treatment Time</td></tr>
                                        <tr><td>bk_length</td><td></td><td>Booking Treatment Length</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td>bk_prov_name</td><td></td><td>Booking Provider Full Name</td></tr>
                                        <tr><td>bk_prov_title</td><td></td><td>Booking Provider Title</td></tr>
                                        <tr><td>bk_prov_firstname</td><td></td><td>Booking Provider Firstname</td></tr>
                                        <tr><td>bk_prov_middlename</td><td></td><td>Booking Provider Middlename</td></tr>
                                        <tr><td>bk_prov_surname</td><td></td><td>Booking Provider Surname</td></tr>
                                        <tr><td>bk_prov_number</td><td></td><td>Booking Provider Number</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td>bk_treatment_notes</td><td></td><td>Booking Treatment Notes</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td>bk_offering_name</td><td></td><td>Offering Name</td></tr>
                                        <tr><td>bk_offering_short_name</td><td></td><td>Offering Short Name</td></tr>
                                        <tr><td>bk_offering_descr</td><td></td><td>Offering Description</td></tr>

                                    </table>

                                </div>
                            </div>
                        </td>

                        <td>
                            <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit
                        </td>

                    </tr>

                </table>


                

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="___autodivheight" class="divautoheight" style="height:300px;">

                    <asp:GridView ID="GrdLetter" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="letter_letter_id" 
                         OnRowCancelingEdit="GrdLetter_RowCancelingEdit" 
                         OnRowDataBound="GrdLetter_RowDataBound" 
                         OnRowEditing="GrdLetter_RowEditing" 
                         OnRowUpdating="GrdLetter_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdLetter_RowCommand" 
                         OnRowDeleting="GrdLetter_RowDeleting" 
                         OnRowCreated="GrdLetter_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         RowStyle-VerticalAlign="top"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="letter_letter_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("letter_letter_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("letter_letter_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Organisation" HeaderStyle-HorizontalAlign="Left" SortExpression="letterorg_name" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='<%# Eval("letterorg_name") == DBNull.Value ? "Default" : Eval("letterorg_name") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:Label ID="lblOrganisation" runat="server" Text='Default'></asp:Label> 
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Letter Type" HeaderStyle-HorizontalAlign="Left" SortExpression="lettertype_descr" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlLetterType" runat="server" DataTextField="descr" DataValueField="letter_type_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLetterType" runat="server" Text='<%# Eval("lettertype_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewLetterType" runat="server" DataTextField="descr" DataValueField="letter_type_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Site" HeaderStyle-HorizontalAlign="Left" SortExpression="site_name" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlSite" runat="server" DataTextField="site_name" DataValueField="site_site_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSite" runat="server" Text='<%# Eval("site_name") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewSite" runat="server" DataTextField="name" DataValueField="site_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Doc Name" HeaderStyle-HorizontalAlign="Left" SortExpression="letter_docname" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtDocName" runat="server" Text='<%# Bind("letter_docname") %>'></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateDocNameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtDocName"
                                        ValidationExpression="^[0-9a-zA-Z\-_\s\.]+$"
                                        ErrorMessage="DocName can only be letters or hyphens, underscore, or full stops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewDocName" runat="server" ></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewDocNameRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewDocName"
                                        ValidationExpression="^[0-9a-zA-Z\-_\s\.]+$"
                                        ErrorMessage="DocName can only be letters, hyphens, underscore, or full stops."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDocName" runat="server" Text='<%# Bind("letter_docname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="File Exists" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFileExists" runat="server"></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Medicare Rej Code" HeaderStyle-HorizontalAlign="Left" SortExpression="letter_code" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="90%" ID="txtCode" runat="server" Text='<%# Bind("letter_code") %>' MaxLength="10"></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateCodeRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtCode"
                                        ValidationExpression="^[0-9a-zA-Z\-_\s]+$"
                                        ErrorMessage="Code can only be letters or hyphens, or underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="90%" ID="txtNewCode" runat="server" MaxLength="10" ></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewCodeRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewCode"
                                        ValidationExpression="^[0-9a-zA-Z\-_\s]+$"
                                        ErrorMessage="Code can only be letters, hyphens, or underscore."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCode" runat="server" Text='<%# Bind("letter_code") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Medicare Rej. Msg For New PT Inv." HeaderStyle-HorizontalAlign="Left" SortExpression="letter_reject_message" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="max_width_XXpx"> 
                                <EditItemTemplate> 
                                    <asp:TextBox Width="95%" ID="txtMessage" runat="server" Text='<%# Bind("letter_reject_message") %>' MaxLength="200" CssClass="max_width_letters_mc_rej_msg"></asp:TextBox> 
                                    <asp:RegularExpressionValidator ID="txtValidateMessageRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtMessage"
                                        ValidationExpression="^[0-9a-zA-Z\-_\s\[\]\(\)]+$"
                                        ErrorMessage="Medicare Rej Message can only be letters, numbers, hyphens, underscore, or brackets."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryEdit">*</asp:RegularExpressionValidator>
                                </EditItemTemplate> 
                                <FooterTemplate>
                                    <asp:TextBox Width="95%" ID="txtNewMessage" runat="server" MaxLength="200" CssClass="max_width_letters_mc_rej_msg"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="txtValidateNewMessageRegex" runat="server" CssClass="failureNotification" 
                                        ControlToValidate="txtNewMessage"
                                        ValidationExpression="^[0-9a-zA-Z\-_\s\[\]\(\)]+$"
                                        ErrorMessage="Medicare Rej Message can only be letters, numbers, hyphens, underscore, or brackets."
                                        Display="Dynamic"
                                        ValidationGroup="ValidationSummaryAdd">*</asp:RegularExpressionValidator>
                                </FooterTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMessage" runat="server" Text='<%# Bind("letter_reject_message") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Reclaimable" SortExpression="letter_is_allowed_reclaim" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsAllowedReclaim" runat="server" Checked='<%# Eval("letter_is_allowed_reclaim").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsAllowedReclaim" runat="server" Text='<%# Eval("letter_is_allowed_reclaim").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsAllowedReclaim" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Send To Medico" SortExpression="letter_is_send_to_medico" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsSendToMedico" runat="server" Checked='<%# Eval("letter_is_send_to_medico").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsSendToMedico" runat="server" Text='<%# Eval("letter_is_send_to_medico").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsSendToMedico" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Manual Override" SortExpression="letter_is_manual_override" FooterStyle-VerticalAlign="Top" Visible="false"> 
                                <EditItemTemplate> 
                                    <asp:CheckBox ID="chkIsManualOverride" runat="server" Checked='<%# Eval("letter_is_manual_override").ToString()=="True"?true:false %>' />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsManualOverride" runat="server" Text='<%# Eval("letter_is_manual_override").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:CheckBox ID="chkNewIsManualOverride" runat="server" />
                                </FooterTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Copies To Print" HeaderStyle-HorizontalAlign="Left" SortExpression="letter_num_copies_to_print" FooterStyle-VerticalAlign="Top" Visible="false"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlNumCopiesToPrint" runat="server" SelectedValue='<%# Eval("letter_num_copies_to_print") %>'> 
                                        <asp:ListItem Value="0" Text="0"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="1"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="4"></asp:ListItem>
                                        <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                    </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblNumCopiesToPrint" runat="server" Text='<%# Eval("letter_num_copies_to_print") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewNumCopiesToPrint" runat="server">
                                        <asp:ListItem Value="0" Text="0"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="4"></asp:ListItem>
                                        <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                    </asp:DropDownList>
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

                            <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" /> 

                        </Columns> 
                    </asp:GridView>

                </div>
            </center>


            <table id="spn_manage_files" runat="server" class="block_center vertical_align_top">

                <tr>

                    <td style="width:70px"></td>

                    <td>
                        <br />
                        <a name="current_files_tag"></a><strong>Current Files</strong>
                        <br /><br />
                        <asp:Repeater id="lstCurrentFiles" runat="server">
                            <HeaderTemplate>
                                <table style="width:100%">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:LinkButton ID="btnDelFile" runat="server" ToolTip="Delete" CommandArgument='<%# Bind("filepath") %>' OnClick="btnDeleteFie_Click" Text='<%# Bind("text") %>'
                                                        OnClientClick="javascript:if (!confirm('Are you sure you want to permanently delete this file?')) return false;" />
                                        <asp:Label ID="lblFileName" Text='<%# Bind("filename") %>' runat="server"></asp:Label> 
                                        (<asp:LinkButton ID="lnkFileName" runat="server" CommandArgument='<%# Bind("filepath") %>' OnClick="btnDownload_Click" Text='download' />)
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>

                    <td style="width:75px"></td>

                    <td>
                        <br />
                        <strong>Upload Files</strong>
                        <br />
                        <br />
                        <table>
                            <tr>
                                <td>
                                    <asp:FileUpload ID="FileUpload1" runat="server" class="multi" accept="dot|doc|docx|pdf" style="width:80px;overflow:hidden;" />
                                    <div style="line-height:9px;">&nbsp;</div>
                                    <asp:Button ID="btnUpload" runat="server" Text="Upload All" onclick="btnUpload_Click" /> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:CheckBox ID="chkAllowOverwrite" runat="server" Font-Size="Small" ForeColor="GrayText" Text="&nbsp;Allow File Overwrite" />
                                    <br />
                                    <br />
                                    <asp:Label ID="lblUploadMessage" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>

                <tr style="height:15px;">
                    <td colspan="4"></td>
                </tr>

                <tr>
                    <td colspan="4" style="text-align:center;">

                            <div style="height:28px;"></div>

                            <a href="javascript:void(0)" onclick="show_hide('div_invfields_list'); return false" style="white-space:nowrap">Show/Hide Invoice Fields</a><br />

                            <div id="div_invfields_list" style="display:none;position: absolute; left: 50%;">
                                <div style="position:relative; left:-50%; background:#FFFFFF;" class="block_center">

                                    <table class="padded-table-2px text-left">

                                        <tr><td style="width:10px;white-space:nowrap;">curr_date</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Current Date</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td style="width:10px;white-space:nowrap;">inv_nbr</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Invoice Nbr</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">inv_date</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Invoice Date</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">inv_debtor_name</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Invoice Debtor Name (org or pt)</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">inv_debtor_addr</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Invoice Debtor Address</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">inv_debtor_addr_tabbedx1</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Invoice Debtor Address Tabbed</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;vertical-align:top;">inv_pay_online_link</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Invoice Online Payment Link<br />(Only For Ezidebit Enabled)</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td style="width:10px;white-space:nowrap;">bk_pt_fullname</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Patient Full Name</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_pt_addr</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Patient Address</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_pt_addr_tabbedx1</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Patient Address Tabbed</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_prov_fullname</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Provider Full Name</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_prov_number</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Provider Number</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_date</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Date</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_next_info</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Next Booking Date/Time</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;vertical-align:top;">bk_purchase_order_nbr</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Purchase Order Booking Nbr<br />Includes text "<i>Purchase Order Nbr: 123456</i>"</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td style="width:10px;white-space:nowrap;">bk_org_name</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Clinic/Fac Name</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_org_abn</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Clinic/Fac ABN</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_org_acn</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Clinic/Fac ACN</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_org_addr</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Clinic/Fac Address</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_org_addr_tabbedx1</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Clinic/Fac Address Tabbed</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_org_phone</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Clinic/Fac Phone Nbr</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_org_office_fax</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Clinic/Fac Fax Nbr</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_org_web</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Clinic/Fac Website</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">bk_org_email</td><td style="width:10px;"></td><td style="width:10px;white-space:nowrap;">Booking Clinic/Fac Email Address</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td style="width:10px;white-space:nowrap;">pt_hc_card_nbr</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Active Health Card Nbr</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">pt_hc_card_name</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Active Health Card Name</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">pt_hc_card_refsigneddate</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Active Health Card EPC/Ref. Signed Date</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">pt_epc_expire_date</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Active Health Card EPC Expire Date</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">pt_epc_count_remaining</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Active Health Card EPC Count Remaining</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                        <tr><td style="width:10px;white-space:nowrap;">ref_name</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Referrer Full Name</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">ref_title</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Referrer Title</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">ref_firstname</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Referrer First Name</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">ref_middlename</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Referrer Middlename</td></tr>
                                        <tr><td style="width:10px;white-space:nowrap;">ref_surname</td><td style="width:10px;white-space:nowrap;"></td><td style="width:10px;white-space:nowrap;">Referrer Surname</td></tr>
                                        <tr><td colspan="3">&nbsp;</td></tr>

                                    </table>

                                </div>
                            </div>

                            <div style="height:12px;"></div>

                    </td>
                </tr>

                <tr>

                    <td></td>

                    <td>
                        <a name="invoice_templates_tag"></a><strong>Invoice Templates</strong>
                        <br /><br />

                        <table style="width:100%">
                            <tr id="rowInvoice" runat="server">
                                <td>
                                    <asp:Label ID="lblFileNameInvoice" runat="server"></asp:Label> (<asp:LinkButton ID="lnkFileNameInvoice" runat="server" OnClick="btnDownload_Click" Text='download' />)
                                </td>
                            </tr>
                            <tr id="rowInvoicePrivate" runat="server">
                                <td>
                                    <asp:Label ID="lblFileNamePrivateInvoice" runat="server"></asp:Label> (<asp:LinkButton ID="lnkFileNamePrivateInvoice" runat="server" OnClick="btnDownload_Click" Text='download' />) [<i>for non-booking invoices</i>]
                                </td>
                            </tr>
                            <tr id="rowInvoiceAC" runat="server">
                                <td>
                                    <asp:Label ID="lblFileNameInvoiceAC" runat="server"></asp:Label> (<asp:LinkButton ID="lnkFileNameInvoiceAC" runat="server" OnClick="btnDownload_Click" Text='download' />)
                                </td>
                            </tr>

                            <tr style="height:10px;"><td></td></tr>

                            <tr id="rowInvoiceOutstanding" runat="server">
                                <td>
                                    <asp:Label ID="lblFileNameInvoiceOutstanding" runat="server"></asp:Label> (<asp:LinkButton ID="lnkFileNameInvoiceOutstanding" runat="server" OnClick="btnDownload_Click" Text='download' />)
                                </td>
                            </tr>
                            <tr id="rowInvoiceOutstandingAC" runat="server">
                                <td>
                                    <asp:Label ID="lblFileNameInvoiceOutstandingAC" runat="server"></asp:Label> (<asp:LinkButton ID="lnkFileNameInvoiceOutstandingAC" runat="server" OnClick="btnDownload_Click" Text='download' />)
                                </td>
                            </tr>

                            <tr style="height:15px;" id="rowACTreatmentListSpace" runat="server"><td></td></tr>

                            <tr id="rowTreatmentList" runat="server">
                                <td>
                                    <asp:Label ID="lblTreatmentList" runat="server"></asp:Label> (<asp:LinkButton ID="lnkTreatmentList" runat="server" OnClick="btnDownload_Click" Text='download' />)
                                </td>
                            </tr>
                            <tr id="rowACTreatmentList" runat="server">
                                <td>
                                    <asp:Label ID="lblACTreatmentList" runat="server"></asp:Label> (<asp:LinkButton ID="lnkACTreatmentList" runat="server" OnClick="btnDownload_Click" Text='download' />)
                                </td>
                            </tr>

                            <tr style="height:15px;"><td></td></tr>

                            <tr id="rowBlankTemplate" runat="server">
                                <td>
                                    <asp:Label ID="lblBlankTemplate" runat="server"></asp:Label> (<asp:LinkButton ID="lnkBlankTemplate" runat="server" OnClick="btnDownload_Click" Text='download' />)
                                </td>
                            </tr>
                            <tr id="rowBlankTemplateAC" runat="server">
                                <td>
                                    <asp:Label ID="lblBlankTemplateAC" runat="server"></asp:Label> (<asp:LinkButton ID="lnkBlankTemplateAC" runat="server" OnClick="btnDownload_Click" Text='download' />)
                                </td>
                            </tr>

                        </table>

                    </td>

                    <td style="width:75px"></td>

                    <td id="row_invoice_upload" runat="server">
                        <strong>Upload Files</strong>
                        <br /><br />
                        <table>
                            <tr>
                                <td>
                                    <asp:FileUpload ID="FileUpload2" runat="server" class="multi" accept="dot|doc|docx" style="width:80px;overflow:hidden;" />
                                    <div style="line-height:9px;">&nbsp;</div>
                                    <asp:Button ID="btnUploadInvoice" runat="server" Text="Upload All" OnClientClick="javascript:if (!confirm('This will permanently overwrite existing file(s) which are not recoverable. Are you sure?')) return false;" onclick="btnUploadInvoice_Click" />
                                    <br />
                                    <br />
                                    <asp:Label ID="lblUploadInvoiceMessage" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>

                <tr>
                    <td colspan="4"><div style="height:25px;"></div></td>
                </tr>

                <tr>

                    <td></td>

                    <td colspan="3">

                        <table>
                            <tr>
                                <td style="color:blue;">
                                    <b>*</b> Note that for the invoice templates:
                                    <ul>
                                        <li>For best results, do not change the format of these documents. Changes should be only to the letter head.</li>
                                        <li>They use a special format and different fields than other letters.</li>
                                        <li>Do not remove the table containing the words "Item", "Quantity", and "Price" as that is where the invoice lines go.</li>
                                        <li>Do not add another table to the document before the first table - the invoice lines will go into the first table the document has.</li>
                                    </ul>

                                </td>
                            </tr>
                        </table>

                    </td>

                </tr>
            </table>





        </div>
    </div>


</asp:Content>



