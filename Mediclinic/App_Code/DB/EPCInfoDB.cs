using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

public class EPCInfoDB
{
    public static EPCInfo GetEPCInfo(int patient_id)
    {
        int  MedicareMaxNbrServicesPerYear        = Convert.ToInt32(SystemVariableDB.GetByDescr("MedicareMaxNbrServicesPerYear").Value);
        int  NbrMedicareServicesUsedSoFarThisYear = (int)InvoiceDB.GetMedicareCountByPatientAndYear(patient_id, DateTime.Now.Year);

        return new EPCInfo(MedicareMaxNbrServicesPerYear, NbrMedicareServicesUsedSoFarThisYear);
    }
}
