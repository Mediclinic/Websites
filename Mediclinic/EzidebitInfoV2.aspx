<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="EzidebitInfoV2.aspx.cs" Inherits="EzidebitInfoV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
<script>

    addLoadEvent(function () { show_all(false); });

    function show_all(do_show) {
        var table = document.getElementById("tbl_output");
        if (table == null)
            return;

        var ncols = table.rows[0].cells.length;
        for (var i = 6; i < ncols; i++) {
            show_hide_column(i, do_show);
        }
    }

    function show_hide_column(col_no, do_show) {
        var stl = (do_show) ? '' : 'none';
        var table = document.getElementById("tbl_output");
        if (table == null)
            return;

        for (var i = 0, row; row = table.rows[i]; i++) {
            row.cells[col_no].style.display = stl;
        }
    }

</script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Ezidebit Info</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <div class="user_login_form">

                <!-- use panel instead of div so that we can put a default button when hitting enter in the date fields -->
                <asp:Panel runat="server" class="border_top_bottom" DefaultButton="btnSearch">

                    <table class="block_center">
                        <tr>
                            <td><asp:Label ID="lblSearchDate" runat="server">Start Date: </asp:Label></td>
                            <td><asp:TextBox ID="txtStartDate" runat="server" Columns="10"></asp:TextBox></td>
                            <td><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>

                            <td style="width:15px"></td>

                            <td><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                            <td><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                            <td><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>

                            <td style="width:15px"></td>

                            <td><asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" /></td>

                        </tr>
                    </table>
                </asp:Panel>

            </div>


            <center>
                <a href="javascript:void(0)" onclick="show_all(true);return false;">Show</a> / <a href="javascript:void(0)" onclick="show_all(false);return false;">Hide</a>
            </center>
            

            <center>
                <asp:Label ID="lblOutput" runat="server"></asp:Label>
            </center>

            <br />
            <br />

    
        </div>
    </div>

</asp:Content>



