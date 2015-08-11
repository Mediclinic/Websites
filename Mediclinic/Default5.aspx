<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="Default5.aspx.cs" Inherits="_Default5" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

    <style type="text/css"> 
        #dialogoverlay{ 
            display: none; 
            opacity: .6; 
            position: fixed; 
            top: 0px; 
            left: 0px; 
            background: #FFF; 
            width: 100%; 
            z-index: 10; 

        } 
        #dialogbox{ 
            display: none; 
            position: fixed; 
            background: #000; 
            /*border-radius:9px; */
            width:550px; 
            z-index: 10; 

        } 
        #dialogbox > div{ background:#FFF; margin: 1px 2px 2px 1px;  } 
        #dialogbox > div > #dialogboxhead{ background: #58aac9; font-size:17px; font-weight:bold; padding:5px; color:black; text-align:center; } 
        #dialogbox > div > #dialogboxbody{ background: #d8f3f3; padding:6px; color:black; } 
        #dialogbox > div > #dialogboxfoot{ background: #58aac9; padding:5px; text-align:right; } 

        #dialogbox > div > #seperator1{ background: #777; height:1px; } 
        #dialogbox > div > #seperator2{ background: #777; height:1px; } 
    </style>
    <script>
        /* http://www.developphp.com/view.php?tid=1384 */
        function CustomAlert(){ 
            this.render = function(heading, dialog){ 
                var winW = window.innerWidth; 
                var winH = window.innerHeight; 
                var dialogoverlay = document.getElementById('dialogoverlay'); 
                var dialogbox = document.getElementById('dialogbox'); 
                dialogoverlay.style.display = "block"; 
                dialogoverlay.style.height = winH+"px"; 
                dialogbox.style.left = (winW/2) - (550 * .5)+"px"; 
                dialogbox.style.top = "100px"; 
                dialogbox.style.display = "block"; 
                document.getElementById('dialogboxhead').innerHTML = heading;
                document.getElementById('dialogboxbodytext').innerHTML = dialog;
                document.getElementById('dialogboxfoot').innerHTML = '<button onclick="Alert.ok()">OK</button>';

                document.getElementById('dialogboxhead').style.display = heading == '' ? "none" : "block";
                document.getElementById('dialogboxhead').style.display = heading == '' ? "none" : "block";

            }
            this.ok = function () {
                document.getElementById('dialogbox').style.display = "none";
                document.getElementById('dialogoverlay').style.display = "none";
            }
        }
        var Alert = new CustomAlert();

    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div id="dialogoverlay"></div> 
    <div id="dialogbox"> 
        <div> 
            <div id="dialogboxhead"></div> 
            <div id="seperator1"></div> 
            <div id="dialogboxbody">
                <center>
                    <table>
                        <tr>
                            <td>
                                <span id="dialogboxbodytext"></span>
                            </td>
                        </tr>
                    </table>
                </center>
            </div> 
            <div id="seperator2"></div> 
            <div id="dialogboxfoot"></div> 
        </div> 
    </div>

    <div class="clearfix">
        <div class="page_title"><span id="lblHeading">&nbsp;</span></div>
        <div class="main_content" id="main_content" runat="server" style="background: url(../imagesV2/login_bg.png) center top no-repeat #EDEDED;">
            <div class="text-center">

                <div class="page_title">
                    <h3>Welcome <asp:Label ID="lblStaffName" runat="server"/>!</h3>
                    <div style="height:5px;"></div>
                    <h4>You Are Logged In To  <asp:Label ID="lblSiteName" runat="server"/></h4>
                </div>
                <br />

                <button onclick="alert('You look very pretty today.')">Default Alert</button>
                <button onclick="Alert.render('AA','You look very pretty today.');return false;">Custom Alert</button>
                <button onclick="Alert.render('','And you also smell very nice.<br />And you also smell <b>very nice</b>. And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />And you also smell very nice.<br />');return false;">Custom Alert 2</button>

                <asp:Label ID="lblTest" runat="server"></asp:Label>

                <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification" ValidationGroup="ValidationSummary"/>
                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" CssClass="failureNotification"></asp:Label>
            </div>

            <br />
            --
            <br />
            <br />

            <!-- http://www.codeproject.com/Articles/235117/ASP-NET-Chart-Control
                https://msdn.microsoft.com/en-us/library/dd456764%28v=vs.140%29.aspx
                 -->


            <asp:Chart ID="cTestChart2" runat="server" BorderlineColor="Black"
               BorderlineDashStyle="Solid" BackColor="182, 214, 236" BackGradientStyle="TopBottom"
               BackSecondaryColor="White" Width="1000px" Height="1000px">

               <Titles>
                  <asp:Title Name="Title1" Text="Bookings" Alignment="TopCenter" Font="Verdana, 12pt, style=Bold" />
               </Titles>

               <Series>
                   <asp:Series Name="Series1"  IsValueShownAsLabel="True" ChartType="StackedBar">
                   </asp:Series>
               </Series>

               <ChartAreas>
                  <asp:ChartArea Name="ChartArea1" BackGradientStyle="TopBottom" BackSecondaryColor="#B6D6EC" BorderDashStyle="Solid" BorderWidth="2">
                     <AxisX>
                        <MajorGrid Enabled="False" />
                     </AxisX>
                  </asp:ChartArea>
               </ChartAreas>

            </asp:Chart>




            <br />
            <br />
            --
            <br />



            <div id="autodivheight" class="divautoheight" style="height:200px;">
            </div>

        </div>
    </div>

</asp:Content>



