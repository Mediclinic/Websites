<%@ Page Title="Welcome" Language="C#" MasterPageFile="~/SiteV2.master" AutoEventWireup="true" CodeFile="DefaultTapHoldV2.aspx.cs" Inherits="_DefaultTapHoldV2" %>
<%@ Register TagPrefix="UC" TagName="BookingModalElementControl" Src="~/Bookings/BookingModalElementControlV2.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

    <link href="Styles/booking_modal_boxV2.css" rel="stylesheet" type="text/css" />

    <script src="http://code.jquery.com/mobile/1.1.0/jquery.mobile-1.1.0.min.js"></script>

    <script type="text/javascript">

        function reveal_modal(divID) {

            window.onscroll = function () { document.getElementById(divID).style.top = document.body.scrollTop; };
            document.getElementById(divID).style.display = "block";
            document.getElementById(divID).style.top = document.body.scrollTop;
        }
        function hide_modal(divID) {
            document.getElementById(divID).style.display = "none";
        }

    </script>


</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

        <UC:BookingModalElementControl ID="bookingModalElementControl" runat="server" />

        <div data-role="content">	
            <div id="testy"  class="touchableImage">
                Test Here...
                <br />
                <br />
                .....
            </div>

            <table>
                <tr>
                    <td id="someidhere" class="tapholdtest">
                        TD CELL HERE <br />
                        TAP HOLD IT
                    </td>
                </tr>
            </table>

        </div>


    <br />
    <br />
    <br />
    <br />


    <asp:Label ID="lblOutput" runat="server"></asp:Label>


    <br />
    <br />
    <br />
    <br />
    <br />
    <br />


    
        <script>


            $(document).ready(function () {
                $('td.tapholdtest').contextMenu('ContextMenu_Past_Completed_Has_Invoice', {

                    bindings: {
                        'pchi_viewinvoice': function (t) {
                            ;
                        },
                        'pchi_futurebooking': function (t) {
                            ;
                        },
                        'pchi_printinvoice': function (t) {
                            ;
                        },
                        'pchi_emailinvoice': function (t) {
                            ;
                        },
                        'pchi_reversebooking': function (t) {
                            ;
                        },
                        'pchi_printletters': function (t) {
                            ;
                        }
                    }
                });
            });




            $("td.tapholdtest").on("taphold", function (e) {
                e.stopPropagation();
                //alert(e.target.id);
                // reveal_modal('modalPage');



                // ----------------------------------------------------------------------------------------
                // ok ... now try this on the booking sheet .. and put it on the server and see if it works
                // ----------------------------------------------------------------------------------------



                var text = " \
                <a href=\"#\" style=\"text-decoration:none\" onclick=\"alert('1st clicked - " + e.target.id + "');\">First</a><br /> \
                <a href=\"#\" style=\"text-decoration:none\" onclick=\"alert('2nd clicked - " + e.target.id + "');\">Second</a><br /><br /> \
                ";


                var text2 = "\
                    <table style=\"margin-left:auto !important; margin-right:auto !important; vertical-align:middle !important; text-align:center; width:100%;\" border=\"1\">\
                        <tr><td><a href=\"#\" style=\"text-decoration:none\" onclick=\"alert('1st clicked - " + e.target.id + "');\">First</a></td></tr>\
                        <tr><td><a href=\"#\" style=\"text-decoration:none\" onclick=\"alert('2nd clicked - " + e.target.id + "');\">Second</a></td></tr>\
                        <tr><td><a href=\"#\" style=\"text-decoration:none\" onclick=\"alert('3rd clicked - " + e.target.id + "');\">Third</a></td></tr>\
                    </table>";

                document.getElementById("div_modalPopupGeneric").innerHTML = text2;
                document.getElementById('spnModalPage_Generic').style.width = "300px";
                document.getElementById('spnModalPage_Generic').style.height = "172px";
                reveal_modal('modalPopup_Generic');
                





                /*
                alert('aa');

                var element = document.getElementById(e.target.id);

                if (document.createEvent) {
                    alert('xx-1');
                    var ev = document.createEvent('HTMLEvents');
                    alert('xx-2');
                    ev.initEvent('contextMenu', true, false);
                    alert('xx-3');
                    element.dispatchEvent(ev);
                    alert('xx-4');
                } else { // Internet Explorer
                    alert('yy');
                    element.fireEvent('oncontextmenu');
                }
                */


                //alert('bb');

                //e.target.fireEvent('contextMenu');

                //alert('cc');

            });



            $(".touchableImage").on("taphold", function (e) {

                alert('--aa');
                //e.stopPropagation();
                //alert('bb');

                /// actually .. try show right click menu .... 

                //reveal_modal('modalPage');

                $(this).simpledialog2({
                    mode: "blank",
                    headerText: "Image Options",
                    showModal: false,
                    forceInput: true,
                    headerClose: true,
                    blankContent: "<ul data-role='listview'><li><a href='' onclick='alert(\"fb clicked\")'>Send to Facebook</a></li><li><a href=''>Send to Twitter</a></li><li><a href=''>Send to Cat</a></li></ul>"
                });

                alert('cc');
            });
        </script>

</asp:Content>



