CREATE TABLE EmailHistory
(
 email_history_id       int        not null PRIMARY KEY identity,
 sms_and_email_type_id  int        not null FOREIGN KEY REFERENCES SMSAndEmailType(sms_and_email_type_id),
 patient_id             int                 FOREIGN KEY REFERENCES Patient(patient_id),
 booking_id             int                 FOREIGN KEY REFERENCES Booking(booking_id),
 email                  varchar(80)  not null,
 message                varchar(max) not null,
 datetime_sent          datetime     not null,
);