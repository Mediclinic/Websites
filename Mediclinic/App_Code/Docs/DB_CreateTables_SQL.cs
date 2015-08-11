
/* uspAddRiseEmail

--------------------------------------

-- DROP PROCEDURE uspAddRiseEmail;

CREATE PROCEDURE uspAddRiseEmail 

	 @email                  varchar(250)				
	,@dob                    datetime
	,@gender                 varchar(1)
	,@patientHomeTelephone   varchar(100)
	,@patientWorkTelephone   varchar(100)
	,@patientMobileTelephone varchar(100)
	,@firstname              varchar(100)
	,@surname                varchar(100)

AS

Declare @pt_count int
Declare @entity_id int

BEGIN TRAN


    SET @pt_count = (SELECT count(*) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname)
    IF (@entity_id IS NOT NULL AND @pt_count = 1)
    BEGIN
        SET @entity_id = (SELECT Person.entity_id FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname)
    END

    SET @pt_count = (SELECT count(*) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname and gender = @gender)
    IF (@entity_id IS NOT NULL AND @pt_count = 1)
    BEGIN
        SET @entity_id = (SELECT Person.entity_id FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname and gender = @gender)
    END


    SET @pt_count = (SELECT count(*) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname 
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientHomeTelephone   or REPLACE(addr_line1, ' ', '') = REPLACE(@patientHomeTelephone,   ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
					   )
    IF (@entity_id IS NULL AND @pt_count = 1)
    BEGIN
        SET @entity_id = (SELECT Person.entity_id 
						  FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id 
						  WHERE firstname = @firstname AND surname = @surname 
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientHomeTelephone   or REPLACE(addr_line1, ' ', '') = REPLACE(@patientHomeTelephone,   ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
						 )
    END

    SET @pt_count = (SELECT count(*) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname 
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientHomeTelephone   or REPLACE(addr_line1, ' ', '') = REPLACE(@patientHomeTelephone,   ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
					   )
    IF (@entity_id IS NULL AND @pt_count = 1)
    BEGIN
        SET @entity_id = (SELECT Person.entity_id 
						  FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id 
						  WHERE firstname = @firstname AND surname = @surname 
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientHomeTelephone   or REPLACE(addr_line1, ' ', '') = REPLACE(@patientHomeTelephone,   ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
						 )
    END

    SET @pt_count = (SELECT count(*) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname 
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
					   )
    IF (@entity_id IS NULL AND @pt_count = 1)
    BEGIN
        SET @entity_id = (SELECT Person.entity_id 
						  FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id 
						  WHERE firstname = @firstname AND surname = @surname 
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
						 )
    END

    SET @pt_count = (SELECT count(*) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname 
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientHomeTelephone   or REPLACE(addr_line1, ' ', '') = REPLACE(@patientHomeTelephone,   ' ', '')) ) >= 1
					   )
    IF (@entity_id IS NULL AND @pt_count = 1)
    BEGIN
        SET @entity_id = (SELECT Person.entity_id 
						  FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id 
						  WHERE firstname = @firstname AND surname = @surname 
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', '')) ) >= 1
							AND ( SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientHomeTelephone   or REPLACE(addr_line1, ' ', '') = REPLACE(@patientHomeTelephone,   ' ', '')) ) >= 1
						 )
    END




    SET @pt_count = (SELECT count(*) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname AND (SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientHomeTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientHomeTelephone, ' ', ''))) >= 1)
    IF (@entity_id IS NULL AND @pt_count = 1)
    BEGIN
        SET @entity_id = (SELECT Person.entity_id FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname AND (SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientHomeTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientHomeTelephone, ' ', ''))) >= 1)
    END

    SET @pt_count = (SELECT count(*) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname AND (SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientWorkTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientWorkTelephone, ' ', ''))) >= 1)
    IF (@entity_id IS NULL AND @pt_count = 1)
    BEGIN
        SET @entity_id = (SELECT Person.entity_id FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname AND (SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientWorkTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientWorkTelephone, ' ', ''))) >= 1)
    END

    SET @pt_count = (SELECT count(*) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname AND (SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', ''))) >= 1)
    IF (@entity_id IS NULL AND @pt_count = 1)
    BEGIN
        SET @entity_id = (SELECT Person.entity_id FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @firstname AND surname = @surname AND (SELECT COUNT(*) FROM ContactAus WHERE entity_id = Person.entity_id AND (addr_line1 = @patientMobileTelephone or REPLACE(addr_line1, ' ', '') = REPLACE(@patientMobileTelephone, ' ', ''))) >= 1)
    END





    IF @entity_id IS NOT NULL
    BEGIN

 		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				27,
				'',  -- free text
				@email,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				NULL,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

    END
    ELSE
    BEGIN

        PRINT '--- NOT FOUND ---' + @email

    END

COMMIT TRAN

--------------------------------------

 */



/*
DELETE FROM Person WHERE        (person_id NOT IN (SELECT person_id FROM Staff)) AND (person_id NOT IN  (SELECT  person_id FROM Patient)) AND (person_id NOT IN  (SELECT  person_id FROM Referrer));
DELETE FROM Entity WHERE        (entity_id NOT IN (SELECT entity_id FROM Site)) AND (entity_id NOT IN (SELECT entity_id FROM  Person)) AND (entity_id NOT IN (SELECT entity_id FROM  Organisation)) AND (entity_id NOT IN (SELECT entity_id FROM  Booking))  AND (entity_id NOT IN (SELECT entity_id FROM  Invoice));
DBCC CHECKIDENT(Person,RESEED,1);
DBCC CHECKIDENT(Person);
DBCC CHECKIDENT(Entity,RESEED,1);
DBCC CHECKIDENT(Entity);

DELETE FROM Entity WHERE        (entity_id NOT IN (SELECT entity_id FROM Site)) AND (entity_id NOT IN (SELECT entity_id FROM  Person)) AND (entity_id NOT IN (SELECT entity_id FROM  Organisation)) AND (entity_id NOT IN (SELECT entity_id FROM Invoice))  AND (entity_id NOT IN (SELECT entity_id FROM Booking));
DBCC CHECKIDENT(Entity,RESEED,1);
DBCC CHECKIDENT(Entity);
*/

/* Indexes

CREATE INDEX MCSFIndex01 ON Booking (patient_id);
CREATE INDEX MCSFIndex02 ON Invoice (booking_id);
CREATE INDEX MCSFIndex03 ON InvoiceLine (invoice_id);
CREATE INDEX MCSFIndex04 ON InvoiceLine (patient_id);
CREATE INDEX MCSFIndex05 ON Note (entity_id);
CREATE INDEX MCSFIndex06 ON ContactAus (entity_id);
CREATE INDEX MCSFIndex07 ON Person (entity_id);
CREATE INDEX MCSFIndex08 ON Organisation (entity_id);
CREATE INDEX MCSFIndex09 ON Booking (entity_id);
CREATE INDEX MCSFIndex10 ON Invoice (entity_id);
CREATE INDEX MCSFIndex11 ON BookingPatient (booking_id);
CREATE INDEX MCSFIndex12 ON BookingPatient (patient_id);

*/


/* SystemVariable 

CREATE TABLE SystemVariable
(
 descr                varchar(200)    not null UNIQUE,
 value                varchar(MAX)    not null,
 editable_in_gui      bit             not null,
 viewable_in_gui      bit             not null,
);

INSERT SystemVariable (descr, value, editable_in_gui, viewable_in_gui) VALUES 
('LastDateBatchSendTreatmentNotesAllReferrers','',0,1),
('MedicareMaxNbrServicesPerYear','5',1,1),
('MedicareEclaimsLicenseNbr','12345',1,1),
('SMSPrice','0.15',1,1),
('SMSCreditNotificationEmailAddress','', 1,1),
('SMSCreditOutOfBalance_SendEmail',  '0',1,1),   -- sent once per day when reminders running when not out before run, but out after
('SMSCreditLowBalance_SendEmail',    '0',1,1),   -- sent once per day when reminders running when sms run havent reached before, but after run has reached it
('SMSCreditLowBalance_Threshold',    '0',1,1),
('MaxNbrProviders','0',1,1),
('StockWarningNotificationEmailAddress','', 1,1),

('BirthdayNotificationEmail_SendEmail','0',0,1),
('BirthdayNotificationEmail_EmailAddress','',0,1),
('BirthdayNotificationEmail_IncPatientsWithMobile','0',0,1),
('BirthdayNotificationEmail_IncPatientsWithEmail','0',0,1),
('BirthdayNotificationEmail_SendMondays','0',0,1),
('BirthdayNotificationEmail_SendTuesdays','0',0,1),
('BirthdayNotificationEmail_SendWednesdays','0',0,1),
('BirthdayNotificationEmail_SendThursdays','0',0,1),
('BirthdayNotificationEmail_SendFridays','0',0,1),
('BirthdayNotificationEmail_SendSaturdays','0',0,1),
('BirthdayNotificationEmail_SendSundays','0',0,1),
('BirthdayNotificationEmail_SendFromDaysAhead_Mondays','0',0,1),
('BirthdayNotificationEmail_SendUntilDaysAhead_Mondays','0',0,1),
('BirthdayNotificationEmail_SendFromDaysAhead_Tuesdays','0',0,1),
('BirthdayNotificationEmail_SendUntilDaysAhead_Tuesdays','0',0,1),
('BirthdayNotificationEmail_SendFromDaysAhead_Wednesdays','0',0,1),
('BirthdayNotificationEmail_SendUntilDaysAhead_Wednesdays','0',0,1),
('BirthdayNotificationEmail_SendFromDaysAhead_Thursdays','0',0,1),
('BirthdayNotificationEmail_SendUntilDaysAhead_Thursdays','0',0,1),
('BirthdayNotificationEmail_SendFromDaysAhead_Fridays','0',0,1),
('BirthdayNotificationEmail_SendUntilDaysAhead_Fridays','0',0,1),
('BirthdayNotificationEmail_SendFromDaysAhead_Saturdays','0',0,1),
('BirthdayNotificationEmail_SendUntilDaysAhead_Saturdays','0',0,1),
('BirthdayNotificationEmail_SendFromDaysAhead_Sundays','0',0,1),
('BirthdayNotificationEmail_SendUntilDaysAhead_Sundays','0',0,1),

('ReferrerEPCAutoGenerateLettersEmail_SendEmail','0',0,1),
('ReferrerEPCAutoGenerateLettersEmail_EmailAddress','',0,1),
('ReferrerEPCAutoGenerateLettersEmail_IncUnsent','1',0,1),
('ReferrerEPCAutoGenerateLettersEmail_IncBatched','0',0,1),
('ReferrerEPCAutoGenerateLettersEmail_SendMethod','Email',0,1),
('ReferrerEPCAutoGenerateLettersEmail_SendMondays','0',0,1),
('ReferrerEPCAutoGenerateLettersEmail_SendTuesdays','0',0,1),
('ReferrerEPCAutoGenerateLettersEmail_SendWednesdays','0',0,1),
('ReferrerEPCAutoGenerateLettersEmail_SendThursdays','0',0,1),
('ReferrerEPCAutoGenerateLettersEmail_SendFridays','0',0,1),
('ReferrerEPCAutoGenerateLettersEmail_SendSaturdays','0',0,1),
('ReferrerEPCAutoGenerateLettersEmail_SendSundays','0',0,1),

('ServiceSpecificBookingReminderLettersToBatch_EmailAddress','',0,1) ,

('AllowAddSiteClinic','0',0,1) ,
('AllowAddSiteAgedCare','0',0,1) ,
('AllowAddSiteGP','0',0,1) ,

('AddressType','ContactAus',0,1) ,
('DefaultState','VIC',0,1) ,

-- note that when adding new site, get the to enter all these, and change the values in the db script when creating db
('Site','New Site',0,1) ,
('CssPage','~/Styles/Site_Balwyn.css',0,1) ,
('BannerMessage','',0,1) ,
('ShowBannerMessage','False',0,1) ,
('Email_FromName','PODIATRYCLINICS',0,1) ,
('Email_FromEmail','info@podiatryclinics.com.au',0,1) ,
('AdminAlertEmail_To','info@podiatryclinics.com.au',0,1) ,

('AutoMedicareClaiming','0',0,1) ,
('PaymentDueDayOfMonth','1',0,1) ,

('LettersEmailSignature','',0,1) ,
('LettersEmailDefaultSubject','',0,1) ,

('InvoiceGapPayments','0',0,1) ,

('EnableDailyBookingReminderSMS',     '1',1,1),
('EnableDailyBookingReminderEmails',  '1',1,1),
('EnableBirthdaySMS',                 '1',1,1),
('EnableBirthdayEmails',              '1',1,1),

('EnableDeletedBookingsAlerts',       '1',1,1),
 
('BookingSheetTimeSlotMins_Clinic',  '10',0,1),
('BookingSheetTimeSlotMins_AgedCare','10',0,1),
('BookingSheetTimeSlotMins_GP',      '6' ,0,1),

('EziDebit_Enabled'   ,'0',0,1),
('EziDebit_DigitalKey','',0,1),
('EziDebit_FormDate'  ,'',0,1),
('Rate_IncomingCall'  ,'0',0,1),
('Rate_OutgoingCall'  ,'0',0,1),
('Rate_CreditCardAmt' ,'0',0,1),
('Rate_CreditCardPct' ,'0',0,1),
('Rate_DebitCardAmt'  ,'0',0,1),
('Rate_DebitCardPct'  ,'0',0,1),

('NbrDaysAheadToSendDailyBookingReminderSMS'  ,'1',0,1),
('SendDailyBookingReminderText_SMS'  ,'',0,1),
('SendDailyBookingReminderText_Email'  ,'',0,1),
('SendDailyBookingReminderText_EmailSubject','Reminder - Appointment at org_name tomorrow at bk_time',0,1);

('PT_Reminders_HasBothSMSandEmail','Both',0,1),
('Staff_Reminders_HasBothSMSandEmail','Both',0,1),

('MainLogo'  ,'comp_logo.png',0,1),
('MainLogoBackground'  ,'login_bg.png',0,1),

('BookingColour_Unavailable'                          ,'#F78181',0,1),
('BookingColour_Available'                            ,'#FFFFFF',0,1),
('BookingColour_UnavailableButAddable'                ,'#F78181',0,1),
('BookingColour_UnavailableButUpdatable'              ,'#F78181',0,1),
('BookingColour_Updatable'                            ,'#F0E68C',0,1),
('BookingColour_FullDayTaken'                         ,'#F78181',0,1),
('BookingColour_CL_EPC_Past_Completed_Has_Invoice'    ,'#EFDFC1',0,1),
('BookingColour_CL_EPC_Past_Completed_No_Invoice'     ,'#EFDFC1',0,1),
('BookingColour_CL_EPC_Future_Unconfirmed'            ,'#D1F7BE',0,1),
('BookingColour_CL_EPC_Future_Confirmed'              ,'#97E173',0,1),
('BookingColour_AC_EPC_Past_Completed_Has_Invoice'    ,'#EFDFC1',0,1),
('BookingColour_AC_EPC_Past_Completed_No_Invoice'     ,'#EFDFC1',0,1),
('BookingColour_AC_EPC_Future_Unconfirmed'            ,'#BAD2FF',0,1),
('BookingColour_AC_EPC_Future_Confirmed'              ,'#7DA9FF',0,1),
('BookingColour_CL_NonEPC_Past_Completed_Has_Invoice' ,'#EFDFC1',0,1),
('BookingColour_CL_NonEPC_Past_Completed_No_Invoice'  ,'#EFDFC1',0,1),
('BookingColour_CL_NonEPC_Future_Unconfirmed'         ,'#FFFFAA',0,1),
('BookingColour_CL_NonEPC_Future_Confirmed'           ,'#FDD017',0,1),
('BookingColour_AC_NonEPC_Past_Completed_Has_Invoice' ,'#EFDFC1',0,1),
('BookingColour_AC_NonEPC_Past_Completed_No_Invoice'  ,'#EFDFC1',0,1),
('BookingColour_AC_NonEPC_Future_Unconfirmed'         ,'#D3FFFA',0,1),
('BookingColour_AC_NonEPC_Future_Confirmed'           ,'#65FFEA',0,1),
('BookingColour_Future_PatientLoggedIn'               ,'#FFFFAA',0,1),
('BookingColour_Past_PatientLoggedIn'                 ,'#EFDFC1',0,1),

('EnableAlert_BookingAddedByPT','1',0,1),
('EnableAlert_BookingEditedByPT','1',0,1),
('EnableAlert_BookingDeletedByPT','1',0,1),
('PTLogin_BookingTimeEditable','0',0,1),

('GST_Percent','10',0,1),

('CC_Nbr','',0,1),
('CC_Exp_Mo','',0,1),
('CC_Exp_Yr','',0,1),
('CC_CCV','',0,1),

('EnableAutoMontlyOverdueReminders','1',0,1),

('AutoSendFaxesAsEmailsIfNoEmailExistsToGPs','0',0,1),

('ProvsCanSeePricesWhenCompletingBks_AC' ,'1',0,1),
('Bookings_ProvsCanSeeOtherProvs'        ,'1',0,1),
('Bookings_ProvsCanSeePatientsOfAllOrgs' ,'0',0,1),

('NextAvailableDefaultNbrDaysShown','42',0,1) ;

*/


/* Entity 

CREATE TABLE Entity
(
 entity_id       int     not null PRIMARY KEY identity
);

*/


/* SiteType

CREATE TABLE SiteType
(
 site_type_id           int     not null PRIMARY KEY identity,
 descr                  varchar(50)    not null,
);

SET IDENTITY_INSERT SiteType ON;
INSERT SiteType
  (site_type_id,descr)
VALUES
  (1,'Clinic'),
  (2,'Aged Care'),
  (3,'GP');
SET IDENTITY_INSERT SiteType OFF;

*/

/* Site  (ref: Entity)

- can have 0 or 1 of each TYPE (clinic/ac) of site
- disallow adding of another site of that type if already has that type of site

CREATE TABLE Site
(
 site_id           int          not null PRIMARY KEY identity,
 entity_id         int          not null FOREIGN KEY REFERENCES Entity(entity_id) UNIQUE,
 name              varchar(100) not null,
 site_type_id      int          not null FOREIGN KEY REFERENCES SiteType(site_type_id),
 abn               varchar(50)  not null,
 acn               varchar(50)  not null,
 tfn               varchar(50)  not null,
 asic              varchar(50)  not null,
 is_provider       bit          not null,   -- for aged care only
 bank_bpay         varchar(50)  not null,	
 bank_bsb          varchar(50)  not null,	
 bank_account      varchar(50)  not null,
 bank_direct_debit_userid       varchar(50)   not null,  -- number bank gives to do direct debits
 bank_username                  varchar(50)   not null,  -- name to be used on Debit order transfer
 oustanding_balance_warning     decimal(10,2) not null,  -- Warning no treat if balance over this (used in aged care)
 print_epc         bit          not null,  -- Enhanced Primary Care
 excl_sun          bit          not null,
 excl_mon          bit          not null,
 excl_tue          bit          not null,
 excl_wed          bit          not null,
 excl_thu          bit          not null,
 excl_fri          bit          not null,
 excl_sat          bit          not null,
 day_start_time    time         not null,
 lunch_start_time  time         not null,
 lunch_end_time    time         not null,
 day_end_time      time         not null,
 fiscal_yr_end     datetime     not null,
 num_booking_months_to_get int  not null DEFAULT 9 -- how many bookings to get when starting bookings
);

*/

/* uspInsertSite

-- DROP PROCEDURE uspInsertSite;

CREATE PROCEDURE uspInsertSite 

 @site_id int
,@name varchar(100)
,@is_clinic bit
,@abn  varchar(50)
,@acn  varchar(50)
,@tfn  varchar(50)
,@asic varchar(50)
,@is_provider bit
,@bank_bpay    varchar(50)
,@bank_bsb     varchar(50)
,@bank_account varchar(50)
,@bank_direct_debit_userid    varchar(50)
,@bank_username               varchar(50)
,@oustanding_balance_warning  decimal(10,2)
,@print_epc         bit
,@excl_sun          bit
,@excl_mon          bit
,@excl_tue          bit
,@excl_wed          bit
,@excl_thu          bit
,@excl_fri          bit
,@excl_sat          bit
,@day_start_time    time
,@lunch_start_time  time
,@lunch_end_time    time
,@day_end_time      time
,@fiscal_yr_end     datetime
,@num_booking_months_to_get int

AS

Declare @entity_id int

BEGIN TRAN

    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()


    SET IDENTITY_INSERT Site ON

    INSERT INTO Site (site_id,entity_id,name,is_clinic,abn,acn,tfn,asic,is_provider,bank_bpay,bank_bsb,bank_account,bank_direct_debit_userid,bank_username,oustanding_balance_warning,print_epc,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,excl_fri,excl_sat,day_start_time,lunch_start_time,lunch_end_time,day_end_time,fiscal_yr_end,num_booking_months_to_get) 
    VALUES
    (
     @site_id 
    ,@entity_id
    ,@name
    ,@is_clinic
    ,@abn
    ,@acn
    ,@tfn
    ,@asic
    ,@is_provider
    ,@bank_bpay
    ,@bank_bsb
    ,@bank_account
    ,@bank_direct_debit_userid
    ,@bank_username
    ,@oustanding_balance_warning
    ,@print_epc
    ,@excl_sun
    ,@excl_mon
    ,@excl_tue
    ,@excl_wed
    ,@excl_thu
    ,@excl_fri
    ,@excl_sat
    ,@day_start_time
    ,@lunch_start_time
    ,@lunch_end_time
    ,@day_end_time
    ,@fiscal_yr_end 
    ,@num_booking_months_to_get 
    )
 
    SET IDENTITY_INSERT Site OFF


COMMIT TRAN

--------------------------------------

 */


/* Title 

CREATE TABLE Title
(
 title_id        int            not null PRIMARY KEY identity,
 descr           varchar(50)    not null,
 display_order   int            not null
);

*/

/* Person 

CREATE TABLE Person
(
 person_id       int     not null PRIMARY KEY identity,
 entity_id       int     not null FOREIGN KEY REFERENCES Entity(entity_id) UNIQUE,
 added_by        int     FOREIGN KEY REFERENCES Person(person_id) DEFAULT null,
 title_id        int     not null FOREIGN KEY REFERENCES Title(title_id),
 firstname       varchar(100)    not null,
 middlename      varchar(100)    not null,
 surname         varchar(100)    not null,
 nickname        varchar(100)    not null,
 gender          varchar(1)      not null,    -- M or F
 dob             datetime,
 person_date_added      datetime not null DEFAULT (GETDATE()),
 person_date_modified   datetime DEFAULT null
);

*/



/* Changing Contact to ContactAus

	SELECT * INTO ContactAus  FROM Contact ;
	ALTER TABLE ContactAus ADD CONSTRAINT DF_DefaultAddedDate DEFAULT (GETDATE()) FOR contact_date_added;;

	ALTER TABLE ContactAus ADD  street_name              varchar(100)  not null default '';
	ALTER TABLE ContactAus ADD  address_channel_type_id  int                    FOREIGN KEY REFERENCES AddressChannelType(address_channel_type_id) default null;

	UPDATE ContactAus
	SET
		ContactAus.street_name             = AddressChannel.descr,
		ContactAus.address_channel_type_id = AddressChannel.address_channel_type_id
	FROM  ContactAus
	INNER JOIN AddressChannel     ON ContactAus.address_channel_id          = AddressChannel.address_channel_id
	INNER JOIN AddressChannelType ON AddressChannel.address_channel_type_id = AddressChannelType.address_channel_type_id;

	ALTER TABLE ContactAus DROP COLUMN address_channel_id;


	SELECT * 
	from ContactAus
	--INNER JOIN AddressChannel     ON ContactAus.address_channel_id          = AddressChannel.address_channel_id
	--INNER JOIN AddressChannelType ON AddressChannel.address_channel_type_id = AddressChannelType.address_channel_type_id
	INNER JOIN AddressChannelType ON ContactAus.address_channel_type_id = AddressChannelType.address_channel_type_id;


    -- drop table Contact
    -- update SystemVariable set value = 'ContactAus' where descr = 'AddressType'
*/


