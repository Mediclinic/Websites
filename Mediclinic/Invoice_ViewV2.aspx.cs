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
using System.Web.UI.HtmlControls;

public partial class Invoice_ViewV2 : System.Web.UI.Page
{
    bool vouchersDiv = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (GetFormIsPopup())
                Utilities.UpdatePageHeaderV2(Page.Master, true);
            btnClose.Visible = GetFormIsPopup();

            if (!IsPostBack)
            {
                FillInvoicesList();
            }

            if (Session["UpdateFromWebPay"] != null)
            {
                PaymentPendingDB.UpdateAllPaymentsPending(null, DateTime.Now.AddDays(-15), DateTime.Now.AddDays(1), Convert.ToInt32(Session["StaffID"]));
                Session.Remove("UpdateFromWebPay");
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

    #region IsValidFormParam(), GetFormParam()

    private bool IsValidFormBooking()
    {
        string id = Request.QueryString["booking_id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private Booking GetFormBooking()
    {
        if (!IsValidFormBooking())
            throw new Exception("Invalid booking id");

        int id = Convert.ToInt32(Request.QueryString["booking_id"]);
        Booking booking = BookingDB.GetByID(id);
        return booking;
    }

    private bool IsValidFormInvoice()
    {
        string id = Request.QueryString["invoice_id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private Invoice GetFormInvoice()
    {
        if (!IsValidFormInvoice())
            throw new Exception("Invalid invoice id");

        int id = Convert.ToInt32(Request.QueryString["invoice_id"]);
        Invoice invoice = InvoiceDB.GetByID(id);
        return invoice;
    }

    private bool GetFormIsPopup()
    {
        return Request.QueryString["is_popup"] == null || Request.QueryString["is_popup"].ToString() == "1";
    }

    #endregion


    protected void FillInvoicesList()
    {
        try
        {
            UserView userView        = UserView.GetInstance();
            int      loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);


            if (IsValidFormBooking())
            {
                Booking booking = GetFormBooking();
                if (booking == null)
                    throw new CustomMessageException("Invalid Booking");

                bool canSeeInvoiceInfo = userView.IsAdminView || userView.IsPrincipal || (booking.Provider != null && loggedInStaffID == booking.Provider.StaffID && booking.DateStart > DateTime.Today.AddMonths(-2));
                if (!canSeeInvoiceInfo)
                    throw new CustomMessageException("You don't have access to see this invoice.");

                DataTable dt_invoices = InvoiceDB.GetDataTable_ByBookingID(booking.BookingID, true);
                FillInvoicesList(dt_invoices);
            }
            else if (IsValidFormInvoice())
            {
                Invoice invoice = GetFormInvoice();
                if (invoice == null)
                    throw new CustomMessageException("Invalid Invoice");

                bool canSeeInvoiceInfo = invoice.Booking != null ?
                    userView.IsAdminView || userView.IsPrincipal || (invoice.Booking != null && invoice.Booking.Provider != null && loggedInStaffID == invoice.Booking.Provider.StaffID && invoice.Booking.DateStart > DateTime.Today.AddMonths(-2))
                    :
                    userView.IsAdminView || userView.IsPrincipal || (loggedInStaffID == invoice.Staff.StaffID && invoice.InvoiceDateAdded > DateTime.Today.AddMonths(-2));
                if (!canSeeInvoiceInfo)
                    throw new CustomMessageException("You don't have access to see this invoice.");


                if (invoice.PayerOrganisation != null && invoice.PayerOrganisation.OrganisationID == -1)
                    lblHeading.Text = invoice.IsReversed ? "View *Reversed* Medicare Invoice" : "View Medicare Invoice";
                else if (invoice.PayerOrganisation != null && invoice.PayerOrganisation.OrganisationID == -2)
                    lblHeading.Text = invoice.IsReversed ? "View *Reversed* DVA Invoice" : "View DVA Invoice";
                else if (invoice.PayerOrganisation != null)
                {
                    string orgTypeDescr = invoice.PayerOrganisation.OrganisationType.OrganisationTypeID == 218 ? "Clinic" : "Facility";
                    lblHeading.Text = invoice.IsReversed ? "View *Reversed* " + orgTypeDescr + "Payable Invoice" : "View " + orgTypeDescr + "Payable Invoice";
                }
                else if (invoice.PayerOrganisation == null && invoice.PayerPatient != null)
                    lblHeading.Text = invoice.IsReversed ? "View *Reversed* PT Payable Invoice" : "View PT Payable Invoice";


                DataTable dt_invoices = InvoiceDB.GetDataTable_ByID(invoice.InvoiceID);
                FillInvoicesList(dt_invoices);
            }
            else
                throw new CustomMessageException();
        }
        catch (CustomMessageException cmEx)
        {
            HideTableAndSetErrorMessage(cmEx.Message);
        }
    }
    protected void FillInvoicesList(DataTable dt_invoices)
    {
        Invoice[] invoices   = new Invoice[dt_invoices.Rows.Count];
        int[]     invoiceIDs = new int[dt_invoices.Rows.Count];


        int countShowing = 0;
        dt_invoices.Columns.Add("message_reversed_wiped", typeof(string));
        dt_invoices.Columns.Add("td_name",                typeof(string)); // for use of td 'name' tag to hide all reversed or hide all rejected
        dt_invoices.Columns.Add("style_display",          typeof(string)); // to set initially reversed and/or rejected as hidden
        dt_invoices.Columns.Add("inv_debtor",             typeof(string));
        dt_invoices.Columns.Add("inv_total_due",          typeof(decimal));
        for (int i = 0; i < dt_invoices.Rows.Count; i++)
        {
            Invoice invoice = InvoiceDB.LoadAll(dt_invoices.Rows[i]);

            invoiceIDs[i] = invoice.InvoiceID;
            invoices[i]   = invoice;

            if (invoice.ReversedBy != null)
            {
                dt_invoices.Rows[i]["message_reversed_wiped"] = "Reversed";
                dt_invoices.Rows[i]["td_name"]                = "td_reversed";
                dt_invoices.Rows[i]["style_display"]          = "none";
            }
            else if (invoice.PayerOrganisation != null && (invoice.PayerOrganisation.OrganisationID == -1 || invoice.PayerOrganisation.OrganisationID == -2) && invoice.Total > 0 && invoice.CreditNotesTotal >= invoice.Total)
            {
                dt_invoices.Rows[i]["message_reversed_wiped"] = "Rejected";
                dt_invoices.Rows[i]["td_name"]                = "td_rejected";
                dt_invoices.Rows[i]["style_display"]          = "none";
            }
            else
            {
                countShowing++;
            }

            if (invoice.PayerOrganisation != null)
                dt_invoices.Rows[i]["inv_debtor"] = invoice.PayerOrganisation.Name;
            else if (invoice.PayerPatient != null)
                dt_invoices.Rows[i]["inv_debtor"] = invoice.PayerPatient.Person.FullnameWithoutMiddlename;
            else
                dt_invoices.Rows[i]["inv_debtor"] = invoice.Booking                != null && 
                                                    invoice.Booking.Patient        != null && 
                                                    invoice.Booking.Patient.Person != null    ? invoice.Booking.Patient.Person.FullnameWithoutMiddlename : string.Empty; // empty for invoices without bookings

            dt_invoices.Rows[i]["inv_total_due"] = invoice.TotalDue.ToString();
        }


        // single db call to get invoicelines into hashtable lookup by invoice
        Hashtable invoiceLinesHash = InvoiceLineDB.GetBulkInvoiceLinesByInvoiceID(invoices);

        dt_invoices.Columns.Add("inv_lines_text", typeof(string));
        for (int i = 0; i < dt_invoices.Rows.Count; i++)
        {
            Invoice invoice = InvoiceDB.LoadAll(dt_invoices.Rows[i]);
            InvoiceLine[] invLines = (InvoiceLine[])invoiceLinesHash[invoice.InvoiceID];

            bool showAreaTreated      = invoice.PayerOrganisation != null && (invoice.PayerOrganisation.OrganisationID == -2 || invoice.PayerOrganisation.OrganisationType.OrganisationTypeID == 150);
            bool showServiceReference = invoice.PayerOrganisation != null && invoice.PayerOrganisation.OrganisationType.OrganisationTypeID == 150;

            string output = "<ul style=\"padding-left:14px;\">";
            foreach (InvoiceLine invLine in invLines)
            {
                string extras = string.Empty;
                if (showAreaTreated || showServiceReference)
                {
                    string linkAreaTreated      = "<a title=\"Edit\" onclick=\"javascript:window.showModalDialog('Invoice_UpdateAreaTreatedV2.aspx?inv_line="      + invLine.InvoiceLineID + "', '', 'dialogWidth:600px;dialogHeight:275px;center:yes;resizable:no; scroll:no');window.location.href=window.location.href;return false;\" href=\"#\">Edit</a>";
                    string linkServiceReference = "<a title=\"Edit\" onclick=\"javascript:window.showModalDialog('Invoice_UpdateServiceReferenceV2.aspx?inv_line=" + invLine.InvoiceLineID + "', '', 'dialogWidth:600px;dialogHeight:275px;center:yes;resizable:no; scroll:no');window.location.href=window.location.href;return false;\" href=\"#\">Edit</a>";
                    
                    extras += "<table>";
                    if (showAreaTreated)
                        extras += "<tr><td>Area Treated</td><td style=\"min-width:10px;\"></td><td>" + (invLine.AreaTreated.Length == 0 ? "[EMPTY]" : invLine.AreaTreated) + "</td><td style=\"min-width:10px;\"></td><td>" + linkAreaTreated + "</td></tr>";
                    if (showServiceReference)
                        extras += "<tr><td>Service Reference</td><td style=\"min-width:10px;\"></td><td>" + (invLine.ServiceReference.Length == 0 ? "[EMPTY]" : invLine.ServiceReference) + "</td><td style=\"min-width:10px;\"></td><td>" + linkServiceReference + "</td></tr>";
                    extras += "</table>";
                }

                output += "<li>" + (invLine.Offering == null ? "" : invLine.Offering.Name) + " x " + ((invLine.Quantity % 1) == 0 ? Convert.ToInt32(invLine.Quantity) : invLine.Quantity) + " = " + invLine.Price + (invLine.Tax == 0 ? "" : " (<i>Inc GST</i>)") + (invLine.Patient.Person == null ? "" : " [" + invLine.Patient.Person.FullnameWithoutMiddlename + "]") + extras + "</li>";
            }
            output += "</ul>";

            dt_invoices.Rows[i]["inv_lines_text"] = output;

            if (countShowing == 0)
                dt_invoices.Rows[i]["style_display"] = "";
        }


        //get approximate page width...
        // 194 = row titles
        // average row = 340 px (about 220-440)
        // add 70px for good measure
        int pageWidth = 194 + 365 * (countShowing == 0 ? 1 : countShowing) + 120;
        Page.ClientScript.RegisterStartupScript(this.GetType(), "resize_window", "<script language=javascript>window.resizeTo(  (" + pageWidth + "+ window.outerWidth - window.innerWidth) < screen.width ? (" + pageWidth + " + window.outerWidth - window.innerWidth) : screen.width , window.outerHeight);</script>");






        if (dt_invoices.Rows.Count <= 1)
        {
            divToggleShowReversedRejected.Visible = false;
        }
        else if (countShowing == 0)
        {
            chkShowReversed.Checked = true;
            chkShowRejected.Checked = true;
        }


        // now databind
        Repeater1.DataSource  = dt_invoices; Repeater1.DataBind();
        Repeater2.DataSource  = dt_invoices; Repeater2.DataBind();
        Repeater3.DataSource  = dt_invoices; Repeater3.DataBind();
        Repeater4.DataSource  = dt_invoices; Repeater4.DataBind();
        Repeater5.DataSource  = dt_invoices; Repeater5.DataBind();
        Repeater6.DataSource  = dt_invoices; Repeater6.DataBind();
        Repeater7.DataSource  = dt_invoices; Repeater7.DataBind();
        Repeater8.DataSource  = dt_invoices; Repeater8.DataBind();
        Repeater9.DataSource  = dt_invoices; Repeater9.DataBind();
        Repeater10.DataSource = dt_invoices; Repeater10.DataBind();
        Repeater11.DataSource = dt_invoices; Repeater11.DataBind();
        Repeater12.DataSource = dt_invoices; Repeater12.DataBind();
        Repeater13.DataSource = dt_invoices; Repeater13.DataBind();
        Repeater14.DataSource = dt_invoices; Repeater14.DataBind();
        Repeater15.DataSource = dt_invoices; Repeater15.DataBind();
        Repeater16.DataSource = dt_invoices; Repeater16.DataBind();
        Repeater17.DataSource = dt_invoices; Repeater17.DataBind();
        Repeater18.DataSource = dt_invoices; Repeater18.DataBind();
        Repeater19.DataSource = dt_invoices; Repeater19.DataBind();


        // non booking invoices (ie standard invoices) will not have a booking
        Booking booking = invoices[0].Booking;
        if (booking != null)
        {
            string patientText = string.Empty;
            if (booking != null && booking.Patient != null)
                patientText = booking.Patient.Person.FullnameWithoutMiddlename;
            else if (invoices[0].PayerPatient != null)
                patientText = invoices[0].PayerPatient.Person.FullnameWithoutMiddlename;
            else
                patientText = "< No patient >";


            // show booking info
            lblBooking_Org.Text                 = booking.Organisation.Name;
            lblBooking_Provider.Text            = booking.Provider.Person.FullnameWithoutMiddlename;
            lblBooking_Patient.Text             = patientText; // booking.Patient.Person.FullnameWithoutMiddlename;
            lblBooking_Offering.Text            = booking.Offering == null ? "< No service >" : booking.Offering.Name;
            lblBooking_BookingStatus.Text       = booking.BookingStatus.Descr;
            lblBooking_Time.Text                = booking.DateStart.Date.ToString("dd MMM yyyy") + " - " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + "-" + booking.DateEnd.ToString("h:mm") + (booking.DateEnd.Hour < 12 ? "am" : "pm");
            lblBooking_PatientMissedAppt.Text   = booking.IsPatientMissedAppt ? "Yes" : "No";
            lblBooking_ProviderMissedAppt.Text  = booking.IsProviderMissedAppt ? "Yes" : "No";
            lblBooking_Emergency.Text           = booking.IsEmergency ? "Yes" : "No";
            lblBooking_Notes.Text               = Note.GetPopupLinkTextV2(15, booking.EntityID, booking.NoteCount > 0, true, 1050, 530, "images/notes-bw-24.jpg", "images/notes-24.png");
        }
        else
        {
            booking_space.Visible                  = false;
            booking_title.Visible                  = false;
            booking_offering.Visible               = false;
            booking_patient.Visible                = false;
            booking_provider.Visible               = false;
            booking_org.Visible                    = false;
            booking_status.Visible                 = false;
            booking_apptmt_time.Visible            = false;
            booking_patiemt_missed_apptmt.Visible  = false;
            booking_provider_missed_apptmt.Visible = false;
            booking_isemergency.Visible            = false;
            booking_notes.Visible                  = false;
        }
    }

    protected void Repeater15_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {

            Staff loggedInStaff = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));

            DataRowView dr = (DataRowView)e.Item.DataItem;
            if (dr == null || dr.Row == null)
                return;
            DataRow row = dr.Row;
            Invoice invoice = InvoiceDB.LoadAll(row);


            // get controls
            Repeater           lstReceipts                      = (Repeater)e.Item.FindControl("lstReceipts");
            HtmlGenericControl div_receipts_list                = (HtmlGenericControl)e.Item.FindControl("div_receipts_list");
            HtmlGenericControl span_receipts_trailing_space_row = (HtmlGenericControl)e.Item.FindControl("span_receipts_trailing_space_row");
            Label              lnkAddReceipt                    = (Label)e.Item.FindControl("lnkAddReceipt");
            LinkButton         showHideReceiptsList             = (LinkButton)e.Item.FindControl("showHideReceiptsList");


            // get receipts
            DataTable tblReciepts = ReceiptDB.GetDataTableByInvoice(invoice.InvoiceID);
            lstReceipts.Visible   = tblReciepts.Rows.Count >  0;
            span_receipts_trailing_space_row.Visible = tblReciepts.Rows.Count > 0;
            if (tblReciepts.Rows.Count > 0)
            {
                tblReciepts.Columns.Add("receipt_url",         typeof(string));
                tblReciepts.Columns.Add("show_status",         typeof(string));
                tblReciepts.Columns.Add("status",              typeof(string));
                tblReciepts.Columns.Add("show_reconcile_link", typeof(string));
                tblReciepts.Columns.Add("reconcile_link",      typeof(string));
                tblReciepts.Columns.Add("show_reverse_link",   typeof(string));
                for (int i = 0; i < tblReciepts.Rows.Count; i++)
                {
                    Receipt receipt = ReceiptDB.LoadAll(tblReciepts.Rows[i]);

                    tblReciepts.Rows[i]["receipt_url"] = receipt.GetViewPopupLinkV2();

                    bool isReconciledOrReversed = receipt.IsReconciled || receipt.IsReversed;
                    tblReciepts.Rows[i]["status"]              =  receipt.IsReconciled    ? "Reconciled" : "Reversed";
                    tblReciepts.Rows[i]["show_status"]         =  isReconciledOrReversed  ? "1" : "0";
                    tblReciepts.Rows[i]["reconcile_link"]      =  receipt.GetReconcilePopupLinkV2("window.location.href = window.location.href;");
                    tblReciepts.Rows[i]["show_reconcile_link"] = !isReconciledOrReversed && (loggedInStaff.IsStakeholder || loggedInStaff.IsMasterAdmin || loggedInStaff.IsAdmin || loggedInStaff.IsPrincipal) ? "1" : "0";
                    tblReciepts.Rows[i]["show_reverse_link"]   = !isReconciledOrReversed  ? "1" : "0";
                }

                lstReceipts.DataSource = tblReciepts;
                lstReceipts.DataBind();
            }

            if (!invoice.IsPaID) // can add items
                lnkAddReceipt.Text = Receipt.GetAddReceiptPopupLinkV2(invoice.InvoiceID, "Add Payment", "window.location.href = window.location.href;");
            else
                lnkAddReceipt.Text = tblReciepts.Rows.Count > 0 ? string.Empty : "No Payments";
            //span_add_receipts_row.Style["text-align"] = (tblReciepts.Rows.Count > 0) ? "center" : null;  // if have table, center add link, else left align
            lnkAddReceipt.Visible = lnkAddReceipt.Text.Length > 0;
            showHideReceiptsList.OnClientClick = "javascript:show_hide_byname('div_receipts_list_" + invoice.InvoiceID + "'); return false;";
            showHideReceiptsList.Visible = tblReciepts.Rows.Count > 0;
            div_receipts_list.Attributes["name"] = "div_receipts_list_" + invoice.InvoiceID;

        }
    }
    protected void lstReceipts_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView dr = (DataRowView)e.Item.DataItem;
            if (dr == null || dr.Row == null)
                return;
            DataRow row = dr.Row;


            Label       lnkViewReceipt     = (Label)e.Item.FindControl("lnkViewReceipt");
            Label       lblPaidBy          = (Label)e.Item.FindControl("lblPaidBy");
            Label       lblReceiptDate     = (Label)e.Item.FindControl("lblReceiptDate");
            Label       lblPaymentType     = (Label)e.Item.FindControl("lblPaymentType");
            Label       lblReceiptTotal    = (Label)e.Item.FindControl("lblReceiptTotal");
            Label       lblReceiptAmountReconciled = (Label)e.Item.FindControl("lblReceiptAmountReconciled");
            Label       lblStatus          = (Label)e.Item.FindControl("lblStatus");
            Label       lnkReconcile       = (Label)e.Item.FindControl("lnkReconcile");
            LinkButton  lnkReverse         = (LinkButton)e.Item.FindControl("lnkReverse");
            HiddenField lblHiddenReceiptID = (HiddenField)e.Item.FindControl("lblHiddenReceiptID");


            lnkViewReceipt.Text = row["receipt_url"].ToString();
            lblPaidBy.Text = "<a href=javascript:void(0)'  style='text-decoration:none;' onclick='return false;' title='Entered By: " + row["person_firstname"] + " " + row["person_surname"] + "'> * </a>";
            lblReceiptDate.Text = Convert.ToDateTime(row["receipt_date_added"]).ToString("dd-MM-yyyy");
            lblPaymentType.Text = row["descr"].ToString();
            lblReceiptTotal.Text = row["total"].ToString();
            lblReceiptAmountReconciled.Text = row["amount_reconciled"].ToString();

            lblStatus.Text = row["status"].ToString();
            lblStatus.Visible = row["show_status"].ToString() == "1";

            lnkReconcile.Text = row["reconcile_link"].ToString() + "<br />";
            lnkReconcile.Visible = row["show_reconcile_link"].ToString() == "1";

            lnkReverse.CommandArgument = row["receipt_id"].ToString();
            lnkReverse.Visible = row["show_reverse_link"].ToString() == "1";
            lnkReverse.OnClientClick = "javascript:if (!confirm('Are you sure you want to reverse this record?')) return false;";
            lblHiddenReceiptID.Value = row["receipt_id"].ToString();


            // if tyro healthclaim invoice .. then open new tab to remove claim throught tyro
            Receipt receipt = ReceiptDB.GetByID(Convert.ToInt32(row["receipt_id"]));
            if (receipt.ReceiptPaymentType.ID == 365 && !receipt.IsReversed)  // Tyro HC Claim
            {
                TyroHealthClaim[] claims = TyroHealthClaimDB.GetByInvoice(receipt.Invoice.InvoiceID);
                //if (true)
                if (claims.Length != 1)
                {
                    Emailer.SimpleAlertEmail(
                        "Receipt " + receipt.ReceiptPaymentType.ID + " is of type 'Tyro HC Claim' but " + claims.Length + " approved uncancelled rows found in TyroHealthClaimDB<br />DB:" + Session["DB"],
                        "Tyro Problem - Multiple Approved Uncancelled TyroPayment Rows Found For Single Receipt",
                        true);

                    lnkReverse.OnClientClick = "alert('Receipt " + receipt.ReceiptPaymentType.ID + " is of type 'Tyro HC Claim' but " + claims.Length + " approved uncancelled rows found in TyroHealthClaimDB.\r\nPlease contact the system administrator to check into this');return false;";
                }
                else
                {
                    lnkReverse.OnClientClick = "open_new_tab('TyroHealthPointClaimV2.aspx?invoice=" + receipt.Invoice.InvoiceID + "&reftag=" + claims[0].OutHealthpointRefTag + "');return false;";
                }
            }
        }
    }

    protected void Repeater16_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView dr = (DataRowView)e.Item.DataItem;
            if (dr == null || dr.Row == null)
                return;
            DataRow row = dr.Row;
            Invoice invoice = InvoiceDB.LoadAll(row);


            // get controls
            Repeater           lstCreditNotes         = (Repeater)e.Item.FindControl("lstCreditNotes");
            HtmlGenericControl div_credit_notes_list  = (HtmlGenericControl)e.Item.FindControl("div_credit_notes_list");
            HtmlGenericControl span_credit_notes_trailing_space_row = (HtmlGenericControl)e.Item.FindControl("span_credit_notes_trailing_space_row");
            Label              lnkAddCreditNote       = (Label)e.Item.FindControl("lnkAddCreditNote");
            LinkButton         showHideCreditNoteList = (LinkButton)e.Item.FindControl("showHideCreditNoteList");


            // get credit notes
            DataTable tblCreditNotes = CreditNoteDB.GetDataTableByInvoice(invoice.InvoiceID);
            lstCreditNotes.Visible = tblCreditNotes.Rows.Count > 0;
            span_credit_notes_trailing_space_row.Visible = tblCreditNotes.Rows.Count > 0;
            if (tblCreditNotes.Rows.Count > 0)
            {
                tblCreditNotes.Columns.Add("credit_note_url", typeof(string));
                tblCreditNotes.Columns.Add("show_status", typeof(string));
                tblCreditNotes.Columns.Add("status", typeof(string));
                tblCreditNotes.Columns.Add("show_reverse_link", typeof(string));
                tblCreditNotes.Columns.Add("show_status_column", typeof(string));
                for (int i = 0; i < tblCreditNotes.Rows.Count; i++)
                {
                    CreditNote creditNote = CreditNoteDB.Load(tblCreditNotes.Rows[i]);
                    tblCreditNotes.Rows[i]["credit_note_url"]    = creditNote.GetViewPopupLinkV2();

                    tblCreditNotes.Rows[i]["show_status"]        =  creditNote.IsReversed ? "1" : "0";
                    tblCreditNotes.Rows[i]["show_reverse_link"]  = !creditNote.IsReversed && !invoice.IsReversed ? "1" : "0";
                    tblCreditNotes.Rows[i]["show_status_column"] = !invoice.IsReversed ? "1" : "0";
                }

                lstCreditNotes.DataSource = tblCreditNotes;
                lstCreditNotes.DataBind();
            }

            if (!invoice.IsPaID) // can add items
                lnkAddCreditNote.Text = CreditNote.GetAddCreditNotePopupLinkV2(invoice.InvoiceID, "window.location.href = window.location.href;");
            else
                lnkAddCreditNote.Text = tblCreditNotes.Rows.Count > 0 ? string.Empty : "No Adjustment Notes";
            //span_add_credit_notes_row.Style["text-align"] = (tblCreditNotes.Rows.Count > 0) ? "center" : null;  // if have table, center add link, else left align
            lnkAddCreditNote.Visible = lnkAddCreditNote.Text.Length > 0;
            showHideCreditNoteList.OnClientClick = "javascript:show_hide_byname('div_credit_notes_list_" + invoice.InvoiceID + "'); return false;";
            showHideCreditNoteList.Visible = tblCreditNotes.Rows.Count > 0;
            div_credit_notes_list.Attributes["name"] = "div_credit_notes_list_" + invoice.InvoiceID;
        }
    }
    protected void lstCreditNotes_ItemCommand(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView dr = (DataRowView)e.Item.DataItem;
            if (dr == null || dr.Row == null)
                return;
            DataRow row = dr.Row;


            Label         lnkViewCreditNote     = (Label)e.Item.FindControl("lnkViewCreditNote");
            Label         lblCreditNoteDate     = (Label)e.Item.FindControl("lblCreditNoteDate");
            Label         lblCreditNoteTotal    = (Label)e.Item.FindControl("lblCreditNoteTotal");
            HtmlTableCell tdStatusColumn        = (HtmlTableCell)e.Item.FindControl("tdStatusColumn");
            Label         lblStatus             = (Label)e.Item.FindControl("lblStatus");
            LinkButton    lnkReverse            = (LinkButton)e.Item.FindControl("lnkReverse");
            HiddenField   lblHiddenCreditNoteID = (HiddenField)e.Item.FindControl("lblHiddenCreditNoteID");


            lnkViewCreditNote.Text = row["credit_note_url"].ToString();
            lblCreditNoteDate.Text = Convert.ToDateTime(row["credit_note_date_added"]).ToString("dd-MM-yyyy");
            lblCreditNoteTotal.Text = row["total"].ToString();

            tdStatusColumn.Visible = row["show_status_column"].ToString() == "1";
            lblStatus.Visible = row["show_status"].ToString() == "1";

            lnkReverse.CommandArgument = row["creditnote_id"].ToString();
            lnkReverse.Visible = row["show_reverse_link"].ToString() == "1";
            lnkReverse.OnClientClick = "javascript:if (!confirm('Are you sure you want to reverse this record?')) return false;";
            lblHiddenCreditNoteID.Value = row["creditnote_id"].ToString();
        }
    }

    protected void Repeater17_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView dr = (DataRowView)e.Item.DataItem;
            if (dr == null || dr.Row == null)
                return;
            DataRow row = dr.Row;
            Invoice invoice = InvoiceDB.LoadAll(row);


            // get controls
            Repeater           lstRefunds                      = (Repeater)e.Item.FindControl("lstRefunds");
            HtmlGenericControl div_refunds_list                = (HtmlGenericControl)e.Item.FindControl("div_refunds_list");
            HtmlGenericControl span_refunds_trailing_space_row = (HtmlGenericControl)e.Item.FindControl("span_refunds_trailing_space_row");
            Label              lnkAddRefund                    = (Label)e.Item.FindControl("lnkAddRefund");
            LinkButton         showHideRefundsList             = (LinkButton)e.Item.FindControl("showHideRefundsList");


            // get refunds
            DataTable tblRefunds = RefundDB.GetDataTableByInvoice(invoice.InvoiceID);
            lstRefunds.Visible = tblRefunds.Rows.Count > 0;
            span_refunds_trailing_space_row.Visible = tblRefunds.Rows.Count > 0;
            if (tblRefunds.Rows.Count > 0)
            {
                tblRefunds.Columns.Add("refund_url", typeof(string));
                for (int i = 0; i < tblRefunds.Rows.Count; i++)
                {
                    Refund refund = RefundDB.LoadAll(tblRefunds.Rows[i]);
                    tblRefunds.Rows[i]["refund_url"] = refund.GetViewPopupLinkV2();
                }

                lstRefunds.DataSource = tblRefunds;
                lstRefunds.DataBind();
            }

            lnkAddRefund.Visible = tblRefunds.Rows.Count == 0;
            //if (!invoice.IsPaID) // can add items
            if (tblRefunds.Rows.Count == 0) // can add items
                lnkAddRefund.Text = Refund.GetAddPopupLinkV2(invoice.InvoiceID, "window.location.href = window.location.href;");
            else
                lnkAddRefund.Text = tblRefunds.Rows.Count > 0 ? string.Empty : "No Refunds";
            //span_add_refunds_row.Style["text-align"] = (tblRefunds.Rows.Count > 0) ? "center" : null;  // if have table, center add link, else left align
            lnkAddRefund.Visible = lnkAddRefund.Text.Length > 0 && invoice.ReceiptsTotal > 0;
            showHideRefundsList.OnClientClick = "javascript:show_hide_byname('div_refunds_list_" + invoice.InvoiceID + "'); return false;";
            showHideRefundsList.Visible = tblRefunds.Rows.Count > 0;
            div_refunds_list.Attributes["name"] = "div_refunds_list_" + invoice.InvoiceID;
        }
    }
    protected void lstRefunds_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView dr = (DataRowView)e.Item.DataItem;
            if (dr == null || dr.Row == null)
                return;
            DataRow row = dr.Row;


            Label lnkViewRefund   = (Label)e.Item.FindControl("lnkViewRefund");
            Label lblRefundDate   = (Label)e.Item.FindControl("lblRefundDate");
            Label lblRefundTotal  = (Label)e.Item.FindControl("lblRefundTotal");
            Label lblRefundReason = (Label)e.Item.FindControl("lblRefundReason");


            lnkViewRefund.Text = row["refund_url"].ToString();
            lblRefundDate.Text = Convert.ToDateTime(row["refund_date_added"]).ToString("dd-MM-yyyy");
            lblRefundTotal.Text = row["total"].ToString();
            lblRefundReason.Text = row["descr"].ToString();
        }
    }

    protected void Repeater19_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView dr = (DataRowView)e.Item.DataItem;
            if (dr == null || dr.Row == null)
                return;
            DataRow row = dr.Row;
            Invoice invoice = InvoiceDB.LoadAll(row);


            // get controls
            Repeater           lstVouchers                      = (Repeater)e.Item.FindControl("lstVouchers");
            HtmlGenericControl div_vouchers_list                = (HtmlGenericControl)e.Item.FindControl("div_vouchers_list");
            HtmlGenericControl span_vouchers_trailing_space_row = (HtmlGenericControl)e.Item.FindControl("span_vouchers_trailing_space_row");
            Label              lnkAddVoucher                    = (Label)e.Item.FindControl("lnkAddVoucher");
            LinkButton         showHideVouchersList             = (LinkButton)e.Item.FindControl("showHideVouchersList");


            // get refunds
            DataTable tblCredit = CreditDB.GetDataTable_ByInvoiceID(invoice.InvoiceID);
            lstVouchers.Visible = tblCredit.Rows.Count > 0;
            span_vouchers_trailing_space_row.Visible = tblCredit.Rows.Count > 0;
            if (tblCredit.Rows.Count > 0)
            {
                tblCredit.Columns.Add("voucher_url",         typeof(string));
                tblCredit.Columns.Add("show_status",         typeof(string));
                tblCredit.Columns.Add("status",              typeof(string));
                tblCredit.Columns.Add("show_reconcile_link", typeof(string));
                tblCredit.Columns.Add("reconcile_link",      typeof(string));
                tblCredit.Columns.Add("show_reverse_link",   typeof(string));
                for (int i = 0; i < tblCredit.Rows.Count; i++)
                {
                    Credit voucher = CreditDB.LoadAll(tblCredit.Rows[i]);

                    tblCredit.Rows[i]["voucher_url"] = voucher.GetViewVoucherUsePopupLinkV2();

                    bool isReversed =  voucher.IsDeleted;
                    tblCredit.Rows[i]["status"]              =  "Reversed";
                    tblCredit.Rows[i]["show_status"]         =  isReversed  ? "1" : "0";
                    tblCredit.Rows[i]["reconcile_link"]      =  "";
                    tblCredit.Rows[i]["show_reverse_link"]   = voucher.IsDeleted ? "0" : "1";
                }

                lstVouchers.DataSource = tblCredit;
                lstVouchers.DataBind();
            }

            if (!invoice.IsPaID) // can add items
                lnkAddVoucher.Text = Receipt.GetAddReceiptPopupLinkV2(invoice.InvoiceID, "Add Voucher Use", "window.location.href = window.location.href;");
            else
                lnkAddVoucher.Text = tblCredit.Rows.Count > 0 ? string.Empty : "No Vouchers";
            //span_add_vouchers_row.Style["text-align"] = (tblReciepts.Rows.Count > 0) ? "center" : null;  // if have table, center add link, else left align
            lnkAddVoucher.Visible = lnkAddVoucher.Text.Length > 0;
            showHideVouchersList.OnClientClick = "javascript:show_hide_byname('div_vouchers_list_" + invoice.InvoiceID + "'); return false;";
            showHideVouchersList.Visible = tblCredit.Rows.Count > 0;
            div_vouchers_list.Attributes["name"] = "div_vouchers_list_" + invoice.InvoiceID;
        }
    }
    protected void lstVouchers_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView dr = (DataRowView)e.Item.DataItem;
            if (dr == null || dr.Row == null)
                return;
            DataRow row = dr.Row;


            Label       lnkViewVoucher     = (Label)e.Item.FindControl("lnkViewVoucher");
            Label       lblPaidBy          = (Label)e.Item.FindControl("lblPaidBy");
            Label       lblVoucherDate     = (Label)e.Item.FindControl("lblVoucherDate");
            Label       lblPaymentType     = (Label)e.Item.FindControl("lblPaymentType");
            Label       lblVoucherTotal    = (Label)e.Item.FindControl("lblVoucherTotal");
            Label       lblVoucherAmountReconciled = (Label)e.Item.FindControl("lblVoucherAmountReconciled");
            Label       lblStatus          = (Label)e.Item.FindControl("lblStatus");
            Label       lnkReconcile       = (Label)e.Item.FindControl("lnkReconcile");
            LinkButton  lnkReverse         = (LinkButton)e.Item.FindControl("lnkReverse");
            HiddenField lblHiddenVoucherID = (HiddenField)e.Item.FindControl("lblHiddenVoucherID");


            lnkViewVoucher.Text = row["voucher_url"].ToString();
            lblPaidBy.Text = "<a href=javascript:void(0)'  style='text-decoration:none;' onclick='return false;' title='Entered By: " + row["person_added_by_firstname"] + " " + row["person_added_by_surname"] + "'> * </a>";
            lblVoucherDate.Text = Convert.ToDateTime(row["credit_date_added"]).ToString("dd-MM-yyyy");
            lblVoucherTotal.Text = (-1 * Convert.ToDecimal(row["credit_amount"])).ToString(); ;

            lblStatus.Text = row["status"].ToString();
            lblStatus.Visible = row["show_status"].ToString() == "1";

            lnkReconcile.Text = "";
            lnkReconcile.Visible = false;

            lnkReverse.CommandArgument = row["credit_credit_id"].ToString();
            lnkReverse.Visible = row["show_reverse_link"].ToString() == "1";
            lnkReverse.OnClientClick = "javascript:if (!confirm('Are you sure you want to reverse this record?')) return false;";
            lblHiddenVoucherID.Value = row["credit_credit_id"].ToString();

        }
    }

    protected void ReverseReceipt_Command(object sender, CommandEventArgs e)
    {
        try
        {

            // for some reason, it doesn't keep the command argument when set in 
            // the code behind in a nested repeater, so set it in a hidden control and its fine

            //int receiptID = Convert.ToInt32(e.CommandArgument);

            int receiptID = -1;
            foreach (Control c in ((Control)sender).Parent.Controls)
                if (c.ID == "lblHiddenReceiptID")
                    receiptID = Convert.ToInt32(((HiddenField)c).Value);


            Receipt receipt = ReceiptDB.GetByID(receiptID);
            if (receipt == null)
                throw new CustomMessageException("Invalid receipt - does not exist");
            if (receipt.IsReversed)
                throw new CustomMessageException("Receipt already reversed");
            if (receipt.IsReconciled)
                throw new CustomMessageException("Can not reverse a receipt that has been reconciled");
            //if (receipt.ReceiptPaymentType.ID == 365)
            //    throw new CustomMessageException("Can not reverse a 'Tyro HC Claim' receipt");

            ReceiptDB.Reverse(receipt.ReceiptID, Convert.ToInt32(Session["StaffID"]));

            FillInvoicesList();
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
    protected void ReverseCreditNote_Command(object sender, CommandEventArgs e)
    {
        try
        {

            // for some reason, it doesn't keep the command argument when set in 
            // the code behind in a nested repeater, so set it in a hidden control and its fine

            //int creditNoteID = Convert.ToInt32(e.CommandArgument);

            int creditNoteID = -1;
            foreach (Control c in ((Control)sender).Parent.Controls)
                if (c.ID == "lblHiddenCreditNoteID")
                    creditNoteID = Convert.ToInt32(((HiddenField)c).Value);


            CreditNote creditNote = CreditNoteDB.GetByID(creditNoteID);
            if (creditNote == null)
                throw new CustomMessageException("Adjustment note - does not exist");
            if (creditNote.IsReversed)
                throw new CustomMessageException("Adjustment note already reversed");

            CreditNoteDB.Reverse(creditNote.CreditNoteID, Convert.ToInt32(Session["StaffID"]));

            FillInvoicesList();
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
    protected void ReverseVoucher_Command(object sender, CommandEventArgs e)
    {
        try
        {
            // for some reason, it doesn't keep the command argument when set in 
            // the code behind in a nested repeater, so set it in a hidden control and its fine

            //int creditID = Convert.ToInt32(e.CommandArgument);

            int creditID = -1;
            foreach (Control c in ((Control)sender).Parent.Controls)
                if (c.ID == "lblHiddenVoucherID")
                    creditID = Convert.ToInt32(((HiddenField)c).Value);


            Credit credit = CreditDB.GetByID(creditID);
            if (credit == null)
                throw new CustomMessageException("Invalid voucher - does not exist");
            if (credit.IsDeleted)
                throw new CustomMessageException("Voucher already reversed");

            CreditDB.SetAsDeleted(credit.CreditID, Convert.ToInt32(Session["StaffID"]));

            FillInvoicesList();
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

    protected void lnkPrint_Command(object sender, CommandEventArgs e)
    {
        int invoiceID = Convert.ToInt32(e.CommandArgument);
        Invoice invoice = InvoiceDB.GetByID(invoiceID);
        Letter.GenerateInvoicesToPrint(new int[] { invoiceID }, Response, invoice.Site.SiteType.ID == 1, invoice.Booking != null);
    }
    protected void lnkEmail_Command(object sender, CommandEventArgs e)
    {
        int invoiceID = Convert.ToInt32(e.CommandArgument);
        Invoice invoice = InvoiceDB.GetByID(invoiceID);

        try
        {
            Letter.GenerateInvoiceToEmail(invoiceID, invoice.Site.SiteType.ID == 1);
            InvoiceDB.UpdateLastDateEmailed(invoiceID, DateTime.Now);
            FillInvoicesList();
            SetErrorMessage("Invoice Sent");
        }
        catch (CustomMessageException ex)
        {
            SetErrorMessage(ex.Message);
        }
    }

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
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