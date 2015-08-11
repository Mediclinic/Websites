using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class PatientDB
{

    public static void Delete(int patient_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Patient WHERE patient_id = " + patient_id.ToString() + "; DBCC CHECKIDENT(Patient,RESEED,1); DBCC CHECKIDENT(Patient);");
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int person_id, bool is_clinic_patient, bool is_gp_patient, bool is_deceased, string flashing_text, int flashing_text_added_by, DateTime flashing_text_last_modified_date, string private_health_fund, string concession_card_number, DateTime concession_card_expiry_date, bool is_diabetic, bool is_member_diabetes_australia, DateTime diabetic_assessment_review_date, int ac_inv_offering_id, int ac_pat_offering_id, string login, string pwd, bool is_company, string abn, string next_of_kin_name, string next_of_kin_relation, string next_of_kin_contact_info)
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
        string sql = "INSERT INTO Patient (person_id,is_clinic_patient,is_gp_patient,is_deceased,flashing_text,flashing_text_added_by,flashing_text_last_modified_date,private_health_fund,concession_card_number,concession_card_expiry_date,is_diabetic,is_member_diabetes_australia,diabetic_assessment_review_date,ac_inv_offering_id,ac_pat_offering_id,login,pwd,is_company,abn,next_of_kin_name,next_of_kin_relation,next_of_kin_contact_info) VALUES (" + "" + person_id + "," + (is_clinic_patient ? "1," : "0,") + (is_gp_patient ? "1," : "0,") + (is_deceased ? "1," : "0,") + "'" + flashing_text + "'," + (flashing_text_added_by == -1 ? "NULL" : flashing_text_added_by.ToString()) + "," + (flashing_text_last_modified_date == DateTime.MinValue ? "NULL" : "'" + flashing_text_last_modified_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",'" + private_health_fund + "','" + concession_card_number + "'," + (concession_card_expiry_date == DateTime.MinValue ? "NULL" : "'" + concession_card_expiry_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (is_diabetic ? "1" : "0") + "," + (is_member_diabetes_australia ? "1" : "0") + "," + (diabetic_assessment_review_date == DateTime.MinValue ? "NULL" : "'" + diabetic_assessment_review_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (ac_inv_offering_id == -1 ? "NULL" : ac_inv_offering_id.ToString()) + "," + (ac_pat_offering_id == -1 ? "NULL" : ac_pat_offering_id.ToString()) + ",'" + login + "','" + pwd + "'" + "," + (is_company ? "1" : "0") + "," + "'" + abn + "'" + "," + "'" + next_of_kin_name + "'" + "," + "'" + next_of_kin_relation + "'" + "," + "'" + next_of_kin_contact_info + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static int InsertWithID(int patient_id, int person_id, bool is_clinic_patient, bool is_gp_patient, bool is_deceased, string flashing_text, int flashing_text_added_by, DateTime flashing_text_last_modified_date, string private_health_fund, string concession_card_number, DateTime concession_card_expiry_date, bool is_diabetic, bool is_member_diabetes_australia, DateTime diabetic_assessment_review_date, int ac_inv_offering_id, int ac_pat_offering_id, string login, string pwd, bool is_company, string abn, string next_of_kin_name, string next_of_kin_relation, string next_of_kin_contact_info)
    {
        if (PatientDB.Exists(patient_id))
            throw new CustomMessageException("ID '" + patient_id + "' already exists in the system.");

        flashing_text            = flashing_text.Replace("'", "''");
        private_health_fund      = private_health_fund.Replace("'", "''");
        concession_card_number   = concession_card_number.Replace("'", "''");
        login                    = login.Replace("'", "''");
        pwd                      = pwd.Replace("'", "''");
        abn                      = abn.Replace("'", "''");
        next_of_kin_name         = next_of_kin_name.Replace("'", "''");
        next_of_kin_relation     = next_of_kin_relation.Replace("'", "''");
        next_of_kin_contact_info = next_of_kin_contact_info.Replace("'", "''");
        string sql = "SET IDENTITY_INSERT Patient ON; INSERT INTO Patient (patient_id, person_id,is_clinic_patient,is_gp_patient,is_deceased,flashing_text,flashing_text_added_by,flashing_text_last_modified_date,private_health_fund,concession_card_number,concession_card_expiry_date,is_diabetic,is_member_diabetes_australia,diabetic_assessment_review_date,ac_inv_offering_id,ac_pat_offering_id,login,pwd,is_company,abn,next_of_kin_name,next_of_kin_relation,next_of_kin_contact_info) VALUES (" + patient_id + "," + person_id + "," + (is_clinic_patient ? "1," : "0,") + (is_gp_patient ? "1," : "0,") + (is_deceased ? "1," : "0,") + "'" + flashing_text + "'," + (flashing_text_added_by == -1 ? "NULL" : flashing_text_added_by.ToString()) + "," + (flashing_text_last_modified_date == DateTime.MinValue ? "NULL" : "'" + flashing_text_last_modified_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",'" + private_health_fund + "','" + concession_card_number + "'," + (concession_card_expiry_date == DateTime.MinValue ? "NULL" : "'" + concession_card_expiry_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (is_diabetic ? "1" : "0") + "," + (is_member_diabetes_australia ? "1" : "0") + "," + (diabetic_assessment_review_date == DateTime.MinValue ? "NULL" : "'" + diabetic_assessment_review_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (ac_inv_offering_id == -1 ? "NULL" : ac_inv_offering_id.ToString()) + "," + (ac_pat_offering_id == -1 ? "NULL" : ac_pat_offering_id.ToString()) + ",'" + login + "','" + pwd + "'" + "," + (is_company ? "1" : "0") + "," + "'" + abn + "'" + "," + "'" + next_of_kin_name + "'" + "," + "'" + next_of_kin_relation + "'" + "," + "'" + next_of_kin_contact_info + "'" + ");SET IDENTITY_INSERT Patient OFF;SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int patient_id, int person_id, bool is_clinic_patient, bool is_gp_patient, bool is_deceased, string flashing_text, int flashing_text_added_by, DateTime flashing_text_last_modified_date, string private_health_fund, string concession_card_number, DateTime concession_card_expiry_date, bool is_diabetic, bool is_member_diabetes_australia, DateTime diabetic_assessment_review_date, int ac_inv_offering_id, int ac_pat_offering_id, string login, string pwd, bool is_company, string abn, string next_of_kin_name, string next_of_kin_relation, string next_of_kin_contact_info)
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
        string sql = "UPDATE Patient SET person_id = " + person_id + ",is_clinic_patient = " + (is_clinic_patient ? "1," : "0,") + "is_gp_patient = " + (is_gp_patient ? "1," : "0,") + "is_deceased = " + (is_deceased ? "1," : "0,") + "flashing_text = '" + flashing_text + "',flashing_text_added_by = " + (flashing_text_added_by == -1 ? "NULL" : flashing_text_added_by.ToString()) + ",flashing_text_last_modified_date = " + (flashing_text_last_modified_date == DateTime.MinValue ? "NULL" : "'" + flashing_text_last_modified_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",private_health_fund = '" + private_health_fund + "',concession_card_number = '" + concession_card_number + "',concession_card_expiry_date = " + (concession_card_expiry_date == DateTime.MinValue ? "NULL" : "'" + concession_card_expiry_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",is_diabetic = " + (is_diabetic ? "1" : "0") + ",is_member_diabetes_australia = " + (is_member_diabetes_australia ? "1" : "0") + ",diabetic_assessment_review_date = " + (diabetic_assessment_review_date == DateTime.MinValue ? "NULL" : "'" + diabetic_assessment_review_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",ac_inv_offering_id = " + (ac_inv_offering_id == -1 ? "NULL" : ac_inv_offering_id.ToString()) + ",ac_pat_offering_id = " + (ac_pat_offering_id == -1 ? "NULL" : ac_pat_offering_id.ToString()) + ",login = '" + login + "',pwd = '" + pwd + "'" + ",is_company = " + (is_company ? "1" : "0") + ", abn = '" + abn + "',next_of_kin_name = '" + next_of_kin_name + "',next_of_kin_relation = '" + next_of_kin_relation + "',next_of_kin_contact_info = '" + next_of_kin_contact_info + "'" + " WHERE patient_id = " + patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateLoginPwd(int patient_id, string login, string pwd)
    {
        login = login.Replace("'", "''");
        pwd = pwd.Replace("'", "''");
        string sql = "UPDATE Patient SET login = '" + login + "', pwd = '" + pwd + "' WHERE patient_id = " + patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdatePwd(int patient_id, string pwd)
    {
        pwd = pwd.Replace("'", "''");
        string sql = "UPDATE Patient SET pwd = '" + pwd + "' WHERE patient_id = " + patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int patient_id, int staff_id)
    {
        Patient patient = PatientDB.GetByID(patient_id);
        PatientHistoryDB.Insert(patient.PatientID, patient.IsClinicPatient, patient.IsGPPatient, patient.IsDeleted, patient.IsDeceased,
                        patient.FlashingText, patient.FlashingTextAddedBy == null ? -1 : patient.FlashingTextAddedBy.StaffID, patient.FlashingTextLastModifiedDate,
                        patient.PrivateHealthFund, patient.ConcessionCardNumber, patient.ConcessionCardExpiryDate, patient.IsDiabetic, patient.IsMemberDiabetesAustralia, patient.DiabeticAAassessmentReviewDate, patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID, patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID, patient.Login, patient.Pwd, patient.IsCompany, patient.ABN, patient.NextOfKinName, patient.NextOfKinRelation, patient.NextOfKinContactInfo, 
                        patient.Person.Title.ID, patient.Person.Firstname, patient.Person.Middlename, patient.Person.Surname, patient.Person.Nickname, patient.Person.Gender, patient.Person.Dob, staff_id);

        string sql = "UPDATE Patient SET is_deleted = 1 WHERE patient_id = " + patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int patient_id, int staff_id)
    {
        Patient patient = PatientDB.GetByID(patient_id);
        PatientHistoryDB.Insert(patient.PatientID, patient.IsClinicPatient, patient.IsGPPatient, patient.IsDeleted, patient.IsDeceased,
                        patient.FlashingText, patient.FlashingTextAddedBy == null ? -1 : patient.FlashingTextAddedBy.StaffID, patient.FlashingTextLastModifiedDate,
                        patient.PrivateHealthFund, patient.ConcessionCardNumber, patient.ConcessionCardExpiryDate, patient.IsDiabetic, patient.IsMemberDiabetesAustralia, patient.DiabeticAAassessmentReviewDate, patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID, patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID, patient.Login, patient.Pwd, patient.IsCompany, patient.ABN, patient.NextOfKinName, patient.NextOfKinRelation, patient.NextOfKinContactInfo, 
                        patient.Person.Title.ID, patient.Person.Firstname, patient.Person.Middlename, patient.Person.Surname, patient.Person.Nickname, patient.Person.Gender, patient.Person.Dob, staff_id);

        string sql = "UPDATE Patient SET is_deleted = 0 WHERE patient_id = " + patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateDeceased(int patient_id, int staff_id)
    {
        Patient patient = PatientDB.GetByID(patient_id);
        PatientHistoryDB.Insert(patient.PatientID, patient.IsClinicPatient, patient.IsGPPatient, patient.IsDeleted, patient.IsDeceased,
                        patient.FlashingText, patient.FlashingTextAddedBy == null ? -1 : patient.FlashingTextAddedBy.StaffID, patient.FlashingTextLastModifiedDate,
                        patient.PrivateHealthFund, patient.ConcessionCardNumber, patient.ConcessionCardExpiryDate, patient.IsDiabetic, patient.IsMemberDiabetesAustralia, patient.DiabeticAAassessmentReviewDate, patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID, patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID, patient.Login, patient.Pwd, patient.IsCompany, patient.ABN, patient.NextOfKinName, patient.NextOfKinRelation, patient.NextOfKinContactInfo, 
                        patient.Person.Title.ID, patient.Person.Firstname, patient.Person.Middlename, patient.Person.Surname, patient.Person.Nickname, patient.Person.Gender, patient.Person.Dob, staff_id);

        string sql = "UPDATE Patient SET is_deceased = 1 WHERE patient_id = " + patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateFlashingText(int patient_id, string flashing_text, int flashing_text_added_by, DateTime flashing_text_last_modified_date)
    {
        flashing_text = flashing_text.Replace("'", "''");
        string sql = "UPDATE Patient SET flashing_text = '" + flashing_text + "',flashing_text_added_by = " + (flashing_text_added_by == -1 ? "NULL" : flashing_text_added_by.ToString()) + ",flashing_text_last_modified_date = " + (flashing_text_last_modified_date == DateTime.MinValue ? "NULL" : "'" + flashing_text_last_modified_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE patient_id = " + patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static bool Exists(int patient_id)
    {
        string sql = "SELECT COUNT(*) FROM Patient WHERE patient_id = " + patient_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }


    public static DataTable GetPatientsAddedByStaff(int staff_id, DateTime start, DateTime end)
    {
        string outerApply = @"
                          OUTER APPLY
                                (
                                SELECT  TOP 1 PatientReferrer.patient_referrer_id, PatientReferrer.register_referrer_id, Referrer.referrer_id, Person.firstname, Person.surname
                                FROM    PatientReferrer
                                               JOIN RegisterReferrer ON RegisterReferrer.register_referrer_id = PatientReferrer.register_referrer_id
                                               JOIN Referrer ON Referrer.referrer_id =  RegisterReferrer.referrer_id
                                               JOIN Person ON Person.person_id = Referrer.person_id

                                WHERE   PatientReferrer.patient_id = pa.patient_id
                                ORDER BY PatientReferrer.patient_referrer_id asc
                                ) referrer_info
";

        string selectItemsForOuterApplyToWork = @"
                          -- these must be selected in the cross apply select fields
                          ,
                          referrer_info.patient_referrer_id as referrer_info_patient_referrer_id, referrer_info.register_referrer_id as referrer_info_register_referrer_id,  
                          referrer_info.referrer_id as referrer_info_referrer_id, referrer_info.firstname as referrer_info_firstname, referrer_info.surname as referrer_info_surname
";

        string sql = JoinedSql
                        .Replace("SELECT", "SELECT DISTINCT")
                        .Replace("FROM", selectItemsForOuterApplyToWork + Environment.NewLine + "FROM")
                        .Replace("WHERE", outerApply + Environment.NewLine + "WHERE")
                        +
                          @" AND p.added_by = (SELECT person_id FROM STAFF WHERE staff_id = " + staff_id + ") " + Environment.NewLine +
                          (start == DateTime.MinValue ? "" : " AND p.person_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end == DateTime.MinValue ? "" : " AND p.person_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @" 
                 ORDER BY  
                          referrer_info.surname, referrer_info.firstname,
                          surname, firstname, middlename ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }


    public static int GetCountAddedByOrg(int organisation_id, DateTime start, DateTime end)
    {
        if (organisation_id == 0)
        {
            return GetCountAddedByOrg(start, end);
        }
        else
        {
            string whereClause = string.Empty;

            if (organisation_id != 0)
                whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " RegisterPatient.organisation_id = " + organisation_id;

            if (start != DateTime.MinValue)
            {
                whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " RegisterPatient.register_patient_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                whereClause += " AND CONVERT(DATE,RegisterPatient.register_patient_date_added) = CONVERT(DATE,Patient.patient_date_added) ";
            }
            if (end != DateTime.MinValue)
            {
                whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " RegisterPatient.register_patient_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                whereClause += " AND CONVERT(DATE,RegisterPatient.register_patient_date_added) = CONVERT(DATE,Patient.patient_date_added) ";
            }

            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " RegisterPatient.is_deleted = 0";

            string sql = "SELECT COUNT(DISTINCT Patient.patient_id) FROM RegisterPatient INNER JOIN Patient ON RegisterPatient.patient_id = Patient.patient_id " + whereClause;
            return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
        }
    }
    public static int GetCountAddedByOrg(DateTime start, DateTime end)
    {
        string whereClause = string.Empty;

        if (start != DateTime.MinValue)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " patient_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        if (end != DateTime.MinValue)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " patient_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'";

        string sql = "SELECT COUNT(patient_id) FROM Patient " + whereClause;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static string JoinedSql = @"
                       SELECT   
                                pa.patient_id, pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased, 
                                pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date,
                                pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                                pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                                " + PersonDB.GetFields("", "p") + @",
                                t.title_id as t_title_id, t.descr as t_descr
                       FROM     
                                Patient AS pa 
                                INNER JOIN Person AS p ON pa.person_id = p.person_id
                                INNER JOIN Title  AS t ON t.title_id = p.title_id
                       WHERE
                                pa.is_deleted = 0 ";

    public static DataTable GetDataTable(bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, bool show_only_is_gp_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchFirstname = "", bool searchFirstnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string searchEmail = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchInternalOrgs = "", string matchProviders = "", string matchReferrers = "", string matchReffererPerson = "", string matchReffererOrg = "", bool onlyDiabetics = false, bool onlyMedicareEPC = false, bool onlyDVAEPC = false)
    {
        matchSurname        = matchSurname.Replace("'", "''");
        matchFirstname      = matchFirstname.Replace("'", "''");
        matchSuburbs        = matchSuburbs.Replace("'", "''");
        matchStreets        = matchStreets.Replace("'", "''");
        searchPhoneNbr      = searchPhoneNbr.Replace("'", "''");
        searchEmail         = searchEmail.Replace("'", "''");
        matchMedicareCardNo = matchMedicareCardNo.Replace("'", "''");
        matchReferrers      = matchReferrers.Replace("'", "''");
        matchReffererPerson = matchReffererPerson.Replace("'", "''");
        matchReffererOrg    = matchReffererOrg.Replace("'", "''");
        matchInternalOrgs   = matchInternalOrgs.Replace("'", "''");
        matchProviders      = matchProviders.Replace("'", "''");
        

        string sql = @"SELECT
                                pa.patient_id, pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased, pa.is_deleted,
                                pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date,
                                pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                                pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                                " + PersonDB.GetFields("", "p") + @", p2.firstname AS added_by_firstname, 
                                t.title_id, t.descr
                                " + (matchSuburbs == "" ? "" : ", (select count(*) from " + Utilities.GetAddressType() + " ad where ad.entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id in (" + matchSuburbs + @")) as address_suburub_count ") + @"
                                " + (Utilities.GetAddressType() != "Contact"    || matchStreets == "" ? "" : ", (select count(*) from Contact    ad where ad.entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.address_channel_id in (" + matchStreets + @")) as address_street_count ") + @"
                                " + (Utilities.GetAddressType() != "ContactAus" || matchStreets == "" ? "" : ", (select count(*) from ContactAus ad where ad.entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND (ad.street_name = '" + matchStreets + "' OR SOUNDEX(ad.street_name) = SOUNDEX('" + matchStreets + "'))) as address_street_count ") + @"

                                ,(SELECT COUNT(*) 
                                  FROM   RegisterPatient INNER JOIN Organisation ON RegisterPatient.organisation_id = Organisation.organisation_id
                                  WHERE  patient_id = pa.patient_id AND RegisterPatient.is_deleted = 0 AND Organisation.is_deleted = 0 " + (UserView.GetInstance().IsAgedCareView ? " AND Organisation.organisation_type_id IN (139,367,372) " : "") + (UserView.GetInstance().IsClinicView ? " AND Organisation.organisation_type_id = 218 " : "") + @"
                                 ) as num_registered_orgs

                       FROM
                                Patient AS pa 
                                INNER JOIN Person p  ON pa.person_id = p.person_id
                                LEFT OUTER JOIN Person p2 ON p2.person_id = p.added_by
                                INNER JOIN Title  t  ON t.title_id = p.title_id
                       WHERE 
                                1 = 1
                                " + (matchSurname.Length > 0 && !searchSurnameOnlyStartsWith ? " AND p.surname LIKE '%" + matchSurname + "%'" : "") + @"
                                " + (matchSurname.Length > 0 &&  searchSurnameOnlyStartsWith ? " AND p.surname LIKE '"  + matchSurname + "%'" : "") + @"
                                " + (matchFirstname.Length > 0 && !searchFirstnameOnlyStartsWith ? " AND p.firstname LIKE '%" + matchFirstname + "%'" : "") + @"
                                " + (matchFirstname.Length > 0 &&  searchFirstnameOnlyStartsWith ? " AND p.firstname LIKE '"  + matchFirstname + "%'" : "") + @"
                                " + (Utilities.GetAddressType().ToString() != "Contact"    || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                                " + (Utilities.GetAddressType().ToString() != "Contact"    || matchStreets   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.address_channel_id IN (" + matchStreets + ")) > 0 ") + @"
                                " + (Utilities.GetAddressType().ToString() != "Contact"    || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_group_id = 2 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                                " + (Utilities.GetAddressType().ToString() != "Contact"    || searchEmail    == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_id       = 27 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND addr_line1 LIKE '" + searchEmail    + "%'" + ") > 0 ") + @"
                                " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                                " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchStreets   == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND (ad.street_name = '" + matchStreets + "' OR SOUNDEX(ad.street_name) = SOUNDEX('" + matchStreets + "'))) > 0 ") + @"
                                " + (Utilities.GetAddressType().ToString() != "ContactAus" || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_group_id = 2 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                                " + (Utilities.GetAddressType().ToString() != "ContactAus" || searchEmail    == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_id       = 27 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND addr_line1 LIKE '" + searchEmail    + "%'" + ") > 0 ") + @"
                                " + (matchMedicareCardNo.Length > 0 && !searchMedicareCardNoOnlyStartsWith ? " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '%" + matchMedicareCardNo + "%') > 0 " : "") + @"
                                " + (matchMedicareCardNo.Length > 0 && searchMedicareCardNoOnlyStartsWith ?  " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '" + matchMedicareCardNo + "%') > 0 " : "") + @"
                                " + (matchDOBDay    == -1 ? "" : " AND datepart(day,p.dob)   = " + matchDOBDay) + @"
                                " + (matchDOBMonth  == -1 ? "" : " AND datepart(month,p.dob) = " + matchDOBMonth) + @"
                                " + (matchDOBYear   == -1 ? "" : " AND datepart(year,p.dob)  = " + matchDOBYear) + @"
                                " + (show_deleted  ? "" : " AND pa.is_deleted  = 0 ") + @"
                                " + (show_deceased ? "" : " AND pa.is_deceased = 0 ") + @"
                                " + (!show_only_is_clinic_patient ? "" : " AND pa.is_clinic_patient = 1 ") + @"
                                " + (!show_only_is_gp_patient     ? "" : " AND pa.is_gp_patient = 1 ") + @"
                                " + (matchInternalOrgs   == "" ? "" : " AND (SELECT COUNT(*) FROM RegisterPatient WHERE RegisterPatient.is_deleted = 0 AND RegisterPatient.patient_id=pa.patient_id AND RegisterPatient.organisation_id IN (" + matchInternalOrgs + ")) > 0 ") + @"
                                " + (matchProviders      == "" ? "" : " AND (SELECT COUNT(*) FROM Booking         WHERE (Booking.date_deleted IS NULL AND Booking.deleted_by IS NULL) AND Booking.patient_id = pa.patient_id AND Booking.provider IN (" + matchProviders + ")) > 0 ") + @"
                                " + (matchReferrers      == "" ? "" : " AND (SELECT COUNT(*) FROM PatientReferrer WHERE PatientReferrer.is_active  = 1 AND PatientReferrer.patient_id=pa.patient_id AND register_referrer_id            IN (" + matchReferrers + ")) > 0 ") + @"
                                " + (matchReffererPerson == "" ? "" : " AND (SELECT COUNT(*) FROM PatientReferrer LEFT JOIN RegisterReferrer ON PatientReferrer.register_referrer_id = RegisterReferrer.register_referrer_id WHERE PatientReferrer.is_active  = 1 AND RegisterReferrer.is_deleted = 0 AND patient_id=pa.patient_id AND RegisterReferrer.referrer_id     IN (" + matchReffererPerson + ")) > 0 ") + @"
                                " + (matchReffererOrg    == "" ? "" : " AND (SELECT COUNT(*) FROM PatientReferrer LEFT JOIN RegisterReferrer ON PatientReferrer.register_referrer_id = RegisterReferrer.register_referrer_id WHERE PatientReferrer.is_active  = 1 AND RegisterReferrer.is_deleted = 0 AND patient_id=pa.patient_id AND RegisterReferrer.organisation_id IN (" + matchReffererOrg    + ")) > 0 ") + @"
                                " + (!onlyDiabetics            ? "" : " AND pa.is_diabetic = 1 ") + @"
                                " + (!onlyMedicareEPC          ? "" :
	                                  @" AND (SELECT COALESCE(SUM(num_services_remaining),0) 
	                                          FROM   HealthCardEPCRemaining epcRemaining LEFT JOIN HealthCard AS hc ON epcRemaining.health_card_id = hc.health_card_id
	                                          WHERE  hc.is_active = 1 AND hc.patient_id = pa.patient_id AND hc.organisation_id = -1 AND hc.date_referral_signed > DATEADD(year,-1,GETDATE())) > 0 ") + @"
                                " + (!onlyDVAEPC               ? "" : " AND (SELECT COUNT(*) FROM HealthCard AS hc WHERE is_active = 1 AND hc.patient_id = pa.patient_id AND organisation_id = -2 AND date_referral_signed > DATEADD(year,-1,GETDATE())) > 0 ") + @"
                       ORDER BY 
                                p.surname, p.firstname, p.middlename";

        return !onlyMedicareEPC ? DBBase.ExecuteQuery(sql).Tables[0] : PatientDB.RemooveIfMedicareYearQuotaUsed(DBBase.ExecuteQuery(sql).Tables[0]);
    }


    public static DataTable GetRecallPatients(DateTime from, DateTime to, bool onlyWithActiveEPC, int org_id)
    {
        string sql = @"
            SELECT 

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
                org.last_batch_run as organisation_last_batch_run, org.is_deleted as organisation_is_deleted,


                patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, patient.is_clinic_patient as patient_is_clinic_patient, patient.is_gp_patient as patient_is_gp_patient, patient.is_deleted as patient_is_deleted, patient.is_deceased as patient_is_deceased, 
                patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn, 
                patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info,
                " + PersonDB.GetFields("person_", "person") + @",
                title.title_id as title_title_id, title.descr as title_descr

	
            FROM            Patient      patient
            LEFT OUTER JOIN Person       person  ON person.person_id = patient.person_id
            LEFT OUTER JOIN Title        title   ON title.title_id   = person.title_id
            JOIN            Booking      booking ON booking.booking_id = 
            (
                SELECT   TOP 1 booking_id 
                FROM     Booking
                WHERE    Booking.date_deleted IS NULL AND Booking.patient_id = patient.patient_id
                ORDER BY Booking.date_start DESC
            )
            LEFT OUTER JOIN Organisation org     ON org.organisation_id = booking.organisation_id



            WHERE
		            patient.is_deleted  = 0
	            AND patient.is_deceased = 0
	            AND patient.is_clinic_patient = 1
                AND org.is_deleted = 0" + 
                (from == DateTime.MinValue ? "" : "AND booking.date_start      >= '" + from.ToString("yyyy-MM-dd") + "'") +
                (to   == DateTime.MinValue ? "" : "AND booking.date_start      <= '" + to.ToString("yyyy-MM-dd")   + "'") +
                (org_id == 0               ? "" : "AND booking.organisation_id  = '" + org_id.ToString()           + "'") +

                (!onlyWithActiveEPC ? "" : @"
	            AND (
			            (
			            (SELECT COALESCE(SUM(num_services_remaining),0) 
			             FROM   HealthCardEPCRemaining epcRemaining 
                                LEFT JOIN HealthCard AS hc ON epcRemaining.health_card_id = hc.health_card_id
			             WHERE  epcRemaining.deleted_by IS NULL AND hc.is_active = 1 AND hc.patient_id = patient.patient_id AND hc.organisation_id = -1 AND hc.date_referral_signed > DATEADD(year,-1,GETDATE())) > 0 
			            )
			            OR
			            (
			            (SELECT COUNT(*) FROM HealthCard AS hc WHERE is_active = 1 AND hc.patient_id = patient.patient_id AND organisation_id = -2 AND date_referral_signed > DATEADD(year,-1,GETDATE())) > 0 
			            )
		            )
                ") + @"
	
            ORDER BY
	            booking.date_start DESC
";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable RemooveIfMedicareYearQuotaUsed(DataTable tbl)
    {
        // remove all that have used max yearly amount
        //
        // this is done seperately to the main query because doing this in the main query slows it down way too much
        // as it has to do a joining subquery on every patient, whereas doing it here only does it on the small subset 
        // of the patients that have an EPC card that has not yet expired

        int[] patientIDs = new int[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            patientIDs[i] = Convert.ToInt32(tbl.Rows[i]["patient_id"]);

        int MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);
        System.Collections.Hashtable patientsMedicareCountThisYearCache = PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, DateTime.Now.Year);

        for (int i = tbl.Rows.Count - 1; i >= 0; i--)
        {
            int patientID = Convert.ToInt32(tbl.Rows[i]["patient_id"]);
            if (patientsMedicareCountThisYearCache[patientID] != null && Convert.ToInt32(patientsMedicareCountThisYearCache[patientID]) >= MedicareMaxNbrServicesPerYear)
                tbl.Rows.RemoveAt(i);
        }

        return tbl;
    }

    public static Patient GetByID(int patient_id, string DB = null)
    {
        string sql = @"SELECT
                                pa.patient_id, pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased, 
                                pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, 
                                pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                                pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                                " + PersonDB.GetFields("", "p") + @",
                                t.title_id as t_title_id, t.descr as t_descr
                       FROM     
                                Patient AS pa 
                                INNER JOIN Person p  ON pa.person_id = p.person_id
                                INNER JOIN Title  t  ON t.title_id = p.title_id
                       WHERE    
                                patient_id = " + patient_id.ToString() + @"
                       ORDER BY 
                                p.surname, p.firstname, p.middlename";

        DataTable tbl = DBBase.ExecuteQuery(sql, DB).Tables[0];

        if (tbl.Rows.Count == 0)
            return null;
        else
        {
            Patient p = Load(tbl.Rows[0]);
            p.Person = PersonDB.Load(tbl.Rows[0]);
            p.Person.Title = IDandDescrDB.Load(tbl.Rows[0], "t_title_id", "t_descr");
            return p;
        }
    }
    public static Patient[] GetByIDs(string patient_ids)
    {
        if (patient_ids == null || patient_ids.Length == 0)
            return  new Patient[] { };

        string sql = JoinedSql + " AND patient_id IN (" + patient_ids + ")" + " ORDER BY p.surname, p.firstname";
        DataTable dt = DBBase.ExecuteQuery(sql).Tables[0];

        Patient[] list = new Patient[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
            list[i] = PatientDB.LoadAll(dt.Rows[i], "t_");

        return list;
    }

    public static Hashtable GetByIDsInHashtable(int[] patient_ids)
    {
        if (patient_ids == null || patient_ids.Length == 0)
            return new Hashtable();

        Hashtable hash = new Hashtable();

        Patient[] patients = GetByIDs(string.Join(",", patient_ids));
        foreach(Patient patient in patients)
            hash[patient.PatientID] = patient;

        return hash;
    }


    public static Patient GetByEntityID(int entity_id)
    {
        string sql = @"SELECT
                                pa.patient_id, pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased, 
                                pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, 
                                pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                                pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                                " + PersonDB.GetFields("", "p") + @",
                                t.title_id as t_title_id, t.descr as t_descr
                       FROM     
                                Patient AS pa 
                                INNER JOIN Person p  ON pa.person_id = p.person_id
                                INNER JOIN Title  t  ON t.title_id = p.title_id
                       WHERE    
                                p.entity_id = " + entity_id.ToString() + @"
                       ORDER BY 
                                p.surname, p.firstname, p.middlename";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        if (tbl.Rows.Count == 0)
            return null;
        else
        {
            Patient p = Load(tbl.Rows[0]);
            p.Person = PersonDB.Load(tbl.Rows[0]);
            p.Person.Title = IDandDescrDB.Load(tbl.Rows[0], "t_title_id", "t_descr");
            return p;
        }
    }


    public static int[] GetEntityIDs(int[] patientIDs)
    {
        if (patientIDs == null || patientIDs.Length == 0)
            return new int[] { };

        string sql = 
            @"SELECT patient_id, entity_id
              FROM   Patient 
              LEFT JOIN Person on Patient.person_id = Person.person_id
              WHERE patient_id in (" + string.Join(",", patientIDs) + @")";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        int[] entityIDs = new int[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            entityIDs[i] = Convert.ToInt32(tbl.Rows[i]["entity_id"]);
        return entityIDs;
    }
    public static Hashtable GetEntityIDsHash(int[] patientIDs)
    {
        if (patientIDs == null || patientIDs.Length == 0)
            return new Hashtable();

        string sql =
            @"SELECT patient_id, entity_id
              FROM   Patient 
              LEFT JOIN Person on Patient.person_id = Person.person_id
              WHERE patient_id in (" + string.Join(",", patientIDs) + @")";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
            hash[Convert.ToInt32(tbl.Rows[i]["patient_id"])] = Convert.ToInt32(tbl.Rows[i]["entity_id"]);
        return hash;
    }



    public static Patient[] GetByPhoneNbr(string phoneNbr)
    {
        phoneNbr = phoneNbr.Replace("'", "''");

        string sql = @"SELECT
                                pa.patient_id, pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased, 
                                pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, 
                                pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                                pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                                " + PersonDB.GetFields("", "p") + @",
                                t.title_id as t_title_id, t.descr as t_descr
                       FROM
                                Patient AS pa 
                                INNER JOIN Person  p  ON pa.person_id = p.person_id
                                INNER JOIN Title   t  ON t.title_id   = p.title_id
                                INNER JOIN " + Utilities.GetAddressType().ToString()+@" ph ON p.entity_id  = ph.entity_id
                       WHERE
                                ph.contact_date_deleted IS NULL AND ph.addr_line1 = '" + phoneNbr + "'" + @"
                       ORDER BY 
                                p.surname, p.firstname, p.middlename";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i], "t_");
        return list;
    }
    public static Patient[] GetByEmail(string email)
    {
        email = email.Replace("'", "''");

        string sql = @"SELECT
                                pa.patient_id, pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased, 
                                pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, 
                                pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                                pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                                " + PersonDB.GetFields("", "p") + @",
                                t.title_id as t_title_id, t.descr as t_descr
                       FROM
                                Patient AS pa 
                                INNER JOIN Person  p  ON pa.person_id = p.person_id
                                INNER JOIN Title   t  ON t.title_id   = p.title_id
                                INNER JOIN " + Utilities.GetAddressType().ToString()+@" ph ON p.entity_id  = ph.entity_id
                       WHERE
                                ph.contact_date_deleted IS NULL AND ph.addr_line1 = '" + email + "'" + @"
                       ORDER BY 
                                p.surname, p.firstname, p.middlename";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i], "t_");
        return list;
    }

    public static Patient[] GetByFirstnameSurnameDOB(string firstname, string surname, DateTime dob)
    {
        firstname = firstname.Replace("'", "''");
        surname   = surname.Replace("'", "''");

        string sql = JoinedSql + " AND p.firstname = '" + firstname + "' AND p.surname = '" + surname + "' AND DATEADD(dd, 0, DATEDIFF(dd, 0, p.dob)) = '" + dob.ToString("yyyy-MM-dd") + "' ORDER BY p.surname, p.firstname";
        DataTable dt = DBBase.ExecuteQuery(sql).Tables[0];

        Patient[] list = new Patient[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
            list[i] = PatientDB.LoadAll(dt.Rows[i], "t_");

        return list;

    }


    public static Patient[] DuplicateSearch(string firstname, string middlename, string surname)
    {
        firstname  = firstname.Trim().Replace("'", "''");;
        middlename = middlename.Trim().Replace("'", "''");;
        surname    = surname.Trim().Replace("'", "''");;

        string sql = JoinedSql;
        if (firstname.Length > 0)
            sql += " AND (p.firstname = '" + firstname + "' OR SOUNDEX(p.firstname) = SOUNDEX('" + firstname + "'))";
        if (middlename.Length > 0)
            sql += " AND (p.middlename = '" + middlename + "' OR SOUNDEX(p.middlename) = SOUNDEX('" + middlename + "'))";
        if (surname.Length > 0)
            sql += " AND (p.surname = '" + surname + "' OR SOUNDEX(p.surname) = SOUNDEX('" + surname + "'))";


        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i], "t_");
        return list;
    }

    public static DataTable GetDataTable_AllNotInc(Patient[] excList)
    {
        string notInList = string.Empty;
        foreach (Patient p in excList)
            notInList += p.PatientID.ToString() + ",";
        if (notInList.Length > 0)
            notInList = notInList.Substring(0, notInList.Length - 1);

        string sql = @"SELECT 
                                pa.patient_id, pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased, 
                                pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, 
                                pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                                pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                                " + PersonDB.GetFields("", "p") + @"
                       FROM
                                Patient AS pa
                                LEFT OUTER JOIN Person AS p ON pa.person_id = p.person_id
                                INNER JOIN Title  t  ON t.title_id = p.title_id
                       WHERE
                                pa.is_deleted = 0 " + ((notInList.Length > 0) ? " AND patient_id NOT IN (" + notInList + ") " : "") + @"
                       ORDER BY 
                                p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Patient[] GetAllNotInc(Patient[] excList)
    {
        DataTable tbl = GetDataTable_AllNotInc(excList);
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = PatientDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i]);
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "t_title_id", "t_descr");
        }

        return list;
    }

    public static Patient[] GetBirthdays(DateTime date)
    {
        string sql = JoinedSql + " AND pa.is_deceased = 0 AND DATEPART(month,p.dob) = " + date.Month + " AND DATEPART(day,p.dob) = " + date.Day + " AND pa.patient_id in (SELECT patient_id FROM RegisterPatient LEFT JOIN Organisation ON RegisterPatient.organisation_id = Organisation.organisation_id WHERE Organisation.is_deleted = 0 AND Organisation.organisation_type_id = 218)";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = PatientDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i]);
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "t_title_id", "t_descr");
        }

        return list;
    }
    public static Patient[] GetBirthdays(int monthStart, int dayStart, int monthEnd, int dayEnd)
    {
        DataTable tbl = GetBirthdays_DataTable(monthStart, dayStart, monthEnd, dayEnd);
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = PatientDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i]);
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "t_title_id", "t_descr");
        }

        return list;
    }
    public static DataTable GetBirthdays_DataTable(int monthStart, int dayStart, int monthEnd, int dayEnd)
    {
        DateTime startDate = new DateTime(2004, monthStart, dayStart);
        DateTime endDate   = new DateTime(2004, monthEnd,   dayEnd);
        if (endDate < startDate) endDate = endDate.AddYears(1);  // if say dec 20 - jan 10, make the jan 10 for next year

        if (endDate.Subtract(startDate).TotalDays > 62)
            throw new CustomMessageException("Can not select more than 2 months");

        string dates = string.Empty;
        for (DateTime curDate = startDate; curDate <= endDate; curDate = curDate.AddDays(1))
            dates += (dates.Length == 0 ? "" : " OR ") + "(DATEPART(month,p.dob) = " + curDate.Month + " AND DATEPART(day,p.dob) = " + curDate.Day + ")";
        dates = @" AND ( " + dates + @" ) ";

        string sql = JoinedSql + " AND pa.is_deceased = 0 " + dates + " AND pa.patient_id in (SELECT patient_id FROM RegisterPatient LEFT JOIN Organisation ON RegisterPatient.organisation_id = Organisation.organisation_id where Organisation.is_deleted = 0 AND Organisation.organisation_type_id = 218)" + " ORDER BY DATEPART(month,p.dob), DATEPART(day,p.dob), p.firstname, p.surname, p.middlename ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }


    public static Hashtable GetFlashingNotesCache(Booking[] bookings)
    {
        ArrayList ptIDs = new ArrayList();
        foreach(Booking booking in bookings)
            if (booking.Patient != null)
                ptIDs.Add(booking.Patient.PatientID);
        return GetFlashingNotesCache((int[])ptIDs.ToArray(typeof(int)));
    }
    public static Hashtable GetFlashingNotesCache(int[] ptIDs)
    {
        if (ptIDs.Length == 0)
            return new Hashtable();

        string sql = @"SELECT patient_id, flashing_text, flashing_text_last_modified_date
                       FROM Patient
                       WHERE patient_id IN (" + string.Join(",", ptIDs) + @")";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            if (((string)tbl.Rows[i]["flashing_text"]).Trim().Length > 0)
            {
                if (tbl.Rows[i]["flashing_text_last_modified_date"] != DBNull.Value)
                    hash[(int)tbl.Rows[i]["patient_id"]] = (string)tbl.Rows[i]["flashing_text"] + " [" + ((DateTime)tbl.Rows[i]["flashing_text_last_modified_date"]).ToString("d/M/yyyy") + "]";
                else
                    hash[(int)tbl.Rows[i]["patient_id"]] = (string)tbl.Rows[i]["flashing_text"];
            }
        }

        return hash;
    }

    public static Hashtable GetOwingCache(Booking[] bookings)
    {
        ArrayList ptIDs = new ArrayList();
        foreach(Booking booking in bookings)
            if (booking.Patient != null)
                ptIDs.Add(booking.Patient.PatientID);
        return GetOwingCache((int[])ptIDs.ToArray(typeof(int)));
    }
    public static Hashtable GetOwingCache(int[] ptIDs)
    {
        if (ptIDs.Length == 0)
            return new Hashtable();

        string sql = @"SELECT 
	                        Invoice.payer_patient_id AS inv_pt_id,
	                        Booking.patient_id       AS bk_pt_id,

	                        (Invoice.total - 
	                        (SELECT      COALESCE(SUM(total) , 0) FROM Receipt    WHERE invoice_id            = Invoice.invoice_id) -
	                        (SELECT      COALESCE(SUM(total) , 0) FROM CreditNote WHERE CreditNote.invoice_id = Invoice.invoice_id) - 
	                        (SELECT      COALESCE(SUM(total) , 0) FROM Refund     WHERE Refund.invoice_id     = Invoice.invoice_id) - 
	                        (SELECT -1 * COALESCE(SUM(amount), 0) FROM Credit     WHERE Credit.invoice_id     = Invoice.invoice_id)) as owing
	
                       FROM Invoice 
                            left join Booking on Booking.booking_id  = Invoice.booking_id 

                       WHERE 
	                        (Invoice.payer_patient_id in (" + string.Join(",", ptIDs) + @") OR (Invoice.payer_organisation_id IS NULL AND Booking.patient_id in (" + string.Join(",", ptIDs) + @")))
	                        AND
	                        (Invoice.total - 
	                        (SELECT      COALESCE(SUM(total) , 0) FROM Receipt    WHERE invoice_id            = Invoice.invoice_id) -
	                        (SELECT      COALESCE(SUM(total) , 0) FROM CreditNote WHERE CreditNote.invoice_id = Invoice.invoice_id) - 
	                        (SELECT      COALESCE(SUM(total) , 0) FROM Refund     WHERE Refund.invoice_id     = Invoice.invoice_id) - 
	                        (SELECT -1 * COALESCE(SUM(amount), 0) FROM Credit     WHERE Credit.invoice_id     = Invoice.invoice_id)) > 0";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int     inv_pt_id = tbl.Rows[i]["inv_pt_id"] == DBNull.Value ? -1 : Convert.ToInt32(tbl.Rows[i]["inv_pt_id"]);
            int     bk_pt_id  = tbl.Rows[i]["bk_pt_id"]  == DBNull.Value ? -1 : Convert.ToInt32(tbl.Rows[i]["bk_pt_id"]);
            int     pt_id     = inv_pt_id != -1 ? inv_pt_id : bk_pt_id;
            decimal owing     = Convert.ToDecimal(tbl.Rows[i]["owing"]);

            if (hash[pt_id] == null)
                hash[pt_id] = owing;
            else
                hash[pt_id] = Convert.ToDecimal(hash[pt_id]) + owing;
        }

        return hash;
    }



    public static void AddACOfferings(ref Hashtable offerings, ref DataTable tbl, string ac_inv_offering_id_col, string ac_inv_offering_descr_col, string ac_pat_offering_id_col, string ac_pat_offering_descr_col, 
        string ac_inv_aged_care_patient_type_id_col, string ac_inv_aged_care_patient_type_descr_col,
        string ac_pat_aged_care_patient_type_id_col, string ac_pat_aged_care_patient_type_descr_col)
    {
        tbl.Columns.Add(ac_inv_offering_descr_col, typeof(String));
        tbl.Columns.Add(ac_pat_offering_descr_col, typeof(String));

        tbl.Columns.Add(ac_inv_aged_care_patient_type_id_col, typeof(String));
        tbl.Columns.Add(ac_inv_aged_care_patient_type_descr_col, typeof(String));
        tbl.Columns.Add(ac_pat_aged_care_patient_type_id_col, typeof(String));
        tbl.Columns.Add(ac_pat_aged_care_patient_type_descr_col, typeof(String));
        

        for(int i=0; i<tbl.Rows.Count; i++)
        {
            tbl.Rows[i][ac_inv_offering_descr_col] = (tbl.Rows[i][ac_inv_offering_id_col] == DBNull.Value) ? (object)DBNull.Value : ((Offering)offerings[Convert.ToInt32(tbl.Rows[i][ac_inv_offering_id_col])]).Name;
            tbl.Rows[i][ac_pat_offering_descr_col] = (tbl.Rows[i][ac_pat_offering_id_col] == DBNull.Value) ? (object)DBNull.Value : ((Offering)offerings[Convert.ToInt32(tbl.Rows[i][ac_pat_offering_id_col])]).Name;

            tbl.Rows[i][ac_inv_aged_care_patient_type_id_col]    = (tbl.Rows[i][ac_inv_offering_id_col] == DBNull.Value) ? (object)DBNull.Value : ((Offering)offerings[Convert.ToInt32(tbl.Rows[i][ac_inv_offering_id_col])]).AgedCarePatientType.ID;
            tbl.Rows[i][ac_inv_aged_care_patient_type_descr_col] = (tbl.Rows[i][ac_inv_offering_id_col] == DBNull.Value) ? (object)DBNull.Value : ((Offering)offerings[Convert.ToInt32(tbl.Rows[i][ac_inv_offering_id_col])]).AgedCarePatientType.Descr;
            tbl.Rows[i][ac_pat_aged_care_patient_type_id_col]    = (tbl.Rows[i][ac_pat_offering_id_col] == DBNull.Value) ? (object)DBNull.Value : ((Offering)offerings[Convert.ToInt32(tbl.Rows[i][ac_pat_offering_id_col])]).AgedCarePatientType.ID;
            tbl.Rows[i][ac_pat_aged_care_patient_type_descr_col] = (tbl.Rows[i][ac_pat_offering_id_col] == DBNull.Value) ? (object)DBNull.Value : ((Offering)offerings[Convert.ToInt32(tbl.Rows[i][ac_pat_offering_id_col])]).AgedCarePatientType.Descr;
        }
    }
    public static void AddACOfferings(ref Hashtable offerings, ref Patient[] patients)
    {
        for (int i = 0; i < patients.Length; i++)
            AddACOfferings(ref offerings, ref patients[i]);
    }
    public static void AddACOfferings(ref Hashtable offerings, ref Patient patient)
    {
        if (patient.ACInvOffering != null)
            patient.ACInvOffering = (Offering)offerings[patient.ACInvOffering.OfferingID];
        if (patient.ACPatOffering != null)
            patient.ACPatOffering = (Offering)offerings[patient.ACPatOffering.OfferingID];
    }
    public static void AddACOfferings(ref Hashtable offerings, ref BookingPatient[] bookingPatients)
    {
        for (int i = 0; i < bookingPatients.Length; i++)
            AddACOfferings(ref offerings, ref bookingPatients[i]);
    }
    public static void AddACOfferings(ref Hashtable offerings, ref BookingPatient bookingPatient)
    {
        if (bookingPatient.Patient.ACInvOffering != null)
            bookingPatient.Patient.ACInvOffering = (Offering)offerings[bookingPatient.Patient.ACInvOffering.OfferingID];
        if (bookingPatient.Patient.ACPatOffering != null)
            bookingPatient.Patient.ACPatOffering = (Offering)offerings[bookingPatient.Patient.ACPatOffering.OfferingID];
    }


    public static Patient[] RemovePatientsByEmailMobile(
        Patient[] patients, 
        bool incHasBothMobileEmail,
        bool incHasMobileNoEmail,
        bool incHasEmailNoMobile,
        bool incHasNeitherMobileEmail)
    {
        if (incHasBothMobileEmail && incHasMobileNoEmail && incHasEmailNoMobile && incHasNeitherMobileEmail)
            return patients;
        if (!incHasBothMobileEmail && !incHasMobileNoEmail && !incHasEmailNoMobile && !incHasNeitherMobileEmail)
            return new Patient[]{ };

        int[] patientIDs = patients.Select(r => r.PatientID).ToArray();
        int[] entityIDs  = patients.Select(r => r.Person.EntityID).ToArray();
        Hashtable entityIDHash = PatientDB.GetEntityIDsHash(patientIDs);
        Hashtable patientIDHash = PatientDB.GetByIDsInHashtable(patientIDs);
        Hashtable emailHash  = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);
        Hashtable mobileHash = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1, "30");

        ArrayList newList = new ArrayList();
        for (int i = 0; i < patients.Length; i++)
        {
            bool hasEmail  = emailHash[patients[i].Person.EntityID]  != null;
            bool hasMobile = mobileHash[patients[i].Person.EntityID] != null;

            if ((incHasBothMobileEmail     &&  hasEmail &&  hasMobile) ||
                 (incHasMobileNoEmail      && !hasEmail &&  hasMobile) ||
                 (incHasEmailNoMobile      &&  hasEmail && !hasMobile) ||
                 (incHasNeitherMobileEmail && !hasEmail && !hasMobile))
                    newList.Add(patients[i]);
        }

        return (Patient[])newList.ToArray(typeof(Patient));
    }
    public static ArrayList RemovePatientsByEmailMobile(
        ArrayList patients, 
        bool incHasBothMobileEmail,
        bool incHasMobileNoEmail,
        bool incHasEmailNoMobile,
        bool incHasNeitherMobileEmail)
    {
        return new ArrayList(RemovePatientsByEmailMobile((Patient[])patients.ToArray(typeof(Patient)), incHasBothMobileEmail, incHasMobileNoEmail, incHasEmailNoMobile, incHasNeitherMobileEmail));
    }


    public static int GetEntityIDByPatientID(int patientID)
    {
        string sql = "SELECT Person.entity_id FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE Patient.patient_id = " + patientID;
        return (int)DBBase.ExecuteSingleResult(sql);
    }


    public static Patient GetByLoginPwd(string login, string pwd)
    {
        login = login.Replace("'", "''");
        pwd   = pwd.Replace("'", "''");
        string sql = JoinedSql + " AND login COLLATE Latin1_General_CS_AS = '" + login + "' AND pwd = '" + pwd + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }
    public static Patient GetByLogin(string login)
    {
        login = login.Replace("'", "''");
        string sql = JoinedSql + " AND login COLLATE Latin1_General_CS_AS = '" + login + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0], "t_");

    }
    public static bool LoginExists(string login, int patient_id_exclude = -1)
    {
        login = login.Replace("'", "''");
        string sql = "SELECT COUNT(*) FROM Patient WHERE login COLLATE Latin1_General_CS_AS = '" + login + "'" + (patient_id_exclude == -1 ? "" : " AND patient_id <> " + patient_id_exclude);
        return (Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0);
    }



    public static Patient LoadAll(DataRow row, string title_prefix = "")
    {
        Patient patient = Load(row);
        patient.Person = PersonDB.Load(row);
        patient.Person.Title = IDandDescrDB.Load(row, title_prefix+"title_id", title_prefix+"descr");
        return patient;
    }

    public static Patient Load(DataRow row, string prefix = "")
    {
        return new Patient(
            Convert.ToInt32(row[prefix + "patient_id"]),
            Convert.ToInt32(row[prefix + "person_id"]),
            Convert.ToDateTime(row[prefix + "patient_date_added"]),
            Convert.ToBoolean(row[prefix + "is_clinic_patient"]),
            Convert.ToBoolean(row[prefix + "is_gp_patient"]),
            Convert.ToBoolean(row[prefix + "is_deleted"]),
            Convert.ToBoolean(row[prefix + "is_deceased"]),
            Convert.ToString(row[prefix + "flashing_text"]),
            row[prefix + "flashing_text_added_by"]           == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "flashing_text_added_by"]),
            row[prefix + "flashing_text_last_modified_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "flashing_text_last_modified_date"]),
            Convert.ToString(row[prefix + "private_health_fund"]),
            Convert.ToString(row[prefix + "concession_card_number"]),
            row[prefix + "concession_card_expiry_date"]      == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "concession_card_expiry_date"]),
            Convert.ToBoolean(row[prefix + "is_diabetic"]),
            Convert.ToBoolean(row[prefix + "is_member_diabetes_australia"]),
            row[prefix + "diabetic_assessment_review_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "diabetic_assessment_review_date"]),
            row[prefix + "ac_inv_offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "ac_inv_offering_id"]),
            row[prefix + "ac_pat_offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "ac_pat_offering_id"]),
            Convert.ToString(row[prefix + "login"]),
            Convert.ToString(row[prefix + "pwd"]),
            Convert.ToBoolean(row[prefix + "is_company"]),
            Convert.ToString(row[prefix + "abn"]),
            Convert.ToString(row[prefix + "next_of_kin_name"]),
            Convert.ToString(row[prefix + "next_of_kin_relation"]),
            Convert.ToString(row[prefix + "next_of_kin_contact_info"])
        );
    }

}