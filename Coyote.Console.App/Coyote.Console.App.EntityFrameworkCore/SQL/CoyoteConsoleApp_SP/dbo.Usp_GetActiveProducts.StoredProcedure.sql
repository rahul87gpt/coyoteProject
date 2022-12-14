/****** Object:  StoredProcedure [dbo].[Usp_GetActiveProducts]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Usp_GetActiveProducts] 
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
	DECLARE @SQLQuery NVARCHAR(MAX), @WhereClause NVARCHAR(MAX) = NULL
	SET @WhereClause=''

	
	IF @GlobalFilter is not NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND prod.[Desc] like ''%'+@GlobalFilter+'%'' '
	END
	IF @Id is not NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND prod.Id = '+ CAST(@Id AS VARCHAR)+'  '
	END
	IF @Status is not NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND prod.Status = ''%'+@Status+'%'' '  
	END
	IF @Description is not NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND prod.[Desc] like ''%'+@Description+'%'' '  
	END
	IF @Number is not NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND prod.Number prod.Number = '''+@Number+''   
	END
	IF @DeptId is not NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND prod.DepartmentId = '+CAST(@DeptId AS VARCHAR)+'  '
	END
	IF @Replicate is not NULL
	BEGIN
		SET @WhereClause = @WhereClause +' AND prod.Replicate = ''%'+@Replicate+'%'' '  
	END
	
	SET @SQLQuery ='SELECT 
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
		,CASE WHEN '+CAST(ISNULL(@SupplierId,0) AS VARCHAR)+' = 0 THEN NULL ELSE SuppQuery.SupplierItem END SupplierProductItem
		,CASE WHEN '+CAST(ISNULL(@SupplierId,0) AS VARCHAR)+' = 0 THEN 0 ELSE SuppQuery.Id END SupplierProductId
	FROM 
	Product prod
	INNER JOIN Tax tx ON prod.TaxId = tx.Id
	INNER JOIN Supplier supp ON prod.SupplierId =  supp.Id
	INNER JOIN Commodity comm ON prod.CommodityId =  comm.Id
	INNER JOIN Department dept ON prod.DepartmentId =  dept.Id
	INNER JOIN MasterListItems grp ON prod.GroupId =  grp.Id
	INNER JOIN MasterListItems prod_categ ON prod.CategoryId =  prod_categ.Id
	INNER JOIN MasterListItems prod_natl_rng ON prod.NationalRangeId =  prod_natl_rng.Id
	Left outer JOIN MasterListItems prod_unit_msr ON prod.UnitMeasureId =  prod_unit_msr.Id
	INNER JOIN MasterListItems prod_tp ON prod.TypeId =  prod_tp.Id
	INNER JOIN MasterListItems prod_manfact ON prod.ManufacturerId =  prod_manfact.Id
	LEFT OUTER JOIN 
	(
		SELECT * FROM SupplierProduct
		WHERE SupplierId='+CAST(ISNULL(@SupplierId,0) AS VARCHAR)+' 
	)SuppQuery ON SuppQuery.ProductId=prod.Id
    WHERE prod.IsDeleted = 0 '
	
	IF  @WhereClause IS NOT NULL
	BEGIN
		SET @SQLQuery = @SQLQuery + @WhereClause
	END
	
	IF @StoreId IS NOT NULL
	BEGIN
		SET @SQLQuery = @SQLQuery + ' AND prod.Id in (SELECT ProductId FROM OutletProduct WHERE StoreId = '+ CAST(@StoreId AS VARCHAR) +' and IsDeleted =0)'
	END 
    
	IF @SortColumn IS NULL
	BEGIN
		IF @SortDirection IS NULL
		BEGIN
			SET @SQLQuery = @SQLQuery + ' ORDER BY prod.UpdatedAt DESC'
		END
		ELSE
		BEGIN
			SET @SQLQuery = @SQLQuery + ' ORDER BY prod.UpdatedAt ASC'
		END
	END
	ELSE
	BEGIN
		IF @SortDirection IS NULL
		BEGIN
			SET @SQLQuery = @SQLQuery + ' ORDER BY prod.'+@SortColumn+' ASC'
		END
		ELSE
		BEGIN
			SET @SQLQuery = @SQLQuery + ' ORDER BY prod.'+@SortColumn+' DESC'
		END 
	END
	IF @MaxResultCount IS NOT NULL and @SkipCount IS NOT NULL
	BEGIN
		
		SET @SQLQuery = @SQLQuery + ' OFFSET '+ CAST(@SkipCount AS VARCHAR)+' ) ROWS
			FETCH NEXT '+ CAST(@MaxResultCount AS VARCHAR)+ ' ROWS ONLY'
    END
	EXECUTE(@SQLQuery)
END
GO
