--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for User
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
-- 02/08/2021 Mihir Hapaliya  updated sp to add or update user roles 
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdUser]
	@UserID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@FirstName	varchar(50)	=	NULL,
	@LastName	varchar(50)	=	NULL,
	@UserRoles xml = NULL,
	@xmlUserRole xml = NULL,	
	@Email	varchar(50)	=	NULL,
	@Password	varchar(50)	=	NULL,
	@IsOAuth bit = NULL,
	@IsActive bit = NULL,
	@CompanyID int = NULL,
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
	DECLARE @tblUserRole TypeUserRole
	
	INSERT INTO @tblUserRole
	(
		[UserRoleID],
		[UserID],
		[RoleID],
		[IsDefault],
		[CreatedBy],
		[CreatedDate],
		[ModifiedBy],
		[ModifiedDate]
	)
	SELECT 
	x.v.value('UserRoleID[1]','int'),
	x.v.value('UserID[1]','int'),
	x.v.value('RoleID[1]','int'),
	x.v.value('IsDefault[1]','bit'),
	x.v.value('CreatedBy[1]','varchar(50)'),
	x.v.value('CreatedDate[1]','datetime'),
	x.v.value('ModifiedBy[1]','varchar(50)'),
	x.v.value('ModifiedDate[1]','datetime')
	FROM @xmlUserRole.nodes('ArrayOfUserRole/UserRole') as x(v)
	
	IF NOT EXISTS(SELECT 1 FROM [User] WHERE UserID = ISNULL(@UserID,0))  
	BEGIN
		INSERT INTO [dbo].[User]
		(
			EmployeeID,
			FirstName,
			LastName,			
			Email,
			Password,
			IsOAuth,
			IsActive,
			CompanyID,
			CreatedBy,
			CreatedDate,
			ModifiedBy,
			ModifiedDate
		)
	OUTPUT INSERTED.UserID INTO @IDTable 
	VALUES
		(
			@EmployeeID,
			@FirstName,
			@LastName,			
			@Email,
			@Password,
			@IsOAuth,
			@IsActive,
			@CompanyID,
			@CreatedBy,
			GETDATE(),
			@CreatedBy,
			GETDATE()
		)
		SELECT @UserID =(SELECT ID FROM @IDTable); 

		INSERT INTO [dbo].[UserRole]
			(
			UserID,
			RoleID,
			IsDefault,
			CreatedBy,
			CreatedDate,
			ModifiedBy,
			ModifiedDate
			)
			SELECT 
			@UserID,
			RoleID,
			IsDefault,
			CreatedBy,
			GETDATE(),
			ModifiedBy,
			ModifiedDate
			FROM @tblUserRole
		

	END
	ELSE
	BEGIN
		UPDATE [dbo].[User]
		SET
			EmployeeID = @EmployeeID,
			FirstName = @FirstName,
			LastName = @LastName,			
			Email = @Email,
			Password = ISNULL(@Password, Password),
			IsOAuth = @IsOAuth,
			IsActive = @IsActive,
			CompanyID = @CompanyID,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE UserID = @UserID


		MERGE UserRole AS TARGET
			USING @tblUserRole AS SOURCE 
			ON (TARGET.UserRoleID = SOURCE.UserRoleID) 
			WHEN MATCHED 
			THEN UPDATE SET TARGET.IsDefault = SOURCE.IsDefault, TARGET.ModifiedBy = SOURCE.ModifiedBy, TARGET.ModifiedDate = GetDate()
			WHEN NOT MATCHED BY TARGET 
			THEN INSERT (UserID, RoleID, IsDefault,CreatedBy,CreatedDate) VALUES (@UserID, SOURCE.RoleID, SOURCE.IsDefault, SOURCE.CreatedBy, GetDate())
			WHEN NOT MATCHED BY SOURCE AND TARGET.UserID = @UserID
			THEN DELETE;

	END
	IF @@ERROR=0   
		SET @ReturnCode  = @UserID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
