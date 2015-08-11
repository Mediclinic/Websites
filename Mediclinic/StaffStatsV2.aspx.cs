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

public partial class StaffStatsV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, true, false, false, true);
                SetupGUI();
                FillForm();
            }

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
        DateTime startOfThisMonth = DateTime.Now.AddDays((-1*DateTime.Now.Day) + 1);
        DateTime endOfThisMonth   = startOfThisMonth.AddMonths(1).AddDays(-1);

        txtStartDate.Text = startOfThisMonth.ToString("dd-MM-yyyy");
        txtEndDate.Text   = endOfThisMonth.ToString("dd-MM-yyyy");

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";
    }

    #endregion

    #region IsValidFormID(), GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$") && StaffDB.Exists(Convert.ToInt32(id));
    }
    private int GetFormID(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormID())
            throw new Exception("Invalid url id");
        return Convert.ToInt32(Request.QueryString["id"]);
    }

    #endregion

    #region FillForm

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        FillForm();
    }

    protected void FillForm()
    {

        try
        {
            if (!CheckIsValidStartEndDates())
                return;

            DataTable tblStats = StaffDB.GetStats2(GetFromDate(), GetToDate(), chkIncDeleted.Checked, chkIncDeleted.Checked);

            tblStats.Columns.Add("total_owing", typeof(decimal));
            tblStats.Columns.Add("total_in", typeof(decimal));
            for (int i = 0; i < tblStats.Rows.Count; i++)
            {
                decimal sum_inv_total = tblStats.Rows[i]["sum_inv_total"] == DBNull.Value ? 0 : Convert.ToDecimal(tblStats.Rows[i]["sum_inv_total"]);
                decimal sum_receipts  = tblStats.Rows[i]["sum_receipts"]  == DBNull.Value ? 0 : Convert.ToDecimal(tblStats.Rows[i]["sum_receipts"]);
                decimal sum_cr_notes  = tblStats.Rows[i]["sum_cr_notes"]  == DBNull.Value ? 0 : Convert.ToDecimal(tblStats.Rows[i]["sum_cr_notes"]);
                tblStats.Rows[i]["total_owing"] = sum_inv_total - sum_receipts - sum_cr_notes;
                tblStats.Rows[i]["total_in"]    = sum_inv_total - sum_cr_notes;

                if (Convert.ToDecimal(tblStats.Rows[i]["total_owing"]) == 0)
                    tblStats.Rows[i]["total_owing"] = DBNull.Value;
                if (Convert.ToDecimal(tblStats.Rows[i]["sum_receipts"]) == 0)
                    tblStats.Rows[i]["sum_receipts"] = DBNull.Value;
                if (Convert.ToDecimal(tblStats.Rows[i]["total_in"]) == 0)
                    tblStats.Rows[i]["total_in"] = DBNull.Value;

            }

            lstStaffStats.DataSource = tblStats;
            lstStaffStats.DataBind();

            // get from footer
            Label lblSum_TotalBookings  = (Label)lstStaffStats.Controls[lstStaffStats.Controls.Count - 1].Controls[0].FindControl("lblSum_TotalBookings");
            Label lblSum_AvgConsultTime = (Label)lstStaffStats.Controls[lstStaffStats.Controls.Count - 1].Controls[0].FindControl("lblSum_AvgConsultTime");
            Label lblSum_Bookings       = (Label)lstStaffStats.Controls[lstStaffStats.Controls.Count - 1].Controls[0].FindControl("lblSum_Bookings");
            Label lblSum_Completions    = (Label)lstStaffStats.Controls[lstStaffStats.Controls.Count - 1].Controls[0].FindControl("lblSum_Completions");
            Label lblSum_Receipts       = (Label)lstStaffStats.Controls[lstStaffStats.Controls.Count - 1].Controls[0].FindControl("lblSum_Receipts");
            Label lblSum_Owing          = (Label)lstStaffStats.Controls[lstStaffStats.Controls.Count - 1].Controls[0].FindControl("lblSum_Owing");
            Label lblSum_total          = (Label)lstStaffStats.Controls[lstStaffStats.Controls.Count - 1].Controls[0].FindControl("lblSum_total");
            Label lblSum_Patients       = (Label)lstStaffStats.Controls[lstStaffStats.Controls.Count - 1].Controls[0].FindControl("lblSum_Patients");


            lblSum_TotalBookings.Text = String.Format("{0:n0}", tblStats.Compute("Sum(total_bookings)", ""));

            decimal total = 0;
            int     count = 0;
            for (int i = 0; i < tblStats.Rows.Count; i++)
            {
                if (tblStats.Rows[i]["avg_minutes"] == DBNull.Value)
                    continue;
                total += Convert.ToDecimal(tblStats.Rows[i]["avg_minutes"]);
                count++;
            }
            if (count > 0)
                lblSum_AvgConsultTime.Text = String.Format("{0:n0}", total / count);

            lblSum_Bookings.Text        = String.Format("{0:n0}", tblStats.Compute("Sum(n_bookings)", ""));
            lblSum_Completions.Text     = String.Format("{0:n0}", tblStats.Compute("Sum(n_completions)", ""));

            object o_sum_receipts = tblStats.Compute("Sum(sum_receipts)", "");
            lblSum_Receipts.Text  = String.Format("{0:C}", o_sum_receipts == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(o_sum_receipts));
            object o_sum_owing    = tblStats.Compute("Sum(total_owing)", "");
            lblSum_Owing.Text     = String.Format("{0:C}", o_sum_owing == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(o_sum_owing));
            object o_sum_total    = tblStats.Compute("Sum(total_in)", "");
            lblSum_total.Text     = String.Format("{0:C}", o_sum_total == DBNull.Value ? (object)DBNull.Value : Convert.ToDecimal(o_sum_total));

            lblSum_Patients.Text  = tblStats.Compute("Sum(n_patients)", "").ToString();
        }
        catch (CustomMessageException cmEx)
        {
            HideTableAndSetErrorMessage(cmEx.Message);
        }
        catch (Exception ex)
        {
            HideTableAndSetErrorMessage(ex.ToString());
        }

    }

    #endregion

    #region CheckIsValidStartEndDates, GetFromDate, GetToDate

    protected bool CheckIsValidStartEndDates()
    {
        try
        {
            if (txtStartDate.Text.Length > 0 && !Utilities.IsValidDate(txtStartDate.Text, "dd-mm-yyyy"))
                throw new CustomMessageException("Invalid start date");
            if (txtEndDate.Text.Length > 0 && !Utilities.IsValidDate(txtEndDate.Text, "dd-mm-yyyy"))
                throw new CustomMessageException("Invalid end date");

            return true;
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return false;
        }
    }
    protected DateTime GetFromDate()
    {
        return txtStartDate.Text.Length > 0 ? Utilities.GetDate(txtStartDate.Text, "dd-mm-yyyy") : DateTime.MinValue;
    }
    protected DateTime GetToDate()
    {
        return txtEndDate.Text.Length > 0 ? Utilities.GetDate(txtEndDate.Text, "dd-mm-yyyy").Add(new TimeSpan(23, 59, 59)) : DateTime.MinValue;
    }

    #endregion


    protected DataTable GetPatientDataTable(int staff_id)
    {
        Staff staff = StaffDB.GetByID(staff_id);

        Hashtable staffHashOriginal = StaffDB.GetAllInHashtable(true, true, true, false);
        Hashtable staffHash = new Hashtable();
        foreach(Staff s in staffHashOriginal.Values)
            staffHash[s.Person.PersonID] = s;

        DataTable tbl = PatientDB.GetPatientsAddedByStaff(staff_id, GetFromDate(), GetToDate());


        // sort by most common referrer
        tbl.Columns.Add("referrer_count", typeof(int));
        tbl.Columns.Add("added_by_name", typeof(String));
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int refCount = 0;
            if (tbl.Rows[i]["referrer_info_referrer_id"] != DBNull.Value)
            {
                for (int j = 0; j < tbl.Rows.Count; j++)
                    if (tbl.Rows[j]["referrer_info_referrer_id"] != DBNull.Value && Convert.ToInt32(tbl.Rows[j]["referrer_info_referrer_id"]) == Convert.ToInt32(tbl.Rows[i]["referrer_info_referrer_id"]))
                        refCount++;
            }

            tbl.Rows[i]["referrer_count"] = refCount;
            tbl.Rows[i]["added_by_name"] = staff.Person.FullnameWithoutMiddlename;
        }
        tbl.DefaultView.Sort = "referrer_count DESC, referrer_info_surname, referrer_info_firstname, surname, firstname, middlename";
        tbl = tbl.DefaultView.ToTable();

        return tbl;
    }


    protected void lnkPatients_Command(object sender, CommandEventArgs e)
    {
        int staff_id = Convert.ToInt32(e.CommandArgument);

        DataTable tbl = GetPatientDataTable(staff_id);
        lstPatients.DataSource = tbl;
        lstPatients.DataBind();

        btnExport.Visible = tbl.Rows.Count > 0;
        btnExport.CommandArgument = staff_id.ToString();
    }


    #region Export

    protected void btnExport_Command(object sender, CommandEventArgs e)
    {
        /*
        int organisation_id = Convert.ToInt32(e.CommandArgument);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        DataTable dt = GetPatientDataTable(organisation_id);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append(dt.Rows[i]["referrer_info_firstname"].ToString() + " " + dt.Rows[i]["referrer_info_surname"].ToString()).Append(",");
            sb.Append(dt.Rows[i]["firstname"].ToString() + " " + dt.Rows[i]["surname"].ToString()).Append(",");
            sb.Append(dt.Rows[i]["organisation_name"].ToString());
            sb.AppendLine();
        }

        ExportCSV(Response, sb.ToString(), "new_patients_export.csv");
        */
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