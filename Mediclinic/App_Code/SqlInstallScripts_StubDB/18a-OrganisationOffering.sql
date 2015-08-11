CREATE TABLE OrganisationOfferings
(
 organisation_offering_id           int           not null PRIMARY KEY identity,
 organisation_id                    int           not null FOREIGN KEY REFERENCES Organisation(organisation_id),
 offering_id                        int           not null FOREIGN KEY REFERENCES Offering(offering_id),
 price                              decimal(8,2)  not null,
 date_active                        datetime               DEFAULT null,
);