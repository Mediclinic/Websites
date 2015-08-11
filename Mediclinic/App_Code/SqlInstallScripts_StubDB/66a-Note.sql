CREATE TABLE Note
(
 note_id             int           not null PRIMARY KEY identity,
 entity_id           int           not null FOREIGN KEY REFERENCES Entity(entity_id),
 note_type_id        int           not null FOREIGN KEY REFERENCES NoteType(note_type_id),
 body_part_id        int                    FOREIGN KEY REFERENCES BodyPart(body_part_id) DEFAULT NULL,
 text                varchar(max)  not null,
 date_added          datetime      not null DEFAULT (GETDATE()),
 date_modified       datetime      DEFAULT null,
 added_by            int                    FOREIGN KEY REFERENCES Staff(staff_id),
 modified_by         int                    FOREIGN KEY REFERENCES Staff(staff_id),
 site_id             int                    FOREIGN KEY REFERENCES Site(site_id),        -- 0/1=ac, 2=clinic -- only patients else null => to know is note for a patient is cilnic note or ag note
                                                                                -- nb: a note for a "site" will have site id as entityid
);