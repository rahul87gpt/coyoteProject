/****** Object:  StoredProcedure [dbo].[Usp_GetAllActiveOutletProducts]    Script Date: 21-08-2020 22:33:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Usp_GetAllActiveOutletProducts] 
 @GlobalFilter NVARCHAR(100) = NULL
,@ProductId INT = NULL
,@Id INT = NULL
,@StoreId int = NULL
,@StoreIds  NVARCHAR(MAX) = NULL
,@SortColumn NVARCHAR(50) = NULL
,@SortDirection NVARCHAR(50) = NULL
,@SkipCount INT = NULL
,@MaxResultCount   INT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	IF OBJECT_ID('tempdb..#OutletProduct') IS NOT NULL
			DROP TABLE #OutletProduct
	IF OBJECT_ID('tempdb..#OutletProduct_Result') IS NOT NULL
			DROP TABLE #OutletProduct_Result
			
	DECLARE @TotalRecordCount INT = 0 , @Total_OutletCount INT = 0 ,@Row_OutletCounter INT = 0 
	CREATE TABLE #OutletProduct_Result(
		[RowIndex] [bigint] IDENTITY(1,1) NOT NULL,
		[Id] [bigint]  NOT NULL,
		[StoreId] [int] NOT NULL,
		[ProductId] [bigint] NOT NULL,
		[SupplierId] [int] NULL,
		[Status] [bit] NOT NULL,
		[Till] [bit] NULL,
		[OpenPrice] [bit] NULL,
		[NormalPrice1] [real] NOT NULL,
		[NormalPrice2] [real] NULL,
		[NormalPrice3] [real] NULL,
		[NormalPrice4] [real] NULL,
		[NormalPrice5] [real] NULL,
		[CartonCost] [real] NOT NULL,
		[CartonCostHost] [real] NULL,
		[CartonCostInv] [real] NULL,
		[CartonCostAvg] [real] NULL,
		[SellPromoCode] [nvarchar](max) NULL,
		[BuyPromoCode] [nvarchar](max) NULL,
		[PromoPrice1] [real] NULL,
		[PromoPrice2] [real] NULL,
		[PromoPrice3] [real] NULL,
		[PromoPrice4] [real] NULL,
		[PromoPrice5] [real] NULL,
		[PromoCartonCost] [real] NULL,
		[QtyOnHand] [real] NOT NULL,
		[MinOnHand] [real] NOT NULL,
		[MaxOnHand] [real] NULL,
		[MinReorderQty] [real] NULL,
		[PickingBinNo] [int] NULL,
		[ChangeLabelInd] [bit] NULL,
		[ChangeTillInd] [bit] NULL,
		[HoldNorm] [nvarchar](max) NULL,
		[ChangeLabelPrinted] [datetime] NULL,
		[LabelQty] [real] NULL,
		[ShortLabelInd] [bit] NULL,
		[SkipReorder] [bit] NOT NULL,
		[SpecPrice] [real] NULL,
		[SpecCode] [nvarchar](max) NULL,
		[SpecFrom] [datetime] NULL,
		[SpecTo] [datetime] NULL,
		[GenCode] [nvarchar](max) NULL,
		[SpecCartonCost] [real] NULL,
		[ScalePlu] [real] NULL,
		[FifoStock] [bit] NULL,
		[Mrp] [real] NULL,
		[CreatedAt] [datetime] NOT NULL,
		[UpdatedAt] [datetime] NOT NULL,
		[ProductNumber] [bigint] NOT NULL,
		[ProductDesc] NVARCHAR(50) NOT NULL,
		[UnitQty] [real] NOT NULL,
		SupplierCode [nvarchar](30) NULL,
		SupplierDesc [nvarchar](80) NULL,
		StoreCode  [nvarchar](30) NULL,
		StoreDesc [nvarchar](30) NULL,
		Offer1PromoCode NVARCHAR(50),
		Offer2PromoCode NVARCHAR(50),
		Offer3PromoCode NVARCHAR(50),
		Offer4PromoCode NVARCHAR(50),
		MixMatch1PromoCode NVARCHAR(50),
		MixMatch2PromoCode NVARCHAR(50),
		MemberOfferCode NVARCHAR(50),
		ItemCost REAL NOT NULL,
		GP REAL  
	)
	SELECT * INTO #OutletProduct FROM 
	(
		SELECT 
			 out_prd.Id
			,out_prd.StoreId
			,out_prd.ProductId
			,out_prd.SupplierId
			,out_prd.Status
			,out_prd.Till
			,out_prd.OpenPrice
			,out_prd.NormalPrice1
			,out_prd.NormalPrice2
			,out_prd.NormalPrice3
			,out_prd.NormalPrice4
			,out_prd.NormalPrice5
			,out_prd.CartonCost
			,out_prd.CartonCostHost
			,out_prd.CartonCostInv
			,out_prd.CartonCostAvg
			,out_prd.SellPromoCode
			,out_prd.BuyPromoCode
			,out_prd.PromoPrice1
			,out_prd.PromoPrice2
			,out_prd.PromoPrice3
			,out_prd.PromoPrice4
			,out_prd.PromoPrice5
			,out_prd.PromoCartonCost
			,out_prd.QtyOnHand
			,out_prd.MinOnHand
			,out_prd.MaxOnHand
			,out_prd.MinReorderQty
			,out_prd.PickingBinNo
			,out_prd.ChangeLabelInd
			,out_prd.ChangeTillInd
			,out_prd.HoldNorm
			,out_prd.ChangeLabelPrinted
			,out_prd.LabelQty
			,out_prd.ShortLabelInd
			,out_prd.SkipReorder
			,out_prd.SpecPrice
			,out_prd.SpecCode
			,out_prd.SpecFrom
			,out_prd.SpecTo
			,out_prd.GenCode
			,out_prd.SpecCartonCost
			,out_prd.ScalePlu
			,out_prd.FifoStock
			,out_prd.Mrp
			,out_prd.CreatedAt
			,out_prd.UpdatedAt
			,prd.Number ProductNumber
			,prd.[Desc] ProductDesc
			,prd.UnitQty
			,supp.Code SupplierCode
			,supp.[Desc] SupplierDesc
			,stoe.Code StoreCode
			,stoe.Code StoreDesc
			,BuyPromoCode Offer1PromoCode
			,BuyPromoCode Offer2PromoCode
			,BuyPromoCode Offer3PromoCode
			,BuyPromoCode Offer4PromoCode
			,BuyPromoCode MixMatch1PromoCode
			,BuyPromoCode MixMatch2PromoCode
			,BuyPromoCode MemberOfferCode
			,ISNULL((out_prd.CartonCost / NULLIF(prd.CartonQty,0)),0) ItemCost  
			,((out_prd.NormalPrice1 - ((out_prd.CartonCost / NULLIF(prd.CartonQty,0)) * prd.UnitQty)) * 100) / NULLIF(out_prd.NormalPrice1, 0) as GP  
			FROM OutletProduct out_prd  with(READUNCOMMITTED)
			INNER JOIN Product prd with(READUNCOMMITTED) ON out_prd.ProductId=prd.Id 
			INNER JOIN Store stoe  with(READUNCOMMITTED) ON out_prd.StoreId=stoe.Id 
			Left Outer JOIN Supplier supp  with(READUNCOMMITTED) ON out_prd.SupplierId=supp.Id
		WHERE out_prd.IsDeleted = 0 
		AND ( @Id IS NULL OR out_prd.Id=@Id )
		AND ( @StoreId IS NULL OR out_prd.StoreId=@StoreId )
		AND ( @ProductId IS NULL OR out_prd.ProductId=@ProductId )
		AND ( @StoreIds IS NULL OR CAST(out_prd.StoreId AS VARCHAR) in (SELECT Item FROM dbo.SplitString(@StoreIds, ',') ))
	)x
	SET @TotalRecordCount = @@ROWCOUNT
	IF @MaxResultCount IS NOT NULL
	BEGIN
	
			INSERT INTO  #OutletProduct_Result
			SELECT * FROM #OutletProduct 
			ORDER BY UpdatedAt DESC
			OFFSET (@SkipCount) ROWS FETCH NEXT ISNULL(@MaxResultCount,0) ROWS ONLY
			SET @Total_OutletCount = @@ROWCOUNT
    END
	ELSE
	BEGIN
			INSERT INTO  #OutletProduct_Result
			SELECT * FROM #OutletProduct 
			ORDER BY UpdatedAt DESC
			SET @Total_OutletCount = @@ROWCOUNT
	END
	
	SET @Row_OutletCounter = 1
	DECLARE @Prod_Id INT = 0  ,@PromoOfferProductCount INT = 0, @PromoOfferProductCounter INT = 0, @PromoOfferProductCode NVARCHAR(50) = NULL
	,@PromoOfferProduct_ColumnName VARCHAR(50), @SQLQuery NVARCHAR(MAX),@PromoMixmatchCount INT = 0, @PromoMixmatchCounter INT = 0,@SortingExpression NVARCHAR(100)
	,@PromoMixmatch_ColumnName VARCHAR(50), @PromoMixmatchCode NVARCHAR(50) = NULL,@MemberOfferCode  NVARCHAR(50)= NULL,@ItemCost REAL,@UnitQty REAL = NULL
	WHILE @Total_OutletCount >= @Row_OutletCounter 
	BEGIN
		
		SELECT @Prod_Id = ProductId,@ItemCost=ItemCost,@UnitQty=UnitQty FROM #OutletProduct_Result WHERE RowIndex = @Row_OutletCounter
		
		SELECT TOP 1 @MemberOfferCode = Code from Promotion WHERE ID in (Select PromotionId from PromoMemberOffer with(READUNCOMMITTED) WHERE PRODUCTId=@Prod_Id) ORDER BY CreatedAt desc
		IF @UnitQty IS NOT NULL and @UnitQty > 0
		BEGIN
			IF @ItemCost > 0
			BEGIN
				UPDATE  #OutletProduct_Result SET ItemCost = ItemCost * UnitQty  WHERE ProductId = @Prod_Id
			END 
		END 
		UPDATE  #OutletProduct_Result SET MemberOfferCode = @MemberOfferCode  WHERE ProductId = @Prod_Id
		
		IF OBJECT_ID('tempdb..#PromoOfferProduct') IS NOT NULL
			DROP TABLE #PromoOfferProduct
		
		SELECT ROW_NUMBER() OVER (ORDER BY CreatedAt DESC) RowIndex,* INTO #PromoOfferProduct FROM (
		SELECT TOP 4 *  FROM Promotion WHERE ID IN (SELECT PromotionId FROM PromoOffer WHERE ID IN (SELECT PromotionOfferId FROM PromoOfferProduct WHERE PRODUCTId=@Prod_Id) )   ORDER BY CreatedAt desc)x
		SET @PromoOfferProductCount = @@ROWCOUNT
		
		SET @PromoOfferProductCounter = 1 
		WHILE @PromoOfferProductCount >= @PromoOfferProductCounter
		BEGIN
			SELECT @PromoOfferProductCode=Code FROM  #PromoOfferProduct WHERE RowIndex = @PromoOfferProductCounter
			
			SET @PromoOfferProduct_ColumnName= 'Offer'+CAST(@PromoOfferProductCounter AS VARCHAR)+'PromoCode'
			SET @SQLQuery='UPDATE  #OutletProduct_Result SET '+@PromoOfferProduct_ColumnName+' = '''+@PromoOfferProductCode+''' WHERE ProductId = '+CAST(@Prod_Id AS VARCHAR)+''
		
			EXECUTE( @SQLQuery)
		
			SET @PromoOfferProductCounter = @PromoOfferProductCounter + 1 
			SET @PromoOfferProduct_ColumnName = NULL
			SET @SQLQuery=NULL
		END
		IF OBJECT_ID('tempdb..#PromoMixmatch') IS NOT NULL
			DROP TABLE #PromoMixmatch
		
		SELECT ROW_NUMBER() OVER (ORDER BY CreatedAt DESC) RowIndex,* INTO #PromoMixmatch FROM (
		SELECT TOP 2 * FROM Promotion WHERE ID IN (SELECT PromotionId FROM PromoMixmatch WHERE ID IN (Select PromotionMixmatchId FROM PromoMixmatchProduct WHERE PRODUCTId=@Prod_Id) )    ORDER BY CreatedAt desc)x
		SET @PromoMixmatchCount =@@ROWCOUNT
		SET @PromoMixmatchCounter = 1 
		
		WHILE @PromoMixmatchCount >= @PromoMixmatchCounter
		BEGIN
			SELECT @PromoMixmatchCode = Code FROM  #PromoMixmatch WHERE RowIndex = @PromoMixmatchCounter
		
			SET @PromoMixmatch_ColumnName= 'MixMatch'+CAST(@PromoMixmatchCounter AS VARCHAR)+'PromoCode'
			SET @SQLQuery='UPDATE  #OutletProduct_Result SET '+@PromoMixmatch_ColumnName+' = '''+@PromoMixmatchCode+''' WHERE ProductId = '+CAST(@Prod_Id AS VARCHAR)+''
		
			EXECUTE( @SQLQuery)
		
			SET @PromoMixmatchCounter = @PromoMixmatchCounter + 1 
			SET @PromoMixmatch_ColumnName = NULL
			SET @SQLQuery=NULL
		END

		SET @Row_OutletCounter = @Row_OutletCounter + 1
		SET @Prod_Id = 0
		SET @PromoOfferProductCount=0
		SET @PromoOfferProductCode=NULL
		SET @MemberOfferCode=NULL
	END 
	SET @SortingExpression=''
	IF @SortColumn IS NULL
	BEGIN
		IF @SortDirection IS NULL
		BEGIN
			SET @SortingExpression = @SortingExpression + ' ORDER BY UpdatedAt DESC' 
		END
		ELSE
		BEGIN
			SET @SortingExpression = @SortingExpression + ' ORDER BY UpdatedAt ASC'
		END
	END
	ELSE
	BEGIN
		IF @SortDirection IS NULL
			BEGIN
				SET @SortingExpression = @SortingExpression + ' ORDER BY '+@SortColumn+' ASC'
			END
		ELSE
			BEGIN
				SET @SortingExpression = @SortingExpression + ' ORDER BY '+@SortColumn+' DESC'
			END 
	END	SET @SQLQuery=''	SET @SQLQuery = @SQLQuery + ' 	SELECT  Id		,StoreId		,ProductId		,SupplierId		,[Status]		,Till		,OpenPrice		,NormalPrice1		,NormalPrice2		,NormalPrice3		,NormalPrice4		,NormalPrice5		,CartonCost		,CartonCostHost		,CartonCostInv		,CartonCostAvg		,SellPromoCode		,BuyPromoCode		,PromoPrice1		,PromoPrice2		,PromoPrice3		,PromoPrice4		,PromoPrice5		,PromoCartonCost		,QtyOnHand		,MinOnHand		,MaxOnHand		,MinReorderQty		,PickingBinNo		,ChangeLabelInd		,ChangeTillInd		,HoldNorm		,ChangeLabelPrinted		,LabelQty		,ShortLabelInd		,SkipReorder		,SpecPrice		,SpecCode		,SpecFrom		,SpecTo		,GenCode		,SpecCartonCost		,ScalePlu		,FifoStock		,Mrp		,CreatedAt		,UpdatedAt		,ProductNumber		,ProductDesc		,SupplierCode		,SupplierDesc		,StoreCode		,StoreDesc		,Offer1PromoCode		,Offer2PromoCode		,Offer3PromoCode		,Offer4PromoCode		,MixMatch1PromoCode		,MixMatch2PromoCode		,MemberOfferCode		,ItemCost 
		,GP
	FROM #OutletProduct_Result '+@SortingExpression+''
	
	EXECUTE (@SQLQuery )
	SELECT @TotalRecordCount RecordCount
	DROP TABLE #OutletProduct
	DROP TABLE #OutletProduct_Result
END
GO