using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Offering
{

    public Offering(int offering_id, int offering_type_id, int field_id, int aged_care_patient_type_id, 
                int num_clinic_visits_allowed_per_year, int offering_invoice_type_id, 
                string name, string short_name, string descr, bool is_gst_exempt, decimal default_price, int service_time_minutes, int max_nbr_claimable, int max_nbr_claimable_months, string medicare_company_code, string dva_company_code, string tac_company_code, 
                decimal medicare_charge, decimal dva_charge, decimal tac_charge, 
                string popup_message, int reminder_letter_months_later_to_send, int reminder_letter_id, 
                bool use_custom_color, string custom_color)
    {
        this.offering_id                            = offering_id;
        this.offering_type                          = new IDandDescr(offering_type_id);
        this.field                                  = new IDandDescr(field_id);
        this.aged_care_patient_type = new IDandDescr(aged_care_patient_type_id);
        this.num_clinic_visits_allowed_per_year     = num_clinic_visits_allowed_per_year;
        this.offering_invoice_type                  = new IDandDescr(offering_invoice_type_id);
        this.name                                   = name;
        this.short_name                             = short_name;
        this.descr                                  = descr;
        this.is_gst_exempt                          = is_gst_exempt;
        this.default_price                          = default_price;
        this.service_time_minutes                   = service_time_minutes;
        this.max_nbr_claimable                      = max_nbr_claimable;
        this.max_nbr_claimable_months               = max_nbr_claimable_months;
        this.medicare_company_code                  = medicare_company_code;
        this.dva_company_code                       = dva_company_code;
        this.tac_company_code                       = tac_company_code;
        this.medicare_charge                        = medicare_charge;
        this.dva_charge                             = dva_charge;
        this.tac_charge                             = tac_charge;
        this.popup_message                          = popup_message;
        this.reminder_letter_months_later_to_send   = reminder_letter_months_later_to_send;
        this.reminder_letter_id                     = reminder_letter_id;
        this.use_custom_color                       = use_custom_color;
        this.custom_color                           = custom_color;
    }
    public Offering(int offering_id)
    {
        this.offering_id = offering_id;
    }


    private int offering_id;
    public int OfferingID
    {
        get { return this.offering_id; }
        set { this.offering_id = value; }
    }
    private IDandDescr offering_type;
    public IDandDescr OfferingType
    {
        get { return this.offering_type; }
        set { this.offering_type = value; }
    }
    private IDandDescr field;
    public IDandDescr Field
    {
        get { return this.field; }
        set { this.field = value; }
    }
    private IDandDescr aged_care_patient_type;
    public IDandDescr AgedCarePatientType
    {
        get { return this.aged_care_patient_type; }
        set { this.aged_care_patient_type = value; }
    }
    private int num_clinic_visits_allowed_per_year;
    public int NumClinicVisitsAllowedPerYear
    {
        get { return this.num_clinic_visits_allowed_per_year; }
        set { this.num_clinic_visits_allowed_per_year = value; }
    }
    private IDandDescr offering_invoice_type;
    public IDandDescr OfferingInvoiceType
    {
        get { return this.offering_invoice_type; }
        set { this.offering_invoice_type = value; }
    }
    private string name;
    public string Name
    {
        get { return this.name; }
        set { this.name = value; }
    }
    private string short_name;
    public string ShortName
    {
        get { return this.short_name; }
        set { this.short_name = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    private bool is_gst_exempt;
    public bool IsGstExempt
    {
        get { return this.is_gst_exempt; }
        set { this.is_gst_exempt = value; }
    }
    private decimal default_price;
    public decimal DefaultPrice
    {
        get { return this.default_price; }
        set { this.default_price = value; }
    }
    private int service_time_minutes;
    public int ServiceTimeMinutes
    {
        get { return this.service_time_minutes; }
        set { this.service_time_minutes = value; }
    }
    private int max_nbr_claimable;
    public int MaxNbrClaimable
    {
        get { return this.max_nbr_claimable; }
        set { this.max_nbr_claimable= value; }
    }
    private int max_nbr_claimable_months;
    public int MaxNbrClaimableMonths
    {
        get { return this.max_nbr_claimable_months; }
        set { this.max_nbr_claimable_months = value; }
    }
    private string medicare_company_code;
    public string MedicareCompanyCode
    {
        get { return this.medicare_company_code; }
        set { this.medicare_company_code = value; }
    }
    private string dva_company_code;
    public string DvaCompanyCode
    {
        get { return this.dva_company_code; }
        set { this.dva_company_code = value; }
    }
    private string tac_company_code;
    public string TacCompanyCode
    {
        get { return this.tac_company_code; }
        set { this.tac_company_code = value; }
    }
    private decimal medicare_charge;
    public decimal MedicareCharge
    {
        get { return this.medicare_charge; }
        set { this.medicare_charge = value; }
    }
    private decimal dva_charge;
    public decimal DvaCharge
    {
        get { return this.dva_charge; }
        set { this.dva_charge = value; }
    }
    private decimal tac_charge;
    public decimal TacCharge
    {
        get { return this.tac_charge; }
        set { this.tac_charge = value; }
    }
    private string popup_message;
    public string PopupMessage
    {
        get { return this.popup_message; }
        set { this.popup_message = value; }
    }
    private int reminder_letter_months_later_to_send;
    public int ReminderLetterMonthsLaterToSend
    {
        get { return this.reminder_letter_months_later_to_send; }
        set { this.reminder_letter_months_later_to_send = value; }
    }
    private int reminder_letter_id;
    public int ReminderLetterID
    {
        get { return this.reminder_letter_id; }
        set { this.reminder_letter_id = value; }
    }
    private bool use_custom_color;
    public bool UseCustomColor
    {
        get { return this.use_custom_color; }
        set { this.use_custom_color = value; }
    }
    private string custom_color;
    public string CustomColor
    {
        get { return this.custom_color; }
        set { this.custom_color = value; }
    }
    
    public override string ToString()
    {
        return offering_id.ToString() + " " + offering_type.ID.ToString() + " " + field.ID.ToString() + " " + aged_care_patient_type.ID.ToString() + " " +
                num_clinic_visits_allowed_per_year + " " + offering_invoice_type.ID.ToString() + " " +
                name.ToString() + " " + short_name.ToString() + " " + descr.ToString() + " " + is_gst_exempt.ToString() + " " + default_price.ToString() + " " +
                service_time_minutes.ToString() + " " + max_nbr_claimable.ToString() + " " + max_nbr_claimable_months.ToString() + " " +
                medicare_company_code.ToString() + " " + dva_company_code.ToString() + " " + medicare_charge.ToString() + " " + dva_charge.ToString() + " " + tac_charge.ToString();
    }

    public static Offering[] RemoveByID(Offering[] inList, int offeringIDToRemove)
    {
        System.Collections.ArrayList newList = new System.Collections.ArrayList();
        for (int i = 0; i < inList.Length; i++)
        {
            if (inList[i].OfferingID != offeringIDToRemove)
                newList.Add(inList[i]);
        }

        return (Offering[])newList.ToArray(typeof(Offering));
    }

}