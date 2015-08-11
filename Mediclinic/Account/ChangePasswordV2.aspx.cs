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

public partial class ChangePasswordV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Utilities.SetNoCache(Response);
            CurrentPassword.Focus();
        }
        
    }


    protected void ChangePasswordButton_Click(object sender, EventArgs e)
    {
        ChangePassword(CurrentPassword.Text, NewPassword.Text);
    }

    protected void ChangePassword(string oldPwd, string newPwd)
    {

        if (!UserView.GetInstance().IsPatient)
        {
            Staff staff = StaffDB.GetByID(Convert.ToInt32(Session["StaffID"]));
            bool validUser = (staff != null);
            if (!validUser)
            {
                this.FailureText.Text = "Invalid staff member. Plase contact the system administrator." + "<br />";
                ResetFields();
                return;
            }

            if (staff.Pwd != oldPwd)
            {
                this.FailureText.Text = "Old password is not correct.";
                ResetFields();
                return;
            }

            if (newPwd.Length < 6)
            {
                this.FailureText.Text = "New passwords must be at least 6 characters.";
                ResetFields();
                return;
            }

            StaffDB.UpdatePwd(staff.StaffID, newPwd);
            CurrentPassword.Attributes.Add("value", "");
            NewPassword.Attributes.Add("value", "");
            ConfirmNewPassword.Attributes.Add("value", "");

            this.FailureText.Text = "Password successfully changed!";

            Response.Redirect("~/Account/ChangePasswordSuccessV2.aspx");
        }
        else
        {
            Patient patient = PatientDB.GetByID(Convert.ToInt32(Session["PatientID"]));
            bool validUser = (patient != null);
            if (!validUser)
            {
                this.FailureText.Text = "Invalid patient. Plase contact the system administrator." + "<br />";
                ResetFields();
                return;
            }

            if (patient.Pwd != oldPwd)
            {
                this.FailureText.Text = "Old password is not correct.";
                ResetFields();
                return;
            }

            if (newPwd.Length < 6)
            {
                this.FailureText.Text = "New passwords must be at least 6 characters.";
                ResetFields();
                return;
            }

            PatientDB.UpdatePwd(patient.PatientID, newPwd);
            CurrentPassword.Attributes.Add("value", "");
            NewPassword.Attributes.Add("value", "");
            ConfirmNewPassword.Attributes.Add("value", "");

            this.FailureText.Text = "Password successfully changed!";

            Response.Redirect("~/BookingsV2.aspx?orgs=" + Session["OrgID"] + "&ndays=3");
        }
    }

    protected void ResetFields()
    {
        CurrentPassword.Attributes.Add("value", CurrentPassword.Text);
        NewPassword.Attributes.Add("value", NewPassword.Text);
        ConfirmNewPassword.Attributes.Add("value", ConfirmNewPassword.Text);
    }

}