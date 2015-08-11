<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default1.aspx.cs" Inherits="_Default1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

    <script type="text/javascript">

        // if page loaded more than X mins ago, and idle more than Y seoncs
        // then redirect but with scroll/etc updated

        var pageLoadTime = new Date().valueOf();
        var pageIdleTime = new Date().valueOf();

        document.onkeypress = resetIdleTime;
        document.onmousemove = resetIdleTime;
        window.onload = checkPageLoadAndIdleTime;

        function resetIdleTime() {
            pageIdleTime = new Date().valueOf();
        }

        function checkPageLoadAndIdleTime() {

            var loadedSeconds = (new Date().valueOf() - pageLoadTime) / 1000;
            var idleSeconds   = (new Date().valueOf() - pageIdleTime) / 1000;

            document.getElementById('txtLoadTime').value = Math.floor(loadedSeconds);
            document.getElementById('txtIdleTime').value = Math.floor(idleSeconds);


            var min_loaded_seconds = 10; // 600 = 10 mins
            var min_idle_seconds   = 3;  // 60 = 1 min 

            if (loadedSeconds > min_loaded_seconds && idleSeconds > min_idle_seconds) {

                /*
                if (confirm("Page data needs to be refreshed. Click OK to refresh.")) {

                    window.location.href = window.location.href;

                    //
                    // but update with scroll val's etc ... same as refresh button!!
                    //
                }
                */


                // update as page just loaded so dont show msg for another 10 mins
                pageLoadTime = new Date().valueOf();
            }

            setTimeout(checkPageLoadAndIdleTime, 1000);  // change to every 15 seconds maybe...
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <h2>
        Welcome <asp:Label ID="lblStaffName" runat="server"/>!
    </h2>

    <table id="tbl_bleh" runat="server" class="hiddencol">
        <tr>
            <td>Loaded: </td>
            <td><asp:TextBox ID="txtLoadTime" Columns="8" runat="server"/></td>
        </tr>
        <tr>
            <td>Idle: </td>
            <td><asp:TextBox ID="txtIdleTime" Columns="8" runat="server"/></td>
        </tr>
    </table>


    <br /><br />

    <asp:Button ID="btnBlah" runat="server" Text="Async Test" OnClick="btnBlah_Click" />
    <br />
    <asp:Label ID="lblBlah" runat="server"></asp:Label>


    <br /><br />
    <asp:Button ID="Button2" runat="server" Text="Test2" OnClick="Button2_Click" CausesValidation="True" ValidationGroup="validationSummary" />
    <br />
    <asp:Label ID="Label2" runat="server"></asp:Label>

    <br /><br />
    <asp:Button ID="Button3" runat="server" Text="Test3" OnClick="Button3_Click" />
    <br />
    <asp:Label ID="Label3" runat="server"></asp:Label>

    <br /><br />


    <asp:Panel ID="pnlBlah" runat="server">
    <table border="0" cellpadding="0" cellspacing="0">



        <tr>
            <td>Initial Staff Member Firstname</td>
            <td><asp:TextBox ID="txtInitialStaffFirstname" runat="server" Columns="40" >John</asp:TextBox></td>
        </tr>
        <tr>
            <td>Initial Staff Member Surname</td>
            <td><asp:TextBox ID="txtInitialStaffSurname" runat="server" Columns="40" onblur="document.getElementById('txtInitialStaffLogin').value = document.getElementById('txtInitialStaffFirstname').value.toLowerCase().replace(/\s+/g, '') + document.getElementById('txtInitialStaffSurname').value.toLowerCase().replace(/\s+/g, '');" >Smith</asp:TextBox></td>
        </tr>
        <tr>
            <td>Initial Staff Member Login</td>
            <td><asp:TextBox ID="txtInitialStaffLogin" runat="server" Columns="40" ></asp:TextBox></td>
        </tr>
        <tr>
            <td>Medicare Eclaims License Nbr</td>
            <td><asp:TextBox ID="txtMedicareEclaimsLicenseNbr" runat="server" Columns="10" >12345</asp:TextBox></td>
        </tr>
        <tr>
            <td>SMS Price</td>
            <td><asp:TextBox ID="txtSMSPrice" runat="server" Columns="10" >0.15</asp:TextBox></td>
        </tr>
        <tr>
            <td>Max Nbr Providers</td>
            <td><asp:TextBox ID="txtMaxNbrProviders" runat="server" Columns="10" >0</asp:TextBox></td>
        </tr>
        <tr>
            <td>Allow Add Site Clinic</td>
            <td>
                <asp:DropDownList ID="ddlAllowAddSiteClinic" runat="server">
                    <asp:ListItem Text="No" Value="0" />
                    <asp:ListItem Text="Yes" Value="1" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>Allow Add Site Aged Care</td>
            <td>
                <asp:DropDownList ID="ddlAllowAddSiteAgedCare" runat="server">
                    <asp:ListItem Text="No" Value="0" />
                    <asp:ListItem Text="Yes" Value="1" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>Banner Message</td>
            <td><asp:TextBox ID="txtBannerMessage" runat="server" Columns="40" ></asp:TextBox></td>
        </tr>
        <tr>
            <td>Show Banner Message</td>
            <td>
                <asp:DropDownList ID="ddlShowBannerMessage" runat="server">
                    <asp:ListItem Text="No" Value="False" />
                    <asp:ListItem Text="Yes" Value="True" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>Clinic Site Name</td>
            <td><asp:TextBox ID="txtSiteName" runat="server" Columns="40" >Podiatryclinics Pty Ltd</asp:TextBox></td>
        </tr>
        <tr>
            <td>Email Sending - From Name</td>
            <td><asp:TextBox ID="txtEmail_FromName" runat="server" Columns="40" >PODIATRYCLINICS</asp:TextBox></td>
        </tr>
        <tr>
            <td>Email Sending - From Email</td>
            <td><asp:TextBox ID="txtEmail_FromEmail" runat="server" Columns="40" >info@podiatryclinics.com.au</asp:TextBox></td>
        </tr>
        <tr>
            <td>Admin Alert Email<br />eg deleted bookings, etc</td>
            <td><asp:TextBox ID="txtAdminAlertEmail_To" runat="server" Columns="40" >info@podiatryclinics.com.au</asp:TextBox></td>
        </tr>

        <tr style="height:20px">
            <td colspan="3"></td>
        </tr>
        <tr>
            <td>Field 1 (eg Podiatry)</td>
            <td><asp:TextBox ID="txtField1" runat="server" Columns="40"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Field 2 (eg Physiotherapy)</td>
            <td><asp:TextBox ID="txtField2" runat="server" Columns="40"></asp:TextBox></td>
        </tr>
    </table>
    </asp:Panel>


    <br />
    <br />
    <asp:Button ID="Button4" runat="server" Text="Create DB" OnClick="Button4_Click" />
    <br />
    <asp:Label ID="Label4" runat="server"></asp:Label>

</asp:Content>

