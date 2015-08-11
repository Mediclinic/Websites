-------------------------------------------

-- defines what sort of patient the offering is designed specifically for (within aged care)
CREATE TABLE AgedCarePatientType
(
 aged_care_patient_type_id    int  not null PRIMARY KEY identity,
 descr varchar(50) not null
);

INSERT AgedCarePatientType
   (descr)
VALUES
   ('--Not Aged Care--'),
   ('Low Care'),
   ('High Care'),
   ('Low Care Funded'),
   ('High Care Unfunded'),
   ('Low Care Emergency'),
   ('High care Emergency'),
   ('Medicare'),
   ('DVA'),
   ('TAC');
------------------------------------------

-- to control which type of invoice an offering could appear on
CREATE TABLE OfferingInvoiceType
(
 offering_invoice_type_id    int  not null PRIMARY KEY identity,
 descr varchar(50) not null
);

SET IDENTITY_INSERT OfferingInvoiceType ON;
INSERT OfferingInvoiceType
   (offering_invoice_type_id,descr)
VALUES
   (0,'Error'),
   (1,'Clinic Booking'),  -- clinic
   (2,'Standard'),
   (3,'Std&booking'),
   (4,'A.C.Fac. Booking'),
   (5,'Studio');  --  
SET IDENTITY_INSERT OfferingInvoiceType OFF;
-------------------------------------------