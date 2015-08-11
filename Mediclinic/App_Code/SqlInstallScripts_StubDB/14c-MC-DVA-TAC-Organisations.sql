
DECLARE @date DATETIME
SET @date = Convert(date, getdate()) 

EXEC uspInsertOrganisation 
     @organisation_id               = -1
    ,@parent_organisation_id        = NULL
    ,@organisation_type_id          = 149
    ,@organisation_customer_type_id = 149
    ,@name                          = 'Medicare'
    ,@acn                           = ''
    ,@abn                           = ''
    ,@organisation_date_added       = @date
    ,@organisation_date_modified    = NULL
    ,@is_debtor                     = 0
    ,@is_creditor                   = 1
    ,@bpay_account                  = 0
    ,@is_deleted                    = 0
    ,@weeks_per_service_cycle       = 0
    
    ,@start_date                    = 0
    ,@end_date                      = 0
    ,@comment                       = 0
    ,@free_services                 = 0
    ,@excl_sun                      = 0
    ,@excl_mon                      = 0
    ,@excl_tue                      = 0
    ,@excl_wed                      = 0
    ,@excl_thu                      = 0
    ,@excl_fri                      = 0
    ,@excl_sat                      = 0
    
    ,@sun_start_time                = '00:00:00'
    ,@sun_end_time                  = '00:00:00'
    ,@mon_start_time                = '00:00:00'
    ,@mon_end_time                  = '00:00:00'
    ,@tue_start_time                = '00:00:00'
    ,@tue_end_time                  = '00:00:00'
    ,@wed_start_time                = '00:00:00'
    ,@wed_end_time                  = '00:00:00'
    ,@thu_start_time                = '00:00:00'
    ,@thu_end_time                  = '00:00:00'
    ,@fri_start_time                = '00:00:00'
    ,@fri_end_time                  = '00:00:00'
    ,@sat_start_time                = '00:00:00'
    ,@sat_end_time                  = '00:00:00'
    
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



EXEC uspInsertOrganisation 
     @organisation_id               = -2
    ,@parent_organisation_id        = NULL
    ,@organisation_type_id          = 149
    ,@organisation_customer_type_id = 149
    ,@name                          = 'DVA'
    ,@acn                           = ''
    ,@abn                           = ''
    ,@organisation_date_added       = @date
    ,@organisation_date_modified    = NULL
    ,@is_debtor                     = 0
    ,@is_creditor                   = 1
    ,@bpay_account                  = 0
    ,@is_deleted                    = 0
    ,@weeks_per_service_cycle       = 0
    
    ,@start_date                    = 0
    ,@end_date                      = 0
    ,@comment                       = 0
    ,@free_services                 = 0
    ,@excl_sun                      = 0
    ,@excl_mon                      = 0
    ,@excl_tue                      = 0
    ,@excl_wed                      = 0
    ,@excl_thu                      = 0
    ,@excl_fri                      = 0
    ,@excl_sat                      = 0
    
    ,@sun_start_time                = '00:00:00'
    ,@sun_end_time                  = '00:00:00'
    ,@mon_start_time                = '00:00:00'
    ,@mon_end_time                  = '00:00:00'
    ,@tue_start_time                = '00:00:00'
    ,@tue_end_time                  = '00:00:00'
    ,@wed_start_time                = '00:00:00'
    ,@wed_end_time                  = '00:00:00'
    ,@thu_start_time                = '00:00:00'
    ,@thu_end_time                  = '00:00:00'
    ,@fri_start_time                = '00:00:00'
    ,@fri_end_time                  = '00:00:00'
    ,@sat_start_time                = '00:00:00'
    ,@sat_end_time                  = '00:00:00'
    
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



EXEC uspInsertOrganisation 
     @organisation_id               = -3
    ,@parent_organisation_id        = NULL
    ,@organisation_type_id          = 149
    ,@organisation_customer_type_id = 149
    ,@name                          = 'TAC / Workcover'
    ,@acn                           = ''
    ,@abn                           = ''
    ,@organisation_date_added       = @date
    ,@organisation_date_modified    = NULL
    ,@is_debtor                     = 0
    ,@is_creditor                   = 1
    ,@bpay_account                  = 0
    ,@is_deleted                    = 0
    ,@weeks_per_service_cycle       = 0
    
    ,@start_date                    = 0
    ,@end_date                      = 0
    ,@comment                       = 0
    ,@free_services                 = 0
    ,@excl_sun                      = 0
    ,@excl_mon                      = 0
    ,@excl_tue                      = 0
    ,@excl_wed                      = 0
    ,@excl_thu                      = 0
    ,@excl_fri                      = 0
    ,@excl_sat                      = 0
    
    ,@sun_start_time                = '00:00:00'
    ,@sun_end_time                  = '00:00:00'
    ,@mon_start_time                = '00:00:00'
    ,@mon_end_time                  = '00:00:00'
    ,@tue_start_time                = '00:00:00'
    ,@tue_end_time                  = '00:00:00'
    ,@wed_start_time                = '00:00:00'
    ,@wed_end_time                  = '00:00:00'
    ,@thu_start_time                = '00:00:00'
    ,@thu_end_time                  = '00:00:00'
    ,@fri_start_time                = '00:00:00'
    ,@fri_end_time                  = '00:00:00'
    ,@sat_start_time                = '00:00:00'
    ,@sat_end_time                  = '00:00:00'
    
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


