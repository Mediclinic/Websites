CREATE TABLE StaffPosition
(
 staff_position_id    int  not null PRIMARY KEY identity,
 descr varchar(50) not null
);

SET IDENTITY_INSERT StaffPosition ON;
INSERT StaffPosition
  (staff_position_id, descr)
VALUES
(1,'Accountant'),
(2,'Business Owner'),
(3,'C E O'),
(4,'Clerk'),
(5,'Clinical Psychologyist '),
(6,'GM'),
(7,'Myotherapist / Massage'),
(8,'Operation Manager'),
(9,'Patient Administrator'),
(10,'Physiotherapist'),
(11,'Pilates Instructor'),
(12,'Podiatrist'),
(13,'Standard Carepro'),
(14,'Unknown');
SET IDENTITY_INSERT StaffPosition OFF;
