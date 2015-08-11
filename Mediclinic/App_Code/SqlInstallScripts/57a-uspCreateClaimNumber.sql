CREATE PROCEDURE uspCreateClaimNumber @invoice_id int, @invoice_date datetime
AS

DECLARE @healthcare_claim_number nvarchar(10);

BEGIN TRAN

    SET @healthcare_claim_number = (SELECT TOP 1 claim_number FROM InvoiceHealthcareClaimNumbers WHERE is_active = 1 AND (last_date_used IS NULL OR last_date_used < DATEADD(year,-2,@invoice_date)) ORDER BY id)

    IF @healthcare_claim_number IS NULL 
    BEGIN

        RAISERROR('No claim numbers left!', 16, 1)

    END
    ELSE
    BEGIN

        IF (SELECT count(*) FROM Invoice WHERE invoice_date_added > DATEADD(year,-2,@invoice_date)  AND healthcare_claim_number = @healthcare_claim_number) = 0
        BEGIN
            UPDATE InvoiceHealthcareClaimNumbers SET last_date_used = @invoice_date WHERE claim_number = @healthcare_claim_number
            UPDATE Invoice SET healthcare_claim_number = @healthcare_claim_number WHERE invoice_id =  @invoice_id
        END
        ELSE
        BEGIN
            SET @healthcare_claim_number = NULL
            RAISERROR('Error: Claim number already in use: @new_claim_number = %d', 16, 1, @healthcare_claim_number)
        END

    END

    SELECT @healthcare_claim_number;

COMMIT TRAN
