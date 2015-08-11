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

public partial class BookingCreateInvoiceGroupV2 : System.Web.UI.Page
{
  

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            Utilities.UpdatePageHeaderV2(Page.Master, true);
            HideErrorMessage();

            if (!IsPostBack)
            {
                if (!IsValidFormBookingID())
                    throw new CustomMessageException("Invalid or no booking ID.");
                Booking booking = BookingDB.GetByID(GetFormBookingID());
                if (booking == null)
                    throw new CustomMessageException("Invalid or no booking ID.");


                // set these before 'if (!IsPostBack)' so that any errors from there are displayed instead of the below message.
                if (booking.BookingStatus.ID != 0)
                {
                    HideTableAndSetErrorMessage("Booking already set as " + booking.BookingStatus.Descr + ".");
                    btnSubmit.Visible = false;
                    btnCancel.Visible = false;
                    btnClose.Visible  = true;
                    br_close_button_space.Visible = true;
                }

                Session.Remove("patientlist_sortexpression");
                Session.Remove("patientlist_data");
                Session.Remove("bookingpatients_sortexpression");
                Session.Remove("bookingpatients_data");
                Session.Remove("bookingpatientofferings_data");


                if (!UserView.GetInstance().IsClinicView)
                    throw new CustomMessageException("Not logged into the Clinic site.");

                if (!IsValidFormBookingID())
                    throw new CustomMessageException("Invalid or no booking ID.");

                SetupGUI(booking);
                FillGrid_BookingPatients(booking);
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

    #region SetupGUI

    protected void SetupGUI(Booking booking)
    {
        string startAMPM = booking.DateStart.Hour < 12 ? "am" : "pm";
        string endAMPM   = booking.DateEnd.Hour   < 12 ? "am" : "pm";
        string timeDisplayed = startAMPM == endAMPM ?
            (booking.DateStart.Hour % 12)               + "-" + (booking.DateEnd.Hour % 12) + startAMPM :
            (booking.DateStart.Hour % 12)   + startAMPM + " - " + (booking.DateEnd.Hour % 12) + endAMPM;

        lblHeading.Text = "Booking<small><font color=\"#989898\"> at </font></small>" + booking.Organisation.Name + "<small><font color=\"#989898\"> with </font></small>" + booking.Provider.Person.FullnameWithoutMiddlename + "<small><font color=\"#989898\"> on </font></small>" + booking.DateStart.ToString("d MMM yyyy") + "<small><font color=\"#989898\"> at </font></small>" + timeDisplayed;

        bool add_patients = Request.QueryString["type"] != null && Request.QueryString["type"] == "add_patients";
        if (add_patients)
        {
            btnSubmit.Visible                  = false;
            buttonsSpace.Visible               = false;
            chkGenerateSystemLetters.Visible   = false;
            spaceGenerateSystemLetters.Visible = false;
            btnCancel.Text                     = "Close";
        }

        bool refresh_on_close = Request.QueryString["refresh_on_close"] != null && Request.QueryString["refresh_on_close"] == "1";
        if (refresh_on_close)
        {
            btnCancel.OnClientClick = "window.opener.location.href = window.opener.location.href;self.close();";

            // make sure if user clicks "x" to close the window, this value is passed on so the other page gets this value passed on too
            if (refresh_on_close) // refresh parent when parent opened this as tab
                Page.ClientScript.RegisterStartupScript(this.GetType(), "on_close_window", "<script language=javascript>window.onbeforeunload = function(){ window.opener.location.href = window.opener.location.href; }</script>");
        }
    }

    #endregion


    #region UpdateAreaTreated()

    public void UpdateAreaTreated()
    {
        DataTable dt = Session["bookingpatientofferings_data"] as DataTable;

        for (int i = 0; i < GrdBookingPatients.Rows.Count; i++)
        {
            Label    lblId            = (Label)GrdBookingPatients.Rows[i].FindControl("lblId");
            TextBox  txtAreaTreatedBP   = (TextBox)GrdBookingPatients.Rows[i].FindControl("txtAreaTreated");
            Repeater rptBkPtOfferings = (Repeater)GrdBookingPatients.Rows[i].FindControl("rptBkPtOfferings");

            if (lblId.Text != string.Empty && txtAreaTreatedBP != null)
                BookingPatientDB.UpdateAreaTreated(Convert.ToInt32(lblId.Text), txtAreaTreatedBP.Text);

            for (int j = 0; j < rptBkPtOfferings.Items.Count; j++)
            {
                HiddenField hiddenBookingPatientOfferingID = (HiddenField)rptBkPtOfferings.Items[j].FindControl("hiddenBookingPatientOfferingID");
                TextBox txtAreaTreated = (TextBox)rptBkPtOfferings.Items[j].FindControl("txtAreaTreated");

                int booking_patient_offering_id = Convert.ToInt32(hiddenBookingPatientOfferingID.Value);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    if (Convert.ToInt32(dt.Rows[k]["bpo_booking_patient_offering_id"]) == booking_patient_offering_id)
                    {
                        BookingPatientOfferingDB.UpdateAreaTreated(booking_patient_offering_id, txtAreaTreated.Text.Trim());
                        dt.Rows[k]["bpo_area_treated"] = txtAreaTreated.Text.Trim();
                    }
                }
            }
        }

        Session["bookingpatientofferings_data"] = dt;
    }

    #endregion

    #region GrdBookingPatients

    protected void FillGrid_BookingPatients(Booking booking = null)
    {
        if (booking == null)
            booking = BookingDB.GetByID(GetFormBookingID());

        bool isMobileDevice = Utilities.IsMobileDevice(Request);

        DataTable dt = BookingPatientDB.GetDataTable_ByBookingID(booking.BookingID);

        Hashtable offeringsHashPrices = GetOfferingHashtable(booking);
        Session["bookingpatientofferings_offeringshashprice_data"] = offeringsHashPrices;

        Hashtable offeringsHash       = OfferingDB.GetHashtable(true, -1, null, true);
        Session["bookingpatientofferings_offeringshash_data"] = offeringsHash;


        // get info to show EPC's remaining
        int[] patientIDs = new int[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
            patientIDs[i] = Convert.ToInt32(dt.Rows[i]["bp_patient_id"]);
        Hashtable patientHealthCardCache        = PatientsHealthCardsCacheDB.GetBullkActive(patientIDs);
        Hashtable epcRemainingCache             = GetEPCRemainingCache(patientHealthCardCache);
        Hashtable patientsMedicareCountCache    = PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, DateTime.Today.Year);
        Hashtable patientsEPCRemainingCache     = PatientsEPCRemainingCacheDB.GetBullk(patientIDs, DateTime.Today.AddYears(-1));
        int       MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);



        dt.Columns.Add("epc_org_name"        , typeof(String));
        dt.Columns.Add("epc_org_id"          , typeof(Int32));
        dt.Columns.Add("epc_expire_date"     , typeof(DateTime));
        dt.Columns.Add("has_valid_epc"       , typeof(Boolean));
        dt.Columns.Add("epc_count_remaining" , typeof(Int32));
        dt.Columns.Add("epc_text"            , typeof(String));

        dt.Columns.Add("show_area_treated"   , typeof(Boolean));
        dt.Columns.Add("is_dva", typeof(Boolean));
        dt.Columns.Add("is_tac", typeof(Boolean));

        dt.Columns.Add("notes_text", typeof(string));


