using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class RegisterPatient
{

    public RegisterPatient(int register_patient_id, int organisation_id, int patient_id, DateTime register_patient_date_added)
    {
        this.register_patient_id = register_patient_id;
        this.organisation = new Organisation(organisation_id);
        this.patient = new Patient(patient_id);
        this.register_patient_date_added = register_patient_date_added;
    }

    private int register_patient_id;
    public int RegisterPatientID
    {
        get { return this.register_patient_id; }
        set { this.register_patient_id = value; }
    }
    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private DateTime register_patient_date_added;
    public DateTime RegisterPatientDateAdded
    {
        get { return this.register_patient_date_added; }
        set { this.register_patient_date_added = value; }
    }
    public override string ToString()
    {
        return register_patient_id.ToString() + " " + organisation.OrganisationID.ToString() + " " + patient.PatientID.ToString() + " " + register_patient_date_added.ToString();
    }


    public static RegisterPatient[] RemoveByID(RegisterPatient[] registered_patients, int id_to_remove)
    {
        RegisterPatient[] newList = new RegisterPatient[registered_patients.Length - 1];

        bool found = false;
        for (int i = 0; i < registered_patients.Length; i++)
        {
            if (registered_patients[i].RegisterPatientID != id_to_remove)
                newList[i - (found ? 1 : 0)] = registered_patients[i];
            else
                found = true;
        }

        return newList;
    }

}