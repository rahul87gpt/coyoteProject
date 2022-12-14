
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_GetSalesChartDetailedReport]
@startDate datetime,
@endDate datetime,
@storeIds nvarchar(max) = null,
@productStartId bigint = null,
@productEndId bigint = null,
@tillId bigint = null,
@commodityIds nvarchar(max) = null,
@departmentIds nvarchar(max) = null,
@categoryIds nvarchar(max) = null,
@groupIds nvarchar(max) = null,
@suppliers nvarchar(max) = NULL,
@manufacturerIds nvarchar(max) = null,
@isPromoSales bit = 0,
@promocode nvarchar(50) = null,
@ChartReportFor NVARCHAR(50) = null
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @SQLQuery NVARCHAR(MAX) = NULL,@WhereClause  NVARCHAR(MAX) = NULL, @GroupByClause NVARCHAR(MAX) = NULL, @CoulmnList NVARCHAR(MAX) = NULL
	SET @SQLQuery = ''
	
	SET @WhereClause = ' WHERE (trans.[Date] >= '''+ CONVERT(NVARCHAR(100),@startDate,110)+''') 
	AND (trans.[Date] <= '''+CONVERT(NVARCHAR(100),@endDate,110)+''') AND (trans.[Type] = ''ITEMSALE'')'

	SET @CoulmnList = ' SELECT ROUND(SUM(CASE WHEN '+CAST(@isPromoSales AS VARCHAR)+' = 1 THEN ISNULL(PromoSales,0) ELSE trans.Amt END),2) as TRX_AMT,'
	
	BEGIN
		SET @CoulmnList = @CoulmnList +' prod.[Desc] PROD_DESC, 
										trans.DepartmentId as DEP_ID,
										D.[Desc] as DEP_DESC,
										trans.CommodityId as COM_ID, 
										C.[Desc] as COM_DESC,
										trans.[Group] as GRP_ID, 
										G.[Name] as GRP_DESC, 
										trans.CategoryId as CAT_ID,
										CAT.[Name] as CAT_DESC '	
		SET @GroupByClause = ' GROUP BY prod.[Desc],
								trans.DepartmentId,
								D.[Desc],
								trans.CommodityId, 
								C.[Desc],
								trans.[Group], 
								G.[Name], 
								trans.CategoryId,
								CAT.[Name]'
	END
	
	IF @productStartId IS NOT NULL
	BEGIN
		SET @WhereClause=@WhereClause + ' AND (prod.[Number] >='+ CAST(@productStartId AS VARCHAR)+' )'
	END 
	IF @productEndId IS NOT NULL
	BEGIN
		SET @WhereClause=@WhereClause + ' AND (prod.[Number] <='+ CAST(@productEndId AS VARCHAR)+' )'
	END 

	IF @tillId IS NOT NULL AND @tillId > 0 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND trans.TillId = @tillId '
	END 
	IF @isPromoSales = 1 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND ISNULL(PromoSales,0) <> 0'
	END 
	IF @storeIds IS NOT NULL 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND  trans.OutletId IN (SELECT * FROM [dbo].[SplitString](ISNULL('''+@storeIds+''', ''''),'',''))'
	END
	IF @commodityIds IS NOT NULL 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND trans.CommodityId IN (SELECT * FROM [dbo].[SplitString](ISNULL('''+@commodityIds+''', ''''),'',''))'
	END
	IF @departmentIds IS NOT NULL 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND trans.DepartmentId IN (SELECT * FROM [dbo].[SplitString](ISNULL('''+@departmentIds+''', ''''),'',''))'
	END
	IF @categoryIds IS NOT NULL 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND trans.[CategoryId] IN (SELECT * FROM [dbo].[SplitString](ISNULL('''+@categoryIds+''', ''''),'',''))'
	END
	IF @groupIds IS NOT NULL 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND trans.[Group] IN (SELECT * FROM [dbo].[SplitString](ISNULL('''+@groupIds+''', ''''),'',''))'
	END
	IF @suppliers IS NOT NULL 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND trans.SupplierId IN (SELECT * FROM [dbo].[SplitString](ISNULL('''+@suppliers+''', ''''),'',''))'
	END
	IF @manufacturerIds IS NOT NULL 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND trans.ManufacturerId IN (SELECT * FROM [dbo].[SplitString](ISNULL('''+@manufacturerIds+''', ''''),'',''))'
	END
	IF @tillId IS NOT NULL 
	BEGIN
		SET @WhereClause=@WhereClause + ' AND TRX_TILL IN (SELECT * FROM [dbo].[SplitString](ISNULL('''+@tillId+''', ''''),'',''))'
	END
	
	SET @SQLQuery = @SQLQuery + @CoulmnList

	SET @SQLQuery = @SQLQuery +'
	FROM  [Transaction] trans with(readuncommitted)
	LEFT JOIN Product prod with(readuncommitted) ON trans.ProductId= prod.Id
	LEFT JOIN Store stre with(readuncommitted) ON trans.OutletId = stre.Id
	LEFT JOIN Department D with(readuncommitted) ON trans.DepartmentId=d.Id
	LEFT JOIN Commodity C with(readuncommitted) ON trans.CommodityId=C.Id
	LEFT JOIN MasterListItems G with(readuncommitted) ON trans.[Group]=G.Id
	LEFT JOIN MasterListItems CAT with(readuncommitted) ON trans.CategoryId=CAT.Id'
	
	IF @ChartReportFor='Supplier'
	BEGIN
		SET @SQLQuery = @SQLQuery +' LEFT JOIN Supplier S with(readuncommitted) ON trans.SupplierId=S.Id '
	END
	
	SET @SQLQuery = @SQLQuery + '' + CHAR(13) + CHAR(10) + '' + @WhereClause + '' + CHAR(13) + CHAR(10) + '' + @GroupByClause

	EXECUTE(@SQLQuery)

END
GO
