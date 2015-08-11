using System;
using System.Collections;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using System.Linq;

public class OverdueInvoiceReminders
{
    protected static bool IsDebug = false;

    public static string Run(bool incDisplay, bool incSending)
    {
        bool EnableAutoMontlyOverdueReminders = Convert.ToInt32(((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["EnableAutoMontlyOverdueReminders"].Value) == 1;
        if (!EnableAutoMontlyOverdueReminders)
            incSending = false;

        string output = string.Empty;
        foreach (Site site in SiteDB.GetAll())
            output += (output.Length == 0 ? string.Empty : "<br /><br /><br />") + Run(incDisplay, incSending, site);
        return output;
    }

    public static string Run(bool incDisplay, bool incSending, Site site)
    {
        int daysOverdueAtleast = 7;

        string output = string.Empty;

        DataTable dtPTs = InvoiceDB.GetAllOutstandingByPatientAsReport(site.SiteID, daysOverdueAtleast);
        dtPTs.DefaultView.Sort = "patient_surname";
        dtPTs = dtPTs.DefaultView.ToTable();


        output += "<br /><br /><font color=\"blue\"><u>" + site.Name + " - <b>PTs</b></u></font><br />";
        output += "<table border=\"1\">";
        for (int i = 0; i < (dtPTs.Rows.Count); i++)
        {
            Patient patient = PatientDB.GetByID(Convert.ToInt32(dtPTs.Rows[i]["patient_id"]));
            string emails = string.Empty;

            try
            {
                emails = SendEmailToPT(incSending, dtPTs, patient, site);
            }
            catch (Exception ex)
            {
                output += Environment.NewLine + "    <tr><td>" + patient.Person.FullnameWithoutMiddlename + "</td><td>" + patient.PatientID + "</td><td>" + ex.ToString().Replace(Environment.NewLine, "<br />") + "</td></tr>";
            }

            output += Environment.NewLine + "    <tr><td>" + patient.Person.FullnameWithoutMiddlename + "</td><td>" + patient.PatientID + "</td><td>" + emails + "</td></tr>";
        }
        output += "</table>";


        bool insuranceCompanies = site.SiteType.ID != 2;  // in clinics system, generate for insurance companies, in aged care, run for aged care facs
        DataTable dtOrgs = InvoiceDB.GetAllOutstandingByOrgAsReport(site.SiteID, daysOverdueAtleast, insuranceCompanies);
        dtOrgs.DefaultView.Sort = "name";
        dtOrgs = dtOrgs.DefaultView.ToTable();

        output += "<br /><br /><font color=\"blue\"><u>" + site.Name + " - <b>Orgs</b></u></font><br />";
        output += "<table border=\"1\">";
        for (int i = 0; i < (dtOrgs.Rows.Count); i++)
        {
            Organisation org = OrganisationDB.GetByID(Convert.ToInt32(dtOrgs.Rows[i]["organisation_id"]));
            string emails = string.Empty;
            
            try
            {
                emails = SendEmailToOrgs(incSending, dtOrgs, org, site);
            }
            catch (Exception ex)
            {
                output += Environment.NewLine + "    <tr><td>" + org.Name + "</td><td>" + org.OrganisationID + "</td><td>" + ex.ToString().Replace(Environment.NewLine, "<br />") + "</td></tr>";
            }

            output += Environment.NewLine + "    <tr><td>" + org.Name + "</td><td>" + org.OrganisationID + "</td><td>" + emails + "</td></tr>";
        }
        output += "</table>";


        return output;
    }

    protected static string SendEmailToPT(bool incSending, DataTable dt, Patient patient, Site site)
    {
        DataRow row = dt.Select("patient_id = " + patient.PatientID)[0];
        string daysOverdueCommaSep = (string)row["days_overdue_comma_sep"];

        string[] emails = ContactDB.GetEmailsByEntityID(patient.Person.EntityID);
        if (emails.Length == 0)
            return string.Empty + "[" + daysOverdueCommaSep + "]";
        

        if (incSending)
        {
            string invoiceIDsCommaSep  = (string)row["invoice_ids_comma_sep"];
            int[] invoiceIDs = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);

            bool   isClinics = site.SiteType.ID != 2;
            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            string originalFile = Letter.GetLettersDirectory() + (isClinics ? @"OverdueInvoiceTemplate.docx" : @"OverdueInvoiceTemplateAC.docx");
            string tmpDir = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
            System.IO.Directory.CreateDirectory(tmpDir);
            string outputFile = tmpDir + "OverdueInvoices.pdf";


            try
            {
                Letter.GenerateOutstandingInvoices(originalFile, outputFile, invoiceIDs, patient.PatientID, -1);

                EmailerNew.SimpleEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    IsDebug ? "eli.pollak@mediclinic.com.au" : string.Join(",", emails),
                    "Overdue Invoices",
                    "Pease find your Invoices attached. <br />Please call us if you do not agree with the Invoice amount stated.<br /><br />Thank you.",
                    true,
                    new string[] { outputFile },
                    false,
                    null,
                    "eli.pollak@mediclinic.com.au"
                    );
            }
            finally
            {
                try { if (System.IO.File.Exists(outputFile)) System.IO.File.Delete(outputFile); }
                catch (Exception) { }

                // delete temp dir
                if (tmpDir != null)
                {

                    try { System.IO.Directory.Delete(tmpDir, true); }
                    catch (Exception) { }
                }
            }
        }

        return string.Join(", ", emails) + "[" + daysOverdueCommaSep + "]";
    }

    protected static string SendEmailToOrgs(bool incSending, DataTable dt, Organisation org, Site site)
    {
        DataRow row = dt.Select("organisation_id = " + org.OrganisationID)[0]; 
        string daysOverdueCommaSep = (string)row["days_overdue_comma_sep"];

        string[] emails = ContactDB.GetEmailsByEntityID(org.EntityID);
        if (emails.Length == 0)
            return string.Empty + "[" + daysOverdueCommaSep + "]";

        if (incSending)
        {
            string invoiceIDsCommaSep  = (string)row["invoice_ids_comma_sep"];
            int[] invoiceIDs = Array.ConvertAll<string, int>(invoiceIDsCommaSep.Split(','), Convert.ToInt32);


            bool   isClinics           = site.SiteType.ID != 2;
            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            string originalFile        = Letter.GetLettersDirectory() + (isClinics ? @"OverdueInvoiceTemplate.docx" : @"OverdueInvoiceTemplateAC.docx");
            string tmpDir              = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
            System.IO.Directory.CreateDirectory(tmpDir);
            string outputFile          = tmpDir + "OverdueInvoices.pdf";


            try
            {
                Letter.GenerateOutstandingInvoices(originalFile, outputFile, invoiceIDs, -1, org.OrganisationID);

                EmailerNew.SimpleEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    IsDebug ? "eli.pollak@mediclinic.com.au" : string.Join(",", emails),
                    "Overdue Invoices",
                    "Pease find your Invoices attached. <br />Please call us if you do not agree with the Invoice amount stated.<br /><br />Thank you.",
                    true,
                    new string[] { outputFile },
                    false,
                    null,
                    "eli.pollak@mediclinic.com.au"
                    );

            }
            finally
            {
                try { if (System.IO.File.Exists(outputFile)) System.IO.File.Delete(outputFile); }
                catch (Exception) { }

                // delete temp dir
                if (tmpDir != null)
                {

                    try { System.IO.Directory.Delete(tmpDir, true); }
                    catch (Exception) { }
                }
            }
        }

        return string.Join(", ", emails) + "[" + daysOverdueCommaSep + "]";
    }

}