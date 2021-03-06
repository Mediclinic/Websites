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

public partial class SiteListV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, true, false, false, true);
                Session.Remove("siteinfo_sortexpression");
                Session.Remove("siteinfo_data");
                FillGrid();
            }

            this.GrdSite.EnableViewState = true;

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

    #region GrdSite

    private bool hideFotter = false;

    protected void FillGrid()
    {
        DataTable dt = SiteDB.GetDataTable();
        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["siteinfo_sortexpression"] != null && Session["siteinfo_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["siteinfo_sortexpression"].ToString();
                GrdSite.DataSource = dataView;
            }
            else
            {
                GrdSite.DataSource = dt;
            }


            Session["siteinfo_data"] = dt;
            try
            {
                GrdSite.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdSite.DataSource = dt;
            GrdSite.DataBind();

            int TotalColumns = GrdSite.Rows[0].Cells.Count;
            GrdSite.Rows[0].Cells.Clear();
            GrdSite.Rows[0].Cells.Add(new TableCell());
            GrdSite.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdSite.Rows[0].Cells[0].Text = "No Record Found";
        }


        GrdSite.FooterRow.Visible = !hideFotter;
        btnAddSite.Visible = br_before_add_site_btn_1.Visible = br_before_add_site_btn_2.Visible = !hideFotter;
        
    }
    protected void GrdSite_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdSite_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["siteinfo_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("site_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlNumBookingMonthsToGet = (DropDownList)e.Row.FindControl("ddlNumBookingMonthsToGet");
            if (ddlNumBookingMonthsToGet != null)
            {
                for (int i = 1; i < 37; i++)
                    ddlNumBookingMonthsToGet.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlNumBookingMonthsToGet.SelectedValue = thisRow["num_booking_months_to_get"].ToString();
            }


            DropDownList ddlClinic = (DropDownList)e.Row.FindControl("ddlClinic");
            if (ddlClinic != null)
            {
                bool showClinicAddOption = true;
                bool showAgedCareAddOption = true;
                bool showGPAddOption = true;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dt.Rows[i]["site_type_id"]) == 1)
                        showClinicAddOption = false;
                    if (Convert.ToInt32(dt.Rows[i]["site_type_id"]) == 2)
                        showAgedCareAddOption = false;
                    if (Convert.ToInt32(dt.Rows[i]["site_type_id"]) == 3)
                        showGPAddOption = false;
                }
                if (Convert.ToInt32(thisRow["site_type_id"]) == 1)
                    showClinicAddOption = true;
                if (Convert.ToInt32(thisRow["site_type_id"]) == 2)
                    showAgedCareAddOption = true;
                if (Convert.ToInt32(thisRow["site_type_id"]) == 3)
                    showGPAddOption = true;

                ddlClinic.Items.Clear();
                if (showClinicAddOption)
                    ddlClinic.Items.Add(new ListItem("Clinic", "1"));
                if (showAgedCareAddOption)
                    ddlClinic.Items.Add(new ListItem("Aged Care", "2"));
                if (showGPAddOption)
                    ddlClinic.Items.Add(new ListItem("GP", "3"));

                ddlClinic.SelectedValue = Convert.ToInt32(thisRow["site_type_id"]).ToString();
            }


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlNumBookingMonthsToGet = (DropDownList)e.Row.FindControl("ddlNewNumBookingMonthsToGet");
            for (int i = 1; i < 37; i++)
                ddlNumBookingMonthsToGet.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlNumBookingMonthsToGet.SelectedValue = "9";

            TextBox txtFiscalYrEnd = (TextBox)e.Row.FindControl("txtNewFiscalYrEnd");
            txtFiscalYrEnd.Text = (new DateTime((DateTime.Now.Month >= 7) ? DateTime.Now.Year + 1 : DateTime.Now.Year, 6, 30)).ToString("dd-MM-yyyy");


            bool showClinicAddOption   = Convert.ToInt32(SystemVariableDB.GetByDescr("AllowAddSiteClinic").Value)   == 1;
            bool showAgedCareAddOption = Convert.ToInt32(SystemVariableDB.GetByDescr("AllowAddSiteAgedCare").Value) == 1;
            bool showGPAddOption       = Convert.ToInt32(SystemVariableDB.GetByDescr("AllowAddSiteGP").Value) == 1;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToInt32(dt.Rows[i]["site_type_id"]) == 1)
                    showClinicAddOption = false;
                if (Convert.ToInt32(dt.Rows[i]["site_type_id"]) == 2)
                    showAgedCareAddOption = false;
            }
            DropDownList ddlClinic = (DropDownList) e.Row.FindControl("ddlNewClinic");
            ddlClinic.Items.Clear();
            hideFotter = true;
            if (showClinicAddOption)
            {
                ddlClinic.Items.Add(new ListItem("Clinic", "1"));
                hideFotter = false;
            }
            if (showAgedCareAddOption)
            {
                ddlClinic.Items.Add(new ListItem("Aged Care", "2"));
                hideFotter = false;
            }
            if (showAgedCareAddOption)
            {
                ddlClinic.Items.Add(new ListItem("GP", "3"));
                hideFotter = false;
            }
        }
    }
    protected void GrdSite_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdSite.EditIndex = -1;
        FillGrid();
    }
    protected void GrdSite_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId                         = (Label)GrdSite.Rows[e.RowIndex].FindControl("lblId");
        TextBox      txtName                       = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtName");
        DropDownList ddlClinic                     = (DropDownList)GrdSite.Rows[e.RowIndex].FindControl("ddlClinic");
        TextBox      txtABN                        = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtABN");
        TextBox      txtACN                        = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtACN");
        TextBox      txtTFN                        = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtTFN");
        TextBox      txtASIC                       = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtASIC");
        CheckBox     chkIsProvider                 = (CheckBox)GrdSite.Rows[e.RowIndex].FindControl("chkIsProvider");
        TextBox      txtBPay                       = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtBPay");
        TextBox      txtBSB                        = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtBSB");
        TextBox      txtBankAccount                = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtBankAccount");
        TextBox      txtBankDirectDebitUserID      = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtBankDirectDebitUserID");
        TextBox      txtBankUsername               = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtBankUsername");
        TextBox      txtOustandingBalanceWarning   = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtOustandingBalanceWarning");
        CheckBox     chkIsPrintEPC                 = (CheckBox)GrdSite.Rows[e.RowIndex].FindControl("chkIsPrintEPC");
        DropDownList ddlNumBookingMonthsToGet      = (DropDownList)GrdSite.Rows[e.RowIndex].FindControl("ddlNumBookingMonthsToGet");
        TextBox      txtFiscalYrEnd                = (TextBox)GrdSite.Rows[e.RowIndex].FindControl("txtFiscalYrEnd");


        string[] fyeParts = txtFiscalYrEnd.Text.Trim().Split(new char[] { '-' });
        DateTime fye = new DateTime(2000, Convert.ToInt32(fyeParts[1]), Convert.ToInt32(fyeParts[0]),23,59,59);

        SiteDB.Update(Convert.ToInt32(lblId.Text), Utilities.FormatName(txtName.Text), Convert.ToInt32(ddlClinic.SelectedValue), txtABN.Text, txtACN.Text, txtTFN.Text, txtASIC.Text,
                        chkIsProvider.Checked, txtBPay.Text, txtBSB.Text, txtBankAccount.Text, txtBankDirectDebitUserID.Text, txtBankUsername.Text,
                        Convert.ToDecimal(txtOustandingBalanceWarning.Text), chkIsPrintEPC.Checked, false, false, false, false, false, false, false,
                        new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0), new TimeSpan(18, 0, 0), fye, Convert.ToInt32(ddlNumBookingMonthsToGet.SelectedValue));

        GrdSite.EditIndex = -1;
        FillGrid();
    }
    protected void GrdSite_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdSite.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //SiteDB.Delete(Convert.ToInt32(lblId.Text));
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
    protected void GrdSite_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            TextBox      txtName                       = (TextBox)GrdSite.FooterRow.FindControl("txtNewName");
            DropDownList ddlClinic                     = (DropDownList)GrdSite.FooterRow.FindControl("ddlNewClinic");
            TextBox      txtABN                        = (TextBox)GrdSite.FooterRow.FindControl("txtNewABN");
            TextBox      txtACN                        = (TextBox)GrdSite.FooterRow.FindControl("txtNewACN");
            TextBox      txtTFN                        = (TextBox)GrdSite.FooterRow.FindControl("txtNewTFN");
            TextBox      txtASIC                       = (TextBox)GrdSite.FooterRow.FindControl("txtNewASIC");
            CheckBox     chkIsProvider                 = (CheckBox)GrdSite.FooterRow.FindControl("chkNewIsProvider");
            TextBox      txtBPay                       = (TextBox)GrdSite.FooterRow.FindControl("txtNewBPay");
            TextBox      txtBSB                        = (TextBox)GrdSite.FooterRow.FindControl("txtNewBSB");
            TextBox      txtBankAccount                = (TextBox)GrdSite.FooterRow.FindControl("txtNewBankAccount");
            TextBox      txtBankDirectDebitUserID      = (TextBox)GrdSite.FooterRow.FindControl("txtNewBankDirectDebitUserID");
            TextBox      txtBankUsername               = (TextBox)GrdSite.FooterRow.FindControl("txtNewBankUsername");
            TextBox      txtOustandingBalanceWarning   = (TextBox)GrdSite.FooterRow.FindControl("txtNewOustandingBalanceWarning");
            CheckBox     chkIsPrintEPC                 = (CheckBox)GrdSite.FooterRow.FindControl("chkNewIsPrintEPC");
            DropDownList ddlNumBookingMonthsToGet      = (DropDownList)GrdSite.FooterRow.FindControl("ddlNewNumBookingMonthsToGet");
            TextBox      txtFiscalYrEnd                = (TextBox)GrdSite.FooterRow.FindControl("txtNewFiscalYrEnd");


            string[] fyeParts = txtFiscalYrEnd.Text.Trim().Split(new char[] { '-' });
            DateTime fye = new DateTime(2000, Convert.ToInt32(fyeParts[1]), Convert.ToInt32(fyeParts[0]),23,59,59);

            SiteDB.Insert(Utilities.FormatName(txtName.Text), Convert.ToInt32(ddlClinic.SelectedValue), txtABN.Text, txtACN.Text, txtTFN.Text, txtASIC.Text,
                            chkIsProvider.Checked, txtBPay.Text, txtBSB.Text, txtBankAccount.Text, txtBankDirectDebitUserID.Text, txtBankUsername.Text,
                            Convert.ToDecimal(txtOustandingBalanceWarning.Text), chkIsPrintEPC.Checked, false, false, false, false, false, false, false,
                            new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0), new TimeSpan(18, 0, 0), fye, Convert.ToInt32(ddlNumBookingMonthsToGet.SelectedValue));

            FillGrid();
        }
    }
    protected void GrdSite_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdSite.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdSite.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["siteinfo_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["siteinfo_sortexpression"] == null)
                Session["siteinfo_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["siteinfo_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["siteinfo_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdSite.DataSource = dataView;
            GrdSite.DataBind();
        }
    }

    #endregion

    protected void ValidDateCheck(object sender, ServerValidateEventArgs e)
    {
        try
        {
            CustomValidator cv = (CustomValidator)sender;
            GridViewRow grdRow = ((GridViewRow)cv.Parent.Parent);
            TextBox txtDate = grdRow.RowType == DataControlRowType.Footer ? (TextBox)grdRow.FindControl("txtNewFiscalYrEnd") : (TextBox)grdRow.FindControl("txtFiscalYrEnd");

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