/****** Object:  StoredProcedure [dbo].[Usp_GetStockOnHandReport]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Usp_GetStockOnHandReport]
 @startDate datetime = null
,@endDate datetime = null
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
SELECT 
	olet_prod.ProductId,
	SUM((olet_prod.CartonCost * olet_prod.QtyOnHand) / NULLIF(prod.CartonQty, 1)) as SUM_ONHAND_COST, 
	SUM(olet_prod.QtyOnHand) as SUM_ONHAND, 
	SUM(olet_prod.QtyOnHand / NULLIF(prod.CartonQty, 1)) as SUM_ONHAND_CTNS, 
	prod.Number PROD_NUMBER,prod.[Desc] PROD_DESC,prod.DepartmentId PROD_DEPARTMENT,
	prod.CartonQty PROD_CARTON_QTY,tx.Code PROD_TAX_CODE,prod.[Replicate] PROD_REPLICATE, 
	dept.[Desc] as DEP_DESC,
	CASE when tx.Code = 'GST' then 
	  SUM(((olet_prod.CartonCost * olet_prod.QtyOnHand) / NULLIF(prod.CartonQty, 1)) / 1.1) 
	 ELSE 
	  SUM((olet_prod.CartonCost * olet_prod.QtyOnHand) / NULLIF(prod.CartonQty, 1)) 
	 END AS SUM_ONHAND_EXCOST
FROM OutletProduct olet_prod 
INNER JOIN Product prod ON  olet_prod.ProductId = prod.Id
INNER JOIN Tax tx ON prod.TaxId=tx.Id
LEFT JOIN Department dept ON prod.DepartmentId=dept.Id
WHERE (olet_prod.[Status] = 1)
AND (olet_prod.QtyOnHand <> 0)  
AND (@suppliers IS NULL OR prod.SupplierId IN (@suppliers)) 
AND (@departmentIds IS NULL OR prod.DepartmentId IN (SELECT * FROM dbo.SplitString(ISNULL(@departmentIds, ''),',')))
AND (@commodityIds IS NULL OR prod.CommodityId IN (SELECT * FROM dbo.SplitString(ISNULL(@commodityIds, ''),',')))
AND (@categoryIds IS NULL OR prod.CategoryId IN (SELECT * FROM dbo.SplitString(ISNULL(@categoryIds, ''),',')))
AND (@groupIds IS NULL OR prod.GroupId IN (SELECT * FROM dbo.SplitString(ISNULL(@groupIds, ''),',')))
AND (@manufacturerIds IS NULL OR prod.ManufacturerId IN (SELECT * FROM dbo.SplitString(ISNULL(@manufacturerIds, ''),','))) 
AND (@productStartId IS NULL OR olet_prod.ProductId >= @productStartId)
AND (@productStartId IS NULL OR olet_prod.ProductId <= @productEndId)
AND (@storeIds IS NULL OR prod.StoreId IN (SELECT * FROM dbo.SplitString(ISNULL(@storeIds, ''),','))) 
GROUP BY prod.DepartmentId
		,prod.[Replicate]
		,prod.[Desc]
		,prod.Number
		,prod.CartonQty
		,tx.Code
		,dept.[Desc]
		,olet_prod.ProductId
ORDER BY olet_prod.ProductId
		,prod.DepartmentId
		,prod.[Replicate]
		,prod.[Desc]
		,prod.Number
		,prod.CartonQty
		,tx.Code
		,dept.[Desc] 
EXEC [dbo].[GetReportsFilterTables] @startDate,@endDate,@productStartId,@productEndId,@promoSales,@promoCode,@summary,@drillDown,@contineous,@variance,@wastage,@merge,@tillId,@storeIds,@zoneIds,@dayRange,@departmentIds,@commodityIds,@categoryIds,@groupIds,@suppliers,@manufacturerIds,null,@reportType
GO
