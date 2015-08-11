CREATE TABLE BookingUnavailabilityReasonType
(
 booking_unavailability_reason_type_id    int         not null PRIMARY KEY identity,
 descr                                    varchar(50) not null
);

SET IDENTITY_INSERT BookingUnavailabilityReasonType ON;
INSERT BookingUnavailabilityReasonType
   (booking_unavailability_reason_type_id,descr)
VALUES
   (340, 'Organisation Unavailability'),
   (341, 'Provider Unavailability');
SET IDENTITY_INSERT BookingUnavailabilityReasonType  OFF;