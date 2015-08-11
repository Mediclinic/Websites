using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class MedicareClaimNbrDB
{

    public static string InsertIntoInvoice(int invID, DateTime date)
    {
        // set claim number if medicare/dva invoice
        System.Collections.Hashtable spParams = new System.Collections.Hashtable();
        spParams["@invoice_id"] = invID;
        spParams["@invoice_date"] = date;
        string claimNbr = DBBase.ExecuteSingleResult_SP("uspCreateClaimNumber", spParams).ToString();
        return claimNbr;
    }

}