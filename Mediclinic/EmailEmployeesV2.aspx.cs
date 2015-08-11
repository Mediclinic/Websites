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
using System.Text;

public partial class EmailEmployeesV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = string.Empty;

            // need to be admin+ to see this page
            PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, false, false, false);


            if (!IsPostBack)
            {
                string fromName  = ((SystemVariables)Session["SystemVariables"])["Email_FromName"].Value;

                FreeTextBox1.Text = @"<center>
<font color='#2E2E2E'>
<h1>Newsletter</h1>
<img src='https://portal.mediclinic.com.au/imagesV2/comp_logo.png'><br>
<br>
<div style=""display: inline-block;min-width:450px;text-align:left;"">

        <br>
        <br>
        <br>
        Best regards,<br>
        " + fromName + @"<br>
        <br>

</div>
</font>
</center>

";
                EmailAllUsers(true, true);
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
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"javascript:void(0)\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

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


    protected void btnSendEmail_Click(object sender, EventArgs e)
    {
        EmailAllUsers(false, false);
    }
    protected void btnPreviewEmail_Click(object sender, EventArgs e)
    {
        EmailAllUsers(true, false);
    }
    protected void EmailAllUsers(bool previewOnly, bool showEmailsOnly)
    {
        bool IsDebug = Utilities.IsDev();

        try
        {
            txtSubject.Text = txtSubject.Text.Trim();
            if (!showEmailsOnly && txtSubject.Text.Length == 0)
            {
                SetErrorMessage("No Subject Entered");
                return;
            }
            if (!showEmailsOnly && FreeTextBox1.Text.Trim().Length == 0)
            {
                SetErrorMessage("No Email Text Entered");
                return;
            }


            string fromEmail = IsDebug ? "eli.pollak@mediclnic.com.au" : ((SystemVariables)System.Web.HttpContext.Current.Session["SystemVariables"])["Email_FromEmail"].Value;
            string fromName  = ((SystemVariables)Session["SystemVariables"])["Email_FromName"].Value;


            ArrayList emailInfoList = new ArrayList(); // list of Tuple<site, person, email>

            DataTable tblStaff = StaffDB.GetDataTable();

            // get entity ID's so we can get all emails into a hashtable in one db query
            int[] entityIDs = new int[tblStaff.Rows.Count];
            for (int j = 0; j < tblStaff.Rows.Count; j++)
                entityIDs[j] = Convert.ToInt32(tblStaff.Rows[j]["person_entity_id"]);
            Hashtable emailHash = PatientsContactCacheDB.GetBullkEmail(entityIDs, -1);

            // add the emails to the datatable - as comma-seperated string
            tblStaff.Columns.Add("database_name", typeof(String));
            tblStaff.Columns.Add("emails", typeof(String));
            tblStaff.Columns.Add("site", typeof(String));
            for (int j = 0; j < tblStaff.Rows.Count; j++)
            {
                Staff s = StaffDB.LoadAll(tblStaff.Rows[j]);

                if (emailHash[s.Person.EntityID] != null)
                {
                    if (Utilities.GetAddressType().ToString() == "Contact")
                    {
                        if (emailHash[s.Person.EntityID] != null)
                            foreach (Contact c in (Contact[])emailHash[s.Person.EntityID])
                                if (Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                                    emailInfoList.Add(new Tuple<string, string>(s.Person.FullnameWithoutMiddlename, c.AddrLine1.Trim()));
                    }
                    else if (Utilities.GetAddressType().ToString() == "ContactAus")
                    {
                        if (emailHash[s.Person.EntityID] != null)
                            foreach (ContactAus c in (ContactAus[])emailHash[s.Person.EntityID])
                                if (Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                                    emailInfoList.Add(new Tuple<string, string>(s.Person.FullnameWithoutMiddlename, c.AddrLine1.Trim()));
                    }
                    else
                        throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
                }
            }

            
            // send the email

            string output = showEmailsOnly ? "" :
                "<h4>" + (previewOnly ? "Message Preview" : "Message Sent") + "</h4>" + Environment.NewLine +
                "<table style=\"min-width:400px;border:1px solid black;text-align:center;background-color:#FFFFFF;border-spacing:2px;border-collapse:separate;\"><tr><td><b>" + txtSubject.Text + "</b></td></tr></table><div style=\"height:10px;\"></div>" + Environment.NewLine +
                "<table style=\"min-width:500px;border:1px solid black;text-align:left;background-color:#FFFFFF;border-spacing:10px;border-collapse:separate;\"><tr style=\"min-height:200px;\"><td>" + (FreeTextBox1.Text.Length == 0 ? "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" : FreeTextBox1.Text) + "</td></tr></table><br />";

            output += "<h4>" + (previewOnly ? "Will Be Sent To" : "Sent To") + "</h4><table border=\"1\" class=\"table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center\" >";
            string emails = string.Empty;

            foreach (Tuple<string, string> emailInfo in emailInfoList)
            {
                output += "<tr><td style=\"padding-left:4px;padding-right:4px;text-align:left !important;\">" + emailInfo.Item1 + "</td><td style=\"padding-left:4px;padding-right:4px;text-align:left !important;\">" + emailInfo.Item2 + "</td></tr>";
                emails = (emails.Length == 0 ? "" : ",") + emailInfo.Item2; 
            }
            if (emailInfoList.Count == 0)
                output += "<tr><td style=\"padding-left:4px;padding-right:4px;text-align:left !important;\">&nbsp;No Staff Have Emails In The Selected Site(s)&nbsp;</td></tr>";
            output += "</table>";



            if (!previewOnly && emails.Length > 0)
            {
                // email: put to addresss as from address
                // email: put all emails in BCC
                EmailerNew.SimpleEmail(
                    fromEmail,
                    fromName,
                    fromEmail,
                    txtSubject.Text.Trim(),
                    FreeTextBox1.Text,
                    true,
                    null,
                    false,
                    null,
                    IsDebug ? "eli@elipollak.com" : emails
                    );
            }

            lblEmailOutput.Text = output;

            if (!showEmailsOnly)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "download", @"<script language=javascript>addLoadEvent(function () { window.location.hash = ""emailing_tag""; });</script>");
        }
        finally
        {
        }

    }



}