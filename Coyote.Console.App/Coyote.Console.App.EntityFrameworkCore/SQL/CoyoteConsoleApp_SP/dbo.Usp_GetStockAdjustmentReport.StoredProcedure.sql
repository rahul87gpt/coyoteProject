/****** Object:  StoredProcedure [dbo].[Usp_GetStockAdjustmentReport]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Usp_GetStockAdjustmentReport]
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
,@reportType nvarchar(200) = null
AS
BEGIN
	SELECT 
		 SUM(trans.Cost  - trans.ExGSTCost) as SUM_EXCOST
		,SUM(trans.StockMovement) as SUM_MOVEMENT_UNITS
		,SUM(trans.StockMovement / NULLIF(trans.CartonQty, 0)) as SUM_MOVEMENT_CTNS
		,trans.Tender TRX_TENDER
		,trans.ProductId TRX_PRODUCT
		,trans.CartonQty TRX_CARTON_QTY
		,prod.[Desc] PROD_DESC
		,tx.Code PROD_TAX_CODE
		,dept.Code PROD_DEPARTMENT
		,mstItm.[Name] as ADJ_DESC 
	FROM [TransactionReport] trans 
	INNER join [MasterListItems] mstItm ON mstItm.Code=trans.Tender 
	INNER join MasterList mst ON mstItm.ListId=mst.Id and mst.Code='ADJUSTCODE' 
	LEFT JOIN Product prod ON trans.ProductId = prod.Id
	LEFT JOIN Department dept ON dept.Id = prod.DepartmentId
	LEFT JOIN Tax tx on tx.Id=prod.TaxId
	WHERE (trans.[Date] BETWEEN @startDate AND @endDate)
		AND (@storeIds IS NULL OR trans.OutletId IN (SELECT * FROM Dbo.SplitString(ISNULL(@storeIds, ''),','))) 
		AND (@productStartId IS NULL OR trans.ProductId >= @productStartId)
		AND (@productEndId IS NULL OR trans.ProductId <= @productEndId)
		AND (@tillId IS NULL OR trans.TillId = @tillId) 
		AND (@commodityIds IS NULL OR trans.CommodityId IN (SELECT * FROM Dbo.SplitString(ISNULL(@commodityIds, ''),',')))
		AND (@departmentIds IS NULL OR trans.DepartmentId IN (SELECT * FROM Dbo.SplitString(ISNULL(@departmentIds, ''),',')))
		AND (@categoryIds IS NULL OR trans.CategoryId IN (SELECT * FROM Dbo.SplitString(ISNULL(@categoryIds, ''),',')))
		AND (@groupIds IS NULL OR trans.[Group] IN (SELECT * FROM Dbo.SplitString(ISNULL(@groupIds, ''),','))) 
		AND (@suppliers IS NULL OR trans.SupplierId IN (@suppliers))
		AND (@manufacturerIds IS NULL OR trans.ManufacturerId IN (SELECT * FROM Dbo.SplitString(ISNULL(@manufacturerIds, ''),','))) 
		AND (trans.[Type] = 'ADJUSTMENT' OR trans.[Type] = 'WASTAGE')
	GROUP BY trans.Tender
			,dept.Code, prod.[Desc]
			,trans.ProductId
			,trans.CartonQty
			,tx.Code
			,mstItm.[Name] 
	ORDER BY trans.Tender
			,dept.Code
			,prod.[Desc]
			,trans.ProductId
			,trans.CartonQty
			,tx.Code, mstItm.[Name] 

	EXEC [dbo].[GetReportsFilterTables] @startDate,@endDate,@productStartId,@productEndId,@promoSales,@promoCode,@summary,@drillDown,@contineous,@variance,@wastage,@merge,@tillId,@storeIds,@zoneIds,@dayRange,@departmentIds,@commodityIds,@categoryIds,@groupIds,@suppliers,@manufacturerIds,null,@reportType  
END


 --SELECT * FROM [TransactionReport] t
 --INNER join [MasterListItems] m ON m.Code=t.Tender 
 --INNER join MasterList mst ON m.ListId=mst.Id and mst.Code='ADJUSTCODE' 
 --WHERE 
 --t.Tender is not null AND ([Type] = 'ADJUSTMENT' OR [Type] = 'WASTAGE')
GO
