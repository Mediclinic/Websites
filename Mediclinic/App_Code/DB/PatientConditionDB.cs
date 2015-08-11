using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class PatientConditionDB
{

    public static void Delete(int patient_condition_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM PatientCondition WHERE patient_condition_id = " + patient_condition_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void Delete(int patient_id, int condition_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM PatientCondition WHERE patient_id = " + patient_id.ToString() + " AND condition_id = " + condition_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void DeleteByPatient(int patient_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM PatientCondition WHERE patient_id = " + patient_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int patient_id, int condition_id, DateTime date, int nweeksdue, string text, bool is_deleted)
    {
        text = text.Replace("'", "''");
        string sql = "INSERT INTO PatientCondition (patient_id,condition_id,date,nweeksdue,text,is_deleted) VALUES (" + "" + patient_id + "," + "" + condition_id + "," + (date == DateTime.MinValue ? "NULL" : "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + nweeksdue + "," + "'" + text + "'," + (is_deleted ? "1" : "0") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int patient_condition_id, int patient_id, int condition_id, DateTime date, int nweeksdue, string text, bool is_deleted)
    {
        text = text.Replace("'", "''");
        string sql = "UPDATE PatientCondition SET patient_id = " + patient_id + ",condition_id = " + condition_id + ",date = " + (date == DateTime.MinValue ? "NULL" : "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",nweeksdue = " + nweeksdue + ",text = '" + text + "',is_deleted = " + (is_deleted ? "1" : "0") + " WHERE patient_condition_id = " + patient_condition_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void Update(int patient_id, int condition_id, DateTime date, int nweeksdue, string text, bool is_deleted)
    {
        text = text.Replace("'", "''");
        string sql = "UPDATE PatientCondition SET patient_id = " + patient_id + ",condition_id = " + condition_id + ",date = " + (date == DateTime.MinValue ? "NULL" : "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",nweeksdue = " + nweeksdue + ",text = '" + text + "',is_deleted = " + (is_deleted ? "1" : "0") + " WHERE patient_id = " + patient_id.ToString() + " AND condition_id = " + condition_id.ToString() + " AND is_deleted = 0";
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int patient_condition_id)
    {
        string sql = "UPDATE PatientCondition SET is_deleted = 1 WHERE patient_condition_id = " + patient_condition_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int patient_condition_id)
    {
        string sql = "UPDATE PatientCondition SET is_deleted = 0 WHERE patient_condition_id = " + patient_condition_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static PatientCondition GetByID(int patient_condition_id)
    {
        string sql = JoinedSql + " WHERE patient_condition.patient_condition_id = " + patient_condition_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery( sql ).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static DataTable GetDatatable_ByPatientID(int patient_id, bool inc_deleted = false)
    {
        string sql = JoinedSql + " WHERE patient_condition.patient_id = " + patient_id.ToString() + (inc_deleted ? "" : " AND patient_condition.is_deleted = 0");
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static PatientCondition[] GetByPatientID(int patient_id, bool inc_deleted = false)
    {
        DataTable tbl = GetDatatable_ByPatientID(patient_id, inc_deleted);
        PatientCondition[] list = new PatientCondition[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = PatientConditionDB.LoadAll(tbl.Rows[i]);
        return list;
    }
    public static Hashtable GetHashtable_ByPatientID(int patient_id, bool inc_deleted = false)
    {
        Hashtable selectedConditions = new Hashtable();
        DataTable dt_pt_conditions = GetDatatable_ByPatientID(patient_id, inc_deleted);
        for (int i = 0; i < dt_pt_conditions.Rows.Count; i++)
            selectedConditions[Convert.ToInt32(dt_pt_conditions.Rows[i]["condition_condition_id"])] = PatientConditionDB.LoadAll(dt_pt_conditions.Rows[i]);
        return selectedConditions;
    }

    private static string JoinedSql = @"
            SELECT 
                patient_condition.patient_condition_id as patient_condition_patient_condition_id,
                patient_condition.patient_id           as patient_condition_patient_id,
                patient_condition.condition_id         as patient_condition_condition_id,
                patient_condition.date                 as patient_condition_date,
                patient_condition.nweeksdue            as patient_condition_nweeksdue,
                patient_condition.text                 as patient_condition_text,
                patient_condition.is_deleted           as patient_condition_is_deleted,

                condition.condition_id   as condition_condition_id,
                condition.descr          as condition_descr,
                condition.show_date      as condition_show_date, 
                condition.show_nweeksdue as condition_show_nweeksdue, 
                condition.show_text      as condition_show_text,
                condition.display_order  as condition_display_order,
                condition.is_deleted     as condition_is_deleted,

                patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, 
                patient.is_clinic_patient as patient_is_clinic_patient, patient.is_gp_patient as patient_is_gp_patient,
                patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn, 
                patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info,

                " + PersonDB.GetFields("person_", "person") + @",

                title.title_id as title_title_id, title.descr as title_descr

            FROM 
                PatientCondition patient_condition
                INNER JOIN Condition condition ON condition.condition_id = patient_condition.condition_id
                INNER JOIN Patient   patient   ON patient.patient_id     = patient_condition.patient_id
                INNER JOIN Person    person    ON person.person_id       = patient.person_id
                INNER JOIN Title     title     ON title.title_id         = person.title_id ";


    public static PatientCondition LoadAll(DataRow row)
    {
        PatientCondition patientCondition = Load(row, "patient_condition_");
        patientCondition.Condition = ConditionDB.Load(row, "condition_");
        patientCondition.Patient   = PatientDB.Load(row, "patient_");
        patientCondition.Patient.Person = PersonDB.Load(row, "person_", "entity_id");
        patientCondition.Patient.Person.Title = IDandDescrDB.Load(row, "title_title_id", "title_descr");
        return patientCondition;
    }

    public static PatientCondition Load(DataRow row, string prefix = "")
    {
        return new PatientCondition(
            Convert.ToInt32(row[prefix + "patient_condition_id"]),
            Convert.ToInt32(row[prefix + "patient_id"]),
            Convert.ToInt32(row[prefix + "condition_id"]),
            row[prefix + "date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date"]),
            Convert.ToInt32(row[prefix + "nweeksdue"]),
            Convert.ToString(row[prefix + "text"]),
            Convert.ToBoolean(row[prefix + "is_deleted"])
        );
    }

}