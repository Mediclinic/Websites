using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;

public partial class PatientReferrerControlV2 : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void UpdateReferrersList()
    {
        setReferrersList(null);
    }


    protected enum UrlParamType { Add, Edit, View, None };

    protected int PatientID
    {
        get { return patientReferrerPatientID.Value == "" ? -1 : Convert.ToInt32(patientReferrerPatientID.Value); }
        set { patientReferrerPatientID.Value = value.ToString(); }
    }
    protected UrlParamType FormType
    {
        get 
        {
            if (patientReferrerFormType.Value == UrlParamType.Add.ToString())
                return UrlParamType.Add;
            else if (patientReferrerFormType.Value == UrlParamType.Edit.ToString())
                return UrlParamType.Edit;
            else if (patientReferrerFormType.Value == UrlParamType.View.ToString())
                return UrlParamType.View;
            else if (patientReferrerFormType.Value == UrlParamType.None.ToString())
                return UrlParamType.None;
            else
                return UrlParamType.None;
        }
        set
        {
            patientReferrerFormType.Value = value.ToString();
        }
    }
    protected UrlParamType GetFormTypeFromString(string formType)
    {
        if (formType.ToLower() == "add")
            return UrlParamType.Add;
        else if (formType.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (formType.ToLower() == "view")
            return UrlParamType.View;
        else
            return UrlParamType.None;
    }
    public void SetInfo(int patientID, string formType)
    {
        this.PatientID = patientID;
        this.FormType = GetFormTypeFromString(formType);

        UpdateInfo();
    }
    public void ReSetInfo()
    {
        UpdateInfo();
    }




    protected void UpdateInfo()
    {
        HideAllRows();
        SetupGUI();

        if (FormType == UrlParamType.Edit || FormType == UrlParamType.View)
            InitForm(this.PatientID);
    }

    protected void HideAllRows()
    {
        displayHaveReferrerRow.Visible = false;
        editRow.Visible = false;
        displayNoReferrerRow.Visible = false;
        addRow.Visible = false;
        newReferrersLinkRow.Visible = false;


    }

    private void SetupGUI()
    {
        bool editable = true; // GetUrlParamType() == UrlParamType.Add || GetUrlParamType() == UrlParamType.Edit;
        Utilities.SetEditControlBackColour(ddlReferrer,    editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        Utilities.SetEditControlBackColour(ddlNewReferrer, editable, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
    }


    protected void InitForm() // assumes edit mode
    {
        HideAllRows();

        InitForm(this.PatientID);
    }
    protected void InitForm(int patientID)
    {
        HideAllRows();

        string url = "/PatientReferrerHistoryPopupV2.aspx?id=" + patientID;
        string text = "History";
        string onclick = @"onclick=""open_new_tab('" + url + @"');return false;""";
        lblPatientReferrerHistoryPopup.Text = "<a " + onclick + " href=\"\">" + text + "</a>";


        PatientReferrer[] patientReferrer = PatientReferrerDB.GetActiveEPCPatientReferrersOf(patientID);  // = PatientReferrerDB.GetEPCPatientReferrersOf(patient.PatientID);
        if (patientReferrer.Length > 0)
        {
            PatientReferrer currentPatRegReferrer = patientReferrer[patientReferrer.Length-1]; // get latest
            RegisterReferrer curRegReferrer        = currentPatRegReferrer.RegisterReferrer;

            displayHaveReferrerRow.Visible = true;

            // only allow removing a referrer if no EPC set [ie no active healthcard, or healthcard with neither date set]
            HealthCard hc = HealthCardDB.GetActiveByPatientID(patientID);
            bool allowDelete = hc == null || !hc.HasEPC();
            btnDelete.Visible = allowDelete;
            lblDeleteRegistrationReferrerBtnSeperator.Visible = allowDelete;

            //lblReferrer.Text = curRegReferrer.Referrer.Person.Surname + ", " + curRegReferrer.Referrer.Person.Firstname + " [" + curRegReferrer.Organisation.Name + "]" + " [" + currentPatRegReferrer.PatientReferrerDateAdded.ToString("dd-MM-yyyy") + "]";

            string phNumTxt = string.Empty;

            if (Utilities.GetAddressType().ToString() == "Contact")
            {
                Contact[] phNums = ContactDB.GetByEntityID(2, curRegReferrer.Organisation.EntityID);
                for (int i = 0; i < phNums.Length; i++)
                    phNumTxt += (i > 0 ? "<br />" : "") + Utilities.FormatPhoneNumber(phNums[i].AddrLine1) + " &nbsp;&nbsp; (" + phNums[i].ContactType.Descr + ")";
            }
            else if (Utilities.GetAddressType().ToString() == "ContactAus")
            {
                ContactAus[] phNums = ContactAusDB.GetByEntityID(2, curRegReferrer.Organisation.EntityID);
                for (int i = 0; i < phNums.Length; i++)
                    phNumTxt += (i > 0 ? "<br />" : "") + Utilities.FormatPhoneNumber(phNums[i].AddrLine1) + " &nbsp;&nbsp; (" + phNums[i].ContactType.Descr + ")";
            }
            else
                throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

            lblReferrer.Text = curRegReferrer.Referrer.Person.Surname + ", " + curRegReferrer.Referrer.Person.Firstname + (curRegReferrer.Organisation.Name.Length == 0 ? "" : " [" + curRegReferrer.Organisation.Name + "]") + "<br />" + Environment.NewLine +
                  "<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">" + Environment.NewLine +
                  "<tr><td>" + "Provider Nbr:" + "</td><td style=\"width:12px\"></td><td><font color=\"#A52A2A\">" + currentPatRegReferrer.RegisterReferrer.ProviderNumber + "</font></td></tr>" + Environment.NewLine +
                  "<tr><td>" + "Date Added:" + "</td><td style=\"width:12px\"></td><td><font color=\"#A52A2A\">" + currentPatRegReferrer.PatientReferrerDateAdded.ToString("dd-MM-yyyy") + "</font></td></tr>" + Environment.NewLine +
                  "</table>" + Environment.NewLine +
                  (phNumTxt.Length == 0 ? "" : phNumTxt + "<br />"); // put in referrers fax and phone numbers

            lblReferrerRegisterID.Text = curRegReferrer.RegisterReferrerID.ToString();
        }
        else
        {
            displayNoReferrerRow.Visible = true;
        }
    }

    protected void btnChangeToEditMode_Click(object sender, EventArgs e)
    {
        // dont allow editing

        displayHaveReferrerRow.Visible = false;
        editRow.Visible = true;
        newReferrersLinkRow.Visible = true;

        PatientReferrer[] patientReferrer = PatientReferrerDB.GetEPCPatientReferrersOf(this.PatientID);
        PatientReferrer currentPatRegReferrer = patientReferrer[patientReferrer.Length-1];

        setReferrersList(currentPatRegReferrer);
    }
    protected void setReferrersList(PatientReferrer currentPatRegReferrer = null)
    {
        if (currentPatRegReferrer == null)
        {
            PatientReferrer[] patientReferrer = PatientReferrerDB.GetEPCPatientReferrersOf(this.PatientID);
            if (patientReferrer.Length > 0)
                currentPatRegReferrer = patientReferrer[patientReferrer.Length - 1];
        }

        int nItems = ddlReferrer.Items.Count;
        for (int i = 0; i < nItems; i++)
            ddlReferrer.Items.RemoveAt(0);

        DataTable rr = RegisterReferrerDB.GetDataTable(0, -1, false, new int[] { 191 });
        if (currentPatRegReferrer != null)
        {
            // if refererrer set as inactive (ie user set as deleted, but is still in system)
            // then it will show the referrer as his referrer, but when hitting update, the list will not contain it, and throws an error
            // so if set as inactive (deleted), then add to the list
            bool isDeletedReferrer = rr.Select("register_referrer_id=" + currentPatRegReferrer.RegisterReferrer.RegisterReferrerID).Length == 0;
            if (isDeletedReferrer)
            {
                DataRow newRow = rr.NewRow();
                newRow["surname"]              = currentPatRegReferrer.RegisterReferrer.Referrer.Person.Surname;
                newRow["firstname"]            = currentPatRegReferrer.RegisterReferrer.Referrer.Person.Firstname;
                newRow["middlename"]           = currentPatRegReferrer.RegisterReferrer.Referrer.Person.Middlename;
                newRow["name"]                 = currentPatRegReferrer.RegisterReferrer.Organisation.Name;
                newRow["register_referrer_id"] = currentPatRegReferrer.RegisterReferrer.RegisterReferrerID;

                bool inserted = false;
                for (int i = rr.Rows.Count - 1; i >= 0; i--)
                {
                    if (currentPatRegReferrer.RegisterReferrer.Referrer.Person.Surname.CompareTo(rr.Rows[i]["surname"].ToString()) < 0)
                        continue;
                    if (currentPatRegReferrer.RegisterReferrer.Referrer.Person.Surname.CompareTo(rr.Rows[i]["surname"].ToString()) == 0)
                    {
                        if (currentPatRegReferrer.RegisterReferrer.Referrer.Person.Firstname.CompareTo(rr.Rows[i]["firstname"].ToString()) < 0)
                            continue;
                        if (currentPatRegReferrer.RegisterReferrer.Referrer.Person.Middlename.CompareTo(rr.Rows[i]["surname"].ToString()) == 0)
                        {
                            if (currentPatRegReferrer.RegisterReferrer.Referrer.Person.Firstname.CompareTo(rr.Rows[i]["middlename"].ToString()) < 0)
                                continue;
                        }
                    }

                    // now insert before this one

                    if (i == rr.Rows.Count)
                        rr.Rows.Add(newRow);
                    else
                        rr.Rows.InsertAt(newRow, i+1);

                    inserted = true;
                    break;
                }

                if (!inserted)
                {
                    if (rr.Rows.Count == 0)
                        rr.Rows.Add(newRow);
                    else
                        rr.Rows.InsertAt(newRow, 0);
                }
            }
        }
        foreach (DataRowView row in rr.DefaultView)
            ddlReferrer.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString() + " [" + row["name"].ToString() + "]", row["register_referrer_id"].ToString()));

        if (currentPatRegReferrer != null)
            ddlReferrer.SelectedValue = currentPatRegReferrer.RegisterReferrer.RegisterReferrerID.ToString();
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            PatientReferrerDB.UpdateSetEPCRefInactive(Convert.ToInt32(lblReferrerRegisterID.Text), this.PatientID); // update inactive means when they add, we have to reactivate rather than add new row
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        InitForm();
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        // dont allow updating

        /*
        try
        {
            PatientReferrer[] patientReferrer = PatientReferrerDB.GetEPCPatientReferrersOf(this.PatientID);
            PatientReferrer currentPatRegReferrer = patientReferrer[0];

            PatientReferrerDB.Update(currentPatRegReferrer.PatientReferrerID, this.PatientID, Convert.ToInt32(ddlReferrer.SelectedValue), currentPatRegReferrer.Organisation == null ? 0 : currentPatRegReferrer.Organisation.OrganisationID, false);
        }
        catch (UniqueConstraintException)
        {
            // happens when 2 forms allow adding, and they added one and now they already have active referrer
            // do nothing and let form re-update and show their active referrer
        }

        InitForm();
        */
    }
    protected void btnCancelEdit_Click(object sender, EventArgs e)
    {
        InitForm();
    }
    protected void btnChangeToAddMode_Click(object sender, EventArgs e)
    {
        displayNoReferrerRow.Visible = false;
        addRow.Visible = true;
        newReferrersLinkRow.Visible = true;


        int nItems = ddlNewReferrer.Items.Count;
        for(int i=0; i<nItems; i++)
            ddlNewReferrer.Items.RemoveAt(0);


        DataTable rr = RegisterReferrerDB.GetDataTable(0, -1, false, new int[] { 191 });
        foreach (DataRowView row in rr.DefaultView)
            ddlNewReferrer.Items.Add(new ListItem(row["surname"].ToString() + ", " + row["firstname"].ToString() + " [" + row["name"].ToString() + "]", row["register_referrer_id"].ToString()));
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        SetOrUpdateReferrer(Convert.ToInt32(ddlNewReferrer.SelectedValue));
    }
    protected void btnCancelAdd_Click(object sender, EventArgs e)
    {
        InitForm();
    }


    #region SetErrorMessage

    private void SetErrorMessage(string errMsg = "", string details = "")
    {
        displayHaveReferrerRow.Visible = false;
        editRow.Visible                = false;
        displayNoReferrerRow.Visible   = false;
        addRow.Visible                 = false;

        errorRow.Visible               = true;


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

    #endregion


    protected void btnRegisterReferrerUpdate_Click(object sender, EventArgs e)
    {
        int newRegisterReferrerID = Convert.ToInt32(txtUpdateRegisterReferrerID.Text);
        SetOrUpdateReferrer(newRegisterReferrerID);
    }

    protected void SetOrUpdateReferrer(int newRegisterReferrerID)
    {

        // if change this back to  " = PatientReferrerDB.GetEPCPatientReferrersOf(patient.PatientID); "  then make sure go through whole list 
        PatientReferrer[] patientReferrer = PatientReferrerDB.GetActiveEPCPatientReferrersOf(this.PatientID);
        if (patientReferrer.Length > 0)
        {
            PatientReferrer  currentPatRegReferrer = patientReferrer[patientReferrer.Length-1];
            RegisterReferrer curRegReferrer        = currentPatRegReferrer.RegisterReferrer;

            if (curRegReferrer.RegisterReferrerID == newRegisterReferrerID)
                return;
            else
                PatientReferrerDB.UpdateSetInactive(currentPatRegReferrer.PatientReferrerID);
        }

        PatientReferrerDB.Insert(this.PatientID, newRegisterReferrerID, 0, false);
        InitForm();
    }

}