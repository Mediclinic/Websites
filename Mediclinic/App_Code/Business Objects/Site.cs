using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Site
{

    public Site(int site_id, int entity_id, string name, int site_type_id, string abn, string acn, string tfn,
                string asic, bool is_provider, string bank_bpay, string bank_bsb, string bank_account, string bank_direct_debit_userid,
                string bank_username, decimal oustanding_balance_warning, bool print_epc, bool excl_sun, bool excl_mon, bool excl_tue,
                bool excl_wed, bool excl_thu, bool excl_fri, bool excl_sat, TimeSpan day_start_time, TimeSpan lunch_start_time,
                TimeSpan lunch_end_time, TimeSpan day_end_time, DateTime fiscal_yr_end, int num_booking_months_to_get)
    {
        this.site_id = site_id;
        this.entity_id = entity_id;
        this.name = name;
        this.site_type = new IDandDescr(site_type_id);
        this.abn = abn;
        this.acn = acn;
        this.tfn = tfn;
        this.asic = asic;
        this.is_provider = is_provider;
        this.bank_bpay = bank_bpay;
        this.bank_bsb = bank_bsb;
        this.bank_account = bank_account;
        this.bank_direct_debit_userid = bank_direct_debit_userid;
        this.bank_username = bank_username;
        this.oustanding_balance_warning = oustanding_balance_warning;
        this.print_epc = print_epc;
        this.excl_sun = excl_sun;
        this.excl_mon = excl_mon;
        this.excl_tue = excl_tue;
        this.excl_wed = excl_wed;
        this.excl_thu = excl_thu;
        this.excl_fri = excl_fri;
        this.excl_sat = excl_sat;
        this.day_start_time = day_start_time;
        this.lunch_start_time = lunch_start_time;
        this.lunch_end_time = lunch_end_time;
        this.day_end_time = day_end_time;
        this.fiscal_yr_end = fiscal_yr_end;
        this.num_booking_months_to_get = num_booking_months_to_get;
    }
    public Site(int site_id)
    {
        this.site_id = site_id;
    }

    private int site_id;
    public int SiteID
    {
        get { return this.site_id; }
        set { this.site_id = value; }
    }
    private int entity_id;
    public int EntityID
    {
        get { return this.entity_id; }
        set { this.entity_id = value; }
    }
    private string name;
    public string Name
    {
        get { return this.name; }
        set { this.name = value; }
    }
    private IDandDescr site_type;
    public IDandDescr SiteType
    {
        get { return this.site_type; }
        set { this.site_type = value; }
    }
    private string abn;
    public string Abn
    {
        get { return this.abn; }
        set { this.abn = value; }
    }
    private string acn;
    public string Acn
    {
        get { return this.acn; }
        set { this.acn = value; }
    }
    private string tfn;
    public string Tfn
    {
        get { return this.tfn; }
        set { this.tfn = value; }
    }
    private string asic;
    public string Asic
    {
        get { return this.asic; }
        set { this.asic = value; }
    }
    private bool is_provider;
    public bool IsProvider
    {
        get { return this.is_provider; }
        set { this.is_provider = value; }
    }
    private string bank_bpay;
    public string BankBpay
    {
        get { return this.bank_bpay; }
        set { this.bank_bpay = value; }
    }
    private string bank_bsb;
    public string BankBsb
    {
        get { return this.bank_bsb; }
        set { this.bank_bsb = value; }
    }
    private string bank_account;
    public string BankAccount
    {
        get { return this.bank_account; }
        set { this.bank_account = value; }
    }
    private string bank_direct_debit_userid;
    public string BankDirectDebitUserID
    {
        get { return this.bank_direct_debit_userid; }
        set { this.bank_direct_debit_userid = value; }
    }
    private string bank_username;
    public string BankUsername
    {
        get { return this.bank_username; }
        set { this.bank_username = value; }
    }
    private decimal oustanding_balance_warning;
    public decimal OustandingBalanceWarning
    {
        get { return this.oustanding_balance_warning; }
        set { this.oustanding_balance_warning = value; }
    }
    private bool print_epc;
    public bool PrintEpc
    {
        get { return this.print_epc; }
        set { this.print_epc = value; }
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
    private TimeSpan day_start_time;
    public TimeSpan DayStartTime
    {
        get { return this.day_start_time; }
        set { this.day_start_time = value; }
    }
    private TimeSpan lunch_start_time;
    public TimeSpan LunchStartTime
    {
        get { return this.lunch_start_time; }
        set { this.lunch_start_time = value; }
    }
    private TimeSpan lunch_end_time;
    public TimeSpan LunchEndTime
    {
        get { return this.lunch_end_time; }
        set { this.lunch_end_time = value; }
    }
    private TimeSpan day_end_time;
    public TimeSpan DayEndTime
    {
        get { return this.day_end_time; }
        set { this.day_end_time = value; }
    }
    private DateTime fiscal_yr_end;
    public DateTime FiscalYrEnd
    {
        get { return this.fiscal_yr_end; }
        set { this.fiscal_yr_end = value; }
    }
    private int num_booking_months_to_get;
    public int NumBookingMonthsToGet
    {
        get { return this.num_booking_months_to_get; }
        set { this.num_booking_months_to_get = value; }
    }
    public override string ToString()
    {
        return site_id.ToString() + " " + entity_id.ToString() + " " + name.ToString() + " " + site_type.ID.ToString() + " " + abn.ToString() + " " + acn.ToString() + " " +
                tfn.ToString() + " " + asic.ToString() + " " + is_provider.ToString() + " " + bank_bpay.ToString() + " " + bank_bsb.ToString() + " " +
                bank_account.ToString() + " " + bank_direct_debit_userid.ToString() + " " + bank_username.ToString() + " " + oustanding_balance_warning.ToString() + " " + print_epc.ToString() + " " +
                excl_sun.ToString() + " " + excl_mon.ToString() + " " + excl_tue.ToString() + " " + excl_wed.ToString() + " " + excl_thu.ToString() + " " +
                excl_fri.ToString() + " " + excl_sat.ToString() + " " + day_start_time.ToString() + " " + lunch_start_time.ToString() + " " + lunch_end_time.ToString() + " " +
                day_end_time.ToString() + " " + fiscal_yr_end.ToString() + " " + num_booking_months_to_get.ToString();
    }

}