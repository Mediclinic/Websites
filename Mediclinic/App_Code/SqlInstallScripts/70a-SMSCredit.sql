CREATE TABLE SMSCredit
(
 sms_credit_id        int          not null PRIMARY KEY identity,
 amount               decimal(8,2) not null, 
 datetime_added       datetime     not null,
);