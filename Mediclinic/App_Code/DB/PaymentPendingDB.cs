using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;

public class PaymentPendingDB
{

    public static void Delete(string DB, int payment_pending_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM PaymentPending WHERE payment_pending_id = " + payment_pending_id.ToString(), DB);
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static int Insert(string DB, int invoice_id, decimal payment_amount, string customer_name)
    {
        customer_name = customer_name.Replace("'", "''");
        string sql = "INSERT INTO PaymentPending (invoice_id,payment_amount,customer_name,date_added,out_date_processed,out_payment_result,out_payment_result_code,out_payment_result_text,out_bank_receipt_id,out_paytecht_payment_id) VALUES (" + "" + invoice_id + "," + "" + payment_amount + "," + "'" + customer_name + "'," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "NULL," + "''," + "''," + "''," + "''," + "''" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql, DB));
    }

    public static void Update(string DB, DateTime out_date_processed, int payment_pending_id, string out_payment_result, string out_payment_result_code, string out_payment_result_text, string out_bank_receipt_id, string out_paytecht_payment_id)
    {
        out_payment_result      = out_payment_result.Replace("'", "''");
        out_payment_result_code = out_payment_result_code.Replace("'", "''");
        out_payment_result_text = out_payment_result_text.Replace("'", "''");
        out_bank_receipt_id     = out_bank_receipt_id.Replace("'", "''");
        out_paytecht_payment_id = out_paytecht_payment_id.Replace("'", "''");
        string sql = "UPDATE PaymentPending SET out_date_processed = '" + out_date_processed.ToString("yyyy-MM-dd HH:mm:ss") + "',out_payment_result = '" + out_payment_result + "',out_payment_result_code = '" + out_payment_result_code + "',out_payment_result_text = '" + out_payment_result_text + "',out_bank_receipt_id = '" + out_bank_receipt_id + "',out_paytecht_payment_id = '" + out_paytecht_payment_id + "' WHERE payment_pending_id = " + payment_pending_id.ToString();
        DBBase.ExecuteNonResult(sql, DB);
    }


    public static string UpdateAllPaymentsPending(string DB, DateTime from, DateTime to, int staffID, bool incOutput = false)
    {
        bool isStakeholder = HttpContext.Current.Session != null && HttpContext.Current.Session["IsStakeholder"] != null && Convert.ToBoolean(HttpContext.Current.Session["IsStakeholder"]);

        NonPCIServiceClient client = new NonPCIServiceClient();

        px.ezidebit.com.au.EziResponseOfArrayOfPaymentTHgMB7oL result = client.GetPayments(
            ((SystemVariables)HttpContext.Current.Session["SystemVariables"])["EziDebit_DigitalKey"].Value,
            "ALL",
            "ALL",
            "ALL",
            "",
            from.ToString("yyyy-MM-dd"),
            to.ToString("yyyy-MM-dd"),
            "PAYMENT",
            "",
            ""
            );


        string output = string.Empty;

        output += "Error: " + result.Error + "<br /><br />";

        if (result.Data != null)
        {

            // some erroneous payment references have gotten in and then there is an erorr converting it to an int to sort it.
            bool containsOnlyInts = true;
            string allPaymentRefs = string.Empty;
            foreach (px.ezidebit.com.au.Payment payment in result.Data)
            {
                if (!Regex.IsMatch(payment.PaymentReference, @"^\d+$"))
                {
                    allPaymentRefs += "<tr><td><font color=\"red\">" + payment.PaymentReference + "</font></td><td style=\"min-width:10px;\"></td><td>$" + payment.ScheduledAmount + "</td><td style=\"min-width:10px;\"></td><td>" + (payment.SettlementDate == null ? "" : payment.SettlementDate.Value.ToString("d MMM yyyy  mm:ss")) + "</td></tr>";
                    containsOnlyInts = false;
                }
                else
                {
                    allPaymentRefs += "<tr><td>" + payment.PaymentReference + "</td><td style=\"min-width:10px;\"></td><td>$" + payment.ScheduledAmount + "</td><td style=\"min-width:10px;\"></td><td>" + (payment.SettlementDate == null ? "" : payment.SettlementDate.Value.ToString("d MMM yyyy  mm:ss")) + "</td></tr>";
                }
            }

            if (containsOnlyInts)
            {
                Array.Sort(result.Data, delegate(px.ezidebit.com.au.Payment p1, px.ezidebit.com.au.Payment p2)
                {
                    return Convert.ToInt32(p1.PaymentReference).CompareTo(Convert.ToInt32(p2.PaymentReference));
                });
            }


            for (int i = 0; i < result.Data.Length; i++)
            {
                if (!Regex.IsMatch(result.Data[i].PaymentReference, @"^\d+$"))
                    continue;

                PaymentPending paymentPending = PaymentPendingDB.GetByID(DB, Convert.ToInt32(result.Data[i].PaymentReference));

                if (paymentPending == null)
                    continue;

                if (paymentPending.OutDateProcessed != DateTime.MinValue &&
                    paymentPending.OutPaymentResult == "A" &&
                    (result.Data[i].PaymentStatus.ToUpper() != "S" && result.Data[i].PaymentStatus.ToUpper() != "P"))
                    Emailer.SimpleAlertEmail(
                        "Ezidebit invoice payment added and set to \"A\" but payment status not in (\"S\",\"P\"): " + result.Data[i].PaymentStatus.ToUpper() + ".<br />payment_pending_id: " + paymentPending.PaymentPendingID + "<br />DB: " + (DB == null ? System.Web.HttpContext.Current.Session["DB"] : DB),
                        "Ezidebit Reconcilliation - Payment Status Mismatch",
                        true);
                
                if (paymentPending.OutDateProcessed != DateTime.MinValue)
                    continue;


                //
                // During real time transactions, results can be
                //
                // A = Approved
                // U = Unable to process at that time (Failed)
                // F = Failed                         (Failed)
                //
                // On the instant payment screen, we set in our DB as Approved (& generate receipt), or else we do not enter the result
                // There is no option (A/U/F) for Pending to update later
                //
                //
                // During this reconcilliation, results can be
                //
                // S   = Successful
                // P   = Pending (just means waiting for money to physically be sent to our bank)
                // F/D = (Dishonour/Fatal Dishonour)
                //
                //
                // Their instant payment page will always know if it was successful or failed at the time of transaction.
                //
                // So in the reconciliation web service, since 'Pending' is not a fail code, it means any payment 
                // set to Pending is definitely successful and just waiting for the money to be actually sent.
                //
                // Ezidebit support confirmed this.
                //

                if (result.Data[i].PaymentStatus.ToUpper() == "S" || result.Data[i].PaymentStatus.ToUpper() == "P")
                {
                    PaymentPendingDB.Update(DB, result.Data[i].TransactionTime.Value, paymentPending.PaymentPendingID, "A", "00", "APPROVED", result.Data[i].BankReceiptID, result.Data[i].PaymentID);

                    // update this invoice as paid!
                    if (!Convert.ToBoolean(ConfigurationManager.AppSettings["EziDebit_Debugging"]))
                    {
                        Invoice invoice = InvoiceDB.GetByID(paymentPending.InvoiceID);

                        if (result.Data[i].ScheduledAmount != (double)paymentPending.PaymentAmount)
                            Emailer.SimpleAlertEmail(
                                "Ezidebit invoice late payment added but initial payment amount and reconcilliation ammount differ (" + paymentPending.PaymentAmount + ", " + result.Data[i].ScheduledAmount + ")<br />payment_pending_id: " + paymentPending.PaymentPendingID + "<br />Invoice: " + invoice.InvoiceID + "<br />DB: " + (DB == null ? System.Web.HttpContext.Current.Session["DB"] : DB) + "<br />Original Amount: " + paymentPending.PaymentAmount + "<br />Ezidebit Sync Amount: " + result.Data[i].ScheduledAmount + "<br />Staff: " + StaffDB.GetByID(staffID).Person.FullnameWithoutMiddlename,
                                "Ezidebit Reconcilliation Amounts Differ. Invoice " + paymentPending.InvoiceID,
                                true);

                        decimal totalOwed  = invoice.TotalDue - paymentPending.PaymentAmount;
                        bool    isOverPaid = totalOwed <  0;
                        bool    isPaid     = totalOwed <= 0;

                        int receiptID = ReceiptDB.Insert(DB, 363, paymentPending.InvoiceID, paymentPending.PaymentAmount, 0, false, isOverPaid, DateTime.MinValue, staffID);

                        if (isPaid)
                            InvoiceDB.UpdateIsPaid(DB, invoice.InvoiceID, true);

                        if (isOverPaid)
                        {
                            // send email to someone .. to fix up the overpayment
                            Emailer.SimpleAlertEmail(
                                "Ezidebit invoice late web payment added and is overpaid.<br />payment_pending_id: " + paymentPending.PaymentPendingID + "<br />Invoice: " + invoice.InvoiceID + "<br />DB: " + (DB == null ? System.Web.HttpContext.Current.Session["DB"] : DB),
                                "Ezidebit Invoice OverPaid. Invoice: " + invoice.InvoiceID,
                                true);
                        }
                    }
                }
                if (result.Data[i].PaymentStatus.ToUpper() == "F" || result.Data[i].PaymentStatus.ToUpper() == "D")
                {
                    PaymentPendingDB.Update(DB, result.Data[i].TransactionTime.Value, paymentPending.PaymentPendingID, "F", result.Data[i].BankReturnCode, result.Data[i].BankFailedReason, result.Data[i].BankReceiptID, result.Data[i].PaymentID);
                }

            }


            System.Collections.Hashtable ppHash = new System.Collections.Hashtable();
            if (incOutput)
            {
                DataTable dt = PaymentPendingDB.GetDataTable(DB);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PaymentPending pp = PaymentPendingDB.Load(dt.Rows[i]);
                    ppHash[pp.PaymentPendingID] = pp;
                }
            }


            output += "<table id=\"tbl_output\" class=\"table table-bordered table-striped table-grid table-grid-top-bottum-padding-normal auto_width block_center\" border=\"1\">";

            output += @"<tr><th style=""vertical-align:top !important;"">" +
                   @"<br />Date                      </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Payment Reference         </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Payment Status
                    <table class=""text_left"">
                    <tr style=""white-space:nowrap;""><td>(<b>S</b> = Successful)</td></tr>
                    <tr style=""white-space:nowrap;""><td>(<b>F</b> = Failed)</td></tr>
                    <tr style=""white-space:nowrap;""><td>(<b>P</b> = Pending)</td></tr>
                    </table>                   </th><th style=""vertical-align:top !important;"">" +

                   @"<b>[Internal]<br/></b>Invoice ID           </th><th style=""vertical-align:top !important;"">" +
                   @"<b>[Internal]<br/></b>Customer Name        </th><th style=""vertical-align:top !important;"">" +
                   @"<b>[Internal]<br/></b>Payment Amount       </th><th style=""vertical-align:top !important;"">" +
                   @"<b>[Internal]<br/></b>OutPayment Result    
                        <table class=""text_left"">
                        <tr style=""white-space:nowrap;""><td>(<b>A</b> = Accepted)</td></tr>
                        <tr style=""white-space:nowrap;""><td>(<b>F</b> = Failed)</td></tr>
                        </table>                   </th><th style=""background-color:grey !important;"">" +

                   @"                          </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Bank Failed Reason        </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Bank Receipt ID           </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Bank Return Code          </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Customer Name             </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Debit Date                </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Settlement Date           </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Ezidebit Customer ID      </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Payment ID                </th><th style=""vertical-align:top !important;"">" +
                   (isStakeholder ? @"<br />Payment Amount            </th><th style=""vertical-align:top !important;"">" : "") +
                   @"<br />Payment Method            </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Payment Source            </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Scheduled Amount          </th><th style=""vertical-align:top !important;"">" +
                   (isStakeholder ? @"<br />Transaction Fee Client    </th><th style=""vertical-align:top !important;"">" : "") +
                   (isStakeholder ? @"<br />Transaction Fee Customer  </th><th style=""vertical-align:top !important;"">" : "") +
                   @"<br />Transaction Time          </th><th style=""vertical-align:top !important;"">" +
                   @"<br />Ezidebit Invoice ID       </th>";

            output += "</tr>";


            for (int i = result.Data.Length-1; i >= 0; i--)
            {
                PaymentPending pp = null;
                if (Regex.IsMatch(result.Data[i].PaymentReference, @"^\d+$"))
                    pp = ppHash[Convert.ToInt32(result.Data[i].PaymentReference)] as PaymentPending;

                bool failed = result.Data[i].PaymentStatus != "S" && result.Data[i].PaymentStatus != "P";

                string invLink = pp == null ? null : String.Format("Invoice_ViewV2.aspx?invoice_id={0}", pp.InvoiceID);
                string onClick = pp == null ? null : "javascript:window.showModalDialog('" + invLink + "', '', 'dialogWidth:775px;dialogHeight:900px;center:yes;resizable:no; scroll:no');return false;";

                output += "<tr" + (!failed ? "" : " style='color:red;' ") + "><td>" +

                    (pp == null ? "" : (pp.DateAdded.ToString("d MMM, yyyy") + " &nbsp;&nbsp;&nbsp; " + pp.DateAdded.ToString("HH:mm"))) + "&nbsp;</td><td>&nbsp;" +

                    (failed ? "<b>" : "") +  result.Data[i].PaymentReference + (failed ? "</b>" : "") + "&nbsp;</td><td>&nbsp;" +
                    (failed ? "<b>" : "") +  result.Data[i].PaymentStatus    + (failed ? "</b>" : "") + "&nbsp;</td><td>&nbsp;" +

                    (pp == null ? "" : "<a href=\"" + invLink + "\"" + (onClick == null ? "" : " onclick=\"" + onClick + "\"") + ">"+pp.InvoiceID+"</a>")    + "&nbsp;</td><td>&nbsp;" +
                    (pp == null ? "" : pp.CustomerName.ToString())      + "&nbsp;</td><td>&nbsp;" +
                    (pp == null ? "" : pp.PaymentAmount.ToString())     + "&nbsp;</td><td>&nbsp;" +
                    (pp == null ? "" : pp.OutPaymentResult.ToString())  + "&nbsp;</td><td style=\"background-color:grey;\">&nbsp;" +

                                                              "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].BankFailedReason         + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].BankReceiptID            + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].BankReturnCode           + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].CustomerName             + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].DebitDate                + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].SettlementDate           + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].EzidebitCustomerID       + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].PaymentID                + "&nbsp;</td><td>&nbsp;" +
                    (isStakeholder ? result.Data[i].PaymentAmount           + "&nbsp;</td><td>&nbsp;" : "") + 
                    result.Data[i].PaymentMethod            + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].PaymentSource            + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].ScheduledAmount          + "&nbsp;</td><td>&nbsp;" + 
                    (isStakeholder ? result.Data[i].TransactionFeeClient    + "&nbsp;</td><td>&nbsp;" : "") + 
                    (isStakeholder ? result.Data[i].TransactionFeeCustomer  + "&nbsp;</td><td>&nbsp;" : "") + 
                    result.Data[i].TransactionTime.Value    + "&nbsp;</td><td>&nbsp;" +
                    result.Data[i].InvoiceID                + "&nbsp;</td>" +
                    "</tr>";
            }
            output += "</table>";
        }
        else if (result.ErrorMessage != null && result.ErrorMessage.Length > 0)
        {
            for (int i = 0; i < result.ErrorMessage.Length; i++)
            {
                output += "EziDebit Error: " + result.ErrorMessage[i] + "<br />" + Environment.NewLine;
            }

            Emailer.SimpleAlertEmail(
                output,
                "EziDebit Web Service Error",
                true);
            Logger.LogQuery(output, false, true, false);
        }

        client.Close();  // Always close the client.

        return output;
    }


    public static DataTable GetDataTable(string DB)
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery(sql, DB).Tables[0];
    }

    public static PaymentPending GetByID(string DB, int payment_pending_id)
    {
        string sql = JoinedSql + " WHERE payment_pending_id = " + payment_pending_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql, DB).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static string JoinedSql = @"
                       SELECT   
                                payment_pending_id,invoice_id,payment_amount,customer_name,date_added,out_date_processed,out_payment_result,out_payment_result_code,out_payment_result_text,out_bank_receipt_id,out_paytecht_payment_id 
                       FROM 
                                PaymentPending ";

    public static PaymentPending Load(DataRow row, string prefix = "")
    {
        return new PaymentPending(
            Convert.ToInt32(row[prefix    + "payment_pending_id"]),
            Convert.ToInt32(row[prefix    + "invoice_id"]),
            Convert.ToDecimal(row[prefix  + "payment_amount"]),
            Convert.ToString(row[prefix   + "customer_name"]),
            Convert.ToDateTime(row[prefix + "date_added"]),
            row[prefix + "out_date_processed"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "out_date_processed"]),
            Convert.ToString(row[prefix + "out_payment_result"]),
            Convert.ToString(row[prefix + "out_payment_result_code"]),
            Convert.ToString(row[prefix + "out_payment_result_text"]),
            Convert.ToString(row[prefix + "out_bank_receipt_id"]),
            Convert.ToString(row[prefix + "out_paytecht_payment_id"])
        );
    }

}