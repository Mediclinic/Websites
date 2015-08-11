CREATE PROCEDURE uspInsertPatient 

     @staff_id             int

    ,@added_by             int
    ,@title_id             int
    ,@firstname            varchar(100)
    ,@middlename           varchar(100)
    ,@surname              varchar(100)
    ,@nickname              varchar(100)
    ,@gender               varchar(1)
    ,@dob                  datetime
    ,@person_date_added    datetime
    ,@person_date_modified datetime

    ,@patient_id         int
    ,@patient_date_added datetime
    ,@is_clinic_patient  bit
    ,@is_deleted         bit
    ,@is_deceased        bit


AS

Declare @entity_id int
Declare @person_id int

BEGIN TRAN

    IF @staff_id = 0
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

        SET @person_id = (SELECT person_id FROM STAFF WHERE staff_id = @staff_id)

    END


    SET IDENTITY_INSERT Patient ON

    INSERT INTO Patient (patient_id,person_id, patient_date_added, is_clinic_patient, is_deleted, is_deceased, flashing_text, flashing_text_added_by, flashing_text_last_modified_date, private_health_fund, concession_card_number,concession_card_expiry_date,is_diabetic,is_member_diabetes_australia,diabetic_assessment_review_date) 
    VALUES
    (
     @patient_id
    ,@person_id
    ,@patient_date_added
    ,@is_clinic_patient
    ,@is_deleted
    ,@is_deceased
    ,''
	,NULL
	,NULL
    ,''
    ,''
    ,null
    ,0
    ,0
	,NULL
    )
 
    SET IDENTITY_INSERT Patient OFF

COMMIT TRAN
