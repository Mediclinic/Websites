<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PatientReferrerControlV2.ascx.cs" Inherits="PatientReferrerControlV2" %>

<asp:TextBox ID="txtUpdateRegisterReferrerID" runat="server" CssClass="hiddencol" />
<asp:Button ID="btnRegisterReferrerUpdate"   runat="server" CssClass="hiddencol" Text="" onclick="btnRegisterReferrerUpdate_Click" />

<asp:HiddenField ID="patientReferrerPatientID" runat="server" />
<asp:HiddenField ID="patientReferrerFormType" runat="server" />

<table>
    <tr id="displayHaveReferrerRow" runat="server" valign="top">
        <td class="nowrap" style="min-width:120px;">
            <asp:Label ID="lblReferrerText" runat="server" Text="Referrer: " Font-Bold="true" />&nbsp;
            <br />
            <asp:LinkButton ID="btnRegisterReferrerListPopup" runat="server" Text="Change" OnClientClick="javascript:get_register_referrer(); return false;"/>
            <asp:Label ID="lblDeleteRegistrationReferrerBtnSeperator" runat="server">|</asp:Label>
            <asp:LinkButton ID="btnDelete" runat="server" Text="Remove" onclick="btnDelete_Click" OnClientClick="javascript:if (!confirm('Are you sure you want to delete this referrer?')) return false;" />
            <br />
            <asp:Label ID="lblPatientReferrerHistoryPopup" runat="server"/>
        </td>
        <td style="min-width:8px;"></td>
        <td class="nowrap">
            <asp:Label ID="lblReferrer" runat="server" />
            <asp:Label ID="lblReferrerRegisterID" runat="server" CssClass="hiddencol" />
        </td>
        <td style="width:20px"></td>
        <td class="nowrap">
            <asp:LinkButton Visible="False" ID="btnChangeToEditMode" runat="server" Text="Edit"  onclick="btnChangeToEditMode_Click" />
        </td>
        <td style="width:1px"></td>
        <td class="nowrap">
            <asp:LinkButton Visible="False" ID="btnDelete2" runat="server" Text="Remove" onclick="btnDelete_Click" OnClientClick="javascript:if (!confirm('Are you sure you want to delete this referrer?')) return false;" />
        </td>
    </tr>
    <tr id="editRow" runat="server" valign="top">
        <td class="nowrap" style="min-width:120px;">
            <asp:Label ID="lblReferrerText3" runat="server" Text="Referrer: " Font-Bold="true" />&nbsp;
        </td>
        <td style="min-width:8px;"></td>
        <td class="nowrap" align="center">
            <asp:DropDownList ID="ddlReferrer" runat="server"></asp:DropDownList>
        </td>
        <td style="width:20px"></td>
        <td class="nowrap">
            <asp:LinkButton ID="btnUpdate" runat="server" Text="Update" onclick="btnUpdate_Click" />
        </td>
        <td style="width:1px"></td>
        <td class="nowrap">
            <asp:LinkButton ID="btnCancelEdit" runat="server" Text="Cancel" onclick="btnCancelEdit_Click" />
        </td>
    </tr>
    <tr id="displayNoReferrerRow" runat="server" valign="top">
        <td class="nowrap" style="min-width:120px;">
            <asp:Label ID="Label1" runat="server" Text="Referrer: " Font-Bold="true" />
        </td>
        <td style="min-width:8px;"></td>
        <td class="nowrap" align="center">
            <asp:Label ID="lblNoReferrer" runat="server" Text="No Doctor"></asp:Label>
        </td>
        <td style="width:20px"></td>
        <td class="nowrap">
            <asp:LinkButton Visible="False" ID="btnChangeToAddMode" runat="server" Text="Allocate" onclick="btnChangeToAddMode_Click" />
            <asp:LinkButton ID="btnRegisterReferrerListPopupAdd" runat="server" Text="Allocate" OnClientClick="javascript:get_register_referrer(); return false;"/>
        </td>
        <td></td>
        <td></td>
    </tr>
    <tr id="addRow" runat="server" valign="top">
        <td class="nowrap" style="min-width:120px;">
            <asp:Label ID="lblReferrerText2" runat="server" Text="Referrer: " Font-Bold="true" />&nbsp;
        </td>
        <td style="min-width:8px;"></td>
        <td class="nowrap" align="center">
            <asp:DropDownList ID="ddlNewReferrer" runat="server"></asp:DropDownList>
        </td>
        <td style="width:20px"></td>
        <td class="nowrap">
            <asp:LinkButton ID="btnAdd" runat="server" Text="Allocate" onclick="btnAdd_Click" />
        </td>
        <td style="width:1px"></td>
        <td class="nowrap">
            <asp:LinkButton ID="btnCancelAdd" runat="server" Text="Cancel" onclick="btnCancelAdd_Click" />
        </td>
    </tr>
    <tr id="errorRow" runat="server" valign="top">
        <td class="nowrap" colspan="5">
            <asp:Label ID="lblErrorMessage" runat="server" Font-Bold="true" />
        </td>
    </tr>
    <tr id="newReferrersLinkRow" runat="server" height="35" valign="top">
        <td></td>
        <td class="nowrap" colspan="4">
            &nbsp;&nbsp;&nbsp;&nbsp;<a href="javascript:void(0)" onclick="javascript:update_referrers();return false;">Doctor Not Found? Add New Doctor</a>
        </td>
    </tr>
</table>