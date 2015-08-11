CREATE TABLE SMSAndEmailType
(
 sms_and_email_type_id  int          not null PRIMARY KEY identity,
 descr                  varchar(50)  not null,
);


SET IDENTITY_INSERT SMSAndEmailType ON;
INSERT SMSAndEmailType
   (sms_and_email_type_id,descr)
VALUES
   (1,  'Appointment Reminder'),
   (2,  'Birthday');
SET IDENTITY_INSERT SMSAndEmailType OFF;