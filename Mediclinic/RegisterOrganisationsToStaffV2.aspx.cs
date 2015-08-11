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

public partial class RegisterOrganisationsToStaffV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            SetProviderNbrs();

            if (!IsPostBack)
            {
                Session.Remove("registerorgtostaff_sortexpression");
                Session.Remove("registerorgtostaff_data");
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

    protected void SetProviderNbrs()
    {
        string provs = StaffDB.GetAllProviderNbrs() + RegisterStaffDB.GetAllProviderNbrs();
        if (provs.Length > 0) provs = provs.Substring(0, provs.Length - 1);
        lblProvNbrs.Text = provs;
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

        Staff staff = StaffDB.GetByID(GetFormID());
        if (staff == null)
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }
        staff.Person = PersonDB.GetByID(staff.Person.PersonID);

        lblHeading.Text = Page.Title = "Manage Clinics/Facilities For :  " + staff.Person.Firstname + " " + staff.Person.Surname;
        this.lnkThisStaff.NavigateUrl = staff.IsExternal ?  "~/StaffDetailExternalV2.aspx?type=view&id=" + GetFormID().ToString() :  "~/StaffDetailV2.aspx?type=view&id=" + GetFormID().ToString();
        this.lnkThisStaff.Text = "Back to details for " + staff.Person.Firstname + " " + staff.Person.Surname;


        if (staff.IsExternal)
        {
            GrdRegistration.Columns[3].Visible = false;
            GrdRegistration.Columns[4].Visible = false;
            GrdRegistration.Columns[5].Visible = false;
            GrdRegistration.Columns[6].Visible = false;
            GrdRegistration.Columns[7].Visible = false;
            GrdRegistration.Columns[8].Visible = false;
            GrdRegistration.Columns[9].Visible = false;
            GrdRegistration.Columns[10].Visible = false;
            GrdRegistration.Columns[11].Visible = false;
        }


        DataTable dt = RegisterStaffDB.GetDataTable_OrganisationsOf(staff.StaffID, null, chkShowDeleted.Checked);
        Session["registerorgtostaff_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["registerorgtostaff_sortexpression"] != null && Session["registerorgtostaff_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["registerorgtostaff_sortexpression"].ToString();
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

        if (e.Row.RowType != DataControlRowType.Pager)
        {
            foreach (DataControlField col in GrdRegistration.Columns)
            {
                if (!chkShowDeleted.Checked)
                    if (col.HeaderText.ToLower().Trim() == "deleted")
                        e.Row.Cells[GrdRegistration.Columns.IndexOf(col)].CssClass = "hiddencol";
            }
        }
    }
    protected void GrdRegistration_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Staff staff = StaffDB.GetByID(GetFormID());
        if (staff == null)
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        DataTable dt = Session["registerorgtostaff_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("register_staff_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlOrganisation = (DropDownList)e.Row.FindControl("ddlOrganisation");
            if (ddlOrganisation != null)
            {
                Organisation[] incList_orig = RegisterStaffDB.GetOrganisationsOf(staff.StaffID);
                Organisation[] incList = Organisation.RemoveByID(incList_orig, Convert.ToInt32(thisRow["organisation_id"]));
                DataTable orgs = OrganisationDB.GetDataTable_AllNotInc(incList, true, false, false, true, true);
                orgs.DefaultView.Sort = "name ASC";

                foreach (DataRowView row in orgs.DefaultView)
                    ddlOrganisation.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
                ddlOrganisation.SelectedValue = thisRow["organisation_id"].ToString();
            }

            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["registration_is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName = "_UnDelete";
                    btnDelete.ImageUrl = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip = "UnDelete";

                    btnDelete.Visible = false;
                }
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlOrganisation = (DropDownList)e.Row.FindControl("ddlNewOrganisation");
            if (ddlOrganisation != null)
            {
                Organisation[] incList = RegisterStaffDB.GetOrganisationsOf(staff.StaffID);
                DataTable orgs = OrganisationDB.GetDataTable_AllNotInc(incList, true, false, false, true, true);
                orgs.DefaultView.Sort = "name ASC";

                foreach (DataRowView row in orgs.DefaultView)
                    ddlOrganisation.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));

                if (orgs.Rows.Count == 0)
                    hideFotter = true;
            }

            if (staff.IsExternal)
            {
                CheckBox chkNewIncMondays    = (CheckBox)e.Row.FindControl("chkNewIncMondays");
                CheckBox chkNewIncTuesdays   = (CheckBox)e.Row.FindControl("chkNewIncTuesdays");
                CheckBox chkNewIncWednesdays = (CheckBox)e.Row.FindControl("chkNewIncWednesdays");
                CheckBox chkNewIncThursdays  = (CheckBox)e.Row.FindControl("chkNewIncThursdays");
                CheckBox chkNewIncFridays    = (CheckBox)e.Row.FindControl("chkNewIncFridays");
                CheckBox chkNewIncSaturdays  = (CheckBox)e.Row.FindControl("chkNewIncSaturdays");
                CheckBox chkNewIncSundays    = (CheckBox)e.Row.FindControl("chkNewIncSundays");

                if (chkNewIncMondays    != null)
                    chkNewIncMondays.Checked    = false;
                if (chkNewIncTuesdays   != null)
                    chkNewIncTuesdays.Checked   = false;
                if (chkNewIncWednesdays != null)
                    chkNewIncWednesdays.Checked = false;
                if (chkNewIncThursdays  != null)
                    chkNewIncThursdays.Checked  = false;
                if (chkNewIncFridays    != null)
                    chkNewIncFridays.Checked    = false;
                if (chkNewIncSaturdays  != null)
                    chkNewIncSaturdays.Checked  = false;
                if (chkNewIncSundays    != null)
                    chkNewIncSundays.Checked    = false;
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
        DropDownList ddlOrganisation = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlOrganisation");
        TextBox txtProviderNumber = (TextBox)GrdRegistration.Rows[e.RowIndex].FindControl("txtProviderNumber");
        CheckBox chkMainProvider  = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkMainProvider");
        CheckBox chkIncMondays    = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncMondays");
        CheckBox chkIncTuesdays   = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncTuesdays");
        CheckBox chkIncWednesdays = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncWednesdays");
        CheckBox chkIncThursdays  = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncThursdays");
        CheckBox chkIncFridays    = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncFridays");
        CheckBox chkIncSaturdays  = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncSaturdays");
        CheckBox chkIncSundays    = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncSundays");


        Staff staff = StaffDB.GetByID(GetFormID());
        if (staff == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        RegisterStaffDB.Update(Convert.ToInt32(lblId.Text), Convert.ToInt32(ddlOrganisation.SelectedValue), staff.StaffID, txtProviderNumber.Text, chkMainProvider.Checked,
                               !chkIncSundays.Checked, !chkIncMondays.Checked, !chkIncTuesdays.Checked, !chkIncWednesdays.Checked, !chkIncThursdays.Checked, !chkIncFridays.Checked, !chkIncSaturdays.Checked);
        if (chkMainProvider.Checked)
            RegisterStaffDB.UpdateAllOtherStaffAsNotMainProviders(Convert.ToInt32(ddlOrganisation.SelectedValue), staff.StaffID);

        GrdRegistration.EditIndex = -1;
        FillGrid();
    }
    protected void GrdRegistration_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.FooterRow.FindControl("lblId");

        RegisterStaff registerStaff = RegisterStaffDB.GetByID(Convert.ToInt32(lblId.Text));
        if (BookingDB.GetCountByProviderAndOrg(registerStaff.Staff.StaffID, registerStaff.Organisation.OrganisationID) > 0)
        {
            SetErrorMessage("Can not remove registration of '" + registerStaff.Staff.Person.FullnameWithoutMiddlename + "' to '" + registerStaff.Organisation.Name + "' because there exists a booking for this provider there.");
            return;
        }
        
        try
        {
            RegisterStaffDB.UpdateInactive(Convert.ToInt32(lblId.Text), false);
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
            DropDownList ddlOrganisation = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewOrganisation");
            TextBox txtProviderNumber = (TextBox)GrdRegistration.FooterRow.FindControl("txtNewProviderNumber");
            CheckBox chkMainProvider = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewMainProvider");
            CheckBox chkIncMondays    = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncMondays");
            CheckBox chkIncTuesdays   = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncTuesdays");
            CheckBox chkIncWednesdays = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncWednesdays");
            CheckBox chkIncThursdays  = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncThursdays");
            CheckBox chkIncFridays    = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncFridays");
            CheckBox chkIncSaturdays  = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncSaturdays");
            CheckBox chkIncSundays    = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncSundays");


            Staff staff = StaffDB.GetByID(GetFormID());
            if (staff == null)
            {
                HideTableAndSetErrorMessage("");
                return;
            }

            try
            {
                RegisterStaffDB.Insert(Convert.ToInt32(ddlOrganisation.SelectedValue), staff.StaffID, txtProviderNumber.Text, chkMainProvider.Checked,
                                       !chkIncSundays.Checked, !chkIncMondays.Checked, !chkIncTuesdays.Checked, !chkIncWednesdays.Checked, !chkIncThursdays.Checked, !chkIncFridays.Checked, !chkIncSaturdays.Checked);
                if (chkMainProvider.Checked)
                    RegisterStaffDB.UpdateAllOtherStaffAsNotMainProviders(Convert.ToInt32(ddlOrganisation.SelectedValue), staff.StaffID);
            }
            catch (UniqueConstraintException) 
            {
                // happens when 2 forms allow adding - do nothing and let form re-update
            }
            FillGrid();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int register_staff_id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    RegisterStaffDB.UpdateInactive(register_staff_id);
                else
                    RegisterStaffDB.UpdateActive(register_staff_id);
            }
            catch (CustomMessageException cmEx)
            {
                SetErrorMessage(cmEx.Message);
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
        DataTable dataTable = Session["registerorgtostaff_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["registerorgtostaff_sortexpression"] == null)
                Session["registerorgtostaff_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["registerorgtostaff_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["registerorgtostaff_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdRegistration.DataSource = dataView;
            GrdRegistration.DataBind();
        }
    }

    #endregion

    #region chkShowDeleted

    protected void chkShowDeleted_CheckedChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        GrdRegistration.Visible = false;
        lnkThisStaff.Visible = false;
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