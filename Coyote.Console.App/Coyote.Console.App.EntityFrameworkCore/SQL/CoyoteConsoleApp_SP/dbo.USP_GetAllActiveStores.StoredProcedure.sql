/****** Object:  StoredProcedure [dbo].[USP_GetAllActiveStores]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_GetAllActiveStores] 
  @GlobalFilter NVARCHAR(100) = NULL
 ,@Id NVARCHAR(MAX) = NULL--'7,12,95'
 ,@PageNumber INT = 1
 ,@PageSize   INT = 1000
 ,@SortColumn NVARCHAR(50) = NULL
 ,@SortDirection NVARCHAR(50) = NULL
AS
BEGIN
	DECLARE @TotalRecordCount INT =0
	IF @PageNumber = 0
	BEGIN
		SET @PageNumber = 1
	END
	IF OBJECT_ID('tempdb..#StoreTable') IS NOT NULL
			DROP TABLE #StoreTable
	SELECT * INTO #StoreTable FROM (
	SELECT 
	str.Id
	,str.Code
	,str.GroupId
	,str.Address1
	,str.Address2
	,str.Address3
	,str.PhoneNumber
	,str.Fax
	,str.PostCode
	,str.Status
	,str.[Desc]
	,str.Email
	,str.PriceZone
	,str.SellingInd
	,str.StockInd
	,str.DelName
	,str.DelAddr1
	,str.DelAddr2
	,str.DelAddr3
	,str.DelPostCode
	,str.CostType
	,str.Abn
	,str.BudgetGrowthFact
	,str.EntityNumber
	,str.CostZone
	,str.WarehouseId
	,str.OutletPriceFromOutlet
	,str.PriceFromLevel
	,str.Latitude
	,str.Longitude
	,str.CreatedAt
	,str.UpdatedAt
	,str.OpenHours
	,str.FuelSite
	,str.NameOnApp
	,str.AddressOnApp
	,str.DisplayOnApp
	,str.AppOrders
	,str.LabelTypeNormal
	,str.LabelTypePromo
	,str.LabelTypeShort
	,str.CreatedById
	,str.UpdatedById
	,str.IsDeleted
	,str_grp.code GroupCode
	,str_grp.Name GroupName
	FROM [Store] str with(READUNCOMMITTED)
	INNER JOIN StoreGroup str_grp with(READUNCOMMITTED) ON str.GroupId = str_grp.Id
	WHERE str.IsDeleted = 0 AND (@GlobalFilter IS NULL OR (str.Code like '%'+@GlobalFilter+'%' OR str.[Desc] like '%'+@GlobalFilter+'%'))
	AND (@Id IS NULL OR CAST(str.Id AS VARCHAR) in (SELECT Item FROM dbo.SplitString(@Id, ',') ))
	)X ORDER BY UpdatedAt DESC
	SET @TotalRecordCount = @@ROWCOUNT
	
	SELECT * FROM #StoreTable 
	ORDER BY UpdatedAt DESC
	OFFSET @PageSize * (@PageNumber - 1) ROWS FETCH NEXT @PageSize ROWS ONLY
	
	SELECT @TotalRecordCount TotalRecordCount

	DROP TABLE #StoreTable
END
GO