/* Country

CREATE TABLE Country
(
 country_id             int          not null PRIMARY KEY identity,
 descr                  varchar(50) not null,
);


SET IDENTITY_INSERT Country ON;
INSERT Country
  (country_id,descr)
VALUES
  (3,'NEW ZEALAND'),
  (40,'UNITED KINGDOM'),
  (44,'AUSTRALIA'),
  (167,'UNITES STATES');
SET IDENTITY_INSERT Country OFF;


INSERT Country
  (descr)
VALUES
  (UPPER('Afghanistan')),
  (UPPER('Albania')),
  (UPPER('Algeria')),
  (UPPER('American Samoa')),
  (UPPER('Andorra')),
  (UPPER('Angola')),
  (UPPER('Anguilla')),
  (UPPER('Antigua and Barbuda')),
  (UPPER('Argentina')),
  (UPPER('Armenia')),
  (UPPER('Aruba')),
--  (UPPER('Australia')),
  (UPPER('Austria')),
  (UPPER('Azerbaijan')),
  (UPPER('Bahamas')),
  (UPPER('Bahrain')),
  (UPPER('Bangladesh')),
  (UPPER('Barbados')),
  (UPPER('Belarus')),
  (UPPER('Belgium')),
  (UPPER('Belize')),
  (UPPER('Benin')),
  (UPPER('Bermuda')),
  (UPPER('Bhutan')),
  (UPPER('Bolivia')),
  (UPPER('Bosnia-Herzegovina')),
  (UPPER('Botswana')),
  (UPPER('Bouvet Island')),
  (UPPER('Brazil')),
  (UPPER('Brunei')),
  (UPPER('Bulgaria')),
  (UPPER('Burkina Faso')),
  (UPPER('Burundi')),
  (UPPER('Cambodia')),
  (UPPER('Cameroon')),
  (UPPER('Canada')),
  (UPPER('Cape Verde')),
  (UPPER('Cayman Islands')),
  (UPPER('Central African Republic')),
  (UPPER('Chad')),
  (UPPER('Chile')),
  (UPPER('China')),
  (UPPER('Christmas Island')),
  (UPPER('Cocos (Keeling) Islands')),
  (UPPER('Colombia')),
  (UPPER('Comoros')),
  (UPPER('Congo, Democratic Republic of the (Zaire)')),
  (UPPER('Congo, Republic of')),
  (UPPER('Cook Islands')),
  (UPPER('Costa Rica')),
  (UPPER('Croatia')),
  (UPPER('Cuba')),
  (UPPER('Cyprus')),
  (UPPER('Czech Republic')),
  (UPPER('Denmark')),
  (UPPER('Djibouti')),
  (UPPER('Dominica')),
  (UPPER('Dominican Republic')),
  (UPPER('Ecuador')),
  (UPPER('Egypt')),
  (UPPER('El Salvador')),
  (UPPER('Equatorial Guinea')),
  (UPPER('Eritrea')),
  (UPPER('Estonia')),
  (UPPER('Ethiopia')),
  (UPPER('Falkland Islands')),
  (UPPER('Faroe Islands')),
  (UPPER('Fiji')),
  (UPPER('Finland')),
  (UPPER('France')),
  (UPPER('French Guiana')),
  (UPPER('Gabon')),
  (UPPER('Gambia')),
  (UPPER('Georgia')),
  (UPPER('Germany')),
  (UPPER('Ghana')),
  (UPPER('Gibraltar')),
  (UPPER('Greece')),
  (UPPER('Greenland')),
  (UPPER('Grenada')),
  (UPPER('Guadeloupe (French)')),
  (UPPER('Guam (USA)')),
  (UPPER('Guatemala')),
  (UPPER('Guinea')),
  (UPPER('Guinea Bissau')),
  (UPPER('Guyana')),
  (UPPER('Haiti')),
  (UPPER('Holy See')),
  (UPPER('Honduras')),
  (UPPER('Hong Kong')),
  (UPPER('Hungary')),
  (UPPER('Iceland')),
  (UPPER('India')),
  (UPPER('Indonesia')),
  (UPPER('Iran')),
  (UPPER('Iraq')),
  (UPPER('Ireland')),
  (UPPER('Israel')),
  (UPPER('Italy')),
  (UPPER('Ivory Coast (Cote D`Ivoire)')),
  (UPPER('Jamaica')),
  (UPPER('Japan')),
  (UPPER('Jordan')),
  (UPPER('Kazakhstan')),
  (UPPER('Kenya')),
  (UPPER('Kiribati')),
  (UPPER('Kuwait')),
  (UPPER('Kyrgyzstan')),
  (UPPER('Laos')),
  (UPPER('Latvia')),
  (UPPER('Lebanon')),
  (UPPER('Lesotho')),
  (UPPER('Liberia')),
  (UPPER('Libya')),
  (UPPER('Liechtenstein')),
  (UPPER('Lithuania')),
  (UPPER('Luxembourg')),
  (UPPER('Macau')),
  (UPPER('Macedonia')),
  (UPPER('Madagascar')),
  (UPPER('Malawi')),
  (UPPER('Malaysia')),
  (UPPER('Maldives')),
  (UPPER('Mali')),
  (UPPER('Malta')),
  (UPPER('Marshall Islands')),
  (UPPER('Martinique (French)')),
  (UPPER('Mauritania')),
  (UPPER('Mauritius')),
  (UPPER('Mayotte')),
  (UPPER('Mexico')),
  (UPPER('Micronesia')),
  (UPPER('Moldova')),
  (UPPER('Monaco')),
  (UPPER('Mongolia')),
  (UPPER('Montenegro')),
  (UPPER('Montserrat')),
  (UPPER('Morocco')),
  (UPPER('Mozambique')),
  (UPPER('Myanmar')),
  (UPPER('Namibia')),
  (UPPER('Nauru')),
  (UPPER('Nepal')),
  (UPPER('Netherlands')),
  (UPPER('Netherlands Antilles')),
  (UPPER('New Caledonia (French)')),
--  (UPPER('New Zealand')),
  (UPPER('Nicaragua')),
  (UPPER('Niger')),
  (UPPER('Nigeria')),
  (UPPER('Niue')),
  (UPPER('Norfolk Island')),
  (UPPER('North Korea')),
  (UPPER('Northern Mariana Islands')),
  (UPPER('Norway')),
  (UPPER('Oman')),
  (UPPER('Pakistan')),
  (UPPER('Palau')),
  (UPPER('Panama')),
  (UPPER('Papua New Guinea')),
  (UPPER('Paraguay')),
  (UPPER('Peru')),
  (UPPER('Philippines')),
  (UPPER('Pitcairn Island')),
  (UPPER('Poland')),
  (UPPER('Polynesia (French)')),
  (UPPER('Portugal')),
  (UPPER('Puerto Rico')),
  (UPPER('Qatar')),
  (UPPER('Reunion')),
  (UPPER('Romania')),
  (UPPER('Russia')),
  (UPPER('Rwanda')),
  (UPPER('Saint Helena')),
  (UPPER('Saint Kitts and Nevis')),
  (UPPER('Saint Lucia')),
  (UPPER('Saint Pierre and Miquelon')),
  (UPPER('Saint Vincent and Grenadines')),
  (UPPER('Samoa')),
  (UPPER('San Marino')),
  (UPPER('Sao Tome and Principe')),
  (UPPER('Saudi Arabia')),
  (UPPER('Senegal')),
  (UPPER('Serbia')),
  (UPPER('Seychelles')),
  (UPPER('Sierra Leone')),
  (UPPER('Singapore')),
  (UPPER('Slovakia')),
  (UPPER('Slovenia')),
  (UPPER('Solomon Islands')),
  (UPPER('Somalia')),
  (UPPER('South Africa')),
  (UPPER('South Georgia and South Sandwich Islands')),
  (UPPER('South Korea')),
  (UPPER('Spain')),
  (UPPER('Sri Lanka')),
  (UPPER('Sudan')),
  (UPPER('Suriname')),
  (UPPER('Svalbard and Jan Mayen Islands')),
  (UPPER('Swaziland')),
  (UPPER('Sweden')),
  (UPPER('Switzerland')),
  (UPPER('Syria')),
  (UPPER('Taiwan')),
  (UPPER('Tajikistan')),
  (UPPER('Tanzania')),
  (UPPER('Thailand')),
  (UPPER('Timor-Leste (East Timor)')),
  (UPPER('Togo')),
  (UPPER('Tokelau')),
  (UPPER('Tonga')),
  (UPPER('Trinidad and Tobago')),
  (UPPER('Tunisia')),
  (UPPER('Turkey')),
  (UPPER('Turkmenistan')),
  (UPPER('Turks and Caicos Islands')),
  (UPPER('Tuvalu')),
  (UPPER('Uganda')),
  (UPPER('Ukraine')),
  (UPPER('United Arab Emirates')),
--  (UPPER('United Kingdom')),
--  (UPPER('United States')),
  (UPPER('Uruguay')),
  (UPPER('Uzbekistan')),
  (UPPER('Vanuatu')),
  (UPPER('Venezuela')),
  (UPPER('Vietnam')),
  (UPPER('Virgin Islands')),
  (UPPER('Wallis and Futuna Islands')),
  (UPPER('Yemen')),
  (UPPER('Zambia')),
  (UPPER('Zimbabwe'));

-------------------------------------------
SELECT   *
FROM     Country;
-------------------------------------------
*/

/* Suburb

CREATE TABLE Suburb
(
 suburb_id             int            not null PRIMARY KEY identity,
 name                  varchar(50)    not null,
 postcode              varchar(8)     not null,
 state                 varchar(4)     not null,
 amended_date          datetime                 DEFAULT null,
 amended_by            int                      FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT null,
 previous              varchar(100)   not null,
);

*/

/* AddressChannelType

CREATE TABLE AddressChannelType
(
 address_channel_type_id    int      not null PRIMARY KEY identity,
 descr                      varchar(50)    not null
);

*/

/* AddressChannel  (ref: AddressChannelType)

CREATE TABLE AddressChannel
(
 address_channel_id       int           not null PRIMARY KEY identity,
 descr                    varchar(100)  not null,
 address_channel_type_id  int           not null FOREIGN KEY REFERENCES AddressChannelType(address_channel_type_id),
 address_channel_date_added    datetime DEFAULT (GETDATE()),
 address_channel_date_modified datetime DEFAULT NULL
);

*/

/* ContactTypeGroup

CREATE TABLE ContactTypeGroup
(
 contact_type_group_id    int      not null PRIMARY KEY identity,
 descr                   varchar(50)    not null
);

SET IDENTITY_INSERT ContactTypeGroup ON;
INSERT ContactTypeGroup
  (contact_type_group_id,descr)
VALUES
  (1,'Mailing'),
  (2,'Telecoms'),
  (3,'Facility'),
  (4,'Internet');
 SET IDENTITY_INSERT ContactTypeGroup OFF;

-------------------------------------------
SELECT   *
FROM     ContactTypeGroup;
-------------------------------------------
*/

/* ContactType  (ref: ContactTypeGroup)

CREATE TABLE ContactType
(
 contact_type_id         int          not null PRIMARY KEY identity,
 contact_type_group_id   int          not null FOREIGN KEY REFERENCES ContactTypeGroup(contact_type_group_id),
 display_order           int          not null,
 descr                   varchar(50)  not null,
);

SET IDENTITY_INSERT ContactType ON;
INSERT ContactType
  (contact_type_id,contact_type_group_id, display_order, descr)
VALUES

  (35, 1,1,'Home address'),
  (36, 1,2,'Business address'),
  (37, 1,3,'PO Box'),
  (38, 1,4,'Private Box'),
  (39, 1,5,'Document exchange'),
  (262,1,6,'GPO Box'),

  (29,2,16,'Fax home'),
  (30,2,10,'Mobile'),
  (31,2,17,'Office Fax'),
  (32,2,12,'Pager'),
  (33,2,11,'Home Phone'),
  (34,2,13,'Office Phone'),
  (42,2,14,'Toll free phone'),
  (43,2,15,'Toll free fax'),

  (166,3,20,'Bedroom'),

  (27, 4,18,'E-mail'),
  (28, 4,18,'WWW');

 SET IDENTITY_INSERT ContactType OFF;

-------------------------------------------
SELECT   *
FROM     ContactType;
-------------------------------------------
*/

/* Contact   (ref: Entity, ContactType, AddressChannel, Suburb, Country, Site)

CREATE TABLE Contact
(
 contact_id            int           not null PRIMARY KEY identity,
 entity_id             int           not null FOREIGN KEY REFERENCES Entity(entity_id),

 contact_type_id       int           not null FOREIGN KEY REFERENCES ContactType(contact_type_id),
 
 free_text             varchar(100)  not null,                            -- such as (sisters phone nbr)
 addr_line1            varchar(100)  not null,                            -- contains phone number or website for those types
 addr_line2            varchar(100)  not null,
 address_channel_id    int                    FOREIGN KEY REFERENCES AddressChannel(address_channel_id),
 suburb_id             int                    FOREIGN KEY REFERENCES Suburb(suburb_id),
 country_id            int                    FOREIGN KEY REFERENCES Country(country_id),

 site_id               int                    FOREIGN KEY REFERENCES Site(site_id),

 is_billing            bit           not null,
 is_non_billing        bit           not null,
 contact_date_added    datetime      not null DEFAULT (GETDATE()),
 contact_date_modified datetime                    DEFAULT NULL,
 contact_date_deleted  datetime                    DEFAULT NULL,
);

*/

/* ContactAus   (ref: ContactType, AddressChannelType, Suburb, Country, Site)

CREATE TABLE ContactAus
(
 contact_id               int           not null PRIMARY KEY identity,
 entity_id                int           not null FOREIGN KEY REFERENCES Entity(entity_id),

 contact_type_id          int           not null FOREIGN KEY REFERENCES ContactType(contact_type_id),
 
 free_text                varchar(100)  not null,                            -- such as (sisters phone nbr)
 addr_line1               varchar(100)  not null,                            -- contains phone number or website for those types
 addr_line2               varchar(100)  not null,
 street_name              varchar(100)  not null, 
 address_channel_type_id  int                    FOREIGN KEY REFERENCES AddressChannelType(address_channel_type_id),
 suburb_id                int                    FOREIGN KEY REFERENCES Suburb(suburb_id),
 country_id               int                    FOREIGN KEY REFERENCES Country(country_id),

 site_id                  int                    FOREIGN KEY REFERENCES Site(site_id),

 is_billing               bit           not null,
 is_non_billing           bit           not null,
 contact_date_added       datetime      not null DEFAULT (GETDATE()),
 contact_date_modified    datetime                    DEFAULT NULL,
 contact_date_deleted     datetime                    DEFAULT NULL,
);

*/

/* uspInsertContact_Person

--------------------------------------

-- DROP PROCEDURE uspInsertContact_Person;

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

--------------------------------------

 */

/* uspInsertContact_Other

--------------------------------------

DROP PROCEDURE uspInsertContact_Other;

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

--------------------------------------

 */

/* ADD THIS IN! -- ufnFilterNonDigit, ufnFilterNonAlphaNumeric

 -- select [dbo].[ufnFilterNonDigit]('123-456-7890') will return 1234567890

CREATE FUNCTION [dbo].[ufnFilterNonDigit]( @Input varchar(256))
RETURNS varchar(256)
AS
BEGIN
    If PATINDEX('%[^0-9]%', @Input) > 0
        WHILE PATINDEX('%[^0-9]%', @Input) > 0
            SET @Input = Stuff(@Input, PATINDEX('%[^0-9]%', @Input), 1, '')
    RETURN @Input
END



CREATE FUNCTION [dbo].[ufnFilterNonAlphaNumeric]( @Input varchar(256))
RETURNS varchar(256)
AS
BEGIN
    If PATINDEX('%[^0-9a-zA-Z]%', @Input) > 0
        WHILE PATINDEX('%[^0-9a-zA-Z]%', @Input) > 0
            SET @Input = Stuff(@Input, PATINDEX('%[^0-9a-zA-Z]%', @Input), 1, '')
    RETURN @Input
END

*/




/*  Field

CREATE TABLE Field
(
 field_id      int         not null PRIMARY KEY identity,
 descr         varchar(50) not null,
 has_offerings bit         not null,
);

SET IDENTITY_INSERT Field ON;
INSERT Field
   (field_id,descr, has_offerings)
VALUES
   (0  ,'None'                , 1),
   (63 ,'Management'          , 0),
   (64 ,'Strategic Manager'   , 0),
   (65 ,'Operational Manager' , 0),
   (66 ,'Clerical Staff'      , 0),
   (67 ,'Service Provider'    , 0),
   (68 ,'Podiatrist'          , 1),
   (134,'Software DEVELOPER'  , 0),
   (155,'Tactical Manager'    , 0),
   (277,'Physiotherapist'     , 1),
   (312,'Myotherapist'        , 0);

SET IDENTITY_INSERT Field OFF;
-------------------------------------------
SELECT  field_id, descr, has_offerings
FROM    Field;
-------------------------------------------

*/

/* StaffPosition

- position is for resumes .. not used
- role is for when already in the company
- position not used anymore as only exist in the system when they already work there
- so currently this is hidden, but probably can delete it...


CREATE TABLE StaffPosition
(
 staff_position_id    int  not null PRIMARY KEY identity,
 descr varchar(50) not null
);

*/

/* CostCentre

CREATE TABLE CostCentre
(
 costcentre_id  int         not null PRIMARY KEY identity,
 descr          varchar(50) not null,
 parent_id      int                  FOREIGN KEY REFERENCES CostCentre(costcentre_id)
);

*/

/* Staff  (ref: Person, StaffPosition, Field, CostCentre)

 - stakeholder  - no customer of this system has access  -- make the support logins be stakeholders
 - master-admin - can see all of admin plus a bit more 
 - admin        - receptionist - can see all patients, staff info, etc
 - principle    - has open acces to all patient notes and clinics - other pod's dont
 - provider     - can be seen on booking sheet to make bookings for them, else not on booking sheet

CREATE TABLE Staff
(
 staff_id                    int           not null PRIMARY KEY identity,
 person_id                   int           not null FOREIGN KEY REFERENCES Person(person_id),
 login                       varchar(50)   not null,
 pwd                         varchar(50)   not null,
 staff_position_id           int           not null FOREIGN KEY REFERENCES StaffPosition(staff_position_id),
 field_id                    int           not null FOREIGN KEY REFERENCES Field(field_id),
 costcentre_id               int           not null FOREIGN KEY REFERENCES CostCentre(costcentre_id),
 is_contractor               bit           not null,
 tfn                         varchar(50)   not null,
 provider_number             varchar(50)   not null,   --  for aged care only
 is_fired                    bit           not null,
 is_commission               bit           not null,           --  for payroll
 commission_percent          decimal(5,2)  not null,  --  for payroll
 is_stakeholder              bit           not null,
 is_master_admin             bit           not null,   --  can see a couple of extra things that admins cant see
 is_admin                    bit           not null,
 is_principal                bit           not null,   --  if can see all patients (but indicate dont belong to them) .. else view only own patients 
 is_provider                 bit           not null,   --  only shows booking sheet if someone is set as provider (& allocated to that org) -- or if they had bookings previously entered
 is_external                 bit           not null,

 staff_date_added            datetime      not null DEFAULT (GETDATE()),  -- the date the  staff file was added, so diff to person date_added
 start_date                  datetime               DEFAULT null,         -- the date they become a staff, so diff to person created date
 end_date                    datetime               DEFAULT null,
 comment                     varchar(max)  not null,

 num_days_to_display_on_booking_screen int not null DEFAULT 3,
 show_header_on_booking_screen bit         not null DEFAULT 1,
 bk_screen_field_id          int                    FOREIGN KEY REFERENCES Field(field_id) DEFAULT NULL,
 bk_screen_show_key          bit           not null DEFAULT 1,

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


*/

/* uspInsertStaff

--------------------------------------

-- DROP PROCEDURE uspInsertStaff;

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
    ,@is_provider        int
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
    ,@is_fired
    ,@is_commission
    ,@commission_percent
    ,@is_stakeholder
    ,@is_principal
    ,@is_admin
    ,0
    ,@is_provider
    ,0
    ,@staff_date_added
    ,@start_date
    ,@end_date
    ,@comment
    )
 
    SET IDENTITY_INSERT Staff OFF


COMMIT TRAN

--------------------------------------

*/

/* UserLogin (ref: Staff, Site)

CREATE TABLE UserLogin
(
 userlogin_id     int     not null PRIMARY KEY identity,
 staff_id         int     FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT null,
 patient_id       int     FOREIGN KEY REFERENCES Patient(patient_id) DEFAULT null,
 username         varchar(50)  not null,    -- so can see attempted login if no username exists
 site_id          int     FOREIGN KEY REFERENCES Site(site_id),
 is_successful    bit           not null,
 session_id       varchar(100)  not null,
 login_time       datetime      not null DEFAULT (GETDATE()),
 last_access_time datetime      not null DEFAULT (GETDATE()),
 last_access_page varchar(max)  not null DEFAULT '',
 is_logged_off    bit           not null DEFAULT 0,
 ipaddress        varchar(50)  not null
);

*/

/* StaffSiteRestriction  (ref: Staff, Site)

CREATE TABLE StaffSiteRestriction
(
 staff_site_restriction_id  int  not null PRIMARY KEY identity,
 staff_id                   int  not null FOREIGN KEY REFERENCES Staff(staff_id),
 site_id                    int  not null FOREIGN KEY REFERENCES Site(site_id),
 CONSTRAINT uc_unique_staff_site UNIQUE (staff_id,site_id)
);

*/




/* :: facilityclinic diagram and groupfacility explination
  
fac  type 218

facilityclinic
        \ 
       registration - contract
        / 
    staff

-----------------------------------------------------

*/

/* OrganisationCustomerType

CREATE TABLE OrganisationCustomerType
(
 organisation_customer_type_id  int          not null PRIMARY KEY identity,
 descr                          varchar(50) not null,
);

*/

/* OrganisationTypeGroup

CREATE TABLE OrganisationTypeGroup
(
 organisation_type_group_id    int         not null PRIMARY KEY identity,
 descr                         varchar(50) not null,
);

SET IDENTITY_INSERT OrganisationTypeGroup ON;
INSERT OrganisationTypeGroup
   (organisation_type_group_id,descr)
VALUES
   (1, 'Group Organisation - Govt'),
   (2, 'Group Organisation - Other'),
   (3, 'Corporation - Other'),
   (4, 'External'),
   (5, 'Clinic'),
   (6, 'Aged Care');
SET IDENTITY_INSERT OrganisationTypeGroup OFF;

-------------------------------------------
SELECT  *
FROM    OrganisationTypeGroup;
-------------------------------------------

*/

/* OrganisationType  (ref: OrganisationTypeGroup)

CREATE TABLE OrganisationType
(
 organisation_type_id        int          not null PRIMARY KEY identity,
 descr                       varchar(50)  not null,
 organisation_type_group_id  int          not null FOREIGN KEY REFERENCES OrganisationTypeGroup(organisation_type_group_id),
 display_order               int          not null,
);

SET IDENTITY_INSERT OrganisationType ON;
INSERT OrganisationType
   (organisation_type_id,descr,organisation_type_group_id,display_order)
VALUES
   (1, 'Medicare',1,11),
   (2, 'Dept Veterans Affairs',1,12),
   (98,'Health Related Organisation',4,13),
   (99,'Medical Clinic',4,14),
   (100,'Laboratory',4,15),
   (147,'Sole Trader',4,16),
   (148,'Small or Medium Sized Enterprise',4,17),
   (152,'Advertising Agency',4,18),
   (191,'Medical Practice',4,19),
   (218,'Clinic',5,1),
   (139,'Aged Care Facility',6,2),
   (367,'Aged Care Wing',6,3),
   (372,'Aged Care Unit',6,4),

   (141,'Group Organisation',2,8),
   (149,'Corporation',3,7);

SET IDENTITY_INSERT OrganisationType OFF;

-------------------------------------------
SELECT  *
FROM    OrganisationType;
-------------------------------------------

*/

/* Organisation (ref: Entity, Organisation, OrganisationType, OrganisationCustomerType)

-------------------------------
Clinics
  is_debtor   - false
  is_creditor - false

Aged care facility
  is_debtor   - true
  is_creditor - false

Aged care wards + units
  is_debtor   - false
  is_creditor - false

Medicare, DVA and TAG
  is_debtor   - true (as they do get invoiced)

All others
  is_debtor   - false
  is_creditor - true

Rule of thumb
  If an invoice can be sent to the Legal Entity (person or organisation) then the isadebtor is set to true else false
  If the Legal Entity is able to bill Mediclinic then isacreditor is set to true, else false

Once set they are never changed
-------------------------------


-- DROP TABLE Organisation;

CREATE TABLE Organisation
(
 organisation_id               int          not null PRIMARY KEY identity,
 entity_id                     int          not null FOREIGN KEY REFERENCES Entity(entity_id) UNIQUE,
 parent_organisation_id        int                   FOREIGN KEY REFERENCES Organisation(organisation_id) DEFAULT null,
 use_parent_offernig_prices    bit          not null,
 organisation_type_id          int          not null FOREIGN KEY REFERENCES OrganisationType(organisation_type_id),
 organisation_customer_type_id int          not null FOREIGN KEY REFERENCES OrganisationCustomerType(organisation_customer_type_id),
 name                          varchar(100) not null,
 acn                           varchar(50)  not null,
 abn                           varchar(50)  not null,
 organisation_date_added       datetime     not null DEFAULT (GETDATE()),
 organisation_date_modified    datetime              DEFAULT null,
 is_debtor                     bit          not null,
 is_creditor                   bit          not null,
 bpay_account                  varchar(50)  not null,
 is_deleted                    bit          not null DEFAULT 0,           -- if logically deleted from system but still exists in system

 weeks_per_service_cycle int    not null,
 start_date                    datetime              DEFAULT null,   -- was:   start_date        datetime     not null DEFAULT (GETDATE()),
 end_date                      datetime              DEFAULT null,
 comment                       varchar(max) not null,
 free_services                 int          not null,
 excl_sun                      bit          not null,
 excl_mon                      bit          not null,
 excl_tue                      bit          not null,
 excl_wed                      bit          not null,
 excl_thu                      bit          not null,
 excl_fri                      bit          not null,
 excl_sat                      bit          not null,
 sun_start_time                time         not null,
 sun_end_time                  time         not null,
 mon_start_time                time         not null,
 mon_end_time                  time         not null,
 tue_start_time                time         not null,
 tue_end_time                  time         not null,
 wed_start_time                time         not null,
 wed_end_time                  time         not null,
 thu_start_time                time         not null,
 thu_end_time                  time         not null,
 fri_start_time                time         not null,
 fri_end_time                  time         not null,
 sat_start_time                time         not null,
 sat_end_time                  time         not null,

 sun_lunch_start_time          time         not null,
 sun_lunch_end_time            time         not null,
 mon_lunch_start_time          time         not null,
 mon_lunch_end_time            time         not null,
 tue_lunch_start_time          time         not null,
 tue_lunch_end_time            time         not null,
 wed_lunch_start_time          time         not null,
 wed_lunch_end_time            time         not null,
 thu_lunch_start_time          time         not null,
 thu_lunch_end_time            time         not null,
 fri_lunch_start_time          time         not null,
 fri_lunch_end_time            time         not null,
 sat_lunch_start_time          time         not null,
 sat_lunch_end_time            time         not null,

 last_batch_run                datetime               DEFAULT null,
);

*/

