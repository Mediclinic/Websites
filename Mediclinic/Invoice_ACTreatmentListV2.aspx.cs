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
using System.IO;

public partial class Invoice_ACTreatmentListV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, false);
                Session.Remove("invoicelistac_sortexpression");
                Session.Remove("invoicelistac_data");

                if (!IsValidFormBooking())
                    throw new CustomMessageException("No booking in url");

                Booking booking = BookingDB.GetByID(GetFormBooking());


                string orgAddressText;
                if (Utilities.GetAddressType().ToString() == "Contact")
                {
                    Contact orgAddress       = ContactDB.GetFirstByEntityID(1,  booking.Organisation != null ? booking.Organisation.EntityID : -1);
                    orgAddressText           = orgAddress == null    ? "No address found" : orgAddress.GetFormattedAddress("No address found");
                }
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                {
                    ContactAus orgAddressAus = ContactAusDB.GetFirstByEntityID(1,  booking.Organisation != null ? booking.Organisation.EntityID : -1);
                    orgAddressText           = orgAddressAus == null ? "No address found" : orgAddressAus.GetFormattedAddress("No address found");
                }
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());


                lblTreatmentDate.Text = booking.DateStart.ToString("d MMM, yyyy") + "&nbsp;&nbsp;" + booking.DateStart.ToString("H:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm");
                lblOrgType.Text       = booking.Organisation.OrganisationType.Descr;
                lblOrgName.Text       = booking.Organisation.Name;
                lblOrgAddress.Text    = orgAddressText.Replace(Environment.NewLine, "<br />");
                lblProviderName.Text  = booking.Provider.Person.FullnameWithoutMiddlename;
                lblProviderNbr.Text   = booking.Provider.ProviderNumber;

                string link = @"http://localhost:2524/Invoice_ViewV2.aspx?booking_id=264225";
                lblInvLink.Text = "<a href=\"" + link + "\" onclick=\"open_new_tab('" + link + "');return false;\">View All Invoices</a>";

            
                if (!booking.Organisation.IsAgedCare)
                {
                    tr_address.Visible             = false;
                    btnEmailToFac.Visible          = false;
                    br_before_email_to_fac.Visible = false;

                    RegisterStaff regStaff = RegisterStaffDB.GetByStaffIDAndOrganisationID(booking.Provider.StaffID, booking.Organisation.OrganisationID, true);
                    if (regStaff != null)
                        lblProviderNbr.Text = regStaff.ProviderNumber;
                }


                FillGrid();
            }

            this.GrdBooking.EnableViewState = true;

        }
        catch (CustomMessageException ex)
        {
            if (IsPostBack) SetErrorMessage(ex.Message);
            else HideTableAndSetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            if (IsPostBack) SetErrorMessage("", ex.ToString());
            else HideTableAndSetErrorMessage("", ex.ToString());
        }
    }

    #endregion

    #region GetFormParam

    protected bool IsValidFormBooking()
    {
        string booking = Request.QueryString["booking"];
        return booking != null && Regex.IsMatch(booking, @"^\d+$") && BookingDB.Exists(Convert.ToInt32(booking));
    }
    protected int GetFormBooking(bool checkIsValid = true)
    {
        if (checkIsValid && !IsValidFormBooking())
            throw new Exception("Invalid url patient");
        return Convert.ToInt32(Request.QueryString["booking"]);
    }

    #endregion

    #region GrdBooking

    protected void FillGrid()
    {
        Booking booking = BookingDB.GetByID(GetFormBooking());

        DataTable dt = Letter.GenerateInvoiceLines_ByBookingID(GetFormBooking(chkIncAddOns.Checked), booking.Organisation.IsAgedCare);

        // add to sort on room correctly
        dt.Columns.Add("PaddedRoom", typeof(String));
        for (int i = 0; i < dt.Rows.Count; i++)
            dt.Rows[i]["PaddedRoom"] = dt.Rows[i]["Room"] == DBNull.Value ? DBNull.Value : (object)PadRoomNbr(dt.Rows[i]["Room"].ToString().Trim(), true);

        Session["invoicelistac_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["invoicelistac_sortexpression"] != null && Session["invoicelistac_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["invoicelistac_sortexpression"].ToString();
                GrdBooking.DataSource = dataView;
            }
            else
            {
                GrdBooking.DataSource = dt;
            }


            try
            {
                GrdBooking.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdBooking.DataSource = dt;
            GrdBooking.DataBind();

            int TotalColumns = GrdBooking.Rows[0].Cells.Count;
            GrdBooking.Rows[0].Cells.Clear();
            GrdBooking.Rows[0].Cells.Add(new TableCell());
            GrdBooking.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdBooking.Rows[0].Cells[0].Text = "No Record Found";
        }

        if (!booking.Organisation.IsAgedCare)
            GrdBooking.Columns[1].Visible = false;

    }
    protected void GrdBooking_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {

        }
    }
    protected void GrdBooking_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["invoicelistac_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            /*
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("booking_booking_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];
            Booking booking = BookingDB.LoadFull(thisRow);
            */

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[2].Text = "Patient";
            e.Row.Cells[3].Text = "Service";
        }
    }
    protected void GrdBooking_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
    }
    protected void GrdBooking_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
    }
    protected void GrdBooking_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdBooking_RowCommand(object sender, GridViewCommandEventArgs e)
    {
    }
    protected void GrdBooking_RowEditing(object sender, GridViewEditEventArgs e)
    {
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdBooking.EditIndex >= 0)
            return;

        DataTable dataTable = Session["invoicelistac_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["invoicelistac_sortexpression"] == null)
                Session["invoicelistac_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["invoicelistac_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["invoicelistac_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdBooking.DataSource = dataView;
            GrdBooking.DataBind();
        }
    }

    protected string PadRoomNbr(string room, bool padZeros = false)
    {
        if (!padZeros)
            return room;

        room = room.Trim();

        char[] roomArray = room.ToCharArray();


        string numberPart = string.Empty;
        string rest = string.Empty;

        int i = 0;
        for (; i < roomArray.Length; i++)
        {
            if (!Char.IsDigit(roomArray[i]))
                break;

            numberPart += roomArray[i];
        }

        if (i < roomArray.Length)
            rest = room.Substring(i);

        if (numberPart.Length > 0)
            numberPart = numberPart.PadLeft(3, '0');

        return numberPart + rest;
    }

    #endregion

    #region btnExport_Click, btnEmailToFac_Click

    protected void btnExport_Click(object sender, EventArgs e)
    {
        Booking booking    = BookingDB.GetByID(GetFormBooking());
        bool    isAgedCare = booking.Organisation.IsAgedCare;


        string tmpLettersDirectory = FileHelper.GetTempDirectoryName(Letter.GetTempLettersDirectory());
        Directory.CreateDirectory(tmpLettersDirectory);

        string templateFileName = isAgedCare ? "ACTreatmentList.docx" : "TreatmentList.docx";
        string originalFile = Letter.GetLettersDirectory() + templateFileName;
        string tmpOutputFile = tmpLettersDirectory + "TreatmentList.pdf"; // System.IO.Path.GetExtension(originalFile));

        if (!File.Exists(originalFile))
        {
            SetErrorMessage("Template File '" + templateFileName + "' does not exist.");
            return;
        }

        MergeFile(isAgedCare, originalFile, tmpOutputFile);

        Letter.FileContents fileContents = new Letter.FileContents(System.IO.File.ReadAllBytes(tmpOutputFile), "TreatmentList.pdf");
        File.Delete(tmpOutputFile);
        System.IO.File.SetAttributes(tmpLettersDirectory, FileAttributes.Normal);
        System.IO.Directory.Delete(tmpLettersDirectory, true);

        // Nothing gets past the "DownloadDocument" method because it outputs the file 
        // which is writing a response to the client browser and calls Response.End()
        // So make sure any other code that functions goes before this
        Letter.DownloadDocument(Response, fileContents.Contents, fileContents.DocName);
    }

    protected void btnEmailToFac_Click(object sender, EventArgs e)
    {
        Booking booking = BookingDB.GetByID(GetFormBooking());

        string[] emails = ContactDB.GetEmailsByEntityID(booking.Organisation.EntityID);

        if (emails.Length == 0)
        {
            SetErrorMessage("No email address set for '" + booking.Organisation.Name + "'. Please set one to email treatment list to them.");
            return;
        }
        else
        {
            string tmpLettersDirectory = FileHelper.GetTempDirectoryName(Letter.GetTempLettersDirectory());
            Directory.CreateDirectory(tmpLettersDirectory);

            string originalFile = Letter.GetLettersDirectory() + @"ACTreatmentList.docx";
            string tmpOutputFile = tmpLettersDirectory + "TreatmentList.pdf";

            if (!File.Exists(originalFile))
            {
                SetErrorMessage("Template File 'ACTreatmentList.docx' does not exist.");
                return;
            }

            MergeFile(true, originalFile, tmpOutputFile);

            try
            {
                EmailerNew.SimpleEmail(
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value,
                    ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    (Utilities.IsDev() ? "eli.pollak@mediclinic.com.au" : string.Join(",", emails)),
                    "Treatement List",
                    "Pease find the <u>Residents Treated List</u> for " + booking.DateStart.ToString("d MMM, yyyy") + " at " + booking.Organisation.Name + "<br /><br />Regards,<br />" + ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromName"].Value,
                    true,
                    new string[] { tmpOutputFile },
                    false,
                    null
                    );

                SetErrorMessage("Emailed to &nbsp;&nbsp;" + booking.Organisation.Name + " (" + string.Join(", ", emails) + ")");
            }
            catch (CustomMessageException cmEx)
            {
                SetErrorMessage(cmEx.Message);
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }
            finally
            {
                try { if (System.IO.File.Exists(tmpOutputFile)) System.IO.File.Delete(tmpOutputFile); }
                catch (Exception) { }

                // delete temp dir
                if (tmpLettersDirectory != null)
                {
                    try { System.IO.Directory.Delete(tmpLettersDirectory, true); }
                    catch (Exception) { }
                }
            }
        }

    }

    protected void MergeFile(bool isAgedCare, string originalFile, string outputFile)
    {
        Booking   booking = BookingDB.GetByID(GetFormBooking());
        DataTable dt      = Session["invoicelistac_data"] as DataTable;


        string orgAddressText , orgAddressTabbedText , orgPhoneText , orgFaxText, orgWebText, orgEmailText;
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact orgAddress = ContactDB.GetFirstByEntityID(1,    booking.Organisation != null ? booking.Organisation.EntityID : -1);
            Contact orgPhone   = ContactDB.GetFirstByEntityID(null, booking.Organisation != null ? booking.Organisation.EntityID : -1, "30,33,34");
            Contact orgFax     = ContactDB.GetFirstByEntityID(-1,   booking.Organisation != null ? booking.Organisation.EntityID : -1, 29);
            Contact orgWeb     = ContactDB.GetFirstByEntityID(-1,   booking.Organisation != null ? booking.Organisation.EntityID : -1, 28);
            Contact orgEmail   = ContactDB.GetFirstByEntityID(-1,   booking.Organisation != null ? booking.Organisation.EntityID : -1, 27);

            orgAddressText            = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found");
            orgAddressTabbedText      = orgAddress     == null    ? "No address found"      : orgAddress.GetFormattedAddress("No address found", 1);
            orgPhoneText              = orgPhone       == null    ? "No phone number found" : orgPhone.GetFormattedPhoneNumber("No phone number found");
            orgFaxText                = orgFax         == null    ? "No fax number found"   : orgFax.GetFormattedPhoneNumber("No fax number found");
            orgWebText                = orgWeb         == null    ? "No website found"      : orgWeb.AddrLine1;
            orgEmailText              = orgEmail       == null    ? "No email found"        : orgEmail.AddrLine1;
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus orgAddressAus  = ContactAusDB.GetFirstByEntityID(1,    booking.Organisation != null ? booking.Organisation.EntityID : -1);
            ContactAus orgPhoneAus    = ContactAusDB.GetFirstByEntityID(null, booking.Organisation != null ? booking.Organisation.EntityID : -1, "30,33,34");
            ContactAus orgFaxAus      = ContactAusDB.GetFirstByEntityID(-1,   booking.Organisation != null ? booking.Organisation.EntityID : -1, 29);
            ContactAus orgWebAus      = ContactAusDB.GetFirstByEntityID(-1,   booking.Organisation != null ? booking.Organisation.EntityID : -1, 28);
            ContactAus orgEmailAus    = ContactAusDB.GetFirstByEntityID(-1,   booking.Organisation != null ? booking.Organisation.EntityID : -1, 27);

            orgAddressText            = orgAddressAus      == null ? "No address found"      : orgAddressAus.GetFormattedAddress("No address found");
            orgAddressTabbedText      = orgAddressAus      == null ? "No address found"      : orgAddressAus.GetFormattedAddress("No address found", 1);
            orgPhoneText              = orgPhoneAus        == null ? "No phone number found" : orgPhoneAus.GetFormattedPhoneNumber("No phone number found");
            orgFaxText                = orgFaxAus          == null ? "No fax number found"   : orgFaxAus.GetFormattedPhoneNumber("No fax number found");
            orgWebText                = orgWebAus          == null ? "No website found"      : orgWebAus.AddrLine1;
            orgEmailText              = orgEmailAus        == null ? "No email found"        : orgEmailAus.AddrLine1;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());


        string providerNumber = booking.Provider.ProviderNumber;
        if (!isAgedCare)
        {
            RegisterStaff regStaff = RegisterStaffDB.GetByStaffIDAndOrganisationID(booking.Provider.StaffID, booking.Organisation.OrganisationID, true);
            if (regStaff != null)
                providerNumber = regStaff.ProviderNumber;
        }


        System.Data.DataSet sourceDataSet = new System.Data.DataSet();
        sourceDataSet.Tables.Add("MergeIt");


        sourceDataSet.Tables[0].Columns.Add("curr_date");

        sourceDataSet.Tables[0].Columns.Add("bk_prov_fullname");
        sourceDataSet.Tables[0].Columns.Add("bk_prov_number");
        sourceDataSet.Tables[0].Columns.Add("bk_date");

        sourceDataSet.Tables[0].Columns.Add("bk_org_name");
        sourceDataSet.Tables[0].Columns.Add("bk_org_abn");
        sourceDataSet.Tables[0].Columns.Add("bk_org_acn");
        sourceDataSet.Tables[0].Columns.Add("bk_org_bpay_account");
        sourceDataSet.Tables[0].Columns.Add("bk_org_addr");
        sourceDataSet.Tables[0].Columns.Add("bk_org_addr_tabbedx1");
        sourceDataSet.Tables[0].Columns.Add("bk_org_phone");
        sourceDataSet.Tables[0].Columns.Add("bk_org_office_fax");
        sourceDataSet.Tables[0].Columns.Add("bk_org_web");
        sourceDataSet.Tables[0].Columns.Add("bk_org_email");


        sourceDataSet.Tables[0].Rows.Add(
            DateTime.Now.ToString("d MMMM, yyyy"),

            booking.Provider.Person.FullnameWithoutMiddlename,
            providerNumber,
            booking.DateStart.ToString("d MMMM, yyyy"),

            booking.Organisation.Name,
            booking.Organisation.Abn,
            booking.Organisation.Acn,
            booking.Organisation.BpayAccount,
            orgAddressText,
            orgAddressTabbedText,
            orgPhoneText,
            orgFaxText,
            orgWebText,
            orgEmailText
            );




        string[,] tblInfo = new string[dt.Rows.Count, isAgedCare ? 5 : 4];

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (isAgedCare)
            {
                /*
                    0 = i (row nbr)
                    1 = room (addr1 of patient)
                    2 = resident
                    3 = resident type
                    4 = debtor
                */

                tblInfo[i, 0] = (i + 1).ToString();
                tblInfo[i, 1] = dt.Rows[i]["Room"].ToString();
                tblInfo[i, 2] = dt.Rows[i]["PatientName"].ToString();
                tblInfo[i, 3] = dt.Rows[i]["ItemDescr"].ToString();
                tblInfo[i, 4] = dt.Rows[i]["Debtor"].ToString();
            }
            else
            {
                /*
                    0 = i (row nbr)
                    1 = patient
                    2 = service
                    3 = debtor
                */

                tblInfo[i, 0] = (i + 1).ToString();
                tblInfo[i, 1] = dt.Rows[i]["PatientName"].ToString();
                tblInfo[i, 2] = dt.Rows[i]["ItemDescr"].ToString();
                tblInfo[i, 3] = dt.Rows[i]["Debtor"].ToString();
            }
        }



        // merge

        string errorString = null;
        WordMailMerger.Merge(

            originalFile,
            outputFile,
            sourceDataSet,

            tblInfo,
            1,
            true,

            true,
            null,
            true,
            null,
            out errorString);

        if (errorString != string.Empty)
            throw new Exception(errorString);

    }

    protected void chkIncAddOns_CheckedChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    #endregion  

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        GrdBooking.Visible = false;
        btnExport.Visible = false;
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