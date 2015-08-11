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

public partial class PatientDetailV3 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();
            

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, false);
                hiddenIsMobileDevice.Value = Utilities.IsMobileDevice(Request) ? "1" : "0";

                UserView userView = UserView.GetInstance();

                if (GetUrlParamType() == UrlParamType.None)
                    throw new Exception("No url parameter 'type'");
                if (!IsValidFormID())
                    throw new Exception("No valid id in url");

                Patient patient = PatientDB.GetByID(GetFormID());
                if (patient == null)
                    throw new Exception("Invalid id in url");

                if (Request.QueryString["test"] == null && !patient.IsGPPatient)
                    throw new Exception("Patient is not a GP patient.");

                // add to logged in providers org, then redirect back without add_to_this_org in url
                if (Request.QueryString["add_to_this_org"] != null)
                {
                    if (!userView.IsAdminView && !RegisterPatientDB.IsPatientRegisteredToOrg(patient.PatientID, Convert.ToInt32(Session["OrgID"])))
                        RegisterPatientDB.Insert(Convert.ToInt32(Session["OrgID"]), patient.PatientID);
                    Response.Redirect(UrlParamModifier.Remove(Request.RawUrl, "add_to_this_org"));
                }

                Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
                PatientDB.AddACOfferings(ref offeringsHash, ref patient);

                SetupGUI();
                FillEditViewForm(patient, GetUrlParamType() == UrlParamType.Edit);

                GrdCondition.Visible = false;
                GrdConditionView.Visible = false;
                if (GetUrlParamType() == UrlParamType.Edit)
                {
                    GrdCondition.Visible = true;
                    FillGrdCondition(patient);
                }
                else if (GetUrlParamType() == UrlParamType.View)
                {
                    GrdConditionView.Visible = true;
                    FillGrdConditionView(patient);
                }



                string scannedDocsDir = patient.GetScannedDocsDirectory();
                if (scannedDocsDir.EndsWith("\\")) scannedDocsDir = scannedDocsDir.Substring(0, scannedDocsDir.Length-1);
                this.HomeFolder = scannedDocsDir;

                if (!Directory.Exists(scannedDocsDir))
                    CreateDirectory(scannedDocsDir);

                if (string.IsNullOrEmpty(this.HomeFolder) || !Directory.Exists(GetFullyQualifiedFolderPath(this.HomeFolder)))
                    throw new ArgumentException(String.Format("The HomeFolder setting '{0}' is not set or is invalid", this.HomeFolder));

                int mst = GetFormMST();
                this.CurrentFolder = mst == -1 ? this.HomeFolder : this.HomeFolder + "\\" + "__" + mst + "__";
                FillScannedDocGrid();




                if (GetUrlParamType() == UrlParamType.Edit)
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "setcursor", "<script language=javascript>SetCursorToTextEnd('txtFirstname');</script>");
                if (Utilities.GetAddressType().ToString() == "Contact")
                {
                    addressControl.Visible = true;
                    addressControl.Set(patient.Person.EntityID, true, EntityType.GetByType(EntityType.EntityTypeEnum.Patient), userView.IsAgedCareView);
                }
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                {
                    addressAusControl.Visible = true;
                    addressAusControl.Set(patient.Person.EntityID, true, EntityType.GetByType(EntityType.EntityTypeEnum.Patient), userView.IsAgedCareView);
                }
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

                //txtFirstname.Focus();

                Page.ClientScript.RegisterStartupScript(this.GetType(), "settab", "<script language=javascript>show_tab('tab1');</script>");

            }
            else
            {
                GrdRegistration_Reset();
                if (hiddenFieldSelectedTab.Value.Length > 0)
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "settab", "<script language=javascript>show_tab('" + hiddenFieldSelectedTab.Value + "');</script>");
            }



            try
            {
                lblUploadMessage.Text = string.Empty;
                if (!IsPostBack)
                {
                    Session.Remove("letter_sortexpression");
                    Session.Remove("letter_data");
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                if (IsPostBack) SetErrorMessage("Connection to network files currently unavailable.");
                else HideTableAndSetErrorMessage("Connection to network files currently unavailable.");

            }

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

    #region GetUrlParamType(), IsValidFormID(), GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new CustomMessageException("Invalid url id");

        string id = Request.QueryString["id"];
        return Convert.ToInt32(id);
    }

    private Patient GetFormPatient()
    {
        try
        {
            string patientID = Request.QueryString["id"];
            if (patientID == null || !Regex.IsMatch(patientID, @"^\d+$"))
                throw new CustomMessageException("Invalid patient id");
            return PatientDB.GetByID(Convert.ToInt32(Request.QueryString["id"]));
        }
        catch (CustomMessageException ex)
        {
            HideTableAndSetErrorMessage(Utilities.IsDev() ? ex.Message : "");
            return null;
        }
    }

    private int GetFormMST()
    {
        string mst = Request.QueryString["mst"];
        if (mst == null)
            return -1;
        if (!Regex.IsMatch(mst, @"^\d{1,2}$"))
            throw new CustomMessageException("Invalid mst");

        return Convert.ToInt32(mst);
    }

    private enum UrlParamType { Edit, View, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "view")
            return UrlParamType.View;
        else
            return UrlParamType.None;
    }

    #endregion


    private void SetupGUI()
    {
        ddlDOB_Day.Items.Add(new ListItem("--", "-1"));
        ddlDOB_Month.Items.Add(new ListItem("--", "-1"));
        ddlDOB_Year.Items.Add(new ListItem("----", "-1"));

        for (int i = 1; i <= 31; i++)
            ddlDOB_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1; i <= 12; i++)
            ddlDOB_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1900; i <= DateTime.Today.Year; i++)
            ddlDOB_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

        ddlDARev_Day.Items.Add(new ListItem("--", "-1"));
        ddlDARev_Month.Items.Add(new ListItem("--", "-1"));
        ddlDARev_Year.Items.Add(new ListItem("----", "-1"));

        for (int i = 1; i <= 31; i++)
            ddlDARev_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 1; i <= 12; i++)
            ddlDARev_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 2000; i <= DateTime.Today.Year; i++)
            ddlDARev_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

        txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
        txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate',   this, 'dmy', '-'); return false;";


        ddlConcessionCardExpiry_Month.Items.Add(new ListItem("--", "-1"));
        ddlConcessionCardExpiry_Year.Items.Add(new ListItem("--", "-1"));

        for (int i = 1; i <= 12; i++)
            ddlConcessionCardExpiry_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = DateTime.Today.Year - 10; i <= DateTime.Today.Year + 10; i++)
            ddlConcessionCardExpiry_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

        acTypeRow.Visible = UserView.GetInstance().IsAgedCareView;


        ddlMedicalServiceType.Items.Clear();
        ddlMedicalServiceType.Items.Add(new ListItem("All Medical Services", "-1"));
        foreach (IDandDescr mst in MedicalServiceTypeDB.GetAll())
            ddlMedicalServiceType.Items.Add(new ListItem(mst.Descr, mst.ID.ToString()));

        int selectedMst = GetFormMST();
        if (selectedMst != -1)
            ddlMedicalServiceType.SelectedValue = selectedMst.ToString();


        bool editable = GetUrlParamType() == UrlParamType.Edit;
        Utilities.SetEditControlBackColour(ddlTitle ,                     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtFirstname ,                 editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtMiddlename ,                editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtSurname ,                   editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtNickname ,                  editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlGender ,                    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Day ,                   editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Month ,                 editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDOB_Year ,                  editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDARev_Day ,                 editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDARev_Month ,               editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlDARev_Year ,                editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPrivateHealthFund,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtConcessionCardNbr,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlConcessionCardExpiry_Month, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlConcessionCardExpiry_Year,  editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlACInvOffering,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtLogin,                      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtPwd,                        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtNextOfKinName,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtNextOfKinRelation,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtNextOfKinContactInfo,       editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }

    protected void btnUpdateNotesIcon_Click(object sender, EventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        //lnkNotes.ImageUrl = NoteDB.HasNotes(patient.Person.EntityID) ? "~/images/notes-48.png" : "~/images/notes-bw-48.jpg";

        SetNotesList(patient);
    }
    protected void btnUpdateMedNotesIcon_Click(object sender, EventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        //lnkNotes.ImageUrl = NoteDB.HasNotes(patient.Person.EntityID) ? "~/images/notes-48.png" : "~/images/notes-bw-48.jpg";

        SetMedNotesList(patient);
    }
    protected void btnUpdateMedCondNotesIcon_Click(object sender, EventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        //lnkNotes.ImageUrl = NoteDB.HasNotes(patient.Person.EntityID) ? "~/images/notes-48.png" : "~/images/notes-bw-48.jpg";

        SetMedCondNotesList(patient);
    }
    protected void btnUpdateFlashingTextIcon_Click(object sender, EventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());

        if (patient.FlashingTextAddedBy != null)
            patient.FlashingTextAddedBy = StaffDB.GetByID(patient.FlashingTextAddedBy.StaffID);

        string addedByHoverLink = patient.FlashingTextAddedBy == null ? "" : @"<a href=""javascript:void(0)"" onclick=""javascript:return false;"" title='Added By: " + patient.FlashingTextAddedBy.Person.FullnameWithoutMiddlename + @"' style=""text-decoration: none"">*</a>";
        lblFlashingText.Text = (patient.FlashingTextLastModifiedDate == DateTime.MinValue ? "" : @"<span style=""font-size:70%;font-weight:normal;"">" + "[" + patient.FlashingTextLastModifiedDate.ToString("d'/'M'/'yyyy") + addedByHoverLink + "]</span> ") + patient.FlashingText;
    }
    protected void btnUpdateBookingList_Click(object sender, EventArgs e)
    {
        SetBookingsList();
    }

    protected void btnSearchBookingList_Click(object sender, EventArgs e)
    {
        SetBookingsList();
    }
    protected DataTable SetBookingsList(Patient patient = null)
    {
        if (txtStartDate.Text.Length > 0 && !Utilities.IsValidDate(txtStartDate.Text, "dd-mm-yyyy"))
        {
            SetErrorMessage("Start date must be empty or valid and of the format dd-mm-yyyy");
            return null;
        }
        if (txtEndDate.Text.Length > 0 && !Utilities.IsValidDate(txtEndDate.Text, "dd-mm-yyyy"))
        {
            SetErrorMessage("End date must be empty or valid and of the format dd-mm-yyyy");
            return null;
        }
        DateTime startDate = txtStartDate.Text.Length == 0 ? DateTime.MinValue : Utilities.GetDate(txtStartDate.Text, "dd-mm-yyyy");
        DateTime endDate   = txtEndDate.Text.Length   == 0 ? DateTime.MinValue : Utilities.GetDate(txtEndDate.Text,   "dd-mm-yyyy");

        UserView userView        = UserView.GetInstance();
        int      loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);

        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());


        Invoice[] outstandingInvoices = InvoiceDB.GetOutstanding(patient.PatientID, -1, false);

        // show outstanding for those not attached to a booking on top of booking list
        // show others with the bookings list (below)
        System.Collections.ArrayList outstandingPrivateInvoices = new System.Collections.ArrayList();
        for (int i = 0; i < outstandingInvoices.Length; i++)
            if (outstandingInvoices[i].Booking == null)
                outstandingPrivateInvoices.Add(outstandingInvoices[i]);
        lblInvoiceOwingMessage.Text = string.Empty;
        if (outstandingPrivateInvoices.Count > 0)
        {
            lblInvoiceOwingMessage.Text = "<font color=\"red\">Has outstanding invoices on cash sales:<br>";

            lblInvoiceOwingMessage.Text += "<table class=\"block_center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">";
            for (int i = 0; i < outstandingPrivateInvoices.Count; i++)
            {
                string onclick = @"onclick=""javascript:window.showModalDialog('Invoice_ViewV2.aspx?invoice_id=" + ((Invoice)outstandingPrivateInvoices[i]).InvoiceID + @"', '', 'dialogWidth:675px;dialogHeight:725px;center:yes;resizable:no; scroll:no');return false;""";
                string link = "<a " + onclick + " href=\"\">View Inv.</a>";

                lblInvoiceOwingMessage.Text += "<tr>";
                lblInvoiceOwingMessage.Text += "  <td style=\"width:30px\"></td>";
                lblInvoiceOwingMessage.Text += "  <td><b>Owes $" + ((Invoice)outstandingPrivateInvoices[i]).TotalDue + "</b>"+"</td>";
                lblInvoiceOwingMessage.Text += "  <td style=\"width:15px\"></td>";
                lblInvoiceOwingMessage.Text += "  <td>" + link + "</td>";
                lblInvoiceOwingMessage.Text += "</tr>";
            }

            lblInvoiceOwingMessage.Text += "</table>";
            lblInvoiceOwingMessage.Text += "</font><br />";
        }




        DataTable tblBookingList = BookingDB.GetDataTable_Between(startDate, endDate, null, null, patient, null, true, null);

        int[] booking_ids = new int[tblBookingList.Rows.Count];
        int[] entity_ids  = new int[tblBookingList.Rows.Count];
        for(int i=0; i<tblBookingList.Rows.Count; i++)
        {
            booking_ids[i] = Convert.ToInt32(tblBookingList.Rows[i]["booking_booking_id"]);
            entity_ids[i]  = Convert.ToInt32(tblBookingList.Rows[i]["booking_entity_id"]);
        }
        Hashtable changeHistoryHash = BookingDB.GetChangeHistoryCountHash(booking_ids);

        lblBookingListCount.Text = "("+tblBookingList.Rows.Count+")";
        if (tblBookingList.Rows.Count == 0)
        {
            //lblBookingsList_NoRowsMessage.Visible = true;

            lblBookingsList_NoRowsMessage.Visible = false;

            lstBookingList.DataSource = tblBookingList;
            lstBookingList.DataBind();
            System.Web.UI.HtmlControls.HtmlTableRow lblEmptyData = lstBookingList.Controls[lstBookingList.Controls.Count - 1].Controls[0].FindControl("trEmptyData") as System.Web.UI.HtmlControls.HtmlTableRow;
            lblEmptyData.Visible = true;
            System.Web.UI.HtmlControls.HtmlTableCell tdEmptyData = lstBookingList.Controls[lstBookingList.Controls.Count - 1].Controls[0].FindControl("tdEmptyData") as System.Web.UI.HtmlControls.HtmlTableCell;
            tdEmptyData.Attributes["colspan"] = "100%";


            bk_list_show_invoice_row.Visible        = false;
            bk_list_show_notes_row.Visible          = false;
            bk_list_show_printletter_row.Visible    = false;
            bk_list_show_bookingsheet_row.Visible   = false;
            bk_list_show_outstanding_row.Visible    = false;
            bk_list_show_change_history_row.Visible = false;

        }
        else
        {
            lblBookingsList_NoRowsMessage.Visible = false;
            //pnlBookingsList.Visible = true;


            System.Collections.Hashtable staffHash = StaffDB.GetAllInHashtable(true, true, false, false);
            System.Collections.ArrayList bookingsWithInvoices = new System.Collections.ArrayList();

            Hashtable noteHash = NoteDB.GetNoteHash(entity_ids, true);


            tblBookingList.Columns.Add("notes_text",                  typeof(string));
            tblBookingList.Columns.Add("booking_notes_text",          typeof(string));
            tblBookingList.Columns.Add("invoice_text",                typeof(string));
            tblBookingList.Columns.Add("booking_url",                 typeof(string));
            tblBookingList.Columns.Add("hide_booking_link",           typeof(Boolean));
            tblBookingList.Columns.Add("show_invoice_row",            typeof(int));
            tblBookingList.Columns.Add("show_notes_row",              typeof(int));
            tblBookingList.Columns.Add("show_printletter_row",        typeof(int));
            tblBookingList.Columns.Add("show_bookingsheet_row",       typeof(int));
            tblBookingList.Columns.Add("inv_type_text",               typeof(string));
            tblBookingList.Columns.Add("inv_outstanding_text",        typeof(string));
            tblBookingList.Columns.Add("show_outstanding_row",        typeof(int));
            tblBookingList.Columns.Add("added_by_deleted_by_row",     typeof(string));
            tblBookingList.Columns.Add("booking_files_link",          typeof(string));
            tblBookingList.Columns.Add("booking_change_history_link", typeof(string));
            tblBookingList.Columns.Add("hide_change_history_link",    typeof(Boolean));
            tblBookingList.Columns.Add("show_change_history_row",     typeof(string));
            tblBookingList.Columns.Add("is_deleted",                  typeof(Boolean));
            tblBookingList.Columns.Add("is_cancelled",                typeof(Boolean));
            bool hasInvoiceRows      = false;
            bool hasNotesRows        = false;
            bool hasPrintLetterRows  = false;
            bool hasBookingSheetRows = false;
            bool hasOutstandingRows  = false;
            for (int i = 0; i < tblBookingList.Rows.Count; i++)
            {
                Booking curBooking = BookingDB.LoadFull(tblBookingList.Rows[i]);
                bool    isDeleted = curBooking.DateDeleted != DateTime.MinValue || curBooking.DeletedBy != null;

                if (curBooking.Patient != null)
                {
                    bool IsMobileDevice = Utilities.IsMobileDevice(Request);
                    tblBookingList.Rows[i]["notes_text"] = Note.GetBookingPopupLinkTextV2(curBooking.EntityID, curBooking.NoteCount > 0, true, 1425, 700, "images/notes-bw-24.jpg", "images/notes-24.png", "btnUpdateBookingList.click()", !IsMobileDevice);
                    //tblBookingList.Rows[i]["notes_text"] = Note.GetPopupLinkTextV2(15, curBooking.EntityID, curBooking.NoteCount > 0, true, 980, 530, "images/notes-bw-24.jpg", "images/notes-24.png", "btnUpdateBookingList.click()");
                }


                string notesText = string.Empty;
                Note[] bkNotes = (Note[])noteHash[curBooking.EntityID];
                if (bkNotes != null)
                {
                    foreach (Note note in bkNotes)
                    {
                        if (note.IsDeleted)
                        {
                            // just dont show if deleted - they can open the notes to see it

                            /*
                            notesText += (notesText.Length == 0 ? "" : "<br /><br />") +
                                "<font color=\"gray\">" +
                                "<u>" + note.DateAdded.ToString("dd / MM / yyyy") + "</u>" + "<br />" +
                                "<u>" + note.NoteType.Descr + "</u>" + "<br />" +
                                "*** DELETED ***<br />" +
                                ((note.BodyPart.ID == -1 || note.BodyPart.Descr.Length == 0) ? "" : @"<u>" + Eval("body_part_descr") + "</u> : ") + note.Text.Replace("\n", "<br/>") +
                                "</font>";
                            */
                        }
                        else
                        {
                            notesText += (notesText.Length == 0 ? "" : "<br /><br />") +
                                "<b><u>" + note.DateAdded.ToString("dd / MM / yyyy") + "</u></b>" + "<br />" +
                                "<u>" + note.NoteType.Descr + "</u>" + "<br />" +
                                "<font color=\"#a52a2a\">" + ((note.BodyPart.ID == -1 || note.BodyPart.Descr.Length == 0) ? "" : "<i>" + note.BodyPart.Descr + "</i>:<br />") + note.Text.Replace("\n", "<br/>") + "</font>";
                        }
                    }
                }
                if (notesText.Length == 0)
                    notesText = "No Notes.";
                tblBookingList.Rows[i]["booking_notes_text"] = notesText;


                bool canSeeInvoiceInfo = userView.IsAdminView || userView.IsPrincipal || (curBooking.Provider != null && curBooking.Provider.StaffID == loggedInStaffID && curBooking.DateStart > DateTime.Today.AddMonths(-2));
                if (canSeeInvoiceInfo && Convert.ToInt32(tblBookingList.Rows[i]["booking_inv_count"]) > 0)
                {
                    string onclick = @"onclick=""javascript:window.showModalDialog('Invoice_ViewV2.aspx?booking_id=" + curBooking.BookingID + @"', '', 'dialogWidth:820px;dialogHeight:860px;center:yes;resizable:no; scroll:no');return false;""";
                    tblBookingList.Rows[i]["invoice_text"] = "<a " + onclick + " href=\"\">View Inv.</a>";

                    if (curBooking.DateDeleted == DateTime.MinValue && curBooking.DeletedBy == null)
                        hasInvoiceRows = true;

                    bookingsWithInvoices.Add(curBooking.BookingID);
                }
                else
                {
                    tblBookingList.Rows[i]["invoice_text"] = "";
                }

                tblBookingList.Rows[i]["hide_booking_link"] = !((userView.IsClinicView   && curBooking.Organisation.OrganisationType.OrganisationTypeID == 218) ||
                                                                (userView.IsAgedCareView && (new List<int> { 139, 367, 372 }).Contains(curBooking.Organisation.OrganisationType.OrganisationTypeID)));

                if (curBooking.DateDeleted == DateTime.MinValue && curBooking.DeletedBy == null)
                {
                    hasNotesRows        = true;
                    hasPrintLetterRows  = true;
                    if (!Convert.ToBoolean(tblBookingList.Rows[i]["hide_booking_link"]))
                        hasBookingSheetRows = true;
                }

                string urlParams = string.Empty;
                if (curBooking.Organisation != null)
                    urlParams += (urlParams.Length == 0 ? "?" : "&") + "orgs=" + curBooking.Organisation.OrganisationID;
                if (curBooking.Patient != null)
                    urlParams += (urlParams.Length == 0 ? "?" : "&") + "patient=" + curBooking.Patient.PatientID;
                urlParams += (urlParams.Length == 0 ? "?" : "&") + "scroll_to_cell=" + "td_" + (curBooking.Organisation != null ? "" : curBooking.Organisation.OrganisationID.ToString()) + "_" + curBooking.Provider.StaffID + "_" + curBooking.DateStart.ToString("yyyy_MM_dd_HHmm");
                urlParams += (urlParams.Length == 0 ? "?" : "&") + "date=" + curBooking.DateStart.ToString("yyyy_MM_dd");
                tblBookingList.Rows[i]["booking_url"] = curBooking.GetBookingSheetLinkV2();

                decimal totalOwing = 0;
                for(int j=0; j<outstandingInvoices.Length; j++)
                    if (outstandingInvoices[j].Booking != null && outstandingInvoices[j].Booking.BookingID == curBooking.BookingID)
                        totalOwing += outstandingInvoices[j].TotalDue;
                tblBookingList.Rows[i]["inv_outstanding_text"] = totalOwing == 0 ? "" : "<font color=\"red\"><b>Owes<br />$" + totalOwing.ToString() + "</b></font>";
                if (totalOwing > 0)
                    hasOutstandingRows = true;


                string addedBy     = curBooking.AddedBy     == null || staffHash[curBooking.AddedBy.StaffID]     == null ? "" : ((Staff)staffHash[curBooking.AddedBy.StaffID]).Person.FullnameWithoutMiddlename;
                string addedDate   = curBooking.DateCreated == DateTime.MinValue                                         ? "" : curBooking.DateCreated.ToString("MMM d, yyyy");
                string deletedBy   = curBooking.DeletedBy   == null || staffHash[curBooking.DeletedBy.StaffID]   == null ? "" : ((Staff)staffHash[curBooking.DeletedBy.StaffID]).Person.FullnameWithoutMiddlename;
                string deletedDate = curBooking.DateDeleted == DateTime.MinValue                                         ? "" : curBooking.DateDeleted.ToString("MMM d, yyyy");
                string cancelledBy = curBooking.CancelledBy == null || staffHash[curBooking.CancelledBy.StaffID] == null ? "" : ((Staff)staffHash[curBooking.CancelledBy.StaffID]).Person.FullnameWithoutMiddlename;
                string cancelledDate = curBooking.DateCancelled == DateTime.MinValue                                     ? "" : curBooking.DateCancelled.ToString("MMM d, yyyy");
                
                string added_by_deleted_by_row = string.Empty;
                added_by_deleted_by_row += "Added By: " + addedBy + " (" + addedDate + ")";
                if (deletedBy.Length > 0 || deletedDate.Length > 0)
                    added_by_deleted_by_row += "\r\nDeleted By: " + deletedBy + " (" + deletedDate + ")";
                if (cancelledBy.Length > 0 || cancelledDate.Length > 0)
                    added_by_deleted_by_row += "\r\nCancelled By: " + cancelledBy + " (" + cancelledDate + ")";
                
                tblBookingList.Rows[i]["added_by_deleted_by_row"]     = added_by_deleted_by_row;
                tblBookingList.Rows[i]["booking_files_link"]          = curBooking.GetScannedDocsImageLink(true);
                tblBookingList.Rows[i]["booking_change_history_link"] = curBooking.GetBookingChangeHistoryPopupLinkImage();
                tblBookingList.Rows[i]["hide_change_history_link"]    = changeHistoryHash[curBooking.BookingID] == null;
                tblBookingList.Rows[i]["is_deleted"]                  = (curBooking.BookingStatus != null && curBooking.BookingStatus.ID == -1) || isDeleted;
                tblBookingList.Rows[i]["is_cancelled"]                = curBooking.BookingStatus  != null && curBooking.BookingStatus.ID == 188;
            }

            System.Collections.Hashtable hashHasMedicareOrDVAInvoices = BookingDB.GetHashHasMedicareDVA((int[])bookingsWithInvoices.ToArray(typeof(int)), patient.PatientID);

            for (int i = 0; i < tblBookingList.Rows.Count; i++)
            {
                tblBookingList.Rows[i]["show_invoice_row"]        = hasInvoiceRows              ? 1 : 0;
                tblBookingList.Rows[i]["show_notes_row"]          = hasNotesRows                ? 1 : 0;
                tblBookingList.Rows[i]["show_printletter_row"]    = hasPrintLetterRows          ? 1 : 0;
                tblBookingList.Rows[i]["show_bookingsheet_row"]   = hasBookingSheetRows         ? 1 : 0;
                tblBookingList.Rows[i]["show_outstanding_row"]    = hasOutstandingRows          ? 1 : 0;
                tblBookingList.Rows[i]["show_change_history_row"] = changeHistoryHash.Count > 0 ? 1 : 0;

                int  booking_id   = Convert.ToInt32(tblBookingList.Rows[i]["booking_booking_id"]);
                bool has_medicare = hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -1)] != null && Convert.ToBoolean(hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -1)]);
                bool has_dva      = hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -2)] != null && Convert.ToBoolean(hashHasMedicareOrDVAInvoices[new Hashtable2D.Key(booking_id, -2)]);
                if (has_medicare) tblBookingList.Rows[i]["inv_type_text"] = "Medicare";
                else if (has_dva) tblBookingList.Rows[i]["inv_type_text"] = "DVA";
                else              tblBookingList.Rows[i]["inv_type_text"] = string.Empty;
            }
            tblBookingList.DefaultView.Sort = "booking_date_start DESC";
            tblBookingList = tblBookingList.DefaultView.ToTable();
            lstBookingList.DataSource = tblBookingList;
            lstBookingList.DataBind();

            bk_list_show_invoice_row.Visible        = hasInvoiceRows;
            bk_list_show_notes_row.Visible          = hasNotesRows;
            bk_list_show_printletter_row.Visible    = hasPrintLetterRows;
            bk_list_show_bookingsheet_row.Visible   = hasBookingSheetRows;
            bk_list_show_outstanding_row.Visible    = hasOutstandingRows;
            bk_list_show_change_history_row.Visible = changeHistoryHash.Count > 0;
        }

        return tblBookingList;
    }

    protected void btnPrintBookingList_Click(object sender, EventArgs e)
    {
        DataTable tblBookingList = SetBookingsList();
        if (tblBookingList == null)
            return;

        try
        {
            string originalFile        = Letter.GetLettersDirectory() + @"BookingListForPatient.docx";
            string tmpLettersDirectory = Letter.GetTempLettersDirectory();
            string tmpOutputFile       = FileHelper.GetTempFileName(tmpLettersDirectory + "BookingList." + System.IO.Path.GetExtension(originalFile));


            // create table data to populate

            DataTable dt = tblBookingList;
            string[,] tblInfo = null;
            bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
            if (tblEmpty)
            {
                tblInfo = new string[1, 4];
                tblInfo[0, 0] = "No Bookings Found";
                tblInfo[0, 1] = "";
                tblInfo[0, 2] = "";
                tblInfo[0, 3] = "";
            }
            else
            {
                tblInfo = new string[dt.Rows.Count, 4];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string inv_type_text = tblBookingList.Rows[i]["inv_type_text"].ToString();
                    if (inv_type_text.Length > 0) inv_type_text = " (" + inv_type_text + ")";

                    Booking booking = BookingDB.LoadFull(dt.Rows[i]);
                    tblInfo[i, 0] = booking.DateStart.ToString("d MMM yyyy") + Environment.NewLine + booking.DateStart.ToString("h:mm") + " - " + booking.DateEnd.ToString("h:mm") + (booking.DateEnd.Hour < 12 ? "am" : "pm");
                    tblInfo[i, 1] = booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename;
                    tblInfo[i, 2] = (booking.Offering == null ? "" : booking.Offering.Name + Environment.NewLine) + booking.Provider.Person.FullnameWithoutMiddlename + " @ " + booking.Organisation.Name;
                    tblInfo[i, 3] = booking.BookingStatus.Descr + Environment.NewLine + inv_type_text;
                }
            }


            // create empty dataset

            System.Data.DataSet sourceDataSet = new System.Data.DataSet();
            sourceDataSet.Tables.Add("MergeIt");


            // merge

            string errorString = null;
            WordMailMerger.Merge(

                originalFile,
                tmpOutputFile,
                sourceDataSet,

                tblInfo,
                1,
                true,

                false,
                null,
                true,
                null,
                out errorString);

            if (errorString != string.Empty)
                throw new CustomMessageException(errorString);

            Letter.FileContents fileContents = new Letter.FileContents(System.IO.File.ReadAllBytes(tmpOutputFile), "BookingList." + System.IO.Path.GetExtension(originalFile));
            System.IO.File.Delete(tmpOutputFile);


            // Nothing gets past the "DownloadDocument" method because it outputs the file 
            // which is writing a response to the client browser and calls Response.End()
            // So make sure any other code that functions goes before this
            Letter.DownloadDocument(Response, fileContents.Contents, fileContents.DocName);
        }
        catch(CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch(Exception ex)
        {
            SetErrorMessage(ex.ToString());
        }

    }


    protected void btnUpdateNoteList_Click(object sender, EventArgs e)
    {
        SetNotesList();
    }
    protected void SetNotesList(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        DataTable tblNoteList = NoteDB.GetDataTable_ByEntityID(patient.Person.EntityID, null, false);
        for (int i = tblNoteList.Rows.Count - 1; i >= 0; i--)
        {
            Note note = NoteDB.Load(tblNoteList.Rows[i]);
            if (note.NoteType.ID == 1 || note.NoteType.ID == 2)
                tblNoteList.Rows.RemoveAt(i);
        }

        lblNotesListCount.Text = "(" + tblNoteList.Rows.Count + ")";
        if (tblNoteList.Rows.Count == 0)
        {
            //lblNotesList_NoRowsMessage.Visible = true;
            //pnlNotesList.Visible = false;

            lblNotesList_NoRowsMessage.Visible = false;

            lstNoteList.DataSource = tblNoteList;
            lstNoteList.DataBind();
            System.Web.UI.HtmlControls.HtmlTableRow lblEmptyData = lstNoteList.Controls[lstNoteList.Controls.Count - 1].Controls[0].FindControl("trEmptyData") as System.Web.UI.HtmlControls.HtmlTableRow;
            lblEmptyData.Visible = true;
            System.Web.UI.HtmlControls.HtmlTableCell tdEmptyData = lstNoteList.Controls[lstNoteList.Controls.Count - 1].Controls[0].FindControl("tdEmptyData") as System.Web.UI.HtmlControls.HtmlTableCell;
            tdEmptyData.Attributes["colspan"] = "100%";
        }
        else
        {
            lblNotesList_NoRowsMessage.Visible = false;
            pnlNotesList.Visible = true;

            //tblBookingList.DefaultView.Sort = "booking_date_start DESC";
            lstNoteList.DataSource = tblNoteList;
            lstNoteList.DataBind();
        }


        //
        // set booking list scroll panel max-height depending on size of notes list
        //

        int totalNoteLines = 0;
        for (int i = 0; i < tblNoteList.Rows.Count; i++)
            totalNoteLines += ((int)(NoteDB.Load(tblNoteList.Rows[i]).Text.Length / 70) + 1);

        /*
        if (totalNoteLines > 10)
            pnlBookingsList.Style["max-height"] = "250px";
        else if (totalNoteLines > 6)
            pnlBookingsList.Style["max-height"] = "320px";
        else if (totalNoteLines > 2)
            pnlBookingsList.Style["max-height"] = "390px";
        else
            pnlBookingsList.Style["max-height"] = "470px";
        */

    }

    protected void btnUpdateMedNoteList_Click(object sender, EventArgs e)
    {
        SetMedNotesList();
    }
    protected void SetMedNotesList(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        DataTable tblMedNoteList = NoteDB.GetDataTable_ByEntityID(patient.Person.EntityID, "1", false);

        lblMedNotesListCount.Text = "(" + tblMedNoteList.Rows.Count + ")";
        if (tblMedNoteList.Rows.Count == 0)
        {
            //lblMedNotesList_NoRowsMessage.Visible = true;
            //pnlMedNotesList.Visible = false;

            lblMedNotesList_NoRowsMessage.Visible = false;

            lstMedNoteList.DataSource = tblMedNoteList;
            lstMedNoteList.DataBind();
            System.Web.UI.HtmlControls.HtmlTableRow lblEmptyData = lstMedNoteList.Controls[lstMedNoteList.Controls.Count - 1].Controls[0].FindControl("trEmptyData") as System.Web.UI.HtmlControls.HtmlTableRow;
            lblEmptyData.Visible = true;
            System.Web.UI.HtmlControls.HtmlTableCell tdEmptyData = lstMedNoteList.Controls[lstMedNoteList.Controls.Count - 1].Controls[0].FindControl("tdEmptyData") as System.Web.UI.HtmlControls.HtmlTableCell;
            tdEmptyData.Attributes["colspan"] = "100%";
        }
        else
        {
            lblMedNotesList_NoRowsMessage.Visible = false;
            pnlMedNotesList.Visible = true;

            //tblBookingList.DefaultView.Sort = "booking_date_start DESC";
            lstMedNoteList.DataSource = tblMedNoteList;
            lstMedNoteList.DataBind();
        }


        //
        // set booking list scroll panel max-height depending on size of notes list
        //

        int totalNoteLines = 0;
        for (int i = 0; i < tblMedNoteList.Rows.Count; i++)
            totalNoteLines += ((int)(NoteDB.Load(tblMedNoteList.Rows[i]).Text.Length / 70) + 1);

        /*
        if (totalNoteLines > 10)
            pnlBookingsList.Style["max-height"] = "250px";
        else if (totalNoteLines > 6)
            pnlBookingsList.Style["max-height"] = "320px";
        else if (totalNoteLines > 2)
            pnlBookingsList.Style["max-height"] = "390px";
        else
            pnlBookingsList.Style["max-height"] = "470px";
        */

    }

    protected void btnUpdateMedCondNoteList_Click(object sender, EventArgs e)
    {
        SetMedCondNotesList();
    }
    protected void SetMedCondNotesList(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        DataTable tblMedCondNoteList = NoteDB.GetDataTable_ByEntityID(patient.Person.EntityID, "2", false);

        lblMedCondNotesListCount.Text = "(" + tblMedCondNoteList.Rows.Count + ")";
        if (tblMedCondNoteList.Rows.Count == 0)
        {
            //lblMedCondNotesList_NoRowsMessage.Visible = true;
            //pnlMedCondNotesList.Visible = false;

            lblMedCondNotesList_NoRowsMessage.Visible = false;

            lstMedCondNoteList.DataSource = tblMedCondNoteList;
            lstMedCondNoteList.DataBind();
            System.Web.UI.HtmlControls.HtmlTableRow lblEmptyData = lstMedCondNoteList.Controls[lstMedCondNoteList.Controls.Count - 1].Controls[0].FindControl("trEmptyData") as System.Web.UI.HtmlControls.HtmlTableRow;
            lblEmptyData.Visible = true;
            System.Web.UI.HtmlControls.HtmlTableCell tdEmptyData = lstMedCondNoteList.Controls[lstMedCondNoteList.Controls.Count - 1].Controls[0].FindControl("tdEmptyData") as System.Web.UI.HtmlControls.HtmlTableCell;
            tdEmptyData.Attributes["colspan"] = "100%";
        }
        else
        {
            lblMedCondNotesList_NoRowsMessage.Visible = false;
            pnlMedCondNotesList.Visible = true;

            //tblBookingList.DefaultView.Sort = "booking_date_start DESC";
            lstMedCondNoteList.DataSource = tblMedCondNoteList;
            lstMedCondNoteList.DataBind();
        }


        //
        // set booking list scroll panel max-height depending on size of notes list
        //

        int totalNoteLines = 0;
        for (int i = 0; i < tblMedCondNoteList.Rows.Count; i++)
            totalNoteLines += ((int)(NoteDB.Load(tblMedCondNoteList.Rows[i]).Text.Length / 70) + 1);

        /*
        if (totalNoteLines > 10)
            pnlBookingsList.Style["max-height"] = "250px";
        else if (totalNoteLines > 6)
            pnlBookingsList.Style["max-height"] = "320px";
        else if (totalNoteLines > 2)
            pnlBookingsList.Style["max-height"] = "390px";
        else
            pnlBookingsList.Style["max-height"] = "470px";
        */

    }


    private void FillEditViewForm(Patient patient, bool isEditMode)
    {
        UserView userView = UserView.GetInstance();

        if (patient.IsDeleted && isEditMode && !userView.IsAdminView)
            isEditMode = false;


        Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
        PatientDB.AddACOfferings(ref offeringsHash, ref patient);


        patient.Person = PersonDB.GetByID(patient.Person.PersonID);
        Person addedBy = patient.Person.PersonID < 0 ? null : PersonDB.GetByID(patient.Person.AddedBy);

        lblHeading.Text = patient.Person.FullnameWithoutMiddlename + (patient.Person.Nickname.Length == 0 ? "" : " (" + patient.Person.Nickname + ")");
        lblPatientID.Text = "[ID:" + patient.PatientID + "]";
        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + patient.Person.FullnameWithoutMiddlename;

        string screen_id = "6";
        string screen_id_body_chart  = "16";
        string screen_id_medications = "17";
        string screen_id_medical_conditions = "18";
        string allFeatures = "dialogWidth:980px;dialogHeight:530;center:yes;resizable:no; scroll:no";
        string allFeaturesBodyChart = "dialogWidth:1200;dialogHeight:640;center:yes;resizable:no; scroll:no";
        string js          = "javascript:window.showModalDialog('" + "NoteListV2.aspx?id=" + patient.Person.EntityID.ToString() + "&screen=" + screen_id                    + "', '', '" + allFeatures          + "');document.getElementById('btnUpdateNotesIcon').click();return false;";
        string jsBodyChart = "javascript:window.showModalDialog('" + "NoteListV2.aspx?id=" + patient.Person.EntityID.ToString() + "&screen=" + screen_id_body_chart         + "', '', '" + allFeaturesBodyChart + "');document.getElementById('btnUpdateNotesIcon').click();return false;";
        string jsMed       = "javascript:window.showModalDialog('" + "NoteListV2.aspx?id=" + patient.Person.EntityID.ToString() + "&screen=" + screen_id_medications        + "', '', '" + allFeatures          + "');document.getElementById('btnUpdateMedNotesIcon').click();return false;";
        string jsMedCond   = "javascript:window.showModalDialog('" + "NoteListV2.aspx?id=" + patient.Person.EntityID.ToString() + "&screen=" + screen_id_medical_conditions + "', '', '" + allFeatures          + "');document.getElementById('btnUpdateMedCondNotesIcon').click();return false;";
        this.lnkNotes.Attributes["onclick"]          = js;
        this.lnkNotesBodyChart.Attributes["onclick"] = jsBodyChart;
        this.lnkMedNotes.Attributes["onclick"]       = jsMed;
        this.lnkMedCondNotes.Attributes["onclick"]   = jsMedCond;


        string allFeaturesFlashingText = "dialogWidth:525px;dialogHeight:255px;center:yes;resizable:no; scroll:no";
        string jsFlashingText = "javascript: document.getElementById('lblFlashingText').style.display = ''; window.showModalDialog('" + "PatientFlashingMessageDetailV2.aspx?type=edit&id=" + patient.PatientID.ToString() + "', '', '" + allFeaturesFlashingText + "');document.getElementById('btnUpdateFlashingTextIcon').click();return false;";
        this.lnkFlashingText.Attributes.Add("onclick", jsFlashingText);
        this.lnkFlashingText.ImageUrl = "~/images/asterisk_12.png";

        if (patient.FlashingTextAddedBy != null)
            patient.FlashingTextAddedBy = StaffDB.GetByID(patient.FlashingTextAddedBy.StaffID);
        string addedByHoverLink = patient.FlashingTextAddedBy == null ? "" : @"<a href=""javascript:void(0)"" onclick=""javascript:return false;"" title='Added By: " + patient.FlashingTextAddedBy.Person.FullnameWithoutMiddlename + @"' style=""text-decoration: none"">*</a>";
        this.lblFlashingText.Text = (patient.FlashingTextLastModifiedDate == DateTime.MinValue ? "" : @"<span style=""font-size:70%;font-weight:normal;"">" + "[" + patient.FlashingTextLastModifiedDate.ToString("d'/'M'/'yyyy") + addedByHoverLink + "]</span> ") + patient.FlashingText;
        //this.lblFlashingText.Attributes.Add("onclick", jsFlashingText);


        if (!Utilities.IsDev())
            idRow.Attributes["class"] = "hiddencol";

        this.lnkMakeBooking.Visible = true;
        this.lnkMakeBooking.Text = "Book Other Clinic";
        this.lnkMakeBooking.NavigateUrl = userView.IsAgedCareView ?
            "~/SelectOrganisationsV2.aspx" :
            "~/SelectOrganisationsV2.aspx?patient=" + GetFormID().ToString();

        

        this.lnkInvoices.Visible = true;
        this.lnkInvoices.Text = "View Invoices";
        this.lnkInvoices.NavigateUrl = "~/InvoiceListV2.aspx?patient=" + GetFormID().ToString();



        Booking mostRecentBooking = BookingDB.GetMostRecent(patient.PatientID);
        this.lnkLastBooking.Text = mostRecentBooking == null ? "None" : mostRecentBooking.DateStart.ToString("d MMM yyyy");
        this.lnkLastBooking.NavigateUrl = mostRecentBooking == null ? "javascript:return false;" : mostRecentBooking.GetBookingSheetLinkV2();

        Booking nextBooking = BookingDB.GetNextAfterNow(patient.PatientID);
        this.lnkNextBooking.Text = nextBooking == null ? "None" : nextBooking.DateStart.ToString("d MMM yyyy");
        this.lnkNextBooking.NavigateUrl = nextBooking == null ? "javascript:return false;" : nextBooking.GetBookingSheetLinkV2();


        this.lnkLetterPrintHistory.NavigateUrl = String.Format("~/Letters_SentHistoryV2.aspx?patient={0}", patient.PatientID);
        this.lnkPrintLetter.NavigateUrl = String.Format("~/Letters_PrintV2.aspx?patient={0}", patient.PatientID);


        lblId.Text               = patient.PatientID.ToString();
        lblAddedBy.Text          = addedBy == null ? "--" : addedBy.Firstname + " " + addedBy.Surname;
        lblPatientDateAdded.Text = patient.PatientDateAdded.ToString("dd-MM-yyyy");

        btnDeleteUndeletePatient.CommandName = patient.IsDeleted ? "UnDelete" : "Delete";
        btnDeleteUndeletePatient.Text = patient.IsDeleted ? "Un-Archive" : "Archive";
        lblPatientStatus.Text = patient.IsDeleted ? "<b><font color=\"red\">Archived</font></b>" : "Active";

        btnHistory.OnClientClick = @"javascript:window.showModalDialog('/PatientEditHistoryV2.aspx?id=" + patient.PatientID + @"', '', 'dialogWidth:" + (!userView.IsAgedCareView ? "1350" : "1450") + "px;dialogHeight:450px;center:yes;resizable:no; scroll:no');return false;";
        changeHistoryLinknRow.Visible = PatientHistoryDB.Exists(patient.PatientID);


        if (isEditMode)
        {
            DataTable dt_titles = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Title", "", " descr ", "title_id", "descr");
            for (int i = dt_titles.Rows.Count - 1; i >= 0; i--)
            {
                if (Convert.ToInt32(dt_titles.Rows[i]["title_id"]) == 0)
                {
                    DataRow newRow = dt_titles.NewRow();
                    newRow.ItemArray = dt_titles.Rows[i].ItemArray;
                    dt_titles.Rows.RemoveAt(i);
                    dt_titles.Rows.InsertAt(newRow, 0);
                    break;
                }
            }
            ddlTitle.DataSource = dt_titles;
            ddlTitle.DataBind();

            ddlTitle.SelectedValue  = patient.Person.Title.ID.ToString();
            txtFirstname.Text       = patient.Person.Firstname;
            txtMiddlename.Text      = patient.Person.Middlename;
            txtSurname.Text         = patient.Person.Surname;
            txtNickname.Text        = patient.Person.Nickname;
            if (ddlGender.Items.FindByValue(patient.Person.Gender) == null)
                ddlGender.Items.Add(new ListItem(patient.Person.Gender == "" ? "--" : patient.Person.Gender, patient.Person.Gender));
            ddlGender.SelectedValue = patient.Person.Gender;

            if (patient.Person.Dob != DateTime.MinValue)
            {
                ddlDOB_Day.SelectedValue   = patient.Person.Dob.Day.ToString();
                ddlDOB_Month.SelectedValue = patient.Person.Dob.Month.ToString();

                int firstYearSelectable = Convert.ToInt32(ddlDOB_Year.Items[1].Value);
                int lastYearSelectable  = Convert.ToInt32(ddlDOB_Year.Items[ddlDOB_Year.Items.Count - 1].Value);
                if (patient.Person.Dob.Year < firstYearSelectable)
                    ddlDOB_Year.Items.Insert(1, new ListItem(patient.Person.Dob.Year.ToString(), patient.Person.Dob.Year.ToString()));
                if (patient.Person.Dob.Year > lastYearSelectable)
                    ddlDOB_Year.Items.Add(new ListItem(patient.Person.Dob.Year.ToString(), patient.Person.Dob.Year.ToString()));

                ddlDOB_Year.SelectedValue = patient.Person.Dob.Year.ToString();
            }
            if (patient.DiabeticAAassessmentReviewDate != DateTime.MinValue)
            {
                ddlDARev_Day.SelectedValue = patient.DiabeticAAassessmentReviewDate.Day.ToString();
                ddlDARev_Month.SelectedValue = patient.DiabeticAAassessmentReviewDate.Month.ToString();

                int firstYearSelectable = Convert.ToInt32(ddlDARev_Year.Items[1].Value);
                int lastYearSelectable = Convert.ToInt32(ddlDARev_Year.Items[ddlDARev_Year.Items.Count - 1].Value);
                if (patient.DiabeticAAassessmentReviewDate.Year < firstYearSelectable)
                    ddlDARev_Year.Items.Insert(1, new ListItem(patient.DiabeticAAassessmentReviewDate.Year.ToString(), patient.DiabeticAAassessmentReviewDate.Year.ToString()));
                if (patient.DiabeticAAassessmentReviewDate.Year > lastYearSelectable)
                    ddlDARev_Year.Items.Add(new ListItem(patient.DiabeticAAassessmentReviewDate.Year.ToString(), patient.DiabeticAAassessmentReviewDate.Year.ToString()));

                ddlDARev_Year.SelectedValue = patient.DiabeticAAassessmentReviewDate.Year.ToString();
            }

            chkIsClinicPatient.Checked = patient.IsClinicPatient;
            chkIsDeceased.Checked = patient.IsDeceased;
            chkIsDiabetic.Checked = patient.IsDiabetic;
            chkIsMemberDiabetesAustralia.Checked = patient.IsMemberDiabetesAustralia;

            txtPrivateHealthFund.Text = patient.PrivateHealthFund;
            txtConcessionCardNbr.Text = patient.ConcessionCardNumber;
            if (patient.ConcessionCardExpiryDate != DateTime.MinValue)
            {
                Utilities.AddIfNotExists(ddlConcessionCardExpiry_Month, patient.ConcessionCardExpiryDate.Month);
                ddlConcessionCardExpiry_Month.SelectedValue = patient.ConcessionCardExpiryDate.Month.ToString();

                Utilities.AddIfNotExists(ddlConcessionCardExpiry_Year, patient.ConcessionCardExpiryDate.Year);
                ddlConcessionCardExpiry_Year.SelectedValue = patient.ConcessionCardExpiryDate.Year.ToString();
            }




            int    ac_inv_offering_id   = patient.ACInvOffering == null ? -1           : patient.ACInvOffering.OfferingID;
            int    ac_pat_offering_id   = patient.ACPatOffering == null ? -1           : patient.ACPatOffering.OfferingID;
            string ac_inv_offering_name = patient.ACInvOffering == null ? string.Empty : patient.ACInvOffering.Name;
            string ac_pat_offering_name = patient.ACPatOffering == null ? string.Empty : patient.ACPatOffering.Name;

            int    ac_inv_aged_care_patient_type_id    = patient.ACInvOffering == null ? -1           : patient.ACInvOffering.AgedCarePatientType.ID;
            string ac_inv_aged_care_patient_type_descr = patient.ACInvOffering == null ? string.Empty : patient.ACInvOffering.AgedCarePatientType.Descr;
            int    ac_pat_aged_care_patient_type_id    = patient.ACInvOffering == null ? -1           : patient.ACPatOffering.AgedCarePatientType.ID;
            string ac_pat_aged_care_patient_type_descr = patient.ACInvOffering == null ? string.Empty : patient.ACPatOffering.AgedCarePatientType.Descr;

            DataTable dt_offerings = OfferingDB.GetDataTable(true, -1, null, true);
            for (int i = dt_offerings.Rows.Count - 1; i >= 0; i--)
            {
                Offering o = OfferingDB.LoadAll(dt_offerings.Rows[i]);
                int o_id = o.OfferingID;
                int o_ac_pt_id = Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"]);

                if (Convert.ToInt32(dt_offerings.Rows[i]["o_offering_id"]) != ac_inv_offering_id &&
                    (Convert.ToInt32(dt_offerings.Rows[i]["o_aged_care_patient_type_id"]) == 1 || Convert.ToBoolean(dt_offerings.Rows[i]["o_is_deleted"])))
                    dt_offerings.Rows.RemoveAt(i);

                // if clinic patient and no ac pt type set, only allow HC/LC/HCU/LCF
                else if ((ac_inv_aged_care_patient_type_id == -1 || ac_pat_aged_care_patient_type_id == -1) &&
                    !(new List<int> { 2, 3, 4, 5 }).Contains(Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"])))
                    dt_offerings.Rows.RemoveAt(i);

                else if (!(new List<int> { 2, 4 }).Contains(ac_pat_aged_care_patient_type_id) && Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"]) == 9) // if not LC/LCF - remove option for DVA
                    dt_offerings.Rows.RemoveAt(i);
                else if (!(new List<int> { 2, 4 }).Contains(ac_pat_aged_care_patient_type_id) && Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"]) == 6) // if not LC/LCF - remove option for LCE
                    dt_offerings.Rows.RemoveAt(i);
                else if (!(new List<int> { 3, 5 }).Contains(ac_pat_aged_care_patient_type_id) && Convert.ToInt32(dt_offerings.Rows[i]["acpatientcat_aged_care_patient_type_id"]) == 7) // if not HC/HCU - remove option for HCE
                    dt_offerings.Rows.RemoveAt(i);

                else if (ac_inv_offering_id != -1 && (new List<int> { 2, 3, 4, 5 }).Contains(ac_inv_aged_care_patient_type_id) && (new List<int> { 6, 7, 8, 9, 10 }).Contains(Convert.ToInt32(dt_offerings.Rows[i]["o_aged_care_patient_type_id"])))
                    dt_offerings.Rows[i]["o_name"] = dt_offerings.Rows[i]["o_name"].ToString() + " (" + ac_pat_offering_name + ")";
            }

            DataView dv = dt_offerings.DefaultView;
            dv.Sort = "acpatientcat_aged_care_patient_type_id, o_name";
            dt_offerings = dv.ToTable();

            ddlACInvOffering.DataSource = dt_offerings;
            ddlACInvOffering.DataBind();

            if (patient.ACInvOffering != null)
                ddlACInvOffering.SelectedValue = patient.ACInvOffering.OfferingID.ToString();

            txtLogin.Text = patient.Login;
            txtPwd.Text = patient.Pwd;

            txtNextOfKinName.Text        = patient.NextOfKinName;
            txtNextOfKinRelation.Text    = patient.NextOfKinRelation;
            txtNextOfKinContactInfo.Text = patient.NextOfKinContactInfo;

            lblTitle.Visible                = false;
            lblFirstname.Visible            = false;
            lblMiddlename.Visible           = false;
            lblSurname.Visible              = false;
            lblNickname.Visible             = false;
            lblGender.Visible               = false;
            lblDOB.Visible                  = false;
            lblDARev.Visible                = false;
            lblIsClinicPatient.Visible      = false;
            lblIsDeceased.Visible           = false;
            lblIsDiabetic.Visible           = false;
            lblIsMemberDiabetesAustralia.Visible = false;
            lblPrivateHealthFund.Visible    = false;
            lblConcessionCardNbr.Visible    = false;
            lblConcessionCardExpiry.Visible = false;
            lblACInvOffering.Visible        = false;
            lblLogin.Visible                = false;
            lblPwd.Visible                  = false;
            lblNextOfKinName.Visible        = false;
            lblNextOfKinRelation.Visible    = false;
            lblNextOfKinContactInfo.Visible = false;
        }
        else
        {
            int age = -1;
            if (patient.Person.Dob != DateTime.MinValue)
            {
                DateTime now = DateTime.Today;
                age = now.Year - patient.Person.Dob.Year;
                if (now.Month < patient.Person.Dob.Month || (now.Month == patient.Person.Dob.Month && now.Day < patient.Person.Dob.Day))
                    age--;
            }

            lblTitle.Text             = patient.Person.Title.ID          == 0 ? "--" : patient.Person.Title.Descr;
            lblFirstname.Text         = patient.Person.Firstname.Length  == 0 ? "--" : patient.Person.Firstname;
            lblMiddlename.Text        = patient.Person.Middlename.Length == 0 ? "--" : patient.Person.Middlename;
            lblSurname.Text           = patient.Person.Surname.Length    == 0 ? "--" : patient.Person.Surname;
            lblNickname.Text          = patient.Person.Nickname.Length   == 0 ? "--" : patient.Person.Nickname;
            lblGender.Text            = GetGenderText(patient.Person.Gender);
            lblDOB.Text               = patient.Person.Dob                     == DateTime.MinValue ? "--" : patient.Person.Dob.ToString("dd-MMM-yyyy") + " (Age " + age + ")";
            lblDARev.Text             = patient.DiabeticAAassessmentReviewDate == DateTime.MinValue ? "--" : patient.DiabeticAAassessmentReviewDate.ToString("dd-MMM-yyyy");
            lblIsClinicPatient.Text   = patient.IsClinicPatient ? "Yes" : "No";
            lblIsDeceased.Text        = patient.IsDeceased      ? "Yes" : "No";
            lblIsDiabetic.Text        = patient.IsDiabetic      ? "YES" : "No";
            lblIsMemberDiabetesAustralia.Text = patient.IsMemberDiabetesAustralia ? "Yes" : "No";
            lblPrivateHealthFund.Text = patient.PrivateHealthFund.Length == 0 ? "--" : patient.PrivateHealthFund;
            lblConcessionCardNbr.Text    = patient.ConcessionCardNumber.Length == 0 ? "--" : patient.ConcessionCardNumber;
            lblConcessionCardExpiry.Text = patient.ConcessionCardExpiryDate     == DateTime.MinValue ? "--" : patient.ConcessionCardExpiryDate.ToString("MM  '/'  yyyy");
            lblAddedBy.Font.Bold          = true;
            lblPatientDateAdded.Font.Bold = true;

            if (patient.IsDiabetic)
            {
                lblIsDiabetic.Font.Bold = true;
                lblIsDiabeticText.Font.Bold = true;
                lblIsDiabetic.ForeColor = System.Drawing.Color.Red;
                lblIsDiabeticText.ForeColor = System.Drawing.Color.Red;
            }


            if (patient.ACInvOffering == null)
                lblACInvOffering.Text = string.Empty;
            else if ((new List<int> { 2, 3, 4, 5 }).Contains(patient.ACInvOffering.AgedCarePatientType.ID))
                lblACInvOffering.Text = patient.ACInvOffering.Name;
            else if ((new List<int> { 6, 7, 8, 9, 10 }).Contains(patient.ACInvOffering.AgedCarePatientType.ID))
                lblACInvOffering.Text = patient.ACInvOffering.Name + " (" + patient.ACPatOffering.Name + ")";
            else // (?)
                lblACInvOffering.Text = patient.ACInvOffering.Name;

            lblLogin.Text = patient.Login.Length == 0 ? "--" : patient.Login;
            lblPwd.Text   = patient.Pwd.Length   == 0 ? "--" : patient.Pwd;

            lblNextOfKinName.Text        = patient.NextOfKinName.Length        == 0 ? "--" : patient.NextOfKinName;
            lblNextOfKinRelation.Text    = patient.NextOfKinRelation.Length    == 0 ? "--" : patient.NextOfKinRelation;
            lblNextOfKinContactInfo.Text = patient.NextOfKinContactInfo.Length == 0 ? "--" : patient.NextOfKinContactInfo.Replace(Environment.NewLine, "<br />");

            ddlTitle.Visible             = false;
            txtFirstname.Visible         = false;
            txtMiddlename.Visible        = false;
            txtSurname.Visible           = false;
            txtNickname.Visible          = false;
            ddlGender.Visible            = false;
            ddlDOB_Day.Visible           = false;
            ddlDOB_Month.Visible         = false;
            ddlDOB_Year.Visible          = false;
            ddlDARev_Day.Visible         = false;
            ddlDARev_Month.Visible       = false;
            ddlDARev_Year.Visible        = false;
            chkIsClinicPatient.Visible   = false;
            chkIsDeceased.Visible        = false;
            chkIsDiabetic.Visible        = false;
            chkIsMemberDiabetesAustralia.Visible  = false;
            txtPrivateHealthFund.Visible = false;
            txtConcessionCardNbr.Visible = false;
            ddlConcessionCardExpiry_Month.Visible = false;
            ddlConcessionCardExpiry_Year.Visible  = false;
            ddlACInvOffering.Visible     = false;
            txtLogin.Visible             = false;
            txtPwd.Visible               = false;
            txtNextOfKinName.Visible     = false;
            txtNextOfKinRelation.Visible = false;
            txtNextOfKinContactInfo.Visible = false;
        }


        //this.lnkClinics.Visible = true;
        //this.lnkClinics.NavigateUrl = "~/RegisterOrganisationsToPatientV2.aspx?id=" + GetFormID().ToString() + "&type=edit";
        //this.lnkClinics.Text = "Add / Remove";

        //DataTable incList = RegisterPatientDB.GetDataTable_OrganisationsOf(patient.PatientID);
        //incList.DefaultView.Sort = "name ASC";
        //lstOrgs.DataSource = incList;
        //lstOrgs.DataBind();

        FillOrganisationRegistrationGrid();

        SetBookingsList(patient);
        SetNotesList(patient);
        SetMedNotesList(patient);
        SetMedCondNotesList(patient);
        GrdReferrals_FillGrid();
        GrdHealthCards_FillGrid();


        btnSubmit.Text = isEditMode ? "Update Details" : "Edit Details";

        btnSubmit.Visible = true;
        btnCancel.Visible = isEditMode;


        btnDeleteUndeletePatient.Visible = userView.IsAdminView;
        if (patient.IsDeleted && !userView.IsAdminView)
        {
            btnSubmit.Visible = false;
            btnCancel.Visible = false;
        }
    }
    protected void lstLetterHistory_RowCommand(object sender, RepeaterCommandEventArgs e)
    {
        if (e.CommandName.Equals("RetrieveLetterDB"))
        {
            int letter_print_hisotory_id = Int32.Parse((string)e.CommandArgument);
            LetterFile letterFile = LetterPrintHistoryDB.GetLetterFile(letter_print_hisotory_id);
            if (letterFile == null)
                throw new CustomMessageException("No file with selected ID");

            Letter.DownloadDocument(Response, letterFile.Contents, letterFile.Name);
        }
        if (e.CommandName.Equals("RetrieveLetterFlatFile"))
        {
            int letter_print_hisotory_id = Int32.Parse((string)e.CommandArgument);
            LetterPrintHistory letterPrintHistory = LetterPrintHistoryDB.GetByID(letter_print_hisotory_id);

            string historyDir = Letter.GetLettersHistoryDirectory(letterPrintHistory.Organisation.OrganisationID);

            string filePath = historyDir + letterPrintHistory.LetterPrintHistoryID + System.IO.Path.GetExtension(letterPrintHistory.DocName);
            if (!System.IO.File.Exists(filePath))
                throw new CustomMessageException("No file with selected ID");

            byte[] fileContents = System.IO.File.ReadAllBytes(filePath);
            Letter.DownloadDocument(Response, fileContents, letterPrintHistory.DocName);
        }
    }
    protected string GetGenderText(string originalText)
    {
        if (originalText.ToUpper() == "M")
            return "Male";
        else if (originalText.ToUpper() == "F")
            return "Female";
        else
            return "--";
    }
    protected void SetNotEditable(System.Web.UI.WebControls.WebControl c)
    {
        c.Enabled = false;
        c.BorderStyle = BorderStyle.None;
        c.BackColor = System.Drawing.Color.Transparent;
        c.ForeColor = System.Drawing.Color.Black;
    }




    protected void DOBAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue);
    }
    protected void DARevAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidDate(ddlDARev_Day.SelectedValue, ddlDARev_Month.SelectedValue, ddlDARev_Year.SelectedValue);
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (!ddlDOBValidateAllOrNoneSet.IsValid)
            return;
        if (!ddlDARevValidateAllOrNoneSet.IsValid)
            return;

        if (GetUrlParamType() == UrlParamType.View)
        {
            Response.Redirect( UrlParamModifier.AddEdit(Request.RawUrl, "type", "edit") );
        }
        else if (GetUrlParamType() == UrlParamType.Edit)
        {
            if ((ddlConcessionCardExpiry_Year.SelectedValue  != "-1" && ddlConcessionCardExpiry_Month.SelectedValue == "-1") ||
                (ddlConcessionCardExpiry_Month.SelectedValue != "-1" && ddlConcessionCardExpiry_Year.SelectedValue  == "-1"))
            {
                SetErrorMessage("Concession Card Expiry Must Be Both Set or Both Unset.");
                return;
            }


            DateTime concessionCardExpiryDate = ddlConcessionCardExpiry_Year.SelectedValue == "-1" || ddlConcessionCardExpiry_Month.SelectedValue == "-1" ?
                                                DateTime.MinValue :
                                                new DateTime(Convert.ToInt32(ddlConcessionCardExpiry_Year.SelectedValue), Convert.ToInt32(ddlConcessionCardExpiry_Month.SelectedValue), 1);

            Patient patient = PatientDB.GetByID(Convert.ToInt32(this.lblId.Text));


            txtLogin.Text = txtLogin.Text.Trim();
            txtPwd.Text   = txtPwd.Text.Trim();
            if (txtLogin.Text.Length > 0)
            {
                if (txtPwd.Text.Length == 0)
                {
                    lblErrorMessage.Text = "Password is required when username entered";
                    lblErrorMessage.Visible = true;
                    return;
                }

                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && patient.Login != txtLogin.Text && UserDatabaseMapperDB.UsernameExists(txtLogin.Text))
                {
                    lblErrorMessage.Text = "Login name already in use by another user";
                    lblErrorMessage.Visible = true;
                    return;
                }
                if (PatientDB.LoginExists(txtLogin.Text, patient.PatientID))
                {
                    lblErrorMessage.Text = "Login name already in use by another user";
                    lblErrorMessage.Visible = true;
                    return;
                }
            }


            Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
            PatientDB.AddACOfferings(ref offeringsHash, ref patient);


            int acInvOfferingID_New = patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID;
            int acPatOfferingID_New = patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID;

            if (patient.ACInvOffering == null || patient.ACPatOffering == null)
            {
                acInvOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
                acPatOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
            }
            else if (UserView.GetInstance().IsAgedCareView && patient.ACInvOffering.OfferingID != Convert.ToInt32(ddlACInvOffering.SelectedValue))
            {
                int acInvOfferingID_Old = patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID;
                int acPatOfferingID_Old = patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID;

                acInvOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
                acPatOfferingID_New = acPatOfferingID_Old;

                int acInvAcPtTypeID_New = ((Offering)offeringsHash[Convert.ToInt32(ddlACInvOffering.SelectedValue)]).AgedCarePatientType.ID;
                int acInvAcPtTypeID_Old = patient.ACInvOffering.AgedCarePatientType.ID;

	            //when updating:
	            //- if changing to LC/HC/LCF/HCUF    - change BOTH to that (to make sure second is always clearly the pt type)
	            //- if changing to MC/DVA/TAC/LCE/HCE
                //  - if prev_first is LC/HC/LCF/HCUF     - move prev_first to second, and set first as selected
                //  - if prev_first is MC/DVA/TAC/LCE/HCE - set first as selected (and leave second)

                if ((new List<int> { 2, 3, 4, 5 }).Contains(acInvAcPtTypeID_New))
                {
                    acPatOfferingID_New = acInvOfferingID_New;
                }
                else if ((new List<int> { 6, 7, 8, 9, 10 }).Contains(acInvAcPtTypeID_New))
                {
                    if ((new List<int> { 2, 3, 4, 5 }).Contains(acInvAcPtTypeID_Old))
                        acPatOfferingID_New = acInvOfferingID_Old;
                }
                else // (?)
                    ; //
            }

            PatientHistoryDB.Insert(patient.PatientID, patient.IsClinicPatient, patient.IsGPPatient, patient.IsDeleted, patient.IsDeceased,
                                    patient.FlashingText, patient.FlashingTextAddedBy == null ? -1 : patient.FlashingTextAddedBy.StaffID, patient.FlashingTextLastModifiedDate,
                                    patient.PrivateHealthFund, patient.ConcessionCardNumber, patient.ConcessionCardExpiryDate, patient.IsDiabetic, patient.IsMemberDiabetesAustralia, patient.DiabeticAAassessmentReviewDate, patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID, patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID, patient.Login, patient.Pwd, patient.IsCompany, patient.ABN, patient.NextOfKinName, patient.NextOfKinRelation, patient.NextOfKinContactInfo, 
                                    patient.Person.Title.ID, patient.Person.Firstname, patient.Person.Middlename, patient.Person.Surname, patient.Person.Nickname, patient.Person.Gender, patient.Person.Dob, Convert.ToInt32(Session["StaffID"]));

            PersonDB.Update(patient.Person.PersonID, Convert.ToInt32(ddlTitle.SelectedValue), Utilities.FormatName(txtFirstname.Text), Utilities.FormatName(txtMiddlename.Text), Utilities.FormatName(txtSurname.Text), txtNickname.Text, ddlGender.SelectedValue, GetDOBFromForm(), DateTime.Now);
            PatientDB.Update(patient.PatientID, patient.Person.PersonID, patient.IsClinicPatient, patient.IsGPPatient, chkIsDeceased.Checked,
                patient.FlashingText, patient.FlashingTextAddedBy == null ? -1 : patient.FlashingTextAddedBy.StaffID, patient.FlashingTextLastModifiedDate,
                txtPrivateHealthFund.Text, txtConcessionCardNbr.Text, concessionCardExpiryDate, chkIsDiabetic.Checked, chkIsMemberDiabetesAustralia.Checked, GetDARevFromForm(), acInvOfferingID_New, acPatOfferingID_New, txtLogin.Text, txtLogin.Text.Length == 0 ? "" : txtPwd.Text, patient.IsCompany, patient.ABN, txtNextOfKinName.Text.Trim(), txtNextOfKinRelation.Text.Trim(), txtNextOfKinContactInfo.Text.Trim());

            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && patient.Login != txtLogin.Text)
            {
                UserDatabaseMapper curDBMapper = UserDatabaseMapperDB.GetByLogin(patient.Login, Session["DB"].ToString());
                if (txtLogin.Text.Length == 0) // delete from db
                {
                    if (curDBMapper != null)
                        UserDatabaseMapperDB.Delete(curDBMapper.ID);
                }
                else
                {
                    if (curDBMapper != null)
                        UserDatabaseMapperDB.Update(curDBMapper.ID, txtLogin.Text, Session["DB"].ToString());
                    else
                        UserDatabaseMapperDB.Insert(txtLogin.Text, Session["DB"].ToString());
                }
            }


            UpdateConditions(patient);


            Response.Redirect( UrlParamModifier.AddEdit(Request.RawUrl, "type", "view") );
        }
        else
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
        }
    }



    public DateTime GetDOBFromForm()
    {
        return GetDate(ddlDOB_Day.SelectedValue, ddlDOB_Month.SelectedValue, ddlDOB_Year.SelectedValue, "DOB");
    }
    public DateTime GetDARevFromForm()
    {
        return GetDate(ddlDARev_Day.SelectedValue, ddlDARev_Month.SelectedValue, ddlDARev_Year.SelectedValue, "DARev");
    }
    public DateTime GetDate(string day, string month, string year, string fieldNme)
    {
        if (day == "-1" && month == "-1" && year == "-1")
            return DateTime.MinValue;

        else if (day != "-1" && month != "-1" && year != "-1")
            return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));

        else
            throw new Exception(fieldNme + " format is some selected and some not selected.");
    }
    public bool IsValidDate(string day, string month, string year)
    {
        bool invalid = ((day == "-1" || month == "-1" || year == "-1") && (day != "-1" || month != "-1" || year != "-1"));
        return !invalid;
    }

    #region GrdCondition

    protected void FillGrdCondition(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        DataTable dt = ConditionDB.GetDataTable(false);
        Hashtable conditionHash = ConditionDB.GetHashtable(false);

        Session["patientcondition_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdCondition.DataSource = dt;
            try
            {
                GrdCondition.DataBind();

                Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);
                foreach (GridViewRow row in GrdCondition.Rows)
                {
                    Label lblId = row.FindControl("lblId") as Label;
                    CheckBox chkSelect = row.FindControl("chkSelect") as CheckBox;
                    DropDownList ddlDate_Day       = (DropDownList)row.FindControl("ddlDate_Day");
                    DropDownList ddlDate_Month     = (DropDownList)row.FindControl("ddlDate_Month");
                    DropDownList ddlDate_Year      = (DropDownList)row.FindControl("ddlDate_Year");
                    DropDownList ddlNbrWeeksDue    = (DropDownList)row.FindControl("ddlNbrWeeksDue");

                    Label        lblNextDue        = row.FindControl("lblNextDue") as Label;
                    Label        lblWeeksLater     = row.FindControl("lblWeeksLater") as Label;
                    Label        lblAdditionalInfo = row.FindControl("lblAdditionalInfo") as Label;
                    

                    System.Web.UI.HtmlControls.HtmlControl br_date      = (System.Web.UI.HtmlControls.HtmlControl)row.FindControl("br_date");
                    System.Web.UI.HtmlControls.HtmlControl br_nweeksdue = (System.Web.UI.HtmlControls.HtmlControl)row.FindControl("br_nweeksdue");
                    System.Web.UI.HtmlControls.HtmlControl br_text      = (System.Web.UI.HtmlControls.HtmlControl)row.FindControl("br_text");

                    TextBox txtText = (TextBox)row.FindControl("txtText");

                    if (lblId == null || chkSelect == null)
                        continue;

                    Condition condition = (Condition)conditionHash[Convert.ToInt32(lblId.Text)];

                    br_date.Visible             = condition.ShowDate;
                    ddlDate_Day.Visible         = condition.ShowDate;
                    ddlDate_Month.Visible       = condition.ShowDate;
                    ddlDate_Year.Visible        = condition.ShowDate;
                    br_nweeksdue.Visible        = condition.ShowNWeeksDue;
                    ddlNbrWeeksDue.Visible      = condition.ShowNWeeksDue;
                    lblNextDue.Visible          = condition.ShowNWeeksDue;
                    lblWeeksLater.Visible       = condition.ShowNWeeksDue;
                    br_text.Visible             = condition.ShowText;
                    txtText.Visible             = condition.ShowText;
                    lblAdditionalInfo.Visible   = condition.ShowText;


                    if (selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                    {
                        PatientCondition ptCondition = (PatientCondition)selectedConditions[Convert.ToInt32(lblId.Text)];

                        chkSelect.Checked = selectedConditions[Convert.ToInt32(lblId.Text)] != null;

                        if (condition.ShowDate)
                        {
                            if (ptCondition.Date != DateTime.MinValue)
                            {
                                ddlDate_Day.SelectedValue   = ptCondition.Date.Day.ToString();
                                ddlDate_Month.SelectedValue = ptCondition.Date.Month.ToString();
                                ddlDate_Year.SelectedValue  = ptCondition.Date.Year.ToString();
                            }
                        }
                        if (condition.ShowNWeeksDue)
                        {
                            ddlNbrWeeksDue.SelectedValue = ptCondition.NWeeksDue.ToString();
                        }
                        if (condition.ShowText)
                        {
                            txtText.Text = ptCondition.Text;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }

            //Sort("parent_descr", "ASC");
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdCondition.DataSource = dt;
            GrdCondition.DataBind();

            int TotalColumns = GrdCondition.Rows[0].Cells.Count;
            GrdCondition.Rows[0].Cells.Clear();
            GrdCondition.Rows[0].Cells.Add(new TableCell());
            GrdCondition.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdCondition.Rows[0].Cells[0].Text = "No Conditions";
        }
    }
    protected void GrdCondition_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdCondition_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["patientcondition_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("condition_condition_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlDate_Day    = (DropDownList)e.Row.FindControl("ddlDate_Day");
            DropDownList ddlDate_Month  = (DropDownList)e.Row.FindControl("ddlDate_Month");
            DropDownList ddlDate_Year   = (DropDownList)e.Row.FindControl("ddlDate_Year");
            DropDownList ddlNbrWeeksDue = (DropDownList)e.Row.FindControl("ddlNbrWeeksDue");

            ddlDate_Day.Items.Add(new ListItem("--", "-1"));
            ddlDate_Month.Items.Add(new ListItem("--", "-1"));
            ddlDate_Year.Items.Add(new ListItem("--", "-1"));
            for (int i = 1; i <= 31; i++)
                ddlDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 1; i <= 12; i++)
                ddlDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                ddlDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

            for (int i = 0; i <= 52; i++)
                ddlNbrWeeksDue.Items.Add(new ListItem(i.ToString(), i.ToString()));
            


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["patientcondition_is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName   = "_UnDelete";
                    btnDelete.ImageUrl      = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip       = "UnDelete";
                }
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdCondition_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdCondition.EditIndex = -1;
        FillGrdCondition();
    }
    protected void GrdCondition_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdCondition.Rows[e.RowIndex].FindControl("lblId");

        //Condition condition = ConditionDB.GetByID(Convert.ToInt32(lblId.Text));
        //ConditionDB.Update(Convert.ToInt32(lblId.Text), txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), condition.IsDeleted);

        GrdCondition.EditIndex = -1;
        FillGrdCondition();
    }
    protected void GrdCondition_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdCondition.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //CostCentreDB.Delete(Convert.ToInt32(lblId.Text));
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrdCondition();
    }
    protected void GrdCondition_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {

            //ConditionDB.Insert(txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), false);

            FillGrdCondition();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    PatientConditionDB.UpdateInactive(id);
                else
                    PatientConditionDB.UpdateActive(id);
            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                if (Utilities.IsDev())
                    SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
                else
                    SetErrorMessage("Can not delete because other records depend on this");
            }

            FillGrdCondition();
        }

    }
    protected void GrdCondition_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdCondition.EditIndex = e.NewEditIndex;
        FillGrdCondition();
    }
    protected void GrdCondition_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdCondition.EditIndex >= 0)
            return;

        GrdCondition_Sort(e.SortExpression);
    }

    protected void GrdCondition_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["patientcondition_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["patientcondition_sortexpression"] == null)
                Session["patientcondition_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["patientcondition_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["patientcondition_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdCondition.DataSource = dataView;
            GrdCondition.DataBind();
        }
    }

    #endregion

    #region GrdConditionView

    protected void FillGrdConditionView(Patient patient = null)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        DataTable dt = ConditionDB.GetDataTable(false);
        Hashtable conditionHash = ConditionDB.GetHashtable(false);

        Session["patientconditionview_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdConditionView.DataSource = dt;
            try
            {
                GrdConditionView.DataBind();

                Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);

                int nRows = 0;
                for(int i = GrdConditionView.Rows.Count-1 ; i>= 0; i--)
                {
                    Label lblId             = GrdConditionView.Rows[i].FindControl("lblId") as Label;

                    Label lblDate           = GrdConditionView.Rows[i].FindControl("lblDate") as Label;

                    Label lblNextDue        = GrdConditionView.Rows[i].FindControl("lblNextDue") as Label;
                    Label lblDateDue        = GrdConditionView.Rows[i].FindControl("lblDateDue") as Label;

                    Label lblAdditionalInfo = GrdConditionView.Rows[i].FindControl("lblAdditionalInfo") as Label;
                    Label lblText           = GrdConditionView.Rows[i].FindControl("lblText") as Label;
                    
                    System.Web.UI.HtmlControls.HtmlControl br_date      = (System.Web.UI.HtmlControls.HtmlControl)GrdConditionView.Rows[i].FindControl("br_date");
                    System.Web.UI.HtmlControls.HtmlControl br_nweeksdue = (System.Web.UI.HtmlControls.HtmlControl)GrdConditionView.Rows[i].FindControl("br_nweeksdue");
                    System.Web.UI.HtmlControls.HtmlControl br_text      = (System.Web.UI.HtmlControls.HtmlControl)GrdConditionView.Rows[i].FindControl("br_text");


                    if (lblId == null)
                        continue;

                    Condition condition = (Condition)conditionHash[Convert.ToInt32(lblId.Text)];

                    br_date.Visible             = condition.ShowDate;
                    lblDate.Visible             = condition.ShowDate;
                    br_nweeksdue.Visible        = condition.ShowNWeeksDue;
                    lblNextDue.Visible          = condition.ShowNWeeksDue;
                    lblDateDue.Visible          = condition.ShowNWeeksDue;
                    br_text.Visible             = condition.ShowText;
                    lblText.Visible             = condition.ShowText;
                    lblAdditionalInfo.Visible   = condition.ShowText;


                    if (selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                    {
                        PatientCondition ptCondition = (PatientCondition)selectedConditions[Convert.ToInt32(lblId.Text)];

                        if (condition.ShowDate)
                        {
                            lblDate.Text = ptCondition.Date == DateTime.MinValue ? "[Date Not Set]" : ptCondition.Date.ToString("d MMM, yyyy");
                        }
                        if (condition.ShowNWeeksDue)
                        {
                            bool expired = ptCondition.Date != DateTime.MinValue && ptCondition.Date.AddDays(7 * ptCondition.NWeeksDue) < DateTime.Today;
                            lblDateDue.Text = ptCondition.Date == DateTime.MinValue ? "[Date Not Set]" : (expired ? "<font color=\"red\">" : "") + ptCondition.Date.AddDays(7 * ptCondition.NWeeksDue).ToString("d MMM, yyyy") + (expired ? "</font>" : "");
                        }
                        if (condition.ShowText)
                        {
                            lblText.Text = ptCondition.Text.Length == 0 ? "[Blank]" : ptCondition.Text;
                        }

                        nRows++;
                    }
                    else
                    {
                        GrdConditionView.Rows[i].Visible = false;
                    }

                }

                if (nRows == 0)
                {
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        dt.Rows.RemoveAt(i);

                    dt.Rows.Add(dt.NewRow());
                    GrdConditionView.DataSource = dt;
                    GrdConditionView.DataBind();

                    int TotalColumns = GrdConditionView.Rows[0].Cells.Count;
                    GrdConditionView.Rows[0].Cells.Clear();
                    GrdConditionView.Rows[0].Cells.Add(new TableCell());
                    GrdConditionView.Rows[0].Cells[0].ColumnSpan = TotalColumns;
                    GrdConditionView.Rows[0].Cells[0].Text = "PT has no conditions";

                }

            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }

            //Sort("parent_descr", "ASC");
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdConditionView.DataSource = dt;
            GrdConditionView.DataBind();

            int TotalColumns = GrdConditionView.Rows[0].Cells.Count;
            GrdConditionView.Rows[0].Cells.Clear();
            GrdConditionView.Rows[0].Cells.Add(new TableCell());
            GrdConditionView.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdConditionView.Rows[0].Cells[0].Text = "PT has no conditions";
        }
    }
    protected void GrdConditionView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdConditionView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["patientconditionview_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("condition_condition_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["patientconditionview_is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName   = "_UnDelete";
                    btnDelete.ImageUrl      = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip       = "UnDelete";
                }
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdConditionView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdConditionView.EditIndex = -1;
        FillGrdConditionView();
    }
    protected void GrdConditionView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdConditionView.Rows[e.RowIndex].FindControl("lblId");

        //Condition condition = ConditionDB.GetByID(Convert.ToInt32(lblId.Text));
        //ConditionDB.Update(Convert.ToInt32(lblId.Text), txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), condition.IsDeleted);

        GrdConditionView.EditIndex = -1;
        FillGrdConditionView();
    }
    protected void GrdConditionView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdConditionView.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //CostCentreDB.Delete(Convert.ToInt32(lblId.Text));
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrdConditionView();
    }
    protected void GrdConditionView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {

            //ConditionDB.Insert(txtDescr.Text, Convert.ToInt32(ddlDisplayOrder.SelectedValue), false);

            FillGrdConditionView();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    PatientConditionDB.UpdateInactive(id);
                else
                    PatientConditionDB.UpdateActive(id);
            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                if (Utilities.IsDev())
                    SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
                else
                    SetErrorMessage("Can not delete because other records depend on this");
            }

            FillGrdConditionView();
        }

    }
    protected void GrdConditionView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdConditionView.EditIndex = e.NewEditIndex;
        FillGrdConditionView();
    }
    protected void GrdConditionView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdConditionView.EditIndex >= 0)
            return;

        GrdConditionView_Sort(e.SortExpression);
    }

    protected void GrdConditionView_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["patientconditionview_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["patientconditionview_sortexpression"] == null)
                Session["patientconditionview_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["patientconditionview_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["patientconditionview_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdConditionView.DataSource = dataView;
            GrdConditionView.DataBind();
        }
    }

    #endregion

    #region UpdateConditions

    protected void UpdateConditions(Patient patient)
    {
        if (patient == null)
            patient = PatientDB.GetByID(GetFormID());

        Hashtable selectedConditions = PatientConditionDB.GetHashtable_ByPatientID(patient.PatientID, false);
        foreach (GridViewRow row in GrdCondition.Rows)
        {
            Label        lblId          = row.FindControl("lblId") as Label;
            CheckBox     chkSelect      = row.FindControl("chkSelect") as CheckBox;
            DropDownList ddlDate_Day    = (DropDownList)row.FindControl("ddlDate_Day");
            DropDownList ddlDate_Month  = (DropDownList)row.FindControl("ddlDate_Month");
            DropDownList ddlDate_Year   = (DropDownList)row.FindControl("ddlDate_Year");
            DropDownList ddlNbrWeeksDue = (DropDownList)row.FindControl("ddlNbrWeeksDue");
            TextBox      txtText        = (TextBox)row.FindControl("txtText");


            if (lblId == null || chkSelect == null)
                continue;


            DateTime date = DateTime.MinValue;
            if (ddlDate_Day.Visible && ddlDate_Month.Visible && ddlDate_Year.Visible)
            {
                if (!IsValidDate(Convert.ToInt32(ddlDate_Day.Text), Convert.ToInt32(ddlDate_Month.Text), Convert.ToInt32(ddlDate_Year.Text)))
                {
                    SetErrorMessage("Please enter a valid date or unset all");
                    return;
                }
                else
                    date = GetDateFromForm(Convert.ToInt32(ddlDate_Day.Text), Convert.ToInt32(ddlDate_Month.Text), Convert.ToInt32(ddlDate_Year.Text));
            }

            int nweeksdue = ddlNbrWeeksDue.Visible ? Convert.ToInt32(ddlNbrWeeksDue.Text) : 0;
            string text = txtText.Visible ? txtText.Text.Trim() : string.Empty;

            if (chkSelect.Checked && selectedConditions[Convert.ToInt32(lblId.Text)] == null)
                PatientConditionDB.Insert(patient.PatientID, Convert.ToInt32(lblId.Text), date, nweeksdue, text, false);

            if (!chkSelect.Checked && selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                PatientConditionDB.Delete(patient.PatientID, Convert.ToInt32(lblId.Text));

            if (chkSelect.Checked && selectedConditions[Convert.ToInt32(lblId.Text)] != null)
                PatientConditionDB.Update(patient.PatientID, Convert.ToInt32(lblId.Text), date, nweeksdue, text, false);
        }

        FillGrdCondition(patient);
        FillGrdConditionView(patient);

        SetErrorMessage("Updated");
    }

    public DateTime GetDateFromForm(int day, int month, int year)
    {
        if (day == -1 && month == -1 && year == -1)
            return DateTime.MinValue;

        else if (day != -1 && month != -1 && year != -1)
            return new DateTime(year, month, day);

        else
            throw new Exception("Date format is some selected and some not selected.");
    }
    public bool IsValidDate(int day, int month, int year)
    {
        bool isvalid = ((day == -1 && month == -1 && year == -1) ||
                        (day != -1 && month != -1 && year != -1 && _IsValidDate(day, month, year)));
        return isvalid;
    }
    private bool _IsValidDate(int day, int month, int year)
    {
        try
        {
            DateTime dt = new DateTime(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region GrdRegistration

    protected void GrdRegistration_Reset()
    {
        DataTable dt = Session["addeditpatient_referrals_data"] as DataTable;
        if (dt == null)
            return;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (tblEmpty)
        {
            int TotalColumns = GrdRegistration.Rows[0].Cells.Count;
            GrdRegistration.Rows[0].Cells.Clear();
            GrdRegistration.Rows[0].Cells.Add(new TableCell());
            GrdRegistration.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdRegistration.Rows[0].Cells[0].Text = "No Organisations Allocated Yet";
        }
    }

    protected void FillOrganisationRegistrationGrid()
    {
        if (!IsValidFormID())
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage(Utilities.IsDev() ? "No patient exists with this ID" : "");
            return;
        }


        DataTable dt = RegisterPatientDB.GetDataTable_OrganisationsOf(patient.PatientID);
        dt.DefaultView.Sort = "register_patient_id DESC"; // sort on most recently added org
        dt = dt.DefaultView.ToTable();

        Session["addeditpatient_referrals_data"] = dt;


        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["sortExpression"] != null && Session["sortExpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["sortExpression"].ToString();
                GrdRegistration.DataSource = dataView;
            }
            else
            {
                GrdRegistration.DataSource = dt;
            }


            try
            {
                GrdRegistration.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdRegistration.DataSource = dt;
            GrdRegistration.DataBind();

            int TotalColumns = GrdRegistration.Rows[0].Cells.Count;
            GrdRegistration.Rows[0].Cells.Clear();
            GrdRegistration.Rows[0].Cells.Add(new TableCell());
            GrdRegistration.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdRegistration.Rows[0].Cells[0].Text = "No Organisations Allocated Yet";
        }

        SetOrgDdlList();
    }
    protected void GrdRegistration_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Pager)
        {
            //if (!Utilities.IsDev())
                e.Row.Cells[0].CssClass = "hiddencol";
        }
    }
    protected void GrdRegistration_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        UserView userView = UserView.GetInstance();

        DataTable dt = Session["addeditpatient_referrals_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("register_patient_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            DropDownList ddlOrganisation = (DropDownList)e.Row.FindControl("ddlOrganisation");
            if (ddlOrganisation != null)
            {
                Organisation[] incList_orig = RegisterPatientDB.GetOrganisationsOf(patient.PatientID);
                Organisation[] incList = Organisation.RemoveByID(incList_orig, Convert.ToInt32(thisRow["organisation_id"]));
                DataTable orgs = OrganisationDB.GetDataTable_AllNotInc(incList, true, !userView.IsClinicView, !userView.IsAgedCareView, true, true);
                orgs.DefaultView.Sort = "name ASC";
                foreach (DataRowView row in orgs.DefaultView)
                    if (row["name"].ToString().Trim().Length > 0)
                        ddlOrganisation.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
                ddlOrganisation.SelectedValue = thisRow["organisation_id"].ToString();
            }

            HyperLink lnkBookings = (HyperLink)e.Row.FindControl("lnkBookings");
            if (lnkBookings != null)
            {
                lnkBookings.NavigateUrl = string.Format("~/BookingsV2.aspx?orgs={0}&patient={1}", Convert.ToInt32(thisRow["organisation_id"]), patient.PatientID);

                lnkBookings.Visible = (userView .IsGPView      && Convert.ToInt32(thisRow["organisation_type_id"]) == 218) || 
                                      (userView.IsClinicView   && Convert.ToInt32(thisRow["organisation_type_id"]) == 218) || 
                                      (userView.IsAgedCareView && (new List<int> { 139, 367, 372 }).Contains(Convert.ToInt32(thisRow["organisation_type_id"])));
            }


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlOrganisation = (DropDownList)e.Row.FindControl("ddlNewOrganisation");
            if (ddlOrganisation != null)
            {
                Organisation[] incList = RegisterPatientDB.GetOrganisationsOf(patient.PatientID);
                DataTable orgs = OrganisationDB.GetDataTable_AllNotInc(incList, true, !userView.IsClinicView, !userView.IsAgedCareView, true, true);
                orgs.DefaultView.Sort = "name ASC";

                foreach (DataRowView row in orgs.DefaultView)
                    ddlOrganisation.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
            }
        }
    }
    protected void GrdRegistration_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdRegistration.EditIndex = -1;
        FillOrganisationRegistrationGrid();
    }
    protected void GrdRegistration_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlOrganisation = (DropDownList)GrdRegistration.Rows[e.RowIndex].FindControl("ddlOrganisation");

        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        RegisterPatientDB.Update(Convert.ToInt32(lblId.Text), Convert.ToInt32(ddlOrganisation.SelectedValue), patient.PatientID);

        GrdRegistration.EditIndex = -1;
        FillOrganisationRegistrationGrid();
    }
    protected void GrdRegistration_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdRegistration.Rows[e.RowIndex].FindControl("lblId");

        RegisterPatient registerPatient = RegisterPatientDB.GetByID(Convert.ToInt32(lblId.Text));
        if (BookingDB.GetCountByPatientAndOrg(registerPatient.Patient.PatientID, registerPatient.Organisation.OrganisationID) > 0)
        {
            SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' because there exists a booking for this patient there.");
            return;
        }

        if (registerPatient.Organisation.IsAgedCare)
        {
            int agedCareOrgRegistrations = RegisterPatientDB.GetCountByPatientAndOrgTypeGroup(registerPatient.Patient.PatientID, "6");
            if (agedCareOrgRegistrations < 2)
            {
                SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' until another Fac/Wing/Unit has been added.");
                return;
            }
        }


        try
        {
            RegisterPatientDB.UpdateInactive(Convert.ToInt32(lblId.Text), false);
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                HideTableAndSetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                HideTableAndSetErrorMessage("Can not delete because other records depend on this");
        }

        FillOrganisationRegistrationGrid();
    }
    protected void GrdRegistration_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlOrganisation = (DropDownList)GrdRegistration.FooterRow.FindControl("ddlNewOrganisation");

            Patient patient = PatientDB.GetByID(GetFormID());
            if (patient == null)
            {
                HideTableAndSetErrorMessage("");
                return;
            }

            try
            {
                RegisterPatientDB.Insert(Convert.ToInt32(ddlOrganisation.SelectedValue), patient.PatientID);
            }
            catch (UniqueConstraintException)
            {
                // happens when 2 forms allow adding
                // do nothing and let form re-update
            }

            FillOrganisationRegistrationGrid();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int resiter_patient_id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                {
                    RegisterPatient registerPatient = RegisterPatientDB.GetByID(resiter_patient_id);
                    if (BookingDB.GetCountByPatientAndOrg(registerPatient.Patient.PatientID, registerPatient.Organisation.OrganisationID) > 0)
                    {
                        SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' because there exists a booking for this patient there.");
                        return;
                    }


                    if (registerPatient.Organisation.IsAgedCare)
                    {
                        int agedCareOrgRegistrations = RegisterPatientDB.GetCountByPatientAndOrgTypeGroup(registerPatient.Patient.PatientID, "6");
                        if (agedCareOrgRegistrations < 2)
                        {
                            SetErrorMessage("Can not remove registration of '" + registerPatient.Patient.Person.FullnameWithoutMiddlename + "' to '" + registerPatient.Organisation.Name + "' until another Fac/Wing/Unit has been added.");
                            return;
                        }
                    }

                    RegisterPatientDB.UpdateInactive(resiter_patient_id);
                }
                else
                    RegisterPatientDB.UpdateActive(resiter_patient_id);
            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                SetErrorMessage("Can not delete because other records depend on this" + (Utilities.IsDev() ? " : " + fkcEx.Message : "") );
            }

            FillOrganisationRegistrationGrid();
        }
    }
    protected void GrdRegistration_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdRegistration.EditIndex = e.NewEditIndex;
        FillOrganisationRegistrationGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdRegistration.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["addeditpatient_referrals_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression"] == null)
                Session["sortExpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["sortExpression"] = sortExpression + " " + newSortExpr;

            GrdRegistration.DataSource = dataView;
            GrdRegistration.DataBind();
        }
    }

    protected void lnkRegisterPatient_Click(object sender, EventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        try
        {
            RegisterPatientDB.Insert(Convert.ToInt32(ddlOrganisation.SelectedValue), patient.PatientID);
        }
        catch (UniqueConstraintException)
        {
            // happens when 2 forms allow adding
            // do nothing and let form re-update
        }

        FillOrganisationRegistrationGrid();

    }
    protected void SetOrgDdlList()
    {
        UserView userView = UserView.GetInstance();

        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        Organisation[] incList = RegisterPatientDB.GetOrganisationsOf(patient.PatientID);
        DataTable orgs = OrganisationDB.GetDataTable_AllNotInc(incList, true, !userView.IsClinicView, !userView.IsAgedCareView, true, true);
        orgs.DefaultView.Sort = "name ASC";

        ddlOrganisation.Items.Clear();
        foreach (DataRowView row in orgs.DefaultView)
            ddlOrganisation.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
    }

    #endregion

    #region GrdReferrals

    protected void GrdReferrals_FillGrid()
    {
        if (!IsValidFormID())
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage(Utilities.IsDev() ? "No patient exists with this ID" : "");
            return;
        }



        int mst = GetFormMST();

        DataTable dt = ReferralDB.GetDataTable(false, patient.PatientID, mst);
        dt.DefaultView.Sort = "referral_id DESC";
        dt = dt.DefaultView.ToTable();

        int[] referralIDs = new int[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
            referralIDs[i] = Convert.ToInt32(dt.Rows[i]["referral_id"]);

        Hashtable referralsRemainingHash = ReferralRemainingDB.GetHashtableByHealthCardIDs(referralIDs);

        dt.Columns.Add("rerferrals_remaining_text", typeof(String));
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            int referral_id = Convert.ToInt32(dt.Rows[i]["referral_id"]);

            string output = string.Empty;
            ReferralRemaining[] referralsRemaining = (ReferralRemaining[])referralsRemainingHash[referral_id];
            if (referralsRemaining != null && referralsRemaining.Length > 0)
            {
                output += "<table border=0>";
                for (int j = 0; j < referralsRemaining.Length; j++)
                    output += "<tr><td style=\"text-align:left !important;\">" + referralsRemaining[j].Field.Descr + "</td><td style=\"width:4px;\"></td><td>" + referralsRemaining[j].NumServicesRemaining + "</td>";
                output += "</table>";
            }

            dt.Rows[i]["rerferrals_remaining_text"] = output;
        }





        Session["addeditpatient_referrals_data"] = dt;


        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["addeditpatient_referrals_sortexpr"] != null && Session["addeditpatient_referrals_sortexpr"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["addeditpatient_referrals_sortexpr"].ToString();
                GrdReferrals.DataSource = dataView;
            }
            else
            {
                GrdReferrals.DataSource = dt;
            }


            try
            {
                GrdReferrals.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdReferrals.DataSource = dt;
            GrdReferrals.DataBind();

            int TotalColumns = GrdReferrals.Rows[0].Cells.Count;
            GrdReferrals.Rows[0].Cells.Clear();
            GrdReferrals.Rows[0].Cells.Add(new TableCell());
            GrdReferrals.Rows[0].Cells[0].ColumnSpan = TotalColumns - 1;  // hiding first id col in production
            GrdReferrals.Rows[0].Cells[0].Text = "No Referrals Allocated Yet";
        }

        HealthCard[] patientActiveHealthCards = HealthCardDB.GetAllByPatientID(patient.PatientID, false);

        if (patientActiveHealthCards.Length == 0)
        {
            int TotalColumns = GrdReferrals.FooterRow.Cells.Count;
            GrdReferrals.FooterRow.Cells.Clear();
            GrdReferrals.FooterRow.Cells.Add(new TableCell());
            GrdReferrals.FooterRow.Cells[0].ColumnSpan = TotalColumns - 1;  // hiding first id col in production
            GrdReferrals.FooterRow.Cells[0].Text = "A Health Card Is Required To Be Added Before Referrals Can Be Added.";
        }

    }
    protected void GrdReferrals_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Pager)
        {
            //if (!Utilities.IsDev())
            e.Row.Cells[0].CssClass = "hiddencol";
        }
    }
    protected void GrdReferrals_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        DataTable dt = Session["addeditpatient_referrals_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("referral_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];

            int referral_id = Convert.ToInt32(lblId.Text);

            DropDownList ddlMedicalServiceType = (DropDownList)e.Row.FindControl("ddlMedicalServiceType");
            if (ddlMedicalServiceType != null)
            {
                DataTable titles = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "MedicalServiceType", "", " descr ", "medical_service_type_id", "descr");
                ddlMedicalServiceType.DataSource = titles;
                ddlMedicalServiceType.DataTextField = "descr";
                ddlMedicalServiceType.DataValueField = "medical_service_type_id";
                ddlMedicalServiceType.DataBind();
                ddlMedicalServiceType.SelectedValue = thisRow["mct_medical_service_type_id"].ToString();
            }



            DropDownList ddlDateReferralSigned_Day = (DropDownList)e.Row.FindControl("ddlDateReferralSigned_Day");
            DropDownList ddlDateReferralSigned_Month = (DropDownList)e.Row.FindControl("ddlDateReferralSigned_Month");
            DropDownList ddlDateReferralSigned_Year = (DropDownList)e.Row.FindControl("ddlDateReferralSigned_Year");
            DropDownList ddlDateReferralReceived_Day = (DropDownList)e.Row.FindControl("ddlDateReferralReceived_Day");
            DropDownList ddlDateReferralReceived_Month = (DropDownList)e.Row.FindControl("ddlDateReferralReceived_Month");
            DropDownList ddlDateReferralReceived_Year = (DropDownList)e.Row.FindControl("ddlDateReferralReceived_Year");

            if (ddlDateReferralSigned_Day     != null &&
                ddlDateReferralSigned_Month   != null &&
                ddlDateReferralSigned_Year    != null &&
                ddlDateReferralReceived_Day   != null &&
                ddlDateReferralReceived_Month != null &&
                ddlDateReferralReceived_Year  != null)
            {
                ddlDateReferralSigned_Day.Items.Add(new ListItem("--", "-1"));
                ddlDateReferralSigned_Month.Items.Add(new ListItem("--", "-1"));
                ddlDateReferralSigned_Year.Items.Add(new ListItem("--", "-1"));
                ddlDateReferralReceived_Day.Items.Add(new ListItem("--", "-1"));
                ddlDateReferralReceived_Month.Items.Add(new ListItem("--", "-1"));
                ddlDateReferralReceived_Year.Items.Add(new ListItem("--", "-1"));

                for (int i = 1; i <= 31; i++)
                {
                    ddlDateReferralSigned_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlDateReferralReceived_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
                for (int i = 1; i <= 12; i++)
                {
                    ddlDateReferralSigned_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlDateReferralReceived_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
                for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
                {
                    ddlDateReferralSigned_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlDateReferralReceived_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }


                DateTime dateReferralSigned = thisRow["date_referral_signed"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(thisRow["date_referral_signed"]);
                DateTime dateReferralReceivedInOffice = thisRow["date_referral_received_in_office"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(thisRow["date_referral_received_in_office"]);

                if (dateReferralSigned != DateTime.MinValue)
                {
                    ddlDateReferralSigned_Day.SelectedValue = dateReferralSigned.Day.ToString();
                    ddlDateReferralSigned_Month.SelectedValue = dateReferralSigned.Month.ToString();
                    ddlDateReferralSigned_Year.SelectedValue = dateReferralSigned.Year.ToString();
                }
                if (dateReferralReceivedInOffice != DateTime.MinValue)
                {
                    ddlDateReferralReceived_Day.SelectedValue = dateReferralReceivedInOffice.Day.ToString();
                    ddlDateReferralReceived_Month.SelectedValue = dateReferralReceivedInOffice.Month.ToString();
                    ddlDateReferralReceived_Year.SelectedValue = dateReferralReceivedInOffice.Year.ToString();
                }
            }

            int countReferralRemaining = 0;

            Repeater rptReferralRemaining = (Repeater)e.Row.FindControl("rptReferralRemaining");
            if (rptReferralRemaining != null)
            {
                DataTable dt_ReferralRemaining = ReferralRemainingDB.GetDataTable(new int[] { referral_id });
                rptReferralRemaining.DataSource = dt_ReferralRemaining;
                rptReferralRemaining.DataBind();

                countReferralRemaining = dt_ReferralRemaining.Rows.Count;
            }


            DropDownList ddlFields = (DropDownList)e.Row.FindControl("ddlFields");
            LinkButton btnFieldsShowToAdd = (LinkButton)e.Row.FindControl("btnFieldsShowToAdd");
            Button btnFieldAdd = (Button)e.Row.FindControl("btnFieldAdd");
            Button btnFieldsCancelAdd = (Button)e.Row.FindControl("btnFieldsCancelAdd");


            DataTable dt_fields = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "has_offerings = 1 AND field_id <> 0", "descr", "field_id", "descr");

            List<int> referralsVisible = (hiddenViewDdlFieldIDs.Value.Length == 0) ? new List<int> { } : hiddenViewDdlFieldIDs.Value.Split(',').Select(int.Parse).ToList();
            if (referralsVisible.Contains(referral_id))
            {

                ddlFields.Items.Clear();
                foreach (DataRow row in dt_fields.Rows)
                {
                    IDandDescr field = IDandDescrDB.Load(row, "field_id", "descr");
                    ListItem li = new ListItem(field.Descr, field.ID.ToString());
                    li.Attributes.Add("title", field.Descr);
                    ddlFields.Items.Add(li);
                }

                ddlFields.Visible = true;
                btnFieldAdd.Visible = true;
                btnFieldsCancelAdd.Visible = true;
                btnFieldsShowToAdd.Visible = false;

                btnFieldAdd.CommandName = "AddField";
                btnFieldAdd.CommandArgument = referral_id.ToString();

                btnFieldsCancelAdd.CommandName = "CancelAddField";
                btnFieldsCancelAdd.CommandArgument = referral_id.ToString();
            }
            else
            {
                ddlFields.Items.Clear();
                ddlFields.Visible = false;
                btnFieldAdd.Visible = false;
                btnFieldsCancelAdd.Visible = false;

                btnFieldsShowToAdd.CommandName = "ShowFieldsDLL";
                btnFieldsShowToAdd.CommandArgument = referral_id.ToString();
            }

            if (countReferralRemaining == dt_fields.Rows.Count)
                btnFieldsShowToAdd.Visible = false;



            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

            DropDownList ddlMedicalServiceType = (DropDownList)e.Row.FindControl("ddlNewMedicalServiceType");
            DataTable titles = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "MedicalServiceType", "", " descr ", "medical_service_type_id", "descr");
            ddlMedicalServiceType.DataSource = titles;
            ddlMedicalServiceType.DataTextField = "descr";
            ddlMedicalServiceType.DataValueField = "medical_service_type_id";
            ddlMedicalServiceType.DataBind();


            
            DropDownList ddlDateReferralSigned_Day     = (DropDownList)e.Row.FindControl("ddlNewDateReferralSigned_Day");
            DropDownList ddlDateReferralSigned_Month   = (DropDownList)e.Row.FindControl("ddlNewDateReferralSigned_Month");
            DropDownList ddlDateReferralSigned_Year    = (DropDownList)e.Row.FindControl("ddlNewDateReferralSigned_Year");
            DropDownList ddlDateReferralReceived_Day   = (DropDownList)e.Row.FindControl("ddlNewDateReferralReceived_Day");
            DropDownList ddlDateReferralReceived_Month = (DropDownList)e.Row.FindControl("ddlNewDateReferralReceived_Month");
            DropDownList ddlDateReferralReceived_Year  = (DropDownList)e.Row.FindControl("ddlNewDateReferralReceived_Year");

            ddlDateReferralSigned_Day.Items.Add(new ListItem("--", "-1"));
            ddlDateReferralSigned_Month.Items.Add(new ListItem("--", "-1"));
            ddlDateReferralSigned_Year.Items.Add(new ListItem("--", "-1"));
            ddlDateReferralReceived_Day.Items.Add(new ListItem("--", "-1"));
            ddlDateReferralReceived_Month.Items.Add(new ListItem("--", "-1"));
            ddlDateReferralReceived_Year.Items.Add(new ListItem("--", "-1"));

            for (int i = 1; i <= 31; i++)
            {
                ddlDateReferralSigned_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlDateReferralReceived_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            for (int i = 1; i <= 12; i++)
            {
                ddlDateReferralSigned_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlDateReferralReceived_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            for (int i = 2000; i <= DateTime.Today.Year + 5; i++)
            {
                ddlDateReferralSigned_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlDateReferralReceived_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }



            DropDownList ddlHealthCard = (DropDownList)e.Row.FindControl("ddlNewHealthCard");

            HealthCard[] patientActiveHealthCards = HealthCardDB.GetAllByPatientID(patient.PatientID, false);
            foreach (HealthCard healthCard in patientActiveHealthCards)
                ddlHealthCard.Items.Add(new ListItem(healthCard.CardNbr, healthCard.HealthCardID.ToString()));


        }
    }
    protected void GrdReferrals_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdReferrals.EditIndex = -1;
        GrdReferrals_FillGrid();
    }
    protected void GrdReferrals_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdReferrals.Rows[e.RowIndex].FindControl("lblId");

        DropDownList ddlMedicalServiceType = (DropDownList)GrdReferrals.Rows[e.RowIndex].FindControl("ddlMedicalServiceType");

        DropDownList ddlDateReferralSigned_Day     = (DropDownList)GrdReferrals.Rows[e.RowIndex].FindControl("ddlDateReferralSigned_Day");
        DropDownList ddlDateReferralSigned_Month   = (DropDownList)GrdReferrals.Rows[e.RowIndex].FindControl("ddlDateReferralSigned_Month");
        DropDownList ddlDateReferralSigned_Year    = (DropDownList)GrdReferrals.Rows[e.RowIndex].FindControl("ddlDateReferralSigned_Year");

        DropDownList ddlDateReferralReceived_Day   = (DropDownList)GrdReferrals.Rows[e.RowIndex].FindControl("ddlDateReferralReceived_Day");
        DropDownList ddlDateReferralReceived_Month = (DropDownList)GrdReferrals.Rows[e.RowIndex].FindControl("ddlDateReferralReceived_Month");
        DropDownList ddlDateReferralReceived_Year  = (DropDownList)GrdReferrals.Rows[e.RowIndex].FindControl("ddlDateReferralReceived_Year");



        DateTime dateReferralSigned = DateTime.MinValue;
        if (ddlDateReferralSigned_Day.SelectedValue == "-1" && ddlDateReferralSigned_Month.SelectedValue == "-1" && ddlDateReferralSigned_Year.SelectedValue == "-1")
            dateReferralSigned = DateTime.MinValue;
        else if (ddlDateReferralSigned_Day.SelectedValue != "-1" && ddlDateReferralSigned_Month.SelectedValue != "-1" && ddlDateReferralSigned_Year.SelectedValue != "-1")
            dateReferralSigned = new DateTime(Convert.ToInt32(ddlDateReferralSigned_Year.SelectedValue), Convert.ToInt32(ddlDateReferralSigned_Month.SelectedValue), Convert.ToInt32(ddlDateReferralSigned_Day.SelectedValue));
        else
        {
            SetErrorMessage("Referral Signed Date format is some selected and some not selected.");
            return;
        }

        DateTime dateReferralReceived = DateTime.MinValue;
        if (ddlDateReferralReceived_Day.SelectedValue == "-1" && ddlDateReferralReceived_Month.SelectedValue == "-1" && ddlDateReferralReceived_Year.SelectedValue == "-1")
            dateReferralReceived = DateTime.MinValue;
        else if (ddlDateReferralReceived_Day.SelectedValue != "-1" && ddlDateReferralReceived_Month.SelectedValue != "-1" && ddlDateReferralReceived_Year.SelectedValue != "-1")
            dateReferralReceived = new DateTime(Convert.ToInt32(ddlDateReferralReceived_Year.SelectedValue), Convert.ToInt32(ddlDateReferralReceived_Month.SelectedValue), Convert.ToInt32(ddlDateReferralReceived_Day.SelectedValue));
        else
        {
            SetErrorMessage("Referral Received Date format is some selected and some not selected.");
            return;
        }

        
        DataTable dt = Session["addeditpatient_referrals_data"] as DataTable;
        DataRow[] foundRows = dt.Select("referral_id=" + lblId.Text);
        Referral referral = ReferralDB.Load(foundRows[0]);

        ReferralDB.Update(
            Convert.ToInt32(lblId.Text), 
            referral.HealthCard.HealthCardID,
            Convert.ToInt32(ddlMedicalServiceType.SelectedValue),
            referral.RegisterReferrer == null ? -1 : referral.RegisterReferrer.RegisterReferrerID,
            dateReferralSigned,
            dateReferralReceived,
            referral.AddedOrLastModifiedBy.StaffID,
            referral.AddedOrLastModifiedDate,
            referral.DeletedBy == null ? -1 : referral.DeletedBy.StaffID,
            referral.DateDeleted);

        GrdReferrals.EditIndex = -1;
        GrdReferrals_FillGrid();
    }
    protected void GrdReferrals_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdReferrals_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Insert")
        {
            DropDownList ddlMedicalServiceType         = (DropDownList)GrdReferrals.FooterRow.FindControl("ddlNewMedicalServiceType");

            DropDownList ddlDateReferralSigned_Day     = (DropDownList)GrdReferrals.FooterRow.FindControl("ddlNewDateReferralSigned_Day");
            DropDownList ddlDateReferralSigned_Month   = (DropDownList)GrdReferrals.FooterRow.FindControl("ddlNewDateReferralSigned_Month");
            DropDownList ddlDateReferralSigned_Year    = (DropDownList)GrdReferrals.FooterRow.FindControl("ddlNewDateReferralSigned_Year");

            DropDownList ddlDateReferralReceived_Day   = (DropDownList)GrdReferrals.FooterRow.FindControl("ddlNewDateReferralReceived_Day");
            DropDownList ddlDateReferralReceived_Month = (DropDownList)GrdReferrals.FooterRow.FindControl("ddlNewDateReferralReceived_Month");
            DropDownList ddlDateReferralReceived_Year  = (DropDownList)GrdReferrals.FooterRow.FindControl("ddlNewDateReferralReceived_Year");

            DropDownList ddlHealthCard = (DropDownList)GrdReferrals.FooterRow.FindControl("ddlNewHealthCard");


            DateTime dateReferralSigned = DateTime.MinValue;
            if (ddlDateReferralSigned_Day.SelectedValue == "-1" && ddlDateReferralSigned_Month.SelectedValue == "-1" && ddlDateReferralSigned_Year.SelectedValue == "-1")
                dateReferralSigned = DateTime.MinValue;
            else if (ddlDateReferralSigned_Day.SelectedValue != "-1" && ddlDateReferralSigned_Month.SelectedValue != "-1" && ddlDateReferralSigned_Year.SelectedValue != "-1")
                dateReferralSigned = new DateTime(Convert.ToInt32(ddlDateReferralSigned_Year.SelectedValue), Convert.ToInt32(ddlDateReferralSigned_Month.SelectedValue), Convert.ToInt32(ddlDateReferralSigned_Day.SelectedValue));
            else
            {
                SetErrorMessage("Referral Signed Date format is some selected and some not selected.");
                return;
            }

            DateTime dateReferralReceived = DateTime.MinValue;
            if (ddlDateReferralReceived_Day.SelectedValue == "-1" && ddlDateReferralReceived_Month.SelectedValue == "-1" && ddlDateReferralReceived_Year.SelectedValue == "-1")
                dateReferralReceived = DateTime.MinValue;
            else if (ddlDateReferralReceived_Day.SelectedValue != "-1" && ddlDateReferralReceived_Month.SelectedValue != "-1" && ddlDateReferralReceived_Year.SelectedValue != "-1")
                dateReferralReceived = new DateTime(Convert.ToInt32(ddlDateReferralReceived_Year.SelectedValue), Convert.ToInt32(ddlDateReferralReceived_Month.SelectedValue), Convert.ToInt32(ddlDateReferralReceived_Day.SelectedValue));
            else
            {
                SetErrorMessage("Referral Received Date format is some selected and some not selected.");
                return;
            }


            ReferralDB.Insert(
                Convert.ToInt32(ddlHealthCard.SelectedValue),
                Convert.ToInt32(ddlMedicalServiceType.SelectedValue),
                -1,
                dateReferralSigned,
                dateReferralReceived,
                Convert.ToInt32(Session["StaffID"]),
                DateTime.Now,
                -1,
                DateTime.MinValue);

            GrdReferrals_FillGrid();
        }



        if (e.CommandName.Equals("_Delete"))
        {
            int referral_id = Convert.ToInt32(e.CommandArgument);
            ReferralDB.UpdateAsDeleted(referral_id, Convert.ToInt32(Session["StaffID"]));
            GrdReferrals_FillGrid();
        }
        
        if (e.CommandName.Equals("AddField"))
        {
            int referral_id = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = (GridViewRow)(((Control)e.CommandSource).NamingContainer);
            DropDownList ddlFields = (DropDownList)row.FindControl("ddlFields");


            bool alreadyExists = false;

            DataTable dt_ReferralRemaining = ReferralRemainingDB.GetDataTable(new int[] { referral_id });
            for (int i = 0; i < dt_ReferralRemaining.Rows.Count; i++)
                if (Convert.ToInt32(dt_ReferralRemaining.Rows[i]["epcremaining_field_id"]) == Convert.ToInt32(ddlFields.SelectedValue))
                        alreadyExists = true;

            if (!alreadyExists)
                ReferralRemainingDB.Insert(referral_id, Convert.ToInt32(ddlFields.SelectedValue), 1);

            hiddenViewDdlFieldIDs.Value = "";
            GrdReferrals_FillGrid();
        }

        if (e.CommandName.Equals("CancelAddField"))
        {
            int referral_id = Convert.ToInt32(e.CommandArgument);

            hiddenViewDdlFieldIDs.Value = "";
            GrdReferrals_FillGrid();
        }

        if (e.CommandName.Equals("ShowFieldsDLL"))
        {
            int referral_id = Convert.ToInt32(e.CommandArgument);
            hiddenViewDdlFieldIDs.Value = referral_id.ToString();
            GrdReferrals_FillGrid();
        }    
    }
    protected void GrdReferrals_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdReferrals.EditIndex = e.NewEditIndex;
        GrdReferrals_FillGrid();
    }
    protected void GrdReferrals_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdReferrals.EditIndex >= 0)
            return;

        GrdReferrals_Sort(e.SortExpression);
    }
    protected void GrdReferrals_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["addeditpatient_referrals_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["addeditpatient_referrals_sortexpr"] == null)
                Session["addeditpatient_referrals_sortexpr"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["addeditpatient_referrals_sortexpr"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["addeditpatient_referrals_sortexpr"] = sortExpression + " " + newSortExpr;

            GrdReferrals.DataSource = dataView;
            GrdReferrals.DataBind();
        }
    }


    protected void rptReferralRemaining_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName.Equals("RemoveItem"))
        {
            int referral_remaining_id = Convert.ToInt32(e.CommandArgument);
            ReferralRemainingDB.UpdateAsDeleted(referral_remaining_id, Convert.ToInt32(Session["StaffID"]));
            GrdReferrals_FillGrid();
        }

        if (e.CommandName.Equals("AddQty"))
        {
            int referral_remaining_id = Convert.ToInt32(e.CommandArgument);
            ReferralRemainingDB.UpdateNumServicesRemainingByAmount(referral_remaining_id, 1);
            GrdReferrals_FillGrid();
        }

        if (e.CommandName.Equals("SubtractQty"))
        {

            int referral_remaining_id = Convert.ToInt32(e.CommandArgument);
            ReferralRemainingDB.UpdateNumServicesRemainingByAmount(referral_remaining_id, -1);
            GrdReferrals_FillGrid();
        }
    }


    protected void btnRegisterReferrer_ToUpdateInList_Click(object sender, EventArgs e)
    {
        int reg_ref_id  = Convert.ToInt32(hiddenField_RegRefID_ToUpdateInList.Value);
        int referral_id = Convert.ToInt32(hiddenField_ReferralID_ToUpdateInList.Value);
        ReferralDB.UpdateRegRef(referral_id, reg_ref_id);
        GrdReferrals_FillGrid();
    }


    #endregion

    #region GrdHealthCards

    protected void GrdHealthCards_FillGrid()
    {
        if (!IsValidFormID())
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
            return;
        }

        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage(Utilities.IsDev() ? "No patient exists with this ID" : "");
            return;
        }

        DataTable dt = HealthCardDB.GetDataTable_ByPatientID(patient.PatientID, false);

        Session["addeditpatient_healthcards_data"] = dt;


        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["addeditpatient_healthcards_sortexpr"] != null && Session["addeditpatient_healthcards_sortexpr"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["addeditpatient_healthcards_sortexpr"].ToString();
                GrdHealthCards.DataSource = dataView;
            }
            else
            {
                GrdHealthCards.DataSource = dt;
            }


            try
            {
                GrdHealthCards.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdHealthCards.DataSource = dt;
            GrdHealthCards.DataBind();

            int TotalColumns = GrdHealthCards.Rows[0].Cells.Count;
            GrdHealthCards.Rows[0].Cells.Clear();
            GrdHealthCards.Rows[0].Cells.Add(new TableCell());
            GrdHealthCards.Rows[0].Cells[0].ColumnSpan = TotalColumns - 1;  // hiding first id col in production
            GrdHealthCards.Rows[0].Cells[0].Text = "No HealthCards Allocated Yet";
        }

    }
    protected void GrdHealthCards_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Pager)
        {
            //if (!Utilities.IsDev())
            e.Row.Cells[0].CssClass = "hiddencol";
        }
    }
    protected void GrdHealthCards_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Patient patient = PatientDB.GetByID(GetFormID());
        if (patient == null)
        {
            HideTableAndSetErrorMessage("");
            return;
        }

        DataTable dt = Session["addeditpatient_healthcards_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("health_card_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];

            int health_card_id = Convert.ToInt32(lblId.Text);
            HealthCard hc = HealthCardDB.Load(thisRow);



            DropDownList ddlHealthCardOrganisation                 = (DropDownList)e.Row.FindControl("ddlHealthCardOrganisation");
            Label        lblHealthCardCardNbrFamilyNbrSeperator = (Label)e.Row.FindControl("lblHealthCardCardNbrFamilyNbrSeperator");
            DropDownList ddlHealthCardCardFamilyMemberNbr          = (DropDownList)e.Row.FindControl("ddlHealthCardCardFamilyMemberNbr");

            if (ddlHealthCardOrganisation != null)
            {
                DataTable orgs = OrganisationDB.GetDataTable_GroupOrganisations();
                for (int i = orgs.Rows.Count - 1; i >= 0; i--)
                {
                    Organisation org = OrganisationDB.Load(orgs.Rows[i]);
                    if (org.OrganisationID != -1 && org.OrganisationID != -2)
                        orgs.Rows.RemoveAt(i);
                }
                ddlHealthCardOrganisation.DataSource = orgs;
                ddlHealthCardOrganisation.DataTextField = "name";
                ddlHealthCardOrganisation.DataValueField = "organisation_id";
                ddlHealthCardOrganisation.DataBind();
                ddlHealthCardOrganisation.SelectedValue = "-1";

                ddlHealthCardOrganisation.SelectedValue = hc.Organisation.OrganisationID.ToString();

                ddlHealthCardOrganisation.Attributes["onchange"] = "javascript:ddl_health_card_type_changed('" + ddlHealthCardOrganisation.ClientID + "', '" + lblHealthCardCardNbrFamilyNbrSeperator.ClientID + "', '" + ddlHealthCardCardFamilyMemberNbr.ClientID + "');";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ddl_health_card_type_changed", "<script language=javascript>ddl_health_card_type_changed('" + ddlHealthCardOrganisation.ClientID + "', '" + lblHealthCardCardNbrFamilyNbrSeperator.ClientID + "', '" + ddlHealthCardCardFamilyMemberNbr.ClientID + "');</script>");
            }

            if (ddlHealthCardCardFamilyMemberNbr != null)
            {
                if (hc.CardFamilyMemberNbr == "0")
                    ddlHealthCardCardFamilyMemberNbr.Items.Add(new ListItem("0", "0"));
                for (int i = 1; i < 10; i++)
                    ddlHealthCardCardFamilyMemberNbr.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlHealthCardCardFamilyMemberNbr.SelectedValue = hc.CardFamilyMemberNbr;
            }

            DropDownList ddlHealthCardCardExpiry_Month = (DropDownList)e.Row.FindControl("ddlHealthCardCardExpiry_Month");
            DropDownList ddlHealthCardCardExpiry_Year = (DropDownList)e.Row.FindControl("ddlHealthCardCardExpiry_Year");
            if (ddlHealthCardCardExpiry_Month != null && ddlHealthCardCardExpiry_Year != null)
            {
                ddlHealthCardCardExpiry_Month.Items.Add(new ListItem("--", "-1"));
                ddlHealthCardCardExpiry_Year.Items.Add(new ListItem("--", "-1"));

                for (int i = 1; i <= 12; i++)
                    ddlHealthCardCardExpiry_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = DateTime.Today.Year - 10; i <= DateTime.Today.Year + 10; i++)
                    ddlHealthCardCardExpiry_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                if (hc.ExpiryDate != DateTime.MinValue)
                {
                    ddlHealthCardCardExpiry_Month.SelectedValue = hc.ExpiryDate.Month.ToString();
                    ddlHealthCardCardExpiry_Year.SelectedValue = hc.ExpiryDate.Year.ToString();
                }
            }


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlHealthCardOrganisation = (DropDownList)e.Row.FindControl("ddlNewHealthCardOrganisation");
            if (ddlHealthCardOrganisation != null)
            {
                DataTable orgs = OrganisationDB.GetDataTable_GroupOrganisations();
                for (int i = orgs.Rows.Count - 1; i >= 0; i--)
                {
                    Organisation org = OrganisationDB.Load(orgs.Rows[i]);
                    if (org.OrganisationID != -1 && org.OrganisationID != -2)
                        orgs.Rows.RemoveAt(i);
                }
                ddlHealthCardOrganisation.DataSource = orgs;
                ddlHealthCardOrganisation.DataTextField = "name";
                ddlHealthCardOrganisation.DataValueField = "organisation_id";
                ddlHealthCardOrganisation.DataBind();
                ddlHealthCardOrganisation.SelectedValue = "-1";
            }

            DropDownList ddlHealthCardCardFamilyMemberNbr = (DropDownList)e.Row.FindControl("ddlNewHealthCardCardFamilyMemberNbr");
            if (ddlHealthCardCardFamilyMemberNbr != null)
            {
                for (int i = 1; i < 10; i++)
                    ddlHealthCardCardFamilyMemberNbr.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }

            DropDownList ddlHealthCardCardExpiry_Month = (DropDownList)e.Row.FindControl("ddlNewHealthCardCardExpiry_Month");
            DropDownList ddlHealthCardCardExpiry_Year = (DropDownList)e.Row.FindControl("ddlNewHealthCardCardExpiry_Year");
            if (ddlHealthCardCardExpiry_Month != null && ddlHealthCardCardExpiry_Year != null)
            {
                ddlHealthCardCardExpiry_Month.Items.Add(new ListItem("--", "-1"));
                ddlHealthCardCardExpiry_Year.Items.Add(new ListItem("--", "-1"));

                for (int i = 1; i <= 12; i++)
                    ddlHealthCardCardExpiry_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                for (int i = DateTime.Today.Year - 10; i <= DateTime.Today.Year + 10; i++)
                    ddlHealthCardCardExpiry_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
    }
    protected void GrdHealthCards_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdHealthCards.EditIndex = -1;
        GrdHealthCards_FillGrid();
    }
    protected void GrdHealthCards_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdHealthCards.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlHealthCardOrganisation = (DropDownList)GrdHealthCards.Rows[e.RowIndex].FindControl("ddlHealthCardOrganisation");
        TextBox      txtHealthCardCardName = (TextBox)GrdHealthCards.Rows[e.RowIndex].FindControl("txtHealthCardCardName");
        TextBox      txtHealthCardCardNbr = (TextBox)GrdHealthCards.Rows[e.RowIndex].FindControl("txtHealthCardCardNbr");
        DropDownList ddlHealthCardCardFamilyMemberNbr = (DropDownList)GrdHealthCards.Rows[e.RowIndex].FindControl("ddlHealthCardCardFamilyMemberNbr");
        DropDownList ddlHealthCardCardExpiry_Month = (DropDownList)GrdHealthCards.Rows[e.RowIndex].FindControl("ddlHealthCardCardExpiry_Month");
        DropDownList ddlHealthCardCardExpiry_Year = (DropDownList)GrdHealthCards.Rows[e.RowIndex].FindControl("ddlHealthCardCardExpiry_Year");

        DataTable dt = Session["addeditpatient_healthcards_data"] as DataTable;
        DataRow[] foundRows = dt.Select("health_card_id=" + lblId.Text);
        HealthCard hc = HealthCardDB.Load(foundRows[0]);



        if (Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue) != hc.Organisation.OrganisationID)
        {
            bool alreadyHasThisOrg = false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HealthCard thisHC = HealthCardDB.Load(dt.Rows[i]);
                if (thisHC.HealthCardID != hc.HealthCardID && 
                    Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue) == thisHC.Organisation.OrganisationID)
                    alreadyHasThisOrg = true;
            }

            if (alreadyHasThisOrg)
            {
                lblValidationErrorHealthCard.Text = "This patient already has a " + ddlHealthCardOrganisation.SelectedItem.Text + " card. Please select another type.";
                lblValidationErrorHealthCard.Visible = true;
                return;
            }
        }



        DateTime healthCardCardExpiry = DateTime.MinValue;
        if (ddlHealthCardCardExpiry_Month.SelectedValue == "-1" && ddlHealthCardCardExpiry_Year.SelectedValue == "-1")
            healthCardCardExpiry = DateTime.MinValue;
        else if (ddlHealthCardCardExpiry_Month.SelectedValue != "-1" && ddlHealthCardCardExpiry_Year.SelectedValue != "-1")
            healthCardCardExpiry = new DateTime(Convert.ToInt32(ddlHealthCardCardExpiry_Year.SelectedValue), Convert.ToInt32(ddlHealthCardCardExpiry_Month.SelectedValue), 1);
        else
        {
            lblValidationErrorHealthCard.Text = "HealthCard Expiry Date format is some selected and some not selected.";
            lblValidationErrorHealthCard.Visible = true;
            return;
        }

        

        HealthCardDB.Update(
            hc.HealthCardID,
            hc.Patient.PatientID,
            Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue),
            txtHealthCardCardName.Text,
            txtHealthCardCardNbr.Text,
            Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue) == -1 ? ddlHealthCardCardFamilyMemberNbr.SelectedValue : "",
            healthCardCardExpiry,
            hc.DateReferralSigned,
            hc.DateReferralReceivedInOffice,
            hc.IsActive,
            Convert.ToInt32(Session["StaffID"]),
            hc.AreaTreated);

        GrdHealthCards.EditIndex = -1;
        GrdHealthCards_FillGrid();

    }
    protected void GrdHealthCards_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdHealthCards_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Insert")
        {
            DropDownList ddlHealthCardOrganisation        = (DropDownList)GrdHealthCards.FooterRow.FindControl("ddlNewHealthCardOrganisation");
            TextBox      txtHealthCardCardName            = (TextBox)GrdHealthCards.FooterRow.FindControl("txtNewHealthCardCardName");
            TextBox      txtHealthCardCardNbr             = (TextBox)GrdHealthCards.FooterRow.FindControl("txtNewHealthCardCardNbr");
            DropDownList ddlHealthCardCardFamilyMemberNbr = (DropDownList)GrdHealthCards.FooterRow.FindControl("ddlNewHealthCardCardFamilyMemberNbr");
            DropDownList ddlHealthCardCardExpiry_Month    = (DropDownList)GrdHealthCards.FooterRow.FindControl("ddlNewHealthCardCardExpiry_Month");
            DropDownList ddlHealthCardCardExpiry_Year     = (DropDownList)GrdHealthCards.FooterRow.FindControl("ddlNewHealthCardCardExpiry_Year");


            Patient patient = PatientDB.GetByID(GetFormID());
            if (patient == null)
            {
                HideTableAndSetErrorMessage("");
                return;
            }


            DataTable dt = Session["addeditpatient_healthcards_data"] as DataTable;

            bool alreadyHasThisOrg = false;
            bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
            if (!tblEmpty)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HealthCard thisHC = HealthCardDB.Load(dt.Rows[i]);
                    if (Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue) == thisHC.Organisation.OrganisationID)
                        alreadyHasThisOrg = true;
                }
            }
            if (alreadyHasThisOrg)
            {
                lblValidationErrorHealthCard.Text = "This patient already has a " + ddlHealthCardOrganisation.SelectedItem.Text + " card. Please select another type.";
                lblValidationErrorHealthCard.Visible = true;
                return;
            }


            DateTime healthCardCardExpiry = DateTime.MinValue;
            if (ddlHealthCardCardExpiry_Month.SelectedValue == "-1" && ddlHealthCardCardExpiry_Year.SelectedValue == "-1")
                healthCardCardExpiry = DateTime.MinValue;
            else if (ddlHealthCardCardExpiry_Month.SelectedValue != "-1" && ddlHealthCardCardExpiry_Year.SelectedValue != "-1")
                healthCardCardExpiry = new DateTime(Convert.ToInt32(ddlHealthCardCardExpiry_Year.SelectedValue), Convert.ToInt32(ddlHealthCardCardExpiry_Month.SelectedValue), 1);
            else
            {
                lblValidationErrorHealthCard.Text = "HealthCard Expiry Date format is some selected and some not selected.";
                lblValidationErrorHealthCard.Visible = true;
                return;
            }

        
            HealthCardDB.Insert(
                patient.PatientID,
                Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue),
                txtHealthCardCardName.Text,
                txtHealthCardCardNbr.Text,
                Convert.ToInt32(ddlHealthCardOrganisation.SelectedValue) == -1 ? ddlHealthCardCardFamilyMemberNbr.SelectedValue : "",
                healthCardCardExpiry,
                DateTime.MinValue,
                DateTime.MinValue,
                true,
                Convert.ToInt32(Session["StaffID"]),
                "");


            GrdHealthCards_FillGrid();
            GrdReferrals_FillGrid();
        }
    }
    protected void GrdHealthCards_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdHealthCards.EditIndex = e.NewEditIndex;
        GrdHealthCards_FillGrid();
    }
    protected void GrdHealthCards_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdHealthCards.EditIndex >= 0)
            return;

        GrdHealthCards_Sort(e.SortExpression);
    }
    protected void GrdHealthCards_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["addeditpatient_healthcards_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["addeditpatient_healthcards_sortexpr"] == null)
                Session["addeditpatient_healthcards_sortexpr"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["addeditpatient_healthcards_sortexpr"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["addeditpatient_healthcards_sortexpr"] = sortExpression + " " + newSortExpr;

            GrdHealthCards.DataSource = dataView;
            GrdHealthCards.DataBind();
        }
    }

    #endregion


    protected void ddlMedicalServiceType_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect(UrlParamModifier.Update(ddlMedicalServiceType.SelectedValue != "-1", Request.RawUrl, "mst", ddlMedicalServiceType.SelectedValue));
    }

    protected void btnDeleteUndeletePatient_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "Delete" || e.CommandName == "UnDelete")
        {
            if (e.CommandName == "Delete")
            {
                PatientDB.UpdateInactive(GetFormID(), Convert.ToInt32(Session["StaffID"]));
            }
            if (e.CommandName == "UnDelete")
            {
                PatientDB.UpdateActive(GetFormID(), Convert.ToInt32(Session["StaffID"]));
            }

            Patient patient = PatientDB.GetByID(GetFormID());
            btnDeleteUndeletePatient.CommandName = patient.IsDeleted ? "UnDelete"   : "Delete";
            btnDeleteUndeletePatient.Text        = patient.IsDeleted ? "Un-Archive" : "Archive";
            lblPatientStatus.Text                = patient.IsDeleted ? "<b><font color=\"red\">Archived</font></b>" : "Active";

            changeHistoryLinknRow.Visible        = true;
        }
    }



    // http://www.4guysfromrolla.com/articles/090110-1.aspx

    #region GrdScannedDoc

    protected void FillScannedDocGrid()
    {
        try
        {
            // Get the list of files & folders in the CurrentFolder
            DirectoryInfo   currentDirInfo = new DirectoryInfo(GetFullyQualifiedFolderPath(this.CurrentFolder));
            bool            currentDirIsHomeFolder = TwoFoldersAreEquivalent(currentDirInfo.FullName, GetFullyQualifiedFolderPath(this.HomeFolder));

            if (!currentDirInfo.Exists) // in case the folder has been deleted in another page
                throw new CustomMessageException("Could not find directory: " + currentDirInfo.Name);

            DirectoryInfo[] folders        = currentDirInfo.GetDirectories();
            FileInfo[]      files          = currentDirInfo.GetFiles();


            List<FileSystemItemCS> fsItems = new List<FileSystemItemCS>(folders.Length + files.Length);

            // Add the ".." option, if needed
            if (!currentDirIsHomeFolder)
            {
                FileSystemItemCS parentFolder = new FileSystemItemCS(currentDirInfo.Parent);
                parentFolder.Name = "..";
                fsItems.Add(parentFolder);
            }

            Hashtable medicalServiceTypeHash = MedicalServiceTypeDB.GetMedicareServiceTypeHash();
            foreach (DirectoryInfo folder in folders)
            {
                FileSystemItemCS fileSystemItemCS = new FileSystemItemCS(folder);
                if (currentDirIsHomeFolder)
                    fileSystemItemCS.Name = MedicalServiceType.ReplaceMedicareServiceTypeFolders(fileSystemItemCS.Name, medicalServiceTypeHash, true);
                fsItems.Add(fileSystemItemCS);
            }

            foreach (FileInfo file in files)
                fsItems.Add(new FileSystemItemCS(file));


            //GrdScannedDoc.DataSource = fsItems;
            //GrdScannedDoc.DataBind();

            if (fsItems.Count > 0)
            {
                GrdScannedDoc.DataSource = fsItems;
                GrdScannedDoc.DataBind();
            }
            else
            {
                fsItems.Add(new FileSystemItemCS());
                GrdScannedDoc.DataSource = fsItems;
                GrdScannedDoc.DataBind();

                int TotalColumns = GrdScannedDoc.Rows[0].Cells.Count;
                GrdScannedDoc.Rows[0].Cells.Clear();
                GrdScannedDoc.Rows[0].Cells.Add(new TableCell());
                GrdScannedDoc.Rows[0].Cells[0].ColumnSpan = TotalColumns;
                GrdScannedDoc.Rows[0].Cells[0].Text = "No Files Or Folders Found";
            }



            string currentFolderDisplay = this.CurrentFolder;
            if (currentFolderDisplay.StartsWith("~/") || currentFolderDisplay.StartsWith("~\\"))
                currentFolderDisplay = currentFolderDisplay.Substring(2);
            if (currentFolderDisplay.StartsWith("~/") || currentFolderDisplay.StartsWith("~\\"))
                currentFolderDisplay = currentFolderDisplay.Substring(2);

            currentFolderDisplay = currentFolderDisplay.Substring(this.HomeFolder.Length);
            if (currentFolderDisplay.StartsWith("\\")) currentFolderDisplay = currentFolderDisplay.Substring(1);
            if (currentFolderDisplay == "") currentFolderDisplay = "[Root Directory]";

            currentFolderDisplay = MedicalServiceType.ReplaceMedicareServiceTypeFolders(currentFolderDisplay, medicalServiceTypeHash, true);

            lblCurrentPath.Text = "Viewing Folder: &nbsp;&nbsp; <b><font color=\"blue\">" + currentFolderDisplay + "</font></b>";
        }
        catch(CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch(Exception ex)
        {
            SetErrorMessage("", ex.ToString());
        }
    }
    protected void GrdScannedDoc_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {
        }

        UserView userview = UserView.GetInstance();
        if (!userview.IsMasterAdmin)
            e.Row.Cells[4].CssClass = "hiddencol";

    }
    protected void GrdScannedDoc_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FileSystemItemCS item = e.Row.DataItem as FileSystemItemCS;

            if (item.IsFolder)
            {
                LinkButton lbFolderItem = e.Row.FindControl("lbFolderItem") as LinkButton;
                lbFolderItem.Text = string.Format(@"<img src=""{0}"" alt="""" />&nbsp;{1}", Page.ResolveClientUrl("~/images/folder.png"), item.Name);
            }
            else
            {
                Literal ltlFileItem = e.Row.FindControl("ltlFileItem") as Literal;
                if (this.CurrentFolder.StartsWith("~"))
                    ltlFileItem.Text = string.Format(@"<a href=""{0}"" target=""_blank"">{1}</a>",
                            Page.ResolveClientUrl(string.Concat(this.CurrentFolder, "/", item.Name).Replace("//", "/")),
                            item.Name);
                else
                    ltlFileItem.Text = item.Name;
            }


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                btnDelete.OnClientClick =
                    item.IsFolder ?
                    "javascript:if (!confirm('Are you sure you want to PERMANENTLY delete this folder and all of it\\'s contents?')) return false;" :
                    "javascript:if (!confirm('Are you sure you want to PERMANENTLY delete this file?')) return false;";
            }

        }
    }
    protected void GrdScannedDoc_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdScannedDoc.EditIndex = -1;
        FillScannedDocGrid();
    }
    protected void GrdScannedDoc_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        /*
        Label   lblId     = (Label)GrdScannedDoc.Rows[e.RowIndex].FindControl("lblId");
        TextBox txtFolder = (TextBox)GrdScannedDoc.Rows[e.RowIndex].FindControl("txtFolder");


        DataTable dt = Session["scanned_doc_data"] as DataTable;
        DataRow[] foundRows = dt.Select("scanned_doc_id=" + lblId.Text);
        ScannedDoc scannedDoc = ScannedDocDB.LoadAll(foundRows[0]);

        ScannedDocDB.Update(
            scannedDoc.ScannedDocID, 
            scannedDoc.Patient.PatientID, 
            scannedDoc.MedicalServiceType == null ? -1 : scannedDoc.MedicalServiceType.ID, 
            txtFolder.Text.ToUpper(),
            scannedDoc.Filename.Trim());

        GrdScannedDoc.EditIndex = -1;
        FillScannedDocGrid();
        */
    }
    protected void GrdScannedDoc_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
    }
    protected void GrdScannedDoc_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "OpenFolder")
        {
            if (string.CompareOrdinal(e.CommandArgument.ToString(), "..") == 0)
            {
                string currentFullPath = this.CurrentFolder;
                if (currentFullPath.EndsWith("\\") || currentFullPath.EndsWith("/"))
                    currentFullPath = currentFullPath.Substring(0, currentFullPath.Length - 1);

                currentFullPath = currentFullPath.Replace("/", "\\");

                string[] folders = currentFullPath.Split("\\".ToCharArray());

                this.CurrentFolder = string.Join("\\", folders, 0, folders.Length - 1);
            }
            else
                this.CurrentFolder = (string)e.CommandArgument;

            FillScannedDocGrid();
        }
    }
    protected void GrdScannedDoc_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdScannedDoc.EditIndex = e.NewEditIndex;
        FillScannedDocGrid();
    }
    protected void GrdScannedDoc_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdScannedDoc.EditIndex >= 0)
            return;

        DataTable dataTable = Session["scanned_doc_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["scanned_doc_sortexpression"] == null)
                Session["scanned_doc_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["scanned_doc_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["scanned_doc_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdScannedDoc.DataSource = dataView;
            GrdScannedDoc.DataBind();
        }
    }

    #endregion

    #region Properties

    public string HomeFolder
    {
        get
        {
            return ViewState["HomeFolder"] as string;
        }
        set
        {
            ViewState["HomeFolder"] = value;
        }
    }

    public string CurrentFolder
    {
        get { return ViewState["CurrentFolder"] as string; }
        set { ViewState["CurrentFolder"] = value; }
    }

   
    #endregion

    #region DisplaySize, GetFullyQualifiedFolderPath, TwoFoldersAreEquivalent

    protected string DisplaySize(long? size)
    {
        if (size == null)
            return string.Empty;
        else
        {
            if (size < 1024)
                return string.Format("{0:N0} bytes", size.Value);
            else if (size < 1048576)
                return String.Format("{0:N0} KB", size.Value / 1024);
            else
                return String.Format("{0:0.#} MB", (double)size.Value / 1048576);
        }
    }    

    private string GetFullyQualifiedFolderPath(string folderPath)
    {
        if (folderPath.StartsWith("~"))
            return Server.MapPath(folderPath);
        else
            return folderPath;
    }

    private bool TwoFoldersAreEquivalent(string folderPath1, string folderPath2)
    {
        // Chop off any trailing slashes...
        if (folderPath1.EndsWith("\\") || folderPath1.EndsWith("/"))
            folderPath1 = folderPath1.Substring(0, folderPath1.Length - 1);

        if (folderPath2.EndsWith("\\") || folderPath2.EndsWith("/"))
            folderPath2 = folderPath1.Substring(0, folderPath2.Length - 1);

        return string.CompareOrdinal(folderPath1, folderPath2) == 0;
    }

    #endregion



    #region btnUpload_Click, btnDownload_Click, btnDeleteFie_Click

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            SaveFiles(this.CurrentFolder, chkAllowOverwrite.Checked);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            SetErrorMessage("Connection to network files currently unavailable.");
            return;
        }
    }

    protected void btnDownload_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton lnkbtn = (LinkButton)sender;

            string fileToDownload = lnkbtn.CommandArgument;
            FileInfo fi = new FileInfo(fileToDownload);

            string fileNameToDownload = fi.Name.Substring(Utilities.IndexOfNth(fi.Name, '_', 2) + 1);
            DownloadDocument(fileToDownload, fileNameToDownload, false);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
    }

    protected void btnDeleteFie_Click(object sender, EventArgs e)
    {
        try
        {
            ImageButton btnDelete = (ImageButton)sender;
            string itemToDelete = btnDelete.CommandArgument;

            if (Directory.Exists(itemToDelete) || File.Exists(itemToDelete)) // don't throw an error if it was deleted in another page
            {
                // delete the actual file/folder
                FileAttributes attr = File.GetAttributes(itemToDelete);
                if (attr.HasFlag(FileAttributes.Directory))
                    System.IO.Directory.Delete(itemToDelete, true);
                else
                    System.IO.File.Delete(itemToDelete);
            }

            FillScannedDocGrid();
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            SetErrorMessage("Connection to network files currently unavailable.");
            return;
        }

    }

    protected void btnAddDirectory_Click(object sender, EventArgs e)
    {
        txtNewDirectory.Text = txtNewDirectory.Text.Trim();

        string curFolder = this.CurrentFolder;
        if (!curFolder.EndsWith("\\"))
            curFolder += "\\";

        try
        {
            if (Regex.IsMatch(txtNewDirectory.Text, @"__\d+__"))
                throw new CustomMessageException("Folder name can not be double underscore, followed by digits, followed by a double underscore");

            CreateDirectory(curFolder + txtNewDirectory.Text);
            txtNewDirectory.Text = string.Empty;
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException && ex.Message == "Illegal characters in path.")
                SetErrorMessage("Contains invalid folder characters");
            else
                SetErrorMessage(ex.Message);
        }
        finally
        {
            FillScannedDocGrid();
        }
    }

    #region SaveFiles()

    private bool SaveFiles(string dir, bool allowOverwrite)
    {
        string allowedFileTypes = "bmp|dcs|doc|docm|docx|dot|dotm|dotx|gz|gif|htm|html|ipg|jpe|jpeg|jpg|mp3|mp4|wav|pdf|png|ppa|ppam|pps|ppsm|ppt|pptm|tar|tgz|tif|tiff|txt|xla|xlam|xlc|xld|xlk|xll|xlm|xls|xlsx|xlt|xltm|xltx|xlw|xml|z|zip|7z";

        dir = dir.EndsWith(@"\") ? dir : dir + @"\";

        System.Text.StringBuilder _messageToUser = new System.Text.StringBuilder("Files Uploaded:<br>");

        try
        {

            HttpFileCollection _files = Request.Files;

            if (_files.Count == 0 || (_files.Count == 1 && System.IO.Path.GetFileName(_files[0].FileName) == string.Empty))
            {
                lblUploadMessage.Text = " <font color=\"red\">No Files Selected</font> <BR>";
                return true;
            }


            for (int i = 0; i < _files.Count; i++)
            {
                HttpPostedFile _postedFile = _files[i];
                string _fileName = System.IO.Path.GetFileName(_postedFile.FileName);
                if (_fileName.Length == 0)
                    continue;

                if (_postedFile.ContentLength > 20971520)
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! Over allowable file size limit!</font> <BR>");
                if (!ExtIn(System.IO.Path.GetExtension(_fileName), allowedFileTypes))
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! Only " + ExtToDisplay(allowedFileTypes) + " files allowed!</font> <BR>");

                if (!allowOverwrite && File.Exists(dir + "\\" + _fileName))
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! File already exists. To allow overwrite, check the \"Allowed File Overwrite\" box</font> <BR>");
            }

            int countZeroFileLength = 0;
            for (int i = 0; i < _files.Count; i++)
            {
                HttpPostedFile _postedFile = _files[i];
                string _fileName = System.IO.Path.GetFileName(_postedFile.FileName);
                if (_fileName.Length == 0)
                    continue;

                if (_postedFile.ContentLength > 0)
                {
                    string dbName = System.Web.HttpContext.Current.Session["DB"].ToString().Replace("_TestDB", "");
                    bool useLocalFolder = Convert.ToBoolean(
                        System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] != null ?
                        System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] :
                        System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal"]
                        );

                    if (useLocalFolder)
                    {
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        _postedFile.SaveAs(dir + _fileName);
                        _messageToUser.Append(_fileName + "<BR>");
                    }
                    else  // use network folder
                    {
                        string username    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderUsername"];
                        string password    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderPassword"];

                        string networkName =
                            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] != null ?
                            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] :
                            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder"];

                        if (networkName.EndsWith(@"\"))
                            networkName = networkName.Substring(0, networkName.Length - 1);

                        using (new NetworkConnection(networkName, new System.Net.NetworkCredential(username, password)))
                        {
                            if (!Directory.Exists(dir))
                                Directory.CreateDirectory(dir);

                            _postedFile.SaveAs(dir + _fileName);
                            _messageToUser.Append(_fileName + "<BR>");
                        }
                    }


                    /*
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    //_postedFile.SaveAs(Server.MapPath("MyFiles") + "\\" + System.IO.Path.GetFileName(_postedFile.FileName));
                    _postedFile.SaveAs(dir + _fileName);
                    _messageToUser.Append(_fileName + "<BR>");
                    */

                }
                else
                {
                    countZeroFileLength++;
                }
            }

            if (_files.Count > 0 && countZeroFileLength == _files.Count)
                throw new Exception("<font color=\"red\">File" + (_files.Count > 1 ? "s are" : " is") + " 0 kb.</font>");
            else if (_files.Count > 0 && countZeroFileLength > 0)
                throw new Exception("<font color=\"red\">File(s) of 0 kb were not uploaded.</font>");

            lblUploadMessage.Text = _messageToUser.ToString();
            return true;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            lblUploadMessage.Text = "Connection to network files currently unavailable.";
            return false;
        }
        catch (System.Exception ex)
        {
            lblUploadMessage.Text = ex.Message;
            return false;
        }
        finally
        {
            try
            {
                FillScannedDocGrid();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                SetErrorMessage("Connection to network files currently unavailable.");
            }

        }
    }

    protected bool ExtIn(string ext, string allowedExtns)
    {
        ext = ext.ToUpper();
        string[] allowedExtnsList = allowedExtns.Split('|');
        foreach (string curExt in allowedExtnsList)
            if (ext == "." + curExt.ToUpper())
                return true;

        return false;
    }

    protected string ExtToDisplay(string allowedExtns)
    {
        string ret = string.Empty;
        foreach (string curExt in allowedExtns.Split('|'))
        {
            if (ret.Length > 0)
                ret += ",";

            ret += ("." + curExt);
        }

        return ret;
    }

    #endregion

    #region DownloadFile

    protected void DownloadDocument(string filepath, string downloadFileName, bool deleteDocAfterDownload)
    {
        System.IO.FileInfo file = new System.IO.FileInfo(filepath);
        if (!file.Exists)
            throw new CustomMessageException("File doesn't exist: " + file.Name);

        try
        {
            string contentType = "application/octet-stream";
            try { contentType = Utilities.GetMimeType(System.IO.Path.GetExtension(downloadFileName)); }
            catch (Exception) { }

            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFileName + "\"" );
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.WriteFile(filepath, true);
            if (deleteDocAfterDownload)
                File.Delete(filepath);
            Response.End();
        }
        catch (System.Web.HttpException ex) 
        {
            // ignore exception where user closed the download box
            if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                throw;
        }
    }

    #endregion

    #endregion

    #region CreateDirectory(dir)

    protected void CreateDirectory(string dir)
    {
        string dbName = System.Web.HttpContext.Current.Session["DB"].ToString().Replace("_TestDB", "");
        bool useLocalFolder = Convert.ToBoolean(
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] != null ?
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal" + "_" + dbName] :
            System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsFolderUseLocal"]
            );

        if (useLocalFolder)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        else  // use network folder
        {
            string username    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderUsername"];
            string password    = System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolderPassword"];

            string networkName =
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] != null ?
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder" + "_" + dbName] :
                System.Configuration.ConfigurationManager.AppSettings["PatientScannedDocsNetworkFolder"];

            if (networkName.EndsWith(@"\"))
                networkName = networkName.Substring(0, networkName.Length - 1);

            using (new NetworkConnection(networkName, new System.Net.NetworkCredential(username, password)))
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
        }
    }

    #endregion


    #region lstCurrentFiles_ItemDataBound

    protected void lstCurrentFiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Repeater myRepeater = (Repeater)sender;
        if (e.Item.ItemType == ListItemType.Footer)
        {
            Label lblNoScannedDocsText = e.Item.FindControl("lblNoScannedDocsText") as Label;
            lblNoScannedDocsText.Visible = myRepeater.Items.Count == 0;
        }
    }

    #endregion


    #region HideTableAndSetErrorMessage, SetErrorMessage, HideErrorMessag

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
        lnkFlashingText.Visible = false;
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

        lblValidationErrorHealthCard.Visible = false;
        lblValidationErrorHealthCard.Text = "";

        lblValidationErrorReferral.Visible = false;
        lblValidationErrorReferral.Text = "";
    }

    #endregion

}