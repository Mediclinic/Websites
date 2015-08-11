<%@ Page Title="Cost Centres" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="PatientConditionListV2.aspx.cs" Inherits="PatientConditionListV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Patient Conditions</asp:Label></div>
        <center class="main_content" style="padding:20px 5px;">

            <center>
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </center>

            <div style="height:10px;"></div>

            <center>

                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <table>
                        <tr>
                            <td style="vertical-align:top;">
                                <asp:GridView ID="GrdCondition" runat="server" 
                                        AutoGenerateColumns="False" DataKeyNames="condition_condition_id" 
                                        OnRowCancelingEdit="GrdCondition_RowCancelingEdit" 
                                        OnRowDataBound="GrdCondition_RowDataBound" 
                                        OnRowEditing="GrdCondition_RowEditing" 
                                        OnRowUpdating="GrdCondition_RowUpdating" ShowHeader="False" ShowFooter="False" 
                                        OnRowCommand="GrdCondition_RowCommand" 
                                        OnRowCreated="GrdCondition_RowCreated"
                                        AllowSorting="True" 
                                        OnSorting="GrdCondition_Sorting"
                                        ClientIDMode="Predictable"
                                        GridLines="None"
                                        CssClass="table table-grid-top-bottum-padding-normal auto_width block_center">

                                    <Columns> 

                                        <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="condition_condition_id" Visible="false"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblId" runat="server" Text='<%# Bind("condition_condition_id") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Checkbox" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_descr"> 
                                            <ItemTemplate> 
                                                <asp:CheckBox ID="chkSelect" runat="server" />
                                                &nbsp;
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_descr" ItemStyle-CssClass="text_left"> 
                                            <ItemTemplate> 

                                                <asp:Label ID="lblDescr" runat="server" Text='<%# Bind("condition_descr") %>'></asp:Label> 

                                                <div id="br_date" runat="server" style="height:5px;"></div>
                                                <asp:DropDownList ID="ddlDate_Day" runat="server"></asp:DropDownList>
                                                <asp:DropDownList ID="ddlDate_Month" runat="server"></asp:DropDownList>
                                                <asp:DropDownList ID="ddlDate_Year" runat="server"></asp:DropDownList>

                                                <div id="br_nweeksdue" runat="server" style="height:5px;"></div>
                                                <asp:Label ID="lblNextDue" runat="server" Text="Next Due: "></asp:Label>
                                                <asp:DropDownList ID="ddlNbrWeeksDue" runat="server"></asp:DropDownList> 
                                                <asp:Label ID="lblWeeksLater" runat="server" Text="Weeks Later "></asp:Label>

                                                <div id="br_text" runat="server" style="height:5px;"></div>
                                                <asp:Label ID="lblAdditionalInfo" runat="server" Text="<u>Additional Info</u>:<br />"></asp:Label>
                                                <asp:TextBox ID="txtText" runat="server" TextMode="MultiLine" Rows="3" Columns="30"></asp:TextBox>

                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                    </Columns> 

                                </asp:GridView>
                            </td>
                            <td style="min-width:35px;"></td>
                            <td style="vertical-align:top;">
                                <asp:GridView ID="GrdConditionView" runat="server" 
                                        AutoGenerateColumns="False" DataKeyNames="condition_condition_id" 
                                        OnRowCancelingEdit="GrdConditionView_RowCancelingEdit" 
                                        OnRowDataBound="GrdConditionView_RowDataBound" 
                                        OnRowEditing="GrdConditionView_RowEditing" 
                                        OnRowUpdating="GrdConditionView_RowUpdating" ShowHeader="False" ShowFooter="False" 
                                        OnRowCommand="GrdConditionView_RowCommand" 
                                        OnRowCreated="GrdConditionView_RowCreated"
                                        AllowSorting="True" 
                                        OnSorting="GrdConditionView_Sorting"
                                        ClientIDMode="Predictable"
                                        GridLines="None"
                                        CssClass="table table-grid-top-bottum-padding-normal auto_width block_center">

                                    <Columns> 

                                        <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="condition_condition_id" Visible="false"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblId" runat="server" Text='<%# Eval("condition_condition_id") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Checkbox" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_descr"> 
                                            <ItemTemplate> 
                                                •
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Descr" HeaderStyle-HorizontalAlign="Left" SortExpression="condition_descr" ItemStyle-CssClass="text_left"> 
                                            <ItemTemplate> 

                                                <asp:Label ID="lblDescr" runat="server" Text='<%# Eval("condition_descr") %>'></asp:Label> 

                                                <div id="br_date" runat="server" style="height:1px;"></div>
                                                <asp:Label ID="lblDate" runat="server"></asp:Label> 

                                                <div id="br_nweeksdue" runat="server" style="height:1px;"></div>
                                                <asp:Label ID="lblNextDue" runat="server" Text="Next Due: "></asp:Label>
                                                <asp:Label ID="lblDateDue" runat="server"></asp:Label> 

                                                <div id="br_text" runat="server" style="height:1px;"></div>
                                                <asp:Label ID="lblAdditionalInfo" runat="server" Text="<u>Additional Info</u>:<br />"></asp:Label>
                                                <asp:Label ID="lblText" runat="server"></asp:Label> 

                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                    </Columns> 

                                </asp:GridView>
                            </td>
                        </tr>
                    </table>




                </div>

                <br />

                <asp:Button ID="btnUpdate" runat="server" Text ="Update" OnClick="btnUpdate_Click" ValidationGroup="ValidationSummaryEdit" />
                &nbsp;
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="self.close();" />

            </center>

        </div>
    </div>


</asp:Content>



