CREATE FUNCTION [dbo].[GenerateEmployeeCode] 
(
	@EmploymentTypeID int, 
	@Country varchar(50)
) 
RETURNS varchar(12)
AS
BEGIN
	DECLARE @EmployeeCode varchar(12), @EmploymentType VARCHAR(20), @EmployeeCodePrefix varchar(12)

	SELECT @EmploymentType = [Value] FROM ListValue LV
	WHERE LV.ListValueID = @EmploymentTypeID

	SELECT @EmployeeCode = [ValueDesc] FROM ListValue LV
	JOIN ListType LT ON LT.ListTypeID = LV.ListTypeID
	WHERE LT.Type='EMPLOYEECODE' AND LV.Value = @Country + '-' + @EmploymentType 

	SELECT @EmployeeCodePrefix = SUBSTRING(@EmployeeCode,1, CHARINDEX('-',@EmployeeCode)-1)

	SELECT TOP 1 @EmployeeCode = ISNULL(EmployeeCode,@EmployeeCode) FROM Employee
	WHERE EmployeeCode LIKE @EmployeeCodePrefix + '%' AND EmploymentTypeID = @EmploymentTypeID 
	AND Country = @Country
	ORDER BY CreatedDate DESC

	SELECT @EmployeeCode = REPLACE(@EmployeeCode,'-','')
	
	SELECT @EmployeeCode =  @EmployeeCodePrefix + CONVERT(VARCHAR(10),CONVERT(INT,REPLACE(@EmployeeCode,@EmployeeCodePrefix,''))+1)
	
	RETURN @EmployeeCode
END