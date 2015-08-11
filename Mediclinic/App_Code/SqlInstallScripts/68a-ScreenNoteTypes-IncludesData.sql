CREATE TABLE ScreenNoteTypes
(
 screen_note_type_id int           not null PRIMARY KEY identity,
 screen_id           int           not null FOREIGN KEY REFERENCES Screen(screen_id),
 note_type_id        int           FOREIGN KEY REFERENCES NoteType(note_type_id),
 display_order       int           not null,
);


INSERT ScreenNoteTypes
   (screen_id,note_type_id,display_order)
VALUES

   (1, 51,  1),
   (1, 190, 2),
   (1, 209, 3),
   (1, 210, 4),
   (1, 211, 5),
   (1, 223, 6),
   (1, 225, 7),
   (1, 228, 8),

   (2, 51,  1),
   (2, 190, 2),
   (2, 209, 3),
   (2, 210, 4),
   (2, 211, 5),
   (2, 223, 6),
   (2, 225, 7),
   (2, 228, 8),


   (9, 51, 1),
   (9, 254, 2),
   (9, 209, 3),
   (9, 49, 4),
   (9, 50, 5),

   (10, 51, 1),
   (10, 254, 2),
   (10, 209, 3),
   (10, 49, 4),
   (10, 50, 5),

   (11, 223, 1),
   (11, 225, 2),

   (12, 223, 1),
   (12, 225, 2),

   (13, 51, 1),
   (13, 223, 2),

   (14, 51, 1),
   (14, 223, 2),

   (5, 51, 1),
   (5, 254, 2),

   (6, 51, 1),
   (6, 254, 2),

   (15, 252, 1);