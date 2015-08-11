CREATE TABLE BookingChangeHistory
(
 booking_change_history_id        int           not null PRIMARY KEY identity,
 booking_id                       int           FOREIGN KEY REFERENCES Booking(booking_id)  not null,
 moved_by                         int           FOREIGN KEY REFERENCES Staff(staff_id)  not null,
 date_moved                       datetime      not null DEFAULT (GETDATE()),
 booking_change_history_reason_id int           FOREIGN KEY REFERENCES BookingChangeHistoryReason(booking_change_history_reason_id)  not null,
 previous_datetime                datetime      not null  -- can get original date by getting for booking_id and finding the lowest booking_change_history_id
);