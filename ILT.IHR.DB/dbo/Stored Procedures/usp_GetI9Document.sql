CREATE PROCEDURE [dbo].[usp_GetI9Document]
	@I9DocumentID	int	=	NULL,
	@I9DocName	varchar(100)	=	NULL,
	@I9DocType	varchar(50)	=	NULL,
	@WorkAuthID	int	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		I9DocumentID,
		I9DocName,
		I9DocTypeID,
		WorkAuthID,
		CreatedBy,
		CreatedDate,
		ModifiedBy,
		ModifiedDate,
		TimeStamp
	FROM [dbo].[I9Document]
	WHERE I9DocumentID = ISNULL(@I9DocumentID, I9DocumentID)
END