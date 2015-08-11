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

public partial class BookingCreateInvoiceAgedCareV2 : System.Web.UI.Page
{
  
    public bool ProvsCanSeePricesWhenCompletingBks_AC()
    {
        bool ProvsCanSeePricesWhenCompletingBks_AC = Convert.ToInt32(SystemVariableDB.GetByDescr("ProvsCanSeePricesWhenCompletingBks_AC").Value) == 1;
        return ProvsCanSeePricesWhenCompletingBks_AC;
    }


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

                Booking booking = BookingDB.GetByID(GetFormBookingID());
                if (booking == null)
                    throw new CustomMessageException("Invalid or no booking ID.");

                TimeSpan bkTime = booking.DateEnd.Subtract(booking.DateStart);
                txtTotalHours.Text = String.Format("{0:0.0}", bkTime.TotalHours);

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


                if (!UserView.GetInstance().IsAgedCareView)
                    throw new CustomMessageException("Not logged into the Aged Care site.");

                if (!IsValidFormBookingID())
                    throw new CustomMessageException("Invalid or no booking ID.");

                SetupGUI(booking);
                FillGrid_PatientList();
                FillGrid_BookingPatients();
            }

            this.GrdPatientList.EnableViewState = true;
            //this.GrdBookingPatients.EnableViewState = false;

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

    protected void SetupGUI(Booking booking = null)
    {
        PageCompletionType pageCompletionType = GetUrlParamType(booking);
        if (pageCompletionType == PageCompletionType.None)
            throw new Exception("No 'type' field in url");


        bool editable = true;
        Utilities.SetEditControlBackColour(txtHourlyPrice,  editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtTotalHours,   editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtTotal,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.White);
        txtTotal.BackColor = System.Drawing.Color.White;
        
        
        bool isMulti  = pageCompletionType == PageCompletionType.MultiPTs_SeperateInvoices    || pageCompletionType == PageCompletionType.MultiPTs_SingleHourlyInvoice;
        bool isHourly = pageCompletionType == PageCompletionType.MultiPTs_SingleHourlyInvoice || pageCompletionType == PageCompletionType.SinglePT_SingleHourlyInvoice;

        linkConvertCompletionType.Text        = isHourly ? "Convert To Standard (Not Hourly) Invoice" : "Convert To Hourly Invoice";
        linkConvertCompletionType.NavigateUrl = UrlParamModifier.AddEdit(Request.RawUrl, "completion_type", isHourly ? "standard" : "hourly");


        int thisBookingPatientID = -1;

        if (!isMulti)
        {
            // hide left table and column seperator
            GrdPatientList.Visible  = false;
            lblHeadingBookedPatients.Visible = false;
            td_grd_pt_list.Visible  = false;
            td_column_space.Visible = false;

            // hide box to add residents
            add_resident_row.Visible       = false;

            // make sure is added in right table and any other patients removed
            BookingPatient[] patients = BookingPatientDB.GetByBookingID(booking.BookingID);
            for (int i = 0; i < patients.Length; i++)
            {
                if (patients[i].Patient.PatientID == booking.Patient.PatientID)
                {
                    BookingPatientOfferingDB.DeleteByBookingPatientID(patients[i].BookingPatientID);
                    BookingPatientDB.Delete(patients[i].BookingPatientID);
                }
                else
                    thisBookingPatientID = patients[i].BookingPatientID;
            }
            if (thisBookingPatientID == -1)
            {
                HealthCard hc = HealthCardDB.GetActiveByPatientID(booking.Patient.PatientID);
                thisBookingPatientID = BookingPatientDB.Insert(booking.BookingID, booking.Patient.PatientID, -1, hc == null ? string.Empty : hc.AreaTreated, Convert.ToInt32(Session["StaffID"]));
            }


            // remove 'remove' button from right table
            for (int i = 0; i < GrdBookingPatients.Columns.Count; i++)
                if (GrdBookingPatients.Columns[i].SortExpression == "remove")
                    GrdBookingPatients.Columns[i].Visible = false;
        }

        if (isHourly)
        {
            // remove any extras from other invoice
            BookingPatient[] patients = BookingPatientDB.GetByBookingID(booking.BookingID);
            for (int i = 0; i < patients.Length; i++)
                BookingPatientOfferingDB.DeleteByBookingPatientID(patients[i].BookingPatientID);

            // hide extras column in right table
            for (int i = 0; i < GrdBookingPatients.Columns.Count; i++)
                if (GrdBookingPatients.Columns[i].HeaderText == "Extras" || GrdBookingPatients.Columns[i].HeaderText == "PT Type" || GrdBookingPatients.Columns[i].SortExpression == "hide_edit_col")
                    GrdBookingPatients.Columns[i].Visible = false;

            // hide all costs [do this below when adding costs .. just don't if isHourly]
        }

        hourly_price_table.Visible = isHourly;

        bool add_patients = Request.QueryString["type"] != null && Request.QueryString["type"] == "add_patients";
        if (add_patients)
        {
            btnSubmit.Visible                   = false;
            buttonsSpace.Visible                = false;
            generate_system_letters_row.Visible = false;
            add_resident_space_row.Visible      = false;
            btnCancel.Text                      = "Close";
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

    #region GetUrlParamType()

    private enum PageCompletionType { SinglePT_SeperateInvoices, SinglePT_SingleHourlyInvoice, MultiPTs_SeperateInvoices, MultiPTs_SingleHourlyInvoice, None };
    private PageCompletionType GetUrlParamType(Booking booking = null)
    {
        if (booking == null)
            booking = BookingDB.GetByID(GetFormBookingID());

        if (booking == null)
            return PageCompletionType.None;

        string type = Request.QueryString["completion_type"];
        if (type != null && type.ToLower() == "hourly")
            return booking.Patient == null ? PageCompletionType.MultiPTs_SingleHourlyInvoice : PageCompletionType.SinglePT_SingleHourlyInvoice;
        else if (type != null && type.ToLower() == "standard")
            return booking.Patient == null ? PageCompletionType.MultiPTs_SeperateInvoices : PageCompletionType.SinglePT_SeperateInvoices;
        else
            return PageCompletionType.None;
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

    #region GrdPatientList

    protected void FillGrid_PatientList()
    {

        Booking booking = BookingDB.GetByID(GetFormBookingID());
        if (booking == null)
        {
            SetErrorMessage("Invalid booking ID.");
            return;
        }


        string startAMPM = booking.DateStart.Hour < 12 ? "am" : "pm";
        string endAMPM   = booking.DateEnd.Hour   < 12 ? "am" : "pm";
        string timeDisplayed = startAMPM == endAMPM ?
            (booking.DateStart.Hour % 12)               + "-" + (booking.DateEnd.Hour % 12) + startAMPM :
            (booking.DateStart.Hour % 12)   + startAMPM + " - " + (booking.DateEnd.Hour % 12) + endAMPM;

        lblHeading.Text = "Booking<small><font color=\"#989898\"> at </font></small>" + booking.Organisation.Name + "<small><font color=\"#989898\"> with </font></small>" + booking.Provider.Person.FullnameWithoutMiddlename + "<small><font color=\"#989898\"> on </font></small>" + booking.DateStart.ToString("d MMM yyyy") + "<small><font color=\"#989898\"> at </font></small>" + timeDisplayed;

        lblHeadingPatientList.Text = "Residents of " + booking.Organisation.Name;

        DataTable dt = RegisterPatientDB.GetDataTable_PatientsOf(false, booking.Organisation.OrganisationID);

        Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
        PatientDB.AddACOfferings(ref offeringsHash, ref dt, "ac_inv_offering_id", "ac_inv_offering_name", "ac_pat_offering_id", "ac_pat_offering_name",
                                                            "ac_inv_aged_care_patient_type_id", "ac_inv_aged_care_patient_type_descr",
                                                            "ac_pat_aged_care_patient_type_id", "ac_pat_aged_care_patient_type_descr"
                                                            );

        // get info to show EPC's remaining
        int[] patientIDs = new int[dt.Rows.Count];
        int[] entityIDs = new int[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            patientIDs[i] = Convert.ToInt32(dt.Rows[i]["patient_id"]);
            entityIDs[i] = Convert.ToInt32(dt.Rows[i]["entity_id"]);
        }
        Hashtable patientHealthCardCache        = PatientsHealthCardsCacheDB.GetBullkActive(patientIDs);
        Hashtable epcRemainingCache             = GetEPCRemainingCache(patientHealthCardCache);
        Hashtable patientsMedicareCountCache    = PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, DateTime.Today.Year);
        Hashtable patientsEPCRemainingCache     = PatientsEPCRemainingCacheDB.GetBullk(patientIDs, DateTime.Today.AddYears(-1));
        int       MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);
        Hashtable bedroomHash                   = GetBedroomHash(entityIDs, false);


        // In display, have pt type 
        // - if mcd/dva/emergency : Medicare (Low Care)
        // - if LC/HC/ETC         : Low Care
        dt.Columns.Add("ac_offering", typeof(String));

        dt.Columns.Add("epc_org_name"        , typeof(String));
        dt.Columns.Add("epc_org_id"          , typeof(Int32));
        dt.Columns.Add("epc_expire_date"     , typeof(DateTime));
        dt.Columns.Add("has_valid_epc"       , typeof(Boolean));
        dt.Columns.Add("epc_count_remaining" , typeof(Int32));
        dt.Columns.Add("epc_text"            , typeof(String));

        dt.Columns.Add("room"                , typeof(String));
        dt.Columns.Add("room_sort"           , typeof(String));

        for (int i = 0; i < dt.Rows.Count; i++)
        {

            //
            // first add in epc info
            //

            int patientID = Convert.ToInt32(dt.Rows[i]["patient_id"]);
            int entityID = Convert.ToInt32(dt.Rows[i]["entity_id"]);

            HealthCard               hc                       = GetHealthCardFromCache(patientHealthCardCache, patientID);
            bool                     hasEPC                   = hc != null && hc.DateReferralSigned != DateTime.MinValue;
            HealthCardEPCRemaining[] epcsRemaining            = !hasEPC ? new HealthCardEPCRemaining[] { } : GetEPCRemainingFromCache(epcRemainingCache, hc, booking.Provider.Field.ID);
            int                      totalServicesAllowedLeft = !hasEPC ? 0 : (MedicareMaxNbrServicesPerYear - (int)patientsMedicareCountCache[patientID]);

            int totalEpcsRemaining = 0;
            for (int j = 0; j < epcsRemaining.Length; j++)
                totalEpcsRemaining += epcsRemaining[j].NumServicesRemaining;

            DateTime referralSignedDate = !hasEPC ? DateTime.MinValue : hc.DateReferralSigned.Date;
            DateTime hcExpiredDate      = !hasEPC ? DateTime.MinValue : referralSignedDate.AddYears(1);
            bool     isExpired          = !hasEPC ? true              : hcExpiredDate <= DateTime.Today;

            int nServicesLeft = 0;
            if (hc != null && DateTime.Today >= referralSignedDate.Date && DateTime.Today < hcExpiredDate.Date)
                nServicesLeft = totalEpcsRemaining;
            if (hc != null && totalServicesAllowedLeft < nServicesLeft)
                nServicesLeft = totalServicesAllowedLeft;

            bool has_valid_epc = hasEPC && !isExpired && (hc.Organisation.OrganisationID == -2 || (hc.Organisation.OrganisationID == -1 && nServicesLeft > 0));
            int epc_count_remaining = hasEPC && hc.Organisation.OrganisationID == -1 ? nServicesLeft : -1;

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


            //
            // now add in AC PT type
            //

            int ac_inv_offering_id = dt.Rows[i]["ac_inv_offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(dt.Rows[i]["ac_inv_offering_id"]);
            int ac_pat_offering_id = dt.Rows[i]["ac_pat_offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(dt.Rows[i]["ac_pat_offering_id"]);
            string ac_inv_offering_name = dt.Rows[i]["ac_inv_offering_name"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[i]["ac_inv_offering_name"]);
            string ac_pat_offering_name = dt.Rows[i]["ac_pat_offering_name"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[i]["ac_pat_offering_name"]);

            int    ac_inv_aged_care_patient_type_id    = dt.Rows[i]["ac_inv_aged_care_patient_type_id"]    == DBNull.Value ? -1           : Convert.ToInt32(dt.Rows[i]["ac_inv_aged_care_patient_type_id"]);
            string ac_inv_aged_care_patient_type_descr = dt.Rows[i]["ac_inv_aged_care_patient_type_descr"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[i]["ac_inv_aged_care_patient_type_descr"]);
            int    ac_pat_aged_care_patient_type_id    = dt.Rows[i]["ac_pat_aged_care_patient_type_id"]    == DBNull.Value ? -1           : Convert.ToInt32(dt.Rows[i]["ac_pat_aged_care_patient_type_id"]);
            string ac_pat_aged_care_patient_type_descr = dt.Rows[i]["ac_pat_aged_care_patient_type_descr"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[i]["ac_pat_aged_care_patient_type_descr"]);


            if (ac_inv_offering_id == -1)
                dt.Rows[i]["ac_offering"] = string.Empty;
            else if ((new List<int> { 2, 3, 4, 5 }).Contains(ac_inv_aged_care_patient_type_id))
                dt.Rows[i]["ac_offering"] = ac_inv_offering_name;
            else if ((new List<int> { 6, 7, 8, 9, 10 }).Contains(ac_inv_aged_care_patient_type_id))
                dt.Rows[i]["ac_offering"] = ac_inv_offering_name + " (" + ac_pat_offering_name + ")";
            else // (?)
                dt.Rows[i]["ac_offering"] = ac_inv_offering_name;

            //
            // room
            //

            dt.Rows[i]["room"]      = bedroomHash[entityID] == null ? "" : (string)bedroomHash[entityID];
            dt.Rows[i]["room_sort"] = bedroomHash[entityID] == null ? "" : PadRoomNbr((string)bedroomHash[entityID], true);
        }

        if (Session["patientlist_sortexpression"] != null)
        {
            DataView dv = dt.DefaultView;
            dv.Sort = Session["patientlist_sortexpression"] == null ? "room asc" : (string)Session["patientlist_sortexpression"];
            dt = dv.ToTable();
        }


        Session["patientlist_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdPatientList.DataSource = dt;
            try
            {
                GrdPatientList.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdPatientList.DataSource = dt;
            GrdPatientList.DataBind();

            int TotalColumns = GrdPatientList.Rows[0].Cells.Count;
            GrdPatientList.Rows[0].Cells.Clear();
            GrdPatientList.Rows[0].Cells.Add(new TableCell());
            GrdPatientList.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdPatientList.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdPatientList_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdPatientList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["patientlist_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("patient_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            Label lblEPCExpiry = (Label)e.Row.FindControl("lblEPCExpiry");
            if (lblEPCExpiry != null)
                if (!Convert.ToBoolean(thisRow["has_valid_epc"]))
                    lblEPCExpiry.ForeColor = System.Drawing.Color.Red;

            DropDownList ddlACInvOffering = (DropDownList)e.Row.FindControl("ddlACInvOffering");
            if (ddlACInvOffering != null)
            {
                int ac_inv_offering_id = thisRow["ac_inv_offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(thisRow["ac_inv_offering_id"]);
                int ac_pat_offering_id = thisRow["ac_pat_offering_id"] == DBNull.Value ? -1 : Convert.ToInt32(thisRow["ac_pat_offering_id"]);
                string ac_inv_offering_name = thisRow["ac_inv_offering_name"] == DBNull.Value ? string.Empty : Convert.ToString(thisRow["ac_inv_offering_name"]);
                string ac_pat_offering_name = thisRow["ac_pat_offering_name"] == DBNull.Value ? string.Empty : Convert.ToString(thisRow["ac_pat_offering_name"]);

                int    ac_inv_aged_care_patient_type_id    = thisRow["ac_inv_aged_care_patient_type_id"]    == DBNull.Value ? -1           : Convert.ToInt32(thisRow["ac_inv_aged_care_patient_type_id"]);
                string ac_inv_aged_care_patient_type_descr = thisRow["ac_inv_aged_care_patient_type_descr"] == DBNull.Value ? string.Empty : Convert.ToString(thisRow["ac_inv_aged_care_patient_type_descr"]);
                int    ac_pat_aged_care_patient_type_id    = thisRow["ac_pat_aged_care_patient_type_id"]    == DBNull.Value ? -1           : Convert.ToInt32(thisRow["ac_pat_aged_care_patient_type_id"]);
                string ac_pat_aged_care_patient_type_descr = thisRow["ac_pat_aged_care_patient_type_descr"] == DBNull.Value ? string.Empty : Convert.ToString(thisRow["ac_pat_aged_care_patient_type_descr"]);

                DataTable dt_offerings = OfferingDB.GetDataTable(true, -1, null, true);
                for (int i = dt_offerings.Rows.Count - 1; i >= 0; i--)
                {
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

                if (ac_inv_offering_id != -1)
                    ddlACInvOffering.SelectedValue = ac_inv_offering_id.ToString();
            }


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

        }
    }
    protected void GrdPatientList_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        UpdateAreaTreated();

        GrdPatientList.EditIndex = -1;
        FillGrid_PatientList();
    }
    protected void GrdPatientList_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (!UserView.GetInstance().IsAgedCareView)
        {
            SetErrorMessage("Not logged into the Aged Care site.");
            return;
        }

        UpdateAreaTreated();

        Label lblId = (Label)GrdPatientList.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlACInvOffering = (DropDownList)GrdPatientList.Rows[e.RowIndex].FindControl("ddlACInvOffering");


        DataTable dt = Session["patientlist_data"] as DataTable;
        DataRow[] foundRows = dt.Select("patient_id=" + lblId.Text);
        DataRow row = foundRows[0];
        Patient patient = PatientDB.LoadAll(row);

        Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
        PatientDB.AddACOfferings(ref offeringsHash, ref patient);


        int acInvOfferingID_New = patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID;
        int acPatOfferingID_New = patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID;



        if (patient.ACInvOffering == null || patient.ACPatOffering == null)
        {
            acInvOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
            acPatOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
        }
        else if (patient.ACInvOffering.OfferingID != Convert.ToInt32(ddlACInvOffering.SelectedValue))
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
                                patient.FlashingText, patient.FlashingTextAddedBy == null ? -1 : patient.FlashingTextAddedBy.StaffID, patient.FlashingTextLastModifiedDate, patient.PrivateHealthFund, patient.ConcessionCardNumber, patient.ConcessionCardExpiryDate, patient.IsDiabetic, patient.IsMemberDiabetesAustralia, patient.DiabeticAAassessmentReviewDate, patient.ACInvOffering == null ? -1 : patient.ACInvOffering.OfferingID, patient.ACPatOffering == null ? -1 : patient.ACPatOffering.OfferingID, patient.Login, patient.Pwd, patient.IsCompany, patient.ABN, patient.NextOfKinName, patient.NextOfKinRelation, patient.NextOfKinContactInfo, 
                                patient.Person.Title.ID, patient.Person.Firstname, patient.Person.Middlename, patient.Person.Surname, patient.Person.Nickname, patient.Person.Gender, patient.Person.Dob, Convert.ToInt32(Session["StaffID"]));

        PatientDB.Update(patient.PatientID, patient.Person.PersonID, patient.IsClinicPatient, patient.IsGPPatient, patient.IsDeceased, patient.FlashingText, patient.FlashingTextAddedBy == null ? -1 : patient.FlashingTextAddedBy.StaffID, patient.FlashingTextLastModifiedDate, patient.PrivateHealthFund, patient.ConcessionCardNumber, patient.ConcessionCardExpiryDate, patient.IsDiabetic, patient.IsMemberDiabetesAustralia, patient.DiabeticAAassessmentReviewDate, acInvOfferingID_New, acPatOfferingID_New, patient.Login, patient.Pwd, patient.IsCompany, patient.ABN, patient.NextOfKinName, patient.NextOfKinRelation, patient.NextOfKinContactInfo);


        GrdPatientList.EditIndex = -1;
        FillGrid_PatientList();
        GrdBookingPatients.EditIndex = -1;
        FillGrid_BookingPatients();
    }
    protected void GrdPatientList_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        UpdateAreaTreated();

        Label lblId = (Label)GrdPatientList.Rows[e.RowIndex].FindControl("lblId");

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

        FillGrid_PatientList();
    }
    protected void GrdPatientList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Add"))
        {
            UpdateAreaTreated();

            int patient_id = Convert.ToInt32(e.CommandArgument);
            int booking_id = GetFormBookingID();

            if (!BookingPatientDB.Exists(booking_id, patient_id))
            {

                Patient pt = PatientDB.GetByID(patient_id);
                if (pt.ACInvOffering == null || pt.ACPatOffering == null)
                {
                    SetErrorMessage(pt.Person.FullnameWithoutMiddlename + " does not have an 'PT Type' set. Please set one before adding.");
                    return;
                }

                HealthCard hc = HealthCardDB.GetActiveByPatientID(patient_id);
                BookingPatientDB.Insert(booking_id, patient_id, -1, hc == null ? string.Empty : hc.AreaTreated, Convert.ToInt32(Session["StaffID"]));
            }


            FillGrid_PatientList();
            FillGrid_BookingPatients();
        }
    }
    protected void GrdPatientList_RowEditing(object sender, GridViewEditEventArgs e)
    {
        UpdateAreaTreated();

        GrdPatientList.EditIndex = e.NewEditIndex;
        FillGrid_PatientList();
    }
    protected void GrdPatientList_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdPatientList.EditIndex >= 0)
            return;

        GrdPatientList_Sort(e.SortExpression);
    }

    protected void GrdPatientList_Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["patientlist_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["patientlist_sortexpression"] == null)
                Session["patientlist_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["patientlist_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["patientlist_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdPatientList.DataSource = dataView;
            GrdPatientList.DataBind();
        }
    }

    #endregion

    #region GrdBookingPatients

    protected void FillGrid_BookingPatients()
    {

        Booking booking = BookingDB.GetByID(GetFormBookingID());
        if (booking == null)
        {
            SetErrorMessage("Invalid booking ID.");
            return;
        }

        lblHeadingBookedPatients.Text = "Booked";


        PageCompletionType pageCompletionType = GetUrlParamType(booking);
        bool isMulti  = pageCompletionType == PageCompletionType.MultiPTs_SeperateInvoices    || pageCompletionType == PageCompletionType.MultiPTs_SingleHourlyInvoice;
        bool isHourly = pageCompletionType == PageCompletionType.MultiPTs_SingleHourlyInvoice || pageCompletionType == PageCompletionType.SinglePT_SingleHourlyInvoice;



        DataTable dt = BookingPatientDB.GetDataTable_ByBookingID(booking.BookingID);

        Hashtable offeringsHashPrices = GetOfferingHashtable(booking);
        Session["bookingpatientofferings_offeringshashprice_data"] = offeringsHashPrices;

        Hashtable offeringsHash       = OfferingDB.GetHashtable(true, -1, null, true);
        Session["bookingpatientofferings_offeringshash_data"] = offeringsHash;

        PatientDB.AddACOfferings(ref offeringsHash, ref dt, "patient_ac_inv_offering_id", "patient_ac_inv_offering_name", "patient_ac_pat_offering_id", "patient_ac_pat_offering_name",
                                                            "ac_inv_aged_care_patient_type_id", "ac_inv_aged_care_patient_type_descr",
                                                            "ac_pat_aged_care_patient_type_id", "ac_pat_aged_care_patient_type_descr"
                                                            );

        // get info to show EPC's remaining
        int[] patientIDs = new int[dt.Rows.Count];
        int[] entityIDs = new int[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            patientIDs[i] = Convert.ToInt32(dt.Rows[i]["bp_patient_id"]);
            entityIDs[i]  = Convert.ToInt32(dt.Rows[i]["patient_person_entity_id"]);
        }
        Hashtable patientHealthCardCache        = PatientsHealthCardsCacheDB.GetBullkActive(patientIDs);
        Hashtable epcRemainingCache             = GetEPCRemainingCache(patientHealthCardCache);
        Hashtable patientsMedicareCountCache    = PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, DateTime.Today.Year);
        Hashtable patientsEPCRemainingCache     = PatientsEPCRemainingCacheDB.GetBullk(patientIDs, DateTime.Today.AddYears(-1));
        int       MedicareMaxNbrServicesPerYear = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);
        Hashtable bedroomHash                   = GetBedroomHash(entityIDs);


        //in display, have pt type 
        //- if mcd/dva/emergency : Medicare (Low Care)
        //- if LC/HC/ETC         : Low Care
        dt.Columns.Add("patient_ac_offering",      typeof(String));
        dt.Columns.Add("patient_ac_offering_cost", typeof(String));
        dt.Columns.Add("is_dva",                   typeof(Boolean));

        dt.Columns.Add("epc_org_name"        , typeof(String));
        dt.Columns.Add("epc_org_id"          , typeof(Int32));
        dt.Columns.Add("epc_expire_date"     , typeof(DateTime));
        dt.Columns.Add("has_valid_epc"       , typeof(Boolean));
        dt.Columns.Add("epc_count_remaining" , typeof(Int32));
        dt.Columns.Add("epc_text"            , typeof(String));

        dt.Columns.Add("room"                , typeof(String));

        for (int i = 0; i < dt.Rows.Count; i++)
        {

            //
            // first add in epc info
            //

            int patientID = Convert.ToInt32(dt.Rows[i]["bp_patient_id"]);
            int entityID  = Convert.ToInt32(dt.Rows[i]["patient_person_entity_id"]);

            HealthCard               hc                       = GetHealthCardFromCache(patientHealthCardCache, patientID);
            bool                     hasEPC                   = hc != null && hc.DateReferralSigned != DateTime.MinValue;
            HealthCardEPCRemaining[] epcsRemaining            = !hasEPC ? new HealthCardEPCRemaining[] { } : GetEPCRemainingFromCache(epcRemainingCache, hc, booking.Provider.Field.ID);
            int                      totalServicesAllowedLeft = !hasEPC ? 0 : (MedicareMaxNbrServicesPerYear - (int)patientsMedicareCountCache[patientID]);

            int totalEpcsRemaining = 0;
            for (int j = 0; j < epcsRemaining.Length; j++)
                totalEpcsRemaining += epcsRemaining[j].NumServicesRemaining;

            DateTime referralSignedDate = !hasEPC ? DateTime.MinValue : hc.DateReferralSigned.Date;
            DateTime hcExpiredDate      = !hasEPC ? DateTime.MinValue : referralSignedDate.AddYears(1);
            bool     isExpired          = !hasEPC ? true              : hcExpiredDate <= DateTime.Today;

            int nServicesLeft = 0;
            if (hc != null && DateTime.Today >= referralSignedDate.Date && DateTime.Today < hcExpiredDate.Date)
                nServicesLeft = totalEpcsRemaining;
            if (hc != null && totalServicesAllowedLeft < nServicesLeft)
                nServicesLeft = totalServicesAllowedLeft;

            bool has_valid_epc = hasEPC && !isExpired && (hc.Organisation.OrganisationID == -2 || (hc.Organisation.OrganisationID == -1 && nServicesLeft > 0));
            int epc_count_remaining = hasEPC && hc.Organisation.OrganisationID == -1 ? nServicesLeft : -1;

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


            //
            // now add in AC PT type
            //

            int    ac_inv_offering_id   = dt.Rows[i]["patient_ac_inv_offering_id"]   == DBNull.Value ? -1           : Convert.ToInt32(dt.Rows[i]["patient_ac_inv_offering_id"]);
            int    ac_pat_offering_id   = dt.Rows[i]["patient_ac_pat_offering_id"]   == DBNull.Value ? -1           : Convert.ToInt32(dt.Rows[i]["patient_ac_pat_offering_id"]);
            string ac_inv_offering_name = dt.Rows[i]["patient_ac_inv_offering_name"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[i]["patient_ac_inv_offering_name"]);
            string ac_pat_offering_name = dt.Rows[i]["patient_ac_pat_offering_name"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[i]["patient_ac_pat_offering_name"]);

            int    ac_inv_aged_care_patient_type_id    = dt.Rows[i]["ac_inv_aged_care_patient_type_id"]    == DBNull.Value ? -1           : Convert.ToInt32(dt.Rows[i]["ac_inv_aged_care_patient_type_id"]);
            string ac_inv_aged_care_patient_type_descr = dt.Rows[i]["ac_inv_aged_care_patient_type_descr"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[i]["ac_inv_aged_care_patient_type_descr"]);
            int    ac_pat_aged_care_patient_type_id    = dt.Rows[i]["ac_pat_aged_care_patient_type_id"]    == DBNull.Value ? -1           : Convert.ToInt32(dt.Rows[i]["ac_pat_aged_care_patient_type_id"]);
            string ac_pat_aged_care_patient_type_descr = dt.Rows[i]["ac_pat_aged_care_patient_type_descr"] == DBNull.Value ? string.Empty : Convert.ToString(dt.Rows[i]["ac_pat_aged_care_patient_type_descr"]);



            // offering_id	name
            //
            // 1	LC
            // 7	LCF
            // 2	HC
            // 8	HCU
            //
            // 3	DVA
            // 4	Medicare
            // 9	TAC
            //
            // 5	HCE
            // 6	LCE

            Offering offering = offeringsHashPrices[ac_inv_offering_id] == null ? (Offering)offeringsHash[ac_inv_offering_id] : (Offering)offeringsHashPrices[ac_inv_offering_id];
            decimal GST_Percent  = Convert.ToDecimal(((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["GST_Percent"].Value);
            decimal GST_Modifier = ((Offering)offeringsHash[ac_inv_offering_id]).IsGstExempt ? 1 : ((decimal)100 + GST_Percent) / (decimal)100;

            if (ac_inv_offering_id == -1)
            {
                dt.Rows[i]["patient_ac_offering"] = string.Empty;
                dt.Rows[i]["patient_ac_offering_cost"] = DBNull.Value;
            }
            else if ((new List<int> { 2,3,4,5 }).Contains(ac_inv_aged_care_patient_type_id))
            {
                dt.Rows[i]["patient_ac_offering"] = ac_inv_offering_name;
                dt.Rows[i]["patient_ac_offering_cost"] = isHourly ? "" : " $" + (offering.DefaultPrice * GST_Modifier);
            }


            else if ((new List<int> { 9 }).Contains(ac_inv_aged_care_patient_type_id))
            {
                dt.Rows[i]["patient_ac_offering"] = ac_inv_offering_name + " (" + ac_pat_offering_name + ")";
                dt.Rows[i]["patient_ac_offering_cost"] = isHourly ? "" : " $" + (offering.DvaCharge * GST_Modifier);
            }
            else if ((new List<int> { 8 }).Contains(ac_inv_aged_care_patient_type_id))
            {
                dt.Rows[i]["patient_ac_offering"] = ac_inv_offering_name + " (" + ac_pat_offering_name + ")";
                dt.Rows[i]["patient_ac_offering_cost"] = isHourly ? "" : " $" + offering.MedicareCharge;  // No GST on Medicare
            }
            else if ((new List<int> { 10 }).Contains(ac_inv_aged_care_patient_type_id))
            {
                dt.Rows[i]["patient_ac_offering"] = ac_inv_offering_name + " (" + ac_pat_offering_name + ")";
                dt.Rows[i]["patient_ac_offering_cost"] = isHourly ? "" : " $" + (offering.TacCharge * GST_Modifier);
            }
            else if ((new List<int> { 6,7 }).Contains(ac_inv_aged_care_patient_type_id))
            {
                dt.Rows[i]["patient_ac_offering"] = ac_inv_offering_name + " (" + ac_pat_offering_name + ")";
                dt.Rows[i]["patient_ac_offering_cost"] = isHourly ? "" : " $" + (offering.DefaultPrice * GST_Modifier);
            }


            else // (?)
            {
                dt.Rows[i]["patient_ac_offering"] = ac_inv_offering_name;
                dt.Rows[i]["patient_ac_offering_cost"] = DBNull.Value;
            }


            dt.Rows[i]["is_dva"] = ac_inv_aged_care_patient_type_id == 9;

            //
            // room
            //

            dt.Rows[i]["room"] = bedroomHash[entityID] == null ? "" : (string)bedroomHash[entityID];

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
            Label lblId = (Label)e.Row.FindControl("lblId");
            Label lblPatientId = (Label)e.Row.FindControl("lblPatientId");
            DataRow[] foundRows = dt.Select("bp_booking_patient_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];

            int booking_patient_id = Convert.ToInt32(lblId.Text);

            bool isDVA = Convert.ToBoolean(thisRow["is_dva"]);


            DropDownList ddlACInvOffering = (DropDownList)e.Row.FindControl("ddlACInvOffering");
            if (ddlACInvOffering != null)
            {
                int    ac_inv_offering_id   = thisRow["patient_ac_inv_offering_id"]   == DBNull.Value ? -1           : Convert.ToInt32(thisRow["patient_ac_inv_offering_id"]);
                int    ac_pat_offering_id   = thisRow["patient_ac_pat_offering_id"]   == DBNull.Value ? -1           : Convert.ToInt32(thisRow["patient_ac_pat_offering_id"]);
                string ac_inv_offering_name = thisRow["patient_ac_inv_offering_name"] == DBNull.Value ? string.Empty : Convert.ToString(thisRow["patient_ac_inv_offering_name"]);
                string ac_pat_offering_name = thisRow["patient_ac_pat_offering_name"] == DBNull.Value ? string.Empty : Convert.ToString(thisRow["patient_ac_pat_offering_name"]);

                int    ac_inv_aged_care_patient_type_id    = thisRow["ac_inv_aged_care_patient_type_id"]    == DBNull.Value ? -1           : Convert.ToInt32(thisRow["ac_inv_aged_care_patient_type_id"]);
                string ac_inv_aged_care_patient_type_descr = thisRow["ac_inv_aged_care_patient_type_descr"] == DBNull.Value ? string.Empty : Convert.ToString(thisRow["ac_inv_aged_care_patient_type_descr"]);
                int    ac_pat_aged_care_patient_type_id    = thisRow["ac_pat_aged_care_patient_type_id"]    == DBNull.Value ? -1           : Convert.ToInt32(thisRow["ac_pat_aged_care_patient_type_id"]);
                string ac_pat_aged_care_patient_type_descr = thisRow["ac_pat_aged_care_patient_type_descr"] == DBNull.Value ? string.Empty : Convert.ToString(thisRow["ac_pat_aged_care_patient_type_descr"]);

                DataTable dt_offerings = OfferingDB.GetDataTable(true, -1, null, true);
                for (int i = dt_offerings.Rows.Count - 1; i >= 0; i--)
                {
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

                if (ac_inv_offering_id != -1)
                    ddlACInvOffering.SelectedValue = ac_inv_offering_id.ToString();
            }


            Repeater rptBkPtOfferings = (Repeater)e.Row.FindControl("rptBkPtOfferings");
            if (rptBkPtOfferings != null)
            {
                Hashtable offeringsHashPrices = (Hashtable)Session["bookingpatientofferings_offeringshashprice_data"];

                DataTable dt_allbkptofferings = Session["bookingpatientofferings_data"] as DataTable;
                DataTable dt_bkptofferings = dt_allbkptofferings.Clone(); // copy structure
                for (int i = 0; i < dt_allbkptofferings.Rows.Count; i++)
                    if (Convert.ToInt32(dt_allbkptofferings.Rows[i]["bpo_booking_patient_id"]) == booking_patient_id)
                        dt_bkptofferings.Rows.Add(dt_allbkptofferings.Rows[i].ItemArray);


                dt_bkptofferings.Columns.Add("is_dva", typeof(Boolean));
                dt_bkptofferings.Columns.Add("area_treated", typeof(String));
                for (int i = 0; i < dt_bkptofferings.Rows.Count; i++)
                {
                    dt_bkptofferings.Rows[i]["is_dva"] = isDVA;
                    dt_bkptofferings.Rows[i]["area_treated"] = string.Empty;
                }

                DataView dv = dt_bkptofferings.DefaultView;
                dv.Sort = "offering_name";
                dt_bkptofferings = dv.ToTable();

                //DataTable dt_bkptofferings = BookingPatientOfferingDB.GetDataTable_ByBookingPatientID(Convert.ToInt32(lblId.Text));
                rptBkPtOfferings.DataSource = dt_bkptofferings;
                rptBkPtOfferings.DataBind();
            }


            DropDownList ddlOfferings = (DropDownList)e.Row.FindControl("ddlOfferings");
            System.Web.UI.HtmlControls.HtmlControl br = (System.Web.UI.HtmlControls.HtmlControl)e.Row.FindControl("br_before_addcancelbuttons");
            Button btnBkPtOfferingsAdd = (Button)e.Row.FindControl("btnBkPtOfferingsAdd");
            Button btnBkPtOfferingsCancelAdd = (Button)e.Row.FindControl("btnBkPtOfferingsCancelAdd");

            

            List<int> bkPtIDsVisible = (hiddenViewDdlBookingPatientIDs.Value.Length == 0) ? new List<int> { } : hiddenViewDdlBookingPatientIDs.Value.Split(',').Select(int.Parse).ToList();
            if (bkPtIDsVisible.Contains(booking_patient_id))
            {
                DataTable dt_all_offerings = OfferingDB.GetDataTable(false, "3,4", "63,89"); // OfferingDB.GetDataTable(4, "63,89"); // 3 = Clinic & AC, 4 = AC  // 63=Services,89=Products

                //dt_all_offerings.DefaultView.Sort = "name";
                //dt_all_offerings = dt_all_offerings.DefaultView.ToTable();

                ddlOfferings.Style["max-width"] = "250px";
                ddlOfferings.Items.Clear();
                foreach (DataRow row in dt_all_offerings.Rows)
                {
                    Offering offering = OfferingDB.LoadAll(row);
                    ddlOfferings.Items.Add(new ListItem(offering.Name, offering.OfferingID.ToString()));
                }

                ddlOfferings.Visible = true;
                br.Visible = true;
                btnBkPtOfferingsCancelAdd.Visible = true;

                btnBkPtOfferingsAdd.CommandName = "AddOffering";
                btnBkPtOfferingsAdd.CommandArgument = booking_patient_id.ToString();

                btnBkPtOfferingsCancelAdd.CommandName = "CancelAddOffering";
                btnBkPtOfferingsCancelAdd.CommandArgument = booking_patient_id.ToString();
            }
            else
            {
                ddlOfferings.Items.Clear();
                ddlOfferings.Visible = false;
                br.Visible = false;
                btnBkPtOfferingsCancelAdd.Visible = false;

                btnBkPtOfferingsAdd.CommandName = "ShowOfferingsDLL";
                btnBkPtOfferingsAdd.CommandArgument = booking_patient_id.ToString();
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
        if (!UserView.GetInstance().IsAgedCareView)
        {
            SetErrorMessage("Not logged into the Aged Care site.");
            return;
        }

        UpdateAreaTreated();

        Label lblId = (Label)GrdBookingPatients.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlACInvOffering = (DropDownList)GrdBookingPatients.Rows[e.RowIndex].FindControl("ddlACInvOffering");


        DataTable dt = Session["bookingpatients_data"] as DataTable;
        DataRow[] foundRows = dt.Select("bp_booking_patient_id=" + lblId.Text);
        DataRow row = foundRows[0];

        BookingPatient bookingPatient = BookingPatientDB.LoadAll(row);

        System.Collections.Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
        if (bookingPatient.Patient.ACInvOffering != null)
            bookingPatient.Patient.ACInvOffering = (Offering)offeringsHash[bookingPatient.Patient.ACInvOffering.OfferingID];
        if (bookingPatient.Patient.ACPatOffering != null)
            bookingPatient.Patient.ACPatOffering = (Offering)offeringsHash[bookingPatient.Patient.ACPatOffering.OfferingID];


        int acInvOfferingID_New = bookingPatient.Patient.ACInvOffering == null ? -1 : bookingPatient.Patient.ACInvOffering.OfferingID;
        int acPatOfferingID_New = bookingPatient.Patient.ACPatOffering == null ? -1 : bookingPatient.Patient.ACPatOffering.OfferingID;

        if (bookingPatient.Patient.ACInvOffering == null || bookingPatient.Patient.ACPatOffering == null)
        {
            acInvOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
            acPatOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
        }
        else if (bookingPatient.Patient.ACInvOffering.OfferingID != Convert.ToInt32(ddlACInvOffering.SelectedValue))
        {
            int acInvOfferingID_Old = bookingPatient.Patient.ACInvOffering == null ? -1 : bookingPatient.Patient.ACInvOffering.OfferingID;
            int acPatOfferingID_Old = bookingPatient.Patient.ACPatOffering == null ? -1 : bookingPatient.Patient.ACPatOffering.OfferingID;

            acInvOfferingID_New = Convert.ToInt32(ddlACInvOffering.SelectedValue);
            acPatOfferingID_New = acPatOfferingID_Old;

            int acInvAcPtTypeID_New = ((Offering)offeringsHash[Convert.ToInt32(ddlACInvOffering.SelectedValue)]).AgedCarePatientType.ID;
            int acInvAcPtTypeID_Old = bookingPatient.Patient.ACInvOffering.AgedCarePatientType.ID;

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



        PatientHistoryDB.Insert(bookingPatient.Patient.PatientID, bookingPatient.Patient.IsClinicPatient, bookingPatient.Patient.IsGPPatient, bookingPatient.Patient.IsDeleted, bookingPatient.Patient.IsDeceased,
                                bookingPatient.Patient.FlashingText, bookingPatient.Patient.FlashingTextAddedBy == null ? -1 : bookingPatient.Patient.FlashingTextAddedBy.StaffID, bookingPatient.Patient.FlashingTextLastModifiedDate, bookingPatient.Patient.PrivateHealthFund, bookingPatient.Patient.ConcessionCardNumber, bookingPatient.Patient.ConcessionCardExpiryDate, bookingPatient.Patient.IsDiabetic, bookingPatient.Patient.IsMemberDiabetesAustralia, bookingPatient.Patient.DiabeticAAassessmentReviewDate, bookingPatient.Patient.ACInvOffering == null ? -1 : bookingPatient.Patient.ACInvOffering.OfferingID, bookingPatient.Patient.ACPatOffering == null ? -1 : bookingPatient.Patient.ACPatOffering.OfferingID, bookingPatient.Patient.Login, bookingPatient.Patient.Pwd, bookingPatient.Patient.IsCompany, bookingPatient.Patient.ABN, bookingPatient.Patient.NextOfKinName, bookingPatient.Patient.NextOfKinRelation, bookingPatient.Patient.NextOfKinContactInfo, 
                                bookingPatient.Patient.Person.Title.ID, bookingPatient.Patient.Person.Firstname, bookingPatient.Patient.Person.Middlename, bookingPatient.Patient.Person.Surname, bookingPatient.Patient.Person.Nickname, bookingPatient.Patient.Person.Gender, bookingPatient.Patient.Person.Dob, Convert.ToInt32(Session["StaffID"]));

        PatientDB.Update(bookingPatient.Patient.PatientID, bookingPatient.Patient.Person.PersonID, bookingPatient.Patient.IsClinicPatient, bookingPatient.Patient.IsGPPatient, bookingPatient.Patient.IsDeceased, bookingPatient.Patient.FlashingText, bookingPatient.Patient.FlashingTextAddedBy == null ? -1 : bookingPatient.Patient.FlashingTextAddedBy.StaffID, bookingPatient.Patient.FlashingTextLastModifiedDate, bookingPatient.Patient.PrivateHealthFund, bookingPatient.Patient.ConcessionCardNumber, bookingPatient.Patient.ConcessionCardExpiryDate, bookingPatient.Patient.IsDiabetic, bookingPatient.Patient.IsMemberDiabetesAustralia, bookingPatient.Patient.DiabeticAAassessmentReviewDate, acInvOfferingID_New, acPatOfferingID_New, bookingPatient.Patient.Login, bookingPatient.Patient.Pwd, bookingPatient.Patient.IsCompany, bookingPatient.Patient.ABN, bookingPatient.Patient.NextOfKinName, bookingPatient.Patient.NextOfKinRelation, bookingPatient.Patient.NextOfKinContactInfo);


        GrdBookingPatients.EditIndex = -1;
        GrdPatientList.EditIndex = -1;
        FillGrid_PatientList();
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

                Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
                PatientDB.AddACOfferings(ref offeringsHash, ref bp);
                bool isDva = bp.Patient.ACInvOffering.AgedCarePatientType.ID == 9;

                string areaTreated = string.Empty;
                if (isDva)
                    areaTreated = HealthCardDB.GetActiveByPatientID(bp.Patient.PatientID).AreaTreated;

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

    protected Hashtable GetBedroomHash(int[] entityIDs, bool padZeros = false)
    {

        Hashtable bedroomHash = new Hashtable();
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Hashtable contactHash = ContactDB.GetHashByEntityIDs(-1, entityIDs, 166, -1);
            foreach (int key in contactHash.Keys)
                bedroomHash[key] = PadRoomNbr(((Contact[])contactHash[key])[0].AddrLine1.Trim(), padZeros);
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            Hashtable contactAusHash = ContactAusDB.GetHashByEntityIDs(-1, entityIDs, 166, -1);
            foreach (int key in contactAusHash.Keys)
                bedroomHash[key] = PadRoomNbr(((ContactAus[])contactAusHash[key])[0].AddrLine1.Trim(), padZeros);
        }
        else
        {
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
        }

        return bedroomHash;
    }
    protected string PadRoomNbr(string room, bool padZeros = false)
    {
        if (!padZeros)
            return room;

        room = room.Trim();

        char[] roomArray = room.ToCharArray();


        string numberPart = string.Empty;
        string rest       = string.Empty;

        int i = 0;
        for ( ; i < roomArray.Length; i++)
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


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        UpdateAreaTreated();

        PageCompletionType pageCompletionType = GetUrlParamType();
        bool isHourly = pageCompletionType == PageCompletionType.MultiPTs_SingleHourlyInvoice || pageCompletionType == PageCompletionType.SinglePT_SingleHourlyInvoice;

        if (!isHourly)
            CreatInvoices_Standard();
        else
            CreatInvoices_Hourly();
    }

    protected void CreatInvoices_Standard()
    {
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

            int[] patientIDs        = new int[bookingPatients.Length];
            int[] bookingPatientIDs = new int[bookingPatients.Length];
            for (int i = 0; i < bookingPatients.Length; i++)
            {
                if (bookingPatients[i].Patient.ACInvOffering == null || bookingPatients[i].Patient.ACPatOffering == null)
                {
                    SetErrorMessage(bookingPatients[i].Patient.Person.FullnameWithoutMiddlename + " does not have an 'PT Type' set. Please set one before completing.");
                    return;
                }

                bookingPatientIDs[i] = bookingPatients[i].BookingPatientID;
                patientIDs[i]        = bookingPatients[i].Patient.PatientID;
            }

            // caches for less db lookups to handle many patients
            Hashtable bpoHash                            = BookingPatientOfferingDB.GetHashtable(bookingPatientIDs);
            Hashtable patientHealthCardCache             = PatientsHealthCardsCacheDB.GetBullkActive(patientIDs);
            Hashtable epcRemainingCache                  = GetEPCRemainingCache(patientHealthCardCache);
            Hashtable patientsMedicareCountThisYearCache = PatientsMedicareCardCountThisYearCacheDB.GetBullk(patientIDs, DateTime.Today.Year);
            Hashtable patientsEPCRemainingCache          = PatientsEPCRemainingCacheDB.GetBullk(patientIDs, DateTime.Today.AddYears(-1));
            Hashtable patientReferrerCache               = PatientReferrerDB.GetEPCReferrersOf(patientIDs, true);
            int       MedicareMaxNbrServicesPerYear      = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);

            Hashtable offeringsHashPrices = GetOfferingHashtable(booking);

            /*
             [done] LCF            - combined inv's   [fac]                            - 
             [done] LC             - individual inv's [pt]  [fac if no pt addr]        - 
             [done] HC             - combined inv's   [fac]                            - 

             [done] DVA            - combined inv's   [dva]                            - 
             [done] Medicare       - combined inv's   [medicare]                       - only medicare .. if aditinoal items, create seperate invoice for patient

             [done] LC Emergency   - individual inv's [pt]  [fac if no pt addr]        - 
             [done] HC Emergency   - individual inv's [fac]   
            */

            /*
            delete InvoiceLine where invoice_id > 130904
            delete Invoice where booking_id = 93602
            update Booking set booking_status_id = 0 where booking_id = 93602

            select * from Invoice where booking_id = 93602
            select * from InvoiceLine where invoice_id > 130902
            */


            int       MC_Invoice_NextID    = 1;
            ArrayList MC_Invoices          = new ArrayList();
            Hashtable MC_InvoiceLines      = new Hashtable();

            ArrayList EPCRemaining_Changes = new ArrayList(); // Tuple<int,int,int> (epcRemaining.ID, curNum, newNum)
            ArrayList EPCRefLetterInfo     = new ArrayList(); // Tuple<BookingPatient, int, RegisterReferrer, Booking.InvoiceType, Healthcard, int> (bookingPatient, fieldID, ptReferrer, invType, hc, epcremainingAfter)


            int       DVA_Invoice_NextID   = 1;
            ArrayList DVA_Invoices         = new ArrayList();
            Hashtable DVA_InvoiceLines     = new Hashtable();


            int       LC_Invoice_NextID    = 1;
            ArrayList LC_Invoices          = new ArrayList();
            Hashtable LC_InvoiceLines      = new Hashtable();

            ArrayList LCF_InvoiceLines     = new ArrayList();
            ArrayList HC_InvoiceLines      = new ArrayList();

            int       LCE_Invoice_NextID   = 1;
            ArrayList LCE_Invoices         = new ArrayList();
            Hashtable LCE_InvoiceLines     = new Hashtable();

            int       HCE_Invoice_NextID   = 1;
            ArrayList HCE_Invoices         = new ArrayList();
            Hashtable HCE_InvoiceLines     = new Hashtable();


            // used to check update stock and check warning level emails sent
            ArrayList invoiceLinesExtras = new ArrayList();

            for (int i = 0; i < bookingPatients.Length; i++)
            {
                BookingPatient           bp   = bookingPatients[i];
                BookingPatientOffering[] bpos = bpoHash[bp.BookingPatientID] == null ? new BookingPatientOffering[] { } : (BookingPatientOffering[])((ArrayList)bpoHash[bp.BookingPatientID]).ToArray(typeof(BookingPatientOffering));

                // change to use org specific price
                for (int j = 0; j < bpos.Length; j++)
                    if (offeringsHashPrices[bpos[j].Offering.OfferingID] != null)
                        bpos[j].Offering = (Offering)offeringsHashPrices[bpos[j].Offering.OfferingID];


                // change to use org specific price - but keep IsGstExempt
                if (offeringsHashPrices[bp.Patient.ACInvOffering.OfferingID] != null)
                {
                    bool isGstExempt = bp.Patient.ACInvOffering.IsGstExempt;
                    bp.Patient.ACInvOffering = (Offering)offeringsHashPrices[bp.Patient.ACInvOffering.OfferingID];
                    bp.Patient.ACInvOffering.IsGstExempt = isGstExempt;
                }
                if (offeringsHashPrices[bp.Patient.ACPatOffering.OfferingID] != null)
                {
                    bool isGstExempt = bp.Patient.ACPatOffering.IsGstExempt;
                    bp.Patient.ACPatOffering = (Offering)offeringsHashPrices[bp.Patient.ACPatOffering.OfferingID];
                    bp.Patient.ACPatOffering.IsGstExempt = isGstExempt;
                }

                // use the field of the provider for what epcremaining field to count down in medicare    [[ do this AFTER changing to org specific price ]]
                bp.Patient.ACInvOffering.Field.ID = booking.Provider.Field.ID;
                bp.Patient.ACPatOffering.Field.ID = booking.Provider.Field.ID;


                if (bp.Patient.ACInvOffering.AgedCarePatientType.ID == 8) // Medicare
                {
                    HealthCard               hc            = GetHealthCardFromCache(patientHealthCardCache, bp.Patient.PatientID);
                    HealthCardEPCRemaining[] epcsRemaining = hc == null ? new HealthCardEPCRemaining[] { } : GetEPCRemainingFromCache(epcRemainingCache, hc, booking.Provider.Field.ID);
                    Booking.InvoiceType      invType       = booking.GetInvoiceType(hc, bp.Patient.ACInvOffering, bp.Patient, patientsMedicareCountThisYearCache, epcRemainingCache, MedicareMaxNbrServicesPerYear);

                    if (invType == Booking.InvoiceType.Medicare)
                    {
                        MC_Invoices.Add(new Invoice(MC_Invoice_NextID, -1, 363, booking.BookingID, -1, -1, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, bp.Patient.ACInvOffering.MedicareCharge, 0, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));

                        MC_InvoiceLines[MC_Invoice_NextID] = new ArrayList();
                        ((ArrayList)MC_InvoiceLines[MC_Invoice_NextID]).Add(new InvoiceLine(-1, MC_Invoice_NextID, bp.Patient.PatientID, bp.Patient.ACInvOffering.OfferingID, 1, bp.Patient.ACInvOffering.MedicareCharge, 0, "", "", -1));  // HERE

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

                                InvoiceLine invoiceLine = new InvoiceLine(-1, MC_Invoice_NextID, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst, "", "", -1);  // HERE
                                ((ArrayList)MC_InvoiceLines[MC_Invoice_NextID]).Add(invoiceLine);
                                invoiceLinesExtras.Add(invoiceLine);
                            }

                            MC_Invoice_NextID++;
                        }
                    }
                    else
                    {
                        bp.Patient.ACInvOffering = bp.Patient.ACPatOffering; // this will get run in the below code..
                    }
                }
                else if (bp.Patient.ACInvOffering.AgedCarePatientType.ID == 9) // DVA
                {
                    HealthCard               hc            = GetHealthCardFromCache(patientHealthCardCache, bp.Patient.PatientID);
                    HealthCardEPCRemaining[] epcsRemaining = hc == null ? new HealthCardEPCRemaining[] { } : HealthCardEPCRemainingDB.GetByHealthCardID(hc.HealthCardID, booking.Provider.Field.ID);
                    Booking.InvoiceType      invType       = booking.GetInvoiceType(hc, bp.Patient.ACInvOffering, bp.Patient, patientsMedicareCountThisYearCache, epcRemainingCache, MedicareMaxNbrServicesPerYear);

                    /*
                     * anyone can have medicare
                     * but only LC/LCF can use DVA referral -- if not LC/LCF then cant pay by dva
                     */
                    if ((bp.Patient.ACPatOffering.AgedCarePatientType.ID == 2 || bp.Patient.ACPatOffering.AgedCarePatientType.ID == 4) && invType == Booking.InvoiceType.DVA)
                    {
                        decimal total = bp.Patient.ACInvOffering.DvaCharge;
                        decimal gst   = total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                        for (int j = 0; j < bpos.Length; j++)
                        {
                            total += bpos[j].Quantity * bpos[j].Offering.DvaCharge;
                            gst   += bpos[j].Quantity * bpos[j].Offering.DvaCharge * (bpos[j].Offering.IsGstExempt ? (decimal)0 : (GST_Percent) / (decimal)100);
                        }

                        DVA_Invoices.Add(new Invoice(DVA_Invoice_NextID, -1, 363, booking.BookingID, -2, -1, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, total + gst, gst, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));

                        DVA_InvoiceLines[DVA_Invoice_NextID] = new ArrayList();
                        decimal line_total = bp.Patient.ACInvOffering.DvaCharge;
                        decimal line_gst   = line_total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                        ((ArrayList)DVA_InvoiceLines[DVA_Invoice_NextID]).Add(new InvoiceLine(-1, DVA_Invoice_NextID, bp.Patient.PatientID, bp.Patient.ACInvOffering.OfferingID, 1, line_total + line_gst, line_gst, bp.AreaTreated, "", -1));  // HERE
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
                    else
                    {
                        bp.Patient.ACInvOffering = bp.Patient.ACPatOffering; // this will get run in the below code..
                    }
                }




                if (bp.Patient.ACInvOffering.AgedCarePatientType.ID == 2) // LC
                {
                    decimal total = bp.Patient.ACInvOffering.DefaultPrice;
                    decimal gst   = total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        total += bpos[j].Quantity * bpos[j].Offering.DefaultPrice;
                        gst   += bpos[j].Quantity * bpos[j].Offering.DefaultPrice * (bpos[j].Offering.IsGstExempt ? (decimal)0 : (GST_Percent) / (decimal)100);
                    }

                    LC_Invoices.Add(new Invoice(LC_Invoice_NextID, -1, 363, booking.BookingID, 0, bp.Patient.PatientID, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, total + gst, gst, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));

                    LC_InvoiceLines[LC_Invoice_NextID] = new ArrayList();
                    decimal line_total = bp.Patient.ACInvOffering.DefaultPrice;
                    decimal line_gst   = line_total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    ((ArrayList)LC_InvoiceLines[LC_Invoice_NextID]).Add(new InvoiceLine(-1, LC_Invoice_NextID, bp.Patient.PatientID, bp.Patient.ACInvOffering.OfferingID, 1, line_total + line_gst, line_gst, "", "", -1));  // HERE
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        decimal bpos_total = Convert.ToDecimal(bpos[j].Quantity) * bpos[j].Offering.DefaultPrice;
                        decimal bpos_gst   = bpos_total * (bpos[j].Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);

                        InvoiceLine invoiceLine = new InvoiceLine(-1, LC_Invoice_NextID, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst, "", "", -1);  // HERE
                        ((ArrayList)LC_InvoiceLines[LC_Invoice_NextID]).Add(invoiceLine);
                        invoiceLinesExtras.Add(invoiceLine);
                    }

                    LC_Invoice_NextID++;
                }
                else if (bp.Patient.ACInvOffering.AgedCarePatientType.ID == 4) // LCF
                {
                    decimal total = bp.Patient.ACInvOffering.DefaultPrice;
                    decimal gst   = total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    LCF_InvoiceLines.Add(new InvoiceLine(-1, -1, bp.Patient.PatientID, bp.Patient.ACInvOffering.OfferingID, 1, total + gst, gst, "", "", -1));  // HERE

                    for (int j = 0; j < bpos.Length; j++)
                    {
                        decimal bpos_total = Convert.ToDecimal(bpos[j].Quantity) * bpos[j].Offering.DefaultPrice;
                        decimal bpos_gst   = bpos_total * (bpos[j].Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);

                        InvoiceLine invoiceLine = new InvoiceLine(-1, -1, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst, "", "", -1);  // HERE
                        LCF_InvoiceLines.Add(invoiceLine);
                        invoiceLinesExtras.Add(invoiceLine);
                    }
                }
                else if (bp.Patient.ACInvOffering.AgedCarePatientType.ID == 3 || bp.Patient.ACInvOffering.AgedCarePatientType.ID == 5) // HC/HCU
                {
                    decimal total = bp.Patient.ACInvOffering.DefaultPrice;
                    decimal gst   = total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    HC_InvoiceLines.Add(new InvoiceLine(-1, -1, bp.Patient.PatientID, bp.Patient.ACInvOffering.OfferingID, 1, total + gst, gst, "", "", -1));  // HERE

                    for (int j = 0; j < bpos.Length; j++)
                    {
                        decimal bpos_total = Convert.ToDecimal(bpos[j].Quantity) * bpos[j].Offering.DefaultPrice;
                        decimal bpos_gst   = bpos_total * (bpos[j].Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);

                        InvoiceLine invoiceLine = new InvoiceLine(-1, -1, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst, "", "", -1);  // HERE
                        HC_InvoiceLines.Add(invoiceLine);
                        invoiceLinesExtras.Add(invoiceLine);
                    }
                }
                else if (bp.Patient.ACInvOffering.AgedCarePatientType.ID == 6) // LCE
                {
                    decimal total = bp.Patient.ACInvOffering.DefaultPrice;
                    decimal gst   = total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    for(int j=0; j<bpos.Length; j++)
                    {
                        total += bpos[j].Quantity * bpos[j].Offering.DefaultPrice;
                        gst   += bpos[j].Quantity * bpos[j].Offering.DefaultPrice * (bpos[j].Offering.IsGstExempt ? (decimal)0 : (GST_Percent) / (decimal)100);
                    }



                    LCE_InvoiceLines[LCE_Invoice_NextID] = new ArrayList();
                    LCE_Invoices.Add(new Invoice(LCE_Invoice_NextID, -1, 363, booking.BookingID, 0, bp.Patient.PatientID, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, total + gst, gst, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));
                    decimal line_total = bp.Patient.ACInvOffering.DefaultPrice;
                    decimal line_gst   = line_total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    ((ArrayList)LCE_InvoiceLines[LCE_Invoice_NextID]).Add(new InvoiceLine(-1, LCE_Invoice_NextID, bp.Patient.PatientID, bp.Patient.ACInvOffering.OfferingID, 1, line_total + line_gst, line_gst, "", "", -1));  // HERE
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        decimal bpos_total = Convert.ToDecimal(bpos[j].Quantity) * bpos[j].Offering.DefaultPrice;
                        decimal bpos_gst   = bpos_total * (bpos[j].Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);

                        InvoiceLine invoiceLine = new InvoiceLine(-1, LCE_Invoice_NextID, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst, "", "", -1);  // HERE
                        ((ArrayList)LCE_InvoiceLines[LCE_Invoice_NextID]).Add(invoiceLine);
                        invoiceLinesExtras.Add(invoiceLine);
                    }

                    LCE_Invoice_NextID++;
                }
                else if (bp.Patient.ACInvOffering.AgedCarePatientType.ID == 7) // HCE
                {
                    decimal total = bp.Patient.ACInvOffering.DefaultPrice;
                    decimal gst   = total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        total += bpos[j].Quantity * bpos[j].Offering.DefaultPrice;
                        gst   += bpos[j].Quantity * bpos[j].Offering.DefaultPrice * (bpos[j].Offering.IsGstExempt ? (decimal)0 : (GST_Percent) / (decimal)100);
                    }

                    HCE_InvoiceLines[HCE_Invoice_NextID] = new ArrayList();
                    HCE_Invoices.Add(new Invoice(HCE_Invoice_NextID, -1, 363, booking.BookingID, booking.Organisation.OrganisationID, -1, 0, "", -1, "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), DateTime.Now, total + gst, gst, 0, 0, 0, 0, false, false, false, -1, DateTime.MinValue, DateTime.MinValue));
                    decimal line_total = bp.Patient.ACInvOffering.DefaultPrice;
                    decimal line_gst   = line_total * (bp.Patient.ACInvOffering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);
                    ((ArrayList)HCE_InvoiceLines[HCE_Invoice_NextID]).Add(new InvoiceLine(-1, HCE_Invoice_NextID, bp.Patient.PatientID, bp.Patient.ACInvOffering.OfferingID, 1, line_total + line_gst, line_gst, "", "", -1));  // HERE
                    for (int j = 0; j < bpos.Length; j++)
                    {
                        decimal bpos_total = Convert.ToDecimal(bpos[j].Quantity) * bpos[j].Offering.DefaultPrice;
                        decimal bpos_gst   = bpos_total * (bpos[j].Offering.IsGstExempt ? (decimal)0 : GST_Percent / (decimal)100);

                        InvoiceLine invoiceLine = new InvoiceLine(-1, HCE_Invoice_NextID, bp.Patient.PatientID, bpos[j].Offering.OfferingID, Convert.ToDecimal(bpos[j].Quantity), bpos_total + bpos_gst, bpos_gst, "", "", -1);  // HERE
                        ((ArrayList)HCE_InvoiceLines[HCE_Invoice_NextID]).Add(invoiceLine);
                        invoiceLinesExtras.Add(invoiceLine);
                    }

                    HCE_Invoice_NextID++;
                }
            }




            try
            {

                CreateInvoices(booking.BookingID, MC_Invoice_NextID,  MC_Invoices,  MC_InvoiceLines,  true,  ref invIDs, ref invLineIDs);  // Medicare
                foreach (Tuple<int,int,int> epcRemaining in EPCRemaining_Changes)
                    HealthCardEPCRemainingDB.UpdateNumServicesRemaining(epcRemaining.Item1, epcRemaining.Item3);

                CreateInvoices(booking.BookingID, DVA_Invoice_NextID, DVA_Invoices, DVA_InvoiceLines, true, ref invIDs, ref invLineIDs);   // DVA
                CreateInvoices(booking.BookingID, LC_Invoice_NextID,  LC_Invoices,  LC_InvoiceLines,  false, ref invIDs, ref invLineIDs);  // LC
                CreateInvoice (booking.BookingID, booking.Organisation.OrganisationID, -1, LCF_InvoiceLines, ref invIDs, ref invLineIDs);  // LCF
                CreateInvoice (booking.BookingID, booking.Organisation.OrganisationID, -1, HC_InvoiceLines,  ref invIDs, ref invLineIDs);  // HC
                CreateInvoices(booking.BookingID, LCE_Invoice_NextID, LCE_Invoices, LCE_InvoiceLines, false, ref invIDs, ref invLineIDs);  // LCE
                CreateInvoices(booking.BookingID, HCE_Invoice_NextID, HCE_Invoices, HCE_InvoiceLines, false, ref invIDs, ref invLineIDs);  // HCE

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

    protected void CreatInvoices_Hourly()
    {
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

            decimal hourly = 0;
            decimal hours = 0;
            if (!decimal.TryParse(txtHourlyPrice.Text, out hourly))
                throw new CustomMessageException("Hourly Price is not a valid number");
            if (!decimal.TryParse(txtTotalHours.Text, out hours))
                throw new CustomMessageException("Hours is not a valid number");

            ///////////////////
            // create invoice
            ///////////////////

            // keep id's to delete if exception and need to roll back

            ArrayList invIDs = new ArrayList();
            ArrayList invLineIDs = new ArrayList();


            // get list of patients and associated info

            BookingPatient[] bookingPatients = BookingPatientDB.GetByBookingID(booking.BookingID);
            if (bookingPatients.Length == 0)
                throw new CustomMessageException("No patients added");
            Hashtable offeringsHash = OfferingDB.GetHashtable(true, -1, null, true);
            PatientDB.AddACOfferings(ref offeringsHash, ref bookingPatients);


            ArrayList InvoiceLines = new ArrayList();
            for (int i = 0; i < bookingPatients.Length; i++)
                InvoiceLines.Add(new InvoiceLine(-1, -1, bookingPatients[i].Patient.PatientID, -1, 1, 0, 0, "", "", -1));    // HERE

            try
            {
                CreateInvoice(booking.BookingID, booking.Organisation.OrganisationID, -1, InvoiceLines, ref invIDs, ref invLineIDs, hourly, hours);

                // set booking as completed
                BookingDB.UpdateSetBookingStatusID(booking.BookingID, 187);
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

    protected void CreateInvoice(int BookingID, int PayerOrgID, int PayerPatientID, ArrayList InvoiceLines, ref ArrayList invIDs, ref ArrayList invLineIDs, decimal hourly = -1, decimal hours = -1)
    {
        if (InvoiceLines.Count > 0)
        {
            decimal total = 0;
            decimal gst   = 0;
            string message = string.Empty;

            if (hourly == -1 && hours == -1)
            {
                for (int i = 0; i < InvoiceLines.Count; i++)
                {
                    total += ((InvoiceLine)InvoiceLines[i]).Price;
                    gst   += ((InvoiceLine)InvoiceLines[i]).Tax;
                }
            }
            else
            {
                total = hourly * hours;
                message = hours.ToString("#.###") + " hours @ $" + String.Format("{0:0.00}", hourly) + "/hr = $" + String.Format("{0:0.00}", total);
            }

            int invoiceID = InvoiceDB.Insert(363, BookingID, PayerOrgID, PayerPatientID, 0, "", message, Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), total + gst, gst, false, false, false, DateTime.MinValue);
            invIDs.Add(invoiceID);

            for (int i = 0; i < InvoiceLines.Count; i++)
            {
                InvoiceLine invLine = (InvoiceLine)InvoiceLines[i];
                int invoiceLineID = InvoiceLineDB.Insert(invoiceID, invLine.Patient.PatientID, invLine.Offering == null ? -1 : invLine.Offering.OfferingID, invLine.Quantity, invLine.Price, invLine.Tax, invLine.AreaTreated, "", invLine.OfferingOrder == null ? -1 : invLine.OfferingOrder.OfferingOrderID);
                invLineIDs.Add(invoiceLineID);
            }
        }
    }

    protected void btnAddPatient_Click(object sender, EventArgs e)
    {
        Booking booking = BookingDB.GetByID(GetFormBookingID());
        if (!RegisterPatientDB.IsPatientRegisteredToOrg(Convert.ToInt32(lblPatientIDToAdd.Value), booking.Organisation.OrganisationID))
        {
            RegisterPatientDB.Insert(booking.Organisation.OrganisationID, Convert.ToInt32(lblPatientIDToAdd.Value));
            FillGrid_PatientList();
            FillGrid_BookingPatients();
        }
    }



    protected Hashtable GetOfferingHashtable(Booking booking = null)
    {
        if (booking == null)
        {
            booking = BookingDB.GetByID(GetFormBookingID());
            if (booking == null)
                throw new CustomMessageException("Invalid or no booking ID.");
        }

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
        hourly_price_table.Visible          = false;

        generate_system_letters_row.Visible = false;
        add_resident_row.Visible            = false;

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