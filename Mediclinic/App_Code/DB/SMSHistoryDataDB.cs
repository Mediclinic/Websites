using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class SMSHistoryDataDB
{

    public static void Delete(int sms_history_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM SMSHistory WHERE sms_history_id = " + sms_history_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int sms_and_email_type_id, int patient_id, int booking_id, string phone_number, string message, decimal cost, string smstech_message_id)
    {
        if (phone_number.Length > 20)
            phone_number = phone_number.Substring(0, 20);
        phone_number = phone_number.Replace("'", "''");
        message = message.Replace("'", "''");
        smstech_message_id = smstech_message_id.Replace("'", "''");
        string sql = "INSERT INTO SMSHistory (sms_and_email_type_id,patient_id,booking_id,phone_number,message,cost,datetime_sent,smstech_message_id,smstech_status,smstech_datetime) VALUES (" + sms_and_email_type_id + "," + (patient_id == -1 ? "NULL" : patient_id.ToString()) + "," + (booking_id == -1 ? "NULL" : booking_id.ToString()) + "," + "'" + phone_number + "'," + "'" + message + "'," + cost + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "'" + smstech_message_id + "'," + "'',NULL);SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(string smstech_message_id, string smstech_status)
    {
        smstech_message_id = smstech_message_id.Replace("'", "''");
        smstech_status = smstech_status.Replace("'", "''");
        string sql = "UPDATE SMSHistory SET smstech_status = '" + smstech_status + "',smstech_datetime = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE  datetime_sent >= '" + DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss") + "' AND smstech_message_id = '" + smstech_message_id + "';";
        //string sql = "UPDATE SMSHistory SET smstech_status = '" + smstech_status + "',smstech_datetime = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE sms_history_id = (SELECT TOP 1 sms_history_id FROM SMSHistory WHERE datetime_sent >= '" + DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss") + "' AND smstech_message_id = '" + smstech_message_id + "' ORDER BY sms_history_id DESC);"; ;
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable(int sms_and_email_type_id, bool ascending = true)
    {
        return DBBase.ExecuteQuery(JoinedSql + (sms_and_email_type_id == -1 ? "" : " WHERE SMSHistory.sms_and_email_type_id = " + sms_and_email_type_id) + (ascending ? "" : " ORDER BY sms_history_id DESC ")).Tables[0];
    }

    public static SMSHistoryData GetByID(int sms_history_id)
    {
        string sql = JoinedSql + " WHERE sms_history_id = " + sms_history_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    protected static string
        JoinedSql = @"

            SELECT 
                    sms_history_id, SMSHistory.sms_and_email_type_id, SMSHistory.patient_id, SMSHistory.booking_id,phone_number,message,cost,datetime_sent,smstech_message_id,smstech_status,smstech_datetime,

                    SMSAndEmailType.sms_and_email_type_id AS sms_and_email_type_sms_and_email_type_id, SMSAndEmailType.descr AS sms_and_email_type_descr, 

                    patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, 
                    patient.is_clinic_patient as patient_is_clinic_patient,patient.is_gp_patient as patient_is_gp_patient,patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                    patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                    patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn, 
                    patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info,

                    " + PersonDB.GetFields("person_patient_", "person_patient") + @",
                    title_patient.title_id as title_patient_title_id, title_patient.descr as title_patient_descr,


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
                    booking.recurring_start_time as booking_recurring_start_time,booking.recurring_end_time as booking_recurring_end_time

            FROM 

                    SMSHistory
                    INNER JOIN SMSAndEmailType                ON SMSAndEmailType.sms_and_email_type_id  = SMSHistory.sms_and_email_type_id
                    LEFT OUTER JOIN Patient patient           ON patient.patient_id                     = SMSHistory.patient_id
                    LEFT OUTER JOIN Person  person_patient    ON person_patient.person_id               = patient.person_id
                    LEFT OUTER JOIN Title   title_patient     ON title_patient.title_id                 = person_patient.title_id
                    LEFT OUTER JOIN Booking booking           ON booking.booking_id                     = SMSHistory.booking_id";


    public static decimal GetTotal()
    {
        string sql = "SELECT COALESCE(sum(cost), 0) FROM SMSHistory";
        return Convert.ToDecimal(DBBase.ExecuteSingleResult(sql));
    }


    public static decimal GetPTReminders(DateTime fromDate, DateTime toDate)
    {
        string sql = "SELECT COALESCE(sum(cost), 0) FROM SMSHistory WHERE patient_id IS NULL AND booking_id IS NOT NULL" +
             (fromDate != DateTime.MinValue ? " AND datetime_sent >= '" + fromDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "") +
             (toDate   != DateTime.MinValue ? " AND datetime_sent <= '" + toDate.ToString("yyyy-MM-dd HH:mm:ss")   + "'" : "");
        return Convert.ToDecimal(DBBase.ExecuteSingleResult(sql));
    }
    public static decimal GetPTBirthdays(DateTime fromDate, DateTime toDate)
    {
        string sql = "SELECT COALESCE(sum(cost), 0) FROM SMSHistory WHERE patient_id IS NOT NULL AND booking_id IS NULL" +
             (fromDate != DateTime.MinValue ? " AND datetime_sent >= '" + fromDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "") +
             (toDate   != DateTime.MinValue ? " AND datetime_sent <= '" + toDate.ToString("yyyy-MM-dd HH:mm:ss")   + "'" : "");
        return Convert.ToDecimal(DBBase.ExecuteSingleResult(sql));
    }
    public static decimal GetStaffReminders(DateTime fromDate, DateTime toDate)
    {
        string sql = "SELECT COALESCE(sum(cost), 0) FROM SMSHistory WHERE patient_id IS NULL AND booking_id IS NULL" +
             (fromDate != DateTime.MinValue ? " AND datetime_sent >= '" + fromDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "") +
             (toDate   != DateTime.MinValue ? " AND datetime_sent <= '" + toDate.ToString("yyyy-MM-dd HH:mm:ss")   + "'" : "");
        return Convert.ToDecimal(DBBase.ExecuteSingleResult(sql));
    }



    public static SMSHistoryData LoadAll(DataRow row)
    {
        SMSHistoryData s = Load(row);
        if (row["booking_booking_id"] != DBNull.Value)
            s.Booking = BookingDB.Load(row, "booking_", false, false);
        if (row["patient_patient_id"] != DBNull.Value)
        {
            s.Patient = PatientDB.Load(row, "patient_");
            s.Patient.Person = PersonDB.Load(row, "person_patient_");
            s.Patient.Person.Title = IDandDescrDB.Load(row, "title_patient_title_id", "title_patient_descr");
        }

        return s;
    }

    public static SMSHistoryData Load(DataRow row)
    {
        return new SMSHistoryData(
            Convert.ToInt32(row["sms_history_id"]),
            Convert.ToInt32(row["sms_and_email_type_id"]),
            row["patient_id"]            == DBNull.Value ? -1 : Convert.ToInt32(row["patient_id"]),
            row["booking_id"]            == DBNull.Value ? -1 : Convert.ToInt32(row["booking_id"]),
            Convert.ToString(row["phone_number"]),
            Convert.ToString(row["message"]),
            Convert.ToDecimal(row["cost"]),
            Convert.ToDateTime(row["datetime_sent"]),
            Convert.ToString(row["smstech_message_id"]),
            Convert.ToString(row["smstech_status"]),
            row["smstech_datetime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["smstech_datetime"])
        );
    }

}