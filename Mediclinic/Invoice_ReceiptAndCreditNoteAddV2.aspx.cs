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

public partial class Invoice_ReceiptAndCreditNoteAddV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);

            if (!IsPostBack)
            {
                // coming back from the payment gateway, they put 
                //     "?field1=1&field2=2....."
                // so this comes out as
                //      "http:site.com//Page.aspx?invoice_id=12345" + "?field1=1&field2=2....."
                // so we need to change the second "?" to "&"
                int index = Utilities.IndexOfNth(Request.RawUrl, '?', 2);
                if (index != -1)
                {
                    char[] charArray = Request.RawUrl.ToCharArray();
                    charArray[index] = '&';
                    string newURL = new string(charArray);
                    Response.Redirect(newURL);
                }

                if (IsValidFormID())
                {
                    CheckReturnFromOnlinePayment();
                    FillEmptyAddForm();
                    SetupGUI(); // in this form, grid must be created first
                }
                else
                    HideTableAndSetErrorMessage("", "Invalid URL Parameters");
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

    protected void CheckReturnFromOnlinePayment()
    {
        string PT_PaymentReference  = Request.Form["PT_PaymentReference"]  != null ? Request.Form["PT_PaymentReference"]  : Request.QueryString["PT_PaymentReference"];
        string PT_PaymentResult     = Request.Form["PT_PaymentResult"]     != null ? Request.Form["PT_PaymentResult"]     : Request.QueryString["PT_PaymentResult"];
        string PT_PaymentResultCode = Request.Form["PT_PaymentResultCode"] != null ? Request.Form["PT_PaymentResultCode"] : Request.QueryString["PT_PaymentResultCode"];
        string PT_PaymentResultText = Request.Form["PT_PaymentResultText"] != null ? Request.Form["PT_PaymentResultText"] : Request.QueryString["PT_PaymentResultText"];
        string PT_BankReceiptID     = Request.Form["PT_BankReceiptID"]     != null ? Request.Form["PT_BankReceiptID"]     : Request.QueryString["PT_BankReceiptID"];
        string PT_PayTechPaymentID  = Request.Form["PT_PayTechPaymentID"]  != null ? Request.Form["PT_PayTechPaymentID"]  : Request.QueryString["PT_PayTechPaymentID"];


        if (PT_PaymentReference  != null && 
            PT_PaymentResult     != null && 
            PT_PaymentResultCode != null && 
            PT_PaymentResultText != null && 
            PT_BankReceiptID     != null && 
            PT_PayTechPaymentID  != null)
        {

            PaymentPending paymentPending = PaymentPendingDB.GetByID(null, Convert.ToInt32(Request.QueryString["PT_PaymentReference"]));
            Invoice        invoice        = InvoiceDB.GetByID(paymentPending.InvoiceID);

            PaymentPendingDB.Update(
                null,
                DateTime.Now,
                Convert.ToInt32(Request.QueryString["PT_PaymentReference"]),
                PT_PaymentResult,
                PT_PaymentResultCode,
                PT_PaymentResultText,
                PT_BankReceiptID,
                PT_PayTechPaymentID
                );

            if (PT_PaymentResult == "A" && !Convert.ToBoolean(ConfigurationManager.AppSettings["EziDebit_Debugging"]))
            {
                decimal totalOwed  = invoice.TotalDue - paymentPending.PaymentAmount;
                bool    isOverPaid = totalOwed <  0;
                bool    isPaid     = totalOwed <= 0;

                int receiptID = ReceiptDB.Insert(null, 363, paymentPending.InvoiceID, paymentPending.PaymentAmount, 0, false, isOverPaid, DateTime.MinValue, Convert.ToInt32(Session["StaffID"]));

                if (isPaid)
                    InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);

                if (isPaid)
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "close_form", @"<script language=javascript>self.close();</script>");
            }


            string url = Request.RawUrl;
            //url = UrlParamModifier.Remove(url, "PT_PaymentReference");
            url = UrlParamModifier.Remove(url, "PT_PaymentResult");
            url = UrlParamModifier.Remove(url, "PT_PaymentResultCode");
            url = UrlParamModifier.Remove(url, "PT_PaymentResultText");
            url = UrlParamModifier.Remove(url, "PT_BankReceiptID");
            url = UrlParamModifier.Remove(url, "PT_PayTechPaymentID");
            Response.Redirect(url);

        }
        else if (PT_PaymentReference != null)
        {
            PaymentPending paymentPending = PaymentPendingDB.GetByID(null, Convert.ToInt32(Request.QueryString["PT_PaymentReference"]));
            if (paymentPending.OutPaymentResult == "A")
                SetErrorMessage("Online Payment Approved: $" + paymentPending.PaymentAmount + ".<br />Adjusted Amount Owed Remaining Showing Above.");
            if (paymentPending.OutPaymentResult == "U")
                SetErrorMessage("Online Payment was not able to be processed at this time");
            if (paymentPending.OutPaymentResult == "F")
                SetErrorMessage("<div style=\"height:15px;\"></div><span style=\"font-size: 150%;\">Online Payment Failed: <b>" + paymentPending.OutPaymentResultText + "</b></span><div style=\"height:2px;\"></div>");
        }

    }



    #region IsValidFormID(), GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new Exception("Invalid url id");

        string id = Request.QueryString["id"];
        return Convert.ToInt32(id);
    }

    #endregion

    #region SetupGUI()

    public void SetupGUI()
    {
        bool refresh_on_close = Request.QueryString["refresh_on_close"] != null && Request.QueryString["refresh_on_close"] == "1";

        if (Request.QueryString["window_size"] != null && Regex.IsMatch(Request.QueryString["window_size"], @"\d+_\d+"))
        {
            string[] size = Request.QueryString["window_size"].Split('_');
            Page.ClientScript.RegisterStartupScript(this.GetType(), "resize_window", "<script language=javascript>window.resizeTo(" + size[0] + ", " + size[1] + ");</script>");
        }

        // if this popup is called from anotehr page that needs to send back a return value to the opening page, then set the value
        if (Request.QueryString["returnValue"] != null)
        {
            string returnValue = Request.QueryString["returnValue"] != null ? Request.QueryString["returnValue"] : "false";

            if (refresh_on_close)
                btnCancel.OnClientClick = "window.returnValue=" + returnValue + ";window.opener.location.href = window.opener.location.href;self.close();";
            else
                btnCancel.OnClientClick = "window.returnValue=" + returnValue + ";self.close();";

            // make sure if user clicks "x" to close the window, this value is passed on so the other page gets this value passed on too
            if (refresh_on_close) // refresh parent when parent opened this as tab
                Page.ClientScript.RegisterStartupScript(this.GetType(), "on_close_window", "<script language=javascript>window.onbeforeunload = function(){ window.opener.location.href = window.opener.location.href; }</script>");
            else                  // return value as this is a popup and parent can get this value and know what to do
                Page.ClientScript.RegisterStartupScript(this.GetType(), "on_close_window", "<script language=javascript>window.onbeforeunload = function(){ " + "window.returnValue=" + returnValue + ";" + " }</script>");
        }
    }

    #endregion


    private void FillEmptyAddForm()
    {
        Invoice invoice = InvoiceDB.GetByID(GetFormID());
        if (invoice == null)
        {
            HideTableAndSetErrorMessage("Invalid invoice ID");
            return;
        }

        lblInvoiceNbr.Text  = invoice.InvoiceID.ToString();
        lblAmountOwing.Text = "$" + invoice.TotalDue.ToString();


        DataTable dt = DBBase.GetGenericDataTable(null, "ReceiptPaymentType", "receipt_payment_type_id", "descr");

        // add column for displaying data in first few rows with invoice id and invoice amount owing
        dt.Columns.Add("text");
        dt.Columns.Add("tab_index");
        for (int i = dt.Rows.Count - 1; i >= 0; i--)
        {
            dt.Rows[i]["text"] = "";
            if (Convert.ToInt32(dt.Rows[i]["receipt_payment_type_id"]) == 363)
                dt.Rows.RemoveAt(i);
        }

        lstPayments.DataSource = dt;
        lstPayments.DataBind();

        for (int i = lstPayments.Items.Count-1; i >= 0; i--)
        {
            Label   lblReceiptPaymentTypeID     = (Label)lstPayments.Items[i].FindControl("lblTypeID");
            TextBox txtReceiptPaymentTypeAmount = (TextBox)lstPayments.Items[i].FindControl("txtAmount");
            Button  btnWebPay                   = (Button)lstPayments.Items[i].FindControl("btnWebPay");

            if (lblReceiptPaymentTypeID.Text != "133" && lblReceiptPaymentTypeID.Text != "362")
                btnWebPay.Visible = false;

            if (((SystemVariables)Session["SystemVariables"])["EziDebit_Enabled"].Value != "1")
                btnWebPay.Visible = false;

            Utilities.SetEditControlBackColour(txtReceiptPaymentTypeAmount,  true, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        }

        if (lstPayments.Items.Count > 0)
        {
            TextBox txtReceiptPaymentTypeAmount = (TextBox)lstPayments.Items[0].FindControl("txtAmount");
            SetFocus(txtReceiptPaymentTypeAmount);
        }
        


        int entityID = -1;
        if (invoice.PayerOrganisation != null)
            entityID = invoice.PayerOrganisation.EntityID;
        else if (invoice.PayerPatient != null) 
            entityID = invoice.PayerPatient.Person.EntityID;
        else if (invoice.Booking != null && invoice.Booking.Patient != null)
            entityID = BookingDB.GetByID(invoice.Booking.BookingID).Patient.Person.EntityID;

        DataTable dt_vouchers = CreditDB.GetUnusedVouchers(entityID);
        lstVouchers.DataSource = dt_vouchers;
        lstVouchers.DataBind();

        for (int i = lstVouchers.Items.Count - 1; i >= 0; i--)
        {
            TextBox txtAmount = (TextBox)lstVouchers.Items[i].FindControl("txtAmount");
            Utilities.SetEditControlBackColour(txtAmount, true, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        }

        if (lstVouchers.Items.Count == 0)
            divVouchers.Visible = false;




        Utilities.SetEditControlBackColour(txtCreditNoteTotal,  true, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtCreditCardReason, true, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        btnSubmit.Text = "Add Payment(s)";
        btnCancel.Visible = true;
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // close this window
        Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (!IsValidFormID())
        {
            HideTableAndSetErrorMessage();
            return;
        }
        Invoice invoice = InvoiceDB.GetByID(GetFormID());
        if (invoice == null)
        {
            HideTableAndSetErrorMessage("Invalid invoice ID");
            return;
        }


        decimal total = 0;

        
        ArrayList receipts = new ArrayList();
        for (int i = 0; i < lstPayments.Items.Count; i++)
        {
            Label        lblTypeID     = (Label)       lstPayments.Items[i].FindControl("lblTypeID");
            TextBox      txtAmount     = (TextBox)     lstPayments.Items[i].FindControl("txtAmount");

            txtAmount.Text = txtAmount.Text.Trim();
            if (txtAmount.Text.Length > 0 && lblTypeID != null)
            {
                receipts.Add(new Tuple<int, decimal>(Convert.ToInt32(lblTypeID.Text), Convert.ToDecimal(txtAmount.Text)));
                total += Convert.ToDecimal(txtAmount.Text);
            }
        }


        ArrayList vouchers = new ArrayList();
        for (int i = 0; i < lstVouchers.Items.Count; i++)
        {
            HiddenField  hiddenCreditID = (HiddenField)lstVouchers.Items[i].FindControl("hiddenCreditID");
            HiddenField  hiddenEntityID = (HiddenField)lstVouchers.Items[i].FindControl("hiddenEntityID");
            TextBox      txtAmount      = (TextBox)    lstVouchers.Items[i].FindControl("txtAmount");

            txtAmount.Text = txtAmount.Text.Trim();
            if (txtAmount.Text.Length > 0)
            {
                vouchers.Add(new Tuple<int, int, decimal>(Convert.ToInt32(hiddenCreditID.Value), Convert.ToInt32(hiddenEntityID.Value), Convert.ToDecimal(txtAmount.Text)));
                total += Convert.ToDecimal(txtAmount.Text);
            }
        }


        if (txtCreditNoteTotal.Text == string.Empty)
            txtCreditNoteTotal.Text = "0";
        total += Convert.ToDecimal(txtCreditNoteTotal.Text);

        decimal totalOwed  = invoice.TotalDue - total;
        bool    isOverPaid = totalOwed <  0;
        bool    isPaid     = totalOwed <= 0;

        if (isOverPaid)
        {
            SetErrorMessage("Total can not be more than the amount owing.");
            return;
        }


        // put in try/catch block in case someone just used the vouchers and there is more being used than is remaining in the voucher
        ArrayList creditIDsAdded = new ArrayList();
        try
        {
            foreach (Tuple<int, int, decimal> item in vouchers)
            {
                int creditID = CreditDB.Insert_UseVoucher(item.Item2, item.Item3, item.Item1, invoice.InvoiceID, Convert.ToInt32(Session["StaffID"]));
                creditIDsAdded.Add(creditID);
            }
        }
        catch(Exception ex)
        {
            // roll back
            foreach(int creditID in creditIDsAdded)
                CreditDB.Delete(creditID);

            SetErrorMessage(ex.Message);
            return;
        }

        foreach (Tuple<int, decimal> item in receipts)
            ReceiptDB.Insert(null, item.Item1, invoice.InvoiceID, item.Item2, Convert.ToDecimal(0.00), false, isOverPaid, DateTime.MinValue, Convert.ToInt32(Session["StaffID"]));

        if (Convert.ToDecimal(txtCreditNoteTotal.Text) > 0)
            CreditNoteDB.Insert(invoice.InvoiceID, Convert.ToDecimal(txtCreditNoteTotal.Text), txtCreditCardReason.Text, Convert.ToInt32(Session["StaffID"]));

        if (isPaid)
            InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);

        FillEmptyAddForm();

        // close this window
        string returnValue = Request.QueryString["returnValue"] != null ? Request.QueryString["returnValue"] : "false";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=" + returnValue + ";window.opener.location.href = window.opener.location.href;self.close();</script>");
    }


    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
        header_table.Visible = false;
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


    protected void btnWebPay_Command(object sender, CommandEventArgs e)
    {
        for (int i = 0; i < lstPayments.Items.Count; i++)
        {
            Label   lblTypeID = (Label)lstPayments.Items[i].FindControl("lblTypeID");
            TextBox txtAmount = (TextBox)lstPayments.Items[i].FindControl("txtAmount");
            Button  btnWebPay = (Button)lstPayments.Items[i].FindControl("btnWebPay");

            if (lblTypeID.Text == e.CommandArgument.ToString())
            {
                txtAmount.Text = txtAmount.Text.Trim();

                decimal amountEntered;
                bool isNumeric = decimal.TryParse(txtAmount.Text, out amountEntered);

                if (!isNumeric)
                {
                    SetErrorMessage("Please enter a valid amount in the textbox next to the Pay Now button.");
                    return;
                }


                Invoice invoice = InvoiceDB.GetByID(GetFormID());

                if (amountEntered > invoice.TotalDue)
                {
                    SetErrorMessage("Please make sure the amount is not more than the total due.");
                    return;
                }

                string PT_CustomerName = string.Empty;
                if (invoice.PayerOrganisation != null)
                    PT_CustomerName = invoice.PayerOrganisation.Name;
                else if (invoice.PayerPatient != null)
                    PT_CustomerName = invoice.PayerPatient.Person.FullnameWithoutMiddlename;
                else if (invoice.Booking != null && invoice.Booking.Patient != null)
                    PT_CustomerName = invoice.Booking.Patient.Person.FullnameWithoutMiddlename;



                string url = Request.Url.ToString();
                url = UrlParamModifier.Remove(url, "PT_PaymentReference");
                url = UrlParamModifier.Remove(url, "PT_PaymentResult");
                url = UrlParamModifier.Remove(url, "PT_PaymentResultCode");
                url = UrlParamModifier.Remove(url, "PT_PaymentResultText");
                url = UrlParamModifier.Remove(url, "PT_BankReceiptID");
                url = UrlParamModifier.Remove(url, "PT_PayTechPaymentID");
                System.Drawing.Size size = Receipt.GetPopupWindowAddSize();
                url = UrlParamModifier.AddEdit(url, "window_size", (size.Width + 15) + "_" + (size.Height + 75));
                string PT_ReturnUrl = System.Web.HttpUtility.UrlEncode(url);

                Session["UpdateFromWebPay"] = 1;

                Response.Redirect("~/Invoice_WebPayV2.aspx?InvoiceID=" + invoice.InvoiceID + "&PT_CustomerName="+PT_CustomerName+"&PT_PaymentAmount="+txtAmount.Text+"&PT_ReturnUrl=" + PT_ReturnUrl);
            }

        }

    }
}