/* uspInsertOrganisation

--------------------------------------

DROP PROCEDURE uspInsertOrganisation;

CREATE PROCEDURE uspInsertOrganisation 

     @organisation_id               int
    ,@parent_organisation_id        int
    ,@organisation_type_id          int
    ,@organisation_customer_type_id int
    ,@name                          varchar(100)
    ,@acn                           varchar(50)
    ,@abn                           varchar(50)
    ,@organisation_date_added       datetime
    ,@organisation_date_modified    datetime
    ,@is_debtor                     bit
    ,@is_creditor                   bit
    ,@bpay_account                  varchar(50)
    ,@is_deleted                    bit

    ,@weeks_per_service_cycle       int
    ,@start_date                    datetime
    ,@end_date                      datetime
    ,@comment                       varchar(max)
    ,@free_services                 int
    ,@excl_sun                      bit
    ,@excl_mon                      bit
    ,@excl_tue                      bit
    ,@excl_wed                      bit
    ,@excl_thu                      bit
    ,@excl_fri                      bit
    ,@excl_sat                      bit
    ,@sun_start_time                time
    ,@sun_end_time                  time
    ,@mon_start_time                time
    ,@mon_end_time                  time
    ,@tue_start_time                time
    ,@tue_end_time                  time
    ,@wed_start_time                time
    ,@wed_end_time                  time
    ,@thu_start_time                time
    ,@thu_end_time                  time
    ,@fri_start_time                time
    ,@fri_end_time                  time
    ,@sat_start_time                time
    ,@sat_end_time                  time
    ,@sun_lunch_start_time          time
    ,@sun_lunch_end_time            time
    ,@mon_lunch_start_time          time
    ,@mon_lunch_end_time            time
    ,@tue_lunch_start_time          time
    ,@tue_lunch_end_time            time
    ,@wed_lunch_start_time          time
    ,@wed_lunch_end_time            time
    ,@thu_lunch_start_time          time
    ,@thu_lunch_end_time            time
    ,@fri_lunch_start_time          time
    ,@fri_lunch_end_time            time
    ,@sat_lunch_start_time          time
    ,@sat_lunch_end_time            time
    ,@last_batch_run                datetime

AS

Declare @entity_id int

BEGIN TRAN


    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()


    SET IDENTITY_INSERT Organisation ON

    INSERT INTO Organisation (organisation_id,entity_id,parent_organisation_id,use_parent_offernig_prices,organisation_type_id,organisation_customer_type_id,
                              name,acn,abn,organisation_date_added,organisation_date_modified,is_debtor,is_creditor,bpay_account,is_deleted,

                              weeks_per_service_cycle,start_date,end_date,comment,free_services,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,
                              excl_fri,excl_sat,
                              sun_start_time,sun_end_time,mon_start_time,mon_end_time,tue_start_time,tue_end_time,wed_start_time,
                              wed_end_time,thu_start_time,thu_end_time,fri_start_time,fri_end_time,sat_start_time,sat_end_time,
                              sun_lunch_start_time,sun_lunch_end_time,mon_lunch_start_time,mon_lunch_end_time,tue_lunch_start_time,tue_lunch_end_time,wed_lunch_start_time,
                              wed_lunch_end_time,thu_lunch_start_time,thu_lunch_end_time,fri_lunch_start_time,fri_lunch_end_time,sat_lunch_start_time,sat_lunch_end_time,
                              last_batch_run)
    VALUES
    (
     @organisation_id
    ,@entity_id
    ,@parent_organisation_id
    ,0
    ,@organisation_type_id
    ,@organisation_customer_type_id
    ,@name
    ,@acn
    ,@abn
    ,@organisation_date_added
    ,@organisation_date_modified
    ,@is_debtor
    ,@is_creditor
    ,@bpay_account
    ,@is_deleted

    ,@weeks_per_service_cycle
    ,@start_date
    ,@end_date
    ,@comment
    ,@free_services
    ,@excl_sun
    ,@excl_mon
    ,@excl_tue
    ,@excl_wed
    ,@excl_thu
    ,@excl_fri
    ,@excl_sat
    ,@sun_start_time
    ,@sun_end_time
    ,@mon_start_time
    ,@mon_end_time
    ,@tue_start_time
    ,@tue_end_time
    ,@wed_start_time
    ,@wed_end_time
    ,@thu_start_time
    ,@thu_end_time
    ,@fri_start_time
    ,@fri_end_time
    ,@sat_start_time
    ,@sat_end_time
    ,@sun_lunch_start_time
    ,@sun_lunch_end_time
    ,@mon_lunch_start_time
    ,@mon_lunch_end_time
    ,@tue_lunch_start_time
    ,@tue_lunch_end_time
    ,@wed_lunch_start_time
    ,@wed_lunch_end_time
    ,@thu_lunch_start_time
    ,@thu_lunch_end_time
    ,@fri_lunch_start_time
    ,@fri_lunch_end_time
    ,@sat_lunch_start_time
    ,@sat_lunch_end_time
    ,@last_batch_run
    )
 
    SET IDENTITY_INSERT Organisation OFF


COMMIT TRAN

--------------------------------------

 */


/* RegisterStaff  (ref: Organisation, Staff)

CREATE TABLE RegisterStaff
(
 register_staff_id          int          not null PRIMARY KEY identity,
 register_staff_date_added  datetime     not null DEFAULT (GETDATE()),
 organisation_id            int          FOREIGN KEY REFERENCES Organisation(organisation_id) DEFAULT null,
 staff_id                   int          FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT null,
 provider_number            varchar(50) not null,
 main_provider_for_clinic   bit not null,
 excl_sun                   bit not null DEFAULT 0,
 excl_mon                   bit not null DEFAULT 0,
 excl_tue                   bit not null DEFAULT 0,
 excl_wed                   bit not null DEFAULT 0,
 excl_thu                   bit not null DEFAULT 0,
 excl_fri                   bit not null DEFAULT 0,
 excl_sat                   bit not null DEFAULT 0,
 is_deleted                 bit not null DEFAULT 0,
);

*/




/* Patient  (ref: Person, Offering)  [SEE DB_Notes_Mediclinic.txt]


Knowing if a patient is a clinic patient, aged care patient, or both
--------------------------------------------------------------------

NB. someone can be both ac resident AND clinic patient
NB. can not just get patient.isadebtor=0 to get ac residents because then will miss out on ac residents who are also patients


If clinic patient : isadebtor=1  (changed to "is_clinic_patient" !)
- when adding clinic patient, set to true
- when adding registration of existing person to org type clinic (type 218), set to true
- when adding existing person to clinic (eg provider view will auto add to that org which is a clinic), set to true
- to get all clinic patients, get isadebtor=1


If ac resident: isadebtor can be 0 or 1 
- default to 0 for new person (else dont touch it as it is whether someone is a clinic patient)
- to get all ac residents list (and the wing they are in):
      join patient to 
      contact table (links patient to facilitid (wing=bedroom))
      this is where isadebtor is set for aged care resident (contact tbl, rows type 308)
- whether an ac reident is a "debtor" is found in contact table
  contact.is_debtor set
    lc/hc-unfunded = true
    otherwise = false or not set



CREATE TABLE Patient
(
 patient_id                        int           not null PRIMARY KEY identity,
 person_id                         int           not null FOREIGN KEY REFERENCES Person(person_id),
 patient_date_added                datetime      not null DEFAULT (GETDATE()),
 is_clinic_patient                 bit           not null,                      -- "is_debtor" used in BEST to select only fac patients = true ... might be able to delete later... ??
 is_gp_patient                     bit           not null, 
 is_deleted                        bit           not null DEFAULT 0,
 is_deceased                       bit           not null,
 flashing_text                     varchar(max)  not null,
 flashing_text_added_by            int                     FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT null,
 flashing_text_last_modified_date  datetime                DEFAULT null,
 private_health_fund               varchar(100)  not null,
 concession_card_number            varchar(100)  not null,
 concession_card_expiry_date       datetime,
 is_diabetic                       bit           not null,
 is_member_diabetes_australia      bit           not null,
 diabetic_assessment_review_date   datetime               DEFAULT null,
 ac_inv_offering_id                int                    FOREIGN KEY REFERENCES Offering(offering_id) DEFAULT null,
 ac_pat_offering_id                int                    FOREIGN KEY REFERENCES Offering(offering_id) DEFAULT null,
 login                             varchar(50)   not null DEFAULT '',
 pwd                               varchar(50)   not null DEFAULT '',
 is_company                        bit           not null, 
 abn                               varchar(50)   not null,
 next_of_kin_name                  varchar(100)  not null DEFAULT ''
 next_of_kin_relation              varchar(100)  not null DEFAULT ''
 next_of_kin_contact_info          varchar(2000) not null DEFAULT ''  -- multi-line to enter multiple numbers and/or addresses
);

*/

/* uspInsertPatient

--------------------------------------

DROP PROCEDURE uspInsertPatient;

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

--------------------------------------

 */

/* uspInsertPatientV2

--------------------------------------

DROP PROCEDURE uspInsertPatientV2;

CREATE PROCEDURE uspInsertPatientV2

     @staff_id             int

    ,@added_by             int
    ,@title_id             int
    ,@firstname            varchar(100)
    ,@middlename           varchar(100)
    ,@surname              varchar(100)
    ,@nickname             varchar(100)
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

    INSERT INTO Patient (patient_id,person_id, patient_date_added, is_clinic_patient, is_deleted, is_deceased, flashing_text, flashing_text_added_by, flashing_text_last_modified_date, private_health_fund, concession_card_number,concession_card_expiry_date,is_diabetic,is_member_diabetes_australia,diabetic_assessment_review_date,
                         ac_inv_offering_id, ac_pat_offering_id, login, pwd, is_gp_patient) 
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

    ,NULL
    ,NULL
    ,''
    ,''
    ,0

    )
 
    SET IDENTITY_INSERT Patient OFF

COMMIT TRAN

--------------------------------------

 */

/* uspInsertPatientV3

--------------------------------------

--DROP PROCEDURE uspInsertPatientV3;

CREATE PROCEDURE uspInsertPatientV3

     @staff_id             int

    ,@added_by             int
    ,@title_id             int
    ,@firstname            varchar(100)
    ,@middlename           varchar(100)
    ,@surname              varchar(100)
    ,@nickname             varchar(100)
    ,@gender               varchar(1)
    ,@dob                  datetime
    ,@person_date_added    datetime
    ,@person_date_modified datetime

    ,@patient_id         int
    ,@patient_date_added datetime
    ,@is_clinic_patient  bit
    ,@is_deleted         bit
    ,@is_deceased        bit



    ,@ac_type_id              int
    ,@ac_org_id               int
    ,@ac_org_name             varchar(500)
    ,@ac_org_wingid           int
    ,@ac_org_wingname         varchar(500)
    ,@ac_org_room             varchar(500)

    ,@site_id                 int

    ,@phone                   varchar(100)
    ,@phone_contact_type_id   int
						
    ,@addr_contact_type_id    int
    ,@addr1                   varchar(100)
    ,@addr2                   varchar(100)
    ,@street                  varchar(100)
    ,@address_channel_type_id int
						
    ,@suburb                  varchar(100)
    ,@state                   varchar(100)
    ,@postcode                varchar(100)

    ,@flashing_text           varchar(1000)

AS

Declare @entity_id int
Declare @person_id int

Declare @suburb_id int

Declare @today     datetime
Declare @parent_organisation_id   int

BEGIN TRAN

	SET @today = GETDATE()

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



    IF (@patient_id <> 0)
    BEGIN

        SET IDENTITY_INSERT Patient ON

        INSERT INTO Patient (patient_id,person_id, patient_date_added, is_clinic_patient, is_deleted, is_deceased, flashing_text, flashing_text_added_by, flashing_text_last_modified_date, private_health_fund, concession_card_number,concession_card_expiry_date,is_diabetic,is_member_diabetes_australia,diabetic_assessment_review_date,
                             ac_inv_offering_id, ac_pat_offering_id, login, pwd, is_gp_patient) 
        VALUES
        (
         @patient_id
        ,@person_id
        ,@patient_date_added
        ,@is_clinic_patient
        ,@is_deleted
        ,@is_deceased
        ,@flashing_text
        ,NULL
        ,NULL
        ,''
        ,''
        ,null
        ,0
        ,0
        ,NULL

        ,@ac_type_id
        ,@ac_type_id
        ,''
        ,''
        ,0

        )
 
        SET IDENTITY_INSERT Patient OFF

    END
    ELSE
    BEGIN

        INSERT INTO Patient (person_id, patient_date_added, is_clinic_patient, is_deleted, is_deceased, flashing_text, flashing_text_added_by, flashing_text_last_modified_date, private_health_fund, concession_card_number,concession_card_expiry_date,is_diabetic,is_member_diabetes_australia,diabetic_assessment_review_date,
                             ac_inv_offering_id, ac_pat_offering_id, login, pwd, is_gp_patient) 
        VALUES
        (
         @person_id
        ,@patient_date_added
        ,@is_clinic_patient
        ,@is_deleted
        ,@is_deceased
        ,@flashing_text
        ,NULL
        ,NULL
        ,''
        ,''
        ,null
        ,0
        ,0
        ,NULL

        ,@ac_type_id
        ,@ac_type_id
        ,''
        ,''
        ,0

        )

        SET @patient_id = SCOPE_IDENTITY()

    END



	IF (@phone <> '' AND LEN(@phone) > 0)
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				@phone_contact_type_id,
				'',  -- free text
				@phone,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				@site_id,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END


	IF (@addr1 <> '' OR @addr2 <> '' OR @street <> '' OR @suburb <> '' OR @suburb <> '' OR @postcode <> '' OR @state <> '' )
	BEGIN

     	SET @suburb_id = (SELECT suburb_id FROM Suburb WHERE name = @suburb AND postcode = @postcode AND state = @state)

        IF (@suburb_id IS NULL AND LEN(@postcode) = 0 AND (SELECT COUNT(*) FROM Suburb WHERE  name = @suburb AND state = @state) = 1)
        BEGIN
        	SET @suburb_id = (SELECT TOP 1 suburb_id FROM Suburb WHERE name = @suburb AND state = @state)
        END


		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				@addr_contact_type_id,
				'',  -- free text
				@addr1,
				@addr2,
				@street,  -- street_name
				@address_channel_type_id, -- address_channel_type_id
				@suburb_id,
				NULL, -- country_id,
				@site_id,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END


    SET @ac_org_name     = LTRIM(RTRIM(@ac_org_name))
    SET @ac_org_wingname = LTRIM(RTRIM(@ac_org_wingname))
    SET @ac_org_room     = LTRIM(RTRIM(@ac_org_room))


    IF @ac_org_id = 0 AND LEN(@ac_org_name) > 0
    BEGIN

        SET @ac_org_id = (SELECT TOP 1 organisation_id FROM Organisation WHERE name = @ac_org_name AND organisation_type_id in (139,367,372) AND is_deleted = 0)

        IF (@ac_org_id IS NULL)
        BEGIN

                -- get new org id (as proceedure already adds with id)
                SET @ac_org_id = (SELECT MAX(organisation_id) from Organisation) + 1

				EXEC uspInsertOrganisation 
					
					 @organisation_id               = @ac_org_id
					,@parent_organisation_id        = NULL
					,@organisation_type_id          = 139
					,@organisation_customer_type_id = 0
					,@name                          = @ac_org_name
					,@acn                           = ''
					,@abn                           = ''
					,@organisation_date_added       = @today
					,@organisation_date_modified    = NULL
					,@is_debtor                     = 0
					,@is_creditor                   = 0
					,@bpay_account                  = ''
					,@is_deleted                    = 0
					
					,@weeks_per_service_cycle       = 0
					,@start_date                    = @today
					,@end_date                      = NULL
					,@comment                       = ''
					,@free_services                 = 0
					,@excl_sun                      = 0
					,@excl_mon                      = 0
					,@excl_tue                      = 0
					,@excl_wed                      = 0
					,@excl_thu                      = 0
					,@excl_fri                      = 0
					,@excl_sat                      = 0
					,@sun_start_time                = '08:00:00'
					,@sun_end_time                  = '18:00:00'
					,@mon_start_time                = '08:00:00'
					,@mon_end_time                  = '18:00:00'
					,@tue_start_time                = '08:00:00'
					,@tue_end_time                  = '18:00:00'
					,@wed_start_time                = '08:00:00'
					,@wed_end_time                  = '18:00:00'
					,@thu_start_time                = '08:00:00'
					,@thu_end_time                  = '18:00:00'
					,@fri_start_time                = '08:00:00'
					,@fri_end_time                  = '18:00:00'
					,@sat_start_time                = '08:00:00'
					,@sat_end_time                  = '18:00:00'
					,@sun_lunch_start_time          = '00:00:00'
					,@sun_lunch_end_time            = '00:00:00'
					,@mon_lunch_start_time          = '00:00:00'
					,@mon_lunch_end_time            = '00:00:00'
					,@tue_lunch_start_time          = '00:00:00'
					,@tue_lunch_end_time            = '00:00:00'
					,@wed_lunch_start_time          = '00:00:00'
					,@wed_lunch_end_time            = '00:00:00'
					,@thu_lunch_start_time          = '00:00:00'
					,@thu_lunch_end_time            = '00:00:00'
					,@fri_lunch_start_time          = '00:00:00'
					,@fri_lunch_end_time            = '00:00:00'
					,@sat_lunch_start_time          = '00:00:00'
					,@sat_lunch_end_time            = '00:00:00'
					,@last_batch_run                = NULL
        END

    END


    IF @ac_org_wingid = 0 AND LEN(@ac_org_wingname) > 0
    BEGIN

        SET @ac_org_wingid = (SELECT TOP 1 organisation_id FROM Organisation WHERE name = @ac_org_wingname AND organisation_type_id in (139,367,372) AND is_deleted = 0)

        IF (@ac_org_wingid IS NULL)
        BEGIN

				SET @parent_organisation_id = NULL
				IF (@ac_org_id <> 0) BEGIN SET @parent_organisation_id = @ac_org_id  END

                -- get new org id (as proceedure already adds with id)
                SET @ac_org_wingid = (SELECT MAX(organisation_id) from Organisation) + 1

				EXEC uspInsertOrganisation 
					
					 @organisation_id               = @ac_org_wingid
					,@parent_organisation_id        = @parent_organisation_id
					,@organisation_type_id          = 367
					,@organisation_customer_type_id = 0
					,@name                          = @ac_org_wingname
					,@acn                           = ''
					,@abn                           = ''
					,@organisation_date_added       = @today
					,@organisation_date_modified    = NULL
					,@is_debtor                     = 0
					,@is_creditor                   = 0
					,@bpay_account                  = ''
					,@is_deleted                    = 0
					
					,@weeks_per_service_cycle       = 0
					,@start_date                    = @today
					,@end_date                      = NULL
					,@comment                       = ''
					,@free_services                 = 0
					,@excl_sun                      = 0
					,@excl_mon                      = 0
					,@excl_tue                      = 0
					,@excl_wed                      = 0
					,@excl_thu                      = 0
					,@excl_fri                      = 0
					,@excl_sat                      = 0
					,@sun_start_time                = '08:00:00'
					,@sun_end_time                  = '18:00:00'
					,@mon_start_time                = '08:00:00'
					,@mon_end_time                  = '18:00:00'
					,@tue_start_time                = '08:00:00'
					,@tue_end_time                  = '18:00:00'
					,@wed_start_time                = '08:00:00'
					,@wed_end_time                  = '18:00:00'
					,@thu_start_time                = '08:00:00'
					,@thu_end_time                  = '18:00:00'
					,@fri_start_time                = '08:00:00'
					,@fri_end_time                  = '18:00:00'
					,@sat_start_time                = '08:00:00'
					,@sat_end_time                  = '18:00:00'
					,@sun_lunch_start_time          = '00:00:00'
					,@sun_lunch_end_time            = '00:00:00'
					,@mon_lunch_start_time          = '00:00:00'
					,@mon_lunch_end_time            = '00:00:00'
					,@tue_lunch_start_time          = '00:00:00'
					,@tue_lunch_end_time            = '00:00:00'
					,@wed_lunch_start_time          = '00:00:00'
					,@wed_lunch_end_time            = '00:00:00'
					,@thu_lunch_start_time          = '00:00:00'
					,@thu_lunch_end_time            = '00:00:00'
					,@fri_lunch_start_time          = '00:00:00'
					,@fri_lunch_end_time            = '00:00:00'
					,@sat_lunch_start_time          = '00:00:00'
					,@sat_lunch_end_time            = '00:00:00'
					,@last_batch_run                = NULL
        END

    END

	IF (@ac_org_room <> '')
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				166,
				'',  -- free text
				@ac_org_room,
				'',
				'',  -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				@site_id,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END

    IF @ac_org_wingid <> 0 
    BEGIN

        INSERT into RegisterPatient
        VALUES (
             @ac_org_wingid
            ,@patient_id
            ,GETDATE()
            ,0
        )

    END

    IF @ac_org_wingid = 0 AND @ac_org_id <> 0
    BEGIN

        INSERT into RegisterPatient
        VALUES (
             @ac_org_id
            ,@patient_id
            ,GETDATE()
            ,0
        )

    END


COMMIT TRAN

--------------------------------------

 */

/* uspInsertPatientV4

--------------------------------------

--DROP PROCEDURE uspInsertPatientV4;

CREATE PROCEDURE uspInsertPatientV4

     @staff_id             int

    ,@added_by             int
    ,@title_id             int
    ,@firstname            varchar(100)
    ,@middlename           varchar(100)
    ,@surname              varchar(100)
    ,@nickname             varchar(100)
    ,@gender               varchar(1)
    ,@dob                  datetime
    ,@person_date_added    datetime
    ,@person_date_modified datetime

    ,@patient_id         int
    ,@patient_date_added datetime
    ,@is_clinic_patient  bit
    ,@is_deleted         bit
    ,@is_deceased        bit



    ,@ac_type_id              int
    ,@ac_org_id               int
    ,@ac_org_name             varchar(500)
    ,@ac_org_wingid           int
    ,@ac_org_wingname         varchar(500)
    ,@ac_org_room             varchar(500)

    ,@site_id                 int

    ,@phone                   varchar(100)
    ,@phone_contact_type_id   int

    ,@homephone               varchar(100)
    ,@workphone               varchar(100)
    ,@mobile                  varchar(100)
    ,@email                   varchar(200)


    ,@addr_contact_type_id    int
    ,@addr1                   varchar(100)
    ,@addr2                   varchar(100)
    ,@street                  varchar(100)
    ,@address_channel_type_id int
						
    ,@suburb                  varchar(100)
    ,@state                   varchar(100)
    ,@postcode                varchar(100)

    ,@flashing_text           varchar(1000)

AS

Declare @entity_id int
Declare @person_id int

Declare @suburb_id int

Declare @today     datetime
Declare @parent_organisation_id   int

BEGIN TRAN

	SET @today = GETDATE()

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



    IF (@patient_id <> 0)
    BEGIN

        SET IDENTITY_INSERT Patient ON

        INSERT INTO Patient (patient_id,person_id, patient_date_added, is_clinic_patient, is_deleted, is_deceased, flashing_text, flashing_text_added_by, flashing_text_last_modified_date, private_health_fund, concession_card_number,concession_card_expiry_date,is_diabetic,is_member_diabetes_australia,diabetic_assessment_review_date,
                             ac_inv_offering_id, ac_pat_offering_id, login, pwd, is_gp_patient) 
        VALUES
        (
         @patient_id
        ,@person_id
        ,@patient_date_added
        ,@is_clinic_patient
        ,@is_deleted
        ,@is_deceased
        ,@flashing_text
        ,NULL
        ,NULL
        ,''
        ,''
        ,null
        ,0
        ,0
        ,NULL

        ,@ac_type_id
        ,@ac_type_id
        ,''
        ,''
        ,0

        )
 
        SET IDENTITY_INSERT Patient OFF

    END
    ELSE
    BEGIN

        INSERT INTO Patient (person_id, patient_date_added, is_clinic_patient, is_deleted, is_deceased, flashing_text, flashing_text_added_by, flashing_text_last_modified_date, private_health_fund, concession_card_number,concession_card_expiry_date,is_diabetic,is_member_diabetes_australia,diabetic_assessment_review_date,
                             ac_inv_offering_id, ac_pat_offering_id, login, pwd, is_gp_patient) 
        VALUES
        (
         @person_id
        ,@patient_date_added
        ,@is_clinic_patient
        ,@is_deleted
        ,@is_deceased
        ,@flashing_text
        ,NULL
        ,NULL
        ,''
        ,''
        ,null
        ,0
        ,0
        ,NULL

        ,@ac_type_id
        ,@ac_type_id
        ,''
        ,''
        ,0

        )

        SET @patient_id = SCOPE_IDENTITY()

    END



	IF (LEN(@phone) > 0)
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				@phone_contact_type_id,
				'',  -- free text
				@phone,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				@site_id,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END

	IF (LEN(@homephone) > 0)
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				33,
				'',  -- free text
				@homephone,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				@site_id,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END

	IF (LEN(@workphone) > 0)
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				34,
				'',  -- free text
				@workphone,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				@site_id,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END

	IF (LEN(@mobile) > 0)
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				30,
				'',  -- free text
				@mobile,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				@site_id,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END

	IF (LEN(@email) > 0)
    BEGIN

 		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				27,
				'',  -- free text
				@email,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				NULL,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

    END


	IF (@addr1 <> '' OR @addr2 <> '' OR @street <> '' OR @suburb <> '' OR @suburb <> '' OR @postcode <> '' OR @state <> '' )
	BEGIN

     	SET @suburb_id = (SELECT suburb_id FROM Suburb WHERE name = @suburb AND postcode = @postcode AND state = @state)

        IF (@suburb_id IS NULL AND LEN(@postcode) = 0 AND (SELECT COUNT(*) FROM Suburb WHERE  name = @suburb AND state = @state) = 1)
        BEGIN
        	SET @suburb_id = (SELECT TOP 1 suburb_id FROM Suburb WHERE name = @suburb AND state = @state)
        END

        IF (@suburb_id IS NULL AND LEN(@state) = 0 AND (SELECT COUNT(*) FROM Suburb WHERE  name = @suburb AND postcode = @postcode) = 1)
        BEGIN
        	SET @suburb_id = (SELECT TOP 1 suburb_id FROM Suburb WHERE name = @suburb AND postcode = @postcode)
        END

        IF (@suburb_id IS NULL AND LEN(@suburb) = 0 AND (SELECT COUNT(*) FROM Suburb WHERE  state = @state AND postcode = @postcode) = 1)
        BEGIN
        	SET @suburb_id = (SELECT TOP 1 suburb_id FROM Suburb WHERE state = @state AND postcode = @postcode)
        END


		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				@addr_contact_type_id,
				'',  -- free text
				@addr1,
				@addr2,
				@street,  -- street_name
				@address_channel_type_id, -- address_channel_type_id
				@suburb_id,
				NULL, -- country_id,
				@site_id,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END


    SET @ac_org_name     = LTRIM(RTRIM(@ac_org_name))
    SET @ac_org_wingname = LTRIM(RTRIM(@ac_org_wingname))
    SET @ac_org_room     = LTRIM(RTRIM(@ac_org_room))


    IF @ac_org_id = 0 AND LEN(@ac_org_name) > 0
    BEGIN

        SET @ac_org_id = (SELECT TOP 1 organisation_id FROM Organisation WHERE name = @ac_org_name AND organisation_type_id in (139,367,372) AND is_deleted = 0)

        IF (@ac_org_id IS NULL)
        BEGIN

                -- get new org id (as proceedure already adds with id)
                SET @ac_org_id = (SELECT MAX(organisation_id) from Organisation) + 1

				EXEC uspInsertOrganisation 
					
					 @organisation_id               = @ac_org_id
					,@parent_organisation_id        = NULL
					,@organisation_type_id          = 139
					,@organisation_customer_type_id = 0
					,@name                          = @ac_org_name
					,@acn                           = ''
					,@abn                           = ''
					,@organisation_date_added       = @today
					,@organisation_date_modified    = NULL
					,@is_debtor                     = 0
					,@is_creditor                   = 0
					,@bpay_account                  = ''
					,@is_deleted                    = 0
					
					,@weeks_per_service_cycle       = 0
					,@start_date                    = @today
					,@end_date                      = NULL
					,@comment                       = ''
					,@free_services                 = 0
					,@excl_sun                      = 0
					,@excl_mon                      = 0
					,@excl_tue                      = 0
					,@excl_wed                      = 0
					,@excl_thu                      = 0
					,@excl_fri                      = 0
					,@excl_sat                      = 0
					,@sun_start_time                = '08:00:00'
					,@sun_end_time                  = '18:00:00'
					,@mon_start_time                = '08:00:00'
					,@mon_end_time                  = '18:00:00'
					,@tue_start_time                = '08:00:00'
					,@tue_end_time                  = '18:00:00'
					,@wed_start_time                = '08:00:00'
					,@wed_end_time                  = '18:00:00'
					,@thu_start_time                = '08:00:00'
					,@thu_end_time                  = '18:00:00'
					,@fri_start_time                = '08:00:00'
					,@fri_end_time                  = '18:00:00'
					,@sat_start_time                = '08:00:00'
					,@sat_end_time                  = '18:00:00'
					,@sun_lunch_start_time          = '00:00:00'
					,@sun_lunch_end_time            = '00:00:00'
					,@mon_lunch_start_time          = '00:00:00'
					,@mon_lunch_end_time            = '00:00:00'
					,@tue_lunch_start_time          = '00:00:00'
					,@tue_lunch_end_time            = '00:00:00'
					,@wed_lunch_start_time          = '00:00:00'
					,@wed_lunch_end_time            = '00:00:00'
					,@thu_lunch_start_time          = '00:00:00'
					,@thu_lunch_end_time            = '00:00:00'
					,@fri_lunch_start_time          = '00:00:00'
					,@fri_lunch_end_time            = '00:00:00'
					,@sat_lunch_start_time          = '00:00:00'
					,@sat_lunch_end_time            = '00:00:00'
					,@last_batch_run                = NULL
        END

    END


    IF @ac_org_wingid = 0 AND LEN(@ac_org_wingname) > 0
    BEGIN

        SET @ac_org_wingid = (SELECT TOP 1 organisation_id FROM Organisation WHERE name = @ac_org_wingname AND organisation_type_id in (139,367,372) AND is_deleted = 0)

        IF (@ac_org_wingid IS NULL)
        BEGIN

				SET @parent_organisation_id = NULL
				IF (@ac_org_id <> 0) BEGIN SET @parent_organisation_id = @ac_org_id  END

                -- get new org id (as proceedure already adds with id)
                SET @ac_org_wingid = (SELECT MAX(organisation_id) from Organisation) + 1

				EXEC uspInsertOrganisation 
					
					 @organisation_id               = @ac_org_wingid
					,@parent_organisation_id        = @parent_organisation_id
					,@organisation_type_id          = 367
					,@organisation_customer_type_id = 0
					,@name                          = @ac_org_wingname
					,@acn                           = ''
					,@abn                           = ''
					,@organisation_date_added       = @today
					,@organisation_date_modified    = NULL
					,@is_debtor                     = 0
					,@is_creditor                   = 0
					,@bpay_account                  = ''
					,@is_deleted                    = 0
					
					,@weeks_per_service_cycle       = 0
					,@start_date                    = @today
					,@end_date                      = NULL
					,@comment                       = ''
					,@free_services                 = 0
					,@excl_sun                      = 0
					,@excl_mon                      = 0
					,@excl_tue                      = 0
					,@excl_wed                      = 0
					,@excl_thu                      = 0
					,@excl_fri                      = 0
					,@excl_sat                      = 0
					,@sun_start_time                = '08:00:00'
					,@sun_end_time                  = '18:00:00'
					,@mon_start_time                = '08:00:00'
					,@mon_end_time                  = '18:00:00'
					,@tue_start_time                = '08:00:00'
					,@tue_end_time                  = '18:00:00'
					,@wed_start_time                = '08:00:00'
					,@wed_end_time                  = '18:00:00'
					,@thu_start_time                = '08:00:00'
					,@thu_end_time                  = '18:00:00'
					,@fri_start_time                = '08:00:00'
					,@fri_end_time                  = '18:00:00'
					,@sat_start_time                = '08:00:00'
					,@sat_end_time                  = '18:00:00'
					,@sun_lunch_start_time          = '00:00:00'
					,@sun_lunch_end_time            = '00:00:00'
					,@mon_lunch_start_time          = '00:00:00'
					,@mon_lunch_end_time            = '00:00:00'
					,@tue_lunch_start_time          = '00:00:00'
					,@tue_lunch_end_time            = '00:00:00'
					,@wed_lunch_start_time          = '00:00:00'
					,@wed_lunch_end_time            = '00:00:00'
					,@thu_lunch_start_time          = '00:00:00'
					,@thu_lunch_end_time            = '00:00:00'
					,@fri_lunch_start_time          = '00:00:00'
					,@fri_lunch_end_time            = '00:00:00'
					,@sat_lunch_start_time          = '00:00:00'
					,@sat_lunch_end_time            = '00:00:00'
					,@last_batch_run                = NULL
        END

    END

	IF (@ac_org_room <> '')
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@entity_id,
				166,
				'',  -- free text
				@ac_org_room,
				'',
				'',  -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				@site_id,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END

    IF @ac_org_wingid <> 0 
    BEGIN

        INSERT into RegisterPatient
        VALUES (
             @ac_org_wingid
            ,@patient_id
            ,GETDATE()
            ,0
        )

    END

    IF @ac_org_wingid = 0 AND @ac_org_id <> 0
    BEGIN

        INSERT into RegisterPatient
        VALUES (
             @ac_org_id
            ,@patient_id
            ,GETDATE()
            ,0
        )

    END


COMMIT TRAN

--------------------------------------

 */



