<%@ Page Title="Birthdays List" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Notifications_PTWeeklyBirthdaysAutoEmailingV2.aspx.cs" Inherits="Notifications_PTWeeklyBirthdaysAutoEmailingV2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="Scripts/post_to_url.js"></script>
    <script type="text/javascript">

        function notification_info_edited() {

            //elem.style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkEnableEmails").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("txtEmailAddress").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 

            document.getElementById("chkSendMondays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendTuesdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendWednesdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendThursdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendFridays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendSaturdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("chkSendSundays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 


            document.getElementById("ddlFromDaysAheadMondays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlUntilDaysAheadMondays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlFromDaysAheadTuesdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlUntilDaysAheadTuesdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlFromDaysAheadWednesdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlUntilDaysAheadWednesdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlFromDaysAheadThursdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlUntilDaysAheadThursdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlFromDaysAheadFridays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlUntilDaysAheadFridays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlFromDaysAheadSaturdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlUntilDaysAheadSaturdays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlFromDaysAheadSundays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 
            document.getElementById("ddlUntilDaysAheadSundays").style.backgroundColor = '#FAFAD2';  // LightGoldenrodYellow 

            document.getElementById("btnUpdateNotificationInfo").className = ""; // make it visible
            document.getElementById("btnRevertNotificationInfo").className = ""; // make it visible

            document.getElementById("test_run_button_row").className = "hiddencol";
        }

        function open_new_window(URL) {
            NewWindow = window.open(URL, "_blank", "toolbar=no,menubar=0,status=0,copyhistory=0,scrollbars=yes,resizable=1,location=0,height=" + screen.height + ',width=' + screen.width);
            NewWindow.location = URL;
        }

        function go_to_print_batch() {
            var h = new Object(); // or just {}
            h['selected_patient_ids'] = document.getElementById('hiddenPatientIDs').value.trim();
            post_to_url("PrintBatchLetters.aspx", h, "post");
        }

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <asp:Button ID="btnDefaultButton_NoSubmit" runat="server" CssClass="hiddencol" OnClientClick="javascript:return false;" />

    <div class="clearfix">
        <div class="page_title"><asp:Label ID="lblHeading" runat="server">Birthdays List</asp:Label></div>
        <div class="main_content" style="padding:10px 5px;">

            <div class="user_login_form">

            </div>

            <div class="text-center">
                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>


            <asp:HiddenField ID="hiddenPatientIDs" runat="server" />


            <center>
                <table>
                    <tr style="vertical-align:top">
                        <td>



                            <table>
                                <tr>

                                    <!-- change this to just month/day in dropdowns ....  -->
                                    <td id="td_search_dates" runat="server">
                                        <table>

                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblSearchDate" runat="server">Start Daty/Month: </asp:Label></td>
                                                <td class="nowrap"><asp:DropDownList ID="ddlStartDate_Day" runat="server"></asp:DropDownList></td>
                                                <td>/</td>
                                                <td class="nowrap"><asp:DropDownList ID="ddlStartDate_Month" runat="server"></asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><asp:Label ID="lblEndDate" runat="server">End Day/Month: </asp:Label></td>
                                                <td class="nowrap"><asp:DropDownList ID="ddlEndDate_Day" runat="server"></asp:DropDownList></td>
                                                <td>/</td>
                                                <td class="nowrap"><asp:DropDownList ID="ddlEndDate_Month" runat="server"></asp:DropDownList></td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:30px"></td>

                                    <td class="nowrap">
                                        <asp:CheckBox ID="chkIncWithMobile" runat="server" Text="&nbsp;Inc With Mobile"/> 
                                        <br />
                                        <asp:CheckBox ID="chkIncWithEmail" runat="server" Text="&nbsp;Inc With Email"/> 
                                    </td>

                                    <td style="width:30px"></td>

                                    <td>
                                      <table>
                                            <tr>
                                                <td class="nowrap" align="center">
                                                    <asp:Button ID="btnSearch" runat="server" Text="Update" OnClick="btnSearch_Click" Width="100%" style="min-width:75px;" CssClass="thin_button" />
                                                    <div style="line-height:5px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap" align="center">
                                                    <asp:Button ID="btnExport" runat="server" Text="Export List" OnClick="btnExport_Click" Width="100%" CssClass="thin_button" />
                                                    <div style="line-height:5px;">&nbsp;</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap" align="center">
                                                    <asp:Button ID="btnPrintBatch" runat="server" Text="Print Letters" OnClientClick="go_to_print_batch(); return false;" Width="100%" CssClass="thin_button" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td style="width:15px"></td>

                                    <td><asp:CheckBox ID="chkUsePaging" runat="server" Text="&nbsp;use paging" Font-Size="X-Small" AutoPostBack="True" OnCheckedChanged="chkUsePaging_CheckedChanged" CssClass="nowrap" /></td>

                                </tr>
                            </table>

                            <br />

                            <div id="autodivheight" class="divautoheight" style="height:500px;margin-left:25px;">
                                <asp:GridView ID="GrdSummaryReport" runat="server" 
                                    AutoGenerateColumns="False" DataKeyNames="patient_id" 
                                    OnRowCancelingEdit="GrdSummaryReport_RowCancelingEdit" 
                                    OnRowDataBound="GrdSummaryReport_RowDataBound" 
                                    OnRowEditing="GrdSummaryReport_RowEditing" 
                                    OnRowUpdating="GrdSummaryReport_RowUpdating" ShowFooter="True" 
                                    OnRowCommand="GrdSummaryReport_RowCommand" 
                                    OnRowDeleting="GrdSummaryReport_RowDeleting" 
                                    OnRowCreated="GrdSummaryReport_RowCreated"
                                    AllowSorting="True" 
                                    OnSorting="GridView_Sorting"
                                    AllowPaging="True"
                                    OnPageIndexChanging="GrdStaff_PageIndexChanging"
                                    PageSize="17"
                                    ClientIDMode="Predictable"
                                    CssClass="table table-bordered table-striped table-grid table-grid-top-bottum-padding-thick auto_width block_center">
                                    <PagerSettings Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />

                                    <Columns> 

                                        <asp:TemplateField HeaderText="ID"  HeaderStyle-HorizontalAlign="Left" SortExpression="patient_id"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblId" runat="server" Text='<%# Bind("patient_id") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="D.O.B." HeaderStyle-HorizontalAlign="Left" SortExpression="dob" FooterStyle-VerticalAlign="Top" ItemStyle-CssClass="nowrap"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblDOB" runat="server" Text='<%# Bind("dob", "{0:d MMMMM, yyyy}") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" SortExpression="surname" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:HyperLink ID="lnkName" runat="server"  Text='<%# Eval("firstname") + " " + Eval("surname") + (Eval("t_title_id") == DBNull.Value || (int)Eval("t_title_id") == 0 ? "" :  " ("+Eval("t_descr")+")").ToString() %>' NavigateUrl='<%#  String.Format("~/PatientDetailV2.aspx?type=view&id={0}",Eval("patient_id"))%>' />
                                            </ItemTemplate> 
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Clinic Patient" SortExpression="is_clinic_patient" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsClinicPatient" runat="server" Text='<%# Eval("is_clinic_patient").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="GP Patient" SortExpression="is_gp_patient" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblIsGPPatient" runat="server" Text='<%# Eval("is_gp_patient").ToString()=="True"?"Yes":"No" %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>
            
                                        <asp:TemplateField HeaderText="Mobile" SortExpression="mobile" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("mobile") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>
            
                                        <asp:TemplateField HeaderText="Email" SortExpression="email" FooterStyle-VerticalAlign="Top"> 
                                            <ItemTemplate> 
                                                <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("email") %>'></asp:Label> 
                                            </ItemTemplate> 
                                        </asp:TemplateField>
             

                                    </Columns> 

                                </asp:GridView>
                            </div>

                        </td>

                        <td style="width:20px"></td>
                        <td style="min-width:1px;background-color:#606060"></td>
                        <td style="width:20px"></td>

                        <td>

                            <table>
                                <tr style="height:15px">
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <th colspan="3">Automated Email Birthday List Settings</th>
                                </tr>
                                <tr style="height:15px">
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td>Enable</td>
                                    <td style="width:15px"></td>
                                    <td><input type="checkbox" id="chkEnableEmails" runat="server" value="Accept Form" onclick="notification_info_edited()" /></td>
                                </tr>
                                <tr style="height:4px">
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td>Email</td>
                                    <td></td>
                                    <td><asp:TextBox ID="txtEmailAddress" runat="server" Columns="50" onkeyup="notification_info_edited();"></asp:TextBox></td>
                                </tr>
                                <tr style="height:4px">
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Inc Patients With Mobile</td>
                                    <td></td>
                                    <td><input type="checkbox" id="chkIncPatientsWithMobile" runat="server" value="Accept Form" onclick="notification_info_edited()" /></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Inc Patients With Email</td>
                                    <td></td>
                                    <td><input type="checkbox" id="chkIncPatientsWithEmail" runat="server" value="Accept Form" onclick="notification_info_edited()" /></td>
                                </tr>
                                <tr style="height:4px">
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td class="nowrap">Days To Send</td>
                                    <td></td>
                                    <td>

                                        <table>
                                            <tr>
                                                <td class="nowrap"><input type="checkbox" id="chkSendMondays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;Mondays</td>
                                                <td class="nowrap"> From <asp:DropDownList ID="ddlFromDaysAheadMondays" runat="server" onchange="notification_info_edited()"/> to <asp:DropDownList ID="ddlUntilDaysAheadMondays" runat="server" onchange="notification_info_edited()"/> Days Ahead</td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><input type="checkbox" id="chkSendTuesdays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;Tuesdays</td>
                                                <td class="nowrap"> From <asp:DropDownList ID="ddlFromDaysAheadTuesdays" runat="server" onchange="notification_info_edited()"/> to <asp:DropDownList ID="ddlUntilDaysAheadTuesdays" runat="server" onchange="notification_info_edited()"/> Days Ahead</td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><input type="checkbox" id="chkSendWednesdays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;Wednesdays</td>
                                                <td class="nowrap"> From <asp:DropDownList ID="ddlFromDaysAheadWednesdays" runat="server" onchange="notification_info_edited()"/> to <asp:DropDownList ID="ddlUntilDaysAheadWednesdays" runat="server" onchange="notification_info_edited()"/> Days Ahead</td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><input type="checkbox" id="chkSendThursdays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;Thursdays</td>
                                                <td class="nowrap"> From <asp:DropDownList ID="ddlFromDaysAheadThursdays" runat="server" onchange="notification_info_edited()"/> to <asp:DropDownList ID="ddlUntilDaysAheadThursdays" runat="server" onchange="notification_info_edited()"/> Days Ahead</td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><input type="checkbox" id="chkSendFridays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;Fridays</td>
                                                <td class="nowrap"> From <asp:DropDownList ID="ddlFromDaysAheadFridays" runat="server" onchange="notification_info_edited()"/> to <asp:DropDownList ID="ddlUntilDaysAheadFridays" runat="server" onchange="notification_info_edited()"/> Days Ahead</td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><input type="checkbox" id="chkSendSaturdays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;Saturdays</td>
                                                <td class="nowrap"> From <asp:DropDownList ID="ddlFromDaysAheadSaturdays" runat="server" onchange="notification_info_edited()"/> to <asp:DropDownList ID="ddlUntilDaysAheadSaturdays" runat="server" onchange="notification_info_edited()"/> Days Ahead</td>
                                            </tr>
                                            <tr>
                                                <td class="nowrap"><input type="checkbox" id="chkSendSundays" runat="server" value="Accept Form" onclick="notification_info_edited()" />&nbsp;Sundays</td>
                                                <td class="nowrap"> From <asp:DropDownList ID="ddlFromDaysAheadSundays" runat="server" onchange="notification_info_edited()"/> to <asp:DropDownList ID="ddlUntilDaysAheadSundays" runat="server" onchange="notification_info_edited()"/> Days Ahead</td>
                                            </tr>
                                        </table>

                                    </td>
                                </tr>
                                <tr style="height:20px">
                                    <td colspan="3"></td>
                                </tr>
                                <tr>
                                    <td colspan="3" align="center">
                                        <asp:Button ID="btnUpdateNotificationInfo" runat="server" Text="Update" OnClick="btnUpdateNotificationInfo_Click" />
                                        &nbsp;&nbsp;
                                        <asp:Button ID="btnRevertNotificationInfo" runat="server" Text="Revert" OnClick="btnRevertNotificationInfo_Click" />
                                    </td>
                                </tr>
                                <tr id="test_run_button_row">
                                    <td colspan="3" align="center">
                                        <asp:Button ID="btnTest" runat="server" Text="Run Test" OnClick="btnTest_Click" />
                                    </td>
                                </tr>

                            </table>

                        </td>
                    </tr>
                </table>
            </center>





        </div>
    </div>


</asp:Content>


