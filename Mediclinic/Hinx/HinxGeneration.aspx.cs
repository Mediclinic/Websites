using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections;
using System.Configuration;
using System.Data;


public partial class HinxGen : System.Web.UI.Page
{

    // http://portal.mediclinic.com.au:803/Hinx/HinxGeneration.aspx?pwd=mah_sms_reminder
    // http://portal.mediclinic.com.au:803/Hinx/HinxGeneration.aspx?pwd=mah_sms_reminder&inc_sending=false


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {

            string pwd = Request.Form["pwd"];

            if (pwd == null)
                pwd = Request.QueryString["pwd"];

            if (pwd != null)  // sent by http post
            {
                if (pwd == null || pwd != System.Configuration.ConfigurationManager.AppSettings["SMSRunRemindersPwd"])
                    throw new CustomMessageException("Incorrect password");

                string exceptionOutput = string.Empty; 

                if (Session != null && Session["DB"] != null)
                {
                    throw new CustomMessageException("Can not run this while logged in.");
                }
                else
                {
                    System.Data.DataTable tbl = DBBase.ExecuteQuery("EXEC sp_databases;", "master").Tables[0];
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        string databaseName = tbl.Rows[i][0].ToString();

                        if (!Regex.IsMatch(databaseName, @"Mediclinic_\d{4}"))
                            continue;
                        if (databaseName == "Mediclinic_0026")
                            continue;

                        try
                        {
                            Session["DB"] = databaseName;
                            Session["SystemVariables"] = SystemVariableDB.GetAll();

                            if (Convert.ToInt32(SystemVariableDB.GetByDescr("AutoMedicareClaiming").Value) == 1)
                            {
                                GenerateMedicareHinxFiles();
                                GenerateDVAHinxFiles();
                            }
                        }
                        catch (Exception ex)
                        {
                            exceptionOutput += Environment.NewLine + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " " + "DB: " + databaseName;
                            exceptionOutput += Environment.NewLine + ex.ToString();
                        }
                        finally
                        {
                            Session.Remove("DB");
                            Session.Remove("SystemVariables");
                        }
                    }

                }


                if (exceptionOutput.Length > 0)
                {
                    Response.Write("Run Completed But With Errors!");
                    Response.Write(Environment.NewLine + exceptionOutput);
                }
                else
                {
                    Response.Write("Run Completed!");
                }
            }

        }
        catch (CustomMessageException ex)
        {
            Response.Write(ex.Message);
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + ex.ToString());
        }
    }



    protected void GenerateMedicareHinxFiles()
    {
        Site[] allSites = SiteDB.GetAll();
        foreach (Site site in allSites)
        {
            GenerateClaimNumbers(-1, site.SiteID);
            GenerateHinxFiles(HinxFile.ClaimType.Medicare, GetFromDate(), GetToDate(), site.SiteID);
        }
    }
    protected void GenerateDVAHinxFiles()
    {
        Site[] allSites = SiteDB.GetAll();
        foreach (Site site in allSites)
        {
            GenerateClaimNumbers(-2, site.SiteID);
            GenerateHinxFiles(HinxFile.ClaimType.DVA, GetFromDate(), GetToDate(), site.SiteID);
        }
    }

    protected DateTime GetFromDate()
    {
        // if before 1/10, include until LAST financial year start (to give 3 months leeway to geenrate
        // otherwise from start of this financial year
        return new DateTime(DateTime.Now.AddYears(DateTime.Now.Month < 10 ? -1 : 0).Year, 7, 1);
    }
    protected DateTime GetToDate()
    {
        return DateTime.MinValue;
    }

    protected void GenerateClaimNumbers(int organisation_id, int site_id)
    {
        Hashtable sitesHash = SiteDB.GetAllInHashtable();
        Hashtable claimNumberInvoiceGroups = new Hashtable();


        DataTable dt = InvoiceDB.GetMedicareInvoicesWithoutClaimNumbers(true, organisation_id, site_id, GetFromDate(), GetToDate());
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            Invoice invoice = InvoiceDB.LoadAll(dt.Rows[i]);

            int      providerID = invoice.Booking.Provider.StaffID;
            int      orgID      = ((Site)sitesHash[invoice.Site.SiteID]).SiteType.ID == 1 ? invoice.Booking.Organisation.OrganisationID : 0;
            DateTime date       = invoice.Booking.DateStart.Date;

            Hashtable3D.KeyWithDate hashKey = Hashtable3D.KeyWithDate.New(providerID, orgID, date);

            if (claimNumberInvoiceGroups[hashKey] == null)
                claimNumberInvoiceGroups[hashKey] = new ArrayList();

            ((ArrayList)claimNumberInvoiceGroups[hashKey]).Add(invoice);
        }
        foreach (Hashtable3D.KeyWithDate hashKey in claimNumberInvoiceGroups.Keys)
        {
            string claimNumber = string.Empty;

            ArrayList invoiceList = (ArrayList)claimNumberInvoiceGroups[hashKey];
            for (int i = 0; i < invoiceList.Count; i++)
            {
                if (claimNumber == string.Empty)
                    claimNumber = MedicareClaimNbrDB.InsertIntoInvoice(((Invoice)invoiceList[i]).InvoiceID, DateTime.Now.Date);
                else
                    InvoiceDB.SetClaimNumber(((Invoice)invoiceList[i]).InvoiceID, claimNumber);
            }
        }
    }


    protected void GenerateHinxFiles(HinxFile.ClaimType claimType, DateTime fromDate, DateTime toDate, int site_id)
    {
        try
        {
            int startTimeGenerateReport = Environment.TickCount;
            HinxFile hinxFile = new HinxFile(claimType, site_id);
            HinxFile.ReportItems reportItems = hinxFile.GenerateReportItems(fromDate, toDate, false);
            double executionTimeGenerateReport = (double)(Environment.TickCount - startTimeGenerateReport) / 1000.0;

            // Commented out since they want to allow overwrite of old files rather than error message saying remove them to re-generate
            //
            //string[] existingHinxFiles = hinxFile.GetExistingHinxFiles(reportItems.reportTable);
            //if (existingHinxFiles.Length > 0)
            //{
            //    int itemsPerLine = 8;
            //    string existing = string.Empty;
            //    for (int i = 0; i < existingHinxFiles.Length; i++)
            //        existing += (i != 0 && i % itemsPerLine == 0 ? "<br />" : "") + existingHinxFiles[i] + (i != existingHinxFiles.Length - 1 ? ", " : "");
            //    HideTableAndSetErrorMessage("Can not generate while the following eclaim files already exist : " + "<br />" + string.Join("<br />", existing));
            //    return;
            //}

            string failedItemsMessage = string.Empty;
            try 
            { 
                hinxFile.CreateXML(reportItems.ToBeClaimed); 
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Emailer.SimpleErrorEmail("HINX AUTO Generation - Failed to generate - connection to network files currently unavailable.");
                return;
            }
            catch (HINXUnsuccessfulItemsException ex)
            { 
                failedItemsMessage = ex.Message; 
            }
        }
        catch (CustomMessageException cmEx)
        {
            Emailer.SimpleErrorEmail("HINX AUTO Generation - " + cmEx.Message);
        }
        catch (Exception ex)
        {
            Emailer.SimpleErrorEmail("HINX AUTO Generation - " + ex.ToString());
        }
    }



}
