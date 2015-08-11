using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using System.Configuration;

public class BookingDB
{

    public static void Delete(int booking_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Booking WHERE booking_id = " + booking_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(DateTime date_start, DateTime date_end, int organisation_id, int provider, int patient_id, int offering_id, int booking_type_id, int booking_status_id, int booking_unavailability_reason_id, int added_by, int booking_confirmed_by_type_id, int confirmed_by, DateTime date_confirmed, int deleted_by, DateTime date_deleted, int cancelled_by, DateTime date_cancelled, bool is_patient_missed_appt, bool is_provider_missed_appt, bool is_emergency, bool is_recurring, DayOfWeek recurring_weekday, TimeSpan recurring_start_time, TimeSpan recurring_end_time)
    {
        int entityID = EntityDB.Insert();

        string sql = "INSERT INTO Booking (entity_id,date_start,date_end,organisation_id,provider,patient_id,offering_id,booking_type_id,booking_status_id, booking_unavailability_reason_id,added_by,booking_confirmed_by_type_id,confirmed_by,date_confirmed,deleted_by,date_deleted,cancelled_by,date_cancelled,is_patient_missed_appt,is_provider_missed_appt,is_emergency,need_to_generate_first_letter,need_to_generate_last_letter,has_generated_system_letters,is_recurring,recurring_weekday_id,recurring_start_time,recurring_end_time,arrival_time,sterilisation_code,informed_consent_added_by,informed_consent_date) VALUES (" + entityID + "," + "'" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "'," + (date_end == DateTime.MinValue ? "NULL" : "'" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + "" + (organisation_id == 0 ? "null" : organisation_id.ToString()) + "," + "" + (provider == -1 ? "null" : provider.ToString()) + "," + "" + (patient_id == -1 ? "null" : patient_id.ToString()) + "," + "" + (offering_id == -1 ? "null" : offering_id.ToString()) + "," + "" + booking_type_id + "," + "" + (booking_status_id == -1 ? "null" : booking_status_id.ToString()) + "," + (booking_unavailability_reason_id == -1 ? "null" : booking_unavailability_reason_id.ToString()) + "," + "" + added_by + "," + (booking_confirmed_by_type_id == -1 ? "null" : booking_confirmed_by_type_id.ToString()) + "," + (confirmed_by == -1 ? "null" : confirmed_by.ToString()) + "," + (date_confirmed == DateTime.MinValue ? "null" : "'" + date_confirmed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + "" + (deleted_by == -1 ? "null" : deleted_by.ToString()) + "," + (date_deleted == DateTime.MinValue ? "null" : "'" + date_deleted.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (cancelled_by == -1 ? "null" : cancelled_by.ToString()) + "," + (date_cancelled == DateTime.MinValue ? "null" : "'" + date_cancelled.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (is_patient_missed_appt ? "1," : "0,") + (is_provider_missed_appt ? "1," : "0,") + (is_emergency ? "1," : "0,") + "0,0,0," + (is_recurring ? "1," : "0,") + WeekDayDB.GetWeekDayID(recurring_weekday) + ",'" + (recurring_start_time == null ? TimeSpan.Zero.ToString() : recurring_start_time.ToString()) + "','" + (recurring_end_time == null ? TimeSpan.Zero.ToString() : recurring_end_time.ToString()) + "',NULL,'',NULL,NULL);SELECT SCOPE_IDENTITY();";
        int booking_id = Convert.ToInt32(DBBase.ExecuteSingleResult(sql));

        SendPTSelfBookingAlertEmail(booking_id, DBAction.Insert, DateTime.MinValue, DateTime.MinValue);

        return booking_id;
    }
    public static void Update(int booking_id, DateTime date_start, DateTime date_end, int organisation_id, int provider, int patient_id, int offering_id, int booking_type_id, int booking_status_id, int booking_unavailability_reason_id, int added_by, int booking_confirmed_by_type_id, int confirmed_by, DateTime date_confirmed, int deleted_by, DateTime date_deleted, int cancelled_by, DateTime date_cancelled, bool is_patient_missed_appt, bool is_provider_missed_appt, bool is_emergency, bool is_recurring, DayOfWeek recurring_weekday, TimeSpan recurring_start_time, TimeSpan recurring_end_time)
    {
        Booking originalBooking = BookingDB.GetByID(booking_id);

        string sql = "UPDATE Booking SET date_start = '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "',date_end = " + (date_end == DateTime.MinValue ? "NULL" : "'" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",organisation_id = " + (organisation_id == -1 ? "null" : organisation_id.ToString()) + ",provider = " + (provider == -1 ? "null" : provider.ToString()) + ",patient_id = " + (patient_id == -1 ? "null" : patient_id.ToString()) + ",offering_id = " + (offering_id == -1 ? "null" : offering_id.ToString()) + ",booking_type_id = " + booking_type_id + ",booking_status_id = " + (booking_status_id == -1 ? "null" : booking_status_id.ToString()) + ",booking_unavailability_reason_id = " + (booking_unavailability_reason_id == -1 ? "null" : booking_unavailability_reason_id.ToString()) + ",added_by = " + added_by + ",booking_confirmed_by_type_id = " + (booking_confirmed_by_type_id == -1 ? "null" : booking_confirmed_by_type_id.ToString()) + ",confirmed_by = " + (confirmed_by == -1 ? "null" : confirmed_by.ToString()) + ",date_confirmed = " + (date_confirmed == DateTime.MinValue ? "null" : "'" + date_confirmed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",deleted_by = " + (deleted_by == -1 ? "null" : deleted_by.ToString()) + ",date_deleted = " + (date_deleted == DateTime.MinValue ? "null" : "'" + date_deleted.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",cancelled_by = " + (cancelled_by == -1 ? "null" : cancelled_by.ToString()) + ",date_cancelled = " + (date_cancelled == DateTime.MinValue ? "null" : "'" + date_cancelled.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",is_patient_missed_appt = " + (is_patient_missed_appt ? "1," : "0,") + "is_provider_missed_appt = " + (is_provider_missed_appt ? "1," : "0,") + "is_emergency = " + (is_emergency ? "1," : "0,") + "is_recurring = " + (is_recurring ? "1," : "0,") + "recurring_weekday_id = " + WeekDayDB.GetWeekDayID(recurring_weekday) + ",recurring_start_time = '" + (recurring_start_time == null ? TimeSpan.Zero.ToString() : recurring_start_time.ToString()) + "',recurring_end_time = '" + (recurring_end_time == null ? TimeSpan.Zero.ToString() : recurring_end_time.ToString()) + "' WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);

        SendPTSelfBookingAlertEmail(booking_id, DBAction.Update, originalBooking.DateStart, originalBooking.DateEnd);
    }
    public static void UpdateSetDeleted(int booking_id, int deleted_by)
    {
        Booking booking = BookingDB.GetByID(booking_id);

        if (booking == null)
            return;

        string sql = "UPDATE Booking SET booking_status_id = -1,deleted_by = " + deleted_by.ToString() + ",date_deleted = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);

        if (booking.Patient != null && BookingDB.GetCountByPatientAndOrg(booking.Patient.PatientID, booking.Organisation.OrganisationID) == 0)
            if (RegisterPatientDB.GetCountByPatientAndOrgTypeGroup(booking.Patient.PatientID, "6") > 1)
                RegisterPatientDB.UpdateInactive(booking.Patient.PatientID, booking.Organisation.OrganisationID, false);


        // send alert email

        if (booking.BookingTypeID == 34)
        {
            string mailText = @"This is an administrative email to notify you that a booking has been deleted.

<u>Logged-in user performing the delete</u>
" + StaffDB.GetByID(deleted_by).Person.FullnameWithoutMiddlename + @"

<u>Booking details</u>
<table border=""0"" cellpadding=""2"" cellspacing=""2""><tr><td>Booking ID:</td><td>" + booking.BookingID + @"</td></tr><tr><td>Booking Date:</td><td>" + booking.DateStart.ToString("d MMM, yyyy") + " " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + @"</td></tr><tr><td>Organisation:</td><td>" + booking.Organisation.Name + @"</td></tr><tr><td>Provider:</td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr><tr><td>Patient:</td><td>" + (booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename + " [ID:" + booking.Patient.PatientID + "]") + @"</td></tr><tr><td>Status:</td><td>" + booking.BookingStatus.Descr + @"</td></tr></table>

Regards,
Mediclinic
";

            bool EnableDeletedBookingsAlerts = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableDeletedBookingsAlerts").Value) == 1;

            if (EnableDeletedBookingsAlerts && !Utilities.IsDev())
                Emailer.AsyncSimpleEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["AdminAlertEmail_To"].Value,
                    "Notification that a booking has been deleted",
                    mailText.Replace(Environment.NewLine, "<br />"),
                    true,
                    null);
        }

        SendPTSelfBookingAlertEmail(booking_id, DBAction.Delete, DateTime.MinValue, DateTime.MinValue);
    }
    public static void UpdateUnsetDeletedByCancelledBy(int booking_id)
    {
        string sql = "UPDATE Booking SET cancelled_by = NULL, date_cancelled = NULL, deleted_by = NULL, date_deleted = NULL  WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateSetConfirmed(int booking_id, int booking_confirmed_by_type_id, int confirmed_by)
    {
        string sql = "UPDATE Booking SET booking_confirmed_by_type_id = " + (booking_confirmed_by_type_id == -1 ? "NULL" : booking_confirmed_by_type_id.ToString()) + ", confirmed_by = " + (confirmed_by == -1 ? "NULL" : confirmed_by.ToString()) + ",date_confirmed = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetUnconfirmed(int booking_id)
    {
        string sql = "UPDATE Booking SET booking_confirmed_by_type_id = NULL, confirmed_by = NULL , date_confirmed = NULL WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetConfirmed(int[] booking_ids, int booking_confirmed_by_type_id, int confirmed_by)
    {
        if (booking_ids.Length == 0)
            return;

        string sql = "UPDATE Booking SET booking_confirmed_by_type_id = " + (booking_confirmed_by_type_id == -1 ? "NULL" : booking_confirmed_by_type_id.ToString()) + ", confirmed_by = " + (confirmed_by == -1 ? "NULL" : confirmed_by.ToString()) + ",date_confirmed = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE booking_id IN (" + string.Join(",", booking_ids) + ")";
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetBookingStatusID(int booking_id, int booking_status_id)
    {
        string sql = "UPDATE Booking SET booking_status_id = " + booking_status_id.ToString() + " WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetCancelledByPatient(int booking_id, int cancelled_by)
    {
        string sql = "UPDATE Booking SET booking_status_id = 188, cancelled_by = " + cancelled_by + ", date_cancelled = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetDeceasedByPatient(int booking_id)
    {
        string sql = "UPDATE Booking SET booking_status_id = 189 WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetGeneratedSystemLetters(int booking_id, bool need_to_generate_first_letter, bool need_to_generate_last_letter, bool has_generated_system_letters)
    {
        string sql = "UPDATE Booking SET need_to_generate_first_letter = " + (need_to_generate_first_letter ? "1," : "0,") + @"
                                         need_to_generate_last_letter  = " + (need_to_generate_last_letter  ? "1," : "0,") + @"
                                         has_generated_system_letters  = " + (has_generated_system_letters  ? "1"  : "0" ) + @"
            
             WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetArrivalTime(int booking_id)
    {
        string sql = "UPDATE Booking SET arrival_time = (GETDATE()) WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void RemoveArrivalTime(int booking_id)
    {
        string sql = "UPDATE Booking SET arrival_time = NULL WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSterilistionCode(int booking_id, string sterilisation_code)
    {
        sterilisation_code = sterilisation_code.Replace("'", "''");
        string sql = "UPDATE Booking SET sterilisation_code = '" + sterilisation_code + "' WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInformedConsent(int booking_id, bool informed_consent, int staff_id)
    {
        string sql = "UPDATE Booking SET informed_consent_added_by = " + (informed_consent ? staff_id.ToString() : "NULL") + ", informed_consent_date = " + (informed_consent ? "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL") + " WHERE booking_id = " + booking_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    
    public static int GetCountByEntityID(int entity_id)
    {
        string sql = "SELECT COUNT(*) FROM Booking WHERE entity_id = " + entity_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static bool Exists(int booking_id)
    {
        string sql = "SELECT COUNT(*) FROM Booking WHERE booking_id = " + booking_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static int GetCountByPatientAndOrg(int patient_id, int organisation_id)
    {
        string sql = "SELECT COUNT(*) FROM Booking WHERE patient_id = " + patient_id + " AND organisation_id = " + organisation_id + " AND deleted_by IS NULL AND date_deleted IS NULL AND booking_type_id = 34";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static int GetCountByProviderAndOrg(int staff_id, int organisation_id, string status_ids_to_include = null)
    {
        string sql = "SELECT COUNT(*) FROM Booking WHERE provider = " + staff_id + " AND organisation_id = " + organisation_id + ((status_ids_to_include != null && status_ids_to_include.Length > 0) ? " AND booking_status_id IN (" + status_ids_to_include + ") " : "") + " AND deleted_by IS NULL AND date_deleted IS NULL AND booking_type_id = 34";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static int GetCountAddedByOrg(int organisation_id, DateTime start, DateTime end)
    {
        string whereClause = string.Empty;

        if (organisation_id != 0)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " organisation_id = " + organisation_id;
        if (start != DateTime.MinValue)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " date_created >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        if (end != DateTime.MinValue)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " date_created <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'";

        whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " booking_type_id = 34 AND (booking_status_id in (0,187,188)) ";

        string sql = "SELECT COUNT(*) FROM Booking " + whereClause;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static DateTime GetLastBookingDate(int patient_id)
    {
        string sql = @"SELECT TOP 1 date_start FROM Booking WHERE patient_id = " + patient_id + @" AND date_start <= '" + DateTime.Today.ToString("yyyy-MM-dd") + @" 23:00:00' ORDER BY date_start DESC";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? DateTime.MinValue : Convert.ToDateTime(tbl.Rows[0]["date_start"]);
    }

    public static Hashtable GetLastBookingDates(int[] patient_ids, int organisation_id = 0)
    {
        if (patient_ids == null || patient_ids.Length == 0)
            return new Hashtable();

        string sql = @"
            SELECT b.patient_id, b.date_start
            FROM Booking b
            INNER JOIN (
                SELECT patient_id, MAX(date_start) AS LastDate
                FROM Booking
                WHERE patient_id IN (" + string.Join(",", patient_ids) + @")
                  AND Booking.booking_status_id not in (-1, 188, 189)
	              AND date_start <= GETDATE()
                  " + (organisation_id == 0 ? "" : " AND organisation_id = " + organisation_id) + @"
                GROUP BY patient_id
            ) tm ON b.patient_id = tm.patient_id AND b.date_start = tm.LastDate";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
            hash[Convert.ToInt32(tbl.Rows[i]["patient_id"])] = Convert.ToDateTime(tbl.Rows[i]["date_start"]);

        return hash;
    }
    public static Hashtable GetNextBookingDates(int[] patient_ids, int organisation_id = 0)
    {
        if (patient_ids == null || patient_ids.Length == 0)
            return new Hashtable();

        string sql = @"
            SELECT b.patient_id, b.date_start
            FROM Booking b
            INNER JOIN (
                SELECT patient_id, MIN(date_start) AS LastDate
                FROM Booking
                WHERE patient_id IN (" + string.Join(",", patient_ids) + @")
                  AND Booking.booking_status_id <> -1
	              AND date_start > GETDATE()
                  " + (organisation_id == 0 ? "" : " AND organisation_id = " + organisation_id) + @"
                GROUP BY patient_id
            ) tm ON b.patient_id = tm.patient_id AND b.date_start = tm.LastDate";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
            hash[Convert.ToInt32(tbl.Rows[i]["patient_id"])] = Convert.ToDateTime(tbl.Rows[i]["date_start"]);

        return hash;
    }



    public static Hashtable GetHashHasMedicareDVA(int[] booking_ids, int patientID = -1)
    {
        if (booking_ids == null || booking_ids.Length == 0)
            booking_ids = new int[] { -1 };

        string sql = @"

                SELECT 
	                Booking.booking_id,
	                CASE WHEN EXISTS (SELECT invoice_id FROM Invoice WHERE " + (patientID == -1 ? "1=1" : " (Booking.patient_id = " + patientID + @" OR (SELECT count(invoice_line_id) FROM InvoiceLine WHERE InvoiceLine.invoice_id = Invoice.invoice_id AND InvoiceLine.patient_id = " + patientID + @") > 0) ") + @" AND Invoice.booking_id = Booking.booking_id AND payer_organisation_id = -1 AND (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE CreditNote.invoice_id = Invoice.invoice_id) < Invoice.total) THEN '1' ELSE '0' END AS has_medicare,
	                CASE WHEN EXISTS (SELECT invoice_id FROM Invoice WHERE " + (patientID == -1 ? "1=1" : " (Booking.patient_id = " + patientID + @" OR (SELECT count(invoice_line_id) FROM InvoiceLine WHERE InvoiceLine.invoice_id = Invoice.invoice_id AND InvoiceLine.patient_id = " + patientID + @") > 0) ") + @" AND Invoice.booking_id = Booking.booking_id AND payer_organisation_id = -2 AND (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE CreditNote.invoice_id = Invoice.invoice_id) < Invoice.total) THEN '1' ELSE '0' END AS has_dva
                FROM Booking
                WHERE booking_id IN (" + string.Join(",", booking_ids) + @")";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int  booking_id   = Convert.ToInt32(tbl.Rows[i]["booking_id"]);
            bool has_medicare = tbl.Rows[i]["has_medicare"].ToString() == "1";
            bool has_dva      = tbl.Rows[i]["has_dva"].ToString()      == "1";

            hash[new Hashtable2D.Key(booking_id, -1)] = has_medicare;
            hash[new Hashtable2D.Key(booking_id, -2)] = has_dva;
        }

        return hash;
    }

    public static IDandDescr GetStatusByID(int booking_status_id)
    {
        string sql = "SELECT booking_status_id,descr from BookingStatus WHERE booking_status_id = " + booking_status_id;
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return tbl.Rows.Count == 0 ? null : IDandDescrDB.Load(tbl.Rows[0], "booking_status_id", "descr");
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT booking_id,entity_id,date_start,date_end,organisation_id,provider,patient_id,offering_id,booking_type_id,booking_status_id, booking_unavailability_reason_id,added_by,date_created,booking_confirmed_by_type_id,confirmed_by,date_confirmed,deleted_by,date_deleted,cancelled_by,date_cancelled,is_patient_missed_appt,is_provider_missed_appt,is_emergency FROM Booking";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Booking GetByID(int booking_id)
    {
        string sql = JoinedSQL(true, true, true, true) + " AND booking_id = " + booking_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadFull(tbl.Rows[0]);
    }
    public static Booking GetByEntityID(int entity_id, string DB = null)
    {
        string sql = JoinedSQL(true, true, true, true) + " AND booking.entity_id = " + entity_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql, DB).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadFull(tbl.Rows[0]);
    }


    public static Booking GetMostRecent(int patient_id)
    {
        string sql = JoinedSQL(true, true, true) + " AND (patient.patient_id = " + patient_id + ") AND booking.date_start <= (GETDATE()) AND booking.booking_status_id in (0,187)  ORDER BY booking.date_start DESC";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Booking booking = (tbl.Rows.Count == 0) ? null : LoadFull(tbl.Rows[0]);

        string sql2 = @"
            SELECT   TOP 1 *
            FROM     BookingPatient 
                     LEFT JOIN Booking ON BookingPatient.booking_id = Booking.booking_id
            WHERE    BookingPatient.patient_id = " + patient_id + @" 
	                 AND Booking.deleted_by IS NULL AND date_deleted IS NULL AND booking.booking_type_id = 34 AND (booking.booking_status_id IS NULL OR booking.booking_status_id in (0,187))
	                 AND Booking.date_start <= (GETDATE()) 
            ORDER BY Booking.date_start DESC";
        DataTable tbl2 = DBBase.ExecuteQuery(sql2).Tables[0];
        Booking booking2 = (tbl2.Rows.Count == 0) ? null : BookingDB.GetByID(Convert.ToInt32(tbl2.Rows[0]["booking_id"]));

        if      (booking == null && booking2 == null) return null;
        else if (booking != null && booking2 == null) return booking;
        else if (booking == null && booking2 != null) return booking2;
        else                                          return booking.DateStart > booking2.DateStart ? booking : booking2;
    }
    public static Booking GetNextAfterNow(int patient_id, bool incCompleted = true)
    {
        string sql = JoinedSQL(true, true, true) + " AND (patient.patient_id = " + patient_id + ") AND booking.date_start > (GETDATE()) AND booking.booking_status_id in (0" + (incCompleted ? ",187" : "") + ")  ORDER BY booking.date_start ASC";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Booking booking = (tbl.Rows.Count == 0) ? null : LoadFull(tbl.Rows[0]);

        string sql2 = @"
            SELECT   TOP 1 * 
            FROM     BookingPatient LEFT JOIN Booking ON BookingPatient.booking_id = Booking.booking_id
            WHERE    BookingPatient.patient_id = " + patient_id + @" 
	                 AND Booking.deleted_by IS NULL AND date_deleted IS NULL AND booking.booking_type_id = 34 AND (booking.booking_status_id IS NULL OR booking.booking_status_id in (0,187))
	                 AND Booking.date_start > (GETDATE()) 
            ORDER BY Booking.date_start ASC";
        DataTable tbl2 = DBBase.ExecuteQuery(sql2).Tables[0];
        Booking booking2 = (tbl2.Rows.Count == 0) ? null : BookingDB.GetByID(Convert.ToInt32(tbl2.Rows[0]["booking_id"]));

        if      (booking == null && booking2 == null) return null;
        else if (booking != null && booking2 == null) return booking;
        else if (booking == null && booking2 != null) return booking2;
        else                                          return booking.DateStart < booking2.DateStart ? booking : booking2;
    }
    public static Booking GetNextAfterToday(int patient_id, bool incCompleted = true)
    {
        string sql = JoinedSQL(true, true, true) + " AND (patient.patient_id = " + patient_id + ") AND booking.date_start > (CONVERT (date, GETDATE())) AND booking.booking_status_id in (0" + (incCompleted ? ",187" : "") + ")  ORDER BY booking.date_start ASC";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Booking booking = (tbl.Rows.Count == 0) ? null : LoadFull(tbl.Rows[0]);

        string sql2 = @"
            SELECT   TOP 1 * 
            FROM     BookingPatient LEFT JOIN Booking ON BookingPatient.booking_id = Booking.booking_id
            WHERE    BookingPatient.patient_id = " + patient_id + @" 
	                 AND Booking.deleted_by IS NULL AND date_deleted IS NULL AND booking.booking_type_id = 34 AND (booking.booking_status_id IS NULL OR booking.booking_status_id in (0,187))
	                 AND Booking.date_start > (CONVERT (date, GETDATE()))
            ORDER BY Booking.date_start ASC";
        DataTable tbl2 = DBBase.ExecuteQuery(sql2).Tables[0];
        Booking booking2 = (tbl2.Rows.Count == 0) ? null : BookingDB.GetByID(Convert.ToInt32(tbl2.Rows[0]["booking_id"]));

        if      (booking == null && booking2 == null) return null;
        else if (booking != null && booking2 == null) return booking;
        else if (booking == null && booking2 != null) return booking2;
        else                                          return booking.DateStart < booking2.DateStart ? booking : booking2;
    }


    public static DataTable GetDataTable_Between(DateTime date_start, DateTime date_end, Staff[] staff, Organisation[] organisations, Patient patient, Staff added_by = null, bool incDeleted = false, string statusIDsToInclude = null, bool onlyUnavailabilities = false, string booking_id_serach = "")
    {
        return GetDataTable_Between(false, date_start, date_end, staff, organisations, patient, added_by, incDeleted, statusIDsToInclude, onlyUnavailabilities, booking_id_serach);
    }
    public static DataTable GetDataTable_Between(bool debugPageLoadTime, DateTime date_start, DateTime date_end, Staff[] staff, Organisation[] organisations, Patient patient, Staff added_by = null, bool incDeleted = false, string statusIDsToInclude = null, bool onlyUnavailabilities = false, string booking_id_serach = "")
    {
        int[] orgIDList = new int[organisations == null ? 0 : organisations.Length];
        if (organisations != null)
            for (int i = 0; i < organisations.Length; i++)
                orgIDList[i] = organisations[i].OrganisationID;
        string org_ids = string.Join(",", orgIDList);

        int[] staffIDList = new int[staff == null ? 0 : staff.Length];
        if (staff != null)
            for (int i = 0; i < staff.Length; i++)
                staffIDList[i] = staff[i].StaffID;
        string staff_ids = string.Join(",", staffIDList);


        string condition = string.Empty;

        // will get rows with EITHER org or staff
        if (staff != null && staff.Length > 0 && organisations != null && organisations.Length > 0)
            condition = " AND (booking.provider IN (" + (staff_ids.Length > 0 ? staff_ids : "-1") + ") OR booking.organisation_id IN (" + org_ids + ") OR booking.organisation_id IS NULL ) ";
        else if (staff != null && staff.Length > 0)
            condition = " AND booking.provider IN (" + staff_ids + ") ";
        else if (organisations != null && organisations.Length > 0)
            condition = " AND (booking.organisation_id IN (" + org_ids + ") OR booking.organisation_id IS NULL) ";


        if (onlyUnavailabilities)
        {
            condition += " AND (booking.booking_type_id IN (340, 341, 342)) ";
        }
        else
        {
            // dont include unavailable days for other orgs, so ... get where - normal booking (34) or (this org or blank org)
            if (organisations != null && organisations.Length > 0)
                condition += " AND (booking.booking_type_id = 34 OR booking.organisation_id IN (" + org_ids + ") OR booking.organisation_id IS NULL) ";
        }


        // will get ONLY rows by patient
        if (patient != null)
        {
            // get completed bookings where there are multiple patients .. get patients from invoice lines
            string acBkIDs = string.Empty;
            string ac_sql = @"
                SELECT Booking.booking_id
                FROM   InvoiceLine 
                        LEFT JOIN Invoice ON InvoiceLine.invoice_id = Invoice.invoice_id 
                        LEFT JOIN Booking ON Booking.booking_id = Invoice.booking_id
                WHERE  InvoiceLine.patient_id = " + patient.PatientID;
            DataTable ac_tbl = DBBase.ExecuteQuery(ac_sql).Tables[0];
            for (int i = 0; i < ac_tbl.Rows.Count; i++)
                if (ac_tbl.Rows[i]["booking_id"] != DBNull.Value)
                    acBkIDs += (acBkIDs.Length == 0 ? "" : ",") + Convert.ToInt32(ac_tbl.Rows[i]["booking_id"]);


            // get incomplete group bookings where there are multiple patients .. get patients from bookingpatient table
            string groupBkIDs = string.Empty;
            string group_sql = @"
                SELECT DISTINCT BookingPatient.booking_id
                FROM BookingPatient 
                    LEFT JOIN Booking ON BookingPatient.booking_id = Booking.booking_id
                    LEFT JOIN Organisation ON Booking.organisation_id = Organisation.organisation_id
                WHERE BookingPatient.is_deleted = 0 AND Booking.booking_status_id in (0) AND BookingPatient.patient_id = " + patient.PatientID + @"
                    AND Organisation.organisation_type_id = 218";
            DataTable group_tbl = DBBase.ExecuteQuery(group_sql).Tables[0];
            for (int i = 0; i < group_tbl.Rows.Count; i++)
                if (group_tbl.Rows[i]["booking_id"] != DBNull.Value)
                    groupBkIDs += (groupBkIDs.Length == 0 ? "" : ",") + Convert.ToInt32(group_tbl.Rows[i]["booking_id"]);

            //condition += " AND (patient.patient_id = " + patient.PatientID + " OR (SELECT COUNT(*) FROM BookingPatient WHERE BookingPatient.booking_id = booking.booking_id AND BookingPatient.patient_id = " + patient.PatientID + ") > 0) ";
            condition += " AND (patient.patient_id = " + patient.PatientID + @"
                                    " + @"--OR EXISTS (SELECT 1 FROM InvoiceLine LEFT JOIN Invoice ON InvoiceLine.invoice_id = Invoice.invoice_id WHERE Invoice.booking_id = booking.booking_id AND InvoiceLine.patient_id = " + patient.PatientID + @")
                                    " + (acBkIDs.Length    > 0 ? " OR booking.booking_id IN (" + acBkIDs    + @") " : "") + @"
                                    " + (groupBkIDs.Length > 0 ? " OR booking.booking_id IN (" + groupBkIDs + @") " : "") + @"
                                    ) ";
       
        }


        if (added_by != null)
            condition += " AND booking.added_by = " + added_by.StaffID;


        if (statusIDsToInclude != null && statusIDsToInclude.Length > 0)
            condition += " AND booking.booking_status_id IN (" + statusIDsToInclude + ") ";

        if (booking_id_serach != null && booking_id_serach.Length > 0)
            condition += " AND (CONVERT(varchar, booking.booking_id) LIKE '%" + booking_id_serach + "%') ";


        string recurring_condition     = string.Empty;
        string non_recurring_condition = string.Empty;
        if (added_by != null) // if searching by who added, also search by the date added
        {
            if (date_start != DateTime.MinValue && date_end != DateTime.MinValue)
                condition += " AND (  booking.date_created > '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + (date_end == DateTime.MinValue ? "" : " AND booking.date_created < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"";
            else if (date_start != DateTime.MinValue)
                condition += " AND (  booking.date_created > '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
            else if (date_end != DateTime.MinValue)
                condition += " AND (  booking.date_created < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
            else
                condition += "";
        }
        else // else search by booking date
        {
            if (date_start != DateTime.MinValue && date_end != DateTime.MinValue)
            {
                recurring_condition     = " AND (  date_start >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "' AND date_end <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
                non_recurring_condition = " AND (  date_end IS NULL OR date_end >= '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + (date_end == DateTime.MinValue ? "" : " AND date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"";
            }
            else if (date_start != DateTime.MinValue)
            {
                recurring_condition     = " AND (  date_start >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                non_recurring_condition = " AND (  date_end IS NULL OR date_end >= '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + @"";
            }
            else if (date_end != DateTime.MinValue)
            {
                recurring_condition     = " AND (  date_end  <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
                non_recurring_condition = " AND (  date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
            }
            else
            {
                recurring_condition     = "";
                non_recurring_condition = "";
            }
        }


        /*
        string sql = JoinedSQL() + @" AND date_deleted is null 
                                      AND (
                                             (is_recurring = 0 AND (  date_start >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "' AND date_end <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + @"' )) OR  
                                             (is_recurring = 1 AND (  date_end IS NULL OR date_end >= '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + (date_end == DateTime.MinValue ? "" : " AND date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "'") + @") 
                                          ) " + condition + @"
                       ORDER BY 
                            booking_type_id, date_start, date_end";
        */


        string sql = JoinedSQL(false, false, false, incDeleted) + @"
                                      AND (
                                             (is_recurring = 0 " + recurring_condition     + @") OR  
                                             (is_recurring = 1 " + non_recurring_condition + @") 
                                          ) " + condition + @"
                       --ORDER BY 
                       --     booking_type_id DESC, date_start, date_end";

        if (debugPageLoadTime)
            Logger.LogQuery("ExecuteQuery___Start", false, false, true);
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];


        // order by here, it is much faster
        tbl.DefaultView.Sort = "booking_booking_type_id DESC, booking_date_start, booking_date_end";
        tbl = tbl.DefaultView.ToTable();



        // now put in the note count and invoice count seperately -- it makes the query 150ms instead of 500ms!

        string entityIDs  = "0"; // to make sure there is at least one
        string bookingIDs = "0"; // to make sure there is at least one
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            bookingIDs += (bookingIDs.Length == 0 ? "" : ",") + tbl.Rows[i]["booking_booking_id"];
            entityIDs  += (entityIDs.Length  == 0 ? "" : ",") + tbl.Rows[i]["booking_entity_id"];
        }

        DataTable tblNoteCounts    = DBBase.ExecuteQuery(@"SELECT entity_id,  COUNT(entity_id)  as count FROM Note    WHERE entity_id IN (" + entityIDs + @") GROUP BY entity_id").Tables[0];
        DataTable tblInvoiceCounts = DBBase.ExecuteQuery(@"SELECT booking_id, COUNT(booking_id) as count FROM Invoice WHERE reversed_by IS NULL AND booking_id IN (" + bookingIDs + @") GROUP BY booking_id").Tables[0];

        Hashtable noteCountHash = new Hashtable();
        Hashtable invCountHash  = new Hashtable();
        for (int i = 0; i < tblNoteCounts.Rows.Count; i++)
            noteCountHash[Convert.ToInt32(tblNoteCounts.Rows[i]["entity_id"])] = Convert.ToInt32(tblNoteCounts.Rows[i]["count"]);
        for (int i = 0; i < tblInvoiceCounts.Rows.Count; i++)
            invCountHash[Convert.ToInt32(tblInvoiceCounts.Rows[i]["booking_id"])] = Convert.ToInt32(tblInvoiceCounts.Rows[i]["count"]);

        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int booking_id = Convert.ToInt32(tbl.Rows[i]["booking_booking_id"]);
            int entity_id  = Convert.ToInt32(tbl.Rows[i]["booking_entity_id"]);

            if (noteCountHash[entity_id] != null)
                tbl.Rows[i]["booking_note_count"] = noteCountHash[entity_id];

            if (invCountHash[booking_id] != null)
                tbl.Rows[i]["booking_inv_count"] = invCountHash[booking_id];
        }


        if (debugPageLoadTime)
            Logger.LogQuery("ExecuteQuery___End", false, false, true);
        return tbl;
    }
    public static Booking[] GetBetween(DateTime date_start, DateTime date_end, Staff[] staff, Organisation[] organisations, Patient patient, Staff added_by, bool incDeleted = false, string statusIDsToInclude = null, bool onlyUnavailabilities = false, string booking_id_serach = "")
    {
        return GetBetween(false, date_start, date_end, staff, organisations, patient, added_by, incDeleted, statusIDsToInclude, onlyUnavailabilities, booking_id_serach);
    }
    public static Booking[] GetBetween(bool debugPageLoadTime, DateTime date_start, DateTime date_end, Staff[] staff, Organisation[] organisations, Patient patient, Staff added_by, bool incDeleted = false, string statusIDsToInclude = null, bool onlyUnavailabilities = false, string booking_id_serach = "")
    {
        DataTable tbl = GetDataTable_Between(debugPageLoadTime, date_start, date_end, staff, organisations, patient, added_by, incDeleted, statusIDsToInclude, onlyUnavailabilities, booking_id_serach);

        Booking[] bookings = new Booking[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bookings[i] = LoadFull(tbl.Rows[i]);

        return bookings;
    }

    // to check provider and any org, just levae org as null....
    public static Booking[] GetToCheckOverlap_OneTime(DateTime date_start, DateTime date_end, Staff staff, Organisation organisation, bool onlyThisOrg, bool incNullOrgs, bool includeUnavailableDays, Booking[] bookingsToIgnore = null)
    {
        string condition = string.Empty;
        if (staff != null && organisation != null)
        {
            if (onlyThisOrg)
                condition += " AND (provider = " + staff.StaffID + " AND (booking.organisation_id = " + organisation.OrganisationID + (incNullOrgs ? " OR booking.organisation_id IS NULL" : "") +")) ";
            else
                condition += " AND (provider = " + staff.StaffID + " OR  booking.organisation_id = " + organisation.OrganisationID + ") ";
        }
        else if (staff != null)
            condition += " AND provider = " + staff.StaffID + " ";
        else if (organisation != null)
            condition += " AND booking.organisation_id = " + organisation.OrganisationID + " ";

        // dont include unavailable days for other orgs, so ... get where - normal booking (34) or (this org or blank org)
        if (organisation != null)
            condition += " AND (booking.booking_type_id = 34 OR booking.organisation_id = " + organisation.OrganisationID + (incNullOrgs ? " OR booking.organisation_id IS NULL" : "") + ") ";

        if (!includeUnavailableDays)
            condition += " AND booking.booking_type_id = 34 ";



        // and get only for these weekdays
        string weekdayCondition = string.Empty;
        int weekday_id = WeekDayDB.GetWeekDayID(date_start.DayOfWeek);
        string onetimeCondition   = " (is_recurring = 0 AND ( datepart(dw,date_start) = " + weekday_id + " ))";
        string recurringCondition = " (is_recurring = 1 AND ( recurring_weekday_id    = " + weekday_id + " ))";
        weekdayCondition = " AND ( " + onetimeCondition + " OR " + recurringCondition + ") ";
        condition += weekdayCondition;


        string sql = JoinedSQL() + @" AND date_deleted is null AND date_end > '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "' AND date_start < '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + "' " + condition + @"
                       ORDER BY 
                            date_start, date_end";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];


        //Booking[] bookings = new Booking[tbl.Rows.Count];
        //for (int i = 0; i < tbl.Rows.Count; i++)
        //    bookings[i] = LoadFull(tbl.Rows[i]);
        //
        //return bookings;


        ArrayList bookings = new ArrayList();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Booking b = LoadFull(tbl.Rows[i]);
            bool ignore = false;
            if (bookingsToIgnore != null)
            {
                foreach (Booking bookingToIgnore in bookingsToIgnore)
                    if (b.BookingID == bookingToIgnore.BookingID)
                        ignore = true;
            }

            if (!ignore)
                bookings.Add(b);
        }

        return (Booking[])bookings.ToArray(typeof(Booking));
    }

    public static Booking[] GetToCheckOverlap_Recurring(DateTime date_start, DateTime date_end, TimeSpan recurring_start_time, TimeSpan recurring_end_time, string recurring_weekdays, Staff staff, Organisation organisation, bool onlyThisOrg, bool incNullOrgs, bool includeUnavailableDays, bool includeCustomerBookings)
    {
        string condition = string.Empty;
        if (staff != null && organisation != null)
        {
            if (onlyThisOrg)
                condition += " AND (provider = " + staff.StaffID + " AND (booking.organisation_id = " + organisation.OrganisationID + (incNullOrgs ? " OR booking.organisation_id IS NULL" : "") + ")) ";
            else
                condition += " AND provider = " + staff.StaffID + " ";
        }
        else if (staff != null)
            condition += " AND provider = " + staff.StaffID + " ";
        else if (organisation != null)
            condition += " AND booking.organisation_id = " + organisation.OrganisationID + " ";

        // dont include unavailable days for other orgs, so ... get where - normal booking (34) or (this org or blank org)
        if (organisation != null)
            condition += " AND (booking.booking_type_id = 34 OR booking.organisation_id = " + organisation.OrganisationID + (incNullOrgs ? " OR booking.organisation_id IS NULL" : "") + ") ";

        if (!includeUnavailableDays)
            condition += " AND booking.booking_type_id = 34 ";

        if (!includeCustomerBookings)
            condition += " AND booking.booking_type_id <> 34 ";



        // and get only for these weekdays
        string weekdayCondition = string.Empty;
        ArrayList weedayConditionsOneTimeBookings   = new ArrayList();
        ArrayList weedayConditionsRecurringBookings = new ArrayList();
        for (int i = 0; i < 7; i++)
        {
            if (recurring_weekdays[i] != '1')
                continue;
            weedayConditionsOneTimeBookings.Add(" datepart(dw,date_start) = " + (i + 1) + " ");
            weedayConditionsRecurringBookings.Add(" recurring_weekday_id = " + (i + 1) + " ");
        }
        if (weedayConditionsOneTimeBookings.Count > 0)
        {
            string onetimeCondition   = " (is_recurring = 0 AND (" + string.Join(" OR ", weedayConditionsOneTimeBookings.ToArray())   + "))";
            string recurringCondition = " (is_recurring = 1 AND (" + string.Join(" OR ", weedayConditionsRecurringBookings.ToArray()) + "))";
            weekdayCondition          = " AND ( " + onetimeCondition + " OR " + recurringCondition + ") ";
        }
        condition += weekdayCondition;


        string sql = JoinedSQL() + @" AND date_deleted is null AND (date_end IS NULL OR date_end > '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + (date_end == DateTime.MinValue ? "" : " AND date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "'") + condition + @"
                       ORDER BY 
                            date_start, date_end";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Booking[] bookings = new Booking[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bookings[i] = LoadFull(tbl.Rows[i]);

        return bookings;
    }


    public static Booking[] GetUnavailableDaysByStartEndDate(DateTime date_start, DateTime date_end, Staff staff, Organisation organisation)
    {
        string condition = string.Empty;
        if (staff != null && organisation != null)
            condition += " AND (provider = " + staff.StaffID + " OR booking.organisation_id = " + organisation.OrganisationID + ") ";
        else if (staff != null)
            condition += " AND provider = " + staff.StaffID + " ";
        else if (organisation != null)
            condition += " AND booking.organisation_id = " + organisation.OrganisationID + " ";

        condition += " AND booking.booking_type_id <> 34 ";


        string sql = JoinedSQL() + @" AND date_deleted is null AND date_start = '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "' AND date_end = '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + "' " + condition + @"
                       ORDER BY 
                            date_start, date_end";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Booking[] bookings = new Booking[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bookings[i] = LoadFull(tbl.Rows[i]);

        return bookings;
    }

    // --------------------------------------------------------------------------------
    // FIND REFERENCES OF THIS .. SEE IF NEED TO UPDATE TO INCLUDE RECURRING BOOKINGS!!
    // same for GetMostRecent, and GetNext
    // --------------------------------------------------------------------------------
    public static Booking[] GetFutureBookings(Staff staff, Organisation organisation, string weekday, TimeSpan startTime, TimeSpan endTime, bool outsideThisTime = false)
    {
        weekday = weekday.Substring(0, 1).ToUpper() + weekday.Substring(1).ToLower();
        string start = startTime.Hours.ToString().PadLeft(2, '0') + ":" + startTime.Minutes.ToString().PadLeft(2, '0') + ":00";
        string end   = endTime.Hours.ToString().PadLeft(2, '0')   + ":" + endTime.Minutes.ToString().PadLeft(2, '0')   + ":00";


        string condition = string.Empty;
        if (staff != null && organisation != null)
            condition += " AND (provider = " + staff.StaffID + " OR booking.organisation_id = " + organisation.OrganisationID + ") ";
        else if (staff != null)
            condition += " AND provider = " + staff.StaffID + " ";
        else if (organisation != null)
            condition += " AND booking.organisation_id = " + organisation.OrganisationID + " ";

        string sql = JoinedSQL() + @" AND date_deleted is null AND date_start >= GETDATE()
                AND convert(varchar, date_end, 108) > '" + start + @"' 
                AND convert(varchar, date_start, 108) < '" + end + @"' " + condition + @" AND booking.booking_type_id = 34 AND  (DATENAME(dw, date_start) = '"+weekday+@"')
                       ORDER BY 
                            date_start, date_end";

        if (outsideThisTime)
            sql = JoinedSQL() + @" AND date_deleted is null AND date_start >= GETDATE()
                AND (convert(varchar, date_start, 108) <  '" + start + @"' OR convert(varchar, date_start, 108) >  '" + end + @"') " +
                condition + @" AND booking.booking_type_id = 34 AND  (DATENAME(dw, date_start) = '" + weekday + @"')
                       ORDER BY 
                            date_start, date_end";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Booking[] bookings = new Booking[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bookings[i] = LoadFull(tbl.Rows[i]);

        return bookings;
    }


    public static DataTable GetUnavailableProvBookingsBetween(int[] orgIDs, DateTime startDate, DateTime endDate)
    {
        if (orgIDs == null || orgIDs.Length == 0)
            orgIDs = new int[] { 0 };

        string sql = @"

                         SELECT *, 
                                0 as note_count,
                                0 as inv_count

                         FROM 
                                Booking 
                         WHERE

                                (date_deleted IS  NULL) 

                                AND

                                (organisation_id IS NULL OR organisation_id IN (" + string.Join(",", orgIDs) + @")) 

                                AND

                                (
                                (booking_type_id = 341   AND ( (is_recurring = 0 AND CONVERT(TIME,Booking.date_start) = '00:00' AND CONVERT(TIME,Booking.date_end) >= '23:59') OR (is_recurring = 1 AND recurring_start_time = '00:00' AND recurring_end_time >= '23:59')   )) OR
                                (booking_type_id = 342   AND ( (is_recurring = 0 AND CONVERT(TIME,Booking.date_start) = '00:00' AND CONVERT(TIME,Booking.date_end) >= '23:59') OR (is_recurring = 1 AND recurring_start_time = '00:00' AND recurring_end_time >= '23:59')   ))
                                ) 

                                AND

                                (
                                    (SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, Booking.date_start))) <= '" + endDate.ToString("yyyy-MM-dd")   + " 00:00:00" + @"'
                                    OR
                                    (SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, Booking.date_end)))   >= '" + startDate.ToString("yyyy-MM-dd") + " 00:00:00" + @"'
                                )

                    ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static string JoinedSQL(bool incNoteCount = true, bool incInvoiceCount = true,  bool onlyTopRow = false, bool incDeleted = false)
    {

        return @"SELECT " + (onlyTopRow ? " top 1 " : "") + @"
                            booking.booking_id as booking_booking_id,booking.entity_id as booking_entity_id,
                            booking.date_start as booking_date_start,booking.date_end as booking_date_end,booking.organisation_id as booking_organisation_id,
                            booking.provider as booking_provider,booking.patient_id as booking_patient_id,booking.offering_id as booking_offering_id,booking.booking_type_id as booking_booking_type_id,
                            booking.booking_status_id as booking_booking_status_id,booking.booking_unavailability_reason_id as booking_booking_unavailability_reason_id,booking.added_by as booking_added_by,booking.date_created as booking_date_created,
                            booking.booking_confirmed_by_type_id as booking_booking_confirmed_by_type_id,booking.confirmed_by as booking_confirmed_by,booking.date_confirmed as booking_date_confirmed,
                            booking.deleted_by as booking_deleted_by, booking.date_deleted as booking_date_deleted, booking.cancelled_by as booking_cancelled_by, booking.date_cancelled as booking_date_cancelled,
                            booking.is_patient_missed_appt as booking_is_patient_missed_appt,booking.is_provider_missed_appt as booking_is_provider_missed_appt,
                            booking.is_emergency as booking_is_emergency,
                            booking.need_to_generate_first_letter as booking_need_to_generate_first_letter,booking.need_to_generate_last_letter as booking_need_to_generate_last_letter,booking.has_generated_system_letters as booking_has_generated_system_letters,
                            booking.arrival_time              as booking_arrival_time,
                            booking.sterilisation_code        as booking_sterilisation_code,
                            booking.informed_consent_added_by as booking_informed_consent_added_by, 
                            booking.informed_consent_date     as booking_informed_consent_date,
                            booking.is_recurring as booking_is_recurring,booking.recurring_weekday_id as booking_recurring_weekday_id,
                            booking.recurring_start_time as booking_recurring_start_time,booking.recurring_end_time as booking_recurring_end_time
                            ,
                            booking_status.booking_status_id as booking_status_booking_status_id,
                            booking_status.descr as booking_status_descr
                            ,
                            booking_confirmed_by_type.booking_confirmed_by_type_id as booking_confirmed_by_type_booking_confirmed_by_type_id,
                            booking_confirmed_by_type.descr as booking_confirmed_by_type_descr
                            ,
                            booking_unavailability_reason.booking_unavailability_reason_id as booking_unavailability_reason_booking_unavailability_reason_id,
                            booking_unavailability_reason.descr as booking_unavailability_reason_descr
                            ,
                            " + (incNoteCount ? @"(SELECT COUNT(*) FROM Note note WHERE note.entity_id = booking.entity_id)" : "0") + @" AS booking_note_count
                            ,
                            " + (incInvoiceCount ? @"(SELECT COUNT(*) FROM Invoice inv WHERE inv.reversed_by IS NULL AND inv.booking_id = booking.booking_id)" : "0") + @" AS booking_inv_count
                            ,
                            org.organisation_id as organisation_organisation_id, org.entity_id as organisation_entity_id, org.parent_organisation_id as organisation_parent_organisation_id, org.use_parent_offernig_prices as organisation_use_parent_offernig_prices, 
                            org.organisation_type_id as organisation_organisation_type_id, org.organisation_customer_type_id as organisation_organisation_customer_type_id, org.name as organisation_name, org.acn as organisation_acn, org.abn as organisation_abn, 
                            org.organisation_date_added as organisation_organisation_date_added, org.organisation_date_modified as organisation_organisation_date_modified, org.is_debtor as organisation_is_debtor, org.is_creditor as organisation_is_creditor, 
                            org.bpay_account as organisation_bpay_account, org.weeks_per_service_cycle as organisation_weeks_per_service_cycle, org.start_date as organisation_start_date, 
                            org.end_date as organisation_end_date, org.comment as organisation_comment, org.free_services as organisation_free_services, org.excl_sun as organisation_excl_sun, 
                            org.excl_mon as organisation_excl_mon, org.excl_tue as organisation_excl_tue, org.excl_wed as organisation_excl_wed, org.excl_thu as organisation_excl_thu, 
                            org.excl_fri as organisation_excl_fri, org.excl_sat as organisation_excl_sat, 
                            org.sun_start_time as organisation_sun_start_time, org.sun_end_time as organisation_sun_end_time, 
                            org.mon_start_time as organisation_mon_start_time, org.mon_end_time as organisation_mon_end_time, org.tue_start_time as organisation_tue_start_time, 
                            org.tue_end_time as organisation_tue_end_time, org.wed_start_time as organisation_wed_start_time, org.wed_end_time as organisation_wed_end_time, 
                            org.thu_start_time as organisation_thu_start_time, org.thu_end_time as organisation_thu_end_time, org.fri_start_time as organisation_fri_start_time, 
                            org.fri_end_time as organisation_fri_end_time, org.sat_start_time as organisation_sat_start_time, org.sat_end_time as organisation_sat_end_time, 
                            org.sun_lunch_start_time as organisation_sun_lunch_start_time, org.sun_lunch_end_time as organisation_sun_lunch_end_time, 
                            org.mon_lunch_start_time as organisation_mon_lunch_start_time, org.mon_lunch_end_time as organisation_mon_lunch_end_time, org.tue_lunch_start_time as organisation_tue_lunch_start_time, 
                            org.tue_lunch_end_time as organisation_tue_lunch_end_time, org.wed_lunch_start_time as organisation_wed_lunch_start_time, org.wed_lunch_end_time as organisation_wed_lunch_end_time, 
                            org.thu_lunch_start_time as organisation_thu_lunch_start_time, org.thu_lunch_end_time as organisation_thu_lunch_end_time, org.fri_lunch_start_time as organisation_fri_lunch_start_time, 
                            org.fri_lunch_end_time as organisation_fri_lunch_end_time, org.sat_lunch_start_time as organisation_sat_lunch_start_time, org.sat_lunch_end_time as organisation_sat_lunch_end_time, 
                            org.last_batch_run as organisation_last_batch_run, org.is_deleted as organisation_is_deleted
                            ,
                            org_type.organisation_type_id as org_type_organisation_type_id,org_type.descr as org_type_descr,org_type.organisation_type_group_id as org_type_organisation_type_group_id
                            ,
                            typegroup.organisation_type_group_id as typegroup_organisation_type_group_id, typegroup.descr as typegroup_descr
                            ,
                            provider.staff_id as provider_staff_id, provider.person_id as provider_person_id, provider.login as provider_login, provider.pwd as provider_pwd, 
                            provider.staff_position_id as provider_staff_position_id, provider.field_id as provider_field_id, provider.costcentre_id as provider_costcentre_id, 
                            provider.is_contractor as provider_is_contractor, provider.tfn as provider_tfn, provider.provider_number as provider_provider_number, 
                            provider.is_fired as provider_is_fired, provider.is_commission as provider_is_commission, provider.commission_percent as provider_commission_percent, 
                            provider.is_stakeholder as provider_is_stakeholder, provider.is_master_admin as provider_is_master_admin, provider.is_admin as provider_is_admin, provider.is_principal as provider_is_principal, provider.is_provider as provider_is_provider, provider.is_external as provider_is_external,
                            provider.staff_date_added as provider_staff_date_added, provider.start_date as provider_start_date, provider.end_date as provider_end_date, provider.comment as provider_comment, 
                            provider.num_days_to_display_on_booking_screen as provider_num_days_to_display_on_booking_screen,  provider.show_header_on_booking_screen as provider_show_header_on_booking_screen,
                            provider.bk_screen_field_id as provider_bk_screen_field_id, provider.bk_screen_show_key as provider_bk_screen_show_key, provider.enable_daily_reminder_sms as provider_enable_daily_reminder_sms, provider.enable_daily_reminder_email as provider_enable_daily_reminder_email
                            ,
                            " + PersonDB.GetFields("person_provider_", "person_provider") + @"
                            ,
                            title_provider.title_id as title_provider_title_id, title_provider.descr as title_provider_descr
                            ,
                            patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, 
                            patient.is_clinic_patient as patient_is_clinic_patient, patient.is_gp_patient as patient_is_gp_patient,
                            patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                            patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                            patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn, 
                            patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info
                            ,
                            " + PersonDB.GetFields("person_patient_", "person_patient") + @"
                            ,
                            title_patient.title_id as title_patient_title_id, title_patient.descr as title_patient_descr
                            ,
                            offering.offering_id as offering_offering_id, offering.offering_type_id as offering_offering_type_id, offering.field_id as offering_field_id,
                            offering.aged_care_patient_type_id as offering_aged_care_patient_type_id, 
                            offering.num_clinic_visits_allowed_per_year as offering_num_clinic_visits_allowed_per_year, 
                            offering.offering_invoice_type_id as offering_offering_invoice_type_id, 
                            offering.name as offering_name, offering.short_name as offering_short_name, offering.descr as offering_descr, offering.is_gst_exempt as offering_is_gst_exempt, 
                            offering.default_price as offering_default_price, offering.service_time_minutes as offering_service_time_minutes, 
                            offering.max_nbr_claimable as offering_max_nbr_claimable, offering.max_nbr_claimable_months as offering_max_nbr_claimable_months,
                            offering.medicare_company_code as offering_medicare_company_code, offering.dva_company_code as offering_dva_company_code, offering.tac_company_code as offering_tac_company_code, 
                            offering.medicare_charge as offering_medicare_charge, offering.dva_charge as offering_dva_charge, offering.tac_charge as offering_tac_charge,
                            offering.popup_message as offering_popup_message, offering.reminder_letter_months_later_to_send as offering_reminder_letter_months_later_to_send, offering.reminder_letter_id as offering_reminder_letter_id, offering.use_custom_color as offering_use_custom_color, offering.custom_color as offering_custom_color
                            ,
                            offeringfield.field_id as offeringfield_field_id, offeringfield.descr as offeringfield_descr
                            ,
                            added_by.staff_id as added_by_staff_id, added_by.person_id as added_by_person_id, added_by.login as added_by_login, added_by.pwd as added_by_pwd, 
                            added_by.staff_position_id as added_by_staff_position_id, added_by.field_id as added_by_field_id, added_by.costcentre_id as added_by_costcentre_id, 
                            added_by.is_contractor as added_by_is_contractor, added_by.tfn as added_by_tfn, added_by.provider_number as added_by_provider_number, 
                            added_by.is_fired as added_by_is_fired, added_by.is_commission as added_by_is_commission, added_by.commission_percent as added_by_commission_percent, 
                            added_by.is_stakeholder as added_by_is_stakeholder, added_by.is_master_admin as added_by_is_master_admin, added_by.is_admin as added_by_is_admin, added_by.is_principal as added_by_is_principal, added_by.is_provider as added_by_is_provider, added_by.is_external as added_by_is_external,
                            added_by.staff_date_added as added_by_staff_date_added, added_by.start_date as added_by_start_date, added_by.end_date as added_by_end_date, added_by.comment as added_by_comment, 
                            added_by.num_days_to_display_on_booking_screen as added_by_num_days_to_display_on_booking_screen,  added_by.show_header_on_booking_screen as added_by_show_header_on_booking_screen,
                            added_by.bk_screen_field_id as added_by_bk_screen_field_id, added_by.bk_screen_show_key as added_by_bk_screen_show_key, added_by.enable_daily_reminder_sms as added_by_enable_daily_reminder_sms, added_by.enable_daily_reminder_email as added_by_enable_daily_reminder_email
                            ,
                            " + PersonDB.GetFields("person_added_by_", "person_added_by") + @"
                            ,
                            title_added_by.title_id as title_added_by_title_id, title_added_by.descr as title_added_by_descr
                            ,
                            confirmed_by.staff_id as confirmed_by_staff_id, confirmed_by.person_id as confirmed_by_person_id, confirmed_by.login as confirmed_by_login, confirmed_by.pwd as confirmed_by_pwd, 
                            confirmed_by.staff_position_id as confirmed_by_staff_position_id, confirmed_by.field_id as confirmed_by_field_id, confirmed_by.costcentre_id as confirmed_by_costcentre_id, 
                            confirmed_by.is_contractor as confirmed_by_is_contractor, confirmed_by.tfn as confirmed_by_tfn, confirmed_by.provider_number as confirmed_by_provider_number, 
                            confirmed_by.is_fired as confirmed_by_is_fired, confirmed_by.is_commission as confirmed_by_is_commission, confirmed_by.commission_percent as confirmed_by_commission_percent, 
                            confirmed_by.is_stakeholder as confirmed_by_is_stakeholder, confirmed_by.is_master_admin as confirmed_by_is_master_admin, confirmed_by.is_admin as confirmed_by_is_admin, confirmed_by.is_principal as confirmed_by_is_principal, confirmed_by.is_provider as confirmed_by_is_provider, confirmed_by.is_external as confirmed_by_is_external,
                            confirmed_by.staff_date_added as confirmed_by_staff_date_added, confirmed_by.start_date as confirmed_by_start_date, confirmed_by.end_date as confirmed_by_end_date, 
                            confirmed_by.comment as confirmed_by_comment, confirmed_by.num_days_to_display_on_booking_screen as confirmed_by_num_days_to_display_on_booking_screen,  confirmed_by.show_header_on_booking_screen as confirmed_by_show_header_on_booking_screen,
                            confirmed_by.bk_screen_field_id as confirmed_by_bk_screen_field_id, confirmed_by.bk_screen_show_key as confirmed_by_bk_screen_show_key, confirmed_by.enable_daily_reminder_sms as confirmed_by_enable_daily_reminder_sms, confirmed_by.enable_daily_reminder_email as confirmed_by_enable_daily_reminder_email
                            ,
                            " + PersonDB.GetFields("person_confirmed_by_", "person_confirmed_by") + @"
                            ,
                            title_confirmed_by.title_id as title_confirmed_by_title_id, title_confirmed_by.descr as title_confirmed_by_descr
                            ,
                            deleted_by.staff_id as deleted_by_staff_id, deleted_by.person_id as deleted_by_person_id, deleted_by.login as deleted_by_login, deleted_by.pwd as deleted_by_pwd, 
                            deleted_by.staff_position_id as deleted_by_staff_position_id, deleted_by.field_id as deleted_by_field_id, deleted_by.costcentre_id as deleted_by_costcentre_id, 
                            deleted_by.is_contractor as deleted_by_is_contractor, deleted_by.tfn as deleted_by_tfn, deleted_by.provider_number as deleted_by_provider_number, 
                            deleted_by.is_fired as deleted_by_is_fired, deleted_by.is_commission as deleted_by_is_commission, deleted_by.commission_percent as deleted_by_commission_percent, 
                            deleted_by.is_stakeholder as deleted_by_is_stakeholder, deleted_by.is_master_admin as deleted_by_is_master_admin, deleted_by.is_admin as deleted_by_is_admin, deleted_by.is_principal as deleted_by_is_principal, deleted_by.is_provider as deleted_by_is_provider, deleted_by.is_external as deleted_by_is_external,
                            deleted_by.staff_date_added as deleted_by_staff_date_added, deleted_by.start_date as deleted_by_start_date, deleted_by.end_date as deleted_by_end_date, deleted_by.comment as deleted_by_comment, 
                            deleted_by.num_days_to_display_on_booking_screen as deleted_by_num_days_to_display_on_booking_screen,  deleted_by.show_header_on_booking_screen as deleted_by_show_header_on_booking_screen,
                            deleted_by.bk_screen_field_id as deleted_by_bk_screen_field_id, deleted_by.bk_screen_show_key as deleted_by_bk_screen_show_key, deleted_by.enable_daily_reminder_sms as deleted_by_enable_daily_reminder_sms, deleted_by.enable_daily_reminder_email as deleted_by_enable_daily_reminder_email
                            ,
                            " + PersonDB.GetFields("person_deleted_by_", "person_deleted_by") + @"
                            ,
                            title_deleted_by.title_id as title_deleted_by_title_id, title_deleted_by.descr as title_deleted_by_descr
                            ,
                            cancelled_by.staff_id as cancelled_by_staff_id, cancelled_by.person_id as cancelled_by_person_id, cancelled_by.login as cancelled_by_login, cancelled_by.pwd as cancelled_by_pwd, 
                            cancelled_by.staff_position_id as cancelled_by_staff_position_id, cancelled_by.field_id as cancelled_by_field_id, cancelled_by.costcentre_id as cancelled_by_costcentre_id, 
                            cancelled_by.is_contractor as cancelled_by_is_contractor, cancelled_by.tfn as cancelled_by_tfn, cancelled_by.provider_number as cancelled_by_provider_number, 
                            cancelled_by.is_fired as cancelled_by_is_fired, cancelled_by.is_commission as cancelled_by_is_commission, cancelled_by.commission_percent as cancelled_by_commission_percent, 
                            cancelled_by.is_stakeholder as cancelled_by_is_stakeholder, cancelled_by.is_master_admin as cancelled_by_is_master_admin, cancelled_by.is_admin as cancelled_by_is_admin, cancelled_by.is_principal as cancelled_by_is_principal, cancelled_by.is_provider as cancelled_by_is_provider, cancelled_by.is_external as cancelled_by_is_external,
                            cancelled_by.staff_date_added as cancelled_by_staff_date_added, cancelled_by.start_date as cancelled_by_start_date, cancelled_by.end_date as cancelled_by_end_date, cancelled_by.comment as cancelled_by_comment, 
                            cancelled_by.num_days_to_display_on_booking_screen as cancelled_by_num_days_to_display_on_booking_screen,  cancelled_by.show_header_on_booking_screen as cancelled_by_show_header_on_booking_screen,
                            cancelled_by.bk_screen_field_id as cancelled_by_bk_screen_field_id, cancelled_by.bk_screen_show_key as cancelled_by_bk_screen_show_key, cancelled_by.enable_daily_reminder_sms as cancelled_by_enable_daily_reminder_sms, cancelled_by.enable_daily_reminder_email as cancelled_by_enable_daily_reminder_email
                            ,
                            " + PersonDB.GetFields("person_cancelled_by_", "person_cancelled_by") + @"
                            ,
                            title_cancelled_by.title_id as title_cancelled_by_title_id, title_cancelled_by.descr as title_cancelled_by_descr

                       FROM 
                            Booking booking
                            LEFT OUTER JOIN BookingStatus booking_status    ON booking.booking_status_id     = booking_status.booking_status_id
                            LEFT OUTER JOIN BookingConfirmedByType      booking_confirmed_by_type       ON booking.booking_confirmed_by_type_id     = booking_confirmed_by_type.booking_confirmed_by_type_id
                            LEFT OUTER JOIN BookingUnavailabilityReason booking_unavailability_reason   ON booking.booking_unavailability_reason_id = booking_unavailability_reason.booking_unavailability_reason_id
                            LEFT OUTER JOIN Organisation org                ON org.organisation_id           = booking.organisation_id
                            LEFT OUTER JOIN OrganisationType org_type       ON org.organisation_type_id      = org_type.organisation_type_id
                            LEFT OUTER JOIN OrganisationTypeGroup typegroup ON org_type.organisation_type_group_id = typegroup.organisation_type_group_id
                            LEFT OUTER JOIN Staff provider                  ON provider.staff_id             = booking.provider
                            LEFT OUTER JOIN Person person_provider          ON person_provider.person_id     = provider.person_id
                            LEFT OUTER JOIN Title  title_provider           ON title_provider.title_id       = person_provider.title_id
                            LEFT OUTER JOIN Patient patient                 ON patient.patient_id            = booking.patient_id
                            LEFT OUTER JOIN Person person_patient           ON person_patient.person_id      = patient.person_id
                            LEFT OUTER JOIN Title  title_patient            ON title_patient.title_id        = person_patient.title_id
                            LEFT OUTER JOIN Offering offering               ON offering.offering_id          = booking.offering_id
                            LEFT OUTER JOIN Field offeringfield             ON offeringfield.field_id        = offering.field_id
                            LEFT OUTER JOIN Staff added_by                  ON added_by.staff_id             = booking.added_by
                            LEFT OUTER JOIN Person person_added_by          ON person_added_by.person_id     = added_by.person_id
                            LEFT OUTER JOIN Title  title_added_by           ON title_added_by.title_id       = person_added_by.title_id
                            LEFT OUTER JOIN Staff  confirmed_by             ON confirmed_by.staff_id         = booking.confirmed_by
                            LEFT OUTER JOIN Person person_confirmed_by      ON person_confirmed_by.person_id = confirmed_by.person_id
                            LEFT OUTER JOIN Title  title_confirmed_by       ON title_confirmed_by.title_id   = person_confirmed_by.title_id
                            LEFT OUTER JOIN Staff  deleted_by               ON deleted_by.staff_id           = booking.deleted_by
                            LEFT OUTER JOIN Person person_deleted_by        ON person_deleted_by.person_id   = deleted_by.person_id
                            LEFT OUTER JOIN Title  title_deleted_by         ON title_deleted_by.title_id     = person_deleted_by.title_id
                            LEFT OUTER JOIN Staff  cancelled_by             ON cancelled_by.staff_id         = booking.cancelled_by
                            LEFT OUTER JOIN Person person_cancelled_by      ON person_cancelled_by.person_id = cancelled_by.person_id
                            LEFT OUTER JOIN Title  title_cancelled_by       ON title_cancelled_by.title_id   = person_cancelled_by.title_id

                       WHERE 
                            " + (incDeleted ? "" : " deleted_by is null AND date_deleted is null AND ") + " booking.booking_type_id <> 35 AND (booking.booking_status_id IS NULL OR booking.booking_status_id in (" + (incDeleted ? "-1," : "") + "0,187,188)) ";
    }


    public static Patient[] GetPatientsOfBookingsWithProviderAtOrg(int provider_id, int organisation_id)
    {
        string sql = @"SELECT   DISTINCT
                                pa.patient_id, pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased, 
                                pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                                pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info, 
                                " + PersonDB.GetFields("", "p") + @",
                                t.title_id, t.descr
                       FROM
                                Booking b
                                INNER JOIN Patient pa ON b.patient_id = pa.patient_id
                                INNER JOIN Person  p  ON pa.person_id = p.person_id
                                INNER JOIN Title   t  ON t.title_id = p.title_id
                       WHERE 
                                pa.is_deleted = 0 
                                AND b.provider = " + provider_id + @"
                                AND b.organisation_id = " + organisation_id + @"
                       ORDER BY 
                                p.surname, p.firstname, p.middlename";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Patient p = PatientDB.Load(tbl.Rows[i]);
            p.Person = PersonDB.Load(tbl.Rows[i]);
            p.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
            list[i] = p;
        }

        return list;
    }

    /*
    3. at time of generating batch letters (  >>>>>>>>>>  ONLY FOR PROVIDER -- SO MAYBE PUT IN MENU?? <<<<<<<<<<<<<<<<<<<<<<< )
       - get list by booking date so that know which is first/last/etc
       - get all "completed" bookings 
       - from start date (null possible) till end date (null possible) 
       - by provider, or by org
       - get where has_generated_system_letters = 0
    */

    public static int GetBookingCountToGenerateSystemLetters(DateTime date_start, DateTime date_end, int provider_id = -1, int organisation_id = 0)
    {
        string recurring_condition     = string.Empty;
        string non_recurring_condition = string.Empty;
        if (date_start != DateTime.MinValue && date_end != DateTime.MinValue)
        {
            recurring_condition     = "AND (  booking.date_start >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "' AND booking.date_end <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
            non_recurring_condition = "AND (  booking.date_end IS NULL OR booking.date_end > '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + (date_end == DateTime.MinValue ? "" : " AND booking.date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"";
        }
        else if (date_start != DateTime.MinValue)
        {
            recurring_condition     = "AND (  booking.date_start >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            non_recurring_condition = "AND (  booking.date_end IS NULL OR booking.date_end > '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + @"";
        }
        else if (date_end != DateTime.MinValue)
        {
            recurring_condition     = "AND (  booking.date_end  <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
            non_recurring_condition = "AND (  booking.date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
        }
        else
        {
            recurring_condition     = "";
            non_recurring_condition = "";
        }

        string sql = "SELECT COUNT(*) FROM BOOKING booking WHERE  booking.deleted_by is null AND booking.date_deleted is null AND booking.booking_type_id <> 35 AND (booking.booking_status_id IS NULL OR booking.booking_status_id in (0,187,188)) "
                                 + " AND booking.booking_status_id = 187 "
                                 + " AND booking.booking_type_id   = 34  "
                                 + " AND booking.has_generated_system_letters = 0 "
                                 + (provider_id     == -1 ? "" : " AND booking.provider = " + provider_id)
                                 + (organisation_id ==  0 ? "" : " AND booking.organisation_id = " + organisation_id)
                                 + (date_start == DateTime.MinValue ? "" : " AND booking.organisation_id = " + organisation_id)
                                 + @" AND (
                                             (booking.is_recurring = 0 " + recurring_condition     + @") OR  
                                             (booking.is_recurring = 1 " + non_recurring_condition + @") 
                                          ) ";

        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static Booking[] GetBookingsToGenerateSystemLetters(DateTime date_start, DateTime date_end, int provider_id = -1, int organisation_id = 0)
    {
        string recurring_condition     = string.Empty;
        string non_recurring_condition = string.Empty;
        if (date_start != DateTime.MinValue && date_end != DateTime.MinValue)
        {
            recurring_condition     = "AND (  booking.date_start >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "' AND booking.date_end <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
            non_recurring_condition = "AND (  booking.date_end IS NULL OR booking.date_end > '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + (date_end == DateTime.MinValue ? "" : " AND booking.date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"";
        }
        else if (date_start != DateTime.MinValue)
        {
            recurring_condition     = "AND (  booking.date_start >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            non_recurring_condition = "AND (  booking.date_end IS NULL OR booking.date_end > '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + @"";
        }
        else if (date_end != DateTime.MinValue)
        {
            recurring_condition     = "AND (  booking.date_end  <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
            non_recurring_condition = "AND (  booking.date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
        }
        else
        {
            recurring_condition     = "";
            non_recurring_condition = "";
        }

        string sql = JoinedSQL()
                                 + " AND booking.booking_status_id = 187 "
                                 + " AND booking.booking_type_id   = 34  "
                                 + " AND booking.has_generated_system_letters = 0 "
                                 + (provider_id     == -1 ? "" : " AND booking.provider = " + provider_id)
                                 + (organisation_id ==  0 ? "" : " AND booking.organisation_id = " + organisation_id)
                                 + (date_start == DateTime.MinValue ? "" : " AND booking.organisation_id = " + organisation_id)
                                 + @" AND (
                                             (booking.is_recurring = 0 " + recurring_condition     + @") OR  
                                             (booking.is_recurring = 1 " + non_recurring_condition + @") 
                                          ) "
                                 + " ORDER BY booking.date_start ";


        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Booking[] bookings = new Booking[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bookings[i] = LoadFull(tbl.Rows[i]);

        return bookings;
    }


    // ==========================================================================================================
    // Get this into a big datatable, and sort by regerferrer_id - then its all one query
    // Stop trying to put it all into objects - it will slow it all down too much from too many queries from the page that generates the letters !
    // ==========================================================================================================
    public static DataTable GetBookingsWithEPCLetters(DateTime bookingStartDate, DateTime bookingEndDate, int register_referrer_id = -1, int patient_id = -1, bool inc_sent = false, bool inc_unsent = false, bool inc_batching = false, bool inc_non_batching = false)
    {

        string sentOrUnsent = string.Empty;
        if (!inc_sent && !inc_unsent)
            sentOrUnsent = " 1<>1 AND ";
        else if (!inc_sent &&  inc_unsent)
            sentOrUnsent = " booking.has_generated_system_letters = 0 AND ";
        else if ( inc_sent && !inc_unsent)
            sentOrUnsent = " booking.has_generated_system_letters = 1 AND ";

        string referrerBatching = string.Empty;
        if (inc_unsent)
        {
            if (!inc_batching && !inc_non_batching)
                referrerBatching = " 1<>1 AND ";
            else if (!inc_batching && inc_non_batching)
                referrerBatching = " regref.batch_send_all_patients_treatment_notes = 0 AND ";
            else if (inc_batching && !inc_non_batching)
                referrerBatching = " regref.batch_send_all_patients_treatment_notes = 1 AND ";
        }

        string sql = @"
        SELECT DISTINCT

            -- booking info

            booking.booking_id as booking_booking_id,booking.entity_id as booking_entity_id,
            booking.date_start as booking_date_start,booking.date_end as booking_date_end,booking.organisation_id as booking_organisation_id,
            booking.provider as booking_provider,booking.patient_id as booking_patient_id,booking.offering_id as booking_offering_id,booking.booking_type_id as booking_booking_type_id,
            booking.booking_status_id as booking_booking_status_id,booking.booking_unavailability_reason_id as booking_booking_unavailability_reason_id,booking.added_by as booking_added_by,booking.date_created as booking_date_created,
            booking.booking_confirmed_by_type_id as booking_booking_confirmed_by_type_id,booking.confirmed_by as booking_confirmed_by,booking.date_confirmed as booking_date_confirmed,
            booking.deleted_by as booking_deleted_by, booking.date_deleted as booking_date_deleted,
            booking.cancelled_by as booking_cancelled_by, booking.date_cancelled as booking_date_cancelled,
            booking.is_patient_missed_appt as booking_is_patient_missed_appt,booking.is_provider_missed_appt as booking_is_provider_missed_appt,
            booking.is_emergency as booking_is_emergency,
            booking.need_to_generate_first_letter as booking_need_to_generate_first_letter,booking.need_to_generate_last_letter as booking_need_to_generate_last_letter,booking.has_generated_system_letters as booking_has_generated_system_letters,
            booking.arrival_time              as booking_arrival_time,
            booking.sterilisation_code        as booking_sterilisation_code,
            booking.informed_consent_added_by as booking_informed_consent_added_by, 
            booking.informed_consent_date     as booking_informed_consent_date,

            booking.is_recurring as booking_is_recurring,booking.recurring_weekday_id as booking_recurring_weekday_id,
            booking.recurring_start_time as booking_recurring_start_time,booking.recurring_end_time as booking_recurring_end_time,

            offering.offering_id as offering_offering_id, offering.offering_type_id as offering_offering_type_id, offering.field_id as offering_field_id,
            offering.aged_care_patient_type_id as offering_aged_care_patient_type_id, 
            offering.num_clinic_visits_allowed_per_year as offering_num_clinic_visits_allowed_per_year, 
            offering.offering_invoice_type_id as offering_offering_invoice_type_id, 
            offering.name as offering_name, offering.short_name as offering_short_name, offering.descr as offering_descr, offering.is_gst_exempt as offering_is_gst_exempt, 
            offering.default_price as offering_default_price, offering.service_time_minutes as offering_service_time_minutes, 
            offering.max_nbr_claimable as offering_max_nbr_claimable, offering.max_nbr_claimable_months as offering_max_nbr_claimable_months,
            offering.medicare_company_code as offering_medicare_company_code, offering.dva_company_code as offering_dva_company_code, offering.tac_company_code as offering_tac_company_code, 
            offering.medicare_charge as offering_medicare_charge, offering.dva_charge as offering_dva_charge, offering.tac_charge as offering_tac_charge,
            offering.popup_message as offering_popup_message, offering.reminder_letter_months_later_to_send as offering_reminder_letter_months_later_to_send, offering.reminder_letter_id as offering_reminder_letter_id, offering.use_custom_color as offering_use_custom_color, offering.custom_color as offering_custom_color,

            (SELECT COUNT(*) FROM Note note WHERE note.entity_id = booking.entity_id) AS booking_note_count,
            -- can use this - would be faster (also in booking class changing to booking HasNotes (instead of count), since dont need to know the count!!
            -- CASE WHEN EXISTS (SELECT * FROM Note WHERE entity_id = booking.entity_id) THEN '1' ELSE '0' END AS booking_note_count


            -- patientreferrer info

            pr.patient_referrer_id as pr_patient_referrer_id, pr.patient_id as pr_patient_id, pr.register_referrer_id as pr_register_referrer_id, pr.organisation_id as pr_organisation_id, pr.patient_referrer_date_added as pr_patient_referrer_date_added, pr.is_debtor as pr_is_debtor, pr.is_active as pr_is_active, 
            pat.patient_id as patient_patient_id,pat.person_id as patient_person_id, pat.patient_date_added as patient_patient_date_added, pat.is_clinic_patient as patient_is_clinic_patient, pat.is_gp_patient as patient_is_gp_patient, 
            pat.is_deleted as patient_is_deleted, pat.is_deceased as patient_is_deceased, 
            pat.flashing_text as patient_flashing_text, pat.flashing_text_added_by as patient_flashing_text_added_by, pat.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
            pat.private_health_fund as patient_private_health_fund, pat.concession_card_number as patient_concession_card_number, pat.concession_card_expiry_date as patient_concession_card_expiry_date, pat.is_diabetic as patient_is_diabetic, pat.is_member_diabetes_australia as patient_is_member_diabetes_australia, pat.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, pat.ac_inv_offering_id as patient_ac_inv_offering_id, pat.ac_pat_offering_id as patient_ac_pat_offering_id, pat.login as patient_login, pat.pwd as patient_pwd, pat.is_company as patient_is_company, pat.abn as patient_abn, 
            pat.next_of_kin_name as patient_next_of_kin_name, pat.next_of_kin_relation as patient_next_of_kin_relation, pat.next_of_kin_contact_info as patient_next_of_kin_contact_info, 
            regref.register_referrer_id as regref_register_referrer_id,regref.organisation_id as regref_organisation_id, regref.referrer_id as regref_referrer_id, regref.provider_number as regref_provider_number,
            regref.report_every_visit_to_referrer as regref_report_every_visit_to_referrer,
            regref.batch_send_all_patients_treatment_notes as regref_batch_send_all_patients_treatment_notes, regref.date_last_batch_send_all_patients_treatment_notes as regref_date_last_batch_send_all_patients_treatment_notes,
            regref.register_referrer_date_added as regref_register_referrer_date_added,  
            ref.referrer_id as referrer_referrer_id,ref.person_id as referrer_person_id, ref.referrer_date_added as referrer_referrer_date_added, 
            org.entity_id as organisation_entity_id, org.parent_organisation_id as organisation_parent_organisation_id, org.use_parent_offernig_prices as organisation_use_parent_offernig_prices, org.organisation_type_id as organisation_organisation_type_id, org.organisation_customer_type_id as organisation_organisation_customer_type_id,org.name as organisation_name, org.acn as organisation_acn, org.abn as organisation_abn, org.organisation_date_added as organisation_organisation_date_added, 
            org.organisation_id as organisation_organisation_id, org.organisation_date_modified as organisation_organisation_date_modified, org.is_debtor as organisation_is_debtor, org.is_creditor as organisation_is_creditor, org.bpay_account as organisation_bpay_account, org.weeks_per_service_cycle as organisation_weeks_per_service_cycle, org.start_date as organisation_start_date, 
            org.end_date as organisation_end_date, org.comment as organisation_comment, org.free_services as organisation_free_services, org.excl_sun as organisation_excl_sun, org.excl_mon as organisation_excl_mon, org.excl_tue as organisation_excl_tue, org.excl_wed as organisation_excl_wed, org.excl_thu as organisation_excl_thu, org.excl_fri as organisation_excl_fri, org.excl_sat as organisation_excl_sat, 
            org.sun_start_time as organisation_sun_start_time, org.sun_end_time as organisation_sun_end_time, org.mon_start_time as organisation_mon_start_time, org.mon_end_time as organisation_mon_end_time, org.tue_start_time as organisation_tue_start_time, org.tue_end_time as organisation_tue_end_time, org.wed_start_time as organisation_wed_start_time, org.wed_end_time as organisation_wed_end_time, 
            org.thu_start_time as organisation_thu_start_time, org.thu_end_time as organisation_thu_end_time, org.fri_start_time as organisation_fri_start_time, org.fri_end_time as organisation_fri_end_time, org.sat_start_time as organisation_sat_start_time, org.sat_end_time as organisation_sat_end_time, 
            org.sun_lunch_start_time as organisation_sun_lunch_start_time, org.sun_lunch_end_time as organisation_sun_lunch_end_time, org.mon_lunch_start_time as organisation_mon_lunch_start_time, org.mon_lunch_end_time as organisation_mon_lunch_end_time, org.tue_lunch_start_time as organisation_tue_lunch_start_time, org.tue_lunch_end_time as organisation_tue_lunch_end_time, org.wed_lunch_start_time as organisation_wed_lunch_start_time, org.wed_lunch_end_time as organisation_wed_lunch_end_time, 
            org.thu_lunch_start_time as organisation_thu_lunch_start_time, org.thu_lunch_end_time as organisation_thu_lunch_end_time, org.fri_lunch_start_time as organisation_fri_lunch_start_time, org.fri_lunch_end_time as organisation_fri_lunch_end_time, org.sat_lunch_start_time as organisation_sat_lunch_start_time, org.sat_lunch_end_time as organisation_sat_lunch_end_time, 
            org.last_batch_run as organisation_last_batch_run, org.is_deleted as organisation_is_deleted,

            " + PersonDB.GetFields("patient_person_", "patient_person") + @",
            patient_person_title.title_id as patient_person_title_title_id, patient_person_title.descr as patient_person_title_descr,

            " + PersonDB.GetFields("referrer_person_", "referrer_person") + @",
            referrer_person_title.title_id as referrer_person_title_title_id, referrer_person_title.descr as referrer_person_title_descr,

            -- hc info

            hc.health_card_id as hc_health_card_id, hc.patient_id as hc_patient_id, hc.organisation_id as hc_organisation_id, hc.card_name as hc_card_name, hc.card_nbr as hc_card_nbr, hc.card_family_member_nbr as hc_card_family_member_nbr, hc.expiry_date as hc_expiry_date,
            hc.date_referral_signed as hc_date_referral_signed, hc.date_referral_received_in_office as hc_date_referral_received_in_office, hc.is_active as hc_is_active, hc.added_or_last_modified_by as hc_added_or_last_modified_by, hc.added_or_last_modified_date as hc_added_or_last_modified_date, hc.area_treated as hc_area_treated,

            -- if regref has email
             CASE WHEN EXISTS (SELECT * FROM " + Utilities.GetAddressType() + @" WHERE contact_date_deleted IS NULL AND entity_id = org.entity_id AND contact_type_id = 27 AND addr_line1 LIKE '%_@_%_.__%') THEN '1' ELSE '0' END AS ref_has_email,
            (SELECT TOP 1 addr_line1 FROM " + Utilities.GetAddressType() + @" WHERE contact_date_deleted IS NULL AND entity_id = org.entity_id AND contact_type_id = 27) as ref_email,
             CASE WHEN EXISTS (SELECT * FROM " + Utilities.GetAddressType() + @" WHERE contact_date_deleted IS NULL AND entity_id = org.entity_id AND contact_type_id = 29 AND PATINDEX('%[0-9][0-9][0-9]%', addr_line1) > 0) THEN '1' ELSE '0' END AS ref_has_fax,
            (SELECT TOP 1 addr_line1 FROM " + Utilities.GetAddressType() + @" WHERE contact_date_deleted IS NULL AND entity_id = org.entity_id AND contact_type_id = 29) as ref_fax

        FROM

            Booking booking
            INNER JOIN Offering AS offering ON offering.offering_id = booking.offering_id 

            INNER JOIN PatientReferrer AS pr  ON pr.patient_id = booking.patient_id
            INNER JOIN Patient AS pat ON pat.patient_id = pr.patient_id 
            LEFT OUTER JOIN RegisterReferrer AS regref ON regref.register_referrer_id = pr.register_referrer_id 
            LEFT OUTER JOIN Referrer AS ref ON ref.referrer_id = regref.referrer_id 
            LEFT OUTER JOIN Organisation AS org ON org.organisation_id = regref.organisation_id
            INNER JOIN Person AS patient_person  ON patient_person.person_id  = pat.person_id
            INNER JOIN Title  AS patient_person_title ON patient_person_title.title_id = patient_person.title_id
            INNER JOIN Person AS referrer_person ON referrer_person.person_id = ref.person_id
            INNER JOIN Title  AS referrer_person_title ON referrer_person_title.title_id = referrer_person.title_id

            INNER JOIN Invoice AS invoice ON invoice.booking_id = booking.booking_id AND invoice.payer_organisation_id = -1

            INNER JOIN HealthCard AS hc ON booking.patient_id = hc.patient_id AND hc.is_active = 1

        WHERE
            booking.deleted_by IS NULL AND booking.date_deleted IS NULL AND
            booking.booking_type_id = 34 AND
            booking.booking_status_id = 187 AND
            " + sentOrUnsent + @"
            " + referrerBatching + @"
            pr.is_active = 1 
            " + (register_referrer_id == -1 ? "" : "AND regref.register_referrer_id = " + register_referrer_id) + @"
            " + (patient_id == -1 ? "" : "AND booking.patient_id = " + patient_id) + @"
            " + (bookingStartDate == DateTime.MinValue ? "" : " AND booking.date_start >= '" + bookingStartDate.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
            " + (bookingEndDate   == DateTime.MinValue ? "" : " AND booking.date_end   <= '" + bookingEndDate.Date.AddHours(23).ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
            AND (booking.need_to_generate_first_letter = 1 OR booking.need_to_generate_last_letter = 1 OR (regref.report_every_visit_to_referrer = 1 AND (SELECT COUNT(*) FROM Note note WHERE note.entity_id = booking.entity_id) > 0))
        ORDER BY 
            regref.batch_send_all_patients_treatment_notes, ref_has_email DESC, referrer_person.surname,referrer_person.firstname, booking.patient_id";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }



    public static DataTable GetReport_Bookings(DateTime booking_date_start, DateTime booking_date_end, int organisation_id, int provider_id, int organisation_type_group_id, string statusIDsToInclude, bool useTreatmentDate = true)  // organisation_type_group_id : [clinic=5, aged care = 6]
    {
        string sql = @"

                SELECT

                        -- so can use BookingDB.Load(row) .. to get link to booking page this booking
                        Booking.booking_id as booking_booking_id,Booking.entity_id as booking_entity_id,
                        Booking.date_start as booking_date_start,Booking.date_end as booking_date_end,Booking.organisation_id as booking_organisation_id,
                        Booking.provider as booking_provider,Booking.patient_id as booking_patient_id,Booking.offering_id as booking_offering_id,Booking.booking_type_id as booking_booking_type_id,
                        Booking.booking_status_id as booking_booking_status_id,Booking.booking_unavailability_reason_id as booking_booking_unavailability_reason_id,Booking.added_by as booking_added_by,Booking.date_created as booking_date_created,
                        Booking.booking_confirmed_by_type_id as booking_booking_confirmed_by_type_id,Booking.confirmed_by as booking_confirmed_by,Booking.date_confirmed as booking_date_confirmed,
                        Booking.deleted_by as booking_deleted_by, Booking.date_deleted as booking_date_deleted,
                        Booking.cancelled_by as booking_cancelled_by, Booking.date_cancelled as booking_date_cancelled,
                        Booking.is_patient_missed_appt as booking_is_patient_missed_appt,Booking.is_provider_missed_appt as booking_is_provider_missed_appt,
                        Booking.is_emergency as booking_is_emergency,
                        Booking.need_to_generate_first_letter as booking_need_to_generate_first_letter,Booking.need_to_generate_last_letter as booking_need_to_generate_last_letter,Booking.has_generated_system_letters as booking_has_generated_system_letters,
                        Booking.arrival_time as booking_arrival_time,
                        Booking.sterilisation_code as booking_sterilisation_code,
                        Booking.informed_consent_added_by as booking_informed_consent_added_by, 
                        Booking.informed_consent_date     as booking_informed_consent_date,
                        Booking.is_recurring as booking_is_recurring,Booking.recurring_weekday_id as booking_recurring_weekday_id,
                        Booking.recurring_start_time as booking_recurring_start_time,Booking.recurring_end_time as booking_recurring_end_time,



                        Organisation.name               as organisation_name, 
                        Provider.staff_id               as provider_staff_id, 
                        ProviderPerson.firstname        as provider_firstname, 
                        ProviderPerson.surname          as provider_surname,
                        PatientPerson.firstname         as patient_firstname, 
                        PatientPerson.surname           as patient_surname,
                        PatientPerson.entity_id         as patient_entity_id,
                        BookingStatus.booking_status_id as booking_status_id,
                        BookingStatus.descr             as booking_status_descr,

                        ISNULL((SELECT SUM(Receipt.total)      -- CASH --
                        FROM Receipt 
                             LEFT JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Receipt.reversed_by IS NULL AND Receipt.reversed_date IS NULL AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) AND Invoice.booking_id = Booking.booking_id and Receipt.total > 0 AND receipt_payment_type_id = 129
                        ),0) as total_cash_receipts,

                        ISNULL((SELECT SUM(Receipt.total)     -- EFT --
                        FROM Receipt 
                             LEFT JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Receipt.reversed_by IS NULL AND Receipt.reversed_date IS NULL AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) AND Invoice.booking_id = Booking.booking_id and Receipt.total > 0 AND receipt_payment_type_id = 130
                        ),0) as total_eft_receipts,

                        ISNULL((SELECT SUM(Receipt.total)     -- CREDIT CARD --
                        FROM Receipt 
                             LEFT JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Receipt.reversed_by IS NULL AND Receipt.reversed_date IS NULL AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) AND Invoice.booking_id = Booking.booking_id and Receipt.total > 0 AND receipt_payment_type_id = 133
                        ),0) as total_credit_card_receipts,

                        ISNULL((SELECT SUM(Receipt.total)     -- CHEQUE --
                        FROM Receipt 
                             LEFT JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Receipt.reversed_by IS NULL AND Receipt.reversed_date IS NULL AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) AND Invoice.booking_id = Booking.booking_id and Receipt.total > 0 AND receipt_payment_type_id = 136
                        ),0) as total_cheque_receipts,

                        ISNULL((SELECT SUM(Receipt.total)     -- MONEY ORDER --
                        FROM Receipt 
                             LEFT JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Receipt.reversed_by IS NULL AND Receipt.reversed_date IS NULL AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) AND Invoice.booking_id = Booking.booking_id and Receipt.total > 0 AND receipt_payment_type_id = 229
                        ),0) as total_money_order_receipts,

                        ISNULL((SELECT SUM(Receipt.total)     -- DIRECT CREDIT --
                        FROM Receipt 
                             LEFT JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Receipt.reversed_by IS NULL AND Receipt.reversed_date IS NULL AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) AND Invoice.booking_id = Booking.booking_id and Receipt.total > 0 AND receipt_payment_type_id = 362
                        ),0) as total_direct_credit_receipts,




                        ISNULL((SELECT SUM(Invoice.total)     -- DVA INVOICES TOTAL --
                        FROM Invoice 
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Invoice.booking_id = Booking.booking_id AND Invoice.payer_organisation_id = -2
                        ),0) as dva_invoices_total,

                        ISNULL((SELECT SUM(Invoice.total)     -- MEDICARE INVOICES TOTAL --
                        FROM Invoice 
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Invoice.booking_id = Booking.booking_id AND Invoice.payer_organisation_id = -1
                        ),0) as medicare_invoices_total,



                        ISNULL((SELECT SUM(Receipt.total)     -- RECEIPTS TOTAL --
                        FROM Receipt 
                             LEFT JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Receipt.reversed_by IS NULL AND Receipt.reversed_date IS NULL AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) AND Invoice.booking_id = Booking.booking_id and Receipt.total > 0
                        ),0) as total_receipts_non_medicare_non_dva,

                        ISNULL((SELECT SUM(CreditNote.total)     -- CREDIT NOTES TOTAL --
                        FROM CreditNote 
                             LEFT JOIN Invoice ON CreditNote.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND CreditNote.reversed_by IS NULL AND CreditNote.reversed_date IS NULL AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) AND Invoice.booking_id = Booking.booking_id and CreditNote.total > 0
                        ),0) as total_credit_notes_non_medicare_non_dva,

                        ISNULL((SELECT SUM(CreditNote.total)     -- CREDIT NOTES TOTAL --
                        FROM CreditNote 
                             LEFT JOIN Invoice ON CreditNote.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND CreditNote.reversed_by IS NULL AND CreditNote.reversed_date IS NULL AND Invoice.booking_id = Booking.booking_id and CreditNote.total > 0
                        ),0) as total_credit_notes,

                        ISNULL((SELECT -1 * SUM(Credit.amount)     -- VOUCHERS TOTAL --
                        FROM Credit 
                             LEFT JOIN Invoice ON Credit.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Credit.deleted_by IS NULL AND Credit.date_deleted IS NULL AND Invoice.booking_id = Booking.booking_id and Credit.amount < 0
                        ),0) as total_vouchers,

                        ISNULL((SELECT SUM(invoice.total)     -- INVOICES TOTAL --
                        FROM Invoice 
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Invoice.booking_id = Booking.booking_id AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2))
                        ),0) as invoices_total_non_medicare_non_dva,

                        ISNULL((SELECT COUNT(invoice.total)   -- INVOICES COUNT --
                        FROM Invoice 
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Invoice.booking_id = Booking.booking_id AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2))
                        ),0) as invoices_count_non_medicare_non_dva,



                        Offering.offering_id as offering_id, 
                        Offering.name as offering_name, 
                        Offering.default_price as offering_default_price,  -- Can also select medicare/dva/ins price if used


                        ISNULL((SELECT SUM(invoice.total)     -- INVOICES TOTAL --
                        FROM Invoice 
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Invoice.booking_id = Booking.booking_id
                        ),0) as invoices_total,

                        ISNULL((SELECT SUM(invoice.gst)     -- INVOICES GST TOTAL --
                        FROM Invoice 
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Invoice.booking_id = Booking.booking_id
                        ),0) as invoices_gst_total,

                        /* Do this in the code getting invoice lines into a hashtable for only the bookings retrieved - using this XML within the DB takes a massive amount of time
                        STUFF(
                               (
                                SELECT  InvoiceLine.invoice_line_id, Offering.name AS offering_name, InvoiceLine.quantity, InvoiceLine.price, InvoiceLine.offering_id, InvoiceLine.area_treated, InvoiceLine.offering_order_id
                                FROM    InvoiceLine 
                                        INNER JOIN Invoice ON Invoice.invoice_id = InvoiceLine.invoice_id 
                                        LEFT OUTER JOIN Offering ON InvoiceLine.offering_id = Offering.offering_id
                                WHERE  Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND (Invoice.booking_id = Booking.booking_id) 
                                FOR XML PATH('')
                               ), 
                               1, 0, '') AS invoice_line_list
                        */
                        '' as invoice_line_lists


                        -- ,Booking.*

                FROM
                        Booking
                        LEFT JOIN Organisation ON Organisation.organisation_id = Booking.organisation_id
                        " + (organisation_type_group_id != -1 ? "LEFT JOIN OrganisationType on OrganisationType.organisation_type_id = Organisation.organisation_type_id " : "") + @"
                        LEFT JOIN Staff  Provider       ON Provider.staff_id               = Booking.provider
                        LEFT JOIN Person ProviderPerson ON ProviderPerson.person_id        = Provider.person_id
                        LEFT JOIN Patient               ON Patient.patient_id              = Booking.patient_id
                        LEFT JOIN Person PatientPerson  ON PatientPerson.person_id         = Patient.person_id
                        LEFT JOIN BookingStatus         ON BookingStatus.booking_status_id = Booking.booking_status_id
                        LEFT JOIN Offering              ON Offering.offering_id            = Booking.offering_id

                WHERE
                        Booking.booking_type_id = 34 " +

                        (
                        useTreatmentDate ?
                            (
                                (booking_date_start != DateTime.MinValue ? " AND Booking.date_start >= '" + booking_date_start.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "") +
                                (booking_date_end   != DateTime.MinValue ? " AND Booking.date_end   <= '" + booking_date_end.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "")
                            )
                            :
                            (
                                (booking_date_start != DateTime.MinValue ? " AND Booking.date_created >= '" + booking_date_start.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "") +
                                (booking_date_end   != DateTime.MinValue ? " AND Booking.date_created <= '" + booking_date_end.ToString("yyyy-MM-dd HH:mm:ss")   + "'" : "")
                            )
                        ) +

                        (organisation_id != -1 ? " AND Booking.organisation_id  = " + organisation_id : "") +
                        (provider_id     != -1 ? " AND Booking.provider         = " + provider_id     : "") +
                        (organisation_type_group_id != -1 ? " AND OrganisationType.organisation_type_group_id = " + organisation_type_group_id : "") +
                        (statusIDsToInclude != null && statusIDsToInclude.Length > 0 ? " AND booking.booking_status_id IN (" + statusIDsToInclude + ") " : "") + @"

                ORDER BY 
                         " + (useTreatmentDate ?
                           "Organisation.name, ProviderPerson.firstname, ProviderPerson.surname, Booking.date_start" :
                           "Organisation.name, ProviderPerson.firstname, ProviderPerson.surname, CONVERT (DATE, Booking.date_created), Booking.date_start");

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        /*
        // get some otehr stuff here!

        int[] bookingIDs = new int[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bookingIDs[i] = Convert.ToInt32(tbl.Rows[i]["booking_booking_id"]);
        string bookingIDsString = string.Join(",", bookingIDs);
        if (bookingIDsString.Length == 0) bookingIDsString = "0";


        string sql_rcpts_cash = @"
                        SELECT Invoice.booking_id, Sum(Receipt.total) as total      -- CASH --
                        FROM Receipt 
                             LEFT JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
                        WHERE Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL AND Receipt.reversed_by IS NULL AND Receipt.reversed_date IS NULL AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) and Receipt.total > 0 AND receipt_payment_type_id = 129
                        AND Invoice.booking_id in (" + bookingIDsString + @")
                        GROUP BY booking_id";

        DataTable tbl_rcts_cash = DBBase.ExecuteQuery(sql_rcpts_cash).Tables[0];
        Hashtable hash_rcts_cash = new Hashtable();
        for (int i = 0; i < tbl_rcts_cash.Rows.Count; i++)
            hash_rcts_cash[Convert.ToInt32(tbl_rcts_cash.Rows[i]["booking_id"])] = Convert.ToDecimal(tbl_rcts_cash.Rows[i]["total"]);
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int bookingID = Convert.ToInt32(tbl.Rows[i]["booking_booking_id"]);

            if (hash_rcts_cash[bookingID] != null)
                tbl.Rows[i]["total_cash_receipts"] = (decimal)hash_rcts_cash[bookingID];
        }
        */


        return tbl;
    }
    public static DataTable GetReport_Bookings_SubSection_InvoiceLines(int[] booking_ids)
    {
        string sql = @"

                SELECT  Invoice.booking_id, InvoiceLine.invoice_line_id, Offering.name AS offering_name, InvoiceLine.quantity, InvoiceLine.price, InvoiceLine.offering_id, InvoiceLine.area_treated, InvoiceLine.service_reference, InvoiceLine.offering_order_id
                FROM    InvoiceLine 
                        INNER JOIN Invoice ON Invoice.invoice_id = InvoiceLine.invoice_id 
                        LEFT OUTER JOIN Offering ON InvoiceLine.offering_id = Offering.offering_id
                WHERE  Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL
	                AND Invoice.booking_id IN (" + (booking_ids == null || booking_ids.Length == 0 ? "0" : string.Join(",", booking_ids)) + @")

                ORDER BY Invoice.booking_id, invoice.invoice_id";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetReport_InvoiceLines(DateTime booking_date_start, DateTime booking_date_end, DateTime invoice_date_start, DateTime invoice_date_end, int organisation_id, int provider_id, int offering_id, int organisation_type_group_id)  // organisation_type_group_id : [clinic=5, aged care = 6]
    {
        string sql = @"

                SELECT

                        InvoiceLine.invoice_line_id       as invoice_line_id,
                        InvoiceLine.price                 as invoice_line_price,
                        InvoiceLine.tax                   as invoice_line_tax,
                        InvoiceLine.quantity              as invoice_line_quantity,

                        InvoiceLine.invoice_id            as invoice_id,

                        Invoice.invoice_date_added,
                        Booking.booking_id,
                        Booking.date_created              as booking_date_created,
                        Booking.date_start                as booking_date_start,
                        Booking.date_end                  as booking_date_end,
                        BookingOrganisation.name          as booking_organisation_name,
                        NonBookingOrganisation.name       as non_booking_organisation_name,

                        Provider.staff_id                 as provider_staff_id,
                        ProviderPerson.firstname          as provider_firstname, 
                        ProviderPerson.surname            as provider_surname,
                        BookingPatient.patient_id         as booking_patient_id,
                        BookingPatientPerson.firstname    as booking_patient_firstname, 
                        BookingPatientPerson.surname      as booking_patient_surname,
                        NonBookingPatient.patient_id      as non_booking_patient_id,
                        NonBookingPatientPerson.firstname as non_booking_patient_firstname, 
                        NonBookingPatientPerson.surname   as non_booking_patient_surname,
                        Offering.offering_id              as offering_id,
                        Offering.name                     as offering_name,
                        
                        ISNULL((SELECT SUM(CreditNote.total)     -- CREDIT NOTES TOTAL --
                        FROM CreditNote 
                             LEFT JOIN Invoice i2 ON CreditNote.invoice_id = i2.invoice_id
                        WHERE i2.reversed_by IS NULL AND i2.reversed_date IS NULL AND CreditNote.reversed_by IS NULL AND CreditNote.reversed_date IS NULL AND CreditNote.invoice_id = InvoiceLine.invoice_id and CreditNote.total > 0
                        ),0) as total_credit_notes,

                        (SELECT TOP 1 date_moved FROM BookingChangeHistory WHERE booking_id = Booking.booking_id ORDER BY booking_change_history_id DESC) AS booking_date_last_moved

                FROM
                        InvoiceLine 
                        LEFT JOIN Invoice ON Invoice.invoice_id = InvoiceLine.invoice_id
                        LEFT JOIN Booking ON Booking.booking_id = Invoice.booking_id
                        LEFT JOIN BookingStatus         ON BookingStatus.booking_status_id = Booking.booking_status_id
                        LEFT JOIN Organisation as NonBookingOrganisation ON NonBookingOrganisation.organisation_id = Invoice.non_booking_invoice_organisation_id
                        " + (organisation_type_group_id != -1 ? "LEFT JOIN OrganisationType as NonBookingOrganisationType on NonBookingOrganisationType.organisation_type_id = NonBookingOrganisation.organisation_type_id " : "") + @"
                        LEFT JOIN Organisation as BookingOrganisation    ON BookingOrganisation.organisation_id    = Booking.organisation_id
                        " + (organisation_type_group_id != -1 ? "LEFT JOIN OrganisationType as BookingOrganisationType    on BookingOrganisationType.organisation_type_id    = BookingOrganisation.organisation_type_id "    : "") + @"
                        LEFT JOIN Staff   Provider                ON Provider.staff_id                 = Booking.provider
                        LEFT JOIN Person  ProviderPerson          ON ProviderPerson.person_id          = Provider.person_id
                        LEFT JOIN Patient BookingPatient          ON BookingPatient.patient_id         = Booking.patient_id
                        LEFT JOIN Person  BookingPatientPerson    ON BookingPatientPerson.person_id    = BookingPatient.person_id
                        LEFT JOIN Patient NonBookingPatient       ON NonBookingPatient.patient_id      = Invoice.payer_patient_id
                        LEFT JOIN Person  NonBookingPatientPerson ON NonBookingPatientPerson.person_id = NonBookingPatient.person_id
                        LEFT JOIN Offering                        ON Offering.offering_id              = InvoiceLine.offering_id

                WHERE
                        Invoice.reversed_by IS NULL AND Invoice.reversed_date IS NULL 
                        AND (Invoice.booking_id IS NULL OR (Booking.booking_type_id = 34 ))" + 
                        (invoice_date_start      != DateTime.MinValue ? " AND Invoice.invoice_date_added  >= '" + invoice_date_start.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "") + 
                        (invoice_date_end        != DateTime.MinValue ? " AND Invoice.invoice_date_added  <= '" + invoice_date_end.ToString("yyyy-MM-dd HH:mm:ss")   + "'" : "") +
                        (booking_date_start      != DateTime.MinValue ? " AND (Invoice.booking_id IS NULL OR (Booking.date_start          >= '" + booking_date_start.ToString("yyyy-MM-dd HH:mm:ss") + "'))" : "") + 
                        (booking_date_end        != DateTime.MinValue ? " AND (Invoice.booking_id IS NULL OR (Booking.date_end            <= '" + booking_date_end.ToString("yyyy-MM-dd HH:mm:ss")   + "'))" : "") +
                        (organisation_id != -1                ? " AND (Invoice.booking_id IS NULL OR (Booking.organisation_id  = "  + organisation_id + "))" : "") +
                        (provider_id     != -1                ? " AND (Invoice.booking_id IS NULL OR (Booking.provider         = "  + provider_id     + "))" : "") + 
                        (offering_id     != -1                ? " AND  InvoiceLine.offering_id = " + offering_id : "") +
                        (organisation_type_group_id != -1     ? " AND (Invoice.booking_id IS NULL     OR (BookingOrganisationType.organisation_type_group_id    = " + organisation_type_group_id + "))" : "") +
                        (organisation_type_group_id != -1     ? " AND (Invoice.booking_id IS NOT NULL OR (NonBookingOrganisationType.organisation_type_group_id = " + organisation_type_group_id + "))" : "") +
                        
                        // needed or else we also get non-booking invoices for all clinics
                        (organisation_id != -1                ? " AND (Booking.organisation_id IN (" + organisation_id + ") OR Invoice.payer_organisation_id IN (" + organisation_id + ") OR Invoice.non_booking_invoice_organisation_id IN (" + organisation_id + "))" : "") + @"

                ORDER BY 
                        ProviderPerson.firstname, ProviderPerson.surname, Invoice.invoice_date_added DESC, Booking.booking_id
        ";

        return DBBase.ExecuteQuery(sql, null, 60).Tables[0];
    }

    public static DataTable GetReport_StaffTimetable(DateTime date_start, DateTime date_end, bool inc_bookings, bool inc_aged_care, bool inc_clinics, int organisation_id = -1, int provider_id = -1)
    {

        // change first day to be start of week
        //int delta = DayOfWeek.Monday - date_start.DayOfWeek;
        //date_start = date_start.AddDays(delta);

        // change last day to be end of week
        //delta = DayOfWeek.Monday - date_end.DayOfWeek;
        //date_end = date_end.AddDays(delta + 6);


        // get staff set as providers and not fired
        Staff[] staff = StaffDB.GetAll();
        ArrayList list = new ArrayList();
        for (int i = 0; i < staff.Length; i++)
            if (staff[i].IsProvider && !staff[i].IsFired)
                list.Add(staff[i]);
        staff = (Staff[])list.ToArray(typeof(Staff));


        if (provider_id != -1)
            staff = new Staff[] { StaffDB.GetByID(provider_id) };

        Hashtable staffIDHash = new Hashtable();
        for (int i = 0; i < staff.Length; i++)
            staffIDHash[staff[i].StaffID] = staff[i];


        // get orgs 
        Organisation[] orgs = OrganisationDB.GetAll(false, true, !inc_clinics, !inc_aged_care, true, true);

        Hashtable orgIDHash = new Hashtable();
        for (int i = 0; i < orgs.Length; i++)
            orgIDHash[orgs[i].OrganisationID] = orgs[i];


        // get counts
        string sql = @"

                WITH DateTable
                AS
                (
                    SELECT CAST('" + date_start.ToString("yyyyMMdd") + @"' AS Date) AS [date]
                    UNION ALL
                    SELECT DATEADD(dd, 1, [date])
                    FROM DateTable
                    WHERE DATEADD(dd, 1, [date]) <= cast('" + date_end.ToString("yyyyMMdd") + @"' as Date)
                )
                SELECT dt.[DATE], bk.provider, bk.organisation_id, Count(*) as [count]
                FROM [DateTable] dt
                     INNER JOIN Booking bk ON DATEADD(dd, 0, DATEDIFF(dd, 0, bk.date_start)) = dt.[date]
                     INNER JOIN Staff st ON bk.provider = st.staff_id
                     INNER JOIN Organisation org ON org.organisation_id = bk.organisation_id
                WHERE bk.booking_status_id <> -1 AND bk.booking_type_id = 34
                      " + (organisation_id == -1 ? "" : " AND bk.organisation_id = " + organisation_id) + @"
                      " + (provider_id     == -1 ? "" : " AND bk.provider        = " + provider_id) + @"
                GROUP BY dt.[date], bk.provider, bk.organisation_id
                ORDER BY dt.[date], bk.provider, bk.organisation_id
                OPTION (MAXRECURSION 1000)
        ";

        DataTable dt = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable countHash = new Hashtable();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            DateTime date     = Convert.ToDateTime(dt.Rows[i]["date"]);
            int      staff_id = Convert.ToInt32(dt.Rows[i]["provider"]);
            int      org_id   = Convert.ToInt32(dt.Rows[i]["organisation_id"]);
            int      count    = Convert.ToInt32(dt.Rows[i]["count"]);

            countHash[new Hashtable3DKey(date, staff_id, org_id)] = count;
        }



        //
        // get where staff are allocated to an org regardless of bookings 
        //
        Hashtable regStaffHash2D = RegisterStaffDB.Get2DHashByStaffIDOrgID();  // get by:  hash[new Hashtable2D.Key(staffID, orgID)]



        //
        // put all the results into a table to return
        //

        DataTable tbl = new DataTable();
        tbl.Columns.Add("Date", typeof(string));
        for (int i = 0; i < staff.Length; i++)
            tbl.Columns.Add(staff[i].StaffID.ToString(), typeof(string));

        DataRow row = tbl.NewRow();
        row["Date"] = "";
        for (int i = 0; i < staff.Length; i++)
            row[staff[i].StaffID.ToString()] = staff[i].Person.FullnameWithoutMiddlename;
        tbl.Rows.Add(row);


        for (DateTime curDate = date_start; curDate < date_end; curDate = curDate.AddDays(1))
        {
            if (curDate.DayOfWeek == DayOfWeek.Monday)
                tbl.Rows.Add(tbl.NewRow());

            row = tbl.NewRow();
            row["Date"] = curDate.ToString("dd MMM dddd");

            foreach (Staff s in staff)
            {
                string orgsText = string.Empty;
                int colMod = 0;
                foreach (Organisation org in orgs)
                {
                    if (organisation_id != -1 && org.OrganisationID != organisation_id)
                        continue;

                    RegisterStaff regStaff = regStaffHash2D[new Hashtable2D.Key(s.StaffID, org.OrganisationID)] as RegisterStaff;
                    bool workingThisDay = regStaff != null &&
                        ((curDate.DayOfWeek == DayOfWeek.Sunday    && !regStaff.ExclSun) ||
                         (curDate.DayOfWeek == DayOfWeek.Monday    && !regStaff.ExclMon) ||
                         (curDate.DayOfWeek == DayOfWeek.Tuesday   && !regStaff.ExclTue) ||
                         (curDate.DayOfWeek == DayOfWeek.Wednesday && !regStaff.ExclWed) ||
                         (curDate.DayOfWeek == DayOfWeek.Thursday  && !regStaff.ExclThu) ||
                         (curDate.DayOfWeek == DayOfWeek.Friday    && !regStaff.ExclFri) ||
                         (curDate.DayOfWeek == DayOfWeek.Saturday  && !regStaff.ExclSat));

                    if (inc_bookings)
                    {
                        if (countHash[new Hashtable3DKey(curDate, s.StaffID, org.OrganisationID)] != null && (int)countHash[new Hashtable3DKey(curDate, s.StaffID, org.OrganisationID)] > 0)
                            orgsText += (orgsText.Length == 0 ? "" : " / ") + (colMod++ % 2 == 0 ? "<font color=\"blue\">" : "<font color=\"green\">") + org.Name + "</font>" + "<b>[" + countHash[new Hashtable3DKey(curDate, s.StaffID, org.OrganisationID)] + "]</b>";
                        else if (workingThisDay)
                            orgsText += (orgsText.Length == 0 ? "" : " / ") + (colMod++ % 2 == 0 ? "<font color=\"blue\">" : "<font color=\"green\">") + org.Name + "</font>" + "[0]";
                    }
                    else
                    {
                        if (workingThisDay)
                            orgsText += (orgsText.Length == 0 ? "" : " / ") + (colMod++ % 2 == 0 ? "<font color=\"blue\">" : "<font color=\"green\">") + org.Name + "</font>";
                    }

                }
                row[s.StaffID.ToString()] = orgsText;
            }

            tbl.Rows.Add(row);
        }


        return tbl;
    }

    public static DataTable GetReport_BookingsTotalHours(DateTime date_start, DateTime date_end, bool inc_incomplete_bookings, bool inc_paid_unavailabilties, bool inc_aged_care, bool inc_clinics, int organisation_id = -1, int provider_id = -1)
    {

        // change first day to be start of week
        //int delta = DayOfWeek.Monday - date_start.DayOfWeek;
        //date_start = date_start.AddDays(delta);

        // change last day to be end of week
        //delta = DayOfWeek.Monday - date_end.DayOfWeek;
        //date_end = date_end.AddDays(delta + 6);


        // get staff set as providers and not fired
        Staff[] staff = StaffDB.GetAll();
        ArrayList list = new ArrayList();
        for (int i = 0; i < staff.Length; i++)
            if (staff[i].IsProvider && !staff[i].IsFired)
                list.Add(staff[i]);
        staff = (Staff[])list.ToArray(typeof(Staff));


        if (provider_id != -1)
            staff = new Staff[] { StaffDB.GetByID(provider_id) };

        Hashtable staffIDHash = new Hashtable();
        for (int i = 0; i < staff.Length; i++)
            staffIDHash[staff[i].StaffID] = staff[i];


        // get orgs 
        Organisation[] orgs = OrganisationDB.GetAll(false, true, !inc_clinics, !inc_aged_care, true, true);

        Hashtable orgIDHash = new Hashtable();
        for (int i = 0; i < orgs.Length; i++)
            orgIDHash[orgs[i].OrganisationID] = orgs[i];


        // get counts
        string sql = @"

                WITH DateTable
                AS
                (
                    SELECT CAST('" + date_start.ToString("yyyyMMdd") + @"' AS Date) AS [date]
                    UNION ALL
                    SELECT DATEADD(dd, 1, [date])
                    FROM DateTable
                    WHERE DATEADD(dd, 1, [date]) <= cast('" + date_end.ToString("yyyyMMdd") + @"' as Date)
                )
                SELECT 
					dt.[DATE], 
					sum (DATEDIFF(minute,bk.date_start,bk.date_end)) as [minutes],
					bk.provider, bk.organisation_id
                FROM [DateTable] dt
                     INNER JOIN Booking bk ON DATEADD(dd, 0, DATEDIFF(dd, 0, bk.date_start)) = dt.[date]
                     INNER JOIN Staff st ON bk.provider = st.staff_id
                     INNER JOIN Organisation org ON org.organisation_id = bk.organisation_id
                WHERE (bk.booking_status_id IN (" + (inc_incomplete_bookings ? "0,187" : "187") + @") " + (inc_paid_unavailabilties ? " OR bk.booking_type_id = 36 " : "") + @") AND (bk.booking_type_id = 34 " + (inc_paid_unavailabilties ? " OR bk.booking_type_id = 36 " : "") + @")
                      " + (organisation_id == -1 ? "" : " AND bk.organisation_id = " + organisation_id) + @"
                      " + (provider_id == -1 ? "" : " AND bk.provider        = " + provider_id) + @"
                GROUP BY dt.[date], bk.provider, bk.organisation_id
                ORDER BY dt.[date], bk.provider, bk.organisation_id
                OPTION (MAXRECURSION 1000)
        ";


        DataTable dt = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable minutesHash = new Hashtable();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            DateTime date     = Convert.ToDateTime(dt.Rows[i]["date"]);
            int      staff_id = Convert.ToInt32(dt.Rows[i]["provider"]);
            int      org_id   = Convert.ToInt32(dt.Rows[i]["organisation_id"]);
            int      minutes  = Convert.ToInt32(dt.Rows[i]["minutes"]);

            minutesHash[new Hashtable3DKey(date, staff_id, org_id)] = minutes;
        }




        //
        // put all the results into a table to return
        //

        DataTable tbl = new DataTable();
        tbl.Columns.Add("Date", typeof(string));
        for (int i = 0; i < staff.Length; i++)
            tbl.Columns.Add(staff[i].StaffID.ToString(), typeof(string));

        DataRow row = tbl.NewRow();
        row["Date"] = "";
        for (int i = 0; i < staff.Length; i++)
        {
            int _totalMinutes = GetTotalMinutes(minutesHash, DateTime.MinValue, staff[i].StaffID, -1);
            int _hrs  = _totalMinutes / 60;
            int _mins = _totalMinutes % 60;

            row[staff[i].StaffID.ToString()] = staff[i].Person.FullnameWithoutMiddlename + "<br />" + "<b>" + _hrs + "h " + _mins.ToString().PadLeft(2, '0') + "m</b>";
        }
        tbl.Rows.Add(row);


        for (DateTime curDate = date_start; curDate < date_end; curDate = curDate.AddDays(1))
        {
            if (curDate.DayOfWeek == DayOfWeek.Monday)
                tbl.Rows.Add(tbl.NewRow());


            int _totalMinutes = GetTotalMinutes(minutesHash, curDate, -1, -1);
            int _hrs  = _totalMinutes / 60;
            int _mins = _totalMinutes % 60;

            row = tbl.NewRow();
            row["Date"] = curDate.ToString("dd MMM dddd") + "<br />" + "<b>" + _hrs + "h " + _mins.ToString().PadLeft(2, '0') + "m</b>";

            foreach (Staff s in staff)
            {
                string orgsText = string.Empty;
                int colMod = 0;
                foreach (Organisation org in orgs)
                {
                    if (organisation_id != -1 && org.OrganisationID != organisation_id)
                        continue;

                    if (minutesHash[new Hashtable3DKey(curDate, s.StaffID, org.OrganisationID)] != null && (int)minutesHash[new Hashtable3DKey(curDate, s.StaffID, org.OrganisationID)] > 0)
                    {
                        int totalMinutes = Convert.ToInt32(minutesHash[new Hashtable3DKey(curDate, s.StaffID, org.OrganisationID)]);
                        int hrs  = totalMinutes / 60;
                        int mins = totalMinutes % 60;

                        orgsText += (orgsText.Length == 0 ? "" : " / ") + (colMod++ % 2 == 0 ? "<font color=\"blue\">" : "<font color=\"green\">") + org.Name + "</font>" + "<br /><b>" + hrs + "h " + mins.ToString().PadLeft(2, '0') + "m</b>";
                    }

                }
                row[s.StaffID.ToString()] = orgsText;
            }

            tbl.Rows.Add(row);
        }


        return tbl;
    }
    public static int GetTotalMinutes(Hashtable minutesHash, DateTime date, int staff_id = -1, int org_id = -1)
    {

        int total = 0;
        foreach (DictionaryEntry entry in minutesHash)
        {
            Hashtable3DKey key = (Hashtable3DKey)entry.Key;
            if ((date == DateTime.MinValue || key.Dimension1 == date) && (staff_id == -1 || key.Dimension2 == staff_id) && (org_id == -1 || key.Dimension3 == org_id))
                total += Convert.ToInt32(entry.Value);
        }

        return total;
    }


    public static DataTable GetReport_Receipts(DateTime date_start, DateTime date_end, int organisation_id, int added_by_staff_id, bool inc_medicare, bool inc_dva, bool inc_private, bool inc_reconciled, int receiptPaymentTypeID, int organisation_type_group_id)  // organisation_type_group_id : [clinic=5, aged care = 6]
    {
        string sql = @"

                SELECT  

                        Receipt.receipt_id, Receipt.receipt_payment_type_id, Receipt.invoice_id,Receipt.total, Receipt.amount_reconciled, Receipt.is_failed_to_clear,
                        Receipt.is_overpaid, Receipt.receipt_date_added, Receipt.reconciliation_date, Receipt.staff_id, Receipt.reversed_by, Receipt.reversed_date, Receipt.pre_reversed_amount,
                        ReceiptPaymentType.receipt_payment_type_id as receipt_payment_type_receipt_payment_type_id, ReceiptPaymentType.descr as receipt_payment_type_descr,


                        ReceiptStaff.staff_id as receipt_staff_staff_id,ReceiptStaff.person_id as receipt_staff_person_id,ReceiptStaff.login as receipt_staff_login,ReceiptStaff.pwd as receipt_staff_pwd,ReceiptStaff.staff_position_id as receipt_staff_staff_position_id,
                        ReceiptStaff.field_id as receipt_staff_field_id,ReceiptStaff.is_contractor as receipt_staff_is_contractor,ReceiptStaff.tfn as receipt_staff_tfn,
                        ReceiptStaff.is_fired as receipt_staff_is_fired,ReceiptStaff.costcentre_id as receipt_staff_costcentre_id,
                        ReceiptStaff.is_admin as receipt_staff_is_admin,ReceiptStaff.provider_number as receipt_staff_provider_number,ReceiptStaff.is_commission as receipt_staff_is_commission,ReceiptStaff.commission_percent as receipt_staff_commission_percent,
                        ReceiptStaff.is_stakeholder as receipt_staff_is_stakeholder,ReceiptStaff.is_master_admin as receipt_staff_is_master_admin,ReceiptStaff.is_admin as receipt_staff_is_admin,ReceiptStaff.is_principal as receipt_staff_is_principal,ReceiptStaff.is_provider as receipt_staff_is_provider, ReceiptStaff.is_external as receipt_staff_is_external,
                        ReceiptStaff.staff_date_added as receipt_staff_staff_date_added,ReceiptStaff.start_date as receipt_staff_start_date,ReceiptStaff.end_date as receipt_staff_end_date,ReceiptStaff.comment as receipt_staff_comment,
                        ReceiptStaff.num_days_to_display_on_booking_screen as receipt_staff_num_days_to_display_on_booking_screen,  ReceiptStaff.show_header_on_booking_screen as receipt_staff_show_header_on_booking_screen,
                        ReceiptStaff.bk_screen_field_id as receipt_staff_bk_screen_field_id, ReceiptStaff.bk_screen_show_key as receipt_staff_bk_screen_show_key, ReceiptStaff.enable_daily_reminder_sms as receipt_staff_enable_daily_reminder_sms, ReceiptStaff.enable_daily_reminder_email as receipt_staff_enable_daily_reminder_email,

                        " + PersonDB.GetFields("receipt_staff_person_", "ReceiptStaffPerson") + @",
                        ReceiptStaffPersonTitle.title_id as receipt_staff_person_title_title_id, ReceiptStaffPersonTitle.descr as receipt_staff_person_title_descr,


                        ReceiptReversedBy.staff_id as receipt_reversed_by_staff_id,ReceiptReversedBy.person_id as receipt_reversed_by_person_id,ReceiptReversedBy.login as receipt_reversed_by_login,ReceiptReversedBy.pwd as receipt_reversed_by_pwd,ReceiptReversedBy.staff_position_id as receipt_reversed_by_staff_position_id,
                        ReceiptReversedBy.field_id as receipt_reversed_by_field_id,ReceiptReversedBy.is_contractor as receipt_reversed_by_is_contractor,ReceiptReversedBy.tfn as receipt_reversed_by_tfn,
                        ReceiptReversedBy.is_fired as receipt_reversed_by_is_fired,ReceiptReversedBy.costcentre_id as receipt_reversed_by_costcentre_id,
                        ReceiptReversedBy.is_admin as receipt_reversed_by_is_admin,ReceiptReversedBy.provider_number as receipt_reversed_by_provider_number,ReceiptReversedBy.is_commission as receipt_reversed_by_is_commission,ReceiptReversedBy.commission_percent as receipt_reversed_by_commission_percent,
                        ReceiptReversedBy.is_stakeholder as receipt_reversed_by_is_stakeholder,ReceiptReversedBy.is_master_admin as receipt_reversed_by_is_master_admin,ReceiptReversedBy.is_admin as receipt_reversed_by_is_admin,ReceiptReversedBy.is_principal as receipt_reversed_by_is_principal,ReceiptReversedBy.is_provider as receipt_reversed_by_is_provider, ReceiptReversedBy.is_external as receipt_reversed_by_is_external,
                        ReceiptReversedBy.staff_date_added as receipt_reversed_by_staff_date_added,ReceiptReversedBy.start_date as receipt_reversed_by_start_date,ReceiptReversedBy.end_date as receipt_reversed_by_end_date,ReceiptReversedBy.comment as receipt_reversed_by_comment,
                        ReceiptReversedBy.num_days_to_display_on_booking_screen as receipt_reversed_by_num_days_to_display_on_booking_screen,  ReceiptReversedBy.show_header_on_booking_screen as receipt_reversed_by_show_header_on_booking_screen,
                        ReceiptReversedBy.bk_screen_field_id as receipt_reversed_by_bk_screen_field_id, ReceiptReversedBy.bk_screen_show_key as receipt_reversed_by_bk_screen_show_key, ReceiptReversedBy.enable_daily_reminder_sms as receipt_reversed_by_enable_daily_reminder_sms, ReceiptReversedBy.enable_daily_reminder_email as receipt_reversed_by_enable_daily_reminder_email,

                        " + PersonDB.GetFields("receipt_reversed_by_person_", "ReceiptReversedByPerson") + @",
                        ReceiptReversedByPersonTitle.title_id as receipt_reversed_by_person_title_title_id, ReceiptReversedByPersonTitle.descr as receipt_reversed_by_person_title_descr,

                        -- so can use BookingDB.Load(row) .. to get link to booking page this booking
                        Booking.booking_id as booking_booking_id,Booking.entity_id as booking_entity_id,
                        Booking.date_start as booking_date_start,Booking.date_end as booking_date_end,Booking.organisation_id as booking_organisation_id,
                        Booking.provider as booking_provider,Booking.patient_id as booking_patient_id,Booking.offering_id as booking_offering_id,Booking.booking_type_id as booking_booking_type_id,
                        Booking.booking_status_id as booking_booking_status_id,Booking.booking_unavailability_reason_id as booking_booking_unavailability_reason_id,Booking.added_by as booking_added_by,Booking.date_created as booking_date_created,
                        Booking.booking_confirmed_by_type_id as booking_booking_confirmed_by_type_id,Booking.confirmed_by as booking_confirmed_by,Booking.date_confirmed as booking_date_confirmed,
                        Booking.deleted_by as booking_deleted_by, Booking.date_deleted as booking_date_deleted,
                        Booking.cancelled_by as booking_cancelled_by, Booking.date_cancelled as booking_date_cancelled,
                        Booking.is_patient_missed_appt as booking_is_patient_missed_appt,Booking.is_provider_missed_appt as booking_is_provider_missed_appt,
                        Booking.is_emergency as booking_is_emergency,
                        Booking.need_to_generate_first_letter as booking_need_to_generate_first_letter,Booking.need_to_generate_last_letter as booking_need_to_generate_last_letter,Booking.has_generated_system_letters as booking_has_generated_system_letters,
                        Booking.arrival_time as booking_arrival_time,
                        Booking.sterilisation_code as booking_sterilisation_code,
                        Booking.informed_consent_added_by as booking_informed_consent_added_by, 
                        Booking.informed_consent_date     as booking_informed_consent_date,
                        Booking.is_recurring as booking_is_recurring,Booking.recurring_weekday_id as booking_recurring_weekday_id,
                        Booking.recurring_start_time as booking_recurring_start_time,Booking.recurring_end_time as booking_recurring_end_time,

                        ISNULL(BookingOrganisation.organisation_id, NonBookingOrganisation.organisation_id) as organisation_id,
                        ISNULL(BookingOrganisation.name, NonBookingOrganisation.name) as organisation_name,



                        BookingPatientPerson.firstname    as booking_patient_firstname, 
                        BookingPatientPerson.surname      as booking_patient_surname,
                        NonBookingPatientPerson.firstname as non_booking_patient_firstname, 
                        NonBookingPatientPerson.surname   as non_booking_patient_surname,


                        Invoice.payer_organisation_id     as inv_payer_organisation_id,
                        Invoice.payer_patient_id          as inv_payer_patient_id,

                        payer_organisation.name           as inv_payer_organisation_name, 
                        payer_patient_person.firstname    as inv_payer_patient_person_firstname, 
                        payer_patient_person.surname      as inv_payer_patient_person_surname



                FROM 
                        Receipt
                        INNER      JOIN ReceiptPaymentType                  ON Receipt.receipt_payment_type_id = ReceiptPaymentType.receipt_payment_type_id 

                        LEFT OUTER JOIN Staff  ReceiptStaff                 ON Receipt.staff_id            = ReceiptStaff.staff_id
                        LEFT OUTER JOIN Person ReceiptStaffPerson           ON ReceiptStaff.person_id      = ReceiptStaffPerson.person_id
                        LEFT OUTER JOIN Title  ReceiptStaffPersonTitle      ON ReceiptStaffPerson.title_id = ReceiptStaffPersonTitle.title_id

                        INNER JOIN		Invoice                             ON Receipt.invoice_id = Invoice.invoice_id
                        LEFT OUTER JOIN Booking                             ON Booking.booking_id = Invoice.booking_id

                        LEFT OUTER JOIN Organisation BookingOrganisation    ON BookingOrganisation.organisation_id    = Booking.organisation_id
                        LEFT OUTER JOIN Organisation NonBookingOrganisation ON NonBookingOrganisation.organisation_id = Invoice.non_booking_invoice_organisation_id

                        LEFT OUTER JOIN Staff  ReceiptReversedBy            ON ReceiptReversedBy.staff_id        = Receipt.reversed_by
                        LEFT OUTER JOIN Person ReceiptReversedByPerson      ON ReceiptReversedByPerson.person_id = ReceiptReversedBy.person_id
                        LEFT OUTER JOIN Title  ReceiptReversedByPersonTitle ON ReceiptReversedByPerson.title_id  = ReceiptReversedByPersonTitle.title_id

                        LEFT JOIN Patient BookingPatient                    ON BookingPatient.patient_id         = Booking.patient_id
                        LEFT JOIN Person  BookingPatientPerson              ON BookingPatientPerson.person_id    = BookingPatient.person_id
                        LEFT JOIN Patient NonBookingPatient                 ON NonBookingPatient.patient_id      = Invoice.payer_patient_id
                        LEFT JOIN Person  NonBookingPatientPerson           ON NonBookingPatientPerson.person_id = NonBookingPatient.person_id

                        LEFT OUTER JOIN Organisation  payer_organisation    ON Invoice.payer_organisation_id     = payer_organisation.organisation_id
                        LEFT OUTER JOIN Patient       payer_patient         ON Invoice.payer_patient_id          = payer_patient.patient_id
                        LEFT OUTER JOIN Person        payer_patient_person  ON payer_patient_person.person_id    = payer_patient.person_id


                WHERE   1=1 " + 
                        (date_start        != DateTime.MinValue ? " AND (Receipt.receipt_date_added          >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "')" + Environment.NewLine : "") + 
                        (date_end          != DateTime.MinValue ? " AND (Receipt.receipt_date_added          <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss")   + "')" + Environment.NewLine : "") + 
                        (added_by_staff_id != -1                ? " AND (Receipt.staff_id = " + added_by_staff_id + ")" + Environment.NewLine : "") + 
                        (organisation_id   != -1                ? " AND (BookingOrganisation.organisation_id  =  " + organisation_id + " OR NonBookingOrganisation.organisation_id = " + organisation_id + ")" + Environment.NewLine : "") +
                        (!inc_reconciled                        ? " AND Receipt.amount_reconciled < Receipt.total " : "") +
                        (receiptPaymentTypeID  != -1            ? " AND Receipt.receipt_payment_type_id = " + receiptPaymentTypeID : "");
                    


        if (inc_private)
        {
            if (!inc_medicare && !inc_dva)
                sql += " AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id NOT IN (-1,-2)) ";
            else if (!inc_medicare)
                sql += " AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id <> -1) ";
            else if (!inc_dva)
                sql += " AND (Invoice.payer_organisation_id IS NULL OR Invoice.payer_organisation_id <> -2) ";
        }
        else // if (!inc_private)
        {
            if (inc_medicare && inc_dva)
                sql += " AND Invoice.payer_organisation_id IN (-1,-2) ";
            else if (inc_medicare)
                sql += " AND Invoice.payer_organisation_id = -1 ";
            else if (inc_dva)
                sql += " AND Invoice.payer_organisation_id = -2 ";
            else
                ; // sql += " AND 1 <> 1 ";
        }


        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public struct Hashtable3DKey
    {
        public readonly DateTime Dimension1;
        public readonly int      Dimension2;
        public readonly int      Dimension3;
        public Hashtable3DKey(DateTime p1, int p2, int p3)
        {
            Dimension1 = p1;
            Dimension2 = p2;
            Dimension3 = p3;
        }
        // Equals and GetHashCode ommitted
    }


    // Gets where a service was X months ago (as set in the offering table) and they have not had this service since then - to print reminder letters
    public static Booking[] GetWhenLastServiceFromXMonthsAgoToGenerageReminderLetter(int offering_id, DateTime curDate, int nMonthsAgo)
    {

        /*  -- working prototype to check below one works
            SELECT b.* 
            FROM   booking b
                   LEFT JOIN Patient      p on b.patient_id = p.patient_id
                   LEFT JOIN Organisation o on o.organisation_id = b.organisation_id
	
            WHERE  b.booking_id = ( 
	
                -- Get highest booking id from list where we got only most recent bookings 
                -- Need to get highest booking_id for when there are 2 bookings on the same day to remove duplicates
                SELECT MAX(booking_id) FROM (
                    SELECT b2.*
                    FROM   Booking b2
                    WHERE  b2.offering_id = 28 
                      -- Get most recent booking for this offering
                      and b2.date_start = ( SELECT MAX(b3.date_start) FROM   Booking b3 WHERE  b3.patient_id = b2.patient_id )
                ) b4
                WHERE b4.patient_id = b.patient_id
		
            )

            AND b.booking_status_id = 34 AND p.is_deleted = 0 AND p.is_deceased = 0 AND o.is_deleted = 0 and b.
  
            -- need to also get say 31st jan if it is now 30apr
            -- if first of month:
            -- and date of yeterday was < 31
            -- get date in (3mo ago, and from 3mo minus 1 day ago, 2 days? etc?)

            -- Also need to ignore when NOT first of month, but gets first of month
            -- eg 31st May and 1 month ago = 1st May (cuz no  31 Apr
            -- ie look for when currently NOT first of money, but new date X months ago IS first of month

            AND (SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, b.date_start))) IN ('2013-06-12')
         */


        string dateStringsCommaSep = string.Empty;


        // Get (Add/remove incorrect/missing) days
        // Eg 30 Apr, then 1May -- 1 month ago misses 31 March totally
        // Eg 31 Mar -- a month ago has no 31st, so dont add any date

        // if first of month and last of last month was 30th, need to get 31st of x months ago
        if (curDate.Day == 1)
        {
            DateTime yesterday = curDate.AddDays(-1);
            DateTime monthsAgoYesterday = yesterday.AddMonths(-1 * nMonthsAgo);
            for (DateTime tmp = monthsAgoYesterday.AddDays(1); tmp.Month == monthsAgoYesterday.Month; tmp = tmp.AddDays(1))
                dateStringsCommaSep += (dateStringsCommaSep.Length == 0 ? "" : ", ") + "'" + tmp.ToString("yyyy-MM-dd") + "'";
        }

        // only add today x months ago if that month has this day in it (eg if 31 May, then 1 month ago does not return 31 Apr since Apr has 30 days)
        DateTime monthsAgo = curDate.AddMonths(-1 * nMonthsAgo);
        if (curDate.Day == monthsAgo.Day)
            dateStringsCommaSep += (dateStringsCommaSep.Length == 0 ? "" : ", ") + "'" + monthsAgo.ToString("yyyy-MM-dd") + "'";

        // End get days


        if (dateStringsCommaSep.Length == 0)
            return new Booking[]{};


        string sql = JoinedSQL() + @"

            AND booking.booking_id = ( 
	
	            -- Get highest booking id from list where we got only most recent bookings 
	            -- Need to get highest booking_id for when there are 2 bookings on the same day to remove duplicates
	            SELECT MAX(booking_id) FROM (
		            SELECT b2.*
		            FROM   Booking b2
		            WHERE  b2.offering_id = " + offering_id + @" 
		              -- Get most recent booking for this offering
		              and b2.date_start = ( SELECT MAX(b3.date_start) FROM   Booking b3 WHERE  b3.patient_id = b2.patient_id )
	            ) b4
	            WHERE b4.patient_id = booking.patient_id
		
            )

            AND patient.is_deleted = 0 AND patient.is_deceased = 0 AND org.is_deleted = 0
  
            -- need to also get say 31st jan if it is now 30apr
            -- if first of month:
            -- and date of yeterday was < 31
            -- get date in (3mo ago, and from 3mo minus 1 day ago, 2 days? etc?)

            -- Also need to ignore when NOT first of month, but gets first of month
            -- eg 31st May and 1 month ago = 1st May (cuz no  31 Apr
            -- ie look for when currently NOT first of money, but new date X months ago IS first of month

            AND (SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, booking.date_start))) IN (" + dateStringsCommaSep + @")";


        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Booking[] bookings = new Booking[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            bookings[i] = LoadFull(tbl.Rows[i]);

        return bookings;
    }


    public static Hashtable GetChangeHistoryCountHash(Booking[] bookings)
    {
        int[] booking_ids = new int[bookings.Length];
        for(int i=0; i<bookings.Length; i++)
            booking_ids[i] = bookings[i].BookingID;

        return GetChangeHistoryCountHash(booking_ids);
    }
    public static Hashtable GetChangeHistoryCountHash(int[] booking_ids)
    {
        if (booking_ids == null || booking_ids.Length == 0)
            return new Hashtable();

        string sql = @"SELECT booking_id, COUNT(*) as count
                       FROM BookingChangeHistory
                       WHERE booking_id IN (" + string.Join(",", booking_ids) + @")
                       GROUP BY booking_id";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
            hash[(int)tbl.Rows[i]["booking_id"]] = (int)tbl.Rows[i]["count"];

        return hash;
    }


    protected enum DBAction { Insert, Update, Delete };
    protected static void SendPTSelfBookingAlertEmail(int booking_id, DBAction dbAction, DateTime originalBookingStart, DateTime originalBookingEnd)
    {
        if (Utilities.IsDev()) return;
        if (!UserView.GetInstance().IsPatient) return;

        Booking booking = BookingDB.GetByID(booking_id);
        if (booking.BookingTypeID != 34) return;


        string emailAlertTo = ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["AdminAlertEmail_To"].Value; // "eli@elipollak.com"

        if (dbAction == DBAction.Insert)
        {
            // send pt alert email
            if (((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["EnableAlert_BookingAddedByPT"].Value == "1")
            {
                string subjectText = @"Notification that a PATIENT has added/moved/deleted their own booking";
                string mailText = @"This is an administrative email to notify you that a <b>patient</b> has <b>added</b> a booking for them self.

<u>Patient making the appointment</u>
" + booking.Patient.Person.FullnameWithoutMiddlename + " [ID: " + booking.Patient.PatientID + "]" + @"

<u>Booking details</u>
<table border=""0"" cellpadding=""2"" cellspacing=""2""><tr><td>Booking ID:</td><td>" + booking.BookingID + @"</td></tr><tr><td>Booking Date:</td><td>" + booking.DateStart.ToString("d MMM, yyyy") + " " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + "-" + booking.DateEnd.ToString("h:mm") + (booking.DateEnd.Hour < 12 ? "am" : "pm") + @"</td></tr><tr><td>Organisation:</td><td>" + booking.Organisation.Name + @"</td></tr><tr><td>Provider:</td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr><tr><td>Patient:</td><td>" + (booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename + " [ID:" + booking.Patient.PatientID + "]") + @"</td></tr><tr><td>Status:</td><td>" + booking.BookingStatus.Descr + @"</td></tr></table>


Please note that you can switch off these email alerts in the website settings page.

Regards,
Mediclinic
";
                Emailer.AsyncSimpleEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    emailAlertTo,
                    subjectText,
                    mailText.Replace(Environment.NewLine, "<br />"),
                    true,
                    null);
            }
        }

        if (dbAction == DBAction.Update)
        {
            // send pt alert email
            if (((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["EnableAlert_BookingEditedByPT"].Value == "1")
            {
                string subjectText = @"Notification that a PATIENT has added/moved/deleted their own booking";
                string mailText = @"This is an administrative email to notify you that a <b>PATIENT</b> has <b>moved</b> a booking for them self.

<u>Patient moving the appointment</u>
" + booking.Patient.Person.FullnameWithoutMiddlename + " [ID: " + booking.Patient.PatientID + "]" + @"

<u>Booking details</u>
<table border=""0"" cellpadding=""2"" cellspacing=""2""><tr><td>Booking ID:</td><td>" + booking.BookingID + @"</td></tr><tr><td>Original Booking Date:</td><td>" + originalBookingStart.ToString("d MMM, yyyy") + " " + originalBookingStart.ToString("h:mm") + (originalBookingStart.Hour < 12 ? "am" : "pm") + "-" + originalBookingEnd.ToString("h:mm") + (originalBookingEnd.Hour < 12 ? "am" : "pm") + @"</td></tr><tr><td>New Booking Date:</td><td>" + booking.DateStart.ToString("d MMM, yyyy") + " " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + "-" + booking.DateEnd.ToString("h:mm") + (booking.DateEnd.Hour < 12 ? "am" : "pm") + @"</td></tr><tr><td>Organisation:</td><td>" + booking.Organisation.Name + @"</td></tr><tr><td>Provider:</td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr><tr><td>Patient:</td><td>" + (booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename + " [ID:" + booking.Patient.PatientID + "]") + @"</td></tr><tr><td>Status:</td><td>" + booking.BookingStatus.Descr + @"</td></tr></table>


Please note that you can switch off these email alerts in the website settings page.

Regards,
Mediclinic
";
                Emailer.AsyncSimpleEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    emailAlertTo,
                    subjectText,
                    mailText.Replace(Environment.NewLine, "<br />"),
                    true,
                    null);
            }
        }


        if (dbAction == DBAction.Delete)
        {
            // send pt alert email
            if (((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["EnableAlert_BookingDeletedByPT"].Value == "1")
            {
                string subjectText = @"Notification that a PATIENT has added/moved/deleted their own booking";
                string mailText = @"This is an administrative email to notify you that a <b>patient</b> has <b>deleted</b> their booking.

<u>Patient deleting the appointment</u>
" + booking.Patient.Person.FullnameWithoutMiddlename + " [ID: " + booking.Patient.PatientID + "]" + @"

<u>Booking details</u>
<table border=""0"" cellpadding=""2"" cellspacing=""2""><tr><td>Booking ID:</td><td>" + booking.BookingID + @"</td></tr><tr><td>Booking Date:</td><td>" + booking.DateStart.ToString("d MMM, yyyy") + " " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + "-" + booking.DateEnd.ToString("h:mm") + (booking.DateEnd.Hour < 12 ? "am" : "pm") + @"</td></tr><tr><td>Organisation:</td><td>" + booking.Organisation.Name + @"</td></tr><tr><td>Provider:</td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr><tr><td>Patient:</td><td>" + (booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename + " [ID:" + booking.Patient.PatientID + "]") + @"</td></tr><tr><td>Status:</td><td>" + booking.BookingStatus.Descr + @"</td></tr></table>


Please note that you can switch off these email alerts in the website settings page.

Regards,
Mediclinic
";
                Emailer.AsyncSimpleEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    emailAlertTo,
                    subjectText,
                    mailText.Replace(Environment.NewLine, "<br />"),
                    true,
                    null);
            }
        }


        bool booking_is_within_3_days = booking.DateStart.Date >= DateTime.Today && booking.DateStart.Date <= DateTime.Today.AddDays(3);
        if (dbAction == DBAction.Delete && booking_is_within_3_days)  
        {
            // send email to PROVIDER
            if (((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["EnableAlert_BookingDeletedByPT"].Value == "1")
            {

                // Get Provider Email
                string[] emails = ContactDB.GetEmailsByEntityID(booking.Provider.Person.EntityID);

                string subjectText = @"Notification that your PATIENT has deleted their booking on " + booking.DateStart.ToString("d MMM, yyyy") + " at " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + " " + booking.Organisation.Name;
                string mailText = @"This is an administrative email to notify you that a <b>patient</b> has <b>deleted</b> their booking.

<u>Patient deleting the appointment</u>
" + booking.Patient.Person.FullnameWithoutMiddlename + " [ID: " + booking.Patient.PatientID + "]" + @"

<u>Booking details</u>
<table border=""0"" cellpadding=""2"" cellspacing=""2""><tr><td>Booking ID:</td><td>" + booking.BookingID + @"</td></tr><tr><td>Booking Date:</td><td><b>" + booking.DateStart.ToString("d MMM, yyyy") + " " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + "-" + booking.DateEnd.ToString("h:mm") + (booking.DateEnd.Hour < 12 ? "am" : "pm") + @"</b></td></tr><tr><td>Organisation:</td><td><b>" + booking.Organisation.Name + @"</b></td></tr><tr><td>Provider:</td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr><tr><td>Patient:</td><td><b>" + (booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename + " [ID:" + booking.Patient.PatientID + "]") + @"</b></td></tr><tr><td>Status:</td><td>" + booking.BookingStatus.Descr + @"</td></tr></table>


Regards,
Mediclinic
";
                if (emails.Length > 0)
                    Emailer.AsyncSimpleEmail(
                        string.Join(",", emails),
                        ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                        emailAlertTo,
                        subjectText,
                        mailText.Replace(Environment.NewLine, "<br />"),
                        true,
                        null);
            }
        }

    }


    public static Booking Load(DataRow row, string prefix = "", bool inc_note_count = true, bool inc_inv_count = true)
    {
        return new Booking(
            Convert.ToInt32(row[prefix + "booking_id"]),
            Convert.ToInt32(row[prefix + "entity_id"]),
            Convert.ToDateTime(row[prefix + "date_start"]),
            row[prefix + "date_end"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_end"]),
            row[prefix + "organisation_id"] == DBNull.Value ?  0 : Convert.ToInt32(row[prefix + "organisation_id"]),
            row[prefix + "provider"]        == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "provider"]),
            row[prefix + "patient_id"]      == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "patient_id"]),
            row[prefix + "offering_id"]     == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "offering_id"]),
            Convert.ToInt32(row[prefix + "booking_type_id"]),
            row[prefix + "booking_status_id"] == DBNull.Value ? -2                : Convert.ToInt32(row[prefix + "booking_status_id"]),
            row[prefix + "booking_unavailability_reason_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "booking_unavailability_reason_id"]),
            row[prefix + "added_by"]        == DBNull.Value ? -1                  : Convert.ToInt32(row[prefix + "added_by"]),
            row[prefix + "date_created"]    == DBNull.Value ? DateTime.MinValue   : Convert.ToDateTime(row[prefix + "date_created"]),
            row[prefix + "booking_confirmed_by_type_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "booking_confirmed_by_type_id"]),
            row[prefix + "confirmed_by"]      == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "confirmed_by"]),
            row[prefix + "date_confirmed"]    == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_confirmed"]),
            row[prefix + "deleted_by"]        == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "deleted_by"]),
            row[prefix + "date_deleted"]      == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_deleted"]),
            row[prefix + "cancelled_by"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "cancelled_by"]),
            row[prefix + "date_cancelled"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_cancelled"]),
            Convert.ToBoolean(row[prefix + "is_patient_missed_appt"]),
            Convert.ToBoolean(row[prefix + "is_provider_missed_appt"]),
            Convert.ToBoolean(row[prefix + "is_emergency"]),
            Convert.ToBoolean(row[prefix + "need_to_generate_first_letter"]),
            Convert.ToBoolean(row[prefix + "need_to_generate_last_letter"]),
            Convert.ToBoolean(row[prefix + "has_generated_system_letters"]),
            row[prefix + "arrival_time"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "arrival_time"]),
            Convert.ToString(row[prefix + "sterilisation_code"]),
            row[prefix + "informed_consent_added_by"] == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "informed_consent_added_by"]),
            row[prefix + "informed_consent_date"]     == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "informed_consent_date"]),
            Convert.ToBoolean(row[prefix + "is_recurring"]),
            row[prefix + "recurring_weekday_id"]      == DBNull.Value ? DayOfWeek.Sunday  : WeekDayDB.GetDayOfWeek(Convert.ToInt32(row[prefix + "recurring_weekday_id"])),
            row[prefix + "recurring_start_time"]      == DBNull.Value ? TimeSpan.Zero     : (TimeSpan)(row[prefix + "recurring_start_time"]),
            row[prefix + "recurring_end_time"]        == DBNull.Value ? TimeSpan.Zero     : (TimeSpan)(row[prefix + "recurring_end_time"]),

            inc_note_count ? Convert.ToInt32(row[prefix + "note_count"]) : -1,
            inc_inv_count  ? Convert.ToInt32(row[prefix + "inv_count"])  : -1
        );
    }

    public static Booking LoadFull(DataRow row)
    {
        Booking b = Load(row, "booking_");

        if (row["booking_status_booking_status_id"] != DBNull.Value)
            b.BookingStatus = IDandDescrDB.Load(row, "booking_status_booking_status_id", "booking_status_descr");
        if (row["booking_confirmed_by_type_booking_confirmed_by_type_id"] != DBNull.Value)
            b.BookingConfirmedByType = IDandDescrDB.Load(row, "booking_confirmed_by_type_booking_confirmed_by_type_id", "booking_confirmed_by_type_descr");
        if (row["booking_unavailability_reason_booking_unavailability_reason_id"] != DBNull.Value)
            b.BookingUnavailabilityReason = IDandDescrDB.Load(row, "booking_unavailability_reason_booking_unavailability_reason_id", "booking_unavailability_reason_descr");

        if (row["organisation_organisation_id"] != DBNull.Value)
        {
            b.Organisation = OrganisationDB.Load(row, "organisation_");
            b.Organisation.OrganisationType = OrganisationTypeDB.Load(row, "org_type_");
            b.Organisation.OrganisationType.OrganisationTypeGroup = IDandDescrDB.Load(row, "typegroup_organisation_type_group_id", "typegroup_descr");
        }
        if (row["provider_staff_id"] != DBNull.Value)
            b.Provider = StaffDB.Load(row, "provider_");
        if (row["provider_staff_id"] != DBNull.Value)
        {
            b.Provider.Person = PersonDB.Load(row, "person_provider_");
            b.Provider.Person.Title = IDandDescrDB.Load(row, "title_provider_title_id", "title_provider_descr");
        }
        if (row["patient_patient_id"] != DBNull.Value)
            b.Patient = PatientDB.Load(row, "patient_");
        if (row["patient_patient_id"] != DBNull.Value)
        {
            b.Patient.Person = PersonDB.Load(row, "person_patient_");
            b.Patient.Person.Title = IDandDescrDB.Load(row, "title_patient_title_id", "title_patient_descr");
        }
        if (row["offering_offering_id"] != DBNull.Value)
            b.Offering = OfferingDB.Load(row, "offering_");
        if (row["offeringfield_field_id"] != DBNull.Value)
            b.Offering.Field = IDandDescrDB.Load(row, "offeringfield_field_id", "offeringfield_descr");
        
        if (row["added_by_staff_id"] != DBNull.Value)
            b.AddedBy = StaffDB.Load(row, "added_by_");
        if (row["person_added_by_person_id"] != DBNull.Value)
        {
            b.AddedBy.Person = PersonDB.Load(row, "person_added_by_");
            b.AddedBy.Person.Title = IDandDescrDB.Load(row, "title_added_by_title_id", "title_added_by_descr");
        }
        if (row["confirmed_by_staff_id"] != DBNull.Value)
            b.ConfirmedBy = StaffDB.Load(row, "confirmed_by_");
        if (row["person_confirmed_by_person_id"] != DBNull.Value)
        {
            b.ConfirmedBy.Person = PersonDB.Load(row, "person_confirmed_by_");
            b.ConfirmedBy.Person.Title = IDandDescrDB.Load(row, "title_confirmed_by_title_id", "title_confirmed_by_descr");
        }
        if (row["deleted_by_staff_id"] != DBNull.Value)
            b.DeletedBy = StaffDB.Load(row, "deleted_by_");
        if (row["person_deleted_by_person_id"] != DBNull.Value)
        {
            b.DeletedBy.Person.Title = IDandDescrDB.Load(row, "title_deleted_by_title_id", "title_deleted_by_descr");
            b.DeletedBy.Person = PersonDB.Load(row, "person_deleted_by_");
        }
        if (row["cancelled_by_staff_id"] != DBNull.Value)
            b.CancelledBy = StaffDB.Load(row, "cancelled_by_");
        if (row["person_cancelled_by_person_id"] != DBNull.Value)
        {
            b.CancelledBy.Person.Title = IDandDescrDB.Load(row, "title_cancelled_by_title_id", "title_cancelled_by_descr");
            b.CancelledBy.Person = PersonDB.Load(row, "person_cancelled_by_");
        }

        return b;
    }

}