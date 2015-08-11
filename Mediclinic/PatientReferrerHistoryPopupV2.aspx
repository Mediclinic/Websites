<%@ Page Title="Patient Edit History" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="PatientReferrerHistoryPopupV2.aspx.cs" Inherits="PatientReferrerHistoryPopupV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Patient Referrer History</span></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdPatientReferrer" runat="server" 
                            AutoGenerateColumns="False" DataKeyNames="pr_patient_referrer_id" 
                            OnRowCancelingEdit="GrdPatientReferrer_RowCancelingEdit" 
                            OnRowDataBound="GrdPatientReferrer_RowDataBound" 
                            OnRowEditing="GrdPatientReferrer_RowEditing" 
                            OnRowUpdating="GrdPatientReferrer_RowUpdating" ShowFooter="False" 
                            OnRowCommand="GrdPatientReferrer_RowCommand" 
                            OnRowDeleting="GrdPatientReferrer_RowDeleting" 
                            OnRowCreated="GrdPatientReferrer_RowCreated"
                            AllowSorting="True" 
                            OnSorting="GridView_Sorting"
                            RowStyle-VerticalAlign="top"
                            AllowPaging="False"
                            ClientIDMode="Predictable"
                            CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="pr_patient_referrer_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("pr_patient_referrer_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Referrer" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblRegisterReferrer" runat="server" Text='<%# ( (Eval("referrer_person_title_descr") == DBNull.Value || (int)Eval("referrer_person_title_title_id") == 0) ? "" : Eval("referrer_person_title_descr") + " ") + Eval("referrer_person_firstname") + " " + Eval("referrer_person_surname")  %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Date Added" HeaderStyle-HorizontalAlign="Left" SortExpression="pr_patient_referrer_date_added" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDateAdded" runat="server" Text='<%# Bind("pr_patient_referrer_date_added", "{0:dd-MM-yyyy HH:mm}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Active" SortExpression="pr_is_active" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsActive" runat="server" Text='<%# Eval("pr_is_active").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
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



