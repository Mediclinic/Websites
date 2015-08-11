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

public partial class Notifications_PTWeeklyBirthdaysAutoEmailingV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, true, false);
                Session.Remove("sortExpression_bookingswithoutsmsoremail");
                Session.Remove("data_bookingswithoutsmsoremail");
                SetupGUI();
                SetNotificationInfo();
                FillGrid();
            }


            this.GrdSummaryReport.EnableViewState = true;

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

    protected void SetupGUI()
    {
        chkIncWithMobile.Checked = IsValidFormIncWithMobile() ? GetFormIncWithMobile(false) : false;
        chkIncWithEmail.Checked  = IsValidFormIncWithEmail()  ? GetFormIncWithEmail(false)  : false;


        ddlStartDate_Month.Items.Clear();
        ddlEndDate_Month.Items.Clear();
        for (int i = 1; i <= 12; i++)
        {
            ddlStartDate_Month.Items.Add(new ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i), i.ToString()));
            ddlEndDate_Month.Items.Add(new ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i), i.ToString()));
        }

        ddlStartDate_Day.Items.Clear();
        ddlEndDate_Day.Items.Clear();
        for (int i = 1; i <= 31; i++)
        {
            ddlStartDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlEndDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }


        DateTime startDate = IsValidFormStartDate() ? GetFormStartDate(false) : DateTime.Today;
        DateTime endDate   = IsValidFormEndDate()   ? GetFormEndDate(false)   : DateTime.Today.AddDays(7);

        ddlStartDate_Month.SelectedValue = startDate.Month.ToString();
        ddlStartDate_Day.SelectedValue   = startDate.Day.ToString();
        ddlEndDate_Month.SelectedValue   = endDate.Month.ToString();
        ddlEndDate_Day.SelectedValue     = endDate.Day.ToString();

        ddlFromDaysAheadMondays.Items.Clear();
        ddlUntilDaysAheadMondays.Items.Clear();
        ddlFromDaysAheadTuesdays.Items.Clear();
        ddlUntilDaysAheadTuesdays.Items.Clear();
        ddlFromDaysAheadWednesdays.Items.Clear();
        ddlUntilDaysAheadWednesdays.Items.Clear();
        ddlFromDaysAheadThursdays.Items.Clear();
        ddlUntilDaysAheadThursdays.Items.Clear();
        ddlFromDaysAheadFridays.Items.Clear();
        ddlUntilDaysAheadFridays.Items.Clear();
        ddlFromDaysAheadSaturdays.Items.Clear();
        ddlUntilDaysAheadSaturdays.Items.Clear();
        ddlFromDaysAheadSundays.Items.Clear();
        ddlUntilDaysAheadSundays.Items.Clear();

        for (int i = 0; i <= 35; i++)
        {
            ddlFromDaysAheadMondays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlUntilDaysAheadMondays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlFromDaysAheadTuesdays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlUntilDaysAheadTuesdays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlFromDaysAheadWednesdays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlUntilDaysAheadWednesdays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlFromDaysAheadThursdays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlUntilDaysAheadThursdays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlFromDaysAheadFridays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlUntilDaysAheadFridays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlFromDaysAheadSaturdays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlUntilDaysAheadSaturdays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlFromDaysAheadSundays.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlUntilDaysAheadSundays.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
    }


    #region GrdSummaryReport

    protected void FillGrid()
    {
        DataTable dt;

        try
        {
            dt = PatientDB.GetBirthdays_DataTable(Convert.ToInt32(ddlStartDate_Month.SelectedValue), Convert.ToInt32(ddlStartDate_Day.SelectedValue),
                                                  Convert.ToInt32(ddlEndDate_Month.SelectedValue), Convert.ToInt32(ddlEndDate_Day.SelectedValue));
        }
        catch (CustomMessageException ex)
        {
            SetErrorMessage(ex.Message);
            return;
        }


        // get their mobile and emails

        Patient[] patients = new Patient[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            patients[i] = PatientDB.Load(dt.Rows[i]);
            patients[i].Person = PersonDB.Load(dt.Rows[i]);
            patients[i].Person.Title = IDandDescrDB.Load(dt.Rows[i], "t_title_id", "t_descr");
        }

        Hashtable patientContactPhoneNbrHash = GetPatientPhoneNbrCache(patients);
        Hashtable patientContactEmailHash    = GetPatientEmailCache(patients);

        ArrayList patientIDs = new ArrayList();

        dt.Columns.Add("mobile", typeof(string));
        dt.Columns.Add("email",  typeof(string));
        for (int i = dt.Rows.Count-1; i >= 0; i--)
        {
            string phoneNumPatient = GetPhoneNbr(patientContactPhoneNbrHash, patients[i].Person.EntityID, true);
            string emailPatient    = GetEmail(patientContactEmailHash, patients[i].Person.EntityID);

            if ((!chkIncWithMobile.Checked && (phoneNumPatient != null && phoneNumPatient.Length > 0)) ||
                (!chkIncWithEmail.Checked  && (emailPatient    != null && emailPatient.Length    > 0)))
            {
                dt.Rows.RemoveAt(i);
                continue;
            }

            dt.Rows[i]["mobile"] = phoneNumPatient == null ? "" : phoneNumPatient;
            dt.Rows[i]["email"]  = emailPatient    == null ? ""  : emailPatient;

            patientIDs.Add(patients[i].PatientID);
        }


        hiddenPatientIDs.Value = string.Join(",", (int[])patientIDs.ToArray(typeof(int)));
        
        Session["data_bookingswithoutsmsoremail"] = dt;

        if (!IsPostBack)
            chkUsePaging.Checked = dt.Rows.Count > 50;

        this.GrdSummaryReport.AllowPaging = chkUsePaging.Checked;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sortExpression_bookingswithoutsmsoremail"] != null && Session["sortExpression_bookingswithoutsmsoremail"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sortExpression_bookingswithoutsmsoremail"].ToString();
                GrdSummaryReport.DataSource = dataView;
            }
            else
            {
                GrdSummaryReport.DataSource = dt;
            }


            try
            {
                GrdSummaryReport.DataBind();
                GrdSummaryReport.PagerSettings.FirstPageText = "1";
                GrdSummaryReport.PagerSettings.LastPageText = GrdSummaryReport.PageCount.ToString();
                GrdSummaryReport.DataBind();

            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            } 
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdSummaryReport.DataSource = dt;
            GrdSummaryReport.DataBind();

            int TotalColumns = GrdSummaryReport.Rows[0].Cells.Count;
            GrdSummaryReport.Rows[0].Cells.Clear();
            GrdSummaryReport.Rows[0].Cells.Add(new TableCell());
            GrdSummaryReport.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdSummaryReport.Rows[0].Cells[0].Text = "No Record Found";
        }

    }
    protected void GrdSummaryReport_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdSummaryReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["data_bookingswithoutsmsoremail"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("patient_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];

            Patient patient = PatientDB.Load(thisRow);
            patient.Person = PersonDB.Load(thisRow);
            patient.Person.Title = IDandDescrDB.Load(thisRow, "t_title_id", "t_descr");












            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

        }
    }
    protected void GrdSummaryReport_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdSummaryReport_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdSummaryReport.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }
    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["data_bookingswithoutsmsoremail"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression_bookingswithoutsmsoremail"] == null)
                Session["sortExpression_bookingswithoutsmsoremail"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression_bookingswithoutsmsoremail"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["sortExpression_bookingswithoutsmsoremail"] = sortExpression + " " + newSortExpr;

            GrdSummaryReport.DataSource = dataView;
            GrdSummaryReport.DataBind();
        }
    }
    protected void GrdStaff_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdSummaryReport.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    #endregion

    #region GetCache & GetPhoneNbr/GetEmail from hashtable caches

    protected static Regex re = new Regex("[^0-9]"); // new Regex("[^0-9 -,]");

    protected static string GetPhoneNbr(Hashtable contactHash, int entityID, bool onlyMobile)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] contacts = (Contact[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            foreach (Contact c in contacts)
            {
                if (onlyMobile && c.ContactType.ContactTypeID != 30)  // ignore if not mobile nbr
                    continue;

                string phNum = re.Replace(c.AddrLine1, "").Trim();
                if (phNum.Length > 0)
                    return phNum;
            }
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] contacts = (ContactAus[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            foreach (ContactAus c in contacts)
            {
                if (onlyMobile && c.ContactType.ContactTypeID != 30)  // ignore if not mobile nbr
                    continue;

                string phNum = re.Replace(c.AddrLine1, "").Trim();
                if (phNum.Length > 0)
                    return phNum;
            }
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        return null;
    }

    protected static string GetPhoneNbrs(Hashtable contactHash, int entityID)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] contacts = (Contact[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            string nbrs = string.Empty;
            for (int i = 0; i < contacts.Length; i++)
            {
                if (i > 0 && i == contacts.Length - 1)
                    nbrs += " or " + re.Replace(contacts[i].AddrLine1, "").Trim();
                else if (i > 0)
                    nbrs += ", " + re.Replace(contacts[i].AddrLine1, "").Trim();
                else
                    nbrs += re.Replace(contacts[i].AddrLine1, "").Trim();
            }

            return nbrs;
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] contacts = (ContactAus[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            string nbrs = string.Empty;
            for (int i = 0; i < contacts.Length; i++)
            {
                if (i > 0 && i == contacts.Length - 1)
                    nbrs += " or " + re.Replace(contacts[i].AddrLine1, "").Trim();
                else if (i > 0)
                    nbrs += ", " + re.Replace(contacts[i].AddrLine1, "").Trim();
                else
                    nbrs += re.Replace(contacts[i].AddrLine1, "").Trim();
            }

            return nbrs;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
    }

    protected static string GetEmail(Hashtable contactHash, int entityID)
    {
        return ContactDB.GetEmailsCommaSepByEntityID(contactHash, entityID, false, true);
    }

    protected static Hashtable GetPatientPhoneNbrCache(Patient[] patients)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Patient patient in patients)
            entityIDArrayList.Add(patient.Person.EntityID);
        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));

        Hashtable contactHash = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1);

        return contactHash;
    }

    protected static Hashtable GetPatientEmailCache(Patient[] patients)
    {
        ArrayList entityIDArrayList = new ArrayList();
        foreach (Patient patient in patients)
            entityIDArrayList.Add(patient.Person.EntityID);
        int[] entityIDs = (int[])entityIDArrayList.ToArray(typeof(int));

        Hashtable contactHash = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);

        return contactHash;
    }

    protected static Hashtable GetPatientRegOrgCache(Patient[] patients)
    {
        ArrayList patientIDArrayList = new ArrayList();
        foreach (Patient patient in patients)
            patientIDArrayList.Add(patient.PatientID);
        int[] patientIDs = (int[])patientIDArrayList.ToArray(typeof(int));

        Hashtable regOrgHash = new Hashtable();
        System.Data.DataTable tbl = RegisterPatientDB.GetDataTable_OrganisationsOf(patientIDs, true, false, false, true, true);
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int patientID = Convert.ToInt32(tbl.Rows[i]["patient_id"]);
            Organisation org = OrganisationDB.Load(tbl.Rows[i], "", "organisation_entity_id", "organisation_is_deleted");

            if (regOrgHash[patientID] == null)
                regOrgHash[patientID] = new System.Collections.ArrayList();
            ((System.Collections.ArrayList)regOrgHash[patientID]).Add(org);
        }

        return regOrgHash;
    }

    #endregion
    
    #region IsValidFormStartDate(),  GetFormStartDate()....


    protected bool IsValidFormIncWithMobile()
    {
        string inc_with_mobile = Request.QueryString["inc_with_mobile"];
        return inc_with_mobile != null && (inc_with_mobile == "0" || inc_with_mobile == "1");
    }
    protected bool GetFormIncWithMobile(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncWithMobile())
            throw new Exception("Invalid url 'inc_with_mobile'");
        return Request.QueryString["inc_with_mobile"] == "1";
    }

    protected bool IsValidFormIncWithEmail()
    {
        string inc_with_email = Request.QueryString["inc_with_email"];
        return inc_with_email != null && (inc_with_email == "0" || inc_with_email == "1");
    }
    protected bool GetFormIncWithEmail(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormIncWithEmail())
            throw new Exception("Invalid url 'inc_with_email'");
        return Request.QueryString["inc_with_email"] == "1";
    }



    protected bool IsValidFormDate(string queryString)
    {
        string date = Request.QueryString[queryString];

        if (date == null)
            return false;
        if (!Regex.IsMatch(date, @"^\d{1,2}_\d{1,2}$"))
            return false;

        string[] parts = date.Split('_');
        return (Convert.ToInt32(parts[0]) >= 1 && Convert.ToInt32(parts[0]) <= 12) &&
                (Convert.ToInt32(parts[1]) >= 1 && Convert.ToInt32(parts[0]) <= 31);
    }
    protected DateTime GetFormStartDate(string queryString, bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormStartDate())
            throw new Exception("Invalid url '" + queryString + "'");

        string date = Request.QueryString[queryString];
        string[] parts = date.Split('_');

        return new DateTime(2004, Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
    }


    protected bool IsValidFormStartDate()
    {
        return IsValidFormDate("start_date");
    }
    protected DateTime GetFormStartDate(bool checkIsValid = true)
    {
        return GetFormStartDate("start_date", checkIsValid);
    }
    protected bool IsValidFormEndDate()
    {
        return IsValidFormDate("end_date");
    }
    protected DateTime GetFormEndDate(bool checkIsValid = true)
    {
        return GetFormStartDate("end_date", checkIsValid);
    }

    #endregion

    #region btnSearch_Click, chkUsePaging_CheckedChanged

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        HideErrorMessage();


        DateTime startDate = new DateTime(2004, Convert.ToInt32(ddlStartDate_Month.SelectedValue), Convert.ToInt32(ddlStartDate_Day.SelectedValue));
        DateTime endDate   = new DateTime(2004, Convert.ToInt32(ddlEndDate_Month.SelectedValue),   Convert.ToInt32(ddlEndDate_Day.SelectedValue));
        if (endDate < startDate) endDate = endDate.AddYears(1);  // if say dec 20 - jan 10, make the jan 10 for next year

        if (endDate.Subtract(startDate).TotalDays > 62)
        {
            SetErrorMessage("Can not select more than 2 months");
            return;
        }

        string url = ClearSearchesFromUrl(Request.RawUrl);
        url = UrlParamModifier.AddEdit(url, "start_date"     , startDate.ToString("M_d"));
        url = UrlParamModifier.AddEdit(url, "end_date"       , endDate.ToString("M_d"));
        url = UrlParamModifier.AddEdit(url, "inc_with_mobile", chkIncWithMobile.Checked ? "1" : "0");
        url = UrlParamModifier.AddEdit(url, "inc_with_email" , chkIncWithEmail.Checked  ? "1" : "0");

        Response.Redirect(url);
    }

    protected string ClearSearchesFromUrl(string url)
    {
        url = UrlParamModifier.Remove(url, "start_date");
        url = UrlParamModifier.Remove(url, "end_date");
        url = UrlParamModifier.Remove(url, "inc_with_mobile");
        url = UrlParamModifier.Remove(url, "inc_with_email");


        return url;
    }

    protected void chkUsePaging_CheckedChanged(object sender, EventArgs e)
    {
        this.GrdSummaryReport.AllowPaging = chkUsePaging.Checked;
        FillGrid();
    }

    #endregion

    #region btnUpdateNotificationInfo_Click

    protected void btnUpdateNotificationInfo_Click(object sender, EventArgs e)
    {
        txtEmailAddress.Text = txtEmailAddress.Text.Trim();


        try
        {
            if (chkEnableEmails.Checked && txtEmailAddress.Text.Length == 0)
                throw new CustomMessageException("To enable this, please set an email address");

            txtEmailAddress.Text = Utilities.CleanEmailAddresses(txtEmailAddress.Text);
            if (txtEmailAddress.Text.Length > 0 && !Utilities.IsValidEmailAddresses(txtEmailAddress.Text, false))
                throw new CustomMessageException("Invalid email address");

            if (Convert.ToInt32(ddlFromDaysAheadMondays.SelectedValue) > Convert.ToInt32(ddlUntilDaysAheadMondays.SelectedValue))
                throw new CustomMessageException("Monday: From Days Ahead can not be more than Until Days Ahead");
            if (Convert.ToInt32(ddlFromDaysAheadTuesdays.SelectedValue) > Convert.ToInt32(ddlUntilDaysAheadTuesdays.SelectedValue))
                throw new CustomMessageException("Tuesday: From Days Ahead can not be more than Until Days Ahead");
            if (Convert.ToInt32(ddlFromDaysAheadWednesdays.SelectedValue) > Convert.ToInt32(ddlUntilDaysAheadWednesdays.SelectedValue))
                throw new CustomMessageException("Wednesday: From Days Ahead can not be more than Until Days Ahead");
            if (Convert.ToInt32(ddlFromDaysAheadThursdays.SelectedValue) > Convert.ToInt32(ddlUntilDaysAheadThursdays.SelectedValue))
                throw new CustomMessageException("Thursday: From Days Ahead can not be more than Until Days Ahead");
            if (Convert.ToInt32(ddlFromDaysAheadFridays.SelectedValue) > Convert.ToInt32(ddlUntilDaysAheadFridays.SelectedValue))
                throw new CustomMessageException("Friday: From Days Ahead can not be more than Until Days Ahead");
            if (Convert.ToInt32(ddlFromDaysAheadSaturdays.SelectedValue) > Convert.ToInt32(ddlUntilDaysAheadSaturdays.SelectedValue))
                throw new CustomMessageException("Saturday: From Days Ahead can not be more than Until Days Ahead");
            if (Convert.ToInt32(ddlFromDaysAheadSundays.SelectedValue) > Convert.ToInt32(ddlUntilDaysAheadSundays.SelectedValue))
                throw new CustomMessageException("Sunday: From Days Ahead can not be more than Until Days Ahead");
        }
        catch (CustomMessageException ex)
        {
            SetErrorMessage(ex.Message);
            return;
        }


        SystemVariableDB.Update("BirthdayNotificationEmail_SendEmail",      chkEnableEmails.Checked ? "1" : "0");
        SystemVariableDB.Update("BirthdayNotificationEmail_EmailAddress",   txtEmailAddress.Text);

        SystemVariableDB.Update("BirthdayNotificationEmail_IncPatientsWithMobile", chkIncPatientsWithMobile.Checked ? "1" : "0");
        SystemVariableDB.Update("BirthdayNotificationEmail_IncPatientsWithEmail",  chkIncPatientsWithEmail.Checked ? "1" : "0");

        SystemVariableDB.Update("BirthdayNotificationEmail_SendMondays",    chkSendMondays.Checked    ? "1" : "0");
        SystemVariableDB.Update("BirthdayNotificationEmail_SendTuesdays",   chkSendTuesdays.Checked   ? "1" : "0");
        SystemVariableDB.Update("BirthdayNotificationEmail_SendWednesdays", chkSendWednesdays.Checked ? "1" : "0");
        SystemVariableDB.Update("BirthdayNotificationEmail_SendThursdays",  chkSendThursdays.Checked  ? "1" : "0");
        SystemVariableDB.Update("BirthdayNotificationEmail_SendFridays",    chkSendFridays.Checked    ? "1" : "0");
        SystemVariableDB.Update("BirthdayNotificationEmail_SendSaturdays",  chkSendSaturdays.Checked  ? "1" : "0");
        SystemVariableDB.Update("BirthdayNotificationEmail_SendSundays",    chkSendSundays.Checked    ? "1" : "0");

        SystemVariableDB.Update("BirthdayNotificationEmail_SendFromDaysAhead_Mondays",     ddlFromDaysAheadMondays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendUntilDaysAhead_Mondays",    ddlUntilDaysAheadMondays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendFromDaysAhead_Tuesdays",   ddlFromDaysAheadTuesdays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendUntilDaysAhead_Tuesdays",  ddlUntilDaysAheadTuesdays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendFromDaysAhead_Wednesdays",  ddlFromDaysAheadWednesdays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendUntilDaysAhead_Wednesdays", ddlUntilDaysAheadWednesdays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendFromDaysAhead_Thursdays",   ddlFromDaysAheadThursdays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendUntilDaysAhead_Thursdays",  ddlUntilDaysAheadThursdays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendFromDaysAhead_Fridays",     ddlFromDaysAheadFridays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendUntilDaysAhead_Fridays",    ddlUntilDaysAheadFridays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendFromDaysAhead_Saturdays",   ddlFromDaysAheadSaturdays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendUntilDaysAhead_Saturdays",  ddlUntilDaysAheadSaturdays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendFromDaysAhead_Sundays",     ddlFromDaysAheadSundays.SelectedValue);
        SystemVariableDB.Update("BirthdayNotificationEmail_SendUntilDaysAhead_Sundays",    ddlUntilDaysAheadSundays.SelectedValue);

        SetNotificationInfo(); // re-set to show it was update in the db
    }

    protected void btnRevertNotificationInfo_Click(object sender, EventArgs e)
    {
        SetNotificationInfo();
    }


    protected void SetNotificationInfo()
    {
        chkEnableEmails.Checked   = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendEmail").Value) == 1;
        txtEmailAddress.Text      = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_EmailAddress").Value;

        chkIncPatientsWithMobile.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_IncPatientsWithMobile").Value) == 1;
        chkIncPatientsWithEmail.Checked  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_IncPatientsWithEmail").Value)  == 1;

        chkSendMondays.Checked    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendMondays").Value)    == 1;
        chkSendTuesdays.Checked   = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendTuesdays").Value)   == 1;
        chkSendWednesdays.Checked = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendWednesdays").Value) == 1;
        chkSendThursdays.Checked  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendThursdays").Value)  == 1;
        chkSendFridays.Checked    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFridays").Value)    == 1;
        chkSendSaturdays.Checked  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendSaturdays").Value)  == 1;
        chkSendSundays.Checked    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendSundays").Value)    == 1;

        ddlFromDaysAheadMondays.SelectedValue     = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Mondays").Value;
        ddlUntilDaysAheadMondays.SelectedValue    = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Mondays").Value;
        ddlFromDaysAheadTuesdays.SelectedValue    = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Tuesdays").Value;
        ddlUntilDaysAheadTuesdays.SelectedValue   = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Tuesdays").Value;
        ddlFromDaysAheadWednesdays.SelectedValue  = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Wednesdays").Value;
        ddlUntilDaysAheadWednesdays.SelectedValue = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Wednesdays").Value;
        ddlFromDaysAheadThursdays.SelectedValue   = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Thursdays").Value;
        ddlUntilDaysAheadThursdays.SelectedValue  = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Thursdays").Value;
        ddlFromDaysAheadFridays.SelectedValue     = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Fridays").Value;
        ddlUntilDaysAheadFridays.SelectedValue    = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Fridays").Value;
        ddlFromDaysAheadSaturdays.SelectedValue   = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Saturdays").Value;
        ddlUntilDaysAheadSaturdays.SelectedValue  = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Saturdays").Value;
        ddlFromDaysAheadSundays.SelectedValue     = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Sundays").Value;
        ddlUntilDaysAheadSundays.SelectedValue    = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Sundays").Value;
        
        btnUpdateNotificationInfo.CssClass = "hiddencol";
        btnRevertNotificationInfo.CssClass = "hiddencol";
    }


    #endregion

    #region btnExport_Click

    protected void btnExport_Click(object sender, EventArgs e)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("\"" + "D.O.B."           + "\"").Append(",");
        sb.Append("\"" + "Name"             + "\"").Append(",");
        sb.Append("\"" + "Clinic Patient"   + "\"").Append(",");
        sb.Append("\"" + "Mobile"           + "\"").Append(",");
        sb.Append("\"" + "Email"            + "\"").Append(",");

        sb.AppendLine();


        DataTable dt = Session["data_bookingswithoutsmsoremail"] as DataTable;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("\"" + Convert.ToDateTime(dt.Rows[i]["dob"]).ToString("d MMMMM, yyyy")                    + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["firstname"].ToString() + " " + dt.Rows[i]["surname"].ToString() + (dt.Rows[i]["t_title_id"] == DBNull.Value || Convert.ToInt32(dt.Rows[i]["t_title_id"]) == 0 ? "" :  " ("+dt.Rows[i]["t_descr"]+")") + "\"").Append(",");
                sb.Append("\"" + (Convert.ToBoolean(dt.Rows[i]["is_clinic_patient"]) ? "Yes" : "No")                + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["mobile"].ToString()                                                    + "\"").Append(",");
                sb.Append("\"" + dt.Rows[i]["email"].ToString()                                                     + "\"").Append(",");
                sb.AppendLine();
            }
        }


        ExportCSV(Response, sb.ToString(), "Birthdays.csv");
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

    protected void btnTest_Click(object sender, EventArgs e)
    {
        try
        {

            bool   enableEmails           = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendEmail").Value) == 1;
            string emailAddress           = SystemVariableDB.GetByDescr("BirthdayNotificationEmail_EmailAddress").Value;

            bool   incPatientsWithMobile  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_IncPatientsWithMobile").Value) == 1;
            bool   incPatientsWithEmail   = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_IncPatientsWithEmail").Value)  == 1;

            bool   sendMondays            = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendMondays").Value)    == 1;
            bool   sendTuesdays           = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendTuesdays").Value)   == 1;
            bool   sendWednesdays         = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendWednesdays").Value) == 1;
            bool   sendThursdays          = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendThursdays").Value)  == 1;
            bool   sendFridays            = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFridays").Value)    == 1;
            bool   sendSaturdays          = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendSaturdays").Value)  == 1;
            bool   sendSundays            = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendSundays").Value)    == 1;

            int    fromDaysAheadMondays     = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Mondays").Value);
            int    untilDaysAheadMondays    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Mondays").Value);
            int    fromDaysAheadTuesdays    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Tuesdays").Value);
            int    untilDaysAheadTuesdays   = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Tuesdays").Value);
            int    fromDaysAheadWednesdays  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Wednesdays").Value);
            int    untilDaysAheadWednesdays = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Wednesdays").Value);
            int    fromDaysAheadThursdays   = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Thursdays").Value);
            int    untilDaysAheadThursdays  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Thursdays").Value);
            int    fromDaysAheadFridays     = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Fridays").Value);
            int    untilDaysAheadFridays    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Fridays").Value);
            int    fromDaysAheadSaturdays   = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Saturdays").Value);
            int    untilDaysAheadSaturdays  = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Saturdays").Value);
            int    fromDaysAheadSundays     = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendFromDaysAhead_Sundays").Value);
            int    untilDaysAheadSundays    = Convert.ToInt32(SystemVariableDB.GetByDescr("BirthdayNotificationEmail_SendUntilDaysAhead_Sundays").Value);

            int fromDaysAhead = 0, untilDaysAhead = 0;
            if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)    { fromDaysAhead = fromDaysAheadMondays;    untilDaysAhead = untilDaysAheadMondays;    }
            if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)   { fromDaysAhead = fromDaysAheadTuesdays;   untilDaysAhead = untilDaysAheadTuesdays;   }
            if (DateTime.Today.DayOfWeek == DayOfWeek.Wednesday) { fromDaysAhead = fromDaysAheadWednesdays; untilDaysAhead = untilDaysAheadWednesdays; }
            if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday)  { fromDaysAhead = fromDaysAheadThursdays;  untilDaysAhead = untilDaysAheadThursdays;  }
            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)    { fromDaysAhead = fromDaysAheadFridays;    untilDaysAhead = untilDaysAheadFridays;    }
            if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)  { fromDaysAhead = fromDaysAheadSaturdays;  untilDaysAhead = untilDaysAheadSaturdays;  }
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)    { fromDaysAhead = fromDaysAheadSundays;    untilDaysAhead = untilDaysAheadSundays;    }

            DateTime start = DateTime.Now.AddDays(fromDaysAhead);
            DateTime end   = DateTime.Now.AddDays(untilDaysAhead);


            if (!Utilities.IsValidEmailAddresses(emailAddress, false))
                throw new CustomMessageException("No emails will be sent in the test if you have not set an email address.");

            SmsAndEmailBirthdayMessages.RunBirthdaysWithoutSMSorEmail(false);
            SetErrorMessage("Test Run Completed.");
        }
        catch (CustomMessageException ex)
        {
            SetErrorMessage(ex.Message);
        }
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