--===============================================================
-- Author : Sanjan Madishetti
-- Created Date : 01/22/2020
-- Description : Insert/Update SP for LeaveAccrual
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By			Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdLeaveAccrual]
	@LeaveAccrualID	int	=	NULL,
	@Country varchar(50) = NULL,
	@AccruedDate	date	=	NULL,
	@AccruedValue	numeric(5,1) =	0,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL,
	@ReturnCode INT = 0 OUTPUT 

AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	DECLARE @LeaveTypeID int, @EmploymentTypeID int
	
	IF NOT EXISTS(SELECT 1 FROM LeaveAccrual WHERE Country = @Country AND (LeaveAccrualID = ISNULL(@LeaveAccrualID,0) OR FORMAT(AccruedDate,'MM/yyyy') = FORMAT(@AccruedDate, 'MM/yyyy')))  
	BEGIN
		INSERT INTO [dbo].[LeaveAccrual]
		(
			Country,
			AccruedDate,
			AccruedValue,
			CreatedBy,
			CreatedDate,
			ModifiedBy,
			ModifiedDate
		)
	OUTPUT INSERTED.LeaveAccrualID INTO @IDTable 
	VALUES
		(
			@Country,
			@AccruedDate,
			@AccruedValue,
			@CreatedBy,
			GETDATE(),
			@CreatedBy,
			GETDATE()
		)
		SELECT @LeaveAccrualID =(SELECT ID FROM @IDTable);  

		SELECT @LeaveTypeID = ListValueID FROM ListValue LV
		INNER JOIN ListType LT ON LV.ListTypeID = Lt.ListTypeID
		WHERE LT.Type = 'VacationType' AND LV.Value = 'CASUAL'

		SELECT @EmploymentTypeID = ListValueID FROM ListValue LV
		INNER JOIN ListType LT ON LV.ListTypeID = Lt.ListTypeID
		WHERE LT.Type = 'EmploymentType' AND LV.Value = 'FTE'

		UPDATE LB
		SET LB.VacationTotal = LB.VacationTotal + @AccruedValue
		FROM [dbo].[LeaveBalance] LB
		INNER JOIN [dbo].Employee E ON LB.EmployeeID = E.EmployeeID
		WHERE LB.LeaveYear = YEAR(@AccruedDate)
		AND LB.LeaveTypeID = @LeaveTypeID
		AND E.Country = @Country
		AND TermDate IS NULL
		AND E.EmploymentTypeID = @EmploymentTypeID

	END
	ELSE
		THROW 99999, 'Leave already accrued for the requested month', 1;  

	IF @@ERROR=0   
		SET @ReturnCode  = @LeaveAccrualID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END