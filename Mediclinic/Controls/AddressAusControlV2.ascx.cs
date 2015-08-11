using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text.RegularExpressions;


public partial class Controls_AddressAusControlV2 : System.Web.UI.UserControl
{

    protected int EntityID
    {
        get { return addressControlEntityID.Value == "" ? -1 : Convert.ToInt32(addressControlEntityID.Value); }
        set { addressControlEntityID.Value = value.ToString(); }
    }
    protected bool Enabled
    {
        get { return addressControlEnabled.Value == "" ? false : Convert.ToBoolean(addressControlEnabled.Value); }
        set { addressControlEnabled.Value = value.ToString(); }
    }
    protected EntityType EntityType
    {
        get { return EntityType.GetByString(addressEntityType.Value); }
        set { addressEntityType.Value = EntityType.GetByType(value.Type).String; }
    }
    protected bool IncBedrooms
    {
        get { return addressIncBedrooms.Value == "" ? false : Convert.ToBoolean(addressIncBedrooms.Value); }
        set { addressIncBedrooms.Value = value.ToString(); }
    }


    public void Set(int entity_id, bool enabled, EntityType entity_type, bool incBedrooms = false)
    {
        this.EntityID   = entity_id;
        this.Enabled    = enabled;
        this.EntityType = entity_type;
        this.IncBedrooms = incBedrooms;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Initialise();
    }

    protected void Initialise()
    {
        if (!this.Enabled || this.EntityID == -1)
            return;

        SetGUI();
        UpdateTables();
    }

    protected void btnUpdateAddresses_Click(object sender, EventArgs e)
    {
        //
        // the update causes page validation problems ... just redirect ... and this method not called with adding addresses from line 10 lines below (dont know why), so just redirect via js
        //

        //UpdateTables();
        Response.Redirect(Request.RawUrl);
    }

    #region SetGUI()

    protected void SetGUI()
    {
        bool isMobileDevice = Utilities.IsMobileDevice(Request);

        string entity_type = this.EntityType.Type == EntityType.EntityTypeEnum.None ? "" : "&entity_type=" + this.EntityType.String;


        string jsAddAddress;
        string jsAddPhoneNums;
        string jsAddEmails;
        string jsAddBedrooms;

        if (!isMobileDevice)
        {
            string allFeaturesAddAddress = "dialogWidth:525px;dialogHeight:470px;center:yes;resizable:no; scroll:no";
            jsAddAddress     = "javascript:window.showModalDialog('ContactAusDetailV2.aspx?type=add" + entity_type + "&id=" + this.EntityID.ToString() + "&contact_type_group=1', '', '" + allFeaturesAddAddress + "'); window.location.href = window.location.href;return false;";

            string allFeaturesOther = "dialogWidth:460px;dialogHeight:340px;center:yes;resizable:no; scroll:no";
            jsAddPhoneNums   = "javascript:window.showModalDialog('ContactAusDetailV2.aspx?type=add" + entity_type + "&id=" + this.EntityID.ToString() + "&contact_type_group=2', '', '" + allFeaturesOther + "'); window.location.href = window.location.href;;return false;";
            jsAddEmails      = "javascript:window.showModalDialog('ContactAusDetailV2.aspx?type=add" + entity_type + "&id=" + this.EntityID.ToString() + "&contact_type_group=4', '', '" + allFeaturesOther + "'); window.location.href = window.location.href;return false;";
            jsAddBedrooms    = "javascript:window.showModalDialog('ContactAusDetailV2.aspx?type=add" + entity_type + "&id=" + this.EntityID.ToString() + "&contact_type_group=3', '', '" + allFeaturesOther + "'); window.location.href = window.location.href;return false;";
        }
        else
        {
            jsAddAddress     = "open_new_tab('ContactAusDetailV2.aspx?type=add" + entity_type + "&id=" + this.EntityID.ToString() + "&contact_type_group=1&refresh_on_close=1'); return false;";
            jsAddPhoneNums   = "open_new_tab('ContactAusDetailV2.aspx?type=add" + entity_type + "&id=" + this.EntityID.ToString() + "&contact_type_group=2&refresh_on_close=1'); return false;";
            jsAddEmails      = "open_new_tab('ContactAusDetailV2.aspx?type=add" + entity_type + "&id=" + this.EntityID.ToString() + "&contact_type_group=4&refresh_on_close=1'); return false;";
            jsAddBedrooms    = "open_new_tab('ContactAusDetailV2.aspx?type=add" + entity_type + "&id=" + this.EntityID.ToString() + "&contact_type_group=3&refresh_on_close=1'); return false;";
        }

        this.lnkAddAddress.Visible = true;
        this.lnkAddAddress.NavigateUrl = " ";
        this.lnkAddAddress.Text = "+";
        this.lnkAddAddress.Attributes["onclick"] = jsAddAddress;
        this.lnkAddAddress.Style["text-decoration"] = "none";

        this.lnkAddPhoneNums.Visible = true;
        this.lnkAddPhoneNums.NavigateUrl = "  ";
        this.lnkAddPhoneNums.Text = "+";
        this.lnkAddPhoneNums.Attributes["onclick"] = jsAddPhoneNums;
        this.lnkAddPhoneNums.Style["text-decoration"] = "none";

        this.lnkAddEmails.Visible = true;
        this.lnkAddEmails.NavigateUrl = "  ";
        this.lnkAddEmails.Text = "+";
        this.lnkAddEmails.Attributes["onclick"] = jsAddEmails;
        this.lnkAddEmails.Style["text-decoration"] = "none";

        this.lnkAddBedrooms.Visible = true;
        this.lnkAddBedrooms.NavigateUrl = "  ";
        this.lnkAddBedrooms.Text = "+";
        this.lnkAddBedrooms.Attributes["onclick"] = jsAddBedrooms;
        this.lnkAddBedrooms.Style["text-decoration"] = "none";

        this.tblBedrooms.Visible = this.IncBedrooms;
        this.phBedrooms.Visible = this.IncBedrooms;

    }

