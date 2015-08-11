CREATE TABLE Screen
(
 screen_id          int  not null PRIMARY KEY identity,
 descr              varchar(50) not null
);

SET IDENTITY_INSERT Screen ON;
INSERT Screen
   (screen_id,descr)
VALUES
   (1,'SiteList'),
   (2,'AddEditSite'),
   (3,'StaffList'),
   (4,'AddEditStaff'),
   (5,'PatientList'),
   (6,'AddEditPatient'),
   (7,'ReferrerList'),
   (8,'AddEditReferrer'),
   (9,'OrganisationList - All External'),
   (10,'AddEditOrganisatoin - All External'),
   (11,'OrganisationList - Internal Clinics'),
   (12,'AddEditOrganisatoin - Internal Clinics'),
   (13,'OrganisationList - Internal Aged Care'),
   (14,'AddEditOrganisatoin - Internal Aged Care'),
   (15,'Bookings');
SET IDENTITY_INSERT Screen OFF;