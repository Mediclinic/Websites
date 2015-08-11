CREATE TABLE BookingChangeHistoryReason
(
 booking_change_history_reason_id    int         not null PRIMARY KEY identity,
 descr                               varchar(50) not null,
 display_order                       int         not null
);