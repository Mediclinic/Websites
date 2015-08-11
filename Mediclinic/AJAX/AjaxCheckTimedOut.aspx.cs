using System;
using System.Web;

public partial class AjaxCheckTimedOut : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        Response.Write((Session == null || Session["DB"] == null) ? "1" : "0");
    }
}