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

public partial class StaffOfferingsListV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, true, false, false, true);
                Session.Remove("staff_offerngs_sortexpression");
                Session.Remove("staff_offerings_data");

                SetUpGUI();
                FillGrid();
            }

            this.GrdStaffOfferings.EnableViewState = true;

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

    #endregion

    #region SetUpGUI

    protected void SetUpGUI()
    {
        ddlStaff.Items.Clear();
        ddlStaff.Items.Add(new ListItem("All Providers", "-1"));
        DataTable dtStaff = StaffDB.GetDataTable();
        for (int i = 0; i < dtStaff.Rows.Count; i++)
            if (!Convert.ToBoolean(dtStaff.Rows[i]["staff_is_fired"]) && Convert.ToBoolean(dtStaff.Rows[i]["staff_is_provider"]))
                ddlStaff.Items.Add(new ListItem(dtStaff.Rows[i]["person_firstname"].ToString() + " " + dtStaff.Rows[i]["person_surname"].ToString(), dtStaff.Rows[i]["staff_staff_id"].ToString()));

        ddlOfferings.Style["max-width"] = "375px";
        ddlOfferings.Items.Clear();
        ddlOfferings.Items.Add(new ListItem("All Offerings", "-1"));
        DataTable dtOfferings = OfferingDB.GetDataTable(false, "1,3", "63,89");
        for (int i = 0; i < dtOfferings.Rows.Count; i++)
            if (!Convert.ToBoolean(dtOfferings.Rows[i]["o_is_deleted"]))
                ddlOfferings.Items.Add(new ListItem(dtOfferings.Rows[i]["o_name"].ToString(), dtOfferings.Rows[i]["o_offering_id"].ToString()));

        if (IsValidFormStaffID())
            ddlStaff.SelectedValue = StaffDB.GetByID(GetFormStaffID()).StaffID.ToString();
        if (IsValidFormOfferingID())
            ddlOfferings.SelectedValue = OfferingDB.GetByID(GetFormOfferingID()).OfferingID.ToString();
    }

    #endregion

    #region GetFormParam, IsValidFormParam

    private bool IsValidFormStaffID()
    {
        string id = Request.QueryString["staff"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormStaffID()
    {
        if (!IsValidFormStaffID())
            throw new Exception("Invalid url staff id");

        string id = Request.QueryString["staff"];
        return Convert.ToInt32(id);
    }

    private bool IsValidFormOfferingID()
    {
        string id = Request.QueryString["offering"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormOfferingID()
    {
        if (!IsValidFormOfferingID())
            throw new Exception("Invalid url offering id");

        string id = Request.QueryString["offering"];
        return Convert.ToInt32(id);
    }

    #endregion

    #region GrdStaffOfferings

    protected void FillGrid()
    {
        //DataTable dt = IsValidFormStaffID() ?  StaffOfferingsDB.GetDataTableByStaffID(GetFormStaffID(), false) : StaffOfferingsDB.GetDataTable(false);
        DataTable dt = StaffOfferingsDB.GetDataTable(false, IsValidFormStaffID() ? GetFormStaffID() : -1, IsValidFormOfferingID() ? GetFormOfferingID() : -1);

        dt.Columns.Add("is_active", typeof(bool));
        for (int i = 0; i < dt.Rows.Count; i++)
            dt.Rows[i]["is_active"] = dt.Rows[i]["so_date_active"] == DBNull.Value || Convert.ToDateTime(dt.Rows[i]["so_date_active"]).Date > DateTime.Today ? false : true;
        Session["staff_offerings_data"] = dt;


        this.GrdStaffOfferings.AllowPaging = dt.Rows.Count > 50;

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["staff_offerngs_sortexpression"] != null && Session["staff_offerngs_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["staff_offerngs_sortexpression"].ToString();
                GrdStaffOfferings.DataSource = dataView;
            }
            else
            {
                GrdStaffOfferings.DataSource = dt;
            }


            try
            {
                GrdStaffOfferings.DataBind();
                GrdStaffOfferings.PagerSettings.FirstPageText = "1";
                GrdStaffOfferings.PagerSettings.LastPageText = GrdStaffOfferings.PageCount.ToString();
                GrdStaffOfferings.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdStaffOfferings.DataSource = dt;
            GrdStaffOfferings.DataBind();

            int TotalColumns = GrdStaffOfferings.Rows[0].Cells.Count;
            GrdStaffOfferings.Rows[0].Cells.Clear();
            GrdStaffOfferings.Rows[0].Cells.Add(new TableCell());
            GrdStaffOfferings.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdStaffOfferings.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdStaffOfferings_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";

        //if (IsValidFormStaffID())
        //    e.Row.Cells[1].CssClass = "hiddencol";

    }
    protected void GrdStaffOfferings_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["staff_offerings_data"] as DataTable;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("so_staff_offering_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

            DropDownList ddlStaff = (DropDownList)e.Row.FindControl("ddlNewStaff");
            DataTable staff   = StaffDB.GetDataTable();
            DataView dataView = new DataView(staff);
            dataView.Sort     = "person_firstname, person_surname";
            staff             = dataView.ToTable();
            for (int i = 0; i < staff.Rows.Count; i++)
            {
                Staff s = StaffDB.LoadAll(staff.Rows[i]);
                ddlStaff.Items.Add(new ListItem(s.Person.FullnameWithoutMiddlename, s.StaffID.ToString()));
            }


            DropDownList ddlOffering = (DropDownList)e.Row.FindControl("ddlNewOffering");
            string offering_invoice_type_id = null;
            if (UserView.GetInstance().IsAgedCareView)
                offering_invoice_type_id = "3,4"; // 4 = AC
            else // if (!UserView.GetInstance().IsAgedCareView)
                offering_invoice_type_id = "1,3"; // 4 = Clinic

            DataTable offerings = OfferingDB.GetDataTable(false, offering_invoice_type_id);
            ddlOffering.DataSource = offerings;
            ddlOffering.DataTextField = "o_name";
            ddlOffering.DataValueField = "o_offering_id";
            ddlOffering.DataBind();


        }
    }
    protected void GrdStaffOfferings_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdStaffOfferings.EditIndex = -1;
        FillGrid();
    }
    protected void GrdStaffOfferings_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdStaffOfferings.Rows[e.RowIndex].FindControl("lblId");
        CheckBox chkIsCommission      = (CheckBox)GrdStaffOfferings.Rows[e.RowIndex].FindControl("chkIsCommission");
        TextBox  txtCommissionPercent = (TextBox) GrdStaffOfferings.Rows[e.RowIndex].FindControl("txtCommissionPercent");
        CheckBox chkIsFixedRate       = (CheckBox)GrdStaffOfferings.Rows[e.RowIndex].FindControl("chkIsFixedRate");
        TextBox  txtFixedRate         = (TextBox)GrdStaffOfferings.Rows[e.RowIndex].FindControl("txtFixedRate");
        TextBox  txtActiveDate        = (TextBox)GrdStaffOfferings.Rows[e.RowIndex].FindControl("txtActiveDate");

        if (chkIsCommission.Checked && chkIsFixedRate.Checked)
        {
            SetErrorMessage("Can not be both comission and fixed rate. Please select only one.");
            return;
        }

        DataTable      dt             = Session["staff_offerings_data"] as DataTable;
        DataRow[]      foundRows      = dt.Select("so_staff_offering_id=" + lblId.Text);
        StaffOfferings staffOfferings = StaffOfferingsDB.LoadAll(foundRows[0]);

        StaffOfferingsDB.Update(staffOfferings.StaffOfferingID, staffOfferings.Staff.StaffID, staffOfferings.Offering.OfferingID,
            chkIsCommission.Checked,Convert.ToDecimal(txtCommissionPercent.Text),chkIsFixedRate.Checked,Convert.ToDecimal(txtFixedRate.Text),
            GetDate(txtActiveDate.Text));

        GrdStaffOfferings.EditIndex = -1;
        FillGrid();

    }
    protected void GrdStaffOfferings_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdStaffOfferings.Rows[e.RowIndex].FindControl("lblId");
        int staff_offering_id = Convert.ToInt32(lblId.Text);

        try
        {
            StaffOfferingsDB.Delete(staff_offering_id);
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
    protected void GrdStaffOfferings_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            CustomValidator txtValidateNewActiveDate = (CustomValidator)GrdStaffOfferings.FooterRow.FindControl("txtValidateNewActiveDate");
            if (!txtValidateNewActiveDate.IsValid)
                return;

            DropDownList ddlStaff         = (DropDownList)GrdStaffOfferings.FooterRow.FindControl("ddlNewStaff");
            DropDownList ddlOffering      = (DropDownList)GrdStaffOfferings.FooterRow.FindControl("ddlNewOffering");
            CheckBox chkIsCommission      = (CheckBox)GrdStaffOfferings.FooterRow.FindControl("chkNewIsCommission");
            TextBox  txtCommissionPercent = (TextBox) GrdStaffOfferings.FooterRow.FindControl("txtNewCommissionPercent");
            CheckBox chkIsFixedRate       = (CheckBox)GrdStaffOfferings.FooterRow.FindControl("chkNewIsFixedRate");
            TextBox  txtFixedRate         = (TextBox) GrdStaffOfferings.FooterRow.FindControl("txtNewFixedRate");
            TextBox  txtActiveDate        = (TextBox)GrdStaffOfferings.FooterRow.FindControl("txtNewActiveDate");


            if (chkIsCommission.Checked && chkIsFixedRate.Checked)
            {
                SetErrorMessage("Can not be both comission and fixed rate. Please select only one.");
                return;
            }

            int staff_id = IsValidFormStaffID() ? GetFormStaffID() : Convert.ToInt32(ddlStaff.SelectedValue);
            StaffOfferingsDB.Insert(staff_id, Convert.ToInt32(ddlOffering.SelectedValue),
                chkIsCommission.Checked, Convert.ToDecimal(txtCommissionPercent.Text), chkIsFixedRate.Checked, Convert.ToDecimal(txtFixedRate.Text),
                GetDate(txtActiveDate.Text));

            FillGrid();
        }
    }
    protected void GrdStaffOfferings_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdStaffOfferings.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdStaffOfferings.EditIndex >= 0)
            return;

        DataTable dataTable = Session["staff_offerings_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["staff_offerngs_sortexpression"] == null)
                Session["staff_offerngs_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["staff_offerngs_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["staff_offerngs_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdStaffOfferings.DataSource = dataView;
            GrdStaffOfferings.DataBind();
        }
    }
    protected void GrdStaffOfferings_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdStaffOfferings.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    #endregion

    #region GetDate, IsValidDate, ValidDateCheck

    public DateTime GetDate(string inDate)
    {
        inDate = inDate.Trim();

        if (inDate.Length == 0)
        {
            return DateTime.MinValue;
        }
        else
        {
            string[] dobParts = inDate.Split(new char[] { '-' });
            return new DateTime(Convert.ToInt32(dobParts[2]), Convert.ToInt32(dobParts[1]), Convert.ToInt32(dobParts[0]));
        }
    }
    public bool IsValidDate(string inDate)
    {
        inDate = inDate.Trim();
        return inDate.Length == 0 || Regex.IsMatch(inDate, @"^\d{2}\-\d{2}\-\d{4}$");
    }
    protected void ValidDateCheck(object sender, ServerValidateEventArgs e)
    {
        try
        {
            CustomValidator cv      = (CustomValidator)sender;
            GridViewRow     grdRow  = ((GridViewRow)cv.Parent.Parent);
            TextBox txtDate = grdRow.RowType == DataControlRowType.Footer ? (TextBox)grdRow.FindControl("txtNewActiveDate") : (TextBox)grdRow.FindControl("txtActiveDate");

            if (!IsValidDate(txtDate.Text))
                throw new Exception();

            DateTime d = GetDate(txtDate.Text);
            e.IsValid = true;
        }
        catch (Exception)
        {
            e.IsValid = false;
        }
    }

    #endregion

    #region ddlStaff_SelectedIndexChanged, ddlOfferings_SelectedIndexChanged

    protected void ddlStaff_SelectedIndexChanged(object sender, EventArgs e)
    {
        string url = Request.RawUrl;
        url = UrlParamModifier.Update(ddlStaff.SelectedValue != "-1", url, "staff", ddlStaff.SelectedValue);
        Response.Redirect(url);
    }
    protected void ddlOfferings_SelectedIndexChanged(object sender, EventArgs e)
    {
        string url = Request.RawUrl;
        url = UrlParamModifier.Update(ddlOfferings.SelectedValue != "-1", url, "offering", ddlOfferings.SelectedValue);
        Response.Redirect(url);
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