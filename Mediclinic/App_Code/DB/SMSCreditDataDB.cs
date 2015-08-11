using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class SMSCreditDataDB
{

    public static void Delete(int sms_credit_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM SMSCredit WHERE sms_credit_id = " + sms_credit_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(decimal amount, DateTime datetime_added)
    {
        string sql = "INSERT INTO SMSCredit (amount,datetime_added) VALUES (" + "" + amount + "," + "'" + datetime_added.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int sms_credit_id, decimal amount, DateTime datetime_added)
    {
        string sql = "UPDATE SMSCredit SET amount = " + amount + ",datetime_added = '" + datetime_added.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE sms_credit_id = " + sms_credit_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT sms_credit_id,amount,datetime_added FROM SMSCredit";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static SMSCreditData GetByID(int sms_credit_id)
    {
        string sql = "SELECT sms_credit_id,amount,datetime_added FROM SMSCredit WHERE sms_credit_id = " + sms_credit_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static  decimal GetTotal()
    {
        string sql = "SELECT COALESCE(sum(amount), 0) FROM SMSCredit";
        return Convert.ToDecimal(DBBase.ExecuteSingleResult(sql));
    }


    public static SMSCreditData Load(DataRow row)
    {
        return new SMSCreditData(
            Convert.ToInt32(row["sms_credit_id"]),
            Convert.ToDecimal(row["amount"]),
            Convert.ToDateTime(row["datetime_added"])
        );
    }

}