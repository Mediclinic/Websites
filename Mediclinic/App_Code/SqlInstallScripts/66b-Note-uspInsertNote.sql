CREATE PROCEDURE uspInsertNote 

     @note_id             int
    ,@note_type_id        int
    ,@text                varchar(max)
    ,@date_added          datetime
    ,@date_modified       datetime
    ,@site_id             int

    ,@import_type         varchar(50)   -- to know where to get entityid from
    ,@id                  int           -- to know where to get entityid from
          
AS

Declare @person_id int
Declare @entity_id int

BEGIN TRAN

    IF @import_type = 'booking'
    BEGIN
        SET @entity_id = (SELECT entity_id FROM Booking       WHERE booking_id = @id)
    END
    IF @import_type = 'patient'
    BEGIN
        SET @person_id = (SELECT person_id FROM Patient       WHERE patient_id = @id)
        SET @entity_id = (SELECT entity_id FROM Person        WHERE person_id = @person_id)
    END
    --IF @import_type = 'referrer'
    --BEGIN
    --    SET @person_id = (SELECT person_id FROM Referrer      WHERE referrer_id = @id)
    --    SET @entity_id = (SELECT entity_id FROM Person        WHERE person_id = @person_id)
    --END
    --IF @import_type = 'staff'
    --BEGIN
    --    SET @person_id = (SELECT person_id FROM Staff         WHERE staff_id = @id)
    --    SET @entity_id = (SELECT entity_id FROM Person        WHERE person_id = @person_id)
    --END
    IF @import_type = 'org'
    BEGIN
        SET @entity_id = (SELECT entity_id FROM Organisation  WHERE organisation_id = @id)
    END
    IF @import_type = 'site'
    BEGIN
        SET @entity_id = (SELECT entity_id FROM Site          WHERE site_id = @id)
    END
    IF @import_type = 'invoice'
    BEGIN
        SET @entity_id = (SELECT entity_id FROM Invoice       WHERE invoice_id = @id)
    END
    IF      @import_type <> 'booking'
        AND @import_type <> 'patient'
    --    AND @import_type <> 'referrer'   -- referrers dont have notes
    --    AND @import_type <> 'staff'      -- referrers dont have notes
        AND @import_type <> 'org' 
        AND @import_type <> 'site' 
        AND @import_type <> 'invoice'
    BEGIN
        RAISERROR('Unknown type: @import_type = %d, @note_id = %d ', 16, 1, @import_type, @note_id)
    END


    IF @entity_id IS NULL
    BEGIN
        RAISERROR('Unknown entity_id: @note_id = %d ', 16, 1, @note_id)
    END



    SET IDENTITY_INSERT Note ON

    INSERT INTO Note (note_id,entity_id,note_type_id,text,date_added,date_modified,site_id)
    VALUES
    (
     @note_id
    ,@entity_id
    ,@note_type_id
    ,@text
    ,@date_added
    ,@date_modified
    ,@site_id
    )
 
    SET IDENTITY_INSERT Note OFF


COMMIT TRAN