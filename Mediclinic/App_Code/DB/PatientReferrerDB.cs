using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class PatientReferrerDB
{

    public static void Delete(int patient_referrer_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM PatientReferrer WHERE patient_referrer_id = " + patient_referrer_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void DeleteActiveEPCRef(int register_referrer_id, int patient_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM PatientReferrer WHERE is_active = 1 AND register_referrer_id = " + register_referrer_id.ToString() + " AND patient_id = " + patient_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void DeleteActiveNonEPCRef(int organisation_id, int patient_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM PatientReferrer WHERE is_active = 1 AND organisation_id = " + organisation_id.ToString() + " AND patient_id = " + patient_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static int Insert(int patient_id, int register_referrer_id, int organisation_id, bool is_debtor)
    {
        if (register_referrer_id == -1 && organisation_id == 0)
            throw new Exception("Can not have both reg-referrer and org null.");

        if (register_referrer_id != -1)
        {
            string nRowsSql = "SELECT COUNT(*) FROM PatientReferrer WHERE is_active = 1 AND register_referrer_id IS NOT NULL AND patient_id = " + patient_id.ToString();
            if (Convert.ToInt32(DBBase.ExecuteSingleResult(nRowsSql)) > 0)
                throw new UniqueConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        }

        string sql = "INSERT INTO PatientReferrer (patient_id,register_referrer_id, organisation_id, patient_referrer_date_added,is_debtor) VALUES (" + "" + patient_id + "," + "" + (register_referrer_id == -1 ? "NULL" : register_referrer_id.ToString()) + "," + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + (is_debtor ? "1" : "0") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int patient_referrer_id, int patient_id, int register_referrer_id, int organisation_id, bool is_debtor)
    {
        string sql = "UPDATE PatientReferrer SET patient_id = " + patient_id + ",register_referrer_id = " + (register_referrer_id == -1 ? "NULL" : register_referrer_id.ToString()) + ",organisation_id = " + (organisation_id == 0 ? "NULL" : organisation_id.ToString()) + ",is_debtor = " + (is_debtor ? "1" : "0") + " WHERE patient_referrer_id = " + patient_referrer_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateMoveAllPatients(int register_referrer_id_from, int register_referrer_id_to)
    {
        string sql = "UPDATE PatientReferrer SET register_referrer_id = " + register_referrer_id_to + " WHERE register_referrer_id = " + register_referrer_id_from;
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateSetEPCRefInactive(int register_referrer_id, int patient_id)
    {
        string sql = "UPDATE PatientReferrer SET is_active = 0 WHERE register_referrer_id = " + register_referrer_id.ToString() + " AND patient_id = " + patient_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateSetInactive(int patient_referrer_id)
    {
        string sql = "UPDATE PatientReferrer SET is_active = 0 WHERE patient_referrer_id = " + patient_referrer_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable(bool onlyActive, bool show_deleted = false, bool show_deceased = false)
    {
        string sql = JoinedSQL() + 
                         (onlyActive    ? " AND pr.is_active = 1 " : "") + 
                         (show_deleted  ? "" : " AND pat.is_deleted = 0 ") +
                         (show_deceased ? "" : " AND pat.is_deceased = 0 ");
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    // hashtable: register_referrer_id => PatientReferrer[]
    public static Hashtable GetHashtableByRegRef(bool onlyActive, bool show_deleted = false, bool show_deceased = false)
    {
        Hashtable hash = new Hashtable();

        DataTable dt = GetDataTable(onlyActive, show_deleted, show_deceased);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            PatientReferrer pr = LoadAll(dt.Rows[i]);

            if (hash[pr.RegisterReferrer.RegisterReferrerID] == null)
                hash[pr.RegisterReferrer.RegisterReferrerID] = new ArrayList();
            ((ArrayList)hash[pr.RegisterReferrer.RegisterReferrerID]).Add(pr);
        }

        // convert from arraylists to arrays 
        ArrayList keys = new ArrayList();
        foreach (System.Collections.DictionaryEntry de in hash)
            keys.Add(de.Key);
        foreach (int key in keys)
            hash[key] = (PatientReferrer[])((ArrayList)hash[key]).ToArray(typeof(PatientReferrer)); ;

        return hash;
    }

    public static PatientReferrer GetByID(int patient_referrer_id)
    {
        //string sql = "SELECT patient_referrer_id,patient_id,register_referrer_id,patient_referrer_date_added,is_debtor,is_active FROM PatientReferrer WHERE patient_referrer_id = " + patient_referrer_id.ToString();
        string sql = JoinedSQL() + " AND patient_referrer_id = " + patient_referrer_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        if (tbl.Rows.Count == 0)
            return null;
        else
        {
            PatientReferrer pr = Load(tbl.Rows[0]);
            pr.Patient = PatientDB.Load(tbl.Rows[0]);
            pr.Patient.Person = PersonDB.Load(tbl.Rows[0], "patient_person_");
            pr.Patient.Person.Title = IDandDescrDB.Load(tbl.Rows[0], "patient_person_title_title_id", "patient_person_title_descr");

            if (tbl.Rows[0]["pr_register_referrer_id"] != DBNull.Value)
            {
                pr.RegisterReferrer = RegisterReferrerDB.Load(tbl.Rows[0]);
                pr.RegisterReferrer.Referrer = ReferrerDB.Load(tbl.Rows[0]);
                pr.RegisterReferrer.Referrer.Person = PersonDB.Load(tbl.Rows[0], "referrer_person_");
                pr.RegisterReferrer.Referrer.Person.Title = IDandDescrDB.Load(tbl.Rows[0], "referrer_person_title_title_id", "referrer_person_title_descr");
                pr.RegisterReferrer.Organisation = OrganisationDB.Load(tbl.Rows[0], "organisation_");
            }
            if (tbl.Rows[0]["pr_organisation_id"] != DBNull.Value)
            {
                pr.Organisation = OrganisationDB.Load(tbl.Rows[0], "nonepcorg_");
            }

            return pr;
        }
    }


    private static string JoinedSQL()
    {
        string sql = @"
        SELECT
            pr.patient_referrer_id as pr_patient_referrer_id, pr.patient_id as pr_patient_id, pr.register_referrer_id as pr_register_referrer_id, pr.organisation_id as pr_organisation_id, pr.patient_referrer_date_added as pr_patient_referrer_date_added, pr.is_debtor as pr_is_debtor, pr.is_active as pr_is_active, 
            pat.patient_id as patient_patient_id,pat.person_id as patient_person_id, pat.patient_date_added as patient_patient_date_added, pat.is_clinic_patient as patient_is_clinic_patient, pat.is_gp_patient as patient_is_gp_patient, pat.is_deleted as patient_is_deleted, pat.is_deceased as patient_is_deceased, 
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

            nonepcorg.entity_id as nonepcorg_entity_id, nonepcorg.parent_organisation_id as nonepcorg_parent_organisation_id, nonepcorg.use_parent_offernig_prices as nonepcorg_use_parent_offernig_prices, nonepcorg.organisation_type_id as nonepcorg_organisation_type_id, nonepcorg.organisation_customer_type_id as nonepcorg_organisation_customer_type_id,nonepcorg.name as nonepcorg_name, nonepcorg.acn as nonepcorg_acn, nonepcorg.abn as nonepcorg_abn, nonepcorg.organisation_date_added as nonepcorg_organisation_date_added, 
            nonepcorg.organisation_id as nonepcorg_organisation_id, nonepcorg.organisation_date_modified as nonepcorg_organisation_date_modified, nonepcorg.is_debtor as nonepcorg_is_debtor, nonepcorg.is_creditor as nonepcorg_is_creditor, nonepcorg.bpay_account as nonepcorg_bpay_account, nonepcorg.weeks_per_service_cycle as nonepcorg_weeks_per_service_cycle, nonepcorg.start_date as nonepcorg_start_date, 
            nonepcorg.end_date as nonepcorg_end_date, nonepcorg.comment as nonepcorg_comment, nonepcorg.free_services as nonepcorg_free_services, nonepcorg.excl_sun as nonepcorg_excl_sun, nonepcorg.excl_mon as nonepcorg_excl_mon, nonepcorg.excl_tue as nonepcorg_excl_tue, nonepcorg.excl_wed as nonepcorg_excl_wed, nonepcorg.excl_thu as nonepcorg_excl_thu, nonepcorg.excl_fri as nonepcorg_excl_fri, nonepcorg.excl_sat as nonepcorg_excl_sat, 
            nonepcorg.sun_start_time as nonepcorg_sun_start_time, nonepcorg.sun_end_time as nonepcorg_sun_end_time, nonepcorg.mon_start_time as nonepcorg_mon_start_time, nonepcorg.mon_end_time as nonepcorg_mon_end_time, nonepcorg.tue_start_time as nonepcorg_tue_start_time, nonepcorg.tue_end_time as nonepcorg_tue_end_time, nonepcorg.wed_start_time as nonepcorg_wed_start_time, nonepcorg.wed_end_time as nonepcorg_wed_end_time, 
            nonepcorg.thu_start_time as nonepcorg_thu_start_time, nonepcorg.thu_end_time as nonepcorg_thu_end_time, nonepcorg.fri_start_time as nonepcorg_fri_start_time, nonepcorg.fri_end_time as nonepcorg_fri_end_time, nonepcorg.sat_start_time as nonepcorg_sat_start_time, nonepcorg.sat_end_time as nonepcorg_sat_end_time, 
            nonepcorg.sun_lunch_start_time as nonepcorg_sun_lunch_start_time, nonepcorg.sun_lunch_end_time as nonepcorg_sun_lunch_end_time, nonepcorg.mon_lunch_start_time as nonepcorg_mon_lunch_start_time, nonepcorg.mon_lunch_end_time as nonepcorg_mon_lunch_end_time, nonepcorg.tue_lunch_start_time as nonepcorg_tue_lunch_start_time, nonepcorg.tue_lunch_end_time as nonepcorg_tue_lunch_end_time, nonepcorg.wed_lunch_start_time as nonepcorg_wed_lunch_start_time, nonepcorg.wed_lunch_end_time as nonepcorg_wed_lunch_end_time, 
            nonepcorg.thu_lunch_start_time as nonepcorg_thu_lunch_start_time, nonepcorg.thu_lunch_end_time as nonepcorg_thu_lunch_end_time, nonepcorg.fri_lunch_start_time as nonepcorg_fri_lunch_start_time, nonepcorg.fri_lunch_end_time as nonepcorg_fri_lunch_end_time, nonepcorg.sat_lunch_start_time as nonepcorg_sat_lunch_start_time, nonepcorg.sat_lunch_end_time as nonepcorg_sat_lunch_end_time, 
            nonepcorg.last_batch_run as nonepcorg_last_batch_run, nonepcorg.is_deleted as nonepcorg_is_deleted

        FROM
            PatientReferrer AS pr 
            INNER JOIN Patient AS pat ON pat.patient_id = pr.patient_id 
            LEFT OUTER JOIN RegisterReferrer AS regref ON regref.register_referrer_id = pr.register_referrer_id 
            LEFT OUTER JOIN Referrer AS ref ON ref.referrer_id = regref.referrer_id 
            LEFT OUTER JOIN Organisation AS org ON org.organisation_id = regref.organisation_id
            INNER JOIN Person AS patient_person  ON patient_person.person_id  = pat.person_id
            INNER JOIN Title  AS patient_person_title ON patient_person_title.title_id = patient_person.title_id
            INNER JOIN Person AS referrer_person ON referrer_person.person_id = ref.person_id
            INNER JOIN Title  AS referrer_person_title ON referrer_person_title.title_id = referrer_person.title_id

            LEFT OUTER JOIN Organisation AS nonepcorg ON nonepcorg.organisation_id = pr.organisation_id

        WHERE
            1 = 1 "; // is_active = 1";

        return sql;
    }


    // used when adding new EPC
    //
    // often the user will add a new referrer and THEN add a new EPC card
    // so we need to get the referrer that was in the system yesterday to send last EPC letters.
    public static PatientReferrer GetMostRecentlyAddedBeforeToday(int patient_id, PatientReferrer[] patientReferrers)
    {
        PatientReferrer patRefMostRecentBeforeToday = null;

        if (patientReferrers.Length > 0)
        {
            for (int i = patientReferrers.Length - 1; i >= 0; i--)
            {
                //if (patientReferrers[i].PatientReferrerDateAdded >= DateTime.Today)
                //    continue;

                if (patRefMostRecentBeforeToday == null || patientReferrers[i].PatientReferrerDateAdded > patRefMostRecentBeforeToday.PatientReferrerDateAdded)
                    patRefMostRecentBeforeToday = patientReferrers[i];

                else if (patientReferrers[i].PatientReferrerDateAdded == patRefMostRecentBeforeToday.PatientReferrerDateAdded &&
                         patientReferrers[i].PatientReferrerID        >  patRefMostRecentBeforeToday.PatientReferrerID)
                    patRefMostRecentBeforeToday = patientReferrers[i];
            }
        }

        return patRefMostRecentBeforeToday;
    }


    public static DataTable GetDataTable_EPCReferrersOf(int patient_id)
    {
        return _GetDataTable_EPCReferrersOf(patient_id, false);
    }
    public static DataTable GetDataTable_ActiveEPCReferrersOf(int patient_id)
    {
        return _GetDataTable_EPCReferrersOf(patient_id, true);
    }
    private static DataTable _GetDataTable_EPCReferrersOf(int patient_id, bool onlyActive)
    {
        string sql = JoinedSQL() + @" 
                        AND pat.patient_id = " + patient_id.ToString() + (onlyActive ? " AND pr.is_active = 1 " : "") + @"
                        ORDER BY pr.patient_referrer_date_added";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static RegisterReferrer[] GetEPCReferrersOf(int patient_id)
    {
        return _GetEPCReferrersOf(patient_id, false);
    }
    public static RegisterReferrer[] GetActiveEPCReferrersOf(int patient_id)
    {
        return _GetEPCReferrersOf(patient_id, true);
    }
    private static RegisterReferrer[] _GetEPCReferrersOf(int patient_id, bool onlyActive)
    {
        DataTable tbl = onlyActive ? GetDataTable_ActiveEPCReferrersOf(patient_id) : GetDataTable_EPCReferrersOf(patient_id);
        RegisterReferrer[] list = new RegisterReferrer[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            RegisterReferrer rr = RegisterReferrerDB.Load(tbl.Rows[i], "regref_");
            rr.Referrer = ReferrerDB.Load(tbl.Rows[i], "referrer_");
            rr.Referrer.Person = PersonDB.Load(tbl.Rows[i], "referrer_person_");
            rr.Referrer.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "referrer_person_title_title_id", "referrer_person_title_descr");
            rr.Organisation = OrganisationDB.Load(tbl.Rows[i], "organisation_");
            list[i] = rr;
        }
        return list;
    }

    // hashtable: patient_id => RegisterReferrer[]
    public static Hashtable GetEPCReferrersOf(int[] patient_ids, bool onlyActive)
    {
        string sql = JoinedSQL() + @" 
                        AND " + (patient_ids != null && patient_ids.Length > 0 ? " pat.patient_id IN (" + string.Join(",",patient_ids) + @")" : "1 <> 1" ) + @"
                        " + (onlyActive ? " AND pr.is_active = 1 " : "") + @"
                        ORDER BY patient_referrer_date_added";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            RegisterReferrer rr = RegisterReferrerDB.Load(tbl.Rows[i], "regref_");
            rr.Referrer = ReferrerDB.Load(tbl.Rows[i], "referrer_");
            rr.Referrer.Person = PersonDB.Load(tbl.Rows[i], "referrer_person_");
            rr.Referrer.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "referrer_person_title_title_id", "referrer_person_title_descr");
            rr.Organisation = OrganisationDB.Load(tbl.Rows[i], "organisation_");

            int patient_id = Convert.ToInt32(tbl.Rows[i]["pr_patient_id"]);
            if (hash[patient_id] == null)
                hash[patient_id] = new System.Collections.ArrayList();
            ((System.Collections.ArrayList)hash[patient_id]).Add(rr);
        }


        // convert from arraylists to arrays 
        ArrayList keys = new ArrayList();
        foreach (System.Collections.DictionaryEntry de in hash)
            keys.Add(de.Key);
        foreach (int key in keys)
            hash[key] = (RegisterReferrer[])((ArrayList)hash[key]).ToArray(typeof(RegisterReferrer)); ;

        return hash;
    }


    public static PatientReferrer[] GetEPCPatientReferrersOf(int patient_id)
    {
        return _GetEPCPatientReferrersOf(patient_id, false);
    }
    public static PatientReferrer[] GetActiveEPCPatientReferrersOf(int patient_id)
    {
        return _GetEPCPatientReferrersOf(patient_id, true);
    }
    private static PatientReferrer[] _GetEPCPatientReferrersOf(int patient_id, bool onlyActive)
    {
        DataTable tbl = onlyActive ? GetDataTable_ActiveEPCReferrersOf(patient_id) : GetDataTable_EPCReferrersOf(patient_id);
        PatientReferrer[] list = new PatientReferrer[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            PatientReferrer pr = Load(tbl.Rows[i], "pr_");
            pr.RegisterReferrer = RegisterReferrerDB.Load(tbl.Rows[i], "regref_");
            pr.RegisterReferrer.Referrer = ReferrerDB.Load(tbl.Rows[i], "referrer_");
            pr.RegisterReferrer.Referrer.Person = PersonDB.Load(tbl.Rows[i], "referrer_person_");
            pr.RegisterReferrer.Referrer.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "referrer_person_title_title_id", "referrer_person_title_descr");
            if (tbl.Rows[i]["organisation_organisation_id"] != DBNull.Value)
                pr.RegisterReferrer.Organisation = OrganisationDB.Load(tbl.Rows[i], "organisation_");
            pr.Patient = PatientDB.Load(tbl.Rows[i], "patient_");
            pr.Patient.Person = PersonDB.Load(tbl.Rows[i], "patient_person_");
            pr.Patient.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "patient_person_title_title_id", "patient_person_title_descr");
            list[i] = pr;
        }
        return list;
    }


    public static DataTable GetDataTable_PatientsOf(int register_referrer_id, bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, bool show_only_is_gp_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchReferrers = "")
    {
        return _GetDataTable_PatientsOf(register_referrer_id, false, show_deleted, show_deceased, show_only_is_clinic_patient, show_only_is_gp_patient, matchSurname, searchSurnameOnlyStartsWith, matchSuburbs, matchStreets, searchPhoneNbr, matchMedicareCardNo, searchMedicareCardNoOnlyStartsWith, matchDOBDay, matchDOBMonth, matchDOBYear, matchReferrers);
    }
    public static DataTable GetDataTable_ActivePatientsOf(int register_referrer_id, bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, bool show_only_is_gp_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchReferrers = "")
    {
        return _GetDataTable_PatientsOf(register_referrer_id, true, show_deleted, show_deceased, show_only_is_clinic_patient, show_only_is_gp_patient, matchSurname, searchSurnameOnlyStartsWith, matchSuburbs, matchStreets, searchPhoneNbr, matchMedicareCardNo, searchMedicareCardNoOnlyStartsWith, matchDOBDay, matchDOBMonth, matchDOBYear, matchReferrers);
    }

    private static DataTable _GetDataTable_PatientsOf(int register_referrer_id, bool onlyActive, bool show_deleted = false, bool show_deceased = false, bool show_only_is_clinic_patient = false, bool show_only_is_gp_patient = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchSuburbs = "", string matchStreets = "", string searchPhoneNbr = "", string matchMedicareCardNo = "", bool searchMedicareCardNoOnlyStartsWith = false, int matchDOBDay = -1, int matchDOBMonth = -1, int matchDOBYear = -1, string matchReferrers = "")
    {
        matchSurname         = matchSurname.Replace("'", "''");
        matchSuburbs         = matchSuburbs.Replace("'", "''");
        matchStreets         = matchStreets.Replace("'", "''");
        searchPhoneNbr       = searchPhoneNbr.Replace("'", "''");
        matchMedicareCardNo  = matchMedicareCardNo.Replace("'", "''");
        matchReferrers       = matchReferrers.Replace("'", "''");

        string sql = @"SELECT DISTINCT
                         pa.patient_id,pa.person_id, pa.patient_date_added, pa.is_clinic_patient, pa.is_gp_patient, pa.is_deleted, pa.is_deceased,pa.is_deleted,
                         pa.flashing_text, pa.flashing_text_added_by, pa.flashing_text_last_modified_date, 
                         pa.private_health_fund, pa.concession_card_number, pa.concession_card_expiry_date, pa.is_diabetic, pa.is_member_diabetes_australia, pa.diabetic_assessment_review_date, pa.ac_inv_offering_id, pa.ac_pat_offering_id, pa.login, pa.pwd, pa.is_company, pa.abn, 
                         pa.next_of_kin_name, pa.next_of_kin_relation, pa.next_of_kin_contact_info,
                         " + PersonDB.GetFields("", "p") + @",
                         t.title_id, t.descr
                       FROM
                         PatientReferrer AS pr 
                         LEFT OUTER JOIN Patient pa  ON pr.patient_id = pa.patient_id 
                         LEFT OUTER JOIN Person  p   ON pa.person_id  = p.person_id
                         LEFT OUTER JOIN Title   t   ON t.title_id    = p.title_id
                       WHERE  
                         pr.register_referrer_id = " + register_referrer_id.ToString() + (onlyActive ? " AND pr.is_active = 1 " : "") + @"
                         " + (matchSurname.Length > 0 && !searchSurnameOnlyStartsWith ? " AND p.surname LIKE '%" + matchSurname + "%'" : "") + @"
                         " + (matchSurname.Length > 0 &&  searchSurnameOnlyStartsWith ? " AND p.surname LIKE '" + matchSurname + "%'" : "") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || matchStreets   == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.address_channel_id IN (" + matchStreets + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "Contact"    || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM Contact AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchSuburbs   == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND ad.suburb_id IN (" + matchSuburbs + ")) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || matchStreets == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND (ad.street_name = '" + matchStreets + "' OR SOUNDEX(ad.street_name) = SOUNDEX('" + matchStreets + "'))) > 0 ") + @"
                         " + (Utilities.GetAddressType().ToString() != "ContactAus" || searchPhoneNbr == "" ? "" : " AND (SELECT COUNT(*) FROM ContactAus AS ad WHERE entity_id=p.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + searchPhoneNbr + "%'" + ") > 0 ") + @"
                         " + (matchMedicareCardNo.Length > 0 && !searchMedicareCardNoOnlyStartsWith ? " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '%" + matchMedicareCardNo + "%') > 0 " : "") + @"
                         " + (matchMedicareCardNo.Length > 0 && searchMedicareCardNoOnlyStartsWith ?  " AND (SELECT COUNT(*) FROM HealthCard hc WHERE hc.patient_id=pa.patient_id AND is_active = 1 AND [dbo].[ufnFilterNonAlphaNumeric]( hc.card_nbr) LIKE '" + matchMedicareCardNo + "%') > 0 " : "") + @"
                         " + (matchDOBDay    == -1 ? "" : " AND datepart(day,p.dob)   = " + matchDOBDay)   + @"
                         " + (matchDOBMonth  == -1 ? "" : " AND datepart(month,p.dob) = " + matchDOBMonth) + @"
                         " + (matchDOBYear   == -1 ? "" : " AND datepart(year,p.dob)  = " + matchDOBYear)  + @"
                         " + (show_deleted         ? "" : " AND pa.is_deleted = 0 ") + @"
                         " + (show_deceased        ? "" : " AND pa.is_deceased = 0 ") + @"
                         " + (!show_only_is_clinic_patient ? "" : " AND pa.is_clinic_patient   = 1 ") + @"
                         " + (!show_only_is_gp_patient ? "" : " AND pa.is_gp_patient   = 1 ") + @"
                         " + (matchReferrers == "" ? "" : " AND (SELECT COUNT(*) FROM PatientReferrer WHERE PatientReferrer.is_active = 1 AND patient_id=pa.patient_id AND register_referrer_id IN (" + matchReferrers + ")) > 0 ") + @"
                       ORDER BY 
                         p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Patient[] GetPatientsOf(int register_referrer_id)
    {
        return _GetPatientsOf(register_referrer_id, false);
    }
    public static Patient[] GetActivePatientsOf(int register_referrer_id)
    {
        return _GetPatientsOf(register_referrer_id, true);
    }
    private static Patient[] _GetPatientsOf(int register_referrer_id, bool onlyActive)
    {
        DataTable tbl = onlyActive ? GetDataTable_ActivePatientsOf(register_referrer_id) : GetDataTable_PatientsOf(register_referrer_id);
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Patient patient = PatientDB.Load(tbl.Rows[i]);
            patient.Person = PersonDB.Load(tbl.Rows[i]);
            patient.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
            list[i] = patient;
        }
        return list;
    }

    public static PatientReferrer LoadAll(DataRow row)
    {
        PatientReferrer pr                        = PatientReferrerDB.Load(row, "pr_");
        pr.RegisterReferrer                       = RegisterReferrerDB.Load(row, "regref_");
        pr.RegisterReferrer.Referrer              = ReferrerDB.Load(row, "referrer_");
        pr.RegisterReferrer.Referrer.Person       = PersonDB.Load(row, "referrer_person_");
        pr.RegisterReferrer.Referrer.Person.Title = IDandDescrDB.Load(row, "referrer_person_title_title_id", "referrer_person_title_descr");
        if (row["organisation_organisation_id"] != DBNull.Value)
            pr.RegisterReferrer.Organisation      = OrganisationDB.Load(row, "organisation_");
        pr.Patient                                = PatientDB.Load(row, "patient_");
        pr.Patient.Person                         = PersonDB.Load(row, "patient_person_");
        pr.Patient.Person.Title                   = IDandDescrDB.Load(row, "patient_person_title_title_id", "patient_person_title_descr");
        if (row["nonepcorg_entity_id"] != DBNull.Value)
            pr.Organisation = OrganisationDB.Load(row, "nonepcorg_");

        return pr;
    }

    public static PatientReferrer Load(DataRow row, string prefix="")
    {
        return new PatientReferrer(
            Convert.ToInt32(row[prefix+"patient_referrer_id"]),
            Convert.ToInt32(row[prefix+"patient_id"]),
            Convert.ToInt32(row[prefix+"register_referrer_id"]),
            row[prefix + "organisation_id"] == DBNull.Value ? 0 : Convert.ToInt32(row[prefix + "organisation_id"]),
            Convert.ToDateTime(row[prefix+"patient_referrer_date_added"]),
            Convert.ToBoolean(row[prefix+"is_debtor"])
        );
    }

}