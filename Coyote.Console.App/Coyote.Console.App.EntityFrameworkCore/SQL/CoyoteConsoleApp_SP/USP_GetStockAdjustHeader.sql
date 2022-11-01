CREATE PROCEDURE USP_GetStockAdjustHeader
 @GlobalFilter NVARCHAR(100) = NULL
,@OutletId  INT = NULL
,@SkipCount INT = NULL
,@MaxResultCount   INT = NULL
,@SortColumn NVARCHAR(50) = NULL
,@SortDirection NVARCHAR(50) = NULL
AS

SET NOCOUNT ON;
BEGIN
	DECLARE @SqlQuery NVARCHAR(MAX) , @WhereClause NVARCHAR(Max), @TotalRecordCount INT =0
	IF OBJECT_ID('tempdb..#StockAdjustHeader') IS NOT NULL
		DROP TABLE #StockAdjustHeader
	CREATE TABLE #StockAdjustHeader(
		[Id] [bigint] NOT NULL,
		[OutletId] [int] NOT NULL,
		[PostToDate] [datetime] NULL,
		[Reference] [nvarchar](10) NULL,
		[Total] [real] NOT NULL,
		[CreatedById] [int] NOT NULL,
		[UpdatedById] [int] NOT NULL,
		[CreatedAt] [datetime] NOT NULL,
		[UpdatedAt] [datetime] NOT NULL,
		[IsDeleted] [bit] NOT NULL,
		OutletCode NVARCHAR(100),
		OutletDesc NVARCHAR(200)
	)
	SET @WhereClause=''
	IF @GlobalFilter is not NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND (stre.[Desc] like ''%'+@GlobalFilter+'%'' OR stre.[Code] like ''%'+@GlobalFilter+'%'') '
	END
	IF @OutletId is not NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND stkAdjHed.OutletId = '+ CAST(@OutletId AS VARCHAR)+'  '
	END
	SET @SqlQuery = ''
	SET @SqlQuery = @SqlQuery +'
	INSERT INTO #StockAdjustHeader
	SELECT 
	 stkAdjHed.Id
	,stkAdjHed.OutletId
	,stkAdjHed.PostToDate
	,stkAdjHed.Reference
	,stkAdjHed.Total
	,stkAdjHed.CreatedById
	,stkAdjHed.UpdatedById 
	,stkAdjHed.CreatedAt 
	,stkAdjHed.UpdatedAt
	,stkAdjHed.IsDeleted
	,stre.Code OutletCode
	,stre.[Desc] OutletDesc 
	FROM StockAdjustHeader stkAdjHed with(READUNCOMMITTED) 
	INNER JOIN Store stre with(READUNCOMMITTED)  ON stkAdjHed.OutletId=stre.Id
	WHERE stkAdjHed.IsDeleted = 0 ' + @WhereClause + ' ORDER BY stkAdjHed.UpdatedAT Desc'

	IF @MaxResultCount IS NOT NULL
	BEGIN
		SET @SQLQuery = @SQLQuery + ' OFFSET ('+CAST(@SkipCount AS VARCHAR)+') ROWS FETCH NEXT ISNULL('+CAST(@MaxResultCount AS VARCHAR)+',0) ROWS ONLY'
	END
	EXECUTE(@SqlQuery)
	SET @TotalRecordCount = @@ROWCOUNT
	SET @SqlQuery = ''
	SET @SqlQuery = @SqlQuery +'SELECT * FROM #StockAdjustHeader '
	IF @SortColumn IS NULL
		BEGIN
			SET @SQLQuery = @SQLQuery + '  ORDER BY UpdatedAt DESC '
		END
		ELSE
		BEGIN
		IF @SortDirection IS NULL
			BEGIN
				SET @SQLQuery = @SQLQuery + ' ORDER BY '+@SortColumn+' ASC '
			END
		ELSE
			BEGIN
				SET @SQLQuery = @SQLQuery + ' ORDER BY '+@SortColumn+' DESC '
			END 
		END 
	EXECUTE(@SqlQuery) 

	SELECT @TotalRecordCount TotalRecordCount
	
	DROP TABLE #StockAdjustHeader
END