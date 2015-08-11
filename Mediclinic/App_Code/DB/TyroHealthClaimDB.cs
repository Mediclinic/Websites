using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class TyroHealthClaimDB
{

    public static void Delete(int tyro_health_claim_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM TyroHealthClaim WHERE tyro_health_claim_id = " + tyro_health_claim_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static Tuple<int, string> Insert(int invoice_id, string db, decimal amount)
    {
        return Insert(invoice_id, db, amount, DateTime.MinValue, "", "", 0, DateTime.MinValue, DateTime.MinValue, "", "", "", 0, "", "");
    }
    public static Tuple<int, string> Insert(int invoice_id, string db, decimal amount, DateTime out_date_processed, string out_result, string out_healthpointRefTag, decimal out_healthpointTotalBenefitAmount, DateTime out_healthpointSettlementDateTime, DateTime out_healthpointTerminalDateTime, string out_healthpointMemberNumber, string out_healthpointProviderId, string out_healthpointServiceType, decimal out_healthpointGapAmount, string out_healthpointPhfResponseCode, string out_healthpointPhfResponseCodeDescription)
    {
        out_result                                = out_result.Replace("'", "''");
        out_healthpointRefTag                     = out_healthpointRefTag.Replace("'", "''");
        out_healthpointMemberNumber               = out_healthpointMemberNumber.Replace("'", "''");
        out_healthpointProviderId                 = out_healthpointProviderId.Replace("'", "''");
        out_healthpointServiceType                = out_healthpointServiceType.Replace("'", "''");
        out_healthpointPhfResponseCode            = out_healthpointPhfResponseCode.Replace("'", "''");
        out_healthpointPhfResponseCodeDescription = out_healthpointPhfResponseCodeDescription.Replace("'", "''");
        string sql = "INSERT INTO TyroHealthClaim (invoice_id,tyro_transaction_id,amount,date_added,out_date_processed,out_result,out_healthpointRefTag,out_healthpointTotalBenefitAmount,out_healthpointSettlementDateTime,out_healthpointTerminalDateTime,out_healthpointMemberNumber,out_healthpointProviderId,out_healthpointServiceType,out_healthpointGapAmount,out_healthpointPhfResponseCode,out_healthpointPhfResponseCodeDescription,date_cancelled) VALUES (" + "" + invoice_id + "," + "''," + "" + amount + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + (out_date_processed == DateTime.MinValue ? "NULL" : "'" + out_date_processed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + "'" + out_result + "'," + "'" + out_healthpointRefTag + "'," + "" + out_healthpointTotalBenefitAmount + "," + (out_healthpointSettlementDateTime == DateTime.MinValue ? "NULL" : "'" + out_healthpointSettlementDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (out_healthpointTerminalDateTime == DateTime.MinValue ? "NULL" : "'" + out_healthpointTerminalDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + "'" + out_healthpointMemberNumber + "'," + "'" + out_healthpointProviderId + "'," + "'" + out_healthpointServiceType + "'," + "" + out_healthpointGapAmount + "," + "'" + out_healthpointPhfResponseCode + "'," + "'" + out_healthpointPhfResponseCodeDescription + "'" + ",NULL);SELECT SCOPE_IDENTITY();";
        int tyro_health_claim_id = Convert.ToInt32(DBBase.ExecuteSingleResult(sql));

        string tyro_transaction_id = db.Substring(11) + "-" + tyro_health_claim_id.ToString().PadLeft(6, '0');
        string sql_update = "UPDATE TyroHealthClaim SET tyro_transaction_id = '" + tyro_transaction_id.Replace("'", "''") + "' WHERE tyro_health_claim_id = " + tyro_health_claim_id;
        DBBase.ExecuteNonResult(sql_update);

        return new Tuple<int, string>(tyro_health_claim_id, tyro_transaction_id);
    }

    public static void Update(int tyro_health_claim_id, string out_result, string out_healthpointRefTag, decimal out_healthpointTotalBenefitAmount, DateTime out_healthpointSettlementDateTime, DateTime out_healthpointTerminalDateTime, string out_healthpointMemberNumber, string out_healthpointProviderId, string out_healthpointServiceType, decimal out_healthpointGapAmount, string out_healthpointPhfResponseCode, string out_healthpointPhfResponseCodeDescription)
    {
        out_result                                = out_result.Replace("'", "''");
        out_healthpointRefTag                     = out_healthpointRefTag.Replace("'", "''");
        out_healthpointMemberNumber               = out_healthpointMemberNumber.Replace("'", "''");
        out_healthpointProviderId                 = out_healthpointProviderId.Replace("'", "''");
        out_healthpointServiceType                = out_healthpointServiceType.Replace("'", "''");
        out_healthpointPhfResponseCode            = out_healthpointPhfResponseCode.Replace("'", "''");
        out_healthpointPhfResponseCodeDescription = out_healthpointPhfResponseCodeDescription.Replace("'", "''");
        string sql = "UPDATE TyroHealthClaim SET out_date_processed = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',out_result = '" + out_result + "',out_healthpointRefTag = '" + out_healthpointRefTag + "',out_healthpointTotalBenefitAmount = " + out_healthpointTotalBenefitAmount + ",out_healthpointSettlementDateTime = '" + out_healthpointSettlementDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "',out_healthpointTerminalDateTime = '" + out_healthpointTerminalDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "',out_healthpointMemberNumber = '" + out_healthpointMemberNumber + "',out_healthpointProviderId = '" + out_healthpointProviderId + "',out_healthpointServiceType = '" + out_healthpointServiceType + "',out_healthpointGapAmount = " + out_healthpointGapAmount + ",out_healthpointPhfResponseCode = '" + out_healthpointPhfResponseCode + "',out_healthpointPhfResponseCodeDescription = '" + out_healthpointPhfResponseCodeDescription + "' WHERE tyro_health_claim_id = " + tyro_health_claim_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateByTyroTransactionID(string tyro_transaction_id, string out_result, string out_healthpointRefTag, decimal out_healthpointTotalBenefitAmount, DateTime out_healthpointSettlementDateTime, DateTime out_healthpointTerminalDateTime, string out_healthpointMemberNumber, string out_healthpointProviderId, string out_healthpointServiceType, decimal out_healthpointGapAmount, string out_healthpointPhfResponseCode, string out_healthpointPhfResponseCodeDescription)
    {
        tyro_transaction_id                       = tyro_transaction_id.Replace("'", "''");
        out_result                                = out_result.Replace("'", "''");
        out_healthpointRefTag                     = out_healthpointRefTag.Replace("'", "''");
        out_healthpointMemberNumber               = out_healthpointMemberNumber.Replace("'", "''");
        out_healthpointProviderId                 = out_healthpointProviderId.Replace("'", "''");
        out_healthpointServiceType                = out_healthpointServiceType.Replace("'", "''");
        out_healthpointPhfResponseCode            = out_healthpointPhfResponseCode.Replace("'", "''");
        out_healthpointPhfResponseCodeDescription = out_healthpointPhfResponseCodeDescription.Replace("'", "''");
        string sql = "UPDATE TyroHealthClaim SET out_date_processed = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',out_result = '" + out_result + "',out_healthpointRefTag = '" + out_healthpointRefTag + "',out_healthpointTotalBenefitAmount = " + out_healthpointTotalBenefitAmount + ",out_healthpointSettlementDateTime = '" + out_healthpointSettlementDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "',out_healthpointTerminalDateTime = '" + out_healthpointTerminalDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "',out_healthpointMemberNumber = '" + out_healthpointMemberNumber + "',out_healthpointProviderId = '" + out_healthpointProviderId + "',out_healthpointServiceType = '" + out_healthpointServiceType + "',out_healthpointGapAmount = " + out_healthpointGapAmount + ",out_healthpointPhfResponseCode = '" + out_healthpointPhfResponseCode + "',out_healthpointPhfResponseCodeDescription = '" + out_healthpointPhfResponseCodeDescription + "' WHERE tyro_transaction_id = '" + tyro_transaction_id + "'";
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateCancelled(string out_healthpointRefTag)
    {
        string sql = "UPDATE TyroHealthClaim SET date_cancelled = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE out_healthpointRefTag = '" + out_healthpointRefTag + "'";
        DBBase.ExecuteNonResult(sql);
    }


    public static DataTable GetDataTable()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery( sql ).Tables[0];
    }

    public static TyroHealthClaim GetByID(int tyro_health_claim_id)
    {
        string sql = JoinedSql + " WHERE tyro_health_claim_id = " + tyro_health_claim_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static TyroHealthClaim GetByByTyroTransactionID(string tyro_transaction_id)
    {
        tyro_transaction_id = tyro_transaction_id.Replace("'", "''");
        string sql = JoinedSql + " WHERE tyro_transaction_id = '" + tyro_transaction_id + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static TyroHealthClaim GetByRefTag(string refTag)
    {
        refTag = refTag.Replace("'", "''");
        string sql = JoinedSql + " WHERE out_healthpointRefTag = '" + refTag + "'";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static TyroHealthClaim[] GetByInvoice(int invoice_id, bool onlyApproved = true, bool excCancelled = true)
    {
        string sql = JoinedSql + " WHERE invoice_id = " + invoice_id + (onlyApproved ? " AND out_result = 'APPROVED' " : "") + (excCancelled ? " AND date_cancelled IS NULL " : "");
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        TyroHealthClaim[] list = new TyroHealthClaim[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = Load(tbl.Rows[i]);

        return list;
    }
    

    public static string JoinedSql = @"
                SELECT   
                        tyro_health_claim_id,invoice_id,tyro_transaction_id,amount,date_added,out_date_processed,out_result,out_healthpointRefTag,out_healthpointTotalBenefitAmount,out_healthpointSettlementDateTime,out_healthpointTerminalDateTime,out_healthpointMemberNumber,out_healthpointProviderId,out_healthpointServiceType,out_healthpointGapAmount,out_healthpointPhfResponseCode,out_healthpointPhfResponseCodeDescription,date_cancelled
                FROM 
                        TyroHealthClaim ";

    public static TyroHealthClaim Load(DataRow row, string prefix = "")
    {
        return new TyroHealthClaim(
            Convert.ToInt32(row[prefix    + "tyro_health_claim_id"]),
            Convert.ToInt32(row[prefix    + "invoice_id"]),
            Convert.ToString(row[prefix   + "tyro_transaction_id"]),
            Convert.ToDecimal(row[prefix  + "amount"]),
            Convert.ToDateTime(row[prefix + "date_added"]),
            row[prefix + "out_date_processed"]                == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "out_date_processed"]),
            Convert.ToString(row[prefix   + "out_result"]),
            Convert.ToString(row[prefix   + "out_healthpointRefTag"]),
            Convert.ToDecimal(row[prefix  + "out_healthpointTotalBenefitAmount"]),
            row[prefix + "out_healthpointSettlementDateTime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "out_healthpointSettlementDateTime"]),
            row[prefix + "out_healthpointTerminalDateTime"]   == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "out_healthpointTerminalDateTime"]),
            Convert.ToString(row[prefix   + "out_healthpointMemberNumber"]),
            Convert.ToString(row[prefix   + "out_healthpointProviderId"]),
            Convert.ToString(row[prefix   + "out_healthpointServiceType"]),
            Convert.ToDecimal(row[prefix  + "out_healthpointGapAmount"]),
            Convert.ToString(row[prefix   + "out_healthpointPhfResponseCode"]),
            Convert.ToString(row[prefix   + "out_healthpointPhfResponseCodeDescription"]),
            row[prefix + "date_cancelled"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_cancelled"])
        );
    }

}