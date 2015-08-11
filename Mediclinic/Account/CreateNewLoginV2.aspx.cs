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

public partial class CreateNewLoginV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Utilities.SetNoCache(Response);
            main_content.Style["background"] = (Session["SystemVariables"] == null) ? "url(../imagesV2/login_bg.png) center top no-repeat #EDEDED" : "url(../imagesV2/" + ((SystemVariables)Session["SystemVariables"])["MainLogoBackground"].Value + ") center top no-repeat #EDEDED";
        }

        Page.Form.DefaultFocus = Email.ClientID;
    }


    protected void CreateLoginButton_Click(object sender, EventArgs e)
    {
        CreateLogin(Email.Text.Trim());
    }

    protected void CreateLogin(string email)
    {
        email = email.Replace("'","''");

        //string curDbName = Session["DB"].ToString();

        try
        {

            List<Tuple<string, Patient, bool>> list = new List<Tuple<string, Patient, bool>>();


            System.Data.DataTable tbl = DBBase.ExecuteQuery("EXEC sp_databases;", "master").Tables[0];
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                string databaseName = tbl.Rows[i][0].ToString();

                if (!Regex.IsMatch(databaseName, @"Mediclinic_\d{4}"))
                    continue;
                //if (databaseName == "Mediclinic_0001")
                //    continue;

                System.Text.StringBuilder output = new System.Text.StringBuilder();

                Session["DB"] = databaseName;
                Session["SystemVariables"] = SystemVariableDB.GetAll();


                bool allowPatientLogins            = ((SystemVariables)Session["SystemVariables"])["AllowPatientLogins"].Value            == "1";
                bool allowPatientsToCreateOwnLogin = ((SystemVariables)Session["SystemVariables"])["AllowPatientsToCreateOwnLogin"].Value == "1";

                if (!allowPatientLogins || !allowPatientsToCreateOwnLogin)
                    continue;


                int[] entityIDs;
                if (Utilities.GetAddressType().ToString() == "Contact")
                    entityIDs = ContactDB.GetByAddrLine1(null, email, 27).Select(r => r.EntityID).ToArray();
                else if (Utilities.GetAddressType().ToString() == "ContactAus")
                    entityIDs = ContactAusDB.GetByAddrLine1(null, email, 27).Select(r => r.EntityID).ToArray();
                else
                    throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());


                foreach (int entityID in entityIDs)
                {
                    Patient patient = PatientDB.GetByEntityID(entityID);
                    if (patient == null || patient.IsDeceased || patient.IsDeleted)
                        continue;

                    bool hasLoginDetails = patient.Login.Length > 0;
                    if (!hasLoginDetails)
                    {
                        string login = Regex.Replace(patient.Person.Firstname, @"[^A-Za-z]+", "").ToLower() + Regex.Replace(patient.Person.Surname, @"[^A-Za-z]+", "").ToLower();
                        string loginTry = login;

                        Random rnd = new Random();
                        int    nbr = rnd.Next(11, 999);

                        do
                        {
                            bool loginUsed = (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]) && UserDatabaseMapperDB.UsernameExists(loginTry)) ||
                                             (PatientDB.LoginExists(loginTry));

                            if (!loginUsed)
                            {
                                patient.Login = loginTry;
                                patient.Pwd   = loginTry == login ? login + nbr : loginTry;

                                PatientDB.UpdateLoginPwd(patient.PatientID, patient.Login, patient.Pwd);
                                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["UseConfigDB"]))
                                    UserDatabaseMapperDB.Insert(loginTry, Session["DB"].ToString());

                                break;
                            }

                            nbr++;
                            loginTry = login + nbr;

                        } while (true);

                    }
                    
                    SendPasswordRetrievalEmail(patient.Login, patient.Pwd, email);
                    list.Add(new Tuple<string, Patient, bool>(databaseName, patient, hasLoginDetails));
                }

                Session.Remove("DB");
                Session.Remove("SystemVariables");
            }


            System.Text.StringBuilder finalOutput = new System.Text.StringBuilder();
            foreach (Tuple<string, Patient, bool> item in list)
                finalOutput.Append("<tr><td>" + item.Item1 + "</td><td>" + item.Item2.Person.FullnameWithoutMiddlename + "</td><td>" + item.Item3 + "</td><td>" + item.Item2.Login + " | " + item.Item2.Pwd + "</td></tr>");


            //FailureText.Text = "Count: " + list.Count + "<br /><table border=\"1\" class=\"block_center padded-table-2px\">" + finalOutput.ToString() + "</table>";


            if (list.Count == 0)
                throw new CustomMessageException("No patients found with this email");

            this.FailureText.Text = "An email has been sent with new login details";

        }
        catch (CustomMessageException cmEx)
        {
            this.FailureText.Text = cmEx.Message;
        }
        finally
        {
            //Session["DB"] = curDbName;
            //Session["SystemVariables"] = SystemVariableDB.GetAll();
            Session.Remove("DB");
            Session.Remove("SystemVariables");
        }

    }

    protected void SendPasswordRetrievalEmail(string login, string pwd, string email)
    {
        string body = @"Below is your login/password details as requested from the Create Web Login webpage.<br>
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
            "Password Generation Request",
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