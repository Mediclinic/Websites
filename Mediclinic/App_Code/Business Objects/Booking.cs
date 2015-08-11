using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections;
using System.IO;


public class Booking
{

    public Booking(int booking_id, int entity_id, DateTime date_start, DateTime date_end, int organisation_id, int provider, int patient_id,
                int offering_id, int booking_type_id, int booking_status_id, int booking_unavailability_reason_id, 
                int added_by, DateTime date_created, int booking_confirmed_by_type_id,  int confirmed_by, DateTime date_confirmed, 
                int deleted_by, DateTime date_deleted, int cancelled_by, DateTime date_cancelled, 
                bool is_patient_missed_appt, bool is_provider_missed_appt, bool is_emergency, 
                bool need_to_generate_first_letter, bool need_to_generate_last_letter, bool has_generated_system_letters,
                DateTime arrival_time, string sterilisation_code, int informed_consent_added_by, DateTime informed_consent_date,
                bool is_recurring, DayOfWeek recurring_day_of_week, TimeSpan recurring_start_time, TimeSpan recurring_end_time,
                int note_count, int inv_count)
    {
        this.booking_id                    = booking_id;
        this.entity_id                     = entity_id;
        this.date_start                    = date_start;
        this.date_end                      = date_end;
        this.organisation                  = organisation_id   ==  0 ? null : new Organisation(organisation_id);
        this.provider                      = provider          == -1 ? null : new Staff(provider);
        this.patient                       = patient_id        == -1 ? null : new Patient(patient_id);
        this.offering                      = offering_id       == -1 ? null : new Offering(offering_id);
        this.booking_type_id               = booking_type_id;
        this.booking_status                = booking_status_id == -2 ? null : new IDandDescr(booking_status_id);
        this.booking_unavailability_reason = booking_unavailability_reason_id == -1 ? null : new IDandDescr(booking_unavailability_reason_id);
        this.added_by                      = added_by          == -1 ? null :new Staff(added_by);
        this.date_created                  = date_created;
        this.booking_confirmed_by_type     = booking_confirmed_by_type_id == -1 ? null : new IDandDescr(booking_confirmed_by_type_id);
        this.confirmed_by                  = confirmed_by      == -1 ? null : new Staff(confirmed_by);
        this.date_confirmed                = date_confirmed;
        this.deleted_by                    = deleted_by        == -1 ? null : new Staff(deleted_by);
        this.date_deleted                  = date_deleted;
        this.cancelled_by                  = cancelled_by      == -1 ? null : new Staff(cancelled_by);
        this.date_cancelled                = date_cancelled;
        this.is_patient_missed_appt        = is_patient_missed_appt;
        this.is_provider_missed_appt       = is_provider_missed_appt;
        this.is_emergency                  = is_emergency;
        this.need_to_generate_first_letter = need_to_generate_first_letter;
        this.need_to_generate_last_letter  = need_to_generate_last_letter;
        this.has_generated_system_letters  = has_generated_system_letters;
        this.arrival_time                  = arrival_time;
        this.sterilisation_code            = sterilisation_code;
        this.informed_consent_added_by     = informed_consent_added_by == -1 ? null : new Staff(informed_consent_added_by);
        this.informed_consent_date         = informed_consent_date;
        this.is_recurring                  = is_recurring;
        this.recurring_day_of_week         = recurring_day_of_week;
        this.recurring_start_time          = recurring_start_time;
        this.recurring_end_time            = recurring_end_time;

        this.note_count                    = note_count;
        this.inv_count                     = inv_count;
    }
    public Booking(int booking_id)
    {
        this.booking_id = booking_id;
    }