        for (int i = 0; i < dt.Rows.Count; i++)
        {

            // add in epc info

            int patientID = Convert.ToInt32(dt.Rows[i]["bp_patient_id"]);

            HealthCard               hc                       = GetHealthCardFromCache(patientHealthCardCache, patientID);
            bool                     hasEPC                   = hc != null && hc.DateReferralSigned != DateTime.MinValue;
            HealthCardEPCRemaining[] epcsRemaining            = !hasEPC ? new HealthCardEPCRemaining[] { } : GetEPCRemainingFromCache(epcRemainingCache, hc, booking.Provider.Field.ID);
            int                      totalServicesAllowedLeft = !hasEPC ? 0 : (MedicareMaxNbrServicesPerYear - (int)patientsMedicareCountCache[patientID]);
            int                      totalEpcsRemaining       = epcsRemaining.Sum(o => o.NumServicesRemaining);
            DateTime                 referralSignedDate       = !hasEPC ? DateTime.MinValue : hc.DateReferralSigned.Date;
            DateTime                 hcExpiredDate            = !hasEPC ? DateTime.MinValue : referralSignedDate.AddYears(1);
            bool                     isExpired                = !hasEPC ? true              : hcExpiredDate <= DateTime.Today;

            int nServicesLeft = 0;
            if (hc != null && DateTime.Today >= referralSignedDate.Date && DateTime.Today < hcExpiredDate.Date)
                nServicesLeft = totalEpcsRemaining;
            if (hc != null && totalServicesAllowedLeft < nServicesLeft)
                nServicesLeft = totalServicesAllowedLeft;

            bool has_valid_epc       = hasEPC && !isExpired && (hc.Organisation.OrganisationID == -2 || (hc.Organisation.OrganisationID == -1 && nServicesLeft > 0));
            int  epc_count_remaining = hasEPC && hc.Organisation.OrganisationID == -1 ? nServicesLeft : -1;

            dt.Rows[i]["epc_org_id"]          = hasEPC ? (object)(hc.Organisation.OrganisationID == -1 ? -1   : -2)    : DBNull.Value;
            dt.Rows[i]["epc_org_name"]        = hasEPC ? (object)(hc.Organisation.OrganisationID == -1 ? "MC" : "DVA") : DBNull.Value;
            dt.Rows[i]["has_valid_epc"]       = has_valid_epc;
            dt.Rows[i]["epc_expire_date"]     = hasEPC ? hcExpiredDate : (object)DBNull.Value;
            dt.Rows[i]["epc_count_remaining"] = epc_count_remaining != -1 ? epc_count_remaining : (object)DBNull.Value;
            dt.Rows[i]["epc_text"] = !hasEPC ? (object)DBNull.Value :
                (
                    "<div style=\"height:6px;\"></div>EPC: " + (hc.Organisation.OrganisationID == -1 ? "Medicare" : "DVA") + "<br />" +
                    (isExpired ? "<font color=\"red\">" : "") + "Exp. " + hcExpiredDate.ToString("dd-MM-yyyy") + (isExpired ? "</font>" : "") + "<br />" +
                    (epc_count_remaining == 0 ? "<font color=\"red\">" : "") + (hc.Organisation.OrganisationID == -1 ? "Remaining: " + epc_count_remaining.ToString() : "") + (epc_count_remaining == 0 ? "</font>" : "")
                );

            if (hc != null && hc.Organisation.OrganisationType.OrganisationTypeID == 150)
            {
                hc.Organisation = OrganisationDB.GetByID(hc.Organisation.OrganisationID);

                dt.Rows[i]["epc_org_id"]          = hc.Organisation.OrganisationID;
                dt.Rows[i]["epc_org_name"]        = hc.Organisation.Name;
                dt.Rows[i]["has_valid_epc"]       = true;
                dt.Rows[i]["epc_expire_date"]     = (object)DBNull.Value;
                dt.Rows[i]["epc_count_remaining"] = (object)DBNull.Value;
                dt.Rows[i]["epc_text"]            = "<div style=\"height:6px;\"></div>Ins.: " + hc.Organisation.Name;
            }

            dt.Rows[i]["show_area_treated"] = hc != null && ((hc.Organisation.OrganisationID == -2 && has_valid_epc) || hc.Organisation.OrganisationType.OrganisationTypeID == 150);
            dt.Rows[i]["is_dva"]            = hc != null && hc.Organisation.OrganisationID == -2 && has_valid_epc;
            dt.Rows[i]["is_tac"]            = hc != null && hc.Organisation.OrganisationType.OrganisationTypeID == 150;

            dt.Rows[i]["notes_text"] = Note.GetBookingPopupLinkTextV2(Convert.ToInt32(dt.Rows[i]["bp_entity_id"]), Convert.ToInt32(dt.Rows[i]["bp_note_count"]) > 0, true, 1425, 700, "images/notes-bw-24.jpg", "images/notes-24.png", "btnUpdateBookingList.click()", !isMobileDevice);
        }

        Session["bookingpatients_data"] = dt;


