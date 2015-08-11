using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

public class RegisterReferrerDB
{

    public static void Delete(int register_referrer_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM RegisterReferrer WHERE register_referrer_id = " + register_referrer_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int organisation_id, int referrer_id, string provider_number, bool report_every_visit_to_referrer, bool batch_send_all_patients_treatment_notes)
    {
        provider_number = provider_number.Replace("'", "''");

        string nRowsSql = "SELECT COUNT(*) FROM RegisterReferrer WHERE is_deleted = 0 AND organisation_id = " + organisation_id.ToString() + " AND referrer_id = " + referrer_id.ToString();
        if (Convert.ToInt32(DBBase.ExecuteSingleResult(nRowsSql)) > 0)
            throw new UniqueConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        string sql = "INSERT INTO RegisterReferrer (organisation_id,referrer_id, provider_number,report_every_visit_to_referrer,batch_send_all_patients_treatment_notes,date_last_batch_send_all_patients_treatment_notes) VALUES (" + "" + organisation_id + "," + "" + referrer_id + ",UPPER('" + provider_number + "')," + (report_every_visit_to_referrer ? "1" : "0") + "," + (batch_send_all_patients_treatment_notes ? "1" : "0") + "," + "NULL" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int register_referrer_id, int organisation_id, int referrer_id, string provider_number, bool report_every_visit_to_referrer, bool batch_send_all_patients_treatment_notes, DateTime date_last_batch_send_all_patients_treatment_notes)
    {
        provider_number = provider_number.Replace("'", "''");

        string sql = "UPDATE RegisterReferrer SET organisation_id = " + organisation_id + ",referrer_id = " + referrer_id + ",provider_number = UPPER('" + provider_number + "'),report_every_visit_to_referrer = " + (report_every_visit_to_referrer ? "1" : "0") + ",batch_send_all_patients_treatment_notes = " + (batch_send_all_patients_treatment_notes ? "1" : "0") + ",date_last_batch_send_all_patients_treatment_notes = " + (date_last_batch_send_all_patients_treatment_notes == DateTime.MinValue ? "NULL" : "'" + date_last_batch_send_all_patients_treatment_notes.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE register_referrer_id = " + register_referrer_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int register_referrer_id)
    {
        string sql = "UPDATE RegisterReferrer SET is_deleted = 1 WHERE register_referrer_id = " + register_referrer_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int register_referrer_id)
    {
        string sql = "UPDATE RegisterReferrer SET is_deleted = 0 WHERE register_referrer_id = " + register_referrer_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateLastBatchSendAllPatientsTreatmentNotes(int register_referrer_id, DateTime date_last_batch_send_all_patients_treatment_notes)
    {
        UpdateLastBatchSendAllPatientsTreatmentNotes(new int[] { register_referrer_id }, date_last_batch_send_all_patients_treatment_notes);
    }
    public static void UpdateLastBatchSendAllPatientsTreatmentNotes(int[] register_referrer_ids, DateTime date_last_batch_send_all_patients_treatment_notes)
    {
        if (register_referrer_ids.Length == 0)
            return;

        string sql = "UPDATE RegisterReferrer SET date_last_batch_send_all_patients_treatment_notes = " + (date_last_batch_send_all_patients_treatment_notes == DateTime.MinValue ? "NULL" : "'" + date_last_batch_send_all_patients_treatment_notes.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE register_referrer_id IN (" + string.Join(",", register_referrer_ids) + ")";
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateLastBatchSendAllPatientsTreatmentNotes_All(DateTime date_last_batch_send_all_patients_treatment_notes)
    {
        string sql = "UPDATE RegisterReferrer SET date_last_batch_send_all_patients_treatment_notes = " + (date_last_batch_send_all_patients_treatment_notes == DateTime.MinValue ? "NULL" : "'" + date_last_batch_send_all_patients_treatment_notes.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE batch_send_all_patients_treatment_notes = 1";
        DBBase.ExecuteNonResult(sql);
    }

    public static bool Exists(int register_referrer_id)
    {
        string sql = "SELECT COUNT(*) FROM RegisterReferrer WHERE register_referrer_id = " + register_referrer_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }
    public static bool Exists(int organisation_id, int referrer_id, int register_referrer_id_exclude = -1)
    {
        string sql = "SELECT COUNT(*) FROM RegisterReferrer WHERE organisation_id = " + organisation_id + " AND referrer_id = " + referrer_id + (register_referrer_id_exclude == -1 ? "" : " AND register_referrer_id <> " + register_referrer_id_exclude.ToString());
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }


    public static RegisterReferrer[] GetAllByBatchSendAllTreatmentNotes(bool batch_send_all_patients_treatment_notes)
    {
        string sql = JoinedSql() + " AND r.batch_send_all_patients_treatment_notes = " + (batch_send_all_patients_treatment_notes ? "1" : "0");
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        RegisterReferrer[] list = new RegisterReferrer[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }

    public static string[] BulkGetAllTreatmentNotes(DateTime date_start, DateTime date_end, string newline = "\n", bool incNoteIDForDebug = false) 
    {
        string recurring_condition = string.Empty;
        string non_recurring_condition = string.Empty;
        if (date_start != DateTime.MinValue && date_end != DateTime.MinValue)
        {
            recurring_condition     = "AND (  booking.date_start >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "' AND booking.date_end <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
            non_recurring_condition = "AND (  booking.date_end IS NULL OR booking.date_end > '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + (date_end == DateTime.MinValue ? "" : " AND booking.date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"";
        }
        else if (date_start != DateTime.MinValue)
        {
            recurring_condition     = "AND (  booking.date_start >= '" + date_start.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            non_recurring_condition = "AND (  booking.date_end IS NULL OR booking.date_end > '" + date_start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "') " + @"";
        }
        else if (date_end != DateTime.MinValue)
        {
            recurring_condition     = "AND (  booking.date_end  <= '" + date_end.ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
            non_recurring_condition = "AND (  booking.date_start < '" + date_end.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + @"' )";
        }
        else
        {
            recurring_condition     = "";
            non_recurring_condition = "";
        }


        string sql = @"

                SELECT 
                    note.note_id as note_note_id, note.entity_id as note_entity_id, note.note_type_id as note_note_type_id, note.body_part_id as note_body_part_id, note.text as note_text, note.date_added as note_date_added, note.date_modified as note_date_modified, note.added_by as note_added_by, note.modified_by as note_modified_by, note.site_id as note_site_id,


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
                    booking.recurring_start_time as booking_recurring_start_time,booking.recurring_end_time as booking_recurring_end_time,
                    (SELECT 0) AS booking_note_count,
                    (SELECT 0) AS booking_inv_count,

                    patient.patient_id as patient_patient_id, patient.person_id as patient_person_id, patient.patient_date_added as patient_patient_date_added, 
                    patient.is_clinic_patient as patient_is_clinic_patient,patient.is_gp_patient as patient_is_gp_patient,patient.is_deleted as patient_is_deleted,patient.is_deceased as patient_is_deceased, 
                    patient.flashing_text as patient_flashing_text, patient.flashing_text_added_by as patient_flashing_text_added_by, patient.flashing_text_last_modified_date as patient_flashing_text_last_modified_date, 
                    patient.private_health_fund as patient_private_health_fund, patient.concession_card_number as patient_concession_card_number, patient.concession_card_expiry_date as patient_concession_card_expiry_date, patient.is_diabetic as patient_is_diabetic, patient.is_member_diabetes_australia as patient_is_member_diabetes_australia, patient.diabetic_assessment_review_date as patient_diabetic_assessment_review_date, patient.ac_inv_offering_id as patient_ac_inv_offering_id, patient.ac_pat_offering_id as patient_ac_pat_offering_id, patient.login as patient_login, patient.pwd as patient_pwd, patient.is_company as patient_is_company, patient.abn as patient_abn, 
                    patient.next_of_kin_name as patient_next_of_kin_name, patient.next_of_kin_relation as patient_next_of_kin_relation, patient.next_of_kin_contact_info as patient_next_of_kin_contact_info,

                    " + PersonDB.GetFields("person_patient_", "person_patient") + @", 
                    title_patient.title_id as title_patient_title_id, title_patient.descr as title_patient_descr,


                    rr.register_referrer_id as rr_register_referrer_id, rr.organisation_id as rr_organisation_id, rr.referrer_id as rr_referrer_id, 
                    rr.provider_number as rr_provider_number, rr.report_every_visit_to_referrer as rr_report_every_visit_to_referrer, 
                    rr.batch_send_all_patients_treatment_notes as rr_batch_send_all_patients_treatment_notes, 
                    rr.date_last_batch_send_all_patients_treatment_notes as rr_date_last_batch_send_all_patients_treatment_notes, 
                    rr.register_referrer_date_added as rr_register_referrer_date_added, rr.is_deleted as rr_is_deleted,

                    ref.referrer_id as ref_referrer_id, ref.person_id as ref_person_id, ref.referrer_date_added as ref_referrer_date_added, 
                    " + PersonDB.GetFields("p_", "p") + @", 
                    t.title_id as t_title_id, t.descr as t_descr

                FROM
                    Note note
                    INNER JOIN Booking          booking           ON booking.entity_id                      = note.entity_id
                    INNER JOIN Patient          patient           ON patient.patient_id                     = booking.patient_id
                    INNER JOIN Person           person_patient    ON person_patient.person_id               = patient.person_id
                    INNER JOIN Title            title_patient     ON title_patient.title_id                 = person_patient.title_id

                    INNER Join PatientReferrer  patient_referrer  ON patient_referrer.patient_id            = patient.patient_id
                    INNER Join RegisterReferrer rr                ON rr.register_referrer_id                = patient_referrer.register_referrer_id
                    INNER JOIN Referrer         ref               ON ref.referrer_id                        = rr.referrer_id 
                    INNER JOIN Person           p                 ON p.person_id                            = ref.person_id
                    INNER JOIN Title            t                 ON t.title_id                             = p.title_id

                WHERE
                    note.note_type_id = 252             -- only provider treatment notes
                    AND booking.booking_status_id = 187 -- only get completed bookings
                    AND booking.booking_type_id   = 34  -- only get bookings for patients (not blockout-prov/org-timeslot bookings)
                    AND (
                            (booking.is_recurring = 0 " + recurring_condition + @") OR  
                            (booking.is_recurring = 1 " + non_recurring_condition + @") 
                        )
                    AND rr.is_deleted = 0
                    AND rr.batch_send_all_patients_treatment_notes = 1

                ORDER BY 
                    rr.register_referrer_id, person_patient.surname, person_patient.firstname, booking.date_start, note.note_id
                    ";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        string[] notes = new string[tbl.Rows.Count];
        for(int i=0; i<tbl.Rows.Count; i++)
        {
            Booking booking = BookingDB.Load(tbl.Rows[i], "booking_");
            booking.Patient = PatientDB.Load(tbl.Rows[i], "patient_");
            booking.Patient.Person = PersonDB.Load(tbl.Rows[i], "person_patient_");
            booking.Patient.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_patient_title_id", "title_patient_descr");

            Note note    = NoteDB.Load(tbl.Rows[i], "note_");

            RegisterReferrer rr = RegisterReferrerDB.Load(tbl.Rows[i], "rr_");
            rr.Referrer = ReferrerDB.Load(tbl.Rows[i], "ref_");
            rr.Referrer.Person = PersonDB.Load(tbl.Rows[i], "p_");
            rr.Referrer.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "t_title_id", "t_descr");

            notes[i] = booking.GetNoteTextForTreatmentLetter(rr.Referrer, note, newline, incNoteIDForDebug);
        }

        return notes;
    }


    public static DataTable GetDataTable(int org_id = 0, int referrer_id = -1, bool showDeleted = false, int[] orgTypes = null, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, string matchSuburb = "", string matchPostcode = "", string matchProviderNbr = "", bool searchProviderNbrOnlyStartsWith = false, string matchPhoneNbr = "", bool searchPhoneNbrOnlyStartsWith = false)
    {
        matchSurname     = matchSurname.Replace("'", "''");
        matchSuburb      = matchSuburb.Replace("'", "''");
        matchPostcode    = matchPostcode.Replace("'", "''");
        matchProviderNbr = matchProviderNbr.Replace("'", "''");
        matchPhoneNbr    = matchPhoneNbr.Replace("'", "''");

        bool   incSuburbSearch    = (matchSuburb   != null && matchSuburb.Length   > 0);
        bool   incPostcodeSearch  = (matchPostcode != null && matchPostcode.Length > 0);
        string suburbSearchClause = (!incSuburbSearch && !incPostcodeSearch) ? "" : @" AND (SELECT COUNT(*) FROM " + Utilities.GetAddressType().ToString() + @" LEFT JOIN Suburb ON Contact.suburb_id = Suburb.suburb_id WHERE entity_id = o.entity_id " + (incSuburbSearch ? " AND Suburb.name LIKE '%" + matchSuburb + "%' " : "") + (incPostcodeSearch ? " AND Suburb.postcode LIKE '%" + matchPostcode + "%' " : "") + ") > 0 ";

        string sql = JoinedSql(showDeleted, false, false) +
                                    (org_id != 0 ? " AND r.organisation_id = " + org_id : "") +
                                    (referrer_id != -1 ? " AND r.referrer_id = " + referrer_id : "") +
                                    (orgTypes != null && orgTypes.Length > 0 ? " AND o.organisation_type_id IN (" + string.Join(",", orgTypes) + ")" : "") +
                                    ((matchSurname.Length > 0 && !searchSurnameOnlyStartsWith) ? " AND p.surname LIKE '%" + matchSurname + "%'" : "") + ((matchSurname.Length > 0 && searchSurnameOnlyStartsWith) ? " AND p.surname LIKE '" + matchSurname + "%'" : "") +
                                    ((matchProviderNbr.Length > 0 && !searchProviderNbrOnlyStartsWith) ? " AND r.provider_number LIKE '%" + matchProviderNbr + "%'" : "") + ((matchProviderNbr.Length > 0 && searchProviderNbrOnlyStartsWith) ? " AND r.provider_number LIKE '" + matchProviderNbr + "%'" : "") +
                                    (matchPhoneNbr.Length  > 0 ? " AND (SELECT COUNT(*) FROM " + Utilities.GetAddressType().ToString() + @" AS ad LEFT JOIN ContactType ON ContactType.contact_type_id = ad.contact_type_id WHERE ContactType.contact_type_group_id = 2 AND entity_id=o.entity_id AND ad.contact_date_deleted IS NULL AND dbo.ufnFilterNonDigit(addr_line1) LIKE '" + (searchPhoneNbrOnlyStartsWith ? "" : "%") + matchPhoneNbr + "%'" + ") > 0 " : "") +
                                    suburbSearchClause +
                                    " ORDER BY p.surname, p.firstname, p.middlename ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Hashtable GetHashtableByReferrer(bool incDeleted = false)
    {
        Hashtable hashtable = new Hashtable();

        DataTable tbl = DBBase.ExecuteQuery(JoinedSql(incDeleted, incDeleted, incDeleted)).Tables[0];
        foreach (DataRow row in tbl.Rows)
        {
            if (row["organisation_id"] == DBNull.Value)
                continue;

            Referrer r = ReferrerDB.Load(row);
            r.Person = PersonDB.Load(row, "", "person_entity_id");
            r.Person.Title = IDandDescrDB.Load(row, "title_id", "descr");
            Organisation o = OrganisationDB.Load(row, "", "organisation_entity_id", "organisation_is_deleted");

            if (hashtable[r.ReferrerID] == null)
                hashtable[r.ReferrerID] = new ArrayList();
            ((ArrayList)hashtable[r.ReferrerID]).Add(o);
        }

        return hashtable;
    }

    public static RegisterReferrer GetByID(int register_referrer_id)
    {
        string sql = JoinedSql(true, true, true) + " AND register_referrer_id = " + register_referrer_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        if (tbl.Rows.Count == 0)
            return null;
        else
            return LoadAll(tbl.Rows[0]);
    }

    private static string JoinedSql(bool incRegRefDeleted = false, bool incRefDeleted = false, bool incOrgDeleted = false)
    {
        string whereClause = " 1=1 ";
        if (!incRegRefDeleted)
            whereClause += (whereClause.Length == 0 ? "" : " AND ") + " r.is_deleted = 0 ";
        if (!incRefDeleted)
            whereClause += (whereClause.Length == 0 ? "" : " AND ") + " ref.is_deleted = 0 ";
        if (!incOrgDeleted)
            whereClause += (whereClause.Length == 0 ? "" : " AND ") + " o.is_deleted = 0 ";


        string sql = @"
        SELECT
                      r.register_referrer_id, r.organisation_id, r.referrer_id, r.provider_number, r.report_every_visit_to_referrer, r.batch_send_all_patients_treatment_notes, r.date_last_batch_send_all_patients_treatment_notes, r.register_referrer_date_added, r.is_deleted,
                      o.entity_id as organisation_entity_id, o.parent_organisation_id, o.use_parent_offernig_prices, 
                      o.organisation_type_id, o.organisation_customer_type_id, o.name, o.acn, o.abn, o.organisation_date_added, o.organisation_date_modified, o.is_debtor, o.is_creditor, o.bpay_account, 
                      o.weeks_per_service_cycle, o.start_date, o.end_date, o.comment, o.free_services, o.excl_sun, o.excl_mon, o.excl_tue, o.excl_wed, o.excl_thu, o.excl_fri, o.excl_sat, 
                      o.sun_start_time, o.sun_end_time, o.mon_start_time, o.mon_end_time, o.tue_start_time, o.tue_end_time, o.wed_start_time, o.wed_end_time, 
                      o.thu_start_time, o.thu_end_time, o.fri_start_time, o.fri_end_time, o.sat_start_time, o.sat_end_time, 
                      o.sun_lunch_start_time, o.sun_lunch_end_time, o.mon_lunch_start_time, o.mon_lunch_end_time, o.tue_lunch_start_time, o.tue_lunch_end_time, o.wed_lunch_start_time, o.wed_lunch_end_time, 
                      o.thu_lunch_start_time, o.thu_lunch_end_time, o.fri_lunch_start_time, o.fri_lunch_end_time, o.sat_lunch_start_time, o.sat_lunch_end_time, 
                      o.last_batch_run, o.is_deleted as organisation_is_deleted,
                      ref.person_id, ref.referrer_date_added, 
                      " + PersonDB.GetFields("", "p").Replace("p.entity_id", "p.entity_id AS person_entity_id") + @", 
                      t.title_id, t.descr
        FROM
                      RegisterReferrer        r 
                      LEFT OUTER JOIN Organisation o   ON o.organisation_id = r.organisation_id 
                      INNER JOIN      Referrer     ref ON ref.referrer_id   = r.referrer_id 
                      INNER JOIN      Person       p   ON p.person_id       = ref.person_id
                      INNER JOIN      Title        t   ON t.title_id        = p.title_id
        WHERE
                      " + whereClause;

        return sql;
    }



    public static Hashtable GetByIDsInHashtable(int[] regRefIDs)
    {
        if (regRefIDs == null || regRefIDs.Length == 0)
            return new Hashtable();

        Hashtable hash = new Hashtable();

        string sql = JoinedSql(true, true, true) + @" AND r.register_referrer_id in (" + string.Join(",", regRefIDs) + @")";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            RegisterReferrer regRef = LoadAll(tbl.Rows[i]);
            hash[regRef.RegisterReferrerID] = regRef;
        }

        return hash;
    }

    public static int[] GetOrgEntityIDs(int[] regRefIDs)
    {
        if (regRefIDs == null || regRefIDs.Length == 0)
            return new int[] { };

        string sql =
            @"SELECT register_referrer_id, Organisation.entity_id
              FROM   RegisterReferrer
		             LEFT JOIN Organisation ON RegisterReferrer.organisation_id = Organisation.organisation_id
              WHERE  register_referrer_id in (" + string.Join(",", regRefIDs) + @")";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        int[] entityIDs = new int[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            entityIDs[i] = Convert.ToInt32(tbl.Rows[i]["entity_id"]);
        return entityIDs;
    }
    public static Hashtable GetOrgEntityIDsHash(int[] regRefIDs)
    {
        if (regRefIDs == null || regRefIDs.Length == 0)
            return new Hashtable();

        string sql =
            @"SELECT register_referrer_id, Organisation.entity_id
              FROM   RegisterReferrer
		             LEFT JOIN Organisation ON RegisterReferrer.organisation_id = Organisation.organisation_id
              WHERE  register_referrer_id in (" + string.Join(",", regRefIDs) + @")";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
            hash[Convert.ToInt32(tbl.Rows[i]["register_referrer_id"])] = Convert.ToInt32(tbl.Rows[i]["entity_id"]);
        return hash;
    }


    public static RegisterReferrer[] RemoveRegisterReferrersByEmailMobile(
        RegisterReferrer[] regRefs, 
        bool incHasBothMobileEmail,
        bool incHasMobileNoEmail,
        bool incHasEmailNoMobile,
        bool incHasNeitherMobileEmail)
    {
        if (incHasBothMobileEmail && incHasMobileNoEmail && incHasEmailNoMobile && incHasNeitherMobileEmail)
            return regRefs;
        if (!incHasBothMobileEmail && !incHasMobileNoEmail && !incHasEmailNoMobile && !incHasNeitherMobileEmail)
            return new RegisterReferrer[] { };

        int[]     regRefIDs    = regRefs.Select(r => r.RegisterReferrerID).ToArray();
        int[]     entityIDs    = regRefs.Select(r => r.Organisation.EntityID).ToArray();
        Hashtable entityIDHash = RegisterReferrerDB.GetOrgEntityIDsHash(regRefIDs);
        Hashtable regRefIDHash = RegisterReferrerDB.GetByIDsInHashtable(regRefIDs);
        Hashtable emailHash    = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);
        Hashtable mobileHash   = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1, "30");

        ArrayList newList = new ArrayList();
        for (int i = 0; i < regRefs.Length; i++)
        {
            bool hasEmail  = emailHash[regRefs[i].Organisation.EntityID]  != null;
            bool hasMobile = mobileHash[regRefs[i].Organisation.EntityID] != null;

            if ((incHasBothMobileEmail     &&  hasEmail &&  hasMobile) ||
                 (incHasMobileNoEmail      && !hasEmail &&  hasMobile) ||
                 (incHasEmailNoMobile      &&  hasEmail && !hasMobile) ||
                 (incHasNeitherMobileEmail && !hasEmail && !hasMobile))
                newList.Add(regRefs[i]);
        }

        return (RegisterReferrer[])newList.ToArray(typeof(RegisterReferrer));
    }
    public static ArrayList RemoveRegisterReferrersByEmailMobile(
        ArrayList regRefs, 
        bool incHasBothMobileEmail,
        bool incHasMobileNoEmail,
        bool incHasEmailNoMobile,
        bool incHasNeitherMobileEmail)
    {
        return new ArrayList(RemoveRegisterReferrersByEmailMobile((RegisterReferrer[])regRefs.ToArray(typeof(RegisterReferrer)), incHasBothMobileEmail, incHasMobileNoEmail, incHasEmailNoMobile, incHasNeitherMobileEmail));
    }

    public static void GetCountsByEmailMobile(
        RegisterReferrer[] regRefs,
        ref int hasBothMobileEmail,
        ref int hasMobileNoEmail,
        ref int hasEmailNoMobile,
        ref int hasNeitherMobileEmail)
    {
        int[]     regRefIDs    = regRefs.Select(r => r.RegisterReferrerID).ToArray();
        int[]     entityIDs    = regRefs.Select(r => r.Organisation.EntityID).ToArray();
        Hashtable entityIDHash = RegisterReferrerDB.GetOrgEntityIDsHash(regRefIDs);
        Hashtable regRefIDHash = RegisterReferrerDB.GetByIDsInHashtable(regRefIDs);
        Hashtable emailHash    = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);
        Hashtable mobileHash   = PatientsContactCacheDB.GetBullkPhoneNumbers(entityIDs, -1, "30");

        hasBothMobileEmail    = 0;
        hasMobileNoEmail      = 0;
        hasEmailNoMobile      = 0;
        hasNeitherMobileEmail = 0;

        for (int i = 0; i < regRefs.Length; i++)
        {
            bool hasEmail  = emailHash[regRefs[i].Organisation.EntityID]  != null;
            bool hasMobile = mobileHash[regRefs[i].Organisation.EntityID] != null;

            if (hasEmail  &&  hasMobile)
                hasBothMobileEmail++;
            if (!hasEmail &&  hasMobile)
                hasMobileNoEmail++;
            if (hasEmail  && !hasMobile)
                hasEmailNoMobile++;
            if (!hasEmail && !hasMobile)
                hasNeitherMobileEmail++;
        }
    }


    public static DataTable GetDataTable_OrganisationsOf(int referrer_id, bool showDeletedOrgs = false, bool showDeletedRegRefs = false, string matchName = "", bool searchNameOnlyStartsWith = false)
    {
        matchName = matchName.Replace("'", "''");

        string sql = @"SELECT 
                         r.register_referrer_id, r.organisation_id, r.referrer_id, r.provider_number, r.report_every_visit_to_referrer, r.batch_send_all_patients_treatment_notes, r.date_last_batch_send_all_patients_treatment_notes, r.register_referrer_date_added, 
                         o.organisation_id, o.entity_id as organisation_entity_id, o.parent_organisation_id, o.use_parent_offernig_prices, o.organisation_type_id, o.organisation_customer_type_id, o.name, o.acn, o.abn, o.organisation_date_added, o.organisation_date_modified, o.is_debtor, o.is_creditor, o.bpay_account, 
                         o.weeks_per_service_cycle, o.start_date, o.end_date, o.comment, o.free_services, o.excl_sun, o.excl_mon, o.excl_tue, o.excl_wed, o.excl_thu, o.excl_fri, o.excl_sat, 
                         o.sun_start_time, o.sun_end_time, o.mon_start_time, o.mon_end_time, o.tue_start_time, o.tue_end_time, o.wed_start_time, o.wed_end_time, o.thu_start_time, o.thu_end_time, o.fri_start_time, o.fri_end_time, o.sat_start_time, o.sat_end_time, 
                         o.sun_lunch_start_time, o.sun_lunch_end_time, o.mon_lunch_start_time, o.mon_lunch_end_time, o.tue_lunch_start_time, o.tue_lunch_end_time, o.wed_lunch_start_time, o.wed_lunch_end_time, o.thu_lunch_start_time, o.thu_lunch_end_time, o.fri_lunch_start_time, o.fri_lunch_end_time, o.sat_lunch_start_time, o.sat_lunch_end_time, 
                         o.last_batch_run, 
                         o.is_deleted as organisation_is_deleted, r.is_deleted as register_referrer_is_deleted
                       FROM 
                         RegisterReferrer AS r 
                         LEFT OUTER JOIN Organisation AS o ON r.organisation_id = o.organisation_id 
                       WHERE 
                         r.organisation_id IS NOT null AND " +
                         (showDeletedRegRefs ? "" : "r.is_deleted = 0 AND ") + 
                         (showDeletedOrgs    ? "" : "o.is_deleted = 0 AND ") + 
                         "r.referrer_id = " + referrer_id.ToString() +
                         ((matchName.Length > 0 && !searchNameOnlyStartsWith) ? " AND o.name LIKE '%" + matchName + "%'" : "") + 
                         ((matchName.Length > 0 && searchNameOnlyStartsWith)  ? " AND o.name LIKE '"  + matchName + "%'" : "") + @"
                       ORDER BY o.name";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Organisation[] GetOrganisationsOf(int staff_id)
    {
        DataTable tbl = GetDataTable_OrganisationsOf(staff_id);
        Organisation[] list = new Organisation[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = OrganisationDB.Load(tbl.Rows[i], "", "organisation_entity_id", "organisation_is_deleted");
        return list;
    }

    public static DataTable GetDataTable_ReferrersOf(int organistion_id, bool showDeletedRefs = false, bool showDeletedRegRefs = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false)
    {
        matchSurname = matchSurname.Replace("'", "''");

        string sql = @"SELECT 
                         rr.register_referrer_id, rr.organisation_id, rr.referrer_id, rr.provider_number, rr.report_every_visit_to_referrer,rr.batch_send_all_patients_treatment_notes,rr.date_last_batch_send_all_patients_treatment_notes,rr.register_referrer_date_added, 
                         r.person_id, r.referrer_date_added, r.is_deleted as referrer_is_deleted,
                         " + PersonDB.GetFields("", "p").Replace("p.entity_id", "p.entity_id AS person_entity_id") + @", 
                         t.title_id, t.descr,
                         r.is_deleted as referrer_is_deleted, rr.is_deleted as register_referrer_is_deleted
                       FROM
                         RegisterReferrer AS rr 
                         LEFT OUTER JOIN Referrer AS r ON rr.referrer_id = r.referrer_id 
                         LEFT OUTER JOIN Person   AS p ON r.person_id = p.person_id
                         LEFT OUTER JOIN Title       t   ON t.title_id        = p.title_id
                       WHERE 
                       " +(showDeletedRegRefs ? "" : "rr.is_deleted = 0 AND ") + 
                         (showDeletedRefs    ? "" : "r.is_deleted  = 0 AND ") + 
                         "rr.organisation_id = " + organistion_id.ToString() + 
                         ((matchSurname.Length > 0 && !searchSurnameOnlyStartsWith) ? " AND p.surname LIKE '%" + matchSurname + "%'" : "") + 
                         ((matchSurname.Length > 0 && searchSurnameOnlyStartsWith)  ? " AND p.surname LIKE '"  + matchSurname + "%'" : "") + @"
                       ORDER BY 
                         p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Referrer[] GetReferrersOf(int organistion_id, bool showDeletedRefs = false, bool showDeletedRegRefs = false)
    {
        DataTable tbl = GetDataTable_ReferrersOf(organistion_id, showDeletedRefs, showDeletedRegRefs);
        Referrer[] list = new Referrer[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = ReferrerDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i], "", "person_entity_id");
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
        }
        return list;
    }



    public static DataTable GetDataTable_AllNotInc(RegisterReferrer[] excList)
    {
        string notInList = string.Empty;
        foreach (RegisterReferrer p in excList)
            notInList += p.RegisterReferrerID.ToString() + ",";
        if (notInList.Length > 0)
            notInList = notInList.Substring(0, notInList.Length - 1);

        string sql = JoinedSql();

        if (notInList.Length > 0)
            sql += @" AND register_referrer_id NOT IN (" + notInList + @") ";

        sql += @" ORDER BY 
                         p.surname, p.firstname, p.middlename, o.name";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Patient[] GetAllNotInc(RegisterReferrer[] excList)
    {
        DataTable tbl = GetDataTable_AllNotInc(excList);
        Patient[] list = new Patient[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = PatientDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i]);
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "type_id", "descr");
        }

        return list;
    }


    public static DataTable GetDataTable_AllActiveRegRefByPatientsOfInternalOrg(int organisation_id)
    {
        string sql = @"
            SELECT DISTINCT 
                rr.register_referrer_id, rr.organisation_id, rr.referrer_id, rr.provider_number, rr.report_every_visit_to_referrer, rr.batch_send_all_patients_treatment_notes, rr.date_last_batch_send_all_patients_treatment_notes, rr.register_referrer_date_added, rr.is_deleted,
                o.entity_id as organisation_entity_id, o.parent_organisation_id, o.use_parent_offernig_prices, 
                o.organisation_type_id, o.organisation_customer_type_id, o.name, o.acn, o.abn, o.organisation_date_added, o.organisation_date_modified, o.is_debtor, o.is_creditor, o.bpay_account, 
                o.weeks_per_service_cycle, o.start_date, o.end_date, o.comment, o.free_services, o.excl_sun, o.excl_mon, o.excl_tue, o.excl_wed, o.excl_thu, o.excl_fri, o.excl_sat, 
                o.sun_start_time, o.sun_end_time, o.mon_start_time, o.mon_end_time, o.tue_start_time, o.tue_end_time, o.wed_start_time, o.wed_end_time, 
                o.thu_start_time, o.thu_end_time, o.fri_start_time, o.fri_end_time, o.sat_start_time, o.sat_end_time, 
                o.sun_lunch_start_time, o.sun_lunch_end_time, o.mon_lunch_start_time, o.mon_lunch_end_time, o.tue_lunch_start_time, o.tue_lunch_end_time, o.wed_lunch_start_time, o.wed_lunch_end_time, 
                o.thu_lunch_start_time, o.thu_lunch_end_time, o.fri_lunch_start_time, o.fri_lunch_end_time, o.sat_lunch_start_time, o.sat_lunch_end_time, 
                o.last_batch_run, o.is_deleted as organisation_is_deleted,
                ref.person_id, ref.referrer_date_added, 
                " + PersonDB.GetFields("", "p").Replace("p.entity_id", "p.entity_id AS person_entity_id") + @", 
                t.title_id, t.descr


            FROM 
	            PatientReferrer pr
	            LEFT JOIN RegisterReferrer rr ON pr.register_referrer_id = rr.register_referrer_id
                LEFT OUTER JOIN Organisation o   ON o.organisation_id = rr.organisation_id 
                LEFT JOIN       Referrer     ref ON ref.referrer_id   = rr.referrer_id 
                LEFT JOIN       Person       p   ON p.person_id       = ref.person_id
                LEFT JOIN       Title        t   ON t.title_id        = p.title_id

	            LEFT JOIN Patient patient ON pr.patient_id = patient.patient_id
	
            WHERE
	            pr.is_active = 1 AND rr.is_deleted = 0 AND patient.is_deleted = 0

	            -- make sure its a referring 'doctor' and not a referring organisation (ie a mailout that referred a new customer
	            AND pr.register_referrer_id IS NOT NULL
	    
	            -- get by org_id
	            AND pr.patient_id IN (SELECT patient_id FROM RegisterPatient WHERE is_deleted = 0 AND organisation_id = " + organisation_id + @")
	
	            -- where patiet has multiple referring doctors, get only the current one
	            AND pr.patient_referrer_date_added = (
				            SELECT MAX(pr2.patient_referrer_date_added) FROM PatientReferrer pr2 WHERE pr2.register_referrer_id IS NOT NULL AND pr2.patient_id = pr.patient_id
	            )
                ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static RegisterReferrer[] GetAllActiveRegRefByPatientsOfInternalOrg(int organisation_id)
    {
        DataTable tbl = GetDataTable_AllActiveRegRefByPatientsOfInternalOrg(organisation_id);
        RegisterReferrer[] list = new RegisterReferrer[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }



    public static RegisterReferrer LoadAll(DataRow row)
    {
        RegisterReferrer rr = Load(row);
        rr.Referrer = ReferrerDB.Load(row);
        rr.Referrer.Person = PersonDB.Load(row, "", "person_entity_id");
        rr.Referrer.Person.Title = IDandDescrDB.Load(row, "title_id", "descr");
        if (row["organisation_id"] != DBNull.Value)
            rr.Organisation = OrganisationDB.Load(row, "", "organisation_entity_id", "organisation_is_deleted");
        return rr;
    }

    public static RegisterReferrer Load(DataRow row, string prefix="")
    {
        return new RegisterReferrer(
            Convert.ToInt32(row[prefix+"register_referrer_id"]),
            row[prefix+"organisation_id"] == DBNull.Value ? 0 : Convert.ToInt32(row[prefix+"organisation_id"]),
            Convert.ToInt32(row[prefix+"referrer_id"]),
            Convert.ToString(row[prefix + "provider_number"]),
            Convert.ToBoolean(row[prefix+"report_every_visit_to_referrer"]),
            Convert.ToBoolean(row[prefix+"batch_send_all_patients_treatment_notes"]),
            row[prefix+"date_last_batch_send_all_patients_treatment_notes"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix+"date_last_batch_send_all_patients_treatment_notes"]),
            Convert.ToDateTime(row[prefix+"register_referrer_date_added"])
        );
    }

}