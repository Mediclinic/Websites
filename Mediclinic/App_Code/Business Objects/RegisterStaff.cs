using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class RegisterStaff
{

    public RegisterStaff(int register_staff_id, DateTime register_staff_date_added, int organisation_id, int staff_id, 
                         string provider_number, bool main_provider_for_clinic,
                         bool excl_sun, bool excl_mon, bool excl_tue, bool excl_wed, bool excl_thu, bool excl_fri, bool excl_sat)
    {
        this.register_staff_id = register_staff_id;
        this.register_staff_date_added = register_staff_date_added;
        this.organisation = new Organisation(organisation_id);
        this.staff = new Staff(staff_id);
        this.provider_number = provider_number;
        this.main_provider_for_clinic = main_provider_for_clinic;
        this.excl_sun = excl_sun;
        this.excl_mon = excl_mon;
        this.excl_tue = excl_tue;
        this.excl_wed = excl_wed;
        this.excl_thu = excl_thu;
        this.excl_fri = excl_fri;
        this.excl_sat = excl_sat;
    }

    private int register_staff_id;
    public int RegisterStaffID
    {
        get { return this.register_staff_id; }
        set { this.register_staff_id = value; }
    }
    private DateTime register_staff_date_added;
    public DateTime RegisterStaffDateAdded
    {
        get { return this.register_staff_date_added; }
        set { this.register_staff_date_added = value; }
    }
    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    private string provider_number;
    public string ProviderNumber
    {
        get { return this.provider_number; }
        set { this.provider_number = value; }
    }
    private bool main_provider_for_clinic;
    public bool MainProviderForClinic
    {
        get { return this.main_provider_for_clinic; }
        set { this.main_provider_for_clinic = value; }
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

    public override string ToString()
    {
        return register_staff_id.ToString() + " " + register_staff_date_added.ToString() + " " + organisation.OrganisationID.ToString() + " " + staff.StaffID.ToString() + " " + provider_number.ToString() + " " + 
                main_provider_for_clinic.ToString();
    }

}