CREATE TABLE LetterPrintHistory
(
 letter_print_history_id               int           not null PRIMARY KEY identity,
 letter_id                             int           not null FOREIGN KEY REFERENCES Letter(letter_id),

 letter_print_history_send_method_id   int           not null FOREIGN KEY REFERENCES LetterPrintHistorySendMethod(letter_print_history_send_method_id),

 booking_id                            int                    FOREIGN KEY REFERENCES Booking(booking_id),
 patient_id                            int                    FOREIGN KEY REFERENCES Patient(patient_id),
 organisation_id                       int                    FOREIGN KEY REFERENCES Organisation(organisation_id),
 register_referrer_id                  int                    FOREIGN KEY REFERENCES RegisterReferrer(register_referrer_id), 
 staff_id                              int                    FOREIGN KEY REFERENCES Staff(staff_id),

 health_card_action_id                 int                    FOREIGN KEY REFERENCES HealthCardAction(health_card_action_id), 

 doc_name                              varchar(100)  not null,
 doc_contents                          varbinary(MAX),
 
 from_best                             bit           not null DEFAULT 0,
 date                                  datetime      not null DEFAULT (GETDATE()),
);