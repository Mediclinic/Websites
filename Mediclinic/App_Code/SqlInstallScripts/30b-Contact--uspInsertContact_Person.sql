CREATE PROCEDURE uspInsertContact_Person 

     @staff_id              int
    ,@raw_person_id         int

    ,@contact_id            int
    ,@contact_type_id       int
    ,@free_text             varchar(100)
    ,@addr_line1            varchar(100)
    ,@addr_line2            varchar(100)
    ,@address_channel_id    int
    ,@suburb_id             int
    ,@country_id            int
    ,@site_id               int
    ,@is_billing            bit
    ,@is_non_billing        bit
    ,@contact_date_added    datetime
    ,@contact_date_modified datetime
    ,@contact_date_deleted  datetime

AS

Declare @entity_id int
Declare @person_id int
Declare @count_patient  int
Declare @count_referrer int

BEGIN TRAN

    IF @staff_id <> 0
    BEGIN

        SET @person_id = (SELECT person_id FROM STAFF WHERE staff_id = @staff_id)

    END
    ELSE
    BEGIN

        -- if @raw_person_id exists in patient table, get person id from that
        -- else if @raw_person_id exists in referrer table, get person id from that
        -- else throw error

        SET @count_patient  = (SELECT COUNT(*) FROM Patient WHERE patient_id   = @raw_person_id)
        SET @count_referrer = (SELECT COUNT(*) FROM Referrer WHERE referrer_id = @raw_person_id)


        IF @count_patient = 0 AND @count_referrer = 0
        BEGIN
            RAISERROR('No person found @address_id = %d, @staff_id = %d, raw_person_id = %d ', 16, 1, @contact_id, @staff_id, @raw_person_id)
        END

        IF @count_patient > 0
        BEGIN
            SET @person_id = (SELECT person_id FROM Patient WHERE patient_id = @raw_person_id)
        END

        IF @count_patient = 0 AND @count_referrer > 0
        BEGIN
            SET @person_id = (SELECT person_id FROM Referrer WHERE referrer_id = @raw_person_id)
        END

    END

    SET @entity_id = (SELECT entity_id FROM Person WHERE person_id = @person_id)



    SET IDENTITY_INSERT Contact ON

    INSERT INTO Contact (contact_id,entity_id,contact_type_id,free_text,addr_line1,addr_line2,address_channel_id,suburb_id,country_id,
                         site_id,is_billing,is_non_billing,contact_date_added,contact_date_modified,contact_date_deleted)
    VALUES
    (
     @contact_id
    ,@entity_id
    ,@contact_type_id
    ,@free_text
    ,@addr_line1
    ,@addr_line2
    ,@address_channel_id
    ,@suburb_id
    ,@country_id
    ,@site_id
    ,@is_billing
    ,@is_non_billing
    ,@contact_date_added
    ,@contact_date_modified
    ,@contact_date_deleted
    )
 
    SET IDENTITY_INSERT Contact OFF


COMMIT TRAN