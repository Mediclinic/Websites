CREATE TABLE Invoice
(
 invoice_id                  int           not null PRIMARY KEY identity,
 entity_id                   int           not null FOREIGN KEY REFERENCES Entity(entity_id) UNIQUE,
 invoice_type_id             int           not null FOREIGN KEY REFERENCES InvoiceType(invoice_type_id),
 booking_id                  int                    FOREIGN KEY REFERENCES Booking(booking_id),    -- was combined as "9 FD_BookingReceipt" - split on import data

 payer_organisation_id       int           FOREIGN KEY REFERENCES Organisation(organisation_id), -- payer of invoice (medicare/dva/ac-org). null = patient_id paid   ** not in old table, this is the LE single row (or no row) in registration table. will need to get him to join on import
 payer_patient_id            int           FOREIGN KEY REFERENCES Patient(patient_id),           -- payer of invoice (medicare/dva/ac-org). null = patient_id paid   ** not in old table, this is the LE single row (or no row) in registration table. will need to get him to join on import

 non_booking_invoice_organisation_id int   FOREIGN KEY REFERENCES Organisation(organisation_id), 

 healthcare_claim_number     varchar(50)   not null DEFAULT '',
 reject_letter_id            int           FOREIGN KEY REFERENCES Letter(letter_id), -- temp stores medicare rej code until new invoice recreated. only invoices with this not empty need to be re-invoiced
 message                     varchar(200)  not null DEFAULT '',

 staff_id                    int           not null FOREIGN KEY REFERENCES Staff(staff_id), 
 site_id                     int                    FOREIGN KEY REFERENCES Site(site_id), 
 invoice_date_added          datetime      not null DEFAULT (GETDATE()),
 total                       decimal(8,2)  not null, 
 gst                         decimal(8,2)  not null,
 is_paid                     bit           not null,
 is_refund                   bit           not null,                                      -- was combined as "15 FD_OverpaidOrRefund"  - split on import data
 is_batched                  bit           not null, 

 reversed_by                int                     FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT NULL,
 reversed_date              datetime                DEFAULT NULL,

 last_date_emailed          datetime                DEFAULT NULL,
);
