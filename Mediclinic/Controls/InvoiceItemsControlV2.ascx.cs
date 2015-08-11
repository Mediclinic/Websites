using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class InvoiceItemsControlV2 : System.Web.UI.UserControl
{

    public event EventHandler UserControlSubmitClicked;
    public event EventHandler UserControlMakePrivateInvoiceClicked;

    protected bool errorOccurred = false;

    protected static bool IncGstOnInvoices_MC        = false;
    protected static bool IncGstOnInvoices_DVA       = true;
    protected static bool IncGstOnInvoices_Insurance = false;
    protected static bool IncGstOnInvoices_Private   = true;


    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.errorOccurred)
            return;

        HideErrorMessage();
        spnErrorClosePageButtons.Visible = false;
        spnButtons.Visible = true;

        if (!IsPostBack)
        {
            Session.Remove("data_selected");
            Session.Remove("sortExpression_Offering");
            Session.Remove("sortExpression_Selected");
        }
        else
            UpdateAreaTreated();

        this.GrdOffering.EnableViewState = true;
        this.GrdSelectedList.EnableViewState = false;
    }

    #endregion

    #region public Get/Set properties

    private Booking booking;
    public  Booking Booking
    {
        get { return this.booking; }
        set { this.booking = value; }
    }

    public bool LabelSetPrivateInvoiceVisible
    {
        get { return this.spnCreatePrivateInvoice.Visible; }
        set { this.spnCreatePrivateInvoice.Visible = value; }
    }
    public string SubmitButtonText
    {
        get { return btnSubmit.Text; }
        set { btnSubmit.Text = value; }
    }
    public bool ButtonCancelVisible
    {
        get { return this.btnCancel.Visible; }
        set { this.btnCancel.Visible = value; }
    }

    public bool SpanTotalVisible
    {
        get { return this.spnTotal.Visible; }
        set { this.spnTotal.Visible = value; }
    }
    public bool SpanButtonsVisible
    {
        get { return this.spnButtons.Visible; }
        set { this.spnButtons.Visible = value; }
    }
    public bool SpanErrorClosePageButtonsVisible
    {
        get { return this.spnErrorClosePageButtons.Visible; }
        set { this.spnErrorClosePageButtons.Visible = value; }
    }

    public void HideElementsForError()
    {
        this.spnTotal.Visible                 = false;
        this.spnButtons.Visible               = false;
        this.spnCreatePrivateInvoice.Visible  = false;
        this.spnButtons.Visible               = false;

        this.GrdOffering.Visible              = false;
        this.GrdSelectedList.Visible          = false;

        this.spnErrorClosePageButtons.Visible = true;

        this.errorOccurred = true;
    }

    public Booking.InvoiceType InvoiceType
    {
        get 
        {
            if (hiddenField_InvoiceType.Value == "Medicare")
                return Booking.InvoiceType.Medicare;
            else if (hiddenField_InvoiceType.Value == "DVA")
                return Booking.InvoiceType.DVA;
            else if (hiddenField_InvoiceType.Value == "Insurance")
                return Booking.InvoiceType.Insurance;
            else if (hiddenField_InvoiceType.Value == "None")
                return Booking.InvoiceType.None;
            else if (hiddenField_InvoiceType.Value == "NoneFromCombinedYearlyThreshholdReached")
                return Booking.InvoiceType.NoneFromCombinedYearlyThreshholdReached;
            else if (hiddenField_InvoiceType.Value == "NoneFromOfferingYearlyThreshholdReached")
                return Booking.InvoiceType.NoneFromOfferingYearlyThreshholdReached;
            else if (hiddenField_InvoiceType.Value == "NoneFromExpired")
                return Booking.InvoiceType.NoneFromExpired;

            throw new Exception("Unknown invoice type");
        }
        set
        {
            hiddenField_InvoiceType.Value = value.ToString();

            //GrdSelectedList.Columns[1].Visible = (value != Booking.InvoiceType.None);
            //GrdOffering.Columns[3].Visible     = (value != Booking.InvoiceType.None);

            if (value == Booking.InvoiceType.Insurance)
            {
                GrdOffering.Columns[6].HeaderText = "Ins. Price";
                GrdOffering.Columns[7].HeaderText = "Ins. GST";

                GrdSelectedList.Columns[5].HeaderText = "Unit Price (Ins.)";
                GrdSelectedList.Columns[9].HeaderText = "Total (Ins.)";

                lblHcTotalText.Text = "Ins. Total: ";
            }

            GrdOffering.Columns[6].Visible     = (value == Booking.InvoiceType.Medicare || value == Booking.InvoiceType.DVA || value == Booking.InvoiceType.Insurance);
            GrdOffering.Columns[7].Visible     = (value == Booking.InvoiceType.Medicare || value == Booking.InvoiceType.DVA || value == Booking.InvoiceType.Insurance);
            GrdSelectedList.Columns[5].Visible = (value == Booking.InvoiceType.Medicare || value == Booking.InvoiceType.DVA || value == Booking.InvoiceType.Insurance);
            GrdSelectedList.Columns[9].Visible = (value == Booking.InvoiceType.Medicare || value == Booking.InvoiceType.DVA || value == Booking.InvoiceType.Insurance);

            tr_hc_row.Visible       = (value != Booking.InvoiceType.None);
            tr_nonhc_row.Visible    = (value != Booking.InvoiceType.None);
            tr_hc_space_row.Visible = (value != Booking.InvoiceType.None);
        }
    }

    #endregion


    #region UpdateAreaTreated()

    public void UpdateAreaTreated()
    {
        DataTable dt_selected_list = GetSelectedList();

        bool tblEmpty = (dt_selected_list.Rows.Count == 0) || (dt_selected_list.Rows.Count == 1 && dt_selected_list.Rows[0][0] == DBNull.Value);
        if (tblEmpty)
            return;

        for(int i=0; i<GrdSelectedList.Rows.Count; i++)
        {
            Label   lblId               = (Label)GrdSelectedList.Rows[i].FindControl("lblId");
            TextBox txtAreaTreated      = (TextBox)GrdSelectedList.Rows[i].FindControl("txtAreaTreated");
            TextBox txtServiceReference = (TextBox)GrdSelectedList.Rows[i].FindControl("txtServiceReference");

            int     offering_id    = Convert.ToInt32(lblId.Text);

            for (int j = 0; j < dt_selected_list.Rows.Count; j++)
            {
                if (Convert.ToInt32(dt_selected_list.Rows[j]["offering_id"]) == offering_id)
                {
                    dt_selected_list.Rows[j]["area_treated"]      = txtAreaTreated.Text.Trim();
                    dt_selected_list.Rows[j]["service_reference"] = txtServiceReference.Text.Trim();
                }
            }
        }

        Session["data_selected"] = dt_selected_list;
    }

    #endregion


    #region GrdOffering

    public void FillOfferingGrid()
    {
        Booking.InvoiceType invType = InvoiceType;

		decimal GST_Percent  = Convert.ToDecimal(((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["GST_Percent"].Value);
		decimal GST_Modifier = GST_Percent/(decimal)100;

        bool hasGstItems_HC = false;
        bool hasGstItems_PT = false;

        DataTable dt_offering;
        DataTable dt_org_offering = null; // just to get the prices if there is a specific price for this clinic
        try
        {
            if (this.booking == null)
                dt_offering = OrganisationOfferingsDB.GetDataTable_OfferingsByOrg(true, 0);  // get empty datatable
            else
            {
                if (booking.Organisation.OrganisationType.OrganisationTypeGroup.ID == 5) // clinics
                {
                    dt_offering     = OfferingDB.GetDataTable(false, "1,3", "63");
                    dt_org_offering = OrganisationOfferingsDB.GetDataTable_OfferingsByOrg(true, booking.Organisation.OrganisationID, "1,3", "63,89");  // dt_offering = OfferingDB.GetDataTable(1);
                }
                else if (booking.Organisation.OrganisationType.OrganisationTypeGroup.ID == 6)  // aged care
                {
                    dt_offering     = OfferingDB.GetDataTable(false, "3,4", "63");
                    dt_org_offering = OrganisationOfferingsDB.GetDataTable_OfferingsByOrg(true, booking.Organisation.OrganisationID, "3,4", "63,89");  // dt_offering = OfferingDB.GetDataTable(4);
                }
                else
                    throw new Exception("Unknown booking screen type");


                // If row exists in org-offering table, then use that price
                for (int i = 0; i < dt_org_offering.Rows.Count; i++)
                    for (int j = 0; j < dt_offering.Rows.Count; j++)
                        if (Convert.ToInt32(dt_offering.Rows[j]["o_offering_id"]) == Convert.ToInt32(dt_org_offering.Rows[i]["o_offering_id"]))
                            dt_offering.Rows[j]["o_default_price"] = dt_org_offering.Rows[i]["o_default_price"];
            }


            for (int i = dt_offering.Rows.Count - 1; i >= 0; i--)
            {
                // remove service they are here for
                if (booking.Offering.OfferingID == Convert.ToInt32(dt_offering.Rows[i]["o_offering_id"]))
                    dt_offering.Rows.RemoveAt(i);
                else
                {
                    // if pt pays  invoice, use default price for all (so no change)
                    // if medicare invoice, use default price for all offerings OTHER than the service they are here for (so no change)
                    // if dva      invoice, use dva price for all
                    
                    // Remove this ... and show pt price and hc price on the screen and use that on the data tables
                    //
                    //if (invType == Booking.InvoiceType.DVA)
                    //    dt_offering.Rows[i]["o_default_price"] = dt_offering.Rows[i]["o_dva_charge"];
                }
            }


            // add all products (by invoice type id  1 or 4, and offering_type_ids for only products : "89")
            DataTable dt_products = null;
            if (this.booking == null)
            {
                dt_products = OfferingDB.GetDataTable(false, UserView.GetInstance().IsAgedCareView ? "3,4" : "1,3", "89");
            }
            else
            {
                if (booking.Organisation.OrganisationType.OrganisationTypeGroup.ID == 5)
                    dt_products = OfferingDB.GetDataTable(false, "1,3", "89");
                else if (booking.Organisation.OrganisationType.OrganisationTypeGroup.ID == 6)
                    dt_products = OfferingDB.GetDataTable(false, "3,4", "89");
                else
                    throw new Exception("Unknown booking screen type");

                //
                // If row exists in org-offering table, then use that price
                //
                if (dt_org_offering != null)
                {
                    for (int i = 0; i < dt_org_offering.Rows.Count; i++)
                        for (int j = 0; j < dt_products.Rows.Count; j++)
                            if (Convert.ToInt32(dt_products.Rows[j]["o_offering_id"]) == Convert.ToInt32(dt_org_offering.Rows[i]["o_offering_id"]))
                                dt_products.Rows[j]["o_default_price"] = dt_org_offering.Rows[i]["o_default_price"];
                }
            }


            for (int i = 0; i < dt_products.Rows.Count; i++)
                dt_offering.ImportRow(dt_products.Rows[i]);


            bool invoiceGapPayments = Convert.ToInt32(SystemVariableDB.GetByDescr("InvoiceGapPayments").Value) == 1;

            dt_offering.Columns.Add("hc_paid");
            dt_offering.Columns.Add("pt_price", typeof(decimal));
            dt_offering.Columns.Add("hc_price", typeof(decimal));
            dt_offering.Columns.Add("pt_gst",   typeof(decimal));
            dt_offering.Columns.Add("hc_gst",   typeof(decimal));
            for (int i = 0; i < dt_offering.Rows.Count; i++)
            {
                bool isGstExempt = Convert.ToBoolean(dt_offering.Rows[i]["o_is_gst_exempt"]);

                string medicare_company_code = dt_offering.Rows[i]["o_medicare_company_code"].ToString();
                string dva_company_code      = dt_offering.Rows[i]["o_dva_company_code"].ToString();
                string tac_company_code      = dt_offering.Rows[i]["o_tac_company_code"].ToString();

                bool incGstOnPTInvoices = IncGstOnInvoices_Private && !isGstExempt;
                bool incGstOnHCInvoices = (invType == Booking.InvoiceType.Medicare  && IncGstOnInvoices_MC        && !isGstExempt) ||
                                          (invType == Booking.InvoiceType.DVA       && IncGstOnInvoices_DVA       && !isGstExempt) ||
                                          (invType == Booking.InvoiceType.Insurance && IncGstOnInvoices_Insurance && !isGstExempt);

                dt_offering.Rows[i]["pt_price"] = Convert.ToDecimal(dt_offering.Rows[i]["o_default_price"]);
                dt_offering.Rows[i]["hc_price"] = 0;
                dt_offering.Rows[i]["pt_gst"]   = !incGstOnPTInvoices ? 0 : Convert.ToDecimal(dt_offering.Rows[i]["pt_price"]) * GST_Modifier;
                dt_offering.Rows[i]["hc_gst"]   = !incGstOnHCInvoices ? 0 : Convert.ToDecimal(dt_offering.Rows[i]["hc_price"]) * GST_Modifier;



                if (invType == Booking.InvoiceType.DVA)
                {
                    dt_offering.Rows[i]["hc_paid"]  = dva_company_code.Length > 0;

                    if (dva_company_code.Length > 0)
                    {
                        decimal default_price = Convert.ToDecimal(dt_offering.Rows[i]["o_default_price"]);
                        decimal dva_price     = Convert.ToDecimal(dt_offering.Rows[i]["o_dva_charge"]);

                        dt_offering.Rows[i]["pt_price"] = (invoiceGapPayments && default_price > dva_price) ? default_price - dva_price : 0;
                        dt_offering.Rows[i]["hc_price"] = dva_price;
                        dt_offering.Rows[i]["pt_gst"]   = !incGstOnPTInvoices ? 0 : Convert.ToDecimal(dt_offering.Rows[i]["pt_price"]) * GST_Modifier;
                        dt_offering.Rows[i]["hc_gst"]   = !incGstOnHCInvoices ? 0 : Convert.ToDecimal(dt_offering.Rows[i]["hc_price"]) * GST_Modifier;
                    }
                }
                if (invType == Booking.InvoiceType.Insurance)
                {
                    dt_offering.Rows[i]["hc_paid"]  = tac_company_code.Length > 0;

                    //if (tac_company_code.Length > 0)
                    //{
                        decimal default_price = Convert.ToDecimal(dt_offering.Rows[i]["o_default_price"]);
                        decimal tac_price = (tac_company_code.Length > 0) ? Convert.ToDecimal(dt_offering.Rows[i]["o_tac_charge"]) : default_price;

                        dt_offering.Rows[i]["pt_price"] = (invoiceGapPayments && default_price > tac_price) ? default_price - tac_price : 0;
                        dt_offering.Rows[i]["hc_price"] = tac_price;
                        dt_offering.Rows[i]["pt_gst"]   = !incGstOnPTInvoices ? 0 : Convert.ToDecimal(dt_offering.Rows[i]["pt_price"]) * GST_Modifier;
                        dt_offering.Rows[i]["hc_gst"]   = !incGstOnHCInvoices ? 0 : Convert.ToDecimal(dt_offering.Rows[i]["hc_price"]) * GST_Modifier;
                    //}
                }
                else if (InvoiceType == Booking.InvoiceType.Medicare)
                {
                    dt_offering.Rows[i]["hc_paid"] = false; // medicare invoice - all items to add beyond booking offering are privately invoiced
                }
                else
                {
                    dt_offering.Rows[i]["hc_paid"] = false;
                }


                if (!isGstExempt && Convert.ToDecimal(dt_offering.Rows[i]["hc_gst"]) > 0)
                    hasGstItems_HC = true;
                if (!isGstExempt && Convert.ToDecimal(dt_offering.Rows[i]["pt_gst"]) > 0)
                    hasGstItems_PT = true;
            }
        }
        catch (Exception ex)
        {
            SetErrorMessage("", ex.ToString());
            //return;
            throw;
        }

        Session["data_offering"] = dt_offering;


        if (!hasGstItems_HC && !hasGstItems_PT)
        {
            GrdOffering.Columns[7].Visible = false;
            GrdOffering.Columns[5].Visible = false;
        }


        if (dt_offering.Rows.Count > 0)
        {

            if (IsPostBack && Session["sortExpression_Offering"] != null && Session["sortExpression_Offering"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt_offering);
                dataView.Sort = Session["sortExpression_Offering"].ToString();
                GrdOffering.DataSource = dataView;
            }
            else
            {
                GrdOffering.DataSource = dt_offering;
            }


            try
            {
                GrdOffering.DataBind();



                // add items for javascript live search so can have 
                // dropdown that when chosing an item, it clicks the right button

                string fieldsSep = "[[fieldsSep]]";
                string itemSep = "[[itemSep]]";

                string output = string.Empty;
                for (int i = 0; i < GrdOffering.Rows.Count; i++)
                {
                    Label  lblShortName = (Label)GrdOffering.Rows[i].FindControl("lblShortName");
                    Button btnAdd       = (Button)GrdOffering.Rows[i].FindControl("btnAdd");
                    output += (i == 0 ? "" : itemSep) + lblShortName.Text + fieldsSep + btnAdd.ClientID;
                }

                hiddenItemList.Value = output;

                // end live search data


            }
            catch (Exception)
            {
                //SetErrorMessage("", ex.ToString()); // already should be showing in page containing this control

                this.HideElementsForError();
                throw;
            }
        }
        else
        {
            dt_offering.Rows.Add(dt_offering.NewRow());
            GrdOffering.DataSource = dt_offering;
            GrdOffering.DataBind();

            int TotalColumns = GrdOffering.Rows[0].Cells.Count;
            GrdOffering.Rows[0].Cells.Clear();
            GrdOffering.Rows[0].Cells.Add(new TableCell());
            GrdOffering.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdOffering.Rows[0].Cells[0].Text = "No Items Found";
        }
    }
    protected void GrdOffering_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdOffering_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt_offering = Session["data_offering"] as DataTable;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdOffering_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdOffering.EditIndex = -1;
        FillOfferingGrid();
    }
    protected void GrdOffering_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GrdOffering.EditIndex = -1;
        FillOfferingGrid();
    }
    protected void GrdOffering_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdOffering.Rows[e.RowIndex].FindControl("lblId");
        int financial_document_id = Convert.ToInt32(lblId.Text);

        try
        {
            //OfferingDB.UpdateInactive(financial_document_id);
            //PersonDB.Delete(person_id);
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillOfferingGrid();
    }
    protected void GrdOffering_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Add"))
        {
            int offeringID = Int32.Parse((string)e.CommandArgument);
            Add(offeringID);
        }
    }
    protected void btnAdd_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName.Equals("Add"))
        {
            int offeringID = Int32.Parse((string)e.CommandArgument);

            UpdateAreaTreated();
            Add(offeringID);
        }

    }
    protected void Add(int offeringID)
    {

        string areaTreated = string.Empty;
        if (InvoiceType == Booking.InvoiceType.DVA || InvoiceType == Booking.InvoiceType.Insurance)
            areaTreated = HealthCardDB.GetActiveByPatientID(Booking.Patient.PatientID).AreaTreated;


        int index = -1;
        for (int i = 0; i < GrdOffering.Rows.Count; i++)
        {
            Label lbl = (Label)GrdOffering.Rows[i].FindControl("lblId");
            if (lbl != null && lbl.Text == offeringID.ToString())
                index = i;
        }

        if (index == -1)
            return;



        Label lblId = (Label)GrdOffering.Rows[index].FindControl("lblId");
        Label lblShortName = (Label)GrdOffering.Rows[index].FindControl("lblShortName");
        Label lblDefaultPrice = (Label)GrdOffering.Rows[index].FindControl("lblDefaultPrice");


        // see about medicare and dva code ...
        DataTable dt_offering = Session["data_offering"] as DataTable;
        DataRow[] foundRows = dt_offering.Select("o_offering_id=" + lblId.Text);
        DataRow thisRow = foundRows[0];

        string medicare_company_code = thisRow["o_medicare_company_code"].ToString();
        string dva_company_code      = thisRow["o_dva_company_code"].ToString();
        string tac_company_code      = thisRow["o_tac_company_code"].ToString();
        bool hcPaid = (this.InvoiceType == Booking.InvoiceType.DVA && dva_company_code.Length > 0) || (this.InvoiceType == Booking.InvoiceType.Insurance && tac_company_code.Length > 0);


        DataTable dt_selected_list = GetSelectedList();
        foreach (DataRow curRow in dt_selected_list.Rows)
            if (curRow["offering_id"].ToString() == lblId.Text)
                return;

        DataRow row = dt_selected_list.NewRow();
        row["offering_id"]            = lblId.Text;
        row["hc_paid"]                = hcPaid;
        row["short_name"]             = lblShortName.Text;
        row["name"]                   = thisRow["o_name"].ToString();
        row["area_treated"]           = areaTreated;
        row["service_reference"]      = string.Empty;
        row["show_area_treated"]      = InvoiceType == Booking.InvoiceType.DVA || InvoiceType == Booking.InvoiceType.Insurance;
        row["show_service_reference"] = InvoiceType == Booking.InvoiceType.Insurance;
        row["default_price"]          = thisRow["o_default_price"];
        row["pt_price"]               = thisRow["pt_price"];          // added
        row["hc_price"]               = thisRow["hc_price"];          // added
        row["pt_gst"]                 = thisRow["pt_gst"];            // added
        row["hc_gst"]                 = thisRow["hc_gst"];            // added
        row["quantity"]               = "1";
        row["total_line_price"]       = thisRow["o_default_price"];
        row["total_pt_price"]         = thisRow["pt_price"];          // added
        row["total_pt_gst"]           = thisRow["pt_gst"];            // added
        row["total_hc_price"]         = thisRow["hc_price"];          // added
        row["total_hc_gst"]           = thisRow["hc_gst"];            // added
        row["on_order"]               = false;

        dt_selected_list.Rows.Add(row);

        Session["data_selected"] = dt_selected_list;
        UpdateTotal(dt_selected_list);

        FillSelectedGrid();
    }

    protected void GrdOffering_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdOffering.EditIndex = e.NewEditIndex;
        FillOfferingGrid();
    }
    protected void GridOffering_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdOffering.EditIndex >= 0)
            return;

        DataTable dataTable = Session["data_offering"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression_Offering"] == null)
                Session["sortExpression_Offering"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression_Offering"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["sortExpression_Offering"] = e.SortExpression + " " + newSortExpr;

            GrdOffering.DataSource = dataView;
            GrdOffering.DataBind();
        }
    }

    #endregion


    #region GrdSelectedList

    public void FillSelectedGrid()
    {
        DataTable dt_selected_list = GetSelectedList();

        if (dt_selected_list.Rows.Count > 0)
        {

            if (IsPostBack && Session["sortExpression_Selected"] != null && Session["sortExpression_Selected"].ToString().Length > 0)
            {
                string s = Session["sortExpression_Selected"].ToString();

                DataView dataView = new DataView(dt_selected_list);
                dataView.Sort = Session["sortExpression_Selected"].ToString();
                GrdSelectedList.DataSource = dataView;
            }
            else
            {
                GrdSelectedList.DataSource = dt_selected_list;
            }

            UpdateTotal(dt_selected_list);

            try
            {
                GrdSelectedList.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.ToString());
            }
        }
        else
        {
            dt_selected_list.Rows.Add(dt_selected_list.NewRow());
            GrdSelectedList.DataSource = dt_selected_list;
            GrdSelectedList.DataBind();

            int TotalColumns = GrdSelectedList.Rows[0].Cells.Count;
            GrdSelectedList.Rows[0].Cells.Clear();
            GrdSelectedList.Rows[0].Cells.Add(new TableCell());
            GrdSelectedList.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdSelectedList.Rows[0].Cells[0].Text = "Add Items From The Offerings List.";
        }
    }
    protected void GrdSelectedList_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdSelectedList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dt = Session["data_selected"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 0) || (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("offering_id = '" + lblId.Text + "'");
            DataRow thisRow = foundRows[0];

            if (this.booking != null && Convert.ToInt32(lblId.Text) == this.booking.Offering.OfferingID)
            {
                e.Row.Cells[11].ColumnSpan = 2;
                foreach (Control c in e.Row.Cells[11].Controls)
                    c.Visible = false;
                e.Row.Cells[12].Visible = false;
            }
            else
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    e.Row.Cells[11].ColumnSpan = 2;
                    e.Row.Cells[12].Visible = false;

                    TextBox txtQuantity = (TextBox)e.Row.FindControl("txtQuantity");
                    if (txtQuantity != null)
                        txtQuantity.Focus();
                }

            }



            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
        }
    }
    protected void GrdSelectedList_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        UpdateAreaTreated();
        GrdSelectedList.EditIndex = -1;
        FillSelectedGrid();
    }
    protected void GrdSelectedList_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        UpdateAreaTreated();

        Label lblId = (Label)GrdSelectedList.Rows[e.RowIndex].FindControl("lblId");
        TextBox txtQuantity = (TextBox)GrdSelectedList.Rows[e.RowIndex].FindControl("txtQuantity");

        DataTable dt_selected_list = Session["data_selected"] as DataTable;
        for (int i = 0; i < dt_selected_list.Rows.Count; i++)
        {
            if (dt_selected_list.Rows[i]["offering_id"].ToString() == lblId.Text)
            {
                dt_selected_list.Rows[i]["quantity"] = txtQuantity.Text;

                double total_line_price = Convert.ToDouble(txtQuantity.Text) * Convert.ToDouble(dt_selected_list.Rows[i]["default_price"]);
                dt_selected_list.Rows[i]["total_line_price"] = string.Format("{0:0.00}", total_line_price);

                double total_pt_price = Convert.ToDouble(txtQuantity.Text) * Convert.ToDouble(dt_selected_list.Rows[i]["pt_price"]);
                dt_selected_list.Rows[i]["total_pt_price"] = string.Format("{0:0.00}", total_pt_price);

                double total_hc_price = Convert.ToDouble(txtQuantity.Text) * Convert.ToDouble(dt_selected_list.Rows[i]["hc_price"]);
                dt_selected_list.Rows[i]["total_hc_price"] = string.Format("{0:0.00}", total_hc_price);
            }
        }

        // do the update
        Session["data_selected"] = dt_selected_list;

        UpdateTotal(dt_selected_list);

        GrdSelectedList.EditIndex = -1;
        FillSelectedGrid();

    }
    protected void GrdSelectedList_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        UpdateAreaTreated();

        FillSelectedGrid();

        Label lblId = (Label)GrdSelectedList.Rows[e.RowIndex].FindControl("lblId");

        if (this.booking != null && Convert.ToInt32(lblId.Text) == this.booking.Offering.OfferingID)
        {
            SetErrorMessage("Can not remove the offering that this booking was for. You can only modify the quantity.");
            return;
        }


        DataTable dt_selected_list = Session["data_selected"] as DataTable;
        for (int i = dt_selected_list.Rows.Count-1; i >= 0 ; i--)
            if (dt_selected_list.Rows[i]["offering_id"].ToString() == lblId.Text)
                dt_selected_list.Rows.RemoveAt(i);

        // do the update
        Session["data_selected"] = dt_selected_list;

        UpdateTotal(dt_selected_list);

        FillSelectedGrid();
    }
    protected void GrdSelectedList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("AddOne") || e.CommandName.Equals("SubtractOne"))
        {
            UpdateAreaTreated();

            int index = Int32.Parse((string)e.CommandArgument);
            Label lblId = (Label)GrdSelectedList.Rows[index].FindControl("lblId");

            DataTable dt_selected_list = Session["data_selected"] as DataTable;
            for (int i = 0; i < dt_selected_list.Rows.Count; i++)
            {
                if (dt_selected_list.Rows[i]["offering_id"].ToString() == lblId.Text)
                {
                    if (e.CommandName.Equals("AddOne"))
                    {
                        dt_selected_list.Rows[i]["quantity"] = (Convert.ToInt32(dt_selected_list.Rows[i]["quantity"].ToString()) + 1).ToString();
                    }
                    if (e.CommandName.Equals("SubtractOne"))
                    {
                        if (Convert.ToInt32(dt_selected_list.Rows[i]["quantity"].ToString()) <= 1)
                            continue;

                        dt_selected_list.Rows[i]["quantity"] = (Convert.ToInt32(dt_selected_list.Rows[i]["quantity"].ToString()) - 1).ToString();
                    }

                    double total_line_price = Convert.ToDouble(dt_selected_list.Rows[i]["quantity"]) * Convert.ToDouble(dt_selected_list.Rows[i]["default_price"]);
                    dt_selected_list.Rows[i]["total_line_price"] = string.Format("{0:0.00}", total_line_price);

                    double total_pt_price = Convert.ToDouble(dt_selected_list.Rows[i]["quantity"]) * Convert.ToDouble(dt_selected_list.Rows[i]["pt_price"]);
                    dt_selected_list.Rows[i]["total_pt_price"] = string.Format("{0:0.00}", total_pt_price);

                    double total_hc_price = Convert.ToDouble(dt_selected_list.Rows[i]["quantity"]) * Convert.ToDouble(dt_selected_list.Rows[i]["hc_price"]);
                    dt_selected_list.Rows[i]["total_hc_price"] = string.Format("{0:0.00}", total_hc_price);

                    double total_pt_gst = Convert.ToDouble(dt_selected_list.Rows[i]["quantity"]) * Convert.ToDouble(dt_selected_list.Rows[i]["pt_gst"]);
                    dt_selected_list.Rows[i]["total_pt_gst"] = string.Format("{0:0.00}", total_pt_gst);

                    double total_hc_gst = Convert.ToDouble(dt_selected_list.Rows[i]["quantity"]) * Convert.ToDouble(dt_selected_list.Rows[i]["hc_gst"]);
                    dt_selected_list.Rows[i]["total_hc_gst"] = string.Format("{0:0.00}", total_hc_gst);


                }
            }

            // do the update
            Session["data_selected"] = dt_selected_list;

            UpdateTotal(dt_selected_list);

            FillSelectedGrid();
        }
    }
    protected void GrdSelectedList_RowEditing(object sender, GridViewEditEventArgs e)
    {
        UpdateAreaTreated();
        GrdSelectedList.EditIndex = e.NewEditIndex;
        FillSelectedGrid();
    }
    protected void GridSelectedList_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdSelectedList.EditIndex >= 0)
            return;

        DataTable dataTable = Session["data_selected"] as DataTable;

        if (dataTable != null)
        {
            if (Session["sortExpression_Selected"] == null)
                Session["sortExpression_Selected"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["sortExpression_Selected"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["sortExpression_Selected"] = e.SortExpression + " " + newSortExpr;

            GrdSelectedList.DataSource = dataView;
            GrdSelectedList.DataBind();
        }
    }

    protected void chkOnOrder_CheckedChanged(object sender, EventArgs e)
    {
        DataTable dt_selected_list = Session["data_selected"] as DataTable;
        for (int j = 0; j < GrdSelectedList.Rows.Count; j++)
        {
            Label    lblId      = (Label)GrdSelectedList.Rows[j].FindControl("lblId");
            CheckBox chkOnOrder = (CheckBox)GrdSelectedList.Rows[j].FindControl("chkOnOrder");
            for (int i = 0; i < dt_selected_list.Rows.Count; i++)
                if (dt_selected_list.Rows[i]["offering_id"].ToString() == lblId.Text)
                    dt_selected_list.Rows[i]["on_order"] = chkOnOrder.Checked;
        }

        // do the update
        Session["data_selected"] = dt_selected_list;
    }

    #endregion

    #region GetSelectedList()

    public DataTable GetSelectedList()
    {
        DataTable dt_selected_list = Session["data_selected"] as DataTable;

        string areaTreated = string.Empty;
        if (InvoiceType == Booking.InvoiceType.DVA || InvoiceType == Booking.InvoiceType.Insurance)
            areaTreated = HealthCardDB.GetActiveByPatientID(Booking.Patient.PatientID).AreaTreated;

        if (dt_selected_list == null)
        {
            dt_selected_list = new DataTable();
            dt_selected_list.Columns.Add(new DataColumn("offering_id"));
            dt_selected_list.Columns.Add(new DataColumn("hc_paid"));
            dt_selected_list.Columns.Add(new DataColumn("short_name"));
            dt_selected_list.Columns.Add(new DataColumn("name"));
            dt_selected_list.Columns.Add(new DataColumn("area_treated"));
            dt_selected_list.Columns.Add(new DataColumn("service_reference"));
            dt_selected_list.Columns.Add(new DataColumn("show_area_treated", typeof(Boolean)));
            dt_selected_list.Columns.Add(new DataColumn("show_service_reference", typeof(Boolean)));
            dt_selected_list.Columns.Add(new DataColumn("default_price"));
            dt_selected_list.Columns.Add(new DataColumn("pt_price"));                // added
            dt_selected_list.Columns.Add(new DataColumn("hc_price"));                // added
            dt_selected_list.Columns.Add(new DataColumn("pt_gst"));                  // added
            dt_selected_list.Columns.Add(new DataColumn("hc_gst"));                  // added
            dt_selected_list.Columns.Add(new DataColumn("quantity"));
            dt_selected_list.Columns.Add(new DataColumn("total_line_price"));
            dt_selected_list.Columns.Add(new DataColumn("total_line_gst"));
            dt_selected_list.Columns.Add(new DataColumn("total_pt_price"));          // added
            dt_selected_list.Columns.Add(new DataColumn("total_hc_price"));          // added
            dt_selected_list.Columns.Add(new DataColumn("total_pt_gst"));            // added
            dt_selected_list.Columns.Add(new DataColumn("total_hc_gst"));            // added
            dt_selected_list.Columns.Add(new DataColumn("on_order", typeof(Boolean))); 


            if (this.booking != null)
            {
                Booking.InvoiceType invType = InvoiceType;

                Offering orgOffering = OrganisationOfferingsDB.GetOfferingByOrgAndOffering(this.booking.Organisation.OrganisationID, this.booking.Offering.OfferingID);
                if (orgOffering == null)
                    orgOffering = this.booking.Offering;

                bool    invoiceGapPayments = Convert.ToInt32(SystemVariableDB.GetByDescr("InvoiceGapPayments").Value) == 1;
                decimal GST_Percent = Convert.ToDecimal(((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["GST_Percent"].Value);
                decimal GST_Modifier = GST_Percent / (decimal)100;

                bool incGstOnPTInvoices = IncGstOnInvoices_Private && !orgOffering.IsGstExempt;
                bool incGstOnHCInvoices = (invType == Booking.InvoiceType.Medicare  && IncGstOnInvoices_MC        && !orgOffering.IsGstExempt) ||
                                          (invType == Booking.InvoiceType.DVA       && IncGstOnInvoices_DVA       && !orgOffering.IsGstExempt) ||
                                          (invType == Booking.InvoiceType.Insurance && IncGstOnInvoices_Insurance && !orgOffering.IsGstExempt);


                decimal pt_price = orgOffering.DefaultPrice;
                decimal hc_price = 0;
                decimal pt_tax = !incGstOnPTInvoices ? 0 : pt_price * GST_Modifier;
                decimal hc_tax = !incGstOnHCInvoices ? 0 : hc_price * GST_Modifier;

                if (invType == Booking.InvoiceType.Medicare)
                {
                    pt_price = invoiceGapPayments && orgOffering.DefaultPrice > orgOffering.MedicareCharge ? orgOffering.DefaultPrice - orgOffering.MedicareCharge : 0;
                    hc_price = orgOffering.MedicareCharge;
                    pt_tax   = !incGstOnPTInvoices ? 0 : pt_price * GST_Modifier;
                    hc_tax = !incGstOnHCInvoices ? 0 : hc_price * GST_Modifier;
                }
                if (invType == Booking.InvoiceType.DVA)
                {
                    pt_price = invoiceGapPayments && orgOffering.DefaultPrice > orgOffering.DvaCharge ? orgOffering.DefaultPrice - orgOffering.DvaCharge : 0;
                    hc_price = orgOffering.DvaCharge;
                    pt_tax   = !incGstOnPTInvoices ? 0 : pt_price * GST_Modifier;
                    hc_tax   = !incGstOnHCInvoices ? 0 : hc_price * GST_Modifier;
                }
                if (invType == Booking.InvoiceType.Insurance)
                {
                    hc_price = orgOffering.TacCompanyCode.Length > 0 ? orgOffering.TacCharge : orgOffering.DefaultPrice;
                    pt_price = invoiceGapPayments && orgOffering.DefaultPrice > hc_price ? orgOffering.DefaultPrice - hc_price : 0;
                    pt_tax   = !incGstOnPTInvoices ? 0 : pt_price * GST_Modifier;
                    hc_tax   = !incGstOnHCInvoices ? 0 : hc_price * GST_Modifier;
                }


                DataRow row = dt_selected_list.NewRow();
                row["offering_id"]            = booking.Offering.OfferingID;
                row["hc_paid"]                = (this.InvoiceType == Booking.InvoiceType.DVA || this.InvoiceType == Booking.InvoiceType.Medicare);
                row["short_name"]             = booking.Offering.ShortName;
                row["name"]                   = booking.Offering.Name;
                row["area_treated"]           = areaTreated;
                row["service_reference"]      = "";
                row["show_area_treated"]      = InvoiceType == Booking.InvoiceType.DVA || InvoiceType == Booking.InvoiceType.Insurance;
                row["show_service_reference"] = InvoiceType == Booking.InvoiceType.Insurance;
                row["default_price"]          = row["total_line_price"] = orgOffering.DefaultPrice;
                row["pt_price"]               = row["total_pt_price"]   =  pt_price;                   // added
                row["pt_gst"]                 = row["total_pt_gst"]     =  pt_tax;                     // added
                row["hc_price"]               = row["total_hc_price"]   =  hc_price;                   // added
                row["hc_gst"]                 = row["total_hc_gst"]     =  hc_tax;                     // added
                row["quantity"]               = "1";
                row["on_order"]               = false;
                dt_selected_list.Rows.Add(row);
            }

            Session["data_selected"] = dt_selected_list;
        }

        if (dt_selected_list.Rows.Count == 1 && dt_selected_list.Rows[0][0] == DBNull.Value)
            dt_selected_list.Rows.RemoveAt(0);

        return dt_selected_list;
    }

    #endregion

    #region UpdateTotal(dt_selected_list)

    private void UpdateTotal(DataTable dt_selected_list)
    {
        decimal ptTotal = 0;
        decimal hcTotal = 0;
        decimal ptGST   = 0;
        decimal hcGST   = 0;


        for (int i = 0; i < dt_selected_list.Rows.Count; i++)
        {
            if (dt_selected_list.Rows[i]["total_pt_price"] != DBNull.Value && dt_selected_list.Rows[i]["total_hc_price"] != DBNull.Value)
            {
                ptTotal += Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]);
                hcTotal += Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_price"]);
                ptGST   += Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]);
                hcGST   += Convert.ToDecimal(dt_selected_list.Rows[i]["total_hc_gst"]);
            }
        }



        lblHcTotalPrice.Text    = hcGST == 0         ? string.Format("{0:C}", hcTotal)           : string.Format("{0:C}", hcTotal + hcGST)                   + " &nbsp;&nbsp; <span style=\"font-weight: normal;\"> (Inc GST " + string.Format("{0:C}", hcGST)         + " )</span>";
        lblNonHcTotalPrice.Text = ptGST == 0         ? string.Format("{0:C}", ptTotal)           : string.Format("{0:C}", ptTotal + ptGST)                   + " &nbsp;&nbsp; <span style=\"font-weight: normal;\"> (Inc GST " + string.Format("{0:C}", ptGST)         + " )</span>";
        lblTotalPrice.Text      = ptGST + hcGST == 0 ? string.Format("{0:C}", ptTotal + hcTotal) : string.Format("{0:C}", ptTotal + hcTotal + ptGST + hcGST) + " &nbsp;&nbsp; <span style=\"font-weight: normal;\"> (Inc GST " + string.Format("{0:C}", ptGST + hcGST) + " )</span>";
    }

    #endregion


    #region btnSubmit_Click

    public void btnSubmit_Click(object sender, EventArgs e)
    {
        if (GrdSelectedList.EditIndex != -1)
        {
            SetErrorMessage("Please cancel or update item quantity edit before completing.");
            return;
        }

        DataTable dt_selected_list = GetSelectedList();
        if (dt_selected_list.Rows.Count == 0)
        {
            FillSelectedGrid();
            SetErrorMessage("Please select some offerings to be invoiced.");
            return;
        }

        UpdateAreaTreated();

        if (UserControlSubmitClicked != null)
            UserControlSubmitClicked(this, EventArgs.Empty);
    }

    #endregion

    #region lnkCreatePrivateInvoice_Click

    protected void lnkCreatePrivateInvoice_Click(object sender, EventArgs e)
    {
        if (UserControlMakePrivateInvoiceClicked != null)
            UserControlMakePrivateInvoiceClicked(this, EventArgs.Empty);
    }

    #endregion

    #region SetErrorMessage, HideErrorMessag

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
        spnButtons.Visible = true;
        spnErrorClosePageButtons.Visible = false;

        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}