<%@ Page Title="Create New Site" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="CreateNewCustomerSiteV2.aspx.cs" Inherits="CreateNewCustomerSiteV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function capitalize_first(txtbox) {
            txtbox.value = txtbox.value.charAt(0).toUpperCase() + txtbox.value.slice(1);
        }

        function adjustOffset(el, offset) {
            var val = el.value, newOffset = offset;
            if (val.indexOf("\r\n") > -1) {
                var matches = val.replace(/\r\n/g, "\n").slice(0, offset).match(/\n/g);
                newOffset += matches ? matches.length : 0;
            }
            return newOffset;
        };
        setCaretToPos = function (input, selectionStart, selectionEnd) {
            input.focus();
            if (input.setSelectionRange) {
                selectionStart = adjustOffset(input, selectionStart);
                selectionEnd = adjustOffset(input, selectionEnd);
                input.setSelectionRange(selectionStart, selectionEnd);

            } else if (input.createTextRange) {
                var range = input.createTextRange();
                range.collapse(true);
                range.moveEnd('character', selectionEnd);
                range.moveStart('character', selectionStart);
                range.select();
            }
        };
        function SetEnd(elemId) {
            var elem = document.getElementById(elemId);
            setTimeout(setCaretToPos(elem, elem.value.length, elem.value.length), 100);
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Create New Site</span></div>
        <div class="main_content_with_header">

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <center>

                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblResultMessage" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>

                <table id="main_table" runat="server">
                    <tr>
                        <td>

                            <div style="height:25px;"></div>

                            <table style="text-align:left;">

                                <tr>
                                    <td>Company Name <span class="required_asterisk">*</span></td>
                                    <td style="min-width:15px;"></td>
                                    <td><asp:TextBox ID="txtCompanyName" runat="server" Columns="40" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Firstname <span class="required_asterisk">*</span></td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtInitialStaffFirstname" runat="server" Columns="40" onkeyup="capitalize_first(this);" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Surname <span class="required_asterisk">*</span></td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtInitialStaffSurname" runat="server" Columns="40" onkeyup="capitalize_first(this);" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Company Email <span class="required_asterisk">*</span></td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtCompanyEmail" runat="server" Columns="40" ></asp:TextBox></td>
                                </tr>

                                <tr style="height:12px">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td>Address Line 1 <span class="required_asterisk">*</span></td>
                                    <td style="min-width:15px;"></td>
                                    <td><asp:TextBox ID="txtAddrLine1" runat="server" Columns="40" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Address Line 2</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtAddrLine2" runat="server" Columns="40" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>City</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtAddrCity" runat="server" Columns="40" onkeyup="capitalize_first(this);" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>State / Province / Region</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtAddrStateProvinceRegion" runat="server" Columns="40" onkeyup="capitalize_first(this);" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Postal / Zip Code</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtAddrPostcode" runat="server" Columns="40" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Country</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtAddrCountry" runat="server" Columns="40" onkeyup="capitalize_first(this);" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Phone Nbr (incl area code) <span class="required_asterisk">*</span></td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtPhoneNbr" runat="server" Columns="40" ></asp:TextBox></td>
                                </tr>

                                <tr style="height:12px">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td>Max Nbr Providers <span class="required_asterisk">*</span></td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlMaxNbrProviders" runat="server">
                                            <asp:ListItem Text="1" Value="1" />
                                            <asp:ListItem Text="5" Value="5" />
                                            <asp:ListItem Text="10" Value="10" />
                                            <asp:ListItem Text="20" Value="20" />
                                            <asp:ListItem Text="50" Value="50" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>

                                <tr style="height:12px">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td>Field 1</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlField1" runat="server" style="width:200px;"></asp:DropDownList>
                                        &nbsp;&nbsp;
                                        <asp:RadioButton ID="radioField1" runat="server" GroupName="setAsProvider" Text="&nbsp;Set yourself as this?" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Field 2</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlField2" runat="server" style="width:200px;"></asp:DropDownList>
                                        &nbsp;&nbsp;
                                        <asp:RadioButton ID="radioField2" runat="server" GroupName="setAsProvider" Text="&nbsp;Set yourself as this?" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Field 3</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlField3" runat="server" style="width:200px;"></asp:DropDownList>
                                        &nbsp;&nbsp;
                                        <asp:RadioButton ID="radioField3" runat="server" GroupName="setAsProvider" Text="&nbsp;Set yourself as this?" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Field 4</td>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtField4" runat="server" style="width:200px;"></asp:TextBox>
                                        &nbsp;&nbsp;
                                        <asp:RadioButton ID="radioField4" runat="server" GroupName="setAsProvider" Text="&nbsp;Set yourself as this?" />
                                    </td>
                                </tr>

                                <tr style="height:20px">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td>Are you also one of the providers?</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="chkIsProvider" runat="server" Text="" /></td>
                                </tr>

                                <tr style="height:20px">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td colspan="3" style="text-align:center;">
                                        <asp:Button ID="btnSubmit" runat="server" Text="Create Site" OnClick="btnSubmit_Click" />
                                        <br />
                                    </td>
                                </tr>
                            </table>


                        </td>
                    </tr>
                </table>

            </center>

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>

        </div>
    </div>

</asp:Content>



