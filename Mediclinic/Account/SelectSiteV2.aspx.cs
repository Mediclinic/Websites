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

public partial class SelectSiteV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            main_content.Style["background"] = (Session["SystemVariables"] == null) ? "url(../imagesV2/login_bg.png) center top no-repeat #EDEDED" : "url(../imagesV2/" + ((SystemVariables)Session["SystemVariables"])["MainLogoBackground"].Value + ") center top no-repeat #EDEDED";

        bool showPageHeader = Request.QueryString["show_header"] == null || Request.QueryString["show_header"] == "1";
        if (!showPageHeader)
            Utilities.UpdatePageHeaderV2(Page.Master, true);

        Staff staff = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));
        DataTable dt = StaffSiteRestrictionDB.GetDataTable_SitesNotRestricted(staff.StaffID, -1, false);

        lstSites.DataSource = dt;
        lstSites.DataBind();

        lblNoSitesMessage.Visible = dt.Rows.Count == 0;


        if (!IsPostBack)
        {
            if (dt.Rows.Count == 1)
            {
                Session["OrgID"] = null;
                Session["OrgName"] = null;
                Session["IsMultipleOrgs"] = false;
                Select(Convert.ToInt32(dt.Rows[0]["site_id"]));
            }
            else if (dt.Rows.Count == 2 && Session["SiteID"] != null) // if already in a site, just switch to the other one.
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Session["OrgID"] = null;
                    Session["OrgName"] = null;
                    Session["IsMultipleOrgs"] = false;
                    if (Convert.ToInt32(dt.Rows[i]["site_id"]) != Convert.ToInt32(Session["SiteID"]))
                    {
                        Select(Convert.ToInt32(dt.Rows[i]["site_id"]));
                        return;
                    }
                }
            }
        }

    }


    protected void lnkSelect_Click(object sender, CommandEventArgs e)
    {
        Select(Convert.ToInt32(e.CommandArgument));
    }
    protected void Select(int siteID)
    {
        Site site = SiteDB.GetByID(siteID);
        Session["SiteID"]         = site.SiteID;
        Session["SiteName"]       = site.Name;
        Session["SiteIsClinic"]   = site.SiteType.ID == 1;
        Session["SiteIsAgedCare"] = site.SiteType.ID == 2;
        Session["SiteIsGP"]       = site.SiteType.ID == 3;
        Session["SiteTypeID"]     = site.SiteType.ID;
        Session["SiteTypeDescr"]  = site.SiteType.Descr;


        if (Session["PatientID"] == null)
            UserLoginDB.UpdateSite(Convert.ToInt32(Session["StaffID"]), -1, siteID);
        else
            UserLoginDB.UpdateSite(-1, Convert.ToInt32(Session["PatientID"]), siteID);


        if (!Convert.ToBoolean(Session["IsAdmin"]) && Session["OrgID"] == null)  // need to choose org
        {
            Response.Redirect("~/Account/SelectOrgV2.aspx" + (Request.QueryString["from_url"] == null ? "" : "?" + Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url="))), false);
            return;
        }
        else
        {
            if (Request.QueryString["from_url"] != null)
            {
                Response.Redirect(Server.UrlDecode(Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url=") + 9)), false);
                return;
            }
            else
            {
                Response.Redirect(Convert.ToInt32(Session["StaffID"]) >= 0 ? "~/Default.aspx" : "~/StaffLoginsV2.aspx", false);
                return;
            }
        }
    }

}