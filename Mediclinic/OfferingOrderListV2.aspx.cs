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

public partial class OfferingOrderListV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, false, false, false, true);
                Session.Remove("staff_offerngs_sortexpression");
                Session.Remove("offeringorder_data");

                SetUpGUI();
                FillGrid();

                if (IsValidFormPatientID())
                {
                    Patient patient = PatientDB.GetByID(GetFormPatientID());
                    lblHeading.Text = "Oders For " + patient.Person.FullnameWithoutMiddlename;
                }
            }

            this.GrdOfferingOrder.EnableViewState = true;

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
        UserView userView = UserView.GetInstance();

        ddlOrganisations.Items.Clear();
        ddlOrganisations.Items.Add(new ListItem("All " + (userView.IsAgedCareView ? "Facilities" : "Clinics"), "0"));
        foreach (Organisation curOrg in OrganisationDB.GetAll(false, true, !userView.IsClinicView && !userView.IsGPView, !userView.IsAgedCareView, true, true))
            ddlOrganisations.Items.Add(new ListItem(curOrg.Name, curOrg.OrganisationID.ToString()));

        ddlStaff.Items.Clear();
        ddlStaff.Items.Add(new ListItem("All Staff", "-1"));
        DataTable dtStaff = StaffDB.GetDataTable();
        DataView dataViewStaff = new DataView(dtStaff);
        dataViewStaff.Sort = "person_firstname, person_surname";
        dtStaff = dataViewStaff.ToTable();
        for (int i = 0; i < dtStaff.Rows.Count; i++)
            if (!Convert.ToBoolean(dtStaff.Rows[i]["staff_is_fired"]))
                ddlStaff.Items.Add(new ListItem(dtStaff.Rows[i]["person_firstname"].ToString() + " " + dtStaff.Rows[i]["person_surname"].ToString(), dtStaff.Rows[i]["staff_staff_id"].ToString()));

        ddlOfferings.Items.Clear();
        ddlOfferings.Items.Add(new ListItem("All Products & Services", "-1"));
        DataTable dtOfferings = OfferingDB.GetDataTable(false, "1,3", "63,89");
        DataView dataViewOfferings = new DataView(dtOfferings);
        dataViewOfferings.Sort = "o_name";
        dtOfferings = dataViewOfferings.ToTable();


        for (int i = 0; i < dtOfferings.Rows.Count; i++)
            if (!Convert.ToBoolean(dtOfferings.Rows[i]["o_is_deleted"]))
                ddlOfferings.Items.Add(new ListItem(dtOfferings.Rows[i]["o_name"].ToString(), dtOfferings.Rows[i]["o_offering_id"].ToString()));

        if (IsValidFormOrganisationID())
            ddlOrganisations.SelectedValue = OrganisationDB.GetByID(GetFormOrganisationID()).OrganisationID.ToString();
        if (IsValidFormStaffID())
            ddlStaff.SelectedValue = StaffDB.GetByID(GetFormStaffID()).StaffID.ToString();
        if (IsValidFormOfferingID())
            ddlOfferings.SelectedValue = OfferingDB.GetByID(GetFormOfferingID()).OfferingID.ToString();

        txtStartDate.Text = IsValidFormStartDate() ? (GetFormStartDate(false) == DateTime.MinValue ? "" : GetFormStartDate(false).ToString("dd-MM-yyyy")) : "";
        txtEndDate.Text   = IsValidFormEndDate()   ? (GetFormEndDate(false)   == DateTime.MinValue ? "" : GetFormEndDate(false).ToString("dd-MM-yyyy"))   : "";

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";

        chkOnlyFilled.Checked   = IsValidFormOnlyFilled()   ? GetFormOnlyFilled()   : false;
        chkOnlyUnfilled.Checked = IsValidFormOnlyUnfilled() ? GetFormOnlyUnfilled() : true;
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

    private bool IsValidFormPatientID()
    {
        string id = Request.QueryString["patient"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormPatientID()
    {
        if (!IsValidFormPatientID())
            throw new Exception("Invalid url Patient id");

        string id = Request.QueryString["patient"];
        return Convert.ToInt32(id);
    }

    private bool IsValidFormOrganisationID()
    {
        string id = Request.QueryString["org"];
        return id != null && Regex.IsMatch(id, @"^\d+$") && id != "0";
    }
    private int GetFormOrganisationID()
    {
        if (!IsValidFormOrganisationID())
            throw new Exception("Invalid url Organisation id");

        string id = Request.QueryString["org"];
        return Convert.ToInt32(id);
    }

    protected bool IsValidFormOnlyFilled()
    {
        string only_filled = Request.QueryString["only_filled"];
        return only_filled != null && (only_filled == "0" || only_filled == "1");
    }
    protected bool GetFormOnlyFilled(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormOnlyFilled())
            throw new Exception("Invalid url 'only_filled'");
        return Request.QueryString["only_filled"] == "1";
    }

    protected bool IsValidFormOnlyUnfilled()
    {
        string only_unfilled = Request.QueryString["only_unfilled"];
        return only_unfilled != null && (only_unfilled == "0" || only_unfilled == "1");
    }
    protected bool GetFormOnlyUnfilled(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormOnlyUnfilled())
            throw new Exception("Invalid url 'only_unfilled'");
        return Request.QueryString["only_unfilled"] == "1";
    }


    protected bool IsValidFormStartDate()
    {
        string start_date = Request.QueryString["start_date"];
        return start_date != null && (start_date.Length == 0 || Regex.IsMatch(start_date, @"^\d{4}_\d{2}_\d{2}$"));
    }
    protected DateTime GetFormStartDate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormStartDate())
            throw new Exception("Invalid url 'start date'");
        return Request.QueryString["start_date"].Length == 0 ? DateTime.MinValue : GetDateFromString(Request.QueryString["start_date"], "yyyy_mm_dd");
    }
    protected bool IsValidFormEndDate()
    {
        string end_date = Request.QueryString["end_date"];
        return end_date != null && (end_date.Length == 0 || Regex.IsMatch(end_date, @"^\d{4}_\d{2}_\d{2}$"));
    }
    protected DateTime GetFormEndDate(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormEndDate())
            throw new Exception("Invalid url 'end date'");
        return Request.QueryString["end_date"].Length == 0 ? DateTime.MinValue : GetDateFromString(Request.QueryString["end_date"], "yyyy_mm_dd");
    }
    protected DateTime GetDateFromString(string sDate, string format)
    {
        if (format == "yyyy_mm_dd")
        {
            string[] dateparts = sDate.Split('_');
            return new DateTime(Convert.ToInt32(dateparts[0]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[2]));
        }
        else if (format == "dd_mm_yyyy")
        {
            string[] dateparts = sDate.Split('_');
            return new DateTime(Convert.ToInt32(dateparts[2]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[0]));
        }
        if (format == "yyyy-mm-dd")
        {
            string[] dateparts = sDate.Split('-');
            return new DateTime(Convert.ToInt32(dateparts[0]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[2]));
        }
        else if (format == "dd-mm-yyyy")
        {
            string[] dateparts = sDate.Split('-');
            return new DateTime(Convert.ToInt32(dateparts[2]), Convert.ToInt32(dateparts[1]), Convert.ToInt32(dateparts[0]));
        }
        else
            throw new ArgumentOutOfRangeException("Unknown date format");
    }

    #endregion

    #region ValidDateCheck, GetDate, IsValidDate

    public bool IsValidDate(string day, string month, string year)
    {
        bool invalid = ((day == "-1" || month == "-1" || year == "-1") && (day != "-1" || month != "-1" || year != "-1"));

        if ((day == "-1" && month == "-1" && year == "-1"))
            return true;
        else if ((day == "-1" || month == "-1" || year == "-1"))
            return false;

        try
        {
            DateTime d = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    protected DateTime GetDate(string day, string month, string year)
    {
        if ((day == "-1" && month == "-1" && year == "-1"))
            return DateTime.MinValue;

        return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
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
    protected void ddlOrganisations_SelectedIndexChanged(object sender, EventArgs e)
    {
        string url = Request.RawUrl;
        url = UrlParamModifier.Update(ddlOrganisations.SelectedValue != "0", url, "org", ddlOrganisations.SelectedValue);
        Response.Redirect(url);
    }

    #endregion

    #region btnSearch_Click, chkUsePaging_CheckedChanged

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        HideErrorMessage();

        if (txtStartDate.Text.Length > 0 && (!Regex.IsMatch(txtStartDate.Text, @"^\d{2}\-\d{2}\-\d{4}$") || !IsValidDate(txtStartDate.Text)))
        {
            SetErrorMessage("Start date must be empty or valid and of the format dd-mm-yyyy");
            return;
        }
        if (txtEndDate.Text.Length > 0 && (!Regex.IsMatch(txtEndDate.Text, @"^\d{2}\-\d{2}\-\d{4}$") || !IsValidDate(txtEndDate.Text)))
        {
            SetErrorMessage("End date must be empty or valid and of the format dd-mm-yyyy");
            return;
        }


        DateTime startDate = txtStartDate.Text.Length == 0 ? DateTime.MinValue : GetDateFromString(txtStartDate.Text, "dd-mm-yyyy");
        DateTime endDate   = txtEndDate.Text.Length   == 0 ? DateTime.MinValue : GetDateFromString(txtEndDate.Text, "dd-mm-yyyy");

        string url = ClearSearchesFromUrl(Request.RawUrl);
        url = UrlParamModifier.AddEdit(url, "start_date", startDate == DateTime.MinValue ? "" : startDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "end_date"  , endDate == DateTime.MinValue   ? "" : endDate.ToString("yyyy_MM_dd"));
        url = UrlParamModifier.AddEdit(url, "only_filled"  , chkOnlyFilled.Checked   ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "only_unfilled", chkOnlyUnfilled.Checked ? "1" : "0");

        Response.Redirect(url);
    }

    protected string ClearSearchesFromUrl(string url)
    {
        url = UrlParamModifier.Remove(url, "start_date");
        url = UrlParamModifier.Remove(url, "end_date");
        url = UrlParamModifier.Remove(url, "only_filled");
        url = UrlParamModifier.Remove(url, "only_unfilled");

        return url;
    }

    protected void chkUsePaging_CheckedChanged(object sender, EventArgs e)
    {
        this.GrdOfferingOrder.AllowPaging = chkUsePaging.Checked;
        FillGrid();
    }

    #endregion

    #region btnExport_Click

    protected void btnExport_Click(object sender, EventArgs e)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("\"" + "Patient" + "\"").Append(",");
        sb.Append("\"" + "Clinic" + "\"").Append(",");
        sb.Append("\"" + "Staff" + "\"").Append(",");
        sb.Append("\"" + "Product" + "\"").Append(",");
        sb.Append("\"" + "Ordered Date" + "\"").Append(",");
        sb.Append("\"" + "Filled Date" + "\"").Append(",");
        sb.Append("\"" + "Cancelled Date" + "\"").Append(",");
        sb.Append("\"" + "Quantity" + "\"").Append(",");
        sb.Append("\"" + "Description" + "\"").Append(",");
        sb.AppendLine();


        DataTable dt = Session["offeringorder_data"] as DataTable;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("\"" + dt.Rows[i]["person_patient_firstname"].ToString() + " " + dt.Rows[i]["person_patient_surname"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["organisation_name"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["person_staff_firstname"].ToString() + " " + dt.Rows[i]["person_staff_firstname"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["offering_name"].ToString() + "\"").Append(",");

                sb.Append("\"" + (dt.Rows[i]["offeringorder_date_ordered"] == DBNull.Value ? "" : Convert.ToDateTime(dt.Rows[i]["offeringorder_date_ordered"]).ToString("dd MMM yyyy HH:mm")) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["offeringorder_date_filled"] == DBNull.Value ? "" : Convert.ToDateTime(dt.Rows[i]["offeringorder_date_filled"]).ToString("dd MMM yyyy HH:mm")) + "\"").Append(",");
                sb.Append("\"" + (dt.Rows[i]["offeringorder_date_cancelled"] == DBNull.Value ? "" : Convert.ToDateTime(dt.Rows[i]["offeringorder_date_cancelled"]).ToString("dd MMM yyyy HH:mm")) + "\"").Append(",");

                sb.Append("\"" + dt.Rows[i]["offeringorder_quantity"].ToString() + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["offeringorder_descr"].ToString() + "\"").Append(",");

                sb.AppendLine();
            }
        }

        ExportCSV(Response, sb.ToString(), "SummaryReport.csv");
    }
    protected static void ExportCSV(HttpResponse response, string fileText, string fileName)
    {
        byte[] buffer = GetBytes(fileText);

        try
        {
            response.Clear();
            response.ContentType = "text/plain";
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            response.End();
        }
        catch (System.Web.HttpException ex)
        {
            // ignore exception where user closed the download box
            if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                throw;
        }
    }
    protected static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    #endregion

    #region GrdOfferingOrder

    protected void FillGrid()
    {
        DateTime fromDate = IsValidDate(txtStartDate.Text) && GetDate(txtStartDate.Text) != DateTime.MinValue ? GetDate(txtStartDate.Text) : DateTime.MinValue;
        DateTime toDate   = IsValidDate(txtEndDate.Text)   && GetDate(txtEndDate.Text)   != DateTime.MinValue   ? GetDate(txtEndDate.Text) : DateTime.MinValue;

        UserView userView = UserView.GetInstance();
        for (int i = 0; i < GrdOfferingOrder.Columns.Count; i++)
            if (GrdOfferingOrder.Columns[i].SortExpression == "organisation_name")
                GrdOfferingOrder.Columns[i].HeaderText = userView.IsAgedCareView ? "Facility" : "Clinic";

        DataTable dt = OfferingOrderDB.GetDataTable(
            fromDate,
            toDate,
            IsValidFormOfferingID()     ? GetFormOfferingID()     : -1,
            IsValidFormOrganisationID() ? GetFormOrganisationID() :  0,
            IsValidFormPatientID()      ? GetFormPatientID()      : -1,
            IsValidFormStaffID()        ? GetFormStaffID()        : -1,
            chkOnlyFilled.Checked,
            chkOnlyUnfilled.Checked
           );

        Session["offeringorder_data"] = dt;

        if (!IsPostBack)
            chkUsePaging.Checked = dt.Rows.Count > 50;

        this.GrdOfferingOrder.AllowPaging = chkUsePaging.Checked;


        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["staff_offerngs_sortexpression"] != null && Session["staff_offerngs_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["staff_offerngs_sortexpression"].ToString();
                GrdOfferingOrder.DataSource = dataView;
            }
            else
            {
                GrdOfferingOrder.DataSource = dt;
            }


            try
            {
                GrdOfferingOrder.DataBind();
                GrdOfferingOrder.PagerSettings.FirstPageText = "1";
                GrdOfferingOrder.PagerSettings.LastPageText = GrdOfferingOrder.PageCount.ToString();
                GrdOfferingOrder.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdOfferingOrder.DataSource = dt;
            GrdOfferingOrder.DataBind();

            int TotalColumns = GrdOfferingOrder.Rows[0].Cells.Count;
            GrdOfferingOrder.Rows[0].Cells.Clear();
            GrdOfferingOrder.Rows[0].Cells.Add(new TableCell());
            GrdOfferingOrder.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdOfferingOrder.Rows[0].Cells[0].Text = "No Record Found";
        }

        if (IsValidFormPatientID())
        {
            for (int i = 0; i < GrdOfferingOrder.Columns.Count; i++)
                if (GrdOfferingOrder.Columns[i].SortExpression == "person_patient_surname")
                    GrdOfferingOrder.Columns[i].Visible = false;
        }
        else
        {
            GrdOfferingOrder.FooterRow.Visible = false;
        }
    }
    protected void GrdOfferingOrder_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdOfferingOrder_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        UserView userView = UserView.GetInstance();

        DataTable dt = Session["offeringorder_data"] as DataTable;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("offeringorder_offering_order_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlOrganisation = (DropDownList)e.Row.FindControl("ddlOrganisation");
            if (ddlOrganisation != null)
            {
                ddlOrganisation.Items.Clear();
                foreach (Organisation curOrg in OrganisationDB.GetAll(false, true, !userView.IsClinicView && !userView.IsGPView, !userView.IsAgedCareView, true, true))
                    ddlOrganisation.Items.Add(new ListItem(curOrg.Name, curOrg.OrganisationID.ToString()));
            }
            
            DropDownList ddlStaff = (DropDownList)e.Row.FindControl("ddlStaff");
            if (ddlStaff != null)
            {
                DataTable staff = StaffDB.GetDataTable();
                DataView dataView = new DataView(staff);
                dataView.Sort = "person_firstname, person_surname";
                staff = dataView.ToTable();
                for (int i = 0; i < staff.Rows.Count; i++)
                {
                    Staff s = StaffDB.LoadAll(staff.Rows[i]);
                    ddlStaff.Items.Add(new ListItem(s.Person.FullnameWithoutMiddlename, s.StaffID.ToString()));
                }

                if (thisRow["offeringorder_staff_id"] != DBNull.Value)
                    ddlStaff.SelectedValue = thisRow["offeringorder_staff_id"].ToString();
            }


            DropDownList ddlOffering = (DropDownList)e.Row.FindControl("ddlOffering");
            if (ddlOffering != null)
            {
                string offering_invoice_type_id = null;
                if (userView.IsAgedCareView)
                    offering_invoice_type_id = "3,4";
                else
                    offering_invoice_type_id = "1,3";

                DataTable offerings = OfferingDB.GetDataTable(false, offering_invoice_type_id);
                ddlOffering.DataSource = offerings;
                ddlOffering.DataTextField = "o_name";
                ddlOffering.DataValueField = "o_offering_id";
                ddlOffering.DataBind();

                if (thisRow["offeringorder_offering_id"] != DBNull.Value)
                    ddlOffering.SelectedValue = thisRow["offeringorder_offering_id"].ToString();
            }

            DropDownList ddlQuantity = (DropDownList)e.Row.FindControl("ddlQuantity");
            if (ddlQuantity != null)
            {
                for (int i = 1; i <= 20; i++)
                    ddlQuantity.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 25; i <= 100; i += 5)
                    ddlQuantity.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 150; i <= 1000; i += 50)
                    ddlQuantity.Items.Add(new ListItem(i.ToString(), i.ToString()));

                ddlQuantity.SelectedValue = thisRow["offeringorder_quantity"].ToString();
            }




            DropDownList ddlOrderedDate_Day = (DropDownList)e.Row.FindControl("ddlOrderedDate_Day");
            DropDownList ddlOrderedDate_Month = (DropDownList)e.Row.FindControl("ddlOrderedDate_Month");
            DropDownList ddlOrderedDate_Year = (DropDownList)e.Row.FindControl("ddlOrderedDate_Year");
            if (ddlOrderedDate_Day != null && ddlOrderedDate_Month != null && ddlOrderedDate_Year != null)
            {
                for (int i = 1; i <= 31; i++)
                    ddlOrderedDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 1; i <= 12; i++)
                    ddlOrderedDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                    ddlOrderedDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                if (thisRow["offeringorder_date_ordered"] != DBNull.Value)
                {
                    DateTime OrderedDate = Convert.ToDateTime(thisRow["offeringorder_date_ordered"]);

                    ddlOrderedDate_Day.SelectedValue = OrderedDate.Day.ToString();
                    ddlOrderedDate_Month.SelectedValue = OrderedDate.Month.ToString();

                    int firstYearSelectable = Convert.ToInt32(ddlOrderedDate_Year.Items[1].Value);
                    int lastYearSelectable = Convert.ToInt32(ddlOrderedDate_Year.Items[ddlOrderedDate_Year.Items.Count - 1].Value);
                    if (OrderedDate.Year < firstYearSelectable)
                        ddlOrderedDate_Year.Items.Insert(1, new ListItem(OrderedDate.Year.ToString(), OrderedDate.Year.ToString()));
                    if (OrderedDate.Year > lastYearSelectable)
                        ddlOrderedDate_Year.Items.Add(new ListItem(OrderedDate.Year.ToString(), OrderedDate.Year.ToString()));

                    ddlOrderedDate_Year.SelectedValue = OrderedDate.Year.ToString();
                }
            }

            DropDownList ddlFilledDate_Day = (DropDownList)e.Row.FindControl("ddlFilledDate_Day");
            DropDownList ddlFilledDate_Month = (DropDownList)e.Row.FindControl("ddlFilledDate_Month");
            DropDownList ddlFilledDate_Year = (DropDownList)e.Row.FindControl("ddlFilledDate_Year");
            if (ddlFilledDate_Day != null && ddlFilledDate_Month != null && ddlFilledDate_Year != null)
            {
                ddlFilledDate_Day.Items.Add(new ListItem("--", "-1"));
                ddlFilledDate_Month.Items.Add(new ListItem("--", "-1"));
                ddlFilledDate_Year.Items.Add(new ListItem("----", "-1"));

                for (int i = 1; i <= 31; i++)
                    ddlFilledDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 1; i <= 12; i++)
                    ddlFilledDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                    ddlFilledDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                if (thisRow["offeringorder_date_filled"] != DBNull.Value)
                {
                    DateTime FilledDate = Convert.ToDateTime(thisRow["offeringorder_date_filled"]);

                    ddlFilledDate_Day.SelectedValue = FilledDate.Day.ToString();
                    ddlFilledDate_Month.SelectedValue = FilledDate.Month.ToString();

                    int firstYearSelectable = Convert.ToInt32(ddlFilledDate_Year.Items[1].Value);
                    int lastYearSelectable = Convert.ToInt32(ddlFilledDate_Year.Items[ddlFilledDate_Year.Items.Count - 1].Value);
                    if (FilledDate.Year < firstYearSelectable)
                        ddlFilledDate_Year.Items.Insert(1, new ListItem(FilledDate.Year.ToString(), FilledDate.Year.ToString()));
                    if (FilledDate.Year > lastYearSelectable)
                        ddlFilledDate_Year.Items.Add(new ListItem(FilledDate.Year.ToString(), FilledDate.Year.ToString()));

                    ddlFilledDate_Year.SelectedValue = FilledDate.Year.ToString();
                }
            }

            DropDownList ddlCancelledDate_Day = (DropDownList)e.Row.FindControl("ddlCancelledDate_Day");
            DropDownList ddlCancelledDate_Month = (DropDownList)e.Row.FindControl("ddlCancelledDate_Month");
            DropDownList ddlCancelledDate_Year = (DropDownList)e.Row.FindControl("ddlCancelledDate_Year");
            if (ddlCancelledDate_Day != null && ddlCancelledDate_Month != null && ddlCancelledDate_Year != null)
            {
                ddlCancelledDate_Day.Items.Add(new ListItem("--", "-1"));
                ddlCancelledDate_Month.Items.Add(new ListItem("--", "-1"));
                ddlCancelledDate_Year.Items.Add(new ListItem("----", "-1"));

                for (int i = 1; i <= 31; i++)
                    ddlCancelledDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 1; i <= 12; i++)
                    ddlCancelledDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                    ddlCancelledDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                if (thisRow["offeringorder_date_cancelled"] != DBNull.Value)
                {
                    DateTime CancelledDate = Convert.ToDateTime(thisRow["offeringorder_date_cancelled"]);

                    ddlCancelledDate_Day.SelectedValue = CancelledDate.Day.ToString();
                    ddlCancelledDate_Month.SelectedValue = CancelledDate.Month.ToString();

                    int firstYearSelectable = Convert.ToInt32(ddlCancelledDate_Year.Items[1].Value);
                    int lastYearSelectable = Convert.ToInt32(ddlCancelledDate_Year.Items[ddlCancelledDate_Year.Items.Count - 1].Value);
                    if (CancelledDate.Year < firstYearSelectable)
                        ddlCancelledDate_Year.Items.Insert(1, new ListItem(CancelledDate.Year.ToString(), CancelledDate.Year.ToString()));
                    if (CancelledDate.Year > lastYearSelectable)
                        ddlCancelledDate_Year.Items.Add(new ListItem(CancelledDate.Year.ToString(), CancelledDate.Year.ToString()));

                    ddlCancelledDate_Year.SelectedValue = CancelledDate.Year.ToString();
                }
            }


            Button btnSetFilled   = (Button)e.Row.FindControl("btnSetFilled");
            Button btnSetCancelled = (Button)e.Row.FindControl("btnSetCancelled");

            if (btnSetFilled != null && btnSetCancelled != null)
                btnSetFilled.Visible = btnSetCancelled.Visible = thisRow["offeringorder_date_filled"] == DBNull.Value && thisRow["offeringorder_date_cancelled"] == DBNull.Value;

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

            DropDownList ddlOrganisation = (DropDownList)e.Row.FindControl("ddlNewOrganisation");
            ddlOrganisation.Items.Clear();
            foreach (Organisation curOrg in OrganisationDB.GetAll(false, true, !userView.IsClinicView && !userView.IsGPView, !userView.IsAgedCareView, true, true))
                ddlOrganisation.Items.Add(new ListItem(curOrg.Name, curOrg.OrganisationID.ToString()));


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


            ddlStaff.SelectedValue = Session["StaffID"].ToString();

            DropDownList ddlOffering = (DropDownList)e.Row.FindControl("ddlNewOffering");
            string offering_invoice_type_id = null;
            if (userView.IsAgedCareView)
                offering_invoice_type_id = "3,4";
            else
                offering_invoice_type_id = "1,3";

            DataTable offerings = OfferingDB.GetDataTable(false, offering_invoice_type_id);
            ddlOffering.DataSource = offerings;
            ddlOffering.DataTextField = "o_name";
            ddlOffering.DataValueField = "o_offering_id";
            ddlOffering.DataBind();

            //DropDownList ddlPatient = (DropDownList)e.Row.FindControl("ddlNewPatient");
            //ddlPatient.Items.Add(new ListItem(".mickey .mouse", 54950.ToString()));

            DropDownList ddlQuantity = (DropDownList)e.Row.FindControl("ddlNewQuantity");
            for (int i = 1; i <= 20; i++)
                ddlQuantity.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 25; i <= 100; i += 5)
                ddlQuantity.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 150; i <= 1000; i += 50)
                ddlQuantity.Items.Add(new ListItem(i.ToString(), i.ToString()));


            DropDownList ddlOrderedDate_Day = (DropDownList)e.Row.FindControl("ddlNewOrderedDate_Day");
            DropDownList ddlOrderedDate_Month = (DropDownList)e.Row.FindControl("ddlNewOrderedDate_Month");
            DropDownList ddlOrderedDate_Year = (DropDownList)e.Row.FindControl("ddlNewOrderedDate_Year");
            for (int i = 1; i <= 31; i++)
                ddlOrderedDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 1; i <= 12; i++)
                ddlOrderedDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                ddlOrderedDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

            ddlOrderedDate_Day.SelectedValue   =  DateTime.Today.Day.ToString();
            ddlOrderedDate_Month.SelectedValue = DateTime.Today.Month.ToString();
            ddlOrderedDate_Year.SelectedValue  = DateTime.Today.Year.ToString();

            DropDownList ddlFilledDate_Day = (DropDownList)e.Row.FindControl("ddlNewFilledDate_Day");
            DropDownList ddlFilledDate_Month = (DropDownList)e.Row.FindControl("ddlNewFilledDate_Month");
            DropDownList ddlFilledDate_Year = (DropDownList)e.Row.FindControl("ddlNewFilledDate_Year");
            ddlFilledDate_Day.Items.Add(new ListItem("--", "-1"));
            ddlFilledDate_Month.Items.Add(new ListItem("--", "-1"));
            ddlFilledDate_Year.Items.Add(new ListItem("----", "-1"));
            for (int i = 1; i <= 31; i++)
                ddlFilledDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 1; i <= 12; i++)
                ddlFilledDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                ddlFilledDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

            DropDownList ddlCancelledDate_Day = (DropDownList)e.Row.FindControl("ddlNewCancelledDate_Day");
            DropDownList ddlCancelledDate_Month = (DropDownList)e.Row.FindControl("ddlNewCancelledDate_Month");
            DropDownList ddlCancelledDate_Year = (DropDownList)e.Row.FindControl("ddlNewCancelledDate_Year");
            ddlCancelledDate_Day.Items.Add(new ListItem("--", "-1"));
            ddlCancelledDate_Month.Items.Add(new ListItem("--", "-1"));
            ddlCancelledDate_Year.Items.Add(new ListItem("----", "-1"));
            for (int i = 1; i <= 31; i++)
                ddlCancelledDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 1; i <= 12; i++)
                ddlCancelledDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                ddlCancelledDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
        
        }
    }
    protected void GrdOfferingOrder_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdOfferingOrder.EditIndex = -1;
        FillGrid();
    }
    protected void GrdOfferingOrder_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label        lblId            = (Label)GrdOfferingOrder.Rows[e.RowIndex].FindControl("lblId");
        //DropDownList ddlPatient       = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlPatient");
        DropDownList ddlOrganisation  = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlOrganisation");
        DropDownList ddlStaff         = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlStaff");
        DropDownList ddlOffering      = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlOffering");
        DropDownList ddlQuantity      = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlQuantity");
        TextBox      txtDescr         = (TextBox)GrdOfferingOrder.Rows[e.RowIndex].FindControl("txtDescr");

        DropDownList ddlOrderedDate_Day     = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlOrderedDate_Day");
        DropDownList ddlOrderedDate_Month   = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlOrderedDate_Month");
        DropDownList ddlOrderedDate_Year    = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlOrderedDate_Year");
        DropDownList ddlCancelledDate_Day   = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlCancelledDate_Day");
        DropDownList ddlCancelledDate_Month = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlCancelledDate_Month");
        DropDownList ddlCancelledDate_Year  = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlCancelledDate_Year");
        DropDownList ddlFilledDate_Day      = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlFilledDate_Day");
        DropDownList ddlFilledDate_Month    = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlFilledDate_Month");
        DropDownList ddlFilledDate_Year     = (DropDownList)GrdOfferingOrder.Rows[e.RowIndex].FindControl("ddlFilledDate_Year");

        if (!IsValidDate(ddlOrderedDate_Day.SelectedValue, ddlOrderedDate_Month.SelectedValue, ddlOrderedDate_Year.SelectedValue))
        {
            SetErrorMessage("Please set a valid Ordered Date.");
            return;
        }
        if (!IsValidDate(ddlCancelledDate_Day.SelectedValue, ddlCancelledDate_Month.SelectedValue, ddlCancelledDate_Year.SelectedValue))
        {
            SetErrorMessage("Please set a valid Cancelled Date or set day/momth/year to '--'.");
            return;
        }
        if (!IsValidDate(ddlFilledDate_Day.SelectedValue, ddlFilledDate_Month.SelectedValue, ddlFilledDate_Year.SelectedValue))
        {
            SetErrorMessage("Please set a valid Filled Date or set day/momth/year to '--'.");
            return;
        }


        OfferingOrder offeringOrder = OfferingOrderDB.GetByID(Convert.ToInt32(lblId.Text));
        DateTime      newFilledDate = GetDate(ddlFilledDate_Day.SelectedValue, ddlFilledDate_Month.SelectedValue, ddlFilledDate_Year.SelectedValue);

        OfferingOrderDB.Update(
            Convert.ToInt32(lblId.Text),
            Convert.ToInt32(ddlOffering.SelectedValue),
            Convert.ToInt32(ddlOrganisation.SelectedValue),
            Convert.ToInt32(ddlStaff.SelectedValue),
            offeringOrder.Patient.PatientID,
            Convert.ToInt32(ddlQuantity.SelectedValue),
            GetDate(ddlOrderedDate_Day.SelectedValue, ddlOrderedDate_Month.SelectedValue, ddlOrderedDate_Year.SelectedValue),
            newFilledDate,
            GetDate(ddlCancelledDate_Day.SelectedValue, ddlCancelledDate_Month.SelectedValue, ddlCancelledDate_Year.SelectedValue),
            txtDescr.Text);

        if (offeringOrder.DateFilled == DateTime.MinValue && newFilledDate != DateTime.MinValue)
            StockDB.UpdateAndCheckWarning(offeringOrder.Organisation.OrganisationID, offeringOrder.Offering.OfferingID, offeringOrder.Quantity);
        if (offeringOrder.DateFilled != DateTime.MinValue && newFilledDate == DateTime.MinValue)
            StockDB.UpdateAndCheckWarning(offeringOrder.Organisation.OrganisationID, offeringOrder.Offering.OfferingID, -1 * offeringOrder.Quantity);

        GrdOfferingOrder.EditIndex = -1;
        FillGrid();
    }
    protected void GrdOfferingOrder_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        /*
        Label lblId = (Label)GrdOfferingOrder.Rows[e.RowIndex].FindControl("lblId");
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
        */
    }
    protected void GrdOfferingOrder_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            //DropDownList ddlPatient       = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewPatient");
            DropDownList ddlOrganisation  = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewOrganisation");
            DropDownList ddlStaff         = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewStaff");
            DropDownList ddlOffering      = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewOffering");
            DropDownList ddlQuantity      = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewQuantity");
            TextBox      txtDescr         = (TextBox)GrdOfferingOrder.FooterRow.FindControl("txtNewDescr");

            DropDownList ddlOrderedDate_Day     = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewOrderedDate_Day");
            DropDownList ddlOrderedDate_Month   = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewOrderedDate_Month");
            DropDownList ddlOrderedDate_Year    = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewOrderedDate_Year");
            DropDownList ddlCancelledDate_Day   = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewCancelledDate_Day");
            DropDownList ddlCancelledDate_Month = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewCancelledDate_Month");
            DropDownList ddlCancelledDate_Year  = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewCancelledDate_Year");
            DropDownList ddlFilledDate_Day      = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewFilledDate_Day");
            DropDownList ddlFilledDate_Month    = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewFilledDate_Month");
            DropDownList ddlFilledDate_Year     = (DropDownList)GrdOfferingOrder.FooterRow.FindControl("ddlNewFilledDate_Year");

            if (!IsValidDate(ddlOrderedDate_Day.SelectedValue, ddlOrderedDate_Month.SelectedValue, ddlOrderedDate_Year.SelectedValue))
            {
                SetErrorMessage("Please set a valid Ordered Date.");
                return;
            }
            if (!IsValidDate(ddlCancelledDate_Day.SelectedValue, ddlCancelledDate_Month.SelectedValue, ddlCancelledDate_Year.SelectedValue))
            {
                SetErrorMessage("Please set a valid Cancelled Date or set day/momth/year to '--'.");
                return;
            }
            if (!IsValidDate(ddlFilledDate_Day.SelectedValue, ddlFilledDate_Month.SelectedValue, ddlFilledDate_Year.SelectedValue))
            {
                SetErrorMessage("Please set a valid Filled Date or set day/momth/year to '--'.");
                return;
            }

            //int staff_id = IsValidFormStaffID() ? GetFormStaffID() : Convert.ToInt32(ddlStaff.SelectedValue);
            OfferingOrderDB.Insert(
                Convert.ToInt32(ddlOffering.SelectedValue),
                Convert.ToInt32(ddlOrganisation.SelectedValue),
                Convert.ToInt32(ddlStaff.SelectedValue),
                GetFormPatientID(),
                Convert.ToInt32(ddlQuantity.SelectedValue),
                GetDate(ddlOrderedDate_Day.SelectedValue, ddlOrderedDate_Month.SelectedValue, ddlOrderedDate_Year.SelectedValue),
                GetDate(ddlFilledDate_Day.SelectedValue, ddlFilledDate_Month.SelectedValue, ddlFilledDate_Year.SelectedValue),
                GetDate(ddlCancelledDate_Day.SelectedValue, ddlCancelledDate_Month.SelectedValue, ddlCancelledDate_Year.SelectedValue),
                txtDescr.Text);

            FillGrid();
        }

        if (e.CommandName == "SetFilled" || e.CommandName == "SetCancelled")
        {
            OfferingOrder o = OfferingOrderDB.GetByID(Convert.ToInt32(e.CommandArgument));
            if (e.CommandName == "SetFilled")
            {
                OfferingOrderDB.Update(
                    o.OfferingOrderID,
                    o.Offering.OfferingID,
                    o.Organisation.OrganisationID,
                    o.Staff.StaffID,
                    o.Patient.PatientID,
                    o.Quantity,
                    o.DateOrdered,
                    DateTime.Today,
                    o.DateCancelled,
                    o.Descr);


                // if was not filled before setting as filled, count down stock
                if (o.DateFilled == DateTime.MinValue)
                    StockDB.UpdateAndCheckWarning(o.Organisation.OrganisationID, o.Offering.OfferingID, o.Quantity);

            }

            if (e.CommandName == "SetCancelled")
                OfferingOrderDB.Update(
                    o.OfferingOrderID,
                    o.Offering.OfferingID,
                    o.Organisation.OrganisationID,
                    o.Staff.StaffID,
                    o.Patient.PatientID,
                    o.Quantity,
                    o.DateOrdered,
                    o.DateFilled,
                    DateTime.Today,
                    o.Descr);

            FillGrid();
        }

    }
    protected void GrdOfferingOrder_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdOfferingOrder.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdOfferingOrder.EditIndex >= 0)
            return;

        DataTable dataTable = Session["offeringorder_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["staff_offerngs_sortexpression"] == null)
                Session["staff_offerngs_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["staff_offerngs_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["staff_offerngs_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdOfferingOrder.DataSource = dataView;
            GrdOfferingOrder.DataBind();
        }
    }
    protected void GrdOfferingOrder_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdOfferingOrder.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    #endregion

    /*
    protected void UpdateStock(OfferingOrder o, bool filled)
    {

        if (filled)
        {
            Stock[] stockList = StockDB.GetByOrg(o.Organisation.OrganisationID);
            string warningEmail = SystemVariableDB.GetByDescr("StockWarningNotificationEmailAddress").Value;

            for (int i = 0; i < stockList.Length; i++)
            {
                if (o.Offering.OfferingID == stockList[i].Offering.OfferingID && stockList[i].Quantity > 0)
                {
                    int prevQty = stockList[i].Quantity;
                    int postQty = stockList[i].Quantity - o.Quantity;
                    if (postQty < 0) postQty = 0;

                    if (warningEmail.Length > 0 && stockList[i].WarningAmount >= 0 && stockList[i].WarningAmount < prevQty && stockList[i].WarningAmount >= postQty)
                    {
                        try
                        {
                            Emailer.SimpleEmail(
                                warningEmail,
                                "Stock Warning Level Reached For " + stockList[i].Offering.Name + " at " + o.Organisation.Name,
                                "This is an automated email to notify you that the stock warning level of <b>" + stockList[i].WarningAmount + "</b> items that was set for <b>" + stockList[i].Offering.Name + "</b> at <b>" + o.Organisation.Name + "</b> has been reached and you may need to re-stock.<br /><br />Best regards,<br />Mediclinic",
                                true,
                                null,
                                null
                                );
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex, true);
                        }
                    }

                    StockDB.UpdateQuantity(stockList[i].StockID, postQty);
                }
            }
        }
        else // set unfilled
        {
            Stock[] stockList = StockDB.GetByOrg(o.Organisation.OrganisationID);
            for (int i = 0; i < stockList.Length; i++)
            {
                if (o.Offering.OfferingID == stockList[i].Offering.OfferingID && stockList[i].Quantity >= 0)
                {
                    int prevQty = stockList[i].Quantity;
                    int postQty = stockList[i].Quantity + o.Quantity;
                    StockDB.UpdateQuantity(stockList[i].StockID, postQty);
                }
            }
        }

    }
    */


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

    protected void chkOnlyUnfilled_CheckedChanged(object sender, EventArgs e)
    {
        btnSearch_Click(null, EventArgs.Empty);
    }
    protected void chkOnlyFilled_CheckedChanged(object sender, EventArgs e)
    {
        btnSearch_Click(null, EventArgs.Empty);

    }
}