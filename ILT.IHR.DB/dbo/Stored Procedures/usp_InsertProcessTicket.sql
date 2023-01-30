--===============================================================
-- Author : Nimesh Patel
-- Created Date : 11/23/2021
-- Description : Add into Process data ticket table
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--06/14/2021	Mihir Hapaliya	Reminder Duration added
--===============================================================

CREATE PROCEDURE [dbo].[usp_InsertProcessTicket]
@ProcessDataId int,
@ChangeNotificationEmailId Varchar(50) = NULL,
@EmailApprovalValidity int = NULL
AS
BEGIN

	DECLARE @TicketStatusId INT
	DECLARE @TicketAssignedStatusId INT
	DECLARE @RequestedByID INT
	DECLARE @CREATEDBY varchar(50)
	DECLARE @ProcessDataStatusId INT
	DECLARE @ProcessWizardID INT
	DECLARE @TicketIdTable TABLE(ID INT)
	DECLARE @DataField XML;
	DECLARE @FirstName VARCHAR(50);
	DECLARE @LastName VARCHAR(50);
	DECLARE @EmployeeName VARCHAR(50);
	DECLARE @EmployeeID INT ;
	DECLARE @Email VARCHAR(100);
	DECLARE @Phone VARCHAR(50);
	DECLARE @StartDate VARCHAR(50);
	DECLARE @TermDate VARCHAR(50);
	DECLARE @Desc VARCHAR(500);
	DECLARE @ModuleId INT;
	DECLARE @RequesterEmailId varchar(100);
	

	SELECT @ProcessDataStatusId = PD.StatusID, @RequestedByID = U.EmployeeID, @CREATEDBY=PD.ModifiedBy FROM [dbo].[ProcessData] PD 
		JOIN ListValue LV ON PD.StatusID = LV.ListValueID 
		JOIN [User] U ON PD.ModifiedBy = U.FirstName + ' ' + U.LastName
		WHERE LV.[Value]='INPROCESS' AND PD.ProcessDataID = @ProcessDataId

	SELECT @RequesterEmailId = (CASE WHEN ISNULL(WorkEmail,'') <> '' THEN WorkEmail ELSE Email END) FROM Employee WHERE EmployeeID = @RequestedByID


		IF (ISNULL(@ProcessDataStatusId,0)<>0 AND NOT EXISTS(SELECT 1 FROM [dbo].[ProcessDataTicket] WHERE ProcessDataID = ISNULL(@ProcessDataID,0)))  
			BEGIN
				
				SELECT @TicketStatusId = V.ListValueID FROM ListValue V
				INNER JOIN ListType T on T.ListTypeID = V.ListTypeID
				WHERE T.[TYPE]='TICKETSTATUS' AND V.[Value]='NEW'

				SELECT  @TicketAssignedStatusId = V.ListValueID FROM ListValue V
				INNER JOIN ListType T on T.ListTypeID = V.ListTypeID
				WHERE T.[TYPE]='TICKETSTATUS' AND V.[Value]='ASSIGNED'

				
				SELECT @DataField = [Data] FROM dbo.ProcessData WHERE ProcessDataID = @ProcessDataId 

				SELECT
						@EmployeeID=  [Table].[Column].value('EmployeeID[1]', 'INT'),
						@FirstName = [Table].[Column].value('FirstName[1]', 'varchar(50)'),
						@LastName = [Table].[Column].value('LastName[1]', 'varchar(50)'),
						@Email = [Table].[Column].value('Email[1]', 'varchar(100)'),
						@Phone = [Table].[Column].value('Phone[1]', 'varchar(50)'),
						@StartDate = [Table].[Column].value('StartDate[1]', 'varchar(50)'),
						@TermDate = [Table].[Column].value('TermDate[1]', 'varchar(50)'),
						@EmployeeName = [Table].[Column].value('Employee[1]', 'varchar(50)')
						FROM @DataField.nodes('/WizardData/CandidateInfo') AS [Table]([Column])   

				
				IF (@EmployeeID IS NOT NULL)
				(
					SELECT @FirstName = FirstName, @LastName = LAstName, @Email = WorkEmail, @Phone = Phone FROM Employee WHERE EmployeeID = @EmployeeID
				)
			
				IF(@FirstName IS NOT Null)
					SET @Desc = 'Name: '+ @FirstName;
				
				
				IF(@LastName IS NOT Null)
					SET @Desc = @Desc + ' ' +  @LastName

				IF(@Email IS NOT Null)
					SET @Desc = @Desc + ', Email: ' +  @Email


				IF(@Phone IS NOT Null)
					SET @Desc = @Desc + ', Phone: ' +  @Phone

				IF(@StartDate IS NOT Null)
					SET @Desc = @Desc + ', Start Date: ' +  @StartDate

			    IF(@TermDate IS NOT Null)
					SET @Desc = @Desc + ', Term Date: ' +  @TermDate

				INSERT INTO [dbo].[Ticket]
				(
					TicketTypeID,
					RequestedByID,
					ModuleID,
					ID,
					Title,
					[Description],
					AssignedToID,
					StatusID,
					ReminderDuration,
					CreatedBy,
					CreatedDate	
				)
				OUTPUT INSERTED.TicketID INTO @TicketIdTable 
				SELECT 
				TicketTypeId,
				@RequestedByID,
				Null,
				Null,
				Task = CASE WHEN @EmployeeName IS NOT NULL THEN Task + ' - ' + @EmployeeName ELSE Task + ' - ' + @FirstName + ' ' +  @LastName END,
				@Desc +  Char(13)+Char(10) + TaskDescription,
				PG.AssignTo,
				Case when PG.AssignTo IS NOT NULL THEN @TicketAssignedStatusId  ELSE @TicketStatusId END,
				PG.ReminderDuration,
				@CREATEDBY,
				GETDATE()
				FROM [dbo].[ProcessConfig] PG 
				INNER JOIN [dbo].[ProcessData] PD ON PD.ProcessWizardID= PG.ProcessWizardId
				WHERE PD.ProcessDataID = @ProcessDataId
				
				INSERT INTO [dbo].[ProcessDataTicket] (ProcessDataID, TicketID) SELECT @ProcessDataID,ID FROM @TicketIdTable
				
				SELECT @ModuleId = ModuleID FROM Module WHERE ModuleShort='TICKET'

			
				INSERT INTO [dbo].[EmailApproval]
				(
					ModuleID,
					ID,
					ValidTime,
					IsActive,
					Value,
					LinkID,
					ApproverEmail,
					EmailSubject,
					EmailFrom,
					EmailTo,
					EmailCC,
					EmailBCC,
					EmailBody,
					SendDate,
					SentCount,
					ReminderDuration,
					CreatedBy,
					CreatedDate,
					ModifiedBy,
					ModifiedDate
				)
				SELECT  @ModuleId, T.TicketID, GETDATE()+7,1,'New',NEWID(),NULL,'IHR Ticket created for '+ @CREATEDBY,NULL,ISNULL(CASE WHEN E.WorkEmail IS NOT NULL THEN E.WorkEmail ELSE E.Email END,@ChangeNotificationEmailId),
				@RequesterEmailId + ';' + STUFF(  
					  (SELECT Distinct ';' + U.Email FROM [User] U
					INNER JOIN [UserRole] UR ON UR.UserID = U.UserID
					INNER JOIN [Role] R ON R.RoleID = UR.RoleID
					WHERE R.RoleShort = L1.ValueDesc
					 FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 1, '')
					,NULL,
				'Ticket #'+ CONVERT(VARCHAR(20),T.TicketID) + ' has been Created <br/><ul style=''margin-bottom: 0px;''><li>Type: ' + L.ValueDesc + '<br/></li><li>Requester: ' + @CREATEDBY + '</li><li>Title: ' + T.Title + '</li><li>Description: ' + T.Description + '</li><li>Submitted Date: ' + FORMAT(GETDATE(),'dd MMM yyy HH:mm:ss') + ' GMT </li><li>Comments: </li></ul>',
				NULL, 0,T.ReminderDuration,@CREATEDBY, GETDATE(), NULL,NULL
				FROM ProcessDataTicket PDT 
				INNER JOIN [dbo].[Ticket]  T ON T.TicketID = PDT.TicketID
				INNER JOIN [dbo].[ListValue] L ON T.TicketTypeID = L.ListValueID
				INNER JOIN [dbo].[ListValue] L1 ON L.Value = L1.Value
				INNER JOIN [dbo].[ListType] LT ON LT.ListTypeID = L1.ListTypeID AND LT.Type = 'TicketEmailMap'
				LEFT JOIN Employee E ON E.EmployeeID = T.AssignedToID
				WHERE PDT.ProcessDataID = @ProcessDataId
				
			END
		END