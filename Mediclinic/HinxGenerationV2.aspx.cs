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


/*

Generating a word doc from html:
From : http://aspalliance.com/794_Dynamic_Generation_of_Word_Document_Report_in_ASPNET_with_HTML

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    protected void btnGenerateDocument_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.Charset = "";
        
        HttpContext.Current.Response.ContentType = "application/msword";
        
        string strFileName = "GenerateDocument"  + ".doc";
        HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=\"" + strFileName + "\"");

        StringBuilder strHTMLContent = new StringBuilder();
        
        strHTMLContent.Append(" <h1 title='Heading' align='Center' style='font-family:verdana;font-size:80%;color:black'><u>Document Heading</u> </h1>".ToString());
        strHTMLContent.Append("<br>".ToString());
        strHTMLContent.Append("<table align='Center'>".ToString());
        strHTMLContent.Append("<tr>".ToString());
        strHTMLContent.Append("<td style='width: 100px;background:#99CC00'><b>Column 1</b></td>".ToString());
        strHTMLContent.Append("<td style='width: 100px;background:#99CC00'><b>Column 2</b></td>".ToString());
        strHTMLContent.Append("<td style='width: 100px;background:#99CC00'><b>Column 3</b></td>".ToString());
        strHTMLContent.Append("</tr>".ToString());
        strHTMLContent.Append("<tr>".ToString());
        strHTMLContent.Append("<td style='width: 100px'>a</td>".ToString());
        strHTMLContent.Append("<td style='width: 100px'>b</td>".ToString());
        strHTMLContent.Append("<td style='width: 100px'>c</td>".ToString());
        strHTMLContent.Append("</tr>".ToString());
        strHTMLContent.Append("<tr>".ToString());
        strHTMLContent.Append("<td style='width: 100px'>d</td>".ToString());
        strHTMLContent.Append("<td style='width: 100px'>e</td>".ToString());
        strHTMLContent.Append("<td style='width: 100px'>f</td>".ToString());
        strHTMLContent.Append("</tr>".ToString());
        strHTMLContent.Append("</table>".ToString());
        strHTMLContent.Append("<br><br>".ToString());
        strHTMLContent.Append("<p align='Center'> Note : This is dynamically generated word document </p>".ToString());
            
       
        HttpContext.Current.Response.Write(strHTMLContent);
        HttpContext.Current.Response.End();
        HttpContext.Current.Response.Flush();
    }
}

*/

