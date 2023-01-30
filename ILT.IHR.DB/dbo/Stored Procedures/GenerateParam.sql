-- =============================================  
-- Author:  Sudeep Kukreti
-- Create date: 05/17/2020  
-- Description: Procedure to generate Store Proc
-- Change History  
-- =============================================  
-- Date			Changed By		Changes

-- =============================================

CREATE PROCEDURE [dbo].[GenerateParam]
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
			PRINT 'base.parms.Add("@' + @column_name +'", obj.'+@column_name + ');'
			/*
			--base.SQLServerConnObj.AddParameter("@pLastName", obj.LastName, SqlDbType.NVarChar, 30, ParameterDirection.Input);
			if @column_type='varchar' 
				PRINT 'base.SQLServerConnObj.AddParameter("@' + @column_name +'", obj.'+@column_name +', SqlDbType.NVarChar, '+  @column_size + ', ParameterDirection.Input);' 
			else if @column_type='int'
				PRINT 'base.SQLServerConnObj.AddParameter("@' + @column_name +'", obj.'+@column_name +', SqlDbType.Int, sizeof(int), ParameterDirection.Input);' 
			else if @column_type='bit' 
				PRINT 'base.SQLServerConnObj.AddParameter("@' + @column_name +'", obj.'+@column_name +', SqlDbType.Bit, 1, ParameterDirection.Input);' 
			else if @column_type in ('date','datetime')  
				PRINT 'base.SQLServerConnObj.AddParameter("@' + @column_name +'", obj.'+@column_name +', SqlDbType.DateTime, 8, ParameterDirection.Input);' 
			*/
			FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity, @column_size    
		END -- column loop   
		CLOSE cColumn   
		DEALLOCATE cColumn   
	END   
 -- render end of root namespace   
END
