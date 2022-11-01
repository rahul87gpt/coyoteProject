SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Narendra
-- =============================================
CREATE PROCEDURE [dbo].[GetSalesAndStockTrxSheet] 
@startDate datetime,
@endDate datetime,
@productStartId bigint = null,
@productEndId bigint = null,
@promoSales bit=0,
@promoCode nvarchar(max) = null,
@tillId bigint = null,
@storeIds nvarchar(max) = null,
@dayRange nvarchar(max) = null,
@departmentIds nvarchar(max) = null,
@commodityIds nvarchar(max) = null,
@categoryIds nvarchar(max) = null,
@groupIds nvarchar(max) = null,
@suppliers nvarchar(max) = null,
@manufacturerIds nvarchar(max) = null,
@reportType nvarchar(200)=null

AS
BEGIN
SET NOCOUNT ON;
IF(lower(@reportType)='itemsale')
BEGIN
	SELECT t.[Date]
		  ,t.[Type]
		  ,t.[Day]
		  ,p.[Number] AS [ProductNumber]
		  ,p.[Desc] AS [ProductDesc]
		  ,o.[Code] AS [OutletCode]
		  ,o.[Desc] AS [OutletDesc]
		  ,tl.[Code] AS [TillCode]
		  ,tl.[Desc] AS [TillDesc]
		  ,t.[Qty] AS [SaleQty]
		  ,t.[Amt] AS [SaleAmt]
		  ,t.[Cost] AS [SaleCost]
		  ,t.[Discount]
		  ,CASE 
			WHEN t.[Amt] = 0
			  THEN 0
			ELSE ((t.[Amt] - t.[Cost]) * 100) / t.[Amt]
			END AS [GP_Pcnt]
		  ,'' as [SubType]
		  ,t.[Weekend]
		  ,t.[ExGSTAmt]
		  ,t.[ExGSTCost]
		  ,pr.[Code] AS PromotionCode
		  ,pr.[Desc] AS PromotionDesc
		  ,t.[PromoSales]
		  ,t.[PromoSalesGST]
		  ,s.[Code] AS SupplierCode
		  ,s.[Desc] AS SupplierDesc
		  ,c.[Code] AS CommodityCode
		  ,c.[Desc] AS CommodityDesc
		  ,d.[Code] AS DepartmentCode
		  ,d.[Desc] AS DepartmentDesc
		  ,ca.[Code] AS CategoryCode
		  ,ca.[Name] AS CategoryDesc
		  ,g.[Code] AS GroupCode
		  ,g.[Name] AS GroupDesc
		  ,t.[Member]
		  ,t.[ManualInd] AS [Manual]
		  ,t.[UnitQty] AS SellUnitQty
		  ,t.[StockMovement]
		  ,t.[Parent]
		  ,t.[CartonQty] 
	  FROM [dbo].[Transaction] t 
		   INNER JOIN [dbo].[Product] p ON p.Id = t.ProductId 
		   INNER JOIN [dbo].[Store] o ON o.Id = t.OutletId 
		   INNER JOIN [dbo].[Till] tl ON tl.Id = t.TillId
		   INNER JOIN [dbo].[Department] d ON d.Id = t.DepartmentId
		   INNER JOIN [dbo].[Commodity] c ON c.Id = t.CommodityId
		   INNER JOIN [dbo].[Supplier] s ON s.Id = t.SupplierId 
		   INNER JOIN [dbo].[MasterListItems] g ON g.Id = t.[Group] 
		   INNER JOIN [dbo].[MasterListItems] ca ON ca.Id = t.[CategoryId] 
		   LEFT JOIN [dbo].[Promotion] pr ON pr.[Id] = t.[PromoSellId]
	  WHERE (t.[Date] between @startDate and @endDate)
			and (@tillId is null or t.TillId = @tillId) 
			and ((@productStartId is null or t.ProductId>=@productStartId) and (@productEndId is null or t.ProductId<=@productEndId)) 
			and (@promoSales=0 or (@promoSales=1 and t.PromoSales!=0) and (@promoCode is null or pr.[Code]=@promoCode))
			and (@dayRange is null or t.[Day] in (select * from [dbo].[SplitString](isnull(@dayRange,''),',')))  
			and (@storeIds is null or t.OutletId in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			and (@commodityIds is null or t.CommodityId in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			and (@departmentIds is null or t.DepartmentId in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			and (@categoryIds is null or t.CategoryId in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			and (@groupIds is null or t.[Group] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			and (@suppliers is null or t.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			and (@manufacturerIds is null or t.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			and t.[Type]='ITEMSALE'
END
ELSE IF(lower(@reportType)='stock')
BEGIN
	SELECT t.[Date]
		  ,t.[Type]
		  ,t.[Day]
		  ,p.[Number] AS [ProductNumber]
		  ,p.[Desc] AS [ProductDesc]
		  ,o.[Code] AS [OutletCode]
		  ,o.[Desc] AS [OutletDesc]
		  ,tl.[Code] AS [TillCode]
		  ,tl.[Desc] AS [TillDesc]
		  ,t.[Qty] AS [SaleQty]
		  ,t.[Amt] AS [SaleAmt]
		  ,t.[Cost] AS [SaleCost]
		  ,t.[Discount]
		  ,CASE 
			WHEN t.[Amt] = 0
			  THEN 0
			ELSE ((t.[Amt] - t.[Cost]) * 100) / t.[Amt]
			END AS [GP_Pcnt]
		  ,'' as [SubType]
		  ,t.[Weekend]
		  ,t.[ExGSTAmt]
		  ,t.[ExGSTCost]
		  ,pr.[Code] AS PromotionCode
		  ,pr.[Desc] AS PromotionDesc
		  ,t.[PromoSales]
		  ,t.[PromoSalesGST]
		  ,s.[Code] AS SupplierCode
		  ,s.[Desc] AS SupplierDesc
		  ,c.[Code] AS CommodityCode
		  ,c.[Desc] AS CommodityDesc
		  ,d.[Code] AS DepartmentCode
		  ,d.[Desc] AS DepartmentDesc
		  ,ca.[Code] AS CategoryCode
		  ,ca.[Name] AS CategoryDesc
		  ,g.[Code] AS GroupCode
		  ,g.[Name] AS GroupDesc
		  ,t.[Member]
		  ,t.[ManualInd] AS [Manual]
		  ,t.[UnitQty] AS SellUnitQty
		  ,t.[StockMovement]
		  ,t.[Parent]
		  ,t.[CartonQty] 
	  FROM [dbo].[Transaction] t 
		   INNER JOIN [dbo].[Product] p ON p.Id = t.ProductId 
		   INNER JOIN [dbo].[Store] o ON o.Id = t.OutletId 
		   INNER JOIN [dbo].[Till] tl ON tl.Id = t.TillId
		   INNER JOIN [dbo].[Department] d ON d.Id = t.DepartmentId
		   INNER JOIN [dbo].[Commodity] c ON c.Id = t.CommodityId
		   INNER JOIN [dbo].[Supplier] s ON s.Id = t.SupplierId 
		   INNER JOIN [dbo].[MasterListItems] g ON g.Id = t.[Group] 
		   INNER JOIN [dbo].[MasterListItems] ca ON ca.Id = t.[CategoryId] 
		   LEFT JOIN [dbo].[Promotion] pr ON pr.[Id] = t.[PromoSellId]
	  WHERE (t.[Date] between @startDate and @endDate)
			and (@tillId is null or t.TillId = @tillId) 
			and ((@productStartId is null or t.ProductId>=@productStartId) and (@productEndId is null or t.ProductId<=@productEndId)) 
			and (@dayRange is null or t.[Day] in (select * from [dbo].[SplitString](isnull(@dayRange,''),',')))  
			and (@storeIds is null or t.OutletId in (select * from [dbo].[SplitString](isnull(@storeIds,''),','))) 
			and (@commodityIds is null or t.CommodityId in (select * from [dbo].[SplitString](isnull(@commodityIds,''),','))) 
			and (@departmentIds is null or t.DepartmentId in (select * from [dbo].[SplitString](isnull(@departmentIds,''),','))) 
			and (@categoryIds is null or t.CategoryId in (select * from [dbo].[SplitString](isnull(@categoryIds,''),','))) 
			and (@groupIds is null or t.[Group] in (select * from [dbo].[SplitString](isnull(@groupIds,''),','))) 
			and (@suppliers is null or t.[SupplierId] in (select * from [dbo].[SplitString](isnull(@suppliers,''),','))) 
			and (@manufacturerIds is null or t.[ManufacturerId] in (select * from [dbo].[SplitString](isnull(@manufacturerIds,''),','))) 
			and t.[Type]!='ITEMSALE'
END
END
GO