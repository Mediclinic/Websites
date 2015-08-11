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
using System.Xml;
using System.Text;
using System.IO;

public partial class TyroHealthPointReconciliationV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = string.Empty;
            lblXMLOutput.Text    = string.Empty;
            divTestButtons.Visible = Request["debug"] != null && Request["debug"] == "1";

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
            resultXML += "<tr><td>Result </td><td style=\"width:10px;\"></td><td>" + hiddenResultXML.Value + "</td></tr>";
            if (hiddenErrorCodeXML.Value.Length > 0)
                resultXML += "<tr><td>Error Code </td><td></td><td>" + hiddenErrorCodeXML.Value + "</td></tr>";
            if (hiddenErrorDescrXML.Value.Length > 0)
                resultXML += "<tr><td>Error Descr </td><td></td><td>" + hiddenErrorDescrXML.Value + "</td></tr>";
            resultXML += "</table>";

            resultXML += "Raw Data: <a href=\"javascript:void(0)\" onclick=\"document.getElementById('xml_data').style.display='';\">Show</a> <a href=\"javascript:void(0)\" onclick=\"document.getElementById('xml_data').style.display='none';\">Hide</a>";
            resultXML += "<div id=\"xml_data\" style=\"display:none;\">";
            resultXML += "<table><tr><td><pre style=\"white-space:nowrap;text-align:left;\">" + hiddenDataXML.Value.Replace("<", "&lt;").Replace(">", "&gt;").Replace(Environment.NewLine, "<br />") + "</pre></td></tr></table>";
            resultXML += "</div>";
            lblXMLOutput.Text = Request["debug"] != null && Request["debug"] == "1" ? resultXML : string.Empty;


            string errorMsg = string.Empty;
            if (hiddenResultXML.Value != "success")
                errorMsg += (errorMsg.Length > 0 ? "<br />" : "") +  "Result: " + hiddenResultXML.Value;
            if (hiddenErrorCodeXML.Value != string.Empty)
                errorMsg += (errorMsg.Length > 0 ? "<br />" : "") + "Error: " + hiddenErrorCodeXML.Value;
            if (hiddenErrorDescrXML.Value != string.Empty)
                errorMsg += (errorMsg.Length > 0 ? "<br />" : "") + "Error: " + hiddenErrorDescrXML.Value;

            if (errorMsg.Length > 0)
                throw new CustomMessageException(errorMsg);

            DateTime dateTime;
            if (!DateTime.TryParseExact(txtSearchDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                throw new CustomMessageException("Exception: DateTime is not in correct format: " + txtSearchDate.Text);


            this.Reconcile(hiddenDataXML.Value);


            /*
            TyroPaymentPendingDB.Reconcile(
                Session["DB"].ToString(),
                dateTime,
                hiddenDataXML.Value);
            */

        }
        catch (Exception ex)
        {
            lblErrorMessage.Text += (lblErrorMessage.Text.Length == 0 ? "" : "<br /><br />") + (ex is CustomMessageException ? ex.Message : ex.ToString());

            //Email myself

        }
    }

    protected void Reconcile(string xml)
    {
        StringBuilder debutOutput = new StringBuilder();

        bool claimsExist = false;
        bool unavailable = false;
        using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
        {
            claimsExist = reader.ReadToFollowing("claim");
        }       
        using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
        {
            unavailable = reader.ReadToFollowing("unavailable");
        }

        //debutOutput.AppendLine("claimsExist: " + claimsExist + "<br />");
        //debutOutput.AppendLine("unavailable: " + unavailable + "<br />");
        //debutOutput.AppendLine("<br />");
        

        using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
        {
            reader.ReadToFollowing("claims-reconciliation-response");
            reader.ReadToFollowing("claims-reconciliation");

            ArrayList allFieldsList = new ArrayList();  // ordered list of all keys through all hashtables
            Hashtable allFieldsHash = new Hashtable();  // hashtable of all keys through all hashtables
            Hashtable hash = ReadSubtree(reader.ReadSubtree(), ref allFieldsList, ref allFieldsHash);

            string terminalID = hash["healthpoint-terminal-id"].ToString();
            string claimDateStr = hash["claim-date"].ToString();
            DateTime? claimDate = GetDateFromString(claimDateStr, "yyyyMMdd");
            if (claimDate != null)
                claimDateStr = claimDate.Value.ToString("d MMM, yyyy");

            debutOutput.AppendLine("<br/>");
            debutOutput.AppendLine("Terminal ID: <b>" + terminalID + "</b></br />" + Environment.NewLine);
            debutOutput.AppendLine("Claim Date: <b>" + claimDateStr + "</b></br />" + Environment.NewLine);



            if (unavailable)
            {
                debutOutput.AppendLine("<b>Report For This Date Does Not Exist.</b>");
            }
            else if (!claimsExist)
            {
                debutOutput.AppendLine("<b>No Claims On This Date.</b>");
            }
            else if (claimsExist)
            {
                ArrayList list = new ArrayList();  // list of hashtables of claims
                allFieldsList  = new ArrayList();  // ordered list of all keys through all hashtables
                allFieldsHash  = new Hashtable();  // hashtable of all keys through all hashtables

                // put each claim into their own hashtable
                while (reader.ReadToFollowing("claim"))
                {
                    hash = ReadSubtree(reader.ReadSubtree(), ref allFieldsList, ref allFieldsHash);
                    list.Add(hash);
                }

                // put all hashtables of claims into a table to display
                string table = "<table border=1 class=\"table table-bordered table-white table-grid table-grid-top-bottum-padding-normal auto_width block_center\">" + Environment.NewLine;

                // add ordered number of payments showing 1,2,3,....
                table += "<tr><td></td>";
                for(int i=0; i<list.Count; i++)
                    table += "<td style=\"text-align:center;\">" + (i+1) + "</td>";
                table += "</tr>" + Environment.NewLine;

                // add invoice line
                string refTagKey = "ref-tag";
                table += "<tr><td></td>";
                foreach (Hashtable claimHash in list)
                {
                    string field = string.Empty;
                    if (claimHash[refTagKey] != null)
                    {
                        TyroHealthClaim tyroHealthClaim = TyroHealthClaimDB.GetByRefTag(claimHash[refTagKey].ToString());
                        if (tyroHealthClaim != null)
                        {
                            string invLink = "Invoice_ViewV2.aspx?invoice_id=" + tyroHealthClaim.InvoiceID;
                            string invHref = "<a href=\"" + invLink + "\" onclick=\"open_new_tab('" + invLink + "')\" >Invoice</a>";
                            field += " " + invHref;
                        }
                    }
                    table += "<td style=\"text-align:center;\">" + field + "</td>";
                }
                table += "</tr>" + Environment.NewLine;

                // add all other information
                foreach (string key in allFieldsList)
                {
                    table += "<tr><td>" + key + "</td>";
                    foreach (Hashtable claimHash in list)
                    {
                        string field;
                        if (claimHash[key] == null)
                            field = string.Empty;
                        else if (!key.EndsWith("date-time"))
                            field = claimHash[key].ToString();
                        else
                        {
                            DateTime? date = GetDateFromString(claimHash[key].ToString(), "yyyyMMddHHmmss");
                            field = (date == null) ? field = claimHash[key].ToString() : date.Value.ToString("d MMM, yyyy hh:mm");
                        }

                        table += "<td>" + field + "</td>";
                    }
                    table += "</tr>" + Environment.NewLine;
                }
                table += "</table>" + Environment.NewLine;

                debutOutput.AppendLine(table);
            }
        }

        lblReadableOutput.Text = debutOutput.ToString(); ;

    }

    protected Hashtable ReadSubtree(XmlReader subTree, ref ArrayList allFieldsList, ref Hashtable allFieldsHash)
    {
        string name = null;
        string val = null;
        Hashtable hash = new Hashtable();
        while (subTree.Read())
        {
            switch (subTree.NodeType)
            {
                case XmlNodeType.Element:
                    name = subTree.Name;
                    break;
                case XmlNodeType.Text:
                    val = subTree.Value;
                    break;
                case XmlNodeType.EndElement:
                    if (name != null && val != null)
                    {
                        hash[name] = val;
                        if (allFieldsHash[name] == null)
                        {
                            allFieldsHash[name] = 1;
                            allFieldsList.Add(name);
                        }
                    }
                    name = null;
                    val = null;
                    break;
            }
        }

        return hash;
    }

    protected DateTime? GetDateFromString(string date, string format)
    {
        DateTime outDateTime;
        if (!DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
            return null;
        else
            return outDateTime;
    }

    #region btnTest_Click

    protected void btnTest_Click(object sender, CommandEventArgs e)
    {
        if (e.CommandArgument == "T1")
            Reconcile(T1);
        if (e.CommandArgument == "T2")
            Reconcile(T2);
        if (e.CommandArgument == "T3")
            Reconcile(T3);
    }

    protected string T1
    {
        get
        {
            return
      @"<claims-reconciliation-response>
          <claims-reconciliation>
            <healthpoint-terminal-id>TYR99906</healthpoint-terminal-id>
            <claim-date>20150506</claim-date>
          </claims-reconciliation>
          <claim>
            <terminal-date-time>20150506111513</terminal-date-time>
            <settlement-date-time>20150507140000</settlement-date-time>
            <response-date-time>20150506112519</response-date-time>
            <transaction-date-time>20150506111513</transaction-date-time>
            <provider-id>4237955J</provider-id>
            <private-health-fund-id>00</private-health-fund-id>
            <ref-tag>4893325</ref-tag>
            <status>CAN</status>
            <total-claim-amount>6800</total-claim-amount>
            <private-health-fund-total-benefit-amount>6800</private-health-fund-total-benefit-amount>
            <private-health-fund-response-code>00</private-health-fund-response-code>
            <text/>
          </claim>
          <claim>
            <terminal-date-time>20150506113643</terminal-date-time>
            <settlement-date-time>20150507140000</settlement-date-time>
            <response-date-time>20150506113643</response-date-time>
            <transaction-date-time>20150506113643</transaction-date-time>
            <provider-id>4237955J</provider-id>
            <private-health-fund-id/>
            <ref-tag>4893326</ref-tag>
            <status>AVOID</status>
            <total-claim-amount>6800</total-claim-amount>
            <private-health-fund-total-benefit-amount>0</private-health-fund-total-benefit-amount>
            <private-health-fund-response-code/>
            <text/>
          </claim>
          <claim>
            <terminal-date-time>20150506113815</terminal-date-time>
            <settlement-date-time>20150507140000</settlement-date-time>
            <response-date-time>20150506113815</response-date-time>
            <transaction-date-time>20150506113815</transaction-date-time>
            <provider-id>4237955J</provider-id>
            <private-health-fund-id/>
            <ref-tag>4893327</ref-tag>
            <status>AVOID</status>
            <total-claim-amount>6800</total-claim-amount>
            <private-health-fund-total-benefit-amount>0</private-health-fund-total-benefit-amount>
            <private-health-fund-response-code/>
            <text/>
          </claim>
          <claim>
            <terminal-date-time>20150506113949</terminal-date-time>
            <settlement-date-time>20150507140000</settlement-date-time>
            <response-date-time>20150506113949</response-date-time>
            <transaction-date-time>20150506113949</transaction-date-time>
            <provider-id>4237955J</provider-id>
            <private-health-fund-id/>
            <ref-tag>4893328</ref-tag>
            <status>AVOID</status>
            <total-claim-amount>6800</total-claim-amount>
            <private-health-fund-total-benefit-amount>0</private-health-fund-total-benefit-amount>
            <private-health-fund-response-code/>
            <text/>
          </claim>
          <claim>
            <terminal-date-time>20150506114343</terminal-date-time>
            <settlement-date-time>20150507140000</settlement-date-time>
            <response-date-time>20150506114343</response-date-time>
            <transaction-date-time>20150506114343</transaction-date-time>
            <provider-id>4237955J</provider-id>
            <private-health-fund-id/>
            <ref-tag>4893329</ref-tag>
            <status>AVOID</status>
            <total-claim-amount>6800</total-claim-amount>
            <private-health-fund-total-benefit-amount>0</private-health-fund-total-benefit-amount>
            <private-health-fund-response-code/>
            <text/>
          </claim>
          <claim>
            <terminal-date-time>20150506120902</terminal-date-time>
            <settlement-date-time>20150507140000</settlement-date-time>
            <response-date-time>20150506120958</response-date-time>
            <transaction-date-time>20150506120902</transaction-date-time>
            <provider-id>4237955J</provider-id>
            <private-health-fund-id>00</private-health-fund-id>
            <ref-tag>4893331</ref-tag>
            <status>CAN</status>
            <total-claim-amount>6800</total-claim-amount>
            <private-health-fund-total-benefit-amount>6800</private-health-fund-total-benefit-amount>
            <private-health-fund-response-code>00</private-health-fund-response-code>
            <text/>
          </claim>
          <check>
            <total-number-claims>0000006</total-number-claims>
            <sum-benefit-amount>00000013600</sum-benefit-amount>
            <sum-claim-amount>00000040800</sum-claim-amount>
          </check>
        </claims-reconciliation-response>";
        }
    }

    protected string T2
    {
        get
        {
            return
      @"<claims-reconciliation-response>
          <claims-reconciliation>
            <healthpoint-terminal-id>TYR99906</healthpoint-terminal-id>
            <claim-date>20150507</claim-date>
          </claims-reconciliation>
          <unavailable>No report created.</unavailable>
        </claims-reconciliation-response>";
        }
    }

    protected string T3
    {
        get
        {
            return
      @"<claims-reconciliation-response>
          <claims-reconciliation>
            <healthpoint-terminal-id>TYR99906</healthpoint-terminal-id>
            <claim-date>20150502</claim-date>
          </claims-reconciliation>
          <check>
            <total-number-claims>0</total-number-claims>
            <sum-benefit-amount>0</sum-benefit-amount>
            <sum-claim-amount>0</sum-claim-amount>
          </check>
        </claims-reconciliation-response>";
        }
    }

    #endregion

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