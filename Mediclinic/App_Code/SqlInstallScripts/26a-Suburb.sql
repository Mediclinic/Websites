CREATE TABLE Suburb
(
 suburb_id             int            not null PRIMARY KEY identity,
 name                  varchar(50)    not null,
 postcode              varchar(8)     not null,
 state                 varchar(4)     not null,
 amended_date          datetime                 DEFAULT null,
 amended_by            int                      FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT null,
 previous              varchar(100)   not null,
);
