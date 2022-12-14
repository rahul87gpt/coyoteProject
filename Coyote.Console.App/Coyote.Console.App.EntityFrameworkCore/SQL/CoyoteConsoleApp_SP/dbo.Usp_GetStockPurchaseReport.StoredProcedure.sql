/****** Object:  StoredProcedure [dbo].[Usp_GetStockPurchaseReport]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Usp_GetStockPurchaseReport] 
 @invoiceStartDate datetime 
,@invoiceEndDate datetime
,@startDate datetime
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
,@useInvoiceDates INT = 0
AS
BEGIN
IF @useInvoiceDates = 0 
BEGIN
	SET @invoiceStartDate = @startDate
	SET @invoiceEndDate = @endDate
END
SELECT 
 SUM(ordr_dtl.FinalLineTotal - ordr_hedr.GstAmt) AS SUM_EXCOST
,SUM(ordr_dtl.FinalLineTotal) AS SUM_COST
,SUM(ordr_hedr.GstAmt) AS SUM_GST
,SUM(ordr_dtl.TotalUnits) AS SUM_UNITS
,supler.Code ORDH_SUPPLIER
,supler.[Desc] AS SUPP_NAME
,prod.Number PROD_NUMBER
,PROD.[Desc] PROD_DESC
,prod.DepartmentId PROD_DEPARTMENT
,Cast(0.0 AS FLOAT) AS RebateAmount
,ordr_hedr.OutletId ORDL_OUTLET
,stre.[Desc] OUTL_DESC
,ordr_hedr.InvoiceDate ORDH_INVOICE_DATE
FROM OrderHeader ordr_hedr
INNER JOIN OrderDetail ordr_dtl ON ordr_dtl.OrderHeaderId=ordr_hedr.Id 
INNER JOIN Store stre ON ordr_hedr.OutletId=stre.Id
INNER JOIN Product prod ON ordr_dtl.ProductId=prod.Id
LEFT OUTER JOIN Supplier supler ON ordr_hedr.SupplierId=supler.Id
INNER JOIN MasterListItems odr_tp ON ordr_hedr.TypeId = odr_tp.Id
WHERE odr_tp.Code='INVOICE' AND ordr_dtl.TotalUnits <> 0
AND (CASE WHEN @useInvoiceDates = 1 THEN ordr_hedr.InvoiceDate ELSE ordr_hedr.PostedDate END 
BETWEEN @invoiceStartDate AND @invoiceEndDate) 
AND (@productStartId IS NULL OR ordr_dtl.ProductId >= @productStartId)
AND (@productStartId IS NULL OR ordr_dtl.ProductId <= @productEndId)
AND (@storeIds IS NULL OR ordr_hedr.OutletId IN (SELECT * FROM dbo.SplitString(ISNULL(@storeIds, ''),','))) 
AND (@departmentIds IS NULL OR prod.DepartmentId IN (SELECT * FROM dbo.SplitString(ISNULL(@departmentIds, ''),',')))
AND (@commodityIds IS NULL OR prod.CommodityId IN (SELECT * FROM dbo.SplitString(ISNULL(@commodityIds, ''),',')))
AND (@categoryIds IS NULL OR prod.CategoryId IN (SELECT * FROM dbo.SplitString(ISNULL(@categoryIds, ''),',')))
AND (@groupIds IS NULL OR prod.GroupId IN (SELECT * FROM dbo.SplitString(ISNULL(@groupIds, ''),',')))
AND (@manufacturerIds IS NULL OR prod.ManufacturerId IN (SELECT * FROM dbo.SplitString(ISNULL(@manufacturerIds, ''),',')))
AND  (@suppliers IS NULL OR ordr_hedr.SupplierId IN (SELECT * FROM DBO.SplitString(ISNULL(@suppliers, ''),',')))
GROUP BY supler.Code
	,supler.[Desc]
	,prod.Number
	,PROD.[Desc]
	,prod.DepartmentId
	,ordr_hedr.OutletId
	,stre.[Desc]
	,ordr_hedr.InvoiceDate
ORDER BY supler.Code
		,supler.[Desc]
		,prod.Number
		,PROD.[Desc]
		,prod.DepartmentId
		,ordr_hedr.OutletId
		,stre.[Desc]
		,ordr_hedr.InvoiceDate

EXEC [dbo].[GetReportsFilterTables] @startDate,@endDate,@productStartId,@productEndId,@promoSales,@promoCode,@summary,@drillDown,@contineous,@variance,@wastage,@merge,@tillId,@storeIds,@zoneIds,@dayRange,@departmentIds,@commodityIds,@categoryIds,@groupIds,@suppliers,@manufacturerIds,null,@reportType

END
GO
