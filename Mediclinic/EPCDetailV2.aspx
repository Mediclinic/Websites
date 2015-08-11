<%@ Page Title="EPC Detail" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="EPCDetailV2.aspx.cs" Inherits="EPCDetailV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function show_page_load_message() {
            if (getUrlVars()["type"] == "add") {

                // delay so it is only shown if it is taking a long time (ie if a last treatment letter is being generated)
                setTimeout(function () {
                    show_hide('loadingDiv', true);
                }, 650);

            }
        }
        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }
        function show_hide(id, show) {
            obj = document.getElementById(id);
            obj.style.display = show ? "" : "none";
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">EPC Info</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="EditHealthCardValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="EditHealthCardValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <div id="loadingDiv" runat="server" style="display:none">
                    <table>
                        <tr style="vertical-align:middle">
                            <td style="color:blue"><asp:Label ID="lblGenerateLettersMsg" runat="server">Please wait while we generate any previous EPC Letters.</asp:Label></td>
                            <td><img src="images/loading_circle_small.gif" alt="Loading..."/></td>
                        </tr>
                    </table>
                </div>

                <br />
                <br />
                <table id="maintable" runat="server">

                    <tr>
                        <td style="vertical-align:top">


                            <table>
                                <tr id="idRow" runat="server">
                                    <td>ID</td>
                                    <td style="width:12px"></td>
                                    <td style="width:200px"><asp:Label ID="lblId" runat="server"></asp:Label></td>
                                    <td></td>
                                    <td></td>
                                </tr>

                                <tr>
                                    <td>Referral Received</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlDateReferralReceived_Day" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDateReferralReceived_Month" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDateReferralReceived_Year" runat="server"></asp:DropDownList>
                                    </td>
                                    <td><asp:CustomValidator ID="ddlDateReferralReceivedAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                                ControlToValidate="ddlDateReferralReceived_Day"
                                                OnServerValidate="DateReferralReceivedAllOrNoneCheck"
                                                ErrorMessage="Date Received must have each of day/month/year selected, or all set to '--'"
                                                Display="Dynamic"
                                                ValidationGroup="EditHealthCardValidationSummary" Enabled="False">*</asp:CustomValidator>
                                        <asp:CustomValidator ID="ddlDateReferralReceivedAllSet" runat="server"  CssClass="failureNotification"  
                                                ControlToValidate="ddlDateReferralReceived_Day"
                                                OnServerValidate="DateReferralReceivedAllCheck"
                                                ErrorMessage="Date Received must have each of day/month/year set & be a valid date"
                                                Display="Dynamic"
                                                ValidationGroup="EditHealthCardValidationSummary">*</asp:CustomValidator>
                                        <asp:CustomValidator ID="ddlDateReferralReceivedDateNotAfterToday" runat="server"  CssClass="failureNotification"  
                                                ControlToValidate="ddlDateReferralReceived_Day"
                                                OnServerValidate="DateReferralReceivedDateNotAfterTodayCheck"
                                                ErrorMessage="Date Received can not be after today"
                                                Display="Dynamic"
                                                ValidationGroup="EditHealthCardValidationSummary">*</asp:CustomValidator>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>Referral Signed</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlDateReferralSigned_Day" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDateReferralSigned_Month" runat="server"></asp:DropDownList>
                                        <asp:DropDownList ID="ddlDateReferralSigned_Year" runat="server"></asp:DropDownList>
                                    </td>
                                    <td><asp:CustomValidator ID="ddlDateReferralSignedAllOrNoneSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDateReferralSigned_Day"
                                            OnServerValidate="DateReferralSignedAllOrNoneCheck"
                                            ErrorMessage="Date Signed must have each of day/month/year selected, or all set to '--'"
                                            Display="Dynamic"
                                            ValidationGroup="EditHealthCardValidationSummary" Enabled="false">*</asp:CustomValidator>
                                        <asp:CustomValidator ID="ddlDateReferralSignedAllSet" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDateReferralSigned_Day"
                                            OnServerValidate="DateReferralSignedAllCheck"
                                            ErrorMessage="Date Signed must have each of day/month/year set & be a valid date"
                                            Display="Dynamic"
                                            ValidationGroup="EditHealthCardValidationSummary">*</asp:CustomValidator>
                                        <asp:CustomValidator ID="ddlDateReferralSignedDateNotAfterToday" runat="server"  CssClass="failureNotification"  
                                            ControlToValidate="ddlDateReferralSigned_Day"
                                            OnServerValidate="DateReferralSignedDateNotAfterTodayCheck"
                                            ErrorMessage="Date Signed can not be after today"
                                            Display="Dynamic"
                                            ValidationGroup="EditHealthCardValidationSummary">*</asp:CustomValidator>

                                    </td>
                                    <td></td>
                                </tr>

                                <tr height="10">
                                    <td></td>
                                </tr>

                                <tr id="tr_epc_remaining">
                                    <td>
                                        <asp:Repeater id="lstEPCRemaining" runat="server">
                                            <HeaderTemplate>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="nowrap"><asp:Label ID="lblFieldDescr" Text='<%# Eval("field_desc") + " services" %>' runat="server"/></td>
                                                    <td></td>
                                                    <td><asp:DropDownList ID="ddlNumServicesRamining" runat="server" SelectedValue='<%# Bind("num_remaining") %>' BackColor="LightGoldenrodYellow">
                                                            <asp:ListItem Value="0">0</asp:ListItem>
                                                            <asp:ListItem Value="1">1</asp:ListItem>
                                                            <asp:ListItem Value="2">2</asp:ListItem>
                                                            <asp:ListItem Value="3">3</asp:ListItem>
                                                            <asp:ListItem Value="4">4</asp:ListItem>
                                                            <asp:ListItem Value="5">5</asp:ListItem>
                                                            <asp:ListItem Value="6">6</asp:ListItem>
                                                            <asp:ListItem Value="7">7</asp:ListItem>
                                                            <asp:ListItem Value="8">8</asp:ListItem>
                                                            <asp:ListItem Value="9">9</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:HiddenField ID="lblFieldID" Value='<%# Eval("field_id") %>' runat="server" />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </td>
                    
                    
                                </tr>





                                <tr id="spn_epcremaining_row" runat="server">
                                    <td colspan="5">

                                        <br />
                                        <strong>Number Of Services Remaining</strong>
                                        <br /><br />

                                        <asp:GridView ID="GrdHealthCardEPCRemaining" runat="server" 
                                             AutoGenerateColumns="False" DataKeyNames="epcremaining_health_card_epc_remaining_id" 
                                             OnRowCancelingEdit="GrdHealthCardEPCRemaining_RowCancelingEdit" 
                                             OnRowDataBound="GrdHealthCardEPCRemaining_RowDataBound" 
                                             OnRowEditing="GrdHealthCardEPCRemaining_RowEditing" 
                                             OnRowUpdating="GrdHealthCardEPCRemaining_RowUpdating" ShowFooter="True"  ShowHeader="False"
                                             OnRowCommand="GrdHealthCardEPCRemaining_RowCommand" 
                                             OnRowDeleting="GrdHealthCardEPCRemaining_RowDeleting" 
                                             OnRowCreated="GrdHealthCardEPCRemaining_RowCreated"
                                             AllowSorting="True" 
                                             OnSorting="GrdHealthCardEPCRemaining_Sorting" 
                                             GridLines="None"
                                             ClientIDMode="Predictable">

                                            <Columns> 

                                                <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="epcremaining_health_card_epc_remaining_id"> 
                                                    <EditItemTemplate> 
                                                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("epcremaining_health_card_epc_remaining_id") %>'></asp:Label>
                                                    </EditItemTemplate> 
                                                    <ItemTemplate> 
                                                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("epcremaining_health_card_epc_remaining_id") %>'></asp:Label> 
                                                    </ItemTemplate> 
                                                </asp:TemplateField> 

                                                <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" SortExpression="field_descr" FooterStyle-VerticalAlign="Top"> 
                                                    <EditItemTemplate> 
                                                        <asp:Label ID="lblField" runat="server" Text='<%# Eval("field_descr") %>'></asp:Label> 
                                                    </EditItemTemplate> 
                                                    <ItemTemplate> 
                                                        <asp:Label ID="lblField" runat="server" Text='<%# Eval("field_descr") %>'></asp:Label> 
                                                    </ItemTemplate> 
                                                    <FooterTemplate> 
                                                        <asp:DropDownList ID="ddlNewField" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList>
                                                    </FooterTemplate> 
                                                </asp:TemplateField> 

                                                <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                    <ItemTemplate> 
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    </ItemTemplate> 
                                                    <FooterTemplate> 
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    </FooterTemplate> 
                                                </asp:TemplateField> 

                                                <asp:TemplateField HeaderText="Remaining" HeaderStyle-HorizontalAlign="Left" SortExpression="epcremaining_num_services_remaining" FooterStyle-VerticalAlign="Top"> 
                                                    <EditItemTemplate> 
                                                        <asp:DropDownList ID="ddlNumServicesRemaining" runat="server"> </asp:DropDownList>
                                                    </EditItemTemplate> 
                                                    <ItemTemplate> 
                                                        <asp:Label ID="lblNumServicesRemaining" runat="server" Text='<%# Eval("epcremaining_num_services_remaining") %>'></asp:Label> 
                                                    </ItemTemplate> 
                                                    <FooterTemplate> 
                                                        <asp:DropDownList ID="ddlNewNumServicesRemaining" runat="server"> </asp:DropDownList>
                                                    </FooterTemplate> 
                                                </asp:TemplateField> 

                                                <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                    <ItemTemplate> 
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    </ItemTemplate> 
                                                    <FooterTemplate> 
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    </FooterTemplate> 
                                                </asp:TemplateField> 


                                                <asp:TemplateField HeaderText="Edit" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                                    <EditItemTemplate> 
                                                        <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="EditHealthCardEPCRemainingValidationSummary"></asp:LinkButton> 
                                                        <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                                    </EditItemTemplate> 
                                                    <FooterTemplate> 
                                                        <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="AddHealthCardEPCRemainingValidationGroup"></asp:LinkButton> 
                                                    </FooterTemplate> 
                                                    <ItemTemplate> 
                                                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton> 
                                                    </ItemTemplate> 
                                                </asp:TemplateField> 

                                                <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left"> 
                                                    <ItemTemplate> 
                                                        &nbsp;
                                                    </ItemTemplate> 
                                                    <FooterTemplate> 
                                                        &nbsp;
                                                    </FooterTemplate> 
                                                </asp:TemplateField> 

                                                <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" ShowHeader="True" /> 


                                            </Columns> 

                                        </asp:GridView>

                                    </td>
                                </tr>

                                <tr height="40">
                                    <td colspan="5"></td>
                                </tr>

                                <tr>
                                    <td colspan="5" align="center">
                                        <br />  
                                        <asp:Button ID="btnSubmit" runat="server" Text="Button" onclick="btnSubmit_Click" OnClientClick="show_page_load_message();" CausesValidation="True" ValidationGroup="EditHealthCardValidationSummary" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" onclick="btnCancel_Click" Visible="False" />
                                        <br />              
                                    </td>
                                </tr>

                            </table>




                        </td>
                    </tr>
                </table>


            </center>

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>


</asp:Content>



