CREATE PROCEDURE uspInsertContact_Other 

     @id                    int
    ,@table                 varchar(50)

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
Declare @count  int


BEGIN TRAN


    IF @table = 'Organisation'    -- GROUP ORG JUST SEND IN -1 * THE ID  !!
    BEGIN

        SET @count = (SELECT COUNT(*) FROM Organisation WHERE organisation_id   = @id)
        IF @count = 0
        BEGIN
            RAISERROR('No organisation with id = %d (@address_id = %d)', 16, 1, @id, @contact_id)
        END
        SET @entity_id = (SELECT entity_id FROM Organisation WHERE organisation_id = @id)

    END
    ELSE
    BEGIN

        IF @table = 'Site'
        BEGIN

            SET @count = (SELECT COUNT(*) FROM Site WHERE site_id   = @id)
            IF @count = 0
            BEGIN
                RAISERROR('No site with id = %d (@address_id = %d)', 16, 1, @id, @contact_id)
            END
            SET @entity_id = (SELECT entity_id FROM Site WHERE site_id = @id)

        END
        ELSE
        BEGIN
            RAISERROR('Unknown type : %d', 16, 1, @table)
        END

    END



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
