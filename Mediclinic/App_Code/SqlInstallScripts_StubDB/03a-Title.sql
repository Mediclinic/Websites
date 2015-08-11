CREATE TABLE Title
(
 title_id        int            not null PRIMARY KEY identity,
 descr           varchar(50)    not null,
 display_order   int            not null
);

SET IDENTITY_INSERT Title ON;
INSERT Title
  (title_id,descr,display_order)
VALUES
  (0,'None',0),
  (6,'Mr',0),
  (7,'Mrs',0),
  (23,'Dr',0),
  (24,'Prof',0),
  (25,'Rev',0),
  (26,'MS',0),
  (265,'Sr',0);
SET IDENTITY_INSERT Title OFF;
