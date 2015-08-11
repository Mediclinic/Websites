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

public partial class OrganisationStatsV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, true, false, false, true);
                SetupGUI();
                FillForm();
            }

        }
        catch (CustomMessageException ex)
        {
            if (IsPostBack) SetErrorMessage(ex.Message);
            else HideTableAndSetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            if (IsPostBack) SetErrorMessage("", ex.ToString());
            else HideTableAndSetErrorMessage("", ex.ToString());
        }
    }

    protected void SetupGUI()
    {
        DateTime startOfThisMonth = DateTime.Now.AddDays((-1*DateTime.Now.Day) + 1);
        DateTime endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1);

        txtStartDate.Text = startOfThisMonth.ToString("dd-MM-yyyy");
        txtEndDate.Text   = endOfThisMonth.ToString("dd-MM-yyyy");

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";

        lblHeading.InnerText = UserView.GetInstance().IsAgedCareView ? "Facilities Statistics" : "Clinics Statistics";
    }

    #endregion

    #region IsValidFormID(), GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$") && OrganisationDB.Exists(Convert.ToInt32(id));
    }
    private int GetFormID(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormID())
            throw new Exception("Invalid url id");
        return Convert.ToInt32(Request.QueryString["id"]);
    }

    #endregion

    #region FillForm

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        FillForm();
    }

    protected void FillForm()
    {

        try
        {
            if (!CheckIsValidStartEndDates())
                return;

            DataTable tblStats = OrganisationDB.GetStats(GetFromDate(), GetToDate(), UserView.GetInstance().IsClinicView ? 5 : 6, chkIncDeleted.Checked);
            lstOrgStats.DataSource = tblStats;
            lstOrgStats.DataBind();

            // get from footer

            Label lblSum_TotalBookings  = (Label)lstOrgStats.Controls[lstOrgStats.Controls.Count - 1].Controls[0].FindControl("lblSum_TotalBookings");
            Label lblSum_AvgConsultTime = (Label)lstOrgStats.Controls[lstOrgStats.Controls.Count - 1].Controls[0].FindControl("lblSum_AvgConsultTime");
            Label lblSum_Bookings       = (Label)lstOrgStats.Controls[lstOrgStats.Controls.Count - 1].Controls[0].FindControl("lblSum_Bookings");
            Label lblSum_Patients       = (Label)lstOrgStats.Controls[lstOrgStats.Controls.Count - 1].Controls[0].FindControl("lblSum_Patients");

            lblSum_TotalBookings.Text = String.Format("{0:n0}", tblStats.Compute("Sum(total_bookings)", ""));

            decimal total = 0;
            int     count = 0;
            for (int i = 0; i < tblStats.Rows.Count; i++)
            {
                if (tblStats.Rows[i]["avg_minutes"] == DBNull.Value)
                    continue;
                total += Convert.ToDecimal(tblStats.Rows[i]["avg_minutes"]);
                count++;
            }
            if (count > 0)
                lblSum_AvgConsultTime.Text = String.Format("{0:n0}", total / count);
            
            lblSum_Bookings.Text = String.Format("{0:n0}", tblStats.Compute("Sum(n_bookings)", ""));
            lblSum_Patients.Text = String.Format("{0:n0}", tblStats.Compute("Sum(n_patients)", ""));
        }
        catch (CustomMessageException cmEx)
        {
            HideTableAndSetErrorMessage(cmEx.Message);
        }
        catch (Exception ex)
        {
            HideTableAndSetErrorMessage(Utilities.IsDev() ? ex.ToString() : string.Empty);
        }

    }

    #endregion

    protected bool CheckIsValidStartEndDates()
    {
        try
        {
            if (txtStartDate.Text.Length > 0 && !Utilities.IsValidDate(txtStartDate.Text, "dd-mm-yyyy"))
                throw new CustomMessageException("Invalid start date");
            if (txtEndDate.Text.Length > 0 && !Utilities.IsValidDate(txtEndDate.Text, "dd-mm-yyyy"))
                throw new CustomMessageException("Invalid end date");

            return true;
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return false;
        }
    }
    protected DateTime GetFromDate()
    {
        return txtStartDate.Text.Length > 0 ? Utilities.GetDate(txtStartDate.Text, "dd-mm-yyyy") : DateTime.MinValue;
    }
    protected DateTime GetToDate()
    {
        return txtEndDate.Text.Length > 0 ? Utilities.GetDate(txtEndDate.Text, "dd-mm-yyyy").Add(new TimeSpan(23, 59, 59)) : DateTime.MinValue;
    }


    protected DataTable GetPatientDataTable(int organisation_id)
    {
        Organisation org = OrganisationDB.GetByID(organisation_id);

        Hashtable staffHashOriginal = StaffDB.GetAllInHashtable(true, true, true, false);
        Hashtable staffHash = new Hashtable();
        foreach(Staff s in staffHashOriginal.Values)
            staffHash[s.Person.PersonID] = s;

        DataTable tbl = RegisterPatientDB.GetPatientsAddedByOrg(organisation_id, GetFromDate(), GetToDate());

        tbl.Columns.Add("organisation_name");
        tbl.Columns.Add("organisation_id");
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            tbl.Rows[i]["organisation_name"] = org.Name;
            tbl.Rows[i]["organisation_id"] = org.OrganisationID;
        }

        // sort by most common referrer
        tbl.Columns.Add("referrer_count", typeof(int));
        tbl.Columns.Add("added_by_count", typeof(int));
        tbl.Columns.Add("added_by"      , typeof(String));
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int refCount = 0;
            if (tbl.Rows[i]["referrer_info_referrer_id"] != DBNull.Value)
            {
                for (int j = 0; j < tbl.Rows.Count; j++)
                    if (tbl.Rows[j]["referrer_info_referrer_id"] != DBNull.Value && Convert.ToInt32(tbl.Rows[j]["referrer_info_referrer_id"]) == Convert.ToInt32(tbl.Rows[i]["referrer_info_referrer_id"]))
                        refCount++;
            }

            tbl.Rows[i]["referrer_count"] = refCount;

            int addedByCount = 0;
            if (tbl.Rows[i]["patient_person_added_by"] != DBNull.Value)
            {
                for (int j = 0; j < tbl.Rows.Count; j++)
                    if (tbl.Rows[j]["patient_person_added_by"] != DBNull.Value && Convert.ToInt32(tbl.Rows[j]["patient_person_added_by"]) == Convert.ToInt32(tbl.Rows[i]["patient_person_added_by"]))
                        addedByCount++;
            }

            tbl.Rows[i]["added_by_count"] = addedByCount;


            if (tbl.Rows[i]["patient_person_added_by"] != DBNull.Value)
            {
                int staffID = Convert.ToInt32(tbl.Rows[i]["patient_person_added_by"]);
                Staff s1 = (Staff)staffHash[staffID];
                string s2 = s1.Person.FullnameWithoutMiddlename;
            }


            tbl.Rows[i]["added_by"] = tbl.Rows[i]["patient_person_added_by"] == DBNull.Value ? (object)DBNull.Value : ((Staff)staffHash[Convert.ToInt32(tbl.Rows[i]["patient_person_added_by"])]).Person.FullnameWithoutMiddlename;

        }
        tbl.DefaultView.Sort = "referrer_count DESC, referrer_info_surname, referrer_info_firstname, patient_person_surname, patient_person_firstname, patient_person_middlename";
        tbl = tbl.DefaultView.ToTable();

        return tbl;
    }


    protected void lnkPatients_Command(object sender, CommandEventArgs e)
    {
        int organisation_id = Convert.ToInt32(e.CommandArgument);

        DataTable tbl = GetPatientDataTable(organisation_id);
        lstPatients.DataSource = tbl;
        lstPatients.DataBind();

        btnExport.Visible = tbl.Rows.Count > 0;
        btnExport.CommandArgument = organisation_id.ToString();
    }

    
    protected void btnExport_Command(object sender, CommandEventArgs e)
    {
        int organisation_id = Convert.ToInt32(e.CommandArgument);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        DataTable dt = GetPatientDataTable(organisation_id);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append(dt.Rows[i]["referrer_info_firstname"].ToString() + " " + dt.Rows[i]["referrer_info_surname"].ToString()).Append(",");
            sb.Append(dt.Rows[i]["patient_person_firstname"].ToString() + " " + dt.Rows[i]["patient_person_surname"].ToString()).Append(",");
            sb.Append(dt.Rows[i]["organisation_name"].ToString());
            sb.AppendLine();
        }

        ExportCSV(Response, sb.ToString(), "new_patients_export.csv");
    }
    protected static void ExportCSV(HttpResponse response, string fileText, string fileName)
    {
        byte[] buffer = GetBytes(fileText);

        try
        {
            response.Clear();
            response.ContentType = "text/plain";
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            response.End();
        }
        catch (System.Web.HttpException ex) 
        {
            // ignore exception where user closed the download box
            if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                throw;
        }
    }
    protected static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }


    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        SetErrorMessage(errMsg, details);
    }
    private void SetErrorMessage(string errMsg = "", string details = "")
    {
        if (errMsg.Contains(Environment.NewLine))
            errMsg = errMsg.Replace(Environment.NewLine, "<br />");

        // double escape so shows up literally on webpage for 'alert' message
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"#\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

        lblErrorMessage.Visible = true;
        if (errMsg != null && errMsg.Length > 0)
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}