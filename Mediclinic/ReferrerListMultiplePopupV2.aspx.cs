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

public partial class ReferrerListMultiplePopupV2 : System.Web.UI.Page
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
                Session.Remove("referrerlistmultiplepopup_sortexpression");
                Session.Remove("referrerlistmultiplepopup_data");
                FillReferrerGrid();
                txtSearchFullName.Focus();
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

    #region GetUrlParams

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

    #region GrdReferrer

    protected void FillReferrerGrid()
    {
        DataTable dt = RegisterReferrerDB.GetDataTable(0, -1, false, new int[] { 191 }, txtSearchSurname.Text.Trim(), chkSurnameSearchOnlyStartWith.Checked, txtSearchSuburb.Text, txtSearchPostcode.Text);
        Session["referrerlistmultiplepopup_data"] = dt;

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["referrerlistmultiplepopup_sortexpression"] != null && Session["referrerlistmultiplepopup_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["referrerlistmultiplepopup_sortexpression"].ToString();
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
                this.lblErrorMessage.Visible = true;
                this.lblErrorMessage.Text = ex.ToString();
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
        {
            e.Row.Cells[0].CssClass = "hiddencol";
        }
    }
    protected void GrdReferrer_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["referrerlistmultiplepopup_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("register_referrer_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            Button btnSelect = (Button)e.Row.FindControl("btnSelect");
            if (btnSelect != null)
                btnSelect.OnClientClick = "javascript:select_referrer('" + thisRow["register_referrer_id"].ToString() + "');";


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdReferrer_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdReferrer.EditIndex = -1;
        FillReferrerGrid();
    }
    protected void GrdReferrer_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdReferrer_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdReferrer_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdReferrer_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdReferrer.EditIndex = e.NewEditIndex;
        FillReferrerGrid();
    }
    protected void GrdReferrer_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdReferrer.EditIndex >= 0)
            return;

        DataTable dataTable = Session["referrerlistmultiplepopup_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["referrerlistmultiplepopup_sortexpression"] == null)
                Session["referrerlistmultiplepopup_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["referrerlistmultiplepopup_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["referrerlistmultiplepopup_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdReferrer.DataSource = dataView;
            GrdReferrer.DataBind();
        }
    }
    protected void GrdReferrer_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdReferrer.PageIndex = e.NewPageIndex;
        FillReferrerGrid();
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


        FillReferrerGrid();
    }
    protected void btnClearSurnameSearch_Click(object sender, EventArgs e)
    {
        txtSearchSurname.Text = string.Empty;

        FillReferrerGrid();
    }

    protected void btnSearchSuburb_Click(object sender, EventArgs e)
    {
        if (!Regex.IsMatch(txtSearchSuburb.Text, @"^[a-zA-Z\-\']*$"))
        {
            SetErrorMessage("Search text can only be letters and hyphens");
            return;
        }
        else if (txtSearchSuburb.Text.Trim().Length == 0)
        {
            SetErrorMessage("No search text entered");
            return;
        }
        else
            HideErrorMessage();


        FillReferrerGrid();
    }
    protected void btnClearSuburbSearch_Click(object sender, EventArgs e)
    {
        txtSearchSuburb.Text = string.Empty;

        FillReferrerGrid();
    }

    protected void btnSearchPostcode_Click(object sender, EventArgs e)
    {
        if (!Regex.IsMatch(txtSearchPostcode.Text, @"^[0-9]*$"))
        {
            SetErrorMessage("Search text can only be numbers");
            return;
        }
        else if (txtSearchPostcode.Text.Trim().Length == 0)
        {
            SetErrorMessage("No search text entered");
            return;
        }
        else
            HideErrorMessage();


        FillReferrerGrid();
    }
    protected void btnClearPostcodeSearch_Click(object sender, EventArgs e)
    {
        txtSearchPostcode.Text = string.Empty;

        FillReferrerGrid();
    }

    #endregion

    #region btnUpdateReferrersList_Click

    protected void btnUpdateReferrersList_Click(object sender, EventArgs e)
    {
        FillReferrerGrid();
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        GrdReferrer.Visible = false;
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