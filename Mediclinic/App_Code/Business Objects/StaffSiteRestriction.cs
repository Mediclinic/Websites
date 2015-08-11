using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class StaffSiteRestriction
{

    public StaffSiteRestriction(int staff_site_restriction_id, int staff_id, int site_id)
    {
        this.staff_site_restriction_id = staff_site_restriction_id;
        this.staff = new Staff(staff_id);
        this.site  = new Site(site_id);
    }

    private int staff_site_restriction_id;
    public int StaffSiteRestrictionID
    {
        get { return this.staff_site_restriction_id; }
        set { this.staff_site_restriction_id = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    private Site site;
    public Site Site
    {
        get { return this.site; }
        set { this.site = value; }
    }
    public override string ToString()
    {
        return staff_site_restriction_id.ToString() + " " + staff.StaffID.ToString() + " " + site.SiteID.ToString();
    }

}