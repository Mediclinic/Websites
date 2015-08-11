using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class TyroHealthClaimItemDB
{

    public static void Delete(int tyro_health_claim_item_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM TyroHealthClaimItem WHERE tyro_health_claim_item_id = " + tyro_health_claim_item_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    public static int Insert(int tyro_health_claim_id, decimal out_claimAmount, decimal out_rebateAmount, string out_serviceCode, string out_description, string out_serviceReference, string out_patientId, DateTime out_serviceDate, string out_responseCodeString)
    {
        out_serviceCode         = out_serviceCode.Replace("'", "''");
        out_description         = out_description.Replace("'", "''");
        out_serviceReference    = out_serviceReference.Replace("'", "''");
        out_patientId           = out_patientId.Replace("'", "''");
        out_responseCodeString  = out_responseCodeString.Replace("'", "''");
        string sql = "INSERT INTO TyroHealthClaimItem (tyro_health_claim_id,out_claimAmount,out_rebateAmount,out_serviceCode,out_description,out_serviceReference,out_patientId,out_serviceDate,out_responseCodeString) VALUES (" + "" + tyro_health_claim_id + "," + "" + out_claimAmount + "," + "" + out_rebateAmount + "," + "'" + out_serviceCode + "'," + "'" + out_description + "'," + "'" + out_serviceReference + "'," + "'" + out_patientId + "'," + "'" + out_serviceDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "'" + out_responseCodeString + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int tyro_health_claim_item_id, int tyro_health_claim_id, decimal out_claimAmount, decimal out_rebateAmount, string out_serviceCode, string out_description, string out_serviceReference, string out_patientId, DateTime out_serviceDate, string out_responseCodeString)
    {
        out_serviceCode         = out_serviceCode.Replace("'", "''");
        out_description         = out_description.Replace("'", "''");
        out_serviceReference    = out_serviceReference.Replace("'", "''");
        out_patientId           = out_patientId.Replace("'", "''");
        out_responseCodeString  = out_responseCodeString.Replace("'", "''");
        string sql = "UPDATE TyroHealthClaimItem SET tyro_health_claim_id = " + tyro_health_claim_id + ",out_claimAmount = " + out_claimAmount + ",out_rebateAmount = " + out_rebateAmount + ",out_serviceCode = '" + out_serviceCode + "',out_description = '" + out_description + "',out_serviceReference = '" + out_serviceReference + "',out_patientId = '" + out_patientId + "',out_serviceDate = '" + out_serviceDate.ToString("yyyy-MM-dd HH:mm:ss") + "',out_responseCodeString = '" + out_responseCodeString + "' WHERE tyro_health_claim_item_id = " + tyro_health_claim_item_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static TyroHealthClaimItem GetByID(int tyro_health_claim_item_id)
    {
        string sql = JoinedSql + " WHERE tyro_health_claim_item_id = " + tyro_health_claim_item_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static string JoinedSql = @"
                SELECT   
                        tyro_health_claim_item_id,tyro_health_claim_id,out_claimAmount,out_rebateAmount,out_serviceCode,out_description,out_serviceReference,out_patientId,out_serviceDate,out_responseCodeString 
                FROM 
                        TyroHealthClaimItem";

    public static TyroHealthClaimItem Load(DataRow row, string prefix = "")
    {
        return new TyroHealthClaimItem(
            Convert.ToInt32(row[prefix    + "tyro_health_claim_item_id"]),
            Convert.ToInt32(row[prefix    + "tyro_health_claim_id"]),
            Convert.ToDecimal(row[prefix  + "out_claimAmount"]),
            Convert.ToDecimal(row[prefix  + "out_rebateAmount"]),
            Convert.ToString(row[prefix   + "out_serviceCode"]),
            Convert.ToString(row[prefix   + "out_description"]),
            Convert.ToString(row[prefix   + "out_serviceReference"]),
            Convert.ToString(row[prefix   + "out_patientId"]),
            Convert.ToDateTime(row[prefix + "out_serviceDate"]),
            Convert.ToString(row[prefix   + "out_responseCodeString"])
        );
    }

}