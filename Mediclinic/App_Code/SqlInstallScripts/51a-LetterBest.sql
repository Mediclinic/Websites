CREATE TABLE LetterBest
(
 letter_id           int           not null PRIMARY KEY identity,
 letter_type_id      int           not null FOREIGN KEY REFERENCES LetterType(letter_type_id),
 code                varchar(10)   not null,
 reject_message      varchar(200)  not null,
 docname             varchar(100)  not null,
 is_send_to_medico   bit           not null,
 is_allowed_reclaim  bit           not null,
 is_manual_override  bit           not null,
 num_copies_to_print int           not null,
 is_deleted          bit           not null,
);
