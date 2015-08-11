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
        --(@receipt_id IN (6834,25577,25580,24748,25586,25599,37962,55090,59828,59844,62335,62340,69846,69847,69849,71185,71191,72257,72258,72260,72266,72286,92257,92874,93042,101012,101013,123465,136970,136975,136978,137002,149736,149739,150001,151413,151416,159538,159541,160844))
        --    AND
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