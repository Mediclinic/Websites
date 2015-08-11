using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxGetStaffName : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string staff_id = Request.QueryString["staff"];
            if (staff_id == null || !Regex.IsMatch(staff_id, @"^\-?\d+$"))
                throw new CustomMessageException();

            Staff staff = StaffDB.GetByID(Convert.ToInt32(staff_id));
            if (staff_id == "-1" || staff == null)
                throw new CustomMessageException();

            Response.Write(staff.Person.FullnameWithoutMiddlename);
        }
        catch (SessionTimedOutException)
        {
            Utilities.UnsetSessionVariables();
            Response.Write("SessionTimedOutException");
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "please contact system administrator."));
        }
    }
}