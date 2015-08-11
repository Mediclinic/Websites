<%@ Page Title="Maintain Treatment Letters" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Letters_MaintainTreatmentLettersV2.aspx.cs" Inherits="Letters_MaintainTreatmentLettersV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <!--script type="text/javascript" src="ScriptsV2/autodivheight.js" ></!--script-->
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Maintain Treatment Letters</asp:Label> &nbsp; <asp:HyperLink ID="lnkToEntity" runat="server"></asp:HyperLink></div>
        <div class="main_content" style="padding:20px 5px;">

            <div class="user_login_form">

                <img src="imagesV2/edit.png" alt="edit icon" style="margin:0 5px 5px 0;" />Edit
                <span style="padding-left:50px;"><img src="imagesV2/x.png" alt="delete icon" style="margin:0 5px 5px 0;" />Delete</span>
            </div>

            <div class="block_center">
                <asp:ValidationSummary ID="ValidationSummaryAdd" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryAdd"/>
                <asp:ValidationSummary ID="ValidationSummaryEdit" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummaryEdit"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <center>
                <div id="__autodivheight" class="divautoheight" >

                    <asp:GridView ID="GrdTreatmentTemplateLetters" runat="server" 
                         AutoGenerateColumns="False" DataKeyNames="lettertreatmenttemplate_letter_treatment_template_id" 
                         OnRowCancelingEdit="GrdTreatmentTemplateLetters_RowCancelingEdit" 
                         OnRowDataBound="GrdTreatmentTemplateLetters_RowDataBound" 
                         OnRowEditing="GrdTreatmentTemplateLetters_RowEditing" 
                         OnRowUpdating="GrdTreatmentTemplateLetters_RowUpdating" ShowFooter="True" 
                         OnRowCommand="GrdTreatmentTemplateLetters_RowCommand" 
                         OnRowDeleting="GrdTreatmentTemplateLetters_RowDeleting" 
                         OnRowCreated="GrdTreatmentTemplateLetters_RowCreated"
                         AllowSorting="True" 
                         OnSorting="GridView_Sorting"
                         RowStyle-VerticalAlign="top"
                         ClientIDMode="Predictable"
                         CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center max_width_100pc;">

                        <Columns> 

                            <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="lettertreatmenttemplate_letter_treatment_template_id"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("lettertreatmenttemplate_letter_treatment_template_id") %>'></asp:Label>
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("lettertreatmenttemplate_letter_treatment_template_id") %>'></asp:Label> 
                                </ItemTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="EPC/Ref. Type" HeaderStyle-HorizontalAlign="Left" SortExpression="field_descr" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:Label ID="lblField" runat="server" Text='<%# Eval("field_descr") %>'></asp:Label> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblField" runat="server" Text='<%# Eval("field_descr") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewField" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="First Letter" HeaderStyle-HorizontalAlign="Left" SortExpression="firstletter_docname" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlFirstLetter" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblFirstLetter" runat="server" Text='<%# Eval("firstletter_docname") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewFirstLetter" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Last Letter" HeaderStyle-HorizontalAlign="Left" SortExpression="lastletter_docname" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlLastLetter" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLastLetter" runat="server" Text='<%# Eval("lastletter_docname") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewLastLetter" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Last Letter PT" HeaderStyle-HorizontalAlign="Left" SortExpression="lastletterpt_docname" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlLastLetterPT" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLastLetterPT" runat="server" Text='<%# Eval("lastletterpt_docname") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewLastLetterPT" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Last Letter When EPC/Ref. Replaced" HeaderStyle-HorizontalAlign="Left" SortExpression="lastletterwhenreplacingepc_docname" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlLastLetterWhenReplacingEPC" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblLastLetterWhenReplacingEPC" runat="server" Text='<%# Eval("lastletterwhenreplacingepc_docname") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewLastLetterWhenReplacingEPC" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="Treatment Note Letter" HeaderStyle-HorizontalAlign="Left" SortExpression="treatmentnotesletter_docname" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:DropDownList ID="ddlTreatmentNoteLetter" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList> 
                                </EditItemTemplate> 
                                <ItemTemplate> 
                                    <asp:Label ID="lblTreatmentNoteLetter" runat="server" Text='<%# Eval("treatmentnotesletter_docname") %>'></asp:Label> 
                                </ItemTemplate> 
                                <FooterTemplate> 
                                    <asp:DropDownList ID="ddlNewTreatmentNoteLetter" runat="server" DataTextField="descr" DataValueField="field_id"> </asp:DropDownList>
                                </FooterTemplate> 
                            </asp:TemplateField> 


                            <asp:TemplateField HeaderText="" ShowHeader="False" HeaderStyle-HorizontalAlign="Left" FooterStyle-VerticalAlign="Top"> 
                                <EditItemTemplate> 
                                    <asp:LinkButton ID="lnkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ValidationGroup="ValidationSummaryEdit"></asp:LinkButton> 
                                    <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton> 
                                </EditItemTemplate> 
                                <FooterTemplate> 
                                    <asp:LinkButton ID="lnkAdd" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ValidationGroup="ValidationSummaryAdd"></asp:LinkButton> 
                                </FooterTemplate> 
                                <ItemTemplate> 
                                   <asp:ImageButton ID="lnkEdit" runat="server" CommandName="Edit" ImageUrl="~/images/Inline-edit-icon-24.png"  AlternateText="Inline Edit" ToolTip="Inline Edit"/>
                                </ItemTemplate> 
                            </asp:TemplateField> 

                            <asp:CommandField HeaderText="" ShowDeleteButton="True" ShowHeader="True" ButtonType="Image"  DeleteImageUrl="~/images/Delete-icon-24.png" /> 

                        </Columns> 
                    </asp:GridView>

                </div>
            </center>

            <div id="autodivheight" class="divautoheight" style="height:500px;">
            </div>


        </div>
    </div>


</asp:Content>



