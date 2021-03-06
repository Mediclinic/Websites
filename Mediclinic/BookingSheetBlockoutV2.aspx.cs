﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;

public partial class BookingSheetBlockoutV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();
            if (GetFormIsPopup())
                Utilities.UpdatePageHeaderV2(Page.Master, true);

            if (!IsPostBack)
            {
                SetupGUI();
            }
            else
            {
                UpdateList();
            }


            txtStartDate_Picker.OnClientClick = "displayDatePicker('txtStartDate', this, 'dmy', '-'); return false;";
            txtEndDate_Picker.OnClientClick   = "displayDatePicker('txtEndDate', this, 'dmy', '-'); return false;";

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


    #region GetUrlParamType(), IsValidFormID(), GetFormID()

    private bool IsValidFormOrgID()
    {
        string id = Request.QueryString["org"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormOrgID()
    {
        if (!IsValidFormOrgID())
            throw new Exception("Invalid url org");

        string id = Request.QueryString["org"];
        return Convert.ToInt32(id);
    }

    private bool IsValidFormStaffID()
    {
        string id = Request.QueryString["staff"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormStaffID()
    {
        if (!IsValidFormStaffID())
            throw new Exception("Invalid url staff");

        string id = Request.QueryString["staff"];
        return Convert.ToInt32(id);
    }

    protected DateTime GetFormDate()
    {
        try
        {
            string dateString = Request.QueryString["date"];
            if (dateString == null)
                return DateTime.MinValue;

            string[] parts = dateString.Split('_');
            if (parts.Length != 3)
                throw new InvalidExpressionException("Does not contain 3 parts seeperated by underscore :" + dateString);
            return new DateTime(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]));
        }
        catch (Exception)
        {
            return DateTime.MinValue;
        }
    }

    private enum UrlParamType { Org, Staff, Both, None };
    private UrlParamType GetUrlParamType()
    {
        if (Request.QueryString["org"] != null && Request.QueryString["staff"] != null)
            return UrlParamType.Both;
        else if (Request.QueryString["org"] != null)
            return UrlParamType.Org;
        else if (Request.QueryString["staff"] != null)
            return UrlParamType.Staff;
        else
            return UrlParamType.None;
    }

    private bool GetFormIsPopup()
    {
        return Request.QueryString["is_popup"] == null || Request.QueryString["is_popup"].ToString() == "1";
    }

    #endregion

    #region SetupGUI()

    public void SetupGUI()
    {

        Staff        staff = null;
        Organisation org   = null;

        if (IsValidFormStaffID())
        {
            staff = StaffDB.GetByID(GetFormStaffID());
            if (staff == null)
                throw new CustomMessageException("Invalid url staff");
            lblProvider.Text = staff.Person.FullnameWithoutMiddlename;
        }
        else
        {
            tr_provider_row.Visible       = false;
            tr_provider_only_row.Visible  = false;
            tr_provider_space_row.Visible = false;
        }

        if (IsValidFormOrgID())
        {
            org = OrganisationDB.GetByID(GetFormOrgID());
            if (org == null)
                throw new CustomMessageException("Invalid url org");
            lblOrganistion.Text = org.Name;
        }
        else
        {
            tr_org_row.Visible               = false;
            tr_organisation_only_row.Visible = false;
            tr_organisation_space_row.Visible    = false;
        }


        if (staff != null && org != null)
            lblHeading.Text = "Booking Sheet Blockout For " + staff.Person.FullnameWithoutMiddlename + " at " + org.Name;
        else if (staff != null)
            lblHeading.Text = "Booking Sheet Blockout For " + staff.Person.FullnameWithoutMiddlename;
        else if (org != null)
            lblHeading.Text = "Booking Sheet Blockout For " + org.Name;
        else
            lblHeading.Text = "Booking Sheet Blockout For All Clnics/Facilities";

        tblInfoOnSettingSpecificUnavailabilities.Visible = staff == null || org == null;

        DateTime date = GetFormDate();
        if (date == DateTime.MinValue)
            date = DateTime.Today;

        if (date.DayOfWeek == DayOfWeek.Sunday)
            chkSunday.Checked = true;
        if (date.DayOfWeek == DayOfWeek.Monday)
            chkMonday.Checked = true;
        if (date.DayOfWeek == DayOfWeek.Tuesday)
            chkTuesday.Checked = true;
        if (date.DayOfWeek == DayOfWeek.Wednesday)
            chkWednesday.Checked = true;
        if (date.DayOfWeek == DayOfWeek.Thursday)
            chkThursday.Checked = true;
        if (date.DayOfWeek == DayOfWeek.Friday)
            chkFriday.Checked = true;
        if (date.DayOfWeek == DayOfWeek.Saturday)
            chkSaturday.Checked = true;


        ddlStartHour.Items.Clear();
        ddlEndHour.Items.Clear();
        for (int i = 0; i < 24; i++)
        {
            ddlStartHour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0')));
            ddlEndHour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0')));
        }

        ddlStartMinute.Items.Clear();
        ddlEndMinute.Items.Clear();
        for (int i = 0; i < 60; i+=10)
        {
            ddlStartMinute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0')));
            ddlEndMinute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0')));
        }

        txtStartDate.Text = date == DateTime.MinValue ? DateTime.Now.ToString("dd-MM-yyyy") : date.ToString("dd-MM-yyyy");
        txtEndDate.Text   = date == DateTime.MinValue ? DateTime.Now.ToString("dd-MM-yyyy") : date.ToString("dd-MM-yyyy");

        ddlOrgUnavailabilityReason.Items.Add(new ListItem("[None]", "-1"));
        DataTable tblUnavailabilityReasons = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "BookingUnavailabilityReason", "", "descr", "booking_unavailability_reason_id", "booking_unavailability_reason_type_id", "descr");
        foreach (DataRow row in tblUnavailabilityReasons.Rows)
        {
            if (row["booking_unavailability_reason_type_id"].ToString() == "341")
                ddlProvUnavailabilityReason.Items.Add(new ListItem(row["descr"].ToString(), row["booking_unavailability_reason_id"].ToString()));
            else if (row["booking_unavailability_reason_type_id"].ToString() == "340")
                ddlOrgUnavailabilityReason.Items.Add(new ListItem(row["descr"].ToString(), row["booking_unavailability_reason_id"].ToString()));
        }

        ddlEveryNWeeks.Items.Clear();
        for (int i = 1; i <= 13; i++)
            ddlEveryNWeeks.Items.Add(new ListItem(i.ToString(), i.ToString()));

        if (IsValidFormStaffID())
            ddlOrgUnavailabilityReason.Visible  = false;
        else
            ddlProvUnavailabilityReason.Visible = false;

        UpdateList(org, staff);
    }

    protected void UpdateList()
    {
        Staff        staff = IsValidFormStaffID() ? StaffDB.GetByID(GetFormStaffID()) : null;
        Organisation org   = IsValidFormOrgID()   ? OrganisationDB.GetByID(GetFormOrgID()) : null;
        UpdateList(org, staff);
    }
    protected void UpdateList(Organisation org, Staff staff)
    {
        DataTable bookings = BookingDB.GetDataTable_Between(DateTime.MinValue, DateTime.MinValue, staff == null || !chkOnlyThisProvider.Checked ? null : new Staff[] { staff }, org == null || !chkOnlyThisOrganistion.Checked ? null : new Organisation[] { org }, null, null, false, null, true);
        for (int i = bookings.Rows.Count - 1; i >= 0; i--)
        {
            if (staff == null && bookings.Rows[i]["booking_provider"] != DBNull.Value)
            {
                bookings.Rows.RemoveAt(i);
                continue;
            }
            if (staff != null && chkOnlyThisProvider.Checked && (bookings.Rows[i]["booking_provider"] == DBNull.Value || Convert.ToInt32(bookings.Rows[i]["booking_provider"]) != staff.StaffID))
            {
                bookings.Rows.RemoveAt(i);
                continue;
            }
            if (org == null && bookings.Rows[i]["booking_organisation_id"] != DBNull.Value)
            {
                bookings.Rows.RemoveAt(i);
                continue;
            }
            if (org != null && chkOnlyThisOrganistion.Checked && (bookings.Rows[i]["booking_organisation_id"] == DBNull.Value || Convert.ToInt32(bookings.Rows[i]["booking_organisation_id"]) != org.OrganisationID))
            {
                bookings.Rows.RemoveAt(i);
                continue;
            }
        }

        DataView dataView = new DataView(bookings);
        dataView.Sort = "booking_date_start DESC";
        bookings = dataView.ToTable();

        DataTable individualBookings = bookings.Copy();
        DataTable recurringBookings  = bookings.Copy();
        for (int i = bookings.Rows.Count - 1; i >= 0; i--)
        {
            if (Convert.ToBoolean(bookings.Rows[i]["booking_is_recurring"]))
                individualBookings.Rows.RemoveAt(i);
            else
                recurringBookings.Rows.RemoveAt(i);
        }

        lstIndividualBookings.DataSource = individualBookings;
        lstIndividualBookings.DataBind();

        lstRecurringBookings.DataSource = recurringBookings;
        lstRecurringBookings.DataBind();
    }

    protected void lstIndividualBookings_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        if (e.Item.ItemType == ListItemType.Footer)
        {
            Control c = e.Item.FindControl("footerTableRow");
            c.Visible = lstIndividualBookings.Items.Count < 1;
        }
    }
    protected void lstRecurringBookings_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Footer)
        {
            Control c = e.Item.FindControl("footerTableRow");
            c.Visible = lstRecurringBookings.Items.Count < 1;
        }
    }

    protected void btnDelete_Command(object sender, CommandEventArgs e)
    {
        int booking_id = Convert.ToInt32(e.CommandArgument);
        BookingDB.Delete(booking_id);
        UpdateList();
    }

    #endregion


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {

            int staff_id = -1;
            Staff staff = null;
            if (IsValidFormStaffID() && chkOnlyThisProvider.Checked)
            {
                staff = StaffDB.GetByID(GetFormStaffID());
                if (staff == null)
                    throw new CustomMessageException("Invalid url staff");
                staff_id = staff.StaffID;
            }

            int org_id = 0;
            Organisation org = null;
            if (IsValidFormOrgID() && chkOnlyThisOrganistion.Checked)
            {
                org = OrganisationDB.GetByID(GetFormOrgID());
                if (org == null)
                    throw new CustomMessageException("Invalid url org");
                lblOrganistion.Text = org.Name;
            }


            int booking_type_id = org_id != 0 ? 341 : 342;

            // need to make sure at least one day is selected
            if (!chkSunday.Checked && !chkMonday.Checked && !chkTuesday.Checked && !chkWednesday.Checked &&
                !chkThursday.Checked && !chkFriday.Checked && !chkSaturday.Checked)
                throw new CustomMessageException("At least one day must be selected");

            string days = (chkSunday.Checked ? "1" : "0") + (chkMonday.Checked ? "1" : "0") + (chkTuesday.Checked ? "1" : "0") + (chkWednesday.Checked ? "1" : "0") +
                           (chkThursday.Checked ? "1" : "0") + (chkFriday.Checked ? "1" : "0") + (chkSaturday.Checked ? "1" : "0");


            bool allDay = chkAllDay.Checked;
            TimeSpan start_time = allDay ? new TimeSpan(0, 0, 0) : new TimeSpan(Convert.ToInt32(ddlStartHour.SelectedValue), Convert.ToInt32(ddlStartMinute.SelectedValue), 0);
            TimeSpan end_time = allDay ? new TimeSpan(23, 59, 0) : new TimeSpan(Convert.ToInt32(ddlEndHour.SelectedValue), Convert.ToInt32(ddlEndMinute.SelectedValue), 0);

            if (!allDay && (start_time >= end_time))
                throw new CustomMessageException("End time must be after start time");

            // need to check start date and end date are valid dates  (make another method to check this)    is_valid_date(txt_date)
            string start_date_text = txtStartDate.Text;
            string end_date_text = txtEndDate.Text;
            bool valid_start_date = Regex.IsMatch(start_date_text, @"^\d{2}\-\d{2}\-\d{4}$");
            bool valid_end_date = Regex.IsMatch(end_date_text, @"^\d{2}\-\d{2}\-\d{4}$");
            if (!valid_start_date)
                throw new CustomMessageException("Invalid start date - Must be in the format dd-mm-yyyy");
            if (!valid_end_date)
                throw new CustomMessageException("Invalid end date - Must be in the format dd-mm-yyyy");


            DateTime start_datetime = new DateTime(Convert.ToInt32(txtStartDate.Text.Substring(6, 4)), Convert.ToInt32(txtStartDate.Text.Substring(3, 2)), Convert.ToInt32(txtStartDate.Text.Substring(0, 2)));
            DateTime end_datetime = end_date_text.Length == 0 ? DateTime.MinValue : new DateTime(Convert.ToInt32(txtEndDate.Text.Substring(6, 4)), Convert.ToInt32(txtEndDate.Text.Substring(3, 2)), Convert.ToInt32(txtEndDate.Text.Substring(0, 2)));
            bool same_start_and_end_date = (start_datetime == end_datetime);
            int every_n_weeks = Convert.ToInt32(ddlEveryNWeeks.SelectedValue);


            // need to check that IF end date not null ... check 3nd date is after first date
            if (end_date_text.Length > 0)
            {
                if (start_datetime > end_datetime)
                    throw new CustomMessageException("End date must be after start date");

                // add one day to the end date because 7th-8th will want 8th included, so make it 7th 00:00 to 9th 00:00
                end_datetime = end_datetime.AddDays(1);
            }


            if (!same_start_and_end_date && every_n_weeks > 1 && radBookingSequenceTypeSeries.Checked)
                throw new CustomMessageException("For bookings less frequently than every 1 week, you must select \"Create seperate unavailabilities\"." +
                      ((end_date_text.Length > 0) ? "" : "\r\n" +
                      "\r\n" +
                      "You also must set an end date when creating seperate unavailabilities."));

            if (!same_start_and_end_date && every_n_weeks == 1 && !radBookingSequenceTypeSeperate.Checked && !radBookingSequenceTypeSeries.Checked)
                throw new CustomMessageException("Please select either \"Create seperate unavailabilities\" or \"Create single series\"" + "\r\n" +
                    "<small>" +
                    "Creating seperate unavailabilities - once created, deleting one of those day's unavailability will not remove other unavailabilities" + "\r\n" +
                    "Creating as a series - once created, deleting any instance of the series will remove all instances of this series" +
                    "</small>");

            if (radBookingSequenceTypeSeperate.Checked && end_date_text.Length == 0)
                throw new CustomMessageException("Can not select \"Create seperate unavailabilities\" without an end date" + "\r\n" +
                    "\r\n" +
                    "Either add an end date, or change to \"Create single series\"");

            bool create_as_series = !same_start_and_end_date && radBookingSequenceTypeSeries.Checked;
            if (every_n_weeks > 1) create_as_series = false;

            int unavailability_reason_id = -1;
            if (ddlProvUnavailabilityReason.Visible)
                unavailability_reason_id = Convert.ToInt32(ddlProvUnavailabilityReason.SelectedValue);
            if (ddlOrgUnavailabilityReason.Visible)
                unavailability_reason_id = Convert.ToInt32(ddlOrgUnavailabilityReason.SelectedValue);



            Booking[] bookings = BookingDB.GetToCheckOverlap_Recurring(start_datetime, end_datetime, start_time, end_time, days, staff, org, booking_type_id == 342, true, false, true);
            //if (Booking.HasOverlap(bookings, start_datetime, end_datetime, days, start_time, end_time, null))
            //    throw new CustomMessageException("Please move or delete existing bookings first.");
            Booking[] overlappingBookings = Booking.GetOverlappingBookings(bookings, start_datetime, end_datetime, days, start_time, end_time, every_n_weeks, null);
            if (overlappingBookings.Length > 0)
            {
                string space = "          ";
                string bookingDates = overlappingBookings.Length == 0 ? string.Empty : "<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">";
                for (int i = 0; i < overlappingBookings.Length; i++)
                {
                    string href = overlappingBookings[i].GetBookingSheetLink();
                    if (href.StartsWith("~/")) href = href.Substring(2);
                    string allFeatures = "dialogWidth:1500px;dialogHeight:1000px;center:yes;resizable:no; scroll:no";
                    string js          = "javascript:window.showModalDialog('" + href + "', '', '" + allFeatures + "');document.getElementById('btnUpdateEPCInfo').click();return false;";
                    string link = "<a href=\"#\" onclick=\"" + js + "\">" + (overlappingBookings[i].Patient != null ? overlappingBookings[i].Patient.Person.FullnameWithoutMiddlename : overlappingBookings[i].BookingID.ToString()) + "</a>";
                    bookingDates += "<tr><td>" + space + overlappingBookings[i].DateStart.ToString(@"ddd MMM d, yyy HH:mm") + "</td><td width=\"10\"></td><td>" + link + "</td></tr>";
                }
                bookingDates += overlappingBookings.Length == 0 ? string.Empty : "</table>";
                throw new CustomMessageException("Can not create an unavailability until these existing bookings have been deleted or moved:" + "<br /><small>" + bookingDates + "</small>");
            }


            // MAKE BOOKING FOR EACH WEEK DAY!
            bool madeAtLeastOneBooking = false;
            for (int i = 0; i < 7; i++)
            {
                if (days[i] != '1')
                    continue;
                DayOfWeek dayOfWeek = WeekDayDB.GetDayOfWeek(i + 1);


                if (create_as_series)
                {
                    BookingDB.Insert(start_datetime, end_datetime, org == null ? 0 : org.OrganisationID, staff == null ? -1 : staff.StaffID, -1, -1,
                                booking_type_id, 0, Convert.ToInt32(unavailability_reason_id), Convert.ToInt32(Session["StaffID"]), 1, Convert.ToInt32(Session["StaffID"]), DateTime.Now, -1, DateTime.MinValue, -1, DateTime.MinValue, false, false, false, true, dayOfWeek, start_time, end_time);
                    madeAtLeastOneBooking = true;
                }
                else
                {
                    // get which dates will occur .. and create individual bookings....
                    DateTime curStartDate = start_datetime;
                    while (curStartDate.DayOfWeek != dayOfWeek)
                        curStartDate = curStartDate.AddDays(1);

                    DateTime curStartDateTime = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, start_time.Hours, start_time.Minutes, 0);
                    DateTime curEndDateTime = new DateTime(curStartDate.Year, curStartDate.Month, curStartDate.Day, end_time.Hours, end_time.Minutes, 0);
                    int weekNbr = 0;
                    while ( (allDay && curStartDateTime.Date < end_datetime.Date) || (!allDay && curStartDateTime.Date <= end_datetime.Date) )
                    {
                        if (weekNbr % every_n_weeks == 0)
                        {
                            BookingDB.Insert(curStartDateTime, curEndDateTime, org == null ? 0 : org.OrganisationID, staff == null ? -1 : staff.StaffID, -1, -1,
                                    booking_type_id, 0, Convert.ToInt32(unavailability_reason_id), Convert.ToInt32(Session["StaffID"]), 1, Convert.ToInt32(Session["StaffID"]), DateTime.Now, -1, DateTime.MinValue, -1, DateTime.MinValue, false, false, false, false, curStartDateTime.DayOfWeek, TimeSpan.Zero, TimeSpan.Zero);
                            madeAtLeastOneBooking = true;
                        }

                        curStartDateTime = curStartDateTime.AddDays(7);
                        curEndDateTime   = curEndDateTime.AddDays(7);
                        weekNbr++;
                    }

                }

            }

            if (!madeAtLeastOneBooking)
                throw new CustomMessageException("No bookings made - please check that the day/s of week selected are within the dates specified.");

            UpdateList();

            // close this window
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=true;self.close();</script>");

        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
        }
        catch (Exception ex)
        {
            SetErrorMessage("", ex.ToString());
        }
    }

    // can not use single quote in .aspx page that is needed to put in character for
    // .PadLeft(2, '0')
    // so using this instead

    protected static string PadLeft(string s, int len, string padChar)
    {
        return s.PadLeft(len, padChar[0]);
    }

    #region HideTableAndSetErrorMessage, SetErrorMessage, HideErrorMessag

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
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