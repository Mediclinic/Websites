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

public partial class ConditionListV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);
                Session.Remove("condition_sortexpression");
                Session.Remove("condition_data");
                chkShowDeleted.Checked = Request.QueryString["show_deleted"] != null && Request.QueryString["show_deleted"] == "1";
                FillGrid();
            }

            this.GrdCondition.EnableViewState = true;

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

    #region GrdCondition

    protected void FillGrid()
    {
        DataTable dt = ConditionDB.GetDataTable(chkShowDeleted.Checked);
        Session["condition_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdCondition.DataSource = dt;
            try
            {
                GrdCondition.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }

            //Sort("parent_descr", "ASC");
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdCondition.DataSource = dt;
            GrdCondition.DataBind();

            int TotalColumns = GrdCondition.Rows[0].Cells.Count;
            GrdCondition.Rows[0].Cells.Clear();
            GrdCondition.Rows[0].Cells.Add(new TableCell());
            GrdCondition.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdCondition.Rows[0].Cells[0].Text = "No Conditions Found";
        }
    }
    protected void GrdCondition_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";

        if (e.Row.RowType != DataControlRowType.Pager)
        {
            foreach (DataControlField col in GrdCondition.Columns)
            {
                if (!chkShowDeleted.Checked)
                    if (col.HeaderText.ToLower().Trim() == "deleted")
                        e.Row.Cells[GrdCondition.Columns.IndexOf(col)].CssClass = "hiddencol";
            }
        }
    }
    protected void GrdCondition_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["condition_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("condition_condition_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];

            DropDownList ddlDisplayOrder = (DropDownList)e.Row.FindControl("ddlDisplayOrder");
            if (ddlDisplayOrder != null)
            {
                for (int i = 0; i <= 50; i++)
                    ddlDisplayOrder.Items.Add(new ListItem(i.ToString(), i.ToString()));

                ddlDisplayOrder.SelectedValue = foundRows[0]["condition_display_order"].ToString();
            }

            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["condition_is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName   = "_UnDelete";
                    btnDelete.ImageUrl      = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip       = "UnDelete";
                }
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlDisplayOrder = (DropDownList)e.Row.FindControl("ddlNewDisplayOrder");
            for (int i = 0; i <= 50; i++)
                ddlDisplayOrder.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
    }
    protected void GrdCondition_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdCondition.EditIndex = -1;
        FillGrid();
    }
    protected void GrdCondition_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId            = (Label)GrdCondition.Rows[e.RowIndex].FindControl("lblId");
        TextBox      txtDescr         = (TextBox)GrdCondition.Rows[e.RowIndex].FindControl("txtDescr");
        DropDownList ddlDisplayOrder  = (DropDownList)GrdCondition.Rows[e.RowIndex].FindControl("ddlDisplayOrder");
        CheckBox     chkShowDate      = (CheckBox)GrdCondition.Rows[e.RowIndex].FindControl("chkShowDate");
        CheckBox     chkShowNWeeksDue = (CheckBox)GrdCondition.Rows[e.RowIndex].FindControl("chkShowNWeeksDue");
        CheckBox     chkShowText      = (CheckBox)GrdCondition.Rows[e.RowIndex].FindControl("chkShowText");

        Condition condition = ConditionDB.GetByID(Convert.ToInt32(lblId.Text));
        ConditionDB.Update(Convert.ToInt32(lblId.Text), txtDescr.Text, chkShowDate.Checked, chkShowNWeeksDue.Checked, chkShowText.Checked, Convert.ToInt32(ddlDisplayOrder.SelectedValue), condition.IsDeleted);

        GrdCondition.EditIndex = -1;
        FillGrid();
    }
    protected void GrdCondition_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdCondition.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //CostCentreDB.Delete(Convert.ToInt32(lblId.Text));
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
    protected void GrdCondition_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlParent        = (DropDownList)GrdCondition.FooterRow.FindControl("ddlNewParent");
            TextBox      txtDescr         = (TextBox)GrdCondition.FooterRow.FindControl("txtNewDescr");
            DropDownList ddlDisplayOrder  = (DropDownList)GrdCondition.FooterRow.FindControl("ddlNewDisplayOrder");
            CheckBox     chkShowDate      = (CheckBox)GrdCondition.FooterRow.FindControl("chkNewShowDate");
            CheckBox     chkShowNWeeksDue = (CheckBox)GrdCondition.FooterRow.FindControl("chkNewShowNWeeksDue");
            CheckBox     chkShowText      = (CheckBox)GrdCondition.FooterRow.FindControl("chkNewShowText");

            ConditionDB.Insert(txtDescr.Text, chkShowDate.Checked, chkShowNWeeksDue.Checked, chkShowText.Checked, Convert.ToInt32(ddlDisplayOrder.SelectedValue), false);

            FillGrid();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    ConditionDB.UpdateInactive(id);
                else
                    ConditionDB.UpdateActive(id);
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
    protected void GrdCondition_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdCondition.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdCondition.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["condition_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["condition_sortexpression"] == null)
                Session["condition_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["condition_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["condition_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdCondition.DataSource = dataView;
            GrdCondition.DataBind();
        }
    }

    #endregion

    #region lbAddOneDisplayOrderToAll_Click, lbSubtractOneDisplayOrderToAll_Click, chkShowDeleted_Submit

    protected void lbAddOneDisplayOrderToAll_Click(object sender, EventArgs e)
    {
        ConditionDB.UpdateDisplayOrderAll(1);
        FillGrid();
    }
    protected void lbSubtractOneDisplayOrderToAll_Click(object sender, EventArgs e)
    {
        ConditionDB.UpdateDisplayOrderAll(-1);
        FillGrid();
    }

    protected void chkShowDeleted_Submit(object sender, EventArgs e)
    {
        string url = Request.RawUrl;
        url = UrlParamModifier.Update(chkShowDeleted.Checked, url, "show_deleted", "1");
        Response.Redirect(url);
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