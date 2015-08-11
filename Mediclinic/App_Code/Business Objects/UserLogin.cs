using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class UserLogin
{

    public UserLogin(int userlogin_id, int staff_id, int patient_id , string username, int site_id, bool is_successful, bool is_logged_off, string session_id, DateTime login_time,
                DateTime last_access_time, string last_access_page, string ipaddress)
    {
        this.userlogin_id = userlogin_id;
        this.staff   = staff_id == -1 ? null : new Staff(staff_id);
        this.patient = patient_id == -1 ? null : new Patient(patient_id);
        this.username = username;
        this.site = site_id < 0 ? null : new Site(site_id);
        this.is_successful = is_successful;
        this.is_logged_off = is_logged_off;
        this.session_id = session_id;
        this.login_time = login_time;
        this.last_access_time = last_access_time;
        this.last_access_page = last_access_page;
        this.ipaddress = ipaddress;
    }

    private int userlogin_id;
    public int UserloginID
    {
        get { return this.userlogin_id; }
        set { this.userlogin_id = value; }
    }
    private Staff staff;
    public Staff Staff
    {
        get { return this.staff; }
        set { this.staff = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private string username;
    public string Username
    {
        get { return this.username; }
        set { this.username = value; }
    }
    private Site site;
    public Site Site
    {
        get { return this.site; }
        set { this.site = value; }
    }
    private bool is_successful;
    public bool IsSuccessful
    {
        get { return this.is_successful; }
        set { this.is_successful = value; }
    }
    private bool is_logged_off;
    public bool IsLoggedOff
    {
        get { return this.is_logged_off; }
        set { this.is_logged_off = value; }
    }
    private string session_id;
    public string SessionID
    {
        get { return this.session_id; }
        set { this.session_id = value; }
    }
    private DateTime login_time;
    public DateTime LoginTime
    {
        get { return this.login_time; }
        set { this.login_time = value; }
    }
    private DateTime last_access_time;
    public DateTime LastAccessTime
    {
        get { return this.last_access_time; }
        set { this.last_access_time = value; }
    }
    private string last_access_page;
    public string LastAccessPage
    {
        get { return this.last_access_page; }
        set { this.last_access_page = value; }
    }
    private string ipaddress;
    public string Ipaddress
    {
        get { return this.ipaddress; }
        set { this.ipaddress = value; }
    }
    public override string ToString()
    {
        return userlogin_id.ToString() + " " + staff.StaffID.ToString() + " " + username.ToString() + " " + is_successful.ToString() + " " + is_logged_off.ToString() + " " + session_id.ToString() + " " +
                login_time.ToString() + " " + last_access_time.ToString() + " " + ipaddress.ToString();
    }

}