
CREATE TABLE OrganisationType
(
 organisation_type_id        int          not null PRIMARY KEY identity,
 descr                       varchar(50)  not null,
 organisation_type_group_id  int          not null FOREIGN KEY REFERENCES OrganisationTypeGroup(organisation_type_group_id),
 display_order               int          not null,
);

SET IDENTITY_INSERT OrganisationType ON;
INSERT OrganisationType
   (organisation_type_id,descr,organisation_type_group_id,display_order)
VALUES
   (1, 'Medicare',1,11),
   (2, 'Dept Veterans Affairs',1,12),
   (98,'Health Related Organisation',4,13),
   (99,'Medical Clinic',4,14),
   (100,'Laboratory',4,15),
   (147,'Sole Trader',4,16),
   (148,'Small or Medium Sized Enterprise',4,17),
   (152,'Advertising Agency',4,18),
   (191,'Medical Practice',4,19),
   (218,'Clinic',5,1),
   (139,'Aged Care Facility',6,2),
   (367,'Aged Care Wing',6,3),
   (372,'Aged Care Unit',6,4),

   (141,'Group Organisation',2,8),
   (149,'Corporation',3,7);

SET IDENTITY_INSERT OrganisationType OFF;
