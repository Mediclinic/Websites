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

public partial class BookingChangeHistoryReasonListV2 : System.Web.UI.Page
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
                Session.Remove("bookingchangehistoryreason_sortexpression");
                Session.Remove("bookingchangehistoryreason_data");
                FillGrid();
            }

            if (!Utilities.IsDev())
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);

            this.GrdBookingChangeHistoryReason.EnableViewState = true;

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

    #region GrdBookingChangeHistoryReason

    protected void FillGrid()
    {
        DataTable dt = BookingChangeHistoryReasonDB.GetDataTable();
        Session["bookingchangehistoryreason_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdBookingChangeHistoryReason.DataSource = dt;
            try
            {
                GrdBookingChangeHistoryReason.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdBookingChangeHistoryReason.DataSource = dt;
            GrdBookingChangeHistoryReason.DataBind();

            int TotalColumns = GrdBookingChangeHistoryReason.Rows[0].Cells.Count;
            GrdBookingChangeHistoryReason.Rows[0].Cells.Clear();
            GrdBookingChangeHistoryReason.Rows[0].Cells.Add(new TableCell());
            GrdBookingChangeHistoryReason.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdBookingChangeHistoryReason.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdBookingChangeHistoryReason_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdBookingChangeHistoryReason_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["bookingchangehistoryreason_data"] as DataTable;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int booking_change_history_reason_id = Convert.ToInt32(((Label)e.Row.FindControl("lblId")).Text);
            DataRow[] foundRows = dt.Select("booking_change_history_reason_id=" + booking_change_history_reason_id.ToString());

            DropDownList ddlDisplayOrder = (DropDownList)e.Row.FindControl("ddlDisplayOrder");
            if (ddlDisplayOrder != null)
            {
                for (int i = 0; i <= 50; i++)
                    ddlDisplayOrder.Items.Add(new ListItem(i.ToString(), i.ToString()));

                ddlDisplayOrder.SelectedValue = foundRows[0]["display_order"].ToString();
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
    protected void GrdBookingChangeHistoryReason_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdBookingChangeHistoryReason.EditIndex = -1;
        FillGrid();
    }
    protected void GrdBookingChangeHistoryReason_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId           = (Label)GrdBookingChangeHistoryReason.Rows[e.RowIndex].FindControl("lblId");
        TextBox      txtDescr        = (TextBox)GrdBookingChangeHistoryReason.Rows[e.RowIndex].FindControl("txtDescr");
        DropDownList ddlDisplayOrder = (DropDownList)GrdBookingChangeHistoryReason.Rows[e.RowIndex].FindControl("ddlDisplayOrder");

        BookingChangeHistoryReasonDB.Update(Convert.ToInt32(lblId.Text), txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue));

        GrdBookingChangeHistoryReason.EditIndex = -1;
        FillGrid();
    }
    protected void GrdBookingChangeHistoryReason_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdBookingChangeHistoryReason.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //BookingChangeHistoryReasonDB.Delete(Convert.ToInt32(lblId.Text));
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
    protected void GrdBookingChangeHistoryReason_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            TextBox      txtDescr        = (TextBox)GrdBookingChangeHistoryReason.FooterRow.FindControl("txtNewDescr");
            DropDownList ddlDisplayOrder = (DropDownList)GrdBookingChangeHistoryReason.FooterRow.FindControl("ddlNewDisplayOrder");

            BookingChangeHistoryReasonDB.Insert(txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue));

            FillGrid();
        }
    }
    protected void GrdBookingChangeHistoryReason_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdBookingChangeHistoryReason.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdBookingChangeHistoryReason.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["bookingchangehistoryreason_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["bookingchangehistoryreason_sortexpression"] == null)
                Session["bookingchangehistoryreason_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["bookingchangehistoryreason_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["bookingchangehistoryreason_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdBookingChangeHistoryReason.DataSource = dataView;
            GrdBookingChangeHistoryReason.DataBind();
        }
    }

    #endregion

    #region lbAddOneDisplayOrderToAll_Click, lbSubtractOneDisplayOrderToAll_Click

    protected void lbAddOneDisplayOrderToAll_Click(object sender, EventArgs e)
    {
        BookingChangeHistoryReasonDB.UpdateDisplayOrderAll(1);
        FillGrid();
    }
    protected void lbSubtractOneDisplayOrderToAll_Click(object sender, EventArgs e)
    {
        BookingChangeHistoryReasonDB.UpdateDisplayOrderAll(-1);
        FillGrid();
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