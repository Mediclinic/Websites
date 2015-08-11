CREATE TABLE InvoiceHealthcareClaimNumbers
(
 id               int          not null PRIMARY KEY identity,
 claim_number     varchar(10)  not null UNIQUE,
 last_date_used   datetime     default null,                      -- null = not yet used
 is_active        bit          not null                           -- to allocate in a particular database which claim number can be used
);