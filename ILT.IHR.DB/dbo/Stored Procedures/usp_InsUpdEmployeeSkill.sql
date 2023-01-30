--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 06/08/2022
-- Description : Insert/Update SP for Employee Skill
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdEmployeeSkill]
	@EmployeeSkillID int = NULL,
	@EmployeeID	int	= NULL,
	@SkillTypeID int = NULL,
	@Skill	varchar(100) = NULL,
	@Experience	int	= NULL,
	@CreatedBy	varchar(50)	= NULL,
	@CreatedDate	datetime = NULL,
	@ModifiedBy	varchar(50)	= NULL,
	@ModifiedDate	datetime = NULL,
	@TimeStamp	timestamp =	NULL,
	@ReturnCode INT = 0 OUTPUT 
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	
	IF NOT EXISTS(SELECT 1 FROM EmployeeSkill WHERE EmployeeSkillID = ISNULL(@EmployeeSkillID,0))  
		BEGIN
			INSERT INTO [dbo].[EmployeeSkill]
			(
				EmployeeID,
				SkillTypeID,
				Skill,
				Experience,
				CreatedBy,
				CreatedDate
			)
			OUTPUT INSERTED.EmployeeSkillID INTO @IDTable 
			VALUES
			(
				@EmployeeID,
				@SkillTypeID,
				@Skill,
				@Experience,
				@CreatedBy,
				GETDATE()
			)
			SELECT @EmployeeSkillID=(SELECT ID FROM @IDTable);  
		END
	ELSE
		BEGIN
			UPDATE [dbo].[EmployeeSkill]
			SET
				EmployeeID = @EmployeeID,
				SkillTypeID = @SkillTypeID,
				Skill = @Skill,
				Experience = @Experience,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = GETDATE()
			WHERE EmployeeSkillID = @EmployeeSkillID
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @EmployeeSkillID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END