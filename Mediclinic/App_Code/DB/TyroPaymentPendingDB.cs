using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;
using System.Xml;
using System.Text;
using System.IO;
using System.Globalization;


public class TyroPaymentPendingDB
{

    public static void Delete(int tyro_payment_pending_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM TyroPaymentPending WHERE tyro_payment_pending_id = " + tyro_payment_pending_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static Tuple<int, string> Insert(int invoice_id, string db, int tyro_payment_type_id, decimal amount, decimal cashout)
    {
        string sql = "INSERT INTO TyroPaymentPending (invoice_id,tyro_transaction_id,tyro_payment_type_id,amount,cashout,date_added,out_date_processed,out_result,out_cardType,out_transactionReference,out_authorisationCode,out_issuerActionCode) VALUES (" + "" + invoice_id + "," + "''," + "" + tyro_payment_type_id + "," + "" + amount + "," + "" + cashout + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "NULL," + "''," + "''," + "''," + "''," + "''" + ");SELECT SCOPE_IDENTITY();";
        int tyro_payment_pending_id = Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
        
        string tyro_transaction_id = db.Substring(11) + "-" + tyro_payment_pending_id.ToString().PadLeft(6,'0');
        string sql_update = "UPDATE TyroPaymentPending SET tyro_transaction_id = '" + tyro_transaction_id.Replace("'", "''") + "' WHERE tyro_payment_pending_id = " + tyro_payment_pending_id;
        DBBase.ExecuteNonResult(sql_update);

        return new Tuple<int, string>(tyro_payment_pending_id, tyro_transaction_id);
    }

    public static void Update(string DB, int tyro_payment_pending_id, string out_result, string out_cardType, string out_transactionReference, string out_authorisationCode, string out_issuerActionCode)
    {
        out_result               = out_result.Replace("'", "''");
        out_cardType             = out_cardType.Replace("'", "''");
        out_transactionReference = out_transactionReference.Replace("'", "''");
        out_authorisationCode    = out_authorisationCode.Replace("'", "''");
        out_issuerActionCode     = out_issuerActionCode.Replace("'", "''");
        string sql = "UPDATE TyroPaymentPending SET out_date_processed = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',out_result = '" + out_result + "',out_cardType = '" + out_cardType + "',out_transactionReference = '" + out_transactionReference + "',out_authorisationCode = '" + out_authorisationCode + "',out_issuerActionCode = '" + out_issuerActionCode + "' WHERE tyro_payment_pending_id = " + tyro_payment_pending_id.ToString();
        DBBase.ExecuteNonResult(sql, DB);
    }
    public static void UpdateByTyroTransactionID(string DB, string tyro_transaction_id, string out_result, string out_cardType, string out_transactionReference, string out_authorisationCode, string out_issuerActionCode, DateTime out_date_processed)
    {
        tyro_transaction_id = tyro_transaction_id.Replace("'", "''");
        out_result = out_result.Replace("'", "''");
        out_cardType = out_cardType.Replace("'", "''");
        out_transactionReference = out_transactionReference.Replace("'", "''");
        out_authorisationCode = out_authorisationCode.Replace("'", "''");
        out_issuerActionCode = out_issuerActionCode.Replace("'", "''");
        string sql = "UPDATE TyroPaymentPending SET out_date_processed = '" + out_date_processed.ToString("yyyy-MM-dd HH:mm:ss") + "',out_result = '" + out_result + "',out_cardType = '" + out_cardType + "',out_transactionReference = '" + out_transactionReference + "',out_authorisationCode = '" + out_authorisationCode + "',out_issuerActionCode = '" + out_issuerActionCode + "' WHERE tyro_transaction_id = '" + tyro_transaction_id.ToString() + "'";
        DBBase.ExecuteNonResult(sql, DB);
    }

    public static void Reconcile(string DB, DateTime date, string xml)
    {
        /*
        string xmlString =
              @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <reconciliation-detail mid=""200"" tid=""200"" terminal-business-day=""2015-04-14"" total=""1620.00"">
                <transaction type=""purchase"" card-type=""visa"" amount=""100.00"" tip=""10.00"" transaction-local-date-time=""2015-04-14 12:30:30"" tyro-reference=""870020"" merchant-reference=""071061048231306351219677"" settlement-date=""2015-04-16""/>
                <transaction type=""purchase"" card-type=""mastercard"" amount=""200.00"" transaction-local-date-time=""2015-04-14 12:31:20"" tyro-reference=""885214"" merchant-reference=""071061048231306351219678"" settlement-date=""2015-04-16""/>
                <transaction type=""purchase"" card-type=""jcb"" amount=""40.00"" transaction-local-date-time=""2015-04-14 12:33:23"" tyro-reference=""896534"" merchant-reference=""071061048231306351219679""/>
                <transaction type=""purchase"" card-type=""amex"" amount=""30.00"" transaction-local-date-time=""2015-04-14 12:36:35"" tyro-reference=""905845"" merchant-reference=""071061048231306351219680""/>
                <transaction type=""purchase"" card-type=""eftpos"" amount=""650.00"" cash-out=""50.00"" transaction-local-date-time=""2015-04-14 12:40:30"" tyro-reference=""912556"" merchant-reference=""071061048231306351219681"" settlement-date=""2015-04-16""/>
                <transaction type=""purchase"" card-type=""eftpos"" amount=""450.00"" transaction-local-date-time=""2015-04-14 12:50:30"" tyro-reference=""920187"" merchant-reference=""071061048231306351219682"" settlement-date=""2015-04-16""/>
                <transaction type=""purchase"" card-type=""diners"" amount=""70.00"" transaction-local-date-time=""2015-04-14 13:30:30"" tyro-reference=""935587"" merchant-reference=""071061048231306351219683""/>
                <transaction type=""void"" card-type=""mastercard"" amount=""-80.00"" transaction-local-date-time=""2015-04-14 13:50:30"" tyro-reference=""946585"" merchant-reference=""071061048231306351219684"" settlement-date=""2015-04-16""/>
                <transaction type=""purchase"" card-type=""visa"" amount=""170.00"" transaction-local-date-time=""2015-04-14 14:23:25"" tyro-reference=""953594"" merchant-reference=""071061048231306351219685"" settlement-date=""2015-04-16""/>
                <transaction type=""refund"" card-type=""visa"" amount=""-70.00"" transaction-local-date-time=""2015-04-14 15:41:12"" tyro-reference=""962548"" merchant-reference=""071061048231306351219685"" settlement-date=""2015-04-16""/>
                </reconciliation-detail>";
        */

        StringBuilder debutOutput = new StringBuilder();

        using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
        {
            reader.ReadToFollowing("reconciliation-detail");

            string mid = reader.GetAttribute("mid");
            string tid = reader.GetAttribute("tid");
            string terminal_business_day = reader.GetAttribute("terminal-business-day");
            string total = reader.GetAttribute("total");

            debutOutput.AppendLine("reconciliation-detail :  " + 
                " mid:"                   + mid + 
                " tid:"                   + tid + 
                " terminal_business_day:" + terminal_business_day + 
                " total:"                 + total);

            while (reader.ReadToFollowing("transaction"))
            {
                string type                         = reader.GetAttribute("type");
                string card_type                    = reader.GetAttribute("card-type");
                string _amount                       = reader.GetAttribute("amount");
                string cash_out                     = reader.GetAttribute("cash-out");
                string tip                          = reader.GetAttribute("tip");
                string transaction_local_date_time  = reader.GetAttribute("transaction-local-date-time");
                string tyro_reference               = reader.GetAttribute("tyro-reference");
                string merchant_reference           = reader.GetAttribute("merchant-reference");       // transactionId that we sent in = tyro_payment_pending_id
                string settlement_date              = reader.GetAttribute("settlement-date");

                debutOutput.AppendLine("transaction :  " +
                    " type:" + type +
                    " card_type:" + card_type +
                    " amount:" + _amount +
                    " cash_out:" + cash_out +
                    " tip:" + tip +
                    " transaction_local_date_time:" + transaction_local_date_time +
                    " tyro_reference:" + tyro_reference +
                    " merchant_reference:" + merchant_reference +
                    " settlement_date:" + settlement_date
                    );


                /*
                select * from TyroPaymentPending

                update TyroPaymentPending
                set 
	                tyro_transaction_id = '071061048231306351219677',
	                amount = 101.00,
	                out_date_processed = NULL,
	                out_result = '',
	                out_cardType = '',
	                out_transactionReference = '',
	                out_authorisationCode = '',
	                out_issuerActionCode = ''
                WHERE tyro_payment_pending_id = 1
                */


                TyroPaymentPending tyroPaymentPending = TyroPaymentPendingDB.GetByByTyroTransactionID(DB, merchant_reference);

                if (tyroPaymentPending == null)
                    continue;
                if (tyroPaymentPending.OutDateProcessed != DateTime.MinValue)
                    continue;

                int tyro_payment_type_id = -1;
                if (type == "purchase")
                    tyro_payment_type_id = 1;
                else if (type == "refund")
                    tyro_payment_type_id = 2;

                bool sucessfulTransaction = (tyro_payment_type_id == 1 || tyro_payment_type_id == 2);
                if (sucessfulTransaction)
                {
                    decimal amount;
                    if (!Decimal.TryParse(_amount, out amount))
                    {
                        Emailer.SimpleAlertEmail(
                            "Tyro invoice late payment added but amount is not decimal type (" + _amount + ")<br />tyro_payment_pending_id: " + tyroPaymentPending.TyroPaymentPendingID + "<br />Invoice: " + tyroPaymentPending.InvoiceID + "<br />DB: " + DB + "<br /><br />" + xml.Replace("<", "&lt;").Replace(">", "&gt;").Replace(Environment.NewLine, "<br />"),
                            "Tyro Reconcilliation Amount Not Decimal Type. Invoice : " + tyroPaymentPending.InvoiceID,
                            true);
                        continue;
                    }

                    DateTime transactionDateTime;
                    if (!DateTime.TryParseExact(transaction_local_date_time, "yyyy-dd-MM hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out transactionDateTime))
                    {
                        Emailer.SimpleAlertEmail(
                            "Tyro invoice late payment added but transaction_local_date_time is not parsable (" + transaction_local_date_time + ")<br />tyro_payment_pending_id: " + tyroPaymentPending.TyroPaymentPendingID + "<br />Invoice: " + tyroPaymentPending.InvoiceID + "<br />DB: " + DB + "<br /><br />" + xml.Replace("<", "&lt;").Replace(">", "&gt;").Replace(Environment.NewLine, "<br />"),
                            "Tyro Reconcilliation Amount Not Decimal Type. Invoice : " + tyroPaymentPending.InvoiceID,
                            true);
                        continue;
                    }

                    TyroPaymentPendingDB.UpdateByTyroTransactionID(DB, merchant_reference, "APPROVED", card_type, tyro_reference, "", "", transactionDateTime);

                    if (amount != tyroPaymentPending.Amount)
                        Emailer.SimpleAlertEmail(
                            "Tyro invoice late payment added but initial payment amount and reconcilliation ammount differ (" + tyroPaymentPending.Amount + ", " + amount + ")<br />tyro_payment_pending_id: " + tyroPaymentPending.TyroPaymentPendingID + "<br />Invoice: " + tyroPaymentPending.InvoiceID + "<br />DB: " + DB,
                            "Tyro Reconcilliation Amounts Differ. Invoice : " + tyroPaymentPending.InvoiceID,
                            true);

                    if (tyroPaymentPending.TyroPaymentTypeID != tyro_payment_type_id)
                        Emailer.SimpleAlertEmail(
                            "Tyro invoice late payment added but payment types differ (" + tyroPaymentPending.TyroPaymentTypeID + ", " + tyro_payment_type_id + ")<br />tyro_payment_pending_id: " + tyroPaymentPending.TyroPaymentPendingID + "<br />Invoice: " + tyroPaymentPending.InvoiceID + "<br />DB: " + DB,
                            "Tyro Reconcilliation Types Differ. Invoice : " + tyroPaymentPending.InvoiceID,
                            true);


                    Invoice invoice = InvoiceDB.GetByID(tyroPaymentPending.InvoiceID, DB);

                    if (tyroPaymentPending.TyroPaymentTypeID == 1) // payment
                    {
                        decimal totalOwed  = invoice.TotalDue - tyroPaymentPending.Amount;
                        bool    isOverPaid = totalOwed <  0;
                        bool    isPaid     = totalOwed <= 0;

                        ReceiptDB.Insert(DB, 364, tyroPaymentPending.InvoiceID, tyroPaymentPending.Amount, 0, false, isOverPaid, DateTime.MaxValue, -8);

                        if (isPaid)
                            InvoiceDB.UpdateIsPaid(DB, invoice.InvoiceID, true);

                        if (isOverPaid)
                        {
                            // send email to someone .. to fix up the overpayment....
                            Emailer.SimpleAlertEmail(
                                "Tyro invoice late payment added and is overpaid.<br />tyro_payment_pending_id: " + tyroPaymentPending.TyroPaymentPendingID + "<br />Invoice: " + invoice.InvoiceID + "<br />DB: " + DB,
                                "Tyro Invoice OverPaid: " + invoice.InvoiceID,
                                true);
                        }
                    }
                    if (tyroPaymentPending.TyroPaymentTypeID == 2) // refund
                    {
                        amount = amount * -1; // reconcilliation report shows refund amount as negative amounts

                        decimal totalOwed  = invoice.TotalDue + tyroPaymentPending.Amount;
                        bool    isPaid     = totalOwed <= 0;

                        RefundDB.Insert(tyroPaymentPending.InvoiceID, tyroPaymentPending.Amount, 308, "", -8, DB);

                        if (totalOwed > 0)
                            InvoiceDB.UpdateIsPaid(DB, tyroPaymentPending.InvoiceID, false);
                    }

                    Emailer.SimpleAlertEmail(
                        "Tyro Invoice Payment Updated Asynchonously (ie Late). <br />tyro_payment_pending_id: " + tyroPaymentPending.TyroPaymentPendingID + "<br />Invoice: " + invoice.InvoiceID + "<br />DB: " + DB,
                        "Tyro Invoice Payment Updated Asynchonously (ie Late). Invoice: " + invoice.InvoiceID,
                        true);
                }
            }
        }

        Logger.LogQuery(debutOutput.ToString());
    }

    public static DataTable GetDataTable()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery( sql ).Tables[0];
    }

    public static TyroPaymentPending GetByID(string DB, int tyro_payment_pending_id)
    {
        string sql = JoinedSql + " WHERE tyro_payment_pending_id = " + tyro_payment_pending_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql, DB).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static TyroPaymentPending GetByByTyroTransactionID(string DB, string tyro_transaction_id)
    {
        tyro_transaction_id = tyro_transaction_id.Replace("'", "''");
        string sql = JoinedSql + " WHERE tyro_transaction_id = '" + tyro_transaction_id.ToString() + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql, DB).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static string JoinedSql = @"
                SELECT   
                        tyro_payment_pending_id,invoice_id,tyro_transaction_id,tyro_payment_type_id,amount,cashout,date_added,out_date_processed,out_result,out_cardType,out_transactionReference,out_authorisationCode,out_issuerActionCode
                FROM 
                        TyroPaymentPending ";

    public static TyroPaymentPending Load(DataRow row, string prefix = "")
    {
        return new TyroPaymentPending(
            Convert.ToInt32(row[prefix    + "tyro_payment_pending_id"]),
            Convert.ToInt32(row[prefix    + "invoice_id"]),
            Convert.ToString(row[prefix   + "tyro_transaction_id"]),
            Convert.ToInt32(row[prefix    + "tyro_payment_type_id"]),
            Convert.ToDecimal(row[prefix  + "amount"]),
            Convert.ToDecimal(row[prefix  + "cashout"]),
            Convert.ToDateTime(row[prefix + "date_added"]),
            row[prefix + "out_date_processed"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "out_date_processed"]),
            Convert.ToString(row[prefix   + "out_result"]),
            Convert.ToString(row[prefix   + "out_cardType"]),
            Convert.ToString(row[prefix   + "out_transactionReference"]),
            Convert.ToString(row[prefix   + "out_authorisationCode"]),
            Convert.ToString(row[prefix   + "out_issuerActionCode"])
        );
    }

}