
CREATE TABLE PatientHistory
(
 patient_history_id               int           not null PRIMARY KEY identity,
 patient_id                       int           not null FOREIGN KEY REFERENCES Patient(patient_id), 

 is_clinic_patient                bit           not null,
 is_gp_patient                    bit           not null,
 is_deleted                       bit           not null DEFAULT 0,
 is_deceased                      bit           not null,
 flashing_text                    varchar(max)  not null,
 flashing_text_added_by           int                    FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT null,
 flashing_text_last_modified_date datetime               DEFAULT null,
 private_health_fund              varchar(100)  not null,
 concession_card_number           varchar(100)  not null,
 concession_card_expiry_date      datetime               DEFAULT null,
 is_diabetic                      bit           not null,
 is_member_diabetes_australia     bit           not null,
 diabetic_assessment_review_date  datetime               DEFAULT null,
 ac_inv_offering_id               int                    FOREIGN KEY REFERENCES Offering(offering_id) DEFAULT null,
 ac_pat_offering_id               int                    FOREIGN KEY REFERENCES Offering(offering_id) DEFAULT null,
 login                            varchar(50)   not null DEFAULT '',
 pwd                              varchar(50)   not null DEFAULT '',

 title_id              int           not null FOREIGN KEY REFERENCES Title(title_id),
 firstname             varchar(100)  not null,
 middlename            varchar(100)  not null,
 surname               varchar(100)  not null,
 nickname              varchar(100)  not null,
 gender                varchar(1)    not null,
 dob                   datetime,

 modified_from_this_by int           not null FOREIGN KEY REFERENCES Staff(staff_id),
 date_added            datetime      not null,

);