    private int booking_id;
    public int BookingID
    {
        get { return this.booking_id; }
        set { this.booking_id = value; }
    }
    private int entity_id;
    public int EntityID
    {
        get { return this.entity_id; }
        set { this.entity_id = value; }
    }
    private DateTime date_start;
    public DateTime DateStart
    {
        get { return this.date_start; }
        set { this.date_start = value; }
    }
    private DateTime date_end;
    public DateTime DateEnd
    {
        get { return this.date_end; }
        set { this.date_end = value; }
    }

    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private Staff provider;
    public Staff Provider
    {
        get { return this.provider; }
        set { this.provider = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private Offering offering;
    public Offering Offering
    {
        get { return this.offering; }
        set { this.offering = value; }
    }
    private int booking_type_id;
    public int BookingTypeID
    {
        get { return this.booking_type_id; }
        set { this.booking_type_id = value; }
    }
    private IDandDescr booking_status;
    public IDandDescr BookingStatus
    {
        get { return this.booking_status; }
        set { this.booking_status = value; }
    }
    private IDandDescr booking_unavailability_reason;
    public IDandDescr BookingUnavailabilityReason
    {
        get { return this.booking_unavailability_reason; }
        set { this.booking_unavailability_reason = value; }
    }

    private Staff added_by;
    public Staff AddedBy
    {
        get { return this.added_by; }
        set { this.added_by = value; }
    }
    private DateTime date_created;
    public DateTime DateCreated
    {
        get { return this.date_created; }
        set { this.date_created = value; }
    }
    private IDandDescr booking_confirmed_by_type;
    public IDandDescr BookingConfirmedByType
    {
        get { return this.booking_confirmed_by_type; }
        set { this.booking_confirmed_by_type = value; }
    }
    private Staff confirmed_by;
    public Staff ConfirmedBy
    {
        get { return this.confirmed_by; }
        set { this.confirmed_by = value; }
    }
    private DateTime date_confirmed;
    public DateTime DateConfirmed
    {
        get { return this.date_confirmed; }
        set { this.date_confirmed = value; }
    }
    private Staff deleted_by;
    public Staff DeletedBy
    {
        get { return this.deleted_by; }
        set { this.deleted_by = value; }
    }
    private DateTime date_deleted;
    public DateTime DateDeleted
    {
        get { return this.date_deleted; }
        set { this.date_deleted = value; }
    }
    private Staff cancelled_by;
    public Staff CancelledBy
    {
        get { return this.cancelled_by; }
        set { this.cancelled_by = value; }
    }
    private DateTime date_cancelled;
    public DateTime DateCancelled
    {
        get { return this.date_cancelled; }
        set { this.date_cancelled = value; }
    }
    private bool is_patient_missed_appt;
    public bool IsPatientMissedAppt
    {
        get { return this.is_patient_missed_appt; }
        set { this.is_patient_missed_appt = value; }
    }
    private bool is_provider_missed_appt;
    public bool IsProviderMissedAppt
    {
        get { return this.is_provider_missed_appt; }
        set { this.is_provider_missed_appt = value; }
    }
    private bool is_emergency;
    public bool IsEmergency
    {
        get { return this.is_emergency; }
        set { this.is_emergency = value; }
    }
    private bool need_to_generate_first_letter;
    public bool NeedToGenerateFirstLetter
    {
        get { return this.need_to_generate_first_letter; }
        set { this.need_to_generate_first_letter = value; }
    }
    private bool need_to_generate_last_letter;
    public bool NeedToGenerateLastLetter
    {
        get { return this.need_to_generate_last_letter; }
        set { this.need_to_generate_last_letter = value; }
    }
    private bool has_generated_system_letters;
    public bool HasGeneratedSystemLetters
    {
        get { return this.has_generated_system_letters; }
        set { this.has_generated_system_letters = value; }
    }
    private DateTime arrival_time;
    public DateTime ArrivalTime
    {
        get { return this.arrival_time; }
        set { this.arrival_time = value; }
    }
    private string sterilisation_code;
    public string SterilisationCode
    {
        get { return this.sterilisation_code; }
        set { this.sterilisation_code = value; }
    }
    private Staff informed_consent_added_by;
    public Staff InformedConsentAddedBy
    {
        get { return this.informed_consent_added_by; }
        set { this.informed_consent_added_by = value; }
    }
    private DateTime informed_consent_date;
    public DateTime InformedConsentDate
    {
        get { return this.informed_consent_date; }
        set { this.informed_consent_date = value; }
    }
    private bool is_recurring;
    public bool IsRecurring
    {
        get { return this.is_recurring; }
        set { this.is_recurring = value; }
    }
    private DayOfWeek recurring_day_of_week;
    public DayOfWeek RecurringDayOfWeek
    {
        get { return this.recurring_day_of_week; }
        set { this.recurring_day_of_week = value; }
    }
    private TimeSpan recurring_start_time;
    public TimeSpan RecurringStartTime
    {
        get { return this.recurring_start_time; }
        set { this.recurring_start_time = value; }
    }
    private TimeSpan recurring_end_time;
    public TimeSpan RecurringEndTime
    {
        get { return this.recurring_end_time; }
        set { this.recurring_end_time = value; }
    }

    private int note_count;
    public int NoteCount
    {
        get { return this.note_count; }
        set { this.note_count = value; }
    }
    private int inv_count;
    public int InvoiceCount
    {
        get { return this.inv_count; }
        set { this.inv_count = value; }
    }

    public override string ToString()
    {
        return booking_id.ToString() + " " + entity_id.ToString() + " " + date_start.ToString() + " " + date_end.ToString() + " " + organisation.OrganisationID.ToString() + " " + provider.StaffID.ToString() + " " +
                patient.PatientID.ToString() + " " + offering.OfferingID.ToString() + " " + booking_type_id.ToString() + " " + booking_status.ID.ToString() + " " + added_by.StaffID.ToString() + " " +
                date_created.ToString() + " " + (confirmed_by == null ? "" : confirmed_by.StaffID.ToString()) + " " + date_confirmed.ToString() + " " + (deleted_by == null ? "" : deleted_by.StaffID.ToString()) + " " + date_deleted.ToString() + " " +
                is_patient_missed_appt.ToString() + " " + is_provider_missed_appt.ToString() + " " + is_emergency.ToString();
    }


    public void Reverse(bool isAdmin, int reversedBy)
    {
        Invoice[] invoices = InvoiceDB.GetByBookingID(this.BookingID);

        try
        {
            string errorString = string.Empty;
            if (!this.CanReverse(isAdmin, out errorString, invoices))
                throw new CustomMessageException("Error:" + errorString);

            foreach (Invoice invoice in invoices)
                InvoiceDB.Reverse(invoice.InvoiceID, reversedBy);

            BookingDB.UpdateSetBookingStatusID(this.BookingID, 0);
            this.BookingStatus.ID = 0;  // do this at the end so any earlier exception allows the catch block to reset this correctly
            BookingDB.UpdateUnsetDeletedByCancelledBy(this.BookingID);


            // send alert email

            string mailText = @"This is an administrative email to notify you that a booking has been reversed.

<u>Logged-in user performing the reverse</u>
" + StaffDB.GetByID(reversedBy).Person.FullnameWithoutMiddlename + @"

<u>Booking details</u>
<table border=""0"" cellpadding=""2"" cellspacing=""2""><tr><td>Booking ID:</td><td>" + this.BookingID + @"</td></tr><tr><td>Invoice ID" + (invoices.Length > 1 ? "s" : "") + @" Reversed:</td><td>" + string.Join(", ", invoices.Select(r => r.InvoiceID).ToArray()) + @"</td></tr><tr><td>Booking Date:</td><td>" + this.DateStart.ToString("d MMM, yyyy") + " " + this.DateStart.ToString("h:mm") + (this.DateStart.Hour < 12 ? "am" : "pm") + @"</td></tr><tr><td>Organisation:</td><td>" + this.Organisation.Name + @"</td></tr><tr><td>Provider:</td><td>" + this.Provider.Person.FullnameWithoutMiddlename + @"</td></tr><tr><td>Patient:</td><td>" + (this.Patient == null ? "" : this.Patient.Person.FullnameWithoutMiddlename + " [ID:" + this.Patient.PatientID + "]") + @"</td></tr><tr><td>Status:</td><td>" + this.BookingStatus.Descr + @"</td></tr></table>

Regards,
Mediclinic
";
            bool EnableDeletedBookingsAlerts = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableDeletedBookingsAlerts").Value) == 1;

            if (EnableDeletedBookingsAlerts && !Utilities.IsDev())
                Emailer.AsyncSimpleEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["AdminAlertEmail_To"].Value,
                    "Notification that a booking has been reversed",
                    mailText.Replace(Environment.NewLine, "<br />"),
                    true,
                    null);
        }
        catch(Exception)
        {
            foreach (Invoice invoice in invoices)
                InvoiceDB.UnReverse(invoice.InvoiceID);
            BookingDB.UpdateSetBookingStatusID(this.BookingID, this.BookingStatus.ID);

            throw;
        }
    }
    public bool CanReverse(bool isAdmin, out string errorString)
    {
        Invoice[] invoices = InvoiceDB.GetByBookingID(this.BookingID);

        errorString = string.Empty;
        return this.CanReverse(isAdmin, out errorString, invoices);
    }
    protected bool CanReverse(bool isAdmin, out string errorString, Invoice[] invoices)
    {
        errorString = string.Empty;

        try
        {
            if (this.BookingStatus.ID <= 0)
            {
                errorString = "Can not reverse deleted or un-completed bookings";
                return false;
            }


            if (! isAdmin)
            {
                int maxDaysToReverseForNonAdmin = 7;
                DateTime cutOffDate = DateTime.Now.Date.AddDays(-1 * maxDaysToReverseForNonAdmin);

                /*
                DateTime completionDate = DateTime.MinValue;
                foreach (Invoice invoice in invoices)
                {
                    if (completionDate == DateTime.MinValue)
                        completionDate = invoice.InvoiceDateAdded;
                    else
                        if (invoice.InvoiceDateAdded < completionDate)
                            completionDate = invoice.InvoiceDateAdded;
                }
                if (completionDate != DateTime.MinValue && completionDate < cutOffDate)
                {
                    errorString = "Unable to reverse booking that was completed over " + maxDaysToReverseForNonAdmin + " days ago";
                    return false;
                }
                */

                if (this.DateStart.Date < cutOffDate)
                {
                    errorString = "Unable to reverse booking whose treatment date is over " + maxDaysToReverseForNonAdmin + " days ago";
                    return false;
                }
            }


            foreach (Invoice invoice in invoices)
            {
                string invoiceReversalErrorString = string.Empty;
                if (!invoice.CanReverse(out invoiceReversalErrorString))
                {
                    errorString = "Unable to revesre booking " + this.BookingID + " because " + invoiceReversalErrorString + "." + Environment.NewLine +
                                  "View invoices and ensure that all receipts and vouchers used for all invoices of this booking have been reversed" + Environment.NewLine +
                                  "Note that you can not reverse a booking that has reconciled invoices.";
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            errorString = Utilities.IsDev() ? ex.ToString() : "An error has occurred. Plase contact the system administrator.";
            return false;
        }
    }


    public string GetScannedDocsDirectory()
    {
        string dbName = System.Web.HttpContext.Current.Session["DB"].ToString();
        bool useLocalFolder = Convert.ToBoolean(
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] != null ?
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] :
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal"]
            );

        string configName = useLocalFolder ? "PatientScannedDocsLocalFolder" : "PatientScannedDocsNetworkFolder";
        string dir = 
            System.Configuration.ConfigurationManager.AppSettings[configName + "_" + dbName] != null ?
            System.Configuration.ConfigurationManager.AppSettings[configName + "_" + dbName] :
            System.Configuration.ConfigurationManager.AppSettings[configName].Replace("[DBNAME]", dbName);

        dir = dir.EndsWith(@"\") ? dir : dir + @"\";
        return dir + "BK_" + this.BookingID + @"\";
    }
    public string GetScannedDocsImageLink(bool includeHoverFileList = false)
    {
        string linkTitle = "Scanned Documents";
        if (includeHoverFileList)
        {
            try
            {
                linkTitle = string.Empty;
                FileInfo[] scannedDocs = this.GetScannedDocs();
                string scannedDocsDir = this.GetScannedDocsDirectory();
                foreach (FileInfo fi in scannedDocs)
                    linkTitle += (linkTitle.Length == 0 ? "" : "\r\n") + fi.FullName.Substring(scannedDocsDir.Length);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // don't need to set any message - it just wont show the current files
            }
        }

        string ptFilesOnclick = "open_new_tab('PatientScannedFileUploadsV2.aspx?booking=" + this.BookingID + "');return false;";
        return "<input type=\"image\" onclick=\"" + ptFilesOnclick + "\" title=\"" + linkTitle + "\" alt=\"Scanned Documents\" src=\"images/documents-icon-24-thin.png\"/>"; ;
    }
    public FileInfo[] GetScannedDocs()
    {
        string dbName = System.Web.HttpContext.Current.Session["DB"].ToString().Replace("_TestDB", "");
        bool useLocalFolder = Convert.ToBoolean(
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] != null ?
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] :
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal"]
            );

        if (useLocalFolder)
        {

            string s = GetScannedDocsDirectory();

            if (!Directory.Exists(GetScannedDocsDirectory()))
                return new FileInfo[] { };

            DirectoryInfo dir = new DirectoryInfo(GetScannedDocsDirectory());
            return (FileInfo[])GetFiles(dir).ToArray(typeof(FileInfo));  // dir.GetFiles();
        }
        else  // use network folder
        {
            string username    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderUsername"];
            string password    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderPassword"];

            string networkName =
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] != null ?
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] :
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder"];

