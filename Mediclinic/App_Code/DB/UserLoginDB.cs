using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class UserLoginDB
{

    public static void Delete(int userlogin_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM UserLogin WHERE userlogin_id = " + userlogin_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void DeleteByStaffID(int staff_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM UserLogin WHERE staff_id = " + staff_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void DeleteByPatientID(int patient_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM UserLogin WHERE patient_id = " + patient_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int staff_id, int patient_id, string username, int site_id, bool is_successful, string session_id, string ipaddress)
    {
        username = username.Replace("'", "''");
        session_id = session_id.Replace("'", "''");
        ipaddress = ipaddress.Replace("'", "''");
        string sql = "INSERT INTO UserLogin (staff_id,patient_id,username,site_id,is_successful,session_id,ipaddress) VALUES (" + "" + (staff_id == -1 ? "NULL" : staff_id.ToString()) + "," + (patient_id == -1 ? "NULL" : patient_id.ToString()) + "," + "'" + username + "'," + (site_id < 0 ? "NULL" : site_id.ToString()) + "," + (is_successful ? "1," : "0,") + "'" + session_id + "'," + "'" + ipaddress + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int userlogin_id, int staff_id, int patient_id, string username, int site_id, bool is_successful, string session_id, DateTime last_access_time, string last_access_page, string ipaddress)
    {
        username = username.Replace("'", "''");
        session_id = session_id.Replace("'", "''");
        last_access_page = last_access_page.Replace("'", "''");
        ipaddress = ipaddress.Replace("'", "''");
        string sql = "UPDATE UserLogin SET staff_id = " + (staff_id == -1 ? "NULL" : staff_id.ToString()) + ",patient_id = " + (patient_id == -1 ? "NULL" : patient_id.ToString()) + ",username = '" + username + "'" + "site_id = " + (site_id < 0 ? "NULL" : site_id.ToString()) + ",is_successful = " + (is_successful ? "1," : "0,") + "session_id = '" + session_id + "',last_access_time = '" + last_access_time.ToString("yyyy-MM-dd HH:mm:ss") + "'" + (last_access_page.Length == 0 ? "" : ",last_access_page = '" + last_access_page + "'") + ",ipaddress = '" + ipaddress + " WHERE userlogin_id = " + userlogin_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateLastAccessTime(int userlogin_id, DateTime last_access_time, string last_access_page)
    {
        last_access_page = last_access_page.Replace("'", "''");
        string sqlUpdateOld = "UPDATE UserLogin SET is_logged_off = 1 WHERE last_access_time < '" + DateTime.Now.Subtract(new TimeSpan(2, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss") + "';";
        string sql = "UPDATE UserLogin SET last_access_time = '" + last_access_time.ToString("yyyy-MM-dd HH:mm:ss") + "'" + (last_access_page.Length == 0 ? "" : ",last_access_page = '" + last_access_page + "'") + " WHERE userlogin_id = " + userlogin_id.ToString() + ";" + sqlUpdateOld;
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetOtherSessionsLoggedOut(int staff_id, int patient_id, string session_id)
    {
        string sql = staff_id != -1 ?
            "UPDATE UserLogin SET is_logged_off = 1 WHERE staff_id   = " + staff_id.ToString()   + " AND session_id <> '" + session_id.ToString() + "'" :
            "UPDATE UserLogin SET is_logged_off = 1 WHERE patient_id = " + patient_id.ToString() + " AND session_id <> '" + session_id.ToString() + "'";
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetOtherSessionsOfThisUserLoggedOut(int userlogin_id, int staff_id, int patient_id)
    {
        string sql = staff_id != -1 ?
            "UPDATE UserLogin SET is_logged_off = 1 WHERE userlogin_id <> " + userlogin_id.ToString() + " AND staff_id = " + staff_id.ToString() + "":
            "UPDATE UserLogin SET is_logged_off = 1 WHERE userlogin_id <> " + userlogin_id.ToString() + " AND patient_id = " + patient_id.ToString() + "";
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetAllSessionsLoggedOut(int staff_id, int patient_id)
    {
        string sql = staff_id != -1 ?
            "UPDATE UserLogin SET is_logged_off = 1 WHERE staff_id   = " + staff_id.ToString() :
            "UPDATE UserLogin SET is_logged_off = 1 WHERE patient_id = " + patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateLoggedOffByUserLoginID(int userlogin_id)
    {
        string sql = "UPDATE UserLogin SET is_logged_off = 1 WHERE userlogin_id = " + userlogin_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateLoggedOffByStaffID(int staff_id)
    {
        string sql = "UPDATE UserLogin SET is_logged_off = 1 WHERE staff_id = " + staff_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateSite(int staff_id, int patient_id, int site_id)
    {

        string sql = staff_id != -1 ?
            @"
            UPDATE userlogin
            SET site_id = " + (site_id < 0 ? "NULL" : site_id.ToString()) + @"
            WHERE userlogin_id IN 
            (
               SELECT TOP 1 userlogin_id
               FROM userlogin
               WHERE staff_id = " + staff_id + @" AND is_logged_off = 0
               ORDER BY userlogin_id DESC
            )"

            :

            @"
            UPDATE userlogin
            SET site_id = " + (site_id < 0 ? "NULL" : site_id.ToString()) + @"
            WHERE userlogin_id IN 
            (
               SELECT TOP 1 userlogin_id
               FROM userlogin
               WHERE patient_id = " + patient_id + @" AND is_logged_off = 0
               ORDER BY userlogin_id DESC
            )";
                
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable(int nDaysToRetrieve)
    {
        string sql = JoinedSql(false) + " WHERE (userlogin.staff_id IS NULL OR userlogin.staff_id > 0) AND userlogin.last_access_time > '" + DateTime.Now.Subtract(new TimeSpan(nDaysToRetrieve, 0, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY userlogin.last_access_time DESC";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static DataTable GetDataTable(bool incSupportStaff, DateTime startDate, DateTime endDate, bool incStaff, bool incPatients, int staff_id = -1, int patient_id = -1)
    {
        string sql = JoinedSql(false) + " WHERE " + (incSupportStaff ? "" : " (userlogin.staff_id IS NULL OR userlogin.staff_id > 0) AND ")
             + (incStaff  ? "" : " userlogin.staff_id IS NULL AND ")
             + (incPatients ? "" : " userlogin.patient_id IS NULL AND ")
             + (staff_id == -1 ? "" : " userlogin.staff_id = " + staff_id + " AND ") 
             + (patient_id == -1 ? "" : " userlogin.patient_id = " + patient_id + " AND ") 
             + " userlogin.login_time >= '" + startDate.ToString("yyyy-MM-dd HH:mm:ss") 
             + "' AND userlogin.last_access_time <= '" + endDate.ToString("yyyy-MM-dd HH:mm:ss") 
             + "' ORDER BY userlogin.last_access_time DESC";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static UserLogin GetByID(int userlogin_id)
    {
        string sql = JoinedSql(true) + " WHERE userlogin.userlogin_id = " + userlogin_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static UserLogin GetByUserID(int staff_id, int patient_id)
    {
        string sql = staff_id != -1 ?
            JoinedSql(true) + " WHERE userlogin.staff_id   = " + staff_id.ToString()   + " AND userlogin.is_successful = 1 AND userlogin.is_logged_off = 0 AND userlogin.last_access_time >'" + DateTime.Now.Subtract(new TimeSpan(1, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY userlogin.last_access_time DESC" :
            JoinedSql(true) + " WHERE userlogin.patient_id = " + patient_id.ToString() + " AND userlogin.is_successful = 1 AND userlogin.is_logged_off = 0 AND userlogin.last_access_time >'" + DateTime.Now.Subtract(new TimeSpan(1, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY userlogin.last_access_time DESC";
        
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }
    public static UserLogin GetCurLoggedIn(int staff_id, int patient_id, string session_id, int site_id = -1)
    {
        string sql = staff_id != -1 ?
            JoinedSql(true) + " WHERE userlogin.staff_id   = " + staff_id.ToString()   + (site_id < 0 ? "" : " AND userlogin.site_id = " + site_id + " ") + " AND userlogin.session_id = '" + session_id + "' AND userlogin.is_successful = 1 AND userlogin.is_logged_off = 0 AND userlogin.last_access_time >'" + DateTime.Now.Subtract(new TimeSpan(1, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY userlogin.last_access_time DESC" :
            JoinedSql(true) + " WHERE userlogin.patient_id = " + patient_id.ToString() + (site_id < 0 ? "" : " AND userlogin.site_id = " + site_id + " ") + " AND userlogin.session_id = '" + session_id + "' AND userlogin.is_successful = 1 AND userlogin.is_logged_off = 0 AND userlogin.last_access_time >'" + DateTime.Now.Subtract(new TimeSpan(1, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY userlogin.last_access_time DESC";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static int GetCount(int numDays = -1)
    {
        string sql = "SELECT COUNT (*) FROM UserLogin WHERE staff_id > 0 " + (numDays > 0 ? "AND last_access_time >= DATEADD(day,-" + numDays + ", GETDATE());" : "");
        return (int)DBBase.ExecuteSingleResult(sql);
    }
    public static int GetStaffCount(int numDays = -1)
    {
        string sql = "SELECT COUNT (DISTINCT staff_id) FROM UserLogin WHERE staff_id > 0 " + (numDays > 0 ? "AND last_access_time >= DATEADD(day,-" + numDays + ", GETDATE());" : "");
        return (int)DBBase.ExecuteSingleResult(sql);
    }

    protected static string JoinedSql(bool onlyTopRow = false)
    {
        return @"SELECT 
                        " + (onlyTopRow ? " top 1 " : "") + @" userlogin.userlogin_id as userlogin_userlogin_id,userlogin.staff_id as userlogin_staff_id,userlogin.patient_id as userlogin_patient_id,userlogin.username as userlogin_username,
                        userlogin.site_id as userlogin_site_id,userlogin.is_successful as userlogin_is_successful,userlogin.is_logged_off as userlogin_is_logged_off,
                        userlogin.session_id as userlogin_session_id,userlogin.login_time as userlogin_login_time,userlogin.last_access_time as userlogin_last_access_time,userlogin.last_access_page as userlogin_last_access_page,
                        userlogin.ipaddress as userlogin_ipaddress,



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

                        " + PersonDB.GetFields("person_", "p") + @",
                        t.title_id as title_title_id, t.descr as title_descr,
                        c.costcentre_id as cost_centre_costcentre_id,c.descr as cost_centre_descr,c.parent_id as cost_centre_parent_id,
                        sr.field_id as field_field_id,sr.descr as field_descr,
                        sp.staff_position_id as staff_position_staff_position_id,sp.descr as staff_position_descr,



                        patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, patient.is_clinic_patient as patient_is_clinic_patient, patient.is_gp_patient as patient_is_gp_patient, patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                        patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                        patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn, 
                        patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info,

                        " + PersonDB.GetFields("pperson_", "pp") + @",
                        tt.title_id as ttitle_title_id, tt.descr as ttitle_descr,



                        site.site_id as site_site_id,site.entity_id as site_entity_id,site.name as site_name,site.site_type_id as site_site_type_id,site.abn as site_abn,site.acn as site_acn,site.tfn as site_tfn,
                        site.asic as site_asic,site.is_provider as site_is_provider,site.bank_bpay as site_bank_bpay,site.bank_bsb as site_bank_bsb,site.bank_account as site_bank_account,
                        site.bank_direct_debit_userid as site_bank_direct_debit_userid,site.bank_username as site_bank_username,site.oustanding_balance_warning as site_oustanding_balance_warning,
                        site.print_epc as site_print_epc,site.excl_sun as site_excl_sun,site.excl_mon as site_excl_mon,site.excl_tue as site_excl_tue,site.excl_wed as site_excl_wed,site.excl_thu as site_excl_thu,
                        site.excl_fri as site_excl_fri,site.excl_sat as site_excl_sat,site.day_start_time as site_day_start_time,site.lunch_start_time as site_lunch_start_time,
                        site.lunch_end_time as site_lunch_end_time,site.day_end_time as site_day_end_time,site.fiscal_yr_end as site_fiscal_yr_end,site.num_booking_months_to_get as site_num_booking_months_to_get,

                        sitetype.descr as site_type_descr 

                 FROM 
                        UserLogin userlogin
                        LEFT OUTER JOIN Site          site     ON site.site_id         = userlogin.site_id 
                        LEFT OUTER JOIN SiteType      sitetype ON site.site_type_id    = sitetype.site_type_id


                        LEFT OUTER JOIN Staff         staff   ON staff.staff_id       = userlogin.staff_id 
                        LEFT OUTER JOIN Person        p       ON staff.person_id      = p.person_id
                        LEFT OUTER JOIN Title         t       ON t.title_id           = p.title_id

                        LEFT OUTER JOIN Patient       patient ON patient.patient_id   = userlogin.patient_id 
                        LEFT OUTER JOIN Person        pp      ON patient.person_id    = pp.person_id
                        LEFT OUTER JOIN Title         tt      ON tt.title_id          = pp.title_id

                        LEFT OUTER JOIN Field         sr      ON staff.field_id       = sr.field_id
                        LEFT OUTER JOIN StaffPosition sp      ON sp.staff_position_id = staff.staff_position_id
                        LEFT OUTER JOIN CostCentre AS c       ON c.costcentre_id      = staff.costcentre_id


";
    }



    public static UserLogin LoadAll(DataRow row)
    {
        UserLogin userlogin = Load(row, "userlogin_");
        if (row["userlogin_site_id"] != DBNull.Value)
            userlogin.Site = SiteDB.Load(row, "site_");

        if (row["userlogin_staff_id"] != DBNull.Value)
            userlogin.Staff = StaffDB.LoadAll(row);
        if (row["userlogin_patient_id"] != DBNull.Value)
        {
            userlogin.Patient = PatientDB.Load(row, "patient_");
            userlogin.Patient.Person = PersonDB.Load(row, "pperson_");
            userlogin.Patient.Person.Title = IDandDescrDB.Load(row, "ttitle_title_id", "ttitle_descr");
        }

        return userlogin;
    }

    public static UserLogin Load(DataRow row, string prefix="")
    {
        return new UserLogin(
            Convert.ToInt32(row[prefix + "userlogin_id"]),
            row[prefix + "staff_id"]   == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "staff_id"]),
            row[prefix + "patient_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "patient_id"]),
            Convert.ToString(row[prefix + "username"]),
            row[prefix + "site_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "site_id"]),
            Convert.ToBoolean(row[prefix + "is_successful"]),
            Convert.ToBoolean(row[prefix + "is_logged_off"]),
            Convert.ToString(row[prefix + "session_id"]),
            Convert.ToDateTime(row[prefix + "login_time"]),
            Convert.ToDateTime(row[prefix + "last_access_time"]),
            Convert.ToString(row[prefix + "last_access_page"]),
            Convert.ToString(row[prefix + "ipaddress"])
        );
    }

}