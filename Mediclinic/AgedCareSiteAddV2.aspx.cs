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
using System.IO;

public partial class AgedCareSiteAddV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        PagePermissions.EnforcePermissions_RequireAll(Session, Response, true, false, false, false, false, true);

        if (!IsPostBack)
            SetupGUI();

        if (SiteDB.GetSiteByType(SiteDB.SiteType.AgedCare) != null)
            HideTableAndSetErrorMessage("Aged Care Site Already Exists");
    }

    protected void SetupGUI()
    {
        txtACSiteName.Focus();
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (SiteDB.GetSiteByType(SiteDB.SiteType.AgedCare) != null)
        {
            HideTableAndSetErrorMessage("Aged Care Site Already Exists");
            return;
        }


        string errMsg = string.Empty;

        txtACSiteName.Text = Utilities.FormatName(txtACSiteName.Text.Trim());

        if (txtACSiteName.Text == string.Empty)
            errMsg += (errMsg.Length == 0 ? "" : "<br />") + "Site Name is required";

        if (errMsg.Length > 0)
        {
            SetErrorMessage(errMsg);
            return; 
        }



        try
        {

            string sql = @"

Declare @entity_id int

INSERT INTO Entity DEFAULT VALUES;
SET @entity_id = SCOPE_IDENTITY()

SET IDENTITY_INSERT Site ON
INSERT INTO Site
	(
		site_id,
		entity_id,
		name,
		abn,
		acn,
		tfn,
		asic,
		is_provider,
		bank_bpay,
		bank_bsb,
		bank_account,
		bank_direct_debit_userid,
		bank_username,
		oustanding_balance_warning,
		print_epc,
		excl_sun,
		excl_mon,
		excl_tue,
		excl_wed,
		excl_thu,
		excl_fri,
		excl_sat,
		day_start_time,
		lunch_start_time,
		lunch_end_time,
		day_end_time,
		fiscal_yr_end,
		num_booking_months_to_get,
		site_type_id
	) 
    VALUES
    (
		2,
		@entity_id,
		'" + txtACSiteName.Text + @"',
		'',
		'',
		'',
		'',
		0,
		'',
		'',
		'',
		'',
		'',
		0.00,
		1,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		'08:00:00.0000000',
		'00:00:00.0000000',
		'00:00:00.0000000',
		'22:00:00.0000000',
		'2014-06-30 00:00:00.000',
		54,
		2
    )
    
SET IDENTITY_INSERT Site OFF

";

            DBBase.ExecuteNonResult(sql);

            HideTableAndSetErrorMessage("Site Added!<br /><br /><font color=\"blue\"><center><table style=\"text-align:left;max-width:520px;\"><tr><td><ol><li>Logout and then log back in so the 'change site' link is showing</li><li>Go into the new AC site</li><li>Go to menu <b><u>Letters</u></b> ⇨ <b><u>Treatment Letters</u></b></li><li>Add a row for each profession until the row where you add is no longer showing (indicating there exists a row for all professions already)<br /><br /></li><li>You may also need to add <u>AC invoice</u> and <u>AC Overdue Invoice</u> templates in the letters page</li></ol></td></tr></table></center></font>");
            SetupGUI();

        }
        catch (CustomMessageException ex)
        {
            SetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            SetErrorMessage("", ex.ToString());
        }

    }



    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        main_table.Visible = false;
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