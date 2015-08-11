<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BookingKeyControl.ascx.cs" Inherits="Bookings_BookingKeyControl" %>


    <table id="tbl_non_edit_mode" runat="server" border="0" cellpadding="0" cellspacing="0" style="width:360px">
        <tr>
            <td>

                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_Available" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">Available</td>
                    </tr>
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_Unavailable" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">Unavailable</td>
                    </tr>

                    <tr height="10">
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>

                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_CL_Unconfirmed" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">[Clinic] Unconf.</td>
                    </tr>
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_CL_Confirmed" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">[Clinic] Conf.</td>
                    </tr>
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_CL_Unconfirmed_epc" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">[Clinic] Unconf. Ref.</td>
                    </tr>
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_CL_Confirmed_epc" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">[Clinic] Conf. Ref.</td>
                    </tr>

                </table>

            </td>

            <td style="width:12px"></td>
            
            <td>

                <table style="width:180px" border="0" cellpadding="0" cellspacing="0">

                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_Completed" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">Completed</td>
                    </tr>

                    <tr>
                        <td><br /></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td><br /></td>
                        <td></td>
                        <td></td>
                    </tr>

                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_AC_Unconfirmed" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">[AC] Unconf.</td>
                    </tr>
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_AC_Confirmed" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">[AC] Conf.</td>
                    </tr>
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_AC_Unconfirmed_epc" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">[AC] Unconf. Ref.</td>
                    </tr>
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_AC_Confirmed_epc" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">[AC] Conf. Ref.</td>
                    </tr>

                </table>

            </td>

        </tr>
    </table>

    <table id="tbl_edit_mode" runat="server" border="0" cellpadding="0" cellspacing="0" style="width:400px">
        <tr>

            <td>

                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_edit_Available" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">Available</td>
                    </tr>
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_edit_Unavailable" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">Unavailable</td>
                    </tr>
                    <tr>
                        <td style="width:30px">
                            <table border="1" cellspacing="0" cellpadding="0" width="100%"><tr><td id="td_edit_Booking" runat="server">&nbsp;</td></tr></table>
                        </td>
                        <td style="width:5px"></td>
                        <td style="text-align:left">Booking To Edit</td>
                    </tr>
                </table>

            </td>

        </tr>
    </table>