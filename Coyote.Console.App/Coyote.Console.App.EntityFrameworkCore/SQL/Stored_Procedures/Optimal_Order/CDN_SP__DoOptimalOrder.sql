USE [AASandbox]
GO
/****** Object:  StoredProcedure [dbo].[CDN_SP__DoOptimalOrder]    Script Date: 9/5/2020 1:41:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[CDN_SP__DoOptimalOrder] 
	@aRunDate DATETIME								
	,@aSalesStartDate DATETIME				
	,@aSalesEndDate DATETIME				
	,@aDoRecalc BIT							
	,@vShowOptimalReport BIT 				
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
	,@aOrderNum INT = - 1					
	,@aProdList VARCHAR(max) = NULL			
	,@PromoDailySales FLOAT = 0
	,@PromoTotalSales FLOAT = 0
	,@SalePromoCode VARCHAR(50) = NULL
	,@StrSalePromoEndDate VARCHAR(50) = NULL
	,@IsANewProd BIT = 0
	,@CreateOptimalOrder_Result BIT = NULL OUT
	,@Debug int = 0
	,@DebugProduct INT = NULL
AS
BEGIN TRY
	IF @DEBUG = 1 SELECT '[CDN_SP__DoOptimalOrder]'
	DECLARE @PromoSalesDaySpanRange INT = 28
	DECLARE @NonPromoDaysSpan INT = 56
	DECLARE @NewProductSpan INT = 180
	DECLARE @ProductList TABLE (
		id INT identity(1, 1)
		,OUTP_PRODUCT FLOAT
		,PROD_DEPARTMENT FLOAT
		,PROD_PARENT FLOAT
		,PROD_ALT_SUPPLIER NVARCHAR(3)
		,DailySales FLOAT
		,PROD_UNIT_QTY FLOAT
		,OUTP_QTY_ONHAND FLOAT
		,OUTP_MIN_ONHAND FLOAT
		,OUTP_CARTON_COST FLOAT
		,PROD_CARTON_QTY FLOAT
		,PROD_HOST_NUMBER NVARCHAR(30)
		,PROD_HOST_NUMBER_2 NVARCHAR(30)
		,PROD_HOST_NUMBER_3 NVARCHAR(30)
		,OUTP_MIN_REORDER_QTY FLOAT
		,OUTP_MAX_ONHAND FLOAT
		,SalesTotal FLOAT
		,Is_Processed BIT
		
		)

	DECLARE @NonPromoDailySales FLOAT = 0
	DECLARE @CheckPromoBuy BIT = 0
	DECLARE @ExcludeOrderNum INT

	IF @aDoRecalc = 1
	BEGIN
		SELECT 'NOT TO COVER NOW'
	END
	ELSE IF @vShowOptimalReport = 1
	BEGIN
		SELECT 'NOT TO COVER NOW'
	END
	ELSE
	BEGIN
		SET @ExcludeOrderNum = - 1
		IF @DEBUG = 4 OR @DEBUG = 1 
		BEGIN
		SELECT 'CDN_SP__DoOptimalOrder' CDN_SP__DoOptimalOrder, @aSalesStartDate aSalesStartDate, @aSalesEndDate aSalesEndDate, DateDiff(dd, @aSalesStartDate, @aSalesEndDate)+1 DateDif_PLUS_1, OUTP_PRODUCT
			,PROD_DEPARTMENT
			,PROD_PARENT
			,PROD_ALT_SUPPLIER
			,DailySales
			,PROD_UNIT_QTY
			,OUTP_QTY_ONHAND
			,OUTP_MIN_ONHAND
			,OUTP_CARTON_COST
			,PROD_CARTON_QTY
			,PROD_HOST_NUMBER
			,PROD_HOST_NUMBER_2
			,PROD_HOST_NUMBER_3
			,OUTP_MIN_REORDER_QTY
			,OUTP_MAX_ONHAND
			,SalesTotal
			,0
			
		FROM (
			SELECT @OSS_Outlet Outlet,@OSS_Supplier Supplier,OUTP_PRODUCT
				,PROD_DEPARTMENT
				,PROD_PARENT
				,PROD_ALT_SUPPLIER
				,ISNULL((Sum(TRX_QTY) / (DateDiff(dd, @aSalesStartDate, @aSalesEndDate) +1 )),0) AS DailySales
				,PROD_UNIT_QTY
				,OUTP_QTY_ONHAND
				,OUTP_MIN_ONHAND
				,OUTP_CARTON_COST
				,PROD_CARTON_QTY
				,PROD_HOST_NUMBER
				,PROD_HOST_NUMBER_2
				,PROD_HOST_NUMBER_3
				,Coalesce(OUTP_MIN_REORDER_QTY, 0) AS OUTP_MIN_REORDER_QTY
				,Coalesce(OUTP_MAX_ONHAND, 0) AS OUTP_MAX_ONHAND
				,Sum(TRX_QTY) AS SalesTotal
				
			FROM (
				SELECT OUTP_PRODUCT
					,PROD_DEPARTMENT
					,ISNULL(PROD_PARENT, 0) AS PROD_PARENT
					,PROD_ALT_SUPPLIER
					,OUTP_MIN_ONHAND
					,OUTP_CARTON_COST
					,PROD_CARTON_QTY
					,PROD_HOST_NUMBER
					,PROD_HOST_NUMBER_2
					,PROD_HOST_NUMBER_3
					,OUTP_MIN_REORDER_QTY
					,OUTP_MAX_ONHAND
					,PROD_UNIT_QTY
					,TRX_DEPARTMENT
					,CASE 
						WHEN OUTP_QTY_ONHAND > 0
							THEN OUTP_QTY_ONHAND
						ELSE 0
						END AS OUTP_QTY_ONHAND
					,CASE 
						WHEN TRX_TYPE = 'ITEMSALE'
							THEN SUM(TRX_QTY)
						WHEN TRX_TYPE = 'CHILDSALE'
							THEN CASE 
									WHEN ISNULL(PROD_PARENT, 0) = 0
										THEN - 1 * SUM(TRX_STOCK_MOVEMENT)
									ELSE (- 1 * SUM(TRX_STOCK_MOVEMENT)) * Prod_unit_qty
									END
						WHEN TRX_TYPE = 'WASTAGE'
							THEN CASE 
									WHEN (
											TRX_DEPARTMENT IN (
												SELECT CODE_KEY_NUM
												FROM dbo.CodeTbl
												WHERE Code_Key_Type = 'DEPARTMENT'
													AND CODE_ALP_6 = 'True'
												)
											)
										THEN 0
									ELSE CASE 
											WHEN ISNULL(PROD_PARENT, 0) = 0
												THEN - 1 * SUM(TRX_QTY)
											ELSE (- 1 * SUM(TRX_QTY)) * Prod_unit_qty
											END
									END
						END AS TRX_QTY
				FROM dbo.OUTPTBL WITH (NOLOCK)
				JOIN dbo.PRODTBL WITH (NOLOCK) ON (PROD_NUMBER = OUTP_PRODUCT) AND (OUTP_OUTLET = @OSS_Outlet) AND (OUTP_STATUS = 'Active')
				AND (OUTP_SKIP_REORDER_YN <> 'Y') AND  (@DebugProduct IS NULL OR PROD_NUMBER IN (@DebugProduct))
				
				LEFT JOIN dbo.TRXTBL WITH (NOLOCK) ON TRX_PRODUCT = OUTP_PRODUCT AND (TRX_OUTLET = @OSS_Outlet)
				AND TRX_OUTLET = OUTP_OUTLET AND TRX_DATE BETWEEN @aSalesStartDate AND @aSalesEndDate

				LEFT JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) ON OUTP_OUTLET = BB.Outlet AND OUTP_PRODUCT = BB.Product AND
				BB.Outlet=@OSS_Outlet AND BB.supplier = @OSS_Supplier AND BB.BEST_BUY = 1
				
				WHERE (
						(
							(OUTP_SUPPLIER = @OSS_Supplier OR BB.BEST_BUY=1)							
							AND (TRX_DATE BETWEEN @aSalesStartDate	AND @aSalesEndDate)
						)
						OR 
						(
							(OUTP_MIN_ONHAND > 0) AND (OUTP_MIN_ONHAND > Coalesce(OUTP_QTY_ONHAND, 0))							
							AND ( OUTP_SUPPLIER = @OSS_Supplier OR BB.BEST_BUY = 1)
						)
					 )
					AND (TRX_TYPE IN ('WASTAGE' ,'ITEMSALE','CHILDSALE'))
					---AND (TRX_TYPE IN ('ITEMSALE'))
				GROUP BY OUTP_PRODUCT
					,PROD_DEPARTMENT
					,TRX_TYPE
					,PROD_PARENT
					,PROD_ALT_SUPPLIER
					,OUTP_QTY_ONHAND
					,OUTP_MIN_ONHAND
					,Prod_unit_qty
					,OUTP_CARTON_COST
					,PROD_CARTON_QTY
					,PROD_HOST_NUMBER
					,PROD_HOST_NUMBER_2
					,PROD_HOST_NUMBER_3
					,OUTP_MIN_REORDER_QTY
					,OUTP_MAX_ONHAND
					,TRX_DEPARTMENT
					,TRX_TENDER
				) TheData
			GROUP BY OUTP_PRODUCT
				,PROD_DEPARTMENT
				,PROD_PARENT
				,PROD_ALT_SUPPLIER
				,OUTP_QTY_ONHAND
				,OUTP_MIN_ONHAND
				,OUTP_CARTON_COST
				,PROD_CARTON_QTY
				,PROD_HOST_NUMBER
				,PROD_HOST_NUMBER_2
				,PROD_HOST_NUMBER_3
				,OUTP_MIN_REORDER_QTY
				,OUTP_MAX_ONHAND
				,PROD_UNIT_QTY
			) TEMP
		WHERE SalesTotal IS NULL OR DailySales > 0 OR OUTP_QTY_ONHAND < OUTP_MIN_ONHAND -- including only those items which daily sales > 0 or required Qty On Hand >= MinOnHand 
		ORDER BY PROD_PARENT
			,OUTP_PRODUCT
			IF @DEBUG = 4  RETURN
		END
				
		INSERT INTO @ProductList
		SELECT OUTP_PRODUCT
			,PROD_DEPARTMENT
			,PROD_PARENT
			,PROD_ALT_SUPPLIER
			,DailySales
			,PROD_UNIT_QTY
			,OUTP_QTY_ONHAND
			,OUTP_MIN_ONHAND
			,OUTP_CARTON_COST
			,PROD_CARTON_QTY
			,PROD_HOST_NUMBER
			,PROD_HOST_NUMBER_2
			,PROD_HOST_NUMBER_3
			,OUTP_MIN_REORDER_QTY
			,OUTP_MAX_ONHAND
			,SalesTotal
			,0
		FROM (
			SELECT @OSS_Outlet Outlet,@OSS_Supplier Supplier,OUTP_PRODUCT
				,PROD_DEPARTMENT
				,PROD_PARENT
				,PROD_ALT_SUPPLIER
				,ISNULL((Sum(TRX_QTY) / (DateDiff(dd, @aSalesStartDate, @aSalesEndDate)+1 )),0) AS DailySales
				,PROD_UNIT_QTY
				,OUTP_QTY_ONHAND
				,OUTP_MIN_ONHAND
				,OUTP_CARTON_COST
				,PROD_CARTON_QTY
				,PROD_HOST_NUMBER
				,PROD_HOST_NUMBER_2
				,PROD_HOST_NUMBER_3
				,Coalesce(OUTP_MIN_REORDER_QTY, 0) AS OUTP_MIN_REORDER_QTY
				,Coalesce(OUTP_MAX_ONHAND, 0) AS OUTP_MAX_ONHAND
				,Sum(TRX_QTY) AS SalesTotal
			FROM (
				SELECT OUTP_PRODUCT
					,PROD_DEPARTMENT
					,ISNULL(PROD_PARENT, 0) AS PROD_PARENT
					,PROD_ALT_SUPPLIER
					,OUTP_MIN_ONHAND
					,OUTP_CARTON_COST
					,PROD_CARTON_QTY
					,PROD_HOST_NUMBER
					,PROD_HOST_NUMBER_2
					,PROD_HOST_NUMBER_3
					,OUTP_MIN_REORDER_QTY
					,OUTP_MAX_ONHAND
					,PROD_UNIT_QTY
					,TRX_DEPARTMENT
					,CASE 
						WHEN OUTP_QTY_ONHAND > 0
							THEN OUTP_QTY_ONHAND
						ELSE 0
						END AS OUTP_QTY_ONHAND
					,CASE 
						WHEN TRX_TYPE = 'ITEMSALE'
							THEN SUM(TRX_QTY)
						WHEN TRX_TYPE = 'CHILDSALE'
							THEN CASE 
									WHEN ISNULL(PROD_PARENT, 0) = 0
										THEN - 1 * SUM(TRX_STOCK_MOVEMENT)
									ELSE (- 1 * SUM(TRX_STOCK_MOVEMENT)) * Prod_unit_qty
									END
						WHEN TRX_TYPE = 'WASTAGE'
							THEN CASE 
									WHEN (
											TRX_DEPARTMENT IN (
												SELECT CODE_KEY_NUM
												FROM dbo.CodeTbl
												WHERE Code_Key_Type = 'DEPARTMENT'
													AND CODE_ALP_6 = 'True'
												)
											)
										THEN 0
									ELSE CASE 
											WHEN ISNULL(PROD_PARENT, 0) = 0
												THEN - 1 * SUM(TRX_QTY)
											ELSE (- 1 * SUM(TRX_QTY)) * Prod_unit_qty
											END
									END
						END AS TRX_QTY
				FROM dbo.OUTPTBL WITH (NOLOCK)
				JOIN dbo.PRODTBL WITH (NOLOCK) ON (PROD_NUMBER = OUTP_PRODUCT) AND (OUTP_OUTLET = @OSS_Outlet) 
				AND (OUTP_STATUS = 'Active') AND (OUTP_SKIP_REORDER_YN <> 'Y') AND  (@DebugProduct IS NULL OR PROD_NUMBER IN (@DebugProduct))
				
				LEFT JOIN dbo.TRXTBL WITH (NOLOCK) ON TRX_PRODUCT = OUTP_PRODUCT AND (TRX_OUTLET = @OSS_Outlet)
				AND TRX_OUTLET = OUTP_OUTLET AND TRX_DATE BETWEEN @aSalesStartDate AND @aSalesEndDate
				
				LEFT JOIN [dbo].CDN_BEST_BY_SUPPLIER BB  WITH(NOLOCK) ON OUTP_OUTLET = BB.Outlet AND OUTP_PRODUCT = BB.Product AND
				BB.Outlet=@OSS_Outlet AND BB.supplier = @OSS_Supplier AND BB.BEST_BUY = 1
				
				WHERE (
						(
							(OUTP_SUPPLIER = @OSS_Supplier OR BB.BEST_BUY = 1)							
							AND (TRX_DATE BETWEEN @aSalesStartDate	AND @aSalesEndDate)
						)
						OR 
						(
							(OUTP_MIN_ONHAND > 0) AND (OUTP_MIN_ONHAND > Coalesce(OUTP_QTY_ONHAND, 0))							
							AND ( OUTP_SUPPLIER = @OSS_Supplier OR BB.BEST_BUY = 1)
						)
					 )
					AND (TRX_TYPE IN ('WASTAGE' ,'ITEMSALE','CHILDSALE'))
					--AND (TRX_TYPE IN ('ITEMSALE'))
				GROUP BY OUTP_PRODUCT
					,PROD_DEPARTMENT
					,TRX_TYPE
					,PROD_PARENT
					,PROD_ALT_SUPPLIER
					,OUTP_QTY_ONHAND
					,OUTP_MIN_ONHAND
					,Prod_unit_qty
					,OUTP_CARTON_COST
					,PROD_CARTON_QTY
					,PROD_HOST_NUMBER
					,PROD_HOST_NUMBER_2
					,PROD_HOST_NUMBER_3
					,OUTP_MIN_REORDER_QTY
					,OUTP_MAX_ONHAND
					,TRX_DEPARTMENT
					,TRX_TENDER
				) TheData
			GROUP BY OUTP_PRODUCT
				,PROD_DEPARTMENT
				,PROD_PARENT
				,PROD_ALT_SUPPLIER
				,OUTP_QTY_ONHAND
				,OUTP_MIN_ONHAND
				,OUTP_CARTON_COST
				,PROD_CARTON_QTY
				,PROD_HOST_NUMBER
				,PROD_HOST_NUMBER_2
				,PROD_HOST_NUMBER_3
				,OUTP_MIN_REORDER_QTY
				,OUTP_MAX_ONHAND
				,PROD_UNIT_QTY
			) TEMP
		WHERE SalesTotal IS NULL OR DailySales > 0 OR OUTP_QTY_ONHAND < OUTP_MIN_ONHAND -- including only those items which daily sales > 0 or required Qty On Hand >= MinOnHand 
		ORDER BY PROD_PARENT
			,OUTP_PRODUCT

		DECLARE @ID FLOAT
			,@OUTP_PRODUCT FLOAT
			,@PROD_DEPARTMENT FLOAT
			,@PROD_PARENT FLOAT
			,@PROD_ALT_SUPPLIER NVARCHAR(3)
			,@DailySales FLOAT
			,@PROD_UNIT_QTY FLOAT
			,@OUTP_QTY_ONHAND FLOAT
			,@OUTP_MIN_ONHAND FLOAT
			,@OUTP_CARTON_COST FLOAT
			,@PROD_CARTON_QTY FLOAT
			,@PROD_HOST_NUMBER NVARCHAR(30)
			,@PROD_HOST_NUMBER_2 NVARCHAR(30)
			,@PROD_HOST_NUMBER_3 NVARCHAR(30)
			,@OUTP_MIN_REORDER_QTY FLOAT
			,@OUTP_MAX_ONHAND FLOAT
			,@SalesTotal FLOAT
			,@cnt INT = 1
			,@totalcnt INT

		SELECT @totalcnt = count(*) FROM @ProductList
		IF @Debug = 1 Select @totalcnt total, @OSS_Outlet OUTP_OUTLET 

		IF(@totalcnt=0)
			RETURN

		BEGIN
			--IF @DEBUG = 1 SELECT '[CDN_SP__CreateOptimalOrder]',@aProduct aProduct, @aPromoUnits,@aPromoDailySales ,'SET @aPromoUnits = Round(@aPromoDailySales * 3, 0)'
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

			IF @DEBUG = 1 SELECT '[CDN_SP__CreateOptimalOrder]',@aOrderNum ,'SET @aOrderNum = @orderNo'
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
			

					IF @DEBUG = 1 SELECT '[CDN_SP__CreateOptimalOrder]','SET @aOrderNum = ISNULL(@aNewOrderNum,0)+1', @aOrderNum aOrderNum,@aNewOrderNum aNewOrderNum
				
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
		END



		WHILE @totalcnt >= @cnt
		BEGIN
			IF @DEBUG = 21 SELECT 'fetching product' aCommodityCoverDays,GETDATE() timetaken
			SELECT TOP 1 @ID = Id
				,@OUTP_PRODUCT = OUTP_PRODUCT
				,@PROD_DEPARTMENT = PROD_DEPARTMENT
				,@PROD_PARENT = PROD_PARENT
				,@PROD_ALT_SUPPLIER = PROD_ALT_SUPPLIER
				,@DailySales = DailySales
				,@PROD_UNIT_QTY = PROD_UNIT_QTY
				,@OUTP_QTY_ONHAND = OUTP_QTY_ONHAND
				,@OUTP_MIN_ONHAND = OUTP_MIN_ONHAND
				,@OUTP_CARTON_COST = OUTP_CARTON_COST
				,@PROD_CARTON_QTY = PROD_CARTON_QTY
				,@PROD_HOST_NUMBER = PROD_HOST_NUMBER
				,@PROD_HOST_NUMBER_2 = PROD_HOST_NUMBER_2
				,@PROD_HOST_NUMBER_3 = PROD_HOST_NUMBER_3
				,@OUTP_MIN_REORDER_QTY = OUTP_MIN_REORDER_QTY
				,@OUTP_MAX_ONHAND = OUTP_MAX_ONHAND
				,@SalesTotal = SalesTotal -- comment it
			FROM @ProductList
			WHERE ID = @cnt
			IF @DEBUG = 21 SELECT 'fetched product' aCommodityCoverDays,GETDATE() timetaken


			SET @cnt = @cnt + 1
			
			
			IF (@PROD_UNIT_QTY = 0)
				SET @PROD_UNIT_QTY = 1
			SET @SalesTotal = @SalesTotal * @PROD_UNIT_QTY
			SET @DailySales = @DailySales * @PROD_UNIT_QTY
			
			DECLARE @IsANewProduct BIT = 1

			IF @DEBUG = 21 SELECT 'DoOptimal-WHILE PRODUCT LIST' [WHILE PRODUCT LIST'], GETDATE() timetaken ,@cnt-1 PROCESSING,@totalcnt TOTALOF,@OUTP_PRODUCT OUTP_PRODUCT

			SET @IsANewProduct = CASE 
					WHEN NOT EXISTS (
							SELECT 1
							FROM dbo.TrxTbl WITH (NOLOCK)
							WHERE TRX_DATE < DATEADD(DD, - @NewProductSpan, @aRunDate)
								AND TRX_OUTLET = @OSS_Outlet
								AND TRX_PRODUCT IN (
									@OUTP_PRODUCT
									,@PROD_PARENT
									)
								AND TRX_TYPE = 'ITEMSALE'
							)
						THEN 1
					ELSE 0
					END
			IF @Debug = 1 OR @Debug = 2 
			

			IF @DEBUG = 21 SELECT 'DoOptimal-IsANewProduct' IsANewProduct, GETDATE() timetaken

			
			--- Get First Trx date
			--DECLARE @MinDate DATE 
		 --   SELECT @MinDate = Min(TRX_DATE)
			--FROM dbo.TrxTbl WITH (NOLOCK)
			--WHERE TRX_OUTLET = @OSS_Outlet
			--AND TRX_TYPE = 'ITEMSALE'
		 --   AND TRX_PRODUCT = @OUTP_PRODUCT

			SET @StrSalePromoEndDate = ''

			IF (@IsANewProduct = 1)
			BEGIN
				SET @StrSalePromoEndDate = 'New Prod';
				SET @IsANewProd = 1;

				--DECLARE @NewProdAdjustedSales FLOAT

				------GetNewProdAdjustedSales
				-----SELECT @MinDate = Min(TRX_DATE)
				-----FROM dbo.TrxTbl WITH (NOLOCK)
				----WHERE TRX_OUTLET = @OSS_Outlet
				-----	AND TRX_TYPE = 'ITEMSALE'
				-----	AND TRX_PRODUCT = @OUTP_PRODUCT

				--SET @NonPromoDailySales = 0
				--IF DATEDIFF(DD, @MinDate, @aSalesEndDate) >= 7
				--BEGIN
				----- SET @NewProdAdjustedSales = @SalesTotal / DATEDIFF(DD, @MinDate, @aSalesEndDate);
				--	EXEC dbo.CDN_SP_GetNonPromoSalesCalculation @OUTP_PRODUCT, @PROD_PARENT, @OSS_Outlet, @OSS_Supplier, @aRunDate, @NonPromoDaysSpan,@NonPromoDailySales OUT, @Debug, @MinDate, @IsANewProd
				--END
				--ELSE
				--BEGIN
				--	---SET @NewProdAdjustedSales = @SalesTotal / 7;
				-- EXEC dbo.CDN_SP_GetNonPromoSalesCalculation @OUTP_PRODUCT, @PROD_PARENT, @OSS_Outlet, @OSS_Supplier, @aRunDate, @NonPromoDaysSpan,@NonPromoDailySales OUT, @Debug, @MinDate
    --            END

				--IF (@NewProdAdjustedSales > @DailySales)
				--	SET @DailySales = @NewProdAdjustedSales;

				--IF @Debug = 1 OR @Debug = 2 SELECT @NewProdAdjustedSales NewProdAdjustedSales,@DailySales DailySales, * FROM dbo.TrxTbl WITH (NOLOCK) WHERE TRX_OUTLET = @OSS_Outlet AND TRX_TYPE = 'ITEMSALE' AND TRX_PRODUCT = @OUTP_PRODUCT ORDER BY TRX_DATE
			END
			--ELSE
			---	SET @StrSalePromoEndDate = ''
			BEGIN ------{ Start GetNextWeeksPromoDetails 
				DECLARE @CoverDays INT = 0

				BEGIN ------{ Start GetCoverDays 
					DECLARE @aCommodityCoverDays INT = - 1

					BEGIN ------{ Start GetCommodityCoverDays @OUTP_PRODUCT
						SELECT @aCommodityCoverDays = CODE_NUM_2
						FROM dbo.ProdTbl WITH (NOLOCK)
						JOIN dbo.CodeTbl WITH (NOLOCK) ON CODE_KEY_TYPE = 'Commodity'
							AND PROD_COMMODITY = CODE_KEY_NUM
							AND Prod_Number = @OUTP_PRODUCT

						IF @DEBUG = 21 SELECT 'DoOptimal-aCommodityCoverDays' aCommodityCoverDays,GETDATE() timetaken

						SET @aCommodityCoverDays = ISNULL(@aCommodityCoverDays, - 1)
					END ------} End   GetCommodityCoverDays 			

					IF (@OSS_MultipleOrdersInAWeek = 1)
					BEGIN
						IF (@aCommodityCoverDays > 0)
						BEGIN
							SELECT @CoverDays = CASE 
									WHEN (@OSS_CoverDays < @aCommodityCoverDays)
										THEN @OSS_CoverDays
									ELSE @aCommodityCoverDays
									END
						END
						ELSE
						BEGIN
							SET @CoverDays = @OSS_CoverDays
						END;
					END
					ELSE
					BEGIN
						IF (@aCommodityCoverDays > 0)
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
				END ------} End    GetCoverDays 

				DECLARE @GetSaleTypeCase BIT = 1,@PRMH_PROM_CODE NVARCHAR(30),@PRMP_Promo_Units FLOAT,@PRMH_START DATE,@PRMH_PROMOTION_TYPE NVARCHAR(30),@PRMH_END DATE
				
				SET @CheckPromoBuy = 0
				EXEC dbo.CDN_SP_GetSalePromoCodeStarting @aRunDate,@OSS_Outlet,@OSS_Supplier,@OUTP_PRODUCT,@PROD_PARENT,@CoverDays,@GetSaleTypeCase,@CheckPromoBuy OUT,@PRMH_PROM_CODE OUT,@PRMP_Promo_Units OUT,@PRMH_START OUT,@PRMH_PROMOTION_TYPE OUT,@PRMH_END OUT,@Debug				
				
				IF @DEBUG IN (1,2)
				SELECT @CheckPromoBuy CheckPromoBuy 

				IF @DEBUG = 21 SELECT 'DoOptimal-CDN_SP_GetSalePromoCodeStarting' CDN_SP_GetSalePromoCodeStarting, GETDATE() timetaken
				
				--IF @IsANewProduct = 1
				--	SET @NonPromoDailySales = @NewProdAdjustedSales
				--ELSE 
				--	BEGIN
					
				--	SET @NonPromoDailySales = 0
				--	EXEC dbo.CDN_SP_GetNonPromoSalesCalculation @OUTP_PRODUCT, @PROD_PARENT, @OSS_Outlet, @OSS_Supplier, @aRunDate, @NonPromoDaysSpan,@NonPromoDailySales OUT,@Debug
					
				--	IF @DEBUG = 21 SELECT 'DoOptimal-CDN_fn_GetNonPromoSalesCalculation' CDN_fn_GetNonPromoSalesCalculation, GETDATE() timetaken
				--END

				EXEC dbo.CDN_SP_GetNonPromoSalesCalculation @OUTP_PRODUCT, @PROD_PARENT, @OSS_Outlet, @OSS_Supplier, @aRunDate, @NonPromoDaysSpan,@NonPromoDailySales OUT, @Debug, @IsANewProd
				
				IF @DEBUG = 21 SELECT 'DoOptimal-CDN_fn_GetNonPromoSalesCalculation' CDN_fn_GetNonPromoSalesCalculation, GETDATE() timetaken


			END ------} End   GetNextWeeksPromoDetails 
			
			BEGIN ------{ Start GetPromoDetailsInDateRange     
				DECLARE @PromoUnits FLOAT = 0
					,@aPromoTotalSales FLOAT = 0
					,@aPromoDailySales FLOAT = 0
					,@aSalePromoCode NVARCHAR(100)
					,@aStrSalePromoEndDate NVARCHAR(100)
					,@Result_GetPromoDetailsInDateRange BIT
				
				SET @aPromoDailySales = 0
				SET @aPromoTotalSales = 0

				EXEC dbo.CDN_SP_GetPromoDetailsInDateRange @aRunDate,@OSS_Outlet,@OSS_Supplier,@OUTP_PRODUCT,@PROD_PARENT,@PromoSalesDaySpanRange,@PromoUnits OUT,@aPromoTotalSales OUT,@aPromoDailySales OUT,@aSalePromoCode OUT,@aStrSalePromoEndDate OUT,@Result_GetPromoDetailsInDateRange OUT,@Debug
				
				IF @DEBUG = 21 SELECT 'DoOptimal-CDN_SP_GetPromoDetailsInDateRange' CDN_fn_GetNonPromoSalesCalculation, GETDATE() timetaken


				IF @Result_GetPromoDetailsInDateRange = 1
				BEGIN
					SET @aPromoTotalSales = @aPromoTotalSales * @PROD_UNIT_QTY
					SET @aPromoDailySales = @aPromoDailySales * @PROD_UNIT_QTY
				END
			END ------{ End   GetPromoDetailsInDateRange
			DECLARE @aPromoDailySalesOriginal FLOAT = @aPromoDailySales

			IF @DEBUG = 1 SELECT '[[CDN_SP__DoOptimalOrder]]','sales rate all',@OUTP_PRODUCT OUTP_PRODUCT,@OSS_Outlet OSS_Outlet, @DailySales DailySales,@NonPromoDailySales NonPromoDailySales,@aPromoDailySales aPromoDailySales,@aPromoDailySalesOriginal aPromoDailySalesOriginal
			
			
		---	SET @DailySales = ROUND(@DailySales,2);
		---	SET @NonPromoDailySales = ROUND(@NonPromoDailySales,2);
		---	SET @aPromoDailySales = ROUND(@aPromoDailySales,2);

			SET @DailySales =  FLOOR (@DailySales*100) /100
			SET @NonPromoDailySales =  FLOOR (@NonPromoDailySales*100) /100
			SET @aPromoDailySales =  FLOOR (@aPromoDailySales*100) /100
			

			--IF (@NonPromoDailySales > 0)						
			--	SET @DailySales = @NonPromoDailySales
			--Else
			--	SET @NonPromoDailySales = @DailySales
			
			--IF (@aPromoDailySales < @DailySales)
			--	SET @aPromoDailySales = @DailySales
				 
				IF (@aPromoDailySales <= 0)
				SET @aPromoDailySales = 2 * @NonPromoDailySales
				
				IF (@aPromoDailySales < @NonPromoDailySales)
				SET @aPromoDailySales = @NonPromoDailySales
		

			IF @DEBUG = 21 SELECT 'DoOptimal-sales finding', @DailySales DailySales,@aPromoDailySales PromoDailySales,@OUTP_PRODUCT OUTP_PRODUCT,GETDATE() timetaken
			
			IF @DEBUG = 1 SELECT '[[CDN_SP__DoOptimalOrder]]','sales rate all',@OUTP_PRODUCT OUTP_PRODUCT,@OSS_Outlet OSS_Outlet, @DailySales DailySales,@NonPromoDailySales NonPromoDailySales,@aPromoDailySales aPromoDailySales,@aPromoDailySalesOriginal aPromoDailySalesOriginal
			
			IF @DEBUG = 1 
			SELECT 
				 @aOrderNum						aOrderNum
				,@OUTP_PRODUCT					OUTP_PRODUCT
				,@PROD_PARENT					PROD_PARENT
				,@PROD_ALT_SUPPLIER				PROD_ALT_SUPPLIER	
				,@DailySales					DailySales
				,@OUTP_QTY_ONHAND				OUTP_QTY_ONHAND
				,@OUTP_MIN_ONHAND				OUTP_MIN_ONHAND
				,@OUTP_CARTON_COST				OUTP_CARTON_COST
				,@PROD_CARTON_QTY				PROD_CARTON_QTY
				,@OUTP_MIN_REORDER_QTY			OUTP_MIN_REORDER_QTY
				,@ExcludeOrderNum				ExcludeOrderNum
				,@aRunDate						aRunDate
				,@aDoRecalc						aDoRecalc
				,@OUTP_MAX_ONHAND				OUTP_MAX_ONHAND
				,@SalesTotal					SalesTotal
				,@aProdList						aProdList
				,@PromoUnits					PromoUnits
				,@aPromoDailySales				aPromoDailySales
				,@aPromoTotalSales				aPromoTotalSales
				,@SalePromoCode					SalePromoCode
				,@StrSalePromoEndDate			StrSalePromoEndDate
				,@IsANewProd					IsANewProd
				,@aSalesEndDate					aSalesEndDate
				,@OSS_Id						OSS_Id
				,@PROD_DEPARTMENT				PROD_DEPARTMENT
				,@CheckPromoBuy					CheckPromoBuy
				,@NonPromoDailySales			NonPromoDailySales
				,@aPromoDailySalesOriginal 		aPromoDailySalesOriginal 
				,@CreateOptimalOrder_Result 	CreateOptimalOrder_Result 
				,@Debug 						Debug 
				
			EXEC dbo.CDN_SP__CreateOptimalOrder 
				@aOrderNum OUT
				,@OUTP_PRODUCT
				,@PROD_PARENT
				,@PROD_ALT_SUPPLIER
				,@DailySales
				,@OUTP_QTY_ONHAND
				,@OUTP_MIN_ONHAND
				,@OUTP_CARTON_COST
				,@PROD_CARTON_QTY
				,@OUTP_MIN_REORDER_QTY
				,@ExcludeOrderNum
				,@aRunDate
				,@aDoRecalc
				,@OUTP_MAX_ONHAND
				,@SalesTotal
				,@aProdList
				,@PromoUnits
				,@aPromoDailySales
				,@aPromoTotalSales
				,@SalePromoCode
				,@StrSalePromoEndDate
				,@IsANewProd
				,@aSalesEndDate
				,@OSS_Id
				,@OSS_Outlet
				,@OSS_DOWGenerateOrder
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
				,@PROD_DEPARTMENT
				,@CheckPromoBuy
				,@NonPromoDailySales
				,@aPromoDailySalesOriginal 
				,@CreateOptimalOrder_Result OUT
				,@Debug = @Debug 

				IF @DEBUG = 21 SELECT 'DoOptimal-CDN_SP__CreateOptimalOrder after call' CDN_fn_GetNonPromoSalesCalculation, GETDATE() timetaken

		END
	END
END TRY
BEGIN CATCH
    INSERT INTO [dbo].[CDN_AOO_Errors] 
				 (UserName,		ErrorNumber,	ErrorState,		ErrorSeverity,	 ErrorLine,		ErrorProcedure,		ErrorMessage,	ErrorDateTime,
				 Outlet,	 Supplier,		Product,Detail,Detail2)
    VALUES		 (SUSER_SNAME(),ERROR_NUMBER(), ERROR_STATE(),  ERROR_SEVERITY(),ERROR_LINE(),  ERROR_PROCEDURE(),  ERROR_MESSAGE(),GETDATE(),
				 @OSS_Outlet,@OSS_Supplier, @OUTP_PRODUCT,'CDN_SP__DoOptimalOrder',NULL);
END CATCH