            if (networkName.EndsWith(@"\"))
                networkName = networkName.Substring(0, networkName.Length - 1);

            using (new NetworkConnection(networkName, new System.Net.NetworkCredential(username, password)))
            {
                if (!System.IO.Directory.Exists(GetScannedDocsDirectory()))
                    return new FileInfo[] { };
                DirectoryInfo dir = new DirectoryInfo(GetScannedDocsDirectory() + @"\");
                return (FileInfo[])GetFiles(dir).ToArray(typeof(FileInfo));  // dir.GetFiles();
            }
        }

        /*
        if (!Directory.Exists(GetScannedDocsDirectory()))
            return new FileInfo[]{};

        DirectoryInfo dir = new DirectoryInfo(GetScannedDocsDirectory() + @"\");
        return dir.GetFiles();
        */
    }
    protected ArrayList GetFiles(DirectoryInfo dir)
    {
        ArrayList list = new ArrayList();
        foreach (DirectoryInfo subDir in dir.GetDirectories())
            list.AddRange(GetFiles(subDir));

        list.AddRange(dir.GetFiles());

        return list;
    }



    // check one time bookings
    public static bool HasOverlap(Booking[] bookings, DateTime startDateTime, DateTime endDateTime, Booking ignoreBooking = null)
    {
        foreach (Booking booking in bookings)
        {
            if (ignoreBooking != null && booking.BookingID == ignoreBooking.BookingID)
                continue;    // dont include this booking .. can overlapt itself

            if (booking.IsRecurring)
            {
                // if its a recurring booking and end "date" is after start "date", and end date is at "00:00", then intersection of "date" should not incude the last "day"
                DateTime bookingEndDate = (booking.DateEnd != DateTime.MinValue && booking.DateStart.Date != booking.DateEnd.Date && booking.DateEnd.TimeOfDay == TimeSpan.Zero) ? booking.DateEnd.AddDays(-1) : booking.DateEnd;

                if (DateIntersects(booking.DateStart, bookingEndDate, startDateTime, endDateTime, booking.RecurringDayOfWeek) &&
                    TimeIntersects(booking.RecurringStartTime, booking.RecurringEndTime, startDateTime.TimeOfDay, endDateTime.TimeOfDay))
                    return true;
            }
            else
            {
                if (booking.DateStart < endDateTime && booking.DateEnd > startDateTime)
                    return true;
            }
        }
        return false;
    }

    public static Booking[] GetOverlappingBookings(Booking[] bookings, DateTime startDateTime, DateTime endDateTime, Booking ignoreBooking = null)
    {
        System.Collections.ArrayList overlappingBookings = new System.Collections.ArrayList();

        foreach (Booking booking in bookings)
        {
            if (ignoreBooking != null && booking.BookingID == ignoreBooking.BookingID)
                continue;    // dont include this booking .. can overlapt itself

            if (booking.IsRecurring)
            {
                // if its a recurring booking and end "date" is after start "date", and end date is at "00:00", then intersection of "date" should not incude the last "day"
                DateTime bookingEndDate = (booking.DateEnd != DateTime.MinValue && booking.DateStart.Date != booking.DateEnd.Date && booking.DateEnd.TimeOfDay == TimeSpan.Zero) ? booking.DateEnd.AddDays(-1) : booking.DateEnd;

                if (DateIntersects(booking.DateStart, bookingEndDate, startDateTime, endDateTime, booking.RecurringDayOfWeek) &&
                    TimeIntersects(booking.RecurringStartTime, booking.RecurringEndTime, startDateTime.TimeOfDay, endDateTime.TimeOfDay))
                    overlappingBookings.Add(booking);
            }
            else
            {
                if (booking.DateStart < endDateTime && booking.DateEnd > startDateTime)
                    overlappingBookings.Add(booking);
            }
        }
        return (Booking[])overlappingBookings.ToArray(typeof(Booking));
    }

    // check recurring bookings
    public static Booking HasOverlap(Booking[] bookings, DateTime startDateTime, DateTime endDateTime, string days, TimeSpan startTime, TimeSpan endTime, int everyNWeeks = 1, Booking ignoreBooking = null)
    {
        for (int i = 0; i < 7; i++)
        {
            if (days[i] == '0')
                continue;

            DayOfWeek dayOfWeek = (DayOfWeek)((((int)DayOfWeek.Sunday) + i) % 7);

            Booking clash = HasOverlap(bookings, startDateTime, endDateTime, dayOfWeek, startTime, endTime, everyNWeeks, ignoreBooking);
            if (clash != null)
                return clash;
            //if (HasOverlap(bookings, startDateTime, endDateTime, dayOfWeek, startTime, endTime, ignoreBooking))
            //    return true;
        }

        return null;
    }
    protected static Booking HasOverlap(Booking[] bookings, DateTime startDateTime, DateTime endDateTime, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, int everyNWeeks = 1, Booking ignoreBooking = null)
    {
        foreach (Booking booking in bookings)
        {
            if (ignoreBooking != null && booking.BookingID == ignoreBooking.BookingID)
                continue;    // dont include this booking .. can overlapt itself


            // if its a recurring booking and end "date" is after start "date", and end date is at "00:00", then intersection of "date" should not incude the last "day"
            DateTime bookingEndDate = (booking.DateEnd != DateTime.MinValue && booking.DateStart.Date != booking.DateEnd.Date && booking.DateEnd.TimeOfDay == TimeSpan.Zero) ? booking.DateEnd.AddDays(-1) : booking.DateEnd;
            DateTime thisEndDate    = (endDateTime     != DateTime.MinValue && startDateTime.Date     != endDateTime.Date     && endDateTime.TimeOfDay     == TimeSpan.Zero) ? endDateTime.AddDays(-1)     : endDateTime;


            if (everyNWeeks == 1)
            {
                if (booking.IsRecurring)
                {
                    if (DateIntersects(booking.DateStart, bookingEndDate, startDateTime, thisEndDate, dayOfWeek) &&
                        TimeIntersects(booking.RecurringStartTime, booking.RecurringEndTime, startTime, endTime))
                        return booking;
                }
                else
                {
                    if (DateIntersects(booking.DateStart.Date, booking.DateEnd.Date, startDateTime, thisEndDate, dayOfWeek) &&
                        TimeIntersects(booking.DateStart.TimeOfDay, booking.DateEnd.TimeOfDay, startTime, endTime))
                        return booking;
                }
            }
            else
            {
                // get which dates will occur, and create individual bookings
                DateTime curStartDate = startDateTime;
                while (curStartDate.DayOfWeek != dayOfWeek)
                    curStartDate = curStartDate.AddDays(1);

                DateTime curStartDateTime = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, startTime.Hours, startTime.Minutes, 0);
                DateTime curEndDateTime = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, endTime.Hours, endTime.Minutes, 0);
                int weekNbr = 0;
                while (curStartDateTime.Date <= endDateTime.Date)
                {
                    if (weekNbr % everyNWeeks == 0)
                    {
                        if (booking.IsRecurring)
                        {
                            if (DateIntersects(booking.DateStart, bookingEndDate, curStartDateTime.Date, curStartDateTime.Date, dayOfWeek) &&
                                TimeIntersects(booking.RecurringStartTime, booking.RecurringEndTime, startTime, endTime))
                                return booking;
                        }
                        else
                        {
                            if (DateIntersects(booking.DateStart.Date, booking.DateEnd.Date, curStartDateTime.Date, curStartDateTime.Date, dayOfWeek) &&
                                TimeIntersects(booking.DateStart.TimeOfDay, booking.DateEnd.TimeOfDay, startTime, endTime))
                                return booking;
                        }
                    }

                    curStartDateTime = curStartDateTime.AddDays(7);
                    curEndDateTime = curEndDateTime.AddDays(7);
                    weekNbr++;
                }
            }
        }
        return null;
    }

    public static bool TimeIntersects(TimeSpan starTime1, TimeSpan endTime1, TimeSpan startTime2, TimeSpan endTime2)
    {
        return starTime1 < endTime2 && endTime1 > startTime2;
    }
    public static bool DateIntersects(DateTime starTime1, DateTime endTime1, DateTime startTime2, DateTime endTime2, DayOfWeek dayOfWeek)
    {
        DateTime[] intersection = GetIntersection(starTime1, endTime1, startTime2, endTime2);
        if (intersection == null)
            return false;

        for (DateTime d = intersection[0]; (intersection[1] == DateTime.MinValue || d <= intersection[1]) && d < intersection[0].AddDays(9); d = d.AddDays(1))
            if (d.DayOfWeek == dayOfWeek)
                return true;

        return false;
    }

    protected static DateTime[] GetIntersection(DateTime mainStart, DateTime mainEnd, DateTime intervalStart, DateTime intervalEnd)
    {
        if (mainEnd == DateTime.MinValue && intervalEnd == DateTime.MinValue)
            return GetIntersection(mainStart, intervalStart);
        if (intervalEnd == DateTime.MinValue)
            return GetIntersection(mainStart, mainEnd, intervalStart);
        if (mainEnd == DateTime.MinValue)
            return GetIntersection(intervalStart, intervalEnd, mainStart);


        if (mainStart > mainEnd || intervalStart > intervalEnd)
            return null;
        if (intervalStart > mainEnd || intervalEnd < mainStart)
            return null;

        if (intervalStart >= mainStart && intervalEnd <= mainEnd)
            return new DateTime[] { intervalStart, intervalEnd};

        DateTime tempStart = intervalStart;
        DateTime tempEnd   = intervalEnd;

        if (mainStart > intervalStart) tempStart = mainStart;
        if (mainEnd   < intervalEnd)   tempEnd   = mainEnd;
        return new DateTime[] { tempStart, tempEnd };
    }
    protected static DateTime[] GetIntersection(DateTime mainStart, DateTime intervalStart)
    {
        return new DateTime[] { mainStart > intervalStart ? mainStart : intervalStart, DateTime.MinValue };
    }
    protected static DateTime[] GetIntersection(DateTime mainStart, DateTime mainEnd, DateTime intervalStart)
    {
        if (mainStart > mainEnd)
            return null;

        if (intervalStart > mainEnd)
            return null;


        DateTime tempStart = intervalStart;

        if (mainStart > intervalStart) tempStart = mainStart;
        return new DateTime[] { tempStart, mainEnd };
    }


    public static Booking[] GetOverlappingBookings(Booking[] bookings, DateTime startDateTime, DateTime endDateTime, string days, TimeSpan startTime, TimeSpan endTime, int everyNWeeks = 1, Booking ignoreBooking = null)
    {
        System.Collections.ArrayList overlappingBookings = new System.Collections.ArrayList();

        for (int i = 0; i < 7; i++)
        {
            if (days[i] == '0')
                continue;

            DayOfWeek dayOfWeek = (DayOfWeek)((((int)DayOfWeek.Sunday) + i) % 7);
            overlappingBookings.AddRange(GetOverlappingBookings(bookings, startDateTime, endDateTime, dayOfWeek, startTime, endTime, everyNWeeks, ignoreBooking));
        }

        return (Booking[])overlappingBookings.ToArray(typeof(Booking));
    }
    protected static Booking[] GetOverlappingBookings(Booking[] bookings, DateTime startDateTime, DateTime endDateTime, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, int everyNWeeks = 1, Booking ignoreBooking = null)
    {
        System.Collections.ArrayList overlappingBookings = new System.Collections.ArrayList();

        foreach (Booking booking in bookings)
        {
            if (ignoreBooking != null && booking.BookingID == ignoreBooking.BookingID)
                continue;    // dont include this booking .. can overlapt itself


            // if its a recurring booking and end "date" is after start "date", and end date is at "00:00", then intersection of "date" should not incude the last "day"
            DateTime bookingEndDate = (booking.DateEnd != DateTime.MinValue && booking.DateStart.Date != booking.DateEnd.Date && booking.DateEnd.TimeOfDay == TimeSpan.Zero) ? booking.DateEnd.AddDays(-1) : booking.DateEnd;
            DateTime thisEndDate = (endDateTime != DateTime.MinValue && startDateTime.Date != endDateTime.Date && endDateTime.TimeOfDay == TimeSpan.Zero) ? endDateTime.AddDays(-1) : endDateTime;

            if (everyNWeeks == 1)
            {
                if (booking.IsRecurring)
                {
                    if (DateIntersects(booking.DateStart, bookingEndDate, startDateTime, thisEndDate, dayOfWeek) &&
                        TimeIntersects(booking.RecurringStartTime, booking.RecurringEndTime, startTime, endTime))
                        overlappingBookings.Add(booking);
                }
                else
                {
                    if (DateIntersects(booking.DateStart.Date, booking.DateEnd.Date, startDateTime, thisEndDate, dayOfWeek) &&
                        TimeIntersects(booking.DateStart.TimeOfDay, booking.DateEnd.TimeOfDay, startTime, endTime))
                        overlappingBookings.Add(booking);
                }
            }
            else
            {
                // get which dates will occur, and create individual bookings
                DateTime curStartDate = startDateTime;
                while (curStartDate.DayOfWeek != dayOfWeek)
                    curStartDate = curStartDate.AddDays(1);

                DateTime curStartDateTime = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, startTime.Hours, startTime.Minutes, 0);
                DateTime curEndDateTime = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, endTime.Hours, endTime.Minutes, 0);
                int weekNbr = 0;
                while (curStartDateTime.Date <= endDateTime.Date)
                {
                    if (weekNbr % everyNWeeks == 0)
                    {
                        if (booking.IsRecurring)
                        {
                            if (DateIntersects(booking.DateStart, bookingEndDate, curStartDateTime.Date, curStartDateTime.Date, dayOfWeek) &&
                                TimeIntersects(booking.RecurringStartTime, booking.RecurringEndTime, startTime, endTime))
                                overlappingBookings.Add(booking);
                        }
                        else
                        {
                            if (DateIntersects(booking.DateStart.Date, booking.DateEnd.Date, curStartDateTime.Date, curStartDateTime.Date, dayOfWeek) &&
                                TimeIntersects(booking.DateStart.TimeOfDay, booking.DateEnd.TimeOfDay, startTime, endTime))
                                overlappingBookings.Add(booking);
                        }
                    }

                    curStartDateTime = curStartDateTime.AddDays(7);
                    curEndDateTime = curEndDateTime.AddDays(7);
                    weekNbr++;
                }
            }
        }

        return (Booking[])overlappingBookings.ToArray(typeof(Booking));
    }



    public DateTime GetEPCDateSigned(Patient patient)  // can be patient on invlice line, not booking patient -- so keep as argument
    {
        return HealthCardActionDB.GetEPCDateSigned(patient.PatientID, this.DateStart.Date);
    }

    public string GetBookingSheetLink()
    {

        DateTime dateStart = this.DateStart;

        if (this.IsRecurring)
        {
            DayOfWeek bkDayOfWeek = this.RecurringDayOfWeek;
            while (dateStart.DayOfWeek != this.RecurringDayOfWeek)
                dateStart = dateStart.AddDays(1);
        }

        string urlParams = string.Empty;
        if (this.Organisation != null)
            urlParams += (urlParams.Length == 0 ? "?" : "&") + "orgs=" + this.Organisation.OrganisationID;
        if (this.Patient != null)
            urlParams += (urlParams.Length == 0 ? "?" : "&") + "patient=" + this.Patient.PatientID;
        //if (this.Offering != null)
        //    urlParams += (urlParams.Length == 0 ? "?" : "&") + "offering=" + this.Offering.OfferingID;
        urlParams += (urlParams.Length == 0 ? "?" : "&") + "scroll_to_cell=" + "td_" + this.Organisation.OrganisationID + "_" + this.provider.StaffID + "_" + this.DateStart.ToString("yyyy_MM_dd_HHmm");
        urlParams += (urlParams.Length == 0 ? "?" : "&") + "date=" + dateStart.ToString("yyyy_MM_dd");

        //return String.Format("~/BookingsV2.aspx?type=patient&patient={0}&org={1}&staff={2}&offering={3}&date={4}", this.Patient.PatientID, this.Organisation.OrganisationID, this.Provider.StaffID, this.Offering.OfferingID, this.DateStart.ToString("yyyy_MM_dd"));
        //return String.Format("~/BookingsV2.aspx?orgs={0}&patient={1}&offering={2}&date={3}", this.Organisation.OrganisationID, this.Patient.PatientID, this.Offering.OfferingID, this.DateStart.ToString("yyyy_MM_dd"));
        return "~/BookingsV2.aspx"+urlParams;
    }
    public string GetBookingSheetLinkV2(bool show_unavailable_staff = false)
    {

        DateTime dateStart = this.DateStart;

        if (this.IsRecurring)
        {
            DayOfWeek bkDayOfWeek = this.RecurringDayOfWeek;
            while (dateStart.DayOfWeek != this.RecurringDayOfWeek)
                dateStart = dateStart.AddDays(1);
        }

        string urlParams = string.Empty;
        if (this.Organisation != null)
            urlParams += (urlParams.Length == 0 ? "?" : "&") + "orgs=" + this.Organisation.OrganisationID;
        if (this.Patient != null)
            urlParams += (urlParams.Length == 0 ? "?" : "&") + "patient=" + this.Patient.PatientID;
        //if (this.Offering != null)
        //    urlParams += (urlParams.Length == 0 ? "?" : "&") + "offering=" + this.Offering.OfferingID;
        urlParams += (urlParams.Length == 0 ? "?" : "&") + "scroll_to_cell=" + "td_" + this.Organisation.OrganisationID + "_" + this.provider.StaffID + "_" + this.DateStart.ToString("yyyy_MM_dd_HHmm");
        urlParams += (urlParams.Length == 0 ? "?" : "&") + "date=" + dateStart.ToString("yyyy_MM_dd");
        if (show_unavailable_staff)
            urlParams += (urlParams.Length == 0 ? "?" : "&") + "show_unavailable_staff=1";


        //return String.Format("~/BookingsV2.aspx?type=patient&patient={0}&org={1}&staff={2}&offering={3}&date={4}", this.Patient.PatientID, this.Organisation.OrganisationID, this.Provider.StaffID, this.Offering.OfferingID, this.DateStart.ToString("yyyy_MM_dd"));
        //return String.Format("~/BookingsV2.aspx?orgs={0}&patient={1}&offering={2}&date={3}", this.Organisation.OrganisationID, this.Patient.PatientID, this.Offering.OfferingID, this.DateStart.ToString("yyyy_MM_dd"));
        return "~/BookingsV2.aspx"+urlParams;
    }

    public string[] GetNoteTextForTreatmentLetter(Referrer referrer, string newline = "\n", bool incNoteIDForDebug = false)
    {
        if (this.NoteCount == 0)
            return new string[0];

        System.Collections.ArrayList notesList = new System.Collections.ArrayList();
        foreach (Note note in GetTreatmentNotes())
            notesList.Add(this.GetNoteTextForTreatmentLetter(referrer, note, newline, incNoteIDForDebug));
        return (string[])notesList.ToArray(typeof(string));
    }
    public string GetNoteTextForTreatmentLetter(Referrer referrer, Note note, string newline = "\n", bool incNoteIDForDebug = false)
    {
        string noteText = string.Empty;
        
        if (incNoteIDForDebug)
            noteText += "Note ID: " + note.NoteID + newline + newline;

        noteText += "Patient: " + this.Patient.Person.Fullname + newline;
        if (referrer != null)
            noteText += "Referrer: " + referrer.Person.FullnameWithoutMiddlename + newline;
        noteText += newline;
        noteText += "Treatment Date: " + this.DateStart.ToString("d MMMM, yyyy") + newline;
        noteText += "Treatment Organisation: " + this.Organisation.Name + newline;
        noteText += "Treatment Service: " + this.Offering.Name + newline;
        noteText += newline;
        noteText += newline;
        noteText += "Treatment Note:" + newline;
        noteText += newline;
        noteText += note.Text.Replace("\n", newline);

        return noteText;
    }
    public Note[] GetTreatmentNotes()
    {
        if (this.NoteCount == 0)
            return new Note[] { };

        return NoteDB.GetByEntityID(this.EntityID, "252");
    }

    public Letter.FileContents GetTreatmentLetter(Letter.FileFormat fileFormat, Patient patient, HealthCard hc, Referrer referrer, int siteID, int staffID, bool keepInHistory, int letterPrintHistorySendMethodID)
    {
        Letter.FileContents fileContentsStandardTreatment = Letter.SendTreatmentLetter(fileFormat, this, patient, hc, Letter.TreatmentLetterType.TreatmentNotes, Booking.InvoiceType.Medicare, this.Offering.Field.ID, siteID, staffID, referrer, keepInHistory, letterPrintHistorySendMethodID);
        if (fileContentsStandardTreatment == null)
        {
            System.Data.DataTable tbl = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "field_id=" + this.Offering.Field.ID, "", "field_id", "descr");
            IDandDescr field = IDandDescrDB.Load(tbl.Rows[0], "field_id", "descr");
            throw new CustomMessageException("Failed to generage treatment notes letter. No treatment notes letter template created for " + field.Descr);
        }

        return fileContentsStandardTreatment;
    }


    public Letter.FileContents[] GetSystemLettersList(Letter.FileFormat fileFormat, Patient patient, HealthCard hc, int fieldID, Referrer referrer, bool keepInHistory, bool needToGenerateFirstLetter, bool needToGenerateLastLetter, bool needToGenerateTreatmentLetter, bool needToGenerateLastLetterPT, int siteID, int staffID, int letterPrintHistorySendMethodID)
    {
        System.Collections.ArrayList fileContentsArrayList = new System.Collections.ArrayList();

        if (needToGenerateFirstLetter)
        {
            Letter.FileContents fileContents = Letter.SendTreatmentLetter(fileFormat, this, patient, hc, Letter.TreatmentLetterType.First, Booking.InvoiceType.Medicare, fieldID, siteID, staffID, null, keepInHistory, letterPrintHistorySendMethodID);
            if (fileContents == null)
                throw new CustomMessageException("Failed to generage first letter. No first letter template created for " + GetField().Descr);
            fileContentsArrayList.Add(fileContents);
        }
        if (needToGenerateLastLetter)
        {
            Letter.FileContents fileContents = Letter.SendTreatmentLetter(fileFormat, this, patient, hc, Letter.TreatmentLetterType.Last, Booking.InvoiceType.Medicare, fieldID, siteID, staffID, null, keepInHistory, letterPrintHistorySendMethodID);
            if (fileContents == null)
                throw new CustomMessageException("Failed to generage last letter. No last letter template created for " + GetField().Descr);
            fileContentsArrayList.Add(fileContents);
        }
        if (!needToGenerateFirstLetter && !needToGenerateLastLetter && needToGenerateTreatmentLetter)
        {
            Letter.FileContents fileContents = Letter.SendTreatmentLetter(fileFormat, this, patient, hc, Letter.TreatmentLetterType.TreatmentNotes, Booking.InvoiceType.Medicare, fieldID, siteID, staffID, referrer, keepInHistory, letterPrintHistorySendMethodID);
            if (fileContents == null)
                throw new CustomMessageException("Failed to generage treatment notes letter. No treatment notes letter template created for " + GetField().Descr);
            fileContentsArrayList.Add(fileContents);
        }
        if (needToGenerateLastLetterPT)
        {
            Letter.FileContents fileContents = Letter.SendTreatmentLetter(fileFormat, this, patient, hc, Letter.TreatmentLetterType.LastPT, Booking.InvoiceType.Medicare, fieldID, siteID, staffID, null, keepInHistory, letterPrintHistorySendMethodID);
            if (fileContents == null)
                throw new CustomMessageException("Failed to generage pt last letter. No pt last letter template created for " + GetField().Descr);
            fileContentsArrayList.Add(fileContents);
        }

        Letter.FileContents[] fileContentsList = (Letter.FileContents[])fileContentsArrayList.ToArray(typeof(Letter.FileContents));

        return fileContentsList;
    }
    public IDandDescr GetField()
    {
        int fieldID = this.Offering != null ? this.Offering.Field.ID : this.provider.Field.ID;
        System.Data.DataTable tbl = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "field_id=" + fieldID, "", "field_id", "descr");
        IDandDescr field = IDandDescrDB.Load(tbl.Rows[0], "field_id", "descr");
        return field;
    }


    public string GetBookingChangeHistoryPopupLink(string text = "H", string js_to_call = null, bool only_js = false, bool usePopup = true)
    {
        string url = "BookingChangeHistoryPopupV2.aspx?id=" + this.BookingID;

        string js =
            usePopup ?
            "javascript:window.showModalDialog('" + url + @"', '', 'dialogWidth:850px;dialogHeight:550px;center:yes;resizable:no;scroll:no');" + (js_to_call != null ? js_to_call : "") + @"return false;" :
            "javascript: open_new_tab('" + url + "');return false;";
        string onclick = "onclick=\"" + js + "\"";

        return only_js ? js : "<a " + onclick + " href=\"\">" + text + "</a>";
    }
    public string GetBookingChangeHistoryPopupLinkImage(string js_to_call = null, bool only_js = false, bool usePopup = true)
    {
        string url = "BookingChangeHistoryPopupV2.aspx?id=" + this.BookingID;

        string js =
            usePopup ?
            "javascript:window.showModalDialog('" + url + @"', '', 'dialogWidth:850px;dialogHeight:550px;center:yes;resizable:no;scroll:no');" + (js_to_call != null ? js_to_call : "") + @"return false;" :
            "javascript: open_new_tab('" + url + "');return false;";
        string onclick = "onclick=\"" + js + "\"";

        return only_js ? js : @"<input title=""View Booking Movement History"" src=""/images/Letter-H-gold-24-Thin.png"" " + onclick + @" type=""image"">";
    }

    public static string GetLink(DateTime startDate, int[] orgIDs, int nDays = -1, int patientID = -1, Tuple<int, int, DateTime> scrollTocell = null) 
    {
        string url = "/BookingsV2.aspx?orgs=" + string.Join("_", orgIDs) +
            (patientID != -1 ? "&patient=" + patientID : "") +
            (nDays != -1 ? "&ndays=" + nDays : "") +
            (startDate != DateTime.Today && startDate != DateTime.MinValue ? "&date=" + startDate.ToString("yyyy_MM_dd") : "") +
            (scrollTocell != null ? "&scroll_to_cell=td_" + scrollTocell.Item1 + "_" + scrollTocell.Item2 + "_" + scrollTocell.Item3.ToString("yyyy_MM_dd_HHmm") : "");


        return url;
    }



    #region SendReminderEmail

    public void SendReminderEmail(Booking previousBooking = null)
    {
        if (Utilities.IsDev())
            return;

        if (this.BookingTypeID != 34)       // only bookings, not days marked off
            return;

        if (this.Patient == null)           // prob aged care booking ... get patients list when do aged care system
            return;

        if (this.DateStart < DateTime.Now)  // don't send a reminder if booking is made for in the past
            return;


        IDandDescr field = this.Offering != null ? this.Offering.Field : null;
        if (field == null || field.ID == 0)
        {
            System.Data.DataTable tbl = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "field_id=" + this.provider.Field.ID, "", "field_id", "descr");
            field = IDandDescrDB.Load(tbl.Rows[0], "field_id", "descr");
        }


        string emailPatient = GetEmail(this.Patient.Person.EntityID);
        if (emailPatient == null)
            return;

        string phoneNumOrg = GetOrgPhoneNbrs(this.Organisation.EntityID);
        if (phoneNumOrg == null)
            phoneNumOrg = string.Empty;



        Site site = SiteDB.GetSiteByType(this.Organisation.IsAgedCare ? SiteDB.SiteType.AgedCare : SiteDB.SiteType.Clinic);

        string subjectCalendar = (field.ID == 0 ? @"Appointment - " : Utilities.FormatName(field.Descr) + @" appointment - ")  + this.Organisation.Name;
        string subjectEmail    = (field.ID == 0 ? @"Appointment"    : Utilities.FormatName(field.Descr) + @" appointment")     + (previousBooking == null ? "" : " UPDATE") + @" - " + this.DateStart.ToString("h:mm") + (this.DateStart.Hour < 12 ? "am" : "pm") + " at " + this.Organisation.Name + " on " + this.DateStart.ToString("d MMM, yyyy");

        string bodyCalendar    = (field.ID == 0 ? "Appointment at " : Utilities.FormatName(field.Descr) + @" appointment at ") + this.Organisation.Name + @". To cancel or change your booking, contact us" + (phoneNumOrg == null || phoneNumOrg.Length == 0 ? "." : " on " + phoneNumOrg) + ".";
        string bodyEmail = 