/* PatientHistory  (ref: Patient, Offering)

CREATE TABLE PatientHistory
(
 patient_history_id                int           not null PRIMARY KEY identity,
 patient_id                        int           not null FOREIGN KEY REFERENCES Patient(patient_id), 

 is_clinic_patient                 bit           not null,
 is_gp_patient                     bit           not null,
 is_deleted                        bit           not null DEFAULT 0,
 is_deceased                       bit           not null,
 flashing_text                     varchar(max)  not null,
 flashing_text_added_by            int                    FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT null,
 flashing_text_last_modified_date  datetime               DEFAULT null,
 private_health_fund               varchar(100)  not null,
 concession_card_number            varchar(100)  not null,
 concession_card_expiry_date       datetime               DEFAULT null,
 is_diabetic                       bit           not null,
 is_member_diabetes_australia      bit           not null,
 diabetic_assessment_review_date   datetime               DEFAULT null,
 ac_inv_offering_id                int                    FOREIGN KEY REFERENCES Offering(offering_id) DEFAULT null,
 ac_pat_offering_id                Int                    FOREIGN KEY REFERENCES Offering(offering_id) DEFAULT null,
 login                             varchar(50)   not null DEFAULT '',
 pwd                               varchar(50)   not null DEFAULT '',
 is_company                        bit           not null, 
 abn                               varchar(50)   not null,
 next_of_kin_name                  varchar(100)  not null DEFAULT ''
 next_of_kin_relation              varchar(100)  not null DEFAULT ''
 next_of_kin_contact_info          varchar(2000) not null DEFAULT ''

 title_id                          int           not null FOREIGN KEY REFERENCES Title(title_id),
 firstname                         varchar(100)  not null,
 middlename                        varchar(100)  not null,
 surname                           varchar(100)  not null,
 nickname                          varchar(100)  not null,
 gender                            varchar(1)    not null,
 dob                               datetime,

 modified_from_this_by             int           not null FOREIGN KEY REFERENCES Staff(staff_id),
 date_added                        datetime      not null,

);

*/


/* OfferingType, AgedCarePatientType, OfferingInvoiceType 

-------------------------------------------
CREATE TABLE OfferingType
(
 offering_type_id    int  not null PRIMARY KEY identity,
 descr varchar(50) not null
);

SET IDENTITY_INSERT OfferingType ON;
INSERT OfferingType
   (offering_type_id,descr)
VALUES
   (63,'Service'),
   (89,'Product'),
   (90,'Capitalised Asset (Ignore)'),
   (194,'Package'),
   (398,'DVA Visit'),
   (399,'Travel');
SET IDENTITY_INSERT OfferingType OFF;
-------------------------------------------

-- defines what sort of patient the offering is designed specifically for (within aged care)
CREATE TABLE AgedCarePatientType
(
 aged_care_patient_type_id    int  not null PRIMARY KEY identity,
 descr varchar(50) not null
);

INSERT AgedCarePatientType
   (descr)
VALUES
   ('--Not Aged Care--'),
   ('Low Care'),
   ('High Care'),
   ('Low Care Funded'),
   ('High Care Unfunded'),
   ('Low Care Emergency'),
   ('High care Emergency'),
   ('Medicare'),
   ('DVA'),
   ('TAC');
-------------------------------------------

-- to control which type of invoice an offering could appear on
CREATE TABLE OfferingInvoiceType
(
 offering_invoice_type_id    int  not null PRIMARY KEY identity,
 descr varchar(50) not null
);

SET IDENTITY_INSERT OfferingInvoiceType ON;
INSERT OfferingInvoiceType
   (offering_invoice_type_id,descr)
VALUES
   (0,'Error'),
   (1,'Clinic'),  -- clinic
   (3,'Clinic & Aged Care'),
   (4,'Aged Care');  
SET IDENTITY_INSERT OfferingInvoiceType OFF;
-------------------------------------------

*/

/* Offering  (ref: Field, OfferingType, AgedCarePatientType, OfferingInvoiceType, Letter)

CREATE TABLE Offering
(
 offering_id                        int           not null PRIMARY KEY identity,
 field_id                           int           not null FOREIGN KEY REFERENCES Field(field_id),
 offering_type_id                   int           not null FOREIGN KEY REFERENCES OfferingType(offering_type_id),
 aged_care_patient_type_id  int   not null FOREIGN KEY REFERENCES AgedCarePatientType(aged_care_patient_type_id),
 num_clinic_visits_allowed_per_year int           not null,    -- 0 for null  (= warning level in old system)
 offering_invoice_type_id           int           not null FOREIGN KEY REFERENCES OfferingInvoiceType( offering_invoice_type_id),

 name                      varchar(2000)  not null,  -- 2000 just for site 40 Medicilnic Client Invoicing - other sites limited in GUI to 100 chars
 short_name                varchar(100)   not null,
 descr                     varchar(500)   not null,

 is_gst_exempt             bit           not null,
 default_price             decimal(8,2)  not null,

 service_time_minutes      int           not null,
 is_deleted                bit           not null DEFAULT 0,   -- set if offering no longer relevant

 max_nbr_claimable         int           not null,    -- NEW, not in BEST.  0 = no max
 max_nbr_claimable_months  int           not null,    -- NEW, not in BEST.  how many past months before a treatment the max nbr is for

 medicare_company_code     varchar(50)   not null,    -- national health id for claims. on medicare invoice
 dva_company_code          varchar(50)   not null,    -- dva item that dva see on invoice
 tac_company_code          varchar(50)   not null,

 medicare_charge           decimal(5,2)  not null,
 dva_charge                decimal(5,2)  not null,
 tac_charge                decimal(5,2)  not null,

 popup_message             varchar(max)  not null,

 reminder_letter_months_later_to_send  int  not null, -- zero means disabled, but if enabled (more than zero) then reminder_letter_id must be set and not null
 reminder_letter_id                    int            FOREIGN KEY REFERENCES Letter(letter_id),

 use_custom_color          bit           not null default 0,
 custom_color              varchar(10)   not null default 'FFFFFF',

);



dva_visit_type [FIELD REMOVED 3/7/2013]:
-------------
in BEST, dva_visit_type has a fk (stored as a fucking string?!?) that links to another table (in his case KPI_KPI_SEQ [ie kpi_id] which is his generic table for everything of type id|descr)
this other table is meant to have a descr that has a medicare code for dva "home visit" types
to get this list, search for kpi row of type 44 (there is only one in there right now)

his reason for making it link to a whole other table is :

""""
When dealing with offerings of type 398, I provide a look up list of all the DVA home visit codes (from the kpi table).
This prevents a person from entering an invalid code when creating a DVA home visit offering. It was easier for me to do
this than to have to get Marcus provide me with the valid DVA home visit reason codes and then checking for a possible error.
"""
but when I said it was exactly the same as the normal codes he already has as free text (medicare_company_code, dva_company_code),
he said it was because "I did not know how many characters to provide" and so he made it link to another table 
this reason makes no fucking sense since he could jsut have made it the same length as the field in the kpi table that it references....

so - in this system we will just put it directly in that field and not link it anywhere or make any other table

*/


/* OrganisationOfferings  (ref: Organisation, Offering)

CREATE TABLE OrganisationOfferings
(
 organisation_offering_id           int           not null PRIMARY KEY identity,
 organisation_id                    int           not null FOREIGN KEY REFERENCES Organisation(organisation_id),
 offering_id                        int           not null FOREIGN KEY REFERENCES Offering(offering_id),
 price                              decimal(8,2)  not null,
 date_active                        datetime               DEFAULT null,
);

*/


/* StaffOfferings  (ref: Staff, Offering)

CREATE TABLE StaffOfferings
(
 staff_offering_id                  int           not null PRIMARY KEY identity,
 staff_id                           int           not null FOREIGN KEY REFERENCES Staff(staff_id),
 offering_id                        int           not null FOREIGN KEY REFERENCES Offering(offering_id),
 is_commission                      bit           not null,
 commission_percent                 decimal(5,2)  not null,
 is_fixed_rate                      bit           not null,
 fixed_rate                         decimal(8,2)  not null,
 date_active                        datetime               DEFAULT null,  -- no start date = inactive -- get most recent active one (top 1 where not null order by start date desc)
);

*/




/* Referrer  (ref: Person)

CREATE TABLE Referrer
(
 referrer_id               int           not null PRIMARY KEY identity,
 person_id                 int           not null FOREIGN KEY REFERENCES Person(person_id),
 referrer_date_added       datetime      not null DEFAULT (GETDATE()),
 is_deleted                bit           not null DEFAULT 0,
);

*/

/* uspInsertReferrer

--------------------------------------

DROP PROCEDURE uspInsertReferrer;

CREATE PROCEDURE uspInsertReferrer 

     @added_by             int
    ,@title_id             int
    ,@firstname            varchar(100)
    ,@middlename           varchar(100)
    ,@surname              varchar(100)
    ,@nickname             varchar(100)
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

--------------------------------------

EXEC uspInsertReferrer 

     @added_by             = 1
    ,@title_id             = 6
    ,@firstname            = 'fn'
    ,@middlename           = 'mn'
    ,@surname              = 'sn'
    ,@nickname             = ''
    ,@gender               = 'M'
    ,@dob                  = '1980-01-01 00:00:00'
    ,@person_date_added    = '1980-01-01 00:00:00'
    ,@person_date_modified = '1980-01-01 00:00:00'

    ,@referrer_id          = 20
    ,@referrer_date_added  = '2010-01-01 00:00:00'
    ,@is_deleted           = 0

--------------------------------------

 */



/* RegisterReferrer  (ref: Organisation, Referrer)

-- NB referrer role (in contact file) is one of [0,286]. 286 = "GP / Medical Practitioner", so can just leave it out
-- NB organisation_id can be null because charles had a bunch of currupt data because he never learned about the idea of data integrity and foreign fucking keys

CREATE TABLE RegisterReferrer
(
 register_referrer_id                               int           not null PRIMARY KEY identity,
 organisation_id                                    int                    FOREIGN KEY REFERENCES Organisation(organisation_id),
 referrer_id                                        int           not null FOREIGN KEY REFERENCES Referrer(referrer_id),
 provider_number                                    varchar(50)   not null,
 report_every_visit_to_referrer                     bit           not null,
 batch_send_all_patients_treatment_notes            bit           not null,
 date_last_batch_send_all_patients_treatment_notes  datetime          DEFAULT null,
 register_referrer_date_added                       datetime      not null DEFAULT (GETDATE()),
 is_deleted                                         bit           not null DEFAULT 0,
);

*/

/* RegisterPatient  (ref: Organisation, Patient)

CREATE TABLE RegisterPatient
(
 register_patient_id         int           not null PRIMARY KEY identity,
 organisation_id             int           not null FOREIGN KEY REFERENCES Organisation(organisation_id),
 patient_id                  int           not null FOREIGN KEY REFERENCES Patient(patient_id),
 register_patient_date_added datetime      not null DEFAULT (GETDATE()),
 is_deleted                  bit not null DEFAULT 0, 
);

*/

/* PatientReferrer  (ref: RegisterReferrer, Patient, Organisation)

CREATE TABLE PatientReferrer
(
 patient_referrer_id            int          not null PRIMARY KEY identity,
 patient_id                     int          not null FOREIGN KEY REFERENCES Patient(patient_id),
 register_referrer_id           int                   FOREIGN KEY REFERENCES RegisterReferrer(register_referrer_id),  --     epc referrers 
 organisation_id                int                   FOREIGN KEY REFERENCES Organisation(organisation_id),           -- non epc referrers
 patient_referrer_date_added    datetime     not null DEFAULT (GETDATE()),
 is_debtor                      bit          not null,
 is_active                      bit          not null DEFAULT 1,
);

*/


/* uspInsertRegisterReferrer

--------------------------------------

--DROP PROCEDURE uspInsertRegisterReferrer;

CREATE PROCEDURE uspInsertRegisterReferrer 

     @title_id             int
    ,@firstname            varchar(100)
    ,@middlename           varchar(100)
    ,@surname              varchar(100)
    ,@nickname             varchar(100)
    ,@gender               varchar(1)

    ,@org_name             varchar(100)
    ,@org_ph               varchar(100)
    ,@org_fax              varchar(100)
    ,@org_email            varchar(100)
    ,@org_web              varchar(100)

	,@org_addr_line1       varchar(100)
	,@org_addr_line2       varchar(100)
	,@org_suburb           varchar(100)
	,@org_postcode         varchar(100)
	,@org_state            varchar(100)
	,@org_country          varchar(100)



AS

Declare @person_entity_id           int
Declare @person_id                  int
Declare @referrer_id                int
Declare @organisation_entity_id     int
Declare @organisation_id            int
Declare @today                      datetime
Declare @suburb_id                  int

BEGIN TRAN


    SET @today = GETDATE()


    INSERT INTO Entity DEFAULT VALUES;
    SET @person_entity_id = SCOPE_IDENTITY()


    INSERT INTO Person (added_by,entity_id, title_id, firstname, middlename, surname, nickname, gender, dob, person_date_added, person_date_modified) 
    VALUES
    (
     NULL
    ,@person_entity_id
    ,@title_id
    ,@firstname
    ,@middlename
    ,@surname
    ,@nickname
    ,@gender
    ,NULL
    ,@today
    ,NULL
    )

    SET @person_id = SCOPE_IDENTITY()


    INSERT INTO Referrer (person_id, referrer_date_added, is_deleted) 
    VALUES
    (
     @person_id
    ,@today
    ,0
    )
 
    SET @referrer_id = SCOPE_IDENTITY()




    INSERT INTO Entity DEFAULT VALUES;
    SET @organisation_entity_id = SCOPE_IDENTITY()


    INSERT INTO Organisation (entity_id,parent_organisation_id,use_parent_offernig_prices,organisation_type_id,organisation_customer_type_id,
                              name,acn,abn,organisation_date_added,organisation_date_modified,is_debtor,is_creditor,bpay_account,is_deleted,

                              weeks_per_service_cycle,start_date,end_date,comment,free_services,excl_sun,excl_mon,excl_tue,excl_wed,excl_thu,
                              excl_fri,excl_sat,
                              sun_start_time,sun_end_time,mon_start_time,mon_end_time,tue_start_time,tue_end_time,wed_start_time,
                              wed_end_time,thu_start_time,thu_end_time,fri_start_time,fri_end_time,sat_start_time,sat_end_time,
                              sun_lunch_start_time,sun_lunch_end_time,mon_lunch_start_time,mon_lunch_end_time,tue_lunch_start_time,tue_lunch_end_time,wed_lunch_start_time,
                              wed_lunch_end_time,thu_lunch_start_time,thu_lunch_end_time,fri_lunch_start_time,fri_lunch_end_time,sat_lunch_start_time,sat_lunch_end_time,
                              last_batch_run)
    VALUES
    (
     @organisation_entity_id
    ,NULL
    ,0
    ,191
    ,0
    ,@org_name
    ,''
    ,''
    ,@today
    ,NULL
    ,0
    ,1
    ,'0'
    ,0

    ,0
    ,NULL
    ,NULL
    ,''
    ,0

    ,0
    ,0
    ,0
    ,0
    ,0
    ,0
    ,0

    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'

    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'

    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'

    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,'00:00:00'
    ,NULL
    )
 
    SET @organisation_id = SCOPE_IDENTITY()



    INSERT INTO RegisterReferrer (organisation_id, referrer_id, provider_number, report_every_visit_to_referrer, 
                                  batch_send_all_patients_treatment_notes, date_last_batch_send_all_patients_treatment_notes, 
                                  register_referrer_date_added, is_deleted) 
    VALUES
    (
      @organisation_id
     ,@referrer_id
     ,''
     ,0
     ,1
     ,NULL
     ,@today
     ,0
    )
    

    SET @org_ph    = LTRIM(RTRIM(@org_ph))
    SET @org_fax   = LTRIM(RTRIM(@org_fax))
    SET @org_email = LTRIM(RTRIM(@org_email))
    SET @org_web   = LTRIM(RTRIM(@org_web))
    
	IF (LEN(@org_ph) > 0)
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@organisation_entity_id,
				34,
				'',  -- free text
				@org_ph,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				NULL,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)
	END    

	IF (LEN(@org_fax) > 0)
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@organisation_entity_id,
				29,
				'',  -- free text
				@org_fax,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				NULL,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)
	END   
	
	IF (LEN(@org_email) > 0)
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@organisation_entity_id,
				27,
				'',  -- free text
				@org_email,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				NULL,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)
	END   
	
	IF (LEN(@org_web) > 0)
	BEGIN

		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@organisation_entity_id,
				28,
				'',  -- free text
				@org_web,
				'',   -- addr_line2
				'',   -- street_name
				NULL, -- address_channel_type_id
				NULL,
				NULL, -- country_id,
				NULL,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)
	END   
    

	IF (@org_addr_line1 <> '' OR @org_addr_line2 <> '' OR @org_suburb <> '' OR @org_postcode <> '' OR @org_state <> '' )
	BEGIN

     	SET @suburb_id = (SELECT suburb_id FROM Suburb WHERE name = @org_suburb AND postcode = @org_postcode AND state = @org_state)

        IF (@suburb_id IS NULL AND LEN(@org_postcode) = 0 AND (SELECT COUNT(*) FROM Suburb WHERE  name = @org_suburb AND state = @org_state) = 1)
        BEGIN
        	SET @suburb_id = (SELECT TOP 1 suburb_id FROM Suburb WHERE name = @org_suburb AND state = @org_state)
        END

        IF (@suburb_id IS NULL AND LEN(@org_state) = 0 AND (SELECT COUNT(*) FROM Suburb WHERE  name = @org_suburb AND postcode = @org_postcode) = 1)
        BEGIN
        	SET @suburb_id = (SELECT TOP 1 suburb_id FROM Suburb WHERE name = @org_suburb AND postcode = @org_postcode)
        END

        IF (@suburb_id IS NULL AND LEN(@org_suburb) = 0 AND (SELECT COUNT(*) FROM Suburb WHERE  state = @org_state AND postcode = @org_postcode) = 1)
        BEGIN
        	SET @suburb_id = (SELECT TOP 1 suburb_id FROM Suburb WHERE state = @org_state AND postcode = @org_postcode)
        END


		IF (@suburb_id IS NULL AND (@org_suburb <> '' OR @org_postcode <> '' OR @org_state <> ''))
		BEGIN
			
			IF (@org_suburb <> '')
			BEGIN
			
				IF(LEN(@org_addr_line2) <> 0)
				BEGIN
					SET @org_addr_line2 = @org_addr_line2 + ', '
				END
			
				SET @org_addr_line2 = @org_addr_line2 + @org_suburb
				
			END			
			
			IF (@org_state <> '')
			BEGIN
			
				IF(LEN(@org_addr_line2) <> 0)
				BEGIN
					SET @org_addr_line2 = @org_addr_line2 + ', '
				END
			
				SET @org_addr_line2 = @org_addr_line2 + @org_state
				
			END		
			
			IF (@org_postcode <> '')
			BEGIN
			
				IF(LEN(@org_addr_line2) <> 0)
				BEGIN
					SET @org_addr_line2 = @org_addr_line2 + ', '
				END
			
				SET @org_addr_line2 = @org_addr_line2 + @org_postcode
				
			END		
			
		END


		INSERT INTO ContactAus ( 
				entity_id,
				contact_type_id,
				free_text,
				addr_line1,
				addr_line2,
				street_name,
				address_channel_type_id,
				suburb_id,
				country_id,
				site_id,
				is_billing,
				is_non_billing,
				contact_date_added,
				contact_date_modified,
				contact_date_deleted
		)
		VALUES 
		(
				@organisation_entity_id,
				36,
				'',  -- free text
				@org_addr_line1,
				@org_addr_line2,
				'',  -- street_name
				NULL, -- address_channel_type_id
				@suburb_id,
				NULL, -- country_id,
				NULL,  -- site_id
				0,  -- is_billing
				0,  -- is_non_billing
				GETDATE(),
				NULL,
				NULL
		)

	END

COMMIT TRAN

--------------------------------------

 */




