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

public partial class NotesV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);

            if (!IsPostBack)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "settab", "<script language=javascript>show_tab('tab1');</script>");

                if (!IsValidFormBKID())
                    throw new Exception("Invalid BK IDs");

                Booking booking = BookingDB.GetByEntityID(GetFormBKID());
                if (booking != null && booking.Patient != null)
                {
                    int age = Utilities.GetAge(booking.Patient.Person.Dob);
                    lnkToEntity.Text = booking.Patient.Person.FullnameWithoutMiddlename + (booking.Patient.Person.Dob == DateTime.MinValue ? "" : "<br />" + booking.Patient.Person.Dob.ToString("d MMM yyyy") + " (Age " + age + ")");

                    if (booking.Patient.Person.Dob != DateTime.MinValue)
                    {
                        //lnkToEntity.Style["font-size"] = "90%";
                        lnkToEntity.Font.Size = FontUnit.Large;
                    }


                    if (booking.Patient.IsCompany)
                        lblSterilisationCode.Text = "Purchase Order No.";
                }
                else
                {
                    lnkToEntity.Text = "(Group Booking)";
                }

                hiddenIsMobileDevice.Value = Utilities.IsMobileDevice(Request) ? "1" : "0";

                ResetSterilisationCode();
                ResetInformedConcent();
                RefillAllGrids();

                // set for use for saving note text in a cookie for when they are logged out and needs to be reset
                // but only for this user and this notes entity
                userID.Value = Session["StaffID"].ToString();
                entityID.Value = GetFormBKID().ToString();

                SetNotesTextFromCookie();
            }
            else
            {
                if (hiddenFieldSelectedTab.Value.Length > 0)
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "settab", "<script language=javascript>show_tab('" + hiddenFieldSelectedTab.Value + "');</script>");

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

    protected void SetNotesTextFromCookie()
    {
        string load_saved_note1 = GrdNote1.FooterRow == null ? "" : "load_note(document.getElementById('" + ((TextBox)GrdNote1.FooterRow.FindControl("txtNewText")).ClientID + "'), document.getElementById('" + userID.ClientID + "').value, document.getElementById('" + entityID.ClientID + "').value);";
        string load_saved_note2 = GrdNote2.FooterRow == null ? "" : "load_note(document.getElementById('" + ((TextBox)GrdNote2.FooterRow.FindControl("txtNewText")).ClientID + "'), document.getElementById('" + userID.ClientID + "').value, document.getElementById('" + entityID.ClientID + "').value);";
        string load_saved_note3 = GrdNote3.FooterRow == null ? "" : "load_note(document.getElementById('" + ((TextBox)GrdNote3.FooterRow.FindControl("txtNewText")).ClientID + "'), document.getElementById('" + userID.ClientID + "').value, document.getElementById('" + entityID.ClientID + "').value);";
        if (load_saved_note1.Length > 0 || load_saved_note2.Length > 0 || load_saved_note3.Length > 0)
            Page.ClientScript.RegisterStartupScript(this.GetType(), "set_notes", "<script language=javascript>" + load_saved_note1 + load_saved_note2 + load_saved_note3 + "</script>");
    }

    #region IsValidFormID, GetFormID

    private bool IsValidFormBKID()
    {
        string bk_id = Request.QueryString["bk_id"];
        return bk_id != null && Regex.IsMatch(bk_id, @"^\d+$") && EntityDB.IDExists(Convert.ToInt32(Request.QueryString["bk_id"]));
    }
    private int GetFormBKID()
    {
        if (!IsValidFormBKID())
            throw new Exception("Invalid url bk_id");
        return Convert.ToInt32(Request.QueryString["bk_id"]);
    }

    #endregion

    protected int ScreenID1 = 15;  // Provider Notes
    protected int ScreenID2 = 16;  // Body Chart Notes
    protected int ScreenID3 = 19;  // Session Notes

    #region ResetSterilisationCode(), btnUpdateSterilisationCode_Click

    protected void ResetSterilisationCode()
    {
        Booking booking = !IsValidFormBKID() ? null : BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
            txtSterilisationCode.Text = booking.SterilisationCode;
    }

    protected void btnUpdateSterilisationCode_Click(object sender, EventArgs e)
    {
        Booking booking = !IsValidFormBKID() ? null : BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
            BookingDB.UpdateSterilistionCode(booking.BookingID, txtSterilisationCode.Text.Trim());
        RefillAllGrids();
        SetNotesTextFromCookie();
    }

    #endregion

    #region ResetInformedConcent(), chkInformedConcent_CheckedChanged

    protected void ResetInformedConcent()
    {
        Booking booking = !IsValidFormBKID() ? null : BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
        {
            if (booking.InformedConsentAddedBy != null)
                booking.InformedConsentAddedBy = StaffDB.GetByID(booking.InformedConsentAddedBy.StaffID);

            chkInformedConcent.Checked = booking.InformedConsentAddedBy != null && booking.InformedConsentDate != DateTime.MinValue;
            chkInformedConcent.Attributes["title"] = chkInformedConcent.Checked ? "Added By" + booking.InformedConsentAddedBy.Person.FullnameWithoutMiddlename + " (" + booking.InformedConsentDate.ToString("d MMM, yyyy h:mm") + (booking.InformedConsentDate.Hour <= 12 ? "am" : "pm") + ")" : string.Empty;
        }
    }

    protected void chkInformedConcent_CheckedChanged(object sender, EventArgs e)
    {
        Booking booking = !IsValidFormBKID() ? null : BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
            BookingDB.UpdateInformedConsent(booking.BookingID, chkInformedConcent.Checked, Convert.ToInt32(Session["StaffID"]));

        ResetInformedConcent();
        RefillAllGrids();
        SetNotesTextFromCookie();
    }

    #endregion


    #region RefillAll()

    protected void RefillAllGrids()
    {
        GrdNote1_FillNoteGrid();
        GrdNote2_FillNoteGrid();
        GrdNote3_FillNoteGrid();
    }

    #endregion

    #region GrdNote1

    protected void DisallowAddEditIfNoPermissions1()
    {
        // if its a booking note
        // only allow add/edit if by the provider of the booking, or by a "principle" staff memeber

        UserView userView        = UserView.GetInstance();
        int      loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
        {
            bool canAddEdit = (booking.Provider != null && loggedInStaffID == booking.Provider.StaffID) || userView.IsPrincipal || userView.IsMasterAdmin || userView.IsStakeholder;
            if (!canAddEdit)
            {
                GrdNote1.FooterRow.Visible = false;
                for (int i = 0; i < GrdNote1.Columns.Count; i++)
                    if (GrdNote1.Columns[i].HeaderText.Trim() == ".")
                        GrdNote1.Columns[i].Visible = false;
            }
        }
    }

    protected void GrdNote1_FillNoteGrid()
    {
        if (!IsValidFormBKID())
        {
            if (!Utilities.IsDev() || Request.QueryString["id"] != null)
            {
                HideTableAndSetErrorMessage();
                return;
            }

            // can still view all if dev and no id set .. but no insert/edit
            GrdNote1.Columns[5].Visible = false;
        }


        DataTable dt = IsValidFormBKID() ? NoteDB.GetDataTable_ByEntityID(GetFormBKID(), null, true, true) : NoteDB.GetDataTable(true);

        Hashtable allowedNoteTypes = new Hashtable();
        DataTable noteTypes = ScreenNoteTypesDB.GetDataTable_ByScreenID(ScreenID1);
        for(int i=0; i<noteTypes.Rows.Count; i++)
            allowedNoteTypes[Convert.ToInt32(noteTypes.Rows[i]["note_type_id"])] = 1;

        for (int i = dt.Rows.Count - 1; i >= 0; i--)
            if (allowedNoteTypes[Convert.ToInt32(dt.Rows[i]["note_type_id"])] == null)
                dt.Rows.RemoveAt(i);


        UserView userView = UserView.GetInstance();

        bool canSeeModifiedBy = userView.IsStakeholder || userView.IsMasterAdmin;
        dt.Columns.Add("last_modified_note_info_visible", typeof(Boolean));
        for (int i = 0; i < dt.Rows.Count; i++)
            dt.Rows[i]["last_modified_note_info_visible"] = canSeeModifiedBy;


        ViewState["noteinfo1_data"] = dt;



        // add note info to hidden field to use when emailing notes

        string emailBodyText = string.Empty;

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
        {
            emailBodyText += @"<br /><br />
<u>Treatment Information</u>
<br />
<table border=""0"" cellpadding=""0"" cellspacing=""0"">" +
    (booking.Patient == null ? "" : @"<tr><td>Patient</td><td style=""width:10px;""></td><td>" + booking.Patient.Person.FullnameWithoutMiddlename + @"</td></tr>") +
    (booking.Offering == null ? "" : @"<tr><td>Service</td><td></td><td>" + booking.Offering.Name + @"</td></tr>") + @"
    <tr><td>Date</td><td></td><td>" + booking.DateStart.ToString("dd-MM-yyyy") + @"</td></tr>
    <tr><td>Provider</td><td></td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr>
</table>";
        }

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            Note n = NoteDB.Load(dt.Rows[i]);
            emailBodyText += "<br /><br /><u>Note (" + n.DateAdded.ToString("dd-MM-yyyy") + ")</u><br />" + n.Text.Replace(Environment.NewLine, "<br />");
        }
        emailText.Value = emailBodyText + "<br /><br />" + SystemVariableDB.GetByDescr("LettersEmailSignature").Value; ;



        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && ViewState["noteinfo1_sortexpression"] != null && ViewState["noteinfo1_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = ViewState["noteinfo1_sortexpression"].ToString();
                GrdNote1.DataSource = dataView;
            }
            else
            {
                GrdNote1.DataSource = dt;
            }


            try
            {
                GrdNote1.DataBind();
            }
            catch (Exception ex)
            {
                this.lblErrorMessage.Visible = true;
                this.lblErrorMessage.Text = ex.ToString();
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdNote1.DataSource = dt;
            GrdNote1.DataBind();

            int TotalColumns = GrdNote1.Rows[0].Cells.Count;
            GrdNote1.Rows[0].Cells.Clear();
            GrdNote1.Rows[0].Cells.Add(new TableCell());
            GrdNote1.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdNote1.Rows[0].Cells[0].Text = "No Record Found";
        }


        Tuple<string, string, string, string> refsEmailInfo = GetReferrersEmail();
        ImageButton btnEmail = GrdNote1.HeaderRow.FindControl("btnEmail") as ImageButton;
        if (refsEmailInfo != null)
        {
            btnEmail.Visible = true;
            ((HiddenField)GrdNote1.HeaderRow.FindControl("hiddenRefEmail")).Value = refsEmailInfo.Item1;
            ((HiddenField)GrdNote1.HeaderRow.FindControl("hiddenRefName")).Value = refsEmailInfo.Item2;
            ((HiddenField)GrdNote1.HeaderRow.FindControl("hiddenBookingOrg")).Value = refsEmailInfo.Item3;
            ((HiddenField)GrdNote1.HeaderRow.FindControl("HiddenBookingPatientName")).Value = refsEmailInfo.Item4;
        }
        else
        {
            btnEmail.Visible = false;
        }

        DisallowAddEditIfNoPermissions1(); // place this after databinding
    }
    protected void GrdNote1_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {
            e.Row.Cells[0].CssClass = "hiddencol";
            e.Row.Cells[1].CssClass = "hiddencol";
        }

        if (e.Row.RowType != DataControlRowType.Pager && !UserView.GetInstance().IsAdminView)
            e.Row.Cells[6].CssClass = "hiddencol";

        if (e.Row.RowType != DataControlRowType.Pager && (ScreenID1 == 16 || ScreenID1 == 17 || ScreenID1 == 18))
        {
            foreach (DataControlField col in GrdNote1.Columns)
                if (col.HeaderText.ToLower().Trim() == "type")
                    e.Row.Cells[GrdNote1.Columns.IndexOf(col)].CssClass = "hiddencol";
        }
        if (e.Row.RowType != DataControlRowType.Pager && (ScreenID1 != 16))
        {
            foreach (DataControlField col in GrdNote1.Columns)
                if (col.HeaderText.ToLower().Trim() == "body part")
                    e.Row.Cells[GrdNote1.Columns.IndexOf(col)].CssClass = "hiddencol";
        }

        if (e.Row.RowType == DataControlRowType.DataRow && e.Row.DataItem != null)
        {
            TextBox lblNote = (TextBox)e.Row.FindControl("lblNote");
            if (lblNote != null)
            {
                DataTable dt = ViewState["noteinfo1_data"] as DataTable;
                string text = dt.Rows[e.Row.RowIndex]["text"].ToString();
                string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                lblNote.Rows = lines.Length <= 1 ? 1 : lines.Length - 1;
            }
        }

        if (ScreenID1 == 17 && e.Row.RowType == DataControlRowType.Header)
            e.Row.Cells[5].Text = "Medication";

        if (ScreenID1 == 18 && e.Row.RowType == DataControlRowType.Header)
            e.Row.Cells[5].Text = "Medical Condition";

    }
    protected void GrdNote1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            UserView userView = UserView.GetInstance();

            // if there exists note with the type not in the list of types offered on this screen, use this to add below with "foundRows"
            DataTable allNoteTypes = DBBase.GetGenericDataTable(null, "NoteType", "note_type_id", "descr");
            DataTable allBodyParts = DBBase.GetGenericDataTable(null, "BodyPart", "body_part_id", "descr");

            DataTable noteTypes = ScreenNoteTypesDB.GetDataTable_ByScreenID(ScreenID1);
            DataTable sites = SiteDB.GetDataTable();

            DataTable dt = ViewState["noteinfo1_data"] as DataTable;
            bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
            if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblId = (Label)e.Row.FindControl("lblId");
                DataRow[] foundRows = dt.Select("note_id=" + lblId.Text);
                DataRow thisRow = foundRows[0];


                bool isDeleted = thisRow["deleted_by"] != DBNull.Value || thisRow["date_deleted"] != DBNull.Value;


                DropDownList ddlDate_Day = (DropDownList)e.Row.FindControl("ddlDate_Day");
                DropDownList ddlDate_Month = (DropDownList)e.Row.FindControl("ddlDate_Month");
                DropDownList ddlDate_Year = (DropDownList)e.Row.FindControl("ddlDate_Year");
                if (ddlDate_Day != null && ddlDate_Month != null && ddlDate_Year != null)
                {
                    ddlDate_Day.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Month.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Year.Items.Add(new ListItem("----", "-1"));

                    for (int i = 1; i <= 31; i++)
                        ddlDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = 1; i <= 12; i++)
                        ddlDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = DateTime.Today.Year - 1; i <= DateTime.Today.Year + 1; i++)
                        ddlDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                    if (thisRow["date_added"] != DBNull.Value)
                    {
                        DateTime Date = Convert.ToDateTime(thisRow["date_added"]);

                        ddlDate_Day.SelectedValue = Date.Day.ToString();
                        ddlDate_Month.SelectedValue = Date.Month.ToString();

                        int firstYearSelectable = Convert.ToInt32(ddlDate_Year.Items[1].Value);
                        int lastYearSelectable = Convert.ToInt32(ddlDate_Year.Items[ddlDate_Year.Items.Count - 1].Value);
                        if (Date.Year < firstYearSelectable)
                            ddlDate_Year.Items.Insert(1, new ListItem(Date.Year.ToString(), Date.Year.ToString()));
                        if (Date.Year > lastYearSelectable)
                            ddlDate_Year.Items.Add(new ListItem(Date.Year.ToString(), Date.Year.ToString()));

                        ddlDate_Year.SelectedValue = Date.Year.ToString();
                    }
                }


                DropDownList ddlNoteType = (DropDownList)e.Row.FindControl("ddlNoteType");
                if (ddlNoteType != null)
                {
                    ddlNoteType.DataSource     = noteTypes;
                    ddlNoteType.DataTextField  = "descr";
                    ddlNoteType.DataValueField = "note_type_id";
                    ddlNoteType.DataBind();

                    // if this note type is not in the list for this screen, add it to the edit list
                    bool found = false;
                    foreach (ListItem li in ddlNoteType.Items)
                        if (li.Value == thisRow["note_type_id"].ToString())
                            found = true;
                    if (!found)
                        ddlNoteType.Items.Add(new ListItem(thisRow["note_type_descr"].ToString(), thisRow["note_type_id"].ToString()));

                    ddlNoteType.SelectedValue = thisRow["note_type_id"].ToString();
                }

                DropDownList ddlBodyPart = (DropDownList)e.Row.FindControl("ddlBodyPart");
                if (ddlBodyPart != null)
                {

                    ddlBodyPart.Items.Clear();
                    ddlBodyPart.Items.Add(new ListItem("","-1"));
                    for(int i=0; i<allBodyParts.Rows.Count; i++)
                        ddlBodyPart.Items.Add(new ListItem(allBodyParts.Rows[i]["body_part_id"].ToString() + ". " + allBodyParts.Rows[i]["descr"].ToString(), allBodyParts.Rows[i]["body_part_id"].ToString()));

                    ddlBodyPart.SelectedValue = thisRow["body_part_id"].ToString();
                }


                ImageButton lnkEdit = (ImageButton)e.Row.FindControl("lnkEdit");
                if (lnkEdit != null)
                {
                    lnkEdit.Visible = !isDeleted && 
                    (userView.IsAdminView || 
                     (thisRow["added_by_staff_id"]    != DBNull.Value && Convert.ToInt32(thisRow["added_by_staff_id"])    == Convert.ToInt32(Session["StaffID"])) ||
                     (thisRow["modified_by_staff_id"] != DBNull.Value && Convert.ToInt32(thisRow["modified_by_staff_id"]) == Convert.ToInt32(Session["StaffID"])));
                }


                /*
                DropDownList ddlSite = (DropDownList)e.Row.FindControl("ddlSite");
                if (ddlSite != null)
                {
                    ddlSite.Items.Add(new ListItem("--", "-1"));
                    foreach (DataRow row in sites.Rows)
                        ddlSite.Items.Add(new ListItem(row["name"].ToString(), row["site_id"].ToString()));
                    ddlSite.SelectedValue = thisRow["site_id"].ToString();
                }
                */


                ImageButton lnkDelete = (ImageButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.Visible = userView.IsAdminView ||
                    (thisRow["added_by_staff_id"]    != DBNull.Value && Convert.ToInt32(thisRow["added_by_staff_id"])    == Convert.ToInt32(Session["StaffID"])) ||
                    (thisRow["modified_by_staff_id"] != DBNull.Value && Convert.ToInt32(thisRow["modified_by_staff_id"]) == Convert.ToInt32(Session["StaffID"]));

                    if (isDeleted)
                    {
                        lnkDelete.CommandName = "_UnDelete";
                        lnkDelete.ImageUrl = "~/images/tick-24.png";
                        lnkDelete.ToolTip = "Un-Delete";
                    }
                }

                if (isDeleted)
                {
                    e.Row.AddCssClass("deleted_note");
                    e.Row.Style["display"] = "none";
                    e.Row.Style["color"] = "gray";
                }


                Utilities.AddConfirmationBox(e);
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                    Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {

                DropDownList ddlDate_Day = (DropDownList)e.Row.FindControl("ddlNewDate_Day");
                DropDownList ddlDate_Month = (DropDownList)e.Row.FindControl("ddlNewDate_Month");
                DropDownList ddlDate_Year = (DropDownList)e.Row.FindControl("ddlNewDate_Year");
                if (ddlDate_Day != null && ddlDate_Month != null && ddlDate_Year != null)
                {
                    ddlDate_Day.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Month.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Year.Items.Add(new ListItem("----", "-1"));

                    for (int i = 1; i <= 31; i++)
                        ddlDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = 1; i <= 12; i++)
                        ddlDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = DateTime.Today.Year - 1; i <= DateTime.Today.Year + 1; i++)
                        ddlDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                    ddlDate_Day.SelectedValue = DateTime.Today.Day.ToString();
                    ddlDate_Month.SelectedValue = DateTime.Today.Month.ToString();
                    ddlDate_Year.SelectedValue = DateTime.Today.Year.ToString();

                }

                DropDownList ddlNoteType = (DropDownList)e.Row.FindControl("ddlNewNoteType");
                ddlNoteType.DataSource = noteTypes;
                ddlNoteType.DataBind();

                DropDownList ddlBodyPart = (DropDownList)e.Row.FindControl("ddlNewBodyPart");
                ddlBodyPart.Items.Add(new ListItem("", "-1"));
                for (int i = 0; i < allBodyParts.Rows.Count; i++)
                    ddlBodyPart.Items.Add(new ListItem(allBodyParts.Rows[i]["body_part_id"].ToString() + ". " + allBodyParts.Rows[i]["descr"].ToString(), allBodyParts.Rows[i]["body_part_id"].ToString()));


                // set note text in cookie in case user logged out, to keep note text for this user and this entity
                TextBox txtNewText = (TextBox)e.Row.FindControl("txtNewText");
                txtNewText.Attributes["onkeyup"] = "set_note(document.getElementById('" + txtNewText.ClientID + "'), document.getElementById('" + userID.ClientID + "').value, document.getElementById('" + entityID.ClientID + "').value);";

                /*
                DropDownList ddlSite = (DropDownList)e.Row.FindControl("ddlNewSite");
                ddlSite.Items.Add(new ListItem("--", "-1"));
                foreach (DataRow row in sites.Rows)
                    ddlSite.Items.Add(new ListItem(row["name"].ToString(), row["site_id"].ToString()));
                ddlSite.SelectedValue = Session["SiteID"].ToString();
                */
            }
        }
        catch (Exception ex)
        {
            if (Utilities.IsDev())
                throw;
            else
                HideTableAndSetErrorMessage(ex is CustomMessageException ? ex.Message : "");
        }
    }
    protected void GrdNote1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdNote1.Columns[9].Visible = true;
        GrdNote1.EditIndex = -1;
        RefillAllGrids();
    }
    protected void GrdNote1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdNote1.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlNoteType = (DropDownList)GrdNote1.Rows[e.RowIndex].FindControl("ddlNoteType");
        DropDownList ddlBodyPart = (DropDownList)GrdNote1.Rows[e.RowIndex].FindControl("ddlBodyPart");
        TextBox txtText = (TextBox)GrdNote1.Rows[e.RowIndex].FindControl("txtText");
        //DropDownList ddlSite = (DropDownList)GrdNote1.Rows[e.RowIndex].FindControl("ddlSite");
        DropDownList ddlDate_Day = (DropDownList)GrdNote1.Rows[e.RowIndex].FindControl("ddlDate_Day");
        DropDownList ddlDate_Month = (DropDownList)GrdNote1.Rows[e.RowIndex].FindControl("ddlDate_Month");
        DropDownList ddlDate_Year = (DropDownList)GrdNote1.Rows[e.RowIndex].FindControl("ddlDate_Year");

        DataTable dt = ViewState["noteinfo1_data"] as DataTable;
        DataRow[] foundRows = dt.Select("note_id=" + lblId.Text);
        Note note = NoteDB.Load(foundRows[0]);

        DateTime date = GetDate(ddlDate_Day.SelectedValue, ddlDate_Month.SelectedValue, ddlDate_Year.SelectedValue);
        NoteDB.Update(Convert.ToInt32(lblId.Text), date, Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(ddlNoteType.SelectedValue), Convert.ToInt32(ddlBodyPart.SelectedValue), txtText.Text, note.Site.SiteID);



        // if its a booking note
        // email admin so they know if a provider is sabotaging the system (it has happened before)

        int loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)  // if note is for a booking
        {

            int thresholdCharacters = 50;
            int totalCharactersBefore = note.Text.Trim().Length;
            int totalCharactersAfter = txtText.Text.Trim().Length;
            int difference = totalCharactersAfter - totalCharactersBefore;

            if (totalCharactersBefore > thresholdCharacters && totalCharactersAfter < thresholdCharacters && difference < -20)
            {
                string mailText = @"This is an administrative email to notify you that notes for a booking may have been deleted.

<u>Logged-in user performing the udate</u>
" + StaffDB.GetByID(loggedInStaffID).Person.FullnameWithoutMiddlename + @"

<u>Original Text (Characters: " + totalCharactersBefore + @")</u>
<font color=""blue"">" + note.Text.Replace(Environment.NewLine, "<br />") + @"</font>

<u>Updated Text (Characters: " + totalCharactersAfter + @")</u>
<font color=""blue"">" + txtText.Text.Replace(Environment.NewLine, "<br />") + @"</font>

<u>Booking details</u>
<table border=""0"" cellpadding=""2"" cellspacing=""2""><tr><td>Booking ID:</td><td>" + booking.BookingID + @"</td></tr><tr><td>Booking Date:</td><td>" + booking.DateStart.ToString("d MMM, yyyy") + " " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + @"</td></tr><tr><td>Organisation:</td><td>" + booking.Organisation.Name + @"</td></tr><tr><td>Provider:</td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr><tr><td>Patient:</td><td>" + (booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename + " [ID:" + booking.Patient.PatientID + "]") + @"</td></tr><tr><td>Status:</td><td>" + booking.BookingStatus.Descr + @"</td></tr></table>

Regards,
Mediclinic
";
                bool EnableDeletedBookingsAlerts = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableDeletedBookingsAlerts").Value) == 1;

                if (EnableDeletedBookingsAlerts && !Utilities.IsDev())
                    Emailer.AsyncSimpleEmail(
                        ((SystemVariables)Session["SystemVariables"])["Email_FromEmail"].Value,
                        ((SystemVariables)Session["SystemVariables"])["Email_FromName"].Value,
                        ((SystemVariables)Session["SystemVariables"])["AdminAlertEmail_To"].Value,
                        "Notification that booking notes may have been deleted",
                        mailText.Replace(Environment.NewLine, "<br />"),
                        true,
                        null);
            }
        }



        GrdNote1.Columns[7].Visible = true;
        GrdNote1.EditIndex = -1;
        RefillAllGrids();
    }
    protected void GrdNote1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdNote1.Rows[e.RowIndex].FindControl("lblId");
        int note_id = Convert.ToInt32(lblId.Text);

        try
        {
            //NoteDB.UpdateInactive(note_id);
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        RefillAllGrids();
    }
    protected void GrdNote1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            if (!IsValidFormBKID())
                throw new CustomMessageException();

            Label lblId = (Label)GrdNote1.FooterRow.FindControl("lblId");
            DropDownList ddlNoteType = (DropDownList)GrdNote1.FooterRow.FindControl("ddlNewNoteType");
            DropDownList ddlBodyPart = (DropDownList)GrdNote1.FooterRow.FindControl("ddlNewBodyPart");
            TextBox txtText = (TextBox)GrdNote1.FooterRow.FindControl("txtNewText");
            //DropDownList ddlSite = (DropDownList)GrdNote1.FooterRow.FindControl("ddlNewSite");
            DropDownList ddlDate_Day = (DropDownList)GrdNote1.FooterRow.FindControl("ddlNewDate_Day");
            DropDownList ddlDate_Month = (DropDownList)GrdNote1.FooterRow.FindControl("ddlNewDate_Month");
            DropDownList ddlDate_Year = (DropDownList)GrdNote1.FooterRow.FindControl("ddlNewDate_Year");

            if (!IsValidDate(ddlDate_Day.SelectedValue, ddlDate_Month.SelectedValue, ddlDate_Year.SelectedValue))
                return;

            DateTime date = GetDate(ddlDate_Day.SelectedValue, ddlDate_Month.SelectedValue, ddlDate_Year.SelectedValue);
            NoteDB.Insert(GetFormBKID(), date, Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(ddlNoteType.SelectedValue), Convert.ToInt32(ddlBodyPart.SelectedValue), txtText.Text, Convert.ToInt32(Session["SiteID"]));

            RefillAllGrids();

            string clear_saved_note = "clear_note(document.getElementById('" + ((TextBox)GrdNote1.FooterRow.FindControl("txtNewText")).ClientID + "'), document.getElementById('" + userID.ClientID + "').value, document.getElementById('" + entityID.ClientID + "').value);";
            ScriptManager.RegisterStartupScript(GrdNote1, this.GetType(), "unset_cookie", clear_saved_note, true);
        }
        if (e.CommandName == "_Delete")
        {
            NoteDB.SetDeleted(Convert.ToInt32(e.CommandArgument), Convert.ToInt32(Session["StaffID"]));
            RefillAllGrids();
        }
        if (e.CommandName == "_UnDelete")
        {
            NoteDB.SetNotDeleted(Convert.ToInt32(e.CommandArgument), Convert.ToInt32(Session["StaffID"]));
            RefillAllGrids();
        }
    }
    protected void GrdNote1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdNote1.Columns[9].Visible = false;
        GrdNote1.EditIndex = e.NewEditIndex;
        RefillAllGrids();
    }
    protected void GrdNote1_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdNote1.EditIndex >= 0)
            return;

        DataTable dataTable = ViewState["noteinfo1_data"] as DataTable;

        if (dataTable != null)
        {
            if (ViewState["noteinfo1_sortexpression"] == null)
                ViewState["noteinfo1_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = ViewState["noteinfo1_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            ViewState["noteinfo1_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdNote1.DataSource = dataView;
            GrdNote1.DataBind();
        }
    }

    #endregion

    #region GrdNote2

    protected void DisallowAddEditIfNoPermissions2()
    {
        // if its a booking note
        // only allow add/edit if by the provider of the booking, or by a "principle" staff memeber

        UserView userView        = UserView.GetInstance();
        int      loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
        {
            bool canAddEdit = (booking.Provider != null && loggedInStaffID == booking.Provider.StaffID) || userView.IsPrincipal || userView.IsMasterAdmin || userView.IsStakeholder;
            if (!canAddEdit)
            {
                GrdNote2.FooterRow.Visible = false;
                for (int i = 0; i < GrdNote2.Columns.Count; i++)
                    if (GrdNote2.Columns[i].HeaderText.Trim() == ".")
                        GrdNote2.Columns[i].Visible = false;
            }
        }
    }

    protected void GrdNote2_FillNoteGrid()
    {
        DataTable dt = NoteDB.GetDataTable_ByEntityID(GetFormBKID(), null, true, true);

        Hashtable allowedNoteTypes = new Hashtable();
        DataTable noteTypes = ScreenNoteTypesDB.GetDataTable_ByScreenID(ScreenID2);
        for(int i=0; i<noteTypes.Rows.Count; i++)
            allowedNoteTypes[Convert.ToInt32(noteTypes.Rows[i]["note_type_id"])] = 1;

        for (int i = dt.Rows.Count - 1; i >= 0; i--)
            if (allowedNoteTypes[Convert.ToInt32(dt.Rows[i]["note_type_id"])] == null)
                dt.Rows.RemoveAt(i);



        UserView userView = UserView.GetInstance();

        bool canSeeModifiedBy = userView.IsStakeholder || userView.IsMasterAdmin;
        dt.Columns.Add("last_modified_note_info_visible", typeof(Boolean));
        for (int i = 0; i < dt.Rows.Count; i++)
            dt.Rows[i]["last_modified_note_info_visible"] = canSeeModifiedBy;


        ViewState["noteinfo2_data"] = dt;


        // add note info to hidden field to use when emailing notes

        string emailBodyText = string.Empty;

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
        {
            emailBodyText += @"<br /><br />
<u>Treatment Information</u>
<br />
<table border=""0"" cellpadding=""0"" cellspacing=""0"">" +
    (booking.Patient == null ? "" : @"<tr><td>Patient</td><td style=""width:10px;""></td><td>" + booking.Patient.Person.FullnameWithoutMiddlename + @"</td></tr>") +
    (booking.Offering == null ? "" : @"<tr><td>Service</td><td></td><td>" + booking.Offering.Name + @"</td></tr>") + @"
    <tr><td>Date</td><td></td><td>" + booking.DateStart.ToString("dd-MM-yyyy") + @"</td></tr>
    <tr><td>Provider</td><td></td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr>
</table>";
        }

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            Note n = NoteDB.Load(dt.Rows[i]);
            emailBodyText += "<br /><br /><u>Note (" + n.DateAdded.ToString("dd-MM-yyyy") + ")</u><br />" + n.Text.Replace(Environment.NewLine, "<br />");
        }
        emailText.Value = emailBodyText + "<br /><br />" + SystemVariableDB.GetByDescr("LettersEmailSignature").Value; ;


        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && ViewState["noteinfo2_sortexpression"] != null && ViewState["noteinfo2_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = ViewState["noteinfo2_sortexpression"].ToString();
                GrdNote2.DataSource = dataView;
            }
            else
            {
                GrdNote2.DataSource = dt;
            }


            try
            {
                GrdNote2.DataBind();
            }
            catch (Exception ex)
            {
                this.lblErrorMessage.Visible = true;
                this.lblErrorMessage.Text = ex.ToString();
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdNote2.DataSource = dt;
            GrdNote2.DataBind();

            int TotalColumns = GrdNote2.Rows[0].Cells.Count;
            GrdNote2.Rows[0].Cells.Clear();
            GrdNote2.Rows[0].Cells.Add(new TableCell());
            GrdNote2.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdNote2.Rows[0].Cells[0].Text = "No Record Found";
        }


        Tuple<string, string, string, string> refsEmailInfo = GetReferrersEmail();
        ImageButton btnEmail = GrdNote2.HeaderRow.FindControl("btnEmail") as ImageButton;
        if (refsEmailInfo != null)
        {
            btnEmail.Visible = true;
            ((HiddenField)GrdNote2.HeaderRow.FindControl("hiddenRefEmail")).Value = refsEmailInfo.Item1;
            ((HiddenField)GrdNote2.HeaderRow.FindControl("hiddenRefName")).Value = refsEmailInfo.Item2;
            ((HiddenField)GrdNote2.HeaderRow.FindControl("hiddenBookingOrg")).Value = refsEmailInfo.Item3;
            ((HiddenField)GrdNote2.HeaderRow.FindControl("HiddenBookingPatientName")).Value = refsEmailInfo.Item4;
        }
        else
        {
            btnEmail.Visible = false;
        }

        DisallowAddEditIfNoPermissions2(); // place this after databinding
    }
    protected void GrdNote2_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {
            e.Row.Cells[0].CssClass = "hiddencol";
            e.Row.Cells[1].CssClass = "hiddencol";
        }

        if (e.Row.RowType != DataControlRowType.Pager && !UserView.GetInstance().IsAdminView)
            e.Row.Cells[6].CssClass = "hiddencol";

        if (e.Row.RowType != DataControlRowType.Pager && (ScreenID2 == 16 || ScreenID2 == 17 || ScreenID2 == 18))
        {
            foreach (DataControlField col in GrdNote2.Columns)
                if (col.HeaderText.ToLower().Trim() == "type")
                    e.Row.Cells[GrdNote2.Columns.IndexOf(col)].CssClass = "hiddencol";
        }
        if (e.Row.RowType != DataControlRowType.Pager && (ScreenID2 != 16))
        {
            foreach (DataControlField col in GrdNote2.Columns)
                if (col.HeaderText.ToLower().Trim() == "body part")
                    e.Row.Cells[GrdNote2.Columns.IndexOf(col)].CssClass = "hiddencol";
        }

        if (e.Row.RowType == DataControlRowType.DataRow && e.Row.DataItem != null)
        {
            TextBox lblNote = (TextBox)e.Row.FindControl("lblNote");
            if (lblNote != null)
            {
                DataTable dt = ViewState["noteinfo2_data"] as DataTable;
                string text = dt.Rows[e.Row.RowIndex]["text"].ToString();
                string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                lblNote.Rows = lines.Length <= 1 ? 1 : lines.Length - 1;
            }
        }

        if (ScreenID2 == 17 && e.Row.RowType == DataControlRowType.Header)
            e.Row.Cells[5].Text = "Medication";

        if (ScreenID2 == 18 && e.Row.RowType == DataControlRowType.Header)
            e.Row.Cells[5].Text = "Medical Condition";

    }
    protected void GrdNote2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            UserView userView = UserView.GetInstance();

            // if there exists note with the type not in the list of types offered on this screen, use this to add below with "foundRows"
            DataTable allNoteTypes = DBBase.GetGenericDataTable(null, "NoteType", "note_type_id", "descr");
            DataTable allBodyParts = DBBase.GetGenericDataTable(null, "BodyPart", "body_part_id", "descr");

            DataTable noteTypes = ScreenNoteTypesDB.GetDataTable_ByScreenID(ScreenID2);
            DataTable sites = SiteDB.GetDataTable();

            DataTable dt = ViewState["noteinfo2_data"] as DataTable;
            bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
            if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblId = (Label)e.Row.FindControl("lblId");
                DataRow[] foundRows = dt.Select("note_id=" + lblId.Text);
                DataRow thisRow = foundRows[0];


                bool isDeleted = thisRow["deleted_by"] != DBNull.Value || thisRow["date_deleted"] != DBNull.Value;


                DropDownList ddlDate_Day = (DropDownList)e.Row.FindControl("ddlDate_Day");
                DropDownList ddlDate_Month = (DropDownList)e.Row.FindControl("ddlDate_Month");
                DropDownList ddlDate_Year = (DropDownList)e.Row.FindControl("ddlDate_Year");
                if (ddlDate_Day != null && ddlDate_Month != null && ddlDate_Year != null)
                {
                    ddlDate_Day.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Month.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Year.Items.Add(new ListItem("----", "-1"));

                    for (int i = 1; i <= 31; i++)
                        ddlDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = 1; i <= 12; i++)
                        ddlDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = DateTime.Today.Year - 1; i <= DateTime.Today.Year + 1; i++)
                        ddlDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                    if (thisRow["date_added"] != DBNull.Value)
                    {
                        DateTime Date = Convert.ToDateTime(thisRow["date_added"]);

                        ddlDate_Day.SelectedValue = Date.Day.ToString();
                        ddlDate_Month.SelectedValue = Date.Month.ToString();

                        int firstYearSelectable = Convert.ToInt32(ddlDate_Year.Items[1].Value);
                        int lastYearSelectable = Convert.ToInt32(ddlDate_Year.Items[ddlDate_Year.Items.Count - 1].Value);
                        if (Date.Year < firstYearSelectable)
                            ddlDate_Year.Items.Insert(1, new ListItem(Date.Year.ToString(), Date.Year.ToString()));
                        if (Date.Year > lastYearSelectable)
                            ddlDate_Year.Items.Add(new ListItem(Date.Year.ToString(), Date.Year.ToString()));

                        ddlDate_Year.SelectedValue = Date.Year.ToString();
                    }
                }


                DropDownList ddlNoteType = (DropDownList)e.Row.FindControl("ddlNoteType");
                if (ddlNoteType != null)
                {
                    ddlNoteType.DataSource     = noteTypes;
                    ddlNoteType.DataTextField  = "descr";
                    ddlNoteType.DataValueField = "note_type_id";
                    ddlNoteType.DataBind();

                    // if this note type is not in the list for this screen, add it to the edit list
                    bool found = false;
                    foreach (ListItem li in ddlNoteType.Items)
                        if (li.Value == thisRow["note_type_id"].ToString())
                            found = true;
                    if (!found)
                        ddlNoteType.Items.Add(new ListItem(thisRow["note_type_descr"].ToString(), thisRow["note_type_id"].ToString()));

                    ddlNoteType.SelectedValue = thisRow["note_type_id"].ToString();
                }

                DropDownList ddlBodyPart = (DropDownList)e.Row.FindControl("ddlBodyPart");
                if (ddlBodyPart != null)
                {

                    ddlBodyPart.Items.Clear();
                    ddlBodyPart.Items.Add(new ListItem("","-1"));
                    for(int i=0; i<allBodyParts.Rows.Count; i++)
                        ddlBodyPart.Items.Add(new ListItem(allBodyParts.Rows[i]["body_part_id"].ToString() + ". " + allBodyParts.Rows[i]["descr"].ToString(), allBodyParts.Rows[i]["body_part_id"].ToString()));

                    ddlBodyPart.SelectedValue = thisRow["body_part_id"].ToString();
                }


                ImageButton lnkEdit = (ImageButton)e.Row.FindControl("lnkEdit");
                if (lnkEdit != null)
                {
                    lnkEdit.Visible = !isDeleted && 
                    (userView.IsAdminView || 
                     (thisRow["added_by_staff_id"]    != DBNull.Value && Convert.ToInt32(thisRow["added_by_staff_id"])    == Convert.ToInt32(Session["StaffID"])) ||
                     (thisRow["modified_by_staff_id"] != DBNull.Value && Convert.ToInt32(thisRow["modified_by_staff_id"]) == Convert.ToInt32(Session["StaffID"])));
                }


                /*
                DropDownList ddlSite = (DropDownList)e.Row.FindControl("ddlSite");
                if (ddlSite != null)
                {
                    ddlSite.Items.Add(new ListItem("--", "-1"));
                    foreach (DataRow row in sites.Rows)
                        ddlSite.Items.Add(new ListItem(row["name"].ToString(), row["site_id"].ToString()));
                    ddlSite.SelectedValue = thisRow["site_id"].ToString();
                }
                */


                ImageButton lnkDelete = (ImageButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.Visible = userView.IsAdminView ||
                    (thisRow["added_by_staff_id"] != DBNull.Value && Convert.ToInt32(thisRow["added_by_staff_id"]) == Convert.ToInt32(Session["StaffID"])) ||
                    (thisRow["modified_by_staff_id"] != DBNull.Value && Convert.ToInt32(thisRow["modified_by_staff_id"]) == Convert.ToInt32(Session["StaffID"]));

                    if (isDeleted)
                    {
                        lnkDelete.CommandName = "_UnDelete";
                        lnkDelete.ImageUrl = "~/images/tick-24.png";
                        lnkDelete.ToolTip = "Un-Delete";
                    }
                }

                if (isDeleted)
                {
                    e.Row.AddCssClass("deleted_note");
                    e.Row.Style["display"] = "none";
                    e.Row.Style["color"] = "gray";
                }


                Utilities.AddConfirmationBox(e);
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                    Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {

                DropDownList ddlDate_Day = (DropDownList)e.Row.FindControl("ddlNewDate_Day");
                DropDownList ddlDate_Month = (DropDownList)e.Row.FindControl("ddlNewDate_Month");
                DropDownList ddlDate_Year = (DropDownList)e.Row.FindControl("ddlNewDate_Year");
                if (ddlDate_Day != null && ddlDate_Month != null && ddlDate_Year != null)
                {
                    ddlDate_Day.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Month.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Year.Items.Add(new ListItem("----", "-1"));

                    for (int i = 1; i <= 31; i++)
                        ddlDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = 1; i <= 12; i++)
                        ddlDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = DateTime.Today.Year - 1; i <= DateTime.Today.Year + 1; i++)
                        ddlDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                    ddlDate_Day.SelectedValue = DateTime.Today.Day.ToString();
                    ddlDate_Month.SelectedValue = DateTime.Today.Month.ToString();
                    ddlDate_Year.SelectedValue = DateTime.Today.Year.ToString();

                }

                DropDownList ddlNoteType = (DropDownList)e.Row.FindControl("ddlNewNoteType");
                ddlNoteType.DataSource = noteTypes;
                ddlNoteType.DataBind();

                DropDownList ddlBodyPart = (DropDownList)e.Row.FindControl("ddlNewBodyPart");
                ddlBodyPart.Items.Add(new ListItem("", "-1"));
                for (int i = 0; i < allBodyParts.Rows.Count; i++)
                    ddlBodyPart.Items.Add(new ListItem(allBodyParts.Rows[i]["body_part_id"].ToString() + ". " + allBodyParts.Rows[i]["descr"].ToString(), allBodyParts.Rows[i]["body_part_id"].ToString()));


                // set note text in cookie in case user logged out, to keep note text for this user and this entity
                TextBox txtNewText = (TextBox)e.Row.FindControl("txtNewText");
                txtNewText.Attributes["onkeyup"] = "set_note(document.getElementById('" + txtNewText.ClientID + "'), document.getElementById('" + userID.ClientID + "').value, document.getElementById('" + entityID.ClientID + "').value);";


                /*
                DropDownList ddlSite = (DropDownList)e.Row.FindControl("ddlNewSite");
                ddlSite.Items.Add(new ListItem("--", "-1"));
                foreach (DataRow row in sites.Rows)
                    ddlSite.Items.Add(new ListItem(row["name"].ToString(), row["site_id"].ToString()));
                ddlSite.SelectedValue = Session["SiteID"].ToString();
                */
            }
        }
        catch (Exception ex)
        {
            if (Utilities.IsDev())
                throw;
            else
                HideTableAndSetErrorMessage(ex is CustomMessageException ? ex.Message : "");
        }
    }
    protected void GrdNote2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdNote2.Columns[9].Visible = true;
        GrdNote2.EditIndex = -1;
        RefillAllGrids();
    }
    protected void GrdNote2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdNote2.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlNoteType = (DropDownList)GrdNote2.Rows[e.RowIndex].FindControl("ddlNoteType");
        DropDownList ddlBodyPart = (DropDownList)GrdNote2.Rows[e.RowIndex].FindControl("ddlBodyPart");
        TextBox txtText = (TextBox)GrdNote2.Rows[e.RowIndex].FindControl("txtText");
        //DropDownList ddlSite = (DropDownList)GrdNote2.Rows[e.RowIndex].FindControl("ddlSite");
        DropDownList ddlDate_Day = (DropDownList)GrdNote2.Rows[e.RowIndex].FindControl("ddlDate_Day");
        DropDownList ddlDate_Month = (DropDownList)GrdNote2.Rows[e.RowIndex].FindControl("ddlDate_Month");
        DropDownList ddlDate_Year = (DropDownList)GrdNote2.Rows[e.RowIndex].FindControl("ddlDate_Year");

        DataTable dt = ViewState["noteinfo2_data"] as DataTable;
        DataRow[] foundRows = dt.Select("note_id=" + lblId.Text);
        Note note = NoteDB.Load(foundRows[0]);

        DateTime date = GetDate(ddlDate_Day.SelectedValue, ddlDate_Month.SelectedValue, ddlDate_Year.SelectedValue);
        NoteDB.Update(Convert.ToInt32(lblId.Text), date, Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(ddlNoteType.SelectedValue), Convert.ToInt32(ddlBodyPart.SelectedValue), txtText.Text, note.Site.SiteID);



        // if its a booking note
        // email admin so they know if a provider is sabotaging the system (it has happened before)

        int loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)  // if note is for a booking
        {

            int thresholdCharacters = 50;
            int totalCharactersBefore = note.Text.Trim().Length;
            int totalCharactersAfter = txtText.Text.Trim().Length;
            int difference = totalCharactersAfter - totalCharactersBefore;

            if (totalCharactersBefore > thresholdCharacters && totalCharactersAfter < thresholdCharacters && difference < -20)
            {
                string mailText = @"This is an administrative email to notify you that notes for a booking may have been deleted.

<u>Logged-in user performing the udate</u>
" + StaffDB.GetByID(loggedInStaffID).Person.FullnameWithoutMiddlename + @"

<u>Original Text (Characters: " + totalCharactersBefore + @")</u>
<font color=""blue"">" + note.Text.Replace(Environment.NewLine, "<br />") + @"</font>

<u>Updated Text (Characters: " + totalCharactersAfter + @")</u>
<font color=""blue"">" + txtText.Text.Replace(Environment.NewLine, "<br />") + @"</font>

<u>Booking details</u>
<table border=""0"" cellpadding=""2"" cellspacing=""2""><tr><td>Booking ID:</td><td>" + booking.BookingID + @"</td></tr><tr><td>Booking Date:</td><td>" + booking.DateStart.ToString("d MMM, yyyy") + " " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + @"</td></tr><tr><td>Organisation:</td><td>" + booking.Organisation.Name + @"</td></tr><tr><td>Provider:</td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr><tr><td>Patient:</td><td>" + (booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename + " [ID:" + booking.Patient.PatientID + "]") + @"</td></tr><tr><td>Status:</td><td>" + booking.BookingStatus.Descr + @"</td></tr></table>

Regards,
Mediclinic
";
                bool EnableDeletedBookingsAlerts = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableDeletedBookingsAlerts").Value) == 1;

                if (EnableDeletedBookingsAlerts && !Utilities.IsDev())
                    Emailer.AsyncSimpleEmail(
                        ((SystemVariables)Session["SystemVariables"])["Email_FromEmail"].Value,
                        ((SystemVariables)Session["SystemVariables"])["Email_FromName"].Value,
                        ((SystemVariables)Session["SystemVariables"])["AdminAlertEmail_To"].Value,
                        "Notification that booking notes may have been deleted",
                        mailText.Replace(Environment.NewLine, "<br />"),
                        true,
                        null);
            }
        }



        GrdNote2.Columns[7].Visible = true;
        GrdNote2.EditIndex = -1;
        RefillAllGrids();
    }
    protected void GrdNote2_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdNote2.Rows[e.RowIndex].FindControl("lblId");
        int note_id = Convert.ToInt32(lblId.Text);

        try
        {
            //NoteDB.UpdateInactive(note_id);
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        RefillAllGrids();
    }
    protected void GrdNote2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            Label lblId = (Label)GrdNote2.FooterRow.FindControl("lblId");
            DropDownList ddlNoteType = (DropDownList)GrdNote2.FooterRow.FindControl("ddlNewNoteType");
            DropDownList ddlBodyPart = (DropDownList)GrdNote2.FooterRow.FindControl("ddlNewBodyPart");
            TextBox txtText = (TextBox)GrdNote2.FooterRow.FindControl("txtNewText");
            //DropDownList ddlSite = (DropDownList)GrdNote2.FooterRow.FindControl("ddlNewSite");
            DropDownList ddlDate_Day = (DropDownList)GrdNote2.FooterRow.FindControl("ddlNewDate_Day");
            DropDownList ddlDate_Month = (DropDownList)GrdNote2.FooterRow.FindControl("ddlNewDate_Month");
            DropDownList ddlDate_Year = (DropDownList)GrdNote2.FooterRow.FindControl("ddlNewDate_Year");

            if (!IsValidDate(ddlDate_Day.SelectedValue, ddlDate_Month.SelectedValue, ddlDate_Year.SelectedValue))
                return;

            DateTime date = GetDate(ddlDate_Day.SelectedValue, ddlDate_Month.SelectedValue, ddlDate_Year.SelectedValue);
            NoteDB.Insert(GetFormBKID(), date, Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(ddlNoteType.SelectedValue), Convert.ToInt32(ddlBodyPart.SelectedValue), txtText.Text, Convert.ToInt32(Session["SiteID"]));

            RefillAllGrids();

            string clear_saved_note = "clear_note(document.getElementById('" + ((TextBox)GrdNote2.FooterRow.FindControl("txtNewText")).ClientID + "'), document.getElementById('" + userID.ClientID + "').value, document.getElementById('" + entityID.ClientID + "').value);";
            ScriptManager.RegisterStartupScript(GrdNote2, this.GetType(), "unset_cookie", clear_saved_note, true);
        }
        if (e.CommandName == "_Delete")
        {
            NoteDB.SetDeleted(Convert.ToInt32(e.CommandArgument), Convert.ToInt32(Session["StaffID"]));
            RefillAllGrids();
        }
        if (e.CommandName == "_UnDelete")
        {
            NoteDB.SetNotDeleted(Convert.ToInt32(e.CommandArgument), Convert.ToInt32(Session["StaffID"]));
            RefillAllGrids();
        }
    }
    protected void GrdNote2_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdNote2.Columns[9].Visible = false;
        GrdNote2.EditIndex = e.NewEditIndex;
        RefillAllGrids();
    }
    protected void GrdNote2_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdNote2.EditIndex >= 0)
            return;

        DataTable dataTable = ViewState["noteinfo2_data"] as DataTable;

        if (dataTable != null)
        {
            if (ViewState["noteinfo2_sortexpression"] == null)
                ViewState["noteinfo2_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = ViewState["noteinfo2_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            ViewState["noteinfo2_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdNote2.DataSource = dataView;
            GrdNote2.DataBind();
        }
    }

    #endregion

    #region GrdNote3

    protected void DisallowAddEditIfNoPermissions3()
    {
        // if its a booking note
        // only allow add/edit if by the provider of the booking, or by a "principle" staff memeber


        UserView userView = UserView.GetInstance();
        int loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
        {
            bool canAddEdit = (booking.Provider != null && loggedInStaffID == booking.Provider.StaffID) || userView.IsPrincipal || userView.IsMasterAdmin || userView.IsStakeholder;
            if (!canAddEdit)
            {
                GrdNote3.FooterRow.Visible = false;
                for (int i = 0; i < GrdNote3.Columns.Count; i++)
                    if (GrdNote3.Columns[i].HeaderText.Trim() == ".")
                        GrdNote3.Columns[i].Visible = false;
            }
        }
    }

    protected void GrdNote3_FillNoteGrid()
    {
        if (!IsValidFormBKID())
        {
            if (!Utilities.IsDev() || Request.QueryString["id"] != null)
            {
                HideTableAndSetErrorMessage();
                return;
            }

            // can still view all if dev and no id set .. but no insert/edit
            GrdNote3.Columns[5].Visible = false;
        }


        DataTable dt = IsValidFormBKID() ? NoteDB.GetDataTable_ByEntityID(GetFormBKID(), null, true, true) : NoteDB.GetDataTable(true);

        Hashtable allowedNoteTypes = new Hashtable();
        DataTable noteTypes = ScreenNoteTypesDB.GetDataTable_ByScreenID(ScreenID3);
        for(int i=0; i<noteTypes.Rows.Count; i++)
            allowedNoteTypes[Convert.ToInt32(noteTypes.Rows[i]["note_type_id"])] = 1;

        for (int i = dt.Rows.Count - 1; i >= 0; i--)
            if (allowedNoteTypes[Convert.ToInt32(dt.Rows[i]["note_type_id"])] == null)
                dt.Rows.RemoveAt(i);


        UserView userView = UserView.GetInstance();

        bool canSeeModifiedBy = userView.IsStakeholder || userView.IsMasterAdmin;
        dt.Columns.Add("last_modified_note_info_visible", typeof(Boolean));
        for (int i = 0; i < dt.Rows.Count; i++)
            dt.Rows[i]["last_modified_note_info_visible"] = canSeeModifiedBy;


        ViewState["noteinfo3_data"] = dt;



        // add note info to hidden field to use when emailing notes

        string emailBodyText = string.Empty;

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
        {
            emailBodyText += @"<br /><br />
<u>Treatment Information</u>
<br />
<table border=""0"" cellpadding=""0"" cellspacing=""0"">" +
    (booking.Patient == null ? "" : @"<tr><td>Patient</td><td style=""width:10px;""></td><td>" + booking.Patient.Person.FullnameWithoutMiddlename + @"</td></tr>") +
    (booking.Offering == null ? "" : @"<tr><td>Service</td><td></td><td>" + booking.Offering.Name + @"</td></tr>") + @"
    <tr><td>Date</td><td></td><td>" + booking.DateStart.ToString("dd-MM-yyyy") + @"</td></tr>
    <tr><td>Provider</td><td></td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr>
</table>";
        }

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            Note n = NoteDB.Load(dt.Rows[i]);
            emailBodyText += "<br /><br /><u>Note (" + n.DateAdded.ToString("dd-MM-yyyy") + ")</u><br />" + n.Text.Replace(Environment.NewLine, "<br />");
        }
        emailText.Value = emailBodyText + "<br /><br />" + SystemVariableDB.GetByDescr("LettersEmailSignature").Value; ;



        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && ViewState["noteinfo3_sortexpression"] != null && ViewState["noteinfo3_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = ViewState["noteinfo3_sortexpression"].ToString();
                GrdNote3.DataSource = dataView;
            }
            else
            {
                GrdNote3.DataSource = dt;
            }


            try
            {
                GrdNote3.DataBind();
            }
            catch (Exception ex)
            {
                this.lblErrorMessage.Visible = true;
                this.lblErrorMessage.Text = ex.ToString();
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdNote3.DataSource = dt;
            GrdNote3.DataBind();

            int TotalColumns = GrdNote3.Rows[0].Cells.Count;
            GrdNote3.Rows[0].Cells.Clear();
            GrdNote3.Rows[0].Cells.Add(new TableCell());
            GrdNote3.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdNote3.Rows[0].Cells[0].Text = "No Record Found";
        }


        Tuple<string, string, string, string> refsEmailInfo = GetReferrersEmail();
        ImageButton btnEmail = GrdNote3.HeaderRow.FindControl("btnEmail") as ImageButton;
        if (refsEmailInfo != null)
        {
            btnEmail.Visible = true;
            ((HiddenField)GrdNote3.HeaderRow.FindControl("hiddenRefEmail")).Value = refsEmailInfo.Item1;
            ((HiddenField)GrdNote3.HeaderRow.FindControl("hiddenRefName")).Value = refsEmailInfo.Item2;
            ((HiddenField)GrdNote3.HeaderRow.FindControl("hiddenBookingOrg")).Value = refsEmailInfo.Item3;
            ((HiddenField)GrdNote3.HeaderRow.FindControl("HiddenBookingPatientName")).Value = refsEmailInfo.Item4;
        }
        else
        {
            btnEmail.Visible = false;
        }

        DisallowAddEditIfNoPermissions3(); // place this after databinding
    }
    protected void GrdNote3_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {
            e.Row.Cells[0].CssClass = "hiddencol";
            e.Row.Cells[1].CssClass = "hiddencol";
        }

        if (e.Row.RowType != DataControlRowType.Pager && !UserView.GetInstance().IsAdminView)
            e.Row.Cells[6].CssClass = "hiddencol";

        if (e.Row.RowType != DataControlRowType.Pager && (ScreenID3 == 16 || ScreenID3 == 17 || ScreenID3 == 18))
        {
            foreach (DataControlField col in GrdNote3.Columns)
                if (col.HeaderText.ToLower().Trim() == "type")
                    e.Row.Cells[GrdNote3.Columns.IndexOf(col)].CssClass = "hiddencol";
        }
        if (e.Row.RowType != DataControlRowType.Pager && (ScreenID3 != 16))
        {
            foreach (DataControlField col in GrdNote3.Columns)
                if (col.HeaderText.ToLower().Trim() == "body part")
                    e.Row.Cells[GrdNote3.Columns.IndexOf(col)].CssClass = "hiddencol";
        }

        if (e.Row.RowType == DataControlRowType.DataRow && e.Row.DataItem != null)
        {
            TextBox lblNote = (TextBox)e.Row.FindControl("lblNote");
            if (lblNote != null)
            {
                DataTable dt = ViewState["noteinfo3_data"] as DataTable;
                string text = dt.Rows[e.Row.RowIndex]["text"].ToString();
                string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                lblNote.Rows = lines.Length <= 1 ? 1 : lines.Length - 1;
            }
        }

        if (ScreenID3 == 17 && e.Row.RowType == DataControlRowType.Header)
            e.Row.Cells[5].Text = "Medication";

        if (ScreenID3 == 18 && e.Row.RowType == DataControlRowType.Header)
            e.Row.Cells[5].Text = "Medical Condition";

    }
    protected void GrdNote3_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            UserView userView = UserView.GetInstance();

            // if there exists note with the type not in the list of types offered on this screen, use this to add below with "foundRows"
            DataTable allNoteTypes = DBBase.GetGenericDataTable(null, "NoteType", "note_type_id", "descr");
            DataTable allBodyParts = DBBase.GetGenericDataTable(null, "BodyPart", "body_part_id", "descr");

            DataTable noteTypes = ScreenNoteTypesDB.GetDataTable_ByScreenID(ScreenID3);
            DataTable sites = SiteDB.GetDataTable();

            DataTable dt = ViewState["noteinfo3_data"] as DataTable;
            bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
            if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblId = (Label)e.Row.FindControl("lblId");
                DataRow[] foundRows = dt.Select("note_id=" + lblId.Text);
                DataRow thisRow = foundRows[0];


                bool isDeleted = thisRow["deleted_by"] != DBNull.Value || thisRow["date_deleted"] != DBNull.Value;


                DropDownList ddlDate_Day = (DropDownList)e.Row.FindControl("ddlDate_Day");
                DropDownList ddlDate_Month = (DropDownList)e.Row.FindControl("ddlDate_Month");
                DropDownList ddlDate_Year = (DropDownList)e.Row.FindControl("ddlDate_Year");
                if (ddlDate_Day != null && ddlDate_Month != null && ddlDate_Year != null)
                {
                    ddlDate_Day.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Month.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Year.Items.Add(new ListItem("----", "-1"));

                    for (int i = 1; i <= 31; i++)
                        ddlDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = 1; i <= 12; i++)
                        ddlDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = DateTime.Today.Year - 1; i <= DateTime.Today.Year + 1; i++)
                        ddlDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                    if (thisRow["date_added"] != DBNull.Value)
                    {
                        DateTime Date = Convert.ToDateTime(thisRow["date_added"]);

                        ddlDate_Day.SelectedValue = Date.Day.ToString();
                        ddlDate_Month.SelectedValue = Date.Month.ToString();

                        int firstYearSelectable = Convert.ToInt32(ddlDate_Year.Items[1].Value);
                        int lastYearSelectable = Convert.ToInt32(ddlDate_Year.Items[ddlDate_Year.Items.Count - 1].Value);
                        if (Date.Year < firstYearSelectable)
                            ddlDate_Year.Items.Insert(1, new ListItem(Date.Year.ToString(), Date.Year.ToString()));
                        if (Date.Year > lastYearSelectable)
                            ddlDate_Year.Items.Add(new ListItem(Date.Year.ToString(), Date.Year.ToString()));

                        ddlDate_Year.SelectedValue = Date.Year.ToString();
                    }
                }


                DropDownList ddlNoteType = (DropDownList)e.Row.FindControl("ddlNoteType");
                if (ddlNoteType != null)
                {
                    ddlNoteType.DataSource     = noteTypes;
                    ddlNoteType.DataTextField  = "descr";
                    ddlNoteType.DataValueField = "note_type_id";
                    ddlNoteType.DataBind();

                    // if this note type is not in the list for this screen, add it to the edit list
                    bool found = false;
                    foreach (ListItem li in ddlNoteType.Items)
                        if (li.Value == thisRow["note_type_id"].ToString())
                            found = true;
                    if (!found)
                        ddlNoteType.Items.Add(new ListItem(thisRow["note_type_descr"].ToString(), thisRow["note_type_id"].ToString()));

                    ddlNoteType.SelectedValue = thisRow["note_type_id"].ToString();
                }

                DropDownList ddlBodyPart = (DropDownList)e.Row.FindControl("ddlBodyPart");
                if (ddlBodyPart != null)
                {

                    ddlBodyPart.Items.Clear();
                    ddlBodyPart.Items.Add(new ListItem("","-1"));
                    for(int i=0; i<allBodyParts.Rows.Count; i++)
                        ddlBodyPart.Items.Add(new ListItem(allBodyParts.Rows[i]["body_part_id"].ToString() + ". " + allBodyParts.Rows[i]["descr"].ToString(), allBodyParts.Rows[i]["body_part_id"].ToString()));

                    ddlBodyPart.SelectedValue = thisRow["body_part_id"].ToString();
                }


                ImageButton lnkEdit = (ImageButton)e.Row.FindControl("lnkEdit");
                if (lnkEdit != null)
                {
                    lnkEdit.Visible = !isDeleted && 
                    (userView.IsAdminView || 
                     (thisRow["added_by_staff_id"]    != DBNull.Value && Convert.ToInt32(thisRow["added_by_staff_id"])    == Convert.ToInt32(Session["StaffID"])) ||
                     (thisRow["modified_by_staff_id"] != DBNull.Value && Convert.ToInt32(thisRow["modified_by_staff_id"]) == Convert.ToInt32(Session["StaffID"])));
                }


                /*
                DropDownList ddlSite = (DropDownList)e.Row.FindControl("ddlSite");
                if (ddlSite != null)
                {
                    ddlSite.Items.Add(new ListItem("--", "-1"));
                    foreach (DataRow row in sites.Rows)
                        ddlSite.Items.Add(new ListItem(row["name"].ToString(), row["site_id"].ToString()));
                    ddlSite.SelectedValue = thisRow["site_id"].ToString();
                }
                */


                ImageButton lnkDelete = (ImageButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.Visible = userView.IsAdminView ||
                    (thisRow["added_by_staff_id"] != DBNull.Value && Convert.ToInt32(thisRow["added_by_staff_id"]) == Convert.ToInt32(Session["StaffID"])) ||
                    (thisRow["modified_by_staff_id"] != DBNull.Value && Convert.ToInt32(thisRow["modified_by_staff_id"]) == Convert.ToInt32(Session["StaffID"]));

                    if (isDeleted)
                    {
                        lnkDelete.CommandName = "_UnDelete";
                        lnkDelete.ImageUrl = "~/images/tick-24.png";
                        lnkDelete.ToolTip = "Un-Delete";
                    }
                }

                if (isDeleted)
                {
                    e.Row.AddCssClass("deleted_note");
                    e.Row.Style["display"] = "none";
                    e.Row.Style["color"] = "gray";
                }


                Utilities.AddConfirmationBox(e);
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                    Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {

                DropDownList ddlDate_Day = (DropDownList)e.Row.FindControl("ddlNewDate_Day");
                DropDownList ddlDate_Month = (DropDownList)e.Row.FindControl("ddlNewDate_Month");
                DropDownList ddlDate_Year = (DropDownList)e.Row.FindControl("ddlNewDate_Year");
                if (ddlDate_Day != null && ddlDate_Month != null && ddlDate_Year != null)
                {
                    ddlDate_Day.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Month.Items.Add(new ListItem("--", "-1"));
                    ddlDate_Year.Items.Add(new ListItem("----", "-1"));

                    for (int i = 1; i <= 31; i++)
                        ddlDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = 1; i <= 12; i++)
                        ddlDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    for (int i = DateTime.Today.Year - 1; i <= DateTime.Today.Year + 1; i++)
                        ddlDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));

                    ddlDate_Day.SelectedValue = DateTime.Today.Day.ToString();
                    ddlDate_Month.SelectedValue = DateTime.Today.Month.ToString();
                    ddlDate_Year.SelectedValue = DateTime.Today.Year.ToString();

                }

                DropDownList ddlNoteType = (DropDownList)e.Row.FindControl("ddlNewNoteType");
                ddlNoteType.DataSource = noteTypes;
                ddlNoteType.DataBind();

                DropDownList ddlBodyPart = (DropDownList)e.Row.FindControl("ddlNewBodyPart");
                ddlBodyPart.Items.Add(new ListItem("", "-1"));
                for (int i = 0; i < allBodyParts.Rows.Count; i++)
                    ddlBodyPart.Items.Add(new ListItem(allBodyParts.Rows[i]["body_part_id"].ToString() + ". " + allBodyParts.Rows[i]["descr"].ToString(), allBodyParts.Rows[i]["body_part_id"].ToString()));


                // set note text in cookie in case user logged out, to keep note text for this user and this entity
                TextBox txtNewText = (TextBox)e.Row.FindControl("txtNewText");
                txtNewText.Attributes["onkeyup"] = "set_note(document.getElementById('" + txtNewText.ClientID + "'), document.getElementById('" + userID.ClientID + "').value, document.getElementById('" + entityID.ClientID + "').value);";


                /*
                DropDownList ddlSite = (DropDownList)e.Row.FindControl("ddlNewSite");
                ddlSite.Items.Add(new ListItem("--", "-1"));
                foreach (DataRow row in sites.Rows)
                    ddlSite.Items.Add(new ListItem(row["name"].ToString(), row["site_id"].ToString()));
                ddlSite.SelectedValue = Session["SiteID"].ToString();
                */
            }
        }
        catch (Exception ex)
        {
            if (Utilities.IsDev())
                throw;
            else
                HideTableAndSetErrorMessage(ex is CustomMessageException ? ex.Message : "");
        }
    }
    protected void GrdNote3_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdNote3.Columns[9].Visible = true;
        GrdNote3.EditIndex = -1;
        RefillAllGrids();
    }
    protected void GrdNote3_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdNote3.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlNoteType = (DropDownList)GrdNote3.Rows[e.RowIndex].FindControl("ddlNoteType");
        DropDownList ddlBodyPart = (DropDownList)GrdNote3.Rows[e.RowIndex].FindControl("ddlBodyPart");
        TextBox txtText = (TextBox)GrdNote3.Rows[e.RowIndex].FindControl("txtText");
        //DropDownList ddlSite = (DropDownList)GrdNote3.Rows[e.RowIndex].FindControl("ddlSite");
        DropDownList ddlDate_Day = (DropDownList)GrdNote3.Rows[e.RowIndex].FindControl("ddlDate_Day");
        DropDownList ddlDate_Month = (DropDownList)GrdNote3.Rows[e.RowIndex].FindControl("ddlDate_Month");
        DropDownList ddlDate_Year = (DropDownList)GrdNote3.Rows[e.RowIndex].FindControl("ddlDate_Year");

        DataTable dt = ViewState["noteinfo3_data"] as DataTable;
        DataRow[] foundRows = dt.Select("note_id=" + lblId.Text);
        if (foundRows.Length == 0)
        {
            // alert as to why
            string message = @"note_id=" + lblId.Text + @"";
            Emailer.SimpleAlertEmail(
                message,
                "Note Page : foundRows.Length == 0",
                true);
        }
        Note note = NoteDB.Load(foundRows[0]);

        DateTime date = GetDate(ddlDate_Day.SelectedValue, ddlDate_Month.SelectedValue, ddlDate_Year.SelectedValue);
        NoteDB.Update(Convert.ToInt32(lblId.Text), date, Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(ddlNoteType.SelectedValue), Convert.ToInt32(ddlBodyPart.SelectedValue), txtText.Text, note.Site.SiteID);



        // if its a booking note
        // email admin so they know if a provider is sabotaging the system (it has happened before)

        int loggedInStaffID = Session["StaffID"] == null ? -1 : Convert.ToInt32(Session["StaffID"]);

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)  // if note is for a booking
        {

            int thresholdCharacters = 50;
            int totalCharactersBefore = note.Text.Trim().Length;
            int totalCharactersAfter = txtText.Text.Trim().Length;
            int difference = totalCharactersAfter - totalCharactersBefore;

            if (totalCharactersBefore > thresholdCharacters && totalCharactersAfter < thresholdCharacters && difference < -20)
            {
                string mailText = @"This is an administrative email to notify you that notes for a booking may have been deleted.

<u>Logged-in user performing the udate</u>
" + StaffDB.GetByID(loggedInStaffID).Person.FullnameWithoutMiddlename + @"

<u>Original Text (Characters: " + totalCharactersBefore + @")</u>
<font color=""blue"">" + note.Text.Replace(Environment.NewLine, "<br />") + @"</font>

<u>Updated Text (Characters: " + totalCharactersAfter + @")</u>
<font color=""blue"">" + txtText.Text.Replace(Environment.NewLine, "<br />") + @"</font>

<u>Booking details</u>
<table border=""0"" cellpadding=""2"" cellspacing=""2""><tr><td>Booking ID:</td><td>" + booking.BookingID + @"</td></tr><tr><td>Booking Date:</td><td>" + booking.DateStart.ToString("d MMM, yyyy") + " " + booking.DateStart.ToString("h:mm") + (booking.DateStart.Hour < 12 ? "am" : "pm") + @"</td></tr><tr><td>Organisation:</td><td>" + booking.Organisation.Name + @"</td></tr><tr><td>Provider:</td><td>" + booking.Provider.Person.FullnameWithoutMiddlename + @"</td></tr><tr><td>Patient:</td><td>" + (booking.Patient == null ? "" : booking.Patient.Person.FullnameWithoutMiddlename + " [ID:" + booking.Patient.PatientID + "]") + @"</td></tr><tr><td>Status:</td><td>" + booking.BookingStatus.Descr + @"</td></tr></table>

Regards,
Mediclinic
";
                bool EnableDeletedBookingsAlerts = Convert.ToInt32(SystemVariableDB.GetByDescr("EnableDeletedBookingsAlerts").Value) == 1;

                if (EnableDeletedBookingsAlerts && !Utilities.IsDev())
                    Emailer.AsyncSimpleEmail(
                        ((SystemVariables)Session["SystemVariables"])["Email_FromEmail"].Value,
                        ((SystemVariables)Session["SystemVariables"])["Email_FromName"].Value,
                        ((SystemVariables)Session["SystemVariables"])["AdminAlertEmail_To"].Value,
                        "Notification that booking notes may have been deleted",
                        mailText.Replace(Environment.NewLine, "<br />"),
                        true,
                        null);
            }
        }



        GrdNote3.Columns[7].Visible = true;
        GrdNote3.EditIndex = -1;
        RefillAllGrids();
    }
    protected void GrdNote3_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdNote3.Rows[e.RowIndex].FindControl("lblId");
        int note_id = Convert.ToInt32(lblId.Text);

        try
        {
            //NoteDB.UpdateInactive(note_id);
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        RefillAllGrids();
    }
    protected void GrdNote3_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            if (!IsValidFormBKID())
                throw new CustomMessageException();

            Label lblId = (Label)GrdNote3.FooterRow.FindControl("lblId");
            DropDownList ddlNoteType = (DropDownList)GrdNote3.FooterRow.FindControl("ddlNewNoteType");
            DropDownList ddlBodyPart = (DropDownList)GrdNote3.FooterRow.FindControl("ddlNewBodyPart");
            TextBox txtText = (TextBox)GrdNote3.FooterRow.FindControl("txtNewText");
            //DropDownList ddlSite = (DropDownList)GrdNote3.FooterRow.FindControl("ddlNewSite");
            DropDownList ddlDate_Day = (DropDownList)GrdNote3.FooterRow.FindControl("ddlNewDate_Day");
            DropDownList ddlDate_Month = (DropDownList)GrdNote3.FooterRow.FindControl("ddlNewDate_Month");
            DropDownList ddlDate_Year = (DropDownList)GrdNote3.FooterRow.FindControl("ddlNewDate_Year");

            if (!IsValidDate(ddlDate_Day.SelectedValue, ddlDate_Month.SelectedValue, ddlDate_Year.SelectedValue))
                return;

            DateTime date = GetDate(ddlDate_Day.SelectedValue, ddlDate_Month.SelectedValue, ddlDate_Year.SelectedValue);
            NoteDB.Insert(GetFormBKID(), date, Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(ddlNoteType.SelectedValue), Convert.ToInt32(ddlBodyPart.SelectedValue), txtText.Text, Convert.ToInt32(Session["SiteID"]));

            RefillAllGrids();

            string clear_saved_note = "clear_note(document.getElementById('" + ((TextBox)GrdNote3.FooterRow.FindControl("txtNewText")).ClientID + "'), document.getElementById('" + userID.ClientID + "').value, document.getElementById('" + entityID.ClientID + "').value);";
            ScriptManager.RegisterStartupScript(GrdNote3, this.GetType(), "unset_cookie", clear_saved_note, true);
        }
        if (e.CommandName == "_Delete")
        {
            NoteDB.SetDeleted(Convert.ToInt32(e.CommandArgument), Convert.ToInt32(Session["StaffID"]));
            RefillAllGrids();
        }
        if (e.CommandName == "_UnDelete")
        {
            NoteDB.SetNotDeleted(Convert.ToInt32(e.CommandArgument), Convert.ToInt32(Session["StaffID"]));
            RefillAllGrids();
        }
    }
    protected void GrdNote3_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdNote3.Columns[9].Visible = false;
        GrdNote3.EditIndex = e.NewEditIndex;
        RefillAllGrids();
    }
    protected void GrdNote3_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdNote3.EditIndex >= 0)
            return;

        DataTable dataTable = ViewState["noteinfo3_data"] as DataTable;

        if (dataTable != null)
        {
            if (ViewState["noteinfo3_sortexpression"] == null)
                ViewState["noteinfo3_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = ViewState["noteinfo3_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            ViewState["noteinfo3_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdNote3.DataSource = dataView;
            GrdNote3.DataBind();
        }
    }

    #endregion



    #region GetReferrersEmail()

    protected Tuple<string, string, string, string> GetReferrersEmail()
    {
        if (!IsValidFormBKID())
            return null;

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking == null || booking.Patient == null)
            return null;

        PatientReferrer[] patientReferrer = PatientReferrerDB.GetActiveEPCPatientReferrersOf(booking.Patient.PatientID);
        if (patientReferrer.Length == 0)
            return null;

        PatientReferrer currentPatRegReferrer = patientReferrer[patientReferrer.Length - 1];
        RegisterReferrer curRegReferrer = currentPatRegReferrer.RegisterReferrer;

        //string refName = curRegReferrer.Referrer.Person.Surname + ", " + curRegReferrer.Referrer.Person.Firstname + " [" + curRegReferrer.Organisation.Name + "]" + " [" + currentPatRegReferrer.PatientReferrerDateAdded.ToString("dd-MM-yyyy") + "]";
        //SetErrorMessage("Name: " + refName);

        string[] emails = ContactDB.GetEmailsByEntityID(currentPatRegReferrer.RegisterReferrer.Organisation.EntityID);
        if (emails.Length == 0)
            return null;

        string refEmail = string.Join(",", emails);
        string refName = (curRegReferrer.Referrer.Person.Title.ID == 0 ? "Dr." : curRegReferrer.Referrer.Person.Title.Descr) + " " + curRegReferrer.Referrer.Person.Surname;
        string bookingOrg = booking.Organisation.Name;
        string bookingPatientName = booking.Patient.Person.FullnameWithoutMiddlename;
        return new Tuple<string, string, string, string>(refEmail, refName, bookingOrg, bookingPatientName);
    }

    #endregion

    #region btnPrint_Click, btnEmail_Click

    protected void btnPrint_Click1(object sender, ImageClickEventArgs e)
    {
        btnPrint_Click(GrdNote1);
    }
    protected void btnEmail_Click1(object sender, ImageClickEventArgs e)
    {
        btnEmail_Click(GrdNote1);
    }

    protected void btnPrint_Click2(object sender, ImageClickEventArgs e)
    {
        btnPrint_Click(GrdNote2);
    }
    protected void btnEmail_Click2(object sender, ImageClickEventArgs e)
    {
        btnEmail_Click(GrdNote2);
    }

    protected void btnPrint_Click3(object sender, ImageClickEventArgs e)
    {
        btnPrint_Click(GrdNote3);
    }
    protected void btnEmail_Click3(object sender, ImageClickEventArgs e)
    {
        btnEmail_Click(GrdNote3);
    }



    protected void btnPrint_Click(GridView grdView)
    {
        try
        {
            string outputFileName = "Notes.pdf";
            byte[] fileContents = GetNoteFileContents(grdView, outputFileName);
            Letter.DownloadDocument(Response, fileContents, outputFileName);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage("Error: " + cmEx.Message);
        }
    }
    protected void btnEmail_Click(GridView grdView)
    {
        string refsEmail   = ((HiddenField)grdView.HeaderRow.FindControl("hiddenRefEmail")).Value;
        string refsName    = ((HiddenField)grdView.HeaderRow.FindControl("hiddenRefName")).Value;
        string bookingOrg  = ((HiddenField)grdView.HeaderRow.FindControl("hiddenBookingOrg")).Value;
        string patientName = ((HiddenField)grdView.HeaderRow.FindControl("HiddenBookingPatientName")).Value;

        string tmpDir = GetNewTmpDir();


        //create delegate and invoke it asynchrnously, control passes past this line while this is done in another thread
        //AsyncSendEmailDelegate myAction = new AsyncSendEmailDelegate(SendEmail);
        //myAction.BeginInvoke(refsEmail, refsName, bookingOrg, patientName, tmpDir, null, null);

        // dont send async as the session info is unavailable for a whole lot 
        bool sent = SendEmail(grdView, refsEmail, refsName, bookingOrg, patientName, tmpDir);
        if (sent)
            SetErrorMessage("Email sent");
    }

    protected string GetNewTmpDir()
    {
        string tmpLettersDirectory = Letter.GetTempLettersDirectory();
        if (!System.IO.Directory.Exists(tmpLettersDirectory))
            throw new CustomMessageException("Temp letters directory doesn't exist");
        string tmpDir = FileHelper.GetTempDirectoryName(tmpLettersDirectory);
        System.IO.Directory.CreateDirectory(tmpDir);

        return tmpDir;
    }

    protected delegate void AsyncSendEmailDelegate(string refsEmail, string refsName, string bookingOrg, string patientName, string tmpDir);
    protected bool SendEmail(GridView grdView, string refsEmail, string refsName, string bookingOrg, string patientName, string tmpDir)
    {
        try
        {
            string tmpFilename = tmpDir + "Notes.pdf";
            CreateNoteFile(grdView, tmpFilename);
            
            // email the referrer
            Emailer.SimpleEmail(
                bookingOrg,
                refsEmail,
                "Treatment Notes for " + patientName,
                @"Dear " + refsName + ",<br /><br />Please find booking notes attached for " + patientName + "<br /><br />Regards,<br />" + bookingOrg,
                true,
                new string[] { tmpFilename },
                null
                );

            System.IO.File.Delete(tmpFilename);
            System.IO.Directory.Delete(tmpDir);

            //SetErrorMessage("Email sent");

            return true;
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, false);
            //SetErrorMessage("Error: " + cmEx.Message);
            return false;
        }
    }

    protected byte[] GetNoteFileContents(GridView grdView, string outputFileName)
    {
        string tmpDir = GetNewTmpDir();
        string tmpFilename = tmpDir + outputFileName;
        CreateNoteFile(grdView, tmpFilename);

        byte[] fileContents = System.IO.File.ReadAllBytes(tmpFilename);
        System.IO.File.Delete(tmpFilename);
        System.IO.Directory.Delete(tmpDir);

        return fileContents;
    }

    protected void CreateNoteFile(GridView grdView, string tmpFilename)
    {

        string header = string.Empty;

        Booking booking = BookingDB.GetByEntityID(GetFormBKID());
        if (booking != null)
        {
            Site site = SiteDB.GetByID(Convert.ToInt32(Session["SiteID"]));

            string[] phNums;
            if (Utilities.GetAddressType().ToString() == "Contact")
                phNums = ContactDB.GetByEntityID(-1, booking.Organisation.EntityID, 34).Select(r => r.AddrLine1).ToArray();
            else if (Utilities.GetAddressType().ToString() == "ContactAus")
                phNums = ContactAusDB.GetByEntityID(-1, booking.Organisation.EntityID, 34).Select(r => r.AddrLine1).ToArray();
            else
                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

            if (phNums.Length == 0)
            {
                if (Utilities.GetAddressType().ToString() == "Contact")
                    phNums = ContactDB.GetByEntityID(-1, site.EntityID, 34).Select(r => r.AddrLine1).ToArray();
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                    phNums = ContactAusDB.GetByEntityID(-1, site.EntityID, 34).Select(r => r.AddrLine1).ToArray();
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
            }

            string numbers = string.Empty;
            if (phNums.Length > 0)
                numbers += " - TEL " + phNums[0];
            if (phNums.Length > 1)
                numbers += ", " + phNums[1];

            header += "Company: \t" + site.Name + numbers + Environment.NewLine;
            header += "Clinic:    \t" + booking.Organisation.Name + Environment.NewLine;

            if (booking.Patient != null)
                header += "Patient: \t" + booking.Patient.Person.FullnameWithoutMiddlename + Environment.NewLine;
            if (booking.Offering != null)
                header += "Service:   \t" + booking.Offering.Name + Environment.NewLine;

            header += "Provider: \t" + booking.Provider.Person.FullnameWithoutMiddlename + Environment.NewLine;
            header += "Date of Consultation: \t" + booking.DateStart.ToString("d MMM yyyy") + Environment.NewLine + Environment.NewLine + "Treatment Note:" + Environment.NewLine + Environment.NewLine;
        }


        System.Collections.ArrayList notesList = new System.Collections.ArrayList();
        foreach (GridViewRow row in grdView.Rows)
        {
            Label lblId = row.FindControl("lblId") as Label;
            Label lblText = row.FindControl("lblText") as Label;
            CheckBox chkPrint = row.FindControl("chkPrint") as CheckBox;

            if (lblId == null || lblText == null || chkPrint == null)
                continue;

            if (chkPrint.Checked)
                notesList.Add(header + lblText.Text.Replace("<br/>", "\n"));
        }

        if (notesList.Count == 0)
            throw new CustomMessageException("Please select at least one note to print.");

        UserView userView = UserView.GetInstance();
        bool isAgedCare = booking != null && booking.Organisation != null ? booking.Organisation.IsAgedCare : userView.IsAgedCareView;
        string filename = isAgedCare ? "BlankTemplateAC.docx" : "BlankTemplate.docx";
        string originalFile = Letter.GetLettersDirectory() +filename ;
        if (!System.IO.File.Exists(originalFile))
            throw new CustomMessageException("Template File '" + filename + "' does not exist.");

        string errorString = string.Empty;
        if (!WordMailMerger.Merge(originalFile, tmpFilename, null, null, 0, false, true, (string[])notesList.ToArray(typeof(string)), false, null, out errorString))
            throw new CustomMessageException("Error:" + errorString);
    }

    #endregion



    protected void DateAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        try
        {
            CustomValidator cv = (CustomValidator)sender;
            GridViewRow grdRow = ((GridViewRow)cv.Parent.Parent);
            DropDownList _ddlDate_Day = (DropDownList)grdRow.FindControl(grdRow.RowType == DataControlRowType.Footer ? "ddlNewDate_Day" : "ddlDate_Day");
            DropDownList _ddlDate_Month = (DropDownList)grdRow.FindControl(grdRow.RowType == DataControlRowType.Footer ? "ddlNewDate_Month" : "ddlDate_Month");
            DropDownList _ddlDate_Year = (DropDownList)grdRow.FindControl(grdRow.RowType == DataControlRowType.Footer ? "ddlNewDate_Year" : "ddlDate_Year");

            e.IsValid = IsValidDate(_ddlDate_Day.SelectedValue, _ddlDate_Month.SelectedValue, _ddlDate_Year.SelectedValue);
        }
        catch (Exception)
        {
            e.IsValid = false;
        }

    }
    public bool IsValidDate(string day, string month, string year)
    {
        bool invalid = ((day == "-1" || month == "-1" || year == "-1") && (day != "-1" || month != "-1" || year != "-1"));

        if ((day == "-1" || month == "-1" || year == "-1"))
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



    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        GrdNote1.Visible = false;
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