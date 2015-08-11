CREATE TABLE RegisterReferrer
(
 register_referrer_id                               int           not null PRIMARY KEY identity,
 organisation_id                                    int                    FOREIGN KEY REFERENCES Organisation(organisation_id),
 referrer_id                                        int           not null FOREIGN KEY REFERENCES Referrer(referrer_id),
 provider_number                                    varchar(50)   not null,
 report_every_visit_to_referrer                     bit           not null,
 batch_send_all_patients_treatment_notes            bit           not null,
 date_last_batch_send_all_patients_treatment_notes  datetime          DEFAULT null,
 register_referrer_date_added                       datetime      not null DEFAULT (GETDATE()),
 is_deleted                                         bit           not null DEFAULT 0,
);
