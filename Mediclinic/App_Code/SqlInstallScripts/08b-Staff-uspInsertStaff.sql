CREATE PROCEDURE uspInsertStaff 

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


    ,@staff_id           int
    ,@login              varchar(50)
    ,@pwd                varchar(50)
    ,@staff_position_id  int
    ,@field_id           int
    ,@costcentre_id      int
    ,@is_contractor      bit
    ,@tfn                varchar(50)
    ,@provider_number    varchar(50)
    ,@is_fired           bit
    ,@is_commission      bit
    ,@commission_percent  decimal(5,2)
    ,@is_stakeholder     bit
    ,@is_principal       bit
    ,@is_admin           bit
    ,@staff_date_added   datetime
    ,@start_date         datetime
    ,@end_date           datetime
    ,@comment            varchar(max)


AS

Declare @entity_id int
Declare @person_id int

BEGIN TRAN


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


    SET IDENTITY_INSERT Staff ON

    INSERT INTO Staff (staff_id,person_id,login,pwd,staff_position_id,field_id,costcentre_id,is_contractor,tfn,provider_number,
                       is_fired,is_commission,
                       commission_percent,is_stakeholder,is_principal,is_admin,is_master_admin,is_provider,is_external,staff_date_added,start_date,end_date,comment)
    VALUES
    (
     @staff_id
    ,@person_id
    ,@login
    ,@pwd
    ,@staff_position_id
    ,@field_id
    ,@costcentre_id
    ,@is_contractor
    ,@tfn
    ,@provider_number
    ,@excl_sun
    ,@excl_mon
    ,@excl_tue
    ,@excl_wed
    ,@excl_thu
    ,@excl_fri
    ,@excl_sat
    ,@is_fired
    ,@is_commission
    ,@commission_percent
    ,@is_stakeholder
    ,@is_principal
    ,@is_admin
    ,0
    ,1
	,0
    ,@staff_date_added
    ,@start_date
    ,@end_date
    ,@comment
    )
 
    SET IDENTITY_INSERT Staff OFF


COMMIT TRAN