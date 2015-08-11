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

public partial class LostPasswordV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Utilities.SetNoCache(Response);
            main_content.Style["background"] = (Session["SystemVariables"] == null) ? "url(../imagesV2/login_bg.png) center top no-repeat #EDEDED" : "url(../imagesV2/" + ((SystemVariables)Session["SystemVariables"])["MainLogoBackground"].Value + ") center top no-repeat #EDEDED";
        }

        Page.Form.DefaultFocus = Username.ClientID;
    }


    protected void RetrieveButton_Click(object sender, EventArgs e)
    {
        Retrieve(Username.Text.Trim());
    }

    protected void Retrieve(string username)
    {
        try
        {

            Session.Remove("DB");
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseConfigDB"]))
            {
                Session["DB"] = System.Configuration.ConfigurationManager.AppSettings["Database"];
            }
            else // Get DB from Mediclinic_Main
            {
                UserDatabaseMapper user = UserDatabaseMapperDB.GetByLogin(username);
                if (user == null)
                {
                    this.FailureText.Text = "Login Failed.";
                    return;
                }

                Session["DB"] = user.DBName;
            }

            Session["SystemVariables"] = SystemVariableDB.GetAll();


            if (username.Length > 0)
            {
                Staff   staff   = StaffDB.GetByLogin(username);
                Patient patient = PatientDB.GetByLogin(username);

                if (staff != null && !staff.IsFired)
                {
                    string[] emails = ContactDB.GetEmailsByEntityID(staff.Person.EntityID);

                    if (emails.Length == 0)
                        throw new CustomMessageException("No email is set for user: " + username);

                    SendPasswordRetrievalEmail(staff.Login, staff.Pwd, string.Join(",", emails));

                    this.FailureText.Text = "An email has been sent with login details for '" + username + "' to the email address(es) associated with that user";
                }
                else if (patient != null && !patient.IsDeleted)
                {
                    string[] emails = ContactDB.GetEmailsByEntityID(patient.Person.EntityID);

                    if (emails.Length == 0)
                        throw new CustomMessageException("No email is set for user: " + username);

                    SendPasswordRetrievalEmail(patient.Login, patient.Pwd, string.Join(",", emails));
                    
                    this.FailureText.Text = "An email has been sent with login details for '" + username + "' to the email address(es) associated with that user";
                }
                else
                    throw new CustomMessageException("Username does not exist");
            }
            else
            {
                throw new CustomMessageException("Please enter a username");
            }

        }
        catch (CustomMessageException cmEx)
        {
            this.FailureText.Text = cmEx.Message;
        }
        finally
        {
            Session.Remove("DB");
            Session.Remove("SystemVariables");
        }
    }

    protected void SendPasswordRetrievalEmail(string login, string pwd, string email)
    {
        string body = @"Below is your login/password details as requested from the Lost Password page.<br>
<br>
<table>
  <tr>
    <td>Username:</td>
    <td><b>" + login + @"</b></td>
  </tr>
  <tr>
    <td>Password:</td>
    <td><b>" + pwd + @"</b></td>
  </tr>
</table>

<br>
<br>    Regards,
<br>    Mediclinic";


        Emailer.SimpleEmail(email,
            "Password Retrieval Request",
            body,
            true,
            null,
            null);
    }

    protected void lnkLogin_Click(object sender, EventArgs e)
    {
        // un-set auth cookie that was set to allow this page to not be directed to always goes to config file field 'loginUrl'
        System.Web.Security.FormsAuthentication.SetAuthCookie("--", true);

        Response.Redirect("~/Account/LoginV2.aspx", false);
    }

}