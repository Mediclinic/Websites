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

public partial class OrganisationListV2 : System.Web.UI.Page
{

    protected static string Multiply(string s, int count)
    {
        string output = string.Empty;
        for (int i = 0; i < count; i++)
            output += s;
        return output;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);
                Session.Remove("organisationinfo_sortexpression");
                Session.Remove("organisationinfo_data");

                FillGrid();

                switch (GetUrlParamType())
                {
                    case UrlParamType.Clinic:
                        lblHeading.InnerText = "Clinics"; break;
                    case UrlParamType.AgedCare:
                        lblHeading.InnerText = "Aged Care Facilities"; break;
                    case UrlParamType.Insurance:
                        lblHeading.InnerText = "Insurance Companies"; spn_icons.Visible = false;  break;
                    case UrlParamType.External:
                        if (IsValidFormOrgTypeIDs() && GetFormOrgTypeIDs() == "191")
                            lblHeading.InnerText = "External Organisations - Medical Practices";
                        else
                            lblHeading.InnerText = "External Organisations";
                        break;
                }

                // only stakeholder and master admin can delete or view deleted
                UserView userView = UserView.GetInstance();
                if (!userView.IsAdminView)
                {
                    chkShowDeleted.Visible = false;
                    lblShowDeleted.Visible = false;
                }

