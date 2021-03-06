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

public partial class Notifications_PTBirthdayEmailHistoryV2 : System.Web.UI.Page
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
                Session.Remove("sortExpression_email_history");
                Session.Remove("data_email_history");
                FillGrid();
            }

            this.GrdEmailHistory.EnableViewState = true;

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


    #region GrdEmailHistory

    protected void FillGrid()
    {
        DataTable dt = EmailHistoryDataDB.GetDataTable(2, false);
        Session["data_email_history"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sortExpression_email_history"] != null && Session["sortExpression_email_history"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sortExpression_email_history"].ToString();
                GrdEmailHistory.DataSource = dataView;
            }
            else
            {
                GrdEmailHistory.DataSource = dt;
            }


            try
            {
                GrdEmailHistory.DataBind();
                GrdEmailHistory.PagerSettings.FirstPageText = "1";
                GrdEmailHistory.PagerSettings.LastPageText = GrdEmailHistory.PageCount.ToString();
                GrdEmailHistory.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdEmailHistory.DataSource = dt;
            GrdEmailHistory.DataBind();

            int TotalColumns = GrdEmailHistory.Rows[0].Cells.Count;
            GrdEmailHistory.Rows[0].Cells.Clear();
            GrdEmailHistory.Rows[0].Cells.Add(new TableCell());
            GrdEmailHistory.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdEmailHistory.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdEmailHistory_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdEmailHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
    }
    protected void GrdEmailHistory_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }
    protected void GrdEmailHistory_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdEmailHistory_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdEmailHistory_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdEmailHistory_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdEmailHistory.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["data_email_history"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression_email_history"] == null)
                Session["sortExpression_email_history"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression_email_history"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["sortExpression_email_history"] = sortExpression + " " + newSortExpr;

            GrdEmailHistory.DataSource = dataView;
            GrdEmailHistory.DataBind();
        }
    }

    protected void GrdPatient_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdEmailHistory.PageIndex = e.NewPageIndex;
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