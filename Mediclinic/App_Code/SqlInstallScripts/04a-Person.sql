CREATE TABLE Person
(
 person_id       int     not null PRIMARY KEY identity,
 entity_id       int     not null FOREIGN KEY REFERENCES Entity(entity_id) UNIQUE,
 added_by        int     FOREIGN KEY REFERENCES Person(person_id) DEFAULT null,
 title_id        int     not null FOREIGN KEY REFERENCES Title(title_id),
 firstname       varchar(100)    not null,
 middlename      varchar(100)    not null,
 surname         varchar(100)    not null,
 nickname        varchar(100)    not null,
 gender          varchar(1)      not null,    -- M or F
 dob             datetime,
 person_date_added      datetime not null DEFAULT (GETDATE()),
 person_date_modified   datetime DEFAULT null
);