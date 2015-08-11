CREATE TABLE SMSHistory
(
 sms_history_id         int          not null PRIMARY KEY identity,
 sms_and_email_type_id  int          not null FOREIGN KEY REFERENCES SMSAndEmailType(sms_and_email_type_id),
 patient_id             int                   FOREIGN KEY REFERENCES Patient(patient_id),
 booking_id             int                   FOREIGN KEY REFERENCES Booking(booking_id),
 phone_number           varchar(20)  not null,
 message                varchar(max) not null,
 cost                   decimal(8,2) not null, 
 datetime_sent          datetime     not null,
 smstech_message_id     varchar(50)  not null,
 smstech_status         varchar(50)  not null,
 smstech_datetime       datetime              DEFAULT NULL,
);