/* HealthCard  (ref: Patient, Organisation, Staff)


-------------------------------------------------------
Knowing if a card "has" an epc: if either date set
So
=> make sure both dates set for new card!
=> and dont let them remove either date [once set] !!!

=> and ofc dont allow del referrer (from patient)
-------------------------------------------------------

=> dva card still needs epc with signed dates, but no max number cuz can claim unlimited

-------------------------------------------------------
EPC - why disallowing blank signed dates
- without a signed date, its not a valid epc card - you can't claim on it without the signed date, 
  so enforcing a signed date at the time it is put in will save you ALOT of headaches later on for claiming when they forgot to put the date in.
- it still will not show on the booking screen anyway that it is an EPC booking because without the signed date set, 
  it is not a valid EPC and it will be shown as a private invoice so it doesnt help the booking screen

Making sure they enter the whole thing in one go. Either:
 - if they really want an EPC card put in over the phone, get date on the phone
 - else put it ALL in when the patient turns up
so they rather than putting in part of the EPC when they call and part when they turn up, 
its safer to put in the epc all in one go and they won't forget to have the EPC invalid without a signed date.

Putting it in twice is going to turn into a huge mess by having an option to leave the signed date blank. 
-------------------------------------------------------


CREATE TABLE HealthCard
(
 health_card_id                       int          not null PRIMARY KEY identity,
 patient_id                           int          not null FOREIGN KEY REFERENCES Patient(patient_id),
 organisation_id                      int          not null FOREIGN KEY REFERENCES Organisation(organisation_id),  -- Medicare or DVA
 card_name                            varchar(50)  not null, 
 card_nbr                             varchar(50)  not null, 
 card_family_member_nbr               varchar(4)   not null, 
 expiry_date                          datetime,
 date_referral_signed                 datetime default null,
 date_referral_received_in_office     datetime default null,
 is_active                            bit          not null,                                -- if they hvae 2 cards .. only one can be active at a time
 added_or_last_modified_by            int          FOREIGN KEY REFERENCES Staff(staff_id),  -- nullable since added after BEST data
 added_or_last_modified_date          datetime     DEFAULT (GETDATE()),                     -- nullable since added after BEST data
 area_treated                         varchar(500) not null, 
);

*/

/* HealthCardActionType

CREATE TABLE HealthCardActionType
(
 health_card_action_type_id  int  not null PRIMARY KEY identity,
 descr varchar(50)  not null
);

SET IDENTITY_INSERT HealthCardActionType ON;
INSERT HealthCardActionType
   (health_card_action_type_id,descr)
VALUES
   (1,'Requested'),
   (0,'Received'),
   (2,'First Treatment (Letter Sent)'),
   (3,'Last Letter Sent')
SET IDENTITY_INSERT HealthCardActionType OFF;

*/

/* HealthCardAction  (ref: HealthCardActionType, HealthCard)

CREATE TABLE HealthCardAction
(
 health_card_action_id      int      not null PRIMARY KEY identity,
 health_card_id             int      not null FOREIGN KEY REFERENCES HealthCard(health_card_id),
 health_card_action_type_id int      not null FOREIGN KEY REFERENCES HealthCardActionType(health_card_action_type_id),
 action_date                datetime not null DEFAULT (GETDATE())
);

*/

/* HealthCardEPCChangeHistory  (ref: HealthCard, Staff)

CREATE TABLE HealthCardEPCChangeHistory
(
  health_card_epc_change_history_id     int     not null PRIMARY KEY identity,
  health_card_id                        int     not null FOREIGN KEY REFERENCES HealthCard(health_card_id),
  staff_id                              int     not null FOREIGN KEY REFERENCES Staff(staff_id),
  date                                  datetime          DEFAULT (GETDATE()),
  is_new_epc_card_set                   bit     not null,

  pre_date_referral_signed              datetime,
  pre_date_referral_received_in_office  datetime,

  post_date_referral_signed             datetime,
  post_date_referral_received_in_office datetime
);

*/


/* HealthCardEPCRemaining  (ref: HealthCard, Field)

CREATE TABLE HealthCardEPCRemaining
(
 health_card_epc_remaining_id  int          not null PRIMARY KEY identity,
 health_card_id                int          not null FOREIGN KEY REFERENCES HealthCard(health_card_id),
 field_id                      int          not null FOREIGN KEY REFERENCES Field(field_id),
 num_services_remaining        int          not null,
 deleted_by                    int          FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,
 date_deleted                  datetime     DEFAULT null,
);

 */

/* HealthCardEPCRemainingChangeHistory  (ref: HealthCardEPCRemaining, Staff)

CREATE TABLE HealthCardEPCRemainingChangeHistory
(
  health_card_epc_remaining_change_history_id   int      not null PRIMARY KEY identity,
  health_card_epc_remaining_id                  int      not null FOREIGN KEY REFERENCES HealthCardEPCRemaining(health_card_epc_remaining_id),
  staff_id                                      int      not null FOREIGN KEY REFERENCES Staff(staff_id),
  date                                          datetime          DEFAULT (GETDATE()),
  pre_num_services_remaining                    int,       -- CAN BE NULL ... SHOWS THAT ITS AN ADDITION, NOT AN EDIT
  post_num_services_remaining                   int,       -- CAN BE NULL ... SHOWS THAT ITS A  DELETION, NOT AN EDIT
);

*/




/* WeekDay

CREATE TABLE WeekDay
(
 weekday_id    int  not null PRIMARY KEY identity,
 descr              varchar(10) not null
);

SET IDENTITY_INSERT WeekDay ON;
INSERT WeekDay
   (weekday_id,descr)
VALUES
   (1, 'Sunday'),
   (2, 'Monday'),
   (3, 'Tuesday'),
   (4, 'Wednesday'),
   (5, 'Thursday'),
   (6, 'Friday'),
   (7, 'Saturday');
SET IDENTITY_INSERT WeekDay OFF;

-------------------------------------------
SELECT  *
FROM    WeekDay;
-------------------------------------------

*/

/* BookingType

CREATE TABLE BookingType
(
 booking_type_id    int  not null PRIMARY KEY identity,
 descr              varchar(50) not null
);

SET IDENTITY_INSERT BookingType ON;
INSERT BookingType
   (booking_type_id,descr)
VALUES
   (34, 'Patient or Aged Care booking'),
   (35, 'Staff Visit'),
   (36, 'Paid Break'),
   (340,'Facility Unavailable'),              -- org unavailable - [whole day/days] - bookings can be made for clinics but after a warning message
   (341,'Provider-Facility Unavailability'),  -- provider unavailable but only at a clinic [specific hr-to-hr]
   (342,'Provider Unavailability');           -- provider unavailable [specific hr-to-hr]
SET IDENTITY_INSERT BookingType OFF;

-------------------------------------------
SELECT  *
FROM    BookingType;
-------------------------------------------

*/

/* BookingStatus

CREATE TABLE BookingStatus
(
 booking_status_id    int  not null PRIMARY KEY identity,
 descr              varchar(50) not null
);

SET IDENTITY_INSERT BookingStatus ON;
INSERT BookingStatus
   (booking_status_id,descr)
VALUES
   (-1, 'Deleted'),
   (0,  'InComplete'),
   (187,'Completed'),
   (188,'Cancelled'),
   (189,'Deceased');
SET IDENTITY_INSERT BookingStatus OFF;

-------------------------------------------
SELECT  *
FROM    BookingStatus;
-------------------------------------------

*/

/* BookingUnavailabilityReasonType

CREATE TABLE BookingUnavailabilityReasonType
(
 booking_unavailability_reason_type_id    int         not null PRIMARY KEY identity,
 descr                                    varchar(50) not null
);

SET IDENTITY_INSERT BookingUnavailabilityReasonType ON;
INSERT BookingUnavailabilityReasonType
   (booking_unavailability_reason_type_id,descr)
VALUES
   (340, 'Organisation Unavailability'),
   (341, 'Provider Unavailability');
SET IDENTITY_INSERT BookingUnavailabilityReasonType  OFF;

*/

/* BookingConfirmedByType

CREATE TABLE BookingConfirmedByType
(
 booking_confirmed_by_type_id    int  not null PRIMARY KEY identity,
 descr                           varchar(50) not null
);

SET IDENTITY_INSERT BookingConfirmedByType ON;
INSERT BookingConfirmedByType
   (booking_confirmed_by_type_id,descr)
VALUES
   (1, 'Staff'),
   (2, 'SMS'),
   (3, 'Email');
SET IDENTITY_INSERT BookingConfirmedByType OFF;

*/

/* BookingUnavailabilityReason  (ref: BookingUnavailabilityReasonType)

CREATE TABLE BookingUnavailabilityReason
(
 booking_unavailability_reason_id      int         not null PRIMARY KEY identity,
 booking_unavailability_reason_type_id int         not null FOREIGN KEY REFERENCES BookingUnavailabilityReasonType(booking_unavailability_reason_type_id),
 descr                                 varchar(50) not null
);

*/

/* Booking  (ref: Entity, Organisation, Staff, Patient, Offering, BookingType, BookingStatus, BookingUnavailabilityReason, BookingConfirmedByType, WeekDay)

CREATE TABLE Booking
(
 booking_id           int           not null PRIMARY KEY identity,
 entity_id            int           not null FOREIGN KEY REFERENCES Entity(entity_id) UNIQUE,
 date_start           datetime      not null,
 date_end             datetime,                                                    -- can be null for recurring bookings

 organisation_id      int                    FOREIGN KEY REFERENCES Organisation(organisation_id),     -- can be null for type 341 prov unavail days
 provider             int                    FOREIGN KEY REFERENCES Staff(staff_id),                   -- can be null for type 340 org unavail days
 patient_id           int                    FOREIGN KEY REFERENCES Patient(patient_id) DEFAULT null,  -- null for aged care, or patient id for clinic bookind
 offering_id          int                    FOREIGN KEY REFERENCES Offering(offering_id),
 booking_type_id      int           not null FOREIGN KEY REFERENCES BookingType(booking_type_id),
 booking_status_id    int                    FOREIGN KEY REFERENCES BookingStatus(booking_status_id),                       -- used for 34 bookings
 booking_unavailability_reason_id int        FOREIGN KEY REFERENCES BookingUnavailabilityReason(booking_unavailability_reason_id),  -- used for unavailable 340/341 bookings

 added_by                      int           FOREIGN KEY REFERENCES Staff(staff_id),
 date_created                  datetime      DEFAULT (GETDATE()),
 booking_confirmed_by_type_id  int           FOREIGN KEY REFERENCES BookingConfirmedByType(booking_confirmed_by_type_id)  DEFAULT null, 
 confirmed_by                  int           FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,              -- if unset, can be unconfirmed OR "confirmed by auto sms/email reminder"
 date_confirmed                datetime      DEFAULT null,                                                      -- so go by this: is confirmed if this is set, else unset
 deleted_by                    int           FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,
 date_deleted                  datetime      DEFAULT null,
 cancelled_by                  int           FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,
 date_cancelled                datetime      DEFAULT null,

 is_patient_missed_appt        bit  not null,
 is_provider_missed_appt       bit  not null,
 is_emergency                  bit  not null,

 need_to_generate_first_letter bit  not null,
 need_to_generate_last_letter  bit  not null,
 has_generated_system_letters  bit  not null,  -- need for if need to generate treatment letters also

 arrival_time         datetime              default null,
 sterilisation_code   varchar(200) not null,
 informed_consent_added_by     int           FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,
 informed_consent_date         datetime      DEFAULT null,

 is_recurring         bit           not null,                                     -- date_start/date_end is when reocurring booking start and ends
 recurring_weekday_id int           FOREIGN KEY REFERENCES WeekDay(weekday_id),   -- in Booking class, use object DayOfWeek
 recurring_start_time time,
 recurring_end_time   time,                                                       -- in Booking class, use Timespan ... inserting : '00:00:00'
);

*/

/* uspInsertBooking

--------------------------------------

DROP PROCEDURE uspInsertBooking;

CREATE PROCEDURE uspInsertBooking 

     @booking_id           int
    ,@date_start           datetime
    ,@date_end             datetime

    ,@organisation_id      int
    ,@provider             int
    ,@patient_id           int
    ,@offering_id          int
    ,@booking_type_id      int

    ,@booking_status_id                int
    ,@booking_unavailability_reason_id int


    ,@added_by             int     
    ,@date_created         datetime
    ,@confirmed_by         int     
    ,@date_confirmed       datetime
    ,@deleted_by           int     
    ,@date_deleted         datetime

    ,@is_patient_missed_appt        bit  
    ,@is_provider_missed_appt       bit  
    ,@is_emergency                  bit     

    ,@need_to_generate_first_letter bit     
    ,@need_to_generate_last_letter  bit     
    ,@has_generated_system_letters  bit     

    ,@arrival_time         datetime

    ,@is_recurring         bit     
    ,@recurring_weekday_id int     
    ,@recurring_start_time time 
    ,@recurring_end_time   time    



AS

Declare @entity_id int
Declare @booking_confirmed_by_type_id int

BEGIN TRAN


    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()



    SET @booking_confirmed_by_type_id = NULL
    IF (@confirmed_by IS NOT NULL) OR (@date_confirmed IS NOT NULL)
    BEGIN
        SET @booking_confirmed_by_type_id = 1
    END



    SET IDENTITY_INSERT Booking ON

    INSERT INTO Booking (booking_id,entity_id,date_start,date_end
                        ,organisation_id,provider,patient_id,offering_id,booking_type_id,booking_status_id,booking_unavailability_reason_id
                        ,added_by,date_created,booking_confirmed_by_type_id,confirmed_by,date_confirmed,deleted_by,date_deleted
                        ,is_patient_missed_appt,is_provider_missed_appt,is_emergency
                        ,need_to_generate_first_letter,need_to_generate_last_letter,has_generated_system_letters
                        ,arrival_time
                        ,is_recurring,recurring_weekday_id,recurring_start_time,recurring_end_time)
    VALUES
    (
     @booking_id
    ,@entity_id
    ,@date_start
    ,@date_end

    ,@organisation_id
    ,@provider
    ,@patient_id
    ,@offering_id
    ,@booking_type_id

    ,@booking_status_id
    ,@booking_unavailability_reason_id

    ,@added_by
    ,@date_created
    ,@booking_confirmed_by_type_id
    ,@confirmed_by
    ,@date_confirmed
    ,@deleted_by    
    ,@date_deleted  

    ,@is_patient_missed_appt  
    ,@is_provider_missed_appt 
    ,@is_emergency         

    ,@need_to_generate_first_letter
    ,@need_to_generate_last_letter
    ,@has_generated_system_letters

    ,@arrival_time         

    ,@is_recurring         
    ,@recurring_weekday_id 
    ,@recurring_start_time 
    ,@recurring_end_time   
    )
 
    SET IDENTITY_INSERT Booking OFF


COMMIT TRAN

--------------------------------------

 */

/* uspInsertBooking_Rise

--------------------------------------

--DROP PROCEDURE uspInsertBooking_Rise;

CREATE PROCEDURE uspInsertBooking_Rise

     @booking_id           int
    ,@date_start           datetime
    ,@date_end             datetime


    ,@organisation_id      int
    ,@organisation_name    varchar(100)  -- only if org_id = 0
    ,@organisation_type_id int           -- only if org_id = 0

    ,@provider             int
    ,@provider_title       varchar(100)  -- only if provider = 0
    ,@provider_firstname   varchar(100)  -- only if provider = 0
    ,@provider_surname     varchar(100)  -- only if provider = 0
    ,@provider_nbr         varchar(100)  -- only if provider = 0

    ,@patient_id           int
    ,@patient_title        varchar(100)  -- only if patient_id = 0
    ,@patient_firstname    varchar(100)  -- only if patient_id = 0
    ,@patient_surname      varchar(100)  -- only if patient_id = 0


    ,@offering_id          int
    ,@booking_type_id      int

    ,@booking_status_id                int
    ,@booking_unavailability_reason_id int


    ,@added_by             int     
    ,@date_created         datetime
    ,@confirmed_by         int     
    ,@date_confirmed       datetime
    ,@deleted_by           int     
    ,@date_deleted         datetime

    ,@is_patient_missed_appt        bit  
    ,@is_provider_missed_appt       bit  
    ,@is_emergency                  bit     

    ,@need_to_generate_first_letter bit     
    ,@need_to_generate_last_letter  bit     
    ,@has_generated_system_letters  bit     

    ,@arrival_time         datetime

    ,@is_recurring         bit     
    ,@recurring_weekday_id int     
    ,@recurring_start_time time 
    ,@recurring_end_time   time    



AS

Declare @entity_id  int
Declare @booking_confirmed_by_type_id int
Declare @staff_position_id int
Declare @pt_count int

BEGIN TRAN


    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()


    SET @booking_confirmed_by_type_id = NULL
    IF (@confirmed_by IS NOT NULL) OR (@date_confirmed IS NOT NULL)
    BEGIN
        SET @booking_confirmed_by_type_id = 1
    END


	IF @patient_id = 0
	BEGIN
		SET @pt_count = (SELECT COUNT(patient_id) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @patient_firstname AND surname = @patient_surname)
		
		IF @pt_count = 0
		BEGIN
			RAISERROR('Unknown PT: @patient_firstname = %d @patient_firstname = %d', 16, 1, @patient_firstname, @patient_surname)
		END
		IF @pt_count > 1
		BEGIN
			RAISERROR('Multiple PTs Found: @patient_firstname = %d @patient_firstname = %d', 16, 1, @patient_firstname, @patient_surname)
		END
	END
	SET @patient_id = (SELECT TOP 1 patient_id FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @patient_firstname AND surname = @patient_surname)

    IF @organisation_id = 0
    BEGIN

		SET @organisation_id = (SELECT TOP 1 organisation_id FROM Organisation WHERE name = @organisation_name AND organisation_type_id = @organisation_type_id AND is_deleted = 0)
		IF @organisation_id IS NULL
		BEGIN
		
                -- get new org id (as proceedure already adds with id)
                SET @organisation_id = (SELECT MAX(organisation_id) from Organisation) + 1

				EXEC uspInsertOrganisation 
					
					 @organisation_id               = @organisation_id
					,@parent_organisation_id        = NULL
					,@organisation_type_id          = @organisation_type_id
					,@organisation_customer_type_id = 0
					,@name                          = @organisation_name
					,@acn                           = ''
					,@abn                           = ''
					,@organisation_date_added       = GETDATE
					,@organisation_date_modified    = NULL
					,@is_debtor                     = 0
					,@is_creditor                   = 0
					,@bpay_account                  = ''
					,@is_deleted                    = 0
					
					,@weeks_per_service_cycle       = 0
					,@start_date                    = GETDATE
					,@end_date                      = NULL
					,@comment                       = ''
					,@free_services                 = 0
					,@excl_sun                      = 0
					,@excl_mon                      = 0
					,@excl_tue                      = 0
					,@excl_wed                      = 0
					,@excl_thu                      = 0
					,@excl_fri                      = 0
					,@excl_sat                      = 0
					,@sun_start_time                = '08:00:00'
					,@sun_end_time                  = '18:00:00'
					,@mon_start_time                = '08:00:00'
					,@mon_end_time                  = '18:00:00'
					,@tue_start_time                = '08:00:00'
					,@tue_end_time                  = '18:00:00'
					,@wed_start_time                = '08:00:00'
					,@wed_end_time                  = '18:00:00'
					,@thu_start_time                = '08:00:00'
					,@thu_end_time                  = '18:00:00'
					,@fri_start_time                = '08:00:00'
					,@fri_end_time                  = '18:00:00'
					,@sat_start_time                = '08:00:00'
					,@sat_end_time                  = '18:00:00'
					,@sun_lunch_start_time          = '00:00:00'
					,@sun_lunch_end_time            = '00:00:00'
					,@mon_lunch_start_time          = '00:00:00'
					,@mon_lunch_end_time            = '00:00:00'
					,@tue_lunch_start_time          = '00:00:00'
					,@tue_lunch_end_time            = '00:00:00'
					,@wed_lunch_start_time          = '00:00:00'
					,@wed_lunch_end_time            = '00:00:00'
					,@thu_lunch_start_time          = '00:00:00'
					,@thu_lunch_end_time            = '00:00:00'
					,@fri_lunch_start_time          = '00:00:00'
					,@fri_lunch_end_time            = '00:00:00'
					,@sat_lunch_start_time          = '00:00:00'
					,@sat_lunch_end_time            = '00:00:00'
					,@last_batch_run                = NULL
        END
		
    END
    
    IF @provider = 0
    BEGIN

		SET @provider = (SELECT TOP 1 staff_id 
						 FROM Staff 
							  LEFT JOIN Person ON Staff.person_id = Person.person_id 
						 WHERE firstname = @provider_firstname AND surname = @provider_surname AND is_fired = 0)
				
		IF @provider IS NULL
		BEGIN
		
                SET @staff_position_id = (SELECT top 1 staff_position_id from StaffPosition where descr = 'Unknown')

                -- get new id (as proceedure already adds with id)
                SET @provider = (SELECT MAX(staff_id) from Staff) + 1


				EXEC uspInsertStaff

					 @added_by             = NULL
					,@title_id             = 0
					,@firstname            = @provider_firstname
					,@middlename           = ''
					,@surname              = @provider_surname
					,@nickname             = ''
					,@gender               = ''
					,@dob                  = NULL
					,@person_date_added    = GETDATE
					,@person_date_modified = NULL
					,@staff_id             = @provider
					,@login                = ''
					,@pwd                  = ''
					,@staff_position_id    = @staff_position_id
					,@field_id             = 313
					,@costcentre_id        = 59
					,@is_contractor        = 0
					,@tfn                  = ''
					,@provider_number      = @provider_nbr
					,@is_fired             = 0
					,@is_commission        = 0
					,@commission_percent   = 0
					,@is_stakeholder       = 0
					,@is_principal         = 0
					,@is_admin             = 0
					,@is_provider          = 1
					,@staff_date_added     = 0
					,@start_date           = GETDATE
					,@end_date             = NULL
					,@comment              = ''
        END
		
    END


    IF (@booking_id = 0)
    BEGIN

        INSERT INTO Booking (entity_id,date_start,date_end
                            ,organisation_id,provider,patient_id,offering_id,booking_type_id,booking_status_id,booking_unavailability_reason_id
                            ,added_by,date_created,booking_confirmed_by_type_id,confirmed_by,date_confirmed,deleted_by,date_deleted
                            ,is_patient_missed_appt,is_provider_missed_appt,is_emergency
                            ,need_to_generate_first_letter,need_to_generate_last_letter,has_generated_system_letters
                            ,arrival_time
                            ,is_recurring,recurring_weekday_id,recurring_start_time,recurring_end_time)
        VALUES
        (
         @entity_id
        ,@date_start
        ,@date_end

        ,@organisation_id
        ,@provider
        ,@patient_id
        ,@offering_id
        ,@booking_type_id

        ,@booking_status_id
        ,@booking_unavailability_reason_id

        ,@added_by
        ,@date_created
        ,@booking_confirmed_by_type_id
        ,@confirmed_by
        ,@date_confirmed
        ,@deleted_by    
        ,@date_deleted  

        ,@is_patient_missed_appt  
        ,@is_provider_missed_appt 
        ,@is_emergency         

        ,@need_to_generate_first_letter
        ,@need_to_generate_last_letter
        ,@has_generated_system_letters

        ,@arrival_time         

        ,@is_recurring         
        ,@recurring_weekday_id 
        ,@recurring_start_time 
        ,@recurring_end_time   
        )
 
        SET IDENTITY_INSERT Booking OFF

    END

    IF (@booking_id <> 0)
    BEGIN

        SET IDENTITY_INSERT Booking ON

        INSERT INTO Booking (booking_id,entity_id,date_start,date_end
                            ,organisation_id,provider,patient_id,offering_id,booking_type_id,booking_status_id,booking_unavailability_reason_id
                            ,added_by,date_created,booking_confirmed_by_type_id,confirmed_by,date_confirmed,deleted_by,date_deleted
                            ,is_patient_missed_appt,is_provider_missed_appt,is_emergency
                            ,need_to_generate_first_letter,need_to_generate_last_letter,has_generated_system_letters
                            ,arrival_time
                            ,is_recurring,recurring_weekday_id,recurring_start_time,recurring_end_time)
        VALUES
        (
         @booking_id
        ,@entity_id
        ,@date_start
        ,@date_end

        ,@organisation_id
        ,@provider
        ,@patient_id
        ,@offering_id
        ,@booking_type_id

        ,@booking_status_id
        ,@booking_unavailability_reason_id

        ,@added_by
        ,@date_created
        ,@booking_confirmed_by_type_id
        ,@confirmed_by
        ,@date_confirmed
        ,@deleted_by    
        ,@date_deleted  

        ,@is_patient_missed_appt  
        ,@is_provider_missed_appt 
        ,@is_emergency         

        ,@need_to_generate_first_letter
        ,@need_to_generate_last_letter
        ,@has_generated_system_letters

        ,@arrival_time         

        ,@is_recurring         
        ,@recurring_weekday_id 
        ,@recurring_start_time 
        ,@recurring_end_time   
        )
 
        SET IDENTITY_INSERT Booking OFF

    END


COMMIT TRAN


--------------------------------------

 */


/* BookingChangeHistoryReason

CREATE TABLE BookingChangeHistoryReason
(
 booking_change_history_reason_id    int         not null PRIMARY KEY identity,
 descr                               varchar(50) not null,
 display_order                       int         not null
);

*/

/* BookingChangeHistory  (ref: Booking, Staff. BookingChangeHistoryReason)

CREATE TABLE BookingChangeHistory
(
 booking_change_history_id        int           not null PRIMARY KEY identity,
 booking_id                       int           FOREIGN KEY REFERENCES Booking(booking_id)  not null,
 moved_by                         int           FOREIGN KEY REFERENCES Staff(staff_id)  not null,
 date_moved                       datetime      not null DEFAULT (GETDATE()),
 booking_change_history_reason_id int           FOREIGN KEY REFERENCES BookingChangeHistoryReason(booking_change_history_reason_id)  not null,
 previous_datetime                datetime      not null  -- can get original date by getting for booking_id and finding the lowest booking_change_history_id
);

*/


/* BookingPatient  (ref: Booking, Patient, Staff)

CREATE TABLE BookingPatient
(
 booking_patient_id            int          NOT NULL PRIMARY KEY identity,
 booking_id                    int          FOREIGN KEY REFERENCES Booking(booking_id) NOT NULL,
 patient_id                    int          FOREIGN KEY REFERENCES Patient(patient_id) NOT NULL,
 entity_id                     int          FOREIGN KEY REFERENCES Entity(entity_id) NOT NULL,  -- used to attach notes to patients in a group booking 

 added_by                      int          FOREIGN KEY REFERENCES Staff(staff_id),
 added_date                    datetime,

 is_deleted                    bit          NOT NULL,
 deleted_by                    int          FOREIGN KEY REFERENCES Staff(staff_id),
 deleted_date                  datetime,

 -- used for generating system letters for aged care - fields identical to in the Booking table for the same purpose
 need_to_generate_first_letter bit  not null,
 need_to_generate_last_letter  bit  not null,
 has_generated_system_letters  bit  not null,  -- need for if need to generate treatment letters also

  -- main service of treatment billed for - null in AC as attached to the PT already, but can not be null in Group Bks
  -- needed to distinguish what medicare is paying for in list of items to invoice a pt
  offering_id                  int          FOREIGN KEY REFERENCES Offering(offering_id) NOT NULL,
  area_treated                 varchar(500) not null,
);

*/