@"Hello " + this.Patient.Person.Firstname + @"<br />
<br />
This is a courtesy message to advise you that you have " + (field.ID == 0 ? @"an appointment at " : " a " + field.Descr.ToLower() + " appointment at ") + this.Organisation.Name + " on " + this.DateStart.ToString("ddd MMM d, yyyy") + " " + this.DateStart.ToString("h:mm") + (this.DateStart.Hour < 12 ? "am" : "pm") + @".<br />
<br />
If for any reason you need to cancel or change your booking, please contact us" + (phoneNumOrg == null || phoneNumOrg.Length == 0 ? "." : " on " + phoneNumOrg) + @".<br />
<br />
Have a great day!<br />
<br />
Regards,<br />
" + site.Name;

        if (previousBooking != null)
            bodyEmail = 
@"Hello " + this.Patient.Person.Firstname + @"<br />
<br />

This is a courtesy message to advise you that your " + (field.ID == 0 ? "" : field.Descr.ToLower()) + @" appointment has been changed.<br />
<br />
<table border=""0"" cellspacing=""0"" cellpadding=""0"">
  <tr>
    <td>
        Prevoiusly
    </td>
    <td style=""width:15px;""></td>
    <td>
        " + previousBooking.DateStart.ToString("ddd MMM d, yyyy") + " " + previousBooking.DateStart.ToString("h:mm") + (previousBooking.DateStart.Hour < 12 ? "am" : "pm") + @".<br />
    </td>
  </tr>
  <tr>
    <td>
        <b>Now</b>
    </td>
    <td style=""width:15px;""></td>
    <td>
        <b>" + this.DateStart.ToString("ddd MMM d, yyyy") + " " + this.DateStart.ToString("h:mm") + (this.DateStart.Hour < 12 ? "am" : "pm") + @"</b>.<br />
    </td>
  </tr>
