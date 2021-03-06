﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;

public partial class PatientListPopupV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);

            if (!IsPostBack)
            {
                Session.Remove("patientinfo_sortexpression");
                Session.Remove("patientinfo_data");
                FillPatientGrid();
                txtSearchFullName.Focus();
            }

            SetFocus(txtSearchFullName);
            this.GrdPatient.EnableViewState = true;
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

    #region GetUrlParams

    protected bool IsValidFormOrg()
    {
        string orgID = Request.QueryString["org"];
        return orgID != null && Regex.IsMatch(orgID, @"^\d+$") && OrganisationDB.Exists(Convert.ToInt32(orgID));
    }
    protected int GetFormOrg(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormOrg())
            throw new Exception("Invalid url org");
        return Convert.ToInt32(Request.QueryString["org"]);
    }

    protected bool IsValidFormOrgs()
    {
        string orgIDs = Request.QueryString["orgs"];
        return orgIDs != null && Regex.IsMatch(orgIDs, @"^[\d,]+$") && OrganisationDB.Exists(orgIDs);
    }
    protected string GetFormOrgs(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormOrgs())
            throw new Exception("Invalid url orgs");
        return Request.QueryString["orgs"];
    }

    protected bool IsValidFormRef()
    {
        string regRefID = Request.QueryString["ref"];
        return regRefID != null && Regex.IsMatch(regRefID, @"^\d+$") && RegisterReferrerDB.Exists(Convert.ToInt32(regRefID));
    }
    protected int GetFormRef(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormRef())
            throw new Exception("Invalid url ref");
        return Convert.ToInt32(Request.QueryString["ref"]);
    }

    #endregion

    #region GrdPatient

    protected void FillPatientGrid()
    {
        UserView userView = UserView.GetInstance();

        lblHeading.Text = !userView.IsAgedCareView ? "Patients" : "Residents";

        
        int regRefID = IsValidFormRef() ? GetFormRef(false) : -1;
        int orgID = IsValidFormOrg() ? GetFormOrg(false) : 0;
        string orgIDs = orgID != 0 ? orgID.ToString() : (IsValidFormOrgs() ? GetFormOrgs(false) : string.Empty);


        DataTable dt = null;

        if (regRefID != -1)
            dt = PatientReferrerDB.GetDataTable_PatientsOf(regRefID, false, false, userView.IsClinicView, userView.IsGPView, txtSearchSurname.Text.Trim(), chkSurnameSearchOnlyStartWith.Checked);
        else if (orgIDs != string.Empty)
            dt = RegisterPatientDB.GetDataTable_PatientsOf(false, orgIDs, false, false, userView.IsClinicView, userView.IsGPView, txtSearchSurname.Text.Trim(), chkSurnameSearchOnlyStartWith.Checked);
        else
            dt = PatientDB.GetDataTable(false, false, userView.IsClinicView, userView.IsGPView, txtSearchSurname.Text.Trim(), chkSurnameSearchOnlyStartWith.Checked);

        // update AjaxLivePatientSurnameSearch and PatientListV2.aspx and PatientListPopup to disallow providers to see other patients.
        if (userView.IsProviderView)  // remove any patients who they haven't had bookings with before
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

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["patientinfo_sortexpression"] != null && Session["patientinfo_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["patientinfo_sortexpression"].ToString();
                GrdPatient.DataSource = dataView;
            }
            else
            {
                GrdPatient.DataSource = dt;
            }

            try
            {
                GrdPatient.DataBind();
                GrdPatient.PagerSettings.FirstPageText = "1";
                GrdPatient.PagerSettings.LastPageText = GrdPatient.PageCount.ToString();
                GrdPatient.DataBind();
            }
            catch (Exception ex)
            {
                this.lblErrorMessage.Visible = true;
                this.lblErrorMessage.Text = ex.ToString();
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdPatient.DataSource = dt;
            GrdPatient.DataBind();

            int TotalColumns = GrdPatient.Rows[0].Cells.Count;
            GrdPatient.Rows[0].Cells.Clear();
            GrdPatient.Rows[0].Cells.Add(new TableCell());
            GrdPatient.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdPatient.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdPatient_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {
            e.Row.Cells[0].CssClass = "hiddencol";
        }
    }
    protected void GrdPatient_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["patientinfo_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("patient_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            Button btnSelect = (Button)e.Row.FindControl("btnSelect");
            if (btnSelect != null)
                btnSelect.OnClientClick = "javascript:select_patient('" + thisRow["patient_id"].ToString() + ":" + thisRow["firstname"].ToString() + " " + thisRow["surname"].ToString() + "');";


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdPatient_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdPatient.EditIndex = -1;
        FillPatientGrid();
    }
    protected void GrdPatient_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdPatient_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdPatient_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdPatient_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdPatient.EditIndex = e.NewEditIndex;
        FillPatientGrid();
    }
    protected void GrdPatient_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdPatient.EditIndex >= 0)
            return;

        DataTable dataTable = Session["patientinfo_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["patientinfo_sortexpression"] == null)
                Session["patientinfo_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["patientinfo_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["patientinfo_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdPatient.DataSource = dataView;
            GrdPatient.DataBind();
        }
    }
    protected void GrdPatient_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdPatient.PageIndex = e.NewPageIndex;
        FillPatientGrid();
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
        else if (txtSearchSurname.Text.Trim().Length == 0)
        {
            SetErrorMessage("No search text entered");
            return;
        }
        else
            HideErrorMessage();


        FillPatientGrid();
    }
    protected void btnClearSurnameSearch_Click(object sender, EventArgs e)
    {
        txtSearchSurname.Text = string.Empty;

        FillPatientGrid();
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