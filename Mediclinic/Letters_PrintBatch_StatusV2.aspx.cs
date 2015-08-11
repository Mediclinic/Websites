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

public partial class Letters_PrintBatch_StatusV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);
                Session.Remove("lettersprintbatchstatus_sortexpression");
                Session.Remove("lettersprintbatchstatus_data");

                FillGrid();

                //txtSearchName.Focus();
            }


            this.GrdBulkLetterSendingQueue.EnableViewState = true;

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

    #region GetFormBatchID()

    protected bool IsValidFormBatchID()
    {
        string batch_id = Request.QueryString["batch_id"];
        return batch_id != null && Regex.IsMatch(batch_id, @"^\d+$");
    }
    protected int GetFormBatchID()
    {
        if (!IsValidFormBatchID())
            throw new Exception("Invalid batch id");

        return Convert.ToInt32(Request.QueryString["batch_id"]);
    }

    #endregion

    #region GrdBulkLetterSendingQueue

    protected void FillGrid()
    {
        int bulk_letter_sending_queue_batch_id = IsValidFormBatchID() ? GetFormBatchID() : -1;
        DataTable dt = BulkLetterSendingQueueDB.GetDataTable(bulk_letter_sending_queue_batch_id);


        // send method hashtable
        DataTable sendMethodTbl = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "LetterPrintHistorySendMethod", "", "", "letter_print_history_send_method_id", "descr");
        Hashtable sendMethodHash = new Hashtable();
        for (int i = 0; i < sendMethodTbl.Rows.Count; i++)
            sendMethodHash[Convert.ToInt32(sendMethodTbl.Rows[i]["letter_print_history_send_method_id"])] = (string)sendMethodTbl.Rows[i]["descr"];

        // patient hashtable
        ArrayList ptIDs = new ArrayList();
        for (int i = 0; i < dt.Rows.Count; i++)
            if (dt.Rows[i]["patient_id"] != DBNull.Value)
                ptIDs.Add((int)dt.Rows[i]["patient_id"]);
        Hashtable patientHash = PatientDB.GetByIDsInHashtable((int[])ptIDs.ToArray(typeof(int)));

        // staff hashtable
        Hashtable staffHash = StaffDB.GetAllInHashtable(true, true, true, false);

        // referrersHash
        Hashtable referrersHash = ReferrerDB.GetHashtableByReferrer();

        // letters hashtable
        Hashtable letterHash = LetterDB.GetHashTable();


        // add from hashtable
        dt.Columns.Add("letter_print_history_send_method_descr", typeof(String));
        dt.Columns.Add("added_by_name", typeof(String));
        dt.Columns.Add("patient_name", typeof(String));
        dt.Columns.Add("referrer_name", typeof(String));
        dt.Columns.Add("letter_doc_name", typeof(String));

        int SMSSent     = 0;
        int SMSUnSent   = 0;
        int EmailSent   = 0;
        int EmailUnSent = 0;
        int PrintSent   = 0;
        int PrintUnSent = 0;

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int  letter_print_history_send_method_id = Convert.ToInt32(dt.Rows[i]["letter_print_history_send_method_id"]);
            bool sent = dt.Rows[i]["datetime_sent"] != DBNull.Value;

            dt.Rows[i]["letter_print_history_send_method_descr"] = (string)sendMethodHash[letter_print_history_send_method_id];
            dt.Rows[i]["added_by_name"] = dt.Rows[i]["added_by"]   == DBNull.Value ? "" : ((Staff)staffHash[Convert.ToInt32(dt.Rows[i]["added_by"])]).Person.FullnameWithoutMiddlename;
            dt.Rows[i]["patient_name"]  = dt.Rows[i]["patient_id"] == DBNull.Value ? "" : ((Patient)patientHash[Convert.ToInt32(dt.Rows[i]["patient_id"])]).Person.FullnameWithoutMiddlename;

            dt.Rows[i]["referrer_name"] = dt.Rows[i]["referrer_id"] == DBNull.Value ? "" : ((Referrer)referrersHash[Convert.ToInt32(dt.Rows[i]["referrer_id"])]).Person.FullnameWithoutMiddlename;

            string source_template_path = dt.Rows[i]["email_letter_source_template_path"].ToString();
            string letter_doc_name      = source_template_path.Length == 0 ? "" : System.IO.Path.GetFileName(source_template_path);
            dt.Rows[i]["letter_doc_name"] = letter_doc_name;

            if (letter_print_history_send_method_id == 3 && sent)
                SMSSent++;
            if (letter_print_history_send_method_id == 3 && !sent)
                SMSUnSent++;
            if (letter_print_history_send_method_id == 2 && sent)
                EmailSent++;
            if (letter_print_history_send_method_id == 2 && !sent)
                EmailUnSent++;
            if (letter_print_history_send_method_id == 1 && sent)
                PrintSent++;
            if (letter_print_history_send_method_id == 1 && !sent)
                PrintUnSent++;
        }

        lblSMSSent.Text     = SMSSent.ToString();
        lblSMSUnSent.Text   = SMSUnSent.ToString();
        lblEmailSent.Text   = EmailSent.ToString();
        lblEmailUnSent.Text = EmailUnSent.ToString();
        lblPrintSent.Text   = PrintSent.ToString();
        lblPrintUnSent.Text = PrintUnSent.ToString();
        lblTotalSent.Text   = (SMSSent   + EmailSent   + PrintSent).ToString();
        lblTotalUnSent.Text = (SMSUnSent + EmailUnSent + PrintUnSent).ToString();


        DataView dv = new DataView(dt);
        dv.Sort = "letter_print_history_send_method_id";
        dt = dv.ToTable();


        Session["lettersprintbatchstatus_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["lettersprintbatchstatus_sortexpression"] != null && Session["lettersprintbatchstatus_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["lettersprintbatchstatus_sortexpression"].ToString();
                GrdBulkLetterSendingQueue.DataSource = dataView;
            }
            else
            {
                GrdBulkLetterSendingQueue.DataSource = dt;
            }

            GrdBulkLetterSendingQueue.AllowPaging = false;

            try
            {
                GrdBulkLetterSendingQueue.DataBind();
                GrdBulkLetterSendingQueue.PagerSettings.FirstPageText = "1";
                GrdBulkLetterSendingQueue.PagerSettings.LastPageText = GrdBulkLetterSendingQueue.PageCount.ToString();
                GrdBulkLetterSendingQueue.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdBulkLetterSendingQueue.DataSource = dt;
            GrdBulkLetterSendingQueue.DataBind();

            int TotalColumns = GrdBulkLetterSendingQueue.Rows[0].Cells.Count;
            GrdBulkLetterSendingQueue.Rows[0].Cells.Clear();
            GrdBulkLetterSendingQueue.Rows[0].Cells.Add(new TableCell());
            GrdBulkLetterSendingQueue.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdBulkLetterSendingQueue.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdBulkLetterSendingQueue_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdBulkLetterSendingQueue_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        //DataTable custTypes = DBBase.GetGenericDataTable("OrganisationCustomerType", "organisation_customer_type_id", "descr");
        DataTable dt = Session["lettersprintbatchstatus_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            bool hasData = dt.Rows[0][0].ToString() != string.Empty;
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("bulk_letter_sending_queue_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];



            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

        }
    }
    protected void GrdBulkLetterSendingQueue_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdBulkLetterSendingQueue.EditIndex = -1;
        FillGrid();
    }
    protected void GrdBulkLetterSendingQueue_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdBulkLetterSendingQueue_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdBulkLetterSendingQueue_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdBulkLetterSendingQueue_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdBulkLetterSendingQueue.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdBulkLetterSendingQueue.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }
    protected void GrdBulkLetterSendingQueue_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdBulkLetterSendingQueue.PageIndex = e.NewPageIndex;
        FillGrid();
    }
    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["lettersprintbatchstatus_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["lettersprintbatchstatus_sortexpression"] == null)
                Session["lettersprintbatchstatus_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["lettersprintbatchstatus_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["lettersprintbatchstatus_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdBulkLetterSendingQueue.DataSource = dataView;
            GrdBulkLetterSendingQueue.DataBind();
        }
    }

    #endregion

    #region chkUsePaging_CheckedChanged, chkShowDeleted_CheckedChanged

    protected void chkUsePaging_CheckedChanged(object sender, EventArgs e)
    {
        this.GrdBulkLetterSendingQueue.AllowPaging = chkUsePaging.Checked;
        FillGrid();
    }
    protected void chkShowDeleted_CheckedChanged(object sender, EventArgs e)
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