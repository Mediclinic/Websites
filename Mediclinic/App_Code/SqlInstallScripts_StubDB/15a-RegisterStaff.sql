CREATE TABLE RegisterStaff
(
 register_staff_id          int          not null PRIMARY KEY identity,
 register_staff_date_added  datetime     not null DEFAULT (GETDATE()),
 organisation_id            int          FOREIGN KEY REFERENCES Organisation(organisation_id) DEFAULT null,
 staff_id                   int          FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT null,
 provider_number            varchar(50) not null,
 main_provider_for_clinic   bit not null,

 excl_sun                   bit not null DEFAULT 0,
 excl_mon                   bit not null DEFAULT 0,
 excl_tue                   bit not null DEFAULT 0,
 excl_wed                   bit not null DEFAULT 0,
 excl_thu                   bit not null DEFAULT 0,
 excl_fri                   bit not null DEFAULT 0,
 excl_sat                   bit not null DEFAULT 0,

 is_deleted                 bit not null DEFAULT 0,
);