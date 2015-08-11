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

public partial class OrganisationCustomerTypeListV2 : System.Web.UI.Page
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
                Session.Remove("organisationcustomertype_sortexpression");
                Session.Remove("organisationcustomertype_data");

                bool hasClinics = false;
                bool hasAC      = false;
                foreach (Site site in SiteDB.GetAll())
                {
                    if (site.SiteType.ID == 1 || site.SiteType.ID == 3)
                        hasClinics = true;
                    if (site.SiteType.ID == 2)
                        hasAC = true;
                }

                if (hasClinics && hasAC)
                    lblHeading.Text = Page.Title = "Clinic/Facility Customer Types";
                else if (hasClinics)
                    lblHeading.Text = Page.Title = "Clinic Customer Types";
                else if (hasAC)
                    lblHeading.Text = Page.Title = "Facility Customer Types";

                FillGrid();
            }

            if (!Utilities.IsDev())
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);

            this.GrdOrganisationCustomerType.EnableViewState = true;

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
        DataTable dt = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "OrganisationCustomerType", "organisation_customer_type_id NOT IN (0,139,275)", "descr", "organisation_customer_type_id", "descr");
        Session["organisationcustomertype_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdOrganisationCustomerType.DataSource = dt;
            try
            {
                GrdOrganisationCustomerType.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdOrganisationCustomerType.DataSource = dt;
            GrdOrganisationCustomerType.DataBind();

            int TotalColumns = GrdOrganisationCustomerType.Rows[0].Cells.Count;
            GrdOrganisationCustomerType.Rows[0].Cells.Clear();
            GrdOrganisationCustomerType.Rows[0].Cells.Add(new TableCell());
            GrdOrganisationCustomerType.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdOrganisationCustomerType.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdOrganisationCustomerType_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdOrganisationCustomerType_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["organisationcustomertype_data"] as DataTable;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //int organisation_customer_type_id = Convert.ToInt32(((Label)e.Row.FindControl("lblId")).Text);
            //DataRow[] foundRows = dt.Select("organisation_customer_type_id=" + organisation_customer_type_id.ToString());

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdOrganisationCustomerType_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdOrganisationCustomerType.EditIndex = -1;
        FillGrid();
    }
    protected void GrdOrganisationCustomerType_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId           = (Label)GrdOrganisationCustomerType.Rows[e.RowIndex].FindControl("lblId");
        TextBox      txtDescr        = (TextBox)GrdOrganisationCustomerType.Rows[e.RowIndex].FindControl("txtDescr");

        string sql = "UPDATE OrganisationCustomerType SET descr = '" + txtDescr.Text.Replace("'", "''") + "'" + " WHERE organisation_customer_type_id = " + lblId.Text;
        DBBase.ExecuteNonResult(sql);

        GrdOrganisationCustomerType.EditIndex = -1;
        FillGrid();
    }
    protected void GrdOrganisationCustomerType_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdOrganisationCustomerType.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //OrganisationCustomerTypeDB.Delete(Convert.ToInt32(lblId.Text));
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
    protected void GrdOrganisationCustomerType_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            TextBox      txtDescr        = (TextBox)GrdOrganisationCustomerType.FooterRow.FindControl("txtNewDescr");
            DropDownList ddlDisplayOrder = (DropDownList)GrdOrganisationCustomerType.FooterRow.FindControl("ddlNewDisplayOrder");

            string sql = "INSERT INTO OrganisationCustomerType (descr) VALUES ('" + txtDescr.Text.Replace("'", "''") + "'" + ");SELECT SCOPE_IDENTITY();";
            int newID = Convert.ToInt32(DBBase.ExecuteSingleResult(sql));

            FillGrid();
        }
    }
    protected void GrdOrganisationCustomerType_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdOrganisationCustomerType.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdOrganisationCustomerType.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["organisationcustomertype_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["organisationcustomertype_sortexpression"] == null)
                Session["organisationcustomertype_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["organisationcustomertype_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["organisationcustomertype_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdOrganisationCustomerType.DataSource = dataView;
            GrdOrganisationCustomerType.DataBind();
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