CREATE TABLE NoteType
(
 note_type_id       int  not null PRIMARY KEY identity,
 descr              varchar(50) not null
);

SET IDENTITY_INSERT NoteType ON;
INSERT NoteType
   (note_type_id,descr)
VALUES
   (48,  'Problem'),
   (49,  'Compliment'),
   (50,  'Contact Made'),
   (51,  'Admin Notes Only'),
   (190, 'Referral Message'),
   (209, 'Patient List - Invoice'),
   (210, 'Receipt Message'),
   (211, 'Credit Note Message'),
   (223, 'Parking'),
   (225, 'Point of Sale'),
   (228, 'Refund Message'),
   (252, 'Provider Note'),
   (253, 'Facility Patient List'),
   (254, 'Patient List - Medicare Med Cond'),
   (316, 'Provider Clinic Notes');
SET IDENTITY_INSERT NoteType OFF;