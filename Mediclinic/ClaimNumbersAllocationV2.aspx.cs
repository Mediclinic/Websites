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

public partial class ClaimNumbersAllocationV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, true, false, false, false, false, true);
                ResetCheckboxes();
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


    protected void btnUpdate_Click(object sender, EventArgs e)
    {

        int startTime = Environment.TickCount; 

        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        for (int i = 0; i < claimNumberAllocationChkBoxList.Items.Count; i++)
        {
            ListItem li         = claimNumberAllocationChkBoxList.Items[i];
            string   range      = li.Value;
            string   startRange = range.Split('_')[0];
            string   endRange   = range.Split('_')[1];

            string _sql = @"UPDATE InvoiceHealthcareClaimNumbers SET is_active = " + (li.Selected ? "1" : "0") + "   WHERE claim_number >= '" + startRange + @"' AND claim_number <= '" + endRange + @"';";
            sql.AppendLine(_sql);
        }

        DBBase.ExecuteNonResult(sql.ToString());

        string result1 = "Update Time: " + ((double)(Environment.TickCount - startTime) / 1000.0) + " seconds";


        startTime = Environment.TickCount; 

        ResetCheckboxes();

        string result2 = "Reload Time: " + ((double)(Environment.TickCount - startTime) / 1000.0) + " seconds";


        lblLoadTimeMessage.Text = result1 + ", &nbsp;&nbsp;&nbsp;&nbsp;" + result2;
        lblLoadTimeMessage.Visible = true;

    }

    protected void ResetCheckboxes()
    {
        int startTime = Environment.TickCount; 

        ResetCheckboxes1();
        //ResetCheckboxes2();  // much slower

        lblLoadTimeMessage.Text = "Load Time: " + ((double)(Environment.TickCount - startTime) / 1000.0) + " seconds";
        lblLoadTimeMessage.Visible = true;
    }

    protected void ResetCheckboxes1()
    {
        System.Text.StringBuilder sql = new System.Text.StringBuilder();

        sql.AppendLine("SELECT ");

        for (int i = 0; i < claimNumberAllocationChkBoxList.Items.Count; i++)
        {
            ListItem li         = claimNumberAllocationChkBoxList.Items[i];
            string   range      = li.Value;
            string   startRange = range.Split('_')[0];
            string   endRange   = range.Split('_')[1];

            string _sql = @"SELECT CASE WHEN EXISTS (SELECT * FROM InvoiceHealthcareClaimNumbers WHERE claim_number >= '" + startRange + @"' AND claim_number <= '" + endRange + @"' AND is_active = 1) THEN 1 ELSE 0 END";
            sql.AppendLine("(" + _sql + ") AS " + range + (i < claimNumberAllocationChkBoxList.Items.Count - 1 ? "," : " "));
        }

        System.Data.DataTable tbl = DBBase.ExecuteQuery(sql.ToString()).Tables[0];

        for (int i = 0; i < claimNumberAllocationChkBoxList.Items.Count; i++)
        {
            ListItem li         = claimNumberAllocationChkBoxList.Items[i];
            string   range      = li.Value;
            string   startRange = range.Split('_')[0];
            string   endRange   = range.Split('_')[1];

            bool contains = Convert.ToBoolean(tbl.Rows[0][range]);

            li.Selected = contains;
            li.Attributes.Add("style", li.Selected ? "font-weight:bold;background-color:yellow;" : "");
        }
    }

    protected void ResetCheckboxes2()
    {
        for (int i = 0; i < claimNumberAllocationChkBoxList.Items.Count; i++)
        {
            ListItem li         = claimNumberAllocationChkBoxList.Items[i];
            string   range      = li.Value;
            string   startRange = range.Split('_')[0];
            string   endRange   = range.Split('_')[1];

            string sql = @"SELECT CASE WHEN EXISTS (SELECT * FROM InvoiceHealthcareClaimNumbers WHERE claim_number >= '" + startRange + @"' AND claim_number <= '" + endRange + @"' AND is_active = 1) THEN 1 ELSE 0 END;";
            bool contains = Convert.ToBoolean(DBBase.ExecuteSingleResult(sql));
            li.Selected = contains;
            li.Attributes.Add("style", li.Selected ? "font-weight:bold;background-color:yellow;" : "");
        }
    }


    #region HideTableAndSetErrorMessage, SetErrorMessage, HideErrorMessag

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
        btnUpdate.Visible = false;
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
        lblLoadTimeMessage.Visible = false;
        lblLoadTimeMessage.Text = "";
    }

    #endregion

}