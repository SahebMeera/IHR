-- =============================================  
-- Author:  Sudeep Kukreti  
-- Create date: 05/17/2020  
-- Description: Procedure to generate DTOs  
-- Change History  
-- =============================================  
-- Date   Changed By  Changes  

-- =============================================  
CREATE PROCEDURE [dbo].[GenerateDTO]   
 @pTableName VARCHAR(50)  
AS  
BEGIN  
  
 SET NOCOUNT ON   
  
 DECLARE @object_id INT,   
 @object_schema NVARCHAR(128),   
 @object_name NVARCHAR(128),   
 @object_type NVARCHAR(2)   
  
 DECLARE cObject CURSOR READ_ONLY FOR   
 SELECT top 1  o.[object_id],   
 s.[name] AS 'schema',   
 REPLACE(o.[name],' ','_'),   
 o.[type]   
 FROM    [sys].[objects] o   
 INNER JOIN [sys].[schemas] AS s ON o.[schema_id] = s.[schema_id]   
 WHERE   o.[type] IN ( 'U', 'V' )   
 AND o.name=@pTableName  
 ORDER BY s.[name], o.[type]   
  
 OPEN cObject   
  
 FETCH NEXT FROM cObject INTO @object_id, @object_schema, @object_name, @object_type   
  
 WHILE @@FETCH_STATUS = 0           
  BEGIN   
   -- scope the class in the schema of the object   
   -- to prevent collisions. e.g. dbo.Customers and testing.Customers   
   PRINT 'using System;'  
   PRINT 'using System.Collections.Generic;'  
   PRINT ''  
   PRINT 'namespace ILT.IHR.DTO'  
   PRINT '{'  
   PRINT '    public partial class '+@object_name+' : AbstractDataObject'  
   PRINT '    {'  
    
    DECLARE @column_id INT,   
    @column_name VARCHAR(128),   
    @column_type VARCHAR(30),   
    @column_nullable BIT,   
    @property_type VARCHAR(30),  
    @column_identity BIT  
    
   -- foreach Column in object   
    DECLARE cColumn CURSOR READ_ONLY   
    FOR SELECT  c.[column_id],   
       REPLACE(c.[name],' ','_'),   
       --t.[name] AS 'type',   
       (SELECT top 1 Name from [sys].[types] t where t.[system_type_id] =c.[system_type_id]) as type,  
       c.[is_nullable],  
       c.[is_identity]  
     FROM    [sys].[all_columns] c   
      -- LEFT JOIN [sys].[types] t ON c.[system_type_id] = t.[system_type_id]   
     WHERE   c.[object_id] = @object_id   
       --AND NOT t.[name] IN ('sysname')  
       AND NOT REPLACE(c.[name],' ','_') IN('CreatedBy','CreatedDate','ModifiedBy','ModifiedDate','Timestamp') -- system objects dupe sysname with nvarchar   
     ORDER BY c.[column_id]     
   OPEN cColumn   
    
   FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity   
   WHILE @@FETCH_STATUS = 0   
    BEGIN   
     -- translate SQL type to CLR type   
     SET @property_type = CASE @column_type   
             WHEN 'image' THEN 'byte[]'   
             WHEN 'text' THEN 'string'   
             WHEN 'uniqueidentifier' THEN 'Guid'   
             WHEN 'date' THEN 'DateTime'   
             WHEN 'time' THEN 'TimeSpan'   
             WHEN 'datetime2' THEN 'DateTime'   
             WHEN 'datetimeoffset'   
             THEN 'DateTimeOffset'   
             WHEN 'tinyint' THEN 'byte'   
             WHEN 'smallint' THEN 'short'   
             WHEN 'int' THEN 'int'   
             WHEN 'smalldatetime' THEN 'DateTime'   
             WHEN 'real' THEN 'Single'   
             WHEN 'money' THEN 'decimal'   
             WHEN 'datetime' THEN 'DateTime'   
             WHEN 'float' THEN 'double'   
             WHEN 'sql_variant' THEN 'object' -- if you know the underlying type, use that   
             WHEN 'ntext' THEN 'string'   
             WHEN 'bit' THEN 'bool'   
             WHEN 'decimal' THEN 'decimal'   
             WHEN 'numeric' THEN 'decimal'   
             WHEN 'smallmoney' THEN 'decimal'   
             WHEN 'bigint' THEN 'long'   
             WHEN 'hierarchyid'   
             THEN 'Microsoft.SqlServer.Types.SqlHierarchyId'   
             WHEN 'geometry' THEN 'Microsoft.SqlServer.Types.SqlGeometry'   
             WHEN 'geography' THEN 'Microsoft.SqlServer.Types.SqlGeography'   
             WHEN 'varbinary' THEN 'byte[]'   
             WHEN 'varchar' THEN 'string'   
             WHEN 'binary' THEN 'byte[]'   
             WHEN 'char' THEN 'string'   
             WHEN 'timestamp' THEN 'byte[]'   
             WHEN 'nvarchar' THEN 'string'   
             WHEN 'nchar' THEN 'string'   
             WHEN 'xml' THEN 'string' -- TODO: figure how to handle.. string?   
             WHEN 'sysname' THEN 'string'   
             ELSE 'object'   
           END    
     -- TODO: handle FILESTREAM attribute varbinary(max) Byte[]   
    
     -- if the CLR type is a reference type, ignore nullable bit on source field   
     IF ( @property_type IN ( 'object', 'string', 'Guid', 'byte[]' ) )    
      SET @column_nullable = 0 ; -- is ref type   
    
     -- if field type effectively is nullable, indicate with '?'   
     SET @property_type = @property_type + CASE @column_nullable   
                WHEN 1 THEN '?'   
                ELSE ''   
                 END   
      -- render property                                                         
    IF  @column_identity = 0  
     PRINT '        public ' + @property_type + ' ' + @column_name+' {get; set;}'   
    ELSE  
     BEGIN  
      PRINT '        public ' + @property_type + ' ' + @column_name  
      PRINT '        {'  
      PRINT '           get{return base.RecordID;}'  
      PRINT '           set{base.RecordID=value;}'  
      PRINT '        }'  
     END  
                      
      FETCH NEXT FROM cColumn INTO @column_id, @column_name, @column_type, @column_nullable, @column_identity    
     END -- column loop   
     CLOSE cColumn   
    DEALLOCATE cColumn   
     
    PRINT '     }'   
    PRINT ' }'       
    FETCH NEXT FROM cObject INTO @object_id, @object_schema, @object_name,   
       @object_type   
   END   
   -- object loop   
 CLOSE cObject   
 DEALLOCATE cObject   
-- render end of root namespace   
END