/* BookingPatientOffering  (ref: BookingPatient, Offering, Staff)  -- used only for "additional offerings" beyond main one that is attached to the patient

CREATE TABLE BookingPatientOffering
(
 booking_patient_offering_id int          NOT NULL PRIMARY KEY identity,
 booking_patient_id          int          FOREIGN KEY REFERENCES BookingPatient(booking_patient_id) NOT NULL,

 offering_id                 int          FOREIGN KEY REFERENCES Offering(offering_id) NOT NULL,
 quantity                    int          NOT NULL,

 added_by                    int          FOREIGN KEY REFERENCES Staff(staff_id),
 added_date                  datetime,

 is_deleted                  bit          NOT NULL,
 deleted_by                  int          FOREIGN KEY REFERENCES Staff(staff_id),
 deleted_date                datetime,
 area_treated                varchar(500) not null,
);

*/



/* NoteType

CREATE TABLE NoteType
(
 note_type_id    int         not null PRIMARY KEY identity,
 descr           varchar(50) not null
);

SET IDENTITY_INSERT NoteType ON;
INSERT NoteType
   (note_type_id,descr)
VALUES
   (1,   'Medication'),
   (2,   'Medical Condition');
   (48,  'Problem'),
   (49,  'Compliment'),
   (50,  'Contact Made'),
   (51,  'Admin Notes Only'),
   (190, 'Referral Message'),
   (209, 'Patient List - Invoice'),
   (210, 'Receipt Message'),
   (211, 'Credit Note Message'),
   (223, 'Parking'),
   (225, 'Point of Sale'),
   (228, 'Refund Message'),
   (252, 'Provider Note'),
   (253, 'Facility Patient List'),
   (254, 'Patient List - Medicare Med Cond');
SET IDENTITY_INSERT NoteType OFF;

-------------------------------------------
SELECT  *
FROM    NoteType;
-------------------------------------------

*/

/* BodyPart

CREATE TABLE BodyPart
(
 body_part_id    int         not null PRIMARY KEY identity,
 descr           varchar(50) not null
);

INSERT INTO BodyPart (descr)
VALUES 
('Face'),
('Neck'),
('Thyroid'),
('Left Chest'),
('Right Chest'),
('Spleen'),
('Liver'),
('Stomach'),
('Abdomen'),
('Pelvis'),
('Genitals'),
('Left Thigh'),
('Right Thigh'),
('Left Shin'),
('Right Shin'),
('Left Arm'),
('Right Arm'),
('Back Of Head'),
('Nape'),
('Upper Back'),
('Middle Back'),
('Lower Back'),
('Back Of Left Thigh'),
('Back Of Right Thigh'),
('Left Calf'),
('Right Calf'),
('Left Foot'),
('Right Foot'),
('Left Shoulder'),
('Right Shoulder'),
('Left Wrist'),
('Right Wrist'),
('Left Hand'),
('Right Hand'),
('Left Fingers'),
('Right Fingers'),
('Left Ankle'),
('Right Ankle'),
('Left Elbow'),
('Right Elbow'),
('Left Knee'),
('Right Knee');


*/

/* Note  (ref: Entity, NoteType, BodyPart, Staff, Site)

CREATE TABLE Note
(
 note_id             int           not null PRIMARY KEY identity,
 entity_id           int           not null FOREIGN KEY REFERENCES Entity(entity_id),
 note_type_id        int           not null FOREIGN KEY REFERENCES NoteType(note_type_id),
 body_part_id        int                    FOREIGN KEY REFERENCES BodyPart(body_part_id) DEFAULT NULL,
 text                varchar(max)  not null,
 date_added          datetime      not null DEFAULT (GETDATE()),
 date_modified       datetime               DEFAULT null,
 date_deleted        datetime               DEFAULT null,
 added_by            int                    FOREIGN KEY REFERENCES Staff(staff_id),
 modified_by         int                    FOREIGN KEY REFERENCES Staff(staff_id),
 deleted_by          int                    FOREIGN KEY REFERENCES Staff(staff_id),
 site_id             int                    FOREIGN KEY REFERENCES Site(site_id),        -- 0/1=ac, 2=clinic -- only patients else null => to know is note for a patient is cilnic note or ag note
                                                                                -- nb: a note for a "site" will have site id as entityid
);

*/

/* NoteHistory  (ref: Note, NoteType, BodyPart, Staff, Site)

CREATE TABLE NoteHistory
(
 note_history_id     int           not null PRIMARY KEY identity,
 note_id             int           not null FOREIGN KEY REFERENCES Note(note_id),    -- links back to note (entity left out as it's not editable)
 note_type_id        int           not null FOREIGN KEY REFERENCES NoteType(note_type_id),
 body_part_id        int                    FOREIGN KEY REFERENCES BodyPart(body_part_id) DEFAULT NULL,
 text                varchar(max)  not null,
 date_added          datetime      not null DEFAULT (GETDATE()),
 date_modified       datetime               DEFAULT null,
 date_deleted        datetime               DEFAULT null,
 added_by            int                    FOREIGN KEY REFERENCES Staff(staff_id),
 modified_by         int                    FOREIGN KEY REFERENCES Staff(staff_id),
 deleted_by          int                    FOREIGN KEY REFERENCES Staff(staff_id),
 site_id             int                    FOREIGN KEY REFERENCES Site(site_id),
);

*/

/* uspInsertNote

--------------------------------------

DROP PROCEDURE uspInsertNote;

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

--------------------------------------

 */

/* uspInsertBooking_Rise

--------------------------------------

--DROP PROCEDURE uspInsertBooking_Rise;

CREATE PROCEDURE uspInsertBooking_Rise

     @booking_id           int
    ,@date_start           datetime
    ,@date_end             datetime


    ,@organisation_id      int
    ,@organisation_name    varchar(100)  -- only if org_id = 0
    ,@organisation_type_id int           -- only if org_id = 0

    ,@provider             int
    ,@provider_title       varchar(100)  -- only if provider = 0
    ,@provider_firstname   varchar(100)  -- only if provider = 0
    ,@provider_surname     varchar(100)  -- only if provider = 0
    ,@provider_nbr         varchar(100)  -- only if provider = 0

    ,@patient_id           int
    ,@patient_title        varchar(100)  -- only if patient_id = 0
    ,@patient_firstname    varchar(100)  -- only if patient_id = 0
    ,@patient_surname      varchar(100)  -- only if patient_id = 0


    ,@offering_id          int
    ,@booking_type_id      int

    ,@booking_status_id                int
    ,@booking_unavailability_reason_id int


    ,@added_by             int     
    ,@date_created         datetime
    ,@confirmed_by         int     
    ,@date_confirmed       datetime
    ,@deleted_by           int     
    ,@date_deleted         datetime

    ,@is_patient_missed_appt        bit  
    ,@is_provider_missed_appt       bit  
    ,@is_emergency                  bit     

    ,@need_to_generate_first_letter bit     
    ,@need_to_generate_last_letter  bit     
    ,@has_generated_system_letters  bit     

    ,@arrival_time         datetime

    ,@is_recurring         bit     
    ,@recurring_weekday_id int     
    ,@recurring_start_time time 
    ,@recurring_end_time   time    



AS

Declare @entity_id  int
Declare @booking_confirmed_by_type_id int
Declare @staff_position_id int
Declare @pt_count int

BEGIN TRAN


    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()


    SET @booking_confirmed_by_type_id = NULL
    IF (@confirmed_by IS NOT NULL) OR (@date_confirmed IS NOT NULL)
    BEGIN
        SET @booking_confirmed_by_type_id = 1
    END


	IF @patient_id = 0
	BEGIN
		SET @pt_count = (SELECT COUNT(patient_id) FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @patient_firstname AND surname = @patient_surname)
		
		IF @pt_count = 0
		BEGIN
			RAISERROR('Unknown PT: @patient_firstname = %d @patient_firstname = %d', 16, 1, @patient_firstname, @patient_surname)
		END
		IF @pt_count > 1
		BEGIN
			RAISERROR('Multiple PTs Found: @patient_firstname = %d @patient_firstname = %d', 16, 1, @patient_firstname, @patient_surname)
		END
	END
	SET @patient_id = (SELECT TOP 1 patient_id FROM Patient LEFT JOIN Person ON Patient.person_id = Person.person_id WHERE firstname = @patient_firstname AND surname = @patient_surname)

    IF @organisation_id = 0
    BEGIN

		SET @organisation_id = (SELECT TOP 1 organisation_id FROM Organisation WHERE name = @organisation_name AND organisation_type_id = @organisation_type_id AND is_deleted = 0)
		IF @organisation_id IS NULL
		BEGIN
		
                -- get new org id (as proceedure already adds with id)
                SET @organisation_id = (SELECT MAX(organisation_id) from Organisation) + 1

				EXEC uspInsertOrganisation 
					
					 @organisation_id               = @organisation_id
					,@parent_organisation_id        = NULL
					,@organisation_type_id          = @organisation_type_id
					,@organisation_customer_type_id = 0
					,@name                          = @organisation_name
					,@acn                           = ''
					,@abn                           = ''
					,@organisation_date_added       = GETDATE
					,@organisation_date_modified    = NULL
					,@is_debtor                     = 0
					,@is_creditor                   = 0
					,@bpay_account                  = ''
					,@is_deleted                    = 0
					
					,@weeks_per_service_cycle       = 0
					,@start_date                    = GETDATE
					,@end_date                      = NULL
					,@comment                       = ''
					,@free_services                 = 0
					,@excl_sun                      = 0
					,@excl_mon                      = 0
					,@excl_tue                      = 0
					,@excl_wed                      = 0
					,@excl_thu                      = 0
					,@excl_fri                      = 0
					,@excl_sat                      = 0
					,@sun_start_time                = '08:00:00'
					,@sun_end_time                  = '18:00:00'
					,@mon_start_time                = '08:00:00'
					,@mon_end_time                  = '18:00:00'
					,@tue_start_time                = '08:00:00'
					,@tue_end_time                  = '18:00:00'
					,@wed_start_time                = '08:00:00'
					,@wed_end_time                  = '18:00:00'
					,@thu_start_time                = '08:00:00'
					,@thu_end_time                  = '18:00:00'
					,@fri_start_time                = '08:00:00'
					,@fri_end_time                  = '18:00:00'
					,@sat_start_time                = '08:00:00'
					,@sat_end_time                  = '18:00:00'
					,@sun_lunch_start_time          = '00:00:00'
					,@sun_lunch_end_time            = '00:00:00'
					,@mon_lunch_start_time          = '00:00:00'
					,@mon_lunch_end_time            = '00:00:00'
					,@tue_lunch_start_time          = '00:00:00'
					,@tue_lunch_end_time            = '00:00:00'
					,@wed_lunch_start_time          = '00:00:00'
					,@wed_lunch_end_time            = '00:00:00'
					,@thu_lunch_start_time          = '00:00:00'
					,@thu_lunch_end_time            = '00:00:00'
					,@fri_lunch_start_time          = '00:00:00'
					,@fri_lunch_end_time            = '00:00:00'
					,@sat_lunch_start_time          = '00:00:00'
					,@sat_lunch_end_time            = '00:00:00'
					,@last_batch_run                = NULL
        END
		
    END
    
    IF @provider = 0
    BEGIN

		SET @provider = (SELECT TOP 1 staff_id 
						 FROM Staff 
							  LEFT JOIN Person ON Staff.person_id = Person.person_id 
						 WHERE firstname = @provider_firstname AND surname = @provider_surname AND is_fired = 0)
				
		IF @provider IS NULL
		BEGIN
		
                SET @staff_position_id = (SELECT top 1 staff_position_id from StaffPosition where descr = 'Unknown')

                -- get new id (as proceedure already adds with id)
                SET @provider = (SELECT MAX(staff_id) from Staff) + 1


				EXEC uspInsertStaff

					 @added_by             = NULL
					,@title_id             = 0
					,@firstname            = @provider_firstname
					,@middlename           = ''
					,@surname              = @provider_surname
					,@nickname             = ''
					,@gender               = ''
					,@dob                  = NULL
					,@person_date_added    = GETDATE
					,@person_date_modified = NULL
					,@staff_id             = @provider
					,@login                = ''
					,@pwd                  = ''
					,@staff_position_id    = @staff_position_id
					,@field_id             = 313
					,@costcentre_id        = 59
					,@is_contractor        = 0
					,@tfn                  = ''
					,@provider_number      = @provider_nbr
					,@is_fired             = 0
					,@is_commission        = 0
					,@commission_percent   = 0
					,@is_stakeholder       = 0
					,@is_principal         = 0
					,@is_admin             = 0
					,@is_provider          = 1
					,@staff_date_added     = 0
					,@start_date           = GETDATE
					,@end_date             = NULL
					,@comment              = ''
        END
		
    END


    IF (@booking_id = 0)
    BEGIN

        INSERT INTO Booking (entity_id,date_start,date_end
                            ,organisation_id,provider,patient_id,offering_id,booking_type_id,booking_status_id,booking_unavailability_reason_id
                            ,added_by,date_created,booking_confirmed_by_type_id,confirmed_by,date_confirmed,deleted_by,date_deleted
                            ,is_patient_missed_appt,is_provider_missed_appt,is_emergency
                            ,need_to_generate_first_letter,need_to_generate_last_letter,has_generated_system_letters
                            ,arrival_time
                            ,is_recurring,recurring_weekday_id,recurring_start_time,recurring_end_time)
        VALUES
        (
         @entity_id
        ,@date_start
        ,@date_end

        ,@organisation_id
        ,@provider
        ,@patient_id
        ,@offering_id
        ,@booking_type_id

        ,@booking_status_id
        ,@booking_unavailability_reason_id

        ,@added_by
        ,@date_created
        ,@booking_confirmed_by_type_id
        ,@confirmed_by
        ,@date_confirmed
        ,@deleted_by    
        ,@date_deleted  

        ,@is_patient_missed_appt  
        ,@is_provider_missed_appt 
        ,@is_emergency         

        ,@need_to_generate_first_letter
        ,@need_to_generate_last_letter
        ,@has_generated_system_letters

        ,@arrival_time         

        ,@is_recurring         
        ,@recurring_weekday_id 
        ,@recurring_start_time 
        ,@recurring_end_time   
        )
 
        SET IDENTITY_INSERT Booking OFF

    END

    IF (@booking_id <> 0)
    BEGIN

        SET IDENTITY_INSERT Booking ON

        INSERT INTO Booking (booking_id,entity_id,date_start,date_end
                            ,organisation_id,provider,patient_id,offering_id,booking_type_id,booking_status_id,booking_unavailability_reason_id
                            ,added_by,date_created,booking_confirmed_by_type_id,confirmed_by,date_confirmed,deleted_by,date_deleted
                            ,is_patient_missed_appt,is_provider_missed_appt,is_emergency
                            ,need_to_generate_first_letter,need_to_generate_last_letter,has_generated_system_letters
                            ,arrival_time
                            ,is_recurring,recurring_weekday_id,recurring_start_time,recurring_end_time)
        VALUES
        (
         @booking_id
        ,@entity_id
        ,@date_start
        ,@date_end

        ,@organisation_id
        ,@provider
        ,@patient_id
        ,@offering_id
        ,@booking_type_id

        ,@booking_status_id
        ,@booking_unavailability_reason_id

        ,@added_by
        ,@date_created
        ,@booking_confirmed_by_type_id
        ,@confirmed_by
        ,@date_confirmed
        ,@deleted_by    
        ,@date_deleted  

        ,@is_patient_missed_appt  
        ,@is_provider_missed_appt 
        ,@is_emergency         

        ,@need_to_generate_first_letter
        ,@need_to_generate_last_letter
        ,@has_generated_system_letters

        ,@arrival_time         

        ,@is_recurring         
        ,@recurring_weekday_id 
        ,@recurring_start_time 
        ,@recurring_end_time   
        )
 
        SET IDENTITY_INSERT Booking OFF

    END


COMMIT TRAN

*/



/* Screen

CREATE TABLE Screen
(
 screen_id          int  not null PRIMARY KEY identity,
 descr              varchar(50) not null
);

SET IDENTITY_INSERT Screen ON;
INSERT Screen
   (screen_id,descr)
VALUES
   (1,'SiteList'),
   (2,'AddEditSite'),
   (3,'StaffList'),
   (4,'AddEditStaff'),
   (5,'PatientList'),
   (6,'AddEditPatient'),
   (7,'ReferrerList'),
   (8,'AddEditReferrer'),
   (9,'OrganisationList - All External'),
   (10,'AddEditOrganisatoin - All External'),
   (11,'OrganisationList - Internal Clinics'),
   (12,'AddEditOrganisatoin - Internal Clinics'),
   (13,'OrganisationList - Internal Aged Care'),
   (14,'AddEditOrganisatoin - Internal Aged Care'),
   (15,'Bookings'),
   (16,'Bookings - Body Chart'),
   (17,'Bookings - Medication'),
   (18,'Bookings - Medical Condition');
SSET IDENTITY_INSERT Screen OFF;

-------------------------------------------
SELECT  *
FROM    Screen;
-------------------------------------------

*/

/* ScreenNoteTypes  (ref: Screen, NoteType)

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

   (15, 252, 1),

   (17, 1,  1),
   (18, 2,  1);

-------------------------------------------
SELECT   *
FROM     ScreenNoteTypes
ORDER BY note_type_id
-------------------------------------------

*/




/* ****** Reversing Receipt/Adj Note ******

Reversing Receipt/Adj Note
--------------------------
if total = 0 => disallow (show msg)
if receipt reconciled = true - disallow (show msg)
if receipt linked to deposit slip - disallow (show msg)
ask for confirm (js alert)

else:
=> set total,gst= 0
=> if is receipt and has overpayment record, remove that and set isoverpayment = false
=> udpate GL (for the year when it was done]
=> update ispaid=0 for invoice

*/

/* ****** Reversing a Booking (ie reversing invoice too) ******

Reversing a Booking (ie reversing invoice too)
----------------------------------------------

if receipts sum > 0 - disallow (show msg telling them have to reverse receipts first)
  (if receipts been reconciled, then disallow removing of invoices at all and therefore reversing the invoice)
if total due > 0 tell them to create credit notes to make total due zero first (or have program auto generate it)
*** Note that when reversing credit notes or receipts, the appropriate g/l account records have to be amended and the g/l transactions deleted

if no receiepts AND total due = 0 - can reverse invoice:
=> update GL (see charles email "Reversing an Invoice")
=> set invoice as reversed
=> set booking status as not completed

*/



// ===>> DO NOT DELETE FINAICIALDOC SCHEMAS -- NEED FOR LATER

/* FinancialDocumentType

CREATE TABLE FinancialDocumentType
(
 financial_document_type_id       int  not null PRIMARY KEY identity,
 descr                            varchar(50) not null
);

SET IDENTITY_INSERT FinancialDocumentType ON;
INSERT FinancialDocumentType
   (financial_document_type_id,descr)
VALUES

   (59,  'Credit Note'),             -- just reduces the price of the invoice (marcus calls it adjustment note) -- links back to invoice, not to inv line, so dont know what was changed -- which is ok for medicare since only 1 service per inv, but dva has multiple
   (107, 'Clinic Invoice'),      - Invoice
   (108, 'Standard Invoice'),    - Invoice
   (321, 'Bank Withdrawl'),
   (326, 'Receipt'),             - Receipt (= invoice payment received)
   (329, 'Safe Deposit'),             -- simlar to till deposit (but in an actual night safe)     -- not used !!
   (330, 'Till Deposit'),             -- saying who has the money temporarily (eg from provider)  -- is used
   (357, 'Refund'),                  -- where a patient pays their bill, and then complains saying they are not happy with service
   (359, 'Overpayment'),             -- eg if medicare increases rate they pay back to rovider and so the patient then only has to pay less (ie overpaid)
   (363, 'Aged Care Invoice');   - Invoice

SET IDENTITY_INSERT FinancialDocumentType OFF;

-------------------------------------------
SELECT  *
FROM    FinancialDocumentType;
-------------------------------------------

*/

/* FinancialDocument  (ref: FinancialDocumentType, ReceiptPaymentType, ReceiptReturnReason, Organisation, Booking, Patient, Staff, Site, Note)


  -- no entity id needed becaue using a registration many-to-many table doc-entity   (since many patients for one aged care booking)
     tbl: FinancialDocumentEntityRegistration


  ===>>> AHHHHHHHHHHH ! ! !
         "xref_financial_document_id"  is when it is a credit note and needs to know which invoice it was for !!!!!!!!!!!!!!!!!!
         so change this so that you have Invoice table and CreditNote that has invoice_id   



CREATE TABLE FinancialDocument
(
 financial_document_id       int  not null PRIMARY KEY identity,
 xref_financial_document_id  int           FOREIGN KEY REFERENCES FinancialDocument(financial_document_id), -- cross ref - eg if receipt this is inv number was combied with xref_financial_document_id - split on import data

 financial_document_type_id  int  not null FOREIGN KEY REFERENCES FinancialDocumentType(financial_document_type_id),

 receipt_payment_type_id     int           FOREIGN KEY REFERENCES ReceiptPaymentType(receipt_payment_type_id),   -- was combined as 13 FD_PaymentType_Refund
 receipt_return_reason_id    int           FOREIGN KEY REFERENCES ReceiptReturnReason(receipt_return_reason_id), -- was combined as 13 FD_PaymentType_Refund


 organisation_id             int           FOREIGN KEY REFERENCES Organisation(organisation_id),   -- ** not in old table, this is the LE single row (or no row) in registration table. will need to get him to join on import
 organisation_claim_number   varchar(50)   not null DEFAULT '',


 booking_id                  int           FOREIGN KEY REFERENCES Booking(booking_id),   -- was combined as "9 FD_BookingReceipt" - split on import data
 xref_booking_id             int           FOREIGN KEY REFERENCES Booking(booking_id),   -- was combied with xref_financial_document_id - split on import data

 patient_id                  int           FOREIGN KEY REFERENCES Patient(patient_id),   -- was combined as "9 FD_BookingReceipt" - split on import data -- null for clinic booking as patient id is in booking_id; null in ac booking as is in registration table; only used for standard (non-booking) invoices


 staff_id                    int  not null FOREIGN KEY REFERENCES Staff(staff_id), 

 site_id                     int           FOREIGN KEY REFERENCES Site(site_id), 

 receipt_pos_note_id         int           FOREIGN KEY REFERENCES Note(Note_id),         -- Note record containing the POS number

 date                        datetime      not null DEFAULT (GETDATE()),
 bank_processed_date         datetime      not null DEFAULT (GETDATE()),

 total                       decimal(5,2)  not null, 

 invioce_gst                 decimal(5,2)  not null,   -- (invoices) was combined as 8 FD_GovtTaxOrReconciled
 receipt_amount_reconciled   decimal(5,2)  not null,   -- (receipts) was combined as 8 FD_GovtTaxOrReconciled

 is_invoice_paid             bit           not null,
 is_receipt_failed_to_clear  bit           not null,

 is_invoice_overpaid         bit           not null,  -- only used for invoices  -- (invoices) was combined as "15 FD_OverpaidOrRefund"  - split on import data
 is_receipt_refund           bit           not null,  -- only used for receipts  -- (receipts) was combined as "15 FD_OverpaidOrRefund"
 is_invoice_batched          bit           not null, 
);

INSERT FinancialDocument
   (xref_financial_document_id,financial_document_type_id,receipt_payment_type_id,receipt_return_reason_id,
    organisation_id,booking_id,xref_booking_id,
    patient_id,staff_id,site_id,receipt_pos_note_id,
    total,invioce_gst,receipt_amount_reconciled,
    is_invoice_paid,is_receipt_failed_to_clear,is_receipt_overpaid,is_invoice_refund,is_invoice_batched)
VALUES
    (null, 107, null, null, 
     1, null, null, 
     1, 1, 1, null,
     29.99, 2.99, 0.00,
     1, 0, 0,0,0)


-------------------------------------------
SELECT  *
FROM    FinancialDocument;
-------------------------------------------

*/

/* FinancialDocumentLine  (ref: FinancialDocument)

CREATE TABLE FinancialDocumentLine
(
 financial_document_line_id  int  not null PRIMARY KEY identity,
 financial_document_id       int           FOREIGN KEY REFERENCES FinancialDocument(financial_document_id),
 offering_id                 int           FOREIGN KEY REFERENCES Offering(offering_id),
 quantity                    decimal(5,2)  not null,
 price                       decimal(5,2)  not null, 
 tax                         decimal(5,2)  not null, 
);

INSERT FinancialDocumentLine
   (financial_document_id,offering_id,quantity,price,tax)
VALUES
    (1, 1, 1, 25.00, 0.00),
    (1, 2, 3, 25.00, 0.00);
-------------------------------------------
SELECT  *
FROM    FinancialDocumentLine;
-------------------------------------------

*/

/* Stored Proc : uspCreateClaimNumber


-- exec uspCreateClaimNumber 10;

DROP PROCEDURE uspCreateClaimNumber ;


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[uspCreateClaimNumber] @invoice_id int, @invoice_date datetime
AS

DECLARE @healthcare_claim_number nvarchar(10);

BEGIN TRAN

    SET @healthcare_claim_number = (SELECT TOP 1 claim_number FROM InvoiceHealthcareClaimNumbers WHERE is_active = 1 AND (last_date_used IS NULL OR last_date_used < DATEADD(year,-2,@invoice_date)) ORDER BY id)

    IF @healthcare_claim_number IS NULL 
    BEGIN

        RAISERROR('No claim numbers left!', 16, 1)

    END
    ELSE
    BEGIN

        IF (SELECT count(*) FROM Invoice WHERE invoice_date_added > DATEADD(year,-2,@invoice_date)  AND healthcare_claim_number = @healthcare_claim_number) = 0
        BEGIN
            UPDATE InvoiceHealthcareClaimNumbers SET last_date_used = @invoice_date WHERE claim_number = @healthcare_claim_number
            UPDATE Invoice SET healthcare_claim_number = @healthcare_claim_number WHERE invoice_id =  @invoice_id
        END
        ELSE
        BEGIN
            SET @healthcare_claim_number = NULL
            RAISERROR('Error: Claim number already in use: @new_claim_number = %d', 16, 1, @healthcare_claim_number)
        END

    END

    SELECT @healthcare_claim_number;

COMMIT TRAN

GO

*/


/* InvoiceType

CREATE TABLE InvoiceType
(
 invoice_type_id       int         not null PRIMARY KEY identity,
 descr                 varchar(50) not null
);

SET IDENTITY_INSERT InvoiceType ON;
INSERT InvoiceType
   (invoice_type_id,descr)
VALUES
   (107, 'Clinic Invoice'),  
   (108, 'Standard Invoice'),
   (363, 'Aged Care Invoice');
SET IDENTITY_INSERT InvoiceType OFF;

*/

