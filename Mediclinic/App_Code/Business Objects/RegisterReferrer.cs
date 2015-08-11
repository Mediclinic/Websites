using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class RegisterReferrer
{

    public RegisterReferrer(int register_referrer_id, int organisation_id, int referrer_id, string provider_number, 
        bool report_every_visit_to_referrer, bool batch_send_all_patients_treatment_notes, DateTime date_last_batch_send_all_patients_treatment_notes, 
        DateTime register_referrer_date_added)
    {
        this.register_referrer_id = register_referrer_id;
        this.organisation = organisation_id == 0 ? null : new Organisation(organisation_id);
        this.referrer = new Referrer(referrer_id);
        this.provider_number = provider_number;
        this.report_every_visit_to_referrer = report_every_visit_to_referrer;
        this.batch_send_all_patients_treatment_notes = batch_send_all_patients_treatment_notes;
        this.date_last_batch_send_all_patients_treatment_notes = date_last_batch_send_all_patients_treatment_notes;
        this.register_referrer_date_added = register_referrer_date_added;
    }
    public RegisterReferrer(int register_referrer_id)
    {
        this.register_referrer_id = register_referrer_id;
    }

    private int register_referrer_id;
    public int RegisterReferrerID
    {
        get { return this.register_referrer_id; }
        set { this.register_referrer_id = value; }
    }
    private Organisation organisation;
    public Organisation Organisation
    {
        get { return this.organisation; }
        set { this.organisation = value; }
    }
    private Referrer referrer;
    public Referrer Referrer
    {
        get { return this.referrer; }
        set { this.referrer = value; }
    }
    private string provider_number;
    public string ProviderNumber
    {
        get { return this.provider_number; }
        set { this.provider_number = value; }
    }
    private bool report_every_visit_to_referrer;
    public bool ReportEveryVisitToReferrer
    {
        get { return this.report_every_visit_to_referrer; }
        set { this.report_every_visit_to_referrer = value; }
    }
    private bool batch_send_all_patients_treatment_notes;
    public bool BatchSendAllPatientsTreatmentNotes
    {
        get { return this.batch_send_all_patients_treatment_notes; }
        set { this.batch_send_all_patients_treatment_notes = value; }
    }
    private DateTime date_last_batch_send_all_patients_treatment_notes;
    public DateTime DateLastBatchSendAllPatientsTreatmentNotes
    {
        get { return this.date_last_batch_send_all_patients_treatment_notes; }
        set { this.date_last_batch_send_all_patients_treatment_notes = value; }
    }
    private DateTime register_referrer_date_added;
    public DateTime RegisterReferrerDateAdded
    {
        get { return this.register_referrer_date_added; }
        set { this.register_referrer_date_added = value; }
    }
    public override string ToString()
    {
        return register_referrer_id.ToString() + " " + organisation.OrganisationID.ToString() + " " + Referrer.ReferrerID.ToString() + " " + register_referrer_date_added.ToString();
    }

    public static RegisterReferrer[] RemoveByID(RegisterReferrer[] registered_referrers, int id_to_remove)
    {
        RegisterReferrer[] newList = new RegisterReferrer[registered_referrers.Length - 1];

        bool found = false;
        for (int i = 0; i < registered_referrers.Length; i++)
        {
            if (registered_referrers[i].RegisterReferrerID != id_to_remove)
                newList[i - (found ? 1 : 0)] = registered_referrers[i];
            else
                found = true;
        }

        return newList;
    }

}