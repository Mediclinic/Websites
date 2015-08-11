using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Invoice
{

    public Invoice(int invoice_id, int entity_id, int invoice_type_id, int booking_id, int payer_organisation_id, int payer_patient_id, int non_booking_invoice_organisation_id, string healthcare_claim_number, int reject_letter_id, string message,
                   int staff_id, int site_id, DateTime invoice_date_added, decimal total, decimal gst, decimal receipts_total, decimal vouchers_total, decimal credit_notes_total, decimal refunds_total,
                   bool is_paid, bool is_refund, bool is_batched, int reversed_by, DateTime reversed_date, DateTime last_date_emailed)
    {
        this.invoice_id              = invoice_id;
        this.entity_id               = entity_id;
        this.invoice_type            = new IDandDescr(invoice_type_id);
        this.booking                 = booking_id            == -1 ? null : new Booking(booking_id);
        this.payer_organisation      = payer_organisation_id ==  0 ? null : new Organisation(payer_organisation_id);
        this.payer_patient           = payer_patient_id      == -1 ? null : new Patient(payer_patient_id);
        this.non_booking_invoice_organisation = non_booking_invoice_organisation_id == -1 ? null : new Organisation(non_booking_invoice_organisation_id);
        this.healthcare_claim_number = healthcare_claim_number;
        this.reject_letter           = reject_letter_id      == -1 ? null : new Letter(reject_letter_id);
        this.message                 = message;
        this.staff                   = new Staff(staff_id);
        this.site                    = site_id               == -1 ? null : new Site(site_id);
        this.invoice_date_added      = invoice_date_added;
        this.total                   = total;
        this.gst                     = gst;
        this.receipts_total          = receipts_total;
        this.vouchers_total          = vouchers_total;
        this.credit_notes_total      = credit_notes_total;
        this.refunds_total           = refunds_total;
        this.is_paid                 = is_paid;
        this.is_refund               = is_refund;
        this.is_batched              = is_batched;
        this.reversed_by             = reversed_by == -1 ? null : new Staff(reversed_by);
        this.reversed_date           = reversed_date;
        this.last_date_emailed       = last_date_emailed;
    }
    public Invoice(int invoice_id)
    {
        this.invoice_id = invoice_id;
    }

    private int invoice_id;
    public int InvoiceID
    {
        get { return this.invoice_id; }
        set { this.invoice_id = value; }
    }
    private int entity_id;
    public int EntityID
    {
        get { return this.entity_id; }
        set { this.entity_id = value; }
    }
    private IDandDescr invoice_type;
    public IDandDescr InvoiceType
    {
        get { return this.invoice_type; }
        set { this.invoice_type = value; }
    }
    private Booking booking;
    public Booking Booking
    {
        get { return this.booking; }
        set { this.booking = value; }
    }
    private Organisation payer_organisation;
    public Organisation PayerOrganisation
    {
        get { return this.payer_organisation; }
        set { this.payer_organisation = value; }
    }
    private Patient payer_patient;
    public Patient PayerPatient
    {
        get { return this.payer_patient; }
        set { this.payer_patient = value; }
    }
    private Organisation non_booking_invoice_organisation;
    public Organisation NonBookinginvoiceOrganisation
    {
        get { return this.non_booking_invoice_organisation; }
        set { this.non_booking_invoice_organisation = value; }
    }
    private string healthcare_claim_number;
    public string HealthcareClaimNumber
    {
        get { return this.healthcare_claim_number; }
        set { this.healthcare_claim_number = value; }
    }
    private Letter reject_letter;
    public Letter RejectLetter
    {
        get { return this.reject_letter; }
        set { this.reject_letter = value; }
    }
    private string message;
    public string Message
    {
        get { return this.message; }
        set { this.message = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    private Site site;
    public Site Site
    {
        get { return this.site; }
        set { this.site = value; }
    }
    private DateTime invoice_date_added;
    public DateTime InvoiceDateAdded
    {
        get { return this.invoice_date_added; }
        set { this.invoice_date_added = value; }
    }
    private decimal total;
    public decimal Total
    {
        get { return this.total; }
        set { this.total = value; }
    }
    private decimal gst;
    public decimal Gst
    {
        get { return this.gst; }
        set { this.gst = value; }
    }
    private decimal receipts_total;
    public decimal ReceiptsTotal
    {
        get { return this.receipts_total; }
        set { this.receipts_total = value; }
    }
    private decimal vouchers_total;
    public decimal VouchersTotal
    {
        get { return this.vouchers_total; }
        set { this.vouchers_total = value; }
    }    
    private decimal credit_notes_total;
    public decimal CreditNotesTotal
    {
        get { return this.credit_notes_total; }
        set { this.credit_notes_total = value; }
    }
    private decimal refunds_total;
    public decimal RefundsTotal
    {
        get { return this.refunds_total; }
        set { this.refunds_total = value; }
    }
    public decimal TotalDue
    {
        get { return this.total - this.receipts_total - this.VouchersTotal - this.credit_notes_total + this.refunds_total; }
    }
    private bool is_paid;
    public bool IsPaID
    {
        get { return this.is_paid; }
        set { this.is_paid = value; }
    }
    private bool is_refund;
    public bool IsRefund
    {
        get { return this.is_refund; }
        set { this.is_refund = value; }
    }
    private bool is_batched;
    public bool IsBatched
    {
        get { return this.is_batched; }
        set { this.is_batched = value; }
    }
    public bool IsReversed
    {
        get { return this.ReversedBy != null || this.ReversedDate != DateTime.MinValue; }
    }
    private Staff reversed_by;
    public Staff ReversedBy
    {
        get { return this.reversed_by; }
        set { this.reversed_by = value; }
    }
    private DateTime reversed_date;
    public DateTime ReversedDate
    {
        get { return this.reversed_date; }
        set { this.reversed_date = value; }
    }
    private DateTime last_date_emailed;
    public DateTime LastDateEmailed
    {
        get { return this.last_date_emailed; }
        set { this.last_date_emailed = value; }
    }

    public override string ToString()
    {
        return invoice_id.ToString() + " " + invoice_type.ID.ToString() + " " + booking.BookingID.ToString() + " " + payer_organisation.OrganisationID.ToString() + " " + payer_patient.PatientID.ToString() + " " +
                healthcare_claim_number.ToString() + " " + staff.StaffID.ToString() + " " + site.SiteID.ToString() + " " + invoice_date_added.ToString() + " " +
                total.ToString() + " " + gst.ToString() + " " + is_paid.ToString() + " " + is_refund.ToString() + " " + is_batched.ToString() + " " +
                reversed_by.StaffID.ToString() + " " + reversed_date.ToString();
    }


    public bool CanReverse(out string errorString)
    {
        errorString = string.Empty;

        if (this.IsReversed)
        {
            errorString = "Invoice " + this.InvoiceID + " is already reversed";
            return false;
        }


        if (this.ReceiptsTotal > 0 && this.VouchersTotal> 0)
        {
            errorString = "Invoice " + this.InvoiceID + " has receipts and vouchers used that need to be reversed";
            return false;
        }
        else if (this.ReceiptsTotal > 0)
        {
            errorString =  "Invoice " + this.InvoiceID + " has receipts that need to be reversed";
            return false;
        }
        else if (this.VouchersTotal > 0)
        {
            errorString = "Invoice " + this.InvoiceID + " has vouchers used that need to be reversed";
            return false;
        }

        return true;
    }

    public string GetDebtor(bool asLink = false)
    {
        string name = null;
        string url  = null;

        if (this.PayerOrganisation != null)
        {
            name = this.PayerOrganisation.Name;
            if (this.PayerOrganisation.OrganisationID != -1 && this.PayerOrganisation.OrganisationID != -2 || this.PayerOrganisation.OrganisationType.OrganisationTypeID != 150)
                url = "OrganisationDetailV2.aspx?type=view&id=" + this.PayerOrganisation.OrganisationID;
        }
        else if (this.PayerPatient != null)
        {
            name = this.PayerPatient.Person.FullnameWithoutMiddlename;
            url = "PatientDetailV2.aspx?type=view&id=" + this.PayerPatient.PatientID;
        }
        else
        {
            if (this.Booking != null && this.Booking.Patient != null)
            {
                // can add this query each row because in the whole system there is only 32 invoices that get to here
                // since the rest keep the patient as the payer_patient
                // and doing this for only 32 rows avoids pulling all the extra data for all invoices so its faster doing this
                Booking booking = BookingDB.GetByID(this.Booking.BookingID);
                name = booking.Patient.Person.FullnameWithoutMiddlename;
                url = "PatientDetailV2.aspx?type=view&id=" + booking.Patient.PatientID;
            }
            else // no debtor for some cash invoices
            {
                ;
            }
        }


        if (!asLink)
        {
            return name == null ? string.Empty : name;
        }
        else
        {
            if (name == null)
                return string.Empty;
            else if (url == null)
                return name;
            else
                return "<a href=\"#\" onclick=\"open_new_tab('" + url + "');return false;\">" + name + "</a>";
        }
    }

    public static string EncodeInvoiceHash(int invoiceID, string DB)
    {
        string inString = DB + "__" + invoiceID.ToString().PadLeft(6, '0');
        return SimpleAES.Encrypt(SimpleAES.KeyType.Invoices, inString);
    }
    public static bool IsValidInvoiceHash(string inString)
    {
        if (inString == null)
            return false;

        inString = inString.Replace(" ", "+");

        string result = SimpleAES.Decrypt(SimpleAES.KeyType.Invoices, inString);
        if (result == null || !System.Text.RegularExpressions.Regex.IsMatch(result, @"^Mediclinic_\d{4}__\d+$"))
            return false;

        string[] resultSplit = result.Split(new string[] { "__" }, StringSplitOptions.None);
        string DB = resultSplit[0];
        string invNbr = resultSplit[1];

        if (!Utilities.IsValidDB(DB.Substring(DB.Length-4)))
            return false;

        if (InvoiceDB.GetByID(Convert.ToInt32(invNbr), DB) == null)
            return false;

        return true;
    }
    public static Tuple<string, int> DecodeInvoiceHash(string inString)
    {
        if (inString == null)
            return null;

        inString = inString.Replace(" ", "+");

        string result = SimpleAES.Decrypt(SimpleAES.KeyType.Invoices, inString);
        if (result == null || !System.Text.RegularExpressions.Regex.IsMatch(result, @"^Mediclinic_\d{4}__\d+$"))
            return null;

        string[] resultSplit = result.Split(new string[] { "__" }, StringSplitOptions.None);
        string DB = resultSplit[0];
        string invNbr = resultSplit[1];

        return new Tuple<string, int>(DB, Convert.ToInt32(invNbr));
    }

}