using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;

public partial class AjaxLiveSuburbSearch : System.Web.UI.Page
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
        string suburbNameMatch = Request.QueryString["q"];
        if (suburbNameMatch == null || suburbNameMatch.Length == 0)
            throw new CustomMessageException();

        string stateMatch = Request.QueryString["state"];

        //suburbNameMatch = suburbNameMatch.Replace("[", "").Replace("]", "").Trim();


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


        DataTable results = SuburbDB.GetDataTable(true, suburbNameMatch, true, null, stateMatch);
        if (results.Rows.Count > maxResults)
            return "Too many results (" + results.Rows.Count + ")";

        string result      = string.Empty;
        string tableResult = string.Empty;

        foreach (DataRow row in results.Rows)
        {
            string href    = link_href.Replace("[suburb_id]",    row["suburb_id"].ToString()).Replace("[state]", row["state"].ToString()).Replace("[postcode]", row["postcode"].ToString()).Replace("[name]", row["name"].ToString());
            string onclick = link_onclick.Replace("[suburb_id]", row["suburb_id"].ToString()).Replace("[state]", row["state"].ToString()).Replace("[postcode]", row["postcode"].ToString()).Replace("[name]", row["name"].ToString().Replace("'", "\\'"));

            //string link = "<a " + (onclick.Length == 0 ? "" : " onclick=\"" + onclick + "\" ") + " href='" + href + @"' onmouseover=""show_staff_detail('suburb_detail_" + row["suburb_id"].ToString() + "', " + row["suburb_id"].ToString() + @"); return true;"" onmouseout=""hide_suburb_detail('suburb_detail_" + row["suburb_id"].ToString() + @"'); return true;"" >" + row["name"] + ", " + row["postcode"] + ", " + row["state"] + @"</a>";
            string link = "<a " + (onclick.Length == 0 ? "" : " onclick=\"" + onclick + "\" ") + " href='" + href + @"'>" + row["name"] + ", " + row["postcode"] + ", " + row["state"] + @"</a>";
            string hoverDiv = @"<div id=""suburb_detail_" + row["suburb_id"].ToString() + @""" style=""display: none;"" class=""FAQ"">IMAGE OR TEXT HERE<br> </div>";

            result      += (result.Length == 0 ? "" : "<br />") + link;
            tableResult += "<tr><td>" + link + "</td><td>" + hoverDiv + "</td></tr>";
        }


        bool useTableResult = true;
        if (useTableResult)
            return "<table>" + tableResult + "</table>";
        else
            return result;
    }

}