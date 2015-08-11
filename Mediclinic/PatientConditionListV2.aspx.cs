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

public partial class PatientConditionListV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, true);
                Session.Remove("patientcondition_sortexpression");
                Session.Remove("patientcondition_data");

                Patient patient = PatientDB.GetByID(GetFormPatientID());
                if (patient == null)
                    throw new Exception("Invalid url id");

                lblHeading.Text = "Patient Conditions For<br />" + patient.Person.FullnameWithoutMiddlename;
                FillGrdCondition(patient);
                FillGrdConditionView(patient);
            }

            this.GrdCondition.EnableViewState = true;

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

    #region IsValidFormPatientID(), GetFormPatientID()

    private bool IsValidFormPatientID()
    {
        string patient = Request.QueryString["patient"];
        return patient != null && Regex.IsMatch(patient, @"^\d+$");
    }
    private int GetFormPatientID()
    {
        if (!IsValidFormPatientID())
            throw new CustomMessageException("Invalid url id");

        string patient = Request.QueryString["patient"];
        return Convert.ToInt32(patient);
    }

    #endregion


    #region GrdCondition

    protected void FillGrdCondition(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormPatientID());

        DataTable dt = ConditionDB.GetDataTable(false);
        Hashtable conditionHash = ConditionDB.GetHashtable(false);

        Session["patientcondition_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdCondition.DataSource = dt;
            try
            {
                GrdCondition.DataBind();

                Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);
                foreach (GridViewRow row in GrdCondition.Rows)
                {
                    Label lblId = row.FindControl("lblId") as Label;
                    CheckBox chkSelect = row.FindControl("chkSelect") as CheckBox;
                    DropDownList ddlDate_Day       = (DropDownList)row.FindControl("ddlDate_Day");
                    DropDownList ddlDate_Month     = (DropDownList)row.FindControl("ddlDate_Month");
                    DropDownList ddlDate_Year      = (DropDownList)row.FindControl("ddlDate_Year");
                    DropDownList ddlNbrWeeksDue    = (DropDownList)row.FindControl("ddlNbrWeeksDue");

                    Label        lblNextDue        = row.FindControl("lblNextDue") as Label;
                    Label        lblWeeksLater     = row.FindControl("lblWeeksLater") as Label;
                    Label        lblAdditionalInfo = row.FindControl("lblAdditionalInfo") as Label;
                    

                    System.Web.UI.HtmlControls.HtmlControl br_date      = (System.Web.UI.HtmlControls.HtmlControl)row.FindControl("br_date");
                    System.Web.UI.HtmlControls.HtmlControl br_nweeksdue = (System.Web.UI.HtmlControls.HtmlControl)row.FindControl("br_nweeksdue");
                    System.Web.UI.HtmlControls.HtmlControl br_text      = (System.Web.UI.HtmlControls.HtmlControl)row.FindControl("br_text");

                    TextBox txtText = (TextBox)row.FindControl("txtText");

                    if (lblId == null || chkSelect == null)
                        continue;

                    Condition condition = (Condition)conditionHash[Convert.ToInt32(lblId.Text)];

                    br_date.Visible             = condition.ShowDate;
                    ddlDate_Day.Visible         = condition.ShowDate;
                    ddlDate_Month.Visible       = condition.ShowDate;
                    ddlDate_Year.Visible        = condition.ShowDate;
                    br_nweeksdue.Visible        = condition.ShowNWeeksDue;
                    ddlNbrWeeksDue.Visible      = condition.ShowNWeeksDue;
                    lblNextDue.Visible          = condition.ShowNWeeksDue;
                    lblWeeksLater.Visible       = condition.ShowNWeeksDue;
                    br_text.Visible             = condition.ShowText;
                    txtText.Visible             = condition.ShowText;
                    lblAdditionalInfo.Visible   = condition.ShowText;


                    if (selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                    {
                        PatientCondition ptCondition = (PatientCondition)selectedConditions[Convert.ToInt32(lblId.Text)];

                        chkSelect.Checked = selectedConditions[Convert.ToInt32(lblId.Text)] != null;

                        if (condition.ShowDate)
                        {
                            if (ptCondition.Date != DateTime.MinValue)
                            {
                                ddlDate_Day.SelectedValue   = ptCondition.Date.Day.ToString();
                                ddlDate_Month.SelectedValue = ptCondition.Date.Month.ToString();
                                ddlDate_Year.SelectedValue  = ptCondition.Date.Year.ToString();
                            }
                        }
                        if (condition.ShowNWeeksDue)
                        {
                            ddlNbrWeeksDue.SelectedValue = ptCondition.NWeeksDue.ToString();
                        }
                        if (condition.ShowText)
                        {
                            txtText.Text = ptCondition.Text;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }

            //Sort("parent_descr", "ASC");
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdCondition.DataSource = dt;
            GrdCondition.DataBind();

            int TotalColumns = GrdCondition.Rows[0].Cells.Count;
            GrdCondition.Rows[0].Cells.Clear();
            GrdCondition.Rows[0].Cells.Add(new TableCell());
            GrdCondition.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdCondition.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdCondition_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdCondition_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["patientcondition_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("condition_condition_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlDate_Day    = (DropDownList)e.Row.FindControl("ddlDate_Day");
            DropDownList ddlDate_Month  = (DropDownList)e.Row.FindControl("ddlDate_Month");
            DropDownList ddlDate_Year   = (DropDownList)e.Row.FindControl("ddlDate_Year");
            DropDownList ddlNbrWeeksDue = (DropDownList)e.Row.FindControl("ddlNbrWeeksDue");

            ddlDate_Day.Items.Add(new ListItem("--", "-1"));
            ddlDate_Month.Items.Add(new ListItem("--", "-1"));
            ddlDate_Year.Items.Add(new ListItem("--", "-1"));
            for (int i = 1; i <= 31; i++)
                ddlDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 1; i <= 12; i++)
                ddlDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                ddlDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

            for (int i = 0; i <= 52; i++)
                ddlNbrWeeksDue.Items.Add(new ListItem(i.ToString(), i.ToString()));
            


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["patientcondition_is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName   = "_UnDelete";
                    btnDelete.ImageUrl      = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip       = "UnDelete";
                }
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdCondition_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdCondition.EditIndex = -1;
        FillGrdCondition();
    }
    protected void GrdCondition_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdCondition.Rows[e.RowIndex].FindControl("lblId");

        //Condition condition = ConditionDB.GetByID(Convert.ToInt32(lblId.Text));
        //ConditionDB.Update(Convert.ToInt32(lblId.Text), txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), condition.IsDeleted);

        GrdCondition.EditIndex = -1;
        FillGrdCondition();
    }
    protected void GrdCondition_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdCondition.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //CostCentreDB.Delete(Convert.ToInt32(lblId.Text));
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrdCondition();
    }
    protected void GrdCondition_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {

            //ConditionDB.Insert(txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), false);

            FillGrdCondition();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    PatientConditionDB.UpdateInactive(id);
                else
                    PatientConditionDB.UpdateActive(id);
            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                if (Utilities.IsDev())
                    SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
                else
                    SetErrorMessage("Can not delete because other records depend on this");
            }

            FillGrdCondition();
        }

    }
    protected void GrdCondition_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdCondition.EditIndex = e.NewEditIndex;
        FillGrdCondition();
    }
    protected void GrdCondition_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdCondition.EditIndex >= 0)
            return;

        GrdCondition_Sort(e.SortExpression);
    }

    protected void GrdCondition_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["patientcondition_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["patientcondition_sortexpression"] == null)
                Session["patientcondition_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["patientcondition_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["patientcondition_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdCondition.DataSource = dataView;
            GrdCondition.DataBind();
        }
    }

    #endregion

    #region GrdConditionView

    protected void FillGrdConditionView(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormPatientID());

        DataTable dt = ConditionDB.GetDataTable(false);
        Hashtable conditionHash = ConditionDB.GetHashtable(false);

        Session["patientconditionview_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdConditionView.DataSource = dt;
            try
            {
                GrdConditionView.DataBind();

                Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);

                for(int i = GrdConditionView.Rows.Count-1 ; i>= 0; i--)
                {
                    Label lblId             = GrdConditionView.Rows[i].FindControl("lblId") as Label;

                    Label lblDate           = GrdConditionView.Rows[i].FindControl("lblDate") as Label;

                    Label lblNextDue        = GrdConditionView.Rows[i].FindControl("lblNextDue") as Label;
                    Label lblDateDue        = GrdConditionView.Rows[i].FindControl("lblDateDue") as Label;

                    Label lblAdditionalInfo = GrdConditionView.Rows[i].FindControl("lblAdditionalInfo") as Label;
                    Label lblText           = GrdConditionView.Rows[i].FindControl("lblText") as Label;
                    
                    System.Web.UI.HtmlControls.HtmlControl br_date      = (System.Web.UI.HtmlControls.HtmlControl)GrdConditionView.Rows[i].FindControl("br_date");
                    System.Web.UI.HtmlControls.HtmlControl br_nweeksdue = (System.Web.UI.HtmlControls.HtmlControl)GrdConditionView.Rows[i].FindControl("br_nweeksdue");
                    System.Web.UI.HtmlControls.HtmlControl br_text      = (System.Web.UI.HtmlControls.HtmlControl)GrdConditionView.Rows[i].FindControl("br_text");


                    if (lblId == null)
                        continue;

                    Condition condition = (Condition)conditionHash[Convert.ToInt32(lblId.Text)];

                    br_date.Visible             = condition.ShowDate;
                    lblDate.Visible             = condition.ShowDate;
                    br_nweeksdue.Visible        = condition.ShowNWeeksDue;
                    lblNextDue.Visible          = condition.ShowNWeeksDue;
                    lblDateDue.Visible          = condition.ShowNWeeksDue;
                    br_text.Visible             = condition.ShowText;
                    lblText.Visible             = condition.ShowText;
                    lblAdditionalInfo.Visible   = condition.ShowText;


                    if (selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                    {
                        PatientCondition ptCondition = (PatientCondition)selectedConditions[Convert.ToInt32(lblId.Text)];

                        if (condition.ShowDate)
                        {
                            lblDate.Text = ptCondition.Date == DateTime.MinValue ? "[Date Not Set]" : ptCondition.Date.ToString("d MMM, yyyy");
                        }
                        if (condition.ShowNWeeksDue)
                        {
                            bool expired = ptCondition.Date != DateTime.MinValue && ptCondition.Date.AddDays(7 * ptCondition.NWeeksDue) < DateTime.Today;
                            lblDateDue.Text = ptCondition.Date == DateTime.MinValue ? "[Date Not Set]" : (expired ? "<font color=\"red\">" : "") + ptCondition.Date.AddDays(7 * ptCondition.NWeeksDue).ToString("d MMM, yyyy") + (expired ? "</font>" : "");
                        }
                        if (condition.ShowText)
                        {
                            lblText.Text = ptCondition.Text.Length == 0 ? "[Blank]" : ptCondition.Text;
                        }

                    }
                    else
                    {
                        GrdConditionView.Rows[i].Visible = false;
                    }

                }

            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }

            //Sort("parent_descr", "ASC");
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdConditionView.DataSource = dt;
            GrdConditionView.DataBind();

            int TotalColumns = GrdConditionView.Rows[0].Cells.Count;
            GrdConditionView.Rows[0].Cells.Clear();
            GrdConditionView.Rows[0].Cells.Add(new TableCell());
            GrdConditionView.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdConditionView.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdConditionView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdConditionView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["patientconditionview_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("condition_condition_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["patientconditionview_is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName   = "_UnDelete";
                    btnDelete.ImageUrl      = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip       = "UnDelete";
                }
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdConditionView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdConditionView.EditIndex = -1;
        FillGrdConditionView();
    }
    protected void GrdConditionView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdConditionView.Rows[e.RowIndex].FindControl("lblId");

        //Condition condition = ConditionDB.GetByID(Convert.ToInt32(lblId.Text));
        //ConditionDB.Update(Convert.ToInt32(lblId.Text), txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), condition.IsDeleted);

        GrdConditionView.EditIndex = -1;
        FillGrdConditionView();
    }
    protected void GrdConditionView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdConditionView.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //CostCentreDB.Delete(Convert.ToInt32(lblId.Text));
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrdConditionView();
    }
    protected void GrdConditionView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {

            //ConditionDB.Insert(txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), false);

            FillGrdConditionView();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    PatientConditionDB.UpdateInactive(id);
                else
                    PatientConditionDB.UpdateActive(id);
            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                if (Utilities.IsDev())
                    SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
                else
                    SetErrorMessage("Can not delete because other records depend on this");
            }

            FillGrdConditionView();
        }

    }
    protected void GrdConditionView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdConditionView.EditIndex = e.NewEditIndex;
        FillGrdConditionView();
    }
    protected void GrdConditionView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdConditionView.EditIndex >= 0)
            return;

        GrdConditionView_Sort(e.SortExpression);
    }

    protected void GrdConditionView_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["patientconditionview_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["patientconditionview_sortexpression"] == null)
                Session["patientconditionview_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["patientconditionview_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["patientconditionview_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdConditionView.DataSource = dataView;
            GrdConditionView.DataBind();
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



    #region btnUpdate_Click

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormPatientID());
        if (patient == null)
            throw new Exception("Invalid url id");

        Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);
        foreach (GridViewRow row in GrdCondition.Rows)
        {
            Label        lblId          = row.FindControl("lblId") as Label;
            CheckBox     chkSelect      = row.FindControl("chkSelect") as CheckBox;
            DropDownList ddlDate_Day    = (DropDownList)row.FindControl("ddlDate_Day");
            DropDownList ddlDate_Month  = (DropDownList)row.FindControl("ddlDate_Month");
            DropDownList ddlDate_Year   = (DropDownList)row.FindControl("ddlDate_Year");
            DropDownList ddlNbrWeeksDue = (DropDownList)row.FindControl("ddlNbrWeeksDue");
            TextBox      txtText        = (TextBox)row.FindControl("txtText");


            if (lblId == null || chkSelect == null)
                continue;


            DateTime date = DateTime.MinValue;
            if (ddlDate_Day.Visible && ddlDate_Month.Visible && ddlDate_Year.Visible)
            {
                if (!IsValidDate(Convert.ToInt32(ddlDate_Day.Text), Convert.ToInt32(ddlDate_Month.Text), Convert.ToInt32(ddlDate_Year.Text)))
                {
                    SetErrorMessage("Please enter a valid date or unset all");
                    return;
                }
                else
                    date = GetDateFromForm(Convert.ToInt32(ddlDate_Day.Text), Convert.ToInt32(ddlDate_Month.Text), Convert.ToInt32(ddlDate_Year.Text));
            }

            int nweeksdue = ddlNbrWeeksDue.Visible ? Convert.ToInt32(ddlNbrWeeksDue.Text) : 0;
            string text = txtText.Visible ? txtText.Text.Trim() : string.Empty;

            if (chkSelect.Checked && selectedConditions[Convert.ToInt32(lblId.Text)] == null)
                PatientConditionDB.Insert(patient.PatientID, Convert.ToInt32(lblId.Text), date, nweeksdue, text, false);

            if (!chkSelect.Checked && selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                PatientConditionDB.Delete(patient.PatientID, Convert.ToInt32(lblId.Text));

            if (chkSelect.Checked && selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                PatientConditionDB.Update(patient.PatientID, Convert.ToInt32(lblId.Text), date, nweeksdue, text, false);
        }

        FillGrdCondition(patient);
        FillGrdConditionView(patient);

        SetErrorMessage("Updated");
    }

    public DateTime GetDateFromForm(int day, int month, int year)
    {
        if (day == -1 && month == -1 && year == -1)
            return DateTime.MinValue;

        else if (day != -1 && month != -1 && year != -1)
            return new DateTime(year, month, day);

        else
            throw new Exception("Date format is some selected and some not selected.");
    }
    public bool IsValidDate(int day, int month, int year)
    {
        bool isvalid = ((day == -1 && month == -1 && year == -1) ||
                        (day != -1 && month != -1 && year != -1 && _IsValidDate(day, month, year)));
        return isvalid;
    }
    private bool _IsValidDate(int day, int month, int year)
    {
        try
        {
            DateTime dt = new DateTime(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

}