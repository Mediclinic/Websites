using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

public class RegisterStaffDB
{

    public static void Delete(int register_staff_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM RegisterStaff WHERE register_staff_id = " + register_staff_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int organisation_id, int staff_id, string provider_number, bool main_provider_for_clinic, bool excl_sun, bool excl_mon, bool excl_tue, bool excl_wed, bool excl_thu, bool excl_fri, bool excl_sat)
    {
        string nRowsSql = "SELECT COUNT(*) FROM RegisterStaff WHERE is_deleted = 0 AND organisation_id = " + organisation_id.ToString() + " AND staff_id = " + staff_id.ToString();
        if (Convert.ToInt32(DBBase.ExecuteSingleResult(nRowsSql)) > 0)
            throw new UniqueConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        provider_number = provider_number.Replace("'", "''");
        string sql = "INSERT INTO RegisterStaff (organisation_id,staff_id,provider_number,main_provider_for_clinic,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat) VALUES (" + organisation_id + "," + "" + staff_id + "," + "'" + provider_number + "'," + (main_provider_for_clinic ? "1," : "0,") + (excl_sun ? "1," : "0,") + (excl_mon ? "1," : "0,") + (excl_tue ? "1," : "0,") + (excl_wed ? "1," : "0,") + (excl_thu ? "1," : "0,") + (excl_fri ? "1," : "0,") + (excl_sat ? "1" : "0") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int register_staff_id, int organisation_id, int staff_id, string provider_number, bool main_provider_for_clinic, bool excl_sun, bool excl_mon, bool excl_tue, bool excl_wed, bool excl_thu, bool excl_fri, bool excl_sat)
    {
        provider_number = provider_number.Replace("'", "''");
        string sql = "UPDATE RegisterStaff SET organisation_id = " + organisation_id + ",staff_id = " + staff_id + ",provider_number = '" + provider_number + "',main_provider_for_clinic = " + (main_provider_for_clinic ? "1," : "0,") + "excl_sun = " + (excl_sun ? "1," : "0,") + "excl_mon = " + (excl_mon ? "1," : "0,") + "excl_tue = " + (excl_tue ? "1," : "0,") + "excl_wed = " + (excl_wed ? "1," : "0,") + "excl_thu = " + (excl_thu ? "1," : "0,") + "excl_fri = " + (excl_fri ? "1," : "0,") + "excl_sat = " + (excl_sat ? "1" : "0") + " WHERE register_staff_id = " + register_staff_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int register_staff_id, bool checkFKConstraint = true)
    {
        if (checkFKConstraint)
        {
            RegisterStaff registerStaff = RegisterStaffDB.GetByID(register_staff_id);
            if (BookingDB.GetCountByProviderAndOrg(registerStaff.Staff.StaffID, registerStaff.Organisation.OrganisationID, "0") > 0)
                throw new CustomMessageException("Can not remove registration of '" + registerStaff.Staff.Person.FullnameWithoutMiddlename + "' to '" + registerStaff.Organisation.Name + "' because there exists incomplete bookings for this provider there.");
        }

        string sql = "UPDATE RegisterStaff SET is_deleted = 1 WHERE register_staff_id = " + register_staff_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int register_staff_id, bool checkFKConstraint = true)
    {
        if (checkFKConstraint)
        {
            RegisterStaff registerStaff = RegisterStaffDB.GetByID(register_staff_id);
            if (IsStaffWorkingInOrg(registerStaff.Staff.StaffID, registerStaff.Organisation.OrganisationID, false))
                throw new CustomMessageException("Can not undelete registration of '" + registerStaff.Staff.Person.FullnameWithoutMiddlename + "' to '" + registerStaff.Organisation.Name + "' because a new active regsitration exists already.");
        }

        string sql = "UPDATE RegisterStaff SET is_deleted = 0 WHERE register_staff_id = " + register_staff_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateAllOtherStaffAsNotMainProviders(int organisation_id, int staff_id)
    {
        string sql = "UPDATE RegisterStaff SET main_provider_for_clinic = 0 WHERE organisation_id = " + organisation_id + " AND staff_id <> " + staff_id;
        DBBase.ExecuteNonResult(sql);
    }

    public static bool IsStaffWorkingInOrg(int staff_id, int organisation_id, bool inc_deleted = true)
    {
        string sql = "SELECT COUNT(*) FROM RegisterStaff WHERE " + (inc_deleted ? "" : " is_deleted = 0 AND ") + " staff_id = " + staff_id.ToString() + " AND organisation_id = " + organisation_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }
    public static int GetCountByProvNbr(string provider_number)
    {
        provider_number = provider_number.Replace("'", "''");
        string sql = "SELECT COUNT(*) FROM RegisterStaff WHERE provider_number = '" + provider_number.ToString() + "'";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static string GetAllProviderNbrs()
    {
        string sql = "SELECT DISTINCT provider_number FROM RegisterStaff WHERE LEN(provider_number) > 0";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        string s = string.Empty;
        foreach (DataRow row in tbl.Rows)
            s += row["provider_number"].ToString() + ",";
        return s;
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT register_staff_id,register_staff_date_added,organisation_id,staff_id,provider_number,main_provider_for_clinic,excl_sun, excl_mon, excl_tue, excl_wed, excl_thu, excl_fri, excl_sat FROM RegisterStaff WHERE is_deleted = 0";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static RegisterStaff GetByID(int register_staff_id)
    {
        string sql = JoinedSql + " AND register_staff_id = " + register_staff_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        if (tbl.Rows.Count == 0)
            return null;
        else
        {
            RegisterStaff rr = Load(tbl.Rows[0], "registration_provider_number");
            rr.Staff = StaffDB.Load(tbl.Rows[0], "staff_");
            rr.Staff.Person = PersonDB.Load(tbl.Rows[0], "", "person_entity_id");
            rr.Organisation = OrganisationDB.Load(tbl.Rows[0], "organisation_");
            return rr;
        }
    }
    public static RegisterStaff GetByStaffIDAndOrganisationID(int staff_id, int organisation_id, bool inc_deleted = false)
    {
        string sql = JoinedSql + " AND r.staff_id = " + staff_id + " AND r.organisation_id = " + organisation_id + (inc_deleted ? "" : " AND r.is_deleted = 0 ");
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        if (tbl.Rows.Count == 0)
            return null;
        else
        {
            RegisterStaff rr = Load(tbl.Rows[0], "registration_provider_number");
            rr.Staff = StaffDB.Load(tbl.Rows[0], "staff_");
            rr.Staff.Person = PersonDB.Load(tbl.Rows[0], "", "person_entity_id");
            rr.Organisation = OrganisationDB.Load(tbl.Rows[0], "organisation_");
            return rr;
        }
    }



    public static DataTable GetDataTable_OrganisationsOf(int staff_id, string organisation_type_group_ids = null, bool inc_deleted = false)
    {
        bool select_by_type_group_id = organisation_type_group_ids != null && organisation_type_group_ids.Length > 0;

        string sql = @"SELECT 
                         r.register_staff_id,r.register_staff_date_added,r.organisation_id,r.provider_number AS registration_provider_number,r.main_provider_for_clinic,
                         r.excl_sun, r.excl_mon, r.excl_tue, r.excl_wed, r.excl_thu, r.excl_fri, r.excl_sat, r.is_deleted as registration_is_deleted,
                         o.organisation_id, o.entity_id as organisation_entity_id, o.parent_organisation_id, o.use_parent_offernig_prices, o.organisation_type_id, o.organisation_customer_type_id, o.name, o.acn, o.abn, o.organisation_date_added, o.organisation_date_modified, o.is_debtor, o.is_creditor, o.bpay_account, 
                         o.weeks_per_service_cycle, o.start_date, o.end_date, o.comment, o.free_services, o.excl_sun, o.excl_mon, o.excl_tue, o.excl_wed, o.excl_thu, o.excl_fri, o.excl_sat, 
                         o.sun_start_time, o.sun_end_time, o.mon_start_time, o.mon_end_time, o.tue_start_time, o.tue_end_time, o.wed_start_time, o.wed_end_time, o.thu_start_time, o.thu_end_time, o.fri_start_time, o.fri_end_time, o.sat_start_time, o.sat_end_time, 
                         o.sun_lunch_start_time, o.sun_lunch_end_time, o.mon_lunch_start_time, o.mon_lunch_end_time, o.tue_lunch_start_time, o.tue_lunch_end_time, o.wed_lunch_start_time, o.wed_lunch_end_time, o.thu_lunch_start_time, o.thu_lunch_end_time, o.fri_lunch_start_time, o.fri_lunch_end_time, o.sat_lunch_start_time, o.sat_lunch_end_time, 
                         o.last_batch_run, o.is_deleted as organisation_is_deleted
                       FROM 
                         RegisterStaff AS r 
                         LEFT OUTER JOIN Organisation AS o ON r.organisation_id = o.organisation_id 
                        " + (select_by_type_group_id ? "LEFT OUTER JOIN OrganisationType org_type ON o.organisation_type_id = org_type.organisation_type_id " : "") + @"
                       WHERE 
                          r.staff_id = " + staff_id.ToString() + (inc_deleted ? "" : " AND r.is_deleted = 0 AND o.is_deleted = 0 ") + (select_by_type_group_id ? " AND org_type.organisation_type_group_id IN (" + organisation_type_group_ids + ") " : "") + @"
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

    public static DataTable GetDataTable_StaffOf(int organistion_id, bool inc_deleted = false, bool only_providers = false, bool excl_external = true)
    {
        string sql = @"SELECT 
                         r.register_staff_id,r.register_staff_date_added,r.organisation_id,r.provider_number AS registration_provider_number,r.main_provider_for_clinic,
                         r.excl_sun, r.excl_mon, r.excl_tue, r.excl_wed, r.excl_thu, r.excl_fri, r.excl_sat, r.is_deleted as registration_is_deleted,
                         s.staff_id, s.person_id, s.login, s.pwd, s.staff_position_id, s.field_id, s.costcentre_id, s.is_contractor, s.tfn, s.provider_number, 
                         s.is_fired, s.is_commission, s.commission_percent, 
                         s.is_stakeholder, s.is_master_admin, s.is_admin, s.is_principal, s.is_provider, s.is_external,
                         s.staff_date_added,  s.start_date, s.end_date, s.comment, 
                         s.num_days_to_display_on_booking_screen,
                         s.show_header_on_booking_screen,
                         s.bk_screen_field_id,
                         s.bk_screen_show_key,
                         s.enable_daily_reminder_sms, 
                         s.enable_daily_reminder_email,

                         sr.field_id as field_field_id,sr.descr as field_descr,

                         " + PersonDB.GetFields("", "p").Replace("p.entity_id", "p.entity_id AS person_entity_id") + @", 
                         t.title_id, t.descr
                       FROM
                         RegisterStaff AS r 
                         LEFT OUTER JOIN Staff  s  ON r.staff_id  = s.staff_id 
                         LEFT OUTER JOIN Person p  ON s.person_id = p.person_id
                         LEFT OUTER JOIN Title  t  ON t.title_id  = p.title_id
                         LEFT OUTER  JOIN Field sr ON s.field_id  = sr.field_id
                       WHERE
                         s.staff_id > 0 AND s.is_fired = 0 " + (inc_deleted ? "" : " AND r.is_deleted = 0 ") + (!only_providers ? "" : " AND s.is_provider = 1 ") + (!excl_external ? "" : " AND s.is_external = 0 ") + " AND r.organisation_id = " + organistion_id.ToString() + @" 
                       ORDER BY 
                         p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static DataTable GetDataTable_StaffOf(int[] organistion_ids, bool inc_deleted = false, bool only_providers = false, bool excl_external = true)
    {
        if (organistion_ids == null || organistion_ids.Length == 0)
            organistion_ids = new int[] { 0 };

        string sql = @"SELECT DISTINCT
                         s.staff_id, s.person_id, s.login, s.pwd, s.staff_position_id, s.field_id, s.costcentre_id, s.is_contractor, s.tfn, s.provider_number, 
                         s.is_fired, s.is_commission, s.commission_percent, 
                         s.is_stakeholder, s.is_master_admin, s.is_admin, s.is_principal, s.is_provider, s.is_external,
                         s.staff_date_added,  s.start_date, s.end_date, s.comment, 
                         s.num_days_to_display_on_booking_screen,
                         s.show_header_on_booking_screen,
                         s.bk_screen_field_id,
                         s.bk_screen_show_key,
                         s.enable_daily_reminder_sms, 
                         s.enable_daily_reminder_email,

                         sr.field_id as field_field_id,sr.descr as field_descr,

                         " + PersonDB.GetFields("", "p").Replace("p.entity_id", "p.entity_id AS person_entity_id") + @", 
                         t.title_id, t.descr
                       FROM
                         RegisterStaff AS r 
                         LEFT OUTER JOIN Staff  s  ON r.staff_id  = s.staff_id 
                         LEFT OUTER JOIN Person p  ON s.person_id = p.person_id
                         LEFT OUTER JOIN Title  t  ON t.title_id  = p.title_id
                         LEFT OUTER  JOIN Field sr ON s.field_id  = sr.field_id
                       WHERE
                         s.staff_id > 0 AND s.is_fired = 0 " + (inc_deleted ? "" : " AND r.is_deleted = 0 ") + (!only_providers ? "" : " AND s.is_provider = 1 ") + (!excl_external ? "" : " AND s.is_external = 0 ") + " AND r.organisation_id IN (" + string.Join(",", organistion_ids) + @") 
                       ORDER BY 
                         p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Staff[] GetStaffOf(int organistion_id, bool providersOnly = false)
    {
        DataTable tbl = GetDataTable_StaffOf(organistion_id);

        ArrayList list = new ArrayList();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Staff staff = StaffDB.Load(tbl.Rows[i]);
            staff.Person = PersonDB.Load(tbl.Rows[i], "", "person_entity_id");
            staff.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
            staff.Field = IDandDescrDB.Load(tbl.Rows[i], "field_field_id", "field_descr");

            if (staff.IsProvider || !providersOnly)
                list.Add(staff);
        }
        return (Staff[])list.ToArray(typeof(Staff));
    }
    public static Staff[] GetStaffOf(int[] organistion_ids, bool providersOnly = false)
    {
        DataTable tbl = GetDataTable_StaffOf(organistion_ids);

        ArrayList list = new ArrayList();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Staff staff = StaffDB.Load(tbl.Rows[i]);
            staff.Person = PersonDB.Load(tbl.Rows[i], "", "person_entity_id");
            staff.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
            staff.Field = IDandDescrDB.Load(tbl.Rows[i], "field_field_id", "field_descr");

            if (staff.IsProvider || !providersOnly)
                list.Add(staff);
        }
        return (Staff[])list.ToArray(typeof(Staff));
    }


    // returns 2d hashtable
    // get by:  hash[new Hashtable2D.Key(staffID, orgID)]
    public static Hashtable Get2DHashByStaffIDOrgID(int[] staff_ids = null)
    {
        string sql = JoinedSql + (staff_ids != null && staff_ids.Length > 0 ? @" AND r.staff_id IN(" + string.Join(",", staff_ids) + ")" : "" );
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            RegisterStaff rr = Load(tbl.Rows[i], "registration_provider_number");
            rr.Staff = StaffDB.Load(tbl.Rows[i], "staff_");
            rr.Staff.Person = PersonDB.Load(tbl.Rows[i], "", "person_entity_id");
            rr.Organisation = OrganisationDB.Load(tbl.Rows[i], "organisation_");
            hash[new Hashtable2D.Key(rr.Staff.StaffID, rr.Organisation.OrganisationID)] = rr;
        }

        return hash;
    }

    // returns 2d hashtable
    // get by:  hash[new Hashtable2D.Key(staffID, (int)DayOfWeek.Sunday)]
    public static Hashtable Get2DHashByStaffIDDayID(int[] staff_ids = null)
    {
        string sql = JoinedSql + (staff_ids != null && staff_ids.Length > 0 ? @" AND r.staff_id IN(" + string.Join(",", staff_ids) + ")" : "");
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            RegisterStaff rr = Load(tbl.Rows[i], "registration_provider_number");
            rr.Staff = StaffDB.Load(tbl.Rows[i], "staff_");
            rr.Staff.Person = PersonDB.Load(tbl.Rows[i], "", "person_entity_id");
            rr.Organisation = OrganisationDB.Load(tbl.Rows[i], "organisation_");

            if (!rr.ExclSun) AddToStaffIDDayIDHash(ref hash, ref rr, DayOfWeek.Sunday);
            if (!rr.ExclMon) AddToStaffIDDayIDHash(ref hash, ref rr, DayOfWeek.Monday);
            if (!rr.ExclTue) AddToStaffIDDayIDHash(ref hash, ref rr, DayOfWeek.Tuesday);
            if (!rr.ExclWed) AddToStaffIDDayIDHash(ref hash, ref rr, DayOfWeek.Wednesday);
            if (!rr.ExclThu) AddToStaffIDDayIDHash(ref hash, ref rr, DayOfWeek.Thursday);
            if (!rr.ExclFri) AddToStaffIDDayIDHash(ref hash, ref rr, DayOfWeek.Friday);
            if (!rr.ExclSat) AddToStaffIDDayIDHash(ref hash, ref rr, DayOfWeek.Saturday);
        }

        return hash;
    }
    protected static void AddToStaffIDDayIDHash(ref Hashtable hash, ref RegisterStaff rr, DayOfWeek dayOfWeek)
    {
        if (hash[new Hashtable2D.Key(rr.Staff.StaffID, (int)dayOfWeek)] == null)
            hash[new Hashtable2D.Key(rr.Staff.StaffID, (int)dayOfWeek)] = new ArrayList();
        ((ArrayList)hash[new Hashtable2D.Key(rr.Staff.StaffID, (int)dayOfWeek)]).Add(rr);
    }


    public static DataTable GetDataTable_All()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static RegisterStaff[] GetAll()
    {
        DataTable tbl = GetDataTable_All();

        RegisterStaff[] list = new RegisterStaff[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = RegisterStaffDB.Load(tbl.Rows[i]);

            list[i].Staff = StaffDB.Load(tbl.Rows[i], "staff_");
            list[i].Staff.Person = PersonDB.Load(tbl.Rows[i], "", "person_entity_id");
            list[i].Staff.Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");

            list[i].Organisation = OrganisationDB.Load(tbl.Rows[i], "organisation_");
        }

        return list;
    }

    protected static string JoinedSql = @"
                    SELECT
                        r.register_staff_id,r.register_staff_date_added,r.organisation_id,r.staff_id,r.provider_number AS registration_provider_number,
                        r.excl_sun, r.excl_mon, r.excl_tue, r.excl_wed, r.excl_thu, r.excl_fri, r.excl_sat, r.main_provider_for_clinic,

                        -- o.organisation_id, o.entity_id as organisation_entity_id, o.parent_organisation_id, o.use_parent_offernig_prices, o.organisation_type_id, o.organisation_customer_type_id, o.name, o.acn, o.abn, o.organisation_date_added, o.organisation_date_modified, o.is_debtor, o.is_creditor, o.bpay_account, 
                        -- o.weeks_per_service_cycle, o.start_date, o.end_date, o.comment, o.free_services, o.excl_sun, o.excl_mon, o.excl_tue, o.excl_wed, o.excl_thu, o.excl_fri, o.excl_sat, 
                        -- o.sun_start_time, o.sun_end_time, o.mon_start_time, o.mon_end_time, o.tue_start_time, o.tue_end_time, o.wed_start_time, o.wed_end_time, o.thu_start_time, o.thu_end_time, o.fri_start_time, o.fri_end_time, o.sat_start_time, o.sat_end_time, 
                        -- o.sun_lunch_start_time, o.sun_lunch_end_time, o.mon_lunch_start_time, o.mon_lunch_end_time, o.tue_lunch_start_time, o.tue_lunch_end_time, o.wed_lunch_start_time, o.wed_lunch_end_time, o.thu_lunch_start_time, o.thu_lunch_end_time, o.fri_lunch_start_time, o.fri_lunch_end_time, o.sat_lunch_start_time, o.sat_lunch_end_time, 
                        -- o.last_batch_run,

                        o.organisation_id as organisation_organisation_id, o.entity_id as organisation_entity_id, o.parent_organisation_id as organisation_parent_organisation_id, o.use_parent_offernig_prices as organisation_use_parent_offernig_prices, 
                        o.organisation_type_id as organisation_organisation_type_id, o.organisation_customer_type_id as organisation_organisation_customer_type_id, o.name as organisation_name, o.acn as organisation_acn, o.abn as organisation_abn, 
                        o.organisation_date_added as organisation_organisation_date_added, o.organisation_date_modified as organisation_organisation_date_modified, o.is_debtor as organisation_is_debtor, o.is_creditor as organisation_is_creditor, 
                        o.bpay_account as organisation_bpay_account, o.weeks_per_service_cycle as organisation_weeks_per_service_cycle, o.start_date as organisation_start_date, 
                        o.end_date as organisation_end_date, o.comment as organisation_comment, o.free_services as organisation_free_services, o.excl_sun as organisation_excl_sun, 
                        o.excl_mon as organisation_excl_mon, o.excl_tue as organisation_excl_tue, o.excl_wed as organisation_excl_wed, o.excl_thu as organisation_excl_thu, 
                        o.excl_fri as organisation_excl_fri, o.excl_sat as organisation_excl_sat, 
                        o.sun_start_time as organisation_sun_start_time, o.sun_end_time as organisation_sun_end_time, 
                        o.mon_start_time as organisation_mon_start_time, o.mon_end_time as organisation_mon_end_time, o.tue_start_time as organisation_tue_start_time, 
                        o.tue_end_time as organisation_tue_end_time, o.wed_start_time as organisation_wed_start_time, o.wed_end_time as organisation_wed_end_time, 
                        o.thu_start_time as organisation_thu_start_time, o.thu_end_time as organisation_thu_end_time, o.fri_start_time as organisation_fri_start_time, 
                        o.fri_end_time as organisation_fri_end_time, o.sat_start_time as organisation_sat_start_time, o.sat_end_time as organisation_sat_end_time, 
                        o.sun_lunch_start_time as organisation_sun_lunch_start_time, o.sun_lunch_end_time as organisation_sun_lunch_end_time, 
                        o.mon_lunch_start_time as organisation_mon_lunch_start_time, o.mon_lunch_end_time as organisation_mon_lunch_end_time, o.tue_lunch_start_time as organisation_tue_lunch_start_time, 
                        o.tue_lunch_end_time as organisation_tue_lunch_end_time, o.wed_lunch_start_time as organisation_wed_lunch_start_time, o.wed_lunch_end_time as organisation_wed_lunch_end_time, 
                        o.thu_lunch_start_time as organisation_thu_lunch_start_time, o.thu_lunch_end_time as organisation_thu_lunch_end_time, o.fri_lunch_start_time as organisation_fri_lunch_start_time, 
                        o.fri_lunch_end_time as organisation_fri_lunch_end_time, o.sat_lunch_start_time as organisation_sat_lunch_start_time, o.sat_lunch_end_time as organisation_sat_lunch_end_time, 
                        o.last_batch_run as organisation_last_batch_run, o.is_deleted as organisation_is_deleted,


                        s.staff_id as staff_staff_id,s.person_id as staff_person_id,s.login as staff_login,s.pwd as staff_pwd,s.staff_position_id as staff_staff_position_id,
                        s.field_id as staff_field_id,s.is_contractor as staff_is_contractor,s.tfn as staff_tfn,
                        s.is_fired as staff_is_fired,s.costcentre_id as staff_costcentre_id,
                        s.is_admin as staff_is_admin,s.provider_number as staff_provider_number,s.is_commission as staff_is_commission,s.commission_percent as staff_commission_percent,
                        s.is_stakeholder as staff_is_stakeholder,s.is_master_admin as staff_is_master_admin,s.is_admin as staff_is_admin,s.is_principal as staff_is_principal,s.is_provider as staff_is_provider, s.is_external as staff_is_external,
                        s.staff_date_added as staff_staff_date_added,s.start_date as staff_start_date,s.end_date as staff_end_date,s.comment as staff_comment,
                        s.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen, 
                        s.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                        s.bk_screen_field_id as staff_bk_screen_field_id,
                        s.bk_screen_show_key as staff_bk_screen_show_key,
                        s.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                        s.enable_daily_reminder_email as staff_enable_daily_reminder_email,


                        " + PersonDB.GetFields("", "p").Replace("p.entity_id", "p.entity_id AS person_entity_id") + @", 
                        t.title_id, t.descr,

                        sr.field_id as field_field_id,sr.descr as field_descr

                    FROM
                         RegisterStaff AS r 
                         LEFT OUTER JOIN Organisation o  ON r.organisation_id = o.organisation_id 
                         LEFT OUTER JOIN Staff        s  ON r.staff_id        = s.staff_id 
                         LEFT OUTER JOIN Person       p  ON s.person_id       = p.person_id
                         LEFT OUTER JOIN Title        t  ON t.title_id        = p.title_id
                         LEFT OUTER  JOIN Field        sr ON s.field_id        = sr.field_id

                    WHERE
                         s.staff_id > 0 AND s.is_fired = 0 AND r.is_deleted = 0 ";


    public static DataTable GetDataTable_WorkingStaffOf(int organistion_id, DateTime date)
    {
        string sql = @"SELECT Distinct
                         --r.register_staff_id,r.register_staff_date_added,r.organisation_id,r.provider_number AS registration_provider_number,
                         --r.excl_sun, r.excl_mon, r.excl_tue, r.excl_wed, r.excl_thu, r.excl_fri, r.excl_sat, r.main_provider_for_clinic,
                         s.staff_id, s.person_id, s.login, s.pwd, s.staff_position_id, s.field_id, s.costcentre_id, s.is_contractor, s.tfn, s.provider_number, 
                         s.is_fired, s.is_commission, s.commission_percent, s.is_stakeholder, s.is_master_admin, s.is_admin, s.is_principal, s.is_provider, s.is_external,
                         s.staff_date_added, s.start_date, s.end_date, s.comment, 
                         s.num_days_to_display_on_booking_screen, 
                         s.show_header_on_booking_screen,
                         s.bk_screen_field_id,
                         s.bk_screen_show_key,
                         s.enable_daily_reminder_sms, 
                         s.enable_daily_reminder_email,

                         " + PersonDB.GetFields("", "p").Replace("p.entity_id", "p.entity_id AS person_entity_id") + @", 
                         t.title_id, t.descr
                       FROM
                         RegisterStaff AS r 
                         LEFT OUTER JOIN Staff  s  ON r.staff_id  = s.staff_id 
                         LEFT OUTER JOIN Person p  ON s.person_id = p.person_id
                         LEFT OUTER JOIN Title  t  ON t.title_id  = p.title_id
                       WHERE
                         s.staff_id > 0 AND s.is_fired = 0 AND r.is_deleted = 0 AND r.organisation_id = " + organistion_id + @" 

                         -- get unavail bookings for whole day .... and make sure there are none
                         AND (SELECT COUNT(*) FROM Booking 
                               WHERE
                                     (
                                      (booking_type_id = 341 AND (organisation_id IS NULL OR organisation_id = r.organisation_id)  AND ( (is_recurring = 0 AND CONVERT(TIME,Booking.date_start) = '00:00' AND CONVERT(TIME,Booking.date_end) >= '23:59') OR (is_recurring = 1 AND recurring_start_time = '00:00' AND recurring_end_time >= '23:59')   )) OR
                                      (booking_type_id = 342 AND (organisation_id = r.organisation_id)                             AND ( (is_recurring = 0 AND CONVERT(TIME,Booking.date_start) = '00:00' AND CONVERT(TIME,Booking.date_end) >= '23:59') OR (is_recurring = 1 AND recurring_start_time = '00:00' AND recurring_end_time >= '23:59')   ))
                                     ) 

                                     AND

                                     (date_deleted IS  NULL) 

                                     AND

                                     (
                                      (is_recurring = 0 AND (SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, Booking.date_start)))  = '" + date.ToString("yyyy-MM-dd") + " 00:00:00" + @"') OR

                                      (is_recurring = 1 AND (SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, Booking.date_start))) <= '" + date.ToString("yyyy-MM-dd") + " 00:00:00" + @"'    
                                                        AND (SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, Booking.date_end)))   >= '" + date.ToString("yyyy-MM-dd") + " 00:00:00" + @"'
                                                        AND (Booking.recurring_weekday_id = " +  WeekDayDB.GetWeekDayID(date.DayOfWeek) + @"))
                                     )

                                     AND

                                     (provider = r.staff_id) ) = 0

                         AND (SELECT COUNT(*) FROM Staff WHERE (staff_id = r.staff_id) AND (excl_" +date.DayOfWeek.ToString().Substring(0, 3).ToLower()+@" = 0)) > 0

                       ORDER BY 
                         p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Staff[] GetWorkingStaffOf(int organistion_id, DateTime date)
    {
        DataTable tbl = GetDataTable_WorkingStaffOf(organistion_id, date);
        Staff[] list = new Staff[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = StaffDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i], "", "person_entity_id");
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
        }
        return list;
    }






    public static RegisterStaff Load(DataRow row, string providerNumberColumnName = "provider_number")
    {
        return new RegisterStaff(
            Convert.ToInt32(row["register_staff_id"]),
            Convert.ToDateTime(row["register_staff_date_added"]),
            Convert.ToInt32(row["organisation_id"]),
            Convert.ToInt32(row["staff_id"]),
            Convert.ToString(row[providerNumberColumnName]),
            Convert.ToBoolean(row["main_provider_for_clinic"]),
            Convert.ToBoolean(row["excl_sun"]),
            Convert.ToBoolean(row["excl_mon"]),
            Convert.ToBoolean(row["excl_tue"]),
            Convert.ToBoolean(row["excl_wed"]),
            Convert.ToBoolean(row["excl_thu"]),
            Convert.ToBoolean(row["excl_fri"]),
            Convert.ToBoolean(row["excl_sat"])
        );
    }

}