using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class PatientHistoryDB
{

    public static void Delete(int patient_history_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM PatientHistory WHERE patient_history_id = " + patient_history_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int patient_id, bool is_clinic_patient, bool is_gp_patient, bool is_deleted, bool is_deceased, string flashing_text, int flashing_text_added_by, DateTime flashing_text_last_modified_date, string private_health_fund, string concession_card_number, DateTime concession_card_expiry_date, bool is_diabetic, bool is_member_diabetes_australia, DateTime diabetic_assessment_review_date, int ac_inv_offering_id, int ac_pat_offering_id, string login, string pwd, bool is_company, string abn, string next_of_kin_name, string next_of_kin_relation, string next_of_kin_contact_info, int title_id, string firstname, string middlename, string surname, string nickname, string gender, DateTime dob, int modified_from_this_by)
    {
        flashing_text            = flashing_text.Replace("'", "''");
        private_health_fund      = private_health_fund.Replace("'", "''");
        concession_card_number   = concession_card_number.Replace("'", "''");
        login                    = login.Replace("'", "''");
        pwd                      = pwd.Replace("'", "''");
        abn                      = abn.Replace("'", "''");
        next_of_kin_name         = next_of_kin_name.Replace("'", "''");
        next_of_kin_relation     = next_of_kin_relation.Replace("'", "''");
        next_of_kin_contact_info = next_of_kin_contact_info.Replace("'", "''");
        firstname                = firstname.Replace("'", "''");
        middlename               = middlename.Replace("'", "''");
        surname                  = surname.Replace("'", "''");
        nickname                 = nickname.Replace("'", "''");
        gender                   = gender.Replace("'", "''");
        string sql = "INSERT INTO PatientHistory (patient_id,is_clinic_patient,is_gp_patient,is_deleted,is_deceased,flashing_text,flashing_text_added_by,flashing_text_last_modified_date,private_health_fund,concession_card_number,concession_card_expiry_date,is_diabetic,is_member_diabetes_australia,diabetic_assessment_review_date,ac_inv_offering_id,ac_pat_offering_id,login,pwd,is_company,abn,next_of_kin_name,next_of_kin_relation,next_of_kin_contact_info,title_id,firstname,middlename,surname,nickname,gender,dob,modified_from_this_by,date_added) VALUES (" + "" + patient_id + "," + (is_clinic_patient ? "1," : "0,") + (is_gp_patient ? "1," : "0,") + (is_deleted ? "1," : "0,") + (is_deceased ? "1," : "0,") + "'" + flashing_text + "'," + (flashing_text_added_by == -1 ? "NULL" : flashing_text_added_by.ToString()) + "," + (flashing_text_last_modified_date == DateTime.MinValue ? "NULL" : "'" + flashing_text_last_modified_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",'" + private_health_fund + "','" + concession_card_number + "'," + (concession_card_expiry_date == DateTime.MinValue ? "NULL" : "'" + concession_card_expiry_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (is_diabetic ? "1" : "0") + "," + (is_member_diabetes_australia ? "1" : "0") + "," + (diabetic_assessment_review_date == DateTime.MinValue ? "NULL" : "'" + diabetic_assessment_review_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (ac_inv_offering_id == -1 ? "NULL" : ac_inv_offering_id.ToString()) + "," + (ac_pat_offering_id == -1 ? "NULL" : ac_pat_offering_id.ToString()) + ",'" + login + "','" + pwd + "'" + "," + (is_company ? "1" : "0") + "," + "'" + abn + "'" + "," + "'" + next_of_kin_name + "'" + "," + "'" + next_of_kin_relation + "'" + "," + "'" + next_of_kin_contact_info + "'" + "," + title_id + "," + "'" + firstname + "'," + "'" + middlename + "'," + "'" + surname + "'," + "'" + nickname + "'," + "'" + gender + "'," + (dob == DateTime.MinValue ? "NULL" : "'" + dob.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + "" + modified_from_this_by + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static void Update(int patient_history_id, int patient_id, bool is_clinic_patient, bool is_gp_patient, bool is_deleted, bool is_deceased, string flashing_text, int flashing_text_added_by, DateTime flashing_text_last_modified_date, string private_health_fund, string concession_card_number, DateTime concession_card_expiry_date, bool is_diabetic, bool is_member_diabetes_australia, DateTime diabetic_assessment_review_date, int ac_inv_offering_id, int ac_pat_offering_id, string login, string pwd, bool is_company, string abn, string next_of_kin_name, string next_of_kin_relation, string next_of_kin_contact_info, int title_id, string firstname, string middlename, string surname, string nickname, string gender, DateTime dob)
    {
        flashing_text            = flashing_text.Replace("'", "''");
        private_health_fund      = private_health_fund.Replace("'", "''");
        concession_card_number   = concession_card_number.Replace("'", "''");
        login                    = login.Replace("'", "''");
        pwd                      = pwd.Replace("'", "''");
        abn                      = abn.Replace("'", "''");
        next_of_kin_name         = next_of_kin_name.Replace("'", "''");
        next_of_kin_relation     = next_of_kin_relation.Replace("'", "''");
        next_of_kin_contact_info = next_of_kin_contact_info.Replace("'", "''");
        firstname                = firstname.Replace("'", "''");
        middlename               = middlename.Replace("'", "''");
        surname                  = surname.Replace("'", "''");
        nickname                 = nickname.Replace("'", "''");
        gender                   = gender.Replace("'", "''");
        string sql = "UPDATE PatientHistory SET patient_id = " + patient_id + ",is_clinic_patient = " + (is_clinic_patient ? "1," : "0,") + "is_gp_patient = " + (is_gp_patient ? "1," : "0,") + "is_deleted = " + (is_deleted ? "1," : "0,") + "is_deceased = " + (is_deceased ? "1," : "0,") + "flashing_text = '" + flashing_text + "',flashing_text_added_by = " + (flashing_text_added_by == -1 ? "NULL" : flashing_text_added_by.ToString()) + "',flashing_text_last_modified_date = " + (flashing_text_last_modified_date == DateTime.MinValue ? "NULL" : "'" + flashing_text_last_modified_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",private_health_fund = '" + private_health_fund + "',concession_card_number = '" + concession_card_number + "',concession_card_expiry_date = " + (concession_card_expiry_date == DateTime.MinValue ? "NULL" : "'" + concession_card_expiry_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",is_diabetic = " + (is_diabetic ? "1" : "0") + ",is_member_diabetes_australia = " + (is_member_diabetes_australia ? "1" : "0") + ", diabetic_assessment_review_date =" + (diabetic_assessment_review_date == DateTime.MinValue ? "NULL" : "'" + diabetic_assessment_review_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",ac_inv_offering_id = " + (ac_inv_offering_id == -1 ? "NULL" : ac_inv_offering_id.ToString()) + ",ac_pat_offering_id = " + (ac_pat_offering_id == -1 ? "NULL" : ac_pat_offering_id.ToString()) + ",login = '" + login + "',pwd = '" + pwd + "'" + ",is_company = " + (is_company ? "1" : "0") + ", abn = '" + abn + "',next_of_kin_name = '" + next_of_kin_name + "',next_of_kin_relation = '" + next_of_kin_relation + "',next_of_kin_contact_info = '" + next_of_kin_contact_info + "',title_id = " + title_id + ",firstname = '" + firstname + "',middlename = '" + middlename + "',surname = '" + surname + "',nickname = '" + nickname + "',gender = '" + gender + "',dob = " + (dob == DateTime.MinValue ? "NULL" : "'" + dob.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE patient_history_id = " + patient_history_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static bool Exists(int patient_id)
    {
        string sql = "SELECT COUNT(*) FROM PatientHistory WHERE patient_id = " + patient_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static DataTable GetDataTable_ByPatientID(int patient_id)
    {
        string sql = JoinedSql + " WHERE patient_id = " + patient_id;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static PatientHistory[] GetByPatientID(int patient_id)
    {
        DataTable tbl = GetDataTable_ByPatientID(patient_id);
        PatientHistory[] list = new PatientHistory[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }

    public static PatientHistory GetByID(int patient_history_id)
    {
        string sql = JoinedSql + " WHERE patient_history_id = " + patient_history_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    protected static string
        JoinedSql = @"
            SELECT 
                    PatientHistory.patient_history_id,PatientHistory.patient_id,PatientHistory.is_clinic_patient,PatientHistory.is_gp_patient,PatientHistory.is_deleted,PatientHistory.is_deceased,
                    PatientHistory.flashing_text, PatientHistory.flashing_text_added_by, PatientHistory.flashing_text_last_modified_date,
                    PatientHistory.private_health_fund, PatientHistory.concession_card_number, PatientHistory.concession_card_expiry_date, PatientHistory.is_diabetic, PatientHistory.is_member_diabetes_australia, PatientHistory.diabetic_assessment_review_date, PatientHistory.ac_inv_offering_id, PatientHistory.ac_pat_offering_id, PatientHistory.login, PatientHistory.pwd, PatientHistory.is_company, PatientHistory.abn, 
                    PatientHistory.next_of_kin_name, PatientHistory.next_of_kin_relation, PatientHistory.next_of_kin_contact_info,
                    PatientHistory.title_id,PatientHistory.firstname,PatientHistory.middlename,
                    PatientHistory.surname,PatientHistory.nickname,PatientHistory.gender,PatientHistory.dob,PatientHistory.modified_from_this_by,PatientHistory.date_added,
                    Title.descr,

                    staff.staff_id as staff_staff_id,staff.person_id as staff_person_id,staff.login as staff_login,staff.pwd as staff_pwd,staff.staff_position_id as staff_staff_position_id,
                    staff.field_id as staff_field_id,staff.is_contractor as staff_is_contractor,staff.tfn as staff_tfn,
                    staff.is_fired as staff_is_fired,staff.costcentre_id as staff_costcentre_id,
                    staff.is_admin as staff_is_admin,staff.provider_number as staff_provider_number,staff.is_commission as staff_is_commission,staff.commission_percent as staff_commission_percent,
                    staff.is_stakeholder as staff_is_stakeholder,staff.is_master_admin as staff_is_master_admin,staff.is_admin as staff_is_admin,staff.is_principal as staff_is_principal,staff.is_provider as staff_is_provider, staff.is_external as staff_is_external,
                    staff.staff_date_added as staff_staff_date_added,staff.start_date as staff_start_date,staff.end_date as staff_end_date,staff.comment as staff_comment, 
                    staff.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen,
                    staff.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                    staff.bk_screen_field_id as staff_bk_screen_field_id,
                    staff.bk_screen_show_key as staff_bk_screen_show_key,
                    staff.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                    staff.enable_daily_reminder_email as staff_enable_daily_reminder_email,

                    " + PersonDB.GetFields("staff_person_", "staff_person") + @",
                    title_staff.title_id as title_staff_title_id, title_staff.descr as title_staff_descr

            FROM 
                    PatientHistory

                    INNER      JOIN Staff    staff         ON PatientHistory.modified_from_this_by  = staff.staff_id
                    LEFT OUTER JOIN Person   staff_person  ON staff_person.person_id                = staff.person_id
                    LEFT OUTER JOIN Title    title_staff   ON title_staff.title_id                  = staff_person.title_id

                    INNER JOIN Title ON Title.title_id = PatientHistory.title_id ";


    public static PatientHistory LoadAll(DataRow row)
    {
        PatientHistory p = Load(row);
        p.Title = IDandDescrDB.Load(row, "title_id", "descr");

        p.ModifiedFromThisBy = StaffDB.Load(row, "staff_person_");
        p.ModifiedFromThisBy.Person = PersonDB.Load(row, "staff_person_");
        p.ModifiedFromThisBy.Person.Title = IDandDescrDB.Load(row, "title_staff_title_id", "title_staff_descr");
        
        return p;
    }

    public static PatientHistory Load(DataRow row)
    {
        return new PatientHistory(
            Convert.ToInt32(row["patient_history_id"]),
            Convert.ToInt32(row["patient_id"]),
            Convert.ToBoolean(row["is_clinic_patient"]),
            Convert.ToBoolean(row["is_gp_patient"]),
            Convert.ToBoolean(row["is_deleted"]),
            Convert.ToBoolean(row["is_deceased"]),
            Convert.ToString(row["flashing_text"]),
            row["flashing_text_added_by"] == DBNull.Value ? -1 : Convert.ToInt32(row["flashing_text_added_by"]),
            row["flashing_text_last_modified_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["flashing_text_last_modified_date"]),
            Convert.ToString(row["private_health_fund"]),
            Convert.ToString(row["concession_card_number"]),
            row["concession_card_expiry_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["concession_card_expiry_date"]),
            Convert.ToBoolean(row["is_diabetic"]),
            Convert.ToBoolean(row["is_member_diabetes_australia"]),
            row["diabetic_assessment_review_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["diabetic_assessment_review_date"]),
            row["ac_inv_offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["ac_inv_offering_id"]),
            row["ac_pat_offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(row["ac_pat_offering_id"]),
            Convert.ToString(row["login"]),
            Convert.ToString(row["pwd"]),
            Convert.ToBoolean(row["is_company"]),
            Convert.ToString(row["abn"]),
            Convert.ToString(row["next_of_kin_name"]),
            Convert.ToString(row["next_of_kin_relation"]),
            Convert.ToString(row["next_of_kin_contact_info"]),
            Convert.ToInt32(row["title_id"]),
            Convert.ToString(row["firstname"]),
            Convert.ToString(row["middlename"]),
            Convert.ToString(row["surname"]),
            Convert.ToString(row["nickname"]),
            Convert.ToString(row["gender"]),
            row["dob"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["dob"]),
            Convert.ToInt32(row["modified_from_this_by"]),
            Convert.ToDateTime(row["date_added"])
        );
    }

}