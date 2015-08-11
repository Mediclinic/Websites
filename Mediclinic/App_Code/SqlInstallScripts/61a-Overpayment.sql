CREATE TABLE Overpayment
(
 overpayment_id              int  not null PRIMARY KEY identity,
 receipt_id                  int  not null FOREIGN KEY REFERENCES Receipt(receipt_id),
 total                       decimal(8,2)  not null, 
 overpayment_date_added      datetime      not null DEFAULT (GETDATE()),
 staff_id                    int  not null FOREIGN KEY REFERENCES Staff(staff_id), 
);