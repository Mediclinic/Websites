CREATE TABLE LetterPrintHistorySendMethod
(
 letter_print_history_send_method_id      int         not null PRIMARY KEY identity,
 descr                                    varchar(50) not null
);

SET IDENTITY_INSERT LetterPrintHistorySendMethod ON;
INSERT LetterPrintHistorySendMethod 
   (letter_print_history_send_method_id,descr)
VALUES
   (1, 'Mail'),
   (2, 'Email'),
   (3, 'SMS');
SET IDENTITY_INSERT LetterPrintHistorySendMethod OFF;