CREATE TABLE AddressChannel
(
 address_channel_id       int           not null PRIMARY KEY identity,
 descr                    varchar(100)  not null,
 address_channel_type_id  int           not null FOREIGN KEY REFERENCES AddressChannelType(address_channel_type_id),
 address_channel_date_added    datetime DEFAULT (GETDATE()),
 address_channel_date_modified datetime DEFAULT NULL
);