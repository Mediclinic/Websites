CREATE TABLE BookingStatus
(
 booking_status_id    int  not null PRIMARY KEY identity,
 descr              varchar(50) not null
);

SET IDENTITY_INSERT BookingStatus ON;
INSERT BookingStatus
   (booking_status_id,descr)
VALUES
   (-1, 'Deleted'),
   (0,  'InComplete'),
   (187,'Completed'),
   (188,'Cancelled'),
   (189,'Deceased');
SET IDENTITY_INSERT BookingStatus OFF;