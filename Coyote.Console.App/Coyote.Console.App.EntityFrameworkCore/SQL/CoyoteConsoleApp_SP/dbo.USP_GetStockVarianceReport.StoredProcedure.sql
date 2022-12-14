/****** Object:  StoredProcedure [dbo].[USP_GetStockVarianceReport]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_GetStockVarianceReport]
 @startDate datetime
,@endDate datetime
,@productStartId bigint = null
,@productEndId bigint = null
,@promoSales bit=0
,@promoCode nvarchar(max) = null
,@summary bit=0
,@drillDown bit=0
,@contineous bit=0
,@variance bit=0
,@wastage bit=0
,@merge bit=0
,@tillId bigint = null
,@storeIds nvarchar(max) = null
,@zoneIds nvarchar(max) = null
,@dayRange nvarchar(max) = null
,@departmentIds nvarchar(max) = null
,@commodityIds nvarchar(max) = null
,@categoryIds nvarchar(max) = null
,@groupIds nvarchar(max) = null
,@suppliers nvarchar(max) = null
,@manufacturerIds nvarchar(max) = null
,@productIds nvarchar(max) = null
,@reportType nvarchar(200)
 
AS
SET NOCOUNT ON;
BEGIN
	SELECT SUM(trx.Cost - trx.ExGSTCost) AS SUM_EXCOST
		,SUM(trx.NewOnHand) AS SUM_NEW_ONHAND
		,SUM(trx.StockMovement) AS SUM_MOVEMENT_UNITS
		,SUM(trx.StockMovement / NULLIF(trx.CartonQty, 0)) AS SUM_MOVEMENT_CTNS
		,D.Code TRX_DEPARTMENT
		,p.Number TRX_PRODUCT
		,trx.CartonQty TRX_CARTON_QTY
		,p.[Desc] PROD_DESC
		,t.Code PROD_TAX_CODE
		,p.[Replicate] PROD_REPLICATE
		,D.[Desc] AS DEP_DESC
	FROM [Transaction] trx with(readuncommitted)
	LEFT JOIN Department D with(readuncommitted) ON D.Id = trx.DepartmentId
	LEFT JOIN Product p with(readuncommitted) ON p.Id = trx.ProductId
	LEFT JOIN Tax t with(readuncommitted) ON t.Id = p.TaxId
	WHERE (trx.DATE BETWEEN @startDate AND @endDate )
		AND (trx.StockMovement <> 0)
		AND (@storeIds IS NULL OR trx.OutletId IN (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
		AND (@productStartId IS NULL OR trx.ProductId >= @productStartId)
		AND (@productStartId IS NULL OR trx.ProductId <= @productEndId)
		AND (@tillId IS NULL OR trx.TillId = @tillId) 
		AND (@commodityIds IS NULL OR trx.CommodityId IN (select * from [dbo].[SplitString](isnull(@commodityIds,''),',')))
		AND (@departmentIds IS NULL OR trx.DepartmentId IN (select * from [dbo].[SplitString](isnull(@departmentIds,''),',')))
		AND (@categoryIds IS NULL OR trx.CategoryId IN (select * from [dbo].[SplitString](isnull(@categoryIds,''),',')))
		AND (@groupIds IS NULL OR trx.[Group] IN (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
		AND (@suppliers IS NULL OR trx.SupplierId IN (select * from [dbo].[SplitString](isnull(@suppliers,''),',')))
		AND (@manufacturerIds IS NULL OR trx.ManufacturerId IN (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),',')))
		AND (trx.[Type] = 'VARIANCE')
	GROUP BY D.Code
		,p.Number
		,p.[Replicate]
		,p.[Desc]
		,trx.ProductId
		,trx.CartonQty
		,t.Code
		,D.[Desc]
	ORDER BY TRX_DEPARTMENT
		,PROD_REPLICATE
		,PROD_DESC
		,trx.ProductId
		,TRX_CARTON_QTY
		,PROD_TAX_CODE
		,D.[Desc]

	EXEC [dbo].[GetReportsFilterTables] @startDate
		,@endDate
		,@productStartId
		,@productEndId
		,@promoSales
		,@promoCode
		,@summary
		,@drillDown
		,@contineous
		,@variance
		,@wastage
		,@merge
		,@tillId
		,@storeIds
		,@zoneIds
		,@dayRange
		,@departmentIds
		,@commodityIds
		,@categoryIds
		,@groupIds
		,@suppliers
		,@manufacturerIds
		,NULL
		,@reportType
END
GO
