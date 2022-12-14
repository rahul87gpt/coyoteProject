/****** Object:  StoredProcedure [dbo].[usp_GetActiveCommodities]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_GetActiveCommodities] 
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
	Select 
	 cmdty.Id
	,cmdty.Code
	,cmdty.[Desc]
	,cmdty.CoverDays
	,cmdty.DepartmentId
	,cmdty.GPPcntLevel1
	,cmdty.GPPcntLevel2
	,cmdty.GPPcntLevel3
	,cmdty.GPPcntLevel4
	,cmdty.[Status]
	,cmdty.CreatedAt
	,cmdty.UpdatedAt
	,cmdty.CreatedById
	,cmdty.UpdatedById
	,dept.[Desc] Department
	FROM Commodity cmdty with(READUNCOMMITTED)
	INNER JOIN Department dept with(READUNCOMMITTED) ON cmdty.DepartmentId=dept.Id
	WHERE cmdty.IsDeleted=0 '
	IF @SortColumn IS NULL
	BEGIN
		SET @SqlQuery = @SqlQuery +' ORDER BY cmdty.UpdatedAt DESC '
	END
	ELSE
	BEGIN
		IF @SortDirection IS NULL
		BEGIN
		SET @SqlQuery = @SqlQuery +' ORDER BY cmdty.['+ @SortColumn +'] ASC '
		END
		ELSE
		BEGIN
			SET @SqlQuery = @SqlQuery +' ORDER BY cmdty.['+ @SortColumn +'] DESC '
		END
	END
	pRINT @SqlQuery
	IF @MaxResultCount IS NOT NULL
	BEGIN
		SET @SqlQuery = @SqlQuery +' OFFSET ('+CAST(@SkipCount AS VARCHAR)+') ROWS FETCH NEXT ISNULL('+CAST(@MaxResultCount AS VARCHAR)+',0) ROWS ONLY'
	END

	EXECUTE(@SqlQuery)
	IF @IsCountRequired = 1
	BEGIN
		SELECT COUNT(1) TotalRecordCount FROM Commodity WHERE IsDeleted=0
	END
END
GO
