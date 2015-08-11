using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class LetterBestPrintHistoryDB
{

    public static void Delete(int letter_print_history_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM LetterBestPrintHistory WHERE letter_print_history_id = " + letter_print_history_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int letter_id, int patient_id)
    {
        string sql = "INSERT INTO LetterBestPrintHistory (letter_id,patient_id) VALUES (" + "" + letter_id + "," + "" + (patient_id == -1 ? "NULL" : patient_id.ToString()) + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int letter_print_history_id, int letter_id, int patient_id)
    {
        string sql = "UPDATE LetterBestPrintHistory SET letter_id = " + letter_id + ",patient_id = " + (patient_id == -1 ? "NULL" : patient_id.ToString()) + " WHERE letter_print_history_id = " + letter_print_history_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }



    public static string JoinedSql = @"
                        SELECT
                                lph.letter_print_history_id as lph_letter_print_history_id,lph.letter_id as lph_letter_id,lph.patient_id as lph_patient_id,lph.date as lph_date
                                ,
                                letter.letter_id as letter_letter_id,letter.letter_type_id as letter_letter_type_id,
                                letter.code as letter_code,letter.docname as letter_docname,letter.is_send_to_medico as letter_is_send_to_medico,letter.is_allowed_reclaim as letter_is_allowed_reclaim,letter.is_manual_override as letter_is_manual_override,letter.num_copies_to_print as letter_num_copies_to_print
                                ,
                                lettertype.descr as lettertype_descr, lettertype.letter_type_id as lettertype_letter_type_id
                                ,

                                patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, patient.is_clinic_patient as patient_is_clinic_patient, patient.is_gp_patient as patient_is_gp_patient,
                                patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                                patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                                patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn,
                                patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info
                                ,
                                " + PersonDB.GetFields("person_patient_", "person_patient") + @"
                                ,
                                title_patient.title_id as title_patient_title_id, title_patient.descr as title_patient_descr

                        FROM 
                                LetterBestPrintHistory lph

                                INNER JOIN LetterBest        letter          ON lph.letter_id              = letter.letter_id
                                INNER JOIN LetterType        lettertype      ON letter.letter_type_id      = lettertype.letter_type_id 
                                
                                LEFT OUTER JOIN Patient      patient         ON patient.patient_id         = lph.patient_id 
                                LEFT OUTER JOIN Person       person_patient  ON person_patient.person_id   = patient.person_id
                                LEFT OUTER JOIN Title        title_patient   ON title_patient.title_id     = person_patient.title_id ";



    public static DataTable GetDataTable(int patient_id = -1)
    {
        string sql = JoinedSql;

        if (patient_id != -1)
        {
            string where = string.Empty;
            if (patient_id != -1)
                where += " AND lph.patient_id = " + patient_id;

            sql += " WHERE " + where.Substring(5);
        }

        sql += " ORDER BY date DESC ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static LetterBestPrintHistory GetByID(int letter_print_history_id)
    {
        string sql = JoinedSql + " WHERE lph.letter_print_history_id = " + letter_print_history_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static LetterBestPrintHistory LoadAll(DataRow row)
    {
        LetterBestPrintHistory lph = Load(row, "lph_");

        lph.Letter = LetterBestDB.Load(row, "letter_");

        lph.Letter.LetterType = IDandDescrDB.Load(row, "lettertype_letter_type_id", "lettertype_descr");

        if (row["patient_patient_id"] != DBNull.Value)
            lph.Patient = PatientDB.Load(row, "patient_");
        if (row["patient_patient_id"] != DBNull.Value)
        {
            lph.Patient.Person = PersonDB.Load(row, "person_patient_");
            lph.Patient.Person.Title = IDandDescrDB.Load(row, "title_patient_title_id", "title_patient_descr");
        }

        return lph;
    }


    public static LetterBestPrintHistory Load(DataRow row, string prefix = "")
    {
        return new LetterBestPrintHistory(
            Convert.ToInt32(row[prefix+"letter_print_history_id"]),
            Convert.ToInt32(row[prefix+"letter_id"]),
            row[prefix + "patient_id"]            == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "patient_id"]),
            Convert.ToDateTime(row[prefix+"date"])
        );
    }

}