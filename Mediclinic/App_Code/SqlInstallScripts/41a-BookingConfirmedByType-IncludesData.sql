CREATE TABLE BookingConfirmedByType
(
 booking_confirmed_by_type_id    int  not null PRIMARY KEY identity,
 descr                           varchar(50) not null
);

SET IDENTITY_INSERT BookingConfirmedByType ON;
INSERT BookingConfirmedByType
   (booking_confirmed_by_type_id,descr)
VALUES
   (1, 'Staff'),
   (2, 'SMS'),
   (3, 'Email');
SET IDENTITY_INSERT BookingConfirmedByType OFF;