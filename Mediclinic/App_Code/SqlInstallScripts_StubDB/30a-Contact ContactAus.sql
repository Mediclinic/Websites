/*  -- dont even add this to new db's....
CREATE TABLE Contact
(
 contact_id            int           not null PRIMARY KEY identity,
 entity_id             int           not null FOREIGN KEY REFERENCES Entity(entity_id),

 contact_type_id       int           not null FOREIGN KEY REFERENCES ContactType(contact_type_id),
 
 free_text             varchar(100)  not null,                            -- such as (sisters phone nbr or address)
 addr_line1            varchar(100)  not null,                            -- contains phone number or website for those types
 addr_line2            varchar(100)  not null,
 address_channel_id    int                    FOREIGN KEY REFERENCES AddressChannel(address_channel_id),
 suburb_id             int                    FOREIGN KEY REFERENCES Suburb(suburb_id),
 country_id            int                    FOREIGN KEY REFERENCES Country(country_id),

 site_id               int                    FOREIGN KEY REFERENCES Site(site_id),

 is_billing            bit           not null,
 is_non_billing        bit           not null,
 contact_date_added    datetime      not null DEFAULT (GETDATE()),
 contact_date_modified datetime                    DEFAULT NULL,
 contact_date_deleted  datetime                    DEFAULT NULL,
);
*/

CREATE TABLE ContactAus
(
 contact_id               int           not null PRIMARY KEY identity,
 entity_id                int           not null FOREIGN KEY REFERENCES Entity(entity_id),

 contact_type_id          int           not null FOREIGN KEY REFERENCES ContactType(contact_type_id),
 
 free_text                varchar(100)  not null,                            -- such as (sisters phone nbr)
 addr_line1               varchar(100)  not null,                            -- contains phone number or website for those types
 addr_line2               varchar(100)  not null,
 street_name              varchar(100)  not null, 
 address_channel_type_id  int                    FOREIGN KEY REFERENCES AddressChannelType(address_channel_type_id),
 suburb_id                int                    FOREIGN KEY REFERENCES Suburb(suburb_id),
 country_id               int                    FOREIGN KEY REFERENCES Country(country_id),

 site_id                  int                    FOREIGN KEY REFERENCES Site(site_id),

 is_billing               bit           not null,
 is_non_billing           bit           not null,
 contact_date_added       datetime      not null DEFAULT (GETDATE()),
 contact_date_modified    datetime                    DEFAULT NULL,
 contact_date_deleted     datetime                    DEFAULT NULL,
);