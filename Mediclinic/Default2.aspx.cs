using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;

public partial class _Default2 : System.Web.UI.Page
{

    private struct TyroHealthPointClaimIten
    {
        public string claimAmount;
        public string serviceCode;
        public string description;
        public string serviceReference;
        public string patientId;
        public string serviceDate;

        public TyroHealthPointClaimIten(string claimAmount, string serviceCode, string description, string serviceReference, string patientId, string serviceDate)
        {
            this.claimAmount      = claimAmount;
            this.serviceCode      = serviceCode;
            this.description      = description;
            this.serviceReference = serviceReference;
            this.patientId        = patientId;
            this.serviceDate      = serviceDate;
        }
    }



    protected void Page_Load(object sender, EventArgs e)
    {
        //RunJSON();
        // <a href=\"" + Patient.GetUnsubscribeLink(54950, Session["DB"].ToString()) + "\">Unsubscribe</a>

        //RunEncryptionTest1();
        //RunEncyrptionTest2();

        //Run3();
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        RunGet();
    }

    protected void RunGet()
    {
        string key        = "tyro123";
        string tid        = "200";
        string mid        = "200";
        string format     = "xml";
        string reportType = "detail";
        string date       = DateTime.Today.ToString("yyyyMMdd");
        string url        = "https://integrationsimulator.test.tyro.com/terminals/" + mid + "/" + tid + "/reconciliationReports/" + date + "." + format + "?key=" + key + "&reportType=" + reportType;
        try
        {
            System.Net.HttpStatusCode httpResponseStatusCode;
            string output = Utilities.HttpGet(url, out httpResponseStatusCode, "Mediclinic/1.0");

            if ((int)httpResponseStatusCode != 200)
            {
                lblErrorMessage.Text = "RequestError: " + (int)httpResponseStatusCode + " - " + httpResponseStatusCode.ToString().Replace(Environment.NewLine, "<br />");

                // email alert
            }
            else
            {
                lblOutput.Text = "<pre style=\"white-space:pre;\">" + output.Replace("<", "&lt;").Replace(">", "&gt;") + "</pre>";

                // process result
            }
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = ex.ToString().Replace(Environment.NewLine, "<br />");
        }
    }

    protected void RunJSON()
    {
        // crete claim item list to turn into a JSON object
        List<TyroHealthPointClaimIten> claimItems = new List<TyroHealthPointClaimIten>();
        claimItems.Add(new TyroHealthPointClaimIten(
                "123.45",
                "abc",
                "descr1",
                "serviceRef1",
                "ptID1",
                "serviceDate1"));
        claimItems.Add(new TyroHealthPointClaimIten(
                "987.65",
                "def",
                "descr2",
                "serviceRef2",
                "ptID2",
                "serviceDate2"));

        // convert to JSON array
        string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(claimItems);

        // save JSON array on the page accessable to JS to send
        Page.ClientScript.RegisterStartupScript(this.GetType(), "claim_items", "<script language=javascript>var allClaimItems = " + json + ";</script>");

        // to test it worked
        Page.ClientScript.RegisterStartupScript(this.GetType(), "Confi", "var pp='';  for(var i=0; i<allClaimItems.length; i++) { pp += allClaimItems[i].claimAmount + '\\r\\n';} alert(pp);", true);
    }

    protected void RunEncyrptionTest2()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("<table>");

        string pt1 = "Mediclinic_0001" + "__" + 54950.ToString().PadLeft(6, '0');
        sb.AppendLine("<t><td>" + pt1 + "</td><td>" + SimpleAES.Encrypt(SimpleAES.KeyType.EmailUnsubscribe, pt1) + "</td><td>" + SimpleAES.Decrypt(SimpleAES.KeyType.EmailUnsubscribe, SimpleAES.Encrypt(SimpleAES.KeyType.EmailUnsubscribe, pt1)) + "</td><td><a href=\"" + Patient.GetUnsubscribeLink(54950, Session["DB"].ToString()) + "\">Unsubscribe</a></td></tr>");

        string pt2 = "Mediclinic_0001" + "__" + 68335.ToString().PadLeft(6, '0');
        sb.AppendLine("<t><td>" + pt2 + "</td><td>" + SimpleAES.Encrypt(SimpleAES.KeyType.EmailUnsubscribe, pt2) + "</td><td>" + SimpleAES.Decrypt(SimpleAES.KeyType.EmailUnsubscribe, SimpleAES.Encrypt(SimpleAES.KeyType.EmailUnsubscribe, pt2)) + "</td><td><a href=\"" + Patient.GetUnsubscribeLink(68335, Session["DB"].ToString()) + "\">Unsubscribe</a></td></tr>");

