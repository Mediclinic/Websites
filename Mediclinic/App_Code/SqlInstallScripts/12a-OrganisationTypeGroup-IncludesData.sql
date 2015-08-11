CREATE TABLE OrganisationTypeGroup
(
 organisation_type_group_id    int          not null PRIMARY KEY identity,
 descr                         varchar(50) not null,
);

SET IDENTITY_INSERT OrganisationTypeGroup ON;
INSERT OrganisationTypeGroup
   (organisation_type_group_id,descr)
VALUES
   (1, 'Group Organisation - Govt'),
   (2, 'Group Organisation - Other'),
   (3, 'Corporation - Other'),
   (4, 'External'),
   (5, 'Clinic'),
   (6, 'Aged Care');
SET IDENTITY_INSERT OrganisationTypeGroup OFF;

