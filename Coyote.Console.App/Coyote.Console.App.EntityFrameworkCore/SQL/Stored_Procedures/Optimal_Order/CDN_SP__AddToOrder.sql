USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP__AddToOrder]    Script Date: 9/5/2020 1:38:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CDN_SP__AddToOrder] (
	@aOrderNum INT														
	,@aProduct INT
	,@aOrderQty FLOAT
	,@aPromCtnCost FLOAT
	,@aSortOrder INT
	,@aDoRecalc BIT
	,@aMinReorderQty FLOAT
	,@aOnHand FLOAT
	,@aOnOrder FLOAT
	,@aSalesTotal FLOAT
	,@NonPromoDailySales FLOAT
	,@aCoverUnits FLOAT
	,@aInvBuy BIT
	,@PromoBuy BIT
	,@NonPromoBuy BIT
	,@ProdDept INT
	,@CheaperResult  NVARCHAR(30)
	,@CoverDaysUsed FLOAT
	,@aProdList VARCHAR(500)
	,@aMinOnHand FLOAT
	,@aPromoUnits FLOAT
	,@aBuyPromoCode VARCHAR(500)
	,@aSalePromoCode VARCHAR(500)
	,@aStrSalePromoEndDate VARCHAR(500)
	,@aCoverDaysUsed INT
	,@aCommodityCoverDays INT
	,@CoverDays INT
	,@aPromoDailySales FLOAT				
	,@OSS_Id INT 							
	,@OSS_Outlet INT 						
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
	,@aPromoBuy BIT
	,@aNonPromoBuy BIT
	,@Perishable BIT
	,@aPromoDailySalesOriginal FLOAT
	,@aCartonCost FLOAT
	,@PromoDisc FLOAT
	,@PromoEndDate DATE
	,@CreateOptimalOrder_Result BIT OUT
	,@Debug INT = 0
	)
