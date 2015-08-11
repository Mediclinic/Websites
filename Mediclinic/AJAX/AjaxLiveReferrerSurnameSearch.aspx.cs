using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;

public partial class AjaxLiveReferrerSurnameSearch : System.Web.UI.Page
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

        if (surnameMatch.EndsWith(", "))
            surnameMatch = surnameMatch.Substring(0, surnameMatch.Length - 2).Trim();
        if (surnameMatch.EndsWith(","))
            surnameMatch = surnameMatch.Substring(0, surnameMatch.Length - 1).Trim();

        bool containsFirstname = false;
        string firstnameMatch = null;
        if (surnameMatch.Contains(", "))
        {
            containsFirstname = true;
            int index = surnameMatch.IndexOf(", ");
            firstnameMatch = surnameMatch.Substring(index + 2);
            surnameMatch   = surnameMatch.Substring(0, index);
        }


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


        DataTable dt = Session["referrerlistpopup_data"] as DataTable;
        DataRow[] foundRows = containsFirstname ? dt.Select("surname='" + surnameMatch + "' AND firstname LIKE '" + firstnameMatch + "*'", "surname, firstname, middlename") : dt.Select("surname LIKE '" + surnameMatch + "*'", "surname, firstname, middlename");
        if (foundRows.Length > maxResults)
            return "Too many results (" + foundRows.Length + ")";

        string result      = string.Empty;
        string tableResult = string.Empty;

        foreach (DataRow row in foundRows)
        {
            string provider_number = row["provider_number"] == DBNull.Value || row["provider_number"].ToString().Length == 0 ? "" : " <small>Prov Nbr:</small> <b>" + row["provider_number"] + "</b>";
            //string provider_number = row["provider_number"] == DBNull.Value ? "" : " [Prov Nbr: " + row["provider_number"] + "]";

            string href    = link_href.Replace("[register_referrer_id]", row["register_referrer_id"].ToString()).Replace("[firstname]", row["firstname"].ToString()).Replace("[surname]", row["surname"].ToString());
            string onclick = link_onclick.Replace("[register_referrer_id]", row["register_referrer_id"].ToString()).Replace("[firstname]", row["firstname"].ToString()).Replace("[surname]", row["surname"].ToString().Replace("'","\\'"));

            string link = "<a " + (onclick.Length == 0 ? "" : " onclick=\"" + onclick + "\" ") + " href='" + href + @"' onmouseover=""show_register_referrer_detail('register_referrer_detail_" + row["register_referrer_id"].ToString() + "', " + row["register_referrer_id"].ToString() + @"); return true;"" onmouseout=""hide_register_referrer_detail('register_referrer_detail_" + row["register_referrer_id"].ToString() + @"'); return true;"" >" + row["surname"] + ", " + row["firstname"] + (row["middlename"].ToString().Length == 0 ? "" : " " + row["middlename"]) + @"</a>";
            string hoverDiv = @"<div id=""register_referrer_detail_" + row["register_referrer_id"].ToString() + @""" style=""display: none;"" class=""FAQ"">IMAGE OR TEXT HERE<br> </div>";

            result      += (result.Length == 0 ? "" : "<br />") + link + provider_number;
            tableResult += "<tr><td>" + link + "</td><td style=\"width:8px\"></td><td>" + hoverDiv + provider_number + "</td></tr>";
        }


        bool useTableResult = true;
        if (useTableResult)
            return "<table>" + tableResult + "</table>";
        else
            return result;
    }

}