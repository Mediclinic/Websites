using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Organisation
{

    public Organisation(int organisation_id, int entity_id, int parent_organisation_id, bool use_parent_offernig_prices, int organisation_type_id, int organisation_customer_type_id, string name, string acn, string abn,
                DateTime organisation_date_added, DateTime organisation_date_modified, bool is_debtor, bool is_creditor,string bpay_account, 
                int weeks_per_service_cycle, DateTime start_date, DateTime end_date, string comment, int free_services,
                bool excl_sun, bool excl_mon, bool excl_tue, bool excl_wed, bool excl_thu, bool excl_fri,
                bool excl_sat, 
                TimeSpan sun_start_time, TimeSpan sun_end_time, TimeSpan mon_start_time, TimeSpan mon_end_time, 
                TimeSpan tue_start_time, TimeSpan tue_end_time, TimeSpan wed_start_time, TimeSpan wed_end_time, 
                TimeSpan thu_start_time, TimeSpan thu_end_time, TimeSpan fri_start_time, TimeSpan fri_end_time, 
                TimeSpan sat_start_time, TimeSpan sat_end_time,
                TimeSpan sun_lunch_start_time, TimeSpan sun_lunch_end_time, TimeSpan mon_lunch_start_time, TimeSpan mon_lunch_end_time,
                TimeSpan tue_lunch_start_time, TimeSpan tue_lunch_end_time, TimeSpan wed_lunch_start_time, TimeSpan wed_lunch_end_time,
                TimeSpan thu_lunch_start_time, TimeSpan thu_lunch_end_time, TimeSpan fri_lunch_start_time, TimeSpan fri_lunch_end_time,
                TimeSpan sat_lunch_start_time, TimeSpan sat_lunch_end_time,
                DateTime last_batch_run, bool is_deleted)
    {
        this.organisation_id = organisation_id;
        this.entity_id = entity_id;
        this.parent_organisation = parent_organisation_id == 0 ? null : new Organisation(parent_organisation_id);
        this.use_parent_offernig_prices = use_parent_offernig_prices;
        this.organisationType = new OrganisationType(organisation_type_id);
        this.organisation_customer_type_id =  organisation_customer_type_id;
        this.name = name;
        this.acn = acn;
        this.abn = abn;
        this.organisation_date_added = organisation_date_added;
        this.organisation_date_modified = organisation_date_modified;
        this.is_debtor = is_debtor;
        this.is_creditor = is_creditor;
        this.bpay_account = bpay_account;
        this.weeks_per_service_cycle = weeks_per_service_cycle;
        this.start_date = start_date;
        this.end_date = end_date;
        this.comment = comment;
        this.free_services = free_services;
        this.excl_sun = excl_sun;
        this.excl_mon = excl_mon;
        this.excl_tue = excl_tue;
        this.excl_wed = excl_wed;
        this.excl_thu = excl_thu;
        this.excl_fri = excl_fri;
        this.excl_sat = excl_sat;
        this.sun_start_time = sun_start_time;
        this.sun_end_time   = sun_end_time;
        this.mon_start_time = mon_start_time;
        this.mon_end_time   = mon_end_time;
        this.tue_start_time = tue_start_time;
        this.tue_end_time   = tue_end_time;
        this.wed_start_time = wed_start_time;
        this.wed_end_time   = wed_end_time;
        this.thu_start_time = thu_start_time;
        this.thu_end_time   = thu_end_time;
        this.fri_start_time = fri_start_time;
        this.fri_end_time   = fri_end_time;
        this.sat_start_time = sat_start_time;
        this.sat_end_time   = sat_end_time;
        this.sun_lunch_start_time = sun_lunch_start_time;
        this.sun_lunch_end_time   = sun_lunch_end_time;
        this.mon_lunch_start_time = mon_lunch_start_time;
        this.mon_lunch_end_time   = mon_lunch_end_time;
        this.tue_lunch_start_time = tue_lunch_start_time;
        this.tue_lunch_end_time   = tue_lunch_end_time;
        this.wed_lunch_start_time = wed_lunch_start_time;
        this.wed_lunch_end_time   = wed_lunch_end_time;
        this.thu_lunch_start_time = thu_lunch_start_time;
        this.thu_lunch_end_time   = thu_lunch_end_time;
        this.fri_lunch_start_time = fri_lunch_start_time;
        this.fri_lunch_end_time   = fri_lunch_end_time;
        this.sat_lunch_start_time = sat_lunch_start_time;
        this.sat_lunch_end_time   = sat_lunch_end_time;
        this.last_batch_run = last_batch_run;
        this.is_deleted = is_deleted;
    }
    public Organisation(int organisation_id)
    {
        this.organisation_id = organisation_id;
    }

    private int organisation_id;
    public int OrganisationID
    {
        get { return this.organisation_id; }
        set { this.organisation_id = value; }
    }
    private int entity_id;
    public int EntityID
    {
        get { return this.entity_id; }
        set { this.entity_id = value; }
    }
    private Organisation parent_organisation;
    public Organisation ParentOrganisation
    {
        get { return this.parent_organisation; }
        set { this.parent_organisation = value; }
    }
    private bool use_parent_offernig_prices;
    public bool UseParentOffernigPrices
    {
        get { return this.use_parent_offernig_prices; }
        set { this.use_parent_offernig_prices = value; }
    }


    // normally will be null from Load method - unless explicitly put in
    private Organisation[] treeChildren;
    public Organisation[] TreeChildren
    {
        get { return this.treeChildren; }
        set { this.treeChildren = value; }
    }
    private int treeLevel;
    public int TreeLevel
    {
        get { return this.treeLevel; }
        set { this.treeLevel = value; }
    }
    private System.Data.DataRow treeDataRow;
    public System.Data.DataRow TreeDataRow
    {
        get { return this.treeDataRow; }
        set { this.treeDataRow = value; }
    }


    private OrganisationType organisationType;
    public OrganisationType OrganisationType
    {
        get { return this.organisationType; }
        set { this.organisationType = value; }
    }
    private int organisation_customer_type_id;
    public int OrganisationCustomerTypeID
    {
        get { return this.organisation_customer_type_id; }
        set { this.organisation_customer_type_id = value; }
    }
    private string name;
    public string Name
    {
        get { return this.name; }
        set { this.name = value; }
    }
    private string acn;
    public string Acn
    {
        get { return this.acn; }
        set { this.acn = value; }
    }
    private string abn;
    public string Abn
    {
        get { return this.abn; }
        set { this.abn = value; }
    }
    private DateTime organisation_date_added;
    public DateTime OrganisationDateAdded
    {
        get { return this.organisation_date_added; }
        set { this.organisation_date_added = value; }
    }
    private DateTime organisation_date_modified;
    public DateTime OrganisationDateModified
    {
        get { return this.organisation_date_modified; }
        set { this.organisation_date_modified = value; }
    }
    private bool is_debtor;
    public bool IsDebtor
    {
        get { return this.is_debtor; }
        set { this.is_debtor = value; }
    }
    private bool is_creditor;
    public bool IsCreditor
    {
        get { return this.is_creditor; }
        set { this.is_creditor = value; }
    }
    private string bpay_account;
    public string BpayAccount
    {
        get { return this.bpay_account; }
        set { this.bpay_account = value; }
    }
    private int weeks_per_service_cycle;
    public int WeeksPerServiceCycle
    {
        get { return this.weeks_per_service_cycle; }
        set { this.weeks_per_service_cycle = value; }
    }
    private DateTime start_date;
    public DateTime StartDate
    {
        get { return this.start_date; }
        set { this.start_date = value; }
    }
    private DateTime end_date;
    public DateTime EndDate
    {
        get { return this.end_date; }
        set { this.end_date = value; }
    }
    private string comment;
    public string Comment
    {
        get { return this.comment; }
        set { this.comment = value; }
    }
    private int free_services;
    public int FreeServices
    {
        get { return this.free_services; }
        set { this.free_services = value; }
    }
    private bool excl_sun;
    public bool ExclSun
    {
        get { return this.excl_sun; }
        set { this.excl_sun = value; }
    }
    private bool excl_mon;
    public bool ExclMon
    {
        get { return this.excl_mon; }
        set { this.excl_mon = value; }
    }
    private bool excl_tue;
    public bool ExclTue
    {
        get { return this.excl_tue; }
        set { this.excl_tue = value; }
    }
    private bool excl_wed;
    public bool ExclWed
    {
        get { return this.excl_wed; }
        set { this.excl_wed = value; }
    }
    private bool excl_thu;
    public bool ExclThu
    {
        get { return this.excl_thu; }
        set { this.excl_thu = value; }
    }
    private bool excl_fri;
    public bool ExclFri
    {
        get { return this.excl_fri; }
        set { this.excl_fri = value; }
    }
    private bool excl_sat;
    public bool ExclSat
    {
        get { return this.excl_sat; }
        set { this.excl_sat = value; }
    }

    private TimeSpan sun_start_time;
    public TimeSpan SunStartTime
    {
        get { return this.sun_start_time; }
        set { this.sun_start_time = value; }
    }
    private TimeSpan sun_end_time;
    public TimeSpan SunEndTime
    {
        get { return this.sun_end_time; }
        set { this.sun_end_time = value; }
    }
    private TimeSpan mon_start_time;
    public TimeSpan MonStartTime
    {
        get { return this.mon_start_time; }
        set { this.mon_start_time = value; }
    }
    private TimeSpan mon_end_time;
    public TimeSpan MonEndTime
    {
        get { return this.mon_end_time; }
        set { this.mon_end_time = value; }
    }
    private TimeSpan tue_start_time;
    public TimeSpan TueStartTime
    {
        get { return this.tue_start_time; }
        set { this.tue_start_time = value; }
    }
    private TimeSpan tue_end_time;
    public TimeSpan TueEndTime
    {
        get { return this.tue_end_time; }
        set { this.tue_end_time = value; }
    }
    private TimeSpan wed_start_time;
    public TimeSpan WedStartTime
    {
        get { return this.wed_start_time; }
        set { this.wed_start_time = value; }
    }
    private TimeSpan wed_end_time;
    public TimeSpan WedEndTime
    {
        get { return this.wed_end_time; }
        set { this.wed_end_time = value; }
    }
    private TimeSpan thu_start_time;
    public TimeSpan ThuStartTime
    {
        get { return this.thu_start_time; }
        set { this.thu_start_time = value; }
    }
    private TimeSpan thu_end_time;
    public TimeSpan ThuEndTime
    {
        get { return this.thu_end_time; }
        set { this.thu_end_time = value; }
    }
    private TimeSpan fri_start_time;
    public TimeSpan FriStartTime
    {
        get { return this.fri_start_time; }
        set { this.fri_start_time = value; }
    }
    private TimeSpan fri_end_time;
    public TimeSpan FriEndTime
    {
        get { return this.fri_end_time; }
        set { this.fri_end_time = value; }
    }
    private TimeSpan sat_start_time;
    public TimeSpan SatStartTime
    {
        get { return this.sat_start_time; }
        set { this.sat_start_time = value; }
    }
    private TimeSpan sat_end_time;
    public TimeSpan SatEndTime
    {
        get { return this.sat_end_time; }
        set { this.sat_end_time = value; }
    }
    private TimeSpan sun_lunch_start_time;
    public TimeSpan SunLunchStartTime
    {
        get { return this.sun_lunch_start_time; }
        set { this.sun_lunch_start_time = value; }
    }
    private TimeSpan sun_lunch_end_time;
    public TimeSpan SunLunchEndTime
    {
        get { return this.sun_lunch_end_time; }
        set { this.sun_lunch_end_time = value; }
    }
    private TimeSpan mon_lunch_start_time;
    public TimeSpan MonLunchStartTime
    {
        get { return this.mon_lunch_start_time; }
        set { this.mon_lunch_start_time = value; }
    }
    private TimeSpan mon_lunch_end_time;
    public TimeSpan MonLunchEndTime
    {
        get { return this.mon_lunch_end_time; }
        set { this.mon_lunch_end_time = value; }
    }
    private TimeSpan tue_lunch_start_time;
    public TimeSpan TueLunchStartTime
    {
        get { return this.tue_lunch_start_time; }
        set { this.tue_lunch_start_time = value; }
    }
    private TimeSpan tue_lunch_end_time;
    public TimeSpan TueLunchEndTime
    {
        get { return this.tue_lunch_end_time; }
        set { this.tue_lunch_end_time = value; }
    }
    private TimeSpan wed_lunch_start_time;
    public TimeSpan WedLunchStartTime
    {
        get { return this.wed_lunch_start_time; }
        set { this.wed_lunch_start_time = value; }
    }
    private TimeSpan wed_lunch_end_time;
    public TimeSpan WedLunchEndTime
    {
        get { return this.wed_lunch_end_time; }
        set { this.wed_lunch_end_time = value; }
    }
    private TimeSpan thu_lunch_start_time;
    public TimeSpan ThuLunchStartTime
    {
        get { return this.thu_lunch_start_time; }
        set { this.thu_lunch_start_time = value; }
    }
    private TimeSpan thu_lunch_end_time;
    public TimeSpan ThuLunchEndTime
    {
        get { return this.thu_lunch_end_time; }
        set { this.thu_lunch_end_time = value; }
    }
    private TimeSpan fri_lunch_start_time;
    public TimeSpan FriLunchStartTime
    {
        get { return this.fri_lunch_start_time; }
        set { this.fri_lunch_start_time = value; }
    }
    private TimeSpan fri_lunch_end_time;
    public TimeSpan FriLunchEndTime
    {
        get { return this.fri_lunch_end_time; }
        set { this.fri_lunch_end_time = value; }
    }
    private TimeSpan sat_lunch_start_time;
    public TimeSpan SatLunchStartTime
    {
        get { return this.sat_lunch_start_time; }
        set { this.sat_lunch_start_time = value; }
    }
    private TimeSpan sat_lunch_end_time;
    public TimeSpan SatLunchEndTime
    {
        get { return this.sat_lunch_end_time; }
        set { this.sat_lunch_end_time = value; }
    }
    private DateTime last_batch_run;
    public DateTime LastBatchRun
    {
        get { return this.last_batch_run; }
        set { this.last_batch_run = value; }
    }

    private bool is_deleted;
    public bool IsDeleted
    {
        get { return this.is_deleted; }
        set { this.is_deleted = value; }
    }

    public override string ToString()
    {
        return organisation_id.ToString() + " " + entity_id + " " + parent_organisation.OrganisationID.ToString() + " " + organisationType.OrganisationTypeID.ToString() + " " + organisation_customer_type_id + " " + name.ToString() + " " + acn.ToString() + " " +
                abn.ToString() + " " + organisation_date_added.ToString() + " " + organisation_date_modified.ToString() + " " + is_debtor.ToString() + " " + is_creditor.ToString() + " " +
                bpay_account.ToString() + " " + weeks_per_service_cycle.ToString() + " " + start_date.ToString() + " " +
                end_date.ToString() + " " + comment.ToString() + " " + free_services.ToString() + " " + excl_sun.ToString() + " " + excl_mon.ToString() + " " +
                excl_tue.ToString() + " " + excl_wed.ToString() + " " + excl_thu.ToString() + " " + excl_fri.ToString() + " " + excl_sat.ToString() + " " +
                sun_start_time.ToString() + " " + sun_end_time.ToString() + " " + mon_start_time.ToString() + " " + mon_end_time.ToString() + " " + tue_start_time.ToString() + " " +
                tue_end_time.ToString() + " " + wed_start_time.ToString() + " " + wed_end_time.ToString() + " " + thu_start_time.ToString() + " " + thu_end_time.ToString() + " " +
                fri_start_time.ToString() + " " + fri_end_time.ToString() + " " + sat_start_time.ToString() + " " + sat_end_time.ToString() + " " + last_batch_run.ToString();
    }


    public bool IsAgedCare
    {
        get { return this.OrganisationType.OrganisationTypeID == 139 || this.OrganisationType.OrganisationTypeID == 367 || this.OrganisationType.OrganisationTypeID == 372; }
    }
    public bool IsClinic
    {
        get { return this.OrganisationType.OrganisationTypeID == 218; }
    }
    public bool IsGP
    {
        get { return this.OrganisationType.OrganisationTypeID == 150; }
    }

    public SiteDB.SiteType GetSiteType()
    {
        if (IsAgedCare)
            return SiteDB.SiteType.AgedCare;
        else if (IsGP)
            return SiteDB.SiteType.GP;
        else // if (IsClinic)
            return SiteDB.SiteType.Clinic;
    }

    public bool IsOpen(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday:
                return !this.ExclSun;
            case DayOfWeek.Monday:
                return !this.ExclMon;
            case DayOfWeek.Tuesday:
                return !this.ExclTue;
            case DayOfWeek.Wednesday:
                return !this.ExclWed;
            case DayOfWeek.Thursday:
                return !this.ExclThu;
            case DayOfWeek.Friday:
                return !this.ExclFri;
            case DayOfWeek.Saturday:
                return !this.ExclSat;
            default:
                throw new Exception("Unknown day of week");
        }
    }

    public StartEndTime GetStartEndTime(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday:
                return new StartEndTime(this.SunStartTime, this.SunEndTime);
            case DayOfWeek.Monday:
                return new StartEndTime(this.MonStartTime, this.MonEndTime);
            case DayOfWeek.Tuesday:
                return new StartEndTime(this.TueStartTime, this.TueEndTime);
            case DayOfWeek.Wednesday:
                return new StartEndTime(this.WedStartTime, this.WedEndTime);
            case DayOfWeek.Thursday:
                return new StartEndTime(this.ThuStartTime, this.ThuEndTime);
            case DayOfWeek.Friday:
                return new StartEndTime(this.FriStartTime, this.FriEndTime);
            case DayOfWeek.Saturday:
                return new StartEndTime(this.SatStartTime, this.SatEndTime);
            default:
                throw new Exception("Unknown day of week");
        }
    }
    public StartEndTime GetStartEndLunchTime(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday:
                return new StartEndTime(this.SunLunchStartTime, this.SunLunchEndTime);
            case DayOfWeek.Monday:
                return new StartEndTime(this.MonLunchStartTime, this.MonLunchEndTime);
            case DayOfWeek.Tuesday:
                return new StartEndTime(this.TueLunchStartTime, this.TueLunchEndTime);
            case DayOfWeek.Wednesday:
                return new StartEndTime(this.WedLunchStartTime, this.WedLunchEndTime);
            case DayOfWeek.Thursday:
                return new StartEndTime(this.ThuLunchStartTime, this.ThuLunchEndTime);
            case DayOfWeek.Friday:
                return new StartEndTime(this.FriLunchStartTime, this.FriLunchEndTime);
            case DayOfWeek.Saturday:
                return new StartEndTime(this.SatLunchStartTime, this.SatLunchEndTime);
            default:
                throw new Exception("Unknown day of week");
        }
    }

    public StartEndTime GetStartEndTime(DateTime startDate, DateTime endDate)
    {
        TimeSpan startTime = new TimeSpan(12, 0, 0);
        TimeSpan endTime = new TimeSpan(12, 10, 0);

        DateTime curDate = startDate;
        for (int i = 0; i < 7; i++)
        {
            curDate = curDate.AddDays(1);
            if (curDate > endDate)
                break;

            StartEndTime startendTime = this.GetStartEndTime(curDate.DayOfWeek);
            if (startendTime.StartTime != StartEndTime.NullTimeSpan && startendTime.StartTime < startTime)
                startTime = startendTime.StartTime;
            if (startendTime.EndTime != StartEndTime.NullTimeSpan && startendTime.EndTime > endTime)
                endTime = startendTime.EndTime;
        }
        return new StartEndTime(startTime, endTime);
    }

    public static StartEndTime GetStartEndTime(DateTime startDate, DateTime endDate, Organisation[] orgs)
    {
        TimeSpan startTime = new TimeSpan(12, 0, 0);
        TimeSpan endTime = new TimeSpan(12, 10, 0);

        for (int i = 0; i < orgs.Length; i++)
        {
            StartEndTime startEndTime = orgs[i].GetStartEndTime(startDate, endDate);
            if (startEndTime.StartTime < startTime)
                startTime = startEndTime.StartTime;
            if (startEndTime.EndTime > endTime)
                endTime = startEndTime.EndTime;
        }
        return new StartEndTime(startTime, endTime);
    }




    public static Organisation[] RemoveByID(Organisation[] orgs, int organisation_id_to_remove)
    {
        bool found = false;
        for (int i = 0; i < orgs.Length; i++)
            if (orgs[i].OrganisationID == organisation_id_to_remove)
                found = true;


        Organisation[] newList = new Organisation[found ? orgs.Length - 1 : orgs.Length];

        found = false;
        for (int i = 0; i < orgs.Length; i++)
        {
            if (orgs[i].OrganisationID != organisation_id_to_remove)
                newList[i - (found ? 1 : 0)] = orgs[i];
            else
                found = true;
        }

        return newList;
    }

}