        sb.AppendLine("</table>");
        lblOutput.Text = sb.ToString() + "<br />";
    }

    protected void RunEncryptionTest1()
    {
        /*
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("<table>");

        string inv1 = "Mediclinic_0001" + "__" + 329508.ToString().PadLeft(6, '0');
        sb.AppendLine("<t><td>" + inv1 + "</td><td>" + SimpleAES.Encrypt(SimpleAES.KeyType.Invoices, inv1) + "</td><td>" + SimpleAES.Decrypt(SimpleAES.KeyType.Invoices, SimpleAES.Encrypt(SimpleAES.KeyType.Invoices, inv1)) + "</td></tr>");

        inv1 = "Mediclinic_0001" + "__" + 329605.ToString().PadLeft(6, '0');
        sb.AppendLine("<t><td>" + inv1 + "</td><td>" + SimpleAES.Encrypt(SimpleAES.KeyType.Invoices, inv1) + "</td><td>" + SimpleAES.Decrypt(SimpleAES.KeyType.Invoices, SimpleAES.Encrypt(SimpleAES.KeyType.Invoices, inv1)) + "</td></tr>");

        for (int i = 1; i < 50; i++)
        {

            // can attach db, inv nbr, patient_id (if needed), anything else..
            string DB = i % 2 == 0 ? "Mediclinic_0001" : "Mediclinic_0023";
            string inv = DB + "__" + i.ToString().PadLeft(6, '0');

            sb.AppendLine("<t><td>" + inv + "</td><td>" + SimpleAES.Encrypt(SimpleAES.KeyType.Invoices, inv) + "</td><td>" + SimpleAES.Decrypt(SimpleAES.KeyType.Invoices, SimpleAES.Encrypt(SimpleAES.KeyType.Invoices, inv)) + "</td></tr>");
        }

        sb.AppendLine("</table>");
        lblOutput.Text = sb.ToString() + "<br />";
        */
    }




    protected void btnSubmit_Click_1(object sender, EventArgs e)
    {
        lblOutput.Text = colorPicker.Value;



        Patient patient = PatientDB.GetByID(54950);
        if (patient == null)
            throw new Exception("Invalid url id");

        Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);
        foreach (ListItem li in chkBoxListConditions.Items)
        {
            if (li.Selected && selectedConditions[Convert.ToInt32(li.Value)] == null)
                PatientConditionDB.Insert(patient.PatientID, Convert.ToInt32(li.Value), DateTime.MinValue, 0, "", false);

            if (!li.Selected && selectedConditions[Convert.ToInt32(li.Value)] != null)
                PatientConditionDB.Delete(patient.PatientID, Convert.ToInt32(li.Value));
        }

        //SetErrorMessage("Updated");
        Response.Redirect(Request.RawUrl);
    }

    protected void Run3()
    {

        //ReferralDB.Insert(26963, 2, new DateTime(2014, 1, 2), new DateTime(2014, 1, 2), -4, DateTime.Now, -1, DateTime.MinValue);
        //ReferralDB.Insert(26963, 3, new DateTime(2014, 1, 2), new DateTime(2014, 1, 2), -4, DateTime.Now, -1, DateTime.MinValue);

        //ReferralDB.Update(1, 26963, 1, new DateTime(2014, 1, 2), new DateTime(2014, 1, 2), -4, DateTime.Now, -1, DateTime.MinValue);

        //ReferralRemainingDB.Insert(1, 68, 1);
        //ReferralRemainingDB.Insert(1, 277, 2);
        //ReferralRemainingDB.Insert(2, 68, 3);
        //ReferralRemainingDB.Insert(2, 277, 4);

        //ReferralRemainingDB.Update(4, 1, 68, 4);

    }

    protected void Run2()
    {

        //// // PresentationFramework.dll


        //Microsoft.WindowsAPICodePack.Shell.PropertySystem.PropertyKey[

        //var shellFile = ShellFile.FromParsingName(filePath);
        //ShellPropertyWriter w = shellFile.Properties.GetPropertyWriter();
        //try
        //{
        //    w.WriteProperty(SystemProperties.System.Author, new string[] { "MyTest", "Test" });
        //    w.WriteProperty(SystemProperties.System.Music.Artist, new string[] { "MyTest", "Test" });
        //    w.WriteProperty(SystemProperties.System.Music.DisplayArtist, "Test");
        //}
        //catch (Exception ex)
        //{

        //}
        //w.Close();

    }

    protected void Run()
    {
        Booking[] bookings = GetBookingsSortedByProviderTimeOrganisation(DateTime.Today.Date.AddDays(0));
        ArrayList list     = OrganiseData(bookings);


        // change list from   Tuple<Staff, ArrayList>
        // to                 Tuple<Staff, string, ArrayList>  -- string is mobile number or null if none



        string output = string.Empty;
        for (int i = 0; i < bookings.Length; i++)
            output += bookings[i].BookingID + " " + bookings[i].Provider.StaffID + " " + bookings[i].Organisation.OrganisationID + " [" + bookings[i].DateStart.ToString("HH:mm") + " - " + bookings[i].DateEnd.ToString("HH:mm") + "]" + "<br />";


        for (int i = 0; i < list.Count; i++)
        {
            Tuple<Staff, ArrayList> t = (Tuple<Staff, ArrayList>)list[i];
            output += "<br /><br /><b><u>" + t.Item1.Person.FullnameWithoutMiddlename + " [ID:" + t.Item1.StaffID + "]</u></b>";


            string text = "Hi " + t.Item1.Person.Firstname + Environment.NewLine + "Tomorrow you have appointments at:" + Environment.NewLine;
            for (int j = 0; j < t.Item2.Count; j++)
            {
                Tuple<Organisation, DateTime, DateTime> t2 = (Tuple<Organisation, DateTime, DateTime>)t.Item2[j];

                text += Environment.NewLine + "[" + ConvertTime(t2.Item2) + " - " + ConvertTime(t2.Item3) + "] " + t2.Item1.Name;
                output += "<br />" + t2.Item1.Name + " [ID:" + t2.Item1.OrganisationID + "] " + ConvertTime(t2.Item2) + " - " + ConvertTime(t2.Item3);
            }

            output += "<br /><br />" + "<b><font color=\"blue\">" + text.Replace(Environment.NewLine, "<br />") + "</font></b>";
        }

        lblOutput.Text += output;
    }


    protected string ConvertTime(DateTime dt)
    {
        return dt.ToString("h:mm") + (dt.Hour < 12 ? "am" : "pm");
    }


    protected Booking[] GetBookingsSortedByProviderTimeOrganisation(DateTime date)
    {
        // get bookings
        date = date.Date;
        Booking[] bookings = BookingDB.GetBetween(date, date.AddHours(23).AddMinutes(59), null, null, null, null);

        // remove unavailabilities
        ArrayList list = new ArrayList();
        for (int i = 0; i < bookings.Length; i++)
            if (bookings[i].BookingTypeID == 34)
                list.Add(bookings[i]);
        bookings = (Booking[])list.ToArray(typeof(Booking));

        // sort by prov, bk time, org
        Array.Sort(bookings, BkCompare);

        return bookings;
    }

    protected int BkCompare(Booking x, Booking y)
    {
        if (x.Provider.StaffID > y.Provider.StaffID)
            return 1;
        if (x.Provider.StaffID < y.Provider.StaffID)
            return -1;

        if (x.DateStart > y.DateStart)
            return 1;
        if (x.DateStart < y.DateStart)
            return -1;

        if (x.Organisation.OrganisationID > y.Organisation.OrganisationID)
            return 1;
        if (x.Organisation.OrganisationID < y.Organisation.OrganisationID)
            return -1;

        return 0;
    }


    // list of   Tuple<Staff, ArrayList>
    // each provicer has list of  Tuple<Organisation, DateTime, DateTime>  -- start/end times
    protected ArrayList OrganiseData(Booking[] bookings)
    {
        ArrayList list = new ArrayList();

        Staff        curProvider  = null;
        Organisation curOrg       = null;
        DateTime     curStartTime = DateTime.MinValue;
        DateTime     curEndTime   = DateTime.MinValue;
 

        for (int i = 0; i < bookings.Length; i++)
        {
            if (curProvider == null || curOrg == null)
            {
                curProvider  = bookings[i].Provider;
                curOrg       = bookings[i].Organisation;
                curStartTime = bookings[i].DateStart;
                curEndTime   = bookings[i].DateEnd;
            }
            else 
            {
                if (curProvider.StaffID == bookings[i].Provider.StaffID && curOrg.OrganisationID == bookings[i].Organisation.OrganisationID)
                {
                    curEndTime = bookings[i].DateEnd;
                }
                else
                {
                    Add(ref list, curProvider, curOrg, curStartTime, curEndTime);

                    curProvider  = bookings[i].Provider;
                    curOrg       = bookings[i].Organisation;
                    curStartTime = bookings[i].DateStart;
                    curEndTime   = bookings[i].DateEnd;
                }
            }
        }

        // add the last one
        if (curProvider != null || curOrg != null)
            Add(ref list, curProvider, curOrg, curStartTime, curEndTime);

        return list;
    }

    protected void Add(ref ArrayList list, Staff staff, Organisation org, DateTime start, DateTime end)
    {
        Tuple<Organisation, DateTime, DateTime> t = new Tuple<Organisation, DateTime, DateTime>(org, start, end);

        bool providerFound = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (((Tuple<Staff, ArrayList>)list[i]).Item1.StaffID == staff.StaffID)
            {
                ((ArrayList)((Tuple<Staff, ArrayList>)list[i]).Item2).Add(t);
                providerFound = true;
            }
        }

        if (!providerFound)
        {
            Tuple<Staff, ArrayList> t2 = new Tuple<Staff, ArrayList>(staff, new ArrayList());
            ((ArrayList)((Tuple<Staff, ArrayList>)t2).Item2).Add(t);
            list.Add(t2);
        }
    }


    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        SetErrorMessage(errMsg, details);
    }
    private void SetErrorMessage(string errMsg = "", string details = "")
    {
        if (errMsg.Contains(Environment.NewLine))
            errMsg = errMsg.Replace(Environment.NewLine, "<br />");

        // double escape so shows up literally on webpage for 'alert' message
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"#\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

        lblErrorMessage.Visible = true;
        if (errMsg != null && errMsg.Length > 0)
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}