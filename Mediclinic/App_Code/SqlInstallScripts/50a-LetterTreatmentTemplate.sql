CREATE TABLE LetterTreatmentTemplate
(
 letter_treatment_template_id       int  not null PRIMARY KEY identity,
 field_id                           int  not null FOREIGN KEY REFERENCES Field(field_id),

 first_letter_id                    int  not null FOREIGN KEY REFERENCES Letter(letter_id),
 last_letter_id                     int  not null FOREIGN KEY REFERENCES Letter(letter_id),
 last_letter_when_replacing_epc_id  int  not null FOREIGN KEY REFERENCES Letter(letter_id),
 treatment_notes_letter_id          int  not null FOREIGN KEY REFERENCES Letter(letter_id),

 site_id                            int  not null FOREIGN KEY REFERENCES Site(site_id),
);