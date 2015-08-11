CREATE TABLE ReceiptPaymentType
(
 receipt_payment_type_id  int         not null PRIMARY KEY identity,
 descr                    varchar(50) not null
);

SET IDENTITY_INSERT ReceiptPaymentType ON;
INSERT ReceiptPaymentType 
   (receipt_payment_type_id,descr)
VALUES
   (129, 'Cash'),
   (130, 'HICAPS'),      -- was 'EFT/HICAPS'
   (133, 'CC / EFTPOS'), -- was 'Credit card'
   (136, 'Cheque'),
   (229, 'Money Order'),
   (362, 'Direct Credit');
SET IDENTITY_INSERT ReceiptPaymentType OFF;