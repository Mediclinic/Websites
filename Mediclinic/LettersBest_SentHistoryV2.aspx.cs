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

public partial class LettersBest_SentHistoryV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, false);
                Utilities.SetNoCache(Response);
            }
            HideErrorMessage();

            if (!IsPostBack)
            {
                Session.Remove("letterbestprinthistory_sortexpression");
                Session.Remove("letterbestprinthistory_data");

                FillGrid();
            }

            this.GrdLetterPrintHistory.EnableViewState = true;

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

    #region GetFormParam

    protected bool IsValidFormPatient()
    {
        string patient_id = Request.QueryString["patient"];
        return patient_id != null && Regex.IsMatch(patient_id, @"^\d+$") && PatientDB.Exists(Convert.ToInt32(patient_id));
    }
    protected int GetFormPatient(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormPatient())
            throw new Exception("Invalid url patient");
        return Convert.ToInt32(Request.QueryString["patient"]);
    }

    #endregion

    #region GrdLetterPrintHistory

    protected void FillGrid()
    {
        int patient_id  = IsValidFormPatient() ? GetFormPatient(false) : -1;
        if (patient_id != -1)
        {
            Patient patient = PatientDB.GetByID(patient_id);
            Page.Title = lblHeading.Text = "B.E.S.T. Letter Print History For ";
            lnkToEntity.Text = patient.Person.FullnameWithoutMiddlename;
            lnkToEntity.NavigateUrl = "PatientDetailV2.aspx?type=view&id=" + patient.PatientID;
        }


        DataTable dt = LetterBestPrintHistoryDB.GetDataTable(patient_id);
        Session["letterbestprinthistory_data"] = dt;

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["letterbestprinthistory_sortexpression"] != null && Session["letterbestprinthistory_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["letterbestprinthistory_sortexpression"].ToString();
                GrdLetterPrintHistory.DataSource = dataView;
            }
            else
            {
                GrdLetterPrintHistory.DataSource = dt;
            }


            try
            {
                GrdLetterPrintHistory.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdLetterPrintHistory.DataSource = dt;
            GrdLetterPrintHistory.DataBind();

            int TotalColumns = GrdLetterPrintHistory.Rows[0].Cells.Count;
            GrdLetterPrintHistory.Rows[0].Cells.Clear();
            GrdLetterPrintHistory.Rows[0].Cells.Add(new TableCell());
            GrdLetterPrintHistory.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdLetterPrintHistory.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdLetterPrintHistory_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdLetterPrintHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["letterbestprinthistory_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            //DataRow[] foundRows = dt.Select("lph_letter_print_history_id=" + lblId.Text);
            //DataRow thisRow = foundRows[0];

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdLetterPrintHistory_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdLetterPrintHistory.EditIndex = -1;
        FillGrid();
    }
    protected void GrdLetterPrintHistory_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdLetterPrintHistory_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdLetterPrintHistory_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdLetterPrintHistory_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdLetterPrintHistory.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdLetterPrintHistory.EditIndex >= 0)
            return;

        DataTable dataTable = Session["letterbestprinthistory_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["letterbestprinthistory_sortexpression"] == null)
                Session["letterbestprinthistory_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["letterbestprinthistory_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["letterbestprinthistory_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdLetterPrintHistory.DataSource = dataView;
            GrdLetterPrintHistory.DataBind();
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