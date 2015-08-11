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
using System.Xml;
using System.Text;
using System.IO;
using System.Globalization;

public partial class TyroHealthPointClaimV2 : System.Web.UI.Page
{

    // http://localhost:2524/TyroHealthPointClaimV2.aspx?invoice=335688&debug=1&send=0

    private struct TyroHealthPointClaimIten
    {
        public string claimAmount;
        public string serviceCode;
        public string description;
        public string serviceReference;
        public string patientId;
        public string serviceDate;

        public TyroHealthPointClaimIten(
            string claimAmount, 
            string serviceCode, 
            string description, 
            string serviceReference, 
            string patientId, 
            string serviceDate)
        {
            this.claimAmount = claimAmount;
            this.serviceCode = serviceCode;
            this.description = description;
            this.serviceReference = serviceReference;
            this.patientId = patientId;
            this.serviceDate = serviceDate;
        }
    }


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
                if (invoice.Booking == null)
                    throw new CustomMessageException("Invalid invoice in URL - invoice not attached to a booking");


                string refTag = Request.QueryString["reftag"];
                if (refTag != null)
                {
                    lblHeading.InnerHtml = "Cancel Tyro Payment";

                    TyroHealthClaim tyroHealthClaim = TyroHealthClaimDB.GetByRefTag(refTag);
                    if (tyroHealthClaim == null)
                        throw new CustomMessageException("Invalid reftag in URL");
                    if (tyroHealthClaim.InvoiceID != invoice.InvoiceID)
                        throw new CustomMessageException("Invoice and reftag in URL do not match");
                    if (tyroHealthClaim.OutResult != "APPROVED")
                        throw new CustomMessageException("Non-approved claims can not be cancelled");
                    if (tyroHealthClaim.DateCancelled != DateTime.MinValue)
                        throw new CustomMessageException("This claim has already been cancelled");

                    btnInitiateHealthPointClaim.Visible = false;
                    btnCancelHealthPointClaim.Visible   = true;
                }


                if (refTag == null && (invoice.IsPaID || invoice.TotalDue <= 0))
                    throw new CustomMessageException("Invoice already paid");
                if (refTag == null && invoice.ReceiptsTotal > 0)
                    throw new CustomMessageException("Invoice already has a payment on it");

