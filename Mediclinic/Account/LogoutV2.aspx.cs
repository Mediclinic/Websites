using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;

public partial class LogoutV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        LogoutAll();
    }


    protected void LogoutAll()
    {
        try {
            if (Session != null && Session["StaffID"] != null && Session["DB"] != null)
                UserLoginDB.UpdateLoggedOffByStaffID(Convert.ToInt32(Session["StaffID"]));
        } catch(Exception) {}

        Utilities.UnsetSessionVariables();
        Utilities.LogoutV2(Session, Response, Request, false);
    }

}