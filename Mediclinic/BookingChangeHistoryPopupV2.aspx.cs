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

public partial class BookingChangeHistoryPopupV2 : System.Web.UI.Page
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
                Session.Remove("bookingedithistory_sortexpression");
                Session.Remove("bookingedithistory_data");
                FillGrid();
            }

            this.GrdPatient.EnableViewState = true;

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

    private Booking GetFormBooking()
    {
        try
        {
            string id = Request.QueryString["id"];
            return (id == null || !Regex.IsMatch(id, @"^\d+$")) ? null : BookingDB.GetByID(Convert.ToInt32(id));
        }
        catch (Exception)
        {
            return null;
        }
    }


    #region GrdPatient

    protected void FillGrid()
    {
        Booking booking = GetFormBooking();
        if (booking == null)
        {
            SetErrorMessage("Invalid booking id");
            return;
        }

        lblHeadingDetail.Text = 

@"<table>
  <tr>
    <td>Date/Time</td>
    <td style=""min-width:10px;""></td>
    <td>" + booking.DateStart.ToString(@"dd MMM yyyy H:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + " - " + booking.DateEnd.ToString(@"H:mm") + (booking.DateEnd.Hour < 12 ? "am" : "pm") + @"</td>
  <tr>
  <tr>
    <td>Location</td>
    <td></td>
    <td>" + booking.Organisation.Name + @"</td>
  <tr>
"
+ (booking.Patient == null ? "" : 
@"<tr>
    <td>Patient</td>
    <td></td>
    <td>" + booking.Patient.Person.FullnameWithoutMiddlename + @"</td>
  <tr>
")

+ (booking.Offering == null ? "" : 
@"<tr>
    <td>Service</td>
    <td></td>
    <td>" + booking.Offering.Name + @"</td>
  <tr>
")
 
+ "</table>";

        DataTable dt = BookingChangeHistoryDB.GetDataTable_ByBookingID(booking.BookingID);
        Session["bookingedithistory_data"] = dt;

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["bookingedithistory_sortexpression"] != null && Session["bookingedithistory_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["bookingedithistory_sortexpression"].ToString();
                GrdPatient.DataSource = dataView;
            }
            else
            {
                GrdPatient.DataSource = dt;
            }


            try
            {
                GrdPatient.DataBind();
                GrdPatient.PagerSettings.FirstPageText = "1";
                GrdPatient.PagerSettings.LastPageText = GrdPatient.PageCount.ToString();
                GrdPatient.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdPatient.DataSource = dt;
            GrdPatient.DataBind();

            int TotalColumns = GrdPatient.Rows[0].Cells.Count;
            GrdPatient.Rows[0].Cells.Clear();
            GrdPatient.Rows[0].Cells.Add(new TableCell());
            GrdPatient.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdPatient.Rows[0].Cells[0].Text = "No Changes Made";
        }
    }
    protected void GrdPatient_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";

        if (e.Row.RowType != DataControlRowType.Pager)
        {
            foreach (DataControlField col in GrdPatient.Columns)
            {
                if (!UserView.GetInstance().IsAgedCareView)
                    if (col.HeaderText.ToLower().Trim() == "ac type")
                        e.Row.Cells[GrdPatient.Columns.IndexOf(col)].CssClass = "hiddencol";
            }
        }
    }
    protected void GrdPatient_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }
    protected void GrdPatient_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }
    protected void GrdPatient_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdPatient_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdPatient_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdPatient_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdPatient.EditIndex >= 0)
            return;

        DataTable dataTable = Session["bookingedithistory_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["bookingedithistory_sortexpression"] == null)
                Session["bookingedithistory_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["bookingedithistory_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["bookingedithistory_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdPatient.DataSource = dataView;
            GrdPatient.DataBind();
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