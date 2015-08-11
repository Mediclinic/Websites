<%@ Page Title="User Logins" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StaffLoginsV2.aspx.cs" Inherits="StaffLoginsV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">User Logins <asp:Label ID="lblNumCurrentlyLoggedIn" runat="server"></asp:Label></span></div>
        <div class="main_content_with_header">
            <div class="user_login_form">

                <div>
                    <center>
                    <table>
                        <tr>
                            <td><asp:Label ID="lblSearchDate" runat="server">Start Date: </asp:Label></td>
                            <td><asp:TextBox ID="txtStartDate" runat="server" Columns="10"></asp:TextBox></td>
                            <td><asp:ImageButton ID="txtStartDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>

                            <td style="width:15px"></td>

                            <td><asp:Label ID="lblEndDate" runat="server">End Date: </asp:Label></td>
                            <td><asp:TextBox ID="txtEndDate" runat="server" Columns="10"></asp:TextBox></td>
                            <td><asp:ImageButton ID="txtEndDate_Picker" runat="server" ImageUrl="~/images/Calendar-icon-24px.png" /></td>

                            <td style="width:12px"></td>

                            <td><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:DropDownList ID="ddlDisplayUserType" runat="server" OnSelectedIndexChanged="ddlDisplayUserType_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="All" Value="All"></asp:ListItem>
                                    <asp:ListItem Text="Staff Only" Value="Staff Only"></asp:ListItem>
                                    <asp:ListItem Text="Patients Only" Value="Patients Only"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                    </center>
                </div>

                <span id="img_log_user_off_icon" runat="server"><img src="imagesV2/x.png" alt="log user off icon" style="margin:0 5px 5px 0;" />Log User Off</span>

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="autodivheight" class="divautoheight" style="height:500px;width:auto;padding-right:17px;">

                    <asp:GridView ID="GrdUserLogin" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="userlogin_userlogin_id" 
                         OnRowCancelingEdit="GrdUserLogin_RowCancelingEdit" 
                         OnRowDataBound="GrdUserLogin_RowDataBound" 
                         OnRowEditing="GrdUserLogin_RowEditing" 
                         OnRowUpdating="GrdUserLogin_RowUpdating" ShowFooter="False" 
                         OnRowCommand="GrdUserLogin_RowCommand" 
                         OnRowDeleting="GrdUserLogin_RowDeleting" 
                         OnRowCreated="GrdUserLogin_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         AllowPaging="True"
                         OnPageIndexChanging="GrdUserLogin_PageIndexChanging"
                         PageSize="16"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />
        

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="userlogin_userlogin_id" ItemStyle-CssClass="text_left"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("userlogin_userlogin_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("userlogin_userlogin_id") %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Type"  HeaderStyle-HorizontalAlign="Left" SortExpression="user_to_display" ItemStyle-CssClass="nowrap text_left"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblUserType" runat="server" Text='<%# Eval("userlogin_staff_id") != DBNull.Value ? "Staff" : "Patient"  %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblUserType" runat="server" Text='<%# Eval("user_type")  %>' Font-Bold='<%# Eval("userlogin_is_successful") != DBNull.Value && ((bool)Eval("userlogin_is_successful") && Eval("userlogin_staff_id") != DBNull.Value && ((int)Eval("userlogin_staff_id")) >= 0) && !((bool)Eval("userlogin_is_logged_off"))  %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Username"  HeaderStyle-HorizontalAlign="Left" SortExpression="user_to_display" ItemStyle-CssClass="nowrap text_left"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblUsername" runat="server" Text='<%# Bind("user_to_display") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblUsername" runat="server" Text='<%# Bind("user_to_display") %>' Font-Bold='<%# Eval("userlogin_is_successful") != DBNull.Value && ((bool)Eval("userlogin_is_successful") && Eval("userlogin_staff_id") != DBNull.Value && ((int)Eval("userlogin_staff_id")) >= 0) && !((bool)Eval("userlogin_is_logged_off"))  %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Successful"  HeaderStyle-HorizontalAlign="Left" SortExpression="userlogin_is_successful"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblIsSuccessful" runat="server" Text='<%# Eval("userlogin_is_successful").ToString()=="True"?"Yes":"No" %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsSuccessful" runat="server" Text='<%# Eval("userlogin_is_successful").ToString()=="True"?"Yes":"No" %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Site"  HeaderStyle-HorizontalAlign="Left" SortExpression="site_name" ItemStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblSite" runat="server" Text='<%# Eval("site_name") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSite" runat="server" Text='<%# Eval("site_name") %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Logged Off"  HeaderStyle-HorizontalAlign="Left" SortExpression="userlogin_is_logged_off" HeaderStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblIsLoggedOff" runat="server" Text='<%# ((bool)Eval("userlogin_is_logged_off")) ? "Yes" : "No"  %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsLoggedOff" runat="server" Text='<%#   Eval("userlogin_is_logged_off") == DBNull.Value ? "" : (  ((bool)Eval("userlogin_is_logged_off")) ? "Yes" : "No"  )  %>' Font-Bold='<%# Eval("userlogin_is_successful") != DBNull.Value && ((bool)Eval("userlogin_is_successful") && Eval("userlogin_staff_id") != DBNull.Value && ((int)Eval("userlogin_staff_id")) >= 0) &&  !((bool)Eval("userlogin_is_logged_off"))  %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Login Time"  HeaderStyle-HorizontalAlign="Left" SortExpression="userlogin_login_time" ItemStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblLoginTime" runat="server" Text='<%# Bind("userlogin_login_time", "{0:yyyy-MM-dd HH:mm:ss}")  %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLoginTime" runat="server" Text='<%# Bind("userlogin_login_time", "{0:yyyy-MM-dd HH:mm:ss}") %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value  || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Last Access Time"  HeaderStyle-HorizontalAlign="Left" SortExpression="userlogin_last_access_time" ItemStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblLastAccessTime" runat="server" Text='<%# Bind("userlogin_last_access_time", "{0:yyyy-MM-dd HH:mm:ss}") %>'></asp:Label>
                                    &nbsp;
                                    <asp:LinkButton ID="lblLastAccessPageToolTip" runat="server" Text="?" ToolTip='<%# Bind("userlogin_last_access_page") %>' Visible='<%# ((string)Eval("userlogin_last_access_page")).Length > 0 %>' OnClientClick="javascript:return false;" />
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLastAccessTime" runat="server" Text='<%# Bind("userlogin_last_access_time", "{0:yyyy-MM-dd HH:mm:ss}") %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value  || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                    &nbsp;
                                    <asp:HyperLink ID="lnkLastAccessPageToolTip" runat="server" Text="?" ToolTip='<%# Bind("userlogin_last_access_page") %>' NavigateUrl='<%# Bind("userlogin_last_access_page") %>' Visible='<%# Eval("userlogin_is_successful") == DBNull.Value || ((string)Eval("userlogin_last_access_page")).Length > 0 %>' onclick="return false;" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="IP Address"  HeaderStyle-HorizontalAlign="Left" SortExpression="userlogin_ipaddress" HeaderStyle-CssClass="nowrap"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblIPAddress" runat="server" Text='<%# Bind("userlogin_ipaddress") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIPAddress" runat="server" Text='<%# Bind("userlogin_ipaddress") %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value  || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Country"  HeaderStyle-HorizontalAlign="Left"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblCountry" runat="server"></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblCountry" runat="server" ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value  || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="SessionID"  HeaderStyle-HorizontalAlign="Left" SortExpression="userlogin_session_id" Visible="false"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblSessionID" runat="server" Text='<%# Bind("userlogin_session_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSessionID" runat="server" Text='<%# Bind("userlogin_session_id") %>' ForeColor='<%# Eval("userlogin_is_successful") == DBNull.Value || !(bool)Eval("userlogin_is_successful") || Eval("userlogin_staff_id") == DBNull.Value  || ((int)Eval("userlogin_staff_id")) >= 0 ? System.Drawing.Color.Black : System.Drawing.Color.Gray %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <%--<asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-16.png" />--%>

                            <asp:TemplateField HeaderText="Log Off" ShowHeader="True" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle"  HeaderStyle-CssClass="nowrap">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnLogOff" runat="server"  CommandName="_LogOff" CommandArgument='<%# Bind("userlogin_userlogin_id") %>' ImageUrl="~/images/Delete-icon-16.png" AlternateText="Delete" ToolTip="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns> 

                    </asp:GridView>

                </div>
            </center>

        </div>
    </div>

</asp:Content>



