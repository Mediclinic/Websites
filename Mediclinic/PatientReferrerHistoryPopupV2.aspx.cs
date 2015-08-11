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

public partial class PatientReferrerHistoryPopupV2 : System.Web.UI.Page
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
                Session.Remove("patientreferrerhistory_sortexpression");
                Session.Remove("patientreferrerhistory_data");
                FillGrid();
            }

            this.GrdPatientReferrer.EnableViewState = true;

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

    private Patient GetFormPatient()
    {
        try
        {
            string id = Request.QueryString["id"];
            return (id == null || !Regex.IsMatch(id, @"^\d+$")) ? null : PatientDB.GetByID(Convert.ToInt32(id));
        }
        catch (Exception)
        {
            return null;
        }
    }


    #region GrdPatientReferrer

    protected void FillGrid()
    {
        Patient patient = GetFormPatient();
        if (patient == null)
        {
            SetErrorMessage("Invalid patient id");
            return;
        }


        DataTable dt = PatientReferrerDB.GetDataTable_EPCReferrersOf(patient.PatientID);
        Session["patientreferrerhistory_data"] = dt;

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["patientreferrerhistory_sortexpression"] != null && Session["patientreferrerhistory_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["patientreferrerhistory_sortexpression"].ToString();
                GrdPatientReferrer.DataSource = dataView;
            }
            else
            {
                GrdPatientReferrer.DataSource = dt;
            }


            try
            {
                GrdPatientReferrer.DataBind();
                GrdPatientReferrer.PagerSettings.FirstPageText = "1";
                GrdPatientReferrer.PagerSettings.LastPageText = GrdPatientReferrer.PageCount.ToString();
                GrdPatientReferrer.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdPatientReferrer.DataSource = dt;
            GrdPatientReferrer.DataBind();

            int TotalColumns = GrdPatientReferrer.Rows[0].Cells.Count;
            GrdPatientReferrer.Rows[0].Cells.Clear();
            GrdPatientReferrer.Rows[0].Cells.Add(new TableCell());
            GrdPatientReferrer.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdPatientReferrer.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdPatientReferrer_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdPatientReferrer_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }
    protected void GrdPatientReferrer_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }
    protected void GrdPatientReferrer_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdPatientReferrer_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdPatientReferrer_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdPatientReferrer_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdPatientReferrer.EditIndex >= 0)
            return;

        DataTable dataTable = Session["patientreferrerhistory_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["patientreferrerhistory_sortexpression"] == null)
                Session["patientreferrerhistory_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["patientreferrerhistory_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["patientreferrerhistory_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdPatientReferrer.DataSource = dataView;
            GrdPatientReferrer.DataBind();
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