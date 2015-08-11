CREATE TABLE Offering
(
 offering_id                        int           not null PRIMARY KEY identity,
 field_id                           int           not null FOREIGN KEY REFERENCES Field(field_id),
 offering_type_id                   int           not null FOREIGN KEY REFERENCES OfferingType(offering_type_id),
 aged_care_patient_type_id  int   not null FOREIGN KEY REFERENCES AgedCarePatientType(aged_care_patient_type_id),
 num_clinic_visits_allowed_per_year int           not null,    -- 0 for null  (= warning level in old system)
 offering_invoice_type_id           int           not null FOREIGN KEY REFERENCES OfferingInvoiceType( offering_invoice_type_id),

 name                      varchar(100)  not null,
 short_name                varchar(100)  not null,
 descr                     varchar(500)  not null,

 is_gst_exempt             bit           not null,
 default_price             decimal(8,2)  not null,

 service_time_minutes      int           not null,
 is_deleted                bit           not null DEFAULT 0,   -- set if offering no longer relevant

 max_nbr_claimable         int           not null,    -- NEW, not in BEST.  0 = no max
 max_nbr_claimable_months  int           not null,    -- NEW, not in BEST.  how many past months before a treatment the max nbr is for

 medicare_company_code     varchar(50)   not null,    -- national health id for claims. on medicare invoice
 dva_company_code          varchar(50)   not null,    -- dva item that dva see on invoice
 tac_company_code          varchar(50)   not null,

 medicare_charge           decimal(5,2)  not null,
 dva_charge                decimal(5,2)  not null,
 tac_charge                decimal(5,2)  not null,

 popup_message             varchar(max)  not null,

 reminder_letter_months_later_to_send  int  not null, -- zero means disabled, but if enabled (more than zero) then reminder_letter_id must be set and not null
 reminder_letter_id                    int            FOREIGN KEY REFERENCES Letter(letter_id),

 use_custom_color          bit           not null default 0,
 custom_color              varchar(10)   not null default 'FFFFFF',

);