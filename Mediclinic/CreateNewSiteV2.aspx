<%@ Page Title="Create New Site" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="CreateNewSiteV2.aspx.cs" Inherits="CreateNewSiteV2" %>

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

                            <div style="height:12px;"></div>

                            <center><h4>Create New DB</h4></center>

                            <div style="height:8px;"></div>

                            <table style="text-align:left;">
                                <tr>
                                    <td>Company Name</td>
                                    <td style="min-width:15px;"></td>
                                    <td><asp:TextBox ID="txtCompanyName" runat="server" Columns="40" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Firstname</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtInitialStaffFirstname" runat="server" Columns="40" onkeyup="capitalize_first(this);" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Surname</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtInitialStaffSurname" runat="server" Columns="40" onkeyup="capitalize_first(this);" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Personal Email</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtPersonalEmail" runat="server" Columns="40" onblur="if (document.getElementById('txtCompanyEmail').value == '') { document.getElementById('txtCompanyEmail').value = document.getElementById('txtPersonalEmail').value; }; SetEnd('txtCompanyEmail');" ></asp:TextBox> deleted bookings alerts, etc, sent here</td>
                                </tr>
                                <tr>
                                    <td>Company Email</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtCompanyEmail" runat="server" Columns="40" ></asp:TextBox> referrer letters sent 'from' this address</td>
                                </tr>

                                <tr style="height:12px">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td>Auto Generate Medicare Claim Nbrs</td>
                                    <td></td>
                                    <td><asp:CheckBox ID="lblAutoGenerateMedicareInvoiceClaimNbrs" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td>Medicare Eclaims License Nbr</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtMedicareEclaimsLicenseNbr" runat="server" Columns="10" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>SMS Price</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtSMSPrice" runat="server" Columns="10" ></asp:TextBox> eg 0.15</td>
                                </tr>
                                <tr>
                                    <td>Max Nbr Providers</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtMaxNbrProviders" runat="server" Columns="10" ></asp:TextBox> eg 5</td>
                                </tr>

                                <tr style="height:12px">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td>Banner Message</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtBannerMessage" runat="server" Columns="40" ></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Show Banner Message</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlShowBannerMessage" runat="server">
                                            <asp:ListItem Text="No" Value="False" />
                                            <asp:ListItem Text="Yes" Value="True" />
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
                                    <td colspan="3" style="text-align:center;">
                                        <asp:Button ID="btnSubmit" runat="server" Text="Create DB" OnClick="btnSubmit_Click" />
                                        <br />
                                    </td>
                                </tr>
                            </table>


                        </td>
                    </tr>
                </table>


                <asp:Label ID="lblResultMessage" runat="server"></asp:Label>


            </center>

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>

        </div>
    </div>

</asp:Content>



