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

public partial class NoteEditHistoryV2 : System.Web.UI.Page
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
                Session.Remove("noteedithistory_sortexpression");
                Session.Remove("noteedithistory_data");
                FillGrid();
            }

            this.GrdNote.EnableViewState = true;

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

    private Note GetFormNote()
    {
        try
        {
            string id = Request.QueryString["id"];
            return (id == null || !Regex.IsMatch(id, @"^\d+$")) ? null : NoteDB.GetByID(Convert.ToInt32(id));
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    #region GrdNote

    protected void FillGrid()
    {
        Note note = GetFormNote();
        if (note == null)
        {
            SetErrorMessage("Invalid note id");
            return;
        }


        DataTable dt = NoteHistoryDB.GetDataTable(note.NoteID);
        Session["noteedithistory_data"] = dt;

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["noteedithistory_sortexpression"] != null && Session["noteedithistory_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["noteedithistory_sortexpression"].ToString();
                GrdNote.DataSource = dataView;
            }
            else
            {
                GrdNote.DataSource = dt;
            }


            try
            {
                GrdNote.DataBind();
                GrdNote.PagerSettings.FirstPageText = "1";
                GrdNote.PagerSettings.LastPageText = GrdNote.PageCount.ToString();
                GrdNote.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdNote.DataSource = dt;
            GrdNote.DataBind();

            int TotalColumns = GrdNote.Rows[0].Cells.Count;
            GrdNote.Rows[0].Cells.Clear();
            GrdNote.Rows[0].Cells.Add(new TableCell());
            GrdNote.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdNote.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdNote_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdNote_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }
    protected void GrdNote_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }
    protected void GrdNote_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdNote_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdNote_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdNote_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdNote.EditIndex >= 0)
            return;

        DataTable dataTable = Session["noteedithistory_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["noteedithistory_sortexpression"] == null)
                Session["noteedithistory_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["noteedithistory_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["noteedithistory_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdNote.DataSource = dataView;
            GrdNote.DataBind();
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