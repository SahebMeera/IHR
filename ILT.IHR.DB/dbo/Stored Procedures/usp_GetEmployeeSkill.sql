--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 06/08/2022
-- Description : Select SP for Employee Skill
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetEmployeeSkill]
    @EmployeeSkillID int = NULL,
	@EmployeeID	int	= NULL,
	@SkillTypeID int = NULL,
	@Skill	varchar(100) = NULL
	--@Experience	int	= NULL,
	--@CreatedBy	varchar(50)	= NULL,
	--@CreatedDate	datetime = NULL,
	--@ModifiedBy	varchar(50)	= NULL,
	--@ModifiedDate	datetime = NULL,
	--@TimeStamp	timestamp =	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT 
		ES.EmployeeSkillID,
		ES.SkillTypeID,
		ST.[ValueDesc] as SkillType,
		ES.EmployeeID,
		E.FirstName + ' ' + E.LastName as EmployeeName,
		ES.Skill,
		ES.Experience,
		ES.CreatedBy,
		ES.CreatedDate,
		ES.ModifiedBy,
		ES.ModifiedDate,
		ES.TimeStamp
	FROM [dbo].[EmployeeSkill] ES
    INNER JOIN [Employee] E on ES.EmployeeID = E.EmployeeID
	LEFT JOIN ListValue ST ON ES.SkillTypeID = ST.ListValueID
	WHERE ES.EmployeeSkillID =  ISNULL(NULLIF(@EmployeeSkillID,0), ES.EmployeeSkillID) 
    AND ES.EmployeeID = ISNULL(NULLIF(@EmployeeID,0), ES.EmployeeID)
	AND ES.SkillTypeID = ISNULL(NULLIF(@SkillTypeID,0), ES.SkillTypeID)
	AND ES.Skill = ISNULL(NULLIF(@Skill,''), ES.Skill)
	 
END