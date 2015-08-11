CREATE TABLE Booking
(
 booking_id           int           not null PRIMARY KEY identity,
 entity_id            int           not null FOREIGN KEY REFERENCES Entity(entity_id) UNIQUE,
 date_start           datetime      not null,
 date_end             datetime,                                                    -- can be null for recurring bookings

 organisation_id      int                    FOREIGN KEY REFERENCES Organisation(organisation_id),     -- can be null for type 341 prov unavail days
 provider             int                    FOREIGN KEY REFERENCES Staff(staff_id),                   -- can be null for type 340 org unavail days
 patient_id           int                    FOREIGN KEY REFERENCES Patient(patient_id) DEFAULT null,  -- null for aged care, or patient id for clinic bookind
 offering_id          int                    FOREIGN KEY REFERENCES Offering(offering_id),
 booking_type_id      int           not null FOREIGN KEY REFERENCES BookingType(booking_type_id),
 booking_status_id    int                    FOREIGN KEY REFERENCES BookingStatus(booking_status_id),                       -- used for 34 bookings
 booking_unavailability_reason_id int        FOREIGN KEY REFERENCES BookingUnavailabilityReason(booking_unavailability_reason_id),  -- used for unavailable 340/341 bookings

 added_by                      int           FOREIGN KEY REFERENCES Staff(staff_id),
 date_created                  datetime      DEFAULT (GETDATE()),
 booking_confirmed_by_type_id  int           FOREIGN KEY REFERENCES BookingConfirmedByType(booking_confirmed_by_type_id)  DEFAULT null, 
 confirmed_by                  int           FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,              -- if unset, can be unconfirmed OR "confirmed by auto sms/email reminder"
 date_confirmed                datetime      DEFAULT null,                                                      -- so go by this: is confirmed if this is set, else unset
 deleted_by                    int           FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,
 date_deleted                  datetime      DEFAULT null,
 cancelled_by                  int           FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,
 date_cancelled                datetime      DEFAULT null,

 is_patient_missed_appt        bit  not null,
 is_provider_missed_appt       bit  not null,
 is_emergency                  bit  not null,

 need_to_generate_first_letter bit  not null,
 need_to_generate_last_letter  bit  not null,
 has_generated_system_letters  bit  not null,  -- need for if need to generate treatment letters also

 arrival_time                  datetime      default null,
 sterilisation_code            varchar(200)  not null,
 informed_consent_added_by     int           FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,
 informed_consent_date         datetime      DEFAULT null,

 is_recurring         bit           not null,                                     -- date_start/date_end is when reocurring booking start and ends
 recurring_weekday_id int           FOREIGN KEY REFERENCES WeekDay(weekday_id),   -- in Booking class, use object DayOfWeek
 recurring_start_time time,
 recurring_end_time   time,                                                       -- in Booking class, use Timespan ... inserting : '00:00:00'
);


CREATE TABLE BookingPatient
(
 booking_patient_id          int          NOT NULL PRIMARY KEY identity,
 booking_id                  int          FOREIGN KEY REFERENCES Booking(booking_id) NOT NULL,
 patient_id                  int          FOREIGN KEY REFERENCES Patient(patient_id) NOT NULL,

 added_by                    int          FOREIGN KEY REFERENCES Staff(staff_id),
 added_date                  datetime,

 is_deleted                  bit          NOT NULL,
 deleted_by                  int          FOREIGN KEY REFERENCES Staff(staff_id),
 deleted_date                datetime,

 -- used for generating system letters for aged care - fields identical to in the Booking table for the same purpose
 need_to_generate_first_letter bit  not null,
 need_to_generate_last_letter  bit  not null,
 has_generated_system_letters  bit  not null,  -- need for if need to generate treatment letters also
);


CREATE TABLE BookingPatientOffering
(
 booking_patient_offering_id int          NOT NULL PRIMARY KEY identity,
 booking_patient_id          int          FOREIGN KEY REFERENCES BookingPatient(booking_patient_id) NOT NULL,

 offering_id                 int          FOREIGN KEY REFERENCES Offering(offering_id) NOT NULL,
 quantity                    int          NOT NULL,

 added_by                    int          FOREIGN KEY REFERENCES Staff(staff_id),
 added_date                  datetime,

 is_deleted                  bit          NOT NULL,
 deleted_by                  int          FOREIGN KEY REFERENCES Staff(staff_id),
 deleted_date                datetime,
 area_treated                varchar(500) not null,
);

