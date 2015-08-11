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

public partial class InvoicePaymentV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            HideErrorMessage();

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

                if (IsValidFormParams())
                {
                    FillEmptyAddForm();
                    CheckReturnFromOnlinePayment();
                    SetupGUI(); // in this form, grid must be created first
                }
                else
                    HideTableAndSetErrorMessage("Invalid Link");
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

    #region CheckReturnFromOnlinePayment

    protected void CheckReturnFromOnlinePayment()
    {
        string PT_PaymentReference  = Request.Form["PT_PaymentReference"]  != null ? Request.Form["PT_PaymentReference"]  : Request.QueryString["PT_PaymentReference"];
        string PT_PaymentResult     = Request.Form["PT_PaymentResult"]     != null ? Request.Form["PT_PaymentResult"]     : Request.QueryString["PT_PaymentResult"];
        string PT_PaymentResultCode = Request.Form["PT_PaymentResultCode"] != null ? Request.Form["PT_PaymentResultCode"] : Request.QueryString["PT_PaymentResultCode"];
        string PT_PaymentResultText = Request.Form["PT_PaymentResultText"] != null ? Request.Form["PT_PaymentResultText"] : Request.QueryString["PT_PaymentResultText"];
        string PT_BankReceiptID     = Request.Form["PT_BankReceiptID"]     != null ? Request.Form["PT_BankReceiptID"]     : Request.QueryString["PT_BankReceiptID"];
        string PT_PayTechPaymentID  = Request.Form["PT_PayTechPaymentID"]  != null ? Request.Form["PT_PayTechPaymentID"]  : Request.QueryString["PT_PayTechPaymentID"];


        string DB = GetFormParams(false).Item1;


        if (PT_PaymentReference  != null && 
            PT_PaymentResult     != null && 
            PT_PaymentResultCode != null && 
            PT_PaymentResultText != null && 
            PT_BankReceiptID     != null && 
            PT_PayTechPaymentID  != null)
        {
            PaymentPending paymentPending = PaymentPendingDB.GetByID(DB, Convert.ToInt32(Request.QueryString["PT_PaymentReference"]));
            Invoice        invoice        = InvoiceDB.GetByID(paymentPending.InvoiceID, DB);

            PaymentPendingDB.Update(
                DB,
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

                int receiptID = ReceiptDB.Insert(DB, 363, paymentPending.InvoiceID, paymentPending.PaymentAmount, 0, false, isOverPaid, DateTime.MinValue, -6);

                if (isPaid)
                    InvoiceDB.UpdateIsPaid(DB, invoice.InvoiceID, true);

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
            PaymentPending paymentPending = PaymentPendingDB.GetByID(DB, Convert.ToInt32(Request.QueryString["PT_PaymentReference"]));
            if (paymentPending.OutPaymentResult == "A")
            {
                Invoice invoice = InvoiceDB.GetByID(paymentPending.InvoiceID, DB);
                if (invoice.IsPaID)
                    SetErrorMessage("Online Payment Approved: $" + paymentPending.PaymentAmount + ".<br />Thank you for your payment.");
                else
                    SetErrorMessage("Online Payment Approved: $" + paymentPending.PaymentAmount + ".<br />Adjusted Amount Owed Remaining Showing Above.");
            }
            if (paymentPending.OutPaymentResult == "U")
                SetErrorMessage("Online Payment was not able to be processed at this time");
            if (paymentPending.OutPaymentResult == "F")
                SetErrorMessage("Online Payment Failed: " + paymentPending.OutPaymentResultText);
        }

    }

    #endregion

    #region IsValidFormID(), GetFormID()

    protected bool IsValidFormParams()
    {
        return Invoice.IsValidInvoiceHash(Request.QueryString["id"]);
    }
    protected Tuple<string, int> GetFormParams(bool checkIsValid = true)
    {
        if (checkIsValid && !Invoice.IsValidInvoiceHash(Request.QueryString["id"]))
            throw new Exception("Invalid Link");

        return Invoice.DecodeInvoiceHash(Request.QueryString["id"]);
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
    }

    #endregion
    
    #region FillEmptyAddForm, btnWebPay_Command

    private void FillEmptyAddForm()
    {
        Tuple<string, int> formParams = GetFormParams(false);

        Invoice invoice = InvoiceDB.GetByID(formParams.Item2, formParams.Item1);
        if (invoice == null)
        {
            HideTableAndSetErrorMessage("Invalid invoice ID");
            return;
        }


        lblInvoiceNbr.Text  = invoice.InvoiceID.ToString();
        lblAmountOwing.Text = "$" + invoice.TotalDue.ToString();


        DataTable dt = DBBase.GetGenericDataTable(formParams.Item1, "ReceiptPaymentType", "receipt_payment_type_id", "descr");
        for (int i = dt.Rows.Count - 1; i >= 0 ; i--)
            if (Convert.ToInt32(dt.Rows[i]["receipt_payment_type_id"]) != 133)
                dt.Rows.RemoveAt(i);

        // add column for displaying data in first few rows with invoice id and invoice amount owing
        dt.Columns.Add("text");
        dt.Columns.Add("tab_index");
        for (int i = 0; i < dt.Rows.Count; i++)
            dt.Rows[i]["text"] = "";

        lstPayments.DataSource = dt;
        lstPayments.DataBind();

        for (int i = lstPayments.Items.Count-1; i >= 0; i--)
        {
            Label   lblReceiptPaymentTypeID     = (Label)lstPayments.Items[i].FindControl("lblTypeID");
            TextBox txtReceiptPaymentTypeAmount = (TextBox)lstPayments.Items[i].FindControl("txtAmount");
            Button  btnWebPay                   = (Button)lstPayments.Items[i].FindControl("btnWebPay");

            if (lblReceiptPaymentTypeID.Text != "133" && lblReceiptPaymentTypeID.Text != "362")
                btnWebPay.Visible = false;

            txtReceiptPaymentTypeAmount.Text = invoice.TotalDue.ToString();

            SystemVariables sysVariables = SystemVariableDB.GetAll(formParams.Item1);
            if (sysVariables["EziDebit_Enabled"].Value != "1")
                btnWebPay.Visible = false;

            Utilities.SetEditControlBackColour(txtReceiptPaymentTypeAmount,  true, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        }

        if (invoice.IsPaID)
        {
            maintable.Visible = false;
            SetErrorMessage("Invoice already paid");
        }
    }

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


                Tuple<string, int> formParams = GetFormParams(false);
                Invoice invoice = InvoiceDB.GetByID(formParams.Item2, formParams.Item1);

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

                /*
                string b = Request.QueryString["id"];
                string a = HttpUtility.HtmlEncode(Request.QueryString["id"].ToString());
                string f = HttpUtility.HtmlDecode(Request.QueryString["id"].ToString());
                string c = HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(Request.QueryString["id"].ToString()));

                string d = HttpUtility.HtmlEncode("~/Invoice_WebPayV2.aspx?id=" + HttpUtility.HtmlDecode(Request.QueryString["id"]) + "&InvoiceID=" + invoice.InvoiceID + "&PT_CustomerName=" + PT_CustomerName + "&PT_PaymentAmount=" + txtAmount.Text + "&PT_ReturnUrl=" + PT_ReturnUrl);

                string h = Uri.EscapeDataString(HttpUtility.HtmlDecode(Request.QueryString["id"].ToString()));
                */


                Response.Redirect("~/Invoice_WebPayV2.aspx?id=" + Uri.EscapeDataString(Uri.UnescapeDataString(Request.QueryString["id"])) + "&InvoiceID=" + invoice.InvoiceID + "&PT_CustomerName=" + PT_CustomerName + "&PT_PaymentAmount=" + txtAmount.Text + "&PT_ReturnUrl=" + PT_ReturnUrl);
            }

        }

    }

    #endregion
    
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

}