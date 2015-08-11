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
        --(@creditnote_id IN (11170,19929,20438,25602,52301,92875,101857,136939))
        --    AND
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