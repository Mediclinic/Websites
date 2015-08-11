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

public partial class SelectOrgV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            main_content.Style["background"] = (Session["SystemVariables"] == null) ? "url(../imagesV2/login_bg.png) center top no-repeat #EDEDED" : "url(../imagesV2/" + ((SystemVariables)Session["SystemVariables"])["MainLogoBackground"].Value + ") center top no-repeat #EDEDED";

        bool showPageHeader = Request.QueryString["show_header"] == null || Request.QueryString["show_header"] == "1";
        if (!showPageHeader)
            Utilities.UpdatePageHeader(Page.Master, true, true);

        int type_group_id = -1;
        if (Session["SiteTypeID"] != null && (Convert.ToInt32(Session["SiteTypeID"]) == 1 || Convert.ToInt32(Session["SiteTypeID"]) == 3))
        {
            lblHeading.InnerHtml = "Select A Clinic";
            type_group_id = 5;
        }
        else if (Session["SiteTypeID"] != null && Convert.ToInt32(Session["SiteTypeID"]) == 2)
        {
            lblHeading.InnerHtml = "Select A Facility/Wing";
            type_group_id = 6;
        }
        else
            throw new Exception("Unknown SiteTypeID in session");


        if (Session["PatientID"] != null)
        {
            DataTable dt = RegisterPatientDB.GetDataTable_OrganisationsOf(Convert.ToInt32(Session["PatientID"]), true, false, true, true, true);

            if (dt.Rows.Count == 1)  // if only org, auto select it
            {
                Session["IsMultipleOrgs"] = false;
                Select(Convert.ToInt32(dt.Rows[0]["organisation_id"]));
            }
            else
            {
                Session["IsMultipleOrgs"] = true;
                lstOrgs.DataSource = dt;
                lstOrgs.DataBind();
            }
        }
        else // if (Session["StaffID"] != null)
        {
            DataTable dt = RegisterStaffDB.GetDataTable_OrganisationsOf(Convert.ToInt32(Session["StaffID"]), type_group_id.ToString());

            if (dt.Rows.Count == 1)  // if only org, auto select it
            {
                Session["IsMultipleOrgs"] = false;
                Select(Convert.ToInt32(dt.Rows[0]["organisation_id"]));
            }
            else
            {
                Session["IsMultipleOrgs"] = true;
                lstOrgs.DataSource = dt;
                lstOrgs.DataBind();
            }
        }
    }
    protected void lstOrgs_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (lstOrgs.Items.Count < 1)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                Label lblFooter = (Label)e.Item.FindControl("lblEmptyData");
                lblFooter.Visible = true;
            }
        }
    }



    protected void lnkSelect_Click(object sender, CommandEventArgs e)
    {
        Select(Convert.ToInt32(e.CommandArgument));
    }
    protected void Select(int orgID)
    {
        Organisation org   = OrganisationDB.GetByID(orgID);
        Session["OrgID"]   = org.OrganisationID.ToString();
        Session["OrgName"] = org.Name;

        if (Request.QueryString["from_url"] != null)
        {
            Response.Redirect(Server.UrlDecode(Request.RawUrl.Substring(Request.RawUrl.IndexOf("from_url=") + 9)));
        }
        else if (Session["PatientID"] != null)
        {
            Response.Redirect("~/BookingNextAvailableV2.aspx");
        }
        else
        {
            //Response.Redirect("~/Default.aspx");

            string ndays = Session["PatientId"] == null ? "1" : "3"; // provider login shows just today, patient iew 3 days
            bool isExternalView = Session["IsExternal"] != null && Convert.ToBoolean(Session["IsExternal"]);
            if (isExternalView)
                ndays = (int)Session["StaffID"] != -5 ? "3" : "4";  // external login shows 3 days (less days looks better), but 4 days for call center (for more functionality)

            Response.Redirect("~/BookingsV2.aspx?orgs=" + Session["OrgID"] + "&ndays=" + ndays);
        }
    }

}