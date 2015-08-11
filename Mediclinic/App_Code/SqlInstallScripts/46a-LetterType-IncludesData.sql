CREATE TABLE LetterType
(
 letter_type_id                   int  not null PRIMARY KEY identity,
 descr                            varchar(50) not null
);

SET IDENTITY_INSERT LetterType ON;
INSERT LetterType
   (letter_type_id,descr)
VALUES
   (3,    'Resident'),
   (214,  'Aged care facility'),
   (234,  'Medicare'),
   (235,  'DVA'),
   (389,  'Doctor'),
   (390,  'Recall'),
   (391,  'Reminder'),
   (392,  'Courtesy'),
   (393,  'Allocated');
SET IDENTITY_INSERT LetterType OFF;