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

public partial class StockListV2 : System.Web.UI.Page
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
                Session.Remove("registerstocktoorg_sortexpression");
                Session.Remove("registerstocktoorg_data");
                FillGrid();
                FillGridUpdateHistory();
                SetNotificationInfo();
                hiddenUpdateHistoryShowing.Value = "False";
                SetOrgsDDL();
            }

            div_show_hide_stock_update_history.Style["display"] = Convert.ToBoolean(hiddenUpdateHistoryShowing.Value) ? "block" : "none";

            this.GrdRegistration.EnableViewState = true;
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


    private bool IsValidFormID()
    {
        string raw_id = Request.QueryString["id"];
        if (raw_id == null)
            return false;

        return Regex.IsMatch(raw_id, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new Exception("Invalid ID");
        return Convert.ToInt32(Request.QueryString["id"]);
    }


    #region GrdRegistration

    private bool hideFotter = false;

    protected void FillGrid()
    {
        Organisation org = null;
        if (IsValidFormID())
            org = OrganisationDB.GetByID(GetFormID());


        DataTable dt = org == null ? dt = StockDB.GetDataTable(!UserView.GetInstance().IsAgedCareView ? 5 : 6) : dt = StockDB.GetDataTable_ByOrg(org.OrganisationID);
        Session["registerstocktoorg_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["registerstocktoorg_sortexpression"] != null && Session["registerstocktoorg_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["registerstocktoorg_sortexpression"].ToString();
                GrdRegistration.DataSource = dataView;
            }
            else
            {
                GrdRegistration.DataSource = dt;
            } 
            
            
            try
            {
                GrdRegistration.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdRegistration.DataSource = dt;
            GrdRegistration.DataBind();

            int TotalColumns = GrdRegistration.Rows[0].Cells.Count;
            GrdRegistration.Rows[0].Cells.Clear();
            GrdRegistration.Rows[0].Cells.Add(new TableCell());
            GrdRegistration.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdRegistration.Rows[0].Cells[0].Text = "No Record Found";
        }

        if (org == null || hideFotter)
            GrdRegistration.FooterRow.Visible = false;
    }
    protected void GrdRegistration_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdRegistration_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Organisation org = null;
        if (IsValidFormID())
            org = OrganisationDB.GetByID(GetFormID());

        DataTable dt = Session["registerstocktoorg_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("s_stock_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlOffering = (DropDownList)e.Row.FindControl("ddlOffering");
            if (ddlOffering != null)
            {
                Offering[] incList_orig = StockDB.GetOfferingsByOrg(Convert.ToInt32(thisRow["organisation_id"]));
                Offering[] incList = Offering.RemoveByID(incList_orig, Convert.ToInt32(thisRow["o_offering_id"]));
                DataTable offering = OfferingDB.GetDataTable_AllNotInc(incList);
                offering.DefaultView.Sort = "o_name ASC";
                foreach (DataRowView row in offering.DefaultView)
                {
                    if (Convert.ToInt32(row["o_offering_id"]) == Convert.ToInt32(thisRow["o_offering_id"]) || Convert.ToInt32(row["o_offering_type_id"]) == 89)  // only products, or other if was product when added but now not a product
                        ddlOffering.Items.Add(new ListItem(row["o_name"].ToString(), row["o_offering_id"].ToString()));
                }
                ddlOffering.SelectedValue = thisRow["o_offering_id"].ToString();
            }

            DropDownList ddlQuantity_Zeros    = (DropDownList)e.Row.FindControl("ddlQuantity_Zeros");
            DropDownList ddlQuantity_Tens     = (DropDownList)e.Row.FindControl("ddlQuantity_Tens");
            DropDownList ddlQuantity_Hundreds = (DropDownList)e.Row.FindControl("ddlQuantity_Hundreds");
            if (ddlQuantity_Zeros != null && ddlQuantity_Tens != null && ddlQuantity_Hundreds != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    ddlQuantity_Zeros.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlQuantity_Tens.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlQuantity_Hundreds.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
                int qty = Convert.ToInt32(thisRow["s_qty"]);
                ddlQuantity_Zeros.SelectedValue = ((qty % 10)/1).ToString();
                ddlQuantity_Tens.SelectedValue = ((qty % 100)/10).ToString();
                ddlQuantity_Hundreds.SelectedValue = ((qty % 1000)/100).ToString();
            }

            DropDownList ddlWarningAmount = (DropDownList)e.Row.FindControl("ddlWarningAmount");
            if (ddlWarningAmount != null)
            {
                ddlWarningAmount.Items.Add(new ListItem("No Warning", "-1"));
                for (int i = 0; i <= 50; i++)
                    ddlWarningAmount.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlWarningAmount.SelectedValue = thisRow["s_warning_amt"].ToString();
            }

            Label lblWarningAmount = (Label)e.Row.FindControl("lblWarningAmount");
            if (lblWarningAmount != null)
            {
                if (lblWarningAmount.Text == "-1")
                    lblWarningAmount.Text = "No Warning";
            }


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (org != null && e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblOrganisation = (Label)e.Row.FindControl("lblNewOrganisation");
            if (lblOrganisation != null)
                lblOrganisation.Text = org.Name;

            DropDownList ddlOffering = (DropDownList)e.Row.FindControl("ddlNewOffering");
            if (ddlOffering != null)
            {
                Offering[] incList = StockDB.GetOfferingsByOrg(org.OrganisationID);
                DataTable offering = OfferingDB.GetDataTable_AllNotInc(incList);
                offering.DefaultView.Sort = "o_name ASC";
                foreach (DataRowView row in offering.DefaultView)
                {
                    if (Convert.ToInt32(row["o_offering_type_id"]) == 89)  // only products
                        ddlOffering.Items.Add(new ListItem(row["o_name"].ToString(), row["o_offering_id"].ToString()));
                }

                if (offering.Rows.Count == 0)
                    hideFotter = true;
            }

            DropDownList ddlQuantity_Zeros    = (DropDownList)e.Row.FindControl("ddlNewQuantity_Zeros");
            DropDownList ddlQuantity_Tens     = (DropDownList)e.Row.FindControl("ddlNewQuantity_Tens");
            DropDownList ddlQuantity_Hundreds = (DropDownList)e.Row.FindControl("ddlNewQuantity_Hundreds");
            if (ddlQuantity_Zeros != null && ddlQuantity_Tens != null && ddlQuantity_Hundreds != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    ddlQuantity_Zeros.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlQuantity_Tens.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlQuantity_Hundreds.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
            }

            DropDownList ddlWarningAmount = (DropDownList)e.Row.FindControl("ddlNewWarningAmount");
            if (ddlWarningAmount != null)
            {
                ddlWarningAmount.Items.Add(new ListItem("No Warning", "-1"));
                for (int i = 0; i <= 50; i++)
                    ddlWarningAmount.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }

        }
    }
    protected void GrdRegistration_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdRegistration.EditIndex = -1;
        FillGrid();
        FillGridUpdateHistory();
    }
    protected void GrdRegistration_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId                = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlOffering          = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlOffering");
        DropDownList ddlWarningAmount     = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlWarningAmount");
        DropDownList ddlQuantity_Zeros    = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlQuantity_Zeros");
        DropDownList ddlQuantity_Tens     = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlQuantity_Tens");
        DropDownList ddlQuantity_Hundreds = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlQuantity_Hundreds");


        Stock stock = StockDB.GetByID(Convert.ToInt32(lblId.Text));
        if (stock != null)
        {
            int qty = 100 * Convert.ToInt32(ddlQuantity_Hundreds.SelectedValue) + 10 * Convert.ToInt32(ddlQuantity_Tens.SelectedValue) + Convert.ToInt32(ddlQuantity_Zeros.SelectedValue);
            StockDB.Update(stock.StockID, stock.Organisation.OrganisationID, stock.Offering.OfferingID, qty, Convert.ToInt32(ddlWarningAmount.SelectedValue));
            if (qty != stock.Quantity)
                StockUpdateHistoryDB.Insert(stock.Organisation.OrganisationID, stock.Offering.OfferingID, qty - stock.Quantity, false, false, Convert.ToInt32(Session["StaffID"]));
        }

        GrdRegistration.EditIndex = -1;
        FillGrid();
        FillGridUpdateHistory();
    }
    protected void GrdRegistration_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            Stock stock = StockDB.GetByID(Convert.ToInt32(lblId.Text));
            if (stock != null)
            {
                StockDB.Delete(Convert.ToInt32(lblId.Text));
                StockUpdateHistoryDB.Insert(stock.Organisation.OrganisationID, stock.Offering.OfferingID, -1 * stock.Quantity, false, true, Convert.ToInt32(Session["StaffID"]));
            }
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                HideTableAndSetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                HideTableAndSetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrid();
        FillGridUpdateHistory();
    }
    protected void GrdRegistration_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            Organisation org = OrganisationDB.GetByID(GetFormID());
            if (org == null)
            {
                HideTableAndSetErrorMessage("");
                return;
            }

            DropDownList ddlOffering          = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewOffering");
            DropDownList ddlWarningAmount     = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewWarningAmount");
            DropDownList ddlQuantity_Zeros    = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewQuantity_Zeros");
            DropDownList ddlQuantity_Tens     = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewQuantity_Tens");
            DropDownList ddlQuantity_Hundreds = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewQuantity_Hundreds");

            try
            {
                Stock stock = StockDB.GetOfferingByOrgAndOffering(org.OrganisationID, Convert.ToInt32(ddlOffering.SelectedValue));
                if (stock == null)
                {
                    int qty = 100 * Convert.ToInt32(ddlQuantity_Hundreds.SelectedValue) + 10 * Convert.ToInt32(ddlQuantity_Tens.SelectedValue) + Convert.ToInt32(ddlQuantity_Zeros.SelectedValue);
                    StockDB.Insert(org.OrganisationID, Convert.ToInt32(ddlOffering.SelectedValue), qty, Convert.ToInt32(ddlWarningAmount.SelectedValue));
                    StockUpdateHistoryDB.Insert(org.OrganisationID, Convert.ToInt32(ddlOffering.SelectedValue), qty, true, false, Convert.ToInt32(Session["StaffID"]));
                }
            }
            catch (UniqueConstraintException) 
            {
                // happens when 2 forms allow adding - do nothing and let form re-update
                ;
            }
            FillGrid();
            FillGridUpdateHistory();
        }
    }
    protected void GrdRegistration_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdRegistration.EditIndex = e.NewEditIndex;
        FillGrid();
        FillGridUpdateHistory();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdRegistration.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }
    protected void GrdRegistration_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdRegistration.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["registerstocktoorg_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["registerstocktoorg_sortexpression"] == null)
                Session["registerstocktoorg_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["registerstocktoorg_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["registerstocktoorg_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdRegistration.DataSource = dataView;
            GrdRegistration.DataBind();
        }
    }

    #endregion

    #region GrdUpdateHistory

    protected void FillGridUpdateHistory()
    {
        Organisation org = null;
        if (IsValidFormID())
            org = OrganisationDB.GetByID(GetFormID());

        DataTable dt = org == null ? StockUpdateHistoryDB.GetDataTable() : StockUpdateHistoryDB.GetDataTable_ByOrg(org.OrganisationID);
        Session["registerstockupdatehistory_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["registerstockupdatehistory_sortexpression"] != null && Session["registerstockupdatehistory_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["registerstockupdatehistory_sortexpression"].ToString();
                GrdUpdateHistory.DataSource = dataView;
            }
            else
            {
                GrdUpdateHistory.DataSource = dt;
            }


            try
            {
                GrdUpdateHistory.DataBind();
                GrdUpdateHistory.PagerSettings.FirstPageText = "1";
                GrdUpdateHistory.PagerSettings.LastPageText = GrdUpdateHistory.PageCount.ToString();
                GrdUpdateHistory.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdUpdateHistory.DataSource = dt;
            GrdUpdateHistory.DataBind();

            int TotalColumns = GrdUpdateHistory.Rows[0].Cells.Count;
            GrdUpdateHistory.Rows[0].Cells.Clear();
            GrdUpdateHistory.Rows[0].Cells.Add(new TableCell());
            GrdUpdateHistory.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdUpdateHistory.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdUpdateHistory_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdUpdateHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["registerstockupdatehistory_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            //Label lblId = (Label)e.Row.FindControl("lblId");
            //DataRow[] foundRows = dt.Select("sa_stock_update_history_id=" + lblId.Text);
            //DataRow thisRow = foundRows[0];

        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

        }
    }
    protected void GrdUpdateHistory_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdUpdateHistory.EditIndex = -1;
        FillGridUpdateHistory();
    }
    protected void GrdUpdateHistory_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdUpdateHistory_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdUpdateHistory_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdUpdateHistory_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GridView_Sorting_UpdateHistory(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdUpdateHistory.EditIndex >= 0)
            return;

        SortUpdateHistory(e.SortExpression);
    }
    protected void GrdUpdateHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdUpdateHistory.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void SortUpdateHistory(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["registerstockupdatehistory_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["registerstockupdatehistory_sortexpression"] == null)
                Session["registerstockupdatehistory_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["registerstockupdatehistory_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["registerstockupdatehistory_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdUpdateHistory.DataSource = dataView;
            GrdUpdateHistory.DataBind();
        }
    }

    #endregion

    #region SetNotificationInfo, btnUpdateNotificationInfo_Click, btnRevertNotificationInfo_Click

    protected void btnUpdateNotificationInfo_Click(object sender, EventArgs e)
    {
        txtStockWarningLevelNotificationEmailAddress.Text = txtStockWarningLevelNotificationEmailAddress.Text.Trim();

        if (txtStockWarningLevelNotificationEmailAddress.Text.Length > 0 && !Utilities.IsValidEmailAddress(txtStockWarningLevelNotificationEmailAddress.Text))
        {
            SetErrorMessage("Invalid email address. It must be blank or a valid email address.");
            return;
        }

        SystemVariableDB.Update("StockWarningNotificationEmailAddress", txtStockWarningLevelNotificationEmailAddress.Text);
    }

    protected void btnRevertNotificationInfo_Click(object sender, EventArgs e)
    {
        SetNotificationInfo();
    }

    protected void SetNotificationInfo()
    {
        txtStockWarningLevelNotificationEmailAddress.Text = SystemVariableDB.GetByDescr("StockWarningNotificationEmailAddress").Value;
        btnUpdateNotificationInfo.CssClass = "hiddencol";
        btnRevertNotificationInfo.CssClass = "hiddencol";
    }

    #endregion

    #region SetOrgsDDL, ddlOrgs_SelectedIndexChanged

    protected void SetOrgsDDL()
    {
        DataTable dt = null;
        string type = null;
        if (!UserView.GetInstance().IsAgedCareView)
        {
            dt = OrganisationDB.GetDataTable_Clinics();
            type = "Clinics";
            lblOrgType.Text = "Clinic";
        }
        else
        {
            dt = OrganisationDB.GetDataTable_AgedCareFacs();
            type = "Facilities";
            lblOrgType.Text = "Facility";
        }

        ddlOrgs.Items.Clear();
        ddlOrgs.Items.Add(new ListItem("All " + type, "0"));
        for(int i=0; i<dt.Rows.Count; i++)
            ddlOrgs.Items.Add(new ListItem(dt.Rows[i]["name"].ToString(), dt.Rows[i]["organisation_id"].ToString()));

        if (IsValidFormID())
        {
            ddlOrgs.SelectedValue = GetFormID().ToString();
            lblHowToAddItems.Visible = false;
        }
    }

    protected void ddlOrgs_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlOrgs.SelectedValue == "0")
            Response.Redirect(UrlParamModifier.Remove(Request.RawUrl, "id"));
        else
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "id", ddlOrgs.SelectedValue));
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        GrdRegistration.Visible = false;
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