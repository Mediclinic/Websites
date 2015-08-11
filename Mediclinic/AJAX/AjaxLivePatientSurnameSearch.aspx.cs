using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;

public partial class AjaxLivePatientSurnameSearch : System.Web.UI.Page
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


        DataTable dt = Session["patientinfo_data"] as DataTable;

        //dt = null;

        // if from patient list page, just use the data from the session, else retrieve it serperately
        if (dt == null)
        {
            UserView userView                     = UserView.GetInstance();
            bool     ProvsCanSeePatientsOfAllOrgs = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Bookings_ProvsCanSeePatientsOfAllOrgs"].Value == "1";
            bool     canSeeAllPatients            = userView.IsAdminView || (userView.IsProviderView && ProvsCanSeePatientsOfAllOrgs);

            if (canSeeAllPatients  && userView.IsClinicView)
                dt = PatientDB.GetDataTable(false, false, userView.IsClinicView);
            else if (canSeeAllPatients && !userView.IsClinicView)
                dt = RegisterPatientDB.GetDataTable_PatientsOfOrgGroupType(false, "6", false, false, userView.IsClinicView);
            else // no admin view - so org is set
                dt = RegisterPatientDB.GetDataTable_PatientsOf(false, Convert.ToInt32(Session["OrgID"]), false, false, userView.IsClinicView);

            // update AjaxLivePatientSurnameSearch and PatientInfo.aspx and PatientListPopup to disallow providers to see other patients.
            if (userView.IsProviderView && !canSeeAllPatients)  // remove any patients who they haven't had bookings with before
            {
                Patient[] patients = BookingDB.GetPatientsOfBookingsWithProviderAtOrg(Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["OrgID"]));
                System.Collections.Hashtable hash = new System.Collections.Hashtable();
                foreach (Patient p in patients)
                    hash[p.PatientID] = 1;

                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    if (hash[Convert.ToInt32(dt.Rows[i]["patient_id"])] == null)
                        dt.Rows.RemoveAt(i);
            }

            Session["patientinfo_data"] = dt;
        }


        DataRow[] foundRows = containsFirstname ? dt.Select("surname='" + surnameMatch + "' AND firstname LIKE '" + firstnameMatch + "*'", "surname, firstname, middlename") : dt.Select("surname LIKE '" + surnameMatch + "*'", "surname, firstname, middlename");
        if (foundRows.Length > maxResults)
            return "Too many results (" + foundRows.Length + ")";
        if (foundRows.Length == 0)
            return "No results matching that text";

        string result      = string.Empty;
        string tableResult = string.Empty;

        foreach (DataRow row in foundRows)
        {
            string dob    = row["dob"] == DBNull.Value ? "" : " [DOB: "+Convert.ToDateTime(row["dob"]).ToString("dd-MM-yyyy")+"]";

            string href    = link_href.Replace("[patient_id]", row["patient_id"].ToString()).Replace("[firstname]", row["firstname"].ToString()).Replace("[surname]", row["surname"].ToString());
            string onclick = link_onclick.Replace("[patient_id]", row["patient_id"].ToString()).Replace("[firstname]", row["firstname"].ToString()).Replace("[surname]", row["surname"].ToString().Replace("'", "\\'"));

            string link = "<a " + (onclick.Length == 0 ? "" : " onclick=\"" + onclick + "\" ") + " href='" + href + @"' onmouseover=""show_patient_detail('patient_detail_" + row["patient_id"].ToString() + "', " + row["patient_id"].ToString() + @"); return true;"" onmouseout=""hide_patient_detail('patient_detail_" + row["patient_id"].ToString() + @"'); return true;"" >" + row["surname"] + ", " + row["firstname"] + (row["middlename"].ToString().Length == 0 ? "" : " " + row["middlename"]) + @"</a>";
            string hoverDiv = @"<div id=""patient_detail_" + row["patient_id"].ToString() + @""" style=""display: none;"" class=""FAQ"">IMAGE OR TEXT HERE<br> </div>";

            result      += (result.Length == 0 ? "" : "<br />") + link + dob;
            tableResult += "<tr><td>" + link + "</td><td>" + hoverDiv + dob + "</td></tr>";
        }


        bool useTableResult = true;
        if (useTableResult)
            return "<table>" + tableResult + "</table>";
        else
            return result;
    }

}