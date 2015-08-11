CREATE TABLE CostCentre
(
 costcentre_id  int         not null PRIMARY KEY identity,
 descr          varchar(50) not null,
 parent_id      int                  FOREIGN KEY REFERENCES CostCentre(costcentre_id)
);