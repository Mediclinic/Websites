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

public partial class Letters_PrintV2 : System.Web.UI.Page
{

    // -- http://localhost:7608/Mediclinic/Letters_PrintV2.aspx?patient=1&org=1550&letter_type=234&letter=2

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, false);
                Session.Remove("data_selected");
                Session.Remove("sortExpression_Selected");
                Session.Remove("sortExpression_Offering");

                SetGUI();
                SetUrlFields();
                PopulateLettersList();
            }
            else
            {
                ResetPatientName();
                ResetOrgName();
                SetBooking();
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

    #region SetUrlFields, SetBooking

    protected void SetBooking()
    {
        string booking_patient_id = Request.QueryString["bookingpatient"];
        string booking_id         = Request.QueryString["booking"];

        if (booking_patient_id != null)
        {
            if (!Regex.IsMatch(booking_patient_id, @"^\d+$"))
                throw new CustomMessageException();

            BookingPatient bookingPatient = BookingPatientDB.GetByID(Convert.ToInt32(booking_patient_id));
            if (bookingPatient == null)
                throw new CustomMessageException();
            if (bookingPatient.Booking.Organisation == null)
                throw new CustomMessageException();

            // get selected id's
            ArrayList selectedIDs = new ArrayList();
            foreach (RepeaterItem item in lstNotes.Items)
            {
                CheckBox chkUseNote = (CheckBox)item.FindControl("chkUseNote");
                Label    lblNoteID  = (Label)item.FindControl("lblNoteID");
                if (chkUseNote.Checked)
                    selectedIDs.Add(lblNoteID.Text);
            }

            txtUpdatePatientID.Text      = bookingPatient.Patient.PatientID.ToString();
            txtUpdatePatientName.Text    = bookingPatient.Patient.Person.FullnameWithoutMiddlename;
            txtUpdatePatientName.Visible = false;
            lblUpdatePatientName.Text    = "<a href=\"#=\" onclick=\"open_new_window('PatientDetailV2.aspx?type=view&id=" + bookingPatient.Patient.PatientID + "'); return false;\">" + bookingPatient.Patient.Person.FullnameWithoutMiddlename + "</a>";
            lblUpdatePatientName.Visible = true;

            txtUpdateOrganisationID.Text      = bookingPatient.Booking.Organisation.OrganisationID.ToString();
            txtUpdateOrganisationName.Text    = bookingPatient.Booking.Organisation.Name;
            txtUpdateOrganisationName.Visible = false;
            lblUpdateOrganisationName.Text    = "<a href=\"#=\" onclick=\"open_new_window('OrganisationDetailV2.aspx?type=view&id=" + bookingPatient.Booking.Organisation.OrganisationID + "'); return false;\">" + bookingPatient.Booking.Organisation.Name + "</a>";
            lblUpdateOrganisationName.Visible = true;



            // show booking info

            lnkBookingSheetForPatient.Text = "Booking sheet for " + bookingPatient.Patient.Person.FullnameWithoutMiddlename;
            lnkBookingSheetForPatient.PostBackUrl = String.Format("~/BookingsV2.aspx?type=patient&patient={0}&org={1}&staff={2}&offering={3}&date={4}", bookingPatient.Patient.PatientID, bookingPatient.Booking.Organisation.OrganisationID, bookingPatient.Booking.Provider.StaffID, bookingPatient.Offering.OfferingID, bookingPatient.Booking.DateStart.ToString("yyyy_MM_dd"));

            lnkBookingListForPatient.Text = "Booking list for " + bookingPatient.Patient.Person.FullnameWithoutMiddlename;
            lnkBookingListForPatient.PostBackUrl = String.Format("~/BookingsListV2.aspx?patient={0}", bookingPatient.Patient.PatientID);

            lblBooking_Provider.Text      = bookingPatient.Booking.Provider.Person.FullnameWithoutMiddlename;
            lblBooking_Offering.Text      = bookingPatient.Offering.Name;
            lblBooking_BookingStatus.Text = bookingPatient.Booking.BookingStatus.Descr.ToString();
            lblBooking_Time.Text          = bookingPatient.Booking.DateStart.Date.ToString("dd MMM yyyy") + " - " + bookingPatient.Booking.DateStart.ToString("hh:mm") + "-" + bookingPatient.Booking.DateEnd.ToString("hh:mm");
            lblBooking_Notes.Text         = Note.GetPopupLinkTextV2(15, bookingPatient.EntityID, bookingPatient.NoteCount > 0, true, 1050, 530, "images/notes-bw-24.jpg", "images/notes-24.png");


            // display list of notes in repeater
            DataTable notes = NoteDB.GetDataTable_ByEntityID(bookingPatient.EntityID);
            lstNotes.DataSource = notes;
            lstNotes.DataBind();

            // check id's that were previously checked
            foreach (RepeaterItem item in lstNotes.Items)
            {
                CheckBox chkUseNote      = (CheckBox)item.FindControl("chkUseNote");
                Label    lblNoteID       = (Label)item.FindControl("lblNoteID");
                Label    lblOriginalText = (Label)item.FindControl("lblOriginalText");
                chkUseNote.Checked  = selectedIDs.Contains(lblNoteID.Text);
            }

            // hide if got from url ... no need to change it
            btnPatientListPopup.Visible = false;
            btnClearPatient.Visible = false;
            btnOrganisationListPopup.Visible = false;
            btnClearOrganisation.Visible = false;
        }
        else if (booking_id != null)
        {
            if (!Regex.IsMatch(booking_id, @"^\d+$"))
                throw new CustomMessageException();

            Booking booking = BookingDB.GetByID(Convert.ToInt32(booking_id));
            if (booking == null)
                throw new CustomMessageException();
            if (booking.Patient == null)
            {
                DataTable dt = BookingPatientDB.GetDataTable_ByBookingID(booking.BookingID);
                lstBookingPatients.DataSource = dt;
                lstBookingPatients.DataBind();

                main_table.Visible = false;
                select_booking_patient_table.Visible = true;
                return;

                //throw new CustomMessageException();
            }
            if (booking.Organisation == null)
                throw new CustomMessageException();

            // get selected id's
            ArrayList selectedIDs = new ArrayList();
            foreach (RepeaterItem item in lstNotes.Items)
            {
                CheckBox chkUseNote = (CheckBox)item.FindControl("chkUseNote");
                Label    lblNoteID  = (Label)item.FindControl("lblNoteID");
                if (chkUseNote.Checked)
                    selectedIDs.Add(lblNoteID.Text);
            }

            txtUpdatePatientID.Text   = booking.Patient.PatientID.ToString();
            txtUpdatePatientName.Text = booking.Patient.Person.FullnameWithoutMiddlename;
            txtUpdatePatientName.Visible = false;
            lblUpdatePatientName.Text = "<a href=\"#=\" onclick=\"open_new_window('PatientDetailV2.aspx?type=view&id=" + booking.Patient.PatientID + "'); return false;\">" + booking.Patient.Person.FullnameWithoutMiddlename + "</a>";
            lblUpdatePatientName.Visible = true;

            txtUpdateOrganisationID.Text = booking.Organisation.OrganisationID.ToString();
            txtUpdateOrganisationName.Text = booking.Organisation.Name;
            txtUpdateOrganisationName.Visible = false;
            lblUpdateOrganisationName.Text = "<a href=\"#=\" onclick=\"open_new_window('OrganisationDetailV2.aspx?type=view&id=" + booking.Organisation.OrganisationID + "'); return false;\">" + booking.Organisation.Name + "</a>";
            lblUpdateOrganisationName.Visible = true;



            // show booking info

            lnkBookingSheetForPatient.Text = "Booking sheet for " + booking.Patient.Person.FullnameWithoutMiddlename;
            lnkBookingSheetForPatient.PostBackUrl =  String.Format("~/BookingsV2.aspx?type=patient&patient={0}&org={1}&staff={2}&offering={3}&date={4}",booking.Patient.PatientID, booking.Organisation.OrganisationID, booking.Provider.StaffID, booking.Offering.OfferingID, booking.DateStart.ToString("yyyy_MM_dd"));

            lnkBookingListForPatient.Text = "Booking list for " + booking.Patient.Person.FullnameWithoutMiddlename;
            lnkBookingListForPatient.PostBackUrl = String.Format("~/BookingsListV2.aspx?patient={0}", booking.Patient.PatientID);

            lblBooking_Provider.Text = booking.Provider.Person.FullnameWithoutMiddlename;
            lblBooking_Offering.Text = booking.Offering.Name;
            lblBooking_BookingStatus.Text = booking.BookingStatus.Descr.ToString();
            lblBooking_Time.Text = booking.DateStart.Date.ToString("dd MMM yyyy") + " - " + booking.DateStart.ToString("hh:mm") + "-" + booking.DateEnd.ToString("hh:mm");
            lblBooking_Notes.Text = Note.GetPopupLinkTextV2(15, booking.EntityID, booking.NoteCount > 0, true, 1050, 530, "images/notes-bw-24.jpg", "images/notes-24.png");


            // display list of notes in repeater
            DataTable notes = NoteDB.GetDataTable_ByEntityID(booking.EntityID);
            lstNotes.DataSource = notes;
            lstNotes.DataBind();

            // check id's that were previously checked
            foreach (RepeaterItem item in lstNotes.Items)
            {
                CheckBox chkUseNote      = (CheckBox)item.FindControl("chkUseNote");
                Label    lblNoteID       = (Label)item.FindControl("lblNoteID");
                Label    lblOriginalText = (Label)item.FindControl("lblOriginalText");
                chkUseNote.Checked  = selectedIDs.Contains(lblNoteID.Text);
            }

            // hide if got from url ... no need to change it
            btnPatientListPopup.Visible = false;
            btnClearPatient.Visible = false;
            btnOrganisationListPopup.Visible = false;
            btnClearOrganisation.Visible = false;
        }
    }


    protected void lstBookingPatients_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (lstBookingPatients.Items.Count < 1)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                Label lblFooter = (Label)e.Item.FindControl("lblEmptyData");
                lblFooter.Visible = true;
            }
        }
    }


    protected void lstNotes_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Footer)
        {
            System.Web.UI.HtmlControls.HtmlTableRow myNoDataRow = (System.Web.UI.HtmlControls.HtmlTableRow)e.Item.FindControl("myNoDataRow");
            myNoDataRow.Visible = this.lstNotes.Items.Count == 0;
        }
    }

    protected void SetGUI()
    {
        UserView userView = UserView.GetInstance();
        lblOrganisationType.Text = !userView.IsAgedCareView ? "Clinic" : "Facility";
    }

    protected void SetUrlFields()
    {
        try
        {
            string booking_patient_id = Request.QueryString["bookingpatient"];
            string booking_id         = Request.QueryString["booking"];

            if (booking_patient_id != null)
            {
                lblHeading.Text = "Print A Letter For Booking";
                SetBooking();
            }
            else if (booking_id != null)
            {
                lblHeading.Text = "Print A Letter For Booking";
                SetBooking();
            }
            else
            {
                lblHeading.Text = "Print A Letter";

                td_booking_space.Visible = false;
                td_booking.Visible = false;

                string patient_id = Request.QueryString["patient"];
                if (patient_id != null && patient_id != "-1")
                {
                    if (!Regex.IsMatch(patient_id, @"^\d+$"))
                        throw new CustomMessageException();

                    Patient patient = PatientDB.GetByID(Convert.ToInt32(patient_id));
                    if (patient == null)
                        throw new CustomMessageException();

                    txtUpdatePatientID.Text = patient.PatientID.ToString();
                    txtUpdatePatientName.Text = patient.Person.FullnameWithoutMiddlename;
                    txtUpdatePatientName.Visible = false;
                    lblUpdatePatientName.Text = "<a href=\"#=\" onclick=\"open_new_window('PatientDetailV2.aspx?type=view&id=" + patient.PatientID + "'); return false;\">" + patient.Person.FullnameWithoutMiddlename + "</a>";
                    lblUpdatePatientName.Visible = true;


                    // hide if got from url ... no need to change it
                    btnPatientListPopup.Visible = false;
                    btnClearPatient.Visible = false;


                    // if patient only linked to 1 org, then set org
                    Organisation[] orgs = RegisterPatientDB.GetOrganisationsOf(patient.PatientID);
                    if (orgs.Length == 1)
                    {
                        txtUpdateOrganisationID.Text = orgs[0].OrganisationID.ToString();
                        txtUpdateOrganisationName.Text = orgs[0].Name;
                        txtUpdateOrganisationName.Visible = false;
                        lblUpdateOrganisationName.Text = "<a href=\"#=\" onclick=\"open_new_window('OrganisationDetailV2.aspx?type=view&id=" + orgs[0].OrganisationID + "'); return false;\">" + orgs[0].Name + "</a>";
                        lblUpdateOrganisationName.Visible = true;

                        PopulateLettersList();

                        // hide if got from url ... no need to change it
                        btnOrganisationListPopup.Visible = false;
                        btnClearOrganisation.Visible = false;
                    }
                }

                string org_id = Request.QueryString["org"];
                if (org_id != null && org_id != "0")
                {
                    if (!Regex.IsMatch(org_id, @"^\d+$"))
                        throw new CustomMessageException();

                    Organisation org = OrganisationDB.GetByID(Convert.ToInt32(org_id));
                    if (org == null)
                        throw new CustomMessageException();

                    txtUpdateOrganisationID.Text = org.OrganisationID.ToString();
                    txtUpdateOrganisationName.Text = org.Name;
                    txtUpdateOrganisationName.Visible = false;
                    lblUpdateOrganisationName.Text = "<a href=\"#=\" onclick=\"open_new_window('OrganisationDetailV2.aspx?type=view&id=" + org.OrganisationID + "'); return false;\">" + org.Name + "</a>";
                    lblUpdateOrganisationName.Visible = true;

                    PopulateLettersList();

                    // hide if got from url ... no need to change it
                    btnOrganisationListPopup.Visible = false;
                    btnClearOrganisation.Visible = false;
                }
            }


            UpdateTextbox(txtUpdatePatientName, lblUpdatePatientName, txtUpdatePatientID.Text.Length == 0);
            UpdateTextbox(txtUpdateOrganisationName, lblUpdateOrganisationName, txtUpdateOrganisationID.Text.Length == 0);

            string letter_id = Request.QueryString["letter"];
            if (letter_id != null && letter_id != "-1")
            {
                if (!Regex.IsMatch(letter_id, @"^\d+$"))
                    throw new CustomMessageException();

                Letter letter = LetterDB.GetByID(Convert.ToInt32(letter_id));
                if (letter == null)
                    throw new CustomMessageException();

                foreach(ListItem item in lstLetters.Items)
                    if (item.Value == letter.LetterID.ToString())
                        item.Selected = true;
            }


            txtSubject.Text   = SystemVariableDB.GetByDescr("LettersEmailDefaultSubject").Value;
            FreeTextBox1.Text = SystemVariableDB.GetByDescr("LettersEmailSignature").Value;

        }
        catch (CustomMessageException ex)
        {
            SetErrorMessage();
        }
    }

    #endregion

    #region PopulateLettersList, LetterExists

    protected void chkUseDefaultDocs_CheckedChanged(object sender, EventArgs e)
    {
        PopulateLettersList();
    }

    protected void PopulateLettersList()
    {
        UserView userView = UserView.GetInstance();

        int orgID = (txtUpdateOrganisationID.Text.Length == 0) ? 0 : Convert.ToInt32(txtUpdateOrganisationID.Text);
        if (orgID == 0 && !userView.IsAdminView)
            orgID = Convert.ToInt32(Session["OrgID"]);
        bool usingDeafultLetters = orgID == 0 || chkUseDefaultDocs.Checked;  // || !LetterDB.OrgHasdocs(orgID);
        DataTable letters = LetterDB.GetDataTable_ByOrg(usingDeafultLetters ? 0 : orgID, usingDeafultLetters ? Convert.ToInt32(Session["SiteID"]) : -1);

        lblSpaceBeforeUseDefaultDocsCheckbox.Visible = orgID != 0;
        chkUseDefaultDocs.Visible                    = orgID != 0;
        chkUseDefaultDocs.ForeColor                  = System.Drawing.Color.DarkOrchid;
        lblDefaultOrOrgSpecificDocs.Visible          = orgID == 0;
        lblSpaceBeforeUseDefaultDocsCheckbox.Visible = orgID == 0;
        if (usingDeafultLetters)
            SetLabel(lblDefaultOrOrgSpecificDocs, "**Using Default Docs", System.Drawing.Color.Blue, false);
        else
            SetLabel(lblDefaultOrOrgSpecificDocs, "**Using Organisation Specific Docs", System.Drawing.Color.DarkOrchid, false);

        // remove ones that dont exists
        for (int i = letters.Rows.Count - 1; i >= 0; i--)
        {
            Letter letter = LetterDB.LoadAll(letters.Rows[i]);
            if (!letter.FileExists(Convert.ToInt32(Session["SiteID"])))
                letters.Rows.RemoveAt(i);
        }

        lstLetters.DataSource = letters;
        lstLetters.DataTextField  = "letter_docname";
        lstLetters.DataValueField = "letter_letter_id";
        lstLetters.DataBind();
    }

    protected void SetLabel(Label lbl, string text, System.Drawing.Color color, bool isBold)
    {
        lbl.Text = text;
        lbl.ForeColor = color;
        if (isBold)
            lbl.Font.Bold = true;
    }

    protected bool LetterExists(int letterID)
    {
        Letter letter = LetterDB.GetByID(letterID);
        bool useDefaultDocs = letter.Organisation == null ? true : !LetterDB.OrgHasdocs(letter.Organisation.OrganisationID);

        string dir = Letter.GetLettersDirectory();
        return (File.Exists(dir + (useDefaultDocs ? "" : letter.Organisation.OrganisationID + @"\") + letter.Docname));
    }

    #endregion

    #region btnPatientUpdated_Click, btnOrganisationUpdated_Click

    protected void btnPatientUpdated_Click(object sender, EventArgs e)
    {

    }
    protected void btnOrganisationUpdated_Click(object sender, EventArgs e)
    {
        string curDocName = lstLetters.SelectedItem == null ? null : lstLetters.SelectedItem.Text;

        PopulateLettersList();

        if (curDocName != null)
        {
            for (int i = 0; i < lstLetters.Items.Count; i++)
            {
                if (lstLetters.Items[i].Text == curDocName)
                {
                    lstLetters.SelectedIndex = i;
                    break;
                }
            }
        }

    }

    #endregion

    #region ResetPatientName, ResetOrgName, btnUpdatePatient_Click, btnUpdateOrganisation_Click

    protected void ResetPatientName()
    {
        if (txtUpdatePatientID.Text.Length == 0)
        {
            txtUpdatePatientName.Text = "";
            txtUpdatePatientName.Visible = true;
            lblUpdatePatientName.Visible = false;
        }
        else
        {
            Patient patient = PatientDB.GetByID(Convert.ToInt32(txtUpdatePatientID.Text));
            txtUpdatePatientName.Text = patient.Person.FullnameWithoutMiddlename;
            txtUpdatePatientName.Visible = false;
            lblUpdatePatientName.Text = "<a href=\"#=\" onclick=\"open_new_window('PatientDetailV2.aspx?type=view&id=" + patient.PatientID + "'); return false;\">" + patient.Person.FullnameWithoutMiddlename + "</a>";
            lblUpdatePatientName.Visible = true;

        }

        UpdateTextbox(txtUpdatePatientName, lblUpdatePatientName, txtUpdatePatientID.Text.Length == 0);
    }

    protected void ResetOrgName()
    {
        if (txtUpdateOrganisationID.Text.Length == 0)
        {
            txtUpdateOrganisationName.Text = "";
            txtUpdateOrganisationName.Visible = true;
            lblUpdateOrganisationName.Visible = false;
        }
        else
        {
            Organisation org = OrganisationDB.GetByID(Convert.ToInt32(txtUpdateOrganisationID.Text));
            txtUpdateOrganisationName.Text = org.Name;
            txtUpdateOrganisationName.Visible = false;
            lblUpdateOrganisationName.Text = "<a href=\"#=\" onclick=\"open_new_window('OrganisationDetailV2.aspx?type=view&id=" + org.OrganisationID + "'); return false;\">" + org.Name + "</a>";
            lblUpdateOrganisationName.Visible = true;

        }

        UpdateTextbox(txtUpdateOrganisationName, lblUpdateOrganisationName, txtUpdateOrganisationID.Text.Length == 0);
    }

    protected void btnUpdatePatient_Click(object sender, EventArgs e)
    {
        if (txtUpdatePatientID.Text.Length > 0)
            this.lblPatientName.Text = txtUpdatePatientID.Text;
        else
            this.lblPatientName.Text = "--";
    }

    protected void btnUpdateOrganisation_Click(object sender, EventArgs e)
    {
        if (txtUpdateOrganisationID.Text.Length > 0)
            this.lblOrganisationName.Text = txtUpdateOrganisationID.Text;
        else
            this.lblOrganisationName.Text = "--";
    }

    protected void UpdateTextbox(TextBox textbox, Label label, bool isEmpty)
    {
        if (!isEmpty)
            textbox.Attributes["style"] = "border:none;background-color:transparent;color:black;";
        else
            textbox.Attributes["style"] = "width:175px;";

        textbox.Visible = isEmpty;
        label.Visible = !isEmpty;
    }

    #endregion

    #region PrintLetter

    protected void btnPrint_Click(object sender, EventArgs e)
    {

        try
        {

            //ScriptManager.RegisterClientScriptBlock(this, GetType(), "fancyBox", "alert('a');", true);  

            int letterPrintHistorySendMethodID = 1; // send by mail


            // make sure org and patient selected
            if (txtUpdatePatientID.Text.Length == 0)
                throw new CustomMessageException("Please select a patient.");
            //if (txtUpdateOrganisationID.Text.Length == 0)    //--- checking in javascript .. cuz can be blank and use site info in place of org info
            //    throw new CustomMessageException("Please select an organisation.");


            if (lstLetters.GetSelectedIndices().Length == 0)
                throw new CustomMessageException("Please select a letter.");

            // get letter and make sure it exists
            Letter letter = LetterDB.GetByID(Convert.ToInt32(lstLetters.SelectedValue));
            string sourchTemplatePath = letter.GetFullPath(Convert.ToInt32(Session["SiteID"]));
            if (!File.Exists(sourchTemplatePath))
                throw new CustomMessageException("File doesn't exist.");

            // get list of selected notes!
            ArrayList list = new ArrayList();
            foreach (RepeaterItem item in lstNotes.Items)
            {
                if (((CheckBox)item.FindControl("chkUseNote")).Checked)
                {
                    Label lblNoteID = (Label)item.FindControl("lblNoteID");
                    Note note = NoteDB.GetByID(Convert.ToInt32(lblNoteID.Text));
                    list.Add(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Treatment Note (" + note.DateAdded.ToString("dd-MM-yyyy") + "):" + Environment.NewLine + Environment.NewLine + ((Label)item.FindControl("lblOriginalText")).Text);
                }
            }
            string[] notes = (string[])list.ToArray(typeof(string));

            int bookingID = -1;
            if (Request.QueryString["booking"] != null) 
                bookingID = Convert.ToInt32(Request.QueryString["booking"]);
            if (Request.QueryString["bookingpatient"] != null)
            {
                BookingPatient bp = BookingPatientDB.GetByID(Convert.ToInt32(Request.QueryString["bookingpatient"]));
                bookingID = bp.Booking.BookingID;
            }

            Letter.SendLetter(Response,
                Letter.FileFormat.Word,  // .pdf
                SiteDB.GetByID(Convert.ToInt32(Session["SiteID"])), 
                letter.LetterID, 
                txtUpdateOrganisationID.Text == "" ? 0 : Convert.ToInt32(txtUpdateOrganisationID.Text),
                Convert.ToInt32(txtUpdatePatientID.Text),
                Convert.ToInt32(Session["StaffID"]),
                bookingID,
                -1,
                1, 
                notes,
                true, 
                letterPrintHistorySendMethodID);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }

    }

    #endregion
    
    #region EmailLetter

    protected void SendEmail_Click(Object Src, EventArgs E)
    {
        Send();
    }
    protected void Send()
    {
        string[] attachments = null;

        try
        {

            // Output.Text = FreeTextBox1.Text;

            if (txtEmailTo.Text.Trim().Length == 0)
            {
                Page.Form.DefaultFocus = txtSubject.ClientID;
                throw new CustomMessageException("Please enter an email address to send to");
            }
            if (!Utilities.IsValidEmailAddresses(txtEmailTo.Text.Trim(), false))
            {
                Page.Form.DefaultFocus = txtSubject.ClientID;
                throw new CustomMessageException("Please enter valid email address(es) to send to");
            }
            if (txtSubject.Text.Trim().Length == 0)
            {
                Page.Form.DefaultFocus = txtSubject.ClientID;
                throw new CustomMessageException("Please enter an email subject");
            }

            string to = txtEmailTo.Text;
            string subject = txtSubject.Text;
            string message = FreeTextBox1.Text;

            attachments = GetAttachments(new System.Web.UI.HtmlControls.HtmlInputFile[] { inpAttachment1, inpAttachment2, inpAttachment3 });


            // make sure org and patient selected
            if (txtUpdatePatientID.Text.Length == 0)
                throw new CustomMessageException("Please select a patient.");
            //if (txtUpdateOrganisationID.Text.Length == 0)    //--- checking in javascript .. cuz can be blank and use site info in place of org info
            //    throw new CustomMessageException("Please select an organisation.");


            Letter letter = null;
            string sourchTemplatePath = null;
            if (lstLetters.GetSelectedIndices().Length > 0)
            {
                // get letter and make sure it exists
                letter = LetterDB.GetByID(Convert.ToInt32(lstLetters.SelectedValue));
                sourchTemplatePath = letter.GetFullPath(Convert.ToInt32(Session["SiteID"]));
                if (!File.Exists(sourchTemplatePath))
                    throw new CustomMessageException("File doesn't exist.");
            }

            // get list of selected notes!
            ArrayList list = new ArrayList();
            foreach (RepeaterItem item in lstNotes.Items)
            {
                if (((CheckBox)item.FindControl("chkUseNote")).Checked)
                {
                    Label lblNoteID = (Label)item.FindControl("lblNoteID");
                    Note note = NoteDB.GetByID(Convert.ToInt32(lblNoteID.Text));
                    list.Add(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Treatment Note (" + note.DateAdded.ToString("dd-MM-yyyy") + "):" + Environment.NewLine + Environment.NewLine + ((Label)item.FindControl("lblOriginalText")).Text);
                }

            }
            string[] notes = (string[])list.ToArray(typeof(string));

            string tmpFinalFileName = null;
            if (letter != null)
            {
                tmpFinalFileName = Letter.CreateLetterAndReturnTempFile(
                    Letter.FileFormat.PDF,
                    SiteDB.GetByID(Convert.ToInt32(Session["SiteID"])),
                    letter.LetterID,
                    txtUpdateOrganisationID.Text == "" ? 0 : Convert.ToInt32(txtUpdateOrganisationID.Text),
                    Convert.ToInt32(txtUpdatePatientID.Text),
                    Convert.ToInt32(Session["StaffID"]),
                    Request.QueryString["booking"] == null ? -1 : Convert.ToInt32(Request.QueryString["booking"]),
                    -1,
                    1,
                    notes,
                    true,
                    1);

                if (attachments == null) attachments = new string[] { };
                string[] newAttachments = new string[attachments.Length + 1];
                newAttachments[0] = tmpFinalFileName;
                Array.Copy(attachments, 0, newAttachments, 1, attachments.Length);
                attachments = newAttachments;
            }

            Emailer.SimpleEmail(
                (string)Session["SiteName"],
                to,
                subject,
                message,
                true,
                attachments,
                null);

            //RemoveDraft();

            SetErrorMessage("Email Sent!");

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
            if (attachments != null)
                foreach (string file in attachments)
                    System.IO.File.Delete(file);
        }
    }
    protected string[] GetAttachments(System.Web.UI.HtmlControls.HtmlInputFile[] htmlInputFiles)
    {
        ArrayList attachments = new ArrayList();
        foreach (System.Web.UI.HtmlControls.HtmlInputFile htmlInputFile in htmlInputFiles)
        {
            if (htmlInputFile.PostedFile == null)
                continue;

            HttpPostedFile attFile = htmlInputFile.PostedFile;
            if (attFile.ContentLength == 0)
                continue;

            string strFileName = System.IO.Path.GetFileName(htmlInputFile.PostedFile.FileName);
            string hostedFilePath = Letter.GetTempLettersDirectory() + strFileName;
            htmlInputFile.PostedFile.SaveAs(hostedFilePath);  // Save the file on the server

            attachments.Add(hostedFilePath);
        }

        return (string[])attachments.ToArray(typeof(string));
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        main_table.Visible = false;
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

    protected void btnPTEmail_Click(object sender, EventArgs e)
    {
        InsertEmail(true, false);
    }
    protected void btnGPEmail_Click(object sender, EventArgs e)
    {
        InsertEmail(false, true);
    }
    protected void InsertEmail(bool PT, bool GP)
    {

        if (txtUpdatePatientID.Text.Length == 0)
        {
            SetErrorMessage("No PT Selected");
            return;
        }


        string[] emails  = null;
        string   refName = null;

        Patient patient = PatientDB.GetByID(Convert.ToInt32(txtUpdatePatientID.Text));

        if (PT)
        {
            emails = ContactDB.GetEmailsByEntityID(patient.Person.EntityID);
        }
        else if (GP)
        {
            PatientReferrer[] patRefs = PatientReferrerDB.GetActiveEPCPatientReferrersOf(patient.PatientID);
            if (patRefs.Length > 0)
            {
                refName = patRefs[0].RegisterReferrer.Referrer.Person.FullnameWithoutMiddlename;
                emails = ContactDB.GetEmailsByEntityID(patRefs[0].RegisterReferrer.Organisation.EntityID);
            }
        }

        if (emails != null && emails.Length > 0)
            txtEmailTo.Text = string.Join(",", emails);
        else
        {
            if (PT)
                SetErrorMessage("No email address set for " + patient.Person.FullnameWithoutMiddlename + "<br />");
            else if (GP && refName == null)
                SetErrorMessage("No referrer set for " + patient.Person.FullnameWithoutMiddlename + "<br />");
            else if (GP)
                SetErrorMessage("No email set for referrer " + refName + "<br />");

            txtEmailTo.Text = string.Empty;
        }
    }

}