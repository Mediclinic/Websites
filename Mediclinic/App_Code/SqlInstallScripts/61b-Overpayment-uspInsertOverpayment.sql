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
        --(@overpayment_id IN (37963,52931,141876))
        --    AND
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