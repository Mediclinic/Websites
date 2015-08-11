CREATE TABLE StaffOfferings
(
 staff_offering_id                  int           not null PRIMARY KEY identity,
 staff_id                           int           not null FOREIGN KEY REFERENCES Staff(staff_id),
 offering_id                        int           not null FOREIGN KEY REFERENCES Offering(offering_id),
 is_commission                      bit           not null,
 commission_percent                 decimal(5,2)  not null,
 is_fixed_rate                      bit           not null,
 fixed_rate                         decimal(8,2)  not null,
 date_active                        datetime               DEFAULT null,  -- no start date = inactive -- get most recent active one (top 1 where not null order by start date desc)
);