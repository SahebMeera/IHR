--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 06/08/2022
-- Description : Delete SP for Employee Skill
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_DeleteEmployeeSkill]
	@EmployeeSkillID	int
AS 
BEGIN
	DELETE FROM  [dbo].[EmployeeSkill]
	WHERE EmployeeSkillID = @EmployeeSkillID
END