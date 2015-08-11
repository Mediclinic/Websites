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

public partial class OfferingListV2 : System.Web.UI.Page
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
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, true, false, false, true);
                Session.Remove("offeringinfo_sortexpression");
                Session.Remove("offeringinfo_data");

                if (IsValidFormInvType())
                {
                    IDandDescr invType = GetFormInvType();
                    lblHeading.Text = "Products & Services For " + invType.Descr;
                }

                FillGrid();
                txtSearchName.Focus();
            }

            this.GrdOffering.EnableViewState = true;

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

    #region GetUrlParamType()

    private bool IsValidFormInvType()
    {
        string id = Request.QueryString["inv_type"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private IDandDescr GetFormInvType()
    {
        if (!IsValidFormInvType())
            throw new Exception("Invalid url inv type");

        DataTable invTypesByUrlParam = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "OfferingInvoiceType", "offering_invoice_type_id=" + Request.QueryString["inv_type"], "", "offering_invoice_type_id", "descr");
        if (invTypesByUrlParam.Rows.Count == 0)
            throw new Exception("Invalid url inv type");

        return IDandDescrDB.Load(invTypesByUrlParam.Rows[0], "offering_invoice_type_id", "descr");
    }

    private bool IsValidFormInvTypes()
    {
        string ids = Request.QueryString["inv_type"];
        return ids != null && Regex.IsMatch(ids, @"^(\d+)(,\s*\d+)*$");
    }
    private string GetFormInvTypes()
    {
        if (!IsValidFormInvTypes())
            throw new Exception("Invalid url inv type");

        return Request.QueryString["inv_type"];
    }

    private bool IsValidIsAgedCareResidentTypes()
    {
        string id = Request.QueryString["is_ac_res_types"];
        return id != null && (id == "1" || id == "0");
    }
    private bool GetFormIsAgedCareResidentTypes()
    {
        if (!IsValidIsAgedCareResidentTypes())
            throw new Exception("Invalid url is_ac_res_types");

        return Request.QueryString["is_ac_res_types"] == "1";
    }

    #endregion

    #region GrdOffering

    protected void FillGrid()
    {
        bool isAgedCareResidentTypes = IsValidIsAgedCareResidentTypes() ? GetFormIsAgedCareResidentTypes() : false;
        if (isAgedCareResidentTypes)
            lblHeading.Text = "Aged Care Resident Types";


        string searchName = "";
        if (Request.QueryString["name_search"] != null && Request.QueryString["name_search"].Length > 0)
        {
            searchName = Request.QueryString["name_search"];
            txtSearchName.Text = Request.QueryString["name_search"];
        }
        bool searchNameOnlyStartsWith = true;
        if (Request.QueryString["name_starts_with"] != null && Request.QueryString["name_starts_with"].Length > 0)
        {
            searchNameOnlyStartsWith = Request.QueryString["name_starts_with"] == "0" ? false : true;
            chkSearchOnlyStartWith.Checked = searchNameOnlyStartsWith;
        }
        else
        {
            chkSearchOnlyStartWith.Checked = searchNameOnlyStartsWith;
        }


        DataTable dt = OfferingDB.GetDataTable(isAgedCareResidentTypes, "1,3,4", null, chkShowDeleted.Checked, searchName, searchNameOnlyStartsWith);
        Session["offeringinfo_data"] = dt;


        this.GrdOffering.AllowPaging = false;
        if (!IsPostBack)
            chkUsePaging.Checked = dt.Rows.Count > 50;
        this.GrdOffering.AllowPaging = chkUsePaging.Checked;


        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["offeringinfo_sortexpression"] != null && Session["offeringinfo_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["offeringinfo_sortexpression"].ToString();
                GrdOffering.DataSource = dataView;
            }
            else
            {
                GrdOffering.DataSource = dt;
            }


            try
            {
                GrdOffering.DataBind();
                GrdOffering.PagerSettings.FirstPageText = "1";
                GrdOffering.PagerSettings.LastPageText = GrdOffering.PageCount.ToString();
                GrdOffering.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdOffering.DataSource = dt;
            GrdOffering.DataBind();

            int TotalColumns = GrdOffering.Rows[0].Cells.Count;
            GrdOffering.Rows[0].Cells.Clear();
            GrdOffering.Rows[0].Cells.Add(new TableCell());
            GrdOffering.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdOffering.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdOffering_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Pager)
        {
            foreach (DataControlField col in GrdOffering.Columns)
                if (!chkShowDeleted.Checked && col.HeaderText.ToLower().Trim() == "deleted")
                    e.Row.Cells[GrdOffering.Columns.IndexOf(col)].CssClass = "hiddencol";

            if (!Utilities.IsDev())
                e.Row.Cells[0].CssClass = "hiddencol";

            e.Row.Cells[2].CssClass = "hiddencol";
            e.Row.Cells[3].CssClass = "hiddencol";

            bool isAgedCareResidentTypes = IsValidIsAgedCareResidentTypes() ? GetFormIsAgedCareResidentTypes() : false;
            if (UserView.GetInstance().IsClinicView || !isAgedCareResidentTypes)
                e.Row.Cells[6].CssClass = "hiddencol"; // Patient Subcategory (Aged Care) -- add/update in to add/edit code so if invis, puts right one in.

            if (isAgedCareResidentTypes)
            {
                e.Row.Cells[4].CssClass  = "hiddencol";  // is always a 'Service'
                e.Row.Cells[8].CssClass  = "hiddencol";  // is always 'A.C.Fac. Booking'
                e.Row.Cells[7].CssClass  = "hiddencol";  // Yearly Visits Allowed
                e.Row.Cells[10].CssClass = "hiddencol";  // Service Time Mins
                e.Row.Cells[17].CssClass = "hiddencol";  // Max Claimable Nbr
                e.Row.Cells[18].CssClass = "hiddencol";  // Max Claimable Months
                e.Row.Cells[19].CssClass = "hiddencol";  // Reminder Letters - Months
                e.Row.Cells[20].CssClass = "hiddencol";  // Reminder Letter
            }
        }
    }
    protected void GrdOffering_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable offeringTypes                = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "OfferingType", "offering_type_id <> 90", "", "offering_type_id", "descr");
        DataTable fields                       = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Field", "has_offerings = 1", "descr", "field_id", "descr");
        DataTable offeringPatientSubcategories = DBBase.GetGenericDataTable(null, "AgedCarePatientType", "aged_care_patient_type_id", "descr");
        DataTable offeringInvoiceTypes         = DBBase.GetGenericDataTable(null, "OfferingInvoiceType", "offering_invoice_type_id", "descr");
        DataTable letters                      = LetterDB.GetDataTable_ByLetterType(391);

        int BookingScreenDefaultServiceID = Convert.ToInt32(SystemVariableDB.GetByDescr("BookingScreenDefaultServiceID").Value);

        // move 'None' item to the top
        for (int i = 0; i < fields.Rows.Count; i++)
        {
            if (fields.Rows[i]["descr"].ToString() != "None")
                continue;

            DataRow newRow = fields.NewRow();
            newRow.ItemArray = fields.Rows[i].ItemArray;
            fields.Rows.RemoveAt(i);
            fields.Rows.InsertAt(newRow, 0);
            break;
        }


        DataTable dt = Session["offeringinfo_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("o_offering_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            int offering_id = Convert.ToInt32(thisRow["o_offering_id"]);


            TextBox txtName = (TextBox)e.Row.FindControl("txtName");
            if (txtName != null)
            {
                if (Session["DB"] != null && Session["DB"].ToString() == "Mediclinic_0040")
                {
                    txtName.MaxLength = 2000;
                    txtName.TextMode = TextBoxMode.MultiLine;
                }
                else if (thisRow["o_name"].ToString().Contains(Environment.NewLine))
                {
                    txtName.TextMode = TextBoxMode.MultiLine;
                }
            }          


            DropDownList ddlOfferingType = (DropDownList)e.Row.FindControl("ddlOfferingType");
            if (ddlOfferingType != null)
            {
                ddlOfferingType.DataSource = offeringTypes;
                ddlOfferingType.DataTextField = "descr";
                ddlOfferingType.DataValueField = "offering_type_id";
                ddlOfferingType.DataBind();
                ddlOfferingType.SelectedValue = thisRow["o_offering_type_id"].ToString();
            }

            DropDownList ddlField = (DropDownList)e.Row.FindControl("ddlField");
            if (ddlField != null)
            {
                ddlField.DataSource = fields;
                ddlField.DataTextField = "descr";
                ddlField.DataValueField = "field_id";
                ddlField.DataBind();
                ddlField.SelectedValue = thisRow["o_field_id"].ToString();
            }

            DropDownList ddlOfferingPatientSubcategory = (DropDownList)e.Row.FindControl("ddlOfferingPatientSubcategory");
            if (ddlOfferingPatientSubcategory != null)
            {
                ddlOfferingPatientSubcategory.DataSource = offeringPatientSubcategories;
                ddlOfferingPatientSubcategory.DataTextField = "descr";
                ddlOfferingPatientSubcategory.DataValueField = "aged_care_patient_type_id";
                ddlOfferingPatientSubcategory.DataBind();
                ddlOfferingPatientSubcategory.SelectedValue = thisRow["o_aged_care_patient_type_id"].ToString();
            }

            DropDownList ddlOfferingInvoiceType = (DropDownList)e.Row.FindControl("ddlOfferingInvoiceType");
            if (ddlOfferingInvoiceType != null)
            {
                if (IsValidFormInvType())
                {
                    IDandDescr invType = GetFormInvType();
                    ddlOfferingInvoiceType.Items.Add(new ListItem(invType.Descr, invType.ID.ToString()));
                }
                else
                {
                    string v = thisRow["o_offering_invoice_type_id"].ToString();

                    ddlOfferingInvoiceType.DataSource = offeringInvoiceTypes;
                    ddlOfferingInvoiceType.DataTextField = "descr";
                    ddlOfferingInvoiceType.DataValueField = "offering_invoice_type_id";
                    ddlOfferingInvoiceType.DataBind();
                    ddlOfferingInvoiceType.SelectedValue = thisRow["o_offering_invoice_type_id"].ToString();
                }
            }

            DropDownList ddlServiceTimeMinutes = (DropDownList)e.Row.FindControl("ddlServiceTimeMinutes");
            if (ddlServiceTimeMinutes != null)
            {
                ddlServiceTimeMinutes.Items.AddRange(GetListOfTimes());
                int valueToSelect = Convert.ToInt32(dt.Rows[e.Row.RowIndex]["o_service_time_minutes"]);
                if (ddlServiceTimeMinutes.Items.FindByValue(valueToSelect.ToString()) != null)
                    ddlServiceTimeMinutes.SelectedValue = valueToSelect.ToString();
                else
                {
                    ddlServiceTimeMinutes.Items.Clear();
                    ddlServiceTimeMinutes.Items.AddRange(GetListOfTimes(valueToSelect));
                    ddlServiceTimeMinutes.SelectedValue = valueToSelect.ToString();
                }
            }

            DropDownList ddlNumClinicVisitsAllowedPerYear = (DropDownList)e.Row.FindControl("ddlNumClinicVisitsAllowedPerYear");
            if (ddlNumClinicVisitsAllowedPerYear != null)
            {
                for (int i = 0; i < 6; i++)
                    ddlNumClinicVisitsAllowedPerYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlNumClinicVisitsAllowedPerYear.SelectedValue = thisRow["o_num_clinic_visits_allowed_per_year"].ToString();
            }

            DropDownList ddlMaxNbrClaimable = (DropDownList)e.Row.FindControl("ddlMaxNbrClaimable");
            if (ddlMaxNbrClaimable != null)
            {
                for (int i = 0; i < 10; i++)
                    ddlMaxNbrClaimable.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlMaxNbrClaimable.SelectedValue = thisRow["o_max_nbr_claimable"].ToString();
            }

            DropDownList ddlMaxNbrClaimableMonths = (DropDownList)e.Row.FindControl("ddlMaxNbrClaimableMonths");
            if (ddlMaxNbrClaimableMonths != null)
            {
                for (int i = 0; i <= 24; i++)
                    ddlMaxNbrClaimableMonths.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlMaxNbrClaimableMonths.SelectedValue = thisRow["o_max_nbr_claimable_months"].ToString();
            }

            DropDownList ddlReminderLetterMonthsLaterToSend = (DropDownList)e.Row.FindControl("ddlReminderLetterMonthsLaterToSend");
            if (ddlReminderLetterMonthsLaterToSend != null)
            {
                for (int i = 0; i <= 24; i++)
                    ddlReminderLetterMonthsLaterToSend.Items.Add(new ListItem((i == 0 ? "Disabled" : i.ToString()), i.ToString()));
                ddlReminderLetterMonthsLaterToSend.SelectedValue = thisRow["o_reminder_letter_months_later_to_send"].ToString();
            }

            DropDownList ddlReminderLetter = (DropDownList)e.Row.FindControl("ddlReminderLetter");
            if (ddlReminderLetter != null)
            {
                ddlReminderLetter.Items.Add(new ListItem("--", "-1"));
                for (int i = 0; i < letters.Rows.Count; i++)
                    ddlReminderLetter.Items.Add(new ListItem(letters.Rows[i]["letter_docname"].ToString(), letters.Rows[i]["letter_letter_id"].ToString()));
                if (thisRow["o_reminder_letter_id"] != DBNull.Value)
                    ddlReminderLetter.SelectedValue = thisRow["o_reminder_letter_id"].ToString();
            }

            Label lblReminderLetter = (Label)e.Row.FindControl("lblReminderLetter");
            if (lblReminderLetter != null)
            {
                if (thisRow["o_reminder_letter_id"] == DBNull.Value)
                    lblReminderLetter.Text = string.Empty;

                else
                {
                    for (int i = 0; i < letters.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(letters.Rows[i]["letter_letter_id"]) == Convert.ToInt32(thisRow["o_reminder_letter_id"]))
                        {
                            lblReminderLetter.Text = letters.Rows[i]["letter_docname"].ToString();
                            break;
                        }
                    }
                }
            }
            


            
            ImageButton lnkPopupMessage = (ImageButton)e.Row.FindControl("lnkPopupMessage");
            if (lnkPopupMessage != null)
            {
                string allFeatures = "dialogWidth:550px;dialogHeight:400px;center:yes;resizable:no; scroll:no";
                string js = "javascript:window.showModalDialog('OfferingPopupMessageDetailV2.aspx?type=edit&id=" + offering_id.ToString() + "', '', '" + allFeatures + "');return false;";

                lnkPopupMessage.Visible = true;
                lnkPopupMessage.PostBackUrl = "  ";
                lnkPopupMessage.Attributes.Add("onclick", js);
            }


            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["o_is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName = "_UnDelete";
                    btnDelete.ImageUrl = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip = "UnDelete";
                }
            }


            LinkButton lnkUpdate = (LinkButton)e.Row.FindControl("lnkUpdate");
            if (lnkUpdate != null)
            {
                TextBox txtMedicareCharge = (TextBox)e.Row.FindControl("txtMedicareCharge");
                TextBox txtDvaCharge = (TextBox)e.Row.FindControl("txtDvaCharge");
                TextBox txtTacCharge = (TextBox)e.Row.FindControl("txtTacCharge");

                lnkUpdate.OnClientClick = "set_if_empty_price(document.getElementById('" + txtMedicareCharge.ClientID + "'),document.getElementById('" + txtDvaCharge.ClientID + "'),document.getElementById('" + txtTacCharge.ClientID + "'));";
            }

            Image imgBookingScreenDefaultService = (Image)e.Row.FindControl("imgBookingScreenDefaultService");
            imgBookingScreenDefaultService.Visible = offering_id == BookingScreenDefaultServiceID;

            LinkButton btnSetAsBookingScreenDefaultService = (LinkButton)e.Row.FindControl("btnSetAsBookingScreenDefaultService");
            btnSetAsBookingScreenDefaultService.Visible = offering_id != BookingScreenDefaultServiceID;


            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlOfferingType = (DropDownList)e.Row.FindControl("ddlNewOfferingType");
            ddlOfferingType.DataSource = offeringTypes;
            ddlOfferingType.DataBind();

            DropDownList ddlField = (DropDownList)e.Row.FindControl("ddlNewField");
            ddlField.DataSource = fields;
            ddlField.DataBind();

            DropDownList ddlOfferingPatientSubcategory = (DropDownList)e.Row.FindControl("ddlNewOfferingPatientSubcategory");
            ddlOfferingPatientSubcategory.DataSource = offeringPatientSubcategories;
            ddlOfferingPatientSubcategory.DataBind();

            bool isAgedCareResidentTypes = IsValidIsAgedCareResidentTypes() ? GetFormIsAgedCareResidentTypes() : false;
            if (isAgedCareResidentTypes)
                ddlOfferingPatientSubcategory.Items.RemoveAt(0);

            DropDownList ddlOfferingInvoiceType = (DropDownList)e.Row.FindControl("ddlNewOfferingInvoiceType");
            if (IsValidFormInvType())
            {
                IDandDescr invType = GetFormInvType();
                ddlOfferingInvoiceType.Items.Add(new ListItem(invType.Descr, invType.ID.ToString()));
            }
            else
            {
                //ddlOfferingInvoiceType.DataSource = offeringInvoiceTypes;
                //ddlOfferingInvoiceType.DataBind();
                foreach (DataRow row in offeringInvoiceTypes.Rows)
                    if (row["offering_invoice_type_id"].ToString() != "0")
                        ddlOfferingInvoiceType.Items.Add(new ListItem(row["descr"].ToString(), row["offering_invoice_type_id"].ToString()));

                ddlOfferingInvoiceType.SelectedValue = UserView.GetInstance().IsClinicView ? "1" : "4"; // 1 = clinics, 4 = aged care
            }

            DropDownList ddlServiceTimeMinutes = (DropDownList)e.Row.FindControl("ddlNewServiceTimeMinutes");
            for (int i = 0; i < 20; i++)
                ddlServiceTimeMinutes.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 20; i <= 90; i++)
                if (i % 5 == 0)
                    ddlServiceTimeMinutes.Items.Add(new ListItem(i.ToString(), i.ToString()));
            for (int i = 91; i <= 480; i++)
                if (i % 30 == 0)
                    ddlServiceTimeMinutes.Items.Add(new ListItem(i.ToString(), i.ToString()));


            DropDownList ddlNumClinicVisitsAllowedPerYear = (DropDownList)e.Row.FindControl("ddlNewNumClinicVisitsAllowedPerYear");
            if (ddlNumClinicVisitsAllowedPerYear != null)
            {
                for (int i = 0; i < 6; i++)
                    ddlNumClinicVisitsAllowedPerYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }

            DropDownList ddlMaxNbrClaimable = (DropDownList)e.Row.FindControl("ddlNewMaxNbrClaimable");
            for (int i = 0; i < 10; i++)
                ddlMaxNbrClaimable.Items.Add(new ListItem(i.ToString(), i.ToString()));

            DropDownList ddlMaxNbrClaimableMonths = (DropDownList)e.Row.FindControl("ddlNewMaxNbrClaimableMonths");
            for (int i = 0; i <= 24; i++)
                ddlMaxNbrClaimableMonths.Items.Add(new ListItem(i.ToString(), i.ToString()));

            DropDownList ddlReminderLetterMonthsLaterToSend = (DropDownList)e.Row.FindControl("ddlNewReminderLetterMonthsLaterToSend");
            for (int i = 0; i <= 24; i++)
                ddlReminderLetterMonthsLaterToSend.Items.Add(new ListItem((i == 0 ? "Disabled" : i.ToString()), i.ToString()));

            DropDownList ddlReminderLetter = (DropDownList)e.Row.FindControl("ddlNewReminderLetter");
            ddlReminderLetter.Items.Add(new ListItem("--", "-1"));
            for (int i = 0; i < letters.Rows.Count; i++)
                ddlReminderLetter.Items.Add(new ListItem(letters.Rows[i]["letter_docname"].ToString(), letters.Rows[i]["letter_letter_id"].ToString()));


            LinkButton lnkAdd = (LinkButton)e.Row.FindControl("lnkAdd");
            TextBox txtMedicareCharge = (TextBox)e.Row.FindControl("txtNewMedicareCharge");
            TextBox txtDvaCharge = (TextBox)e.Row.FindControl("txtNewDvaCharge");
            TextBox txtTacCharge = (TextBox)e.Row.FindControl("txtNewTacCharge");
            lnkAdd.OnClientClick = "set_if_empty_price(document.getElementById('" + txtMedicareCharge.ClientID + "'),document.getElementById('" + txtDvaCharge.ClientID + "'),document.getElementById('" + txtTacCharge.ClientID + "'));";

        }
    }

    private ListItem[] GetListOfTimes(int valueToSelect = -1)
    {
        ArrayList list = new ArrayList();
        for (int i = 0; i < 20; i++)
            list.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 20; i < 91; i++)
        {
            if (i % 5 == 0)
                list.Add(new ListItem(i.ToString(), i.ToString()));
            else if (valueToSelect == i+1)
                list.Add(new ListItem(valueToSelect.ToString(), valueToSelect.ToString()));
        }
        for (int i = 91; i <= 480; i++)
        {
            if (i % 30 == 0)
                list.Add(new ListItem(i.ToString(), i.ToString()));
            else if (valueToSelect == i + 1)
                list.Add(new ListItem(valueToSelect.ToString(), valueToSelect.ToString()));
        }
        if (valueToSelect > 480)
            list.Add(new ListItem(valueToSelect.ToString(), valueToSelect.ToString()));

        return (ListItem[])list.ToArray(typeof(ListItem));
    }

    protected void GrdOffering_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdOffering.EditIndex = -1;
        FillGrid();
    }
    protected void GrdOffering_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label   lblId = (Label)GrdOffering.Rows[e.RowIndex].FindControl("lblId");
        TextBox txtName = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtName");
        TextBox txtShortName = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtShortName");
        TextBox txtDescr = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtDescr");
        DropDownList ddlOfferingType = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlOfferingType");
        DropDownList ddlField = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlField");
        DropDownList ddlOfferingPatientSubcategory = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlOfferingPatientSubcategory");
        DropDownList ddlNumClinicVisitsAllowedPerYear = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlNumClinicVisitsAllowedPerYear");
        DropDownList ddlOfferingInvoiceType = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlOfferingInvoiceType");
        CheckBox chkIsGstExempt = (CheckBox)GrdOffering.Rows[e.RowIndex].FindControl("chkIsGstExempt");
        TextBox txtDefaultPrice = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtDefaultPrice");
        DropDownList ddlServiceTimeMinutes = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlServiceTimeMinutes");
        DropDownList ddlMaxNbrClaimable = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlMaxNbrClaimable");
        DropDownList ddlMaxNbrClaimableMonths = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlMaxNbrClaimableMonths");
        TextBox txtMedicareCompanyCode = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtMedicareCompanyCode");
        TextBox txtDvaCompanyCode = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtDvaCompanyCode");
        TextBox txtTacCompanyCode = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtTacCompanyCode");
        TextBox txtMedicareCharge = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtMedicareCharge");
        TextBox txtDvaCharge = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtDvaCharge");
        TextBox txtTacCharge = (TextBox)GrdOffering.Rows[e.RowIndex].FindControl("txtTacCharge");
        DropDownList ddlReminderLetterMonthsLaterToSend = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlReminderLetterMonthsLaterToSend");
        DropDownList ddlReminderLetter = (DropDownList)GrdOffering.Rows[e.RowIndex].FindControl("ddlReminderLetter");

        CheckBox chkUseCustomColour = (CheckBox)GrdOffering.Rows[e.RowIndex].FindControl("chkUseCustomColour");
        System.Web.UI.HtmlControls.HtmlInputText ColorPicker = (System.Web.UI.HtmlControls.HtmlInputText)GrdOffering.Rows[e.RowIndex].FindControl("ColorPicker");


        if (Convert.ToInt32(ddlReminderLetterMonthsLaterToSend.SelectedValue) > 0 && Convert.ToInt32(ddlReminderLetter.SelectedValue) == -1)
        {
            SetErrorMessage("For reminder letters - you must either set the number of months as disabled or select a reminder letter.");
            return;
        }

        Offering offering = OfferingDB.GetByID(Convert.ToInt32(lblId.Text));

        // if logged not AC system, set as was 
        // if logged not Clinic system, set as was
        // these are hidden in the gui also in method 'GrdOffering_RowCreated'
        int offeringPatientSubcategoryID = !UserView.GetInstance().IsAgedCareView ? offering.AgedCarePatientType.ID : Convert.ToInt32(ddlOfferingPatientSubcategory.SelectedValue);

        OfferingDB.Update(Convert.ToInt32(lblId.Text),
            Convert.ToInt32(ddlOfferingType.SelectedValue), Convert.ToInt32(ddlField.SelectedValue),
            offeringPatientSubcategoryID,
            Convert.ToInt32(ddlNumClinicVisitsAllowedPerYear.SelectedValue), 
            Convert.ToInt32(ddlOfferingInvoiceType.SelectedValue),
            txtName.Text, txtShortName.Text, txtDescr.Text,
            chkIsGstExempt.Checked, Convert.ToDecimal(txtDefaultPrice.Text), Convert.ToInt32(ddlServiceTimeMinutes.Text),
            Convert.ToInt32(ddlMaxNbrClaimable.SelectedValue), Convert.ToInt32(ddlMaxNbrClaimableMonths.SelectedValue),
            txtMedicareCompanyCode.Text.Trim(), txtDvaCompanyCode.Text.Trim(), txtTacCompanyCode.Text.Trim(),
            Convert.ToDecimal(txtMedicareCharge.Text), Convert.ToDecimal(txtDvaCharge.Text), Convert.ToDecimal(txtTacCharge.Text),offering.PopupMessage,
            Convert.ToInt32(ddlReminderLetterMonthsLaterToSend.SelectedValue),
            Convert.ToInt32(ddlReminderLetter.SelectedValue),
            chkUseCustomColour.Checked,
            ColorPicker.Value
            );

        Session["OfferingColors"] = OfferingDB.GetColorCodes();

        GrdOffering.EditIndex = -1;
        FillGrid();
    }
    protected void GrdOffering_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdOffering.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            OfferingDB.UpdateInactive(Convert.ToInt32(lblId.Text));
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrid();
    }
    protected void GrdOffering_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            TextBox txtName = (TextBox)GrdOffering.FooterRow.FindControl("txtNewName");
            TextBox txtShortName = (TextBox)GrdOffering.FooterRow.FindControl("txtNewShortName");
            TextBox txtDescr = (TextBox)GrdOffering.FooterRow.FindControl("txtNewDescr");
            DropDownList ddlOfferingType = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewOfferingType");
            DropDownList ddlField = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewField");
            DropDownList ddlOfferingPatientSubcategory = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewOfferingPatientSubcategory");
            DropDownList ddlNumClinicVisitsAllowedPerYear = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewNumClinicVisitsAllowedPerYear");
            DropDownList ddlOfferingInvoiceType = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewOfferingInvoiceType");
            CheckBox chkIsGstExempt = (CheckBox)GrdOffering.FooterRow.FindControl("chkNewIsGstExempt");
            TextBox txtDefaultPrice = (TextBox)GrdOffering.FooterRow.FindControl("txtNewDefaultPrice");
            DropDownList ddlServiceTimeMinutes = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewServiceTimeMinutes");
            DropDownList ddlMaxNbrClaimable = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewMaxNbrClaimable");
            DropDownList ddlMaxNbrClaimableMonths = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewMaxNbrClaimableMonths");
            TextBox txtMedicareCompanyCode = (TextBox)GrdOffering.FooterRow.FindControl("txtNewMedicareCompanyCode");
            TextBox txtDvaCompanyCode = (TextBox)GrdOffering.FooterRow.FindControl("txtNewDvaCompanyCode");
            TextBox txtTacCompanyCode = (TextBox)GrdOffering.FooterRow.FindControl("txtNewTacCompanyCode");
            TextBox txtMedicareCharge = (TextBox)GrdOffering.FooterRow.FindControl("txtNewMedicareCharge");
            TextBox txtDvaCharge = (TextBox)GrdOffering.FooterRow.FindControl("txtNewDvaCharge");
            TextBox txtTacCharge = (TextBox)GrdOffering.FooterRow.FindControl("txtNewTacCharge");
            DropDownList ddlReminderLetterMonthsLaterToSend = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewReminderLetterMonthsLaterToSend");
            DropDownList ddlReminderLetter = (DropDownList)GrdOffering.FooterRow.FindControl("ddlNewReminderLetter");
            CheckBox chkUseCustomColour = (CheckBox)GrdOffering.FooterRow.FindControl("chkNewUseCustomColour");
            System.Web.UI.HtmlControls.HtmlInputText ColorPicker = (System.Web.UI.HtmlControls.HtmlInputText)GrdOffering.FooterRow.FindControl("NewColorPicker");


            if (Convert.ToInt32(ddlReminderLetterMonthsLaterToSend.SelectedValue) > 0 && Convert.ToInt32(ddlReminderLetter.SelectedValue) == -1)
            {
                SetErrorMessage("For reminder letters - you must either set the number of months as disabled or select a reminder letter.");
                return;
            }


            // if logged not AC system, set AC patient subcat as 1 (--Not Aged Care--)
            // if logged not Clinic system, set clinic visit type as -1 (--Not Clinic--)
            // these are hidden in the gui also in method 'GrdOffering_RowCreated'
            int offeringPatientSubcategoryID = !UserView.GetInstance().IsAgedCareView ? 1 : Convert.ToInt32(ddlOfferingPatientSubcategory.SelectedValue);

            OfferingDB.Insert(Convert.ToInt32(ddlOfferingType.SelectedValue), Convert.ToInt32(ddlField.SelectedValue),
                offeringPatientSubcategoryID,
                Convert.ToInt32(ddlNumClinicVisitsAllowedPerYear.SelectedValue), 
                Convert.ToInt32(ddlOfferingInvoiceType.SelectedValue),
                txtName.Text, txtShortName.Text, txtDescr.Text,
                chkIsGstExempt.Checked, Convert.ToDecimal(txtDefaultPrice.Text), Convert.ToInt32(ddlServiceTimeMinutes.Text),
                Convert.ToInt32(ddlMaxNbrClaimable.SelectedValue), Convert.ToInt32(ddlMaxNbrClaimableMonths.SelectedValue),
                txtMedicareCompanyCode.Text.Trim(), txtDvaCompanyCode.Text.Trim(), txtTacCompanyCode.Text.Trim(),
                Convert.ToDecimal(txtMedicareCharge.Text), Convert.ToDecimal(txtDvaCharge.Text), Convert.ToDecimal(txtTacCharge.Text), "",
                Convert.ToInt32(ddlReminderLetterMonthsLaterToSend.SelectedValue),
                Convert.ToInt32(ddlReminderLetter.SelectedValue),
                chkUseCustomColour.Checked,
                ColorPicker.Value
                );

            Session["OfferingColors"] = OfferingDB.GetColorCodes();

            FillGrid();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int offering_id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    OfferingDB.UpdateInactive(offering_id);
                else
                    OfferingDB.UpdateActive(offering_id);
            }
            catch (ForeignKeyConstraintException fkcEx)
            {
                if (Utilities.IsDev())
                    SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
                else
                    SetErrorMessage("Can not delete because other records depend on this");
            }

            FillGrid();
        }

        if (e.CommandName.Equals("SetAsBookingScreenDefaultService"))
        {
            int offering_id = Convert.ToInt32(e.CommandArgument);
            SystemVariableDB.Update("BookingScreenDefaultServiceID", offering_id.ToString());
            FillGrid();
        }


        
    }
    protected void GrdOffering_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdOffering.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdOffering.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }
    protected void GrdOffering_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdOffering.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["offeringinfo_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["offeringinfo_sortexpression"] == null)
                Session["offeringinfo_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["offeringinfo_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["offeringinfo_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdOffering.DataSource = dataView;
            GrdOffering.DataBind();
        }
    }

    #endregion

    #region btnSearchName_Click, btnClearNameSearch_Click

    protected void btnSearchName_Click(object sender, EventArgs e)
    {
        //if (!Regex.IsMatch(txtSearchName.Text, @"^[a-zA-Z\-]*$"))
        //{
        //    SetErrorMessage("Search text can only be letters and hyphens");
        //    return;
        //}
        //else 
        if (txtSearchName.Text.Trim().Length == 0)
        {
            SetErrorMessage("No search text entered");
            return;
        }
        else
            HideErrorMessage();


        string url = Request.RawUrl;
        url = UrlParamModifier.AddEdit(url, "name_search", txtSearchName.Text);
        url = UrlParamModifier.AddEdit(url, "name_starts_with", chkSearchOnlyStartWith.Checked ? "1" : "0");
        Response.Redirect(url);
    }
    protected void btnClearNameSearch_Click(object sender, EventArgs e)
    {
        string url = Request.RawUrl;
        url = UrlParamModifier.Remove(url, "name_search");
        url = UrlParamModifier.Remove(url, "name_starts_with");
        Response.Redirect(url);
    }

    protected void chkUsePaging_CheckedChanged(object sender, EventArgs e)
    {
        this.GrdOffering.AllowPaging = chkUsePaging.Checked;
        FillGrid();
    }
    protected void chkShowDeleted_CheckedChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    #endregion

    #region btnUpdatePrice_Command

    protected void btnUpdatePrice_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "UpdateMedicarePriceAll")
        {
            decimal newPrice = Convert.ToDecimal(txtNewPrice.Text);
            OfferingDB.UpdateAllMedicare(newPrice);
            FillGrid();
        }
    }

    #endregion

    #region btnUpdateGstExempt_Command

    protected void btnUpdateGstExempt_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "SetAllGstExempt")
        {
            OfferingDB.UpdateAllGstExempt(true);
            FillGrid();
        }
        if (e.CommandName == "SetAllGstNotExempt")
        {
            OfferingDB.UpdateAllGstExempt(false);
            FillGrid();
        }
    }

    #endregion
    
    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
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