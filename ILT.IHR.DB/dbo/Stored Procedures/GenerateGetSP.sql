-- =============================================  
-- Author:  Sudeep Kukreti
-- Create date: 05/17/2020  
-- Description: Procedure to generate Store Proc
-- Change History  
-- =============================================  
-- Date			Changed By		Changes

-- =============================================

CREATE PROCEDURE [dbo].[GenerateGetSP]
	@pTableName VARCHAR(50)  
AS  
BEGIN  
	SET NOCOUNT ON
	DECLARE @PK_Column VARCHAR(128)   
	DECLARE @AliasName VARCHAR(1) = @pTableName

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
		PRINT '-- Description : Select SP for '+ LTRIM(RTRIM(@pTableName))  
		PRINT '--'  
		PRINT '-- Revision History:'  
		PRINT '-----------------------------------------------------------------'  
		PRINT '-- Date            By          Description'  
		PRINT '--'   
		PRINT '--==============================================================='  
		PRINT ''  
		PRINT 'CREATE PROCEDURE [dbo].[usp_Get' +LTRIM(RTRIM(@pTableName))+']'
		
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
			table_name = LTRIM(RTRIM(@pTableName)) 
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

    
		-- render end of class   
		PRINT 'AS '       
		PRINT 'BEGIN'
		PRINT '	SET NOCOUNT ON;'
		PRINT '	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;'
		PRINT ''
		PRINT '	SELECT '
		OPEN cColumn   
	    
		FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity , @column_size  
		WHILE @@FETCH_STATUS = 0   
		BEGIN   
		 PRINT '		' + @AliasName+ '.' + @column_name +','
		FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity, @column_size
		END -- column loop   
		CLOSE cColumn   
		DEALLOCATE cColumn   
		PRINT '	FROM [dbo].[' + LTRIM(RTRIM(@pTableName) +']') + ' ' + @AliasName
		PRINT '	WHERE ' + @AliasName+ '.' + @PK_Column + ' = @' + @PK_Column	
		PRINT 'END'
	END   
 -- render end of root namespace   
END
