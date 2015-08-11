CREATE FUNCTION [dbo].[ufnFilterNonAlphaNumeric]( @Input varchar(256))
RETURNS varchar(256)
AS
BEGIN
    If PATINDEX('%[^0-9a-zA-Z]%', @Input) > 0
        WHILE PATINDEX('%[^0-9a-zA-Z]%', @Input) > 0
            SET @Input = Stuff(@Input, PATINDEX('%[^0-9a-zA-Z]%', @Input), 1, '')
    RETURN @Input
END