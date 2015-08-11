using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxBookingUpdateShowHeaderSection : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string show = Request.QueryString["show"];

            if (show == null || (show != "1" && show != "0"))
                throw new CustomMessageException("URL field show does not exist");

            Session["ShowHeaderOnBookingScreen"] = show != "0";

            if (!(new List<int> { -5, -7, -8 }).Contains((int)Session["StaffID"])) 
                StaffDB.UpdateShowHeaderOnBookingScreen(Convert.ToInt32(Session["StaffID"]), show != "0");
            else
                StaffDB.UpdateShowHeaderOnBookingScreen(Convert.ToInt32(Session["PreviousStaffID"]), show != "0", Convert.ToString(Session["PreviousDB"]));
        }
        catch (SessionTimedOutException)
        {
            Utilities.UnsetSessionVariables();
            Response.Write("SessionTimedOutException");
        }
        catch (Exception ex)
        {
            Response.Write((Utilities.IsDev() ? "Exception: " + ex.ToString() : "Error - please contact system administrator."));
        }
    }
}