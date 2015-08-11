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

public partial class StaffOfferingsBulkUpdateV2 : System.Web.UI.Page
{

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
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, true, false, false, true);
                SetUpGUI();
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

    #region SetUpGUI

    protected void SetUpGUI()
    {
        ddlNewStaff.Items.Clear();
        ddlNewStaff.Items.Add(new ListItem("All Staff", "-1"));
        DataTable dtStaff = StaffDB.GetDataTable();
        for (int i = 0; i < dtStaff.Rows.Count; i++)
            if (!Convert.ToBoolean(dtStaff.Rows[i]["staff_is_fired"]) && Convert.ToBoolean(dtStaff.Rows[i]["staff_is_provider"]))
                ddlNewStaff.Items.Add(new ListItem(dtStaff.Rows[i]["person_firstname"].ToString() + " " + dtStaff.Rows[i]["person_surname"].ToString(), dtStaff.Rows[i]["staff_staff_id"].ToString()));

        ddlNewOffering.Style["max-width"] = "250px";
        ddlNewOffering.Items.Clear();
        ddlNewOffering.Items.Add(new ListItem("All Offerings", "-1"));
        DataTable dtOfferings = OfferingDB.GetDataTable(false, "1,3", "63,89");
        for (int i = 0; i < dtOfferings.Rows.Count; i++)
            if (!Convert.ToBoolean(dtOfferings.Rows[i]["o_is_deleted"]))
                ddlNewOffering.Items.Add(new ListItem(dtOfferings.Rows[i]["o_name"].ToString(), dtOfferings.Rows[i]["o_offering_id"].ToString()));

        txtNewActiveDate.Text = DateTime.Today.ToString("dd-MM-yyyy");
    }

    #endregion

    #region GetDate, IsValidDate, ValidDateCheck

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
    protected void ValidDateCheck(object sender, ServerValidateEventArgs e)
    {
        try
        {
            TextBox txtDate = txtNewActiveDate;

            if (!IsValidDate(txtDate.Text))
                throw new Exception();

            DateTime d = GetDate(txtDate.Text);
            e.IsValid = true;
        }
        catch (Exception)
        {
            e.IsValid = false;
        }
    }

    #endregion

    protected void btnUpdate_Click(object sender, EventArgs e)
    {

        if (chkNewIsCommission.Checked && chkNewIsFixedRate.Checked)
        {
            SetErrorMessage("Can not be both comission and fixed rate. Please select only one.");
            return;
        }

        StaffOfferingsDB.UpdateBulk(Convert.ToInt32(ddlNewStaff.SelectedValue), Convert.ToInt32(ddlNewOffering.SelectedValue), 
            chkNewIsCommission.Checked, Convert.ToDecimal(txtNewCommissionPercent.Text), chkNewIsFixedRate.Checked, Convert.ToDecimal(txtNewFixedRate.Text), 
            GetDate(txtNewActiveDate.Text));

        SetErrorMessage("Updated.");
    }

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