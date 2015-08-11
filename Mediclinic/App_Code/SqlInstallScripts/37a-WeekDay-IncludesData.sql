CREATE TABLE WeekDay
(
 weekday_id    int  not null PRIMARY KEY identity,
 descr              varchar(10) not null
);

SET IDENTITY_INSERT WeekDay ON;
INSERT WeekDay
   (weekday_id,descr)
VALUES
   (1, 'Sunday'),
   (2, 'Monday'),
   (3, 'Tuesday'),
   (4, 'Wednesday'),
   (5, 'Thursday'),
   (6, 'Friday'),
   (7, 'Saturday');
SET IDENTITY_INSERT WeekDay OFF;