using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PatientReferrer
{

    public PatientReferrer(int patient_referrer_id, int patient_id, int register_referrer_id, int organisation_id, DateTime patient_referrer_date_added, bool is_debtor)
    {
        this.patient_referrer_id            = patient_referrer_id;
        this.patient                        = new Patient(patient_id);
        this.registerReferrer               = register_referrer_id == -1 ? null : new RegisterReferrer(register_referrer_id);
        this.organisation                   = organisation_id      ==  0 ? null : new Organisation(organisation_id);
        this.patient_referrer_date_added    = patient_referrer_date_added;
        this.is_debtor                      = is_debtor;
    }

    private int patient_referrer_id;
    public int PatientReferrerID
    {
        get { return this.patient_referrer_id; }
        set { this.patient_referrer_id = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private RegisterReferrer registerReferrer;
    public RegisterReferrer RegisterReferrer
    {
        get { return this.registerReferrer; }
        set { this.registerReferrer = value; }
    }
    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private DateTime patient_referrer_date_added;
    public DateTime PatientReferrerDateAdded
    {
        get { return this.patient_referrer_date_added; }
        set { this.patient_referrer_date_added = value; }
    }
    private bool is_debtor;
    public bool IsDebtor
    {
        get { return this.is_debtor; }
        set { this.is_debtor = value; }
    }
    public override string ToString()
    {
        return patient_referrer_id.ToString() + " " + patient.PatientID.ToString() + " " + registerReferrer.RegisterReferrerID.ToString() + " " + patient_referrer_date_added.ToString() + " " + is_debtor.ToString();
    }

}