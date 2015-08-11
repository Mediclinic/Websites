<%@ Page Title="Staff Stats" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="OrganisationStatsChartsV2.aspx.cs" Inherits="OrganisationStatsChartsV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <div class="clearfix">
        <div class="page_title"><span id="lblHeading" runat="server">Organisation Statistics Charts</span></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div class="border_top_bottom">
                    <table class="block_center">
                        <tr>

                            <td rowspan="2">

                                <asp:DropDownList ID="ddlChartType" runat="server" style="width:100%">
                                    <asp:ListItem Value="StackedBar" Text="Bar Graph"/>
                                    <asp:ListItem Value="Column" Text="Column Graph"/>
                                    <asp:ListItem Value="Pie" Text="Pie Chart"/>
                                    <asp:ListItem Value="Doughnut" Text="Doughnut Chart"/>
                                    <asp:ListItem Value="Area" Text="Area Graph"/>
                                    <asp:ListItem Value="BoxPlot" Text="BoxPlot Graph"/>
                                    <asp:ListItem Value="Line" Text="Line Graph"/>
                                    <asp:ListItem Value="Point" Text="Point Graph"/>
                                    <asp:ListItem Value="Range" Text="Range Graph"/>
                                </asp:DropDownList>
                                <br />
                                <asp:DropDownList ID="ddlType" runat="server" style="width:100%" />

                            </td>

                            <td rowspan="2" style="width:25px"></td>

                            <td><asp:Label ID="lblSearchDate" runat="server">Start Date: &nbsp;&nbsp;</asp:Label></td>
                            <td><asp:TextBox ID="txtStartDate" runat="server" Columns="10"></asp:TextBox></td>
                            <td><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>

                            <td rowspan="2" style="width:25px"></td>

                            <td rowspan="2"><asp:CheckBox ID="chkIncDeleted" runat="server" Text="" Font-Bold="false" OnCheckedChanged="btnSearch_Click" />&nbsp;<label for="chkIncDeleted" style="font-weight:normal;">Include Deleted Clinics/Facs</label></td>

                            <td rowspan="2" style="width:25px"></td>

                            <td rowspan="2"><asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" /></td>

                        </tr>
                        <tr>
                            <td><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                            <td><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                            <td><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>

                        </tr>
                    </table>
                </div>

            </div>


            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <div style="height:1px;"></div>
            <center>
                <h3>
                    <asp:Label ID="lblChartHeading" runat="server"></asp:Label>
                </h3>
            </center>
            <div style="height:8px;"></div>
            
            <table class="block_center">
                <tr>
                    <td>

                        <!-- 
                            http://www.codeproject.com/Articles/235117/ASP-NET-Chart-Control
                            https://msdn.microsoft.com/en-us/library/dd456764%28v=vs.140%29.aspx
                         -->

                        <asp:Chart ID="chartOrgStats" runat="server" 
                           BackColor="#EDEDED" 
                            Width="900px" 
                            Height="900px">

                           <Titles>
                              <asp:Title Name="Title1" Text="Bookings" 
                                  Alignment="TopCenter" 
                                  Font="Tahoma, 15pt, style=Bold"  
                                  ForeColor="#817f7f" 
                                  Visible="false" />
                           </Titles>

                           <Series>
                               <asp:Series Name="Series1"  IsValueShownAsLabel="True" ChartType="StackedBar">
                               </asp:Series>
                           </Series>

                           <ChartAreas>
                              <asp:ChartArea Name="ChartArea1" 
                                    BackGradientStyle="DiagonalLeft" 
                                    BackColor="#AACD4B" 
                                    BackSecondaryColor="#EDEDED" 
                                    BorderDashStyle="Solid" 
                                    BorderColor="#404040" 
                                    BorderWidth="1">
                                 <AxisX>
                                    <MajorGrid Enabled="False" />
                                 </AxisX>
                                 <Position Height="100" Width="100" X="0" Y="0" />
                              </asp:ChartArea>
                           </ChartAreas>

                        </asp:Chart>
                    </td>
                </tr>
            </table>


            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>


        </div>
    </div>


</asp:Content>