/* Invoice  (ref: Entity, InvoiceType, Booking, Organisation, Patient, Letter, Staff, Site)

- clinic inv 107
  - has bookingid set
  - can have patientid null -> since patient id is in booking
    NOTE: 107 clinic bookings can have no payer, meaning the payer is the patient in the booking)
  - can create either or both of 2 invoice types
    - one with orgid (payer) = medicare/dva (has claim number) 
    - one with orgid (payer) = null (privately paid by patient)
  [patient id that is in booking, is also in the invoice line - dunno if its needed tho]

- standard invoice 108
  - has bookingid = null
  - has patientid set
  - orgid = org invoice made at (non-108 types this will be in booking, and this field there is as the payer of invoice)

- aged care inv 363
  - has bookingid set (for multiple ppl per booking)
  - has patientid null -> since patient id is in invoice lines, and attached to booking
  - if patient pays (hc-funded/low-care), then
      - payer_organisation_id = null
      - payer_patient_id      = patientid that pays own bill
  - if patient doesnt pays (medicare/dva/hc/lc-funded), then
      - payer_organisation_id = org that pays (medicare/dva/facility)
      - payer_patient_id      = null

-- for more notes, see UnderstandingInvoices.txt

 

CREATE TABLE Invoice
(
 invoice_id                  int           not null PRIMARY KEY identity,
 entity_id                   int           not null FOREIGN KEY REFERENCES Entity(entity_id) UNIQUE,
 invoice_type_id             int           not null FOREIGN KEY REFERENCES InvoiceType(invoice_type_id),
 booking_id                  int                    FOREIGN KEY REFERENCES Booking(booking_id),    -- was combined as "9 FD_BookingReceipt" - split on import data

 payer_organisation_id       int           FOREIGN KEY REFERENCES Organisation(organisation_id), -- payer of invoice (medicare/dva/ac-org). null = patient_id paid   ** not in old table, this is the LE single row (or no row) in registration table. will need to get him to join on import
 payer_patient_id            int           FOREIGN KEY REFERENCES Patient(patient_id),           -- payer of invoice (medicare/dva/ac-org). null = patient_id paid   ** not in old table, this is the LE single row (or no row) in registration table. will need to get him to join on import

 non_booking_invoice_organisation_id int   FOREIGN KEY REFERENCES Organisation(organisation_id), 

 healthcare_claim_number     varchar(50)   not null DEFAULT '',
 reject_letter_id            int           FOREIGN KEY REFERENCES Letter(letter_id), -- temp stores medicare rej code until new invoice recreated. only invoices with this not empty need to be re-invoiced
 message                     varchar(200)  not null DEFAULT '',

 staff_id                    int           not null FOREIGN KEY REFERENCES Staff(staff_id), 
 site_id                     int                    FOREIGN KEY REFERENCES Site(site_id), 
 invoice_date_added          datetime      not null DEFAULT (GETDATE()),
 total                       decimal(8,2)  not null, 
 gst                         decimal(8,2)  not null,
 is_paid                     bit           not null,
 is_refund                   bit           not null,                                      -- was combined as "15 FD_OverpaidOrRefund"  - split on import data
 is_batched                  bit           not null, 

 reversed_by                int                     FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT NULL,
 reversed_date              datetime                DEFAULT NULL,

 last_date_emailed          datetime                DEFAULT NULL,
);


*/

/* uspInsertInvoice

--------------------------------------

DROP PROCEDURE uspInsertInvoice;

CREATE PROCEDURE uspInsertInvoice 

     @invoice_id                  int
    ,@invoice_type_id             int
    ,@booking_id                  int
    ,@payer_organisation_id       int
    ,@payer_patient_id            int
    ,@healthcare_claim_number     varchar(50)
    ,@reject_letter_id            int
    ,@staff_id                    int  
    ,@site_id                     int         
    ,@invoice_date_added          datetime    
    ,@total                       decimal(8,2)
    ,@gst                         decimal(8,2)
    ,@is_paid                     bit
    ,@is_refund                   bit
    ,@is_batched                  bit
    ,@reversed_by                 int
    ,@reversed_date               datetime


AS

Declare @entity_id int

BEGIN TRAN


    INSERT INTO Entity DEFAULT VALUES;
    SET @entity_id = SCOPE_IDENTITY()


    SET IDENTITY_INSERT Invoice ON

    INSERT INTO Invoice (invoice_id,entity_id,invoice_type_id,booking_id
                        ,payer_organisation_id,payer_patient_id,non_booking_invoice_organisation_id,healthcare_claim_number,reject_letter_id,staff_id,site_id
                        ,invoice_date_added,total,gst,is_paid,is_refund,is_batched,reversed_by,reversed_date)
    VALUES
    (
     @invoice_id                  
    ,@entity_id                   
    ,@invoice_type_id             
    ,@booking_id                  
    ,@payer_organisation_id       
    ,@payer_patient_id            
    ,NULL
    ,@healthcare_claim_number     
    ,@reject_letter_id
    ,@staff_id                    
    ,@site_id                     
    ,@invoice_date_added          
    ,@total                       
    ,@gst                         
    ,@is_paid                     
    ,@is_refund                 
    ,@is_batched                  
    ,@reversed_by                 
    ,@reversed_date               
    )
 
    SET IDENTITY_INSERT Invoice OFF


COMMIT TRAN

--------------------------------------

 */

/* InvoiceLine  (ref: Invoice, Patient, Organisation)

CREATE TABLE InvoiceLine
(
 invoice_line_id   int  not null PRIMARY KEY identity,
 invoice_id        int  not null FOREIGN KEY REFERENCES Invoice(invoice_id),
 patient_id        int           FOREIGN KEY REFERENCES Patient(patient_id),                -- nullable
 offering_id       int           FOREIGN KEY REFERENCES Offering(offering_id),
 quantity          decimal(8,2)  not null,
 price             decimal(8,2)  not null, 
 tax               decimal(8,2)  not null, 
 area_treated      varchar(500)  not null,
 service_reference varchar(2)    not null,                                                  -- "body part / condition identifier" for insurance
 offering_order_id int           FOREIGN KEY REFERENCES OfferingOrder(offering_order_id),   -- nullable
);

*/

/* InvoiceHealthcareClaimNumbers

CREATE TABLE InvoiceHealthcareClaimNumbers
(
 id               int          not null PRIMARY KEY identity,
 claim_number     varchar(10)  not null UNIQUE,
 last_date_used   datetime     default null,                      -- null = not yet used
 is_active        bit          not null                           -- to allocate in a particular database which claim number can be used
);

*/

/* Stored Proc : uspCreateClaimNumber


-- exec uspCreateClaimNumber 10;

DROP PROCEDURE uspCreateClaimNumber;


CREATE PROCEDURE uspCreateClaimNumber @invoice_id int, @invoice_date datetime
AS

DECLARE @healthcare_claim_number nvarchar(10);

BEGIN TRAN

    SET @healthcare_claim_number = (SELECT TOP 1 claim_number FROM InvoiceHealthcareClaimNumbers WHERE is_active = 1 AND (last_date_used IS NULL OR last_date_used < DATEADD(year,-2,@invoice_date)) ORDER BY id)

    IF @healthcare_claim_number IS NULL 
    BEGIN

        RAISERROR('No claim numbers left!', 16, 1)

    END
    ELSE
    BEGIN

        IF (SELECT count(*) FROM Invoice WHERE invoice_date_added > DATEADD(year,-2,@invoice_date)  AND healthcare_claim_number = @healthcare_claim_number) = 0
        BEGIN
            UPDATE InvoiceHealthcareClaimNumbers SET last_date_used = @invoice_date WHERE claim_number = @healthcare_claim_number
            UPDATE Invoice SET healthcare_claim_number = @healthcare_claim_number WHERE invoice_id =  @invoice_id
        END
        ELSE
        BEGIN
            SET @healthcare_claim_number = NULL
            RAISERROR('Error: Claim number already in use: @new_claim_number = %d', 16, 1, @healthcare_claim_number)
        END

    END

    SELECT @healthcare_claim_number;

COMMIT TRAN

GO

*/

/* [OLD] Stored Proc : uspCreateClaimNumber


-- exec uspCreateClaimNumber 10;

DROP PROCEDURE uspCreateClaimNumber ;

CREATE PROCEDURE uspCreateClaimNumber @invoice_id int
AS


DECLARE @nbr nvarchar(20);
DECLARE @position INT

Declare @letters as char(26) ;
SET @letters= 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';


BEGIN TRAN

    IF (SELECT COUNT(*) FROM Invoice WHERE (healthcare_claim_number <> '')) = 0
    BEGIN
        SET @nbr = 'A0000';
        IF (SELECT count(*) FROM Invoice WHERE (healthcare_claim_number <> '') and invoice_date_added > DATEADD(year,-2,GETDATE())  AND healthcare_claim_number = @nbr) = 0
            UPDATE Invoice SET healthcare_claim_number = @nbr WHERE invoice_id =  @invoice_id
        ELSE
        BEGIN
            SET @nbr = NULL
            RAISERROR('Number already in use.', 16, 1)
        END
    END
    ELSE
    BEGIN
        SET @nbr = (SELECT     TOP (1) healthcare_claim_number
                    FROM       Invoice
                    WHERE      (healthcare_claim_number <> '' AND invoice_id <> @invoice_id)
                    ORDER BY   invoice_id  DESC);

        IF SUBSTRING(@nbr, 2, 4) <> '9999'
            SET @nbr = SUBSTRING(@nbr, 1, 1) + RIGHT(REPLICATE('0', 4)+CAST(  CONVERT(varchar(20),CONVERT (int,SUBSTRING(@nbr, 2, 4))+1) AS VARCHAR(4)), 4)
        ELSE IF CHAR(ASCII(SUBSTRING(@nbr, 1, 1))) = 'Z'
            SET @nbr = 'A0000';
        ELSE
            SET @nbr =  Substring(@letters,CharIndex(CHAR(ASCII(SUBSTRING(@nbr,  1, 1))) ,@letters,1)+1,1)  + '0000';


        IF (SELECT count(*) FROM Invoice WHERE (healthcare_claim_number <> '') and invoice_date_added > DATEADD(year,-2,GETDATE())  AND healthcare_claim_number = @nbr) = 0
            UPDATE Invoice SET healthcare_claim_number = @nbr WHERE  invoice_id =  @invoice_id
        ELSE
        BEGIN
            SET @nbr = NULL
            RAISERROR('Number already in use.', 16, 1)
        END

    END

    SELECT @nbr;

COMMIT TRAN


GO

*/

/* -------- FIX UP CLAIM NUMBERS

UPDATE InvoiceHealthcareClaimNumbers SET last_date_used = 
(
   SELECT top 1 invoice_date_added 
   FROM Invoice 
   WHERE healthcare_claim_number = InvoiceHealthcareClaimNumbers.claim_number 
   ORDER BY invoice_date_added DESC
)

*/


/* ReceiptPaymentType

CREATE TABLE ReceiptPaymentType
(
 receipt_payment_type_id  int         not null PRIMARY KEY identity,
 descr                    varchar(50) not null
);

SET IDENTITY_INSERT ReceiptPaymentType ON;
INSERT ReceiptPaymentType 
   (receipt_payment_type_id,descr)
VALUES
   (129, 'Cash'),
   (130, 'HICAPS'),      -- was 'EFT/HICAPS'
   (133, 'CC / EFTPOS'), -- was 'Credit card'
   (136, 'Cheque'),
   (229, 'Money Order'),
   (362, 'Direct Credit'),
   (363, 'Online Payment'),
   (364, 'Tyro Payment'),
   (365, 'Tyro HC Claim');

SET IDENTITY_INSERT ReceiptPaymentType OFF;

*/

/* Receipt  (ref: ReceiptPaymentType, Invoice, POSMachine, Staff)


    total owing = inv total - recepit - adj notes

    overpayment record is created to avoid bigger queries
    - but there is no current need to show invoice that have ovepayments
    - that is only in the accounting system to find invoice overpaid
      he gets:
      - overpayment records, which gives receipt, then invoice

    when overpayment fixed (ie money paid to customer) just delete overpaid record
    [but this is not implemented yet]


    ==> looks like can just fucking use a query instead of overpayment table crap


    invoice.is_paid
    ---------------

    when receipting OR adding adjusment notes:

    (adjustment note reduces the total owing)
    total owing = inv total - all receipts - all adj notes

    if owing <= 0 => set invoice as paid
    if owing >  0 => add overpayment record



1  FD_Id                   - recepit_id
2  FD_Date                 - date_added (is always todays date)
5  FD_XRef                 - invoice_id
7  FD_XrefFileSw
6  FD_Total                - total
8  FD_GovtTaxOrReconciled  - amount_reconciled (ie on bank statement -- should be same, but need it) .. auto put same as total, but let them edit later
10 FD_StaffId              - staff_id
12 FD_FailedToClear        - failed_to_clear
13 FD_PaymentType_Refund   - receipt_payment_type_id
14 FD_ProcessDate          - reconciliation_date 
15 FD_OverpaidOrRefund     - is_overpaid  (can leave out to get on fly is better!)
16 FD_POSXref              - pos_machine_id (they put in in a note represents a room/machine where was receipted via POS) 


CREATE TABLE Receipt
(
 receipt_id                 int           not null PRIMARY KEY identity,
 receipt_payment_type_id    int           not null FOREIGN KEY REFERENCES ReceiptPaymentType(receipt_payment_type_id),
 invoice_id                 int           not null FOREIGN KEY REFERENCES Invoice(invoice_id),

 total                      decimal(8,2)  not null, 
 amount_reconciled          decimal(8,2)  not null,                       -- zero  when adding receipt -- update on reconciliation
 is_failed_to_clear         bit           not null,                       -- false when adding receipt -- update on reconciliation
 is_overpaid                bit           not null,                       -- get all receipts for this invoice PLUS the amount of this one
	                                                                      --   if over than invoice amount, then (ONLY THIS) receipt set to overpaid => and need to create an overpayment record!
 receipt_date_added         datetime      not null DEFAULT (GETDATE()),
 reconciliation_date        datetime               DEFAULT NULL,          -- null  when adding receipt -- auto set current date on reconciliation

 staff_id                   int           not null FOREIGN KEY REFERENCES Staff(staff_id), 

 reversed_by                int                    FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT NULL,
 reversed_date              datetime               DEFAULT NULL,
 pre_reversed_amount        decimal(8,2)  not null DEFAULT 0.00,
);

*/

/* uspInsertReceipt

--------------------------------------

DROP PROCEDURE uspInsertReceipt;

CREATE PROCEDURE uspInsertReceipt 

     @receipt_id                  int
    ,@receipt_payment_type_id     int
    ,@invoice_id                  int
    ,@total                       decimal(8,2)
    ,@amount_reconciled           decimal(8,2)
    ,@is_failed_to_clear          bit         
    ,@is_overpaid                 bit         
    ,@receipt_date_added          datetime    
    ,@reconciliation_date         datetime    
    ,@staff_id                    int  
    ,@reversed_by                 int
    ,@reversed_date               datetime
    ,@pre_reversed_amount         decimal(8,2)


AS

DECLARE @ignore bit;

BEGIN TRAN


    -- these are receipts with no invoice (has been deleted or invoice_id wrote as bad data), charles says to disregard them
    SET @ignore = 0;
    IF (
        (@receipt_id IN (6834,25577,25580,24748,25586,25599,37962,55090,59828,59844,62335,62340,69846,69847,69849,71185,71191,72257,72258,72260,72266,72286,92257,92874,93042,101012,101013,123465,136970,136975,136978,137002,149736,149739,150001,151413,151416,159538,159541,160844))
            AND
        ((SELECT COUNT(*) FROM Invoice WHERE invoice_id = @invoice_id) = 0)
       )
    BEGIN
        SET @ignore = 1;
    END


    IF  (@ignore = 0)
    BEGIN
 
        IF (SELECT COUNT(*) FROM Invoice WHERE invoice_id = @invoice_id) = 0
            RAISERROR('No invoice_id found:  @receipt_id = %d, @invoice_id = %d', 16, 1, @receipt_id, @invoice_id)


        SET IDENTITY_INSERT Receipt ON

        INSERT INTO Receipt (receipt_id,receipt_payment_type_id,invoice_id,total,amount_reconciled,is_failed_to_clear,is_overpaid,
                             receipt_date_added,reconciliation_date,staff_id,reversed_by,reversed_date,pre_reversed_amount)
        VALUES
        (
         @receipt_id             
        ,@receipt_payment_type_id
        ,@invoice_id             
        ,@total                  
        ,@amount_reconciled      
        ,@is_failed_to_clear     
        ,@is_overpaid            
        ,@receipt_date_added     
        ,@reconciliation_date    
        ,@staff_id               
        ,@reversed_by                 
        ,@reversed_date               
        ,@pre_reversed_amount         
        )
 
        SET IDENTITY_INSERT Receipt OFF

    END


COMMIT TRAN

--------------------------------------

 */


/* Overpayment  (ref: Receipt, Staff)

- 2 types of overpayments:
  - medicare overpayment - medicare pays more than what you realised when their payments went up before system udpated (and should change the InvoiceLine, Invoice, GeneralLedger, Receipt that thta has overpaid field)
  - non-medicare overpayment (a payment overpays by mistake)

- if total receipts is over inv amount ... then create overpayment record

- if medicare overpayment, can just update the invoice, invlice line, genral ledger ... dont need to make overpayment
  but if dva ... multile invoice lines .. so dont know which line to adjust... ???  charles says its difficult


1	FD_Id                   - overpayment_id
2	FD_Date	Date            - date_added (is always todays date)
3	FD_Type	Number                   - overpayment = 359
4	FD_Site	Number          - site_id
5	FD_XRef	Number          - invoice_id 
6	FD_Total                - total
10	FD_StaffId              - staff_id


CREATE TABLE Overpayment
(
 overpayment_id              int  not null PRIMARY KEY identity,
 receipt_id                  int  not null FOREIGN KEY REFERENCES Receipt(receipt_id),
 total                       decimal(8,2)  not null, 
 overpayment_date_added      datetime      not null DEFAULT (GETDATE()),
 staff_id                    int  not null FOREIGN KEY REFERENCES Staff(staff_id), 
);

*/

/* uspInsertOverpayment

--------------------------------------

DROP PROCEDURE uspInsertOverpayment;

CREATE PROCEDURE uspInsertOverpayment

     @overpayment_id              int
    ,@receipt_id                  int
    ,@total                       decimal(8,2)
    ,@overpayment_date_added      datetime    
    ,@staff_id                    int  

AS

DECLARE @ignore bit;

BEGIN TRAN


    -- these are overpayments with no receipt [or receipt with no invoice] (has been deleted), charles says to disregard them
    -- overpayment_id, receipt_id, invoice_id
    -- 37963           37962       35121 -- doesn't exist
    -- 52931           0 -- doesn't exist
    -- 141876          141875      0     -- doesn't exist

    SET @ignore = 0;
    IF (
        (@overpayment_id IN (37963,52931,141876))
            AND
        ((SELECT COUNT(*) FROM Receipt WHERE receipt_id = @receipt_id) = 0)
       )
    BEGIN
        SET @ignore = 1;
    END


    IF  (@ignore = 0)
    BEGIN
 
        IF (SELECT COUNT(*) FROM Receipt WHERE receipt_id = @receipt_id) = 0
            RAISERROR('No receipt_id found:  @overpayment_id = %d, @receipt_id = %d', 16, 1, @overpayment_id, @receipt_id)


        SET IDENTITY_INSERT Overpayment ON

        INSERT INTO Overpayment (overpayment_id,receipt_id,total,overpayment_date_added,staff_id)
        VALUES
        (
         @overpayment_id             
        ,@receipt_id             
        ,@total                  
        ,@overpayment_date_added     
        ,@staff_id               
        )
 
        SET IDENTITY_INSERT Overpayment OFF

    END


COMMIT TRAN

--------------------------------------

 */


/* CreditNote  (ref: Invoice, Staff)

1	FD_Id                      - credit_note_id
2	FD_Date                    - receipt_date_added
3	FD_Type                    - 59
5	FD_XRef                    - invoice_id
6	FD_Total                   - total
10	FD_StaffId                 - staff_id

==> reason:
    registration table
	   3 Reg_Doc = financial document id
	   5 Reg_FileSwitch = 59
	   8 Reg_RefNo = reason (free text)


CREATE TABLE CreditNote
(
 creditnote_id              int            not null PRIMARY KEY identity,
 invoice_id                 int            not null FOREIGN KEY REFERENCES Invoice(invoice_id),
 total                      decimal(8,2)   not null, 
 reason                     varchar(250)   not null,
 credit_note_date_added     datetime       not null DEFAULT (GETDATE()),
 staff_id                   int            not null FOREIGN KEY REFERENCES Staff(staff_id), 

 reversed_by                int                     FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT NULL,
 reversed_date              datetime                DEFAULT NULL,
 pre_reversed_amount        decimal(8,2)   not null DEFAULT 0.00,
);

*/

/* uspInsertCreditNote

--------------------------------------

DROP PROCEDURE uspInsertCreditNote;

CREATE PROCEDURE uspInsertCreditNote

     @creditnote_id               int
    ,@invoice_id                  int
    ,@total                       decimal(8,2)
    ,@reason                      varchar(50)
    ,@credit_note_date_added      datetime    
    ,@staff_id                    int  
    ,@reversed_by                 int
    ,@reversed_date               datetime
    ,@pre_reversed_amount         decimal(8,2)

AS

DECLARE @ignore bit;

BEGIN TRAN


    -- these are credit notes with no invoice
    -- 11170   inv 10874  [deleted]
    -- 19929   inv 19911             booking 16783 [deleted]
    -- 20438   inv 20437  [deleted]
    -- 25602   inv 25569  [deleted]
    -- 52301   inv 0
    -- 92875   inv 92858  [deleted]
    -- 101857  inv 101853 [deleted]
    -- 136939  inv 136006 [deleted]
    SET @ignore = 0;
    IF (
        (@creditnote_id IN (11170,19929,20438,25602,52301,92875,101857,136939))
            AND
        ((SELECT COUNT(*) FROM Invoice WHERE invoice_id = @invoice_id) = 0)
       )
    BEGIN
        SET @ignore = 1;
    END


    IF  (@ignore = 0)
    BEGIN
 
        IF (SELECT COUNT(*) FROM Invoice WHERE invoice_id = @invoice_id) = 0
            RAISERROR('No invoice_id found:  @creditnote_id = %d, @invoice_id = %d', 16, 1, @creditnote_id, @invoice_id)


        SET IDENTITY_INSERT CreditNote ON

        INSERT INTO CreditNote (creditnote_id,invoice_id,total,reason,credit_note_date_added,staff_id,reversed_by,reversed_date,pre_reversed_amount)
        VALUES
        (
         @creditnote_id             
        ,@invoice_id             
        ,@total                  
        ,@reason
        ,@credit_note_date_added     
        ,@staff_id               
        ,@reversed_by                 
        ,@reversed_date               
        ,@pre_reversed_amount     
        )
 
        SET IDENTITY_INSERT CreditNote OFF

    END


COMMIT TRAN

--------------------------------------

 */


/* RefundReason

CREATE TABLE RefundReason
(
 refund_reason_id  int         not null PRIMARY KEY identity,
 descr             varchar(50) not null
);

SET IDENTITY_INSERT RefundReason ON;
INSERT RefundReason
   (refund_reason_id,descr)
VALUES
   (227, 'Unhappy With Service'),
   (308, 'Incorrect Pricing');
SET IDENTITY_INSERT RefundReason OFF;

*/

/* Refund  (ref: Invoice, RefundReason, Staff)

=> any time a refund is done, it has to close the invoice
   therefore credit note created for invoice for amount of still owing
   and that sets invoice as "is_paid
=> amount refunded must be less than or equal to "total receipts"

if inv is_paid = false (refund will close inv it to set as paid):
 either
 - disallow refund
 - [JS confirm popup to warn this, if yes then:] add credit note for total "due" (not amount of refund!) to set it as paid 

then is_paid, so:
- create receipt record
- set invoice as - is_refund = true
- update ledger


1	FD_Id                     - refund_id
2	FD_Date                   - refund_date_added
3	FD_Type                   - 357
5	FD_XRef                   - invoice_id
6	FD_Total                  - total
10	FD_StaffId                - staff_id
13	FD_PaymentType_Refund     - reason id why they are refunding  -- kpi table csf=38
16	FD_POSXref                - note id for free text why they are refunding  -- stored as note [this is just text field for a reason open text]


CREATE TABLE Refund
(
 refund_id                   int           not null PRIMARY KEY identity,
 invoice_id                  int           not null FOREIGN KEY REFERENCES Invoice(invoice_id),
 total                       decimal(8,2)  not null, 
 refund_reason_id            int           not null FOREIGN KEY REFERENCES RefundReason(refund_reason_id),
 comment                     varchar(max)  not null,
 refund_date_added           datetime      not null DEFAULT (GETDATE()),
 staff_id                    int           not null FOREIGN KEY REFERENCES Staff(staff_id), 
);

*/

/* uspInsertRefund

--------------------------------------

DROP PROCEDURE uspInsertRefund;

CREATE PROCEDURE uspInsertRefund

     @refund_id                   int
    ,@invoice_id                  int
    ,@total                       decimal(8,2)
    ,@refund_reason_id            int
    ,@comment                     varchar(500)
    ,@refund_date_added           datetime
    ,@staff_id                    int

AS

BEGIN TRAN


        SET IDENTITY_INSERT Refund ON

        INSERT INTO Refund (refund_id,invoice_id,total,refund_reason_id,comment,refund_date_added,staff_id)
        VALUES
        (
         @refund_id
        ,@invoice_id
        ,@total
        ,@refund_reason_id
        ,@comment
        ,@refund_date_added
        ,@staff_id
        )
 
        SET IDENTITY_INSERT Refund OFF


COMMIT TRAN

--------------------------------------

 */


/* PaymentPending

CREATE TABLE PaymentPending
(
 payment_pending_id             int           not null PRIMARY KEY identity,

 invoice_id                     int           not null FOREIGN KEY REFERENCES Invoice(invoice_id),
 payment_amount                 decimal(8,2)  not null,
 customer_name                  varchar(250)  not null,
 date_added                     datetime      not null,

 out_date_processed             datetime      DEFAULT NULL,
 out_payment_result             varchar(1)    not null,
 out_payment_result_code        varchar(50)   not null,
 out_payment_result_text        varchar(250)  not null,
 out_bank_receipt_id            varchar(50)   not null,
 out_paytecht_payment_id        varchar(50)   not null,
);

*/

