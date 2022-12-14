/****** Object:  StoredProcedure [dbo].[Usp_GetStockWastageReportProductWise]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Usp_GetStockWastageReportProductWise]
-- [dbo].[GetStockWastageReportProductWise] '2020-03-01','2020-08-18',null,null,null,null,null,null
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
As
SELECT 
SUM(trx.Cost - trx.ExGSTCost) as SUM_EXCOST, SUM(trx.StockMovement) as SUM_MOVEMENT_UNITS, SUM(trx.StockMovement / NULLIF(trx.CartonQty, 0)) as SUM_MOVEMENT_CTNS, 
D.Code TRX_DEPARTMENT,p.Number TRX_PRODUCT,trx.CartonQty TRX_CARTON_QTY,p.[Desc] PROD_DESC,t.Code PROD_TAX_CODE,D.[Desc] as DEP_DESC 
FROM [Transaction] trx LEFT JOIN Department D ON D.Id = trx.DepartmentId
LEFT JOIN Product p ON p.Id = trx.ProductId 
LEFT JOIN Tax t on t.Id=p.TaxId
WHERE (trx.Date BETWEEN @startDate AND @endDate)  
AND (@storeIds IS NULL OR trx.OutletId IN (@storeIds)) 
AND (@productStartId IS NULL OR trx.ProductId >= @productStartId)
AND (@productStartId IS NULL OR trx.ProductId <= @productEndId)
AND (@tillId IS NULL OR trx.TillId = @tillId) 
AND (@commodityIds IS NULL OR trx.CommodityId IN (@commodityIds))
AND (@departmentIds IS NULL OR trx.DepartmentId IN (@departmentIds))
AND (@categoryIds IS NULL OR trx.CategoryId IN (@categoryIds))
AND (@groupIds IS NULL OR trx.[Group] IN (@groupIds)) 
AND (@suppliers IS NULL OR trx.SupplierId IN (@suppliers))
AND (@manufacturerIds IS NULL OR trx.ManufacturerId IN (@manufacturerIds))  
AND (trx.[Type] = 'WASTAGE' OR trx.Tender = 'WASTAGE')
GROUP BY D.Code, p.[Desc], p.Number, trx.CartonQty, t.Code, D.[Desc] 
ORDER BY D.Code--, PROD_DESC, TRX_PRODUCT, TRX_CARTON_QTY, PROD_TAX_CODE, CODE_DESC 

EXEC [dbo].[GetReportsFilterTables] @startDate,@endDate,@productStartId,@productEndId,@promoSales,@promoCode,@summary,@drillDown,@contineous,@variance,@wastage,@merge,@tillId,@storeIds,@zoneIds,@dayRange,@departmentIds,@commodityIds,@categoryIds,@groupIds,@suppliers,@manufacturerIds,null,@reportType  
GO
