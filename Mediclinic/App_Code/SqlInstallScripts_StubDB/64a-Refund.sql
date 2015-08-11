CREATE TABLE Refund
(
 refund_id                   int           not null PRIMARY KEY identity,
 invoice_id                  int           not null FOREIGN KEY REFERENCES Invoice(invoice_id),
 total                       decimal(8,2)  not null, 
 refund_reason_id            int           not null FOREIGN KEY REFERENCES RefundReason(refund_reason_id),
 comment                     varchar(max)  not null,
 refund_date_added           datetime      not null DEFAULT (GETDATE()),
 staff_id                    int           not null FOREIGN KEY REFERENCES Staff(staff_id), 
);