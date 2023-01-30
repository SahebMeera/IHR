--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 12/08/2020
-- Description : Insert/Update SP for Leave
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdLeave]
	@LeaveID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@Title	varchar(100)	=	NULL,
	@Detail	varchar(500)	=	NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
	@IncludesHalfDay bit	=	0,
	@Duration  numeric(5,1) =	NULL,
	@LeaveTypeID	int	=	NULL,
	@RequesterID	int	=	NULL,
	@ApproverID	int	=	NULL,
	@StatusID	int	=	NULL,
	@Comment	varchar(500)	=	NULL,
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

	Declare @CancelStatusID int = 0
	Declare @DeniedStatusID int = 0
	Declare @UnpaidLeaveID int = 0

	SELECT @Duration = dbo.LeaveDays(@StartDate, @EndDate, E.Country) FROM Employee E
	WHERE E.EmployeeID = @EmployeeID

	SELECT @CancelStatusID = ListValueID FROM ListValue LV  
	INNER JOIN ListType LT ON LV.ListTypeID = Lt.ListTypeID  
	WHERE LT.Type = 'LeaveRequestStatus' AND LV.Value = 'CANCELLED'

	SELECT @DeniedStatusID = ListValueID FROM ListValue LV  
	INNER JOIN ListType LT ON LV.ListTypeID = Lt.ListTypeID  
	WHERE LT.Type = 'LeaveRequestStatus' AND LV.Value = 'DENIED'

	SELECT @UnpaidLeaveID = ListValueID FROM ListValue LV  
	INNER JOIN ListType LT ON LV.ListTypeID = Lt.ListTypeID  
	WHERE LT.Type = 'VacationType' AND LV.Value = 'LWP'

	IF (@IncludesHalfDay = 1 AND @Duration > 0) 
		SET @Duration = @Duration - 0.5

	DECLARE @LeaveBalanceTable TABLE
	(
		EmployeeID	int,
		LeaveTypeID	int,
		StartDate date,
		EndDate date,
		PrevStatusID int,
		NewStatusID int
	)

	--IF NOT EXISTS(SELECT 1 FROM Leave WHERE LeaveID = ISNULL(@LeaveID,0)) 
	IF NOT EXISTS (SELECT LeaveID FROM Leave WHERE LeaveID = ISNULL(@LeaveID,0) )
	BEGIN
		IF NOT EXISTS(SELECT LeaveID FROM Leave WHERE (EmployeeID = @EmployeeID AND ((StartDate <= @endDate and @startDate <= EndDate) 
				  OR  (StartDate >= @startDate and EndDate <= @endDate)) AND (StatusID <> @CancelStatusID) AND (StatusID <> @DeniedStatusID)))
				  OR @LeaveTypeID = @UnpaidLeaveID
			BEGIN
				INSERT INTO [dbo].[Leave]
				(
					EmployeeID,
					Title,
					Detail,
					StartDate,
					EndDate,
					IncludesHalfDay,
					Duration,
					LeaveTypeID,
					RequesterID,
					ApproverID,
					StatusID,
					Comment,
					CreatedBy,
					CreatedDate
				)
				OUTPUT INSERTED.LeaveID INTO @IDTable 
				VALUES
				(
					@EmployeeID,
					@Title,
					@Detail,
					@StartDate,
					@EndDate,
					@IncludesHalfDay,
					@Duration,
					@LeaveTypeID,
					@RequesterID,
					@ApproverID,
					@StatusID,
					@Comment,
					@CreatedBy,
					GETDATE()
				)
				SELECT @LeaveID=(SELECT ID FROM @IDTable); 

				UPDATE LB
				SET LB.UnpaidLeave = LB.UnpaidLeave + @Duration
				FROM [dbo].[LeaveBalance] LB
				INNER JOIN ListValue LV ON   LV.ListValueID = @LeaveTypeID
				INNER JOIN ListValue LV1 ON   LV1.ListValueID = @StatusID
				WHERE LB.EmployeeID = @EmployeeID
				AND LB.LeaveYear =  YEAR(@StartDate)
				AND (LV.Value = 'LWP' AND LV1.Value='APPROVED' ) 
				
			END
	END
	ELSE
		UPDATE [dbo].[Leave]
		SET
			EmployeeID = @EmployeeID,
			Title = @Title,
			Detail = @Detail,
			StartDate = @StartDate,
			EndDate = @EndDate,
			IncludesHalfDay = @IncludesHalfDay,
			Duration = @Duration,
			LeaveTypeID = @LeaveTypeID,
			RequesterID = @RequesterID,
			ApproverID = @ApproverID,
			StatusID = @StatusID,
			Comment = @Comment,
			ModifiedBy = @ModifiedBy,
			ModifiedDate =GETDATE()
		OUTPUT 
			inserted.EmployeeID, 
			inserted.LeaveTypeID, 
			inserted.StartDate,
			inserted.EndDate,
			deleted.StatusID, 
			inserted.StatusID
		INTO @LeaveBalanceTable
		WHERE LeaveID = @LeaveID 

		UPDATE LB
		SET VacationUsed = VacationUsed + 
		((CASE WHEN LV1.Value = 'APPROVED' THEN 1 WHEN LV1.Value = 'CANCELLED' THEN -1 ELSE 0 END) * @Duration)
		FROM [dbo].[LeaveBalance] LB
		INNER JOIN @LeaveBalanceTable LBT ON LB.EmployeeID = LBT.EmployeeID AND LB.LeaveYear = YEAR(LBT.EndDate) AND LB.LeaveTypeID = LBT.LeaveTypeID
		INNER JOIN ListValue LV ON LBT.PrevStatusID = LV.ListValueID 
		INNER JOIN ListValue LV1 ON LBT.NewStatusID = LV1.ListValueID
		--INNER JOIN Employee E ON LBT.EmployeeID = E.EmployeeID
		WHERE (LV1.Value = 'APPROVED' AND LV.Value = 'PENDING') OR
		(LV1.Value = 'CANCELLED' AND LV.Value = 'APPROVED')

	IF @@ERROR=0   
		SET @ReturnCode  = @LeaveID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
END