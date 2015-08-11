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

public partial class RegisterOrganisationsToPatientV2 : System.Web.UI.Page
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
                Session.Remove("registerorgtopatient_sortexpression");
                Session.Remove("registerorgtopatient_data");
                FillGrid();
            }

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


    #region GetUrlParamType(), IsValidFormID(), GetFormID()

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

    private enum UrlParamType { Edit, SelectToGoToBookings, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "select_to_go_to_bookings")
            return UrlParamType.SelectToGoToBookings;
        else
            return UrlParamType.None;
    }

    #endregion

    #region GrdRegistration

    private bool hideFotter = false;

    protected void FillGrid()
    {
        if (!IsValidFormID())
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage(Utilities.IsDev() ? "No patient exists with this ID" : "");
            return;
        }
        patient.Person = PersonDB.GetByID(patient.Person.PersonID);

        lblHeading.Text = Page.Title = "Manage Registrations For :  " + patient.Person.Firstname + " " + patient.Person.Surname;
        this.lnkThisPatient.NavigateUrl = "~/PatientDetailV2.aspx?type=view&id=" + GetFormID().ToString();
        this.lnkThisPatient.Text = "Back to details for " + patient.Person.FullnameWithoutMiddlename;


        DataTable dt = RegisterPatientDB.GetDataTable_OrganisationsOf(patient.PatientID);
        Session["registerorgtopatient_data"] = dt;


        spn_booking_screen_link.Visible   = UserView.GetInstance().IsAdminView;
        lblSelectOrgBeforeBooking.Visible = dt.Rows.Count == 0 && GetUrlParamType() == UrlParamType.SelectToGoToBookings;
        lnkBookingScreen.Visible          = dt.Rows.Count >  0;
        lnkBookingScreen.NavigateUrl      = String.Format("~/BookingScreenGetPatientOrgsV2.aspx?patient_id={0}", Request["id"]);
        lnkBookingScreen.Text             = "Make Booking";


        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["registerorgtopatient_sortexpression"] != null && Session["registerorgtopatient_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["registerorgtopatient_sortexpression"].ToString();
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
                HideTableAndSetErrorMessage("", ex.ToString());
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

        if (hideFotter)
            GrdRegistration.FooterRow.Visible = false;
    }
    protected void GrdRegistration_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdRegistration_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        UserView userView = UserView.GetInstance();

        DataTable dt = Session["registerorgtopatient_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("register_patient_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlOrganisation = (DropDownList)e.Row.FindControl("ddlOrganisation");
            if (ddlOrganisation != null)
            {
                Organisation[] incList_orig = RegisterPatientDB.GetOrganisationsOf(patient.PatientID);
                Organisation[] incList = Organisation.RemoveByID(incList_orig, Convert.ToInt32(thisRow["organisation_id"]));
                DataTable orgs = OrganisationDB.GetDataTable_AllNotInc(incList, true, !userView.IsClinicView, !userView.IsAgedCareView, true, true);
                orgs.DefaultView.Sort = "name ASC";
                foreach (DataRowView row in orgs.DefaultView)
                    ddlOrganisation.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
                ddlOrganisation.SelectedValue = thisRow["organisation_id"].ToString();
            }

            HyperLink lnkBookings = (HyperLink)e.Row.FindControl("lnkBookings");
            if (lnkBookings != null)
            {
                lnkBookings.NavigateUrl = string.Format("~/BookingsV2.aspx?orgs={0}&patient={1}", Convert.ToInt32(thisRow["organisation_id"]), patient.PatientID);
            }


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlOrganisation = (DropDownList)e.Row.FindControl("ddlNewOrganisation");
            if (ddlOrganisation != null)
            {
                Organisation[] incList = RegisterPatientDB.GetOrganisationsOf(patient.PatientID);
                DataTable orgs = OrganisationDB.GetDataTable_AllNotInc(incList, true, !userView.IsClinicView, !userView.IsAgedCareView, true, true);
                orgs.DefaultView.Sort = "name ASC";

                foreach (DataRowView row in orgs.DefaultView)
                    ddlOrganisation.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));

                if (orgs.Rows.Count == 0)
                    hideFotter = true;
            }
        }
    }
    protected void GrdRegistration_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdRegistration.EditIndex = -1;
        FillGrid();
    }
    protected void GrdRegistration_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlOrganisation = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlOrganisation");

        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        RegisterPatientDB.Update(Convert.ToInt32(lblId.Text), Convert.ToInt32(ddlOrganisation.SelectedValue), patient.PatientID);

        GrdRegistration.EditIndex = -1;
        FillGrid();
    }
    protected void GrdRegistration_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");

        RegisterPatient registerPatient = RegisterPatientDB.GetByID(Convert.ToInt32(lblId.Text));
        if (BookingDB.GetCountByPatientAndOrg(registerPatient.Patient.PatientID, registerPatient.Organisation.OrganisationID) > 0)
        {
            SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' because there exists a booking for this patient there.");
            return;
        }

        try
        {
            RegisterPatientDB.UpdateInactive(Convert.ToInt32(lblId.Text), false);
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                HideTableAndSetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                HideTableAndSetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrid();
    }
    protected void GrdRegistration_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlOrganisation = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewOrganisation");

            Patient patient = PatientDB.GetByID(GetFormID());
            if (patient == null)
            {
                HideTableAndSetErrorMessage("");
                return;
            }

            try
            {
                RegisterPatientDB.Insert(Convert.ToInt32(ddlOrganisation.SelectedValue), patient.PatientID);
            }
            catch (UniqueConstraintException)
            {
                // happens when 2 forms allow adding
                // do nothing and let form re-update
            }

            FillGrid();
        }
    }
    protected void GrdRegistration_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdRegistration.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdRegistration.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["registerorgtopatient_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["registerorgtopatient_sortexpression"] == null)
                Session["registerorgtopatient_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["registerorgtopatient_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["registerorgtopatient_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdRegistration.DataSource = dataView;
            GrdRegistration.DataBind();
        }
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        GrdRegistration.Visible = false;
        lnkThisPatient.Visible = false; 
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