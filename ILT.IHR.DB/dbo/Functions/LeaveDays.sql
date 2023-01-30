CREATE FUNCTION [dbo].[LeaveDays] 
(
	@Start smalldatetime, 
	@End smalldatetime, 
	@Country varchar(50)
) 
RETURNS int
AS
BEGIN
	DECLARE @Days int, @Holidays int, @WeekEnds int, @WorkDays int
	
	SELECT @Days = DATEDIFF(day,@Start,@End)+1
	SELECT @WeekEnds = @Days-( (DATEDIFF(dd, @Start, @End) + 1)-(DATEDIFF(wk, @Start, @End) * 2)
					  -(CASE WHEN DATENAME(dw, @Start) = 'Sunday' THEN 1 ELSE 0 END)
					  -(CASE WHEN DATENAME(dw, @End) = 'Saturday' THEN 1 ELSE 0 END))
	SELECT @Holidays = COUNT(1) FROM Holiday WHERE (StartDate BETWEEN @Start AND @End) AND ISNULL(Country,@Country) = @Country
	SELECT @WorkDays = @Days - ISNULL(@Holidays,0) - ISNULL(@WeekEnds,0)
	
	RETURN @WorkDays
END