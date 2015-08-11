CREATE TABLE Title
(
 title_id        int            not null PRIMARY KEY identity,
 descr           varchar(50)    not null,
 display_order   int            not null
);