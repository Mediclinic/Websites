CREATE TABLE LetterBestPrintHistory
(
 letter_print_history_id      int           not null PRIMARY KEY identity,
 letter_id                    int           not null FOREIGN KEY REFERENCES LetterBest(letter_id),
 patient_id                   int           not null FOREIGN KEY REFERENCES Patient(patient_id),
 date                         datetime      not null DEFAULT (GETDATE()),
);
