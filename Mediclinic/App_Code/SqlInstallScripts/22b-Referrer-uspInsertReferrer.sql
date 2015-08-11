CREATE PROCEDURE uspInsertReferrer 

     @added_by             int
    ,@title_id             int
    ,@firstname            varchar(100)
    ,@middlename           varchar(100)
    ,@surname              varchar(100)
    ,@nickname              varchar(100)
    ,@gender               varchar(1)
    ,@dob                  datetime
    ,@person_date_added    datetime
    ,@person_date_modified datetime

    ,@referrer_id          int
    ,@referrer_date_added  datetime
    ,@is_deleted           bit


AS

Declare @entity_id int
Declare @person_id int
Declare @count int

BEGIN TRAN


    SET @count = (SELECT COUNT(*) FROM Patient WHERE patient_id = @referrer_id)
    IF @count = 0
    BEGIN

        INSERT INTO Entity DEFAULT VALUES;
        SET @entity_id = SCOPE_IDENTITY()


        INSERT INTO Person (added_by,entity_id, title_id, firstname, middlename, surname, nickname, gender, dob, person_date_added, person_date_modified) 
        VALUES
        (
         @added_by
        ,@entity_id
        ,@title_id
        ,@firstname
        ,@middlename
        ,@surname
        ,@nickname
        ,@gender
        ,@dob
        ,@person_date_added
        ,@person_date_modified
        )

        SET @person_id = SCOPE_IDENTITY()

    END
    ELSE
    BEGIN

        SET @person_id = (SELECT person_id FROM Patient WHERE patient_id = @referrer_id)

    END


    SET IDENTITY_INSERT Referrer ON

    INSERT INTO Referrer (referrer_id,person_id, referrer_date_added, is_deleted) 
    VALUES
    (
     @referrer_id
    ,@person_id
    ,@referrer_date_added
    ,@is_deleted
    )
 
    SET IDENTITY_INSERT Referrer OFF


COMMIT TRAN
