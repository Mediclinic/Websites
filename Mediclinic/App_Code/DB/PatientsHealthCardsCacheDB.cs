using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

public class PatientsHealthCardsCacheDB
{

    public static Hashtable GetBullkActive(int[] patient_ids)
    {
        // remove duplicates
        ArrayList uniquePatientIDs = new ArrayList();
        for (int i = 0; i < patient_ids.Length; i++)
            if (!uniquePatientIDs.Contains(patient_ids[i]))
                uniquePatientIDs.Add(patient_ids[i]);
        patient_ids = (int[])uniquePatientIDs.ToArray(typeof(int));



        HealthCard[] patientHealthCards = HealthCardDB.GetByPatientIDs(patient_ids);

        Hashtable patientCards = new Hashtable();
        for (int i = 0; i < patientHealthCards.Length; i++)
        {
            if (patientCards[patientHealthCards[i].Patient.PatientID] == null)
                patientCards[patientHealthCards[i].Patient.PatientID] = new PatientActiveHealthCards();

            ((PatientActiveHealthCards)patientCards[patientHealthCards[i].Patient.PatientID]).Add(patientHealthCards[i]);
        }

        return patientCards;
    }

    public static Hashtable GetBullkMostRecent(int[] patient_ids, int organisation_id)
    {
        // remove duplicates
        ArrayList uniquePatientIDs = new ArrayList();
        for (int i = 0; i < patient_ids.Length; i++)
            if (!uniquePatientIDs.Contains(patient_ids[i]))
                uniquePatientIDs.Add(patient_ids[i]);
        patient_ids = (int[])uniquePatientIDs.ToArray(typeof(int));



        HealthCard[] patientHealthCards = HealthCardDB.GetByPatientIDs(patient_ids, false, organisation_id);

        Hashtable patientCards = new Hashtable();
        for (int i = 0; i < patientHealthCards.Length; i++)
        {
            if (patientCards[patientHealthCards[i].Patient.PatientID] == null)
                patientCards[patientHealthCards[i].Patient.PatientID] = new PatientActiveHealthCards();

            ((PatientActiveHealthCards)patientCards[patientHealthCards[i].Patient.PatientID]).Add(patientHealthCards[i]);
        }

        return patientCards;
    }

}