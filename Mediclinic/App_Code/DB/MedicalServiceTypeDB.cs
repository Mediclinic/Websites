using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;

public class MedicalServiceTypeDB
{

    public static Hashtable GetMedicareServiceTypeHash()
    {
        Hashtable medicalServiceTypeHash = new Hashtable();
        DataTable dt_MST = DBBase.GetGenericDataTable(null, "MedicalServiceType", "medical_service_type_id", "descr");
        for (int i = 0; i < dt_MST.Rows.Count; i++)
            medicalServiceTypeHash[dt_MST.Rows[i]["medical_service_type_id"].ToString()] = dt_MST.Rows[i]["descr"].ToString();

        return medicalServiceTypeHash;
    }

    public static IDandDescr[] GetAll()
    {
        DataTable dt_MST = DBBase.GetGenericDataTable(null, "MedicalServiceType", "medical_service_type_id", "descr");

        IDandDescr[] list = new IDandDescr[dt_MST.Rows.Count];
        Hashtable medicalServiceTypeHash = new Hashtable();
        for (int i = 0; i < dt_MST.Rows.Count; i++)
            list[i] = new IDandDescr(Convert.ToInt32(dt_MST.Rows[i]["medical_service_type_id"]), Convert.ToString(dt_MST.Rows[i]["descr"]));

        return list;
    }
    
}