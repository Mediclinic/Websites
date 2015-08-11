using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class OverpaymentDB
{

    public static void Delete(int overpayment_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Overpayment WHERE overpayment_id = " + overpayment_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int receipt_id, decimal total, int staff_id)
    {
        string sql = "INSERT INTO Overpayment (receipt_id,total,overpayment_date_added,staff_id) VALUES (" + "" + receipt_id + "," + "" + total + "," + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + "" + staff_id + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int overpayment_id, int receipt_id, decimal total)
    {
        string sql = "UPDATE Overpayment SET receipt_id = " + receipt_id + ",total = " + total + " WHERE overpayment_id = " + overpayment_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static DataTable GetDataTable()
    {
        string sql = "SELECT overpayment_id,receipt_id,total,overpayment_date_added,staff_id FROM Overpayment";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Overpayment GetByID(int overpayment_id)
    {
        string sql = "SELECT overpayment_id,receipt_id,total,overpayment_date_added,staff_id FROM Overpayment WHERE overpayment_id = " + overpayment_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : Load(tbl.Rows[0]);
    }

    public static int GetCountByReceiptID (int receipt_id)
    {
        string sql = "SELECT COUNT(*) FROM Overpayment WHERE receipt_id = " + receipt_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static void DeleteByReceiptID(int receipt_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Overpayment WHERE receipt_id = " + receipt_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }

    

    public static Overpayment Load(DataRow row)
    {
        return new Overpayment(
            Convert.ToInt32(row["overpayment_id"]),
            Convert.ToInt32(row["receipt_id"]),
            Convert.ToDecimal(row["total"]),
            Convert.ToDateTime(row["overpayment_date_added"]),
            Convert.ToInt32(row["staff_id"])
        );
    }

}