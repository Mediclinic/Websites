CREATE TABLE BookingType
(
 booking_type_id    int  not null PRIMARY KEY identity,
 descr              varchar(50) not null
);

SET IDENTITY_INSERT BookingType ON;
INSERT BookingType
   (booking_type_id,descr)
VALUES
   (34, 'Patient or Aged Care booking'),
   (35, 'Staff Visit'),
   (340,'Facility Unavailable'),              -- org unavailable - [whole day/days] - bookings can be made for clinics but after a warning message
   (341,'Provider-Facility Unavailability'),  -- provider unavailable but only at a clinic [specific hr-to-hr]
   (342,'Provider Unavailability');           -- provider unavailable [specific hr-to-hr]
SET IDENTITY_INSERT BookingType OFF;