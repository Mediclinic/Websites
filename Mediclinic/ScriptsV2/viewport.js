// JavaScript Document

window.onload = function() {
    // if there is no session (session = false)
    if (!session) {
        // call function to get the screen size
        getScreenWidth();

        // make ajax call to php page to set the session variable
        setSession();
    }
}

function getScreenWidth() {
    layoutWidth = screen.width;
}

function setSession() {
    var xmlhttp;
    if (window.XMLHttpRequest)
      {// code for IE7+, Firefox, Chrome, Opera, Safari
      xmlhttp=new XMLHttpRequest();
      }
    else
      {// code for IE6, IE5
      xmlhttp=new ActiveXObject("Microsoft.XMLHTTP");
      }
    xmlhttp.onreadystatechange=function()
      {
      if (xmlhttp.readyState==4 && xmlhttp.status==200)
        {
        // Reload the page
        window.location.reload();
        }
      }
    xmlhttp.open("POST","set_session.php?layoutWidth=" + layoutWidth,true);
    xmlhttp.send();
}