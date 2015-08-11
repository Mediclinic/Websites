<%@ Page Title="Organisation List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="OrganisationListPopupV2.aspx.cs" Inherits="OrganisationListPopupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        function select_organisation(val) {
            window.returnValue = val;
            self.close();
        }
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Organisation List</asp:Label></div>
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
                            <tr style="vertical-align:top;">
                                <td runat="server" id="td_select_clinics_or_agedcare">
                                    <asp:CheckBox ID="chkIncClinics"  runat="server" oncheckedchanged="chkIncClinics_CheckedChanged"  AutoPostBack="true" Checked="True" /> Include Clinics 
                                    <br />
                                    <asp:CheckBox ID="chkIncAgedCare" runat="server" oncheckedchanged="chkIncAgedCare_CheckedChanged" AutoPostBack="true" Checked="True" /> Include Aged Care 
                                </td>
                                <td style="width:25px"></td>
                                <td>
                                    <asp:Label ID="lblSearchName" runat="server">Search By Name: </asp:Label>
                                    <asp:TextBox ID="txtSearchOrganisation" runat="server"></asp:TextBox>
                                    <asp:CheckBox ID="chkOrganisationSearchOnlyStartWith" runat="server" Text="starts with" Font-Size="X-Small" Checked="true" />
                                    <asp:Button ID="btnSearchOrganisation" runat="server" Text="Search" onclick="btnSearchOrganisation_Click" />
                                    <asp:Button ID="btnClearOrganisation" runat="server" Text="Clear" onclick="btnClearOrganisationSearch_Click" />
                                </td>
                            </tr>
                        </table>


                    </center>
                </div>

            </div>


            <center>

                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdOrganisation" runat="server" 
                        AutoGenerateColumns="False" DataKeyNames="organisation_id" 
                        OnRowCancelingEdit="GrdOrganisation_RowCancelingEdit" 
                        OnRowDataBound="GrdOrganisation_RowDataBound" 
                        OnRowEditing="GrdOrganisation_RowEditing" 
                        OnRowUpdating="GrdOrganisation_RowUpdating" ShowFooter="False" 
                        OnRowCommand="GrdOrganisation_RowCommand" 
                        OnRowDeleting="GrdOrganisation_RowDeleting" 
                        OnRowCreated="GrdOrganisation_RowCreated"
                        AllowSorting="True" 
                        OnSorting="GrdOrganisation_Sorting"
                        RowStyle-VerticalAlign="top"
                        AllowPaging="True"
                        OnPageIndexChanging="GrdOrganisation_PageIndexChanging"
                        PageSize="15"
                        ClientIDMode="Predictable"
                        CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center">
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />


                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="organisation_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("organisation_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="text_left" SortExpression="name"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblName" runat="server" Text='<%# Bind("name") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="nowrap"  SortExpression="organisation_type_id" FooterStyle-VerticalAlign="Top"> 
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



