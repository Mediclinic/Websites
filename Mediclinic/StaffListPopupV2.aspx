<%@ Page Title="Staff" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="StaffListPopupV2.aspx.cs" Inherits="StaffListPopupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="Scripts/get_details_of_person.js" type="text/javascript"></script>
    <script type="text/javascript">
        function select_staff(val) {
            window.returnValue = val;
            self.close();
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Staff</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">
            <div class="user_login_form" style="width: 600px;">

                <div class="border_top_bottom">
                    <center>

                        <table>
                            <tr>
                                <td>
                                    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
                                </td>
                            </tr>
                        </table>

                        <table class="padded-table-2px">
                            <tr>
                                <td><asp:Label ID="lblSearchSurname" runat="server">Narrow Below List By Surname: </asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtSearchSurname" runat="server"></asp:TextBox>
                                    <asp:CheckBox ID="chkSurnameSearchOnlyStartWith" runat="server" Text="starts with" Font-Size="X-Small" Checked="true" />
                                </td>
                                <td><asp:Button ID="btnSearchSurname" runat="server" Text="Search" onclick="btnSearchSurname_Click" /></td>
                                <td><asp:Button ID="btnClearSurname" runat="server" Text="Clear" onclick="btnClearSurnameSearch_Click" /></td>
                            </tr>
                        </table>


                    </center>
                </div>

            </div>


            <center>

                <div id="autodivheight" class="divautoheight" style="height:500px;width:auto;padding-right:17px;">

                    <asp:GridView ID="GrdStaff" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="staff_id" 
                        OnRowCancelingEdit="GrdStaff_RowCancelingEdit" 
                        OnRowDataBound="GrdStaff_RowDataBound" 
                        OnRowEditing="GrdStaff_RowEditing" 
                        OnRowUpdating="GrdStaff_RowUpdating" ShowFooter="False" 
                        OnRowCommand="GrdStaff_RowCommand" 
                        OnRowDeleting="GrdStaff_RowDeleting" 
                        OnRowCreated="GrdStaff_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GrdStaff_Sorting"
                        RowStyle-VerticalAlign="top"
                        AllowPaging="False"
                        OnPageIndexChanging="GrdStaff_PageIndexChanging"
                        PageSize="15"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="staff_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("staff_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Title" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Firstname" HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFirstname" runat="server" Text='<%# Bind("firstname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="M.name" HeaderStyle-HorizontalAlign="Left" SortExpression="middlename" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMiddlename" runat="server" Text='<%# Bind("middlename") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Surname" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSurname" runat="server" Text='<%# Bind("surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Gender" HeaderStyle-HorizontalAlign="Left" SortExpression="gender" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblGender" runat="server" Text='<%# ( Eval("gender").ToString() == "M")?"Male" : (( Eval("gender").ToString() == "F")?"Female" : "-") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Button ID="btnSelect" runat="server" Text="Select" />
                                </ItemTemplate> 
                            </asp:TemplateField> 

                        </Columns> 
                    </asp:GridView>
                    
                </div>

                <br />
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="window.returnValue=false;self.close();" />

            </center>

        </div>
    </div>


</asp:Content>



