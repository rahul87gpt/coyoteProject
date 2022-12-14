USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP__CreateOptimalOrder]    Script Date: 9/5/2020 1:39:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CDN_SP__CreateOptimalOrder] (
	 @aOrderNum INT OUT
	,@aProduct INT
	,@aProdParent INT
	,@PROD_ALT_SUPPLIER NVARCHAR(3)
	,@aDailySales FLOAT
	,@aQtyOnHand FLOAT
	,@aMinOnHand FLOAT
	,@aCartonCost FLOAT
	,@aCartonQty FLOAT
	,@aMinReorderQty FLOAT
	,@aExcludeOrderNo INT
	,@aRunDate DATETIME
	,@aDoRecalc BIT
	,@aMaxOnHand FLOAT
	,@aSalesTotal FLOAT
	,@aProdList NVARCHAR(500)
	,@aPromoUnits FLOAT
	,@aPromoDailySales FLOAT
	,@PromoTotalSales FLOAT
	,@aSalePromoCode VARCHAR(500)
	,@aStrSalePromoEndDate VARCHAR(500)
	,@aIsANewProd BIT
	,@aSalesEndDate DATETIME				
	,@OSS_Id INT 							
	,@OSS_Outlet INT 		
	,@OSS_DOWGenerateOrder INT				
	,@OSS_MultipleOrdersInAWeek BIT		
	,@OSS_Supplier VARCHAR(30) 				
	,@OSS_CoverDays INT,					
	 @OSS_InvoiceOrderOffset INT			
	,@OSS_ReceiveOrderOffset INT
	,@OSS_DiscountThresholdThree FLOAT
	,@OSS_CoverDaysDiscountThreshold3 INT
	,@OSS_DiscountThresholdTwo FLOAT
	,@OSS_CoverDaysDiscountThreshold2 INT
	,@OSS_DiscountThresholdOne FLOAT
	,@OSS_CoverDaysDiscountThreshold1 INT
	,@OSS_OrderNonDefaultSupplier BIT
	,@PROD_DEPARTMENT INT
	,@CheckPromoBuy BIT = 0
	,@NonPromoDailySales FLOAT = 0
	,@aPromoDailySalesOriginal FLOAT = 0
	,@CreateOptimalOrder_Result BIT = 0 OUT
	,@Debug INT = 0
	)
