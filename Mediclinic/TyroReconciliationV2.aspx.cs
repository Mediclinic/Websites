using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;

public partial class TyroReconciliationV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            lblErrorMessage.Text = string.Empty;
            lblTextOutput.Text   = string.Empty;
            lblXMLOutput.Text    = string.Empty;

            if (!IsPostBack)
            {
                txtSearchDate_Picker.OnClientClick = "displayDatePicker('txtSearchDate', this, 'dmy', '-'); return false;";
                txtSearchDate.Text = DateTime.Today.ToString("dd-MM-yyyy");
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


    protected void btnProcessResults_Click(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = string.Empty;

            txtSearchDate.Text = hiddenDate.Value;

            string resultXML = "<table style=\"text-align:left;white-space:pre;vertical-align:top;\">";
            resultXML += "<tr><td>Result  </td><td style=\"width:10px;\"></td><td>" + hiddenResultXML.Value + "</td><td style=\"width:55px;\"></td><td>Error   </td><td></td><td>" + hiddenErrorXML.Value + "</td></tr>";
            resultXML += "<tr><td>Type    </td><td></td><td>" + hiddenTypeXML.Value + "</td><td></td><td>Format  </td><td></td><td>" + hiddenFormatXML.Value + "</td></tr>";
            resultXML += "</table>";
            if (hiddenDataXML.Value.Length > 0)
            {
                resultXML += "Raw Data: <a href=\"javascript:void(0)\" onclick=\"document.getElementById('xml_data').style.display='';\">Show</a> <a href=\"javascript:void(0)\" onclick=\"document.getElementById('xml_data').style.display='none';\">Hide</a>";
                resultXML += "<div id=\"xml_data\" style=\"display:none;\">";
                resultXML += "<table><tr><td><pre style=\"white-space:nowrap;text-align:left;\">" + hiddenDataXML.Value.Replace("<", "&lt;").Replace(">", "&gt;").Replace(Environment.NewLine, "<br />") + "</pre></td></tr></table>";
                resultXML += "</div>";
            }

            string resultText = "<table style=\"text-align:left;white-space:pre;\">";
            resultText += "<tr><td>Result  </td><td style=\"width:10px;\"></td><td>" + hiddenResultText.Value + "</td><td style=\"width:55px;\"></td><td>Error   </td><td></td><td>" + hiddenErrorText.Value + "</td></tr>";
            resultText += "<tr><td>Type    </td><td></td><td>" + hiddenTypeText.Value + "</td><td></td><td>Format  </td><td></td><td>" + hiddenFormatText.Value + "</td></tr>";
            resultText += "</table>";
            if (hiddenDataText.Value.Length > 0)
                resultText += "<table><tr><td style=\"white-space:nowrap;text-align:left;\"><pre>" + hiddenDataText.Value + "</pre></td></tr></table>";


            lblXMLOutput.Text = Request["debug"] != null && Request["debug"] == "1" ? resultXML : string.Empty;
            lblTextOutput.Text = resultText;


            string errorMsg = string.Empty;
            if (hiddenResultXML.Value != "success")
                errorMsg += (errorMsg.Length > 0 ? "<br />" : "") + "Result: " + hiddenResultXML.Value;
            if (hiddenErrorXML.Value != string.Empty && hiddenErrorXML.Value != "undefined")
                errorMsg += (errorMsg.Length > 0 ? "<br />" : "") + "Error: " + hiddenErrorXML.Value;

            if (errorMsg.Length > 0)
                throw new CustomMessageException(errorMsg);

            DateTime dateTime;
            if (!DateTime.TryParseExact(txtSearchDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                throw new CustomMessageException("DateTime is not in correct format: " + txtSearchDate.Text);

            TyroPaymentPendingDB.Reconcile(
                Session["DB"].ToString(),
                dateTime,
                hiddenDataXML.Value);
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text += (lblErrorMessage.Text.Length == 0 ? "" : "<br /><br />") + (ex is CustomMessageException ? ex.Message : ex.ToString());

            //Email myself

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