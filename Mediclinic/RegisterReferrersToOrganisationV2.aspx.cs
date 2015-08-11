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

public partial class RegisterReferrersToOrganisationV2 : System.Web.UI.Page
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
                Session.Remove("registerreferrertoorg_sortexpression");
                Session.Remove("registerreferrertoorg_data");
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


        DataTable dt = RegisterReferrerDB.GetDataTable_ReferrersOf(org.OrganisationID);
        Session["registerreferrertoorg_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["registerreferrertoorg_sortexpression"] != null && Session["registerreferrertoorg_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["registerreferrertoorg_sortexpression"].ToString();
                GrdRegistration.DataSource = dataView;
            }
            else
            {
                GrdRegistration.DataSource = dt;
            } 
            
            
            try
            {
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

        DataTable dt = Session["registerreferrertoorg_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("register_referrer_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];

            DropDownList ddlReferrer = (DropDownList)e.Row.FindControl("ddlReferrer");
            if (ddlReferrer != null)
            {
                Referrer[] incList_orig = RegisterReferrerDB.GetReferrersOf(org.OrganisationID);
                Referrer[] incList = Referrer.RemoveByID(incList_orig, Convert.ToInt32(thisRow["referrer_id"]));
                DataTable referrers = ReferrerDB.GetDataTable_AllNotInc(incList);
                referrers.DefaultView.Sort = "surname ASC";
                foreach (DataRowView row in referrers.DefaultView)
                    ddlReferrer.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString() + " " + row["middlename"].ToString(), row["referrer_id"].ToString()));
                ddlReferrer.SelectedValue = thisRow["referrer_id"].ToString();
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlReferrer = (DropDownList)e.Row.FindControl("ddlNewReferrer");
            if (ddlReferrer != null)
            {
                Referrer[] incList = RegisterReferrerDB.GetReferrersOf(org.OrganisationID);
                DataTable referrers = ReferrerDB.GetDataTable_AllNotInc(incList);
                referrers.DefaultView.Sort = "surname ASC";
                foreach (DataRowView row in referrers.DefaultView)
                    ddlReferrer.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString() + " " + row["middlename"].ToString(), row["referrer_id"].ToString()));

                if (referrers.Rows.Count == 0)
                    hideFotter = true;
            }

            DropDownList ddlIsClinic = (DropDownList)e.Row.FindControl("ddlNewIsClinic");
            ddlIsClinic.SelectedValue = UserView.GetInstance().IsClinicView ? "1" : "0";
        }
    }
    protected void GrdRegistration_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdRegistration.EditIndex = -1;
        FillGrid();
    }
    protected void GrdRegistration_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId             = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlReferrer       = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlReferrer");
        TextBox      txtProviderNumber = (TextBox)GrdRegistration.Rows[e.RowIndex].FindControl("txtProviderNumber");
        DropDownList ddlIsClinic       = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlIsClinic");
        CheckBox     chkIsReportEveryVisit                   = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIsReportEveryVisit");
        CheckBox     chkIsBatchSendAllPatientsTreatmentNotes = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIsBatchSendAllPatientsTreatmentNotes");

        Organisation org = OrganisationDB.GetByID(GetFormID());
        if (org == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        RegisterReferrer registerReferrer = RegisterReferrerDB.GetByID(Convert.ToInt32(lblId.Text));
        RegisterReferrerDB.Update(Convert.ToInt32(lblId.Text), org.OrganisationID, Convert.ToInt32(ddlReferrer.SelectedValue), txtProviderNumber.Text, chkIsReportEveryVisit.Checked, chkIsBatchSendAllPatientsTreatmentNotes.Checked, registerReferrer.DateLastBatchSendAllPatientsTreatmentNotes);

        GrdRegistration.EditIndex = -1;
        FillGrid();
    }
    protected void GrdRegistration_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            RegisterReferrerDB.UpdateInactive(Convert.ToInt32(lblId.Text));
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
            DropDownList ddlReferrer       = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewReferrer");
            TextBox      txtProviderNumber = (TextBox)GrdRegistration.FooterRow.FindControl("txtNewProviderNumber");
            DropDownList ddlIsClinic       = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewIsClinic");
            CheckBox     chkIsReportEveryVisit                   = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIsReportEveryVisit");
            CheckBox     chkIsBatchSendAllPatientsTreatmentNotes = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIsBatchSendAllPatientsTreatmentNotes");

            Organisation org = OrganisationDB.GetByID(GetFormID());
            if (org == null)
            {
                HideTableAndSetErrorMessage("");
                return;
            }

            try
            {
                RegisterReferrerDB.Insert(org.OrganisationID, Convert.ToInt32(ddlReferrer.SelectedValue), txtProviderNumber.Text, chkIsReportEveryVisit.Checked, chkIsBatchSendAllPatientsTreatmentNotes.Checked);
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

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["registerreferrertoorg_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["registerreferrertoorg_sortexpression"] == null)
                Session["registerreferrertoorg_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["registerreferrertoorg_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["registerreferrertoorg_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdRegistration.DataSource = dataView;
            GrdRegistration.DataBind();
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