AS
BEGIN TRY
	DECLARE @Result BIT = 0
	DECLARE @PromoDisc FLOAT = 0
	DECLARE @UnitsOnOrder FLOAT
	DECLARE @CoverUnits FLOAT
	DECLARE @StandardOrderQty FLOAT
	DECLARE @InvestmentOrderQty FLOAT
	DECLARE @OrderQty FLOAT = 0
	DECLARE @PromoEndDate DATETIME
	DECLARE @TestPromoEndDate DATETIME
	DECLARE @PromCtnCost FLOAT
	DECLARE @InvBuy BIT
	DECLARE @UnitsAvail FLOAT
	DECLARE @BuyPromoCode NVARCHAR(100)
	DECLARE @CommodityCoverDays INT
	DECLARE @CoverDaysUsed INT
	DECLARE @DailySales FLOAT
	DECLARE @PromotionOrderQty FLOAT
	DECLARE @PromoBuy BIT = 0
	DECLARE @NonPromoBuy BIT = 0
	DECLARE @NormalCoverDays INT = 0
	DECLARE @MaxPerishableProdCoverDays INT
	DECLARE @PerishableProduct BIT = 0

				
	SET @MaxPerishableProdCoverDays = 10
	SET @PerishableProduct = 0

	DECLARE @ProdDept INT = @PROD_DEPARTMENT

	IF (@ProdDept > 0) AND @ProdDept IN(2,4,24,25) 
	BEGIN
		SET @PerishableProduct = 1
	END
	
	SET @Result = 1

	SET @InvestmentOrderQty = 0.0
	SET @PromCtnCost = 0.0;
	SET @DailySales = 0

	DECLARE @promoDailySalesUsed BIT

	SET @promoDailySalesUsed = 0

	DECLARE @CheaperResult NVARCHAR(30) = NULL
	
	
	IF @DEBUG IN (1,2)
	SELECT '[BEFORE]', @PerishableProduct PerishableProduct, @aPromoUnits aPromoUnits

	IF @PerishableProduct = 1
	BEGIN
		SET @aPromoUnits = Round(@aPromoDailySales * 2, 0)
	END
	ELSE
	BEGIN
		SET @aPromoUnits = Round(@aPromoDailySales * 3, 0)
	END

	IF @DEBUG IN (1,2)
	SELECT '[AFTER]', @PerishableProduct PerishableProduct, @aPromoUnits aPromoUnits
	

	--IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]',@aProduct aProduct, @aPromoUnits,@aPromoDailySales ,'SET @aPromoUnits = Round(@aPromoDailySales * 3, 0)'
	--Newly Added Start
	DECLARE @aOrderCreationType INT = 3	,@ORDH_DOCUMENT_TYPE NVARCHAR(10)='ORDER',@ORDH_DOCUMENT_STATUS NVARCHAR(10)='NEW'
	DECLARE @orderNo INT = (
			SELECT TOP 1 ORDH_ORDER_NO
			FROM ORDHTBL
			WHERE ORDH_SUPPLIER =  @OSS_Supplier
				AND [ORDH_OUTLET] = @OSS_Outlet
				AND CONVERT(VARCHAR(8),ORDH_ORDER_DATE,112) = CONVERT(VARCHAR(8),@aRunDate,112)
				AND ORDH_Creatation_Type = 3
				AND ORDH_DOCUMENT_TYPE = 'ORDER'
				AND ORDH_DOCUMENT_STATUS = 'NEW'
			)

	IF @orderNo IS NOT NULL
		SET @aOrderNum = @orderNo

	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]',@aOrderNum ,'SET @aOrderNum = @orderNo'
	IF @aOrderNum = -1
	BEGIN
		--GetNextUnUsedOrderNo  Start
		--DECLARE @aNewOrderNum INT = (				
		--SELECT ORDH_ORDER_NO					
		--FROM [dbo].ORDHTBL WITH (NOLOCK)
		--FROM [dbo].ORDHTBL WITH (NOLOCK)
		--WHERE ORDH_OUTLET = @OSS_Outlet AND ORDH_SUPPLIER = @OSS_Supplier AND CONVERT(VARCHAR(8),ORDH_ORDER_DATE,112) = CONVERT(VARCHAR(8),@aRunDate,112)
		--)
		DECLARE @aNewOrderNum INT
		Select @aNewOrderNum=min(RowNumber) from  (
		SELECT 
		ORDH_ORDER_NO, 
		ROW_NUMBER() OVER (ORDER BY ORDH_ORDER_NO) AS RowNumber    
		FROM ORDHTBL where ORDH_OUTLET = @OSS_Outlet
		) Data  
		where ORDH_ORDER_NO <> RowNumber
		and not exists ( select 1 from ORDHTBL where ORDH_ORDER_NO = RowNumber and ORDH_OUTLET = @OSS_Outlet ); 

		IF(ISNULL(@aNewOrderNum,0)<=0)
			BEGIN
			SELECT @aNewOrderNum =MAX(ORDH_ORDER_NO)				
			FROM [dbo].ORDHTBL WITH (NOLOCK)
			--FROM [dbo].ORDHTBL WITH (NOLOCK)
			WHERE ORDH_OUTLET = @OSS_Outlet --AND ORDH_SUPPLIER = @OSS_Supplier --AND CONVERT(VARCHAR(8),ORDH_ORDER_DATE,112) = CONVERT(VARCHAR(8),@aRunDate,112)

			--IF(ISNULL(@aNewOrderNum,0)<=0)
			--	SELECT @aNewOrderNum =MAX(ORDH_ORDER_NO)				
			--	FROM [dbo].ORDHTBL WITH (NOLOCK)
			--	--FROM [dbo].ORDHTBL WITH (NOLOCK)
			--	WHERE ORDH_OUTLET = @OSS_Outlet --AND ORDH_SUPPLIER = @OSS_Supplier AND CONVERT(VARCHAR(8),ORDH_ORDER_DATE,112) = CONVERT(VARCHAR(8),@aRunDate,112)

		
			SET @aOrderNum = ISNULL(@aNewOrderNum,0)+1
			

			IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','SET @aOrderNum = ISNULL(@aNewOrderNum,0)+1', @aOrderNum aOrderNum,@aNewOrderNum aNewOrderNum
				
		END
		ELSE
			SET @aOrderNum = @aNewOrderNum


		DECLARE @existInSupplier VARCHAR(30) --VARCHAR(80) It is 80 in CODETBL  but ORDHTBL only accepts 30 nvarchar 

		SELECT @existInSupplier = SUBSTRING(CODE_DESC,0,30)
		FROM [dbo].CODETBL
		WHERE CODE_KEY_TYPE = 'SUPPLIER'
			AND CODE_KEY_NUM = 0
			AND CODE_KEY_ALP = @OSS_Supplier
					

		IF @Debug =1 SELECT 
			@aOrderNum						ORDH_ORDER_NO
			,@OSS_Outlet					ORDH_OUTLET
			,@ORDH_DOCUMENT_TYPE			ORDH_DOCUMENT_TYPE
			,@ORDH_DOCUMENT_STATUS			ORDH_DOCUMENT_STATUS
			,CAST(@aRunDate AS DATE)		ORDH_ORDER_DATE
			,GETDATE()						ORDH_TIMESTAMP
			,@OSS_Supplier					ORDH_SUPPLIER
			,@existInSupplier				ORDH_SUPPLIER_NAME
			,'AOO'							ORDH_REFERENCE
			,@aOrderCreationType			ORDH_Creatation_Type

		INSERT INTO ORDHTBL (
				ORDH_ORDER_NO
			,ORDH_OUTLET
			,ORDH_DOCUMENT_TYPE
			,ORDH_DOCUMENT_STATUS
			,ORDH_ORDER_DATE
			,ORDH_TIMESTAMP
			,ORDH_SUPPLIER
			,ORDH_SUPPLIER_NAME
			,ORDH_REFERENCE
			,ORDH_Creatation_Type
			)
		VALUES (
			@aOrderNum
			,@OSS_Outlet
			,@ORDH_DOCUMENT_TYPE
			,@ORDH_DOCUMENT_STATUS
			,CAST(@aRunDate AS DATE)
			,GETDATE()
			,@OSS_Supplier
			,@existInSupplier
			,'AOO'
			,@aOrderCreationType
			)
				
		--GetNextUnUsedOrderNo  END
	END
	--Newly Added END
	
	
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','SET @aPromoUnits = @aMinOnHand', @aPromoUnits aPromoUnits,@aMinOnHand aMinOnHand ,@ProdDept ProdDept ,@aOrderNum aOrderNum
	
	IF @aPromoUnits < @aMinOnHand
		SET @aPromoUnits = @aMinOnHand
		
	DECLARE @aCommodityCoverDays1 INT
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]  @aPromoUnits < @aMinOnHand','SET @aPromoUnits = @aMinOnHand', @aPromoUnits aPromoUnits,@aMinOnHand aMinOnHand ,@ProdDept ProdDept ,@aOrderNum aOrderNum
	--// get promo and work out disc %
	--//Remove Investment buy entirely on Milk
	IF (@ProdDept <> 24)
	BEGIN
		--GetBuyPromoDiscount  {start
		DECLARE @OrderDate DATETIME

		SET @OrderDate = DATEADD(DD, @OSS_InvoiceOrderOffset, @aRunDate);

		DECLARE @MinPrmCtnCost FLOAT
		DECLARE @aPromEndDate DATETIME
		DECLARE @aPromoCode VARCHAR(500)

		SELECT TOP 1 @MinPrmCtnCost = Min(PRMP_CARTON_COST)
			,@aPromEndDate = PRMH_END
			,@aPromoCode = PRMP_PROM_CODE
		FROM [dbo].PRMPTBL WITH (NOLOCK)
		JOIN [dbo].PRMHTBL WITH (NOLOCK) ON PRMH_PROM_CODE = PRMP_PROM_CODE
		JOIN [dbo].PRODTBL WITH (NOLOCK) ON Prod_Number = PRMP_PRODUCT
		WHERE PRMH_PROMOTION_TYPE = 'BUYING'
			AND PRMH_START <= @OrderDate
			AND PRMH_END >= @OrderDate
			AND PRMH_STATUS = 'Active'
			AND PRMP_CARTON_COST > 0
			AND PRMH_OUTLET_ZONE IN (
				SELECT CODE_KEY_ALP
				FROM CodeTbl
				WHERE Code_Key_Type = 'ZONEOUTLET'
					AND CODE_KEY_NUM = @OSS_Outlet
				)
			AND PRMP_SUPPLIER = @OSS_Supplier
			AND (
				PRMP_PRODUCT = CAST(@aProduct AS VARCHAR)
				OR PRMP_PRODUCT = CAST(@aProdParent AS VARCHAR)
				)
		GROUP BY PRMH_END
			,PRMP_PROM_CODE
		ORDER BY 1,2 desc

		IF @DEBUG =1
		SELECT '[CDN_SP__AddToOrder]  PRMP_CARTON_COST,PRMH_END,PRMP_PROM_CODE', PRMP_CARTON_COST CARTON_COST,PRMH_END enddate,PRMP_PROM_CODE,*
		FROM [dbo].PRMPTBL WITH (NOLOCK)
		JOIN [dbo].PRMHTBL WITH (NOLOCK) ON PRMH_PROM_CODE = PRMP_PROM_CODE
		JOIN [dbo].PRODTBL WITH (NOLOCK) ON Prod_Number = PRMP_PRODUCT
		WHERE PRMH_PROMOTION_TYPE = 'BUYING'
			AND PRMH_START <= @OrderDate
			AND PRMH_END >= @OrderDate
			AND PRMH_STATUS = 'Active'
			AND PRMP_CARTON_COST > 0
			AND PRMH_OUTLET_ZONE IN (
				SELECT CODE_KEY_ALP
				FROM CodeTbl
				WHERE Code_Key_Type = 'ZONEOUTLET'
					AND CODE_KEY_NUM = @OSS_Outlet
				)
			AND PRMP_SUPPLIER = @OSS_Supplier
			AND (
				PRMP_PRODUCT = CAST(@aProduct AS VARCHAR)
				OR PRMP_PRODUCT = CAST(@aProdParent AS VARCHAR)
				)		
		ORDER BY PRMP_CARTON_COST,PRMH_END desc


		IF (@aCartonCost > @MinPrmCtnCost)
			AND (@MinPrmCtnCost > 0.0)
		BEGIN
			SET @PromoDisc = ((@aCartonCost - @MinPrmCtnCost) / @aCartonCost) * 100;
			SET @PromoDisc = ROUND(@PromoDisc,2);
			SET @PromCtnCost = @MinPrmCtnCost;
			SET @PromoEndDate = @aPromEndDate
		END
		ELSE
		BEGIN
			SET @aPromoCode = NULL
			SET @MinPrmCtnCost = NULL
			SET @aPromEndDate = NULL
		END
				--GetBuyPromoDiscount  {END
	END;
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','------{ Start GetCommodityCoverDays @aProduct ', @PromoDisc PromoDisc,@PromCtnCost PromCtnCost,@aPromoCode aPromoCode
	BEGIN ------{ Start GetCommodityCoverDays @aProduct

		SELECT @aCommodityCoverDays1 = CODE_NUM_2
		FROM [dbo].ProdTbl WITH (NOLOCK)
		JOIN [dbo].CodeTbl WITH (NOLOCK) ON CODE_KEY_TYPE = 'Commodity'
			AND PROD_COMMODITY = CODE_KEY_NUM
			AND Prod_Number = @aProduct

		SET @CommodityCoverDays = ISNULL(@aCommodityCoverDays1, -1)
	END ------} End   GetCommodityCoverDays 	
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]',@CommodityCoverDays CommodityCoverDays,@OSS_MultipleOrdersInAWeek OSS_MultipleOrdersInAWeek,@PromCtnCost PromCtnCost
	--// if it's a new prod then only use the sales range for actual sales
	IF @OSS_MultipleOrdersInAWeek = 1
	BEGIN
		IF @CommodityCoverDays > 0
		BEGIN
			SET @CoverDaysUsed = CASE 
					WHEN @OSS_CoverDays < @CommodityCoverDays
						THEN @OSS_CoverDays
					ELSE @CommodityCoverDays
					END;
		END
		ELSE
		BEGIN
			SET @CoverDaysUsed = @OSS_CoverDays
		END
	END
	ELSE
	BEGIN
		IF @CommodityCoverDays > 0
		BEGIN
			SET @CoverDaysUsed = CASE 
					WHEN @OSS_InvoiceOrderOffset + 7 < @CommodityCoverDays
						THEN @OSS_InvoiceOrderOffset + 7
					ELSE @CommodityCoverDays
					END;
		END
		ELSE
		BEGIN
			SET @CoverDaysUsed = @OSS_InvoiceOrderOffset + 7;
		END
	END
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','SET @CoverDaysUsed = @OSS_InvoiceOrderOffset + 7;',@PromCtnCost PromCtnCost, @CoverDaysUsed CoverDaysUsed,@OSS_OrderNonDefaultSupplier OSS_OrderNonDefaultSupplier
	--// For perishable product, cover days shoud not be greater than 10 days  
	IF (
			(@ProdDept = 2)
			OR (@ProdDept = 4)
			OR (@ProdDept = 24)
			OR (@ProdDept = 25)
			)
		AND (@CoverDaysUsed > @MaxPerishableProdCoverDays)
	BEGIN
		SET @CoverDaysUsed = @MaxPerishableProdCoverDays;
	END;

	SET @NormalCoverDays = @CoverDaysUsed
	SET @CoverUnits = (@NonPromoDailySales * @CoverDaysUsed)

	IF (@aMinOnHand < 0 OR @aMinOnHand IS NULL)
		SET @aMinOnHand = 0

	IF (@aPromoUnits < 0 OR @aPromoUnits IS NULL)
		SET @aPromoUnits = 0

	--// add Min on hand to top up
	IF (@aQtyOnHand < 0 OR @aQtyOnHand IS NULL)
		SET @aQtyOnHand = 0

	--  // NEW
	DECLARE @CoverDays INT = 0

	---CAN BE OPTIMIZE BY REMOVE GET COVER DAYS
	BEGIN ------{ Start GetCoverDays 
		DECLARE @aCommodityCoverDays INT = - 1
		BEGIN ------{ Start GetCommodityCoverDays @OUTP_PRODUCT
			SELECT @aCommodityCoverDays = CODE_NUM_2
			FROM [dbo].ProdTbl WITH (NOLOCK)
			JOIN [dbo].CodeTbl WITH (NOLOCK) ON CODE_KEY_TYPE = 'Commodity'
				AND PROD_COMMODITY = CODE_KEY_NUM
				AND Prod_Number = @aProduct

			SET @aCommodityCoverDays = ISNULL(@aCommodityCoverDays, - 1)
		END ------} End   GetCommodityCoverDays 			

		IF @OSS_MultipleOrdersInAWeek = 1
		BEGIN
			IF @aCommodityCoverDays > 0
			BEGIN
				SELECT @CoverDays = CASE 
						WHEN (@OSS_CoverDays < @aCommodityCoverDays)
							THEN @OSS_CoverDays
						ELSE @aCommodityCoverDays
						END
			END
			ELSE
			BEGIN
				SET @CoverDays = @OSS_CoverDays;
			END;
		END
		ELSE
		BEGIN
			IF @aCommodityCoverDays > 0
			BEGIN
				SELECT @CoverDays = CASE 
						WHEN ((@OSS_InvoiceOrderOffset + 7) < @aCommodityCoverDays)
							THEN (@OSS_InvoiceOrderOffset + 7)
						ELSE @aCommodityCoverDays
						END
			END
			ELSE
			BEGIN
				SET @CoverDays = @OSS_InvoiceOrderOffset + 7;
			END;
		END;
				--SELECT @SalesTotal SalesTotal, @DailySales DailySales, @IsANewProduct IsANewProduct,@aCommodityCoverDays aCommodityCoverDays,@CoverDays CoverDays   -- comment it		
	END ------} End    GetCoverDays 
	
	SET @TestPromoEndDate = DATEADD(dd, @CoverDays, @aRunDate)
	SET @OSS_CoverDaysDiscountThreshold1 = ISNULL(@OSS_CoverDaysDiscountThreshold1,0)
	SET @OSS_CoverDaysDiscountThreshold2 =ISNULL(@OSS_CoverDaysDiscountThreshold2,0)
	SET @OSS_CoverDaysDiscountThreshold3 =ISNULL(@OSS_CoverDaysDiscountThreshold3,0)
	DECLARE @@INVESTMENTBUY BIT = 0
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','(@CommodityCoverDays = 0 OR  @CommodityCoverDays >= @OSS_CoverDaysDiscountThreshold1)',@PromCtnCost PromCtnCost,@PromoDisc PromoDisc, @CommodityCoverDays CommodityCoverDays,@OSS_CoverDaysDiscountThreshold1 OSS_CoverDaysDiscountThreshold1,@TestPromoEndDate TestPromoEndDate, @PromoEndDate PromoEndDate
	
	IF ((@TestPromoEndDate >= @PromoEndDate) AND ISNULL(@PromCtnCost,0) > 0 AND (@CommodityCoverDays <= 0 OR  @CommodityCoverDays >= @OSS_CoverDaysDiscountThreshold1))
	BEGIN
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','INVESTMENT BUYYYY AND (@PromoEndDate > @aRunDate AND (@CommodityCoverDays = 0 OR  @CommodityCoverDays >= @OSS_CoverDaysDiscountThreshold1))', @TestPromoEndDate,@PromoEndDate
		--// do invest buy amount maybe!!
		DECLARE @@OSS_DiscountThreshold FLOAT = NULL
		DECLARE @@OSS_CoverDaysDiscountThreshold FLOAT 

		IF @PromoDisc >= @OSS_DiscountThresholdThree
		BEGIN
			SET @@OSS_DiscountThreshold = @OSS_DiscountThresholdThree
			SET @@OSS_CoverDaysDiscountThreshold = @OSS_CoverDaysDiscountThreshold3
		END
		ELSE IF (@PromoDisc >= @OSS_DiscountThresholdTwo)
		BEGIN
			SET @@OSS_DiscountThreshold = @OSS_DiscountThresholdThree
			SET @@OSS_CoverDaysDiscountThreshold = @OSS_CoverDaysDiscountThreshold2
		END
		ELSE IF (@PromoDisc >= @OSS_DiscountThresholdOne) AND (@PromoDisc < @OSS_DiscountThresholdTwo)
		BEGIN
			SET @@OSS_DiscountThreshold = @OSS_DiscountThresholdOne
			SET @@OSS_CoverDaysDiscountThreshold = @OSS_CoverDaysDiscountThreshold1
		END

		IF @@OSS_DiscountThreshold IS NOT NULL
		BEGIN
			IF @CommodityCoverDays > 0
				SET @CoverDaysUsed = CASE WHEN @@OSS_CoverDaysDiscountThreshold < @CommodityCoverDays THEN @@OSS_CoverDaysDiscountThreshold ELSE @CommodityCoverDays END
			ELSE
				SET @CoverDaysUsed = @@OSS_CoverDaysDiscountThreshold;

			--// For perishable product, cover days shoud not be greater than 10 days
			DECLARE @COST FLOAT =  @PromCtnCost
			

			IF (@OSS_OrderNonDefaultSupplier = 1) AND (@PROD_ALT_SUPPLIER = 'No') EXEC [dbo].CDN_SP_GetCheaperSupplier @aProduct,@aProdParent,@OSS_Outlet,@OSS_Supplier,@aRunDate,@COST OUT,@CoverDaysUsed OUT,@CheaperResult OUT, @DEBUG,@OSS_Id,@OSS_DOWGenerateOrder

			IF  @CheaperResult IS NOT NULL AND @CoverDaysUsed = 0
			BEGIN
				IF @DEBUG = 1 SELECT 'RETURN [CDN_SP__AddToOrder]','CDN_SP_GetCheaperSupplier '+CAST(@MaxPerishableProdCoverDays as VARCHAR(3))+' CHEAPER today'
				RETURN
			END
			DECLARE @@PRESISHABLE BIT = 0
			IF (@ProdDept > 0) AND @ProdDept IN(2,4,24,25) -- Perishable with less than cover days should not be INVESTMENT BUY
			BEGIN
				IF((@CoverDaysUsed > @MaxPerishableProdCoverDays) )
					SET @CoverDaysUsed = @MaxPerishableProdCoverDays;
				SET @@PRESISHABLE = 1
			END

			IF @DEBUG = 1 SELECT 'FOUND  [CDN_SP__AddToOrder]','BUY Investment '+CAST(@CoverDaysUsed as VARCHAR(3))+' CHEAPER today',@CoverUnits CoverUnits,@InvestmentOrderQty InvestmentOrderQty,@@PRESISHABLE ,@CheaperResult 

			IF @CheaperResult IS NULL AND @@PRESISHABLE = 0
			BEGIN
				SET @@INVESTMENTBUY = 1
				SET @CoverUnits = (@NormalCoverDays * @aPromoDailySales) + (@CoverDaysUsed - @NormalCoverDays) * @NonPromoDailySales;
				SET @InvestmentOrderQty = @CoverUnits + @aMinOnHand --+ @aPromoUnits;
				IF @DEBUG = 1 SELECT 'FOUND  [CDN_SP__AddToOrder]','BUY Investment '+CAST(@CoverDaysUsed as VARCHAR(3))+' CHEAPER today',@CoverUnits CoverUnits,@InvestmentOrderQty InvestmentOrderQty
			END
		END
	END
	ELSE
	BEGIN
		IF @DEBUG = 1 SELECT 'INV FOUND BUT THRESHOLD IS NULL', @CommodityCoverDays CommodityCoverDays,@OSS_CoverDaysDiscountThreshold1 OSS_CoverDaysDiscountThreshold1,@PromCtnCost PromCtnCost
		--SET @PromCtnCost = 0.0;
	END
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','ELSE NOT INV BUY (@CommodityCoverDays = 0 OR  @CommodityCoverDays >= @OSS_CoverDaysDiscountThreshold1)',@PromCtnCost PromCtnCost, @CommodityCoverDays CommodityCoverDays,@OSS_CoverDaysDiscountThreshold1 OSS_CoverDaysDiscountThreshold1,@CoverDaysUsed CoverDaysUsed

	IF @DEBUG IN (1,2) SELECT @PROD_ALT_SUPPLIER PROD_ALT_SUPPLIER
 	IF (@@INVESTMENTBUY = 1) --inv by
	BEGIN
		SET @OrderQty = @InvestmentOrderQty;
		SET @InvBuy = 1;
		SET @PromoBuy = 0
		SET @NonPromoBuy = 0
		SET @BuyPromoCode = @aPromoCode
		IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','INVESTMENT BUY',@PromCtnCost PromCtnCost
	END
	ELSE IF (@CheckPromoBuy = 1) --promo by
	BEGIN
		SET @COST = CASE WHEN @PromCtnCost > 0 THEN @PromCtnCost ELSE @aCartonCost END
		IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','ELSE IF (@CheckPromoBuy = 1)',@CheckPromoBuy checkPromoBuy,@PromCtnCost PromCtnCost
		IF (@OSS_OrderNonDefaultSupplier = 1) AND (@PROD_ALT_SUPPLIER = 'No') EXEC CDN_SP_GetCheaperSupplier @aProduct,@aProdParent,@OSS_Outlet,@OSS_Supplier,@aRunDate,@COST OUT,@CoverDaysUsed OUT,@CheaperResult OUT, @DEBUG,@OSS_Id,@OSS_DOWGenerateOrder

		IF @CheaperResult IS NOT NULL AND @CoverDaysUsed = 0
		BEGIN
			IF @DEBUG = 1 SELECT 'RETURN [CDN_SP__AddToOrder]','CDN_SP_GetCheaperSupplier CHEAPER in promobuy today',@CheaperResult CheaperResult
			RETURN
		END

		DECLARE @PromoCoverUnits FLOAT = @aPromoDailySales * @CoverDaysUsed

		SET @PromotionOrderQty = @PromoCoverUnits + @aPromoUnits
		SET @OrderQty = @PromotionOrderQty;
		SET @InvBuy = 0
		SET @PromoBuy = 1
		SET @NonPromoBuy = 0
		SET @BuyPromoCode = @aPromoCode-- Either cheaper find or perishable but still having buy promo available if not found buy promo it will automatically  NULL
		IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','PROMO BUY',@PromCtnCost PromCtnCost
	END
	ELSE
	BEGIN
		SET @COST = CASE WHEN @PromCtnCost > 0 THEN @PromCtnCost ELSE @aCartonCost END
		IF (@OSS_OrderNonDefaultSupplier = 1) AND (@PROD_ALT_SUPPLIER = 'No') EXEC CDN_SP_GetCheaperSupplier @aProduct,@aProdParent,@OSS_Outlet,@OSS_Supplier,@aRunDate,@COST OUT,@CoverDaysUsed OUT,@CheaperResult OUT, @DEBUG,@OSS_Id,@OSS_DOWGenerateOrder

		IF @CheaperResult IS NOT NULL AND @CoverDaysUsed = 0
			BEGIN
				IF @DEBUG = 1 SELECT 'RETURN [CDN_SP__AddToOrder]','CDN_SP_GetCheaperSupplier CHEAPER normal buy today'
				RETURN
			END

		SET @StandardOrderQty = ((@NonPromoDailySales * @CoverDaysUsed) + @aMinOnHand);-- //Either Remove aQtyOnHand from here or from 
		SET @OrderQty = @StandardOrderQty;
		SET @InvBuy = 0
		SET @PromoBuy = 0
		SET @NonPromoBuy = 1
		SET @BuyPromoCode = @aPromoCode-- Either cheaper find or perishable but still having buy promo available if not found buy promo it will automatically  NULL
		IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','@StandardOrderQty = ((@NonPromoDailySales * @CoverDaysUsed) + @aMinOnHand)',@StandardOrderQty StandardOrderQty,@NonPromoDailySales NonPromoDailySales, @CoverDaysUsed CoverDaysUsed, @aMinOnHand MinOnHand
		IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','NORMAL BUY'
	END
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','@PromCtnCost > 0.0) --inv by',@CheckPromoBuy CheckPromoBuy,@PromCtnCost PromCtnCost,@InvBuy InvBuy,@PromoBuy PromoBuy,@NonPromoBuy NonPromoBuy,@CheaperResult CheaperResult,@COST CHEAPER_COST,@PromCtnCost PromCtnCost,@BuyPromoCode BuyPromoCode

	--// get units on order.
	-----------------------------------GetUnitsOnOrder   START	
	IF @aExcludeOrderNo = - 1
	BEGIN
		WITH TheData (UNITS_ON_ORDER)
		AS (
			SELECT CASE coalesce(PROD_PARENT, 0)
					WHEN 0
						THEN SUM(ORDL_TOTAL_UNITS)
					ELSE SUM(ORDL_TOTAL_UNITS) * PROD_UNIT_QTY
					END AS UNITS_ON_ORDER
			FROM [dbo].ORDLTBL WITH (NOLOCK)
			JOIN [dbo].PRODTBL WITH (NOLOCK) ON Prod_number = ORDL_PRODUCT
			WHERE (ORDL_OUTLET = @OSS_Outlet)
				AND (
					ORDL_PRODUCT IN (
						CAST(@aProduct AS VARCHAR), CAST(@aProdParent AS VARCHAR)
						)
					)
				AND (ORDL_DOCUMENT_TYPE = 'ORDER')
				AND (ORDL_DOCUMENT_STATUS = 'ORDER')
			GROUP BY PROD_PARENT
				,PROD_UNIT_QTY
			)
		SELECT @UnitsOnOrder = sum(UNITS_ON_ORDER)
		FROM TheData
	END
	ELSE
	BEGIN
		WITH TheData (UNITS_ON_ORDER)
		AS (
			SELECT CASE coalesce(PROD_PARENT, 0)
					WHEN 0
						THEN SUM(ORDL_TOTAL_UNITS)
					ELSE SUM(ORDL_TOTAL_UNITS) * PROD_UNIT_QTY
					END AS UNITS_ON_ORDER
			FROM [dbo].ORDLTBL WITH (NOLOCK)
			JOIN [dbo].PRODTBL WITH (NOLOCK) ON Prod_number = ORDL_PRODUCT
			WHERE (ORDL_OUTLET = @OSS_Outlet)
				AND (
					ORDL_PRODUCT IN (
						CAST(@aProduct AS VARCHAR),
						CAST(@aProdParent AS VARCHAR)
						)
					)
				AND (ORDL_DOCUMENT_TYPE = 'ORDER')
				AND (ORDL_DOCUMENT_STATUS = 'ORDER')
				AND (ORDL_ORDER_NO <> @aExcludeOrderNo)
			GROUP BY PROD_PARENT
				,PROD_UNIT_QTY
			)
		SELECT @UnitsOnOrder = sum(UNITS_ON_ORDER)
		FROM TheData
	END;
	SET @UnitsOnOrder = ISNULL(@UnitsOnOrder,0)

	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','In Debug recal with on order ZERO',@UnitsOnOrder UnitsOnOrder
	--IF (@DEBUG = 1 AND @UnitsOnOrder > 0)
	--BEGIN
	--	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','In Debug making on order ZERO',@UnitsOnOrder UnitsOnOrder
	--	SET @UnitsOnOrder = 0
	--END

	
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','SET @UnitsAvail = ISNULL(@UnitsOnOrder, 0) + @aQtyOnHand',@UnitsAvail UnitsAvail,@UnitsOnOrder UnitsOnOrder,@aQtyOnHand aQtyOnHand, ROUND(@NonPromoDailySales * 56,0) 'NonPromo56DaySale',  ROUND(@aPromoDailySales * 56,0) 'Promo56DaySale'
	------------------------------------GetUnitsOnOrder   END
	
	---SET @UnitsOnOrder = 0 -- Remove this line after testing 
	SET @UnitsAvail = ISNULL(@UnitsOnOrder, 0) + @aQtyOnHand

	--// First see if enough Cover on hand + On order, if so exit
	--Select @UnitsAvail UnitsAvail , @OrderQty OrderQty , 'if so exit'
	IF (@UnitsAvail >= @OrderQty)
	BEGIN
		SET @Result = 0
		IF @DEBUG = 1 SELECT 'RETURN [CDN_SP__AddToOrder]','ALREADY IN ORDER + IN HAND GREATER IF',' (@UnitsAvail >= @OrderQty)',@OrderQty OrderQty,@UnitsAvail UnitsAvail
		RETURN
	END;

	SET @OrderQty = (@OrderQty - @UnitsAvail);
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','(@OrderQty - ISNULL(@UnitsAvail, 0));',@OrderQty OrderQty,@UnitsAvail UnitsAvail,@aMinReorderQty aMinReorderQty
	IF (
			(@aMinReorderQty > 1)
			AND (@OrderQty < @aMinReorderQty)
			)
	BEGIN
		SET @OrderQty = @aMinReorderQty
	END

	IF (
			@OrderQty < 0
			OR @OrderQty IS NULL
			)
	BEGIN
		SET @OrderQty = 0
	END

	--// Stephen: We must always round up to the next unit or the next carton, never down.
	SET @OrderQty = ceiling(@OrderQty);

	IF (
			(@OrderQty > 0)
			OR @aDoRecalc = 1
			)
	BEGIN
	IF @DEBUG = 1 SELECT '[CDN_SP__AddToOrder]','dbo.[CDN_SP__AddToOrder] @aOrderNum', 
			 @aOrderNum						aOrderNum
			,@aProduct						aProduct
			,@OrderQty						OrderQty
			,@PromCtnCost					PromCtnCost
			,0
			,@aDoRecalc						aDoRecalc
			,@aMinReorderQty		        aMinReorderQty
			,@aQtyOnHand					aQtyOnHand
			,@UnitsOnOrder					UnitsOnOrder
			,@aSalesTotal					aSalesTotal
			,@NonPromoDailySales			aNonPromoDailySales		
			,@OrderQty						OrderQty
			,@InvBuy						InvBuy
			,@aProdList						aProdList
			,@aMinOnHand					aMinOnHand
			,@aPromoUnits					aPromoUnits
			,@BuyPromoCode					BuyPromoCode
			,@aSalePromoCode				aSalePromoCode
			,@aStrSalePromoEndDate			aStrSalePromoEndDate
			,@CoverDaysUsed					CoverDaysUsed
			,@CommodityCoverDays			CommodityCoverDays
			,@aPromoDailySales				aPromoDailySales
			,@OSS_Id						OSS_Id
			,@PromoBuy						PromoBuy
			,@NonPromoBuy					NonPromoBuy
			,@aPromoDailySalesOriginal		aPromoDailySalesOriginal
			,@InvBuy 						InvBuy 
			,@PromoBuy						PromoBuy
		   , @NonPromoBuy					NonPromoBuy
		   , @PerishableProduct             Perishable
	EXEC dbo.[CDN_SP__AddToOrder]
			@aOrderNum
			,@aProduct
			,@OrderQty
			,@PromCtnCost
			,0
			,@aDoRecalc
			,@aMinReorderQty
			,@aQtyOnHand
			,@UnitsOnOrder
			,@aSalesTotal
			,@NonPromoDailySales
			,@OrderQty
			,@InvBuy 
			,@PromoBuy
		    ,@NonPromoBuy
			,@ProdDept
			,@CheaperResult
			,@CoverDaysUsed
			,@aProdList
			,@aMinOnHand
			,@aPromoUnits
			,@BuyPromoCode
			,@aSalePromoCode
			,@aStrSalePromoEndDate
			,@CoverDaysUsed
			,@CommodityCoverDays
			,@NormalCoverDays
			,@aPromoDailySales
			,@OSS_Id
			,@OSS_Outlet
			,@OSS_MultipleOrdersInAWeek
			,@OSS_Supplier
			,@OSS_CoverDays
			,@OSS_InvoiceOrderOffset
			,@OSS_ReceiveOrderOffset 
			,@OSS_DiscountThresholdThree 
			,@OSS_CoverDaysDiscountThreshold3 
			,@OSS_DiscountThresholdTwo 
			,@OSS_CoverDaysDiscountThreshold2 
			,@OSS_DiscountThresholdOne 
			,@OSS_CoverDaysDiscountThreshold1 
			,@OSS_OrderNonDefaultSupplier 
			,@PromoBuy
			,@NonPromoBuy
			,@PerishableProduct
			,@aPromoDailySalesOriginal
			,@aCartonCost
			,@PromoDisc
			,@PromoEndDate
			,@CreateOptimalOrder_Result OUT
			,@Debug = @Debug
			
	END
	ELSE
	BEGIN
		BEGIN
				IF @DEBUG = 1 SELECT 'RETURN [CDN_SP__AddToOrder]',' @OrderQty is less than zero else part',@OrderQty
				RETURN
			END
			--CodeSite.Send('Failed test "if ((OrderQty > aMinOnHand) and (OrderQty > 0)) then"');
	END
END TRY
BEGIN CATCH
    INSERT INTO [dbo].[CDN_AOO_Errors] 
				 (UserName,		ErrorNumber,	ErrorState,		ErrorSeverity,	 ErrorLine,		ErrorProcedure,		ErrorMessage,	ErrorDateTime,
				 Outlet,	 Supplier,		Product,Detail,Detail2)
    VALUES		 (SUSER_SNAME(),ERROR_NUMBER(), ERROR_STATE(),  ERROR_SEVERITY(),ERROR_LINE(),  ERROR_PROCEDURE(),  ERROR_MESSAGE(),GETDATE(),
				 @OSS_Outlet,@OSS_Supplier, @aProduct,'CDN_SP__AddToOrder',NULL);
END CATCH