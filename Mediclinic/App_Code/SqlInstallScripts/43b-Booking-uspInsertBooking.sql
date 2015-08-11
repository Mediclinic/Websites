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