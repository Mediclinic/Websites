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

public partial class Letters_SentHistoryV2 : System.Web.UI.Page
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
                Session.Remove("letterprinthistory_sortexpression");
                Session.Remove("letterprinthistory_data");

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

    protected bool IsValidFormOrganisation()
    {
        string organsiation_id = Request.QueryString["org"];
        return organsiation_id != null && Regex.IsMatch(organsiation_id, @"^\d+$") && OrganisationDB.Exists(Convert.ToInt32(organsiation_id));
    }
    protected int GetFormOrganisation(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormOrganisation())
            throw new Exception("Invalid url organsiation");
        return Convert.ToInt32(Request.QueryString["org"]);
    }

    protected bool IsValidFormRegReferrer()
    {
        string referrer_id = Request.QueryString["referrer"];
        return referrer_id != null && Regex.IsMatch(referrer_id, @"^\d+$") && ReferrerDB.Exists(Convert.ToInt32(referrer_id));
    }
    protected int GetFormRegReferrer(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormRegReferrer())
            throw new Exception("Invalid url referrer");
        return Convert.ToInt32(Request.QueryString["referrer"]);
    }

    #endregion

    #region GrdLetterPrintHistory

    protected void FillGrid()
    {
        int org_id               = IsValidFormOrganisation() ? GetFormOrganisation(false) :  0;
        int patient_id           = IsValidFormPatient()      ? GetFormPatient(false)      : -1;
        int register_referrer_id = IsValidFormRegReferrer()  ? GetFormRegReferrer(false)  : -1;


        if (patient_id != -1)
        {
            Patient patient  = PatientDB.GetByID(patient_id);
            lblHeading.Text  = "Letter Print History For ";
            lnkToEntity.Text = patient.Person.FullnameWithoutMiddlename;
            lnkToEntity.NavigateUrl = "PatientDetailV2.aspx?type=view&id=" + patient.PatientID;
        }
        else if (org_id != 0)
        {
            Organisation org = OrganisationDB.GetByID(org_id);
            lblHeading.Text  = "Letter Print History For ";
            lnkToEntity.Text = org.Name;
            lnkToEntity.NavigateUrl = "OrganisationDetailV2.aspx?type=view&id=" + org.OrganisationID;
        }


        DataTable dt = LetterPrintHistoryDB.GetDataTable(DateTime.MinValue, DateTime.MinValue, org_id, patient_id, register_referrer_id, Convert.ToInt32(Session["SiteID"]));
        Session["letterprinthistory_data"] = dt;

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["letterprinthistory_sortexpression"] != null && Session["letterprinthistory_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["letterprinthistory_sortexpression"].ToString();
                GrdLetterPrintHistory.DataSource = dataView;
            }
            else
            {
                GrdLetterPrintHistory.DataSource = dt;
            }


            try
            {
                GrdLetterPrintHistory.DataBind();
                GrdLetterPrintHistory.PagerSettings.FirstPageText = "1";
                GrdLetterPrintHistory.PagerSettings.LastPageText = GrdLetterPrintHistory.PageCount.ToString();
                GrdLetterPrintHistory.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
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
        DataTable dt = Session["letterprinthistory_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("lph_letter_print_history_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];

            LetterPrintHistory letterPrintHistory = LetterPrintHistoryDB.LoadAll(thisRow);

            Button btnRetrieveFlatFile = (Button)e.Row.FindControl("btnRetrieveFlatFile");
            if (btnRetrieveFlatFile != null && btnRetrieveFlatFile.CssClass != "hiddencol")
            {
                string historyDir = Letter.GetLettersHistoryDirectory(thisRow["lph_organisation_id"] == DBNull.Value ? 0 : Convert.ToInt32(thisRow["lph_organisation_id"]));
                string filePath    = historyDir + letterPrintHistory.LetterPrintHistoryID + System.IO.Path.GetExtension(letterPrintHistory.DocName);
                string filePathPDF = historyDir + letterPrintHistory.LetterPrintHistoryID + ".pdf";

                btnRetrieveFlatFile.Visible = System.IO.File.Exists(filePath) || System.IO.File.Exists(filePathPDF);
            }


            HyperLink lnkBookingSheetForPatient = (HyperLink)e.Row.FindControl("lnkBookingSheetForPatient");
            if (lnkBookingSheetForPatient != null)
            {
                if (thisRow["lph_booking_id"] == DBNull.Value)
                {
                    lnkBookingSheetForPatient.Visible = false;
                }
                else
                {
                    int     booking_id = Convert.ToInt32(thisRow["lph_booking_id"]);
                    Booking booking    = BookingDB.GetByID(booking_id);
                    lnkBookingSheetForPatient.NavigateUrl = booking.GetBookingSheetLinkV2();
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
    protected void GrdLetterPrintHistory_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdLetterPrintHistory.EditIndex = -1;
        FillGrid();
    }
    protected void GrdLetterPrintHistory_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        //Label lblId = (Label)GrdLetterPrintHistory.Rows[e.RowIndex].FindControl("lblId");
        //DropDownList ddlTitle = (DropDownList)GrdLetterPrintHistory.Rows[e.RowIndex].FindControl("ddlTitle");
        //TextBox txtFirstname = (TextBox)GrdLetterPrintHistory.Rows[e.RowIndex].FindControl("txtFirstname");
        //TextBox txtMiddlename = (TextBox)GrdLetterPrintHistory.Rows[e.RowIndex].FindControl("txtMiddlename");
        //TextBox txtSurname = (TextBox)GrdLetterPrintHistory.Rows[e.RowIndex].FindControl("txtSurname");
        //DropDownList ddlGender = (DropDownList)GrdLetterPrintHistory.Rows[e.RowIndex].FindControl("ddlGender");
        //TextBox txtDOB = (TextBox)GrdLetterPrintHistory.Rows[e.RowIndex].FindControl("txtDOB");

        //CheckBox chkIsDeceased = (CheckBox)GrdLetterPrintHistory.Rows[e.RowIndex].FindControl("chkIsDeceased");


        //string[] dobParts = txtDOB.Text.Trim().Split(new char[] { '-' });
        //DateTime dob = new DateTime(Convert.ToInt32(dobParts[2]), Convert.ToInt32(dobParts[1]), Convert.ToInt32(dobParts[0]));


        //int letter_print_history_id = Convert.ToInt32(lblId.Text);
        //int person_id = GetPersonID(Convert.ToInt32(lblId.Text));


        //DataTable dt = Session["letterprinthistory_data"] as DataTable;
        //DataRow[] foundRows = dt.Select("person_id=" + person_id.ToString());
        //DataRow row = foundRows[0];

        //PersonDB.Update(person_id, ddlTitle.SelectedValue, Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), ddlGender.SelectedValue, dob, DateTime.Now);
        //LetterPrintHistoryDB.Update(letter_print_history_id, person_id, chkIsDeceased.Checked);


        //GrdLetterPrintHistory.EditIndex = -1;
        //FillGrid();

    }
    protected void GrdLetterPrintHistory_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //Label lblId = (Label)GrdLetterPrintHistory.Rows[e.RowIndex].FindControl("lblId");
        //int letter_print_history_id = Convert.ToInt32(lblId.Text);
        //int person_id = GetPersonID(Convert.ToInt32(lblId.Text));

        //try
        //{
        //    LetterPrintHistoryDB.UpdateInactive(letter_print_history_id);
        //    //PersonDB.Delete(person_id);
        //}
        //catch (ForeignKeyConstraintException fkcEx)
        //{
        //    if (Utilities.IsDev())
        //        SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
        //    else
        //        SetErrorMessage("Can not delete because other records depend on this");
        //}

        //FillGrid();
    }
    protected void GrdLetterPrintHistory_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //if (e.CommandName.Equals("Insert"))
        //{
        //    DropDownList ddlTitle = (DropDownList)GrdLetterPrintHistory.FooterRow.FindControl("ddlNewTitle");
        //    TextBox txtFirstname = (TextBox)GrdLetterPrintHistory.FooterRow.FindControl("txtNewFirstname");
        //    TextBox txtMiddlename = (TextBox)GrdLetterPrintHistory.FooterRow.FindControl("txtNewMiddlename");
        //    TextBox txtSurname = (TextBox)GrdLetterPrintHistory.FooterRow.FindControl("txtNewSurname");
        //    DropDownList ddlGender = (DropDownList)GrdLetterPrintHistory.FooterRow.FindControl("ddlNewGender");
        //    TextBox txtDOB = (TextBox)GrdLetterPrintHistory.FooterRow.FindControl("txtNewDOB");

        //    CheckBox chkIsDeceased = (CheckBox)GrdLetterPrintHistory.FooterRow.FindControl("chkNewIsDeceased");



        //    string[] dobParts = txtDOB.Text.Trim().Split(new char[] { '-' });
        //    DateTime dob = new DateTime(Convert.ToInt32(dobParts[2]), Convert.ToInt32(dobParts[1]), Convert.ToInt32(dobParts[0]));

        //    Staff loggedInStaff = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));
        //    int person_id = PersonDB.Insert(loggedInStaff.Person.PersonID, ddlTitle.SelectedValue, Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), ddlGender.SelectedValue, dob);
        //    LetterPrintHistoryDB.Insert(person_id, chkIsDeceased.Checked, "", "");

        //    FillGrid();
        //}

        if (e.CommandName.Equals("RetrieveLetterDB"))
        {
            int index = Int32.Parse((string)e.CommandArgument);
            Label lblId = (Label)GrdLetterPrintHistory.Rows[index].FindControl("lblId");

            LetterFile letterFile = LetterPrintHistoryDB.GetLetterFile(Convert.ToInt32(lblId.Text));
            if (letterFile == null)
                throw new CustomMessageException("No file with selected ID");

            Letter.DownloadDocument(Response, letterFile.Contents, letterFile.Name);
        }

        if (e.CommandName.Equals("RetrieveLetterFlatFile"))
        {
            int index = Int32.Parse((string)e.CommandArgument);
            Label lblId = (Label)GrdLetterPrintHistory.Rows[index].FindControl("lblId");

            LetterPrintHistory letterPrintHistory = LetterPrintHistoryDB.GetByID(Convert.ToInt32(lblId.Text));

            DataTable dt = Session["letterprinthistory_data"] as DataTable;
            DataRow[] foundRows = dt.Select("lph_letter_print_history_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];
            string historyDir = Letter.GetLettersHistoryDirectory(thisRow["lph_organisation_id"] == DBNull.Value ? 0 : Convert.ToInt32(thisRow["lph_organisation_id"]));

            string filePath    = historyDir + letterPrintHistory.LetterPrintHistoryID + System.IO.Path.GetExtension(letterPrintHistory.DocName);
            string filePathPDF = historyDir + letterPrintHistory.LetterPrintHistoryID + ".pdf";
            if (!System.IO.File.Exists(filePath) && !System.IO.File.Exists(filePathPDF))
                throw new CustomMessageException("No file with selected ID");

            bool isPDF = System.IO.File.Exists(filePathPDF);

            byte[] fileContents = isPDF ? System.IO.File.ReadAllBytes(filePathPDF) : System.IO.File.ReadAllBytes(filePath);
            Letter.DownloadDocument(Response, fileContents, isPDF ? System.IO.Path.ChangeExtension(letterPrintHistory.DocName, ".pdf") : letterPrintHistory.DocName);
        }


        


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

        DataTable dataTable = Session["letterprinthistory_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["letterprinthistory_sortexpression"] == null)
                Session["letterprinthistory_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["letterprinthistory_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["letterprinthistory_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdLetterPrintHistory.DataSource = dataView;
            GrdLetterPrintHistory.DataBind();
        }
    }
    protected void GrdLetterPrintHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdLetterPrintHistory.PageIndex = e.NewPageIndex;
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