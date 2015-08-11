<%@ Page Title="EPC Detail" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="UploadTooLargeV2.aspx.cs" Inherits="UploadTooLargeV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">&nbsp;</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

            <div style="height:8px;"></div>

            <center>
                <table style="text-align:center;">
                    <tr>
                        <td>

                            <h2>
                                Uploaded File Is Too Large
                            </h2>

                            <br />
                            <br />
                            <br />
                            <br />

                            <a href="javascript:history.back()">Back</a>

                        </td>
                    </tr>
                </table>
            </center>

            <br /><br />

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>
        </div>
    </div>


</asp:Content>