        int[] bookingPatientIDs = new int[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
            bookingPatientIDs[i] = Convert.ToInt32(dt.Rows[i]["bp_booking_patient_id"]);

        // replace prices with that of this org
        DataTable dt_bk_pt_offerings = BookingPatientOfferingDB.GetDataTable(bookingPatientIDs);
        for (int i = 0; i < dt_bk_pt_offerings.Rows.Count; i++)
        {
            int offering_id = Convert.ToInt32(dt_bk_pt_offerings.Rows[i]["offering_offering_id"]);
            if (offeringsHashPrices[offering_id] != null)
                dt_bk_pt_offerings.Rows[i]["offering_default_price"] = ((Offering)offeringsHashPrices[offering_id]).DefaultPrice;
        }

        Session["bookingpatientofferings_data"] = dt_bk_pt_offerings;


        if (dt.Rows.Count > 0)
        {
            GrdBookingPatients.DataSource = dt;
            try
            {
                GrdBookingPatients.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }

            btnSubmit.OnClientClick = "";
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdBookingPatients.DataSource = dt;
            GrdBookingPatients.DataBind();

            int TotalColumns = GrdBookingPatients.Rows[0].Cells.Count;
            GrdBookingPatients.Rows[0].Cells.Clear();
            GrdBookingPatients.Rows[0].Cells.Add(new TableCell());
            GrdBookingPatients.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdBookingPatients.Rows[0].Cells[0].Text = "No Record Found";

            btnSubmit.OnClientClick = "alert('No patients added');return false;";
        }
    }
    protected void GrdBookingPatients_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {
            e.Row.Cells[0].CssClass = "hiddencol";
            e.Row.Cells[1].CssClass = "hiddencol";
        }
    }
    protected void GrdBookingPatients_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["bookingpatients_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label     lblId        = (Label)e.Row.FindControl("lblId");
            Label     lblPatientId = (Label)e.Row.FindControl("lblPatientId");
            DataRow[] foundRows    = dt.Select("bp_booking_patient_id=" + lblId.Text);
            DataRow   thisRow      = foundRows[0];

            int  booking_patient_id = Convert.ToInt32(lblId.Text);
            bool showAreaTreated    = (bool)thisRow["show_area_treated"];
            bool isDVA = (bool)thisRow["is_dva"];
            bool isTAC = (bool)thisRow["is_tac"];


            Label lblName = (Label)e.Row.FindControl("lblName");
            if (lblName != null)
                lblName.Text = "<a href='PatientDetailV2.aspx?type=view&id=" + thisRow["patient_patient_id"] + "' onclick=\"open_new_tab('PatientDetailV2.aspx?type=view&id=" + thisRow["patient_patient_id"] + "');return false;\">" + thisRow["patient_person_firstname"] + " " + thisRow["patient_person_surname"] + "</a>";

            Repeater rptBkPtOfferings = (Repeater)e.Row.FindControl("rptBkPtOfferings");
            if (rptBkPtOfferings != null)
            {
                Hashtable offeringsHashPrices = (Hashtable)Session["bookingpatientofferings_offeringshashprice_data"];

                DataTable dt_allbkptofferings = Session["bookingpatientofferings_data"] as DataTable;
                DataTable dt_bkptofferings = dt_allbkptofferings.Clone(); // copy structure
                for (int i = 0; i < dt_allbkptofferings.Rows.Count; i++)
                    if (Convert.ToInt32(dt_allbkptofferings.Rows[i]["bpo_booking_patient_id"]) == booking_patient_id)
                        dt_bkptofferings.Rows.Add(dt_allbkptofferings.Rows[i].ItemArray);


                dt_bkptofferings.Columns.Add("show_area_treated", typeof(Boolean));
                dt_bkptofferings.Columns.Add("area_treated", typeof(String));
                dt_bkptofferings.Columns.Add("is_dva", typeof(Boolean));
                dt_bkptofferings.Columns.Add("is_tac", typeof(Boolean));
                for (int i = 0; i < dt_bkptofferings.Rows.Count; i++)
                {
                    dt_bkptofferings.Rows[i]["show_area_treated"] = showAreaTreated;
                    dt_bkptofferings.Rows[i]["area_treated"] = string.Empty;
                    dt_bkptofferings.Rows[i]["is_dva"] = isDVA;
                    dt_bkptofferings.Rows[i]["is_tac"] = isTAC;
                }



                DataView dv = dt_bkptofferings.DefaultView;
                dv.Sort = "offering_name";
                dt_bkptofferings = dv.ToTable();

                //DataTable dt_bkptofferings = BookingPatientOfferingDB.GetDataTable_ByBookingPatientID(Convert.ToInt32(lblId.Text));
                rptBkPtOfferings.DataSource = dt_bkptofferings;
                rptBkPtOfferings.DataBind();
            }


            System.Web.UI.HtmlControls.HtmlControl space_before_offerings_ddl = (System.Web.UI.HtmlControls.HtmlControl)e.Row.FindControl("space_before_offerings_ddl");
            DropDownList ddlOfferings = (DropDownList)e.Row.FindControl("ddlOfferings");
            System.Web.UI.HtmlControls.HtmlControl space_before_addcancelbuttons = (System.Web.UI.HtmlControls.HtmlControl)e.Row.FindControl("space_before_addcancelbuttons");
            Button btnBkPtOfferingsAdd = (Button)e.Row.FindControl("btnBkPtOfferingsAdd");
            Button btnBkPtOfferingsCancelAdd = (Button)e.Row.FindControl("btnBkPtOfferingsCancelAdd");

            

            List<int> bkPtIDsVisible = (hiddenViewDdlBookingPatientIDs.Value.Length == 0) ? new List<int> { } : hiddenViewDdlBookingPatientIDs.Value.Split(',').Select(int.Parse).ToList();
            if (bkPtIDsVisible.Contains(booking_patient_id))
            {
                DataTable dt_all_offerings = OfferingDB.GetDataTable(false, "1,3", "63,89");  // 1=Clinic 3=Clinic&AC, 4=AC  // 63=Services,89=Products
                ddlOfferings.Style["max-width"] = "250px";
                ddlOfferings.Items.Clear();
                ddlOfferings.DataSource     = dt_all_offerings;
                ddlOfferings.DataTextField  = "o_name";
                ddlOfferings.DataValueField = "o_offering_id";
                ddlOfferings.DataBind();

                ddlOfferings.Visible = space_before_offerings_ddl.Visible = true;
                space_before_addcancelbuttons.Visible = true;
                btnBkPtOfferingsCancelAdd.Visible = true;

                btnBkPtOfferingsAdd.CommandName = "AddOffering";
                btnBkPtOfferingsAdd.CommandArgument = booking_patient_id.ToString();

                btnBkPtOfferingsCancelAdd.CommandName = "CancelAddOffering";
                btnBkPtOfferingsCancelAdd.CommandArgument = booking_patient_id.ToString();
            }
            else
            {
                ddlOfferings.Items.Clear();
                ddlOfferings.Visible = space_before_offerings_ddl.Visible = false;

                space_before_addcancelbuttons.Visible = false;
                btnBkPtOfferingsCancelAdd.Visible = false;

                btnBkPtOfferingsAdd.CommandName = "ShowOfferingsDLL";
                btnBkPtOfferingsAdd.CommandArgument = booking_patient_id.ToString();
            }


            DropDownList ddlBPOffering = (DropDownList)e.Row.FindControl("ddlBPOffering");
            if (ddlBPOffering != null)
            {
                DataTable dt_all_offerings = OfferingDB.GetDataTable(false, "1,3", "63,89");  // 1=Clinic 3=Clinic&AC, 4=AC  // 63=Services,89=Products
                ddlBPOffering.Style["max-width"] = "250px";
                ddlBPOffering.Items.Clear();
                ddlBPOffering.DataSource = dt_all_offerings;
                ddlBPOffering.DataTextField = "o_name";
                ddlBPOffering.DataValueField = "o_offering_id";
                ddlBPOffering.DataBind();

                if (thisRow["bp_offering_id"] != DBNull.Value)
                    ddlBPOffering.SelectedValue = thisRow["bp_offering_id"].ToString();
            }



            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

        }
    }
    protected void GrdBookingPatients_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        UpdateAreaTreated();

        GrdBookingPatients.EditIndex = -1;
        FillGrid_BookingPatients();
    }
    protected void GrdBookingPatients_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        UpdateAreaTreated();

        Label lblId = (Label)GrdBookingPatients.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlBPOffering = (DropDownList)GrdBookingPatients.Rows[e.RowIndex].FindControl("ddlBPOffering");


        DataTable dt = Session["bookingpatients_data"] as DataTable;
        DataRow[] foundRows = dt.Select("bp_booking_patient_id=" + lblId.Text);
        DataRow row = foundRows[0];

        BookingPatientDB.UpdateOffering(Convert.ToInt32(lblId.Text), Convert.ToInt32(ddlBPOffering.SelectedValue));

        GrdBookingPatients.EditIndex = -1;
        FillGrid_BookingPatients();
    }
    protected void GrdBookingPatients_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        UpdateAreaTreated();

        Label lblId = (Label)GrdBookingPatients.Rows[e.RowIndex].FindControl("lblId");

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

        FillGrid_BookingPatients();
    }
    protected void GrdBookingPatients_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Remove"))
        {
            UpdateAreaTreated();

            int booking_patient_id = Convert.ToInt32(e.CommandArgument);
            BookingPatientDB.UpdateInactive(booking_patient_id, Convert.ToInt32(Session["StaffID"]));
            FillGrid_BookingPatients();
        }

        if (e.CommandName.Equals("AddOffering"))
        {
            UpdateAreaTreated();

            int booking_patient_id = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = (GridViewRow)(((Control)e.CommandSource).NamingContainer);
            DropDownList ddlOfferings = (DropDownList)row.FindControl("ddlOfferings");


            bool alreadyExists = false;
            DataTable dt_allbkptofferings = Session["bookingpatientofferings_data"] as DataTable;
            for (int i = 0; i < dt_allbkptofferings.Rows.Count; i++)
                if (Convert.ToInt32(dt_allbkptofferings.Rows[i]["bpo_booking_patient_id"]) == booking_patient_id &&
                    Convert.ToInt32(dt_allbkptofferings.Rows[i]["bpo_offering_id"])        == Convert.ToInt32(ddlOfferings.SelectedValue))
                        alreadyExists = true;

            if (!alreadyExists)
            {
                BookingPatient bp = BookingPatientDB.GetByID(booking_patient_id);

                HealthCard hc                 = HealthCardDB.GetActiveByPatientID(bp.Patient.PatientID);
                bool       hasEPC             = hc != null && hc.DateReferralSigned != DateTime.MinValue;
                DateTime   referralSignedDate = !hasEPC ? DateTime.MinValue : hc.DateReferralSigned.Date;
                DateTime   hcExpiredDate      = !hasEPC ? DateTime.MinValue : referralSignedDate.AddYears(1);
                bool       isExpired          = !hasEPC ? true              : hcExpiredDate <= DateTime.Today;
                bool       has_valid_dva_epc  = hasEPC && !isExpired && (hc.Organisation.OrganisationID == -2);
                bool       show_area_treated  = hc != null && ((hc.Organisation.OrganisationID == -2 && has_valid_dva_epc) || hc.Organisation.OrganisationType.OrganisationTypeID == 150); ;
                string     areaTreated        = show_area_treated ? hc.AreaTreated : string.Empty;
                BookingPatientOfferingDB.Insert(booking_patient_id, Convert.ToInt32(ddlOfferings.SelectedValue), 1, Convert.ToInt32(Session["StaffID"]), areaTreated);
            }

            hiddenViewDdlBookingPatientIDs.Value = "";
            FillGrid_BookingPatients();
        }

        if (e.CommandName.Equals("CancelAddOffering"))
        {
            UpdateAreaTreated();

            int booking_patient_id = Convert.ToInt32(e.CommandArgument);

            hiddenViewDdlBookingPatientIDs.Value = "";
            FillGrid_BookingPatients();
        }
        
        if (e.CommandName.Equals("ShowOfferingsDLL"))
        {
            UpdateAreaTreated();

            int booking_patient_id = Convert.ToInt32(e.CommandArgument);
            hiddenViewDdlBookingPatientIDs.Value = booking_patient_id.ToString();
            FillGrid_BookingPatients();
        }

    }
    protected void GrdBookingPatients_RowEditing(object sender, GridViewEditEventArgs e)
    {
        UpdateAreaTreated();

        GrdBookingPatients.EditIndex = e.NewEditIndex;
        FillGrid_BookingPatients();
    }
    protected void GrdBookingPatients_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdBookingPatients.EditIndex >= 0)
            return;

        GrdBookingPatients_Sort(e.SortExpression);
    }

    protected void GrdBookingPatients_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["bookingpatients_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["bookingpatients_sortexpression"] == null)
                Session["bookingpatients_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["bookingpatients_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["bookingpatients_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdBookingPatients.DataSource = dataView;
            GrdBookingPatients.DataBind();
        }
    }

    protected void btnRemoveBookingPatientOffering_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName.Equals("RemoveBookingPatientOffering"))
        {
            UpdateAreaTreated();

            int booking_patient_offering_id = Convert.ToInt32(e.CommandArgument);
            BookingPatientOfferingDB.UpdateInactive(booking_patient_offering_id, Convert.ToInt32(Session["StaffID"]));
            FillGrid_BookingPatients();
        }

        if (e.CommandName.Equals("AddQty"))
        {
            UpdateAreaTreated();

            int booking_patient_offering_id = Convert.ToInt32(e.CommandArgument);
            BookingPatientOffering bpo = BookingPatientOfferingDB.GetByID(booking_patient_offering_id);
            BookingPatientOfferingDB.UpdateQuantity(booking_patient_offering_id, bpo.Quantity + 1);
            FillGrid_BookingPatients();
        }

        if (e.CommandName.Equals("SubtractQty"))
        {
            UpdateAreaTreated();

            int booking_patient_offering_id = Convert.ToInt32(e.CommandArgument);
            BookingPatientOffering bpo = BookingPatientOfferingDB.GetByID(booking_patient_offering_id);
            if (bpo.Quantity > 1)
                BookingPatientOfferingDB.UpdateQuantity(booking_patient_offering_id, bpo.Quantity - 1);
            FillGrid_BookingPatients();
        }
    }

    #endregion

    #region GetHealthCardFromCache, GetEPCRemainingCache, GetEPCRemainingFromCache

    protected HealthCard GetHealthCardFromCache(Hashtable patientHealthCardCache, int patientID)
    {
        HealthCard hc = null;
        if (patientHealthCardCache[patientID] != null)
        {
            PatientActiveHealthCards hcs = (PatientActiveHealthCards)patientHealthCardCache[patientID];
            if (hcs.MedicareCard != null)
                hc = hcs.MedicareCard;
            if (hcs.DVACard != null)
                hc = hcs.DVACard;
            if (hcs.TACCard != null)
                hc = hcs.TACCard;
        }

        return hc;
    }

    protected Hashtable GetEPCRemainingCache(Hashtable patientHealthCardCache)
    {
        ArrayList healthCardIDs = new ArrayList();
        foreach (PatientActiveHealthCards ptHCs in patientHealthCardCache.Values)
        {
            if (ptHCs.MedicareCard != null)
                healthCardIDs.Add(ptHCs.MedicareCard.HealthCardID);
            if (ptHCs.DVACard != null)
                healthCardIDs.Add(ptHCs.DVACard.HealthCardID);
        }

        return HealthCardEPCRemainingDB.GetHashtableByHealthCardIDs((int[])healthCardIDs.ToArray(typeof(int)));
    }

    protected HealthCardEPCRemaining[] GetEPCRemainingFromCache(Hashtable epcRemainingCache, HealthCard hc, int fieldID)
    {
        if (hc == null)
            return new HealthCardEPCRemaining[] { };

        HealthCardEPCRemaining[] epcsRemainingForThisType = null;
        if (epcRemainingCache == null)
        {
            epcsRemainingForThisType = HealthCardEPCRemainingDB.GetByHealthCardID(hc.HealthCardID, fieldID);
        }
        else
        {
            ArrayList remainingThisField = new ArrayList();
            HealthCardEPCRemaining[] allRemainingThisHealthCareCard = (HealthCardEPCRemaining[])epcRemainingCache[hc.HealthCardID];
            if (allRemainingThisHealthCareCard != null)
                for (int i = 0; i < allRemainingThisHealthCareCard.Length; i++)
                    if (allRemainingThisHealthCareCard[i].Field.ID == fieldID)
                        remainingThisField.Add(allRemainingThisHealthCareCard[i]);
            epcsRemainingForThisType = (HealthCardEPCRemaining[])remainingThisField.ToArray(typeof(HealthCardEPCRemaining));
        }

        return epcsRemainingForThisType == null ? new HealthCardEPCRemaining[] { } : epcsRemainingForThisType;
    }

    #endregion


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        UpdateAreaTreated();

        try
        {
            ///////////////
            // validation
            ///////////////

            Booking booking = BookingDB.GetByID(GetFormBookingID());
            if (booking == null)
                throw new CustomMessageException("Invalid booking");
            if (booking.BookingStatus.ID != 0)
                throw new CustomMessageException("Booking already set as : " + BookingDB.GetStatusByID(booking.BookingStatus.ID).Descr);
            if (InvoiceDB.GetCountByBookingID(booking.BookingID) > 0) // shouldnt get here since should have been set as completed and thrown in error above
                throw new CustomMessageException("Booking already has an invoice");



            ///////////////////
            // create invoice
            ///////////////////


            decimal GST_Percent = Convert.ToDecimal(((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["GST_Percent"].Value);


            // keep id's to delete if exception and need to roll back

            ArrayList invIDs     = new ArrayList();
            ArrayList invLineIDs = new ArrayList();


            // get list of patients and associated info

            BookingPatient[] bookingPatients = BookingPatientDB.GetByBookingID(booking.BookingID);
            if (bookingPatients.Length == 0)
                throw new CustomMessageException("No patients added");
            Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
            PatientDB.AddACOfferings(ref offeringsHash, ref bookingPatients);

            int[] patientIDs        = bookingPatients.Select(o => o.Patient.PatientID).ToArray();
            int[] bookingPatientIDs = bookingPatients.Select(o => o.BookingPatientID).ToArray();

            // caches for less db lookups to handle many patients
            Hashtable bpoHash                       = BookingPatientOfferingDB.GetHashtable(bookingPatientIDs);
            Hashtable patientHealthCardCache        = PatientsHealthCardsCacheDB.GetBullkActive(patientIDs);
            Hashtable epcRemainingCache             = GetEPCRemainingCache(patientHealthCardCache);
            Hashtable patientsMedicareCountCache    = PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, DateTime.Today.Year);
            Hashtable patientsEPCRemainingCache     = PatientsEPCRemainingCacheDB.GetBullk(patientIDs, DateTime.Today.AddYears(-1));
            Hashtable patientReferrerCache          = PatientReferrerDB.GetEPCReferrersOf(patientIDs, true);
            int       MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);

            Hashtable offeringsHashPrices = GetOfferingHashtable(booking);



            int       MC_Invoice_NextID    = 1;
            ArrayList MC_Invoices          = new ArrayList();
            Hashtable MC_InvoiceLines      = new Hashtable();

            ArrayList EPCRemaining_Changes = new ArrayList(); // Tuple<int,int,int> (epcRemaining.ID, curNum, newNum)
            ArrayList EPCRefLetterInfo     = new ArrayList(); // Tuple<BookingPatient, int, RegisterReferrer, Booking.InvoiceType, Healthcard, int> (bookingPatient, fieldID, ptReferrer, invType, hc, epcremainingAfter)


            int       DVA_Invoice_NextID   = 1;
            ArrayList DVA_Invoices         = new ArrayList();
            Hashtable DVA_InvoiceLines     = new Hashtable();

            int       TAC_Invoice_NextID   = 1;
            ArrayList TAC_Invoices         = new ArrayList();
            Hashtable TAC_InvoiceLines     = new Hashtable();

            int       Prv_Invoice_NextID   = 1;
            ArrayList Prv_Invoices         = new ArrayList();
            Hashtable Prv_InvoiceLines     = new Hashtable();


            // used to check update stock and check warning level emails sent
            ArrayList invoiceLinesExtras = new ArrayList();

            for (int i = 0; i < bookingPatients.Length; i++)
            {
                Patient                  patient = bookingPatients[i].Patient;
                BookingPatient           bp      = bookingPatients[i];
                BookingPatientOffering[] bpos    = bpoHash[bp.BookingPatientID] == null ? new BookingPatientOffering[] { } : (BookingPatientOffering[])((ArrayList)bpoHash[bp.BookingPatientID]).ToArray(typeof(BookingPatientOffering));

                HealthCard               hc                       = GetHealthCardFromCache(patientHealthCardCache, patient.PatientID);
                bool                     hasEPC                   = hc != null && hc.DateReferralSigned != DateTime.MinValue;
                HealthCardEPCRemaining[] epcsRemaining            = !hasEPC ? new HealthCardEPCRemaining[] { } : GetEPCRemainingFromCache(epcRemainingCache, hc, booking.Provider.Field.ID);
                int                      totalServicesAllowedLeft = !hasEPC ? 0 : (MedicareMaxNbrServicesPerYear - (int)patientsMedicareCountCache[patient.PatientID]);
                int                      totalEpcsRemaining       = epcsRemaining.Sum(o => o.NumServicesRemaining);
                DateTime                 referralSignedDate       = !hasEPC ? DateTime.MinValue : hc.DateReferralSigned.Date;
                DateTime                 hcExpiredDate            = !hasEPC ? DateTime.MinValue : referralSignedDate.AddYears(1);
                bool                     isExpired                = !hasEPC ? true              : hcExpiredDate <= DateTime.Today;


                Booking.InvoiceType invType = booking.GetInvoiceType(hc, bp.Offering, bp.Patient, patientsMedicareCountCache, epcRemainingCache, MedicareMaxNbrServicesPerYear);


                // change to use org specific price
                for (int j = 0; j < bpos.Length; j++)
                    if (offeringsHashPrices[bpos[j].Offering.OfferingID] != null)
                        bpos[j].Offering = (Offering)offeringsHashPrices[bpos[j].Offering.OfferingID];
                if (offeringsHashPrices[bp.Offering.OfferingID] != null)
                    bp.Offering = (Offering)offeringsHashPrices[bp.Offering.OfferingID];



// RUN IT AND TRY IT!!!
// Area Treated needed for main service -- in both AC and Group Classes...


                if (invType == Booking.InvoiceType.Medicare)
                {
                    MC_Invoices.Add(new Invoice(MC_Invoice_NextID, -1, 363, booking.BookingID, -1, -1, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, bp.Offering.MedicareCharge, 0, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));

                    MC_InvoiceLines[MC_Invoice_NextID] = new ArrayList();
                    ((ArrayList)MC_InvoiceLines[MC_Invoice_NextID]).Add(new InvoiceLine(-1, MC_Invoice_NextID, bp.Patient.PatientID, bp.Offering.OfferingID, 1, bp.Offering.MedicareCharge, 0, "", "", -1));  // HERE

                    MC_Invoice_NextID++;

                    int newEpcRemainingCount = -1;
                    for (int j = 0; j < epcsRemaining.Length; j++)
                        if (epcsRemaining[j].Field.ID == booking.Provider.Field.ID)
                            if (epcsRemaining[j].NumServicesRemaining > 0)
                            {
                                EPCRemaining_Changes.Add( new Tuple<int,int,int>(epcsRemaining[j].HealthCardEpcRemainingID, epcsRemaining[j].NumServicesRemaining, epcsRemaining[j].NumServicesRemaining - 1) );
                                newEpcRemainingCount = epcsRemaining[j].NumServicesRemaining - 1;
                            }

                    RegisterReferrer[] regRefs = (RegisterReferrer[])patientReferrerCache[bp.Patient.PatientID];
                    if (regRefs != null && regRefs.Length > 0)
                        EPCRefLetterInfo.Add(new Tuple<BookingPatient, int, RegisterReferrer, Booking.InvoiceType, HealthCard, int>(bp, booking.Provider.Field.ID, regRefs[regRefs.Length - 1], invType, hc, newEpcRemainingCount));


                    //
                    // now add extras to seperate private invoice for the patient
                    //

                    if (bpos.Length > 0)
                    {
                        decimal total = 0;
                        decimal gst   = 0;
                        for (int j = 0; j < bpos.Length; j++)
                        {
                            total += bpos[j].Quantity * bpos[j].Offering.DefaultPrice;
                            gst   += bpos[j].Quantity * bpos[j].Offering.DefaultPrice * (bpos[j].Offering.IsGstExempt ? (decimal)0 : (GST_Percent) / (decimal)100);
                        }

                        MC_Invoices.Add(new Invoice(MC_Invoice_NextID, -1, 363, booking.BookingID, 0, bp.Patient.PatientID, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, total + gst, gst, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));

                        MC_InvoiceLines[MC_Invoice_NextID] = new ArrayList();
                        for (int j = 0; j < bpos.Length; j++)
                        {
                            decimal bpos_total = Convert.ToDecimal(bpos[j].Quantity) * bpos[j].Offering.DefaultPrice;
                            decimal bpos_gst   = bpos_total * (bpos[j].Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                            InvoiceLine invoiceLine = new InvoiceLine(-1, MC_Invoice_NextID, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst , "", "", -1);  // HERE
                            ((ArrayList)MC_InvoiceLines[MC_Invoice_NextID]).Add(invoiceLine);
                            invoiceLinesExtras.Add(invoiceLine);
                        }

                        MC_Invoice_NextID++;
                    }
                }
                else if (invType == Booking.InvoiceType.DVA)
                {
                    decimal total = bp.Offering.DvaCharge;
                    decimal gst   = total * (bp.Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        total += bpos[j].Quantity * (bpos[j].Offering.DvaCompanyCode.Length > 0 ? bpos[j].Offering.DvaCharge : bpos[j].Offering.DefaultPrice);
                        gst   += bpos[j].Quantity * (bpos[j].Offering.DvaCompanyCode.Length > 0 ? bpos[j].Offering.DvaCharge : bpos[j].Offering.DefaultPrice) * (bpos[j].Offering.IsGstExempt ? (decimal)0 : (GST_Percent) / (decimal)100);
                    }

                    DVA_Invoices.Add(new Invoice(DVA_Invoice_NextID, -1, 363, booking.BookingID, -2, -1, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, total + gst, gst, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));

                    DVA_InvoiceLines[DVA_Invoice_NextID] = new ArrayList();
                    decimal line_total = bp.Offering.DvaCharge;
                    decimal line_gst   = line_total * (bp.Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    ((ArrayList)DVA_InvoiceLines[DVA_Invoice_NextID]).Add(new InvoiceLine(-1, DVA_Invoice_NextID, bp.Patient.PatientID, bp.Offering.OfferingID, 1, line_total + line_gst, line_gst, bp.AreaTreated, "", -1));  // HERE
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        decimal bpos_total = Convert.ToDecimal(bpos[j].Quantity) * (bpos[j].Offering.DvaCompanyCode.Length > 0 ? bpos[j].Offering.DvaCharge : bpos[j].Offering.DefaultPrice);
                        decimal bpos_gst   = bpos_total * (bpos[j].Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                        InvoiceLine invoiceLine = new InvoiceLine(-1, DVA_Invoice_NextID, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst, bpos[j].AreaTreated, "", -1);  // HERE
                        ((ArrayList)DVA_InvoiceLines[DVA_Invoice_NextID]).Add(invoiceLine);
                        invoiceLinesExtras.Add(invoiceLine);
                    }

                    DVA_Invoice_NextID++;


                    RegisterReferrer[] regRefs = (RegisterReferrer[])patientReferrerCache[bp.Patient.PatientID];
                    if (regRefs != null && regRefs.Length > 0)
                        EPCRefLetterInfo.Add(new Tuple<BookingPatient, int, RegisterReferrer, Booking.InvoiceType, HealthCard, int>(bp, booking.Provider.Field.ID, regRefs[regRefs.Length - 1], invType, hc, -1));
                }
                else if (invType == Booking.InvoiceType.Insurance)
                {
                    decimal total = bp.Offering.TacCharge;
                    decimal gst   = total * (bp.Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        total += bpos[j].Quantity * (bpos[j].Offering.TacCompanyCode.Length > 0 ? bpos[j].Offering.TacCharge : bpos[j].Offering.DefaultPrice);
                        gst   += bpos[j].Quantity * (bpos[j].Offering.TacCompanyCode.Length > 0 ? bpos[j].Offering.TacCharge : bpos[j].Offering.DefaultPrice) * (bpos[j].Offering.IsGstExempt ? (decimal)0 : (GST_Percent) / (decimal)100);
                    }

                    TAC_Invoices.Add(new Invoice(TAC_Invoice_NextID, -1, 363, booking.BookingID, hc.Organisation.OrganisationID, -1, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, total + gst, gst, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));

                    TAC_InvoiceLines[TAC_Invoice_NextID] = new ArrayList();
                    decimal line_total = bp.Offering.TacCharge;
                    decimal line_gst = line_total * (bp.Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    ((ArrayList)TAC_InvoiceLines[TAC_Invoice_NextID]).Add(new InvoiceLine(-1, TAC_Invoice_NextID, bp.Patient.PatientID, bp.Offering.OfferingID, 1, line_total + line_gst, line_gst, bp.AreaTreated, "", -1));  // HERE
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        decimal bpos_total = Convert.ToDecimal(bpos[j].Quantity) * (bpos[j].Offering.TacCompanyCode.Length > 0 ? bpos[j].Offering.TacCharge : bpos[j].Offering.DefaultPrice);
                        decimal bpos_gst   = bpos_total * (bpos[j].Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                        InvoiceLine invoiceLine = new InvoiceLine(-1, TAC_Invoice_NextID, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst, bpos[j].AreaTreated, "", -1);  // HERE
                        ((ArrayList)TAC_InvoiceLines[TAC_Invoice_NextID]).Add(invoiceLine);
                        invoiceLinesExtras.Add(invoiceLine);
                    }

                    TAC_Invoice_NextID++;


                    RegisterReferrer[] regRefs = (RegisterReferrer[])patientReferrerCache[bp.Patient.PatientID];
                    if (regRefs != null && regRefs.Length > 0)
                        EPCRefLetterInfo.Add(new Tuple<BookingPatient, int, RegisterReferrer, Booking.InvoiceType, HealthCard, int>(bp, booking.Provider.Field.ID, regRefs[regRefs.Length - 1], invType, hc, -1));
                }
                else // private invoice
                {
                    decimal total = bp.Offering.DefaultPrice;
                    decimal gst   = total * (bp.Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        total += bpos[j].Quantity * bpos[j].Offering.DefaultPrice;
                        gst   += bpos[j].Quantity * bpos[j].Offering.DefaultPrice * (bpos[j].Offering.IsGstExempt ? (decimal)0 : (GST_Percent) / (decimal)100);
                    }

                    Prv_Invoices.Add(new Invoice(Prv_Invoice_NextID, -1, 363, booking.BookingID, 0, bp.Patient.PatientID, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, total + gst, gst, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));

                    Prv_InvoiceLines[Prv_Invoice_NextID] = new ArrayList();
                    decimal line_total = bp.Offering.DefaultPrice;
                    decimal line_gst   = line_total * (bp.Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    ((ArrayList)Prv_InvoiceLines[Prv_Invoice_NextID]).Add(new InvoiceLine(-1, Prv_Invoice_NextID, bp.Patient.PatientID, bp.Offering.OfferingID, 1, line_total + line_gst, line_gst, "", "", -1));  // HERE
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        decimal bpos_total = Convert.ToDecimal(bpos[j].Quantity) * bpos[j].Offering.DefaultPrice;
                        decimal bpos_gst   = bpos_total * (bpos[j].Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                        InvoiceLine invoiceLine = new InvoiceLine(-1, Prv_Invoice_NextID, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst, bpos[j].AreaTreated, "", -1);  // HERE
                        ((ArrayList)Prv_InvoiceLines[Prv_Invoice_NextID]).Add(invoiceLine);
                        invoiceLinesExtras.Add(invoiceLine);
                    }

                    Prv_Invoice_NextID++;


                    RegisterReferrer[] regRefs = (RegisterReferrer[])patientReferrerCache[bp.Patient.PatientID];
                    if (regRefs != null && regRefs.Length > 0)
                        EPCRefLetterInfo.Add(new Tuple<BookingPatient, int, RegisterReferrer, Booking.InvoiceType, HealthCard, int>(bp, booking.Provider.Field.ID, regRefs[regRefs.Length - 1], invType, hc, -1));
                }
            }



            try
            {
                CreateInvoices(booking.BookingID, MC_Invoice_NextID,  MC_Invoices,  MC_InvoiceLines,  true,  ref invIDs, ref invLineIDs);  // Medicare
                foreach (Tuple<int,int,int> epcRemaining in EPCRemaining_Changes)
                    HealthCardEPCRemainingDB.UpdateNumServicesRemaining(epcRemaining.Item1, epcRemaining.Item3);

                CreateInvoices(booking.BookingID, DVA_Invoice_NextID, DVA_Invoices, DVA_InvoiceLines, true,  ref invIDs, ref invLineIDs);   // DVA
                CreateInvoices(booking.BookingID, TAC_Invoice_NextID, TAC_Invoices, TAC_InvoiceLines, false, ref invIDs, ref invLineIDs);   // TAC
                CreateInvoices(booking.BookingID, Prv_Invoice_NextID, Prv_Invoices, Prv_InvoiceLines, false, ref invIDs, ref invLineIDs);   // Prv

                // set booking as completed
                BookingDB.UpdateSetBookingStatusID(booking.BookingID, 187);




                // =============================================================================================================================================

                // Create Referrer Letters


                //
                // check that reversing invoice for clinics ... these are reset(?)
                //

                
                ArrayList allFileContents = new ArrayList();

                foreach (Tuple<BookingPatient, int, RegisterReferrer, Booking.InvoiceType, HealthCard, int> ptInfo in EPCRefLetterInfo)
                {
                    BookingPatient      bookingPatient    = ptInfo.Item1;
                    int                 fieldID           = ptInfo.Item2;
                    RegisterReferrer    registerReferrer  = ptInfo.Item3;
                    Booking.InvoiceType invType           = ptInfo.Item4;
                    HealthCard          hc                = ptInfo.Item5;
                    int                 epcCountRemaining = ptInfo.Item6;


                    // send referrer letters
                    //
                    // NB: FIRST/LAST letters ONLY FOR MEDICARE - DVA doesn't need letters
                    // Treatment letters for anyone with epc though -- even for private invoices
                    if (registerReferrer != null)
                    {
                        bool needToGenerateFirstLetter = false;
                        bool needToGenerateLastLetter  = false;
                        bool needToGenerateTreatmentLetter = registerReferrer.ReportEveryVisitToReferrer; // send treatment letter whether privately paid or not

                        if (invType == Booking.InvoiceType.Medicare)  // create first/last letters only if medicare
                        {
                            int nPodTreatmentsThisEPC = (int)InvoiceDB.GetMedicareCountByPatientAndDateRange(bookingPatient.Patient.PatientID, hc.DateReferralSigned.Date, DateTime.Now, -1, fieldID);
                            needToGenerateFirstLetter = (nPodTreatmentsThisEPC == 1);
                            needToGenerateLastLetter  = (epcCountRemaining == 0);
                        }

                        // if already generating first or last letter, don't generate treatement letter also
                        if (needToGenerateFirstLetter || needToGenerateLastLetter)
                            needToGenerateTreatmentLetter = false;


                        // TODO: Send Letter By Email

                        // ordereed by shippping/billing addr desc, so if any set, that will be the first one

                        string[] emails = ContactDB.GetEmailsByEntityID(registerReferrer.Organisation.EntityID);

                        bool generateSystemLetters = !registerReferrer.BatchSendAllPatientsTreatmentNotes && (emails.Length > 0 || chkGenerateSystemLetters.Checked);
                        int letterPrintHistorySendMethodID = emails.Length == 0 ? 1 : 2;

                        if (generateSystemLetters)
                        {
                            Letter.FileContents[] fileContentsList = booking.GetSystemLettersList(emails.Length > 0 ? Letter.FileFormat.PDF : Letter.FileFormat.Word, bookingPatient.Patient, hc, fieldID, registerReferrer.Referrer, true, needToGenerateFirstLetter, needToGenerateLastLetter, needToGenerateTreatmentLetter, false, Convert.ToInt32(Session["SiteID"]), Convert.ToInt32(Session["StaffID"]), letterPrintHistorySendMethodID);
                            if (fileContentsList != null && fileContentsList.Length > 0)
                            {
                                if (emails.Length > 0)
                                {
                                    Letter.EmailSystemLetter((string)Session["SiteName"], string.Join(",", emails), fileContentsList);
                                }
                                else
                                {
                                    allFileContents.AddRange(fileContentsList);
                                }
                            }
                        }

                        //BookingDB.UpdateSetGeneratedSystemLetters(booking.BookingID, needToGenerateFirstLetter, needToGenerateLastLetter, generateSystemLetters);
                        BookingPatientDB.UpdateSetGeneratedSystemLetters(bookingPatient.BookingPatientID, needToGenerateFirstLetter, needToGenerateLastLetter, generateSystemLetters);
                    }

                }


                if (allFileContents.Count > 0)
                {
                    Letter.FileContents[] fileContentsList = (Letter.FileContents[])allFileContents.ToArray(typeof(Letter.FileContents));
                    Letter.FileContents   fileContents     = Letter.FileContents.Merge(fileContentsList, "Treatment Letters.pdf"); // change here to create as pdf
                    Session["downloadFile_Contents"] = fileContents.Contents;
                    Session["downloadFile_DocName"]  = fileContents.DocName;
                    //showDownloadPopup = true;

                }


                // ==============================================================================================================================================



                // successfully completed, so update and check warning level for stocks
                foreach (InvoiceLine invoiceLine in invoiceLinesExtras)
                    if (invoiceLine.OfferingOrder == null) // stkip counting down if item is on order
                        StockDB.UpdateAndCheckWarning(booking.Organisation.OrganisationID, invoiceLine.Offering.OfferingID, (int)invoiceLine.Quantity);

            }
            catch (Exception ex)
            {
                if (ex is CustomMessageException == false)
                    Logger.LogException(ex);

                // roll back...
                BookingDB.UpdateSetBookingStatusID(booking.BookingID, 0);
                //BookingDB.UpdateSetGeneratedSystemLetters(booking.BookingID, booking.NeedToGenerateFirstLetter, booking.NeedToGenerateLastLetter, booking.HasGeneratedSystemLetters);
                foreach (int invLineID in invLineIDs)
                    InvoiceLineDB.Delete(invLineID);
                foreach (int invID in invIDs)
                    InvoiceDB.Delete(invID);
                foreach (Tuple<int,int,int> epcRemaining in EPCRemaining_Changes)
                    HealthCardEPCRemainingDB.UpdateNumServicesRemaining(epcRemaining.Item1, epcRemaining.Item2);

                throw;
            }


            SetErrorMessage("Done!");

            // close this window
            bool showDownloadPopup = false;
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=" + (showDownloadPopup ? "true" : "false") + ";self.close();</script>");
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Message.StartsWith("No claim numbers left") || sqlEx.Message.StartsWith("Error: Claim number already in use"))
                SetErrorMessage(sqlEx.Message);
            else
                SetErrorMessage("", sqlEx.ToString());
            return;
        }
        catch (Exception ex)
        {
            SetErrorMessage("", ex.ToString());
            return;
        }
    }

    protected void CreateInvoices(int BookingID, int Invoice_NextID, ArrayList Invoices, Hashtable InvoiceLines, bool AddClaimNbr, ref ArrayList invIDs, ref ArrayList invLineIDs)
    {
        string claimNumber = string.Empty;

        for (int i = 1; i < Invoice_NextID; i++)
        {
            Invoice       curInvoice      = (Invoice)Invoices[i-1];
            InvoiceLine[] curInvoiceLines = (InvoiceLine[])((ArrayList)InvoiceLines[i]).ToArray(typeof(InvoiceLine));

            int invoiceID = InvoiceDB.Insert(363, BookingID, curInvoice.PayerOrganisation == null ? 0 : curInvoice.PayerOrganisation.OrganisationID, curInvoice.PayerPatient == null ? -1 : curInvoice.PayerPatient.PatientID, 0, "", "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), curInvoice.Total, curInvoice.Gst, false, false, false, DateTime.MinValue);
            invIDs.Add(invoiceID);

            if (AddClaimNbr && Convert.ToInt32(SystemVariableDB.GetByDescr("AutoMedicareClaiming").Value) == 1)
            {
                if (claimNumber == string.Empty)
                    claimNumber = MedicareClaimNbrDB.InsertIntoInvoice(invoiceID, DateTime.Now.Date);
                else
                    InvoiceDB.SetClaimNumber(invoiceID, claimNumber);
            }

            for (int j = 0; j < curInvoiceLines.Length; j++)
            {
                InvoiceLine invLine = (InvoiceLine)curInvoiceLines[j];
                int invoiceLineID = InvoiceLineDB.Insert(invoiceID, invLine.Patient.PatientID, invLine.Offering.OfferingID, invLine.Quantity, invLine.Price, invLine.Tax, invLine.AreaTreated, "", invLine.OfferingOrder == null ? -1 : invLine.OfferingOrder.OfferingOrderID);
                invLineIDs.Add(invoiceLineID);
            }
        }
    }


    protected void btnAddPatient_Click(object sender, EventArgs e)
    {
        UpdateAreaTreated();

        int patient_id = Convert.ToInt32(lblPatientIDToAdd.Value);
        int booking_id = GetFormBookingID();

        if (!BookingPatientDB.Exists(booking_id, patient_id))
        {
            Booking    booking = BookingDB.GetByID(booking_id);
            HealthCard hc      = HealthCardDB.GetActiveByPatientID(patient_id);

            int booking_patient_id = BookingPatientDB.Insert(booking_id, patient_id, booking.Offering.OfferingID, hc == null ? string.Empty : hc.AreaTreated, Convert.ToInt32(Session["StaffID"]));
        }

        FillGrid_BookingPatients();
    }

    protected Hashtable GetOfferingHashtable(Booking booking = null)
    {
        if (booking == null)
            booking = BookingDB.GetByID(GetFormBookingID());

        Organisation org = booking.Organisation;
        while (org.ParentOrganisation != null && org.UseParentOffernigPrices)
            org = OrganisationDB.GetByID(org.ParentOrganisation.OrganisationID);

        return OfferingDB.GetHashtableByOrg(org);
    }


    #region DOBAllOrNoneCheck, ValidDateCheck

    protected void DOBAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        try
        {
            CustomValidator cv = (CustomValidator)sender;
            GridViewRow grdRow = ((GridViewRow)cv.Parent.Parent);
            //TextBox txtDate = grdRow.RowType == DataControlRowType.Footer ? (TextBox)grdRow.FindControl("txtNewDOB") : (TextBox)grdRow.FindControl("txtDOB");
            DropDownList _ddlDOB_Day = (DropDownList)grdRow.FindControl(grdRow.RowType == DataControlRowType.Footer ? "ddlNewDOB_Day" : "ddlDOB_Day");
            DropDownList _ddlDOB_Month = (DropDownList)grdRow.FindControl(grdRow.RowType == DataControlRowType.Footer ? "ddlNewDOB_Month" : "ddlDOB_Month");
            DropDownList _ddlDOB_Year = (DropDownList)grdRow.FindControl(grdRow.RowType == DataControlRowType.Footer ? "ddlNewDOB_Year" : "ddlDOB_Year");

            e.IsValid = IsValidDate(_ddlDOB_Day.SelectedValue, _ddlDOB_Month.SelectedValue, _ddlDOB_Year.SelectedValue);
        }
        catch (Exception)
        {
            e.IsValid = false;
        }

    }
    public bool IsValidDate(string day, string month, string year)
    {
        bool invalid = ((day == "-1" || month == "-1" || year == "-1") && (day != "-1" || month != "-1" || year != "-1"));

        if ((day == "-1" && month == "-1" && year == "-1"))
            return true;
        else if ((day == "-1" || month == "-1" || year == "-1"))
            return false;

        try
        {
            DateTime d = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    protected DateTime GetDate(string day, string month, string year)
    {
        if ((day == "-1" && month == "-1" && year == "-1"))
            return DateTime.MinValue;

        return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
    }

    protected void ValidDateCheck(object sender, ServerValidateEventArgs e)
    {
        try
        {
            CustomValidator cv = (CustomValidator)sender;
            GridViewRow grdRow = ((GridViewRow)cv.Parent.Parent);
            TextBox txtDate = grdRow.RowType == DataControlRowType.Footer ? (TextBox)grdRow.FindControl("txtNewDOB") : (TextBox)grdRow.FindControl("txtDOB");

            if (!IsValidDate(txtDate.Text))
                throw new Exception();

            DateTime d = GetDate(txtDate.Text);

            e.IsValid = (d == DateTime.MinValue) || (Utilities.IsValidDBDateTime(d) && Utilities.IsValidDOB(d));
        }
        catch (Exception)
        {
            e.IsValid = false;
        }
    }
    public DateTime GetDate(string inDate)
    {
        inDate = inDate.Trim();

        if (inDate.Length == 0)
        {
            return DateTime.MinValue;
        }
        else
        {
            string[] dobParts = inDate.Split(new char[] { '-' });
            return new DateTime(Convert.ToInt32(dobParts[2]), Convert.ToInt32(dobParts[1]), Convert.ToInt32(dobParts[0]));
        }
    }
    public bool IsValidDate(string inDate)
    {
        inDate = inDate.Trim();
        try
        {
            if (inDate.Length == 0)
                return true;

            if (!System.Text.RegularExpressions.Regex.IsMatch(inDate, @"^\d{2}\-\d{2}\-\d{4}$"))
                return false;

            string[] parts = inDate.Split('-');
            DateTime d = new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]));
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }

    #endregion

    #region GetUrlParamType()

    private bool IsValidFormBookingID()
    {
        string booking_id = Request.QueryString["booking"];
        return booking_id != null && Regex.IsMatch(booking_id, @"^\d+$") && BookingDB.Exists(Convert.ToInt32(booking_id));
    }
    private int GetFormBookingID()
    {
        if (!IsValidFormBookingID())
            throw new CustomMessageException("Invalid url id");

        string booking_id = Request.QueryString["booking"];
        return Convert.ToInt32(booking_id);
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        header_div.Visible                  = false;
        main_table.Visible                  = false;
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