    #endregion


    #region UpdateTables()

    protected void UpdateTables()
    {
        DataTable dt = ContactAusDB.GetDataTable_ByEntityID(-1, this.EntityID);

        CreateTable(phAddresses, dt.Select("atg_contact_type_group_id=1"), 525, 500);
        CreateTable(phPhoneNums, dt.Select("atg_contact_type_group_id=2"), 450, 365);
        CreateTable(phBedrooms , dt.Select("atg_contact_type_group_id=3"), 450, 365);
        CreateTable(phEmails   , dt.Select("atg_contact_type_group_id=4"), 450, 365);
        //this.CreateTableEmails   (dt.Select("ad_contact_type_id=27"));
    }
    protected void CreateTable(PlaceHolder placeHolder, DataRow[] rows, int width, int height)
    {
        placeHolder.Controls.Clear();
        Table t = new Table();
        placeHolder.Controls.Add(t);
        for (int i = 0; i < rows.Length; i++)
        {
            ContactAus contact = ContactAusDB.LoadAll(rows[i]);
            if (contact.FreeText.Trim().Length > 0)
            {
                t.Rows.Add(CreateNewRow(contact, width, height, true, true));
                t.Rows.Add(CreateNewRow(contact, width, height, true, false));
            }
            else
            {
                t.Rows.Add(CreateNewRow(contact, width, height, false, false));
            }
        }
    }
    protected TableRow CreateNewRow(ContactAus contact, int width, int height, bool hasFreeTextRow = false, bool isFreeTextRow = false)
    {
        bool isMobileDevice = Utilities.IsMobileDevice(Request);

        bool isAddress = contact.ContactType.ContactTypeGroup.ID == 1;
        bool isPhone   = contact.ContactType.ContactTypeGroup.ID == 2;
        bool isBedroom = contact.ContactType.ContactTypeGroup.ID == 3;

        bool isEmail   = contact.ContactType.ContactTypeID == 27;
        bool isWebsite = contact.ContactType.ContactTypeID == 28;

        int  rowspan       = !hasFreeTextRow ? 1 : (hasFreeTextRow && isFreeTextRow ? 2 : 0);
        bool showFreeText  = hasFreeTextRow && isFreeTextRow;


        TableRow tr = new TableRow();

        if (rowspan > 0)
        {
            tr.Cells.Add(NewCell("[", false, 1, rowspan));

            string onclick;

            if (!isMobileDevice)
            {
                string allFeatures = "dialogWidth:" + width + "px;dialogHeight:" + height + "px;center:yes;resizable:no; scroll:no";
                // this was causing the outer page to validate its controls for no reason I can understand, so I just changed it to refresh the page
                //string onclick     = "onclick=\"javascript:window.showModalDialog('ContactAusDetailV2.aspx?type=edit&id=" + contact.ContactID.ToString() + "', '', '" + allFeatures + "');document.getElementById('" + btnUpdateAddresses.ClientID + "').click();return false;\"";
                onclick = "onclick=\"javascript:window.showModalDialog('ContactAusDetailV2.aspx?type=edit&id=" + contact.ContactID.ToString() + "', '', '" + allFeatures + "');window.location.href=window.location.href;return false;\"";
            }
            else
            {
                onclick = "onclick=\"open_new_tab('ContactAusDetailV2.aspx?type=edit&id=" + contact.ContactID.ToString() + "&refresh_on_close=1');return false;\"";
            }

            string href = "<a style=\"text-decoration: none\" title=\"Edit\" AlternateText=\"Edit\" " + onclick + " href=\"#\"><img src=\"/images/edit-icon-10.png\"></a>";

            tr.Cells.Add(NewCell(href, false, 1, rowspan));

            TableCell   cell = new TableCell();
            ImageButton btn = new ImageButton() { 
                ID              = "btnContact_" + contact.ContactID.ToString(),
                ImageUrl        = "~/images/Delete-icon-10.png",
                CommandArgument = contact.ContactID.ToString(),
                OnClientClick   = "javascript:if (!confirm('Are you sure you want to delete this?')) return false;",
                ToolTip         = "Delete"
            };
            btn.Command += new CommandEventHandler(DeleteContact_Command);
            btn.Style["text-decoration"] = "none";
            cell.Controls.Add(btn);
            if (rowspan > 1) cell.RowSpan = rowspan;
            tr.Cells.Add(cell);

            tr.Cells.Add(NewCell("]", false, 1, rowspan));
            tr.Cells.Add(NewCell("&nbsp;", false, 1, rowspan));
        }


        if (isAddress)
        {
            if (showFreeText)
            {
                tr.Cells.Add(NewCell(contact.FreeText + ":", false, 5));
            }
            else
            {
                tr.Cells.Add(NewCell(contact.AddrLine1, true));
                tr.Cells.Add(NewCell("&nbsp;"));
                tr.Cells.Add(NewCell(contact.StreetName + (contact.AddressChannelType == null ? "" : (contact.StreetName.Length == 0 ? "" : " ") + contact.AddressChannelType.Descr), true));
                tr.Cells.Add(NewCell("&nbsp;"));
                tr.Cells.Add(NewCell(contact.Suburb != null ? contact.Suburb.Name : "", true));
            }
        }
        else if (isBedroom)
        {
            if (showFreeText)
            {
                tr.Cells.Add(NewCell(contact.FreeText + ":", false, 3));
            }
            else
            {
                tr.Cells.Add(NewCell(contact.AddrLine1, true));
            }
        }
        else
        {
            if (showFreeText)
            {
                tr.Cells.Add(NewCell(contact.FreeText + ":", false, 3));
            }
            else
            {
                if (isPhone)
                    tr.Cells.Add(NewCell(Utilities.FormatPhoneNumber(contact.AddrLine1), true));
                else if (isEmail)
                {
                    bool updateOnClose = false;

                    /*
                    string allFeaturesEmail = "dialogWidth:675px;dialogHeight:675px;center:yes;resizable:no; scroll:no";
                    string onclickEmail = "onclick=\"javascript:window.showModalDialog('SendEmail.aspx?&id=" + contact.ContactID.ToString() + "', '', '" + allFeaturesEmail + "');" + (updateOnClose ? "document.getElementById('" + btnUpdateAddresses.ClientID + "').click();" : "") + "return false;\"";
                    string hrefEmail = "<a style=\"text-decoration: none\" title=\"Edit\" AlternateText=\"Edit\" " + onclickEmail + " href=\"\">" + contact.AddrLine1 + "</a>";
                    tr.Cells.Add(NewCell(hrefEmail, true));
                    */

                    tr.Cells.Add(NewCell(contact.AddrLine1, true));
                }
                else if (isWebsite)
                {
                    string allFeaturesEmail = ""; // "dialogWidth:675px;dialogHeight:600px;center:yes;resizable:no; scroll:no";
                    string onclickEmail = "onclick=\"window.open('" + (contact.AddrLine1.StartsWith("http://") || contact.AddrLine1.StartsWith("https://") ? "" : "http://") + contact.AddrLine1 + "', '', '" + allFeaturesEmail + "');" + "return false;\"";
                    string hrefEmail = "<a style=\"text-decoration: none\" " + onclickEmail + " href=\"\">" + contact.AddrLine1 + "</a>";
                    tr.Cells.Add(NewCell(hrefEmail, true));
                }
                else
                    tr.Cells.Add(NewCell(contact.AddrLine1, true));

                tr.Cells.Add(NewCell("&nbsp;&nbsp;&nbsp;"));
                tr.Cells.Add(NewCell("(" + contact.ContactType.Descr + ")", true));
            }
        }

        return tr;
    }
    protected TableCell NewCell(string text, bool nowrap = false, int colspan = 1, int rowspan = 1)
    {
        TableCell tc = new TableCell();
        if (nowrap)
            tc.CssClass = "nowrap";
        if (colspan > 1)
            tc.ColumnSpan = colspan;
        if (rowspan > 1)
            tc.RowSpan = rowspan;
        Label lbl = new Label();
        lbl.Text = text;
        tc.Controls.Add(lbl);
        return tc;
    }

    void DeleteContact_Command(object sender, CommandEventArgs e)
    {
        int contactID = Convert.ToInt32(e.CommandArgument);
        ContactAusDB.UpdateInactive(contactID);

        //
        // the update causes the page with this control to validate to run and I can't find why ... just redirect
        //

        //UpdateTables();
        Response.Redirect(Request.RawUrl);


        // call the main page to redirect ??
        // pass in EventHandler like in InvoiceItemsControl.ascx.cs    ??
        // but need fucking update code on all sites ffs
    }

    #endregion

}