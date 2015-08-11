using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class RegisterPatientDB
{

    public static void Delete(int register_patient_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM RegisterPatient WHERE register_patient_id = " + register_patient_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int organisation_id, int patient_id)
    {
        string nRowsSql = "SELECT COUNT(*) FROM RegisterPatient WHERE is_deleted = 0 AND organisation_id = " + organisation_id.ToString() + " AND patient_id = " + patient_id.ToString();
        if (Convert.ToInt32(DBBase.ExecuteSingleResult(nRowsSql)) > 0)
            throw new UniqueConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        string sql = "INSERT INTO RegisterPatient (organisation_id,patient_id) VALUES (" + "" + organisation_id + "," + "" + patient_id + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int register_patient_id, int organisation_id, int patient_id)
    {
        string sql = "UPDATE RegisterPatient SET organisation_id = " + organisation_id + ",patient_id = " + patient_id + " WHERE register_patient_id = " + register_patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int register_patient_id, bool checkFKConstraint=true)
    {
        if (checkFKConstraint)
        {
            RegisterPatient registerPatient = RegisterPatientDB.GetByID(register_patient_id);
            if (BookingDB.GetCountByPatientAndOrg(registerPatient.Patient.PatientID, registerPatient.Organisation.OrganisationID) > 0)
                throw new CustomMessageException("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' because there exists a booking for this patient there.");
        }

        string sql = "UPDATE RegisterPatient SET is_deleted = 1 WHERE register_patient_id = " + register_patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int patient_id, int organisation_id, bool checkFKConstraint = true)
    {
        if (checkFKConstraint)
        {
            if (BookingDB.GetCountByPatientAndOrg(patient_id, organisation_id) > 0)
            {
                Patient      patient = PatientDB.GetByID(patient_id);
                Organisation org     = OrganisationDB.GetByID(organisation_id);
                throw new CustomMessageException("Can not remove registration of '" + patient.Person.FullnameWithoutMiddlename + "' to '" + org.Name + "' because there exists a booking for this patient there.");
            }
        }

        string sql = "UPDATE RegisterPatient SET is_deleted = 1 WHERE patient_id = " + patient_id + " AND organisation_id = " + organisation_id;
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int register_patient_id, bool checkFKConstraint = true)
    {
        if (checkFKConstraint)
        {
            RegisterPatient registerPatient = RegisterPatientDB.GetByID(register_patient_id);
            string nRowsSql = "SELECT COUNT(*) FROM RegisterPatient WHERE is_deleted = 0 AND organisation_id = " + registerPatient.Organisation.OrganisationID.ToString() + " AND patient_id = " + registerPatient.Patient.PatientID.ToString();
            if (Convert.ToInt32(DBBase.ExecuteSingleResult(nRowsSql)) > 0)
                throw new UniqueConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        }

        string sql = "UPDATE RegisterPatient SET is_deleted = 0 WHERE register_patient_id = " + register_patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }


    public static int GetCountByPatientAndOrgTypeGroup(int patient_id, string organistion_group_type_ids)
    {
        if (organistion_group_type_ids.Length == 0)
            organistion_group_type_ids = "0";

        string sql = @"
                SELECT 
                    COUNT(*) 
                FROM RegisterPatient 
                     INNER JOIN Organisation ON Organisation.organisation_id = RegisterPatient.organisation_id
                     INNER JOIN OrganisationType ON OrganisationType.organisation_type_id = Organisation.organisation_type_id
                WHERE 
                     RegisterPatient.patient_id = " + patient_id + @"
                     AND OrganisationType.organisation_type_group_id IN (" + organistion_group_type_ids + @")
                     AND RegisterPatient.is_deleted = 0";

        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static DataTable GetPatientsAddedByOrg(int organisation_id, DateTime start, DateTime end)
    {
        string sql = @"

                 SELECT DISTINCT  

                          pat.patient_id as patient_patient_id,
                          pat.patient_date_added as patient_patient_date_added,
                          pat.person_id as patient_person_id, pat.patient_date_added as patient_patient_date_added,pat.is_clinic_patient as patient_is_clinic_patient,pat.is_gp_patient as patient_is_gp_patient,pat.is_deleted as patient_is_deleted,pat.is_deceased as patient_is_deceased,
                          pat.flashing_text as patient_flashing_text, pat.flashing_text_added_by as patient_flashing_text_added_by, pat.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                          pat.private_health_fund as patient_private_health_fund, pat.concession_card_number as patient_concession_card_number, pat.concession_card_expiry_date as patient_concession_card_expiry_date, pat.is_diabetic as patient_is_diabetic, pat.is_member_diabetes_australia as patient_is_member_diabetes_australia, pat.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, pat.ac_inv_offering_id as patient_ac_inv_offering_id, pat.ac_pat_offering_id as patient_ac_pat_offering_id, pat.login as patient_login, pat.pwd as patient_pwd, pat.is_company as patient_is_company, pat.abn as patient_abn, 
                          pat.next_of_kin_name as patient_next_of_kin_name, pat.next_of_kin_relation as patient_next_of_kin_relation, pat.next_of_kin_contact_info as patient_next_of_kin_contact_info,
                          " + PersonDB.GetFields("patient_person_", "p") + @", 
                          t.title_id as patient_person_title_title_id, t.descr as patient_person_title_descr,

                          -- these must be selected in the cross apply select fields
                          referrer_info.patient_referrer_id as referrer_info_patient_referrer_id, referrer_info.register_referrer_id as referrer_info_register_referrer_id,  
                          referrer_info.referrer_id as referrer_info_referrer_id, referrer_info.firstname as referrer_info_firstname, referrer_info.surname as referrer_info_surname

                 FROM     
                          RegisterPatient 
                          INNER JOIN Patient pat      ON RegisterPatient.patient_id = pat.patient_id AND CONVERT(DATE, RegisterPatient.register_patient_date_added) = CONVERT(DATE, pat.patient_date_added) 
                          INNER JOIN Person  p        ON p.person_id       = pat.person_id
                          INNER JOIN Title   t        ON t.title_id        = p.title_id

                          OUTER APPLY
                                (
                                SELECT  TOP 1 PatientReferrer.patient_referrer_id, PatientReferrer.register_referrer_id, Referrer.referrer_id, Person.firstname, Person.surname
                                FROM    PatientReferrer
                                               JOIN RegisterReferrer ON RegisterReferrer.register_referrer_id = PatientReferrer.register_referrer_id
                                               JOIN Referrer ON Referrer.referrer_id =  RegisterReferrer.referrer_id
                                               JOIN Person ON Person.person_id = Referrer.person_id

                                WHERE   PatientReferrer.patient_id = RegisterPatient.patient_id
                                ORDER BY PatientReferrer.patient_referrer_id asc
                                ) referrer_info

                 WHERE    
                          RegisterPatient.is_deleted = 0 AND RegisterPatient.organisation_id = " + organisation_id + " " +
                          (start == DateTime.MinValue ? "" : " AND RegisterPatient.register_patient_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND RegisterPatient.register_patient_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @" 
                 ORDER BY  
                          referrer_info.surname, referrer_info.firstname,
                          p.surname, p.firstname, p.middlename ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }






    public static bool IsPatientRegisteredToOrg(int patient_id, int organisation_id)
    {
        string sql = "SELECT COUNT(*) FROM RegisterPatient WHERE is_deleted = 0 AND patient_id = " + patient_id.ToString() + " AND organisation_id = " + organisation_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }


    public static RegisterPatient[] GetAll(bool inc_deleted_patients = true, bool inc_deceased_patients = true, bool inc_deleted_orgs = true, string organistion_group_type_ids = null, string organistion_type_ids = null)
    {
        DataTable tbl = GetDataTable(inc_deleted_patients, inc_deceased_patients, inc_deleted_orgs, organistion_group_type_ids, organistion_type_ids);
        RegisterPatient[] list = new RegisterPatient[tbl.Rows.Count];
        for(int i=0; i<tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }

    public static DataTable GetDataTable(bool inc_deleted_patients = true, bool inc_deceased_patients = true, bool inc_deleted_orgs = true, string organistion_group_type_ids = null, string organistion_type_ids = null)
    {
        string sql = JoinedSql()
            + (organistion_type_ids       != null && organistion_type_ids.Length       > 0 ? " AND o.organisation_type_id IN        (" + organistion_type_ids       + ")" : "")
            + (organistion_group_type_ids != null && organistion_group_type_ids.Length > 0 ? " AND ot.organisation_type_group_id IN (" + organistion_group_type_ids + ")" : "")
            + (inc_deleted_patients  ? "" : " AND pat.is_deleted  = 0")
            + (inc_deceased_patients ? "" : " AND pat.is_deceased = 0")
            + (inc_deleted_orgs      ? "" : " AND o.is_deleted    = 0");
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static RegisterPatient GetByID(int register_patient_id)
    {
        string sql = JoinedSql() + " AND register_patient_id = " + register_patient_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    protected static string JoinedSql(bool onlyTopRow = false)
    {
        return @"SELECT " + (onlyTopRow ? " top 1 " : "") + @"
                      r.register_patient_id, r.organisation_id, r.patient_id, r.register_patient_date_added, 
                      o.entity_id as organisation_entity_id, o.parent_organisation_id, o.use_parent_offernig_prices, 
                      o.organisation_type_id, o.organisation_customer_type_id, o.name, o.acn, o.abn, o.organisation_date_added, o.organisation_date_modified, o.is_debtor, o.is_creditor, o.bpay_account, 
                      o.weeks_per_service_cycle, o.start_date, o.end_date, o.comment, o.free_services, o.excl_sun, o.excl_mon, o.excl_tue, o.excl_wed, o.excl_thu, o.excl_fri, o.excl_sat, 
                      o.sun_start_time, o.sun_end_time, o.mon_start_time, o.mon_end_time, o.tue_start_time, o.tue_end_time, o.wed_start_time, o.wed_end_time, 
                      o.thu_start_time, o.thu_end_time, o.fri_start_time, o.fri_end_time, o.sat_start_time, o.sat_end_time, 
                      o.sun_lunch_start_time, o.sun_lunch_end_time, o.mon_lunch_start_time, o.mon_lunch_end_time, o.tue_lunch_start_time, o.tue_lunch_end_time, o.wed_lunch_start_time, o.wed_lunch_end_time, 
                      o.thu_lunch_start_time, o.thu_lunch_end_time, o.fri_lunch_start_time, o.fri_lunch_end_time, o.sat_lunch_start_time, o.sat_lunch_end_time, 
                      o.last_batch_run, o.is_deleted as organisation_is_deleted, 
                      pat.person_id, pat.patient_date_added,pat.is_clinic_patient,pat.is_gp_patient,pat.is_deleted,pat.is_deceased,
                      pat.flashing_text, pat.flashing_text_added_by, pat.flashing_text_last_modified_date, 
                      pat.private_health_fund, pat.concession_card_number, pat.concession_card_expiry_date, pat.is_diabetic, pat.is_member_diabetes_australia, pat.diabetic_assessment_review_date, pat.ac_inv_offering_id, pat.ac_pat_offering_id, pat.login, pat.pwd, pat.is_company, pat.abn, 
                      pat.next_of_kin_name, pat.next_of_kin_relation, pat.next_of_kin_contact_info,
                      " + PersonDB.GetFields("", "p").Replace("p.entity_id", "p.entity_id AS person_entity_id") + @", 
                      t.title_id, t.descr
        FROM
                      RegisterPatient AS r 
                      INNER JOIN Organisation o   ON o.organisation_id = r.organisation_id 
                      INNER JOIN OrganisationType  ot  ON ot.organisation_type_id = o.organisation_type_id
                      INNER JOIN Patient      pat ON pat.patient_id    = r.patient_id 
                      INNER JOIN Person       p   ON p.person_id       = pat.person_id
                      INNER JOIN Title        t   ON t.title_id        = p.title_id
        WHERE
                      r.is_deleted = 0";
    }



    public static DataTable GetDataTable_OrganisationsOf(int patient_id, bool exclGroupOrg = true, bool exclClinics = false, bool exclAgedCareFacs = false, bool exclIns = true, bool exclExternal = false, string matchName = "", bool searchNameOnlyStartsWith = false)
    {
        return GetDataTable_OrganisationsOf(new int[] { patient_id }, exclGroupOrg, exclClinics, exclAgedCareFacs, exclIns, exclExternal, matchName, searchNameOnlyStartsWith);
    }
    public static DataTable GetDataTable_OrganisationsOf(int[] patient_ids, bool exclGroupOrg = true, bool exclClinics = false, bool exclAgedCareFacs = false, bool exclIns = true, bool exclExternal = false, string matchName = "", bool searchNameOnlyStartsWith = false)
    {
        matchName = matchName.Replace("'", "''");
        if (patient_ids == null || patient_ids.Length == 0)
            patient_ids = new int[] { -1 };

        string BaseSql = @"SELECT
                                    r.register_patient_id, r.organisation_id, r.patient_id, r.register_patient_date_added,
                                    o.organisation_id,o.entity_id as organisation_entity_id,o.parent_organisation_id,parent.name as parent_name, o.use_parent_offernig_prices, o.organisation_type_id,o.organisation_customer_type_id,o.name,o.acn,o.abn,o.organisation_date_added,o.organisation_date_modified,o.is_debtor,o.is_creditor,o.bpay_account,o.weeks_per_service_cycle,o.start_date,o.end_date,o.comment,o.free_services,o.excl_sun,o.excl_mon,o.excl_tue,o.excl_wed,o.excl_thu,o.excl_fri,o.excl_sat,
                                    o.sun_start_time,o.sun_end_time,o.mon_start_time,o.mon_end_time,o.tue_start_time,o.tue_end_time,o.wed_start_time,o.wed_end_time,o.thu_start_time,o.thu_end_time,o.fri_start_time,o.fri_end_time,o.sat_start_time,o.sat_end_time,
                                    o.sun_lunch_start_time,o.sun_lunch_end_time,o.mon_lunch_start_time,o.mon_lunch_end_time,o.tue_lunch_start_time,o.tue_lunch_end_time,o.wed_lunch_start_time,o.wed_lunch_end_time,o.thu_lunch_start_time,o.thu_lunch_end_time,o.fri_lunch_start_time,o.fri_lunch_end_time,o.sat_lunch_start_time,o.sat_lunch_end_time,
                                    o.last_batch_run, o.is_deleted as organisation_is_deleted,
                                    type.organisation_type_id as type_organisation_type_id,type.descr as type_descr,type.organisation_type_group_id as organisation_type_group_id,
                                    typegroup.organisation_type_group_id as typegroup_organisation_type_group_id, typegroup.descr as typegroup_descr,
                                    ct.organisation_customer_type_id as ct_organisation_customer_type_id, ct.descr as ct_descr
                            FROM    
                                    RegisterPatient r
                                    INNER JOIN      Organisation             o         ON r.organisation_id = o.organisation_id
                                    LEFT OUTER JOIN Organisation             parent    ON o.parent_organisation_id = parent.organisation_id
                                    LEFT OUTER JOIN OrganisationType         type      ON o.organisation_type_id = type.organisation_type_id 
                                    LEFT OUTER JOIN OrganisationTypeGroup    typegroup ON type.organisation_type_group_id = typegroup.organisation_type_group_id
                                    INNER JOIN      OrganisationCustomerType ct        ON o.organisation_customer_type_id = ct.organisation_customer_type_id ";


        string notInTypeGroupList = string.Empty;
        if (exclGroupOrg)
            notInTypeGroupList += (notInTypeGroupList.Length > 0 ? "," : "") + "1,2,3";
        if (exclExternal)
            notInTypeGroupList += (notInTypeGroupList.Length > 0 ? "," : "") + "4";
        if (exclClinics)
            notInTypeGroupList += (notInTypeGroupList.Length > 0 ? "," : "") + "5";
        if (exclAgedCareFacs)
            notInTypeGroupList += (notInTypeGroupList.Length > 0 ? "," : "") + "6";
        if (exclIns)
            notInTypeGroupList += (notInTypeGroupList.Length > 0 ? "," : "") + "7";



        string sqlGroups = BaseSql + "  WHERE r.is_deleted = 0 AND o.is_deleted = 0 AND r.patient_id IN (" + string.Join(",", patient_ids) + @") AND type.organisation_type_group_id IN (1) ";
        string sqlNoGroups = BaseSql + "  WHERE r.is_deleted = 0 AND o.is_deleted = 0 AND r.patient_id IN(" + string.Join(",", patient_ids) + @") AND type.organisation_type_group_id NOT IN (1,2,3) " + ((notInTypeGroupList.Length > 0) ? " AND type.organisation_type_group_id NOT IN (" + notInTypeGroupList + @") " : "") + ((matchName.Length > 0 && !searchNameOnlyStartsWith) ? " AND o.name LIKE '%" + matchName + "%'" : "") + ((matchName.Length > 0 && searchNameOnlyStartsWith) ? " AND o.name LIKE '" + matchName + "%'" : "");
        string sql = exclGroupOrg ?
                        sqlNoGroups + " ORDER BY o.name" :
                        sql = sql = "SELECT * FROM (" + AddField(sqlGroups, "TMP_ORD = 0") + " UNION " + AddField(sqlNoGroups, "TMP_ORD = 1") + ") x ORDER BY TMP_ORD, name";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static string AddField(string sql, string fields)
    {
        string[] parts = sql.Split(new string[] { "FROM" }, StringSplitOptions.None);
        return parts[0] + "," + fields + " FROM " + parts[1];
    }
    public static Organisation[] GetOrganisationsOf(int patient_id, bool exclGroupOrg = true, bool exclClinics = false, bool exclAgedCareFacs = false, bool exclIns = true, bool exclExternal = false, string matchName = "", bool searchNameOnlyStartsWith = false)
    {
        DataTable tbl = GetDataTable_OrganisationsOf(patient_id, exclGroupOrg, exclClinics, exclAgedCareFacs, exclIns, exclExternal, matchName, searchNameOnlyStartsWith);
        Organisation[] list = new Organisation[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = OrganisationDB.Load(tbl.Rows[i], "", "organisation_entity_id", "organisation_is_deleted");
        return list;
    }


    public static Organisation GetMostRecentOrganisationOf(int patient_id)
    {
        string sql = JoinedSql(true) + " AND pat.patient_id = " + patient_id.ToString() + " ORDER BY r.register_patient_date_added DESC";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : OrganisationDB.Load(tbl.Rows[0], "", "organisation_entity_id", "organisation_is_deleted");
    }
    public static Hashtable GetMostRecentOrganisationOf(int[] patient_ids)
    {
        if (patient_ids == null || patient_ids.Length == 0)
            return new Hashtable();

        string sql = JoinedSql(false) + 
                    " AND pat.patient_id IN (" + string.Join(",", patient_ids) + ")" +
                    " AND r.register_patient_id = (SELECT TOP 1 register_patient_id FROM RegisterPatient WHERE patient_id = pat.patient_id ORDER BY register_patient_date_added DESC)";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int          ptID = Convert.ToInt32(tbl.Rows[i]["patient_id"]);
            Organisation org  = OrganisationDB.Load(tbl.Rows[i], "", "organisation_entity_id", "organisation_is_deleted");
            hash[ptID] = org;
        }

        return hash;
    }

    public static DataTable GetDataTable_PatientsOf(bool onlyLastSeenHere, int organistion_id, bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, bool show_only_is_gp_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchFirstname = "", bool searchFirstnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string searchEmail = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchInternalOrgs = "", string matchProviders = "", string matchReferrers = "", string matchReffererPerson = "", string matchReffererOrg = "", bool onlyDiabetics = false, bool onlyMedicareEPC = false, bool onlyDVAEPC = false)
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
                         rr.register_patient_id, rr.organisation_id, rr.patient_id, rr.register_patient_date_added, 
                         pa.person_id, pa.patient_date_added,pa.is_clinic_patient,pa.is_gp_patient,pa.is_deleted,pa.is_deceased,pa.is_deleted,
                         pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, 
                         pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                         pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                         " + PersonDB.GetFields("", "p") + @", p2.firstname AS added_by_firstname, 
                         t.title_id, t.descr

                        ,(SELECT COUNT(*) 
                            FROM   RegisterPatient INNER JOIN Organisation ON RegisterPatient.organisation_id = Organisation.organisation_id
                            WHERE  patient_id = pa.patient_id AND RegisterPatient.is_deleted = 0 AND Organisation.is_deleted = 0 " + (UserView.GetInstance().IsAgedCareView ? " AND Organisation.organisation_type_id IN (139,367,372) " : "") + (UserView.GetInstance().IsClinicView ? " AND Organisation.organisation_type_id = 218 " : "") + @"
                            ) as num_registered_orgs

                       FROM
                         RegisterPatient AS rr 
                         LEFT OUTER JOIN Patient pa  ON rr.patient_id = pa.patient_id 
                         LEFT OUTER JOIN Person  p   ON pa.person_id  = p.person_id
                         LEFT OUTER JOIN Person  p2  ON p2.person_id  = p.added_by
                         LEFT OUTER JOIN Title   t   ON t.title_id    = p.title_id
                       WHERE
                         rr.is_deleted = 0 AND rr.organisation_id = " + organistion_id.ToString() + @"

                         " + (onlyLastSeenHere ? " AND rr.register_patient_id = (SELECT TOP 1 register_patient_id FROM RegisterPatient LEFT JOIN Organisation ON RegisterPatient.organisation_id = Organisation.organisation_id WHERE patient_id = rr.patient_id AND RegisterPatient.is_deleted = 0 AND Organisation.is_deleted = 0 ORDER BY register_patient_id DESC)    " : "") + @"

                         " + (matchSurname.Length > 0 && !searchSurnameOnlyStartsWith ? " AND p.surname LIKE '%" + matchSurname + "%'" : "") + @"
                         " + (matchSurname.Length > 0 &&  searchSurnameOnlyStartsWith ? " AND p.surname LIKE '" + matchSurname + "%'" : "") + @"
                         " + (matchFirstname.Length > 0 && !searchFirstnameOnlyStartsWith ? " AND p.firstname LIKE '%" + matchFirstname + "%'" : "") + @"
                         " + (matchFirstname.Length > 0 &&  searchFirstnameOnlyStartsWith ? " AND p.firstname LIKE '"  + matchFirstname + "%'" : "") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || matchStreets   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.address_channel_id IN (" + matchStreets + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_group_id = 2  AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || searchEmail    == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_id       = 27 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND addr_line1 LIKE '" + searchEmail    + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchStreets   == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND (ad.street_name = '" + matchStreets + "' OR SOUNDEX(ad.street_name) = SOUNDEX('" + matchStreets + "'))) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_group_id = 2  AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || searchEmail    == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_id       = 27 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND addr_line1 LIKE '" + searchEmail    + "%'" + ") > 0 ") + @"
                         " + (matchMedicareCardNo.Length > 0 && !searchMedicareCardNoOnlyStartsWith ? " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '%" + matchMedicareCardNo + "%') > 0 " : "") + @"
                         " + (matchMedicareCardNo.Length > 0 && searchMedicareCardNoOnlyStartsWith ?  " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '" + matchMedicareCardNo + "%') > 0 " : "") + @"
                         " + (matchDOBDay    == -1 ? "" : " AND datepart(day,p.dob)   = " + matchDOBDay) + @"
                         " + (matchDOBMonth  == -1 ? "" : " AND datepart(month,p.dob) = " + matchDOBMonth) + @"
                         " + (matchDOBYear   == -1 ? "" : " AND datepart(year,p.dob)  = " + matchDOBYear) + @"
                         " + (show_deleted         ? "" : " AND pa.is_deleted = 0 ") + @"
                         " + (show_deceased        ? "" : " AND pa.is_deceased = 0 ") + @"
                         " + (!show_only_is_clinic_patient ? "" : " AND pa.is_clinic_patient    = 1 ") + @"
                         " + (!show_only_is_gp_patient ? "" : " AND pa.is_gp_patient    = 1 ") + @"
                         " + (matchInternalOrgs   == "" ? "" : " AND (SELECT COUNT(*) FROM RegisterPatient WHERE RegisterPatient.is_deleted = 0 AND RegisterPatient.patient_id=pa.patient_id AND RegisterPatient.organisation_id IN (" + matchInternalOrgs + ")) > 0 ") + @"
                         " + (matchProviders      == "" ? "" : " AND (SELECT COUNT(*) FROM Booking         WHERE (Booking.date_deleted IS NULL AND Booking.deleted_by IS NULL) AND Booking.patient_id = pa.patient_id AND Booking.provider IN (" + matchProviders + ")) > 0 ") + @"
                         " + (matchReferrers      == "" ? "" : " AND (SELECT COUNT(*) FROM PatientReferrer WHERE PatientReferrer.is_active = 1 AND patient_id=pa.patient_id AND register_referrer_id IN (" + matchReferrers + ")) > 0 ") + @"
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
    public static Patient[] GetPatientsOf(bool onlyLastSeenHere, int organistion_id, bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, bool show_only_is_gp_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchFirstname = "", bool searchFirstnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string searchEmail = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchInternalOrgs = "", string matchProviders = "", string matchReferrers = "", string matchReffererPerson = "", string matchReffererOrg = "", bool onlyDiabetics = false, bool onlyMedicareEPC = false, bool onlyDVAEPC = false)
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

        DataTable tbl = GetDataTable_PatientsOf(onlyLastSeenHere, organistion_id, show_deleted, show_deceased, show_only_is_clinic_patient, show_only_is_gp_patient, matchSurname, searchSurnameOnlyStartsWith, matchFirstname, searchFirstnameOnlyStartsWith, matchSuburbs, matchStreets, searchPhoneNbr, searchEmail, matchMedicareCardNo, searchMedicareCardNoOnlyStartsWith, matchDOBDay, matchDOBMonth, matchDOBYear, matchInternalOrgs, matchProviders, matchReferrers, matchReffererPerson, matchReffererOrg, onlyDiabetics, onlyMedicareEPC, onlyDVAEPC);
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = PatientDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i], "");
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
        }
        return list;
    }

    public static DataTable GetDataTable_PatientsOf(bool onlyLastSeenHere, string organistion_ids, bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, bool show_only_is_gp_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchFirstname = "", bool searchFirstnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string searchEmail = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchInternalOrgs = "", string matchProviders = "", string matchReferrers = "", string matchReffererPerson = "", string matchReffererOrg = "", bool onlyDiabetics = false, bool onlyMedicareEPC = false, bool onlyDVAEPC = false)
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

        if (organistion_ids == "")
            organistion_ids = "0";

        string sql = @"SELECT DISTINCT
                         --rr.register_patient_id, rr.organisation_id, rr.patient_id, rr.register_patient_date_added, 
                         pa.patient_id,pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased,pa.is_deleted,
                         pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, 
                         pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                         pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                         " + PersonDB.GetFields("", "p") + @", p2.firstname AS added_by_firstname, 
                         t.title_id, t.descr

                        ,(SELECT COUNT(*) 
                            FROM   RegisterPatient INNER JOIN Organisation ON RegisterPatient.organisation_id = Organisation.organisation_id
                            WHERE  patient_id = pa.patient_id AND RegisterPatient.is_deleted = 0 AND Organisation.is_deleted = 0 " + (UserView.GetInstance().IsAgedCareView ? " AND Organisation.organisation_type_id IN (139,367,372) " : "") + (UserView.GetInstance().IsClinicView ? " AND Organisation.organisation_type_id = 218 " : "") + @"
                            ) as num_registered_orgs

                       FROM
                         RegisterPatient AS rr 
                         LEFT OUTER JOIN Patient pa  ON rr.patient_id = pa.patient_id 
                         LEFT OUTER JOIN Person  p   ON pa.person_id = p.person_id
                         LEFT OUTER JOIN Person  p2  ON p2.person_id  = p.added_by
                         LEFT OUTER JOIN Title   t   ON t.title_id    = p.title_id
                       WHERE
                         rr.is_deleted = 0 AND rr.organisation_id IN (" + organistion_ids + @")
                         " + (matchSurname.Length > 0 && !searchSurnameOnlyStartsWith ? " AND p.surname LIKE '%" + matchSurname + "%'" : "") + @"
                         " + (matchSurname.Length > 0 &&  searchSurnameOnlyStartsWith ? " AND p.surname LIKE '"  + matchSurname + "%'" : "") + @"
                         " + (matchFirstname.Length > 0 && !searchFirstnameOnlyStartsWith ? " AND p.firstname LIKE '%" + matchFirstname + "%'" : "") + @"
                         " + (matchFirstname.Length > 0 &&  searchFirstnameOnlyStartsWith ? " AND p.firstname LIKE '"  + matchFirstname + "%'" : "") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || matchStreets   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.address_channel_id IN (" + matchStreets + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || searchEmail    == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND addr_line1                        LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchStreets   == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND (ad.street_name = '" + matchStreets + "' OR SOUNDEX(ad.street_name) = SOUNDEX('" + matchStreets + "'))) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || searchEmail    == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND addr_line1                        LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (matchMedicareCardNo.Length > 0 && !searchMedicareCardNoOnlyStartsWith ? " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '%" + matchMedicareCardNo + "%') > 0 " : "") + @"
                         " + (matchMedicareCardNo.Length > 0 && searchMedicareCardNoOnlyStartsWith ?  " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '" + matchMedicareCardNo + "%') > 0 " : "") + @"
                         " + (matchDOBDay    == -1 ? "" : " AND datepart(day,p.dob)   = " + matchDOBDay)   + @"
                         " + (matchDOBMonth  == -1 ? "" : " AND datepart(month,p.dob) = " + matchDOBMonth) + @"
                         " + (matchDOBYear   == -1 ? "" : " AND datepart(year,p.dob)  = " + matchDOBYear)  + @"
                         " + (show_deleted         ? "" : " AND pa.is_deleted = 0 ") + @"
                         " + (show_deceased        ? "" : " AND pa.is_deceased = 0 ") + @"
                         " + (!show_only_is_clinic_patient ? "" : " AND pa.is_clinic_patient   = 1 ") + @"
                         " + (!show_only_is_gp_patient ? "" : " AND pa.is_gp_patient   = 1 ") + @"
                         " + (matchInternalOrgs   == "" ? "" : " AND (SELECT COUNT(*) FROM RegisterPatient WHERE RegisterPatient.is_deleted = 0 AND RegisterPatient.patient_id=pa.patient_id AND RegisterPatient.organisation_id IN (" + matchInternalOrgs + ")) > 0 ") + @"
                         " + (matchProviders      == "" ? "" : " AND (SELECT COUNT(*) FROM Booking         WHERE (Booking.date_deleted IS NULL AND Booking.deleted_by IS NULL) AND Booking.patient_id = pa.patient_id AND Booking.provider IN (" + matchProviders + ")) > 0 ") + @"
                         " + (matchReferrers      == "" ? "" : " AND (SELECT COUNT(*) FROM PatientReferrer WHERE PatientReferrer.is_active = 1 AND patient_id=pa.patient_id AND register_referrer_id IN (" + matchReferrers + ")) > 0 ") + @"
                         " + (matchReffererPerson == "" ? "" : " AND (SELECT COUNT(*) FROM PatientReferrer LEFT JOIN RegisterReferrer ON PatientReferrer.register_referrer_id = RegisterReferrer.register_referrer_id WHERE PatientReferrer.is_active  = 1 AND RegisterReferrer.is_deleted = 0 AND patient_id=pa.patient_id AND RegisterReferrer.referrer_id     IN (" + matchReffererPerson + ")) > 0 ") + @"
                         " + (matchReffererOrg    == "" ? "" : " AND (SELECT COUNT(*) FROM PatientReferrer LEFT JOIN RegisterReferrer ON PatientReferrer.register_referrer_id = RegisterReferrer.register_referrer_id WHERE PatientReferrer.is_active  = 1 AND RegisterReferrer.is_deleted = 0 AND patient_id=pa.patient_id AND RegisterReferrer.organisation_id IN (" + matchReffererOrg    + ")) > 0 ") + @"
                         " + (!onlyDiabetics            ? "" : " AND pa.is_diabetic = 1 ") + @"
                         " + (!onlyMedicareEPC ? "" :
                                 @" AND (SELECT COALESCE(SUM(num_services_remaining),0) 
	                                     FROM   HealthCardEPCRemaining epcRemaining LEFT JOIN HealthCard AS hc ON epcRemaining.health_card_id = hc.health_card_id
	                                     WHERE  hc.is_active = 1 AND hc.patient_id = pa.patient_id AND hc.organisation_id = -1 AND hc.date_referral_signed > DATEADD(year,-1,GETDATE())) > 0 ") + @"
                         " + (!onlyDVAEPC ? "" : " AND (SELECT COUNT(*) FROM HealthCard AS hc WHERE is_active = 1 AND hc.patient_id = pa.patient_id AND organisation_id = -2 AND date_referral_signed > DATEADD(year,-1,GETDATE())) > 0 ") + @"
                       ORDER BY 
                         p.surname, p.firstname, p.middlename";

        return !onlyMedicareEPC ? DBBase.ExecuteQuery(sql).Tables[0] : PatientDB.RemooveIfMedicareYearQuotaUsed(DBBase.ExecuteQuery(sql).Tables[0]);
    }
    public static Patient[] GetPatientsOf(bool onlyLastSeenHere, string organistion_ids, bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchFirstname = "", bool searchFirstnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string searchEmail = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchInternalOrgs = "", string matchProviders = "", string matchReferrers = "", string matchReffererPerson = "", string matchReffererOrg = "", bool onlyDiabetics = false, bool onlyMedicareEPC = false, bool onlyDVAEPC = false)
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

        DataTable tbl = GetDataTable_PatientsOf(onlyLastSeenHere, organistion_ids, show_deleted, show_deceased, show_only_is_clinic_patient, show_only_is_clinic_patient, matchSurname, searchSurnameOnlyStartsWith, matchFirstname, searchFirstnameOnlyStartsWith, matchSuburbs, matchStreets, searchPhoneNbr, searchEmail, matchMedicareCardNo, searchMedicareCardNoOnlyStartsWith, matchDOBDay, matchDOBMonth, matchDOBYear, matchInternalOrgs, matchProviders, matchReferrers, matchReffererPerson, matchReffererOrg, onlyDiabetics, onlyMedicareEPC, onlyDVAEPC);
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = PatientDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i], "");
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
        }
        return list;
    }

    public static DataTable GetDataTable_PatientsOfOrgGroupType(bool onlyLastSeenHere, string organistion_group_type_ids, bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, bool show_only_is_gp_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchFirstname = "", bool searchFirstnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string searchEmail = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchInternalOrgs = "", string matchProviders = "", string matchReferrers = "", string matchReffererPerson = "", string matchReffererOrg = "", bool onlyDiabetics = false, bool onlyMedicareEPC = false, bool onlyDVAEPC = false)
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

        if (organistion_group_type_ids == "")
            organistion_group_type_ids = "0";

        string sql = @"SELECT DISTINCT
                         --rr.register_patient_id, rr.organisation_id, rr.patient_id, rr.register_patient_date_added, 
                         pa.patient_id,pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased,pa.is_deleted,
                         pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, 
                         pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                         pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                         " + PersonDB.GetFields("", "p") + @", p2.firstname AS added_by_firstname, 
                         t.title_id, t.descr

                        ,(SELECT COUNT(*) 
                            FROM   RegisterPatient INNER JOIN Organisation ON RegisterPatient.organisation_id = Organisation.organisation_id
                            WHERE  patient_id = pa.patient_id AND RegisterPatient.is_deleted = 0 AND Organisation.is_deleted = 0 " + (UserView.GetInstance().IsAgedCareView ? " AND Organisation.organisation_type_id IN (139,367,372) " : "") + (UserView.GetInstance().IsClinicView ? " AND Organisation.organisation_type_id = 218 " : "") + @"
                            ) as num_registered_orgs

                       FROM
                         RegisterPatient AS rr 
                         LEFT OUTER JOIN Patient           pa  ON rr.patient_id = pa.patient_id 
                         LEFT OUTER JOIN Person            p   ON pa.person_id = p.person_id
                         LEFT OUTER JOIN Person            p2  ON p2.person_id  = p.added_by
                         LEFT OUTER JOIN Title             t   ON t.title_id    = p.title_id
                         LEFT OUTER JOIN Organisation      o   ON o.organisation_id = rr.organisation_id 
                         LEFT OUTER JOIN OrganisationType  ot  ON ot.organisation_type_id = o.organisation_type_id
                       WHERE
                         rr.is_deleted = 0 AND ot.organisation_type_group_id IN (" + organistion_group_type_ids + @")
                         " + (matchSurname.Length > 0 && !searchSurnameOnlyStartsWith ? " AND p.surname LIKE '%" + matchSurname + "%'" : "") + @"
                         " + (matchSurname.Length > 0 &&  searchSurnameOnlyStartsWith ? " AND p.surname LIKE '"  + matchSurname + "%'" : "") + @"
                         " + (matchFirstname.Length > 0 && !searchFirstnameOnlyStartsWith ? " AND p.firstname LIKE '%" + matchFirstname + "%'" : "") + @"
                         " + (matchFirstname.Length > 0 &&  searchFirstnameOnlyStartsWith ? " AND p.firstname LIKE '"  + matchFirstname + "%'" : "") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || matchStreets   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.address_channel_id IN (" + matchStreets + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_group_id = 2 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || searchEmail    == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_id       = 27 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND addr_line1                       LIKE '" + searchEmail    + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchStreets   == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND (ad.street_name = '" + matchStreets + "' OR SOUNDEX(ad.street_name) = SOUNDEX('" + matchStreets + "'))) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_group_id = 2 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || searchEmail    == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id   WHERE ContactType.contact_type_id       = 27 AND entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND addr_line1                       LIKE '" + searchEmail    + "%'" + ") > 0 ") + @"
                         " + (matchMedicareCardNo.Length > 0 && !searchMedicareCardNoOnlyStartsWith ? " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '%" + matchMedicareCardNo + "%') > 0 " : "") + @"
                         " + (matchMedicareCardNo.Length > 0 && searchMedicareCardNoOnlyStartsWith ? " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '" + matchMedicareCardNo + "%') > 0 " : "") + @"
                         " + (matchDOBDay   == -1 ? "" : " AND datepart(day,p.dob)   = " + matchDOBDay) + @"
                         " + (matchDOBMonth == -1 ? "" : " AND datepart(month,p.dob) = " + matchDOBMonth) + @"
                         " + (matchDOBYear  == -1 ? "" : " AND datepart(year,p.dob)  = " + matchDOBYear) + @"
                         " + (show_deleted        ? "" : " AND pa.is_deleted = 0 ") + @"
                         " + (show_deceased       ? "" : " AND pa.is_deceased = 0 ") + @"
                         " + (!show_only_is_clinic_patient ? "" : " AND pa.is_clinic_patient   = 1 ") + @"
                         " + (!show_only_is_gp_patient ? "" : " AND pa.is_gp_patient   = 1 ") + @"
                         " + (matchInternalOrgs   == "" ? "" : " AND (SELECT COUNT(*) FROM RegisterPatient WHERE RegisterPatient.is_deleted = 0 AND RegisterPatient.patient_id=pa.patient_id AND RegisterPatient.organisation_id IN (" + matchInternalOrgs + ")) > 0 ") + @"
                         " + (matchProviders      == "" ? "" : " AND (SELECT COUNT(*) FROM Booking         WHERE (Booking.date_deleted IS NULL AND Booking.deleted_by IS NULL) AND Booking.patient_id = pa.patient_id AND Booking.provider IN (" + matchProviders + ")) > 0 ") + @"
                         " + (matchReferrers      == "" ? "" : " AND (SELECT COUNT(*) FROM PatientReferrer WHERE PatientReferrer.is_active = 1 AND patient_id=pa.patient_id AND register_referrer_id IN (" + matchReferrers + ")) > 0 ") + @"
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
    public static Patient[] GetPatientsOfGroupType(bool onlyLastSeenHere, string organistion_group_type_ids, bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, bool show_only_is_gp_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchFirstname = "", bool searchFirstnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string searchEmail = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchInternalOrgs = "", string matchProviders = "", string matchReferrers = "", string matchReffererPerson = "", string matchReffererOrg = "", bool onlyDiabetics = false, bool onlyMedicareEPC = false, bool onlyDVAEPC = false)
    {
        DataTable tbl = GetDataTable_PatientsOfOrgGroupType(onlyLastSeenHere, organistion_group_type_ids, show_deleted, show_deceased, show_only_is_clinic_patient, show_only_is_gp_patient, matchSurname, searchSurnameOnlyStartsWith, matchFirstname,  searchFirstnameOnlyStartsWith, matchSuburbs, matchStreets, searchPhoneNbr, searchEmail, matchMedicareCardNo, searchMedicareCardNoOnlyStartsWith, matchDOBDay, matchDOBMonth, matchDOBYear, matchInternalOrgs, matchProviders, matchReferrers, matchReffererPerson, matchReffererOrg, onlyDiabetics, onlyMedicareEPC, onlyDVAEPC);
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = PatientDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i], "");
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
        }
        return list;
    }

    public static bool IsAgedCarePatient(int patient_id)
    {
        string sql = @"SELECT 
                         COUNT(rr.patient_id)
                       FROM
                         RegisterPatient AS rr 
                         LEFT OUTER JOIN Organisation      o   ON o.organisation_id = rr.organisation_id 
                         LEFT OUTER JOIN OrganisationType  ot  ON ot.organisation_type_id = o.organisation_type_id
                       WHERE
                         rr.patient_id = " + patient_id + " AND rr.is_deleted = 0 AND ot.organisation_type_group_id IN (6) ";

        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }


    public static DataTable GetDataTable_PatientsWithNoOrg()
    {
        string sql = @"SELECT DISTINCT
                         pat.patient_id,
                         pat.person_id, pat.patient_date_added, pat.is_clinic_patient, pat.is_gp_patient, pat.is_deleted, pat.is_deceased,
                         pat.flashing_text, pat.flashing_text_added_by, pat.flashing_text_last_modified_date, 
                         pat.private_health_fund, pat.concession_card_number, pat.concession_card_expiry_date, pat.is_diabetic, pat.is_member_diabetes_australia, pat.diabetic_assessment_review_date, pat.ac_inv_offering_id, pat.ac_pat_offering_id, pat.login, pat.pwd, pat.is_company, pat.abn, 
                         pat.next_of_kin_name, pat.next_of_kin_relation, pat.next_of_kin_contact_info,
                         " + PersonDB.GetFields("", "p").Replace("p.entity_id", "p.entity_id as person_entity_id") + @", 
                         t.title_id as t_title_id, t.descr as t_descr
                       FROM
                         Patient AS pat
                         LEFT OUTER JOIN Person p ON pat.person_id = p.person_id
                         LEFT OUTER JOIN Title  t ON t.title_id = p.title_id
                       WHERE
                         pat.is_deleted = 0 AND 
                        ((SELECT COUNT(*) FROM RegisterPatient AS registerpatient WHERE (patient_id = pat.patient_id AND is_deleted = 0)) = 0)
                       ORDER BY 
                         p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Patient[] GetPatientsWithNoOrg()
    {
        DataTable tbl = GetDataTable_PatientsWithNoOrg();
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = PatientDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i], "", "person_entity_id");
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "t_title_id", "t_descr");
        }
        return list;
    }


    public static RegisterPatient LoadAll(DataRow row)
    {
        RegisterPatient rp = Load(row);
        rp.Organisation = OrganisationDB.Load(row, "", "organisation_entity_id", "organisation_is_deleted");
        rp.Patient = PatientDB.Load(row);
        rp.Patient.Person = PersonDB.Load(row, "", "person_entity_id");
        rp.Patient.Person.Title = IDandDescrDB.Load(row, "title_id", "descr");
        return rp;
    }

    public static RegisterPatient Load(DataRow row)
    {
        return new RegisterPatient(
            Convert.ToInt32(row["register_patient_id"]),
            Convert.ToInt32(row["organisation_id"]),
            Convert.ToInt32(row["patient_id"]),
            Convert.ToDateTime(row["register_patient_date_added"])
        );
    }

}