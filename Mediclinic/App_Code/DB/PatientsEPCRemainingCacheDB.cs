using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

public class PatientsEPCRemainingCacheDB
{
    public static Hashtable GetBullk(int[] patient_ids, DateTime start_date)
    {
        if (patient_ids.Length == 0)
            return new Hashtable();

        // remove duplicates
        ArrayList uniquePatientIDs = new ArrayList();
        for (int i = 0; i < patient_ids.Length; i++)
            if (!uniquePatientIDs.Contains(patient_ids[i]))
                uniquePatientIDs.Add(patient_ids[i]);
        patient_ids = (int[])uniquePatientIDs.ToArray(typeof(int));

        return HealthCardEPCRemainingDB.GetTotalServicesRemainingByPatients(patient_ids, start_date);
    }
}






