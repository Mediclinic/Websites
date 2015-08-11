CREATE TABLE BookingUnavailabilityReason
(
 booking_unavailability_reason_id      int         not null PRIMARY KEY identity,
 booking_unavailability_reason_type_id int         not null FOREIGN KEY REFERENCES BookingUnavailabilityReasonType(booking_unavailability_reason_type_id),
 descr                                 varchar(50) not null
);

SET IDENTITY_INSERT BookingUnavailabilityReason ON;
INSERT BookingUnavailabilityReason
  (booking_unavailability_reason_id,booking_unavailability_reason_type_id,descr)
VALUES
  (174,340,'CHRISTMAS DAY'),
  (175,340,'BOXING DAY'),
  (176,340,'NEW YEARS DAY'),
  (177,340,'*PUBLIC HOLIDAY'),
  (185,340,'GOOD FRIDAY'),
  (186,340,'EASTER MONDAY'),
  (301,340,'AUSTRALIA DAY'),
  (302,340,'LABOUR DAY'),
  (303,340,'EASTER SATURDAY'),
  (304,340,'ANZAC DAY'),
  (305,340,'QUEEN''S BIRTHDAY'),
  (306,340,'MELBOURNE CUP DAY');
SET IDENTITY_INSERT BookingUnavailabilityReason OFF;

SET IDENTITY_INSERT BookingUnavailabilityReason ON;
INSERT BookingUnavailabilityReason
  (booking_unavailability_reason_id,booking_unavailability_reason_type_id,descr)
VALUES
  (168,341,'Annual leave'),
  (169,341,'Sick leave'),
  (170,341,'Compassionate leave'),
  (171,341,'Maternity leave'),
  (172,341,'Long service leave'),
  (173,341,'Paternity leave'),
  (213,341,'Time off'),
  (289,341,'Additional time required'),
  (293,341,'Prov needs to leave'),
  (294,341,'Patient Report'),
  (299,341,'Reminder'),
  (307,341,'Not Available');
SET IDENTITY_INSERT BookingUnavailabilityReason OFF;
