using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class OrganisationDB
{

    public static void Delete(int organisation_id)
    {
        try
        {
            Organisation o = OrganisationDB.GetByID(organisation_id);
            if (o != null)
            {
                DBBase.ExecuteNonResult("DELETE FROM Organisation WHERE organisation_id = " + organisation_id.ToString());
                if (EntityDB.NumForeignKeyDependencies(o.EntityID) == 0)
                    EntityDB.Delete(o.EntityID, false);
            }
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int parent_organisation_id, bool use_parent_offernig_prices, int organisation_type_id, int organisation_customer_type_id, string name, string acn, string abn, bool is_debtor, bool is_creditor, string bpay_account, int weeks_per_service_cycle, DateTime start_date, DateTime end_date, string comment, int free_services, bool excl_sun, bool excl_mon, bool excl_tue, bool excl_wed, bool excl_thu, bool excl_fri, bool excl_sat, TimeSpan sun_start_time, TimeSpan sun_end_time, TimeSpan mon_start_time, TimeSpan mon_end_time, TimeSpan tue_start_time, TimeSpan tue_end_time, TimeSpan wed_start_time, TimeSpan wed_end_time, TimeSpan thu_start_time, TimeSpan thu_end_time, TimeSpan fri_start_time, TimeSpan fri_end_time, TimeSpan sat_start_time, TimeSpan sat_end_time, TimeSpan sun_lunch_start_time, TimeSpan sun_lunch_end_time, TimeSpan mon_lunch_start_time, TimeSpan mon_lunch_end_time, TimeSpan tue_lunch_start_time, TimeSpan tue_lunch_end_time, TimeSpan wed_lunch_start_time, TimeSpan wed_lunch_end_time, TimeSpan thu_lunch_start_time, TimeSpan thu_lunch_end_time, TimeSpan fri_lunch_start_time, TimeSpan fri_lunch_end_time, TimeSpan sat_lunch_start_time, TimeSpan sat_lunch_end_time, DateTime last_batch_run)
    {
        int entityID = EntityDB.Insert();

        name = name.Replace("'", "''");
        acn = acn.Replace("'", "''");
        abn = abn.Replace("'", "''");
        bpay_account = bpay_account.Replace("'", "''");
        comment = comment.Replace("'", "''");
        string sql = "INSERT INTO Organisation (entity_id,parent_organisation_id,use_parent_offernig_prices,organisation_type_id,organisation_customer_type_id,name,acn,abn,is_debtor,is_creditor,bpay_account,weeks_per_service_cycle,start_date,end_date,comment,free_services,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat,sun_start_time,sun_end_time,mon_start_time,mon_end_time,tue_start_time,tue_end_time,wed_start_time,wed_end_time,thu_start_time,thu_end_time,fri_start_time,fri_end_time,sat_start_time,sat_end_time,sun_lunch_start_time,sun_lunch_end_time,mon_lunch_start_time,mon_lunch_end_time,tue_lunch_start_time,tue_lunch_end_time,wed_lunch_start_time,wed_lunch_end_time,thu_lunch_start_time,thu_lunch_end_time,fri_lunch_start_time,fri_lunch_end_time,sat_lunch_start_time,sat_lunch_end_time,last_batch_run) VALUES (" + entityID.ToString() + "," + (parent_organisation_id == 0 ? "NULL" : parent_organisation_id.ToString()) + "," + (use_parent_offernig_prices ? "1" : "0") + "," + organisation_type_id + "," + organisation_customer_type_id + ",'" + name + "'," + "'" + acn + "'," + "'" + abn + "'," + (is_debtor ? "1," : "0,") + (is_creditor ? "1," : "0,") + "'" + bpay_account + "'," + weeks_per_service_cycle + "," + (start_date == DateTime.MinValue ? "NULL" : "'" + start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (end_date == DateTime.MinValue ? "NULL" : "'" + end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + "'" + comment + "'," + "" + free_services + "," + (excl_sun ? "1," : "0,") + (excl_mon ? "1," : "0,") + (excl_tue ? "1," : "0,") + (excl_wed ? "1," : "0,") + (excl_thu ? "1," : "0,") + (excl_fri ? "1," : "0,") + (excl_sat ? "1," : "0,") + "'" + sun_start_time.ToString() + "'," + "'" + sun_end_time.ToString() + "'," + "'" + mon_start_time.ToString() + "'," + "'" + mon_end_time.ToString() + "'," + "'" + tue_start_time.ToString() + "'," + "'" + tue_end_time.ToString() + "'," + "'" + wed_start_time.ToString() + "'," + "'" + wed_end_time.ToString() + "'," + "'" + thu_start_time.ToString() + "'," + "'" + thu_end_time.ToString() + "'," + "'" + fri_start_time.ToString() + "'," + "'" + fri_end_time.ToString() + "'," + "'" + sat_start_time.ToString() + "'," + "'" + sat_end_time.ToString() + "'," + "'" + sun_lunch_start_time.ToString() + "'," + "'" + sun_lunch_end_time.ToString() + "'," + "'" + mon_lunch_start_time.ToString() + "'," + "'" + mon_lunch_end_time.ToString() + "'," + "'" + tue_lunch_start_time.ToString() + "'," + "'" + tue_lunch_end_time.ToString() + "'," + "'" + wed_lunch_start_time.ToString() + "'," + "'" + wed_lunch_end_time.ToString() + "'," + "'" + thu_lunch_start_time.ToString() + "'," + "'" + thu_lunch_end_time.ToString() + "'," + "'" + fri_lunch_start_time.ToString() + "'," + "'" + fri_lunch_end_time.ToString() + "'," + "'" + sat_lunch_start_time.ToString() + "'," + "'" + sat_lunch_end_time.ToString() + "'," + (last_batch_run == DateTime.MinValue ? "NULL" : "'" + last_batch_run.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static int InsertExtOrg(int organisation_type_id, string name, string acn, string abn, bool is_debtor, bool is_creditor, string bpay_account, string comment)
    {
        return Insert(0, false, organisation_type_id, 0, name, acn, abn, is_debtor, is_creditor, bpay_account, 0, DateTime.MinValue, DateTime.MinValue, comment, 0, false, false, false, false, false, false, false, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, DateTime.MinValue);
    }
    public static void Update(int organisation_id, int parent_organisation_id, bool use_parent_offernig_prices, int organisation_type_id, int organisation_customer_type_id, string name, string acn, string abn, bool is_debtor, bool is_creditor, string bpay_account, int weeks_per_service_cycle, DateTime start_date, DateTime end_date, string comment, int free_services, bool excl_sun, bool excl_mon, bool excl_tue, bool excl_wed, bool excl_thu, bool excl_fri, bool excl_sat, TimeSpan sun_start_time, TimeSpan sun_end_time, TimeSpan mon_start_time, TimeSpan mon_end_time, TimeSpan tue_start_time, TimeSpan tue_end_time, TimeSpan wed_start_time, TimeSpan wed_end_time, TimeSpan thu_start_time, TimeSpan thu_end_time, TimeSpan fri_start_time, TimeSpan fri_end_time, TimeSpan sat_start_time, TimeSpan sat_end_time, TimeSpan sun_lunch_start_time, TimeSpan sun_lunch_end_time, TimeSpan mon_lunch_start_time, TimeSpan mon_lunch_end_time, TimeSpan tue_lunch_start_time, TimeSpan tue_lunch_end_time, TimeSpan wed_lunch_start_time, TimeSpan wed_lunch_end_time, TimeSpan thu_lunch_start_time, TimeSpan thu_lunch_end_time, TimeSpan fri_lunch_start_time, TimeSpan fri_lunch_end_time, TimeSpan sat_lunch_start_time, TimeSpan sat_lunch_end_time, DateTime last_batch_run)
    {
        name = name.Replace("'", "''");
        acn = acn.Replace("'", "''");
        abn = abn.Replace("'", "''");
        bpay_account = bpay_account.Replace("'", "''");
        comment = comment.Replace("'", "''");
        string sql = "UPDATE Organisation SET parent_organisation_id = " + (parent_organisation_id == 0 ? "NULL" : parent_organisation_id.ToString()) + ",use_parent_offernig_prices = " + (use_parent_offernig_prices ? "1" : "0") + ",organisation_type_id = " + organisation_type_id + ",organisation_customer_type_id = " + organisation_customer_type_id + ",name = '" + name + "',acn = '" + acn + "',abn = '" + abn + "',organisation_date_modified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',is_debtor = " + (is_debtor ? "1," : "0,") + "is_creditor = " + (is_creditor ? "1," : "0,") + "bpay_account = '" + bpay_account + "',weeks_per_service_cycle = " + weeks_per_service_cycle + "," + "start_date = " + (start_date == DateTime.MinValue ? "NULL" : "'" + start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",end_date = " + (end_date == DateTime.MinValue ? "NULL" : "'" + end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",comment = '" + comment + "',free_services = " + free_services + ",excl_sun = " + (excl_sun ? "1," : "0,") + "excl_mon = " + (excl_mon ? "1," : "0,") + "excl_tue = " + (excl_tue ? "1," : "0,") + "excl_wed = " + (excl_wed ? "1," : "0,") + "excl_thu = " + (excl_thu ? "1," : "0,") + "excl_fri = " + (excl_fri ? "1," : "0,") + "excl_sat = " + (excl_sat ? "1," : "0,") + "sun_start_time = '" + sun_start_time.ToString() + "',sun_end_time = '" + sun_end_time.ToString() + "',mon_start_time = '" + mon_start_time.ToString() + "',mon_end_time = '" + mon_end_time.ToString() + "',tue_start_time = '" + tue_start_time.ToString() + "',tue_end_time = '" + tue_end_time.ToString() + "',wed_start_time = '" + wed_start_time.ToString() + "',wed_end_time = '" + wed_end_time.ToString() + "',thu_start_time = '" + thu_start_time.ToString() + "',thu_end_time = '" + thu_end_time.ToString() + "',fri_start_time = '" + fri_start_time.ToString() + "',fri_end_time = '" + fri_end_time.ToString() + "',sat_start_time = '" + sat_start_time.ToString() + "',sat_end_time = '" + sat_end_time.ToString() + "',sun_lunch_start_time = '" + sun_lunch_start_time.ToString() + "',sun_lunch_end_time = '" + sun_lunch_end_time.ToString() + "',mon_lunch_start_time = '" + mon_lunch_start_time.ToString() + "',mon_lunch_end_time = '" + mon_lunch_end_time.ToString() + "',tue_lunch_start_time = '" + tue_lunch_start_time.ToString() + "',tue_lunch_end_time = '" + tue_lunch_end_time.ToString() + "',wed_lunch_start_time = '" + wed_lunch_start_time.ToString() + "',wed_lunch_end_time = '" + wed_lunch_end_time.ToString() + "',thu_lunch_start_time = '" + thu_lunch_start_time.ToString() + "',thu_lunch_end_time = '" + thu_lunch_end_time.ToString() + "',fri_lunch_start_time = '" + fri_lunch_start_time.ToString() + "',fri_lunch_end_time = '" + fri_lunch_end_time.ToString() + "',sat_lunch_start_time = '" + sat_lunch_start_time.ToString() + "',sat_lunch_end_time = '" + sat_lunch_end_time.ToString() + "',last_batch_run = " + (last_batch_run == DateTime.MinValue ? "NULL" : "'" + last_batch_run.ToString("yyyy-MM-dd HH:mm:ss") + "'") + " WHERE organisation_id = " + organisation_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateExtOrg(int organisation_id, int organisation_type_id, string name, string acn, string abn, DateTime organisation_date_modified, bool is_debtor, bool is_creditor, string bpay_account, string comment)
    {
        name = name.Replace("'", "''");
        acn = acn.Replace("'", "''");
        abn = abn.Replace("'", "''");
        bpay_account = bpay_account.Replace("'", "''");
        comment = comment.Replace("'", "''");
        string sql = "UPDATE Organisation SET organisation_type_id = " + organisation_type_id + ",name = '" + name + "',acn = '" + acn + "',abn = '" + abn + "',organisation_date_modified = '" + organisation_date_modified.ToString("yyyy-MM-dd HH:mm:ss") + "',is_debtor = " + (is_debtor ? "1," : "0,") + "is_creditor = " + (is_creditor ? "1," : "0,") + "bpay_account = '" + bpay_account + "',comment = '" + comment + "'" + " WHERE organisation_id = " + organisation_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int organisation_id)
    {
        string sql = "UPDATE Organisation SET is_deleted = 1 WHERE organisation_id = " + organisation_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int organisation_id)
    {
        string sql = "UPDATE Organisation SET is_deleted = 0 WHERE organisation_id = " + organisation_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    
    public static int GetCountByEntityID(int entity_id)
    {
        string sql = "SELECT COUNT(*) FROM Organisation WHERE entity_id = " + entity_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static bool Exists(int organisation_id)
    {
        string sql = "SELECT COUNT(*) FROM Organisation WHERE organisation_id = " + organisation_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }
    public static bool Exists(string organisation_ids)
    {
        string sql = @"SELECT organisation_id
                       FROM   ORGANISATION
                       WHERE  organisation_id IN (" + organisation_ids + @")";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        System.Collections.Hashtable eistingOrgs = new System.Collections.Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
            eistingOrgs[Convert.ToInt32(tbl.Rows[i]["organisation_id"])] = 1;

        foreach (string str_id in organisation_ids.Split(','))
            if (eistingOrgs[Convert.ToInt32(str_id)] == null)
                return false;

        return true;
    }

    public static DataTable GetStats(DateTime start, DateTime end, int organisation_type_group_id, bool incDeleted)
    {
        string sql = @"

            SELECT organisation_id, entity_id, name,

               (SELECT    COUNT(*)
                 FROM     Booking
                 WHERE    organisation_id = Organisation.organisation_id
                          " + (start == DateTime.MinValue ? "" : " AND date_created >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          " + (end   == DateTime.MinValue ? "" : " AND date_created <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @"
                          AND booking_type_id = 34 AND booking_status_id IN (0, 187, 188)) AS n_bookings,

               (SELECT    COUNT(DISTINCT Patient.patient_id)
                 FROM     RegisterPatient 
                          INNER JOIN Patient ON RegisterPatient.patient_id = Patient.patient_id AND DATEDIFF(DAY, Patient.patient_date_added, RegisterPatient.register_patient_date_added) < 14
                 WHERE    RegisterPatient.is_deleted = 0 AND RegisterPatient.organisation_id = Organisation.organisation_id " +
                          (start == DateTime.MinValue ? "" : " AND RegisterPatient.register_patient_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND RegisterPatient.register_patient_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @") AS n_patients,

               (SELECT    COUNT(*)
                 FROM     Booking
                 WHERE    organisation_id = Organisation.organisation_id
                          " + (start == DateTime.MinValue ? "" : " AND date_start >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          " + (end   == DateTime.MinValue ? "" : " AND date_start <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @"
                          AND booking_type_id = 34 AND booking_status_id IN (0, 187, 188)) AS total_bookings,

               (SELECT    SUM(datediff(minute,date_start, date_end))/COUNT(*)
                 FROM     Booking
                 WHERE    organisation_id = Organisation.organisation_id
                          " + (start == DateTime.MinValue ? "" : " AND date_start >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          " + (end == DateTime.MinValue ? "" : " AND date_start <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          AND booking_type_id = 34 AND booking_status_id IN (0, 187, 188)) AS avg_minutes


            FROM Organisation
                 INNER JOIN OrganisationType ON Organisation.organisation_type_id = OrganisationType.organisation_type_id 
            WHERE OrganisationType.organisation_type_group_id IN (5,6) " + (incDeleted ? "" : @" AND Organisation.is_deleted = 0") + @"
            ORDER BY n_patients DESC"; // ORDER BY Organisation.name";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return tbl;
    }



    private static string JoinedSql = @"SELECT  
                                                o.organisation_id,o.entity_id,o.parent_organisation_id,o.use_parent_offernig_prices,parent.name as parent_name, o.organisation_type_id,o.organisation_customer_type_id,o.name,o.acn,o.abn,o.organisation_date_added,o.organisation_date_modified,o.is_debtor,o.is_creditor,o.bpay_account,o.weeks_per_service_cycle,o.start_date,o.end_date,o.comment,o.free_services,
                                                o.excl_sun,o.excl_mon,o.excl_tue,o.excl_wed,o.excl_thu,o.excl_fri,o.excl_sat,
                                                o.sun_start_time,o.sun_end_time,o.mon_start_time,o.mon_end_time,o.tue_start_time,o.tue_end_time,o.wed_start_time,o.wed_end_time,o.thu_start_time,o.thu_end_time,o.fri_start_time,o.fri_end_time,o.sat_start_time,o.sat_end_time,o.last_batch_run,
                                                o.sun_lunch_start_time,o.sun_lunch_end_time,o.mon_lunch_start_time,o.mon_lunch_end_time,o.tue_lunch_start_time,o.tue_lunch_end_time,o.wed_lunch_start_time,o.wed_lunch_end_time,o.thu_lunch_start_time,o.thu_lunch_end_time,o.fri_lunch_start_time,o.fri_lunch_end_time,o.sat_lunch_start_time,o.sat_lunch_end_time,o.is_deleted,
                                                type.organisation_type_id as type_organisation_type_id,type.descr as type_descr,type.organisation_type_group_id as type_organisation_type_group_id,
                                                typegroup.organisation_type_group_id as typegroup_organisation_type_group_id, typegroup.descr as typegroup_descr,
                                                ct.organisation_customer_type_id as ct_organisation_customer_type_id, ct.descr as ct_descr
                                        FROM    
                                                Organisation o
                                                LEFT OUTER JOIN Organisation             parent    ON o.parent_organisation_id = parent.organisation_id
                                                INNER JOIN      OrganisationType         type      ON o.organisation_type_id = type.organisation_type_id 
                                                INNER JOIN      OrganisationTypeGroup    typegroup ON type.organisation_type_group_id = typegroup.organisation_type_group_id 
                                                INNER JOIN      OrganisationCustomerType ct        ON o.organisation_customer_type_id = ct.organisation_customer_type_id ";


    public static string AddField(string sql, string fields)
    {
        string[] parts = sql.Split( new string[]{"FROM"}, StringSplitOptions.None );
        return parts[0] + "," + fields + " FROM " + parts[1];
    }
    public static DataTable GetDataTable(int org_id = 0, bool showDeleted = false, bool exclGroupOrg = true, bool exclClinics = false, bool exclAgedCareFacs = false, bool exclIns = true, bool exclExternal = false, string matchName = "", bool searchNameOnlyStartsWith = false, string org_type_ids = "")
    {
        matchName = matchName.Replace("'", "''");

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


        string sqlGroups   = JoinedSql + "  WHERE type.organisation_type_group_id IN (1)         " + (org_type_ids != null && org_type_ids.Length > 0 ? " AND o.organisation_type_id IN (" + org_type_ids + ") " : "");
        string sqlNoGroups = JoinedSql + "  WHERE type.organisation_type_group_id NOT IN (1,2,3) " + (org_type_ids != null && org_type_ids.Length > 0 ? " AND o.organisation_type_id IN (" + org_type_ids + ") " : "") + (showDeleted ? "" : " AND o.is_deleted = 0 ") + (org_id != 0 ? " AND o.organisation_id = " + org_id : "") + ((notInTypeGroupList.Length > 0) ? " AND type.organisation_type_group_id NOT IN (" + notInTypeGroupList + @") " : "") + ((matchName.Length > 0 && !searchNameOnlyStartsWith) ? " AND o.name LIKE '%" + matchName + "%'" : "") + ((matchName.Length > 0 && searchNameOnlyStartsWith) ? " AND o.name LIKE '" + matchName + "%'" : "");
        string sql = exclGroupOrg ? 
                        sqlNoGroups + " ORDER BY o.name" :
                        sql = sql = "SELECT * FROM (" + AddField(sqlGroups, "TMP_ORD = 0") + " UNION " + AddField(sqlNoGroups, "TMP_ORD = 1") + ") x ORDER BY TMP_ORD, name";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return tbl;
    }
    public static Organisation[] GetAll(bool showDeleted = false, bool exclGroupOrg = true, bool exclClinics = false, bool exclAgedCareFacs = false, bool exclIns = true, bool exclExternal = false, string matchName = "", bool searchNameOnlyStartsWith = false, string org_type_ids = "")
    {
        DataTable tbl = GetDataTable(0, showDeleted, exclGroupOrg, exclClinics, exclAgedCareFacs, exclIns, exclExternal, matchName, searchNameOnlyStartsWith, org_type_ids);

        Organisation[] list = new Organisation[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = Load(tbl.Rows[i]);

        return list;
    }

    public static Hashtable GetAllHashtable(bool showDeleted = false, bool exclGroupOrg = true, bool exclClinics = false, bool exclAgedCareFacs = false, bool exclIns = true, bool exclExternal = false, string matchName = "", bool searchNameOnlyStartsWith = false, string org_type_ids = "")
    {
        Organisation[] list = GetAll(showDeleted, exclGroupOrg, exclClinics, exclAgedCareFacs, exclIns, exclExternal, matchName, searchNameOnlyStartsWith, org_type_ids);

        Hashtable hash = new Hashtable();
        for (int i = 0; i < list.Length; i++)
            hash[list[i].OrganisationID] = list[i];

        return hash;
    }

    public static DataTable GetDataTable_GroupOrganisations()
    {
        string sql = JoinedSql + " WHERE o.organisation_id < 0 ";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return tbl;
    }


    public static DataTable GetDataTable_External(bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false, bool exclGroupOrg = true, string org_type_ids = "")
    {
        return GetDataTable_ByTypeGroup("4", showDeleted, matchNname, searchNameOnlyStartsWith, exclGroupOrg, org_type_ids);
    }
    public static DataTable GetDataTable_Clinics(bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false)
    {
        return GetDataTable_ByTypeGroup("5", showDeleted, matchNname, searchNameOnlyStartsWith);
    }
    public static DataTable GetDataTable_AgedCareFacs(bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false)
    {
        return GetDataTable_ByTypeGroup("6", showDeleted, matchNname, searchNameOnlyStartsWith);
    }
    public static DataTable GetDataTable_Insurance(bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false)
    {
        return GetDataTable_ByTypeGroup("7", showDeleted, matchNname, searchNameOnlyStartsWith);
    }
    public static DataTable GetDataTable_ByTypeGroup(string organisation_type_group_ids, bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false, bool exclGroupOrg = true, string org_type_ids = "")
    {
        matchNname = matchNname.Replace("'", "''");

        if (!exclGroupOrg)
            organisation_type_group_ids = (organisation_type_group_ids.Length > 0 ? "," : "") + "1,2";

        string sql = JoinedSql + " WHERE " + (showDeleted ? "" : " o.is_deleted = 0 AND ") + " type.organisation_type_group_id IN (" + organisation_type_group_ids + ") " + (org_type_ids != null && org_type_ids.Length > 0 ? " AND o.organisation_type_id IN (" + org_type_ids + ") " : "") + ((matchNname.Length > 0 && !searchNameOnlyStartsWith) ? " AND o.name LIKE '%" + matchNname + "%'" : "") + ((matchNname.Length > 0 && searchNameOnlyStartsWith) ? " AND o.name LIKE '" + matchNname + "%'" : "");
        sql += "  ORDER BY o.name";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }



    public static DataTable GetDataTable_ByType(int[] types, Organisation[] excList = null)
    {
        string notInList = string.Empty;
        if (excList != null)
            foreach (Organisation o in excList)
                notInList += o.OrganisationID.ToString() + ",";
        if (notInList.Length > 0)
            notInList = notInList.Substring(0, notInList.Length - 1);

        string sql = JoinedSql + " WHERE o.is_deleted = 0 AND type.organisation_type_id IN (" + string.Join(",", types) + ")" + (notInList.Length > 0 ? " AND o.organisation_id NOT IN (" + notInList + ")" : "");
        sql += "  ORDER BY o.name";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Organisation[] Get_ByType(int[] types, Organisation[] excList)
    {
        DataTable tbl = GetDataTable_ByType(types, excList);
        Organisation[] list = new Organisation[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = Load(tbl.Rows[i]);
            list[i].OrganisationType = OrganisationTypeDB.Load(tbl.Rows[0], "type_");
        }
        return list;
    }

    public static Organisation GetByID(int organisation_id)
    {
        string sql = JoinedSql + @"  WHERE o.organisation_id = " + organisation_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        if (tbl.Rows.Count == 0)
            return null;
        else
        {
            Organisation org = Load(tbl.Rows[0]);
            org.OrganisationType = OrganisationTypeDB.Load(tbl.Rows[0], "type_");
            return org;
        }
    }

    public static Organisation[] GetChildrenOf(int parent_organisation_id)
    {
        string sql = JoinedSql + @"  WHERE o.is_deleted = 0 AND o.parent_organisation_id = " + parent_organisation_id.ToString() + " ORDER BY o.name";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Organisation[] list = new Organisation[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = OrganisationDB.Load(tbl.Rows[i]);

        return list;
    }


    public static int GetEntityIDByOrganisationID(int organisationID)
    {
        string sql = "SELECT Organisation.entity_id FROM Organisation WHERE Organisation.organisation_id = " + organisationID;
        return (int)DBBase.ExecuteSingleResult(sql);
    }

    public static int[] GetInsuranceCompanyOrgIDs()
    {
        string sql_to_get_insurance_company_ids =
                    @"SELECT organisation_id 
                      FROM  Organisation 
                            LEFT OUTER JOIN OrganisationType ON Organisation.organisation_type_id = OrganisationType.organisation_type_id
                      WHERE OrganisationType.organisation_type_group_id = 7";

        DataTable tbl_ins_company_ids = DBBase.ExecuteQuery(sql_to_get_insurance_company_ids).Tables[0];

        int[] list = new int[tbl_ins_company_ids.Rows.Count];
        for (int i = 0; i < tbl_ins_company_ids.Rows.Count; i++)
            list[i] = Convert.ToInt32(tbl_ins_company_ids.Rows[i]["organisation_id"]);

        return list;
    }
    public static bool IsInsuranceCompanyOrg(int organisation_id)
    {
        return Array.IndexOf(OrganisationDB.GetInsuranceCompanyOrgIDs(), organisation_id) != -1;
    }


    public static DataTable GetDataTable_AllNotInc(Organisation[] excList, bool exclGroupOrg = true, bool exclClinics = false, bool exclAgedCareFacs = false, bool exclIns = true, bool exclExternal = false)
    {
        string notInList = string.Empty;
        foreach (Organisation o in excList)
            notInList += o.OrganisationID.ToString() + ",";
        if (notInList.Length > 0)
            notInList = notInList.Substring(0, notInList.Length - 1);

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


        string sqlGroups = @"SELECT 
                                    organisation_id,entity_id,parent_organisation_id,use_parent_offernig_prices,o.organisation_type_id,o.organisation_customer_type_id,name,acn,abn,organisation_date_added,organisation_date_modified,is_debtor,is_creditor,bpay_account,weeks_per_service_cycle,start_date,end_date,comment,free_services,
                                    excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat,
                                    sun_start_time,sun_end_time,mon_start_time,mon_end_time,tue_start_time,tue_end_time,wed_start_time,wed_end_time,thu_start_time,thu_end_time,fri_start_time,fri_end_time,sat_start_time,sat_end_time,
                                    sun_lunch_start_time,sun_lunch_end_time,mon_lunch_start_time,mon_lunch_end_time,tue_lunch_start_time,tue_lunch_end_time,wed_lunch_start_time,wed_lunch_end_time,thu_lunch_start_time,thu_lunch_end_time,fri_lunch_start_time,fri_lunch_end_time,sat_lunch_start_time,sat_lunch_end_time,
                                    last_batch_run ,TMP_ORD = 0
                               FROM 
                                    Organisation o
                                    INNER JOIN OrganisationType type on type.organisation_type_id = o.organisation_type_id
                                    INNER JOIN OrganisationTypeGroup typegroup ON type.organisation_type_group_id = typegroup.organisation_type_group_id 
                               WHERE
                                    type.organisation_type_group_id IN (1)  " + @"
                                  --ORDER BY 
                                  --  name
                                  ";

        string sqlNoGroups = @"SELECT 
                                    organisation_id,entity_id,parent_organisation_id,use_parent_offernig_prices,o.organisation_type_id,o.organisation_customer_type_id,name,acn,abn,organisation_date_added,organisation_date_modified,is_debtor,is_creditor,bpay_account,weeks_per_service_cycle,start_date,end_date,comment,free_services,
                                    excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat,
                                    sun_start_time,sun_end_time,mon_start_time,mon_end_time,tue_start_time,tue_end_time,wed_start_time,wed_end_time,thu_start_time,thu_end_time,fri_start_time,fri_end_time,sat_start_time,sat_end_time,
                                    sun_lunch_start_time,sun_lunch_end_time,mon_lunch_start_time,mon_lunch_end_time,tue_lunch_start_time,tue_lunch_end_time,wed_lunch_start_time,wed_lunch_end_time,thu_lunch_start_time,thu_lunch_end_time,fri_lunch_start_time,fri_lunch_end_time,sat_lunch_start_time,sat_lunch_end_time,
                                    last_batch_run ,TMP_ORD = 1
                               FROM 
                                    Organisation o
                                    INNER JOIN OrganisationType type on type.organisation_type_id = o.organisation_type_id
                                    INNER JOIN OrganisationTypeGroup typegroup ON type.organisation_type_group_id = typegroup.organisation_type_group_id 
                               WHERE
                                    type.organisation_type_group_id NOT IN (1,2,3) AND is_deleted = 0 " + ((notInList.Length > 0) ? " AND organisation_id NOT IN (" + notInList + @") " : "") + ((notInTypeGroupList.Length > 0) ? " AND type.organisation_type_group_id NOT IN (" + notInTypeGroupList + @") " : "") + @"
                                  --ORDER BY 
                                  --  name
                                  ";

        string sql = "SELECT * FROM (" + (exclGroupOrg ? "" :  sqlGroups + " UNION ") + sqlNoGroups + ") x ORDER BY TMP_ORD, name";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Organisation[] GetAllNotInc(Organisation[] excList, bool exclGroupOrg = true, bool exclIns = true)
    {
        DataTable tbl = GetDataTable_AllNotInc(excList, exclGroupOrg, false, false, exclIns);
        Organisation[] list = new Organisation[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = OrganisationDB.Load(tbl.Rows[i]);

        return list;
    }

    public static Organisation LoadAll(DataRow row)
    {
        Organisation org = Load(row);
        org.OrganisationType = OrganisationTypeDB.Load(row, "type_");
        org.OrganisationType.OrganisationTypeGroup = IDandDescrDB.Load(row, "typegroup_organisation_type_group_id", "typegroup_descr");
        org.OrganisationCustomerTypeID = Convert.ToInt32(row["ct_organisation_customer_type_id"]);

        return org;
    }

    public static Organisation Load(DataRow row, string prefix = "", string entityIdColumnName = "entity_id", string isDeletedColumnName = "")
    {
        return new Organisation(
            Convert.ToInt32(row[prefix + "organisation_id"]),
            Convert.ToInt32(row[prefix + entityIdColumnName]),
            (row[prefix + "parent_organisation_id"] == DBNull.Value ? 0 : Convert.ToInt32(row[prefix + "parent_organisation_id"])),
            Convert.ToBoolean(row[prefix + "use_parent_offernig_prices"]),
            Convert.ToInt32(row[prefix + "organisation_type_id"]),
            Convert.ToInt32(row[prefix + "organisation_customer_type_id"]),
            Convert.ToString(row[prefix + "name"]),
            Convert.ToString(row[prefix + "acn"]),
            Convert.ToString(row[prefix + "abn"]),
            Convert.ToDateTime(row[prefix + "organisation_date_added"]),
            (row[prefix + "organisation_date_modified"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "organisation_date_modified"])),
            Convert.ToBoolean(row[prefix + "is_debtor"]),
            Convert.ToBoolean(row[prefix + "is_creditor"]),
            Convert.ToString(row[prefix + "bpay_account"]),
            Convert.ToInt32(row[prefix + "weeks_per_service_cycle"]),
            (row[prefix + "start_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "start_date"])),
            (row[prefix + "end_date"]   == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "end_date"])),
            Convert.ToString(row[prefix + "comment"]),
            Convert.ToInt32(row[prefix + "free_services"]),
            Convert.ToBoolean(row[prefix + "excl_sun"]),
            Convert.ToBoolean(row[prefix + "excl_mon"]),
            Convert.ToBoolean(row[prefix + "excl_tue"]),
            Convert.ToBoolean(row[prefix + "excl_wed"]),
            Convert.ToBoolean(row[prefix + "excl_thu"]),
            Convert.ToBoolean(row[prefix + "excl_fri"]),
            Convert.ToBoolean(row[prefix + "excl_sat"]),
            (TimeSpan)row[prefix + "sun_start_time"],
            (TimeSpan)row[prefix + "sun_end_time"],
            (TimeSpan)row[prefix + "mon_start_time"],
            (TimeSpan)row[prefix + "mon_end_time"],
            (TimeSpan)row[prefix + "tue_start_time"],
            (TimeSpan)row[prefix + "tue_end_time"],
            (TimeSpan)row[prefix + "wed_start_time"],
            (TimeSpan)row[prefix + "wed_end_time"],
            (TimeSpan)row[prefix + "thu_start_time"],
            (TimeSpan)row[prefix + "thu_end_time"],
            (TimeSpan)row[prefix + "fri_start_time"],
            (TimeSpan)row[prefix + "fri_end_time"],
            (TimeSpan)row[prefix + "sat_start_time"],
            (TimeSpan)row[prefix + "sat_end_time"],
            (TimeSpan)row[prefix + "sun_lunch_start_time"],
            (TimeSpan)row[prefix + "sun_lunch_end_time"],
            (TimeSpan)row[prefix + "mon_lunch_start_time"],
            (TimeSpan)row[prefix + "mon_lunch_end_time"],
            (TimeSpan)row[prefix + "tue_lunch_start_time"],
            (TimeSpan)row[prefix + "tue_lunch_end_time"],
            (TimeSpan)row[prefix + "wed_lunch_start_time"],
            (TimeSpan)row[prefix + "wed_lunch_end_time"],
            (TimeSpan)row[prefix + "thu_lunch_start_time"],
            (TimeSpan)row[prefix + "thu_lunch_end_time"],
            (TimeSpan)row[prefix + "fri_lunch_start_time"],
            (TimeSpan)row[prefix + "fri_lunch_end_time"],
            (TimeSpan)row[prefix + "sat_lunch_start_time"],
            (TimeSpan)row[prefix + "sat_lunch_end_time"],
            (row[prefix + "last_batch_run"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "last_batch_run"])),
            isDeletedColumnName == "" ? Convert.ToBoolean(row[prefix + "is_deleted"]) : Convert.ToBoolean(row[isDeletedColumnName])
        );
    }

}