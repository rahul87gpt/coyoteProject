/****** Object:  StoredProcedure [dbo].[Usp_GetAllActiveProducts]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Usp_GetAllActiveProducts]
 @GlobalFilter NVARCHAR(100) = NULL
,@Id INT = NULL
,@Status NVARCHAR(100)= NULL
,@Description NVARCHAR(100)= NULL
,@Number NVARCHAR(100)= NULL
,@DeptId INT = NULL
,@Replicate NVARCHAR(100)= NULL
,@StoreId int = NULL
,@SortColumn NVARCHAR(50) = NULL
,@SortDirection NVARCHAR(50) = NULL
,@SupplierId INT = NULL
,@SkipCount INT = NULL
,@MaxResultCount   INT = NULL
AS
BEGIN
	DECLARE @SQLQuery NVARCHAR(MAX), @TotalRecordCount INT = 0
	
	IF OBJECT_ID('tempdb..#ProductTable') IS NOT NULL
		DROP TABLE #ProductTable
	IF OBJECT_ID('tempdb..#ProductTable_Result') IS NOT NULL
    	DROP TABLE #ProductTable_Result
	CREATE TABLE #ProductTable_Result(
	[Id] [bigint] NOT NULL,
	[Number] [bigint] NOT NULL,
	[Desc] [nvarchar](50) NOT NULL,
	[PosDesc] [nvarchar](20) NULL,
	[Status] [bit] NOT NULL,
	[CartonQty] [int] NOT NULL,
	[UnitQty] [real] NOT NULL,
	[CartonCost] [real] NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[SupplierId] [int] NOT NULL,
	[CommodityId] [int] NOT NULL,
	[TaxId] [bigint] NOT NULL,
	[GroupId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[ManufacturerId] [int] NOT NULL,
	[TypeId] [int] NOT NULL,
	[NationalRangeId] [int] NOT NULL,
	[UnitMeasureId] [int] NULL,
	[ScaleInd] [bit] NULL,
	[GmFlagInd] [bit] NULL,
	[SlowMovingInd] [bit] NULL,
	[WarehouseFrozenInd] [bit] NULL,
	[StoreFrozenInd] [bit] NULL,
	[AustMadeInd] [bit] NULL,
	[AustOwnedInd] [bit] NULL,
	[OrganicInd] [bit] NULL,
	[HeartSmartInd] [bit] NULL,
	[GenericInd] [bit] NULL,
	[SeasonalInd] [bit] NULL,
	[Parent] [bigint] NULL,
	[LabelQty] [real] NULL,
	[Replicate] [nvarchar](15) NULL,
	[Freight] [nvarchar](15) NULL,
	[Size] [nvarchar](8) NULL,
	[Litres] [real] NULL,
	[VarietyInd] [bit] NULL,
	[HostNumber] [nvarchar](15) NULL,
	[HostNumber2] [nvarchar](15) NULL,
	[HostNumber3] [nvarchar](15) NULL,
	[HostNumber4] [nvarchar](15) NULL,
	[HostItemType] [nvarchar](1) NULL,
	[HostItemType2] [nvarchar](1) NULL,
	[HostItemType3] [nvarchar](1) NULL,
	[HostItemType4] [nvarchar](1) NULL,
	[HostItemType5] [nvarchar](1) NULL,
	[LastApnSold] [bigint] NULL,
	[Rrp] [real] NULL,
	[AltSupplier] [bit] NULL,
	[DeletedAt] [datetime] NULL,
	[DeactivatedAt] [datetime] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
	[CreatedById] [int] NOT NULL,
	[UpdatedById] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[ImagePath] [nvarchar](max) NULL,
	[AccessOutletIds] [nvarchar](max) NULL,
	[StoreId] [int] NULL,
	[TareWeight] [real] NULL,
	[Info] [nvarchar](50) NULL,
	[Tax] NVARCHAR(15) NOT NULL,
	[Tax_Desc] NVARCHAR(30) NOT NULL,
	[Supplier] NVARCHAR(30) NOT NULL,
	[Supplier_Desc] NVARCHAR(80) NOT NULL,
	[Commodity] NVARCHAR(30) NOT NULL,
	[Commodity_Desc] NVARCHAR(80) NOT NULL,
	[Department] NVARCHAR(30) NOT NULL,
	[Department_Desc] NVARCHAR(80) NOT NULL,
	[[Group] NVARCHAR(50) NOT NULL,
	[Group_Name] NVARCHAR(100) NOT NULL,
	[Category] NVARCHAR(50) NOT NULL,
	[Category_Name] NVARCHAR(100) NOT NULL,
	[NationalRange] NVARCHAR(50) NOT NULL,
	[NationalRange_Name] NVARCHAR(100) NOT NULL,
	[UnitMeasure] NVARCHAR(50)  NULL,
	[UnitMeasure_Name] NVARCHAR(100)  NULL,
	[[Type] NVARCHAR(50) NOT NULL,
	[Type_Name] NVARCHAR(100) NOT NULL,
	[Manufacturer] NVARCHAR(50) NOT NULL,
	[Manufacturer_Name] NVARCHAR(100) NOT NULL,
	[SupplierProductItem] NVARCHAR(30) NULL,
	[SupplierProductId] INT DEFAULT 0
)

	SELECT * INTO #ProductTable FROM ( 
		SELECT  
			 prod.Id
			,prod.Number
			,prod.[Desc]
			,prod.PosDesc
			,prod.Status
			,prod.CartonQty
			,prod.UnitQty
			,prod.CartonCost
			,prod.DepartmentId
			,prod.SupplierId
			,prod.CommodityId
			,prod.TaxId
			,prod.GroupId
			,prod.CategoryId
			,prod.ManufacturerId
			,prod.TypeId
			,prod.NationalRangeId
			,prod.UnitMeasureId
			,prod.ScaleInd
			,prod.GmFlagInd
			,prod.SlowMovingInd
			,prod.WarehouseFrozenInd
			,prod.StoreFrozenInd
			,prod.AustMadeInd
			,prod.AustOwnedInd
			,prod.OrganicInd
			,prod.HeartSmartInd
			,prod.GenericInd
			,prod.SeasonalInd
			,prod.Parent
			,prod.LabelQty
			,prod.Replicate
			,prod.Freight
			,prod.Size
			,prod.Litres
			,prod.VarietyInd
			,prod.HostNumber
			,prod.HostNumber2
			,prod.HostNumber3
			,prod.HostNumber4
			,prod.HostItemType
			,prod.HostItemType2
			,prod.HostItemType3
			,prod.HostItemType4
			,prod.HostItemType5
			,prod.LastApnSold
			,prod.Rrp
			,prod.AltSupplier
			,prod.DeletedAt
			,prod.DeactivatedAt
			,prod.CreatedAt
			,prod.UpdatedAt
			,prod.CreatedById
			,prod.UpdatedById
			,prod.IsDeleted
			,prod.ImagePath
			,prod.AccessOutletIds
			,prod.StoreId
			,prod.TareWeight
			,prod.Info
			,tx.Code Tax
			,tx.[Desc] Tax_Desc
			,supp.Code Supplier
			,supp.[Desc] Supplier_Desc
			,comm.Code Commodity
			,comm.[Desc] Commodity_Desc
			,dept.Code Department
			,dept.[Desc] Department_Desc
			,grp.Code [Group]
			,grp.Name Group_Name
			,prod_categ.Code Category
			,prod_categ.Name Category_Name
			,prod_natl_rng.Code NationalRange
			,prod_natl_rng.Name NationalRange_Name
			,prod_unit_msr.Code UnitMeasure
			,prod_unit_msr.Name UnitMeasure_Name
			,prod_tp.Code [Type]
			,prod_tp.Name Type_Name
			,prod_manfact.Code Manufacturer
			,prod_manfact.Name Manufacturer_Name
			,CASE WHEN ISNULL(@SupplierId,0) = 0 THEN NULL ELSE SuppQuery.SupplierItem END SupplierProductItem
			,CASE WHEN ISNULL(@SupplierId,0) = 0 THEN 0 ELSE SuppQuery.Id END SupplierProductId
		FROM Product prod with(READUNCOMMITTED)
		INNER JOIN Tax tx  with(READUNCOMMITTED) ON prod.TaxId = tx.Id
		INNER JOIN Supplier supp  with(READUNCOMMITTED) ON prod.SupplierId =  supp.Id
		INNER JOIN Commodity comm  with(READUNCOMMITTED) ON prod.CommodityId =  comm.Id
		INNER JOIN Department dept  with(READUNCOMMITTED) ON prod.DepartmentId =  dept.Id
		INNER JOIN MasterListItems grp  with(READUNCOMMITTED) ON prod.GroupId =  grp.Id
		INNER JOIN MasterListItems prod_categ  with(READUNCOMMITTED) ON prod.CategoryId =  prod_categ.Id
		INNER JOIN MasterListItems prod_natl_rng with(READUNCOMMITTED) ON prod.NationalRangeId =  prod_natl_rng.Id
		LEFT OUTER JOIN MasterListItems prod_unit_msr  with(READUNCOMMITTED) ON prod.UnitMeasureId =  prod_unit_msr.Id
		INNER JOIN MasterListItems prod_tp  with(READUNCOMMITTED) ON prod.TypeId =  prod_tp.Id
		INNER JOIN MasterListItems prod_manfact  with(READUNCOMMITTED) ON prod.ManufacturerId =  prod_manfact.Id
		LEFT OUTER JOIN 
		(
			SELECT * FROM SupplierProduct  with(READUNCOMMITTED)
			WHERE SupplierId=ISNULL(@SupplierId,0)
		)SuppQuery ON SuppQuery.ProductId=prod.Id
		WHERE prod.IsDeleted = 0 AND (@GlobalFilter IS NULL OR prod.[Desc] like '%'+@GlobalFilter+'%')
		AND( @Id IS NULL OR prod.Id = @Id )
		AND ( @Status IS NULL OR prod.Status = @Status )
		AND ( @Description IS NULL OR prod.[Desc] like '%'+@GlobalFilter+'%')
		AND ( @Number IS NULL OR  prod.Number = @Number )
		AND ( @DeptId IS NULL OR  prod.DepartmentId = @DeptId )
		AND ( @Replicate IS NULL OR prod.Replicate = @Replicate )
		AND ( @StoreId IS NULL OR (prod.Id in (SELECT ProductId FROM OutletProduct WHERE StoreId = @StoreId and IsDeleted = 0)))
	) x 
	SET @TotalRecordCount = @@ROWCOUNT
	IF @MaxResultCount IS NOT NULL
	BEGIN
		INSERT INTO  #ProductTable_Result
		SELECT * FROM #ProductTable ORDER BY UpdatedAt DESC
		OFFSET (@SkipCount) ROWS FETCH NEXT ISNULL(@MaxResultCount,0) ROWS ONLY
	END
	ELSE
	BEGIN
		INSERT INTO #ProductTable_Result SELECT * FROM #ProductTable
	END 
	IF @SortColumn IS NULL
	BEGIN
		SELECT * FROM #ProductTable_Result ORDER BY UpdatedAt DESC 
	END
	ELSE
	BEGIN
		IF @SortDirection IS NULL
			BEGIN
				SET @SQLQuery = ''
				SET @SQLQuery = @SQLQuery + ' SELECT * FROM #ProductTable_Result ORDER BY '+@SortColumn+' ASC'
				EXECUTE(@SQLQuery)
			END
		ELSE
			BEGIN
				SET @SQLQuery = ''
				SET @SQLQuery = @SQLQuery + ' SELECT * FROM #ProductTable_Result ORDER BY '+@SortColumn+' DESC'
				EXECUTE(@SQLQuery)
			END 
	END
	SELECT @TotalRecordCount TotalRecordCount
END
GO
