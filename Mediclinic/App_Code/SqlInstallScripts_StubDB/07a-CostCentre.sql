CREATE TABLE CostCentre
(
 costcentre_id  int         not null PRIMARY KEY identity,
 descr          varchar(50) not null,
 parent_id      int                  FOREIGN KEY REFERENCES CostCentre(costcentre_id)
);

SET IDENTITY_INSERT CostCentre ON;
INSERT CostCentre
  (costcentre_id,descr,parent_id)
VALUES
  (56,'GENERAL MANAGER',NULL),
  (57,'SALES & MARKETING',NULL),
  (58,'RESEARCH & DEVELOPMENT',NULL),
  (59,'SALES',57),
  (60,'MARKETING',57),
  (61,'PROMOTIONS',57),
  (62,'BOARD OF DIRECTORS',NULL),
  (104,'FINANCES',NULL);
SET IDENTITY_INSERT CostCentre OFF;
