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
