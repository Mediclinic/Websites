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

public partial class RegisterPatientsToOrganisationV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                Session.Remove("registerpatienttoorg_sortexpression");
                Session.Remove("registerpatienttoorg_data");
                FillGrid();
            }

            this.GrdRegistration.EnableViewState = true;

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

    private bool IsValidFormID()
    {
        string raw_id = Request.QueryString["id"];
        if (raw_id == null)
            return false;

        return Regex.IsMatch(raw_id, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new Exception("Invalid ID");
        return Convert.ToInt32(Request.QueryString["id"]);
    }

    private bool GetFormViewOnlyLast()
    {
        string view_only_current = Request.QueryString["view_only_current"];
        if (view_only_current == null)
            return false;

        return view_only_current == "1";
    }

    #region GrdRegistration

    private bool hideFotter = false;

    protected void FillGrid()
    {
        if (!IsValidFormID())
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        Organisation org = OrganisationDB.GetByID(GetFormID());
        if (org == null)
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        lblHeading.Text = Page.Title = "Manage Registrations For :  " + org.Name;
        this.lnkThisOrg.NavigateUrl = "~/OrganisationDetailV2.aspx?type=view&id=" + GetFormID().ToString();
        this.lnkThisOrg.Text = "Back to details for " + org.Name;



        string searchSurname = "";
        if (Request.QueryString["surname_search"] != null && Request.QueryString["surname_search"].Length > 0)
        {
            searchSurname = Request.QueryString["surname_search"];
            txtSearchSurname.Text = Request.QueryString["surname_search"];
        }
        bool searchSurnameOnlyStartsWith = true;
        if (Request.QueryString["surname_starts_with"] != null && Request.QueryString["surname_starts_with"].Length > 0)
        {
            searchSurnameOnlyStartsWith = Request.QueryString["surname_starts_with"] == "0" ? false : true;
            chkSurnameSearchOnlyStartWith.Checked = searchSurnameOnlyStartsWith;
        }
        else
        {
            chkSurnameSearchOnlyStartWith.Checked = searchSurnameOnlyStartsWith;
        }


        DataTable dt = RegisterPatientDB.GetDataTable_PatientsOf(GetFormViewOnlyLast(), org.OrganisationID, false, false, false, false, searchSurname, searchSurnameOnlyStartsWith);


        int[] ptIDs = new int[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
            ptIDs[i] = Convert.ToInt32(dt.Rows[i]["patient_id"]);


        // get last and next booking dates

        Hashtable lastBookingDates = BookingDB.GetLastBookingDates(ptIDs, org.OrganisationID);
        Hashtable nextBookingDates = BookingDB.GetNextBookingDates(ptIDs, org.OrganisationID);
        dt.Columns.Add("last_booking_date", typeof(DateTime));
        dt.Columns.Add("next_booking_date", typeof(DateTime));
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            dt.Rows[i]["last_booking_date"] = lastBookingDates[Convert.ToInt32(dt.Rows[i]["patient_id"])] == null ? (object)DBNull.Value : (DateTime)lastBookingDates[Convert.ToInt32(dt.Rows[i]["patient_id"])];
            dt.Rows[i]["next_booking_date"] = nextBookingDates[Convert.ToInt32(dt.Rows[i]["patient_id"])] == null ? (object)DBNull.Value : (DateTime)nextBookingDates[Convert.ToInt32(dt.Rows[i]["patient_id"])];
        }
        


        // get epc info

        Hashtable mostRecentRecallHashByPatientID = LetterPrintHistoryDB.GetMostRecentRecallHashByPatients(ptIDs);
        
        Hashtable patientHealthCardCache        = PatientsHealthCardsCacheDB.GetBullkActive(ptIDs);
        Hashtable epcRemainingCache             = GetEPCRemainingCache(patientHealthCardCache);
        Hashtable patientsMedicareCountCache    = PatientsMedicareCardCountThisYearCacheDB.GetBullk(ptIDs, DateTime.Today.Year);
        Hashtable patientsEPCRemainingCache     = PatientsEPCRemainingCacheDB.GetBullk(ptIDs, DateTime.Today.AddYears(-1));
        int       MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);


        dt.Columns.Add("epc_expire_date"     , typeof(DateTime));
        dt.Columns.Add("has_valid_epc"       , typeof(Boolean));
        dt.Columns.Add("epc_count_remaining" , typeof(Int32));

        dt.Columns.Add("most_recent_recall_sent", typeof(DateTime));
        for (int i = dt.Rows.Count - 1; i >= 0; i--)
        {
            int patientID = Convert.ToInt32(dt.Rows[i]["patient_id"]);

            HealthCard               hc                       = GetHealthCardFromCache(patientHealthCardCache, patientID);
            bool                     hasEPC                   = hc != null && hc.DateReferralSigned != DateTime.MinValue;
            HealthCardEPCRemaining[] epcsRemaining            = !hasEPC ? new HealthCardEPCRemaining[] { } : GetEPCRemainingFromCache(epcRemainingCache, hc);
            int                      totalServicesAllowedLeft = !hasEPC ? 0 : (MedicareMaxNbrServicesPerYear - (int)patientsMedicareCountCache[patientID]);

            int totalEpcsRemaining = 0;
            for (int j = 0; j < epcsRemaining.Length; j++)
                totalEpcsRemaining += epcsRemaining[j].NumServicesRemaining;

            DateTime referralSignedDate = !hasEPC ? DateTime.MinValue : hc.DateReferralSigned.Date;
            DateTime hcExpiredDate      = !hasEPC ? DateTime.MinValue : referralSignedDate.AddYears(1);
            bool     isExpired          = !hasEPC ? true              : hcExpiredDate <= DateTime.Today;

            int nServicesLeft = 0;
            if (hc != null && DateTime.Today >= referralSignedDate.Date && DateTime.Today < hcExpiredDate.Date)
                nServicesLeft = totalEpcsRemaining;
            if (hc != null && totalServicesAllowedLeft < nServicesLeft)
                nServicesLeft = totalServicesAllowedLeft;

            bool has_valid_epc = hasEPC && !isExpired && (hc.Organisation.OrganisationID == -2 || (hc.Organisation.OrganisationID == -1 && nServicesLeft > 0));
            int epc_count_remaining = hasEPC && hc.Organisation.OrganisationID == -1 ? nServicesLeft : -1;

            dt.Rows[i]["has_valid_epc"]       = has_valid_epc;
            dt.Rows[i]["epc_expire_date"]     = hasEPC ? hcExpiredDate : (object)DBNull.Value;
            dt.Rows[i]["epc_count_remaining"] = epc_count_remaining != -1 ? epc_count_remaining : (object)DBNull.Value;

            dt.Rows[i]["most_recent_recall_sent"] = mostRecentRecallHashByPatientID[patientID] == null ? (object)DBNull.Value : ((LetterPrintHistory)mostRecentRecallHashByPatientID[patientID]).Date;
        }



        Session["registerpatienttoorg_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["registerpatienttoorg_sortexpression"] != null && Session["registerpatienttoorg_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["registerpatienttoorg_sortexpression"].ToString();
                GrdRegistration.DataSource = dataView;
            }
            else
            {
                GrdRegistration.DataSource = dt;
            } 
            
            
            try
            {
                GrdRegistration.DataBind();
                GrdRegistration.PagerSettings.FirstPageText = "1";
                GrdRegistration.PagerSettings.LastPageText = GrdRegistration.PageCount.ToString();
                GrdRegistration.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdRegistration.DataSource = dt;
            GrdRegistration.DataBind();

            int TotalColumns = GrdRegistration.Rows[0].Cells.Count;
            GrdRegistration.Rows[0].Cells.Clear();
            GrdRegistration.Rows[0].Cells.Add(new TableCell());
            GrdRegistration.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdRegistration.Rows[0].Cells[0].Text = "No Record Found";
        }

        if (hideFotter)
            GrdRegistration.FooterRow.Visible = false;
    }
    protected void GrdRegistration_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdRegistration_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Organisation org = OrganisationDB.GetByID(GetFormID());
        if (org == null)
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        DataTable dt = Session["registerpatienttoorg_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("register_patient_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlPatient = (DropDownList)e.Row.FindControl("ddlPatient");
            if (ddlPatient != null)
            {
                Patient[] incList_orig = RegisterPatientDB.GetPatientsOf(GetFormViewOnlyLast(), org.OrganisationID);
                Patient[] incList = Patient.RemoveByID(incList_orig, Convert.ToInt32(thisRow["patient_id"]));
                DataTable patient = PatientDB.GetDataTable_AllNotInc(incList);
                patient.DefaultView.Sort = "surname ASC";
                foreach (DataRowView row in patient.DefaultView)
                    ddlPatient.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString() + " " + row["middlename"].ToString(), row["patient_id"].ToString()));
                ddlPatient.SelectedValue = thisRow["patient_id"].ToString();
            }

            Label lblEPCExpiry     = (Label)e.Row.FindControl("lblEPCExpiry");
            Label lblEPCsRemaining = (Label)e.Row.FindControl("lblEPCsRemaining");
            if (lblEPCExpiry != null && lblEPCsRemaining != null)
            {
                if (!Convert.ToBoolean(thisRow["has_valid_epc"]))
                {
                    lblEPCExpiry.ForeColor     = System.Drawing.Color.Red;
                    lblEPCsRemaining.ForeColor = System.Drawing.Color.Red;
                }
            }

            HyperLink lnkBookings = (HyperLink)e.Row.FindControl("lnkBookings");
            if (lnkBookings != null)
            {
                lnkBookings.NavigateUrl = string.Format("~/BookingsV2.aspx?orgs={0}&patient={1}", org.OrganisationID, Convert.ToInt32(thisRow["patient_id"]));
            }

            Label lnkPatient = (Label)e.Row.FindControl("lnkPatient");
            if (lnkPatient != null)
            {
                string URL = "PatientDetailV2.aspx?type=view&id=" + Convert.ToInt32(thisRow["patient_id"]);
                if (URL.StartsWith("~")) URL = URL.Substring(1);
                lnkPatient.Text = "<a href=\"#\" onclick=\"var win=window.open('" + URL + "', '_blank'); win.focus();return false;\" >" + thisRow["firstname"] + " " + thisRow["surname"] + "</a>";
            }


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer && GrdRegistration.ShowFooter)
        {
            DropDownList ddlPatient = (DropDownList)e.Row.FindControl("ddlNewPatient");
            if (ddlPatient != null)
            {
                Patient[] incList = RegisterPatientDB.GetPatientsOf(GetFormViewOnlyLast(), org.OrganisationID);
                DataTable patient = PatientDB.GetDataTable_AllNotInc(incList);
                patient.DefaultView.Sort = "surname ASC";
                foreach (DataRowView row in patient.DefaultView)
                    ddlPatient.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString() + " " + row["middlename"].ToString(), row["patient_id"].ToString()));

                if (patient.Rows.Count == 0)
                    hideFotter = true;

            }
        }
    }
    protected void GrdRegistration_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdRegistration.EditIndex = -1;
        FillGrid();
    }
    protected void GrdRegistration_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlPatient = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlPatient");

        Organisation org = OrganisationDB.GetByID(GetFormID());
        if (org == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        RegisterPatientDB.Update(Convert.ToInt32(lblId.Text), org.OrganisationID, Convert.ToInt32(ddlPatient.SelectedValue));

        GrdRegistration.EditIndex = -1;
        FillGrid();
    }
    protected void GrdRegistration_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");

        RegisterPatient registerPatient = RegisterPatientDB.GetByID(Convert.ToInt32(lblId.Text));
        if (BookingDB.GetCountByPatientAndOrg(registerPatient.Patient.PatientID, registerPatient.Organisation.OrganisationID) > 0)
        {
            SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' because there exists a booking for this patient there.");
            return;
        }

        int agedCareOrgRegistrations = RegisterPatientDB.GetCountByPatientAndOrgTypeGroup(registerPatient.Patient.PatientID, "6");
        if (agedCareOrgRegistrations < 2)
        {
            SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' until they have been added to another Fac/Wing/Unit.");
            return;
        }


        try
        {
            RegisterPatientDB.UpdateInactive(Convert.ToInt32(lblId.Text), false);
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                HideTableAndSetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                HideTableAndSetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrid();
    }
    protected void GrdRegistration_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlPatient = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewPatient");

            Organisation org = OrganisationDB.GetByID(GetFormID());
            if (org == null)
            {
                HideTableAndSetErrorMessage("");
                return;
            }

            try
            {
                RegisterPatientDB.Insert(org.OrganisationID, Convert.ToInt32(ddlPatient.SelectedValue));
            }
            catch (UniqueConstraintException) 
            {
                // happens when 2 forms allow adding - do nothing and let form re-update
            }
            FillGrid();
        }
    }
    protected void GrdRegistration_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdRegistration.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdRegistration.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }
    protected void GrdRegistration_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdRegistration.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["registerpatienttoorg_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["registerpatienttoorg_sortexpression"] == null)
                Session["registerpatienttoorg_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["registerpatienttoorg_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["registerpatienttoorg_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdRegistration.DataSource = dataView;
            GrdRegistration.DataBind();
        }
    }

    #endregion

    #region GetEPCRemainingCache, GetHealthCardFromCache, GetEPCRemainingFromCache

    protected Hashtable GetEPCRemainingCache(Hashtable patientHealthCardCache)
    {
        ArrayList healthCardIDs = new ArrayList();
        foreach (PatientActiveHealthCards ptHCs in patientHealthCardCache.Values)
        {
            if (ptHCs.MedicareCard != null)
                healthCardIDs.Add(ptHCs.MedicareCard.HealthCardID);
            if (ptHCs.DVACard != null)
                healthCardIDs.Add(ptHCs.DVACard.HealthCardID);
        }

        return HealthCardEPCRemainingDB.GetHashtableByHealthCardIDs((int[])healthCardIDs.ToArray(typeof(int)));
    }

    protected HealthCard GetHealthCardFromCache(Hashtable patientHealthCardCache, int patientID)
    {
        HealthCard hc = null;
        if (patientHealthCardCache[patientID] != null)
        {
            PatientActiveHealthCards hcs = (PatientActiveHealthCards)patientHealthCardCache[patientID];
            if (hcs.MedicareCard != null)
                hc = hcs.MedicareCard;
            if (hcs.DVACard != null)
                hc = hcs.DVACard;
        }

        return hc;
    }

    protected HealthCardEPCRemaining[] GetEPCRemainingFromCache(Hashtable epcRemainingCache, HealthCard hc)
    {
        if (hc == null)
            return new HealthCardEPCRemaining[] { };

        HealthCardEPCRemaining[] epcsRemaining = null;
        if (epcRemainingCache == null)
        {
            epcsRemaining = HealthCardEPCRemainingDB.GetByHealthCardID(hc.HealthCardID, -1);
        }
        else
        {
            epcsRemaining = (HealthCardEPCRemaining[])epcRemainingCache[hc.HealthCardID];
        }

        return epcsRemaining == null ? new HealthCardEPCRemaining[] { } : epcsRemaining;
    }

    #endregion

    #region btnExport_Click

    protected void btnExport_Click(object sender, EventArgs e)
    {
        Organisation org = OrganisationDB.GetByID(GetFormID());
        if (org == null)
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        DataTable dt = Session["registerpatienttoorg_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (tblEmpty)
            dt.Rows.RemoveAt(0);


        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("ID"                       ).Append(",");
        sb.Append("Clinic/Fac"               ).Append(",");
        sb.Append("Patient"                  ).Append(",");
        sb.Append("Date Added To Clinic/Fac" ).Append(",");
        sb.Append("Last Booking"             ).Append(",");
        sb.Append("Next Booking"             ).Append(",");
        sb.Append("EPC Expires"              ).Append(",");
        sb.Append("EPC's Remaining"          ).Append(",");
        sb.Append("Last Recall Letter Sent");
        sb.AppendLine();

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append( dt.Rows[i]["patient_id"].ToString()                                                                                                    ).Append(",");
            sb.Append( org.Name                                                                                                                               ).Append(",");
            sb.Append( dt.Rows[i]["firstname"].ToString() + " " + dt.Rows[i]["surname"].ToString()                                                            ).Append(",");
            sb.Append( ((DateTime)dt.Rows[i]["register_patient_date_added"]).ToString("dd-MM-yyyy")                                                           ).Append(",");
            sb.Append( dt.Rows[i]["last_booking_date"] == DBNull.Value ? "" : ((DateTime)dt.Rows[i]["last_booking_date"]).ToString("dd-MM-yyyy")              ).Append(",");
            sb.Append( dt.Rows[i]["next_booking_date"] == DBNull.Value ? "" : ((DateTime)dt.Rows[i]["next_booking_date"]).ToString("dd-MM-yyyy")              ).Append(",");
            sb.Append( dt.Rows[i]["epc_expire_date"]   == DBNull.Value ? "" : ((DateTime)dt.Rows[i]["epc_expire_date"]).ToString("dd-MM-yyyy")                ).Append(",");
            sb.Append( dt.Rows[i]["epc_count_remaining"].ToString()                                                                                           ).Append(",");
            sb.Append( dt.Rows[i]["most_recent_recall_sent"] == DBNull.Value ? "" : ((DateTime)dt.Rows[i]["most_recent_recall_sent"]).ToString("dd-MM-yyyy")  );
            sb.AppendLine();
        }

        ExportCSV(Response, sb.ToString(), "Patients of " + org.Name + ".csv");
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

    #endregion

    #region btnSearchSurname_Click, btnClearSurnameSearch_Click

    protected void btnSearchSurname_Click(object sender, EventArgs e)
    {
        if (!Regex.IsMatch(txtSearchSurname.Text, @"^[a-zA-Z\-\']*$"))
        {
            SetErrorMessage("Search text can only be letters and hyphens");
            return;
        }
        else
            HideErrorMessage();

        string url = Request.RawUrl;
        url = UrlParamModifier.AddEdit(url, "surname_search", txtSearchSurname.Text);
        url = UrlParamModifier.AddEdit(url, "surname_starts_with", chkSurnameSearchOnlyStartWith.Checked ? "1" : "0");
        Response.Redirect(url);
    }
    protected void btnClearSurnameSearch_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["surname_search"] != null || Request.QueryString["surname_starts_with"] != null)
        {
            string url = Request.RawUrl;
            url = UrlParamModifier.Remove(url, "surname_search");
            url = UrlParamModifier.Remove(url, "surname_starts_with");
            Response.Redirect(url);
        }
        else
            txtSearchSurname.Text = "";
    }

    protected void btnSurnameSearch_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "SurnameSearch")
        {
            string letter = e.CommandArgument.ToString();

            if (letter == "All")
            {
                string url = Request.RawUrl;
                url = UrlParamModifier.Remove(url, "surname_search");
                url = UrlParamModifier.Remove(url, "surname_starts_with");
                Response.Redirect(url);
            }
            else
            {
                string url = Request.RawUrl;
                url = UrlParamModifier.AddEdit(url, "surname_search", letter);
                url = UrlParamModifier.AddEdit(url, "surname_starts_with", "1");
                Response.Redirect(url);
            }
        }
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        GrdRegistration.Visible = false;
        lnkThisOrg.Visible = false; 
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