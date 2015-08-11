CREATE TABLE AddressChannelType
(
 address_channel_type_id    int      not null PRIMARY KEY identity,
 descr                      varchar(50)    not null
);


SET IDENTITY_INSERT AddressChannelType ON;
INSERT AddressChannelType
  (address_channel_type_id,descr)
VALUES
  (1,'Street'),
  (2,'Road'),
  (12,'Avenue'),
  (13,'Place'),
  (14,'Highway'),
  (16,'Crescent'),
  (17,'Drive'),
  (18,'Blvd'),
  (19,'Cutting'),
  (20,'Parade'),
  (21,'Close'),
  (22,'Lane'),
  (41,'<None>'),
  (214,'Court'),
  (236,'Circuit'),
  (237,'Grove'),
  (238,'Terrace'),
  (239,'Way'),
  (240,'Square'),
  (241,'Walk'),
  (242,'Rise'),
  (243,'Mews'),
  (244,'Chase'),
  (245,'Hill'),
  (246,'Ridge'),
  (247,'Wynd'),
  (258,'Concourse'),
  (259,'Promonade'),
  (263,'Esplanade'),
  (271,'Strip'),
  (309,'Hub'),
  (310,'Circle'),
  (315,'Glade'),
  (318,'pass'),
  (319,'View');
SET IDENTITY_INSERT AddressChannelType OFF;
