using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxCheckProviderNbrExists : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string provider_nrb = Request.QueryString["provider_nrb"];
            if (provider_nrb == null || provider_nrb.Trim().Length == 0)
                throw new CustomMessageException();

            provider_nrb = provider_nrb.Trim();

            string allNbrs1 = RegisterStaffDB.GetAllProviderNbrs();
            string allNbrs2 = StaffDB.GetAllProviderNbrs();
            Response.Write( (allNbrs1.Contains(provider_nrb) || allNbrs2.Contains(provider_nrb)) ? "1" : "0" );
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