using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

public class InvoiceDB
{

    public static void Delete(int invoice_id)
    {
        try
        {
            Invoice inv = GetByID(invoice_id);
            if (inv != null)
            {
                DBBase.ExecuteNonResult("DELETE FROM Invoice WHERE invoice_id = " + invoice_id.ToString());
                if (EntityDB.NumForeignKeyDependencies(inv.EntityID) == 0)
                    EntityDB.Delete(inv.EntityID, false);
            }
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int invoice_type_id, int booking_id, int payer_organisation_id, int payer_patient_id, int non_booking_invoice_organisation_id, string healthcare_claim_number, string message, int staff_id, int site_id, decimal total, decimal gst, bool is_paid, bool is_refund, bool is_batched, DateTime last_date_emailed)
    {
        if (healthcare_claim_number.Length > 0 && !OrganisationDB.IsInsuranceCompanyOrg(payer_organisation_id) && ClaimNumberExists(healthcare_claim_number, DateTime.Now))
            throw new UniqueConstraintException("Claim Number Exists : " + healthcare_claim_number);

        healthcare_claim_number = healthcare_claim_number.Replace("'", "''");
        message = message.Replace("'", "''");
        
        int entityID = EntityDB.Insert();
        string sql = "INSERT INTO Invoice (entity_id,invoice_type_id,booking_id,payer_organisation_id,payer_patient_id,non_booking_invoice_organisation_id,healthcare_claim_number,reject_letter_id,message,staff_id,site_id,invoice_date_added,total,gst,is_paid,is_refund,is_batched,reversed_by,reversed_date, last_date_emailed) VALUES (" + entityID + "," + invoice_type_id + "," + "" + (booking_id == -1 ? "NULL" : booking_id.ToString()) + "," + "" + (payer_organisation_id == 0 ? "NULL" : payer_organisation_id.ToString()) + "," + "" + (payer_patient_id == -1 ? "NULL" : payer_patient_id.ToString()) + "," + "" + (non_booking_invoice_organisation_id == 0 ? "NULL" : non_booking_invoice_organisation_id.ToString()) + "," + "'" + healthcare_claim_number + "'," + "NULL,'" + message + "'," + staff_id + "," + "" + (site_id == -1 ? "NULL" : site_id.ToString()) + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + total + "," + "" + gst + "," + (is_paid ? "1," : "0,") + (is_refund ? "1," : "0,") + (is_batched ? "1," : "0,") + "NULL" + "," + "NULL" + "," + (last_date_emailed == DateTime.MinValue ? "null" : "'" + last_date_emailed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static void Update(int invoice_id, int invoice_type_id, int booking_id, int payer_organisation_id, int payer_patient_id, int non_booking_invoice_organisation_id, string healthcare_claim_number, int reject_letter_id, string message, decimal total, decimal gst, bool is_paid, bool is_refund, bool is_batched, int reversed_by, DateTime reversed_date, DateTime last_date_emailed)
    {
        /*
         * Not needed any more since a claim number is used over multiple invoices - plus users can not update claim numbers anyway.
         * 
        Invoice inv = InvoiceDB.GetByID(invoice_id);
        if (healthcare_claim_number.Length > 0 && ClaimNumberExists(healthcare_claim_number, inv.InvoiceDateAdded, inv.InvoiceID))
            throw new UniqueConstraintException("Claim Number Exists : " + healthcare_claim_number);
         */

        healthcare_claim_number = healthcare_claim_number.Replace("'", "''");

        string sql = "UPDATE Invoice SET invoice_type_id = " + invoice_type_id + ",booking_id = " + (booking_id == -1 ? "NULL" : booking_id.ToString()) + ",payer_organisation_id = " + (payer_organisation_id == 0 ? "NULL" : payer_organisation_id.ToString()) + ",payer_patient_id = " + (payer_patient_id == -1 ? "NULL" : payer_patient_id.ToString()) + ",non_booking_invoice_organisation_id = " + (non_booking_invoice_organisation_id == 0 ? "NULL" : non_booking_invoice_organisation_id.ToString()) + ",healthcare_claim_number = '" + healthcare_claim_number + "',reject_letter_id = " + (reject_letter_id == -1 ? "NULL" : reject_letter_id.ToString()) + ",message = '" + message + "',total = " + total + ",gst = " + gst + ",is_paid = " + (is_paid ? "1," : "0,") + "is_refund = " + (is_refund ? "1," : "0,") + "is_batched = " + (is_batched ? "1," : "0,") + "reversed_by = " + (reversed_by == -1 ? "NULL" : reversed_by.ToString()) + ",reversed_date = " + (reversed_date == DateTime.MinValue ? "NULL" : "'" + reversed_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ", last_date_emailed = " + (last_date_emailed == DateTime.MinValue ? "NULL" : "'" + last_date_emailed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE invoice_id = " + invoice_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void Reverse(int invoice_id, int reversed_by)
    {
        Invoice invoice = InvoiceDB.GetByID(invoice_id);
        if (invoice == null)
            throw new CustomMessageException("Invalid invoice id :" + invoice_id);
        if (invoice.IsReversed)
            throw new CustomMessageException("Invoice already reversed");
        if (invoice.ReceiptsTotal > 0)
            throw new CustomMessageException("Invoice has receipts totalling more than zero. Reverse the non-zero receipts first");

        if (invoice.TotalDue > 0)
            CreditNoteDB.Insert(invoice_id, invoice.TotalDue, "Auto generated for inv reversal.", reversed_by);

        // set total=0, set not overpaid, set who and when it was reversed, and original amount
        string sql = "UPDATE Invoice SET is_paid = 1, reversed_by = " + reversed_by + ",reversed_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE invoice_id = " + invoice_id.ToString();



        InvoiceLine[] invoiceLines = InvoiceLineDB.GetByInvoiceID(invoice.InvoiceID);


        // update stock

        foreach (InvoiceLine invoiceLine in invoiceLines)
        {
            // if on order
            //    - if filled, count back up, else dont count as no stock has been moved anywhere
            // delete order as we are reversing the invoice
            //
            // if not on order, then count up as stock has been moved
            //
            if (invoiceLine.OfferingOrder != null)
            {
                OfferingOrder o = OfferingOrderDB.GetByID(invoiceLine.OfferingOrder.OfferingOrderID);

                bool isFilled = o.DateFilled != DateTime.MinValue;
                if (isFilled) // count back up
                    StockDB.UpdateAndCheckWarning(o.Organisation.OrganisationID, o.Offering.OfferingID, -1 * o.Quantity);

                InvoiceLineDB.UpdateRemoveOfferingOrder(invoiceLine.InvoiceLineID, o.OfferingOrderID);
                OfferingOrderDB.Delete(o.OfferingOrderID);
            }
            else
            {
                if (invoice.Booking != null)
                    StockDB.UpdateAndCheckWarning(invoice.Booking.Organisation.OrganisationID, invoiceLine.Offering.OfferingID, -1 * (int)invoiceLine.Quantity);
                else if (invoice.NonBookinginvoiceOrganisation != null)
                    StockDB.UpdateAndCheckWarning(invoice.NonBookinginvoiceOrganisation.OrganisationID, invoiceLine.Offering.OfferingID, -1 * (int)invoiceLine.Quantity);
            }
        }


        // if
        // - medicare invoice (not even dva as they are unlimited and dont keep count)
        // - patient has medicare card
        // - medicare card has epc card
        // - epc date-referreral-signed <= booking treatment date
        // - patient has referrer
        //
        // then --> increment the epc count --> for that field

        if (invoice.PayerOrganisation != null && invoice.PayerOrganisation.OrganisationID == -1)
        {
            bool isClinic   = invoice.InvoiceType.ID == 107;
            bool isAgedCare = invoice.InvoiceType.ID == 363;

            Booking       booking      = BookingDB.GetByID(invoice.Booking.BookingID);

            foreach (InvoiceLine invoiceLine in invoiceLines)
            {
                HealthCard hc = HealthCardDB.GetActiveByPatientID( isClinic ? booking.Patient.PatientID : invoiceLine.Patient.PatientID );

                if (hc == null || hc.Organisation.OrganisationID != -1 || !hc.HasEPC() || hc.DateReferralSigned.Date > booking.DateStart.Date)
                    continue;

                invoiceLine.Offering = OfferingDB.GetByID(invoiceLine.Offering.OfferingID);
                if (isAgedCare) invoiceLine.Offering.Field = booking.Provider.Field;
                foreach (HealthCardEPCRemaining epcRemaining in HealthCardEPCRemainingDB.GetByHealthCardID(hc.HealthCardID))
                    if (epcRemaining.Field.ID == invoiceLine.Offering.Field.ID)
                        HealthCardEPCRemainingDB.UpdateNumServicesRemaining(epcRemaining.HealthCardEpcRemainingID, epcRemaining.NumServicesRemaining + 1);
            }
        }


        DBBase.ExecuteNonResult(sql);
    }
    public static void UnReverse(int invoice_id)  // If exceptions thrown when reversing multiple invoices of a booking, need to un-reverse it back to being in use
    {
        Invoice invoice = InvoiceDB.GetByID(invoice_id);
        if (invoice == null)
            throw new CustomMessageException("Invalid invoice id :" + invoice_id);

        // set total=0, set not overpaid, set who and when it was reversed, and original amount
        string sql = "UPDATE Invoice SET reversed_by = NULL, reversed_date = NULL WHERE invoice_id = " + invoice_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    private static bool ClaimNumberExists(string healthcare_claim_number, DateTime date, int invoice_id_exception = -1)
    {
        string sql = "SELECT count(*) FROM Invoice WHERE invoice_date_added > '" + date.AddYears(-2).ToString("yyyy-MM-dd 00:00:00") + "' AND healthcare_claim_number = '" + healthcare_claim_number + "'" + (invoice_id_exception == -1 ? "" : "AND invoice_id <> " + invoice_id_exception);
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }
    public static void UpdateIsPaid(string DB, int invoice_id, bool is_paid, bool clearRejLetter = false)
    {
        string sql = "UPDATE Invoice SET is_paid = " + (is_paid ? "1" : "0") + (clearRejLetter ? ", reject_letter_id = NULL " : "") + " WHERE invoice_id = " + invoice_id.ToString();
        DBBase.ExecuteNonResult(sql, DB);
    }
    public static void UpdateIsRefund(int invoice_id, bool is_refund)
    {
        string sql = "UPDATE Invoice SET is_refund = " + (is_refund ? "1" : "0") + " WHERE invoice_id = " + invoice_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateRejectLetterID(int invoice_id, int letter_id)
    {
        string sql = "UPDATE Invoice SET reject_letter_id = " + (letter_id == -1 ? "NULL" : letter_id.ToString()) + " WHERE invoice_id = " + invoice_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateLastDateEmailed(int invoice_id, DateTime last_date_emailed)
    {
        string sql = "UPDATE Invoice SET last_date_emailed = " + (last_date_emailed == DateTime.MinValue ? "null" : "'" + last_date_emailed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE invoice_id = " + invoice_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void SetClaimNumber(int invoice_id, string healthcare_claim_number)
    {
        healthcare_claim_number = healthcare_claim_number.Replace("'", "''"); 
        
        string sql = "UPDATE Invoice SET healthcare_claim_number = '" + healthcare_claim_number + "' WHERE invoice_id = " + invoice_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static int GetCountByEntityID(int entity_id)
    {
        string sql = "SELECT COUNT(*) FROM Invoice WHERE entity_id = " + entity_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }


    public static int GetCountByBookingID(int booking_id, bool inc_reversed = false)
    {
        string sql = @"SELECT  COUNT(*) FROM Invoice WHERE " + (inc_reversed ? "" : " reversed_by IS NULL AND ") + " booking_id = " + booking_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static decimal GetMedicareCountByPatientAndYear(int patient_id, int year, int offering_id = -1, bool inc_reversed = false)
    {
        return GetMedicareCountByPatientAndDateRange(patient_id, new DateTime(year, 1, 1), new DateTime(year+1, 1, 1), offering_id, -1, inc_reversed);
    }
    // fld: 68 = 'Podiatrist', 277 ='Physiotherapist'
    public static decimal GetMedicareCountByPatientAndDateRange(int patient_id, DateTime start, DateTime end, int offering_id = -1, int field_id = -1, bool inc_reversed = false)
    {

        //
        // seperate queries for clinics and bookings because when combine, it takes 2.2 seconds instead of 0.14 seconds with 2 seperate queries
        //


        string sql_clinics = @"SELECT  COALESCE(SUM(inv_line.quantity),0)
                               FROM    InvoiceLine inv_line
                                       INNER JOIN Invoice  inv     ON inv_line.invoice_id = inv.invoice_id
                                       INNER JOIN Booking  b       ON inv.booking_id      = b.booking_id
                                       INNER JOIN Offering o       ON o.offering_id       = inv_line.offering_id
                                       INNER JOIN Staff    bk_prov ON bk_prov.staff_id    = b.provider

                               WHERE   (b.patient_id = " + patient_id + @") 
                                       " + (inc_reversed ? "" : "AND inv.reversed_by IS NULL ") + @" 


                                       -- in BEST, its only a medicare inv if the claim number exists and is not blank (i think it is a full stop)
                                       -- so if before 2013-07-01, make sure it has those conditions
                                       AND (inv.invoice_date_added >= '2013-07-01' OR inv.healthcare_claim_number IS NOT NULL)
                                       --AND inv.healthcare_claim_number IS NOT NULL 
                                       --AND inv.healthcare_claim_number <> '' 

                                       AND inv.payer_organisation_id = -1 " +
                                       (offering_id == -1 ? "" : " AND inv_line.offering_id = " + offering_id) +
                                       (field_id == -1 ? "" : " AND (o.field_id = " + field_id + " OR (o.field_id = 0 AND bk_prov.field_id = " + field_id + ")) -- aged care line offerings have field_id 0 so use bk prov field_id ") + @"
                                       AND b.date_start >= '" + start.ToString("yyyy-MM-dd HH:mm:ss") + @"' 
                                       AND b.date_start <= '" + end.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                       AND (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE CreditNote.invoice_id = inv.invoice_id) < inv.total    -- if sum credit notes == total, then its been rejected and re-invoiced
                                       ";

        string sql_aged_care = @"SELECT  COALESCE(SUM(inv_line.quantity),0)
                                 FROM    InvoiceLine inv_line
                                       INNER JOIN Invoice  inv     ON inv_line.invoice_id = inv.invoice_id
                                       INNER JOIN Booking  b       ON inv.booking_id      = b.booking_id
                                       INNER JOIN Offering o       ON o.offering_id       = inv_line.offering_id
                                       INNER JOIN Staff    bk_prov ON bk_prov.staff_id    = b.provider

                                 WHERE   (b.patient_id IS NULL AND inv_line.patient_id = " + patient_id + @")
                                       " + (inc_reversed ? "" : "AND inv.reversed_by IS NULL ") + @" 


                                       -- in BEST, its only a medicare inv if the claim number exists and is not blank (i think it is a full stop)
                                       -- so if before 2013-07-01, make sure it has those conditions
                                       AND (inv.invoice_date_added >= '2013-07-01' OR inv.healthcare_claim_number IS NOT NULL)
                                       --AND inv.healthcare_claim_number IS NOT NULL 
                                       --AND inv.healthcare_claim_number <> '' 

                                       AND inv.payer_organisation_id = -1 " +
                                       (offering_id == -1 ? "" : " AND inv_line.offering_id = " + offering_id) +
                                       (field_id == -1 ? "" : " AND (o.field_id = " + field_id + " OR (o.field_id = 0 AND bk_prov.field_id = " + field_id + ")) -- aged care line offerings have field_id 0 so use bk prov field_id ") + @"
                                       AND b.date_start >= '" + start.ToString("yyyy-MM-dd HH:mm:ss") + @"' 
                                       AND b.date_start <= '" + end.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                                       AND (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE CreditNote.invoice_id = inv.invoice_id) < inv.total    -- if sum credit notes == total, then its been rejected and re-invoiced
                                       ";

        return Convert.ToDecimal(DBBase.ExecuteSingleResult(sql_clinics)) + Convert.ToDecimal(DBBase.ExecuteSingleResult(sql_aged_care));
    }
    public static Hashtable GetMedicareCountByPatientsAndYear(int[] patient_ids, int year, bool inc_reversed = false)
    {

        //
        // seperate queries for clinics and bookings because when combine, it takes 2.2 seconds instead of 0.14 seconds with 2 seperate queries
        //

        /*
        string sql_clinics = @"SELECT p.patient_id, 
                            (
                             SELECT  COALESCE(SUM(inv_line.quantity), 0)
                             FROM    InvoiceLine inv_line
                                     INNER JOIN Invoice inv ON inv_line.invoice_id = inv.invoice_id
                                     INNER JOIN Booking b ON inv.booking_id = b.booking_id
                             WHERE   (b.patient_id = p.patient_id)                                                  -- CLINICS - PT IS IN BOOKING
                                     " + (inc_reversed ? "" : "AND inv.reversed_by IS NULL ") + @" 

                                     -- in BEST, its only a medicare inv if the claim number exists and is not blank (i think it is a full stop)
                                     -- so if before 2013-07-01, make sure it has those conditions
                                     AND (inv.invoice_date_added >= '2013-07-01' OR inv.healthcare_claim_number IS NOT NULL)
                                     --AND inv.healthcare_claim_number IS NOT NULL 
                                     --AND inv.healthcare_claim_number <> '' 

                                     AND inv.payer_organisation_id = -1 
                                     AND b.date_start >= '" + year + @"-01-01 00:00:00' 
                                     AND b.date_start < '" + (year + 1) + @"-01-01 00:00:00'
                                     AND (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE CreditNote.invoice_id = inv.invoice_id) < inv.total    -- if sum credit notes == total, then its been rejected and re-invoiced
                            ) as count

                        FROM  Patient p
                        WHERE p.patient_id IN (" + string.Join(",", patient_ids) + @")
                        ";
        */

        string sql_clinics = @"SELECT  b.patient_id, sum(inv_line.quantity) as count
                               FROM    InvoiceLine inv_line
                                       INNER JOIN Invoice inv ON inv_line.invoice_id = inv.invoice_id
                                       INNER JOIN Booking b ON inv.booking_id = b.booking_id
                               WHERE   --(b.patient_id = p.patient_id)                                                  -- CLINICS - PT IS IN BOOKING
                                       --AND 
                                       inv.reversed_by IS NULL  

                                       -- in BEST, its only a medicare inv if the claim number exists and is not blank (i think it is a full stop)
                                       -- so if before 2013-07-01, make sure it has those conditions
                                       AND (inv.invoice_date_added >= '2013-07-01' OR inv.healthcare_claim_number IS NOT NULL)
                                       --AND inv.healthcare_claim_number IS NOT NULL 
                                       --AND inv.healthcare_claim_number <> '' 

                                      AND inv.payer_organisation_id = -1 
                                      AND b.date_start >= '2015-01-01 00:00:00' 
                                      AND b.date_start < '2016-01-01 00:00:00'
                                      AND (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE CreditNote.invoice_id = inv.invoice_id) < inv.total    -- if sum credit notes == total, then its been rejected and re-invoiced
                                     
                                      AND b.patient_id IN (" + string.Join(",", patient_ids) + @")
                               GROUP BY b.patient_id";

        /*
        string sql_aged_care = @"SELECT p.patient_id, 
                            (
                             SELECT  COALESCE(SUM(inv_line.quantity), 0)
                             FROM    InvoiceLine inv_line
                                     INNER JOIN Invoice inv ON inv_line.invoice_id = inv.invoice_id
                                     INNER JOIN Booking b ON inv.booking_id = b.booking_id
                             WHERE   (b.patient_id IS NULL AND inv_line.patient_id = p.patient_id)                  -- AGED CARE - PT IS IN INVOICELINE
                                     " + (inc_reversed ? "" : "AND inv.reversed_by IS NULL ") + @" 

                                     -- in BEST, its only a medicare inv if the claim number exists and is not blank (i think it is a full stop)
                                     -- so if before 2013-07-01, make sure it has those conditions
                                     AND (inv.invoice_date_added >= '2013-07-01' OR inv.healthcare_claim_number IS NOT NULL)
                                     --AND inv.healthcare_claim_number IS NOT NULL 
                                     --AND inv.healthcare_claim_number <> '' 

                                     AND inv.payer_organisation_id = -1 
                                     AND b.date_start >= '" + year + @"-01-01 00:00:00' 
                                     AND b.date_start < '" + (year + 1) + @"-01-01 00:00:00'
                                     AND (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE CreditNote.invoice_id = inv.invoice_id) < inv.total    -- if sum credit notes == total, then its been rejected and re-invoiced
                            ) as count

                        FROM  Patient p
                        WHERE p.patient_id IN (" + string.Join(",", patient_ids) + @")
                        ";
        */

        string sql_aged_care = @"SELECT  inv_line.patient_id, sum(inv_line.quantity) as count
                                 FROM    InvoiceLine inv_line
                                         INNER JOIN Invoice inv ON inv_line.invoice_id = inv.invoice_id
                                         INNER JOIN Booking b ON inv.booking_id = b.booking_id
                                 WHERE   b.patient_id IS NULL                  -- AGED CARE - PT IS IN INVOICELINE
                                         AND inv.reversed_by IS NULL  

                                         -- in BEST, its only a medicare inv if the claim number exists and is not blank (i think it is a full stop)
                                         -- so if before 2013-07-01, make sure it has those conditions
                                         AND (inv.invoice_date_added >= '2013-07-01' OR inv.healthcare_claim_number IS NOT NULL)
                                         --AND inv.healthcare_claim_number IS NOT NULL 
                                         --AND inv.healthcare_claim_number <> '' 

                                         AND inv.payer_organisation_id = -1 
                                         AND b.date_start >= '2015-01-01 00:00:00' 
                                         AND b.date_start < '2016-01-01 00:00:00'
                                         AND (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE CreditNote.invoice_id = inv.invoice_id) < inv.total    -- if sum credit notes == total, then its been rejected and re-invoiced
                                     
                                         AND inv_line.patient_id IN (" + string.Join(",", patient_ids) + @")
                                 GROUP BY inv_line.patient_id";


        DataTable tbl_clinics   = DBBase.ExecuteQuery(sql_clinics).Tables[0];
        DataTable tbl_aged_care = DBBase.ExecuteQuery(sql_aged_care).Tables[0];


        Hashtable result = new Hashtable();
        for(int i=0; i<patient_ids.Length; i++)
            result[patient_ids[i]] = 0;
        for (int i = 0; i < tbl_clinics.Rows.Count; i++)
        {
            int patient_id = Convert.ToInt32(tbl_clinics.Rows[i]["patient_id"]);
            int count      = (int)Convert.ToDecimal(tbl_clinics.Rows[i]["count"]);
            result[patient_id] = count;
        }
        for (int i = 0; i < tbl_aged_care.Rows.Count; i++)
        {
            int patient_id = Convert.ToInt32(tbl_aged_care.Rows[i]["patient_id"]);
            int count      = (int)Convert.ToDecimal(tbl_aged_care.Rows[i]["count"]);
            result[patient_id] = (result[patient_id] == null ? 0 : (int)result[patient_id]) + count;   // ADD IT ON TO EXISTING
        }

        return result;
    }

    public static Invoice[] GetMedicareByPatientAndDateRangeStartDateAsc(int patient_id, DateTime start, DateTime end, int field_id = -1, bool inc_reversed = false)
    {
        string sql = JoinedSql() + 
                        @" WHERE " + 
                            (inc_reversed ? "" : " inv.reversed_by IS NULL AND ") + @" 
                            inv.healthcare_claim_number IS NOT NULL 
                            AND inv.healthcare_claim_number <> '' 
                            AND (booking.patient_id = " + patient_id + @" OR (booking.patient_id IS NULL AND  (SELECT COUNT(*) FROM InvoiceLine WHERE InvoiceLine.invoice_id = inv.invoice_id AND patient_id = " + patient_id + @") > 0)) 
                            AND inv.payer_organisation_id = -1 
                            AND booking.date_start >= '" + start.ToString("yyyy-MM-dd HH:mm:ss") + @"' 
                            AND booking.date_start < '" + end.ToString("yyyy-MM-dd HH:mm:ss") + "'" + 
                            (field_id == -1 ? "" : " AND booking_offering.field_id = " + field_id.ToString()) +
                            " AND (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE CreditNote.invoice_id = inv.invoice_id) < inv.total    -- if sum credit notes == total, then its been rejected and re-invoiced " + 
                        @" ORDER BY booking.date_start ";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Invoice[] invoices = new Invoice[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            invoices[i] = LoadAll(tbl.Rows[i]);

        return invoices;
    }

    public static DataTable GetDataTable(bool inc_deep_booking_info, DateTime date_start, DateTime date_end, bool view_non_reversed = true, bool view_reversed = false, string invoice_type_ids = null, int site_id = -1, int patient_id = -1, int bk_provider_id = -1, string org_ids = null, bool inc_medicare = true, bool inc_dva = true, bool inc_tac = true, bool inc_private = true, bool inc_paid = true, bool inc_unpaid = true, bool only_rejected = false, string inv_id_search = "", string booking_id_serach = "", string claim_nbr_search = "")
    {
        int[] insurance_company_ids = OrganisationDB.GetInsuranceCompanyOrgIDs();

        bool containsWhereClause = false;
        string sql = JoinedSql(inc_deep_booking_info);


        if (date_start != DateTime.MinValue)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " invoice_date_added >= '" + date_start.ToString("yyyy-MM-dd HH:mm") + "'";
            containsWhereClause = true;
        }
        if (date_end != DateTime.MinValue)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " invoice_date_added <= '" + date_end.ToString("yyyy-MM-dd HH:mm") + "'";
            containsWhereClause = true;
        }

        if (view_non_reversed  && !view_reversed)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " inv.reversed_by IS NULL";
            containsWhereClause = true;
        }
        if (!view_non_reversed &&  view_reversed)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " inv.reversed_by IS NOT NULL";
            containsWhereClause = true;
        }
        if (!view_reversed && !view_non_reversed)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " 1 <> 1";
            containsWhereClause = true;
        }

        if (invoice_type_ids != null && invoice_type_ids.Length > 0)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " inv.invoice_type_id IN (" + invoice_type_ids + ")";
            containsWhereClause = true;
        }
        if (site_id != -1)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " inv.site_id = " + site_id;
            containsWhereClause = true;
        }
        if (patient_id != -1)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " (inv.payer_patient_id = " + patient_id + " OR booking.patient_id = " + patient_id + ") ";
            containsWhereClause = true;
        }
        if (bk_provider_id != -1)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " booking.provider = " + bk_provider_id;
            containsWhereClause = true;
        }
        if (org_ids != null)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " (booking.organisation_id IN (" + org_ids + ") OR inv.payer_organisation_id IN (" + org_ids + ") OR inv.non_booking_invoice_organisation_id IN (" + org_ids + ")) ";
            containsWhereClause = true;
        }


        if (inc_private)
        {
            string orgs_excl = string.Empty;
            if (!inc_medicare) orgs_excl += (orgs_excl.Length == 0 ? "" : ",") + "-1";
            if (!inc_dva)      orgs_excl += (orgs_excl.Length == 0 ? "" : ",") + "-2";
            if (!inc_tac && insurance_company_ids.Length > 0)
                               orgs_excl += (orgs_excl.Length == 0 ? "" : ",") + string.Join(",", insurance_company_ids);
            if (orgs_excl.Length > 0)
            {
                sql += (containsWhereClause ? " AND " : " WHERE ") + " (inv.payer_organisation_id IS NULL OR inv.payer_organisation_id NOT IN (" + orgs_excl + ")) ";
                containsWhereClause = true;
            }
        
        }
        else // if (!inc_private)
        {
            string orgs_incl = string.Empty;
            if (inc_medicare) orgs_incl += (orgs_incl.Length == 0 ? "" : ",") + "-1";
            if (inc_dva)      orgs_incl += (orgs_incl.Length == 0 ? "" : ",") + "-2";
            if (inc_tac && insurance_company_ids.Length > 0)
                              orgs_incl += (orgs_incl.Length == 0 ? "" : ",") + string.Join(",", insurance_company_ids);

            if (orgs_incl.Length > 0)
            {
                sql += (containsWhereClause ? " AND " : " WHERE ") + " inv.payer_organisation_id IN (" + orgs_incl + ") ";
                containsWhereClause = true;
            }
            else
            {
                sql += (containsWhereClause ? " AND " : " WHERE ") + " 1 <> 1 ";
                containsWhereClause = true;
            }
        }

        if (!inc_paid && !inc_unpaid)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " 1 <> 1 ";
            containsWhereClause = true;
        }
        else if (!inc_paid)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " inv.is_paid = 0 ";
            containsWhereClause = true;
        }
        else if (!inc_unpaid)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " inv.is_paid = 1 ";
            containsWhereClause = true;
        }

        if (only_rejected)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " inv.reject_letter_id IS NOT NULL ";
            containsWhereClause = true;
        }

        if (inv_id_search != null && inv_id_search.Length > 0)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " (CONVERT(varchar, inv.invoice_id) LIKE '%" + inv_id_search + "%') ";
            containsWhereClause = true;
        }
        if (booking_id_serach != null && booking_id_serach.Length > 0)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " (CONVERT(varchar, inv.booking_id) LIKE '%" + booking_id_serach + "%') ";
            containsWhereClause = true;
        }
        if (claim_nbr_search != null && claim_nbr_search.Length > 0)
        {
            sql += (containsWhereClause ? " AND " : " WHERE ") + " (inv.healthcare_claim_number LIKE '%" + claim_nbr_search + "%') ";
            containsWhereClause = true;
        }

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable_ByID(int invoice_id, string DB = null)
    {
        string sql = JoinedSql() + " WHERE inv.invoice_id = " + invoice_id.ToString();
        return DBBase.ExecuteQuery(sql, DB).Tables[0];
    }
    public static DataTable GetDataTable_ByIDs(int[] invoice_ids)
    {
        string sql = JoinedSql() + " WHERE inv.invoice_id IN (" + (invoice_ids == null || invoice_ids.Length == 0 ? "-1" : string.Join(",", invoice_ids)) + ")";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Invoice GetByID(int invoice_id, string DB = null)
    {
        DataTable tbl = GetDataTable_ByID(invoice_id, DB);
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }
    public static Invoice[] GetByIDs(int[] invoice_ids)
    {
        DataTable tbl = GetDataTable_ByIDs(invoice_ids);

        Invoice[] invoices = new Invoice[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            invoices[i] = LoadAll(tbl.Rows[i]);

        return invoices;
    }

    public static DataTable GetDataTable_ByBookingID(int booking_id, bool inc_reversed = false)
    {
        string sql = JoinedSql() + " WHERE " + (inc_reversed ? "" : " inv.reversed_by IS NULL AND ") + " inv.booking_id = " + booking_id.ToString();
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Invoice[] GetByBookingID(int booking_id, bool inc_reversed = false, bool onlyLastMedicareDvaInvoice = false)
    {
        DataTable tbl = GetDataTable_ByBookingID(booking_id, inc_reversed);

        Invoice[] invoices = new Invoice[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            invoices[i] = LoadAll(tbl.Rows[i]);

        if (onlyLastMedicareDvaInvoice)
            invoices = RemoveAllButLastMedicareDvaInvoices(invoices);

        return invoices;
    }
    private static Invoice[] RemoveAllButLastMedicareDvaInvoices(Invoice[] invoices)
    {
        int highestMedicareInvID = -1;
        int highestDVAInvID      = -1;
        int medicareCount        = 0;
        int dvaCount             = 0;
        for (int i = 0; i < invoices.Length; i++)
        {
            if (invoices[i].PayerOrganisation != null && invoices[i].PayerOrganisation.OrganisationID == -1)
            {
                highestMedicareInvID = Math.Max(highestMedicareInvID, invoices[i].InvoiceID);
                medicareCount++;
            }
            if (invoices[i].PayerOrganisation != null && invoices[i].PayerOrganisation.OrganisationID == -2)
            {
                highestDVAInvID = Math.Max(highestDVAInvID, invoices[i].InvoiceID);
                dvaCount++;
            }
        }

        if (medicareCount > 1 || dvaCount > 1)
        {
            System.Collections.ArrayList newList = new System.Collections.ArrayList();
            for (int i = 0; i < invoices.Length; i++)
            {
                if (invoices[i].PayerOrganisation == null)
                    newList.Add(invoices[i]);
                else if (invoices[i].PayerOrganisation.OrganisationID == -1 && invoices[i].InvoiceID == highestMedicareInvID)
                    newList.Add(invoices[i]);
                else if (invoices[i].PayerOrganisation.OrganisationID == -2 && invoices[i].InvoiceID == highestDVAInvID)
                    newList.Add(invoices[i]);
            }
            invoices = (Invoice[])newList.ToArray(typeof(Invoice));
        }

        return invoices;
    }

    public static Invoice[] RemoveRejectedInvoices(Invoice[] invoices)
    {
        // if medicare/dva ... and credits >= total ... its rejected ... chuck it
        // else
        //   if no more private invoices, chuck it ... otherwise keep it

        int nPrivateInvoicesNotWrittenOff = 0;
        for (int i = 0; i < invoices.Length; i++)
            if (invoices[i].PayerOrganisation == null && invoices[i].CreditNotesTotal < invoices[i].Total)
                nPrivateInvoicesNotWrittenOff++;


        System.Collections.ArrayList newList = new System.Collections.ArrayList();
        for (int i = 0; i < invoices.Length; i++)
        {
            bool isMedicareOrDVA = invoices[i].PayerOrganisation != null && (invoices[i].PayerOrganisation.OrganisationID == -1 || invoices[i].PayerOrganisation.OrganisationID == -2);
            bool isPrivate       = !isMedicareOrDVA;

            // keep normal invoices
            if (invoices[i].CreditNotesTotal < invoices[i].Total || invoices[i].Total == 0)
                newList.Add(invoices[i]);

            // if fully credit noted and medicare - its rejected
            // if fully credit noted and have another private invoice not fully credit noted - its rejected
            // if fully credit noted and private invoice and no other private invoices, it's just been credit noted - so keep it
            else if (isPrivate && nPrivateInvoicesNotWrittenOff == 0)
                    newList.Add(invoices[i]);
        }

        return (Invoice[])newList.ToArray(typeof(Invoice));
    }


    public static DataTable GetMedicareInvoicesWithoutClaimNumbers(bool inc_deep_booking_info, int organisation_id, int site_id, DateTime start, DateTime end, bool inc_reversed = false)
    {
        string sql = JoinedSql(inc_deep_booking_info) + @"
                WHERE 
                        inv.total > 0 
                        AND is_batched = 0
                        AND inv.is_paid = 0 
                        AND inv.reject_letter_id IS NULL
                        AND inv.payer_organisation_id = " + organisation_id + @" 
                        AND inv.site_id = " + site_id + @" 
                        AND DATALENGTH(inv.healthcare_claim_number) = 0      -- this is the difference from the below method
                        " + (start == DateTime.MinValue ? "" : " AND inv.invoice_date_added >= '" + start.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                        " + (end   == DateTime.MinValue ? "" : " AND inv.invoice_date_added <  '" + end.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                        " + (inc_reversed ? "" : " AND inv.reversed_by IS NULL") + @"
                        AND booking.booking_status_id = 187
                ORDER BY 
                        inv.invoice_id";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        InvoiceDB.AddTotalDueColumn(ref tbl);
        return tbl;
    }
    public static DataTable GetUnclaimedMedicareInvoices(bool inc_deep_booking_info, int organisation_id, int site_id, DateTime start, DateTime end, bool inc_reversed = false)
    {
        string sql = JoinedSql(inc_deep_booking_info) + @"
                WHERE 
                        inv.total > 0 
                        AND is_batched = 0
                        AND inv.is_paid = 0 
                        AND inv.reject_letter_id IS NULL
                        AND inv.payer_organisation_id = " + organisation_id + @" 
                        AND inv.site_id = " + site_id + @" 
                        AND DATALENGTH(inv.healthcare_claim_number) > 1      -- get rid of BEST invoices that use full stop to indicate no claim number
                        " + (start == DateTime.MinValue ? "" : " AND inv.invoice_date_added >= '" + start.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                        " + (end   == DateTime.MinValue ? "" : " AND inv.invoice_date_added <  '" + end.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @"
                        " + (inc_reversed               ? "" : " AND inv.reversed_by IS NULL") + @"
                        AND booking.booking_status_id = 187
                ORDER BY 
                        inv.invoice_id";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        InvoiceDB.AddTotalDueColumn(ref tbl);
        return tbl;
    }
    public static Invoice GetByClaimNumber(string healthcare_claim_number)
    {
        string sql = JoinedSql() + " WHERE inv.healthcare_claim_number = '" + healthcare_claim_number + "' ORDER BY inv.invoice_date_added DESC";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        InvoiceDB.AddTotalDueColumn(ref tbl);
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static DataTable GetDataTable_Outstanding(int patient_id, int site_id, bool insuranceCompanies = false)
    {
        int[] insuranceCompanyOrgIDs = OrganisationDB.GetInsuranceCompanyOrgIDs();

        string sql = JoinedSql(true) + @"
                WHERE 

                        inv.total > 0 
                        AND inv.is_paid = 0  
                        --AND inv.payer_organisation_id IS NULL  -- only private invoices
                        " + (site_id != -1 ? "AND inv.site_id = " + site_id : "") + @"
                        AND inv.reversed_by IS NULL

                        -- This is slowing pt detail page down enormously and also seems to have no purpose that I can see at time of commenting it out
                        --
		                -- AND  (inv.total - 
		                --          (SELECT COALESCE(SUM(total), 0) FROM Receipt WHERE invoice_id = inv.invoice_id) -
		                --          (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE invoice_id = inv.invoice_id)) > 0

                        " + 
                        (patient_id == -1 ? string.Empty : 
		                  @"AND (
                                (Booking.patient_id = " + patient_id + @" AND booking.booking_status_id = 187)   -- booking invoices
                                OR
                                (inv.booking_id IS NULL AND inv.payer_patient_id = " + patient_id + @")          -- non-booking invoices
                                ) 
                         ") + @"

                            " + (!insuranceCompanies ? " AND (inv.payer_organisation_id IS NULL OR (inv.payer_organisation_id > 0 AND (inv.payer_organisation_id NOT IN (" + (insuranceCompanyOrgIDs.Length == 0 ? "0" : string.Join(",", insuranceCompanyOrgIDs)) + ")) )) "
                                                     : " AND (inv.payer_organisation_id IN (" + (insuranceCompanyOrgIDs.Length == 0 ? "0" : string.Join(",", insuranceCompanyOrgIDs)) + ")) ") + @"
                ORDER BY 
                        inv.invoice_id";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        InvoiceDB.AddTotalDueColumn(ref tbl);
        return tbl;
    }
    public static Invoice[] GetOutstanding(int patient_id, int site_id, bool insuranceCompanies = false)
    {
        DataTable tbl = GetDataTable_Outstanding(patient_id, site_id, insuranceCompanies);
        Invoice[] invoices = new Invoice[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            invoices[i] = LoadAll(tbl.Rows[i], true);

        return invoices;
    }

    public static Hashtable GetAllOutstandingByPatient(int site_id)
    {
        Hashtable hash = new Hashtable();

        Invoice[] allInvoices = GetOutstanding(-1, site_id, false);
        for (int i = 0; i < allInvoices.Length; i++)
        {
            if (allInvoices[i].PayerOrganisation != null)  // ignore invoices for organisations
                continue;

            int patientID = -1;
            if (allInvoices[i].PayerPatient != null)
                patientID = allInvoices[i].PayerPatient.PatientID;
            else if (allInvoices[i].Booking != null && allInvoices[i].Booking.Patient != null)
                patientID = allInvoices[i].Booking.Patient.PatientID;
            else
                continue;

            if (hash[patientID] == null)
                hash[patientID] = new ArrayList();

            ((ArrayList)hash[patientID]).Add(allInvoices[i]);
        }


        // convert from arraylists to arrays 
        ArrayList keys = new ArrayList();
        foreach (System.Collections.DictionaryEntry de in hash)
            keys.Add(de.Key);
        foreach (int key in keys)
            hash[key] = (Invoice[])((ArrayList)hash[key]).ToArray(typeof(Invoice));


        return hash;
    }
    public static DataTable GetAllOutstandingByPatientAsReport(int site_id, int daysOverdueAtleast = 0)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("patient_fullname", typeof(String));
        dt.Columns.Add("patient_firstname", typeof(String));
        dt.Columns.Add("patient_surname", typeof(String));
        dt.Columns.Add("patient_id", typeof(Int32));
        dt.Columns.Add("total_due", typeof(Decimal));
        dt.Columns.Add("total_inv_count", typeof(Int32));


        dt.Columns.Add("invoice_ids", typeof(String));
        dt.Columns.Add("bk_treatement_dates", typeof(String));
        dt.Columns.Add("bk_orgs", typeof(String));
        dt.Columns.Add("bk_totals", typeof(String));
        dt.Columns.Add("bk_owings", typeof(String));
        dt.Columns.Add("invoice_id_first", typeof(Int32));
        dt.Columns.Add("bk_treatement_date_first", typeof(DateTime));
        dt.Columns.Add("bk_org_first", typeof(String));
        dt.Columns.Add("bk_total_first", typeof(Decimal));
        dt.Columns.Add("bk_owing_first", typeof(Decimal));
        dt.Columns.Add("invoice_ids_comma_sep", typeof(String));
        dt.Columns.Add("days_overdue_comma_sep", typeof(String));




        Hashtable hash = InvoiceDB.GetAllOutstandingByPatient(site_id);

        foreach (int key in hash.Keys)
        {
            Invoice[] invoices = (Invoice[])hash[key];

            Array.Sort(invoices, delegate(Invoice x, Invoice y) {
                DateTime xDate = x.Booking != null ? x.Booking.DateStart : x.InvoiceDateAdded;
                DateTime yDate = y.Booking != null ? y.Booking.DateStart : y.InvoiceDateAdded;
                return xDate.CompareTo(yDate); 
            });


            Patient pt = invoices[0].PayerPatient != null ?
                invoices[0].PayerPatient :
                invoices[0].Booking.Patient;

            string invoice_ids                = string.Empty;
            string bk_treatement_dates        = string.Empty;
            string bk_orgs                    = string.Empty;
            string bk_totals                  = string.Empty;
            string bk_owings                  = string.Empty;
            int      invoice_id_first         = 0;
            DateTime bk_treatement_date_first = DateTime.MinValue;
            string   bk_org_first             = string.Empty;
            decimal  bk_total_first           = 0;
            decimal  bk_owing_first           = 0;
            string invoice_ids_comma_sep      = string.Empty;
            string days_overdue_comma_sep     = string.Empty;

            string  invOutput = string.Empty;
            decimal totalDue  = 0;
            bool    allUnderDaysOverdueThreshhold = true;
            for (int i = 0; i < invoices.Length; i++)
            {
                int daysOverdue = (int)DateTime.Today.Subtract(invoices[i].InvoiceDateAdded).TotalDays;

                invoice_ids              += (invoice_ids.Length           == 0 ? "" : "<br />") +  invoices[i].InvoiceID;
                bk_treatement_dates      += (bk_treatement_dates.Length   == 0 ? "" : "<br />") + (invoices[i].Booking == null ? "" : invoices[i].Booking.DateStart.ToString("dd-MM-yyyy"));
                bk_orgs                  += (bk_orgs.Length               == 0 ? "" : "<br />") + (invoices[i].Booking == null ? "" : invoices[i].Booking.Organisation.Name);
                bk_totals                += (bk_totals.Length             == 0 ? "" : "<br />") + (invoices[i].Booking == null ? "" : string.Format("{0:C}", invoices[i].Total));
                bk_owings                += (bk_owings.Length             == 0 ? "" : "<br />") + (invoices[i].Booking == null ? "" : string.Format("{0:C}", invoices[i].TotalDue));

                if (i == 0)
                {
                    invoice_id_first         =  invoices[i].InvoiceID;
                    bk_treatement_date_first = (invoices[i].Booking == null ? DateTime.MinValue : invoices[i].Booking.DateStart);
                    bk_org_first             = (invoices[i].Booking == null ? ""                : invoices[i].Booking.Organisation.Name);
                    bk_total_first           = (invoices[i].Booking == null ? 0                 : invoices[i].Total);
                    bk_owing_first           = (invoices[i].Booking == null ? 0                 : invoices[i].TotalDue);
                }
                
                invoice_ids_comma_sep  += (invoice_ids_comma_sep.Length  == 0 ? "" : ",") + invoices[i].InvoiceID;
                days_overdue_comma_sep += (days_overdue_comma_sep.Length == 0 ? "" : ",") + daysOverdue;

                if (daysOverdue > daysOverdueAtleast)
                    allUnderDaysOverdueThreshhold = false;

                invOutput += (i == 0 ? "" : "<br />") + "ID: " + invoices[i].InvoiceID + " Amt: " + invoices[i].Total + " Due: <b>" + invoices[i].TotalDue + "</b>";
                totalDue += invoices[i].TotalDue;
            }

            DataRow newRow = dt.NewRow();
            newRow["patient_fullname"]         = pt.Person.FullnameWithoutMiddlename;
            newRow["patient_firstname"]        = pt.Person.Firstname;
            newRow["patient_surname"]          = pt.Person.Surname;
            newRow["patient_id"]               = pt.PatientID;
            newRow["total_due"]                = totalDue;
            newRow["total_inv_count"]          = invoices.Length;

            newRow["invoice_ids"]              = invoice_ids;
            newRow["bk_treatement_dates"]      = bk_treatement_dates;
            newRow["bk_orgs"]                  = bk_orgs;
            newRow["bk_totals"]                = bk_totals;
            newRow["bk_owings"]                = bk_owings;
            newRow["invoice_id_first"]         = invoice_id_first;
            newRow["bk_treatement_date_first"] = bk_treatement_date_first;
            newRow["bk_org_first"] = bk_org_first;
            newRow["bk_total_first"]           = bk_total_first;
            newRow["bk_owing_first"]           = bk_owing_first;
            newRow["invoice_ids_comma_sep"]    = invoice_ids_comma_sep;
            newRow["days_overdue_comma_sep"]   = days_overdue_comma_sep;

            if (!allUnderDaysOverdueThreshhold)
                dt.Rows.Add(newRow);
        }

        return dt;
    }

    public static Hashtable GetAllOutstandingByOrg(int site_id, bool insuranceCompanies = false)
    {
        Hashtable hash = new Hashtable();

        Invoice[] allInvoices = GetOutstanding(-1, site_id, insuranceCompanies);
        for (int i = 0; i < allInvoices.Length; i++)
        {

            int orgID = 0;

            if (allInvoices[i].PayerOrganisation != null)
                orgID = allInvoices[i].PayerOrganisation.OrganisationID;
            else if (allInvoices[i].Booking != null && allInvoices[i].Booking.Organisation != null && allInvoices[i].Booking.Organisation.OrganisationID != null)
                orgID = allInvoices[i].Booking.Organisation.OrganisationID;
            else
                continue;

            if (hash[orgID] == null)
                hash[orgID] = new ArrayList();

            ((ArrayList)hash[orgID]).Add(allInvoices[i]);
        }


        // convert from arraylists to arrays 
        ArrayList keys = new ArrayList();
        foreach (System.Collections.DictionaryEntry de in hash)
            keys.Add(de.Key);
        foreach (int key in keys)
            hash[key] = (Invoice[])((ArrayList)hash[key]).ToArray(typeof(Invoice));


        return hash;
    }
    public static DataTable GetAllOutstandingByOrgAsReport(int site_id, int daysOverdueAtleast = 0, bool insuranceCompanies = false)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("name"           , typeof(String));
        dt.Columns.Add("organisation_id", typeof(Int32));
        dt.Columns.Add("total_due"      , typeof(Decimal));
        dt.Columns.Add("total_inv_count", typeof(Int32));

        dt.Columns.Add("invoice_ids_comma_sep", typeof(String));
        dt.Columns.Add("days_overdue_comma_sep", typeof(String));


        Hashtable hash = InvoiceDB.GetAllOutstandingByOrg(site_id, insuranceCompanies);

        foreach (int key in hash.Keys)
        {
            Invoice[] invoices = (Invoice[])hash[key];

            Array.Sort(invoices, delegate(Invoice x, Invoice y)
            {
                DateTime xDate = x.Booking != null ? x.Booking.DateStart : x.InvoiceDateAdded;
                DateTime yDate = y.Booking != null ? y.Booking.DateStart : y.InvoiceDateAdded;
                return xDate.CompareTo(yDate);
            });


            Organisation org = invoices[0].PayerOrganisation != null ? invoices[0].PayerOrganisation : invoices[0].Booking.Organisation;
            //if (org.Name == null)
            //    org = (Organisation)orgHash[org.OrganisationID];

            string invoice_ids_comma_sep  = string.Empty;
            string days_overdue_comma_sep = string.Empty;

            string  invOutput = string.Empty;
            decimal totalDue  = 0;
            bool    allUnderDaysOverdueThreshhold = true;
            for (int i = 0; i < invoices.Length; i++)
            {
                int daysOverdue =  (int)DateTime.Today.Subtract(invoices[i].InvoiceDateAdded.Date).TotalDays;

                invoice_ids_comma_sep  += (invoice_ids_comma_sep.Length  == 0 ? "" : ",") + invoices[i].InvoiceID;
                days_overdue_comma_sep += (days_overdue_comma_sep.Length == 0 ? "" : ",") + daysOverdue;

                if (daysOverdue >= daysOverdueAtleast)
                    allUnderDaysOverdueThreshhold = false;

                invOutput += (i == 0 ? "" : "<br />") + "ID: " + invoices[i].InvoiceID + " Amt: " + invoices[i].Total + " Due: <b>" + invoices[i].TotalDue + "</b>";
                totalDue += invoices[i].TotalDue;
            }

            DataRow newRow = dt.NewRow();
            newRow["name"]                   = org.Name;
            newRow["organisation_id"]        = org.OrganisationID;
            newRow["total_due"]              = totalDue;
            newRow["total_inv_count"]        = invoices.Length;
            newRow["invoice_ids_comma_sep"]  = invoice_ids_comma_sep;
            newRow["days_overdue_comma_sep"] = days_overdue_comma_sep;

            if (!allUnderDaysOverdueThreshhold)
                dt.Rows.Add(newRow);
        }

        return dt;
    }

    public static Hashtable GetAllByBookings(int[] bookingIDs, int site_id = -1)
    {
        string sql = JoinedSql() + " WHERE " + (bookingIDs == null || bookingIDs.Length == 0 ? " 1 <> 1" : " inv.booking_id IN (" + string.Join(",", bookingIDs) + ")") + (site_id == -1 ? "" : " inv.site_id = " + site_id);
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Invoice invoice = LoadAll(tbl.Rows[i]);
            if (hash[invoice.Booking.BookingID] == null)
                hash[invoice.Booking.BookingID] = new ArrayList();

            ((ArrayList)hash[invoice.Booking.BookingID]).Add(invoice);
        }


        // convert from arraylists to arrays 
        ArrayList keys = new ArrayList();
        foreach (System.Collections.DictionaryEntry de in hash)
            keys.Add(de.Key);
        foreach (int key in keys)
            hash[key] = (Invoice[])((ArrayList)hash[key]).ToArray(typeof(Invoice));


        return hash;
    }



    #region JoinedSQL

    protected static string JoinedSql(bool inc_deep_booking_info = true)
    {
        string sql = @"
                SELECT 
                        inv.invoice_id as inv_invoice_id,inv.entity_id as inv_entity_id,
                        inv.invoice_type_id as inv_invoice_type_id,inv.booking_id as inv_booking_id,inv.payer_organisation_id as inv_payer_organisation_id,
                        inv.payer_patient_id as inv_payer_patient_id,inv.non_booking_invoice_organisation_id as inv_non_booking_invoice_organisation_id,inv.healthcare_claim_number as inv_healthcare_claim_number,inv.reject_letter_id as inv_reject_letter_id, inv.message as inv_message,
                        inv.staff_id as inv_staff_id,inv.site_id as inv_site_id,
                        inv.invoice_date_added as inv_invoice_date_added,inv.total as inv_total,inv.gst as inv_gst,inv.is_paid as inv_is_paid,
                        inv.is_refund as inv_is_refund,inv.is_batched as inv_is_batched,inv.reversed_by as inv_reversed_by,inv.reversed_date as inv_reversed_date,inv.last_date_emailed as inv_last_date_emailed,

                        (SELECT COALESCE(SUM(total), 0) FROM Receipt WHERE invoice_id = inv.invoice_id) as inv_receipts_total,
                        (SELECT -1* COALESCE(SUM(amount), 0) FROM Credit WHERE credit_type_id = 2 AND invoice_id = inv.invoice_id) as inv_vouchers_total,
                        (SELECT COALESCE(SUM(amount_reconciled), 0) FROM Receipt WHERE invoice_id = inv.invoice_id) as inv_reconciled_receipts_total,

                        (SELECT COALESCE(SUM(total), 0) FROM CreditNote WHERE invoice_id = inv.invoice_id) as inv_credit_notes_total,

                        (SELECT COALESCE(SUM(total), 0) FROM Refund WHERE invoice_id = inv.invoice_id) as inv_refunds_total,

                        invoice_type.invoice_type_id as invoice_type_invoice_type_id,invoice_type.descr as invoice_type_descr,


                        (SELECT COUNT(*) FROM Note note WHERE note.entity_id = inv.entity_id) AS inv_note_count,


                        booking.booking_id as booking_booking_id,booking.entity_id as booking_entity_id,booking.date_start as booking_date_start,booking.date_end as booking_date_end,booking.organisation_id as booking_organisation_id,
                        booking.provider as booking_provider,booking.patient_id as booking_patient_id,booking.offering_id as booking_offering_id,booking.booking_type_id as booking_booking_type_id,
                        booking.booking_status_id as booking_booking_status_id,booking.booking_unavailability_reason_id as booking_booking_unavailability_reason_id,
                        booking.added_by as booking_added_by,booking.date_created as booking_date_created,
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

                        ";
            if (inc_deep_booking_info)
                sql += @"
                        booking_status.booking_status_id as booking_status_booking_status_id,
                        booking_status.descr as booking_status_descr,

                        booking_confirmed_by_type.booking_confirmed_by_type_id as booking_confirmed_by_type_booking_confirmed_by_type_id,
                        booking_confirmed_by_type.descr as booking_confirmed_by_type_descr,

                        booking_unavailability_reason.booking_unavailability_reason_id as booking_unavailability_reason_booking_unavailability_reason_id,
                        booking_unavailability_reason.descr as booking_unavailability_reason_descr,

                        (SELECT COUNT(*) FROM Note note WHERE note.entity_id = booking.entity_id) AS booking_note_count,
                        (SELECT COUNT(*) FROM Invoice inv WHERE inv.booking_id = booking.booking_id) AS booking_inv_count,

                        booking_offering.offering_id as booking_offering_offering_id, booking_offering.offering_type_id as booking_offering_offering_type_id, booking_offering.field_id as booking_offering_field_id, booking_offering.aged_care_patient_type_id as booking_offering_aged_care_patient_type_id, 
                        booking_offering.num_clinic_visits_allowed_per_year as booking_offering_num_clinic_visits_allowed_per_year, booking_offering.offering_invoice_type_id as booking_offering_offering_invoice_type_id, 
                        booking_offering.name as booking_offering_name, booking_offering.short_name as booking_offering_short_name, booking_offering.descr as booking_offering_descr, booking_offering.is_gst_exempt as booking_offering_is_gst_exempt, booking_offering.default_price as booking_offering_default_price, booking_offering.service_time_minutes as booking_offering_service_time_minutes, 
                        booking_offering.max_nbr_claimable as booking_offering_max_nbr_claimable, booking_offering.max_nbr_claimable_months as booking_offering_max_nbr_claimable_months, booking_offering.medicare_company_code as booking_offering_medicare_company_code, booking_offering.dva_company_code as booking_offering_dva_company_code, booking_offering.tac_company_code as booking_offering_tac_company_code, 
                        booking_offering.medicare_charge as booking_offering_medicare_charge, booking_offering.dva_charge as booking_offering_dva_charge, booking_offering.tac_charge as booking_offering_tac_charge, 
                        booking_offering.popup_message as booking_offering_popup_message, booking_offering.reminder_letter_months_later_to_send as booking_offering_reminder_letter_months_later_to_send, booking_offering.reminder_letter_id as booking_offering_reminder_letter_id, booking_offering.use_custom_color as booking_offering_use_custom_color, booking_offering.custom_color as booking_offering_custom_color,

                        booking_org.organisation_id as booking_organisation_organisation_id, booking_org.entity_id as booking_organisation_entity_id, booking_org.parent_organisation_id as booking_organisation_parent_organisation_id, booking_org.use_parent_offernig_prices as booking_organisation_use_parent_offernig_prices, 
                        booking_org.organisation_type_id as booking_organisation_organisation_type_id, 
                        booking_org.organisation_customer_type_id as booking_organisation_organisation_customer_type_id,booking_org.name as booking_organisation_name, booking_org.acn as booking_organisation_acn, booking_org.abn as booking_organisation_abn, 
                        booking_org.organisation_date_added as booking_organisation_organisation_date_added, booking_org.organisation_date_modified as booking_organisation_organisation_date_modified, booking_org.is_debtor as booking_organisation_is_debtor, booking_org.is_creditor as booking_organisation_is_creditor, 
                        booking_org.bpay_account as booking_organisation_bpay_account, booking_org.weeks_per_service_cycle as booking_organisation_weeks_per_service_cycle, booking_org.start_date as booking_organisation_start_date, 
                        booking_org.end_date as booking_organisation_end_date, booking_org.comment as booking_organisation_comment, booking_org.free_services as booking_organisation_free_services, booking_org.excl_sun as booking_organisation_excl_sun, 
                        booking_org.excl_mon as booking_organisation_excl_mon, booking_org.excl_tue as booking_organisation_excl_tue, booking_org.excl_wed as booking_organisation_excl_wed, booking_org.excl_thu as booking_organisation_excl_thu, 
                        booking_org.excl_fri as booking_organisation_excl_fri, booking_org.excl_sat as booking_organisation_excl_sat, 
                        booking_org.sun_start_time as booking_organisation_sun_start_time, booking_org.sun_end_time as booking_organisation_sun_end_time, 
                        booking_org.mon_start_time as booking_organisation_mon_start_time, booking_org.mon_end_time as booking_organisation_mon_end_time, booking_org.tue_start_time as booking_organisation_tue_start_time, 
                        booking_org.tue_end_time as booking_organisation_tue_end_time, booking_org.wed_start_time as booking_organisation_wed_start_time, booking_org.wed_end_time as booking_organisation_wed_end_time, 
                        booking_org.thu_start_time as booking_organisation_thu_start_time, booking_org.thu_end_time as booking_organisation_thu_end_time, booking_org.fri_start_time as booking_organisation_fri_start_time, 
                        booking_org.fri_end_time as booking_organisation_fri_end_time, booking_org.sat_start_time as booking_organisation_sat_start_time, booking_org.sat_end_time as booking_organisation_sat_end_time, 
                        booking_org.sun_lunch_start_time as booking_organisation_sun_lunch_start_time, booking_org.sun_lunch_end_time as booking_organisation_sun_lunch_end_time, 
                        booking_org.mon_lunch_start_time as booking_organisation_mon_lunch_start_time, booking_org.mon_lunch_end_time as booking_organisation_mon_lunch_end_time, booking_org.tue_lunch_start_time as booking_organisation_tue_lunch_start_time, 
                        booking_org.tue_lunch_end_time as booking_organisation_tue_lunch_end_time, booking_org.wed_lunch_start_time as booking_organisation_wed_lunch_start_time, booking_org.wed_lunch_end_time as booking_organisation_wed_lunch_end_time, 
                        booking_org.thu_lunch_start_time as booking_organisation_thu_lunch_start_time, booking_org.thu_lunch_end_time as booking_organisation_thu_lunch_end_time, booking_org.fri_lunch_start_time as booking_organisation_fri_lunch_start_time, 
                        booking_org.fri_lunch_end_time as booking_organisation_fri_lunch_end_time, booking_org.sat_lunch_start_time as booking_organisation_sat_lunch_start_time, booking_org.sat_lunch_end_time as booking_organisation_sat_lunch_end_time, 
                        booking_org.last_batch_run as booking_organisation_last_batch_run, booking_org.is_deleted as booking_organisation_is_deleted,

                        booking_patient.patient_id as booking_patient_patient_id, booking_patient.person_id as booking_patient_person_id, booking_patient.patient_date_added as booking_patient_patient_date_added, booking_patient.is_clinic_patient as booking_patient_is_clinic_patient, booking_patient.is_gp_patient as booking_patient_is_gp_patient, 
                        booking_patient.is_deleted as booking_patient_is_deleted, booking_patient.is_deceased as booking_patient_is_deceased, 
                        booking_patient.flashing_text as booking_patient_flashing_text, booking_patient.flashing_text_added_by as booking_patient_flashing_text_added_by, booking_patient.flashing_text_last_modified_date as booking_patient_flashing_text_last_modified_date, 
                        booking_patient.private_health_fund as booking_patient_private_health_fund, booking_patient.concession_card_number as booking_patient_concession_card_number, booking_patient.concession_card_expiry_date as booking_patient_concession_card_expiry_date, booking_patient.is_diabetic as booking_patient_is_diabetic, booking_patient.is_member_diabetes_australia as booking_patient_is_member_diabetes_australia, booking_patient.diabetic_assessment_review_date as booking_patient_diabetic_assessment_review_date, booking_patient.ac_inv_offering_id as booking_patient_ac_inv_offering_id, booking_patient.ac_pat_offering_id as booking_patient_ac_pat_offering_id, booking_patient.login as booking_patient_login, booking_patient.pwd as booking_patient_pwd, booking_patient.is_company as booking_patient_is_company, booking_patient.abn as booking_patient_abn, 
                        booking_patient.next_of_kin_name as booking_patient_next_of_kin_name, booking_patient.next_of_kin_relation as booking_patient_next_of_kin_relation, booking_patient.next_of_kin_contact_info as booking_patient_next_of_kin_contact_info,

                        " + PersonDB.GetFields("booking_patient_person_", "booking_patient_person") + @",
                        title_booking_patient.title_id as title_booking_patient_title_id, title_booking_patient.descr as title_booking_patient_descr,

                        booking_staff.staff_id as booking_staff_staff_id,booking_staff.person_id as booking_staff_person_id,booking_staff.login as booking_staff_login,booking_staff.pwd as booking_staff_pwd,booking_staff.staff_position_id as booking_staff_staff_position_id,
                        booking_staff.field_id as booking_staff_field_id,booking_staff.is_contractor as booking_staff_is_contractor,booking_staff.tfn as booking_staff_tfn,
                        booking_staff.is_fired as booking_staff_is_fired,booking_staff.costcentre_id as booking_staff_costcentre_id,
                        booking_staff.provider_number as booking_staff_provider_number,booking_staff.is_commission as booking_staff_is_commission,booking_staff.commission_percent as booking_staff_commission_percent,
                        booking_staff.is_stakeholder as booking_staff_is_stakeholder, booking_staff.is_master_admin as booking_staff_is_master_admin, booking_staff.is_admin as booking_staff_is_admin, booking_staff.is_principal as booking_staff_is_principal, booking_staff.is_provider as booking_staff_is_provider, booking_staff.is_external as booking_staff_is_external,
                        booking_staff.staff_date_added as booking_staff_staff_date_added,booking_staff.start_date as booking_staff_start_date,booking_staff.end_date as booking_staff_end_date,booking_staff.comment as booking_staff_comment, 
                        booking_staff.num_days_to_display_on_booking_screen as booking_staff_num_days_to_display_on_booking_screen,
                        booking_staff.show_header_on_booking_screen as booking_staff_show_header_on_booking_screen,
                        booking_staff.bk_screen_field_id as booking_staff_bk_screen_field_id,
                        booking_staff.bk_screen_show_key as booking_staff_bk_screen_show_key,
                        booking_staff.enable_daily_reminder_sms as booking_staff_enable_daily_reminder_sms, 
                        booking_staff.enable_daily_reminder_email as booking_staff_enable_daily_reminder_email,


                        " + PersonDB.GetFields("booking_staff_person_", "booking_staff_person") + @",
                        title_booking_staff.title_id as title_booking_staff_title_id, title_booking_staff.descr as title_booking_staff_descr,

                        ";
                sql += @"

                        payer_organisation.organisation_id as payer_organisation_organisation_id, payer_organisation.entity_id as payer_organisation_entity_id, payer_organisation.parent_organisation_id as payer_organisation_parent_organisation_id, payer_organisation.use_parent_offernig_prices as payer_organisation_use_parent_offernig_prices, 
                        payer_organisation.organisation_type_id as payer_organisation_organisation_type_id, 
                        payer_organisation.organisation_customer_type_id as payer_organisation_organisation_customer_type_id,payer_organisation.name as payer_organisation_name, payer_organisation.acn as payer_organisation_acn, payer_organisation.abn as payer_organisation_abn, 
                        payer_organisation.organisation_date_added as payer_organisation_organisation_date_added, payer_organisation.organisation_date_modified as payer_organisation_organisation_date_modified, payer_organisation.is_debtor as payer_organisation_is_debtor, payer_organisation.is_creditor as payer_organisation_is_creditor, 
                        payer_organisation.bpay_account as payer_organisation_bpay_account, payer_organisation.weeks_per_service_cycle as payer_organisation_weeks_per_service_cycle, payer_organisation.start_date as payer_organisation_start_date, 
                        payer_organisation.end_date as payer_organisation_end_date, payer_organisation.comment as payer_organisation_comment, payer_organisation.free_services as payer_organisation_free_services, payer_organisation.excl_sun as payer_organisation_excl_sun, 
                        payer_organisation.excl_mon as payer_organisation_excl_mon, payer_organisation.excl_tue as payer_organisation_excl_tue, payer_organisation.excl_wed as payer_organisation_excl_wed, payer_organisation.excl_thu as payer_organisation_excl_thu, 
                        payer_organisation.excl_fri as payer_organisation_excl_fri, payer_organisation.excl_sat as payer_organisation_excl_sat, 
                        payer_organisation.sun_start_time as payer_organisation_sun_start_time, payer_organisation.sun_end_time as payer_organisation_sun_end_time, 
                        payer_organisation.mon_start_time as payer_organisation_mon_start_time, payer_organisation.mon_end_time as payer_organisation_mon_end_time, payer_organisation.tue_start_time as payer_organisation_tue_start_time, 
                        payer_organisation.tue_end_time as payer_organisation_tue_end_time, payer_organisation.wed_start_time as payer_organisation_wed_start_time, payer_organisation.wed_end_time as payer_organisation_wed_end_time, 
                        payer_organisation.thu_start_time as payer_organisation_thu_start_time, payer_organisation.thu_end_time as payer_organisation_thu_end_time, payer_organisation.fri_start_time as payer_organisation_fri_start_time, 
                        payer_organisation.fri_end_time as payer_organisation_fri_end_time, payer_organisation.sat_start_time as payer_organisation_sat_start_time, payer_organisation.sat_end_time as payer_organisation_sat_end_time, 
                        payer_organisation.sun_lunch_start_time as payer_organisation_sun_lunch_start_time, payer_organisation.sun_lunch_end_time as payer_organisation_sun_lunch_end_time, 
                        payer_organisation.mon_lunch_start_time as payer_organisation_mon_lunch_start_time, payer_organisation.mon_lunch_end_time as payer_organisation_mon_lunch_end_time, payer_organisation.tue_lunch_start_time as payer_organisation_tue_lunch_start_time, 
                        payer_organisation.tue_lunch_end_time as payer_organisation_tue_lunch_end_time, payer_organisation.wed_lunch_start_time as payer_organisation_wed_lunch_start_time, payer_organisation.wed_lunch_end_time as payer_organisation_wed_lunch_end_time, 
                        payer_organisation.thu_lunch_start_time as payer_organisation_thu_lunch_start_time, payer_organisation.thu_lunch_end_time as payer_organisation_thu_lunch_end_time, payer_organisation.fri_lunch_start_time as payer_organisation_fri_lunch_start_time, 
                        payer_organisation.fri_lunch_end_time as payer_organisation_fri_lunch_end_time, payer_organisation.sat_lunch_start_time as payer_organisation_sat_lunch_start_time, payer_organisation.sat_lunch_end_time as payer_organisation_sat_lunch_end_time, 
                        payer_organisation.last_batch_run as payer_organisation_last_batch_run, payer_organisation.is_deleted as payer_organisation_is_deleted,


                        payer_patient.patient_id as payer_patient_patient_id, payer_patient.person_id as payer_patient_person_id, payer_patient.patient_date_added as payer_patient_patient_date_added, payer_patient.is_clinic_patient as payer_patient_is_clinic_patient, payer_patient.is_gp_patient as payer_patient_is_gp_patient, payer_patient.is_deleted as payer_patient_is_deleted, payer_patient.is_deceased as payer_patient_is_deceased, 
                        payer_patient.flashing_text as payer_patient_flashing_text, payer_patient.flashing_text_added_by as payer_patient_flashing_text_added_by, payer_patient.flashing_text_last_modified_date as payer_patient_flashing_text_last_modified_date, 
                        payer_patient.private_health_fund as payer_patient_private_health_fund, payer_patient.concession_card_number as payer_patient_concession_card_number, payer_patient.concession_card_expiry_date as payer_patient_concession_card_expiry_date, payer_patient.is_diabetic as payer_patient_is_diabetic, payer_patient.is_member_diabetes_australia as payer_patient_is_member_diabetes_australia, payer_patient.diabetic_assessment_review_date as payer_patient_diabetic_assessment_review_date, payer_patient.ac_inv_offering_id as payer_patient_ac_inv_offering_id, payer_patient.ac_pat_offering_id as payer_patient_ac_pat_offering_id, payer_patient.login as payer_patient_login, payer_patient.pwd as payer_patient_pwd, payer_patient.is_company as payer_patient_is_company, payer_patient.abn as payer_patient_abn, 
                        payer_patient.next_of_kin_name as payer_patient_next_of_kin_name, payer_patient.next_of_kin_relation as payer_patient_next_of_kin_relation, payer_patient.next_of_kin_contact_info as payer_patient_next_of_kin_contact_info,

                        " + PersonDB.GetFields("payer_patient_person_", "payer_patient_person") + @",
                        title_payer_patient.title_id as title_payer_patient_title_id, title_payer_patient.descr as title_payer_patient_descr,


                        non_booking_invoice_organisation.organisation_id as non_booking_invoice_organisation_organisation_id, non_booking_invoice_organisation.entity_id as non_booking_invoice_organisation_entity_id, non_booking_invoice_organisation.parent_organisation_id as non_booking_invoice_organisation_parent_organisation_id, non_booking_invoice_organisation.use_parent_offernig_prices as non_booking_invoice_organisation_use_parent_offernig_prices, 
                        non_booking_invoice_organisation.organisation_type_id as non_booking_invoice_organisation_organisation_type_id, 
                        non_booking_invoice_organisation.organisation_customer_type_id as non_booking_invoice_organisation_organisation_customer_type_id,non_booking_invoice_organisation.name as non_booking_invoice_organisation_name, non_booking_invoice_organisation.acn as non_booking_invoice_organisation_acn, non_booking_invoice_organisation.abn as non_booking_invoice_organisation_abn, 
                        non_booking_invoice_organisation.organisation_date_added as non_booking_invoice_organisation_organisation_date_added, non_booking_invoice_organisation.organisation_date_modified as non_booking_invoice_organisation_organisation_date_modified, non_booking_invoice_organisation.is_debtor as non_booking_invoice_organisation_is_debtor, non_booking_invoice_organisation.is_creditor as non_booking_invoice_organisation_is_creditor, 
                        non_booking_invoice_organisation.bpay_account as non_booking_invoice_organisation_bpay_account, non_booking_invoice_organisation.weeks_per_service_cycle as non_booking_invoice_organisation_weeks_per_service_cycle, non_booking_invoice_organisation.start_date as non_booking_invoice_organisation_start_date, 
                        non_booking_invoice_organisation.end_date as non_booking_invoice_organisation_end_date, non_booking_invoice_organisation.comment as non_booking_invoice_organisation_comment, non_booking_invoice_organisation.free_services as non_booking_invoice_organisation_free_services, non_booking_invoice_organisation.excl_sun as non_booking_invoice_organisation_excl_sun, 
                        non_booking_invoice_organisation.excl_mon as non_booking_invoice_organisation_excl_mon, non_booking_invoice_organisation.excl_tue as non_booking_invoice_organisation_excl_tue, non_booking_invoice_organisation.excl_wed as non_booking_invoice_organisation_excl_wed, non_booking_invoice_organisation.excl_thu as non_booking_invoice_organisation_excl_thu, 
                        non_booking_invoice_organisation.excl_fri as non_booking_invoice_organisation_excl_fri, non_booking_invoice_organisation.excl_sat as non_booking_invoice_organisation_excl_sat, 
                        non_booking_invoice_organisation.sun_start_time as non_booking_invoice_organisation_sun_start_time, non_booking_invoice_organisation.sun_end_time as non_booking_invoice_organisation_sun_end_time, 
                        non_booking_invoice_organisation.mon_start_time as non_booking_invoice_organisation_mon_start_time, non_booking_invoice_organisation.mon_end_time as non_booking_invoice_organisation_mon_end_time, non_booking_invoice_organisation.tue_start_time as non_booking_invoice_organisation_tue_start_time, 
                        non_booking_invoice_organisation.tue_end_time as non_booking_invoice_organisation_tue_end_time, non_booking_invoice_organisation.wed_start_time as non_booking_invoice_organisation_wed_start_time, non_booking_invoice_organisation.wed_end_time as non_booking_invoice_organisation_wed_end_time, 
                        non_booking_invoice_organisation.thu_start_time as non_booking_invoice_organisation_thu_start_time, non_booking_invoice_organisation.thu_end_time as non_booking_invoice_organisation_thu_end_time, non_booking_invoice_organisation.fri_start_time as non_booking_invoice_organisation_fri_start_time, 
                        non_booking_invoice_organisation.fri_end_time as non_booking_invoice_organisation_fri_end_time, non_booking_invoice_organisation.sat_start_time as non_booking_invoice_organisation_sat_start_time, non_booking_invoice_organisation.sat_end_time as non_booking_invoice_organisation_sat_end_time, 
                        non_booking_invoice_organisation.sun_lunch_start_time as non_booking_invoice_organisation_sun_lunch_start_time, non_booking_invoice_organisation.sun_lunch_end_time as non_booking_invoice_organisation_sun_lunch_end_time, 
                        non_booking_invoice_organisation.mon_lunch_start_time as non_booking_invoice_organisation_mon_lunch_start_time, non_booking_invoice_organisation.mon_lunch_end_time as non_booking_invoice_organisation_mon_lunch_end_time, non_booking_invoice_organisation.tue_lunch_start_time as non_booking_invoice_organisation_tue_lunch_start_time, 
                        non_booking_invoice_organisation.tue_lunch_end_time as non_booking_invoice_organisation_tue_lunch_end_time, non_booking_invoice_organisation.wed_lunch_start_time as non_booking_invoice_organisation_wed_lunch_start_time, non_booking_invoice_organisation.wed_lunch_end_time as non_booking_invoice_organisation_wed_lunch_end_time, 
                        non_booking_invoice_organisation.thu_lunch_start_time as non_booking_invoice_organisation_thu_lunch_start_time, non_booking_invoice_organisation.thu_lunch_end_time as non_booking_invoice_organisation_thu_lunch_end_time, non_booking_invoice_organisation.fri_lunch_start_time as non_booking_invoice_organisation_fri_lunch_start_time, 
                        non_booking_invoice_organisation.fri_lunch_end_time as non_booking_invoice_organisation_fri_lunch_end_time, non_booking_invoice_organisation.sat_lunch_start_time as non_booking_invoice_organisation_sat_lunch_start_time, non_booking_invoice_organisation.sat_lunch_end_time as non_booking_invoice_organisation_sat_lunch_end_time, 
                        non_booking_invoice_organisation.last_batch_run as non_booking_invoice_organisation_last_batch_run, non_booking_invoice_organisation.is_deleted as non_booking_invoice_organisation_is_deleted,


                        staff.staff_id as staff_staff_id,staff.person_id as staff_person_id,staff.login as staff_login,staff.pwd as staff_pwd,staff.staff_position_id as staff_staff_position_id,
                        staff.field_id as staff_field_id,staff.is_contractor as staff_is_contractor,staff.tfn as staff_tfn,
                        staff.is_fired as staff_is_fired,staff.costcentre_id as staff_costcentre_id,
                        staff.provider_number as staff_provider_number,staff.is_commission as staff_is_commission,staff.commission_percent as staff_commission_percent,
                        staff.is_stakeholder as staff_is_stakeholder,staff.is_master_admin as staff_is_master_admin,staff.is_admin as staff_is_admin,staff.is_principal as staff_is_principal,staff.is_provider as staff_is_provider, staff.is_external as staff_is_external,
                        staff.staff_date_added as staff_staff_date_added,staff.start_date as staff_start_date,staff.end_date as staff_end_date,staff.comment as staff_comment, 
                        staff.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen,
                        staff.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                        staff.bk_screen_field_id as staff_bk_screen_field_id,
                        staff.bk_screen_show_key as staff_bk_screen_show_key,
                        staff.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                        staff.enable_daily_reminder_email as staff_enable_daily_reminder_email,

                        " + PersonDB.GetFields("staff_person_", "staff_person") + @",
                        title_staff.title_id as title_staff_title_id, title_staff.descr as title_staff_descr,


                        site.site_id as site_site_id,site.entity_id as site_entity_id,site.name as site_name,site.site_type_id as site_site_type_id,site.abn as site_abn,site.acn as site_acn,site.tfn as site_tfn,
                        site.asic as site_asic,site.is_provider as site_is_provider,site.bank_bpay as site_bank_bpay,site.bank_bsb as site_bank_bsb,site.bank_account as site_bank_account,
                        site.bank_direct_debit_userid as site_bank_direct_debit_userid,site.bank_username as site_bank_username,site.oustanding_balance_warning as site_oustanding_balance_warning,
                        site.print_epc as site_print_epc,site.excl_sun as site_excl_sun,site.excl_mon as site_excl_mon,site.excl_tue as site_excl_tue,site.excl_wed as site_excl_wed,site.excl_thu as site_excl_thu,
                        site.excl_fri as site_excl_fri,site.excl_sat as site_excl_sat,site.day_start_time as site_day_start_time,site.lunch_start_time as site_lunch_start_time,
                        site.lunch_end_time as site_lunch_end_time,site.day_end_time as site_day_end_time,site.fiscal_yr_end as site_fiscal_yr_end,site.num_booking_months_to_get as site_num_booking_months_to_get,

                        sitetype.descr as site_type_descr 

                FROM 
                        Invoice inv
                        INNER      JOIN InvoiceType            invoice_type           ON inv.invoice_type_id              = invoice_type.invoice_type_id
                        LEFT OUTER JOIN Booking                booking                ON inv.booking_id                   = booking.booking_id
                        ";
            if (inc_deep_booking_info)
                sql += @"
                        LEFT OUTER JOIN BookingStatus          booking_status         ON booking.booking_status_id        = booking_status.booking_status_id
                        LEFT OUTER JOIN BookingConfirmedByType      booking_confirmed_by_type     ON booking.booking_confirmed_by_type_id     = booking_confirmed_by_type.booking_confirmed_by_type_id
                        LEFT OUTER JOIN BookingUnavailabilityReason booking_unavailability_reason ON booking.booking_unavailability_reason_id = booking_unavailability_reason.booking_unavailability_reason_id
                        LEFT OUTER JOIN Offering               booking_offering       ON booking.offering_id              = booking_offering.offering_id
                        LEFT OUTER JOIN Organisation           booking_org            ON booking.organisation_id          = booking_org.organisation_id
                        LEFT OUTER JOIN Patient                booking_patient        ON booking.patient_id               = booking_patient.patient_id
                        LEFT OUTER JOIN Person                 booking_patient_person ON booking_patient_person.person_id = booking_patient.person_id
                        LEFT OUTER JOIN Title                  title_booking_patient  ON title_booking_patient.title_id   = booking_patient_person.title_id
                        LEFT OUTER JOIN Staff                  booking_staff          ON booking.provider                 = booking_staff.staff_id
                        LEFT OUTER JOIN Person                 booking_staff_person   ON booking_staff_person.person_id   = booking_staff.person_id
                        LEFT OUTER JOIN Title                  title_booking_staff    ON title_booking_staff.title_id     = booking_staff_person.title_id

                        ";
                sql += @"

                        LEFT OUTER JOIN Organisation           payer_organisation     ON inv.payer_organisation_id        = payer_organisation.organisation_id

                        LEFT OUTER JOIN Patient                payer_patient          ON inv.payer_patient_id             = payer_patient.patient_id
                        LEFT OUTER JOIN Person                 payer_patient_person   ON payer_patient_person.person_id   = payer_patient.person_id
                        LEFT OUTER JOIN Title                  title_payer_patient    ON title_payer_patient.title_id     = payer_patient_person.title_id

                        LEFT OUTER JOIN Organisation           non_booking_invoice_organisation ON inv.non_booking_invoice_organisation_id = non_booking_invoice_organisation.organisation_id

                        INNER      JOIN Staff                  staff                  ON inv.staff_id                     = staff.staff_id
                        LEFT OUTER JOIN Person                 staff_person           ON staff_person.person_id           = staff.person_id
                        LEFT OUTER JOIN Title                  title_staff            ON title_staff.title_id             = staff_person.title_id

                        LEFT OUTER JOIN Site                   site                   ON inv.site_id                      = site.site_id
                        LEFT OUTER JOIN SiteType               sitetype               ON site.site_type_id                = sitetype.site_type_id
                    ";

        return sql;
    }

    #endregion

    public static void AddTotalDueColumn(ref DataTable tblInvoices)
    {
        tblInvoices.Columns.Add("total_due", typeof(Decimal));
        for (int i = 0; i < tblInvoices.Rows.Count; i++)
            tblInvoices.Rows[i]["total_due"] = Convert.ToDecimal(tblInvoices.Rows[i]["inv_total"]) - Convert.ToDecimal(tblInvoices.Rows[i]["inv_credit_notes_total"]) - Convert.ToDecimal(tblInvoices.Rows[i]["inv_receipts_total"]) - Convert.ToDecimal(tblInvoices.Rows[i]["inv_vouchers_total"]);
    }

    public static Invoice LoadAll(DataRow row, bool inc_deep_booking_info = true)
    {
        Invoice fd = Load(row, "inv_");
        
        fd.InvoiceType = IDandDescrDB.Load(row, "invoice_type_invoice_type_id", "invoice_type_descr");

        if (row["booking_booking_id"] != DBNull.Value)
        {
            fd.Booking = BookingDB.Load(row, "booking_", inc_deep_booking_info, inc_deep_booking_info);

            if (inc_deep_booking_info)
            {
                fd.Booking.BookingStatus = IDandDescrDB.Load(row, "booking_status_booking_status_id", "booking_status_descr");

                if (row["booking_offering_offering_id"] != DBNull.Value)
                    fd.Booking.Offering = OfferingDB.Load(row, "booking_offering_");
                if (row["booking_organisation_organisation_id"] != DBNull.Value)
                    fd.Booking.Organisation = OrganisationDB.Load(row, "booking_organisation_");
                if (row["booking_patient_patient_id"] != DBNull.Value)
                {
                    fd.Booking.Patient = PatientDB.Load(row, "booking_patient_");
                    fd.Booking.Patient.Person = PersonDB.Load(row, "booking_patient_person_");
                    fd.Booking.Patient.Person.Title = IDandDescrDB.Load(row, "title_booking_patient_title_id", "title_booking_patient_descr");
                }
                if (row["booking_staff_staff_id"] != DBNull.Value)
                {
                    fd.Booking.Provider = StaffDB.Load(row, "booking_staff_");
                    fd.Booking.Provider.Person = PersonDB.Load(row, "booking_staff_person_");
                    fd.Booking.Provider.Person.Title = IDandDescrDB.Load(row, "title_booking_staff_title_id", "title_booking_staff_descr");
                }
            }

        }

        if (row["payer_organisation_organisation_id"] != DBNull.Value)
            fd.PayerOrganisation = OrganisationDB.Load(row, "payer_organisation_");

        if (row["payer_patient_patient_id"] != DBNull.Value)
            fd.PayerPatient = PatientDB.Load(row, "payer_patient_");
        if (row["payer_patient_person_person_id"] != DBNull.Value)
        {
            fd.PayerPatient.Person = PersonDB.Load(row, "payer_patient_person_");
            fd.PayerPatient.Person.Title = IDandDescrDB.Load(row, "title_payer_patient_title_id", "title_payer_patient_descr");
        }

        if (row["non_booking_invoice_organisation_organisation_id"] != DBNull.Value)
            fd.NonBookinginvoiceOrganisation = OrganisationDB.Load(row, "non_booking_invoice_organisation_");

        fd.Staff = StaffDB.Load(row, "staff_");
        fd.Staff.Person = PersonDB.Load(row, "staff_person_");
        fd.Staff.Person.Title = IDandDescrDB.Load(row, "title_staff_title_id", "title_staff_descr");

        if (row["site_site_id"] != DBNull.Value)
            fd.Site = SiteDB.Load(row, "site_");

        return fd;
    }
    public static Invoice Load(DataRow row, string prefix = "")
    {
        return new Invoice(
            Convert.ToInt32(row[prefix+"invoice_id"]),
            Convert.ToInt32(row[prefix + "entity_id"]),
            Convert.ToInt32(row[prefix+"invoice_type_id"]),
            row[prefix + "booking_id"]            == DBNull.Value ? -1 : Convert.ToInt32(row[prefix+"booking_id"]),
            row[prefix + "payer_organisation_id"] == DBNull.Value ?  0 : Convert.ToInt32(row[prefix+"payer_organisation_id"]),
            row[prefix + "payer_patient_id"]      == DBNull.Value ? -1 : Convert.ToInt32(row[prefix+"payer_patient_id"]),
            row[prefix + "non_booking_invoice_organisation_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "non_booking_invoice_organisation_id"]),
            Convert.ToString(row[prefix + "healthcare_claim_number"]),
            row[prefix + "reject_letter_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "reject_letter_id"]),
            Convert.ToString(row[prefix + "message"]),
            Convert.ToInt32(row[prefix+"staff_id"]),
            row[prefix + "site_id"]               == DBNull.Value ? -1 : Convert.ToInt32(row[prefix+"site_id"]),
            row[prefix + "invoice_date_added"]    == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "invoice_date_added"]),
            Convert.ToDecimal(row[prefix+"total"]),
            Convert.ToDecimal(row[prefix+"gst"]),
            Convert.ToDecimal(row[prefix+"receipts_total"]),
            Convert.ToDecimal(row[prefix + "vouchers_total"]),
            Convert.ToDecimal(row[prefix + "credit_notes_total"]),
            Convert.ToDecimal(row[prefix + "refunds_total"]),
            Convert.ToBoolean(row[prefix+"is_paid"]),
            Convert.ToBoolean(row[prefix+"is_refund"]),
            Convert.ToBoolean(row[prefix + "is_batched"]),
            row[prefix + "reversed_by"]       == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "reversed_by"]),
            row[prefix + "reversed_date"]     == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "reversed_date"]),
            row[prefix + "last_date_emailed"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "last_date_emailed"])
            );
    }

}