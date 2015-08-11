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

public partial class ContactAusDetailV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);

            if (!IsPostBack)
            {
                SetupGUI();

                UrlParamType urlParamType = GetUrlParamType();
                if ((urlParamType == UrlParamType.Edit || urlParamType == UrlParamType.View) && IsValidFormID())
                    FillEditViewForm(urlParamType == UrlParamType.Edit);
                else if (GetUrlParamType() == UrlParamType.Add && IsValidFormID() && GetUrlParamContactTypeGroup() != UrlParamContactTypeGroup.None)
                    FillEmptyAddForm();
                else
                    HideTableAndSetErrorMessage("", "Invalid URL Parameters");
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


    #region GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
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

    private enum UrlParamContactTypeGroup { Mailing, Telecoms, Internet, Bedroom, None };
    private UrlParamContactTypeGroup GetUrlParamContactTypeGroup()
    {
        string type = Request.QueryString["contact_type_group"];
        if (type != null && type.ToLower() == "1")
            return UrlParamContactTypeGroup.Mailing;
        if (type != null && type.ToLower() == "2")
            return UrlParamContactTypeGroup.Telecoms;
        if (type != null && type.ToLower() == "3")
            return UrlParamContactTypeGroup.Bedroom;
        if (type != null && type.ToLower() == "4")
            return UrlParamContactTypeGroup.Internet;
        else
            return UrlParamContactTypeGroup.None;
    }

    private EntityType GetUrlParamEntityType()
    {
        string entityType = Request.QueryString["entity_type"];
        return EntityType.GetByString(entityType);
    }

    #endregion

    #region SetupGUI()

    public void SetupGUI()
    {

        string allFeaturesType = "dialogWidth:500px;dialogHeight:750px;center:yes;resizable:no; scroll:no";
        string jsType = "javascript:window.showModalDialog('" + "ContactTypeListV2.aspx', '', '" + allFeaturesType + "');document.getElementById('btnUpdateType').click();return false;";

        string allFeatures = "dialogWidth:1100px;dialogHeight:600px;center:yes;resizable:no; scroll:no";
        string js = "javascript:window.showModalDialog('" + "StreetAndSuburbInfo.aspx', '', '" + allFeatures + "');document.getElementById('btnUpdateStreetAndSuburb').click();return false;";

        lnkUpdateType.NavigateUrl = "  ";
        lnkUpdateType.Text = "Add/Edit";
        lnkUpdateType.Attributes.Add("onclick", jsType);

        if (Utilities.IsMobileDevice(Request))
        {
            hiddenIsMobileDevice.Value = "1";
            lnkUpdateType.Visible = false;
        }

        DataTable addrChannelTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "AddressChannelType", "", "descr", "address_channel_type_id", "descr");
        ddlAddressChannelType.Items.Add(new ListItem("--", "-1"));
        foreach (DataRow row in addrChannelTypes.Rows)
            ddlAddressChannelType.Items.Add(new ListItem(row["descr"].ToString(), row["address_channel_type_id"].ToString()));

        DataTable countries = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "Country", "", "descr", "country_id", "descr");
        ddlCountry.Items.Add(new ListItem("--", "-1"));
        foreach (DataRow row in countries.Rows)
            ddlCountry.Items.Add(new ListItem(row["descr"].ToString(), row["country_id"].ToString()));
        ddlCountry.SelectedIndex = Utilities.IndexOf(ddlCountry, "australia");


        bool editable = GetUrlParamType() == UrlParamType.Add || GetUrlParamType() == UrlParamType.Edit;
        Utilities.SetEditControlBackColour(ddlContactType, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtFreeText, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtAddrLine1, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtAddrLine2, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(txtStreet, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlAddressChannelType, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlCountry, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }

    protected void btnUpdateStreetAndSuburb_Click(object sender, EventArgs e)
    {
        DataTable addrChannelTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "AddressChannelType", "", "descr", "address_channel_type_id", "descr");
        ddlAddressChannelType.Items.Clear();
        ddlAddressChannelType.Items.Add(new ListItem("--", "-1"));
        foreach (DataRow row in addrChannelTypes.Rows)
            ddlAddressChannelType.Items.Add(new ListItem(row["descr"].ToString(), row["address_channel_type_id"].ToString()));
    }

    protected void btnUpdateType_Click(object sender, EventArgs e)
    {
        DataTable addrTypes = ContactTypeDB.GetDataTable(1);
        ddlContactType.DataSource = addrTypes;
        ddlContactType.DataBind();
        // ddlContactType.SelectedValue = thisRow["ad_contact_type_id"].ToString();
    }

    #endregion


    private void FillEditViewForm(bool isEditMode)
    {
        ContactAus contact = ContactAusDB.GetByID(GetFormID());
        if (contact == null)
        {
            HideTableAndSetErrorMessage("Invalid Contact ID");
            return;
        }

        DataTable addrTypes = ContactTypeDB.GetDataTable(contact.ContactType.ContactTypeGroup.ID);
        ddlContactType.DataSource = addrTypes;
        ddlContactType.DataBind();

        string type = "ContactAus";
        if (contact.ContactType.ContactTypeGroup.ID == 1)
        {
            type = "Address";
            lblLine1Descr.Text = "Line 1";
        }
        if (contact.ContactType.ContactTypeGroup.ID == 2)
        {
            type = "Phone Number";
            lblLine1Descr.Text = "Phone Number";
        }
        if (contact.ContactType.ContactTypeGroup.ID == 3)
        {
            type = "Bedroom";
            lblLine1Descr.Text = "Bedroom";
        }
        if (contact.ContactType.ContactTypeGroup.ID == 4)
        {
            type = "Email/Website";
            lblLine1Descr.Text = "Email/Website";
        }
        lblHeading.Text = isEditMode ? "Edit " + type : "View " + type;

        if (contact.ContactType.ContactTypeGroup.ID != 1)
        {
            line2Row.Visible    = false;
            streetRow.Visible   = false;
            suburbRow.Visible   = false;
            countryRow.Visible  = false;
            if (contact.ContactType.ContactTypeGroup.ID != 4)
            {
                billingRow.Visible = false;
                nonbillingRow.Visible = false;
            }
        }

        lblId.Text = contact.ContactID.ToString();
        idRow.Visible = Utilities.IsDev();


        if (isEditMode)
        {
            ddlContactType.SelectedValue = contact.ContactType.ContactTypeID.ToString();
            txtFreeText.Text  = contact.FreeText;
            txtAddrLine1.Text = contact.ContactType.ContactTypeGroup.ID == 2 ? Utilities.FormatPhoneNumber(contact.AddrLine1) : contact.AddrLine1;
            txtAddrLine2.Text = contact.AddrLine2;

            txtStreet.Text                      = contact.StreetName;
            ddlAddressChannelType.SelectedValue = contact.AddressChannelType == null ? "-1" : contact.AddressChannelType.ID.ToString();
            suburbID.Value                      = contact.Suburb == null ? "-1" : contact.Suburb.SuburbID.ToString();
            lblSuburbText.Text                  = contact.Suburb == null ? "--" : contact.Suburb.Name + ", " + contact.Suburb.State + " (" + contact.Suburb.Postcode + ")";
            ddlCountry.SelectedValue            = contact.Country == null ? "-1" : contact.Country.ID.ToString();
            chkIsBilling.Checked                = contact.IsBilling;
            chkIsNonBilling.Checked             = contact.IsNonBilling;

            lblContactType.Visible          = false;
            lblAddrLine1.Visible            = false;
            lblAddrLine2.Visible            = false;
            lblAddressChannelType.Visible   = false;
            lblCountry.Visible              = false;
            lblIsBilling.Visible            = false;
            lblIsNonBilling.Visible         = false;

        }
        else
        {
            lblContactType.Text         = contact.ContactType.Descr;
            lblAddrLine1.Text           = contact.ContactType.ContactTypeGroup.ID == 2 ? Utilities.FormatPhoneNumber(contact.AddrLine1) : contact.AddrLine1;
            lblAddrLine2.Text           = contact.AddrLine2;
            lblStreet.Text              = contact.StreetName;
            lblAddressChannelType.Text = contact.AddressChannelType == null ? string.Empty : contact.AddressChannelType.Descr;
            suburbID.Value              = contact.Suburb == null ? "-1" : contact.Suburb.SuburbID.ToString();
            lblSuburbText.Text          = contact.Suburb == null ? "--" : contact.Suburb.Name + ", " + contact.Suburb.State + " (" + contact.Suburb.Postcode + ")";
            lblSuburbText.Font.Bold     = true;
            lblCountry.Text             = contact.Country == null ? string.Empty : contact.Country.Descr;
            lblIsBilling.Text           = contact.IsBilling ? "Yes" : "No";
            lblIsNonBilling.Text        = contact.IsNonBilling ? "Yes" : "No";

            lnkUpdateType.Visible       = false;
            txtValidateAddrLine1Required.Visible = false;

            ddlContactType.Visible        = false;
            txtFreeText.Visible           = false;
            txtAddrLine1.Visible          = false;
            txtAddrLine2.Visible          = false;
            txtStreet.Visible             = false;
            ddlAddressChannelType.Visible = false;
            lnkGetSuburb.Visible          = false;
            lnkClearSuburb.Visible        = false;
            ddlCountry.Visible            = false;
            chkIsBilling.Visible          = false;
            chkIsNonBilling.Visible       = false;
        }



        if (isEditMode)
        {
            btnSubmit.Text = "Update Details";
        }
        else // is view mode
        {
            btnSubmit.Visible = false;
            btnCancel.Text = "Close";
        }
    }

    private void FillEmptyAddForm()
    {
        string type = "ContactAus";
        int contactTypeGroupID = -1;
        UrlParamContactTypeGroup urlParamContactTypeGroup = GetUrlParamContactTypeGroup();
        if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Mailing)
        {
            type = "Address";
            lblLine1Descr.Text = "Line 1";
            contactTypeGroupID = 1;
        }
        if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Telecoms)
        {
            type = "Phone Number";
            lblLine1Descr.Text = "Phone Number";
            contactTypeGroupID = 2;
        }
        if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Bedroom)
        {
            type = "Bedroom";
            lblLine1Descr.Text = "Bedroom";
            contactTypeGroupID = 3;
        }
        if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Internet)
        {
            type = "Email/Website";
            lblLine1Descr.Text = "Email/Website";
            contactTypeGroupID = 4;
        }


        lblHeading.Text = "Add " + type;

        if (contactTypeGroupID != 1)
        {
            line2Row.Visible = false;
            streetRow.Visible = false;
            suburbRow.Visible = false;
            countryRow.Visible = false;
            if (contactTypeGroupID != 4)
            {
                billingRow.Visible = false;
                nonbillingRow.Visible = false;
            }
        }
        

        DataTable addrTypes = ContactTypeDB.GetDataTable(contactTypeGroupID);
        ddlContactType.DataSource = addrTypes;
        ddlContactType.DataBind();

        if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Mailing)
        {
            // (35, 'Home address'),
            // (36, 'Business address'),
            if (GetUrlParamEntityType().Type == EntityType.EntityTypeEnum.Referrer ||
                GetUrlParamEntityType().Type == EntityType.EntityTypeEnum.Organisation ||
                GetUrlParamEntityType().Type == EntityType.EntityTypeEnum.Site)
                ddlContactType.SelectedValue = "36";
            else
                ddlContactType.SelectedValue = "35";
        }
        if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Telecoms)
        {
            // (34,2,13,'Office Phone'),
            // (30,2,10,'Mobile'),
            if (GetUrlParamEntityType().Type == EntityType.EntityTypeEnum.Referrer ||
                GetUrlParamEntityType().Type == EntityType.EntityTypeEnum.Organisation ||
                GetUrlParamEntityType().Type == EntityType.EntityTypeEnum.Site)
                ddlContactType.SelectedValue = "34";
            else
                ddlContactType.SelectedValue = "30";
        }

        idRow.Visible = false;


        btnSubmit.Text = "Add " + type;
        btnCancel.Visible = true;
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (GetUrlParamType() == UrlParamType.Edit)
        {
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
            return;
        }

        // close this window
        Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {

        if (GetUrlParamType() == UrlParamType.View)
        {
            maintable.Visible = false; // hide this so that we don't send all the page data (all suburbs, etc) to display before it redirects
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "edit"));
        }
        else if (GetUrlParamType() == UrlParamType.Edit)
        {
            if (!IsValidFormID())
            {
                HideTableAndSetErrorMessage();
                return;
            }
            ContactAus contact = ContactAusDB.GetByID(GetFormID());
            if (contact == null)
            {
                HideTableAndSetErrorMessage("Invalid contact ID");
                return;
            }

            bool isAddress  = contact.ContactType.ContactTypeGroup.ID == 1;
            bool isTelecoms = contact.ContactType.ContactTypeGroup.ID == 2;
            bool isBedroom  = contact.ContactType.ContactTypeGroup.ID == 3;
            bool isWeb      = contact.ContactType.ContactTypeGroup.ID == 4;
            bool isMobile = Convert.ToInt32(ddlContactType.SelectedValue) == 30;
            bool isPOBox = Convert.ToInt32(ddlContactType.SelectedValue) == 37 || Convert.ToInt32(ddlContactType.SelectedValue) == 262;

            bool isEmail = Convert.ToInt32(ddlContactType.SelectedValue) == 27;
            bool isWebsite = Convert.ToInt32(ddlContactType.SelectedValue) == 28;

            txtAddrLine1.Text = txtAddrLine1.Text.Trim();
            if (isMobile && !System.Text.RegularExpressions.Regex.Replace(txtAddrLine1.Text, "[^0-9]", "").StartsWith("0"))
            {
                SetErrorMessage("Mobile number must start with 0");
                return;
            }
            if (isTelecoms && System.Text.RegularExpressions.Regex.Replace(txtAddrLine1.Text, "[^0-9]", "").Length > 13)
            {
                SetErrorMessage("Phone number can not be more than 13 digits");
                return;
            }
            if (isEmail && !Utilities.IsValidEmailAddress(txtAddrLine1.Text))
            {
                SetErrorMessage("Invalid email address");
                return;
            }
            if (isWebsite && !Utilities.IsValidWebURL(txtAddrLine1.Text))
            {
                SetErrorMessage("Invalid website");
                return;
            }
            if (isPOBox && !Regex.IsMatch(txtAddrLine1.Text, "PO Box", RegexOptions.IgnoreCase) && !Regex.IsMatch(txtAddrLine2.Text, "PO Box", RegexOptions.IgnoreCase))
            {
                SetErrorMessage("The address text must contain \"PO Box\"");
                return;
            }




            ContactAusDB.Update(Convert.ToInt32(lblId.Text),
                Convert.ToInt32(ddlContactType.SelectedValue),
                txtFreeText.Text,
                isTelecoms ? System.Text.RegularExpressions.Regex.Replace(txtAddrLine1.Text, "[^0-9]", "") : txtAddrLine1.Text,
                txtAddrLine2.Text,
                txtStreet.Text,
                isAddress ? Convert.ToInt32(ddlAddressChannelType.SelectedValue) : (contact.AddressChannelType == null ? -1 : contact.AddressChannelType.ID),
                //isAddress ? Convert.ToInt32(ddlSuburb.SelectedValue)             : (contact.Suburb             == null ? -1 : contact.Suburb.SuburbID),
                isAddress ? Convert.ToInt32(suburbID.Value) : (contact.Suburb == null ? -1 : contact.Suburb.SuburbID),
                isAddress ? Convert.ToInt32(ddlCountry.SelectedValue) : (contact.Country == null ? -1 : contact.Country.ID),
                contact.Site == null ? Convert.ToInt32(Session["SiteID"]) : contact.Site.SiteID,
                isAddress || isWeb ? chkIsBilling.Checked    : contact.IsBilling,
                isAddress || isWeb ? chkIsNonBilling.Checked : contact.IsNonBilling);



            //close this window
            maintable.Visible = false; // hide this so that we don't send all the page data (all suburbs, etc) to display before it closes

            bool refresh_on_close = Request.QueryString["refresh_on_close"] != null && Request.QueryString["refresh_on_close"] == "1";

            if (refresh_on_close)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.opener.location.href=window.opener.location.href;self.close();</script>");
            else
                Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
        }
        else if (GetUrlParamType() == UrlParamType.Add)
        {
            if (!IsValidFormID())
            {
                HideTableAndSetErrorMessage();
                return;
            }

            int entityID = GetFormID();
            if (!EntityDB.IDExists(entityID))
            {
                HideTableAndSetErrorMessage("Invalid entity ID");
                return;
            }


            int contactTypeGroupID = -1;
            UrlParamContactTypeGroup urlParamContactTypeGroup = GetUrlParamContactTypeGroup();
            if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Mailing)
                contactTypeGroupID = 1;
            else if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Telecoms)
                contactTypeGroupID = 2;
            else if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Bedroom)
                contactTypeGroupID = 3;
            else if (urlParamContactTypeGroup == UrlParamContactTypeGroup.Internet)
                contactTypeGroupID = 4;
            else
            {
                HideTableAndSetErrorMessage("Invalid contact_group_type ID");
                return;
            }


            bool isAddress  = contactTypeGroupID == 1;
            bool isTelecoms = contactTypeGroupID == 2;
            bool isBedroom  = contactTypeGroupID == 3;
            bool isWeb      = contactTypeGroupID == 4;
            bool isMobile   = Convert.ToInt32(ddlContactType.SelectedValue) == 30;
            bool isPOBox    = Convert.ToInt32(ddlContactType.SelectedValue) == 37 || Convert.ToInt32(ddlContactType.SelectedValue) == 262;

            bool isEmail = Convert.ToInt32(ddlContactType.SelectedValue) == 27;
            bool isWebsite = Convert.ToInt32(ddlContactType.SelectedValue) == 28;

            txtAddrLine1.Text = txtAddrLine1.Text.Trim();
            if (isMobile && !System.Text.RegularExpressions.Regex.Replace(txtAddrLine1.Text, "[^0-9]", "").StartsWith("0"))
            {
                SetErrorMessage("Mobile number must start with 0");
                return;
            }
            if (isTelecoms && System.Text.RegularExpressions.Regex.Replace(txtAddrLine1.Text, "[^0-9]", "").Length > 13)
            {
                SetErrorMessage("Phone number can not be more than 13 digits");
                return;
            }
            if (isEmail && !Utilities.IsValidEmailAddress(txtAddrLine1.Text))
            {
                SetErrorMessage("Invalid email address");
                return;
            }
            if (isWebsite && !Utilities.IsValidWebURL(txtAddrLine1.Text))
            {
                SetErrorMessage("Invalid website");
                return;
            }
            if (isPOBox && !Regex.IsMatch(txtAddrLine1.Text, "PO Box", RegexOptions.IgnoreCase) && !Regex.IsMatch(txtAddrLine2.Text, "PO Box", RegexOptions.IgnoreCase))
            {
                SetErrorMessage("The address text must contain \"PO Box\"");
                return;
            }

            int contactID = ContactAusDB.Insert(entityID,
                Convert.ToInt32(ddlContactType.SelectedValue),
                txtFreeText.Text,
                isTelecoms ? System.Text.RegularExpressions.Regex.Replace(txtAddrLine1.Text, "[^0-9]", "") : txtAddrLine1.Text,
                txtAddrLine2.Text,
                txtStreet.Text,
                isAddress ? Convert.ToInt32(ddlAddressChannelType.SelectedValue) : -1,
                //isAddress ? Convert.ToInt32(ddlSuburb.SelectedValue)           : -1,
                isAddress ? Convert.ToInt32(suburbID.Value) : -1,
                isAddress ? Convert.ToInt32(ddlCountry.SelectedValue) : -1,
                Convert.ToInt32(Session["SiteID"]),
                isAddress || isWeb ? chkIsBilling.Checked    : true,
                isAddress || isWeb ? chkIsNonBilling.Checked : true);


            // close this window
            maintable.Visible = false; // hide this so that we don't send all the page data (all suburbs, etc) to display before it closes

            bool refresh_on_close = Request.QueryString["refresh_on_close"] != null && Request.QueryString["refresh_on_close"] == "1";

            if (refresh_on_close)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.opener.location.href=window.opener.location.href;self.close();</script>");
            else
                Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
        }
        else
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
        }
    }

    #region btnSuburbSelectionUpdate_Click

    protected void btnSuburbSelectionUpdate_Click(object sender, EventArgs e)
    {
        UpdateSuburbInfo(true);
    }

    protected void UpdateSuburbInfo(bool redirect)
    {
        return;

        int newSuburbID = Convert.ToInt32(suburbID.Value);

        if (newSuburbID == -1)
        {
            lblSuburbText.Text = "--";
        }
        else
        {
            Suburb suburb = SuburbDB.GetByID(newSuburbID);
            lblSuburbText.Text = suburb.Name + ", " + suburb.State + "(" + suburb.Postcode + ")";
        }

        if (redirect)
        {
            string url = Request.RawUrl;
            url = UrlParamModifier.Update(newSuburbID != -1, url, "suburb", newSuburbID == -1 ? "" : newSuburbID.ToString());
            Response.Redirect(url);
        }
    }

    #endregion


    #region SetErrorMessage, HideErrorMessage

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