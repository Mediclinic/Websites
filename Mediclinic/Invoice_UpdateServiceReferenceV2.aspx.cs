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

public partial class Invoice_UpdateServiceReferenceV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);

            if (!IsPostBack)
                FillEditViewForm();

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


    #region GetFormID()

    private bool IsValidFormID()
    {
        string inv_line = Request.QueryString["inv_line"];
        return inv_line != null && Regex.IsMatch(inv_line, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new Exception("Invalid url inv_line");

        string inv_line = Request.QueryString["inv_line"];
        return Convert.ToInt32(inv_line);
    }

    #endregion


    private void FillEditViewForm()
    {
        InvoiceLine invLine = InvoiceLineDB.GetByID(GetFormID());
        if (invLine == null)
        {
            HideTableAndSetErrorMessage("Invalid InvoiceLine ID");
            return;
        }


        txtFlashingText.Text = invLine.ServiceReference;


        btnSubmit.Text = "Update";
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // close this window
        Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (!IsValidFormID())
        {
            HideTableAndSetErrorMessage();
            return;
        }

        InvoiceLine invLine = InvoiceLineDB.GetByID(GetFormID());
        if (invLine == null)
        {
            HideTableAndSetErrorMessage("Invalid InvoiceLine ID");
            return;
        }

        InvoiceLineDB.UpdateServiceReference(invLine.InvoiceLineID, txtFlashingText.Text);

        //close this window
        Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
    }


    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
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