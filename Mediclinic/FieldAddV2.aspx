<%@ Page Title="Create New Site" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="FieldAddV2.aspx.cs" Inherits="FieldAddV2" %>

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
        <div class="page_title"><span id="lblHeading" runat="server">Add Field</span></div>
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

                            <div style="height:8px;"></div>

                            <table style="text-align:left;">
                                <tr>
                                    <td style="vertical-align:top;">Current Fields</td>
                                    <td style="min-width:15px;"></td>
                                    <td><asp:Label ID="lblCurrentFields" runat="server"></asp:Label></td>
                                </tr>

                                <tr style="height:30px">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td>Field To Add</td>
                                    <td></td>
                                    <td>
                                        <asp:DropDownList ID="ddlField1" runat="server" style="width:200px;"></asp:DropDownList>
                                        <br />
                                        Or
                                        <br />
                                        <asp:TextBox ID="txtField1" runat="server" style="width:200px;"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr style="height:20px">
                                    <td colspan="3"></td>
                                </tr>

                                <tr>
                                    <td colspan="3" style="text-align:center;">
                                        <asp:Button ID="btnSubmit" runat="server" Text="Add Field" OnClick="btnSubmit_Click" />
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



