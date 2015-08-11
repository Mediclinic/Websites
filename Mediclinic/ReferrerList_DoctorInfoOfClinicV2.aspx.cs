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

public partial class ReferrerList_DoctorInfoOfClinicV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, false);
                Session.Remove("referrerinfopersonofclinic_sortexpression");
                Session.Remove("referrerinfopersonofclinic_data");
                FillGrid();
                txtSearchName.Focus();
            }


            this.GrdReferrer.EnableViewState = true;

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

    #endregion

    #region GrdReferrer

    protected void FillGrid()
    {
        string searchName = "";
        if (Request.QueryString["name_search"] != null && Request.QueryString["name_search"].Length > 0)
        {
            searchName = Request.QueryString["name_search"];
            txtSearchName.Text = Request.QueryString["name_search"];
        }
        bool searchNameOnlyStartsWith = true;
        if (Request.QueryString["name_starts_with"] != null && Request.QueryString["name_starts_with"].Length > 0)
        {
            searchNameOnlyStartsWith = Request.QueryString["name_starts_with"] == "0" ? false : true;
            chkSearchOnlyStartWith.Checked = searchNameOnlyStartsWith;
        }
        else
        {
            chkSearchOnlyStartWith.Checked = searchNameOnlyStartsWith;
        }


        if (Request.QueryString["org"] == null || !Regex.IsMatch(Request.QueryString["org"], @"^\d+$"))
        {
            HideTableAndSetErrorMessage("Invalid url org");
            return;
        }



        if (!Regex.IsMatch(Request.QueryString["org"], @"^\d+$"))
        {
            HideTableAndSetErrorMessage("Invalid url org");
            return;
        }
        Organisation org = OrganisationDB.GetByID(Convert.ToInt32(Request.QueryString["org"]));
        if (org == null)
        {
            HideTableAndSetErrorMessage("Invalid url org");
            return;
        }

        lblHeading.Text = "Referrers at " + org.Name;

        DataTable dt = RegisterReferrerDB.GetDataTable_ReferrersOf(org.OrganisationID, true, chkShowDeleted.Checked, searchName, searchNameOnlyStartsWith);

        dt.Columns.Add("original_referrer_id", typeof(Int32));
        dt.Columns.Add("is_deleted", typeof(Boolean));
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            // update these so when delete/undelete - it removes the connection, not the referrer
            dt.Rows[i]["original_referrer_id"] = dt.Rows[i]["referrer_id"];
            dt.Rows[i]["referrer_id"] = dt.Rows[i]["register_referrer_id"];
            dt.Rows[i]["is_deleted"] = dt.Rows[i]["register_referrer_is_deleted"];
        }


        Session["referrerinfopersonofclinic_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["referrerinfopersonofclinic_sortexpression"] != null && Session["referrerinfopersonofclinic_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["referrerinfopersonofclinic_sortexpression"].ToString();
                GrdReferrer.DataSource = dataView;
            }
            else
            {
                GrdReferrer.DataSource = dt;
            }


            try
            {
                GrdReferrer.DataBind();
                GrdReferrer.PagerSettings.FirstPageText = "1";
                GrdReferrer.PagerSettings.LastPageText = GrdReferrer.PageCount.ToString();
                GrdReferrer.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdReferrer.DataSource = dt;
            GrdReferrer.DataBind();

            int TotalColumns = GrdReferrer.Rows[0].Cells.Count;
            GrdReferrer.Rows[0].Cells.Clear();
            GrdReferrer.Rows[0].Cells.Add(new TableCell());
            GrdReferrer.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdReferrer.Rows[0].Cells[0].Text = "No Record Found";
        }

    }
    protected void GrdReferrer_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdReferrer_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["referrerinfopersonofclinic_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("referrer_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlRefs = (DropDownList)e.Row.FindControl("ddlRefs");
            if (ddlRefs != null)
            {
                DataTable dt_refs = dt = ReferrerDB.GetDataTable(-1, "", false, chkShowDeleted.Checked);

                // check for deleted REF
                DataRow[] foundRows_orgs = dt_refs.Select("referrer_id=" + thisRow["original_referrer_id"].ToString());
                if (!chkShowDeleted.Checked && foundRows_orgs.Length == 0)
                {
                    DataTable dt_deleted_orgs = ReferrerDB.GetDataTable(-1, "", false, true);
                    foundRows_orgs = dt_deleted_orgs.Select("referrer_id=" + thisRow["original_referrer_id"].ToString());
                    if (foundRows_orgs.Length == 1)
                        dt_refs.Rows.Add(foundRows_orgs[0].ItemArray);
                }

                ddlRefs.Items.Clear();
                foreach (DataRow row in dt_refs.Rows)
                    ddlRefs.Items.Add(new ListItem(row["firstname"] + " " + row["surname"], row["referrer_id"].ToString()));
                ddlRefs.SelectedValue = thisRow["original_referrer_id"].ToString();
            }


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName = "_UnDelete";
                    btnDelete.ImageUrl = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip = "UnDelete";
                }
            }

            Label lblFirstname = (Label)e.Row.FindControl("lblFirstname");
            if (lblFirstname != null)
                lblFirstname.Visible = (Request.QueryString["org"] != null);

            HyperLink lnkFirstname = (HyperLink)e.Row.FindControl("lnkFirstname");
            if (lnkFirstname != null)
                lnkFirstname.Visible = (Request.QueryString["org"] == null);


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

            DropDownList ddlNewRefs = (DropDownList)e.Row.FindControl("ddlNewRefs");
            if (ddlNewRefs != null)
            {
                DataTable dt_refs = dt = ReferrerDB.GetDataTable(-1, "", false, chkShowDeleted.Checked);

                ddlNewRefs.Items.Clear();
                foreach (DataRow row in dt_refs.Rows)
                    ddlNewRefs.Items.Add(new ListItem(row["firstname"] + " " + row["surname"], row["referrer_id"].ToString()));
            }
        
        }
    }
    protected void GrdReferrer_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdReferrer.EditIndex = -1;
        FillGrid();
    }
    protected void GrdReferrer_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId                 = (Label)GrdReferrer.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlRefs               = (DropDownList)GrdReferrer.Rows[e.RowIndex].FindControl("ddlRefs");
        TextBox      txtProviderNumber     = (TextBox)GrdReferrer.Rows[e.RowIndex].FindControl("txtProviderNumber");
        CheckBox     chkIsReportEveryVisit = (CheckBox)GrdReferrer.Rows[e.RowIndex].FindControl("chkIsReportEveryVisit");
        CheckBox     chkIsBatchSendAllPatientsTreatmentNotes = (CheckBox)GrdReferrer.Rows[e.RowIndex].FindControl("chkIsBatchSendAllPatientsTreatmentNotes");


        RegisterReferrer regRef = RegisterReferrerDB.GetByID(Convert.ToInt32(lblId.Text));

        if (RegisterReferrerDB.Exists(regRef.Organisation.OrganisationID, Convert.ToInt32(ddlRefs.SelectedValue), regRef.RegisterReferrerID))
        {
            SetErrorMessage("Clinic is already linked to " + ddlRefs.SelectedItem.Text + ". If it is not visible, use the 'show deleted' checkbox and un-delete it.");
            GrdReferrer.EditIndex = -1;
            FillGrid();
            return;
        }

        RegisterReferrerDB.Update(
            regRef.RegisterReferrerID,
            regRef.Organisation.OrganisationID, 
            Convert.ToInt32(ddlRefs.SelectedValue),
            txtProviderNumber.Text.Trim(),
            chkIsReportEveryVisit.Checked,
            chkIsBatchSendAllPatientsTreatmentNotes.Checked,
            regRef.DateLastBatchSendAllPatientsTreatmentNotes);

        GrdReferrer.EditIndex = -1;
        FillGrid();
    }
    protected void GrdReferrer_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdReferrer.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            ReferrerDB.UpdateInactive(Convert.ToInt32(lblId.Text));
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrid();
    }
    protected void GrdReferrer_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlRefs               = (DropDownList)GrdReferrer.FooterRow.FindControl("ddlNewRefs");
            TextBox      txtProviderNumber     = (TextBox)GrdReferrer.FooterRow.FindControl("txtNewProviderNumber");
            CheckBox     chkIsReportEveryVisit = (CheckBox)GrdReferrer.FooterRow.FindControl("chkNewIsReportEveryVisit");
            CheckBox     chkIsBatchSendAllPatientsTreatmentNotes = (CheckBox)GrdReferrer.FooterRow.FindControl("chkNewIsBatchSendAllPatientsTreatmentNotes");

            if (RegisterReferrerDB.Exists(Convert.ToInt32(Request.QueryString["org"]), Convert.ToInt32(ddlRefs.SelectedValue)))
            {
                SetErrorMessage("Clinic is already linked to " + ddlRefs.SelectedItem.Text + ". If it is not visible, use the 'show deleted' checkbox and un-delete it.");
                return;
            }

            RegisterReferrerDB.Insert(Convert.ToInt32(Request.QueryString["org"]), Convert.ToInt32(ddlRefs.SelectedValue), txtProviderNumber.Text.Trim(), chkIsReportEveryVisit.Checked, chkIsBatchSendAllPatientsTreatmentNotes.Checked);

            FillGrid();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            try
            {

                // if getting referers of an org, set the reg-ref relationship as active/inactive
                if (Request.QueryString["org"] != null)
                {
                    int reg_ref_id = Convert.ToInt32(e.CommandArgument);

                    if (e.CommandName.Equals("_Delete"))
                        RegisterReferrerDB.UpdateInactive(reg_ref_id);
                    else
                        RegisterReferrerDB.UpdateActive(reg_ref_id);
                }

                // if getting all referrers, set the ref as active/inactive
                else
                {
                    int referrer_id = Convert.ToInt32(e.CommandArgument);

                    if (e.CommandName.Equals("_Delete"))
                        ReferrerDB.UpdateInactive(referrer_id);
                    else
                        ReferrerDB.UpdateActive(referrer_id);
                }

            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                if (Utilities.IsDev())
                    SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
                else
                    SetErrorMessage("Can not delete because other records depend on this");
            }

            FillGrid();
        }

        if (e.CommandName.Equals("ViewPatients"))
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (Request.QueryString["org"] == null)
                FillGrid_Patients(typeof(Referrer), id);
            else
                FillGrid_Patients(typeof(RegisterReferrer), id);
        }

    }
    protected void GrdReferrer_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdReferrer.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdReferrer.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }
    protected void GrdReferrer_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdReferrer.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["referrerinfopersonofclinic_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["referrerinfopersonofclinic_sortexpression"] == null)
                Session["referrerinfopersonofclinic_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["referrerinfopersonofclinic_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["referrerinfopersonofclinic_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdReferrer.DataSource = dataView;
            GrdReferrer.DataBind();
        }
    }

    #endregion

    #region btnSearchName_Click, btnClearNameSearch_Click

    protected void btnSearchName_Click(object sender, EventArgs e)
    {
        //if (!Regex.IsMatch(txtSearchName.Text, @"^[a-zA-Z\-]*$"))
        //{
        //    SetErrorMessage("Search text can only be letters and hyphens");
        //    return;
        //}
        //else 
        if (txtSearchName.Text.Trim().Length == 0)
        {
            SetErrorMessage("No search text entered");
            return;
        }
        else
            HideErrorMessage();


        string url = Request.RawUrl;
        url = UrlParamModifier.AddEdit(url, "name_search", txtSearchName.Text);
        url = UrlParamModifier.AddEdit(url, "name_starts_with", chkSearchOnlyStartWith.Checked ? "1" : "0");
        Response.Redirect(url);
    }
    protected void btnClearNameSearch_Click(object sender, EventArgs e)
    {
        string url = Request.RawUrl;
        url = UrlParamModifier.Remove(url, "name_search");
        url = UrlParamModifier.Remove(url, "name_starts_with");
        Response.Redirect(url);
    }

    protected void chkUsePaging_CheckedChanged(object sender, EventArgs e)
    {
        this.GrdReferrer.AllowPaging = chkUsePaging.Checked;
        FillGrid();
    }
    protected void chkShowDeleted_CheckedChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    #endregion

    #region GrdPatients

    protected void FillGrid_Patients(Type type, int id)
    {

        UserView userView = UserView.GetInstance(); 

        DataTable dt = null;

        lblPatientsHeading.Style["white-space"] = "nowrap";
        if (type == typeof(Referrer))
        {
            Referrer referrer = ReferrerDB.GetByID(id);
            lblPatientsHeading.Text = "Patients of &nbsp;&nbsp;<big><b>" + referrer.Person.FullnameWithoutMiddlename + "</b></big>&nbsp;&nbsp; at &nbsp;&nbsp;<big><b>All Clinics</b></big>";

            if (userView.IsAdminView && userView.IsClinicView)
                dt = PatientDB.GetDataTable(false, false, userView.IsClinicView, false, "", false, "", false, "", "", "", "", "", false, -1, -1, -1, "", "", "", id.ToString(), "", false, false, false);
            if (userView.IsAdminView && !userView.IsClinicView)
                dt = RegisterPatientDB.GetDataTable_PatientsOfOrgGroupType(false, "6", false, false, userView.IsClinicView, false, "", false, "", false, "", "", "", "", "", false, -1, -1, -1, "", "", "", id.ToString(), "", false, false, false);
            if (!userView.IsAdminView)
                dt = RegisterPatientDB.GetDataTable_PatientsOf(false, Convert.ToInt32(Session["OrgID"]), false, false, userView.IsClinicView, false, "", false, "", false, "", "", "", "", "", false, -1, -1, -1, "", "", "", id.ToString(), "", false, false, false);
        }
        else if (type == typeof(RegisterReferrer))
        {
            RegisterReferrer regRef = RegisterReferrerDB.GetByID(id);
            lblPatientsHeading.Text = "Patients of &nbsp;&nbsp;<big><b>" + regRef.Referrer.Person.FullnameWithoutMiddlename + "</b></big>&nbsp;&nbsp; at &nbsp;&nbsp;<big><b>" + regRef.Organisation.Name + "</b></big>";

            if (userView.IsAdminView && userView.IsClinicView)
                dt = PatientDB.GetDataTable(false, false, userView.IsClinicView, false, "", false, "", false, "", "", "", "", "", false, -1, -1, -1, "", "", id.ToString(), "", "", false, false, false);
            if (userView.IsAdminView && !userView.IsClinicView)
                dt = RegisterPatientDB.GetDataTable_PatientsOfOrgGroupType(false, "6", false, false, userView.IsClinicView, false, "", false, "", false, "", "", "", "", "", false, -1, -1, -1, "", "", id.ToString(), "", "", false, false, false);
            if (!userView.IsAdminView)
                dt = RegisterPatientDB.GetDataTable_PatientsOf(false, Convert.ToInt32(Session["OrgID"]), false, false, userView.IsClinicView, false, "", false, "", false, "", "", "", "", "", false, -1, -1, -1, "", "", id.ToString(), "", "", false, false, false);
        }
        else
        {
            SetErrorMessage("Unknown type: " + type.ToString());
            return;
        }


        lblPatientsHeading.Visible = true;
        GrdPatients.Visible = true;



        // put in epc info into the table in a bulk call
        // epc exp date, if valid, how many epc's remaining...


        int[] patientIDs = new int[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
            patientIDs[i] = Convert.ToInt32(dt.Rows[i]["patient_id"]);

        int MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);

        Hashtable patientsMedicareCountThisYearCache = PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, DateTime.Today.Year);
        Hashtable patientsMedicareCountNextYearCache = PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, DateTime.Today.Year + 1);
        Hashtable patientsEPCRemainingCache = PatientsEPCRemainingCacheDB.GetBullk(patientIDs, DateTime.MinValue);

        dt.Columns.Add("epc_signed_date", typeof(DateTime));
        dt.Columns.Add("epc_expiry_date", typeof(DateTime));
        dt.Columns.Add("epc_n_services_left", typeof(Int32));
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int patientID = Convert.ToInt32(dt.Rows[i]["patient_id"]);

            int totalServicesAllowedLeft = (MedicareMaxNbrServicesPerYear - (int)patientsMedicareCountThisYearCache[patientID]);
            Pair totalEPCRemaining = patientsEPCRemainingCache[patientID] as Pair;

            int nServicesLeft = 0;
            if (totalEPCRemaining != null)
            {
                DateTime referralSignedDate = (DateTime)totalEPCRemaining.Second;
                DateTime hcExpiredDate = referralSignedDate.AddYears(1);
                if (DateTime.Today >= referralSignedDate.Date && DateTime.Today < hcExpiredDate.Date)
                    nServicesLeft = (int)totalEPCRemaining.First;
                if (totalServicesAllowedLeft < nServicesLeft)
                    nServicesLeft = totalServicesAllowedLeft;

                dt.Rows[i]["epc_signed_date"] = referralSignedDate;
                dt.Rows[i]["epc_expiry_date"] = hcExpiredDate;
                dt.Rows[i]["epc_n_services_left"] = nServicesLeft;
            }
            else
            {
                dt.Rows[i]["epc_signed_date"] = DBNull.Value;
                dt.Rows[i]["epc_expiry_date"] = DBNull.Value;
                dt.Rows[i]["epc_n_services_left"] = DBNull.Value;
            }

        }



        Session["referrerinfopersonofclinic_patients_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["referrerinfopersonofclinic_patients_sortexpression"] != null && Session["referrerinfopersonofclinic_patients_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["referrerinfopersonofclinic_patients_sortexpression"].ToString();
                GrdPatients.DataSource = dataView;
            }
            else
            {
                GrdPatients.DataSource = dt;
            }


            try
            {
                GrdPatients.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdPatients.DataSource = dt;
            GrdPatients.DataBind();

            int TotalColumns = GrdPatients.Rows[0].Cells.Count;
            GrdPatients.Rows[0].Cells.Clear();
            GrdPatients.Rows[0].Cells.Add(new TableCell());
            GrdPatients.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdPatients.Rows[0].Cells[0].Text = "<center>No Patients</center>";
        }

    }
    protected void GrdPatients_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdPatients_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.Header && UserView.GetInstance().IsAgedCareView)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("EPC", "Ref.");
        }

        DataTable dt = Session["referrerinfopersonofclinic_patients_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");

            string s = "patient_id=" + lblId.Text.Trim();

            DataRow[] foundRows = dt.Select("patient_id=" + lblId.Text.Trim());
            DataRow thisRow = foundRows[0];

            Label lblPatient = (Label)e.Row.FindControl("lblPatient");
            if (lblPatient != null)
            {
                string URL = "PatientDetailV2.aspx?type=view&id=" + thisRow["patient_id"];
                if (URL.StartsWith("~")) URL = URL.Substring(1);
                lblPatient.Text = "<a href=\"#\" onclick=\"open_new_window('" + URL + "');return false;\" >" + thisRow["firstname"] + " " + thisRow["surname"] + "</a>";
            }

            Label lblEPCExpires = (Label)e.Row.FindControl("lblEPCExpires");
            if (lblEPCExpires != null)
            {
                if (thisRow["epc_expiry_date"] != DBNull.Value && Convert.ToDateTime(thisRow["epc_expiry_date"]) < DateTime.Today)
                    lblEPCExpires.ForeColor = System.Drawing.Color.Red;
            }



            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdPatients_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        //if (e.CommandName.Equals("Insert"))
        //{
        //
        //    // do stuff
        //
        //    FillGrid_Patients();
        //}

    }
    protected void GrdPatients_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdPatients.EditIndex >= 0)
            return;

        GrdPatients_Sort(e.SortExpression);
    }
    protected void GrdPatients_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["referrerinfopersonofclinic_patients_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["referrerinfopersonofclinic_patients_sortexpression"] == null)
                Session["referrerinfopersonofclinic_patients_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["referrerinfopersonofclinic_patients_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["referrerinfopersonofclinic_patients_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdPatients.DataSource = dataView;
            GrdPatients.DataBind();
        }
    }

    #endregion
    
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