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
