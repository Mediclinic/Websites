CREATE TABLE RegisterPatient
(
 register_patient_id         int           not null PRIMARY KEY identity,
 organisation_id             int           not null FOREIGN KEY REFERENCES Organisation(organisation_id),
 patient_id                  int           not null FOREIGN KEY REFERENCES Patient(patient_id),
 register_patient_date_added datetime      not null DEFAULT (GETDATE()),
 is_deleted                  bit not null DEFAULT 0, 
);
