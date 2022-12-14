/****** Object:  StoredProcedure [dbo].[usp_GetActiveDepartments]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GetActiveDepartments]   
 @GlobalFilter NVARCHAR(100) = NULL
,@SkipCount INT = NULL
,@MaxResultCount   INT = NULL
,@SortColumn NVARCHAR(50) = NULL
,@SortDirection NVARCHAR(50) = NULL
,@IsCountRequired BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @SqlQuery NVARCHAR(MAX)=NULL
	SET @SqlQuery=''
	SET @SqlQuery = @SqlQuery +'
	SELECT 
	 dept.Id
	,dept.Code
	,dept.[Desc]
	,dept.MapTypeId
	,dept.BudgetGroethFactor
	,dept.RoyaltyDisc
	,dept.AdvertisingDisc
	,dept.AllowSaleDisc
	,dept.ExcludeWastageOptimalOrdering
	,dept.IsDefault
	,dept.[Status]
	,dept.CreatedAt
	,dept.UpdatedAt
	,dept.CreatedById
	,dept.UpdatedById
	,mstItms.[Name] MapType
	FROM Department dept
	LEFT OUTER JOIN MasterListItems mstItms ON dept.MapTypeId=mstItms.Id
	WHERE dept.IsDeleted = 0'
	IF @SortColumn IS NULL
	BEGIN
		SET @SqlQuery = @SqlQuery +' ORDER BY dept.UpdatedAt DESC'
	END
	ELSE
	BEGIN
		IF @SortDirection IS NULL
		BEGIN
		SET @SqlQuery = @SqlQuery +' ORDER BY dept.['+@SortColumn+'] ASC'
		END
		ELSE
		BEGIN
			SET @SqlQuery = @SqlQuery +' ORDER BY dept.['+@SortColumn+'] DESC'
		END
	END
	IF @MaxResultCount IS NOT NULL
	BEGIN
		SET @SqlQuery = @SqlQuery +' OFFSET ('+CAST(@SkipCount AS VARCHAR)+') ROWS FETCH NEXT ISNULL('+CAST(@MaxResultCount AS VARCHAR)+',0) ROWS ONLY'
	END
	EXECUTE(@SqlQuery)
	
	IF @IsCountRequired = 1
	BEGIN
		SELECT COUNT(1) TotalRecordCount FROM Department WHERE IsDeleted=0
	END
END
GO
