CREATE TABLE HealthCard
(
 health_card_id                       int          not null PRIMARY KEY identity,
 patient_id                           int          not null FOREIGN KEY REFERENCES Patient(patient_id),
 organisation_id                      int          not null FOREIGN KEY REFERENCES Organisation(organisation_id),  -- Medicare or DVA
 card_name                            varchar(50)  not null, 
 card_nbr                             varchar(50)  not null, 
 card_family_member_nbr               varchar(4)   not null, 
 expiry_date                          datetime,
 date_referral_signed                 datetime default null,
 date_referral_received_in_office     datetime default null,
 is_active                            bit          not null,                                -- if they hvae 2 cards .. only one can be active at a time
 added_or_last_modified_by            int          FOREIGN KEY REFERENCES Staff(staff_id),  -- nullable since added after BEST data
 added_or_last_modified_date          datetime     DEFAULT (GETDATE()),                     -- nullable since added after BEST data
 area_treated                         varchar(500) not null, 
);