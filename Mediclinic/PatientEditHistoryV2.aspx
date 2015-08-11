<%@ Page Title="Patient Edit History" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="PatientEditHistoryV2.aspx.cs" Inherits="PatientEditHistoryV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">Patient Information Editing History</span></div>
        <div class="main_content" style="padding:20px 5px;">

             <center>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>


                <div id="autodivheight" class="divautoheight" style="height:500px;">

                    <asp:GridView ID="GrdPatient" runat="server" 
                            AutoGenerateColumns="False" DataKeyNames="patient_id" 
                            OnRowCancelingEdit="GrdPatient_RowCancelingEdit" 
                            OnRowDataBound="GrdPatient_RowDataBound" 
                            OnRowEditing="GrdPatient_RowEditing" 
                            OnRowUpdating="GrdPatient_RowUpdating" ShowFooter="False" 
                            OnRowCommand="GrdPatient_RowCommand" 
                            OnRowDeleting="GrdPatient_RowDeleting" 
                            OnRowCreated="GrdPatient_RowCreated"
                            AllowSorting="True" 
                            OnSorting="GridView_Sorting"
                            RowStyle-VerticalAlign="top"
                            AllowPaging="False"
                            ClientIDMode="Predictable"
                            CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_history_id"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval("patient_history_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Title" HeaderStyle-HorizontalAlign="Left" SortExpression="descr" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("descr") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Firstname" HeaderStyle-HorizontalAlign="Left" SortExpression="firstname" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFirstname" runat="server" Text='<%# Eval("firstname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="M.name" HeaderStyle-HorizontalAlign="Left" SortExpression="middlename" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblMiddlename" runat="server" Text='<%# Eval("middlename") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Surname" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblSurname" runat="server" Text='<%# Eval("surname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Nickname" HeaderStyle-HorizontalAlign="Left" SortExpression="nickname" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblNickname" runat="server" Text='<%# Eval("nickname") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Gender" HeaderStyle-HorizontalAlign="Left" SortExpression="gender" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblGender" runat="server" Text='<%# ( Eval("gender").ToString() == "M")?"Male" : (( Eval("gender").ToString() == "F")?"Female" : "-") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="D.O.B." HeaderStyle-HorizontalAlign="Left" SortExpression="dob" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDOB" runat="server" Text='<%# Eval("dob", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Diabetic" SortExpression="is_diabetic" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDiabetic" runat="server" Text='<%# Eval("is_diabetic").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Member Diabetes Aus." SortExpression="is_member_diabetes_australia" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsMemberDiabetesAustralia" runat="server" Text='<%# Eval("is_member_diabetes_australia").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="DA Review" SortExpression="diabetic_assessment_review_date" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblDAReview" runat="server" Text='<%# Eval("diabetic_assessment_review_date", "{0:dd-MM-yyyy}") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Clinic Patient" SortExpression="is_clinic_patient" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsClinicPatient" runat="server" Text='<%# Eval("is_clinic_patient").ToString() == "True" ? "Yes" : "No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="GP Patient" SortExpression="is_gp_patient" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsGPPatient" runat="server" Text='<%# Eval("is_gp_patient").ToString() == "True" ? "Yes" : "No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="AC Type" HeaderStyle-HorizontalAlign="Left" SortExpression="ac_offering" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblACInvOffering" runat="server" Text='<%# Eval("ac_offering")  %>' ></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Username" HeaderStyle-HorizontalAlign="Left" SortExpression="login" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLogin" runat="server" Text='<%# Eval("login")  %>' ></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Pwd" HeaderStyle-HorizontalAlign="Left" SortExpression="pwd" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblPwd" runat="server" Text='<%# Eval("pwd")  %>' ></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Is Company" SortExpression="is_company" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsCompany" runat="server" Text='<%# Eval("is_company").ToString() == "True" ? "Yes" : "No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="ABN" HeaderStyle-HorizontalAlign="Left" SortExpression="abn" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblABN" runat="server" Text='<%# Eval("abn")  %>' ></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Deceased" SortExpression="is_deceased" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDeceased" runat="server" Text='<%# Eval("is_deceased").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Deleted" SortExpression="is_deleted" FooterStyle-VerticalAlign="Top"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblIsDeleted" runat="server" Text='<%# Eval("is_deleted").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Mofidified From This"  HeaderStyle-HorizontalAlign="Left" SortExpression="date_added"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("date_added", "{0:dd-MM-yy}") %>'></asp:Label>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Mofidified By"  HeaderStyle-HorizontalAlign="Left" SortExpression="staff_person_surname"> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblAddedBy" runat="server" Text='<%# Eval("staff_person_firstname") + " " + Eval("staff_person_surname") %>'></asp:Label>
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



