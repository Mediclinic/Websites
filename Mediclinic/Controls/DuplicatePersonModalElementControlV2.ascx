<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DuplicatePersonModalElementControlV2.ascx.cs" Inherits="DuplicatePersonModalElementControlV2" %>

<div style="display: none;"  id="modalPopupDupicatePerson">
    <div class="modalBackground"></div>
    <div class="modalContainer">
        <div class="modalFullDays">
            <div class="modalBody">

                <center>
                    <table style="width:100%;">
                        <tr>
                            <td style="text-align:center;">
                                <strong>
                                    <h3><span id="modalTitle">Duplicate Person Check</span></h3>
                                </strong>
                            </td>
                        </tr>
                        <tr height="15">
                            <td></td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Panel ID="pnlDuplicatePerson" runat="server" ScrollBars="Auto" Width="100%" Height="140px">
                                    <span id="ctable"></span>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr height="15">
                            <td></td>
                        </tr>
                        <tr>
                            <td style="text-align:center;" class="tfoot">
                                <input type="submit" value="None of these" onclick="hide_modal('modalPopupDupicatePerson'); return false;" />
                            </td>
                        </tr>
                    </table>
                </center>

            </div>
        </div>
    </div>
</div>