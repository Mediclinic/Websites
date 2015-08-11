CREATE TABLE InvoiceType
(
 invoice_type_id       int         not null PRIMARY KEY identity,
 descr                 varchar(50) not null
);

SET IDENTITY_INSERT InvoiceType ON;
INSERT InvoiceType
   (invoice_type_id,descr)
VALUES
   (107, 'Clinic Invoice'),  
   (108, 'Standard Invoice'),
   (363, 'Aged Care Invoice');
SET IDENTITY_INSERT InvoiceType OFF;