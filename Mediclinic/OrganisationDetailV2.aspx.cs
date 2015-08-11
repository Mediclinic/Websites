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

public partial class StaffDetailV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);
                SetUpGUI();

                Organisation org = null;
                if ((GetUrlParamType() == UrlParamType.Edit || GetUrlParamType() == UrlParamType.View) && IsValidFormID())
                {
                    org = OrganisationDB.GetByID(GetFormID());
                    if (org != null)
                    {
                        UserView userView = UserView.GetInstance();

                        if (org.OrganisationType.OrganisationTypeID == 218 && userView.IsAgedCareView)
                            throw new CustomMessageException("This is a Clinic but you are logged into Aged Care. Change site to view/edit this Clinic.");
                        if (org.OrganisationType.OrganisationTypeID == 139 && !userView.IsAgedCareView)
                            throw new CustomMessageException("This is an Aged Care Facility but you are not logged into the Aged Care system. Change site to view/edit this Facility.");
                        if (org.OrganisationType.OrganisationTypeID == 367 && !userView.IsAgedCareView)
                            throw new CustomMessageException("This is an Aged Care Facility but you are not logged into the Aged Care system. Change site to view/edit this Facility.");
                        if (org.OrganisationType.OrganisationTypeID == 372 && !userView.IsAgedCareView)
                            throw new CustomMessageException("This is an Aged Care Facility but you are not logged into the Aged Care system. Change site to view/edit this Facility.");

                        FillEditViewForm(org, GetUrlParamType() == UrlParamType.Edit);

                        if (org.IsAgedCare)
                            lblHeading.InnerHtml = "Facility Information";
                        else if (org.IsClinic)
                            lblHeading.InnerHtml = "Clinic Information";
                        else if (org.OrganisationType.OrganisationTypeID == 150)
                        {
                            lblHeading.InnerHtml = "Insurance Company Information";

                            tbl_detailtable_organisation.Attributes["class"] = "detailtable_organisation_insurance";
                            //tbl_detailtable_organisation.RemoveCssClass("detailtable_organisation");
                            //tbl_detailtable_organisation.AddCssClass("detailtable_organisation_insurance");
                        }
                        else if (org.OrganisationID == -2)
                            lblHeading.InnerHtml = "DVA Org Information";
                        else if (org.OrganisationID == -2)
                            lblHeading.InnerHtml = "Medicare Org Information";

                        if (IsValidFormOrgTypeIDs() && GetFormOrgTypeIDs() == "191")
                        {
                            lblHeading.InnerHtml = "Medical Org Information";

                            td_staff_list.Visible = false;
                            td_staff_list_space.Visible = false;
                            td_staff_heading_list.Visible = false;
                            td_staff_heading_list_space.Visible = false;
                            td_patients_list.Visible = false;
                            td_patients_heading_list.Visible = false;

                        }
                        else
                        {
                            td_referrers_list.Visible = false;
                            td_referrers_list_space.Visible = false;
                            td_referrers_heading_list.Visible = false;
                            td_referrers_heading_list_space.Visible = false;
                        }

                        if (Utilities.GetAddressType().ToString() == "Contact")
                        {
                            addressControl.Visible = true;
                            addressControl.Set(org.EntityID, true, EntityType.GetByType(EntityType.EntityTypeEnum.Organisation));
                        }
                        else if (Utilities.GetAddressType().ToString() == "ContactAus")
                        {
                            addressAusControl.Visible = true;
                            addressAusControl.Set(org.EntityID, true, EntityType.GetByType(EntityType.EntityTypeEnum.Organisation));
                        }
                        else
                            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
                    }
                    else
                        HideTableAndSetErrorMessage();

                }
                else if (GetUrlParamType() == UrlParamType.Add)
                {
                    FillEmptyAddForm();

                    UrlParamOrgType orgType = GetUrlParamOrgType();
                    if (orgType == UrlParamOrgType.AgedCare)
                    {
                        lblHeading.InnerHtml = "Add Facility";
                        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + "Add Facility";
                    }
                    else if (orgType == UrlParamOrgType.Clinic)
                    {
                        lblHeading.InnerHtml = "Add Clinic";
                        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + "Add Clinic";
                    }
                    else if (orgType == UrlParamOrgType.Insurance)
                    {
                        lblHeading.InnerHtml = "Add Insurance Company";
                        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + "Add Insurance Company";

                        tbl_detailtable_organisation.Attributes["class"] = "detailtable_organisation_insurance";
                        //tbl_detailtable_organisation.RemoveCssClass("detailtable_organisation");
                        //tbl_detailtable_organisation.AddCssClass("detailtable_organisation_insurance");
                    }
                    else if (IsValidFormOrgTypeIDs() && GetFormOrgTypeIDs() == "191")
                    {
                        lblHeading.InnerHtml = "Add Medical Organisation";
                        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + "Add Medical Organisation";
                    }
                    else if (orgType == UrlParamOrgType.External)
                    {
                        lblHeading.InnerHtml = "Add External Organisation";
                        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + "Add External Organisation";
                    }
                }
                else
                    HideTableAndSetErrorMessage();
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

    #region GetUrlParamCard(), GetUrlParamType(), IsValidFormID(), GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\-?\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new Exception("Invalid url id");

        string id = Request.QueryString["id"];
        return Convert.ToInt32(id);
    }

    private enum UrlParamType { Add, Edit, View, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "add")
            return UrlParamType.Add;
        else if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "view")
            return UrlParamType.View;
        else
            return UrlParamType.None;
    }

    private enum UrlParamOrgType { Clinic, AgedCare, External, Insurance, None };
    private UrlParamOrgType GetUrlParamOrgType()
    {
        string type = Request.QueryString["orgtype"];
        if (type != null && type.ToLower() == "clinic")
            return UrlParamOrgType.Clinic;
        else if (type != null && type.ToLower() == "ac")
            return UrlParamOrgType.AgedCare;
        else if (type != null && type.ToLower() == "ext")
            return UrlParamOrgType.External;
        else if (type != null && type.ToLower() == "ins")
            return UrlParamOrgType.Insurance;
        else if (IsValidFormID())
        {
            Organisation thisOrg = OrganisationDB.GetByID(GetFormID());

            if (thisOrg.OrganisationType.OrganisationTypeGroup.ID == 5)
                return UrlParamOrgType.Clinic;
            else if (thisOrg.OrganisationType.OrganisationTypeGroup.ID == 6)
                return UrlParamOrgType.AgedCare;
            else if (thisOrg.OrganisationType.OrganisationTypeGroup.ID == 4)
                return UrlParamOrgType.External;
            else if (thisOrg.OrganisationType.OrganisationTypeGroup.ID == 7)
                return UrlParamOrgType.Insurance;
            else
                return UrlParamOrgType.None;
        }
        else
            return UrlParamOrgType.None;
    }

    private bool IsValidFormOrgTypeIDs()
    {
        string org_type_ids = Request.QueryString["org_type_ids"];
        return org_type_ids != null;
    }
    private string GetFormOrgTypeIDs()
    {
        if (!IsValidFormOrgTypeIDs())
            throw new Exception("Invalid org type id");

        string org_type_ids = Request.QueryString["org_type_ids"];
        return org_type_ids;
    }

    #endregion

    protected void SetUpGUI()
    {
        lnkBooking.Visible = GetUrlParamType() == UrlParamType.Edit;

        int startYear = -1;
        int endYear   = -1;
        if ((GetUrlParamType() == UrlParamType.Edit || GetUrlParamType() == UrlParamType.View) && IsValidFormID())
        {
            Organisation org = OrganisationDB.GetByID(GetFormID());
            if (org != null)
            {
                if (org.StartDate != DateTime.MinValue)
                    startYear = org.StartDate.Year;
                if (org.EndDate != DateTime.MinValue)
                    endYear = org.EndDate.Year;
            }
        }

        ddlEndDate_Day.Items.Add(new ListItem("--", "-1"));
        ddlEndDate_Month.Items.Add(new ListItem("--", "-1"));
        ddlEndDate_Year.Items.Add(new ListItem("----", "-1"));

        ddlStartDate_Day.Items.Add(new ListItem("--", "-1"));
        ddlStartDate_Month.Items.Add(new ListItem("--", "-1"));
        ddlStartDate_Year.Items.Add(new ListItem("----", "-1"));

        for (int i = 1; i <= 31; i++)
        {
            ddlStartDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlEndDate_Day.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        for (int i = 1; i <= 12; i++)
        {
            ddlStartDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlEndDate_Month.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }


        int ddlFirstYear = 2000;
        int ddlLastYear  = DateTime.Today.Year + 5;

        if (startYear != -1 && startYear < ddlFirstYear)
            ddlStartDate_Year.Items.Add(new ListItem(startYear.ToString(), startYear.ToString()));
        if (endYear   != -1 && endYear   < ddlFirstYear)
            ddlEndDate_Year.Items.Add(new ListItem(endYear.ToString(), endYear.ToString()));
        for (int i = ddlFirstYear; i <= ddlLastYear; i++)
        {
            ddlStartDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlEndDate_Year.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        if (startYear != -1 && startYear >= ddlLastYear)
            ddlStartDate_Year.Items.Add(new ListItem(startYear.ToString(), startYear.ToString()));
        if (endYear   != -1 && endYear   >= ddlLastYear)
            ddlEndDate_Year.Items.Add(new ListItem(endYear.ToString(), endYear.ToString()));


        UrlParamOrgType urlParamOrgType = GetUrlParamOrgType();

        if (IsValidFormOrgTypeIDs() && GetFormOrgTypeIDs() == "191") // hide stuff for medical practice 
        {
            parentRow.Visible          = false;
            useParentOfferingPricesRow.Visible = false;
            typeRow.Visible            = false;
            customerTypeRow.Visible    = false;
            bpayRow.Visible            = false;
            serviceCycleRow.Visible    = false;
            numFreeServicesRow.Visible = false;
            dateAddedRow.Visible       = false;
            startDateRow.Visible       = false;
            endDateRow.Visible         = false;
            lastBatchRunRow.Visible    = false;
            workingDaysRow.Visible     = false;
        }
        if (IsValidFormID() && OrganisationDB.GetByID(GetFormID()).OrganisationType.OrganisationTypeGroup.ID == 3) // hide stuff for DVA/MC
        {
            parentRow.Visible          = false;
            useParentOfferingPricesRow.Visible = false;
            typeRow.Visible            = false;
            customerTypeRow.Visible    = false;
            bpayRow.Visible            = false;
            serviceCycleRow.Visible    = false;
            numFreeServicesRow.Visible = false;
            dateAddedRow.Visible       = false;
            startDateRow.Visible       = false;
            endDateRow.Visible         = false;
            lastBatchRunRow.Visible    = false;
            workingDaysRow.Visible     = false;

            abnRow.Visible             = false;
            acnRow.Visible             = false;
            commentsRow.Visible        = false;
            beforeContactSpac1.Visible = false;
            beforeContactSpac2.Visible = false;
            beforeContactSpac3.Visible = false;
            afterContactSpac1.Visible  = false;
            afterContactSpac2.Visible  = false;
            afterContactSpac3.Visible  = false;
            spnBookingsAndLettersLinks.Visible = false;
            existingOrgInfoSpace.Visible       = false;
            existingOrgInfoSpaceShort.Visible  = false;
            tblRegisteredEntitiesList.Visible  = false;
        }

        if (urlParamOrgType == UrlParamOrgType.Insurance)
        {
            parentRow.Visible                   = false;
            useParentOfferingPricesRow.Visible  = false;
            typeRow.Visible                     = false;
            customerTypeRow.Visible             = false;
            bpayRow.Visible                     = false;
            serviceCycleRow.Visible             = false;
            numFreeServicesRow.Visible          = false;
            dateAddedRow.Visible                = false;
            startDateRow.Visible                = false;
            endDateRow.Visible                  = false;
            lastBatchRunRow.Visible             = false;
            workingDaysRow.Visible              = false;

            abnRow.Visible                      = false;
            acnRow.Visible                      = false;
            commentsRow.Visible                 = false;
            beforeContactSpac1.Visible          = false;
            beforeContactSpac2.Visible          = false;
            beforeContactSpac3.Visible          = false;
            afterContactSpac1.Visible           = false;
            afterContactSpac2.Visible           = false;
            afterContactSpac3.Visible           = false;
            spnBookingsAndLettersLinks.Visible  = false;
            existingOrgInfoSpace.Visible        = false;
            existingOrgInfoSpaceShort.Visible   = false;
            tblRegisteredEntitiesList.Visible   = false;
        }




        bool editable = GetUrlParamType() == UrlParamType.Add || GetUrlParamType() == UrlParamType.Edit;

        Utilities.SetEditControlBackColour(txtName,                    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlParent,                  editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlUseParentOffernigPrices, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlType,                    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlCustType,                editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtABN,                     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtACN,                     editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtBPayAccount,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlServiceCycle,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlFreeServices,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlStartDate_Day,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlStartDate_Month,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlStartDate_Year,          editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlEndDate_Day,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlEndDate_Month,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlEndDate_Year,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtComments,                editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        Utilities.SetEditControlBackColour(chkIncSunday,               editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(chkIncMonday,               editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(chkIncTuesday,              editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(chkIncWednesday,            editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(chkIncThursday,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(chkIncFriday,               editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(chkIncSaturday,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        Utilities.SetEditControlBackColour(ddlSunStart_Hour,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSunEnd_Hour,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlMonStart_Hour,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlMonEnd_Hour,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlTueStart_Hour,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlTueEnd_Hour,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlWedStart_Hour,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlWedEnd_Hour,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlThuStart_Hour,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlThuEnd_Hour,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlFriStart_Hour,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlFriEnd_Hour,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSatStart_Hour,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSatEnd_Hour,             editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        Utilities.SetEditControlBackColour(ddlSunStart_Minute,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSunEnd_Minute,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlMonStart_Minute,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlMonEnd_Minute,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlTueStart_Minute,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlTueEnd_Minute,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlWedStart_Minute,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlWedEnd_Minute,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlThuStart_Minute,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlThuEnd_Minute,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlFriStart_Minute,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlFriEnd_Minute,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSatStart_Minute,         editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSatEnd_Minute,           editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        Utilities.SetEditControlBackColour(ddlSunLunchStart_Hour,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSunLunchEnd_Hour,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlMonLunchStart_Hour,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlMonLunchEnd_Hour,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlTueLunchStart_Hour,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlTueLunchEnd_Hour,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlWedLunchStart_Hour,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlWedLunchEnd_Hour,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlThuLunchStart_Hour,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlThuLunchEnd_Hour,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlFriLunchStart_Hour,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlFriLunchEnd_Hour,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSatLunchStart_Hour,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSatLunchEnd_Hour,        editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);

        Utilities.SetEditControlBackColour(ddlSunLunchStart_Minute,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSunLunchEnd_Minute,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlMonLunchStart_Minute,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlMonLunchEnd_Minute,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlTueLunchStart_Minute,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlTueLunchEnd_Minute,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlWedLunchStart_Minute,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlWedLunchEnd_Minute,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlThuLunchStart_Minute,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlThuLunchEnd_Minute,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlFriLunchStart_Minute,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlFriLunchEnd_Minute,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSatLunchStart_Minute,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlSatLunchEnd_Minute,      editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }

    private void FillDropDowns()
    {

        if (GetUrlParamType() == UrlParamType.Add)
        {
            bool exclClinics      = GetUrlParamOrgType() != UrlParamOrgType.None && GetUrlParamOrgType() != UrlParamOrgType.Clinic;
            bool exclAgedCareFacs = GetUrlParamOrgType() != UrlParamOrgType.None && GetUrlParamOrgType() != UrlParamOrgType.AgedCare;
            bool exclExternal     = GetUrlParamOrgType() != UrlParamOrgType.None && GetUrlParamOrgType() != UrlParamOrgType.External;
            bool exclIns          = GetUrlParamOrgType() != UrlParamOrgType.None && GetUrlParamOrgType() != UrlParamOrgType.Insurance;

            DataTable parentList = OrganisationDB.GetDataTable(0, false, true, exclClinics, exclAgedCareFacs, exclIns, exclExternal);
            ddlParent.Items.Add(new ListItem("--", "0"));
            foreach (DataRow row in parentList.Rows)
                ddlParent.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
        }
        else if (GetUrlParamType() == UrlParamType.Edit)
        {
            Organisation[] thisOrg = new Organisation[] { OrganisationDB.GetByID(GetFormID()) };

            bool exclClinics      = thisOrg[0].OrganisationType.OrganisationTypeGroup.ID != 5;   // GetUrlParamOrgType() != UrlParamOrgType.None && GetUrlParamOrgType() != UrlParamOrgType.Clinic;
            bool exclAgedCareFacs = thisOrg[0].OrganisationType.OrganisationTypeGroup.ID != 6;   // GetUrlParamOrgType() != UrlParamOrgType.None && GetUrlParamOrgType() != UrlParamOrgType.AgedCare;
            bool exclExternal     = thisOrg[0].OrganisationType.OrganisationTypeGroup.ID != 4;   // GetUrlParamOrgType() != UrlParamOrgType.None && GetUrlParamOrgType() != UrlParamOrgType.External;
            bool exclIns          = thisOrg[0].OrganisationType.OrganisationTypeGroup.ID != 7;   // GetUrlParamOrgType() != UrlParamOrgType.None && GetUrlParamOrgType() != UrlParamOrgType.Insurance;

            DataTable parentList = OrganisationDB.GetDataTable_AllNotInc(thisOrg, false, exclClinics, exclAgedCareFacs, exclIns, exclExternal);
            ddlParent.Items.Add(new ListItem("--", "0"));
            foreach (DataRow row in parentList.Rows)
                ddlParent.Items.Add(new ListItem(row["name"].ToString(), row["organisation_id"].ToString()));
        }



        DataTable types = null;
        switch (GetUrlParamOrgType())
        {
            case UrlParamOrgType.Clinic:
                types = OrganisationTypeDB.GetDataTable_Clinics();
                break;
            case UrlParamOrgType.AgedCare:
                types = OrganisationTypeDB.GetDataTable_AgedCareFacs();
                break;
            case UrlParamOrgType.Insurance:
                types = OrganisationTypeDB.GetDataTable_InsuranceOrgs();
                break;
            case UrlParamOrgType.External:
                if (IsValidFormOrgTypeIDs())
                    types = OrganisationTypeDB.GetDataTable_External(GetFormOrgTypeIDs().ToString());
                else
                    types = OrganisationTypeDB.GetDataTable_External();
                break;
            default:
                types = UserView.GetInstance().IsClinicView ? OrganisationTypeDB.GetDataTable_Clinics() : OrganisationTypeDB.GetDataTable_AgedCareFacs();
                break;
        }
        foreach (DataRow row in types.Rows)
        {
            string prefix = Convert.ToInt32(row["organisation_type_group_id"]) == 4 ? "EXT. " : string.Empty;
            ddlType.Items.Add(new ListItem(prefix + row["descr"].ToString(), row["organisation_type_id"].ToString()));
        }


        DataTable custTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "OrganisationCustomerType", "organisation_customer_type_id NOT IN (139,275)", "descr", "organisation_customer_type_id", "descr");

        // move None row to first position - all else leave as alphabetically sorted
        for (int i = custTypes.Rows.Count - 1; i >= 0; i--)
        {
            if (Convert.ToInt32(custTypes.Rows[i]["organisation_customer_type_id"]) == 0)
            {
                DataRow newRow = custTypes.NewRow();
                newRow.ItemArray = custTypes.Rows[i].ItemArray;
                custTypes.Rows.RemoveAt(i);
                custTypes.Rows.InsertAt(newRow, 0);
                break;
            }
        }

        ddlCustType.DataSource = custTypes;
        ddlCustType.DataTextField = "descr";
        ddlCustType.DataValueField = "organisation_customer_type_id";
        ddlCustType.DataBind();


        for (int i = 0; i <= 52; i++)
            ddlServiceCycle.Items.Add(new ListItem(i.ToString(), i.ToString()));
        for (int i = 0; i <= 5; i++)
            ddlFreeServices.Items.Add(new ListItem(i.ToString(), i.ToString()));

        for (int i = 0; i < 24; i++)
        {
            ddlSunStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSunEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlMonStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlMonEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlTueStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlTueEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlWedStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlWedEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlThuStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlThuEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlFriStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlFriEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSatStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSatEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));

            ddlSunLunchStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSunLunchEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlMonLunchStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlMonLunchEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlTueLunchStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlTueLunchEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlWedLunchStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlWedLunchEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlThuLunchStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlThuLunchEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlFriLunchStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlFriLunchEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSatLunchStart_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSatLunchEnd_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
        }
        for (int i = 0; i < 60; i += 10)
        {
            ddlSunStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSunEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlMonStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlMonEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlTueStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlTueEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlWedStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlWedEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlThuStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlThuEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlFriStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlFriEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSatStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSatEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));

            ddlSunLunchStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSunLunchEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlMonLunchStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlMonLunchEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlTueLunchStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlTueLunchEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlWedLunchStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlWedLunchEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlThuLunchStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlThuLunchEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlFriLunchStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlFriLunchEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSatLunchStart_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
            ddlSatLunchEnd_Minute.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString()));
        }

    }

    private void FillEditViewForm(Organisation org, bool isEditMode)
    {
        Page.Title = ((SystemVariables)Session["SystemVariables"])["Site"].Value + " - " + org.Name;

        if (!Utilities.IsDev())
            idRow.Attributes["class"] = "hiddencol";

        existingOrgInfoSpace.Visible      = org.OrganisationType.OrganisationTypeID != 150;
        existingOrgInfoSpaceShort.Visible = false;

        string screen_id;
        if (org.OrganisationType.OrganisationTypeGroup.ID == 4)
            screen_id = "10";
        else if (org.OrganisationType.OrganisationTypeGroup.ID == 5)
            screen_id = "12";  // clinic
        else if (org.OrganisationType.OrganisationTypeGroup.ID == 6)
            screen_id = "14";  // aged care
        else if (org.OrganisationType.OrganisationTypeGroup.ID == 7)  // Ins
            screen_id = null;  // aged care
        else
            throw new CustomMessageException();

        string allFeatures = screen_id == null ? string.Empty : "dialogWidth:980px;dialogHeight:530px;center:yes;resizable:no; scroll:no";
        string js          = screen_id == null ? string.Empty : "javascript:window.showModalDialog('" + "NoteListV2.aspx?id=" + org.EntityID.ToString() + "&screen=" + screen_id + "', '', '" + allFeatures + "');document.getElementById('btnUpdateNotesIcon').click();return false;";
//        this.lnkNotes.Attributes.Add("onclick", js);
//        lnkNotes.ImageUrl = NoteDB.HasNotes(org.EntityID) ? "~/images/notes-48.png" : "~/images/notes-bw-48.jpg";


        if (org.OrganisationType.OrganisationTypeID == 367 || org.OrganisationType.OrganisationTypeID == 372 || org.OrganisationType.OrganisationTypeID == 139)
        {
            lnkBooking.NavigateUrl = String.Format("~/BookingsV2.aspx?orgs={0}", org.OrganisationID);
            lnkBooking.Text = "Booking Sheet";
            this.lnkBooking.Visible = true;

            this.lnkBookingList.Visible = true;
            this.lnkBookingList.NavigateUrl = "~/BookingsListV2.aspx?org=" + org.OrganisationID;
            this.lnkBookingList.Text = "Bookings List";

            /*
            this.lblOrgStructure.Visible = true;
            this.td_vertical_line_org_structure.Visible = true;



            //RegisterPatient[] regPts = RegisterPatientDB.GetAll(true, true, true, "6,2,3");
            RegisterPatient[] regPts = RegisterPatientDB.GetAll(false, false, true, "6,2,3");
            Hashtable patHash = new Hashtable();
            Hashtable orgHash = new Hashtable();

            for (int i = 0; i < regPts.Length; i++)
            {
                if (patHash[regPts[i].Patient.PatientID] == null)
                    patHash[regPts[i].Patient.PatientID] = new ArrayList();
                if (orgHash[regPts[i].Organisation.OrganisationID] == null)
                    orgHash[regPts[i].Organisation.OrganisationID] = new ArrayList();

                ((ArrayList)patHash[regPts[i].Patient.PatientID]).Add(regPts[i]);
                ((ArrayList)orgHash[regPts[i].Organisation.OrganisationID]).Add(regPts[i]);
            }

            string output = string.Empty;
            Organisation[] flattenedTree = OrganisationTree.GetFlattenedTree(null, org.IsDeleted, org.OrganisationID, false, "139,367,372");
            for (int i = 0; i < flattenedTree.Length; i++)
            {
                Organisation curOrg = flattenedTree[i];
                int nPatients = orgHash[curOrg.OrganisationID] == null ? 0 : ((ArrayList)orgHash[curOrg.OrganisationID]).Count;

                for (int j = 0; j < curOrg.TreeLevel; j++)
                    output += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                output += "・ <a href='/OrganisationDetailV2.aspx?type=view&id=" + curOrg.OrganisationID + "'>" + curOrg.Name + "</a>" + " (" + curOrg.OrganisationType.Descr.Replace("Aged Care ", "") + " " + nPatients + " PTs)<br />";
            }
            regPts  = null;
            patHash = null;
            orgHash = null;
            GC.Collect();

            this.lblOrgStructure.Text = output;
            */

        }
        else if (org.OrganisationType.OrganisationTypeID == 218)
        {
            typeRow.Attributes["class"] = "hiddencol";  // only one type, so hide.  But dont make invis (ie remove from page) - need for updating/inserting records

            lnkBooking.NavigateUrl = String.Format("~/BookingsV2.aspx?orgs={0}", org.OrganisationID);
            lnkBooking.Text = "Booking Sheet";
            this.lnkBooking.Visible = true;
//            this.lblOrgStructure.Visible = false;
//            this.td_vertical_line_org_structure.Visible = false;

            this.lnkBookingList.Visible = true;
            this.lnkBookingList.NavigateUrl = "~/BookingsListV2.aspx?org=" + org.OrganisationID;
            this.lnkBookingList.Text = "Bookings List";
        }
        else
        {
            lnkBooking.NavigateUrl = "";
            lnkBooking.Text = "";
            this.lnkBooking.Visible = false;
//            this.lblOrgStructure.Visible = false;
//            this.td_vertical_line_org_structure.Visible = false;
        }


        allFeatures = "dialogWidth:1250px;dialogHeight:835px;center:yes;resizable:no; scroll:no";
        js = "javascript:window.showModalDialog('" + "BookingSheetBlockoutV2.aspx?org=" + org.OrganisationID.ToString() + "', '', '" + allFeatures + "');return false;";
        this.lnkUnavailabilities.Attributes.Add("onclick", js);
        this.lnkUnavailabilities.NavigateUrl = "javascript:void(0)";


        this.lnkInvoices.Text = "Invoices";
        this.lnkInvoices.NavigateUrl = "~/InvoiceListV2.aspx?orgs=" + Request.QueryString["id"].ToString() + "&start_date=" + DateTime.Now.AddMonths(-1).ToString("yyyy_MM_dd") + "&end_date=" + DateTime.Now.ToString("yyyy_MM_dd");



        this.lnkLetters.NavigateUrl            = String.Format("~/Letters_MaintainV2.aspx?org={0}",    org.OrganisationID);
        this.lnkLetterPrintHistory.NavigateUrl = String.Format("~/Letters_SentHistoryV2.aspx?org={0}", org.OrganisationID);
        this.lnkPrintLetter.NavigateUrl        = String.Format("~/Letters_PrintV2.aspx?org={0}",        org.OrganisationID);
        this.lnkPrintBatchLetters.NavigateUrl  = String.Format("~/Letters_PrintBatchV2.aspx?org={0}",  org.OrganisationID);


        this.lnkThisOrgsStaff.Visible = true;
        this.lnkThisOrgsStaff.NavigateUrl = "~/RegisterStaffToOrganisationV2.aspx?id=" + Request.QueryString["id"].ToString();
        this.lnkThisOrgsStaff.Text = "Edit";

        this.lnkThisOrgsReferrers.Visible = true;
        this.lnkThisOrgsReferrers.NavigateUrl = "~/RegisterReferrersToOrganisationV2.aspx?id=" + Request.QueryString["id"].ToString();
        this.lnkThisOrgsReferrers.Text = "Edit";

        this.lnkThisOrgsPatients.Visible = true;
        this.lnkThisOrgsPatients.NavigateUrl = "~/RegisterPatientsToOrganisationV2.aspx?id=" + Request.QueryString["id"].ToString();
        this.lnkThisOrgsPatients.Text = "Edit All";

        this.lnkThisOrgsExistingPatients.Visible = true;
        this.lnkThisOrgsExistingPatients.NavigateUrl = "~/RegisterPatientsToOrganisationV2.aspx?id=" + Request.QueryString["id"].ToString() + "&view_only_current=1";
        this.lnkThisOrgsExistingPatients.Text = "Edit Current PTS";

        


        FillDropDowns();


        lblId.Text           = org.OrganisationID.ToString();
        lblDateAdded.Text    = org.OrganisationDateAdded.ToString("dd-MM-yyyy");
        lblLastBatchRun.Text = org.LastBatchRun == DateTime.MinValue ? "Never" : org.LastBatchRun.ToString("dd-MM-yyyy");

        txtComments.Text = org.Comment;

        chkIncSunday.Checked    = chkIncSun.Checked = !org.ExclSun;
        chkIncMonday.Checked    = chkIncMon.Checked = !org.ExclMon;
        chkIncTuesday.Checked   = chkIncTue.Checked = !org.ExclTue;
        chkIncWednesday.Checked = chkIncWed.Checked = !org.ExclWed;
        chkIncThursday.Checked  = chkIncThu.Checked = !org.ExclThu;
        chkIncFriday.Checked    = chkIncFri.Checked = !org.ExclFri;
        chkIncSaturday.Checked  = chkIncSat.Checked = !org.ExclSat;

        ddlSunStart_Hour.SelectedValue = org.SunStartTime.Hours.ToString();
        ddlSunEnd_Hour.SelectedValue = org.SunEndTime.Hours.ToString();
        ddlMonStart_Hour.SelectedValue = org.MonStartTime.Hours.ToString();
        ddlMonEnd_Hour.SelectedValue = org.MonEndTime.Hours.ToString();
        ddlTueStart_Hour.SelectedValue = org.TueStartTime.Hours.ToString();
        ddlTueEnd_Hour.SelectedValue = org.TueEndTime.Hours.ToString();
        ddlWedStart_Hour.SelectedValue = org.WedStartTime.Hours.ToString();
        ddlWedEnd_Hour.SelectedValue = org.WedEndTime.Hours.ToString();
        ddlThuStart_Hour.SelectedValue = org.ThuStartTime.Hours.ToString();
        ddlThuEnd_Hour.SelectedValue = org.ThuEndTime.Hours.ToString();
        ddlFriStart_Hour.SelectedValue = org.FriStartTime.Hours.ToString();
        ddlFriEnd_Hour.SelectedValue = org.FriEndTime.Hours.ToString();
        ddlSatStart_Hour.SelectedValue = org.SatStartTime.Hours.ToString();
        ddlSatEnd_Hour.SelectedValue = org.SatEndTime.Hours.ToString();

        ddlSunStart_Minute.SelectedValue = org.SunStartTime.Minutes.ToString();
        ddlSunEnd_Minute.SelectedValue = org.SunEndTime.Minutes.ToString();
        ddlMonStart_Minute.SelectedValue = org.MonStartTime.Minutes.ToString();
        ddlMonEnd_Minute.SelectedValue = org.MonEndTime.Minutes.ToString();
        ddlTueStart_Minute.SelectedValue = org.TueStartTime.Minutes.ToString();
        ddlTueEnd_Minute.SelectedValue = org.TueEndTime.Minutes.ToString();
        ddlWedStart_Minute.SelectedValue = org.WedStartTime.Minutes.ToString();
        ddlWedEnd_Minute.SelectedValue = org.WedEndTime.Minutes.ToString();
        ddlThuStart_Minute.SelectedValue = org.ThuStartTime.Minutes.ToString();
        ddlThuEnd_Minute.SelectedValue = org.ThuEndTime.Minutes.ToString();
        ddlFriStart_Minute.SelectedValue = org.FriStartTime.Minutes.ToString();
        ddlFriEnd_Minute.SelectedValue = org.FriEndTime.Minutes.ToString();
        ddlSatStart_Minute.SelectedValue = org.SatStartTime.Minutes.ToString();
        ddlSatEnd_Minute.SelectedValue = org.SatEndTime.Minutes.ToString();

        ddlSunLunchStart_Hour.SelectedValue = org.SunLunchStartTime.Hours.ToString();
        ddlSunLunchEnd_Hour.SelectedValue = org.SunLunchEndTime.Hours.ToString();
        ddlMonLunchStart_Hour.SelectedValue = org.MonLunchStartTime.Hours.ToString();
        ddlMonLunchEnd_Hour.SelectedValue = org.MonLunchEndTime.Hours.ToString();
        ddlTueLunchStart_Hour.SelectedValue = org.TueLunchStartTime.Hours.ToString();
        ddlTueLunchEnd_Hour.SelectedValue = org.TueLunchEndTime.Hours.ToString();
        ddlWedLunchStart_Hour.SelectedValue = org.WedLunchStartTime.Hours.ToString();
        ddlWedLunchEnd_Hour.SelectedValue = org.WedLunchEndTime.Hours.ToString();
        ddlThuLunchStart_Hour.SelectedValue = org.ThuLunchStartTime.Hours.ToString();
        ddlThuLunchEnd_Hour.SelectedValue = org.ThuLunchEndTime.Hours.ToString();
        ddlFriLunchStart_Hour.SelectedValue = org.FriLunchStartTime.Hours.ToString();
        ddlFriLunchEnd_Hour.SelectedValue = org.FriLunchEndTime.Hours.ToString();
        ddlSatLunchStart_Hour.SelectedValue = org.SatLunchStartTime.Hours.ToString();
        ddlSatLunchEnd_Hour.SelectedValue = org.SatLunchEndTime.Hours.ToString();

        ddlSunLunchStart_Minute.SelectedValue = org.SunLunchStartTime.Minutes.ToString();
        ddlSunLunchEnd_Minute.SelectedValue = org.SunLunchEndTime.Minutes.ToString();
        ddlMonLunchStart_Minute.SelectedValue = org.MonLunchStartTime.Minutes.ToString();
        ddlMonLunchEnd_Minute.SelectedValue = org.MonLunchEndTime.Minutes.ToString();
        ddlTueLunchStart_Minute.SelectedValue = org.TueLunchStartTime.Minutes.ToString();
        ddlTueLunchEnd_Minute.SelectedValue = org.TueLunchEndTime.Minutes.ToString();
        ddlWedLunchStart_Minute.SelectedValue = org.WedLunchStartTime.Minutes.ToString();
        ddlWedLunchEnd_Minute.SelectedValue = org.WedLunchEndTime.Minutes.ToString();
        ddlThuLunchStart_Minute.SelectedValue = org.ThuLunchStartTime.Minutes.ToString();
        ddlThuLunchEnd_Minute.SelectedValue = org.ThuLunchEndTime.Minutes.ToString();
        ddlFriLunchStart_Minute.SelectedValue = org.FriLunchStartTime.Minutes.ToString();
        ddlFriLunchEnd_Minute.SelectedValue = org.FriLunchEndTime.Minutes.ToString();
        ddlSatLunchStart_Minute.SelectedValue = org.SatLunchStartTime.Minutes.ToString();
        ddlSatLunchEnd_Minute.SelectedValue = org.SatLunchEndTime.Minutes.ToString();


        // hidden fields for javascript to use for resetting time when a time is changed to one 
        // that has a future booking at that time of the week and we change it back to the previous time
        lblSunStart_Hour.Text = org.SunStartTime.Hours.ToString();
        lblSunStart_Minute.Text = org.SunStartTime.Minutes.ToString();
        lblSunEnd_Hour.Text = org.SunEndTime.Hours.ToString();
        lblSunEnd_Minute.Text = org.SunEndTime.Minutes.ToString();

        lblMonStart_Hour.Text = org.MonStartTime.Hours.ToString();
        lblMonStart_Minute.Text = org.MonStartTime.Minutes.ToString();
        lblMonEnd_Hour.Text = org.MonEndTime.Hours.ToString();
        lblMonEnd_Minute.Text = org.MonEndTime.Minutes.ToString();

        lblTueStart_Hour.Text = org.TueStartTime.Hours.ToString();
        lblTueStart_Minute.Text = org.TueStartTime.Minutes.ToString();
        lblTueEnd_Hour.Text = org.TueEndTime.Hours.ToString();
        lblTueEnd_Minute.Text = org.TueEndTime.Minutes.ToString();

        lblWedStart_Hour.Text = org.WedStartTime.Hours.ToString();
        lblWedStart_Minute.Text = org.WedStartTime.Minutes.ToString();
        lblWedEnd_Hour.Text = org.WedEndTime.Hours.ToString();
        lblWedEnd_Minute.Text = org.WedEndTime.Minutes.ToString();

        lblThuStart_Hour.Text = org.ThuStartTime.Hours.ToString();
        lblThuStart_Minute.Text = org.ThuStartTime.Minutes.ToString();
        lblThuEnd_Hour.Text = org.ThuEndTime.Hours.ToString();
        lblThuEnd_Minute.Text = org.ThuEndTime.Minutes.ToString();

        lblFriStart_Hour.Text = org.FriStartTime.Hours.ToString();
        lblFriStart_Minute.Text = org.FriStartTime.Minutes.ToString();
        lblFriEnd_Hour.Text = org.FriEndTime.Hours.ToString();
        lblFriEnd_Minute.Text = org.FriEndTime.Minutes.ToString();

        lblSatStart_Hour.Text = org.SatStartTime.Hours.ToString();
        lblSatStart_Minute.Text = org.SatStartTime.Minutes.ToString();
        lblSatEnd_Hour.Text = org.SatEndTime.Hours.ToString();
        lblSatEnd_Minute.Text = org.SatEndTime.Minutes.ToString();


        lblSunLunchStart_Hour.Text = org.SunLunchStartTime.Hours.ToString();
        lblSunLunchStart_Minute.Text = org.SunLunchStartTime.Minutes.ToString();
        lblSunLunchEnd_Hour.Text = org.SunLunchEndTime.Hours.ToString();
        lblSunLunchEnd_Minute.Text = org.SunLunchEndTime.Minutes.ToString();

        lblMonLunchStart_Hour.Text = org.MonLunchStartTime.Hours.ToString();
        lblMonLunchStart_Minute.Text = org.MonLunchStartTime.Minutes.ToString();
        lblMonLunchEnd_Hour.Text = org.MonLunchEndTime.Hours.ToString();
        lblMonLunchEnd_Minute.Text = org.MonLunchEndTime.Minutes.ToString();

        lblTueLunchStart_Hour.Text = org.TueLunchStartTime.Hours.ToString();
        lblTueLunchStart_Minute.Text = org.TueLunchStartTime.Minutes.ToString();
        lblTueLunchEnd_Hour.Text = org.TueLunchEndTime.Hours.ToString();
        lblTueLunchEnd_Minute.Text = org.TueLunchEndTime.Minutes.ToString();

        lblWedLunchStart_Hour.Text = org.WedLunchStartTime.Hours.ToString();
        lblWedLunchStart_Minute.Text = org.WedLunchStartTime.Minutes.ToString();
        lblWedLunchEnd_Hour.Text = org.WedLunchEndTime.Hours.ToString();
        lblWedLunchEnd_Minute.Text = org.WedLunchEndTime.Minutes.ToString();

        lblThuLunchStart_Hour.Text = org.ThuLunchStartTime.Hours.ToString();
        lblThuLunchStart_Minute.Text = org.ThuLunchStartTime.Minutes.ToString();
        lblThuLunchEnd_Hour.Text = org.ThuLunchEndTime.Hours.ToString();
        lblThuLunchEnd_Minute.Text = org.ThuLunchEndTime.Minutes.ToString();

        lblFriLunchStart_Hour.Text = org.FriLunchStartTime.Hours.ToString();
        lblFriLunchStart_Minute.Text = org.FriLunchStartTime.Minutes.ToString();
        lblFriLunchEnd_Hour.Text = org.FriLunchEndTime.Hours.ToString();
        lblFriLunchEnd_Minute.Text = org.FriLunchEndTime.Minutes.ToString();

        lblSatLunchStart_Hour.Text = org.SatLunchStartTime.Hours.ToString();
        lblSatLunchStart_Minute.Text = org.SatLunchStartTime.Minutes.ToString();
        lblSatLunchEnd_Hour.Text = org.SatLunchEndTime.Hours.ToString();
        lblSatLunchEnd_Minute.Text = org.SatLunchEndTime.Minutes.ToString();


        if (isEditMode)
        {

            if (org.StartDate != DateTime.MinValue)
            {
                ddlStartDate_Day.SelectedValue = org.StartDate.Day.ToString();
                ddlStartDate_Month.SelectedValue = org.StartDate.Month.ToString();
                ddlStartDate_Year.SelectedValue = org.StartDate.Year.ToString();
            }
            if (org.EndDate != DateTime.MinValue)
            {
                ddlEndDate_Day.SelectedValue = org.EndDate.Day.ToString();
                ddlEndDate_Month.SelectedValue = org.EndDate.Month.ToString();
                ddlEndDate_Year.SelectedValue = org.EndDate.Year.ToString();
            }

            txtName.Text = org.Name;

            bool found = false;
            foreach (ListItem li in ddlParent.Items)
                if (org.ParentOrganisation != null && li.Value == org.ParentOrganisation.OrganisationID.ToString())
                    found = true;
            if (!found)
            {
                foreach(DataRow row in OrganisationDB.GetDataTable().Rows)
                {
                    Organisation o = OrganisationDB.Load(row);
                    if (org.ParentOrganisation != null && org.ParentOrganisation.OrganisationID == o.OrganisationID)
                        ddlParent.Items.Add(new ListItem(o.Name, o.OrganisationID.ToString()));
                }
            }

            ddlParent.SelectedValue = org.ParentOrganisation == null ? "0" : org.ParentOrganisation.OrganisationID.ToString();
            ddlUseParentOffernigPrices.SelectedValue = org.UseParentOffernigPrices.ToString();
            ddlType.SelectedValue = org.OrganisationType.OrganisationTypeID.ToString();

            if (ddlCustType.Items.FindByValue(org.OrganisationCustomerTypeID.ToString()) == null)
            {
                DataTable custTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "OrganisationCustomerType", "organisation_customer_type_id = " + org.OrganisationCustomerTypeID, "descr", "organisation_customer_type_id", "descr");
                ddlCustType.Items.Add(new ListItem(custTypes.Rows[0]["descr"].ToString(), org.OrganisationCustomerTypeID.ToString()));
            }

            ddlCustType.SelectedValue = org.OrganisationCustomerTypeID.ToString();
            txtABN.Text = org.Abn;
            txtACN.Text = org.Acn;
            txtBPayAccount.Text = org.BpayAccount;
            ddlServiceCycle.SelectedValue = org.WeeksPerServiceCycle.ToString();
            ddlFreeServices.SelectedValue = org.FreeServices.ToString();


            chkIncSunday.Checked    = !org.ExclSun;
            chkIncMonday.Checked    = !org.ExclMon;
            chkIncTuesday.Checked   = !org.ExclTue;
            chkIncWednesday.Checked = !org.ExclWed;
            chkIncThursday.Checked  = !org.ExclThu;
            chkIncFriday.Checked    = !org.ExclFri;
            chkIncSaturday.Checked  = !org.ExclSat;

            chkIncSun.Visible = false;
            chkIncMon.Visible = false;
            chkIncTue.Visible = false;
            chkIncWed.Visible = false;
            chkIncThu.Visible = false;
            chkIncFri.Visible = false;
            chkIncSat.Visible = false;


            lblName.Visible         = false;
            lblParent.Visible       = false;
            lblUseParentOffernigPrices.Visible = false;
            lblType.Visible         = false;
            lblCustType.Visible     = false;
            lblABN.Visible          = false;
            lblABN.Visible          = false;
            lblACN.Visible          = false;
            lblBPayAccount.Visible  = false;
            lblServiceCycle.Visible = false;
            lblFreeServices.Visible = false;
            lblStartDate.Visible    = false;
            lblEndDate.Visible      = false;
        }
        else
        {
            string parentOrgName = "";
            foreach(DataRow row in OrganisationDB.GetDataTable().Rows)
            {
                Organisation o = OrganisationDB.Load(row);
                if (org.ParentOrganisation != null && org.ParentOrganisation.OrganisationID == o.OrganisationID)
                    parentOrgName = o.Name;
            }

            string orgCustomerTypeDescr = "";
            foreach (DataRow row in DBBase.GetGenericDataTable(null, "OrganisationCustomerType", "organisation_customer_type_id", "descr").Rows)   
            {
                IDandDescr idAndDesc = IDandDescrDB.Load(row, "organisation_customer_type_id", "descr");
                if (org.OrganisationCustomerTypeID == idAndDesc.ID)
                    orgCustomerTypeDescr = idAndDesc.Descr;
            }


            lblName.Text                    = org.Name;
            lblParent.Text                  = parentOrgName.Length        == 0 ? "--"  : parentOrgName;
            lblUseParentOffernigPrices.Text = org.UseParentOffernigPrices      ? "Yes" : "No";
            lblType.Text                    = org.OrganisationType.Descr;
            lblCustType.Text                = orgCustomerTypeDescr.Length == 0 ? "--"  : orgCustomerTypeDescr;
            lblABN.Text                     = org.Abn.Length              == 0 ? "--"  : org.Abn;
            lblACN.Text                     = org.Acn.Length              == 0 ? "--"  : org.Acn;
            lblBPayAccount.Text             = org.BpayAccount.Length      == 0 ? "--"  : org.BpayAccount;
            lblServiceCycle.Text            = org.WeeksPerServiceCycle.ToString();
            lblFreeServices.Text            = org.FreeServices.ToString();
            lblStartDate.Text               = org.StartDate == DateTime.MinValue ? "--" : org.StartDate.ToString("dd-MM-yyyy");
            lblEndDate.Text                 = org.EndDate   == DateTime.MinValue ? "--" : org.EndDate.ToString("dd-MM-yyyy");


            lblDateAdded.Font.Bold    = true;
            lblLastBatchRun.Font.Bold = true;

            txtComments.Enabled       = false;
            txtComments.ForeColor     = System.Drawing.Color.Black;

            txtName.Visible         = false;
            ddlParent.Visible       = false;
            ddlUseParentOffernigPrices.Visible = false;
            ddlType.Visible         = false;
            ddlCustType.Visible     = false;
            txtABN.Visible          = false;
            txtACN.Visible          = false;
            txtBPayAccount.Visible  = false;
            ddlServiceCycle.Visible = false;
            ddlFreeServices.Visible = false;

            ddlStartDate_Day.Visible   = false;
            ddlStartDate_Month.Visible = false;
            ddlStartDate_Year.Visible  = false;
            ddlEndDate_Day.Visible     = false;
            ddlEndDate_Month.Visible   = false;
            ddlEndDate_Year.Visible    = false;

            chkIncSunday.Visible         = false;
            chkIncMonday.Visible         = false;
            chkIncTuesday.Visible        = false;
            chkIncWednesday.Visible      = false;
            chkIncThursday.Visible       = false;
            chkIncFriday.Visible         = false;
            chkIncSaturday.Visible       = false;

            chkIncSun.Enabled = false;
            chkIncMon.Enabled = false;
            chkIncTue.Enabled = false;
            chkIncWed.Enabled = false;
            chkIncThu.Enabled = false;
            chkIncFri.Enabled = false;
            chkIncSat.Enabled = false;


            ddlSunStart_Hour.Enabled = false;
            ddlSunEnd_Hour.Enabled = false;
            ddlMonStart_Hour.Enabled = false;
            ddlMonEnd_Hour.Enabled = false;
            ddlTueStart_Hour.Enabled = false;
            ddlTueEnd_Hour.Enabled = false;
            ddlWedStart_Hour.Enabled = false;
            ddlWedEnd_Hour.Enabled = false;
            ddlThuStart_Hour.Enabled = false;
            ddlThuEnd_Hour.Enabled = false;
            ddlFriStart_Hour.Enabled = false;
            ddlFriEnd_Hour.Enabled = false;
            ddlSatStart_Hour.Enabled = false;
            ddlSatEnd_Hour.Enabled = false;

            ddlSunStart_Minute.Enabled = false;
            ddlSunEnd_Minute.Enabled = false;
            ddlMonStart_Minute.Enabled = false;
            ddlMonEnd_Minute.Enabled = false;
            ddlTueStart_Minute.Enabled = false;
            ddlTueEnd_Minute.Enabled = false;
            ddlWedStart_Minute.Enabled = false;
            ddlWedEnd_Minute.Enabled = false;
            ddlThuStart_Minute.Enabled = false;
            ddlThuEnd_Minute.Enabled = false;
            ddlFriStart_Minute.Enabled = false;
            ddlFriEnd_Minute.Enabled = false;
            ddlSatStart_Minute.Enabled = false;
            ddlSatEnd_Minute.Enabled = false;

            ddlSunLunchStart_Hour.Enabled = false;
            ddlSunLunchEnd_Hour.Enabled = false;
            ddlMonLunchStart_Hour.Enabled = false;
            ddlMonLunchEnd_Hour.Enabled = false;
            ddlTueLunchStart_Hour.Enabled = false;
            ddlTueLunchEnd_Hour.Enabled = false;
            ddlWedLunchStart_Hour.Enabled = false;
            ddlWedLunchEnd_Hour.Enabled = false;
            ddlThuLunchStart_Hour.Enabled = false;
            ddlThuLunchEnd_Hour.Enabled = false;
            ddlFriLunchStart_Hour.Enabled = false;
            ddlFriLunchEnd_Hour.Enabled = false;
            ddlSatLunchStart_Hour.Enabled = false;
            ddlSatLunchEnd_Hour.Enabled = false;

            ddlSunLunchStart_Minute.Enabled = false;
            ddlSunLunchEnd_Minute.Enabled = false;
            ddlMonLunchStart_Minute.Enabled = false;
            ddlMonLunchEnd_Minute.Enabled = false;
            ddlTueLunchStart_Minute.Enabled = false;
            ddlTueLunchEnd_Minute.Enabled = false;
            ddlWedLunchStart_Minute.Enabled = false;
            ddlWedLunchEnd_Minute.Enabled = false;
            ddlThuLunchStart_Minute.Enabled = false;
            ddlThuLunchEnd_Minute.Enabled = false;
            ddlFriLunchStart_Minute.Enabled = false;
            ddlFriLunchEnd_Minute.Enabled = false;
            ddlSatLunchStart_Minute.Enabled = false;
            ddlSatLunchEnd_Minute.Enabled = false;
        }

        DataTable incStaffList = RegisterStaffDB.GetDataTable_StaffOf(org.OrganisationID);
        incStaffList.DefaultView.Sort = "surname ASC";
        if (incStaffList.Rows.Count == 0)
            lstStaff.Items.Add(new ListItem("No Staff Allocated Yet"));
        else
        {
            foreach (DataRowView row in incStaffList.DefaultView)
                lstStaff.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString()));
        }

        DataTable incReferrerList = RegisterReferrerDB.GetDataTable_ReferrersOf(org.OrganisationID);
        incReferrerList.DefaultView.Sort = "surname ASC";
        if (incReferrerList.Rows.Count == 0)
            lstReferrers.Items.Add(new ListItem("No Referrers Allocated Yet"));
        else
        {
            foreach (DataRowView row in incReferrerList.DefaultView)
                lstReferrers.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString()));
        }


        DataTable incPatientList = RegisterPatientDB.GetDataTable_PatientsOf(false, org.OrganisationID);
        incPatientList.DefaultView.Sort = "surname ASC";
        if (incPatientList.Rows.Count == 0)
            lstPatients.Items.Add(new ListItem("No Patients Allocated Yet"));
        else
        {
            foreach (DataRowView row in incPatientList.DefaultView)
                lstPatients.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString()));
        }



        btnSubmit.Text = isEditMode ? "Update Details" : "Edit Details";
        btnCancel.Visible = isEditMode;
    }

    private void FillEmptyAddForm()
    {
        txtName.Focus();

        //this.lnkNotes.Visible = false;

        this.lnkThisOrgsStaff.Visible            = false;
        this.lnkThisOrgsReferrers.Visible        = false;
        this.lnkThisOrgsPatients.Visible         = false;
        this.lnkThisOrgsExistingPatients.Visible = false;


        if (GetUrlParamOrgType() == UrlParamOrgType.Insurance)
            typeRow.Attributes["class"] = "hiddencol";
        else if (UserView.GetInstance().IsClinicView) // only one type, so hide.  But dont make invis (ie remove from page) - need for updating/inserting records
            typeRow.Attributes["class"] = "hiddencol";

        FillDropDowns();

        idRow.Visible           = false;
        dateAddedRow.Visible    = false;
        lastBatchRunRow.Visible = false;



        existingOrgInfo.Visible           = false;
        existingOrgInfoSpace.Visible      = false;
        existingOrgInfoSpaceShort.Visible = GetUrlParamOrgType() != UrlParamOrgType.Insurance;


        ddlStartDate_Day.SelectedValue = DateTime.Today.Day.ToString();
        ddlStartDate_Month.SelectedValue = DateTime.Today.Month.ToString();
        ddlStartDate_Year.SelectedValue = DateTime.Today.Year.ToString();

        chkIncSun.Visible = false;
        chkIncMon.Visible = false;
        chkIncTue.Visible = false;
        chkIncWed.Visible = false;
        chkIncThu.Visible = false;
        chkIncFri.Visible = false;
        chkIncSat.Visible = false;

        chkIncSunday.Checked    = false;
        chkIncMonday.Checked    = true;
        chkIncTuesday.Checked   = true;
        chkIncWednesday.Checked = true;
        chkIncThursday.Checked  = true;
        chkIncFriday.Checked    = true;
        chkIncSaturday.Checked  = false;

        ddlSunStart_Hour.SelectedValue = "8";
        ddlSunEnd_Hour.SelectedValue   = "18";
        ddlMonStart_Hour.SelectedValue = "8";
        ddlMonEnd_Hour.SelectedValue   = "18";
        ddlTueStart_Hour.SelectedValue = "8";
        ddlTueEnd_Hour.SelectedValue   = "18";
        ddlWedStart_Hour.SelectedValue = "8";
        ddlWedEnd_Hour.SelectedValue   = "18";
        ddlThuStart_Hour.SelectedValue = "8";
        ddlThuEnd_Hour.SelectedValue   = "18";
        ddlFriStart_Hour.SelectedValue = "8";
        ddlFriEnd_Hour.SelectedValue   = "18";
        ddlSatStart_Hour.SelectedValue = "8";
        ddlSatEnd_Hour.SelectedValue   = "18";

        ddlSunStart_Minute.SelectedValue = "0";
        ddlSunEnd_Minute.SelectedValue   = "0";
        ddlMonStart_Minute.SelectedValue = "0";
        ddlMonEnd_Minute.SelectedValue   = "0";
        ddlTueStart_Minute.SelectedValue = "0";
        ddlTueEnd_Minute.SelectedValue   = "0";
        ddlWedStart_Minute.SelectedValue = "0";
        ddlWedEnd_Minute.SelectedValue   = "0";
        ddlThuStart_Minute.SelectedValue = "0";
        ddlThuEnd_Minute.SelectedValue   = "0";
        ddlFriStart_Minute.SelectedValue = "0";
        ddlFriEnd_Minute.SelectedValue   = "0";
        ddlSatStart_Minute.SelectedValue = "0";
        ddlSatEnd_Minute.SelectedValue   = "0";

        btnSubmit.Text = "Add Organisation";
    }

    protected void EndDateAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidEndDate();
    }
    protected void StartDateAllOrNoneCheck(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = IsValidStartDate();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (!ddlEndDateValidateAllOrNoneSet.IsValid || !ddlStartDateValidateAllOrNoneSet.IsValid)
            return;

        string type = Request.QueryString["type"];


        if (GetUrlParamType() == UrlParamType.View)
        {
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "edit"));
        }
        else if (GetUrlParamType() == UrlParamType.Edit)
        {
            Organisation orig = OrganisationDB.GetByID(Convert.ToInt32(lblId.Text));
            if (IsValidFormOrgTypeIDs() && (GetFormOrgTypeIDs() == "191" || GetFormOrgTypeIDs() == "150"))
            {
                OrganisationDB.UpdateExtOrg(Convert.ToInt32(lblId.Text), orig.OrganisationType.OrganisationTypeID, txtName.Text, txtACN.Text, txtABN.Text, DateTime.Today,
                                      orig.IsDebtor, orig.IsCreditor, orig.BpayAccount, txtComments.Text);
            }
            else
            {
                OrganisationDB.Update(Convert.ToInt32(lblId.Text), Convert.ToInt32(ddlParent.SelectedValue), Convert.ToBoolean(ddlUseParentOffernigPrices.SelectedValue), Convert.ToInt32(ddlType.SelectedValue), Convert.ToInt32(ddlCustType.SelectedValue), txtName.Text, txtACN.Text, txtABN.Text,
                                      orig.IsDebtor, orig.IsCreditor, txtBPayAccount.Text, Convert.ToInt32(ddlServiceCycle.SelectedValue),
                                      GetStartDateFromForm(), GetEndDateFromForm(), txtComments.Text, Convert.ToInt32(ddlFreeServices.SelectedValue),
                                      !chkIncSunday.Checked, !chkIncMonday.Checked, !chkIncTuesday.Checked, !chkIncWednesday.Checked,
                                      !chkIncThursday.Checked, !chkIncFriday.Checked, !chkIncSaturday.Checked,
                                      GetSunStartTimeFromForm(), GetSunEndTimeFromForm(),
                                      GetMonStartTimeFromForm(), GetMonEndTimeFromForm(),
                                      GetTueStartTimeFromForm(), GetTueEndTimeFromForm(),
                                      GetWedStartTimeFromForm(), GetWedEndTimeFromForm(),
                                      GetThuStartTimeFromForm(), GetThuEndTimeFromForm(),
                                      GetFriStartTimeFromForm(), GetFriEndTimeFromForm(),
                                      GetSatStartTimeFromForm(), GetSatEndTimeFromForm(),
                                      GetSunLunchStartTimeFromForm(), GetSunLunchEndTimeFromForm(),
                                      GetMonLunchStartTimeFromForm(), GetMonLunchEndTimeFromForm(),
                                      GetTueLunchStartTimeFromForm(), GetTueLunchEndTimeFromForm(),
                                      GetWedLunchStartTimeFromForm(), GetWedLunchEndTimeFromForm(),
                                      GetThuLunchStartTimeFromForm(), GetThuLunchEndTimeFromForm(),
                                      GetFriLunchStartTimeFromForm(), GetFriLunchEndTimeFromForm(),
                                      GetSatLunchStartTimeFromForm(), GetSatLunchEndTimeFromForm(),
                                      GetLastBatchRunFromForm());
            }

            //Response.Redirect( (Request.QueryString["return_url"] != null) ? System.Web.HttpUtility.UrlDecode(Request.QueryString["return_url"]) : "~/OrganisationListV2.aspx");
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
        }
        else if (GetUrlParamType() == UrlParamType.Add)
        {
            if (IsValidFormOrgTypeIDs() && GetFormOrgTypeIDs() == "191")
            {
                int id = OrganisationDB.InsertExtOrg(Convert.ToInt32(GetFormOrgTypeIDs()), txtName.Text, txtACN.Text, txtABN.Text, OrganisationTypeDB.IsDebtor(191), OrganisationTypeDB.IsCreditor(191), "", txtComments.Text);

                //Response.Redirect((Request.QueryString["return_url"] != null) ? System.Web.HttpUtility.UrlDecode(Request.QueryString["return_url"]) : "~/OrganisationListV2.aspx");
                string url = Request.RawUrl;
                url = UrlParamModifier.AddEdit(url, "type", "view");
                url = UrlParamModifier.AddEdit(url, "id", id.ToString());
                Response.Redirect(url);
            }
            else
            {
                int id = OrganisationDB.Insert(Convert.ToInt32(ddlParent.SelectedValue), Convert.ToBoolean(ddlUseParentOffernigPrices.SelectedValue), Convert.ToInt32(ddlType.SelectedValue), Convert.ToInt32(ddlCustType.SelectedValue), txtName.Text, txtACN.Text, txtABN.Text,
                                      OrganisationTypeDB.IsDebtor(Convert.ToInt32(ddlType.SelectedValue)), OrganisationTypeDB.IsCreditor(Convert.ToInt32(ddlType.SelectedValue)), txtBPayAccount.Text, Convert.ToInt32(ddlServiceCycle.SelectedValue),
                                      GetStartDateFromForm(), GetEndDateFromForm(), txtComments.Text, Convert.ToInt32(ddlFreeServices.SelectedValue),
                                      !chkIncSunday.Checked, !chkIncMonday.Checked, !chkIncTuesday.Checked, !chkIncWednesday.Checked,
                                      !chkIncThursday.Checked, !chkIncFriday.Checked, !chkIncSaturday.Checked,
                                      GetSunStartTimeFromForm(), GetSunEndTimeFromForm(),
                                      GetMonStartTimeFromForm(), GetMonEndTimeFromForm(),
                                      GetTueStartTimeFromForm(), GetTueEndTimeFromForm(),
                                      GetWedStartTimeFromForm(), GetWedEndTimeFromForm(),
                                      GetThuStartTimeFromForm(), GetThuEndTimeFromForm(),
                                      GetFriStartTimeFromForm(), GetFriEndTimeFromForm(),
                                      GetSatStartTimeFromForm(), GetSatEndTimeFromForm(),
                                      GetSunLunchStartTimeFromForm(), GetSunLunchEndTimeFromForm(),
                                      GetMonLunchStartTimeFromForm(), GetMonLunchEndTimeFromForm(),
                                      GetTueLunchStartTimeFromForm(), GetTueLunchEndTimeFromForm(),
                                      GetWedLunchStartTimeFromForm(), GetWedLunchEndTimeFromForm(),
                                      GetThuLunchStartTimeFromForm(), GetThuLunchEndTimeFromForm(),
                                      GetFriLunchStartTimeFromForm(), GetFriLunchEndTimeFromForm(),
                                      GetSatLunchStartTimeFromForm(), GetSatLunchEndTimeFromForm(),
                                      DateTime.MinValue);

                //Response.Redirect((Request.QueryString["return_url"] != null) ? System.Web.HttpUtility.UrlDecode(Request.QueryString["return_url"]) : "~/OrganisationListV2.aspx");
                string url = Request.RawUrl;
                url = UrlParamModifier.AddEdit(url, "type", "view");
                url = UrlParamModifier.AddEdit(url, "id", id.ToString());
                Response.Redirect(url);
            }

        }
        else
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
        }


    }

    protected void btnUpdateNotesIcon_Click(object sender, EventArgs e)
    {
        Organisation org = OrganisationDB.GetByID(GetFormID());
//        lnkNotes.ImageUrl = NoteDB.HasNotes(org.EntityID) ? "~/images/notes-48.png" : "~/images/notes-bw-48.jpg";
    }

    #region GetSunStartTimeFromForm() ... GetMonStartTimeFromForm() ... GetLastBatchRunFromForm(), GetDateAddedFromForm()

    public DateTime GetStartDateFromForm()
    {
        if (ddlStartDate_Day.SelectedValue == "-1" && ddlStartDate_Month.SelectedValue == "-1" && ddlStartDate_Year.SelectedValue == "-1")
            return DateTime.MinValue;

        else if (ddlStartDate_Day.SelectedValue != "-1" && ddlStartDate_Month.SelectedValue != "-1" && ddlStartDate_Year.SelectedValue != "-1")
            return new DateTime(Convert.ToInt32(ddlStartDate_Year.SelectedValue), Convert.ToInt32(ddlStartDate_Month.SelectedValue), Convert.ToInt32(ddlStartDate_Day.SelectedValue));

        else
            throw new Exception("Start Date format is some selected and some not selected.");
    }
    public bool IsValidStartDate()
    {
        bool invalid = ((ddlStartDate_Day.SelectedValue == "-1" || ddlStartDate_Month.SelectedValue == "-1" || ddlStartDate_Year.SelectedValue == "-1") &&
                        (ddlStartDate_Day.SelectedValue != "-1" || ddlStartDate_Month.SelectedValue != "-1" || ddlStartDate_Year.SelectedValue != "-1"));
        return !invalid;
    }
    public DateTime GetEndDateFromForm()
    {
        if (ddlEndDate_Day.SelectedValue == "-1" && ddlEndDate_Month.SelectedValue == "-1" && ddlEndDate_Year.SelectedValue == "-1")
            return DateTime.MinValue;

        else if (ddlEndDate_Day.SelectedValue != "-1" && ddlEndDate_Month.SelectedValue != "-1" && ddlEndDate_Year.SelectedValue != "-1")
            return new DateTime(Convert.ToInt32(ddlEndDate_Year.SelectedValue), Convert.ToInt32(ddlEndDate_Month.SelectedValue), Convert.ToInt32(ddlEndDate_Day.SelectedValue));

        else
            throw new Exception("End Date format is some selected and some not selected.");
    }
    public bool IsValidEndDate()
    {
        bool invalid = ((ddlEndDate_Day.SelectedValue == "-1" || ddlEndDate_Month.SelectedValue == "-1" || ddlEndDate_Year.SelectedValue == "-1") &&
                        (ddlEndDate_Day.SelectedValue != "-1" || ddlEndDate_Month.SelectedValue != "-1" || ddlEndDate_Year.SelectedValue != "-1"));
        return !invalid;
    }


    public TimeSpan GetSunStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlSunStart_Hour.SelectedValue), Convert.ToInt32(ddlSunStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetMonStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlMonStart_Hour.SelectedValue), Convert.ToInt32(ddlMonStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetTueStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlTueStart_Hour.SelectedValue), Convert.ToInt32(ddlTueStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetWedStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlWedStart_Hour.SelectedValue), Convert.ToInt32(ddlWedStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetThuStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlThuStart_Hour.SelectedValue), Convert.ToInt32(ddlThuStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetFriStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlFriStart_Hour.SelectedValue), Convert.ToInt32(ddlFriStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetSatStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlSatStart_Hour.SelectedValue), Convert.ToInt32(ddlSatStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetSunEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlSunEnd_Hour.SelectedValue), Convert.ToInt32(ddlSunEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetMonEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlMonEnd_Hour.SelectedValue), Convert.ToInt32(ddlMonEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetTueEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlTueEnd_Hour.SelectedValue), Convert.ToInt32(ddlTueEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetWedEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlWedEnd_Hour.SelectedValue), Convert.ToInt32(ddlWedEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetThuEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlThuEnd_Hour.SelectedValue), Convert.ToInt32(ddlThuEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetFriEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlFriEnd_Hour.SelectedValue), Convert.ToInt32(ddlFriEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetSatEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlSatEnd_Hour.SelectedValue), Convert.ToInt32(ddlSatEnd_Minute.SelectedValue), 0);
    }

    public TimeSpan GetSunLunchStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlSunLunchStart_Hour.SelectedValue), Convert.ToInt32(ddlSunLunchStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetMonLunchStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlMonLunchStart_Hour.SelectedValue), Convert.ToInt32(ddlMonLunchStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetTueLunchStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlTueLunchStart_Hour.SelectedValue), Convert.ToInt32(ddlTueLunchStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetWedLunchStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlWedLunchStart_Hour.SelectedValue), Convert.ToInt32(ddlWedLunchStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetThuLunchStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlThuLunchStart_Hour.SelectedValue), Convert.ToInt32(ddlThuLunchStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetFriLunchStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlFriLunchStart_Hour.SelectedValue), Convert.ToInt32(ddlFriLunchStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetSatLunchStartTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlSatLunchStart_Hour.SelectedValue), Convert.ToInt32(ddlSatLunchStart_Minute.SelectedValue), 0);
    }
    public TimeSpan GetSunLunchEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlSunLunchEnd_Hour.SelectedValue), Convert.ToInt32(ddlSunLunchEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetMonLunchEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlMonLunchEnd_Hour.SelectedValue), Convert.ToInt32(ddlMonLunchEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetTueLunchEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlTueLunchEnd_Hour.SelectedValue), Convert.ToInt32(ddlTueLunchEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetWedLunchEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlWedLunchEnd_Hour.SelectedValue), Convert.ToInt32(ddlWedLunchEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetThuLunchEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlThuLunchEnd_Hour.SelectedValue), Convert.ToInt32(ddlThuLunchEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetFriLunchEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlFriLunchEnd_Hour.SelectedValue), Convert.ToInt32(ddlFriLunchEnd_Minute.SelectedValue), 0);
    }
    public TimeSpan GetSatLunchEndTimeFromForm()
    {
        return new TimeSpan(Convert.ToInt32(ddlSatLunchEnd_Hour.SelectedValue), Convert.ToInt32(ddlSatLunchEnd_Minute.SelectedValue), 0);
    }
    
    
    public DateTime GetLastBatchRunFromForm()
    {
        if (lblLastBatchRun.Text == "Never")
            return DateTime.MinValue;

        string[] parts = lblLastBatchRun.Text.Trim().Split(new char[] { '-' });
        return new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]));
    }
    public DateTime GetDateAddedFromForm()
    {
        string[] parts = lblDateAdded.Text.Trim().Split(new char[] { '-' });
        return new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]));
    }

    #endregion


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