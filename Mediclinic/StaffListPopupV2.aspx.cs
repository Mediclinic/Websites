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

public partial class StaffListPopupV2 : System.Web.UI.Page
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
                Session.Remove("staff_sortExpression");
                Session.Remove("staff_data");
                FillStaffGrid();
                txtSearchSurname.Focus();
            }

            this.GrdStaff.EnableViewState = true;

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

    protected bool IsValidFormOnlyProviders()
    {
        string only_providers = Request.QueryString["only_providers"];
        return only_providers != null && (Request.QueryString["only_providers"] == "1" || Request.QueryString["only_providers"] == "0");
    }
    protected bool GetFormOnlyProviders(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormOnlyProviders())
            throw new Exception("Invalid url field only_providers");
        return Request.QueryString["only_providers"] == "1";
    }


    /*
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
     */

    #endregion

    #region GrdStaff

    protected void FillStaffGrid()
    {
        DataTable dt = StaffDB.GetDataTable_StaffInfo(false, false, false, false, txtSearchSurname.Text.Trim(), chkSurnameSearchOnlyStartWith.Checked);

        bool onlyProviders = IsValidFormOnlyProviders() ? GetFormOnlyProviders(false) : false;
        if (onlyProviders)
        {
            lblHeading.Text = "Providers";
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
                if (!StaffDB.Load(dt.Rows[i]).IsProvider)
                    dt.Rows.RemoveAt(i);
        }

        Session["staff_data"] = dt;

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["staff_sortExpression"] != null && Session["staff_sortExpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["staff_sortExpression"].ToString();
                GrdStaff.DataSource = dataView;
            }
            else
            {
                GrdStaff.DataSource = dt;
            }

            try
            {
                GrdStaff.DataBind();

                // don't need paging -- already have scrolling -- and there won't be too many staff to send in one page
                GrdStaff.PagerSettings.FirstPageText = "1";
                GrdStaff.PagerSettings.LastPageText = GrdStaff.PageCount.ToString();
                GrdStaff.DataBind();
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
            GrdStaff.DataSource = dt;
            GrdStaff.DataBind();

            int TotalColumns = GrdStaff.Rows[0].Cells.Count;
            GrdStaff.Rows[0].Cells.Clear();
            GrdStaff.Rows[0].Cells.Add(new TableCell());
            GrdStaff.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdStaff.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdStaff_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {
            e.Row.Cells[0].CssClass = "hiddencol";
        }
    }
    protected void GrdStaff_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["staff_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("staff_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            Button btnSelect = (Button)e.Row.FindControl("btnSelect");
            if (btnSelect != null)
                btnSelect.OnClientClick = "javascript:select_staff('" + thisRow["staff_id"].ToString() + ":" + thisRow["firstname"].ToString() + " " + thisRow["surname"].ToString() + "');";


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdStaff_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdStaff.EditIndex = -1;
        FillStaffGrid();
    }
    protected void GrdStaff_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdStaff_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdStaff_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdStaff_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdStaff.EditIndex = e.NewEditIndex;
        FillStaffGrid();
    }
    protected void GrdStaff_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdStaff.EditIndex >= 0)
            return;

        DataTable dataTable = Session["staff_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["staff_sortExpression"] == null)
                Session["staff_sortExpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["staff_sortExpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["staff_sortExpression"] = e.SortExpression + " " + newSortExpr;

            GrdStaff.DataSource = dataView;
            GrdStaff.DataBind();
        }
    }
    protected void GrdStaff_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdStaff.PageIndex = e.NewPageIndex;
        FillStaffGrid();
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


        FillStaffGrid();
    }
    protected void btnClearSurnameSearch_Click(object sender, EventArgs e)
    {
        txtSearchSurname.Text = string.Empty;

        FillStaffGrid();
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