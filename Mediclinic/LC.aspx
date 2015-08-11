<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LC.aspx.cs" Inherits="LC" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="ScriptsV2/jquery-1.8.3.js"></script>
    
    <script type='text/javascript'>
        $(function () {
            $("#area").mousedown(function () {
                $("#result").html("Waiting for it...");
            });
            $("#area").longclick(500, function () {
                $("#result").html("You longclicked. Nice!");
            });
            $("#area").click(function () {
                $("#result").html("You clicked. Bummer.");
            });



        });

    </script>



    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>



  <script src="https://raw.github.com/pisi/Longclick/master/jquery.longclick-min.js" type="text/javascript"></script>

<p id="area">Click me!</p>
<p id="result">You didn't click yet.</p>

    
    </div>
    </form>
</body>
</html>
