
CREATE TABLE Stock
(
 stock_id             int           not null PRIMARY KEY identity,
 organisation_id      int           not null FOREIGN KEY REFERENCES Organisation(organisation_id),
 offering_id          int           not null FOREIGN KEY REFERENCES Offering(offering_id),
 qty                  int           not null,
 warning_amt          int           not null,
);


CREATE TABLE StockUpdateHistory
(
 stock_update_history_id    int           not null PRIMARY KEY identity,
 organisation_id            int           not null FOREIGN KEY REFERENCES Organisation(organisation_id),
 offering_id                int           not null FOREIGN KEY REFERENCES Offering(offering_id),
 qty_added                  int           not null,
 is_created                 bit           not null,
 is_deleted                 bit           not null,
 added_by                   int           not null FOREIGN KEY REFERENCES Staff(staff_id),
 date_added                 datetime      not null,
);
