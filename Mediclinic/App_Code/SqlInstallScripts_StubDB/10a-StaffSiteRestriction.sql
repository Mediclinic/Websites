CREATE TABLE StaffSiteRestriction
(
 staff_site_restriction_id  int  not null PRIMARY KEY identity,
 staff_id                   int  not null FOREIGN KEY REFERENCES Staff(staff_id),
 site_id                    int  not null FOREIGN KEY REFERENCES Site(site_id),
 CONSTRAINT uc_unique_staff_site UNIQUE (staff_id,site_id)
);