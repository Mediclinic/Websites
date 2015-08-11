CREATE TABLE Staff
(
 staff_id        int     not null PRIMARY KEY identity,
 person_id       int     not null FOREIGN KEY REFERENCES Person(person_id),
 login           varchar(50)   not null, -- UNIQUE,
 pwd             varchar(50)   not null,
 staff_position_id int   not null FOREIGN KEY REFERENCES StaffPosition(staff_position_id),
 field_id        int     not null FOREIGN KEY REFERENCES Field(field_id),
 costcentre_id   int     not null FOREIGN KEY REFERENCES CostCentre(costcentre_id),
 is_contractor   bit     not null,
 tfn             varchar(50)    not null,
 provider_number varchar(50)    not null,   --  for aged care only
 is_fired        bit     not null,
 is_commission   bit     not null,           --  for payroll
 commission_percent  decimal(5,2) not null,  --  for payroll
 is_stakeholder  bit     not null,
 is_master_admin bit     not null,   --  can see a couple of extra things that admins cant see
 is_admin        bit     not null,
 is_principal    bit     not null,   --  if can see all patients (but indicate dont belong to them) .. else view only own patients 
 is_provider     bit     not null,   --  only shows booking sheet if someone is set as provider (& allocated to that org) -- or if they had bookings previously entered
 is_external     bit     not null,

 staff_date_added   datetime     not null DEFAULT (GETDATE()),  -- the date the  staff file was added, so diff to person date_added
 start_date         datetime              DEFAULT null,         -- the date they become a staff, so diff to person created date
 end_date           datetime              DEFAULT null,
 comment            varchar(max) not null,

 num_days_to_display_on_booking_screen int not null DEFAULT 3,
 show_header_on_booking_screen bit         not null DEFAULT 1,
 bk_screen_field_id int              FOREIGN KEY REFERENCES Field(field_id) DEFAULT NULL,
 bk_screen_show_key bit     not null                                        DEFAULT 1,
 enable_daily_reminder_sms   bit           not null DEFAULT 1,
 enable_daily_reminder_email bit           not null DEFAULT 1
);


    --------

    Declare @entity_id int
    Declare @person_id int

    --------

    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()

    INSERT INTO Person (added_by,entity_id, title_id, firstname, middlename, surname, nickname, gender, dob, person_date_added, person_date_modified) 
    VALUES (NULL, @entity_id, 0, 'Support1','','Support1', '','', NULL, GETDATE(), NULL)
    SET @person_id = SCOPE_IDENTITY()

    SET IDENTITY_INSERT Staff ON
    INSERT INTO Staff (staff_id,person_id,login,pwd,staff_position_id,field_id,costcentre_id,is_contractor,tfn,provider_number,
                       is_fired,is_commission,
                       commission_percent, is_stakeholder, is_master_admin, is_admin, is_principal, is_provider, is_external, staff_date_added,start_date,end_date,comment)
    VALUES
    ( -2, @person_id, 'SUPPORT1', 'randompwd', 14, 0, 59, 0, '', '',  0,0,0.00,  1,1,1,0,0,0, GETDATE(), NULL, NULL,'')
    SET IDENTITY_INSERT Staff OFF

    --------

    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()

    INSERT INTO Person (added_by,entity_id, title_id, firstname, middlename, surname, nickname, gender, dob, person_date_added, person_date_modified) 
    VALUES (NULL, @entity_id, 0, 'Support2','','Support2', '','', NULL, GETDATE(), NULL)
    SET @person_id = SCOPE_IDENTITY()

    SET IDENTITY_INSERT Staff ON
    INSERT INTO Staff (staff_id,person_id,login,pwd,staff_position_id,field_id,costcentre_id,is_contractor,tfn,provider_number,
                       is_fired,is_commission,
                       commission_percent, is_stakeholder, is_master_admin, is_admin, is_principal, is_provider, is_external, staff_date_added,start_date,end_date,comment)
    VALUES
    ( -3, @person_id, 'SUPPORT2', 'randompwd', 14, 0, 59, 0, '', '',  0,0,0.00,  1,1,1,0,0,0, GETDATE(), NULL, NULL,'')
    SET IDENTITY_INSERT Staff OFF

    --------

    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()

    INSERT INTO Person (added_by,entity_id, title_id, firstname, middlename, surname, nickname, gender, dob, person_date_added, person_date_modified) 
    VALUES (NULL, @entity_id, 0, 'Support3','','Support3', '','', NULL, GETDATE(), NULL)
    SET @person_id = SCOPE_IDENTITY()

    SET IDENTITY_INSERT Staff ON
    INSERT INTO Staff (staff_id,person_id,login,pwd,staff_position_id,field_id,costcentre_id,is_contractor,tfn,provider_number,
                       is_fired,is_commission,
                       commission_percent, is_stakeholder, is_master_admin, is_admin, is_principal, is_provider, is_external, staff_date_added,start_date,end_date,comment)
    VALUES
    ( -4, @person_id, 'SUPPORT3', 'randompwd', 14, 0, 59, 0, '', '',  0,0,0.00,  1,1,1,0,0,0, GETDATE(), NULL, NULL,'')
    SET IDENTITY_INSERT Staff OFF

    --------

    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()

    INSERT INTO Person (added_by,entity_id, title_id, firstname, middlename, surname, nickname, gender, dob, person_date_added, person_date_modified) 
    VALUES (NULL, @entity_id, 0, 'Firstname','','Surname', '','', NULL, GETDATE(), NULL)
    SET @person_id = SCOPE_IDENTITY()

    SET IDENTITY_INSERT Staff ON
    INSERT INTO Staff (staff_id,person_id,login,pwd,staff_position_id,field_id,costcentre_id,is_contractor,tfn,provider_number,
                       is_fired,is_commission,
                       commission_percent, is_stakeholder, is_master_admin, is_admin, is_principal, is_provider, is_external, staff_date_added,start_date,end_date,comment)
    VALUES
    ( 1, @person_id, 'login', 'pwd', 14, 0, 56, 0, '', '',  0,0,0.00,  0,1,1,0,0,0, GETDATE(), NULL, NULL,'')
    SET IDENTITY_INSERT Staff OFF

    --------