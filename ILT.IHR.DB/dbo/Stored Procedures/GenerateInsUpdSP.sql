-- =============================================  
-- Author:  Sudeep Kukreti
-- Create date: 05/17/2020  
-- Description: Procedure to generate Store Proc
-- Change History  
-- =============================================  
-- Date			Changed By		Changes

-- =============================================

CREATE PROCEDURE [dbo].[GenerateInsUpdSP]
	@pTableName VARCHAR(50)  
AS  
BEGIN  
	SET NOCOUNT ON
	DECLARE @PK_Column VARCHAR(128)   

	SELECT 
		@PK_Column=c.COLUMN_NAME 
	FROM 	
		INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk ,
		INFORMATION_SCHEMA.KEY_COLUMN_USAGE c
	WHERE
	 	pk.TABLE_NAME = LTRIM(RTRIM(@pTableName))
		AND	CONSTRAINT_TYPE = 'PRIMARY KEY'
		AND	c.TABLE_NAME = pk.TABLE_NAME
		AND	c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME  

	BEGIN   
		PRINT '--==============================================================='  
		PRINT '-- Author : Sudeep Kukreti'
		PRINT '-- Created Date : ' + CONVERT(VARCHAR(10), GETDATE(), 101)  
		PRINT '-- Description : Insert/Update SP for '+ LTRIM(RTRIM(@pTableName))  
		PRINT '--'  
		PRINT '-- Revision History:'  
		PRINT '-----------------------------------------------------------------'  
		PRINT '-- Date            By          Description'  
		PRINT '--'   
		PRINT '--==============================================================='  
		PRINT ''  
		PRINT 'CREATE PROCEDURE [dbo].[usp_InsUpd' +LTRIM(RTRIM(@pTableName))+']'
		PRINT '	@'+@PK_Column+'	int	=	NULL,'
		
		DECLARE @column_id VARCHAR(5),   
		@column_name VARCHAR(128),   
		@column_type VARCHAR(30),   
		@column_nullable VARCHAR(5),   
		@property_type VARCHAR(30),  
		@column_identity VARCHAR (5) ,
		@column_size varchar(5)

		DECLARE cColumn CURSOR READ_ONLY  FOR 
		SELECT 
			ORDINAL_POSITION, column_name, DATA_TYPE, IS_NULLABLE, TABLE_SCHEMA, CHARACTER_MAXIMUM_LENGTH
		FROM 
			INFORMATION_SCHEMA.COLUMNS
		WHERE 
			table_name = LTRIM(RTRIM(@pTableName)) AND column_name <> @PK_Column AND column_name <> 'TimeStamp'
		ORDER BY 
			ORDINAL_POSITION ASC
	        
		OPEN cColumn   
	    
		FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity , @column_size  
		WHILE @@FETCH_STATUS = 0   
		BEGIN   

		-- TODO: handle FILESTREAM attribute varbinary(max) Byte[]   
		-- if the CLR type is a reference type, ignore nullable bit on source field   
		IF ( @property_type IN ( 'object', 'string', 'Guid', 'byte[]' ) )    
		SET @column_nullable = 'NO' ; -- is ref type   
	    
		 -- if field type effectively is nullable, indicate with '?'   
		 --SET @property_type = @property_type + CASE UPPER(@column_nullable)
			--	WHEN 'YES' THEN '''NULL'''   
   --             ELSE 'NOT NULL'   
   --             END   
      -- render property  
		if @column_type='varchar' 
			PRINT '	@' + @column_name +'	' + @column_type  + '('+  @column_size + ')	=	NULL,' -- + @property_type    
		else
		    PRINT '	@' + @column_name +'	' + @column_type + '	=	NULL,' -- 
                      
			FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity, @column_size    
		END -- column loop   
		CLOSE cColumn   
		PRINT '	@TimeStamp	timestamp	=	NULL,'
		PRINT '	@ReturnCode INT = 0 OUTPUT'

		-- render end of class   
		PRINT 'AS '       
		PRINT 'BEGIN'
		PRINT '	SET NOCOUNT ON; '
		PRINT '	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED'
		PRINT ''
		PRINT '	DECLARE @IDTable TABLE(ID INT)'
		PRINT ''
		PRINT '	IF NOT EXISTS(SELECT 1 FROM ' + @pTableName + ' WHERE ' + @PK_Column + ' = ISNULL(@' + @PK_Column+ ',0))'
		PRINT '	BEGIN'
		PRINT '		INSERT INTO [dbo].[' +LTRIM(RTRIM(@pTableName))+']'
		PRINT '		('
		OPEN cColumn   
	    
		FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity , @column_size  
		WHILE @@FETCH_STATUS = 0   
		BEGIN   
		 PRINT '			' + @column_name +','
		FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity, @column_size
		END -- column loop   
		CLOSE cColumn
		PRINT '		)'
		PRINT '		OUTPUT INSERTED.' + @PK_Column +' INTO @IDTable'
		PRINT '		VALUES'
		PRINT '		('
		OPEN cColumn   
		FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity , @column_size  
		WHILE @@FETCH_STATUS = 0   
		BEGIN   
		 PRINT '			@' + @column_name +','
		FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity, @column_size
		END -- column loop   
		CLOSE cColumn
		
		PRINT '		)'
		PRINT '		SELECT @'+@PK_Column +'=(SELECT ID FROM @IDTable);'
		PRINT 'END'
		PRINT '	ELSE'
		PRINT '		UPDATE [dbo].[' +LTRIM(RTRIM(@pTableName))+']'
		PRINT '		SET'
		OPEN cColumn   
	    
		FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity , @column_size  
		WHILE @@FETCH_STATUS = 0   
		BEGIN   
		 PRINT '			' + @column_name + ' = @' + @column_name +','
		FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity, @column_size
		END -- column loop   
		CLOSE cColumn
		DEALLOCATE cColumn   
		PRINT '		WHERE '+@PK_Column +' = @' +@PK_Column
 		PRINT ''
		PRINT '	IF @@ERROR=0'
		PRINT '		SET @ReturnCode  = @'+@PK_Column +';'  
		PRINT '	ELSE'
		PRINT '		SET @ReturnCode = 0'
		PRINT '	RETURN'
		PRINT ''
		PRINT 'END'
	END   
 -- render end of root namespace   
END
