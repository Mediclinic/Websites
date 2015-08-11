<%@ Page Title="Products & Services" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="OfferingListPopupV2.aspx.cs" Inherits="OfferingListPopupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        function select_offering(val) {
            window.returnValue = val;
            self.close();
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Products & Services</asp:Label></div>
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

                        <table>
                            <tr valign="top">
                                <td>
                                    <asp:Label ID="lblSearchName" runat="server">Search By Name: </asp:Label>
                                    <asp:TextBox ID="txtSearchOffering" runat="server"></asp:TextBox>
                                    <asp:CheckBox ID="chkOfferingSearchOnlyStartWith" runat="server" Text="starts with" Font-Size="X-Small" Checked="true" />
                                    <asp:Button ID="btnSearchOffering" runat="server" Text="Search" onclick="btnSearchOffering_Click" />
                                    <asp:Button ID="btnClearOffering" runat="server" Text="Clear" onclick="btnClearOfferingSearch_Click" />
                                </td>
                            </tr>
                        </table>

                    </center>
                </div>

            </div>


            <center>

                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdOffering" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="o_offering_id" 
                        OnRowCancelingEdit="GrdOffering_RowCancelingEdit" 
                        OnRowDataBound="GrdOffering_RowDataBound" 
                        OnRowEditing="GrdOffering_RowEditing" 
                        OnRowUpdating="GrdOffering_RowUpdating" ShowFooter="False" 
                        OnRowCommand="GrdOffering_RowCommand" 
                        OnRowDeleting="GrdOffering_RowDeleting" 
                        OnRowCreated="GrdOffering_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GrdOffering_Sorting"
                        RowStyle-VerticalAlign="top"
                        AllowPaging="True"
                        OnPageIndexChanging="GrdOffering_PageIndexChanging"
                        PageSize="15"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />


                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="o_offering_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("o_offering_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left" SortExpression="o_name"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblName" runat="server" Text='<%# Bind("o_name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="nowrap" SortExpression="type_descr" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblType" runat="server" Text='<%# Eval("type_descr") %>'></asp:Label> 
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



