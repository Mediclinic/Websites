CREATE TABLE PatientReferrer
(
 patient_referrer_id            int          not null PRIMARY KEY identity,
 patient_id                     int          not null FOREIGN KEY REFERENCES Patient(patient_id),
 register_referrer_id           int                   FOREIGN KEY REFERENCES RegisterReferrer(register_referrer_id),  --     epc referrers 
 organisation_id                int                   FOREIGN KEY REFERENCES Organisation(organisation_id),           -- non epc referrers
 patient_referrer_date_added    datetime     not null DEFAULT (GETDATE()),
 is_debtor                      bit          not null,
 is_active                      bit          not null DEFAULT 1,
);