                txtSearchName.Focus();
            }


            this.GrdOrganisation.EnableViewState = true;

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

    #region GetUrlParamCard(), GetUrlParamType()

    protected enum UrlParamType { Clinic, AgedCare, External, Insurance, None };
    protected UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "clinic")
            return UrlParamType.Clinic;
        else if (type != null && type.ToLower() == "ac")
            return UrlParamType.AgedCare;
        else if (type != null && type.ToLower() == "ins")
            return UrlParamType.Insurance;
        else if (type != null && type.ToLower() == "ext")
            return UrlParamType.External;
        else
            return UrlParamType.None;
    }

    protected bool IsValidFormOrgTypeIDs()
    {
        string org_type_ids = Request.QueryString["org_type_ids"];
        return org_type_ids != null;
    }
    protected string GetFormOrgTypeIDs()
    {
        if (!IsValidFormOrgTypeIDs())
            throw new Exception("Invalid org type id");

        string org_type_ids = Request.QueryString["org_type_ids"];
        return org_type_ids;
    }

    #endregion

    #region GrdOrganisation

    protected DataTable GetOrgsDaTatable(UrlParamType urlParamType, bool showDeleted, string searchName = "", bool searchNameOnlyStartsWith = false)
    {
        DataTable dt = null;
        switch (GetUrlParamType())
        {
            case UrlParamType.Clinic:
                dt = OrganisationDB.GetDataTable_Clinics(chkShowDeleted.Checked, searchName, searchNameOnlyStartsWith);
                break;
            case UrlParamType.AgedCare:
                dt = OrganisationDB.GetDataTable_AgedCareFacs(chkShowDeleted.Checked, searchName, searchNameOnlyStartsWith);
                break;
            case UrlParamType.Insurance:
                dt = OrganisationDB.GetDataTable_Insurance(chkShowDeleted.Checked, searchName, searchNameOnlyStartsWith);
                break;
            case UrlParamType.External:
                if (IsValidFormOrgTypeIDs())
                    dt = OrganisationDB.GetDataTable_External(chkShowDeleted.Checked, searchName, searchNameOnlyStartsWith, true, GetFormOrgTypeIDs());
                else
                    dt = OrganisationDB.GetDataTable_External(chkShowDeleted.Checked, searchName, searchNameOnlyStartsWith);
                break;
            default:
                dt = OrganisationDB.GetDataTable(0, chkShowDeleted.Checked, true, false, false, true, false, searchName, searchNameOnlyStartsWith);
                break;
        }

        return dt;
    }

    protected void FillGrid()
    {
        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + (UserView.GetInstance().IsClinicView ? "Clinics" : "Facilies");

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


        DataTable dt = GetOrgsDaTatable(GetUrlParamType(), chkShowDeleted.Checked, searchName, searchNameOnlyStartsWith);
        Session["organisationinfo_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            if (IsPostBack && Session["organisationinfo_sortexpression"] != null && Session["organisationinfo_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["organisationinfo_sortexpression"].ToString();
                GrdOrganisation.DataSource = dataView;
            }
            else
            {
                GrdOrganisation.DataSource = dt;
            }

            GrdOrganisation.AllowPaging = false;

            try
            {
                GrdOrganisation.DataBind();
                GrdOrganisation.PagerSettings.FirstPageText = "1";
                GrdOrganisation.PagerSettings.LastPageText = GrdOrganisation.PageCount.ToString();
                GrdOrganisation.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdOrganisation.DataSource = dt;
            GrdOrganisation.DataBind();

            int TotalColumns = GrdOrganisation.Rows[0].Cells.Count;
            GrdOrganisation.Rows[0].Cells.Clear();
            GrdOrganisation.Rows[0].Cells.Add(new TableCell());
            GrdOrganisation.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdOrganisation.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdOrganisation_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";


        UrlParamType urlParamType = GetUrlParamType();

        if (e.Row.Cells.Count > 2)
        {
            if (urlParamType == UrlParamType.External)
            {
                // dont have booking link for external orgs
                e.Row.Cells[16].CssClass = "hiddencol";
            }
        }

        if (urlParamType == UrlParamType.Insurance)
        {
            if (e.Row.Cells.Count > 1) e.Row.Cells[1].CssClass = "hiddencol";
            if (e.Row.Cells.Count > 2) e.Row.Cells[2].CssClass = "hiddencol";
        }


        if (e.Row.RowType != DataControlRowType.Pager)
        {
            foreach (DataControlField col in GrdOrganisation.Columns)
            {
                // remove type
                if (urlParamType == UrlParamType.Clinic && col.SortExpression == "organisation_type_id")
                    e.Row.Cells[GrdOrganisation.Columns.IndexOf(col)].CssClass = "hiddencol";

                // remove external stuff -- remember to insert/update it for inserts/updates
                if (urlParamType == UrlParamType.External)
                    if (col.SortExpression == "bpay_account"                ||
                        col.SortExpression == "weeks_per_service_cycle"     ||
                        col.SortExpression == "free_services"               ||
                        col.SortExpression == "start_date"                  ||
                        col.SortExpression == "end_date"                    ||
                        col.SortExpression == "last_batch_run")
                        e.Row.Cells[GrdOrganisation.Columns.IndexOf(col)].CssClass = "hiddencol";

                if (urlParamType == UrlParamType.External && IsValidFormOrgTypeIDs() && GetFormOrgTypeIDs() == "191")
                    if (col.SortExpression == "parent_organisation_id"      ||
                        col.SortExpression == "use_parent_offernig_prices"  ||
                        col.SortExpression == "organisation_type_id"        ||
                        col.SortExpression == "ct_organisation_customer_type_id")
                        e.Row.Cells[GrdOrganisation.Columns.IndexOf(col)].CssClass = "hiddencol";

                if (urlParamType == UrlParamType.Insurance)
                    if (col.SortExpression == "parent_organisation_id"              ||
                        col.SortExpression == "use_parent_offernig_prices"          ||
                        col.SortExpression == "organisation_type_id"                ||
                        col.SortExpression == "ct_organisation_customer_type_id"    ||
                        col.SortExpression == "bpay_account"                        ||
                        col.SortExpression == "weeks_per_service_cycle"             ||
                        col.SortExpression == "free_services"                       ||
                        col.SortExpression == "start_date"                          ||
                        col.SortExpression == "end_date"                            ||
                        col.SortExpression == "last_batch_run")
                        e.Row.Cells[GrdOrganisation.Columns.IndexOf(col)].CssClass = "hiddencol";


                if (!chkShowDeleted.Checked)
                    if (col.SortExpression == "is_deleted")
                        e.Row.Cells[GrdOrganisation.Columns.IndexOf(col)].CssClass = "hiddencol";

                if (col.SortExpression == "is_debtor" || col.SortExpression == "is_creditor")
                    e.Row.Cells[GrdOrganisation.Columns.IndexOf(col)].CssClass = "hiddencol";

                // only admins can delete or view deleted
                if (GrdOrganisation.Columns.IndexOf(col) == GrdOrganisation.Columns.Count - 1)
                {
                    UserView userView = UserView.GetInstance();
                    if (!userView.IsAdminView)
                        e.Row.Cells[GrdOrganisation.Columns.IndexOf(col)].CssClass = "hiddencol";
                }

            }
        }

    }
    protected void GrdOrganisation_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        DataTable types = null;
        switch (GetUrlParamType())
        {
            case UrlParamType.Clinic:
                types = OrganisationTypeDB.GetDataTable_Clinics();
                break;
            case UrlParamType.AgedCare:
                types = OrganisationTypeDB.GetDataTable_AgedCareFacs();
                break;
            case UrlParamType.External:
                if (IsValidFormOrgTypeIDs())
                    types = OrganisationTypeDB.GetDataTable_External(GetFormOrgTypeIDs().ToString());
                else
                    types = OrganisationTypeDB.GetDataTable_External();
                break;
            default:
                types = OrganisationTypeDB.GetDataTable();
                break;
        }

        //DataTable custTypes2 = DBBase.GetGenericDataTable(null, "OrganisationCustomerType", "organisation_customer_type_id", "descr");
        DataTable custTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "OrganisationCustomerType", "organisation_customer_type_id NOT IN (139,275)", "", "organisation_customer_type_id", "descr");
        DataTable dt = Session["organisationinfo_data"] as DataTable;
        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            bool hasData = dt.Rows[0][0].ToString() != string.Empty;
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("organisation_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];


            HyperLink lnkRegistraterReferrers = (HyperLink)e.Row.FindControl("lnkRegistraterReferrers");
            if (lnkRegistraterReferrers != null)
                lnkRegistraterReferrers.Visible = (dt.Rows[e.Row.RowIndex]["organisation_type_id"].ToString() == "191");


            ImageButton lnkFullEdit = (ImageButton)e.Row.FindControl("lnkFullEdit");
            if (lnkFullEdit != null)
            {
                if (Request.QueryString["type"] == null || !hasData)
                    lnkFullEdit.Visible = false;
                else
                    lnkFullEdit.PostBackUrl = "~/OrganisationDetailV2.aspx?type=view&id=" + Convert.ToInt32(lblId.Text) + "&orgtype=" + Request.QueryString["type"];
            }

            DropDownList ddlParent = (DropDownList)e.Row.FindControl("ddlParent");
            if (ddlParent != null)
            {
                /*
                Organisation[] thisOrg = new Organisation[] { OrganisationDB.GetByID(Convert.ToInt32(lblId.Text)) };

                bool exclClinics      = GetUrlParamType() != UrlParamType.None && GetUrlParamType() != UrlParamType.Clinic;
                bool exclAgedCareFacs = GetUrlParamType() != UrlParamType.None && GetUrlParamType() != UrlParamType.AgedCare;
                bool exclExternal     = GetUrlParamType() != UrlParamType.None && GetUrlParamType() != UrlParamType.External;
                bool exclIns          = GetUrlParamType() != UrlParamType.None && GetUrlParamType() != UrlParamType.Insurance;
                DataTable parentList = OrganisationDB.GetDataTable_AllNotInc(thisOrg, false, exclClinics, exclAgedCareFacs, exclIns, exclExternal);

                ddlParent.Items.Add(new ListItem("--","0"));
                foreach (DataRow row in parentList.Rows)
                    ddlParent.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
                ddlParent.SelectedValue = thisRow["parent_organisation_id"].ToString();
                */

                ddlParent.Items.Clear();
                ddlParent.Items.Add(new ListItem("--", "0"));
                foreach (DataRow row in GetOrgsDaTatable(GetUrlParamType(), true).Rows)
                {
                    bool isThisOrg = Convert.ToInt32(row["organisation_id"]) == Convert.ToInt32(thisRow["organisation_id"]);
                    bool isDeleted = Convert.ToBoolean(row["is_deleted"]);
                    bool isParentOrg = thisRow["parent_organisation_id"] != DBNull.Value && Convert.ToInt32(row["organisation_id"]) == Convert.ToInt32(thisRow["parent_organisation_id"]);
                    if (!isThisOrg && (!isDeleted || isParentOrg))
                        ddlParent.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
                }
                ddlParent.SelectedValue = thisRow["parent_organisation_id"].ToString();
            }
            DropDownList ddlType = (DropDownList)e.Row.FindControl("ddlType");
            if (ddlType != null)
            {
                foreach (DataRow row in types.Rows)
                {
                    string prefix = Convert.ToInt32(row["organisation_type_group_id"]) == 4 ? "Ext. " : "";
                    ddlType.Items.Add(new ListItem(prefix + row["descr"].ToString(), row["organisation_type_id"].ToString()));
                }
                ddlType.SelectedValue = thisRow["organisation_type_id"].ToString();
            }
            DropDownList ddlCustType = (DropDownList)e.Row.FindControl("ddlCustType");
            if (ddlCustType != null)
            {
                ddlCustType.DataSource = custTypes;
                ddlCustType.DataTextField = "descr";
                ddlCustType.DataValueField = "organisation_customer_type_id";
                ddlCustType.DataBind();

                if (ddlCustType.Items.FindByValue(thisRow["ct_organisation_customer_type_id"].ToString()) == null)
                    ddlCustType.Items.Add(new ListItem(thisRow["ct_descr"].ToString(), thisRow["ct_organisation_customer_type_id"].ToString()));

                ddlCustType.SelectedValue = thisRow["ct_organisation_customer_type_id"].ToString();
            }
            DropDownList ddlServiceCycle = (DropDownList)e.Row.FindControl("ddlServiceCycle");
            if (ddlServiceCycle != null)
            {
                for (int i = 0; i <= 52; i++)
                    ddlServiceCycle.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlServiceCycle.SelectedValue = thisRow["weeks_per_service_cycle"].ToString();
            }
            DropDownList ddlFreeServices = (DropDownList)e.Row.FindControl("ddlFreeServices");
            if (ddlFreeServices != null)
            {
                for (int i = 0; i <= 5; i++)
                    ddlFreeServices.Items.Add(new ListItem(i.ToString(), i.ToString()));
                ddlFreeServices.SelectedValue = thisRow["free_services"].ToString();
            }


            HyperLink lnkBookings = (HyperLink)e.Row.FindControl("lnkBookings");
            HyperLink lnkBookingList = (HyperLink)e.Row.FindControl("lnkBookingList");

            if (lnkBookings != null && lnkBookingList != null && hasData &&
                  (Convert.ToInt32(dt.Rows[e.Row.RowIndex]["type_organisation_type_group_id"]) == 5 ||
                   Convert.ToInt32(dt.Rows[e.Row.RowIndex]["type_organisation_type_group_id"]) == 6))
            {
                int organisation_type_id = Convert.ToInt32(thisRow["organisation_type_id"]);
                int organisation_id = Convert.ToInt32(thisRow["organisation_id"]);

                if (organisation_type_id == 367 || organisation_type_id == 372 || organisation_type_id == 139)
                {
                    lnkBookings.NavigateUrl = String.Format("~/BookingsV2.aspx?orgs={0}", organisation_id);
                    lnkBookingList.NavigateUrl = String.Format("~/BookingsListV2.aspx?org={0}", organisation_id);
                }
                else if (organisation_type_id == 218)
                {
                    lnkBookings.NavigateUrl = String.Format("~/BookingsV2.aspx?orgs={0}", organisation_id);
                    lnkBookingList.NavigateUrl = String.Format("~/BookingsListV2.aspx?org={0}", organisation_id);
                }
                else
                {
                    lnkBookings.Visible = false;
                    lnkBookingList.Visible = false;
                }
            }

            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            if (btnDelete != null)
            {
                bool is_deleted = Convert.ToBoolean(thisRow["is_deleted"]);
                if (is_deleted)
                {
                    btnDelete.CommandName = "_UnDelete";
                    btnDelete.ImageUrl = "~/images/tick-24.png";
                    btnDelete.AlternateText = "UnDelete";
                    btnDelete.ToolTip = "UnDelete";
                }
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlParent = (DropDownList)e.Row.FindControl("ddlNewParent");
            if (ddlParent != null)
            {
                /*
                bool exclClinics      = GetUrlParamType() != UrlParamType.None && GetUrlParamType() != UrlParamType.Clinic;
                bool exclAgedCareFacs = GetUrlParamType() != UrlParamType.None && GetUrlParamType() != UrlParamType.AgedCare;
                bool exclExternal     = GetUrlParamType() != UrlParamType.None && GetUrlParamType() != UrlParamType.External;
                bool exclIns          = GetUrlParamType() != UrlParamType.None && GetUrlParamType() != UrlParamType.Insurance;
                DataTable parentList = OrganisationDB.GetDataTable(false, exclClinics, exclAgedCareFacs, exclIns, exclExternal);
                ddlParent.Items.Clear();
                ddlParent.Items.Add(new ListItem("--", "-1"));
                foreach (DataRow row in parentList.Rows)
                    ddlParent.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
                */

                ddlParent.Items.Clear();
                ddlParent.Items.Add(new ListItem("--", "0"));
                foreach (DataRow row in GetOrgsDaTatable(GetUrlParamType(), false).Rows)
                    ddlParent.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
            }

            DropDownList ddlNewType = (DropDownList)e.Row.FindControl("ddlNewType");
            foreach (DataRow row in types.Rows)
            {
                string prefix = Convert.ToInt32(row["organisation_type_group_id"]) == 5 || Convert.ToInt32(row["organisation_type_group_id"]) == 6 || GetUrlParamType() == UrlParamType.External ? "" : "Ext. ";
                if (Convert.ToInt32(row["organisation_type_group_id"]) == 1)
                    prefix = "";
                ddlNewType.Items.Add(new ListItem(prefix + row["descr"].ToString(), row["organisation_type_id"].ToString()));
            }

            DropDownList ddlCustType = (DropDownList)e.Row.FindControl("ddlNewCustType");
            ddlCustType.DataSource = custTypes;
            ddlCustType.DataTextField = "descr";
            ddlCustType.DataValueField = "organisation_customer_type_id";
            ddlCustType.DataBind();

            DropDownList ddlServiceCycle = (DropDownList)e.Row.FindControl("ddlNewServiceCycle");
            for (int i = 0; i <= 52; i++)
                ddlServiceCycle.Items.Add(new ListItem(i.ToString(), i.ToString()));

            DropDownList ddlFreeServices = (DropDownList)e.Row.FindControl("ddlNewFreeServices");
            for (int i = 0; i <= 5; i++)
                ddlFreeServices.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
    }
    protected void GrdOrganisation_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdOrganisation.EditIndex = -1;
        FillGrid();
    }
    protected void GrdOrganisation_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdOrganisation.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlParent = (DropDownList)GrdOrganisation.Rows[e.RowIndex].FindControl("ddlParent");
        DropDownList ddlUseParentOffernigPrices = (DropDownList)GrdOrganisation.Rows[e.RowIndex].FindControl("ddlUseParentOffernigPrices");
        TextBox txtName = (TextBox)GrdOrganisation.Rows[e.RowIndex].FindControl("txtName");
        DropDownList ddlType = (DropDownList)GrdOrganisation.Rows[e.RowIndex].FindControl("ddlType");
        DropDownList ddlCustType = (DropDownList)GrdOrganisation.Rows[e.RowIndex].FindControl("ddlCustType");
        TextBox txtABN = (TextBox)GrdOrganisation.Rows[e.RowIndex].FindControl("txtABN");
        TextBox txtACN = (TextBox)GrdOrganisation.Rows[e.RowIndex].FindControl("txtACN");
        TextBox txtBPayAccount = (TextBox)GrdOrganisation.Rows[e.RowIndex].FindControl("txtBPayAccount");
        DropDownList ddlServiceCycle = (DropDownList)GrdOrganisation.Rows[e.RowIndex].FindControl("ddlServiceCycle");
        DropDownList ddlIsDebtor = (DropDownList)GrdOrganisation.Rows[e.RowIndex].FindControl("ddlIsDebtor");
        DropDownList ddlIsCreditor = (DropDownList)GrdOrganisation.Rows[e.RowIndex].FindControl("ddlIsCreditor");
        DropDownList ddlFreeServices = (DropDownList)GrdOrganisation.Rows[e.RowIndex].FindControl("ddlFreeServices");
        TextBox txtStartDate = (TextBox)GrdOrganisation.Rows[e.RowIndex].FindControl("txtStartDate");
        TextBox txtEndDate = (TextBox)GrdOrganisation.Rows[e.RowIndex].FindControl("txtEndDate");


        Organisation org = OrganisationDB.GetByID(Convert.ToInt32(lblId.Text));

        UrlParamType urlParamType = GetUrlParamType();
        int orgTypeID = Convert.ToInt32(ddlType.SelectedValue);
        if (urlParamType == UrlParamType.Clinic)
            orgTypeID = 218;

        DateTime startDate = GetDate(txtStartDate.Text.Trim());
        DateTime endDate = GetDate(txtEndDate.Text.Trim());

        if (urlParamType == UrlParamType.External)
        {

            if (urlParamType == UrlParamType.External && IsValidFormOrgTypeIDs() && GetFormOrgTypeIDs() == "191")
            {
                OrganisationDB.UpdateExtOrg(Convert.ToInt32(lblId.Text), Convert.ToInt32(GetFormOrgTypeIDs()), txtName.Text, txtACN.Text, txtABN.Text, DateTime.Now,
                                      org.IsDebtor, org.IsCreditor, org.BpayAccount, org.Comment);
            }
            else
            {
                OrganisationDB.Update(Convert.ToInt32(lblId.Text), Convert.ToInt32(ddlParent.SelectedValue), org.UseParentOffernigPrices, Convert.ToInt32(ddlType.SelectedValue), Convert.ToInt32(ddlCustType.SelectedValue), txtName.Text,

                                      org.Acn, org.Abn, org.IsDebtor, org.IsCreditor, org.BpayAccount, org.WeeksPerServiceCycle, org.StartDate, org.EndDate, org.Comment, org.FreeServices,
                    //txtACN.Text, txtABN.Text, 
                    //Convert.ToBoolean(ddlIsDebtor.SelectedValue), Convert.ToBoolean(ddlIsCreditor.SelectedValue), txtBPayAccount.Text, Convert.ToInt32(ddlServiceCycle.SelectedValue),
                    //org.StartDate, org.EndDate, org.Comment, Convert.ToInt32(ddlFreeServices.SelectedValue),

                                      org.ExclSun, org.ExclMon, org.ExclTue, org.ExclWed, org.ExclThu, org.ExclFri, org.ExclSat,
                                      org.SunStartTime, org.SunEndTime,
                                      org.MonStartTime, org.MonEndTime,
                                      org.TueStartTime, org.TueEndTime,
                                      org.WedStartTime, org.WedEndTime,
                                      org.ThuStartTime, org.ThuEndTime,
                                      org.FriStartTime, org.FriEndTime,
                                      org.SatStartTime, org.SatEndTime,
                                      org.SunLunchStartTime, org.SunLunchEndTime,
                                      org.MonLunchStartTime, org.MonLunchEndTime,
                                      org.TueLunchStartTime, org.TueLunchEndTime,
                                      org.WedLunchStartTime, org.WedLunchEndTime,
                                      org.ThuLunchStartTime, org.ThuLunchEndTime,
                                      org.FriLunchStartTime, org.FriLunchEndTime,
                                      org.SatLunchStartTime, org.SatLunchEndTime,
                                      org.LastBatchRun);
            }
        }
        else
        {
            OrganisationDB.Update(Convert.ToInt32(lblId.Text), Convert.ToInt32(ddlParent.SelectedValue), Convert.ToBoolean(ddlUseParentOffernigPrices.SelectedValue), Convert.ToInt32(ddlType.SelectedValue), Convert.ToInt32(ddlCustType.SelectedValue), txtName.Text, txtACN.Text, txtABN.Text,
                                  org.IsDebtor, org.IsCreditor, txtBPayAccount.Text, Convert.ToInt32(ddlServiceCycle.SelectedValue),
                                  startDate, endDate, org.Comment, Convert.ToInt32(ddlFreeServices.SelectedValue),
                                  org.ExclSun, org.ExclMon, org.ExclTue, org.ExclWed, org.ExclThu, org.ExclFri, org.ExclSat,
                                  org.SunStartTime, org.SunEndTime,
                                  org.MonStartTime, org.MonEndTime,
                                  org.TueStartTime, org.TueEndTime,
                                  org.WedStartTime, org.WedEndTime,
                                  org.ThuStartTime, org.ThuEndTime,
                                  org.FriStartTime, org.FriEndTime,
                                  org.SatStartTime, org.SatEndTime,
                                  org.SunLunchStartTime, org.SunLunchEndTime,
                                  org.MonLunchStartTime, org.MonLunchEndTime,
                                  org.TueLunchStartTime, org.TueLunchEndTime,
                                  org.WedLunchStartTime, org.WedLunchEndTime,
                                  org.ThuLunchStartTime, org.ThuLunchEndTime,
                                  org.FriLunchStartTime, org.FriLunchEndTime,
                                  org.SatLunchStartTime, org.SatLunchEndTime, org.LastBatchRun);
        }


        GrdOrganisation.EditIndex = -1;
        FillGrid();
    }
    protected void GrdOrganisation_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdOrganisation.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            OrganisationDB.UpdateInactive(Convert.ToInt32(lblId.Text));
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
    protected void GrdOrganisation_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlParent = (DropDownList)GrdOrganisation.FooterRow.FindControl("ddlNewParent");
            DropDownList ddlUseParentOffernigPrices = (DropDownList)GrdOrganisation.FooterRow.FindControl("ddlUseParentOffernigPrices");
            TextBox txtName = (TextBox)GrdOrganisation.FooterRow.FindControl("txtNewName");
            DropDownList ddlType = (DropDownList)GrdOrganisation.FooterRow.FindControl("ddlNewType");
            DropDownList ddlCustType = (DropDownList)GrdOrganisation.FooterRow.FindControl("ddlNewCustType");
            TextBox txtABN = (TextBox)GrdOrganisation.FooterRow.FindControl("txtNewABN");
            TextBox txtACN = (TextBox)GrdOrganisation.FooterRow.FindControl("txtNewACN");
            TextBox txtBPayAccount = (TextBox)GrdOrganisation.FooterRow.FindControl("txtNewBPayAccount");
            DropDownList ddlServiceCycle = (DropDownList)GrdOrganisation.FooterRow.FindControl("ddlNewServiceCycle");
            DropDownList ddlIsDebtor = (DropDownList)GrdOrganisation.FooterRow.FindControl("ddlNewIsDebtor");
            DropDownList ddlIsCreditor = (DropDownList)GrdOrganisation.FooterRow.FindControl("ddlNewIsCreditor");
            DropDownList ddlFreeServices = (DropDownList)GrdOrganisation.FooterRow.FindControl("ddlNewFreeServices");

            TimeSpan defaultDayStartTime = new TimeSpan(8, 0, 0);
            TimeSpan defaultDayEndTime = new TimeSpan(20, 0, 0);
            TimeSpan defaultDayLunchStartTime = new TimeSpan(12, 0, 0);
            TimeSpan defaultDayLunchEndTime = new TimeSpan(12, 0, 0);

            UrlParamType urlParamType = GetUrlParamType();
            if (urlParamType == UrlParamType.External)
            {

                if (urlParamType == UrlParamType.External && IsValidFormOrgTypeIDs() && GetFormOrgTypeIDs() == "191")
                {
                    OrganisationDB.InsertExtOrg(Convert.ToInt32(GetFormOrgTypeIDs()), txtName.Text, txtACN.Text, txtABN.Text,
                                          OrganisationTypeDB.IsDebtor(191), OrganisationTypeDB.IsCreditor(191), "", "");
                }
                else
                {
                    OrganisationDB.Insert(Convert.ToInt32(ddlParent.SelectedValue), false, Convert.ToInt32(ddlType.SelectedValue), Convert.ToInt32(ddlCustType.SelectedValue), txtName.Text, txtACN.Text, txtABN.Text,
                                          OrganisationTypeDB.IsDebtor(Convert.ToInt32(ddlType.SelectedValue)), OrganisationTypeDB.IsCreditor(Convert.ToInt32(ddlType.SelectedValue)),

                                          "", 0, DateTime.MinValue, DateTime.MinValue, "", 0,
                        //txtBPayAccount.Text, Convert.ToInt32(ddlServiceCycle.SelectedValue),
                        //DateTime.MinValue DateTime.MinValue, "", Convert.ToInt32(ddlFreeServices.SelectedValue),

                                          false, false, false, false, false, false, false,
                                          defaultDayStartTime, defaultDayEndTime,
                                          defaultDayStartTime, defaultDayEndTime,
                                          defaultDayStartTime, defaultDayEndTime,
                                          defaultDayStartTime, defaultDayEndTime,
                                          defaultDayStartTime, defaultDayEndTime,
                                          defaultDayStartTime, defaultDayEndTime,
                                          defaultDayStartTime, defaultDayEndTime,
                                          defaultDayLunchStartTime, defaultDayLunchEndTime,
                                          defaultDayLunchStartTime, defaultDayLunchEndTime,
                                          defaultDayLunchStartTime, defaultDayLunchEndTime,
                                          defaultDayLunchStartTime, defaultDayLunchEndTime,
                                          defaultDayLunchStartTime, defaultDayLunchEndTime,
                                          defaultDayLunchStartTime, defaultDayLunchEndTime,
                                          defaultDayLunchStartTime, defaultDayLunchEndTime,
                                          DateTime.MinValue);
                }

            }
            else
            {
                OrganisationDB.Insert(Convert.ToInt32(ddlParent.SelectedValue), Convert.ToBoolean(ddlUseParentOffernigPrices.SelectedValue), Convert.ToInt32(ddlType.SelectedValue), Convert.ToInt32(ddlCustType.SelectedValue), txtName.Text, txtACN.Text, txtABN.Text,
                                      OrganisationTypeDB.IsDebtor(Convert.ToInt32(ddlType.SelectedValue)), OrganisationTypeDB.IsCreditor(Convert.ToInt32(ddlType.SelectedValue)),
                                      txtBPayAccount.Text, Convert.ToInt32(ddlServiceCycle.SelectedValue),
                                      DateTime.MinValue, DateTime.MinValue, "", Convert.ToInt32(ddlFreeServices.SelectedValue),
                                      true, false, false, false, false, false, true,
                                      defaultDayStartTime, defaultDayEndTime,
                                      defaultDayStartTime, defaultDayEndTime,
                                      defaultDayStartTime, defaultDayEndTime,
                                      defaultDayStartTime, defaultDayEndTime,
                                      defaultDayStartTime, defaultDayEndTime,
                                      defaultDayStartTime, defaultDayEndTime,
                                      defaultDayStartTime, defaultDayEndTime,
                                      defaultDayLunchStartTime, defaultDayLunchEndTime,
                                      defaultDayLunchStartTime, defaultDayLunchEndTime,
                                      defaultDayLunchStartTime, defaultDayLunchEndTime,
                                      defaultDayLunchStartTime, defaultDayLunchEndTime,
                                      defaultDayLunchStartTime, defaultDayLunchEndTime,
                                      defaultDayLunchStartTime, defaultDayLunchEndTime,
                                      defaultDayLunchStartTime, defaultDayLunchEndTime,
                                      DateTime.MinValue);
            }

            FillGrid();
        }

        if (e.CommandName.Equals("_Delete") || e.CommandName.Equals("_UnDelete"))
        {
            int organisation_id = Convert.ToInt32(e.CommandArgument);

            try
            {
                if (e.CommandName.Equals("_Delete"))
                    OrganisationDB.UpdateInactive(organisation_id);
                else
                    OrganisationDB.UpdateActive(organisation_id);
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

    }
    protected void GrdOrganisation_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdOrganisation.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdOrganisation.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }
    protected void GrdOrganisation_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdOrganisation.PageIndex = e.NewPageIndex;
        FillGrid();
    }
    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["organisationinfo_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["organisationinfo_sortexpression"] == null)
                Session["organisationinfo_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["organisationinfo_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["organisationinfo_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdOrganisation.DataSource = dataView;
            GrdOrganisation.DataBind();
        }
    }

    #endregion

    #region ValidDateCheck, GetDate, IsValidDate

    protected void ValidStartDateCheck(object sender, ServerValidateEventArgs e)
    {
        try
        {
            CustomValidator cv = (CustomValidator)sender;
            GridViewRow grdRow = ((GridViewRow)cv.Parent.Parent);
            TextBox txtStartDate = grdRow.RowType == DataControlRowType.Footer ? (TextBox)grdRow.FindControl("txtNewStartDate") : (TextBox)grdRow.FindControl("txtStartDate");

            if (!IsValidDate(txtStartDate.Text))
                throw new Exception();

            DateTime d = GetDate(txtStartDate.Text);
            e.IsValid = (d == DateTime.MinValue) || Utilities.IsValidDBDateTime(d);
        }
        catch (Exception)
        {
            e.IsValid = false;
        }
    }
    protected void ValidEndDateCheck(object sender, ServerValidateEventArgs e)
    {
        try
        {
            CustomValidator cv = (CustomValidator)sender;
            GridViewRow grdRow = ((GridViewRow)cv.Parent.Parent);
            TextBox txtEndDate = grdRow.RowType == DataControlRowType.Footer ? (TextBox)grdRow.FindControl("txtNewEndDate") : (TextBox)grdRow.FindControl("txtEndDate");

            if (!IsValidDate(txtEndDate.Text))
                throw new Exception();

            DateTime d = GetDate(txtEndDate.Text);
            e.IsValid = (d == DateTime.MinValue) || Utilities.IsValidDBDateTime(d);
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
        return inDate.Length == 0 || Regex.IsMatch(inDate, @"^\d{2}\-\d{2}\-\d{4}$");
    }

    #endregion


    #region btnSearchName_Click, btnClearNameSearch_Click

    protected void btnSearchName_Click(object sender, EventArgs e)
    {
        if (!Regex.IsMatch(txtSearchName.Text, @"^[0-9a-zA-Z\[\]\(\)\-\']*$"))
        {
            SetErrorMessage("Search text can only be letters, numbers, brackets, and hyphens");
            return;
        }
        else if (txtSearchName.Text.Trim().Length == 0)
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
        this.GrdOrganisation.AllowPaging = chkUsePaging.Checked;
        FillGrid();
    }
    protected void chkShowDeleted_CheckedChanged(object sender, EventArgs e)
    {
        FillGrid();
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