</table>

<br />
If for any reason you need to cancel or change your booking, please contact us" + (phoneNumOrg == null || phoneNumOrg.Length == 0 ? "." : " on " + phoneNumOrg) + @".<br />
<br />
Have a great day!<br />
<br />
Regards,<br />
" + site.Name;


        bool isHTML = true;

        string location  = this.Organisation.Name;
        DateTime start   = this.DateStart;
        DateTime end     = this.DateEnd;
        string fromName  = site.Name;
        string fromEmail = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value;

        Emailer.AsyncSimpleEmail(
            fromEmail,
            fromName,
            emailPatient,
            subjectEmail,
            isHTML ? string.Empty : bodyEmail,  // if HTML, then put in as an AlternateView to preserve the HTML
            isHTML,
            isHTML ?
                new System.Net.Mail.AlternateView[] { Emailer.MakeCalendarView(start, end, subjectCalendar, bodyCalendar, location, fromName, fromEmail), Emailer.MakeBodyView(bodyEmail) } :
                new System.Net.Mail.AlternateView[] { Emailer.MakeCalendarView(start, end, subjectCalendar, bodyCalendar, location, fromName, fromEmail) }
            );

    }
    protected static string GetOrgPhoneNbrs(int entityID)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] contacts = ContactDB.GetByEntityIDs(-1, new int[] { entityID }, 34, -1);
            if (contacts == null || contacts.Length == 0)
                return null;

            string nbrs = string.Empty;
            for (int i = 0; i < contacts.Length; i++)
            {
                if (i > 0 && i == contacts.Length - 1)
                    nbrs += " or " + contacts[i].GetFormattedPhoneNumber();
                else if (i > 0)
                    nbrs += ", " + contacts[i].GetFormattedPhoneNumber();
                else
                    nbrs += contacts[i].GetFormattedPhoneNumber();
            }

            return nbrs;

        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] contacts = ContactAusDB.GetByEntityIDs(-1, new int[] { entityID }, 34, -1);
            if (contacts == null || contacts.Length == 0)
                return null;

            string nbrs = string.Empty;
            for (int i = 0; i < contacts.Length; i++)
            {
                if (i > 0 && i == contacts.Length - 1)
                    nbrs += " or " + contacts[i].GetFormattedPhoneNumber();
                else if (i > 0)
                    nbrs += ", " + contacts[i].GetFormattedPhoneNumber();
                else
                    nbrs += contacts[i].GetFormattedPhoneNumber();
            }

            return nbrs;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
    }
    protected static string GetEmail(int entityID)
    {
        return ContactDB.GetEmailsCommaSepByEntityID(entityID, false, true);
    }

    #endregion

    #region GetInvoiceType()

    public enum InvoiceType { Medicare, DVA, Insurance, None, NoneFromCombinedYearlyThreshholdReached, NoneFromOfferingYearlyThreshholdReached, NoneFromExpired };

    public static int GetInvoiceTypeOrgID(Booking.InvoiceType invType)
    {
        switch (invType)
        {
            case (Booking.InvoiceType.Medicare):
                return -1;
            case (Booking.InvoiceType.DVA):
                return -2;
            case (Booking.InvoiceType.Insurance):
                return -3;
            default:
                return 0;
        }
    }

    public InvoiceType GetInvoiceType(Offering offering = null, Patient patient = null)
    {
        return GetInvoiceType(
            HealthCardDB.GetActiveByPatientID((patient == null ? this.Patient : patient).PatientID), 
            offering == null ? this.Offering : offering, 
            patient  == null ? this.Patient  : patient, 
            null, 
            null, 
            -1);
    }
    public InvoiceType GetInvoiceType(HealthCard hc, Offering offering, Patient patient, Hashtable patientsMedicareCountCache, Hashtable epcRemainingCache, int MedicareMaxNbrServicesPerYear)
    {
        if (this.BookingStatus.ID != 0)
            throw new CustomMessageException("Booking already set as : " + BookingDB.GetStatusByID(this.BookingStatus.ID).Descr);

        if (hc != null && hc.Organisation.OrganisationType.OrganisationTypeGroup.ID == 7)
            return InvoiceType.Insurance;

        if (hc == null)
            return InvoiceType.None;


        if (this.provider.Field.ID == 1) // is GP so referreral/EPC is not relevant
        {
            bool isValidMCClaim  = hc.Organisation.OrganisationID == -1 && offering.MedicareCompanyCode.Length > 0;
            bool isValidDVAClaim = hc.Organisation.OrganisationID == -2 && offering.DvaCompanyCode.Length      > 0;
            if (isValidMCClaim || isValidDVAClaim)
                return  (hc.ExpiryDate == DateTime.MinValue || hc.ExpiryDate.Date < DateTime.Today) ? InvoiceType.NoneFromExpired : InvoiceType.Medicare;
        }
        else
        {
            if (!hc.HasEPC(epcRemainingCache))
                return InvoiceType.None;


            bool within1YearOfCard           = this.DateStart.Date >= hc.DateReferralSigned.Date && this.DateStart.Date < hc.DateReferralSigned.Date.AddYears(1); // tested and works!
            bool offeringHasMedicareClaimNbr = offering.MedicareCompanyCode.Length > 0;
            bool offeringHasDVAClaimNbr      = offering.DvaCompanyCode.Length      > 0;


            if (hc.Organisation.OrganisationID == -1 && offeringHasMedicareClaimNbr)
            {
                if (!within1YearOfCard) // in this if block so 'NoneFromExpired' only returned if is medicare/dva card
                    return InvoiceType.NoneFromExpired;


                int maxNbrMedicareServicesPerYear = MedicareMaxNbrServicesPerYear != -1 ? MedicareMaxNbrServicesPerYear : Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);
                int nbrMedicareServicesSoFarThisYear = patientsMedicareCountCache == null ?
                    (int)InvoiceDB.GetMedicareCountByPatientAndYear(patient.PatientID, this.DateStart.Year) :
                    (int)patientsMedicareCountCache[patient.PatientID];
                bool belowYearlyMedicareThreshhold = nbrMedicareServicesSoFarThisYear < maxNbrMedicareServicesPerYear;

                bool belowMedicareThisServiceThreshhold = true;
                if (offering.MaxNbrClaimable > 0)
                {
                    //int nbrMedicareThisServiceSoFarThisPeriod = (int)InvoiceDB.GetMedicareCountByPatientAndYear(patient.PatientID, this.DateStart.Year, offering.OfferingID);
                    int nbrMedicareThisServiceSoFarThisPeriod = (int)InvoiceDB.GetMedicareCountByPatientAndDateRange(patient.PatientID, this.DateStart.Date.AddMonths(-1 * offering.MaxNbrClaimableMonths), this.DateStart.Date, offering.OfferingID);
                    belowMedicareThisServiceThreshhold = nbrMedicareThisServiceSoFarThisPeriod < offering.MaxNbrClaimable;
                }

                if (!belowYearlyMedicareThreshhold)
                    return InvoiceType.NoneFromCombinedYearlyThreshholdReached;
                else if (!belowMedicareThisServiceThreshhold)
                    return InvoiceType.NoneFromOfferingYearlyThreshholdReached;
                else // if (belowYearlyMedicareThreshhold && belowMedicareThisServiceThreshhold)
                {

                    HealthCardEPCRemaining[] epcsRemainingForThisType = null;
                    if (epcRemainingCache == null)
                    {
                        epcsRemainingForThisType = HealthCardEPCRemainingDB.GetByHealthCardID(hc.HealthCardID, offering.Field.ID);
                    }
                    else
                    {
                        ArrayList remainingThisField = new ArrayList();
                        HealthCardEPCRemaining[] allRemainingThisHealthCareCard = (HealthCardEPCRemaining[])epcRemainingCache[hc.HealthCardID];
                        if (allRemainingThisHealthCareCard != null)
                            for (int i = 0; i < allRemainingThisHealthCareCard.Length; i++)
                                if (allRemainingThisHealthCareCard[i].Field.ID == offering.Field.ID)
                                    remainingThisField.Add(allRemainingThisHealthCareCard[i]);
                        epcsRemainingForThisType = (HealthCardEPCRemaining[])remainingThisField.ToArray(typeof(HealthCardEPCRemaining));
                    }


                    if (epcsRemainingForThisType.Length > 0 && epcsRemainingForThisType[0].NumServicesRemaining == 0)
                        return InvoiceType.NoneFromExpired;
                    if (epcsRemainingForThisType.Length > 0 && epcsRemainingForThisType[0].NumServicesRemaining > 0)
                        return InvoiceType.Medicare;
                }

            }
            if (hc.Organisation.OrganisationID == -2 && offeringHasDVAClaimNbr)
            {
                if (!within1YearOfCard) // in this if block so 'NoneFromExpired' only returned if is medicare/dva card
                    return InvoiceType.NoneFromExpired;

                bool belowMedicareThisServiceThreshhold = true;
                if (offering.MaxNbrClaimable > 0)
                {
                    //int nbrMedicareThisServiceSoFarThisPeriod = (int)InvoiceDB.GetMedicareCountByPatientAndYear(patient.PatientID, this.DateStart.Year, offering.OfferingID);
                    int nbrMedicareThisServiceSoFarThisPeriod = (int)InvoiceDB.GetMedicareCountByPatientAndDateRange(patient.PatientID, this.DateStart.Date.AddMonths(-1 * offering.MaxNbrClaimableMonths), this.DateStart.Date, offering.OfferingID);
                    belowMedicareThisServiceThreshhold = nbrMedicareThisServiceSoFarThisPeriod < offering.MaxNbrClaimable;
                }


                if (!belowMedicareThisServiceThreshhold)
                    return InvoiceType.NoneFromOfferingYearlyThreshholdReached;
                else // if (belowMedicareThisServiceThreshhold)
                    return InvoiceType.DVA;
            }
        }


        return InvoiceType.None;
    }

    #endregion

}