--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/24/2020
-- Description : Insert/Update SP for Department
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdDepartment]
	@DepartmentID	int	=	NULL,
	@DeptCode	varchar(10)	=	NULL,
	@DeptName	varchar(50)	=	NULL,
	@DeptLocationID	int	=	NULL,
	@IsActive	bit	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM Department WHERE DepartmentID = ISNULL(@DepartmentID,0))  
		BEGIN
			INSERT INTO [dbo].[Department]
			(
				DeptCode,
				DeptName,
				DeptLocationID,
				IsActive,
				CreatedBy,
				CreatedDate
			)
			OUTPUT INSERTED.DepartmentID INTO @IDTable 
			VALUES
			(
				@DeptCode,
				@DeptName,
				@DeptLocationID,
				@IsActive,
				@CreatedBy,
				GETDATE()
			)
			SELECT @DepartmentID=(SELECT ID FROM @IDTable);  
		END
	ELSE
		BEGIN
			UPDATE [dbo].[Department]
			SET
				DeptCode = @DeptCode,
				DeptName = @DeptName,
				DeptLocationID = @DeptLocationID,
				IsActive = @IsActive,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = GETDATE()
			WHERE DepartmentID = @DepartmentID
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @DepartmentID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
