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
using System.Text;

public partial class TyroPaymentV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string invoiceID = Request.QueryString["invoice"];
                if (invoiceID == null || !Regex.IsMatch(invoiceID, @"^\d+$"))
                    throw new CustomMessageException("No valid invoice in URL");

                Invoice invoice = InvoiceDB.GetByID(Convert.ToInt32(invoiceID));
                if (invoice == null)
                    throw new CustomMessageException("Invalid invoice in URL");
                if (invoice.IsPaID || invoice.TotalDue == 0)
                    throw new CustomMessageException("Invoice already paid");

                SetInvoiceInfo(invoice, true);
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

    #endregion

    #region SetInvoiceInfo, btnUpdateInvoiceInfo_Click

    protected void btnUpdateInvoiceInfo_Click(object sender, EventArgs e)
    {
        SetInvoiceInfo(InvoiceDB.GetByID(Convert.ToInt32(Request.QueryString["invoice"])), false);
    }

    protected void SetInvoiceInfo(Invoice invoice, bool initialSetting)
    {
        string invoiceViewURL = "/Invoice_ViewV2.aspx?invoice_id=" + invoice.InvoiceID;
        lblInvoiceID.Text = "<a href=\"" + invoiceViewURL + "\" onclick=\"open_new_tab('" + invoiceViewURL + "');return false;\">" + invoice.InvoiceID + "</a>";
        lblInvoiceTotal.Text = "$" + invoice.Total.ToString();
        lblInvoiceOwing.Text = "$" + invoice.TotalDue.ToString();
        lblReceiptedTotal.Text = "$" + invoice.ReceiptsTotal.ToString() + (invoice.CreditNotesTotal == 0 ? "" : " &nbsp;&nbsp;($" + invoice.CreditNotesTotal.ToString() + " Credit Noted)") + (invoice.RefundsTotal == 0 ? "" : " &nbsp;&nbsp;($" + invoice.RefundsTotal.ToString() + " Refunds)");
        hiddenInvoiceOwingTotalCents.Value = ((int)(invoice.TotalDue * 100)).ToString();
        hiddenReceiptedAmountTotalCents.Value = ((int)(invoice.ReceiptsTotal * 100)).ToString();
        Page.ClientScript.RegisterStartupScript(this.GetType(), "setamount", "<script language=javascript>if (!is_debugging) document.getElementById('amount').value = '" + invoice.TotalDue.ToString() + "';</script>");
        lblDebtor.Text = invoice.GetDebtor(true);

        if (invoice.Booking != null)
        {
            lblBkDate.Text = invoice.Booking.DateStart.ToString("d MMM, yyyy");
            lblBkOrgText.Text = invoice.Booking.Organisation.IsAgedCare ? "Facility" : "Clinic";
            lblBkOrg.Text = invoice.Booking.Organisation.Name;
        }
        else
        {
            td_bk_date.Visible = false;
            td_bk_org.Visible = false;
        }

        result.InnerHtml          = hiddenResponse.Value;
        merchantReceipt.InnerHtml = (hiddenMerchangeReceipt.Value.Length > 0) ? "<pre>" + hiddenMerchangeReceipt.Value + "</pre>" : string.Empty;
        customerReceipt.InnerHtml = (hiddenCustomerReceipt.Value.Length  > 0) ? "<pre>" + hiddenCustomerReceipt.Value  + "</pre>" : string.Empty;

        btnPrintMerchantReceipt.Style["display"] = hiddenMerchangeReceipt.Value.Length > 0 ? "" : "none";
        btnPrintCustomerReceipt.Style["display"] = hiddenCustomerReceipt.Value.Length  > 0 ? "" : "none";
    }

    #endregion

    #region Print Receipts

    protected void btnPrintMerchantReceipt_Click(object sender, EventArgs e)
    {
        Print(hiddenMerchangeReceipt.Value, "Merchant Receipt.doc");
    }
    protected void btnPrintCustomerReceipt_Click(object sender, EventArgs e)
    {
        Print(hiddenCustomerReceipt.Value, "Customer Receipt.doc");
    }
    protected void Print(string contents, string fileName)
    {
        contents = contents + Environment.NewLine;

        Response.Clear();
        Response.ClearHeaders();
        Response.AddHeader("Content-Length", contents.Length.ToString());
        Response.ContentType = "text/plain";
        Response.AppendHeader("content-disposition", "attachment;filename=\"" + fileName + "\"");
        Response.Write(contents);
        Response.End();
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        header_info_table.Visible = false;
        pairing_link.Visible = false;
        main_table.Visible = false;
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