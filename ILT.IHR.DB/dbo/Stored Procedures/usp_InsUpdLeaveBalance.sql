--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 12/10/2020
-- Description : Insert/Update SP for LeaveBalance
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdLeaveBalance]
	@LeaveBalanceID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@LeaveYear	int	=	NULL,
	@LeaveTypeID	int	=	NULL,
	@VacationTotal	numeric(5,1)	=	NULL,
	@VacationUsed	numeric(5,1)	=	NULL,
	@EncashedLeave numeric(5,1)	=	NULL,
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

	IF NOT EXISTS(SELECT 1 FROM LeaveBalance WHERE LeaveBalanceID = ISNULL(@LeaveBalanceID,0))  
	BEGIN
		INSERT INTO [dbo].[LeaveBalance]
		(
			EmployeeID,
			LeaveYear,
			LeaveTypeID,
			VacationTotal,
			VacationUsed,
			CreatedBy,
			CreatedDate
		)
		OUTPUT INSERTED.LeaveBalanceID INTO @IDTable 
		VALUES
		(
			@EmployeeID,
			@LeaveYear,
			@LeaveTypeID,
			@VacationTotal,
			@VacationUsed,
			@CreatedBy,
			GETDATE()
		)
		SELECT @LeaveBalanceID=(SELECT ID FROM @IDTable); 
	END
	ELSE
		UPDATE [dbo].LeaveBalance
		SET
			EmployeeID = @EmployeeID,
			LeaveYear = @LeaveYear,
			LeaveTypeID = @LeaveTypeID,
			VacationTotal = @VacationTotal,
			VacationUsed = @VacationUsed,
			EncashedLeave = @EncashedLeave,
			ModifiedBy = @ModifiedBy,
			ModifiedDate =GETDATE()
		WHERE LeaveBalanceID = @LeaveBalanceID 

	IF @@ERROR=0   
		SET @ReturnCode  = @LeaveBalanceID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
END