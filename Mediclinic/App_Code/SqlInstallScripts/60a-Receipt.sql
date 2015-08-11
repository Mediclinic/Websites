CREATE TABLE Receipt
(
 receipt_id                 int           not null PRIMARY KEY identity,
 receipt_payment_type_id    int           not null FOREIGN KEY REFERENCES ReceiptPaymentType(receipt_payment_type_id),
 invoice_id                 int           not null FOREIGN KEY REFERENCES Invoice(invoice_id),

 total                      decimal(8,2)  not null, 
 amount_reconciled          decimal(8,2)  not null,                       -- zero  when adding receipt -- update on reconciliation
 is_failed_to_clear         bit           not null,                       -- false when adding receipt -- update on reconciliation
 is_overpaid                bit           not null,                       -- get all receipts for this invoice PLUS the amount of this one
	                                                                      --   if over than invoice amount, then (ONLY THIS) receipt set to overpaid => and need to create an overpayment record!
 receipt_date_added         datetime      not null DEFAULT (GETDATE()),
 reconciliation_date        datetime               DEFAULT NULL,          -- null  when adding receipt -- auto set current date on reconciliation

 staff_id                   int           not null FOREIGN KEY REFERENCES Staff(staff_id), 

 reversed_by                int                    FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT NULL,
 reversed_date              datetime               DEFAULT NULL,
 pre_reversed_amount        decimal(8,2)  not null DEFAULT 0.00,
);