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

public partial class StaffListExternalV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, false, false, false, false);
                Session.Remove("externalstaffinfo_sortexpression");
                Session.Remove("externalstaffinfo_data");

                FillGrid();
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

    #endregion

    #region GrdStaff

    protected void FillGrid()
    {

        string searchSurname = "";
        if (Request.QueryString["surname_search"] != null && Request.QueryString["surname_search"].Length > 0)
        {
            searchSurname = Request.QueryString["surname_search"];
            txtSearchSurname.Text = Request.QueryString["surname_search"];
        }
        bool searchSurnameOnlyStartsWith = true;
        if (Request.QueryString["surname_starts_with"] != null && Request.QueryString["surname_starts_with"].Length > 0)
        {
            searchSurnameOnlyStartsWith = Request.QueryString["surname_starts_with"] == "0" ? false : true;
            chkSurnameSearchOnlyStartWith.Checked = searchSurnameOnlyStartsWith;
        }
        else
        {
            chkSurnameSearchOnlyStartWith.Checked = searchSurnameOnlyStartsWith;
        }



        DataTable dt = StaffDB.GetDataTable_StaffInfo(Convert.ToBoolean(Session["IsStakeholder"]), chkShowFired.Checked, true, true, searchSurname, searchSurnameOnlyStartsWith);

        // remove "Call Center" staff member
        for(int i=dt.Rows.Count-1; i>=0; i--)
            if (((int)dt.Rows[i]["staff_id"]) == -5)
                dt.Rows.RemoveAt(i);

        dt.DefaultView.Sort = "firstname, surname, middlename";
        dt = dt.DefaultView.ToTable();
        Session["externalstaffinfo_data"] = dt;


        this.GrdStaff.AllowPaging = false;
        /*
        if (!IsPostBack)
            chkUsePaging.Checked = dt.Rows.Count > 50;
        this.GrdStaff.AllowPaging = chkUsePaging.Checked;
        */

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["externalstaffinfo_sortexpression"] != null && Session["externalstaffinfo_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["externalstaffinfo_sortexpression"].ToString();
                GrdStaff.DataSource = dataView;
            }
            else
            {
                GrdStaff.DataSource = dt;
            }

            try
            {
                GrdStaff.DataBind();
                GrdStaff.PagerSettings.FirstPageText = "1";
                GrdStaff.PagerSettings.LastPageText = GrdStaff.PageCount.ToString();
                GrdStaff.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
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
            e.Row.Cells[0].CssClass = "hiddencol";

        if (e.Row.RowType != DataControlRowType.Pager)
        {
            foreach (DataControlField col in GrdStaff.Columns)
            {
                if (col.SortExpression == "is_stakeholder" && !Convert.ToBoolean(Session["IsStakeholder"]))
                    e.Row.Cells[GrdStaff.Columns.IndexOf(col)].CssClass = "hiddencol";
                if (col.SortExpression == "is_master_admin" && (!Convert.ToBoolean(Session["IsStakeholder"]) && !Convert.ToBoolean(Session["IsMasterAdmin"])))
                    e.Row.Cells[GrdStaff.Columns.IndexOf(col)].CssClass = "hiddencol";
            }
        }
    }
    protected void GrdStaff_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable fields = DBBase.GetGenericDataTable(null, "Field", "field_id", "descr");
        DataTable costcentres = CostCentreDB.GetDataTable();
        DataTable positions = StaffPositionDB.GetDataTable();
        DataTable dt = Session["externalstaffinfo_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("staff_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlTitle = (DropDownList)e.Row.FindControl("ddlTitle");
            if (ddlTitle != null)
            {
                DataTable titles = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Title", Convert.ToInt32(thisRow["title_id"]) == 0 ? "" : " title_id <> 0 ", " descr ", "title_id", "descr");
                ddlTitle.DataSource = titles;
                ddlTitle.DataTextField = "descr";
                ddlTitle.DataValueField = "title_id";
                ddlTitle.DataBind();
                ddlTitle.SelectedValue = thisRow["title_id"].ToString();
            }
            DropDownList ddlGender = (DropDownList)e.Row.FindControl("ddlGender");
            if (ddlGender != null)
            {
                if (thisRow["gender"].ToString() != "")
                    for (int i = ddlGender.Items.Count - 1; i >= 0; i--)
                        if (ddlGender.Items[i].Value == "")
                            ddlGender.Items.RemoveAt(i);
            }

            if (ddlTitle != null && ddlGender != null)
                ddlTitle.Attributes["onchange"] = "title_changed_reset_gender('" + ddlTitle.ClientID + "','" + ddlGender.ClientID + "');";


            DropDownList ddlDOB_Day = (DropDownList)e.Row.FindControl("ddlDOB_Day");
            DropDownList ddlDOB_Month = (DropDownList)e.Row.FindControl("ddlDOB_Month");
            DropDownList ddlDOB_Year = (DropDownList)e.Row.FindControl("ddlDOB_Year");
            if (ddlDOB_Day != null && ddlDOB_Month != null && ddlDOB_Year != null)
            {
                ddlDOB_Day.Items.Add(new ListItem("--", "-1"));
                ddlDOB_Month.Items.Add(new ListItem("--", "-1"));
                ddlDOB_Year.Items.Add(new ListItem("----", "-1"));

                for (int i = 1; i <= 31; i++)
                    ddlDOB_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 1; i <= 12; i++)
                    ddlDOB_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 1915; i <= DateTime.Today.Year; i++)
                    ddlDOB_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                if (thisRow["dob"] != DBNull.Value)
                {
                    DateTime dob = Convert.ToDateTime(thisRow["dob"]);

                    ddlDOB_Day.SelectedValue = dob.Day.ToString();
                    ddlDOB_Month.SelectedValue = dob.Month.ToString();

                    int firstYearSelectable = Convert.ToInt32(ddlDOB_Year.Items[1].Value);
                    int lastYearSelectable = Convert.ToInt32(ddlDOB_Year.Items[ddlDOB_Year.Items.Count - 1].Value);
                    if (dob.Year < firstYearSelectable)
                        ddlDOB_Year.Items.Insert(1, new ListItem(dob.Year.ToString(), dob.Year.ToString()));
                    if (dob.Year > lastYearSelectable)
                        ddlDOB_Year.Items.Add(new ListItem(dob.Year.ToString(), dob.Year.ToString()));

                    ddlDOB_Year.SelectedValue = dob.Year.ToString();
                }
            }

            TextBox txtPwd = (TextBox)e.Row.FindControl("txtPwd");
            if (txtPwd != null)
                txtPwd.Attributes["value"] = thisRow["pwd"].ToString();

            DropDownList ddlPosition = (DropDownList)e.Row.FindControl("ddlStaffPosition");
            if (ddlPosition != null)
            {
                ddlPosition.DataSource = positions;
                ddlPosition.DataTextField = "descr";
                ddlPosition.DataValueField = "staff_position_id";
                ddlPosition.DataBind();
                ddlPosition.SelectedValue = thisRow["staff_position_id"].ToString();
            }
            DropDownList ddlField = (DropDownList)e.Row.FindControl("ddlField");
            if (ddlField != null)
            {
                ddlField.DataSource = fields;
                ddlField.DataTextField = "descr";
                ddlField.DataValueField = "field_id";
                ddlField.DataBind();
                ddlField.SelectedValue = thisRow["field_id"].ToString();
            }
            DropDownList ddlCostCentre = (DropDownList)e.Row.FindControl("ddlCostCentre");
            if (ddlCostCentre != null)
            {
                ddlCostCentre.DataSource = costcentres;
                ddlCostCentre.DataTextField = "descr";
                ddlCostCentre.DataValueField = "costcentre_id";
                ddlCostCentre.DataBind();
                ddlCostCentre.SelectedValue = thisRow["costcentre_id"].ToString();
            }

            bool is_fired = Convert.ToBoolean(thisRow["is_fired"]);

            DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
            if (ddlStatus != null)
            {
                if (is_fired)
                    ddlStatus.SelectedValue = "Inactive";
                else
                    ddlStatus.SelectedValue = "Active";
            }
            Label lblIsFired = (Label)e.Row.FindControl("lblIsFired");
            if (is_fired)
            {
                e.Row.ForeColor = System.Drawing.Color.Gray;
                if (lblIsFired != null)
                    lblIsFired.ForeColor = System.Drawing.Color.Red;
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DataTable titles = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Title", " title_id <> 0 ", " descr ", "title_id", "descr");
            DropDownList ddlTitle = (DropDownList)e.Row.FindControl("ddlNewTitle");
            ddlTitle.DataSource = titles;
            ddlTitle.DataBind();
            ddlTitle.SelectedIndex = Utilities.IndexOf(ddlTitle, "mr", "mr.");
        }
    }
    protected void GrdStaff_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdStaff.EditIndex = -1;
        FillGrid();
    }
    protected void GrdStaff_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId          = (Label)GrdStaff.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlTitle       = (DropDownList)GrdStaff.Rows[e.RowIndex].FindControl("ddlTitle");
        TextBox      txtFirstname   = (TextBox)GrdStaff.Rows[e.RowIndex].FindControl("txtFirstname");
        TextBox      txtMiddlename  = (TextBox)GrdStaff.Rows[e.RowIndex].FindControl("txtMiddlename");
        TextBox      txtSurname     = (TextBox)GrdStaff.Rows[e.RowIndex].FindControl("txtSurname");
        DropDownList ddlGender      = (DropDownList)GrdStaff.Rows[e.RowIndex].FindControl("ddlGender");

        TextBox      txtLogin       = (TextBox)GrdStaff.Rows[e.RowIndex].FindControl("txtLogin");
        TextBox      txtPwd         = (TextBox)GrdStaff.Rows[e.RowIndex].FindControl("txtPwd");
        DropDownList ddlStatus      = (DropDownList)GrdStaff.Rows[e.RowIndex].FindControl("ddlStatus");


        int staff_id = Convert.ToInt32(lblId.Text);
        int person_id = GetPersonID(Convert.ToInt32(lblId.Text));

        if (person_id == -1) // happens when back button hit after update .. with option to update again ... but no selected row exists within page data
        {
            GrdStaff.EditIndex = -1;
            FillGrid();
            return;
        }


        Staff staff = StaffDB.GetByID(staff_id);
        if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && staff.Login != txtLogin.Text && UserDatabaseMapperDB.UsernameExists(txtLogin.Text))
        {
            SetErrorMessage("Login name already in use by another user");
            return;
        }
        if (staff.Login != txtLogin.Text && StaffDB.LoginExists(txtLogin.Text, staff_id))
        {
            SetErrorMessage("Login name already in use by another user");
            return;
        }
        if (staff.Pwd != txtPwd.Text && txtPwd.Text.Length < 6)
        {
            SetErrorMessage(staff.Pwd.Length >= 6 ? "Password must be at least 6 characters" : "New passwords must be at least 6 characters");
            return;
        }

        DataTable dt = Session["externalstaffinfo_data"] as DataTable;
        DataRow[] foundRows = dt.Select("person_id=" + person_id.ToString());
        DataRow row = foundRows[0];  // Convert.ToInt32(row["person_id"])



        PersonDB.Update(person_id, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), row["nickname"].ToString(), ddlGender.SelectedValue, staff.Person.Dob, DateTime.Now);
        StaffDB.Update(staff_id, person_id, txtLogin.Text, txtPwd.Text, Convert.ToInt32(row["staff_position_id"]), staff.Field.ID, staff.CostCentre.CostCentreID,
                       staff.IsContractor, staff.Tfn, staff.ProviderNumber,
                       ddlStatus.SelectedValue == "Inactive", staff.IsCommission, staff.CommissionPercent,
                       staff.IsStakeholder, staff.IsMasterAdmin, staff.IsAdmin, staff.IsPrincipal, staff.IsProvider, staff.IsExternal,
                       row["start_date"] == DBNull.Value ? DateTime.MinValue : (DateTime)row["start_date"], row["end_date"] == DBNull.Value ? DateTime.MinValue : (DateTime)row["end_date"], row["comment"].ToString(), staff.EnableDailyReminderSMS, staff.EnableDailyReminderEmail);

        if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && staff.Login != txtLogin.Text)
        {
            UserDatabaseMapper curDBMapper = UserDatabaseMapperDB.GetByLogin(staff.Login, Session["DB"].ToString());
            UserDatabaseMapperDB.Update(curDBMapper.ID, txtLogin.Text, Session["DB"].ToString());
        }


        GrdStaff.EditIndex = -1;
        FillGrid();
    }
    protected void GrdStaff_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdStaff.Rows[e.RowIndex].FindControl("lblId");
        int staff_id = Convert.ToInt32(lblId.Text);
        int person_id = GetPersonID(Convert.ToInt32(lblId.Text));

        try
        {
            //StaffDB.Delete(staff_id);
            //PersonDB.Delete(person_id);
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
    protected void GrdStaff_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            CustomValidator txtValidateDOB = (CustomValidator)GrdStaff.FooterRow.FindControl("txtValidateNewDOB");
            if (!txtValidateDOB.IsValid)
                return;

            DropDownList ddlTitle      = (DropDownList)GrdStaff.FooterRow.FindControl("ddlNewTitle");
            TextBox      txtFirstname  = (TextBox)GrdStaff.FooterRow.FindControl("txtNewFirstname");
            TextBox      txtMiddlename = (TextBox)GrdStaff.FooterRow.FindControl("txtNewMiddlename");
            TextBox      txtSurname    = (TextBox)GrdStaff.FooterRow.FindControl("txtNewSurname");
            DropDownList ddlGender     = (DropDownList)GrdStaff.FooterRow.FindControl("ddlNewGender");

            TextBox      txtLogin      = (TextBox)GrdStaff.FooterRow.FindControl("txtNewLogin");
            TextBox      txtPwd        = (TextBox)GrdStaff.FooterRow.FindControl("txtNewPwd");
            DropDownList ddlStatus     = (DropDownList)GrdStaff.FooterRow.FindControl("ddlStatus");


            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && UserDatabaseMapperDB.UsernameExists(txtLogin.Text))
            {
                SetErrorMessage("Login name already in use by another user");
                return;
            }
            if (StaffDB.LoginExists(txtLogin.Text))
            {
                SetErrorMessage("Login name already in use by another user");
                return;
            }
            if (txtPwd.Text.Length < 6)
            {
                SetErrorMessage("Password must be at least 6 characters");
                return;
            }


            DateTime dob = DateTime.MinValue;

            int person_id = -1;
            int mainDbUserID = -1;

            try
            {
                if (!!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                    mainDbUserID = UserDatabaseMapperDB.Insert(txtLogin.Text, Session["DB"].ToString());

                Staff loggedInStaff = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));
                person_id = PersonDB.Insert(loggedInStaff.Person.PersonID, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), "", ddlGender.SelectedValue, dob);
                StaffDB.Insert(person_id, txtLogin.Text, txtPwd.Text, StaffPositionDB.GetByDescr("Unknown").StaffPositionID, 0, 59,
                               false, "", "",
                               ddlStatus.SelectedValue == "Inactive", false, 0,
                               false, false, false, false, false, true,
                               DateTime.Today, DateTime.MinValue, "", false, false);

                FillGrid();
            }
            catch (Exception)
            {
                // roll back - backwards of creation order
                PersonDB.Delete(person_id);
                if (!!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                    UserDatabaseMapperDB.Delete(mainDbUserID);
            }
        }
    }
    protected void GrdStaff_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdStaff.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdStaff.EditIndex >= 0)
            return;

        DataTable dataTable = Session["externalstaffinfo_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["externalstaffinfo_sortexpression"] == null)
                Session["externalstaffinfo_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["externalstaffinfo_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["externalstaffinfo_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdStaff.DataSource = dataView;
            GrdStaff.DataBind();
        }
    }
    protected void GrdStaff_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdStaff.PageIndex = e.NewPageIndex;
        FillGrid();
    }


    private int GetStaffID(int personID)
    {
        DataTable dt = Session["externalstaffinfo_data"] as DataTable;
        DataRow[] foundRows = dt.Select("person_id=" + personID.ToString());
        return Convert.ToInt32(foundRows[0]["staff_id"]);
    }
    private int GetPersonID(int staffID)
    {
        DataTable dt = Session["externalstaffinfo_data"] as DataTable;
        DataRow[] foundRows = dt.Select("staff_id=" + staffID.ToString());
        return foundRows.Length > 0 ? Convert.ToInt32(foundRows[0]["person_id"]) : -1;
    }

    #endregion


    #region btnSearchSurname_Click, btnClearSurnameSearch_Click, chkUsePaging_CheckedChanged, chkShowFired_CheckedChanged

    protected void btnSearchSurname_Click(object sender, EventArgs e)
    {
        if (!Regex.IsMatch(txtSearchSurname.Text, @"^[a-zA-Z\-\']*$"))
        {
            SetErrorMessage("Search text can only be letters and hyphens");
            return;
        }
        else
            HideErrorMessage();

        string url = Request.RawUrl;
        url = UrlParamModifier.AddEdit(url, "surname_search", txtSearchSurname.Text);
        url = UrlParamModifier.AddEdit(url, "surname_starts_with", chkSurnameSearchOnlyStartWith.Checked ? "1" : "0");
        Response.Redirect(url);
    }
    protected void btnClearSurnameSearch_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["surname_search"] != null || Request.QueryString["surname_starts_with"] != null)
        {
            string url = Request.RawUrl;
            url = UrlParamModifier.Remove(url, "surname_search");
            url = UrlParamModifier.Remove(url, "surname_starts_with");
            Response.Redirect(url);
        }
        else
            txtSearchSurname.Text = "";
    }

    protected void chkUsePaging_CheckedChanged(object sender, EventArgs e)
    {
        this.GrdStaff.AllowPaging = chkUsePaging.Checked;
        FillGrid();
    }
    protected void chkShowFired_CheckedChanged(object sender, EventArgs e)
    {
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
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br /><br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br /><br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}