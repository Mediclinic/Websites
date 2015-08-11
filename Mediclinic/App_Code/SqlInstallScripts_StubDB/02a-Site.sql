CREATE TABLE Site
(
 site_id           int     not null PRIMARY KEY identity,
 entity_id         int     not null FOREIGN KEY REFERENCES Entity(entity_id) UNIQUE,
 name              varchar(100) not null,
 site_type_id  int  not null FOREIGN KEY REFERENCES SiteType(site_type_id),
 abn               varchar(50) not null,
 acn               varchar(50) not null,
 tfn               varchar(50) not null,
 asic              varchar(50) not null,
 is_provider       bit          not null,   -- for aged care only
 bank_bpay         varchar(50) not null,	
 bank_bsb          varchar(50) not null,	
 bank_account      varchar(50) not null,
 bank_direct_debit_userid    varchar(50)  not null,  -- number bank gives to do direct debits
 bank_username               varchar(50)  not null,  -- name to be used on Debit order transfer
 oustanding_balance_warning  decimal(10,2) not null,  -- Warning no treat if balance over this (used in aged care)
 print_epc         bit          not null,  -- Enhanced Primary Care
 excl_sun          bit          not null,
 excl_mon          bit          not null,
 excl_tue          bit          not null,
 excl_wed          bit          not null,
 excl_thu          bit          not null,
 excl_fri          bit          not null,
 excl_sat          bit          not null,
 day_start_time    time         not null,
 lunch_start_time  time         not null,
 lunch_end_time    time         not null,
 day_end_time      time         not null,
 fiscal_yr_end     datetime     not null,
 num_booking_months_to_get int not null DEFAULT 9 -- how many bookings to get when starting bookings
);
