--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/07/2020
-- Description : Insert/Update SP for TimeSheet
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdTimeSheet]
(
	@TimeSheetID	int	=	NULL,
	@EmployeeID int = NULL,
	@WeekEnding datetime = NULL,
	@ClientID int = NULL,
	@AssignmentID int = NULL,
	@TotalHours int = 0,
	@FileName varchar(100) = NULL,
	@StatusID int = NULL,
	@SubmittedByID int = NULL,
	@SubmittedDate datetime = NULL,
	--@ApprovedByID int = NULL,
	@ApprovedDate datetime = NULL,
	@ApprovedByEmail varchar(50) = NULL,
	@ClosedByID int = NULL,
	@ClosedDate datetime = NULL,
	@Comment varchar(500) = NULL,
	@xmlTimeEntry xml = NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime =	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime =	NULL,
	@TimeStamp	timestamp	=	NULL,
	@ReturnCode INT = 0 OUTPUT 
)
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	DECLARE @tblTimeEntry TypeTimeEntry

	INSERT INTO @tblTimeEntry
	(
		TimeEntryID,
		TimeSheetID,
		WorkDate,
		Project,
		Activity,
		[Hours]				
	)
	SELECT 
	x.v.value('TimeEntryID[1]','int'),
	x.v.value('TimeSheetID[1]','int'),
	x.v.value('WorkDate[1]','date'),
	x.v.value('Project[1]','varchar(50)'), 
	x.v.value('Activity[1]','varchar(50)'), 
	x.v.value('Hours[1]','int')  
	FROM @xmlTimeEntry.nodes('ArrayOfTimeEntry/TimeEntry') as x(v)
	
	IF NOT EXISTS(SELECT 1 FROM TimeSheet WHERE TimeSheetID = ISNULL(@TimeSheetID,0))  
		BEGIN
			INSERT INTO [dbo].[TimeSheet]
			(
				EmployeeID,
				WeekEnding,
				ClientID,
				AssignmentID,
				TotalHours,
				[FileName],
				DocGuid,
				StatusID,
				SubmittedByID,
				SubmittedDate,
				--ApprovedByID,
				ApprovedDate,
				ApprovedByEmail,
				ClosedByID,
				ClosedDate,
				Comment,
				CreatedBy,
				CreatedDate
			)
			OUTPUT INSERTED.TimeSheetID INTO @IDTable 
			VALUES
			(
				
				@EmployeeID,
				@WeekEnding,
				@ClientID,
				@AssignmentID,
				@TotalHours,
				@FileName,
				NEWID(),
				@StatusID,
				@SubmittedByID,
				@SubmittedDate,
				--@ApprovedByID,
				@ApprovedDate,
				@ApprovedByEmail,
				@ClosedByID,
				@ClosedDate,
				@Comment,
				@CreatedBy,
				GETDATE()
			)
			SELECT @TimeSheetID=(SELECT ID FROM @IDTable);  

			INSERT INTO [dbo].[TimeEntry]
			(
				TimeSheetID,
				WorkDate,
				Project,
				Activity,
				[Hours]				
			)
			SELECT 
			@TimeSheetID,
			WorkDate,
			Project,
			Activity,
			[Hours]	
			FROM @tblTimeEntry
		END
	ELSE
		BEGIN
			UPDATE [dbo].[TimeSheet]
			SET
				EmployeeID = @EmployeeID,
				WeekEnding = @WeekEnding,
				ClientID = @ClientID,
				AssignmentID = @AssignmentID,
				TotalHours = @TotalHours,
				[FileName] = @FileName,
				StatusID = @StatusID,
				SubmittedByID = @SubmittedByID,
				SubmittedDate = @SubmittedDate,
				--ApprovedByID = @ApprovedByID,
				ApprovedDate = @ApprovedDate,
				ApprovedByEmail = @ApprovedByEmail,
				ClosedByID = @ClosedByID,
				ClosedDate = @ClosedDate,
				Comment = @Comment,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = GETDATE()
			WHERE TimeSheetID = @TimeSheetID

			MERGE TimeEntry AS TARGET
			USING @tblTimeEntry AS SOURCE 
			ON (TARGET.TimeEntryID = SOURCE.TimeEntryID) 
			--When records are matched, update the records if there is any change
			WHEN MATCHED --AND TARGET.ProductName <> SOURCE.ProductName OR TARGET.Rate <> SOURCE.Rate 
			THEN UPDATE SET TARGET.WorkDate = SOURCE.WorkDate, TARGET.Project = SOURCE.Project, TARGET.Activity = SOURCE.Activity, TARGET.[Hours] = SOURCE.[Hours] 
			--When no records are matched, insert the incoming records from source table to target table
			WHEN NOT MATCHED BY TARGET 
			THEN INSERT (TimeSheetID, WorkDate, Project, Activity, [Hours]) VALUES (@TimeSheetID, SOURCE.WorkDate, SOURCE.Project, SOURCE.Activity, SOURCE.[Hours])
			--When there is a row that exists in target and same record does not exist in source then delete this record target
			WHEN NOT MATCHED BY SOURCE AND TARGET.TimeSheetID = @TimeSheetID
			THEN DELETE;
			--$action specifies a column of type nvarchar(10) in the OUTPUT clause that returns 
			--one of three values for each row: 'INSERT', 'UPDATE', or 'DELETE' according to the action that was performed on that row
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @TimeSheetID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END