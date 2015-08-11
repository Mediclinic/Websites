using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxGetOfferingInfo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string offering_id = Request.QueryString["offering"];
            if (offering_id == null || !Regex.IsMatch(offering_id, @"^\-?\d+$"))
                throw new CustomMessageException();

            Offering offering = OfferingDB.GetByID(Convert.ToInt32(offering_id));
            if (offering_id == "-1" || offering == null)
                throw new CustomMessageException();


            string fieldsSep  = "<<sep>>";
            string serialized = 
                offering.Name               + fieldsSep +
                offering.ServiceTimeMinutes + fieldsSep +
                offering.PopupMessage       + fieldsSep +
                offering.Field.ID           + fieldsSep +
                offering.Field.Descr        + fieldsSep;

            Response.Write(serialized);
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