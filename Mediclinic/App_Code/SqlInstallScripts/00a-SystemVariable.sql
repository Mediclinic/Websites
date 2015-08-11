CREATE TABLE SystemVariable
(
 descr                varchar(200)    not null UNIQUE,
 value                varchar(200)    not null,
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

('ServiceSpecificBookingReminderLettersToBatch_EmailAddress','',0,1),

('AddressType','ContactAus',0,1) ,

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

('EnableDeletedBookingsAlerts',       '1',1,1);
