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

public partial class RegisterStaffToOrganisationV2 : System.Web.UI.Page
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
                Session.Remove("registerstafftoorg_sortexpression");
                Session.Remove("registerstafftoorg_data");
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

        Organisation org = OrganisationDB.GetByID(GetFormID());
        if (org == null)
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        lblHeading.Text = Page.Title = "Manage Staff For :  " + org.Name;
        this.lnkThisOrg.NavigateUrl = "~/OrganisationDetailV2.aspx?type=view&id=" + GetFormID().ToString();
        this.lnkThisOrg.Text = "Back to details for " + org.Name;


        DataTable dt = RegisterStaffDB.GetDataTable_StaffOf(org.OrganisationID, chkShowDeleted.Checked);
        Session["registerstafftoorg_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["registerstafftoorg_sortexpression"] != null && Session["registerstafftoorg_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["registerstafftoorg_sortexpression"].ToString();
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
        Organisation org = OrganisationDB.GetByID(GetFormID());
        if (org == null)
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        DataTable dt = Session["registerstafftoorg_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("register_staff_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlStaff = (DropDownList)e.Row.FindControl("ddlStaff");
            if (ddlStaff != null)
            {
                Staff[] incList_orig = RegisterStaffDB.GetStaffOf(org.OrganisationID);
                Staff[] incList = Staff.RemoveByID(incList_orig, Convert.ToInt32(thisRow["staff_id"]));
                DataTable staff = StaffDB.GetDataTable_AllNotInc(incList);
                staff.DefaultView.Sort = "surname ASC";
                foreach (DataRowView row in staff.DefaultView)
                    ddlStaff.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString() + " " + row["middlename"].ToString(), row["staff_id"].ToString()));
                ddlStaff.SelectedValue = thisRow["staff_id"].ToString();
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
            DropDownList ddlStaff = (DropDownList)e.Row.FindControl("ddlNewStaff");
            if (ddlStaff != null)
            {
                Staff[] incList = RegisterStaffDB.GetStaffOf(org.OrganisationID);
                DataTable staff = StaffDB.GetDataTable_AllNotInc(incList);
                staff.DefaultView.Sort = "surname ASC";
                foreach (DataRowView row in staff.DefaultView)
                    ddlStaff.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString() + " " + row["middlename"].ToString(), row["staff_id"].ToString()));

                if (staff.Rows.Count == 0)
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
        Label        lblId             = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlStaff          = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlStaff");
        TextBox      txtProviderNumber = (TextBox)GrdRegistration.Rows[e.RowIndex].FindControl("txtProviderNumber");
        CheckBox     chkMainProvider   = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkMainProvider");
        CheckBox     chkIncMondays     = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncMondays");
        CheckBox     chkIncTuesdays    = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncTuesdays");
        CheckBox     chkIncWednesdays  = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncWednesdays");
        CheckBox     chkIncThursdays   = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncThursdays");
        CheckBox     chkIncFridays     = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncFridays");
        CheckBox     chkIncSaturdays   = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncSaturdays");
        CheckBox     chkIncSundays     = (CheckBox)GrdRegistration.Rows[e.RowIndex].FindControl("chkIncSundays");

        Organisation org = OrganisationDB.GetByID(GetFormID());
        if (org == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        RegisterStaffDB.Update(Convert.ToInt32(lblId.Text), org.OrganisationID, Convert.ToInt32(ddlStaff.SelectedValue), txtProviderNumber.Text, chkMainProvider.Checked,
                               !chkIncSundays.Checked, !chkIncMondays.Checked, !chkIncTuesdays.Checked, !chkIncWednesdays.Checked, !chkIncThursdays.Checked, !chkIncFridays.Checked, !chkIncSaturdays.Checked);
        if (chkMainProvider.Checked)
            RegisterStaffDB.UpdateAllOtherStaffAsNotMainProviders(org.OrganisationID, Convert.ToInt32(ddlStaff.SelectedValue));

        GrdRegistration.EditIndex = -1;
        FillGrid();
    }
    protected void GrdRegistration_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");

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
            DropDownList ddlStaff          = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewStaff");
            TextBox      txtProviderNumber = (TextBox)GrdRegistration.FooterRow.FindControl("txtNewProviderNumber");
            CheckBox     chkMainProvider   = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewMainProvider");
            CheckBox     chkIncMondays     = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncMondays");
            CheckBox     chkIncTuesdays    = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncTuesdays");
            CheckBox     chkIncWednesdays  = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncWednesdays");
            CheckBox     chkIncThursdays   = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncThursdays");
            CheckBox     chkIncFridays     = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncFridays");
            CheckBox     chkIncSaturdays   = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncSaturdays");
            CheckBox     chkIncSundays     = (CheckBox)GrdRegistration.FooterRow.FindControl("chkNewIncSundays");


            Organisation org = OrganisationDB.GetByID(GetFormID());
            if (org == null)
            {
                HideTableAndSetErrorMessage("");
                return;
            }

            try
            {
                RegisterStaffDB.Insert(org.OrganisationID, Convert.ToInt32(ddlStaff.SelectedValue), txtProviderNumber.Text, chkMainProvider.Checked,
                                       !chkIncSundays.Checked, !chkIncMondays.Checked, !chkIncTuesdays.Checked, !chkIncWednesdays.Checked, !chkIncThursdays.Checked, !chkIncFridays.Checked, !chkIncSaturdays.Checked);
                if (chkMainProvider.Checked)
                    RegisterStaffDB.UpdateAllOtherStaffAsNotMainProviders(org.OrganisationID, Convert.ToInt32(ddlStaff.SelectedValue));
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
        DataTable dataTable = Session["registerstafftoorg_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["registerstafftoorg_sortexpression"] == null)
                Session["registerstafftoorg_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["registerstafftoorg_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["registerstafftoorg_sortexpression"] = sortExpression + " " + newSortExpr;

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