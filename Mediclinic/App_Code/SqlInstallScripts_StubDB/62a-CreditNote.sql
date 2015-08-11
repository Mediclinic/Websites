CREATE TABLE CreditNote
(
 creditnote_id              int            not null PRIMARY KEY identity,
 invoice_id                 int            not null FOREIGN KEY REFERENCES Invoice(invoice_id),
 total                      decimal(8,2)   not null, 
 reason                     varchar(250)   not null,
 credit_note_date_added     datetime       not null DEFAULT (GETDATE()),
 staff_id                   int            not null FOREIGN KEY REFERENCES Staff(staff_id), 

 reversed_by                int                     FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT NULL,
 reversed_date              datetime                DEFAULT NULL,
 pre_reversed_amount        decimal(8,2)   not null DEFAULT 0.00,
);