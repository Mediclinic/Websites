using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Configuration;
using System.Collections;

using X509Certificates = System.Security.Cryptography.X509Certificates;

using System.Drawing;



public partial class _Default1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["StaffFirstname"] != null)
            lblStaffName.Text = Session["StaffFirstname"].ToString();

        /*
        string output = string.Empty;
        foreach (BookingSlot.Type type in Enum.GetValues(typeof(BookingSlot.Type)))
        {
            if (type == BookingSlot.Type.PatientNotSet ||
                type == BookingSlot.Type.ServiceNotSet ||
                type == BookingSlot.Type.PatientAndServiceNotSet ||
                type == BookingSlot.Type.None)
                continue;

            output += "<br>" + type + " [" + System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(type)) + "] [" + System.Drawing.ColorTranslator.ToHtml(HSLColor.ChangeBrightness(BookingSlot.GetColor(type), -10)) + "]";
        }
        lblBlah.Text = output;
        */

        //lblBlah.Text = " [" + System.Drawing.ColorTranslator.ToHtml(HSLColor.ChangeBrightness(System.Drawing.Color.Tan, 20)) + "]";

        



        btnBlah.Visible = Utilities.IsDev();
        lblBlah.Visible = Utilities.IsDev();
        Button2.Visible = Utilities.IsDev();
        Label2.Visible  = Utilities.IsDev();
        Button3.Visible = Utilities.IsDev();
        Label3.Visible  = Utilities.IsDev();
        Button4.Visible = false; Utilities.IsDev();
        Label4.Visible  = false; Utilities.IsDev();
        pnlBlah.Visible = false; Utilities.IsDev();
    }


    protected void btnBlah_Click(object sender, EventArgs e)
    {

        DataTable dt = InvoiceDB.GetAllOutstandingByPatientAsReport(Convert.ToInt32(Session["SiteID"]));
        dt.DefaultView.Sort = "total_due DESC";
        dt = dt.DefaultView.ToTable();

        // now can put into gridview so can sort by amount owing or name etc..  -- and put sum at bottum and have paging

        // GridView1.Columns[ColumnIndex].HeaderText = "Patient (Count: " + dt.Rows.Count + ")";


        string output = string.Empty;
        output += "<table cellpadding=\"4\"><tr><th>Patient</th><th>Total due (Count)</th><th>View PT Invoices</th></tr>";
        for (int i = 0; i < dt.Rows.Count; i++)
            output += "<tr valign=\"top\"><td><a href='AddEditPatient.aspx?type=view&id=" + dt.Rows[i]["patient_id"] + "'>" + dt.Rows[i]["patient_fullname"] + "</a></td><td><b>" + string.Format("{0:C}", Convert.ToDecimal(dt.Rows[i]["total_due"])) + "</b> (" + dt.Rows[i]["total_inv_count"] + ")</td><td><a href='InvoiceInfo.aspx?patient=" + dt.Rows[i]["patient_id"] + "&inc_medicare=0&inc_dva=0&inc_private=1&inc_paid=0&inc_unpaid=1'>Unpaid Invoices</a> &nbsp;&nbsp; <a href='InvoiceInfo.aspx?patient=" + dt.Rows[i]["patient_id"] + "'>All Invoices</a></td></tr>";
        output += "</table>";

        string totalInfoTbl = @"
            <table>
                <tr><td>Total Patients</td><td> = <b>" + dt.Rows.Count + @"</b></td></tr>
                <tr><td>Total Owed</td><td> = <b>" + string.Format("{0:C}", (decimal)dt.Compute("Sum(total_due)", "")) + @"</b></td></tr>
            </table>";

        lblBlah.Text = totalInfoTbl + output;

        return;



        RegisterPatient[] regPts = RegisterPatientDB.GetAll(true, true, true, "6,2,3");
        //RegisterPatient[] regPts = RegisterPatientDB.GetAll(false, false, true, "6,2,3");
        Hashtable patHash = new Hashtable();
        Hashtable orgHash = new Hashtable();

        for (int i = 0; i < regPts.Length; i++)
        {
            if (patHash[regPts[i].Patient.PatientID] == null)
                patHash[regPts[i].Patient.PatientID] = new ArrayList();
            if (orgHash[regPts[i].Organisation.OrganisationID] == null)
                orgHash[regPts[i].Organisation.OrganisationID] = new ArrayList();

            ((ArrayList)patHash[regPts[i].Patient.PatientID]).Add(regPts[i]);
            ((ArrayList)orgHash[regPts[i].Organisation.OrganisationID]).Add(regPts[i]);
        }



        string output1 = string.Empty, output2 = string.Empty;
        output1 += "<table>";


        Organisation[] flattenedTree = OrganisationTree.GetFlattenedTree(null, false, 0, false, "139,367,372");
        for (int i = 0; i < flattenedTree.Length; i++)
        {
            Organisation org = flattenedTree[i];

            string parentsList = string.Empty;
            Organisation curOrg = org.ParentOrganisation;
            while (curOrg != null)
            {
                parentsList += (parentsList.Length == 0 ? "" : ", ") + "<b>" + curOrg.Name + "</b>";
                curOrg = curOrg.ParentOrganisation;
            }

            int nPatients = orgHash[org.OrganisationID] == null ? 0 : ((ArrayList)orgHash[org.OrganisationID]).Count;

            output1 += "<tr>";
            output1 += "<td><a href='/AddEditOrganisation.aspx?type=view&id=" + org.OrganisationID + "'>" + org.Name + "</a></td>";
            output1 += "<td>" + org.OrganisationID + "</td>";
            output1 += "<td>" + (org.ParentOrganisation == null ? "0" : org.ParentOrganisation.OrganisationID.ToString()) + "</td>";
            output1 += "<td>" + org.TreeLevel + "</td>";
            output1 += "<td>" + parentsList + "</td>";
            output1 += "</tr>";


            // link:   "~/AddEditOrganisation.aspx?type=view&id=" + org.OrganisationID

            if (org.TreeLevel == 0)
                output2 += "<br />";
            for (int j = 0; j < org.TreeLevel; j++)
                output2 += "&nbsp;&nbsp;&nbsp;&nbsp;";
            output2 += "<a href='/AddEditOrganisation.aspx?type=view&id=" + org.OrganisationID + "'>" + org.Name + "</a>" + " (" + org.OrganisationType.Descr.Replace("Aged Care ", "") + " " + nPatients + " pts)<br />";
            //output2 += " - [<b>" + org.OrganisationType.Descr + "</b>] " + "<a href='/AddEditOrganisation.aspx?type=view&id=" + org.OrganisationID + "'>" + org.Name + "</a>" + "[" + org.OrganisationID + "] [<b>" + nPatients + "</b>]<br />";
        }

        lblBlah.Text = output2;


        regPts  = null;
        patHash = null;
        orgHash = null;
        GC.Collect();
        


        /*
        bool onlyWithValidMedicareEPCRemaining = true;
        bool onlyWithValidDVAEPCRemaining      = false;


        string sql = @"

        SELECT * FROM Patient WHERE 1=1 "
        + (!onlyWithValidMedicareEPCRemaining ? "" :
	        @" AND (SELECT COALESCE(SUM(num_services_remaining),0) 
	            FROM   HealthCardEPCRemaining epcRemaining LEFT JOIN HealthCard AS hc ON epcRemaining.health_card_id = hc.health_card_id
	            WHERE  hc.is_active = 1 AND hc.patient_id = Patient.patient_id AND hc.organisation_id = -1 AND hc.date_referral_signed > DATEADD(year,-1,GETDATE())) > 0 ")
        + (!onlyWithValidDVAEPCRemaining ? "" :
            @" AND (SELECT COUNT(*) FROM HealthCard AS hc WHERE is_active = 1 AND hc.patient_id = Patient.patient_id AND organisation_id = -2 AND date_referral_signed > DATEADD(year,-1,GETDATE())) > 0 ");

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        // valid_medicare_epc_remaining
        // valid_dva_epc_remaining

        if (onlyWithValidMedicareEPCRemaining)  
            tbl = PatientDB.RemooveIfMedicareYearQuotaUsed(tbl);

        string output1 = string.Empty;
        for (int i = 0; i < tbl.Rows.Count; i++)
            output1 += (output1.Length == 0 ? "" : ", ") + Convert.ToInt32(tbl.Rows[i]["patient_id"]);
        output1 = "<u>Has Medicare still valid (" + tbl.Rows.Count + ")</u><br>" + output1;

        Label3.Text = output1;
        */



        // move this into 
        //     Booking.SendReminderEmail()   .. throw custom message exception if they have no email)

        // also, if they dont have an email set when about to make a booking, popup message warning them and allowing them to add one first...

        //Booking booking = BookingDB.GetByID(89219);
        //if (booking != null)
        //    booking.SendReminderEmail(booking);
    }

    protected string GetBookingPatientText(BookingPatient bp)
    {
        //return "<tr><td>" + bp.Booking.Organisation.OrganisationID + "</td><td>" + bp.Booking.BookingID + "</td><td>" + bp.Patient.Person.FullnameWithoutMiddlename + "</td></tr>";
        return "<tr><td>" + bp.Booking.Organisation.OrganisationID + "</td><td>" + bp.Booking.Organisation.Name + "</td><td>" + bp.Booking.BookingID + "</td><td>" + bp.Booking.DateStart.ToString("yyyy-MM-dd HH:mm") + "</td><td>" + bp.Patient.Person.FullnameWithoutMiddlename + "</td></tr>";
    }
    protected string GetBlankRow()
    {
        return "<tr><td colspan=\"100%\" align=\"center\"> &nbsp; </td></tr>";
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Label2.Text = "Done";

        // find 3 bookings and 3 ac patients ... and put in below .. and run in


        int bk1 = 93602;
        int bk2 = 93603;
        int bk3 = 93604;

        int pt1 = 3378;
        int pt2 = 8665;
        int pt3 = 11455;

        /*
        BookingPatientDB.Insert(bk1, pt1, Convert.ToInt32(Session["StaffID"]));
        BookingPatientDB.Insert(bk1, pt2, Convert.ToInt32(Session["StaffID"]));
        BookingPatientDB.Insert(bk1, pt3, Convert.ToInt32(Session["StaffID"]));

        BookingPatientDB.Insert(bk2, pt1, Convert.ToInt32(Session["StaffID"]));
        BookingPatientDB.Insert(bk3, pt1, Convert.ToInt32(Session["StaffID"]));
        */


        BookingPatient[] bp1 = BookingPatientDB.GetByBookingID(bk1);
        BookingPatient[] bp2 = BookingPatientDB.GetByBookingID(bk2);
        BookingPatient[] bp3 = BookingPatientDB.GetByBookingID(bk3);

        BookingPatient[] bp4 = BookingPatientDB.GetByPatientID(pt1);
        BookingPatient[] bp5 = BookingPatientDB.GetByPatientID(pt2);
        BookingPatient[] bp6 = BookingPatientDB.GetByPatientID(pt3);


        string output = "<table border=\"1\">";

        foreach (BookingPatient bp in bp1)
            output += GetBookingPatientText(bp);
        output += GetBlankRow();
        foreach (BookingPatient bp in bp2)
            output += GetBookingPatientText(bp);
        output += GetBlankRow();
        foreach (BookingPatient bp in bp3)
            output += GetBookingPatientText(bp);
        output += GetBlankRow();
        foreach (BookingPatient bp in bp4)
            output += GetBookingPatientText(bp);
        output += GetBlankRow();
        foreach (BookingPatient bp in bp5)
            output += GetBookingPatientText(bp);
        output += GetBlankRow();
        foreach (BookingPatient bp in bp6)
            output += GetBookingPatientText(bp);

        output += "</table>";

        Label2.Text += output;


        /*
        string output = string.Empty;

        Offering[] offerings = OfferingDB.GetAll(null);
        for (int i = 0; i < offerings.Length; i++)
        {
            if (offerings[i].ReminderLetterMonthsLaterToSend == 0 || offerings[i].ReminderLetterID == -1)
                continue;

            Booking[] bookings = BookingDB.GetWhenLastServiceFromXMonthsAgoToGenerageReminderLetter(offerings[i].OfferingID, DateTime.Today, offerings[i].ReminderLetterMonthsLaterToSend);


            // generate letters....

            // just copy ReferrerEPCLetters_GenerateUnsent.aspx now...


            output += "<br /><br /><u>" + offerings[i].OfferingID + ": " + offerings[i].Name + "</u>";
            for (int j = 0; j < bookings.Length; j++)
                output += "<br />" + "[" + bookings[j].Offering.OfferingID + "][" + offerings[i].ReminderLetterID + "] " + bookings[j].ToString();
        }
        */



        /*
        Booking[] bookings = BookingDB.GetFromXMonthsAgo(28, new DateTime(2013, 3, 1), 1);
        //Booking[] bookings = BookingDB.GetFromXMonthsAgo(DateTime.Today.AddDays(-2), 1);

        string output = string.Empty;
        for (int i = 0; i < bookings.Length; i++)
            output += "<br />" + "[" + bookings[i].Offering.OfferingID + "] " + bookings[i].ToString();
        */

        //Label2.Text = output.Length == 0 ? "No Bookings" : output;
    }

    protected void Button3_Click(object sender, EventArgs e)
    {

        int bkpt1 = 8;
        int bkpt2 = 7;

        int o1 = 92;  // betadine
        int o2 = 160; // dressing
        int o3 = 185; // mediclinic lotion

        /*
        BookingPatientOfferingDB.Insert(bkpt1, o1, 1, Convert.ToInt32(Session["StaffID"]));
        BookingPatientOfferingDB.Insert(bkpt1, o2, 1, Convert.ToInt32(Session["StaffID"]));
        BookingPatientOfferingDB.Insert(bkpt1, o3, 1, Convert.ToInt32(Session["StaffID"]));

        BookingPatientOfferingDB.Insert(bkpt2, o1, 1, Convert.ToInt32(Session["StaffID"]));
        BookingPatientOfferingDB.Insert(bkpt2, o2, 1, Convert.ToInt32(Session["StaffID"]));
        */


        BookingPatientOffering[] bpo1 = BookingPatientOfferingDB.GetByBookingPatientID(bkpt1);
        BookingPatientOffering[] bpo2 = BookingPatientOfferingDB.GetByBookingPatientID(bkpt2);


        string output = "<table border=\"1\">";

        foreach (BookingPatientOffering bpo in bpo1)
            output += "<tr><td>" + bpo.BookingPatientOfferingID + "</td><td>" + bpo.Offering.Name + "</td><td>" + bpo.Offering.Field.Descr + "</td><td>" + bpo.Quantity + "</td><td>";
        output += GetBlankRow();
        foreach (BookingPatientOffering bpo in bpo2)
            output += "<tr><td>" + bpo.BookingPatientOfferingID + "</td><td>" + bpo.Offering.Name + "</td><td>" + bpo.Offering.Field.Descr + "</td><td>" + bpo.Quantity + "</td><td>";

        output += "</table>";

        Label3.Text += output;




        /*
        string newName = "TestyBB";
        string mdfFile = "Mediclinic_Balwyn";
        string logFile = "Mediclinic_Balwyn_log";

        string sqlB = @"
            Alter Database TestyBB SET SINGLE_USER With ROLLBACK IMMEDIATE;
            Drop database " +newName + @";

            RESTORE DATABASE " + newName + @"
            FROM DISK = 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\Mediclinic_Balwyn201309181901.bak' 
            WITH MOVE '" + mdfFile + @"' TO 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\" + newName + @".mdf',
                 MOVE '" + logFile + @"' TO 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\" + newName + @"_log.ldf',
                 FILE = 1,  
                 NOUNLOAD,  
                 REPLACE,  
                 STATS = 10;
        ";  
        ExecuteNonResult(sqlB);

        string sql = "EXEC sp_databases;";
        DataTable tbl = ExecuteQuery(sql).Tables[0];
        Label3.Text = "";
        for(int i=0; i<tbl.Rows.Count; i++)
            Label3.Text += tbl.Rows[i][0].ToString() + "<br />"; 
        */
    }
    
    public static DataSet ExecuteQuery(string sql)
    {
        try
        {
            //string connString = "Data Source=.\\SQLExpress; Integrated Security=true; User Instance=true; Initial Catalog=master;";
            string connString = "Server=.\\SQLEXPRESS;Database=master;Integrated Security=SSPI;";


            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = connString; //DBBase.GetConnectionString();
            con.Open();

            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sql, con);
            DataSet ds1 = new DataSet();
            da.Fill(ds1);

            da.Dispose();
            con.Close();
            con.Dispose();

            return ds1;
        }
        catch (Exception ex)
        {
            Logger.LogQuery("SQL Exception:" + Environment.NewLine + Environment.NewLine + sql + Environment.NewLine + Environment.NewLine + ex.ToString(), true, true);
            throw;
        }
    }
    public static void ExecuteNonResult(string sql)
    {
        try
        {
            //string connString = "Data Source=.\\SQLExpress; Integrated Security=true; User Instance=true; Initial Catalog=master;";
            string connString = "Server=.\\SQLEXPRESS;Database=master;Integrated Security=SSPI;";


            System.Data.SqlClient.SqlConnection con =
                new System.Data.SqlClient.SqlConnection(connString);

            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            cmd.Connection = con;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            con.Dispose();
        }
        catch (Exception ex)
        {
            Logger.LogQuery("SQL Exception:" + Environment.NewLine + Environment.NewLine + sql + Environment.NewLine + Environment.NewLine + ex.ToString(), true, true);
            throw;
        }
    }



    protected void Button4_Click(object sender, EventArgs e)
    {


        // need to be able to have them put in their own field instead of physio/podiatry .... 


/*
select * from UserDatabaseMapper order by id desc
delete UserDatabaseMapper where dbname in ('Mediclinic_0002','Mediclinic_0003','Mediclinic_0004','Mediclinic_0005')
*/

        // also need to not import field type phys/pod .... 


        try
        {
            decimal decNum;
            int intNum;

            if (txtInitialStaffFirstname.Text.Trim().Length == 0)
                throw new CustomMessageException("Staff Firstname is a required field.");
            if (txtInitialStaffSurname.Text.Trim().Length == 0)
                throw new CustomMessageException("Staff Surname is a required field.");
            if (txtInitialStaffLogin.Text.Trim().Length == 0)
                throw new CustomMessageException("Staff Login is a required field.");
            if (txtMedicareEclaimsLicenseNbr.Text.Trim().Length == 0)
                throw new CustomMessageException("Medicare Eclaims License Nbr is a required field.");
            if (!Decimal.TryParse(txtSMSPrice.Text.Trim(), out decNum))
                throw new CustomMessageException("SMS Price must be a decimal.");
            if (!Int32.TryParse(txtMaxNbrProviders.Text.Trim(), out intNum))
                throw new CustomMessageException("Max Nbr Providers must be a number.");
            if (txtSiteName.Text.Trim().Length == 0)
                throw new CustomMessageException("Clinic Site Name is a required field.");
            if (txtEmail_FromName.Text.Trim().Length == 0)
                throw new CustomMessageException("Email Sending From Name is a required field.");
            if (!Utilities.IsValidEmailAddress(txtEmail_FromEmail.Text.Trim()))
                throw new CustomMessageException("Email Sending From Email must be a valid email address.");
            if (!Utilities.IsValidEmailAddress(txtAdminAlertEmail_To.Text.Trim()))
                throw new CustomMessageException("Admin Alert Email must be a valid email address.");
            if (txtField1.Text.Trim().Length == 0)
                throw new CustomMessageException("Field 1 is a required field.");
        }
        catch(CustomMessageException ex)
        {
            Label4.Text = "<font color=\"red\"><br>" + ex.Message + "</b></font>";
            return; 
        }



        string initialStaffFirstname = txtInitialStaffFirstname.Text.Trim();
        string initialStaffSurname   = txtInitialStaffSurname.Text.Trim();
        string initialStaffLogin     = GetUniqueLogin(txtInitialStaffLogin.Text.Trim().ToLower());


        //
        // clone the DB with new name
        //

        string newDBName = GetNewDBName();
        CloneDB("Mediclinic_StubDB", "Mediclinic_StubDB_log", newDBName);


        //
        // update any config items
        //

        SystemVariableDB.Update("MedicareEclaimsLicenseNbr" , txtMedicareEclaimsLicenseNbr.Text.Trim() , newDBName);
        SystemVariableDB.Update("SMSPrice"                  , txtSMSPrice.Text.Trim()                  , newDBName);
        SystemVariableDB.Update("MaxNbrProviders"           , txtMaxNbrProviders.Text.Trim()           , newDBName);
        SystemVariableDB.Update("AllowAddSiteClinic"        , ddlAllowAddSiteClinic.SelectedValue      , newDBName);
        SystemVariableDB.Update("AllowAddSiteAgedCare"      , ddlAllowAddSiteAgedCare.SelectedValue    , newDBName);
        SystemVariableDB.Update("BannerMessage"             , txtBannerMessage.Text.Trim()             , newDBName);
        SystemVariableDB.Update("ShowBannerMessage"         , ddlShowBannerMessage.SelectedValue       , newDBName);
        SystemVariableDB.Update("Email_FromName"            , txtEmail_FromName.Text                   , newDBName);
        SystemVariableDB.Update("Email_FromEmail"           , txtEmail_FromEmail.Text.Trim()           , newDBName);
        SystemVariableDB.Update("AdminAlertEmail_To"        , txtAdminAlertEmail_To.Text.Trim()        , newDBName);





        //
        // update login/pwd for first staff memeber and for support staff
        //

        string sql_update_staff = @"

            UPDATE Person
            SET 
                Person.firstname = '" + initialStaffFirstname + @"'  
               ,Person.surname   = '" + initialStaffSurname + @"'  
            FROM Person
            JOIN Staff ON Person.person_id = Staff.person_id
            WHERE Staff.staff_id = 1;


            UPDATE Staff SET login = '" + initialStaffLogin + @"'         WHERE staff_id = 1;
            UPDATE Staff SET pwd   = '" + initialStaffLogin + @"'         WHERE staff_id = 1;

            UPDATE Staff SET login = '" + newDBName + "_support1" + @"'   WHERE staff_id = -2;
            UPDATE Staff SET login = '" + newDBName + "_support2" + @"'   WHERE staff_id = -3;
            UPDATE Staff SET login = '" + newDBName + "_support3" + @"'   WHERE staff_id = -4;
        ";

        if (txtField1.Text.Trim().Length > 0)
            sql_update_staff += @"INSERT Field (descr, has_offerings) VALUES ('"+txtField1.Text.Trim()+@"' , 1);" + Environment.NewLine;
        if (txtField2.Text.Trim().Length > 0)
            sql_update_staff += @"INSERT Field (descr, has_offerings) VALUES ('"+txtField2.Text.Trim()+@"' , 1);" + Environment.NewLine;

        sql_update_staff += @"UPDATE Site SET name = '" + txtSiteName.Text.Trim() + @"' WHERE site_type_id = 1;" + Environment.NewLine;


        DBBase.ExecuteNonResult(sql_update_staff, newDBName);

        UserDatabaseMapperDB.Insert(initialStaffLogin,       newDBName);
        UserDatabaseMapperDB.Insert(newDBName + "_support1", newDBName);
        UserDatabaseMapperDB.Insert(newDBName + "_support2", newDBName);
        UserDatabaseMapperDB.Insert(newDBName + "_support3", newDBName);


        Label4.Text =
@"Database created successfully.<br />
<table border=""0"" cellpadding=""0"" cellspacing=""0"">
<tr>
    <td>Database : </td>
    <td style=""width:20px;""></td>
    <td><b>" + newDBName + @"</b></td>
</tr>
<tr>
    <td>Staff Username & Password : </td>
    <td></td>
    <td><b>" + initialStaffLogin + @"</b></td>
</tr>
</table>";

    }

    protected static void CloneDB(string mdFileToClone, string logFileToClone, string newName)
    {
        //string stubDBLocation = @"C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\Mediclinic_Balwyn201309181901.bak";
        string stubDBLocation = @"C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\Mediclinic_StubDB.bak";

        string sql = @"
            --Alter Database " + newName + @" SET SINGLE_USER With ROLLBACK IMMEDIATE;
            --Drop database " + newName + @";

            RESTORE DATABASE " + newName + @" 
            FROM DISK = '" + stubDBLocation + @"' 
            WITH MOVE '" + mdFileToClone + @"' TO 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\" + newName + @".mdf',
                 MOVE '" + logFileToClone + @"' TO 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\" + newName + @"_log.ldf',
                 FILE = 1,  
                 NOUNLOAD,  
                 REPLACE,  
                 STATS = 10;
        ";

        DBBase.ExecuteNonResult(sql, "master");
    }

    protected static string GetNewDBName()
    {
        ArrayList existingDBs = new ArrayList();

        string sql = "EXEC sp_databases;";
        DataTable tbl = ExecuteQuery(sql).Tables[0];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            string s = tbl.Rows[i][0].ToString();
            bool b = System.Text.RegularExpressions.Regex.IsMatch(tbl.Rows[i][0].ToString(), @"^Mediclinic_\d+$");

            if (System.Text.RegularExpressions.Regex.IsMatch(tbl.Rows[i][0].ToString(), @"^Mediclinic_\d+$"))
                existingDBs.Add(Convert.ToInt32(tbl.Rows[i][0].ToString().Substring("Mediclinic_".Length)));
        }
        existingDBs.Sort();

        return "Mediclinic_" + (existingDBs.Count == 0 ? "0001" : ((int)existingDBs[existingDBs.Count-1] + 1).ToString().PadLeft(4, '0'));
    }

    protected static string GetUniqueLogin(string initialLogin)
    {
        UserDatabaseMapper[] allUsers = UserDatabaseMapperDB.GetAll();

        if (!UserDatabaseMapperDB.UsernameExists(allUsers, initialLogin))
            return initialLogin;

        else
            for (int i = 1; ; i++)
                if (!UserDatabaseMapperDB.UsernameExists(allUsers, initialLogin + i.ToString()))
                    return initialLogin + i.ToString();
    }


}


