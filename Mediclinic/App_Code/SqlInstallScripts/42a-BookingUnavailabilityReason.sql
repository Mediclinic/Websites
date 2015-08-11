CREATE TABLE BookingUnavailabilityReason
(
 booking_unavailability_reason_id      int         not null PRIMARY KEY identity,
 booking_unavailability_reason_type_id int         not null FOREIGN KEY REFERENCES BookingUnavailabilityReasonType(booking_unavailability_reason_type_id),
 descr                                 varchar(50) not null
);