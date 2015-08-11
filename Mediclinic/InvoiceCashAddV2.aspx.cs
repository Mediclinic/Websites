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

public partial class InvoiceCashAddV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();


            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, false);
                Session.Remove("data_selected");
                Session.Remove("sortExpression_Selected");
                Session.Remove("sortExpression_Offering");

                SetUrlFieldsAndHeading();

                invoiceItemsControl.InvoiceType = Booking.InvoiceType.None;
                invoiceItemsControl.SubmitButtonText = "Create Invoice";
                invoiceItemsControl.FillOfferingGrid();
                invoiceItemsControl.FillSelectedGrid();
                invoiceItemsControl.LabelSetPrivateInvoiceVisible = false;
                invoiceItemsControl.ButtonCancelVisible = false;
            }
            else
            {
                invoiceItemsControl.InvoiceType = Booking.InvoiceType.None;
                invoiceItemsControl.FillOfferingGrid();
                invoiceItemsControl.FillSelectedGrid();
                ResetPatientName();
                ResetOrgName();
            }

            invoiceItemsControl.UserControlSubmitClicked += new EventHandler(InvoiceItemsControl_SubmitButtonClicked);

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

    #region SetUrlFieldsAndHeading

    protected void SetUrlFieldsAndHeading()
    {
        lblHeading.Text = "Create Cash Invoice";

        try
        {
            string patient_id = Request.QueryString["patient"];
            if (patient_id != null && patient_id != "-1")
            {
                if (!Regex.IsMatch(patient_id, @"^\d+$"))
                    throw new CustomMessageException();

                Patient patient = PatientDB.GetByID(Convert.ToInt32(patient_id));
                if (patient == null)
                    throw new CustomMessageException();

                txtUpdatePatientID.Text = patient.PatientID.ToString();
                txtUpdatePatientName.Text = patient.Person.FullnameWithoutMiddlename;
            }

            string org_id = Request.QueryString["org"];
            if (org_id != null && org_id != "-1")
            {
                if (!Regex.IsMatch(org_id, @"^\d+$"))
                    throw new CustomMessageException();

                Organisation org = OrganisationDB.GetByID(Convert.ToInt32(org_id));
                if (org == null)
                    throw new CustomMessageException();

                txtUpdateOrganisationID.Text = org.OrganisationID.ToString();
                txtUpdateOrganisationName.Text = org.Name;
            }


            if (txtUpdatePatientID.Text.Length > 0 && txtUpdateOrganisationID.Text.Length > 0)
            {
                lblHeading.Text = "Create Cash Invoice : " + txtUpdatePatientName.Text + " at " + txtUpdateOrganisationName.Text;
                patient_row.Attributes["class"] = "hiddencol";
                org_row.Attributes["class"] = "hiddencol";
            }
            if (txtUpdatePatientID.Text.Length > 0)
            {
                lblHeading.Text = "Create Cash Invoice : " + txtUpdatePatientName.Text;
                patient_row.Attributes["class"] = "hiddencol";
            }
            if (txtUpdateOrganisationID.Text.Length > 0)
            {
                lblHeading.Text = "Create Cash Invoice : " + txtUpdateOrganisationName.Text;
                org_row.Attributes["class"] = "hiddencol";
            }

        }
        catch (CustomMessageException)
        {
            SetErrorMessage();
        }
    }

    #endregion

    #region InvoiceItemsControl_SubmitButtonClicked

    private void InvoiceItemsControl_SubmitButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (txtUpdateOrganisationID.Text.Length == 0)
                throw new CustomMessageException("Please select an organisation.");

            Organisation org = OrganisationDB.GetByID(Convert.ToInt32(txtUpdateOrganisationID.Text));
            if (org == null)
                throw new Exception("Unknown organisation selected. Pelase contact your system administrator.");


            // keep id's to delete if exception and need to roll back
            int invID  = -2;
            ArrayList invLineIDs = new ArrayList();
            ArrayList offeringOrderIDs = new ArrayList();

            // used to check update stock and check warning level emails sent
            ArrayList invoiceLines = new ArrayList();


            try
            {

                int patientID = txtUpdatePatientID.Text.Length == 0 ? -1 : Convert.ToInt32(txtUpdatePatientID.Text);

                DataTable dt_selected_list = this.invoiceItemsControl.GetSelectedList();

                decimal total = 0;
                decimal gst = 0;
                for (int i = 0; i < dt_selected_list.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]);
                    gst   += Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]);
                }

                invID = InvoiceDB.Insert(108, -1, 0, patientID, org.OrganisationID, "", "", Convert.ToInt32(Session["StaffID"]), Convert.ToInt32(Session["SiteID"]), total + gst, gst, false, false, false, DateTime.MinValue);
                for (int i = 0; i < dt_selected_list.Rows.Count; i++)
                {

                    int offeringOrderID = -1;
                    if (Convert.ToBoolean(dt_selected_list.Rows[i]["on_order"]))
                    {
                        OfferingOrderDB.Insert(
                            Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]),
                            org.OrganisationID,
                            Convert.ToInt32(Session["StaffID"]),
                            patientID,
                            Convert.ToInt32(dt_selected_list.Rows[i]["quantity"]),
                            DateTime.Today,
                            DateTime.MinValue,
                            DateTime.MinValue,
                            string.Empty
                            );
                        offeringOrderIDs.Add(offeringOrderID);
                    }


                    int invoiceLineID = InvoiceLineDB.Insert(invID, -1, Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]), Convert.ToInt32(dt_selected_list.Rows[i]["quantity"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]) + Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]), "", "", offeringOrderID);
                    invLineIDs.Add(invoiceLineID);
                    invoiceLines.Add(new InvoiceLine(invoiceLineID, invID, -1, Convert.ToInt32(dt_selected_list.Rows[i]["offering_id"]), Convert.ToInt32(dt_selected_list.Rows[i]["quantity"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_price"]) + Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]), Convert.ToDecimal(dt_selected_list.Rows[i]["total_pt_gst"]), "", "", offeringOrderID));
                }

                Session.Remove("data_selected");


                // successfully completed, so update and check warning level for stocks
                foreach (InvoiceLine invoiceLine in invoiceLines)
                    if (invoiceLine.OfferingOrder == null) // stkip counting down if item is on order
                        StockDB.UpdateAndCheckWarning(org.OrganisationID, invoiceLine.Offering.OfferingID, (int)invoiceLine.Quantity);

            }
            catch (Exception ex)
            {
                // roll back...
                foreach (int invLineID in invLineIDs)
                    InvoiceLineDB.Delete(invLineID);
                foreach (int offeringOrderID in offeringOrderIDs)
                    OfferingOrderDB.Delete(offeringOrderID);
                InvoiceDB.Delete(invID);

                throw;
            }

            Response.Redirect("~/Invoice_ViewV2.aspx?invoice_id=" + invID + "&is_popup=0");
            //Response.Redirect("~/InvoiceListV2.aspx?start_date=" + DateTime.Today.ToString("yyyy_MM_dd") + "&end_date=" + DateTime.Today.AddDays(1).ToString("yyyy_MM_dd") + "&inc_medicare=0&inc_dva=0&inc_private=1");
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
        catch (Exception ex)
        {
            SetErrorMessage("", ex.ToString());
            return;
        }

    }

    #endregion

    #region ResetPatientName, ResetOrgName, btnUpdatePatient_Click, btnUpdateOrganisation_Click

    protected void ResetPatientName()
    {
        if (txtUpdatePatientID.Text.Length == 0)
            txtUpdatePatientName.Text = "";
        else
        {
            Patient patient = PatientDB.GetByID(Convert.ToInt32(txtUpdatePatientID.Text));
            txtUpdatePatientName.Text = patient.Person.FullnameWithoutMiddlename;
        }
    }

    protected void ResetOrgName()
    {
        if (txtUpdateOrganisationID.Text.Length == 0)
            txtUpdateOrganisationName.Text = "";
        else
        {
            Organisation org = OrganisationDB.GetByID(Convert.ToInt32(txtUpdateOrganisationID.Text));
            txtUpdateOrganisationName.Text = org.Name;
        }
    }

    protected void btnUpdatePatient_Click(object sender, EventArgs e)
    {
        if (txtUpdatePatientID.Text.Length > 0)
            this.lblPatientName.Text = txtUpdatePatientID.Text;
        else
            this.lblPatientName.Text = "--";
    }

    protected void btnUpdateOrganisation_Click(object sender, EventArgs e)
    {
        if (txtUpdateOrganisationID.Text.Length > 0)
            this.lblOrganisationName.Text = txtUpdateOrganisationID.Text;
        else
            this.lblOrganisationName.Text = "--";
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