                SetInvoiceInfo(invoice);
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
        SetInvoiceInfo(InvoiceDB.GetByID(Convert.ToInt32(Request.QueryString["invoice"])));
    }

    protected void SetInvoiceInfo(Invoice invoice)
    {
        bool isDebug = Request["debug"] != null && Request["debug"] == "1";
        bool useOnlyTestItems = false;

        SaveType saveType = Request.QueryString["reftag"] != null ? SaveType.Cancellation : SaveType.Claim;

        string receiptString = string.Empty;
        foreach(Receipt receipt in ReceiptDB.GetByInvoice(invoice.InvoiceID, false))
            receiptString += (receiptString.Length == 0 ? "" : ", ") + "$" + receipt.Total.ToString();

        string invoiceViewURL  = "/Invoice_ViewV2.aspx?invoice_id=" + invoice.InvoiceID;
        lblInvoiceID.Text      = "<a href=\"" + invoiceViewURL + "\" onclick=\"open_new_tab('" + invoiceViewURL + "');return false;\">" + invoice.InvoiceID + "</a>";
        lblInvoiceTotal.Text   = "$" + invoice.Total.ToString();
        lblInvoiceOwing.Text   = "$" + invoice.TotalDue.ToString();
        lblReceiptedTotal.Text = "$" + invoice.ReceiptsTotal.ToString() + (invoice.CreditNotesTotal == 0 ? "" : " &nbsp;&nbsp;($" + invoice.CreditNotesTotal.ToString() + " Credit Noted)") + (invoice.RefundsTotal == 0 ? "" : " &nbsp;&nbsp;($" + invoice.RefundsTotal.ToString() + " Refunds)");
        lblDebtor.Text         = invoice.GetDebtor(true);
        lblBkDate.Text         = invoice.Booking.DateStart.ToString("d MMM, yyyy");
        lblBkOrgText.Text      = invoice.Booking.Organisation.IsAgedCare? "Facility" : "Clinic";
        lblBkOrg.Text          = invoice.Booking.Organisation.Name;

        System.Data.DataTable tbl = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "field_id=" + invoice.Booking.Provider.Field.ID, "", "field_id", "descr");
        invoice.Booking.Provider.Field = IDandDescrDB.Load(tbl.Rows[0], "field_id", "descr");

        RegisterStaff regStaff = RegisterStaffDB.GetByStaffIDAndOrganisationID(invoice.Booking.Provider.StaffID, invoice.Booking.Organisation.OrganisationID);
        if (regStaff == null)
            throw new CustomMessageException("Staff Member Not Set To This Clinic/Fac.");

        InvoiceLine[] invLines = InvoiceLineDB.GetByInvoiceID(invoice.InvoiceID);
        Hashtable patientHealthCardCache = PatientsHealthCardsCacheDB.GetBullkActive(invLines.Select(x => x.Patient.PatientID).ToArray());

        List<TyroHealthPointClaimIten> claimItems = new List<TyroHealthPointClaimIten>();
        for (int i = 0; i < invLines.Length; i++)
        {
            HealthCard hc = GetHealthCardFromCache(patientHealthCardCache, invLines[i].Patient.PatientID);

            string ptURL  = "PatientDetailV2.aspx?type=view&id=" + invLines[i].Patient.PatientID;
            string ptLink = "<a href=\"" + ptURL + "\" onclick=\"open_new_tab('" + ptURL + "');return false;\">" + invLines[i].Patient.Person.FullnameWithoutMiddlename + "</a>";

            if (hc == null)
                throw new CustomMessageException("No healthcard found for " + ptLink + " (PT-ID:" + invLines[i].Patient.PatientID + ")");
            if (hc.Organisation.OrganisationType.OrganisationTypeID != 150)
                throw new CustomMessageException("Healthcard found for " + ptLink + " (PT-ID:" + invLines[i].Patient.PatientID + ") Is Not An Insurance Card");

            /*
            claim-amount:      claim amount in cents                    - max 10 digits
            service-code:      item number service code                 - max 5  characters
            description:       description of item to appear on receipt - max 32 characters
            service-reference: body part or tooth number suffix         - max 3  characters
            patient-id:        patient ID on card                       - exactly 2 digits
            service-date:      claim date in YYYYMMDD format
            */

            isDebug = true;

            claimItems.Add(new TyroHealthPointClaimIten(
                    ((int)(invLines[i].Price * 100)).ToString(),
                    isDebug ? "F1234" : invLines[i].Offering.TacCompanyCode,
                    isDebug ? "Face"  : invLines[i].AreaTreated,
                    invLines[i].ServiceReference,
                    "",                 // family number on card -- legally they have to enter it themselves
                    isDebug ? DateTime.Today.ToString("yyyyMMdd") : invoice.Booking.DateStart.ToString("yyyyMMdd")));
        }


        //useOnlyTestItems = true;

        // save variables & JSON array on the page accessable to JS to send to Tyro
        if (useOnlyTestItems)
        {
            claimItems = new List<TyroHealthPointClaimIten>();

            claimItems.Add(new TyroHealthPointClaimIten(
                    "10000",
                    "00001",
                    "SKULL XRAY",
                    "01",
                    "02",
                    DateTime.Today.ToString("yyyyMMdd")));

            claimItems.Add(new TyroHealthPointClaimIten(
                    "15000",
                    "00001",
                    "SKULL XRAY",
                    "01",
                    "02",
                    DateTime.Today.ToString("yyyyMMdd")));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "invoice_items",
                @"<script language=javascript>
                var _providerId       = '4237955J';
                var _serviceType      = 'D';
                var _claimItemsCount  = '2';
                var _totalClaimAmount = '25000';
                var _allClaimItems    = " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(claimItems) + @"; 
             </script>");
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "invoice_items",
                @"<script language=javascript>
                    var _providerId       = '" + (isDebug ? "4237955J" : (regStaff.ProviderNumber.Length > 0 ? regStaff.ProviderNumber : invoice.Booking.Provider.ProviderNumber)) + @"';
                    var _serviceType      = '" + GetServiceTypeHashtable()[invoice.Booking.Provider.Field.Descr.ToLower()].ToString() + @"';
                    var _claimItemsCount  = '" + invLines.Length.ToString() + @"';
                    var _totalClaimAmount = '" + ((int)(invLines.Sum(item => item.Price) * 100)).ToString() + @"';
                    var _allClaimItems    = "  + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(claimItems) /* convert to JSON array */ + @"; 
                    " + (saveType == SaveType.Cancellation ? "var _refTag = " + Request.QueryString["reftag"] : string.Empty) + @"
                 </script>");
        }
    }
    protected HealthCard GetHealthCardFromCache(Hashtable patientHealthCardCache, int patientID)
    {
        HealthCard hc = null;
        if (patientHealthCardCache[patientID] != null)
        {
            PatientActiveHealthCards hcs = (PatientActiveHealthCards)patientHealthCardCache[patientID];
            if (hcs.MedicareCard != null)
                hc = hcs.MedicareCard;
            if (hcs.DVACard != null)
                hc = hcs.DVACard;
            if (hcs.TACCard != null)
                hc = hcs.TACCard;
        }

        return hc;
    }

    protected Hashtable _servciceTypeHashtable = null;
    protected Hashtable GetServiceTypeHashtable()
    {
        Hashtable _hash = new Hashtable();
        _hash["General Dentist"]           = "D";
        _hash["Occupational Therapist"]    = "H";
        _hash["Optometrist"]               = "O";
        _hash["Osteopath"]                 = "I";
        _hash["Physiotherapist"]           = "P";
        _hash["Podiatrist"]                = "F";
        _hash["Psychologist"]              = "Y";
        _hash["Dietitian"]                 = "E";
        _hash["Speech Pathologist"]        = "S";
        _hash["Naturopath"]                = "N";
        _hash["Acupuncturist"]             = "B";
        _hash["Massage Therapist"]         = "M";
        _hash["Chiropractor"]              = "C";
        _hash["Exercise Physiology"]       = "U";
        _hash["Endodontist"]               = "3";
        _hash["Oral Surgeon"]              = "4";
        _hash["Orthodontist"]              = "5";
        _hash["Paedodontist (Paediatric)"] = "6";
        _hash["Periodontist"]              = "7";
        _hash["Prosthetist"]               = "8";
        _hash["Prosthodontist"]            = "9";
        _hash["Optical Dispenser"]         = "Z";
        _hash["Dental Technician"]         = "0";

        Hashtable hash = new Hashtable();
        foreach (DictionaryEntry pair in _hash)
            hash.Add(pair.Key.ToString().ToLower(), pair.Value);

        return hash;
    }

    #endregion

    #region btnSaveResult_Click, btnSaveCancellation_Click

    protected void btnSaveResult_Click(object sender, EventArgs e)
    {
        Save(SaveType.Claim);
    }
    protected void btnSaveCancellation_Click(object sender, EventArgs e)
    {
        Save(SaveType.Cancellation);
    }

    protected enum SaveType { Claim = 1, Cancellation = 2 };
    protected void Save(SaveType saveType)
    {
        if (Request.QueryString["debug"] != null && Request.QueryString["debug"] == "1")
            lblXML.Text = "<pre>" + hiddenResponse.Value.Replace("<", "&lt;").Replace(">", "&gt;").Replace(Environment.NewLine, "<br />") + "</pre>";

        Invoice       invoice  = InvoiceDB.GetByID(Convert.ToInt32(Request.QueryString["invoice"]));
        InvoiceLine[] invLines = InvoiceLineDB.GetByInvoiceID(invoice.InvoiceID);


        using (XmlReader reader = XmlReader.Create(new StringReader(hiddenResponse.Value)))
        {
            reader.ReadToFollowing("detail");

            string tyro_transaction_id                   = reader.GetAttribute("tyro_transaction_id");

            string result                                = reader.GetAttribute("result");
            string healthpointErrorCode                  = reader.GetAttribute("healthpointErrorCode");
            string healthpointErrorDescription           = reader.GetAttribute("healthpointErrorDescription");
            string healthpointRefTag                     = reader.GetAttribute("healthpointRefTag");
            string healthpointTotalBenefitAmount         = reader.GetAttribute("healthpointTotalBenefitAmount");
            string healthpointSettlementDateTime         = reader.GetAttribute("healthpointSettlementDateTime");
            string healthpointTerminalDateTime           = reader.GetAttribute("healthpointTerminalDateTime");
            string healthpointMemberNumber               = reader.GetAttribute("healthpointMemberNumber");
            string healthpointProviderId                 = reader.GetAttribute("healthpointProviderId");
            string healthpointServiceType                = reader.GetAttribute("healthpointServiceType");
            string healthpointGapAmount                  = reader.GetAttribute("healthpointGapAmount");
            string healthpointPhfResponseCode            = reader.GetAttribute("healthpointPhfResponseCode");
            string healthpointPhfResponseCodeDescription = reader.GetAttribute("healthpointPhfResponseCodeDescription");



            if (result != "APPROVED") {

                var errMsg = "<br />Result: <b>" + result + "</b>";

                if (healthpointErrorCode != "" && healthpointErrorDescription != "")
                    errMsg += "<br />" + healthpointErrorDescription + " (Error Code " + healthpointErrorCode + ")";
                else if (healthpointErrorCode != "")
                    errMsg += "<br />Error Code: " + healthpointErrorCode;
                else if (healthpointErrorDescription != "")
                    errMsg += "<br />Error: " + healthpointErrorDescription;

                lblErrorMessage.Text= errMsg;

                // Email alert this

                return;
            }




            if (saveType == SaveType.Claim)
            {
                DateTime _healthpointSettlementDateTime;
                if (healthpointSettlementDateTime == "undefined")
                    _healthpointSettlementDateTime = DateTime.MinValue;
                else if (!DateTime.TryParseExact(healthpointSettlementDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _healthpointSettlementDateTime))
                    throw new Exception("healthpointSettlementDateTime not in correct format: " + healthpointSettlementDateTime);

                DateTime _healthpointTerminalDateTime;
                if (healthpointTerminalDateTime == "undefined")
                    _healthpointTerminalDateTime = DateTime.MinValue;
                else if (!DateTime.TryParseExact(healthpointTerminalDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _healthpointTerminalDateTime))
                    throw new Exception("healthpointTerminalDateTime not in correct format: " + healthpointTerminalDateTime);

                decimal paymentAmount = Convert.ToDecimal(healthpointTotalBenefitAmount) / 100;
                decimal gapAmount     = saveType == SaveType.Claim ? Convert.ToDecimal(healthpointGapAmount) / 100 : 0;

                TyroHealthClaimDB.UpdateByTyroTransactionID(
                    tyro_transaction_id,
                    result,
                    healthpointRefTag,
                    paymentAmount,
                    _healthpointSettlementDateTime,
                    _healthpointTerminalDateTime,
                    healthpointMemberNumber,
                    healthpointProviderId,
                    healthpointServiceType,
                    gapAmount,
                    healthpointPhfResponseCode,
                    healthpointPhfResponseCodeDescription);

                TyroHealthClaim tyroHealthClaim = TyroHealthClaimDB.GetByByTyroTransactionID(tyro_transaction_id);

                while (reader.ReadToFollowing("claimItem"))
                {
                    string claimAmount      = reader.GetAttribute("claimAmount");
                    string rebateAmount     = reader.GetAttribute("rebateAmount");
                    string serviceCode      = reader.GetAttribute("serviceCode");
                    string description      = reader.GetAttribute("description");
                    string serviceReference = reader.GetAttribute("serviceReference");
                    string patientId        = reader.GetAttribute("patientId");
                    string serviceDate      = reader.GetAttribute("serviceDate");
                    string responseCode     = reader.GetAttribute("responseCode");

                    DateTime _serviceDate;
                    if (serviceDate == "undefined")
                        _serviceDate = DateTime.MinValue;
                    else if (!DateTime.TryParseExact(serviceDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _serviceDate))
                        throw new Exception("serviceDate not in correct format: " + serviceDate);

                    TyroHealthClaimItemDB.Insert(
                        tyroHealthClaim.TyroHealthClaimID,
                        Convert.ToDecimal(claimAmount)/100,
                        Convert.ToDecimal(rebateAmount) / 100,
                        serviceCode,
                        description,
                        serviceReference,
                        patientId,
                        _serviceDate,
                        responseCode);
                }


                if (result == "APPROVED")
                {
                    decimal totalOwed  = invoice.TotalDue - paymentAmount;
                    bool    isOverPaid = totalOwed <  0;
                    bool    isPaid     = totalOwed <= 0;

                    ReceiptDB.Insert(null, 365, tyroHealthClaim.InvoiceID, paymentAmount, 0, false, isOverPaid, DateTime.MaxValue, -8);

                    if (isPaid)
                        InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);

                    if (isOverPaid)
                    {
                        // send email to someone .. to fix up the overpayment....
                        Emailer.SimpleAlertEmail(
                            "Tyro healthpoint invoice late payment added and is overpaid.<br />tyro_health_claim_id: " + tyroHealthClaim.TyroHealthClaimID + "<br />Invoice: " + invoice.InvoiceID + "<br />DB: " + Session["DB"],
                            "Tyro Healthpoint Invoice OverPaid: " + invoice.InvoiceID,
                            true);
                    }
                }

                string cancelLink = "<a href='TyroHealthPointClaimV2.aspx?invoice=" + invoice.InvoiceID + "&reftag=" + healthpointRefTag + "'>Click Here</a>";
                lblResult.Text = "Result: " + result + (result == "APPROVED" ? "<br />Updated Paid Amounts Shown Above<br />To Cancel This Claim " + cancelLink : "") + "<br /><br />";

            }
            else
            {
                if (result == "APPROVED")
                {
                    TyroHealthClaimDB.UpdateCancelled(healthpointRefTag);

                    // set receipt as reversed
                    TyroHealthClaim tyroHealthClaim = TyroHealthClaimDB.GetByRefTag(healthpointRefTag);

                    Receipt[] receipts = ReceiptDB.GetByInvoice(tyroHealthClaim.InvoiceID, false);
                    if (receipts.Length != 1 || receipts[0].ReceiptPaymentType.ID != 365)
                    {
                        Emailer.SimpleAlertEmail(
                            "Tyro claim reversed (by Tyro) but multiple receipts or receipt not of type 'Tyro HC Claim'.<br />healthpointRefTag = " + healthpointRefTag + "<br />DB: " + Session["DB"],
                            "Tyro claim reversed (by Tyro) but multiple receipts or receipt not of type 'Tyro HC Claim'",
                            true
                           );
                    }

                    ReceiptDB.Reverse(receipts[0].ReceiptID, Convert.ToInt32(Session["StaffID"]));
                }

                lblResult.Text = "Cancellation Result: " + result + (result == "APPROVED" ? "<br />Updated Paid Amounts Shown Above" : "") + "<br /><br />";

            }

        }

        SetInvoiceInfo(InvoiceDB.GetByID(invoice.InvoiceID));
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