CREATE TABLE Patient
(
 patient_id                   int           not null PRIMARY KEY identity,
 person_id                    int           not null FOREIGN KEY REFERENCES Person(person_id),
 patient_date_added           datetime      not null DEFAULT (GETDATE()),
 is_clinic_patient            bit           not null,                      -- "is_debtor" used in BEST to select only fac patients = true ... might be able to delete later... ??
 is_gp_patient                bit           not null,
 is_deleted                   bit           not null DEFAULT 0,
 is_deceased                  bit           not null,
 flashing_text                varchar(max)  not null,
 private_health_fund          varchar(100)  not null,
 concession_card_number       varchar(100)  not null,
 concession_card_expiry_date  datetime,
 is_diabetic                  bit           not null,
 is_member_diabetes_australia bit           not null,
 diabetic_assessment_review_date  datetime           DEFAULT null,
 ac_inv_offering_id           int                    FOREIGN KEY REFERENCES Offering(offering_id) DEFAULT null,
 ac_pat_offering_id           int                    FOREIGN KEY REFERENCES Offering(offering_id) DEFAULT null,
 login                        varchar(50)   not null DEFAULT '',
 pwd                          varchar(50)   not null DEFAULT '',
);