AS
BEGIN TRY

	DECLARE @Outlet INT
	DECLARE @Supplier VARCHAR(500)
	DECLARE @LineNo FLOAT
	DECLARE @TotUnits FLOAT
	DECLARE @LineTotCost FLOAT
	DECLARE @OrderCtnQty FLOAT
	DECLARE @OrderUnitQty FLOAT
	DECLARE @SupplierItem VARCHAR(500)
	DECLARE @WholeCartonCoverDays INT
	DECLARE @TradingDaysCoverage FLOAT
	DECLARE @vShowOptimalReport BIT = 0
	DECLARE @vOnHandTemp FLOAT

	SET @Outlet = @OSS_Outlet
	SET @Supplier = @OSS_Supplier

	
	DECLARE @ORDL_CARTON_COST FLOAT,@ORDL_FINAL_CARTON_COST FLOAT
	DECLARE @ORDL_TOTAL_UNITS INT
	DECLARE @ORDL_LINE_TOTAL FLOAT
	DECLARE @ORDL_UNITS INT
	DECLARE @ORDL_CARTONS INT
	DECLARE @ORDL_CARTON_QTY INT
	DECLARE @ORDL_FINAL_LINE_TOTAL INT
	DECLARE @ORDL_SortOrder INT
	DECLARE @ORDL_SUGG_UNITS_ONHAND FLOAT
	DECLARE @ORDL_SUGG_UNITS_ONORDER FLOAT
	DECLARE @ORDL_SUGG_UNITS_SOLD FLOAT
	DECLARE @ORDL_SUGG_UNITS_AVGDAILY FLOAT
	DECLARE @ORDL_SUGG_UNITS_PRMAVGDAILY FLOAT
	DECLARE @ORDL_SUGG_UNITS_AVGWEEKLY FLOAT
	DECLARE @ORDL_SUGG_UNITS_COVER FLOAT
	DECLARE @ORDL_SUGG_UNITS_INVBUY BIT
	DECLARE @ORDL_Min_OnHand FLOAT
	DECLARE @ORDL_Promo_Units FLOAT
	DECLARE @ORDL_Buy_Promo_Code VARCHAR(50)
	DECLARE @ORDL_Sale_Promo_Code VARCHAR(50)
	DECLARE @ORDL_Sale_Promo_End_Date VARCHAR(50)
	DECLARE @ORDL_OUTLET FLOAT
	DECLARE @ORDL_ORDER_NO FLOAT
	DECLARE @ORDL_LINE_NO FLOAT
	DECLARE @ORDL_PRODUCT FLOAT
	DECLARE @ORDL_DESC VARCHAR(max)
	DECLARE @ORDL_Cover_Days INT
	

	-----Start--LoadProduct--Prod Table ---------
	DECLARE @PROD_NUMBER INT
		,@PROD_CARTON_QTY FLOAT
		,@PROD_DESC VARCHAR(max)
		,@PROD_TAX_CODE VARCHAR(max)

	SELECT TOP 1 @PROD_NUMBER = PROD_NUMBER
		,@PROD_CARTON_QTY = PROD_CARTON_QTY
		,@PROD_DESC = PROD_DESC,
		@PROD_TAX_CODE = PROD_TAX_CODE
	FROM  [dbo].ProdTbl WITH (NOLOCK)
	WHERE Prod_NUMBER = @aProduct
	
	IF (@PROD_NUMBER IS NULL)
	BEGIN
		IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','Prod not exist '
		RETURN
	END
	-----End--LoadProduct--Prod Table ---------
	-----START--fOutletProdTbl--OUTPTBL Table ---------
	DECLARE @OUTP_CARTON_COST FLOAT,@ISOutletProdExists BIT

	SELECT TOP 1 @OUTP_CARTON_COST = OUTP_CARTON_COST,@ISOutletProdExists= OUTP_PRODUCT
	FROM [dbo].OUTPTBL WITH (NOLOCK)
	WHERE OUTP_PRODUCT = @aProduct
		AND OUTP_OUTLET = @Outlet
	IF @ISOutletProdExists IS NULL
	BEGIN
		IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','Outlet Prod not exist ', @aProduct Product,@Outlet Outlet
		RETURN
	END
	SET @OUTP_CARTON_COST = ISNULL(@OUTP_CARTON_COST,0)
	-----START--fOutletProdTbl--OUTPTBL Table ---------
	
	-----Start--fOrdlTbl--OrdlTbl Table ---------
	DECLARE @IsOrderExistsIn BIT = 0
	SELECT TOP 1 @ORDL_TOTAL_UNITS = ORDL_TOTAL_UNITS,@IsOrderExistsIn = CASE WHEN ORDL_ORDER_NO IS NOT NULL THEN 1 ELSE 0 END
		,@ORDL_FINAL_CARTON_COST = ORDL_FINAL_CARTON_COST,@ORDL_LINE_NO = ORDL_LINE_NO
		,@ORDL_Cover_Days = ORDL_Cover_Days
	FROM [dbo].ORDLTBL WITH (NOLOCK) --FROM [dbo].ORDLTBL WITH (NOLOCK)
	WHERE ORDL_OUTLET = @Outlet
		AND ORDL_ORDER_NO = @aOrderNum
		--AND ORDL_LINE_NO = @aOrderNum
		AND ORDL_PRODUCT = @aProduct

	SET @IsOrderExistsIn = ISNULL(@IsOrderExistsIn,0)

	-----End--fOrdlTbl--OrdlTbl Table ---------
	
	IF @IsOrderExistsIn=1 --(@IsOrderExistsIn > 0)
	BEGIN	
		IF @aDoRecalc = 0
		BEGIN
			SET @TotUnits = @aOrderQty
		END
		ELSE
		BEGIN
			SET @TotUnits = ISNULL(@ORDL_TOTAL_UNITS,0) + @aOrderQty;
				--// Added this if this product already added in order and the same product is sustitute product of deleted host product. So combine sales rate should be used.
				--//aDailySales := fOrdlTbl.ORDL_SUGG_UNITS_AVGDAILY.ActualColValue + aDailySales;
		END;	
	END
	ELSE
	BEGIN
		SET @TotUnits = @aOrderQty
		--Note Need to check for NextLineNo
		SET @ORDL_LINE_NO = (
				SELECT Max(ORDL_LINE_NO) AS MaxLineNo
				--FROM [dbo].ORDLTBL WITH (NOLOCK)
				FROM [dbo].ORDLTBL WITH (NOLOCK)
				WHERE ORDL_OUTLET = @Outlet
					AND ORDL_ORDER_NO = @aOrderNum
				)
		SET @ORDL_LINE_NO=ISNULL(@ORDL_LINE_NO,0)+1
	END


	DECLARE @PROD_CARTON_QTYOne FLOAT 
	IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','@ORDL_LINE_NO=ISNULL(@ORDL_LINE_NO,0)',@ORDL_LINE_NO ORDL_LINE_NO,@OrderUnitQty OrderUnitQty,@OrderCtnQty OrderCtnQty,@PROD_CARTON_QTY PROD_CARTON_QTY,@TotUnits TotUnits

	SET @OrderUnitQty = @TotUnits
	SET @OrderCtnQty = 0
	IF (@PROD_CARTON_QTY <> 0)
	BEGIN
		SET @PROD_CARTON_QTYOne = Case when ISNULL(@PROD_CARTON_QTY,0) =0 THEN 1 ELSE @PROD_CARTON_QTY  END
		SET @OrderCtnQty = @TotUnits / @PROD_CARTON_QTYOne
		IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','@ORDL_LINE_NO=ISNULL(@ORDL_LINE_NO,0)+1',@TotUnits TotUnits,@PROD_CARTON_QTY PROD_CARTON_QTY,@OrderCtnQty OrderCtnQty,@PROD_CARTON_QTY PROD_CARTON_QTY,@TotUnits TotUnits
	END
	DECLARE @IsPerishableProduct BIT = (SELECT CASE WHEN (@ProdDept > 0) AND @ProdDept IN(2,4,24,25) THEN 1 ELSE 0 END)
	
	 
    IF @Debug = 1 SELECT ROUND(@NonPromoDailySales * 56,0) 'NonPromo56DaySale' , ROUND(@aPromoDailySales * 56,0) 'Promo56DaysSale'

	IF @IsPerishableProduct=1 -- Perishable
	BEGIN
		IF (@OSS_MultipleOrdersInAWeek = 0) -- then // single order in week
		BEGIN
			IF (@OrderCtnQty >= 0.7)
			BEGIN
				SET @OrderUnitQty = 0
				SET @OrderCtnQty = @OrderCtnQty + 0.3
				SET @OrderCtnQty = Round(@OrderCtnQty, 0)
				SET @TotUnits = @OrderCtnQty * @PROD_CARTON_QTY
			END
			ELSE
			BEGIN
				SET @OrderCtnQty = 0
				SET @OrderUnitQty = @TotUnits
			END
		END
		ELSE
		BEGIN
			--// Multiple order in week
			SET @OrderCtnQty = 0
			SET @OrderUnitQty = @TotUnits
		END
		IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','IF ((@OrderCtnQty > 0) AND (@OrderCtnQty < 1)) ws2ew4dghj',@OSS_MultipleOrdersInAWeek OSS_MultipleOrdersInAWeek,@OrderCtnQty OrderCtnQty,@OrderUnitQty OrderUnitQty,@TotUnits TotUnits
	END
	ELSE
	BEGIN
		IF (@aInvBuy = 0) --then --// Normal Order
		BEGIN
			IF ((@OrderCtnQty > 0) AND (@OrderCtnQty < 1))
			BEGIN
				IF(@PromoBuy = 1)
				BEGIN
					IF ( ROUND( @aPromoDailySales * 56,0) >= @PROD_CARTON_QTY)
					BEGIN
						SET @OrderCtnQty = ceiling(@OrderCtnQty)
						SET @OrderUnitQty = 0
						SET @TotUnits = @OrderCtnQty * @PROD_CARTON_QTY
					END
					ELSE
					BEGIN
						SET @OrderCtnQty = 0
						SET @OrderUnitQty = @TotUnits
					END
				 END
				ELSE IF (@NonPromoBuy = 1)
				BEGIN
					IF ( ROUND(@NonPromoDailySales * 56,0) >= @PROD_CARTON_QTY)
					BEGIN
						SET @OrderCtnQty = ceiling(@OrderCtnQty)
						SET @OrderUnitQty = 0
						SET @TotUnits = @OrderCtnQty * @PROD_CARTON_QTY
					END
					ELSE
					BEGIN
						SET @OrderCtnQty = 0
						SET @OrderUnitQty = @TotUnits
					END
				END

			END
			ELSE IF (@OrderCtnQty >= 1)
			BEGIN
				SET @OrderCtnQty = ceiling(@OrderCtnQty)
				SET @OrderUnitQty = 0
				SET @TotUnits = @OrderCtnQty * @PROD_CARTON_QTY
			END
			IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','IF ((@OrderCtnQty > 0) AND (@OrderCtnQty < 1)) ws2ew4dghj',@OrderCtnQty OrderCtnQty,@OrderUnitQty OrderUnitQty,@TotUnits TotUnits
		END
		ELSE
		BEGIN
			IF ((@OrderCtnQty > 0) AND (@OrderCtnQty < 1))
			BEGIN
				SET @OrderCtnQty = 0
				SET @OrderUnitQty = @TotUnits
			END
			ELSE IF (@OrderCtnQty >= 1)
			BEGIN
				SET @OrderCtnQty = Floor(@OrderCtnQty)
				SET @OrderUnitQty = 0
				SET @TotUnits = @OrderCtnQty * @PROD_CARTON_QTY
			END
			IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','IF ((@OrderCtnQty > 0) AND (@OrderCtnQty < 1)) waasdghj',@OrderCtnQty OrderCtnQty,@OrderUnitQty OrderUnitQty,@TotUnits TotUnits
		END
	END
	

	IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','IF (@aPromCtnCost > 0)',@aPromCtnCost aPromCtnCost,@OrderUnitQty OrderUnitQty,@TotUnits TotUnits,@aInvBuy aInvBuy,@OrderCtnQty OrderCtnQty
	IF (@aInvBuy = 1 and @aPromCtnCost > 0)
	BEGIN
		IF ((@OrderUnitQty > 0) AND (@OrderCtnQty = 0))
		BEGIN
			SET @PROD_CARTON_QTYOne = Case when ISNULL(@PROD_CARTON_QTY,0) =0 THEN 1 ELSE @PROD_CARTON_QTY  END
			SET @LineTotCost = (@TotUnits * @aPromCtnCost) / @PROD_CARTON_QTYOne
		END
		ELSE IF ((@OrderUnitQty = 0) AND (@OrderCtnQty > 0))
		BEGIN
			SET @LineTotCost = @OrderCtnQty * @aPromCtnCost
		END
		SET @ORDL_CARTON_COST = @aCartonCost
		SET @ORDL_FINAL_CARTON_COST = @aPromCtnCost
	IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]',' ((@OrderUnitQty > 0) AND (@OrderCtnQty = 0)) sdsdaqs',@PROD_CARTON_QTY,@OUTP_CARTON_COST,@ORDL_CARTON_COST ORDL_CARTON_COST,@ORDL_FINAL_CARTON_COST ORDL_FINAL_CARTON_COST
	END
	ELSE
	BEGIN
		IF ((@OrderUnitQty > 0) AND (@OrderCtnQty = 0))
		BEGIN
			SET  @PROD_CARTON_QTYOne = Case when ISNULL(@PROD_CARTON_QTY,0) =0 THEN 1 ELSE @PROD_CARTON_QTY  END
			SET @LineTotCost = (@TotUnits * @OUTP_CARTON_COST) / @PROD_CARTON_QTYOne
		END
		ELSE IF ((@OrderUnitQty = 0) AND (@OrderCtnQty > 0))
		BEGIN
			SET @LineTotCost = @OrderCtnQty * @OUTP_CARTON_COST
		END
		SET @ORDL_CARTON_COST = @OUTP_CARTON_COST
		SET @ORDL_FINAL_CARTON_COST = @OUTP_CARTON_COST
		IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','((@OrderUnitQty > 0) AND (@OrderCtnQty = 0)) wsdghj',@PROD_CARTON_QTY,@OUTP_CARTON_COST,@ORDL_CARTON_COST ORDL_CARTON_COST,@ORDL_FINAL_CARTON_COST ORDL_FINAL_CARTON_COST

	END
	IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','SET @ORDL_FINAL_CARTON_COST = @OUTP_CARTON_COST',@PROD_CARTON_QTY,@OUTP_CARTON_COST,@ORDL_CARTON_COST ORDL_CARTON_COST,@ORDL_FINAL_CARTON_COST ORDL_FINAL_CARTON_COST

	-- // ============== New Implementation for carton/units rounding calcalation ends =====================
	SET @ORDL_OUTLET = @Outlet
	SET @ORDL_ORDER_NO = @aOrderNum
	--SET @ORDL_LINE_NO = @LineNo
	SET @ORDL_PRODUCT = @aProduct
	SET @ORDL_DESC = @PROD_DESC
	SET @ORDL_CARTONS = @OrderCtnQty
	SET @ORDL_UNITS = @OrderUnitQty
	SET @ORDL_TOTAL_UNITS = @TotUnits
	SET @ORDL_LINE_TOTAL = ROUND(@LineTotCost,2)
	SET @ORDL_CARTON_QTY = @PROD_CARTON_QTY	
	DECLARE @ORDL_GST_IND NVARCHAR(1) = ''
	DECLARE @NewMinOnHand FLOAT = 0
	
	IF @PROD_TAX_CODE ='GST'
		SET @ORDL_GST_IND = '*'

	SET @ORDL_FINAL_LINE_TOTAL = @LineTotCost
	SET @ORDL_SortOrder = @aSortOrder
	SET @ORDL_SUGG_UNITS_ONHAND = @aOnHand
	SET @ORDL_SUGG_UNITS_ONORDER = ISNULL(@aOnOrder, 0)
	SET @ORDL_SUGG_UNITS_SOLD = @aSalesTotal
	SET @ORDL_SUGG_UNITS_AVGDAILY = @NonPromoDailySales
	SET @ORDL_SUGG_UNITS_PRMAVGDAILY = @aPromoDailySales
	SET @ORDL_SUGG_UNITS_AVGWEEKLY = @NonPromoDailySales * 7
	SET @ORDL_SUGG_UNITS_COVER = @aCoverUnits
	SET @ORDL_SUGG_UNITS_INVBUY = @aInvBuy
	SET @ORDL_Min_OnHand = @aMinOnHand
	SET @ORDL_Promo_Units = @aPromoUnits
	SET @ORDL_Buy_Promo_Code = @aBuyPromoCode
	SET @ORDL_Sale_Promo_Code = @aSalePromoCode
	SET @ORDL_Sale_Promo_End_Date = @aStrSalePromoEndDate

	IF (@vShowOptimalReport = 1)
	BEGIN
		SET @ORDL_CARTON_QTY = @vOnHandTemp --// Actual On Hand
		SET @ORDL_CARTONS = @TotUnits - @vOnHandTemp --// Diff
		SET @ORDL_Sale_Promo_End_Date = @Supplier
	END
	--   // the number of trading days that this stock will cover
	
	---SET @NewMinOnHand = 0
	---if @aPromoUnits >  0
	--	SET @NewMinOnHand = @aPromoUnits
  ---  ELSE
	---	SET @NewMinOnHand = @aMinOnHand
	

	IF (@NonPromoDailySales > 0)
	BEGIN	
			
		IF(@aInvBuy = 1)
			SET @TradingDaysCoverage = ((@aPromoDailySales * @CoverDays + @NonPromoDailySales *(@aCoverDaysUsed-@CoverDays) + @aOnHand + ISNULL(@aOnOrder, 0) - @aMinOnHand )) * 2 / (@aPromoDailySales+@NonPromoDailySales)
		ELSE IF (@aPromoBuy = 1)
			SET @TradingDaysCoverage = (@TotUnits + @aOnHand + ISNULL(@aOnOrder, 0) - @aPromoUnits) / @aPromoDailySales
			---IF(@IsPerishableProduct=1)
			---	SET @TradingDaysCoverage = (@TotUnits + @aOnHand + ISNULL(@aOnOrder, 0) - @aMinOnHand) / @NonPromoDailySales
			---ELSE				
				----SET @TradingDaysCoverage = (@TotUnits + @aOnHand + ISNULL(@aOnOrder, 0) - @aPromoUnits) / @aPromoDailySales

		ELSE		
			SET @TradingDaysCoverage = (@TotUnits + @aOnHand + ISNULL(@aOnOrder, 0) - @aMinOnHand) / @NonPromoDailySales
		
		IF ((@TradingDaysCoverage - Floor(@TradingDaysCoverage)) >= 0.5)
		BEGIN
			SET @TradingDaysCoverage = @TradingDaysCoverage + 1
		END
		ELSE
		BEGIN
			SET @TradingDaysCoverage = @TradingDaysCoverage
		END
	END
	ELSE
	BEGIN
		SET @TradingDaysCoverage = 999
	END

	IF (@TradingDaysCoverage > 999)
	BEGIN
		SET @TradingDaysCoverage = 999
	END
	
	IF @Debug =1
	SELECT @NonPromoDailySales [Av Daily Sales],@aPromoDailySales [Av Daily Sales Prom],@aMinOnHand [Min on Hand],(@aPromoDailySales*3)[Min on Hand Prom]
	,@aOnHand [On Hand],@aOnOrder [On Order],@PROD_CARTON_QTY [Carton Qty],@aCartonCost [Carton Cost],@ORDL_CARTONS [Cartons],@ORDL_CARTON_COST [Line Total]
	,@OrderUnitQty OrderUnitQty
	,@OrderCtnQty OrderCtnQty
	,@TotUnits TotUnits
	,@CoverDays CoverDays
	,@aCoverDaysUsed CoverDaysUsed

	DECLARE @SPRD_SUPPLIER_ITEM VARCHAR(100)=(SELECT TOP 1 SPRD_SUPPLIER_ITEM FROM dbo.SPRDTBL WITH(NOLOCK)  WHERE SPRD_PRODUCT=@aProduct)
	SET @ORDL_Cover_Days = Floor(@TradingDaysCoverage)
	
		-------------------Select Statements------------------------


		IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','IF (@IsOrderExistsIn > 0)','UPDATE',@ORDL_ORDER_NO, @ORDL_LINE_NO,@ORDL_OUTLET
		IF @DEBUG = 5
		SELECT  @ORDL_OUTLET																ORDL_OUTLET											
			,@ORDL_ORDER_NO																ORDL_ORDER_NO										
			,@ORDL_LINE_NO+1															ORDL_LINE_NO										
			,@ORDL_PRODUCT																ORDL_PRODUCT										
			,@ORDL_DESC																	ORDL_DESC											
			,@SPRD_SUPPLIER_ITEM														ORDL_SUPPLIER_ITEM									
			,@ORDL_CARTON_COST															ORDL_CARTON_COST									
			,@ORDL_CARTONS																ORDL_CARTONS										
			,@ORDL_UNITS																ORDL_UNITS											
			,@ORDL_TOTAL_UNITS															ORDL_TOTAL_UNITS									
			--,@ORDL_CARTONS															-,ORDL_ORDER_CARTONS								
			--,@ORDL_UNITS																-,ORDL_ORDER_UNITS									
			--,@ORDL_TOTAL_UNITS														-,ORDL_ORDER_TOTAL_UNITS							
			--,@ORDL_DELIVER_CARTONS													-,ORDL_DELIVER_CARTONS								
			--,@ORDL_DELIVER_UNITS														-,ORDL_DELIVER_UNITS								
			--,@ORDL_DELIVER_TOTAL_UNITS												-,ORDL_DELIVER_TOTAL_UNITS							
			,@ORDL_LINE_TOTAL															ORDL_LINE_TOTAL									
			,@ORDL_CARTON_QTY															ORDL_CARTON_QTY									
			,@ORDL_GST_IND																ORDL_GST_IND										
			--,@ORDL_BONUS_IND															-,ORDL_BONUS_IND									
			--,GETDATE()--@ORDL_POSTED_DATE												-, ORDL_POSTED_DATE									
			--,'ORDER' --@ORDL_DOCUMENT_TYPE											-,ORDL_DOCUMENT_TYPE								
			--,'NEW'--@ORDL_DOCUMENT_STATUS												-,ORDL_DOCUMENT_STATUS								
			,@ORDL_LINE_TOTAL			--	,@ORDL_FINAL_LINE_TOTAL						ORDL_FINAL_LINE_TOTAL								
			,@ORDL_FINAL_CARTON_COST													ORDL_FINAL_CARTON_COST								
			--,@ORDL_FINAL_GST_AMT														-,ORDL_FINAL_GST_AMT
			,@aMinReorderQty                                                            MinReorderQty
			,@ORDL_SUGG_UNITS_ONHAND													ORDL_SUGG_UNITS_ONHAND
			,@ORDL_SUGG_UNITS_ONORDER													ORDL_SUGG_UNITS_ONORDER
			,@ORDL_SUGG_UNITS_SOLD														ORDL_SUGG_UNITS_SOLD
			,@ORDL_SUGG_UNITS_AVGDAILY													ORDL_SUGG_UNITS_AVGDAILY
			,@ORDL_SUGG_UNITS_COVER														ORDL_SUGG_UNITS_COVER
			,@ORDL_SUGG_UNITS_AVGWEEKLY													ORDL_SUGG_UNITS_AVGWEEKLY
			--,@ORDL_Manual_Units_Adjsutment											-,ORDL_Manual_Units_Adjsutment
			,@ORDL_SortOrder															ORDL_SortOrder
			,@ORDL_SUGG_UNITS_INVBUY													ORDL_SUGG_UNITS_INVBUY
			,@ORDL_Promo_Units															ORDL_Promo_Units
			,@ORDL_Min_OnHand															ORDL_Min_OnHand
			,@ORDL_Buy_Promo_Code														ORDL_Buy_Promo_Code
			,@ORDL_Sale_Promo_Code														ORDL_Sale_Promo_Code
			,@ORDL_Sale_Promo_End_Date													ORDL_Sale_Promo_End_Date
			,@ORDL_Cover_Days															ORDL_Cover_Days
			,@aPromoDailySales															ORDL_SUGG_UNITS_PROMO_AVGDAILY
			--,@ORDL_SUGG_UNITS_INVBUY													INVBUY
			,@PromoBuy																	ORDL_PromoBuy
		    ,@NonPromoBuy																ORDL_NonPromoBuy
			--,CASE WHEN @CheaperResult IS NULL THEN 0 ELSE 1 END							CheaperResult
			,@CoverDaysUsed																ORDL_CoverDaysUsed
			,@CheaperResult																ORDL_CheaperSupplier
			,@CoverDays 																ORDL_CoverDays
			--,@aCommodityCoverDays 														CommodityCoverDays
			--,@ProdDept 																	ProductDepartment				
			,@OSS_OrderNonDefaultSupplier 												ORDL_CheckSupplier
			,@PromoDisc																	ORDL_PromoDisc
			,@PromoEndDate																ORDL_PromoEndDate
			,@Perishable                                                                ORDL_Perishable
			,ROUND(@NonPromoDailySales * 56,0)                                          ORDL_NonPromoSales56Days
			,ROUND(@aPromoDailySales * 56,0)                                            ORDL_PromoSales56Days
		IF (@IsOrderExistsIn = 1)
			BEGIN
				IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','IF (@IsOrderExistsIn > 0)','UPDATE',@ORDL_ORDER_NO ORDL_ORDER_NO

				UPDATE ORDLTBL SET
				ORDL_OUTLET					 =				@ORDL_OUTLET				,					--	ORDL_OUTLET
				ORDL_ORDER_NO				 =				@ORDL_ORDER_NO				,					--	,ORDL_ORDER_NO
				ORDL_LINE_NO				 =				@ORDL_LINE_NO				,					--	,ORDL_LINE_NO
				ORDL_PRODUCT				 =				@ORDL_PRODUCT				,					--	,ORDL_PRODUCT
				ORDL_DESC					 =				@ORDL_DESC					,					--	,ORDL_DESC
				ORDL_SUPPLIER_ITEM			 =				@SPRD_SUPPLIER_ITEM			,					--	,ORDL_CARTONS
				ORDL_CARTON_COST			 =				@ORDL_CARTON_COST			,					--	,ORDL_UNITS
				ORDL_CARTONS				 =				@ORDL_CARTONS				,					--	,ORDL_TOTAL_UNITS
				ORDL_UNITS					 =				@ORDL_UNITS					,					--	,ORDL_LINE_TOTAL
				ORDL_TOTAL_UNITS			 =				@ORDL_TOTAL_UNITS			,					--	,ORDL_CARTON_QTY
				--ORDL_ORDER_CARTONS			 =				@ORDL_CARTONS			,				--	,ORDL_FINAL_LINE_TOTAL
				--ORDL_ORDER_UNITS			 =				@ORDL_UNITS			,					--	,ORDL_SortOrder
				--ORDL_ORDER_TOTAL_UNITS		 =				@ORDL_TOTAL_UNITS		,				--	,ORDL_SUGG_UNITS_ONHAND
				--ORDL_DELIVER_CARTONS		 =				@ORDL_DELIVER_CARTONS		,					--	,ORDL_SUGG_UNITS_ONORDER
				--ORDL_DELIVER_UNITS			 =				@ORDL_DELIVER_UNITS			,				--	,ORDL_SUGG_UNITS_SOLD
				--ORDL_DELIVER_TOTAL_UNITS	 =				@ORDL_DELIVER_TOTAL_UNITS	,					--	,ORDL_SUGG_UNITS_AVGDAILY
				ORDL_LINE_TOTAL				 =				@ORDL_LINE_TOTAL			,					--	,ORDL_SUGG_UNITS_AVGWEEKLY
				ORDL_CARTON_QTY				 =				@ORDL_CARTON_QTY			,					--	,ORDL_SUGG_UNITS_COVER
				ORDL_GST_IND				 =				@ORDL_GST_IND				,					--	,ORDL_SUGG_UNITS_INVBUY
				--ORDL_BONUS_IND				 =				@ORDL_BONUS_IND				,				--	,ORDL_Min_OnHand
				--ORDL_POSTED_DATE			 =				GETDATE(),--@ORDL_POSTED_DATE			,					--	,ORDL_Promo_Units
				--ORDL_DOCUMENT_TYPE			 =				'ORDER'			,					--	,ORDL_Buy_Promo_Code
				--ORDL_DOCUMENT_STATUS		 =				'NEW',					--	,ORDL_Sale_Promo_Code
				ORDL_FINAL_LINE_TOTAL		 =				@ORDL_LINE_TOTAL		,					--	,ORDL_Sale_Promo_End_Date
				ORDL_FINAL_CARTON_COST		 =				@ORDL_FINAL_CARTON_COST		,					--	,ORDL_Cover_Days
				--ORDL_FINAL_GST_AMT			 =				@ORDL_FINAL_GST_AMT			,					--	,ORDL_SUGG_UNITS_PRMAVGDAILY
				ORDL_MinReorderQty                =              @aMinReorderQty,
				ORDL_SUGG_UNITS_ONHAND		 =				@ORDL_SUGG_UNITS_ONHAND		,
				ORDL_SUGG_UNITS_ONORDER		 =				@ORDL_SUGG_UNITS_ONORDER	,
				ORDL_SUGG_UNITS_SOLD		 =				@ORDL_SUGG_UNITS_SOLD		,
				ORDL_SUGG_UNITS_AVGDAILY	 =				@ORDL_SUGG_UNITS_AVGDAILY	,
				ORDL_SUGG_UNITS_COVER		 =				@ORDL_SUGG_UNITS_COVER      ,
				ORDL_SUGG_UNITS_AVGWEEKLY    =				@ORDL_SUGG_UNITS_AVGWEEKLY	,
				--ORDL_Manual_Units_Adjsutment =				@ORDL_Manual_Units_Adjsutment,
				ORDL_SortOrder				 =				@ORDL_SortOrder				,
				ORDL_SUGG_UNITS_INVBUY		 =				@ORDL_SUGG_UNITS_INVBUY		,
				ORDL_Promo_Units			 =				@ORDL_Promo_Units			,
				ORDL_Min_OnHand				 =				@ORDL_Min_OnHand			,
				ORDL_Buy_Promo_Code			 =				@ORDL_Buy_Promo_Code		,
				ORDL_Sale_Promo_Code		 =				@ORDL_Sale_Promo_Code		,
				ORDL_Sale_Promo_End_Date	 =				@ORDL_Sale_Promo_End_Date	,
				ORDL_Cover_Days				 =				@ORDL_Cover_Days			,
				ORDL_SUGG_UNITS_PROMO_AVGDAILY	 =			@aPromoDailySales
				--,INVBUY								=		@ORDL_SUGG_UNITS_INVBUY
				,ORDL_PromoBuy							=		@PromoBuy
				,ORDL_NonPromoBuy						=		@NonPromoBuy
				--,CheaperResult						=		CASE WHEN @CheaperResult IS NULL THEN 0 ELSE 1 END
				,ORDL_CoverDaysUsed						=		@CoverDaysUsed
				,ORDL_CheaperSupplier					=		@CheaperResult
				--,CommodityCoverDays					=		@aCommodityCoverDays 
				,ORDL_CoverDays							=		@CoverDays 
				--,ProductDepartment					=		@ProdDept 
				,ORDL_CheckSupplier						=		@OSS_OrderNonDefaultSupplier 
				,ORDL_PromoDisc							=		@PromoDisc
				,ORDL_PromoEndDate						=		@PromoEndDate
				,ORDL_Perishable                         =       @Perishable
				,ORDL_NonPromoSales56Days                =       ROUND(@NonPromoDailySales * 56,0)
				,ORDL_PromoSales56Days                   =       ROUND(@aPromoDailySales * 56,0)
				WHERE
				ORDL_LINE_NO = @ORDL_LINE_NO AND ORDL_PRODUCT = @ORDL_PRODUCT AND ORDL_OUTLET =@ORDL_OUTLET AND ORDL_ORDER_NO =@ORDL_ORDER_NO
		IF @Debug = 1
			SELECT * From ORDLTBL WHERE (ORDL_OUTLET = @ORDL_OUTLET)  AND  ORDL_Product = @ORDL_PRODUCT and ORDL_LINE_NO = @ORDL_LINE_NO
				--UNION Select 0 NEW,*, NULL,NULL,NULL,NULL,NULL,NULL ,NULL,NULL,NULL,NULL,NULL,NULL From ORDLTBL WHERE (ORDL_OUTLET = @ORDL_OUTLET)  AND  ORDL_Product = @ORDL_PRODUCT and  ORDL_ORDER_NO=@ORDL_ORDER_NO and ORDL_LINE_NO = @ORDL_LINE_NO
				--ORDER BY ORDL_ORDER_NO DESC ,NEW
			END
		ELSE
			BEGIN
			IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]','INSERT ',@ORDL_ORDER_NO ORDL_ORDER_NO, @ORDL_LINE_NO ORDL_LINE_NO
			INSERT INTO ORDLTBL (
				ORDL_OUTLET											
				,ORDL_ORDER_NO										
				,ORDL_LINE_NO										
				,ORDL_PRODUCT										
				,ORDL_DESC											
				,ORDL_SUPPLIER_ITEM									
				,ORDL_CARTON_COST									
				,ORDL_CARTONS										
				,ORDL_UNITS											
				,ORDL_TOTAL_UNITS									
				--,ORDL_ORDER_CARTONS								
				--,ORDL_ORDER_UNITS									
				--,ORDL_ORDER_TOTAL_UNITS							
				--,ORDL_DELIVER_CARTONS								
				--,ORDL_DELIVER_UNITS								
				--,ORDL_DELIVER_TOTAL_UNITS							
				,ORDL_LINE_TOTAL									
				,ORDL_CARTON_QTY									
				,ORDL_GST_IND										
				--,ORDL_BONUS_IND									
				--, ORDL_POSTED_DATE									
				--,ORDL_DOCUMENT_TYPE								
				--,ORDL_DOCUMENT_STATUS								
				,ORDL_FINAL_LINE_TOTAL								
				,ORDL_FINAL_CARTON_COST								
				--,ORDL_FINAL_GST_AMT
				,ORDL_SUGG_UNITS_ONHAND
				,ORDL_SUGG_UNITS_ONORDER
				,ORDL_SUGG_UNITS_SOLD
				,ORDL_SUGG_UNITS_AVGDAILY
				,ORDL_SUGG_UNITS_COVER
				,ORDL_SUGG_UNITS_AVGWEEKLY
				--,ORDL_Manual_Units_Adjsutment
				,ORDL_SortOrder
				,ORDL_SUGG_UNITS_INVBUY
				,ORDL_Promo_Units
				,ORDL_Min_OnHand
				,ORDL_Buy_Promo_Code
				,ORDL_Sale_Promo_Code
				,ORDL_Sale_Promo_End_Date
				,ORDL_Cover_Days
				,ORDL_SUGG_UNITS_PROMO_AVGDAILY
				--,INVBUY
				,ORDL_PromoBuy
				,ORDL_NonPromoBuy
				--,CheaperResult
				,ORDL_CoverDaysUsed
				,ORDL_CheaperSupplier
				,ORDL_CoverDays
				--,CommodityCoverDays
				--,ProductDepartment				
				,ORDL_CheckSupplier
				,ORDL_PromoDisc
				,ORDL_PromoEndDate
				,ORDL_MinReorderQty
				,ORDL_Perishable
				,ORDL_NonPromoSales56Days
				,ORDL_PromoSales56Days
				)
			SELECT 
			@ORDL_OUTLET
			,@ORDL_ORDER_NO
			,@ORDL_LINE_NO
			,@ORDL_PRODUCT
			,@ORDL_DESC
			,@SPRD_SUPPLIER_ITEM
			,@ORDL_CARTON_COST
			,@ORDL_CARTONS
			,@ORDL_UNITS
			,@ORDL_TOTAL_UNITS
			--,@ORDL_CARTONS				
			--,@ORDL_UNITS
			--,@ORDL_TOTAL_UNITS			
			--,@ORDL_DELIVER_CARTONS
			--,@ORDL_DELIVER_UNITS
			--,@ORDL_DELIVER_TOTAL_UNITS
			,@ORDL_LINE_TOTAL
			,@ORDL_CARTON_QTY
			,@ORDL_GST_IND
			--,@ORDL_BONUS_IND
			--,GETDATE()--@ORDL_POSTED_DATE
			--,'ORDER' --@ORDL_DOCUMENT_TYPE
			--,'NEW'--@ORDL_DOCUMENT_STATUS
			,@ORDL_LINE_TOTAL			--	,@ORDL_FINAL_LINE_TOTAL
			,@ORDL_FINAL_CARTON_COST
			--,@ORDL_FINAL_GST_AMT
			,@ORDL_SUGG_UNITS_ONHAND
			,@ORDL_SUGG_UNITS_ONORDER
			,@ORDL_SUGG_UNITS_SOLD
			,@ORDL_SUGG_UNITS_AVGDAILY
			,@ORDL_SUGG_UNITS_COVER
			,@ORDL_SUGG_UNITS_AVGWEEKLY
			--,@ORDL_Manual_Units_Adjsutment
			,@ORDL_SortOrder
			,@ORDL_SUGG_UNITS_INVBUY
			,@ORDL_Promo_Units
			,@ORDL_Min_OnHand
			,@ORDL_Buy_Promo_Code
			,@ORDL_Sale_Promo_Code
			,@ORDL_Sale_Promo_End_Date
			,@ORDL_Cover_Days
			,@aPromoDailySales
			--,@ORDL_SUGG_UNITS_INVBUY
			,@PromoBuy
		    ,@NonPromoBuy
			--,CASE WHEN @CheaperResult IS NULL THEN 0 ELSE 1 END
			,@CoverDaysUsed
			,@CheaperResult
			,@CoverDays 
			--,@aCommodityCoverDays 
			--,@ProdDept 
			,@OSS_OrderNonDefaultSupplier 
			,@PromoDisc
			,@PromoEndDate
			,@aMinReorderQty
			,@Perishable
			,ROUND(@NonPromoDailySales * 56,0)
			,ROUND(@aPromoDailySales * 56,0)
			
		IF @Debug = 1 SELECT * FROM ORDLTBL 
			WHERE (ORDL_OUTLET = @ORDL_OUTLET)  AND  ORDL_Product = @ORDL_PRODUCT and ORDL_LINE_NO = @ORDL_LINE_NO
				--UNION Select 0 NEW,*, NULL,NULL,NULL,NULL,NULL,NULL ,NULL,NULL,NULL,NULL,NULL,NULL From ORDLTBL WHERE (ORDL_OUTLET = @ORDL_OUTLET)  AND  ORDL_Product = @ORDL_PRODUCT and  ORDL_ORDER_NO=@ORDL_ORDER_NO and ORDL_LINE_NO = @ORDL_LINE_NO
				--ORDER BY ORDL_ORDER_NO DESC ,NEW
		END
		
		UPDATE [dbo].CDN_BEST_BY_SUPPLIER SET BEST_BUY = 0, [UpdatedAt] = GETDATE() WHERE PRODUCT = @aProduct and SUPPLIER = @OSS_Supplier AND OUTLET = @ORDL_OUTLET

		SET @CreateOptimalOrder_Result = 1

END TRY
BEGIN CATCH
    INSERT INTO [dbo].[CDN_AOO_Errors] 
				 (Username,		ErrorNumber,	ErrorState,		ErrorSeverity,	 ErrorLine,		ErrorProcedure,		ErrorMessage,	ErrorDateTime,
				 Outlet,	 Supplier,		Product,Detail,Detail2)
    VALUES		 (SUSER_SNAME(),ERROR_NUMBER(), ERROR_STATE(),  ERROR_SEVERITY(),ERROR_LINE(),  ERROR_PROCEDURE(),  ERROR_MESSAGE(),GETDATE(),
				 @OSS_Outlet,@OSS_Supplier, @aProduct,'CDN_SP__DoOptimalOrder',@ORDL_ORDER_NO);
END CATCH
