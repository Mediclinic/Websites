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
