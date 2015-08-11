using System;
using System.Web;
using System.Text.RegularExpressions;

public partial class AjaxTyro : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            UrlParamType urlParamType = GetUrlParamType();

            if (urlParamType == UrlParamType.Insert)
                Insert();
            else if (urlParamType == UrlParamType.Update)
                Update();
            else if (urlParamType == UrlParamType.InsertHealthPoint)
                InsertHealthPoint();
            else if (urlParamType == UrlParamType.CanCancelHealthPointClaim)
                CanCancelHealthPointClaim();
            else
                throw new Exception("Unknown type");
        }
        catch (SessionTimedOutException)
        {
            Utilities.UnsetSessionVariables();
            Response.Write("SessionTimedOutException");
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "please contact system administrator."));
        }
    }

    #endregion

    #region Insert, Update

    protected void Insert()
    {
        Invoice  invoice              = UrlInvoice;
        int?     tyro_payment_type_id = UrlTyroPaymentTypeID;
        decimal? amount               = UrlAmount;
        decimal? cashout              = UrlCashout;

        if (invoice == null)
            throw new Exception("Invalid url field invoice_id");
        if (tyro_payment_type_id == null)
            throw new Exception("Invalid url field tyro_payment_type_id");
        if (amount == null)
            throw new Exception("Invalid url field amount");
        if (cashout == null)
            throw new Exception("Invalid url field cashout");

        if (tyro_payment_type_id.Value == 1 && amount.Value > invoice.TotalDue)
        {
            Response.Write("OVERPAYMENT:$" + invoice.TotalDue);
            return;
        }
        if (tyro_payment_type_id.Value == 2 && amount.Value > invoice.ReceiptsTotal)
        {
            Response.Write("OVERREFUND:$" + invoice.ReceiptsTotal);
            return;
        }

        if (tyro_payment_type_id == 2)  // make it negative for refunds
            amount = amount.Value * -1;

        Tuple<int, string> dbFieds = TyroPaymentPendingDB.Insert(invoice.InvoiceID, Session["DB"].ToString(), tyro_payment_type_id.Value, amount.Value, cashout.Value);
        Response.Write(dbFieds.Item2);  // tyro_transaction_id
    }

    protected void Update()
    {
        string tyro_transaction_id      = UrlTyroTransactionID;
        string out_result               = UrlOutResult;
        string out_cardType             = UrlOutCardType;
        string out_transactionReference = UrlOutTransactionReference;
        string out_authorisationCode    = UrlOutAuthorisationCode;
        string out_issuerActionCode     = UrlOutIssuerActionCode;

        if (tyro_transaction_id == null)
            throw new Exception("Invalid url field tyro_transaction_id");
        if (out_result == null)
            throw new Exception("Invalid url field out_result");
        if (out_cardType == null)
            throw new Exception("Invalid url field out_cardType");
        if (out_transactionReference == null)
            throw new Exception("Invalid url field out_transactionReference");
        if (out_authorisationCode == null)
            throw new Exception("Invalid url field out_authorisationCode");
        if (out_issuerActionCode == null)
            throw new Exception("Invalid url field out_issuerActionCode");

        TyroPaymentPendingDB.UpdateByTyroTransactionID(null, tyro_transaction_id, out_result, out_cardType, out_transactionReference, out_authorisationCode, out_issuerActionCode, DateTime.Now);

        if (out_result == "APPROVED")
        {
            TyroPaymentPending tyroPaymentPending = TyroPaymentPendingDB.GetByByTyroTransactionID(null, tyro_transaction_id);

            Invoice invoice = InvoiceDB.GetByID(tyroPaymentPending.InvoiceID);

            int staffID = Session == null || Session["StaffID"] == null ? -8 : Convert.ToInt32(Session["StaffID"]);

            if (tyroPaymentPending.TyroPaymentTypeID == 1) // payment
            {
                decimal totalOwed  = invoice.TotalDue - tyroPaymentPending.Amount;
                bool    isOverPaid = totalOwed <  0;
                bool    isPaid     = totalOwed <= 0;

                ReceiptDB.Insert(null, 364, tyroPaymentPending.InvoiceID, tyroPaymentPending.Amount, 0, false, isOverPaid, DateTime.MaxValue, staffID);

                if (tyroPaymentPending.Cashout > 0)
                    CreditDB.Insert_Cashout(tyroPaymentPending.Cashout, tyroPaymentPending.TyroPaymentPendingID, staffID);

                if (isPaid)
                    InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);

                if (isOverPaid)
                {
                    // send email to someone .. to fix up the overpayment....
                    Emailer.SimpleAlertEmail(
                        "Invoice tyro payment added and is overpaid.<br />tyro_payment_pending_id: " + tyroPaymentPending.TyroPaymentPendingID + "<br />Invoice: " + invoice.InvoiceID + "<br />DB: " + (Session == null || Session["DB"] == null ? "" : Session["DB"]),
                        "Tyro Invoice OverPaid: " + invoice.InvoiceID,
                        true);
                }
            }
            if (tyroPaymentPending.TyroPaymentTypeID == 2) // refund
            {
                decimal totalOwed  = invoice.TotalDue + tyroPaymentPending.Amount;
                bool    isPaid     = totalOwed <= 0;

                RefundDB.Insert(tyroPaymentPending.InvoiceID, tyroPaymentPending.Amount, 308, "", staffID);

                if (totalOwed > 0)
                    InvoiceDB.UpdateIsPaid(null, tyroPaymentPending.InvoiceID, false);
            }
        }

        Response.Write("1"); // to indicate it was successful
    }

    protected void InsertHealthPoint()
    {
        Invoice invoice = UrlInvoice;
        decimal? amount = UrlAmount;

        if (invoice == null)
            throw new Exception("Invalid url field invoice_id");
        if (amount == null)
            throw new Exception("Invalid url field amount");


        if (invoice.IsPaID)
        {
            Response.Write("ISPAID");
            return;
        }
        if (invoice.ReceiptsTotal > 0)
        {
            Response.Write("HASPAYMENT");
            return;
        }

        Tuple<int, string> dbFieds = TyroHealthClaimDB.Insert(invoice.InvoiceID, Session["DB"].ToString(), amount.Value);
        Response.Write(dbFieds.Item2);  // tyro_transaction_id
    }

    protected void CanCancelHealthPointClaim()
    {
        string  reftag  = UrlRefTag;

        if (reftag == null)
            throw new Exception("Invalid url field reftag");


        TyroHealthClaim tyroHealthClaim = TyroHealthClaimDB.GetByRefTag(reftag);

        if (tyroHealthClaim == null)
        {
            Response.Write("Error: Invalid reftag");
            return;
        }
        if (tyroHealthClaim.OutResult != "APPROVED")
        {
            Response.Write("Non-approved claims can not be cancelled");
            return;
        }
        if (tyroHealthClaim.DateCancelled != DateTime.MinValue)
        {
            Response.Write("This claim has already been cancelled");
            return;
        }

        Response.Write("OK");
    }

    #endregion

    #region Url Fields, GetUrlParamType

    protected enum UrlParamType { Insert, Update, InsertHealthPoint, CanCancelHealthPointClaim, None };
    protected UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "insert")
            return UrlParamType.Insert;
        else if (type != null && type.ToLower() == "update")
            return UrlParamType.Update;
        else if (type != null && type.ToLower() == "insert_healthpoint")
            return UrlParamType.InsertHealthPoint;
        else if (type != null && type.ToLower() == "can_cancel_healthpoint")
            return UrlParamType.CanCancelHealthPointClaim;
        else
            return UrlParamType.None;
    }

    protected Invoice UrlInvoice
    {
        get
        {
            try
            {
                string invoice_id = Request.QueryString["invoice_id"];
                if (invoice_id == null || !Regex.IsMatch(invoice_id, @"^\d+$"))
                    return null;
                return InvoiceDB.GetByID(Convert.ToInt32(invoice_id));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    protected int? UrlTyroPaymentTypeID
    {
        get
        {
            string tyro_payment_type_id = Request.QueryString["tyro_payment_type_id"];
            if (tyro_payment_type_id == null || (tyro_payment_type_id != "1" && tyro_payment_type_id != "2"))
                return null;
            return Convert.ToInt32(tyro_payment_type_id);
        }
    }
    protected decimal? UrlAmount
    {
        get
        {
            string amount = Request.QueryString["amount"];
            int value;
            if (amount == null || !Regex.IsMatch(amount, @"^\d+$") || !int.TryParse(amount, out value))
                return null;
            else
                return ((decimal)value)/100;
        }
    }
    protected decimal? UrlCashout
    {
        get
        {
            string cashout = Request.QueryString["cashout"];
            int value;
            if (cashout == null || !Regex.IsMatch(cashout, @"^\d+$") || !int.TryParse(cashout, out value))
                return null;
            else
                return ((decimal)value) / 100;
        }
    }

    protected string UrlTyroTransactionID
    {
        get { return Request.QueryString["tyro_transaction_id"]; }
    }
    protected string UrlOutResult
    {
        get { return Request.QueryString["out_result"]; }
    }
    protected string UrlOutCardType
    {
        get { return Request.QueryString["out_cardType"]; }
    }
    protected string UrlOutTransactionReference
    {
        get { return Request.QueryString["out_transactionReference"]; }
    }
    protected string UrlOutAuthorisationCode
    {
        get { return Request.QueryString["out_authorisationCode"]; }
    }
    protected string UrlOutIssuerActionCode
    {
        get { return Request.QueryString["out_issuerActionCode"]; }
    }

    protected string UrlRefTag
    {
        get { return Request.QueryString["reftag"]; }
    }

    #endregion

}