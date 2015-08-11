using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Staff
{

    public Staff(int staff_id, int person_id, string login, string pwd, int staff_position_id, int field_id, int costcentre_id,
                bool is_contractor, string tfn, string provider_number, bool is_fired, bool is_commission, decimal commission_percent,
                bool is_stakeholder, bool is_master_admin, bool is_admin, bool is_principal, bool is_provider, bool is_external,
                DateTime staff_date_added, DateTime start_date, DateTime end_date, string comment,
                int num_days_to_display_on_booking_screen, bool show_header_on_booking_screen, int bk_screen_field_id, bool bk_screen_show_key, bool enable_daily_reminder_sms, bool enable_daily_reminder_email)
    {
        this.staff_id                      = staff_id;
        this.person                        = new Person(person_id);
        this.login                         = login;
        this.pwd                           = pwd;
        this.staffPosition                 = new StaffPosition(staff_position_id);
        this.field                         = new IDandDescr(field_id);
        this.costCentre                    = new CostCentre(costcentre_id);
        this.is_contractor                 = is_contractor;
        this.tfn                           = tfn;
        this.provider_number               = provider_number;
        this.is_fired                      = is_fired;
        this.is_commission                 = is_commission;
        this.commission_percent            = commission_percent;
        this.is_stakeholder                = is_stakeholder;
        this.is_master_admin               = is_master_admin;
        this.is_admin                      = is_admin;
        this.is_principal                  = is_principal;
        this.is_provider                   = is_provider;
        this.is_external                   = is_external;
        this.staff_date_added              = staff_date_added;
        this.start_date                    = start_date;
        this.end_date                      = end_date;
        this.comment                       = comment;
        this.num_days_to_display_on_booking_screen = num_days_to_display_on_booking_screen;
        this.show_header_on_booking_screen = show_header_on_booking_screen;
        this.bk_screen_field               = bk_screen_field_id == -1 ? null : new IDandDescr(bk_screen_field_id);
        this.bk_screen_show_key            = bk_screen_show_key;
        this.enable_daily_reminder_sms     = enable_daily_reminder_sms;
        this.enable_daily_reminder_email   = enable_daily_reminder_email;

    }
    public Staff(int staff_id)
    {
        this.staff_id = staff_id;
    }

    private int staff_id;
    public int StaffID
    {
        get { return this.staff_id; }
        set { this.staff_id = value; }
    }
    private Person person;
    public Person Person
    {
        get { return this.person; }
        set { this.person = value; }
    }
    private string login;
    public string Login
    {
        get { return this.login; }
        set { this.login = value; }
    }
    private string pwd;
    public string Pwd
    {
        get { return this.pwd; }
        set { this.pwd = value; }
    }
    private StaffPosition staffPosition;
    public StaffPosition StaffPosition
    {
        get { return this.staffPosition; }
        set { this.staffPosition = value; }
    }
    private IDandDescr field;
    public IDandDescr Field
    {
        get { return this.field; }
        set { this.field = value; }
    }
    private CostCentre costCentre;
    public CostCentre CostCentre
    {
        get { return this.costCentre; }
        set { this.costCentre = value; }
    }
    private bool is_contractor;
    public bool IsContractor
    {
        get { return this.is_contractor; }
        set { this.is_contractor = value; }
    }
    private string tfn;
    public string Tfn
    {
        get { return this.tfn; }
        set { this.tfn = value; }
    }
    private string provider_number;
    public string ProviderNumber
    {
        get { return this.provider_number; }
        set { this.provider_number = value; }
    }
    private bool is_fired;
    public bool IsFired
    {
        get { return this.is_fired; }
        set { this.is_fired = value; }
    }
    private bool is_commission;
    public bool IsCommission
    {
        get { return this.is_commission; }
        set { this.is_commission = value; }
    }
    private decimal commission_percent;
    public decimal CommissionPercent
    {
        get { return this.commission_percent; }
        set { this.commission_percent = value; }
    }
    private bool is_stakeholder;
    public bool IsStakeholder
    {
        get { return this.is_stakeholder; }
        set { this.is_stakeholder = value; }
    }
    private bool is_master_admin;
    public bool IsMasterAdmin
    {
        get { return this.is_master_admin; }
        set { this.is_master_admin = value; }
    }
    private bool is_admin;
    public bool IsAdmin
    {
        get { return this.is_admin; }
        set { this.is_admin = value; }
    }
    private bool is_principal;
    public bool IsPrincipal
    {
        get { return this.is_principal; }
        set { this.is_principal = value; }
    }
    private bool is_provider;
    public bool IsProvider
    {
        get { return this.is_provider; }
        set { this.is_provider = value; }
    }
    private bool is_external;
    public bool IsExternal
    {
        get { return this.is_external; }
        set { this.is_external = value; }
    }
    private DateTime staff_date_added;
    public DateTime StaffDateAdded
    {
        get { return this.staff_date_added; }
        set { this.staff_date_added = value; }
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

    private int num_days_to_display_on_booking_screen;
    public int NumDaysToDisplayOnBookingScreen
    {
        get { return this.num_days_to_display_on_booking_screen; }
        set { this.num_days_to_display_on_booking_screen = value; }
    }
    private bool show_header_on_booking_screen;
    public bool ShowHeaderOnBookingScreen
    {
        get { return this.show_header_on_booking_screen; }
        set { this.show_header_on_booking_screen = value; }
    }
    private IDandDescr bk_screen_field;
    public IDandDescr BookingScreenField
    {
        get { return this.bk_screen_field; }
        set { this.bk_screen_field = value; }
    }
    private bool bk_screen_show_key;
    public bool BookingScreenShowKey
    {
        get { return this.bk_screen_show_key; }
        set { this.bk_screen_show_key = value; }
    }
    private bool enable_daily_reminder_sms;
    public bool EnableDailyReminderSMS
    {
        get { return this.enable_daily_reminder_sms; }
        set { this.enable_daily_reminder_sms = value; }
    }
    private bool enable_daily_reminder_email;
    public bool EnableDailyReminderEmail
    {
        get { return this.enable_daily_reminder_email; }
        set { this.enable_daily_reminder_email = value; }
    }



    public override string ToString()
    {
        return staff_id.ToString() + " " + person.PersonID.ToString() + " " + login.ToString() + " " + pwd.ToString() + " " + staffPosition.StaffPositionID.ToString() + " " +
                field.ID.ToString() + " " + costCentre.CostCentreID.ToString() + " " + is_contractor.ToString() + " " + tfn.ToString() + " " + provider_number.ToString() + " " + 
                is_commission.ToString() + " " + commission_percent.ToString() + " " +
                is_stakeholder.ToString() + " " + is_principal.ToString() + " " + is_admin.ToString() + " " + staff_date_added.ToString() + " " + start_date.ToString() + " " + 
                end_date.ToString() + " " + comment.ToString();
    }

    public static Staff[] RemoveByID(Staff[] inList, int staffIDToRemove)
    {
        System.Collections.ArrayList newList = new System.Collections.ArrayList();
        for (int i = 0; i < inList.Length; i++)
        {
            if (inList[i].StaffID != staffIDToRemove)
                newList.Add(inList[i]);
        }

        return (Staff[])newList.ToArray(typeof(Staff));
    }

}