using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text.RegularExpressions;

public partial class Invoice_WebPayV2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Utilities.SetNoCache(Response);
            Utilities.UpdatePageHeaderV2(Page.Master, true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close_form", @"<script language=javascript>window.resizeTo(1020,980);document.getElementById('btnSubmit').click();</script>");
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string InvoiceID         = Request.QueryString["InvoiceID"];
        string PT_CustomerName   = Request.QueryString["PT_CustomerName"];
        string PT_PaymentAmount  = Request.QueryString["PT_PaymentAmount"];
        string PT_ReturnUrl      = Request.QueryString["PT_ReturnUrl"];

        string DB = null;

        if (Request.QueryString["id"] != null)
        {
            Tuple<string, int> formParams = GetFormParams(false);

            Invoice invoice = InvoiceDB.GetByID(formParams.Item2, formParams.Item1);
            if (invoice == null)
            {
                HideTableAndSetErrorMessage("Invalid invoice ID");
                return;
            }
            InvoiceID = invoice.InvoiceID.ToString();
            DB = formParams.Item1;
        }
        else
        {
            DB = Session["DB"].ToString();
        }


        int paymentPendingID = PaymentPendingDB.Insert(DB, Convert.ToInt32(InvoiceID), Convert.ToDecimal(PT_PaymentAmount), PT_CustomerName);

        string url = ConfigurationManager.AppSettings["EziDebit_URL"] + "/Payment.aspx" + 
                    "?PT_DigitalKey="       + SystemVariableDB.GetAll(DB)["EziDebit_DigitalKey"].Value +
                    "&PT_PaymentReference=" + paymentPendingID + 
                    "&PT_CustomerName="     + PT_CustomerName + 
                    "&PT_PaymentAmount="    + PT_PaymentAmount + 
                    "&PT_ReturnURL="        + PT_ReturnUrl;
                           
        Response.Redirect(url);
    }

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