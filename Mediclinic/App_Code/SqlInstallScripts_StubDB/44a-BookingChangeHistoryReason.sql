CREATE TABLE BookingChangeHistoryReason
(
 booking_change_history_reason_id    int         not null PRIMARY KEY identity,
 descr                               varchar(50) not null,
 display_order                       int         not null
);

SET IDENTITY_INSERT BookingChangeHistoryReason ON;
INSERT BookingChangeHistoryReason
  (booking_change_history_reason_id,descr,display_order)
VALUES
  (220,'Sick day',0),
  (221,'Provider on holiday',0),
  (260,'Public holiday',0),
  (261,'Facility requested move',0),
  (266,'Prov Request Move',0),
  (267,'Facility Medical Problem',0),
  (268,'Personal reasons',0),
  (270,'Change of Provider for this session',0),
  (276,'Admininstration reschedule needs',0),
  (279,'Missed Appointment',0),
  (288,'Additional time required',0),
  (300,'Patient requested reschedule',0),
  (313,'Patient ill. Will call back',0);
SET IDENTITY_INSERT BookingChangeHistoryReason OFF;
