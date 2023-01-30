--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/24/2020
-- Description : Insert/Update SP for Assignment
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdAssignment]
	@AssignmentID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
	@VendorID	int	=	NULL,
	@ClientManager	varchar(50)	=	NULL,
	@Title varchar(50) = NULL,
	--@Role	varchar(20)	=	NULL,
	@Address1	varchar(100)	=	NULL,
	@Address2	varchar(100)	=	NULL,
	@City	varchar(50)	=	NULL,
	@State	varchar(50)	=	NULL,
	@Country	varchar(50)	=	NULL,
	@ZipCode	varchar(10)	=	NULL,
	@ClientID	int	=	NULL,
	@EndClientID int = NULL,
	@SubClient varchar(500) = NULL,
	@Comments	varchar(100)	=	NULL,
	@PaymentTypeID	int	=	NULL,
	@TimesheetTypeID int = NULL,
	--@TimesheetApproverID int = NULL,
	@TSApproverEmail varchar(50) = NULL,
	@ApprovedEmailTo varchar(100) = NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@TimeStamp	timestamp	=	NULL,
	@ReturnCode INT = 0 OUTPUT 
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	
	IF NOT EXISTS(SELECT 1 FROM Assignment WHERE AssignmentID = ISNULL(@AssignmentID,0))  
	BEGIN
		INSERT INTO [dbo].[Assignment]
		(
			EmployeeID,
			StartDate,
			EndDate,
			VendorID,
			ClientManager,
			Title,
			--Role,
			Address1,
			Address2,
			City,
			State,
			Country,
			ZipCode,
			ClientID,
			EndClientID,
			SubClient,
			Comments,
			PaymentTypeID,
			TimesheetTypeID,
			--TimesheetApproverID,
			TSApproverEmail,
			ApprovedEmailTo,
			CreatedBy,
			CreatedDate
		)
		OUTPUT INSERTED.AssignmentID INTO @IDTable 
		VALUES
		(
			@EmployeeID,
			@StartDate,
			@EndDate,
			@VendorID,
			@ClientManager,
			@Title,
			--@Role,
			@Address1,
			@Address2,
			@City,
			@State,
			@Country,
			@ZipCode,
			@ClientID,
			@EndClientID,
			@SubClient,
			@Comments,
			@PaymentTypeID,
			@TimesheetTypeID,
			--@TimesheetApproverID,
			@TSApproverEmail,
			@ApprovedEmailTo,
			@CreatedBy,
			GETDATE()
		)
		SELECT @AssignmentID=(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Assignment]
		SET
			EmployeeID = @EmployeeID,
			StartDate = @StartDate,
			EndDate = @EndDate,
			VendorID = @VendorID,
			ClientManager = @ClientManager,
			Title = @Title,
			--Role = @Role,
			Address1 = @Address1,
			Address2 = @Address2,
			City = @City,
			State = @State,
			Country = @Country,
			ZipCode = @ZipCode,
			ClientID = @ClientID,
			EndClientID = @EndClientID,
			SubClient =@SubClient,
			Comments = @Comments,
			PaymentTypeID = @PaymentTypeID,
			TimesheetTypeID = @TimesheetTypeID,
			--TimesheetApproverID = @TimesheetApproverID,
			TSApproverEmail = @TSApproverEmail,
			ApprovedEmailTo = @ApprovedEmailTo,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
			WHERE AssignmentID = ISNULL(@AssignmentID,0)
	END

	IF @@ERROR=0   
		SET @ReturnCode  = @AssignmentID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
END
