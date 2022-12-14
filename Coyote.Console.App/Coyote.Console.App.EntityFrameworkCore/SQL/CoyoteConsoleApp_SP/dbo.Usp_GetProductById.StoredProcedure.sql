/****** Object:  StoredProcedure [dbo].[Usp_GetProductById]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Usp_GetProductById] 
  @ProductId BIGINT --= 314569
 ,@SkipCount INT = NULL
 ,@MaxResultCount   INT = NULL
 AS
SET NOCOUNT ON
BEGIN

	DECLARE 
	 @HostCode NVARCHAR(MAX) =''
	,@Parent_ProductId BIGINT=null
	,@Parent_Number BIGINT=null
	,@Parent_CartonQty INT=null
	,@Parent_Desc NVARCHAR(50)=null
	,@AccessOutletIds NVARCHAR(MAX) =NULL
	
	SELECT * INTO #Warehouse FROM(
	Select ROW_NUMBER() OVER(ORDER BY ID ASC) AS RowIndex,* from Warehouse WHERE IsDeleted=0)X

	SELECT * INTO #Product_Temp FROM (
	SELECT 
	 prdct.Id
	,prdct.Number
	,prdct.[Desc]
	,prdct.[Status]
	,prdct.CommodityId
	,prdct_commdty.[Desc] Commodity
	,prdct.DepartmentId
	,prdct_dept.[Desc] Department
	,prdct.GroupId
	,prdct_grp.[Name] [Group]
	,prdct.CategoryId
	,prdct_categ.[Name] Category
	,prdct.SupplierId
	,suplir.[Desc] Supplier
	,NULL SupplierProductItem
	,NULL SupplierProductId
	,prdct.[Replicate]
	,prdct.TypeId
	,prdct_tp.[Name] [Type]
	,prdct.TaxId
	,prdct_tx.[Desc] Tax
	,prdct.ManufacturerId
	,prdct_Maufact.[Name] Manufacturer
	,prdct.UnitMeasureId
	,prdct_untMsr.[Name] UnitMeasure
	,prdct.CartonQty
	,prdct.CartonCost
	,prdct.TareWeight
	,prdct.UnitQty
	,prdct.Parent
	,CONVERT(float,NULL) ParentCartonQty
	,CONVERT(NVARCHAR(50),NULL) ParentDesc
	,prdct.PosDesc
	,prdct.NationalRangeId
	,prdct_natalrng.[Name] NationalRange
	,NULL AccessOutlets
	,prdct.VarietyInd
	,prdct.SlowMovingInd
	,prdct.ImagePath
	,NULL ImageUploadStatusCode
	,prdct.ScaleInd
	,prdct.GmFlagInd
	,prdct.WarehouseFrozenInd
	,prdct.StoreFrozenInd
	,prdct.AustMadeInd
	,prdct.AustOwnedInd
	,prdct.OrganicInd
	,prdct.HeartSmartInd
	,prdct.GenericInd
	,prdct.SeasonalInd
	,prdct.LabelQty
	,prdct.Freight
	,prdct.Size
	,prdct.Info
	,prdct.Litres
	,CONVERT(NVARCHAR(100),NULL) HostCode
	,CONVERT(NVARCHAR(100),NULL) HostCode2
	,CONVERT(NVARCHAR(100),NULL) HostCode3
	,CONVERT(NVARCHAR(100),NULL) HostCode4
	,CONVERT(NVARCHAR(100),NULL) HostCode5
	,prdct.HostNumber
	,prdct.HostNumber2
	,prdct.HostNumber3
	,prdct.HostNumber4
	,prdct.HostItemType
	,prdct.HostItemType2
	,prdct.HostItemType3
	,prdct.HostItemType4
	,prdct.HostItemType5
	,prdct.LastApnSold
	,prdct.Rrp
	,prdct.AltSupplier
	,prdct.DeletedAt
	,prdct.DeactivatedAt
	,prdct.CreatedAt
	,prdct.UpdatedAt
	,prdct.CreatedById
	,prdct.UpdatedById
	,prdct.AccessOutletIds
	FROM product prdct
	INNER JOIN Supplier suplir ON prdct.SupplierId=suplir.Id 
	INNER JOIN Department prdct_dept ON prdct.DepartmentId=prdct_dept.Id
	INNER JOIN Commodity prdct_commdty ON prdct.CommodityId=prdct_commdty.Id
	INNER JOIN MasterListItems prdct_grp ON prdct.GroupId=prdct_grp.Id
	INNER JOIN MasterListItems prdct_categ ON prdct.CategoryId=prdct_categ.Id
	INNER JOIN MasterListItems prdct_Maufact ON prdct.ManufacturerId=prdct_Maufact.Id
	INNER JOIN MasterListItems prdct_tp ON prdct.TypeId=prdct_tp.Id
	INNER JOIN MasterListItems prdct_natalrng ON prdct.NationalRangeId=prdct_natalrng.Id
	LEFT OUTER JOIN MasterListItems prdct_untMsr ON prdct.UnitMeasureId=prdct_untMsr.Id
	INNER JOIN Tax prdct_tx ON prdct.TaxId=prdct_tx.Id
	WHERE prdct.Id=@ProductId AND prdct.IsDeleted = 0 
	)X
	IF EXISTS(SELECT 1 FROM #Product_Temp WHERE Parent IS NOT NULL)
	BEGIN
		SELECT @Parent_ProductId=Parent FROM #Product_Temp WHERE Parent IS NOT NULL
		PRINT @Parent_ProductId
		IF @Parent_ProductId IS NOT NULL
		BEGIN
			SELECT @Parent_Number = Number
				   ,@Parent_CartonQty=CartonQty
				   ,@Parent_Desc =[Desc]
			FROM Product WHERE NUMBER=@Parent_ProductId AND IsDeleted=0
			UPDATE #Product_Temp SET Parent = @Parent_Number , ParentCartonQty =@Parent_CartonQty,ParentDesc=@Parent_Desc
		END
	END

	IF EXISTS(SELECT 1 FROM #Warehouse WHERE RowIndex =1)
	BEGIN
		SELECT @HostCode=Code FROM #Warehouse WHERE RowIndex =1
		UPDATE #Product_Temp SET HostCode =@HostCode
	END
	IF EXISTS(SELECT 1 FROM #Warehouse WHERE RowIndex =2)
	BEGIN
		SELECT @HostCode=Code FROM #Warehouse WHERE RowIndex =2
		UPDATE #Product_Temp SET HostCode2 =@HostCode
	END
	IF EXISTS(SELECT 1 FROM #Warehouse WHERE RowIndex =3)
	BEGIN
		SELECT @HostCode=Code FROM #Warehouse WHERE RowIndex =3
		UPDATE #Product_Temp SET HostCode3 = @HostCode
	END
	SELECT Id
	,Number
	,[Desc]
	,[Status]
	,CommodityId
	,Commodity
	,DepartmentId
	,Department
	,GroupId
	,[Group]
	,CategoryId
	,Category
	,SupplierId
	,Supplier
	,SupplierProductItem
	,SupplierProductId
	,[Replicate]
	,TypeId
	,[Type]
	,TaxId
	,Tax
	,ManufacturerId
	,Manufacturer
	,UnitMeasureId
	,UnitMeasure
	,CartonQty
	,CartonCost
	,TareWeight
	,UnitQty
	,Parent
	,ParentCartonQty
	,ParentDesc
	,PosDesc
	,NationalRangeId
	,NationalRange
	,AccessOutlets
	,VarietyInd
	,SlowMovingInd
	,ImagePath
	,ImageUploadStatusCode
	,ScaleInd
	,GmFlagInd
	,WarehouseFrozenInd
	,StoreFrozenInd
	,AustMadeInd
	,AustOwnedInd
	,OrganicInd
	,HeartSmartInd
	,GenericInd
	,SeasonalInd
	,LabelQty
	,Freight
	,Size
	,Info
	,Litres
	,HostCode
	,HostCode2
	,HostCode3
	,HostCode4
	,HostCode5
	,HostNumber
	,HostNumber2
	,HostNumber3
	,HostNumber4
	,HostItemType
	,HostItemType2
	,HostItemType3
	,HostItemType4
	,HostItemType5
	,LastApnSold
	,Rrp
	,AltSupplier
	,DeletedAt
	,DeactivatedAt
	,CreatedAt
	,UpdatedAt
	,CreatedById
	,UpdatedById
	,AccessOutletIds
	FROM #Product_Temp
	
	EXECUTE [dbo].[GetSupplierProductByProductId] @ProductId
 
	EXECUTE [dbo].[GetPromotionDetailsByProductId] @ProductId

	SELECT Number FROM APN WHERE ProductId=@ProductId
	SELECT Code FROM #Warehouse
	--IF EXISTS(SELECT 1 FROM #Product_Temp WHERE AccessOutletIds IS NOT NULL)
	BEGIN
		SELECT @AccessOutletIds=AccessOutletIds FROM #Product_Temp WHERE AccessOutletIds IS NOT NULL
		SELECT Item AccessOutlets FROM SplitString(@AccessOutletIds,',')
	END
	EXECUTE Usp_GetListItemByCode 'CATEGORY',@SkipCount,@MaxResultCount
	EXECUTE Usp_GetListItemByCode 'GROUP',@SkipCount,@MaxResultCount
	EXECUTE Usp_GetListItemByCode 'MANUFACTURER',@SkipCount,@MaxResultCount
	EXECUTE Usp_GetListItemByCode 'NATIONALRANGE',@SkipCount,@MaxResultCount
	EXECUTE Usp_GetListItemByCode 'PRODUCT_TYPE',@SkipCount,@MaxResultCount
	EXECUTE Usp_GetListItemByCode 'UNITMEASURE',@SkipCount,@MaxResultCount
	EXECUTE usp_GetActiveCommodities NULL,@SkipCount,@MaxResultCount,NULL,NULL
	EXECUTE usp_GetActiveDepartments NULL,@SkipCount,@MaxResultCount,NULL,NULL
	EXECUTE usp_Get_AllActiveSupplier NULL,@SkipCount,@MaxResultCount,NULL,NULL,0
	EXECUTE Usp_GetActiveTax NULL,@SkipCount,@MaxResultCount,NULL,NULL,0
	DROP TABLE #Product_Temp
	DROP TABLE #Warehouse
END
GO
