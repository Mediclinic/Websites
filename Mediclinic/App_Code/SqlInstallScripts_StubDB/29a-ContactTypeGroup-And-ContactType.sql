CREATE TABLE ContactTypeGroup
(
 contact_type_group_id    int      not null PRIMARY KEY identity,
 descr                   varchar(50)    not null
);

SET IDENTITY_INSERT ContactTypeGroup ON;
INSERT ContactTypeGroup
  (contact_type_group_id,descr)
VALUES
  (1,'Mailing'),
  (2,'Telecoms'),
  (3,'Facility'),
  (4,'Internet');
 SET IDENTITY_INSERT ContactTypeGroup OFF;

 -------------------------------------------------

 CREATE TABLE ContactType
(
 contact_type_id         int          not null PRIMARY KEY identity,
 contact_type_group_id   int          not null FOREIGN KEY REFERENCES ContactTypeGroup(contact_type_group_id),
 display_order           int          not null,
 descr                   varchar(50)  not null,
);

SET IDENTITY_INSERT ContactType ON;
INSERT ContactType
  (contact_type_id,contact_type_group_id, display_order, descr)
VALUES

  (35, 1,1,'Home address'),
  (36, 1,2,'Business address'),
  (37, 1,3,'PO Box'),
  (38, 1,4,'Private Box'),
  (39, 1,5,'Document exchange'),
  (262,1,6,'GPO Box'),

  (29,2,16,'Fax home'),
  (30,2,10,'Mobile'),
  (31,2,17,'Office Fax'),
  (32,2,12,'Pager'),
  (33,2,11,'Home Phone'),
  (34,2,13,'Office Phone'),
  (42,2,14,'Toll free phone'),
  (43,2,15,'Toll free fax'),

  (166,3,20,'Bedroom'),

  (27, 4,18,'E-mail'),
  (28, 4,18,'WWW');

 SET IDENTITY_INSERT ContactType OFF;
