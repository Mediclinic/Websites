using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;

public partial class AjaxLiveReferrerOrgNameSearch : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string result = GetMatches();
            Response.Write(result);
        }
        catch (SessionTimedOutException)
        {
            Utilities.UnsetSessionVariables();
            Response.Write("SessionTimedOutException");
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "Error - please contact system administrator."));
        }
    }

    #endregion


    protected string GetMatches()
    {
        string surnameMatch = Request.QueryString["q"];
        if (surnameMatch == null || surnameMatch.Length == 0)
            throw new CustomMessageException();

        surnameMatch = surnameMatch.Replace("'", "''");

        surnameMatch = surnameMatch.Replace("[", "").Replace("]", "").Trim();


        int maxResults = 80;
        if (Request.QueryString["max_results"] != null)
        {
            if (!Regex.IsMatch(Request.QueryString["max_results"], @"^\d+$"))
                throw new CustomMessageException();
            maxResults = Convert.ToInt32(Request.QueryString["max_results"]);
        }

        string link_href = Request.QueryString["link_href"];
        if (link_href == null)
            throw new CustomMessageException();
        link_href = System.Web.HttpUtility.UrlDecode(link_href);

        string link_onclick = Request.QueryString["link_onclick"];
        if (link_onclick == null)
            throw new CustomMessageException();
        link_onclick = System.Web.HttpUtility.UrlDecode(link_onclick);


        DataTable dt = Session["referrerorglistpopup_data"] as DataTable;
        DataRow[] foundRows = dt.Select("name LIKE '" + surnameMatch + "*'", "name");
        if (foundRows.Length > maxResults)
            return "Too many results (" + foundRows.Length + ")";

        string result      = string.Empty;
        string tableResult = string.Empty;

        foreach (DataRow row in foundRows)
        {
            string href = link_href.Replace("[organisation_id]", row["organisation_id"].ToString()).Replace("[name]", row["name"].ToString());
            string onclick = link_onclick.Replace("[organisation_id]", row["organisation_id"].ToString()).Replace("[name]", row["name"].ToString().Replace("'", "\\'"));

            string link = "<a " + (onclick.Length == 0 ? "" : " onclick=\"" + onclick + "\" ") + " href='" + href + @"' >" + row["name"] + ", " + row["name"] + @"</a>";
            string hoverDiv = ""; // @"<div id=""register_referrer_detail_" + row["organisation_id"].ToString() + @""" style=""display: none;"" class=""FAQ"">IMAGE OR TEXT HERE<br> </div>";

            result      += (result.Length == 0 ? "" : "<br />") + link;
            tableResult += "<tr><td>" + link + "</td><td style=\"width:8px\"></td><td>" + hoverDiv + "</td></tr>";
        }


        bool useTableResult = true;
        if (useTableResult)
            return "<table>" + tableResult + "</table>";
        else
            return result;
    }

}