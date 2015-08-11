using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

public class InvoiceLineDB
{

    public static void Delete(int invoice_line_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM InvoiceLine WHERE invoice_line_id = " + invoice_line_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int invoice_id, int patient_id, int offering_id, decimal quantity, decimal price, decimal tax, string area_treated, string service_reference, int offering_order_id)
    {
        area_treated = area_treated.Replace("'", "''");
        service_reference = service_reference.Replace("'", "''");

        string sql = "INSERT INTO InvoiceLine (invoice_id,patient_id,offering_id,quantity,price,tax, area_treated,service_reference,offering_order_id) VALUES (" + "" + invoice_id + "," + "" + (patient_id == -1 ? "NULL" : patient_id.ToString()) + "," + "" + (offering_id == -1 ? "NULL" : offering_id.ToString()) + "," + "" + quantity + "," + "" + price + "," + "" + tax + "" + ",'" + area_treated + "','" + service_reference + "'," + (offering_order_id == -1 ? "NULL" : offering_order_id.ToString()) + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static void Update(int invoice_line_id, int invoice_id, int patient_id, int offering_id, decimal quantity, decimal price, decimal tax, string area_treated, string service_reference, int offering_order_id)
    {
        area_treated = area_treated.Replace("'", "''");
        service_reference = service_reference.Replace("'", "''");

        string sql = "UPDATE InvoiceLine SET invoice_id = " + invoice_id + ",patient_id = " + (patient_id == -1 ? "NULL" : patient_id.ToString()) + ",offering_id = " + offering_id + ",quantity = " + quantity + ",price = " + price + ",tax = " + tax + ",area_treated = '" + area_treated + "'" + ",service_reference = '" + service_reference  + "'" + ",offering_order_id = " + (offering_order_id == -1 ? "NULL" : offering_order_id.ToString()) + " WHERE invoice_line_id = " + invoice_line_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateAreaTreated(int invoice_line_id, string area_treated)
    {
        area_treated = area_treated.Replace("'", "''");

        string sql = "UPDATE InvoiceLine SET area_treated = '" + area_treated + "' WHERE invoice_line_id = " + invoice_line_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateServiceReference(int invoice_line_id, string service_reference)
    {
        service_reference = service_reference.Replace("'", "''");

        string sql = "UPDATE InvoiceLine SET service_reference = '" + service_reference + "' WHERE invoice_line_id = " + invoice_line_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateRemoveOfferingOrder(int invoice_line_id, int offering_order_id)
    {
        string sql = "UPDATE InvoiceLine SET offering_order_id = NULL WHERE invoice_line_id = " + invoice_line_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    #region Joined SQL

    protected static string JoinedSql = @"
                SELECT 
                        InvoiceLine.invoice_line_id,InvoiceLine.invoice_id,InvoiceLine.patient_id,InvoiceLine.offering_id,InvoiceLine.quantity,InvoiceLine.price,tax,InvoiceLine.area_treated,InvoiceLine.service_reference,InvoiceLine.offering_order_id,

                        Offering.offering_id, Offering.offering_type_id, Offering.field_id, Offering.aged_care_patient_type_id, 
                        Offering.num_clinic_visits_allowed_per_year, Offering.offering_invoice_type_id, 
                        Offering.name, Offering.short_name, Offering.descr, Offering.is_gst_exempt, Offering.default_price, Offering.service_time_minutes, 
                        Offering.max_nbr_claimable,  Offering.max_nbr_claimable_months, Offering.medicare_company_code, Offering.dva_company_code, Offering.tac_company_code, 
                        Offering.medicare_charge, Offering.dva_charge, Offering.tac_charge, Offering.popup_message, Offering.reminder_letter_months_later_to_send, Offering.reminder_letter_id, Offering.use_custom_color, Offering.custom_color, 

                        patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, patient.is_clinic_patient as patient_is_clinic_patient, patient.is_gp_patient as patient_is_gp_patient, patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                        patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                        patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn, 
                        patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info,

                        " + PersonDB.GetFields("patient_person_", "patient_person") + @",
                        title_patient.title_id as title_patient_title_id, title_patient.descr as title_patient_descr

                FROM
                        InvoiceLine
                        LEFT OUTER JOIN Offering                ON Offering.offering_id       = InvoiceLine.offering_id 
                        LEFT OUTER JOIN Patient  patient        ON InvoiceLine.patient_id     = patient.patient_id
                        LEFT OUTER JOIN Person   patient_person ON patient_person.person_id   = patient.person_id
                        LEFT OUTER JOIN Title    title_patient  ON title_patient.title_id     = patient_person.title_id ";

    #endregion


    public static DataTable GetDataTable()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static InvoiceLine GetByID(int invoice_line_id)
    {
        string sql = JoinedSql + " WHERE invoice_line_id = " + invoice_line_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }
    public static InvoiceLine[] GetByInvoiceID(int invoice_id)
    {
        string sql = JoinedSql + " WHERE invoice_id = " + invoice_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        InvoiceLine[] ret = new InvoiceLine[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            ret[i] = LoadAll(tbl.Rows[i]);

        return ret;
    }
    public static InvoiceLine[] GetByInvoiceIDs(int[] invoice_ids)
    {
        string sql = JoinedSql + (invoice_ids != null && invoice_ids.Length > 0 ? " WHERE invoice_id IN (" + string.Join(",", invoice_ids) + ")" : " WHERE 1<>1");
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        InvoiceLine[] ret = new InvoiceLine[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            ret[i] = LoadAll(tbl.Rows[i]);

        return ret;
    }
    public static Hashtable GetBulkInvoiceLinesByInvoiceID(Invoice[] invoices)
    {
        Hashtable hash = new Hashtable();

        int[] invoiceIDs = new int[invoices.Length];
        for (int i = 0; i < invoices.Length; i++)
            invoiceIDs[i] = invoices[i].InvoiceID;
        InvoiceLine[] allInvoiceLines = InvoiceLineDB.GetByInvoiceIDs(invoiceIDs);

        foreach (Invoice curInvoice in invoices)
        {
            System.Collections.ArrayList curInvoiceLines = new System.Collections.ArrayList();
            for (int i = 0; i < allInvoiceLines.Length; i++)
            {
                if (allInvoiceLines[i].InvoiceID == curInvoice.InvoiceID)
                    curInvoiceLines.Add(allInvoiceLines[i]);
            }

            hash[curInvoice.InvoiceID] = (InvoiceLine[])curInvoiceLines.ToArray(typeof(InvoiceLine));
        }

        return hash;
    }

    public static InvoiceLine LoadAll(DataRow row)
    {
        InvoiceLine line = Load(row);
        if (row["offering_id"] != DBNull.Value)
            line.Offering = OfferingDB.Load(row);
        if (row["patient_id"] != DBNull.Value)
        {
            line.Patient = PatientDB.Load(row, "patient_");
            line.Patient.Person = PersonDB.Load(row, "patient_person_");
            line.Patient.Person.Title = IDandDescrDB.Load(row, "title_patient_title_id", "title_patient_descr");
        }

        return line;
    }

    public static InvoiceLine Load(DataRow row, string prefix = "")
    {
        return new InvoiceLine(
            Convert.ToInt32(row[prefix + "invoice_line_id"]),
            Convert.ToInt32(row[prefix + "invoice_id"]),
            row[prefix + "patient_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "patient_id"]),
            row[prefix + "offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "offering_id"]),
            Convert.ToDecimal(row[prefix + "quantity"]),
            Convert.ToDecimal(row[prefix + "price"]),
            Convert.ToDecimal(row[prefix + "tax"]),
            Convert.ToString(row[prefix + "area_treated"]),
            Convert.ToString(row[prefix + "service_reference"]),
            row[prefix + "offering_order_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "offering_order_id"])
        );
    }

}