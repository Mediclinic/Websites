<%@ Page Title="Claim Numbers Allocation" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="ClaimNumbersAllocationV2.aspx.cs" Inherits="ClaimNumbersAllocationV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">

        function select_all_rows(chkBox) { // this uses a plain html checkbox, so it is an actual checkbox passed in
            var chkBoxList = document.getElementById("<%= claimNumberAllocationChkBoxList.ClientID %>");
            var chkBoxCount = chkBoxList.getElementsByTagName("input");
            for (var i = 0; i < chkBoxCount.length; i++) {
                chkBoxCount[i].checked = chkBox.checked;
            }

            show_hide_revert_btn(true);

            return false;
        }

        function revert_all_rows() { // this uses a plain html checkbox, so it is an actual checkbox passed in
            var chkBoxList = document.getElementById("<%= claimNumberAllocationChkBoxList.ClientID %>");
            var chkBoxCount = chkBoxList.getElementsByTagName("input");
            for (var i = 0; i < chkBoxCount.length; i++) {
                chkBoxCount[i].checked = chkBoxCount[i].parentNode.getAttribute('style').indexOf("font-weight:bold") !== -1;
            }

            show_hide_revert_btn(false);
            document.getElementById("chkCheckAllNone").checked = false;

            return false;
        }

        function show_hide_revert_btn(show) {
            document.getElementById("btnRevertSection").className = show ? "" : "hiddencol_keep_space";
        }


    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Claim Numbers Allocation</asp:Label></div>
        <div class="main_content" style="padding:20px 5px;">

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>

                <table id="maintable" runat="server">
                    <tr>
                        <td style="text-align:left">
                            <asp:Label ID="lblLoadTimeMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                        </td>
                        <td style="text-align:right">
                            <label for="chkCheckAllNone" style="font-weight:normal;">All/None</label> <input type="checkbox" id="chkCheckAllNone" name="chkCheckAllNone" onchange="select_all_rows(this);return false;" />
                            <span id="btnRevertSection" class="hiddencol_keep_space">
                                &nbsp;&nbsp;<asp:Button ID="btnRevert" runat="server" Text="Revert" OnClientClick="revert_all_rows();return false;"  />
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align:center; font-family:'Courier New'">
                            <center>
                                <div id="autodivheight" class="divautoheight" style="height:500px; width: auto; padding-right: 17px;">

                                    <asp:CheckBoxList ID="claimNumberAllocationChkBoxList" runat="server" CellPadding="0" CellSpacing="5"
                      
                                           RepeatColumns="10"
                                           RepeatDirection="Vertical"
                                           RepeatLayout="Table"
                                           TextAlign="Right"
                                           CssClass="nowrap"
                                        >
                                        <asp:ListItem Value="A0000_A0999" onclick="show_hide_revert_btn(true);">&nbsp;A0000-A0999</asp:ListItem>
                                        <asp:ListItem Value="B0000_B0999" onclick="show_hide_revert_btn(true);">&nbsp;B0000-B0999</asp:ListItem>
                                        <asp:ListItem Value="C0000_C0999" onclick="show_hide_revert_btn(true);">&nbsp;C0000-C0999</asp:ListItem>
                                        <asp:ListItem Value="D0000_D0999" onclick="show_hide_revert_btn(true);">&nbsp;D0000-D0999</asp:ListItem>
                                        <asp:ListItem Value="E0000_E0999" onclick="show_hide_revert_btn(true);">&nbsp;E0000-E0999</asp:ListItem>
                                        <asp:ListItem Value="F0000_F0999" onclick="show_hide_revert_btn(true);">&nbsp;F0000-F0999</asp:ListItem>
                                        <asp:ListItem Value="G0000_G0999" onclick="show_hide_revert_btn(true);">&nbsp;G0000-G0999</asp:ListItem>
                                        <asp:ListItem Value="H0000_H0999" onclick="show_hide_revert_btn(true);">&nbsp;H0000-H0999</asp:ListItem>
                                        <asp:ListItem Value="I0000_I0999" onclick="show_hide_revert_btn(true);">&nbsp;I0000-I0999</asp:ListItem>
                                        <asp:ListItem Value="J0000_J0999" onclick="show_hide_revert_btn(true);">&nbsp;J0000-J0999</asp:ListItem>
                                        <asp:ListItem Value="K0000_K0999" onclick="show_hide_revert_btn(true);">&nbsp;K0000-K0999</asp:ListItem>
                                        <asp:ListItem Value="L0000_L0999" onclick="show_hide_revert_btn(true);">&nbsp;L0000-L0999</asp:ListItem>
                                        <asp:ListItem Value="M0000_M0999" onclick="show_hide_revert_btn(true);">&nbsp;M0000-M0999</asp:ListItem>
                                        <asp:ListItem Value="N0000_N0999" onclick="show_hide_revert_btn(true);">&nbsp;N0000-N0999</asp:ListItem>
                                        <asp:ListItem Value="O0000_O0999" onclick="show_hide_revert_btn(true);">&nbsp;O0000-O0999</asp:ListItem>
                                        <asp:ListItem Value="P0000_P0999" onclick="show_hide_revert_btn(true);">&nbsp;P0000-P0999</asp:ListItem>
                                        <asp:ListItem Value="Q0000_Q0999" onclick="show_hide_revert_btn(true);">&nbsp;0000-Q0999</asp:ListItem>
                                        <asp:ListItem Value="R0000_R0999" onclick="show_hide_revert_btn(true);">&nbsp;R0000-R0999</asp:ListItem>
                                        <asp:ListItem Value="S0000_S0999" onclick="show_hide_revert_btn(true);">&nbsp;S0000-S0999</asp:ListItem>
                                        <asp:ListItem Value="T0000_T0999" onclick="show_hide_revert_btn(true);">&nbsp;T0000-T0999</asp:ListItem>
                                        <asp:ListItem Value="U0000_U0999" onclick="show_hide_revert_btn(true);">&nbsp;U0000-U0999</asp:ListItem>
                                        <asp:ListItem Value="V0000_V0999" onclick="show_hide_revert_btn(true);">&nbsp;V0000-V0999</asp:ListItem>
                                        <asp:ListItem Value="W0000_W0999" onclick="show_hide_revert_btn(true);">&nbsp;W0000-W0999</asp:ListItem>
                                        <asp:ListItem Value="X0000_X0999" onclick="show_hide_revert_btn(true);">&nbsp;X0000-X0999</asp:ListItem>
                                        <asp:ListItem Value="Y0000_Y0999" onclick="show_hide_revert_btn(true);">&nbsp;Y0000-Y0999</asp:ListItem>
                                        <asp:ListItem Value="Z0000_Z0999" onclick="show_hide_revert_btn(true);">&nbsp;Z0000-Z0999</asp:ListItem>

                                        <asp:ListItem Value="A1000_A1999" onclick="show_hide_revert_btn(true);">&nbsp;A1000-A1999</asp:ListItem>
                                        <asp:ListItem Value="B1000_B1999" onclick="show_hide_revert_btn(true);">&nbsp;B1000-B1999</asp:ListItem>
                                        <asp:ListItem Value="C1000_C1999" onclick="show_hide_revert_btn(true);">&nbsp;C1000-C1999</asp:ListItem>
                                        <asp:ListItem Value="D1000_D1999" onclick="show_hide_revert_btn(true);">&nbsp;D1000-D1999</asp:ListItem>
                                        <asp:ListItem Value="E1000_E1999" onclick="show_hide_revert_btn(true);">&nbsp;E1000-E1999</asp:ListItem>
                                        <asp:ListItem Value="F1000_F1999" onclick="show_hide_revert_btn(true);">&nbsp;F1000-F1999</asp:ListItem>
                                        <asp:ListItem Value="G1000_G1999" onclick="show_hide_revert_btn(true);">&nbsp;G1000-G1999</asp:ListItem>
                                        <asp:ListItem Value="H1000_H1999" onclick="show_hide_revert_btn(true);">&nbsp;H1000-H1999</asp:ListItem>
                                        <asp:ListItem Value="I1000_I1999" onclick="show_hide_revert_btn(true);">&nbsp;I1000-I1999</asp:ListItem>
                                        <asp:ListItem Value="J1000_J1999" onclick="show_hide_revert_btn(true);">&nbsp;J1000-J1999</asp:ListItem>
                                        <asp:ListItem Value="K1000_K1999" onclick="show_hide_revert_btn(true);">&nbsp;K1000-K1999</asp:ListItem>
                                        <asp:ListItem Value="L1000_L1999" onclick="show_hide_revert_btn(true);">&nbsp;L1000-L1999</asp:ListItem>
                                        <asp:ListItem Value="M1000_M1999" onclick="show_hide_revert_btn(true);">&nbsp;M1000-M1999</asp:ListItem>
                                        <asp:ListItem Value="N1000_N1999" onclick="show_hide_revert_btn(true);">&nbsp;N1000-N1999</asp:ListItem>
                                        <asp:ListItem Value="O1000_O1999" onclick="show_hide_revert_btn(true);">&nbsp;O1000-O1999</asp:ListItem>
                                        <asp:ListItem Value="P1000_P1999" onclick="show_hide_revert_btn(true);">&nbsp;P1000-P1999</asp:ListItem>
                                        <asp:ListItem Value="Q1000_Q1999" onclick="show_hide_revert_btn(true);">&nbsp;Q1000-Q1999</asp:ListItem>
                                        <asp:ListItem Value="R1000_R1999" onclick="show_hide_revert_btn(true);">&nbsp;R1000-R1999</asp:ListItem>
                                        <asp:ListItem Value="S1000_S1999" onclick="show_hide_revert_btn(true);">&nbsp;S1000-S1999</asp:ListItem>
                                        <asp:ListItem Value="T1000_T1999" onclick="show_hide_revert_btn(true);">&nbsp;T1000-T1999</asp:ListItem>
                                        <asp:ListItem Value="U1000_U1999" onclick="show_hide_revert_btn(true);">&nbsp;U1000-U1999</asp:ListItem>
                                        <asp:ListItem Value="V1000_V1999" onclick="show_hide_revert_btn(true);">&nbsp;V1000-V1999</asp:ListItem>
                                        <asp:ListItem Value="W1000_W1999" onclick="show_hide_revert_btn(true);">&nbsp;W1000-W1999</asp:ListItem>
                                        <asp:ListItem Value="X1000_X1999" onclick="show_hide_revert_btn(true);">&nbsp;X1000-X1999</asp:ListItem>
                                        <asp:ListItem Value="Y1000_Y1999" onclick="show_hide_revert_btn(true);">&nbsp;Y1000-Y1999</asp:ListItem>
                                        <asp:ListItem Value="Z1000_Z1999" onclick="show_hide_revert_btn(true);">&nbsp;Z1000-Z1999</asp:ListItem>

                                        <asp:ListItem Value="A2000_A2999" onclick="show_hide_revert_btn(true);">&nbsp;A2000-A2999</asp:ListItem>
                                        <asp:ListItem Value="B2000_B2999" onclick="show_hide_revert_btn(true);">&nbsp;B2000-B2999</asp:ListItem>
                                        <asp:ListItem Value="C2000_C2999" onclick="show_hide_revert_btn(true);">&nbsp;C2000-C2999</asp:ListItem>
                                        <asp:ListItem Value="D2000_D2999" onclick="show_hide_revert_btn(true);">&nbsp;D2000-D2999</asp:ListItem>
                                        <asp:ListItem Value="E2000_E2999" onclick="show_hide_revert_btn(true);">&nbsp;E2000-E2999</asp:ListItem>
                                        <asp:ListItem Value="F2000_F2999" onclick="show_hide_revert_btn(true);">&nbsp;F2000-F2999</asp:ListItem>
                                        <asp:ListItem Value="G2000_G2999" onclick="show_hide_revert_btn(true);">&nbsp;G2000-G2999</asp:ListItem>
                                        <asp:ListItem Value="H2000_H2999" onclick="show_hide_revert_btn(true);">&nbsp;H2000-H2999</asp:ListItem>
                                        <asp:ListItem Value="I2000_I2999" onclick="show_hide_revert_btn(true);">&nbsp;I2000-I2999</asp:ListItem>
                                        <asp:ListItem Value="J2000_J2999" onclick="show_hide_revert_btn(true);">&nbsp;J2000-J2999</asp:ListItem>
                                        <asp:ListItem Value="K2000_K2999" onclick="show_hide_revert_btn(true);">&nbsp;K2000-K2999</asp:ListItem>
                                        <asp:ListItem Value="L2000_L2999" onclick="show_hide_revert_btn(true);">&nbsp;L2000-L2999</asp:ListItem>
                                        <asp:ListItem Value="M2000_M2999" onclick="show_hide_revert_btn(true);">&nbsp;M2000-M2999</asp:ListItem>
                                        <asp:ListItem Value="N2000_N2999" onclick="show_hide_revert_btn(true);">&nbsp;N2000-N2999</asp:ListItem>
                                        <asp:ListItem Value="O2000_O2999" onclick="show_hide_revert_btn(true);">&nbsp;O2000-O2999</asp:ListItem>
                                        <asp:ListItem Value="P2000_P2999" onclick="show_hide_revert_btn(true);">&nbsp;P2000-P2999</asp:ListItem>
                                        <asp:ListItem Value="Q2000_Q2999" onclick="show_hide_revert_btn(true);">&nbsp;Q2000-Q2999</asp:ListItem>
                                        <asp:ListItem Value="R2000_R2999" onclick="show_hide_revert_btn(true);">&nbsp;R2000-R2999</asp:ListItem>
                                        <asp:ListItem Value="S2000_S2999" onclick="show_hide_revert_btn(true);">&nbsp;S2000-S2999</asp:ListItem>
                                        <asp:ListItem Value="T2000_T2999" onclick="show_hide_revert_btn(true);">&nbsp;T2000-T2999</asp:ListItem>
                                        <asp:ListItem Value="U2000_U2999" onclick="show_hide_revert_btn(true);">&nbsp;U2000-U2999</asp:ListItem>
                                        <asp:ListItem Value="V2000_V2999" onclick="show_hide_revert_btn(true);">&nbsp;V2000-V2999</asp:ListItem>
                                        <asp:ListItem Value="W2000_W2999" onclick="show_hide_revert_btn(true);">&nbsp;W2000-W2999</asp:ListItem>
                                        <asp:ListItem Value="X2000_X2999" onclick="show_hide_revert_btn(true);">&nbsp;X2000-X2999</asp:ListItem>
                                        <asp:ListItem Value="Y2000_Y2999" onclick="show_hide_revert_btn(true);">&nbsp;Y2000-Y2999</asp:ListItem>
                                        <asp:ListItem Value="Z2000_Z2999" onclick="show_hide_revert_btn(true);">&nbsp;Z2000-Z2999</asp:ListItem>

                                        <asp:ListItem Value="A3000_A3999" onclick="show_hide_revert_btn(true);">&nbsp;A3000-A3999</asp:ListItem>
                                        <asp:ListItem Value="B3000_B3999" onclick="show_hide_revert_btn(true);">&nbsp;B3000-B3999</asp:ListItem>
                                        <asp:ListItem Value="C3000_C3999" onclick="show_hide_revert_btn(true);">&nbsp;C3000-C3999</asp:ListItem>
                                        <asp:ListItem Value="D3000_D3999" onclick="show_hide_revert_btn(true);">&nbsp;D3000-D3999</asp:ListItem>
                                        <asp:ListItem Value="E3000_E3999" onclick="show_hide_revert_btn(true);">&nbsp;E3000-E3999</asp:ListItem>
                                        <asp:ListItem Value="F3000_F3999" onclick="show_hide_revert_btn(true);">&nbsp;F3000-F3999</asp:ListItem>
                                        <asp:ListItem Value="G3000_G3999" onclick="show_hide_revert_btn(true);">&nbsp;G3000-G3999</asp:ListItem>
                                        <asp:ListItem Value="H3000_H3999" onclick="show_hide_revert_btn(true);">&nbsp;H3000-H3999</asp:ListItem>
                                        <asp:ListItem Value="I3000_I3999" onclick="show_hide_revert_btn(true);">&nbsp;I3000-I3999</asp:ListItem>
                                        <asp:ListItem Value="J3000_J3999" onclick="show_hide_revert_btn(true);">&nbsp;J3000-J3999</asp:ListItem>
                                        <asp:ListItem Value="K3000_K3999" onclick="show_hide_revert_btn(true);">&nbsp;K3000-K3999</asp:ListItem>
                                        <asp:ListItem Value="L3000_L3999" onclick="show_hide_revert_btn(true);">&nbsp;L3000-L3999</asp:ListItem>
                                        <asp:ListItem Value="M3000_M3999" onclick="show_hide_revert_btn(true);">&nbsp;M3000-M3999</asp:ListItem>
                                        <asp:ListItem Value="N3000_N3999" onclick="show_hide_revert_btn(true);">&nbsp;N3000-N3999</asp:ListItem>
                                        <asp:ListItem Value="O3000_O3999" onclick="show_hide_revert_btn(true);">&nbsp;O3000-O3999</asp:ListItem>
                                        <asp:ListItem Value="P3000_P3999" onclick="show_hide_revert_btn(true);">&nbsp;P3000-P3999</asp:ListItem>
                                        <asp:ListItem Value="Q3000_Q3999" onclick="show_hide_revert_btn(true);">&nbsp;Q3000-Q3999</asp:ListItem>
                                        <asp:ListItem Value="R3000_R3999" onclick="show_hide_revert_btn(true);">&nbsp;R3000-R3999</asp:ListItem>
                                        <asp:ListItem Value="S3000_S3999" onclick="show_hide_revert_btn(true);">&nbsp;S3000-S3999</asp:ListItem>
                                        <asp:ListItem Value="T3000_T3999" onclick="show_hide_revert_btn(true);">&nbsp;T3000-T3999</asp:ListItem>
                                        <asp:ListItem Value="U3000_U3999" onclick="show_hide_revert_btn(true);">&nbsp;U3000-U3999</asp:ListItem>
                                        <asp:ListItem Value="V3000_V3999" onclick="show_hide_revert_btn(true);">&nbsp;V3000-V3999</asp:ListItem>
                                        <asp:ListItem Value="W3000_W3999" onclick="show_hide_revert_btn(true);">&nbsp;W3000-W3999</asp:ListItem>
                                        <asp:ListItem Value="X3000_X3999" onclick="show_hide_revert_btn(true);">&nbsp;X3000-X3999</asp:ListItem>
                                        <asp:ListItem Value="Y3000_Y3999" onclick="show_hide_revert_btn(true);">&nbsp;Y3000-Y3999</asp:ListItem>
                                        <asp:ListItem Value="Z3000_Z3999" onclick="show_hide_revert_btn(true);">&nbsp;Z3000-Z3999</asp:ListItem>

                                        <asp:ListItem Value="A4000_A4999" onclick="show_hide_revert_btn(true);">&nbsp;A4000-A4999</asp:ListItem>
                                        <asp:ListItem Value="B4000_B4999" onclick="show_hide_revert_btn(true);">&nbsp;B4000-B4999</asp:ListItem>
                                        <asp:ListItem Value="C4000_C4999" onclick="show_hide_revert_btn(true);">&nbsp;C4000-C4999</asp:ListItem>
                                        <asp:ListItem Value="D4000_D4999" onclick="show_hide_revert_btn(true);">&nbsp;D4000-D4999</asp:ListItem>
                                        <asp:ListItem Value="E4000_E4999" onclick="show_hide_revert_btn(true);">&nbsp;E4000-E4999</asp:ListItem>
                                        <asp:ListItem Value="F4000_F4999" onclick="show_hide_revert_btn(true);">&nbsp;F4000-F4999</asp:ListItem>
                                        <asp:ListItem Value="G4000_G4999" onclick="show_hide_revert_btn(true);">&nbsp;G4000-G4999</asp:ListItem>
                                        <asp:ListItem Value="H4000_H4999" onclick="show_hide_revert_btn(true);">&nbsp;H4000-H4999</asp:ListItem>
                                        <asp:ListItem Value="I4000_I4999" onclick="show_hide_revert_btn(true);">&nbsp;I4000-I4999</asp:ListItem>
                                        <asp:ListItem Value="J4000_J4999" onclick="show_hide_revert_btn(true);">&nbsp;J4000-J4999</asp:ListItem>
                                        <asp:ListItem Value="K4000_K4999" onclick="show_hide_revert_btn(true);">&nbsp;K4000-K4999</asp:ListItem>
                                        <asp:ListItem Value="L4000_L4999" onclick="show_hide_revert_btn(true);">&nbsp;L4000-L4999</asp:ListItem>
                                        <asp:ListItem Value="M4000_M4999" onclick="show_hide_revert_btn(true);">&nbsp;M4000-M4999</asp:ListItem>
                                        <asp:ListItem Value="N4000_N4999" onclick="show_hide_revert_btn(true);">&nbsp;N4000-N4999</asp:ListItem>
                                        <asp:ListItem Value="O4000_O4999" onclick="show_hide_revert_btn(true);">&nbsp;O4000-O4999</asp:ListItem>
                                        <asp:ListItem Value="P4000_P4999" onclick="show_hide_revert_btn(true);">&nbsp;P4000-P4999</asp:ListItem>
                                        <asp:ListItem Value="Q4000_Q4999" onclick="show_hide_revert_btn(true);">&nbsp;Q4000-Q4999</asp:ListItem>
                                        <asp:ListItem Value="R4000_R4999" onclick="show_hide_revert_btn(true);">&nbsp;R4000-R4999</asp:ListItem>
                                        <asp:ListItem Value="S4000_S4999" onclick="show_hide_revert_btn(true);">&nbsp;S4000-S4999</asp:ListItem>
                                        <asp:ListItem Value="T4000_T4999" onclick="show_hide_revert_btn(true);">&nbsp;T4000-T4999</asp:ListItem>
                                        <asp:ListItem Value="U4000_U4999" onclick="show_hide_revert_btn(true);">&nbsp;U4000-U4999</asp:ListItem>
                                        <asp:ListItem Value="V4000_V4999" onclick="show_hide_revert_btn(true);">&nbsp;V4000-V4999</asp:ListItem>
                                        <asp:ListItem Value="W4000_W4999" onclick="show_hide_revert_btn(true);">&nbsp;W4000-W4999</asp:ListItem>
                                        <asp:ListItem Value="X4000_X4999" onclick="show_hide_revert_btn(true);">&nbsp;X4000-X4999</asp:ListItem>
                                        <asp:ListItem Value="Y4000_Y4999" onclick="show_hide_revert_btn(true);">&nbsp;Y4000-Y4999</asp:ListItem>
                                        <asp:ListItem Value="Z4000_Z4999" onclick="show_hide_revert_btn(true);">&nbsp;Z4000-Z4999</asp:ListItem>

                                        <asp:ListItem Value="A5000_A5999" onclick="show_hide_revert_btn(true);">&nbsp;A5000-A5999</asp:ListItem>
                                        <asp:ListItem Value="B5000_B5999" onclick="show_hide_revert_btn(true);">&nbsp;B5000-B5999</asp:ListItem>
                                        <asp:ListItem Value="C5000_C5999" onclick="show_hide_revert_btn(true);">&nbsp;C5000-C5999</asp:ListItem>
                                        <asp:ListItem Value="D5000_D5999" onclick="show_hide_revert_btn(true);">&nbsp;D5000-D5999</asp:ListItem>
                                        <asp:ListItem Value="E5000_E5999" onclick="show_hide_revert_btn(true);">&nbsp;E5000-E5999</asp:ListItem>
                                        <asp:ListItem Value="F5000_F5999" onclick="show_hide_revert_btn(true);">&nbsp;F5000-F5999</asp:ListItem>
                                        <asp:ListItem Value="G5000_G5999" onclick="show_hide_revert_btn(true);">&nbsp;G5000-G5999</asp:ListItem>
                                        <asp:ListItem Value="H5000_H5999" onclick="show_hide_revert_btn(true);">&nbsp;H5000-H5999</asp:ListItem>
                                        <asp:ListItem Value="I5000_I5999" onclick="show_hide_revert_btn(true);">&nbsp;I5000-I5999</asp:ListItem>
                                        <asp:ListItem Value="J5000_J5999" onclick="show_hide_revert_btn(true);">&nbsp;J5000-J5999</asp:ListItem>
                                        <asp:ListItem Value="K5000_K5999" onclick="show_hide_revert_btn(true);">&nbsp;K5000-K5999</asp:ListItem>
                                        <asp:ListItem Value="L5000_L5999" onclick="show_hide_revert_btn(true);">&nbsp;L5000-L5999</asp:ListItem>
                                        <asp:ListItem Value="M5000_M5999" onclick="show_hide_revert_btn(true);">&nbsp;M5000-M5999</asp:ListItem>
                                        <asp:ListItem Value="N5000_N5999" onclick="show_hide_revert_btn(true);">&nbsp;N5000-N5999</asp:ListItem>
                                        <asp:ListItem Value="O5000_O5999" onclick="show_hide_revert_btn(true);">&nbsp;O5000-O5999</asp:ListItem>
                                        <asp:ListItem Value="P5000_P5999" onclick="show_hide_revert_btn(true);">&nbsp;P5000-P5999</asp:ListItem>
                                        <asp:ListItem Value="Q5000_Q5999" onclick="show_hide_revert_btn(true);">&nbsp;Q5000-Q5999</asp:ListItem>
                                        <asp:ListItem Value="R5000_R5999" onclick="show_hide_revert_btn(true);">&nbsp;R5000-R5999</asp:ListItem>
                                        <asp:ListItem Value="S5000_S5999" onclick="show_hide_revert_btn(true);">&nbsp;S5000-S5999</asp:ListItem>
                                        <asp:ListItem Value="T5000_T5999" onclick="show_hide_revert_btn(true);">&nbsp;T5000-T5999</asp:ListItem>
                                        <asp:ListItem Value="U5000_U5999" onclick="show_hide_revert_btn(true);">&nbsp;U5000-U5999</asp:ListItem>
                                        <asp:ListItem Value="V5000_V5999" onclick="show_hide_revert_btn(true);">&nbsp;V5000-V5999</asp:ListItem>
                                        <asp:ListItem Value="W5000_W5999" onclick="show_hide_revert_btn(true);">&nbsp;W5000-W5999</asp:ListItem>
                                        <asp:ListItem Value="X5000_X5999" onclick="show_hide_revert_btn(true);">&nbsp;X5000-X5999</asp:ListItem>
                                        <asp:ListItem Value="Y5000_Y5999" onclick="show_hide_revert_btn(true);">&nbsp;Y5000-Y5999</asp:ListItem>
                                        <asp:ListItem Value="Z5000_Z5999" onclick="show_hide_revert_btn(true);">&nbsp;Z5000-Z5999</asp:ListItem>

                                        <asp:ListItem Value="A6000_A6999" onclick="show_hide_revert_btn(true);">&nbsp;A6000-A6999</asp:ListItem>
                                        <asp:ListItem Value="B6000_B6999" onclick="show_hide_revert_btn(true);">&nbsp;B6000-B6999</asp:ListItem>
                                        <asp:ListItem Value="C6000_C6999" onclick="show_hide_revert_btn(true);">&nbsp;C6000-C6999</asp:ListItem>
                                        <asp:ListItem Value="D6000_D6999" onclick="show_hide_revert_btn(true);">&nbsp;D6000-D6999</asp:ListItem>
                                        <asp:ListItem Value="E6000_E6999" onclick="show_hide_revert_btn(true);">&nbsp;E6000-E6999</asp:ListItem>
                                        <asp:ListItem Value="F6000_F6999" onclick="show_hide_revert_btn(true);">&nbsp;F6000-F6999</asp:ListItem>
                                        <asp:ListItem Value="G6000_G6999" onclick="show_hide_revert_btn(true);">&nbsp;G6000-G6999</asp:ListItem>
                                        <asp:ListItem Value="H6000_H6999" onclick="show_hide_revert_btn(true);">&nbsp;H6000-H6999</asp:ListItem>
                                        <asp:ListItem Value="I6000_I6999" onclick="show_hide_revert_btn(true);">&nbsp;I6000-I6999</asp:ListItem>
                                        <asp:ListItem Value="J6000_J6999" onclick="show_hide_revert_btn(true);">&nbsp;J6000-J6999</asp:ListItem>
                                        <asp:ListItem Value="K6000_K6999" onclick="show_hide_revert_btn(true);">&nbsp;K6000-K6999</asp:ListItem>
                                        <asp:ListItem Value="L6000_L6999" onclick="show_hide_revert_btn(true);">&nbsp;L6000-L6999</asp:ListItem>
                                        <asp:ListItem Value="M6000_M6999" onclick="show_hide_revert_btn(true);">&nbsp;M6000-M6999</asp:ListItem>
                                        <asp:ListItem Value="N6000_N6999" onclick="show_hide_revert_btn(true);">&nbsp;N6000-N6999</asp:ListItem>
                                        <asp:ListItem Value="O6000_O6999" onclick="show_hide_revert_btn(true);">&nbsp;O6000-O6999</asp:ListItem>
                                        <asp:ListItem Value="P6000_P6999" onclick="show_hide_revert_btn(true);">&nbsp;P6000-P6999</asp:ListItem>
                                        <asp:ListItem Value="Q6000_Q6999" onclick="show_hide_revert_btn(true);">&nbsp;Q6000-Q6999</asp:ListItem>
                                        <asp:ListItem Value="R6000_R6999" onclick="show_hide_revert_btn(true);">&nbsp;R6000-R6999</asp:ListItem>
                                        <asp:ListItem Value="S6000_S6999" onclick="show_hide_revert_btn(true);">&nbsp;S6000-S6999</asp:ListItem>
                                        <asp:ListItem Value="T6000_T6999" onclick="show_hide_revert_btn(true);">&nbsp;T6000-T6999</asp:ListItem>
                                        <asp:ListItem Value="U6000_U6999" onclick="show_hide_revert_btn(true);">&nbsp;U6000-U6999</asp:ListItem>
                                        <asp:ListItem Value="V6000_V6999" onclick="show_hide_revert_btn(true);">&nbsp;V6000-V6999</asp:ListItem>
                                        <asp:ListItem Value="W6000_W6999" onclick="show_hide_revert_btn(true);">&nbsp;W6000-W6999</asp:ListItem>
                                        <asp:ListItem Value="X6000_X6999" onclick="show_hide_revert_btn(true);">&nbsp;X6000-X6999</asp:ListItem>
                                        <asp:ListItem Value="Y6000_Y6999" onclick="show_hide_revert_btn(true);">&nbsp;Y6000-Y6999</asp:ListItem>
                                        <asp:ListItem Value="Z6000_Z6999" onclick="show_hide_revert_btn(true);">&nbsp;Z6000-Z6999</asp:ListItem>

                                        <asp:ListItem Value="A7000_A7999" onclick="show_hide_revert_btn(true);">&nbsp;A7000-A7999</asp:ListItem>
                                        <asp:ListItem Value="B7000_B7999" onclick="show_hide_revert_btn(true);">&nbsp;B7000-B7999</asp:ListItem>
                                        <asp:ListItem Value="C7000_C7999" onclick="show_hide_revert_btn(true);">&nbsp;C7000-C7999</asp:ListItem>
                                        <asp:ListItem Value="D7000_D7999" onclick="show_hide_revert_btn(true);">&nbsp;D7000-D7999</asp:ListItem>
                                        <asp:ListItem Value="E7000_E7999" onclick="show_hide_revert_btn(true);">&nbsp;E7000-E7999</asp:ListItem>
                                        <asp:ListItem Value="F7000_F7999" onclick="show_hide_revert_btn(true);">&nbsp;F7000-F7999</asp:ListItem>
                                        <asp:ListItem Value="G7000_G7999" onclick="show_hide_revert_btn(true);">&nbsp;G7000-G7999</asp:ListItem>
                                        <asp:ListItem Value="H7000_H7999" onclick="show_hide_revert_btn(true);">&nbsp;H7000-H7999</asp:ListItem>
                                        <asp:ListItem Value="I7000_I7999" onclick="show_hide_revert_btn(true);">&nbsp;I7000-I7999</asp:ListItem>
                                        <asp:ListItem Value="J7000_J7999" onclick="show_hide_revert_btn(true);">&nbsp;J7000-J7999</asp:ListItem>
                                        <asp:ListItem Value="K7000_K7999" onclick="show_hide_revert_btn(true);">&nbsp;K7000-K7999</asp:ListItem>
                                        <asp:ListItem Value="L7000_L7999" onclick="show_hide_revert_btn(true);">&nbsp;L7000-L7999</asp:ListItem>
                                        <asp:ListItem Value="M7000_M7999" onclick="show_hide_revert_btn(true);">&nbsp;M7000-M7999</asp:ListItem>
                                        <asp:ListItem Value="N7000_N7999" onclick="show_hide_revert_btn(true);">&nbsp;N7000-N7999</asp:ListItem>
                                        <asp:ListItem Value="O7000_O7999" onclick="show_hide_revert_btn(true);">&nbsp;O7000-O7999</asp:ListItem>
                                        <asp:ListItem Value="P7000_P7999" onclick="show_hide_revert_btn(true);">&nbsp;P7000-P7999</asp:ListItem>
                                        <asp:ListItem Value="Q7000_Q7999" onclick="show_hide_revert_btn(true);">&nbsp;Q7000-Q7999</asp:ListItem>
                                        <asp:ListItem Value="R7000_R7999" onclick="show_hide_revert_btn(true);">&nbsp;R7000-R7999</asp:ListItem>
                                        <asp:ListItem Value="S7000_S7999" onclick="show_hide_revert_btn(true);">&nbsp;S7000-S7999</asp:ListItem>
                                        <asp:ListItem Value="T7000_T7999" onclick="show_hide_revert_btn(true);">&nbsp;T7000-T7999</asp:ListItem>
                                        <asp:ListItem Value="U7000_U7999" onclick="show_hide_revert_btn(true);">&nbsp;U7000-U7999</asp:ListItem>
                                        <asp:ListItem Value="V7000_V7999" onclick="show_hide_revert_btn(true);">&nbsp;V7000-V7999</asp:ListItem>
                                        <asp:ListItem Value="W7000_W7999" onclick="show_hide_revert_btn(true);">&nbsp;W7000-W7999</asp:ListItem>
                                        <asp:ListItem Value="X7000_X7999" onclick="show_hide_revert_btn(true);">&nbsp;X7000-X7999</asp:ListItem>
                                        <asp:ListItem Value="Y7000_Y7999" onclick="show_hide_revert_btn(true);">&nbsp;Y7000-Y7999</asp:ListItem>
                                        <asp:ListItem Value="Z7000_Z7999" onclick="show_hide_revert_btn(true);">&nbsp;Z7000-Z7999</asp:ListItem>

                                        <asp:ListItem Value="A8000_A8999" onclick="show_hide_revert_btn(true);">&nbsp;A8000-A8999</asp:ListItem>
                                        <asp:ListItem Value="B8000_B8999" onclick="show_hide_revert_btn(true);">&nbsp;B8000-B8999</asp:ListItem>
                                        <asp:ListItem Value="C8000_C8999" onclick="show_hide_revert_btn(true);">&nbsp;C8000-C8999</asp:ListItem>
                                        <asp:ListItem Value="D8000_D8999" onclick="show_hide_revert_btn(true);">&nbsp;D8000-D8999</asp:ListItem>
                                        <asp:ListItem Value="E8000_E8999" onclick="show_hide_revert_btn(true);">&nbsp;E8000-E8999</asp:ListItem>
                                        <asp:ListItem Value="F8000_F8999" onclick="show_hide_revert_btn(true);">&nbsp;F8000-F8999</asp:ListItem>
                                        <asp:ListItem Value="G8000_G8999" onclick="show_hide_revert_btn(true);">&nbsp;G8000-G8999</asp:ListItem>
                                        <asp:ListItem Value="H8000_H8999" onclick="show_hide_revert_btn(true);">&nbsp;H8000-H8999</asp:ListItem>
                                        <asp:ListItem Value="I8000_I8999" onclick="show_hide_revert_btn(true);">&nbsp;I8000-I8999</asp:ListItem>
                                        <asp:ListItem Value="J8000_J8999" onclick="show_hide_revert_btn(true);">&nbsp;J8000-J8999</asp:ListItem>
                                        <asp:ListItem Value="K8000_K8999" onclick="show_hide_revert_btn(true);">&nbsp;K8000-K8999</asp:ListItem>
                                        <asp:ListItem Value="L8000_L8999" onclick="show_hide_revert_btn(true);">&nbsp;L8000-L8999</asp:ListItem>
                                        <asp:ListItem Value="M8000_M8999" onclick="show_hide_revert_btn(true);">&nbsp;M8000-M8999</asp:ListItem>
                                        <asp:ListItem Value="N8000_N8999" onclick="show_hide_revert_btn(true);">&nbsp;N8000-N8999</asp:ListItem>
                                        <asp:ListItem Value="O8000_O8999" onclick="show_hide_revert_btn(true);">&nbsp;O8000-O8999</asp:ListItem>
                                        <asp:ListItem Value="P8000_P8999" onclick="show_hide_revert_btn(true);">&nbsp;P8000-P8999</asp:ListItem>
                                        <asp:ListItem Value="Q8000_Q8999" onclick="show_hide_revert_btn(true);">&nbsp;Q8000-Q8999</asp:ListItem>
                                        <asp:ListItem Value="R8000_R8999" onclick="show_hide_revert_btn(true);">&nbsp;R8000-R8999</asp:ListItem>
                                        <asp:ListItem Value="S8000_S8999" onclick="show_hide_revert_btn(true);">&nbsp;S8000-S8999</asp:ListItem>
                                        <asp:ListItem Value="T8000_T8999" onclick="show_hide_revert_btn(true);">&nbsp;T8000-T8999</asp:ListItem>
                                        <asp:ListItem Value="U8000_U8999" onclick="show_hide_revert_btn(true);">&nbsp;U8000-U8999</asp:ListItem>
                                        <asp:ListItem Value="V8000_V8999" onclick="show_hide_revert_btn(true);">&nbsp;V8000-V8999</asp:ListItem>
                                        <asp:ListItem Value="W8000_W8999" onclick="show_hide_revert_btn(true);">&nbsp;W8000-W8999</asp:ListItem>
                                        <asp:ListItem Value="X8000_X8999" onclick="show_hide_revert_btn(true);">&nbsp;X8000-X8999</asp:ListItem>
                                        <asp:ListItem Value="Y8000_Y8999" onclick="show_hide_revert_btn(true);">&nbsp;Y8000-Y8999</asp:ListItem>
                                        <asp:ListItem Value="Z8000_Z8999" onclick="show_hide_revert_btn(true);">&nbsp;Z8000-Z8999</asp:ListItem>

                                        <asp:ListItem Value="A9000_A9999" onclick="show_hide_revert_btn(true);">&nbsp;A9000-A9999</asp:ListItem>
                                        <asp:ListItem Value="B9000_B9999" onclick="show_hide_revert_btn(true);">&nbsp;B9000-B9999</asp:ListItem>
                                        <asp:ListItem Value="C9000_C9999" onclick="show_hide_revert_btn(true);">&nbsp;C9000-C9999</asp:ListItem>
                                        <asp:ListItem Value="D9000_D9999" onclick="show_hide_revert_btn(true);">&nbsp;D9000-D9999</asp:ListItem>
                                        <asp:ListItem Value="E9000_E9999" onclick="show_hide_revert_btn(true);">&nbsp;E9000-E9999</asp:ListItem>
                                        <asp:ListItem Value="F9000_F9999" onclick="show_hide_revert_btn(true);">&nbsp;F9000-F9999</asp:ListItem>
                                        <asp:ListItem Value="G9000_G9999" onclick="show_hide_revert_btn(true);">&nbsp;G9000-G9999</asp:ListItem>
                                        <asp:ListItem Value="H9000_H9999" onclick="show_hide_revert_btn(true);">&nbsp;H9000-H9999</asp:ListItem>
                                        <asp:ListItem Value="I9000_I9999" onclick="show_hide_revert_btn(true);">&nbsp;I9000-I9999</asp:ListItem>
                                        <asp:ListItem Value="J9000_J9999" onclick="show_hide_revert_btn(true);">&nbsp;J9000-J9999</asp:ListItem>
                                        <asp:ListItem Value="K9000_K9999" onclick="show_hide_revert_btn(true);">&nbsp;K9000-K9999</asp:ListItem>
                                        <asp:ListItem Value="L9000_L9999" onclick="show_hide_revert_btn(true);">&nbsp;L9000-L9999</asp:ListItem>
                                        <asp:ListItem Value="M9000_M9999" onclick="show_hide_revert_btn(true);">&nbsp;M9000-M9999</asp:ListItem>
                                        <asp:ListItem Value="N9000_N9999" onclick="show_hide_revert_btn(true);">&nbsp;N9000-N9999</asp:ListItem>
                                        <asp:ListItem Value="O9000_O9999" onclick="show_hide_revert_btn(true);">&nbsp;O9000-O9999</asp:ListItem>
                                        <asp:ListItem Value="P9000_P9999" onclick="show_hide_revert_btn(true);">&nbsp;P9000-P9999</asp:ListItem>
                                        <asp:ListItem Value="Q9000_Q9999" onclick="show_hide_revert_btn(true);">&nbsp;Q9000-Q9999</asp:ListItem>
                                        <asp:ListItem Value="R9000_R9999" onclick="show_hide_revert_btn(true);">&nbsp;R9000-R9999</asp:ListItem>
                                        <asp:ListItem Value="S9000_S9999" onclick="show_hide_revert_btn(true);">&nbsp;S9000-S9999</asp:ListItem>
                                        <asp:ListItem Value="T9000_T9999" onclick="show_hide_revert_btn(true);">&nbsp;T9000-T9999</asp:ListItem>
                                        <asp:ListItem Value="U9000_U9999" onclick="show_hide_revert_btn(true);">&nbsp;U9000-U9999</asp:ListItem>
                                        <asp:ListItem Value="V9000_V9999" onclick="show_hide_revert_btn(true);">&nbsp;V9000-V9999</asp:ListItem>
                                        <asp:ListItem Value="W9000_W9999" onclick="show_hide_revert_btn(true);">&nbsp;W9000-W9999</asp:ListItem>
                                        <asp:ListItem Value="X9000_X9999" onclick="show_hide_revert_btn(true);">&nbsp;X9000-X9999</asp:ListItem>
                                        <asp:ListItem Value="Y9000_Y9999" onclick="show_hide_revert_btn(true);">&nbsp;Y9000-Y9999</asp:ListItem>
                                        <asp:ListItem Value="Z9000_Z9999" onclick="show_hide_revert_btn(true);">&nbsp;Z9000-Z9999</asp:ListItem>
                                    </asp:CheckBoxList>

                                </div>
                            </center>
                        </td>
                    </tr>
                    <tr style="height:15px;">
                        <td colspan="2"></td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align:center">
                            <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" Text="Update" />
                        </td>
                    </tr>

                </table>

            </center>

        </div>
    </div>


</asp:Content>