public partial class HinxGenerationV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            HideErrorMessage();
            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, false, false, false, false, false);
                SetupGUI();
                span_dev_stuff.Visible = Utilities.IsDev();
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
        SetEclaimTextBox(false);
        eClaimMinorID_row.Visible = Convert.ToBoolean(Session["IsStakeholder"]);

        txtStartDate.Text = "01-07-" + DateTime.Now.AddYears(DateTime.Now.Month < 7 ? -1 : 0).Year;

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";
        
        div_full_report.Style["display"] = "none";
    }

    #endregion


    #region GrdInvoice_ToBeClaimed

    protected void FillGrid_ToBeClaimed()
    {
        DataTable dt = (DataTable)Session["data_to_be_claimed"];
        lblToBeClaimedCount.Text = dt.Rows.Count.ToString();

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sort_expression_to_be_claimed"] != null && Session["sort_expression_to_be_claimed"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sort_expression_to_be_claimed"].ToString();
                GrdInvoice_ToBeClaimed.DataSource = dataView;
            }
            else
            {
                GrdInvoice_ToBeClaimed.DataSource = dt;
            }


            try
            {
                GrdInvoice_ToBeClaimed.DataBind();
                GrdInvoice_ToBeClaimed.PagerSettings.FirstPageText = "1";
                GrdInvoice_ToBeClaimed.PagerSettings.LastPageText = GrdInvoice_ToBeClaimed.PageCount.ToString();
                GrdInvoice_ToBeClaimed.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdInvoice_ToBeClaimed.DataSource = dt;
            GrdInvoice_ToBeClaimed.DataBind();

            int TotalColumns = GrdInvoice_ToBeClaimed.Rows[0].Cells.Count;
            GrdInvoice_ToBeClaimed.Rows[0].Cells.Clear();
            GrdInvoice_ToBeClaimed.Rows[0].Cells.Add(new TableCell());
            GrdInvoice_ToBeClaimed.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdInvoice_ToBeClaimed.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdInvoice_ToBeClaimed_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Pager)
            return;
    }
    protected void GrdInvoice_ToBeClaimed_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable docTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "InvoiceType", "", "descr", "invoice_type_id", "descr");
        DataTable sites = SiteDB.GetDataTable();
        DataTable orgs = OrganisationDB.GetDataTable_GroupOrganisations();

        DataTable dt = Session["data_to_be_claimed"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lnkId = (LinkButton)e.Row.FindControl("lnkId");
            DataRow[] foundRows = dt.Select("inv_invoice_id=" + lnkId.Text);
            DataRow thisRow = foundRows[0];


            if (lnkId != null)
            {
                /*
                if (thisRow["booking_booking_id"] != DBNull.Value)
                {
                    lnkId.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?booking_id={0}', '', 'dialogWidth:775px;dialogHeight:900px;center:yes;resizable:no; scroll:no');return false;", thisRow["booking_booking_id"]);
                }
                else
                {
                    lnkId.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?invoice_id={0}', '', 'dialogWidth:775px;dialogHeight:725px;center:yes;resizable:no; scroll:no');return false;", thisRow["inv_invoice_id"]);
                }
                */
                lnkId.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?invoice_id={0}', '', 'dialogWidth:775px;dialogHeight:725px;center:yes;resizable:no; scroll:no');return false;", thisRow["inv_invoice_id"]);
            }


            Label lblPayer = (Label)e.Row.FindControl("lblPayer");
            if (lblPayer != null)
            {
                if (thisRow["inv_payer_organisation_id"] != DBNull.Value)
                    lblPayer.Text = thisRow["payer_organisation_name"].ToString();
                else if (thisRow["inv_payer_patient_id"] != DBNull.Value)
                    lblPayer.Text = thisRow["payer_patient_person_firstname"].ToString() + " " + thisRow["payer_patient_person_surname"].ToString();
                else
                    lblPayer.Text = thisRow["booking_patient_person_firstname"].ToString() + " " + thisRow["booking_patient_person_surname"].ToString();
            }


            Label lblPayerPatient = (Label)e.Row.FindControl("lblPayerPatient");
            if (lblPayerPatient != null)
            {
                lblPayerPatient.Text = (thisRow["inv_payer_patient_id"] != DBNull.Value) ?
                    thisRow["payer_patient_person_firstname"].ToString() + " " + thisRow["payer_patient_person_surname"].ToString() :
                    thisRow["booking_patient_person_firstname"].ToString() + " " + thisRow["booking_patient_person_surname"].ToString();
            }

            DropDownList ddlInvoiceType = (DropDownList)e.Row.FindControl("ddlInvoiceType");
            if (ddlInvoiceType != null)
            {
                ddlInvoiceType.DataSource = docTypes;
                ddlInvoiceType.DataTextField = "descr";
                ddlInvoiceType.DataValueField = "invoice_type_id";
                ddlInvoiceType.DataBind();
                ddlInvoiceType.SelectedValue = thisRow["inv_invoice_type_id"].ToString();
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSum_Total = (Label)e.Row.FindControl("lblSum_Total");
            lblSum_Total.Text = String.Format("{0:C}", dt.Compute("Sum(inv_total)", ""));   // dt.Compute("Sum(inv_total)", "").ToString();

            Label lblSum_GST = (Label)e.Row.FindControl("lblSum_GST");
            lblSum_GST.Text = String.Format("{0:C}", dt.Compute("Sum(inv_gst)", ""));  // dt.Compute("Sum(inv_gst)", "").ToString();

            Label lblSum_Receipts = (Label)e.Row.FindControl("lblSum_Receipts");
            lblSum_Receipts.Text = String.Format("{0:C}", dt.Compute("Sum(inv_receipts_total)", ""));  // dt.Compute("Sum(inv_receipts_total)", "").ToString();

            Label lblSum_Vouchers = (Label)e.Row.FindControl("lblSum_Vouchers");
            lblSum_Vouchers.Text = String.Format("{0:C}", dt.Compute("Sum(inv_vouchers_total)", ""));  // dt.Compute("Sum(inv_vouchers_total)", "").ToString();

            Label lblSum_CreditNotes = (Label)e.Row.FindControl("lblSum_CreditNotes");
            lblSum_CreditNotes.Text = String.Format("{0:C}", dt.Compute("Sum(inv_credit_notes_total)", ""));  // dt.Compute("Sum(inv_credit_notes_total)", "").ToString();

            Label lblSum_Due = (Label)e.Row.FindControl("lblSum_Due");
            lblSum_Due.Text = String.Format("{0:C}", dt.Compute("Sum(total_due)", ""));  // dt.Compute("Sum(total_due)", "").ToString();
        }
    }
    protected void GrdInvoice_ToBeClaimed_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdInvoice_ToBeClaimed.EditIndex = -1;
        FillGrid_ToBeClaimed();
    }
    protected void GrdInvoice_ToBeClaimed_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        LinkButton lnkId = (LinkButton)GrdInvoice_ToBeClaimed.Rows[e.RowIndex].FindControl("lnkId");

        TextBox txtOrganisationClaimNumber = (TextBox)GrdInvoice_ToBeClaimed.Rows[e.RowIndex].FindControl("txtOrganisationClaimNumber");
        TextBox txtTotal = (TextBox)GrdInvoice_ToBeClaimed.Rows[e.RowIndex].FindControl("txtTotal");
        TextBox txtGST   = (TextBox)GrdInvoice_ToBeClaimed.Rows[e.RowIndex].FindControl("txtGST");



        int invoice_id = Convert.ToInt32(lnkId.Text);
        DataTable dt = Session["data_to_be_claimed"] as DataTable;
        DataRow[] foundRows = dt.Select("inv_invoice_id=" + invoice_id.ToString());
        Invoice inv = InvoiceDB.LoadAll(foundRows[0]);

        try
        {
            InvoiceDB.Update(inv.InvoiceID,
                inv.InvoiceType.ID,
                inv.Booking == null ? -1 : inv.Booking.BookingID,
                inv.PayerOrganisation == null ? 0 : inv.PayerOrganisation.OrganisationID,
                inv.PayerPatient == null ? -1 : inv.PayerPatient.PatientID,
                inv.NonBookinginvoiceOrganisation == null ? 0 : inv.NonBookinginvoiceOrganisation.OrganisationID,
                txtOrganisationClaimNumber.Text,
                inv.RejectLetter == null ? -1 : inv.RejectLetter.LetterID,
                inv.Message,
                Convert.ToDecimal(txtTotal.Text),
                Convert.ToDecimal(txtGST.Text),
                inv.IsPaID,
                inv.IsRefund,
                inv.IsBatched,
                inv.ReversedBy == null ? -1 : inv.ReversedBy.StaffID,
                inv.ReversedDate,
                inv.LastDateEmailed);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToInt32(dt.Rows[i]["inv_invoice_id"]) == invoice_id)
                {
                    dt.Rows[i]["inv_healthcare_claim_number"] = txtOrganisationClaimNumber.Text;
                    dt.Rows[i]["inv_total"] = Convert.ToDecimal(txtTotal.Text);
                    dt.Rows[i]["inv_gst"] = Convert.ToDecimal(txtGST.Text);
                }
            }
            Session["data_to_be_claimed"] = dt;
        }
        catch (UniqueConstraintException ucEx)
        {
            SetErrorMessage(ucEx.Message);
            return;
        }


        GrdInvoice_ToBeClaimed.EditIndex = -1;
        FillGrid_ToBeClaimed();

    }
    protected void GrdInvoice_ToBeClaimed_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        throw new CustomMessageException("Deleting of financial records is not permitted.");
    }
    protected void GrdInvoice_ToBeClaimed_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
        }
    }
    protected void GrdInvoice_ToBeClaimed_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdInvoice_ToBeClaimed.EditIndex = e.NewEditIndex;
        FillGrid_ToBeClaimed();
    }
    protected void GrdInvoice_ToBeClaimed_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdInvoice_ToBeClaimed.EditIndex >= 0)
            return;

        DataTable dataTable = Session["data_to_be_claimed"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sort_expression_to_be_claimed"] == null)
                Session["sort_expression_to_be_claimed"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sort_expression_to_be_claimed"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["sort_expression_to_be_claimed"] = e.SortExpression + " " + newSortExpr;

            GrdInvoice_ToBeClaimed.DataSource = dataView;
            GrdInvoice_ToBeClaimed.DataBind();
        }
    }
    protected void GrdInvoice_ToBeClaimed_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdInvoice_ToBeClaimed.PageIndex = e.NewPageIndex;
        FillGrid_ToBeClaimed();
    }

    #endregion

    #region GrdInvoice_PartiallyPaidClaims

    protected void FillGrid_PartiallyPaidClaims()
    {
        DataTable dt = (DataTable)Session["data_partially_paid_claims"];
        lblPartiallyPaidClaimsCount.Text = dt.Rows.Count.ToString();

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sort_expression_partially_paid_claims"] != null && Session["sort_expression_partially_paid_claims"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sort_expression_partially_paid_claims"].ToString();
                GrdInvoice_PartiallyPaidClaims.DataSource = dataView;
            }
            else
            {
                GrdInvoice_PartiallyPaidClaims.DataSource = dt;
            }


            try
            {
                GrdInvoice_PartiallyPaidClaims.DataBind();
                GrdInvoice_PartiallyPaidClaims.PagerSettings.FirstPageText = "1";
                GrdInvoice_PartiallyPaidClaims.PagerSettings.LastPageText = GrdInvoice_PartiallyPaidClaims.PageCount.ToString();
                GrdInvoice_PartiallyPaidClaims.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdInvoice_PartiallyPaidClaims.DataSource = dt;
            GrdInvoice_PartiallyPaidClaims.DataBind();

            int TotalColumns = GrdInvoice_PartiallyPaidClaims.Rows[0].Cells.Count;
            GrdInvoice_PartiallyPaidClaims.Rows[0].Cells.Clear();
            GrdInvoice_PartiallyPaidClaims.Rows[0].Cells.Add(new TableCell());
            GrdInvoice_PartiallyPaidClaims.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdInvoice_PartiallyPaidClaims.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdInvoice_PartiallyPaidClaims_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Pager)
            return;
    }
    protected void GrdInvoice_PartiallyPaidClaims_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable docTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "InvoiceType", "", "descr", "invoice_type_id", "descr");
        DataTable sites = SiteDB.GetDataTable();
        DataTable orgs = OrganisationDB.GetDataTable_GroupOrganisations();

        DataTable dt = Session["data_partially_paid_claims"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lnkId = (LinkButton)e.Row.FindControl("lnkId");
            DataRow[] foundRows = dt.Select("inv_invoice_id=" + lnkId.Text);
            DataRow thisRow = foundRows[0];


            if (lnkId != null)
            {
                /*
                if (thisRow["booking_booking_id"] != DBNull.Value)
                {
                    lnkId.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?booking_id={0}', '', 'dialogWidth:775px;dialogHeight:900px;center:yes;resizable:no; scroll:no');return false;", thisRow["booking_booking_id"]);
                }
                else
                {
                    lnkId.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?invoice_id={0}', '', 'dialogWidth:775px;dialogHeight:650px;center:yes;resizable:no; scroll:no');return false;", thisRow["inv_invoice_id"]);
                }
                */
                lnkId.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?invoice_id={0}', '', 'dialogWidth:775px;dialogHeight:650px;center:yes;resizable:no; scroll:no');return false;", thisRow["inv_invoice_id"]);
            }


            Label lblPayer = (Label)e.Row.FindControl("lblPayer");
            if (lblPayer != null)
            {
                if (thisRow["inv_payer_organisation_id"] != DBNull.Value)
                    lblPayer.Text = thisRow["payer_organisation_name"].ToString();
                else if (thisRow["inv_payer_patient_id"] != DBNull.Value)
                    lblPayer.Text = thisRow["payer_patient_person_firstname"].ToString() + " " + thisRow["payer_patient_person_surname"].ToString();
                else
                    lblPayer.Text = thisRow["booking_patient_person_firstname"].ToString() + " " + thisRow["booking_patient_person_surname"].ToString();
            }


            Label lblPayerPatient = (Label)e.Row.FindControl("lblPayerPatient");
            if (lblPayerPatient != null)
            {
                lblPayerPatient.Text = (thisRow["inv_payer_patient_id"] != DBNull.Value) ?
                    thisRow["payer_patient_person_firstname"].ToString() + " " + thisRow["payer_patient_person_surname"].ToString() :
                    thisRow["booking_patient_person_firstname"].ToString() + " " + thisRow["booking_patient_person_surname"].ToString();
            }

            DropDownList ddlInvoiceType = (DropDownList)e.Row.FindControl("ddlInvoiceType");
            if (ddlInvoiceType != null)
            {
                ddlInvoiceType.DataSource = docTypes;
                ddlInvoiceType.DataTextField = "descr";
                ddlInvoiceType.DataValueField = "invoice_type_id";
                ddlInvoiceType.DataBind();
                ddlInvoiceType.SelectedValue = thisRow["inv_invoice_type_id"].ToString();
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSum_Total = (Label)e.Row.FindControl("lblSum_Total");
            lblSum_Total.Text = String.Format("{0:C}", dt.Compute("Sum(inv_total)", ""));   // dt.Compute("Sum(inv_total)", "").ToString();

            Label lblSum_GST = (Label)e.Row.FindControl("lblSum_GST");
            lblSum_GST.Text = String.Format("{0:C}", dt.Compute("Sum(inv_gst)", ""));  // dt.Compute("Sum(inv_gst)", "").ToString();

            Label lblSum_Receipts = (Label)e.Row.FindControl("lblSum_Receipts");
            lblSum_Receipts.Text = String.Format("{0:C}", dt.Compute("Sum(inv_receipts_total)", ""));  // dt.Compute("Sum(inv_receipts_total)", "").ToString();

            Label lblSum_Vouchers = (Label)e.Row.FindControl("lblSum_Vouchers");
            lblSum_Vouchers.Text = String.Format("{0:C}", dt.Compute("Sum(inv_vouchers_total)", ""));  // dt.Compute("Sum(inv_vouchers_total)", "").ToString();

            Label lblSum_CreditNotes = (Label)e.Row.FindControl("lblSum_CreditNotes");
            lblSum_CreditNotes.Text = String.Format("{0:C}", dt.Compute("Sum(inv_credit_notes_total)", ""));  // dt.Compute("Sum(inv_credit_notes_total)", "").ToString();

            Label lblSum_Due = (Label)e.Row.FindControl("lblSum_Due");
            lblSum_Due.Text = String.Format("{0:C}", dt.Compute("Sum(total_due)", ""));  // dt.Compute("Sum(total_due)", "").ToString();
        }
    }
    protected void GrdInvoice_PartiallyPaidClaims_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdInvoice_PartiallyPaidClaims.EditIndex = -1;
        FillGrid_PartiallyPaidClaims();
    }
    protected void GrdInvoice_PartiallyPaidClaims_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        LinkButton lnkId = (LinkButton)GrdInvoice_PartiallyPaidClaims.Rows[e.RowIndex].FindControl("lnkId");

        TextBox txtOrganisationClaimNumber = (TextBox)GrdInvoice_PartiallyPaidClaims.Rows[e.RowIndex].FindControl("txtOrganisationClaimNumber");
        TextBox txtTotal = (TextBox)GrdInvoice_PartiallyPaidClaims.Rows[e.RowIndex].FindControl("txtTotal");
        TextBox txtGST = (TextBox)GrdInvoice_PartiallyPaidClaims.Rows[e.RowIndex].FindControl("txtGST");



        int invoice_id = Convert.ToInt32(lnkId.Text);
        DataTable dt = Session["data_partially_paid_claims"] as DataTable;
        DataRow[] foundRows = dt.Select("inv_invoice_id=" + invoice_id.ToString());
        Invoice inv = InvoiceDB.LoadAll(foundRows[0]);

        try
        {
            InvoiceDB.Update(inv.InvoiceID,
                inv.InvoiceType.ID,
                inv.Booking == null ? -1 : inv.Booking.BookingID,
                inv.PayerOrganisation == null ? 0 : inv.PayerOrganisation.OrganisationID,
                inv.PayerPatient == null ? -1 : inv.PayerPatient.PatientID,
                inv.NonBookinginvoiceOrganisation == null ? 0 : inv.NonBookinginvoiceOrganisation.OrganisationID,
                txtOrganisationClaimNumber.Text,
                inv.RejectLetter == null ? -1 : inv.RejectLetter.LetterID,
                inv.Message,
                Convert.ToDecimal(txtTotal.Text),
                Convert.ToDecimal(txtGST.Text),
                inv.IsPaID,
                inv.IsRefund,
                inv.IsBatched,
                inv.ReversedBy == null ? -1 : inv.ReversedBy.StaffID,
                inv.ReversedDate,
                inv.LastDateEmailed);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToInt32(dt.Rows[i]["inv_invoice_id"]) == invoice_id)
                {
                    dt.Rows[i]["inv_healthcare_claim_number"] = txtOrganisationClaimNumber.Text;
                    dt.Rows[i]["inv_total"] = Convert.ToDecimal(txtTotal.Text);
                    dt.Rows[i]["inv_gst"] = Convert.ToDecimal(txtGST.Text);
                }
            }
            Session["data_partially_paid_claims"] = dt;
        }
        catch (UniqueConstraintException ucEx)
        {
            SetErrorMessage(ucEx.Message);
            return;
        }


        GrdInvoice_PartiallyPaidClaims.EditIndex = -1;
        FillGrid_PartiallyPaidClaims();

    }
    protected void GrdInvoice_PartiallyPaidClaims_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        throw new CustomMessageException("Deleting of financial records is not permitted.");
    }
    protected void GrdInvoice_PartiallyPaidClaims_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
        }
    }
    protected void GrdInvoice_PartiallyPaidClaims_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdInvoice_PartiallyPaidClaims.EditIndex = e.NewEditIndex;
        FillGrid_PartiallyPaidClaims();
    }
    protected void GrdInvoice_PartiallyPaidClaims_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdInvoice_PartiallyPaidClaims.EditIndex >= 0)
            return;

        DataTable dataTable = Session["data_partially_paid_claims"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sort_expression_partially_paid_claims"] == null)
                Session["sort_expression_partially_paid_claims"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sort_expression_partially_paid_claims"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["sort_expression_partially_paid_claims"] = e.SortExpression + " " + newSortExpr;

            GrdInvoice_PartiallyPaidClaims.DataSource = dataView;
            GrdInvoice_PartiallyPaidClaims.DataBind();
        }
    }
    protected void GrdInvoice_PartiallyPaidClaims_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdInvoice_PartiallyPaidClaims.PageIndex = e.NewPageIndex;
        FillGrid_PartiallyPaidClaims();
    }

    #endregion

    #region GrdInvoice_ClaimManually

    protected void FillGrid_ClaimManually()
    {
        DataTable dt = (DataTable)Session["data_claim_manually"];
        lblClaimManuallyCount.Text = dt.Rows.Count.ToString();

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sort_expression_claim_manually"] != null && Session["sort_expression_claim_manually"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sort_expression_claim_manually"].ToString();
                GrdInvoice_ClaimManually.DataSource = dataView;
            }
            else
            {
                GrdInvoice_ClaimManually.DataSource = dt;
            }


            try
            {
                GrdInvoice_ClaimManually.DataBind();
                GrdInvoice_ClaimManually.PagerSettings.FirstPageText = "1";
                GrdInvoice_ClaimManually.PagerSettings.LastPageText = GrdInvoice_ClaimManually.PageCount.ToString();
                GrdInvoice_ClaimManually.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdInvoice_ClaimManually.DataSource = dt;
            GrdInvoice_ClaimManually.DataBind();

            int TotalColumns = GrdInvoice_ClaimManually.Rows[0].Cells.Count;
            GrdInvoice_ClaimManually.Rows[0].Cells.Clear();
            GrdInvoice_ClaimManually.Rows[0].Cells.Add(new TableCell());
            GrdInvoice_ClaimManually.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdInvoice_ClaimManually.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdInvoice_ClaimManually_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Pager)
            return;
    }
    protected void GrdInvoice_ClaimManually_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable docTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "InvoiceType", "", "descr", "invoice_type_id", "descr");
        DataTable sites = SiteDB.GetDataTable();
        DataTable orgs = OrganisationDB.GetDataTable_GroupOrganisations();

        DataTable dt = Session["data_claim_manually"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lnkId = (LinkButton)e.Row.FindControl("lnkId");
            DataRow[] foundRows = dt.Select("inv_invoice_id=" + lnkId.Text);
            DataRow thisRow = foundRows[0];


            if (lnkId != null)
            {
                /*
                if (thisRow["booking_booking_id"] != DBNull.Value)
                {
                    lnkId.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?booking_id={0}', '', 'dialogWidth:775px;dialogHeight:900px;center:yes;resizable:no; scroll:no');return false;", thisRow["booking_booking_id"]);
                }
                else
                {
                    lnkId.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?invoice_id={0}', '', 'dialogWidth:775px;dialogHeight:650px;center:yes;resizable:no; scroll:no');return false;", thisRow["inv_invoice_id"]);
                }
                */
                lnkId.OnClientClick = String.Format("javascript:window.showModalDialog('Invoice_ViewV2.aspx?invoice_id={0}', '', 'dialogWidth:775px;dialogHeight:650px;center:yes;resizable:no; scroll:no');return false;", thisRow["inv_invoice_id"]);
            }


            Label lblPayer = (Label)e.Row.FindControl("lblPayer");
            if (lblPayer != null)
            {
                if (thisRow["inv_payer_organisation_id"] != DBNull.Value)
                    lblPayer.Text = thisRow["payer_organisation_name"].ToString();
                else if (thisRow["inv_payer_patient_id"] != DBNull.Value)
                    lblPayer.Text = thisRow["payer_patient_person_firstname"].ToString() + " " + thisRow["payer_patient_person_surname"].ToString();
                else
                    lblPayer.Text = thisRow["booking_patient_person_firstname"].ToString() + " " + thisRow["booking_patient_person_surname"].ToString();
            }


            Label lblPayerPatient = (Label)e.Row.FindControl("lblPayerPatient");
            if (lblPayerPatient != null)
            {
                lblPayerPatient.Text = (thisRow["inv_payer_patient_id"] != DBNull.Value) ?
                    thisRow["payer_patient_person_firstname"].ToString() + " " + thisRow["payer_patient_person_surname"].ToString() :
                    thisRow["booking_patient_person_firstname"].ToString() + " " + thisRow["booking_patient_person_surname"].ToString();
            }

            DropDownList ddlInvoiceType = (DropDownList)e.Row.FindControl("ddlInvoiceType");
            if (ddlInvoiceType != null)
            {
                ddlInvoiceType.DataSource = docTypes;
                ddlInvoiceType.DataTextField = "descr";
                ddlInvoiceType.DataValueField = "invoice_type_id";
                ddlInvoiceType.DataBind();
                ddlInvoiceType.SelectedValue = thisRow["inv_invoice_type_id"].ToString();
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblSum_Total = (Label)e.Row.FindControl("lblSum_Total");
            lblSum_Total.Text = String.Format("{0:C}", dt.Compute("Sum(inv_total)", ""));   // dt.Compute("Sum(inv_total)", "").ToString();

            Label lblSum_GST = (Label)e.Row.FindControl("lblSum_GST");
            lblSum_GST.Text = String.Format("{0:C}", dt.Compute("Sum(inv_gst)", ""));  // dt.Compute("Sum(inv_gst)", "").ToString();

            Label lblSum_Receipts = (Label)e.Row.FindControl("lblSum_Receipts");
            lblSum_Receipts.Text = String.Format("{0:C}", dt.Compute("Sum(inv_receipts_total)", ""));  // dt.Compute("Sum(inv_receipts_total)", "").ToString();

            Label lblSum_Vouchers = (Label)e.Row.FindControl("lblSum_Vouchers");
            lblSum_Vouchers.Text = String.Format("{0:C}", dt.Compute("Sum(inv_vouchers_total)", ""));  // dt.Compute("Sum(inv_vouchers_total)", "").ToString();

            Label lblSum_CreditNotes = (Label)e.Row.FindControl("lblSum_CreditNotes");
            lblSum_CreditNotes.Text = String.Format("{0:C}", dt.Compute("Sum(inv_credit_notes_total)", ""));  // dt.Compute("Sum(inv_credit_notes_total)", "").ToString();

            Label lblSum_Due = (Label)e.Row.FindControl("lblSum_Due");
            lblSum_Due.Text = String.Format("{0:C}", dt.Compute("Sum(total_due)", ""));  // dt.Compute("Sum(total_due)", "").ToString();
        }
    }
    protected void GrdInvoice_ClaimManually_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdInvoice_ClaimManually.EditIndex = -1;
        FillGrid_ClaimManually();
    }
    protected void GrdInvoice_ClaimManually_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        LinkButton lnkId = (LinkButton)GrdInvoice_ClaimManually.Rows[e.RowIndex].FindControl("lnkId");

        TextBox txtOrganisationClaimNumber = (TextBox)GrdInvoice_ClaimManually.Rows[e.RowIndex].FindControl("txtOrganisationClaimNumber");
        TextBox txtTotal = (TextBox)GrdInvoice_ClaimManually.Rows[e.RowIndex].FindControl("txtTotal");
        TextBox txtGST = (TextBox)GrdInvoice_ClaimManually.Rows[e.RowIndex].FindControl("txtGST");



        int invoice_id = Convert.ToInt32(lnkId.Text);
        DataTable dt = Session["data_claim_manually"] as DataTable;
        DataRow[] foundRows = dt.Select("inv_invoice_id=" + invoice_id.ToString());
        Invoice inv = InvoiceDB.LoadAll(foundRows[0]);

        try
        {
            InvoiceDB.Update(inv.InvoiceID,
                inv.InvoiceType.ID,
                inv.Booking == null ? -1 : inv.Booking.BookingID,
                inv.PayerOrganisation == null ? 0 : inv.PayerOrganisation.OrganisationID,
                inv.PayerPatient == null ? -1 : inv.PayerPatient.PatientID,
                inv.NonBookinginvoiceOrganisation == null ? 0 : inv.NonBookinginvoiceOrganisation.OrganisationID,
                txtOrganisationClaimNumber.Text,
                inv.RejectLetter == null ? -1 : inv.RejectLetter.LetterID,
                inv.Message,
                Convert.ToDecimal(txtTotal.Text),
                Convert.ToDecimal(txtGST.Text),
                inv.IsPaID,
                inv.IsRefund,
                inv.IsBatched,
                inv.ReversedBy == null ? -1 : inv.ReversedBy.StaffID,
                inv.ReversedDate,
                inv.LastDateEmailed);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToInt32(dt.Rows[i]["inv_invoice_id"]) == invoice_id)
                {
                    dt.Rows[i]["inv_healthcare_claim_number"] = txtOrganisationClaimNumber.Text;
                    dt.Rows[i]["inv_total"] = Convert.ToDecimal(txtTotal.Text);
                    dt.Rows[i]["inv_gst"] = Convert.ToDecimal(txtGST.Text);
                }
            }
            Session["data_claim_manually"] = dt;
        }
        catch (UniqueConstraintException ucEx)
        {
            SetErrorMessage(ucEx.Message);
            return;
        }


        GrdInvoice_ClaimManually.EditIndex = -1;
        FillGrid_ClaimManually();

    }
    protected void GrdInvoice_ClaimManually_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        throw new CustomMessageException("Deleting of financial records is not permitted.");
    }
    protected void GrdInvoice_ClaimManually_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
        }
    }
    protected void GrdInvoice_ClaimManually_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdInvoice_ClaimManually.EditIndex = e.NewEditIndex;
        FillGrid_ClaimManually();
    }
    protected void GrdInvoice_ClaimManually_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdInvoice_ClaimManually.EditIndex >= 0)
            return;

        DataTable dataTable = Session["data_claim_manually"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sort_expression_claim_manually"] == null)
                Session["sort_expression_claim_manually"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sort_expression_claim_manually"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["sort_expression_claim_manually"] = e.SortExpression + " " + newSortExpr;

            GrdInvoice_ClaimManually.DataSource = dataView;
            GrdInvoice_ClaimManually.DataBind();
        }
    }
    protected void GrdInvoice_ClaimManually_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdInvoice_ClaimManually.PageIndex = e.NewPageIndex;
        FillGrid_ClaimManually();
    }

    #endregion


    #region btnGenerate methods

    protected bool GenerateByEclaim(string eclaimNbr)
    {
        Invoice invoice = InvoiceDB.GetByClaimNumber(eclaimNbr);
        if (invoice == null)
            return false;

        if (DateTime.Now.Subtract(invoice.InvoiceDateAdded).TotalDays > 370)
            throw new Exception("Invoice is over 2 years old");

        if (invoice.PayerOrganisation.OrganisationID != -1 && invoice.PayerOrganisation.OrganisationID != -2)
            throw new Exception("Not a medicare or dva invoice");

        HinxFile.ClaimType claimType = invoice.PayerOrganisation.OrganisationID == -1 ? HinxFile.ClaimType.Medicare : HinxFile.ClaimType.DVA;
        HinxFile hinxFile = new HinxFile(claimType, Convert.ToInt32(Session["SiteID"]));
        hinxFile.CreateXML(new Invoice[] { invoice });
        return true;
    }
    protected void btnTestGenerate_Click(object sender, EventArgs e)
    {
        try
        {
            if (!GenerateByEclaim(txtInvID.Text))
                throw new Exception("No invoice with claim number " + txtInvID.Text);

            SetErrorMessage("Successfully generated invoice for claim " + txtInvID.Text);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            SetErrorMessage("Connection to network files currently unavailable.");
        }
        catch (Exception ex)
        {
            SetErrorMessage(ex.ToString());
        }
    }
    protected void btnTestGenerateDir_Click(object sender, EventArgs e)
    {
        try
        {
            string noInv = string.Empty;

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(txtDirToConvert.Text);
            int count = 0;
            int noInvCount = 0;
            foreach (System.IO.FileInfo f in dir.GetFiles())
            {
                if (!GenerateByEclaim(f.Name.Replace(".hinx", "")))
                {
                    noInv += (noInv.Length > 0 ? "," : "") + f.Name.Replace(".hinx", "");
                    noInvCount++;
                }
                else
                    count++;
            }

            SetErrorMessage("Successfully generated "+count+" invoices" + "<br />" + "non-existent = " + noInvCount + "<br />" + noInv);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            SetErrorMessage("Connection to network files currently unavailable.");
        }
        catch (Exception ex)
        {
            SetErrorMessage(ex.ToString());
        }
    }



    protected void btnGenerateMedicareHinxFiles_Click(object sender, EventArgs e)
    {
        try
        {
            if (CheckIsValidStartEndDates())
            {
                GenerateClaimNumbers(-1);
                GenerateHinxFiles(HinxFile.ClaimType.Medicare, GetFromDate(), GetToDate());
            }
        }
        catch (Exception ex)
        {
            if (ex.Message == "No claim numbers left!")
                SetErrorMessage("No claim numbers left!<br />Please contact Mediclinic to have more allocated.");
            else
                throw;
        }
    }
    protected void btnGenerateMedicareReport_Click(object sender, EventArgs e)
    {
        if (CheckIsValidStartEndDates())
            GenerateReport(HinxFile.ClaimType.Medicare, GetFromDate(), GetToDate());
    }
    protected void btnGenerateDVAHinxFiles_Click(object sender, EventArgs e)
    {
        try
        {
            if (CheckIsValidStartEndDates())
            {
                GenerateClaimNumbers(-2);
                GenerateHinxFiles(HinxFile.ClaimType.DVA, GetFromDate(), GetToDate());
            }
        }
        catch (Exception ex)
        {
            if (ex.Message == "No claim numbers left!")
                SetErrorMessage("No claim numbers left!<br />Please contact Mediclinic to have more allocated.");
            else
                throw;
        }
    }
    protected void btnGenerateDVAReport_Click(object sender, EventArgs e)
    {
        if (CheckIsValidStartEndDates())
            GenerateReport(HinxFile.ClaimType.DVA, GetFromDate(), GetToDate());
    }

    protected void GenerateHinxFiles(HinxFile.ClaimType claimType, DateTime fromDate, DateTime toDate)
    {
        try
        {
            if (Convert.ToInt32(SystemVariableDB.GetByDescr("AutoMedicareClaiming").Value) != 1)
            {
                SetErrorMessage("Medicare claiming has not been turned on for this customer. Go to website settings and turn it on before running it.");
                return;
            }

            int startTimeGenerateReport = Environment.TickCount;
            HinxFile hinxFile = new HinxFile(claimType, Convert.ToInt32(Session["SiteID"]));
            HinxFile.ReportItems reportItems = hinxFile.GenerateReportItems(fromDate, toDate, false);
            double executionTimeGenerateReport = (double)(Environment.TickCount - startTimeGenerateReport) / 1000.0;

            // Commented out since they want to allow overwrite of old files rather than error message saying remove them to re-generate
            //
            //string[] existingHinxFiles = hinxFile.GetExistingHinxFiles(reportItems.reportTable);
            //if (existingHinxFiles.Length > 0)
            //{
            //    int itemsPerLine = 8;
            //    string existing = string.Empty;
            //    for (int i = 0; i < existingHinxFiles.Length; i++)
            //        existing += (i != 0 && i % itemsPerLine == 0 ? "<br />" : "") + existingHinxFiles[i] + (i != existingHinxFiles.Length - 1 ? ", " : "");
            //    HideTableAndSetErrorMessage("Can not generate while the following eclaim files already exist : " + "<br />" + string.Join("<br />", existing));
            //    return;
            //}

            int startTimeGenerateFiles = Environment.TickCount;

            string failedItemsMessage = string.Empty;
            try 
            { 
                hinxFile.CreateXML(reportItems.ToBeClaimed); 
            }
            catch (System.ComponentModel.Win32Exception)
            {
                SetErrorMessage("Failed to generate - connection to network files currently unavailable.");
                return;
            }
            catch (HINXUnsuccessfulItemsException ex)
            { 
                failedItemsMessage = ex.Message; 
            }

            double executionTimeGenerateFiles = (double)(Environment.TickCount - startTimeGenerateFiles) / 1000.0;

            SetErrorMessage("Successfully generated for " + reportItems.ToBeClaimed.Length + " invoices in " + executionTimeGenerateFiles + " seconds &nbsp;&nbsp;&nbsp;&nbsp; [generated report in " + executionTimeGenerateReport + " seconds]" + (failedItemsMessage.Length == 0 ? "" : "<br /><br />" + failedItemsMessage));
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
    protected void GenerateReport(HinxFile.ClaimType claimType, DateTime fromDate, DateTime toDate)
    {
        try
        {
            HinxFile hinxFile = new HinxFile(claimType, Convert.ToInt32(Session["SiteID"]));
            HinxFile.ReportItems reportItems = hinxFile.GenerateReportItems(fromDate, toDate, false);
            GenerateReport(reportItems.reportTable);
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
    protected void GenerateReport(DataTable reportTable)
    {
        DataTable dtToBeClaimed = reportTable.Clone(); // copy structure
        DataTable dtPartiallyPaidClaims = reportTable.Clone(); // copy structure
        DataTable dtClaimManually = reportTable.Clone(); // copy structure
        for (int i = 0; i < reportTable.Rows.Count; i++)
        {
            if (Convert.ToInt32(reportTable.Rows[i]["display_group_id"]) == 1)
                dtToBeClaimed.Rows.Add(reportTable.Rows[i].ItemArray);
            else if (Convert.ToInt32(reportTable.Rows[i]["display_group_id"]) == 3)
                dtPartiallyPaidClaims.Rows.Add(reportTable.Rows[i].ItemArray);
            else if (Convert.ToInt32(reportTable.Rows[i]["display_group_id"]) == 4)
                dtClaimManually.Rows.Add(reportTable.Rows[i].ItemArray);
        }
        Session["data_to_be_claimed"] = dtToBeClaimed;
        FillGrid_ToBeClaimed();
        Session["data_partially_paid_claims"] = dtPartiallyPaidClaims;
        FillGrid_PartiallyPaidClaims();
        Session["data_claim_manually"] = dtClaimManually;
        FillGrid_ClaimManually();

        Page.ClientScript.RegisterStartupScript(this.GetType(), "collapse_all", "<script language=javascript>expand_collapse_all(false);</script>");
        div_full_report.Style.Remove("display");  // remove style="display:none;"
    }

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


    #region SetEclaimTextBox(editable), btnEditEclaim

    protected void btnEclaimSetEditMode_Click(object sender, EventArgs e)
    {
        SetEclaimTextBox(true);
    }
    protected void btnEclaimCancelEditMode_Click(object sender, EventArgs e)
    {
        SetEclaimTextBox(false);
    }
    protected void btnEclaimUpdate_Click(object sender, EventArgs e)
    {
        if (!txtValidateEclaimNbrRequired.IsValid || !txtValidateEclaimNbrRegex.IsValid)
            return;

        SystemVariableDB.Update("MedicareEclaimsLicenseNbr", txtEclaimNbr.Text);
        SetEclaimTextBox(false);
    }

    protected void SetEclaimTextBox(bool editable)
    {
        txtEclaimNbr.Text     = SystemVariableDB.GetByDescr("MedicareEclaimsLicenseNbr").Value;
        txtEclaimNbr.ReadOnly = !editable;

        btnEclaimSetEditMode.Visible    = !editable;
        btnEclaimCancelEditMode.Visible =  editable;
        btnEclaimUpdate.Visible         =  editable;
        Utilities.SetEditControlBackColour(txtEclaimNbr, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        if (editable)
        {
            // set cursor at "end" of the text
            string jsSetCursorEnd = @"var b98 = document.getElementById('" + txtEclaimNbr.ID.ToString() + @"'); b98.focus(); var val = b98.value; b98.value = ''; b98.value = val;";
            ClientScript.RegisterStartupScript(Page.ClientScript.GetType(), Page.ClientID, jsSetCursorEnd, true);
        }
    }

    #endregion


    #region GenerateClaimNumbers

    protected void GenerateClaimNumbers(int organisation_id)
    {
        Hashtable sitesHash = SiteDB.GetAllInHashtable();
        Hashtable claimNumberInvoiceGroups = new Hashtable();
        

        DataTable dt = InvoiceDB.GetMedicareInvoicesWithoutClaimNumbers(true, organisation_id, Convert.ToInt32(Session["SiteID"]), GetFromDate(), GetToDate());
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            Invoice invoice = InvoiceDB.LoadAll(dt.Rows[i]);

            int      providerID = invoice.Booking.Provider.StaffID;
            int      orgID      = ((Site)sitesHash[invoice.Site.SiteID]).SiteType.ID == 1 ? invoice.Booking.Organisation.OrganisationID : 0;
            DateTime date       = invoice.Booking.DateStart.Date;

            Hashtable3D.KeyWithDate hashKey = Hashtable3D.KeyWithDate.New(providerID, orgID, date);

            if (claimNumberInvoiceGroups[hashKey] == null)
                claimNumberInvoiceGroups[hashKey] = new ArrayList();

            ((ArrayList)claimNumberInvoiceGroups[hashKey]).Add(invoice);
        }
        foreach (Hashtable3D.KeyWithDate hashKey in claimNumberInvoiceGroups.Keys)
        {
            string claimNumber = string.Empty;

            ArrayList invoiceArrayList = (ArrayList)claimNumberInvoiceGroups[hashKey];

            // convert to list for sorting and sort descending on date added so that 
            // claim number most recently used is the last added invoice
            List<Invoice> invoiceList = invoiceArrayList.Cast<Invoice>().ToList();
            invoiceList.Sort(delegate(Invoice a, Invoice b) 
            {
               return b.InvoiceDateAdded.CompareTo(a.InvoiceDateAdded); 
            });

            for (int i = 0; i < invoiceList.Count; i++)
            {
                if (invoiceList[i].InvoiceID == 337927)
                {
                    ;
                }

                if (claimNumber == string.Empty)
                    claimNumber = MedicareClaimNbrDB.InsertIntoInvoice(((Invoice)invoiceList[i]).InvoiceID, DateTime.Now.Date);
                else
                    InvoiceDB.SetClaimNumber(((Invoice)invoiceList[i]).InvoiceID, claimNumber);
            }
        }


        SetErrorMessage(claimNumberInvoiceGroups.Keys.Count + " claim numbers generated for " + dt.Rows.Count + " invoices.");

    }

    #endregion


    #region HideTableAndSetErrorMessage, SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        div_full_report.Style["display"] = "none";
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