CREATE TABLE InvoiceLine
(
 invoice_line_id  int  not null PRIMARY KEY identity,
 invoice_id       int  not null FOREIGN KEY REFERENCES Invoice(invoice_id),
 patient_id       int           FOREIGN KEY REFERENCES Patient(patient_id),   -- nullable
 offering_id      int           FOREIGN KEY REFERENCES Offering(offering_id),
 quantity         decimal(8,2)  not null,
 price            decimal(8,2)  not null, 
 tax              decimal(8,2)  not null, 
 area_treated     varchar(500)  not null,
 offering_order_id int           FOREIGN KEY REFERENCES OfferingOrder(offering_order_id),   -- nullable
);