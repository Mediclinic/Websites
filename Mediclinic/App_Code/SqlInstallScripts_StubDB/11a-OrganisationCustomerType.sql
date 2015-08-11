CREATE TABLE OrganisationCustomerType
(
 organisation_customer_type_id  int          not null PRIMARY KEY identity,
 descr                          varchar(50) not null,
);

SET IDENTITY_INSERT OrganisationCustomerType ON;
INSERT OrganisationCustomerType
  (organisation_customer_type_id,descr)
VALUES
  (0,  'None'),
  (149,'Corporation'),
  (45,'Nursing home'),
  (46,'Hostel'),
  (47,'Retirement village'),
  (137,'Facility');
SET IDENTITY_INSERT OrganisationCustomerType OFF;
