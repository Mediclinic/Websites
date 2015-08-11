using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class EPCInfo
{
    public int MedicareMaxNbrServicesPerYear;
    public int NbrMedicareServicesUsedSoFarThisYear;

    public int RamainingThisYear
    {
        get 
        {
            int ramainingThisYear = MedicareMaxNbrServicesPerYear - NbrMedicareServicesUsedSoFarThisYear;
            if (ramainingThisYear < 0)
                ramainingThisYear = 0;

            return ramainingThisYear;
        }
    }
    public int RemainingNextYear
    {
        get{ return MedicareMaxNbrServicesPerYear - RamainingThisYear; }
    }

    public bool BelowYearlyMedicareThreshhold
    {
        get { return NbrMedicareServicesUsedSoFarThisYear < MedicareMaxNbrServicesPerYear; }
    }


    public EPCInfo(int MedicareMaxNbrServicesPerYear, int NbrMedicareServicesUsedSoFarThisYear)
    {
        this.MedicareMaxNbrServicesPerYear = MedicareMaxNbrServicesPerYear;
        this.NbrMedicareServicesUsedSoFarThisYear = NbrMedicareServicesUsedSoFarThisYear;


    }
}