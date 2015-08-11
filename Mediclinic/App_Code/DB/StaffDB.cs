using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

public class StaffDB
{

    public static void Delete(int staff_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Staff WHERE staff_id = " + staff_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int person_id, string login, string pwd, int staff_position_id, int field_id, int costcentre_id, bool is_contractor, string tfn, string provider_number, bool is_fired, bool is_commission, decimal commission_percent, bool is_stakeholder, bool is_master_admin, bool is_admin, bool is_principal, bool is_provider, bool is_external, DateTime start_date, DateTime end_date, string comment, bool enable_daily_reminder_sms, bool enable_daily_reminder_email)
    {
        login = login.Replace("'", "''");
        pwd = pwd.Replace("'", "''");
        tfn = tfn.Replace("'", "''");
        provider_number = provider_number.Replace("'", "''");
        comment = comment.Replace("'", "''");

        if (LoginExists(login))
            throw new UniqueConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        string sql = "INSERT INTO Staff (person_id,login,pwd,staff_position_id,field_id,costcentre_id,is_contractor,tfn,provider_number,is_fired,is_commission,commission_percent,is_stakeholder,is_master_admin,is_admin,is_principal,is_provider,is_external,start_date,end_date,comment,enable_daily_reminder_sms,enable_daily_reminder_email) VALUES (" + "" + person_id + "," + "'" + login + "'," + "'" + pwd + "'," + "" + staff_position_id + "," + "" + field_id + "," + "" + costcentre_id + "," + (is_contractor ? "1," : "0,") + "'" + tfn + "'," + "UPPER('" + provider_number + "')," + (is_fired ? "1," : "0,") + (is_commission ? "1," : "0,") + "" + commission_percent + "," + (is_stakeholder ? "1," : "0,") + (is_master_admin ? "1," : "0,") + (is_admin ? "1," : "0,") + (is_principal ? "1," : "0,") + (is_provider ? "1," : "0,") + (is_external ? "1," : "0,") + (start_date == DateTime.MinValue ? "null" : "'" + start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (end_date == DateTime.MinValue ? "null" : "'" + end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + "'" + comment + "'," + (enable_daily_reminder_sms ? "1" : "0") + "," + (enable_daily_reminder_email ? "1" : "0") + ");SELECT SCOPE_IDENTITY();";
        int staff_id = Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
        StaffOfferingsDB.InsertBulkByStaffID(staff_id, false, 0, false, 0, DateTime.Today);
        return staff_id;
    }
    public static void Update(int staff_id, int person_id, string login, string pwd, int staff_position_id, int field_id, int costcentre_id, bool is_contractor, string tfn, string provider_number, bool is_fired, bool is_commission, decimal commission_percent, bool is_stakeholder, bool is_master_admin, bool is_admin, bool is_principal, bool is_provider, bool is_external, DateTime start_date, DateTime end_date, string comment, bool enable_daily_reminder_sms, bool enable_daily_reminder_email)
    {
        login = login.Replace("'", "''");
        pwd = pwd.Replace("'", "''");
        tfn = tfn.Replace("'", "''");
        provider_number = provider_number.Replace("'", "''");
        comment = comment.Replace("'", "''");
        string sql = "UPDATE Staff SET person_id = " + person_id + ",login = '" + login + "',pwd = '" + pwd + "',staff_position_id = " + staff_position_id + ",field_id = " + field_id + ",costcentre_id = " + costcentre_id + ",is_contractor = " + (is_contractor ? "1," : "0,") + "tfn = '" + tfn + "',provider_number = UPPER('" + provider_number + "'),is_fired = " + (is_fired ? "1," : "0,") + "is_commission = " + (is_commission ? "1," : "0,") + "commission_percent = " + commission_percent + ",is_stakeholder = " + (is_stakeholder ? "1" : "0") + ",is_master_admin = " + (is_master_admin ? "1" : "0") + ",is_admin = " + (is_admin ? "1" : "0") + ",is_principal = " + (is_principal ? "1" : "0") + ",is_provider = " + (is_provider ? "1" : "0") + ",is_external = " + (is_external ? "1" : "0") + ",start_date = " + (start_date == DateTime.MinValue ? "null" : "'" + start_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",end_date = " + (end_date == DateTime.MinValue ? "null" : "'" + end_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",comment = '" + comment + "'" + ",enable_daily_reminder_sms = " + (enable_daily_reminder_sms ? "1" : "0") + ",enable_daily_reminder_email = " + (enable_daily_reminder_email ? "1" : "0") + " WHERE staff_id = " + staff_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdatePwd(int staff_id, string pwd)
    {
        pwd = pwd.Replace("'", "''");
        string sql = "UPDATE Staff SET pwd = '" + pwd + "' WHERE staff_id = " + staff_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateNumDaysToDisplayOnBookingScreen(int staff_id, int num_days_to_display_on_booking_screen)
    {
        string sql = "UPDATE Staff SET num_days_to_display_on_booking_screen = " + num_days_to_display_on_booking_screen + " WHERE staff_id = " + staff_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateShowHeaderOnBookingScreen(int staff_id, bool show_header_on_booking_screen, string DB = null)
    {
        string sql = "UPDATE Staff SET show_header_on_booking_screen = " + (show_header_on_booking_screen ? "1" : "0") + " WHERE staff_id = " + staff_id.ToString();
        DBBase.ExecuteNonResult(sql, DB);
    }
    public static void UpdateBookingScreenField(int staff_id, int bk_screen_field_id)
    {
        // sometimes call centre was logged into one site and changes into another at another page, so 
        // when they change the services field on the booking, the id for that field may not exist in the system they are now in and this throws an exception
        // so just wrap it in a try/catch block to ignore it and not throw an error for the user
        try
        {
            string sql = bk_screen_field_id == -1 ?
                "UPDATE Staff SET bk_screen_field_id = " + (bk_screen_field_id == -1 ? "NULL" : bk_screen_field_id.ToString()) + " WHERE staff_id = " + staff_id.ToString() :
                "IF (SELECT COUNT(field_id) FROM Field WHERE field_id = " + bk_screen_field_id + ") > 0 BEGIN UPDATE Staff SET bk_screen_field_id = " + bk_screen_field_id + " WHERE staff_id =  " + staff_id + " END";
            DBBase.ExecuteNonResult(sql);
        }
        catch (SqlException) { }
        catch (Exception) { } 
    }
    public static void UpdateBookingScreenShowKey(int staff_id, bool bk_screen_show_key)
    {
        string sql = "UPDATE Staff SET bk_screen_show_key = " + (bk_screen_show_key ? "1" : "0") + " WHERE staff_id = " + staff_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static bool Exists(int staff_id)
    {
        string sql = "SELECT COUNT(*) FROM Staff WHERE staff_id = " + staff_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }
    public static bool Exists(string staff_ids)
    {
        string sql = @"SELECT staff_id
                       FROM   Staff
                       WHERE  staff_id IN (" + staff_ids + @")";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        System.Collections.Hashtable hash = new System.Collections.Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
            hash[Convert.ToInt32(tbl.Rows[i]["organisation_id"])] = 1;

        foreach (string str_id in staff_ids.Split(','))
            if (hash[Convert.ToInt32(str_id)] == null)
                return false;

        return true;
    }

    public static DataTable GetStats(DateTime start, DateTime end, bool incDeletedStaff, bool incSupportStaff)
    {
        string sql = @"


            SELECT s.staff_id, s.person_id, p.entity_id, p.firstname, p.surname,
                s.is_stakeholder, s.is_master_admin, s.is_admin, s.is_provider, s.is_external,


               (SELECT    COUNT(*)
                 FROM     Booking
                 WHERE    added_by = s.staff_id
                          " + (start == DateTime.MinValue ? "" : " AND date_created >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          " + (end   == DateTime.MinValue ? "" : " AND date_created <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @"
                          AND booking_type_id = 34 AND booking_status_id IN (0, 187, 188)) AS n_bookings,

               (SELECT    COUNT(DISTINCT Patient.patient_id)
                 FROM     Patient
			              INNER JOIN Person ON Patient.person_id = Person.person_id
                 WHERE    Person.added_by = p.person_id" +
                          (start == DateTime.MinValue ? "" : " AND Person.person_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND Person.person_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @") AS n_patients,

               (SELECT    COUNT(DISTINCT Booking.booking_id)
                 FROM     Invoice
						  INNER JOIN Booking ON Invoice.booking_id = Booking.booking_id
						  INNER JOIN Staff ON Invoice.staff_id = Staff.staff_id
                 WHERE    Staff.staff_id = s.staff_id" +
                          (start == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @") AS n_completions,

               (SELECT    IsNull(SUM(Receipt.total), 0)
                 FROM     Receipt
						  INNER JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
						  INNER JOIN Staff ON Invoice.staff_id = Staff.staff_id
                 WHERE    Staff.staff_id = s.staff_id" +
                          (start == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @") AS sum_receipts,

               (SELECT    IsNull(SUM(CreditNote.total), 0)
                 FROM     CreditNote
						  INNER JOIN Invoice ON CreditNote.invoice_id = Invoice.invoice_id
						  INNER JOIN Staff ON Invoice.staff_id = Staff.staff_id
                 WHERE    Staff.staff_id = s.staff_id" +
                          (start == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @") AS sum_cr_notes,

               (SELECT    IsNull(SUM(Invoice.total), 0)
                 FROM     Invoice
						  INNER JOIN Staff ON Invoice.staff_id = Staff.staff_id
                 WHERE    Staff.staff_id = s.staff_id" +
                          (start == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end == DateTime.MinValue   ? "" : " AND Invoice.invoice_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @") AS sum_inv_total


            FROM Staff s
            LEFT JOIN Person p ON s.person_id = p.person_id
            WHERE 1=1 " + (incDeletedStaff ? "" : " AND s.is_fired = 0") + (incSupportStaff ? "" : " AND (s.staff_id NOT IN (-2,-3,-4))") + @"
            ORDER BY s.is_stakeholder DESC, s.is_master_admin DESC, s.is_admin DESC, s.is_provider DESC, s.is_external DESC, n_patients DESC, n_bookings DESC, surname, firstname";

        DataTable tbl = DBBase.ExecuteQuery(sql, null, 900).Tables[0];

        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int staff_id = Convert.ToInt32(tbl.Rows[i]["staff_id"]);
            if (staff_id == -5 || staff_id == -7 || staff_id == -8)
            {
                tbl.Rows[i]["is_stakeholder"]  = 0;
                tbl.Rows[i]["is_master_admin"] = 0;
                tbl.Rows[i]["is_admin"]        = 0;
                tbl.Rows[i]["is_external"]     = 0;

                DataRow newRow = tbl.NewRow();
                newRow.ItemArray = tbl.Rows[i].ItemArray;   // We "clone" the row
                tbl.Rows.RemoveAt(i);                       // We remove the old
                tbl.Rows.InsertAt(newRow, tbl.Rows.Count);  // and insert the new
            }
        }

        return tbl;
    }
    public static DataTable GetStats2(DateTime start, DateTime end, bool incDeletedStaff, bool incSupportStaff)
    {
        string sql = @"

            SELECT s.staff_id, s.person_id, p.entity_id, p.firstname, p.surname,
                   s.is_stakeholder, s.is_master_admin, s.is_admin, s.is_provider, s.is_external

            FROM Staff s
            LEFT JOIN Person p ON s.person_id = p.person_id
            WHERE 1=1 " + (incDeletedStaff ? "" : " AND s.is_fired = 0") + (incSupportStaff ? "" : " AND (s.staff_id NOT IN (-2,-3,-4))") + @"
            --ORDER BY s.is_stakeholder DESC, s.is_master_admin DESC, s.is_admin DESC, s.is_provider DESC, s.is_external DESC, surname, firstname";

        DataTable tbl = DBBase.ExecuteQuery(sql, null, 900).Tables[0];

        tbl.Columns.Add("total_bookings", typeof(int));
        tbl.Columns.Add("avg_minutes", typeof(decimal));
        tbl.Columns.Add("n_bookings", typeof(int));
        tbl.Columns.Add("n_patients", typeof(int));
        tbl.Columns.Add("n_completions", typeof(int));
        tbl.Columns.Add("sum_receipts" , typeof(decimal));
        tbl.Columns.Add("sum_cr_notes" , typeof(decimal));
        tbl.Columns.Add("sum_inv_total", typeof(decimal));
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int staff_id = Convert.ToInt32(tbl.Rows[i]["staff_id"]);
            int person_id = Convert.ToInt32(tbl.Rows[i]["person_id"]);

            tbl.Rows[i]["total_bookings"] = GetCountTotalBookings(staff_id, start, end);
            int avg_minutes = GetAvgMinutes(staff_id, start, end);
            tbl.Rows[i]["avg_minutes"]    = avg_minutes == -1 ? (object)DBNull.Value : avg_minutes;
            tbl.Rows[i]["n_bookings"]     = GetCountBookings(staff_id, start, end);
            tbl.Rows[i]["n_patients"]     = GetCountPatients(person_id, start, end);
            tbl.Rows[i]["n_completions"]  = GetCountCompletions(staff_id, start, end);
            tbl.Rows[i]["sum_receipts"]   = GetSumReceipts(staff_id, start, end);
            tbl.Rows[i]["sum_cr_notes"]   = GetSumCrNotes(staff_id, start, end);
            tbl.Rows[i]["sum_inv_total"]  = GetSumInvTotal(staff_id, start, end);
        }

        DataView dv = tbl.DefaultView;
        dv.Sort = "is_stakeholder DESC, is_master_admin DESC, is_admin DESC, is_provider DESC, is_external DESC, n_patients DESC, n_bookings DESC, surname, firstname";
        tbl = dv.ToTable();

        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            int staff_id = Convert.ToInt32(tbl.Rows[i]["staff_id"]);
            if (staff_id == -5 || staff_id == -7 || staff_id == -8)
            {
                tbl.Rows[i]["is_stakeholder"]  = 0;
                tbl.Rows[i]["is_master_admin"] = 0;
                tbl.Rows[i]["is_admin"]        = 0;
                tbl.Rows[i]["is_provider"]     = 0;
                tbl.Rows[i]["is_external"]     = 0;

                DataRow newRow = tbl.NewRow();
                newRow.ItemArray = tbl.Rows[i].ItemArray;   // We "clone" the row
                tbl.Rows.RemoveAt(i);                       // We remove the old
                tbl.Rows.InsertAt(newRow, tbl.Rows.Count);  // and insert the new
            }
        }

        return tbl;
    }

    protected static int GetCountTotalBookings(int staff_id, DateTime start, DateTime end)
    {
        string sql1 = @"  SELECT COUNT(*)
                 FROM     Booking
                 WHERE    provider = " + staff_id + @"
                          " + (start == DateTime.MinValue ? "" : " AND date_start >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          " + (end == DateTime.MinValue ? "" : " AND date_start <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          AND booking_type_id = 34 AND booking_status_id IN (0, 187, 188)";

        return (int)DBBase.ExecuteSingleResult(sql1);
    }
    protected static int GetAvgMinutes(int staff_id, DateTime start, DateTime end)
    {
        string sql1 = @"  SELECT SUM(datediff(minute,date_start, date_end))/COUNT(*)
                 FROM     Booking
                 WHERE    provider = " + staff_id + @"
                          " + (start == DateTime.MinValue ? "" : " AND date_start >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          " + (end == DateTime.MinValue ? "" : " AND date_start <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          AND booking_type_id = 34 AND booking_status_id IN (0, 187, 188)";

        object o = DBBase.ExecuteSingleResult(sql1);

        Type t = o.GetType();


        if (o == DBNull.Value)
            return -1;
        else
            return Convert.ToInt32(DBBase.ExecuteSingleResult(sql1));
    }
    protected static int GetCountBookings(int staff_id, DateTime start, DateTime end)
    {
        string sql1 = @"  SELECT COUNT(*)
                 FROM     Booking
                 WHERE    added_by = " + staff_id + @"
                          " + (start == DateTime.MinValue ? "" : " AND date_created >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + @"
                          " + (end   == DateTime.MinValue ? "" : " AND date_created <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'") + @"
                          AND booking_type_id = 34 AND booking_status_id IN (0, 187, 188)";

        return (int)DBBase.ExecuteSingleResult(sql1);
    }
    protected static int GetCountPatients(int person_id, DateTime start, DateTime end)
    {
        string sql1 = @"  SELECT    COUNT(DISTINCT Patient.patient_id)
                 FROM     Patient
			              INNER JOIN Person ON Patient.person_id = Person.person_id
                 WHERE    Person.added_by = " + person_id +
                          (start == DateTime.MinValue ? "" : " AND Person.person_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND Person.person_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'");

        return (int)DBBase.ExecuteSingleResult(sql1);
    }
    protected static int GetCountCompletions(int staff_id, DateTime start, DateTime end)
    {
        string sql1 = @"  SELECT COUNT(DISTINCT Booking.booking_id)
                 FROM     Invoice
						  INNER JOIN Booking ON Invoice.booking_id = Booking.booking_id
						  INNER JOIN Staff ON Invoice.staff_id = Staff.staff_id
                 WHERE    Staff.staff_id = " + staff_id +
                          (start == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'");

        return (int)DBBase.ExecuteSingleResult(sql1);
    }
    protected static decimal GetSumReceipts(int staff_id, DateTime start, DateTime end)
    {
        string sql1 = @" SELECT IsNull(SUM(Receipt.total), 0)
                 FROM     Receipt
						  INNER JOIN Invoice ON Receipt.invoice_id = Invoice.invoice_id
						  INNER JOIN Staff ON Invoice.staff_id = Staff.staff_id
                 WHERE    Staff.staff_id = " + staff_id +
                          (start == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'");

        return (decimal)DBBase.ExecuteSingleResult(sql1);
    }
    protected static decimal GetSumCrNotes(int staff_id, DateTime start, DateTime end)
    {
        string sql1 = @" SELECT    IsNull(SUM(CreditNote.total), 0)
                 FROM     CreditNote
						  INNER JOIN Invoice ON CreditNote.invoice_id = Invoice.invoice_id
						  INNER JOIN Staff ON Invoice.staff_id = Staff.staff_id
                 WHERE    Staff.staff_id = " + staff_id +
                          (start == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'");

        return (decimal)DBBase.ExecuteSingleResult(sql1);
    }
    protected static decimal GetSumInvTotal(int staff_id, DateTime start, DateTime end)
    {
        string sql1 = @" SELECT    IsNull(SUM(Invoice.total), 0)
                 FROM     Invoice
						  INNER JOIN Staff ON Invoice.staff_id = Staff.staff_id
                 WHERE    Staff.staff_id = " + staff_id +
                          (start == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added >= '" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "'") +
                          (end   == DateTime.MinValue ? "" : " AND Invoice.invoice_date_added <= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss")   + "'");

        return (decimal)DBBase.ExecuteSingleResult(sql1);
    }



    public static int GetCountOfProviders(bool inc_fired = false)
    {
        string sql = "SELECT COUNT(*) FROM Staff WHERE is_provider = 1" + (inc_fired ? "" : " AND is_fired = 0");
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static int GetCountByProvNbr(string provider_number)
    {
        provider_number = provider_number.Replace("'", "''");
        string sql = "SELECT COUNT(*) FROM Staff WHERE provider_number = '" + provider_number.ToString() + "'";
        return Convert.ToInt32( DBBase.ExecuteSingleResult(sql) );
    }

    public static string GetAllProviderNbrs()
    {
        string sql = "SELECT DISTINCT provider_number FROM Staff WHERE LEN(provider_number) > 0";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        string s = string.Empty;
        foreach (DataRow row in tbl.Rows)
            s += row["provider_number"].ToString() + ",";
        return s;
    }

    public static DataTable GetDataTable(bool showIsExternal = false, bool showOnlyExternal = false)
    {
        string sql = JoinedSql + " AND s.is_fired = 0 AND s.staff_id > 0 " + (showIsExternal ? "" : " AND s.is_external = 0 ") + (showOnlyExternal ? " AND s.is_external = 1 " : "") + " ORDER BY p.surname, p.firstname, p.middlename ";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Staff[] GetAll(bool showIsExternal = false, bool showOnlyExternal = false)
    {
        DataTable tbl = GetDataTable(showIsExternal, showOnlyExternal);

        Staff[] list = new Staff[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);

        return list;
    }
    public static System.Collections.Hashtable GetAllInHashtable(bool incSupportStaff = false, bool showIsFired = false, bool showIsExternal = false, bool showOnlyExternal = false)
    {
        System.Collections.Hashtable hash = new System.Collections.Hashtable();

        DataTable tbl = StaffDB.GetDataTable_StaffInfo(incSupportStaff, showIsFired, showIsExternal, showOnlyExternal);
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Staff staff         = Load(tbl.Rows[i]);
            staff.Person        = PersonDB.Load(tbl.Rows[i]);
            staff.Person.Title  = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
            staff.Field         = IDandDescrDB.Load(tbl.Rows[i], "field_field_id" ,"field_descr" );
            staff.StaffPosition = StaffPositionDB.Load(tbl.Rows[i], "staff_position_");
            staff.CostCentre    = CostCentreDB.Load(tbl.Rows[i], "cost_centre_");
            hash[staff.StaffID] = staff;
        }

        return hash;
    }

    public static DataTable GetDataTable_Providers()
    {
        string sql = JoinedSql + " AND s.is_fired = 0 AND s.is_provider = 1 ORDER BY p.surname, p.firstname, p.middlename ";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Staff[] GetProviders()
    {
        DataTable tbl = GetDataTable_Providers();

        Staff[] list = new Staff[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);

        return list;
    }


    public static string JoinedSql = @"
            SELECT 
                s.staff_id as staff_staff_id,s.person_id as staff_person_id,s.login as staff_login,s.pwd as staff_pwd,s.staff_position_id as staff_staff_position_id,
                s.field_id as staff_field_id,s.is_contractor as staff_is_contractor,s.tfn as staff_tfn,
                s.is_fired as staff_is_fired,s.costcentre_id as staff_costcentre_id,
                s.is_admin as staff_is_admin,s.provider_number as staff_provider_number,s.is_commission as staff_is_commission,s.commission_percent as staff_commission_percent,
                s.is_stakeholder as staff_is_stakeholder,s.is_master_admin as staff_is_master_admin,s.is_admin as staff_is_admin,s.is_principal as staff_is_principal,s.is_provider as staff_is_provider,s.is_external as staff_is_external,
                s.staff_date_added as staff_staff_date_added,s.start_date as staff_start_date,s.end_date as staff_end_date,s.comment as staff_comment,
                s.num_days_to_display_on_booking_screen as staff_num_days_to_display_on_booking_screen, 
                s.show_header_on_booking_screen as staff_show_header_on_booking_screen,
                s.bk_screen_field_id as staff_bk_screen_field_id, 
                s.bk_screen_show_key as staff_bk_screen_show_key, 
                s.enable_daily_reminder_sms as staff_enable_daily_reminder_sms, 
                s.enable_daily_reminder_email as staff_enable_daily_reminder_email,

                " + PersonDB.GetFields("person_", "p") + @", 
                t.title_id as title_title_id, t.descr as title_descr,
                c.costcentre_id as cost_centre_costcentre_id,c.descr as cost_centre_descr,c.parent_id as cost_centre_parent_id,
                sr.field_id as field_field_id,sr.descr as field_descr,
                sp.staff_position_id as staff_position_staff_position_id,sp.descr as staff_position_descr
            FROM Staff s 
                LEFT OUTER JOIN Person p    ON s.person_id     = p.person_id
                LEFT OUTER JOIN Title  t    ON t.title_id      = p.title_id
                INNER      JOIN Field  sr   ON s.field_id      = sr.field_id
                LEFT OUTER JOIN StaffPosition sp  ON sp.staff_position_id = s.staff_position_id
                LEFT OUTER JOIN CostCentre AS c   ON c.costcentre_id       = s.costcentre_id
            WHERE 1=1";


    public static DataTable GetDataTable_StaffInfo(bool incSupportStaff, bool showIsFired = false, bool showIsExternal = false, bool showOnlyExternal = false, string matchSurname = "", bool searchSurnameOnlyStartsWith = false)
    {
        matchSurname = matchSurname.Replace("'", "''");

        string sql = @"SELECT
                              s.staff_id as staff_id,login,pwd,pos.staff_position_id as staff_position_id,pos.descr as staff_position_descr,s.field_id as field_id,r.descr AS field_descr,c.costcentre_id as costcentre_id,c.descr AS costcentre_descr,is_contractor,tfn,provider_number,is_fired,is_commission,commission_percent,
                              is_stakeholder,is_master_admin,is_admin,is_principal,is_provider,is_external,
                              s.staff_date_added,start_date,end_date,comment,num_days_to_display_on_booking_screen, show_header_on_booking_screen, bk_screen_field_id, bk_screen_show_key, enable_daily_reminder_sms, enable_daily_reminder_email,

                              " + PersonDB.GetFields("", "p") + @", p2.firstname AS added_by_firstname, 
                              t.title_id, t.descr,
                              c.costcentre_id as cost_centre_costcentre_id,c.descr as cost_centre_descr,c.parent_id as cost_centre_parent_id,
                              r.field_id as field_field_id,r.descr as field_descr,
                              pos.staff_position_id as staff_position_staff_position_id,pos.descr as staff_position_descr
                       FROM Staff s 
                            LEFT OUTER JOIN Person p    ON s.person_id     = p.person_id
                            LEFT OUTER JOIN Person p2   ON p2.person_id    = p.added_by
                            LEFT OUTER JOIN Title  t    ON t.title_id      = p.title_id
                            INNER      JOIN Field  r    ON s.field_id = r.field_id
                            INNER      JOIN StaffPosition pos ON pos.staff_position_id = s.staff_position_id
                            INNER      JOIN CostCentre AS c   ON c.costcentre_id       = s.costcentre_id ";

        string whereClause = string.Empty;

        if (!incSupportStaff)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " s.staff_id > 0 ";
        if (!showIsFired)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " s.is_fired = 0 ";
        if (matchSurname.Length > 0 && !searchSurnameOnlyStartsWith)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " p.surname LIKE '%" + matchSurname + "%'";
        if (matchSurname.Length > 0 && searchSurnameOnlyStartsWith)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " p.surname LIKE '" + matchSurname + "%'";
        if (!showIsExternal)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " s.is_external = 0";
        if (showOnlyExternal)
            whereClause += (whereClause.Length == 0 ? " WHERE " : " AND ") + " s.is_external = 1";

        sql += whereClause + @" ORDER BY p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Staff GetByID(int staff_id)
    {
        string sql = JoinedSql + " AND staff_id = " + staff_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }
    public static Staff GetByEntityID(int entity_id)
    {
        string sql = JoinedSql + " AND p.entity_id = " + entity_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static Staff[] DuplicateSearch(string firstname, string middlename, string surname)
    {
        firstname  = firstname.Trim().Replace("'", "''");;
        middlename = middlename.Trim().Replace("'", "''"); ;
        surname    = surname.Trim().Replace("'", "''");;

        string sql = JoinedSql + " AND s.is_fired = 0 AND s.is_external = 0 AND s.staff_id > 0 ";
        if (firstname.Length > 0)
            sql += " AND (p.firstname = '" + firstname + "' OR SOUNDEX(p.firstname) = SOUNDEX('" + firstname + "'))";
        if (middlename.Length > 0)
            sql += " AND (p.middlename = '" + middlename + "' OR SOUNDEX(p.middlename) = SOUNDEX('" + middlename + "'))";
        if (surname.Length > 0)
            sql += " AND (p.surname = '" + surname + "' OR SOUNDEX(p.surname) = SOUNDEX('" + surname + "'))";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Staff[] list = new Staff[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }

    public static Staff GetByLoginPwd(string login, string pwd)
    {
        login = login.Replace("'", "''");
        pwd   = pwd.Replace("'", "''");

        string sql = "SELECT staff_id,person_id,login,pwd,staff_position_id,field_id,costcentre_id,is_contractor,tfn,provider_number,is_fired,is_commission,commission_percent,is_stakeholder,is_master_admin,is_admin,is_principal,is_provider,is_external,staff_date_added,start_date,end_date,comment FROM Staff WHERE is_fired = 0 AND login COLLATE Latin1_General_CS_AS = '" + login + "' AND pwd = '" + pwd + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }
    public static Staff GetByLogin(string login)
    {
        login = login.Replace("'", "''");

        string sql = JoinedSql + " AND s.is_fired = 0 AND login COLLATE Latin1_General_CS_AS = '" + login + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }
    public static bool LoginExists(string login, int staff_id_exclude = -1)
    {
        login = login.Replace("'", "''");
        string sql = "SELECT COUNT(*) FROM Staff WHERE login COLLATE Latin1_General_CS_AS = '" + login + "'" + (staff_id_exclude == -1 ? "" : " AND staff_id <> " + staff_id_exclude);
        return (Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0);
    }


    public static DataTable GetDataTable_AllNotInc(Staff[] excList)
    {
        string notInList = string.Empty;
        foreach (Staff s in excList)
            notInList += s.StaffID.ToString() + ",";
        if (notInList.Length > 0)
            notInList = notInList.Substring(0, notInList.Length - 1);

        string sql = @"SELECT s.staff_id, s.person_id, s.login, s.pwd, s.staff_position_id, s.field_id, s.costcentre_id, s.is_contractor, s.tfn, s.provider_number, 
                         s.is_fired, s.is_commission, s.commission_percent, s.is_stakeholder, s.is_master_admin, s.is_admin, s.is_principal, s.is_provider, s.is_external, s.staff_date_added, 
                         s.start_date, s.end_date, s.comment, 
                         " + PersonDB.GetFields("", "p") + @", 
                         t.title_id, t.descr
                       FROM
                         Staff AS s
                         LEFT OUTER JOIN Person p  ON s.person_id = p.person_id
                         LEFT OUTER JOIN Title  t  ON t.title_id  = p.title_id
                       WHERE s.staff_id > 0 AND s.is_fired = 0 AND s.is_external = 0 " + ((notInList.Length > 0) ? " AND staff_id NOT IN (" + notInList + @") " : "") + @"
                       ORDER BY 
                         p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Staff[] GetAllNotInc(Staff[] excList)
    {
        DataTable tbl = GetDataTable_AllNotInc(excList);
        return LoadFull(tbl);
    }



    public static Staff[] LoadFull(DataTable tbl, string staff_prefix = "", string person_prefix = "")
    {
        Staff[] list = new Staff[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);

        return list;
    }

    public static Staff LoadAll(DataRow row)
    {
        Staff staff = Load(row, "staff_");
        staff.Person = PersonDB.Load(row, "person_");
        staff.Person.Title = IDandDescrDB.Load(row, "title_title_id", "title_descr");
        staff.Field = IDandDescrDB.Load(row, "field_field_id" ,"field_descr" );
        staff.StaffPosition = StaffPositionDB.Load(row, "staff_position_");
        staff.CostCentre = CostCentreDB.Load(row, "cost_centre_");
        return staff;
    }

    public static Staff Load(DataRow row, string prefix = "")
    {
        return new Staff(
            Convert.ToInt32(row[prefix + "staff_id"]),
            Convert.ToInt32(row[prefix + "person_id"]),
            Convert.ToString(row[prefix + "login"]),
            Convert.ToString(row[prefix + "pwd"]),
            Convert.ToInt32(row[prefix + "staff_position_id"]),
            Convert.ToInt32(row[prefix + "field_id"]),
            Convert.ToInt32(row[prefix + "costcentre_id"]),
            Convert.ToBoolean(row[prefix + "is_contractor"]),
            Convert.ToString(row[prefix + "tfn"]),
            Convert.ToString(row[prefix + "provider_number"]),
            Convert.ToBoolean(row[prefix + "is_fired"]),
            Convert.ToBoolean(row[prefix + "is_commission"]),
            Convert.ToDecimal(row[prefix + "commission_percent"]),
            Convert.ToBoolean(row[prefix + "is_stakeholder"]),
            Convert.ToBoolean(row[prefix + "is_master_admin"]),
            Convert.ToBoolean(row[prefix + "is_admin"]),
            Convert.ToBoolean(row[prefix + "is_principal"]),
            Convert.ToBoolean(row[prefix + "is_provider"]),
            Convert.ToBoolean(row[prefix + "is_external"]),
            Convert.ToDateTime(row[prefix + "staff_date_added"]),
            row[prefix + "start_date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "start_date"]),
            row[prefix + "end_date"]   == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "end_date"]),
            Convert.ToString(row[prefix + "comment"]),
            Convert.ToInt32(row[prefix + "num_days_to_display_on_booking_screen"]),
            Convert.ToBoolean(row[prefix + "show_header_on_booking_screen"]),
            row[prefix + "bk_screen_field_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "bk_screen_field_id"]),
            Convert.ToBoolean(row[prefix + "bk_screen_show_key"]),
            Convert.ToBoolean(row[prefix + "enable_daily_reminder_sms"]),
            Convert.ToBoolean(row[prefix + "enable_daily_reminder_email"])
        );
    }

}