CREATE TABLE Referrer
(
 referrer_id               int           not null PRIMARY KEY identity,
 person_id                 int           not null FOREIGN KEY REFERENCES Person(person_id),
 referrer_date_added       datetime      not null DEFAULT (GETDATE()),
 is_deleted                bit           not null DEFAULT 0,
);