/* TyroPaymentPending, TyroPaymentType

	CREATE Table TyroPaymentType
	(
		tyro_payment_type_id  int            not null PRIMARY KEY identity,
		descr                 varchar(50)    not null
	);

	SET IDENTITY_INSERT TyroPaymentType ON;
	INSERT INTO TyroPaymentType 
		(tyro_payment_type_id,descr)
	Values
		(1, 'Purchase'),
		(2, 'Refund')
	SET IDENTITY_INSERT TyroPaymentType OFF;

	CREATE TABLE TyroPaymentPending
	(
		tyro_payment_pending_id           int           not null PRIMARY KEY identity,

		invoice_id                        int           not null FOREIGN KEY REFERENCES Invoice(invoice_id),
		tyro_transaction_id               varchar(50)   not null,     -- for reconcilliation "[DB]-[tyro_payment_pending_id]"  eg.  0001-304904
		tyro_payment_type_id              int           not null FOREIGN KEY REFERENCES TyroPaymentType(tyro_payment_type_id),
		amount                            decimal(8,2)  not null,
		cashout                           decimal(8,2)  not null,
		date_added                        datetime      not null,

		out_date_processed                datetime      DEFAULT NULL, -- date out params entered into the system
		out_result                        varchar(100)  not null,
		out_cardType                      varchar(100)  not null,
		out_transactionReference          varchar(100)  not null,
		out_authorisationCode             varchar(100)  not null,
		out_issuerActionCode              varchar(100)  not null,
	);

*/

/* TyroHealthClaim, TyroHealthClaimItem

CREATE TABLE TyroHealthClaim
(
	tyro_health_claim_id                        int           not null PRIMARY KEY identity,
	invoice_id                                  int           not null FOREIGN KEY REFERENCES Invoice(invoice_id),
	tyro_transaction_id                         varchar(50)   not null,     -- for reconcilliation "[DB]-[tyro_health_claim_id]"  eg.  0001-304904
	amount                                      decimal(8,2)  not null,
	date_added                                  datetime      not null,

	out_date_processed							datetime      DEFAULT NULL,
	out_result									varchar(100)  not null,     -- "APPROVED"
	out_healthpointRefTag						varchar(100)  not null,     -- "7417220"
	out_healthpointTotalBenefitAmount			decimal(8,2)  not null,     -- "8600"
	out_healthpointSettlementDateTime			datetime      DEFAULT NULL, -- "20150424165436"
	out_healthpointTerminalDateTime				datetime      DEFAULT NULL, -- "20150424165436"
	out_healthpointMemberNumber					varchar(100)  not null,     -- "0000000000"
	out_healthpointProviderId					varchar(100)  not null,     -- "4237955J"
	out_healthpointServiceType					varchar(100)  not null,     -- "F"
	out_healthpointGapAmount					decimal(8,2)  not null,     -- "0"
	out_healthpointPhfResponseCode				varchar(100)  not null,     -- "00"
	out_healthpointPhfResponseCodeDescription	varchar(500)  not null,     -- "APPROVED"
	date_cancelled                              datetime      DEFAULT NULL,
);

	CREATE TABLE TyroHealthClaimItem
	(
		tyro_health_claim_item_id                   int           not null PRIMARY KEY identity,
		tyro_health_claim_id                        int           not null FOREIGN KEY REFERENCES TyroHealthClaim(tyro_health_claim_id),

		out_claimAmount				                decimal(8,2)  not null,     -- "6800"
		out_rebateAmount			                decimal(8,2)  not null,     -- "6800"
		out_serviceCode				                varchar(100)  not null,     -- "F1234"
		out_description				                varchar(500)  not null,     -- "Face"
		out_serviceReference		                varchar(100)  not null,     -- "01"
		out_patientId				                varchar(100)  not null,     -- "47"
		out_serviceDate				                datetime      DEFAULT NULL, -- "20150423"
		out_responseCodeString		                varchar(100)  not null,     -- "0000"
	);

*/

/* Credit, CreditType

-- add voucher (CR-ID, 1,  E-ID,  50.00, 'V-Descr', '2015-12-12',  NULL,   NULL,   NULL,    STAFF-ID, DATEADDED, NULL, NULL, 0)
-- use voucher (CR-ID, 2,  E-ID, -40.00, '',         NULL,         1,      INV-ID, NULL,    STAFF-ID, DATEADDED, NULL, NULL, 0)

-- cashout     (CR-ID, 3,  E-ID,  20.00, '',         NULL,         NULL,   NULL,   TYRO-ID, STAFF-ID, DATEADDED, NULL, NULL, 0)
               (CR-ID, 4,  E-ID, -20.00, '',         NULL,         NULL,   NULL,   TYRO-ID, STAFF-ID, DATEADDED, NULL, NULL, 0)

CREATE TABLE Credit
(
 credit_id               int           not null PRIMARY KEY identity,
 credit_type_id          int           not null FOREIGN KEY REFERENCES CreditType(credit_type_id),
 entity_id               int                    FOREIGN KEY REFERENCES Entity(entity_id),                            -- pt (or org_id if facility/etc doing vouchers only)
 amount                  decimal(8,2)  not null,
 voucher_descr           varchar(1000) not null,                                                                     -- for (1)   adding voucher
 expiry_date             datetime,                                                                                   -- for (1)   adding voucher
 voucher_credit_id       int                    FOREIGN KEY REFERENCES Credit(credit_id),                            -- for (2)   using  voucher - links to which voucher (1) use credit from
 invoice_id              int                    FOREIGN KEY REFERENCES Invoice(invoice_id),                          -- for (2)   using  voucher
 tyro_payment_pending_id int                    FOREIGN KEY REFERENCES TyroPaymentPending(tyro_payment_pending_id),  -- for (3/4) cashout (same tyroId for Tyro->MC & MC->PT)
 added_by                int           not null FOREIGN KEY REFERENCES Staff(staff_id),
 date_added              datetime      not null,
 deleted_by              int                    FOREIGN KEY REFERENCES Staff(staff_id),
 date_deleted            datetime,
 pre_deleted_amount      decimal(8,2)  not null                                                                      -- used the same way as receipts/crnotes/refunds
 modified_by             int                    FOREIGN KEY REFERENCES Staff(staff_id),
 date_modified           datetime               DEFAULT null
);


CREATE TABLE CreditType
(
 credit_type_id  int            not null PRIMARY KEY identity,
 descr           varchar(50)    not null
);

SET IDENTITY_INSERT CreditType ON;
INSERT CreditType
  (credit_type_id, descr)
VALUES
  (1, 'Add Voucher'),
  (2, 'Use Voucher'),
  (3, 'Cashout Tyro To Mediclinic'),
  (4, 'Cashout Mediclinc to PT');
SET IDENTITY_INSERT CreditType OFF;

*/

/* CreditHistory

CREATE TABLE CreditHistory
(
 credit_history_id       int           not null PRIMARY KEY identity,
 credit_id               int           not null FOREIGN KEY REFERENCES Credit(credit_id),    -- links back to credit (entity left out as it's not editable)
 credit_type_id          int           not null FOREIGN KEY REFERENCES CreditType(credit_type_id),
 amount                  decimal(8,2)  not null,
 voucher_descr           varchar(1000) not null,                                                                     -- for (1)   adding voucher
 expiry_date             datetime,                                                                                   -- for (1)   adding voucher
 voucher_credit_id       int                    FOREIGN KEY REFERENCES Credit(credit_id),                            -- for (2)   using  voucher - links to which voucher (1) use credit from
 invoice_id              int                    FOREIGN KEY REFERENCES Invoice(invoice_id),                          -- for (2)   using  voucher
 tyro_payment_pending_id int                    FOREIGN KEY REFERENCES TyroPaymentPending(tyro_payment_pending_id),  -- for (3/4) cashout (same tyroId for Tyro->MC & MC->PT)
 added_by                int           not null FOREIGN KEY REFERENCES Staff(staff_id),
 date_added              datetime      not null,
 deleted_by              int                    FOREIGN KEY REFERENCES Staff(staff_id),
 date_deleted            datetime,
 pre_deleted_amount      decimal(8,2)  not null,
 modified_by             int                    FOREIGN KEY REFERENCES Staff(staff_id),
 date_modified           datetime               DEFAULT null,
);

*/



/* LetterType

CREATE TABLE LetterType
(
 letter_type_id                   int  not null PRIMARY KEY identity,
 descr                            varchar(50) not null
);

SET IDENTITY_INSERT LetterType ON;
INSERT LetterType
   (letter_type_id,descr)
VALUES
   (3,    'Resident'),
   (214,  'Aged care facility'),
   (234,  'Medicare'),
   (235,  'DVA'),
   (389,  'Doctor'),
   (390,  'Recall'),

   (391,  'Reminder'),
   (392,  'Courtesy'),
   (393,  'Allocated');



SET IDENTITY_INSERT LetterType OFF;

-------------------------------------------
SELECT  *
FROM    LetterType;
-------------------------------------------

*/

/* Letter  (ref: LetterType, Organisation, Site)

for invoice rejects:
  234 = medicare
  235 = dva
  214 = reject invoice for a facility
  3   = person in aged care home

so 
 * if debtor is medicare, show 234 types with code not empty
 * if debtor is dva, show 235 types with code not empty
 * if debtor is fac not medicare/dva, show 214 types with code not empty
 * if debtor is person, show 3 types with code not empty

=> when entering items ... make sure if 234/235/3/214 ... NEED code entered (validate)



CREATE TABLE Letter
(
 letter_id           int           not null PRIMARY KEY identity,
 organisation_id     int                    FOREIGN KEY REFERENCES Organisation(organisation_id),
 letter_type_id      int           not null FOREIGN KEY REFERENCES LetterType(letter_type_id),
 site_id             int           not null FOREIGN KEY REFERENCES Site(site_id),
 code                varchar(10)   not null,
 reject_message      varchar(200)  not null,
 docname             varchar(100)  not null,
 is_send_to_medico   bit           not null,
 is_allowed_reclaim  bit           not null,
 is_manual_override  bit           not null,
 num_copies_to_print int           not null,
 is_deleted          bit           not null,
);

*/


/* LetterPrintHistorySendMethod

CREATE TABLE LetterPrintHistorySendMethod
(
 letter_print_history_send_method_id      int         not null PRIMARY KEY identity,
 descr                                    varchar(50) not null
);

SET IDENTITY_INSERT LetterPrintHistorySendMethod ON;
INSERT LetterPrintHistorySendMethod 
   (letter_print_history_send_method_id,descr)
VALUES
   (1, 'Mail'),
   (2, 'Email'),
   (3, 'SMS');
SET IDENTITY_INSERT LetterPrintHistorySendMethod OFF;

*/

/* LetterPrintHistory  (ref: Letter, LetterPrintHistorySendMethod, Booking, Patient, Organisation, RegisterReferrer, Staff, HealthCardAction)

 -- note charles only kept letter_type_id cuz then he can select those types from registration table 
 --      we can get type from letter_id
 --      and dont need to distinguish items that are letterprinthistory becuase we dont use a generic fuckin table for everything


Letters in BEST:
 1. automatic letters (first, last) - kept only in healthcardaction becuase they are attached to an epc
 2. rest are kept in registration table (including new 'treatment notes' letters)
 3. actual copies of letters are put on server by marcus, has nothing to do with BEST or charles


CREATE TABLE LetterPrintHistory
(
 letter_print_history_id               int           not null PRIMARY KEY identity,
 letter_id                             int           not null FOREIGN KEY REFERENCES Letter(letter_id),

 letter_print_history_send_method_id   int           not null FOREIGN KEY REFERENCES LetterPrintHistorySendMethod(letter_print_history_send_method_id),

 booking_id                            int                    FOREIGN KEY REFERENCES Booking(booking_id),
 patient_id                            int                    FOREIGN KEY REFERENCES Patient(patient_id),
 organisation_id                       int                    FOREIGN KEY REFERENCES Organisation(organisation_id),
 register_referrer_id                  int                    FOREIGN KEY REFERENCES RegisterReferrer(register_referrer_id), 
 staff_id                              int                    FOREIGN KEY REFERENCES Staff(staff_id),

 health_card_action_id                 int                    FOREIGN KEY REFERENCES HealthCardAction(health_card_action_id), 

 doc_name                              varchar(100)  not null,
 doc_contents                          varbinary(MAX),
 
 from_best                             bit           not null DEFAULT 0,
 date                                  datetime      not null DEFAULT (GETDATE()),
);



-------------------------------------------
SELECT  *
FROM    LetterPrintHistory
-------------------------------------------

*/

/* LetterTreatmentTemplate  (ref: Letter, Field)

CREATE TABLE LetterTreatmentTemplate
(
 letter_treatment_template_id       int  not null PRIMARY KEY identity,
 field_id                           int  not null FOREIGN KEY REFERENCES Field(field_id),

 first_letter_id                    int  not null FOREIGN KEY REFERENCES Letter(letter_id),
 last_letter_id                     int  not null FOREIGN KEY REFERENCES Letter(letter_id),
 last_letter_pt_id                  int  not null FOREIGN KEY REFERENCES Letter(letter_id),
 last_letter_when_replacing_epc_id  int  not null FOREIGN KEY REFERENCES Letter(letter_id),
 treatment_notes_letter_id          int  not null FOREIGN KEY REFERENCES Letter(letter_id),

 site_id                            int  not null FOREIGN KEY REFERENCES Site(site_id),
);

-------------------------------------------
SELECT  *
FROM    LetterTreatmentTemplate
-------------------------------------------

*/



/* LetterBest  (ref: LetterType)

CREATE TABLE LetterBest
(
 letter_id           int           not null PRIMARY KEY identity,
 letter_type_id      int           not null FOREIGN KEY REFERENCES LetterType(letter_type_id),
 code                varchar(10)   not null,
 docname             varchar(100)  not null,
 is_send_to_medico   bit           not null,
 is_allowed_reclaim  bit           not null,
 is_manual_override  bit           not null,
 num_copies_to_print int           not null,
);

*/

/* LetterBestPrintHistory  (ref: LetterBest, Patient)

CREATE TABLE LetterBestPrintHistory
(
 letter_print_history_id      int           not null PRIMARY KEY identity,
 letter_id                    int           not null FOREIGN KEY REFERENCES LetterBest(letter_id),
 patient_id                   int           not null FOREIGN KEY REFERENCES Patient(patient_id),
 date                         datetime      not null DEFAULT (GETDATE()),
);

*/



/* SMSCredit

CREATE TABLE SMSCredit
(
 sms_credit_id        int          not null PRIMARY KEY identity,
 amount               decimal(8,2) not null, 
 datetime_added       datetime     not null,
);

*/

/* SMSHistory  (ref: SMSAndEmailType, Patient, Booking)

CREATE TABLE SMSHistory
(
 sms_history_id         int          not null PRIMARY KEY identity,
 sms_and_email_type_id  int          not null FOREIGN KEY REFERENCES SMSAndEmailType(sms_and_email_type_id),
 patient_id             int                   FOREIGN KEY REFERENCES Patient(patient_id),
 booking_id             int                   FOREIGN KEY REFERENCES Booking(booking_id),
 phone_number           varchar(20)  not null,
 message                varchar(max) not null,
 cost                   decimal(8,2) not null, 
 datetime_sent          datetime     not null,
 smstech_message_id     varchar(50)  not null,
 smstech_status         varchar(50)  not null,
 smstech_datetime       datetime              DEFAULT NULL,
);

*/

/* EmailHistory  (ref: SMSAndEmailType, Patient, Booking)

CREATE TABLE EmailHistory
(
 email_history_id       int        not null PRIMARY KEY identity,
 sms_and_email_type_id  int        not null FOREIGN KEY REFERENCES SMSAndEmailType(sms_and_email_type_id),
 patient_id             int                 FOREIGN KEY REFERENCES Patient(patient_id),
 booking_id             int                 FOREIGN KEY REFERENCES Booking(booking_id),
 email                  varchar(250)  not null,
 message                varchar(max)  not null,
 datetime_sent          datetime      not null,
);

*/

/* SMSAndEmailType

CREATE TABLE SMSAndEmailType
(
 sms_and_email_type_id  int          not null PRIMARY KEY identity,
 descr                  varchar(50)  not null,
);


SET IDENTITY_INSERT SMSAndEmailType ON;
INSERT SMSAndEmailType
   (sms_and_email_type_id,descr)
VALUES
   (1,  'Appointment Reminder'),
   (2,  'Birthday');
SET IDENTITY_INSERT SMSAndEmailType OFF;

*/



/* Condition

CREATE TABLE Condition
(
 condition_id           int          not null PRIMARY KEY identity,
 descr                  varchar(250) not null,
 show_date              bit          not null,
 show_nweeksdue         bit          not null,
 show_text              bit          not null,
 display_order          int          not null,
 is_deleted             bit          not null,
);

*/

/* PatientCondition

CREATE TABLE PatientCondition
(
 patient_condition_id   int          not null PRIMARY KEY identity,
 patient_id             int                   FOREIGN KEY REFERENCES Patient(patient_id),
 condition_id           int                   FOREIGN KEY REFERENCES Condition(condition_id),
 date                   date,
 nweeksdue              int          not null,
 text                   varchar(500) not null,
 is_deleted             bit          not null,
 CONSTRAINT uc_unique_patient_condition UNIQUE (patient_id,condition_id)
);

*/



/* BulkLetterSendingQueueBatch, BulkLetterSendingQueue

CREATE TABLE BulkLetterSendingQueueBatch
(
 bulk_letter_sending_queue_batch_id         int          not null PRIMARY KEY identity,
 email_address_to_send_printed_letters_to   varchar(200) not null,
 ready_to_process                           bit          not null,    -- when many letters to print are added, dont get all yet to email because not all entered into the queue yet
);


SET IDENTITY_INSERT LetterPrintHistorySendMethod ON;
INSERT LetterPrintHistorySendMethod 
   (letter_print_history_send_method_id,descr)
VALUES
   (3, 'SMS');
SET IDENTITY_INSERT LetterPrintHistorySendMethod OFF;


SET IDENTITY_INSERT SMSAndEmailType ON;
INSERT SMSAndEmailType
   (sms_and_email_type_id,descr)
VALUES
   (4,  'Marketing');
SET IDENTITY_INSERT SMSAndEmailType OFF;



CREATE TABLE BulkLetterSendingQueue
(
 bulk_letter_sending_queue_id                        int     PRIMARY KEY identity NOT NULL,
 bulk_letter_sending_queue_batch_id                  int     FOREIGN KEY REFERENCES BulkLetterSendingQueueBatch(bulk_letter_sending_queue_batch_id)       NOT NULL,
 letter_print_history_send_method_id                 int     FOREIGN KEY REFERENCES LetterPrintHistorySendMethod(letter_print_history_send_method_id) NOT NULL,
 added_by                                            int     FOREIGN KEY REFERENCES Staff(staff_id),        -- nullable for when sent by automated program

 patient_id                                          int     FOREIGN KEY REFERENCES Patient(patient_id),
 referrer_id                                         int     FOREIGN KEY REFERENCES Referrer(referrer_id),
 booking_id                                          int     FOREIGN KEY REFERENCES Booking(booking_id),

 phone_number                                        varchar(20)   not null,
 email_to_address                                    varchar(80)   not null,
 email_to_name                                       varchar(80)   not null,
 email_from_address                                  varchar(80)   not null,
 email_from_name                                     varchar(80)   not null,

 text                                                varchar(8000) not null,
 email_subject                                       varchar(200)  not null,

 email_attachment_location                           varchar(2000) not null,  -- comma seperated
 email_attachment_delete_after_sending               bit           not null,
 email_attachment_folder_delete_after_sending        bit           not null,
 
 email_letter_letter_id                              int     FOREIGN KEY REFERENCES Letter(letter_id), 
 email_letter_keep_history_in_db                     bit not null, 
 email_letter_keep_history_in_file                   bit not null, 
 email_letter_letter_print_history_send_method_id    int     FOREIGN KEY REFERENCES LetterPrintHistorySendMethod(letter_print_history_send_method_id), 
 email_letter_history_dir                            varchar(250), 
 email_letter_history_filename                       varchar(250), 
 email_letter_site_id                                int     FOREIGN KEY REFERENCES Site(site_id), 
 email_letter_organisation_id                        int     FOREIGN KEY REFERENCES Organisation(organisation_id), 
 email_letter_booking_id                             int     FOREIGN KEY REFERENCES Booking(booking_id), 
 email_letter_patient_id                             int     FOREIGN KEY REFERENCES Patient(patient_id), 
 email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref   int FOREIGN KEY REFERENCES RegisterReferrer(register_referrer_id), 
 email_letter_staff_id                               int     FOREIGN KEY REFERENCES Staff(staff_id), 
 email_letter_health_card_action_id                  int     FOREIGN KEY REFERENCES HealthCardAction(health_card_action_id),      
 email_letter_source_template_path                   varchar(250)  not null, 
 email_letter_output_doc_path                        varchar(250)  not null, 
 email_letter_is_double_sided_printing               bit           not null, 
 email_letter_extra_pages                            varchar(8000) not null,   -- eg   note1[sep]note2
 email_letter_item_seperator                         varchar(10)   not null,   -- eg   @=@=@

 sql_to_run_on_completion                            varchar(2000) not null,
 sql_to_run_on_failure                               varchar(2000) not null,

 datetime_added                                      datetime      not null DEFAULT GETDATE(),
 datetime_sending_start                              datetime,           -- null = can send...if more than [1hr?] amd date_sent = null, send to null to resend
 datetime_sent                                       datetime,

);


CREATE TABLE BulkLetterSendingQueueAdditionalLetter
(
 bulk_letter_sending_queue_letter_id                 int     PRIMARY KEY identity NOT NULL,
 bulk_letter_sending_queue_id                        int     FOREIGN KEY REFERENCES BulkLetterSendingQueue(bulk_letter_sending_queue_id) NOT NULL, 

 email_letter_letter_id                              int     FOREIGN KEY REFERENCES Letter(letter_id) NOT NULL, 
 email_letter_keep_history_in_db                     bit not null, 
 email_letter_keep_history_in_file                   bit not null, 
 email_letter_letter_print_history_send_method_id    int     FOREIGN KEY REFERENCES LetterPrintHistorySendMethod(letter_print_history_send_method_id), 
 email_letter_history_dir                            varchar(250), 
 email_letter_history_filename                       varchar(250), 
 email_letter_site_id                                int     FOREIGN KEY REFERENCES Site(site_id), 
 email_letter_organisation_id                        int     FOREIGN KEY REFERENCES Organisation(organisation_id), 
 email_letter_booking_id                             int     FOREIGN KEY REFERENCES Booking(booking_id), 
 email_letter_patient_id                             int     FOREIGN KEY REFERENCES Patient(patient_id), 
 email_letter_register_referrer_id_to_use_instead_of_patients_reg_ref   int FOREIGN KEY REFERENCES RegisterReferrer(register_referrer_id), 
 email_letter_staff_id                               int     FOREIGN KEY REFERENCES Staff(staff_id), 
 email_letter_health_card_action_id                  int     FOREIGN KEY REFERENCES HealthCardAction(health_card_action_id),      
 email_letter_source_template_path                   varchar(250)  not null, 
 email_letter_output_doc_path                        varchar(250)  not null, 
 email_letter_is_double_sided_printing               bit           not null, 
 email_letter_extra_pages                            varchar(8000) not null,   -- eg   note1[sep]note2
 email_letter_item_seperator                         varchar(10)   not null,   -- eg   @=@=@
);


*/



/* Stock  (ref: Organisation, Offering),    StockUpdateHistory  (ref: Organisation, Offering, Staff)

CREATE TABLE Stock
(
 stock_id             int           not null PRIMARY KEY identity,
 organisation_id      int           not null FOREIGN KEY REFERENCES Organisation(organisation_id),
 offering_id          int           not null FOREIGN KEY REFERENCES Offering(offering_id),
 qty                  int           not null,
 warning_amt          int           not null,
);


CREATE TABLE StockUpdateHistory
(
 stock_update_history_id    int           not null PRIMARY KEY identity,
 organisation_id            int           not null FOREIGN KEY REFERENCES Organisation(organisation_id),
 offering_id                int           not null FOREIGN KEY REFERENCES Offering(offering_id),
 qty_added                  int           not null,
 is_created                 bit           not null,
 is_deleted                 bit           not null,
 added_by                   int           not null FOREIGN KEY REFERENCES Staff(staff_id),
 date_added                 datetime      not null,
);


*/

/* OfferingOrder  (ref: Organisation, Staff, Patient, Offering)

CREATE TABLE OfferingOrder
(
 offering_order_id    int           not null PRIMARY KEY identity,

 offering_id          int                    FOREIGN KEY REFERENCES Offering(offering_id),
 organisation_id      int                    FOREIGN KEY REFERENCES Organisation(organisation_id),
 staff_id             int                    FOREIGN KEY REFERENCES Staff(staff_id),
 patient_id           int                    FOREIGN KEY REFERENCES Patient(patient_id) DEFAULT null,

 quantity             int           not null,

 date_ordered         datetime,
 date_filled          datetime,
 date_cancelled       datetime,

 descr                varchar(2000) not null,
);

*/



/* Mediclinic_Main - UserDatabaseMapper

CREATE TABLE UserDatabaseMapper
(
	id                   int     not null PRIMARY KEY identity,
	username             varchar(200)    not null UNIQUE,
	dbname               varchar(200)    not null,
);

*/

/* Mediclinic_Main - SetupNewCustomer 

CREATE TABLE SetupNewCustomer
(
 setup_new_customer_id int          not null PRIMARY KEY identity,

 company_name          varchar(200) not null,
 firstname             varchar(200) not null,
 surname               varchar(200) not null,
 company_email         varchar(200) not null,

 address_line1         varchar(200) not null,
 address_line2         varchar(200) not null,
 city                  varchar(200) not null,
 state_province_region varchar(200) not null,
 postcode              varchar(20)  not null,
 country               varchar(200) not null,
 phone_nbr             varchar(200) not null,

 max_nbr_providers     varchar(5)   not null,

 field1                varchar(30)  not null,
 field2                varchar(30)  not null,
 field3                varchar(30)  not null,
 field4                varchar(30)  not null,

 random_string         varchar(30)  not null  UNIQUE,  -- for link in email to activate it
 date_added_info       datetime     not null,
 date_added_db         datetime,
 db_name               varchar(30)  not null,  -- blank = not yet activated

);

*/