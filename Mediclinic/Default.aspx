<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_DefaultV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">&nbsp;</span></div>
        <div class="main_content" id="main_content" runat="server" style="background: url(../imagesV2/login_bg.png) center top no-repeat #EDEDED;">
            <div class="text-center">

                <div class="page_title">
                    <h3>Welcome <asp:Label ID="lblStaffName" runat="server"/>!</h3>
                    <div style="height:5px;"></div>
                    <h4>You Are Logged In To  <asp:Label ID="lblSiteName" runat="server"/></h4>
                </div>
                <br />


                <noscript>
                    <h4>
                        <font color="red">
                            <br />
                            Javascript has not been enabled in your browser.
                            <div style="height:10px;"></div>
                            Please enable JavaScript for mediclinic software to work correctly.
                            <div style="height:10px;"></div>
                            Thank you.
                        </font>
                    </h4>
                </noscript>

              
               

                <asp:Label ID="lblTest" runat="server"></asp:Label>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>



