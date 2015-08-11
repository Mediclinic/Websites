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

public partial class BookingUnavailabilityReasonListV2 : System.Web.UI.Page
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
                Session.Remove("bookingunavailabilityreason_sortexpression");
                Session.Remove("bookingunavailabilityreason_data");
                FillGrid();
            }

            if (!Utilities.IsDev())
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);

            this.GrdBookingUnavailabilityReason.EnableViewState = true;

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

    #region GridView

    protected void FillGrid()
    {
        DataTable dt = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "BookingUnavailabilityReason", "", "booking_unavailability_reason_type_id, descr", "booking_unavailability_reason_id", "booking_unavailability_reason_type_id", "descr");
        Session["bookingunavailabilityreason_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdBookingUnavailabilityReason.DataSource = dt;
            try
            {
                GrdBookingUnavailabilityReason.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdBookingUnavailabilityReason.DataSource = dt;
            GrdBookingUnavailabilityReason.DataBind();

            int TotalColumns = GrdBookingUnavailabilityReason.Rows[0].Cells.Count;
            GrdBookingUnavailabilityReason.Rows[0].Cells.Clear();
            GrdBookingUnavailabilityReason.Rows[0].Cells.Add(new TableCell());
            GrdBookingUnavailabilityReason.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdBookingUnavailabilityReason.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdBookingUnavailabilityReason_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdBookingUnavailabilityReason_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["bookingunavailabilityreason_data"] as DataTable;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int booking_unavailability_reason_id = Convert.ToInt32(((Label)e.Row.FindControl("lblId")).Text);
            DataRow[] foundRows = dt.Select("booking_unavailability_reason_id=" + booking_unavailability_reason_id.ToString());

            DropDownList ddlType = (DropDownList)e.Row.FindControl("ddlType");
            if (ddlType != null)
                ddlType.SelectedValue = foundRows[0]["booking_unavailability_reason_type_id"].ToString();

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

        }
    }
    protected void GrdBookingUnavailabilityReason_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdBookingUnavailabilityReason.EditIndex = -1;
        FillGrid();
    }
    protected void GrdBookingUnavailabilityReason_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId           = (Label)GrdBookingUnavailabilityReason.Rows[e.RowIndex].FindControl("lblId");
        TextBox      txtDescr        = (TextBox)GrdBookingUnavailabilityReason.Rows[e.RowIndex].FindControl("txtDescr");
        DropDownList ddlType         = (DropDownList)GrdBookingUnavailabilityReason.Rows[e.RowIndex].FindControl("ddlType");

        string sql = "UPDATE BookingUnavailabilityReason SET booking_unavailability_reason_type_id = " + ddlType.SelectedValue + ", descr = '" + txtDescr.Text.Replace("'", "''") + "'" + " WHERE booking_unavailability_reason_id = " + lblId.Text;
        DBBase.ExecuteNonResult(sql);

        GrdBookingUnavailabilityReason.EditIndex = -1;
        FillGrid();
    }
    protected void GrdBookingUnavailabilityReason_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdBookingUnavailabilityReason.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //BookingUnavailabilityReasonDB.Delete(Convert.ToInt32(lblId.Text));
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
    protected void GrdBookingUnavailabilityReason_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            TextBox      txtDescr        = (TextBox)GrdBookingUnavailabilityReason.FooterRow.FindControl("txtNewDescr");
            DropDownList ddlType         = (DropDownList)GrdBookingUnavailabilityReason.FooterRow.FindControl("ddlNewType");

            string sql = "INSERT INTO BookingUnavailabilityReason (booking_unavailability_reason_type_id,descr) VALUES (" + ddlType.SelectedValue + ",'" + txtDescr.Text.Replace("'", "''") + "'" + ");SELECT SCOPE_IDENTITY();";
            int newID = Convert.ToInt32(DBBase.ExecuteSingleResult(sql));

            FillGrid();
        }
    }
    protected void GrdBookingUnavailabilityReason_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdBookingUnavailabilityReason.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdBookingUnavailabilityReason.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["bookingunavailabilityreason_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["bookingunavailabilityreason_sortexpression"] == null)
                Session["bookingunavailabilityreason_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["bookingunavailabilityreason_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["bookingunavailabilityreason_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdBookingUnavailabilityReason.DataSource = dataView;
            GrdBookingUnavailabilityReason.DataBind();
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