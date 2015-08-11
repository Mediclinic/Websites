CREATE PROCEDURE uspInsertContactAus_Patient

     @patient_id            int

    ,@contact_type_id       int
    ,@free_text             varchar(100)
    ,@addr_line1            varchar(100)
    ,@addr_line2            varchar(100)
	,@street_name           varchar(100)
    ,@address_channel_type_id int
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



    SET @count_patient  = (SELECT COUNT(*) FROM Patient WHERE patient_id = @patient_id)

    IF @count_patient = 0
    BEGIN
        RAISERROR('No person found patient_id = %d ', 16, 1, @patient_id)
    END

    IF @count_patient > 0
    BEGIN
        SET @person_id = (SELECT person_id FROM Patient WHERE patient_id = @patient_id)
    END




    SET @entity_id = (SELECT entity_id FROM Person WHERE person_id = @person_id)



    INSERT INTO ContactAus (entity_id,contact_type_id,free_text,addr_line1,addr_line2,street_name,address_channel_type_id,suburb_id,country_id,
                         site_id,is_billing,is_non_billing,contact_date_added,contact_date_modified,contact_date_deleted)
    VALUES
    (
     @entity_id
    ,@contact_type_id
    ,@free_text
    ,@addr_line1
    ,@addr_line2
    ,@street_name
    ,@address_channel_type_id
    ,@suburb_id
    ,@country_id
    ,@site_id
    ,@is_billing
    ,@is_non_billing
    ,@contact_date_added
    ,@contact_date_modified
    ,@contact_date_deleted
    )
 

COMMIT TRAN