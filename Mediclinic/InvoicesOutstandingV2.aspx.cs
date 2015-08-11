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

public partial class InvoicesOutstandingV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);
                Session.Remove("ptinvoicesoutstanding_sortexpression");
                Session.Remove("ptinvoicesoutstanding_data");
                Session.Remove("orginvoicesoutstanding_sortexpression");
                Session.Remove("orginvoicesoutstanding_data");

                UserView userView = UserView.GetInstance();

                for (int i = 0; i < GrdPtInvoicesOutstanding.Columns.Count; i++)
                    if (GrdPtInvoicesOutstanding.Columns[i].HeaderText == "Organisation")
                        GrdPtInvoicesOutstanding.Columns[i].HeaderText = userView.IsAgedCareView ? "Facility" : "Clinic";

                GrdPtInvoicesOutstanding_FillGrid();
                lblPtHeading.Text  = userView.IsAgedCareView ? "Outstanding By Resident&nbsp;&nbsp;&nbsp;&nbsp;" : "Outstanding By Patient&nbsp;&nbsp;&nbsp;&nbsp;";
                lblFacHeading.Text = userView.IsAgedCareView ? "Outstanding By Facility&nbsp;&nbsp;&nbsp;&nbsp;" : "Outstanding By Insurance Co.&nbsp;&nbsp;&nbsp;&nbsp;";

                if (userView.IsAgedCareView)
                {
                    GrdOrgInvoicesOutstanding_FillGrid();
                }
                else
                {
                    GrdOrgInvoicesOutstanding_FillGrid();

                    //GrdOrgInvoicesOutstanding.Visible = false;
                    //lblPtHeading.Visible     = false;
                    //lblFacHeading.Visible    = false;
                    //btnPrintAllFacs.Visible  = false;
                    //btnEmailAllFacs.Visible  = false;
                    //btnExportAllFacs.Visible = false;
                }
            }

            if (!Utilities.IsDev())
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);
            }

            this.GrdPtInvoicesOutstanding.EnableViewState = true;
            this.GrdOrgInvoicesOutstanding.EnableViewState = true;

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

    #region GrdPtInvoicesOutstanding

    protected void GrdPtInvoicesOutstanding_FillGrid()
    {
        DataTable dt = InvoiceDB.GetAllOutstandingByPatientAsReport(Convert.ToInt32(Session["SiteID"]));
        dt.DefaultView.Sort = "patient_surname";
        dt = dt.DefaultView.ToTable();
        Session["ptinvoicesoutstanding_data"] = dt;

        btnExportAllPatients.Visible = dt.Rows.Count > 0;
        btnPrintAllPatients.Visible  = dt.Rows.Count > 0;

        if (dt.Rows.Count > 0)
        {
            GrdPtInvoicesOutstanding.DataSource = dt;
            try
            {
                GrdPtInvoicesOutstanding.DataBind();
                GrdPtInvoicesOutstanding.PagerSettings.FirstPageText = "1";
                GrdPtInvoicesOutstanding.PagerSettings.LastPageText = GrdPtInvoicesOutstanding.PageCount.ToString();
                GrdPtInvoicesOutstanding.DataBind();

            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdPtInvoicesOutstanding.DataSource = dt;
            GrdPtInvoicesOutstanding.DataBind();

            int TotalColumns = GrdPtInvoicesOutstanding.Rows[0].Cells.Count;
            GrdPtInvoicesOutstanding.Rows[0].Cells.Clear();
            GrdPtInvoicesOutstanding.Rows[0].Cells.Add(new TableCell());
            GrdPtInvoicesOutstanding.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdPtInvoicesOutstanding.Rows[0].Cells[0].Text = "No Record Found";
        }

        btnEmailAllPatients.Attributes["onclick"] = "if (!confirm('Estimating 20 seconds per email for " + dt.Rows.Count + " patients, you will need to let this page run for about " + (dt.Rows.Count/3) +" Minutes. \\r\\nAre you sure you want to continue?')) return false;";
    }
    protected void GrdPtInvoicesOutstanding_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdPtInvoicesOutstanding_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["ptinvoicesoutstanding_data"] as DataTable;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblTotalSumDue = (Label)e.Row.FindControl("lblTotalSumDue");
            lblTotalSumDue.Text = String.Format("{0:C}", dt.Compute("Sum(total_due)", ""));
            if (lblTotalSumDue.Text == "") lblTotalSumDue.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";
        }
    }
    protected void GrdPtInvoicesOutstanding_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdPtInvoicesOutstanding.EditIndex = -1;
        GrdPtInvoicesOutstanding_FillGrid();
    }
    protected void GrdPtInvoicesOutstanding_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdPtInvoicesOutstanding_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdPtInvoicesOutstanding_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Email")
        {
            int patientID = Convert.ToInt32(e.CommandArgument);
            Patient patient = PatientDB.GetByID(patientID);

            string[] emails = ContactDB.GetEmailsByEntityID(patient.Person.EntityID);

            if (emails.Length == 0)
            {
                SetErrorMessage("No email address set for '" + patient.Person.FullnameWithoutMiddlename + "'. Please set one to email invoices to them.");
                return;
            }
            else
            {
                DataTable dt = Session["ptinvoicesoutstanding_data"] as DataTable;
                DataRow row = dt.Select("patient_id = " + patientID)[0];

                string invoiceIDsCommaSep = (string)row["invoice_ids_comma_sep"];
                int[] invoiceIDs = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);


                string tmpLettersDirectory = Letter.GetTempLettersDirectory();
                string originalFile = Letter.GetLettersDirectory() + (!UserView.GetInstance().IsAgedCareView ? @"OverdueInvoiceTemplate.docx" : @"OverdueInvoiceTemplateAC.docx");
                string tmpDir = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
                System.IO.Directory.CreateDirectory(tmpDir);
                string outputFile = tmpDir + "OverdueInvoices.pdf";


                try
                {
                    Letter.GenerateOutstandingInvoices(originalFile, outputFile, invoiceIDs, patientID, -1);

                    EmailerNew.SimpleEmail(
                        ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                        ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                        string.Join(",", emails),
                        "Overdue Invoices",
                        "Pease find your Invoices attached. <br/>Please call us if you do not agree with the Invoice amount stated.<br /><br />Thank you.",
                        true,
                        new string[] { outputFile },
                        false,
                        null
                        );

                    SetErrorMessage("Invoices Emailed to " + patient.Person.FullnameWithoutMiddlename + " (" + string.Join(", ", emails) + ")");
                }
                catch (CustomMessageException cmEx)
                {
                    SetErrorMessage(cmEx.Message);
                }
                catch (Exception ex)
                {
                    SetErrorMessage("", ex.ToString());
                }
                finally
                {
                    try { if (System.IO.File.Exists(outputFile)) System.IO.File.Delete(outputFile); }
                    catch (Exception) { }

                    // delete temp dir
                    if (tmpDir != null)
                    {

                        try { System.IO.Directory.Delete(tmpDir, true); }
                        catch (Exception) { }
                    }
                }
            }
        }

        if (e.CommandName == "Print")
        {
            int patientID = Convert.ToInt32(e.CommandArgument);

            DataTable dt = Session["ptinvoicesoutstanding_data"] as DataTable;
            DataRow row = dt.Select("patient_id = " + patientID)[0];

            SetErrorMessage("PT ID: " + row["patient_id"] + "<br />Invoices: " + row["invoice_ids_comma_sep"]);

            string invoiceIDsCommaSep = (string)row["invoice_ids_comma_sep"];
            int[] invoiceIDs = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);

            Letter.GenerateOutstandingInvoicesToPrint(Response, invoiceIDs, patientID, -1, UserView.GetInstance().IsClinicView);
        }

        if (e.CommandName == "SetAllPaid" || e.CommandName == "SetAllWiped")
        {

            try
            {

                int patientID = Convert.ToInt32(e.CommandArgument);

                DataTable dt = Session["ptinvoicesoutstanding_data"] as DataTable;
                DataRow row = dt.Select("patient_id = " + patientID)[0];

                string invoiceIDsCommaSep = (string)row["invoice_ids_comma_sep"];
                int[] invoiceIDs = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);


                foreach (int invoiceID in invoiceIDs)
                {
                    Invoice invoice = InvoiceDB.GetByID(invoiceID);
                    if (invoice == null || invoice.IsPaID)
                        continue;

                    if (e.CommandName.Equals("SetAllPaid"))
                    {
                        ReceiptDB.Insert(null, 362, invoice.InvoiceID, invoice.TotalDue, Convert.ToDecimal(0.00), false, false, DateTime.MinValue, Convert.ToInt32(Session["StaffID"]));
                        InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);
                    }
                    else if (e.CommandName.Equals("SetAllWiped"))
                    {
                        CreditNoteDB.Insert(invoice.InvoiceID, invoice.TotalDue, string.Empty, Convert.ToInt32(Session["StaffID"]));
                        InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);
                    }
                }

                SetErrorMessage("Invoices Set As " + (e.CommandName.Equals("SetAllPaid") ? "Paid" : "Wiped") + " : " + row["invoice_ids_comma_sep"]);

                GrdPtInvoicesOutstanding_FillGrid();

            }
            catch (CustomMessageException cmEx)
            {
                SetErrorMessage(cmEx.Message);
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }

        }

    }
    protected void GrdPtInvoicesOutstanding_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdPtInvoicesOutstanding.EditIndex = e.NewEditIndex;
        GrdPtInvoicesOutstanding_FillGrid();
    }
    protected void GrdPtInvoicesOutstanding_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdPtInvoicesOutstanding.EditIndex >= 0)
            return;

        GrdPtInvoicesOutstanding_Sort(e.SortExpression);
    }
    protected void GrdPtInvoicesOutstanding_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdPtInvoicesOutstanding.PageIndex = e.NewPageIndex;
        GrdPtInvoicesOutstanding_FillGrid();
    }
    protected void GrdPtInvoicesOutstanding_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["ptinvoicesoutstanding_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["ptinvoicesoutstanding_sortexpression"] == null)
                Session["ptinvoicesoutstanding_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["ptinvoicesoutstanding_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["ptinvoicesoutstanding_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdPtInvoicesOutstanding.DataSource = dataView;
            GrdPtInvoicesOutstanding.DataBind();
        }
    }

    #endregion

    #region GrdOrgInvoicesOutstanding

    protected void GrdOrgInvoicesOutstanding_FillGrid()
    {

        bool insuranceCompanies = !UserView.GetInstance().IsAgedCareView;
        DataTable dt = InvoiceDB.GetAllOutstandingByOrgAsReport(Convert.ToInt32(Session["SiteID"]), 0, insuranceCompanies);

        dt.DefaultView.Sort = "name";
        dt = dt.DefaultView.ToTable();
        Session["orginvoicesoutstanding_data"] = dt;

        btnExportAllFacs.Visible = dt.Rows.Count > 0;
        btnPrintAllFacs.Visible  = dt.Rows.Count > 0;

        if (dt.Rows.Count > 0)
        {
            GrdOrgInvoicesOutstanding.DataSource = dt;
            try
            {
                GrdOrgInvoicesOutstanding.DataBind();
                GrdOrgInvoicesOutstanding.PagerSettings.FirstPageText = "1";
                GrdOrgInvoicesOutstanding.PagerSettings.LastPageText = GrdOrgInvoicesOutstanding.PageCount.ToString();
                GrdOrgInvoicesOutstanding.DataBind();

            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdOrgInvoicesOutstanding.DataSource = dt;
            GrdOrgInvoicesOutstanding.DataBind();

            int TotalColumns = GrdOrgInvoicesOutstanding.Rows[0].Cells.Count;
            GrdOrgInvoicesOutstanding.Rows[0].Cells.Clear();
            GrdOrgInvoicesOutstanding.Rows[0].Cells.Add(new TableCell());
            GrdOrgInvoicesOutstanding.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdOrgInvoicesOutstanding.Rows[0].Cells[0].Text = "No Record Found";
        }

        btnEmailAllFacs.Attributes["onclick"] = "if (!confirm('Estimating 20 seconds per email for " + dt.Rows.Count + " facilities, you will need to let this page run for about " + (dt.Rows.Count / 3) + " Minutes. \\r\\nAre you sure you want to continue?')) return false;";
    }
    protected void GrdOrgInvoicesOutstanding_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdOrgInvoicesOutstanding_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["orginvoicesoutstanding_data"] as DataTable;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblTotalSumDue = (Label)e.Row.FindControl("lblTotalSumDue");
            lblTotalSumDue.Text = String.Format("{0:C}", dt.Compute("Sum(total_due)", ""));
            if (lblTotalSumDue.Text == "") lblTotalSumDue.Text = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";
        }
    }
    protected void GrdOrgInvoicesOutstanding_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdOrgInvoicesOutstanding.EditIndex = -1;
        GrdOrgInvoicesOutstanding_FillGrid();
    }
    protected void GrdOrgInvoicesOutstanding_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdOrgInvoicesOutstanding_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdOrgInvoicesOutstanding_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Email")
        {
            int          organisationID = Convert.ToInt32(e.CommandArgument);
            Organisation org            = OrganisationDB.GetByID(organisationID);
            string[]     emails         = ContactDB.GetEmailsByEntityID(org.EntityID);

            if (emails.Length == 0)
            {
                SetErrorMessage("No email address set for '" + org.Name + "'. Please set one to email invoices to them.");
                return;
            }
            else
            {
                DataTable dt = Session["orginvoicesoutstanding_data"] as DataTable;
                DataRow row = dt.Select("organisation_id = " + organisationID)[0];

                string invoiceIDsCommaSep = (string)row["invoice_ids_comma_sep"];
                int[] invoiceIDs = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);


                string tmpLettersDirectory = Letter.GetTempLettersDirectory();
                string originalFile        = Letter.GetLettersDirectory() + (!UserView.GetInstance().IsAgedCareView ? @"OverdueInvoiceTemplate.docx" : @"OverdueInvoiceTemplateAC.docx");
                string tmpDir              = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
                System.IO.Directory.CreateDirectory(tmpDir);
                string outputFile          = tmpDir + "OverdueInvoices.pdf";


                try
                {
                    Letter.GenerateOutstandingInvoices(originalFile, outputFile, invoiceIDs, -1, organisationID);

                    EmailerNew.SimpleEmail(
                        ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                        ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                        string.Join(",", emails),
                        "Overdue Invoices",
                        "Pease find your Invoices attached. <br/>Please call us if you do not agree with the Invoice amount stated.<br /><br />Thank you.",
                        true,
                        new string[]{ outputFile },
                        false,
                        null
                        );

                    SetErrorMessage("Invoices Emailed to " + org.Name + " (" + string.Join(",", emails) + ")");
                }
                catch (CustomMessageException cmEx)
                {
                    SetErrorMessage(cmEx.Message);
                }
                catch (Exception ex)
                {
                    SetErrorMessage("", ex.ToString());
                }
                finally
                {
                    try { if (System.IO.File.Exists(outputFile)) System.IO.File.Delete(outputFile); }
                    catch (Exception) { }

                    // delete temp dir
                    if (tmpDir != null)
                    {

                        try { System.IO.Directory.Delete(tmpDir, true); }
                        catch (Exception) { }
                    }
                }
            }
        }

        if (e.CommandName == "Print")
        {
            int organisationID = Convert.ToInt32(e.CommandArgument);

            DataTable dt = Session["orginvoicesoutstanding_data"] as DataTable;
            DataRow row = dt.Select("organisation_id = " + organisationID)[0];

            SetErrorMessage("Org ID: " + row["organisation_id"] + "<br />Invoices: " + row["invoice_ids_comma_sep"]);

            string invoiceIDsCommaSep = (string)row["invoice_ids_comma_sep"];
            int[] invoiceIDs = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);

            Letter.GenerateOutstandingInvoicesToPrint(Response, invoiceIDs, -1, organisationID, !UserView.GetInstance().IsAgedCareView);
        }

        if (e.CommandName == "SetAllPaid" || e.CommandName == "SetAllWiped")
        {

            try
            {

                int organisationID = Convert.ToInt32(e.CommandArgument);

                DataTable dt = Session["orginvoicesoutstanding_data"] as DataTable;
                DataRow row = dt.Select("organisation_id = " + organisationID)[0];

                string invoiceIDsCommaSep = (string)row["invoice_ids_comma_sep"];
                int[] invoiceIDs = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);


                foreach (int invoiceID in invoiceIDs)
                {
                    Invoice invoice = InvoiceDB.GetByID(invoiceID);
                    if (invoice == null || invoice.IsPaID)
                        continue;

                    if (e.CommandName.Equals("SetAllPaid"))
                    {
                        ReceiptDB.Insert(null, 362, invoice.InvoiceID, invoice.TotalDue, Convert.ToDecimal(0.00), false, false, DateTime.MinValue, Convert.ToInt32(Session["StaffID"]));
                        InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);
                    }
                    else if (e.CommandName.Equals("SetAllWiped"))
                    {
                        CreditNoteDB.Insert(invoice.InvoiceID, invoice.TotalDue, string.Empty, Convert.ToInt32(Session["StaffID"]));
                        InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);
                    }
                }

                SetErrorMessage("Invoices Set As " + (e.CommandName.Equals("SetAllPaid") ? "Paid" : "Wiped") + " : " + row["invoice_ids_comma_sep"]);

                GrdOrgInvoicesOutstanding_FillGrid();

            }
            catch (CustomMessageException cmEx)
            {
                SetErrorMessage(cmEx.Message);
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }

        }

    }
    protected void GrdOrgInvoicesOutstanding_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdOrgInvoicesOutstanding.EditIndex = e.NewEditIndex;
        GrdOrgInvoicesOutstanding_FillGrid();
    }
    protected void GrdOrgInvoicesOutstanding_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdOrgInvoicesOutstanding.EditIndex >= 0)
            return;

        GrdOrgInvoicesOutstanding_Sort(e.SortExpression);
    }
    protected void GrdOrgInvoicesOutstanding_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdOrgInvoicesOutstanding.PageIndex = e.NewPageIndex;
        GrdOrgInvoicesOutstanding_FillGrid();
    }
    protected void GrdOrgInvoicesOutstanding_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["orginvoicesoutstanding_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["orginvoicesoutstanding_sortexpression"] == null)
                Session["orginvoicesoutstanding_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["orginvoicesoutstanding_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["orginvoicesoutstanding_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdOrgInvoicesOutstanding.DataSource = dataView;
            GrdOrgInvoicesOutstanding.DataBind();
        }
    }

    #endregion

    #region btnPrintAllPatients_Command, btnPrintAllFacs_Command

    protected void btnPrintAllPatients_Command(object sender, CommandEventArgs e)
    {
        DataTable dt = Session["ptinvoicesoutstanding_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);

        if (tblEmpty)
            dt = dt.Clone(); // copy schema only to have empty table

        Tuple<int[], int, int>[] list = new Tuple<int[],int,int>[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int    patientID          = Convert.ToInt32(dt.Rows[i]["patient_id"]);
            string invoiceIDsCommaSep = (string)dt.Rows[i]["invoice_ids_comma_sep"];
            int[]  invoiceIDs         = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);

            list[i] = new Tuple<int[], int, int>(invoiceIDs, patientID, -1);
        }

        Letter.GenerateOutstandingInvoicesToPrint_Multiple(Response, list, !UserView.GetInstance().IsAgedCareView);
    }
    protected void btnPrintAllFacs_Command(object sender, CommandEventArgs e)
    {
        DataTable dt = Session["orginvoicesoutstanding_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);

        if (tblEmpty)
            dt = dt.Clone(); // copy schema only to have empty table

        Tuple<int[], int, int>[] list = new Tuple<int[],int,int>[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int    organisationID     = Convert.ToInt32(dt.Rows[i]["organisation_id"]);
            string invoiceIDsCommaSep = (string)dt.Rows[i]["invoice_ids_comma_sep"];
            int[]  invoiceIDs         = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);

            list[i] = new Tuple<int[], int, int>(invoiceIDs, -1, organisationID);
        }

        Letter.GenerateOutstandingInvoicesToPrint_Multiple(Response, list, !UserView.GetInstance().IsAgedCareView);
    }

    protected void btnEmailAllPatients_Command(object sender, CommandEventArgs e)
    {
        DataTable dt = Session["ptinvoicesoutstanding_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);

        if (tblEmpty)
            dt = dt.Clone(); // copy schema only to have empty table

        Tuple<int[], int, int>[] list = new Tuple<int[],int,int>[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int    patientID          = Convert.ToInt32(dt.Rows[i]["patient_id"]);
            string invoiceIDsCommaSep = (string)dt.Rows[i]["invoice_ids_comma_sep"];
            int[]  invoiceIDs         = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);

            list[i] = new Tuple<int[], int, int>(invoiceIDs, patientID, -1);
        }

        if (list.Length == 0)
            SetErrorMessage("No patients to send to.");
        else
        {
            Tuple<int, int> results = Letter.GenerateOutstandingInvoicesToEmail_Multiple(list, !UserView.GetInstance().IsAgedCareView);
            SetErrorMessage(results.Item1 + " of " + results.Item2 + " patients have email set and have been emailed their invoices");
        }
    }
    protected void btnEmailAllFacs_Command(object sender, CommandEventArgs e)
    {
        DataTable dt = Session["orginvoicesoutstanding_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);

        if (tblEmpty)
            dt = dt.Clone(); // copy schema only to have empty table

        Tuple<int[], int, int>[] list = new Tuple<int[],int,int>[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int    organisationID     = Convert.ToInt32(dt.Rows[i]["organisation_id"]);
            string invoiceIDsCommaSep = (string)dt.Rows[i]["invoice_ids_comma_sep"];
            int[]  invoiceIDs         = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);

            list[i] = new Tuple<int[], int, int>(invoiceIDs, -1, organisationID);
        }

        if (list.Length == 0)
            SetErrorMessage("No faciliites to send to.");
        else
        {
            Tuple<int, int> results = Letter.GenerateOutstandingInvoicesToEmail_Multiple(list, !UserView.GetInstance().IsAgedCareView);
            SetErrorMessage(results.Item1 + " of " + results.Item2 + " facilities have email set and have been emailed their invoices");
        }
    }

    

    #endregion

    #region btnExportAllPatients_Command, btnExportAllFacs_Command

    protected void btnExportAllPatients_Command(object sender, CommandEventArgs e)
    {
        System.Text.StringBuilder exportOutput = new System.Text.StringBuilder();
        exportOutput.Append("Firstname").Append(",");
        exportOutput.Append("Surname").Append(",");
        exportOutput.Append("Total Due (Count)").Append(",");
        exportOutput.Append("Invoice #").Append(",");
        exportOutput.Append("Treated").Append(",");
        exportOutput.Append("Clinic").Append(",");
        exportOutput.Append("Total").Append(",");
        exportOutput.Append("Owing").Append(",");
        exportOutput.AppendLine();
        exportOutput.AppendLine();

        DataTable dt = Session["ptinvoicesoutstanding_data"] as DataTable;

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            exportOutput.Append(dt.Rows[i]["patient_firstname"]).Append(",");
            exportOutput.Append(dt.Rows[i]["patient_surname"]).Append(",");
            string totalDue = string.Format("{0:C}", Convert.ToDecimal(dt.Rows[i]["total_due"] == DBNull.Value ? 0 : dt.Rows[i]["total_due"]));
            exportOutput.Append("\"" + totalDue + " (" + dt.Rows[i]["total_inv_count"] + ")" + "\"").Append(",");
            exportOutput.Append("\"" + dt.Rows[i]["invoice_ids"].ToString().Replace("<br />", Environment.NewLine) + "\"").Append(",");
            exportOutput.Append("\"" + dt.Rows[i]["bk_treatement_dates"].ToString().Replace("<br />", Environment.NewLine) + "\"").Append(",");
            exportOutput.Append("\"" + dt.Rows[i]["bk_orgs"].ToString().Replace("<br />", Environment.NewLine) + "\"").Append(",");
            exportOutput.Append("\"" + dt.Rows[i]["bk_totals"].ToString().Replace("<br />", Environment.NewLine) + "\"").Append(",");
            exportOutput.Append("\"" + dt.Rows[i]["bk_owings"].ToString().Replace("<br />", Environment.NewLine) + "\"").Append(",");
            exportOutput.AppendLine();
        }

        exportOutput.AppendLine();

        exportOutput.Append("").Append(",");
        exportOutput.Append("").Append(",");
        string totalSumDue = String.Format("{0:C}", dt.Compute("Sum(total_due)", ""));
        if (totalSumDue == "") totalSumDue = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";
        exportOutput.Append("\"" + totalSumDue + "\"").Append(",");
        exportOutput.Append("").Append(",");
        exportOutput.Append("").Append(",");
        exportOutput.Append("").Append(",");
        exportOutput.Append("").Append(",");
        exportOutput.Append("").Append(",");
        exportOutput.AppendLine();

        ExportCSV(Response, exportOutput.ToString(), "Outstanding PT Invoices.csv");
    }

    protected void btnExportAllFacs_Command(object sender, CommandEventArgs e)
    {
        System.Text.StringBuilder exportOutput = new System.Text.StringBuilder();
        exportOutput.Append("Name").Append(",");
        exportOutput.Append("Total Due (Count)").Append(",");
        exportOutput.AppendLine();
        exportOutput.AppendLine();

        DataTable dt = Session["orginvoicesoutstanding_data"] as DataTable;

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            exportOutput.Append(dt.Rows[i]["name"]).Append(",");
            string totalDue = string.Format("{0:C}", Convert.ToDecimal(dt.Rows[i]["total_due"] == DBNull.Value ? 0 : dt.Rows[i]["total_due"]));
            exportOutput.Append("\"" + totalDue + " (" + dt.Rows[i]["total_inv_count"] + ")" + "\"").Append(",");
            exportOutput.AppendLine();
        }

        exportOutput.AppendLine();

        exportOutput.Append("").Append(",");
        string totalSumDue = String.Format("{0:C}", dt.Compute("Sum(total_due)", ""));
        if (totalSumDue == "") totalSumDue = System.Globalization.RegionInfo.CurrentRegion.CurrencySymbol + "0.00";
        exportOutput.Append("\"" + totalSumDue + "\"").Append(",");
        exportOutput.AppendLine();

        ExportCSV(Response, exportOutput.ToString(), "Outstanding Fac Invoices.csv");
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

    #region chkUsePaging_CheckedChanged

    protected void chkUsePaging_CheckedChanged(object sender, EventArgs e)
    {
        this.GrdPtInvoicesOutstanding.AllowPaging = chkUsePaging.Checked;
        GrdPtInvoicesOutstanding_FillGrid();

        this.GrdOrgInvoicesOutstanding.AllowPaging = chkUsePaging.Checked;
        GrdOrgInvoicesOutstanding_FillGrid();
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