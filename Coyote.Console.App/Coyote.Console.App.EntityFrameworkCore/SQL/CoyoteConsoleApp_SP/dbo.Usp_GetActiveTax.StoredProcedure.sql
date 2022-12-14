/****** Object:  StoredProcedure [dbo].[Usp_GetActiveTax]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Usp_GetActiveTax] 
 @GlobalFilter NVARCHAR(100) = NULL
,@SkipCount INT = NULL
,@MaxResultCount   INT = NULL
,@SortColumn NVARCHAR(50) = NULL
,@SortDirection NVARCHAR(50) = NULL
,@IsCountRequired BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @SqlQuery NVARCHAR(MAX)=NULL,@WhereClause  NVARCHAR(MAX)=NULL
	SET @WhereClause=''
	IF @GlobalFilter IS NOT NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND (tx.[Desc] like ''%'+@GlobalFilter+'%'' OR tx.Code like ''%'+@GlobalFilter+'%'')'
	END
	SET @SqlQuery=''
	SET @SqlQuery = @SqlQuery +'
	SELECT  
	 tx.Id
	,tx.Code
	,tx.[Desc]
	,tx.Factor
	,tx.[Status]
	,tx.CreatedAt
	,tx.UpdatedAt
	,tx.CreatedById
	,tx.UpdatedById
	FROM Tax tx
	WHERE tx.IsDeleted=0'
	
	IF @WhereClause IS NOT NULL
	BEGIN
		SET  @SqlQuery = @SqlQuery + @WhereClause
	END

	IF @SortColumn IS NULL
	BEGIN
		SET @SqlQuery = @SqlQuery +' ORDER BY tx.UpdatedAt DESC'
	END
	ELSE
	BEGIN
		IF @SortDirection IS NULL
		BEGIN
		SET @SqlQuery = @SqlQuery +' ORDER BY tx.['+@SortColumn+'] ASC'
		END
		ELSE
		BEGIN
			SET @SqlQuery = @SqlQuery +' ORDER BY tx.['+@SortColumn+'] DESC'
		END
	END
	IF @MaxResultCount IS NOT NULL
	BEGIN
		SET @SqlQuery = @SqlQuery +' OFFSET ('+CAST(@SkipCount AS VARCHAR)+') ROWS FETCH NEXT ISNULL('+CAST(@MaxResultCount AS VARCHAR)+',0) ROWS ONLY'
	END
	EXECUTE(@SqlQuery)
	
	IF @IsCountRequired IS NULL OR @IsCountRequired = 1
	BEGIN
		SELECT COUNT(1) TotalRecordCount FROM Tax WHERE IsDeleted=0
	END
END
GO
