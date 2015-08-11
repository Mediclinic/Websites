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